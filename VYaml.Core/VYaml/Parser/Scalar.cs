using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using VYaml.Internal;

namespace VYaml.Parser
{
	internal class Scalar : ITokenContent
	{
		public int Length { get; private set; }

		public Scalar(int capacity)
		{
			this.buffer = new byte[capacity];
		}

		public Scalar(ReadOnlySpan<byte> content)
		{
			this.buffer = new byte[content.Length];
			this.Write(content);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<byte> AsSpan()
		{
			return this.buffer.AsSpan(0, this.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<byte> AsSpan(int start, int length)
		{
			return this.buffer.AsSpan(start, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<byte> AsUtf8()
		{
			return this.buffer.AsSpan(0, this.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(byte code)
		{
			if (this.Length == this.buffer.Length)
			{
				this.Grow();
			}
			byte[] array = this.buffer;
			int length = this.Length;
			this.Length = length + 1;
			array[length] = code;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(LineBreakState lineBreak)
		{
			switch (lineBreak)
			{
			case LineBreakState.None:
				return;
			case LineBreakState.Lf:
				this.Write(10);
				return;
			case LineBreakState.CrLf:
				this.Write(13);
				this.Write(10);
				return;
			case LineBreakState.Cr:
				this.Write(13);
				return;
			default:
				throw new ArgumentOutOfRangeException("lineBreak", lineBreak, null);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(ReadOnlySpan<byte> codes)
		{
			this.Grow(this.Length + codes.Length);
			codes.CopyTo(this.buffer.AsSpan(this.Length, codes.Length));
			this.Length += codes.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteUnicodeCodepoint(int codepoint)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)((ushort)codepoint);
			Span<char> span = new Span<char>(intPtr, 1);
			int byteCount = StringEncoding.Utf8.GetByteCount(span);
			Span<byte> span2 = new Span<byte>(stackalloc byte[(UIntPtr)byteCount], byteCount);
			StringEncoding.Utf8.GetBytes(span, span2);
			this.Write(span2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			this.Length = 0;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return StringEncoding.Utf8.GetString(this.AsSpan());
		}

		public unsafe bool IsNull()
		{
			Span<byte> span = this.AsSpan();
			switch (span.Length)
			{
			case 0:
				break;
			case 1:
				if (*span[0] != 126)
				{
					return false;
				}
				break;
			case 2:
			case 3:
				return false;
			case 4:
				if (!span.SequenceEqual(YamlCodes.Null0) && !span.SequenceEqual(YamlCodes.Null1) && !span.SequenceEqual(YamlCodes.Null2))
				{
					return false;
				}
				break;
			default:
				return false;
			}
			return true;
		}

		public bool TryGetBool(out bool value)
		{
			Span<byte> span = this.AsSpan();
			int length = span.Length;
			if (length != 4)
			{
				if (length == 5)
				{
					if (span.SequenceEqual(YamlCodes.False0) || span.SequenceEqual(YamlCodes.False1) || span.SequenceEqual(YamlCodes.False2))
					{
						value = false;
						return true;
					}
				}
			}
			else if (span.SequenceEqual(YamlCodes.True0) || span.SequenceEqual(YamlCodes.True1) || span.SequenceEqual(YamlCodes.True2))
			{
				value = true;
				return true;
			}
			value = false;
			return false;
		}

		public bool TryGetInt32(out int value)
		{
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			ReadOnlySpan<byte> source;
			if (Scalar.TryDetectHex(span, out source))
			{
				return Utf8Parser.TryParse(source, out value, out num, 'x') && num == source.Length;
			}
			if (Scalar.TryDetectHexNegative(span, out source) && Utf8Parser.TryParse(source, out value, out num, 'x') && num == source.Length)
			{
				value *= -1;
				return true;
			}
			ulong num2;
			if (Scalar.TryParseOctal(span, out num2) && num2 <= 2147483647UL)
			{
				value = (int)num2;
				return true;
			}
			return false;
		}

		public bool TryGetInt64(out long value)
		{
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			if (span.Length > YamlCodes.HexPrefix.Length && span.StartsWith(YamlCodes.HexPrefix))
			{
				Span<byte> span2 = span;
				int num2 = YamlCodes.HexPrefix.Length;
				Span<byte> span3 = span2.Slice(num2, span2.Length - num2);
				int num3;
				return Utf8Parser.TryParse(span3, out value, out num3, 'x') && num3 == span3.Length;
			}
			if (span.Length > YamlCodes.HexPrefixNegative.Length && span.StartsWith(YamlCodes.HexPrefixNegative))
			{
				Span<byte> span2 = span;
				int num2 = YamlCodes.HexPrefixNegative.Length;
				Span<byte> span4 = span2.Slice(num2, span2.Length - num2);
				int num4;
				if (Utf8Parser.TryParse(span4, out value, out num4, 'x') && num4 == span4.Length)
				{
					value = -value;
					return true;
				}
			}
			ulong num5;
			if (Scalar.TryParseOctal(span, out num5) && num5 <= 9223372036854775807UL)
			{
				value = (long)num5;
				return true;
			}
			return false;
		}

		public bool TryGetUInt32(out uint value)
		{
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			ReadOnlySpan<byte> source;
			if (Scalar.TryDetectHex(span, out source))
			{
				return Utf8Parser.TryParse(source, out value, out num, 'x') && num == source.Length;
			}
			ulong num2;
			if (Scalar.TryParseOctal(span, out num2) && num2 <= (ulong)-1)
			{
				value = (uint)num2;
				return true;
			}
			return false;
		}

		public bool TryGetUInt64(out ulong value)
		{
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			ReadOnlySpan<byte> source;
			if (Scalar.TryDetectHex(span, out source))
			{
				return Utf8Parser.TryParse(source, out value, out num, 'x') && num == source.Length;
			}
			return Scalar.TryParseOctal(span, out value);
		}

		public bool TryGetFloat(out float value)
		{
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			int length = span.Length;
			if (length != 4)
			{
				if (length == 5)
				{
					if (span.SequenceEqual(YamlCodes.Inf3) || span.SequenceEqual(YamlCodes.Inf4) || span.SequenceEqual(YamlCodes.Inf5))
					{
						value = float.PositiveInfinity;
						return true;
					}
					if (span.SequenceEqual(YamlCodes.NegInf0) || span.SequenceEqual(YamlCodes.NegInf1) || span.SequenceEqual(YamlCodes.NegInf2))
					{
						value = float.NegativeInfinity;
						return true;
					}
				}
			}
			else
			{
				if (span.SequenceEqual(YamlCodes.Inf0) || span.SequenceEqual(YamlCodes.Inf1) || span.SequenceEqual(YamlCodes.Inf2))
				{
					value = float.PositiveInfinity;
					return true;
				}
				if (span.SequenceEqual(YamlCodes.Nan0) || span.SequenceEqual(YamlCodes.Nan1) || span.SequenceEqual(YamlCodes.Nan2))
				{
					value = float.NaN;
					return true;
				}
			}
			return false;
		}

		public bool TryGetDouble(out double value)
		{
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			int length = span.Length;
			if (length != 4)
			{
				if (length == 5)
				{
					if (span.SequenceEqual(YamlCodes.Inf3) || span.SequenceEqual(YamlCodes.Inf4) || span.SequenceEqual(YamlCodes.Inf5))
					{
						value = double.PositiveInfinity;
						return true;
					}
					if (span.SequenceEqual(YamlCodes.NegInf0) || span.SequenceEqual(YamlCodes.NegInf1) || span.SequenceEqual(YamlCodes.NegInf2))
					{
						value = double.NegativeInfinity;
						return true;
					}
				}
			}
			else
			{
				if (span.SequenceEqual(YamlCodes.Inf0) || span.SequenceEqual(YamlCodes.Inf1) || span.SequenceEqual(YamlCodes.Inf2))
				{
					value = double.PositiveInfinity;
					return true;
				}
				if (span.SequenceEqual(YamlCodes.Nan0) || span.SequenceEqual(YamlCodes.Nan1) || span.SequenceEqual(YamlCodes.Nan2))
				{
					value = double.NaN;
					return true;
				}
			}
			return false;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SequenceEqual(Scalar other)
		{
			return this.AsSpan().SequenceEqual(other.AsSpan());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SequenceEqual(ReadOnlySpan<byte> span)
		{
			return this.AsSpan().SequenceEqual(span);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Grow(int sizeHint)
		{
			if (sizeHint <= this.buffer.Length)
			{
				return;
			}
			int i;
			for (i = this.buffer.Length * 200 / 100; i < sizeHint; i = i * 200 / 100)
			{
			}
			this.SetCapacity(i);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryDetectHex(ReadOnlySpan<byte> span, out ReadOnlySpan<byte> slice)
		{
			if (span.Length > YamlCodes.HexPrefix.Length && span.StartsWith(YamlCodes.HexPrefix))
			{
				ReadOnlySpan<byte> readOnlySpan = span;
				int num = YamlCodes.HexPrefix.Length;
				slice = readOnlySpan.Slice(num, readOnlySpan.Length - num);
				return true;
			}
			slice = default(ReadOnlySpan<byte>);
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryDetectHexNegative(ReadOnlySpan<byte> span, out ReadOnlySpan<byte> slice)
		{
			if (span.Length > YamlCodes.HexPrefixNegative.Length && span.StartsWith(YamlCodes.HexPrefixNegative))
			{
				ReadOnlySpan<byte> readOnlySpan = span;
				int num = YamlCodes.HexPrefixNegative.Length;
				slice = readOnlySpan.Slice(num, readOnlySpan.Length - num);
				return true;
			}
			slice = default(ReadOnlySpan<byte>);
			return false;
		}

		private unsafe static bool TryParseOctal(ReadOnlySpan<byte> span, out ulong value)
		{
			if (span.Length <= YamlCodes.OctalPrefix.Length || !span.StartsWith(YamlCodes.OctalPrefix))
			{
				value = 0UL;
				return false;
			}
			int num = YamlCodes.OctalPrefix.Length;
			while (num < span.Length && *span[num] == 48)
			{
				num++;
			}
			if (num >= span.Length)
			{
				value = 0UL;
				return num == span.Length;
			}
			ReadOnlySpan<byte> readOnlySpan = span;
			int num2 = num;
			ReadOnlySpan<byte> readOnlySpan2 = readOnlySpan.Slice(num2, readOnlySpan.Length - num2);
			int num3 = (int)(*readOnlySpan2[0] - 48);
			if (num3 < 0 || num3 > 7 || (num3 > 1 && readOnlySpan2.Length == 22) || readOnlySpan2.Length > 22)
			{
				value = 0UL;
				return false;
			}
			value = (ulong)((long)num3);
			for (int i = 1; i < readOnlySpan2.Length; i++)
			{
				num3 = (int)(*readOnlySpan2[i] - 48);
				if (num3 < 0 || num3 > 7)
				{
					value = 0UL;
					return false;
				}
				value = (value << 3) + (ulong)num3;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Grow()
		{
			int num = this.buffer.Length * 200 / 100;
			if (num < this.buffer.Length + 4)
			{
				num = this.buffer.Length + 4;
			}
			this.SetCapacity(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetCapacity(int newCapacity)
		{
			if (this.buffer.Length >= newCapacity)
			{
				return;
			}
			byte[] destinationArray = ArrayPool<byte>.Shared.Rent(newCapacity);
			Array.Copy(this.buffer, 0, destinationArray, 0, this.Length);
			ArrayPool<byte>.Shared.Return(this.buffer, false);
			this.buffer = destinationArray;
		}

		private const int MinimumGrow = 4;

		private const int GrowFactor = 200;

		[Nullable(1)]
		public static readonly Scalar Null = new Scalar(0);

		[Nullable(1)]
		private byte[] buffer;
	}
}
