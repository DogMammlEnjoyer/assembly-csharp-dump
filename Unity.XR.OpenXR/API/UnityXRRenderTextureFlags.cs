using System;

namespace UnityEngine.XR.OpenXR.API
{
	public enum UnityXRRenderTextureFlags
	{
		kUnityXRRenderTextureFlagsUVDirectionTopToBottom = 1,
		kUnityXRRenderTextureFlagsMultisampleAutoResolve,
		kUnityXRRenderTextureFlagsLockedWidthHeight = 4,
		kUnityXRRenderTextureFlagsWriteOnly = 8,
		kUnityXRRenderTextureFlagsSRGB = 16,
		kUnityXRRenderTextureFlagsOptimizeBufferDiscards = 32,
		kUnityXRRenderTextureFlagsMotionVectorTexture = 64,
		kUnityXRRenderTextureFlagsFoveationOffset = 128,
		kUnityXRRenderTextureFlagsViewportAsRenderArea = 256,
		kUnityXRRenderTextureFlagsHDR = 512
	}
}
