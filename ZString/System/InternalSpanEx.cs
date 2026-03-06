using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	internal static class InternalSpanEx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool EqualsOrdinalIgnoreCase(this ReadOnlySpan<char> span, ReadOnlySpan<char> value)
		{
			return span.Length == value.Length && (value.Length == 0 || InternalSpanEx.EqualsOrdinalIgnoreCase(MemoryMarshal.GetReference<char>(span), MemoryMarshal.GetReference<char>(value), span.Length));
		}

		private unsafe static bool EqualsOrdinalIgnoreCase(ref char charA, ref char charB, int length)
		{
			IntPtr intPtr = IntPtr.Zero;
			if (IntPtr.Size == 8)
			{
				while (length >= 4)
				{
					ulong num = Unsafe.ReadUnaligned<ulong>(Unsafe.As<char, byte>(Unsafe.AddByteOffset<char>(ref charA, intPtr)));
					ulong num2 = Unsafe.ReadUnaligned<ulong>(Unsafe.As<char, byte>(Unsafe.AddByteOffset<char>(ref charB, intPtr)));
					ulong num3 = num | num2;
					if (!InternalSpanEx.AllCharsInUInt32AreAscii((uint)num3 | (uint)(num3 >> 32)))
					{
						goto IL_104;
					}
					if (!InternalSpanEx.UInt64OrdinalIgnoreCaseAscii(num, num2))
					{
						return false;
					}
					intPtr += 8;
					length -= 4;
				}
			}
			while (length >= 2)
			{
				uint num4 = Unsafe.ReadUnaligned<uint>(Unsafe.As<char, byte>(Unsafe.AddByteOffset<char>(ref charA, intPtr)));
				uint num5 = Unsafe.ReadUnaligned<uint>(Unsafe.As<char, byte>(Unsafe.AddByteOffset<char>(ref charB, intPtr)));
				if (!InternalSpanEx.AllCharsInUInt32AreAscii(num4 | num5))
				{
					goto IL_104;
				}
				if (!InternalSpanEx.UInt32OrdinalIgnoreCaseAscii(num4, num5))
				{
					return false;
				}
				intPtr += 4;
				length -= 2;
			}
			if (length == 0)
			{
				return true;
			}
			uint num6 = (uint)(*Unsafe.AddByteOffset<char>(ref charA, intPtr));
			uint num7 = (uint)(*Unsafe.AddByteOffset<char>(ref charB, intPtr));
			if ((num6 | num7) <= 127U)
			{
				if (num6 == num7)
				{
					return true;
				}
				num6 |= 32U;
				return num6 - 97U <= 25U && num6 == (num7 | 32U);
			}
			IL_104:
			return InternalSpanEx.EqualsOrdinalIgnoreCaseNonAscii(Unsafe.AddByteOffset<char>(ref charA, intPtr), Unsafe.AddByteOffset<char>(ref charB, intPtr), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool AllCharsInUInt32AreAscii(uint value)
		{
			return (value & 4286644096U) == 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool AllCharsInUInt64AreAscii(ulong value)
		{
			return (value & 18410996206198128512UL) == 0UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool UInt32OrdinalIgnoreCaseAscii(uint valueA, uint valueB)
		{
			uint num = valueA ^ valueB;
			uint num2 = valueA + 16777472U - 4259905U;
			uint num3 = (valueA | 2097184U) + 8388736U - 8061051U;
			return (((num2 | num3) >> 2 | 4292870111U) & num) == 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool UInt64OrdinalIgnoreCaseAscii(ulong valueA, ulong valueB)
		{
			ulong num = valueA + 36029346783166592UL - 18296152663326785UL;
			ulong num2 = (valueA | 9007336695791648UL) + 72058693566333184UL - 34621950424449147UL;
			ulong num3 = (36029346783166592UL & num & num2) >> 2;
			return (valueA | num3) == (valueB | num3);
		}

		private unsafe static bool EqualsOrdinalIgnoreCaseNonAscii(ref char charA, ref char charB, int length)
		{
			IntPtr intPtr = IntPtr.Zero;
			while (length != 0)
			{
				uint num = (uint)(*Unsafe.AddByteOffset<char>(ref charA, intPtr));
				uint num2 = (uint)(*Unsafe.AddByteOffset<char>(ref charB, intPtr));
				if (num != num2 && ((num | 32U) != (num2 | 32U) || (num | 32U) - 97U > 25U))
				{
					return false;
				}
				intPtr += 2;
				length--;
			}
			return true;
		}
	}
}
