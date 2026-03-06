using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	[BurstCompile(FloatMode = FloatMode.Fast, DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct TileRangeExpansionJob : IJobFor
	{
		public void Execute(int jobIndex)
		{
			int num = jobIndex % this.tileResolution.y;
			int num2 = jobIndex / this.tileResolution.y;
			int num3 = 0;
			NativeArray<short> nativeArray = new NativeArray<short>(this.itemsPerTile, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<InclusiveRange> nativeArray2 = new NativeArray<InclusiveRange>(this.itemsPerTile, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < this.itemsPerTile; i++)
			{
				InclusiveRange value = this.tileRanges[num2 * this.rangesPerItem * this.itemsPerTile + i * this.rangesPerItem + 1 + num];
				if (!value.isEmpty)
				{
					nativeArray[num3] = (short)i;
					nativeArray2[num3] = value;
					num3++;
				}
			}
			int num4 = num2 * this.wordsPerTile * this.tileResolution.x * this.tileResolution.y + num * this.wordsPerTile * this.tileResolution.x;
			for (int j = 0; j < this.tileResolution.x; j++)
			{
				int num5 = num4 + j * this.wordsPerTile;
				for (int k = 0; k < num3; k++)
				{
					int num6 = (int)nativeArray[k];
					int num7 = num6 / 32;
					uint num8 = 1U << num6 % 32;
					if (nativeArray2[k].Contains((short)j))
					{
						ref NativeArray<uint> ptr = ref this.tileMasks;
						int index = num5 + num7;
						ptr[index] |= num8;
					}
				}
			}
			nativeArray.Dispose();
			nativeArray2.Dispose();
		}

		[ReadOnly]
		public NativeArray<InclusiveRange> tileRanges;

		[NativeDisableParallelForRestriction]
		public NativeArray<uint> tileMasks;

		public int rangesPerItem;

		public int itemsPerTile;

		public int wordsPerTile;

		public int2 tileResolution;
	}
}
