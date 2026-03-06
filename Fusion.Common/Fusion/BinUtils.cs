using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusion
{
	public static class BinUtils
	{
		public static string ByteToHex(byte value)
		{
			return BinUtils._byteHexValue[(int)value];
		}

		public unsafe static string BytesToHex(byte* buffer, int length, int columns = 16, string rowSeparator = "\n", string columnSeparator = " ")
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < length)
			{
				stringBuilder.Append(BinUtils._byteHexValue[(int)buffer[i++]]);
				bool flag = i == length;
				if (flag)
				{
					break;
				}
				bool flag2 = i % columns == 0;
				if (flag2)
				{
					stringBuilder.Append(rowSeparator);
				}
				else
				{
					stringBuilder.Append(columnSeparator);
				}
			}
			return stringBuilder.ToString();
		}

		public unsafe static string WordsToHex(int* buffer, int length, int columns = 4, string rowSeparator = "\n", string columnSeparator = " ")
		{
			return BinUtils.WordsToHex(new ReadOnlySpan<uint>((void*)buffer, length), columns, rowSeparator, columnSeparator);
		}

		public unsafe static string WordsToHex(uint* buffer, int length, int columns = 4, string rowSeparator = "\n", string columnSeparator = " ")
		{
			return BinUtils.WordsToHex(new ReadOnlySpan<uint>((void*)buffer, length), columns, rowSeparator, columnSeparator);
		}

		public static string WordsToHex(ReadOnlySpan<int> buffer, int columns = 4, string rowSeparator = "\n", string columnSeparator = " ")
		{
			return BinUtils.WordsToHex(MemoryMarshal.Cast<int, uint>(buffer), columns, rowSeparator, columnSeparator);
		}

		public unsafe static string WordsToHex(ReadOnlySpan<uint> buffer, int columns = 4, string rowSeparator = "\n", string columnSeparator = " ")
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < buffer.Length)
			{
				stringBuilder.Append(BinUtils._byteHexValue[(int)(255U & *buffer[i] >> 24)]);
				stringBuilder.Append(BinUtils._byteHexValue[(int)(255U & *buffer[i] >> 16)]);
				stringBuilder.Append(BinUtils._byteHexValue[(int)(255U & *buffer[i] >> 8)]);
				stringBuilder.Append(BinUtils._byteHexValue[(int)(255U & *buffer[i])]);
				bool flag = ++i % columns == 0;
				if (flag)
				{
					stringBuilder.Append(rowSeparator);
				}
				else
				{
					stringBuilder.Append(columnSeparator);
				}
			}
			return stringBuilder.ToString();
		}

		private static bool TryHexToByte(char c, out byte result)
		{
			bool flag = c >= '0' && c <= '9';
			bool result2;
			if (flag)
			{
				result = (byte)(c - '0');
				result2 = true;
			}
			else
			{
				bool flag2 = c >= 'a' && c <= 'f';
				if (flag2)
				{
					result = (byte)('\n' + c - 'a');
					result2 = true;
				}
				else
				{
					bool flag3 = c >= 'A' && c <= 'F';
					if (flag3)
					{
						result = (byte)('\n' + c - 'A');
						result2 = true;
					}
					else
					{
						result = 0;
						result2 = false;
					}
				}
			}
			return result2;
		}

		public unsafe static int HexToBytes(string str, byte* buffer, int length)
		{
			int num = 0;
			int num2 = 0;
			while (num < str.Length && num2 < length)
			{
				byte b;
				bool flag = BinUtils.TryHexToByte(str[num], out b);
				if (!flag)
				{
					goto IL_6D;
				}
				num++;
				bool flag2 = num == str.Length;
				if (flag2)
				{
					buffer[num2++] = b;
					break;
				}
				byte b2;
				bool flag3 = BinUtils.TryHexToByte(str[num], out b2);
				if (!flag3)
				{
					buffer[num2++] = b;
					goto IL_6D;
				}
				buffer[num2++] = 16 * b + b2;
				IL_82:
				num++;
				continue;
				IL_6D:
				bool flag4 = !char.IsWhiteSpace(str, num);
				if (flag4)
				{
					break;
				}
				goto IL_82;
			}
			return num;
		}

		public unsafe static ValueTuple<int, int> HexToInts(string str, int* buffer, int length)
		{
			int num = 0;
			int num2 = 0;
			while (num2 < length && num < str.Length)
			{
				int num3 = 0;
				int num4 = 0;
				while (num4 < 8 && num < str.Length)
				{
					char c = str[num++];
					byte b;
					bool flag = BinUtils.TryHexToByte(c, out b);
					if (flag)
					{
						num3 = (num3 << 4 | (int)b);
						num4++;
					}
					else
					{
						bool flag2 = !char.IsWhiteSpace(c);
						if (flag2)
						{
							return new ValueTuple<int, int>(num, num2);
						}
						while (num < str.Length && char.IsWhiteSpace(str[num]))
						{
							num++;
						}
						break;
					}
				}
				buffer[num2] = num3;
				num2++;
			}
			return new ValueTuple<int, int>(num, num2);
		}

		public unsafe static string BytesToHex(byte[] buffer, int columns = 16)
		{
			bool flag = buffer == null;
			string result;
			if (flag)
			{
				result = "<null>";
			}
			else
			{
				bool flag2 = buffer.Length == 0;
				if (flag2)
				{
					result = "<empty>";
				}
				else
				{
					byte* buffer2;
					if (buffer == null || buffer.Length == 0)
					{
						buffer2 = null;
					}
					else
					{
						buffer2 = &buffer[0];
					}
					result = BinUtils.BytesToHex(buffer2, buffer.Length, columns, "\n", " ");
				}
			}
			return result;
		}

		public unsafe static string BytesToHex(ReadOnlySpan<byte> buffer, int columns = 16)
		{
			bool flag = buffer.Length == 0;
			string result;
			if (flag)
			{
				result = "<empty>";
			}
			else
			{
				fixed (byte* pinnableReference = buffer.GetPinnableReference())
				{
					byte* buffer2 = pinnableReference;
					result = BinUtils.BytesToHex(buffer2, buffer.Length, columns, "\n", " ");
				}
			}
			return result;
		}

		internal static void RepeatingCopyTo(this ReadOnlySpan<byte> src, Span<byte> dst)
		{
			bool isEmpty = src.IsEmpty;
			if (!isEmpty)
			{
				while (dst.Length >= src.Length)
				{
					src.CopyTo(dst);
					ref Span<byte> ptr = ref dst;
					int length = src.Length;
					dst = ptr.Slice(length, ptr.Length - length);
				}
				bool flag = dst.Length > 0;
				if (flag)
				{
					src.Slice(0, dst.Length).CopyTo(dst);
				}
			}
		}

		internal static bool RepeatingSequenceEqualTo(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> other)
		{
			while (span.Length >= other.Length)
			{
				bool flag = !span.Slice(0, other.Length).SequenceEqual(other);
				if (flag)
				{
					return false;
				}
				ref ReadOnlySpan<byte> ptr = ref span;
				int length = other.Length;
				span = ptr.Slice(length, ptr.Length - length);
			}
			bool flag2 = span.Length > 0;
			if (flag2)
			{
				bool flag3 = !span.SequenceEqual(other.Slice(0, span.Length));
				if (flag3)
				{
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T Read<[IsUnmanaged] T>(this Span<byte> source) where T : struct, ValueType
		{
			Assert.Always<int, int>(source.Length >= sizeof(T), source.Length, sizeof(T));
			return *Unsafe.As<byte, T>(source[0]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T Read<[IsUnmanaged] T>(this Span<int> source) where T : struct, ValueType
		{
			Assert.Always<int, int>(source.Length * 4 >= sizeof(T), source.Length, sizeof(T));
			return *Unsafe.As<int, T>(source[0]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<[IsUnmanaged] T>(this Span<byte> source) where T : struct, ValueType
		{
			Assert.Always<int, int>(source.Length >= sizeof(T), source.Length, sizeof(T));
			return Unsafe.As<byte, T>(source[0]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<[IsUnmanaged] T>(this Span<int> source) where T : struct, ValueType
		{
			Assert.Always<int, int>(source.Length * 4 >= sizeof(T), source.Length, sizeof(T));
			return Unsafe.As<int, T>(source[0]);
		}

		public unsafe static T* AsPointer<[IsUnmanaged] T>(this Span<byte> source) where T : struct, ValueType
		{
			Assert.Always<int, int>(source.Length >= sizeof(T), source.Length, sizeof(T));
			return (T*)Unsafe.AsPointer<byte>(source[0]);
		}

		public unsafe static T* AsPointer<[IsUnmanaged] T>(this Span<int> source) where T : struct, ValueType
		{
			Assert.Always<int, int>(source.Length * 4 >= sizeof(T), source.Length, sizeof(T));
			return (T*)Unsafe.AsPointer<int>(source[0]);
		}

		private static readonly string[] _byteHexValue = new string[]
		{
			"00",
			"01",
			"02",
			"03",
			"04",
			"05",
			"06",
			"07",
			"08",
			"09",
			"0A",
			"0B",
			"0C",
			"0D",
			"0E",
			"0F",
			"10",
			"11",
			"12",
			"13",
			"14",
			"15",
			"16",
			"17",
			"18",
			"19",
			"1A",
			"1B",
			"1C",
			"1D",
			"1E",
			"1F",
			"20",
			"21",
			"22",
			"23",
			"24",
			"25",
			"26",
			"27",
			"28",
			"29",
			"2A",
			"2B",
			"2C",
			"2D",
			"2E",
			"2F",
			"30",
			"31",
			"32",
			"33",
			"34",
			"35",
			"36",
			"37",
			"38",
			"39",
			"3A",
			"3B",
			"3C",
			"3D",
			"3E",
			"3F",
			"40",
			"41",
			"42",
			"43",
			"44",
			"45",
			"46",
			"47",
			"48",
			"49",
			"4A",
			"4B",
			"4C",
			"4D",
			"4E",
			"4F",
			"50",
			"51",
			"52",
			"53",
			"54",
			"55",
			"56",
			"57",
			"58",
			"59",
			"5A",
			"5B",
			"5C",
			"5D",
			"5E",
			"5F",
			"60",
			"61",
			"62",
			"63",
			"64",
			"65",
			"66",
			"67",
			"68",
			"69",
			"6A",
			"6B",
			"6C",
			"6D",
			"6E",
			"6F",
			"70",
			"71",
			"72",
			"73",
			"74",
			"75",
			"76",
			"77",
			"78",
			"79",
			"7A",
			"7B",
			"7C",
			"7D",
			"7E",
			"7F",
			"80",
			"81",
			"82",
			"83",
			"84",
			"85",
			"86",
			"87",
			"88",
			"89",
			"8A",
			"8B",
			"8C",
			"8D",
			"8E",
			"8F",
			"90",
			"91",
			"92",
			"93",
			"94",
			"95",
			"96",
			"97",
			"98",
			"99",
			"9A",
			"9B",
			"9C",
			"9D",
			"9E",
			"9F",
			"A0",
			"A1",
			"A2",
			"A3",
			"A4",
			"A5",
			"A6",
			"A7",
			"A8",
			"A9",
			"AA",
			"AB",
			"AC",
			"AD",
			"AE",
			"AF",
			"B0",
			"B1",
			"B2",
			"B3",
			"B4",
			"B5",
			"B6",
			"B7",
			"B8",
			"B9",
			"BA",
			"BB",
			"BC",
			"BD",
			"BE",
			"BF",
			"C0",
			"C1",
			"C2",
			"C3",
			"C4",
			"C5",
			"C6",
			"C7",
			"C8",
			"C9",
			"CA",
			"CB",
			"CC",
			"CD",
			"CE",
			"CF",
			"D0",
			"D1",
			"D2",
			"D3",
			"D4",
			"D5",
			"D6",
			"D7",
			"D8",
			"D9",
			"DA",
			"DB",
			"DC",
			"DD",
			"DE",
			"DF",
			"E0",
			"E1",
			"E2",
			"E3",
			"E4",
			"E5",
			"E6",
			"E7",
			"E8",
			"E9",
			"EA",
			"EB",
			"EC",
			"ED",
			"EE",
			"EF",
			"F0",
			"F1",
			"F2",
			"F3",
			"F4",
			"F5",
			"F6",
			"F7",
			"F8",
			"F9",
			"FA",
			"FB",
			"FC",
			"FD",
			"FE",
			"FF"
		};
	}
}
