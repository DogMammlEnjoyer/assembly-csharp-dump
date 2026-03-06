using System;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace UnityEngine.Experimental.Rendering
{
	internal static class XRMirrorView
	{
		internal static void RenderMirrorView(CommandBuffer cmd, Camera camera, Material mat, XRDisplaySubsystem display)
		{
			if (Application.platform == RuntimePlatform.Android && !XRGraphicsAutomatedTests.running)
			{
				return;
			}
			if (display == null || !display.running || mat == null)
			{
				return;
			}
			int preferredMirrorBlitMode = display.GetPreferredMirrorBlitMode();
			XRDisplaySubsystem.XRMirrorViewBlitDesc xrmirrorViewBlitDesc;
			if (display.GetMirrorViewBlitDesc(null, out xrmirrorViewBlitDesc, preferredMirrorBlitMode))
			{
				using (new ProfilingScope(cmd, XRMirrorView.k_MirrorViewProfilingSampler))
				{
					cmd.SetRenderTarget((camera.targetTexture != null) ? camera.targetTexture : new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget));
					if (xrmirrorViewBlitDesc.nativeBlitAvailable)
					{
						display.AddGraphicsThreadMirrorViewBlit(cmd, xrmirrorViewBlitDesc.nativeBlitInvalidStates, preferredMirrorBlitMode);
					}
					else
					{
						for (int i = 0; i < xrmirrorViewBlitDesc.blitParamsCount; i++)
						{
							XRDisplaySubsystem.XRBlitParams xrblitParams;
							xrmirrorViewBlitDesc.GetBlitParameter(i, out xrblitParams);
							Vector4 vector = new Vector4(xrblitParams.srcRect.width, xrblitParams.srcRect.height, xrblitParams.srcRect.x, xrblitParams.srcRect.y);
							Vector4 value = new Vector4(xrblitParams.destRect.width, xrblitParams.destRect.height, xrblitParams.destRect.x, xrblitParams.destRect.y);
							if (camera.targetTexture != null || camera.cameraType == CameraType.SceneView || camera.cameraType == CameraType.Preview)
							{
								vector.y = -vector.y;
								vector.w += xrblitParams.srcRect.height;
							}
							HDROutputSettings main = HDROutputSettings.main;
							if (xrblitParams.srcHdrEncoded || main.active)
							{
								ColorGamut gamut = main.active ? main.displayColorGamut : ColorGamut.sRGB;
								object obj = xrblitParams.srcHdrEncoded ? xrblitParams.srcHdrColorGamut : ColorGamut.sRGB;
								ColorPrimaries colorPrimaries = ColorGamutUtility.GetColorPrimaries(gamut);
								object gamut2 = obj;
								ColorPrimaries colorPrimaries2 = ColorGamutUtility.GetColorPrimaries(gamut2);
								HDROutputUtils.ConfigureHDROutput(XRMirrorView.s_MirrorViewMaterialProperty, gamut);
								HDROutputUtils.ConfigureHDROutput(mat, HDROutputUtils.Operation.ColorConversion | HDROutputUtils.Operation.ColorEncoding);
								int value2;
								HDROutputUtils.GetColorEncodingForGamut(gamut2, out value2);
								XRMirrorView.s_MirrorViewMaterialProperty.SetInteger(XRMirrorView.k_SourceHDREncoding, value2);
								float3x3 a = float3x3.identity;
								if (colorPrimaries2 == ColorPrimaries.Rec709)
								{
									a = ColorSpaceUtils.Rec709ToRec2020Mat;
								}
								else if (colorPrimaries2 == ColorPrimaries.P3)
								{
									a = ColorSpaceUtils.P3D65ToRec2020Mat;
								}
								float3x3 b = float3x3.identity;
								if (colorPrimaries == ColorPrimaries.Rec709)
								{
									b = ColorSpaceUtils.Rec2020ToRec709Mat;
								}
								else if (colorPrimaries == ColorPrimaries.P3)
								{
									b = ColorSpaceUtils.Rec2020ToP3D65Mat;
								}
								float3x3 float3x = math.mul(a, b);
								Matrix4x4 value3 = new Matrix4x4(new float4(float3x.c0, 0f), new float4(float3x.c1, 0f), new float4(float3x.c2, 0f), new Vector4(0f, 0f, 0f, 0f));
								XRMirrorView.s_MirrorViewMaterialProperty.SetMatrix(XRMirrorView.k_ColorTransform, value3);
								XRMirrorView.s_MirrorViewMaterialProperty.SetFloat(XRMirrorView.k_MaxNits, main.active ? ((float)main.maxToneMapLuminance) : 160f);
								XRMirrorView.s_MirrorViewMaterialProperty.SetFloat(XRMirrorView.k_SourceMaxNits, xrblitParams.srcHdrEncoded ? ((float)xrblitParams.srcHdrMaxLuminance) : 160f);
							}
							bool flag = !xrblitParams.srcTex.sRGB && (xrblitParams.srcTex.graphicsFormat == GraphicsFormat.R8G8B8A8_UNorm || xrblitParams.srcTex.graphicsFormat == GraphicsFormat.B8G8R8A8_UNorm);
							XRMirrorView.s_MirrorViewMaterialProperty.SetFloat(XRMirrorView.k_SRGBRead, flag ? 1f : 0f);
							XRMirrorView.s_MirrorViewMaterialProperty.SetFloat(XRMirrorView.k_SRGBWrite, (QualitySettings.activeColorSpace == ColorSpace.Linear) ? 0f : 1f);
							XRMirrorView.s_MirrorViewMaterialProperty.SetTexture(XRMirrorView.k_SourceTex, xrblitParams.srcTex);
							XRMirrorView.s_MirrorViewMaterialProperty.SetVector(XRMirrorView.k_ScaleBias, vector);
							XRMirrorView.s_MirrorViewMaterialProperty.SetVector(XRMirrorView.k_ScaleBiasRt, value);
							XRMirrorView.s_MirrorViewMaterialProperty.SetFloat(XRMirrorView.k_SourceTexArraySlice, (float)xrblitParams.srcTexArraySlice);
							if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster) && xrblitParams.foveatedRenderingInfo != IntPtr.Zero)
							{
								cmd.ConfigureFoveatedRendering(xrblitParams.foveatedRenderingInfo);
								cmd.EnableShaderKeyword("_FOVEATED_RENDERING_NON_UNIFORM_RASTER");
							}
							if (xrblitParams.srcTex.dimension != TextureDimension.Tex2DArray)
							{
								cmd.EnableShaderKeyword("DISABLE_TEXTURE2D_X_ARRAY");
							}
							cmd.DrawProcedural(Matrix4x4.identity, mat, 0, MeshTopology.Quads, 4, 1, XRMirrorView.s_MirrorViewMaterialProperty);
							if (xrblitParams.srcTex.dimension != TextureDimension.Tex2DArray && TextureXR.useTexArray)
							{
								cmd.DisableShaderKeyword("DISABLE_TEXTURE2D_X_ARRAY");
							}
						}
					}
				}
			}
			if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster))
			{
				cmd.DisableShaderKeyword("_FOVEATED_RENDERING_NON_UNIFORM_RASTER");
				cmd.ConfigureFoveatedRendering(IntPtr.Zero);
			}
		}

		private static readonly MaterialPropertyBlock s_MirrorViewMaterialProperty = new MaterialPropertyBlock();

		private static readonly ProfilingSampler k_MirrorViewProfilingSampler = new ProfilingSampler("XR Mirror View");

		private static readonly int k_SourceTex = Shader.PropertyToID("_SourceTex");

		private static readonly int k_SourceTexArraySlice = Shader.PropertyToID("_SourceTexArraySlice");

		private static readonly int k_ScaleBias = Shader.PropertyToID("_ScaleBias");

		private static readonly int k_ScaleBiasRt = Shader.PropertyToID("_ScaleBiasRt");

		private static readonly int k_SRGBRead = Shader.PropertyToID("_SRGBRead");

		private static readonly int k_SRGBWrite = Shader.PropertyToID("_SRGBWrite");

		private static readonly int k_MaxNits = Shader.PropertyToID("_MaxNits");

		private static readonly int k_SourceMaxNits = Shader.PropertyToID("_SourceMaxNits");

		private static readonly int k_SourceHDREncoding = Shader.PropertyToID("_SourceHDREncoding");

		private static readonly int k_ColorTransform = Shader.PropertyToID("_ColorTransform");
	}
}
