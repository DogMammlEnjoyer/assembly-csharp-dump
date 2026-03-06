using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OVRSimpleJSON;
using UnityEngine;

public class OVRGLTFLoader
{
	public OVRGLTFLoader(string fileName)
	{
		this.m_glbStream = File.Open(fileName, FileMode.Open);
	}

	public OVRGLTFLoader(byte[] data)
	{
		this.m_glbStream = new MemoryStream(data, 0, data.Length, false, true);
	}

	public OVRGLTFLoader(Func<Stream> deferredStream)
	{
		this.m_deferredStream = deferredStream;
	}

	public OVRGLTFScene LoadGLB(bool supportAnimation, bool loadMips = true)
	{
		IEnumerator enumerator = this.LoadGLBCoroutine(supportAnimation, loadMips);
		while (enumerator.MoveNext())
		{
		}
		return this.scene;
	}

	public IEnumerator LoadGLBCoroutine(bool supportAnimation, bool loadMips = true)
	{
		this.scene = default(OVRGLTFScene);
		this.m_InputAnimationNodes = new Dictionary<OVRGLTFInputNode, OVRGLTFAnimatinonNode>();
		this.m_AnimationLookup = new Dictionary<int, OVRGLTFAnimatinonNode[]>();
		this.m_morphTargetHandlers = new Dictionary<int, OVRGLTFAnimationNodeMorphTargetHandler>();
		this.m_textures = new Dictionary<int, Texture2D>();
		this.m_materials = new Dictionary<int, Material>();
		if (Application.isBatchMode)
		{
			Debug.Log("Batch Mode Single Threaded Loading");
			this.m_jsonData = this.InitializeGLBLoad();
		}
		else
		{
			OVRGLTFLoader.<>c__DisplayClass26_0 CS$<>8__locals1 = new OVRGLTFLoader.<>c__DisplayClass26_0();
			CS$<>8__locals1.task = Task.Run<JSONNode>(() => this.InitializeGLBLoad());
			yield return new WaitUntil(() => CS$<>8__locals1.task.IsCompleted);
			this.m_jsonData = CS$<>8__locals1.task.Result;
			if (CS$<>8__locals1.task.IsFaulted)
			{
				Debug.LogException(CS$<>8__locals1.task.Exception);
			}
			CS$<>8__locals1 = null;
		}
		if (this.m_jsonData == null || !OVRGLTFAccessor.TryCreate(this.m_jsonData["accessors"], this.m_jsonData["bufferViews"], this.m_jsonData["buffers"], this.m_glbStream, out this._dataAccessor))
		{
			Stream glbStream = this.m_glbStream;
			if (glbStream != null)
			{
				glbStream.Close();
			}
			yield break;
		}
		IEnumerator loadGltf = this.LoadGLTF(supportAnimation, loadMips);
		while (loadGltf.MoveNext())
		{
			object obj = loadGltf.Current;
			yield return obj;
		}
		this.m_glbStream.Close();
		if (!this.m_Nodes.Any<GameObject>())
		{
			yield break;
		}
		this.scene.root.transform.Rotate(Vector3.up, 180f);
		this.scene.root.SetActive(true);
		this.scene.animationNodes = this.m_InputAnimationNodes;
		this.scene.animationNodeLookup = this.m_AnimationLookup;
		this.scene.morphTargetHandlers = this.m_morphTargetHandlers.Values.ToList<OVRGLTFAnimationNodeMorphTargetHandler>();
		yield break;
	}

	private JSONNode InitializeGLBLoad()
	{
		if (this.m_deferredStream != null)
		{
			this.m_glbStream = this.m_deferredStream();
		}
		if (OVRGLTFLoader.ValidateGLB(this.m_glbStream))
		{
			byte[] array = OVRGLTFLoader.ReadChunk(this.m_glbStream, OVRChunkType.JSON);
			if (array != null)
			{
				return JSON.Parse(Encoding.ASCII.GetString(array));
			}
		}
		return null;
	}

	public void SetModelShader(Shader shader)
	{
		this.m_Shader = shader;
	}

	public void SetModelAlphaBlendShader(Shader shader)
	{
		this.m_AlphaBlendShader = shader;
	}

	public void SetTextureQualityFiltering(OVRTextureQualityFiltering loadedTexturesQuality)
	{
		this.m_TextureQuality = loadedTexturesQuality;
	}

