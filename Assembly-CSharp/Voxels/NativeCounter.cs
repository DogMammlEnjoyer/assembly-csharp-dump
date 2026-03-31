using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Voxels
{
	public struct NativeCounter : IDisposable
	{
		public unsafe int Count
		{
			get
			{
				return *this._counter;
			}
			set
			{
				*this._counter = value;
			}
		}

		public unsafe NativeCounter(Allocator allocator)
		{
			this._allocator = allocator;
			this._counter = (int*)UnsafeUtility.Malloc(4L, 4, this._allocator);
			this.Count = 0;
		}

		public unsafe int Increment()
		{
			return Interlocked.Increment(ref *this._counter) - 1;
		}

		public unsafe void Dispose()
		{
			UnsafeUtility.Free((void*)this._counter, this._allocator);
		}

		private readonly Allocator _allocator;

		[NativeDisableUnsafePtrRestriction]
		private unsafe readonly int* _counter;
	}
}
