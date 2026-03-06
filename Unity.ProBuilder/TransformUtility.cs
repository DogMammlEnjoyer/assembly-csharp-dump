using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	public static class TransformUtility
	{
		internal static void UnparentChildren(Transform t)
		{
			Transform[] array = new Transform[t.childCount];
			for (int i = t.childCount - 1; i >= 0; i--)
			{
				Transform child = t.GetChild(i);
				array[i] = child;
				child.SetParent(null, true);
			}
			TransformUtility.s_ChildStack.Add(t, array);
		}

		internal static void ReparentChildren(Transform t)
		{
			Transform[] array;
			if (TransformUtility.s_ChildStack.TryGetValue(t, out array))
			{
				Transform[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].SetParent(t, true);
				}
				TransformUtility.s_ChildStack.Remove(t);
			}
		}

		public static Vertex TransformVertex(this Transform transform, Vertex vertex)
		{
			Vertex vertex2 = new Vertex();
			if (vertex.HasArrays(MeshArrays.Position))
			{
				vertex2.position = transform.TransformPoint(vertex.position);
			}
			if (vertex.HasArrays(MeshArrays.Color))
			{
				vertex2.color = vertex.color;
			}
			if (vertex.HasArrays(MeshArrays.Normal))
			{
				vertex2.normal = transform.TransformDirection(vertex.normal);
			}
			if (vertex.HasArrays(MeshArrays.Tangent))
			{
				vertex2.tangent = transform.rotation * vertex.tangent;
			}
			if (vertex.HasArrays(MeshArrays.Texture0))
			{
				vertex2.uv0 = vertex.uv0;
			}
			if (vertex.HasArrays(MeshArrays.Texture1))
			{
				vertex2.uv2 = vertex.uv2;
			}
			if (vertex.HasArrays(MeshArrays.Texture2))
			{
				vertex2.uv3 = vertex.uv3;
			}
			if (vertex.HasArrays(MeshArrays.Texture3))
			{
				vertex2.uv4 = vertex.uv4;
			}
			return vertex2;
		}

		public static Vertex InverseTransformVertex(this Transform transform, Vertex vertex)
		{
			Vertex vertex2 = new Vertex();
			if (vertex.HasArrays(MeshArrays.Position))
			{
				vertex2.position = transform.InverseTransformPoint(vertex.position);
			}
			if (vertex.HasArrays(MeshArrays.Color))
			{
				vertex2.color = vertex.color;
			}
			if (vertex.HasArrays(MeshArrays.Normal))
			{
				vertex2.normal = transform.InverseTransformDirection(vertex.normal);
			}
			if (vertex.HasArrays(MeshArrays.Tangent))
			{
				vertex2.tangent = transform.InverseTransformDirection(vertex.tangent);
			}
			if (vertex.HasArrays(MeshArrays.Texture0))
			{
				vertex2.uv0 = vertex.uv0;
			}
			if (vertex.HasArrays(MeshArrays.Texture1))
			{
				vertex2.uv2 = vertex.uv2;
			}
			if (vertex.HasArrays(MeshArrays.Texture2))
			{
				vertex2.uv3 = vertex.uv3;
			}
			if (vertex.HasArrays(MeshArrays.Texture3))
			{
				vertex2.uv4 = vertex.uv4;
			}
			return vertex2;
		}

		private static Dictionary<Transform, Transform[]> s_ChildStack = new Dictionary<Transform, Transform[]>();
	}
}
