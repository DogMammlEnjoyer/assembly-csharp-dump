using System;

namespace Meta.Voice.NLayer.Decoder
{
	internal class VBRInfo
	{
		internal VBRInfo()
		{
		}

		internal int SampleCount { get; set; }

		internal int SampleRate { get; set; }

		internal int Channels { get; set; }

		internal int VBRFrames { get; set; }

		internal int VBRBytes { get; set; }

		internal int VBRQuality { get; set; }

		internal int VBRDelay { get; set; }

		internal long VBRStreamSampleCount
		{
			get
			{
				return (long)(this.VBRFrames * this.SampleCount);
			}
		}

		internal int VBRAverageBitrate
		{
			get
			{
				return (int)((double)this.VBRBytes / ((double)this.VBRStreamSampleCount / (double)this.SampleRate) * 8.0);
			}
		}
	}
}
