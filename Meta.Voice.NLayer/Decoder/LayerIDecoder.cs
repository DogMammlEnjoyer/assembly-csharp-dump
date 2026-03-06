using System;

namespace Meta.Voice.NLayer.Decoder
{
	internal class LayerIDecoder : LayerIIDecoderBase
	{
		internal static bool GetCRC(MpegFrame frame, ref uint crc)
		{
			return LayerIIDecoderBase.GetCRC(frame, LayerIDecoder._rateTable, LayerIDecoder._allocLookupTable, false, ref crc);
		}

		internal LayerIDecoder() : base(LayerIDecoder._allocLookupTable, 1)
		{
		}

		protected override int[] GetRateTable(IMpegFrame frame)
		{
			return LayerIDecoder._rateTable;
		}

		protected override void ReadScaleFactorSelection(IMpegFrame frame, int[][] scfsi, int channels)
		{
		}

		private static readonly int[] _rateTable = new int[32];

		private static readonly int[][] _allocLookupTable = new int[][]
		{
			new int[]
			{
				4,
				0,
				2,
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
			}
		};
	}
}
