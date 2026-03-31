using System;
using K4os.Compression.LZ4.Engine;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4.Encoders
{
	public class LZ4HighChainEncoder : LZ4EncoderBase
	{
		private unsafe LL.LZ4_streamHC_t* Context
		{
			get
			{
				return this._contextPin.Reference<LL.LZ4_streamHC_t>();
			}
		}

		public LZ4HighChainEncoder(LZ4Level level, int blockSize, int extraBlocks = 0) : base(true, blockSize, extraBlocks)
		{
			if (level < LZ4Level.L03_HC)
			{
				level = LZ4Level.L03_HC;
			}
			if (level > LZ4Level.L12_MAX)
			{
				level = LZ4Level.L12_MAX;
			}
			PinnedMemory.Alloc<LL.LZ4_streamHC_t>(out this._contextPin, false);
			LL.LZ4_initStreamHC(this.Context);
			LL.LZ4_resetStreamHC_fast(this.Context, (int)level);
		}

		protected override void ReleaseUnmanaged()
		{
			base.ReleaseUnmanaged();
			this._contextPin.Free();
		}

		protected unsafe override int EncodeBlock(byte* source, int sourceLength, byte* target, int targetLength)
		{
			return LLxx.LZ4_compress_HC_continue(this.Context, source, target, sourceLength, targetLength);
		}

		protected unsafe override int CopyDict(byte* target, int length)
		{
			return LL.LZ4_saveDictHC(this.Context, target, length);
		}

		private PinnedMemory _contextPin;
	}
}
