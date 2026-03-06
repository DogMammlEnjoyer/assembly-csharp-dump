using System;

namespace Cysharp.Text
{
	internal readonly struct Utf16FormatSegment
	{
		public bool IsFormatArgument
		{
			get
			{
				return this.FormatIndex != -1;
			}
		}

		public Utf16FormatSegment(int offset, int count, int formatIndex, int alignment)
		{
			this.Offset = offset;
			this.Count = count;
			this.FormatIndex = formatIndex;
			this.Alignment = alignment;
		}

		public const int NotFormatIndex = -1;

		public readonly int Offset;

		public readonly int Count;

		public readonly int FormatIndex;

		public readonly int Alignment;
	}
}
