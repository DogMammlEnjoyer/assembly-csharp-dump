using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB3_MeshBakerGrouper : MonoBehaviour, MB_IMeshBakerSettingsHolder
{
	public MB_IMeshBakerSettings GetMeshBakerSettings()
	{
		if (this.meshBakerSettingsAsset == null)
		{
			if (this.meshBakerSettings == null)
			{
				this.meshBakerSettings = new MB3_MeshCombinerSettingsData();
			}
			return this.meshBakerSettings;
		}
		return this.meshBakerSettingsAsset.GetMeshBakerSettings();
	}

	public void GetMeshBakerSettingsAsSerializedProperty(out string propertyName, out Object targetObj)
	{
		if (this.meshBakerSettingsAsset == null)
		{
			targetObj = this;
			propertyName = "meshBakerSettings";
			return;
		}
		targetObj = this.meshBakerSettingsAsset;
		propertyName = "data";
	}

	private void OnDrawGizmosSelected()
	{
		if (this.grouper == null)
		{
			this.grouper = this.CreateGrouper(this.clusterType);
		}
		this.grouper.DrawGizmos(this.sourceObjectBounds, this.data);
	}

	public MB3_MeshBakerGrouperBehaviour CreateGrouper(MB3_MeshBakerGrouper.ClusterType t)
	{
		if (t == MB3_MeshBakerGrouper.ClusterType.grid)
		{
			this.grouper = new MB3_MeshBakerGrouperGrid();
		}
		if (t == MB3_MeshBakerGrouper.ClusterType.pie)
		{
			this.grouper = new MB3_MeshBakerGrouperPie();
		}
		if (t == MB3_MeshBakerGrouper.ClusterType.agglomerative)
		{
			this.grouper = new MB3_MeshBakerGrouperCluster();
		}
		if (t == MB3_MeshBakerGrouper.ClusterType.none)
		{
			this.grouper = new MB3_MeshBakerGrouperNone();
		}
		return this.grouper;
	}

	public void DeleteAllChildMeshBakers()
	{
		foreach (MB3_MeshBakerCommon mb3_MeshBakerCommon in base.GetComponentsInChildren<MB3_MeshBakerCommon>())
		{
			MB_Utility.Destroy(mb3_MeshBakerCommon.meshCombiner.resultSceneObject);
			MB_Utility.Destroy(mb3_MeshBakerCommon.gameObject);
		}
	}

	public List<MB3_MeshBakerCommon> GenerateMeshBakers()
	{
		MB3_TextureBaker component = base.GetComponent<MB3_TextureBaker>();
		if (component == null)
		{
			Debug.LogError("There must be an MB3_TextureBaker attached to this game object.");
			return new List<MB3_MeshBakerCommon>();
		}
		if (component.GetObjectsToCombine().Count == 0)
		{
			Debug.LogError("The MB3_MeshBakerGrouper creates clusters based on the objects to combine in the MB3_TextureBaker component. There were no objects in this list.");
			return new List<MB3_MeshBakerCommon>();
		}
		if (this.parentSceneObject == null || !MB_Utility.IsSceneInstance(this.parentSceneObject.gameObject))
		{
			GameObject gameObject = new GameObject("CombinedMeshes-" + base.name);
			this.parentSceneObject = gameObject.transform;
		}
		List<GameObject> objectsToCombine = component.GetObjectsToCombine();
		MB3_MeshBakerCommon[] componentsInChildren = base.GetComponentsInChildren<MB3_MeshBakerCommon>();
		bool flag = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			List<GameObject> objectsToCombine2 = componentsInChildren[i].GetObjectsToCombine();
			for (int j = 0; j < objectsToCombine2.Count; j++)
			{
				if (objectsToCombine2[j] != null && objectsToCombine.Contains(objectsToCombine2[j]))
				{
					flag = true;
					break;
				}
			}
		}
		bool flag2 = true;
		if (flag)
		{
			flag2 = false;
			Debug.LogError("There are previously generated MeshBaker objects. Please use the editor to delete or replace them");
		}
		if (Application.isPlaying && this.prefabOptions_autoGeneratePrefabs)
		{
			Debug.LogError("Can only use Auto Generate Prefabs in the editor when the game is not playing.");
			flag2 = false;
		}
		List<MB3_MeshBakerCommon> result;
		if (flag2)
		{
			if (flag)
			{
				this.DeleteAllChildMeshBakers();
			}
			if (this.grouper == null || this.grouper.GetClusterType() != this.clusterType)
			{
				this.grouper = this.CreateGrouper(this.clusterType);
			}
			result = this.grouper.DoClustering(component, this, this.data);
		}
		else
		{
			result = new List<MB3_MeshBakerCommon>();
		}
		return result;
	}

	public static readonly Color WHITE_TRANSP = new Color(1f, 1f, 1f, 0.1f);

	public MB3_MeshBakerGrouperBehaviour grouper;

	public MB3_MeshBakerGrouper.ClusterType clusterType;

	public Transform parentSceneObject;

	public GrouperData data;

	[HideInInspector]
	public Bounds sourceObjectBounds = new Bounds(Vector3.zero, Vector3.one);

	public string prefabOptions_outputFolder = "";

	public bool prefabOptions_autoGeneratePrefabs;

	public bool prefabOptions_mergeOutputIntoSinglePrefab;

	public MB3_MeshCombinerSettings meshBakerSettingsAsset;

	public MB3_MeshCombinerSettingsData meshBakerSettings;

	public enum ClusterType
	{
		none,
		grid,
		pie,
		agglomerative
	}
}
