using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace K4os.Compression.LZ4.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	public static class BufferPool
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ShouldBePooled(int length)
		{
			return length >= 512;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static byte[] Rent(int size, bool zero)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(size);
			if (zero)
			{
				array.AsSpan(0, size).Clear();
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Alloc(int size, bool zero = false)
		{
			if (!BufferPool.ShouldBePooled(size))
			{
				return new byte[size];
			}
			return BufferPool.Rent(size, zero);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPooled(byte[] buffer)
		{
			return BufferPool.ShouldBePooled(buffer.Length);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(byte[] buffer)
		{
			if (buffer != null && BufferPool.IsPooled(buffer))
			{
				ArrayPool<byte>.Shared.Return(buffer, false);
			}
		}

		public const int MinPooledSize = 512;
	}
}
