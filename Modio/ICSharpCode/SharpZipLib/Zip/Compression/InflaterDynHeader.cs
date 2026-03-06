using System;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	internal class InflaterDynHeader
	{
		public bool AttemptRead()
		{
			return !this.state.MoveNext() || this.state.Current;
		}

		public InflaterDynHeader(StreamManipulator input)
		{
			this.input = input;
			this.stateMachine = this.CreateStateMachine();
			this.state = this.stateMachine.GetEnumerator();
		}

		private IEnumerable<bool> CreateStateMachine()
		{
			while (!this.input.TryGetBits(5, ref this.litLenCodeCount, 257))
			{
				yield return false;
			}
			while (!this.input.TryGetBits(5, ref this.distanceCodeCount, 1))
			{
				yield return false;
			}
			while (!this.input.TryGetBits(4, ref this.metaCodeCount, 4))
			{
				yield return false;
			}
			int dataCodeCount = this.litLenCodeCount + this.distanceCodeCount;
			if (this.litLenCodeCount > 286)
			{
				throw new ValueOutOfRangeException("litLenCodeCount");
			}
			if (this.distanceCodeCount > 30)
			{
				throw new ValueOutOfRangeException("distanceCodeCount");
			}
			if (this.metaCodeCount > 19)
			{
				throw new ValueOutOfRangeException("metaCodeCount");
			}
			int num;
			for (int i = 0; i < this.metaCodeCount; i = num + 1)
			{
				while (!this.input.TryGetBits(3, ref this.codeLengths, InflaterDynHeader.MetaCodeLengthIndex[i]))
				{
					yield return false;
				}
				num = i;
			}
			InflaterHuffmanTree metaCodeTree = new InflaterHuffmanTree(this.codeLengths);
			int index = 0;
			while (index < dataCodeCount)
			{
				int symbol;
				while ((symbol = metaCodeTree.GetSymbol(this.input)) < 0)
				{
					yield return false;
				}
				if (symbol < 16)
				{
					byte[] array = this.codeLengths;
					num = index;
					index = num + 1;
					array[num] = (byte)symbol;
				}
				else
				{
					int i = 0;
					byte codeLength;
					if (symbol == 16)
					{
						if (index == 0)
						{
							throw new StreamDecodingException("Cannot repeat previous code length when no other code length has been read");
						}
						codeLength = this.codeLengths[index - 1];
						while (!this.input.TryGetBits(2, ref i, 3))
						{
							yield return false;
						}
					}
					else if (symbol == 17)
					{
						codeLength = 0;
						while (!this.input.TryGetBits(3, ref i, 3))
						{
							yield return false;
						}
					}
					else
					{
						codeLength = 0;
						while (!this.input.TryGetBits(7, ref i, 11))
						{
							yield return false;
						}
					}
					if (index + i > dataCodeCount)
					{
						throw new StreamDecodingException("Cannot repeat code lengths past total number of data code lengths");
					}
					for (;;)
					{
						num = i;
						i = num - 1;
						if (num <= 0)
						{
							break;
						}
						byte[] array2 = this.codeLengths;
						num = index;
						index = num + 1;
						array2[num] = codeLength;
					}
				}
			}
			if (this.codeLengths[256] == 0)
			{
				throw new StreamDecodingException("Inflater dynamic header end-of-block code missing");
			}
			this.litLenTree = new InflaterHuffmanTree(new ArraySegment<byte>(this.codeLengths, 0, this.litLenCodeCount));
			this.distTree = new InflaterHuffmanTree(new ArraySegment<byte>(this.codeLengths, this.litLenCodeCount, this.distanceCodeCount));
			yield return true;
			yield break;
		}

		public InflaterHuffmanTree LiteralLengthTree
		{
			get
			{
				InflaterHuffmanTree inflaterHuffmanTree = this.litLenTree;
				if (inflaterHuffmanTree == null)
				{
					throw new StreamDecodingException("Header properties were accessed before header had been successfully read");
				}
				return inflaterHuffmanTree;
			}
		}

		public InflaterHuffmanTree DistanceTree
		{
			get
			{
				InflaterHuffmanTree inflaterHuffmanTree = this.distTree;
				if (inflaterHuffmanTree == null)
				{
					throw new StreamDecodingException("Header properties were accessed before header had been successfully read");
				}
				return inflaterHuffmanTree;
			}
		}

		private const int LITLEN_MAX = 286;

		private const int DIST_MAX = 30;

		private const int CODELEN_MAX = 316;

		private const int META_MAX = 19;

		private static readonly int[] MetaCodeLengthIndex = new int[]
		{
			16,
			17,
			18,
			0,
			8,
			7,
			9,
			6,
			10,
			5,
			11,
			4,
			12,
			3,
			13,
			2,
			14,
			1,
			15
		};

		private readonly StreamManipulator input;

		private readonly IEnumerator<bool> state;

		private readonly IEnumerable<bool> stateMachine;

		private byte[] codeLengths = new byte[316];

		private InflaterHuffmanTree litLenTree;

		private InflaterHuffmanTree distTree;

		private int litLenCodeCount;

		private int distanceCodeCount;

		private int metaCodeCount;
	}
}
