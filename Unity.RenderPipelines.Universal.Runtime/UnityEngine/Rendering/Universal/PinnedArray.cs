using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.Universal
{
	internal struct PinnedArray<T> : IDisposable where T : struct
	{
		public int length
		{
			get
			{
				if (this.managedArray == null)
				{
					return 0;
				}
				return this.managedArray.Length;
			}
		}

		public unsafe PinnedArray(int length)
		{
			this.managedArray = new T[length];
			this.handle = GCHandle.Alloc(this.managedArray, GCHandleType.Pinned);
			this.nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.handle.AddrOfPinnedObject(), length, Allocator.None);
		}

		public void Dispose()
		{
			if (this.managedArray == null)
			{
				return;
			}
			this.handle.Free();
			this = default(PinnedArray<T>);
		}

		public T[] managedArray;

		public GCHandle handle;

		public NativeArray<T> nativeArray;
	}
}
