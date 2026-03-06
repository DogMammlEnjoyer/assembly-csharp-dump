using System;
using System.Buffers.Text;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	internal static class Number
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsNegative(double d)
		{
			return BitConverter.DoubleToInt64Bits(d) < 0L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNegativeInfinity(float f)
		{
			return f == float.NegativeInfinity;
		}

		public unsafe static void Dragon4Double(double value, int cutoffNumber, bool isSignificantDigits, ref Number.NumberBuffer number)
		{
			Number.IsNegative(value);
			int exponent;
			ulong num = Number.ExtractFractionAndBiasedExponent(value, out exponent);
			bool hasUnequalMargins = false;
			uint mantissaHighBitIdx;
			if (num >> 52 != 0UL)
			{
				mantissaHighBitIdx = 52U;
				hasUnequalMargins = (num == 4503599627370496UL);
			}
			else
			{
				mantissaHighBitIdx = (uint)BitOperations.Log2(num);
			}
			int num3;
			int num2 = (int)Number.Dragon4(num, exponent, mantissaHighBitIdx, hasUnequalMargins, cutoffNumber, isSignificantDigits, number.Digits, out num3);
			number.Scale = num3 + 1;
			*number.Digits[num2] = 0;
			number.DigitsCount = num2;
		}

		public unsafe static void Dragon4Single(float value, int cutoffNumber, bool isSignificantDigits, ref Number.NumberBuffer number)
		{
			Number.IsNegative((double)value);
			int exponent;
			uint num = Number.ExtractFractionAndBiasedExponent(value, out exponent);
			bool hasUnequalMargins = false;
			uint mantissaHighBitIdx;
			if (num >> 23 != 0U)
			{
				mantissaHighBitIdx = 23U;
				hasUnequalMargins = (num == 8388608U);
			}
			else
			{
				mantissaHighBitIdx = (uint)BitOperations.Log2(num);
			}
			int num3;
			int num2 = (int)Number.Dragon4((ulong)num, exponent, mantissaHighBitIdx, hasUnequalMargins, cutoffNumber, isSignificantDigits, number.Digits, out num3);
			number.Scale = num3 + 1;
			*number.Digits[num2] = 0;
			number.DigitsCount = num2;
		}

		private unsafe static uint Dragon4(ulong mantissa, int exponent, uint mantissaHighBitIdx, bool hasUnequalMargins, int cutoffNumber, bool isSignificantDigits, Span<byte> buffer, out int decimalExponent)
		{
			int num = 0;
			Number.BigInteger bigInteger;
			Number.BigInteger bigInteger2;
			Number.BigInteger bigInteger3;
			Number.BigInteger* ptr;
			if (hasUnequalMargins)
			{
				Number.BigInteger bigInteger4;
				if (exponent > 0)
				{
					Number.BigInteger.SetUInt64(out bigInteger, 4UL * mantissa);
					bigInteger.ShiftLeft((uint)exponent);
					Number.BigInteger.SetUInt32(out bigInteger2, 4U);
					Number.BigInteger.Pow2((uint)exponent, out bigInteger3);
					Number.BigInteger.Pow2((uint)(exponent + 1), out bigInteger4);
				}
				else
				{
					Number.BigInteger.SetUInt64(out bigInteger, 4UL * mantissa);
					Number.BigInteger.Pow2((uint)(-exponent + 2), out bigInteger2);
					Number.BigInteger.SetUInt32(out bigInteger3, 1U);
					Number.BigInteger.SetUInt32(out bigInteger4, 2U);
				}
				ptr = &bigInteger4;
			}
			else
			{
				if (exponent > 0)
				{
					Number.BigInteger.SetUInt64(out bigInteger, 2UL * mantissa);
					bigInteger.ShiftLeft((uint)exponent);
					Number.BigInteger.SetUInt32(out bigInteger2, 2U);
					Number.BigInteger.Pow2((uint)exponent, out bigInteger3);
				}
				else
				{
					Number.BigInteger.SetUInt64(out bigInteger, 2UL * mantissa);
					Number.BigInteger.Pow2((uint)(-exponent + 1), out bigInteger2);
					Number.BigInteger.SetUInt32(out bigInteger3, 1U);
				}
				ptr = &bigInteger3;
			}
			int num2 = (int)Math.Ceiling((double)(mantissaHighBitIdx + (uint)exponent) * 0.3010299956639812 - 0.69);
			if (num2 > 0)
			{
				bigInteger2.MultiplyPow10((uint)num2);
			}
			else if (num2 < 0)
			{
				Number.BigInteger bigInteger5;
				Number.BigInteger.Pow10((uint)(-(uint)num2), out bigInteger5);
				bigInteger.Multiply(ref bigInteger5);
				bigInteger3.Multiply(ref bigInteger5);
				if (ptr != &bigInteger3)
				{
					Number.BigInteger.Multiply(ref bigInteger3, 2U, out *ptr);
				}
			}
			bool flag = mantissa % 2UL == 0UL;
			bool flag2;
			if (cutoffNumber == -1)
			{
				Number.BigInteger bigInteger6;
				Number.BigInteger.Add(ref bigInteger, ref *ptr, out bigInteger6);
				int num3 = Number.BigInteger.Compare(ref bigInteger6, ref bigInteger2);
				flag2 = (flag ? (num3 >= 0) : (num3 > 0));
			}
			else
			{
				flag2 = (Number.BigInteger.Compare(ref bigInteger, ref bigInteger2) >= 0);
			}
			if (flag2)
			{
				num2++;
			}
			else
			{
				bigInteger.Multiply10();
				bigInteger3.Multiply10();
				if (ptr != &bigInteger3)
				{
					Number.BigInteger.Multiply(ref bigInteger3, 2U, out *ptr);
				}
			}
			int num4 = num2 - buffer.Length;
			if (cutoffNumber != -1)
			{
				int num5;
				if (isSignificantDigits)
				{
					num5 = num2 - cutoffNumber;
				}
				else
				{
					num5 = -cutoffNumber;
				}
				if (num5 > num4)
				{
					num4 = num5;
				}
			}
			num2 = (decimalExponent = num2 - 1);
			uint block = bigInteger2.GetBlock((uint)(bigInteger2.GetLength() - 1));
			if (block < 8U || block > 429496729U)
			{
				uint num6 = (uint)BitOperations.Log2(block);
				uint shift = (59U - num6) % 32U;
				bigInteger2.ShiftLeft(shift);
				bigInteger.ShiftLeft(shift);
				bigInteger3.ShiftLeft(shift);
				if (ptr != &bigInteger3)
				{
					Number.BigInteger.Multiply(ref bigInteger3, 2U, out *ptr);
				}
			}
			uint num7;
			bool flag3;
			bool flag4;
			if (cutoffNumber == -1)
			{
				for (;;)
				{
					num7 = Number.BigInteger.HeuristicDivide(ref bigInteger, ref bigInteger2);
					Number.BigInteger bigInteger7;
					Number.BigInteger.Add(ref bigInteger, ref *ptr, out bigInteger7);
					int num8 = Number.BigInteger.Compare(ref bigInteger, ref bigInteger3);
					int num9 = Number.BigInteger.Compare(ref bigInteger7, ref bigInteger2);
					if (flag)
					{
						flag3 = (num8 <= 0);
						flag4 = (num9 >= 0);
					}
					else
					{
						flag3 = (num8 < 0);
						flag4 = (num9 > 0);
					}
					if (flag3 || flag4 || num2 == num4)
					{
						break;
					}
					*buffer[num] = (byte)(48U + num7);
					num++;
					bigInteger.Multiply10();
					bigInteger3.Multiply10();
					if (ptr != &bigInteger3)
					{
						Number.BigInteger.Multiply(ref bigInteger3, 2U, out *ptr);
					}
					num2--;
				}
			}
			else
			{
				if (num2 < num4)
				{
					num7 = Number.BigInteger.HeuristicDivide(ref bigInteger, ref bigInteger2);
					if (num7 > 5U || (num7 == 5U && !bigInteger.IsZero()))
					{
						decimalExponent++;
						num7 = 1U;
					}
					*buffer[num] = (byte)(48U + num7);
					return (uint)(num + 1);
				}
				flag3 = false;
				flag4 = false;
				for (;;)
				{
					num7 = Number.BigInteger.HeuristicDivide(ref bigInteger, ref bigInteger2);
					if (bigInteger.IsZero() || num2 <= num4)
					{
						break;
					}
					*buffer[num] = (byte)(48U + num7);
					num++;
					bigInteger.Multiply10();
					num2--;
				}
			}
			bool flag5 = flag3;
			if (flag3 == flag4)
			{
				bigInteger.ShiftLeft(1U);
				int num10 = Number.BigInteger.Compare(ref bigInteger, ref bigInteger2);
				flag5 = (num10 < 0);
				if (num10 == 0)
				{
					flag5 = ((num7 & 1U) == 0U);
				}
			}
			if (flag5)
			{
				*buffer[num] = (byte)(48U + num7);
				num++;
			}
			else if (num7 == 9U)
			{
				while (num != 0)
				{
					num--;
					if (*buffer[num] != 57)
					{
						ref byte ptr2 = ref buffer[num];
						ptr2 += 1;
						return (uint)(num + 1);
					}
				}
				*buffer[num] = 49;
				num++;
				decimalExponent++;
			}
			else
			{
				*buffer[num] = (byte)(48U + num7 + 1U);
				num++;
			}
			return (uint)num;
		}

		public unsafe static string FormatDecimal(decimal value, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			int nMaxDigits;
			char c = Number.ParseFormatSpecifier(format, out nMaxDigits);
			byte* digits = stackalloc byte[(UIntPtr)31];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Decimal, digits, 31);
			Number.DecimalToNumber(ref value, ref numberBuffer);
			char* pointer = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
			if (c != '\0')
			{
				Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, nMaxDigits, info);
			}
			else
			{
				Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, info);
			}
			return valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatDecimal(decimal value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
		{
			int nMaxDigits;
			char c = Number.ParseFormatSpecifier(format, out nMaxDigits);
			byte* digits = stackalloc byte[(UIntPtr)31];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Decimal, digits, 31);
			Number.DecimalToNumber(ref value, ref numberBuffer);
			char* pointer = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
			if (c != '\0')
			{
				Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, nMaxDigits, info);
			}
			else
			{
				Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, info);
			}
			return valueStringBuilder.TryCopyTo(destination, out charsWritten);
		}

		internal unsafe static void DecimalToNumber(ref decimal d, ref Number.NumberBuffer number)
		{
			byte* digitsPointer = number.GetDigitsPointer();
			number.DigitsCount = 29;
			number.IsNegative = d.IsNegative();
			byte* ptr = digitsPointer + 29;
			while ((d.Mid() | d.High()) != 0U)
			{
				ptr = Number.UInt32ToDecChars(ptr, DecimalEx.DecDivMod1E9(ref d), 9);
			}
			ptr = Number.UInt32ToDecChars(ptr, d.Low(), 0);
			int num = (int)((long)(digitsPointer + 29 - ptr));
			number.DigitsCount = num;
			number.Scale = num - d.Scale();
			byte* digitsPointer2 = number.GetDigitsPointer();
			while (--num >= 0)
			{
				*(digitsPointer2++) = *(ptr++);
			}
			*digitsPointer2 = 0;
		}

		public unsafe static string FormatDouble(double value, string format, NumberFormatInfo info)
		{
			Span<char> initialBuffer = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
			return Number.FormatDouble(ref valueStringBuilder, value, format.AsSpan(), info) ?? valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatDouble(double value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
		{
			Span<char> initialBuffer = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
			string text = Number.FormatDouble(ref valueStringBuilder, value, format, info);
			if (text == null)
			{
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
			return Number.TryCopyTo(text, destination, out charsWritten);
		}

		private static int GetFloatingPointMaxDigitsAndPrecision(char fmt, ref int precision, NumberFormatInfo info, out bool isSignificantDigits)
		{
			if (fmt == '\0')
			{
				isSignificantDigits = true;
				return precision;
			}
			int result = precision;
			if (fmt <= 'R')
			{
				switch (fmt)
				{
				case 'C':
					break;
				case 'D':
					goto IL_EF;
				case 'E':
					goto IL_9E;
				case 'F':
					goto IL_B1;
				case 'G':
					goto IL_C3;
				default:
					switch (fmt)
					{
					case 'N':
						goto IL_B1;
					case 'O':
					case 'Q':
						goto IL_EF;
					case 'P':
						goto IL_CF;
					case 'R':
						goto IL_E7;
					default:
						goto IL_EF;
					}
					break;
				}
			}
			else
			{
				switch (fmt)
				{
				case 'c':
					break;
				case 'd':
					goto IL_EF;
				case 'e':
					goto IL_9E;
				case 'f':
					goto IL_B1;
				case 'g':
					goto IL_C3;
				default:
					switch (fmt)
					{
					case 'n':
						goto IL_B1;
					case 'o':
					case 'q':
						goto IL_EF;
					case 'p':
						goto IL_CF;
					case 'r':
						goto IL_E7;
					default:
						goto IL_EF;
					}
					break;
				}
			}
			if (precision == -1)
			{
				precision = info.CurrencyDecimalDigits;
			}
			isSignificantDigits = false;
			return result;
			IL_9E:
			if (precision == -1)
			{
				precision = 6;
			}
			precision++;
			isSignificantDigits = true;
			return result;
			IL_B1:
			if (precision == -1)
			{
				precision = info.NumberDecimalDigits;
			}
			isSignificantDigits = false;
			return result;
			IL_C3:
			if (precision == 0)
			{
				precision = -1;
			}
			isSignificantDigits = true;
			return result;
			IL_CF:
			if (precision == -1)
			{
				precision = info.PercentDecimalDigits;
			}
			precision += 2;
			isSignificantDigits = false;
			return result;
			IL_E7:
			precision = -1;
			isSignificantDigits = true;
			return result;
			IL_EF:
			throw new FormatException("SR.Argument_BadFormatSpecifier");
		}

		private unsafe static string FormatDouble(ref ValueStringBuilder sb, double value, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			if (FloatEx.IsFinite(value))
			{
				int num;
				char c = Number.ParseFormatSpecifier(format, out num);
				byte* digits = stackalloc byte[(UIntPtr)769];
				if (c == '\0')
				{
					num = 15;
				}
				Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, digits, 769);
				numberBuffer.IsNegative = FloatEx.IsNegative(value);
				bool flag;
				int nMaxDigits = Number.GetFloatingPointMaxDigitsAndPrecision(c, ref num, info, out flag);
				if (value != 0.0 && (!flag || !Number.Grisu3.TryRunDouble(value, num, ref numberBuffer)))
				{
					Number.Dragon4Double(value, num, flag, ref numberBuffer);
				}
				if (c != '\0')
				{
					if (num == -1)
					{
						nMaxDigits = Math.Max(numberBuffer.DigitsCount, 17);
					}
					Number.NumberToString(ref sb, ref numberBuffer, c, nMaxDigits, info);
				}
				else
				{
					Number.NumberToStringFormat(ref sb, ref numberBuffer, format, info);
				}
				return null;
			}
			if (double.IsNaN(value))
			{
				return info.NaNSymbol;
			}
			if (!FloatEx.IsNegative(value))
			{
				return info.PositiveInfinitySymbol;
			}
			return info.NegativeInfinitySymbol;
		}

		public unsafe static string FormatSingle(float value, string format, NumberFormatInfo info)
		{
			Span<char> initialBuffer = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
			return Number.FormatSingle(ref valueStringBuilder, value, format.AsSpan(), info) ?? valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatSingle(float value, ReadOnlySpan<char> format, NumberFormatInfo info, Span<char> destination, out int charsWritten)
		{
			Span<char> initialBuffer = new Span<char>(stackalloc byte[(UIntPtr)64], 32);
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
			string text = Number.FormatSingle(ref valueStringBuilder, value, format, info);
			if (text == null)
			{
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
			return Number.TryCopyTo(text, destination, out charsWritten);
		}

		private unsafe static string FormatSingle(ref ValueStringBuilder sb, float value, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			if (FloatEx.IsFinite(value))
			{
				int num;
				char c = Number.ParseFormatSpecifier(format, out num);
				byte* digits = stackalloc byte[(UIntPtr)114];
				if (c == '\0')
				{
					num = 7;
				}
				Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, digits, 114);
				numberBuffer.IsNegative = FloatEx.IsNegative(value);
				bool flag;
				int nMaxDigits = Number.GetFloatingPointMaxDigitsAndPrecision(c, ref num, info, out flag);
				if (value != 0f && (!flag || !Number.Grisu3.TryRunSingle(value, num, ref numberBuffer)))
				{
					Number.Dragon4Single(value, num, flag, ref numberBuffer);
				}
				if (c != '\0')
				{
					if (num == -1)
					{
						nMaxDigits = Math.Max(numberBuffer.DigitsCount, 9);
					}
					Number.NumberToString(ref sb, ref numberBuffer, c, nMaxDigits, info);
				}
				else
				{
					Number.NumberToStringFormat(ref sb, ref numberBuffer, format, info);
				}
				return null;
			}
			if (float.IsNaN(value))
			{
				return info.NaNSymbol;
			}
			if (!FloatEx.IsNegative(value))
			{
				return info.PositiveInfinitySymbol;
			}
			return info.NegativeInfinitySymbol;
		}

		private static bool TryCopyTo(string source, Span<char> destination, out int charsWritten)
		{
			if (source.AsSpan().TryCopyTo(destination))
			{
				charsWritten = source.Length;
				return true;
			}
			charsWritten = 0;
			return false;
		}

		public unsafe static string FormatInt32(int value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (value >= 0 && format.Length == 0)
			{
				return Number.UInt32ToDecStr((uint)value, -1);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0)
				{
					return Number.NegativeInt32ToDecStr(value, num, NumberFormatInfo.GetInstance(provider).NegativeSign);
				}
				return Number.UInt32ToDecStr((uint)value, num);
			}
			else
			{
				if (c2 == 'X')
				{
					return Number.Int32ToHexStr(value, c - '!', num);
				}
				NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
				byte* digits = stackalloc byte[(UIntPtr)11];
				Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 11);
				Number.Int32ToNumber(value, ref numberBuffer);
				char* pointer = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
				if (c != '\0')
				{
					Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
				}
				else
				{
					Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.ToString();
			}
		}

		public unsafe static bool TryFormatInt32(int value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (value >= 0 && format.Length == 0)
			{
				return Number.TryUInt32ToDecStr((uint)value, -1, destination, out charsWritten);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0)
				{
					return Number.TryNegativeInt32ToDecStr(value, num, NumberFormatInfo.GetInstance(provider).NegativeSign, destination, out charsWritten);
				}
				return Number.TryUInt32ToDecStr((uint)value, num, destination, out charsWritten);
			}
			else
			{
				if (c2 == 'X')
				{
					return Number.TryInt32ToHexStr(value, c - '!', num, destination, out charsWritten);
				}
				NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
				byte* digits = stackalloc byte[(UIntPtr)11];
				Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 11);
				Number.Int32ToNumber(value, ref numberBuffer);
				char* pointer = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
				if (c != '\0')
				{
					Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
				}
				else
				{
					Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
		}

		public unsafe static string FormatUInt32(uint value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (format.Length == 0)
			{
				return Number.UInt32ToDecStr(value, -1);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return Number.UInt32ToDecStr(value, num);
			}
			if (c2 == 'X')
			{
				return Number.Int32ToHexStr((int)value, c - '!', num);
			}
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			byte* digits = stackalloc byte[(UIntPtr)11];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 11);
			Number.UInt32ToNumber(value, ref numberBuffer);
			char* pointer = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
			if (c != '\0')
			{
				Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
			}
			else
			{
				Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatUInt32(uint value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (format.Length == 0)
			{
				return Number.TryUInt32ToDecStr(value, -1, destination, out charsWritten);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return Number.TryUInt32ToDecStr(value, num, destination, out charsWritten);
			}
			if (c2 == 'X')
			{
				return Number.TryInt32ToHexStr((int)value, c - '!', num, destination, out charsWritten);
			}
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			byte* digits = stackalloc byte[(UIntPtr)11];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 11);
			Number.UInt32ToNumber(value, ref numberBuffer);
			char* pointer = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
			if (c != '\0')
			{
				Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
			}
			else
			{
				Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.TryCopyTo(destination, out charsWritten);
		}

		public unsafe static string FormatInt64(long value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (value >= 0L && format.Length == 0)
			{
				return Number.UInt64ToDecStr((ulong)value, -1);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0L)
				{
					return Number.NegativeInt64ToDecStr(value, num, NumberFormatInfo.GetInstance(provider).NegativeSign);
				}
				return Number.UInt64ToDecStr((ulong)value, num);
			}
			else
			{
				if (c2 == 'X')
				{
					return Number.Int64ToHexStr(value, c - '!', num);
				}
				NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
				byte* digits = stackalloc byte[(UIntPtr)20];
				Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 20);
				Number.Int64ToNumber(value, ref numberBuffer);
				char* pointer = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
				if (c != '\0')
				{
					Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
				}
				else
				{
					Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.ToString();
			}
		}

		public unsafe static bool TryFormatInt64(long value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (value >= 0L && format.Length == 0)
			{
				return Number.TryUInt64ToDecStr((ulong)value, -1, destination, out charsWritten);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				if (value < 0L)
				{
					return Number.TryNegativeInt64ToDecStr(value, num, NumberFormatInfo.GetInstance(provider).NegativeSign, destination, out charsWritten);
				}
				return Number.TryUInt64ToDecStr((ulong)value, num, destination, out charsWritten);
			}
			else
			{
				if (c2 == 'X')
				{
					return Number.TryInt64ToHexStr(value, c - '!', num, destination, out charsWritten);
				}
				NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
				byte* digits = stackalloc byte[(UIntPtr)20];
				Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 20);
				Number.Int64ToNumber(value, ref numberBuffer);
				char* pointer = stackalloc char[(UIntPtr)64];
				ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
				if (c != '\0')
				{
					Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
				}
				else
				{
					Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
				}
				return valueStringBuilder.TryCopyTo(destination, out charsWritten);
			}
		}

		public unsafe static string FormatUInt64(ulong value, ReadOnlySpan<char> format, IFormatProvider provider)
		{
			if (format.Length == 0)
			{
				return Number.UInt64ToDecStr(value, -1);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return Number.UInt64ToDecStr(value, num);
			}
			if (c2 == 'X')
			{
				return Number.Int64ToHexStr((long)value, c - '!', num);
			}
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			byte* digits = stackalloc byte[(UIntPtr)21];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 21);
			Number.UInt64ToNumber(value, ref numberBuffer);
			char* pointer = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
			if (c != '\0')
			{
				Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
			}
			else
			{
				Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.ToString();
		}

		public unsafe static bool TryFormatUInt64(ulong value, ReadOnlySpan<char> format, IFormatProvider provider, Span<char> destination, out int charsWritten)
		{
			if (format.Length == 0)
			{
				return Number.TryUInt64ToDecStr(value, -1, destination, out charsWritten);
			}
			int num;
			char c = Number.ParseFormatSpecifier(format, out num);
			char c2 = c & '￟';
			if ((c2 == 'G' && num < 1) || c2 == 'D')
			{
				return Number.TryUInt64ToDecStr(value, num, destination, out charsWritten);
			}
			if (c2 == 'X')
			{
				return Number.TryInt64ToHexStr((long)value, c - '!', num, destination, out charsWritten);
			}
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
			byte* digits = stackalloc byte[(UIntPtr)21];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 21);
			Number.UInt64ToNumber(value, ref numberBuffer);
			char* pointer = stackalloc char[(UIntPtr)64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(new Span<char>((void*)pointer, 32));
			if (c != '\0')
			{
				Number.NumberToString(ref valueStringBuilder, ref numberBuffer, c, num, instance);
			}
			else
			{
				Number.NumberToStringFormat(ref valueStringBuilder, ref numberBuffer, format, instance);
			}
			return valueStringBuilder.TryCopyTo(destination, out charsWritten);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void Int32ToNumber(int value, ref Number.NumberBuffer number)
		{
			number.DigitsCount = 10;
			if (value >= 0)
			{
				number.IsNegative = false;
			}
			else
			{
				number.IsNegative = true;
				value = -value;
			}
			byte* digitsPointer = number.GetDigitsPointer();
			byte* ptr = Number.UInt32ToDecChars(digitsPointer + 10, (uint)value, 0);
			int num = (int)((long)(digitsPointer + 10 - ptr));
			number.DigitsCount = num;
			number.Scale = num;
			byte* digitsPointer2 = number.GetDigitsPointer();
			while (--num >= 0)
			{
				*(digitsPointer2++) = *(ptr++);
			}
			*digitsPointer2 = 0;
		}

		private unsafe static string NegativeInt32ToDecStr(int value, int digits, string sNegative)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits((uint)(-(uint)value))) + sNegative.Length;
			string text = Number.FastAllocateString(num);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = Number.UInt32ToDecChars(ptr + num, (uint)(-(uint)value), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr2) = sNegative[i];
				}
			}
			return text;
		}

		private unsafe static bool TryNegativeInt32ToDecStr(int value, int digits, string sNegative, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits((uint)(-(uint)value))) + sNegative.Length;
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = Number.UInt32ToDecChars(reference + num, (uint)(-(uint)value), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr) = sNegative[i];
				}
			}
			return true;
		}

		private unsafe static string Int32ToHexStr(int value, char hexBase, int digits)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			string text;
			string result = text = Number.FastAllocateString(num);
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			Number.Int32ToHexChars(ptr + num, (uint)value, (int)hexBase, digits);
			text = null;
			return result;
		}

		private unsafe static bool TryInt32ToHexStr(int value, char hexBase, int digits, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				Number.Int32ToHexChars(reference + num, (uint)value, (int)hexBase, digits);
			}
			return true;
		}

		private unsafe static char* Int32ToHexChars(char* buffer, uint value, int hexBase, int digits)
		{
			while (--digits >= 0 || value != 0U)
			{
				byte b = (byte)(value & 15U);
				*(--buffer) = (char)((int)b + ((b < 10) ? 48 : hexBase));
				value >>= 4;
			}
			return buffer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void UInt32ToNumber(uint value, ref Number.NumberBuffer number)
		{
			number.DigitsCount = 10;
			number.IsNegative = false;
			byte* digitsPointer = number.GetDigitsPointer();
			byte* ptr = Number.UInt32ToDecChars(digitsPointer + 10, value, 0);
			int num = (int)((long)(digitsPointer + 10 - ptr));
			number.DigitsCount = num;
			number.Scale = num;
			byte* digitsPointer2 = number.GetDigitsPointer();
			while (--num >= 0)
			{
				*(digitsPointer2++) = *(ptr++);
			}
			*digitsPointer2 = 0;
		}

		internal unsafe static byte* UInt32ToDecChars(byte* bufferEnd, uint value, int digits)
		{
			while (--digits >= 0 || value != 0U)
			{
				uint num;
				value = MathEx.DivRem(value, 10U, out num);
				*(--bufferEnd) = (byte)(num + 48U);
			}
			return bufferEnd;
		}

		internal unsafe static char* UInt32ToDecChars(char* bufferEnd, uint value, int digits)
		{
			while (--digits >= 0 || value != 0U)
			{
				uint num;
				value = MathEx.DivRem(value, 10U, out num);
				*(--bufferEnd) = (char)(num + 48U);
			}
			return bufferEnd;
		}

		internal unsafe static string UInt32ToDecStr(uint value, int digits)
		{
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			if (num == 1)
			{
				return Number.s_singleDigitStringCache[(int)value];
			}
			string text = Number.FastAllocateString(num);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr + num;
				if (digits <= 1)
				{
					do
					{
						uint num2;
						value = MathEx.DivRem(value, 10U, out num2);
						*(--ptr2) = (char)(num2 + 48U);
					}
					while (value != 0U);
				}
				else
				{
					ptr2 = Number.UInt32ToDecChars(ptr2, value, digits);
				}
			}
			return text;
		}

		private unsafe static bool TryUInt32ToDecStr(uint value, int digits, Span<char> destination, out int charsWritten)
		{
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = reference + num;
				if (digits <= 1)
				{
					do
					{
						uint num2;
						value = MathEx.DivRem(value, 10U, out num2);
						*(--ptr) = (char)(num2 + 48U);
					}
					while (value != 0U);
				}
				else
				{
					ptr = Number.UInt32ToDecChars(ptr, value, digits);
				}
			}
			return true;
		}

		private unsafe static void Int64ToNumber(long input, ref Number.NumberBuffer number)
		{
			ulong value = (ulong)input;
			number.IsNegative = (input < 0L);
			number.DigitsCount = 19;
			if (number.IsNegative)
			{
				value = (ulong)(-(ulong)input);
			}
			byte* digitsPointer = number.GetDigitsPointer();
			byte* ptr = digitsPointer + 19;
			while (Number.High32(value) != 0U)
			{
				ptr = Number.UInt32ToDecChars(ptr, Number.Int64DivMod1E9(ref value), 9);
			}
			ptr = Number.UInt32ToDecChars(ptr, Number.Low32(value), 0);
			int num = (int)((long)(digitsPointer + 19 - ptr));
			number.DigitsCount = num;
			number.Scale = num;
			byte* digitsPointer2 = number.GetDigitsPointer();
			while (--num >= 0)
			{
				*(digitsPointer2++) = *(ptr++);
			}
			*digitsPointer2 = 0;
		}

		private unsafe static string NegativeInt64ToDecStr(long input, int digits, string sNegative)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			ulong value = (ulong)(-(ulong)input);
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value)) + sNegative.Length;
			string text = Number.FastAllocateString(num);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr + num;
				while (Number.High32(value) != 0U)
				{
					ptr2 = Number.UInt32ToDecChars(ptr2, Number.Int64DivMod1E9(ref value), 9);
					digits -= 9;
				}
				ptr2 = Number.UInt32ToDecChars(ptr2, Number.Low32(value), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr2) = sNegative[i];
				}
			}
			return text;
		}

		private unsafe static bool TryNegativeInt64ToDecStr(long input, int digits, string sNegative, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			ulong value = (ulong)(-(ulong)input);
			int num = Math.Max(digits, FormattingHelpers.CountDigits((ulong)(-(ulong)input))) + sNegative.Length;
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = reference + num;
				while (Number.High32(value) != 0U)
				{
					ptr = Number.UInt32ToDecChars(ptr, Number.Int64DivMod1E9(ref value), 9);
					digits -= 9;
				}
				ptr = Number.UInt32ToDecChars(ptr, Number.Low32(value), digits);
				for (int i = sNegative.Length - 1; i >= 0; i--)
				{
					*(--ptr) = sNegative[i];
				}
			}
			return true;
		}

		private unsafe static string Int64ToHexStr(long value, char hexBase, int digits)
		{
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			string text;
			string result = text = Number.FastAllocateString(num);
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			char* buffer = ptr + num;
			if (Number.High32((ulong)value) != 0U)
			{
				buffer = Number.Int32ToHexChars(buffer, Number.Low32((ulong)value), (int)hexBase, 8);
				buffer = Number.Int32ToHexChars(buffer, Number.High32((ulong)value), (int)hexBase, digits - 8);
			}
			else
			{
				buffer = Number.Int32ToHexChars(buffer, Number.Low32((ulong)value), (int)hexBase, Math.Max(digits, 1));
			}
			text = null;
			return result;
		}

		private unsafe static bool TryInt64ToHexStr(long value, char hexBase, int digits, Span<char> destination, out int charsWritten)
		{
			int num = Math.Max(digits, FormattingHelpers.CountHexDigits((ulong)value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* buffer = reference + num;
				if (Number.High32((ulong)value) != 0U)
				{
					buffer = Number.Int32ToHexChars(buffer, Number.Low32((ulong)value), (int)hexBase, 8);
					buffer = Number.Int32ToHexChars(buffer, Number.High32((ulong)value), (int)hexBase, digits - 8);
				}
				else
				{
					buffer = Number.Int32ToHexChars(buffer, Number.Low32((ulong)value), (int)hexBase, Math.Max(digits, 1));
				}
			}
			return true;
		}

		private unsafe static void UInt64ToNumber(ulong value, ref Number.NumberBuffer number)
		{
			number.DigitsCount = 20;
			number.IsNegative = false;
			byte* digitsPointer = number.GetDigitsPointer();
			byte* ptr = digitsPointer + 20;
			while (Number.High32(value) != 0U)
			{
				ptr = Number.UInt32ToDecChars(ptr, Number.Int64DivMod1E9(ref value), 9);
			}
			ptr = Number.UInt32ToDecChars(ptr, Number.Low32(value), 0);
			int num = (int)((long)(digitsPointer + 20 - ptr));
			number.DigitsCount = num;
			number.Scale = num;
			byte* digitsPointer2 = number.GetDigitsPointer();
			while (--num >= 0)
			{
				*(digitsPointer2++) = *(ptr++);
			}
			*digitsPointer2 = 0;
		}

		internal unsafe static string UInt64ToDecStr(ulong value, int digits)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			if (num == 1)
			{
				return Number.s_singleDigitStringCache[(int)(checked((IntPtr)value))];
			}
			string text = Number.FastAllocateString(num);
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* bufferEnd = ptr + num;
				while (Number.High32(value) != 0U)
				{
					bufferEnd = Number.UInt32ToDecChars(bufferEnd, Number.Int64DivMod1E9(ref value), 9);
					digits -= 9;
				}
				bufferEnd = Number.UInt32ToDecChars(bufferEnd, Number.Low32(value), digits);
			}
			return text;
		}

		private unsafe static bool TryUInt64ToDecStr(ulong value, int digits, Span<char> destination, out int charsWritten)
		{
			if (digits < 1)
			{
				digits = 1;
			}
			int num = Math.Max(digits, FormattingHelpers.CountDigits(value));
			if (num > destination.Length)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = num;
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* bufferEnd = reference + num;
				while (Number.High32(value) != 0U)
				{
					bufferEnd = Number.UInt32ToDecChars(bufferEnd, Number.Int64DivMod1E9(ref value), 9);
					digits -= 9;
				}
				bufferEnd = Number.UInt32ToDecChars(bufferEnd, Number.Low32(value), digits);
			}
			return true;
		}

		internal unsafe static char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
		{
			char c = '\0';
			if (format.Length > 0)
			{
				c = (char)(*format[0]);
				if (c - 'A' <= '\u0019' || c - 'a' <= '\u0019')
				{
					if (format.Length == 1)
					{
						digits = -1;
						return c;
					}
					if (format.Length == 2)
					{
						int num = (int)(*format[1] - 48);
						if (num < 10)
						{
							digits = num;
							return c;
						}
					}
					else if (format.Length == 3)
					{
						int num2 = (int)(*format[1] - 48);
						int num3 = (int)(*format[2] - 48);
						if (num2 < 10 && num3 < 10)
						{
							digits = num2 * 10 + num3;
							return c;
						}
					}
					int num4 = 0;
					int num5 = 1;
					while (num5 < format.Length && *format[num5] - 48 < 10 && num4 < 10)
					{
						num4 = num4 * 10 + (int)(*format[num5++]) - 48;
					}
					if (num5 == format.Length || *format[num5] == 0)
					{
						digits = num4;
						return c;
					}
				}
			}
			digits = -1;
			if (format.Length != 0 && c != '\0')
			{
				return '\0';
			}
			return 'G';
		}

		internal unsafe static void NumberToString(ref ValueStringBuilder sb, ref Number.NumberBuffer number, char format, int nMaxDigits, NumberFormatInfo info)
		{
			bool isCorrectlyRounded = number.Kind == Number.NumberBufferKind.FloatingPoint;
			if (format <= 'R')
			{
				switch (format)
				{
				case 'C':
					break;
				case 'D':
					goto IL_1F5;
				case 'E':
					goto IL_11E;
				case 'F':
					goto IL_B4;
				case 'G':
					goto IL_153;
				default:
					switch (format)
					{
					case 'N':
						goto IL_F7;
					case 'O':
					case 'Q':
						goto IL_1F5;
					case 'P':
						goto IL_1AE;
					case 'R':
						goto IL_1E0;
					default:
						goto IL_1F5;
					}
					break;
				}
			}
			else
			{
				switch (format)
				{
				case 'c':
					break;
				case 'd':
					goto IL_1F5;
				case 'e':
					goto IL_11E;
				case 'f':
					goto IL_B4;
				case 'g':
					goto IL_153;
				default:
					switch (format)
					{
					case 'n':
						goto IL_F7;
					case 'o':
					case 'q':
						goto IL_1F5;
					case 'p':
						goto IL_1AE;
					case 'r':
						goto IL_1E0;
					default:
						goto IL_1F5;
					}
					break;
				}
			}
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.CurrencyDecimalDigits;
			}
			Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			Number.FormatCurrency(ref sb, ref number, nMaxDigits, info);
			return;
			IL_B4:
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.NumberDecimalDigits;
			}
			Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			if (number.IsNegative)
			{
				sb.Append(info.NegativeSign);
			}
			Number.FormatFixed(ref sb, ref number, nMaxDigits, null, info.NumberDecimalSeparator, null);
			return;
			IL_F7:
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.NumberDecimalDigits;
			}
			Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			Number.FormatNumber(ref sb, ref number, nMaxDigits, info);
			return;
			IL_11E:
			if (nMaxDigits < 0)
			{
				nMaxDigits = 6;
			}
			nMaxDigits++;
			Number.RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);
			if (number.IsNegative)
			{
				sb.Append(info.NegativeSign);
			}
			Number.FormatScientific(ref sb, ref number, nMaxDigits, info, format);
			return;
			IL_153:
			bool bSuppressScientific = false;
			if (nMaxDigits < 1)
			{
				if (number.Kind == Number.NumberBufferKind.Decimal && nMaxDigits == -1)
				{
					bSuppressScientific = true;
					if (*number.Digits[0] == 0)
					{
						goto IL_19E;
					}
					goto IL_189;
				}
				else
				{
					nMaxDigits = number.DigitsCount;
				}
			}
			Number.RoundNumber(ref number, nMaxDigits, isCorrectlyRounded);
			IL_189:
			if (number.IsNegative)
			{
				sb.Append(info.NegativeSign);
			}
			IL_19E:
			Number.FormatGeneral(ref sb, ref number, nMaxDigits, info, format - '\u0002', bSuppressScientific);
			return;
			IL_1AE:
			if (nMaxDigits < 0)
			{
				nMaxDigits = info.PercentDecimalDigits;
			}
			number.Scale += 2;
			Number.RoundNumber(ref number, number.Scale + nMaxDigits, isCorrectlyRounded);
			Number.FormatPercent(ref sb, ref number, nMaxDigits, info);
			return;
			IL_1E0:
			if (number.Kind == Number.NumberBufferKind.FloatingPoint)
			{
				format -= '\v';
				goto IL_153;
			}
			IL_1F5:
			throw new FormatException("SR.Argument_BadFormatSpecifier");
		}

		internal unsafe static void NumberToStringFormat(ref ValueStringBuilder sb, ref Number.NumberBuffer number, ReadOnlySpan<char> format, NumberFormatInfo info)
		{
			int num = 0;
			byte* digitsPointer = number.GetDigitsPointer();
			int num2 = Number.FindSection(format, (*digitsPointer == 0) ? 2 : (number.IsNegative ? 1 : 0));
			int num3;
			int num4;
			int num5;
			int num6;
			bool flag;
			bool flag2;
			int i;
			for (;;)
			{
				num3 = 0;
				num4 = -1;
				num5 = int.MaxValue;
				num6 = 0;
				flag = false;
				int num7 = -1;
				flag2 = false;
				int num8 = 0;
				i = num2;
				fixed (char* reference = MemoryMarshal.GetReference<char>(format))
				{
					char* ptr = reference;
					char c;
					while (i < format.Length && (c = ptr[(IntPtr)(i++) * 2]) != '\0' && c != ';')
					{
						if (c <= 'E')
						{
							switch (c)
							{
							case '"':
							case '\'':
								while (i < format.Length && ptr[i] != '\0')
								{
									if (ptr[(IntPtr)(i++) * 2] == c)
									{
										break;
									}
								}
								continue;
							case '#':
								num3++;
								continue;
							case '$':
							case '&':
								continue;
							case '%':
								num8 += 2;
								continue;
							default:
								switch (c)
								{
								case ',':
									if (num3 > 0 && num4 < 0)
									{
										if (num7 >= 0)
										{
											if (num7 == num3)
											{
												num++;
												continue;
											}
											flag2 = true;
										}
										num7 = num3;
										num = 1;
										continue;
									}
									continue;
								case '-':
								case '/':
									continue;
								case '.':
									if (num4 < 0)
									{
										num4 = num3;
										continue;
									}
									continue;
								case '0':
									if (num5 == 2147483647)
									{
										num5 = num3;
									}
									num3++;
									num6 = num3;
									continue;
								default:
									if (c != 'E')
									{
										continue;
									}
									break;
								}
								break;
							}
						}
						else if (c != '\\')
						{
							if (c != 'e')
							{
								if (c != '‰')
								{
									continue;
								}
								num8 += 3;
								continue;
							}
						}
						else
						{
							if (i < format.Length && ptr[i] != '\0')
							{
								i++;
								continue;
							}
							continue;
						}
						if ((i < format.Length && ptr[i] == '0') || (i + 1 < format.Length && (ptr[i] == '+' || ptr[i] == '-') && ptr[i + 1] == '0'))
						{
							while (++i < format.Length && ptr[i] == '0')
							{
							}
							flag = true;
						}
					}
				}
				if (num4 < 0)
				{
					num4 = num3;
				}
				if (num7 >= 0)
				{
					if (num7 == num4)
					{
						num8 -= num * 3;
					}
					else
					{
						flag2 = true;
					}
				}
				if (*digitsPointer == 0)
				{
					break;
				}
				number.Scale += num8;
				int pos = flag ? num3 : (number.Scale + num3 - num4);
				Number.RoundNumber(ref number, pos, false);
				if (*digitsPointer != 0)
				{
					goto IL_2A8;
				}
				i = Number.FindSection(format, 2);
				if (i == num2)
				{
					goto IL_2A8;
				}
				num2 = i;
			}
			if (number.Kind != Number.NumberBufferKind.FloatingPoint)
			{
				number.IsNegative = false;
			}
			number.Scale = 0;
			IL_2A8:
			num5 = ((num5 < num4) ? (num4 - num5) : 0);
			num6 = ((num6 > num4) ? (num4 - num6) : 0);
			int num9;
			int j;
			if (flag)
			{
				num9 = num4;
				j = 0;
			}
			else
			{
				num9 = ((number.Scale > num4) ? number.Scale : num4);
				j = number.Scale - num4;
			}
			i = num2;
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)16], 4);
			int num10 = -1;
			if (flag2 && info.NumberGroupSeparator.Length > 0)
			{
				int[] numberGroupSizes = info.NumberGroupSizes;
				int num11 = 0;
				int num12 = 0;
				int num13 = numberGroupSizes.Length;
				if (num13 != 0)
				{
					num12 = numberGroupSizes[num11];
				}
				int num14 = num12;
				int num15 = num9 + ((j < 0) ? j : 0);
				int num16 = (num5 > num15) ? num5 : num15;
				while (num16 > num12 && num14 != 0)
				{
					num10++;
					if (num10 >= span.Length)
					{
						int[] array = new int[span.Length * 2];
						span.CopyTo(array);
						span = array;
					}
					*span[num10] = num12;
					if (num11 < num13 - 1)
					{
						num11++;
						num14 = numberGroupSizes[num11];
					}
					num12 += num14;
				}
			}
			if (number.IsNegative && num2 == 0 && number.Scale != 0)
			{
				sb.Append(info.NegativeSign);
			}
			bool flag3 = false;
			fixed (char* reference = MemoryMarshal.GetReference<char>(format))
			{
				char* ptr2 = reference;
				byte* ptr3 = digitsPointer;
				char c;
				while (i < format.Length && (c = ptr2[(IntPtr)(i++) * 2]) != '\0' && c != ';')
				{
					if (j > 0)
					{
						if (c == '#' || c == '.' || c == '0')
						{
							while (j > 0)
							{
								sb.Append((char)((*ptr3 != 0) ? (*(ptr3++)) : 48));
								if (flag2 && num9 > 1 && num10 >= 0 && num9 == *span[num10] + 1)
								{
									sb.Append(info.NumberGroupSeparator);
									num10--;
								}
								num9--;
								j--;
							}
						}
					}
					if (c <= 'E')
					{
						switch (c)
						{
						case '"':
						case '\'':
							while (i < format.Length && ptr2[i] != '\0' && ptr2[i] != c)
							{
								sb.Append(ptr2[(IntPtr)(i++) * 2]);
							}
							if (i < format.Length && ptr2[i] != '\0')
							{
								i++;
								continue;
							}
							continue;
						case '#':
							break;
						case '$':
						case '&':
							goto IL_798;
						case '%':
							sb.Append(info.PercentSymbol);
							continue;
						default:
							switch (c)
							{
							case ',':
								continue;
							case '-':
							case '/':
								goto IL_798;
							case '.':
								if (num9 == 0 && !flag3 && (num6 < 0 || (num4 < num3 && *ptr3 != 0)))
								{
									sb.Append(info.NumberDecimalSeparator);
									flag3 = true;
									continue;
								}
								continue;
							case '0':
								break;
							default:
								if (c != 'E')
								{
									goto IL_798;
								}
								goto IL_643;
							}
							break;
						}
						if (j < 0)
						{
							j++;
							c = ((num9 <= num5) ? '0' : '\0');
						}
						else
						{
							c = (char)((*ptr3 != 0) ? (*(ptr3++)) : ((num9 > num6) ? 48 : 0));
						}
						if (c != '\0')
						{
							sb.Append(c);
							if (flag2 && num9 > 1 && num10 >= 0 && num9 == *span[num10] + 1)
							{
								sb.Append(info.NumberGroupSeparator);
								num10--;
							}
						}
						num9--;
						continue;
					}
					if (c != '\\')
					{
						if (c != 'e')
						{
							if (c != '‰')
							{
								goto IL_798;
							}
							sb.Append(info.PerMilleSymbol);
							continue;
						}
					}
					else
					{
						if (i < format.Length && ptr2[i] != '\0')
						{
							sb.Append(ptr2[(IntPtr)(i++) * 2]);
							continue;
						}
						continue;
					}
					IL_643:
					bool positiveSign = false;
					int num17 = 0;
					if (flag)
					{
						if (i < format.Length && ptr2[i] == '0')
						{
							num17++;
						}
						else if (i + 1 < format.Length && ptr2[i] == '+' && ptr2[i + 1] == '0')
						{
							positiveSign = true;
						}
						else if (i + 1 >= format.Length || ptr2[i] != '-' || ptr2[i + 1] != '0')
						{
							sb.Append(c);
							continue;
						}
						while (++i < format.Length && ptr2[i] == '0')
						{
							num17++;
						}
						if (num17 > 10)
						{
							num17 = 10;
						}
						int value = (*digitsPointer == 0) ? 0 : (number.Scale - num4);
						Number.FormatExponent(ref sb, info, value, c, num17, positiveSign);
						flag = false;
						continue;
					}
					sb.Append(c);
					if (i < format.Length)
					{
						if (ptr2[i] == '+' || ptr2[i] == '-')
						{
							sb.Append(ptr2[(IntPtr)(i++) * 2]);
						}
						while (i < format.Length)
						{
							if (ptr2[i] != '0')
							{
								break;
							}
							sb.Append(ptr2[(IntPtr)(i++) * 2]);
						}
						continue;
					}
					continue;
					IL_798:
					sb.Append(c);
				}
			}
			if (number.IsNegative && num2 == 0 && number.Scale == 0 && sb.Length > 0)
			{
				sb.Insert(0, info.NegativeSign);
			}
		}

		private static void FormatCurrency(ref ValueStringBuilder sb, ref Number.NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
		{
			foreach (char c in number.IsNegative ? Number.s_negCurrencyFormats[info.CurrencyNegativePattern] : Number.s_posCurrencyFormats[info.CurrencyPositivePattern])
			{
				if (c != '#')
				{
					if (c != '$')
					{
						if (c != '-')
						{
							sb.Append(c);
						}
						else
						{
							sb.Append(info.NegativeSign);
						}
					}
					else
					{
						sb.Append(info.CurrencySymbol);
					}
				}
				else
				{
					Number.FormatFixed(ref sb, ref number, nMaxDigits, info.CurrencyGroupSizes, info.CurrencyDecimalSeparator, info.CurrencyGroupSeparator);
				}
			}
		}

		private unsafe static void FormatFixed(ref ValueStringBuilder sb, ref Number.NumberBuffer number, int nMaxDigits, int[] groupDigits, string sDecimal, string sGroup)
		{
			int i = number.Scale;
			byte* ptr = number.GetDigitsPointer();
			if (i > 0)
			{
				if (groupDigits != null)
				{
					int num = 0;
					int num2 = i;
					int num3 = 0;
					if (groupDigits.Length != 0)
					{
						int num4 = groupDigits[num];
						while (i > num4)
						{
							num3 = groupDigits[num];
							if (num3 == 0)
							{
								break;
							}
							num2 += sGroup.Length;
							if (num < groupDigits.Length - 1)
							{
								num++;
							}
							num4 += groupDigits[num];
							if (num4 < 0 || num2 < 0)
							{
								throw new ArgumentOutOfRangeException();
							}
						}
						num3 = ((num4 == 0) ? 0 : groupDigits[0]);
					}
					num = 0;
					int num5 = 0;
					int digitsCount = number.DigitsCount;
					int num6 = (i < digitsCount) ? i : digitsCount;
					fixed (char* reference = MemoryMarshal.GetReference<char>(sb.AppendSpan(num2)))
					{
						char* ptr2 = reference + num2 - 1;
						for (int j = i - 1; j >= 0; j--)
						{
							*(ptr2--) = (char)((j < num6) ? ptr[j] : 48);
							if (num3 > 0)
							{
								num5++;
								if (num5 == num3 && j != 0)
								{
									for (int k = sGroup.Length - 1; k >= 0; k--)
									{
										*(ptr2--) = sGroup[k];
									}
									if (num < groupDigits.Length - 1)
									{
										num++;
										num3 = groupDigits[num];
									}
									num5 = 0;
								}
							}
						}
						ptr += num6;
					}
				}
				else
				{
					do
					{
						sb.Append((char)((*ptr != 0) ? (*(ptr++)) : 48));
					}
					while (--i > 0);
				}
			}
			else
			{
				sb.Append('0');
			}
			if (nMaxDigits > 0)
			{
				sb.Append(sDecimal);
				if (i < 0 && nMaxDigits > 0)
				{
					int num7 = Math.Min(-i, nMaxDigits);
					sb.Append('0', num7);
					i += num7;
					nMaxDigits -= num7;
				}
				while (nMaxDigits > 0)
				{
					sb.Append((char)((*ptr != 0) ? (*(ptr++)) : 48));
					nMaxDigits--;
				}
			}
		}

		private static void FormatNumber(ref ValueStringBuilder sb, ref Number.NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
		{
			foreach (char c in number.IsNegative ? Number.s_negNumberFormats[info.NumberNegativePattern] : "#")
			{
				if (c != '#')
				{
					if (c != '-')
					{
						sb.Append(c);
					}
					else
					{
						sb.Append(info.NegativeSign);
					}
				}
				else
				{
					Number.FormatFixed(ref sb, ref number, nMaxDigits, info.NumberGroupSizes, info.NumberDecimalSeparator, info.NumberGroupSeparator);
				}
			}
		}

		private unsafe static void FormatScientific(ref ValueStringBuilder sb, ref Number.NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar)
		{
			byte* digitsPointer = number.GetDigitsPointer();
			sb.Append((char)((*digitsPointer != 0) ? (*(digitsPointer++)) : 48));
			if (nMaxDigits != 1)
			{
				sb.Append(info.NumberDecimalSeparator);
			}
			while (--nMaxDigits > 0)
			{
				sb.Append((char)((*digitsPointer != 0) ? (*(digitsPointer++)) : 48));
			}
			int value = (*number.Digits[0] == 0) ? 0 : (number.Scale - 1);
			Number.FormatExponent(ref sb, info, value, expChar, 3, true);
		}

		private unsafe static void FormatExponent(ref ValueStringBuilder sb, NumberFormatInfo info, int value, char expChar, int minDigits, bool positiveSign)
		{
			sb.Append(expChar);
			if (value < 0)
			{
				sb.Append(info.NegativeSign);
				value = -value;
			}
			else if (positiveSign)
			{
				sb.Append(info.PositiveSign);
			}
			char* ptr = stackalloc char[(UIntPtr)20];
			char* ptr2 = Number.UInt32ToDecChars(ptr + 10, (uint)value, minDigits);
			sb.Append(ptr2, (int)((long)(ptr + 10 - ptr2)));
		}

		private unsafe static void FormatGeneral(ref ValueStringBuilder sb, ref Number.NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar, bool bSuppressScientific)
		{
			int i = number.Scale;
			bool flag = false;
			if (!bSuppressScientific && (i > nMaxDigits || i < -3))
			{
				i = 1;
				flag = true;
			}
			byte* digitsPointer = number.GetDigitsPointer();
			if (i > 0)
			{
				do
				{
					sb.Append((char)((*digitsPointer != 0) ? (*(digitsPointer++)) : 48));
				}
				while (--i > 0);
			}
			else
			{
				sb.Append('0');
			}
			if (*digitsPointer != 0 || i < 0)
			{
				sb.Append(info.NumberDecimalSeparator);
				while (i < 0)
				{
					sb.Append('0');
					i++;
				}
				while (*digitsPointer != 0)
				{
					sb.Append((char)(*(digitsPointer++)));
				}
			}
			if (flag)
			{
				Number.FormatExponent(ref sb, info, number.Scale - 1, expChar, 2, true);
			}
		}

		private static void FormatPercent(ref ValueStringBuilder sb, ref Number.NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
		{
			foreach (char c in number.IsNegative ? Number.s_negPercentFormats[info.PercentNegativePattern] : Number.s_posPercentFormats[info.PercentPositivePattern])
			{
				if (c != '#')
				{
					if (c != '%')
					{
						if (c != '-')
						{
							sb.Append(c);
						}
						else
						{
							sb.Append(info.NegativeSign);
						}
					}
					else
					{
						sb.Append(info.PercentSymbol);
					}
				}
				else
				{
					Number.FormatFixed(ref sb, ref number, nMaxDigits, info.PercentGroupSizes, info.PercentDecimalSeparator, info.PercentGroupSeparator);
				}
			}
		}

		internal unsafe static void RoundNumber(ref Number.NumberBuffer number, int pos, bool isCorrectlyRounded)
		{
			byte* digitsPointer = number.GetDigitsPointer();
			int num = 0;
			while (num < pos && digitsPointer[num] != 0)
			{
				num++;
			}
			if (num == pos && Number.<RoundNumber>g__ShouldRoundUp|70_0(digitsPointer, num, number.Kind, isCorrectlyRounded))
			{
				while (num > 0 && digitsPointer[num - 1] == 57)
				{
					num--;
				}
				if (num > 0)
				{
					byte* ptr = digitsPointer + (num - 1);
					*ptr += 1;
				}
				else
				{
					number.Scale++;
					*digitsPointer = 49;
					num = 1;
				}
			}
			else
			{
				while (num > 0 && digitsPointer[num - 1] == 48)
				{
					num--;
				}
			}
			if (num == 0)
			{
				if (number.Kind != Number.NumberBufferKind.FloatingPoint)
				{
					number.IsNegative = false;
				}
				number.Scale = 0;
			}
			digitsPointer[num] = 0;
			number.DigitsCount = num;
		}

		private unsafe static int FindSection(ReadOnlySpan<char> format, int section)
		{
			if (section == 0)
			{
				return 0;
			}
			fixed (char* reference = MemoryMarshal.GetReference<char>(format))
			{
				char* ptr = reference;
				int i = 0;
				while (i < format.Length)
				{
					char c2;
					char c = c2 = ptr[(IntPtr)(i++) * 2];
					if (c2 <= '"')
					{
						if (c2 == '\0')
						{
							return 0;
						}
						if (c2 != '"')
						{
							continue;
						}
					}
					else if (c2 != '\'')
					{
						if (c2 != ';')
						{
							if (c2 != '\\')
							{
								continue;
							}
							if (i < format.Length && ptr[i] != '\0')
							{
								i++;
								continue;
							}
							continue;
						}
						else
						{
							if (--section != 0)
							{
								continue;
							}
							if (i < format.Length && ptr[i] != '\0' && ptr[i] != ';')
							{
								return i;
							}
							return 0;
						}
					}
					while (i < format.Length && ptr[i] != '\0')
					{
						if (ptr[(IntPtr)(i++) * 2] == c)
						{
							break;
						}
					}
				}
				return 0;
			}
		}

		private static uint Low32(ulong value)
		{
			return (uint)value;
		}

		private static uint High32(ulong value)
		{
			return (uint)((value & 18446744069414584320UL) >> 32);
		}

		private static uint Int64DivMod1E9(ref ulong value)
		{
			uint result = (uint)(value % 1000000000UL);
			value /= 1000000000UL;
			return result;
		}

		private static ulong ExtractFractionAndBiasedExponent(double value, out int exponent)
		{
			ulong num = (ulong)BitConverter.DoubleToInt64Bits(value);
			ulong num2 = num & 4503599627370495UL;
			exponent = ((int)(num >> 52) & 2047);
			if (exponent != 0)
			{
				num2 |= 4503599627370496UL;
				exponent -= 1075;
			}
			else
			{
				exponent = -1074;
			}
			return num2;
		}

		private static uint ExtractFractionAndBiasedExponent(float value, out int exponent)
		{
			uint num = (uint)Number.SingleToInt32Bits(value);
			uint num2 = num & 8388607U;
			exponent = (int)(num >> 23 & 255U);
			if (exponent != 0)
			{
				num2 |= 8388608U;
				exponent -= 150;
			}
			else
			{
				exponent = -149;
			}
			return num2;
		}

		private static string FastAllocateString(int length)
		{
			return new string('\0', length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static int SingleToInt32Bits(float value)
		{
			return *(int*)(&value);
		}

		private unsafe static void AccumulateDecimalDigitsIntoBigInteger(ref Number.NumberBuffer number, uint firstIndex, uint lastIndex, out Number.BigInteger result)
		{
			Number.BigInteger.SetZero(out result);
			byte* ptr = number.GetDigitsPointer() + firstIndex;
			uint num2;
			for (uint num = lastIndex - firstIndex; num != 0U; num -= num2)
			{
				num2 = Math.Min(num, 9U);
				uint value = Number.DigitsToUInt32(ptr, (int)num2);
				result.MultiplyPow10(num2);
				result.Add(value);
				ptr += num2;
			}
		}

		private static ulong AssembleFloatingPointBits(in Number.FloatingPointInfo info, ulong initialMantissa, int initialExponent, bool hasZeroTail)
		{
			uint num = Number.BigInteger.CountSignificantBits(initialMantissa);
			int num2 = (int)((uint)info.NormalMantissaBits - num);
			int num3 = initialExponent - num2;
			ulong num4 = initialMantissa;
			int num5 = num3;
			if (num3 > info.MaxBinaryExponent)
			{
				return info.InfinityBits;
			}
			if (num3 < info.MinBinaryExponent)
			{
				int num6 = num2 + num3 + info.ExponentBias - 1;
				num5 = -info.ExponentBias;
				if (num6 < 0)
				{
					num4 = Number.RightShiftWithRounding(num4, -num6, hasZeroTail);
					if (num4 == 0UL)
					{
						return info.ZeroBits;
					}
					if (num4 > info.DenormalMantissaMask)
					{
						num5 = initialExponent - (num6 + 1) - num2;
					}
				}
				else
				{
					num4 <<= num6;
				}
			}
			else if (num2 < 0)
			{
				num4 = Number.RightShiftWithRounding(num4, -num2, hasZeroTail);
				if (num4 > info.NormalMantissaMask)
				{
					num4 >>= 1;
					num5++;
					if (num5 > info.MaxBinaryExponent)
					{
						return info.InfinityBits;
					}
				}
			}
			else if (num2 > 0)
			{
				num4 <<= num2;
			}
			num4 &= info.DenormalMantissaMask;
			return (ulong)((long)(num5 + info.ExponentBias) << (int)info.DenormalMantissaBits | (long)num4);
		}

		private static ulong ConvertBigIntegerToFloatingPointBits(ref Number.BigInteger value, in Number.FloatingPointInfo info, uint integerBitsOfPrecision, bool hasNonZeroFractionalPart)
		{
			int denormalMantissaBits = (int)info.DenormalMantissaBits;
			if (integerBitsOfPrecision <= 64U)
			{
				return Number.AssembleFloatingPointBits(info, value.ToUInt64(), denormalMantissaBits, !hasNonZeroFractionalPart);
			}
			uint num2;
			uint num = MathEx.DivRem(integerBitsOfPrecision, 32U, out num2);
			uint num3 = num - 1U;
			uint num4 = num3 - 1U;
			int num5 = denormalMantissaBits + (int)(num4 * 32U);
			bool flag = !hasNonZeroFractionalPart;
			ulong initialMantissa;
			if (num2 == 0U)
			{
				initialMantissa = ((ulong)value.GetBlock(num3) << 32) + (ulong)value.GetBlock(num4);
			}
			else
			{
				int num6 = (int)num2;
				int num7 = 64 - num6;
				int num8 = num7 - 32;
				num5 += (int)num2;
				uint block = value.GetBlock(num4);
				uint num9 = block >> num6;
				ulong num10 = (ulong)value.GetBlock(num3) << num8;
				initialMantissa = ((ulong)value.GetBlock(num) << num7) + num10 + (ulong)num9;
				uint num11 = (1U << (int)num2) - 1U;
				flag &= ((block & num11) == 0U);
			}
			for (uint num12 = 0U; num12 != num4; num12 += 1U)
			{
				flag &= (value.GetBlock(num12) == 0U);
			}
			return Number.AssembleFloatingPointBits(info, initialMantissa, num5, flag);
		}

		private unsafe static uint DigitsToUInt32(byte* p, int count)
		{
			byte* ptr = p + count;
			uint num = (uint)(*p - 48);
			for (p++; p < ptr; p++)
			{
				num = 10U * num + (uint)(*p) - 48U;
			}
			return num;
		}

		private unsafe static ulong DigitsToUInt64(byte* p, int count)
		{
			byte* ptr = p + count;
			ulong num = (ulong)((long)(*p - 48));
			for (p++; p < ptr; p++)
			{
				num = 10UL * num + (ulong)(*p) - 48UL;
			}
			return num;
		}

		private unsafe static ulong NumberToFloatingPointBits(ref Number.NumberBuffer number, in Number.FloatingPointInfo info)
		{
			uint digitsCount = (uint)number.DigitsCount;
			uint num = (uint)Math.Max(0, number.Scale);
			uint num2 = Math.Min(num, digitsCount);
			uint num3 = digitsCount - num2;
			uint num4 = (uint)Math.Abs((long)number.Scale - (long)((ulong)num2) - (long)((ulong)num3));
			byte* digitsPointer = number.GetDigitsPointer();
			if (info.DenormalMantissaBits == 23 && digitsCount <= 7U && num4 <= 10U)
			{
				float num5 = Number.DigitsToUInt32(digitsPointer, (int)digitsCount);
				float num6 = Number.s_Pow10SingleTable[(int)num4];
				if (num3 != 0U)
				{
					num5 /= num6;
				}
				else
				{
					num5 *= num6;
				}
				return (ulong)Number.SingleToInt32Bits(num5);
			}
			if (digitsCount > 15U || num4 > 22U)
			{
				return Number.NumberToFloatingPointBitsSlow(ref number, info, num, num2, num3);
			}
			double num7 = Number.DigitsToUInt64(digitsPointer, (int)digitsCount);
			double num8 = Number.s_Pow10DoubleTable[(int)num4];
			if (num3 != 0U)
			{
				num7 /= num8;
			}
			else
			{
				num7 *= num8;
			}
			if (info.DenormalMantissaBits == 52)
			{
				return (ulong)BitConverter.DoubleToInt64Bits(num7);
			}
			return (ulong)Number.SingleToInt32Bits((float)num7);
		}

		private static ulong NumberToFloatingPointBitsSlow(ref Number.NumberBuffer number, in Number.FloatingPointInfo info, uint positiveExponent, uint integerDigitsPresent, uint fractionalDigitsPresent)
		{
			uint num = (uint)(info.NormalMantissaBits + 1);
			uint digitsCount = (uint)number.DigitsCount;
			uint num2 = positiveExponent - integerDigitsPresent;
			uint lastIndex = digitsCount;
			Number.BigInteger bigInteger;
			Number.AccumulateDecimalDigitsIntoBigInteger(ref number, 0U, integerDigitsPresent, out bigInteger);
			if (num2 > 0U)
			{
				if ((ulong)num2 > (ulong)((long)info.OverflowDecimalExponent))
				{
					return info.InfinityBits;
				}
				bigInteger.MultiplyPow10(num2);
			}
			uint num3 = Number.BigInteger.CountSignificantBits(ref bigInteger);
			if (num3 >= num || fractionalDigitsPresent == 0U)
			{
				return Number.ConvertBigIntegerToFloatingPointBits(ref bigInteger, info, num3, fractionalDigitsPresent > 0U);
			}
			uint num4 = fractionalDigitsPresent;
			if (number.Scale < 0)
			{
				num4 += (uint)(-(uint)number.Scale);
			}
			if (num3 == 0U && (ulong)num4 - (ulong)((long)digitsCount) > (ulong)((long)info.OverflowDecimalExponent))
			{
				return info.ZeroBits;
			}
			Number.BigInteger bigInteger2;
			Number.AccumulateDecimalDigitsIntoBigInteger(ref number, integerDigitsPresent, lastIndex, out bigInteger2);
			if (bigInteger2.IsZero())
			{
				return Number.ConvertBigIntegerToFloatingPointBits(ref bigInteger, info, num3, fractionalDigitsPresent > 0U);
			}
			Number.BigInteger bigInteger3;
			Number.BigInteger.Pow10(num4, out bigInteger3);
			uint num5 = Number.BigInteger.CountSignificantBits(ref bigInteger2);
			uint num6 = Number.BigInteger.CountSignificantBits(ref bigInteger3);
			uint num7 = 0U;
			if (num6 > num5)
			{
				num7 = num6 - num5;
			}
			if (num7 > 0U)
			{
				bigInteger2.ShiftLeft(num7);
			}
			uint num8 = num - num3;
			uint num9 = num8;
			if (num3 > 0U)
			{
				if (num7 > num9)
				{
					return Number.ConvertBigIntegerToFloatingPointBits(ref bigInteger, info, num3, fractionalDigitsPresent > 0U);
				}
				num9 -= num7;
			}
			uint num10 = num7;
			if (Number.BigInteger.Compare(ref bigInteger2, ref bigInteger3) < 0)
			{
				num10 += 1U;
			}
			bigInteger2.ShiftLeft(num9);
			Number.BigInteger bigInteger4;
			Number.BigInteger bigInteger5;
			Number.BigInteger.DivRem(ref bigInteger2, ref bigInteger3, out bigInteger4, out bigInteger5);
			ulong num11 = bigInteger4.ToUInt64();
			bool flag = !number.HasNonZeroTail && bigInteger5.IsZero();
			uint num12 = Number.BigInteger.CountSignificantBits(num11);
			if (num12 > num8)
			{
				int num13 = (int)(num12 - num8);
				flag = (flag && (num11 & (1UL << num13) - 1UL) == 0UL);
				num11 >>= num13;
			}
			ulong initialMantissa = (bigInteger.ToUInt64() << (int)num8) + num11;
			int initialExponent = (int)((num3 > 0U) ? (num3 - 2U) : (-num10 - 1U));
			return Number.AssembleFloatingPointBits(info, initialMantissa, initialExponent, flag);
		}

		private static ulong RightShiftWithRounding(ulong value, int shift, bool hasZeroTail)
		{
			if (shift >= 64)
			{
				return 0UL;
			}
			ulong num = (1UL << shift - 1) - 1UL;
			ulong num2 = 1UL << shift - 1;
			ulong num3 = 1UL << shift;
			bool lsbBit = (value & num3) > 0UL;
			bool roundBit = (value & num2) > 0UL;
			bool hasTailBits = !hasZeroTail || (value & num) > 0UL;
			return (value >> shift) + (Number.ShouldRoundUp(lsbBit, roundBit, hasTailBits) ? 1UL : 0UL);
		}

		private static bool ShouldRoundUp(bool lsbBit, bool roundBit, bool hasTailBits)
		{
			return roundBit && (hasTailBits || lsbBit);
		}

		internal unsafe static ReadOnlySpan<byte> CharToHexLookup
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.3119C902A2D30870A3FC3661C8D3CC542815988CC258DFA0A4B9396E04855905), 103);
			}
		}

		private unsafe static bool TryNumberToInt32(ref Number.NumberBuffer number, ref int value)
		{
			int num = number.Scale;
			if (num > 10 || num < number.DigitsCount)
			{
				return false;
			}
			byte* digitsPointer = number.GetDigitsPointer();
			int num2 = 0;
			while (--num >= 0)
			{
				if (num2 > 214748364)
				{
					return false;
				}
				num2 *= 10;
				if (*digitsPointer != 0)
				{
					num2 += (int)(*(digitsPointer++) - 48);
				}
			}
			if (number.IsNegative)
			{
				num2 = -num2;
				if (num2 > 0)
				{
					return false;
				}
			}
			else if (num2 < 0)
			{
				return false;
			}
			value = num2;
			return true;
		}

		private unsafe static bool TryNumberToInt64(ref Number.NumberBuffer number, ref long value)
		{
			int num = number.Scale;
			if (num > 19 || num < number.DigitsCount)
			{
				return false;
			}
			byte* digitsPointer = number.GetDigitsPointer();
			long num2 = 0L;
			while (--num >= 0)
			{
				if (num2 > 922337203685477580L)
				{
					return false;
				}
				num2 *= 10L;
				if (*digitsPointer != 0)
				{
					num2 += (long)(*(digitsPointer++) - 48);
				}
			}
			if (number.IsNegative)
			{
				num2 = -num2;
				if (num2 > 0L)
				{
					return false;
				}
			}
			else if (num2 < 0L)
			{
				return false;
			}
			value = num2;
			return true;
		}

		private unsafe static bool TryNumberToUInt32(ref Number.NumberBuffer number, ref uint value)
		{
			int num = number.Scale;
			if (num > 10 || num < number.DigitsCount || number.IsNegative)
			{
				return false;
			}
			byte* digitsPointer = number.GetDigitsPointer();
			uint num2 = 0U;
			while (--num >= 0)
			{
				if (num2 > 429496729U)
				{
					return false;
				}
				num2 *= 10U;
				if (*digitsPointer != 0)
				{
					uint num3 = num2 + (uint)(*(digitsPointer++) - 48);
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		private unsafe static bool TryNumberToUInt64(ref Number.NumberBuffer number, ref ulong value)
		{
			int num = number.Scale;
			if (num > 20 || num < number.DigitsCount || number.IsNegative)
			{
				return false;
			}
			byte* digitsPointer = number.GetDigitsPointer();
			ulong num2 = 0UL;
			while (--num >= 0)
			{
				if (num2 > 1844674407370955161UL)
				{
					return false;
				}
				num2 *= 10UL;
				if (*digitsPointer != 0)
				{
					ulong num3 = num2 + (ulong)((long)(*(digitsPointer++) - 48));
					if (num3 < num2)
					{
						return false;
					}
					num2 = num3;
				}
			}
			value = num2;
			return true;
		}

		internal static int ParseInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			int result;
			Number.ParsingStatus parsingStatus = Number.TryParseInt32(value, styles, info, out result);
			if (parsingStatus != Number.ParsingStatus.OK)
			{
				Number.ThrowOverflowOrFormatException(parsingStatus, TypeCode.Int32);
			}
			return result;
		}

		internal static long ParseInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			long result;
			Number.ParsingStatus parsingStatus = Number.TryParseInt64(value, styles, info, out result);
			if (parsingStatus != Number.ParsingStatus.OK)
			{
				Number.ThrowOverflowOrFormatException(parsingStatus, TypeCode.Int64);
			}
			return result;
		}

		internal static uint ParseUInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			uint result;
			Number.ParsingStatus parsingStatus = Number.TryParseUInt32(value, styles, info, out result);
			if (parsingStatus != Number.ParsingStatus.OK)
			{
				Number.ThrowOverflowOrFormatException(parsingStatus, TypeCode.UInt32);
			}
			return result;
		}

		internal static ulong ParseUInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			ulong result;
			Number.ParsingStatus parsingStatus = Number.TryParseUInt64(value, styles, info, out result);
			if (parsingStatus != Number.ParsingStatus.OK)
			{
				Number.ThrowOverflowOrFormatException(parsingStatus, TypeCode.UInt64);
			}
			return result;
		}

		private unsafe static bool TryParseNumber(ref char* str, char* strEnd, NumberStyles styles, ref Number.NumberBuffer number, NumberFormatInfo info)
		{
			string text = null;
			bool flag = false;
			string value;
			string value2;
			if ((styles & NumberStyles.AllowCurrencySymbol) != NumberStyles.None)
			{
				text = info.CurrencySymbol;
				value = info.CurrencyDecimalSeparator;
				value2 = info.CurrencyGroupSeparator;
				flag = true;
			}
			else
			{
				value = info.NumberDecimalSeparator;
				value2 = info.NumberGroupSeparator;
			}
			int num = 0;
			char* ptr = str;
			char c = (ptr < strEnd) ? (*ptr) : '\0';
			for (;;)
			{
				if (!Number.IsWhite((int)c) || (styles & NumberStyles.AllowLeadingWhite) == NumberStyles.None || ((num & 1) != 0 && (num & 32) == 0 && info.NumberNegativePattern != 2))
				{
					char* ptr2;
					if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = Number.MatchChars(ptr, strEnd, info.PositiveSign)) != null || ((ptr2 = Number.MatchChars(ptr, strEnd, info.NegativeSign)) != null && (number.IsNegative = true))))
					{
						num |= 1;
						ptr = ptr2 - 1;
					}
					else if (c == '(' && (styles & NumberStyles.AllowParentheses) != NumberStyles.None && (num & 1) == 0)
					{
						num |= 3;
						number.IsNegative = true;
					}
					else
					{
						if (text == null || (ptr2 = Number.MatchChars(ptr, strEnd, text)) == null)
						{
							break;
						}
						num |= 32;
						text = null;
						ptr = ptr2 - 1;
					}
				}
				c = ((++ptr < strEnd) ? (*ptr) : '\0');
			}
			int num2 = 0;
			int num3 = 0;
			int num4 = number.Digits.Length - 1;
			for (;;)
			{
				char* ptr2;
				if (Number.IsDigit((int)c))
				{
					num |= 4;
					if (c != '0' || (num & 8) != 0)
					{
						if (num2 < num4)
						{
							*number.Digits[num2++] = (byte)c;
							if (c != '0' || number.Kind != Number.NumberBufferKind.Integer)
							{
								num3 = num2;
							}
						}
						else if (c != '0')
						{
							number.HasNonZeroTail = true;
						}
						if ((num & 16) == 0)
						{
							number.Scale++;
						}
						num |= 8;
					}
					else if ((num & 16) != 0)
					{
						number.Scale--;
					}
				}
				else if ((styles & NumberStyles.AllowDecimalPoint) != NumberStyles.None && (num & 16) == 0 && ((ptr2 = Number.MatchChars(ptr, strEnd, value)) != null || (flag && (num & 32) == 0 && (ptr2 = Number.MatchChars(ptr, strEnd, info.NumberDecimalSeparator)) != null)))
				{
					num |= 16;
					ptr = ptr2 - 1;
				}
				else
				{
					if ((styles & NumberStyles.AllowThousands) == NumberStyles.None || (num & 4) == 0 || (num & 16) != 0 || ((ptr2 = Number.MatchChars(ptr, strEnd, value2)) == null && (!flag || (num & 32) != 0 || (ptr2 = Number.MatchChars(ptr, strEnd, info.NumberGroupSeparator)) == null)))
					{
						break;
					}
					ptr = ptr2 - 1;
				}
				c = ((++ptr < strEnd) ? (*ptr) : '\0');
			}
			bool flag2 = false;
			number.DigitsCount = num3;
			*number.Digits[num3] = 0;
			if ((num & 4) != 0)
			{
				if ((c == 'E' || c == 'e') && (styles & NumberStyles.AllowExponent) != NumberStyles.None)
				{
					char* ptr3 = ptr;
					c = ((++ptr < strEnd) ? (*ptr) : '\0');
					char* ptr2;
					if ((ptr2 = Number.MatchChars(ptr, strEnd, info.PositiveSign)) != null)
					{
						c = (((ptr = ptr2) < strEnd) ? (*ptr) : '\0');
					}
					else if ((ptr2 = Number.MatchChars(ptr, strEnd, info.NegativeSign)) != null)
					{
						c = (((ptr = ptr2) < strEnd) ? (*ptr) : '\0');
						flag2 = true;
					}
					if (Number.IsDigit((int)c))
					{
						int num5 = 0;
						do
						{
							num5 = num5 * 10 + (int)(c - '0');
							c = ((++ptr < strEnd) ? (*ptr) : '\0');
							if (num5 > 1000)
							{
								num5 = 9999;
								while (Number.IsDigit((int)c))
								{
									c = ((++ptr < strEnd) ? (*ptr) : '\0');
								}
							}
						}
						while (Number.IsDigit((int)c));
						if (flag2)
						{
							num5 = -num5;
						}
						number.Scale += num5;
					}
					else
					{
						ptr = ptr3;
						c = ((ptr < strEnd) ? (*ptr) : '\0');
					}
				}
				for (;;)
				{
					if (!Number.IsWhite((int)c) || (styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
					{
						char* ptr2;
						if ((styles & NumberStyles.AllowTrailingSign) != NumberStyles.None && (num & 1) == 0 && ((ptr2 = Number.MatchChars(ptr, strEnd, info.PositiveSign)) != null || ((ptr2 = Number.MatchChars(ptr, strEnd, info.NegativeSign)) != null && (number.IsNegative = true))))
						{
							num |= 1;
							ptr = ptr2 - 1;
						}
						else if (c == ')' && (num & 2) != 0)
						{
							num &= -3;
						}
						else
						{
							if (text == null || (ptr2 = Number.MatchChars(ptr, strEnd, text)) == null)
							{
								break;
							}
							text = null;
							ptr = ptr2 - 1;
						}
					}
					c = ((++ptr < strEnd) ? (*ptr) : '\0');
				}
				if ((num & 2) == 0)
				{
					if ((num & 8) == 0)
					{
						if (number.Kind != Number.NumberBufferKind.Decimal)
						{
							number.Scale = 0;
						}
						if (number.Kind == Number.NumberBufferKind.Integer && (num & 16) == 0)
						{
							number.IsNegative = false;
						}
					}
					str = ptr;
					return true;
				}
			}
			str = ptr;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Number.ParsingStatus TryParseInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				return Number.TryParseInt32IntegerStyle(value, styles, info, out result);
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				result = 0;
				return Number.TryParseUInt32HexNumberStyle(value, styles, Unsafe.As<int, uint>(ref result));
			}
			return Number.TryParseInt32Number(value, styles, info, out result);
		}

		private unsafe static Number.ParsingStatus TryParseInt32Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result)
		{
			result = 0;
			byte* digits = stackalloc byte[(UIntPtr)11];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 11);
			if (!Number.TryStringToNumber(value, styles, ref numberBuffer, info))
			{
				return Number.ParsingStatus.Failed;
			}
			if (!Number.TryNumberToInt32(ref numberBuffer, ref result))
			{
				return Number.ParsingStatus.Overflow;
			}
			return Number.ParsingStatus.OK;
		}

		internal unsafe static Number.ParsingStatus TryParseInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out int result)
		{
			if (value.IsEmpty)
			{
				goto IL_25A;
			}
			int num = 0;
			int num2 = (int)(*value[0]);
			if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(num2))
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_25A;
					}
					num2 = (int)(*value[num]);
				}
				while (Number.IsWhite(num2));
			}
			int num3 = 1;
			if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
			{
				if (info.HasInvariantNumberSigns())
				{
					if (num2 == 45)
					{
						num3 = -1;
						num++;
						if (num >= value.Length)
						{
							goto IL_25A;
						}
						num2 = (int)(*value[num]);
					}
					else if (num2 == 43)
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_25A;
						}
						num2 = (int)(*value[num]);
					}
				}
				else
				{
					value = value.Slice(num);
					num = 0;
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
					{
						num += positiveSign.Length;
						if (num >= value.Length)
						{
							goto IL_25A;
						}
						num2 = (int)(*value[num]);
					}
					else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
					{
						num3 = -1;
						num += negativeSign.Length;
						if (num >= value.Length)
						{
							goto IL_25A;
						}
						num2 = (int)(*value[num]);
					}
				}
			}
			bool flag = false;
			int num4 = 0;
			if (!Number.IsDigit(num2))
			{
				goto IL_25A;
			}
			if (num2 == 48)
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_24E;
					}
					num2 = (int)(*value[num]);
				}
				while (num2 == 48);
				if (!Number.IsDigit(num2))
				{
					goto IL_26A;
				}
			}
			num4 = num2 - 48;
			num++;
			for (int i = 0; i < 8; i++)
			{
				if (num >= value.Length)
				{
					goto IL_24E;
				}
				num2 = (int)(*value[num]);
				if (!Number.IsDigit(num2))
				{
					goto IL_26A;
				}
				num++;
				num4 = 10 * num4 + num2 - 48;
			}
			if (num >= value.Length)
			{
				goto IL_24E;
			}
			num2 = (int)(*value[num]);
			if (!Number.IsDigit(num2))
			{
				goto IL_26A;
			}
			num++;
			flag = (num4 > 214748364);
			num4 = num4 * 10 + num2 - 48;
			flag |= (num4 > (int)(2147483647U + ((uint)num3 >> 31)));
			if (num < value.Length)
			{
				num2 = (int)(*value[num]);
				while (Number.IsDigit(num2))
				{
					flag = true;
					num++;
					if (num >= value.Length)
					{
						goto IL_262;
					}
					num2 = (int)(*value[num]);
				}
				goto IL_26A;
			}
			IL_24B:
			if (flag)
			{
				goto IL_262;
			}
			IL_24E:
			result = num4 * num3;
			return Number.ParsingStatus.OK;
			IL_262:
			result = 0;
			return Number.ParsingStatus.Overflow;
			IL_26A:
			if (Number.IsWhite(num2))
			{
				if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
				{
					goto IL_25A;
				}
				num++;
				while (num < value.Length && Number.IsWhite((int)(*value[num])))
				{
					num++;
				}
				if (num >= value.Length)
				{
					goto IL_24B;
				}
			}
			if (!Number.TrailingZeros(value, num))
			{
				goto IL_25A;
			}
			goto IL_24B;
			IL_25A:
			result = 0;
			return Number.ParsingStatus.Failed;
		}

		internal unsafe static Number.ParsingStatus TryParseInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result)
		{
			if (value.IsEmpty)
			{
				goto IL_270;
			}
			int num = 0;
			int num2 = (int)(*value[0]);
			if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(num2))
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_270;
					}
					num2 = (int)(*value[num]);
				}
				while (Number.IsWhite(num2));
			}
			int num3 = 1;
			if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
			{
				if (info.HasInvariantNumberSigns())
				{
					if (num2 == 45)
					{
						num3 = -1;
						num++;
						if (num >= value.Length)
						{
							goto IL_270;
						}
						num2 = (int)(*value[num]);
					}
					else if (num2 == 43)
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_270;
						}
						num2 = (int)(*value[num]);
					}
				}
				else
				{
					value = value.Slice(num);
					num = 0;
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
					{
						num += positiveSign.Length;
						if (num >= value.Length)
						{
							goto IL_270;
						}
						num2 = (int)(*value[num]);
					}
					else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
					{
						num3 = -1;
						num += negativeSign.Length;
						if (num >= value.Length)
						{
							goto IL_270;
						}
						num2 = (int)(*value[num]);
					}
				}
			}
			bool flag = false;
			long num4 = 0L;
			if (!Number.IsDigit(num2))
			{
				goto IL_270;
			}
			if (num2 == 48)
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_263;
					}
					num2 = (int)(*value[num]);
				}
				while (num2 == 48);
				if (!Number.IsDigit(num2))
				{
					goto IL_282;
				}
			}
			num4 = (long)(num2 - 48);
			num++;
			for (int i = 0; i < 17; i++)
			{
				if (num >= value.Length)
				{
					goto IL_263;
				}
				num2 = (int)(*value[num]);
				if (!Number.IsDigit(num2))
				{
					goto IL_282;
				}
				num++;
				num4 = 10L * num4 + (long)num2 - 48L;
			}
			if (num >= value.Length)
			{
				goto IL_263;
			}
			num2 = (int)(*value[num]);
			if (!Number.IsDigit(num2))
			{
				goto IL_282;
			}
			num++;
			flag = (num4 > 922337203685477580L);
			num4 = num4 * 10L + (long)num2 - 48L;
			flag |= (num4 > (long)(9223372036854775807UL + (ulong)((uint)num3 >> 31)));
			if (num < value.Length)
			{
				num2 = (int)(*value[num]);
				while (Number.IsDigit(num2))
				{
					flag = true;
					num++;
					if (num >= value.Length)
					{
						goto IL_279;
					}
					num2 = (int)(*value[num]);
				}
				goto IL_282;
			}
			IL_260:
			if (flag)
			{
				goto IL_279;
			}
			IL_263:
			result = num4 * (long)num3;
			return Number.ParsingStatus.OK;
			IL_279:
			result = 0L;
			return Number.ParsingStatus.Overflow;
			IL_282:
			if (Number.IsWhite(num2))
			{
				if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
				{
					goto IL_270;
				}
				num++;
				while (num < value.Length && Number.IsWhite((int)(*value[num])))
				{
					num++;
				}
				if (num >= value.Length)
				{
					goto IL_260;
				}
			}
			if (!Number.TrailingZeros(value, num))
			{
				goto IL_270;
			}
			goto IL_260;
			IL_270:
			result = 0L;
			return Number.ParsingStatus.Failed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Number.ParsingStatus TryParseInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				return Number.TryParseInt64IntegerStyle(value, styles, info, out result);
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				result = 0L;
				return Number.TryParseUInt64HexNumberStyle(value, styles, Unsafe.As<long, ulong>(ref result));
			}
			return Number.TryParseInt64Number(value, styles, info, out result);
		}

		private unsafe static Number.ParsingStatus TryParseInt64Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out long result)
		{
			result = 0L;
			byte* digits = stackalloc byte[(UIntPtr)20];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 20);
			if (!Number.TryStringToNumber(value, styles, ref numberBuffer, info))
			{
				return Number.ParsingStatus.Failed;
			}
			if (!Number.TryNumberToInt64(ref numberBuffer, ref result))
			{
				return Number.ParsingStatus.Overflow;
			}
			return Number.ParsingStatus.OK;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Number.ParsingStatus TryParseUInt32(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				return Number.TryParseUInt32IntegerStyle(value, styles, info, out result);
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				return Number.TryParseUInt32HexNumberStyle(value, styles, out result);
			}
			return Number.TryParseUInt32Number(value, styles, info, out result);
		}

		private unsafe static Number.ParsingStatus TryParseUInt32Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result)
		{
			result = 0U;
			byte* digits = stackalloc byte[(UIntPtr)11];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 11);
			if (!Number.TryStringToNumber(value, styles, ref numberBuffer, info))
			{
				return Number.ParsingStatus.Failed;
			}
			if (!Number.TryNumberToUInt32(ref numberBuffer, ref result))
			{
				return Number.ParsingStatus.Overflow;
			}
			return Number.ParsingStatus.OK;
		}

		internal unsafe static Number.ParsingStatus TryParseUInt32IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out uint result)
		{
			if (value.IsEmpty)
			{
				goto IL_252;
			}
			int num = 0;
			int num2 = (int)(*value[0]);
			if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(num2))
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_252;
					}
					num2 = (int)(*value[num]);
				}
				while (Number.IsWhite(num2));
			}
			bool flag = false;
			if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
			{
				if (info.HasInvariantNumberSigns())
				{
					if (num2 == 43)
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_252;
						}
						num2 = (int)(*value[num]);
					}
					else if (num2 == 45)
					{
						flag = true;
						num++;
						if (num >= value.Length)
						{
							goto IL_252;
						}
						num2 = (int)(*value[num]);
					}
				}
				else
				{
					value = value.Slice(num);
					num = 0;
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
					{
						num += positiveSign.Length;
						if (num >= value.Length)
						{
							goto IL_252;
						}
						num2 = (int)(*value[num]);
					}
					else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
					{
						flag = true;
						num += negativeSign.Length;
						if (num >= value.Length)
						{
							goto IL_252;
						}
						num2 = (int)(*value[num]);
					}
				}
			}
			int num3 = 0;
			if (!Number.IsDigit(num2))
			{
				goto IL_252;
			}
			if (num2 == 48)
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_249;
					}
					num2 = (int)(*value[num]);
				}
				while (num2 == 48);
				if (!Number.IsDigit(num2))
				{
					flag = false;
					goto IL_264;
				}
			}
			num3 = num2 - 48;
			num++;
			for (int i = 0; i < 8; i++)
			{
				if (num >= value.Length)
				{
					goto IL_246;
				}
				num2 = (int)(*value[num]);
				if (!Number.IsDigit(num2))
				{
					goto IL_264;
				}
				num++;
				num3 = 10 * num3 + num2 - 48;
			}
			if (num < value.Length)
			{
				num2 = (int)(*value[num]);
				if (!Number.IsDigit(num2))
				{
					goto IL_264;
				}
				num++;
				flag |= (num3 > 429496729 || (num3 == 429496729 && num2 > 53));
				num3 = num3 * 10 + num2 - 48;
				if (num < value.Length)
				{
					num2 = (int)(*value[num]);
					while (Number.IsDigit(num2))
					{
						flag = true;
						num++;
						if (num >= value.Length)
						{
							goto IL_25A;
						}
						num2 = (int)(*value[num]);
					}
					goto IL_264;
				}
			}
			IL_246:
			if (flag)
			{
				goto IL_25A;
			}
			IL_249:
			result = (uint)num3;
			return Number.ParsingStatus.OK;
			IL_25A:
			result = 0U;
			return Number.ParsingStatus.Overflow;
			IL_264:
			if (Number.IsWhite(num2))
			{
				if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
				{
					goto IL_252;
				}
				num++;
				while (num < value.Length && Number.IsWhite((int)(*value[num])))
				{
					num++;
				}
				if (num >= value.Length)
				{
					goto IL_246;
				}
			}
			if (!Number.TrailingZeros(value, num))
			{
				goto IL_252;
			}
			goto IL_246;
			IL_252:
			result = 0U;
			return Number.ParsingStatus.Failed;
		}

		private unsafe static Number.ParsingStatus TryParseUInt32HexNumberStyle(ReadOnlySpan<char> value, NumberStyles styles, out uint result)
		{
			if (value.IsEmpty)
			{
				goto IL_18C;
			}
			int num = 0;
			int num2 = (int)(*value[0]);
			if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(num2))
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_18C;
					}
					num2 = (int)(*value[num]);
				}
				while (Number.IsWhite(num2));
			}
			bool flag = false;
			uint num3 = 0U;
			ReadOnlySpan<byte> charToHexLookup = Number.CharToHexLookup;
			if (num2 >= charToHexLookup.Length || *charToHexLookup[num2] == 255)
			{
				goto IL_18C;
			}
			if (num2 == 48)
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_183;
					}
					num2 = (int)(*value[num]);
				}
				while (num2 == 48);
				if (num2 >= charToHexLookup.Length || *charToHexLookup[num2] == 255)
				{
					goto IL_19C;
				}
			}
			num3 = (uint)(*charToHexLookup[num2]);
			num++;
			for (int i = 0; i < 7; i++)
			{
				if (num >= value.Length)
				{
					goto IL_183;
				}
				num2 = (int)(*value[num]);
				uint num4;
				if (num2 >= charToHexLookup.Length || (num4 = (uint)(*charToHexLookup[num2])) == 255U)
				{
					goto IL_19C;
				}
				num++;
				num3 = 16U * num3 + num4;
			}
			if (num < value.Length)
			{
				num2 = (int)(*value[num]);
				if (num2 < charToHexLookup.Length && *charToHexLookup[num2] != 255)
				{
					do
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_194;
						}
						num2 = (int)(*value[num]);
					}
					while (num2 < charToHexLookup.Length && *charToHexLookup[num2] != 255);
					flag = true;
				}
				goto IL_19C;
			}
			IL_183:
			result = num3;
			return Number.ParsingStatus.OK;
			IL_194:
			result = 0U;
			return Number.ParsingStatus.Overflow;
			IL_19C:
			if (!Number.IsWhite(num2))
			{
				goto IL_1D7;
			}
			if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
			{
				goto IL_18C;
			}
			num++;
			while (num < value.Length && Number.IsWhite((int)(*value[num])))
			{
				num++;
			}
			if (num < value.Length)
			{
				goto IL_1D7;
			}
			IL_180:
			if (!flag)
			{
				goto IL_183;
			}
			goto IL_194;
			IL_1D7:
			if (!Number.TrailingZeros(value, num))
			{
				goto IL_18C;
			}
			goto IL_180;
			IL_18C:
			result = 0U;
			return Number.ParsingStatus.Failed;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Number.ParsingStatus TryParseUInt64(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result)
		{
			if ((styles & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign)) == NumberStyles.None)
			{
				return Number.TryParseUInt64IntegerStyle(value, styles, info, out result);
			}
			if ((styles & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
			{
				return Number.TryParseUInt64HexNumberStyle(value, styles, out result);
			}
			return Number.TryParseUInt64Number(value, styles, info, out result);
		}

		private unsafe static Number.ParsingStatus TryParseUInt64Number(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result)
		{
			result = 0UL;
			byte* digits = stackalloc byte[(UIntPtr)21];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Integer, digits, 21);
			if (!Number.TryStringToNumber(value, styles, ref numberBuffer, info))
			{
				return Number.ParsingStatus.Failed;
			}
			if (!Number.TryNumberToUInt64(ref numberBuffer, ref result))
			{
				return Number.ParsingStatus.Overflow;
			}
			return Number.ParsingStatus.OK;
		}

		internal unsafe static Number.ParsingStatus TryParseUInt64IntegerStyle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out ulong result)
		{
			if (value.IsEmpty)
			{
				goto IL_266;
			}
			int num = 0;
			int num2 = (int)(*value[0]);
			if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(num2))
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_266;
					}
					num2 = (int)(*value[num]);
				}
				while (Number.IsWhite(num2));
			}
			bool flag = false;
			if ((styles & NumberStyles.AllowLeadingSign) != NumberStyles.None)
			{
				if (info.HasInvariantNumberSigns())
				{
					if (num2 == 43)
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_266;
						}
						num2 = (int)(*value[num]);
					}
					else if (num2 == 45)
					{
						flag = true;
						num++;
						if (num >= value.Length)
						{
							goto IL_266;
						}
						num2 = (int)(*value[num]);
					}
				}
				else
				{
					value = value.Slice(num);
					num = 0;
					string positiveSign = info.PositiveSign;
					string negativeSign = info.NegativeSign;
					if (!string.IsNullOrEmpty(positiveSign) && value.StartsWith(positiveSign.AsSpan()))
					{
						num += positiveSign.Length;
						if (num >= value.Length)
						{
							goto IL_266;
						}
						num2 = (int)(*value[num]);
					}
					else if (!string.IsNullOrEmpty(negativeSign) && value.StartsWith(negativeSign.AsSpan()))
					{
						flag = true;
						num += negativeSign.Length;
						if (num >= value.Length)
						{
							goto IL_266;
						}
						num2 = (int)(*value[num]);
					}
				}
			}
			long num3 = 0L;
			if (!Number.IsDigit(num2))
			{
				goto IL_266;
			}
			if (num2 == 48)
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_25D;
					}
					num2 = (int)(*value[num]);
				}
				while (num2 == 48);
				if (!Number.IsDigit(num2))
				{
					flag = false;
					goto IL_27A;
				}
			}
			num3 = (long)(num2 - 48);
			num++;
			for (int i = 0; i < 18; i++)
			{
				if (num >= value.Length)
				{
					goto IL_25A;
				}
				num2 = (int)(*value[num]);
				if (!Number.IsDigit(num2))
				{
					goto IL_27A;
				}
				num++;
				num3 = 10L * num3 + (long)num2 - 48L;
			}
			if (num < value.Length)
			{
				num2 = (int)(*value[num]);
				if (!Number.IsDigit(num2))
				{
					goto IL_27A;
				}
				num++;
				flag |= (num3 > 1844674407370955161L || (num3 == 1844674407370955161L && num2 > 53));
				num3 = num3 * 10L + (long)num2 - 48L;
				if (num < value.Length)
				{
					num2 = (int)(*value[num]);
					while (Number.IsDigit(num2))
					{
						flag = true;
						num++;
						if (num >= value.Length)
						{
							goto IL_26F;
						}
						num2 = (int)(*value[num]);
					}
					goto IL_27A;
				}
			}
			IL_25A:
			if (flag)
			{
				goto IL_26F;
			}
			IL_25D:
			result = (ulong)num3;
			return Number.ParsingStatus.OK;
			IL_26F:
			result = 0UL;
			return Number.ParsingStatus.Overflow;
			IL_27A:
			if (Number.IsWhite(num2))
			{
				if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
				{
					goto IL_266;
				}
				num++;
				while (num < value.Length && Number.IsWhite((int)(*value[num])))
				{
					num++;
				}
				if (num >= value.Length)
				{
					goto IL_25A;
				}
			}
			if (!Number.TrailingZeros(value, num))
			{
				goto IL_266;
			}
			goto IL_25A;
			IL_266:
			result = 0UL;
			return Number.ParsingStatus.Failed;
		}

		private unsafe static Number.ParsingStatus TryParseUInt64HexNumberStyle(ReadOnlySpan<char> value, NumberStyles styles, out ulong result)
		{
			if (value.IsEmpty)
			{
				goto IL_191;
			}
			int num = 0;
			int num2 = (int)(*value[0]);
			if ((styles & NumberStyles.AllowLeadingWhite) != NumberStyles.None && Number.IsWhite(num2))
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_191;
					}
					num2 = (int)(*value[num]);
				}
				while (Number.IsWhite(num2));
			}
			bool flag = false;
			ulong num3 = 0UL;
			ReadOnlySpan<byte> charToHexLookup = Number.CharToHexLookup;
			if (num2 >= charToHexLookup.Length || *charToHexLookup[num2] == 255)
			{
				goto IL_191;
			}
			if (num2 == 48)
			{
				do
				{
					num++;
					if (num >= value.Length)
					{
						goto IL_188;
					}
					num2 = (int)(*value[num]);
				}
				while (num2 == 48);
				if (num2 >= charToHexLookup.Length || *charToHexLookup[num2] == 255)
				{
					goto IL_1A3;
				}
			}
			num3 = (ulong)(*charToHexLookup[num2]);
			num++;
			for (int i = 0; i < 15; i++)
			{
				if (num >= value.Length)
				{
					goto IL_188;
				}
				num2 = (int)(*value[num]);
				uint num4;
				if (num2 >= charToHexLookup.Length || (num4 = (uint)(*charToHexLookup[num2])) == 255U)
				{
					goto IL_1A3;
				}
				num++;
				num3 = 16UL * num3 + (ulong)num4;
			}
			if (num < value.Length)
			{
				num2 = (int)(*value[num]);
				if (num2 < charToHexLookup.Length && *charToHexLookup[num2] != 255)
				{
					do
					{
						num++;
						if (num >= value.Length)
						{
							goto IL_19A;
						}
						num2 = (int)(*value[num]);
					}
					while (num2 < charToHexLookup.Length && *charToHexLookup[num2] != 255);
					flag = true;
				}
				goto IL_1A3;
			}
			IL_188:
			result = num3;
			return Number.ParsingStatus.OK;
			IL_19A:
			result = 0UL;
			return Number.ParsingStatus.Overflow;
			IL_1A3:
			if (!Number.IsWhite(num2))
			{
				goto IL_1DE;
			}
			if ((styles & NumberStyles.AllowTrailingWhite) == NumberStyles.None)
			{
				goto IL_191;
			}
			num++;
			while (num < value.Length && Number.IsWhite((int)(*value[num])))
			{
				num++;
			}
			if (num < value.Length)
			{
				goto IL_1DE;
			}
			IL_185:
			if (!flag)
			{
				goto IL_188;
			}
			goto IL_19A;
			IL_1DE:
			if (!Number.TrailingZeros(value, num))
			{
				goto IL_191;
			}
			goto IL_185;
			IL_191:
			result = 0UL;
			return Number.ParsingStatus.Failed;
		}

		internal static decimal ParseDecimal(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			decimal result;
			Number.ParsingStatus parsingStatus = Number.TryParseDecimal(value, styles, info, out result);
			if (parsingStatus != Number.ParsingStatus.OK)
			{
				Number.ThrowOverflowOrFormatException(parsingStatus, TypeCode.Decimal);
			}
			return result;
		}

		internal unsafe static bool TryNumberToDecimal(ref Number.NumberBuffer number, ref decimal value)
		{
			byte* ptr = number.GetDigitsPointer();
			int i = number.Scale;
			bool isNegative = number.IsNegative;
			uint num = (uint)(*ptr);
			if (num == 0U)
			{
				value = new decimal(0, 0, 0, isNegative, (byte)MathEx.Clamp(-i, 0, 28));
				return true;
			}
			if (i > 29)
			{
				return false;
			}
			ulong num2 = 0UL;
			while (i > -28)
			{
				i--;
				num2 *= 10UL;
				num2 += (ulong)(num - 48U);
				num = (uint)(*(++ptr));
				if (num2 >= 1844674407370955161UL)
				{
					break;
				}
				if (num == 0U)
				{
					while (i > 0)
					{
						i--;
						num2 *= 10UL;
						if (num2 >= 1844674407370955161UL)
						{
							break;
						}
					}
					break;
				}
			}
			uint num3 = 0U;
			while ((i > 0 || (num != 0U && i > -28)) && (num3 < 429496729U || (num3 == 429496729U && (num2 < 11068046444225730969UL || (num2 == 11068046444225730969UL && num <= 53U)))))
			{
				ulong num4 = (ulong)((uint)num2) * 10UL;
				ulong num5 = (ulong)((uint)(num2 >> 32)) * 10UL + (num4 >> 32);
				num2 = (ulong)((uint)num4) + (num5 << 32);
				num3 = (uint)(num5 >> 32) + num3 * 10U;
				if (num != 0U)
				{
					num -= 48U;
					num2 += (ulong)num;
					if (num2 < (ulong)num)
					{
						num3 += 1U;
					}
					num = (uint)(*(++ptr));
				}
				i--;
			}
			if (num >= 53U)
			{
				if (num == 53U && (num2 & 1UL) == 0UL)
				{
					num = (uint)(*(++ptr));
					bool flag = !number.HasNonZeroTail;
					while (num > 0U && flag)
					{
						flag &= (num == 48U);
						num = (uint)(*(++ptr));
					}
					if (flag)
					{
						goto IL_1A8;
					}
				}
				if ((num2 += 1UL) == 0UL && (num3 += 1U) == 0U)
				{
					num2 = 11068046444225730970UL;
					num3 = 429496729U;
					i++;
				}
			}
			IL_1A8:
			if (i > 0)
			{
				return false;
			}
			if (i <= -29)
			{
				value = new decimal(0, 0, 0, isNegative, 28);
			}
			else
			{
				value = new decimal((int)num2, (int)(num2 >> 32), (int)num3, isNegative, (byte)(-(byte)i));
			}
			return true;
		}

		internal static double ParseDouble(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			double result;
			if (!Number.TryParseDouble(value, styles, info, out result))
			{
				Number.ThrowOverflowOrFormatException(Number.ParsingStatus.Failed, TypeCode.Empty);
			}
			return result;
		}

		internal static float ParseSingle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info)
		{
			float result;
			if (!Number.TryParseSingle(value, styles, info, out result))
			{
				Number.ThrowOverflowOrFormatException(Number.ParsingStatus.Failed, TypeCode.Empty);
			}
			return result;
		}

		internal unsafe static Number.ParsingStatus TryParseDecimal(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out decimal result)
		{
			byte* digits = stackalloc byte[(UIntPtr)31];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.Decimal, digits, 31);
			result = 0m;
			if (!Number.TryStringToNumber(value, styles, ref numberBuffer, info))
			{
				return Number.ParsingStatus.Failed;
			}
			if (!Number.TryNumberToDecimal(ref numberBuffer, ref result))
			{
				return Number.ParsingStatus.Overflow;
			}
			return Number.ParsingStatus.OK;
		}

		internal unsafe static bool TryParseDouble(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out double result)
		{
			byte* digits = stackalloc byte[(UIntPtr)769];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, digits, 769);
			if (!Number.TryStringToNumber(value, styles, ref numberBuffer, info))
			{
				ReadOnlySpan<char> span = value.Trim();
				if (span.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol.AsSpan()))
				{
					result = double.PositiveInfinity;
				}
				else if (span.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol.AsSpan()))
				{
					result = double.NegativeInfinity;
				}
				else if (span.EqualsOrdinalIgnoreCase(info.NaNSymbol.AsSpan()))
				{
					result = double.NaN;
				}
				else if (span.StartsWith(info.PositiveSign.AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					span = span.Slice(info.PositiveSign.Length);
					if (span.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol.AsSpan()))
					{
						result = double.PositiveInfinity;
					}
					else
					{
						if (!span.EqualsOrdinalIgnoreCase(info.NaNSymbol.AsSpan()))
						{
							result = 0.0;
							return false;
						}
						result = double.NaN;
					}
				}
				else
				{
					if (!span.StartsWith(info.NegativeSign.AsSpan(), StringComparison.OrdinalIgnoreCase) || !span.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol.AsSpan()))
					{
						result = 0.0;
						return false;
					}
					result = double.NaN;
				}
			}
			else
			{
				result = Number.NumberToDouble(ref numberBuffer);
			}
			return true;
		}

		internal unsafe static bool TryParseSingle(ReadOnlySpan<char> value, NumberStyles styles, NumberFormatInfo info, out float result)
		{
			byte* digits = stackalloc byte[(UIntPtr)114];
			Number.NumberBuffer numberBuffer = new Number.NumberBuffer(Number.NumberBufferKind.FloatingPoint, digits, 114);
			if (!Number.TryStringToNumber(value, styles, ref numberBuffer, info))
			{
				ReadOnlySpan<char> span = value.Trim();
				if (span.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol.AsSpan()))
				{
					result = float.PositiveInfinity;
				}
				else if (span.EqualsOrdinalIgnoreCase(info.NegativeInfinitySymbol.AsSpan()))
				{
					result = float.NegativeInfinity;
				}
				else if (span.EqualsOrdinalIgnoreCase(info.NaNSymbol.AsSpan()))
				{
					result = float.NaN;
				}
				else if (span.StartsWith(info.PositiveSign.AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					span = span.Slice(info.PositiveSign.Length);
					if (!info.PositiveInfinitySymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) && span.EqualsOrdinalIgnoreCase(info.PositiveInfinitySymbol.AsSpan()))
					{
						result = float.PositiveInfinity;
					}
					else
					{
						if (info.NaNSymbol.StartsWith(info.PositiveSign, StringComparison.OrdinalIgnoreCase) || !span.EqualsOrdinalIgnoreCase(info.NaNSymbol.AsSpan()))
						{
							result = 0f;
							return false;
						}
						result = float.NaN;
					}
				}
				else
				{
					if (!span.StartsWith(info.NegativeSign.AsSpan(), StringComparison.OrdinalIgnoreCase) || info.NaNSymbol.StartsWith(info.NegativeSign, StringComparison.OrdinalIgnoreCase) || !span.Slice(info.NegativeSign.Length).EqualsOrdinalIgnoreCase(info.NaNSymbol.AsSpan()))
					{
						result = 0f;
						return false;
					}
					result = float.NaN;
				}
			}
			else
			{
				result = Number.NumberToSingle(ref numberBuffer);
			}
			return true;
		}

		internal unsafe static bool TryStringToNumber(ReadOnlySpan<char> value, NumberStyles styles, ref Number.NumberBuffer number, NumberFormatInfo info)
		{
			fixed (char* reference = MemoryMarshal.GetReference<char>(value))
			{
				char* ptr = reference;
				char* ptr2 = ptr;
				if (!Number.TryParseNumber(ref ptr2, ptr2 + value.Length, styles, ref number, info) || ((int)((long)(ptr2 - ptr)) < value.Length && !Number.TrailingZeros(value, (int)((long)(ptr2 - ptr)))))
				{
					return false;
				}
			}
			return true;
		}

		private unsafe static bool TrailingZeros(ReadOnlySpan<char> value, int index)
		{
			for (int i = index; i < value.Length; i++)
			{
				if (*value[i] != 0)
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsSpaceReplacingChar(char c)
		{
			return c == '\u00a0' || c == '\u202f';
		}

		private unsafe static char* MatchChars(char* p, char* pEnd, string value)
		{
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = ptr;
				if (*ptr2 != '\0')
				{
					do
					{
						char c = (p < pEnd) ? (*p) : '\0';
						if (c != *ptr2 && (!Number.IsSpaceReplacingChar(*ptr2) || c != ' '))
						{
							goto IL_42;
						}
						p++;
						ptr2++;
					}
					while (*ptr2 != '\0');
					return p;
				}
				IL_42:;
			}
			return null;
		}

		private static bool IsWhite(int ch)
		{
			return ch == 32 || ch - 9 <= 4;
		}

		private static bool IsDigit(int ch)
		{
			return ch - 48 <= 9;
		}

		internal static void ThrowOverflowOrFormatException(Number.ParsingStatus status, TypeCode type = TypeCode.Empty)
		{
			throw Number.GetException(status, type);
		}

		internal static void ThrowOverflowException(TypeCode type)
		{
			throw Number.GetException(Number.ParsingStatus.Overflow, type);
		}

		private static Exception GetException(Number.ParsingStatus status, TypeCode type)
		{
			if (status == Number.ParsingStatus.Failed)
			{
				return new FormatException();
			}
			string message;
			switch (type)
			{
			case TypeCode.SByte:
				message = "SR.Overflow_SByte";
				break;
			case TypeCode.Byte:
				message = "SR.Overflow_Byte";
				break;
			case TypeCode.Int16:
				message = "SR.Overflow_Int16";
				break;
			case TypeCode.UInt16:
				message = "SR.Overflow_UInt16";
				break;
			case TypeCode.Int32:
				message = "SR.Overflow_Int32";
				break;
			case TypeCode.UInt32:
				message = "SR.Overflow_UInt32";
				break;
			case TypeCode.Int64:
				message = "SR.Overflow_Int64";
				break;
			case TypeCode.UInt64:
				message = "SR.Overflow_UInt64";
				break;
			default:
				message = "SR.Overflow_Decimal";
				break;
			}
			return new OverflowException(message);
		}

		internal static double NumberToDouble(ref Number.NumberBuffer number)
		{
			double num;
			if (number.DigitsCount == 0 || number.Scale < -324)
			{
				num = 0.0;
			}
			else if (number.Scale > 309)
			{
				num = double.PositiveInfinity;
			}
			else
			{
				num = BitConverter.Int64BitsToDouble((long)Number.NumberToFloatingPointBits(ref number, Number.FloatingPointInfo.Double));
			}
			if (!number.IsNegative)
			{
				return num;
			}
			return -num;
		}

		internal static float NumberToSingle(ref Number.NumberBuffer number)
		{
			float num;
			if (number.DigitsCount == 0 || number.Scale < -45)
			{
				num = 0f;
			}
			else if (number.Scale > 39)
			{
				num = float.PositiveInfinity;
			}
			else
			{
				num = Number.Int32BitsToSingle((int)((uint)Number.NumberToFloatingPointBits(ref number, Number.FloatingPointInfo.Single)));
			}
			if (!number.IsNegative)
			{
				return num;
			}
			return -num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static float Int32BitsToSingle(int value)
		{
			return *(float*)(&value);
		}

		[CompilerGenerated]
		internal unsafe static bool <RoundNumber>g__ShouldRoundUp|70_0(byte* _dig, int _i, Number.NumberBufferKind numberKind, bool _isCorrectlyRounded)
		{
			byte b = _dig[_i];
			return b != 0 && !_isCorrectlyRounded && b >= 53;
		}

		internal const int DecimalPrecision = 29;

		private const int SinglePrecision = 9;

		private const int DoublePrecision = 17;

		private const int SinglePrecisionCustomFormat = 7;

		private const int DoublePrecisionCustomFormat = 15;

		private const int DefaultPrecisionExponentialFormat = 6;

		private const int MaxUInt32DecDigits = 10;

		private const int CharStackBufferSize = 32;

		private const string PosNumberFormat = "#";

		private static readonly string[] s_singleDigitStringCache = new string[]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9"
		};

		private static readonly string[] s_posCurrencyFormats = new string[]
		{
			"$#",
			"#$",
			"$ #",
			"# $"
		};

		private static readonly string[] s_negCurrencyFormats = new string[]
		{
			"($#)",
			"-$#",
			"$-#",
			"$#-",
			"(#$)",
			"-#$",
			"#-$",
			"#$-",
			"-# $",
			"-$ #",
			"# $-",
			"$ #-",
			"$ -#",
			"#- $",
			"($ #)",
			"(# $)"
		};

		private static readonly string[] s_posPercentFormats = new string[]
		{
			"# %",
			"#%",
			"%#",
			"% #"
		};

		private static readonly string[] s_negPercentFormats = new string[]
		{
			"-# %",
			"-#%",
			"-%#",
			"%-#",
			"%#-",
			"#-%",
			"#%-",
			"-% #",
			"# %-",
			"% #-",
			"% -#",
			"#- %"
		};

		private static readonly string[] s_negNumberFormats = new string[]
		{
			"(#)",
			"-#",
			"- #",
			"#-",
			"# -"
		};

		internal const int DecimalNumberBufferLength = 31;

		internal const int DoubleNumberBufferLength = 769;

		internal const int Int32NumberBufferLength = 11;

		internal const int Int64NumberBufferLength = 20;

		internal const int SingleNumberBufferLength = 114;

		internal const int UInt32NumberBufferLength = 11;

		internal const int UInt64NumberBufferLength = 21;

		[Nullable(1)]
		private static readonly float[] s_Pow10SingleTable = new float[]
		{
			1f,
			10f,
			100f,
			1000f,
			10000f,
			100000f,
			1000000f,
			10000000f,
			100000000f,
			1E+09f,
			1E+10f
		};

		[Nullable(1)]
		private static readonly double[] s_Pow10DoubleTable = new double[]
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
			1000000000000000.0,
			10000000000000000.0,
			1E+17,
			1E+18,
			1E+19,
			1E+20,
			1E+21,
			1E+22
		};

		private const int Int32Precision = 10;

		private const int UInt32Precision = 10;

		private const int Int64Precision = 19;

		private const int UInt64Precision = 20;

		private const int DoubleMaxExponent = 309;

		private const int DoubleMinExponent = -324;

		private const int FloatingPointMaxExponent = 309;

		private const int FloatingPointMinExponent = -324;

		private const int SingleMaxExponent = 39;

		private const int SingleMinExponent = -45;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal ref struct BigInteger
		{
			public unsafe static void Add(ref Number.BigInteger lhs, ref Number.BigInteger rhs, out Number.BigInteger result)
			{
				ref Number.BigInteger ptr = ref (lhs._length < rhs._length) ? ref rhs : ref lhs;
				ref Number.BigInteger ptr2 = ref (lhs._length < rhs._length) ? ref lhs : ref rhs;
				int length = ptr._length;
				int length2 = ptr2._length;
				result._length = length;
				ulong num = 0UL;
				int i = 0;
				int j = 0;
				int num2 = 0;
				while (j < length2)
				{
					ulong num3 = num + (ulong)(*(ref ptr._blocks.FixedElementField + (IntPtr)i * 4)) + (ulong)(*(ref ptr2._blocks.FixedElementField + (IntPtr)j * 4));
					num = num3 >> 32;
					*(ref result._blocks.FixedElementField + (IntPtr)num2 * 4) = (uint)num3;
					i++;
					j++;
					num2++;
				}
				while (i < length)
				{
					ulong num4 = num + (ulong)(*(ref ptr._blocks.FixedElementField + (IntPtr)i * 4));
					num = num4 >> 32;
					*(ref result._blocks.FixedElementField + (IntPtr)num2 * 4) = (uint)num4;
					i++;
					num2++;
				}
				if (num != 0UL)
				{
					*(ref result._blocks.FixedElementField + (IntPtr)num2 * 4) = 1U;
					result._length++;
				}
			}

			public unsafe static int Compare(ref Number.BigInteger lhs, ref Number.BigInteger rhs)
			{
				int length = lhs._length;
				int length2 = rhs._length;
				int num = length - length2;
				if (num != 0)
				{
					return num;
				}
				if (length == 0)
				{
					return 0;
				}
				int i = length - 1;
				while (i >= 0)
				{
					long num2 = (long)((ulong)(*(ref lhs._blocks.FixedElementField + (IntPtr)i * 4)) - (ulong)(*(ref rhs._blocks.FixedElementField + (IntPtr)i * 4)));
					if (num2 != 0L)
					{
						if (num2 <= 0L)
						{
							return -1;
						}
						return 1;
					}
					else
					{
						i--;
					}
				}
				return 0;
			}

			public static uint CountSignificantBits(uint value)
			{
				return (uint)(32 - BitOperations.LeadingZeroCount(value));
			}

			public static uint CountSignificantBits(ulong value)
			{
				return (uint)(64 - BitOperations.LeadingZeroCount(value));
			}

			public unsafe static uint CountSignificantBits(ref Number.BigInteger value)
			{
				if (value.IsZero())
				{
					return 0U;
				}
				uint num = (uint)(value._length - 1);
				return num * 32U + Number.BigInteger.CountSignificantBits(*(ref value._blocks.FixedElementField + (IntPtr)((ulong)num * 4UL)));
			}

			public unsafe static void DivRem(ref Number.BigInteger lhs, ref Number.BigInteger rhs, out Number.BigInteger quo, out Number.BigInteger rem)
			{
				if (lhs.IsZero())
				{
					Number.BigInteger.SetZero(out quo);
					Number.BigInteger.SetZero(out rem);
					return;
				}
				int length = lhs._length;
				int length2 = rhs._length;
				if (length == 1 && length2 == 1)
				{
					uint value2;
					uint value = MathEx.DivRem(lhs._blocks.FixedElementField, rhs._blocks.FixedElementField, out value2);
					Number.BigInteger.SetUInt32(out quo, value);
					Number.BigInteger.SetUInt32(out rem, value2);
					return;
				}
				if (length2 == 1)
				{
					int num = length;
					ulong b = (ulong)rhs._blocks.FixedElementField;
					ulong num2 = 0UL;
					for (int i = num - 1; i >= 0; i--)
					{
						ulong num3 = MathEx.DivRem(num2 << 32 | (ulong)(*(ref lhs._blocks.FixedElementField + (IntPtr)i * 4)), b, out num2);
						if (num3 == 0UL && i == num - 1)
						{
							num--;
						}
						else
						{
							*(ref quo._blocks.FixedElementField + (IntPtr)i * 4) = (uint)num3;
						}
					}
					quo._length = num;
					Number.BigInteger.SetUInt32(out rem, (uint)num2);
					return;
				}
				if (length2 > length)
				{
					Number.BigInteger.SetZero(out quo);
					Number.BigInteger.SetValue(out rem, ref lhs);
					return;
				}
				int num4 = length - length2 + 1;
				Number.BigInteger.SetValue(out rem, ref lhs);
				int num5 = length;
				uint num6 = *(ref rhs._blocks.FixedElementField + (IntPtr)(length2 - 1) * 4);
				uint num7 = *(ref rhs._blocks.FixedElementField + (IntPtr)(length2 - 2) * 4);
				int num8 = BitOperations.LeadingZeroCount(num6);
				int num9 = 32 - num8;
				if (num8 > 0)
				{
					num6 = (num6 << num8 | num7 >> num9);
					num7 <<= num8;
					if (length2 > 2)
					{
						num7 |= *(ref rhs._blocks.FixedElementField + (IntPtr)(length2 - 3) * 4) >> num9;
					}
				}
				for (int j = length; j >= length2; j--)
				{
					int num10 = j - length2;
					uint num11 = (j < length) ? (*(ref rem._blocks.FixedElementField + (IntPtr)j * 4)) : 0U;
					ulong num12 = (ulong)num11 << 32 | (ulong)(*(ref rem._blocks.FixedElementField + (IntPtr)(j - 1) * 4));
					uint num13 = (j > 1) ? (*(ref rem._blocks.FixedElementField + (IntPtr)(j - 2) * 4)) : 0U;
					if (num8 > 0)
					{
						num12 = (num12 << num8 | (ulong)(num13 >> num9));
						num13 <<= num8;
						if (j > 2)
						{
							num13 |= *(ref rem._blocks.FixedElementField + (IntPtr)(j - 3) * 4) >> num9;
						}
					}
					ulong num14 = num12 / (ulong)num6;
					if (num14 > (ulong)-1)
					{
						num14 = (ulong)-1;
					}
					while (Number.BigInteger.DivideGuessTooBig(num14, num12, num13, num6, num7))
					{
						num14 -= 1UL;
					}
					if (num14 > 0UL && Number.BigInteger.SubtractDivisor(ref rem, num10, ref rhs, num14) != num11)
					{
						Number.BigInteger.AddDivisor(ref rem, num10, ref rhs);
						num14 -= 1UL;
					}
					if (num4 != 0)
					{
						if (num14 == 0UL && num10 == num4 - 1)
						{
							num4--;
						}
						else
						{
							*(ref quo._blocks.FixedElementField + (IntPtr)num10 * 4) = (uint)num14;
						}
					}
					if (j < num5)
					{
						num5--;
					}
				}
				quo._length = num4;
				for (int k = num5 - 1; k >= 0; k--)
				{
					if (*(ref rem._blocks.FixedElementField + (IntPtr)k * 4) == 0U)
					{
						num5--;
					}
				}
				rem._length = num5;
			}

			public unsafe static uint HeuristicDivide(ref Number.BigInteger dividend, ref Number.BigInteger divisor)
			{
				int num = divisor._length;
				if (dividend._length < num)
				{
					return 0U;
				}
				int num2 = num - 1;
				uint num3 = *(ref dividend._blocks.FixedElementField + (IntPtr)num2 * 4) / (*(ref divisor._blocks.FixedElementField + (IntPtr)num2 * 4) + 1U);
				if (num3 != 0U)
				{
					int num4 = 0;
					ulong num5 = 0UL;
					ulong num6 = 0UL;
					do
					{
						ulong num7 = (ulong)(*(ref divisor._blocks.FixedElementField + (IntPtr)num4 * 4)) * (ulong)num3 + num6;
						num6 = num7 >> 32;
						ulong num8 = (ulong)(*(ref dividend._blocks.FixedElementField + (IntPtr)num4 * 4)) - (ulong)((uint)num7) - num5;
						num5 = (num8 >> 32 & 1UL);
						*(ref dividend._blocks.FixedElementField + (IntPtr)num4 * 4) = (uint)num8;
						num4++;
					}
					while (num4 < num);
					while (num > 0 && *(ref dividend._blocks.FixedElementField + (IntPtr)(num - 1) * 4) == 0U)
					{
						num--;
					}
					dividend._length = num;
				}
				if (Number.BigInteger.Compare(ref dividend, ref divisor) >= 0)
				{
					num3 += 1U;
					int num9 = 0;
					ulong num10 = 0UL;
					do
					{
						ulong num11 = (ulong)(*(ref dividend._blocks.FixedElementField + (IntPtr)num9 * 4)) - (ulong)(*(ref divisor._blocks.FixedElementField + (IntPtr)num9 * 4)) - num10;
						num10 = (num11 >> 32 & 1UL);
						*(ref dividend._blocks.FixedElementField + (IntPtr)num9 * 4) = (uint)num11;
						num9++;
					}
					while (num9 < num);
					while (num > 0 && *(ref dividend._blocks.FixedElementField + (IntPtr)(num - 1) * 4) == 0U)
					{
						num--;
					}
					dividend._length = num;
				}
				return num3;
			}

			public unsafe static void Multiply(ref Number.BigInteger lhs, uint value, out Number.BigInteger result)
			{
				if (lhs.IsZero() || value == 1U)
				{
					Number.BigInteger.SetValue(out result, ref lhs);
					return;
				}
				if (value == 0U)
				{
					Number.BigInteger.SetZero(out result);
					return;
				}
				int length = lhs._length;
				int i = 0;
				uint num = 0U;
				while (i < length)
				{
					ulong num2 = (ulong)(*(ref lhs._blocks.FixedElementField + (IntPtr)i * 4)) * (ulong)value + (ulong)num;
					*(ref result._blocks.FixedElementField + (IntPtr)i * 4) = (uint)num2;
					num = (uint)(num2 >> 32);
					i++;
				}
				if (num != 0U)
				{
					*(ref result._blocks.FixedElementField + (IntPtr)i * 4) = num;
					result._length = length + 1;
					return;
				}
				result._length = length;
			}

			public unsafe static void Multiply(ref Number.BigInteger lhs, ref Number.BigInteger rhs, out Number.BigInteger result)
			{
				if (lhs.IsZero() || rhs.IsOne())
				{
					Number.BigInteger.SetValue(out result, ref lhs);
					return;
				}
				if (rhs.IsZero())
				{
					Number.BigInteger.SetZero(out result);
					return;
				}
				ref Number.BigInteger ptr = ref lhs;
				int length = lhs._length;
				ref Number.BigInteger ptr2 = ref rhs;
				int length2 = rhs._length;
				if (length < length2)
				{
					ptr = ref rhs;
					length = rhs._length;
					ptr2 = ref lhs;
					length2 = lhs._length;
				}
				int num = length2 + length;
				result._length = num;
				BufferEx.ZeroMemory((byte*)result.GetBlocksPointer(), (uint)(num * 4));
				int i = 0;
				int num2 = 0;
				while (i < length2)
				{
					if (*(ref ptr2._blocks.FixedElementField + (IntPtr)i * 4) != 0U)
					{
						int num3 = 0;
						int num4 = num2;
						ulong num5 = 0UL;
						do
						{
							ulong num6 = (ulong)(*(ref result._blocks.FixedElementField + (IntPtr)num4 * 4)) + (ulong)(*(ref ptr2._blocks.FixedElementField + (IntPtr)i * 4)) * (ulong)(*(ref ptr._blocks.FixedElementField + (IntPtr)num3 * 4)) + num5;
							num5 = num6 >> 32;
							*(ref result._blocks.FixedElementField + (IntPtr)num4 * 4) = (uint)num6;
							num4++;
							num3++;
						}
						while (num3 < length);
						*(ref result._blocks.FixedElementField + (IntPtr)num4 * 4) = (uint)num5;
					}
					i++;
					num2++;
				}
				if (num > 0 && *(ref result._blocks.FixedElementField + (IntPtr)(num - 1) * 4) == 0U)
				{
					result._length--;
				}
			}

			public unsafe static void Pow2(uint exponent, out Number.BigInteger result)
			{
				uint num2;
				uint num = Number.BigInteger.DivRem32(exponent, out num2);
				result._length = (int)(num + 1U);
				if (num > 0U)
				{
					BufferEx.ZeroMemory((byte*)result.GetBlocksPointer(), num * 4U);
				}
				*(ref result._blocks.FixedElementField + (IntPtr)((ulong)num * 4UL)) = 1U << (int)num2;
			}

			public unsafe static void Pow10(uint exponent, out Number.BigInteger result)
			{
				Number.BigInteger bigInteger;
				Number.BigInteger.SetUInt32(out bigInteger, Number.BigInteger.s_Pow10UInt32Table[(int)(exponent & 7U)]);
				ref Number.BigInteger ptr = ref bigInteger;
				Number.BigInteger bigInteger2;
				Number.BigInteger.SetZero(out bigInteger2);
				ref Number.BigInteger ptr2 = ref bigInteger2;
				exponent >>= 3;
				uint num = 0U;
				while (exponent != 0U)
				{
					if ((exponent & 1U) != 0U)
					{
						fixed (uint* ptr3 = &Number.BigInteger.s_Pow10BigNumTable[Number.BigInteger.s_Pow10BigNumTableIndices[(int)num]])
						{
							ref Number.BigInteger rhs = ref *(Number.BigInteger*)ptr3;
							Number.BigInteger.Multiply(ref ptr, ref rhs, out ptr2);
						}
						ref Number.BigInteger ptr4 = ref ptr2;
						ptr2 = ref ptr;
						ptr = ref ptr4;
					}
					num += 1U;
					exponent >>= 1;
				}
				Number.BigInteger.SetValue(out result, ref ptr);
			}

			private unsafe static uint AddDivisor(ref Number.BigInteger lhs, int lhsStartIndex, ref Number.BigInteger rhs)
			{
				int length = rhs._length;
				ulong num = 0UL;
				for (int i = 0; i < length; i++)
				{
					ref uint ptr = ref lhs._blocks.FixedElementField + (IntPtr)(lhsStartIndex + i) * 4;
					ulong num2 = (ulong)ptr + num + (ulong)(*(ref rhs._blocks.FixedElementField + (IntPtr)i * 4));
					ptr = (uint)num2;
					num = num2 >> 32;
				}
				return (uint)num;
			}

			private static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo, uint divHi, uint divLo)
			{
				ulong num = (ulong)divHi * q;
				ulong num2 = (ulong)divLo * q;
				num += num2 >> 32;
				num2 &= (ulong)-1;
				return num >= valHi && (num > valHi || (num2 >= (ulong)valLo && num2 > (ulong)valLo));
			}

			private unsafe static uint SubtractDivisor(ref Number.BigInteger lhs, int lhsStartIndex, ref Number.BigInteger rhs, ulong q)
			{
				int length = rhs._length;
				ulong num = 0UL;
				for (int i = 0; i < length; i++)
				{
					num += (ulong)(*(ref rhs._blocks.FixedElementField + (IntPtr)i * 4)) * q;
					uint num2 = (uint)num;
					num >>= 32;
					ref uint ptr = ref lhs._blocks.FixedElementField + (IntPtr)(lhsStartIndex + i) * 4;
					if (ptr < num2)
					{
						num += 1UL;
					}
					ptr -= num2;
				}
				return (uint)num;
			}

			public unsafe void Add(uint value)
			{
				int length = this._length;
				if (length == 0)
				{
					Number.BigInteger.SetUInt32(out this, value);
					return;
				}
				this._blocks.FixedElementField = this._blocks.FixedElementField + value;
				if (this._blocks.FixedElementField >= value)
				{
					return;
				}
				for (int i = 1; i < length; i++)
				{
					*(ref this._blocks.FixedElementField + (IntPtr)i * 4) += 1U;
					if (*(ref this._blocks.FixedElementField + (IntPtr)i * 4) > 0U)
					{
						return;
					}
				}
				*(ref this._blocks.FixedElementField + (IntPtr)length * 4) = 1U;
				this._length = length + 1;
			}

			public unsafe uint GetBlock(uint index)
			{
				return *(ref this._blocks.FixedElementField + (IntPtr)((ulong)index * 4UL));
			}

			public int GetLength()
			{
				return this._length;
			}

			public bool IsOne()
			{
				return this._length == 1 && this._blocks.FixedElementField == 1U;
			}

			public bool IsZero()
			{
				return this._length == 0;
			}

			public void Multiply(uint value)
			{
				Number.BigInteger.Multiply(ref this, value, out this);
			}

			public void Multiply(ref Number.BigInteger value)
			{
				Number.BigInteger bigInteger;
				Number.BigInteger.SetValue(out bigInteger, ref this);
				Number.BigInteger.Multiply(ref bigInteger, ref value, out this);
			}

			public unsafe void Multiply10()
			{
				if (this.IsZero())
				{
					return;
				}
				int i = 0;
				int length = this._length;
				ulong num = 0UL;
				while (i < length)
				{
					ulong num2 = (ulong)(*(ref this._blocks.FixedElementField + (IntPtr)i * 4));
					ulong num3 = (num2 << 3) + (num2 << 1) + num;
					num = num3 >> 32;
					*(ref this._blocks.FixedElementField + (IntPtr)i * 4) = (uint)num3;
					i++;
				}
				if (num != 0UL)
				{
					*(ref this._blocks.FixedElementField + (IntPtr)i * 4) = (uint)num;
					this._length++;
				}
			}

			public void MultiplyPow10(uint exponent)
			{
				if (this.IsZero())
				{
					return;
				}
				Number.BigInteger bigInteger;
				Number.BigInteger.Pow10(exponent, out bigInteger);
				if (bigInteger._length == 1)
				{
					this.Multiply(bigInteger._blocks.FixedElementField);
					return;
				}
				this.Multiply(ref bigInteger);
			}

			public static void SetUInt32(out Number.BigInteger result, uint value)
			{
				if (value == 0U)
				{
					Number.BigInteger.SetZero(out result);
					return;
				}
				result._blocks.FixedElementField = value;
				result._length = 1;
			}

			public unsafe static void SetUInt64(out Number.BigInteger result, ulong value)
			{
				if (value <= (ulong)-1)
				{
					Number.BigInteger.SetUInt32(out result, (uint)value);
					return;
				}
				result._blocks.FixedElementField = (uint)value;
				*(ref result._blocks.FixedElementField + 4) = (uint)(value >> 32);
				result._length = 2;
			}

			public unsafe static void SetValue(out Number.BigInteger result, ref Number.BigInteger value)
			{
				int length = value._length;
				result._length = length;
				BufferEx.Memcpy((byte*)result.GetBlocksPointer(), (byte*)value.GetBlocksPointer(), length * 4);
			}

			public static void SetZero(out Number.BigInteger result)
			{
				result._length = 0;
			}

			public unsafe void ShiftLeft(uint shift)
			{
				int length = this._length;
				if (length == 0 || shift == 0U)
				{
					return;
				}
				uint num2;
				uint num = Number.BigInteger.DivRem32(shift, out num2);
				int i = length - 1;
				int num3 = i + (int)num;
				if (num2 == 0U)
				{
					while (i >= 0)
					{
						*(ref this._blocks.FixedElementField + (IntPtr)num3 * 4) = *(ref this._blocks.FixedElementField + (IntPtr)i * 4);
						i--;
						num3--;
					}
					this._length += (int)num;
					BufferEx.ZeroMemory((byte*)this.GetBlocksPointer(), num * 4U);
					return;
				}
				num3++;
				this._length = num3 + 1;
				uint num4 = 32U - num2;
				uint num5 = 0U;
				uint num6 = *(ref this._blocks.FixedElementField + (IntPtr)i * 4);
				uint num7 = num6 >> (int)num4;
				while (i > 0)
				{
					*(ref this._blocks.FixedElementField + (IntPtr)num3 * 4) = (num5 | num7);
					num5 = num6 << (int)num2;
					i--;
					num3--;
					num6 = *(ref this._blocks.FixedElementField + (IntPtr)i * 4);
					num7 = num6 >> (int)num4;
				}
				*(ref this._blocks.FixedElementField + (IntPtr)num3 * 4) = (num5 | num7);
				*(ref this._blocks.FixedElementField + (IntPtr)(num3 - 1) * 4) = num6 << (int)num2;
				BufferEx.ZeroMemory((byte*)this.GetBlocksPointer(), num * 4U);
				if (*(ref this._blocks.FixedElementField + (IntPtr)(this._length - 1) * 4) == 0U)
				{
					this._length--;
				}
			}

			public unsafe ulong ToUInt64()
			{
				if (this._length > 1)
				{
					return ((ulong)(*(ref this._blocks.FixedElementField + 4)) << 32) + (ulong)this._blocks.FixedElementField;
				}
				if (this._length > 0)
				{
					return (ulong)this._blocks.FixedElementField;
				}
				return 0UL;
			}

			private unsafe uint* GetBlocksPointer()
			{
				return (uint*)Unsafe.AsPointer<uint>(ref this._blocks.FixedElementField);
			}

			private static uint DivRem32(uint value, out uint remainder)
			{
				remainder = (value & 31U);
				return value >> 5;
			}

			private const int BitsForLongestBinaryMantissa = 1074;

			private const int BitsForLongestDigitSequence = 2552;

			private const int MaxBits = 3658;

			private const int BitsPerBlock = 32;

			private const int MaxBlockCount = 115;

			[Nullable(1)]
			private static readonly uint[] s_Pow10UInt32Table = new uint[]
			{
				1U,
				10U,
				100U,
				1000U,
				10000U,
				100000U,
				1000000U,
				10000000U
			};

			[Nullable(1)]
			private static readonly int[] s_Pow10BigNumTableIndices = new int[]
			{
				0,
				2,
				5,
				10,
				18,
				33,
				61,
				116
			};

			[Nullable(1)]
			private static readonly uint[] s_Pow10BigNumTable = new uint[]
			{
				1U,
				100000000U,
				2U,
				1874919424U,
				2328306U,
				4U,
				0U,
				2242703233U,
				762134875U,
				1262U,
				7U,
				0U,
				0U,
				3211403009U,
				1849224548U,
				3668416493U,
				3913284084U,
				1593091U,
				14U,
				0U,
				0U,
				0U,
				0U,
				781532673U,
				64985353U,
				253049085U,
				594863151U,
				3553621484U,
				3288652808U,
				3167596762U,
				2788392729U,
				3911132675U,
				590U,
				27U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				2553183233U,
				3201533787U,
				3638140786U,
				303378311U,
				1809731782U,
				3477761648U,
				3583367183U,
				649228654U,
				2915460784U,
				487929380U,
				1011012442U,
				1677677582U,
				3428152256U,
				1710878487U,
				1438394610U,
				2161952759U,
				4100910556U,
				1608314830U,
				349175U,
				54U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				4234999809U,
				2012377703U,
				2408924892U,
				1570150255U,
				3090844311U,
				3273530073U,
				1187251475U,
				2498123591U,
				3364452033U,
				1148564857U,
				687371067U,
				2854068671U,
				1883165473U,
				505794538U,
				2988060450U,
				3159489326U,
				2531348317U,
				3215191468U,
				849106862U,
				3892080979U,
				3288073877U,
				2242451748U,
				4183778142U,
				2995818208U,
				2477501924U,
				325481258U,
				2487842652U,
				1774082830U,
				1933815724U,
				2962865281U,
				1168579910U,
				2724829000U,
				2360374019U,
				2315984659U,
				2360052375U,
				3251779801U,
				1664357844U,
				28U,
				107U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				689565697U,
				4116392818U,
				1853628763U,
				516071302U,
				2568769159U,
				365238920U,
				336250165U,
				1283268122U,
				3425490969U,
				248595470U,
				2305176814U,
				2111925499U,
				507770399U,
				2681111421U,
				589114268U,
				591287751U,
				1708941527U,
				4098957707U,
				475844916U,
				3378731398U,
				2452339615U,
				2817037361U,
				2678008327U,
				1656645978U,
				2383430340U,
				73103988U,
				448667107U,
				2329420453U,
				3124020241U,
				3625235717U,
				3208634035U,
				2412059158U,
				2981664444U,
				4117622508U,
				838560765U,
				3069470027U,
				270153238U,
				1802868219U,
				3692709886U,
				2161737865U,
				2159912357U,
				2585798786U,
				837488486U,
				4237238160U,
				2540319504U,
				3798629246U,
				3748148874U,
				1021550776U,
				2386715342U,
				1973637538U,
				1823520457U,
				1146713475U,
				833971519U,
				3277251466U,
				905620390U,
				26278816U,
				2680483154U,
				2294040859U,
				373297482U,
				5996609U,
				4109575006U,
				512575049U,
				917036550U,
				1942311753U,
				2816916778U,
				3248920332U,
				1192784020U,
				3537586671U,
				2456567643U,
				2925660628U,
				759380297U,
				888447942U,
				3559939476U,
				3654687237U,
				805U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U,
				0U
			};

			private int _length;

			[FixedBuffer(typeof(uint), 115)]
			private Number.BigInteger.<_blocks>e__FixedBuffer _blocks;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 460)]
			public struct <_blocks>e__FixedBuffer
			{
				public uint FixedElementField;
			}
		}

		internal readonly ref struct DiyFp
		{
			public static Number.DiyFp CreateAndGetBoundaries(double value, out Number.DiyFp mMinus, out Number.DiyFp mPlus)
			{
				Number.DiyFp result = new Number.DiyFp(value);
				result.GetBoundaries(52, out mMinus, out mPlus);
				return result;
			}

			public static Number.DiyFp CreateAndGetBoundaries(float value, out Number.DiyFp mMinus, out Number.DiyFp mPlus)
			{
				Number.DiyFp result = new Number.DiyFp(value);
				result.GetBoundaries(23, out mMinus, out mPlus);
				return result;
			}

			public DiyFp(double value)
			{
				this.f = Number.ExtractFractionAndBiasedExponent(value, out this.e);
			}

			public DiyFp(float value)
			{
				this.f = (ulong)Number.ExtractFractionAndBiasedExponent(value, out this.e);
			}

			public DiyFp(ulong f, int e)
			{
				this.f = f;
				this.e = e;
			}

			public Number.DiyFp Multiply(in Number.DiyFp other)
			{
				uint num = (uint)(this.f >> 32);
				uint num2 = (uint)this.f;
				uint num3 = (uint)(other.f >> 32);
				uint num4 = (uint)other.f;
				ulong num5 = (ulong)num * (ulong)num3;
				ulong num6 = (ulong)num2 * (ulong)num3;
				ulong num7 = (ulong)num * (ulong)num4;
				ulong num8 = ((ulong)num2 * (ulong)num4 >> 32) + (ulong)((uint)num7) + (ulong)((uint)num6);
				num8 += (ulong)int.MinValue;
				return new Number.DiyFp(num5 + (num7 >> 32) + (num6 >> 32) + (num8 >> 32), this.e + other.e + 64);
			}

			public Number.DiyFp Normalize()
			{
				int num = BitOperations.LeadingZeroCount(this.f);
				return new Number.DiyFp(this.f << num, this.e - num);
			}

			public Number.DiyFp Subtract(in Number.DiyFp other)
			{
				return new Number.DiyFp(this.f - other.f, this.e);
			}

			private void GetBoundaries(int implicitBitIndex, out Number.DiyFp mMinus, out Number.DiyFp mPlus)
			{
				mPlus = new Number.DiyFp((this.f << 1) + 1UL, this.e - 1).Normalize();
				if (this.f == 1UL << implicitBitIndex)
				{
					mMinus = new Number.DiyFp((this.f << 2) - 1UL, this.e - 2);
				}
				else
				{
					mMinus = new Number.DiyFp((this.f << 1) - 1UL, this.e - 1);
				}
				mMinus = new Number.DiyFp(mMinus.f << mMinus.e - mPlus.e, mPlus.e);
			}

			public const int DoubleImplicitBitIndex = 52;

			public const int SingleImplicitBitIndex = 23;

			public const int SignificandSize = 64;

			public readonly ulong f;

			public readonly int e;
		}

		internal static class Grisu3
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool IsNegative(double d)
			{
				return BitConverter.DoubleToInt64Bits(d) < 0L;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool IsNegativeInfinity(float f)
			{
				return f == float.NegativeInfinity;
			}

			public unsafe static bool TryRunDouble(double value, int requestedDigits, ref Number.NumberBuffer number)
			{
				double value2 = Number.Grisu3.IsNegative(value) ? (-value) : value;
				int num;
				int num2;
				bool flag;
				if (requestedDigits == -1)
				{
					Number.DiyFp diyFp2;
					Number.DiyFp diyFp3;
					Number.DiyFp diyFp = Number.DiyFp.CreateAndGetBoundaries(value2, out diyFp2, out diyFp3).Normalize();
					flag = Number.Grisu3.TryRunShortest(diyFp2, diyFp, diyFp3, number.Digits, out num, out num2);
				}
				else
				{
					Number.DiyFp diyFp4 = new Number.DiyFp(value2).Normalize();
					flag = Number.Grisu3.TryRunCounted(diyFp4, requestedDigits, number.Digits, out num, out num2);
				}
				if (flag)
				{
					number.Scale = num + num2;
					*number.Digits[num] = 0;
					number.DigitsCount = num;
				}
				return flag;
			}

			public unsafe static bool TryRunSingle(float value, int requestedDigits, ref Number.NumberBuffer number)
			{
				float value2 = Number.Grisu3.IsNegative((double)value) ? (-value) : value;
				int num;
				int num2;
				bool flag;
				if (requestedDigits == -1)
				{
					Number.DiyFp diyFp2;
					Number.DiyFp diyFp3;
					Number.DiyFp diyFp = Number.DiyFp.CreateAndGetBoundaries(value2, out diyFp2, out diyFp3).Normalize();
					flag = Number.Grisu3.TryRunShortest(diyFp2, diyFp, diyFp3, number.Digits, out num, out num2);
				}
				else
				{
					Number.DiyFp diyFp4 = new Number.DiyFp(value2).Normalize();
					flag = Number.Grisu3.TryRunCounted(diyFp4, requestedDigits, number.Digits, out num, out num2);
				}
				if (flag)
				{
					number.Scale = num + num2;
					*number.Digits[num] = 0;
					number.DigitsCount = num;
				}
				return flag;
			}

			private static bool TryRunCounted(in Number.DiyFp w, int requestedDigits, Span<byte> buffer, out int length, out int decimalExponent)
			{
				int minExponent = -60 - (w.e + 64);
				int maxExponent = -32 - (w.e + 64);
				int num;
				Number.DiyFp cachedPowerForBinaryExponentRange = Number.Grisu3.GetCachedPowerForBinaryExponentRange(minExponent, maxExponent, out num);
				Number.DiyFp diyFp = w.Multiply(cachedPowerForBinaryExponentRange);
				int num2;
				bool result = Number.Grisu3.TryDigitGenCounted(diyFp, requestedDigits, buffer, out length, out num2);
				decimalExponent = -num + num2;
				return result;
			}

			private static bool TryRunShortest(in Number.DiyFp boundaryMinus, in Number.DiyFp w, in Number.DiyFp boundaryPlus, Span<byte> buffer, out int length, out int decimalExponent)
			{
				int minExponent = -60 - (w.e + 64);
				int maxExponent = -32 - (w.e + 64);
				int num;
				Number.DiyFp cachedPowerForBinaryExponentRange = Number.Grisu3.GetCachedPowerForBinaryExponentRange(minExponent, maxExponent, out num);
				Number.DiyFp diyFp = w.Multiply(cachedPowerForBinaryExponentRange);
				Number.DiyFp diyFp2 = boundaryMinus.Multiply(cachedPowerForBinaryExponentRange);
				Number.DiyFp diyFp3 = boundaryPlus.Multiply(cachedPowerForBinaryExponentRange);
				int num2;
				bool result = Number.Grisu3.TryDigitGenShortest(diyFp2, diyFp, diyFp3, buffer, out length, out num2);
				decimalExponent = -num + num2;
				return result;
			}

			private static uint BiggestPowerTen(uint number, int numberBits, out int exponentPlusOne)
			{
				int num = (numberBits + 1) * 1233 >> 12;
				uint num2 = Number.Grisu3.s_SmallPowersOfTen[num];
				if (number < num2)
				{
					num--;
					num2 = Number.Grisu3.s_SmallPowersOfTen[num];
				}
				exponentPlusOne = num + 1;
				return num2;
			}

			private unsafe static bool TryDigitGenCounted(in Number.DiyFp w, int requestedDigits, Span<byte> buffer, out int length, out int kappa)
			{
				ulong num = 1UL;
				Number.DiyFp diyFp = new Number.DiyFp(1UL << -w.e, w.e);
				uint num2 = (uint)(w.f >> -diyFp.e);
				ulong num3 = w.f & diyFp.f - 1UL;
				if (num3 == 0UL && (requestedDigits >= 11 || num2 < Number.Grisu3.s_SmallPowersOfTen[requestedDigits - 1]))
				{
					length = 0;
					kappa = 0;
					return false;
				}
				uint num4 = Number.Grisu3.BiggestPowerTen(num2, 64 - -diyFp.e, out kappa);
				length = 0;
				while (kappa > 0)
				{
					uint num5 = MathEx.DivRem(num2, num4, out num2);
					*buffer[length] = (byte)(48U + num5);
					length++;
					requestedDigits--;
					kappa--;
					if (requestedDigits == 0)
					{
						break;
					}
					num4 /= 10U;
				}
				if (requestedDigits == 0)
				{
					ulong rest = ((ulong)num2 << -diyFp.e) + num3;
					return Number.Grisu3.TryRoundWeedCounted(buffer, length, rest, (ulong)num4 << -diyFp.e, num, ref kappa);
				}
				while (requestedDigits > 0 && num3 > num)
				{
					num3 *= 10UL;
					num *= 10UL;
					uint num6 = (uint)(num3 >> -diyFp.e);
					*buffer[length] = (byte)(48U + num6);
					length++;
					requestedDigits--;
					kappa--;
					num3 &= diyFp.f - 1UL;
				}
				if (requestedDigits != 0)
				{
					*buffer[0] = 0;
					length = 0;
					kappa = 0;
					return false;
				}
				return Number.Grisu3.TryRoundWeedCounted(buffer, length, num3, diyFp.f, num, ref kappa);
			}

			private unsafe static bool TryDigitGenShortest(in Number.DiyFp low, in Number.DiyFp w, in Number.DiyFp high, Span<byte> buffer, out int length, out int kappa)
			{
				ulong num = 1UL;
				Number.DiyFp diyFp = new Number.DiyFp(low.f - num, low.e);
				Number.DiyFp diyFp2 = new Number.DiyFp(high.f + num, high.e);
				Number.DiyFp diyFp3 = diyFp2.Subtract(diyFp);
				Number.DiyFp diyFp4 = new Number.DiyFp(1UL << -w.e, w.e);
				uint num2 = (uint)(diyFp2.f >> -diyFp4.e);
				ulong num3 = diyFp2.f & diyFp4.f - 1UL;
				uint num4 = Number.Grisu3.BiggestPowerTen(num2, 64 - -diyFp4.e, out kappa);
				length = 0;
				while (kappa > 0)
				{
					uint num5 = MathEx.DivRem(num2, num4, out num2);
					*buffer[length] = (byte)(48U + num5);
					length++;
					kappa--;
					ulong num6 = ((ulong)num2 << -diyFp4.e) + num3;
					if (num6 < diyFp3.f)
					{
						return Number.Grisu3.TryRoundWeedShortest(buffer, length, diyFp2.Subtract(w).f, diyFp3.f, num6, (ulong)num4 << -diyFp4.e, num);
					}
					num4 /= 10U;
				}
				do
				{
					num3 *= 10UL;
					num *= 10UL;
					diyFp3 = new Number.DiyFp(diyFp3.f * 10UL, diyFp3.e);
					uint num7 = (uint)(num3 >> -diyFp4.e);
					*buffer[length] = (byte)(48U + num7);
					length++;
					kappa--;
					num3 &= diyFp4.f - 1UL;
				}
				while (num3 >= diyFp3.f);
				return Number.Grisu3.TryRoundWeedShortest(buffer, length, diyFp2.Subtract(w).f * num, diyFp3.f, num3, diyFp4.f, num);
			}

			private static Number.DiyFp GetCachedPowerForBinaryExponentRange(int minExponent, int maxExponent, out int decimalExponent)
			{
				double num = Math.Ceiling((double)(minExponent + 64 - 1) * 0.3010299956639812);
				int num2 = (348 + (int)num - 1) / 8 + 1;
				decimalExponent = (int)Number.Grisu3.s_CachedPowersDecimalExponent[num2];
				return new Number.DiyFp(Number.Grisu3.s_CachedPowersSignificand[num2], (int)Number.Grisu3.s_CachedPowersBinaryExponent[num2]);
			}

			private unsafe static bool TryRoundWeedCounted(Span<byte> buffer, int length, ulong rest, ulong tenKappa, ulong unit, ref int kappa)
			{
				if (unit >= tenKappa || tenKappa - unit <= unit)
				{
					return false;
				}
				if (tenKappa - rest > rest && tenKappa - 2UL * rest >= 2UL * unit)
				{
					return true;
				}
				if (rest > unit && (tenKappa <= rest - unit || tenKappa - (rest - unit) <= rest - unit))
				{
					ref byte ptr = ref buffer[length - 1];
					ptr += 1;
					int num = length - 1;
					while (num > 0 && *buffer[num] == 58)
					{
						*buffer[num] = 48;
						ref byte ptr2 = ref buffer[num - 1];
						ptr2 += 1;
						num--;
					}
					if (*buffer[0] == 58)
					{
						*buffer[0] = 49;
						kappa++;
					}
					return true;
				}
				return false;
			}

			private static bool TryRoundWeedShortest(Span<byte> buffer, int length, ulong distanceTooHighW, ulong unsafeInterval, ulong rest, ulong tenKappa, ulong unit)
			{
				ulong num = distanceTooHighW - unit;
				ulong num2 = distanceTooHighW + unit;
				while (rest < num && unsafeInterval - rest >= tenKappa && (rest + tenKappa < num || num - rest >= rest + tenKappa - num))
				{
					ref byte ptr = ref buffer[length - 1];
					ptr -= 1;
					rest += tenKappa;
				}
				return (rest >= num2 || unsafeInterval - rest < tenKappa || (rest + tenKappa >= num2 && num2 - rest <= rest + tenKappa - num2)) && 2UL * unit <= rest && rest <= unsafeInterval - 4UL * unit;
			}

			private const int CachedPowersDecimalExponentDistance = 8;

			private const int CachedPowersMinDecimalExponent = -348;

			private const int CachedPowersPowerMaxDecimalExponent = 340;

			private const int CachedPowersOffset = 348;

			private const double D1Log210 = 0.3010299956639812;

			private const int MaximalTargetExponent = -32;

			private const int MinimalTargetExponent = -60;

			[Nullable(1)]
			private static readonly short[] s_CachedPowersBinaryExponent = new short[]
			{
				-1220,
				-1193,
				-1166,
				-1140,
				-1113,
				-1087,
				-1060,
				-1034,
				-1007,
				-980,
				-954,
				-927,
				-901,
				-874,
				-847,
				-821,
				-794,
				-768,
				-741,
				-715,
				-688,
				-661,
				-635,
				-608,
				-582,
				-555,
				-529,
				-502,
				-475,
				-449,
				-422,
				-396,
				-369,
				-343,
				-316,
				-289,
				-263,
				-236,
				-210,
				-183,
				-157,
				-130,
				-103,
				-77,
				-50,
				-24,
				3,
				30,
				56,
				83,
				109,
				136,
				162,
				189,
				216,
				242,
				269,
				295,
				322,
				348,
				375,
				402,
				428,
				455,
				481,
				508,
				534,
				561,
				588,
				614,
				641,
				667,
				694,
				720,
				747,
				774,
				800,
				827,
				853,
				880,
				907,
				933,
				960,
				986,
				1013,
				1039,
				1066
			};

			[Nullable(1)]
			private static readonly short[] s_CachedPowersDecimalExponent = new short[]
			{
				-348,
				-340,
				-332,
				-324,
				-316,
				-308,
				-300,
				-292,
				-284,
				-276,
				-268,
				-260,
				-252,
				-244,
				-236,
				-228,
				-220,
				-212,
				-204,
				-196,
				-188,
				-180,
				-172,
				-164,
				-156,
				-148,
				-140,
				-132,
				-124,
				-116,
				-108,
				-100,
				-92,
				-84,
				-76,
				-68,
				-60,
				-52,
				-44,
				-36,
				-28,
				-20,
				-12,
				-4,
				4,
				12,
				20,
				28,
				36,
				44,
				52,
				60,
				68,
				76,
				84,
				92,
				100,
				108,
				116,
				124,
				132,
				140,
				148,
				156,
				164,
				172,
				180,
				188,
				196,
				204,
				212,
				220,
				228,
				236,
				244,
				252,
				260,
				268,
				276,
				284,
				292,
				300,
				308,
				316,
				324,
				332,
				340
			};

			[Nullable(1)]
			private static readonly ulong[] s_CachedPowersSignificand = new ulong[]
			{
				18054884314459144840UL,
				13451937075301367670UL,
				10022474136428063862UL,
				14934650266808366570UL,
				11127181549972568877UL,
				16580792590934885855UL,
				12353653155963782858UL,
				18408377700990114895UL,
				13715310171984221708UL,
				10218702384817765436UL,
				15227053142812498563UL,
				11345038669416679861UL,
				16905424996341287883UL,
				12595523146049147757UL,
				9384396036005875287UL,
				13983839803942852151UL,
				10418772551374772303UL,
				15525180923007089351UL,
				11567161174868858868UL,
				17236413322193710309UL,
				12842128665889583758UL,
				9568131466127621947UL,
				14257626930069360058UL,
				10622759856335341974UL,
				15829145694278690180UL,
				11793632577567316726UL,
				17573882009934360870UL,
				13093562431584567480UL,
				9755464219737475723UL,
				14536774485912137811UL,
				10830740992659433045UL,
				16139061738043178685UL,
				12024538023802026127UL,
				17917957937422433684UL,
				13349918974505688015UL,
				9946464728195732843UL,
				14821387422376473014UL,
				11042794154864902060UL,
				16455045573212060422UL,
				12259964326927110867UL,
				18268770466636286478UL,
				13611294676837538539UL,
				10141204801825835212UL,
				15111572745182864684UL,
				11258999068426240000UL,
				16777216000000000000UL,
				12500000000000000000UL,
				9313225746154785156UL,
				13877787807814456755UL,
				10339757656912845936UL,
				15407439555097886824UL,
				11479437019748901445UL,
				17105694144590052135UL,
				12744735289059618216UL,
				9495567745759798747UL,
				14149498560666738074UL,
				10542197943230523224UL,
				15709099088952724970UL,
				11704190886730495818UL,
				17440603504673385349UL,
				12994262207056124023UL,
				9681479787123295682UL,
				14426529090290212157UL,
				10748601772107342003UL,
				16016664761464807395UL,
				11933345169920330789UL,
				17782069995880619868UL,
				13248674568444952270UL,
				9871031767461413346UL,
				14708983551653345445UL,
				10959046745042015199UL,
				16330252207878254650UL,
				12166986024289022870UL,
				18130221999122236476UL,
				13508068024458167312UL,
				10064294952495520794UL,
				14996968138956309548UL,
				11173611982879273257UL,
				16649979327439178909UL,
				12405201291620119593UL,
				9242595204427927429UL,
				13772540099066387757UL,
				10261342003245940623UL,
				15290591125556738113UL,
				11392378155556871081UL,
				16975966327722178521UL,
				12648080533535911531UL
			};

			[Nullable(1)]
			private static readonly uint[] s_SmallPowersOfTen = new uint[]
			{
				1U,
				10U,
				100U,
				1000U,
				10000U,
				100000U,
				1000000U,
				10000000U,
				100000000U,
				1000000000U
			};
		}

		internal ref struct NumberBuffer
		{
			public unsafe NumberBuffer(Number.NumberBufferKind kind, byte* digits, int digitsLength)
			{
				this.DigitsCount = 0;
				this.Scale = 0;
				this.IsNegative = false;
				this.HasNonZeroTail = false;
				this.Kind = kind;
				this.Digits = new Span<byte>((void*)digits, digitsLength);
				*this.Digits[0] = 0;
			}

			[Conditional("DEBUG")]
			public void CheckConsistency()
			{
			}

			public unsafe byte* GetDigitsPointer()
			{
				return (byte*)Unsafe.AsPointer<byte>(this.Digits[0]);
			}

			[NullableContext(1)]
			public unsafe override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append('[');
				stringBuilder.Append('"');
				for (int i = 0; i < this.Digits.Length; i++)
				{
					byte b = *this.Digits[i];
					if (b == 0)
					{
						break;
					}
					stringBuilder.Append((char)b);
				}
				stringBuilder.Append('"');
				stringBuilder.Append(", Length = ").Append(this.DigitsCount);
				stringBuilder.Append(", Scale = ").Append(this.Scale);
				stringBuilder.Append(", IsNegative = ").Append(this.IsNegative);
				stringBuilder.Append(", HasNonZeroTail = ").Append(this.HasNonZeroTail);
				stringBuilder.Append(", Kind = ").Append(this.Kind);
				stringBuilder.Append(']');
				return stringBuilder.ToString();
			}

			public int DigitsCount;

			public int Scale;

			public bool IsNegative;

			public bool HasNonZeroTail;

			public Number.NumberBufferKind Kind;

			public Span<byte> Digits;
		}

		internal enum NumberBufferKind : byte
		{
			Unknown,
			Integer,
			Decimal,
			FloatingPoint
		}

		public readonly struct FloatingPointInfo
		{
			public ulong ZeroBits { get; }

			public ulong InfinityBits { get; }

			public ulong NormalMantissaMask { get; }

			public ulong DenormalMantissaMask { get; }

			public int MinBinaryExponent { get; }

			public int MaxBinaryExponent { get; }

			public int ExponentBias { get; }

			public int OverflowDecimalExponent { get; }

			public ushort NormalMantissaBits { get; }

			public ushort DenormalMantissaBits { get; }

			public ushort ExponentBits { get; }

			public FloatingPointInfo(ushort denormalMantissaBits, ushort exponentBits, int maxBinaryExponent, int exponentBias, ulong infinityBits)
			{
				this.ExponentBits = exponentBits;
				this.DenormalMantissaBits = denormalMantissaBits;
				this.NormalMantissaBits = denormalMantissaBits + 1;
				this.OverflowDecimalExponent = (maxBinaryExponent + (int)(2 * this.NormalMantissaBits)) / 3;
				this.ExponentBias = exponentBias;
				this.MaxBinaryExponent = maxBinaryExponent;
				this.MinBinaryExponent = 1 - maxBinaryExponent;
				this.DenormalMantissaMask = (1L << (int)denormalMantissaBits) - 1L;
				this.NormalMantissaMask = (1L << (int)this.NormalMantissaBits) - 1L;
				this.InfinityBits = infinityBits;
				this.ZeroBits = 0L;
			}

			public static readonly Number.FloatingPointInfo Double = new Number.FloatingPointInfo(52, 11, 1023, 1023, 9218868437227405312UL);

			public static readonly Number.FloatingPointInfo Single = new Number.FloatingPointInfo(23, 8, 127, 127, 2139095040UL);
		}

		internal enum ParsingStatus
		{
			OK,
			Failed,
			Overflow
		}
	}
}
