using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder.Stl
{
	internal static class pb_Stl_Exporter
	{
		public static bool Export(string path, GameObject[] gameObjects, FileType type)
		{
			Mesh[] array = pb_Stl_Exporter.CreateWorldSpaceMeshesWithTransforms((from x in gameObjects
			select x.transform).ToArray<Transform>());
			bool result = false;
			if (array != null && array.Length != 0 && !string.IsNullOrEmpty(path))
			{
				result = pb_Stl.WriteFile(path, array, type, true);
			}
			int num = 0;
			while (array != null && num < array.Length)
			{
				Object.DestroyImmediate(array[num]);
				num++;
			}
			return result;
		}

		private static Mesh[] CreateWorldSpaceMeshesWithTransforms(IList<Transform> transforms)
		{
			if (transforms == null || transforms.Count < 1)
			{
				return null;
			}
			Vector3 a = Vector3.zero;
			for (int i = 0; i < transforms.Count; i++)
			{
				a += transforms[i].position;
			}
			Vector3 position = a / (float)transforms.Count;
			GameObject gameObject = new GameObject();
			gameObject.name = "ROOT";
			gameObject.transform.position = position;
			foreach (Transform transform in transforms)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(transform.gameObject);
				gameObject2.transform.SetParent(transform.parent, false);
				gameObject2.transform.SetParent(gameObject.transform, true);
			}
			gameObject.transform.position = Vector3.zero;
			List<MeshFilter> list = (from x in gameObject.GetComponentsInChildren<MeshFilter>()
			where x.sharedMesh != null
			select x).ToList<MeshFilter>();
			int count = list.Count;
			Mesh[] array = new Mesh[count];
			for (int j = 0; j < count; j++)
			{
				Transform transform2 = list[j].transform;
				Vector3[] vertices = list[j].sharedMesh.vertices;
				Vector3[] normals = list[j].sharedMesh.normals;
				for (int k = 0; k < vertices.Length; k++)
				{
					vertices[k] = transform2.TransformPoint(vertices[k]);
					normals[k] = transform2.TransformDirection(normals[k]);
				}
				array[j] = new Mesh
				{
					name = list[j].name,
					vertices = vertices,
					normals = normals,
					triangles = list[j].sharedMesh.triangles
				};
			}
			Object.DestroyImmediate(gameObject);
			return array;
		}
	}
}
