using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	[BurstCompile(FloatMode = FloatMode.Fast, DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct ZBinningJob : IJobFor
	{
		private static uint EncodeHeader(uint min, uint max)
		{
			return (min & 65535U) | (max & 65535U) << 16;
		}

		private static ValueTuple<uint, uint> DecodeHeader(uint zBin)
		{
			return new ValueTuple<uint, uint>(zBin & 65535U, zBin >> 16 & 65535U);
		}

		public void Execute(int jobIndex)
		{
			int num = jobIndex % this.batchCount;
			int num2 = jobIndex / this.batchCount;
			int num3 = 128 * num;
			int num4 = math.min(num3 + 128, this.binCount) - 1;
			int num5 = num2 * this.binCount;
			uint value = ZBinningJob.EncodeHeader(65535U, 0U);
			for (int i = num3; i <= num4; i++)
			{
				this.bins[(num5 + i) * (2 + this.wordsPerTile)] = value;
				this.bins[(num5 + i) * (2 + this.wordsPerTile) + 1] = value;
			}
			this.FillZBins(num3, num4, 0, this.lightCount, 0, num2 * this.lightCount, num5);
			this.FillZBins(num3, num4, this.lightCount, this.lightCount + this.reflectionProbeCount, 1, this.lightCount * (this.viewCount - 1) + num2 * this.reflectionProbeCount, num5);
		}

		private void FillZBins(int binStart, int binEnd, int itemStart, int itemEnd, int headerIndex, int itemOffset, int binOffset)
		{
			for (int i = itemStart; i < itemEnd; i++)
			{
				float2 @float = this.minMaxZs[itemOffset + i];
				int num = math.max((int)((this.isOrthographic ? @float.x : math.log2(@float.x)) * this.zBinScale + this.zBinOffset), binStart);
				int num2 = math.min((int)((this.isOrthographic ? @float.y : math.log2(@float.y)) * this.zBinScale + this.zBinOffset), binEnd);
				int num3 = i / 32;
				uint num4 = 1U << i % 32;
				for (int j = num; j <= num2; j++)
				{
					int num5 = (binOffset + j) * (2 + this.wordsPerTile);
					ValueTuple<uint, uint> valueTuple = ZBinningJob.DecodeHeader(this.bins[num5 + headerIndex]);
					uint num6 = valueTuple.Item1;
					uint num7 = valueTuple.Item2;
					num6 = math.min(num6, (uint)i);
					num7 = math.max(num7, (uint)i);
					this.bins[num5 + headerIndex] = ZBinningJob.EncodeHeader(num6, num7);
					ref NativeArray<uint> ptr = ref this.bins;
					int index = num5 + 2 + num3;
					ptr[index] |= num4;
				}
			}
		}

		public const int batchSize = 128;

		public const int headerLength = 2;

		[NativeDisableParallelForRestriction]
		public NativeArray<uint> bins;

		[ReadOnly]
		public NativeArray<float2> minMaxZs;

		public float zBinScale;

		public float zBinOffset;

		public int binCount;

		public int wordsPerTile;

		public int lightCount;

		public int reflectionProbeCount;

		public int batchCount;

		public int viewCount;

		public bool isOrthographic;
	}
}
