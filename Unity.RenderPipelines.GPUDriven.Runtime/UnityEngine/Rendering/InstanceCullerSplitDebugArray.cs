using System;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal struct InstanceCullerSplitDebugArray : IDisposable
	{
		public NativeArray<int> Counters
		{
			get
			{
				return this.m_Counters;
			}
		}

		public void Init()
		{
			this.m_Info = new NativeList<InstanceCullerSplitDebugArray.Info>(Allocator.Persistent);
			this.m_Counters = new NativeArray<int>(192, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_CounterSync = new NativeQueue<JobHandle>(Allocator.Persistent);
		}

		public void Dispose()
		{
			this.m_Info.Dispose();
			this.m_Counters.Dispose();
			this.m_CounterSync.Dispose();
		}

		public int TryAddSplits(BatchCullingViewType viewType, int viewInstanceID, int splitCount)
		{
			int length = this.m_Info.Length;
			if (length + splitCount > 64)
			{
				return -1;
			}
			for (int i = 0; i < splitCount; i++)
			{
				InstanceCullerSplitDebugArray.Info info = default(InstanceCullerSplitDebugArray.Info);
				info.viewType = viewType;
				info.viewInstanceID = viewInstanceID;
				info.splitIndex = i;
				this.m_Info.Add(info);
			}
			return length;
		}

		public void AddSync(int baseIndex, JobHandle jobHandle)
		{
			if (baseIndex != -1)
			{
				this.m_CounterSync.Enqueue(jobHandle);
			}
		}

		public void MoveToDebugStatsAndClear(DebugRendererBatcherStats debugStats)
		{
			JobHandle jobHandle;
			while (this.m_CounterSync.TryDequeue(out jobHandle))
			{
				jobHandle.Complete();
			}
			debugStats.instanceCullerStats.Clear();
			for (int i = 0; i < this.m_Info.Length; i++)
			{
				InstanceCullerSplitDebugArray.Info info = this.m_Info[i];
				int num = i * 3;
				InstanceCullerViewStats instanceCullerViewStats = default(InstanceCullerViewStats);
				instanceCullerViewStats.viewType = info.viewType;
				instanceCullerViewStats.viewInstanceID = info.viewInstanceID;
				instanceCullerViewStats.splitIndex = info.splitIndex;
				instanceCullerViewStats.visibleInstancesOnCPU = this.m_Counters[num];
				instanceCullerViewStats.visibleInstancesOnGPU = 0;
				instanceCullerViewStats.visiblePrimitivesOnCPU = this.m_Counters[num + 1];
				instanceCullerViewStats.visiblePrimitivesOnGPU = 0;
				instanceCullerViewStats.drawCommands = this.m_Counters[num + 2];
				debugStats.instanceCullerStats.Add(instanceCullerViewStats);
			}
			this.m_Info.Clear();
			int num2 = 0;
			ref this.m_Counters.FillArray(num2, 0, -1);
		}

		private const int MaxSplitCount = 64;

		private NativeList<InstanceCullerSplitDebugArray.Info> m_Info;

		private NativeArray<int> m_Counters;

		private NativeQueue<JobHandle> m_CounterSync;

		internal struct Info
		{
			public BatchCullingViewType viewType;

			public int viewInstanceID;

			public int splitIndex;
		}
	}
}
