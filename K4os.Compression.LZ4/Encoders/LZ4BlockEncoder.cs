using System;

namespace K4os.Compression.LZ4.Encoders
{
	public class LZ4BlockEncoder : LZ4EncoderBase
	{
		public LZ4BlockEncoder(LZ4Level level, int blockSize) : base(false, blockSize, 0)
		{
			this._level = level;
		}

		protected unsafe override int EncodeBlock(byte* source, int sourceLength, byte* target, int targetLength)
		{
			return LZ4Codec.Encode(source, sourceLength, target, targetLength, this._level);
		}

		protected unsafe override int CopyDict(byte* target, int dictionaryLength)
		{
			return 0;
		}

		private readonly LZ4Level _level;
	}
}
