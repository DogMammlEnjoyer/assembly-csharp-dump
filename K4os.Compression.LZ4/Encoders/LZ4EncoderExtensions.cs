using System;

namespace K4os.Compression.LZ4.Encoders
{
	public static class LZ4EncoderExtensions
	{
		public unsafe static bool Topup(this ILZ4Encoder encoder, ref byte* source, int length)
		{
			int num = encoder.Topup(source, length);
			source += (IntPtr)num;
			return num != 0;
		}

		public unsafe static int Topup(this ILZ4Encoder encoder, byte[] source, int offset, int length)
		{
			byte* ptr;
			if (source == null || source.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &source[0];
			}
			return encoder.Topup(ptr + offset, length);
		}

		public static bool Topup(this ILZ4Encoder encoder, byte[] source, ref int offset, int length)
		{
			int num = encoder.Topup(source, offset, length);
			offset += num;
			return num != 0;
		}

		public unsafe static int Encode(this ILZ4Encoder encoder, byte[] target, int offset, int length, bool allowCopy)
		{
			byte* ptr;
			if (target == null || target.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &target[0];
			}
			return encoder.Encode(ptr + offset, length, allowCopy);
		}

		public static EncoderAction Encode(this ILZ4Encoder encoder, byte[] target, ref int offset, int length, bool allowCopy)
		{
			int num = encoder.Encode(target, offset, length, allowCopy);
			offset += Math.Abs(num);
			if (num == 0)
			{
				return EncoderAction.None;
			}
			if (num >= 0)
			{
				return EncoderAction.Encoded;
			}
			return EncoderAction.Copied;
		}

		public unsafe static EncoderAction Encode(this ILZ4Encoder encoder, ref byte* target, int length, bool allowCopy)
		{
			int num = encoder.Encode(target, length, allowCopy);
			target += (IntPtr)Math.Abs(num);
			if (num == 0)
			{
				return EncoderAction.None;
			}
			if (num >= 0)
			{
				return EncoderAction.Encoded;
			}
			return EncoderAction.Copied;
		}

		public unsafe static EncoderAction TopupAndEncode(this ILZ4Encoder encoder, byte* source, int sourceLength, byte* target, int targetLength, bool forceEncode, bool allowCopy, out int loaded, out int encoded)
		{
			loaded = 0;
			encoded = 0;
			if (sourceLength > 0)
			{
				loaded = encoder.Topup(source, sourceLength);
			}
			return encoder.FlushAndEncode(target, targetLength, forceEncode, allowCopy, loaded, out encoded);
		}

		public unsafe static EncoderAction TopupAndEncode(this ILZ4Encoder encoder, byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength, bool forceEncode, bool allowCopy, out int loaded, out int encoded)
		{
			byte* ptr;
			if (source == null || source.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &source[0];
			}
			byte* ptr2;
			if (target == null || target.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &target[0];
			}
			return encoder.TopupAndEncode(ptr + sourceOffset, sourceLength, ptr2 + targetOffset, targetLength, forceEncode, allowCopy, out loaded, out encoded);
		}

		public unsafe static EncoderAction TopupAndEncode(this ILZ4Encoder encoder, ReadOnlySpan<byte> source, Span<byte> target, bool forceEncode, bool allowCopy, out int loaded, out int encoded)
		{
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* source2 = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* target2 = pinnableReference2;
					return encoder.TopupAndEncode(source2, source.Length, target2, target.Length, forceEncode, allowCopy, out loaded, out encoded);
				}
			}
		}

		private unsafe static EncoderAction FlushAndEncode(this ILZ4Encoder encoder, byte* target, int targetLength, bool forceEncode, bool allowCopy, int loaded, out int encoded)
		{
			encoded = 0;
			int blockSize = encoder.BlockSize;
			if (encoder.BytesReady < (forceEncode ? 1 : blockSize))
			{
				if (loaded <= 0)
				{
					return EncoderAction.None;
				}
				return EncoderAction.Loaded;
			}
			else
			{
				encoded = encoder.Encode(target, targetLength, allowCopy);
				if (!allowCopy || encoded >= 0)
				{
					return EncoderAction.Encoded;
				}
				encoded = -encoded;
				return EncoderAction.Copied;
			}
		}

		public unsafe static EncoderAction FlushAndEncode(this ILZ4Encoder encoder, byte* target, int targetLength, bool allowCopy, out int encoded)
		{
			return encoder.FlushAndEncode(target, targetLength, true, allowCopy, 0, out encoded);
		}

		public unsafe static EncoderAction FlushAndEncode(this ILZ4Encoder encoder, byte[] target, int targetOffset, int targetLength, bool allowCopy, out int encoded)
		{
			byte* ptr;
			if (target == null || target.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &target[0];
			}
			return encoder.FlushAndEncode(ptr + targetOffset, targetLength, true, allowCopy, 0, out encoded);
		}

		public unsafe static EncoderAction FlushAndEncode(this ILZ4Encoder encoder, Span<byte> target, bool allowCopy, out int encoded)
		{
			fixed (byte* pinnableReference = target.GetPinnableReference())
			{
				byte* target2 = pinnableReference;
				return encoder.FlushAndEncode(target2, target.Length, true, allowCopy, 0, out encoded);
			}
		}

		public unsafe static void Drain(this ILZ4Decoder decoder, byte[] target, int targetOffset, int offset, int length)
		{
			fixed (byte[] array = target)
			{
				byte* ptr;
				if (target == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				decoder.Drain(ptr + targetOffset, offset, length);
			}
		}

		public unsafe static void Drain(this ILZ4Decoder decoder, Span<byte> target, int offset, int length)
		{
			fixed (byte* pinnableReference = target.GetPinnableReference())
			{
				byte* target2 = pinnableReference;
				decoder.Drain(target2, offset, length);
			}
		}

		public unsafe static bool DecodeAndDrain(this ILZ4Decoder decoder, byte* source, int sourceLength, byte* target, int targetLength, out int decoded)
		{
			decoded = 0;
			if (sourceLength <= 0)
			{
				return false;
			}
			decoded = decoder.Decode(source, sourceLength, 0);
			if (decoded <= 0 || targetLength < decoded)
			{
				return false;
			}
			decoder.Drain(target, -decoded, decoded);
			return true;
		}

		public unsafe static bool DecodeAndDrain(this ILZ4Decoder decoder, byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength, out int decoded)
		{
			byte* ptr;
			if (source == null || source.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &source[0];
			}
			byte* ptr2;
			if (target == null || target.Length == 0)
			{
				ptr2 = null;
			}
			else
			{
				ptr2 = &target[0];
			}
			return decoder.DecodeAndDrain(ptr + sourceOffset, sourceLength, ptr2 + targetOffset, targetLength, out decoded);
		}

		public unsafe static bool DecodeAndDrain(this ILZ4Decoder decoder, ReadOnlySpan<byte> source, Span<byte> target, out int decoded)
		{
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* source2 = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* target2 = pinnableReference2;
					return decoder.DecodeAndDrain(source2, source.Length, target2, target.Length, out decoded);
				}
			}
		}

		public unsafe static int Inject(this ILZ4Decoder decoder, byte[] buffer, int offset, int length)
		{
			byte* ptr;
			if (buffer == null || buffer.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &buffer[0];
			}
			return decoder.Inject(ptr + offset, length);
		}

		public unsafe static int Decode(this ILZ4Decoder decoder, byte[] buffer, int offset, int length, int blockSize = 0)
		{
			byte* ptr;
			if (buffer == null || buffer.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &buffer[0];
			}
			return decoder.Decode(ptr + offset, length, blockSize);
		}
	}
}
