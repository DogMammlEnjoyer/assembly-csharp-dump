using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.Universal
{
	internal static class NativeArrayExtensions
	{
		public static ref T UnsafeElementAt<T>(this NativeArray<T> array, int index) where T : struct
		{
			return UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafeReadOnlyPtr<T>(), index);
		}

		public static ref T UnsafeElementAtMutable<T>(this NativeArray<T> array, int index) where T : struct
		{
			return UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafePtr<T>(), index);
		}
	}
}
