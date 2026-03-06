using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

internal struct VertexDictionary
{
	public NativeArray<int> GetIndexRemap(NativeArray<Vector3> vertices, NativeArray<int> indices)
	{
		NativeArray<int> nativeArray = new NativeArray<int>(vertices.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
		VertexDictionary.m_VertexDictionary.Clear();
		VertexDictionary.m_VertexDictionary.EnsureCapacity(vertices.Length);
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 key = vertices[i];
			if (!VertexDictionary.m_VertexDictionary.ContainsKey(key))
			{
				nativeArray[i] = i;
				VertexDictionary.m_VertexDictionary.Add(key, i);
			}
			else
			{
				nativeArray[i] = VertexDictionary.m_VertexDictionary[key];
			}
		}
		NativeArray<int> result = new NativeArray<int>(indices.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
		for (int j = 0; j < indices.Length; j++)
		{
			result[j] = nativeArray[indices[j]];
		}
		return result;
	}

	private static Dictionary<Vector3, int> m_VertexDictionary = new Dictionary<Vector3, int>();
}
