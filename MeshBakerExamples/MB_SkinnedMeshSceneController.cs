using System;
using System.Collections.Generic;
using UnityEngine;

public class MB_SkinnedMeshSceneController : MonoBehaviour
{
	private void Start()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.workerPrefab);
		gameObject.transform.position = new Vector3(1.31f, 0.985f, -0.25f);
		Animation component = gameObject.GetComponent<Animation>();
		component.wrapMode = WrapMode.Loop;
		component.cullingType = AnimationCullingType.AlwaysAnimate;
		component.Play("run");
		List<GameObject> objectsToCombine = this.skinnedMeshBaker.GetObjectsToCombine();
		GameObject[] array = new GameObject[objectsToCombine.Count + 1];
		objectsToCombine.CopyTo(array, 0);
		array[array.Length - 1] = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
		this.skinnedMeshBaker.ClearMesh();
		this.skinnedMeshBaker.AddDeleteGameObjects(array, null, true);
		this.skinnedMeshBaker.Apply(null);
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Add/Remove Sword", Array.Empty<GUILayoutOption>()))
		{
			if (this.swordInstance == null)
			{
				Transform parent = this.SearchHierarchyForBone(this.targetCharacter.transform, "RightHandAttachPoint");
				this.swordInstance = Object.Instantiate<GameObject>(this.swordPrefab);
				this.swordInstance.transform.parent = parent;
				this.swordInstance.transform.localPosition = Vector3.zero;
				this.swordInstance.transform.localRotation = Quaternion.identity;
				this.swordInstance.transform.localScale = Vector3.one;
				MeshRenderer componentInChildren = this.swordInstance.GetComponentInChildren<MeshRenderer>();
				componentInChildren.gameObject.name = "Sword";
				GameObject[] gos = new GameObject[]
				{
					componentInChildren.gameObject
				};
				this.skinnedMeshBaker.AddDeleteGameObjects(gos, null, true);
				this.skinnedMeshBaker.Apply(null);
				Debug.Log("Done adding sword.");
			}
			else if (this.skinnedMeshBaker.CombinedMeshContains(this.swordInstance.GetComponentInChildren<MeshRenderer>().gameObject))
			{
				GameObject[] deleteGOs = new GameObject[]
				{
					this.swordInstance.GetComponentInChildren<MeshRenderer>().gameObject
				};
				this.skinnedMeshBaker.AddDeleteGameObjects(null, deleteGOs, true);
				this.skinnedMeshBaker.Apply(null);
				Object.Destroy(this.swordInstance);
				Debug.Log("Done deleting sword.");
				this.swordInstance = null;
			}
		}
		if (GUILayout.Button("Add/Remove Hat", Array.Empty<GUILayoutOption>()))
		{
			if (this.hatInstance == null)
			{
				Transform parent2 = this.SearchHierarchyForBone(this.targetCharacter.transform, "HeadAttachPoint");
				this.hatInstance = Object.Instantiate<GameObject>(this.hatPrefab);
				this.hatInstance.transform.parent = parent2;
				this.hatInstance.transform.localPosition = Vector3.zero;
				this.hatInstance.transform.localRotation = Quaternion.identity;
				this.hatInstance.transform.localScale = Vector3.one;
				MeshRenderer componentInChildren2 = this.hatInstance.GetComponentInChildren<MeshRenderer>();
				componentInChildren2.gameObject.name = "Hat";
				GameObject[] gos2 = new GameObject[]
				{
					componentInChildren2.gameObject
				};
				this.skinnedMeshBaker.AddDeleteGameObjects(gos2, null, true);
				this.skinnedMeshBaker.Apply(null);
				Debug.Log("Done adding Hat");
			}
			else if (this.skinnedMeshBaker.CombinedMeshContains(this.hatInstance.GetComponentInChildren<MeshRenderer>().gameObject))
			{
				GameObject[] deleteGOs2 = new GameObject[]
				{
					this.hatInstance.GetComponentInChildren<MeshRenderer>().gameObject
				};
				this.skinnedMeshBaker.AddDeleteGameObjects(null, deleteGOs2, true);
				this.skinnedMeshBaker.Apply(null);
				Object.Destroy(this.hatInstance);
				Debug.Log("Done deleting Hat");
				this.hatInstance = null;
			}
		}
		if (GUILayout.Button("Add/Remove Glasses", Array.Empty<GUILayoutOption>()))
		{
			if (this.glassesInstance == null)
			{
				Transform parent3 = this.SearchHierarchyForBone(this.targetCharacter.transform, "NoseAttachPoint");
				this.glassesInstance = Object.Instantiate<GameObject>(this.glassesPrefab);
				this.glassesInstance.transform.parent = parent3;
				this.glassesInstance.transform.localPosition = Vector3.zero;
				this.glassesInstance.transform.localRotation = Quaternion.identity;
				this.glassesInstance.transform.localScale = Vector3.one;
				MeshRenderer componentInChildren3 = this.glassesInstance.GetComponentInChildren<MeshRenderer>();
				componentInChildren3.gameObject.name = "Glasses";
				GameObject[] gos3 = new GameObject[]
				{
					componentInChildren3.gameObject
				};
				this.skinnedMeshBaker.AddDeleteGameObjects(gos3, null, true);
				this.skinnedMeshBaker.Apply(null);
				Debug.Log("Done adding glasses");
				return;
			}
			if (this.skinnedMeshBaker.CombinedMeshContains(this.glassesInstance.GetComponentInChildren<MeshRenderer>().gameObject))
			{
				GameObject[] deleteGOs3 = new GameObject[]
				{
					this.glassesInstance.GetComponentInChildren<MeshRenderer>().gameObject
				};
				this.skinnedMeshBaker.AddDeleteGameObjects(null, deleteGOs3, true);
				this.skinnedMeshBaker.Apply(null);
				Object.Destroy(this.glassesInstance);
				this.glassesInstance = null;
				Debug.Log("Done deleting glasses");
			}
		}
	}

	public Transform SearchHierarchyForBone(Transform current, string name)
	{
		if (current.name.Equals(name))
		{
			return current;
		}
		for (int i = 0; i < current.childCount; i++)
		{
			Transform transform = this.SearchHierarchyForBone(current.GetChild(i), name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public GameObject swordPrefab;

	public GameObject hatPrefab;

	public GameObject glassesPrefab;

	public GameObject workerPrefab;

	public GameObject targetCharacter;

	public MB3_MeshBaker skinnedMeshBaker;

	private GameObject swordInstance;

	private GameObject glassesInstance;

	private GameObject hatInstance;
}
