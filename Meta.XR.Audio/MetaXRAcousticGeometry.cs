using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Meta.XR.Acoustics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

internal class MetaXRAcousticGeometry : MonoBehaviour
{
	internal static event Action OnAnyGeometryEnabled;

	internal string RelativeFilePath
	{
		get
		{
			return this.relativeFilePath;
		}
	}

	internal string AbsoluteFilePath
	{
		get
		{
			return Path.GetFullPath(Path.Combine(Application.dataPath, this.RelativeFilePath));
		}
		set
		{
			string text = value.Replace('\\', '/');
			if (text.StartsWith(Application.dataPath))
			{
				this.relativeFilePath = text.Substring(Application.dataPath.Length + 1);
				return;
			}
			Debug.LogError("invalid path " + value + ", outside application path " + Application.dataPath, base.gameObject);
		}
	}

	internal bool EnableSimplification
	{
		get
		{
			return (this.Flags & MeshFlags.ENABLE_SIMPLIFICATION) > MeshFlags.NONE;
		}
		set
		{
			if (value)
			{
				this.Flags |= MeshFlags.ENABLE_SIMPLIFICATION;
				return;
			}
			this.Flags &= ~MeshFlags.ENABLE_SIMPLIFICATION;
		}
	}

	internal bool EnableDiffraction
	{
		get
		{
			return (this.Flags & MeshFlags.ENABLE_DIFFRACTION) > MeshFlags.NONE;
		}
		set
		{
			if (value)
			{
				this.Flags |= MeshFlags.ENABLE_DIFFRACTION;
				return;
			}
			this.Flags &= ~MeshFlags.ENABLE_DIFFRACTION;
		}
	}

	internal float MaxSimplifyError
	{
		get
		{
			return this.maxSimplifyError;
		}
		set
		{
			this.maxSimplifyError = Math.Max(value, 0f);
		}
	}

	internal float MinDiffractionEdgeAngle
	{
		get
		{
			return this.minDiffractionEdgeAngle;
		}
		set
		{
			this.minDiffractionEdgeAngle = Math.Clamp(value, 0f, 180f);
		}
	}

	internal float MinDiffractionEdgeLength
	{
		get
		{
			return this.minDiffractionEdgeLength;
		}
		set
		{
			this.minDiffractionEdgeLength = Math.Max(value, 0f);
		}
	}

	internal float FlagLength
	{
		get
		{
			return this.flagLength;
		}
		set
		{
			this.flagLength = value;
		}
	}

	internal int LodSelection
	{
		get
		{
			return this.lodSelection;
		}
		set
		{
			this.lodSelection = value;
		}
	}

	internal bool UseColliders
	{
		get
		{
			return this.useColliders;
		}
		set
		{
			this.useColliders = value;
		}
	}

	internal bool OverrideExcludeTagsEnabled
	{
		get
		{
			return this.overrideExcludeTagsEnabled;
		}
		set
		{
			this.overrideExcludeTagsEnabled = value;
		}
	}

	internal string[] OverrideExcludeTags
	{
		get
		{
			return this.overrideExcludeTags;
		}
		set
		{
			this.overrideExcludeTags = value;
		}
	}

	internal string[] ExcludeTags
	{
		get
		{
			if (!this.OverrideExcludeTagsEnabled)
			{
				return MetaXRAcousticSettings.Instance.ExcludeTags;
			}
			return this.OverrideExcludeTags;
		}
	}

	internal bool IsLoaded
	{
		get
		{
			return this.loadState_ == MetaXRAcousticGeometry.LoadState.Loaded;
		}
	}

	internal int VertexCount
	{
		get
		{
			return this.vertexCount;
		}
	}

	private void Awake()
	{
		this.StartInternal();
	}

	internal bool StartInternal()
	{
		if (!this.CreatePropagationGeometry())
		{
			return false;
		}
		this.ApplyTransform();
		return true;
	}

	internal bool CreatePropagationGeometry()
	{
		if (this.geometryHandle != IntPtr.Zero)
		{
			Debug.LogWarning("Tried to initialize geometry twice, destroying stale copy", base.gameObject);
			this.DestroyPropagationGeometry();
		}
		if (this.geometryHandle != IntPtr.Zero)
		{
			Debug.LogError("Unable to clean up stale geometry", base.gameObject);
			return false;
		}
		if (MetaXRAcousticNativeInterface.Interface.CreateAudioGeometry(out this.geometryHandle) != 0)
		{
			Debug.LogError("Unable to create geometry handle", base.gameObject);
			return false;
		}
		if (this.FileEnabled)
		{
			if (string.IsNullOrEmpty(this.relativeFilePath))
			{
				if (Application.isPlaying)
				{
					Debug.LogError("No file set, make sure to Bake Mesh to File", base.gameObject);
				}
				return false;
			}
			if (!this.ReadFile())
			{
				return false;
			}
		}
		else if (Application.isPlaying)
		{
			if (base.gameObject.isStatic)
			{
				Debug.LogError("Static geometry requires \"File Enabled\"", base.gameObject);
				return false;
			}
			if (!this.GatherGeometryRuntime())
			{
				return false;
			}
		}
		return true;
	}

