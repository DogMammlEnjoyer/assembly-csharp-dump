using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class ColorGradingLutPass : ScriptableRenderPass
	{
		public ColorGradingLutPass(RenderPassEvent evt, PostProcessData data)
		{
			base.profilingSampler = new ProfilingSampler("Blit Color LUT");
			base.renderPassEvent = evt;
			base.overrideCameraTarget = true;
			this.m_LutBuilderLdr = ColorGradingLutPass.<.ctor>g__Load|7_0(data.shaders.lutBuilderLdrPS);
			this.m_LutBuilderHdr = ColorGradingLutPass.<.ctor>g__Load|7_0(data.shaders.lutBuilderHdrPS);
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormatUsage.Blend))
			{
				this.m_HdrLutFormat = GraphicsFormat.R16G16B16A16_SFloat;
			}
			else if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, GraphicsFormatUsage.Blend))
			{
				this.m_HdrLutFormat = GraphicsFormat.B10G11R11_UFloatPack32;
			}
			else
			{
				this.m_HdrLutFormat = GraphicsFormat.R8G8B8A8_UNorm;
			}
			this.m_LdrLutFormat = GraphicsFormat.R8G8B8A8_UNorm;
			base.useNativeRenderPass = false;
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 && Graphics.minOpenGLESVersion <= OpenGLESVersion.OpenGLES30 && SystemInfo.graphicsDeviceName.StartsWith("Adreno (TM) 3"))
			{
				this.m_AllowColorGradingACESHDR = false;
			}
			this.m_PassData = new ColorGradingLutPass.PassData();
		}

		public void Setup(in RTHandle internalLut)
		{
			this.m_InternalLut = internalLut;
		}

		public void ConfigureDescriptor(in PostProcessingData postProcessingData, out RenderTextureDescriptor descriptor, out FilterMode filterMode)
		{
			PostProcessingData postProcessingData2 = postProcessingData;
			UniversalPostProcessingData universalPostProcessingData = postProcessingData2.universalPostProcessingData;
			this.ConfigureDescriptor(universalPostProcessingData, out descriptor, out filterMode);
		}

		public void ConfigureDescriptor(in UniversalPostProcessingData postProcessingData, out RenderTextureDescriptor descriptor, out FilterMode filterMode)
		{
			bool flag = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;
			int lutSize = postProcessingData.lutSize;
			int width = lutSize * lutSize;
			GraphicsFormat colorFormat = flag ? this.m_HdrLutFormat : this.m_LdrLutFormat;
			descriptor = new RenderTextureDescriptor(width, lutSize, colorFormat, 0);
			descriptor.vrUsage = VRTextureUsage.None;
			filterMode = FilterMode.Bilinear;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalPostProcessingData postProcessingData = frameData.Get<UniversalPostProcessingData>();
			this.m_PassData.cameraData = cameraData;
			this.m_PassData.postProcessingData = postProcessingData;
			this.m_PassData.lutBuilderLdr = this.m_LutBuilderLdr;
			this.m_PassData.lutBuilderHdr = this.m_LutBuilderHdr;
			this.m_PassData.allowColorGradingACESHDR = this.m_AllowColorGradingACESHDR;
			if (renderingData.cameraData.xr.supportsFoveatedRendering)
			{
				renderingData.commandBuffer->SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
			}
			CoreUtils.SetRenderTarget(*renderingData.commandBuffer, this.m_InternalLut, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
			ColorGradingLutPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, this.m_InternalLut);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, ColorGradingLutPass.PassData passData, RTHandle internalLutTarget)
		{
			Material lutBuilderLdr = passData.lutBuilderLdr;
			Material lutBuilderHdr = passData.lutBuilderHdr;
			bool allowColorGradingACESHDR = passData.allowColorGradingACESHDR;
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.ColorGradingLUT)))
			{
				VolumeStack stack = VolumeManager.instance.stack;
				ChannelMixer component = stack.GetComponent<ChannelMixer>();
				ColorAdjustments component2 = stack.GetComponent<ColorAdjustments>();
				ColorCurves component3 = stack.GetComponent<ColorCurves>();
				LiftGammaGain component4 = stack.GetComponent<LiftGammaGain>();
				ShadowsMidtonesHighlights component5 = stack.GetComponent<ShadowsMidtonesHighlights>();
				SplitToning component6 = stack.GetComponent<SplitToning>();
				Tonemapping component7 = stack.GetComponent<Tonemapping>();
				WhiteBalance component8 = stack.GetComponent<WhiteBalance>();
				bool flag = passData.postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;
				Material material = flag ? lutBuilderHdr : lutBuilderLdr;
				Vector3 v = ColorUtils.ColorBalanceToLMSCoeffs(component8.temperature.value, component8.tint.value);
				Vector4 value = new Vector4(component2.hueShift.value / 360f, component2.saturation.value / 100f + 1f, component2.contrast.value / 100f + 1f, 0f);
				Vector4 value2 = new Vector4(component.redOutRedIn.value / 100f, component.redOutGreenIn.value / 100f, component.redOutBlueIn.value / 100f, 0f);
				Vector4 value3 = new Vector4(component.greenOutRedIn.value / 100f, component.greenOutGreenIn.value / 100f, component.greenOutBlueIn.value / 100f, 0f);
				Vector4 value4 = new Vector4(component.blueOutRedIn.value / 100f, component.blueOutGreenIn.value / 100f, component.blueOutBlueIn.value / 100f, 0f);
				Vector4 value5 = new Vector4(component5.shadowsStart.value, component5.shadowsEnd.value, component5.highlightsStart.value, component5.highlightsEnd.value);
				Vector4 vector = component5.shadows.value;
				Vector4 vector2 = component5.midtones.value;
				Vector4 value6 = component5.highlights.value;
				ValueTuple<Vector4, Vector4, Vector4> valueTuple = ColorUtils.PrepareShadowsMidtonesHighlights(vector, vector2, value6);
				Vector4 item = valueTuple.Item1;
				Vector4 item2 = valueTuple.Item2;
				Vector4 item3 = valueTuple.Item3;
				vector = component4.lift.value;
				vector2 = component4.gamma.value;
				value6 = component4.gain.value;
				ValueTuple<Vector4, Vector4, Vector4> valueTuple2 = ColorUtils.PrepareLiftGammaGain(vector, vector2, value6);
				Vector4 item4 = valueTuple2.Item1;
				Vector4 item5 = valueTuple2.Item2;
				Vector4 item6 = valueTuple2.Item3;
				vector = component6.shadows.value;
				vector2 = component6.highlights.value;
				ValueTuple<Vector4, Vector4> valueTuple3 = ColorUtils.PrepareSplitToning(vector, vector2, component6.balance.value);
				Vector4 item7 = valueTuple3.Item1;
				Vector4 item8 = valueTuple3.Item2;
				int lutSize = passData.postProcessingData.lutSize;
				int num = lutSize * lutSize;
				Vector4 value7 = new Vector4((float)lutSize, 0.5f / (float)num, 0.5f / (float)lutSize, (float)lutSize / ((float)lutSize - 1f));
				material.SetVector(ColorGradingLutPass.ShaderConstants._Lut_Params, value7);
				material.SetVector(ColorGradingLutPass.ShaderConstants._ColorBalance, v);
				material.SetVector(ColorGradingLutPass.ShaderConstants._ColorFilter, component2.colorFilter.value.linear);
				material.SetVector(ColorGradingLutPass.ShaderConstants._ChannelMixerRed, value2);
				material.SetVector(ColorGradingLutPass.ShaderConstants._ChannelMixerGreen, value3);
				material.SetVector(ColorGradingLutPass.ShaderConstants._ChannelMixerBlue, value4);
				material.SetVector(ColorGradingLutPass.ShaderConstants._HueSatCon, value);
				material.SetVector(ColorGradingLutPass.ShaderConstants._Lift, item4);
				material.SetVector(ColorGradingLutPass.ShaderConstants._Gamma, item5);
				material.SetVector(ColorGradingLutPass.ShaderConstants._Gain, item6);
				material.SetVector(ColorGradingLutPass.ShaderConstants._Shadows, item);
				material.SetVector(ColorGradingLutPass.ShaderConstants._Midtones, item2);
				material.SetVector(ColorGradingLutPass.ShaderConstants._Highlights, item3);
				material.SetVector(ColorGradingLutPass.ShaderConstants._ShaHiLimits, value5);
				material.SetVector(ColorGradingLutPass.ShaderConstants._SplitShadows, item7);
				material.SetVector(ColorGradingLutPass.ShaderConstants._SplitHighlights, item8);
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveMaster, component3.master.value.GetTexture());
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveRed, component3.red.value.GetTexture());
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveGreen, component3.green.value.GetTexture());
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveBlue, component3.blue.value.GetTexture());
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveHueVsHue, component3.hueVsHue.value.GetTexture());
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveHueVsSat, component3.hueVsSat.value.GetTexture());
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveLumVsSat, component3.lumVsSat.value.GetTexture());
				material.SetTexture(ColorGradingLutPass.ShaderConstants._CurveSatVsSat, component3.satVsSat.value.GetTexture());
				if (flag)
				{
					material.shaderKeywords = null;
					TonemappingMode value8 = component7.mode.value;
					if (value8 != TonemappingMode.Neutral)
					{
						if (value8 == TonemappingMode.ACES)
						{
							material.EnableKeyword(allowColorGradingACESHDR ? "_TONEMAP_ACES" : "_TONEMAP_NEUTRAL");
						}
					}
					else
					{
						material.EnableKeyword("_TONEMAP_NEUTRAL");
					}
					if (passData.cameraData.isHDROutputActive)
					{
						Vector4 value9;
						UniversalRenderPipeline.GetHDROutputLuminanceParameters(passData.cameraData.hdrDisplayInformation, passData.cameraData.hdrDisplayColorGamut, component7, out value9);
						Vector4 value10;
						UniversalRenderPipeline.GetHDROutputGradingParameters(component7, out value10);
						material.SetVector(ShaderPropertyId.hdrOutputLuminanceParams, value9);
						material.SetVector(ShaderPropertyId.hdrOutputGradingParams, value10);
						HDROutputUtils.ConfigureHDROutput(material, passData.cameraData.hdrDisplayColorGamut, HDROutputUtils.Operation.ColorConversion);
					}
				}
				passData.cameraData.xr.StopSinglePass(cmd);
				Blitter.BlitTexture(cmd, internalLutTarget, Vector2.one, material, 0);
				passData.cameraData.xr.StartSinglePass(cmd);
			}
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, out TextureHandle internalColorLut)
		{
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalPostProcessingData postProcessingData = frameData.Get<UniversalPostProcessingData>();
			ColorGradingLutPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<ColorGradingLutPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\ColorGradingLutPass.cs", 283))
			{
				RenderTextureDescriptor desc;
				FilterMode filterMode;
				this.ConfigureDescriptor(postProcessingData, out desc, out filterMode);
				internalColorLut = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_InternalGradingLut", true, filterMode, TextureWrapMode.Clamp);
				passData.cameraData = cameraData;
				passData.postProcessingData = postProcessingData;
				passData.internalLut = internalColorLut;
				rasterRenderGraphBuilder.SetRenderAttachment(internalColorLut, 0, AccessFlags.WriteAll);
				passData.lutBuilderLdr = this.m_LutBuilderLdr;
				passData.lutBuilderHdr = this.m_LutBuilderHdr;
				passData.allowColorGradingACESHDR = this.m_AllowColorGradingACESHDR;
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<ColorGradingLutPass.PassData>(delegate(ColorGradingLutPass.PassData data, RasterGraphContext context)
				{
					ColorGradingLutPass.ExecutePass(context.cmd, data, data.internalLut);
				});
			}
		}

		public void Cleanup()
		{
			CoreUtils.Destroy(this.m_LutBuilderLdr);
			CoreUtils.Destroy(this.m_LutBuilderHdr);
		}

		[CompilerGenerated]
		internal static Material <.ctor>g__Load|7_0(Shader shader)
		{
			if (shader == null)
			{
				Debug.LogError("Missing shader. ColorGradingLutPass render pass will not execute. Check for missing reference in the renderer resources.");
				return null;
			}
			return CoreUtils.CreateEngineMaterial(shader);
		}

		private readonly Material m_LutBuilderLdr;

		private readonly Material m_LutBuilderHdr;

		internal readonly GraphicsFormat m_HdrLutFormat;

		internal readonly GraphicsFormat m_LdrLutFormat;

		private ColorGradingLutPass.PassData m_PassData;

		private RTHandle m_InternalLut;

		private bool m_AllowColorGradingACESHDR = true;

		private class PassData
		{
			internal UniversalCameraData cameraData;

			internal UniversalPostProcessingData postProcessingData;

			internal Material lutBuilderLdr;

			internal Material lutBuilderHdr;

			internal bool allowColorGradingACESHDR;

			internal TextureHandle internalLut;
		}

		private static class ShaderConstants
		{
			public static readonly int _Lut_Params = Shader.PropertyToID("_Lut_Params");

			public static readonly int _ColorBalance = Shader.PropertyToID("_ColorBalance");

			public static readonly int _ColorFilter = Shader.PropertyToID("_ColorFilter");

			public static readonly int _ChannelMixerRed = Shader.PropertyToID("_ChannelMixerRed");

			public static readonly int _ChannelMixerGreen = Shader.PropertyToID("_ChannelMixerGreen");

			public static readonly int _ChannelMixerBlue = Shader.PropertyToID("_ChannelMixerBlue");

			public static readonly int _HueSatCon = Shader.PropertyToID("_HueSatCon");

			public static readonly int _Lift = Shader.PropertyToID("_Lift");

			public static readonly int _Gamma = Shader.PropertyToID("_Gamma");

			public static readonly int _Gain = Shader.PropertyToID("_Gain");

			public static readonly int _Shadows = Shader.PropertyToID("_Shadows");

			public static readonly int _Midtones = Shader.PropertyToID("_Midtones");

			public static readonly int _Highlights = Shader.PropertyToID("_Highlights");

			public static readonly int _ShaHiLimits = Shader.PropertyToID("_ShaHiLimits");

			public static readonly int _SplitShadows = Shader.PropertyToID("_SplitShadows");

			public static readonly int _SplitHighlights = Shader.PropertyToID("_SplitHighlights");

			public static readonly int _CurveMaster = Shader.PropertyToID("_CurveMaster");

			public static readonly int _CurveRed = Shader.PropertyToID("_CurveRed");

			public static readonly int _CurveGreen = Shader.PropertyToID("_CurveGreen");

			public static readonly int _CurveBlue = Shader.PropertyToID("_CurveBlue");

			public static readonly int _CurveHueVsHue = Shader.PropertyToID("_CurveHueVsHue");

			public static readonly int _CurveHueVsSat = Shader.PropertyToID("_CurveHueVsSat");

			public static readonly int _CurveLumVsSat = Shader.PropertyToID("_CurveLumVsSat");

			public static readonly int _CurveSatVsSat = Shader.PropertyToID("_CurveSatVsSat");
		}
	}
}
