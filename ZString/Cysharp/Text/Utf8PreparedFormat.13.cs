using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class Utf8PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
	{
		[Nullable(1)]
		public string FormatString { [NullableContext(1)] get; }

		public int MinSize { get; }

		[NullableContext(1)]
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

		[NullableContext(1)]
		public string Format(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			Utf8ValueStringBuilder utf8ValueStringBuilder = new Utf8ValueStringBuilder(true);
			string result;
			try
			{
				this.FormatTo<Utf8ValueStringBuilder>(ref utf8ValueStringBuilder, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
				result = utf8ValueStringBuilder.ToString();
			}
			finally
			{
				utf8ValueStringBuilder.Dispose();
			}
			return result;
		}

		[NullableContext(1)]
		public void FormatTo<[Nullable(0)] TBufferWriter>(ref TBufferWriter sb, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) where TBufferWriter : IBufferWriter<byte>
		{
			Span<byte> span = this.utf8PreEncodedbuffer.AsSpan<byte>();
			foreach (Utf8FormatSegment utf8FormatSegment in this.segments)
			{
				switch (utf8FormatSegment.FormatIndex)
				{
				case -1:
				{
					Span<byte> span2 = span.Slice(utf8FormatSegment.Offset, utf8FormatSegment.Count);
					Span<byte> span3 = sb.GetSpan(utf8FormatSegment.Count);
					span2.TryCopyTo(span3);
					sb.Advance(utf8FormatSegment.Count);
					break;
				}
				case 0:
					Utf8FormatHelper.FormatTo<TBufferWriter, T1>(ref sb, arg1, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg1");
					break;
				case 1:
					Utf8FormatHelper.FormatTo<TBufferWriter, T2>(ref sb, arg2, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg2");
					break;
				case 2:
					Utf8FormatHelper.FormatTo<TBufferWriter, T3>(ref sb, arg3, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg3");
					break;
				case 3:
					Utf8FormatHelper.FormatTo<TBufferWriter, T4>(ref sb, arg4, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg4");
					break;
				case 4:
					Utf8FormatHelper.FormatTo<TBufferWriter, T5>(ref sb, arg5, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg5");
					break;
				case 5:
					Utf8FormatHelper.FormatTo<TBufferWriter, T6>(ref sb, arg6, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg6");
					break;
				case 6:
					Utf8FormatHelper.FormatTo<TBufferWriter, T7>(ref sb, arg7, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg7");
					break;
				case 7:
					Utf8FormatHelper.FormatTo<TBufferWriter, T8>(ref sb, arg8, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg8");
					break;
				case 8:
					Utf8FormatHelper.FormatTo<TBufferWriter, T9>(ref sb, arg9, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg9");
					break;
				case 9:
					Utf8FormatHelper.FormatTo<TBufferWriter, T10>(ref sb, arg10, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg10");
					break;
				case 10:
					Utf8FormatHelper.FormatTo<TBufferWriter, T11>(ref sb, arg11, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg11");
					break;
				case 11:
					Utf8FormatHelper.FormatTo<TBufferWriter, T12>(ref sb, arg12, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg12");
					break;
				case 12:
					Utf8FormatHelper.FormatTo<TBufferWriter, T13>(ref sb, arg13, utf8FormatSegment.Alignment, utf8FormatSegment.StandardFormat, "arg13");
					break;
				}
			}
		}

		[Nullable(1)]
		private readonly Utf8FormatSegment[] segments;

		[Nullable(1)]
		private readonly byte[] utf8PreEncodedbuffer;
	}
}
