using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public static class IndirectMeshRenderer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void _Init()
	{
		IndirectMeshRenderer._DisposeAll();
		IndirectMeshRenderer._shader = Shader.Find("GorillaTag/IndirectLit");
		if (IndirectMeshRenderer._shader == null)
		{
			Debug.LogError("[IndirectMeshRenderer] Shader 'GorillaTag/IndirectLit' not found. Add it to Always Included Shaders.");
		}
		IndirectMeshRenderer._shaderEmissive = Shader.Find("GorillaTag/IndirectLitEmissive");
		if (IndirectMeshRenderer._shaderEmissive == null)
		{
			Debug.LogError("[IndirectMeshRenderer] Shader 'GorillaTag/IndirectLitEmissive' not found. Add it to Always Included Shaders.");
		}
		Application.quitting += IndirectMeshRenderer._DisposeAll;
		TickSystem<object>.AddPostTickCallback(new IndirectMeshRenderer.PostTickCallback());
	}

	public static void Register(IndirectMeshInstance inst, int groupId = 0)
	{
		Mesh sharedMesh = inst.meshFilter.sharedMesh;
		if (sharedMesh.subMeshCount > 1)
		{
			Debug.LogError(string.Format("[IndirectMeshRenderer] Mesh '{0}' on '{1}' has {2} submeshes ", sharedMesh.name, inst.name, sharedMesh.subMeshCount) + "(likely from static batching). Disable Static on objects with IndirectMeshInstance.", inst);
			return;
		}
		Material sharedMaterial = inst.meshRenderer.sharedMaterial;
		Texture texture = sharedMaterial.HasTexture(ShaderProps._BaseMap) ? sharedMaterial.GetTexture(ShaderProps._BaseMap) : null;
		bool flag = sharedMaterial.IsKeywordEnabled("_EMISSION");
		Shader shader = flag ? IndirectMeshRenderer._shaderEmissive : IndirectMeshRenderer._shader;
		IndirectMeshRenderer.BatchKey key = new IndirectMeshRenderer.BatchKey
		{
			meshId = sharedMesh.GetInstanceID(),
			textureId = ((texture != null) ? texture.GetInstanceID() : 0),
			shaderId = shader.GetInstanceID()
		};
		int count;
		if (!IndirectMeshRenderer._batchLookup.TryGetValue(key, out count))
		{
			IndirectMeshRenderer.DrawBatch drawBatch = new IndirectMeshRenderer.DrawBatch
			{
				mesh = sharedMesh,
				submeshCount = sharedMesh.subMeshCount,
				layer = inst.gameObject.layer,
				matrices = new NativeList<Matrix4x4>(2048, Allocator.Persistent),
				groupIds = new NativeList<int>(2048, Allocator.Persistent),
				visibility = new NativeList<byte>(2048, Allocator.Persistent),
				material = new Material(shader)
				{
					name = sharedMaterial.name + " (Indirect)"
				}
			};
			if (texture != null)
			{
				drawBatch.material.SetTexture(ShaderProps._BaseMap, texture);
			}
			if (sharedMaterial.HasColor(ShaderProps._BaseColor))
			{
				drawBatch.material.SetColor(ShaderProps._BaseColor, sharedMaterial.GetColor(ShaderProps._BaseColor));
			}
			if (flag)
			{
				IndirectMeshRenderer._CopyEmissionProperties(drawBatch.material, sharedMaterial);
			}
			count = IndirectMeshRenderer._batchList.Count;
			IndirectMeshRenderer._batchLookup[key] = count;
			IndirectMeshRenderer._batchList.Add(drawBatch);
			Debug.Log(string.Format("[IndirectMeshRenderer] New batch #{0}: mesh='{1}' tex='{2}' shader='{3}' layer={4} submeshes={5}", new object[]
			{
				count,
				sharedMesh.name,
				(texture != null) ? texture.name : "null",
				shader.name,
				inst.gameObject.layer,
				sharedMesh.subMeshCount
			}));
		}
		IndirectMeshRenderer.DrawBatch drawBatch2 = IndirectMeshRenderer._batchList[count];
		int length = drawBatch2.matrices.Length;
		Matrix4x4 localToWorldMatrix = inst.transform.localToWorldMatrix;
		drawBatch2.matrices.Add(localToWorldMatrix);
		drawBatch2.groupIds.Add(groupId);
		byte b = 1;
		drawBatch2.visibility.Add(b);
		drawBatch2.visibleCount++;
		drawBatch2.dirty = true;
		if (inst.dynamic)
		{
			ref List<IndirectMeshRenderer.DynamicEntry> ptr = ref drawBatch2.dynamicEntries;
			if (ptr == null)
			{
				ptr = new List<IndirectMeshRenderer.DynamicEntry>();
			}
			drawBatch2.dynamicEntries.Add(new IndirectMeshRenderer.DynamicEntry
			{
				transform = inst.transform,
				matrixIndex = length
			});
		}
		IndirectMeshRenderer._batchList[count] = drawBatch2;
	}

	private static void _CopyEmissionProperties(Material dst, Material src)
	{
		if (src.HasTexture(ShaderProps._EmissionMap))
		{
			dst.SetTexture(ShaderProps._EmissionMap, src.GetTexture(ShaderProps._EmissionMap));
		}
		if (src.HasColor(ShaderProps._EmissionColor))
		{
			dst.SetColor(ShaderProps._EmissionColor, src.GetColor(ShaderProps._EmissionColor));
		}
		if (src.HasVector(ShaderProps._EmissionUVScrollSpeed))
		{
			dst.SetVector(ShaderProps._EmissionUVScrollSpeed, src.GetVector(ShaderProps._EmissionUVScrollSpeed));
		}
		if (src.HasFloat(ShaderProps._EmissionDissolveEdgeSize))
		{
			dst.SetFloat(ShaderProps._EmissionDissolveEdgeSize, src.GetFloat(ShaderProps._EmissionDissolveEdgeSize));
		}
		if (src.HasFloat(ShaderProps._EmissionDissolveProgress))
		{
			dst.SetFloat(ShaderProps._EmissionDissolveProgress, src.GetFloat(ShaderProps._EmissionDissolveProgress));
		}
		if (src.HasVector(ShaderProps._EmissionDissolveAnimation))
		{
			dst.SetVector(ShaderProps._EmissionDissolveAnimation, src.GetVector(ShaderProps._EmissionDissolveAnimation));
		}
		if (src.HasFloat(ShaderProps._EmissionMaskByBaseMapAlpha))
		{
			dst.SetFloat(ShaderProps._EmissionMaskByBaseMapAlpha, src.GetFloat(ShaderProps._EmissionMaskByBaseMapAlpha));
		}
	}

	public static void SetGroupVisible(int groupId, bool visible)
	{
		byte b = visible ? 1 : 0;
		for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
		{
			IndirectMeshRenderer.DrawBatch value = IndirectMeshRenderer._batchList[i];
			bool flag = false;
			int length = value.groupIds.Length;
			for (int j = 0; j < length; j++)
			{
				if (value.groupIds[j] == groupId && value.visibility[j] != b)
				{
					value.visibility[j] = b;
					value.visibleCount += (visible ? 1 : -1);
					flag = true;
				}
			}
			if (flag)
			{
				value.dirty = true;
				IndirectMeshRenderer._batchList[i] = value;
			}
		}
	}

	private static void _Render()
	{
		if (IndirectMeshRenderer._batchList.Count == 0)
		{
			return;
		}
		if (!IndirectMeshRenderer._loggedFirstRender)
		{
			IndirectMeshRenderer._loggedFirstRender = true;
			int num = 0;
			for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
			{
				num += IndirectMeshRenderer._batchList[i].visibleCount;
			}
			Debug.Log(string.Format("[IndirectMeshRenderer] First render: {0} batch(es), {1} visible instance(s), stereoMul={2}", IndirectMeshRenderer._batchList.Count, num, 2));
		}
		for (int j = 0; j < IndirectMeshRenderer._batchList.Count; j++)
		{
			IndirectMeshRenderer.DrawBatch drawBatch = IndirectMeshRenderer._batchList[j];
			if (drawBatch.dynamicEntries != null)
			{
				for (int k = drawBatch.dynamicEntries.Count - 1; k >= 0; k--)
				{
					IndirectMeshRenderer.DynamicEntry dynamicEntry = drawBatch.dynamicEntries[k];
					if (dynamicEntry.transform == null)
					{
						if (drawBatch.visibility[dynamicEntry.matrixIndex] != 0)
						{
							drawBatch.visibility[dynamicEntry.matrixIndex] = 0;
							drawBatch.visibleCount--;
							drawBatch.dirty = true;
						}
						int index = drawBatch.dynamicEntries.Count - 1;
						drawBatch.dynamicEntries[k] = drawBatch.dynamicEntries[index];
						drawBatch.dynamicEntries.RemoveAt(index);
					}
					else
					{
						drawBatch.matrices[dynamicEntry.matrixIndex] = dynamicEntry.transform.localToWorldMatrix;
					}
				}
				if (!drawBatch.dirty && drawBatch.dynamicEntries.Count > 0)
				{
					drawBatch.needsUpload = true;
				}
			}
			if (drawBatch.visibleCount == 0)
			{
				if (drawBatch.dirty)
				{
					IndirectMeshRenderer._DisposeBatchBuffers(ref drawBatch);
					drawBatch.dirty = false;
					drawBatch.needsUpload = false;
				}
				IndirectMeshRenderer._batchList[j] = drawBatch;
			}
			else
			{
				if (drawBatch.dirty)
				{
					IndirectMeshRenderer._RebuildBatch(ref drawBatch);
				}
				else if (drawBatch.needsUpload)
				{
					IndirectMeshRenderer._UploadBatch(ref drawBatch);
				}
				IndirectMeshRenderer._batchList[j] = drawBatch;
				Graphics.RenderMeshIndirect(drawBatch.renderParams, drawBatch.mesh, drawBatch.commandBuffer, drawBatch.submeshCount, 0);
			}
		}
	}

	private static void _RebuildBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		int length = batch.matrices.Length;
		int num = batch.visibleCount * 2;
		IndirectMeshRenderer._DisposeBatchBuffers(ref batch);
		batch.matrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, num, 64);
		batch.commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, batch.submeshCount, 20);
		if (!batch.gpuMatrices.IsCreated || batch.gpuMatrices.Length < num)
		{
			if (batch.gpuMatrices.IsCreated)
			{
				batch.gpuMatrices.Dispose();
			}
			batch.gpuMatrices = new NativeArray<Matrix4x4>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		int num2 = 0;
		for (int i = 0; i < length; i++)
		{
			if (batch.visibility[i] != 0)
			{
				Matrix4x4 matrix4x = batch.matrices[i];
				Vector3 rhs = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
				vector = Vector3.Min(vector, rhs);
				vector2 = Vector3.Max(vector2, rhs);
				int num3 = num2 * 2;
				batch.gpuMatrices[num3] = matrix4x;
				batch.gpuMatrices[num3 + 1] = matrix4x;
				num2++;
			}
		}
		batch.matrixBuffer.SetData<Matrix4x4>(batch.gpuMatrices, 0, 0, num);
		Vector3 b = Vector3.one * 10f;
		Bounds worldBounds = new Bounds((vector + vector2) * 0.5f, vector2 - vector + b);
		if (!batch.commandData.IsCreated || batch.commandData.Length != batch.submeshCount)
		{
			if (batch.commandData.IsCreated)
			{
				batch.commandData.Dispose();
			}
			batch.commandData = new NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs>(batch.submeshCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
		for (int j = 0; j < batch.submeshCount; j++)
		{
			batch.commandData[j] = new GraphicsBuffer.IndirectDrawIndexedArgs
			{
				indexCountPerInstance = batch.mesh.GetIndexCount(j),
				startIndex = batch.mesh.GetIndexStart(j),
				baseVertexIndex = batch.mesh.GetBaseVertex(j),
				startInstance = 0U,
				instanceCount = (uint)num
			};
		}
		batch.commandBuffer.SetData<GraphicsBuffer.IndirectDrawIndexedArgs>(batch.commandData);
		batch.renderParams = new RenderParams(batch.material)
		{
			worldBounds = worldBounds,
			layer = batch.layer,
			shadowCastingMode = ShadowCastingMode.Off,
			receiveShadows = false,
			matProps = new MaterialPropertyBlock()
		};
		batch.renderParams.matProps.SetBuffer(IndirectMeshRenderer._spId_Matrices, batch.matrixBuffer);
		batch.dirty = false;
		batch.needsUpload = false;
	}

	private static void _UploadBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		int length = batch.matrices.Length;
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			if (batch.visibility[i] != 0)
			{
				Matrix4x4 matrix4x = batch.matrices[i];
				Vector3 rhs = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
				vector = Vector3.Min(vector, rhs);
				vector2 = Vector3.Max(vector2, rhs);
				int num2 = num * 2;
				batch.gpuMatrices[num2] = matrix4x;
				batch.gpuMatrices[num2 + 1] = matrix4x;
				num++;
			}
		}
		batch.matrixBuffer.SetData<Matrix4x4>(batch.gpuMatrices, 0, 0, num * 2);
		Vector3 b = Vector3.one * 10f;
		batch.renderParams.worldBounds = new Bounds((vector + vector2) * 0.5f, vector2 - vector + b);
		batch.needsUpload = false;
	}

	private static void _DisposeBatchBuffers(ref IndirectMeshRenderer.DrawBatch batch)
	{
		GraphicsBuffer matrixBuffer = batch.matrixBuffer;
		if (matrixBuffer != null)
		{
			matrixBuffer.Dispose();
		}
		batch.matrixBuffer = null;
		GraphicsBuffer commandBuffer = batch.commandBuffer;
		if (commandBuffer != null)
		{
			commandBuffer.Dispose();
		}
		batch.commandBuffer = null;
	}

	private static void _DisposeBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		IndirectMeshRenderer._DisposeBatchBuffers(ref batch);
		if (batch.matrices.IsCreated)
		{
			batch.matrices.Dispose();
		}
		if (batch.groupIds.IsCreated)
		{
			batch.groupIds.Dispose();
		}
		if (batch.visibility.IsCreated)
		{
			batch.visibility.Dispose();
		}
		if (batch.gpuMatrices.IsCreated)
		{
			batch.gpuMatrices.Dispose();
		}
		if (batch.commandData.IsCreated)
		{
			batch.commandData.Dispose();
		}
		if (batch.material != null)
		{
			Object.Destroy(batch.material);
		}
		batch.dynamicEntries = null;
	}

	private static void _DisposeAll()
	{
		for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
		{
			IndirectMeshRenderer.DrawBatch drawBatch = IndirectMeshRenderer._batchList[i];
			IndirectMeshRenderer._DisposeBatch(ref drawBatch);
		}
		IndirectMeshRenderer._batchList.Clear();
		IndirectMeshRenderer._batchLookup.Clear();
	}

	private const string SHADER_NAME = "GorillaTag/IndirectLit";

	private const string SHADER_NAME_EMISSIVE = "GorillaTag/IndirectLitEmissive";

	private const int _k_instancesPerXform = 2;

	private static readonly int _spId_Matrices = Shader.PropertyToID("_Matrices");

	private static Shader _shader;

	private static Shader _shaderEmissive;

	private static readonly Dictionary<IndirectMeshRenderer.BatchKey, int> _batchLookup = new Dictionary<IndirectMeshRenderer.BatchKey, int>();

	private static readonly List<IndirectMeshRenderer.DrawBatch> _batchList = new List<IndirectMeshRenderer.DrawBatch>();

	private static bool _loggedFirstRender;

	private struct BatchKey : IEquatable<IndirectMeshRenderer.BatchKey>
	{
		public bool Equals(IndirectMeshRenderer.BatchKey other)
		{
			return this.meshId == other.meshId && this.textureId == other.textureId && this.shaderId == other.shaderId;
		}

		public override int GetHashCode()
		{
			return (this.meshId * 397 ^ this.textureId) * 397 ^ this.shaderId;
		}

		public override bool Equals(object obj)
		{
			if (obj is IndirectMeshRenderer.BatchKey)
			{
				IndirectMeshRenderer.BatchKey other = (IndirectMeshRenderer.BatchKey)obj;
				return this.Equals(other);
			}
			return false;
		}

		public int meshId;

		public int textureId;

		public int shaderId;
	}

	private struct DynamicEntry
	{
		public Transform transform;

		public int matrixIndex;
	}

	private struct DrawBatch
	{
		public Mesh mesh;

		public Material material;

		public int submeshCount;

		public int layer;

		public NativeList<Matrix4x4> matrices;

		public NativeList<int> groupIds;

		public NativeList<byte> visibility;

		public int visibleCount;

		public NativeArray<Matrix4x4> gpuMatrices;

		public GraphicsBuffer matrixBuffer;

		public GraphicsBuffer commandBuffer;

		public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

		public RenderParams renderParams;

		public bool dirty;

		public bool needsUpload;

		public List<IndirectMeshRenderer.DynamicEntry> dynamicEntries;
	}

	private sealed class PostTickCallback : ITickSystemPost
	{
		public bool PostTickRunning { get; set; }

		public void PostTick()
		{
			IndirectMeshRenderer._Render();
		}
	}
}
