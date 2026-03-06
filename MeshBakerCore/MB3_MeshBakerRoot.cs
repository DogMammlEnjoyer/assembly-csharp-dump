using System;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;

public abstract class MB3_MeshBakerRoot : MonoBehaviour
{
	[HideInInspector]
	public abstract MB2_TextureBakeResults textureBakeResults { get; set; }

	public virtual List<GameObject> GetObjectsToCombine()
	{
		return null;
	}

	public virtual void PurgeNullsFromObjectsToCombine()
	{
	}

	public static bool DoCombinedValidate(MB3_MeshBakerRoot mom, MB_ObjsToCombineTypes objToCombineType, MB2_EditorMethodsInterface editorMethods, MB2_ValidationLevel validationLevel)
	{
		if (mom.textureBakeResults == null)
		{
			Debug.LogError("Need to set Texture Bake Result on " + ((mom != null) ? mom.ToString() : null));
			return false;
		}
		if (mom is MB3_MeshBakerCommon)
		{
			MB3_TextureBaker textureBaker = ((MB3_MeshBakerCommon)mom).GetTextureBaker();
			if (textureBaker != null && textureBaker.textureBakeResults != mom.textureBakeResults)
			{
				Debug.LogWarning("Texture Bake Result on this component is not the same as the Texture Bake Result on the MB3_TextureBaker.");
			}
		}
		List<GameObject> objectsToCombine = mom.GetObjectsToCombine();
		if (!MB3_MeshBakerRoot.ValidateTextureBakerGameObjects(mom, objectsToCombine, validationLevel))
		{
			return false;
		}
		if (mom is MB3_MeshBaker)
		{
			List<GameObject> objectsToCombine2 = mom.GetObjectsToCombine();
			if (objectsToCombine2 == null || objectsToCombine2.Count == 0)
			{
				Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
				return false;
			}
			if (mom is MB3_MeshBaker && ((MB3_MeshBaker)mom).meshCombiner.settings.renderType == MB_RenderType.skinnedMeshRenderer && !editorMethods.ValidateSkinnedMeshes(objectsToCombine2))
			{
				return false;
			}
		}
		if (editorMethods != null)
		{
			editorMethods.CheckPrefabTypes(objToCombineType, objectsToCombine);
		}
		return true;
	}

