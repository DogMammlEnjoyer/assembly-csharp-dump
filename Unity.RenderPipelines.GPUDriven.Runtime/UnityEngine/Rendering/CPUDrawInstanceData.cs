using System;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal class CPUDrawInstanceData
	{
		public NativeList<DrawInstance> drawInstances
		{
			get
			{
				return this.m_DrawInstances;
			}
		}

		public NativeParallelHashMap<DrawKey, int> batchHash
		{
			get
			{
				return this.m_BatchHash;
			}
		}

		public NativeList<DrawBatch> drawBatches
		{
			get
			{
				return this.m_DrawBatches;
			}
		}

		public NativeParallelHashMap<RangeKey, int> rangeHash
		{
			get
			{
				return this.m_RangeHash;
			}
		}

		public NativeList<DrawRange> drawRanges
		{
			get
			{
				return this.m_DrawRanges;
			}
		}

		public NativeArray<int> drawBatchIndices
		{
			get
			{
				return this.m_DrawBatchIndices.AsArray();
			}
		}

		public NativeArray<int> drawInstanceIndices
		{
			get
			{
				return this.m_DrawInstanceIndices.AsArray();
			}
		}

		public bool valid
		{
			get
			{
				return this.m_DrawInstances.IsCreated;
			}
		}

		public void Initialize()
		{
			this.m_RangeHash = new NativeParallelHashMap<RangeKey, int>(1024, Allocator.Persistent);
			this.m_DrawRanges = new NativeList<DrawRange>(Allocator.Persistent);
			this.m_BatchHash = new NativeParallelHashMap<DrawKey, int>(1024, Allocator.Persistent);
			this.m_DrawBatches = new NativeList<DrawBatch>(Allocator.Persistent);
			this.m_DrawInstances = new NativeList<DrawInstance>(1024, Allocator.Persistent);
			this.m_DrawInstanceIndices = new NativeList<int>(1024, Allocator.Persistent);
			this.m_DrawBatchIndices = new NativeList<int>(1024, Allocator.Persistent);
		}

		public void Dispose()
		{
			if (this.m_DrawBatchIndices.IsCreated)
			{
				this.m_DrawBatchIndices.Dispose();
			}
			if (this.m_DrawInstanceIndices.IsCreated)
			{
				this.m_DrawInstanceIndices.Dispose();
			}
			if (this.m_DrawInstances.IsCreated)
			{
				this.m_DrawInstances.Dispose();
			}
			if (this.m_DrawBatches.IsCreated)
			{
				this.m_DrawBatches.Dispose();
			}
			if (this.m_BatchHash.IsCreated)
			{
				this.m_BatchHash.Dispose();
			}
			if (this.m_DrawRanges.IsCreated)
			{
				this.m_DrawRanges.Dispose();
			}
			if (this.m_RangeHash.IsCreated)
			{
				this.m_RangeHash.Dispose();
			}
		}

		public void RebuildDrawListsIfNeeded()
		{
			if (!this.m_NeedsRebuild)
			{
				return;
			}
			this.m_NeedsRebuild = false;
			this.m_DrawInstanceIndices.ResizeUninitialized(this.m_DrawInstances.Length);
			this.m_DrawBatchIndices.ResizeUninitialized(this.m_DrawBatches.Length);
			NativeArray<int> internalDrawIndex = new NativeArray<int>(this.drawBatches.Length * 16, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			JobHandle dependsOn = new PrefixSumDrawInstancesJob
			{
				rangeHash = this.m_RangeHash,
				drawRanges = this.m_DrawRanges,
				drawBatches = this.m_DrawBatches,
				drawBatchIndices = this.m_DrawBatchIndices.AsArray()
			}.Schedule(default(JobHandle));
			new BuildDrawListsJob
			{
				drawInstances = this.m_DrawInstances,
				batchHash = this.m_BatchHash,
				drawBatches = this.m_DrawBatches,
				internalDrawIndex = internalDrawIndex,
				drawInstanceIndices = this.m_DrawInstanceIndices.AsArray()
			}.Schedule(this.m_DrawInstances.Length, 128, dependsOn).Complete();
			internalDrawIndex.Dispose();
		}

		public void DestroyDrawInstanceIndices(NativeArray<int> drawInstanceIndicesToDestroy)
		{
			drawInstanceIndicesToDestroy.ParallelSort().Complete();
			InstanceCullingBatcherBurst.RemoveDrawInstanceIndices(drawInstanceIndicesToDestroy, ref this.m_DrawInstances, ref this.m_RangeHash, ref this.m_BatchHash, ref this.m_DrawRanges, ref this.m_DrawBatches);
		}

		public void DestroyDrawInstances(NativeArray<InstanceHandle> destroyedInstances)
		{
			if (this.m_DrawInstances.IsEmpty || destroyedInstances.Length == 0)
			{
				return;
			}
			this.NeedsRebuild();
			NativeArray<InstanceHandle> instancesSorted = new NativeArray<InstanceHandle>(destroyedInstances, Allocator.TempJob);
			instancesSorted.Reinterpret<int>().ParallelSort().Complete();
			NativeList<int> nativeList = new NativeList<int>(this.m_DrawInstances.Length, Allocator.TempJob);
			new FindDrawInstancesJob
			{
				instancesSorted = instancesSorted,
				drawInstances = this.m_DrawInstances,
				outDrawInstanceIndicesWriter = nativeList.AsParallelWriter()
			}.ScheduleBatch(this.m_DrawInstances.Length, 128, default(JobHandle)).Complete();
			this.DestroyDrawInstanceIndices(nativeList.AsArray());
			instancesSorted.Dispose();
			nativeList.Dispose();
		}

		public void DestroyMaterialDrawInstances(NativeArray<uint> destroyedBatchMaterials)
		{
			if (this.m_DrawInstances.IsEmpty || destroyedBatchMaterials.Length == 0)
			{
				return;
			}
			this.NeedsRebuild();
			NativeArray<uint> materialsSorted = new NativeArray<uint>(destroyedBatchMaterials, Allocator.TempJob);
			materialsSorted.Reinterpret<int>().ParallelSort().Complete();
			NativeList<int> nativeList = new NativeList<int>(this.m_DrawInstances.Length, Allocator.TempJob);
			new FindMaterialDrawInstancesJob
			{
				materialsSorted = materialsSorted,
				drawInstances = this.m_DrawInstances,
				outDrawInstanceIndicesWriter = nativeList.AsParallelWriter()
			}.ScheduleBatch(this.m_DrawInstances.Length, 128, default(JobHandle)).Complete();
			this.DestroyDrawInstanceIndices(nativeList.AsArray());
			materialsSorted.Dispose();
			nativeList.Dispose();
		}

		public void NeedsRebuild()
		{
			this.m_NeedsRebuild = true;
		}

		private NativeParallelHashMap<RangeKey, int> m_RangeHash;

		private NativeList<DrawRange> m_DrawRanges;

		private NativeParallelHashMap<DrawKey, int> m_BatchHash;

		private NativeList<DrawBatch> m_DrawBatches;

		private NativeList<DrawInstance> m_DrawInstances;

		private NativeList<int> m_DrawInstanceIndices;

		private NativeList<int> m_DrawBatchIndices;

		private bool m_NeedsRebuild;
	}
}
