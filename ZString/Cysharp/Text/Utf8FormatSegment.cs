using System;
using System.Buffers;

namespace Cysharp.Text
{
	internal readonly struct Utf8FormatSegment
	{
		public bool IsFormatArgument
		{
			get
			{
				return this.FormatIndex != -1;
			}
		}

		public Utf8FormatSegment(int offset, int count, int formatIndex, StandardFormat format, int alignment)
		{
			this.Offset = offset;
			this.Count = count;
			this.FormatIndex = formatIndex;
			this.StandardFormat = format;
			this.Alignment = alignment;
		}

		public const int NotFormatIndex = -1;

		public readonly int Offset;

		public readonly int Count;

		public readonly int FormatIndex;

		public readonly StandardFormat StandardFormat;

		public readonly int Alignment;
	}
}
