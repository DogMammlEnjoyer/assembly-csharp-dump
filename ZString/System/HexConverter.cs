using System;
using System.Runtime.CompilerServices;

namespace System
{
	internal static class HexConverter
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void ToBytesBuffer(byte value, Span<byte> buffer, int startingIndex = 0, HexConverter.Casing casing = HexConverter.Casing.Upper)
		{
			uint num = (uint)(((int)(value & 240) << 4) + (int)(value & 15) - 35209);
			uint num2 = (uint)(((-num & 28784U) >> 4) + num + (HexConverter.Casing)47545U | casing);
			*buffer[startingIndex + 1] = (byte)num2;
			*buffer[startingIndex] = (byte)(num2 >> 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void ToCharsBuffer(byte value, Span<char> buffer, int startingIndex = 0, HexConverter.Casing casing = HexConverter.Casing.Upper)
		{
			uint num = (uint)(((int)(value & 240) << 4) + (int)(value & 15) - 35209);
			uint num2 = (uint)(((-num & 28784U) >> 4) + num + (HexConverter.Casing)47545U | casing);
			*buffer[startingIndex + 1] = (char)(num2 & 255U);
			*buffer[startingIndex] = (char)(num2 >> 8);
		}

		[return: Nullable(1)]
		public unsafe static string ToString(ReadOnlySpan<byte> bytes, HexConverter.Casing casing = HexConverter.Casing.Upper)
		{
			Span<char> buffer = default(Span<char>);
			if (bytes.Length > 16)
			{
				buffer = new char[bytes.Length * 2].AsSpan<char>();
			}
			else
			{
				int i = bytes.Length * 2;
				buffer = new Span<char>(stackalloc byte[checked(unchecked((UIntPtr)i) * 2)], i);
			}
			int num = 0;
			ReadOnlySpan<byte> readOnlySpan = bytes;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				HexConverter.ToCharsBuffer(*readOnlySpan[i], buffer, num, casing);
				num += 2;
			}
			return buffer.ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToCharUpper(int value)
		{
			value &= 15;
			value += 48;
			if (value > 57)
			{
				value += 7;
			}
			return (char)value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char ToCharLower(int value)
		{
			value &= 15;
			value += 48;
			if (value > 57)
			{
				value += 39;
			}
			return (char)value;
		}

		public enum Casing : uint
		{
			Upper,
			Lower = 8224U
		}
	}
}
