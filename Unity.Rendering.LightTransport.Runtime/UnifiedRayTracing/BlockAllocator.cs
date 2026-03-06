using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal struct BlockAllocator : IDisposable
	{
		public int freeElementsCount
		{
			get
			{
				return this.m_FreeElementCount;
			}
		}

		public int freeBlocks
		{
			get
			{
				return this.m_freeBlocks.Length;
			}
		}

		public int capacity
		{
			get
			{
				return this.m_MaxElementCount;
			}
		}

		public int allocatedSize
		{
			get
			{
				return this.m_MaxElementCount - this.m_FreeElementCount;
			}
		}

		public void Initialize(int maxElementCounts)
		{
			this.m_MaxElementCount = maxElementCounts;
			this.m_FreeElementCount = maxElementCounts;
			if (!this.m_freeBlocks.IsCreated)
			{
				this.m_freeBlocks = new NativeList<BlockAllocator.Block>(Allocator.Persistent);
			}
			else
			{
				this.m_freeBlocks.Clear();
			}
			BlockAllocator.Block block = default(BlockAllocator.Block);
			block.offset = 0;
			block.count = this.m_FreeElementCount;
			this.m_freeBlocks.Add(block);
			if (!this.m_usedBlocks.IsCreated)
			{
				this.m_usedBlocks = new NativeList<BlockAllocator.Block>(Allocator.Persistent);
			}
			else
			{
				this.m_usedBlocks.Clear();
			}
			if (!this.m_freeSlots.IsCreated)
			{
				this.m_freeSlots = new NativeList<int>(Allocator.Persistent);
				return;
			}
			this.m_freeSlots.Clear();
		}

		private int CalculateGeometricGrowthCapacity(int desiredNewCapacity, int maxAllowedNewCapacity)
		{
			int capacity = this.capacity;
			if (capacity > maxAllowedNewCapacity - capacity / 2)
			{
				return maxAllowedNewCapacity;
			}
			int num = capacity + capacity / 2;
			if (num < desiredNewCapacity)
			{
				return desiredNewCapacity;
			}
			return num;
		}

		public int Grow(int newDesiredCapacity, int maxAllowedCapacity = 2147483647)
		{
			int num = this.CalculateGeometricGrowthCapacity(newDesiredCapacity, maxAllowedCapacity);
			int maxElementCount = this.m_MaxElementCount;
			int num2 = num - maxElementCount;
			this.m_FreeElementCount += num2;
			this.m_MaxElementCount = num;
			int num3 = this.m_freeBlocks.Length;
			BlockAllocator.Block block = default(BlockAllocator.Block);
			block.offset = maxElementCount;
			block.count = num2;
			this.m_freeBlocks.Add(block);
			while (num3 != -1)
			{
				num3 = this.MergeBlockFrontBack(num3);
			}
			return this.m_MaxElementCount;
		}

		public BlockAllocator.Allocation GrowAndAllocate(int elementCounts, out int oldCapacity, out int newCapacity)
		{
			return this.GrowAndAllocate(elementCounts, int.MaxValue, out oldCapacity, out newCapacity);
		}

		public BlockAllocator.Allocation GrowAndAllocate(int elementCounts, int maxAllowedCapacity, out int oldCapacity, out int newCapacity)
		{
			oldCapacity = this.capacity;
			int num = this.m_freeBlocks.IsEmpty ? elementCounts : math.max(elementCounts - this.m_freeBlocks[this.m_freeBlocks.Length - 1].count, 0);
			if (maxAllowedCapacity < this.capacity || maxAllowedCapacity - this.capacity < num)
			{
				newCapacity = this.capacity;
				return BlockAllocator.Allocation.Invalid;
			}
			newCapacity = ((num > 0) ? this.Grow(this.capacity + num, maxAllowedCapacity) : this.capacity);
			return this.Allocate(elementCounts);
		}

		public void Dispose()
		{
			this.m_MaxElementCount = 0;
			this.m_FreeElementCount = 0;
			if (this.m_freeBlocks.IsCreated)
			{
				this.m_freeBlocks.Dispose();
			}
			if (this.m_usedBlocks.IsCreated)
			{
				this.m_usedBlocks.Dispose();
			}
			if (this.m_freeSlots.IsCreated)
			{
				this.m_freeSlots.Dispose();
			}
		}

		public BlockAllocator.Allocation Allocate(int elementCounts)
		{
			if (elementCounts > this.m_FreeElementCount || this.m_freeBlocks.IsEmpty)
			{
				return BlockAllocator.Allocation.Invalid;
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < this.m_freeBlocks.Length; i++)
			{
				BlockAllocator.Block block = this.m_freeBlocks[i];
				if (elementCounts <= block.count && (num == -1 || block.count < num2))
				{
					num2 = block.count;
					num = i;
				}
			}
			if (num == -1)
			{
				return BlockAllocator.Allocation.Invalid;
			}
			BlockAllocator.Block block2 = this.m_freeBlocks[num];
			BlockAllocator.Block block3 = block2;
			block3.offset += elementCounts;
			block3.count -= elementCounts;
			block2.count = elementCounts;
			if (block3.count > 0)
			{
				this.m_freeBlocks[num] = block3;
			}
			else
			{
				this.m_freeBlocks.RemoveAtSwapBack(num);
			}
			int num3;
			if (this.m_freeSlots.IsEmpty)
			{
				num3 = this.m_usedBlocks.Length;
				this.m_usedBlocks.Add(block2);
			}
			else
			{
				num3 = this.m_freeSlots[this.m_freeSlots.Length - 1];
				this.m_freeSlots.RemoveAtSwapBack(this.m_freeSlots.Length - 1);
				this.m_usedBlocks[num3] = block2;
			}
			this.m_FreeElementCount -= elementCounts;
			return new BlockAllocator.Allocation
			{
				handle = num3,
				block = block2
			};
		}

		private int MergeBlockFrontBack(int freeBlockId)
		{
			BlockAllocator.Block block = this.m_freeBlocks[freeBlockId];
			for (int i = 0; i < this.m_freeBlocks.Length; i++)
			{
				if (i != freeBlockId)
				{
					BlockAllocator.Block block2 = this.m_freeBlocks[i];
					bool flag = false;
					if (block.offset == block2.offset + block2.count)
					{
						block2.count += block.count;
						flag = true;
					}
					else if (block2.offset == block.offset + block.count)
					{
						block2.offset = block.offset;
						block2.count += block.count;
						flag = true;
					}
					if (flag)
					{
						this.m_freeBlocks[i] = block2;
						this.m_freeBlocks.RemoveAtSwapBack(freeBlockId);
						if (i != this.m_freeBlocks.Length)
						{
							return i;
						}
						return freeBlockId;
					}
				}
			}
			return -1;
		}

		public void FreeAllocation(in BlockAllocator.Allocation allocation)
		{
			this.m_freeSlots.Add(allocation.handle);
			this.m_usedBlocks[allocation.handle] = BlockAllocator.Block.Invalid;
			int num = this.m_freeBlocks.Length;
			this.m_freeBlocks.Add(allocation.block);
			while (num != -1)
			{
				num = this.MergeBlockFrontBack(num);
			}
			this.m_FreeElementCount += allocation.block.count;
		}

		public BlockAllocator.Allocation[] SplitAllocation(in BlockAllocator.Allocation allocation, int count)
		{
			BlockAllocator.Allocation[] array = new BlockAllocator.Allocation[count];
			int num = allocation.block.count / count;
			BlockAllocator.Block block = new BlockAllocator.Block
			{
				offset = allocation.block.offset,
				count = num
			};
			this.m_usedBlocks[allocation.handle] = block;
			array[0] = new BlockAllocator.Allocation
			{
				handle = allocation.handle,
				block = block
			};
			for (int i = 1; i < count; i++)
			{
				BlockAllocator.Block block2 = new BlockAllocator.Block
				{
					offset = allocation.block.offset + i * num,
					count = num
				};
				int num2;
				if (this.m_freeSlots.IsEmpty)
				{
					num2 = this.m_usedBlocks.Length;
					this.m_usedBlocks.Add(block2);
				}
				else
				{
					num2 = this.m_freeSlots[this.m_freeSlots.Length - 1];
					this.m_freeSlots.RemoveAtSwapBack(this.m_freeSlots.Length - 1);
					this.m_usedBlocks[num2] = block2;
				}
				array[i] = new BlockAllocator.Allocation
				{
					handle = num2,
					block = block2
				};
			}
			return array;
		}

		private int m_FreeElementCount;

		private int m_MaxElementCount;

		private NativeList<BlockAllocator.Block> m_freeBlocks;

		private NativeList<BlockAllocator.Block> m_usedBlocks;

		private NativeList<int> m_freeSlots;

		public struct Block
		{
			public int offset;

			public int count;

			public static readonly BlockAllocator.Block Invalid = new BlockAllocator.Block
			{
				offset = 0,
				count = 0
			};
		}

		public struct Allocation
		{
			public readonly bool valid
			{
				get
				{
					return this.handle != -1;
				}
			}

			public int handle;

			public BlockAllocator.Block block;

			public static readonly BlockAllocator.Allocation Invalid = new BlockAllocator.Allocation
			{
				handle = -1
			};
		}
	}
}
