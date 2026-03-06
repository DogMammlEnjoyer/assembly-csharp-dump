using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class PreparedFormatHelper
	{
		internal static Utf16FormatSegment[] Utf16Parse(string format)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			List<Utf16FormatSegment> list = new List<Utf16FormatSegment>();
			int num = 0;
			int length = format.Length;
			int num2 = 0;
			for (;;)
			{
				if (num < length)
				{
					ParserScanResult parserScanResult = FormatParser.ScanFormatString(format, ref num);
					if (ParserScanResult.NormalChar == parserScanResult && num < length)
					{
						continue;
					}
					int num3 = num - num2;
					if (ParserScanResult.EscapedChar == parserScanResult)
					{
						num3--;
					}
					if (num3 != 0)
					{
						list.Add(new Utf16FormatSegment(num2, num3, -1, 0));
					}
					num2 = num;
					if (parserScanResult != ParserScanResult.BraceOpen)
					{
						continue;
					}
				}
				if (num >= length)
				{
					break;
				}
				FormatParser.ParseResult parseResult = FormatParser.Parse(format, num);
				num2 = parseResult.LastIndex;
				num = parseResult.LastIndex;
				list.Add(new Utf16FormatSegment(parseResult.LastIndex - parseResult.FormatString.Length - 1, parseResult.FormatString.Length, parseResult.Index, parseResult.Alignment));
			}
			return list.ToArray();
		}

		internal static Utf8FormatSegment[] Utf8Parse(string format, out byte[] utf8buffer)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			List<Utf8FormatSegment> list = new List<Utf8FormatSegment>();
			utf8buffer = new byte[Encoding.UTF8.GetMaxByteCount(format.Length)];
			int num = 0;
			int num2 = 0;
			int length = format.Length;
			int num3 = 0;
			for (;;)
			{
				if (num2 < length)
				{
					ParserScanResult parserScanResult = FormatParser.ScanFormatString(format, ref num2);
					if (ParserScanResult.NormalChar == parserScanResult && num2 < length)
					{
						continue;
					}
					int num4 = num2 - num3;
					if (ParserScanResult.EscapedChar == parserScanResult)
					{
						num4--;
					}
					if (num4 != 0)
					{
						int bytes = Encoding.UTF8.GetBytes(format, num3, num4, utf8buffer, num);
						list.Add(new Utf8FormatSegment(num, bytes, -1, default(StandardFormat), 0));
						num += bytes;
					}
					num3 = num2;
					if (parserScanResult != ParserScanResult.BraceOpen)
					{
						continue;
					}
				}
				if (num2 >= length)
				{
					break;
				}
				FormatParser.ParseResult parseResult = FormatParser.Parse(format, num2);
				num3 = parseResult.LastIndex;
				num2 = parseResult.LastIndex;
				list.Add(new Utf8FormatSegment(0, 0, parseResult.Index, StandardFormat.Parse(parseResult.FormatString), parseResult.Alignment));
			}
			return list.ToArray();
		}
	}
}
