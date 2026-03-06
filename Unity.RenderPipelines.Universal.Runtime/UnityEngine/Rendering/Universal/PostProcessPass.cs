using System;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class PostProcessPass : ScriptableRenderPass
	{
		public PostProcessPass(RenderPassEvent evt, PostProcessData data, ref PostProcessParams postProcessParams)
		{
			base.profilingSampler = new ProfilingSampler("PostProcessPass");
			base.renderPassEvent = evt;
			this.m_Data = data;
			this.m_Materials = new PostProcessPass.MaterialLibrary(data);
			this.m_BloomMipUp = new RTHandle[16];
			this.m_BloomMipDown = new RTHandle[16];
			this.m_BloomMipDownName = new string[16];
			this.m_BloomMipUpName = new string[16];
			this._BloomMipUp = new TextureHandle[16];
			this._BloomMipDown = new TextureHandle[16];
			for (int i = 0; i < 16; i++)
			{
				this.m_BloomMipUpName[i] = "_BloomMipUp" + i.ToString();
				this.m_BloomMipDownName[i] = "_BloomMipDown" + i.ToString();
			}
			this.m_MRT2 = new RenderTargetIdentifier[2];
			base.useNativeRenderPass = false;
			this.m_BlitMaterial = postProcessParams.blitMaterial;
			bool flag = this.IsHDRFormat(postProcessParams.requestColorFormat);
			bool defaultColorFormatIsAlpha = this.IsAlphaFormat(postProcessParams.requestColorFormat);
			if (flag)
			{
				this.m_DefaultColorFormatIsAlpha = defaultColorFormatIsAlpha;
				if (SystemInfo.IsFormatSupported(postProcessParams.requestColorFormat, GraphicsFormatUsage.Blend))
				{
					this.m_DefaultColorFormat = postProcessParams.requestColorFormat;
				}
				else if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, GraphicsFormatUsage.Blend))
				{
					this.m_DefaultColorFormat = GraphicsFormat.B10G11R11_UFloatPack32;
					this.m_DefaultColorFormatIsAlpha = false;
				}
				else
				{
					this.m_DefaultColorFormat = ((QualitySettings.activeColorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm);
				}
			}
			else
			{
				this.m_DefaultColorFormat = ((QualitySettings.activeColorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm);
				this.m_DefaultColorFormatIsAlpha = true;
			}
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R8G8_UNorm, GraphicsFormatUsage.Render) && SystemInfo.graphicsDeviceVendor.ToLowerInvariant().Contains("arm"))
			{
				this.m_SMAAEdgeFormat = GraphicsFormat.R8G8_UNorm;
			}
			else
			{
				this.m_SMAAEdgeFormat = GraphicsFormat.R8G8B8A8_UNorm;
			}
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R16_UNorm, GraphicsFormatUsage.Blend))
			{
				this.m_GaussianCoCFormat = GraphicsFormat.R16_UNorm;
				return;
			}
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R16_SFloat, GraphicsFormatUsage.Blend))
			{
				this.m_GaussianCoCFormat = GraphicsFormat.R16_SFloat;
				return;
			}
			this.m_GaussianCoCFormat = GraphicsFormat.R8_UNorm;
		}

		public void Cleanup()
		{
			this.m_Materials.Cleanup();
			this.Dispose();
		}

		public void Dispose()
		{
			foreach (RTHandle rthandle in this.m_BloomMipDown)
			{
				if (rthandle != null)
				{
					rthandle.Release();
				}
			}
			foreach (RTHandle rthandle2 in this.m_BloomMipUp)
			{
				if (rthandle2 != null)
				{
					rthandle2.Release();
				}
			}
			RTHandle scalingSetupTarget = this.m_ScalingSetupTarget;
			if (scalingSetupTarget != null)
			{
				scalingSetupTarget.Release();
			}
			RTHandle upscaledTarget = this.m_UpscaledTarget;
			if (upscaledTarget != null)
			{
				upscaledTarget.Release();
			}
			RTHandle fullCoCTexture = this.m_FullCoCTexture;
			if (fullCoCTexture != null)
			{
				fullCoCTexture.Release();
			}
			RTHandle halfCoCTexture = this.m_HalfCoCTexture;
			if (halfCoCTexture != null)
			{
				halfCoCTexture.Release();
			}
			RTHandle pingTexture = this.m_PingTexture;
			if (pingTexture != null)
			{
				pingTexture.Release();
			}
			RTHandle pongTexture = this.m_PongTexture;
			if (pongTexture != null)
			{
				pongTexture.Release();
			}
			RTHandle blendTexture = this.m_BlendTexture;
			if (blendTexture != null)
			{
				blendTexture.Release();
			}
			RTHandle edgeColorTexture = this.m_EdgeColorTexture;
			if (edgeColorTexture != null)
			{
				edgeColorTexture.Release();
			}
			RTHandle edgeStencilTexture = this.m_EdgeStencilTexture;
			if (edgeStencilTexture != null)
			{
				edgeStencilTexture.Release();
			}
			RTHandle tempTarget = this.m_TempTarget;
			if (tempTarget != null)
			{
				tempTarget.Release();
			}
			RTHandle tempTarget2 = this.m_TempTarget2;
			if (tempTarget2 != null)
			{
				tempTarget2.Release();
			}
			RTHandle streakTmpTexture = this.m_StreakTmpTexture;
			if (streakTmpTexture != null)
			{
				streakTmpTexture.Release();
			}
			RTHandle streakTmpTexture2 = this.m_StreakTmpTexture2;
			if (streakTmpTexture2 != null)
			{
				streakTmpTexture2.Release();
			}
			RTHandle screenSpaceLensFlareResult = this.m_ScreenSpaceLensFlareResult;
			if (screenSpaceLensFlareResult != null)
			{
				screenSpaceLensFlareResult.Release();
			}
			RTHandle userLut = this.m_UserLut;
			if (userLut == null)
			{
				return;
			}
			userLut.Release();
		}

		public void Setup(in RenderTextureDescriptor baseDescriptor, in RTHandle source, bool resolveToScreen, in RTHandle depth, in RTHandle internalLut, in RTHandle motionVectors, bool hasFinalPass, bool enableColorEncoding)
		{
			this.m_Descriptor = baseDescriptor;
			this.m_Descriptor.useMipMap = false;
			this.m_Descriptor.autoGenerateMips = false;
			this.m_Source = source;
			this.m_Depth = depth;
			this.m_InternalLut = internalLut;
			this.m_MotionVectors = motionVectors;
			this.m_IsFinalPass = false;
			this.m_HasFinalPass = hasFinalPass;
			this.m_EnableColorEncodingIfNeeded = enableColorEncoding;
			this.m_ResolveToScreen = resolveToScreen;
			this.m_UseSwapBuffer = true;
			this.m_Destination = ScriptableRenderPass.k_CameraTarget;
		}

		public void SetupFinalPass(in RTHandle source, bool useSwapBuffer = false, bool enableColorEncoding = true)
		{
			this.m_Source = source;
			this.m_IsFinalPass = true;
			this.m_HasFinalPass = false;
			this.m_EnableColorEncodingIfNeeded = enableColorEncoding;
			this.m_UseSwapBuffer = useSwapBuffer;
			this.m_Destination = ScriptableRenderPass.k_CameraTarget;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			base.overrideCameraTarget = true;
		}

		public bool CanRunOnTile()
		{
			return false;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			VolumeStack stack = VolumeManager.instance.stack;
			this.m_DepthOfField = stack.GetComponent<DepthOfField>();
			this.m_MotionBlur = stack.GetComponent<MotionBlur>();
			this.m_LensFlareScreenSpace = stack.GetComponent<ScreenSpaceLensFlare>();
			this.m_PaniniProjection = stack.GetComponent<PaniniProjection>();
			this.m_Bloom = stack.GetComponent<Bloom>();
			this.m_LensDistortion = stack.GetComponent<LensDistortion>();
			this.m_ChromaticAberration = stack.GetComponent<ChromaticAberration>();
			this.m_Vignette = stack.GetComponent<Vignette>();
			this.m_ColorLookup = stack.GetComponent<ColorLookup>();
			this.m_ColorAdjustments = stack.GetComponent<ColorAdjustments>();
			this.m_Tonemapping = stack.GetComponent<Tonemapping>();
			this.m_FilmGrain = stack.GetComponent<FilmGrain>();
			this.m_UseFastSRGBLinearConversion = *renderingData.postProcessingData.useFastSRGBLinearConversion;
			this.m_SupportScreenSpaceLensFlare = *renderingData.postProcessingData.supportScreenSpaceLensFlare;
			this.m_SupportDataDrivenLensFlare = *renderingData.postProcessingData.supportDataDrivenLensFlare;
			CommandBuffer cmd = *renderingData.commandBuffer;
			if (this.m_IsFinalPass)
			{
				using (new ProfilingScope(cmd, PostProcessPass.m_ProfilingRenderFinalPostProcessing))
				{
					this.RenderFinalPass(cmd, ref renderingData);
					return;
				}
			}
			if (!this.CanRunOnTile())
			{
				using (new ProfilingScope(cmd, PostProcessPass.m_ProfilingRenderPostProcessing))
				{
					this.Render(cmd, ref renderingData);
				}
			}
		}

		private bool IsHDRFormat(GraphicsFormat format)
		{
			return format == GraphicsFormat.B10G11R11_UFloatPack32 || GraphicsFormatUtility.IsHalfFormat(format) || GraphicsFormatUtility.IsFloatFormat(format);
		}

		private bool IsAlphaFormat(GraphicsFormat format)
		{
			return GraphicsFormatUtility.HasAlphaChannel(format);
		}

		private RenderTextureDescriptor GetCompatibleDescriptor()
		{
			return this.GetCompatibleDescriptor(this.m_Descriptor.width, this.m_Descriptor.height, this.m_Descriptor.graphicsFormat, GraphicsFormat.None);
		}

		private RenderTextureDescriptor GetCompatibleDescriptor(int width, int height, GraphicsFormat format, GraphicsFormat depthStencilFormat = GraphicsFormat.None)
		{
			return PostProcessPass.GetCompatibleDescriptor(this.m_Descriptor, width, height, format, depthStencilFormat);
		}

		internal static RenderTextureDescriptor GetCompatibleDescriptor(RenderTextureDescriptor desc, int width, int height, GraphicsFormat format, GraphicsFormat depthStencilFormat = GraphicsFormat.None)
		{
			desc.depthStencilFormat = depthStencilFormat;
			desc.msaaSamples = 1;
			desc.width = width;
			desc.height = height;
			desc.graphicsFormat = format;
			return desc;
		}

		private bool RequireSRGBConversionBlitToBackBuffer(bool requireSrgbConversion)
		{
			return requireSrgbConversion && this.m_EnableColorEncodingIfNeeded;
		}

		private bool RequireHDROutput(UniversalCameraData cameraData)
		{
			return cameraData.isHDROutputActive && cameraData.captureActions == null;
		}

		private unsafe void Render(CommandBuffer cmd, ref RenderingData renderingData)
		{
			PostProcessPass.<>c__DisplayClass91_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.cmd = cmd;
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			ref ScriptableRenderer ptr = ref universalCameraData.renderer;
			bool isSceneViewCamera = universalCameraData.isSceneViewCamera;
			bool flag = universalCameraData.isStopNaNEnabled && this.m_Materials.stopNaN != null;
			bool flag2 = universalCameraData.antialiasing == AntialiasingMode.SubpixelMorphologicalAntiAliasing;
			Material x = (this.m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian) ? this.m_Materials.gaussianDepthOfField : this.m_Materials.bokehDepthOfField;
			bool flag3 = this.m_DepthOfField.IsActive() && !isSceneViewCamera && x != null;
			bool flag4 = !LensFlareCommonSRP.Instance.IsEmpty() && this.m_SupportDataDrivenLensFlare;
			bool flag5 = this.m_LensFlareScreenSpace.IsActive() && this.m_SupportScreenSpaceLensFlare;
			bool flag6 = this.m_MotionBlur.IsActive() && !isSceneViewCamera;
			bool flag7 = this.m_PaniniProjection.IsActive() && !isSceneViewCamera;
			flag6 = (flag6 && Application.isPlaying);
			bool flag8 = universalCameraData.IsTemporalAAEnabled();
			if (universalCameraData.IsTemporalAARequested() && !flag8)
			{
				TemporalAA.ValidateAndWarn(universalCameraData, false);
			}
			CS$<>8__locals1.amountOfPassesRemaining = (flag ? 1 : 0) + (flag2 ? 1 : 0) + (flag3 ? 1 : 0) + (flag4 ? 1 : 0) + (flag8 ? 1 : 0) + (flag6 ? 1 : 0) + (flag7 ? 1 : 0);
			if (this.m_UseSwapBuffer && CS$<>8__locals1.amountOfPassesRemaining > 0)
			{
				ptr.EnableSwapBufferMSAA(false);
			}
			CS$<>8__locals1.source = (this.m_UseSwapBuffer ? ptr.cameraColorTargetHandle : this.m_Source);
			CS$<>8__locals1.destination = (this.m_UseSwapBuffer ? ptr.GetCameraColorFrontBuffer(CS$<>8__locals1.cmd) : null);
			CS$<>8__locals1.cmd.SetGlobalMatrix(PostProcessPass.ShaderConstants._FullscreenProjMat, GL.GetGPUProjectionMatrix(Matrix4x4.identity, true));
			if (flag)
			{
				using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.StopNaNs)))
				{
					Blitter.BlitCameraTexture(CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), this.<Render>g__GetDestination|91_1(ref CS$<>8__locals1), RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, this.m_Materials.stopNaN, 0);
					this.<Render>g__Swap|91_2(ref ptr, ref CS$<>8__locals1);
				}
			}
			if (flag2)
			{
				using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.SMAA)))
				{
					this.DoSubpixelMorphologicalAntialiasing(ref renderingData.cameraData, CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), this.<Render>g__GetDestination|91_1(ref CS$<>8__locals1));
					this.<Render>g__Swap|91_2(ref ptr, ref CS$<>8__locals1);
				}
			}
			if (flag3)
			{
				URPProfileId marker = (this.m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian) ? URPProfileId.GaussianDepthOfField : URPProfileId.BokehDepthOfField;
				using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(marker)))
				{
					this.DoDepthOfField(ref renderingData.cameraData, CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), this.<Render>g__GetDestination|91_1(ref CS$<>8__locals1), universalCameraData.pixelRect);
					this.<Render>g__Swap|91_2(ref ptr, ref CS$<>8__locals1);
				}
			}
			if (flag8)
			{
				using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.TemporalAA)))
				{
					CommandBuffer cmd2 = CS$<>8__locals1.cmd;
					Material temporalAntialiasing = this.m_Materials.temporalAntialiasing;
					RTHandle source = CS$<>8__locals1.source;
					RTHandle destination = CS$<>8__locals1.destination;
					RTHandle motionVectors = this.m_MotionVectors;
					TemporalAA.ExecutePass(cmd2, temporalAntialiasing, ref renderingData.cameraData, source, destination, (motionVectors != null) ? motionVectors.rt : null);
					this.<Render>g__Swap|91_2(ref ptr, ref CS$<>8__locals1);
				}
			}
			if (flag6)
			{
				using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.MotionBlur)))
				{
					this.DoMotionBlur(CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), this.<Render>g__GetDestination|91_1(ref CS$<>8__locals1), this.m_MotionVectors, ref renderingData.cameraData);
					this.<Render>g__Swap|91_2(ref ptr, ref CS$<>8__locals1);
				}
			}
			if (flag7)
			{
				using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.PaniniProjection)))
				{
					this.DoPaniniProjection(universalCameraData.camera, CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), this.<Render>g__GetDestination|91_1(ref CS$<>8__locals1));
					this.<Render>g__Swap|91_2(ref ptr, ref CS$<>8__locals1);
				}
			}
			using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.UberPostProcess)))
			{
				this.m_Materials.uber.shaderKeywords = null;
				bool flag9 = this.m_Bloom.IsActive();
				bool flag10 = this.m_LensFlareScreenSpace.IsActive();
				if (flag9 || flag10)
				{
					using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.Bloom)))
					{
						this.SetupBloom(CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), this.m_Materials.uber, universalCameraData.isAlphaOutputEnabled);
					}
				}
				if (flag5)
				{
					using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.LensFlareScreenSpace)))
					{
						int num = Mathf.Clamp(this.m_LensFlareScreenSpace.bloomMip.value, 0, this.m_Bloom.maxIterations.value / 2);
						this.DoLensFlareScreenSpace(universalCameraData.camera, CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), this.m_BloomMipUp[0], this.m_BloomMipUp[num]);
					}
				}
				if (flag4)
				{
					bool usePanini;
					float paniniDistance;
					float paniniCropToFit;
					if (this.m_PaniniProjection.IsActive())
					{
						usePanini = true;
						paniniDistance = this.m_PaniniProjection.distance.value;
						paniniCropToFit = this.m_PaniniProjection.cropToFit.value;
					}
					else
					{
						usePanini = false;
						paniniDistance = 1f;
						paniniCropToFit = 1f;
					}
					using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.LensFlareDataDrivenComputeOcclusion)))
					{
						this.LensFlareDataDrivenComputeOcclusion(ref universalCameraData, CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), usePanini, paniniDistance, paniniCropToFit);
					}
					using (new ProfilingScope(CS$<>8__locals1.cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.LensFlareDataDriven)))
					{
						this.LensFlareDataDriven(ref universalCameraData, CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), usePanini, paniniDistance, paniniCropToFit);
					}
				}
				this.SetupLensDistortion(this.m_Materials.uber, isSceneViewCamera);
				this.SetupChromaticAberration(this.m_Materials.uber);
				this.SetupVignette(this.m_Materials.uber, universalCameraData.xr, this.m_Descriptor.width, this.m_Descriptor.height);
				this.SetupColorGrading(CS$<>8__locals1.cmd, ref renderingData, this.m_Materials.uber);
				this.SetupGrain(universalCameraData, this.m_Materials.uber);
				this.SetupDithering(universalCameraData, this.m_Materials.uber);
				if (this.RequireSRGBConversionBlitToBackBuffer(universalCameraData.requireSrgbConversion))
				{
					this.m_Materials.uber.EnableKeyword("_LINEAR_TO_SRGB_CONVERSION");
				}
				if (this.RequireHDROutput(universalCameraData))
				{
					HDROutputUtils.Operation hdrOperations = (!this.m_HasFinalPass && this.m_EnableColorEncodingIfNeeded) ? HDROutputUtils.Operation.ColorEncoding : HDROutputUtils.Operation.None;
					this.SetupHDROutput(universalCameraData.hdrDisplayInformation, universalCameraData.hdrDisplayColorGamut, this.m_Materials.uber, hdrOperations, universalCameraData.rendersOverlayUI);
				}
				if (this.m_UseFastSRGBLinearConversion)
				{
					this.m_Materials.uber.EnableKeyword("_USE_FAST_SRGB_LINEAR_CONVERSION");
				}
				CoreUtils.SetKeyword(this.m_Materials.uber, "_ENABLE_ALPHA_OUTPUT", universalCameraData.isAlphaOutputEnabled);
				DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
				bool flag11 = activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget);
				RenderBufferLoadAction loadAction = RenderBufferLoadAction.DontCare;
				if (this.m_Destination == ScriptableRenderPass.k_CameraTarget && !universalCameraData.isDefaultViewport)
				{
					loadAction = RenderBufferLoadAction.Load;
				}
				RenderTargetIdentifier renderTargetIdentifier = BuiltinRenderTextureType.CameraTarget;
				if (universalCameraData.xr.enabled)
				{
					renderTargetIdentifier = universalCameraData.xr.renderTarget;
				}
				if (!this.m_UseSwapBuffer)
				{
					this.m_ResolveToScreen = (universalCameraData.resolveFinalTarget || this.m_Destination.nameID == renderTargetIdentifier || this.m_HasFinalPass);
				}
				if (this.m_UseSwapBuffer && !this.m_ResolveToScreen)
				{
					if (!this.m_HasFinalPass)
					{
						ptr.EnableSwapBufferMSAA(true);
						CS$<>8__locals1.destination = ptr.GetCameraColorFrontBuffer(CS$<>8__locals1.cmd);
					}
					Blitter.BlitCameraTexture(CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), CS$<>8__locals1.destination, loadAction, RenderBufferStoreAction.Store, this.m_Materials.uber, 0);
					ptr.ConfigureCameraColorTarget(CS$<>8__locals1.destination);
					this.<Render>g__Swap|91_2(ref ptr, ref CS$<>8__locals1);
				}
				else if (!this.m_UseSwapBuffer)
				{
					RTHandle source2 = this.<Render>g__GetSource|91_0(ref CS$<>8__locals1);
					Blitter.BlitCameraTexture(CS$<>8__locals1.cmd, source2, this.<Render>g__GetDestination|91_1(ref CS$<>8__locals1), loadAction, RenderBufferStoreAction.Store, this.m_Materials.uber, 0);
					CommandBuffer cmd3 = CS$<>8__locals1.cmd;
					RTHandle source3 = this.<Render>g__GetDestination|91_1(ref CS$<>8__locals1);
					RTHandle destination2 = this.m_Destination;
					RenderBufferLoadAction loadAction2 = RenderBufferLoadAction.DontCare;
					RenderBufferStoreAction storeAction = RenderBufferStoreAction.Store;
					Material blitMaterial = this.m_BlitMaterial;
					RenderTexture rt = this.m_Destination.rt;
					Blitter.BlitCameraTexture(cmd3, source3, destination2, loadAction2, storeAction, blitMaterial, (rt != null && rt.filterMode == FilterMode.Bilinear) ? 1 : 0);
				}
				else if (this.m_ResolveToScreen)
				{
					if (flag11)
					{
						Blitter.BlitCameraTexture(CS$<>8__locals1.cmd, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), *activeDebugHandler.DebugScreenColorHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, this.m_Materials.uber, 0);
						ptr.ConfigureCameraTarget(*activeDebugHandler.DebugScreenColorHandle, *activeDebugHandler.DebugScreenDepthHandle);
					}
					else
					{
						RTHandleStaticHelpers.SetRTHandleStaticWrapper((universalCameraData.targetTexture != null) ? new RenderTargetIdentifier(universalCameraData.targetTexture) : renderTargetIdentifier);
						RTHandle s_RTHandleWrapper = RTHandleStaticHelpers.s_RTHandleWrapper;
						RenderingUtils.FinalBlit(CS$<>8__locals1.cmd, universalCameraData, this.<Render>g__GetSource|91_0(ref CS$<>8__locals1), s_RTHandleWrapper, loadAction, RenderBufferStoreAction.Store, this.m_Materials.uber, 0);
						ptr.ConfigureCameraColorTarget(s_RTHandleWrapper);
					}
				}
			}
		}

		private unsafe void DoSubpixelMorphologicalAntialiasing(ref CameraData cameraData, CommandBuffer cmd, RTHandle source, RTHandle destination)
		{
			Rect viewport = new Rect(Vector2.zero, new Vector2((float)cameraData.cameraTargetDescriptor.width, (float)cameraData.cameraTargetDescriptor.height));
			Material subpixelMorphologicalAntialiasing = this.m_Materials.subpixelMorphologicalAntialiasing;
			RenderTextureDescriptor compatibleDescriptor = this.GetCompatibleDescriptor(this.m_Descriptor.width, this.m_Descriptor.height, GraphicsFormat.None, GraphicsFormatUtility.GetDepthStencilFormat(24));
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_EdgeStencilTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_EdgeStencilTexture");
			compatibleDescriptor = this.GetCompatibleDescriptor(this.m_Descriptor.width, this.m_Descriptor.height, this.m_SMAAEdgeFormat, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_EdgeColorTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_EdgeColorTexture");
			compatibleDescriptor = this.GetCompatibleDescriptor(this.m_Descriptor.width, this.m_Descriptor.height, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_BlendTexture, compatibleDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_BlendTexture");
			Vector2Int vector2Int = this.m_EdgeColorTexture.useScaling ? this.m_EdgeColorTexture.rtHandleProperties.currentRenderTargetSize : new Vector2Int(this.m_EdgeColorTexture.rt.width, this.m_EdgeColorTexture.rt.height);
			subpixelMorphologicalAntialiasing.SetVector(PostProcessPass.ShaderConstants._Metrics, new Vector4(1f / (float)vector2Int.x, 1f / (float)vector2Int.y, (float)vector2Int.x, (float)vector2Int.y));
			subpixelMorphologicalAntialiasing.SetTexture(PostProcessPass.ShaderConstants._AreaTexture, this.m_Data.textures.smaaAreaTex);
			subpixelMorphologicalAntialiasing.SetTexture(PostProcessPass.ShaderConstants._SearchTexture, this.m_Data.textures.smaaSearchTex);
			subpixelMorphologicalAntialiasing.SetFloat(PostProcessPass.ShaderConstants._StencilRef, 64f);
			subpixelMorphologicalAntialiasing.SetFloat(PostProcessPass.ShaderConstants._StencilMask, 64f);
			subpixelMorphologicalAntialiasing.shaderKeywords = null;
			switch (*cameraData.antialiasingQuality)
			{
			case AntialiasingQuality.Low:
				subpixelMorphologicalAntialiasing.EnableKeyword("_SMAA_PRESET_LOW");
				break;
			case AntialiasingQuality.Medium:
				subpixelMorphologicalAntialiasing.EnableKeyword("_SMAA_PRESET_MEDIUM");
				break;
			case AntialiasingQuality.High:
				subpixelMorphologicalAntialiasing.EnableKeyword("_SMAA_PRESET_HIGH");
				break;
			}
			RenderingUtils.Blit(cmd, source, viewport, this.m_EdgeColorTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, this.m_EdgeStencilTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.ColorStencil, Color.clear, subpixelMorphologicalAntialiasing, 0);
			RenderingUtils.Blit(cmd, this.m_EdgeColorTexture, viewport, this.m_BlendTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, this.m_EdgeStencilTexture, RenderBufferLoadAction.Load, RenderBufferStoreAction.DontCare, ClearFlag.Color, Color.clear, subpixelMorphologicalAntialiasing, 1);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._BlendTexture, this.m_BlendTexture.nameID);
			Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, subpixelMorphologicalAntialiasing, 2);
		}

		private unsafe void DoDepthOfField(ref CameraData cameraData, CommandBuffer cmd, RTHandle source, RTHandle destination, Rect pixelRect)
		{
			if (this.m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian)
			{
				this.DoGaussianDepthOfField(cmd, source, destination, pixelRect, *cameraData.isAlphaOutputEnabled);
				return;
			}
			if (this.m_DepthOfField.mode.value == DepthOfFieldMode.Bokeh)
			{
				this.DoBokehDepthOfField(cmd, source, destination, pixelRect, *cameraData.isAlphaOutputEnabled);
			}
		}

		private void DoGaussianDepthOfField(CommandBuffer cmd, RTHandle source, RTHandle destination, Rect pixelRect, bool enableAlphaOutput)
		{
			int num = 2;
			Material gaussianDepthOfField = this.m_Materials.gaussianDepthOfField;
			int num2 = this.m_Descriptor.width / num;
			int height = this.m_Descriptor.height / num;
			float value = this.m_DepthOfField.gaussianStart.value;
			float y = Mathf.Max(value, this.m_DepthOfField.gaussianEnd.value);
			float num3 = this.m_DepthOfField.gaussianMaxRadius.value * ((float)num2 / 1080f);
			num3 = Mathf.Min(num3, 2f);
			CoreUtils.SetKeyword(gaussianDepthOfField, "_ENABLE_ALPHA_OUTPUT", enableAlphaOutput);
			CoreUtils.SetKeyword(gaussianDepthOfField, "_HIGH_QUALITY_SAMPLING", this.m_DepthOfField.highQualitySampling.value);
			gaussianDepthOfField.SetVector(PostProcessPass.ShaderConstants._CoCParams, new Vector3(value, y, num3));
			RenderTextureDescriptor compatibleDescriptor = this.GetCompatibleDescriptor(this.m_Descriptor.width, this.m_Descriptor.height, this.m_GaussianCoCFormat, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_FullCoCTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_FullCoCTexture");
			compatibleDescriptor = this.GetCompatibleDescriptor(num2, height, this.m_GaussianCoCFormat, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_HalfCoCTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_HalfCoCTexture");
			compatibleDescriptor = this.GetCompatibleDescriptor(num2, height, GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PingTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_PingTexture");
			compatibleDescriptor = this.GetCompatibleDescriptor(num2, height, GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PongTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_PongTexture");
			PostProcessUtils.SetSourceSize(cmd, this.m_FullCoCTexture);
			cmd.SetGlobalVector(PostProcessPass.ShaderConstants._DownSampleScaleFactor, new Vector4(1f / (float)num, 1f / (float)num, (float)num, (float)num));
			Blitter.BlitCameraTexture(cmd, source, this.m_FullCoCTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, gaussianDepthOfField, 0);
			this.m_MRT2[0] = this.m_HalfCoCTexture.nameID;
			this.m_MRT2[1] = this.m_PingTexture.nameID;
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._FullCoCTexture, this.m_FullCoCTexture.nameID);
			CoreUtils.SetRenderTarget(cmd, this.m_MRT2, this.m_HalfCoCTexture);
			Vector2 v = source.useScaling ? new Vector2(source.rtHandleProperties.rtHandleScale.x, source.rtHandleProperties.rtHandleScale.y) : Vector2.one;
			Blitter.BlitTexture(cmd, source, v, gaussianDepthOfField, 1);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._HalfCoCTexture, this.m_HalfCoCTexture.nameID);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._ColorTexture, source);
			Blitter.BlitCameraTexture(cmd, this.m_PingTexture, this.m_PongTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, gaussianDepthOfField, 2);
			Blitter.BlitCameraTexture(cmd, this.m_PongTexture, this.m_PingTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, gaussianDepthOfField, 3);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._ColorTexture, this.m_PingTexture.nameID);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._FullCoCTexture, this.m_FullCoCTexture.nameID);
			Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, gaussianDepthOfField, 4);
		}

		private void PrepareBokehKernel(float maxRadius, float rcpAspect)
		{
			if (this.m_BokehKernel == null)
			{
				this.m_BokehKernel = new Vector4[42];
			}
			int num = 0;
			float num2 = (float)this.m_DepthOfField.bladeCount.value;
			float p = 1f - this.m_DepthOfField.bladeCurvature.value;
			float num3 = this.m_DepthOfField.bladeRotation.value * 0.017453292f;
			for (int i = 1; i < 4; i++)
			{
				float num4 = 0.14285715f;
				float num5 = ((float)i + num4) / (3f + num4);
				int num6 = i * 7;
				for (int j = 0; j < num6; j++)
				{
					float num7 = 6.2831855f * (float)j / (float)num6;
					float num8 = Mathf.Cos(3.1415927f / num2);
					float num9 = Mathf.Cos(num7 - 6.2831855f / num2 * Mathf.Floor((num2 * num7 + 3.1415927f) / 6.2831855f));
					float num10 = num5 * Mathf.Pow(num8 / num9, p);
					float num11 = num10 * Mathf.Cos(num7 - num3);
					float num12 = num10 * Mathf.Sin(num7 - num3);
					float num13 = num11 * maxRadius;
					float num14 = num12 * maxRadius;
					float num15 = num13 * num13;
					float num16 = num14 * num14;
					float z = Mathf.Sqrt(num15 + num16);
					float w = num13 * rcpAspect;
					this.m_BokehKernel[num] = new Vector4(num13, num14, z, w);
					num++;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float GetMaxBokehRadiusInPixels(float viewportHeight)
		{
			return Mathf.Min(0.05f, 14f / viewportHeight);
		}

		private void DoBokehDepthOfField(CommandBuffer cmd, RTHandle source, RTHandle destination, Rect pixelRect, bool enableAlphaOutput)
		{
			int num = 2;
			Material bokehDepthOfField = this.m_Materials.bokehDepthOfField;
			int num2 = this.m_Descriptor.width / num;
			int num3 = this.m_Descriptor.height / num;
			float num4 = this.m_DepthOfField.focalLength.value / 1000f;
			float num5 = this.m_DepthOfField.focalLength.value / this.m_DepthOfField.aperture.value;
			float value = this.m_DepthOfField.focusDistance.value;
			float y = num5 * num4 / (value - num4);
			float maxBokehRadiusInPixels = PostProcessPass.GetMaxBokehRadiusInPixels((float)this.m_Descriptor.height);
			float num6 = 1f / ((float)num2 / (float)num3);
			CoreUtils.SetKeyword(bokehDepthOfField, "_ENABLE_ALPHA_OUTPUT", enableAlphaOutput);
			CoreUtils.SetKeyword(bokehDepthOfField, "_USE_FAST_SRGB_LINEAR_CONVERSION", this.m_UseFastSRGBLinearConversion);
			cmd.SetGlobalVector(PostProcessPass.ShaderConstants._CoCParams, new Vector4(value, y, maxBokehRadiusInPixels, num6));
			int hashCode = this.m_DepthOfField.GetHashCode();
			if (hashCode != this.m_BokehHash || maxBokehRadiusInPixels != this.m_BokehMaxRadius || num6 != this.m_BokehRCPAspect)
			{
				this.m_BokehHash = hashCode;
				this.m_BokehMaxRadius = maxBokehRadiusInPixels;
				this.m_BokehRCPAspect = num6;
				this.PrepareBokehKernel(maxBokehRadiusInPixels, num6);
			}
			cmd.SetGlobalVectorArray(PostProcessPass.ShaderConstants._BokehKernel, this.m_BokehKernel);
			RenderTextureDescriptor compatibleDescriptor = this.GetCompatibleDescriptor(this.m_Descriptor.width, this.m_Descriptor.height, GraphicsFormat.R8_UNorm, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_FullCoCTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_FullCoCTexture");
			compatibleDescriptor = this.GetCompatibleDescriptor(num2, num3, GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PingTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_PingTexture");
			compatibleDescriptor = this.GetCompatibleDescriptor(num2, num3, GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormat.None);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PongTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_PongTexture");
			PostProcessUtils.SetSourceSize(cmd, this.m_FullCoCTexture);
			cmd.SetGlobalVector(PostProcessPass.ShaderConstants._DownSampleScaleFactor, new Vector4(1f / (float)num, 1f / (float)num, (float)num, (float)num));
			float num7 = 1f / (float)this.m_Descriptor.height * (float)num;
			cmd.SetGlobalVector(PostProcessPass.ShaderConstants._BokehConstants, new Vector4(num7, num7 * 2f));
			Blitter.BlitCameraTexture(cmd, source, this.m_FullCoCTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bokehDepthOfField, 0);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._FullCoCTexture, this.m_FullCoCTexture.nameID);
			Blitter.BlitCameraTexture(cmd, source, this.m_PingTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bokehDepthOfField, 1);
			Blitter.BlitCameraTexture(cmd, this.m_PingTexture, this.m_PongTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bokehDepthOfField, 2);
			Blitter.BlitCameraTexture(cmd, this.m_PongTexture, this.m_PingTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bokehDepthOfField, 3);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._DofTexture, this.m_PingTexture.nameID);
			Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bokehDepthOfField, 4);
		}

		private static float GetLensFlareLightAttenuation(Light light, Camera cam, Vector3 wo)
		{
			if (!(light != null))
			{
				return 1f;
			}
			switch (light.type)
			{
			case LightType.Spot:
				return LensFlareCommonSRP.ShapeAttenuationSpotConeLight(light.transform.forward, wo, light.spotAngle, light.innerSpotAngle / 180f);
			case LightType.Directional:
				return LensFlareCommonSRP.ShapeAttenuationDirLight(light.transform.forward, cam.transform.forward);
			case LightType.Point:
				return LensFlareCommonSRP.ShapeAttenuationPointLight();
			default:
				return 1f;
			}
		}

		private void LensFlareDataDrivenComputeOcclusion(ref UniversalCameraData cameraData, CommandBuffer cmd, RenderTargetIdentifier source, bool usePanini, float paniniDistance, float paniniCropToFit)
		{
			if (!LensFlareCommonSRP.IsOcclusionRTCompatible())
			{
				return;
			}
			Camera camera = cameraData.camera;
			Matrix4x4 viewProjMatrix;
			if (cameraData.xr.enabled)
			{
				if (cameraData.xr.singlePassEnabled)
				{
					viewProjMatrix = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrixNoJitter(0), true) * cameraData.GetViewMatrix(0);
				}
				else
				{
					viewProjMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;
					int multipassId = cameraData.xr.multipassId;
				}
			}
			else
			{
				viewProjMatrix = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrixNoJitter(0), true) * cameraData.GetViewMatrix(0);
			}
			cmd.SetGlobalTexture(this.m_Depth.name, this.m_Depth.nameID);
			LensFlareCommonSRP.ComputeOcclusion(this.m_Materials.lensFlareDataDriven, camera, cameraData.xr, cameraData.xr.multipassId, (float)this.m_Descriptor.width, (float)this.m_Descriptor.height, usePanini, paniniDistance, paniniCropToFit, true, camera.transform.position, viewProjMatrix, cmd, false, false, null, null);
			if (cameraData.xr.enabled && cameraData.xr.singlePassEnabled)
			{
				for (int i = 1; i < cameraData.xr.viewCount; i++)
				{
					Matrix4x4 viewProjMatrix2 = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrixNoJitter(i), true) * cameraData.GetViewMatrix(i);
					cmd.SetGlobalTexture(this.m_Depth.name, this.m_Depth.nameID);
					LensFlareCommonSRP.ComputeOcclusion(this.m_Materials.lensFlareDataDriven, camera, cameraData.xr, i, (float)this.m_Descriptor.width, (float)this.m_Descriptor.height, usePanini, paniniDistance, paniniCropToFit, true, camera.transform.position, viewProjMatrix2, cmd, false, false, null, null);
				}
			}
		}

		private void LensFlareDataDriven(ref UniversalCameraData cameraData, CommandBuffer cmd, RenderTargetIdentifier source, bool usePanini, float paniniDistance, float paniniCropToFit)
		{
			Camera camera = cameraData.camera;
			Rect viewport = new Rect(Vector2.zero, new Vector2((float)this.m_Descriptor.width, (float)this.m_Descriptor.height));
			if (!cameraData.xr.enabled || (cameraData.xr.enabled && !cameraData.xr.singlePassEnabled))
			{
				Matrix4x4 viewProjMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;
				LensFlareCommonSRP.DoLensFlareDataDrivenCommon(this.m_Materials.lensFlareDataDriven, camera, viewport, cameraData.xr, cameraData.xr.multipassId, (float)this.m_Descriptor.width, (float)this.m_Descriptor.height, usePanini, paniniDistance, paniniCropToFit, true, camera.transform.position, viewProjMatrix, cmd, false, false, null, null, source, (Light light, Camera cam, Vector3 wo) => PostProcessPass.GetLensFlareLightAttenuation(light, cam, wo), false);
				return;
			}
			for (int i = 0; i < cameraData.xr.viewCount; i++)
			{
				Matrix4x4 viewProjMatrix2 = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrixNoJitter(i), true) * cameraData.GetViewMatrix(i);
				LensFlareCommonSRP.DoLensFlareDataDrivenCommon(this.m_Materials.lensFlareDataDriven, camera, viewport, cameraData.xr, cameraData.xr.multipassId, (float)this.m_Descriptor.width, (float)this.m_Descriptor.height, usePanini, paniniDistance, paniniCropToFit, true, camera.transform.position, viewProjMatrix2, cmd, false, false, null, null, source, (Light light, Camera cam, Vector3 wo) => PostProcessPass.GetLensFlareLightAttenuation(light, cam, wo), false);
			}
		}

		private void DoLensFlareScreenSpace(Camera camera, CommandBuffer cmd, RenderTargetIdentifier source, RTHandle originalBloomTexture, RTHandle screenSpaceLensFlareBloomMipTexture)
		{
			int value = (int)this.m_LensFlareScreenSpace.resolution.value;
			int width = Mathf.Max(1, this.m_Descriptor.width / value);
			int height = Mathf.Max(1, this.m_Descriptor.height / value);
			RenderTextureDescriptor compatibleDescriptor = this.GetCompatibleDescriptor(width, height, this.m_DefaultColorFormat, GraphicsFormat.None);
			if (this.m_LensFlareScreenSpace.IsStreaksActive())
			{
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_StreakTmpTexture, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_StreakTmpTexture");
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_StreakTmpTexture2, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_StreakTmpTexture2");
			}
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_ScreenSpaceLensFlareResult, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_ScreenSpaceLensFlareResult");
			LensFlareCommonSRP.DoLensFlareScreenSpaceCommon(this.m_Materials.lensFlareScreenSpace, camera, (float)this.m_Descriptor.width, (float)this.m_Descriptor.height, this.m_LensFlareScreenSpace.tintColor.value, originalBloomTexture, screenSpaceLensFlareBloomMipTexture, null, this.m_StreakTmpTexture, this.m_StreakTmpTexture2, new Vector4(this.m_LensFlareScreenSpace.intensity.value, this.m_LensFlareScreenSpace.firstFlareIntensity.value, this.m_LensFlareScreenSpace.secondaryFlareIntensity.value, this.m_LensFlareScreenSpace.warpedFlareIntensity.value), new Vector4(this.m_LensFlareScreenSpace.vignetteEffect.value, this.m_LensFlareScreenSpace.startingPosition.value, this.m_LensFlareScreenSpace.scale.value, 0f), new Vector4((float)this.m_LensFlareScreenSpace.samples.value, this.m_LensFlareScreenSpace.sampleDimmer.value, this.m_LensFlareScreenSpace.chromaticAbberationIntensity.value, 0f), new Vector4(this.m_LensFlareScreenSpace.streaksIntensity.value, this.m_LensFlareScreenSpace.streaksLength.value, this.m_LensFlareScreenSpace.streaksOrientation.value, this.m_LensFlareScreenSpace.streaksThreshold.value), new Vector4((float)value, this.m_LensFlareScreenSpace.warpedFlareScale.value.x, this.m_LensFlareScreenSpace.warpedFlareScale.value.y, 0f), cmd, this.m_ScreenSpaceLensFlareResult, false);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._Bloom_Texture, originalBloomTexture);
		}

		internal static void UpdateMotionBlurMatrices(ref Material material, Camera camera, XRPass xr)
		{
			MotionVectorsPersistentData motionVectorsPersistentData = null;
			UniversalAdditionalCameraData universalAdditionalCameraData;
			if (camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
			{
				motionVectorsPersistentData = universalAdditionalCameraData.motionVectorsPersistentData;
			}
			if (motionVectorsPersistentData == null)
			{
				return;
			}
			if (xr.enabled && xr.singlePassEnabled)
			{
				material.SetMatrixArray(PostProcessPass.k_ShaderPropertyId_PrevViewProjMStereo, motionVectorsPersistentData.previousViewProjectionStereo);
				material.SetMatrixArray(PostProcessPass.k_ShaderPropertyId_ViewProjMStereo, motionVectorsPersistentData.viewProjectionStereo);
				return;
			}
			int num = 0;
			if (xr.enabled)
			{
				num = xr.multipassId;
			}
			material.SetMatrix(PostProcessPass.k_ShaderPropertyId_PrevViewProjM, motionVectorsPersistentData.previousViewProjectionStereo[num]);
			material.SetMatrix(PostProcessPass.k_ShaderPropertyId_ViewProjM, motionVectorsPersistentData.viewProjectionStereo[num]);
		}

		private unsafe void DoMotionBlur(CommandBuffer cmd, RTHandle source, RTHandle destination, RTHandle motionVectors, ref CameraData cameraData)
		{
			Material cameraMotionBlur = this.m_Materials.cameraMotionBlur;
			PostProcessPass.UpdateMotionBlurMatrices(ref cameraMotionBlur, *cameraData.camera, cameraData.xr);
			cameraMotionBlur.SetFloat("_Intensity", this.m_MotionBlur.intensity.value);
			cameraMotionBlur.SetFloat("_Clamp", this.m_MotionBlur.clamp.value);
			int num = (int)this.m_MotionBlur.quality.value;
			if (this.m_MotionBlur.mode.value == MotionBlurMode.CameraAndObjects)
			{
				num += 3;
				cameraMotionBlur.SetTexture("_MotionVectorTexture", motionVectors);
			}
			PostProcessUtils.SetSourceSize(cmd, source);
			CoreUtils.SetKeyword(cameraMotionBlur, "_ENABLE_ALPHA_OUTPUT", *cameraData.isAlphaOutputEnabled);
			Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, cameraMotionBlur, num);
		}

		private void DoPaniniProjection(Camera camera, CommandBuffer cmd, RTHandle source, RTHandle destination)
		{
			float value = this.m_PaniniProjection.distance.value;
			Vector2 vector = this.CalcViewExtents(camera, this.m_Descriptor.width, this.m_Descriptor.height);
			Vector2 vector2 = this.CalcCropExtents(camera, value, this.m_Descriptor.width, this.m_Descriptor.height);
			float a = vector2.x / vector.x;
			float b = vector2.y / vector.y;
			float value2 = Mathf.Min(a, b);
			float num = value;
			float w = Mathf.Lerp(1f, Mathf.Clamp01(value2), this.m_PaniniProjection.cropToFit.value);
			Material paniniProjection = this.m_Materials.paniniProjection;
			paniniProjection.SetVector(PostProcessPass.ShaderConstants._Params, new Vector4(vector.x, vector.y, num, w));
			paniniProjection.EnableKeyword((1f - Mathf.Abs(num) > float.Epsilon) ? "_GENERIC" : "_UNIT_DISTANCE");
			Blitter.BlitCameraTexture(cmd, source, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, paniniProjection, 0);
		}

		private Vector2 CalcViewExtents(Camera camera, int width, int height)
		{
			float num = camera.fieldOfView * 0.017453292f;
			float num2 = (float)width / (float)height;
			float num3 = Mathf.Tan(0.5f * num);
			return new Vector2(num2 * num3, num3);
		}

		private Vector2 CalcCropExtents(Camera camera, float d, int width, int height)
		{
			float num = 1f + d;
			Vector2 vector = this.CalcViewExtents(camera, width, height);
			float num2 = Mathf.Sqrt(vector.x * vector.x + 1f);
			float num3 = 1f / num2;
			float num4 = num3 + d;
			return vector * num3 * (num / num4);
		}

		private void SetupBloom(CommandBuffer cmd, RTHandle source, Material uberMaterial, bool enableAlphaOutput)
		{
			BloomDownscaleMode value = this.m_Bloom.downscale.value;
			int num;
			if (value != BloomDownscaleMode.Half)
			{
				if (value != BloomDownscaleMode.Quarter)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = 2;
			}
			else
			{
				num = 1;
			}
			int num2 = Mathf.Max(1, this.m_Descriptor.width >> num);
			int num3 = Mathf.Max(1, this.m_Descriptor.height >> num);
			int num4 = Mathf.Clamp(Mathf.FloorToInt(Mathf.Log((float)Mathf.Max(num2, num3), 2f) - 1f), 1, this.m_Bloom.maxIterations.value);
			float value2 = this.m_Bloom.clamp.value;
			float num5 = Mathf.GammaToLinearSpace(this.m_Bloom.threshold.value);
			float w = num5 * 0.5f;
			float x = Mathf.Lerp(0.05f, 0.95f, this.m_Bloom.scatter.value);
			Material bloom = this.m_Materials.bloom;
			bloom.SetVector(PostProcessPass.ShaderConstants._Params, new Vector4(x, value2, num5, w));
			CoreUtils.SetKeyword(bloom, "_BLOOM_HQ", this.m_Bloom.highQualityFiltering.value);
			CoreUtils.SetKeyword(bloom, "_ENABLE_ALPHA_OUTPUT", enableAlphaOutput);
			RenderTextureDescriptor compatibleDescriptor = this.GetCompatibleDescriptor(num2, num3, this.m_DefaultColorFormat, GraphicsFormat.None);
			for (int i = 0; i < num4; i++)
			{
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_BloomMipUp[i], compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, this.m_BloomMipUpName[i]);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_BloomMipDown[i], compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, this.m_BloomMipDownName[i]);
				compatibleDescriptor.width = Mathf.Max(1, compatibleDescriptor.width >> 1);
				compatibleDescriptor.height = Mathf.Max(1, compatibleDescriptor.height >> 1);
			}
			Blitter.BlitCameraTexture(cmd, source, this.m_BloomMipDown[0], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloom, 0);
			RTHandle source2 = this.m_BloomMipDown[0];
			for (int j = 1; j < num4; j++)
			{
				Blitter.BlitCameraTexture(cmd, source2, this.m_BloomMipUp[j], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloom, 1);
				Blitter.BlitCameraTexture(cmd, this.m_BloomMipUp[j], this.m_BloomMipDown[j], RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloom, 2);
				source2 = this.m_BloomMipDown[j];
			}
			for (int k = num4 - 2; k >= 0; k--)
			{
				RTHandle handle = (k == num4 - 2) ? this.m_BloomMipDown[k + 1] : this.m_BloomMipUp[k + 1];
				RTHandle source3 = this.m_BloomMipDown[k];
				RTHandle destination = this.m_BloomMipUp[k];
				cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._SourceTexLowMip, handle);
				Blitter.BlitCameraTexture(cmd, source3, destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, bloom, 3);
			}
			Color color = this.m_Bloom.tint.value.linear;
			float num6 = ColorUtils.Luminance(color);
			color = ((num6 > 0f) ? (color * (1f / num6)) : Color.white);
			Vector4 value3 = new Vector4(this.m_Bloom.intensity.value, color.r, color.g, color.b);
			uberMaterial.SetVector(PostProcessPass.ShaderConstants._Bloom_Params, value3);
			cmd.SetGlobalTexture(PostProcessPass.ShaderConstants._Bloom_Texture, this.m_BloomMipUp[0]);
			Texture texture = (this.m_Bloom.dirtTexture.value == null) ? Texture2D.blackTexture : this.m_Bloom.dirtTexture.value;
			float num7 = (float)texture.width / (float)texture.height;
			float num8 = (float)this.m_Descriptor.width / (float)this.m_Descriptor.height;
			Vector4 vector = new Vector4(1f, 1f, 0f, 0f);
			float value4 = this.m_Bloom.dirtIntensity.value;
			if (num7 > num8)
			{
				vector.x = num8 / num7;
				vector.z = (1f - vector.x) * 0.5f;
			}
			else if (num8 > num7)
			{
				vector.y = num7 / num8;
				vector.w = (1f - vector.y) * 0.5f;
			}
			uberMaterial.SetVector(PostProcessPass.ShaderConstants._LensDirt_Params, vector);
			uberMaterial.SetFloat(PostProcessPass.ShaderConstants._LensDirt_Intensity, value4);
			uberMaterial.SetTexture(PostProcessPass.ShaderConstants._LensDirt_Texture, texture);
			if (this.m_Bloom.highQualityFiltering.value)
			{
				uberMaterial.EnableKeyword((value4 > 0f) ? "_BLOOM_HQ_DIRT" : "_BLOOM_HQ");
				return;
			}
			uberMaterial.EnableKeyword((value4 > 0f) ? "_BLOOM_LQ_DIRT" : "_BLOOM_LQ");
		}

		private void SetupLensDistortion(Material material, bool isSceneView)
		{
			float b = 1.6f * Mathf.Max(Mathf.Abs(this.m_LensDistortion.intensity.value * 100f), 1f);
			float num = 0.017453292f * Mathf.Min(160f, b);
			float y = 2f * Mathf.Tan(num * 0.5f);
			Vector2 vector = this.m_LensDistortion.center.value * 2f - Vector2.one;
			Vector4 value = new Vector4(vector.x, vector.y, Mathf.Max(this.m_LensDistortion.xMultiplier.value, 0.0001f), Mathf.Max(this.m_LensDistortion.yMultiplier.value, 0.0001f));
			Vector4 value2 = new Vector4((this.m_LensDistortion.intensity.value >= 0f) ? num : (1f / num), y, 1f / this.m_LensDistortion.scale.value, this.m_LensDistortion.intensity.value * 100f);
			material.SetVector(PostProcessPass.ShaderConstants._Distortion_Params1, value);
			material.SetVector(PostProcessPass.ShaderConstants._Distortion_Params2, value2);
			if (this.m_LensDistortion.IsActive() && !isSceneView)
			{
				material.EnableKeyword("_DISTORTION");
			}
		}

		private void SetupChromaticAberration(Material material)
		{
			material.SetFloat(PostProcessPass.ShaderConstants._Chroma_Params, this.m_ChromaticAberration.intensity.value * 0.05f);
			if (this.m_ChromaticAberration.IsActive())
			{
				material.EnableKeyword("_CHROMATIC_ABERRATION");
			}
		}

		private void SetupVignette(Material material, XRPass xrPass, int width, int height)
		{
			Color value = this.m_Vignette.color.value;
			Vector2 vector = this.m_Vignette.center.value;
			float num = (float)width / (float)height;
			if (xrPass != null && xrPass.enabled)
			{
				if (xrPass.singlePassEnabled)
				{
					material.SetVector(PostProcessPass.ShaderConstants._Vignette_ParamsXR, xrPass.ApplyXRViewCenterOffset(vector));
				}
				else
				{
					vector = xrPass.ApplyXRViewCenterOffset(vector);
				}
			}
			Vector4 value2 = new Vector4(value.r, value.g, value.b, this.m_Vignette.rounded.value ? num : 1f);
			Vector4 value3 = new Vector4(vector.x, vector.y, this.m_Vignette.intensity.value * 3f, this.m_Vignette.smoothness.value * 5f);
			material.SetVector(PostProcessPass.ShaderConstants._Vignette_Params1, value2);
			material.SetVector(PostProcessPass.ShaderConstants._Vignette_Params2, value3);
		}

		private unsafe void SetupColorGrading(CommandBuffer cmd, ref RenderingData renderingData, Material material)
		{
			bool flag = *renderingData.postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;
			int num = *renderingData.postProcessingData.lutSize;
			int num2 = num * num;
			float w = Mathf.Pow(2f, this.m_ColorAdjustments.postExposure.value);
			material.SetTexture(PostProcessPass.ShaderConstants._InternalLut, this.m_InternalLut);
			material.SetVector(PostProcessPass.ShaderConstants._Lut_Params, new Vector4(1f / (float)num2, 1f / (float)num, (float)num - 1f, w));
			material.SetTexture(PostProcessPass.ShaderConstants._UserLut, this.m_ColorLookup.texture.value);
			material.SetVector(PostProcessPass.ShaderConstants._UserLut_Params, (!this.m_ColorLookup.IsActive()) ? Vector4.zero : new Vector4(1f / (float)this.m_ColorLookup.texture.value.width, 1f / (float)this.m_ColorLookup.texture.value.height, (float)this.m_ColorLookup.texture.value.height - 1f, this.m_ColorLookup.contribution.value));
			if (flag)
			{
				material.EnableKeyword("_HDR_GRADING");
				return;
			}
			TonemappingMode value = this.m_Tonemapping.mode.value;
			if (value == TonemappingMode.Neutral)
			{
				material.EnableKeyword("_TONEMAP_NEUTRAL");
				return;
			}
			if (value != TonemappingMode.ACES)
			{
				return;
			}
			material.EnableKeyword("_TONEMAP_ACES");
		}

		private void SetupGrain(UniversalCameraData cameraData, Material material)
		{
			if (!this.m_HasFinalPass && this.m_FilmGrain.IsActive())
			{
				material.EnableKeyword("_FILM_GRAIN");
				PostProcessUtils.ConfigureFilmGrain(this.m_Data, this.m_FilmGrain, cameraData.pixelWidth, cameraData.pixelHeight, material);
			}
		}

		private void SetupDithering(UniversalCameraData cameraData, Material material)
		{
			if (!this.m_HasFinalPass && cameraData.isDitheringEnabled)
			{
				material.EnableKeyword("_DITHERING");
				this.m_DitheringTextureIndex = PostProcessUtils.ConfigureDithering(this.m_Data, this.m_DitheringTextureIndex, cameraData.pixelWidth, cameraData.pixelHeight, material);
			}
		}

		private void SetupHDROutput(HDROutputUtils.HDRDisplayInformation hdrDisplayInformation, ColorGamut hdrDisplayColorGamut, Material material, HDROutputUtils.Operation hdrOperations, bool rendersOverlayUI)
		{
			Vector4 value;
			UniversalRenderPipeline.GetHDROutputLuminanceParameters(hdrDisplayInformation, hdrDisplayColorGamut, this.m_Tonemapping, out value);
			material.SetVector(ShaderPropertyId.hdrOutputLuminanceParams, value);
			HDROutputUtils.ConfigureHDROutput(material, hdrDisplayColorGamut, hdrOperations);
			CoreUtils.SetKeyword(material, "_HDR_OVERLAY", rendersOverlayUI);
		}

		private unsafe void RenderFinalPass(CommandBuffer cmd, ref RenderingData renderingData)
		{
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			Material finalPass = this.m_Materials.finalPass;
			finalPass.shaderKeywords = null;
			PostProcessUtils.SetSourceSize(cmd, universalCameraData.renderer.cameraColorTargetHandle);
			this.SetupGrain(renderingData.cameraData.universalCameraData, finalPass);
			this.SetupDithering(renderingData.cameraData.universalCameraData, finalPass);
			if (this.RequireSRGBConversionBlitToBackBuffer(renderingData.cameraData.requireSrgbConversion))
			{
				finalPass.EnableKeyword("_LINEAR_TO_SRGB_CONVERSION");
			}
			HDROutputUtils.Operation operation = HDROutputUtils.Operation.None;
			bool flag = this.RequireHDROutput(renderingData.cameraData.universalCameraData);
			if (flag)
			{
				operation = (this.m_EnableColorEncodingIfNeeded ? HDROutputUtils.Operation.ColorEncoding : HDROutputUtils.Operation.None);
				if (!universalCameraData.postProcessEnabled)
				{
					operation |= HDROutputUtils.Operation.ColorConversion;
				}
				this.SetupHDROutput(universalCameraData.hdrDisplayInformation, universalCameraData.hdrDisplayColorGamut, finalPass, operation, universalCameraData.rendersOverlayUI);
			}
			CoreUtils.SetKeyword(finalPass, "_ENABLE_ALPHA_OUTPUT", universalCameraData.isAlphaOutputEnabled);
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			bool flag2 = activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget);
			if (this.m_UseSwapBuffer)
			{
				this.m_Source = universalCameraData.renderer.GetCameraColorBackBuffer(cmd);
			}
			RTHandle source = this.m_Source;
			RenderBufferLoadAction loadAction = universalCameraData.isDefaultViewport ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
			bool flag3 = universalCameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing;
			bool flag4 = universalCameraData.imageScalingMode == ImageScalingMode.Upscaling && universalCameraData.upscalingFilter == ImageUpscalingFilter.FSR;
			bool flag5 = universalCameraData.IsTemporalAAEnabled() && universalCameraData.taaSettings.contrastAdaptiveSharpening > 0f && !flag4;
			bool isAlphaOutputEnabled = universalCameraData.isAlphaOutputEnabled;
			if (universalCameraData.imageScalingMode != ImageScalingMode.None)
			{
				bool flag6 = flag3 || flag4;
				RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
				cameraTargetDescriptor.msaaSamples = 1;
				cameraTargetDescriptor.depthStencilFormat = GraphicsFormat.None;
				if (!flag)
				{
					cameraTargetDescriptor.graphicsFormat = UniversalRenderPipeline.MakeUnormRenderTextureGraphicsFormat();
				}
				this.m_Materials.scalingSetup.shaderKeywords = null;
				if (flag6)
				{
					if (flag)
					{
						this.SetupHDROutput(universalCameraData.hdrDisplayInformation, universalCameraData.hdrDisplayColorGamut, this.m_Materials.scalingSetup, operation, universalCameraData.rendersOverlayUI);
					}
					if (flag3)
					{
						this.m_Materials.scalingSetup.EnableKeyword("_FXAA");
					}
					if (flag4)
					{
						this.m_Materials.scalingSetup.EnableKeyword(operation.HasFlag(HDROutputUtils.Operation.ColorEncoding) ? "_GAMMA_20_AND_HDR_INPUT" : "_GAMMA_20");
					}
					if (isAlphaOutputEnabled)
					{
						this.m_Materials.scalingSetup.EnableKeyword("_ENABLE_ALPHA_OUTPUT");
					}
					RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_ScalingSetupTarget, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_ScalingSetupTexture");
					Blitter.BlitCameraTexture(cmd, this.m_Source, this.m_ScalingSetupTarget, loadAction, RenderBufferStoreAction.Store, this.m_Materials.scalingSetup, 0);
					source = this.m_ScalingSetupTarget;
				}
				ImageScalingMode imageScalingMode = universalCameraData.imageScalingMode;
				if (imageScalingMode != ImageScalingMode.Upscaling)
				{
					if (imageScalingMode == ImageScalingMode.Downscaling)
					{
						flag5 = false;
					}
				}
				else
				{
					switch (universalCameraData.upscalingFilter)
					{
					case ImageUpscalingFilter.Point:
						if (!flag5)
						{
							finalPass.EnableKeyword("_POINT_SAMPLING");
						}
						break;
					case ImageUpscalingFilter.FSR:
					{
						this.m_Materials.easu.shaderKeywords = null;
						RenderTextureDescriptor cameraTargetDescriptor2 = universalCameraData.cameraTargetDescriptor;
						cameraTargetDescriptor2.msaaSamples = 1;
						cameraTargetDescriptor2.depthStencilFormat = GraphicsFormat.None;
						cameraTargetDescriptor2.width = universalCameraData.pixelWidth;
						cameraTargetDescriptor2.height = universalCameraData.pixelHeight;
						RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_UpscaledTarget, cameraTargetDescriptor2, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_UpscaledTexture");
						Vector2 vector = new Vector2((float)universalCameraData.cameraTargetDescriptor.width, (float)universalCameraData.cameraTargetDescriptor.height);
						Vector2 outputImageSizeInPixels = new Vector2((float)universalCameraData.pixelWidth, (float)universalCameraData.pixelHeight);
						FSRUtils.SetEasuConstants(cmd, vector, vector, outputImageSizeInPixels);
						if (isAlphaOutputEnabled)
						{
							CoreUtils.SetKeyword(this.m_Materials.easu, "_ENABLE_ALPHA_OUTPUT", isAlphaOutputEnabled);
						}
						Blitter.BlitCameraTexture(cmd, source, this.m_UpscaledTarget, loadAction, RenderBufferStoreAction.Store, this.m_Materials.easu, 0);
						float sharpnessLinear = universalCameraData.fsrOverrideSharpness ? universalCameraData.fsrSharpness : 0.92f;
						if (universalCameraData.fsrSharpness > 0f)
						{
							finalPass.EnableKeyword(flag ? "_EASU_RCAS_AND_HDR_INPUT" : "_RCAS");
							FSRUtils.SetRcasConstantsLinear(cmd, sharpnessLinear);
						}
						source = this.m_UpscaledTarget;
						PostProcessUtils.SetSourceSize(cmd, this.m_UpscaledTarget);
						break;
					}
					}
				}
			}
			else if (flag3)
			{
				finalPass.EnableKeyword("_FXAA");
			}
			if (flag5)
			{
				finalPass.EnableKeyword("_RCAS");
				FSRUtils.SetRcasConstantsLinear(cmd, universalCameraData.taaSettings.contrastAdaptiveSharpening);
			}
			RenderTargetIdentifier cameraTargetIdentifier = RenderingUtils.GetCameraTargetIdentifier(ref renderingData);
			if (flag2)
			{
				Blitter.BlitCameraTexture(cmd, source, *activeDebugHandler.DebugScreenColorHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, finalPass, 0);
				universalCameraData.renderer.ConfigureCameraTarget(*activeDebugHandler.DebugScreenColorHandle, *activeDebugHandler.DebugScreenDepthHandle);
				return;
			}
			RTHandleStaticHelpers.SetRTHandleStaticWrapper(cameraTargetIdentifier);
			RTHandle s_RTHandleWrapper = RTHandleStaticHelpers.s_RTHandleWrapper;
			RenderingUtils.FinalBlit(cmd, universalCameraData, source, s_RTHandleWrapper, loadAction, RenderBufferStoreAction.Store, finalPass, 0);
		}

		private void UpdateCameraResolution(RenderGraph renderGraph, UniversalCameraData cameraData, Vector2Int newCameraTargetSize)
		{
			cameraData.cameraTargetDescriptor.width = newCameraTargetSize.x;
			cameraData.cameraTargetDescriptor.height = newCameraTargetSize.y;
			PostProcessPass.UpdateCameraResolutionPassData updateCameraResolutionPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<PostProcessPass.UpdateCameraResolutionPassData>("Update Camera Resolution", out updateCameraResolutionPassData, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 26))
			{
				updateCameraResolutionPassData.newCameraTargetSize = newCameraTargetSize;
				unsafeRenderGraphBuilder.AllowGlobalStateModification(true);
				unsafeRenderGraphBuilder.SetRenderFunc<PostProcessPass.UpdateCameraResolutionPassData>(delegate(PostProcessPass.UpdateCameraResolutionPassData data, UnsafeGraphContext ctx)
				{
					ctx.cmd.SetGlobalVector(ShaderPropertyId.screenSize, new Vector4((float)data.newCameraTargetSize.x, (float)data.newCameraTargetSize.y, 1f / (float)data.newCameraTargetSize.x, 1f / (float)data.newCameraTargetSize.y));
				});
			}
		}

		internal static TextureHandle CreateCompatibleTexture(RenderGraph renderGraph, in TextureHandle source, string name, bool clear, FilterMode filterMode)
		{
			TextureHandle textureHandle = source;
			TextureDesc descriptor = textureHandle.GetDescriptor(renderGraph);
			PostProcessPass.MakeCompatible(ref descriptor);
			descriptor.name = name;
			descriptor.clearBuffer = clear;
			descriptor.filterMode = filterMode;
			return renderGraph.CreateTexture(descriptor);
		}

		internal static TextureHandle CreateCompatibleTexture(RenderGraph renderGraph, in TextureDesc desc, string name, bool clear, FilterMode filterMode)
		{
			TextureDesc compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(desc);
			compatibleDescriptor.name = name;
			compatibleDescriptor.clearBuffer = clear;
			compatibleDescriptor.filterMode = filterMode;
			return renderGraph.CreateTexture(compatibleDescriptor);
		}

		internal static TextureDesc GetCompatibleDescriptor(TextureDesc desc, int width, int height, GraphicsFormat format)
		{
			desc.width = width;
			desc.height = height;
			desc.format = format;
			PostProcessPass.MakeCompatible(ref desc);
			return desc;
		}

		internal static TextureDesc GetCompatibleDescriptor(TextureDesc desc)
		{
			PostProcessPass.MakeCompatible(ref desc);
			return desc;
		}

		internal static void MakeCompatible(ref TextureDesc desc)
		{
			desc.msaaSamples = MSAASamples.None;
			desc.useMipMap = false;
			desc.autoGenerateMips = false;
			desc.anisoLevel = 0;
			desc.discardBuffer = false;
		}

		public void RenderStopNaN(RenderGraph renderGraph, in TextureHandle activeCameraColor, out TextureHandle stopNaNTarget)
		{
			stopNaNTarget = PostProcessPass.CreateCompatibleTexture(renderGraph, activeCameraColor, "_StopNaNsTarget", true, FilterMode.Bilinear);
			PostProcessPass.StopNaNsPassData stopNaNsPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.StopNaNsPassData>("Stop NaNs", out stopNaNsPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_StopNaNs), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 106))
			{
				stopNaNsPassData.stopNaNTarget = stopNaNTarget;
				rasterRenderGraphBuilder.SetRenderAttachment(stopNaNTarget, 0, AccessFlags.ReadWrite);
				stopNaNsPassData.sourceTexture = activeCameraColor;
				rasterRenderGraphBuilder.UseTexture(activeCameraColor, AccessFlags.Read);
				stopNaNsPassData.stopNaN = this.m_Materials.stopNaN;
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.StopNaNsPassData>(delegate(PostProcessPass.StopNaNsPassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					RTHandle rthandle = data.sourceTexture;
					Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(cmd, rthandle, v, data.stopNaN, 0);
				});
			}
		}

		public void RenderSMAA(RenderGraph renderGraph, UniversalResourceData resourceData, AntialiasingQuality antialiasingQuality, in TextureHandle source, out TextureHandle SMAATarget)
		{
			TextureDesc textureDesc = renderGraph.GetTextureDesc(source);
			SMAATarget = PostProcessPass.CreateCompatibleTexture(renderGraph, textureDesc, "_SMAATarget", true, FilterMode.Bilinear);
			textureDesc.clearColor = Color.black;
			textureDesc.clearColor.a = 0f;
			TextureDesc textureDesc2 = textureDesc;
			textureDesc2.format = this.m_SMAAEdgeFormat;
			TextureHandle textureHandle = PostProcessPass.CreateCompatibleTexture(renderGraph, textureDesc2, "_EdgeStencilTexture", true, FilterMode.Bilinear);
			TextureDesc textureDesc3 = textureDesc;
			textureDesc3.format = GraphicsFormatUtility.GetDepthStencilFormat(24);
			TextureHandle textureHandle2 = PostProcessPass.CreateCompatibleTexture(renderGraph, textureDesc3, "_EdgeTexture", true, FilterMode.Bilinear);
			TextureDesc textureDesc4 = textureDesc;
			textureDesc4.format = GraphicsFormat.R8G8B8A8_UNorm;
			TextureHandle textureHandle3 = PostProcessPass.CreateCompatibleTexture(renderGraph, textureDesc4, "_BlendTexture", true, FilterMode.Point);
			Material subpixelMorphologicalAntialiasing = this.m_Materials.subpixelMorphologicalAntialiasing;
			PostProcessPass.SMAASetupPassData smaasetupPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.SMAASetupPassData>("SMAA Material Setup", out smaasetupPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_SMAAMaterialSetup), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 169))
			{
				smaasetupPassData.metrics = new Vector4(1f / (float)textureDesc.width, 1f / (float)textureDesc.height, (float)textureDesc.width, (float)textureDesc.height);
				smaasetupPassData.areaTexture = this.m_Data.textures.smaaAreaTex;
				smaasetupPassData.searchTexture = this.m_Data.textures.smaaSearchTex;
				smaasetupPassData.stencilRef = 64f;
				smaasetupPassData.stencilMask = 64f;
				smaasetupPassData.antialiasingQuality = antialiasingQuality;
				smaasetupPassData.material = subpixelMorphologicalAntialiasing;
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.SMAASetupPassData>(delegate(PostProcessPass.SMAASetupPassData data, RasterGraphContext context)
				{
					data.material.SetVector(PostProcessPass.ShaderConstants._Metrics, data.metrics);
					data.material.SetTexture(PostProcessPass.ShaderConstants._AreaTexture, data.areaTexture);
					data.material.SetTexture(PostProcessPass.ShaderConstants._SearchTexture, data.searchTexture);
					data.material.SetFloat(PostProcessPass.ShaderConstants._StencilRef, data.stencilRef);
					data.material.SetFloat(PostProcessPass.ShaderConstants._StencilMask, data.stencilMask);
					data.material.shaderKeywords = null;
					switch (data.antialiasingQuality)
					{
					case AntialiasingQuality.Low:
						data.material.EnableKeyword("_SMAA_PRESET_LOW");
						return;
					case AntialiasingQuality.Medium:
						data.material.EnableKeyword("_SMAA_PRESET_MEDIUM");
						return;
					case AntialiasingQuality.High:
						data.material.EnableKeyword("_SMAA_PRESET_HIGH");
						return;
					default:
						return;
					}
				});
			}
			PostProcessPass.SMAAPassData smaapassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder2 = renderGraph.AddRasterRenderPass<PostProcessPass.SMAAPassData>("SMAA Edge Detection", out smaapassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_SMAAEdgeDetection), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 210))
			{
				rasterRenderGraphBuilder2.SetRenderAttachment(textureHandle, 0, AccessFlags.Write);
				smaapassData.depthStencilTexture = textureHandle2;
				rasterRenderGraphBuilder2.SetRenderAttachmentDepth(textureHandle2, AccessFlags.Write);
				smaapassData.sourceTexture = source;
				rasterRenderGraphBuilder2.UseTexture(source, AccessFlags.Read);
				IBaseRenderGraphBuilder baseRenderGraphBuilder = rasterRenderGraphBuilder2;
				TextureHandle cameraDepth = resourceData.cameraDepth;
				baseRenderGraphBuilder.UseTexture(cameraDepth, AccessFlags.Read);
				smaapassData.material = subpixelMorphologicalAntialiasing;
				rasterRenderGraphBuilder2.SetRenderFunc<PostProcessPass.SMAAPassData>(delegate(PostProcessPass.SMAAPassData data, RasterGraphContext context)
				{
					Material material = data.material;
					RasterCommandBuffer cmd = context.cmd;
					RTHandle rthandle = data.sourceTexture;
					Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(cmd, rthandle, v, material, 0);
				});
			}
			PostProcessPass.SMAAPassData smaapassData2;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder3 = renderGraph.AddRasterRenderPass<PostProcessPass.SMAAPassData>("SMAA Blend weights", out smaapassData2, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_SMAABlendWeight), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 232))
			{
				rasterRenderGraphBuilder3.SetRenderAttachment(textureHandle3, 0, AccessFlags.Write);
				smaapassData2.depthStencilTexture = textureHandle2;
				rasterRenderGraphBuilder3.SetRenderAttachmentDepth(textureHandle2, AccessFlags.Read);
				smaapassData2.sourceTexture = textureHandle;
				rasterRenderGraphBuilder3.UseTexture(textureHandle, AccessFlags.Read);
				smaapassData2.material = subpixelMorphologicalAntialiasing;
				rasterRenderGraphBuilder3.SetRenderFunc<PostProcessPass.SMAAPassData>(delegate(PostProcessPass.SMAAPassData data, RasterGraphContext context)
				{
					Material material = data.material;
					RasterCommandBuffer cmd = context.cmd;
					RTHandle rthandle = data.sourceTexture;
					Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(cmd, rthandle, v, material, 1);
				});
			}
			PostProcessPass.SMAAPassData smaapassData3;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder4 = renderGraph.AddRasterRenderPass<PostProcessPass.SMAAPassData>("SMAA Neighborhood blending", out smaapassData3, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_SMAANeighborhoodBlend), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 253))
			{
				rasterRenderGraphBuilder4.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder4.SetRenderAttachment(SMAATarget, 0, AccessFlags.Write);
				smaapassData3.sourceTexture = source;
				rasterRenderGraphBuilder4.UseTexture(source, AccessFlags.Read);
				smaapassData3.blendTexture = textureHandle3;
				rasterRenderGraphBuilder4.UseTexture(textureHandle3, AccessFlags.Read);
				smaapassData3.material = subpixelMorphologicalAntialiasing;
				rasterRenderGraphBuilder4.SetRenderFunc<PostProcessPass.SMAAPassData>(delegate(PostProcessPass.SMAAPassData data, RasterGraphContext context)
				{
					Material material = data.material;
					RasterCommandBuffer cmd = context.cmd;
					RTHandle rthandle = data.sourceTexture;
					material.SetTexture(PostProcessPass.ShaderConstants._BlendTexture, data.blendTexture);
					Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(cmd, rthandle, v, material, 2);
				});
			}
		}

		public void UberPostSetupBloomPass(RenderGraph rendergraph, Material uberMaterial, in TextureDesc srcDesc)
		{
			using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_UberPostSetupBloomPass)))
			{
				Color color = this.m_Bloom.tint.value.linear;
				float num = ColorUtils.Luminance(color);
				color = ((num > 0f) ? (color * (1f / num)) : Color.white);
				Vector4 value = new Vector4(this.m_Bloom.intensity.value, color.r, color.g, color.b);
				Texture texture = (this.m_Bloom.dirtTexture.value == null) ? Texture2D.blackTexture : this.m_Bloom.dirtTexture.value;
				float num2 = (float)texture.width / (float)texture.height;
				float num3 = (float)srcDesc.width / (float)srcDesc.height;
				Vector4 vector = new Vector4(1f, 1f, 0f, 0f);
				float value2 = this.m_Bloom.dirtIntensity.value;
				if (num2 > num3)
				{
					vector.x = num3 / num2;
					vector.z = (1f - vector.x) * 0.5f;
				}
				else if (num3 > num2)
				{
					vector.y = num2 / num3;
					vector.w = (1f - vector.y) * 0.5f;
				}
				bool value3 = this.m_Bloom.highQualityFiltering.value;
				uberMaterial.SetVector(PostProcessPass.ShaderConstants._Bloom_Params, value);
				uberMaterial.SetVector(PostProcessPass.ShaderConstants._LensDirt_Params, vector);
				uberMaterial.SetFloat(PostProcessPass.ShaderConstants._LensDirt_Intensity, value2);
				uberMaterial.SetTexture(PostProcessPass.ShaderConstants._LensDirt_Texture, texture);
				if (value3)
				{
					uberMaterial.EnableKeyword((value2 > 0f) ? "_BLOOM_HQ_DIRT" : "_BLOOM_HQ");
				}
				else
				{
					uberMaterial.EnableKeyword((value2 > 0f) ? "_BLOOM_LQ_DIRT" : "_BLOOM_LQ");
				}
			}
		}

		public Vector2Int CalcBloomResolution(Bloom bloom, in TextureDesc bloomSourceDesc)
		{
			BloomDownscaleMode value = this.m_Bloom.downscale.value;
			int num;
			if (value != BloomDownscaleMode.Half)
			{
				if (value != BloomDownscaleMode.Quarter)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = 2;
			}
			else
			{
				num = 1;
			}
			int x = Mathf.Max(1, bloomSourceDesc.width >> num);
			int y = Mathf.Max(1, bloomSourceDesc.height >> num);
			return new Vector2Int(x, y);
		}

		public int CalcBloomMipCount(Bloom bloom, Vector2Int bloomResolution)
		{
			return Mathf.Clamp(Mathf.FloorToInt(Mathf.Log((float)Mathf.Max(bloomResolution.x, bloomResolution.y), 2f) - 1f), 1, this.m_Bloom.maxIterations.value);
		}

		public void RenderBloomTexture(RenderGraph renderGraph, in TextureHandle source, out TextureHandle destination, bool enableAlphaOutput)
		{
			TextureHandle textureHandle = source;
			TextureDesc descriptor = textureHandle.GetDescriptor(renderGraph);
			Vector2Int bloomResolution = this.CalcBloomResolution(this.m_Bloom, descriptor);
			int num = this.CalcBloomMipCount(this.m_Bloom, bloomResolution);
			int num2 = bloomResolution.x;
			int num3 = bloomResolution.y;
			using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_BloomSetup)))
			{
				float value = this.m_Bloom.clamp.value;
				float num4 = Mathf.GammaToLinearSpace(this.m_Bloom.threshold.value);
				float w = num4 * 0.5f;
				float x = Mathf.Lerp(0.05f, 0.95f, this.m_Bloom.scatter.value);
				PostProcessPass.BloomMaterialParams bloomMaterialParams = default(PostProcessPass.BloomMaterialParams);
				bloomMaterialParams.parameters = new Vector4(x, value, num4, w);
				bloomMaterialParams.highQualityFiltering = this.m_Bloom.highQualityFiltering.value;
				bloomMaterialParams.enableAlphaOutput = enableAlphaOutput;
				Material bloom = this.m_Materials.bloom;
				bool flag = !this.m_BloomParamsPrev.Equals(ref bloomMaterialParams);
				bool flag2 = bloom.HasProperty(PostProcessPass.ShaderConstants._Params);
				if (flag || !flag2)
				{
					bloom.SetVector(PostProcessPass.ShaderConstants._Params, bloomMaterialParams.parameters);
					CoreUtils.SetKeyword(bloom, "_BLOOM_HQ", bloomMaterialParams.highQualityFiltering);
					CoreUtils.SetKeyword(bloom, "_ENABLE_ALPHA_OUTPUT", bloomMaterialParams.enableAlphaOutput);
					for (uint num5 = 0U; num5 < 16U; num5 += 1U)
					{
						Material material = this.m_Materials.bloomUpsample[(int)num5];
						material.SetVector(PostProcessPass.ShaderConstants._Params, bloomMaterialParams.parameters);
						CoreUtils.SetKeyword(material, "_BLOOM_HQ", bloomMaterialParams.highQualityFiltering);
						CoreUtils.SetKeyword(material, "_ENABLE_ALPHA_OUTPUT", bloomMaterialParams.enableAlphaOutput);
					}
					this.m_BloomParamsPrev = bloomMaterialParams;
				}
				TextureDesc compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(descriptor, num2, num3, this.m_DefaultColorFormat);
				this._BloomMipDown[0] = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, this.m_BloomMipDownName[0], false, FilterMode.Bilinear);
				this._BloomMipUp[0] = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, this.m_BloomMipUpName[0], false, FilterMode.Bilinear);
				for (int i = 1; i < num; i++)
				{
					num2 = Mathf.Max(1, num2 >> 1);
					num3 = Mathf.Max(1, num3 >> 1);
					TextureHandle[] bloomMipDown = this._BloomMipDown;
					int num6 = i;
					ref TextureHandle ptr = ref this._BloomMipUp[i];
					compatibleDescriptor.width = num2;
					compatibleDescriptor.height = num3;
					bloomMipDown[num6] = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, this.m_BloomMipDownName[i], false, FilterMode.Bilinear);
					ptr = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, this.m_BloomMipUpName[i], false, FilterMode.Bilinear);
				}
			}
			PostProcessPass.BloomPassData bloomPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<PostProcessPass.BloomPassData>("Blit Bloom Mipmaps", out bloomPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.Bloom), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 465))
			{
				bloomPassData.mipCount = num;
				bloomPassData.material = this.m_Materials.bloom;
				bloomPassData.upsampleMaterials = this.m_Materials.bloomUpsample;
				bloomPassData.sourceTexture = source;
				bloomPassData.bloomMipDown = this._BloomMipDown;
				bloomPassData.bloomMipUp = this._BloomMipUp;
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				unsafeRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				for (int j = 0; j < num; j++)
				{
					unsafeRenderGraphBuilder.UseTexture(this._BloomMipDown[j], AccessFlags.ReadWrite);
					unsafeRenderGraphBuilder.UseTexture(this._BloomMipUp[j], AccessFlags.ReadWrite);
				}
				unsafeRenderGraphBuilder.SetRenderFunc<PostProcessPass.BloomPassData>(delegate(PostProcessPass.BloomPassData data, UnsafeGraphContext context)
				{
					CommandBuffer nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
					Material material2 = data.material;
					int mipCount = data.mipCount;
					RenderBufferLoadAction loadAction = RenderBufferLoadAction.DontCare;
					RenderBufferStoreAction storeAction = RenderBufferStoreAction.Store;
					using (new ProfilingScope(nativeCommandBuffer, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_BloomPrefilter)))
					{
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.sourceTexture, data.bloomMipDown[0], loadAction, storeAction, material2, 0);
					}
					using (new ProfilingScope(nativeCommandBuffer, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_BloomDownsample)))
					{
						TextureHandle texture = data.bloomMipDown[0];
						for (int k = 1; k < mipCount; k++)
						{
							TextureHandle textureHandle2 = data.bloomMipDown[k];
							TextureHandle texture2 = data.bloomMipUp[k];
							Blitter.BlitCameraTexture(nativeCommandBuffer, texture, texture2, loadAction, storeAction, material2, 1);
							Blitter.BlitCameraTexture(nativeCommandBuffer, texture2, textureHandle2, loadAction, storeAction, material2, 2);
							texture = textureHandle2;
						}
					}
					using (new ProfilingScope(nativeCommandBuffer, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_BloomUpsample)))
					{
						for (int l = mipCount - 2; l >= 0; l--)
						{
							TextureHandle texture3 = (l == mipCount - 2) ? data.bloomMipDown[l + 1] : data.bloomMipUp[l + 1];
							TextureHandle texture4 = data.bloomMipDown[l];
							TextureHandle texture5 = data.bloomMipUp[l];
							Material material3 = data.upsampleMaterials[l];
							material3.SetTexture(PostProcessPass.ShaderConstants._SourceTexLowMip, texture3);
							Blitter.BlitCameraTexture(nativeCommandBuffer, texture4, texture5, loadAction, storeAction, material3, 3);
						}
					}
				});
				destination = bloomPassData.bloomMipUp[0];
			}
		}

		public void RenderDoF(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, in TextureHandle source, out TextureHandle destination)
		{
			Material material = (this.m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian) ? this.m_Materials.gaussianDepthOfField : this.m_Materials.bokehDepthOfField;
			destination = PostProcessPass.CreateCompatibleTexture(renderGraph, source, "_DoFTarget", true, FilterMode.Bilinear);
			CoreUtils.SetKeyword(material, "_ENABLE_ALPHA_OUTPUT", cameraData.isAlphaOutputEnabled);
			if (this.m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian)
			{
				this.RenderDoFGaussian(renderGraph, resourceData, cameraData, source, destination, ref material);
				return;
			}
			if (this.m_DepthOfField.mode.value == DepthOfFieldMode.Bokeh)
			{
				this.RenderDoFBokeh(renderGraph, resourceData, cameraData, source, destination, ref material);
			}
		}

		public void RenderDoFGaussian(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, in TextureHandle source, TextureHandle destination, ref Material dofMaterial)
		{
			TextureHandle textureHandle = source;
			TextureDesc descriptor = textureHandle.GetDescriptor(renderGraph);
			Material material = dofMaterial;
			int num = 2;
			int num2 = descriptor.width / num;
			int height = descriptor.height / num;
			TextureDesc compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(descriptor, descriptor.width, descriptor.height, this.m_GaussianCoCFormat);
			TextureHandle fullCoCTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, "_FullCoCTexture", true, FilterMode.Bilinear);
			TextureDesc compatibleDescriptor2 = PostProcessPass.GetCompatibleDescriptor(descriptor, num2, height, this.m_GaussianCoCFormat);
			TextureHandle halfCoCTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor2, "_HalfCoCTexture", true, FilterMode.Bilinear);
			TextureDesc compatibleDescriptor3 = PostProcessPass.GetCompatibleDescriptor(descriptor, num2, height, this.m_DefaultColorFormat);
			TextureHandle pingTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor3, "_PingTexture", true, FilterMode.Bilinear);
			TextureDesc compatibleDescriptor4 = PostProcessPass.GetCompatibleDescriptor(descriptor, num2, height, this.m_DefaultColorFormat);
			TextureHandle pongTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor4, "_PongTexture", true, FilterMode.Bilinear);
			PostProcessPass.DoFGaussianPassData doFGaussianPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<PostProcessPass.DoFGaussianPassData>("Depth of Field - Gaussian", out doFGaussianPassData, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 605))
			{
				float value = this.m_DepthOfField.gaussianStart.value;
				float y = Mathf.Max(value, this.m_DepthOfField.gaussianEnd.value);
				float num3 = this.m_DepthOfField.gaussianMaxRadius.value * ((float)num2 / 1080f);
				num3 = Mathf.Min(num3, 2f);
				doFGaussianPassData.downsample = num;
				doFGaussianPassData.cocParams = new Vector3(value, y, num3);
				doFGaussianPassData.highQualitySamplingValue = this.m_DepthOfField.highQualitySampling.value;
				doFGaussianPassData.material = material;
				doFGaussianPassData.materialCoC = this.m_Materials.gaussianDepthOfFieldCoC;
				doFGaussianPassData.sourceTexture = source;
				unsafeRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				doFGaussianPassData.depthTexture = resourceData.cameraDepthTexture;
				IBaseRenderGraphBuilder baseRenderGraphBuilder = unsafeRenderGraphBuilder;
				textureHandle = resourceData.cameraDepthTexture;
				baseRenderGraphBuilder.UseTexture(textureHandle, AccessFlags.Read);
				doFGaussianPassData.fullCoCTexture = fullCoCTexture;
				unsafeRenderGraphBuilder.UseTexture(fullCoCTexture, AccessFlags.ReadWrite);
				doFGaussianPassData.halfCoCTexture = halfCoCTexture;
				unsafeRenderGraphBuilder.UseTexture(halfCoCTexture, AccessFlags.ReadWrite);
				doFGaussianPassData.pingTexture = pingTexture;
				unsafeRenderGraphBuilder.UseTexture(pingTexture, AccessFlags.ReadWrite);
				doFGaussianPassData.pongTexture = pongTexture;
				unsafeRenderGraphBuilder.UseTexture(pongTexture, AccessFlags.ReadWrite);
				doFGaussianPassData.destination = destination;
				unsafeRenderGraphBuilder.UseTexture(destination, AccessFlags.Write);
				unsafeRenderGraphBuilder.SetRenderFunc<PostProcessPass.DoFGaussianPassData>(delegate(PostProcessPass.DoFGaussianPassData data, UnsafeGraphContext context)
				{
					Material material2 = data.material;
					Material materialCoC = data.materialCoC;
					CommandBuffer nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
					RTHandle rthandle = data.sourceTexture;
					RTHandle destination2 = data.destination;
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_SetupDoF)))
					{
						material2.SetVector(PostProcessPass.ShaderConstants._CoCParams, data.cocParams);
						CoreUtils.SetKeyword(material2, "_HIGH_QUALITY_SAMPLING", data.highQualitySamplingValue);
						materialCoC.SetVector(PostProcessPass.ShaderConstants._CoCParams, data.cocParams);
						CoreUtils.SetKeyword(materialCoC, "_HIGH_QUALITY_SAMPLING", data.highQualitySamplingValue);
						PostProcessUtils.SetSourceSize(nativeCommandBuffer, data.sourceTexture);
						material2.SetVector(PostProcessPass.ShaderConstants._DownSampleScaleFactor, new Vector4(1f / (float)data.downsample, 1f / (float)data.downsample, (float)data.downsample, (float)data.downsample));
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFComputeCOC)))
					{
						material2.SetTexture(PostProcessPass.s_CameraDepthTextureID, data.depthTexture);
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.sourceTexture, data.fullCoCTexture, data.materialCoC, 0);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFDownscalePrefilter)))
					{
						material2.SetTexture(PostProcessPass.ShaderConstants._FullCoCTexture, data.fullCoCTexture);
						data.multipleRenderTargets[0] = data.halfCoCTexture;
						data.multipleRenderTargets[1] = data.pingTexture;
						CoreUtils.SetRenderTarget(nativeCommandBuffer, data.multipleRenderTargets, data.halfCoCTexture);
						Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
						Blitter.BlitTexture(nativeCommandBuffer, data.sourceTexture, v, material2, 1);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFBlurH)))
					{
						material2.SetTexture(PostProcessPass.ShaderConstants._HalfCoCTexture, data.halfCoCTexture);
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.pingTexture, data.pongTexture, material2, 2);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFBlurV)))
					{
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.pongTexture, data.pingTexture, material2, 3);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFComposite)))
					{
						material2.SetTexture(PostProcessPass.ShaderConstants._ColorTexture, data.pingTexture);
						material2.SetTexture(PostProcessPass.ShaderConstants._FullCoCTexture, data.fullCoCTexture);
						Blitter.BlitCameraTexture(nativeCommandBuffer, rthandle, destination2, material2, 4);
					}
				});
			}
		}

		public void RenderDoFBokeh(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, in TextureHandle source, in TextureHandle destination, ref Material dofMaterial)
		{
			TextureHandle textureHandle = source;
			TextureDesc descriptor = textureHandle.GetDescriptor(renderGraph);
			int num = 2;
			Material material = dofMaterial;
			int num2 = descriptor.width / num;
			int num3 = descriptor.height / num;
			TextureDesc compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(descriptor, descriptor.width, descriptor.height, GraphicsFormat.R8_UNorm);
			TextureHandle fullCoCTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, "_FullCoCTexture", true, FilterMode.Bilinear);
			TextureDesc compatibleDescriptor2 = PostProcessPass.GetCompatibleDescriptor(descriptor, num2, num3, GraphicsFormat.R16G16B16A16_SFloat);
			TextureHandle pingTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor2, "_PingTexture", true, FilterMode.Bilinear);
			TextureDesc compatibleDescriptor3 = PostProcessPass.GetCompatibleDescriptor(descriptor, num2, num3, GraphicsFormat.R16G16B16A16_SFloat);
			TextureHandle pongTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor3, "_PongTexture", true, FilterMode.Bilinear);
			PostProcessPass.DoFBokehPassData doFBokehPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<PostProcessPass.DoFBokehPassData>("Depth of Field - Bokeh", out doFBokehPassData, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 758))
			{
				float num4 = this.m_DepthOfField.focalLength.value / 1000f;
				float num5 = this.m_DepthOfField.focalLength.value / this.m_DepthOfField.aperture.value;
				float value = this.m_DepthOfField.focusDistance.value;
				float y = num5 * num4 / (value - num4);
				float maxBokehRadiusInPixels = PostProcessPass.GetMaxBokehRadiusInPixels((float)descriptor.height);
				float num6 = 1f / ((float)num2 / (float)num3);
				int hashCode = this.m_DepthOfField.GetHashCode();
				if (hashCode != this.m_BokehHash || maxBokehRadiusInPixels != this.m_BokehMaxRadius || num6 != this.m_BokehRCPAspect)
				{
					this.m_BokehHash = hashCode;
					this.m_BokehMaxRadius = maxBokehRadiusInPixels;
					this.m_BokehRCPAspect = num6;
					this.PrepareBokehKernel(maxBokehRadiusInPixels, num6);
				}
				float uvMargin = 1f / (float)descriptor.height * (float)num;
				doFBokehPassData.bokehKernel = this.m_BokehKernel;
				doFBokehPassData.downSample = num;
				doFBokehPassData.uvMargin = uvMargin;
				doFBokehPassData.cocParams = new Vector4(value, y, maxBokehRadiusInPixels, num6);
				doFBokehPassData.useFastSRGBLinearConversion = this.m_UseFastSRGBLinearConversion;
				doFBokehPassData.sourceTexture = source;
				unsafeRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				doFBokehPassData.depthTexture = resourceData.cameraDepthTexture;
				IBaseRenderGraphBuilder baseRenderGraphBuilder = unsafeRenderGraphBuilder;
				textureHandle = resourceData.cameraDepthTexture;
				baseRenderGraphBuilder.UseTexture(textureHandle, AccessFlags.Read);
				doFBokehPassData.material = material;
				doFBokehPassData.materialCoC = this.m_Materials.bokehDepthOfFieldCoC;
				doFBokehPassData.fullCoCTexture = fullCoCTexture;
				unsafeRenderGraphBuilder.UseTexture(fullCoCTexture, AccessFlags.ReadWrite);
				doFBokehPassData.pingTexture = pingTexture;
				unsafeRenderGraphBuilder.UseTexture(pingTexture, AccessFlags.ReadWrite);
				doFBokehPassData.pongTexture = pongTexture;
				unsafeRenderGraphBuilder.UseTexture(pongTexture, AccessFlags.ReadWrite);
				doFBokehPassData.destination = destination;
				unsafeRenderGraphBuilder.UseTexture(destination, AccessFlags.Write);
				unsafeRenderGraphBuilder.SetRenderFunc<PostProcessPass.DoFBokehPassData>(delegate(PostProcessPass.DoFBokehPassData data, UnsafeGraphContext context)
				{
					Material material2 = data.material;
					Material materialCoC = data.materialCoC;
					CommandBuffer nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
					RTHandle source2 = data.sourceTexture;
					RTHandle destination2 = data.destination;
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_SetupDoF)))
					{
						CoreUtils.SetKeyword(material2, "_USE_FAST_SRGB_LINEAR_CONVERSION", data.useFastSRGBLinearConversion);
						CoreUtils.SetKeyword(materialCoC, "_USE_FAST_SRGB_LINEAR_CONVERSION", data.useFastSRGBLinearConversion);
						material2.SetVector(PostProcessPass.ShaderConstants._CoCParams, data.cocParams);
						material2.SetVectorArray(PostProcessPass.ShaderConstants._BokehKernel, data.bokehKernel);
						material2.SetVector(PostProcessPass.ShaderConstants._DownSampleScaleFactor, new Vector4(1f / (float)data.downSample, 1f / (float)data.downSample, (float)data.downSample, (float)data.downSample));
						material2.SetVector(PostProcessPass.ShaderConstants._BokehConstants, new Vector4(data.uvMargin, data.uvMargin * 2f));
						PostProcessUtils.SetSourceSize(nativeCommandBuffer, data.sourceTexture);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFComputeCOC)))
					{
						material2.SetTexture(PostProcessPass.s_CameraDepthTextureID, data.depthTexture);
						Blitter.BlitCameraTexture(nativeCommandBuffer, source2, data.fullCoCTexture, material2, 0);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFDownscalePrefilter)))
					{
						material2.SetTexture(PostProcessPass.ShaderConstants._FullCoCTexture, data.fullCoCTexture);
						Blitter.BlitCameraTexture(nativeCommandBuffer, source2, data.pingTexture, material2, 1);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFBlurBokeh)))
					{
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.pingTexture, data.pongTexture, material2, 2);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFPostFilter)))
					{
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.pongTexture, data.pingTexture, material2, 3);
					}
					using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_DOFComposite)))
					{
						material2.SetTexture(PostProcessPass.ShaderConstants._DofTexture, data.pingTexture);
						Blitter.BlitCameraTexture(nativeCommandBuffer, source2, destination2, material2, 4);
					}
				});
			}
		}

		public void RenderPaniniProjection(RenderGraph renderGraph, Camera camera, in TextureHandle source, out TextureHandle destination)
		{
			destination = PostProcessPass.CreateCompatibleTexture(renderGraph, source, "_PaniniProjectionTarget", true, FilterMode.Bilinear);
			TextureHandle textureHandle = source;
			TextureDesc descriptor = textureHandle.GetDescriptor(renderGraph);
			float value = this.m_PaniniProjection.distance.value;
			Vector2 vector = this.CalcViewExtents(camera, descriptor.width, descriptor.height);
			Vector2 vector2 = this.CalcCropExtents(camera, value, descriptor.width, descriptor.height);
			float a = vector2.x / vector.x;
			float b = vector2.y / vector.y;
			float value2 = Mathf.Min(a, b);
			float num = value;
			float w = Mathf.Lerp(1f, Mathf.Clamp01(value2), this.m_PaniniProjection.cropToFit.value);
			PostProcessPass.PaniniProjectionPassData paniniProjectionPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.PaniniProjectionPassData>("Panini Projection", out paniniProjectionPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.PaniniProjection), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 899))
			{
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				paniniProjectionPassData.destinationTexture = destination;
				rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.Write);
				paniniProjectionPassData.sourceTexture = source;
				rasterRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				paniniProjectionPassData.material = this.m_Materials.paniniProjection;
				paniniProjectionPassData.paniniParams = new Vector4(vector.x, vector.y, num, w);
				paniniProjectionPassData.isPaniniGeneric = (1f - Mathf.Abs(num) > float.Epsilon);
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.PaniniProjectionPassData>(delegate(PostProcessPass.PaniniProjectionPassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					RTHandle rthandle = data.sourceTexture;
					cmd.SetGlobalVector(PostProcessPass.ShaderConstants._Params, data.paniniParams);
					data.material.EnableKeyword(data.isPaniniGeneric ? "_GENERIC" : "_UNIT_DISTANCE");
					Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(cmd, rthandle, v, data.material, 0);
				});
			}
		}

		private void RenderTemporalAA(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, ref TextureHandle source, out TextureHandle destination)
		{
			destination = PostProcessPass.CreateCompatibleTexture(renderGraph, source, "_TemporalAATarget", false, FilterMode.Bilinear);
			TextureHandle cameraDepth = resourceData.cameraDepth;
			TextureHandle motionVectorColor = resourceData.motionVectorColor;
			TemporalAA.Render(renderGraph, this.m_Materials.temporalAntialiasing, cameraData, ref source, ref cameraDepth, ref motionVectorColor, ref destination);
		}

		private void RenderSTP(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, ref TextureHandle source, out TextureHandle destination)
		{
			TextureHandle cameraDepthTexture = resourceData.cameraDepthTexture;
			TextureHandle motionVectorColor = resourceData.motionVectorColor;
			TextureDesc descriptor = source.GetDescriptor(renderGraph);
			TextureDesc compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(descriptor, cameraData.pixelWidth, cameraData.pixelHeight, GraphicsFormatUtility.GetLinearFormat(descriptor.format));
			compatibleDescriptor.enableRandomWrite = true;
			destination = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, "_CameraColorUpscaledSTP", false, FilterMode.Bilinear);
			int frameCount = Time.frameCount;
			Texture2D noiseTexture = this.m_Data.textures.blueNoise16LTex[frameCount & this.m_Data.textures.blueNoise16LTex.Length - 1];
			StpUtils.Execute(renderGraph, resourceData, cameraData, source, cameraDepthTexture, motionVectorColor, destination, noiseTexture);
			this.UpdateCameraResolution(renderGraph, cameraData, new Vector2Int(compatibleDescriptor.width, compatibleDescriptor.height));
		}

		public void RenderMotionBlur(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, in TextureHandle source, out TextureHandle destination)
		{
			Material cameraMotionBlur = this.m_Materials.cameraMotionBlur;
			destination = PostProcessPass.CreateCompatibleTexture(renderGraph, source, "_MotionBlurTarget", true, FilterMode.Bilinear);
			TextureHandle motionVectorColor = resourceData.motionVectorColor;
			TextureHandle cameraDepthTexture = resourceData.cameraDepthTexture;
			MotionBlurMode value = this.m_MotionBlur.mode.value;
			int num = (int)this.m_MotionBlur.quality.value;
			num += ((value == MotionBlurMode.CameraAndObjects) ? 3 : 0);
			PostProcessPass.MotionBlurPassData motionBlurPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.MotionBlurPassData>("Motion Blur", out motionBlurPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_MotionBlur), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1004))
			{
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.Write);
				motionBlurPassData.sourceTexture = source;
				rasterRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				if (value == MotionBlurMode.CameraAndObjects)
				{
					motionBlurPassData.motionVectors = motionVectorColor;
					rasterRenderGraphBuilder.UseTexture(motionVectorColor, AccessFlags.Read);
				}
				else
				{
					motionBlurPassData.motionVectors = TextureHandle.nullHandle;
				}
				rasterRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
				motionBlurPassData.material = cameraMotionBlur;
				motionBlurPassData.passIndex = num;
				motionBlurPassData.camera = cameraData.camera;
				motionBlurPassData.xr = cameraData.xr;
				motionBlurPassData.enableAlphaOutput = cameraData.isAlphaOutputEnabled;
				motionBlurPassData.intensity = this.m_MotionBlur.intensity.value;
				motionBlurPassData.clamp = this.m_MotionBlur.clamp.value;
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.MotionBlurPassData>(delegate(PostProcessPass.MotionBlurPassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					RTHandle rthandle = data.sourceTexture;
					PostProcessPass.UpdateMotionBlurMatrices(ref data.material, data.camera, data.xr);
					data.material.SetFloat("_Intensity", data.intensity);
					data.material.SetFloat("_Clamp", data.clamp);
					CoreUtils.SetKeyword(data.material, "_ENABLE_ALPHA_OUTPUT", data.enableAlphaOutput);
					PostProcessUtils.SetSourceSize(cmd, data.sourceTexture);
					Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(cmd, rthandle, v, data.material, data.passIndex);
				});
			}
		}

		private void LensFlareDataDrivenComputeOcclusion(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, in TextureDesc srcDesc)
		{
			if (!LensFlareCommonSRP.IsOcclusionRTCompatible())
			{
				return;
			}
			PostProcessPass.LensFlarePassData lensFlarePassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<PostProcessPass.LensFlarePassData>("Lens Flare Compute Occlusion", out lensFlarePassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.LensFlareDataDrivenComputeOcclusion), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1073))
			{
				RTHandle occlusionRT = LensFlareCommonSRP.occlusionRT;
				TextureHandle destinationTexture = renderGraph.ImportTexture(LensFlareCommonSRP.occlusionRT);
				lensFlarePassData.destinationTexture = destinationTexture;
				unsafeRenderGraphBuilder.UseTexture(destinationTexture, AccessFlags.Write);
				lensFlarePassData.cameraData = cameraData;
				lensFlarePassData.viewport = cameraData.pixelRect;
				lensFlarePassData.material = this.m_Materials.lensFlareDataDriven;
				lensFlarePassData.width = (float)srcDesc.width;
				lensFlarePassData.height = (float)srcDesc.height;
				if (this.m_PaniniProjection.IsActive())
				{
					lensFlarePassData.usePanini = true;
					lensFlarePassData.paniniDistance = this.m_PaniniProjection.distance.value;
					lensFlarePassData.paniniCropToFit = this.m_PaniniProjection.cropToFit.value;
				}
				else
				{
					lensFlarePassData.usePanini = false;
					lensFlarePassData.paniniDistance = 1f;
					lensFlarePassData.paniniCropToFit = 1f;
				}
				IBaseRenderGraphBuilder baseRenderGraphBuilder = unsafeRenderGraphBuilder;
				TextureHandle cameraDepthTexture = resourceData.cameraDepthTexture;
				baseRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
				unsafeRenderGraphBuilder.SetRenderFunc<PostProcessPass.LensFlarePassData>(delegate(PostProcessPass.LensFlarePassData data, UnsafeGraphContext ctx)
				{
					Camera camera = data.cameraData.camera;
					XRPass xr = data.cameraData.xr;
					Matrix4x4 viewProjMatrix;
					if (xr.enabled)
					{
						if (xr.singlePassEnabled)
						{
							viewProjMatrix = GL.GetGPUProjectionMatrix(data.cameraData.GetProjectionMatrixNoJitter(0), true) * data.cameraData.GetViewMatrix(0);
						}
						else
						{
							viewProjMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;
							int multipassId = data.cameraData.xr.multipassId;
						}
					}
					else
					{
						viewProjMatrix = GL.GetGPUProjectionMatrix(data.cameraData.GetProjectionMatrixNoJitter(0), true) * data.cameraData.GetViewMatrix(0);
					}
					LensFlareCommonSRP.ComputeOcclusion(data.material, camera, xr, xr.multipassId, data.width, data.height, data.usePanini, data.paniniDistance, data.paniniCropToFit, true, camera.transform.position, viewProjMatrix, ctx.cmd, false, false, null, null);
					if (xr.enabled && xr.singlePassEnabled)
					{
						for (int i = 1; i < xr.viewCount; i++)
						{
							Matrix4x4 viewProjMatrix2 = GL.GetGPUProjectionMatrix(data.cameraData.GetProjectionMatrixNoJitter(i), true) * data.cameraData.GetViewMatrix(i);
							LensFlareCommonSRP.ComputeOcclusion(data.material, camera, xr, i, data.width, data.height, data.usePanini, data.paniniDistance, data.paniniCropToFit, true, camera.transform.position, viewProjMatrix2, ctx.cmd, false, false, null, null);
						}
					}
				});
			}
		}

		public void RenderLensFlareDataDriven(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, in TextureHandle destination, in TextureDesc srcDesc)
		{
			PostProcessPass.LensFlarePassData lensFlarePassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<PostProcessPass.LensFlarePassData>("Lens Flare Data Driven Pass", out lensFlarePassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.LensFlareDataDriven), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1171))
			{
				lensFlarePassData.destinationTexture = destination;
				unsafeRenderGraphBuilder.UseTexture(destination, AccessFlags.Write);
				lensFlarePassData.cameraData = cameraData;
				lensFlarePassData.material = this.m_Materials.lensFlareDataDriven;
				lensFlarePassData.width = (float)srcDesc.width;
				lensFlarePassData.height = (float)srcDesc.height;
				lensFlarePassData.viewport.x = 0f;
				lensFlarePassData.viewport.y = 0f;
				lensFlarePassData.viewport.width = (float)srcDesc.width;
				lensFlarePassData.viewport.height = (float)srcDesc.height;
				if (this.m_PaniniProjection.IsActive())
				{
					lensFlarePassData.usePanini = true;
					lensFlarePassData.paniniDistance = this.m_PaniniProjection.distance.value;
					lensFlarePassData.paniniCropToFit = this.m_PaniniProjection.cropToFit.value;
				}
				else
				{
					lensFlarePassData.usePanini = false;
					lensFlarePassData.paniniDistance = 1f;
					lensFlarePassData.paniniCropToFit = 1f;
				}
				if (LensFlareCommonSRP.IsOcclusionRTCompatible())
				{
					TextureHandle textureHandle = renderGraph.ImportTexture(LensFlareCommonSRP.occlusionRT);
					unsafeRenderGraphBuilder.UseTexture(textureHandle, AccessFlags.Read);
				}
				else
				{
					IBaseRenderGraphBuilder baseRenderGraphBuilder = unsafeRenderGraphBuilder;
					TextureHandle cameraDepthTexture = resourceData.cameraDepthTexture;
					baseRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
				}
				unsafeRenderGraphBuilder.SetRenderFunc<PostProcessPass.LensFlarePassData>(delegate(PostProcessPass.LensFlarePassData data, UnsafeGraphContext ctx)
				{
					Camera camera = data.cameraData.camera;
					XRPass xr = data.cameraData.xr;
					if (!xr.enabled || (xr.enabled && !xr.singlePassEnabled))
					{
						Matrix4x4 viewProjMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true) * camera.worldToCameraMatrix;
						LensFlareCommonSRP.DoLensFlareDataDrivenCommon(data.material, data.cameraData.camera, data.viewport, xr, data.cameraData.xr.multipassId, data.width, data.height, data.usePanini, data.paniniDistance, data.paniniCropToFit, true, camera.transform.position, viewProjMatrix, ctx.cmd, false, false, null, null, data.destinationTexture, (Light light, Camera cam, Vector3 wo) => PostProcessPass.GetLensFlareLightAttenuation(light, cam, wo), false);
						return;
					}
					for (int i = 0; i < xr.viewCount; i++)
					{
						Matrix4x4 viewProjMatrix2 = GL.GetGPUProjectionMatrix(data.cameraData.GetProjectionMatrixNoJitter(i), true) * data.cameraData.GetViewMatrix(i);
						LensFlareCommonSRP.DoLensFlareDataDrivenCommon(data.material, data.cameraData.camera, data.viewport, xr, data.cameraData.xr.multipassId, data.width, data.height, data.usePanini, data.paniniDistance, data.paniniCropToFit, true, camera.transform.position, viewProjMatrix2, ctx.cmd, false, false, null, null, data.destinationTexture, (Light light, Camera cam, Vector3 wo) => PostProcessPass.GetLensFlareLightAttenuation(light, cam, wo), false);
					}
				});
			}
		}

		public TextureHandle RenderLensFlareScreenSpace(RenderGraph renderGraph, Camera camera, in TextureDesc srcDesc, TextureHandle originalBloomTexture, TextureHandle screenSpaceLensFlareBloomMipTexture, bool sameInputOutputTex)
		{
			int value = (int)this.m_LensFlareScreenSpace.resolution.value;
			int width = Math.Max(srcDesc.width / value, 1);
			int height = Math.Max(srcDesc.height / value, 1);
			TextureDesc compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(srcDesc, width, height, this.m_DefaultColorFormat);
			TextureHandle streakTmpTexture = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, "_StreakTmpTexture", true, FilterMode.Bilinear);
			TextureHandle streakTmpTexture2 = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, "_StreakTmpTexture2", true, FilterMode.Bilinear);
			TextureHandle result = PostProcessPass.CreateCompatibleTexture(renderGraph, compatibleDescriptor, "_LensFlareScreenSpace", true, FilterMode.Bilinear);
			PostProcessPass.LensFlareScreenSpacePassData lensFlareScreenSpacePassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<PostProcessPass.LensFlareScreenSpacePassData>("Blit Lens Flare Screen Space", out lensFlareScreenSpacePassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.LensFlareScreenSpace), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1292))
			{
				lensFlareScreenSpacePassData.streakTmpTexture = streakTmpTexture;
				unsafeRenderGraphBuilder.UseTexture(streakTmpTexture, AccessFlags.ReadWrite);
				lensFlareScreenSpacePassData.streakTmpTexture2 = streakTmpTexture2;
				unsafeRenderGraphBuilder.UseTexture(streakTmpTexture2, AccessFlags.ReadWrite);
				lensFlareScreenSpacePassData.screenSpaceLensFlareBloomMipTexture = screenSpaceLensFlareBloomMipTexture;
				unsafeRenderGraphBuilder.UseTexture(screenSpaceLensFlareBloomMipTexture, AccessFlags.ReadWrite);
				lensFlareScreenSpacePassData.originalBloomTexture = originalBloomTexture;
				if (!sameInputOutputTex)
				{
					unsafeRenderGraphBuilder.UseTexture(originalBloomTexture, AccessFlags.ReadWrite);
				}
				lensFlareScreenSpacePassData.actualWidth = srcDesc.width;
				lensFlareScreenSpacePassData.actualHeight = srcDesc.height;
				lensFlareScreenSpacePassData.camera = camera;
				lensFlareScreenSpacePassData.material = this.m_Materials.lensFlareScreenSpace;
				lensFlareScreenSpacePassData.lensFlareScreenSpace = this.m_LensFlareScreenSpace;
				lensFlareScreenSpacePassData.downsample = value;
				lensFlareScreenSpacePassData.result = result;
				unsafeRenderGraphBuilder.UseTexture(result, AccessFlags.ReadWrite);
				unsafeRenderGraphBuilder.SetRenderFunc<PostProcessPass.LensFlareScreenSpacePassData>(delegate(PostProcessPass.LensFlareScreenSpacePassData data, UnsafeGraphContext context)
				{
					UnsafeCommandBuffer cmd = context.cmd;
					Camera camera2 = data.camera;
					ScreenSpaceLensFlare lensFlareScreenSpace = data.lensFlareScreenSpace;
					LensFlareCommonSRP.DoLensFlareScreenSpaceCommon(data.material, camera2, (float)data.actualWidth, (float)data.actualHeight, data.lensFlareScreenSpace.tintColor.value, data.originalBloomTexture, data.screenSpaceLensFlareBloomMipTexture, null, data.streakTmpTexture, data.streakTmpTexture2, new Vector4(lensFlareScreenSpace.intensity.value, lensFlareScreenSpace.firstFlareIntensity.value, lensFlareScreenSpace.secondaryFlareIntensity.value, lensFlareScreenSpace.warpedFlareIntensity.value), new Vector4(lensFlareScreenSpace.vignetteEffect.value, lensFlareScreenSpace.startingPosition.value, lensFlareScreenSpace.scale.value, 0f), new Vector4((float)lensFlareScreenSpace.samples.value, lensFlareScreenSpace.sampleDimmer.value, lensFlareScreenSpace.chromaticAbberationIntensity.value, 0f), new Vector4(lensFlareScreenSpace.streaksIntensity.value, lensFlareScreenSpace.streaksLength.value, lensFlareScreenSpace.streaksOrientation.value, lensFlareScreenSpace.streaksThreshold.value), new Vector4((float)data.downsample, lensFlareScreenSpace.warpedFlareScale.value.x, lensFlareScreenSpace.warpedFlareScale.value.y, 0f), cmd, data.result, false);
				});
			}
			return originalBloomTexture;
		}

		private static void ScaleViewport(RasterCommandBuffer cmd, RTHandle sourceTextureHdl, RTHandle dest, UniversalCameraData cameraData, bool hasFinalPass)
		{
			RenderTargetIdentifier rhs = BuiltinRenderTextureType.CameraTarget;
			if (cameraData.xr.enabled)
			{
				rhs = cameraData.xr.renderTarget;
			}
			if (dest.nameID == rhs || cameraData.targetTexture != null)
			{
				if (hasFinalPass || !cameraData.resolveFinalTarget)
				{
					int width = cameraData.cameraTargetDescriptor.width;
					int height = cameraData.cameraTargetDescriptor.height;
					Rect viewport = new Rect(0f, 0f, (float)width, (float)height);
					cmd.SetViewport(viewport);
					return;
				}
				cmd.SetViewport(cameraData.pixelRect);
			}
		}

		private static void ScaleViewportAndBlit(RasterCommandBuffer cmd, RTHandle sourceTextureHdl, RTHandle dest, UniversalCameraData cameraData, Material material, bool hasFinalPass)
		{
			Vector4 finalBlitScaleBias = RenderingUtils.GetFinalBlitScaleBias(sourceTextureHdl, dest, cameraData);
			PostProcessPass.ScaleViewport(cmd, sourceTextureHdl, dest, cameraData, hasFinalPass);
			Blitter.BlitTexture(cmd, sourceTextureHdl, finalBlitScaleBias, material, 0);
		}

		private static void ScaleViewportAndDrawVisibilityMesh(RasterCommandBuffer cmd, RTHandle sourceTextureHdl, RTHandle dest, UniversalCameraData cameraData, Material material, bool hasFinalPass)
		{
			Vector4 finalBlitScaleBias = RenderingUtils.GetFinalBlitScaleBias(sourceTextureHdl, dest, cameraData);
			PostProcessPass.ScaleViewport(cmd, sourceTextureHdl, dest, cameraData, hasFinalPass);
			MaterialPropertyBlock materialPropertyBlock = XRSystemUniversal.GetMaterialPropertyBlock();
			materialPropertyBlock.SetVector(Shader.PropertyToID("_BlitScaleBias"), finalBlitScaleBias);
			materialPropertyBlock.SetTexture(Shader.PropertyToID("_BlitTexture"), sourceTextureHdl);
			cameraData.xr.RenderVisibleMeshCustomMaterial(cmd, cameraData.xr.occlusionMeshScale, material, materialPropertyBlock, 1, cameraData.IsRenderTargetProjectionMatrixFlipped(dest, null));
		}

		public void RenderFinalSetup(RenderGraph renderGraph, UniversalCameraData cameraData, in TextureHandle source, in TextureHandle destination, ref PostProcessPass.FinalBlitSettings settings)
		{
			PostProcessPass.PostProcessingFinalSetupPassData postProcessingFinalSetupPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.PostProcessingFinalSetupPassData>("Postprocessing Final Setup Pass", out postProcessingFinalSetupPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_FinalSetup), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1433))
			{
				Material scalingSetup = this.m_Materials.scalingSetup;
				scalingSetup.shaderKeywords = null;
				if (settings.isFxaaEnabled)
				{
					scalingSetup.EnableKeyword("_FXAA");
				}
				if (settings.isFsrEnabled)
				{
					scalingSetup.EnableKeyword(settings.hdrOperations.HasFlag(HDROutputUtils.Operation.ColorEncoding) ? "_GAMMA_20_AND_HDR_INPUT" : "_GAMMA_20");
				}
				if (settings.hdrOperations.HasFlag(HDROutputUtils.Operation.ColorEncoding))
				{
					this.SetupHDROutput(cameraData.hdrDisplayInformation, cameraData.hdrDisplayColorGamut, scalingSetup, settings.hdrOperations, cameraData.rendersOverlayUI);
				}
				if (settings.isAlphaOutputEnabled)
				{
					CoreUtils.SetKeyword(scalingSetup, "_ENABLE_ALPHA_OUTPUT", settings.isAlphaOutputEnabled);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				postProcessingFinalSetupPassData.destinationTexture = destination;
				rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.Write);
				postProcessingFinalSetupPassData.sourceTexture = source;
				rasterRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				postProcessingFinalSetupPassData.cameraData = cameraData;
				postProcessingFinalSetupPassData.material = scalingSetup;
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.PostProcessingFinalSetupPassData>(delegate(PostProcessPass.PostProcessingFinalSetupPassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					RTHandle rthandle = data.sourceTexture;
					PostProcessUtils.SetSourceSize(cmd, rthandle);
					bool hasFinalPass = true;
					PostProcessPass.ScaleViewportAndBlit(context.cmd, rthandle, data.destinationTexture, data.cameraData, data.material, hasFinalPass);
				});
			}
		}

		public void RenderFinalFSRScale(RenderGraph renderGraph, in TextureHandle source, in TextureDesc srcDesc, in TextureHandle destination, in TextureDesc dstDesc, bool enableAlphaOutput)
		{
			this.m_Materials.easu.shaderKeywords = null;
			PostProcessPass.PostProcessingFinalFSRScalePassData postProcessingFinalFSRScalePassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.PostProcessingFinalFSRScalePassData>("Postprocessing Final FSR Scale Pass", out postProcessingFinalFSRScalePassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_FinalFSRScale), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1486))
			{
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.Write);
				postProcessingFinalFSRScalePassData.sourceTexture = source;
				rasterRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				postProcessingFinalFSRScalePassData.material = this.m_Materials.easu;
				postProcessingFinalFSRScalePassData.enableAlphaOutput = enableAlphaOutput;
				postProcessingFinalFSRScalePassData.fsrInputSize = new Vector2((float)srcDesc.width, (float)srcDesc.height);
				postProcessingFinalFSRScalePassData.fsrOutputSize = new Vector2((float)dstDesc.width, (float)dstDesc.height);
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.PostProcessingFinalFSRScalePassData>(delegate(PostProcessPass.PostProcessingFinalFSRScalePassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					TextureHandle sourceTexture = data.sourceTexture;
					Material material = data.material;
					bool enableAlphaOutput2 = data.enableAlphaOutput;
					RTHandle rthandle = sourceTexture;
					FSRUtils.SetEasuConstants(cmd, data.fsrInputSize, data.fsrInputSize, data.fsrOutputSize);
					CoreUtils.SetKeyword(material, "_ENABLE_ALPHA_OUTPUT", enableAlphaOutput2);
					Vector2 v = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					Blitter.BlitTexture(cmd, rthandle, v, material, 0);
				});
			}
		}

		public void RenderFinalBlit(RenderGraph renderGraph, UniversalCameraData cameraData, in TextureHandle source, in TextureHandle overlayUITexture, in TextureHandle postProcessingTarget, ref PostProcessPass.FinalBlitSettings settings)
		{
			PostProcessPass.PostProcessingFinalBlitPassData postProcessingFinalBlitPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.PostProcessingFinalBlitPassData>("Postprocessing Final Blit Pass", out postProcessingFinalBlitPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_FinalBlit), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1568))
			{
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				postProcessingFinalBlitPassData.destinationTexture = postProcessingTarget;
				rasterRenderGraphBuilder.SetRenderAttachment(postProcessingTarget, 0, AccessFlags.Write);
				postProcessingFinalBlitPassData.sourceTexture = source;
				rasterRenderGraphBuilder.UseTexture(source, AccessFlags.Read);
				postProcessingFinalBlitPassData.cameraData = cameraData;
				postProcessingFinalBlitPassData.material = this.m_Materials.finalPass;
				postProcessingFinalBlitPassData.settings = settings;
				if (settings.requireHDROutput && this.m_EnableColorEncodingIfNeeded && cameraData.rendersOverlayUI)
				{
					rasterRenderGraphBuilder.UseTexture(overlayUITexture, AccessFlags.Read);
				}
				if (cameraData.xr.enabled)
				{
					bool flag = !XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster);
					rasterRenderGraphBuilder.EnableFoveatedRasterization(cameraData.xr.supportsFoveatedRendering && flag);
				}
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.PostProcessingFinalBlitPassData>(delegate(PostProcessPass.PostProcessingFinalBlitPassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					Material material = data.material;
					bool isFxaaEnabled = data.settings.isFxaaEnabled;
					bool isFsrEnabled = data.settings.isFsrEnabled;
					bool isTaaSharpeningEnabled = data.settings.isTaaSharpeningEnabled;
					bool requireHDROutput = data.settings.requireHDROutput;
					bool resolveToDebugScreen = data.settings.resolveToDebugScreen;
					bool isAlphaOutputEnabled = data.settings.isAlphaOutputEnabled;
					RTHandle rthandle = data.sourceTexture;
					RTHandle handle = data.destinationTexture;
					PostProcessUtils.SetSourceSize(cmd, data.sourceTexture);
					if (isFxaaEnabled)
					{
						material.EnableKeyword("_FXAA");
					}
					if (isFsrEnabled)
					{
						float sharpnessLinear = data.cameraData.fsrOverrideSharpness ? data.cameraData.fsrSharpness : 0.92f;
						if (data.cameraData.fsrSharpness > 0f)
						{
							material.EnableKeyword(requireHDROutput ? "_EASU_RCAS_AND_HDR_INPUT" : "_RCAS");
							FSRUtils.SetRcasConstantsLinear(cmd, sharpnessLinear);
						}
					}
					else if (isTaaSharpeningEnabled)
					{
						material.EnableKeyword("_RCAS");
						FSRUtils.SetRcasConstantsLinear(cmd, data.cameraData.taaSettings.contrastAdaptiveSharpening);
					}
					if (isAlphaOutputEnabled)
					{
						CoreUtils.SetKeyword(material, "_ENABLE_ALPHA_OUTPUT", isAlphaOutputEnabled);
					}
					bool flag2 = !data.cameraData.isSceneViewCamera;
					if (data.cameraData.xr.enabled)
					{
						flag2 = (handle == data.cameraData.xr.renderTarget);
					}
					flag2 &= !resolveToDebugScreen;
					Vector2 vector = rthandle.useScaling ? new Vector2(rthandle.rtHandleProperties.rtHandleScale.x, rthandle.rtHandleProperties.rtHandleScale.y) : Vector2.one;
					bool flag3 = flag2 && data.cameraData.targetTexture == null && SystemInfo.graphicsUVStartsAtTop;
					Vector4 vector2 = flag3 ? new Vector4(vector.x, -vector.y, 0f, vector.y) : new Vector4(vector.x, vector.y, 0f, 0f);
					cmd.SetViewport(data.cameraData.pixelRect);
					if (data.cameraData.xr.enabled && data.cameraData.xr.hasValidVisibleMesh)
					{
						MaterialPropertyBlock materialPropertyBlock = XRSystemUniversal.GetMaterialPropertyBlock();
						materialPropertyBlock.SetVector(Shader.PropertyToID("_BlitScaleBias"), vector2);
						materialPropertyBlock.SetTexture(Shader.PropertyToID("_BlitTexture"), rthandle);
						data.cameraData.xr.RenderVisibleMeshCustomMaterial(cmd, data.cameraData.xr.occlusionMeshScale, material, materialPropertyBlock, 1, !flag3);
						return;
					}
					Blitter.BlitTexture(cmd, rthandle, vector2, material, 0);
				});
			}
		}

		public void RenderFinalPassRenderGraph(RenderGraph renderGraph, ContextContainer frameData, in TextureHandle source, in TextureHandle overlayUITexture, in TextureHandle postProcessingTarget, bool enableColorEncodingIfNeeded)
		{
			VolumeStack stack = VolumeManager.instance.stack;
			this.m_Tonemapping = stack.GetComponent<Tonemapping>();
			this.m_FilmGrain = stack.GetComponent<FilmGrain>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			Material finalPass = this.m_Materials.finalPass;
			finalPass.shaderKeywords = null;
			PostProcessPass.FinalBlitSettings finalBlitSettings = PostProcessPass.FinalBlitSettings.Create();
			TextureDesc textureDesc = renderGraph.GetTextureDesc(source);
			TextureDesc textureDesc2 = textureDesc;
			textureDesc2.width = universalCameraData.pixelWidth;
			textureDesc2.height = universalCameraData.pixelHeight;
			this.m_HasFinalPass = false;
			this.m_IsFinalPass = true;
			this.m_EnableColorEncodingIfNeeded = enableColorEncodingIfNeeded;
			if (this.m_FilmGrain.IsActive())
			{
				finalPass.EnableKeyword("_FILM_GRAIN");
				PostProcessUtils.ConfigureFilmGrain(this.m_Data, this.m_FilmGrain, textureDesc2.width, textureDesc2.height, finalPass);
			}
			if (universalCameraData.isDitheringEnabled)
			{
				finalPass.EnableKeyword("_DITHERING");
				this.m_DitheringTextureIndex = PostProcessUtils.ConfigureDithering(this.m_Data, this.m_DitheringTextureIndex, textureDesc2.width, textureDesc2.height, finalPass);
			}
			if (this.RequireSRGBConversionBlitToBackBuffer(universalCameraData.requireSrgbConversion))
			{
				finalPass.EnableKeyword("_LINEAR_TO_SRGB_CONVERSION");
			}
			finalBlitSettings.hdrOperations = HDROutputUtils.Operation.None;
			finalBlitSettings.requireHDROutput = this.RequireHDROutput(universalCameraData);
			if (finalBlitSettings.requireHDROutput)
			{
				finalBlitSettings.hdrOperations = (this.m_EnableColorEncodingIfNeeded ? HDROutputUtils.Operation.ColorEncoding : HDROutputUtils.Operation.None);
				if (!universalCameraData.postProcessEnabled)
				{
					finalBlitSettings.hdrOperations |= HDROutputUtils.Operation.ColorConversion;
				}
				this.SetupHDROutput(universalCameraData.hdrDisplayInformation, universalCameraData.hdrDisplayColorGamut, finalPass, finalBlitSettings.hdrOperations, universalCameraData.rendersOverlayUI);
			}
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			bool resolveToDebugScreen = activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget);
			finalBlitSettings.resolveToDebugScreen = resolveToDebugScreen;
			finalBlitSettings.isAlphaOutputEnabled = universalCameraData.isAlphaOutputEnabled;
			finalBlitSettings.isFxaaEnabled = (universalCameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing);
			finalBlitSettings.isFsrEnabled = (universalCameraData.imageScalingMode == ImageScalingMode.Upscaling && universalCameraData.upscalingFilter == ImageUpscalingFilter.FSR);
			finalBlitSettings.isTaaSharpeningEnabled = (universalCameraData.IsTemporalAAEnabled() && universalCameraData.taaSettings.contrastAdaptiveSharpening > 0f && !finalBlitSettings.isFsrEnabled && !universalCameraData.IsSTPEnabled());
			TextureDesc textureDesc3 = textureDesc;
			if (!finalBlitSettings.requireHDROutput)
			{
				textureDesc3.format = UniversalRenderPipeline.MakeUnormRenderTextureGraphicsFormat();
			}
			TextureHandle textureHandle = PostProcessPass.CreateCompatibleTexture(renderGraph, textureDesc3, "scalingSetupTarget", true, FilterMode.Point);
			TextureHandle textureHandle2 = PostProcessPass.CreateCompatibleTexture(renderGraph, textureDesc2, "_UpscaledTexture", true, FilterMode.Point);
			TextureHandle textureHandle3 = source;
			if (universalCameraData.imageScalingMode != ImageScalingMode.None)
			{
				if (finalBlitSettings.isFxaaEnabled || finalBlitSettings.isFsrEnabled)
				{
					this.RenderFinalSetup(renderGraph, universalCameraData, textureHandle3, textureHandle, ref finalBlitSettings);
					textureHandle3 = textureHandle;
					finalBlitSettings.isFxaaEnabled = false;
				}
				ImageScalingMode imageScalingMode = universalCameraData.imageScalingMode;
				if (imageScalingMode != ImageScalingMode.Upscaling)
				{
					if (imageScalingMode == ImageScalingMode.Downscaling)
					{
						finalBlitSettings.isTaaSharpeningEnabled = false;
					}
				}
				else
				{
					switch (universalCameraData.upscalingFilter)
					{
					case ImageUpscalingFilter.Point:
						if (!finalBlitSettings.isTaaSharpeningEnabled)
						{
							finalPass.EnableKeyword("_POINT_SAMPLING");
						}
						break;
					case ImageUpscalingFilter.FSR:
						this.RenderFinalFSRScale(renderGraph, textureHandle3, textureDesc, textureHandle2, textureDesc2, finalBlitSettings.isAlphaOutputEnabled);
						textureHandle3 = textureHandle2;
						break;
					}
				}
			}
			else if (finalBlitSettings.isFxaaEnabled)
			{
				finalPass.EnableKeyword("_FXAA");
			}
			this.RenderFinalBlit(renderGraph, universalCameraData, textureHandle3, overlayUITexture, postProcessingTarget, ref finalBlitSettings);
		}

		private TextureHandle TryGetCachedUserLutTextureHandle(RenderGraph renderGraph)
		{
			if (this.m_ColorLookup.texture.value == null)
			{
				if (this.m_UserLut != null)
				{
					this.m_UserLut.Release();
					this.m_UserLut = null;
				}
			}
			else if (this.m_UserLut == null || this.m_UserLut.externalTexture != this.m_ColorLookup.texture.value)
			{
				RTHandle userLut = this.m_UserLut;
				if (userLut != null)
				{
					userLut.Release();
				}
				this.m_UserLut = RTHandles.Alloc(this.m_ColorLookup.texture.value);
			}
			if (this.m_UserLut == null)
			{
				return TextureHandle.nullHandle;
			}
			return renderGraph.ImportTexture(this.m_UserLut);
		}

		public void RenderUberPost(RenderGraph renderGraph, ContextContainer frameData, UniversalCameraData cameraData, UniversalPostProcessingData postProcessingData, in TextureHandle sourceTexture, in TextureHandle destTexture, in TextureHandle lutTexture, in TextureHandle bloomTexture, in TextureHandle overlayUITexture, bool requireHDROutput, bool enableAlphaOutput, bool resolveToDebugScreen, bool hasFinalPass)
		{
			Material uber = this.m_Materials.uber;
			bool isHdrGrading = postProcessingData.gradingMode == ColorGradingMode.HighDynamicRange;
			int lutSize = postProcessingData.lutSize;
			int num = lutSize * lutSize;
			float w = Mathf.Pow(2f, this.m_ColorAdjustments.postExposure.value);
			Vector4 lutParams = new Vector4(1f / (float)num, 1f / (float)lutSize, (float)lutSize - 1f, w);
			TextureHandle userLutTexture = this.TryGetCachedUserLutTextureHandle(renderGraph);
			Vector4 userLutParams = (!this.m_ColorLookup.IsActive()) ? Vector4.zero : new Vector4(1f / (float)this.m_ColorLookup.texture.value.width, 1f / (float)this.m_ColorLookup.texture.value.height, (float)this.m_ColorLookup.texture.value.height - 1f, this.m_ColorLookup.contribution.value);
			PostProcessPass.UberPostPassData uberPostPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.UberPostPassData>("Blit Post Processing", out uberPostPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_UberPost), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 1889))
			{
				UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
				if (cameraData.xr.enabled)
				{
					bool flag = cameraData.xrUniversal.canFoveateIntermediatePasses || universalResourceData.isActiveTargetBackBuffer;
					flag &= !XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster);
					rasterRenderGraphBuilder.EnableFoveatedRasterization(cameraData.xr.supportsFoveatedRendering && flag);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				uberPostPassData.destinationTexture = destTexture;
				rasterRenderGraphBuilder.SetRenderAttachment(destTexture, 0, AccessFlags.Write);
				uberPostPassData.sourceTexture = sourceTexture;
				rasterRenderGraphBuilder.UseTexture(sourceTexture, AccessFlags.Read);
				uberPostPassData.lutTexture = lutTexture;
				rasterRenderGraphBuilder.UseTexture(lutTexture, AccessFlags.Read);
				uberPostPassData.lutParams = lutParams;
				uberPostPassData.userLutTexture = userLutTexture;
				if (userLutTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(userLutTexture, AccessFlags.Read);
				}
				if (this.m_Bloom.IsActive())
				{
					rasterRenderGraphBuilder.UseTexture(bloomTexture, AccessFlags.Read);
					uberPostPassData.bloomTexture = bloomTexture;
				}
				if (requireHDROutput && this.m_EnableColorEncodingIfNeeded)
				{
					TextureHandle textureHandle = overlayUITexture;
					if (textureHandle.IsValid())
					{
						rasterRenderGraphBuilder.UseTexture(overlayUITexture, AccessFlags.Read);
					}
				}
				uberPostPassData.userLutParams = userLutParams;
				uberPostPassData.cameraData = cameraData;
				uberPostPassData.material = uber;
				uberPostPassData.toneMappingMode = this.m_Tonemapping.mode.value;
				uberPostPassData.isHdrGrading = isHdrGrading;
				uberPostPassData.enableAlphaOutput = enableAlphaOutput;
				uberPostPassData.hasFinalPass = hasFinalPass;
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.UberPostPassData>(delegate(PostProcessPass.UberPostPassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					Camera camera = data.cameraData.camera;
					Material material = data.material;
					RTHandle sourceTextureHdl = data.sourceTexture;
					material.SetTexture(PostProcessPass.ShaderConstants._InternalLut, data.lutTexture);
					material.SetVector(PostProcessPass.ShaderConstants._Lut_Params, data.lutParams);
					material.SetTexture(PostProcessPass.ShaderConstants._UserLut, data.userLutTexture);
					material.SetVector(PostProcessPass.ShaderConstants._UserLut_Params, data.userLutParams);
					if (data.bloomTexture.IsValid())
					{
						material.SetTexture(PostProcessPass.ShaderConstants._Bloom_Texture, data.bloomTexture);
					}
					if (data.isHdrGrading)
					{
						material.EnableKeyword("_HDR_GRADING");
					}
					else
					{
						TonemappingMode toneMappingMode = data.toneMappingMode;
						if (toneMappingMode != TonemappingMode.Neutral)
						{
							if (toneMappingMode == TonemappingMode.ACES)
							{
								material.EnableKeyword("_TONEMAP_ACES");
							}
						}
						else
						{
							material.EnableKeyword("_TONEMAP_NEUTRAL");
						}
					}
					CoreUtils.SetKeyword(material, "_ENABLE_ALPHA_OUTPUT", data.enableAlphaOutput);
					if (data.cameraData.xr.enabled && data.cameraData.xr.hasValidVisibleMesh)
					{
						PostProcessPass.ScaleViewportAndDrawVisibilityMesh(cmd, sourceTextureHdl, data.destinationTexture, data.cameraData, material, data.hasFinalPass);
						return;
					}
					PostProcessPass.ScaleViewportAndBlit(cmd, sourceTextureHdl, data.destinationTexture, data.cameraData, material, data.hasFinalPass);
				});
			}
		}

		public void RenderPostProcessingRenderGraph(RenderGraph renderGraph, ContextContainer frameData, in TextureHandle activeCameraColorTexture, in TextureHandle lutTexture, in TextureHandle overlayUITexture, in TextureHandle postProcessingTarget, bool hasFinalPass, bool resolveToDebugScreen, bool enableColorEndingIfNeeded)
		{
			UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
			frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalPostProcessingData universalPostProcessingData = frameData.Get<UniversalPostProcessingData>();
			VolumeStack stack = VolumeManager.instance.stack;
			this.m_DepthOfField = stack.GetComponent<DepthOfField>();
			this.m_MotionBlur = stack.GetComponent<MotionBlur>();
			this.m_PaniniProjection = stack.GetComponent<PaniniProjection>();
			this.m_Bloom = stack.GetComponent<Bloom>();
			this.m_LensFlareScreenSpace = stack.GetComponent<ScreenSpaceLensFlare>();
			this.m_LensDistortion = stack.GetComponent<LensDistortion>();
			this.m_ChromaticAberration = stack.GetComponent<ChromaticAberration>();
			this.m_Vignette = stack.GetComponent<Vignette>();
			this.m_ColorLookup = stack.GetComponent<ColorLookup>();
			this.m_ColorAdjustments = stack.GetComponent<ColorAdjustments>();
			this.m_Tonemapping = stack.GetComponent<Tonemapping>();
			this.m_FilmGrain = stack.GetComponent<FilmGrain>();
			this.m_UseFastSRGBLinearConversion = universalPostProcessingData.useFastSRGBLinearConversion;
			this.m_SupportDataDrivenLensFlare = universalPostProcessingData.supportDataDrivenLensFlare;
			this.m_SupportScreenSpaceLensFlare = universalPostProcessingData.supportScreenSpaceLensFlare;
			this.m_HasFinalPass = hasFinalPass;
			this.m_EnableColorEncodingIfNeeded = enableColorEndingIfNeeded;
			ref ScriptableRenderer ptr = ref universalCameraData.renderer;
			bool isSceneViewCamera = universalCameraData.isSceneViewCamera;
			bool flag = universalCameraData.isStopNaNEnabled && this.m_Materials.stopNaN != null;
			bool flag2 = universalCameraData.antialiasing == AntialiasingMode.SubpixelMorphologicalAntiAliasing;
			Material x = (this.m_DepthOfField.mode.value == DepthOfFieldMode.Gaussian) ? this.m_Materials.gaussianDepthOfField : this.m_Materials.bokehDepthOfField;
			bool flag3 = this.m_DepthOfField.IsActive() && !isSceneViewCamera && x != null;
			bool flag4 = !LensFlareCommonSRP.Instance.IsEmpty() && this.m_SupportDataDrivenLensFlare;
			bool flag5 = this.m_LensFlareScreenSpace.IsActive() && this.m_SupportScreenSpaceLensFlare;
			bool flag6 = this.m_MotionBlur.IsActive() && !isSceneViewCamera;
			bool flag7 = this.m_PaniniProjection.IsActive() && !isSceneViewCamera;
			if (universalCameraData.imageScalingMode == ImageScalingMode.Upscaling)
			{
				bool flag8 = universalCameraData.upscalingFilter == ImageUpscalingFilter.FSR;
			}
			flag6 = (flag6 && Application.isPlaying);
			if (flag6 && this.m_MotionBlur.mode.value == MotionBlurMode.CameraAndObjects)
			{
				flag6 &= ptr.SupportsMotionVectors();
				if (!flag6)
				{
					string message = "Disabling Motion Blur for Camera And Objects because the renderer does not implement motion vectors.";
					if (Time.frameCount % 60 == 0)
					{
						Debug.LogWarning(message);
					}
				}
			}
			bool flag9 = universalCameraData.IsTemporalAAEnabled();
			bool flag10 = universalCameraData.IsSTPRequested();
			bool flag11 = flag9 && flag10;
			if (!flag9 && universalCameraData.IsTemporalAARequested())
			{
				TemporalAA.ValidateAndWarn(universalCameraData, flag10);
			}
			PostProcessPass.PostFXSetupPassData postFXSetupPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PostProcessPass.PostFXSetupPassData>("Setup PostFX passes", out postFXSetupPassData, ProfilingSampler.Get<URPProfileId>(URPProfileId.RG_SetupPostFX), ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\PostProcessPassRenderGraph.cs", 2053))
			{
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<PostProcessPass.PostFXSetupPassData>(delegate(PostProcessPass.PostFXSetupPassData data, RasterGraphContext context)
				{
					context.cmd.SetGlobalMatrix(PostProcessPass.ShaderConstants._FullscreenProjMat, GL.GetGPUProjectionMatrix(Matrix4x4.identity, true));
				});
			}
			TextureHandle textureHandle = activeCameraColorTexture;
			if (flag)
			{
				TextureHandle textureHandle2;
				this.RenderStopNaN(renderGraph, textureHandle, out textureHandle2);
				textureHandle = textureHandle2;
			}
			if (flag2)
			{
				TextureHandle textureHandle3;
				this.RenderSMAA(renderGraph, resourceData, universalCameraData.antialiasingQuality, textureHandle, out textureHandle3);
				textureHandle = textureHandle3;
			}
			if (flag3)
			{
				TextureHandle textureHandle4;
				this.RenderDoF(renderGraph, resourceData, universalCameraData, textureHandle, out textureHandle4);
				textureHandle = textureHandle4;
			}
			if (flag9)
			{
				if (flag11)
				{
					TextureHandle textureHandle5;
					this.RenderSTP(renderGraph, resourceData, universalCameraData, ref textureHandle, out textureHandle5);
					textureHandle = textureHandle5;
				}
				else
				{
					TextureHandle textureHandle6;
					this.RenderTemporalAA(renderGraph, resourceData, universalCameraData, ref textureHandle, out textureHandle6);
					textureHandle = textureHandle6;
				}
			}
			if (flag6)
			{
				TextureHandle textureHandle7;
				this.RenderMotionBlur(renderGraph, resourceData, universalCameraData, textureHandle, out textureHandle7);
				textureHandle = textureHandle7;
			}
			if (flag7)
			{
				TextureHandle textureHandle8;
				this.RenderPaniniProjection(renderGraph, universalCameraData.camera, textureHandle, out textureHandle8);
				textureHandle = textureHandle8;
			}
			this.m_Materials.uber.shaderKeywords = null;
			TextureDesc descriptor = textureHandle.GetDescriptor(renderGraph);
			TextureHandle originalBloomTexture = TextureHandle.nullHandle;
			if (this.m_Bloom.IsActive() || flag5)
			{
				this.RenderBloomTexture(renderGraph, textureHandle, out originalBloomTexture, universalCameraData.isAlphaOutputEnabled);
				if (flag5)
				{
					int num = this.CalcBloomMipCount(this.m_Bloom, this.CalcBloomResolution(this.m_Bloom, descriptor));
					int max = Mathf.Clamp(num - 1, 0, this.m_Bloom.maxIterations.value / 2);
					int num2 = Mathf.Clamp(this.m_LensFlareScreenSpace.bloomMip.value, 0, max);
					TextureHandle screenSpaceLensFlareBloomMipTexture = this._BloomMipUp[num2];
					bool sameInputOutputTex = num2 == 0;
					if (num == 1)
					{
						screenSpaceLensFlareBloomMipTexture = this._BloomMipDown[0];
					}
					originalBloomTexture = this.RenderLensFlareScreenSpace(renderGraph, universalCameraData.camera, descriptor, originalBloomTexture, screenSpaceLensFlareBloomMipTexture, sameInputOutputTex);
				}
				this.UberPostSetupBloomPass(renderGraph, this.m_Materials.uber, descriptor);
			}
			if (flag4)
			{
				this.LensFlareDataDrivenComputeOcclusion(renderGraph, resourceData, universalCameraData, descriptor);
				this.RenderLensFlareDataDriven(renderGraph, resourceData, universalCameraData, textureHandle, descriptor);
			}
			this.SetupLensDistortion(this.m_Materials.uber, isSceneViewCamera);
			this.SetupChromaticAberration(this.m_Materials.uber);
			this.SetupVignette(this.m_Materials.uber, universalCameraData.xr, descriptor.width, descriptor.height);
			this.SetupGrain(universalCameraData, this.m_Materials.uber);
			this.SetupDithering(universalCameraData, this.m_Materials.uber);
			if (this.RequireSRGBConversionBlitToBackBuffer(universalCameraData.requireSrgbConversion))
			{
				this.m_Materials.uber.EnableKeyword("_LINEAR_TO_SRGB_CONVERSION");
			}
			if (this.m_UseFastSRGBLinearConversion)
			{
				this.m_Materials.uber.EnableKeyword("_USE_FAST_SRGB_LINEAR_CONVERSION");
			}
			bool flag12 = this.RequireHDROutput(universalCameraData);
			if (flag12)
			{
				HDROutputUtils.Operation hdrOperations = (!this.m_HasFinalPass && this.m_EnableColorEncodingIfNeeded) ? HDROutputUtils.Operation.ColorEncoding : HDROutputUtils.Operation.None;
				this.SetupHDROutput(universalCameraData.hdrDisplayInformation, universalCameraData.hdrDisplayColorGamut, this.m_Materials.uber, hdrOperations, universalCameraData.rendersOverlayUI);
			}
			bool isAlphaOutputEnabled = universalCameraData.isAlphaOutputEnabled;
			ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			this.RenderUberPost(renderGraph, frameData, universalCameraData, universalPostProcessingData, textureHandle, postProcessingTarget, lutTexture, originalBloomTexture, overlayUITexture, flag12, isAlphaOutputEnabled, resolveToDebugScreen, hasFinalPass);
		}

		[CompilerGenerated]
		private RTHandle <Render>g__GetSource|91_0(ref PostProcessPass.<>c__DisplayClass91_0 A_1)
		{
			return A_1.source;
		}

		[CompilerGenerated]
		private RTHandle <Render>g__GetDestination|91_1(ref PostProcessPass.<>c__DisplayClass91_0 A_1)
		{
			if (A_1.destination == null)
			{
				RenderTextureDescriptor compatibleDescriptor = this.GetCompatibleDescriptor();
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_TempTarget, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_TempTarget");
				A_1.destination = this.m_TempTarget;
			}
			else if (A_1.destination == this.m_Source && this.m_Descriptor.msaaSamples > 1)
			{
				RenderTextureDescriptor compatibleDescriptor = this.GetCompatibleDescriptor();
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_TempTarget2, compatibleDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_TempTarget2");
				A_1.destination = this.m_TempTarget2;
			}
			return A_1.destination;
		}

		[CompilerGenerated]
		private void <Render>g__Swap|91_2(ref ScriptableRenderer r, ref PostProcessPass.<>c__DisplayClass91_0 A_2)
		{
			int amountOfPassesRemaining = A_2.amountOfPassesRemaining - 1;
			A_2.amountOfPassesRemaining = amountOfPassesRemaining;
			if (this.m_UseSwapBuffer)
			{
				r.SwapColorBuffer(A_2.cmd);
				A_2.source = r.cameraColorTargetHandle;
				if (A_2.amountOfPassesRemaining == 0 && !this.m_HasFinalPass)
				{
					r.EnableSwapBufferMSAA(true);
				}
				A_2.destination = r.GetCameraColorFrontBuffer(A_2.cmd);
				return;
			}
			CoreUtils.Swap<RTHandle>(ref A_2.source, ref A_2.destination);
		}

		private RenderTextureDescriptor m_Descriptor;

		private RTHandle m_Source;

		private RTHandle m_Destination;

		private RTHandle m_Depth;

		private RTHandle m_InternalLut;

		private RTHandle m_MotionVectors;

		private RTHandle m_FullCoCTexture;

		private RTHandle m_HalfCoCTexture;

		private RTHandle m_PingTexture;

		private RTHandle m_PongTexture;

		private RTHandle[] m_BloomMipDown;

		private RTHandle[] m_BloomMipUp;

		private string[] m_BloomMipDownName;

		private string[] m_BloomMipUpName;

		private TextureHandle[] _BloomMipUp;

		private TextureHandle[] _BloomMipDown;

		private RTHandle m_BlendTexture;

		private RTHandle m_EdgeColorTexture;

		private RTHandle m_EdgeStencilTexture;

		private RTHandle m_TempTarget;

		private RTHandle m_TempTarget2;

		private RTHandle m_StreakTmpTexture;

		private RTHandle m_StreakTmpTexture2;

		private RTHandle m_ScreenSpaceLensFlareResult;

		private RTHandle m_UserLut;

		private const string k_RenderPostProcessingTag = "Blit PostProcessing Effects";

		private const string k_RenderFinalPostProcessingTag = "Blit Final PostProcessing";

		private static readonly ProfilingSampler m_ProfilingRenderPostProcessing = new ProfilingSampler("Blit PostProcessing Effects");

		private static readonly ProfilingSampler m_ProfilingRenderFinalPostProcessing = new ProfilingSampler("Blit Final PostProcessing");

		private PostProcessPass.MaterialLibrary m_Materials;

		private PostProcessData m_Data;

		private DepthOfField m_DepthOfField;

		private MotionBlur m_MotionBlur;

		private ScreenSpaceLensFlare m_LensFlareScreenSpace;

		private PaniniProjection m_PaniniProjection;

		private Bloom m_Bloom;

		private LensDistortion m_LensDistortion;

		private ChromaticAberration m_ChromaticAberration;

		private Vignette m_Vignette;

		private ColorLookup m_ColorLookup;

		private ColorAdjustments m_ColorAdjustments;

		private Tonemapping m_Tonemapping;

		private FilmGrain m_FilmGrain;

		private const int k_GaussianDoFPassComputeCoc = 0;

		private const int k_GaussianDoFPassDownscalePrefilter = 1;

		private const int k_GaussianDoFPassBlurH = 2;

		private const int k_GaussianDoFPassBlurV = 3;

		private const int k_GaussianDoFPassComposite = 4;

		private const int k_BokehDoFPassComputeCoc = 0;

		private const int k_BokehDoFPassDownscalePrefilter = 1;

		private const int k_BokehDoFPassBlur = 2;

		private const int k_BokehDoFPassPostFilter = 3;

		private const int k_BokehDoFPassComposite = 4;

		private const int k_MaxPyramidSize = 16;

		private readonly GraphicsFormat m_DefaultColorFormat;

		private bool m_DefaultColorFormatIsAlpha;

		private readonly GraphicsFormat m_SMAAEdgeFormat;

		private readonly GraphicsFormat m_GaussianCoCFormat;

		private int m_DitheringTextureIndex;

		private RenderTargetIdentifier[] m_MRT2;

		private Vector4[] m_BokehKernel;

		private int m_BokehHash;

		private float m_BokehMaxRadius;

		private float m_BokehRCPAspect;

		private bool m_IsFinalPass;

		private bool m_HasFinalPass;

		private bool m_EnableColorEncodingIfNeeded;

		private bool m_UseFastSRGBLinearConversion;

		private bool m_SupportScreenSpaceLensFlare;

		private bool m_SupportDataDrivenLensFlare;

		private bool m_ResolveToScreen;

		private bool m_UseSwapBuffer;

		private RTHandle m_ScalingSetupTarget;

		private RTHandle m_UpscaledTarget;

		private Material m_BlitMaterial;

		private PostProcessPass.BloomMaterialParams m_BloomParamsPrev;

		internal static readonly int k_ShaderPropertyId_ViewProjM = Shader.PropertyToID("_ViewProjM");

		internal static readonly int k_ShaderPropertyId_PrevViewProjM = Shader.PropertyToID("_PrevViewProjM");

		internal static readonly int k_ShaderPropertyId_ViewProjMStereo = Shader.PropertyToID("_ViewProjMStereo");

		internal static readonly int k_ShaderPropertyId_PrevViewProjMStereo = Shader.PropertyToID("_PrevViewProjMStereo");

		private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

		private const string _TemporalAATargetName = "_TemporalAATarget";

		private const string _UpscaledColorTargetName = "_CameraColorUpscaledSTP";

		private class MaterialLibrary
		{
			public MaterialLibrary(PostProcessData data)
			{
				this.stopNaN = this.Load(data.shaders.stopNanPS);
				this.subpixelMorphologicalAntialiasing = this.Load(data.shaders.subpixelMorphologicalAntialiasingPS);
				this.gaussianDepthOfField = this.Load(data.shaders.gaussianDepthOfFieldPS);
				this.gaussianDepthOfFieldCoC = this.Load(data.shaders.gaussianDepthOfFieldPS);
				this.bokehDepthOfField = this.Load(data.shaders.bokehDepthOfFieldPS);
				this.bokehDepthOfFieldCoC = this.Load(data.shaders.bokehDepthOfFieldPS);
				this.cameraMotionBlur = this.Load(data.shaders.cameraMotionBlurPS);
				this.paniniProjection = this.Load(data.shaders.paniniProjectionPS);
				this.bloom = this.Load(data.shaders.bloomPS);
				this.temporalAntialiasing = this.Load(data.shaders.temporalAntialiasingPS);
				this.scalingSetup = this.Load(data.shaders.scalingSetupPS);
				this.easu = this.Load(data.shaders.easuPS);
				this.uber = this.Load(data.shaders.uberPostPS);
				this.finalPass = this.Load(data.shaders.finalPostPassPS);
				this.lensFlareDataDriven = this.Load(data.shaders.LensFlareDataDrivenPS);
				this.lensFlareScreenSpace = this.Load(data.shaders.LensFlareScreenSpacePS);
				this.bloomUpsample = new Material[16];
				for (uint num = 0U; num < 16U; num += 1U)
				{
					this.bloomUpsample[(int)num] = this.Load(data.shaders.bloomPS);
				}
			}

			private Material Load(Shader shader)
			{
				if (shader == null)
				{
					Debug.LogErrorFormat("Missing shader. PostProcessing render passes will not execute. Check for missing reference in the renderer resources.", Array.Empty<object>());
					return null;
				}
				if (!shader.isSupported)
				{
					return null;
				}
				return CoreUtils.CreateEngineMaterial(shader);
			}

			internal void Cleanup()
			{
				CoreUtils.Destroy(this.stopNaN);
				CoreUtils.Destroy(this.subpixelMorphologicalAntialiasing);
				CoreUtils.Destroy(this.gaussianDepthOfField);
				CoreUtils.Destroy(this.gaussianDepthOfFieldCoC);
				CoreUtils.Destroy(this.bokehDepthOfField);
				CoreUtils.Destroy(this.bokehDepthOfFieldCoC);
				CoreUtils.Destroy(this.cameraMotionBlur);
				CoreUtils.Destroy(this.paniniProjection);
				CoreUtils.Destroy(this.bloom);
				CoreUtils.Destroy(this.temporalAntialiasing);
				CoreUtils.Destroy(this.scalingSetup);
				CoreUtils.Destroy(this.easu);
				CoreUtils.Destroy(this.uber);
				CoreUtils.Destroy(this.finalPass);
				CoreUtils.Destroy(this.lensFlareDataDriven);
				CoreUtils.Destroy(this.lensFlareScreenSpace);
				for (uint num = 0U; num < 16U; num += 1U)
				{
					CoreUtils.Destroy(this.bloomUpsample[(int)num]);
				}
			}

			public readonly Material stopNaN;

			public readonly Material subpixelMorphologicalAntialiasing;

			public readonly Material gaussianDepthOfField;

			public readonly Material gaussianDepthOfFieldCoC;

			public readonly Material bokehDepthOfField;

			public readonly Material bokehDepthOfFieldCoC;

			public readonly Material cameraMotionBlur;

			public readonly Material paniniProjection;

			public readonly Material bloom;

			public readonly Material[] bloomUpsample;

			public readonly Material temporalAntialiasing;

			public readonly Material scalingSetup;

			public readonly Material easu;

			public readonly Material uber;

			public readonly Material finalPass;

			public readonly Material lensFlareDataDriven;

			public readonly Material lensFlareScreenSpace;
		}

		private static class ShaderConstants
		{
			public static readonly int _TempTarget = Shader.PropertyToID("_TempTarget");

			public static readonly int _TempTarget2 = Shader.PropertyToID("_TempTarget2");

			public static readonly int _StencilRef = Shader.PropertyToID("_StencilRef");

			public static readonly int _StencilMask = Shader.PropertyToID("_StencilMask");

			public static readonly int _FullCoCTexture = Shader.PropertyToID("_FullCoCTexture");

			public static readonly int _HalfCoCTexture = Shader.PropertyToID("_HalfCoCTexture");

			public static readonly int _DofTexture = Shader.PropertyToID("_DofTexture");

			public static readonly int _CoCParams = Shader.PropertyToID("_CoCParams");

			public static readonly int _BokehKernel = Shader.PropertyToID("_BokehKernel");

			public static readonly int _BokehConstants = Shader.PropertyToID("_BokehConstants");

			public static readonly int _PongTexture = Shader.PropertyToID("_PongTexture");

			public static readonly int _PingTexture = Shader.PropertyToID("_PingTexture");

			public static readonly int _Metrics = Shader.PropertyToID("_Metrics");

			public static readonly int _AreaTexture = Shader.PropertyToID("_AreaTexture");

			public static readonly int _SearchTexture = Shader.PropertyToID("_SearchTexture");

			public static readonly int _EdgeTexture = Shader.PropertyToID("_EdgeTexture");

			public static readonly int _BlendTexture = Shader.PropertyToID("_BlendTexture");

			public static readonly int _ColorTexture = Shader.PropertyToID("_ColorTexture");

			public static readonly int _Params = Shader.PropertyToID("_Params");

			public static readonly int _SourceTexLowMip = Shader.PropertyToID("_SourceTexLowMip");

			public static readonly int _Bloom_Params = Shader.PropertyToID("_Bloom_Params");

			public static readonly int _Bloom_Texture = Shader.PropertyToID("_Bloom_Texture");

			public static readonly int _LensDirt_Texture = Shader.PropertyToID("_LensDirt_Texture");

			public static readonly int _LensDirt_Params = Shader.PropertyToID("_LensDirt_Params");

			public static readonly int _LensDirt_Intensity = Shader.PropertyToID("_LensDirt_Intensity");

			public static readonly int _Distortion_Params1 = Shader.PropertyToID("_Distortion_Params1");

			public static readonly int _Distortion_Params2 = Shader.PropertyToID("_Distortion_Params2");

			public static readonly int _Chroma_Params = Shader.PropertyToID("_Chroma_Params");

			public static readonly int _Vignette_Params1 = Shader.PropertyToID("_Vignette_Params1");

			public static readonly int _Vignette_Params2 = Shader.PropertyToID("_Vignette_Params2");

			public static readonly int _Vignette_ParamsXR = Shader.PropertyToID("_Vignette_ParamsXR");

			public static readonly int _Lut_Params = Shader.PropertyToID("_Lut_Params");

			public static readonly int _UserLut_Params = Shader.PropertyToID("_UserLut_Params");

			public static readonly int _InternalLut = Shader.PropertyToID("_InternalLut");

			public static readonly int _UserLut = Shader.PropertyToID("_UserLut");

			public static readonly int _DownSampleScaleFactor = Shader.PropertyToID("_DownSampleScaleFactor");

			public static readonly int _FlareOcclusionRemapTex = Shader.PropertyToID("_FlareOcclusionRemapTex");

			public static readonly int _FlareOcclusionTex = Shader.PropertyToID("_FlareOcclusionTex");

			public static readonly int _FlareOcclusionIndex = Shader.PropertyToID("_FlareOcclusionIndex");

			public static readonly int _FlareTex = Shader.PropertyToID("_FlareTex");

			public static readonly int _FlareColorValue = Shader.PropertyToID("_FlareColorValue");

			public static readonly int _FlareData0 = Shader.PropertyToID("_FlareData0");

			public static readonly int _FlareData1 = Shader.PropertyToID("_FlareData1");

			public static readonly int _FlareData2 = Shader.PropertyToID("_FlareData2");

			public static readonly int _FlareData3 = Shader.PropertyToID("_FlareData3");

			public static readonly int _FlareData4 = Shader.PropertyToID("_FlareData4");

			public static readonly int _FlareData5 = Shader.PropertyToID("_FlareData5");

			public static readonly int _FullscreenProjMat = Shader.PropertyToID("_FullscreenProjMat");
		}

		private class UpdateCameraResolutionPassData
		{
			internal Vector2Int newCameraTargetSize;
		}

		private class StopNaNsPassData
		{
			internal TextureHandle stopNaNTarget;

			internal TextureHandle sourceTexture;

			internal Material stopNaN;
		}

		private class SMAASetupPassData
		{
			internal Vector4 metrics;

			internal Texture2D areaTexture;

			internal Texture2D searchTexture;

			internal float stencilRef;

			internal float stencilMask;

			internal AntialiasingQuality antialiasingQuality;

			internal Material material;
		}

		private class SMAAPassData
		{
			internal TextureHandle sourceTexture;

			internal TextureHandle depthStencilTexture;

			internal TextureHandle blendTexture;

			internal Material material;
		}

		private class UberSetupBloomPassData
		{
			internal Vector4 bloomParams;

			internal Vector4 dirtScaleOffset;

			internal float dirtIntensity;

			internal Texture dirtTexture;

			internal bool highQualityFilteringValue;

			internal TextureHandle bloomTexture;

			internal Material uberMaterial;
		}

		private class BloomPassData
		{
			internal int mipCount;

			internal Material material;

			internal Material[] upsampleMaterials;

			internal TextureHandle sourceTexture;

			internal TextureHandle[] bloomMipUp;

			internal TextureHandle[] bloomMipDown;
		}

		internal struct BloomMaterialParams
		{
			internal bool Equals(ref PostProcessPass.BloomMaterialParams other)
			{
				return this.parameters == other.parameters && this.highQualityFiltering == other.highQualityFiltering && this.enableAlphaOutput == other.enableAlphaOutput;
			}

			internal Vector4 parameters;

			internal bool highQualityFiltering;

			internal bool enableAlphaOutput;
		}

		private class DoFGaussianPassData
		{
			internal int downsample;

			internal RenderingData renderingData;

			internal Vector3 cocParams;

			internal bool highQualitySamplingValue;

			internal TextureHandle sourceTexture;

			internal TextureHandle depthTexture;

			internal Material material;

			internal Material materialCoC;

			internal TextureHandle halfCoCTexture;

			internal TextureHandle fullCoCTexture;

			internal TextureHandle pingTexture;

			internal TextureHandle pongTexture;

			internal RenderTargetIdentifier[] multipleRenderTargets = new RenderTargetIdentifier[2];

			internal TextureHandle destination;
		}

		private class DoFBokehPassData
		{
			internal Vector4[] bokehKernel;

			internal int downSample;

			internal float uvMargin;

			internal Vector4 cocParams;

			internal bool useFastSRGBLinearConversion;

			internal TextureHandle sourceTexture;

			internal TextureHandle depthTexture;

			internal Material material;

			internal Material materialCoC;

			internal TextureHandle halfCoCTexture;

			internal TextureHandle fullCoCTexture;

			internal TextureHandle pingTexture;

			internal TextureHandle pongTexture;

			internal TextureHandle destination;
		}

		private class PaniniProjectionPassData
		{
			internal TextureHandle destinationTexture;

			internal TextureHandle sourceTexture;

			internal Material material;

			internal Vector4 paniniParams;

			internal bool isPaniniGeneric;
		}

		private class MotionBlurPassData
		{
			internal TextureHandle sourceTexture;

			internal TextureHandle motionVectors;

			internal Material material;

			internal int passIndex;

			internal Camera camera;

			internal XRPass xr;

			internal float intensity;

			internal float clamp;

			internal bool enableAlphaOutput;
		}

		private class LensFlarePassData
		{
			internal TextureHandle destinationTexture;

			internal UniversalCameraData cameraData;

			internal Material material;

			internal Rect viewport;

			internal float paniniDistance;

			internal float paniniCropToFit;

			internal float width;

			internal float height;

			internal bool usePanini;
		}

		private class LensFlareScreenSpacePassData
		{
			internal TextureHandle streakTmpTexture;

			internal TextureHandle streakTmpTexture2;

			internal TextureHandle originalBloomTexture;

			internal TextureHandle screenSpaceLensFlareBloomMipTexture;

			internal TextureHandle result;

			internal int actualWidth;

			internal int actualHeight;

			internal Camera camera;

			internal Material material;

			internal ScreenSpaceLensFlare lensFlareScreenSpace;

			internal int downsample;
		}

		private class PostProcessingFinalSetupPassData
		{
			internal TextureHandle destinationTexture;

			internal TextureHandle sourceTexture;

			internal Material material;

			internal UniversalCameraData cameraData;
		}

		private class PostProcessingFinalFSRScalePassData
		{
			internal TextureHandle sourceTexture;

			internal Material material;

			internal bool enableAlphaOutput;

			internal Vector2 fsrInputSize;

			internal Vector2 fsrOutputSize;
		}

		private class PostProcessingFinalBlitPassData
		{
			internal TextureHandle destinationTexture;

			internal TextureHandle sourceTexture;

			internal Material material;

			internal UniversalCameraData cameraData;

			internal PostProcessPass.FinalBlitSettings settings;
		}

		public struct FinalBlitSettings
		{
			public static PostProcessPass.FinalBlitSettings Create()
			{
				return new PostProcessPass.FinalBlitSettings
				{
					isFxaaEnabled = false,
					isFsrEnabled = false,
					isTaaSharpeningEnabled = false,
					requireHDROutput = false,
					resolveToDebugScreen = false,
					isAlphaOutputEnabled = false,
					hdrOperations = HDROutputUtils.Operation.None
				};
			}

			public bool isFxaaEnabled;

			public bool isFsrEnabled;

			public bool isTaaSharpeningEnabled;

			public bool requireHDROutput;

			public bool resolveToDebugScreen;

			public bool isAlphaOutputEnabled;

			public HDROutputUtils.Operation hdrOperations;
		}

		private class UberPostPassData
		{
			internal TextureHandle destinationTexture;

			internal TextureHandle sourceTexture;

			internal TextureHandle lutTexture;

			internal TextureHandle bloomTexture;

			internal Vector4 lutParams;

			internal TextureHandle userLutTexture;

			internal Vector4 userLutParams;

			internal Material material;

			internal UniversalCameraData cameraData;

			internal TonemappingMode toneMappingMode;

			internal bool isHdrGrading;

			internal bool isBackbuffer;

			internal bool enableAlphaOutput;

			internal bool hasFinalPass;
		}

		private class PostFXSetupPassData
		{
		}
	}
}
