using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal static class MemoryUtilities
	{
		public unsafe static T* Malloc<[IsUnmanaged] T>(int count, Allocator allocator) where T : struct, ValueType
		{
			return (T*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<T>() * count), UnsafeUtility.AlignOf<T>(), allocator);
		}

		public unsafe static void Free<[IsUnmanaged] T>(T* p, Allocator allocator) where T : struct, ValueType
		{
			UnsafeUtility.Free((void*)p, allocator);
		}
	}
}
