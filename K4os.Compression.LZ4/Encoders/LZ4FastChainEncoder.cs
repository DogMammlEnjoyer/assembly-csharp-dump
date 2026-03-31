using System;
using K4os.Compression.LZ4.Engine;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4.Encoders
{
	public class LZ4FastChainEncoder : LZ4EncoderBase
	{
		private unsafe LL.LZ4_stream_t* Context
		{
			get
			{
				return this._contextPin.Reference<LL.LZ4_stream_t>();
			}
		}

		public LZ4FastChainEncoder(int blockSize, int extraBlocks = 0) : base(true, blockSize, extraBlocks)
		{
			PinnedMemory.Alloc<LL.LZ4_stream_t>(out this._contextPin, true);
		}

		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			this._contextPin.Free();
		}

		protected unsafe override int EncodeBlock(byte* source, int sourceLength, byte* target, int targetLength)
		{
			return LLxx.LZ4_compress_fast_continue(this.Context, source, target, sourceLength, targetLength, 1);
		}

		protected unsafe override int CopyDict(byte* target, int length)
		{
			return LL.LZ4_saveDict(this.Context, target, length);
		}

		private PinnedMemory _contextPin;
	}
}
