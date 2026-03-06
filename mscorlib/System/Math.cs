using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System
{
	/// <summary>Provides constants and static methods for trigonometric, logarithmic, and other common mathematical functions.</summary>
	public static class Math
	{
		/// <summary>Returns the absolute value of a 16-bit signed integer.</summary>
		/// <param name="value">A number that is greater than <see cref="F:System.Int16.MinValue" />, but less than or equal to <see cref="F:System.Int16.MaxValue" />.</param>
		/// <returns>A 16-bit signed integer, x, such that 0 ≤ x ≤<see cref="F:System.Int16.MaxValue" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> equals <see cref="F:System.Int16.MinValue" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short Abs(short value)
		{
			if (value < 0)
			{
				value = -value;
				if (value < 0)
				{
					Math.ThrowAbsOverflow();
				}
			}
			return value;
		}

		/// <summary>Returns the absolute value of a 32-bit signed integer.</summary>
		/// <param name="value">A number that is greater than <see cref="F:System.Int32.MinValue" />, but less than or equal to <see cref="F:System.Int32.MaxValue" />.</param>
		/// <returns>A 32-bit signed integer, x, such that 0 ≤ x ≤<see cref="F:System.Int32.MaxValue" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> equals <see cref="F:System.Int32.MinValue" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Abs(int value)
		{
			if (value < 0)
			{
				value = -value;
				if (value < 0)
				{
					Math.ThrowAbsOverflow();
				}
			}
			return value;
		}

		/// <summary>Returns the absolute value of a 64-bit signed integer.</summary>
		/// <param name="value">A number that is greater than <see cref="F:System.Int64.MinValue" />, but less than or equal to <see cref="F:System.Int64.MaxValue" />.</param>
		/// <returns>A 64-bit signed integer, x, such that 0 ≤ x ≤<see cref="F:System.Int64.MaxValue" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> equals <see cref="F:System.Int64.MinValue" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Abs(long value)
		{
			if (value < 0L)
			{
				value = -value;
				if (value < 0L)
				{
					Math.ThrowAbsOverflow();
				}
			}
			return value;
		}

		/// <summary>Returns the absolute value of an 8-bit signed integer.</summary>
		/// <param name="value">A number that is greater than <see cref="F:System.SByte.MinValue" />, but less than or equal to <see cref="F:System.SByte.MaxValue" />.</param>
		/// <returns>An 8-bit signed integer, x, such that 0 ≤ x ≤<see cref="F:System.SByte.MaxValue" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> equals <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Abs(sbyte value)
		{
			if (value < 0)
			{
				value = -value;
				if (value < 0)
				{
					Math.ThrowAbsOverflow();
				}
			}
			return value;
		}

		/// <summary>Returns the absolute value of a <see cref="T:System.Decimal" /> number.</summary>
		/// <param name="value">A number that is greater than or equal to <see cref="F:System.Decimal.MinValue" />, but less than or equal to <see cref="F:System.Decimal.MaxValue" />.</param>
		/// <returns>A decimal number, x, such that 0 ≤ x ≤<see cref="F:System.Decimal.MaxValue" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Abs(decimal value)
		{
			return decimal.Abs(ref value);
		}

		[StackTraceHidden]
		private static void ThrowAbsOverflow()
		{
			throw new OverflowException("Negating the minimum value of a twos complement number is invalid.");
		}

		/// <summary>Produces the full product of two 32-bit numbers.</summary>
		/// <param name="a">The first number to multiply.</param>
		/// <param name="b">The second number to multiply.</param>
		/// <returns>The number containing the product of the specified numbers.</returns>
		public static long BigMul(int a, int b)
		{
			return (long)a * (long)b;
		}

		/// <summary>Calculates the quotient of two 32-bit signed integers and also returns the remainder in an output parameter.</summary>
		/// <param name="a">The dividend.</param>
		/// <param name="b">The divisor.</param>
		/// <param name="result">The remainder.</param>
		/// <returns>The quotient of the specified numbers.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="b" /> is zero.</exception>
		public static int DivRem(int a, int b, out int result)
		{
			int num = a / b;
			result = a - num * b;
			return num;
		}

		/// <summary>Calculates the quotient of two 64-bit signed integers and also returns the remainder in an output parameter.</summary>
		/// <param name="a">The dividend.</param>
		/// <param name="b">The divisor.</param>
		/// <param name="result">The remainder.</param>
		/// <returns>The quotient of the specified numbers.</returns>
		/// <exception cref="T:System.DivideByZeroException">
		///   <paramref name="b" /> is zero.</exception>
		public static long DivRem(long a, long b, out long result)
		{
			long num = a / b;
			result = a - num * b;
			return num;
		}

		internal static uint DivRem(uint a, uint b, out uint result)
		{
			uint num = a / b;
			result = a - num * b;
			return num;
		}

		internal static ulong DivRem(ulong a, ulong b, out ulong result)
		{
			ulong num = a / b;
			result = a - num * b;
			return num;
		}

		/// <summary>Returns the smallest integral value that is greater than or equal to the specified decimal number.</summary>
		/// <param name="d">A decimal number.</param>
		/// <returns>The smallest integral value that is greater than or equal to <paramref name="d" />. Note that this method returns a <see cref="T:System.Decimal" /> instead of an integral type.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Ceiling(decimal d)
		{
			return decimal.Ceiling(d);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte Clamp(byte value, byte min, byte max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<byte>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Clamp(decimal value, decimal min, decimal max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<decimal>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp(double value, double min, double max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<double>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short Clamp(short value, short min, short max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<short>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Clamp(int value, int min, int max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<int>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long Clamp(long value, long min, long max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<long>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<sbyte>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Clamp(float value, float min, float max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<float>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort Clamp(ushort value, ushort min, ushort max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<ushort>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Clamp(uint value, uint min, uint max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<uint>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Clamp(ulong value, ulong min, ulong max)
		{
			if (min > max)
			{
				Math.ThrowMinMaxException<ulong>(min, max);
			}
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>Returns the largest integral value less than or equal to the specified decimal number.</summary>
		/// <param name="d">A decimal number.</param>
		/// <returns>The largest integral value less than or equal to <paramref name="d" />.  Note that the method returns an integral value of type <see cref="T:System.Decimal" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Floor(decimal d)
		{
			return decimal.Floor(d);
		}

		/// <summary>Returns the remainder resulting from the division of a specified number by another specified number.</summary>
		/// <param name="x">A dividend.</param>
		/// <param name="y">A divisor.</param>
		/// <returns>A number equal to <paramref name="x" /> - (<paramref name="y" /> Q), where Q is the quotient of <paramref name="x" /> / <paramref name="y" /> rounded to the nearest integer (if <paramref name="x" /> / <paramref name="y" /> falls halfway between two integers, the even integer is returned).  
		///  If <paramref name="x" /> - (<paramref name="y" /> Q) is zero, the value +0 is returned if <paramref name="x" /> is positive, or -0 if <paramref name="x" /> is negative.  
		///  If <paramref name="y" /> = 0, <see cref="F:System.Double.NaN" /> is returned.</returns>
		public static double IEEERemainder(double x, double y)
		{
			if (double.IsNaN(x))
			{
				return x;
			}
			if (double.IsNaN(y))
			{
				return y;
			}
			double num = x % y;
			if (double.IsNaN(num))
			{
				return double.NaN;
			}
			if (num == 0.0 && double.IsNegative(x))
			{
				return --0.0;
			}
			double num2 = num - Math.Abs(y) * (double)Math.Sign(x);
			if (Math.Abs(num2) == Math.Abs(num))
			{
				double num3 = x / y;
				if (Math.Abs(Math.Round(num3)) > Math.Abs(num3))
				{
					return num2;
				}
				return num;
			}
			else
			{
				if (Math.Abs(num2) < Math.Abs(num))
				{
					return num2;
				}
				return num;
			}
		}

		/// <summary>Returns the logarithm of a specified number in a specified base.</summary>
		/// <param name="a">The number whose logarithm is to be found.</param>
		/// <param name="newBase">The base of the logarithm.</param>
		/// <returns>One of the values in the following table. (+Infinity denotes <see cref="F:System.Double.PositiveInfinity" />, -Infinity denotes <see cref="F:System.Double.NegativeInfinity" />, and NaN denotes <see cref="F:System.Double.NaN" />.)  
		///  <paramref name="a" /><paramref name="newBase" /> Return value  
		///
		///  <paramref name="a" />&gt; 0  
		///
		///   (0 &lt;<paramref name="newBase" />&lt; 1) -or-(<paramref name="newBase" />&gt; 1)  
		///
		///   lognewBase(a)  
		///
		///  <paramref name="a" />&lt; 0  
		///
		///   (any value)  
		///
		///   NaN  
		///
		///   (any value)  
		///
		///  <paramref name="newBase" />&lt; 0  
		///
		///   NaN  
		///
		///  <paramref name="a" /> != 1  
		///
		///  <paramref name="newBase" /> = 0  
		///
		///   NaN  
		///
		///  <paramref name="a" /> != 1  
		///
		///  <paramref name="newBase" /> = +Infinity  
		///
		///   NaN  
		///
		///  <paramref name="a" /> = NaN  
		///
		///   (any value)  
		///
		///   NaN  
		///
		///   (any value)  
		///
		///  <paramref name="newBase" /> = NaN  
		///
		///   NaN  
		///
		///   (any value)  
		///
		///  <paramref name="newBase" /> = 1  
		///
		///   NaN  
		///
		///  <paramref name="a" /> = 0  
		///
		///   0 &lt;<paramref name="newBase" />&lt; 1  
		///
		///   +Infinity  
		///
		///  <paramref name="a" /> = 0  
		///
		///  <paramref name="newBase" />&gt; 1  
		///
		///   -Infinity  
		///
		///  <paramref name="a" /> =  +Infinity  
		///
		///   0 &lt;<paramref name="newBase" />&lt; 1  
		///
		///   -Infinity  
		///
		///  <paramref name="a" /> =  +Infinity  
		///
		///  <paramref name="newBase" />&gt; 1  
		///
		///   +Infinity  
		///
		///  <paramref name="a" /> = 1  
		///
		///  <paramref name="newBase" /> = 0  
		///
		///   0  
		///
		///  <paramref name="a" /> = 1  
		///
		///  <paramref name="newBase" /> = +Infinity  
		///
		///   0</returns>
		public static double Log(double a, double newBase)
		{
			if (double.IsNaN(a))
			{
				return a;
			}
			if (double.IsNaN(newBase))
			{
				return newBase;
			}
			if (newBase == 1.0)
			{
				return double.NaN;
			}
			if (a != 1.0 && (newBase == 0.0 || double.IsPositiveInfinity(newBase)))
			{
				return double.NaN;
			}
			return Math.Log(a) / Math.Log(newBase);
		}

		/// <summary>Returns the larger of two 8-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 8-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 8-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[NonVersionable]
		public static byte Max(byte val1, byte val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the larger of two decimal numbers.</summary>
		/// <param name="val1">The first of two decimal numbers to compare.</param>
		/// <param name="val2">The second of two decimal numbers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static decimal Max(decimal val1, decimal val2)
		{
			return *decimal.Max(ref val1, ref val2);
		}

		/// <summary>Returns the larger of two double-precision floating-point numbers.</summary>
		/// <param name="val1">The first of two double-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of two double-precision floating-point numbers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger. If <paramref name="val1" />, <paramref name="val2" />, or both <paramref name="val1" /> and <paramref name="val2" /> are equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NaN" /> is returned.</returns>
		public static double Max(double val1, double val2)
		{
			if (val1 > val2)
			{
				return val1;
			}
			if (double.IsNaN(val1))
			{
				return val1;
			}
			return val2;
		}

		/// <summary>Returns the larger of two 16-bit signed integers.</summary>
		/// <param name="val1">The first of two 16-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 16-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[NonVersionable]
		public static short Max(short val1, short val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the larger of two 32-bit signed integers.</summary>
		/// <param name="val1">The first of two 32-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 32-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[NonVersionable]
		public static int Max(int val1, int val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the larger of two 64-bit signed integers.</summary>
		/// <param name="val1">The first of two 64-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 64-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[NonVersionable]
		public static long Max(long val1, long val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the larger of two 8-bit signed integers.</summary>
		/// <param name="val1">The first of two 8-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 8-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[CLSCompliant(false)]
		[NonVersionable]
		public static sbyte Max(sbyte val1, sbyte val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the larger of two single-precision floating-point numbers.</summary>
		/// <param name="val1">The first of two single-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of two single-precision floating-point numbers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger. If <paramref name="val1" />, or <paramref name="val2" />, or both <paramref name="val1" /> and <paramref name="val2" /> are equal to <see cref="F:System.Single.NaN" />, <see cref="F:System.Single.NaN" /> is returned.</returns>
		public static float Max(float val1, float val2)
		{
			if (val1 > val2)
			{
				return val1;
			}
			if (float.IsNaN(val1))
			{
				return val1;
			}
			return val2;
		}

		/// <summary>Returns the larger of two 16-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 16-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 16-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[NonVersionable]
		[CLSCompliant(false)]
		public static ushort Max(ushort val1, ushort val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the larger of two 32-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 32-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 32-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[NonVersionable]
		[CLSCompliant(false)]
		public static uint Max(uint val1, uint val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the larger of two 64-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 64-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 64-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is larger.</returns>
		[NonVersionable]
		[CLSCompliant(false)]
		public static ulong Max(ulong val1, ulong val2)
		{
			if (val1 < val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two 8-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 8-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 8-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[NonVersionable]
		public static byte Min(byte val1, byte val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two decimal numbers.</summary>
		/// <param name="val1">The first of two decimal numbers to compare.</param>
		/// <param name="val2">The second of two decimal numbers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static decimal Min(decimal val1, decimal val2)
		{
			return *decimal.Min(ref val1, ref val2);
		}

		/// <summary>Returns the smaller of two double-precision floating-point numbers.</summary>
		/// <param name="val1">The first of two double-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of two double-precision floating-point numbers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller. If <paramref name="val1" />, <paramref name="val2" />, or both <paramref name="val1" /> and <paramref name="val2" /> are equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NaN" /> is returned.</returns>
		public static double Min(double val1, double val2)
		{
			if (val1 < val2)
			{
				return val1;
			}
			if (double.IsNaN(val1))
			{
				return val1;
			}
			return val2;
		}

		/// <summary>Returns the smaller of two 16-bit signed integers.</summary>
		/// <param name="val1">The first of two 16-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 16-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[NonVersionable]
		public static short Min(short val1, short val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two 32-bit signed integers.</summary>
		/// <param name="val1">The first of two 32-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 32-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[NonVersionable]
		public static int Min(int val1, int val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two 64-bit signed integers.</summary>
		/// <param name="val1">The first of two 64-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 64-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[NonVersionable]
		public static long Min(long val1, long val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two 8-bit signed integers.</summary>
		/// <param name="val1">The first of two 8-bit signed integers to compare.</param>
		/// <param name="val2">The second of two 8-bit signed integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[NonVersionable]
		[CLSCompliant(false)]
		public static sbyte Min(sbyte val1, sbyte val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two single-precision floating-point numbers.</summary>
		/// <param name="val1">The first of two single-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of two single-precision floating-point numbers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller. If <paramref name="val1" />, <paramref name="val2" />, or both <paramref name="val1" /> and <paramref name="val2" /> are equal to <see cref="F:System.Single.NaN" />, <see cref="F:System.Single.NaN" /> is returned.</returns>
		public static float Min(float val1, float val2)
		{
			if (val1 < val2)
			{
				return val1;
			}
			if (float.IsNaN(val1))
			{
				return val1;
			}
			return val2;
		}

		/// <summary>Returns the smaller of two 16-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 16-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 16-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[NonVersionable]
		[CLSCompliant(false)]
		public static ushort Min(ushort val1, ushort val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two 32-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 32-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 32-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[CLSCompliant(false)]
		[NonVersionable]
		public static uint Min(uint val1, uint val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Returns the smaller of two 64-bit unsigned integers.</summary>
		/// <param name="val1">The first of two 64-bit unsigned integers to compare.</param>
		/// <param name="val2">The second of two 64-bit unsigned integers to compare.</param>
		/// <returns>Parameter <paramref name="val1" /> or <paramref name="val2" />, whichever is smaller.</returns>
		[NonVersionable]
		[CLSCompliant(false)]
		public static ulong Min(ulong val1, ulong val2)
		{
			if (val1 > val2)
			{
				return val2;
			}
			return val1;
		}

		/// <summary>Rounds a decimal value to the nearest integral value, and rounds midpoint values to the nearest even number.</summary>
		/// <param name="d">A decimal number to be rounded.</param>
		/// <returns>The integer nearest the <paramref name="d" /> parameter. If the fractional component of <paramref name="d" /> is halfway between two integers, one of which is even and the other odd, the even number is returned. Note that this method returns a <see cref="T:System.Decimal" /> instead of an integral type.</returns>
		/// <exception cref="T:System.OverflowException">The result is outside the range of a <see cref="T:System.Decimal" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(decimal d)
		{
			return decimal.Round(d, 0);
		}

		/// <summary>Rounds a decimal value to a specified number of fractional digits, and rounds midpoint values to the nearest even number.</summary>
		/// <param name="d">A decimal number to be rounded.</param>
		/// <param name="decimals">The number of decimal places in the return value.</param>
		/// <returns>The number nearest to <paramref name="d" /> that contains a number of fractional digits equal to <paramref name="decimals" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="decimals" /> is less than 0 or greater than 28.</exception>
		/// <exception cref="T:System.OverflowException">The result is outside the range of a <see cref="T:System.Decimal" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(decimal d, int decimals)
		{
			return decimal.Round(d, decimals);
		}

		/// <summary>Rounds a decimal value to the nearest integer, and uses the specified rounding convention for midpoint values.</summary>
		/// <param name="d">A decimal number to be rounded.</param>
		/// <param name="mode">Specification for how to round <paramref name="d" /> if it is midway between two other numbers.</param>
		/// <returns>The integer nearest <paramref name="d" />. If <paramref name="d" /> is halfway between two numbers, one of which is even and the other odd, then <paramref name="mode" /> determines which of the two is returned. Note that this method returns a <see cref="T:System.Decimal" /> instead of an integral type.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="mode" /> is not a valid value of <see cref="T:System.MidpointRounding" />.</exception>
		/// <exception cref="T:System.OverflowException">The result is outside the range of a <see cref="T:System.Decimal" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(decimal d, MidpointRounding mode)
		{
			return decimal.Round(d, 0, mode);
		}

		/// <summary>Rounds a decimal value to a specified number of fractional digits, and uses the specified rounding convention for midpoint values.</summary>
		/// <param name="d">A decimal number to be rounded.</param>
		/// <param name="decimals">The number of decimal places in the return value.</param>
		/// <param name="mode">Specification for how to round <paramref name="d" /> if it is midway between two other numbers.</param>
		/// <returns>The number nearest to <paramref name="d" /> that contains a number of fractional digits equal to <paramref name="decimals" />. If <paramref name="d" /> has fewer fractional digits than <paramref name="decimals" />, <paramref name="d" /> is returned unchanged.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="decimals" /> is less than 0 or greater than 28.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="mode" /> is not a valid value of <see cref="T:System.MidpointRounding" />.</exception>
		/// <exception cref="T:System.OverflowException">The result is outside the range of a <see cref="T:System.Decimal" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Round(decimal d, int decimals, MidpointRounding mode)
		{
			return decimal.Round(d, decimals, mode);
		}

		/// <summary>Rounds a double-precision floating-point value to the nearest integral value, and rounds midpoint values to the nearest even number.</summary>
		/// <param name="a">A double-precision floating-point number to be rounded.</param>
		/// <returns>The integer nearest <paramref name="a" />. If the fractional component of <paramref name="a" /> is halfway between two integers, one of which is even and the other odd, then the even number is returned. Note that this method returns a <see cref="T:System.Double" /> instead of an integral type.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Round(double a);

		/// <summary>Rounds a double-precision floating-point value to a specified number of fractional digits, and rounds midpoint values to the nearest even number.</summary>
		/// <param name="value">A double-precision floating-point number to be rounded.</param>
		/// <param name="digits">The number of fractional digits in the return value.</param>
		/// <returns>The number nearest to <paramref name="value" /> that contains a number of fractional digits equal to <paramref name="digits" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="digits" /> is less than 0 or greater than 15.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(double value, int digits)
		{
			return Math.Round(value, digits, MidpointRounding.ToEven);
		}

		/// <summary>Rounds a double-precision floating-point value to the nearest integer, and uses the specified rounding convention for midpoint values.</summary>
		/// <param name="value">A double-precision floating-point number to be rounded.</param>
		/// <param name="mode">Specification for how to round <paramref name="value" /> if it is midway between two other numbers.</param>
		/// <returns>The integer nearest <paramref name="value" />. If <paramref name="value" /> is halfway between two integers, one of which is even and the other odd, then <paramref name="mode" /> determines which of the two is returned. Note that this method returns a <see cref="T:System.Double" /> instead of an integral type.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="mode" /> is not a valid value of <see cref="T:System.MidpointRounding" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Round(double value, MidpointRounding mode)
		{
			return Math.Round(value, 0, mode);
		}

		/// <summary>Rounds a double-precision floating-point value to a specified number of fractional digits, and uses the specified rounding convention for midpoint values.</summary>
		/// <param name="value">A double-precision floating-point number to be rounded.</param>
		/// <param name="digits">The number of fractional digits in the return value.</param>
		/// <param name="mode">Specification for how to round <paramref name="value" /> if it is midway between two other numbers.</param>
		/// <returns>The number nearest to <paramref name="value" /> that has a number of fractional digits equal to <paramref name="digits" />. If <paramref name="value" /> has fewer fractional digits than <paramref name="digits" />, <paramref name="value" /> is returned unchanged.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="digits" /> is less than 0 or greater than 15.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="mode" /> is not a valid value of <see cref="T:System.MidpointRounding" />.</exception>
		public unsafe static double Round(double value, int digits, MidpointRounding mode)
		{
			if (digits < 0 || digits > 15)
			{
				throw new ArgumentOutOfRangeException("digits", "Rounding digits must be between 0 and 15, inclusive.");
			}
			if (mode < MidpointRounding.ToEven || mode > MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException(SR.Format("The value '{0}' is not valid for this usage of the type {1}.", mode, "MidpointRounding"), "mode");
			}
			if (Math.Abs(value) < Math.doubleRoundLimit)
			{
				double num = Math.roundPower10Double[digits];
				value *= num;
				if (mode == MidpointRounding.AwayFromZero)
				{
					double value2 = Math.ModF(value, &value);
					if (Math.Abs(value2) >= 0.5)
					{
						value += (double)Math.Sign(value2);
					}
				}
				else
				{
					value = Math.Round(value);
				}
				value /= num;
			}
			return value;
		}

		/// <summary>Returns an integer that indicates the sign of a decimal number.</summary>
		/// <param name="value">A signed decimal number.</param>
		/// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.  
		///   Return value  
		///
		///   Meaning  
		///
		///   -1  
		///
		///  <paramref name="value" /> is less than zero.  
		///
		///   0  
		///
		///  <paramref name="value" /> is equal to zero.  
		///
		///   1  
		///
		///  <paramref name="value" /> is greater than zero.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(decimal value)
		{
			return decimal.Sign(ref value);
		}

		/// <summary>Returns an integer that indicates the sign of a double-precision floating-point number.</summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.  
		///   Return value  
		///
		///   Meaning  
		///
		///   -1  
		///
		///  <paramref name="value" /> is less than zero.  
		///
		///   0  
		///
		///  <paramref name="value" /> is equal to zero.  
		///
		///   1  
		///
		///  <paramref name="value" /> is greater than zero.</returns>
		/// <exception cref="T:System.ArithmeticException">
		///   <paramref name="value" /> is equal to <see cref="F:System.Double.NaN" />.</exception>
		public static int Sign(double value)
		{
			if (value < 0.0)
			{
				return -1;
			}
			if (value > 0.0)
			{
				return 1;
			}
			if (value == 0.0)
			{
				return 0;
			}
			throw new ArithmeticException("Function does not accept floating point Not-a-Number values.");
		}

		/// <summary>Returns an integer that indicates the sign of a 16-bit signed integer.</summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.  
		///   Return value  
		///
		///   Meaning  
		///
		///   -1  
		///
		///  <paramref name="value" /> is less than zero.  
		///
		///   0  
		///
		///  <paramref name="value" /> is equal to zero.  
		///
		///   1  
		///
		///  <paramref name="value" /> is greater than zero.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(short value)
		{
			return Math.Sign((int)value);
		}

		/// <summary>Returns an integer that indicates the sign of a 32-bit signed integer.</summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.  
		///   Return value  
		///
		///   Meaning  
		///
		///   -1  
		///
		///  <paramref name="value" /> is less than zero.  
		///
		///   0  
		///
		///  <paramref name="value" /> is equal to zero.  
		///
		///   1  
		///
		///  <paramref name="value" /> is greater than zero.</returns>
		public static int Sign(int value)
		{
			return value >> 31 | (int)((uint)(-(uint)value) >> 31);
		}

		/// <summary>Returns an integer that indicates the sign of a 64-bit signed integer.</summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.  
		///   Return value  
		///
		///   Meaning  
		///
		///   -1  
		///
		///  <paramref name="value" /> is less than zero.  
		///
		///   0  
		///
		///  <paramref name="value" /> is equal to zero.  
		///
		///   1  
		///
		///  <paramref name="value" /> is greater than zero.</returns>
		public static int Sign(long value)
		{
			return (int)(value >> 63 | (long)((ulong)(-(ulong)value) >> 63));
		}

		/// <summary>Returns an integer that indicates the sign of an 8-bit signed integer.</summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.  
		///   Return value  
		///
		///   Meaning  
		///
		///   -1  
		///
		///  <paramref name="value" /> is less than zero.  
		///
		///   0  
		///
		///  <paramref name="value" /> is equal to zero.  
		///
		///   1  
		///
		///  <paramref name="value" /> is greater than zero.</returns>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Sign(sbyte value)
		{
			return Math.Sign((int)value);
		}

		/// <summary>Returns an integer that indicates the sign of a single-precision floating-point number.</summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A number that indicates the sign of <paramref name="value" />, as shown in the following table.  
		///   Return value  
		///
		///   Meaning  
		///
		///   -1  
		///
		///  <paramref name="value" /> is less than zero.  
		///
		///   0  
		///
		///  <paramref name="value" /> is equal to zero.  
		///
		///   1  
		///
		///  <paramref name="value" /> is greater than zero.</returns>
		/// <exception cref="T:System.ArithmeticException">
		///   <paramref name="value" /> is equal to <see cref="F:System.Single.NaN" />.</exception>
		public static int Sign(float value)
		{
			if (value < 0f)
			{
				return -1;
			}
			if (value > 0f)
			{
				return 1;
			}
			if (value == 0f)
			{
				return 0;
			}
			throw new ArithmeticException("Function does not accept floating point Not-a-Number values.");
		}

		/// <summary>Calculates the integral part of a specified decimal number.</summary>
		/// <param name="d">A number to truncate.</param>
		/// <returns>The integral part of <paramref name="d" />; that is, the number that remains after any fractional digits have been discarded.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal Truncate(decimal d)
		{
			return decimal.Truncate(d);
		}

		/// <summary>Calculates the integral part of a specified double-precision floating-point number.</summary>
		/// <param name="d">A number to truncate.</param>
		/// <returns>The integral part of <paramref name="d" />; that is, the number that remains after any fractional digits have been discarded, or one of the values listed in the following table.  
		///  <paramref name="d" /> Return value  
		///
		///  <see cref="F:System.Double.NaN" /><see cref="F:System.Double.NaN" /><see cref="F:System.Double.NegativeInfinity" /><see cref="F:System.Double.NegativeInfinity" /><see cref="F:System.Double.PositiveInfinity" /><see cref="F:System.Double.PositiveInfinity" /></returns>
		public unsafe static double Truncate(double d)
		{
			Math.ModF(d, &d);
			return d;
		}

		private static double copysign(double x, double y)
		{
			long num = BitConverter.DoubleToInt64Bits(x);
			long num2 = BitConverter.DoubleToInt64Bits(y);
			if ((num ^ num2) >> 63 != 0L)
			{
				return BitConverter.Int64BitsToDouble(num ^ long.MinValue);
			}
			return x;
		}

		private static void ThrowMinMaxException<T>(T min, T max)
		{
			throw new ArgumentException(SR.Format("'{0}' cannot be greater than {1}.", min, max));
		}

		/// <summary>Returns the absolute value of a double-precision floating-point number.</summary>
		/// <param name="value">A number that is greater than or equal to <see cref="F:System.Double.MinValue" />, but less than or equal to <see cref="F:System.Double.MaxValue" />.</param>
		/// <returns>A double-precision floating-point number, x, such that 0 ≤ x ≤<see cref="F:System.Double.MaxValue" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Abs(double value);

		/// <summary>Returns the absolute value of a single-precision floating-point number.</summary>
		/// <param name="value">A number that is greater than or equal to <see cref="F:System.Single.MinValue" />, but less than or equal to <see cref="F:System.Single.MaxValue" />.</param>
		/// <returns>A single-precision floating-point number, x, such that 0 ≤ x ≤<see cref="F:System.Single.MaxValue" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float Abs(float value);

		/// <summary>Returns the angle whose cosine is the specified number.</summary>
		/// <param name="d">A number representing a cosine, where <paramref name="d" /> must be greater than or equal to -1, but less than or equal to 1.</param>
		/// <returns>An angle, θ, measured in radians, such that 0 ≤θ≤π  
		///  -or-  
		///  <see cref="F:System.Double.NaN" /> if <paramref name="d" /> &lt; -1 or <paramref name="d" /> &gt; 1 or <paramref name="d" /> equals <see cref="F:System.Double.NaN" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Acos(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Acosh(double d);

		/// <summary>Returns the angle whose sine is the specified number.</summary>
		/// <param name="d">A number representing a sine, where <paramref name="d" /> must be greater than or equal to -1, but less than or equal to 1.</param>
		/// <returns>An angle, θ, measured in radians, such that -π/2 ≤θ≤π/2  
		///  -or-  
		///  <see cref="F:System.Double.NaN" /> if <paramref name="d" /> &lt; -1 or <paramref name="d" /> &gt; 1 or <paramref name="d" /> equals <see cref="F:System.Double.NaN" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Asin(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Asinh(double d);

		/// <summary>Returns the angle whose tangent is the specified number.</summary>
		/// <param name="d">A number representing a tangent.</param>
		/// <returns>An angle, θ, measured in radians, such that -π/2 ≤θ≤π/2.  
		///  -or-  
		///  <see cref="F:System.Double.NaN" /> if <paramref name="d" /> equals <see cref="F:System.Double.NaN" />, -π/2 rounded to double precision (-1.5707963267949) if <paramref name="d" /> equals <see cref="F:System.Double.NegativeInfinity" />, or π/2 rounded to double precision (1.5707963267949) if <paramref name="d" /> equals <see cref="F:System.Double.PositiveInfinity" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Atan(double d);

		/// <summary>Returns the angle whose tangent is the quotient of two specified numbers.</summary>
		/// <param name="y">The y coordinate of a point.</param>
		/// <param name="x">The x coordinate of a point.</param>
		/// <returns>An angle, θ, measured in radians, such that -π≤θ≤π, and tan(θ) = <paramref name="y" /> / <paramref name="x" />, where (<paramref name="x" />, <paramref name="y" />) is a point in the Cartesian plane. Observe the following:  
		///
		/// For (<paramref name="x" />, <paramref name="y" />) in quadrant 1, 0 &lt; θ &lt; π/2.  
		///
		/// For (<paramref name="x" />, <paramref name="y" />) in quadrant 2, π/2 &lt; θ≤π.  
		///
		/// For (<paramref name="x" />, <paramref name="y" />) in quadrant 3, -π &lt; θ &lt; -π/2.  
		///
		/// For (<paramref name="x" />, <paramref name="y" />) in quadrant 4, -π/2 &lt; θ &lt; 0.  
		///
		///
		///  For points on the boundaries of the quadrants, the return value is the following:  
		///
		/// If y is 0 and x is not negative, θ = 0.  
		///
		/// If y is 0 and x is negative, θ = π.  
		///
		/// If y is positive and x is 0, θ = π/2.  
		///
		/// If y is negative and x is 0, θ = -π/2.  
		///
		/// If y is 0 and x is 0, θ = 0.  
		///
		///
		///  If <paramref name="x" /> or <paramref name="y" /> is <see cref="F:System.Double.NaN" />, or if <paramref name="x" /> and <paramref name="y" /> are either <see cref="F:System.Double.PositiveInfinity" /> or <see cref="F:System.Double.NegativeInfinity" />, the method returns <see cref="F:System.Double.NaN" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Atan2(double y, double x);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Atanh(double d);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Cbrt(double d);

		/// <summary>Returns the smallest integral value that is greater than or equal to the specified double-precision floating-point number.</summary>
		/// <param name="a">A double-precision floating-point number.</param>
		/// <returns>The smallest integral value that is greater than or equal to <paramref name="a" />. If <paramref name="a" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, that value is returned. Note that this method returns a <see cref="T:System.Double" /> instead of an integral type.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Ceiling(double a);

		/// <summary>Returns the cosine of the specified angle.</summary>
		/// <param name="d">An angle, measured in radians.</param>
		/// <returns>The cosine of <paramref name="d" />. If <paramref name="d" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, this method returns <see cref="F:System.Double.NaN" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Cos(double d);

		/// <summary>Returns the hyperbolic cosine of the specified angle.</summary>
		/// <param name="value">An angle, measured in radians.</param>
		/// <returns>The hyperbolic cosine of <paramref name="value" />. If <paramref name="value" /> is equal to <see cref="F:System.Double.NegativeInfinity" /> or <see cref="F:System.Double.PositiveInfinity" />, <see cref="F:System.Double.PositiveInfinity" /> is returned. If <paramref name="value" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NaN" /> is returned.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Cosh(double value);

		/// <summary>Returns <see langword="e" /> raised to the specified power.</summary>
		/// <param name="d">A number specifying a power.</param>
		/// <returns>The number <see langword="e" /> raised to the power <paramref name="d" />. If <paramref name="d" /> equals <see cref="F:System.Double.NaN" /> or <see cref="F:System.Double.PositiveInfinity" />, that value is returned. If <paramref name="d" /> equals <see cref="F:System.Double.NegativeInfinity" />, 0 is returned.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Exp(double d);

		/// <summary>Returns the largest integral value less than or equal to the specified double-precision floating-point number.</summary>
		/// <param name="d">A double-precision floating-point number.</param>
		/// <returns>The largest integral value less than or equal to <paramref name="d" />. If <paramref name="d" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, that value is returned.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Floor(double d);

		/// <summary>Returns the natural (base <see langword="e" />) logarithm of a specified number.</summary>
		/// <param name="d">The number whose logarithm is to be found.</param>
		/// <returns>One of the values in the following table.  
		///  <paramref name="d" /> parameter  
		///
		///   Return value  
		///
		///   Positive  
		///
		///   The natural logarithm of <paramref name="d" />; that is, ln <paramref name="d" />, or log e <paramref name="d" /> Zero  
		///
		///  <see cref="F:System.Double.NegativeInfinity" /> Negative  
		///
		///  <see cref="F:System.Double.NaN" /> Equal to <see cref="F:System.Double.NaN" /><see cref="F:System.Double.NaN" /> Equal to <see cref="F:System.Double.PositiveInfinity" /><see cref="F:System.Double.PositiveInfinity" /></returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Log(double d);

		/// <summary>Returns the base 10 logarithm of a specified number.</summary>
		/// <param name="d">A number whose logarithm is to be found.</param>
		/// <returns>One of the values in the following table.  
		///  <paramref name="d" /> parameter  
		///
		///   Return value  
		///
		///   Positive  
		///
		///   The base 10 log of <paramref name="d" />; that is, log 10<paramref name="d" />.  
		///
		///   Zero  
		///
		///  <see cref="F:System.Double.NegativeInfinity" /> Negative  
		///
		///  <see cref="F:System.Double.NaN" /> Equal to <see cref="F:System.Double.NaN" /><see cref="F:System.Double.NaN" /> Equal to <see cref="F:System.Double.PositiveInfinity" /><see cref="F:System.Double.PositiveInfinity" /></returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Log10(double d);

		/// <summary>Returns a specified number raised to the specified power.</summary>
		/// <param name="x">A double-precision floating-point number to be raised to a power.</param>
		/// <param name="y">A double-precision floating-point number that specifies a power.</param>
		/// <returns>The number <paramref name="x" /> raised to the power <paramref name="y" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Pow(double x, double y);

		/// <summary>Returns the sine of the specified angle.</summary>
		/// <param name="a">An angle, measured in radians.</param>
		/// <returns>The sine of <paramref name="a" />. If <paramref name="a" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, this method returns <see cref="F:System.Double.NaN" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Sin(double a);

		/// <summary>Returns the hyperbolic sine of the specified angle.</summary>
		/// <param name="value">An angle, measured in radians.</param>
		/// <returns>The hyperbolic sine of <paramref name="value" />. If <paramref name="value" /> is equal to <see cref="F:System.Double.NegativeInfinity" />, <see cref="F:System.Double.PositiveInfinity" />, or <see cref="F:System.Double.NaN" />, this method returns a <see cref="T:System.Double" /> equal to <paramref name="value" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Sinh(double value);

		/// <summary>Returns the square root of a specified number.</summary>
		/// <param name="d">The number whose square root is to be found.</param>
		/// <returns>One of the values in the following table.  
		///  <paramref name="d" /> parameter  
		///
		///   Return value  
		///
		///   Zero or positive  
		///
		///   The positive square root of <paramref name="d" />.  
		///
		///   Negative  
		///
		///  <see cref="F:System.Double.NaN" /> Equals <see cref="F:System.Double.NaN" /><see cref="F:System.Double.NaN" /> Equals <see cref="F:System.Double.PositiveInfinity" /><see cref="F:System.Double.PositiveInfinity" /></returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Sqrt(double d);

		/// <summary>Returns the tangent of the specified angle.</summary>
		/// <param name="a">An angle, measured in radians.</param>
		/// <returns>The tangent of <paramref name="a" />. If <paramref name="a" /> is equal to <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.NegativeInfinity" />, or <see cref="F:System.Double.PositiveInfinity" />, this method returns <see cref="F:System.Double.NaN" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Tan(double a);

		/// <summary>Returns the hyperbolic tangent of the specified angle.</summary>
		/// <param name="value">An angle, measured in radians.</param>
		/// <returns>The hyperbolic tangent of <paramref name="value" />. If <paramref name="value" /> is equal to <see cref="F:System.Double.NegativeInfinity" />, this method returns -1. If value is equal to <see cref="F:System.Double.PositiveInfinity" />, this method returns 1. If <paramref name="value" /> is equal to <see cref="F:System.Double.NaN" />, this method returns <see cref="F:System.Double.NaN" />.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double Tanh(double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern double FMod(double x, double y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern double ModF(double x, double* intptr);

		/// <summary>Represents the natural logarithmic base, specified by the constant, <see langword="e" />.</summary>
		public const double E = 2.718281828459045;

		/// <summary>Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.</summary>
		public const double PI = 3.141592653589793;

		private const int maxRoundingDigits = 15;

		private static double doubleRoundLimit = 10000000000000000.0;

		private static double[] roundPower10Double = new double[]
		{
			1.0,
			10.0,
			100.0,
			1000.0,
			10000.0,
			100000.0,
			1000000.0,
			10000000.0,
			100000000.0,
			1000000000.0,
			10000000000.0,
			100000000000.0,
			1000000000000.0,
			10000000000000.0,
			100000000000000.0,
			1000000000000000.0
		};
	}
}
