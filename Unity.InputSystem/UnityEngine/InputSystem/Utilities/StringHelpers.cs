using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace UnityEngine.InputSystem.Utilities
{
	internal static class StringHelpers
	{
		public static string Escape(this string str, string chars = "\n\t\r\\\"", string replacements = "ntr\\\"")
		{
			if (str == null)
			{
				return null;
			}
			bool flag = false;
			foreach (char value in str)
			{
				if (chars.Contains(value))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char value2 in str)
			{
				int num = chars.IndexOf(value2);
				if (num == -1)
				{
					stringBuilder.Append(value2);
				}
				else
				{
					stringBuilder.Append('\\');
					stringBuilder.Append(replacements[num]);
				}
			}
			return stringBuilder.ToString();
		}

		public static string Unescape(this string str, string chars = "ntr\\\"", string replacements = "\n\t\r\\\"")
		{
			if (str == null)
			{
				return str;
			}
			if (!str.Contains('\\'))
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c == '\\' && i < str.Length - 2)
				{
					i++;
					c = str[i];
					int num = chars.IndexOf(c);
					if (num != -1)
					{
						stringBuilder.Append(replacements[num]);
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static bool Contains(this string str, char ch)
		{
			return str != null && str.IndexOf(ch) != -1;
		}

		public static bool Contains(this string str, string text, StringComparison comparison)
		{
			return str != null && str.IndexOf(text, comparison) != -1;
		}

		public static string GetPlural(this string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str == "Mouse")
			{
				return "Mice";
			}
			if (str == "mouse")
			{
				return "mice";
			}
			if (str == "Axis")
			{
				return "Axes";
			}
			if (!(str == "axis"))
			{
				return str + "s";
			}
			return "axes";
		}

		public static string NicifyMemorySize(long numBytes)
		{
			if (numBytes > 1073741824L)
			{
				long num = numBytes / 1073741824L;
				float num2 = (float)(numBytes % 1073741824L) / 1f;
				return string.Format("{0} GB", (float)num + num2);
			}
			if (numBytes > 1048576L)
			{
				long num3 = numBytes / 1048576L;
				float num4 = (float)(numBytes % 1048576L) / 1f;
				return string.Format("{0} MB", (float)num3 + num4);
			}
			if (numBytes > 1024L)
			{
				long num5 = numBytes / 1024L;
				float num6 = (float)(numBytes % 1024L) / 1f;
				return string.Format("{0} KB", (float)num5 + num6);
			}
			return string.Format("{0} Bytes", numBytes);
		}

		public static bool FromNicifiedMemorySize(string text, out long result, long defaultMultiplier = 1L)
		{
			text = text.Trim();
			long num = defaultMultiplier;
			if (text.EndsWith("MB", StringComparison.InvariantCultureIgnoreCase))
			{
				num = 1048576L;
				text = text.Substring(0, text.Length - 2);
			}
			else if (text.EndsWith("GB", StringComparison.InvariantCultureIgnoreCase))
			{
				num = 1073741824L;
				text = text.Substring(0, text.Length - 2);
			}
			else if (text.EndsWith("KB", StringComparison.InvariantCultureIgnoreCase))
			{
				num = 1024L;
				text = text.Substring(0, text.Length - 2);
			}
			else if (text.EndsWith("Bytes", StringComparison.InvariantCultureIgnoreCase))
			{
				num = 1L;
				text = text.Substring(0, text.Length - "Bytes".Length);
			}
			long num2;
			if (!long.TryParse(text, out num2))
			{
				result = 0L;
				return false;
			}
			result = num2 * num;
			return true;
		}

		public static int CountOccurrences(this string str, char ch)
		{
			if (str == null)
			{
				return 0;
			}
			int length = str.Length;
			int i = 0;
			int num = 0;
			while (i < length)
			{
				int num2 = str.IndexOf(ch, i);
				if (num2 == -1)
				{
					break;
				}
				num++;
				i = num2 + 1;
			}
			return num;
		}

		public static IEnumerable<Substring> Tokenize(this string str)
		{
			int i = 0;
			int length = str.Length;
			while (i < length)
			{
				while (i < length && char.IsWhiteSpace(str[i]))
				{
					i++;
				}
				if (i == length)
				{
					break;
				}
				if (str[i] == '"')
				{
					i++;
					int endPos = i;
					while (endPos < length && str[endPos] != '"')
					{
						int num;
						if (str[endPos] == '\\' && endPos < length - 1)
						{
							num = endPos + 1;
							endPos = num;
						}
						num = endPos + 1;
						endPos = num;
					}
					yield return new Substring(str, i, endPos - i);
					i = endPos + 1;
				}
				else
				{
					int endPos = i;
					while (endPos < length && !char.IsWhiteSpace(str[endPos]))
					{
						int num = endPos + 1;
						endPos = num;
					}
					yield return new Substring(str, i, endPos - i);
					i = endPos;
				}
			}
			yield break;
		}

		public static IEnumerable<string> Split(this string str, Func<char, bool> predicate)
		{
			if (string.IsNullOrEmpty(str))
			{
				yield break;
			}
			int length = str.Length;
			int position = 0;
			while (position < length)
			{
				char arg = str[position];
				if (predicate(arg))
				{
					int num = position + 1;
					position = num;
				}
				else
				{
					int num2 = position;
					int num = position + 1;
					for (position = num; position < length; position = num)
					{
						arg = str[position];
						if (predicate(arg))
						{
							break;
						}
						num = position + 1;
					}
					int num3 = position;
					yield return str.Substring(num2, num3 - num2);
				}
			}
			yield break;
		}

		public static string Join<TValue>(string separator, params TValue[] values)
		{
			return StringHelpers.Join<TValue>(values, separator);
		}

		public static string Join<TValue>(IEnumerable<TValue> values, string separator)
		{
			string text = null;
			int num = 0;
			StringBuilder stringBuilder = null;
			foreach (TValue tvalue in values)
			{
				if (tvalue != null)
				{
					string text2 = tvalue.ToString();
					if (!string.IsNullOrEmpty(text2))
					{
						num++;
						if (num == 1)
						{
							text = text2;
						}
						else
						{
							if (num == 2)
							{
								stringBuilder = new StringBuilder();
								stringBuilder.Append(text);
							}
							stringBuilder.Append(separator);
							stringBuilder.Append(text2);
						}
					}
				}
			}
			if (num == 0)
			{
				return null;
			}
			if (num == 1)
			{
				return text;
			}
			return stringBuilder.ToString();
		}

		public static string MakeUniqueName<TExisting>(string baseName, IEnumerable<TExisting> existingSet, Func<TExisting, string> getNameFunc)
		{
			if (getNameFunc == null)
			{
				throw new ArgumentNullException("getNameFunc");
			}
			if (existingSet == null)
			{
				return baseName;
			}
			string text = baseName;
			bool flag = false;
			int num = 1;
			if (baseName.Length > 0)
			{
				int num2 = baseName.Length;
				while (num2 > 0 && char.IsDigit(baseName[num2 - 1]))
				{
					num2--;
				}
				if (num2 != baseName.Length)
				{
					num = int.Parse(baseName.Substring(num2)) + 1;
					baseName = baseName.Substring(0, num2);
				}
			}
			while (!flag)
			{
				flag = true;
				foreach (TExisting arg in existingSet)
				{
					if (getNameFunc(arg).Equals(text, StringComparison.InvariantCultureIgnoreCase))
					{
						text = string.Format("{0}{1}", baseName, num);
						flag = false;
						num++;
						break;
					}
				}
			}
			return text;
		}

		public static bool CharacterSeparatedListsHaveAtLeastOneCommonElement(string firstList, string secondList, char separator)
		{
			if (firstList == null)
			{
				throw new ArgumentNullException("firstList");
			}
			if (secondList == null)
			{
				throw new ArgumentNullException("secondList");
			}
			int i = 0;
			int length = firstList.Length;
			int length2 = secondList.Length;
			while (i < length)
			{
				if (firstList[i] == separator)
				{
					i++;
				}
				int num = i + 1;
				while (num < length && firstList[num] != separator)
				{
					num++;
				}
				int num2 = num - i;
				int num3;
				for (int j = 0; j < length2; j = num3 + 1)
				{
					if (secondList[j] == separator)
					{
						j++;
					}
					num3 = j + 1;
					while (num3 < length2 && secondList[num3] != separator)
					{
						num3++;
					}
					int num4 = num3 - j;
					if (num2 == num4)
					{
						int num5 = i;
						int num6 = j;
						bool flag = true;
						for (int k = 0; k < num2; k++)
						{
							char c = firstList[num5 + k];
							char c2 = secondList[num6 + k];
							if (char.ToLowerInvariant(c) != char.ToLowerInvariant(c2))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							return true;
						}
					}
				}
				i = num + 1;
			}
			return false;
		}

		public static int ParseInt(string str, int pos)
		{
			int num = 1;
			int num2 = 0;
			int length = str.Length;
			while (pos < length)
			{
				int num3 = (int)(str[pos] - '0');
				if (num3 < 0 || num3 > 9)
				{
					break;
				}
				num2 = num2 * num + num3;
				num *= 10;
				pos++;
			}
			return num2;
		}

		public static bool WriteStringToBuffer(string text, IntPtr buffer, int bufferSizeInCharacters)
		{
			uint num = 0U;
			return StringHelpers.WriteStringToBuffer(text, buffer, bufferSizeInCharacters, ref num);
		}

		public unsafe static bool WriteStringToBuffer(string text, IntPtr buffer, int bufferSizeInCharacters, ref uint offset)
		{
			if (buffer == IntPtr.Zero)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = string.IsNullOrEmpty(text) ? 0 : text.Length;
			if (num > 65535)
			{
				throw new ArgumentException(string.Format("String exceeds max size of {0} characters", ushort.MaxValue), "text");
			}
			long num2 = (long)((ulong)offset + (ulong)((long)(2 * num)) + 4UL);
			if (num2 > (long)bufferSizeInCharacters)
			{
				return false;
			}
			byte* ptr = (byte*)((void*)buffer) + offset;
			*(short*)ptr = (short)((ushort)num);
			ptr += 2;
			int i = 0;
			while (i < num)
			{
				*(short*)ptr = (short)text[i];
				i++;
				ptr += 2;
			}
			offset = (uint)num2;
			return true;
		}

		public static string ReadStringFromBuffer(IntPtr buffer, int bufferSize)
		{
			uint num = 0U;
			return StringHelpers.ReadStringFromBuffer(buffer, bufferSize, ref num);
		}

		public unsafe static string ReadStringFromBuffer(IntPtr buffer, int bufferSize, ref uint offset)
		{
			if (buffer == IntPtr.Zero)
			{
				throw new ArgumentNullException("buffer");
			}
			if ((ulong)(offset + 4U) > (ulong)((long)bufferSize))
			{
				return null;
			}
			byte* ptr = (byte*)((void*)buffer) + offset;
			ushort num = *(ushort*)ptr;
			ptr += 2;
			if (num == 0)
			{
				return null;
			}
			long num2 = (long)((ulong)offset + (ulong)((long)(2 * num)) + 4UL);
			if (num2 > (long)bufferSize)
			{
				return null;
			}
			string result = Marshal.PtrToStringUni(new IntPtr((void*)ptr), (int)num);
			offset = (uint)num2;
			return result;
		}

		public static bool IsPrintable(this char ch)
		{
			return !char.IsControl(ch) && !char.IsWhiteSpace(ch);
		}

		public static string WithAllWhitespaceStripped(this string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in str)
			{
				if (!char.IsWhiteSpace(c))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static bool InvariantEqualsIgnoreCase(this string left, string right)
		{
			if (string.IsNullOrEmpty(left))
			{
				return string.IsNullOrEmpty(right);
			}
			return string.Equals(left, right, StringComparison.InvariantCultureIgnoreCase);
		}

		public static string ExpandTemplateString(string template, Func<string, string> mapFunc)
		{
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentNullException("template");
			}
			if (mapFunc == null)
			{
				throw new ArgumentNullException("mapFunc");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length = template.Length;
			for (int i = 0; i < length; i++)
			{
				char c = template[i];
				if (c != '{')
				{
					stringBuilder.Append(c);
				}
				else
				{
					i++;
					int num = i;
					while (i < length && template[i] != '}')
					{
						i++;
					}
					string arg = template.Substring(num, i - num);
					string value = mapFunc(arg);
					stringBuilder.Append(value);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
