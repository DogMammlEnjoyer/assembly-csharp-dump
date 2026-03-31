using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4
{
	public static class LZ4Pickler
	{
		[NullableContext(1)]
		public static byte[] Pickle(byte[] source, LZ4Level level = LZ4Level.L00_FAST)
		{
			return LZ4Pickler.Pickle(source.AsSpan<byte>(), level);
		}

		[NullableContext(1)]
		public static byte[] Pickle(byte[] source, int sourceIndex, int sourceLength, LZ4Level level = LZ4Level.L00_FAST)
		{
			return LZ4Pickler.Pickle(source.AsSpan(sourceIndex, sourceLength), level);
		}

		[return: Nullable(1)]
		public unsafe static byte[] Pickle(byte* source, int length, LZ4Level level = LZ4Level.L00_FAST)
		{
			return LZ4Pickler.Pickle(new Span<byte>((void*)source, length), level);
		}

		[return: Nullable(1)]
		public unsafe static byte[] Pickle(ReadOnlySpan<byte> source, LZ4Level level = LZ4Level.L00_FAST)
		{
			int length = source.Length;
			if (length == 0)
			{
				return Mem.Empty;
			}
			if (length <= 1024)
			{
				Span<byte> buffer = new Span<byte>(stackalloc byte[(UIntPtr)1024], 1024);
				return LZ4Pickler.PickleWithBuffer(source, level, buffer);
			}
			PinnedMemory pinnedMemory;
			PinnedMemory.Alloc(out pinnedMemory, length, false);
			byte[] result;
			try
			{
				result = LZ4Pickler.PickleWithBuffer(source, level, pinnedMemory.Span);
			}
			finally
			{
				pinnedMemory.Free();
			}
			return result;
		}

		[return: Nullable(1)]
		private static byte[] PickleWithBuffer(ReadOnlySpan<byte> source, LZ4Level level, Span<byte> buffer)
		{
			int length = source.Length;
			int num = LZ4Codec.Encode(source, buffer, level);
			if (num <= 0 || num >= length)
			{
				byte[] array = new byte[LZ4Pickler.GetUncompressedHeaderSize(0, length) + length];
				Span<byte> target = array.AsSpan<byte>();
				int start = LZ4Pickler.EncodeUncompressedHeader(target, 0, length);
				source.CopyTo(target.Slice(start));
				return array;
			}
			int compressedHeaderSize = LZ4Pickler.GetCompressedHeaderSize(0, length, num);
			byte[] array2 = new byte[compressedHeaderSize + num];
			Span<byte> target2 = array2.AsSpan<byte>();
			int start2 = LZ4Pickler.EncodeCompressedHeader(target2, 0, compressedHeaderSize, length, num);
			buffer.Slice(0, num).CopyTo(target2.Slice(start2));
			return array2;
		}

		public static void Pickle<TBufferWriter>(ReadOnlySpan<byte> source, [Nullable(1)] TBufferWriter writer, LZ4Level level = LZ4Level.L00_FAST) where TBufferWriter : IBufferWriter<byte>
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			int length = source.Length;
			if (length == 0)
			{
				return;
			}
			int pessimisticHeaderSize = LZ4Pickler.GetPessimisticHeaderSize(0, length);
			Span<byte> span = writer.GetSpan(pessimisticHeaderSize + length);
			int num = LZ4Codec.Encode(source, span.Slice(pessimisticHeaderSize, length), level);
			if (num <= 0 || num >= length)
			{
				int num2 = LZ4Pickler.EncodeUncompressedHeader(span, 0, length);
				source.CopyTo(span.Slice(num2));
				writer.Advance(num2 + length);
				return;
			}
			int num3 = LZ4Pickler.EncodeCompressedHeader(span, 0, pessimisticHeaderSize, length, num);
			writer.Advance(num3 + num);
		}

		public static void Pickle(ReadOnlySpan<byte> source, [Nullable(1)] IBufferWriter<byte> writer, LZ4Level level = LZ4Level.L00_FAST)
		{
			LZ4Pickler.Pickle<IBufferWriter<byte>>(source, writer, level);
		}

		private static int GetPessimisticHeaderSize(int version, int sourceLength)
		{
			if (version == 0)
			{
				return 1 + LZ4Pickler.EffectiveSizeOf(sourceLength);
			}
			throw LZ4Pickler.UnexpectedVersion(version);
		}

		private static int GetUncompressedHeaderSize(int version, int sourceLength)
		{
			if (version == 0)
			{
				return 1;
			}
			throw LZ4Pickler.UnexpectedVersion(version);
		}

		private static int GetCompressedHeaderSize(int version, int sourceLength, int encodedLength)
		{
			if (version == 0)
			{
				return 1 + LZ4Pickler.EffectiveSizeOf(sourceLength - encodedLength);
			}
			throw LZ4Pickler.UnexpectedVersion(version);
		}

		private static int EncodeUncompressedHeader(Span<byte> target, int version, int sourceLength)
		{
			if (version == 0)
			{
				return LZ4Pickler.EncodeUncompressedHeaderV0(target);
			}
			throw LZ4Pickler.UnexpectedVersion(version);
		}

		private unsafe static int EncodeUncompressedHeaderV0(Span<byte> target)
		{
			*target[0] = 0;
			return 1;
		}

		private static int EncodeCompressedHeader(Span<byte> target, int version, int headerSize, int sourceLength, int encodedLength)
		{
			if (version == 0)
			{
				return LZ4Pickler.EncodeCompressedHeaderV0(target, headerSize, sourceLength, encodedLength);
			}
			throw LZ4Pickler.UnexpectedVersion(version);
		}

		private unsafe static int EncodeCompressedHeaderV0(Span<byte> target, int headerSize, int sourceLength, int encodedLength)
		{
			int value = sourceLength - encodedLength;
			int num = headerSize - 1;
			*target[0] = LZ4Pickler.EncodeHeaderByteV0(num);
			LZ4Pickler.PokeN(target.Slice(1), value, num);
			return 1 + num;
		}

		private unsafe static void PokeN(Span<byte> target, int value, int size)
		{
			if (size < 0 || size > 4 || target.Length < size)
			{
				throw new ArgumentException(string.Format("Unexpected size: {0}", size));
			}
			Unsafe.CopyBlockUnaligned(target[0], ref *(byte*)(&value), (uint)size);
		}

		private static byte EncodeHeaderByteV0(int sizeOfDiff)
		{
			return (byte)(0 | (LZ4Pickler.EncodeSizeOf(sizeOfDiff) & 3) << 6);
		}

		private static int EffectiveSizeOf(int value)
		{
			if (value > 255)
			{
				if (value <= 65535)
				{
					return 2;
				}
			}
			else if (value >= 0)
			{
				return 1;
			}
			return 4;
		}

		private static int EncodeSizeOf(int size)
		{
			int result;
			if (size == 4)
			{
				result = 3;
			}
			else
			{
				result = size;
			}
			return result;
		}

		[NullableContext(1)]
		private static Exception UnexpectedVersion(int version)
		{
			return new ArgumentException(string.Format("Unexpected pickle version: {0}", version));
		}

		[NullableContext(1)]
		public static byte[] Unpickle(byte[] source)
		{
			return LZ4Pickler.Unpickle(source.AsSpan<byte>());
		}

		[NullableContext(1)]
		public static byte[] Unpickle(byte[] source, int index, int count)
		{
			return LZ4Pickler.Unpickle(source.AsSpan(index, count));
		}

		[return: Nullable(1)]
		public unsafe static byte[] Unpickle(byte* source, int count)
		{
			return LZ4Pickler.Unpickle(new Span<byte>((void*)source, count));
		}

		[return: Nullable(1)]
		public static byte[] Unpickle(ReadOnlySpan<byte> source)
		{
			if (source.Length == 0)
			{
				return Mem.Empty;
			}
			PickleHeader pickleHeader = LZ4Pickler.DecodeHeader(source);
			int num = LZ4Pickler.UnpickledSize(pickleHeader);
			if (num == 0)
			{
				return Mem.Empty;
			}
			byte[] array = new byte[num];
			LZ4Pickler.UnpickleCore(pickleHeader, source, array);
			return array;
		}

		public static void Unpickle<TBufferWriter>(ReadOnlySpan<byte> source, [Nullable(1)] TBufferWriter writer) where TBufferWriter : IBufferWriter<byte>
		{
			writer.Required("writer");
			if (source.Length == 0)
			{
				return;
			}
			PickleHeader pickleHeader = LZ4Pickler.DecodeHeader(source);
			int num = LZ4Pickler.UnpickledSize(pickleHeader);
			Span<byte> target = writer.GetSpan(num).Slice(0, num);
			LZ4Pickler.UnpickleCore(pickleHeader, source, target);
			writer.Advance(num);
		}

		public static void Unpickle(ReadOnlySpan<byte> source, [Nullable(1)] IBufferWriter<byte> writer)
		{
			LZ4Pickler.Unpickle<IBufferWriter<byte>>(source, writer);
		}

		public static int UnpickledSize(ReadOnlySpan<byte> source)
		{
			PickleHeader pickleHeader = LZ4Pickler.DecodeHeader(source);
			return LZ4Pickler.UnpickledSize(pickleHeader);
		}

		private static int UnpickledSize(in PickleHeader header)
		{
			return header.ResultLength;
		}

		public static void Unpickle(ReadOnlySpan<byte> source, Span<byte> output)
		{
			if (source.Length == 0)
			{
				return;
			}
			PickleHeader pickleHeader = LZ4Pickler.DecodeHeader(source);
			LZ4Pickler.UnpickleCore(pickleHeader, source, output);
		}

		private static void UnpickleCore(in PickleHeader header, ReadOnlySpan<byte> source, Span<byte> target)
		{
			ReadOnlySpan<byte> source2 = source.Slice((int)header.DataOffset);
			int num = LZ4Pickler.UnpickledSize(header);
			int length = target.Length;
			if (length != num)
			{
				throw LZ4Pickler.CorruptedPickle(string.Format("Output buffer size ({0}) does not match expected value ({1})", length, num));
			}
			if (!header.IsCompressed)
			{
				source2.CopyTo(target);
				return;
			}
			int num2 = LZ4Codec.Decode(source2, target);
			if (num2 != num)
			{
				throw LZ4Pickler.CorruptedPickle(string.Format("Expected to decode {0} bytes but {1} has been decoded", num, num2));
			}
		}

		private unsafe static PickleHeader DecodeHeader(ReadOnlySpan<byte> source)
		{
			int num = (int)(*source[0] & 7);
			if (num == 0)
			{
				return LZ4Pickler.DecodeHeaderV0(source);
			}
			throw LZ4Pickler.CorruptedPickle(string.Format("Version {0} is not recognized", num));
		}

		private unsafe static PickleHeader DecodeHeaderV0(ReadOnlySpan<byte> source)
		{
			int num = *source[0] >> 6 & 3;
			int num2;
			if (num == 3)
			{
				num2 = 4;
			}
			else
			{
				num2 = num;
			}
			int num3 = num2;
			ushort num4 = (ushort)(1 + num3);
			int num5 = source.Length - (int)num4;
			if (num5 < 0)
			{
				throw LZ4Pickler.CorruptedPickle(string.Format("Unexpected data length: {0}", num5));
			}
			int num6 = (num3 == 0) ? 0 : LZ4Pickler.PeekN(source.Slice(1), num3);
			int resultLength = num5 + num6;
			return new PickleHeader(num4, resultLength, num6 != 0);
		}

		private unsafe static int PeekN(ReadOnlySpan<byte> bytes, int size)
		{
			int result = 0;
			if (size < 0 || size > 4 || size > bytes.Length)
			{
				throw LZ4Pickler.CorruptedPickle(string.Format("Unexpected field size: {0}", size));
			}
			fixed (byte* pinnableReference = bytes.GetPinnableReference())
			{
				byte* source = pinnableReference;
				Unsafe.CopyBlockUnaligned((void*)(&result), (void*)source, (uint)size);
			}
			return result;
		}

		[NullableContext(1)]
		private static Exception CorruptedPickle(string message)
		{
			return new InvalidDataException("Pickle is corrupted: " + message);
		}

		private const int MAX_STACKALLOC = 1024;

		private const byte VersionMask = 7;
	}
}
