using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.Collections;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal sealed class PersistentGpuArray<Tstruct> : IDisposable where Tstruct : struct
	{
		public int elementCount
		{
			get
			{
				return this.m_ElementCount;
			}
		}

		public PersistentGpuArray(int initialSize)
		{
			this.m_SlotAllocator.Initialize(initialSize);
			this.m_GpuBuffer = new ComputeBuffer(initialSize, Marshal.SizeOf<Tstruct>());
			this.m_CpuList = new NativeArray<Tstruct>(initialSize, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_Updates = new BitArray(initialSize);
			this.m_ElementCount = 0;
		}

		public void Dispose()
		{
			this.m_ElementCount = 0;
			this.m_SlotAllocator.Dispose();
			this.m_GpuBuffer.Dispose();
			this.m_CpuList.Dispose();
		}

		public BlockAllocator.Allocation Add(Tstruct element)
		{
			this.m_ElementCount++;
			BlockAllocator.Allocation allocation = this.m_SlotAllocator.Allocate(1);
			if (!allocation.valid)
			{
				this.Grow();
				allocation = this.m_SlotAllocator.Allocate(1);
			}
			this.m_CpuList[allocation.block.offset] = element;
			this.m_Updates[allocation.block.offset] = true;
			this.m_gpuBufferDirty = true;
			return allocation;
		}

		public BlockAllocator.Allocation[] Add(int elementCount)
		{
			this.m_ElementCount += elementCount;
			BlockAllocator.Allocation allocation = this.m_SlotAllocator.Allocate(elementCount);
			if (!allocation.valid)
			{
				this.Grow();
				allocation = this.m_SlotAllocator.Allocate(elementCount);
			}
			return this.m_SlotAllocator.SplitAllocation(allocation, elementCount);
		}

		public void Remove(BlockAllocator.Allocation allocation)
		{
			this.m_ElementCount--;
			this.m_SlotAllocator.FreeAllocation(allocation);
		}

		public void Clear()
		{
			this.m_ElementCount = 0;
			int capacity = this.m_SlotAllocator.capacity;
			this.m_SlotAllocator.Dispose();
			this.m_SlotAllocator = default(BlockAllocator);
			this.m_SlotAllocator.Initialize(capacity);
			this.m_Updates = new BitArray(capacity);
			this.m_gpuBufferDirty = false;
		}

		public void Set(BlockAllocator.Allocation allocation, Tstruct element)
		{
			this.m_CpuList[allocation.block.offset] = element;
			this.m_Updates[allocation.block.offset] = true;
			this.m_gpuBufferDirty = true;
		}

		public Tstruct Get(BlockAllocator.Allocation allocation)
		{
			return this.m_CpuList[allocation.block.offset];
		}

		public void ModifyForEach(Func<Tstruct, Tstruct> lambda)
		{
			for (int i = 0; i < this.m_CpuList.Length; i++)
			{
				this.m_CpuList[i] = lambda(this.m_CpuList[i]);
				this.m_Updates[i] = true;
			}
			this.m_gpuBufferDirty = true;
		}

		public ComputeBuffer GetGpuBuffer(CommandBuffer cmd)
		{
			if (this.m_gpuBufferDirty)
			{
				int num = -1;
				for (int i = 0; i < this.m_Updates.Length; i++)
				{
					if (this.m_Updates[i])
					{
						if (num == -1)
						{
							num = i;
						}
						this.m_Updates[i] = false;
					}
					else if (num != -1)
					{
						int num2 = i;
						cmd.SetBufferData<Tstruct>(this.m_GpuBuffer, this.m_CpuList, num, num, num2 - num);
						num = -1;
					}
				}
				if (num != -1)
				{
					int length = this.m_Updates.Length;
					cmd.SetBufferData<Tstruct>(this.m_GpuBuffer, this.m_CpuList, num, num, length - num);
				}
				this.m_gpuBufferDirty = false;
			}
			return this.m_GpuBuffer;
		}

		private void Grow()
		{
			int capacity = this.m_SlotAllocator.capacity;
			this.m_SlotAllocator.Grow(this.m_SlotAllocator.capacity + 1, int.MaxValue);
			this.m_GpuBuffer.Dispose();
			this.m_GpuBuffer = new ComputeBuffer(this.m_SlotAllocator.capacity, Marshal.SizeOf<Tstruct>());
			NativeArray<Tstruct> cpuList = this.m_CpuList;
			this.m_CpuList = new NativeArray<Tstruct>(this.m_SlotAllocator.capacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			NativeArray<Tstruct>.Copy(cpuList, this.m_CpuList, capacity);
			cpuList.Dispose();
			BitArray updates = this.m_Updates;
			this.m_Updates = new BitArray(this.m_SlotAllocator.capacity);
			for (int i = 0; i < capacity; i++)
			{
				this.m_Updates[i] = updates[i];
			}
		}

		private BlockAllocator m_SlotAllocator;

		private ComputeBuffer m_GpuBuffer;

		private NativeArray<Tstruct> m_CpuList;

		private BitArray m_Updates;

		private bool m_gpuBufferDirty = true;

		private int m_ElementCount;
	}
}
