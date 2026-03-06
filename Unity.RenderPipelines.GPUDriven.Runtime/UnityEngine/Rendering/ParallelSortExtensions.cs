using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	internal static class ParallelSortExtensions
	{
		internal static JobHandle ParallelSort(this NativeArray<int> array)
		{
			if (array.Length <= 1)
			{
				return default(JobHandle);
			}
			JobHandle jobHandle = default(JobHandle);
			if (array.Length >= 2048)
			{
				int num = Mathf.Max(JobsUtility.JobWorkerCount + 1, 1);
				int num2 = Mathf.Max(256, Mathf.CeilToInt((float)array.Length / (float)num));
				int num3 = Mathf.CeilToInt((float)array.Length / (float)num2);
				NativeArray<int> nativeArray = new NativeArray<int>(array.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> counter = new NativeArray<int>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				NativeArray<int> buckets = new NativeArray<int>(num3 * 256, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				NativeArray<int> indices = new NativeArray<int>(num3 * 256, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> indicesSum = new NativeArray<int>(16, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> array2 = array;
				NativeArray<int> arraySorted = nativeArray;
				for (int i = 0; i < 4; i++)
				{
					ParallelSortExtensions.RadixSortBucketCountJob jobData = new ParallelSortExtensions.RadixSortBucketCountJob
					{
						radix = i,
						jobsCount = num3,
						batchSize = num2,
						buckets = buckets,
						array = array2
					};
					ParallelSortExtensions.RadixSortBatchPrefixSumJob jobData2 = new ParallelSortExtensions.RadixSortBatchPrefixSumJob
					{
						radix = i,
						jobsCount = num3,
						array = array2,
						counter = counter,
						buckets = buckets,
						indices = indices,
						indicesSum = indicesSum
					};
					ParallelSortExtensions.RadixSortPrefixSumJob jobData3 = new ParallelSortExtensions.RadixSortPrefixSumJob
					{
						jobsCount = num3,
						indices = indices,
						indicesSum = indicesSum
					};
					ParallelSortExtensions.RadixSortBucketSortJob jobData4 = new ParallelSortExtensions.RadixSortBucketSortJob
					{
						radix = i,
						batchSize = num2,
						indices = indices,
						array = array2,
						arraySorted = arraySorted
					};
					jobHandle = jobData.ScheduleParallel(num3, 1, jobHandle);
					jobHandle = jobData2.ScheduleParallel(16, 1, jobHandle);
					jobHandle = jobData3.ScheduleParallel(16, 1, jobHandle);
					jobHandle = jobData4.ScheduleParallel(num3, 1, jobHandle);
					JobHandle.ScheduleBatchedJobs();
					ParallelSortExtensions.<ParallelSort>g__Swap|2_0(ref array2, ref arraySorted);
				}
				nativeArray.Dispose(jobHandle);
				counter.Dispose(jobHandle);
				buckets.Dispose(jobHandle);
				indices.Dispose(jobHandle);
				indicesSum.Dispose(jobHandle);
			}
			else
			{
				jobHandle = array.SortJob<int>().Schedule(default(JobHandle));
			}
			return jobHandle;
		}

		[CompilerGenerated]
		internal static void <ParallelSort>g__Swap|2_0(ref NativeArray<int> a, ref NativeArray<int> b)
		{
			NativeArray<int> nativeArray = a;
			a = b;
			b = nativeArray;
		}

		private const int kMinRadixSortArraySize = 2048;

		private const int kMinRadixSortBatchSize = 256;

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		internal struct RadixSortBucketCountJob : IJobFor
		{
			public void Execute(int index)
			{
				int num = index * this.batchSize;
				int num2 = math.min(num + this.batchSize, this.array.Length);
				int num3 = index * 256;
				for (int i = num; i < num2; i++)
				{
					int num4 = this.array[i] >> this.radix * 8 & 255;
					ref NativeArray<int> ptr = ref this.buckets;
					int index2 = num3 + num4;
					ptr[index2]++;
				}
			}

			[ReadOnly]
			public int radix;

			[ReadOnly]
			public int jobsCount;

			[ReadOnly]
			public int batchSize;

			[ReadOnly]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> array;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> buckets;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		internal struct RadixSortBatchPrefixSumJob : IJobFor
		{
			private static int AtomicIncrement(NativeArray<int> counter)
			{
				return Interlocked.Increment(UnsafeUtility.AsRef<int>(counter.GetUnsafePtr<int>()));
			}

			private int JobIndexPrefixSum(int sum, int i)
			{
				for (int j = 0; j < this.jobsCount; j++)
				{
					int index = i + j * 256;
					this.indices[index] = sum;
					sum += this.buckets[index];
					this.buckets[index] = 0;
				}
				return sum;
			}

			public void Execute(int index)
			{
				int num = index * 16;
				int num2 = num + 16;
				int num3 = 0;
				for (int i = num; i < num2; i++)
				{
					num3 = this.JobIndexPrefixSum(num3, i);
				}
				this.indicesSum[index] = num3;
				if (ParallelSortExtensions.RadixSortBatchPrefixSumJob.AtomicIncrement(this.counter) == 16)
				{
					int num4 = 0;
					if (this.radix < 3)
					{
						for (int j = 0; j < 16; j++)
						{
							int num5 = this.indicesSum[j];
							this.indicesSum[j] = num4;
							num4 += num5;
						}
					}
					else
					{
						for (int k = 8; k < 16; k++)
						{
							int num6 = this.indicesSum[k];
							this.indicesSum[k] = num4;
							num4 += num6;
						}
						for (int l = 0; l < 8; l++)
						{
							int num7 = this.indicesSum[l];
							this.indicesSum[l] = num4;
							num4 += num7;
						}
					}
					this.counter[0] = 0;
				}
			}

			[ReadOnly]
			public int radix;

			[ReadOnly]
			public int jobsCount;

			[ReadOnly]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> array;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> counter;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> indicesSum;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> buckets;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> indices;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		internal struct RadixSortPrefixSumJob : IJobFor
		{
			public void Execute(int index)
			{
				int num = index * 16;
				int num2 = num + 16;
				int num3 = this.indicesSum[index];
				for (int i = 0; i < this.jobsCount; i++)
				{
					for (int j = num; j < num2; j++)
					{
						int num4 = i * 256 + j;
						ref NativeArray<int> ptr = ref this.indices;
						int index2 = num4;
						ptr[index2] += num3;
					}
				}
			}

			[ReadOnly]
			public int jobsCount;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> indicesSum;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> indices;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		internal struct RadixSortBucketSortJob : IJobFor
		{
			public void Execute(int index)
			{
				int num = index * this.batchSize;
				int num2 = math.min(num + this.batchSize, this.array.Length);
				int num3 = index * 256;
				for (int i = num; i < num2; i++)
				{
					int num4 = this.array[i];
					int num5 = num4 >> this.radix * 8 & 255;
					int index2 = num3 + num5;
					int num6 = this.indices[index2];
					this.indices[index2] = num6 + 1;
					int index3 = num6;
					this.arraySorted[index3] = num4;
				}
			}

			[ReadOnly]
			public int radix;

			[ReadOnly]
			public int batchSize;

			[ReadOnly]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> array;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> indices;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public NativeArray<int> arraySorted;
		}
	}
}
