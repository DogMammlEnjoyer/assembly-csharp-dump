using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Collections;

internal static class OVRNativeList
{
	public static OVRNativeList.CapacityHelper WithSuggestedCapacityFrom<T>([NoEnumeration] IEnumerable<T> collection)
	{
		return new OVRNativeList.CapacityHelper(collection.ToNonAlloc<T>().Count);
	}

	public static OVRNativeList.CapacityHelper WithSuggestedCapacityFrom<T>([NoEnumeration] IEnumerable<T> collection, out OVREnumerable<T> nonAllocatingEnumerable)
	{
		nonAllocatingEnumerable = collection.ToNonAlloc<T>();
		return new OVRNativeList.CapacityHelper(nonAllocatingEnumerable.Count);
	}

	public static OVRNativeList<T> ToNativeList<[IsUnmanaged] T>(this IEnumerable<T> collection, Allocator allocator) where T : struct, ValueType
	{
		OVRNativeList<T> result = new OVRNativeList<T>(allocator);
		result.AddRange(collection);
		return result;
	}

	public readonly struct CapacityHelper
	{
		public CapacityHelper(int? count)
		{
			this._count = count;
		}

		public OVRNativeList<T> AllocateEmpty<[IsUnmanaged] T>(Allocator allocator) where T : struct, ValueType
		{
			return new OVRNativeList<T>(this._count, allocator);
		}

		private readonly int? _count;
	}
}
