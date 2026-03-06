using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct FindDrawInstancesJob : IJobParallelForBatch
	{
		public unsafe void Execute(int startIndex, int count)
		{
			int* ptr = stackalloc int[(UIntPtr)512];
			int count2 = 0;
			for (int i = startIndex; i < startIndex + count; i++)
			{
				ref DrawInstance ptr2 = ref this.drawInstances.ElementAt(i);
				if (this.instancesSorted.BinarySearch(InstanceHandle.FromInt(ptr2.instanceIndex)) >= 0)
				{
					ptr[(IntPtr)(count2++) * 4] = i;
				}
			}
			this.outDrawInstanceIndicesWriter.AddRangeNoResize((void*)ptr, count2);
		}

		public const int k_BatchSize = 128;

		[ReadOnly]
		public NativeArray<InstanceHandle> instancesSorted;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[ReadOnly]
		public NativeList<DrawInstance> drawInstances;

		[WriteOnly]
		public NativeList<int>.ParallelWriter outDrawInstanceIndicesWriter;
	}
}
