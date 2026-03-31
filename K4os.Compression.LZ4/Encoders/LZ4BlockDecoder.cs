using System;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4.Encoders
{
	public class LZ4BlockDecoder : UnmanagedResources, ILZ4Decoder, IDisposable
	{
		private unsafe byte* OutputBuffer
		{
			get
			{
				return this._outputBufferPin.Pointer;
			}
		}

		public LZ4BlockDecoder(int blockSize)
		{
			blockSize = Mem.RoundUp(Math.Max(blockSize, 1024), 1024);
			this._blockSize = blockSize;
			this._outputLength = this._blockSize + 8;
			this._outputIndex = 0;
			PinnedMemory.Alloc(out this._outputBufferPin, this._outputLength + 8, false);
		}

		public int BlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		public int BytesReady
		{
			get
			{
				return this._outputIndex;
			}
		}

		public unsafe int Decode(byte* source, int length, int blockSize = 0)
		{
			base.ThrowIfDisposed();
			if (blockSize <= 0)
			{
				blockSize = this._blockSize;
			}
			if (blockSize > this._blockSize)
			{
				throw new InvalidOperationException();
			}
			int num = LZ4Codec.Decode(source, length, this.OutputBuffer, this._outputLength);
			if (num < 0)
			{
				throw new InvalidOperationException();
			}
			this._outputIndex = num;
			return this._outputIndex;
		}

		public unsafe int Inject(byte* source, int length)
		{
			base.ThrowIfDisposed();
			if (length <= 0)
			{
				return this._outputIndex = 0;
			}
			if (length > this._outputLength)
			{
				throw new InvalidOperationException();
			}
			Mem.Move(this.OutputBuffer, source, length);
			this._outputIndex = length;
			return length;
		}

		public unsafe void Drain(byte* target, int offset, int length)
		{
			base.ThrowIfDisposed();
			offset = this._outputIndex + offset;
			if (offset < 0 || length < 0 || offset + length > this._outputIndex)
			{
				throw new InvalidOperationException();
			}
			Mem.Move(target, this.OutputBuffer + offset, length);
		}

		public unsafe byte* Peek(int offset)
		{
			base.ThrowIfDisposed();
			offset = this._outputIndex + offset;
			if (offset < 0 || offset > this._outputIndex)
			{
				throw new InvalidOperationException();
			}
			return this.OutputBuffer + offset;
		}

		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			this._outputBufferPin.Free();
		}

		private PinnedMemory _outputBufferPin;

		private readonly int _outputLength;

		private int _outputIndex;

		private readonly int _blockSize;
	}
}
