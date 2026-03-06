using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Unity.Hierarchy
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal struct HierarchyNodeChildrenAlloc
	{
		[FieldOffset(0)]
		public unsafe HierarchyNode* Ptr;

		[FieldOffset(8)]
		public int Size;

		[FieldOffset(12)]
		public int Capacity;

		[FieldOffset(16)]
		public int RemovedCount;

		[FixedBuffer(typeof(int), 3)]
		[FieldOffset(20)]
		public HierarchyNodeChildrenAlloc.<Reserved>e__FixedBuffer Reserved;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 12)]
		public struct <Reserved>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
