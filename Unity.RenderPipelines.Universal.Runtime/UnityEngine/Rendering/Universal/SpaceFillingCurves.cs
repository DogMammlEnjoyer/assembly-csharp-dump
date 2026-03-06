using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal static class SpaceFillingCurves
	{
		private static uint Part1By1(uint x)
		{
			x &= 65535U;
			x = ((x ^ x << 8) & 16711935U);
			x = ((x ^ x << 4) & 252645135U);
			x = ((x ^ x << 2) & 858993459U);
			x = ((x ^ x << 1) & 1431655765U);
			return x;
		}

		private static uint Compact1By1(uint x)
		{
			x &= 1431655765U;
			x = ((x ^ x >> 1) & 858993459U);
			x = ((x ^ x >> 2) & 252645135U);
			x = ((x ^ x >> 4) & 16711935U);
			x = ((x ^ x >> 8) & 65535U);
			return x;
		}

		public static uint EncodeMorton2D(uint2 coord)
		{
			return (SpaceFillingCurves.Part1By1(coord.y) << 1) + SpaceFillingCurves.Part1By1(coord.x);
		}

		public static uint2 DecodeMorton2D(uint code)
		{
			return math.uint2(SpaceFillingCurves.Compact1By1(code), SpaceFillingCurves.Compact1By1(code >> 1));
		}
	}
}
