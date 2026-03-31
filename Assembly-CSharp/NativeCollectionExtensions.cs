using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

public static class NativeCollectionExtensions
{
	public static T[] ToArray<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
	{
		return list.AsArray().ToArray();
	}

	public static List<T> ToList<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
	{
		List<T> list2 = new List<T>(list.Length);
		for (int i = 0; i < list.Length; i++)
		{
			list2.Add(list[i]);
		}
		return list2;
	}
}
