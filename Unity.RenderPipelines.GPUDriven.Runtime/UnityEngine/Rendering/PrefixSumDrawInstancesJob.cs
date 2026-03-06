using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct PrefixSumDrawInstancesJob : IJob
	{
		public void Execute()
		{
			int num = 0;
			for (int i = 0; i < this.drawRanges.Length; i++)
			{
				ref DrawRange ptr = ref this.drawRanges.ElementAt(i);
				ptr.drawOffset = num;
				num += ptr.drawCount;
			}
			NativeArray<int> nativeArray = new NativeArray<int>(this.drawRanges.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int j = 0; j < this.drawBatches.Length; j++)
			{
				ref DrawBatch ptr2 = ref this.drawBatches.ElementAt(j);
				int num2;
				if (this.rangeHash.TryGetValue(ptr2.key.range, out num2))
				{
					ref DrawRange ptr3 = ref this.drawRanges.ElementAt(num2);
					this.drawBatchIndices[ptr3.drawOffset + nativeArray[num2]] = j;
					int index = num2;
					int num3 = nativeArray[index];
					nativeArray[index] = num3 + 1;
				}
			}
			int num4 = 0;
			for (int k = 0; k < this.drawBatchIndices.Length; k++)
			{
				int index2 = this.drawBatchIndices[k];
				ref DrawBatch ptr4 = ref this.drawBatches.ElementAt(index2);
				ptr4.instanceOffset = num4;
				num4 += ptr4.instanceCount;
			}
			nativeArray.Dispose();
		}

		[ReadOnly]
		public NativeParallelHashMap<RangeKey, int> rangeHash;

		public NativeList<DrawRange> drawRanges;

		public NativeList<DrawBatch> drawBatches;

		public NativeArray<int> drawBatchIndices;
	}
}
