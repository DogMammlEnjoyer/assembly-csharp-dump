using System;

namespace Meta.Voice.NLayer.Decoder
{
	internal class LayerIIDecoder : LayerIIDecoderBase
	{
		internal static bool GetCRC(MpegFrame frame, ref uint crc)
		{
			return LayerIIDecoderBase.GetCRC(frame, LayerIIDecoder.SelectTable(frame), LayerIIDecoder._allocLookupTable, true, ref crc);
		}

		private static int[] SelectTable(IMpegFrame frame)
		{
			int num = frame.BitRate / ((frame.ChannelMode == MpegChannelMode.Mono) ? 1 : 2) / 1000;
			if (frame.Version != MpegVersion.Version1)
			{
				return LayerIIDecoder._rateLookupTable[4];
			}
			if ((num >= 56 && num <= 80) || (frame.SampleRate == 48000 && num >= 56))
			{
				return LayerIIDecoder._rateLookupTable[0];
			}
			if (frame.SampleRate != 48000 && num >= 96)
			{
				return LayerIIDecoder._rateLookupTable[1];
			}
			if (frame.SampleRate != 32000 && num <= 48)
			{
				return LayerIIDecoder._rateLookupTable[2];
			}
			return LayerIIDecoder._rateLookupTable[3];
		}

		internal LayerIIDecoder() : base(LayerIIDecoder._allocLookupTable, 3)
		{
		}

		protected override int[] GetRateTable(IMpegFrame frame)
		{
			return LayerIIDecoder.SelectTable(frame);
		}

		protected override void ReadScaleFactorSelection(IMpegFrame frame, int[][] scfsi, int channels)
		{
			for (int i = 0; i < 30; i++)
			{
				for (int j = 0; j < channels; j++)
				{
					if (scfsi[j][i] == 2)
					{
						scfsi[j][i] = frame.ReadBits(2);
					}
				}
			}
		}

		private static readonly int[][] _rateLookupTable = new int[][]
		{
			new int[]
			{
				3,
				3,
				3,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				0,
				0,
				0,
				0
			},
			new int[]
			{
				3,
				3,
				3,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				2,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			},
			new int[]
			{
				4,
				4,
				5,
				5,
				5,
				5,
				5,
				5
			},
			new int[]
			{
				4,
				4,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5
			},
			new int[]
			{
				6,
				6,
				6,
				6,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7
			}
		};

		private static readonly int[][] _allocLookupTable = new int[][]
		{
			new int[]
			{
				2,
				0,
				-5,
				-7,
				16
			},
			new int[]
			{
				3,
				0,
				-5,
				-7,
				3,
				-10,
				4,
				5,
				16
			},
			new int[]
			{
				4,
				0,
				-5,
				-7,
				3,
				-10,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				16
			},
			new int[]
			{
				4,
				0,
				-5,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14,
				15,
				16
			},
			new int[]
			{
				4,
				0,
				-5,
				-7,
				-10,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14,
				15
			},
			new int[]
			{
				3,
				0,
				-5,
				-7,
				-10,
				4,
				5,
				6,
				9
			},
			new int[]
			{
				4,
				0,
				-5,
				-7,
				3,
				-10,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14
			},
			new int[]
			{
				2,
				0,
				-5,
				-7,
				3
			}
		};
	}
}
