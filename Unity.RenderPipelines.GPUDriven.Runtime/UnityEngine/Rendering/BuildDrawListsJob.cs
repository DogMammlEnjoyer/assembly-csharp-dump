using System;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct BuildDrawListsJob : IJobParallelFor
	{
		private unsafe static int IncrementCounter(int* counter)
		{
			return Interlocked.Increment(UnsafeUtility.AsRef<int>((void*)counter)) - 1;
		}

		public unsafe void Execute(int index)
		{
			ref DrawInstance ptr = ref this.drawInstances.ElementAt(index);
			int num = this.batchHash[ptr.key];
			ref DrawBatch ptr2 = ref this.drawBatches.ElementAt(num);
			int num2 = BuildDrawListsJob.IncrementCounter((int*)((byte*)this.internalDrawIndex.GetUnsafePtr<int>() + (IntPtr)(num * 16) * 4));
			int index2 = ptr2.instanceOffset + num2;
			this.drawInstanceIndices[index2] = ptr.instanceIndex;
		}

		public const int k_BatchSize = 128;

		public const int k_IntsPerCacheLine = 16;

		[ReadOnly]
		public NativeParallelHashMap<DrawKey, int> batchHash;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[ReadOnly]
		public NativeList<DrawInstance> drawInstances;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[ReadOnly]
		public NativeList<DrawBatch> drawBatches;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<int> internalDrawIndex;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<int> drawInstanceIndices;
	}
}
