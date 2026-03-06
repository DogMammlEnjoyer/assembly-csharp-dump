using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class Utf16PreparedFormat<[Nullable(2)] T1, [Nullable(2)] T2>
	{
		public string FormatString { get; }

		public int MinSize { get; }

		public Utf16PreparedFormat(string format)
		{
			this.FormatString = format;
			this.segments = PreparedFormatHelper.Utf16Parse(format);
			int num = 0;
			foreach (Utf16FormatSegment utf16FormatSegment in this.segments)
			{
				if (!utf16FormatSegment.IsFormatArgument)
				{
					num += utf16FormatSegment.Count;
				}
			}
			this.MinSize = num;
		}

		public string Format(T1 arg1, T2 arg2)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true);
			string result;
			try
			{
				this.FormatTo<Utf16ValueStringBuilder>(ref utf16ValueStringBuilder, arg1, arg2);
				result = utf16ValueStringBuilder.ToString();
			}
			finally
			{
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		public void FormatTo<[Nullable(0)] TBufferWriter>(ref TBufferWriter sb, T1 arg1, T2 arg2) where TBufferWriter : IBufferWriter<char>
		{
			ReadOnlySpan<char> readOnlySpan = this.FormatString.AsSpan();
			foreach (Utf16FormatSegment utf16FormatSegment in this.segments)
			{
				switch (utf16FormatSegment.FormatIndex)
				{
				case -1:
				{
					ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count);
					Span<char> span = sb.GetSpan(utf16FormatSegment.Count);
					readOnlySpan2.TryCopyTo(span);
					sb.Advance(utf16FormatSegment.Count);
					break;
				}
				case 0:
					Utf16FormatHelper.FormatTo<TBufferWriter, T1>(ref sb, arg1, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg1");
					break;
				case 1:
					Utf16FormatHelper.FormatTo<TBufferWriter, T2>(ref sb, arg2, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg2");
					break;
				}
			}
		}

		private readonly Utf16FormatSegment[] segments;
	}
}
