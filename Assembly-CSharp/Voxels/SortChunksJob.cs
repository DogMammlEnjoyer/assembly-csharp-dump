using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	[BurstCompile]
	public struct SortChunksJob : IJob
	{
		public void Execute()
		{
			int count = this.ChunkSet.Count;
			this.SortedChunks.ResizeUninitialized(count);
			NativeArray<int3> nativeArray = new NativeArray<int3>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num = 0;
			foreach (int3 value in this.ChunkSet)
			{
				nativeArray[num++] = value;
			}
			NativeArray<SortChunksJob.SortKey> array = new NativeArray<SortChunksJob.SortKey>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < count; i++)
			{
				uint num2 = (uint)math.distancesq(nativeArray[i], this.TargetPos);
				array[i] = new SortChunksJob.SortKey((ulong)num2 << 32 | (ulong)i);
			}
			array.Sort<SortChunksJob.SortKey>();
			for (int j = 0; j < count; j++)
			{
				int index = (int)(array[j].value & (ulong)-1);
				this.SortedChunks[j] = nativeArray[index];
			}
			array.Dispose();
			nativeArray.Dispose();
		}

		[ReadOnly]
		public NativeHashSet<int3> ChunkSet;

		[ReadOnly]
		public int3 TargetPos;

		public NativeList<int3> SortedChunks;

		private struct SortKey : IComparable<SortChunksJob.SortKey>
		{
			public SortKey(ulong val)
			{
				this.value = val;
			}

			public int CompareTo(SortChunksJob.SortKey other)
			{
				return this.value.CompareTo(other.value);
			}

			public ulong value;
		}
	}
}
