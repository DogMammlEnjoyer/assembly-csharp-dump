using System;
using K4os.Compression.LZ4.Engine;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4.Encoders
{
	public class LZ4ChainDecoder : UnmanagedResources, ILZ4Decoder, IDisposable
	{
		private unsafe byte* OutputBuffer
		{
			get
			{
				return this._outputBufferPin.Pointer;
			}
		}

		private unsafe LL.LZ4_streamDecode_t* Context
		{
			get
			{
				return this._contextPin.Reference<LL.LZ4_streamDecode_t>();
			}
		}

		public LZ4ChainDecoder(int blockSize, int extraBlocks)
		{
			blockSize = Mem.RoundUp(Math.Max(blockSize, 1024), 1024);
			extraBlocks = Math.Max(extraBlocks, 0);
			this._blockSize = blockSize;
			this._outputLength = 65536 + (1 + extraBlocks) * this._blockSize + 32;
			this._outputIndex = 0;
			PinnedMemory.Alloc<LL.LZ4_streamDecode_t>(out this._contextPin, true);
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

		public unsafe int Decode(byte* source, int length, int blockSize)
		{
			if (blockSize <= 0)
			{
				blockSize = this._blockSize;
			}
			this.Prepare(blockSize);
			int num = this.DecodeBlock(source, length, this.OutputBuffer + this._outputIndex, blockSize);
			if (num < 0)
			{
				throw new InvalidOperationException();
			}
			this._outputIndex += num;
			return num;
		}

		public unsafe int Inject(byte* source, int length)
		{
			if (length <= 0)
			{
				return 0;
			}
			if (length > Math.Max(this._blockSize, 65536))
			{
				throw new InvalidOperationException();
			}
			byte* outputBuffer = this.OutputBuffer;
			if (this._outputIndex + length < this._outputLength)
			{
				Mem.Move(outputBuffer + this._outputIndex, source, length);
				this._outputIndex = this.ApplyDict(this._outputIndex + length);
			}
			else if (length >= 65536)
			{
				Mem.Move(outputBuffer, source, length);
				this._outputIndex = this.ApplyDict(length);
			}
			else
			{
				int num = Math.Min(65536 - length, this._outputIndex);
				Mem.Move(outputBuffer, outputBuffer + this._outputIndex - num, num);
				Mem.Move(outputBuffer + num, source, length);
				this._outputIndex = this.ApplyDict(num + length);
			}
			return length;
		}

		public unsafe void Drain(byte* target, int offset, int length)
		{
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

		private void Prepare(int blockSize)
		{
			if (this._outputIndex + blockSize <= this._outputLength)
			{
				return;
			}
			this._outputIndex = this.CopyDict(this._outputIndex);
		}

		private int CopyDict(int index)
		{
			int num = Math.Max(index - 65536, 0);
			int num2 = index - num;
			Mem.Move(this.OutputBuffer, this.OutputBuffer + num, num2);
			LL.LZ4_setStreamDecode(this.Context, this.OutputBuffer, num2);
			return num2;
		}

		private int ApplyDict(int index)
		{
			int num = Math.Max(index - 65536, 0);
			int dictSize = index - num;
			LL.LZ4_setStreamDecode(this.Context, this.OutputBuffer + num, dictSize);
			return index;
		}

		private unsafe int DecodeBlock(byte* source, int sourceLength, byte* target, int targetLength)
		{
			return LLxx.LZ4_decompress_safe_continue(this.Context, source, target, sourceLength, targetLength);
		}

		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			this._contextPin.Free();
			this._outputBufferPin.Free();
		}

		private PinnedMemory _outputBufferPin;

		private PinnedMemory _contextPin;

		private readonly int _blockSize;

		private readonly int _outputLength;

		private int _outputIndex;
	}
}
