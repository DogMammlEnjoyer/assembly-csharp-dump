using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Fusion
{
	public static class Maths
	{
		public static uint QuaternionCompress(Quaternion rot)
		{
			Maths.FastAbs2 fastAbs = default(Maths.FastAbs2);
			fastAbs.single = rot.x;
			fastAbs.uint32 &= 2147483647U;
			float single = fastAbs.single;
			fastAbs.single = rot.y;
			fastAbs.uint32 &= 2147483647U;
			float single2 = fastAbs.single;
			fastAbs.single = rot.z;
			fastAbs.uint32 &= 2147483647U;
			float single3 = fastAbs.single;
			fastAbs.single = rot.w;
			fastAbs.uint32 &= 2147483647U;
			float single4 = fastAbs.single;
			int num = (single > single2) ? 0 : 1;
			int num2 = (single3 > single4) ? 2 : 3;
			int num3 = (((num == 0) ? single : single2) > ((num2 == 2) ? single3 : single4)) ? num : num2;
			float num4;
			float num5;
			float num6;
			float num7;
			switch (num3)
			{
			case 0:
				num4 = rot.y;
				num5 = rot.z;
				num6 = rot.w;
				num7 = rot.x;
				break;
			case 1:
				num4 = rot.x;
				num5 = rot.z;
				num6 = rot.w;
				num7 = rot.y;
				break;
			case 2:
				num4 = rot.x;
				num5 = rot.y;
				num6 = rot.w;
				num7 = rot.z;
				break;
			default:
				num4 = rot.x;
				num5 = rot.y;
				num6 = rot.z;
				num7 = rot.w;
				break;
			}
			bool flag = num7 > 0f;
			uint result;
			if (flag)
			{
				result = ((uint)(num4 * 724.0772f + 512f) | (uint)(num5 * 724.0772f + 512f) << 10 | (uint)(num6 * 724.0772f + 512f) << 20 | (uint)((uint)num3 << 30));
			}
			else
			{
				result = ((uint)(-num4 * 724.0772f + 512f) | (uint)(-num5 * 724.0772f + 512f) << 10 | (uint)(-num6 * 724.0772f + 512f) << 20 | (uint)((uint)num3 << 30));
			}
			return result;
		}

		public static Quaternion QuaternionDecompress(uint buffer)
		{
			int num = (int)(1023U & buffer);
			int num2 = (int)(1023U & buffer >> 10);
			int num3 = (int)(1023U & buffer >> 20);
			int num4 = (int)(buffer >> 30);
			float num5 = (float)((long)num - 512L) * 0.0013810681f;
			float num6 = (float)((long)num2 - 512L) * 0.0013810681f;
			float num7 = (float)((long)num3 - 512L) * 0.0013810681f;
			float num8 = (float)Math.Sqrt(1.0 - (double)(num5 * num5 + num6 * num6 + num7 * num7));
			Quaternion result;
			switch (num4)
			{
			case 0:
				result = new Quaternion(num8, num5, num6, num7);
				break;
			case 1:
				result = new Quaternion(num5, num8, num6, num7);
				break;
			case 2:
				result = new Quaternion(num5, num6, num8, num7);
				break;
			default:
				result = new Quaternion(num5, num6, num7, num8);
				break;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SizeOfBits<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return sizeof(T) * 8;
		}

		public static int BytesRequiredForBits(int b)
		{
			return b + 7 >> 3;
		}

		public static int IntsRequiredForBits(int b)
		{
			return b + 31 >> 5;
		}

		public static short BytesRequiredForBits(short b)
		{
			return (short)(b + 7 >> 3);
		}

		public unsafe static string PrintBits(byte* data, int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[lo ");
			for (int i = 0; i < count; i++)
			{
				byte b = data[i];
				for (int j = 0; j < 8; j++)
				{
					stringBuilder.Append((((int)b & 1 << j) == 1 << j) ? '1' : '0');
				}
				bool flag = i + 1 < count;
				if (flag)
				{
					stringBuilder.Append(" ");
				}
			}
			stringBuilder.Append(" hi]");
			return stringBuilder.ToString();
		}

		public static int BitsRequiredForNumber(int n)
		{
			for (int i = 31; i >= 0; i--)
			{
				int num = 1 << i;
				bool flag = (n & num) == num;
				if (flag)
				{
					return i + 1;
				}
			}
			return 0;
		}

		public static int FloorToInt(double value)
		{
			return (int)Math.Floor(value);
		}

		public static int CeilToInt(double value)
		{
			return (int)Math.Ceiling(value);
		}

		public static int CountUsedBitsMinOne(uint value)
		{
			Assert.Check(value > 0U);
			int num = 0;
			do
			{
				num++;
				value >>= 1;
			}
			while (value > 0U);
			return num;
		}

		public static int BitsRequiredForNumber(uint n)
		{
			for (int i = 31; i >= 0; i--)
			{
				int num = 1 << i;
				bool flag = ((ulong)n & (ulong)((long)num)) == (ulong)((long)num);
				if (flag)
				{
					return i + 1;
				}
			}
			return 0;
		}

		public static uint NextPowerOfTwo(uint v)
		{
			v -= 1U;
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v += 1U;
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSetBits(ulong x)
		{
			x -= (x >> 1 & 6148914691236517205UL);
			x = (x & 3689348814741910323UL) + (x >> 2 & 3689348814741910323UL);
			x = (x + (x >> 4) & 1085102592571150095UL);
			return (int)(x * 72340172838076673UL >> 56);
		}

		public static double MillisecondsToSeconds(double seconds)
		{
			return seconds / 1000.0;
		}

		public static long SecondsToMilliseconds(double seconds)
		{
			return (long)(seconds * 1000.0);
		}

		public static long SecondsToMicroseconds(double seconds)
		{
			return (long)(seconds * 1000000.0);
		}

		public static double MicrosecondsToSeconds(long microseconds)
		{
			return (double)microseconds / 1000000.0;
		}

		public static long MillisecondsToMicroseconds(long milliseconds)
		{
			return milliseconds * 1000L;
		}

		public static double CosineInterpolate(double a, double b, double t)
		{
			double num = (1.0 - Math.Cos(t * 3.141592653589793)) * 0.5;
			return a * (1.0 - num) + b * num;
		}

		public static byte ClampToByte(int v)
		{
			bool flag = v < 0;
			byte result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = v > 255;
				if (flag2)
				{
					result = byte.MaxValue;
				}
				else
				{
					result = (byte)v;
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ZigZagEncode(int i)
		{
			return i >> 31 ^ i << 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ZigZagDecode(int i)
		{
			return i >> 1 ^ -(i & 1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ZigZagEncode(long i)
		{
			return i >> 63 ^ i << 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ZigZagDecode(long i)
		{
			return i >> 1 ^ -(i & 1L);
		}

		public static int Clamp(int v, int min, int max)
		{
			bool flag = v < min;
			int result;
			if (flag)
			{
				result = min;
			}
			else
			{
				bool flag2 = v > max;
				if (flag2)
				{
					result = max;
				}
				else
				{
					result = v;
				}
			}
			return result;
		}

		public static uint Clamp(uint v, uint min, uint max)
		{
			bool flag = v < min;
			uint result;
			if (flag)
			{
				result = min;
			}
			else
			{
				bool flag2 = v > max;
				if (flag2)
				{
					result = max;
				}
				else
				{
					result = v;
				}
			}
			return result;
		}

		public static double Clamp(double v, double min, double max)
		{
			bool flag = v < min;
			double result;
			if (flag)
			{
				result = min;
			}
			else
			{
				bool flag2 = v > max;
				if (flag2)
				{
					result = max;
				}
				else
				{
					result = v;
				}
			}
			return result;
		}

		public static float Clamp(float v, float min, float max)
		{
			bool flag = v < min;
			float result;
			if (flag)
			{
				result = min;
			}
			else
			{
				bool flag2 = v > max;
				if (flag2)
				{
					result = max;
				}
				else
				{
					result = v;
				}
			}
			return result;
		}

		public static double Clamp01(double v)
		{
			bool flag = v < 0.0;
			double result;
			if (flag)
			{
				result = 0.0;
			}
			else
			{
				bool flag2 = v > 1.0;
				if (flag2)
				{
					result = 1.0;
				}
				else
				{
					result = v;
				}
			}
			return result;
		}

		public static float Clamp01(float v)
		{
			bool flag = v < 0f;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				bool flag2 = v > 1f;
				if (flag2)
				{
					result = 1f;
				}
				else
				{
					result = v;
				}
			}
			return result;
		}

		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * Maths.Clamp01(t);
		}

		public static double Lerp(double a, double b, double t)
		{
			return a + (b - a) * Maths.Clamp01(t);
		}

		public static uint Min(uint v, uint max)
		{
			bool flag = v > max;
			uint result;
			if (flag)
			{
				result = max;
			}
			else
			{
				result = v;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BitScanReverse(int v)
		{
			return Maths.BitScanReverse((uint)v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BitScanReverse(uint v)
		{
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			return (int)Maths._debruijnTable32[(int)(v * 130329821U >> 27)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BitScanReverse(long v)
		{
			return Maths.BitScanReverse((ulong)v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BitScanReverse(ulong v)
		{
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v |= v >> 32;
			return Maths.DeBruijnLookupLong[(int)(checked((IntPtr)(unchecked(v * 7783611145303519083UL) >> 57)))];
		}

		private const float ENRANGE = 1.4142133f;

		private const float UNRANGE = 0.7071069f;

		private const uint HALF_ENCODED = 512U;

		private const float ENCODER = 724.0772f;

		private const float DECODER = 0.0013810681f;

		private const uint MASK10BITS = 1023U;

		private static byte[] _debruijnTable32 = new byte[]
		{
			0,
			9,
			1,
			10,
			13,
			21,
			2,
			29,
			11,
			14,
			16,
			18,
			22,
			25,
			3,
			30,
			8,
			12,
			20,
			28,
			15,
			17,
			24,
			7,
			19,
			27,
			23,
			6,
			26,
			5,
			4,
			31
		};

		private static byte[] _debruijnTable64 = new byte[]
		{
			63,
			0,
			58,
			1,
			59,
			47,
			53,
			2,
			60,
			39,
			48,
			27,
			54,
			33,
			42,
			3,
			61,
			51,
			37,
			40,
			49,
			18,
			28,
			20,
			55,
			30,
			34,
			11,
			43,
			14,
			22,
			4,
			62,
			57,
			46,
			52,
			38,
			26,
			32,
			41,
			50,
			36,
			17,
			19,
			29,
			10,
			13,
			21,
			56,
			45,
			25,
			31,
			35,
			16,
			9,
			12,
			44,
			24,
			15,
			8,
			23,
			7,
			6,
			5
		};

		private static readonly int[] DeBruijnLookupLong = new int[]
		{
			0,
			48,
			-1,
			-1,
			31,
			-1,
			15,
			51,
			-1,
			63,
			5,
			-1,
			-1,
			-1,
			19,
			-1,
			23,
			28,
			-1,
			-1,
			-1,
			40,
			36,
			46,
			-1,
			13,
			-1,
			-1,
			-1,
			34,
			-1,
			58,
			-1,
			60,
			2,
			43,
			55,
			-1,
			-1,
			-1,
			50,
			62,
			4,
			-1,
			18,
			27,
			-1,
			39,
			45,
			-1,
			-1,
			33,
			57,
			-1,
			1,
			54,
			-1,
			49,
			-1,
			17,
			-1,
			-1,
			32,
			-1,
			53,
			-1,
			16,
			-1,
			-1,
			52,
			-1,
			-1,
			-1,
			64,
			6,
			7,
			8,
			-1,
			9,
			-1,
			-1,
			-1,
			20,
			10,
			-1,
			-1,
			24,
			-1,
			29,
			-1,
			-1,
			21,
			-1,
			11,
			-1,
			-1,
			41,
			-1,
			25,
			37,
			-1,
			47,
			-1,
			30,
			14,
			-1,
			-1,
			-1,
			-1,
			22,
			-1,
			-1,
			35,
			12,
			-1,
			-1,
			-1,
			59,
			42,
			-1,
			-1,
			61,
			3,
			26,
			38,
			44,
			-1,
			56
		};

		[StructLayout(LayoutKind.Explicit)]
		private struct FastAbs2
		{
			[FieldOffset(0)]
			public uint uint32;

			[FieldOffset(0)]
			public float single;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct FastAbs
		{
			public const uint Mask = 2147483647U;

			[FieldOffset(0)]
			public uint UInt32;

			[FieldOffset(0)]
			public float Single;
		}
	}
}
