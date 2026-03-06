using System;

namespace UnityEngine.XR.OpenXR.API
{
	public enum UnityXRRenderTextureFormat
	{
		kUnityXRRenderTextureFormatRGBA32,
		kUnityXRRenderTextureFormatBGRA32,
		kUnityXRRenderTextureFormatRGB565,
		kUnityXRRenderTextureFormatR16G16B16A16_SFloat,
		kUnityXRRenderTextureFormatRGBA1010102,
		kUnityXRRenderTextureFormatBGRA1010102,
		kUnityXRRenderTextureFormatR11G11B10_UFloat,
		kUnityXRRenderTextureFormatReference = 64,
		kUnityXRRenderTextureFormatSoftReferenceMSAA,
		kUnityXRRenderTextureFormatNone
	}
}
