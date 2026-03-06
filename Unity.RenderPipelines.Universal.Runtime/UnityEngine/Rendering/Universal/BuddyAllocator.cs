using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal struct BuddyAllocator : IDisposable
	{
		private ref BuddyAllocator.Header header
		{
			get
			{
				return UnsafeUtility.AsRef<BuddyAllocator.Header>(this.m_Data);
			}
		}

		private NativeArray<int> freeMaskCounts
		{
			get
			{
				return this.GetNativeArray<int>(this.m_ActiveFreeMaskCounts.Item1, this.m_ActiveFreeMaskCounts.Item2);
			}
		}

		private NativeArray<ulong> freeMasksStorage
		{
			get
			{
				return this.GetNativeArray<ulong>(this.m_FreeMasksStorage.Item1, this.m_FreeMasksStorage.Item2);
			}
		}

		private NativeArray<ulong> FreeMasks(int level)
		{
			return this.freeMasksStorage.GetSubArray(BuddyAllocator.LevelOffset64(level, this.header.branchingOrder), BuddyAllocator.LevelLength64(level, this.header.branchingOrder));
		}

		private NativeArray<int> freeMaskIndicesStorage
		{
			get
			{
				return this.GetNativeArray<int>(this.m_FreeMaskIndicesStorage.Item1, this.m_FreeMaskIndicesStorage.Item2);
			}
		}

		private NativeArray<int> FreeMaskIndices(int level)
		{
			return this.freeMaskIndicesStorage.GetSubArray(BuddyAllocator.LevelOffset64(level, this.header.branchingOrder), BuddyAllocator.LevelLength64(level, this.header.branchingOrder));
		}

		public int levelCount
		{
			get
			{
				return this.header.levelCount;
			}
		}

		public unsafe BuddyAllocator(int levelCount, int branchingOrder, Allocator allocator = Allocator.Persistent)
		{
			int num = sizeof(BuddyAllocator.Header);
			this.m_ActiveFreeMaskCounts = BuddyAllocator.AllocateRange<int>(levelCount, ref num);
			this.m_FreeMasksStorage = BuddyAllocator.AllocateRange<ulong>(BuddyAllocator.LevelOffset64(levelCount, branchingOrder), ref num);
			this.m_FreeMaskIndicesStorage = BuddyAllocator.AllocateRange<int>(BuddyAllocator.LevelOffset64(levelCount, branchingOrder), ref num);
			this.m_Data = UnsafeUtility.Malloc((long)num, 64, allocator);
			UnsafeUtility.MemClear(this.m_Data, (long)num);
			this.m_Allocator = allocator;
			*this.header = new BuddyAllocator.Header
			{
				branchingOrder = branchingOrder,
				levelCount = levelCount
			};
			this.FreeMasks(0)[0] = 15UL;
			this.freeMaskCounts[0] = 1;
		}

		public bool TryAllocate(int requestedLevel, out BuddyAllocation allocation)
		{
			allocation = default(BuddyAllocation);
			int i = requestedLevel;
			NativeArray<int> freeMaskCounts = this.freeMaskCounts;
			while (i >= 0 && freeMaskCounts[i] <= 0)
			{
				i--;
			}
			if (i < 0)
			{
				return false;
			}
			NativeArray<int> nativeArray = this.FreeMaskIndices(i);
			int num = i;
			int num2 = freeMaskCounts[num] - 1;
			freeMaskCounts[num] = num2;
			int num3 = nativeArray[num2];
			NativeArray<ulong> nativeArray2 = this.FreeMasks(i);
			ulong num4 = nativeArray2[num3];
			int num5 = math.tzcnt(num4);
			num4 ^= 1UL << num5;
			nativeArray2[num3] = num4;
			if (num4 != 0UL)
			{
				num2 = i;
				num = freeMaskCounts[num2];
				freeMaskCounts[num2] = num + 1;
				nativeArray[num] = num3;
			}
			int num6 = num3 * 64 + num5;
			while (i < requestedLevel)
			{
				i++;
				num6 <<= this.header.branchingOrder;
				int num7 = num6 >> 6;
				int num8 = num6 & 63;
				NativeArray<ulong> nativeArray3 = this.FreeMasks(i);
				ulong num9 = nativeArray3[num7];
				if (num9 == 0UL)
				{
					NativeArray<int> nativeArray4 = this.FreeMaskIndices(i);
					num = i;
					num2 = freeMaskCounts[num];
					freeMaskCounts[num] = num2 + 1;
					nativeArray4[num2] = num7;
				}
				num9 |= (1UL << BuddyAllocator.Pow2(this.header.branchingOrder)) - 2UL << num8;
				nativeArray3[num7] = num9;
			}
			allocation.level = i;
			allocation.index = num6;
			return true;
		}

		public void Free(BuddyAllocation allocation)
		{
			int i = allocation.level;
			int num = allocation.index;
			while (i >= 0)
			{
				int num2 = num >> 6;
				int num3 = num & 63;
				NativeArray<ulong> nativeArray = this.FreeMasks(i);
				ulong num4 = nativeArray[num2];
				bool flag = num4 == 0UL;
				num4 |= 1UL << num3;
				NativeArray<int> nativeArray2 = this.FreeMaskIndices(i);
				NativeArray<int> freeMaskCounts = this.freeMaskCounts;
				ulong num5 = (1UL << BuddyAllocator.Pow2(this.header.branchingOrder)) - 1UL << (num3 >> this.header.branchingOrder) * BuddyAllocator.Pow2(this.header.branchingOrder);
				if (i == 0 || (~(num4 != 0UL) & num5) != 0UL)
				{
					nativeArray[num2] = num4;
					if (flag)
					{
						int num6 = i;
						int num7 = freeMaskCounts[num6];
						freeMaskCounts[num6] = num7 + 1;
						nativeArray2[num7] = num2;
						return;
					}
					break;
				}
				else
				{
					num4 &= ~num5;
					nativeArray[num2] = num4;
					if (!flag && num4 == 0UL)
					{
						for (int j = 0; j < nativeArray2.Length; j++)
						{
							if (nativeArray2[j] == num2)
							{
								int index = j;
								int num7 = i;
								int num6 = freeMaskCounts[num7] - 1;
								freeMaskCounts[num7] = num6;
								nativeArray2[index] = nativeArray2[num6];
								break;
							}
						}
					}
					i--;
					num >>= this.header.branchingOrder;
				}
			}
		}

		public unsafe void Dispose()
		{
			UnsafeUtility.Free(this.m_Data, this.m_Allocator);
			this.m_Data = default(void*);
			this.m_Allocator = Allocator.Invalid;
		}

		private NativeArray<T> GetNativeArray<T>(int offset, int length) where T : struct
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(BuddyAllocator.PtrAdd(this.m_Data, offset), length, this.m_Allocator);
		}

		private static int LevelOffset(int level, int branchingOrder)
		{
			return BuddyAllocator.Pow2(branchingOrder) * (BuddyAllocator.Pow2(branchingOrder * (level - 1) + branchingOrder) - 1) / (BuddyAllocator.Pow2(branchingOrder) - 1);
		}

		private static int LevelLength(int level, int branchingOrder)
		{
			return BuddyAllocator.Pow2N(branchingOrder, level + 1);
		}

		private static int LevelOffset64(int level, int branchingOrder)
		{
			return math.min(level, 6 / branchingOrder) + BuddyAllocator.LevelOffset(math.max(0, level - 6 / branchingOrder), branchingOrder);
		}

		private static int LevelLength64(int level, int branchingOrder)
		{
			return BuddyAllocator.Pow2N(branchingOrder, math.max(0, level - 6 / branchingOrder + 1));
		}

		private static ValueTuple<int, int> AllocateRange<T>(int length, ref int dataSize) where T : struct
		{
			dataSize = BuddyAllocator.AlignForward(dataSize, UnsafeUtility.AlignOf<T>());
			ValueTuple<int, int> result = new ValueTuple<int, int>(dataSize, length);
			dataSize += length * UnsafeUtility.SizeOf<T>();
			return result;
		}

		private static int AlignForward(int offset, int alignment)
		{
			int num = offset % alignment;
			if (num != 0)
			{
				offset += alignment - num;
			}
			return offset;
		}

		private unsafe static void* PtrAdd(void* ptr, int bytes)
		{
			return (void*)((IntPtr)ptr + bytes);
		}

		private static int Pow2(int n)
		{
			return 1 << n;
		}

		private static int Pow2N(int x, int n)
		{
			return 1 << x * n;
		}

		private unsafe void* m_Data;

		private ValueTuple<int, int> m_ActiveFreeMaskCounts;

		private ValueTuple<int, int> m_FreeMasksStorage;

		private ValueTuple<int, int> m_FreeMaskIndicesStorage;

		private Allocator m_Allocator;

		private struct Header
		{
			public int branchingOrder;

			public int levelCount;

			public int allocationCount;

			public int freeAllocationIdsCount;
		}
	}
}
