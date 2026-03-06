using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class MathUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Approximately(float a, float b)
		{
			float num = b - a;
			return ((num >= 0f) ? num : (-num)) < MathUtility.EpsilonScaled;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ApproximatelyZero(float a)
		{
			return ((a >= 0f) ? a : (-a)) < MathUtility.EpsilonScaled;
		}

		public static double Clamp(double input, double min, double max)
		{
			if (input > max)
			{
				return max;
			}
			if (input >= min)
			{
				return input;
			}
			return min;
		}

		public static double ShortestAngleDistance(double start, double end, double halfMax, double max)
		{
			double num = end - start;
			int num2 = Math.Sign(num);
			num = Math.Abs(num) % max;
			if (num > halfMax)
			{
				num = -(max - num);
			}
			return num * (double)num2;
		}

		public static float ShortestAngleDistance(float start, float end, float halfMax, float max)
		{
			float num = end - start;
			float num2 = Mathf.Sign(num);
			num = Math.Abs(num) % max;
			if (num > halfMax)
			{
				num = -(max - num);
			}
			return num * num2;
		}

		public static bool IsUndefined(this float value)
		{
			return float.IsInfinity(value) || float.IsNaN(value);
		}

		public static bool IsAxisAligned(this Vector3 v)
		{
			return MathUtility.ApproximatelyZero(v.x * v.y) && MathUtility.ApproximatelyZero(v.y * v.z) && MathUtility.ApproximatelyZero(v.z * v.x);
		}

		public static bool IsPositivePowerOfTwo(int value)
		{
			return value > 0 && (value & value - 1) == 0;
		}

		public static int FirstActiveFlagIndex(int value)
		{
			if (value == 0)
			{
				return 0;
			}
			for (int i = 0; i < 32; i++)
			{
				if ((value & 1 << i) != 0)
				{
					return i;
				}
			}
			return 0;
		}

		internal static readonly float EpsilonScaled = Mathf.Epsilon * 8f;
	}
}
