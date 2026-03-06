using System;

namespace Oculus.Interaction.Input
{
	[Flags]
	public enum HandFingerJointFlags
	{
		None = 0,
		Palm = 1,
		Wrist = 2,
		Thumb1 = 4,
		Thumb2 = 8,
		Thumb3 = 16,
		ThumbTip = 32,
		Index0 = 64,
		Index1 = 128,
		Index2 = 256,
		Index3 = 512,
		IndexTip = 1024,
		Middle0 = 2048,
		Middle1 = 4096,
		Middle2 = 8192,
		Middle3 = 16384,
		MiddleTip = 32768,
		Ring0 = 65536,
		Ring1 = 131072,
		Ring2 = 262144,
		Ring3 = 524288,
		RingTip = 1048576,
		Pinky0 = 2097152,
		Pinky1 = 4194304,
		Pinky2 = 8388608,
		Pinky3 = 16777216,
		PinkyTip = 33554432,
		HandMaxSkinnable = 67108864,
		All = 67108863
	}
}
