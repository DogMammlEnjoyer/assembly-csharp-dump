using System;
using Unity.Collections;

namespace Unity.Profiling.LowLevel.Unsafe
{
	internal readonly struct UnsafeAllocLabel
	{
		public UnsafeAllocLabel(string areaName, string objectName, Allocator allocator = Allocator.Persistent)
		{
			bool flag = string.IsNullOrEmpty(areaName);
			if (flag)
			{
				throw new ArgumentNullException("areaName");
			}
			bool flag2 = string.IsNullOrEmpty(objectName);
			if (flag2)
			{
				throw new ArgumentNullException("objectName");
			}
			bool flag3 = allocator != Allocator.Persistent && allocator != Allocator.Domain;
			if (flag3)
			{
				throw new ArgumentException("Only Allocator.Persistent and Allocator.Domain support allocating with a label");
			}
			this.allocator = allocator;
			this.pointer = ProfilerUnsafeUtility.GetOrCreateMemLabel(areaName, objectName);
		}

		internal long RelatedMemorySize
		{
			get
			{
				return ProfilerUnsafeUtility.GetMemLabelRelatedMemorySize(this.pointer);
			}
		}

		public bool Created
		{
			get
			{
				return this.pointer != IntPtr.Zero;
			}
		}

		internal readonly IntPtr pointer;

		internal readonly Allocator allocator;
	}
}