	public void SetMipMapBias(float loadedTexturesMipmapBiasing)
	{
		this.m_TextureMipmapBias = Mathf.Clamp(loadedTexturesMipmapBiasing, -1f, 1f);
	}

	public static OVRTextureQualityFiltering DetectTextureQuality(in Texture2D srcTexture)
	{
		switch (srcTexture.filterMode)
		{
		case FilterMode.Point:
			return OVRTextureQualityFiltering.None;
		case FilterMode.Trilinear:
			if (srcTexture.anisoLevel <= 1)
			{
				return OVRTextureQualityFiltering.Trilinear;
			}
			if (srcTexture.anisoLevel < 4)
			{
				return OVRTextureQualityFiltering.Aniso2x;
			}
			if (srcTexture.anisoLevel < 8)
			{
				return OVRTextureQualityFiltering.Aniso4x;
			}
			if (srcTexture.anisoLevel < 16)
			{
				return OVRTextureQualityFiltering.Aniso8x;
			}
			return OVRTextureQualityFiltering.Aniso16x;
		}
		return OVRTextureQualityFiltering.Bilinear;
	}

	public static void ApplyTextureQuality(OVRTextureQualityFiltering qualityLevel, ref Texture2D destTexture)
	{
		if (destTexture == null)
		{
			return;
		}
		switch (qualityLevel)
		{
		case OVRTextureQualityFiltering.None:
			destTexture.filterMode = FilterMode.Point;
			destTexture.anisoLevel = 0;
			return;
		case OVRTextureQualityFiltering.Bilinear:
			destTexture.filterMode = FilterMode.Bilinear;
			destTexture.anisoLevel = 0;
			return;
		case OVRTextureQualityFiltering.Trilinear:
			destTexture.filterMode = FilterMode.Trilinear;
			destTexture.anisoLevel = 0;
			return;
		default:
			destTexture.filterMode = FilterMode.Trilinear;
			destTexture.anisoLevel = Mathf.FloorToInt(Mathf.Pow(2f, (float)(qualityLevel - OVRTextureQualityFiltering.Trilinear)));
			return;
		}
	}

	public static bool ValidateGLB(Stream glbStream)
	{
		if (glbStream == null)
		{
			return false;
		}
		int num = 4;
		byte[] array = new byte[num];
		glbStream.Read(array, 0, num);
		if (BitConverter.ToUInt32(array, 0) != 1179937895U)
		{
			Debug.LogError("Data stream was not a valid glTF format");
			return false;
		}
		glbStream.Read(array, 0, num);
		if (BitConverter.ToUInt32(array, 0) != 2U)
		{
			Debug.LogError("Only glTF 2.0 is supported");
			return false;
		}
		glbStream.Read(array, 0, num);
		if ((ulong)BitConverter.ToUInt32(array, 0) != (ulong)glbStream.Length)
		{
			Debug.LogError("glTF header length does not match file length");
			return false;
		}
		return true;
	}

	public static byte[] ReadChunk(Stream glbStream, OVRChunkType type)
	{
		uint num;
		if (OVRGLTFLoader.ValidateChunk(glbStream, type, out num))
		{
			byte[] array = new byte[num];
			glbStream.Read(array, 0, (int)num);
			return array;
		}
		return null;
	}

	private static bool ValidateChunk(Stream glbStream, OVRChunkType type, out uint chunkLength)
	{
		int num = 4;
		byte[] array = new byte[num];
		glbStream.Read(array, 0, num);
		chunkLength = BitConverter.ToUInt32(array, 0);
		glbStream.Read(array, 0, num);
		if (BitConverter.ToUInt32(array, 0) != (uint)type)
		{
			Debug.LogError("Read chunk does not match type.");
			return false;
		}
		return true;
	}

