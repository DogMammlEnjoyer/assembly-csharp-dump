using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class MB3_MeshBakerCommon : MB3_MeshBakerRoot
{
	public static int VERSION
	{
		get
		{
			return 100;
		}
	}

	public abstract MB3_MeshCombiner meshCombiner { get; }

	public bool clearBuffersAfterBake
	{
		get
		{
			if (this.version < 100)
			{
				this.UpgradeToCurrentVersionIfNecessary();
				return this._clearBuffersAfterBake;
			}
			Debug.LogError("MeshBaker.clearBuffersAfterBake is deprecated, use the meshCombiner.clearBuffersAfterBake field");
			return this.meshCombiner.clearBuffersAfterBake;
		}
		set
		{
			if (this.version < 100)
			{
				this.UpgradeToCurrentVersionIfNecessary();
				this._clearBuffersAfterBake = value;
				return;
			}
			Debug.LogError("MeshBaker.clearBuffersAfterBake is deprecated, use the meshCombiner.clearBuffersAfterBake field");
			this.meshCombiner.clearBuffersAfterBake = value;
		}
	}

	public void UpgradeToCurrentVersionIfNecessary()
	{
		if (this.version == MB3_MeshBakerCommon.VERSION)
		{
			return;
		}
		if (this.version < 100)
		{
			this.meshCombiner.clearBuffersAfterBake = this._clearBuffersAfterBake;
		}
		this.version = MB3_MeshBakerCommon.VERSION;
	}

	public override MB2_TextureBakeResults textureBakeResults
	{
		get
		{
			return this.meshCombiner.textureBakeResults;
		}
		set
		{
			this.meshCombiner.textureBakeResults = value;
		}
	}

	public override List<GameObject> GetObjectsToCombine()
	{
		if (!this.useObjsToMeshFromTexBaker)
		{
			if (this.objsToMesh == null)
			{
				this.objsToMesh = new List<GameObject>();
			}
			return this.objsToMesh;
		}
		MB3_TextureBaker component = base.gameObject.GetComponent<MB3_TextureBaker>();
		if (component == null && base.gameObject.transform.parent != null)
		{
			component = base.gameObject.transform.parent.GetComponent<MB3_TextureBaker>();
		}
		if (component != null)
		{
			return component.GetObjectsToCombine();
		}
		Debug.LogWarning("Use Objects To Mesh From Texture Baker was checked but no texture baker");
		return new List<GameObject>();
	}

	[ContextMenu("Purge Objects to Combine of null references")]
	public override void PurgeNullsFromObjectsToCombine()
	{
		if (!this.useObjsToMeshFromTexBaker)
		{
			if (this.objsToMesh == null)
			{
				this.objsToMesh = new List<GameObject>();
			}
			Debug.Log(string.Format("Purged {0} null references from objects to combine list.", this.objsToMesh.RemoveAll((GameObject obj) => obj == null)));
			return;
		}
		MB3_TextureBaker component = base.gameObject.GetComponent<MB3_TextureBaker>();
		if (component == null && base.gameObject.transform.parent != null)
		{
			component = base.gameObject.transform.parent.GetComponent<MB3_TextureBaker>();
		}
		if (component != null)
		{
			component.PurgeNullsFromObjectsToCombine();
			return;
		}
		Debug.LogWarning("Use Objects To Mesh From Texture Baker was checked but no texture baker, could not purge");
	}

	public void EnableDisableSourceObjectRenderers(bool show)
	{
		for (int i = 0; i < this.GetObjectsToCombine().Count; i++)
		{
			GameObject gameObject = this.GetObjectsToCombine()[i];
			if (gameObject != null)
			{
				Renderer renderer = MB_Utility.GetRenderer(gameObject);
				if (renderer != null)
				{
					renderer.enabled = show;
				}
				LODGroup componentInParent = renderer.GetComponentInParent<LODGroup>();
				if (componentInParent != null)
				{
					bool flag = true;
					LOD[] lods = componentInParent.GetLODs();
					for (int j = 0; j < lods.Length; j++)
					{
						for (int k = 0; k < lods[j].renderers.Length; k++)
						{
							if (lods[j].renderers[k] != renderer)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						componentInParent.enabled = show;
					}
				}
			}
		}
	}

	public virtual void ClearMesh()
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.ClearMesh();
	}

	public virtual void ClearMesh(MB2_EditorMethodsInterface editorMethods)
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.ClearMesh(editorMethods);
	}

	public virtual void DestroyMesh()
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.DestroyMesh();
	}

	public virtual void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
	{
		this.meshCombiner.DestroyMeshEditor(editorMethods);
	}

	public virtual int GetNumObjectsInCombined()
	{
		return this.meshCombiner.GetNumObjectsInCombined();
	}

	public MB3_TextureBaker GetTextureBaker()
	{
		MB3_TextureBaker component = base.GetComponent<MB3_TextureBaker>();
		if (component != null)
		{
			return component;
		}
		if (base.transform.parent != null && base.gameObject.transform.parent != null)
		{
			return base.transform.parent.GetComponent<MB3_TextureBaker>();
		}
		return null;
	}

	public abstract bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true);

	public abstract bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource = true);

	public virtual bool Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.name = base.name + "-mesh";
		bool result = this.meshCombiner.Apply(uv2GenerationMethod);
		if (this.parentSceneObject != null && this.meshCombiner.resultSceneObject != null)
		{
			this.meshCombiner.resultSceneObject.transform.parent = this.parentSceneObject;
		}
		return result;
	}

	public virtual bool Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapesFlag = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.name = base.name + "-mesh";
		bool result = this.meshCombiner.Apply(triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, colors, bones, blendShapesFlag, uv2GenerationMethod);
		if (this.parentSceneObject != null && this.meshCombiner.resultSceneObject != null)
		{
			this.meshCombiner.resultSceneObject.transform.parent = this.parentSceneObject;
		}
		return result;
	}

	public virtual bool CombinedMeshContains(GameObject go)
	{
		return this.meshCombiner.CombinedMeshContains(go);
	}

	public virtual bool UpdateGameObjects(GameObject[] gos)
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.name = base.name + "-mesh";
		return this.meshCombiner.UpdateGameObjects(gos, true, true, true, true, true, false, false, false, false, false, false, false, false, false);
	}

	public virtual bool UpdateGameObjects(GameObject[] gos, bool updateBounds)
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.name = base.name + "-mesh";
		return this.meshCombiner.UpdateGameObjects(gos, true, true, true, true, true, false, false, false, false, false, false, false, false, false);
	}

	public virtual bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV1, bool updateUV2, bool updateColors, bool updateSkinningInfo)
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.name = base.name + "-mesh";
		return this.meshCombiner.UpdateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, false, false, updateColors, updateSkinningInfo);
	}

	public virtual bool UpdateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateUV5, bool updateUV6, bool updateUV7, bool updateUV8, bool updateColors, bool updateSkinningInfo)
	{
		this.UpgradeToCurrentVersionIfNecessary();
		this.meshCombiner.name = base.name + "-mesh";
		return this.meshCombiner.UpdateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateUV5, updateUV6, updateUV7, updateUV8, updateColors, updateSkinningInfo);
	}

	public virtual void UpdateSkinnedMeshApproximateBounds()
	{
		if (this._ValidateForUpdateSkinnedMeshBounds())
		{
			this.meshCombiner.UpdateSkinnedMeshApproximateBounds();
		}
	}

	public virtual void UpdateSkinnedMeshApproximateBoundsFromBones()
	{
		if (this._ValidateForUpdateSkinnedMeshBounds())
		{
			this.meshCombiner.UpdateSkinnedMeshApproximateBoundsFromBones();
		}
	}

	public virtual void UpdateSkinnedMeshApproximateBoundsFromBounds()
	{
		if (this._ValidateForUpdateSkinnedMeshBounds())
		{
			this.meshCombiner.UpdateSkinnedMeshApproximateBoundsFromBounds();
		}
	}

	protected virtual bool _ValidateForUpdateSkinnedMeshBounds()
	{
		if (this.meshCombiner.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
		{
			Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
			return false;
		}
		if (this.meshCombiner.resultSceneObject == null)
		{
			Debug.LogWarning("Result Scene Object does not exist. No point in calling UpdateSkinnedMeshApproximateBounds.");
			return false;
		}
		if (this.meshCombiner.resultSceneObject.GetComponentInChildren<SkinnedMeshRenderer>() == null)
		{
			Debug.LogWarning("No SkinnedMeshRenderer on result scene object.");
			return false;
		}
		return true;
	}

	public int version;

	[NonReorderable]
	public List<GameObject> objsToMesh;

	public bool useObjsToMeshFromTexBaker = true;

	[FormerlySerializedAs("clearBuffersAfterBake")]
	[SerializeField]
	[HideInInspector]
	private bool _clearBuffersAfterBake;

	public string bakeAssetsInPlaceFolderPath;

	[HideInInspector]
	public GameObject resultPrefab;

	[HideInInspector]
	public bool resultPrefabLeaveInstanceInSceneAfterBake;

	[HideInInspector]
	public Transform parentSceneObject;
}
