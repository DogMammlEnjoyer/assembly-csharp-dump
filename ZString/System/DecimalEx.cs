using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	internal static class DecimalEx
	{
		private static ref DecimalEx.DecCalc AsMutable(ref decimal d)
		{
			return Unsafe.As<decimal, DecimalEx.DecCalc>(ref d);
		}

		internal static uint High(this decimal value)
		{
			return Unsafe.As<decimal, DecimalEx.DecCalc>(ref value).uhi;
		}

		internal static uint Low(this decimal value)
		{
			return Unsafe.As<decimal, DecimalEx.DecCalc>(ref value).ulo;
		}

		internal static uint Mid(this decimal value)
		{
			return Unsafe.As<decimal, DecimalEx.DecCalc>(ref value).umid;
		}

		internal static bool IsNegative(this decimal value)
		{
			return Unsafe.As<decimal, DecimalEx.DecimalBits>(ref value).flags < 0;
		}

		internal static int Scale(this decimal value)
		{
			return (int)((byte)(Unsafe.As<decimal, DecimalEx.DecimalBits>(ref value).flags >> 16));
		}

		internal static uint DecDivMod1E9(ref decimal value)
		{
			return DecimalEx.DecCalc.DecDivMod1E9(DecimalEx.AsMutable(ref value));
		}

		private const int ScaleShift = 16;

		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalBits
		{
			[FieldOffset(0)]
			public int flags;

			[FieldOffset(4)]
			public int hi;

			[FieldOffset(8)]
			public int lo;

			[FieldOffset(12)]
			public int mid;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct DecCalc
		{
			internal static uint DecDivMod1E9(ref DecimalEx.DecCalc value)
			{
				ulong num = ((ulong)value.uhi << 32) + (ulong)value.umid;
				ulong num2 = num / 1000000000UL;
				value.uhi = (uint)(num2 >> 32);
				value.umid = (uint)num2;
				ulong num3 = (num - (ulong)((uint)num2 * 1000000000U) << 32) + (ulong)value.ulo;
				uint num4 = (uint)(num3 / 1000000000UL);
				value.ulo = num4;
				return (uint)num3 - num4 * 1000000000U;
			}

			private const uint TenToPowerNine = 1000000000U;

			[FieldOffset(0)]
			public uint uflags;

			[FieldOffset(4)]
			public uint uhi;

			[FieldOffset(8)]
			public uint ulo;

			[FieldOffset(12)]
			public uint umid;

			[FieldOffset(8)]
			private ulong ulomidLE;
		}
	}
}