	public static bool ValidateTextureBakerGameObjects(MB3_MeshBakerRoot mom, List<GameObject> objsToMesh, MB2_ValidationLevel validationLevel)
	{
		Dictionary<int, MB_Utility.MeshAnalysisResult> dictionary = null;
		if (validationLevel == MB2_ValidationLevel.robust)
		{
			dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult>();
		}
		Dictionary<string, Material> dictionary2 = new Dictionary<string, Material>();
		for (int i = 0; i < objsToMesh.Count; i++)
		{
			GameObject gameObject = objsToMesh[i];
			if (gameObject == null)
			{
				Debug.LogError(string.Format("The list of objects to combine contains a null at position {0}. Select and use [shift + delete] to remove the object, or purge all null objects from the context menu.", i));
				return false;
			}
			for (int j = i + 1; j < objsToMesh.Count; j++)
			{
				if (objsToMesh[i] == objsToMesh[j])
				{
					Debug.LogError("The list of objects to combine contains duplicates at " + i.ToString() + " and " + j.ToString());
					return false;
				}
			}
			Material[] gomaterials = MB_Utility.GetGOMaterials(gameObject);
			if (gomaterials.Length == 0)
			{
				string str = "Object ";
				GameObject gameObject2 = gameObject;
				Debug.LogError(str + ((gameObject2 != null) ? gameObject2.ToString() : null) + " in the list of objects to be combined does not have a material");
				return false;
			}
			Mesh mesh = MB_Utility.GetMesh(gameObject);
			if (mesh == null)
			{
				string str2 = "Object ";
				GameObject gameObject3 = gameObject;
				Debug.LogError(str2 + ((gameObject3 != null) ? gameObject3.ToString() : null) + " in the list of objects to be combined does not have a mesh");
				return false;
			}
			if (mesh != null && mom.textureBakeResults != null && Application.isEditor && !Application.isPlaying && mom.textureBakeResults.doMultiMaterial && validationLevel >= MB2_ValidationLevel.robust)
			{
				MB_Utility.MeshAnalysisResult meshAnalysisResult;
				if (!dictionary.TryGetValue(mesh.GetInstanceID(), out meshAnalysisResult))
				{
					MB_Utility.doSubmeshesShareVertsOrTris(mesh, ref meshAnalysisResult);
					dictionary.Add(mesh.GetInstanceID(), meshAnalysisResult);
				}
				if (meshAnalysisResult.hasOverlappingSubmeshVerts)
				{
					string str3 = "Object ";
					GameObject gameObject4 = objsToMesh[i];
					Debug.LogWarning(str3 + ((gameObject4 != null) ? gameObject4.ToString() : null) + " in the list of objects to combine has overlapping submeshes (submeshes share vertices). If the UVs associated with the shared vertices are important then this bake may not work. If you are using multiple materials then this object can only be combined with objects that use the exact same set of textures (each atlas contains one texture). There may be other undesirable side affects as well. Mesh Master, available in the asset store can fix overlapping submeshes.");
				}
			}
			if (MBVersion.IsUsingAddressables())
			{
				HashSet<string> hashSet = new HashSet<string>();
				for (int k = 0; k < gomaterials.Length; k++)
				{
					if (gomaterials[k] != null)
					{
						if (dictionary2.ContainsKey(gomaterials[k].name))
						{
							if (gomaterials[k] != dictionary2[gomaterials[k].name])
							{
								hashSet.Add(gomaterials[k].name);
							}
						}
						else
						{
							dictionary2.Add(gomaterials[k].name, gomaterials[k]);
						}
					}
				}
				if (hashSet.Count > 0)
				{
					string[] array = new string[hashSet.Count];
					hashSet.CopyTo(array);
					string str4 = string.Join(",", array);
					Debug.LogError("The source objects use different materials that have the same name (" + str4 + "). If using addressables, materials with the same name are considered to be the same material when baking meshes at runtime. If you want to use this Material Bake Result at runtime then all source materials must have distinct names. Baking in edit-mode will still work.");
				}
			}
		}
		return true;
	}

	public Vector3 sortAxis;

	public class ZSortObjects
	{
		public void SortByDistanceAlongAxis(List<GameObject> gos)
		{
			if (this.sortAxis == Vector3.zero)
			{
				Debug.LogError("The sort axis cannot be the zero vector.");
				return;
			}
			Debug.Log("Z sorting meshes along axis numObjs=" + gos.Count.ToString());
			List<MB3_MeshBakerRoot.ZSortObjects.Item> list = new List<MB3_MeshBakerRoot.ZSortObjects.Item>();
			Quaternion rotation = Quaternion.FromToRotation(this.sortAxis, Vector3.forward);
			for (int i = 0; i < gos.Count; i++)
			{
				if (gos[i] != null)
				{
					MB3_MeshBakerRoot.ZSortObjects.Item item = new MB3_MeshBakerRoot.ZSortObjects.Item();
					item.point = gos[i].transform.position;
					item.go = gos[i];
					item.point = rotation * item.point;
					list.Add(item);
				}
			}
			list.Sort(new MB3_MeshBakerRoot.ZSortObjects.ItemComparer());
			for (int j = 0; j < gos.Count; j++)
			{
				gos[j] = list[j].go;
			}
		}

		public Vector3 sortAxis;

		public class Item
		{
			public GameObject go;

			public Vector3 point;
		}

		public class ItemComparer : IComparer<MB3_MeshBakerRoot.ZSortObjects.Item>
		{
			public int Compare(MB3_MeshBakerRoot.ZSortObjects.Item a, MB3_MeshBakerRoot.ZSortObjects.Item b)
			{
				return (int)Mathf.Sign(b.point.z - a.point.z);
			}
		}
	}
}
