using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class Utf8PreparedFormat<[Nullable(2)] T1>
	{
		public string FormatString { get; }

		public int MinSize { get; }

		public Utf8PreparedFormat(string format)
		{
			this.FormatString = format;
			this.segments = PreparedFormatHelper.Utf8Parse(format, out this.utf8PreEncodedbuffer);
			int num = 0;
			foreach (Utf8FormatSegment utf8FormatSegment in this.segments)
			{
				if (!utf8FormatSegment.IsFormatArgument)
				{
					num += utf8FormatSegment.Count;
				}
			}
			this.MinSize = num;
		}

		public string Format(T1 arg1)
		{
			Utf8ValueStringBuilder utf8ValueStringBuilder = new Utf8ValueStringBuilder(true);
			string result;
			try
			{
				this.FormatTo<Utf8ValueStringBuilder>(ref utf8ValueStringBuilder, arg1);
				result = utf8ValueStringBuilder.ToString();
			}
			finally
			{
				utf8ValueStringBuilder.Dispose();
			}
			return result;
		}

		public void FormatTo<[Nullable(0)] TBufferWriter>(ref TBufferWriter sb, T1 arg1) where TBufferWriter : IBufferWriter<byte>
		{
			Span<byte> span = this.utf8PreEncodedbuffer.AsSpan<byte>();
			foreach (Utf8FormatSegment utf8FormatSegment in this.segments)
			{
				int formatIndex = utf8FormatSegment.FormatIndex;
				if (formatIndex != -1)
				{
					if (formatIndex == 0)
					{
						Utf8FormatHelper.FormatTo<TBufferWriter, T1>(ref sb, arg1, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg1");
					}
				}
				else
				{
					Span<byte> span2 = span.Slice(utf8FormatSegment.Offset, utf8FormatSegment.Count);
					Span<byte> span3 = sb.GetSpan(utf8FormatSegment.Count);
					span2.TryCopyTo(span3);
					sb.Advance(utf8FormatSegment.Count);
				}
			}
		}

		private readonly Utf8FormatSegment[] segments;

		private readonly byte[] utf8PreEncodedbuffer;
	}
}
