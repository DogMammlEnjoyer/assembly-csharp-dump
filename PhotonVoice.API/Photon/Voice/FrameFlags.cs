using System;

namespace Photon.Voice
{
	public enum FrameFlags : byte
	{
		Config = 1,
		KeyFrame,
		PartialFrame = 4,
		EndOfStream = 8
	}
}
