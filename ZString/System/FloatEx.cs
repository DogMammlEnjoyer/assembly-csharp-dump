using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class FloatEx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFinite(double d)
		{
			return (BitConverter.DoubleToInt64Bits(d) & long.MaxValue) < 9218868437227405312L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNegative(double d)
		{
			return BitConverter.DoubleToInt64Bits(d) < 0L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFinite(float f)
		{
			return (FloatEx.SingleToInt32Bits(f) & int.MaxValue) < 2139095040;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNegative(float f)
		{
			return FloatEx.SingleToInt32Bits(f) < 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int SingleToInt32Bits(float value)
		{
			return *(int*)(&value);
		}
	}
}
