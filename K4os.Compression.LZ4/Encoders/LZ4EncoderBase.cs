using System;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4.Encoders
{
	public abstract class LZ4EncoderBase : UnmanagedResources, ILZ4Encoder, IDisposable
	{
		private unsafe byte* InputBuffer
		{
			get
			{
				return this._inputBufferPin.Pointer;
			}
		}

		protected LZ4EncoderBase(bool chaining, int blockSize, int extraBlocks)
		{
			blockSize = Mem.RoundUp(Math.Max(blockSize, 1024), 1024);
			extraBlocks = Math.Max(extraBlocks, 0);
			int num = chaining ? 65536 : 0;
			this._blockSize = blockSize;
			this._inputLength = num + (1 + extraBlocks) * blockSize + 32;
			this._inputIndex = (this._inputPointer = 0);
			PinnedMemory.Alloc(out this._inputBufferPin, this._inputLength + 8, false);
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
				return this._inputPointer - this._inputIndex;
			}
		}

		public unsafe int Topup(byte* source, int length)
		{
			base.ThrowIfDisposed();
			if (length == 0)
			{
				return 0;
			}
			int num = this._inputIndex + this._blockSize - this._inputPointer;
			if (num <= 0)
			{
				return 0;
			}
			int num2 = Math.Min(num, length);
			Mem.Move(this.InputBuffer + this._inputPointer, source, num2);
			this._inputPointer += num2;
			return num2;
		}

		public unsafe int Encode(byte* target, int length, bool allowCopy)
		{
			base.ThrowIfDisposed();
			int num = this._inputPointer - this._inputIndex;
			if (num <= 0)
			{
				return 0;
			}
			int num2 = this.EncodeBlock(this.InputBuffer + this._inputIndex, num, target, length);
			if (num2 <= 0)
			{
				throw new InvalidOperationException("Failed to encode chunk. Target buffer too small.");
			}
			if (allowCopy && num2 >= num)
			{
				Mem.Move(target, this.InputBuffer + this._inputIndex, num);
				num2 = -num;
			}
			this.Commit();
			return num2;
		}

		private void Commit()
		{
			this._inputIndex = this._inputPointer;
			if (this._inputIndex + this._blockSize <= this._inputLength)
			{
				return;
			}
			this._inputIndex = (this._inputPointer = this.CopyDict(this.InputBuffer, this._inputPointer));
		}

		protected unsafe abstract int EncodeBlock(byte* source, int sourceLength, byte* target, int targetLength);

		protected unsafe abstract int CopyDict(byte* target, int dictionaryLength);

		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			this._inputBufferPin.Free();
		}

		private PinnedMemory _inputBufferPin;

		private readonly int _inputLength;

		private readonly int _blockSize;

		private int _inputIndex;

		private int _inputPointer;
	}
}
