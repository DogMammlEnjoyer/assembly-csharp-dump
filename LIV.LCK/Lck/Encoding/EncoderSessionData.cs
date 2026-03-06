using System;

namespace Liv.Lck.Encoding
{
	internal struct EncoderSessionData
	{
		public ulong EncodedVideoFrames { readonly get; set; }

		public ulong EncodedAudioSamplesPerChannel { readonly get; set; }

		public float CaptureTimeSeconds { readonly get; set; }
	}
}
