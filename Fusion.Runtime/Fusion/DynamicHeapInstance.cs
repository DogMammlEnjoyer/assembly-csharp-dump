using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public class DynamicHeapInstance
	{
		public DynamicHeapInstance(params Type[] types)
		{
			this._heap = DynamicHeap.Create(DynamicHeap.Config.Default, types);
		}

		protected override void Finalize()
		{
			try
			{
				bool flag = this._heap != null;
				if (flag)
				{
					DynamicHeap.Destroy(this._heap);
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		public unsafe void Free(void* ptr)
		{
			DynamicHeap.Free(this._heap, ptr);
		}

		public unsafe void* Allocate(int size)
		{
			return DynamicHeap.Allocate(this._heap, size);
		}

		public unsafe void* AllocateArray<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			this.VerifyArrayLength(length);
			return DynamicHeap.Allocate(this._heap, sizeof(T) * length);
		}

		public unsafe void* AllocateArrayPointers<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			this.VerifyArrayLength(length);
			return DynamicHeap.Allocate(this._heap, sizeof(T*) * length);
		}

		public unsafe void* AllocateTracked<[IsUnmanaged] T>(bool root = false) where T : struct, ValueType
		{
			return (void*)DynamicHeap.AllocateTracked<T>(this._heap, 1, root);
		}

		public unsafe void* AllocateTrackedArray<[IsUnmanaged] T>(int length, bool root = false) where T : struct, ValueType
		{
			this.VerifyArrayLength(length);
			return (void*)DynamicHeap.AllocateTracked<T>(this._heap, (ushort)length, root);
		}

		public unsafe void* AllocateTrackedArrayPointers<[IsUnmanaged] T>(int length, bool root = false) where T : struct, ValueType
		{
			this.VerifyArrayLength(length);
			return (void*)DynamicHeap.AllocateTrackedPointerArray<T>(this._heap, (ushort)length, root);
		}

		private void VerifyArrayLength(int length)
		{
			Assert.Always(length > 0, "length > 0");
			Assert.Always(length <= 65535, "length <= ushort.MaxValue");
		}

		private unsafe DynamicHeap* _heap;
	}
}
