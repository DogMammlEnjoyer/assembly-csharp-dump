using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Numerics
{
	public static class BitOperations
	{
		private unsafe static ReadOnlySpan<byte> TrailingZeroCountDeBruijn
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.3BF63951626584EB1653F9B8DBB590A5EE1EAE1135A904B9317C3773896DF076), 32);
			}
		}

		private unsafe static ReadOnlySpan<byte> Log2DeBruijn
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.4BCD43D478B9229AB7A13406353712C7944B60348C36B4D0E6B789D10F697652), 32);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(uint value)
		{
			if (value == 0U)
			{
				return 32;
			}
			return 31 - BitOperations.Log2SoftwareFallback(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LeadingZeroCount(ulong value)
		{
			uint num = (uint)(value >> 32);
			if (num == 0U)
			{
				return 32 + BitOperations.LeadingZeroCount((uint)value);
			}
			return BitOperations.LeadingZeroCount(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2(uint value)
		{
			return BitOperations.Log2SoftwareFallback(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2(ulong value)
		{
			uint num = (uint)(value >> 32);
			if (num == 0U)
			{
				return BitOperations.Log2((uint)value);
			}
			return 32 + BitOperations.Log2(num);
		}

		private unsafe static int Log2SoftwareFallback(uint value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return (int)(*Unsafe.AddByteOffset<byte>(MemoryMarshal.GetReference<byte>(BitOperations.Log2DeBruijn), (IntPtr)((int)(value * 130329821U >> 27))));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(uint value)
		{
			value -= (value >> 1 & 1431655765U);
			value = (value & 858993459U) + (value >> 2 & 858993459U);
			value = (value + (value >> 4) & 252645135U) * 16843009U >> 24;
			return (int)value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int PopCount(ulong value)
		{
			if (IntPtr.Size == 4)
			{
				return BitOperations.PopCount((uint)value) + BitOperations.PopCount((uint)(value >> 32));
			}
			value -= (value >> 1 & 6148914691236517205UL);
			value = (value & 3689348814741910323UL) + (value >> 2 & 3689348814741910323UL);
			value = (value + (value >> 4) & 1085102592571150095UL) * 72340172838076673UL >> 56;
			return (int)value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(int value)
		{
			return BitOperations.TrailingZeroCount((uint)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int TrailingZeroCount(uint value)
		{
			if (value == 0U)
			{
				return 32;
			}
			return (int)(*Unsafe.AddByteOffset<byte>(MemoryMarshal.GetReference<byte>(BitOperations.TrailingZeroCountDeBruijn), (IntPtr)((int)((value & -value) * 125613361U >> 27))));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(long value)
		{
			return BitOperations.TrailingZeroCount((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(ulong value)
		{
			uint num = (uint)value;
			if (num == 0U)
			{
				return 32 + BitOperations.TrailingZeroCount((uint)(value >> 32));
			}
			return BitOperations.TrailingZeroCount(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateLeft(uint value, int offset)
		{
			return value << offset | value >> 32 - offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateLeft(ulong value, int offset)
		{
			return value << offset | value >> 64 - offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateRight(uint value, int offset)
		{
			return value >> offset | value << 32 - offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateRight(ulong value, int offset)
		{
			return value >> offset | value << 64 - offset;
		}
	}
}
