using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR
{
	internal class NativeList<T> : IDisposable where T : struct
	{
		public NativeList(int initialCapacity)
		{
			Debug.Assert(initialCapacity > 0);
			this.m_NativeArray = new NativeArray<T>(initialCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}

		public NativeList(int initialCapacity, Allocator allocator)
		{
			Debug.Assert(initialCapacity > 0);
			this.m_NativeArray = new NativeArray<T>(initialCapacity, allocator, NativeArrayOptions.UninitializedMemory);
		}

		private void Expand(int newLength)
		{
			NativeArray<T> nativeArray = new NativeArray<T>(newLength, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			nativeArray.Slice(0, this.m_Count).CopyFrom(this.m_NativeArray);
			this.m_NativeArray.Dispose();
			this.m_NativeArray = nativeArray;
		}

		public void Add(ref T data)
		{
			bool flag = this.m_Count == this.m_NativeArray.Length;
			if (flag)
			{
				this.Expand(this.m_NativeArray.Length << 1);
			}
			int count = this.m_Count;
			this.m_Count = count + 1;
			this.m_NativeArray[count] = data;
		}

		public void Add(NativeSlice<T> src)
		{
			int num = this.m_Count + src.Length;
			bool flag = this.m_NativeArray.Length < num;
			if (flag)
			{
				this.Expand(num << 1);
			}
			this.m_NativeArray.Slice(this.m_Count, src.Length).CopyFrom(src);
			this.m_Count += src.Length;
		}

		public void Clear()
		{
			this.m_Count = 0;
		}

		public NativeSlice<T> GetSlice(int start, int length)
		{
			return this.m_NativeArray.Slice(start, length);
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_NativeArray.Dispose();
				}
				this.disposed = true;
			}
		}

		private NativeArray<T> m_NativeArray;

		private int m_Count;
	}
}
