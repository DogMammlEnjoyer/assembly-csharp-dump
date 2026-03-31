using System;
using System.Runtime.CompilerServices;
using K4os.Compression.LZ4.Engine;

namespace K4os.Compression.LZ4
{
	public static class LZ4Codec
	{
		public static bool Enforce32
		{
			get
			{
				return LL.Enforce32;
			}
			set
			{
				LL.Enforce32 = value;
			}
		}

		public static int MaximumOutputSize(int length)
		{
			return LL.LZ4_compressBound(length);
		}

		public unsafe static int Encode(byte* source, int sourceLength, byte* target, int targetLength, LZ4Level level = LZ4Level.L00_FAST)
		{
			if (sourceLength <= 0)
			{
				return 0;
			}
			int num = (level < LZ4Level.L03_HC) ? LLxx.LZ4_compress_fast(source, target, sourceLength, targetLength, 1) : LLxx.LZ4_compress_HC(source, target, sourceLength, targetLength, (int)level);
			if (num > 0)
			{
				return num;
			}
			return -1;
		}

		public unsafe static int Encode(ReadOnlySpan<byte> source, Span<byte> target, LZ4Level level = LZ4Level.L00_FAST)
		{
			int length = source.Length;
			if (length <= 0)
			{
				return 0;
			}
			int length2 = target.Length;
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* source2 = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* target2 = pinnableReference2;
					return LZ4Codec.Encode(source2, length, target2, length2, level);
				}
			}
		}

		[NullableContext(1)]
		public unsafe static int Encode(byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength, LZ4Level level = LZ4Level.L00_FAST)
		{
			source.Validate(sourceOffset, sourceLength, false);
			target.Validate(targetOffset, targetLength, false);
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
			return LZ4Codec.Encode(ptr + sourceOffset, sourceLength, ptr2 + targetOffset, targetLength, level);
		}

		public unsafe static int Decode(byte* source, int sourceLength, byte* target, int targetLength)
		{
			if (sourceLength <= 0)
			{
				return 0;
			}
			int num = LLxx.LZ4_decompress_safe(source, target, sourceLength, targetLength);
			if (num > 0)
			{
				return num;
			}
			return -1;
		}

		public unsafe static int PartialDecode(byte* source, int sourceLength, byte* target, int targetLength)
		{
			if (sourceLength <= 0)
			{
				return 0;
			}
			int num = LLxx.LZ4_decompress_safe_partial(source, target, sourceLength, targetLength);
			if (num > 0)
			{
				return num;
			}
			return -1;
		}

		public unsafe static int Decode(byte* source, int sourceLength, byte* target, int targetLength, byte* dictionary, int dictionaryLength)
		{
			if (sourceLength <= 0)
			{
				return 0;
			}
			int num = LLxx.LZ4_decompress_safe_usingDict(source, target, sourceLength, targetLength, dictionary, dictionaryLength);
			if (num > 0)
			{
				return num;
			}
			return -1;
		}

		public unsafe static int PartialDecode(ReadOnlySpan<byte> source, Span<byte> target)
		{
			int length = source.Length;
			if (length <= 0)
			{
				return 0;
			}
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* source2 = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* target2 = pinnableReference2;
					return LZ4Codec.PartialDecode(source2, length, target2, target.Length);
				}
			}
		}

		public unsafe static int Decode(ReadOnlySpan<byte> source, Span<byte> target)
		{
			int length = source.Length;
			if (length <= 0)
			{
				return 0;
			}
			int length2 = target.Length;
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* source2 = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* target2 = pinnableReference2;
					return LZ4Codec.Decode(source2, length, target2, length2);
				}
			}
		}

		public unsafe static int Decode(ReadOnlySpan<byte> source, Span<byte> target, ReadOnlySpan<byte> dictionary)
		{
			int length = source.Length;
			if (length <= 0)
			{
				return 0;
			}
			int length2 = target.Length;
			int length3 = dictionary.Length;
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* source2 = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* target2 = pinnableReference2;
					fixed (byte* pinnableReference3 = dictionary.GetPinnableReference())
					{
						byte* dictionary2 = pinnableReference3;
						return LZ4Codec.Decode(source2, length, target2, length2, dictionary2, length3);
					}
				}
			}
		}

		[NullableContext(1)]
		public unsafe static int Decode(byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength)
		{
			source.Validate(sourceOffset, sourceLength, false);
			target.Validate(targetOffset, targetLength, false);
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
			return LZ4Codec.Decode(ptr + sourceOffset, sourceLength, ptr2 + targetOffset, targetLength);
		}

		[NullableContext(1)]
		public unsafe static int Decode(byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength, [Nullable(2)] byte[] dictionary, int dictionaryOffset, int dictionaryLength)
		{
			source.Validate(sourceOffset, sourceLength, false);
			target.Validate(targetOffset, targetLength, false);
			dictionary.Validate(dictionaryOffset, dictionaryLength, true);
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
			byte* ptr3;
			if (dictionary == null || dictionary.Length == 0)
			{
				ptr3 = null;
			}
			else
			{
				ptr3 = &dictionary[0];
			}
			return LZ4Codec.Decode(ptr + sourceOffset, sourceLength, ptr2 + targetOffset, targetLength, ptr3 + dictionaryOffset, dictionaryLength);
		}

		public const int Version = 192;
	}
}
