using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	internal struct EdgeDictionary : IEdgeStore
	{
		public NativeArray<ShadowEdge> GetOutsideEdges(NativeArray<Vector3> vertices, NativeArray<int> indices)
		{
			EdgeDictionary.m_EdgeDictionary.Clear();
			EdgeDictionary.m_EdgeDictionary.EnsureCapacity(indices.Length);
			for (int i = 0; i < indices.Length; i += 3)
			{
				int num = indices[i];
				int num2 = indices[i + 1];
				int num3 = indices[i + 2];
				ShadowEdge key = new ShadowEdge(num, num2);
				ShadowEdge key2 = new ShadowEdge(num2, num3);
				ShadowEdge key3 = new ShadowEdge(num3, num);
				if (EdgeDictionary.m_EdgeDictionary.ContainsKey(key))
				{
					EdgeDictionary.m_EdgeDictionary[key] = EdgeDictionary.m_EdgeDictionary[key] + 1;
				}
				else
				{
					EdgeDictionary.m_EdgeDictionary.Add(key, 1);
				}
				if (EdgeDictionary.m_EdgeDictionary.ContainsKey(key2))
				{
					EdgeDictionary.m_EdgeDictionary[key2] = EdgeDictionary.m_EdgeDictionary[key2] + 1;
				}
				else
				{
					EdgeDictionary.m_EdgeDictionary.Add(key2, 1);
				}
				if (EdgeDictionary.m_EdgeDictionary.ContainsKey(key3))
				{
					EdgeDictionary.m_EdgeDictionary[key3] = EdgeDictionary.m_EdgeDictionary[key3] + 1;
				}
				else
				{
					EdgeDictionary.m_EdgeDictionary.Add(key3, 1);
				}
			}
			int num4 = 0;
			foreach (KeyValuePair<ShadowEdge, int> keyValuePair in EdgeDictionary.m_EdgeDictionary)
			{
				if (keyValuePair.Value == 1)
				{
					num4++;
				}
			}
			int num5 = 0;
			NativeArray<ShadowEdge> result = new NativeArray<ShadowEdge>(num4, Allocator.Temp, NativeArrayOptions.ClearMemory);
			foreach (KeyValuePair<ShadowEdge, int> keyValuePair2 in EdgeDictionary.m_EdgeDictionary)
			{
				if (keyValuePair2.Value == 1)
				{
					result[num5++] = keyValuePair2.Key;
				}
			}
			return result;
		}

		private static Dictionary<ShadowEdge, int> m_EdgeDictionary = new Dictionary<ShadowEdge, int>(new EdgeDictionary.EdgeComparer());

		private class EdgeComparer : IEqualityComparer<ShadowEdge>
		{
			public bool Equals(ShadowEdge edge0, ShadowEdge edge1)
			{
				return (edge0.v0 == edge1.v0 && edge0.v1 == edge1.v1) || (edge0.v1 == edge1.v0 && edge0.v0 == edge1.v1);
			}

			public int GetHashCode(ShadowEdge edge)
			{
				int num = edge.v0;
				int num2 = edge.v1;
				if (edge.v1 < edge.v0)
				{
					num = edge.v1;
					num2 = edge.v0;
				}
				return (num << 15 | num2).GetHashCode();
			}
		}
	}
}
