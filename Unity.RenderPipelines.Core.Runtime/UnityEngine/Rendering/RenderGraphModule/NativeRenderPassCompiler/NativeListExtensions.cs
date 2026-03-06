using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal static class NativeListExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static ReadOnlySpan<T> MakeReadOnlySpan<[IsUnmanaged] T>(this NativeList<T> list, int first, int numElements) where T : struct, ValueType
		{
			return new ReadOnlySpan<T>((void*)(list.GetUnsafeReadOnlyPtr<T>() + (IntPtr)first * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), numElements);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int LastIndex<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return list.Length - 1;
		}
	}
}
