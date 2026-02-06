using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(4)]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkRNG : INetworkStruct
	{
		public NetworkRNG Peek
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double Next()
		{
			return this.NextUInt32Internal() * 2.3283064370807974E-10;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double NextExclusive()
		{
			return this.NextUInt32Internal() * 2.3283064365386963E-10;
		}

		public float NextSingle()
		{
			return (this.NextUInt32Internal() >> 8) * 5.960465E-08f;
		}

		public float NextSingleExclusive()
		{
			return (this.NextUInt32Internal() >> 8) * 5.9604645E-08f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int NextInt32()
		{
			return (int)this.NextUInt32Internal();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint NextUInt32()
		{
			return this.NextUInt32Internal();
		}

		private uint NextUnbiasedUInt32(uint max)
		{
			uint num = (0U - max) % max;
			uint num2;
			bool flag;
			do
			{
				num2 = this.NextUInt32Internal();
				flag = (num2 >= num);
			}
			while (!flag);
			return num2 % max;
		}

		public NetworkRNG(int seed)
		{
			ulong num = (ulong)((long)seed);
			this._state = NetworkRNG.NextSplitMix64(ref num);
			this._inc = NetworkRNG.NextSplitMix64(ref num);
		}

		private uint NextUInt32Internal()
		{
			ulong state = this._state;
			this._state = state * 6364136223846793005UL + (this._inc | 1UL);
			uint num = (uint)((state >> 18 ^ state) >> 27);
			int num2 = (int)(state >> 59);
			return num >> num2 | num << -num2;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}, {2}:{3}", new object[]
			{
				"_state",
				this._state,
				"_inc",
				this._inc
			});
		}

		private static ulong NextSplitMix64(ref ulong x)
		{
			ulong num = x += 11400714819323198485UL;
			num = (num ^ num >> 30) * 13787848793156543929UL;
			num = (num ^ num >> 27) * 10723151780598845931UL;
			return num ^ num >> 31;
		}

		public double RangeInclusive(double minInclusive, double maxInclusive)
		{
			bool flag = minInclusive > maxInclusive;
			if (flag)
			{
				double num = minInclusive;
				minInclusive = maxInclusive;
				maxInclusive = num;
			}
			return minInclusive + this.Next() * (maxInclusive - minInclusive);
		}

		public float RangeInclusive(float minInclusive, float maxInclusive)
		{
			bool flag = minInclusive > maxInclusive;
			if (flag)
			{
				float num = minInclusive;
				minInclusive = maxInclusive;
				maxInclusive = num;
			}
			return minInclusive + this.NextSingle() * (maxInclusive - minInclusive);
		}

		public int RangeExclusive(int minInclusive, int maxExclusive)
		{
			bool flag = minInclusive == maxExclusive;
			int result;
			if (flag)
			{
				result = minInclusive;
			}
			else
			{
				bool flag2 = minInclusive > maxExclusive;
				if (flag2)
				{
					int num = minInclusive;
					minInclusive = maxExclusive;
					maxExclusive = num;
				}
				uint max = (uint)(maxExclusive - minInclusive);
				uint num2 = this.NextUnbiasedUInt32(max);
				result = (int)((long)minInclusive + (long)((ulong)num2));
			}
			return result;
		}

		public int RangeInclusive(int minInclusive, int maxInclusive)
		{
			bool flag = minInclusive > maxInclusive;
			if (flag)
			{
				int num = minInclusive;
				minInclusive = maxInclusive;
				maxInclusive = num;
			}
			uint num2 = (uint)(maxInclusive - minInclusive + 1);
			bool flag2 = num2 == 0U;
			int result;
			if (flag2)
			{
				result = (int)this.NextUInt32Internal();
			}
			else
			{
				uint num3 = this.NextUnbiasedUInt32(num2);
				result = (int)((long)minInclusive + (long)((ulong)num3));
			}
			return result;
		}

		public uint RangeExclusive(uint minInclusive, uint maxExclusive)
		{
			bool flag = minInclusive == maxExclusive;
			uint result;
			if (flag)
			{
				result = minInclusive;
			}
			else
			{
				bool flag2 = minInclusive > maxExclusive;
				if (flag2)
				{
					uint num = minInclusive;
					minInclusive = maxExclusive;
					maxExclusive = num;
				}
				uint max = maxExclusive - minInclusive;
				uint num2 = this.NextUnbiasedUInt32(max);
				result = minInclusive + num2;
			}
			return result;
		}

		public uint RangeInclusive(uint minInclusive, uint maxInclusive)
		{
			bool flag = minInclusive > maxInclusive;
			if (flag)
			{
				uint num = minInclusive;
				minInclusive = maxInclusive;
				maxInclusive = num;
			}
			uint num2 = maxInclusive - minInclusive + 1U;
			bool flag2 = num2 == 0U;
			uint result;
			if (flag2)
			{
				result = this.NextUInt32Internal();
			}
			else
			{
				uint num3 = this.NextUnbiasedUInt32(num2);
				result = minInclusive + num3;
			}
			return result;
		}

		internal const double FP_32_32_ToUnitDoubleInclusive = 2.3283064370807974E-10;

		internal const double FP_32_32_ToUnitDoubleExclusive = 2.3283064365386963E-10;

		internal const float FP_8_24_ToUnitSingleInclusive = 5.960465E-08f;

		internal const float FP_8_24_ToUnitSingleExclusive = 5.9604645E-08f;

		public const int SIZE = 16;

		public const uint MAX = 4294967295U;

		[FieldOffset(0)]
		private ulong _state;

		[FieldOffset(8)]
		private ulong _inc;
	}
}
