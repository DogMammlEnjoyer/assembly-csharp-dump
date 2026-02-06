using System;

namespace CSCore
{
	[Flags]
	public enum ChannelMask
	{
		SpeakerFrontLeft = 1,
		SpeakerFrontRight = 2,
		SpeakerFrontCenter = 4,
		SpeakerLowFrequency = 8,
		SpeakerBackLeft = 16,
		SpeakerBackRight = 32,
		SpeakerFrontLeftOfCenter = 64,
		SpeakerFrontRightOfCenter = 128,
		SpeakerBackCenter = 256,
		SpeakerSideLeft = 512,
		SpeakerSideRight = 1024,
		SpeakerTopCenter = 2048,
		SpeakerTopFrontLeft = 4096,
		SpeakerTopFrontCenter = 8192,
		SpeakerTopFrontRight = 16384,
		SpeakerTopBackLeft = 32768,
		SpeakerTopBackCenter = 65536,
		SpeakerTopBackRight = 131072
	}
}
