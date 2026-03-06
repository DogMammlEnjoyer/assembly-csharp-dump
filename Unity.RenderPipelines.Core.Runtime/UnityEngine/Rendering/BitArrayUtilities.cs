using System;

namespace UnityEngine.Rendering
{
	public static class BitArrayUtilities
	{
		public static bool Get8(uint index, byte data)
		{
			return ((int)data & 1 << (int)index) != 0;
		}

		public static bool Get16(uint index, ushort data)
		{
			return ((int)data & 1 << (int)index) != 0;
		}

		public static bool Get32(uint index, uint data)
		{
			return (data & 1U << (int)index) > 0U;
		}

		public static bool Get64(uint index, ulong data)
		{
			return (data & 1UL << (int)index) > 0UL;
		}

		public static bool Get128(uint index, ulong data1, ulong data2)
		{
			if (index >= 64U)
			{
				return (data2 & 1UL << (int)(index - 64U)) > 0UL;
			}
			return (data1 & 1UL << (int)index) > 0UL;
		}

		public static bool Get256(uint index, ulong data1, ulong data2, ulong data3, ulong data4)
		{
			if (index >= 128U)
			{
				if (index >= 192U)
				{
					return (data4 & 1UL << (int)(index - 192U)) > 0UL;
				}
				return (data3 & 1UL << (int)(index - 128U)) > 0UL;
			}
			else
			{
				if (index >= 64U)
				{
					return (data2 & 1UL << (int)(index - 64U)) > 0UL;
				}
				return (data1 & 1UL << (int)index) > 0UL;
			}
		}

		public static void Set8(uint index, ref byte data, bool value)
		{
			data = (byte)(value ? ((int)data | 1 << (int)index) : ((int)data & ~(1 << (int)index)));
		}

		public static void Set16(uint index, ref ushort data, bool value)
		{
			data = (ushort)(value ? ((int)data | 1 << (int)index) : ((int)data & ~(1 << (int)index)));
		}

		public static void Set32(uint index, ref uint data, bool value)
		{
			data = (value ? (data | 1U << (int)index) : (data & ~(1U << (int)index)));
		}

		public static void Set64(uint index, ref ulong data, bool value)
		{
			data = (value ? (data | 1UL << (int)index) : (data & ~(1UL << (int)index)));
		}

		public static void Set128(uint index, ref ulong data1, ref ulong data2, bool value)
		{
			if (index < 64U)
			{
				data1 = (value ? (data1 | 1UL << (int)index) : (data1 & ~(1UL << (int)index)));
				return;
			}
			data2 = (value ? (data2 | 1UL << (int)(index - 64U)) : (data2 & ~(1UL << (int)(index - 64U))));
		}

		public static void Set256(uint index, ref ulong data1, ref ulong data2, ref ulong data3, ref ulong data4, bool value)
		{
			if (index < 64U)
			{
				data1 = (value ? (data1 | 1UL << (int)index) : (data1 & ~(1UL << (int)index)));
				return;
			}
			if (index < 128U)
			{
				data2 = (value ? (data2 | 1UL << (int)(index - 64U)) : (data2 & ~(1UL << (int)(index - 64U))));
				return;
			}
			if (index < 192U)
			{
				data3 = (value ? (data3 | 1UL << (int)(index - 64U)) : (data3 & ~(1UL << (int)(index - 128U))));
				return;
			}
			data4 = (value ? (data4 | 1UL << (int)(index - 64U)) : (data4 & ~(1UL << (int)(index - 192U))));
		}
	}
}
