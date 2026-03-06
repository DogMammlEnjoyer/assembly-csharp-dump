using System;

namespace Oculus.Interaction.Input.Compatibility.OVR
{
	[Flags]
	public enum HandFingerJointFlags
	{
		None = 0,
		Wrist = 1,
		ForearmStub = 2,
		Thumb0 = 4,
		Thumb1 = 8,
		Thumb2 = 16,
		Thumb3 = 32,
		Index1 = 64,
		Index2 = 128,
		Index3 = 256,
		Middle1 = 512,
		Middle2 = 1024,
		Middle3 = 2048,
		Ring1 = 4096,
		Ring2 = 8192,
		Ring3 = 16384,
		Pinky0 = 32768,
		Pinky1 = 65536,
		Pinky2 = 131072,
		Pinky3 = 262144,
		HandMaxSkinnable = 524288,
		ThumbTip = 524288,
		IndexTip = 1048576,
		MiddleTip = 2097152,
		RingTip = 4194304,
		PinkyTip = 8388608,
		All = 16777215
	}
}
