using System;
using System.Buffers;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Compression
{
	public struct BrotliDecoder : IDisposable
	{
		internal void InitializeDecoder()
		{
			this._state = Interop.Brotli.BrotliDecoderCreateInstance(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (this._state.IsInvalid)
			{
				throw new IOException("Failed to create BrotliDecoder instance");
			}
		}

		internal void EnsureInitialized()
		{
			this.EnsureNotDisposed();
			if (this._state == null)
			{
				this.InitializeDecoder();
			}
		}

		public void Dispose()
		{
			this._disposed = true;
			SafeBrotliDecoderHandle state = this._state;
			if (state == null)
			{
				return;
			}
			state.Dispose();
		}

		private void EnsureNotDisposed()
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("BrotliDecoder", "Can not access a closed Decoder.");
			}
		}

		public unsafe OperationStatus Decompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
		{
			this.EnsureInitialized();
			bytesConsumed = 0;
			bytesWritten = 0;
			if (Interop.Brotli.BrotliDecoderIsFinished(this._state))
			{
				return OperationStatus.Done;
			}
			IntPtr value = (IntPtr)destination.Length;
			IntPtr value2 = (IntPtr)source.Length;
			while ((int)value > 0)
			{
				fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
				{
					byte* ptr = reference;
					fixed (byte* reference2 = MemoryMarshal.GetReference<byte>(destination))
					{
						byte* ptr2 = reference2;
						byte* ptr3 = ptr;
						byte* ptr4 = ptr2;
						IntPtr intPtr;
						int num = Interop.Brotli.BrotliDecoderDecompressStream(this._state, ref value2, &ptr3, ref value, &ptr4, out intPtr);
						if (num == 0)
						{
							return OperationStatus.InvalidData;
						}
						bytesConsumed += source.Length - (int)value2;
						bytesWritten += destination.Length - (int)value;
						switch (num)
						{
						case 1:
							return OperationStatus.Done;
						case 3:
							return OperationStatus.DestinationTooSmall;
						}
						source = source.Slice(source.Length - (int)value2);
						destination = destination.Slice(destination.Length - (int)value);
						if (num == 2 && source.Length == 0)
						{
							return OperationStatus.NeedMoreData;
						}
					}
				}
			}
			return OperationStatus.DestinationTooSmall;
		}

		public unsafe static bool TryDecompress(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
		{
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(source))
			{
				byte* inBytes = reference;
				fixed (byte* reference2 = MemoryMarshal.GetReference<byte>(destination))
				{
					byte* outBytes = reference2;
					IntPtr value = (IntPtr)destination.Length;
					bool result = Interop.Brotli.BrotliDecoderDecompress((IntPtr)source.Length, inBytes, ref value, outBytes);
					bytesWritten = (int)value;
					return result;
				}
			}
		}

		private SafeBrotliDecoderHandle _state;

		private bool _disposed;
	}
}
