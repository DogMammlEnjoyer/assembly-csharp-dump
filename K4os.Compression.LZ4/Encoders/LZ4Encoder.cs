using System;

namespace K4os.Compression.LZ4.Encoders
{
	public static class LZ4Encoder
	{
		public static ILZ4Encoder Create(bool chaining, LZ4Level level, int blockSize, int extraBlocks = 0)
		{
			if (!chaining)
			{
				return LZ4Encoder.CreateBlockEncoder(level, blockSize);
			}
			if (level >= LZ4Level.L03_HC)
			{
				return LZ4Encoder.CreateHighEncoder(level, blockSize, extraBlocks);
			}
			return LZ4Encoder.CreateFastEncoder(blockSize, extraBlocks);
		}

		private static ILZ4Encoder CreateBlockEncoder(LZ4Level level, int blockSize)
		{
			return new LZ4BlockEncoder(level, blockSize);
		}

		private static ILZ4Encoder CreateFastEncoder(int blockSize, int extraBlocks)
		{
			return new LZ4FastChainEncoder(blockSize, extraBlocks);
		}

		private static ILZ4Encoder CreateHighEncoder(LZ4Level level, int blockSize, int extraBlocks)
		{
			return new LZ4HighChainEncoder(level, blockSize, extraBlocks);
		}
	}
}
