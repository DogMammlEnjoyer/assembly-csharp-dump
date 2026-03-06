using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fusion
{
	public static class UTF32Tools
	{
		public unsafe static UTF32Tools.ConversionResult Convert(string str, uint* dst, int dstCapacity)
		{
			bool flag = string.IsNullOrEmpty(str);
			UTF32Tools.ConversionResult result;
			if (flag)
			{
				result = default(UTF32Tools.ConversionResult);
			}
			else
			{
				char* ptr = str;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				result = UTF32Tools.Convert(ptr, str.Length, dst, dstCapacity);
			}
			return result;
		}

		public unsafe static UTF32Tools.ConversionResult Convert(char* str, int strLength, uint* dst, int dstCapacity)
		{
			int num = 0;
			int num2 = 0;
			while (num < dstCapacity && num2 < strLength)
			{
				char c = str[num2];
				bool flag = c - '\ud800' < 'ࠀ';
				bool flag2 = !flag;
				if (flag2)
				{
					dst[num] = (uint)c;
				}
				else
				{
					bool flag3 = char.IsHighSurrogate(c) && num2 < strLength - 1 && char.IsLowSurrogate(str[num2 + 1]);
					if (!flag3)
					{
						Assert.AlwaysFail(string.Format("Failed to convert character {0}", c));
						break;
					}
					char c2 = c;
					char c3 = str[++num2];
					dst[num] = (uint)((c2 - '\ud800') * 'Ѐ' + (c3 - '\udc00')) + 65536U;
				}
				num++;
				num2++;
			}
			return new UTF32Tools.ConversionResult(num, num2);
		}

		internal unsafe static int CompareOrdinal(uint* strA, int aLength, uint* strB, int bLength, bool ignoreCase)
		{
			bool flag = strA == null && aLength > 0;
			if (flag)
			{
				throw new ArgumentNullException("strA");
			}
			bool flag2 = strB == null && bLength > 0;
			if (flag2)
			{
				throw new ArgumentNullException("strB");
			}
			int num = Math.Min(aLength, bLength);
			bool flag3 = !ignoreCase;
			if (flag3)
			{
				for (int i = 0; i < num; i++)
				{
					int num2 = (int)(strA[i] - strB[i]);
					bool flag4 = num2 != 0;
					if (flag4)
					{
						return num2;
					}
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					bool flag5 = !UTF32Tools.IsValidCodePoint(strA[j]);
					if (flag5)
					{
						Assert.AlwaysFail(string.Format("Failed to convert character {0}", strA[j]));
					}
					else
					{
						bool flag6 = !UTF32Tools.IsValidCodePoint(strB[j]);
						if (flag6)
						{
							Assert.AlwaysFail(string.Format("Failed to convert character {0}", strB[j]));
						}
						else
						{
							int num3 = (int)(strA[j] - strB[j]);
							bool flag7 = num3 != 0;
							if (flag7)
							{
								uint num4 = UTF32Tools.ToLowerInvariant(strA[j]);
								bool flag8 = num4 != strB[j];
								if (flag8)
								{
									uint num5 = UTF32Tools.ToLowerInvariant(strB[j]);
									bool flag9 = num4 != num5;
									if (flag9)
									{
										return num3;
									}
								}
							}
						}
					}
				}
			}
			return aLength - bLength;
		}

		internal unsafe static int CompareOrdinal(string strA, uint* strB, int bLength, bool ignoreCase = false)
		{
			bool flag = strA == null;
			if (flag)
			{
				throw new ArgumentNullException("strA");
			}
			int length = strA.Length;
			char* ptr = strA;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			int num = 0;
			int num2 = 0;
			while (num < bLength && num2 < length)
			{
				char c = ptr[num2];
				bool flag2 = c - '\ud800' < 'ࠀ';
				bool flag3 = !flag2;
				if (flag3)
				{
					int num3 = (int)((uint)c - strB[num]);
					bool flag4 = num3 != 0;
					if (flag4)
					{
						bool flag5 = !ignoreCase;
						if (flag5)
						{
							return num3;
						}
						ValueTuple<char, char> valueTuple = UTF32Tools.ToUTF16(strB[num]);
						char item = valueTuple.Item1;
						char item2 = valueTuple.Item2;
						bool flag6 = item2 > '\0';
						if (flag6)
						{
							return num3;
						}
						num3 = (int)(char.ToLowerInvariant(c) - char.ToLowerInvariant(item));
						bool flag7 = num3 != 0;
						if (flag7)
						{
							return num3;
						}
					}
				}
				else
				{
					bool flag8 = char.IsHighSurrogate(c) && num2 < length - 1 && char.IsLowSurrogate(ptr[num2 + 1]);
					if (!flag8)
					{
						Assert.AlwaysFail(string.Format("Failed to convert character {0}", c));
						break;
					}
					char charOrHighSurrogate = c;
					char lowSurrogate = ptr[(IntPtr)(++num2) * 2];
					uint num4 = UTF32Tools.ToUTF32(charOrHighSurrogate, lowSurrogate);
					int result = (int)(num4 - strB[num]);
					bool flag9 = num4 != strB[num];
					if (flag9)
					{
						if (!ignoreCase)
						{
							return result;
						}
						uint num5 = UTF32Tools.ToLowerInvariant(num4);
						bool flag10 = num5 == strB[num];
						if (!flag10)
						{
							uint num6 = UTF32Tools.ToLowerInvariant(strB[num]);
							bool flag11 = num5 == num6;
							if (!flag11)
							{
								return result;
							}
						}
					}
				}
				IL_1CA:
				num++;
				num2++;
				continue;
				goto IL_1CA;
			}
			return length - num2 - (bLength - num);
		}

		internal unsafe static bool EndsWithOrdinal(uint* strA, int aLength, uint* bStr, int bLength, bool ignoreCase = false)
		{
			bool flag = bLength > aLength;
			return !flag && UTF32Tools.CompareOrdinal(strA + (aLength - bLength), bLength, bStr, bLength, ignoreCase) == 0;
		}

		internal unsafe static bool EndsWithOrdinal(uint* strA, int aLength, string strB, bool ignoreCase = false)
		{
			bool flag = strB == null;
			if (flag)
			{
				throw new ArgumentNullException("strB");
			}
			int byteCount = Encoding.UTF32.GetByteCount(strB);
			Assert.Check(byteCount % 4 == 0);
			int num = byteCount / 4;
			bool flag2 = aLength < num;
			return !flag2 && UTF32Tools.CompareOrdinal(strB, strA + (aLength - num), num, ignoreCase) == 0;
		}

		internal unsafe static int GetHashDeterministic(uint* str, int length)
		{
			int num = 352654597;
			int num2 = num;
			for (int i = 0; i < length; i++)
			{
				ValueTuple<char, char> valueTuple = UTF32Tools.ToUTF16(str[i]);
				char item = valueTuple.Item1;
				char item2 = valueTuple.Item2;
				num = ((num << 5) + num ^ (int)item);
				UTF32Tools.Swap(ref num, ref num2);
				bool flag = item2 > '\0';
				if (flag)
				{
					num = ((num << 5) + num ^ (int)item2);
					UTF32Tools.Swap(ref num, ref num2);
				}
			}
			return num + num2 * 1566083941;
		}

		internal unsafe static bool StartsWithOrdinal(uint* strA, int aLength, uint* strB, int bLength, bool ignoreCase = false)
		{
			bool flag = bLength > aLength;
			return !flag && UTF32Tools.CompareOrdinal(strA, bLength, strB, bLength, ignoreCase) == 0;
		}

		internal unsafe static bool StartsWithOrdinal(uint* strA, int aLength, string strB, bool ignoreCase = false)
		{
			bool flag = strB == null;
			if (flag)
			{
				throw new ArgumentNullException("strB");
			}
			int byteCount = Encoding.UTF32.GetByteCount(strB);
			Assert.Check(byteCount % 4 == 0);
			int num = byteCount / 4;
			bool flag2 = aLength < num;
			return !flag2 && UTF32Tools.CompareOrdinal(strB, strA, num, ignoreCase) == 0;
		}

		internal unsafe static int IndexOf(uint* str, int length, string pattern)
		{
			bool flag = length < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			bool flag2 = str == null;
			if (flag2)
			{
				throw new ArgumentNullException("str");
			}
			bool flag3 = pattern == null;
			if (flag3)
			{
				throw new ArgumentNullException("pattern");
			}
			bool flag4 = length == 0;
			int result;
			if (flag4)
			{
				result = -1;
			}
			else
			{
				int length2 = UTF32Tools.GetLength(pattern);
				bool flag5 = length2 > length;
				if (flag5)
				{
					result = -1;
				}
				else
				{
					fixed (string text = pattern)
					{
						char* ptr = text;
						if (ptr != null)
						{
							ptr += RuntimeHelpers.OffsetToStringData / 2;
						}
						char* end = ptr + pattern.Length;
						int num = 0;
						while (num + length2 <= length)
						{
							char* ptr2 = ptr;
							int i;
							for (i = 0; i < length2; i++)
							{
								uint num2 = UTF32Tools.ReadNextCodePoint(ref ptr2, end);
								bool flag6 = str[num + i] != num2;
								if (flag6)
								{
									break;
								}
							}
							bool flag7 = i == length2;
							if (flag7)
							{
								return num;
							}
							num++;
						}
					}
					result = -1;
				}
			}
			return result;
		}

		internal unsafe static int IndexOf(uint* str, int length, uint* pattern, int patternLength)
		{
			bool flag = length < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			bool flag2 = patternLength < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("patternLength");
			}
			bool flag3 = str == null;
			if (flag3)
			{
				throw new ArgumentNullException("str");
			}
			bool flag4 = pattern == null;
			if (flag4)
			{
				throw new ArgumentNullException("pattern");
			}
			bool flag5 = length == 0 || patternLength > length;
			int result;
			if (flag5)
			{
				result = -1;
			}
			else
			{
				int num = 0;
				while (num + patternLength <= length)
				{
					int i;
					for (i = 0; i < patternLength; i++)
					{
						bool flag6 = str[num + i] != pattern[i];
						if (flag6)
						{
							break;
						}
					}
					bool flag7 = i == patternLength;
					if (flag7)
					{
						return num;
					}
					num++;
				}
				result = -1;
			}
			return result;
		}

		internal unsafe static void ToLowerInvariant(uint* src, uint* dst, int length)
		{
			bool flag = src == null;
			if (flag)
			{
				throw new ArgumentNullException("src");
			}
			bool flag2 = dst == null;
			if (flag2)
			{
				throw new ArgumentNullException("dst");
			}
			bool flag3 = length < 0;
			if (flag3)
			{
				throw new ArgumentNullException("length");
			}
			for (int i = 0; i < length; i++)
			{
				dst[i] = UTF32Tools.ToLowerInvariant(src[i]);
			}
		}

		internal unsafe static void ToUpperInvariant(uint* src, uint* dst, int length)
		{
			bool flag = src == null;
			if (flag)
			{
				throw new ArgumentNullException("src");
			}
			bool flag2 = dst == null;
			if (flag2)
			{
				throw new ArgumentNullException("dst");
			}
			bool flag3 = length < 0;
			if (flag3)
			{
				throw new ArgumentNullException("length");
			}
			for (int i = 0; i < length; i++)
			{
				dst[i] = UTF32Tools.ToUpperInvariant(src[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static char GetHighSurrogate(uint scalar)
		{
			return (char)((scalar - 65536U) / 1024U + 55296U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetLength(string str)
		{
			int byteCount = Encoding.UTF32.GetByteCount(str);
			Assert.Check(byteCount % 4 == 0);
			return byteCount / 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static char GetLowSurrogate(uint scalar)
		{
			return (char)((scalar - 65536U) % 1024U + 56320U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsValidCodePoint(uint scalar)
		{
			return (scalar - 1114112U ^ 55296U) >= 4293855232U;
		}

		private unsafe static uint ReadNextCodePoint(ref char* pstr, char* end)
		{
			char* ptr = pstr;
			pstr = ptr + 1;
			char c = *ptr;
			bool flag = char.IsHighSurrogate(c);
			uint result;
			if (flag)
			{
				Assert.Always(pstr < end, "Surrogate found at the end of the string");
				ptr = pstr;
				pstr = ptr + 1;
				char c2 = *ptr;
				Assert.Check(char.IsLowSurrogate(c2));
				result = (uint)((c - '\ud800') * 'Ѐ' + (c2 - '\udc00')) + 65536U;
			}
			else
			{
				Assert.Check(!char.IsLowSurrogate(c));
				result = (uint)c;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Swap(ref int a, ref int b)
		{
			int num = a;
			a = b;
			b = num;
		}

		private unsafe static uint ToLowerInvariant(uint value)
		{
			ValueTuple<char, char> valueTuple = UTF32Tools.ToUTF16(value);
			char item = valueTuple.Item1;
			char item2 = valueTuple.Item2;
			bool flag = item2 == '\0';
			uint result;
			if (flag)
			{
				result = (uint)char.ToLowerInvariant(item);
			}
			else
			{
				char* ptr = stackalloc char[(UIntPtr)4];
				*ptr = item;
				ptr[1] = item2;
				string text = new string(ptr, 0, 2);
				string text2 = text.ToLowerInvariant();
				Assert.Check(text2.Length == 2);
				result = UTF32Tools.ToUTF32(text2[0], text2[1]);
			}
			return result;
		}

		private unsafe static uint ToUpperInvariant(uint value)
		{
			ValueTuple<char, char> valueTuple = UTF32Tools.ToUTF16(value);
			char item = valueTuple.Item1;
			char item2 = valueTuple.Item2;
			bool flag = item2 == '\0';
			uint result;
			if (flag)
			{
				result = (uint)char.ToUpperInvariant(item);
			}
			else
			{
				char* ptr = stackalloc char[(UIntPtr)4];
				*ptr = item;
				ptr[1] = item2;
				string text = new string(ptr, 0, 2);
				string text2 = text.ToUpperInvariant();
				Assert.Check(text2.Length == 2);
				result = UTF32Tools.ToUTF32(text2[0], text2[1]);
			}
			return result;
		}

		private static ValueTuple<char, char> ToUTF16(uint scalar)
		{
			bool flag = scalar >= 65536U;
			ValueTuple<char, char> result;
			if (flag)
			{
				result = new ValueTuple<char, char>(UTF32Tools.GetHighSurrogate(scalar), UTF32Tools.GetLowSurrogate(scalar));
			}
			else
			{
				result = new ValueTuple<char, char>((char)scalar, '\0');
			}
			return result;
		}

		private static uint ToUTF32(char charOrHighSurrogate, char lowSurrogate = '\0')
		{
			bool flag = char.IsHighSurrogate(charOrHighSurrogate);
			uint result;
			if (flag)
			{
				Assert.Check(char.IsLowSurrogate(lowSurrogate));
				result = (uint)((charOrHighSurrogate - '\ud800') * 'Ѐ' + (lowSurrogate - '\udc00')) + 65536U;
			}
			else
			{
				Assert.Check(lowSurrogate == '\0');
				result = (uint)charOrHighSurrogate;
			}
			return result;
		}

		public struct CharEnumerator : IEnumerator<char>, IEnumerator, IDisposable
		{
			internal unsafe CharEnumerator(uint* utf32, int length)
			{
				this._index = 0;
				this.Current = (this._pendingLowSurrogate = '\0');
				this._ptr = utf32;
				this._length = length;
			}

			public char Current { readonly get; private set; }

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			public unsafe bool MoveNext()
			{
				bool flag = this._pendingLowSurrogate > '\0';
				bool result;
				if (flag)
				{
					this.Current = this._pendingLowSurrogate;
					this._pendingLowSurrogate = '\0';
					result = true;
				}
				else
				{
					bool flag2 = this._index >= this._length;
					if (flag2)
					{
						result = false;
					}
					else
					{
						IntPtr ptr = this._ptr;
						int index = this._index;
						this._index = index + 1;
						ValueTuple<char, char> valueTuple = UTF32Tools.ToUTF16(*(ptr + (IntPtr)index * 4));
						this.Current = valueTuple.Item1;
						this._pendingLowSurrogate = valueTuple.Item2;
						result = true;
					}
				}
				return result;
			}

			public void Reset()
			{
				this._index = 0;
			}

			private int _index;

			private int _length;

			private char _pendingLowSurrogate;

			private unsafe uint* _ptr;
		}

		public readonly struct ConversionResult
		{
			public ConversionResult(int words, int characters)
			{
				this.CodePointCount = words;
				this.CharacterCount = characters;
			}

			public readonly int CharacterCount;

			public readonly int CodePointCount;
		}
	}
}