	private void IncrementEnabledGeometryCount()
	{
		MetaXRAcousticGeometry.EnabledGeometryCount++;
		if (MetaXRAcousticGeometry.EnabledGeometryCount == 1)
		{
			MetaXRAcousticGeometry.OnAnyGeometryEnabled();
		}
	}

	private void DecrementEnabledGeometryCount()
	{
		MetaXRAcousticGeometry.EnabledGeometryCount--;
	}

	private void OnEnable()
	{
		if (this.loadState_ == MetaXRAcousticGeometry.LoadState.Interrupted)
		{
			Debug.Log("Resuming interrupted load!!");
			this.ReadFile();
			return;
		}
		if (this.geometryHandle == IntPtr.Zero || (this.loadState_ == MetaXRAcousticGeometry.LoadState.NotLoaded && this.FileEnabled))
		{
			return;
		}
		Debug.Log("Enabling Geometry: " + this.relativeFilePath, base.gameObject);
		MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(this.geometryHandle, ObjectFlags.ENABLED, true);
		this.ApplyTransform();
		MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(this.geometryHandle, ObjectFlags.STATIC, base.gameObject.isStatic);
		if (this.IsLoaded)
		{
			this.IncrementEnabledGeometryCount();
		}
	}

	private void OnDisable()
	{
		if (this.geometryHandle == IntPtr.Zero)
		{
			return;
		}
		Debug.Log("Disabling Geometry: " + this.relativeFilePath, base.gameObject);
		if (this.loadState_ == MetaXRAcousticGeometry.LoadState.Loading && !base.gameObject.activeInHierarchy)
		{
			Debug.Log("Interrupted load!!");
			this.loadState_ = MetaXRAcousticGeometry.LoadState.Interrupted;
		}
		MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(this.geometryHandle, ObjectFlags.ENABLED, false);
		this.ApplyTransform();
		MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(this.geometryHandle, ObjectFlags.STATIC, base.gameObject.isStatic);
		if (this.IsLoaded)
		{
			this.DecrementEnabledGeometryCount();
		}
	}

	private void LateUpdate()
	{
		if (this.geometryHandle == IntPtr.Zero)
		{
			return;
		}
		if (base.transform.hasChanged)
		{
			this.ApplyTransform();
			base.transform.hasChanged = false;
		}
	}

	private void ApplyTransform()
	{
		if (this.geometryHandle == IntPtr.Zero)
		{
			return;
		}
		MetaXRAcousticNativeInterface.INativeInterface @interface = MetaXRAcousticNativeInterface.Interface;
		IntPtr geometry = this.geometryHandle;
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		@interface.AudioGeometrySetTransform(geometry, localToWorldMatrix);
	}

	private void OnDestroy()
	{
		this.DestroyInternal();
	}

	internal bool DestroyInternal()
	{
		return this.DestroyPropagationGeometry();
	}

	private bool DestroyPropagationGeometry()
	{
		bool result;
		lock (this)
		{
			if (this.geometryHandle != IntPtr.Zero && MetaXRAcousticNativeInterface.Interface.DestroyAudioGeometry(this.geometryHandle) != 0)
			{
				Debug.LogError("Unable to destroy geometry", base.gameObject);
				result = false;
			}
			else
			{
				this.geometryHandle = IntPtr.Zero;
				result = true;
			}
		}
		return result;
	}

