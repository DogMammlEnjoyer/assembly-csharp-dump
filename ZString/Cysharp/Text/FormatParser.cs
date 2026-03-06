using System;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	internal static class FormatParser
	{
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ParserScanResult ScanFormatString(string format, ref int i)
		{
			int length = format.Length;
			char c = format[i];
			i++;
			if (c == '}')
			{
				if (i < length && format[i] == '}')
				{
					i++;
					return ParserScanResult.EscapedChar;
				}
				ExceptionUtil.ThrowFormatError();
				return ParserScanResult.NormalChar;
			}
			else
			{
				if (c != '{')
				{
					return ParserScanResult.NormalChar;
				}
				if (i < length && format[i] == '{')
				{
					i++;
					return ParserScanResult.EscapedChar;
				}
				i--;
				return ParserScanResult.BraceOpen;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ParserScanResult ScanFormatString(ReadOnlySpan<char> format, ref int i)
		{
			int length = format.Length;
			char c = (char)(*format[i]);
			i++;
			if (c == '}')
			{
				if (i < length && *format[i] == 125)
				{
					i++;
					return ParserScanResult.EscapedChar;
				}
				ExceptionUtil.ThrowFormatError();
				return ParserScanResult.NormalChar;
			}
			else
			{
				if (c != '{')
				{
					return ParserScanResult.NormalChar;
				}
				if (i < length && *format[i] == 123)
				{
					i++;
					return ParserScanResult.EscapedChar;
				}
				i--;
				return ParserScanResult.BraceOpen;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsDigit(char c)
		{
			return '0' <= c && c <= '9';
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static FormatParser.ParseResult Parse(ReadOnlySpan<char> format, int i)
		{
			char c = '\0';
			int length = format.Length;
			i++;
			if (i == length || !FormatParser.IsDigit(c = (char)(*format[i])))
			{
				ExceptionUtil.ThrowFormatError();
			}
			int num = 0;
			do
			{
				num = num * 10 + (int)c - 48;
				if (++i == length)
				{
					ExceptionUtil.ThrowFormatError();
				}
				c = (char)(*format[i]);
			}
			while (FormatParser.IsDigit(c) && num < 16);
			if (num >= 16)
			{
				ExceptionUtil.ThrowFormatException();
			}
			while (i < length && (c = (char)(*format[i])) == ' ')
			{
				i++;
			}
			int num2 = 0;
			if (c == ',')
			{
				i++;
				while (i < length && (c = (char)(*format[i])) == ' ')
				{
					i++;
				}
				if (i == length)
				{
					ExceptionUtil.ThrowFormatError();
				}
				bool flag = false;
				if (c == '-')
				{
					flag = true;
					if (++i == length)
					{
						ExceptionUtil.ThrowFormatError();
					}
					c = (char)(*format[i]);
				}
				if (!FormatParser.IsDigit(c))
				{
					ExceptionUtil.ThrowFormatError();
				}
				do
				{
					num2 = num2 * 10 + (int)c - 48;
					if (++i == length)
					{
						ExceptionUtil.ThrowFormatError();
					}
					c = (char)(*format[i]);
				}
				while (FormatParser.IsDigit(c) && num2 < 1000);
				if (flag)
				{
					num2 *= -1;
				}
			}
			while (i < length && (c = (char)(*format[i])) == ' ')
			{
				i++;
			}
			ReadOnlySpan<char> formatString = default(ReadOnlySpan<char>);
			if (c == ':')
			{
				i++;
				int num3 = i;
				for (;;)
				{
					if (i == length)
					{
						ExceptionUtil.ThrowFormatError();
					}
					c = (char)(*format[i]);
					if (c == '}')
					{
						break;
					}
					if (c == '{')
					{
						ExceptionUtil.ThrowFormatError();
					}
					i++;
				}
				if (i > num3)
				{
					formatString = format.Slice(num3, i - num3);
				}
			}
			else if (c != '}')
			{
				ExceptionUtil.ThrowFormatError();
			}
			i++;
			return new FormatParser.ParseResult(num, formatString, i, num2);
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FormatParser.ParseResult Parse(string format, int i)
		{
			char c = '\0';
			int length = format.Length;
			i++;
			if (i == length || !FormatParser.IsDigit(c = format[i]))
			{
				ExceptionUtil.ThrowFormatError();
			}
			int num = 0;
			do
			{
				num = num * 10 + (int)c - 48;
				if (++i == length)
				{
					ExceptionUtil.ThrowFormatError();
				}
				c = format[i];
			}
			while (FormatParser.IsDigit(c) && num < 16);
			if (num >= 16)
			{
				ExceptionUtil.ThrowFormatException();
			}
			while (i < length && (c = format[i]) == ' ')
			{
				i++;
			}
			int num2 = 0;
			if (c == ',')
			{
				i++;
				while (i < length && (c = format[i]) == ' ')
				{
					i++;
				}
				if (i == length)
				{
					ExceptionUtil.ThrowFormatError();
				}
				bool flag = false;
				if (c == '-')
				{
					flag = true;
					if (++i == length)
					{
						ExceptionUtil.ThrowFormatError();
					}
					c = format[i];
				}
				if (!FormatParser.IsDigit(c))
				{
					ExceptionUtil.ThrowFormatError();
				}
				do
				{
					num2 = num2 * 10 + (int)c - 48;
					if (++i == length)
					{
						ExceptionUtil.ThrowFormatError();
					}
					c = format[i];
				}
				while (FormatParser.IsDigit(c) && num2 < 1000);
				if (flag)
				{
					num2 *= -1;
				}
			}
			while (i < length && (c = format[i]) == ' ')
			{
				i++;
			}
			ReadOnlySpan<char> formatString = default(ReadOnlySpan<char>);
			if (c == ':')
			{
				i++;
				int num3 = i;
				for (;;)
				{
					if (i == length)
					{
						ExceptionUtil.ThrowFormatError();
					}
					c = format[i];
					if (c == '}')
					{
						break;
					}
					if (c == '{')
					{
						ExceptionUtil.ThrowFormatError();
					}
					i++;
				}
				if (i > num3)
				{
					formatString = format.AsSpan(num3, i - num3);
				}
			}
			else if (c != '}')
			{
				ExceptionUtil.ThrowFormatError();
			}
			i++;
			return new FormatParser.ParseResult(num, formatString, i, num2);
		}

		internal const int ArgLengthLimit = 16;

		internal const int WidthLimit = 1000;

		public readonly ref struct ParseResult
		{
			public ParseResult(int index, ReadOnlySpan<char> formatString, int lastIndex, int alignment)
			{
				this.Index = index;
				this.FormatString = formatString;
				this.LastIndex = lastIndex;
				this.Alignment = alignment;
			}

			public readonly int Index;

			public readonly ReadOnlySpan<char> FormatString;

			public readonly int LastIndex;

			public readonly int Alignment;
		}
	}
}
