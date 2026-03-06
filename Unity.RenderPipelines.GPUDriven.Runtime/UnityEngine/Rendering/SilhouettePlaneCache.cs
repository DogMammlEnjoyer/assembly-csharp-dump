using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	internal struct SilhouettePlaneCache : IDisposable
	{
		public void Init()
		{
			this.m_SubviewIDToIndexMap = new NativeParallelHashMap<int, int>(16, Allocator.Persistent);
			this.m_SlotFreeList = new NativeList<int>(16, Allocator.Persistent);
			this.m_Slots = new NativeList<SilhouettePlaneCache.Slot>(16, Allocator.Persistent);
			this.m_PlaneStorage = new NativeList<Plane>(96, Allocator.Persistent);
		}

		public void Dispose()
		{
			this.m_SubviewIDToIndexMap.Dispose();
			this.m_SlotFreeList.Dispose();
			this.m_Slots.Dispose();
			this.m_PlaneStorage.Dispose();
		}

		public void Update(int viewInstanceID, NativeArray<Plane> planes, int frameIndex)
		{
			int num = Math.Min(planes.Length, 6);
			int num2;
			if (!this.m_SubviewIDToIndexMap.TryGetValue(viewInstanceID, out num2))
			{
				if (this.m_SlotFreeList.Length > 0)
				{
					num2 = this.m_SlotFreeList[this.m_SlotFreeList.Length - 1];
					this.m_SlotFreeList.Length = this.m_SlotFreeList.Length - 1;
				}
				else
				{
					if (this.m_Slots.Length == this.m_Slots.Capacity)
					{
						int num3 = this.m_Slots.Length + 8;
						this.m_Slots.SetCapacity(num3);
						this.m_PlaneStorage.SetCapacity(num3 * 6);
					}
					num2 = this.m_Slots.Length;
					int num4 = num2 + 1;
					this.m_Slots.ResizeUninitialized(num4);
					this.m_PlaneStorage.ResizeUninitialized(num4 * 6);
				}
				this.m_SubviewIDToIndexMap.Add(viewInstanceID, num2);
			}
			this.m_Slots[num2] = new SilhouettePlaneCache.Slot(viewInstanceID, num, frameIndex);
			this.m_PlaneStorage.AsArray().GetSubArray(num2 * 6, num).CopyFrom(planes);
		}

		public void FreeUnusedSlots(int frameIndex, int maximumAge)
		{
			for (int i = 0; i < this.m_Slots.Length; i++)
			{
				SilhouettePlaneCache.Slot slot = this.m_Slots[i];
				if (slot.isActive && frameIndex - slot.lastUsedFrameIndex > maximumAge)
				{
					slot.isActive = false;
					this.m_Slots[i] = slot;
					this.m_SubviewIDToIndexMap.Remove(slot.viewInstanceID);
					this.m_SlotFreeList.Add(i);
				}
			}
		}

		public NativeArray<Plane> GetSubArray(int viewInstanceID)
		{
			int start = 0;
			int length = 0;
			int num;
			if (this.m_SubviewIDToIndexMap.TryGetValue(viewInstanceID, out num))
			{
				start = num * 6;
				length = this.m_Slots[num].planeCount;
			}
			return this.m_PlaneStorage.AsArray().GetSubArray(start, length);
		}

		private const int kMaxSilhouettePlanes = 6;

		private NativeParallelHashMap<int, int> m_SubviewIDToIndexMap;

		private NativeList<int> m_SlotFreeList;

		private NativeList<SilhouettePlaneCache.Slot> m_Slots;

		private NativeList<Plane> m_PlaneStorage;

		private struct Slot
		{
			public Slot(int viewInstanceID, int planeCount, int frameIndex)
			{
				this.isActive = true;
				this.viewInstanceID = viewInstanceID;
				this.planeCount = planeCount;
				this.lastUsedFrameIndex = frameIndex;
			}

			public bool isActive;

			public int viewInstanceID;

			public int planeCount;

			public int lastUsedFrameIndex;
		}
	}
}
