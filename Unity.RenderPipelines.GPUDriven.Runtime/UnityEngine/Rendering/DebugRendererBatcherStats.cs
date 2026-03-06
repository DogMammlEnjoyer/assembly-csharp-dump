using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	internal class DebugRendererBatcherStats : IDisposable
	{
		public DebugRendererBatcherStats()
		{
			this.instanceCullerStats = new NativeList<InstanceCullerViewStats>(Allocator.Persistent);
			this.instanceOcclusionEventStats = new NativeList<InstanceOcclusionEventStats>(Allocator.Persistent);
			this.occluderStats = new NativeList<DebugOccluderStats>(Allocator.Persistent);
		}

		public void FinalizeInstanceCullerViewStats()
		{
			for (int i = 0; i < this.instanceCullerStats.Length; i++)
			{
				InstanceCullerViewStats instanceCullerViewStats = this.instanceCullerStats[i];
				InstanceOcclusionEventStats lastInstanceOcclusionEventStatsForView = this.GetLastInstanceOcclusionEventStatsForView(i);
				if (lastInstanceOcclusionEventStatsForView.viewInstanceID == instanceCullerViewStats.viewInstanceID)
				{
					instanceCullerViewStats.visibleInstancesOnGPU = Math.Min(lastInstanceOcclusionEventStatsForView.visibleInstances, instanceCullerViewStats.visibleInstancesOnCPU);
					instanceCullerViewStats.visiblePrimitivesOnGPU = Math.Min(lastInstanceOcclusionEventStatsForView.visiblePrimitives, instanceCullerViewStats.visiblePrimitivesOnCPU);
				}
				else
				{
					instanceCullerViewStats.visibleInstancesOnGPU = instanceCullerViewStats.visibleInstancesOnCPU;
					instanceCullerViewStats.visiblePrimitivesOnGPU = instanceCullerViewStats.visiblePrimitivesOnCPU;
				}
				this.instanceCullerStats[i] = instanceCullerViewStats;
			}
		}

		private InstanceOcclusionEventStats GetLastInstanceOcclusionEventStatsForView(int viewIndex)
		{
			if (viewIndex < this.instanceCullerStats.Length)
			{
				int viewInstanceID = this.instanceCullerStats[viewIndex].viewInstanceID;
				for (int i = this.instanceOcclusionEventStats.Length - 1; i >= 0; i--)
				{
					if (this.instanceOcclusionEventStats[i].viewInstanceID == viewInstanceID)
					{
						return this.instanceOcclusionEventStats[i];
					}
				}
			}
			return default(InstanceOcclusionEventStats);
		}

		public void Dispose()
		{
			if (this.instanceCullerStats.IsCreated)
			{
				this.instanceCullerStats.Dispose();
			}
			if (this.instanceOcclusionEventStats.IsCreated)
			{
				this.instanceOcclusionEventStats.Dispose();
			}
			if (this.occluderStats.IsCreated)
			{
				this.occluderStats.Dispose();
			}
		}

		public bool enabled;

		public NativeList<InstanceCullerViewStats> instanceCullerStats;

		public NativeList<InstanceOcclusionEventStats> instanceOcclusionEventStats;

		public NativeList<DebugOccluderStats> occluderStats;

		public bool occlusionOverlayEnabled;

		public bool occlusionOverlayCountVisible;

		public bool overrideOcclusionTestToAlwaysPass;
	}
}
