using System;

namespace UnityEngine.UIElements
{
	[Flags]
	internal enum RenderHints
	{
		None = 0,
		GroupTransform = 1,
		BoneTransform = 2,
		ClipWithScissors = 4,
		MaskContainer = 8,
		DynamicColor = 16,
		DynamicPostProcessing = 32,
		DirtyOffset = 6,
		DirtyGroupTransform = 64,
		DirtyBoneTransform = 128,
		DirtyClipWithScissors = 256,
		DirtyMaskContainer = 512,
		DirtyDynamicColor = 1024,
		DirtyDynamicPostProcessing = 2048,
		DirtyAll = 4032
	}
}
