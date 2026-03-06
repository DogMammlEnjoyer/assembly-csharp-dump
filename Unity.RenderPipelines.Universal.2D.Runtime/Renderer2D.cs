using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	internal sealed class Renderer2D : ScriptableRenderer
	{
		internal bool createColorTexture
		{
			get
			{
				return this.m_CreateColorTexture;
			}
		}

		internal bool createDepthTexture
		{
			get
			{
				return this.m_CreateDepthTexture;
			}
		}

		internal ColorGradingLutPass colorGradingLutPass
		{
			get
			{
				return this.m_PostProcessPasses.colorGradingLutPass;
			}
		}

		internal PostProcessPass postProcessPass
		{
			get
			{
				return this.m_PostProcessPasses.postProcessPass;
			}
		}

		internal PostProcessPass finalPostProcessPass
		{
			get
			{
				return this.m_PostProcessPasses.finalPostProcessPass;
			}
		}

		internal RTHandle afterPostProcessColorHandle
		{
			get
			{
				return this.m_PostProcessPasses.afterPostProcessColor;
			}
		}

		internal RTHandle colorGradingLutHandle
		{
			get
			{
				return this.m_PostProcessPasses.colorGradingLut;
			}
		}

		public override int SupportedCameraStackingTypes()
		{
			return 3;
		}

		public Renderer2D(Renderer2DData data) : base(data)
		{
			UniversalRenderPipelineRuntimeShaders universalRenderPipelineRuntimeShaders;
			if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeShaders>(out universalRenderPipelineRuntimeShaders))
			{
				this.m_BlitMaterial = CoreUtils.CreateEngineMaterial(universalRenderPipelineRuntimeShaders.coreBlitPS);
				this.m_BlitHDRMaterial = CoreUtils.CreateEngineMaterial(universalRenderPipelineRuntimeShaders.blitHDROverlay);
				this.m_SamplingMaterial = CoreUtils.CreateEngineMaterial(universalRenderPipelineRuntimeShaders.samplingPS);
			}
			Renderer2DResources renderer2DResources;
			if (GraphicsSettings.TryGetRenderPipelineSettings<Renderer2DResources>(out renderer2DResources))
			{
				this.m_Render2DLightingPass = new Render2DLightingPass(data, this.m_BlitMaterial, this.m_SamplingMaterial, renderer2DResources.fallOffLookup);
				this.m_CopyDepthPass = new CopyDepthPass(RenderPassEvent.AfterRenderingTransparents, renderer2DResources.copyDepthPS, true, false, RenderingUtils.MultisampleDepthResolveSupported(), null);
			}
			this.m_PixelPerfectBackgroundPass = new PixelPerfectBackgroundPass(RenderPassEvent.AfterRenderingTransparents);
			this.m_UpscalePass = new UpscalePass(RenderPassEvent.AfterRenderingPostProcessing, this.m_BlitMaterial);
			this.m_CopyCameraSortingLayerPass = new CopyCameraSortingLayerPass(this.m_BlitMaterial);
			this.m_FinalBlitPass = new FinalBlitPass((RenderPassEvent)1001, this.m_BlitMaterial, this.m_BlitHDRMaterial);
			this.m_DrawOffscreenUIPass = new DrawScreenSpaceUIPass(RenderPassEvent.BeforeRenderingPostProcessing, true);
			this.m_DrawOverlayUIPass = new DrawScreenSpaceUIPass((RenderPassEvent)1002, false);
			this.m_ColorBufferSystem = new RenderTargetBufferSystem("_CameraColorAttachment");
			PostProcessParams postProcessParams = PostProcessParams.Create();
			postProcessParams.blitMaterial = this.m_BlitMaterial;
			postProcessParams.requestColorFormat = GraphicsFormat.B10G11R11_UFloatPack32;
			this.m_PostProcessPasses = new PostProcessPasses(data.postProcessData, ref postProcessParams);
			this.m_UseDepthStencilBuffer = data.useDepthStencilBuffer;
			this.m_Renderer2DData = data;
			base.supportedRenderingFeatures = new ScriptableRenderer.RenderingFeatures();
			this.m_Renderer2DData.lightCullResult = new Light2DCullResult();
			LensFlareCommonSRP.mergeNeeded = 0;
			LensFlareCommonSRP.maxLensFlareWithOcclusionTemporalSample = 1;
			LensFlareCommonSRP.Initialize();
			Light2DManager.Initialize();
			PlatformAutoDetect.Initialize();
			UniversalRenderPipelineRuntimeXRResources universalRenderPipelineRuntimeXRResources;
			if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeXRResources>(out universalRenderPipelineRuntimeXRResources))
			{
				XRSystem.Initialize(new Func<XRPassCreateInfo, XRPass>(XRPassUniversal.Create), universalRenderPipelineRuntimeXRResources.xrOcclusionMeshPS, universalRenderPipelineRuntimeXRResources.xrMirrorViewPS);
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.m_Renderer2DData.Dispose();
			Render2DLightingPass render2DLightingPass = this.m_Render2DLightingPass;
			if (render2DLightingPass != null)
			{
				render2DLightingPass.Dispose();
			}
			this.m_PostProcessPasses.Dispose();
			RTHandle colorTextureHandle = this.m_ColorTextureHandle;
			if (colorTextureHandle != null)
			{
				colorTextureHandle.Release();
			}
			RTHandle depthTextureHandle = this.m_DepthTextureHandle;
			if (depthTextureHandle != null)
			{
				depthTextureHandle.Release();
			}
			this.ReleaseRenderTargets();
			this.m_UpscalePass.Dispose();
			CopyDepthPass copyDepthPass = this.m_CopyDepthPass;
			if (copyDepthPass != null)
			{
				copyDepthPass.Dispose();
			}
			FinalBlitPass finalBlitPass = this.m_FinalBlitPass;
			if (finalBlitPass != null)
			{
				finalBlitPass.Dispose();
			}
			DrawScreenSpaceUIPass drawOffscreenUIPass = this.m_DrawOffscreenUIPass;
			if (drawOffscreenUIPass != null)
			{
				drawOffscreenUIPass.Dispose();
			}
			DrawScreenSpaceUIPass drawOverlayUIPass = this.m_DrawOverlayUIPass;
			if (drawOverlayUIPass != null)
			{
				drawOverlayUIPass.Dispose();
			}
			Light2DManager.Dispose();
			XRSystem.Dispose();
			CoreUtils.Destroy(this.m_BlitMaterial);
			CoreUtils.Destroy(this.m_BlitHDRMaterial);
			CoreUtils.Destroy(this.m_SamplingMaterial);
			this.CleanupRenderGraphResources();
			base.Dispose(disposing);
		}

		internal override void ReleaseRenderTargets()
		{
			this.m_ColorBufferSystem.Dispose();
			this.m_PostProcessPasses.ReleaseRenderTargets();
		}

		public Renderer2DData GetRenderer2DData()
		{
			return this.m_Renderer2DData;
		}

		private Renderer2D.RenderPassInputSummary GetRenderPassInputs(UniversalCameraData cameraData)
		{
			Renderer2D.RenderPassInputSummary result = default(Renderer2D.RenderPassInputSummary);
			for (int i = 0; i < base.activeRenderPassQueue.Count; i++)
			{
				ScriptableRenderPass scriptableRenderPass = base.activeRenderPassQueue[i];
				bool flag = (scriptableRenderPass.input & ScriptableRenderPassInput.Depth) > ScriptableRenderPassInput.None;
				bool flag2 = (scriptableRenderPass.input & ScriptableRenderPassInput.Color) > ScriptableRenderPassInput.None;
				result.requiresDepthTexture = (result.requiresDepthTexture || flag);
				result.requiresColorTexture = (result.requiresColorTexture || flag2);
			}
			result.requiresColorTexture |= (cameraData.postProcessEnabled || cameraData.isHdrEnabled || cameraData.isSceneViewCamera || !cameraData.isDefaultViewport || cameraData.requireSrgbConversion || !cameraData.resolveFinalTarget || (cameraData.cameraTargetDescriptor.msaaSamples > 1 && UniversalRenderer.PlatformRequiresExplicitMsaaResolve()) || this.m_Renderer2DData.useCameraSortingLayerTexture || !Mathf.Approximately(cameraData.renderScale, 1f) || (base.DebugHandler != null && base.DebugHandler.WriteToDebugScreenTexture(cameraData.resolveFinalTarget)));
			return result;
		}

		private void CreateRenderTextures(ref Renderer2D.RenderPassInputSummary renderPassInputs, CommandBuffer cmd, UniversalCameraData cameraData, bool forceCreateColorTexture, FilterMode colorTextureFilterMode, out RTHandle colorTargetHandle, out RTHandle depthTargetHandle)
		{
			ref RenderTextureDescriptor ptr = ref cameraData.cameraTargetDescriptor;
			RenderTextureDescriptor desc = ptr;
			desc.depthStencilFormat = GraphicsFormat.None;
			this.m_ColorBufferSystem.SetCameraSettings(desc, colorTextureFilterMode);
			if (cameraData.renderType == CameraRenderType.Base)
			{
				this.m_CreateColorTexture = renderPassInputs.requiresColorTexture;
				this.m_CreateDepthTexture = renderPassInputs.requiresDepthTexture;
				this.m_CreateColorTexture = (this.m_CreateColorTexture || forceCreateColorTexture);
				this.m_CreateDepthTexture |= this.createColorTexture;
				if (this.createColorTexture)
				{
					if (this.m_ColorBufferSystem.PeekBackBuffer() == null || this.m_ColorBufferSystem.PeekBackBuffer().nameID != BuiltinRenderTextureType.CameraTarget)
					{
						this.m_ColorTextureHandle = this.m_ColorBufferSystem.GetBackBuffer(cmd);
						cmd.SetGlobalTexture("_CameraColorTexture", this.m_ColorTextureHandle.nameID);
						cmd.SetGlobalTexture("_AfterPostProcessTexture", this.m_ColorTextureHandle.nameID);
					}
					this.m_ColorTextureHandle = this.m_ColorBufferSystem.PeekBackBuffer();
				}
				if (this.createDepthTexture)
				{
					RenderTextureDescriptor renderTextureDescriptor = ptr;
					renderTextureDescriptor.colorFormat = RenderTextureFormat.Depth;
					renderTextureDescriptor.depthStencilFormat = CoreUtils.GetDefaultDepthStencilFormat();
					if (!cameraData.resolveFinalTarget && this.m_UseDepthStencilBuffer)
					{
						renderTextureDescriptor.bindMS = (renderTextureDescriptor.msaaSamples > 1 && !SystemInfo.supportsMultisampleAutoResolve && SystemInfo.supportsMultisampledTextures != 0);
					}
					RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_DepthTextureHandle, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_CameraDepthAttachment");
				}
				colorTargetHandle = (this.createColorTexture ? this.m_ColorTextureHandle : ScriptableRenderer.k_CameraTarget);
				depthTargetHandle = (this.createDepthTexture ? this.m_DepthTextureHandle : ScriptableRenderer.k_CameraTarget);
				return;
			}
			UniversalAdditionalCameraData universalAdditionalCameraData;
			cameraData.baseCamera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
			Renderer2D renderer2D = (Renderer2D)universalAdditionalCameraData.scriptableRenderer;
			if (this.m_ColorBufferSystem != renderer2D.m_ColorBufferSystem)
			{
				this.m_ColorBufferSystem.Dispose();
				this.m_ColorBufferSystem = renderer2D.m_ColorBufferSystem;
			}
			this.m_CreateColorTexture = true;
			this.m_CreateDepthTexture = true;
			this.m_ColorTextureHandle = renderer2D.m_ColorTextureHandle;
			this.m_DepthTextureHandle = renderer2D.m_DepthTextureHandle;
			colorTargetHandle = this.m_ColorTextureHandle;
			depthTargetHandle = this.m_DepthTextureHandle;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalRenderingData universalRenderingData = base.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalPostProcessingData universalPostProcessingData = base.frameData.Get<UniversalPostProcessingData>();
			ref RenderTextureDescriptor ptr = ref universalCameraData.cameraTargetDescriptor;
			bool flag = universalPostProcessingData.isEnabled && this.m_PostProcessPasses.isCreated;
			bool flag2 = universalCameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated;
			bool resolveFinalTarget = universalCameraData.resolveFinalTarget;
			FilterMode colorTextureFilterMode = FilterMode.Bilinear;
			PixelPerfectCamera pixelPerfectCamera = null;
			bool forceCreateColorTexture = false;
			bool flag3 = false;
			if (base.DebugHandler != null)
			{
				if (base.DebugHandler.AreAnySettingsActive)
				{
					flag = (flag && base.DebugHandler.IsPostProcessingAllowed);
					flag2 = (flag2 && base.DebugHandler.IsPostProcessingAllowed);
				}
				if (base.DebugHandler.IsActiveForCamera(universalCameraData.isPreviewCamera))
				{
					if (base.DebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget))
					{
						RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
						DebugHandler.ConfigureColorDescriptorForDebugScreen(ref cameraTargetDescriptor, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
						RenderingUtils.ReAllocateHandleIfNeeded(base.DebugHandler.DebugScreenColorHandle, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_DebugScreenColor");
						RenderTextureDescriptor cameraTargetDescriptor2 = universalCameraData.cameraTargetDescriptor;
						DebugHandler.ConfigureDepthDescriptorForDebugScreen(ref cameraTargetDescriptor2, CoreUtils.GetDefaultDepthStencilFormat(), universalCameraData.pixelWidth, universalCameraData.pixelHeight);
						RenderingUtils.ReAllocateHandleIfNeeded(base.DebugHandler.DebugScreenDepthHandle, cameraTargetDescriptor2, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_DebugScreenDepth");
					}
					if (base.DebugHandler.HDRDebugViewIsActive(universalCameraData.resolveFinalTarget))
					{
						base.DebugHandler.hdrDebugViewPass.Setup(universalCameraData, base.DebugHandler.DebugDisplaySettings.lightingSettings.hdrDebugMode);
						base.EnqueuePass(base.DebugHandler.hdrDebugViewPass);
					}
				}
			}
			if (universalCameraData.renderType == CameraRenderType.Base && resolveFinalTarget)
			{
				universalCameraData.camera.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
				if (pixelPerfectCamera != null && pixelPerfectCamera.enabled)
				{
					if (pixelPerfectCamera.offscreenRTSize != Vector2Int.zero)
					{
						forceCreateColorTexture = true;
						ptr.width = pixelPerfectCamera.offscreenRTSize.x;
						ptr.height = pixelPerfectCamera.offscreenRTSize.y;
						FullScreenPassRendererFeature.FullScreenRenderPass fullScreenRenderPass = base.activeRenderPassQueue.Find((ScriptableRenderPass x) => x is FullScreenPassRendererFeature.FullScreenRenderPass) as FullScreenPassRendererFeature.FullScreenRenderPass;
						if (fullScreenRenderPass != null)
						{
							fullScreenRenderPass.ReAllocate(ptr);
						}
					}
					colorTextureFilterMode = FilterMode.Point;
					flag3 = (pixelPerfectCamera.gridSnapping == PixelPerfectCamera.GridSnapping.UpscaleRenderTexture || pixelPerfectCamera.requiresUpscalePass);
				}
			}
			Renderer2D.RenderPassInputSummary renderPassInputs = this.GetRenderPassInputs(universalCameraData);
			CommandBuffer commandBuffer = universalRenderingData.commandBuffer;
			RTHandle rthandle;
			RTHandle rthandle2;
			using (new ProfilingScope(commandBuffer, Renderer2D.m_ProfilingSampler))
			{
				this.CreateRenderTextures(ref renderPassInputs, commandBuffer, universalCameraData, forceCreateColorTexture, colorTextureFilterMode, out rthandle, out rthandle2);
			}
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
			base.ConfigureCameraTarget(rthandle, rthandle2);
			if (flag2)
			{
				RenderTextureDescriptor renderTextureDescriptor;
				FilterMode filterMode;
				this.colorGradingLutPass.ConfigureDescriptor(universalPostProcessingData, out renderTextureDescriptor, out filterMode);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PostProcessPasses.m_ColorGradingLut, renderTextureDescriptor, filterMode, TextureWrapMode.Clamp, 1, 0f, "_InternalGradingLut");
				ColorGradingLutPass colorGradingLutPass = this.colorGradingLutPass;
				RTHandle colorGradingLutHandle = this.colorGradingLutHandle;
				colorGradingLutPass.Setup(colorGradingLutHandle);
				base.EnqueuePass(this.colorGradingLutPass);
			}
			this.m_Render2DLightingPass.Setup(renderPassInputs.requiresDepthTexture || this.m_UseDepthStencilBuffer);
			this.m_Render2DLightingPass.ConfigureTarget(rthandle, rthandle2);
			base.EnqueuePass(this.m_Render2DLightingPass);
			bool rendersOverlayUI = universalCameraData.rendersOverlayUI;
			bool isHDROutputActive = universalCameraData.isHDROutputActive;
			if (rendersOverlayUI && isHDROutputActive)
			{
				this.m_DrawOffscreenUIPass.Setup(universalCameraData, CoreUtils.GetDefaultDepthStencilFormat());
				base.EnqueuePass(this.m_DrawOffscreenUIPass);
			}
			bool flag4 = universalCameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing && !isHDROutputActive;
			bool flag5 = resolveFinalTarget && !flag3 && flag && flag4;
			bool flag6 = base.activeRenderPassQueue.Find((ScriptableRenderPass x) => x.renderPassEvent == RenderPassEvent.AfterRenderingPostProcessing) != null;
			bool flag7 = base.DebugHandler == null || !base.DebugHandler.HDRDebugViewIsActive(universalCameraData.resolveFinalTarget);
			bool flag8 = pixelPerfectCamera != null && pixelPerfectCamera.enabled;
			bool flag9 = universalCameraData.captureActions != null && resolveFinalTarget;
			bool flag10 = resolveFinalTarget && !flag9 && !flag6 && !flag5 && !flag8;
			bool enableColorEncoding = flag10 && flag7;
			if (flag2)
			{
				RenderTextureDescriptor compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(ptr, ptr.width, ptr.height, ptr.graphicsFormat, GraphicsFormat.None);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PostProcessPasses.m_AfterPostProcessColor, compatibleDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_AfterPostProcessTexture");
				PostProcessPass postProcessPass = this.postProcessPass;
				ref RenderTextureDescriptor baseDescriptor = ref ptr;
				bool resolveToScreen = flag10;
				RTHandle colorGradingLutHandle = this.colorGradingLutHandle;
				RTHandle rthandle3 = null;
				postProcessPass.Setup(baseDescriptor, rthandle, resolveToScreen, rthandle2, colorGradingLutHandle, rthandle3, flag5, enableColorEncoding);
				base.EnqueuePass(this.postProcessPass);
			}
			RTHandle colorHandle = rthandle;
			if (flag8 && pixelPerfectCamera.cropFrame != PixelPerfectCamera.CropFrame.None)
			{
				base.EnqueuePass(this.m_PixelPerfectBackgroundPass);
				if (pixelPerfectCamera.requiresUpscalePass)
				{
					int width = pixelPerfectCamera.refResolutionX * pixelPerfectCamera.pixelRatio;
					int height = pixelPerfectCamera.refResolutionY * pixelPerfectCamera.pixelRatio;
					this.m_UpscalePass.Setup(rthandle, width, height, pixelPerfectCamera.finalBlitFilterMode, universalCameraData.cameraTargetDescriptor, out colorHandle);
					base.EnqueuePass(this.m_UpscalePass);
				}
			}
			if (flag5)
			{
				this.finalPostProcessPass.SetupFinalPass(colorHandle, flag6, flag7);
				base.EnqueuePass(this.finalPostProcessPass);
			}
			if (!flag5 && (!flag2 || flag6 || flag9 || flag8) && !(rthandle.nameID == ScriptableRenderer.k_CameraTarget.nameID))
			{
				this.m_FinalBlitPass.Setup(ptr, colorHandle);
				base.EnqueuePass(this.m_FinalBlitPass);
			}
			if (rendersOverlayUI && universalCameraData.isLastBaseCamera && !isHDROutputActive)
			{
				base.EnqueuePass(this.m_DrawOverlayUIPass);
			}
		}

		public unsafe override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
		{
			cullingParameters.cullingOptions = CullingOptions.None;
			cullingParameters.isOrthographic = cameraData.camera->orthographic;
			cullingParameters.shadowDistance = 0f;
			(this.m_Renderer2DData.lightCullResult as Light2DCullResult).SetupCulling(ref cullingParameters, *cameraData.camera);
		}

		internal override void SwapColorBuffer(CommandBuffer cmd)
		{
			this.m_ColorBufferSystem.Swap();
			if (this.m_DepthTextureHandle.nameID != BuiltinRenderTextureType.CameraTarget)
			{
				base.ConfigureCameraTarget(this.m_ColorBufferSystem.GetBackBuffer(cmd), this.m_DepthTextureHandle);
			}
			else
			{
				base.ConfigureCameraColorTarget(this.m_ColorBufferSystem.GetBackBuffer(cmd));
			}
			this.m_ColorTextureHandle = this.m_ColorBufferSystem.GetBackBuffer(cmd);
			cmd.SetGlobalTexture("_CameraColorTexture", this.m_ColorTextureHandle.nameID);
			cmd.SetGlobalTexture("_AfterPostProcessTexture", this.m_ColorTextureHandle.nameID);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal override RTHandle GetCameraColorFrontBuffer(CommandBuffer cmd)
		{
			return this.m_ColorBufferSystem.GetFrontBuffer(cmd);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal override RTHandle GetCameraColorBackBuffer(CommandBuffer cmd)
		{
			return this.m_ColorBufferSystem.GetBackBuffer(cmd);
		}

		internal override void EnableSwapBufferMSAA(bool enable)
		{
			this.m_ColorBufferSystem.EnableMSAA(enable);
		}

		internal static bool IsGLESDevice()
		{
			return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3;
		}

		internal static bool supportsMRT
		{
			get
			{
				return !Renderer2D.IsGLESDevice();
			}
		}

		internal override bool supportsNativeRenderPassRendergraphCompiler
		{
			get
			{
				return true;
			}
		}

		private RTHandle currentRenderGraphCameraColorHandle
		{
			get
			{
				return this.m_RenderGraphCameraColorHandles[Renderer2D.m_CurrentColorHandle];
			}
		}

		private RTHandle nextRenderGraphCameraColorHandle
		{
			get
			{
				Renderer2D.m_CurrentColorHandle = (Renderer2D.m_CurrentColorHandle + 1) % 2;
				return this.currentRenderGraphCameraColorHandle;
			}
		}

		private bool IsPixelPerfectCameraEnabled(UniversalCameraData cameraData)
		{
			PixelPerfectCamera pixelPerfectCamera = null;
			if (cameraData.renderType == CameraRenderType.Base && cameraData.resolveFinalTarget)
			{
				cameraData.camera.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
			}
			return pixelPerfectCamera != null && pixelPerfectCamera.enabled && pixelPerfectCamera.cropFrame > PixelPerfectCamera.CropFrame.None;
		}

		private Renderer2D.ImportResourceSummary GetImportResourceSummary(RenderGraph renderGraph, UniversalCameraData cameraData)
		{
			Renderer2D.ImportResourceSummary importResourceSummary = default(Renderer2D.ImportResourceSummary);
			bool clearOnFirstUse = cameraData.renderType == CameraRenderType.Base;
			bool clearOnFirstUse2 = cameraData.renderType == CameraRenderType.Base || cameraData.clearDepth;
			bool flag = this.IsPixelPerfectCameraEnabled(cameraData);
			bool clearOnFirstUse3 = cameraData.renderType == CameraRenderType.Base && (!this.m_CreateColorTexture || flag);
			bool clearOnFirstUse4 = cameraData.renderType == CameraRenderType.Base && !this.m_CreateColorTexture;
			Color color = (cameraData.camera.clearFlags == CameraClearFlags.Nothing) ? Color.yellow : cameraData.backgroundColor;
			Color clearColor = flag ? Color.black : color;
			if (base.IsSceneFilteringEnabled(cameraData.camera))
			{
				color.a = 0f;
				clearOnFirstUse2 = false;
			}
			DebugHandler debugHandler = cameraData.renderer.DebugHandler;
			if (debugHandler != null && debugHandler.IsActiveForCamera(cameraData.isPreviewCamera) && debugHandler.IsScreenClearNeeded)
			{
				clearOnFirstUse = true;
				clearOnFirstUse2 = true;
				debugHandler.TryGetScreenClearColor(ref color);
			}
			importResourceSummary.cameraColorParams.clearOnFirstUse = clearOnFirstUse;
			importResourceSummary.cameraColorParams.clearColor = color;
			importResourceSummary.cameraColorParams.discardOnLastUse = false;
			importResourceSummary.cameraDepthParams.clearOnFirstUse = clearOnFirstUse2;
			importResourceSummary.cameraDepthParams.clearColor = color;
			importResourceSummary.cameraDepthParams.discardOnLastUse = false;
			importResourceSummary.backBufferColorParams.clearOnFirstUse = clearOnFirstUse3;
			importResourceSummary.backBufferColorParams.clearColor = clearColor;
			importResourceSummary.backBufferColorParams.discardOnLastUse = false;
			importResourceSummary.backBufferDepthParams.clearOnFirstUse = clearOnFirstUse4;
			importResourceSummary.backBufferDepthParams.clearColor = clearColor;
			importResourceSummary.backBufferDepthParams.discardOnLastUse = true;
			bool flag2 = cameraData.targetTexture == null;
			if (cameraData.xr.enabled)
			{
				flag2 = false;
			}
			if (!flag2)
			{
				if (cameraData.xr.enabled)
				{
					importResourceSummary.importInfo.width = cameraData.xr.renderTargetDesc.width;
					importResourceSummary.importInfo.height = cameraData.xr.renderTargetDesc.height;
					importResourceSummary.importInfo.volumeDepth = cameraData.xr.renderTargetDesc.volumeDepth;
					importResourceSummary.importInfo.msaaSamples = cameraData.xr.renderTargetDesc.msaaSamples;
					importResourceSummary.importInfo.format = cameraData.xr.renderTargetDesc.graphicsFormat;
					if (!UniversalRenderer.PlatformRequiresExplicitMsaaResolve())
					{
						importResourceSummary.importInfo.bindMS = (importResourceSummary.importInfo.msaaSamples > 1);
					}
					importResourceSummary.importInfoDepth = importResourceSummary.importInfo;
					importResourceSummary.importInfoDepth.format = cameraData.xr.renderTargetDesc.depthStencilFormat;
				}
				else
				{
					importResourceSummary.importInfo.width = cameraData.targetTexture.width;
					importResourceSummary.importInfo.height = cameraData.targetTexture.height;
					importResourceSummary.importInfo.volumeDepth = cameraData.targetTexture.volumeDepth;
					importResourceSummary.importInfo.msaaSamples = cameraData.targetTexture.antiAliasing;
					importResourceSummary.importInfo.format = cameraData.targetTexture.graphicsFormat;
					importResourceSummary.importInfoDepth = importResourceSummary.importInfo;
					importResourceSummary.importInfoDepth.format = cameraData.targetTexture.depthStencilFormat;
					if (importResourceSummary.importInfoDepth.format == GraphicsFormat.None)
					{
						importResourceSummary.importInfoDepth.format = SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil);
						Debug.LogWarning("In the render graph API, the output Render Texture must have a depth buffer. When you select a Render Texture in any camera's Output Texture property, the Depth Stencil Format property of the texture must be set to a value other than None.");
					}
				}
			}
			else
			{
				int msaaSamples = base.AdjustAndGetScreenMSAASamples(renderGraph, this.m_CreateColorTexture);
				importResourceSummary.importInfo.width = Screen.width;
				importResourceSummary.importInfo.height = Screen.height;
				importResourceSummary.importInfo.volumeDepth = 1;
				importResourceSummary.importInfo.msaaSamples = msaaSamples;
				importResourceSummary.importInfo.format = cameraData.cameraTargetDescriptor.graphicsFormat;
				importResourceSummary.importInfoDepth = importResourceSummary.importInfo;
				importResourceSummary.importInfoDepth.format = SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil);
			}
			return importResourceSummary;
		}

		private void InitializeLayerBatches()
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			this.m_LayerBatches = LayerUtility.CalculateBatches(this.m_Renderer2DData, out this.m_BatchCount);
			if (universal2DResourceData.normalsTexture.Length != this.m_BatchCount)
			{
				universal2DResourceData.normalsTexture = new TextureHandle[this.m_BatchCount];
			}
			if (universal2DResourceData.shadowTextures.Length != this.m_BatchCount)
			{
				universal2DResourceData.shadowTextures = new TextureHandle[this.m_BatchCount][];
			}
			if (universal2DResourceData.lightTextures.Length != this.m_BatchCount)
			{
				universal2DResourceData.lightTextures = new TextureHandle[this.m_BatchCount][];
			}
			for (int i = 0; i < universal2DResourceData.lightTextures.Length; i++)
			{
				if (universal2DResourceData.lightTextures[i] == null || universal2DResourceData.lightTextures[i].Length != this.m_LayerBatches[i].activeBlendStylesIndices.Length)
				{
					universal2DResourceData.lightTextures[i] = new TextureHandle[this.m_LayerBatches[i].activeBlendStylesIndices.Length];
				}
			}
			for (int j = 0; j < universal2DResourceData.shadowTextures.Length; j++)
			{
				if (universal2DResourceData.shadowTextures[j] == null || universal2DResourceData.shadowTextures[j].Length != this.m_LayerBatches[j].shadowIndices.Count)
				{
					universal2DResourceData.shadowTextures[j] = new TextureHandle[this.m_LayerBatches[j].shadowIndices.Count];
				}
			}
		}

		private void CreateResources(RenderGraph renderGraph)
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			ref RenderTextureDescriptor ptr = ref universalCameraData.cameraTargetDescriptor;
			FilterMode filterMode = FilterMode.Bilinear;
			bool resolveFinalTarget = universalCameraData.resolveFinalTarget;
			bool flag = false;
			if (universalCameraData.renderType == CameraRenderType.Base && resolveFinalTarget)
			{
				PixelPerfectCamera pixelPerfectCamera;
				universalCameraData.camera.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
				if (pixelPerfectCamera != null && pixelPerfectCamera.enabled)
				{
					if (pixelPerfectCamera.offscreenRTSize != Vector2Int.zero)
					{
						flag = true;
						ptr.width = pixelPerfectCamera.offscreenRTSize.x;
						ptr.height = pixelPerfectCamera.offscreenRTSize.y;
					}
					filterMode = FilterMode.Point;
					this.ppcUpscaleRT = (pixelPerfectCamera.gridSnapping == PixelPerfectCamera.GridSnapping.UpscaleRenderTexture || pixelPerfectCamera.requiresUpscalePass);
					if (pixelPerfectCamera.requiresUpscalePass)
					{
						RenderTextureDescriptor desc = ptr;
						desc.width = pixelPerfectCamera.refResolutionX * pixelPerfectCamera.pixelRatio;
						desc.height = pixelPerfectCamera.refResolutionY * pixelPerfectCamera.pixelRatio;
						desc.depthStencilFormat = GraphicsFormat.None;
						universal2DResourceData.upscaleTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_UpscaleTexture", true, pixelPerfectCamera.finalBlitFilterMode, TextureWrapMode.Clamp);
					}
				}
			}
			float lightRenderTextureScale = this.m_Renderer2DData.lightRenderTextureScale;
			int width = (int)Mathf.Max(1f, (float)universalCameraData.cameraTargetDescriptor.width * lightRenderTextureScale);
			int height = (int)Mathf.Max(1f, (float)universalCameraData.cameraTargetDescriptor.height * lightRenderTextureScale);
			this.CreateCameraNormalsTextures(renderGraph, ptr, width, height);
			this.CreateLightTextures(renderGraph, width, height);
			this.CreateShadowTextures(renderGraph, width, height);
			if (this.m_Renderer2DData.useCameraSortingLayerTexture)
			{
				this.CreateCameraSortingLayerTexture(renderGraph, ptr);
			}
			if (universalCameraData.renderType == CameraRenderType.Base)
			{
				Renderer2D.RenderPassInputSummary renderPassInputs = this.GetRenderPassInputs(universalCameraData);
				this.m_CreateColorTexture = renderPassInputs.requiresColorTexture;
				this.m_CreateDepthTexture = renderPassInputs.requiresDepthTexture;
				this.m_CreateColorTexture = (this.m_CreateColorTexture || flag);
				this.m_CreateDepthTexture |= this.createColorTexture;
				if (this.createColorTexture)
				{
					ptr.useMipMap = false;
					ptr.autoGenerateMips = false;
					ptr.depthStencilFormat = GraphicsFormat.None;
					RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_RenderGraphCameraColorHandles[0], ptr, filterMode, TextureWrapMode.Clamp, 1, 0f, "_CameraTargetAttachmentA");
					RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_RenderGraphCameraColorHandles[1], ptr, filterMode, TextureWrapMode.Clamp, 1, 0f, "_CameraTargetAttachmentB");
					universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.Camera;
				}
				else
				{
					universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
				}
				if (this.createDepthTexture)
				{
					RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
					cameraTargetDescriptor.useMipMap = false;
					cameraTargetDescriptor.autoGenerateMips = false;
					bool flag2 = cameraTargetDescriptor.msaaSamples > 1 && SystemInfo.supportsMultisampledTextures != 0;
					bool flag3 = RenderingUtils.MultisampleDepthResolveSupported() && renderGraph.nativeRenderPassesEnabled;
					cameraTargetDescriptor.bindMS = (!flag3 && flag2);
					if (Renderer2D.IsGLESDevice())
					{
						cameraTargetDescriptor.bindMS = false;
					}
					if (this.m_CopyDepthPass != null)
					{
						this.m_CopyDepthPass.MsaaSamples = cameraTargetDescriptor.msaaSamples;
						this.m_CopyDepthPass.m_CopyResolvedDepth = !cameraTargetDescriptor.bindMS;
					}
					cameraTargetDescriptor.graphicsFormat = GraphicsFormat.None;
					cameraTargetDescriptor.depthStencilFormat = CoreUtils.GetDefaultDepthStencilFormat();
					RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_RenderGraphCameraDepthHandle, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_CameraDepthAttachment");
					universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.Camera;
				}
				else
				{
					universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
				}
			}
			else
			{
				UniversalAdditionalCameraData universalAdditionalCameraData;
				universalCameraData.baseCamera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
				Renderer2D renderer2D = (Renderer2D)universalAdditionalCameraData.scriptableRenderer;
				this.m_RenderGraphCameraColorHandles = renderer2D.m_RenderGraphCameraColorHandles;
				this.m_RenderGraphCameraDepthHandle = renderer2D.m_RenderGraphCameraDepthHandle;
				this.m_RenderGraphBackbufferColorHandle = renderer2D.m_RenderGraphBackbufferColorHandle;
				this.m_RenderGraphBackbufferDepthHandle = renderer2D.m_RenderGraphBackbufferDepthHandle;
				this.m_CreateColorTexture = renderer2D.m_CreateColorTexture;
				this.m_CreateDepthTexture = renderer2D.m_CreateDepthTexture;
				universalResourceData.activeColorID = (this.m_CreateColorTexture ? UniversalResourceDataBase.ActiveID.Camera : UniversalResourceDataBase.ActiveID.BackBuffer);
				universalResourceData.activeDepthID = (this.m_CreateDepthTexture ? UniversalResourceDataBase.ActiveID.Camera : UniversalResourceDataBase.ActiveID.BackBuffer);
			}
			Renderer2D.ImportResourceSummary importResourceSummary = this.GetImportResourceSummary(renderGraph, universalCameraData);
			if (this.m_CreateColorTexture)
			{
				importResourceSummary.cameraColorParams.discardOnLastUse = resolveFinalTarget;
				importResourceSummary.cameraDepthParams.discardOnLastUse = resolveFinalTarget;
				universalResourceData.cameraColor = renderGraph.ImportTexture(this.currentRenderGraphCameraColorHandle, importResourceSummary.cameraColorParams);
				universalResourceData.cameraDepth = renderGraph.ImportTexture(this.m_RenderGraphCameraDepthHandle, importResourceSummary.cameraDepthParams);
			}
			RenderTargetIdentifier renderTargetIdentifier = (universalCameraData.targetTexture != null) ? new RenderTargetIdentifier(universalCameraData.targetTexture) : BuiltinRenderTextureType.CameraTarget;
			RenderTargetIdentifier renderTargetIdentifier2 = (universalCameraData.targetTexture != null) ? new RenderTargetIdentifier(universalCameraData.targetTexture) : BuiltinRenderTextureType.Depth;
			if (universalCameraData.xr.enabled)
			{
				renderTargetIdentifier = universalCameraData.xr.renderTarget;
				renderTargetIdentifier2 = universalCameraData.xr.renderTarget;
			}
			if (this.m_RenderGraphBackbufferColorHandle == null)
			{
				this.m_RenderGraphBackbufferColorHandle = RTHandles.Alloc(renderTargetIdentifier, "Backbuffer color");
			}
			else if (this.m_RenderGraphBackbufferColorHandle.nameID != renderTargetIdentifier)
			{
				RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_RenderGraphBackbufferColorHandle, renderTargetIdentifier);
			}
			if (this.m_RenderGraphBackbufferDepthHandle == null)
			{
				this.m_RenderGraphBackbufferDepthHandle = RTHandles.Alloc(renderTargetIdentifier2, "Backbuffer depth");
			}
			else if (this.m_RenderGraphBackbufferDepthHandle.nameID != renderTargetIdentifier2)
			{
				RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_RenderGraphBackbufferDepthHandle, renderTargetIdentifier2);
			}
			universalResourceData.backBufferColor = renderGraph.ImportTexture(this.m_RenderGraphBackbufferColorHandle, importResourceSummary.importInfo, importResourceSummary.backBufferColorParams);
			universalResourceData.backBufferDepth = renderGraph.ImportTexture(this.m_RenderGraphBackbufferDepthHandle, importResourceSummary.importInfoDepth, importResourceSummary.backBufferDepthParams);
			RenderTextureDescriptor compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(ptr, ptr.width, ptr.height, ptr.graphicsFormat, GraphicsFormat.None);
			universalResourceData.afterPostProcessColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, compatibleDescriptor, "_AfterPostProcessTexture", true, FilterMode.Point, TextureWrapMode.Clamp);
			if (this.RequiresDepthCopyPass(universalCameraData))
			{
				this.CreateCameraDepthCopyTexture(renderGraph, ptr);
			}
		}

		private void CreateCameraNormalsTextures(RenderGraph renderGraph, RenderTextureDescriptor descriptor, int width, int height)
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height);
			desc.graphicsFormat = RendererLighting.GetRenderTextureFormat();
			desc.autoGenerateMips = false;
			desc.msaaSamples = descriptor.msaaSamples;
			for (int i = 0; i < universal2DResourceData.normalsTexture.Length; i++)
			{
				universal2DResourceData.normalsTexture[i] = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_NormalMap", true, RendererLighting.k_NormalClearColor, FilterMode.Point, TextureWrapMode.Clamp, false);
			}
			if (this.m_Renderer2DData.useDepthStencilBuffer)
			{
				universal2DResourceData.normalsDepth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, new RenderTextureDescriptor(width, height)
				{
					graphicsFormat = GraphicsFormat.None,
					autoGenerateMips = false,
					msaaSamples = descriptor.msaaSamples,
					depthStencilFormat = CoreUtils.GetDefaultDepthStencilFormat()
				}, "_NormalDepth", false, FilterMode.Bilinear, TextureWrapMode.Clamp);
			}
		}

		private void CreateLightTextures(RenderGraph renderGraph, int width, int height)
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height);
			desc.graphicsFormat = RendererLighting.GetRenderTextureFormat();
			desc.autoGenerateMips = false;
			for (int i = 0; i < universal2DResourceData.lightTextures.Length; i++)
			{
				for (int j = 0; j < this.m_LayerBatches[i].activeBlendStylesIndices.Length; j++)
				{
					int num = this.m_LayerBatches[i].activeBlendStylesIndices[j];
					Color black;
					if (!Light2DManager.GetGlobalColor(this.m_LayerBatches[i].startLayerID, num, out black))
					{
						black = Color.black;
					}
					universal2DResourceData.lightTextures[i][j] = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, RendererLighting.k_ShapeLightTextureIDs[num], true, black, FilterMode.Bilinear, TextureWrapMode.Clamp, false);
				}
			}
		}

		private void CreateShadowTextures(RenderGraph renderGraph, int width, int height)
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height);
			desc.graphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;
			desc.autoGenerateMips = false;
			for (int i = 0; i < universal2DResourceData.shadowTextures.Length; i++)
			{
				for (int j = 0; j < this.m_LayerBatches[i].shadowIndices.Count; j++)
				{
					universal2DResourceData.shadowTextures[i][j] = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_ShadowTex", false, FilterMode.Bilinear, TextureWrapMode.Clamp);
				}
			}
			universal2DResourceData.shadowDepth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, new RenderTextureDescriptor(width, height)
			{
				graphicsFormat = GraphicsFormat.None,
				autoGenerateMips = false,
				depthStencilFormat = CoreUtils.GetDefaultDepthStencilFormat()
			}, "_ShadowDepth", false, FilterMode.Bilinear, TextureWrapMode.Clamp);
		}

		private void CreateCameraSortingLayerTexture(RenderGraph renderGraph, RenderTextureDescriptor descriptor)
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			descriptor.msaaSamples = 1;
			FilterMode filterMode;
			CopyCameraSortingLayerPass.ConfigureDescriptor(this.m_Renderer2DData.cameraSortingLayerDownsamplingMethod, ref descriptor, out filterMode);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_CameraSortingLayerHandle, descriptor, filterMode, TextureWrapMode.Clamp, 1, 0f, CopyCameraSortingLayerPass.k_CameraSortingLayerTexture);
			universal2DResourceData.cameraSortingLayerTexture = renderGraph.ImportTexture(this.m_CameraSortingLayerHandle);
		}

		private bool RequiresDepthCopyPass(UniversalCameraData cameraData)
		{
			Renderer2D.RenderPassInputSummary renderPassInputs = this.GetRenderPassInputs(cameraData);
			bool flag = cameraData.requiresDepthTexture || renderPassInputs.requiresDepthTexture;
			return ((cameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated && cameraData.postProcessingRequiresDepthTexture) || flag) && this.m_CreateDepthTexture;
		}

		private void CreateCameraDepthCopyTexture(RenderGraph renderGraph, RenderTextureDescriptor descriptor)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			RenderTextureDescriptor desc = descriptor;
			desc.msaaSamples = 1;
			desc.graphicsFormat = GraphicsFormat.R32_SFloat;
			desc.depthStencilFormat = GraphicsFormat.None;
			universalResourceData.cameraDepthTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_CameraDepthTexture", true, FilterMode.Point, TextureWrapMode.Clamp);
		}

		public override void OnBeginRenderGraphFrame()
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Create<Universal2DResourceData>();
			UniversalResourceDataBase orCreate = base.frameData.GetOrCreate<UniversalResourceData>();
			universal2DResourceData.InitFrame();
			orCreate.InitFrame();
		}

		internal void RecordCustomRenderGraphPasses(RenderGraph renderGraph, RenderPassEvent2D activeRPEvent)
		{
			foreach (ScriptableRenderPass scriptableRenderPass in base.activeRenderPassQueue)
			{
				RenderPassEvent2D renderPassEvent2D;
				int num;
				scriptableRenderPass.GetInjectionPoint2D(out renderPassEvent2D, out num);
				if (renderPassEvent2D == activeRPEvent)
				{
					scriptableRenderPass.RecordRenderGraph(renderGraph, base.frameData);
				}
			}
		}

		internal override void OnRecordRenderGraph(RenderGraph renderGraph, ScriptableRenderContext context)
		{
			UniversalResourceData orCreate = base.frameData.GetOrCreate<UniversalResourceData>();
			base.frameData.Get<UniversalCameraData>();
			this.InitializeLayerBatches();
			this.CreateResources(renderGraph);
			base.SetupRenderGraphCameraProperties(renderGraph, orCreate.isActiveTargetBackBuffer);
			base.ProcessVFXCameraCommand(renderGraph);
			this.OnBeforeRendering(renderGraph);
			this.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent2D.BeforeRendering);
			base.BeginRenderGraphXRRendering(renderGraph);
			this.OnMainRendering(renderGraph);
			this.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent2D.BeforeRenderingPostProcessing);
			this.OnAfterRendering(renderGraph);
			base.EndRenderGraphXRRendering(renderGraph);
		}

		public override void OnEndRenderGraphFrame()
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			UniversalResourceDataBase universalResourceDataBase = base.frameData.Get<UniversalResourceData>();
			universal2DResourceData.EndFrame();
			universalResourceDataBase.EndFrame();
		}

		internal override void OnFinishRenderGraphRendering(CommandBuffer cmd)
		{
			CopyDepthPass copyDepthPass = this.m_CopyDepthPass;
			if (copyDepthPass == null)
			{
				return;
			}
			copyDepthPass.OnCameraCleanup(cmd);
		}

		private void OnBeforeRendering(RenderGraph renderGraph)
		{
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			this.m_LightPass.Setup(renderGraph, ref this.m_Renderer2DData);
			List<Light2D> visibleLights = this.m_Renderer2DData.lightCullResult.visibleLights;
			for (int i = 0; i < visibleLights.Count; i++)
			{
				visibleLights[i].CacheValues();
			}
			ShadowCasterGroup2DManager.CacheValues();
			ShadowRendering.CallOnBeforeRender(universalCameraData.camera, this.m_Renderer2DData.lightCullResult);
			RendererLighting.lightBatch.Reset();
		}

		private void OnMainRendering(RenderGraph renderGraph)
		{
			base.frameData.Get<Universal2DResourceData>();
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			if (universalCameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated)
			{
				TextureHandle internalColorLut;
				this.m_PostProcessPasses.colorGradingLutPass.Render(renderGraph, base.frameData, out internalColorLut);
				universalResourceData.internalColorLut = internalColorLut;
			}
			short cameraSortingLayerBoundsIndex = Render2DLightingPass.GetCameraSortingLayerBoundsIndex(this.m_Renderer2DData);
			bool flag = false;
			for (int i = 0; i < this.m_BatchCount; i++)
			{
				flag |= this.m_LayerBatches[i].lightStats.useLights;
			}
			GlobalPropertiesPass.Setup(renderGraph, base.frameData, this.m_Renderer2DData, universalCameraData, flag);
			for (int j = 0; j < this.m_BatchCount; j++)
			{
				this.m_NormalPass.Render(renderGraph, base.frameData, this.m_Renderer2DData, ref this.m_LayerBatches[j], j);
			}
			for (int k = 0; k < this.m_BatchCount; k++)
			{
				this.m_ShadowPass.Render(renderGraph, base.frameData, this.m_Renderer2DData, ref this.m_LayerBatches[k], k, false);
			}
			for (int l = 0; l < this.m_BatchCount; l++)
			{
				this.m_LightPass.Render(renderGraph, base.frameData, this.m_Renderer2DData, ref this.m_LayerBatches[l], l, false);
			}
			for (int m = 0; m < this.m_BatchCount; m++)
			{
				if (!renderGraph.nativeRenderPassesEnabled && m == 0)
				{
					RTClearFlags cameraClearFlag = (RTClearFlags)ScriptableRenderer.GetCameraClearFlag(universalCameraData);
					if (cameraClearFlag != RTClearFlags.None)
					{
						ClearTargetsPass.Render(renderGraph, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, cameraClearFlag, universalCameraData.backgroundColor);
					}
				}
				ref LayerBatch ptr = ref this.m_LayerBatches[m];
				FilteringSettings filteringSettings;
				LayerUtility.GetFilterSettings(this.m_Renderer2DData, ref this.m_LayerBatches[m], out filteringSettings);
				this.m_RendererPass.Render(renderGraph, base.frameData, this.m_Renderer2DData, ref this.m_LayerBatches, m, ref filteringSettings);
				this.m_ShadowPass.Render(renderGraph, base.frameData, this.m_Renderer2DData, ref this.m_LayerBatches[m], m, true);
				this.m_LightPass.Render(renderGraph, base.frameData, this.m_Renderer2DData, ref this.m_LayerBatches[m], m, true);
				if (this.m_Renderer2DData.useCameraSortingLayerTexture && cameraSortingLayerBoundsIndex >= ptr.layerRange.lowerBound && cameraSortingLayerBoundsIndex <= ptr.layerRange.upperBound)
				{
					this.m_CopyCameraSortingLayerPass.Render(renderGraph, base.frameData);
				}
			}
			if (this.RequiresDepthCopyPass(universalCameraData))
			{
				CopyDepthPass copyDepthPass = this.m_CopyDepthPass;
				if (copyDepthPass != null)
				{
					copyDepthPass.Render(renderGraph, base.frameData, universalResourceData.cameraDepthTexture, universalResourceData.activeDepthTexture, true, "Copy Depth");
				}
			}
			bool rendersOverlayUI = universalCameraData.rendersOverlayUI;
			bool isHDROutputActive = universalCameraData.isHDROutputActive;
			if (rendersOverlayUI && isHDROutputActive)
			{
				TextureHandle overlayUITexture;
				this.m_DrawOffscreenUIPass.RenderOffscreen(renderGraph, base.frameData, CoreUtils.GetDefaultDepthStencilFormat(), out overlayUITexture);
				universalResourceData.overlayUITexture = overlayUITexture;
			}
		}

		private void OnAfterRendering(RenderGraph renderGraph)
		{
			Universal2DResourceData universal2DResourceData = base.frameData.Get<Universal2DResourceData>();
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			base.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalPostProcessingData universalPostProcessingData = base.frameData.Get<UniversalPostProcessingData>();
			bool flag = DebugDisplaySettings<UniversalRenderPipelineDebugDisplaySettings>.Instance.renderingSettings.sceneOverrideMode == DebugSceneOverrideMode.None;
			if (flag)
			{
				base.DrawRenderGraphGizmos(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, GizmoSubset.PreImageEffects);
			}
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			bool flag2 = activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget);
			if (flag2)
			{
				RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
				DebugHandler.ConfigureColorDescriptorForDebugScreen(ref cameraTargetDescriptor, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
				universalResourceData.debugScreenColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_DebugScreenColor", false, FilterMode.Point, TextureWrapMode.Clamp);
				RenderTextureDescriptor cameraTargetDescriptor2 = universalCameraData.cameraTargetDescriptor;
				DebugHandler.ConfigureDepthDescriptorForDebugScreen(ref cameraTargetDescriptor2, CoreUtils.GetDefaultDepthStencilFormat(), universalCameraData.pixelWidth, universalCameraData.pixelHeight);
				universalResourceData.debugScreenDepth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor2, "_DebugScreenDepth", false, FilterMode.Point, TextureWrapMode.Clamp);
			}
			bool flag3 = universalCameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated;
			bool flag4 = universalPostProcessingData.isEnabled && this.m_PostProcessPasses.isCreated;
			PixelPerfectCamera pixelPerfectCamera;
			universalCameraData.camera.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
			bool flag5 = this.IsPixelPerfectCameraEnabled(universalCameraData) && pixelPerfectCamera.requiresUpscalePass;
			bool flag6 = universalCameraData.resolveFinalTarget && !this.ppcUpscaleRT && flag4 && universalCameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing;
			bool flag7 = base.activeRenderPassQueue.Find((ScriptableRenderPass x) => x.renderPassEvent == RenderPassEvent.AfterRenderingPostProcessing) != null;
			bool flag8 = base.DebugHandler == null || !base.DebugHandler.HDRDebugViewIsActive(universalCameraData.resolveFinalTarget);
			bool flag9 = pixelPerfectCamera != null && pixelPerfectCamera.enabled;
			bool flag10 = universalCameraData.captureActions != null && universalCameraData.resolveFinalTarget;
			bool flag11 = universalCameraData.resolveFinalTarget && !flag10 && !flag7 && !flag6 && !flag9;
			bool enableColorEndingIfNeeded = flag11 && flag8;
			if (flag3)
			{
				TextureHandle activeColorTexture = universalResourceData.activeColorTexture;
				bool flag12 = flag11;
				if (!flag12)
				{
					ImportResourceParams importParams = default(ImportResourceParams);
					importParams.clearOnFirstUse = true;
					importParams.clearColor = Color.black;
					importParams.discardOnLastUse = universalCameraData.resolveFinalTarget;
					universalResourceData.cameraColor = renderGraph.ImportTexture(this.nextRenderGraphCameraColorHandle, importParams);
				}
				TextureHandle textureHandle = flag12 ? universalResourceData.backBufferColor : universalResourceData.cameraColor;
				if (flag2 && flag12)
				{
					textureHandle = universalResourceData.debugScreenColor;
				}
				PostProcessPass postProcessPass = this.postProcessPass;
				ContextContainer frameData = base.frameData;
				TextureHandle textureHandle2 = universalResourceData.internalColorLut;
				TextureHandle overlayUITexture = universalResourceData.overlayUITexture;
				postProcessPass.RenderPostProcessingRenderGraph(renderGraph, frameData, activeColorTexture, textureHandle2, overlayUITexture, textureHandle, flag6, flag2, enableColorEndingIfNeeded);
				if (flag12)
				{
					universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
					universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
				}
			}
			this.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent2D.AfterRenderingPostProcessing);
			TextureHandle textureHandle3 = universalResourceData.activeColorTexture;
			if (flag5)
			{
				UpscalePass upscalePass = this.m_UpscalePass;
				Camera camera = universalCameraData.camera;
				TextureHandle textureHandle2 = universal2DResourceData.upscaleTexture;
				upscalePass.Render(renderGraph, camera, textureHandle3, textureHandle2);
				textureHandle3 = universal2DResourceData.upscaleTexture;
			}
			TextureHandle textureHandle4 = flag2 ? universalResourceData.debugScreenColor : universalResourceData.backBufferColor;
			TextureHandle textureHandle5 = flag2 ? universalResourceData.debugScreenDepth : universalResourceData.backBufferDepth;
			if (flag6)
			{
				PostProcessPass postProcessPass2 = this.postProcessPass;
				ContextContainer frameData2 = base.frameData;
				TextureHandle textureHandle2 = universalResourceData.overlayUITexture;
				postProcessPass2.RenderFinalPassRenderGraph(renderGraph, frameData2, textureHandle3, textureHandle2, textureHandle4, flag8);
				textureHandle3 = textureHandle4;
				universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
				universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
			}
			bool flag13 = flag6 || (flag3 && !flag7 && !flag10 && !flag9);
			if (!universalResourceData.isActiveTargetBackBuffer && universalCameraData.resolveFinalTarget && !flag13)
			{
				this.m_FinalBlitPass.Render(renderGraph, base.frameData, universalCameraData, textureHandle3, textureHandle4, universalResourceData.overlayUITexture);
				textureHandle3 = textureHandle4;
				universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
				universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
			}
			bool flag14 = universalCameraData.rendersOverlayUI && universalCameraData.isLastBaseCamera;
			bool isHDROutputActive = universalCameraData.isHDROutputActive;
			if (flag14 && !isHDROutputActive)
			{
				this.m_DrawOverlayUIPass.RenderOverlay(renderGraph, base.frameData, textureHandle3, textureHandle5);
			}
			if (universalCameraData.resolveFinalTarget)
			{
				if (universalCameraData.isSceneViewCamera)
				{
					base.DrawRenderGraphWireOverlay(renderGraph, base.frameData, universalResourceData.backBufferColor);
				}
				if (flag)
				{
					base.DrawRenderGraphGizmos(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, GizmoSubset.PostImageEffects);
				}
			}
		}

		private void CleanupRenderGraphResources()
		{
			RTHandle rthandle = this.m_RenderGraphCameraColorHandles[0];
			if (rthandle != null)
			{
				rthandle.Release();
			}
			RTHandle rthandle2 = this.m_RenderGraphCameraColorHandles[1];
			if (rthandle2 != null)
			{
				rthandle2.Release();
			}
			RTHandle renderGraphCameraDepthHandle = this.m_RenderGraphCameraDepthHandle;
			if (renderGraphCameraDepthHandle != null)
			{
				renderGraphCameraDepthHandle.Release();
			}
			RTHandle renderGraphBackbufferColorHandle = this.m_RenderGraphBackbufferColorHandle;
			if (renderGraphBackbufferColorHandle != null)
			{
				renderGraphBackbufferColorHandle.Release();
			}
			RTHandle renderGraphBackbufferDepthHandle = this.m_RenderGraphBackbufferDepthHandle;
			if (renderGraphBackbufferDepthHandle != null)
			{
				renderGraphBackbufferDepthHandle.Release();
			}
			RTHandle cameraSortingLayerHandle = this.m_CameraSortingLayerHandle;
			if (cameraSortingLayerHandle != null)
			{
				cameraSortingLayerHandle.Release();
			}
			Light2DLookupTexture.Release();
		}

		private const int k_FinalBlitPassQueueOffset = 1;

		private const int k_AfterFinalBlitPassQueueOffset = 2;

		private Render2DLightingPass m_Render2DLightingPass;

		private PixelPerfectBackgroundPass m_PixelPerfectBackgroundPass;

		private UpscalePass m_UpscalePass;

		private CopyDepthPass m_CopyDepthPass;

		private CopyCameraSortingLayerPass m_CopyCameraSortingLayerPass;

		private FinalBlitPass m_FinalBlitPass;

		private DrawScreenSpaceUIPass m_DrawOffscreenUIPass;

		private DrawScreenSpaceUIPass m_DrawOverlayUIPass;

		internal RenderTargetBufferSystem m_ColorBufferSystem;

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Create Camera Textures");

		private bool m_UseDepthStencilBuffer = true;

		private bool m_CreateColorTexture;

		private bool m_CreateDepthTexture;

		internal RTHandle m_ColorTextureHandle;

		internal RTHandle m_DepthTextureHandle;

		private Material m_BlitMaterial;

		private Material m_BlitHDRMaterial;

		private Material m_SamplingMaterial;

		private Renderer2DData m_Renderer2DData;

		private PostProcessPasses m_PostProcessPasses;

		private static int m_CurrentColorHandle = 0;

		private RTHandle[] m_RenderGraphCameraColorHandles = new RTHandle[2];

		private RTHandle m_RenderGraphCameraDepthHandle;

		private RTHandle m_RenderGraphBackbufferColorHandle;

		private RTHandle m_RenderGraphBackbufferDepthHandle;

		private RTHandle m_CameraSortingLayerHandle;

		private DrawNormal2DPass m_NormalPass = new DrawNormal2DPass();

		private DrawLight2DPass m_LightPass = new DrawLight2DPass();

		private DrawShadow2DPass m_ShadowPass = new DrawShadow2DPass();

		private DrawRenderer2DPass m_RendererPass = new DrawRenderer2DPass();

		private LayerBatch[] m_LayerBatches;

		private int m_BatchCount;

		private bool ppcUpscaleRT;

		private struct RenderPassInputSummary
		{
			internal bool requiresDepthTexture;

			internal bool requiresColorTexture;
		}

		private struct ImportResourceSummary
		{
			internal RenderTargetInfo importInfo;

			internal RenderTargetInfo importInfoDepth;

			internal ImportResourceParams cameraColorParams;

			internal ImportResourceParams cameraDepthParams;

			internal ImportResourceParams backBufferColorParams;

			internal ImportResourceParams backBufferDepthParams;
		}
	}
}
