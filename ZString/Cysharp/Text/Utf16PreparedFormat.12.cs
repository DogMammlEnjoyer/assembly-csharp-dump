using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class Utf16PreparedFormat<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
	{
		[Nullable(1)]
		public string FormatString { [NullableContext(1)] get; }

		public int MinSize { get; }

		[NullableContext(1)]
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

		[NullableContext(1)]
		public string Format(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = new Utf16ValueStringBuilder(true);
			string result;
			try
			{
				this.FormatTo<Utf16ValueStringBuilder>(ref utf16ValueStringBuilder, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
				result = utf16ValueStringBuilder.ToString();
			}
			finally
			{
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		[NullableContext(1)]
		public void FormatTo<[Nullable(0)] TBufferWriter>(ref TBufferWriter sb, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) where TBufferWriter : IBufferWriter<char>
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
				case 2:
					Utf16FormatHelper.FormatTo<TBufferWriter, T3>(ref sb, arg3, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg3");
					break;
				case 3:
					Utf16FormatHelper.FormatTo<TBufferWriter, T4>(ref sb, arg4, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg4");
					break;
				case 4:
					Utf16FormatHelper.FormatTo<TBufferWriter, T5>(ref sb, arg5, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg5");
					break;
				case 5:
					Utf16FormatHelper.FormatTo<TBufferWriter, T6>(ref sb, arg6, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg6");
					break;
				case 6:
					Utf16FormatHelper.FormatTo<TBufferWriter, T7>(ref sb, arg7, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg7");
					break;
				case 7:
					Utf16FormatHelper.FormatTo<TBufferWriter, T8>(ref sb, arg8, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg8");
					break;
				case 8:
					Utf16FormatHelper.FormatTo<TBufferWriter, T9>(ref sb, arg9, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg9");
					break;
				case 9:
					Utf16FormatHelper.FormatTo<TBufferWriter, T10>(ref sb, arg10, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg10");
					break;
				case 10:
					Utf16FormatHelper.FormatTo<TBufferWriter, T11>(ref sb, arg11, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg11");
					break;
				case 11:
					Utf16FormatHelper.FormatTo<TBufferWriter, T12>(ref sb, arg12, utf16FormatSegment.Alignment, readOnlySpan.Slice(utf16FormatSegment.Offset, utf16FormatSegment.Count), "arg12");
					break;
				}
			}
		}

		[Nullable(1)]
		private readonly Utf16FormatSegment[] segments;
	}
}