	private IEnumerator LoadGLTF(bool supportAnimation, bool loadMips)
	{
		if (this.m_jsonData == null)
		{
			Debug.LogError("m_jsonData was null");
			yield break;
		}
		JSONNode jsonnode = this.m_jsonData["scenes"];
		if (jsonnode.Count == 0)
		{
			Debug.LogError("No valid scenes in this glTF.");
			yield break;
		}
		this.scene.root = new GameObject("GLB Scene Root");
		Transform sceneRootTransform = this.scene.root.transform;
		this.scene.root.SetActive(false);
		JSONArray nodes = this.m_jsonData["nodes"].AsArray;
		this.m_Nodes = new GameObject[nodes.Count];
		sceneRootTransform.hierarchyCapacity = nodes.Count;
		int num = 0;
		foreach (JSONNode jsonnode2 in nodes.Values)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(sceneRootTransform, false);
			this.m_Nodes[num++] = gameObject;
		}
		JSONArray asArray = jsonnode[0]["nodes"].AsArray;
		this.m_processingNodesStart = Time.realtimeSinceStartup;
		foreach (KeyValuePair<string, JSONNode> aKeyValue in asArray)
		{
			int asInt = aKeyValue.AsInt;
			IEnumerator processNode = this.ProcessNode(nodes, asInt, loadMips, sceneRootTransform);
			while (processNode.MoveNext())
			{
				object obj = processNode.Current;
				yield return obj;
			}
			processNode = null;
		}
		JSONNode.Enumerator enumerator2 = default(JSONNode.Enumerator);
		if (supportAnimation)
		{
			IEnumerator processNode = this.ProcessAnimations();
			while (processNode.MoveNext())
			{
				object obj2 = processNode.Current;
				yield return obj2;
			}
			processNode = null;
		}
		yield break;
	}

	private IEnumerator ProcessNode(JSONArray nodes, int nodeId, bool loadMips, Transform parent)
	{
		bool hasSkipped = false;
		if (Time.realtimeSinceStartup - this.m_processingNodesStart > 0.014285714f)
		{
			this.m_processingNodesStart = Time.realtimeSinceStartup;
			hasSkipped = true;
			yield return null;
		}
		JSONNode node = nodes[nodeId];
		GameObject nodeGameObject = this.m_Nodes[nodeId];
		Transform nodeTransform = nodeGameObject.transform;
		string nodeName = node["name"].Value;
		nodeTransform.name = nodeName;
		nodeTransform.SetParent(parent, false);
		JSONArray asArray = node["children"].AsArray;
		if (asArray.Count > 0)
		{
			foreach (JSONNode jsonnode in asArray.Values)
			{
				int asInt = jsonnode.AsInt;
				IEnumerator processNode = this.ProcessNode(nodes, asInt, loadMips, nodeTransform);
				while (processNode.MoveNext())
				{
					object obj = processNode.Current;
					yield return obj;
				}
				processNode = null;
			}
			JSONNode.ValueEnumerator valueEnumerator = default(JSONNode.ValueEnumerator);
		}
		if (nodeName.StartsWith("batteryIndicator"))
		{
			nodeGameObject.SetActive(false);
			yield break;
		}
		if (node["mesh"] != null)
		{
			int asInt2 = node["mesh"].AsInt;
			OVRMeshData ovrmeshData = this.ProcessMesh(this.m_jsonData["meshes"][asInt2], loadMips);
			if (node["skin"] != null)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = nodeGameObject.AddComponent<SkinnedMeshRenderer>();
				skinnedMeshRenderer.sharedMesh = ovrmeshData.mesh;
				skinnedMeshRenderer.sharedMaterial = ovrmeshData.material;
				int asInt3 = node["skin"].AsInt;
				this.ProcessSkin(this.m_jsonData["skins"][asInt3], skinnedMeshRenderer);
			}
			else
			{
				nodeGameObject.AddComponent<MeshFilter>().sharedMesh = ovrmeshData.mesh;
				nodeGameObject.AddComponent<MeshRenderer>().sharedMaterial = ovrmeshData.material;
			}
			if (ovrmeshData.morphTargets != null)
			{
				this.m_morphTargetHandlers[nodeId] = new OVRGLTFAnimationNodeMorphTargetHandler(ovrmeshData);
			}
		}
		JSONArray asArray2 = node["translation"].AsArray;
		JSONArray asArray3 = node["rotation"].AsArray;
		JSONArray asArray4 = node["scale"].AsArray;
		if (asArray2.Count > 0 || asArray3.Count > 0)
		{
			Vector3 zero = Vector3.zero;
			Quaternion identity = Quaternion.identity;
			if (asArray2.Count > 0)
			{
				zero = new Vector3(asArray2[0] * OVRGLTFLoader.GLTFToUnitySpace.x, asArray2[1] * OVRGLTFLoader.GLTFToUnitySpace.y, asArray2[2] * OVRGLTFLoader.GLTFToUnitySpace.z);
			}
			if (asArray3.Count > 0)
			{
				identity = new Quaternion(asArray3[0] * OVRGLTFLoader.GLTFToUnitySpace.x * -1f, asArray3[1] * OVRGLTFLoader.GLTFToUnitySpace.y * -1f, asArray3[2] * OVRGLTFLoader.GLTFToUnitySpace.z * -1f, asArray3[3]);
			}
			nodeTransform.SetPositionAndRotation(zero, identity);
		}
		if (asArray4.Count > 0)
		{
			nodeTransform.localScale = new Vector3(asArray4[0], asArray4[1], asArray4[2]);
			nodeTransform.gameObject.SetActive(nodeTransform.gameObject.transform.localScale != Vector3.zero);
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float processingNodesStart = this.m_processingNodesStart;
		if (!hasSkipped && Time.realtimeSinceStartup - this.m_processingNodesStart > 0.014285714f)
		{
			this.m_processingNodesStart = Time.realtimeSinceStartup;
			yield return null;
		}
		yield break;
	}

	private OVRMeshData ProcessMesh(JSONNode meshNode, bool loadMips)
	{
		OVRMeshData result = default(OVRMeshData);
		int num = 0;
		JSONNode jsonnode = meshNode["primitives"];
		int[] array = new int[jsonnode.Count];
		for (int i = 0; i < jsonnode.Count; i++)
		{
			JSONNode jsonnode2 = jsonnode[i]["attributes"]["POSITION"];
			JSONNode jsonnode3 = this.m_jsonData["accessors"][jsonnode2.AsInt];
			array[i] = jsonnode3["count"];
			num += array[i];
		}
		int[][] array2 = new int[jsonnode.Count][];
		JSONNode jsonnode4 = jsonnode[0]["material"];
		if (jsonnode4 != null)
		{
			OVRMaterialData ovrmaterialData = this.ProcessMaterial(jsonnode4.AsInt);
			ovrmaterialData.texture = this.ProcessTexture(ovrmaterialData.textureId);
			this.TranscodeTexture(ref ovrmaterialData.texture);
			int asInt = jsonnode4.AsInt;
			Material material;
			if (this.m_materials.TryGetValue(asInt, out material))
			{
				result.material = material;
			}
			else
			{
				Material material2 = this.CreateUnityMaterial(ovrmaterialData, loadMips);
				this.m_materials.Add(asInt, material2);
				result.material = material2;
			}
		}
		OVRMeshAttributes ovrmeshAttributes = default(OVRMeshAttributes);
		OVRMeshAttributes[] array3 = null;
		int num2 = 0;
		for (int j = 0; j < jsonnode.Count; j++)
		{
			JSONNode jsonnode5 = jsonnode[j];
			int asInt2 = jsonnode5["indices"].AsInt;
			this._dataAccessor.Seek(asInt2, false);
			array2[j] = this._dataAccessor.ReadInt();
			OVRGLTFLoader.FlipTriangleIndices(ref array2[j]);
			ovrmeshAttributes = this.ReadMeshAttributes(jsonnode5["attributes"], num, num2);
			JSONNode jsonnode6 = jsonnode5["targets"];
			if (jsonnode6 != null)
			{
				array3 = new OVRMeshAttributes[jsonnode6.Count];
				for (int k = 0; k < jsonnode6.Count; k++)
				{
					array3[k] = this.ReadMeshAttributes(jsonnode6[k], num, num2);
				}
			}
			num2 += array[j];
		}
		Mesh mesh = new Mesh();
		mesh.vertices = ovrmeshAttributes.vertices;
		mesh.normals = ovrmeshAttributes.normals;
		mesh.tangents = ovrmeshAttributes.tangents;
		mesh.colors = ovrmeshAttributes.colors;
		mesh.uv = ovrmeshAttributes.texcoords;
		mesh.boneWeights = ovrmeshAttributes.boneWeights;
		mesh.subMeshCount = jsonnode.Count;
		int num3 = 0;
		for (int l = 0; l < jsonnode.Count; l++)
		{
			mesh.SetIndices(array2[l], MeshTopology.Triangles, l, false, num3);
			num3 += array[l];
		}
		mesh.RecalculateBounds();
		result.mesh = mesh;
		result.morphTargets = array3;
		if (array3 != null)
		{
			result.baseAttributes = ovrmeshAttributes;
		}
		return result;
	}

	private static void FlipTriangleIndices(ref int[] indices)
	{
		for (int i = 0; i < indices.Length; i += 3)
		{
			ref int ptr = ref indices[i];
			int[] array = indices;
			int num = i + 2;
			int num2 = indices[i + 2];
			int num3 = indices[i];
			ptr = num2;
			array[num] = num3;
		}
	}

	private OVRMeshAttributes ReadMeshAttributes(JSONNode jsonAttributes, int totalVertexCount, int vertexOffset)
	{
		OVRMeshAttributes result = default(OVRMeshAttributes);
		JSONNode jsonnode = jsonAttributes["POSITION"];
		if (jsonnode != null)
		{
			this._dataAccessor.Seek(jsonnode.AsInt, false);
			result.vertices = this._dataAccessor.ReadVector3(OVRGLTFLoader.GLTFToUnitySpace);
		}
		jsonnode = jsonAttributes["NORMAL"];
		if (jsonnode != null)
		{
			this._dataAccessor.Seek(jsonnode.AsInt, false);
			result.normals = this._dataAccessor.ReadVector3(OVRGLTFLoader.GLTFToUnitySpace);
		}
		jsonnode = jsonAttributes["TANGENT"];
		if (jsonnode != null)
		{
			this._dataAccessor.Seek(jsonnode.AsInt, false);
			result.tangents = this._dataAccessor.ReadVector4(OVRGLTFLoader.GLTFToUnityTangent);
		}
		jsonnode = jsonAttributes["TEXCOORD_0"];
		if (jsonnode != null)
		{
			this._dataAccessor.Seek(jsonnode.AsInt, false);
			result.texcoords = this._dataAccessor.ReadVector2();
		}
		jsonnode = jsonAttributes["COLOR_0"];
		if (jsonnode != null)
		{
			this._dataAccessor.Seek(jsonnode.AsInt, false);
			result.colors = this._dataAccessor.ReadColor();
		}
		jsonnode = jsonAttributes["WEIGHTS_0"];
		if (jsonnode != null)
		{
			result.boneWeights = new BoneWeight[totalVertexCount];
			this._dataAccessor.Seek(jsonnode.AsInt, false);
			this._dataAccessor.ReadWeights(ref result.boneWeights);
			JSONNode jsonnode2 = jsonAttributes["JOINTS_0"];
			this._dataAccessor.Seek(jsonnode2.AsInt, false);
			this._dataAccessor.ReadJoints(ref result.boneWeights);
		}
		return result;
	}

	private void ProcessSkin(JSONNode skinNode, SkinnedMeshRenderer renderer)
	{
		Matrix4x4[] bindposes = null;
		if (skinNode["inverseBindMatrices"] != null)
		{
			int asInt = skinNode["inverseBindMatrices"].AsInt;
			this._dataAccessor.Seek(asInt, false);
			bindposes = this._dataAccessor.ReadMatrix4x4(OVRGLTFLoader.GLTFToUnitySpace);
		}
		if (skinNode["skeleton"] != null)
		{
			int asInt2 = skinNode["skeleton"].AsInt;
			renderer.rootBone = this.m_Nodes[asInt2].transform;
		}
		Transform[] array = null;
		if (skinNode["joints"] != null)
		{
			JSONArray asArray = skinNode["joints"].AsArray;
			array = new Transform[asArray.Count];
			for (int i = 0; i < asArray.Count; i++)
			{
				array[i] = this.m_Nodes[asArray[i]].transform;
			}
		}
		renderer.sharedMesh.bindposes = bindposes;
		renderer.bones = array;
	}

	private OVRMaterialData ProcessMaterial(int matId)
	{
		OVRMaterialData result = default(OVRMaterialData);
		JSONNode jsonnode = this.m_jsonData["materials"][matId];
		JSONNode jsonnode2 = jsonnode["alphaMode"];
		bool flag = jsonnode2 != null && jsonnode2.Value == "BLEND";
		JSONNode jsonnode3 = jsonnode["pbrMetallicRoughness"];
		result.baseColorFactor = Color.white;
		JSONNode jsonnode4 = jsonnode3["baseColorFactor"];
		if (jsonnode4 != null)
		{
			result.baseColorFactor = new Color(jsonnode4[0].AsFloat, jsonnode4[1].AsFloat, jsonnode4[2].AsFloat, jsonnode4[3].AsFloat);
		}
		JSONNode jsonnode5 = jsonnode3["baseColorTexture"];
		if (jsonnode5 != null)
		{
			int asInt = jsonnode5["index"].AsInt;
			result.textureId = asInt;
		}
		else
		{
			JSONNode jsonnode6 = jsonnode["emissiveTexture"];
			if (jsonnode6 != null)
			{
				int asInt2 = jsonnode6["index"].AsInt;
				result.textureId = asInt2;
			}
		}
		result.shader = (flag ? this.m_AlphaBlendShader : this.m_Shader);
		return result;
	}

	private OVRTextureData ProcessTexture(int textureId)
	{
		JSONNode jsonnode = this.m_jsonData["textures"][textureId];
		int aIndex = -1;
		JSONNode jsonnode2 = jsonnode["extensions"];
		if (jsonnode2 != null)
		{
			JSONNode jsonnode3 = jsonnode2["KHR_texture_basisu"];
			if (jsonnode3 != null)
			{
				aIndex = jsonnode3["source"].AsInt;
			}
		}
		else
		{
			aIndex = jsonnode["source"].AsInt;
		}
		JSONNode jsonnode4 = this.m_jsonData["images"][aIndex];
		OVRTextureData result = default(OVRTextureData);
		string value = jsonnode4["uri"].Value;
		if (!string.IsNullOrEmpty(value))
		{
			result.uri = value;
			return result;
		}
		int asInt = jsonnode4["bufferView"].AsInt;
		string value2 = jsonnode4["mimeType"].Value;
		if (!(value2 == "image/ktx2"))
		{
			if (!(value2 == "image/png"))
			{
				Debug.LogWarning("Unsupported image mimeType '" + jsonnode4["mimeType"].Value + "'");
			}
			else
			{
				result.data = this._dataAccessor.ReadBuffer(asInt);
				result.format = OVRTextureFormat.PNG;
			}
		}
		else
		{
			result.data = this._dataAccessor.ReadBuffer(asInt);
			result.format = OVRTextureFormat.KTX2;
		}
		return result;
	}

	private void TranscodeTexture(ref OVRTextureData textureData)
	{
		if (!string.IsNullOrEmpty(textureData.uri))
		{
			return;
		}
		if (textureData.format == OVRTextureFormat.KTX2)
		{
			OVRKtxTexture.Load(textureData.data, ref textureData);
			return;
		}
		if (textureData.format != OVRTextureFormat.PNG)
		{
			Debug.LogWarning("Only KTX2 textures can be trascoded.");
		}
	}

	private Material CreateUnityMaterial(OVRMaterialData matData, bool loadMips)
	{
		Material material = new Material(matData.shader);
		material.color = matData.baseColorFactor;
		if (loadMips && material.HasProperty("_MainTexMMBias"))
		{
			material.SetFloat("_MainTexMMBias", this.m_TextureMipmapBias);
		}
		Texture2D texture2D = null;
		bool flag = false;
		if (this.m_textures.TryGetValue(matData.textureId, out texture2D))
		{
			material.mainTexture = texture2D;
			return material;
		}
		if (matData.texture.format == OVRTextureFormat.KTX2)
		{
			texture2D = new Texture2D(matData.texture.width, matData.texture.height, matData.texture.transcodedFormat, loadMips);
			texture2D.LoadRawTextureData(matData.texture.data);
			flag = true;
		}
		else if (matData.texture.format == OVRTextureFormat.PNG)
		{
			texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, loadMips);
			texture2D.LoadImage(matData.texture.data);
			flag = true;
		}
		else if (!string.IsNullOrEmpty(matData.texture.uri))
		{
			Func<string, Material, Texture2D> func = this.textureUriHandler;
			texture2D = ((func != null) ? func(matData.texture.uri, material) : null);
		}
		if (!texture2D)
		{
			return material;
		}
		if (flag)
		{
			OVRGLTFLoader.ApplyTextureQuality(this.m_TextureQuality, ref texture2D);
			texture2D.Apply(false, true);
		}
		this.m_textures[matData.textureId] = texture2D;
		material.mainTexture = texture2D;
		return material;
	}

	private OVRGLTFInputNode GetInputNodeType(string name)
	{
		foreach (KeyValuePair<string, OVRGLTFInputNode> keyValuePair in OVRGLTFLoader.InputNodeNameMap)
		{
			if (name.Contains(keyValuePair.Key))
			{
				return keyValuePair.Value;
			}
		}
		return OVRGLTFInputNode.None;
	}

	private IEnumerator ProcessAnimations()
	{
		JSONNode jsonnode = this.m_jsonData["animations"];
		int animationIndex = 0;
		float processingStart = Time.realtimeSinceStartup;
		foreach (KeyValuePair<string, JSONNode> aKeyValue in jsonnode.AsArray)
		{
			JSONNode jsonnode2 = aKeyValue;
			Dictionary<int, OVRGLTFAnimatinonNode> dictionary = new Dictionary<int, OVRGLTFAnimatinonNode>();
			foreach (KeyValuePair<string, JSONNode> aKeyValue2 in jsonnode2["channels"].AsArray)
			{
				JSONNode jsonnode3 = aKeyValue2;
				int asInt = jsonnode3["target"]["node"].AsInt;
				OVRGLTFInputNode inputNodeType = this.GetInputNodeType(this.m_Nodes[asInt].name);
				OVRGLTFAnimatinonNode ovrgltfanimatinonNode;
				if (!dictionary.TryGetValue(asInt, out ovrgltfanimatinonNode))
				{
					OVRGLTFAnimationNodeMorphTargetHandler morphTargetHandler;
					this.m_morphTargetHandlers.TryGetValue(asInt, out morphTargetHandler);
					ovrgltfanimatinonNode = (dictionary[asInt] = new OVRGLTFAnimatinonNode(inputNodeType, this.m_Nodes[asInt], morphTargetHandler));
				}
				if (inputNodeType != OVRGLTFInputNode.None && !this.m_InputAnimationNodes.ContainsKey(inputNodeType))
				{
					this.m_InputAnimationNodes[inputNodeType] = ovrgltfanimatinonNode;
				}
				ovrgltfanimatinonNode.AddChannel(jsonnode3, jsonnode2["samplers"], this._dataAccessor);
			}
			this.m_AnimationLookup[animationIndex] = dictionary.Values.ToArray<OVRGLTFAnimatinonNode>();
			int num = animationIndex;
			animationIndex = num + 1;
			if (Time.realtimeSinceStartup - processingStart > 0.014285714f)
			{
				processingStart = Time.realtimeSinceStartup;
				yield return null;
			}
		}
		JSONNode.Enumerator enumerator = default(JSONNode.Enumerator);
		yield break;
	}

	private const float LoadingMaxTimePerFrame = 0.014285714f;

	private readonly Func<Stream> m_deferredStream;

	private JSONNode m_jsonData;

	private Stream m_glbStream;

	private GameObject[] m_Nodes;

	private Dictionary<OVRGLTFInputNode, OVRGLTFAnimatinonNode> m_InputAnimationNodes;

	private Dictionary<int, OVRGLTFAnimatinonNode[]> m_AnimationLookup;

	private Dictionary<int, OVRGLTFAnimationNodeMorphTargetHandler> m_morphTargetHandlers;

	private Shader m_Shader = Shader.Find("Legacy Shaders/Diffuse");

	private Shader m_AlphaBlendShader = Shader.Find("Unlit/Transparent");

	private OVRTextureQualityFiltering m_TextureQuality;

	private float m_TextureMipmapBias;

	public OVRGLTFScene scene;

	public static readonly Vector3 GLTFToUnitySpace = new Vector3(-1f, 1f, 1f);

	public static readonly Vector3 GLTFToUnityTangent = new Vector4(-1f, 1f, 1f, -1f);

	public static readonly Vector4 GLTFToUnitySpace_Rotation = new Vector4(1f, -1f, -1f, 1f);

	private static Dictionary<string, OVRGLTFInputNode> InputNodeNameMap = new Dictionary<string, OVRGLTFInputNode>
	{
		{
			"button_a",
			OVRGLTFInputNode.Button_A_X
		},
		{
			"button_x",
			OVRGLTFInputNode.Button_A_X
		},
		{
			"button_b",
			OVRGLTFInputNode.Button_B_Y
		},
		{
			"button_y",
			OVRGLTFInputNode.Button_B_Y
		},
		{
			"button_oculus",
			OVRGLTFInputNode.Button_Oculus_Menu
		},
		{
			"trigger_front",
			OVRGLTFInputNode.Trigger_Front
		},
		{
			"trigger_grip",
			OVRGLTFInputNode.Trigger_Grip
		},
		{
			"thumbstick",
			OVRGLTFInputNode.ThumbStick
		}
	};

	public Func<string, Material, Texture2D> textureUriHandler;

	private Dictionary<int, Texture2D> m_textures;

	private Dictionary<int, Material> m_materials;

	private float m_processingNodesStart;

	private OVRGLTFAccessor _dataAccessor;
}
