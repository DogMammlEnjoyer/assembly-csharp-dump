using System;
using System.Collections.Generic;
using GorillaExtensions;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace GorillaTag.Rendering
{
	[DefaultExecutionOrder(-2147482648)]
	public class EdMeshCombinerPrefab : MonoBehaviour
	{
		private void Awake()
		{
			if (this.combinedData == null)
			{
				this.combinedData = new EdMeshCombinedPrefabData();
			}
			EdMeshCombinerPrefab.CombineMeshesRuntime(this, false, this.combinedData);
		}

		private static void Special_MarkDoNotCombine(Component component)
		{
			if (component != null)
			{
				GameObject gameObject = component.gameObject;
				if (gameObject.GetComponent<EdDoNotMeshCombine>() == null)
				{
					gameObject.AddComponent<EdDoNotMeshCombine>();
				}
			}
		}

		public static void CombineMeshesRuntime(EdMeshCombinerPrefab combiner, bool undo = false, EdMeshCombinedPrefabData combinedPrefabData = null)
		{
			bool flag = true;
			foreach (Campfire campfire in combiner.GetComponentsInChildren<Campfire>(true))
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(campfire.baseFire);
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(campfire.middleFire);
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(campfire.topFire);
			}
			GameEntity[] componentsInChildren2 = combiner.GetComponentsInChildren<GameEntity>(true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren2[i]);
			}
			StaticLodGroup[] componentsInChildren3 = combiner.GetComponentsInChildren<StaticLodGroup>(true);
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren3[i]);
			}
			GorillaCaveCrystalVisuals[] componentsInChildren4 = combiner.GetComponentsInChildren<GorillaCaveCrystalVisuals>(false);
			for (int i = 0; i < componentsInChildren4.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren4[i]);
			}
			WaterSurfaceMaterialController[] componentsInChildren5 = combiner.GetComponentsInChildren<WaterSurfaceMaterialController>(false);
			for (int i = 0; i < componentsInChildren5.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren5[i]);
			}
			List<Renderer> componentsInChildrenUntil = combiner.GetComponentsInChildrenUntil(false, false, 64);
			List<Renderer> list = new List<Renderer>(componentsInChildrenUntil.Count);
			foreach (Renderer renderer in componentsInChildrenUntil)
			{
				if (renderer is SkinnedMeshRenderer || renderer is MeshRenderer)
				{
					list.Add(renderer);
				}
			}
			Dictionary<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>> dictionary = new Dictionary<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>>(list.Count);
			List<Transform> list2 = new List<Transform>(list.Count);
			foreach (Renderer renderer2 in list)
			{
				if (renderer2.enabled)
				{
					GameObject gameObject = renderer2.gameObject;
					int staticFlags = gameObject.isStatic ? 1 : 0;
					if (gameObject.isStatic)
					{
						SkinnedMeshRenderer skinnedMeshRenderer = renderer2 as SkinnedMeshRenderer;
						bool flag2 = skinnedMeshRenderer != null;
						MeshFilter meshFilter = null;
						Mesh sharedMesh;
						if (flag2)
						{
							sharedMesh = skinnedMeshRenderer.sharedMesh;
						}
						else
						{
							meshFilter = renderer2.GetComponent<MeshFilter>();
							if (meshFilter == null)
							{
								continue;
							}
							sharedMesh = meshFilter.sharedMesh;
						}
						if (!(sharedMesh == null) && (long)sharedMesh.vertexCount < 65535L)
						{
							MeshCollider component = renderer2.GetComponent<MeshCollider>();
							bool flag3 = component != null;
							if (flag || !flag3 || (!(component.sharedMesh == null) && !component.convex && !(component.sharedMesh != sharedMesh)))
							{
								GorillaSurfaceOverride component2 = renderer2.GetComponent<GorillaSurfaceOverride>();
								int num = (component2 != null) ? component2.overrideIndex : 0;
								int num2 = Mathf.Min(renderer2.sharedMaterials.Length, sharedMesh.subMeshCount);
								if (num2 != 0)
								{
									int num3 = 0;
									int num4 = 0;
									for (int j = 0; j < num2; j++)
									{
										num3 += ((sharedMesh.GetSubMesh(j).topology != MeshTopology.Triangles) ? 1 : 0);
										num4 += ((renderer2.sharedMaterials[j] == null) ? 1 : 0);
									}
									if (num3 > 0)
									{
										string text = "?????";
										Debug.LogError(string.Concat(new string[]
										{
											string.Format("Cannot combine mesh \"{0}\" because it has {1} submeshes with ", sharedMesh.name, num3),
											"a non-triangle topology. Verify FBX import settings does not have \"Keep Quads\" on.\n  - Asset path=\"",
											text,
											"\"\n  - Path in scene=",
											renderer2.transform.GetPathQ()
										}), sharedMesh);
									}
									else if (num4 > 0)
									{
										Debug.LogError("EdMeshCombinerPrefab: Cannot combine Renderer \"" + combiner.name + "\" because it does not have " + string.Format("{0} materials assigned. Path in scene={1}", num4, combiner.transform.GetPathQ()), combiner);
									}
									else
									{
										for (int k = 0; k < num2; k++)
										{
											Material mat = renderer2.sharedMaterials[k];
											int layer = renderer2.gameObject.layer;
											EdMeshCombinerPrefab.CombinerCriteria combinerCriteria = new EdMeshCombinerPrefab.CombinerCriteria
											{
												mat = mat,
												staticFlags = staticFlags,
												lightmapIndex = renderer2.lightmapIndex,
												hasMeshCollider = (!flag && flag3),
												meshCollPhysicsMat = (flag ? null : (flag3 ? component.sharedMaterial : null)),
												surfOverrideIndex = (flag ? 0 : num),
												surfExtraVelMultiplier = (flag ? 0f : ((component2 != null) ? component2.extraVelMultiplier : 1f)),
												surfExtraVelMaxMultiplier = (flag ? 0f : ((component2 != null) ? component2.extraVelMaxMultiplier : 1f)),
												surfSendOnTapEvent = (!flag && component2 != null && component2.sendOnTapEvent),
												objectLayer = ((layer == 27) ? UnityLayer.NoMirror : UnityLayer.Default)
											};
											EdMeshCombinerPrefab.CombinerCriteria key = combinerCriteria;
											List<List<EdMeshCombinerPrefab.CombinerInfo>> list3;
											if (!dictionary.TryGetValue(key, out list3))
											{
												list3 = new List<List<EdMeshCombinerPrefab.CombinerInfo>>
												{
													new List<EdMeshCombinerPrefab.CombinerInfo>(1)
												};
												dictionary[key] = list3;
											}
											int index = list3.Count - 1;
											int num5 = sharedMesh.vertexCount;
											foreach (EdMeshCombinerPrefab.CombinerInfo combinerInfo in list3[index])
											{
												if (combinerInfo.isSkinnedMesh)
												{
													SkinnedMeshRenderer skinnedMeshRenderer2 = (SkinnedMeshRenderer)combinerInfo.renderer;
													num5 += skinnedMeshRenderer2.sharedMesh.vertexCount;
												}
												else
												{
													num5 += combinerInfo.meshFilter.sharedMesh.vertexCount;
												}
											}
											if ((long)num5 >= 65535L)
											{
												index = list3.Count;
												list3.Add(new List<EdMeshCombinerPrefab.CombinerInfo>(1));
											}
											list2.Add(gameObject.transform);
											list3[index].Add(new EdMeshCombinerPrefab.CombinerInfo
											{
												meshFilter = meshFilter,
												renderer = renderer2,
												uvOffsetModifier = renderer2.GetComponent<EdMeshCombinerModifierUVOffset>(),
												subMeshIndex = k,
												isSkinnedMesh = flag2,
												layer = renderer2.sortingLayerID
											});
										}
									}
								}
							}
						}
					}
				}
			}
			Matrix4x4 worldToLocalMatrix = combiner.transform.worldToLocalMatrix;
			PerSceneRenderData perSceneRenderData = null;
			bool flag4 = false;
			new Unity.Mathematics.Random(6746U);
			foreach (KeyValuePair<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>> keyValuePair in dictionary)
			{
				EdMeshCombinerPrefab.CombinerCriteria combinerCriteria;
				List<List<EdMeshCombinerPrefab.CombinerInfo>> list4;
				keyValuePair.Deconstruct(out combinerCriteria, out list4);
				EdMeshCombinerPrefab.CombinerCriteria combinerCriteria2 = combinerCriteria;
				List<List<EdMeshCombinerPrefab.CombinerInfo>> list5 = list4;
				bool isCandleFlame = false;
				foreach (List<EdMeshCombinerPrefab.CombinerInfo> list6 in list5)
				{
					List<Mesh> list7 = new List<Mesh>(list6.Count);
					List<int> list8 = new List<int>(list6.Count);
					List<Matrix4x4> list9 = new List<Matrix4x4>(list6.Count);
					List<Color> list10 = new List<Color>(list6.Count);
					List<int> list11 = new List<int>(list6.Count);
					List<float4> list12 = new List<float4>(list6.Count);
					List<float4> list13 = new List<float4>(list6.Count);
					Dictionary<ValueTuple<Renderer, int>, ValueTuple<Color, int>> dictionary2 = new Dictionary<ValueTuple<Renderer, int>, ValueTuple<Color, int>>();
					foreach (EdMeshCombinerPrefab.CombinerInfo combinerInfo2 in list6)
					{
						MaterialCombinerPerRendererMono materialCombinerPerRendererMono;
						MaterialCombinerPerRendererInfo materialCombinerPerRendererInfo;
						if (combinerInfo2.renderer.TryGetComponent<MaterialCombinerPerRendererMono>(out materialCombinerPerRendererMono) && materialCombinerPerRendererMono.TryGetData(combinerInfo2.renderer, combinerInfo2.subMeshIndex, out materialCombinerPerRendererInfo))
						{
							dictionary2[new ValueTuple<Renderer, int>(combinerInfo2.renderer, combinerInfo2.subMeshIndex)] = new ValueTuple<Color, int>(materialCombinerPerRendererInfo.baseColor, materialCombinerPerRendererInfo.sliceIndex);
						}
						else
						{
							dictionary2[new ValueTuple<Renderer, int>(combinerInfo2.renderer, combinerInfo2.subMeshIndex)] = new ValueTuple<Color, int>(Color.white, -1);
						}
					}
					for (int l = 0; l < list6.Count; l++)
					{
						EdMeshCombinerPrefab.CombinerInfo combinerInfo3 = list6[l];
						Mesh mesh;
						if (combinerInfo3.isSkinnedMesh)
						{
							SkinnedMeshRenderer skinnedMeshRenderer3 = (SkinnedMeshRenderer)combinerInfo3.renderer;
							mesh = new Mesh();
							skinnedMeshRenderer3.BakeMesh(mesh, true);
						}
						else
						{
							mesh = combinerInfo3.meshFilter.sharedMesh;
						}
						if (mesh.vertexCount != 0)
						{
							if (perSceneRenderData != null && perSceneRenderData.representativeRenderer == combinerInfo3.renderer)
							{
								flag4 = true;
							}
							list7.Add(mesh);
							list8.Add(combinerInfo3.subMeshIndex);
							list9.Add(worldToLocalMatrix * combinerInfo3.renderer.transform.localToWorldMatrix);
							list13.Add((combinerInfo3.uvOffsetModifier == null) ? float4.zero : new float4(combinerInfo3.uvOffsetModifier.minUvOffset.x, combinerInfo3.uvOffsetModifier.minUvOffset.y, combinerInfo3.uvOffsetModifier.maxUvOffset.x, combinerInfo3.uvOffsetModifier.maxUvOffset.y));
							list12.Add(combinerInfo3.renderer.lightmapScaleOffset);
							ValueTuple<Color, int> valueTuple = dictionary2[new ValueTuple<Renderer, int>(combinerInfo3.renderer, combinerInfo3.subMeshIndex)];
							Color item = valueTuple.Item1;
							int item2 = valueTuple.Item2;
							list10.Add(item);
							list11.Add(item2);
						}
					}
					using (Mesh.MeshDataArray meshDataArray = Mesh.AcquireReadOnlyMeshData(list7))
					{
						int num6 = 0;
						int num7 = 0;
						for (int m = 0; m < meshDataArray.Length; m++)
						{
							Mesh.MeshData meshData = meshDataArray[m];
							num6 += meshData.vertexCount;
							num7 += meshData.GetSubMesh(list8[m]).indexCount;
						}
						Mesh.MeshDataArray data = Mesh.AllocateWritableMeshData(1);
						Mesh.MeshData meshData2 = data[0];
						IndexFormat indexFormat = (num6 > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16;
						GTVertexDataStreams_Descriptors.DoSetVertexBufferParams(ref meshData2, num6);
						meshData2.SetIndexBufferParams(num7, indexFormat);
						meshData2.subMeshCount = 1;
						NativeArray<int> idxDst = default(NativeArray<int>);
						NativeArray<ushort> idxDst2 = default(NativeArray<ushort>);
						if (indexFormat == IndexFormat.UInt32)
						{
							idxDst = meshData2.GetIndexData<int>();
						}
						else
						{
							idxDst2 = meshData2.GetIndexData<ushort>();
						}
						EdMeshCombinerPrefab.CopyMeshJob jobData = new EdMeshCombinerPrefab.CopyMeshJob
						{
							meshDataArray = meshDataArray,
							sourceSubmeshIndices = new NativeArray<int>(list8.ToArray(), Allocator.TempJob),
							sourceTransforms = new NativeArray<Matrix4x4>(list9.ToArray(), Allocator.TempJob),
							lightmapScaleOffsets = new NativeArray<float4>(list12.ToArray(), Allocator.TempJob),
							baseColors = new NativeArray<Color>(list10.ToArray(), Allocator.TempJob),
							atlasSlices = new NativeArray<int>(list11.ToArray(), Allocator.TempJob),
							uvModifiersMinMax = new NativeArray<float4>(list13.ToArray(), Allocator.TempJob),
							isCandleFlame = isCandleFlame,
							randSeed = 6746U,
							dst0 = meshData2.GetVertexData<GTVertexDataStream0>(0),
							dst1 = meshData2.GetVertexData<GTVertexDataStream1>(1),
							idxDst32 = idxDst,
							idxDst16 = idxDst2,
							use32BitIndices = (indexFormat == IndexFormat.UInt32)
						};
						jobData.Schedule(default(JobHandle)).Complete();
						jobData.sourceSubmeshIndices.Dispose();
						jobData.sourceTransforms.Dispose();
						jobData.baseColors.Dispose();
						jobData.atlasSlices.Dispose();
						jobData.uvModifiersMinMax.Dispose();
						meshData2.SetSubMesh(0, new SubMeshDescriptor(0, num7, MeshTopology.Triangles), MeshUpdateFlags.Default);
						Mesh mesh2 = new Mesh();
						Mesh.ApplyAndDisposeWritableMeshData(data, mesh2, MeshUpdateFlags.Default);
						mesh2.RecalculateBounds();
						GameObject gameObject2 = new GameObject(combinerCriteria2.mat.name + " (combined by EdMeshCombinerPrefab)");
						if (combinedPrefabData != null)
						{
							combinedPrefabData.combined.Add(gameObject2);
						}
						if (combiner.transform != null)
						{
							gameObject2.transform.parent = combiner.transform;
						}
						else
						{
							SceneManager.MoveGameObjectToScene(gameObject2, combiner.gameObject.scene);
						}
						gameObject2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						gameObject2.transform.localScale = Vector3.one;
						gameObject2.isStatic = true;
						gameObject2.layer = (int)combinerCriteria2.objectLayer;
						MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
						meshRenderer.sharedMaterial = combinerCriteria2.mat;
						meshRenderer.lightmapIndex = combinerCriteria2.lightmapIndex;
						if (flag4)
						{
							perSceneRenderData.representativeRenderer = meshRenderer;
						}
						if (perSceneRenderData != null)
						{
							perSceneRenderData.AddMeshToList(gameObject2, meshRenderer);
						}
						MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
						meshFilter2.sharedMesh = mesh2;
						if (!flag && combinerCriteria2.hasMeshCollider)
						{
							MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
							meshCollider.sharedMesh = meshFilter2.sharedMesh;
							meshCollider.convex = false;
							meshCollider.sharedMaterial = combinerCriteria2.meshCollPhysicsMat;
							GorillaSurfaceOverride gorillaSurfaceOverride = gameObject2.AddComponent<GorillaSurfaceOverride>();
							gorillaSurfaceOverride.overrideIndex = combinerCriteria2.surfOverrideIndex;
							gorillaSurfaceOverride.extraVelMultiplier = combinerCriteria2.surfExtraVelMultiplier;
							gorillaSurfaceOverride.extraVelMaxMultiplier = combinerCriteria2.surfExtraVelMaxMultiplier;
							gorillaSurfaceOverride.sendOnTapEvent = combinerCriteria2.surfSendOnTapEvent;
						}
					}
				}
			}
			list2.Sort((Transform a, Transform b) => -a.GetDepth().CompareTo(b.GetDepth()));
			foreach (Transform transform in list2)
			{
				if (!(transform == null) && combinedPrefabData != null)
				{
					MeshRenderer component3 = transform.GetComponent<MeshRenderer>();
					if (component3 != null)
					{
						component3.enabled = false;
						combinedPrefabData.disabled.Add(component3);
					}
					SkinnedMeshRenderer component4 = transform.GetComponent<SkinnedMeshRenderer>();
					if (component4 != null)
					{
						component4.enabled = false;
						combinedPrefabData.disabled.Add(component4);
					}
				}
			}
		}

		protected void OnEnable()
		{
		}

		public EdMeshCombinedPrefabData combinedData;

		private const uint _k_maxVertsForUInt16 = 65535U;

		private const uint _k_maxVertsForUInt32 = 4294967295U;

		private const uint _k_maxVertCount = 65535U;

		[Serializable]
		public struct CombinerInfo
		{
			public MeshFilter meshFilter;

			public Renderer renderer;

			public EdMeshCombinerModifierUVOffset uvOffsetModifier;

			public int subMeshIndex;

			public bool isSkinnedMesh;

			public int layer;
		}

		private struct CombinerCriteria
		{
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int, int, bool, int, float, float, bool>(this.mat.GetInstanceID(), this.staticFlags, this.lightmapIndex, this.hasMeshCollider, this.surfOverrideIndex, this.surfExtraVelMultiplier, this.surfExtraVelMaxMultiplier, this.surfSendOnTapEvent);
			}

			public Material mat;

			public int staticFlags;

			public int lightmapIndex;

			public bool hasMeshCollider;

			public PhysicsMaterial meshCollPhysicsMat;

			public int surfOverrideIndex;

			public float surfExtraVelMultiplier;

			public float surfExtraVelMaxMultiplier;

			public bool surfSendOnTapEvent;

			public UnityLayer objectLayer;
		}

		[BurstCompile]
		private struct CopyMeshJob : IJob
		{
			public void Execute()
			{
				int num = 0;
				int num2 = 0;
				Unity.Mathematics.Random random = new Unity.Mathematics.Random(this.randSeed);
				for (int i = 0; i < this.meshDataArray.Length; i++)
				{
					Mesh.MeshData meshData = this.meshDataArray[i];
					int num3 = this.sourceSubmeshIndices[i];
					SubMeshDescriptor subMesh = meshData.GetSubMesh(num3);
					int vertexCount = meshData.vertexCount;
					int indexCount = subMesh.indexCount;
					Matrix4x4 m = this.sourceTransforms[i];
					bool flag = math.determinant(m) < 0f;
					NativeArray<Vector3> outVertices = new NativeArray<Vector3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
					if (meshData.HasVertexAttribute(VertexAttribute.Position))
					{
						meshData.GetVertices(outVertices);
					}
					else
					{
						for (int j = 0; j < vertexCount; j++)
						{
							outVertices[j] = Vector3.zero;
						}
					}
					NativeArray<Vector3> outNormals = new NativeArray<Vector3>(vertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
					if (meshData.HasVertexAttribute(VertexAttribute.Normal))
					{
						meshData.GetNormals(outNormals);
					}
					else
					{
						for (int k = 0; k < vertexCount; k++)
						{
							outNormals[k] = Vector3.up;
						}
					}
					NativeArray<Vector4> outTangents = new NativeArray<Vector4>(vertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
					if (meshData.HasVertexAttribute(VertexAttribute.Tangent))
					{
						meshData.GetTangents(outTangents);
					}
					else
					{
						for (int l = 0; l < vertexCount; l++)
						{
							outTangents[l] = new Vector4(1f, 0f, 0f, 1f);
						}
					}
					NativeArray<Color> outColors = new NativeArray<Color>(vertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
					if (meshData.HasVertexAttribute(VertexAttribute.Color))
					{
						meshData.GetColors(outColors);
					}
					else
					{
						for (int n = 0; n < vertexCount; n++)
						{
							outColors[n] = Color.white;
						}
					}
					NativeArray<Vector2> outUVs = new NativeArray<Vector2>(vertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
					if (meshData.HasVertexAttribute(VertexAttribute.TexCoord0))
					{
						meshData.GetUVs(0, outUVs);
					}
					else
					{
						for (int num4 = 0; num4 < vertexCount; num4++)
						{
							outUVs[num4] = Vector2.zero;
						}
					}
					NativeArray<Vector2> outUVs2 = new NativeArray<Vector2>(vertexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
					if (meshData.HasVertexAttribute(VertexAttribute.TexCoord1))
					{
						meshData.GetUVs(1, outUVs2);
					}
					else
					{
						for (int num5 = 0; num5 < vertexCount; num5++)
						{
							outUVs2[num5] = Vector2.zero;
						}
					}
					Color color = this.baseColors[i];
					int num6 = this.atlasSlices[i];
					Vector4 vector = this.uvModifiersMinMax[i];
					Vector2 vector2 = new Vector2(random.NextFloat(vector.x, vector.z), random.NextFloat(vector.y, vector.w));
					float num7 = this.isCandleFlame ? random.NextFloat(0f, 1f) : 1f;
					Matrix4x4 transpose = m.inverse.transpose;
					for (int num8 = 0; num8 < vertexCount; num8++)
					{
						Vector3 point = outVertices[num8];
						Vector3 vector3 = outNormals[num8];
						Vector4 vector4 = outTangents[num8];
						Color color2 = outColors[num8];
						Vector2 vector5 = outUVs[num8];
						Vector3 v = m.MultiplyPoint3x4(point);
						Vector3 vector6 = transpose.MultiplyVector(vector3).normalized;
						Vector3 vector7 = transpose.MultiplyVector(new Vector3(vector4.x, vector4.y, vector4.z)).normalized;
						if (flag)
						{
							vector6 = -vector6;
							vector7 = -vector7;
							vector4.w = -vector4.w;
						}
						GTVertexDataStream0 value = new GTVertexDataStream0
						{
							position = v,
							color = new Color(color2.r * color.r, color2.g * color.g, color2.b * color.b, this.isCandleFlame ? num7 : (color2.a * color.a)),
							uv1 = new half4((half)(vector5.x + vector2.x), (half)(vector5.y + vector2.y), (half)((float)num6), (half)num7),
							lightmapUv = new half2((half)(outUVs2[num8].x * this.lightmapScaleOffsets[i].x + this.lightmapScaleOffsets[i].z), (half)(outUVs2[num8].y * this.lightmapScaleOffsets[i].y + this.lightmapScaleOffsets[i].w))
						};
						this.dst0[num + num8] = value;
						GTVertexDataStream1 value2 = new GTVertexDataStream1
						{
							normal = vector6,
							tangent = new Color(vector7.x, vector7.y, vector7.z, vector4.w)
						};
						this.dst1[num + num8] = value2;
					}
					if (this.use32BitIndices)
					{
						NativeArray<int> outIndices = new NativeArray<int>(indexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
						meshData.GetIndices(outIndices, num3, true);
						if (!flag)
						{
							for (int num9 = 0; num9 < indexCount; num9++)
							{
								this.idxDst32[num2 + num9] = num + outIndices[num9];
							}
						}
						else
						{
							for (int num10 = 0; num10 < indexCount; num10 += 3)
							{
								this.idxDst32[num2 + num10] = num + outIndices[num10 + 2];
								this.idxDst32[num2 + num10 + 1] = num + outIndices[num10 + 1];
								this.idxDst32[num2 + num10 + 2] = num + outIndices[num10];
							}
						}
						outIndices.Dispose();
					}
					else
					{
						NativeArray<ushort> outIndices2 = new NativeArray<ushort>(indexCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
						meshData.GetIndices(outIndices2, num3, true);
						if (!flag)
						{
							for (int num11 = 0; num11 < indexCount; num11++)
							{
								this.idxDst16[num2 + num11] = (ushort)(num + (int)outIndices2[num11]);
							}
						}
						else
						{
							for (int num12 = 0; num12 < indexCount; num12 += 3)
							{
								this.idxDst16[num2 + num12] = (ushort)(num + (int)outIndices2[num12 + 2]);
								this.idxDst16[num2 + num12 + 1] = (ushort)(num + (int)outIndices2[num12 + 1]);
								this.idxDst16[num2 + num12 + 2] = (ushort)(num + (int)outIndices2[num12]);
							}
						}
						outIndices2.Dispose();
					}
					outVertices.Dispose();
					outNormals.Dispose();
					outTangents.Dispose();
					outColors.Dispose();
					outUVs.Dispose();
					outUVs2.Dispose();
					num += vertexCount;
					num2 += indexCount;
				}
			}

			[ReadOnly]
			public Mesh.MeshDataArray meshDataArray;

			[ReadOnly]
			public NativeArray<int> sourceSubmeshIndices;

			[ReadOnly]
			public NativeArray<Matrix4x4> sourceTransforms;

			[ReadOnly]
			public NativeArray<float4> lightmapScaleOffsets;

			[ReadOnly]
			public NativeArray<Color> baseColors;

			[ReadOnly]
			public NativeArray<int> atlasSlices;

			[ReadOnly]
			public NativeArray<float4> uvModifiersMinMax;

			public bool isCandleFlame;

			public uint randSeed;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<GTVertexDataStream0> dst0;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<GTVertexDataStream1> dst1;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<int> idxDst32;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<ushort> idxDst16;

			public bool use32BitIndices;
		}
	}
}
