using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal struct ParallelBitArray
	{
		public int Length
		{
			get
			{
				return this.m_Length;
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Bits.IsCreated;
			}
		}

		public ParallelBitArray(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			this.m_Allocator = allocator;
			this.m_Bits = new NativeArray<long>((length + 63) / 64, allocator, options);
			this.m_Length = length;
		}

		public void Dispose()
		{
			this.m_Bits.Dispose();
			this.m_Length = 0;
		}

		public void Dispose(JobHandle inputDeps)
		{
			this.m_Bits.Dispose(inputDeps);
			this.m_Length = 0;
		}

		public void Resize(int newLength)
		{
			int length = this.m_Length;
			if (newLength == length)
			{
				return;
			}
			int length2 = this.m_Bits.Length;
			int num = (newLength + 63) / 64;
			if (num != length2)
			{
				NativeArray<long> nativeArray = new NativeArray<long>(num, this.m_Allocator, NativeArrayOptions.UninitializedMemory);
				if (this.m_Bits.IsCreated)
				{
					NativeArray<long>.Copy(this.m_Bits, nativeArray, this.m_Bits.Length);
					this.m_Bits.Dispose();
				}
				this.m_Bits = nativeArray;
			}
			int num2 = Math.Min(length, newLength);
			for (int i = Math.Min(length2, num); i < this.m_Bits.Length; i++)
			{
				int num3 = Math.Max(num2 - 64 * i, 0);
				if (num3 < 64)
				{
					ulong num4 = (1UL << num3) - 1UL;
					ref NativeArray<long> ptr = ref this.m_Bits;
					int index = i;
					ptr[index] &= (long)num4;
				}
			}
			this.m_Length = newLength;
		}

		public unsafe void Set(int index, bool value)
		{
			int num = index >> 6;
			long* unsafePtr = (long*)this.m_Bits.GetUnsafePtr<long>();
			ulong num2 = 1UL << index;
			long num3 = (long)(~(long)num2);
			long num4 = (long)(value ? num2 : 0UL);
			long num5;
			long value2;
			do
			{
				num5 = Interlocked.Read(ref unsafePtr[num]);
				value2 = ((num5 & num3) | num4);
			}
			while (Interlocked.CompareExchange(ref unsafePtr[num], value2, num5) != num5);
		}

		public unsafe bool Get(int index)
		{
			int num = index >> 6;
			long* unsafeReadOnlyPtr = (long*)this.m_Bits.GetUnsafeReadOnlyPtr<long>();
			long num2 = 1L << index;
			return (unsafeReadOnlyPtr[num] & num2) != 0L;
		}

		public ulong GetChunk(int chunk_index)
		{
			return (ulong)this.m_Bits[chunk_index];
		}

		public void SetChunk(int chunk_index, ulong chunk_bits)
		{
			this.m_Bits[chunk_index] = (long)chunk_bits;
		}

		public unsafe ulong InterlockedReadChunk(int chunk_index)
		{
			long* unsafeReadOnlyPtr = (long*)this.m_Bits.GetUnsafeReadOnlyPtr<long>();
			return (ulong)Interlocked.Read(ref unsafeReadOnlyPtr[chunk_index]);
		}

		public unsafe void InterlockedOrChunk(int chunk_index, ulong chunk_bits)
		{
			long* unsafePtr = (long*)this.m_Bits.GetUnsafePtr<long>();
			long num;
			long value;
			do
			{
				num = Interlocked.Read(ref unsafePtr[chunk_index]);
				value = (num | (long)chunk_bits);
			}
			while (Interlocked.CompareExchange(ref unsafePtr[chunk_index], value, num) != num);
		}

		public int ChunkCount()
		{
			return this.m_Bits.Length;
		}

		public ParallelBitArray GetSubArray(int length)
		{
			return new ParallelBitArray
			{
				m_Bits = this.m_Bits.GetSubArray(0, (length + 63) / 64),
				m_Length = length
			};
		}

		public NativeArray<long> GetBitsArray()
		{
			return this.m_Bits;
		}

		public void FillZeroes(int length)
		{
			length = Math.Min(length, this.m_Length);
			int num = length / 64;
			int num2 = length & 63;
			long num3 = 0L;
			ref this.m_Bits.FillArray(num3, 0, num);
			if (num2 > 0)
			{
				long num4 = (1L << num2) - 1L;
				ref NativeArray<long> ptr = ref this.m_Bits;
				int index = num;
				ptr[index] &= ~num4;
			}
		}

		private Allocator m_Allocator;

		private NativeArray<long> m_Bits;

		private int m_Length;
	}
}
