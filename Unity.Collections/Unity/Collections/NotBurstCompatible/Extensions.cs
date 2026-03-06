using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.NotBurstCompatible
{
	public static class Extensions
	{
		[ExcludeFromBurstCompatTesting("Returns managed array")]
		public static T[] ToArray<[IsUnmanaged] T>(this NativeHashSet<T> set) where T : struct, ValueType, IEquatable<T>
		{
			NativeArray<T> nativeArray = set.ToNativeArray(Allocator.TempJob);
			T[] result = nativeArray.ToArray();
			nativeArray.Dispose();
			return result;
		}

		[ExcludeFromBurstCompatTesting("Returns managed array")]
		public static T[] ToArray<[IsUnmanaged] T>(this NativeParallelHashSet<T> set) where T : struct, ValueType, IEquatable<T>
		{
			NativeArray<T> nativeArray = set.ToNativeArray(Allocator.TempJob);
			T[] result = nativeArray.ToArray();
			nativeArray.Dispose();
			return result;
		}

		[ExcludeFromBurstCompatTesting("Returns managed array")]
		public static T[] ToArrayNBC<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return list.AsArray().ToArray();
		}

		[ExcludeFromBurstCompatTesting("Takes managed array")]
		public static void CopyFromNBC<[IsUnmanaged] T>(this NativeList<T> list, T[] array) where T : struct, ValueType
		{
			list.Clear();
			list.Resize(array.Length, NativeArrayOptions.UninitializedMemory);
			list.AsArray().CopyFrom(array);
		}
	}
}
