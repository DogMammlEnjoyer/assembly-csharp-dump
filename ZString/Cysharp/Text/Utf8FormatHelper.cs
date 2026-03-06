using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class Utf8FormatHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FormatTo<[Nullable(0)] TBufferWriter, [Nullable(2)] T>(ref TBufferWriter sb, T arg, int width, StandardFormat format, string argName) where TBufferWriter : IBufferWriter<byte>
		{
			if (width <= 0)
			{
				Span<byte> span = sb.GetSpan(0);
				int num;
				if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, span, out num, format))
				{
					sb.Advance(0);
					span = sb.GetSpan(Math.Max(span.Length + 1, num));
					if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, span, out num, format))
					{
						ExceptionUtil.ThrowArgumentException(argName);
					}
				}
				sb.Advance(num);
				width *= -1;
				int num2 = width - num;
				if (width > 0 && num2 > 0)
				{
					sb.GetSpan(num2).Fill(32);
					sb.Advance(num2);
					return;
				}
			}
			else
			{
				Utf8FormatHelper.FormatToRightJustify<TBufferWriter, T>(ref sb, arg, width, format, argName);
			}
		}

		private unsafe static void FormatToRightJustify<[Nullable(0)] TBufferWriter, [Nullable(2)] T>(ref TBufferWriter sb, T arg, int width, StandardFormat format, string argName) where TBufferWriter : IBufferWriter<byte>
		{
			if (typeof(T) == typeof(string))
			{
				string text = Unsafe.As<string>(arg);
				int num = width - text.Length;
				if (num > 0)
				{
					sb.GetSpan(num).Fill(32);
					sb.Advance(num);
				}
				ZString.AppendChars<TBufferWriter>(ref sb, text.AsSpan());
				return;
			}
			int num2 = typeof(T).IsValueType ? (Unsafe.SizeOf<T>() * 8) : 1024;
			Span<byte> destination = new Span<byte>(stackalloc byte[(UIntPtr)num2], num2);
			int num3;
			if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, destination, out num3, format))
			{
				num2 = destination.Length * 2;
				destination = new Span<byte>(stackalloc byte[(UIntPtr)num2], num2);
				if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, destination, out num3, format))
				{
					ExceptionUtil.ThrowArgumentException(argName);
				}
			}
			int num4 = width - num3;
			if (num4 > 0)
			{
				sb.GetSpan(num4).Fill(32);
				sb.Advance(num4);
			}
			Span<byte> span = sb.GetSpan(num3);
			destination.CopyTo(span);
			sb.Advance(num3);
		}

		private const byte sp = 32;
	}
}
