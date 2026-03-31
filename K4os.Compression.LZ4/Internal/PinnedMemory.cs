using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace K4os.Compression.LZ4.Internal
{
	public struct PinnedMemory
	{
		public static int MaxPooledSize { get; set; } = 1048576;

		public unsafe readonly byte* Pointer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._pointer;
			}
		}

		public unsafe Span<byte> Span
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Span<byte>((void*)this.Pointer, this._size);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe readonly T* Reference<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return (T*)this._pointer;
		}

		public static PinnedMemory Alloc(int size, bool zero = true)
		{
			PinnedMemory result;
			PinnedMemory.Alloc(out result, size, zero);
			return result;
		}

		public static void Alloc(out PinnedMemory memory, int size, bool zero = true)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (size > PinnedMemory.MaxPooledSize)
			{
				PinnedMemory.AllocateNative(out memory, size, zero);
				return;
			}
			PinnedMemory.RentManagedFromPool(out memory, size, zero);
		}

		public static void Alloc<[IsUnmanaged] T>(out PinnedMemory memory, bool zero = true) where T : struct, ValueType
		{
			PinnedMemory.Alloc(out memory, sizeof(T), zero);
		}

		private unsafe static void AllocateNative(out PinnedMemory memory, int size, bool zero)
		{
			void* pointer = zero ? Mem.AllocZero(size) : Mem.Alloc(size);
			GC.AddMemoryPressure((long)size);
			memory._pointer = (byte*)pointer;
			memory._handle = default(GCHandle);
			memory._size = size;
		}

		private unsafe static void RentManagedFromPool(out PinnedMemory memory, int size, bool zero)
		{
			GCHandle handle = GCHandle.Alloc(BufferPool.Alloc(size, zero), GCHandleType.Pinned);
			byte* pointer = (byte*)((void*)handle.AddrOfPinnedObject());
			memory._pointer = pointer;
			memory._handle = handle;
			memory._size = size;
		}

		public void Clear()
		{
			if (this._size <= 0 || this._pointer == null)
			{
				return;
			}
			Mem.Zero(this._pointer, this._size);
		}

		public void Free()
		{
			if (this._handle.IsAllocated)
			{
				this.ReleaseManaged();
			}
			else if (this._pointer != null)
			{
				this.ReleaseNative();
			}
			this.ClearFields();
		}

		private void ReleaseManaged()
		{
			byte[] buffer = this._handle.IsAllocated ? ((byte[])this._handle.Target) : null;
			this._handle.Free();
			BufferPool.Free(buffer);
		}

		private unsafe void ReleaseNative()
		{
			GC.RemoveMemoryPressure((long)this._size);
			Mem.Free((void*)this._pointer);
		}

		private void ClearFields()
		{
			this._pointer = null;
			this._handle = default(GCHandle);
			this._size = 0;
		}

		private unsafe byte* _pointer;

		private GCHandle _handle;

		private int _size;
	}
}