	private static bool isObjectUsedByLODGroup(GameObject obj, LODGroup lod)
	{
		LOD[] lods = lod.GetLODs();
		for (int i = 0; i < lods.Length; i++)
		{
			Renderer[] renderers = lods[i].renderers;
			for (int j = 0; j < renderers.Length; j++)
			{
				if (renderers[j].gameObject == obj)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static void traverseMeshHierarchy(GameObject obj, bool includeChildren, string[] excludeTags, bool parentWasExcluded, int lodSelection, LODGroup parentLOD, MetaXRAcousticGeometry.ITransformVisitor visitor, object parentData = null)
	{
		if (!obj.activeInHierarchy)
		{
			return;
		}
		LODGroup lodgroup = obj.GetComponent(typeof(LODGroup)) as LODGroup;
		if (lodgroup != null)
		{
			LOD[] lods = lodgroup.GetLODs();
			if (lods.Length != 0)
			{
				if (lods.Length == 1 && lods[0].renderers.Length == 1)
				{
					obj = lods[0].renderers[0].gameObject;
				}
				else
				{
					int num = Mathf.Clamp(lodSelection, 0, lods.Length - 1);
					Renderer[] renderers = lods[num].renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						if (renderers[i] != null && !(renderers[i].gameObject == obj))
						{
							MetaXRAcousticGeometry.traverseMeshHierarchy(renderers[i].gameObject, includeChildren, excludeTags, parentWasExcluded, lodSelection, null, visitor, parentData);
						}
					}
				}
				parentLOD = lodgroup;
			}
		}
		bool flag = true;
		bool flag2 = parentLOD != lodgroup && parentLOD != null && MetaXRAcousticGeometry.isObjectUsedByLODGroup(obj, parentLOD);
		if (excludeTags.Contains(obj.tag) || parentWasExcluded || flag2)
		{
			MetaXRAcousticMaterial component = obj.GetComponent<MetaXRAcousticMaterial>();
			flag = (!(component == null) && component.enabled);
		}
		if (flag)
		{
			parentData = visitor.visit(obj.transform, parentData);
		}
		if (includeChildren)
		{
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				if (transform.GetComponent<MetaXRAcousticGeometry>() == null)
				{
					MetaXRAcousticGeometry.traverseMeshHierarchy(transform.gameObject, includeChildren, excludeTags, !flag, lodSelection, parentLOD, visitor, parentData);
				}
			}
		}
	}

	private bool GatherGeometryInternal(IntPtr geometryHandle, GameObject meshObject, Matrix4x4 worldToLocal, bool ignoreStatic, out int ignoredMeshCount)
	{
		ignoredMeshCount = 0;
		MetaXRAcousticGeometry.IGatherer gatherer;
		if (this.useColliders)
		{
			gatherer = new MetaXRAcousticGeometry.ColliderGatherer();
		}
		else
		{
			gatherer = new MetaXRAcousticGeometry.MeshGatherer(ignoreStatic);
		}
		MetaXRAcousticGeometry.traverseMeshHierarchy(meshObject, this.IncludeChildMeshes, this.ExcludeTags, false, this.lodSelection, null, gatherer, null);
		int num = 0;
		uint num2 = 0U;
		int num3 = 0;
		int num4 = 0;
		foreach (MetaXRAcousticGeometry.MeshMaterial meshMaterial in gatherer.Meshes)
		{
			MetaXRAcousticGeometry.updateCountsForMesh(ref num, ref num2, ref num3, ref num4, meshMaterial.mesh);
		}
		IMaterialDataProvider[] array = new IMaterialDataProvider[1];
		for (int i = 0; i < gatherer.Terrains.Count; i++)
		{
			MetaXRAcousticGeometry.TerrainMaterial terrainMaterial = gatherer.Terrains[i];
			TerrainData terrainData = terrainMaterial.terrain.terrainData;
			int heightmapResolution = terrainData.heightmapResolution;
			int heightmapResolution2 = terrainData.heightmapResolution;
			int num5 = (heightmapResolution - 1) / MetaXRAcousticGeometry.terrainDecimation + 1;
			int num6 = (heightmapResolution2 - 1) / MetaXRAcousticGeometry.terrainDecimation + 1;
			int num7 = num5 * num6;
			int num8 = (num5 - 1) * (num6 - 1) * 6;
			num4++;
			num += num7;
			num2 += (uint)num8;
			num3 += num8 / 3;
			TreePrototype[] treePrototypes = terrainData.treePrototypes;
			if (treePrototypes.Length != 0)
			{
				if (array[0] == null)
				{
					array[0] = terrainMaterial.terrainMaterials.Last<IMaterialDataProvider>();
				}
				terrainMaterial.treePrototypeMeshes = new Mesh[treePrototypes.Length];
				for (int j = 0; j < treePrototypes.Length; j++)
				{
					MeshFilter[] componentsInChildren = treePrototypes[j].prefab.GetComponentsInChildren<MeshFilter>();
					int num9 = int.MaxValue;
					int num10 = -1;
					for (int k = 0; k < componentsInChildren.Length; k++)
					{
						int num11 = componentsInChildren[k].sharedMesh.vertexCount;
						if (num11 < num9)
						{
							num9 = num11;
							num10 = k;
						}
					}
					terrainMaterial.treePrototypeMeshes[j] = componentsInChildren[num10].sharedMesh;
				}
				foreach (TreeInstance treeInstance in terrainData.treeInstances)
				{
					MetaXRAcousticGeometry.updateCountsForMesh(ref num, ref num2, ref num3, ref num4, terrainMaterial.treePrototypeMeshes[treeInstance.prototypeIndex]);
				}
				gatherer.Terrains[i] = terrainMaterial;
			}
		}
		List<Vector3> tempVertices = new List<Vector3>();
		List<int> tempIndices = new List<int>();
		MeshGroup[] array2 = new MeshGroup[num4];
		float[] array3 = new float[num * 3];
		int[] array4 = new int[num2];
		int num12 = 0;
		int num13 = 0;
		int num14 = 0;
		foreach (MetaXRAcousticGeometry.MeshMaterial meshMaterial2 in gatherer.Meshes)
		{
			Matrix4x4 matrix = worldToLocal * meshMaterial2.meshTransform.localToWorldMatrix;
			if (!MetaXRAcousticGeometry.uploadMeshFilter(tempVertices, tempIndices, array2, array3, array4, ref num12, ref num13, ref num14, meshMaterial2.mesh, meshMaterial2.meshMaterials, matrix))
			{
				return false;
			}
		}
		foreach (MetaXRAcousticGeometry.TerrainMaterial terrainMaterial2 in gatherer.Terrains)
		{
			TerrainData terrainData2 = terrainMaterial2.terrain.terrainData;
			Matrix4x4 matrix4x = worldToLocal * terrainMaterial2.terrain.gameObject.transform.localToWorldMatrix;
			int heightmapResolution3 = terrainData2.heightmapResolution;
			int heightmapResolution4 = terrainData2.heightmapResolution;
			float[,] heights = terrainData2.GetHeights(0, 0, heightmapResolution3, heightmapResolution4);
			Vector3 size = terrainData2.size;
			size = new Vector3(size.x / (float)(heightmapResolution3 - 1) * (float)MetaXRAcousticGeometry.terrainDecimation, size.y, size.z / (float)(heightmapResolution4 - 1) * (float)MetaXRAcousticGeometry.terrainDecimation);
			int num15 = (heightmapResolution3 - 1) / MetaXRAcousticGeometry.terrainDecimation + 1;
			int num16 = (heightmapResolution4 - 1) / MetaXRAcousticGeometry.terrainDecimation + 1;
			int num17 = num15 * num16;
			int num18 = (num15 - 1) * (num16 - 1) * 2;
			array2[num14].faceType = FaceType.TRIANGLES;
			array2[num14].faceCount = (UIntPtr)((ulong)((long)num18));
			array2[num14].indexOffset = (UIntPtr)((ulong)((long)num13));
			if (terrainMaterial2.terrainMaterials != null && terrainMaterial2.terrainMaterials.Length != 0)
			{
				array2[num14].material = MetaXRAcousticMaterial.CreateMaterialNativeHandle(terrainMaterial2.terrainMaterials[0].Data);
			}
			else
			{
				array2[num14].material = IntPtr.Zero;
			}
			for (int m = 0; m < num16; m++)
			{
				for (int n = 0; n < num15; n++)
				{
					int num19 = (num12 + m * num15 + n) * 3;
					Vector3 vector = matrix4x.MultiplyPoint3x4(Vector3.Scale(size, new Vector3((float)m, heights[n * MetaXRAcousticGeometry.terrainDecimation, m * MetaXRAcousticGeometry.terrainDecimation], (float)n)));
					array3[num19] = vector.x;
					array3[num19 + 1] = vector.y;
					array3[num19 + 2] = vector.z;
				}
			}
			for (int num20 = 0; num20 < num16 - 1; num20++)
			{
				for (int num21 = 0; num21 < num15 - 1; num21++)
				{
					array4[num13] = num12 + num20 * num15 + num21;
					array4[num13 + 1] = num12 + (num20 + 1) * num15 + num21;
					array4[num13 + 2] = num12 + num20 * num15 + num21 + 1;
					array4[num13 + 3] = num12 + (num20 + 1) * num15 + num21;
					array4[num13 + 4] = num12 + (num20 + 1) * num15 + num21 + 1;
					array4[num13 + 5] = num12 + num20 * num15 + num21 + 1;
					num13 += 6;
				}
			}
			num12 += num17;
			num14++;
			foreach (TreeInstance treeInstance2 in terrainData2.treeInstances)
			{
				Vector3 vector2 = Vector3.Scale(treeInstance2.position, terrainData2.size);
				Matrix4x4 localToWorldMatrix = terrainMaterial2.terrain.gameObject.transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, localToWorldMatrix.GetColumn(3) + new Vector4(vector2.x, vector2.y, vector2.z, 0f));
				Matrix4x4 matrix2 = worldToLocal * localToWorldMatrix;
				if (!MetaXRAcousticGeometry.uploadMeshFilter(tempVertices, tempIndices, array2, array3, array4, ref num12, ref num13, ref num14, terrainMaterial2.treePrototypeMeshes[treeInstance2.prototypeIndex], array, matrix2))
				{
					return false;
				}
			}
		}
		if (num == 0)
		{
			Scene scene = base.gameObject.scene;
			string str = base.gameObject.scene.name + ":" + string.Join("/", (from t in base.gameObject.GetComponentsInParent<Transform>()
			select t.name).Reverse<string>().ToArray<string>());
			Debug.LogError("Geometry unable to upload mesh, vertex count is zero " + str, base.gameObject);
			return false;
		}
		Debug.Log(string.Format("Uploading mesh {0} with {1} vertices", base.name, num));
		MeshSimplification meshSimplification = default(MeshSimplification);
		meshSimplification.thisSize = (UIntPtr)((ulong)((long)Marshal.SizeOf(typeof(MeshSimplification))));
		meshSimplification.flags = this.Flags;
		meshSimplification.unitScale = 1f;
		meshSimplification.maxError = this.MaxSimplifyError;
		meshSimplification.minDiffractionEdgeAngle = this.MinDiffractionEdgeAngle;
		meshSimplification.minDiffractionEdgeLength = this.MinDiffractionEdgeLength;
		meshSimplification.flagLength = this.FlagLength;
		meshSimplification.threadCount = (UIntPtr)1UL;
		int num22 = MetaXRAcousticNativeInterface.Interface.AudioGeometryUploadSimplifiedMeshArrays(geometryHandle, array3, num, array4, array4.Length, array2, array2.Length, ref meshSimplification);
		MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.ENABLED, base.isActiveAndEnabled);
		MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(geometryHandle, ObjectFlags.STATIC, base.gameObject.isStatic);
		foreach (MeshGroup meshGroup in array2)
		{
			MetaXRAcousticMaterial.DestroyMaterialNativeHandle(meshGroup.material);
		}
		if (num22 == 0)
		{
			List<IMaterialDataProvider> list = new List<IMaterialDataProvider>();
			foreach (MetaXRAcousticGeometry.MeshMaterial meshMaterial3 in gatherer.Meshes)
			{
				int subMeshCount = meshMaterial3.mesh.subMeshCount;
				int num23 = (meshMaterial3.meshMaterials == null) ? 0 : meshMaterial3.meshMaterials.Length;
				if (num23 != 0)
				{
					int num24 = Mathf.Min(num23, subMeshCount);
					for (int num25 = 0; num25 < num24; num25++)
					{
						list.Add(meshMaterial3.meshMaterials[num25]);
					}
					for (int num25 = num24; num25 < subMeshCount; num25++)
					{
						list.Add(meshMaterial3.meshMaterials[num23 - 1]);
					}
				}
				else
				{
					for (int num26 = 0; num26 < subMeshCount; num26++)
					{
						list.Add(null);
					}
				}
			}
			foreach (MetaXRAcousticGeometry.TerrainMaterial terrainMaterial3 in gatherer.Terrains)
			{
				if (terrainMaterial3.terrainMaterials != null && terrainMaterial3.terrainMaterials.Length != 0)
				{
					list.AddRange(terrainMaterial3.terrainMaterials);
				}
			}
			return true;
		}
		return false;
	}

	private static bool uploadMeshFilter(List<Vector3> tempVertices, List<int> tempIndices, MeshGroup[] groups, float[] vertices, int[] indices, ref int vertexOffset, ref int indexOffset, ref int groupOffset, Mesh mesh, IMaterialDataProvider[] materials, Matrix4x4 matrix)
	{
		tempVertices.Clear();
		mesh.GetVertices(tempVertices);
		int count = tempVertices.Count;
		for (int i = 0; i < count; i++)
		{
			Vector3 vector = matrix.MultiplyPoint3x4(tempVertices[i]);
			int num = (vertexOffset + i) * 3;
			vertices[num] = vector.x;
			vertices[num + 1] = vector.y;
			vertices[num + 2] = vector.z;
		}
		for (int j = 0; j < mesh.subMeshCount; j++)
		{
			MeshTopology topology = mesh.GetTopology(j);
			if (topology == MeshTopology.Triangles || topology == MeshTopology.Quads)
			{
				tempIndices.Clear();
				mesh.GetIndices(tempIndices, j);
				int count2 = tempIndices.Count;
				for (int k = 0; k < count2; k++)
				{
					indices[indexOffset + k] = tempIndices[k] + vertexOffset;
				}
				if (topology == MeshTopology.Triangles)
				{
					groups[groupOffset + j].faceType = FaceType.TRIANGLES;
					groups[groupOffset + j].faceCount = (UIntPtr)((ulong)((long)(count2 / 3)));
				}
				else if (topology == MeshTopology.Quads)
				{
					groups[groupOffset + j].faceType = FaceType.QUADS;
					groups[groupOffset + j].faceCount = (UIntPtr)((ulong)((long)(count2 / 4)));
				}
				groups[groupOffset + j].indexOffset = (UIntPtr)((ulong)((long)indexOffset));
				if (materials != null && materials.Length != 0)
				{
					int num2 = j;
					if (num2 >= materials.Length)
					{
						num2 = materials.Length - 1;
					}
					groups[groupOffset + j].material = MetaXRAcousticMaterial.CreateMaterialNativeHandle(materials[num2].Data);
				}
				else
				{
					groups[groupOffset + j].material = IntPtr.Zero;
				}
				indexOffset += count2;
			}
		}
		vertexOffset += count;
		groupOffset += mesh.subMeshCount;
		return true;
	}

	private static void updateCountsForMesh(ref int totalVertexCount, ref uint totalIndexCount, ref int totalFaceCount, ref int totalMaterialCount, Mesh mesh)
	{
		totalMaterialCount += mesh.subMeshCount;
		totalVertexCount += mesh.vertexCount;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			MeshTopology topology = mesh.GetTopology(i);
			if (topology == MeshTopology.Triangles || topology == MeshTopology.Quads)
			{
				uint indexCount = mesh.GetIndexCount(i);
				totalIndexCount += indexCount;
				if (topology == MeshTopology.Triangles)
				{
					totalFaceCount += (int)(indexCount / 3U);
				}
				else if (topology == MeshTopology.Quads)
				{
					totalFaceCount += (int)(indexCount / 4U);
				}
			}
		}
	}

	internal bool GatherGeometryRuntime()
	{
		Debug.Log("Gathering geometry");
		int num;
		if (!this.GatherGeometryInternal(this.geometryHandle, base.gameObject, base.gameObject.transform.worldToLocalMatrix, Application.isPlaying, out num))
		{
			return false;
		}
		if (num != 0)
		{
			Debug.LogWarning(string.Format("Failed to upload meshes, {0} static meshes ignored. Turn on \"File Enabled\" to process static meshes offline", num), base.gameObject);
		}
		return true;
	}

	internal bool ReadFile()
	{
		if (string.IsNullOrEmpty(this.AbsoluteFilePath))
		{
			Debug.LogError("Invalid mesh file path", base.gameObject);
			return false;
		}
		int num = this.AbsoluteFilePath.IndexOf("StreamingAssets");
		if (Application.isPlaying && num > 0)
		{
			string relativePath = this.AbsoluteFilePath.Substring(num + 16);
			base.StartCoroutine(this.LoadGeometryAsync(relativePath));
		}
		else
		{
			if (MetaXRAcousticNativeInterface.Interface.AudioGeometryReadMeshFile(this.geometryHandle, this.AbsoluteFilePath) != 0)
			{
				Debug.LogError("Error reading mesh file " + this.AbsoluteFilePath, base.gameObject);
				return false;
			}
			MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(this.geometryHandle, ObjectFlags.ENABLED, base.isActiveAndEnabled);
			this.ApplyTransform();
			MetaXRAcousticNativeInterface.Interface.AudioGeometrySetObjectFlag(this.geometryHandle, ObjectFlags.STATIC, base.gameObject.isStatic);
		}
		return true;
	}

	private IEnumerator LoadGeometryAsync(string relativePath)
	{
		string text = Application.streamingAssetsPath + "/" + relativePath;
		Debug.Log("Loading Geometry " + base.name + " from StreamingAssets " + text, base.gameObject);
		float startTime = Time.realtimeSinceStartup;
		UnityWebRequest unityWebRequest = UnityWebRequest.Get(text);
		this.loadState_ = MetaXRAcousticGeometry.LoadState.Loading;
		yield return unityWebRequest.SendWebRequest();
		if (!string.IsNullOrEmpty(unityWebRequest.error))
		{
			Debug.LogError(string.Format("web request: done={0}: {1}", unityWebRequest.isDone, unityWebRequest.error), base.gameObject);
		}
		float num = Time.realtimeSinceStartup - startTime;
		Debug.Log(string.Format("Geometry {0}, read time = {1}", base.name, num), base.gameObject);
		this.LoadGeometryFromMemory(unityWebRequest.downloadHandler.nativeData);
		yield break;
	}

	private void LoadGeometryFromMemory(NativeArray<byte>.ReadOnly data)
	{
		MetaXRAcousticGeometry.<LoadGeometryFromMemory>d__93 <LoadGeometryFromMemory>d__;
		<LoadGeometryFromMemory>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<LoadGeometryFromMemory>d__.<>4__this = this;
		<LoadGeometryFromMemory>d__.data = data;
		<LoadGeometryFromMemory>d__.<>1__state = -1;
		<LoadGeometryFromMemory>d__.<>t__builder.Start<MetaXRAcousticGeometry.<LoadGeometryFromMemory>d__93>(ref <LoadGeometryFromMemory>d__);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static MetaXRAcousticGeometry()
	{
		MetaXRAcousticGeometry.OnAnyGeometryEnabled = delegate()
		{
		};
		MetaXRAcousticGeometry.terrainDecimation = 4;
	}

	internal static bool AUTO_VALIDATE = true;

	internal const string FILE_EXTENSION = "xrageo";

	internal static int EnabledGeometryCount = 0;

	[SerializeField]
	[FormerlySerializedAs("relativeFilePath_")]
	private string relativeFilePath = "";

	[SerializeField]
	internal bool FileEnabled = true;

	[SerializeField]
	internal bool IncludeChildMeshes = true;

	[SerializeField]
	internal MeshFlags Flags = MeshFlags.ENABLE_SIMPLIFICATION;

	[SerializeField]
	private float maxSimplifyError = 0.1f;

	[SerializeField]
	private float minDiffractionEdgeAngle = 1f;

	[SerializeField]
	private float minDiffractionEdgeLength = 0.01f;

	[SerializeField]
	private float flagLength = 1f;

	[SerializeField]
	private int lodSelection;

	[SerializeField]
	private bool useColliders;

	[SerializeField]
	private bool overrideExcludeTagsEnabled;

	[SerializeField]
	private string[] overrideExcludeTags;

	[NonSerialized]
	internal IntPtr geometryHandle = IntPtr.Zero;

	[NonSerialized]
	internal MetaXRAcousticGeometry.LoadState loadState_;

	[NonSerialized]
	private int vertexCount = -1;

	[SerializeField]
	private Color[] materialColors;

	[SerializeField]
	private Hash128 HierarchyHash;

	internal const int Success = 0;

	private static int terrainDecimation;

	internal enum LoadState
	{
		NotLoaded,
		Loading,
		Interrupted,
		Loaded
	}

	private struct MeshMaterial
	{
		internal Mesh mesh;

		internal Transform meshTransform;

		internal IMaterialDataProvider[] meshMaterials;
	}

	private struct TerrainMaterial
	{
		internal Terrain terrain;

		internal IMaterialDataProvider[] terrainMaterials;

		internal Mesh[] treePrototypeMeshes;
	}

	internal interface ITransformVisitor
	{
		object visit(Transform transform, object userData);
	}

	private interface IGatherer : MetaXRAcousticGeometry.ITransformVisitor
	{
		List<MetaXRAcousticGeometry.MeshMaterial> Meshes { get; }

		List<MetaXRAcousticGeometry.TerrainMaterial> Terrains { get; }
	}

	private class MeshGatherer : MetaXRAcousticGeometry.IGatherer, MetaXRAcousticGeometry.ITransformVisitor
	{
		internal MeshGatherer(bool ignoreStatic)
		{
			this.ignoreStatic = ignoreStatic;
		}

		public object visit(Transform transform, object parentData)
		{
			IMaterialDataProvider[] array = parentData as IMaterialDataProvider[];
			MeshFilter[] components = transform.GetComponents<MeshFilter>();
			Terrain[] components2 = transform.GetComponents<Terrain>();
			IMaterialDataProvider[] array2 = Array.ConvertAll<MetaXRAcousticMaterial, MetaXRAcousticMaterial>((from x in transform.GetComponents<MetaXRAcousticMaterial>()
			where x.enabled
			select x).ToArray<MetaXRAcousticMaterial>(), (MetaXRAcousticMaterial x) => x);
			IMaterialDataProvider[] array3 = array2;
			if (array3 != null && array3.Length != 0)
			{
				int num = array3.Length;
				if (array != null && array.Length > num)
				{
					num = array.Length;
				}
				IMaterialDataProvider[] array4 = new IMaterialDataProvider[num];
				if (array != null)
				{
					for (int i = array3.Length; i < num; i++)
					{
						array4[i] = array[i];
					}
				}
				array = array4;
				for (int j = 0; j < array3.Length; j++)
				{
					array[j] = array3[j];
				}
			}
			foreach (MeshFilter meshFilter in components)
			{
				Mesh sharedMesh = meshFilter.sharedMesh;
				if (!(sharedMesh == null))
				{
					if (this.ignoreStatic && (!sharedMesh.isReadable || transform.gameObject.isStatic))
					{
						Debug.LogError("Mesh: " + meshFilter.gameObject.name + " not readable. Use \"File Enabled\" for static geometry", transform);
						this.ignoredMeshCount++;
					}
					else
					{
						this.meshes.Add(new MetaXRAcousticGeometry.MeshMaterial
						{
							mesh = sharedMesh,
							meshTransform = transform,
							meshMaterials = array
						});
					}
				}
			}
			foreach (Terrain terrain in components2)
			{
				this.terrains.Add(new MetaXRAcousticGeometry.TerrainMaterial
				{
					terrain = terrain,
					terrainMaterials = array
				});
			}
			return array;
		}

		public List<MetaXRAcousticGeometry.MeshMaterial> Meshes
		{
			get
			{
				return this.meshes;
			}
		}

		public List<MetaXRAcousticGeometry.TerrainMaterial> Terrains
		{
			get
			{
				return this.terrains;
			}
		}

		private List<MetaXRAcousticGeometry.MeshMaterial> meshes = new List<MetaXRAcousticGeometry.MeshMaterial>();

		private List<MetaXRAcousticGeometry.TerrainMaterial> terrains = new List<MetaXRAcousticGeometry.TerrainMaterial>();

		internal int ignoredMeshCount;

		internal bool ignoreStatic;
	}

	private class ColliderGatherer : MetaXRAcousticGeometry.IGatherer, MetaXRAcousticGeometry.ITransformVisitor
	{
		public object visit(Transform transform, object parentData)
		{
			IMaterialDataProvider[] array = Array.ConvertAll<MetaXRAcousticMaterial, MetaXRAcousticMaterial>((from x in transform.GetComponents<MetaXRAcousticMaterial>()
			where x.enabled
			select x).ToArray<MetaXRAcousticMaterial>(), (MetaXRAcousticMaterial x) => x);
			IMaterialDataProvider[] array2 = array;
			foreach (MeshCollider meshCollider in transform.GetComponents<MeshCollider>())
			{
				if (!(meshCollider.sharedMesh == null))
				{
					if (array2.Length == 0)
					{
						MetaXRAcousticMaterialProperties metaXRAcousticMaterialProperties = MetaXRAcousticMaterialMapping.Instance.findAcousticMaterial(meshCollider.sharedMaterial);
						if (metaXRAcousticMaterialProperties != null)
						{
							array2 = new IMaterialDataProvider[]
							{
								metaXRAcousticMaterialProperties
							};
						}
					}
					this.meshes.Add(new MetaXRAcousticGeometry.MeshMaterial
					{
						mesh = meshCollider.sharedMesh,
						meshTransform = transform,
						meshMaterials = array2
					});
				}
			}
			foreach (BoxCollider boxCollider in transform.GetComponents<BoxCollider>())
			{
				Mesh mesh = new Mesh();
				Vector3[] vertices = new Vector3[]
				{
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(1f, 1f, 1f)),
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(1f, 1f, -1f)),
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(1f, -1f, 1f)),
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(1f, -1f, -1f)),
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(-1f, 1f, 1f)),
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(-1f, 1f, -1f)),
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(-1f, -1f, 1f)),
					boxCollider.center + Vector3.Scale(boxCollider.size * 0.5f, new Vector3(-1f, -1f, -1f))
				};
				int[] indices = new int[]
				{
					1,
					0,
					2,
					3,
					0,
					4,
					6,
					2,
					4,
					5,
					7,
					6,
					5,
					1,
					3,
					7,
					1,
					5,
					4,
					0,
					2,
					6,
					7,
					3
				};
				mesh.vertices = vertices;
				mesh.SetIndices(indices, MeshTopology.Quads, 0);
				if (array2.Length == 0)
				{
					MetaXRAcousticMaterialProperties metaXRAcousticMaterialProperties2 = MetaXRAcousticMaterialMapping.Instance.findAcousticMaterial(boxCollider.sharedMaterial);
					if (metaXRAcousticMaterialProperties2 != null)
					{
						array2 = new IMaterialDataProvider[]
						{
							metaXRAcousticMaterialProperties2
						};
					}
				}
				this.meshes.Add(new MetaXRAcousticGeometry.MeshMaterial
				{
					mesh = mesh,
					meshTransform = transform,
					meshMaterials = array2
				});
			}
			return null;
		}

		public List<MetaXRAcousticGeometry.MeshMaterial> Meshes
		{
			get
			{
				return this.meshes;
			}
		}

		public List<MetaXRAcousticGeometry.TerrainMaterial> Terrains
		{
			get
			{
				return this.terrains;
			}
		}

		private List<MetaXRAcousticGeometry.MeshMaterial> meshes = new List<MetaXRAcousticGeometry.MeshMaterial>();

		private List<MetaXRAcousticGeometry.TerrainMaterial> terrains = new List<MetaXRAcousticGeometry.TerrainMaterial>();
	}
}
