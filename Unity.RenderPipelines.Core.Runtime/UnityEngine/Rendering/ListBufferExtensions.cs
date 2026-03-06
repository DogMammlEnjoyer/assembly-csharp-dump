using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	public static class ListBufferExtensions
	{
		public unsafe static void QuickSort<[IsUnmanaged] T>(this ListBuffer<T> self) where T : struct, ValueType, IComparable<T>
		{
			CoreUnsafeUtils.QuickSort<int>(self.Count, (void*)self.BufferPtr);
		}
	}
}
