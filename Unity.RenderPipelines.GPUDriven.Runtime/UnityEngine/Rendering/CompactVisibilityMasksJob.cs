using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct CompactVisibilityMasksJob : IJobParallelForBatch
	{
		public void Execute(int startIndex, int count)
		{
			ulong num = 0UL;
			for (int i = 0; i < count; i++)
			{
				if (this.rendererVisibilityMasks[startIndex + i] != 0)
				{
					num |= 1UL << i;
				}
			}
			int chunk_index = startIndex / 64;
			this.compactedVisibilityMasks.InterlockedOrChunk(chunk_index, num);
		}

		public const int k_BatchSize = 64;

		[ReadOnly]
		public NativeArray<byte> rendererVisibilityMasks;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		public ParallelBitArray compactedVisibilityMasks;
	}
}
