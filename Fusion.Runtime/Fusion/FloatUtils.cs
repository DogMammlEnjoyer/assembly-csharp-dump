using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class FloatUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int Compress(float f, int accuracy = 1024)
		{
			return Maths.ZigZagEncode((int)(f * (float)accuracy + (0.5f - (*(uint*)(&f) >> 31))));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Decompress(int value, float accuracy = 1024f)
		{
			return (float)Maths.ZigZagDecode(value) / accuracy;
		}

		public const int DEFAULT_ACCURACY = 1024;
	}
}
