using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Categorization;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	public static class STP
	{
		public static bool IsSupported()
		{
			return true & SystemInfo.supportsComputeShaders & SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3;
		}

		public static Vector2 Jit16(int frameIndex)
		{
			Vector2 result;
			result.x = HaltonSequence.Get(frameIndex, 2) - 0.5f;
			result.y = HaltonSequence.Get(frameIndex, 3) - 0.5f;
			return result;
		}

		public static GUIContent[] debugViewDescriptions
		{
			get
			{
				return STP.s_DebugViewDescriptions;
			}
		}

		public static int[] debugViewIndices
		{
			get
			{
				return STP.s_DebugViewIndices;
			}
		}

		public static STP.PerViewConfig[] perViewConfigs
		{
			get
			{
				return STP.s_PerViewConfigs;
			}
			set
			{
				STP.s_PerViewConfigs = value;
			}
		}

		private static Hash128 ComputeHistoryHash(ref STP.HistoryUpdateInfo info)
		{
			Hash128 result = default(Hash128);
			result.Append<bool>(ref info.useHwDrs);
			result.Append<bool>(ref info.useTexArray);
			result.Append<Vector2Int>(ref info.postUpscaleSize);
			if (!info.useHwDrs)
			{
				result.Append<Vector2Int>(ref info.preUpscaleSize);
			}
			return result;
		}

		private static Vector2Int CalculateConvergenceTextureSize(Vector2Int historyTextureSize)
		{
			return new Vector2Int(CoreUtils.DivRoundUp(historyTextureSize.x, 4), CoreUtils.DivRoundUp(historyTextureSize.y, 4));
		}

		private static float CalculateMotionScale(float deltaTime, float lastDeltaTime)
		{
			float result = 1f;
			if (!Mathf.Approximately(lastDeltaTime, 0f))
			{
				result = deltaTime / lastDeltaTime;
			}
			return result;
		}

		private static Matrix4x4 ExtractRotation(Matrix4x4 input)
		{
			Matrix4x4 result = input;
			result[0, 3] = 0f;
			result[1, 3] = 0f;
			result[2, 3] = 0f;
			result[3, 3] = 1f;
			return result;
		}

		private static int PackVector2ToInt(Vector2 value)
		{
			int num = (int)Mathf.FloatToHalf(value.x);
			uint num2 = (uint)Mathf.FloatToHalf(value.y);
			return num | (int)((int)num2 << 16);
		}

		private unsafe static void PopulateConstantData(ref STP.Config config, ref STP.StpConstantBufferData constants)
		{
			int num = config.noiseTexture.width - 1 & 255;
			int num2 = (config.hasValidHistory ? 1 : 0) << 8;
			int num3 = (config.stencilMask & 255) << 16;
			int num4 = (config.debugViewIndex & 255) << 24;
			int value = num3 | num2 | num | num4;
			float y = (config.farPlane - config.nearPlane) / (config.nearPlane * config.farPlane);
			float z = 1f / config.farPlane;
			constants._StpCommonConstant = new Vector4(BitConverter.Int32BitsToSingle(value), y, z, 0f);
			constants._StpSetupConstants0.x = 1f / (float)config.currentImageSize.x;
			constants._StpSetupConstants0.y = 1f / (float)config.currentImageSize.y;
			constants._StpSetupConstants0.z = 0.5f / (float)config.currentImageSize.x;
			constants._StpSetupConstants0.w = 0.5f / (float)config.currentImageSize.y;
			Vector2 vector = STP.Jit16(config.frameIndex - 1);
			Vector2 vector2 = STP.Jit16(config.frameIndex);
			constants._StpSetupConstants1.x = vector2.x / (float)config.currentImageSize.x - vector.x / (float)config.priorImageSize.x;
			constants._StpSetupConstants1.y = vector2.y / (float)config.currentImageSize.y - vector.y / (float)config.priorImageSize.y;
			constants._StpSetupConstants1.z = vector2.x / (float)config.currentImageSize.x;
			constants._StpSetupConstants1.w = vector2.y / (float)config.currentImageSize.y;
			constants._StpSetupConstants2.x = (float)config.outputImageSize.x;
			constants._StpSetupConstants2.y = (float)config.outputImageSize.y;
			float num5 = 1f / config.nearPlane;
			float w = 1f / Mathf.Log(num5 * config.farPlane, 2f);
			constants._StpSetupConstants2.z = num5;
			constants._StpSetupConstants2.w = w;
			Vector2 vector3;
			vector3.x = 2f;
			vector3.y = 2f;
			vector3.x *= (float)config.priorImageSize.x / ((float)config.priorImageSize.x + 4f);
			vector3.y *= (float)config.priorImageSize.y / ((float)config.priorImageSize.y + 4f);
			constants._StpSetupConstants3.x = vector3[0];
			constants._StpSetupConstants3.y = vector3[1];
			constants._StpSetupConstants3.z = -0.5f * vector3[0];
			constants._StpSetupConstants3.w = -0.5f * vector3[1];
			constants._StpSetupConstants4.x = Mathf.Log(config.farPlane / config.nearPlane, 2f);
			constants._StpSetupConstants4.y = config.nearPlane;
			constants._StpSetupConstants4.z = (config.enableMotionScaling ? STP.CalculateMotionScale(config.deltaTime, config.lastDeltaTime) : 1f);
			constants._StpSetupConstants4.w = 0f;
			constants._StpSetupConstants5.x = (float)config.currentImageSize.x;
			constants._StpSetupConstants5.y = (float)config.currentImageSize.y;
			constants._StpSetupConstants5.z = (float)config.outputImageSize.x / (Mathf.Ceil((float)config.outputImageSize.x / 4f) * 4f);
			constants._StpSetupConstants5.w = (float)config.outputImageSize.y / (Mathf.Ceil((float)config.outputImageSize.y / 4f) * 4f);
			uint num6 = 0U;
			while ((ulong)num6 < (ulong)((long)config.numActiveViews))
			{
				uint num7 = num6 * 8U * 4U;
				STP.PerViewConfig perViewConfig = config.perViewConfigs[(int)num6];
				Vector4 vector4;
				vector4.x = perViewConfig.lastProj[0, 0];
				vector4.y = Mathf.Abs(perViewConfig.lastProj[1, 1]);
				vector4.z = -perViewConfig.lastProj[0, 2];
				vector4.w = -perViewConfig.lastProj[1, 2];
				Vector4 vector5;
				vector5.x = perViewConfig.lastProj[2, 2];
				vector5.y = perViewConfig.lastProj[2, 3];
				vector5.z = perViewConfig.lastProj[3, 2];
				vector5.w = perViewConfig.lastProj[3, 3];
				Vector4 vector6;
				vector6.x = perViewConfig.currentProj[0, 0];
				vector6.y = Mathf.Abs(perViewConfig.currentProj[1, 1]);
				vector6.z = perViewConfig.currentProj[0, 2];
				vector6.w = perViewConfig.currentProj[1, 2];
				Vector4 vector7;
				vector7.x = perViewConfig.currentProj[2, 2];
				vector7.y = perViewConfig.currentProj[2, 3];
				vector7.z = perViewConfig.currentProj[3, 2];
				vector7.w = perViewConfig.currentProj[3, 3];
				Matrix4x4 matrix4x = STP.ExtractRotation(perViewConfig.currentView) * Matrix4x4.Translate(-perViewConfig.currentView.GetColumn(3)) * Matrix4x4.Translate(perViewConfig.lastView.GetColumn(3)) * STP.ExtractRotation(perViewConfig.lastView).transpose;
				Vector4 row = matrix4x.GetRow(0);
				Vector4 row2 = matrix4x.GetRow(1);
				Vector4 row3 = matrix4x.GetRow(2);
				Vector4 vector8;
				vector8.x = perViewConfig.lastLastProj[0, 0];
				vector8.y = Mathf.Abs(perViewConfig.lastLastProj[1, 1]);
				vector8.z = perViewConfig.lastLastProj[0, 2];
				vector8.w = perViewConfig.lastLastProj[1, 2];
				Vector4 vector9;
				vector9.x = perViewConfig.lastLastProj[2, 2];
				vector9.y = perViewConfig.lastLastProj[2, 3];
				vector9.z = perViewConfig.lastLastProj[3, 2];
				vector9.w = perViewConfig.lastLastProj[3, 3];
				Matrix4x4 matrix4x2 = STP.ExtractRotation(perViewConfig.lastLastView) * Matrix4x4.Translate(-perViewConfig.lastLastView.GetColumn(3)) * Matrix4x4.Translate(perViewConfig.lastView.GetColumn(3)) * STP.ExtractRotation(perViewConfig.lastView).transpose;
				Vector4 row4 = matrix4x2.GetRow(0);
				Vector4 row5 = matrix4x2.GetRow(1);
				Vector4 row6 = matrix4x2.GetRow(2);
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)num7 * 4UL)) = vector5.z / vector4.x;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 1U) * 4UL)) = vector5.w / vector4.x;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 2U) * 4UL)) = vector4.z / vector4.x;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 3U) * 4UL)) = vector5.z / vector4.y;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 4U) * 4UL)) = vector5.w / vector4.y;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 5U) * 4UL)) = vector4.w / vector4.y;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 6U) * 4UL)) = row.x * vector6.x + row3.x * vector6.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 7U) * 4UL)) = row.y * vector6.x + row3.y * vector6.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 8U) * 4UL)) = row.z * vector6.x + row3.z * vector6.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 9U) * 4UL)) = row.w * vector6.x + row3.w * vector6.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 10U) * 4UL)) = row2.x * vector6.y + row3.x * vector6.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 11U) * 4UL)) = row2.y * vector6.y + row3.y * vector6.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 12U) * 4UL)) = row2.z * vector6.y + row3.z * vector6.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 13U) * 4UL)) = row2.w * vector6.y + row3.w * vector6.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 14U) * 4UL)) = row3.x * vector7.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 15U) * 4UL)) = row3.y * vector7.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 16U) * 4UL)) = row3.z * vector7.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 17U) * 4UL)) = row3.w * vector7.z + vector7.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 18U) * 4UL)) = row4.x * vector8.x + row6.x * vector8.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 19U) * 4UL)) = row4.y * vector8.x + row6.y * vector8.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 20U) * 4UL)) = row4.z * vector8.x + row6.z * vector8.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 21U) * 4UL)) = row4.w * vector8.x + row6.w * vector8.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 22U) * 4UL)) = row5.x * vector8.y + row6.x * vector8.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 23U) * 4UL)) = row5.y * vector8.y + row6.y * vector8.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 24U) * 4UL)) = row5.z * vector8.y + row6.z * vector8.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 25U) * 4UL)) = row5.w * vector8.y + row6.w * vector8.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 26U) * 4UL)) = row6.x * vector9.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 27U) * 4UL)) = row6.y * vector9.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 28U) * 4UL)) = row6.z * vector9.z;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 29U) * 4UL)) = row6.w * vector9.z + vector9.w;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 30U) * 4UL)) = 0f;
				*(ref constants._StpSetupPerViewConstants.FixedElementField + (IntPtr)((ulong)(num7 + 31U) * 4UL)) = 0f;
				num6 += 1U;
			}
			constants._StpDilConstants0.x = 4f / (float)config.currentImageSize.x;
			constants._StpDilConstants0.y = 4f / (float)config.currentImageSize.y;
			constants._StpDilConstants0.z = BitConverter.Int32BitsToSingle(config.currentImageSize.x >> 2);
			constants._StpDilConstants0.w = BitConverter.Int32BitsToSingle(config.currentImageSize.y >> 2);
			constants._StpTaaConstants0.x = (float)config.currentImageSize.x / (float)config.outputImageSize.x;
			constants._StpTaaConstants0.y = (float)config.currentImageSize.y / (float)config.outputImageSize.y;
			constants._StpTaaConstants0.z = 0.5f * (float)config.currentImageSize.x / (float)config.outputImageSize.x - vector2.x;
			constants._StpTaaConstants0.w = 0.5f * (float)config.currentImageSize.y / (float)config.outputImageSize.y - vector2.y;
			constants._StpTaaConstants1.x = 1f / (float)config.currentImageSize.x;
			constants._StpTaaConstants1.y = 1f / (float)config.currentImageSize.y;
			constants._StpTaaConstants1.z = 1f / (float)config.outputImageSize.x;
			constants._StpTaaConstants1.w = 1f / (float)config.outputImageSize.y;
			constants._StpTaaConstants2.x = 0.5f / (float)config.outputImageSize.x;
			constants._StpTaaConstants2.y = 0.5f / (float)config.outputImageSize.y;
			constants._StpTaaConstants2.z = vector2.x / (float)config.currentImageSize.x - 0.5f / (float)config.currentImageSize.x;
			constants._StpTaaConstants2.w = vector2.y / (float)config.currentImageSize.y + 0.5f / (float)config.currentImageSize.y;
			constants._StpTaaConstants3.x = 0.5f / (float)config.currentImageSize.x;
			constants._StpTaaConstants3.y = 0.5f / (float)config.currentImageSize.y;
			constants._StpTaaConstants3.z = (float)config.outputImageSize.x;
			constants._StpTaaConstants3.w = (float)config.outputImageSize.y;
		}

		private static TextureHandle UseTexture(IBaseRenderGraphBuilder builder, TextureHandle texture, AccessFlags flags = AccessFlags.Read)
		{
			builder.UseTexture(texture, flags);
			return texture;
		}

		public static TextureHandle Execute(RenderGraph renderGraph, ref STP.Config config)
		{
			STP.RuntimeResources renderPipelineSettings = GraphicsSettings.GetRenderPipelineSettings<STP.RuntimeResources>();
			Texture2D noiseTexture = config.noiseTexture;
			RTHandleStaticHelpers.SetRTHandleStaticWrapper(noiseTexture);
			RTHandle s_RTHandleWrapper = RTHandleStaticHelpers.s_RTHandleWrapper;
			RenderTargetInfo info;
			info.width = noiseTexture.width;
			info.height = noiseTexture.height;
			info.volumeDepth = 1;
			info.msaaSamples = 1;
			info.format = noiseTexture.graphicsFormat;
			info.bindMS = false;
			TextureHandle texture = renderGraph.ImportTexture(s_RTHandleWrapper, info, default(ImportResourceParams));
			RTHandle previousHistoryTexture = config.historyContext.GetPreviousHistoryTexture(STP.HistoryTextureType.DepthMotion, config.frameIndex);
			RTHandle previousHistoryTexture2 = config.historyContext.GetPreviousHistoryTexture(STP.HistoryTextureType.Luma, config.frameIndex);
			RTHandle previousHistoryTexture3 = config.historyContext.GetPreviousHistoryTexture(STP.HistoryTextureType.Convergence, config.frameIndex);
			RTHandle previousHistoryTexture4 = config.historyContext.GetPreviousHistoryTexture(STP.HistoryTextureType.Feedback, config.frameIndex);
			RTHandle currentHistoryTexture = config.historyContext.GetCurrentHistoryTexture(STP.HistoryTextureType.DepthMotion, config.frameIndex);
			RTHandle currentHistoryTexture2 = config.historyContext.GetCurrentHistoryTexture(STP.HistoryTextureType.Luma, config.frameIndex);
			RTHandle currentHistoryTexture3 = config.historyContext.GetCurrentHistoryTexture(STP.HistoryTextureType.Convergence, config.frameIndex);
			RTHandle currentHistoryTexture4 = config.historyContext.GetCurrentHistoryTexture(STP.HistoryTextureType.Feedback, config.frameIndex);
			if (config.enableHwDrs)
			{
				currentHistoryTexture.rt.ApplyDynamicScale();
				currentHistoryTexture2.rt.ApplyDynamicScale();
				currentHistoryTexture3.rt.ApplyDynamicScale();
			}
			Vector2Int historyTextureSize = config.enableHwDrs ? config.outputImageSize : config.currentImageSize;
			bool flag = SystemInfo.graphicsDeviceVendorID == STP.kQualcommVendorId;
			Vector2Int vector2Int = new Vector2Int(8, flag ? 16 : 8);
			STP.SetupData setupData;
			STP.SetupData setupData4;
			using (IComputeRenderGraphBuilder computeRenderGraphBuilder = renderGraph.AddComputePass<STP.SetupData>("STP Setup", out setupData, ProfilingSampler.Get<STP.ProfileId>(STP.ProfileId.StpSetup), ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\STP\\STP.cs", 1109))
			{
				setupData.cs = renderPipelineSettings.setupCS;
				setupData.cs.shaderKeywords = null;
				if (flag)
				{
					setupData.cs.EnableKeyword(STP.ShaderKeywords.EnableLargeKernel);
				}
				if (!config.enableTexArray)
				{
					setupData.cs.EnableKeyword(STP.ShaderKeywords.DisableTexture2DXArray);
				}
				STP.PopulateConstantData(ref config, ref setupData.constantBufferData);
				setupData.noiseTexture = STP.UseTexture(computeRenderGraphBuilder, texture, AccessFlags.Read);
				if (config.debugView.IsValid())
				{
					setupData.cs.EnableKeyword(STP.ShaderKeywords.EnableDebugMode);
					setupData.debugView = STP.UseTexture(computeRenderGraphBuilder, config.debugView, AccessFlags.WriteAll);
				}
				setupData.kernelIndex = setupData.cs.FindKernel("StpSetup");
				setupData.viewCount = config.numActiveViews;
				setupData.dispatchSize = new Vector2Int(CoreUtils.DivRoundUp(config.currentImageSize.x, vector2Int.x), CoreUtils.DivRoundUp(config.currentImageSize.y, vector2Int.y));
				setupData.inputColor = STP.UseTexture(computeRenderGraphBuilder, config.inputColor, AccessFlags.Read);
				setupData.inputDepth = STP.UseTexture(computeRenderGraphBuilder, config.inputDepth, AccessFlags.Read);
				setupData.inputMotion = STP.UseTexture(computeRenderGraphBuilder, config.inputMotion, AccessFlags.Read);
				if (config.inputStencil.IsValid())
				{
					setupData.cs.EnableKeyword(STP.ShaderKeywords.EnableStencilResponsive);
					setupData.inputStencil = STP.UseTexture(computeRenderGraphBuilder, config.inputStencil, AccessFlags.Read);
				}
				STP.SetupData setupData2 = setupData;
				IBaseRenderGraphBuilder builder = computeRenderGraphBuilder;
				TextureDesc textureDesc = new TextureDesc(historyTextureSize.x, historyTextureSize.y, config.enableHwDrs, config.enableTexArray);
				textureDesc.name = "STP Intermediate Color";
				textureDesc.format = GraphicsFormat.A2B10G10R10_UNormPack32;
				textureDesc.enableRandomWrite = true;
				setupData2.intermediateColor = STP.UseTexture(builder, renderGraph.CreateTexture(textureDesc), AccessFlags.WriteAll);
				Vector2Int vector2Int2 = STP.CalculateConvergenceTextureSize(historyTextureSize);
				STP.SetupData setupData3 = setupData;
				IBaseRenderGraphBuilder builder2 = computeRenderGraphBuilder;
				textureDesc = new TextureDesc(vector2Int2.x, vector2Int2.y, config.enableHwDrs, config.enableTexArray);
				textureDesc.name = "STP Intermediate Convergence";
				textureDesc.format = GraphicsFormat.R8_UNorm;
				textureDesc.enableRandomWrite = true;
				setupData3.intermediateConvergence = STP.UseTexture(builder2, renderGraph.CreateTexture(textureDesc), AccessFlags.WriteAll);
				setupData.priorDepthMotion = STP.UseTexture(computeRenderGraphBuilder, renderGraph.ImportTexture(previousHistoryTexture), AccessFlags.Read);
				setupData.depthMotion = STP.UseTexture(computeRenderGraphBuilder, renderGraph.ImportTexture(currentHistoryTexture), AccessFlags.WriteAll);
				setupData.priorLuma = STP.UseTexture(computeRenderGraphBuilder, renderGraph.ImportTexture(previousHistoryTexture2), AccessFlags.Read);
				setupData.luma = STP.UseTexture(computeRenderGraphBuilder, renderGraph.ImportTexture(currentHistoryTexture2), AccessFlags.WriteAll);
				setupData.priorFeedback = STP.UseTexture(computeRenderGraphBuilder, renderGraph.ImportTexture(previousHistoryTexture4), AccessFlags.Read);
				setupData.priorConvergence = STP.UseTexture(computeRenderGraphBuilder, renderGraph.ImportTexture(previousHistoryTexture3), AccessFlags.Read);
				computeRenderGraphBuilder.SetRenderFunc<STP.SetupData>(delegate(STP.SetupData data, ComputeGraphContext ctx)
				{
					ConstantBuffer.UpdateData<STP.StpConstantBufferData>(ctx.cmd.m_WrappedCommandBuffer, data.constantBufferData);
					ConstantBuffer.Set<STP.StpConstantBufferData>(data.cs, STP.ShaderResources._StpConstantBufferData);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpBlueNoiseIn, data.noiseTexture);
					if (data.debugView.IsValid())
					{
						ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpDebugOut, data.debugView);
					}
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpInputColor, data.inputColor);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpInputDepth, data.inputDepth);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpInputMotion, data.inputMotion);
					if (data.inputStencil.IsValid())
					{
						ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpInputStencil, data.inputStencil, 0, RenderTextureSubElement.Stencil);
					}
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpIntermediateColor, data.intermediateColor);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpIntermediateConvergence, data.intermediateConvergence);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpPriorDepthMotion, data.priorDepthMotion);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpDepthMotion, data.depthMotion);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpPriorLuma, data.priorLuma);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpLuma, data.luma);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpPriorFeedback, data.priorFeedback);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpPriorConvergence, data.priorConvergence);
					ctx.cmd.DispatchCompute(data.cs, data.kernelIndex, data.dispatchSize.x, data.dispatchSize.y, data.viewCount);
				});
				setupData4 = setupData;
			}
			STP.PreTaaData preTaaData;
			STP.PreTaaData preTaaData3;
			using (IComputeRenderGraphBuilder computeRenderGraphBuilder2 = renderGraph.AddComputePass<STP.PreTaaData>("STP Pre-TAA", out preTaaData, ProfilingSampler.Get<STP.ProfileId>(STP.ProfileId.StpPreTaa), ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\STP\\STP.cs", 1212))
			{
				preTaaData.cs = renderPipelineSettings.preTaaCS;
				preTaaData.cs.shaderKeywords = null;
				if (flag)
				{
					preTaaData.cs.EnableKeyword(STP.ShaderKeywords.EnableLargeKernel);
				}
				if (!config.enableTexArray)
				{
					preTaaData.cs.EnableKeyword(STP.ShaderKeywords.DisableTexture2DXArray);
				}
				preTaaData.noiseTexture = STP.UseTexture(computeRenderGraphBuilder2, texture, AccessFlags.Read);
				if (config.debugView.IsValid())
				{
					preTaaData.cs.EnableKeyword(STP.ShaderKeywords.EnableDebugMode);
					preTaaData.debugView = STP.UseTexture(computeRenderGraphBuilder2, config.debugView, AccessFlags.ReadWrite);
				}
				preTaaData.kernelIndex = preTaaData.cs.FindKernel("StpPreTaa");
				preTaaData.viewCount = config.numActiveViews;
				preTaaData.dispatchSize = new Vector2Int(CoreUtils.DivRoundUp(config.currentImageSize.x, vector2Int.x), CoreUtils.DivRoundUp(config.currentImageSize.y, vector2Int.y));
				preTaaData.intermediateConvergence = STP.UseTexture(computeRenderGraphBuilder2, setupData4.intermediateConvergence, AccessFlags.Read);
				STP.PreTaaData preTaaData2 = preTaaData;
				IBaseRenderGraphBuilder builder3 = computeRenderGraphBuilder2;
				TextureDesc textureDesc = new TextureDesc(historyTextureSize.x, historyTextureSize.y, config.enableHwDrs, config.enableTexArray);
				textureDesc.name = "STP Intermediate Weights";
				textureDesc.format = GraphicsFormat.R8_UNorm;
				textureDesc.enableRandomWrite = true;
				preTaaData2.intermediateWeights = STP.UseTexture(builder3, renderGraph.CreateTexture(textureDesc), AccessFlags.WriteAll);
				preTaaData.luma = STP.UseTexture(computeRenderGraphBuilder2, renderGraph.ImportTexture(currentHistoryTexture2), AccessFlags.Read);
				preTaaData.convergence = STP.UseTexture(computeRenderGraphBuilder2, renderGraph.ImportTexture(currentHistoryTexture3), AccessFlags.WriteAll);
				computeRenderGraphBuilder2.SetRenderFunc<STP.PreTaaData>(delegate(STP.PreTaaData data, ComputeGraphContext ctx)
				{
					ConstantBuffer.Set<STP.StpConstantBufferData>(data.cs, STP.ShaderResources._StpConstantBufferData);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpBlueNoiseIn, data.noiseTexture);
					if (data.debugView.IsValid())
					{
						ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpDebugOut, data.debugView);
					}
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpIntermediateConvergence, data.intermediateConvergence);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpIntermediateWeights, data.intermediateWeights);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpLuma, data.luma);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpConvergence, data.convergence);
					ctx.cmd.DispatchCompute(data.cs, data.kernelIndex, data.dispatchSize.x, data.dispatchSize.y, data.viewCount);
				});
				preTaaData3 = preTaaData;
			}
			STP.TaaData taaData;
			STP.TaaData taaData2;
			using (IComputeRenderGraphBuilder computeRenderGraphBuilder3 = renderGraph.AddComputePass<STP.TaaData>("STP TAA", out taaData, ProfilingSampler.Get<STP.ProfileId>(STP.ProfileId.StpTaa), ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\STP\\STP.cs", 1275))
			{
				taaData.cs = renderPipelineSettings.taaCS;
				taaData.cs.shaderKeywords = null;
				if (flag)
				{
					taaData.cs.EnableKeyword(STP.ShaderKeywords.EnableLargeKernel);
				}
				if (!config.enableTexArray)
				{
					taaData.cs.EnableKeyword(STP.ShaderKeywords.DisableTexture2DXArray);
				}
				taaData.noiseTexture = STP.UseTexture(computeRenderGraphBuilder3, texture, AccessFlags.Read);
				if (config.debugView.IsValid())
				{
					taaData.cs.EnableKeyword(STP.ShaderKeywords.EnableDebugMode);
					taaData.debugView = STP.UseTexture(computeRenderGraphBuilder3, config.debugView, AccessFlags.ReadWrite);
				}
				taaData.kernelIndex = taaData.cs.FindKernel("StpTaa");
				taaData.viewCount = config.numActiveViews;
				taaData.dispatchSize = new Vector2Int(CoreUtils.DivRoundUp(config.outputImageSize.x, vector2Int.x), CoreUtils.DivRoundUp(config.outputImageSize.y, vector2Int.y));
				taaData.intermediateColor = STP.UseTexture(computeRenderGraphBuilder3, setupData4.intermediateColor, AccessFlags.Read);
				taaData.intermediateWeights = STP.UseTexture(computeRenderGraphBuilder3, preTaaData3.intermediateWeights, AccessFlags.Read);
				taaData.priorFeedback = STP.UseTexture(computeRenderGraphBuilder3, renderGraph.ImportTexture(previousHistoryTexture4), AccessFlags.Read);
				taaData.depthMotion = STP.UseTexture(computeRenderGraphBuilder3, renderGraph.ImportTexture(currentHistoryTexture), AccessFlags.Read);
				taaData.convergence = STP.UseTexture(computeRenderGraphBuilder3, renderGraph.ImportTexture(currentHistoryTexture3), AccessFlags.Read);
				taaData.feedback = STP.UseTexture(computeRenderGraphBuilder3, renderGraph.ImportTexture(currentHistoryTexture4), AccessFlags.WriteAll);
				taaData.output = STP.UseTexture(computeRenderGraphBuilder3, config.destination, AccessFlags.WriteAll);
				computeRenderGraphBuilder3.SetRenderFunc<STP.TaaData>(delegate(STP.TaaData data, ComputeGraphContext ctx)
				{
					ConstantBuffer.Set<STP.StpConstantBufferData>(data.cs, STP.ShaderResources._StpConstantBufferData);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpBlueNoiseIn, data.noiseTexture);
					if (data.debugView.IsValid())
					{
						ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpDebugOut, data.debugView);
					}
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpIntermediateColor, data.intermediateColor);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpIntermediateWeights, data.intermediateWeights);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpPriorFeedback, data.priorFeedback);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpDepthMotion, data.depthMotion);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpConvergence, data.convergence);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpFeedback, data.feedback);
					ctx.cmd.SetComputeTextureParam(data.cs, data.kernelIndex, STP.ShaderResources._StpOutput, data.output);
					ctx.cmd.DispatchCompute(data.cs, data.kernelIndex, data.dispatchSize.x, data.dispatchSize.y, data.viewCount);
				});
				taaData2 = taaData;
			}
			return taaData2.output;
		}

		private const int kNumDebugViews = 6;

		private static readonly GUIContent[] s_DebugViewDescriptions = new GUIContent[]
		{
			new GUIContent("Clipped Input Color", "Shows input color clipped to {0 to 1}"),
			new GUIContent("Log Input Depth", "Shows input depth in log scale"),
			new GUIContent("Reversible Tonemapped Input Color", "Shows input color after conversion to reversible tonemaped space"),
			new GUIContent("Shaped Absolute Input Motion", "Visualizes input motion vectors"),
			new GUIContent("Motion Reprojection {R=Prior G=This Sqrt Luma Feedback Diff, B=Offscreen}", "Visualizes reprojected frame difference"),
			new GUIContent("Sensitivity {G=No motion match, R=Responsive, B=Luma}", "Visualize pixel sensitivities")
		};

		private static readonly int[] s_DebugViewIndices = new int[]
		{
			0,
			1,
			2,
			3,
			4,
			5
		};

		private const int kMaxPerViewConfigs = 2;

		private static STP.PerViewConfig[] s_PerViewConfigs = new STP.PerViewConfig[2];

		private const int kNumHistoryTextureTypes = 4;

		private const int kTotalSetupViewConstantsCount = 16;

		private static readonly int kQualcommVendorId = 20803;

		public struct PerViewConfig
		{
			public Matrix4x4 currentProj;

			public Matrix4x4 lastProj;

			public Matrix4x4 lastLastProj;

			public Matrix4x4 currentView;

			public Matrix4x4 lastView;

			public Matrix4x4 lastLastView;
		}

		public struct Config
		{
			public Texture2D noiseTexture;

			public TextureHandle inputColor;

			public TextureHandle inputDepth;

			public TextureHandle inputMotion;

			public TextureHandle inputStencil;

			public TextureHandle debugView;

			public TextureHandle destination;

			public STP.HistoryContext historyContext;

			public bool enableHwDrs;

			public bool enableTexArray;

			public bool enableMotionScaling;

			public float nearPlane;

			public float farPlane;

			public int frameIndex;

			public bool hasValidHistory;

			public int stencilMask;

			public int debugViewIndex;

			public float deltaTime;

			public float lastDeltaTime;

			public Vector2Int currentImageSize;

			public Vector2Int priorImageSize;

			public Vector2Int outputImageSize;

			public int numActiveViews;

			public STP.PerViewConfig[] perViewConfigs;
		}

		internal enum HistoryTextureType
		{
			DepthMotion,
			Luma,
			Convergence,
			Feedback,
			Count
		}

		public struct HistoryUpdateInfo
		{
			public Vector2Int preUpscaleSize;

			public Vector2Int postUpscaleSize;

			public bool useHwDrs;

			public bool useTexArray;
		}

		public sealed class HistoryContext : IDisposable
		{
			public bool Update(ref STP.HistoryUpdateInfo info)
			{
				bool result = true;
				Hash128 hash = STP.ComputeHistoryHash(ref info);
				if (hash != this.m_hash)
				{
					result = false;
					this.Dispose();
					this.m_hash = hash;
					Vector2Int historyTextureSize = info.useHwDrs ? info.postUpscaleSize : info.preUpscaleSize;
					TextureDimension textureDimension = info.useTexArray ? TextureDimension.Tex2DArray : TextureDimension.Tex2D;
					int num = info.useTexArray ? TextureXR.slices : 1;
					int num2 = 0;
					int num3 = 0;
					GraphicsFormat graphicsFormat = GraphicsFormat.None;
					bool useDynamicScaleExplicit = false;
					string text = "";
					for (int i = 0; i < 4; i++)
					{
						switch (i)
						{
						case 0:
							num2 = historyTextureSize.x;
							num3 = historyTextureSize.y;
							graphicsFormat = GraphicsFormat.R32_UInt;
							useDynamicScaleExplicit = info.useHwDrs;
							text = "STP Depth & Motion";
							break;
						case 1:
							num2 = historyTextureSize.x;
							num3 = historyTextureSize.y;
							graphicsFormat = GraphicsFormat.R8G8_UNorm;
							useDynamicScaleExplicit = info.useHwDrs;
							text = "STP Luma";
							break;
						case 2:
						{
							Vector2Int vector2Int = STP.CalculateConvergenceTextureSize(historyTextureSize);
							num2 = vector2Int.x;
							num3 = vector2Int.y;
							graphicsFormat = GraphicsFormat.R8_UNorm;
							useDynamicScaleExplicit = info.useHwDrs;
							text = "STP Convergence";
							break;
						}
						case 3:
							num2 = info.postUpscaleSize.x;
							num3 = info.postUpscaleSize.y;
							graphicsFormat = GraphicsFormat.A2B10G10R10_UNormPack32;
							useDynamicScaleExplicit = false;
							text = "STP Feedback";
							break;
						}
						for (int j = 0; j < 2; j++)
						{
							int num4 = j * 4 + i;
							RTHandle[] textures = this.m_textures;
							int num5 = num4;
							int width = num2;
							int height = num3;
							GraphicsFormat format = graphicsFormat;
							int slices = num;
							FilterMode filterMode = FilterMode.Point;
							TextureWrapMode wrapMode = TextureWrapMode.Repeat;
							TextureDimension dimension = textureDimension;
							bool enableRandomWrite = true;
							bool useMipMap = false;
							bool autoGenerateMips = true;
							bool isShadowMap = false;
							int anisoLevel = 1;
							float mipMapBias = 0f;
							MSAASamples msaaSamples = MSAASamples.None;
							bool bindTextureMS = false;
							bool useDynamicScale = false;
							string name = text;
							textures[num5] = RTHandles.Alloc(width, height, format, slices, filterMode, wrapMode, dimension, enableRandomWrite, useMipMap, autoGenerateMips, isShadowMap, anisoLevel, mipMapBias, msaaSamples, bindTextureMS, useDynamicScale, useDynamicScaleExplicit, RenderTextureMemoryless.None, VRTextureUsage.None, name);
						}
					}
				}
				return result;
			}

			internal RTHandle GetCurrentHistoryTexture(STP.HistoryTextureType historyType, int frameIndex)
			{
				return this.m_textures[(int)((frameIndex & 1) * 4 + historyType)];
			}

			internal RTHandle GetPreviousHistoryTexture(STP.HistoryTextureType historyType, int frameIndex)
			{
				return this.m_textures[(int)(((frameIndex & 1) ^ 1) * 4 + historyType)];
			}

			public void Dispose()
			{
				for (int i = 0; i < this.m_textures.Length; i++)
				{
					if (this.m_textures[i] != null)
					{
						this.m_textures[i].Release();
						this.m_textures[i] = null;
					}
				}
				this.m_hash = Hash128.Compute(0);
			}

			private RTHandle[] m_textures = new RTHandle[8];

			private Hash128 m_hash = Hash128.Compute(0);
		}

		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\STP\\STP.cs")]
		private enum StpSetupPerViewConstants
		{
			Count = 8
		}

		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\STP\\STP.cs", needAccessors = false, generateCBuffer = true)]
		private struct StpConstantBufferData
		{
			public Vector4 _StpCommonConstant;

			public Vector4 _StpSetupConstants0;

			public Vector4 _StpSetupConstants1;

			public Vector4 _StpSetupConstants2;

			public Vector4 _StpSetupConstants3;

			public Vector4 _StpSetupConstants4;

			public Vector4 _StpSetupConstants5;

			[FixedBuffer(typeof(float), 64)]
			[HLSLArray(16, typeof(Vector4))]
			public STP.StpConstantBufferData.<_StpSetupPerViewConstants>e__FixedBuffer _StpSetupPerViewConstants;

			public Vector4 _StpDilConstants0;

			public Vector4 _StpTaaConstants0;

			public Vector4 _StpTaaConstants1;

			public Vector4 _StpTaaConstants2;

			public Vector4 _StpTaaConstants3;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 256)]
			public struct <_StpSetupPerViewConstants>e__FixedBuffer
			{
				public float FixedElementField;
			}
		}

		private static class ShaderResources
		{
			public static readonly int _StpConstantBufferData = Shader.PropertyToID("StpConstantBufferData");

			public static readonly int _StpBlueNoiseIn = Shader.PropertyToID("_StpBlueNoiseIn");

			public static readonly int _StpDebugOut = Shader.PropertyToID("_StpDebugOut");

			public static readonly int _StpInputColor = Shader.PropertyToID("_StpInputColor");

			public static readonly int _StpInputDepth = Shader.PropertyToID("_StpInputDepth");

			public static readonly int _StpInputMotion = Shader.PropertyToID("_StpInputMotion");

			public static readonly int _StpInputStencil = Shader.PropertyToID("_StpInputStencil");

			public static readonly int _StpIntermediateColor = Shader.PropertyToID("_StpIntermediateColor");

			public static readonly int _StpIntermediateConvergence = Shader.PropertyToID("_StpIntermediateConvergence");

			public static readonly int _StpIntermediateWeights = Shader.PropertyToID("_StpIntermediateWeights");

			public static readonly int _StpPriorLuma = Shader.PropertyToID("_StpPriorLuma");

			public static readonly int _StpLuma = Shader.PropertyToID("_StpLuma");

			public static readonly int _StpPriorDepthMotion = Shader.PropertyToID("_StpPriorDepthMotion");

			public static readonly int _StpDepthMotion = Shader.PropertyToID("_StpDepthMotion");

			public static readonly int _StpPriorFeedback = Shader.PropertyToID("_StpPriorFeedback");

			public static readonly int _StpFeedback = Shader.PropertyToID("_StpFeedback");

			public static readonly int _StpPriorConvergence = Shader.PropertyToID("_StpPriorConvergence");

			public static readonly int _StpConvergence = Shader.PropertyToID("_StpConvergence");

			public static readonly int _StpOutput = Shader.PropertyToID("_StpOutput");
		}

		private static class ShaderKeywords
		{
			public static readonly string EnableDebugMode = "ENABLE_DEBUG_MODE";

			public static readonly string EnableLargeKernel = "ENABLE_LARGE_KERNEL";

			public static readonly string EnableStencilResponsive = "ENABLE_STENCIL_RESPONSIVE";

			public static readonly string DisableTexture2DXArray = "DISABLE_TEXTURE2D_X_ARRAY";
		}

		[SupportedOnRenderPipeline(new Type[]
		{

		})]
		[CategoryInfo(Name = "R: STP", Order = 1000)]
		[ElementInfo(Order = 0)]
		[HideInInspector]
		[Serializable]
		internal class RuntimeResources : IRenderPipelineResources, IRenderPipelineGraphicsSettings
		{
			public int version
			{
				get
				{
					return 0;
				}
			}

			public ComputeShader setupCS
			{
				get
				{
					return this.m_setupCS;
				}
				set
				{
					this.SetValueAndNotify(ref this.m_setupCS, value, "setupCS");
				}
			}

			public ComputeShader preTaaCS
			{
				get
				{
					return this.m_preTaaCS;
				}
				set
				{
					this.SetValueAndNotify(ref this.m_preTaaCS, value, "preTaaCS");
				}
			}

			public ComputeShader taaCS
			{
				get
				{
					return this.m_taaCS;
				}
				set
				{
					this.SetValueAndNotify(ref this.m_taaCS, value, "taaCS");
				}
			}

			[SerializeField]
			[ResourcePath("Runtime/STP/StpSetup.compute", SearchType.ProjectPath)]
			private ComputeShader m_setupCS;

			[SerializeField]
			[ResourcePath("Runtime/STP/StpPreTaa.compute", SearchType.ProjectPath)]
			private ComputeShader m_preTaaCS;

			[SerializeField]
			[ResourcePath("Runtime/STP/StpTaa.compute", SearchType.ProjectPath)]
			private ComputeShader m_taaCS;
		}

		private enum ProfileId
		{
			StpSetup,
			StpPreTaa,
			StpTaa
		}

		private class SetupData
		{
			public ComputeShader cs;

			public int kernelIndex;

			public int viewCount;

			public Vector2Int dispatchSize;

			public STP.StpConstantBufferData constantBufferData;

			public TextureHandle noiseTexture;

			public TextureHandle debugView;

			public TextureHandle inputColor;

			public TextureHandle inputDepth;

			public TextureHandle inputMotion;

			public TextureHandle inputStencil;

			public TextureHandle intermediateColor;

			public TextureHandle intermediateConvergence;

			public TextureHandle priorDepthMotion;

			public TextureHandle depthMotion;

			public TextureHandle priorLuma;

			public TextureHandle luma;

			public TextureHandle priorFeedback;

			public TextureHandle priorConvergence;
		}

		private class PreTaaData
		{
			public ComputeShader cs;

			public int kernelIndex;

			public int viewCount;

			public Vector2Int dispatchSize;

			public TextureHandle noiseTexture;

			public TextureHandle debugView;

			public TextureHandle intermediateConvergence;

			public TextureHandle intermediateWeights;

			public TextureHandle luma;

			public TextureHandle convergence;
		}

		private class TaaData
		{
			public ComputeShader cs;

			public int kernelIndex;

			public int viewCount;

			public Vector2Int dispatchSize;

			public TextureHandle noiseTexture;

			public TextureHandle debugView;

			public TextureHandle intermediateColor;

			public TextureHandle intermediateWeights;

			public TextureHandle priorFeedback;

			public TextureHandle depthMotion;

			public TextureHandle convergence;

			public TextureHandle feedback;

			public TextureHandle output;
		}
	}
}
