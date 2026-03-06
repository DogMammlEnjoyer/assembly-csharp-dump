using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	public struct ListBuffer<[IsUnmanaged] T> where T : struct, ValueType
	{
		internal unsafe T* BufferPtr
		{
			get
			{
				return this.m_BufferPtr;
			}
		}

		public unsafe int Count
		{
			get
			{
				return *this.m_CountPtr;
			}
		}

		public int Capacity
		{
			get
			{
				return this.m_Capacity;
			}
		}

		public unsafe ListBuffer(T* bufferPtr, int* countPtr, int capacity)
		{
			this.m_BufferPtr = bufferPtr;
			this.m_Capacity = capacity;
			this.m_CountPtr = countPtr;
		}

		public unsafe T this[in int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new IndexOutOfRangeException(string.Format("Expected a value between 0 and {0}, but received {1}.", this.Count, index));
				}
				return ref this.m_BufferPtr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
			}
		}

		public unsafe ref T GetUnchecked(in int index)
		{
			return ref this.m_BufferPtr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
		}

		public unsafe bool TryAdd(in T value)
		{
			if (this.Count >= this.m_Capacity)
			{
				return false;
			}
			this.m_BufferPtr[(IntPtr)this.Count * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			(*this.m_CountPtr)++;
			return true;
		}

		public unsafe void CopyTo(T* dstBuffer, int startDstIndex, int copyCount)
		{
			UnsafeUtility.MemCpy((void*)(dstBuffer + (IntPtr)startDstIndex * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), (void*)this.m_BufferPtr, (long)(UnsafeUtility.SizeOf<T>() * copyCount));
		}

		public unsafe bool TryCopyTo(ListBuffer<T> other)
		{
			if (other.Count + this.Count >= other.m_Capacity)
			{
				return false;
			}
			UnsafeUtility.MemCpy((void*)(other.m_BufferPtr + (IntPtr)other.Count * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), (void*)this.m_BufferPtr, (long)(UnsafeUtility.SizeOf<T>() * this.Count));
			*other.m_CountPtr += this.Count;
			return true;
		}

		public unsafe bool TryCopyFrom(T* srcPtr, int count)
		{
			if (count + this.Count > this.m_Capacity)
			{
				return false;
			}
			UnsafeUtility.MemCpy((void*)(this.m_BufferPtr + (IntPtr)this.Count * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), (void*)srcPtr, (long)(UnsafeUtility.SizeOf<T>() * count));
			*this.m_CountPtr += count;
			return true;
		}

		private unsafe T* m_BufferPtr;

		private int m_Capacity;

		private unsafe int* m_CountPtr;
	}
}
