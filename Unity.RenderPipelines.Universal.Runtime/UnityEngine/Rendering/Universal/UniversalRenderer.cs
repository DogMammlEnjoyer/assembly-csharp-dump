using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.VFX;

namespace UnityEngine.Rendering.Universal
{
	public sealed class UniversalRenderer : ScriptableRenderer
	{
		public override int SupportedCameraStackingTypes()
		{
			switch (this.m_RenderingMode)
			{
			case RenderingMode.Forward:
			case RenderingMode.ForwardPlus:
				return 3;
			case RenderingMode.Deferred:
			case RenderingMode.DeferredPlus:
				return 1;
			default:
				return 0;
			}
		}

		protected internal override bool SupportsMotionVectors()
		{
			return true;
		}

		protected internal override bool SupportsCameraOpaque()
		{
			return true;
		}

		protected internal override bool SupportsCameraNormals()
		{
			return true;
		}

		internal RenderingMode renderingModeRequested
		{
			get
			{
				return this.m_RenderingMode;
			}
		}

		private bool deferredModeUnsupported
		{
			get
			{
				return GL.wireframe || (base.DebugHandler != null && base.DebugHandler.IsActiveModeUnsupportedForDeferred) || this.m_DeferredLights == null || !this.m_DeferredLights.IsRuntimeSupportedThisFrame();
			}
		}

		internal RenderingMode renderingModeActual
		{
			get
			{
				switch (this.renderingModeRequested)
				{
				case RenderingMode.Deferred:
					if (!this.deferredModeUnsupported)
					{
						return RenderingMode.Deferred;
					}
					return RenderingMode.Forward;
				case RenderingMode.DeferredPlus:
					if (GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>().enableRenderCompatibilityMode)
					{
						return RenderingMode.ForwardPlus;
					}
					if (!this.deferredModeUnsupported)
					{
						return RenderingMode.DeferredPlus;
					}
					return RenderingMode.ForwardPlus;
				}
				return this.renderingModeRequested;
			}
		}

		internal bool usesDeferredLighting
		{
			get
			{
				return this.renderingModeActual == RenderingMode.Deferred || this.renderingModeActual == RenderingMode.DeferredPlus;
			}
		}

		internal bool usesClusterLightLoop
		{
			get
			{
				return this.renderingModeActual == RenderingMode.ForwardPlus || this.renderingModeActual == RenderingMode.DeferredPlus;
			}
		}

		internal bool accurateGbufferNormals
		{
			get
			{
				return this.m_DeferredLights != null && this.m_DeferredLights.AccurateGbufferNormals;
			}
		}

		public DepthPrimingMode depthPrimingMode
		{
			get
			{
				return this.m_DepthPrimingMode;
			}
			set
			{
				this.m_DepthPrimingMode = value;
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

		internal RTHandle colorGradingLut
		{
			get
			{
				return this.m_PostProcessPasses.colorGradingLut;
			}
		}

		internal DeferredLights deferredLights
		{
			get
			{
				return this.m_DeferredLights;
			}
		}

		internal LayerMask prepassLayerMask { get; set; }

		internal LayerMask opaqueLayerMask { get; set; }

		internal LayerMask transparentLayerMask { get; set; }

		internal bool shadowTransparentReceive { get; set; }

		internal GraphicsFormat cameraDepthTextureFormat
		{
			get
			{
				if (this.m_CameraDepthTextureFormat == DepthFormat.Default)
				{
					return CoreUtils.GetDefaultDepthStencilFormat();
				}
				return (GraphicsFormat)this.m_CameraDepthTextureFormat;
			}
		}

		internal GraphicsFormat cameraDepthAttachmentFormat
		{
			get
			{
				if (this.m_CameraDepthAttachmentFormat == DepthFormat.Default)
				{
					return CoreUtils.GetDefaultDepthStencilFormat();
				}
				return (GraphicsFormat)this.m_CameraDepthAttachmentFormat;
			}
		}

		public UniversalRenderer(UniversalRendererData data) : base(data)
		{
			PlatformAutoDetect.Initialize();
			UniversalRenderPipelineRuntimeXRResources universalRenderPipelineRuntimeXRResources;
			if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeXRResources>(out universalRenderPipelineRuntimeXRResources))
			{
				XRSystem.Initialize(new Func<XRPassCreateInfo, XRPass>(XRPassUniversal.Create), universalRenderPipelineRuntimeXRResources.xrOcclusionMeshPS, universalRenderPipelineRuntimeXRResources.xrMirrorViewPS);
				this.m_XRDepthMotionPass = new XRDepthMotionPass(RenderPassEvent.BeforeRenderingPrePasses, universalRenderPipelineRuntimeXRResources.xrMotionVector);
			}
			UniversalRenderPipelineRuntimeShaders universalRenderPipelineRuntimeShaders;
			if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeShaders>(out universalRenderPipelineRuntimeShaders))
			{
				this.m_BlitMaterial = CoreUtils.CreateEngineMaterial(universalRenderPipelineRuntimeShaders.coreBlitPS);
				this.m_BlitHDRMaterial = CoreUtils.CreateEngineMaterial(universalRenderPipelineRuntimeShaders.blitHDROverlay);
				this.m_SamplingMaterial = CoreUtils.CreateEngineMaterial(universalRenderPipelineRuntimeShaders.samplingPS);
			}
			Shader copyDepthShader = null;
			UniversalRendererResources universalRendererResources;
			if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRendererResources>(out universalRendererResources))
			{
				copyDepthShader = universalRendererResources.copyDepthPS;
				this.m_StencilDeferredMaterial = CoreUtils.CreateEngineMaterial(universalRendererResources.stencilDeferredPS);
				this.m_ClusterDeferredMaterial = CoreUtils.CreateEngineMaterial(universalRendererResources.clusterDeferred);
				this.m_CameraMotionVecMaterial = CoreUtils.CreateEngineMaterial(universalRendererResources.cameraMotionVector);
				this.m_StencilCrossFadeRenderPass = new StencilCrossFadeRenderPass(universalRendererResources.stencilDitherMaskSeedPS);
			}
			StencilStateData defaultStencilState = data.defaultStencilState;
			this.m_DefaultStencilState = StencilState.defaultValue;
			this.m_DefaultStencilState.enabled = defaultStencilState.overrideStencilState;
			this.m_DefaultStencilState.SetCompareFunction(defaultStencilState.stencilCompareFunction);
			this.m_DefaultStencilState.SetPassOperation(defaultStencilState.passOperation);
			this.m_DefaultStencilState.SetFailOperation(defaultStencilState.failOperation);
			this.m_DefaultStencilState.SetZFailOperation(defaultStencilState.zFailOperation);
			this.m_IntermediateTextureMode = data.intermediateTextureMode;
			RenderGraphSettings renderGraphSettings;
			if (GraphicsSettings.TryGetRenderPipelineSettings<RenderGraphSettings>(out renderGraphSettings) && !renderGraphSettings.enableRenderCompatibilityMode)
			{
				this.prepassLayerMask = data.prepassLayerMask;
			}
			else
			{
				this.prepassLayerMask = data.opaqueLayerMask;
			}
			this.opaqueLayerMask = data.opaqueLayerMask;
			this.transparentLayerMask = data.transparentLayerMask;
			this.shadowTransparentReceive = data.shadowTransparentReceive;
			UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
			if (asset != null && asset.supportsLightCookies)
			{
				LightCookieManager.Settings settings = LightCookieManager.Settings.Create();
				UniversalRenderPipelineAsset asset2 = UniversalRenderPipeline.asset;
				if (asset2)
				{
					settings.atlas.format = asset2.additionalLightsCookieFormat;
					settings.atlas.resolution = asset2.additionalLightsCookieResolution;
				}
				this.m_LightCookieManager = new LightCookieManager(ref settings);
			}
			base.stripShadowsOffVariants = data.stripShadowsOffVariants;
			base.stripAdditionalLightOffVariants = data.stripAdditionalLightOffVariants;
			ForwardLights.InitParams initParams;
			initParams.lightCookieManager = this.m_LightCookieManager;
			initParams.forwardPlus = (data.renderingMode == RenderingMode.DeferredPlus || data.renderingMode == RenderingMode.ForwardPlus);
			this.m_ForwardLights = new ForwardLights(initParams);
			this.m_RenderingMode = data.renderingMode;
			this.m_DepthPrimingMode = data.depthPrimingMode;
			this.m_CopyDepthMode = data.copyDepthMode;
			this.m_CameraDepthAttachmentFormat = data.depthAttachmentFormat;
			this.m_CameraDepthTextureFormat = data.depthTextureFormat;
			this.useRenderPassEnabled = data.useNativeRenderPass;
			this.m_DepthPrimingRecommended = true;
			this.m_MainLightShadowCasterPass = new MainLightShadowCasterPass(RenderPassEvent.BeforeRenderingShadows);
			this.m_AdditionalLightsShadowCasterPass = new AdditionalLightsShadowCasterPass(RenderPassEvent.BeforeRenderingShadows);
			this.m_XROcclusionMeshPass = new XROcclusionMeshPass(RenderPassEvent.BeforeRenderingOpaques);
			this.m_XRCopyDepthPass = new CopyDepthPass((RenderPassEvent)1002, copyDepthShader, false, false, false, null);
			this.m_DepthPrepass = new DepthOnlyPass(RenderPassEvent.BeforeRenderingPrePasses, RenderQueueRange.opaque, this.prepassLayerMask);
			this.m_DepthNormalPrepass = new DepthNormalOnlyPass(RenderPassEvent.BeforeRenderingPrePasses, RenderQueueRange.opaque, this.prepassLayerMask);
			if (this.renderingModeRequested == RenderingMode.Forward || this.renderingModeRequested == RenderingMode.ForwardPlus)
			{
				this.m_PrimedDepthCopyPass = new CopyDepthPass(RenderPassEvent.AfterRenderingPrePasses, copyDepthShader, true, true, false, null);
			}
			if (this.renderingModeRequested == RenderingMode.Deferred || this.renderingModeRequested == RenderingMode.DeferredPlus)
			{
				this.m_DeferredLights = new DeferredLights(new DeferredLights.InitParams
				{
					stencilDeferredMaterial = this.m_StencilDeferredMaterial,
					clusterDeferredMaterial = this.m_ClusterDeferredMaterial,
					lightCookieManager = this.m_LightCookieManager,
					deferredPlus = (this.renderingModeRequested == RenderingMode.DeferredPlus)
				}, this.useRenderPassEnabled);
				this.m_DeferredLights.AccurateGbufferNormals = data.accurateGbufferNormals;
				this.m_GBufferPass = new GBufferPass(RenderPassEvent.BeforeRenderingGbuffer, RenderQueueRange.opaque, data.opaqueLayerMask, this.m_DefaultStencilState, defaultStencilState.stencilReference, this.m_DeferredLights);
				StencilState stencilState = DeferredLights.OverwriteStencil(this.m_DefaultStencilState, 96);
				ShaderTagId[] shaderTagIds = new ShaderTagId[]
				{
					new ShaderTagId("UniversalForwardOnly"),
					new ShaderTagId("SRPDefaultUnlit"),
					new ShaderTagId("LightweightForward")
				};
				int stencilReference = defaultStencilState.stencilReference | 0;
				this.m_GBufferCopyDepthPass = new CopyDepthPass((RenderPassEvent)211, copyDepthShader, true, false, false, "Copy GBuffer Depth");
				this.m_DeferredPass = new DeferredPass(RenderPassEvent.BeforeRenderingDeferredLights, this.m_DeferredLights);
				this.m_RenderOpaqueForwardOnlyPass = new DrawObjectsPass("Draw Opaques Forward Only", shaderTagIds, true, RenderPassEvent.BeforeRenderingOpaques, RenderQueueRange.opaque, data.opaqueLayerMask, stencilState, stencilReference);
			}
			this.m_RenderOpaqueForwardPass = new DrawObjectsPass(URPProfileId.DrawOpaqueObjects, true, RenderPassEvent.BeforeRenderingOpaques, RenderQueueRange.opaque, data.opaqueLayerMask, this.m_DefaultStencilState, defaultStencilState.stencilReference);
			this.m_RenderOpaqueForwardWithRenderingLayersPass = new DrawObjectsWithRenderingLayersPass(URPProfileId.DrawOpaqueObjects, true, RenderPassEvent.BeforeRenderingOpaques, RenderQueueRange.opaque, data.opaqueLayerMask, this.m_DefaultStencilState, defaultStencilState.stencilReference);
			bool flag = this.m_CopyDepthMode == CopyDepthMode.AfterTransparents;
			RenderPassEvent renderPassEvent = flag ? RenderPassEvent.AfterRenderingTransparents : RenderPassEvent.AfterRenderingSkybox;
			this.m_CopyDepthPass = new CopyDepthPass(renderPassEvent, copyDepthShader, true, false, RenderingUtils.MultisampleDepthResolveSupported() && flag, null);
			this.m_MotionVectorPass = new MotionVectorRenderPass(renderPassEvent + 1, this.m_CameraMotionVecMaterial, data.opaqueLayerMask);
			this.m_DrawSkyboxPass = new DrawSkyboxPass(RenderPassEvent.BeforeRenderingSkybox);
			this.m_CopyColorPass = new CopyColorPass(RenderPassEvent.AfterRenderingSkybox, this.m_SamplingMaterial, this.m_BlitMaterial, null);
			this.m_TransparentSettingsPass = new TransparentSettingsPass(RenderPassEvent.BeforeRenderingTransparents, data.shadowTransparentReceive);
			this.m_RenderTransparentForwardPass = new DrawObjectsPass(URPProfileId.DrawTransparentObjects, false, RenderPassEvent.BeforeRenderingTransparents, RenderQueueRange.transparent, data.transparentLayerMask, this.m_DefaultStencilState, defaultStencilState.stencilReference);
			this.m_OnRenderObjectCallbackPass = new InvokeOnRenderObjectCallbackPass(RenderPassEvent.BeforeRenderingPostProcessing);
			this.m_HistoryRawColorCopyPass = new CopyColorPass(RenderPassEvent.BeforeRenderingPostProcessing, this.m_SamplingMaterial, this.m_BlitMaterial, "Copy Color Raw History");
			this.m_HistoryRawDepthCopyPass = new CopyDepthPass(RenderPassEvent.BeforeRenderingPostProcessing, copyDepthShader, false, RenderingUtils.MultisampleDepthResolveSupported(), false, "Copy Depth Raw History");
			this.m_DrawOffscreenUIPass = new DrawScreenSpaceUIPass(RenderPassEvent.BeforeRenderingPostProcessing, true);
			this.m_DrawOverlayUIPass = new DrawScreenSpaceUIPass((RenderPassEvent)1002, false);
			PostProcessParams postProcessParams = PostProcessParams.Create();
			postProcessParams.blitMaterial = this.m_BlitMaterial;
			postProcessParams.requestColorFormat = GraphicsFormat.B10G11R11_UFloatPack32;
			UniversalRenderPipelineAsset asset3 = UniversalRenderPipeline.asset;
			if (asset3)
			{
				postProcessParams.requestColorFormat = UniversalRenderPipeline.MakeRenderTextureGraphicsFormat(asset3.supportsHDR, asset3.hdrColorBufferPrecision, false);
			}
			this.m_PostProcessPasses = new PostProcessPasses(data.postProcessData, ref postProcessParams);
			this.m_CapturePass = new CapturePass(RenderPassEvent.AfterRendering);
			this.m_FinalBlitPass = new FinalBlitPass((RenderPassEvent)1001, this.m_BlitMaterial, this.m_BlitHDRMaterial);
			this.m_ColorBufferSystem = new RenderTargetBufferSystem("_CameraColorAttachment");
			base.supportedRenderingFeatures = new ScriptableRenderer.RenderingFeatures();
			if (this.renderingModeRequested == RenderingMode.Deferred || this.renderingModeRequested == RenderingMode.DeferredPlus)
			{
				base.supportedRenderingFeatures.msaa = false;
			}
			LensFlareCommonSRP.mergeNeeded = 0;
			LensFlareCommonSRP.maxLensFlareWithOcclusionTemporalSample = 1;
			LensFlareCommonSRP.Initialize();
			this.m_VulkanEnablePreTransform = GraphicsSettings.HasShaderDefine(BuiltinShaderDefine.UNITY_PRETRANSFORM_TO_DISPLAY_ORIENTATION);
		}

		protected override void Dispose(bool disposing)
		{
			this.m_ForwardLights.Cleanup();
			GBufferPass gbufferPass = this.m_GBufferPass;
			if (gbufferPass != null)
			{
				gbufferPass.Dispose();
			}
			this.m_PostProcessPasses.Dispose();
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
			CopyDepthPass copyDepthPass = this.m_CopyDepthPass;
			if (copyDepthPass != null)
			{
				copyDepthPass.Dispose();
			}
			CopyDepthPass primedDepthCopyPass = this.m_PrimedDepthCopyPass;
			if (primedDepthCopyPass != null)
			{
				primedDepthCopyPass.Dispose();
			}
			CopyDepthPass gbufferCopyDepthPass = this.m_GBufferCopyDepthPass;
			if (gbufferCopyDepthPass != null)
			{
				gbufferCopyDepthPass.Dispose();
			}
			CopyDepthPass historyRawDepthCopyPass = this.m_HistoryRawDepthCopyPass;
			if (historyRawDepthCopyPass != null)
			{
				historyRawDepthCopyPass.Dispose();
			}
			CopyDepthPass xrcopyDepthPass = this.m_XRCopyDepthPass;
			if (xrcopyDepthPass != null)
			{
				xrcopyDepthPass.Dispose();
			}
			XRDepthMotionPass xrdepthMotionPass = this.m_XRDepthMotionPass;
			if (xrdepthMotionPass != null)
			{
				xrdepthMotionPass.Dispose();
			}
			StencilCrossFadeRenderPass stencilCrossFadeRenderPass = this.m_StencilCrossFadeRenderPass;
			if (stencilCrossFadeRenderPass != null)
			{
				stencilCrossFadeRenderPass.Dispose();
			}
			RTHandle targetColorHandle = this.m_TargetColorHandle;
			if (targetColorHandle != null)
			{
				targetColorHandle.Release();
			}
			RTHandle targetDepthHandle = this.m_TargetDepthHandle;
			if (targetDepthHandle != null)
			{
				targetDepthHandle.Release();
			}
			this.ReleaseRenderTargets();
			base.Dispose(disposing);
			CoreUtils.Destroy(this.m_BlitMaterial);
			CoreUtils.Destroy(this.m_BlitHDRMaterial);
			CoreUtils.Destroy(this.m_SamplingMaterial);
			CoreUtils.Destroy(this.m_StencilDeferredMaterial);
			CoreUtils.Destroy(this.m_ClusterDeferredMaterial);
			CoreUtils.Destroy(this.m_CameraMotionVecMaterial);
			this.CleanupRenderGraphResources();
			LensFlareCommonSRP.Dispose();
			XRSystem.Dispose();
		}

		internal override void ReleaseRenderTargets()
		{
			this.m_ColorBufferSystem.Dispose();
			if (this.m_DeferredLights != null && !this.m_DeferredLights.UseFramebufferFetch)
			{
				GBufferPass gbufferPass = this.m_GBufferPass;
				if (gbufferPass != null)
				{
					gbufferPass.Dispose();
				}
			}
			this.m_PostProcessPasses.ReleaseRenderTargets();
			MainLightShadowCasterPass mainLightShadowCasterPass = this.m_MainLightShadowCasterPass;
			if (mainLightShadowCasterPass != null)
			{
				mainLightShadowCasterPass.Dispose();
			}
			AdditionalLightsShadowCasterPass additionalLightsShadowCasterPass = this.m_AdditionalLightsShadowCasterPass;
			if (additionalLightsShadowCasterPass != null)
			{
				additionalLightsShadowCasterPass.Dispose();
			}
			RTHandle cameraDepthAttachment = this.m_CameraDepthAttachment;
			if (cameraDepthAttachment != null)
			{
				cameraDepthAttachment.Release();
			}
			RTHandle cameraDepthAttachment_D3d_ = this.m_CameraDepthAttachment_D3d_11;
			if (cameraDepthAttachment_D3d_ != null)
			{
				cameraDepthAttachment_D3d_.Release();
			}
			RTHandle depthTexture = this.m_DepthTexture;
			if (depthTexture != null)
			{
				depthTexture.Release();
			}
			RTHandle normalsTexture = this.m_NormalsTexture;
			if (normalsTexture != null)
			{
				normalsTexture.Release();
			}
			RTHandle decalLayersTexture = this.m_DecalLayersTexture;
			if (decalLayersTexture != null)
			{
				decalLayersTexture.Release();
			}
			RTHandle opaqueColor = this.m_OpaqueColor;
			if (opaqueColor != null)
			{
				opaqueColor.Release();
			}
			RTHandle motionVectorColor = this.m_MotionVectorColor;
			if (motionVectorColor != null)
			{
				motionVectorColor.Release();
			}
			RTHandle motionVectorDepth = this.m_MotionVectorDepth;
			if (motionVectorDepth != null)
			{
				motionVectorDepth.Release();
			}
			this.hasReleasedRTs = true;
		}

		private void SetupFinalPassDebug(UniversalCameraData cameraData)
		{
			if (base.DebugHandler != null && base.DebugHandler.IsActiveForCamera(cameraData.isPreviewCamera))
			{
				DebugFullScreenMode debugFullScreenMode;
				int num;
				if (base.DebugHandler.TryGetFullscreenDebugMode(out debugFullScreenMode, out num) && (debugFullScreenMode != DebugFullScreenMode.ReflectionProbeAtlas || this.usesClusterLightLoop))
				{
					Camera camera = cameraData.camera;
					float num2 = (float)camera.pixelWidth;
					float num3 = (float)camera.pixelHeight;
					float num4 = Mathf.Clamp01((float)num / 100f);
					float num5 = num4 * num3;
					float num6 = num4 * num2;
					RenderTexture renderTexture = null;
					if (debugFullScreenMode == DebugFullScreenMode.ReflectionProbeAtlas)
					{
						renderTexture = this.m_ForwardLights.reflectionProbeManager.atlasRT;
					}
					else if (debugFullScreenMode == DebugFullScreenMode.MainLightShadowMap)
					{
						renderTexture = this.m_MainLightShadowCasterPass.m_MainLightShadowmapTexture.rt;
					}
					else if (debugFullScreenMode == DebugFullScreenMode.AdditionalLightsShadowMap)
					{
						renderTexture = this.m_AdditionalLightsShadowCasterPass.m_AdditionalLightsShadowmapHandle.rt;
					}
					else if (debugFullScreenMode == DebugFullScreenMode.AdditionalLightsCookieAtlas && this.m_LightCookieManager != null)
					{
						LightCookieManager lightCookieManager = this.m_LightCookieManager;
						RenderTexture renderTexture2;
						if (lightCookieManager == null)
						{
							renderTexture2 = null;
						}
						else
						{
							RTHandle additionalLightsCookieAtlasTexture = lightCookieManager.AdditionalLightsCookieAtlasTexture;
							renderTexture2 = ((additionalLightsCookieAtlasTexture != null) ? additionalLightsCookieAtlasTexture.rt : null);
						}
						renderTexture = renderTexture2;
					}
					if (renderTexture != null)
					{
						this.CorrectForTextureAspectRatio(ref num6, ref num5, (float)renderTexture.width, (float)renderTexture.height);
					}
					float num7 = num6 / num2;
					float num8 = num5 / num3;
					Rect displayRect = new Rect(1f - num7, 1f - num8, num7, num8);
					Vector4 zero = Vector4.zero;
					switch (debugFullScreenMode)
					{
					case DebugFullScreenMode.Depth:
						base.DebugHandler.SetDebugRenderTarget(this.m_DepthTexture, displayRect, true, zero);
						return;
					case DebugFullScreenMode.MotionVector:
						zero.x = -0.01f;
						zero.y = 0.01f;
						zero.z = 0f;
						zero.w = 1f;
						base.DebugHandler.SetDebugRenderTarget(this.m_MotionVectorColor, displayRect, true, zero);
						return;
					case DebugFullScreenMode.AdditionalLightsShadowMap:
						base.DebugHandler.SetDebugRenderTarget(this.m_AdditionalLightsShadowCasterPass.m_AdditionalLightsShadowmapHandle, displayRect, false, zero);
						return;
					case DebugFullScreenMode.MainLightShadowMap:
						base.DebugHandler.SetDebugRenderTarget(this.m_MainLightShadowCasterPass.m_MainLightShadowmapTexture, displayRect, false, zero);
						return;
					case DebugFullScreenMode.AdditionalLightsCookieAtlas:
					{
						DebugHandler debugHandler = base.DebugHandler;
						LightCookieManager lightCookieManager2 = this.m_LightCookieManager;
						debugHandler.SetDebugRenderTarget((lightCookieManager2 != null) ? lightCookieManager2.AdditionalLightsCookieAtlasTexture : null, displayRect, false, zero);
						return;
					}
					case DebugFullScreenMode.ReflectionProbeAtlas:
						base.DebugHandler.SetDebugRenderTarget(this.m_ForwardLights.reflectionProbeManager.atlasRTHandle, displayRect, false, zero);
						return;
					default:
						return;
					}
				}
				else
				{
					base.DebugHandler.ResetDebugRenderTarget();
				}
			}
		}

		public static bool IsOffscreenDepthTexture(ref CameraData cameraData)
		{
			return UniversalRenderer.IsOffscreenDepthTexture(cameraData.universalCameraData);
		}

		public static bool IsOffscreenDepthTexture(UniversalCameraData cameraData)
		{
			return cameraData.targetTexture != null && cameraData.targetTexture.format == RenderTextureFormat.Depth;
		}

		private bool IsDepthPrimingEnabledCompatibilityMode(UniversalCameraData cameraData)
		{
			if (!UniversalRenderer.CanCopyDepth(cameraData))
			{
				return false;
			}
			bool flag = !UniversalRenderer.IsWebGL();
			bool flag2 = (this.m_DepthPrimingRecommended && this.m_DepthPrimingMode == DepthPrimingMode.Auto) || this.m_DepthPrimingMode == DepthPrimingMode.Forced;
			bool flag3 = this.m_RenderingMode == RenderingMode.Forward || this.m_RenderingMode == RenderingMode.ForwardPlus;
			bool flag4 = cameraData.renderType == CameraRenderType.Base || cameraData.clearDepth;
			bool flag5 = !UniversalRenderer.IsOffscreenDepthTexture(cameraData);
			bool flag6 = cameraData.cameraTargetDescriptor.msaaSamples == 1;
			return flag2 && flag3 && flag4 && flag5 && flag && flag6;
		}

		private static bool IsWebGL()
		{
			return false;
		}

		private static bool IsGLESDevice()
		{
			return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3;
		}

		private bool IsGLDevice()
		{
			return UniversalRenderer.IsGLESDevice() || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore;
		}

		internal bool HasActiveRenderFeatures()
		{
			if (base.rendererFeatures.Count == 0)
			{
				return false;
			}
			using (List<ScriptableRendererFeature>.Enumerator enumerator = base.rendererFeatures.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isActive)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool HasPassesRequiringIntermediateTexture()
		{
			if (base.activeRenderPassQueue.Count == 0)
			{
				return false;
			}
			using (List<ScriptableRenderPass>.Enumerator enumerator = base.activeRenderPassQueue.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.requiresIntermediateTexture)
					{
						return true;
					}
				}
			}
			return false;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalRenderingData universalRenderingData = base.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = base.frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = base.frameData.Get<UniversalShadowData>();
			UniversalPostProcessingData universalPostProcessingData = base.frameData.Get<UniversalPostProcessingData>();
			this.m_ForwardLights.PreSetup(universalRenderingData, universalCameraData, lightData);
			Camera camera = universalCameraData.camera;
			RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
			CommandBuffer commandBuffer = universalRenderingData.commandBuffer;
			if (base.DebugHandler != null && base.DebugHandler.IsActiveForCamera(universalCameraData.isPreviewCamera))
			{
				if (base.DebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget))
				{
					RenderTextureDescriptor cameraTargetDescriptor2 = universalCameraData.cameraTargetDescriptor;
					DebugHandler.ConfigureColorDescriptorForDebugScreen(ref cameraTargetDescriptor2, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
					RenderingUtils.ReAllocateHandleIfNeeded(base.DebugHandler.DebugScreenColorHandle, cameraTargetDescriptor2, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_DebugScreenColor");
					RenderTextureDescriptor cameraTargetDescriptor3 = universalCameraData.cameraTargetDescriptor;
					DebugHandler.ConfigureDepthDescriptorForDebugScreen(ref cameraTargetDescriptor3, this.cameraDepthTextureFormat, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
					RenderingUtils.ReAllocateHandleIfNeeded(base.DebugHandler.DebugScreenDepthHandle, cameraTargetDescriptor3, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_DebugScreenDepth");
				}
				if (base.DebugHandler.HDRDebugViewIsActive(universalCameraData.resolveFinalTarget))
				{
					base.DebugHandler.hdrDebugViewPass.Setup(universalCameraData, base.DebugHandler.DebugDisplaySettings.lightingSettings.hdrDebugMode);
					base.EnqueuePass(base.DebugHandler.hdrDebugViewPass);
				}
			}
			if (universalCameraData.cameraType != CameraType.Game)
			{
				this.useRenderPassEnabled = false;
			}
			base.useDepthPriming = this.IsDepthPrimingEnabledCompatibilityMode(universalCameraData);
			if (UniversalRenderer.IsOffscreenDepthTexture(universalCameraData))
			{
				base.ConfigureCameraTarget(ScriptableRenderer.k_CameraTarget, ScriptableRenderer.k_CameraTarget);
				base.EnqueuePass(this.m_RenderOpaqueForwardPass);
				base.EnqueuePass(this.m_RenderTransparentForwardPass);
				return;
			}
			bool isPreviewCamera = universalCameraData.isPreviewCamera;
			bool flag = (this.HasActiveRenderFeatures() && this.m_IntermediateTextureMode == IntermediateTextureMode.Always && !isPreviewCamera) || (Application.isEditor && this.usesClusterLightLoop);
			flag |= this.HasPassesRequiringIntermediateTexture();
			this.UpdateCameraHistory(universalCameraData);
			RenderingLayerUtils.Event @event;
			RenderingLayerUtils.MaskSize maskSize;
			bool flag2 = RenderingLayerUtils.RequireRenderingLayers(this, base.rendererFeatures, cameraTargetDescriptor.msaaSamples, out @event, out maskSize);
			if (this.IsGLDevice())
			{
				flag2 = false;
			}
			bool usesDeferredLighting = this.usesDeferredLighting;
			bool flag3 = false;
			bool flag4 = false;
			if (flag2 && !usesDeferredLighting)
			{
				if (@event != RenderingLayerUtils.Event.DepthNormalPrePass)
				{
					if (@event != RenderingLayerUtils.Event.Opaque)
					{
						throw new ArgumentOutOfRangeException();
					}
					flag4 = true;
				}
				else
				{
					flag3 = true;
				}
			}
			UniversalRenderer.RenderPassInputSummary renderPassInputs = this.GetRenderPassInputs(universalCameraData.IsTemporalAAEnabled(), universalPostProcessingData.isEnabled, universalCameraData.isSceneViewCamera, flag3);
			if (this.m_DeferredLights != null)
			{
				this.m_DeferredLights.RenderingLayerMaskSize = maskSize;
				this.m_DeferredLights.UseDecalLayers = flag2;
				this.m_DeferredLights.HasNormalPrepass = renderPassInputs.requiresNormalsTexture;
				this.m_DeferredLights.ResolveMixedLightingMode(lightData);
				this.m_DeferredLights.CreateGbufferResources();
				if (this.m_DeferredLights.UseFramebufferFetch)
				{
					foreach (ScriptableRenderPass scriptableRenderPass in base.activeRenderPassQueue)
					{
						if (scriptableRenderPass.renderPassEvent >= RenderPassEvent.AfterRenderingGbuffer && scriptableRenderPass.renderPassEvent <= RenderPassEvent.BeforeRenderingDeferredLights)
						{
							this.m_DeferredLights.DisableFramebufferFetchInput();
							break;
						}
					}
				}
			}
			bool flag5 = universalCameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated;
			bool flag6 = universalPostProcessingData.isEnabled && this.m_PostProcessPasses.isCreated;
			bool flag7 = flag5 && universalCameraData.postProcessingRequiresDepthTexture;
			bool flag8 = universalCameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated;
			bool flag9 = universalCameraData.isSceneViewCamera || universalCameraData.isPreviewCamera;
			object obj = universalCameraData.requiresDepthTexture || renderPassInputs.requiresDepthTexture || base.useDepthPriming;
			bool flag10 = false;
			bool flag11 = this.m_MainLightShadowCasterPass.Setup(universalRenderingData, universalCameraData, lightData, shadowData);
			bool flag12 = this.m_AdditionalLightsShadowCasterPass.Setup(universalRenderingData, universalCameraData, lightData, shadowData);
			bool flag13 = this.m_TransparentSettingsPass.Setup();
			bool flag14 = this.m_CopyDepthMode == CopyDepthMode.ForcePrepass;
			object obj2 = obj;
			bool flag15 = (obj2 | flag7) != null && (!UniversalRenderer.CanCopyDepth(universalCameraData) || flag14);
			flag15 = (flag15 || flag9);
			flag15 = (flag15 || flag10);
			flag15 = (flag15 || isPreviewCamera);
			flag15 |= renderPassInputs.requiresDepthPrepass;
			flag15 |= renderPassInputs.requiresNormalsTexture;
			if (flag15 && usesDeferredLighting && !renderPassInputs.requiresNormalsTexture)
			{
				flag15 = false;
			}
			flag15 |= base.useDepthPriming;
			if (obj2 != null)
			{
				RenderPassEvent renderPassEvent = (this.m_CopyDepthMode == CopyDepthMode.AfterTransparents) ? RenderPassEvent.AfterRenderingTransparents : RenderPassEvent.AfterRenderingOpaques;
				if (renderPassInputs.requiresDepthTexture)
				{
					renderPassEvent = (RenderPassEvent)Mathf.Min(500, renderPassInputs.requiresDepthTextureEarliestEvent - (RenderPassEvent)1);
				}
				this.m_CopyDepthPass.renderPassEvent = renderPassEvent;
				if (renderPassEvent < RenderPassEvent.AfterRenderingTransparents)
				{
					this.m_CopyDepthPass.m_CopyResolvedDepth = false;
					this.m_CopyDepthMode = CopyDepthMode.AfterOpaques;
				}
			}
			else if (flag7 || flag9 || flag10)
			{
				this.m_CopyDepthPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
			}
			flag |= this.RequiresIntermediateColorTexture(universalCameraData, renderPassInputs);
			flag &= !isPreviewCamera;
			bool flag16 = (obj2 | flag7) != null && !flag15;
			flag16 |= !universalCameraData.resolveFinalTarget;
			flag16 |= (usesDeferredLighting && !this.useRenderPassEnabled);
			flag16 |= base.useDepthPriming;
			flag16 = (flag16 || flag4);
			if (universalCameraData.xr.enabled)
			{
				flag = (flag || flag16);
			}
			if (RTHandles.rtHandleProperties.rtHandleScale.x != 1f || RTHandles.rtHandleProperties.rtHandleScale.y != 1f)
			{
				flag = (flag || flag16);
			}
			if (this.useRenderPassEnabled || base.useDepthPriming)
			{
				flag = (flag || flag16);
			}
			if (SystemInfo.graphicsUVStartsAtTop)
			{
				flag = (flag || flag16);
			}
			RenderTextureDescriptor desc = cameraTargetDescriptor;
			desc.useMipMap = false;
			desc.autoGenerateMips = false;
			desc.depthStencilFormat = GraphicsFormat.None;
			this.m_ColorBufferSystem.SetCameraSettings(desc, FilterMode.Bilinear);
			if (universalCameraData.renderType == CameraRenderType.Base)
			{
				bool flag17 = camera.sceneViewFilterMode == Camera.SceneViewFilterMode.ShowFiltered;
				bool flag18 = (flag || flag16) && !flag17;
				flag16 = (flag16 || flag);
				RenderTargetIdentifier renderTargetIdentifier = BuiltinRenderTextureType.CameraTarget;
				if (universalCameraData.xr.enabled)
				{
					renderTargetIdentifier = universalCameraData.xr.renderTarget;
				}
				if (this.m_TargetColorHandle == null)
				{
					this.m_TargetColorHandle = RTHandles.Alloc(renderTargetIdentifier);
				}
				else if (this.m_TargetColorHandle.nameID != renderTargetIdentifier)
				{
					RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_TargetColorHandle, renderTargetIdentifier);
				}
				if (this.m_TargetDepthHandle == null)
				{
					this.m_TargetDepthHandle = RTHandles.Alloc(renderTargetIdentifier);
				}
				else if (this.m_TargetDepthHandle.nameID != renderTargetIdentifier)
				{
					RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_TargetDepthHandle, renderTargetIdentifier);
				}
				if (flag18)
				{
					this.CreateCameraRenderTarget(context, ref cameraTargetDescriptor, commandBuffer, universalCameraData);
				}
				this.m_RenderOpaqueForwardPass.m_IsActiveTargetBackBuffer = !flag18;
				this.m_RenderTransparentForwardPass.m_IsActiveTargetBackBuffer = !flag18;
				this.m_XROcclusionMeshPass.m_IsActiveTargetBackBuffer = !flag18;
				this.m_ActiveCameraColorAttachment = (flag ? this.m_ColorBufferSystem.PeekBackBuffer() : this.m_TargetColorHandle);
				this.m_ActiveCameraDepthAttachment = (flag16 ? this.m_CameraDepthAttachment : this.m_TargetDepthHandle);
			}
			else
			{
				UniversalAdditionalCameraData universalAdditionalCameraData;
				universalCameraData.baseCamera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
				UniversalRenderer universalRenderer = (UniversalRenderer)universalAdditionalCameraData.scriptableRenderer;
				if (this.m_ColorBufferSystem != universalRenderer.m_ColorBufferSystem)
				{
					this.m_ColorBufferSystem.Dispose();
					this.m_ColorBufferSystem = universalRenderer.m_ColorBufferSystem;
				}
				this.m_ActiveCameraColorAttachment = this.m_ColorBufferSystem.PeekBackBuffer();
				this.m_ActiveCameraDepthAttachment = universalRenderer.m_ActiveCameraDepthAttachment;
				this.m_TargetColorHandle = universalRenderer.m_TargetColorHandle;
				this.m_TargetDepthHandle = universalRenderer.m_TargetDepthHandle;
			}
			if (base.rendererFeatures.Count != 0 && !isPreviewCamera)
			{
				base.ConfigureCameraColorTarget(this.m_ColorBufferSystem.PeekBackBuffer());
			}
			bool flag19 = universalCameraData.requiresOpaqueTexture || renderPassInputs.requiresColorTexture;
			flag19 &= !isPreviewCamera;
			base.ConfigureCameraTarget(this.m_ActiveCameraColorAttachment, this.m_ActiveCameraDepthAttachment);
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
			{
				commandBuffer.CopyTexture(this.m_CameraDepthAttachment, this.m_CameraDepthAttachment_D3d_11);
			}
			bool flag20 = base.activeRenderPassQueue.Find((ScriptableRenderPass x) => x.renderPassEvent == RenderPassEvent.AfterRenderingPostProcessing) != null;
			if (flag11)
			{
				base.EnqueuePass(this.m_MainLightShadowCasterPass);
			}
			if (flag12)
			{
				base.EnqueuePass(this.m_AdditionalLightsShadowCasterPass);
			}
			bool flag21 = !flag15 && (universalCameraData.requiresDepthTexture || flag7 || renderPassInputs.requiresDepthTexture) && flag16;
			if (base.DebugHandler != null && base.DebugHandler.IsActiveForCamera(universalCameraData.isPreviewCamera))
			{
				DebugFullScreenMode debugFullScreenMode;
				base.DebugHandler.TryGetFullscreenDebugMode(out debugFullScreenMode);
				if (debugFullScreenMode == DebugFullScreenMode.Depth)
				{
					flag15 = true;
				}
				if (!base.DebugHandler.IsLightingActive)
				{
					flag11 = false;
					flag12 = false;
					if (!flag9)
					{
						flag15 = false;
						base.useDepthPriming = false;
						flag8 = false;
						flag19 = false;
						flag21 = false;
					}
				}
				if (this.useRenderPassEnabled)
				{
					this.useRenderPassEnabled = base.DebugHandler.IsRenderPassSupported;
				}
			}
			universalCameraData.renderer.useDepthPriming = base.useDepthPriming;
			if (usesDeferredLighting && this.m_DeferredLights.UseFramebufferFetch && (RenderPassEvent.AfterRenderingGbuffer == renderPassInputs.requiresDepthNormalAtEvent || !this.useRenderPassEnabled))
			{
				this.m_DeferredLights.DisableFramebufferFetchInput();
			}
			if ((usesDeferredLighting && !this.useRenderPassEnabled) || flag15 || flag21)
			{
				RenderTextureDescriptor renderTextureDescriptor = cameraTargetDescriptor;
				if (flag15 && !usesDeferredLighting)
				{
					renderTextureDescriptor.graphicsFormat = GraphicsFormat.None;
					renderTextureDescriptor.depthStencilFormat = this.cameraDepthTextureFormat;
				}
				else
				{
					renderTextureDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
					renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
				}
				renderTextureDescriptor.msaaSamples = 1;
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_DepthTexture, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_CameraDepthTexture");
				commandBuffer.SetGlobalTexture(this.m_DepthTexture.name, this.m_DepthTexture.nameID);
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
			}
			bool flag22 = usesDeferredLighting && this.m_DeferredLights.UseRenderingLayers;
			if (flag2 || flag22)
			{
				ref RTHandle ptr = ref this.m_DecalLayersTexture;
				string name = "_CameraRenderingLayersTexture";
				if (flag22)
				{
					ptr = ref this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GBufferRenderingLayers];
					name = ptr.name;
				}
				RenderTextureDescriptor gbufferSlice = cameraTargetDescriptor;
				gbufferSlice.depthStencilFormat = GraphicsFormat.None;
				if (!flag4)
				{
					gbufferSlice.msaaSamples = 1;
				}
				if (flag22)
				{
					gbufferSlice.graphicsFormat = this.m_DeferredLights.GetGBufferFormat(this.m_DeferredLights.GBufferRenderingLayers);
				}
				else
				{
					gbufferSlice.graphicsFormat = RenderingLayerUtils.GetFormat(maskSize);
				}
				if (flag22)
				{
					this.m_DeferredLights.ReAllocateGBufferIfNeeded(gbufferSlice, this.m_DeferredLights.GBufferRenderingLayers);
				}
				else
				{
					RenderingUtils.ReAllocateHandleIfNeeded(ref ptr, gbufferSlice, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, name);
				}
				commandBuffer.SetGlobalTexture(ptr.name, ptr.nameID);
				RenderingLayerUtils.SetupProperties(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), maskSize);
				if (usesDeferredLighting)
				{
					commandBuffer.SetGlobalTexture("_CameraRenderingLayersTexture", ptr.nameID);
				}
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
			}
			if (flag15 && renderPassInputs.requiresNormalsTexture)
			{
				ref RTHandle ptr2 = ref this.m_NormalsTexture;
				string name2 = DepthNormalOnlyPass.k_CameraNormalsTextureName;
				if (usesDeferredLighting)
				{
					ptr2 = ref this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GBufferNormalSmoothnessIndex];
					name2 = ptr2.name;
				}
				RenderTextureDescriptor gbufferSlice2 = cameraTargetDescriptor;
				gbufferSlice2.depthStencilFormat = GraphicsFormat.None;
				gbufferSlice2.msaaSamples = (base.useDepthPriming ? cameraTargetDescriptor.msaaSamples : 1);
				if (usesDeferredLighting)
				{
					gbufferSlice2.graphicsFormat = this.m_DeferredLights.GetGBufferFormat(this.m_DeferredLights.GBufferNormalSmoothnessIndex);
				}
				else
				{
					gbufferSlice2.graphicsFormat = DepthNormalOnlyPass.GetGraphicsFormat();
				}
				if (usesDeferredLighting)
				{
					this.m_DeferredLights.ReAllocateGBufferIfNeeded(gbufferSlice2, this.m_DeferredLights.GBufferNormalSmoothnessIndex);
				}
				else
				{
					RenderingUtils.ReAllocateHandleIfNeeded(ref ptr2, gbufferSlice2, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, name2);
				}
				commandBuffer.SetGlobalTexture(ptr2.name, ptr2.nameID);
				if (usesDeferredLighting)
				{
					commandBuffer.SetGlobalTexture(DepthNormalOnlyPass.k_CameraNormalsTextureName, ptr2.nameID);
				}
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
			}
			if (flag15)
			{
				if (renderPassInputs.requiresNormalsTexture)
				{
					if (usesDeferredLighting)
					{
						int gbufferNormalSmoothnessIndex = this.m_DeferredLights.GBufferNormalSmoothnessIndex;
						if (this.m_DeferredLights.UseRenderingLayers)
						{
							this.m_DepthNormalPrepass.Setup(this.m_ActiveCameraDepthAttachment, this.m_DeferredLights.GbufferAttachments[gbufferNormalSmoothnessIndex], this.m_DeferredLights.GbufferAttachments[this.m_DeferredLights.GBufferRenderingLayers]);
						}
						else if (flag3)
						{
							this.m_DepthNormalPrepass.Setup(this.m_ActiveCameraDepthAttachment, this.m_DeferredLights.GbufferAttachments[gbufferNormalSmoothnessIndex], this.m_DecalLayersTexture);
						}
						else
						{
							this.m_DepthNormalPrepass.Setup(this.m_ActiveCameraDepthAttachment, this.m_DeferredLights.GbufferAttachments[gbufferNormalSmoothnessIndex]);
						}
						if (RenderPassEvent.AfterRenderingGbuffer <= renderPassInputs.requiresDepthNormalAtEvent && renderPassInputs.requiresDepthNormalAtEvent <= RenderPassEvent.BeforeRenderingOpaques)
						{
							this.m_DepthNormalPrepass.shaderTagIds = UniversalRenderer.k_DepthNormalsOnly;
						}
					}
					else if (flag3)
					{
						this.m_DepthNormalPrepass.Setup(this.m_DepthTexture, this.m_NormalsTexture, this.m_DecalLayersTexture);
					}
					else
					{
						this.m_DepthNormalPrepass.Setup(this.m_DepthTexture, this.m_NormalsTexture);
					}
					base.EnqueuePass(this.m_DepthNormalPrepass);
				}
				else if (!usesDeferredLighting)
				{
					this.m_DepthPrepass.Setup(cameraTargetDescriptor, this.m_DepthTexture);
					base.EnqueuePass(this.m_DepthPrepass);
				}
			}
			if (base.useDepthPriming)
			{
				this.m_PrimedDepthCopyPass.Setup(this.m_ActiveCameraDepthAttachment, this.m_DepthTexture);
				base.EnqueuePass(this.m_PrimedDepthCopyPass);
			}
			if (flag8)
			{
				RenderTextureDescriptor renderTextureDescriptor2;
				FilterMode filterMode;
				this.colorGradingLutPass.ConfigureDescriptor(universalPostProcessingData, out renderTextureDescriptor2, out filterMode);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PostProcessPasses.m_ColorGradingLut, renderTextureDescriptor2, filterMode, TextureWrapMode.Clamp, 0, 0f, "_InternalGradingLut");
				ColorGradingLutPass colorGradingLutPass = this.colorGradingLutPass;
				RTHandle colorGradingLut = this.colorGradingLut;
				colorGradingLutPass.Setup(colorGradingLut);
				base.EnqueuePass(this.colorGradingLutPass);
			}
			if (universalCameraData.xr.hasValidOcclusionMesh)
			{
				base.EnqueuePass(this.m_XROcclusionMeshPass);
			}
			bool resolveFinalTarget = universalCameraData.resolveFinalTarget;
			if (usesDeferredLighting)
			{
				if (this.m_DeferredLights.UseFramebufferFetch && (RenderPassEvent.AfterRenderingGbuffer == renderPassInputs.requiresDepthNormalAtEvent || !this.useRenderPassEnabled))
				{
					this.m_DeferredLights.DisableFramebufferFetchInput();
				}
				this.EnqueueDeferred(universalCameraData.cameraTargetDescriptor, flag15, renderPassInputs.requiresNormalsTexture, flag3, flag11, flag12);
			}
			else
			{
				RenderBufferStoreAction storeAction = RenderBufferStoreAction.Store;
				if (cameraTargetDescriptor.msaaSamples > 1)
				{
					storeAction = (flag19 ? RenderBufferStoreAction.StoreAndResolve : RenderBufferStoreAction.Store);
				}
				RenderBufferStoreAction renderBufferStoreAction = (flag19 || flag21 || !resolveFinalTarget) ? RenderBufferStoreAction.Store : RenderBufferStoreAction.DontCare;
				if (universalCameraData.xr.enabled && universalCameraData.xr.copyDepth)
				{
					renderBufferStoreAction = RenderBufferStoreAction.Store;
				}
				if (flag21 && cameraTargetDescriptor.msaaSamples > 1 && RenderingUtils.MultisampleDepthResolveSupported() && this.m_CopyDepthPass.renderPassEvent == RenderPassEvent.AfterRenderingTransparents && !flag19)
				{
					if (renderBufferStoreAction == RenderBufferStoreAction.Store)
					{
						renderBufferStoreAction = RenderBufferStoreAction.StoreAndResolve;
					}
					else if (renderBufferStoreAction == RenderBufferStoreAction.DontCare)
					{
						renderBufferStoreAction = RenderBufferStoreAction.Resolve;
					}
				}
				DrawObjectsPass drawObjectsPass;
				if (flag4)
				{
					drawObjectsPass = this.m_RenderOpaqueForwardWithRenderingLayersPass;
					this.m_RenderOpaqueForwardWithRenderingLayersPass.Setup(this.m_ActiveCameraColorAttachment, this.m_DecalLayersTexture, this.m_ActiveCameraDepthAttachment);
				}
				else
				{
					drawObjectsPass = this.m_RenderOpaqueForwardPass;
				}
				drawObjectsPass.ConfigureColorStoreAction(storeAction, 0U);
				drawObjectsPass.ConfigureDepthStoreAction(renderBufferStoreAction);
				ClearFlag clearFlag = (base.activeRenderPassQueue.Find((ScriptableRenderPass x) => x.renderPassEvent <= RenderPassEvent.BeforeRenderingOpaques && !x.overrideCameraTarget) != null || universalCameraData.renderType != CameraRenderType.Base || camera.clearFlags == CameraClearFlags.Nothing) ? ClearFlag.None : ClearFlag.Color;
				if (SystemInfo.usesLoadStoreActions)
				{
					drawObjectsPass.ConfigureClear(clearFlag, Color.black);
				}
				base.EnqueuePass(drawObjectsPass);
			}
			Skybox skybox;
			if (camera.clearFlags == CameraClearFlags.Skybox && universalCameraData.renderType != CameraRenderType.Overlay && (RenderSettings.skybox != null || (camera.TryGetComponent<Skybox>(out skybox) && skybox.material != null)))
			{
				base.EnqueuePass(this.m_DrawSkyboxPass);
			}
			if (flag21 && (!usesDeferredLighting || !this.useRenderPassEnabled || renderPassInputs.requiresDepthTexture))
			{
				this.m_CopyDepthPass.Setup(this.m_ActiveCameraDepthAttachment, this.m_DepthTexture);
				base.EnqueuePass(this.m_CopyDepthPass);
			}
			if (universalCameraData.renderType == CameraRenderType.Base && !flag15 && !flag21)
			{
				Shader.SetGlobalTexture("_CameraDepthTexture", SystemInfo.usesReversedZBuffer ? Texture2D.blackTexture : Texture2D.whiteTexture);
			}
			if (flag19)
			{
				Downsampling opaqueDownsampling = UniversalRenderPipeline.asset.opaqueDownsampling;
				RenderTextureDescriptor renderTextureDescriptor3 = cameraTargetDescriptor;
				FilterMode filterMode2;
				CopyColorPass.ConfigureDescriptor(opaqueDownsampling, ref renderTextureDescriptor3, out filterMode2);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_OpaqueColor, renderTextureDescriptor3, filterMode2, TextureWrapMode.Clamp, 1, 0f, "_CameraOpaqueTexture");
				this.m_CopyColorPass.Setup(this.m_ActiveCameraColorAttachment, this.m_OpaqueColor, opaqueDownsampling);
				base.EnqueuePass(this.m_CopyColorPass);
			}
			if (renderPassInputs.requiresMotionVectors)
			{
				RenderTextureDescriptor renderTextureDescriptor4 = cameraTargetDescriptor;
				renderTextureDescriptor4.graphicsFormat = GraphicsFormat.R16G16_SFloat;
				renderTextureDescriptor4.depthStencilFormat = GraphicsFormat.None;
				renderTextureDescriptor4.msaaSamples = 1;
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_MotionVectorColor, renderTextureDescriptor4, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_MotionVectorTexture");
				RenderTextureDescriptor renderTextureDescriptor5 = cameraTargetDescriptor;
				renderTextureDescriptor5.graphicsFormat = GraphicsFormat.None;
				renderTextureDescriptor5.depthStencilFormat = cameraTargetDescriptor.depthStencilFormat;
				renderTextureDescriptor5.msaaSamples = 1;
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_MotionVectorDepth, renderTextureDescriptor5, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_MotionVectorDepthTexture");
				MotionVectorRenderPass.SetMotionVectorGlobalMatrices(commandBuffer, universalCameraData);
				this.m_MotionVectorPass.Setup(this.m_MotionVectorColor, this.m_MotionVectorDepth);
				base.EnqueuePass(this.m_MotionVectorPass);
			}
			if (flag13)
			{
				base.EnqueuePass(this.m_TransparentSettingsPass);
			}
			RenderBufferStoreAction storeAction2 = (cameraTargetDescriptor.msaaSamples > 1 && resolveFinalTarget && !isPreviewCamera) ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.Store;
			RenderBufferStoreAction storeAction3 = resolveFinalTarget ? RenderBufferStoreAction.DontCare : RenderBufferStoreAction.Store;
			if (flag21 && this.m_CopyDepthPass.renderPassEvent >= RenderPassEvent.AfterRenderingTransparents)
			{
				storeAction3 = RenderBufferStoreAction.Store;
				if (cameraTargetDescriptor.msaaSamples > 1 && RenderingUtils.MultisampleDepthResolveSupported())
				{
					storeAction3 = RenderBufferStoreAction.Resolve;
				}
			}
			this.m_RenderTransparentForwardPass.ConfigureColorStoreAction(storeAction2, 0U);
			this.m_RenderTransparentForwardPass.ConfigureDepthStoreAction(storeAction3);
			base.EnqueuePass(this.m_RenderTransparentForwardPass);
			base.EnqueuePass(this.m_OnRenderObjectCallbackPass);
			this.SetupVFXCameraBuffer(universalCameraData);
			this.SetupRawColorDepthHistory(universalCameraData, ref cameraTargetDescriptor);
			bool rendersOverlayUI = universalCameraData.rendersOverlayUI;
			bool isHDROutputActive = universalCameraData.isHDROutputActive;
			if (rendersOverlayUI && isHDROutputActive)
			{
				this.m_DrawOffscreenUIPass.Setup(universalCameraData, this.cameraDepthTextureFormat);
				base.EnqueuePass(this.m_DrawOffscreenUIPass);
			}
			bool flag23 = universalCameraData.captureActions != null && resolveFinalTarget;
			bool flag24 = flag6 && resolveFinalTarget && (universalCameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing || (universalCameraData.imageScalingMode == ImageScalingMode.Upscaling && universalCameraData.upscalingFilter != ImageUpscalingFilter.Linear) || (universalCameraData.IsTemporalAAEnabled() && universalCameraData.taaSettings.contrastAdaptiveSharpening > 0f)) && (base.DebugHandler == null || (base.DebugHandler != null && base.DebugHandler.IsPostProcessingAllowed));
			bool flag25 = !flag23 && !flag20 && !flag24;
			bool flag26 = base.DebugHandler == null || !base.DebugHandler.HDRDebugViewIsActive(universalCameraData.resolveFinalTarget);
			if (flag5)
			{
				RenderTextureDescriptor compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(cameraTargetDescriptor, cameraTargetDescriptor.width, cameraTargetDescriptor.height, cameraTargetDescriptor.graphicsFormat, GraphicsFormat.None);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_PostProcessPasses.m_AfterPostProcessColor, compatibleDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_AfterPostProcessTexture");
			}
			if (resolveFinalTarget)
			{
				this.SetupFinalPassDebug(universalCameraData);
				if (flag5)
				{
					bool enableColorEncoding = flag25 && flag26;
					PostProcessPass postProcessPass = this.postProcessPass;
					bool resolveToScreen = flag25;
					RTHandle colorGradingLut = this.colorGradingLut;
					postProcessPass.Setup(cameraTargetDescriptor, this.m_ActiveCameraColorAttachment, resolveToScreen, this.m_ActiveCameraDepthAttachment, colorGradingLut, this.m_MotionVectorColor, flag24, enableColorEncoding);
					base.EnqueuePass(this.postProcessPass);
				}
				RTHandle activeCameraColorAttachment = this.m_ActiveCameraColorAttachment;
				if (flag24)
				{
					this.finalPostProcessPass.SetupFinalPass(activeCameraColorAttachment, true, flag26);
					base.EnqueuePass(this.finalPostProcessPass);
				}
				if (universalCameraData.captureActions != null)
				{
					base.EnqueuePass(this.m_CapturePass);
				}
				if (!flag24 && (!flag5 || flag20 || flag23) && !(this.m_ActiveCameraColorAttachment.nameID == this.m_TargetColorHandle.nameID))
				{
					this.m_FinalBlitPass.Setup(cameraTargetDescriptor, activeCameraColorAttachment);
					base.EnqueuePass(this.m_FinalBlitPass);
				}
				if (rendersOverlayUI && universalCameraData.isLastBaseCamera && !isHDROutputActive)
				{
					base.EnqueuePass(this.m_DrawOverlayUIPass);
				}
				if (universalCameraData.xr.enabled && !(this.m_ActiveCameraDepthAttachment.nameID == universalCameraData.xr.renderTarget) && universalCameraData.xr.copyDepth)
				{
					this.m_XRCopyDepthPass.Setup(this.m_ActiveCameraDepthAttachment, this.m_TargetDepthHandle);
					this.m_XRCopyDepthPass.CopyToDepthXR = true;
					base.EnqueuePass(this.m_XRCopyDepthPass);
					return;
				}
			}
			else if (flag5)
			{
				PostProcessPass postProcessPass2 = this.postProcessPass;
				bool resolveToScreen2 = false;
				RTHandle colorGradingLut = this.colorGradingLut;
				postProcessPass2.Setup(cameraTargetDescriptor, this.m_ActiveCameraColorAttachment, resolveToScreen2, this.m_ActiveCameraDepthAttachment, colorGradingLut, this.m_MotionVectorColor, false, false);
				base.EnqueuePass(this.postProcessPass);
			}
		}

		private void SetupVFXCameraBuffer(UniversalCameraData cameraData)
		{
			if (cameraData != null && cameraData.historyManager != null)
			{
				VFXCameraBufferTypes vfxcameraBufferTypes = VFXManager.IsCameraBufferNeeded(cameraData.camera);
				if (vfxcameraBufferTypes.HasFlag(VFXCameraBufferTypes.Color))
				{
					cameraData.historyManager.RequestAccess<RawColorHistory>();
					RawColorHistory historyForRead = cameraData.historyManager.GetHistoryForRead<RawColorHistory>();
					RTHandle handle = (historyForRead != null) ? historyForRead.GetCurrentTexture(0) : null;
					VFXManager.SetCameraBuffer(cameraData.camera, VFXCameraBufferTypes.Color, handle, 0, 0, (int)((float)cameraData.pixelWidth * cameraData.renderScale), (int)((float)cameraData.pixelHeight * cameraData.renderScale));
				}
				if (vfxcameraBufferTypes.HasFlag(VFXCameraBufferTypes.Depth))
				{
					cameraData.historyManager.RequestAccess<RawDepthHistory>();
					RawDepthHistory historyForRead2 = cameraData.historyManager.GetHistoryForRead<RawDepthHistory>();
					RTHandle handle2 = (historyForRead2 != null) ? historyForRead2.GetCurrentTexture(0) : null;
					VFXManager.SetCameraBuffer(cameraData.camera, VFXCameraBufferTypes.Depth, handle2, 0, 0, (int)((float)cameraData.pixelWidth * cameraData.renderScale), (int)((float)cameraData.pixelHeight * cameraData.renderScale));
				}
			}
		}

		private void SetupRawColorDepthHistory(UniversalCameraData cameraData, ref RenderTextureDescriptor cameraTargetDescriptor)
		{
			if (cameraData != null && cameraData.historyManager != null)
			{
				UniversalCameraHistory historyManager = cameraData.historyManager;
				bool xrMultipassEnabled = cameraData.xr.enabled && !cameraData.xr.singlePassEnabled;
				int multipassId = cameraData.xr.multipassId;
				if (historyManager.IsAccessRequested<RawColorHistory>())
				{
					RTHandle activeCameraColorAttachment = this.m_ActiveCameraColorAttachment;
					if (((activeCameraColorAttachment != null) ? activeCameraColorAttachment.rt : null) != null)
					{
						RawColorHistory historyForWrite = historyManager.GetHistoryForWrite<RawColorHistory>();
						if (historyForWrite != null)
						{
							historyForWrite.Update(ref cameraTargetDescriptor, xrMultipassEnabled);
							if (historyForWrite.GetCurrentTexture(multipassId) != null)
							{
								this.m_HistoryRawColorCopyPass.Setup(this.m_ActiveCameraColorAttachment, historyForWrite.GetCurrentTexture(multipassId), Downsampling.None);
								base.EnqueuePass(this.m_HistoryRawColorCopyPass);
							}
						}
					}
				}
				if (historyManager.IsAccessRequested<RawDepthHistory>())
				{
					RTHandle activeCameraDepthAttachment = this.m_ActiveCameraDepthAttachment;
					if (((activeCameraDepthAttachment != null) ? activeCameraDepthAttachment.rt : null) != null)
					{
						RawDepthHistory historyForWrite2 = historyManager.GetHistoryForWrite<RawDepthHistory>();
						if (historyForWrite2 != null)
						{
							if (!this.m_HistoryRawDepthCopyPass.CopyToDepth)
							{
								RenderTextureDescriptor renderTextureDescriptor = cameraTargetDescriptor;
								renderTextureDescriptor.colorFormat = RenderTextureFormat.RFloat;
								renderTextureDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
								renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
								historyForWrite2.Update(ref renderTextureDescriptor, xrMultipassEnabled);
							}
							else
							{
								RenderTextureDescriptor cameraTargetDescriptor2 = cameraData.cameraTargetDescriptor;
								cameraTargetDescriptor2.graphicsFormat = GraphicsFormat.None;
								historyForWrite2.Update(ref cameraTargetDescriptor2, xrMultipassEnabled);
							}
							if (historyForWrite2.GetCurrentTexture(multipassId) != null)
							{
								this.m_HistoryRawDepthCopyPass.Setup(this.m_ActiveCameraDepthAttachment, historyForWrite2.GetCurrentTexture(multipassId));
								base.EnqueuePass(this.m_HistoryRawDepthCopyPass);
							}
						}
					}
				}
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void SetupLights(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			UniversalRenderingData renderingData2 = base.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = base.frameData.Get<UniversalLightData>();
			this.m_ForwardLights.SetupLights(CommandBufferHelpers.GetUnsafeCommandBuffer(*renderingData.commandBuffer), renderingData2, universalCameraData, lightData);
			if (this.usesDeferredLighting)
			{
				this.m_DeferredLights.SetupLights(*renderingData.commandBuffer, universalCameraData, new Vector2Int(universalCameraData.cameraTargetDescriptor.width, universalCameraData.cameraTargetDescriptor.height), lightData, false);
			}
		}

		public unsafe override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
		{
			bool flag = UniversalRenderPipeline.asset.ShouldUseReflectionProbeAtlasBlending(this.renderingModeActual);
			if (this.usesClusterLightLoop && flag)
			{
				cullingParameters.cullingOptions |= CullingOptions.DisablePerObjectCulling;
			}
			bool flag2 = !UniversalRenderPipeline.asset.supportsMainLightShadows && !UniversalRenderPipeline.asset.supportsAdditionalLightShadows;
			bool flag3 = Mathf.Approximately(*cameraData.maxShadowDistance, 0f);
			if (flag2 || flag3)
			{
				cullingParameters.cullingOptions &= ~CullingOptions.ShadowCasters;
			}
			if (this.usesClusterLightLoop)
			{
				cullingParameters.maximumVisibleLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
				cullingParameters.reflectionProbeSortingCriteria = ReflectionProbeSortingCriteria.None;
			}
			else if (this.renderingModeActual == RenderingMode.Deferred)
			{
				cullingParameters.maximumVisibleLights = 65535;
			}
			else
			{
				cullingParameters.maximumVisibleLights = UniversalRenderPipeline.maxVisibleAdditionalLights + 1;
			}
			cullingParameters.shadowDistance = *cameraData.maxShadowDistance;
			cullingParameters.conservativeEnclosingSphere = UniversalRenderPipeline.asset.conservativeEnclosingSphere;
			cullingParameters.numIterationsEnclosingSphere = UniversalRenderPipeline.asset.numIterationsEnclosingSphere;
		}

		public override void FinishRendering(CommandBuffer cmd)
		{
			this.m_ColorBufferSystem.Clear();
			this.m_ActiveCameraColorAttachment = null;
			this.m_ActiveCameraDepthAttachment = null;
		}

		private void EnqueueDeferred(RenderTextureDescriptor cameraTargetDescriptor, bool hasDepthPrepass, bool hasNormalPrepass, bool hasRenderingLayerPrepass, bool applyMainShadow, bool applyAdditionalShadow)
		{
			this.m_DeferredLights.Setup(applyAdditionalShadow ? this.m_AdditionalLightsShadowCasterPass : null, hasDepthPrepass, hasNormalPrepass, hasRenderingLayerPrepass, this.m_DepthTexture, this.m_ActiveCameraDepthAttachment, this.m_ActiveCameraColorAttachment);
			if (this.useRenderPassEnabled && this.m_DeferredLights.UseFramebufferFetch)
			{
				this.m_GBufferPass.Configure(null, cameraTargetDescriptor);
				this.m_DeferredPass.Configure(null, cameraTargetDescriptor);
			}
			base.EnqueuePass(this.m_GBufferPass);
			if (!this.useRenderPassEnabled || !this.m_DeferredLights.UseFramebufferFetch)
			{
				this.m_GBufferCopyDepthPass.Setup(this.m_CameraDepthAttachment, this.m_DepthTexture);
				base.EnqueuePass(this.m_GBufferCopyDepthPass);
			}
			base.EnqueuePass(this.m_DeferredPass);
			base.EnqueuePass(this.m_RenderOpaqueForwardOnlyPass);
		}

		private UniversalRenderer.RenderPassInputSummary GetRenderPassInputs(bool isTemporalAAEnabled, bool postProcessingEnabled, bool isSceneViewCamera, bool renderingLayerProvidesByDepthNormalPass)
		{
			UniversalRenderer.RenderPassInputSummary renderPassInputSummary = default(UniversalRenderer.RenderPassInputSummary);
			renderPassInputSummary.requiresDepthNormalAtEvent = RenderPassEvent.BeforeRenderingOpaques;
			renderPassInputSummary.requiresDepthTextureEarliestEvent = RenderPassEvent.BeforeRenderingPostProcessing;
			for (int i = 0; i < base.activeRenderPassQueue.Count; i++)
			{
				ScriptableRenderPass scriptableRenderPass = base.activeRenderPassQueue[i];
				bool flag = (scriptableRenderPass.input & ScriptableRenderPassInput.Depth) > ScriptableRenderPassInput.None;
				bool flag2 = (scriptableRenderPass.input & ScriptableRenderPassInput.Normal) > ScriptableRenderPassInput.None;
				bool flag3 = (scriptableRenderPass.input & ScriptableRenderPassInput.Color) > ScriptableRenderPassInput.None;
				bool flag4 = (scriptableRenderPass.input & ScriptableRenderPassInput.Motion) > ScriptableRenderPassInput.None;
				bool flag5 = scriptableRenderPass.renderPassEvent < RenderPassEvent.AfterRenderingOpaques;
				renderPassInputSummary.requiresDepthTexture = (renderPassInputSummary.requiresDepthTexture || flag);
				renderPassInputSummary.requiresDepthPrepass |= (flag2 || (flag && flag5));
				renderPassInputSummary.requiresNormalsTexture = (renderPassInputSummary.requiresNormalsTexture || flag2);
				renderPassInputSummary.requiresColorTexture = (renderPassInputSummary.requiresColorTexture || flag3);
				renderPassInputSummary.requiresMotionVectors = (renderPassInputSummary.requiresMotionVectors || flag4);
				if (flag)
				{
					renderPassInputSummary.requiresDepthTextureEarliestEvent = (RenderPassEvent)Mathf.Min((int)scriptableRenderPass.renderPassEvent, (int)renderPassInputSummary.requiresDepthTextureEarliestEvent);
				}
				if (flag2 || flag)
				{
					renderPassInputSummary.requiresDepthNormalAtEvent = (RenderPassEvent)Mathf.Min((int)scriptableRenderPass.renderPassEvent, (int)renderPassInputSummary.requiresDepthNormalAtEvent);
				}
			}
			if (isTemporalAAEnabled)
			{
				renderPassInputSummary.requiresMotionVectors = true;
			}
			if (postProcessingEnabled)
			{
				MotionBlur component = VolumeManager.instance.stack.GetComponent<MotionBlur>();
				if (component != null && component.IsActive() && component.mode.value == MotionBlurMode.CameraAndObjects)
				{
					renderPassInputSummary.requiresMotionVectors = true;
				}
			}
			if (renderPassInputSummary.requiresMotionVectors)
			{
				renderPassInputSummary.requiresDepthTexture = true;
				renderPassInputSummary.requiresDepthTextureEarliestEvent = (RenderPassEvent)Mathf.Min((int)this.m_MotionVectorPass.renderPassEvent, (int)renderPassInputSummary.requiresDepthTextureEarliestEvent);
			}
			if (renderingLayerProvidesByDepthNormalPass)
			{
				renderPassInputSummary.requiresNormalsTexture = true;
			}
			return renderPassInputSummary;
		}

		private void CreateCameraRenderTarget(ScriptableRenderContext context, ref RenderTextureDescriptor descriptor, CommandBuffer cmd, UniversalCameraData cameraData)
		{
			using (new ProfilingScope(UniversalRenderer.Profiling.createCameraRenderTarget))
			{
				if (this.m_ColorBufferSystem.PeekBackBuffer() == null || this.m_ColorBufferSystem.PeekBackBuffer().nameID != BuiltinRenderTextureType.CameraTarget)
				{
					this.m_ActiveCameraColorAttachment = this.m_ColorBufferSystem.GetBackBuffer(cmd);
					base.ConfigureCameraColorTarget(this.m_ActiveCameraColorAttachment);
					cmd.SetGlobalTexture("_CameraColorTexture", this.m_ActiveCameraColorAttachment.nameID);
					cmd.SetGlobalTexture("_AfterPostProcessTexture", this.m_ActiveCameraColorAttachment.nameID);
				}
				if (this.m_CameraDepthAttachment == null || this.m_CameraDepthAttachment.nameID != BuiltinRenderTextureType.CameraTarget)
				{
					RenderTextureDescriptor renderTextureDescriptor = descriptor;
					renderTextureDescriptor.useMipMap = false;
					renderTextureDescriptor.autoGenerateMips = false;
					renderTextureDescriptor.bindMS = false;
					if (renderTextureDescriptor.msaaSamples > 1 && SystemInfo.supportsMultisampledTextures != 0)
					{
						if (this.IsDepthPrimingEnabledCompatibilityMode(cameraData))
						{
							renderTextureDescriptor.bindMS = true;
						}
						else
						{
							renderTextureDescriptor.bindMS = (!RenderingUtils.MultisampleDepthResolveSupported() || this.m_CopyDepthMode != CopyDepthMode.AfterTransparents);
						}
					}
					if (UniversalRenderer.IsGLESDevice())
					{
						renderTextureDescriptor.bindMS = false;
					}
					renderTextureDescriptor.graphicsFormat = GraphicsFormat.None;
					renderTextureDescriptor.depthStencilFormat = this.cameraDepthAttachmentFormat;
					RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_CameraDepthAttachment, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_CameraDepthAttachment");
					if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
					{
						RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_CameraDepthAttachment_D3d_11, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_CameraDepthAttachment_Temp");
						cmd.SetGlobalTexture(this.m_CameraDepthAttachment.name, this.m_CameraDepthAttachment_D3d_11.nameID);
					}
					else
					{
						cmd.SetGlobalTexture(this.m_CameraDepthAttachment.name, this.m_CameraDepthAttachment.nameID);
					}
					descriptor.depthStencilFormat = renderTextureDescriptor.depthStencilFormat;
				}
			}
			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}

		internal static bool PlatformRequiresExplicitMsaaResolve()
		{
			return (!SystemInfo.supportsMultisampleAutoResolve || !Application.isMobilePlatform) && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Metal;
		}

		private bool RequiresIntermediateColorTexture(UniversalCameraData cameraData, in UniversalRenderer.RenderPassInputSummary renderPassInputs)
		{
			if (cameraData.renderType == CameraRenderType.Base && !cameraData.resolveFinalTarget)
			{
				return true;
			}
			if (this.usesDeferredLighting)
			{
				return true;
			}
			bool isSceneViewCamera = cameraData.isSceneViewCamera;
			RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
			int msaaSamples = cameraTargetDescriptor.msaaSamples;
			bool flag = cameraData.imageScalingMode > ImageScalingMode.None;
			bool flag2 = this.IsScalableBufferManagerUsed(cameraData);
			bool flag3 = cameraTargetDescriptor.dimension == TextureDimension.Tex2D;
			bool flag4 = msaaSamples > 1 && UniversalRenderer.PlatformRequiresExplicitMsaaResolve();
			bool flag5 = cameraData.targetTexture != null && !isSceneViewCamera;
			bool flag6 = cameraData.captureActions != null;
			if (cameraData.xr.enabled)
			{
				flag = false;
				flag2 = false;
				flag3 = (cameraData.xr.renderTargetDesc.dimension == cameraTargetDescriptor.dimension);
			}
			bool flag7 = cameraData.requiresOpaqueTexture || renderPassInputs.requiresColorTexture;
			bool flag8 = (cameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated) || flag7 || flag4 || !cameraData.isDefaultViewport;
			if (flag5)
			{
				return flag8;
			}
			return flag8 || flag || flag2 || cameraData.isHdrEnabled || !flag3 || flag6 || cameraData.requireSrgbConversion;
		}

		private bool IsScalableBufferManagerUsed(UniversalCameraData cameraData)
		{
			bool allowDynamicResolution = cameraData.camera.allowDynamicResolution;
			bool flag = Mathf.Abs(ScalableBufferManager.widthScaleFactor - 1f) > 0.0001f;
			bool flag2 = Mathf.Abs(ScalableBufferManager.heightScaleFactor - 1f) > 0.0001f;
			return allowDynamicResolution && (flag || flag2);
		}

		private static bool CanCopyDepth(UniversalCameraData cameraData)
		{
			bool flag = cameraData.cameraTargetDescriptor.msaaSamples > 1;
			bool flag2 = SystemInfo.copyTextureSupport > CopyTextureSupport.None;
			bool flag3 = RenderingUtils.SupportsRenderTextureFormat(RenderTextureFormat.Depth);
			bool flag4 = !flag && (flag3 || flag2);
			bool flag5 = flag && SystemInfo.supportsMultisampledTextures != 0;
			return (!UniversalRenderer.IsGLESDevice() || !flag5) && (flag4 || flag5);
		}

		internal override void SwapColorBuffer(CommandBuffer cmd)
		{
			this.m_ColorBufferSystem.Swap();
			if (this.m_ActiveCameraDepthAttachment.nameID != BuiltinRenderTextureType.CameraTarget)
			{
				base.ConfigureCameraTarget(this.m_ColorBufferSystem.GetBackBuffer(cmd), this.m_ActiveCameraDepthAttachment);
			}
			else
			{
				base.ConfigureCameraColorTarget(this.m_ColorBufferSystem.GetBackBuffer(cmd));
			}
			this.m_ActiveCameraColorAttachment = this.m_ColorBufferSystem.GetBackBuffer(cmd);
			cmd.SetGlobalTexture("_CameraColorTexture", this.m_ActiveCameraColorAttachment.nameID);
			cmd.SetGlobalTexture("_AfterPostProcessTexture", this.m_ActiveCameraColorAttachment.nameID);
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

		internal override bool supportsNativeRenderPassRendergraphCompiler
		{
			get
			{
				return true;
			}
		}

		private bool DebugHandlerRequireDepthPass(UniversalCameraData cameraData)
		{
			DebugFullScreenMode debugFullScreenMode;
			return base.DebugHandler != null && base.DebugHandler.IsActiveForCamera(cameraData.isPreviewCamera) && base.DebugHandler.TryGetFullscreenDebugMode(out debugFullScreenMode);
		}

		private void CreateDebugTexture(RenderTextureDescriptor descriptor)
		{
			RenderTextureDescriptor renderTextureDescriptor = descriptor;
			renderTextureDescriptor.useMipMap = false;
			renderTextureDescriptor.autoGenerateMips = false;
			renderTextureDescriptor.bindMS = false;
			renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
			RenderingUtils.ReAllocateHandleIfNeeded(ref UniversalRenderer.m_RenderGraphDebugTextureHandle, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_RenderingDebuggerTexture");
		}

		private Rect CalculateUVRect(UniversalCameraData cameraData, float width, float height)
		{
			float num = width / (float)cameraData.pixelWidth;
			float num2 = height / (float)cameraData.pixelHeight;
			return new Rect(1f - num, 1f - num2, num, num2);
		}

		private Rect CalculateUVRect(UniversalCameraData cameraData, int textureHeightPercent)
		{
			float num = Mathf.Clamp01((float)textureHeightPercent / 100f);
			float width = num * (float)cameraData.pixelWidth;
			float height = num * (float)cameraData.pixelHeight;
			return this.CalculateUVRect(cameraData, width, height);
		}

		private void CorrectForTextureAspectRatio(ref float width, ref float height, float sourceWidth, float sourceHeight)
		{
			if (sourceWidth != 0f && sourceHeight != 0f)
			{
				float num = height * sourceWidth / sourceHeight;
				if (num > width)
				{
					height = width * sourceHeight / sourceWidth;
					return;
				}
				width = num;
			}
		}

		private void SetupRenderGraphFinalPassDebug(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			if (base.DebugHandler != null && base.DebugHandler.IsActiveForCamera(universalCameraData.isPreviewCamera))
			{
				DebugFullScreenMode debugFullScreenMode;
				int num;
				if (base.DebugHandler.TryGetFullscreenDebugMode(out debugFullScreenMode, out num) && (debugFullScreenMode != DebugFullScreenMode.ReflectionProbeAtlas || this.usesClusterLightLoop) && debugFullScreenMode != DebugFullScreenMode.STP)
				{
					float num2 = (float)universalCameraData.pixelWidth;
					float num3 = (float)universalCameraData.pixelHeight;
					float num4 = Mathf.Clamp01((float)num / 100f);
					float height = num4 * num3;
					float width = num4 * num2;
					bool supportsStereo = false;
					Vector4 zero = Vector4.zero;
					RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
					if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormatUsage.Linear | GraphicsFormatUsage.Render))
					{
						cameraTargetDescriptor.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
					}
					this.CreateDebugTexture(cameraTargetDescriptor);
					ImportResourceParams importParams = default(ImportResourceParams);
					importParams.clearOnFirstUse = false;
					importParams.discardOnLastUse = false;
					TextureHandle destination = renderGraph.ImportTexture(UniversalRenderer.m_RenderGraphDebugTextureHandle, importParams);
					switch (debugFullScreenMode)
					{
					case DebugFullScreenMode.Depth:
						this.BlitToDebugTexture(renderGraph, universalResourceData.cameraDepthTexture, destination, false);
						supportsStereo = true;
						break;
					case DebugFullScreenMode.MotionVector:
						this.BlitToDebugTexture(renderGraph, universalResourceData.motionVectorColor, destination, true);
						supportsStereo = true;
						zero.x = -0.01f;
						zero.y = 0.01f;
						zero.z = 0f;
						zero.w = 1f;
						break;
					case DebugFullScreenMode.AdditionalLightsShadowMap:
						this.BlitToDebugTexture(renderGraph, universalResourceData.additionalShadowsTexture, destination, false);
						break;
					case DebugFullScreenMode.MainLightShadowMap:
						this.BlitToDebugTexture(renderGraph, universalResourceData.mainShadowsTexture, destination, false);
						break;
					case DebugFullScreenMode.AdditionalLightsCookieAtlas:
					{
						LightCookieManager lightCookieManager = this.m_LightCookieManager;
						TextureHandle source = (lightCookieManager != null && lightCookieManager.AdditionalLightsCookieAtlasTexture != null) ? renderGraph.ImportTexture(this.m_LightCookieManager.AdditionalLightsCookieAtlasTexture) : TextureHandle.nullHandle;
						this.BlitToDebugTexture(renderGraph, source, destination, false);
						break;
					}
					case DebugFullScreenMode.ReflectionProbeAtlas:
					{
						TextureHandle source2 = (this.m_ForwardLights.reflectionProbeManager.atlasRT != null) ? renderGraph.ImportTexture(RTHandles.Alloc(this.m_ForwardLights.reflectionProbeManager.atlasRT, true)) : TextureHandle.nullHandle;
						this.BlitToDebugTexture(renderGraph, source2, destination, false);
						break;
					}
					}
					RenderTexture renderTexture = null;
					switch (debugFullScreenMode)
					{
					case DebugFullScreenMode.AdditionalLightsShadowMap:
					{
						AdditionalLightsShadowCasterPass additionalLightsShadowCasterPass = this.m_AdditionalLightsShadowCasterPass;
						RenderTexture renderTexture2;
						if (additionalLightsShadowCasterPass == null)
						{
							renderTexture2 = null;
						}
						else
						{
							RTHandle additionalLightsShadowmapHandle = additionalLightsShadowCasterPass.m_AdditionalLightsShadowmapHandle;
							renderTexture2 = ((additionalLightsShadowmapHandle != null) ? additionalLightsShadowmapHandle.rt : null);
						}
						renderTexture = renderTexture2;
						break;
					}
					case DebugFullScreenMode.MainLightShadowMap:
					{
						MainLightShadowCasterPass mainLightShadowCasterPass = this.m_MainLightShadowCasterPass;
						RenderTexture renderTexture3;
						if (mainLightShadowCasterPass == null)
						{
							renderTexture3 = null;
						}
						else
						{
							RTHandle mainLightShadowmapTexture = mainLightShadowCasterPass.m_MainLightShadowmapTexture;
							renderTexture3 = ((mainLightShadowmapTexture != null) ? mainLightShadowmapTexture.rt : null);
						}
						renderTexture = renderTexture3;
						break;
					}
					case DebugFullScreenMode.AdditionalLightsCookieAtlas:
					{
						LightCookieManager lightCookieManager2 = this.m_LightCookieManager;
						RenderTexture renderTexture4;
						if (lightCookieManager2 == null)
						{
							renderTexture4 = null;
						}
						else
						{
							RTHandle additionalLightsCookieAtlasTexture = lightCookieManager2.AdditionalLightsCookieAtlasTexture;
							renderTexture4 = ((additionalLightsCookieAtlasTexture != null) ? additionalLightsCookieAtlasTexture.rt : null);
						}
						renderTexture = renderTexture4;
						break;
					}
					case DebugFullScreenMode.ReflectionProbeAtlas:
					{
						ForwardLights forwardLights = this.m_ForwardLights;
						renderTexture = ((forwardLights != null) ? forwardLights.reflectionProbeManager.atlasRT : null);
						break;
					}
					}
					if (renderTexture != null)
					{
						this.CorrectForTextureAspectRatio(ref width, ref height, (float)renderTexture.width, (float)renderTexture.height);
					}
					Rect displayRect = this.CalculateUVRect(universalCameraData, width, height);
					base.DebugHandler.SetDebugRenderTarget(UniversalRenderer.m_RenderGraphDebugTextureHandle, displayRect, supportsStereo, zero);
				}
				else
				{
					base.DebugHandler.ResetDebugRenderTarget();
				}
			}
			DebugFullScreenMode debugFullScreenMode2;
			int num5;
			if (base.DebugHandler != null && !base.DebugHandler.TryGetFullscreenDebugMode(out debugFullScreenMode2, out num5))
			{
				DebugDisplayGPUResidentDrawer gpuResidentDrawerSettings = base.DebugHandler.DebugDisplaySettings.gpuResidentDrawerSettings;
				GPUResidentDrawer.RenderDebugOcclusionTestOverlay(renderGraph, gpuResidentDrawerSettings, universalCameraData.camera.GetInstanceID(), universalResourceData.activeColorTexture);
				float num6 = (float)((int)((float)universalCameraData.pixelHeight * universalCameraData.renderScale));
				float num7 = (float)((int)((float)universalCameraData.pixelHeight * universalCameraData.renderScale));
				float num8 = num7 * (float)num5 / 100f;
				GPUResidentDrawer.RenderDebugOccluderOverlay(renderGraph, gpuResidentDrawerSettings, new Vector2(0.25f * num6, num7 - 1.5f * num8), num8, universalResourceData.activeColorTexture);
			}
		}

		private void SetupAfterPostRenderGraphFinalPassDebug(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			DebugFullScreenMode debugFullScreenMode;
			int textureHeightPercent;
			if (base.DebugHandler != null && base.DebugHandler.IsActiveForCamera(universalCameraData.isPreviewCamera) && base.DebugHandler.TryGetFullscreenDebugMode(out debugFullScreenMode, out textureHeightPercent) && debugFullScreenMode == DebugFullScreenMode.STP)
			{
				this.CreateDebugTexture(universalCameraData.cameraTargetDescriptor);
				ImportResourceParams importParams = default(ImportResourceParams);
				importParams.clearOnFirstUse = false;
				importParams.discardOnLastUse = false;
				TextureHandle destination = renderGraph.ImportTexture(UniversalRenderer.m_RenderGraphDebugTextureHandle, importParams);
				this.BlitToDebugTexture(renderGraph, universalResourceData.stpDebugView, destination, false);
				Rect displayRect = this.CalculateUVRect(universalCameraData, textureHeightPercent);
				Vector4 zero = Vector4.zero;
				base.DebugHandler.SetDebugRenderTarget(UniversalRenderer.m_RenderGraphDebugTextureHandle, displayRect, true, zero);
			}
		}

		private void BlitToDebugTexture(RenderGraph renderGraph, TextureHandle source, TextureHandle destination, bool isSourceTextureColor = false)
		{
			if (!source.IsValid())
			{
				this.BlitEmptyTexture(renderGraph, destination, "Copy To Debug Texture");
				return;
			}
			if (isSourceTextureColor)
			{
				renderGraph.AddCopyPass(source, destination, "Copy Pass Utility", ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\UniversalRendererDebug.cs", 251);
				return;
			}
			RenderGraphUtils.BlitMaterialParameters blitParameters = new RenderGraphUtils.BlitMaterialParameters(source, destination, this.m_DebugBlitMaterial, 0);
			renderGraph.AddBlitPass(blitParameters, "Blit Pass Utility w. Material", ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\UniversalRendererDebug.cs", 260);
		}

		private void BlitEmptyTexture(RenderGraph renderGraph, TextureHandle destination, string passName = "Copy To Debug Texture")
		{
			UniversalRenderer.CopyToDebugTexturePassData copyToDebugTexturePassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<UniversalRenderer.CopyToDebugTexturePassData>(passName, out copyToDebugTexturePassData, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\UniversalRendererDebug.cs", 271))
			{
				copyToDebugTexturePassData.src = renderGraph.defaultResources.blackTexture;
				copyToDebugTexturePassData.dest = destination;
				rasterRenderGraphBuilder.SetRenderAttachment(destination, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.AllowPassCulling(false);
				rasterRenderGraphBuilder.SetRenderFunc<UniversalRenderer.CopyToDebugTexturePassData>(delegate(UniversalRenderer.CopyToDebugTexturePassData data, RasterGraphContext context)
				{
					Blitter.BlitTexture(context.cmd, data.src, new Vector4(1f, 1f, 0f, 0f), 0f, false);
				});
			}
		}

		private RTHandle currentRenderGraphCameraColorHandle
		{
			get
			{
				if (UniversalRenderer.m_CurrentColorHandle < 0)
				{
					return null;
				}
				return UniversalRenderer.m_RenderGraphCameraColorHandles[UniversalRenderer.m_CurrentColorHandle];
			}
		}

		private RTHandle nextRenderGraphCameraColorHandle
		{
			get
			{
				if (UniversalRenderer.m_CurrentColorHandle < 0)
				{
					return null;
				}
				UniversalRenderer.m_CurrentColorHandle = (UniversalRenderer.m_CurrentColorHandle + 1) % 2;
				return this.currentRenderGraphCameraColorHandle;
			}
		}

		private void CleanupRenderGraphResources()
		{
			RTHandle rthandle = UniversalRenderer.m_RenderGraphCameraColorHandles[0];
			if (rthandle != null)
			{
				rthandle.Release();
			}
			RTHandle rthandle2 = UniversalRenderer.m_RenderGraphCameraColorHandles[1];
			if (rthandle2 != null)
			{
				rthandle2.Release();
			}
			RTHandle renderGraphCameraDepthHandle = UniversalRenderer.m_RenderGraphCameraDepthHandle;
			if (renderGraphCameraDepthHandle != null)
			{
				renderGraphCameraDepthHandle.Release();
			}
			RTHandle renderGraphDebugTextureHandle = UniversalRenderer.m_RenderGraphDebugTextureHandle;
			if (renderGraphDebugTextureHandle == null)
			{
				return;
			}
			renderGraphDebugTextureHandle.Release();
		}

		public static TextureHandle CreateRenderGraphTexture(RenderGraph renderGraph, RenderTextureDescriptor desc, string name, bool clear, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
		{
			TextureDesc textureDesc = new TextureDesc(desc.width, desc.height, false, false);
			textureDesc.dimension = desc.dimension;
			textureDesc.clearBuffer = clear;
			textureDesc.bindTextureMS = desc.bindMS;
			textureDesc.format = ((desc.depthStencilFormat != GraphicsFormat.None) ? desc.depthStencilFormat : desc.graphicsFormat);
			textureDesc.slices = desc.volumeDepth;
			textureDesc.msaaSamples = (MSAASamples)desc.msaaSamples;
			textureDesc.name = name;
			textureDesc.enableRandomWrite = desc.enableRandomWrite;
			textureDesc.filterMode = filterMode;
			textureDesc.wrapMode = wrapMode;
			textureDesc.isShadowMap = (desc.shadowSamplingMode != ShadowSamplingMode.None && desc.depthStencilFormat > GraphicsFormat.None);
			textureDesc.vrUsage = desc.vrUsage;
			textureDesc.enableShadingRate = desc.enableShadingRate;
			textureDesc.useDynamicScale = desc.useDynamicScale;
			textureDesc.useDynamicScaleExplicit = desc.useDynamicScaleExplicit;
			return renderGraph.CreateTexture(textureDesc);
		}

		internal static TextureHandle CreateRenderGraphTexture(RenderGraph renderGraph, RenderTextureDescriptor desc, string name, bool clear, Color color, FilterMode filterMode = FilterMode.Point, TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool discardOnLastUse = false)
		{
			TextureDesc textureDesc = new TextureDesc(desc.width, desc.height, false, false);
			textureDesc.dimension = desc.dimension;
			textureDesc.clearBuffer = clear;
			textureDesc.clearColor = color;
			textureDesc.bindTextureMS = desc.bindMS;
			textureDesc.format = ((desc.depthStencilFormat != GraphicsFormat.None) ? desc.depthStencilFormat : desc.graphicsFormat);
			textureDesc.slices = desc.volumeDepth;
			textureDesc.msaaSamples = (MSAASamples)desc.msaaSamples;
			textureDesc.name = name;
			textureDesc.enableRandomWrite = desc.enableRandomWrite;
			textureDesc.filterMode = filterMode;
			textureDesc.wrapMode = wrapMode;
			textureDesc.enableShadingRate = desc.enableShadingRate;
			textureDesc.useDynamicScale = desc.useDynamicScale;
			textureDesc.useDynamicScaleExplicit = desc.useDynamicScaleExplicit;
			textureDesc.discardBuffer = discardOnLastUse;
			textureDesc.vrUsage = desc.vrUsage;
			return renderGraph.CreateTexture(textureDesc);
		}

		private bool RequiresIntermediateAttachments(UniversalCameraData cameraData, in UniversalRenderer.RenderPassInputSummary renderPassInputs, bool requireCopyFromDepth)
		{
			return ((this.HasActiveRenderFeatures() && this.m_IntermediateTextureMode == IntermediateTextureMode.Always) | this.HasPassesRequiringIntermediateTexture() | (Application.isEditor && this.usesClusterLightLoop) | this.RequiresIntermediateColorTexture(cameraData, renderPassInputs)) || requireCopyFromDepth;
		}

		private void UpdateCameraHistory(UniversalCameraData cameraData)
		{
			if (cameraData != null && cameraData.historyManager != null)
			{
				bool flag = cameraData.xr.enabled && !cameraData.xr.singlePassEnabled;
				int multipassId = cameraData.xr.multipassId;
				if (!flag || multipassId == 0)
				{
					UniversalCameraHistory historyManager = cameraData.historyManager;
					historyManager.GatherHistoryRequests();
					historyManager.ReleaseUnusedHistory();
					historyManager.SwapAndSetReferenceSize(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
				}
			}
		}

		private void CreateRenderGraphCameraRenderTargets(RenderGraph renderGraph, bool isCameraTargetOffscreenDepth, in UniversalRenderer.RenderPassInputSummary renderPassInputs, bool requireDepthTexture, bool requireDepthPrepass)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			base.frameData.Get<UniversalPostProcessingData>();
			UniversalRenderer.ClearCameraParams clearCameraParams = this.GetClearCameraParams(universalCameraData);
			this.SetupTargetHandles(universalCameraData);
			this.UpdateCameraHistory(universalCameraData);
			bool flag = requireDepthPrepass && !base.useDepthPriming;
			bool flag2 = flag;
			bool requireCopyFromDepth = requireDepthTexture && !flag;
			if (universalCameraData.renderType == CameraRenderType.Base)
			{
				UniversalRenderer.m_RequiresIntermediateAttachments = this.RequiresIntermediateAttachments(universalCameraData, renderPassInputs, requireCopyFromDepth);
			}
			this.ImportBackBuffers(renderGraph, universalCameraData, clearCameraParams.clearValue, isCameraTargetOffscreenDepth);
			if (UniversalRenderer.m_RequiresIntermediateAttachments && !isCameraTargetOffscreenDepth)
			{
				this.CreateIntermediateCameraColorAttachment(renderGraph, universalCameraData, clearCameraParams.mustClearColor, clearCameraParams.clearValue);
			}
			else
			{
				universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
			}
			if (UniversalRenderer.m_RequiresIntermediateAttachments)
			{
				this.CreateIntermediateCameraDepthAttachment(renderGraph, universalCameraData, clearCameraParams.mustClearDepth, clearCameraParams.clearValue, flag2);
			}
			else
			{
				universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
			}
			this.CreateCameraDepthCopyTexture(renderGraph, universalCameraData.cameraTargetDescriptor, flag2);
			this.CreateCameraNormalsTexture(renderGraph, universalCameraData.cameraTargetDescriptor);
			this.CreateMotionVectorTextures(renderGraph, universalCameraData.cameraTargetDescriptor);
			this.CreateRenderingLayersTexture(renderGraph, universalCameraData.cameraTargetDescriptor);
			if (!isCameraTargetOffscreenDepth)
			{
				this.CreateAfterPostProcessTexture(renderGraph, universalCameraData.cameraTargetDescriptor);
			}
		}

		private UniversalRenderer.ClearCameraParams GetClearCameraParams(UniversalCameraData cameraData)
		{
			bool clearColor = cameraData.renderType == CameraRenderType.Base;
			bool clearDepth = cameraData.renderType == CameraRenderType.Base || cameraData.clearDepth;
			Color clearVal = (cameraData.camera.clearFlags == CameraClearFlags.Nothing && cameraData.targetTexture == null) ? Color.yellow : cameraData.backgroundColor;
			if (base.IsSceneFilteringEnabled(cameraData.camera))
			{
				clearVal.a = 0f;
				clearDepth = false;
			}
			DebugHandler debugHandler = cameraData.renderer.DebugHandler;
			if (debugHandler != null && debugHandler.IsActiveForCamera(cameraData.isPreviewCamera) && debugHandler.IsScreenClearNeeded)
			{
				clearColor = true;
				clearDepth = true;
				if (base.DebugHandler != null && base.DebugHandler.IsActiveForCamera(cameraData.isPreviewCamera))
				{
					base.DebugHandler.TryGetScreenClearColor(ref clearVal);
				}
			}
			return new UniversalRenderer.ClearCameraParams(clearColor, clearDepth, clearVal);
		}

		private void SetupTargetHandles(UniversalCameraData cameraData)
		{
			RenderTargetIdentifier renderTargetIdentifier = (cameraData.targetTexture != null) ? new RenderTargetIdentifier(cameraData.targetTexture) : BuiltinRenderTextureType.CameraTarget;
			RenderTargetIdentifier renderTargetIdentifier2 = (cameraData.targetTexture != null) ? new RenderTargetIdentifier(cameraData.targetTexture) : BuiltinRenderTextureType.Depth;
			if (cameraData.xr.enabled)
			{
				renderTargetIdentifier = cameraData.xr.renderTarget;
				renderTargetIdentifier2 = cameraData.xr.renderTarget;
			}
			if (this.m_TargetColorHandle == null)
			{
				this.m_TargetColorHandle = RTHandles.Alloc(renderTargetIdentifier, "Backbuffer color");
			}
			else if (this.m_TargetColorHandle.nameID != renderTargetIdentifier)
			{
				RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_TargetColorHandle, renderTargetIdentifier);
			}
			if (this.m_TargetDepthHandle == null)
			{
				this.m_TargetDepthHandle = RTHandles.Alloc(renderTargetIdentifier2, "Backbuffer depth");
				return;
			}
			if (this.m_TargetDepthHandle.nameID != renderTargetIdentifier2)
			{
				RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_TargetDepthHandle, renderTargetIdentifier2);
			}
		}

		private void SetupRenderingLayers(int msaaSamples)
		{
			this.m_RequiresRenderingLayer = RenderingLayerUtils.RequireRenderingLayers(this, base.rendererFeatures, msaaSamples, out this.m_RenderingLayersEvent, out this.m_RenderingLayersMaskSize);
			this.m_RenderingLayerProvidesRenderObjectPass = (this.m_RequiresRenderingLayer && this.m_RenderingLayersEvent == RenderingLayerUtils.Event.Opaque);
			this.m_RenderingLayerProvidesByDepthNormalPass = (this.m_RequiresRenderingLayer && this.m_RenderingLayersEvent == RenderingLayerUtils.Event.DepthNormalPrePass);
			if (this.m_DeferredLights != null)
			{
				this.m_DeferredLights.RenderingLayerMaskSize = this.m_RenderingLayersMaskSize;
				this.m_DeferredLights.UseDecalLayers = this.m_RequiresRenderingLayer;
			}
		}

		internal void SetupRenderGraphLights(RenderGraph renderGraph, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			this.m_ForwardLights.SetupRenderGraphLights(renderGraph, renderingData, cameraData, lightData);
			if (this.usesDeferredLighting)
			{
				this.m_DeferredLights.UseFramebufferFetch = renderGraph.nativeRenderPassesEnabled;
				this.m_DeferredLights.SetupRenderGraphLights(renderGraph, cameraData, lightData);
			}
		}

		private void RenderRawColorDepthHistory(RenderGraph renderGraph, UniversalCameraData cameraData, UniversalResourceData resourceData)
		{
			if (cameraData != null && cameraData.historyManager != null && resourceData != null)
			{
				UniversalCameraHistory historyManager = cameraData.historyManager;
				bool xrMultipassEnabled = cameraData.xr.enabled && !cameraData.xr.singlePassEnabled;
				int multipassId = cameraData.xr.multipassId;
				if (historyManager.IsAccessRequested<RawColorHistory>())
				{
					TextureHandle textureHandle = resourceData.cameraColor;
					if (textureHandle.IsValid())
					{
						RawColorHistory historyForWrite = historyManager.GetHistoryForWrite<RawColorHistory>();
						if (historyForWrite != null)
						{
							historyForWrite.Update(ref cameraData.cameraTargetDescriptor, xrMultipassEnabled);
							if (historyForWrite.GetCurrentTexture(multipassId) != null)
							{
								TextureHandle textureHandle2 = renderGraph.ImportTexture(historyForWrite.GetCurrentTexture(multipassId));
								CopyColorPass historyRawColorCopyPass = this.m_HistoryRawColorCopyPass;
								ContextContainer frameData = base.frameData;
								textureHandle = resourceData.cameraColor;
								historyRawColorCopyPass.RenderToExistingTexture(renderGraph, frameData, textureHandle2, textureHandle, Downsampling.None);
							}
						}
					}
				}
				if (historyManager.IsAccessRequested<RawDepthHistory>())
				{
					TextureHandle textureHandle = resourceData.cameraDepth;
					if (textureHandle.IsValid())
					{
						RawDepthHistory historyForWrite2 = historyManager.GetHistoryForWrite<RawDepthHistory>();
						if (historyForWrite2 != null)
						{
							if (!this.m_HistoryRawDepthCopyPass.CopyToDepth)
							{
								RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
								cameraTargetDescriptor.graphicsFormat = GraphicsFormat.R32_SFloat;
								cameraTargetDescriptor.depthStencilFormat = GraphicsFormat.None;
								historyForWrite2.Update(ref cameraTargetDescriptor, xrMultipassEnabled);
							}
							else
							{
								RenderTextureDescriptor cameraTargetDescriptor2 = cameraData.cameraTargetDescriptor;
								cameraTargetDescriptor2.graphicsFormat = GraphicsFormat.None;
								historyForWrite2.Update(ref cameraTargetDescriptor2, xrMultipassEnabled);
							}
							if (historyForWrite2.GetCurrentTexture(multipassId) != null)
							{
								TextureHandle destination = renderGraph.ImportTexture(historyForWrite2.GetCurrentTexture(multipassId));
								this.m_HistoryRawDepthCopyPass.Render(renderGraph, base.frameData, destination, resourceData.cameraDepth, false, "Copy Depth");
							}
						}
					}
				}
			}
		}

		public override void OnBeginRenderGraphFrame()
		{
			base.frameData.Get<UniversalResourceData>().InitFrame();
		}

		internal override void OnRecordRenderGraph(RenderGraph renderGraph, ScriptableRenderContext context)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			UniversalRenderingData renderingData = base.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = base.frameData.Get<UniversalLightData>();
			UniversalPostProcessingData universalPostProcessingData = base.frameData.Get<UniversalPostProcessingData>();
			this.useRenderPassEnabled = renderGraph.nativeRenderPassesEnabled;
			MotionVectorRenderPass.SetRenderGraphMotionVectorGlobalMatrices(renderGraph, universalCameraData);
			this.SetupRenderGraphLights(renderGraph, renderingData, universalCameraData, lightData);
			this.SetupRenderingLayers(universalCameraData.cameraTargetDescriptor.msaaSamples);
			bool flag = universalCameraData.camera.targetTexture != null && universalCameraData.camera.targetTexture.format == RenderTextureFormat.Depth;
			UniversalRenderer.RenderPassInputSummary renderPassInputs = this.GetRenderPassInputs(universalCameraData.IsTemporalAAEnabled(), universalPostProcessingData.isEnabled, universalCameraData.isSceneViewCamera, this.m_RenderingLayerProvidesByDepthNormalPass);
			bool applyPostProcessing = universalCameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated;
			bool requireDepthTexture = UniversalRenderer.RequireDepthTexture(universalCameraData, renderPassInputs, applyPostProcessing);
			bool flag2 = this.RequirePrepassForTextures(universalCameraData, renderPassInputs, requireDepthTexture);
			base.useDepthPriming = UniversalRenderer.IsDepthPrimingEnabledRenderGraph(universalCameraData, renderPassInputs, this.m_DepthPrimingMode, requireDepthTexture, flag2, this.usesDeferredLighting);
			bool flag3 = flag2 || base.useDepthPriming;
			this.CreateRenderGraphCameraRenderTargets(renderGraph, flag, renderPassInputs, requireDepthTexture, flag3);
			DebugHandler debugHandler = base.DebugHandler;
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRendering);
			base.SetupRenderGraphCameraProperties(renderGraph, universalResourceData.isActiveTargetBackBuffer);
			base.ProcessVFXCameraCommand(renderGraph);
			universalCameraData.renderer.useDepthPriming = base.useDepthPriming;
			if (flag)
			{
				this.OnOffscreenDepthTextureRendering(renderGraph, context, universalResourceData, universalCameraData);
				return;
			}
			this.OnBeforeRendering(renderGraph);
			base.BeginRenderGraphXRRendering(renderGraph);
			this.OnMainRendering(renderGraph, context, renderPassInputs, flag3, requireDepthTexture);
			this.OnAfterRendering(renderGraph, applyPostProcessing);
			base.EndRenderGraphXRRendering(renderGraph);
		}

		public override void OnEndRenderGraphFrame()
		{
			base.frameData.Get<UniversalResourceData>().EndFrame();
		}

		internal override void OnFinishRenderGraphRendering(CommandBuffer cmd)
		{
			if (this.usesDeferredLighting)
			{
				this.m_DeferredPass.OnCameraCleanup(cmd);
			}
			this.m_CopyDepthPass.OnCameraCleanup(cmd);
			this.m_DepthNormalPrepass.OnCameraCleanup(cmd);
		}

		public override bool supportsGPUOcclusion
		{
			get
			{
				bool flag = SystemInfo.graphicsDeviceVendorID != 20803;
				if (!flag && !this.m_IssuedGPUOcclusionUnsupportedMsg)
				{
					Debug.LogWarning("The GPU Occlusion Culling feature is currently unavailable on this device due to suspected driver issues.");
					this.m_IssuedGPUOcclusionUnsupportedMsg = true;
				}
				return flag;
			}
		}

		private void OnOffscreenDepthTextureRendering(RenderGraph renderGraph, ScriptableRenderContext context, UniversalResourceData resourceData, UniversalCameraData cameraData)
		{
			if (!renderGraph.nativeRenderPassesEnabled)
			{
				ClearTargetsPass.Render(renderGraph, resourceData.activeColorTexture, resourceData.backBufferDepth, RTClearFlags.Depth, cameraData.backgroundColor);
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingShadows, RenderPassEvent.BeforeRenderingOpaques);
			this.m_RenderOpaqueForwardPass.Render(renderGraph, base.frameData, TextureHandle.nullHandle, resourceData.backBufferDepth, TextureHandle.nullHandle, TextureHandle.nullHandle, uint.MaxValue);
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingOpaques, RenderPassEvent.BeforeRenderingTransparents);
			this.m_RenderTransparentForwardPass.Render(renderGraph, base.frameData, TextureHandle.nullHandle, resourceData.backBufferDepth, TextureHandle.nullHandle, TextureHandle.nullHandle, uint.MaxValue);
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingTransparents, RenderPassEvent.AfterRendering);
		}

		private void OnBeforeRendering(RenderGraph renderGraph)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			UniversalRenderingData renderingData = base.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = base.frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = base.frameData.Get<UniversalShadowData>();
			this.m_ForwardLights.PreSetup(renderingData, universalCameraData, lightData);
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingShadows);
			bool flag = false;
			if (this.m_MainLightShadowCasterPass.Setup(renderingData, universalCameraData, lightData, shadowData))
			{
				flag = true;
				universalResourceData.mainShadowsTexture = this.m_MainLightShadowCasterPass.Render(renderGraph, base.frameData);
			}
			if (this.m_AdditionalLightsShadowCasterPass.Setup(renderingData, universalCameraData, lightData, shadowData))
			{
				flag = true;
				universalResourceData.additionalShadowsTexture = this.m_AdditionalLightsShadowCasterPass.Render(renderGraph, base.frameData);
			}
			if (flag)
			{
				base.SetupRenderGraphCameraProperties(renderGraph, universalResourceData.isActiveTargetBackBuffer);
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingShadows);
			if (universalCameraData.postProcessEnabled && this.m_PostProcessPasses.isCreated)
			{
				TextureHandle internalColorLut;
				this.m_PostProcessPasses.colorGradingLutPass.Render(renderGraph, base.frameData, out internalColorLut);
				universalResourceData.internalColorLut = internalColorLut;
			}
		}

		private unsafe void UpdateInstanceOccluders(RenderGraph renderGraph, UniversalCameraData cameraData, TextureHandle depthTexture)
		{
			int x = (int)((float)cameraData.pixelWidth * cameraData.renderScale);
			int y = (int)((float)cameraData.pixelHeight * cameraData.renderScale);
			bool flag = cameraData.xr.enabled && cameraData.xr.singlePassEnabled;
			OccluderParameters occluderParameters = new OccluderParameters(cameraData.camera.GetInstanceID())
			{
				subviewCount = (flag ? 2 : 1),
				depthTexture = depthTexture,
				depthSize = new Vector2Int(x, y),
				depthIsArray = flag
			};
			int subviewCount = occluderParameters.subviewCount;
			Span<OccluderSubviewUpdate> span = new Span<OccluderSubviewUpdate>(stackalloc byte[checked(unchecked((UIntPtr)subviewCount) * (UIntPtr)sizeof(OccluderSubviewUpdate))], subviewCount);
			for (int i = 0; i < occluderParameters.subviewCount; i++)
			{
				Matrix4x4 viewMatrix = cameraData.GetViewMatrix(i);
				Matrix4x4 projectionMatrix = cameraData.GetProjectionMatrix(i);
				*span[i] = new OccluderSubviewUpdate(i)
				{
					depthSliceIndex = i,
					viewMatrix = viewMatrix,
					invViewMatrix = viewMatrix.inverse,
					gpuProjMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, true),
					viewOffsetWorldSpace = Vector3.zero
				};
			}
			GPUResidentDrawer.UpdateInstanceOccluders(renderGraph, occluderParameters, span);
		}

		private unsafe void InstanceOcclusionTest(RenderGraph renderGraph, UniversalCameraData cameraData, OcclusionTest occlusionTest)
		{
			bool flag = cameraData.xr.enabled && cameraData.xr.singlePassEnabled;
			int num = flag ? 2 : 1;
			OcclusionCullingSettings occlusionCullingSettings = new OcclusionCullingSettings(cameraData.camera.GetInstanceID(), occlusionTest)
			{
				instanceMultiplier = ((flag && !SystemInfo.supportsMultiview) ? 2 : 1)
			};
			int num2 = num;
			Span<SubviewOcclusionTest> span = new Span<SubviewOcclusionTest>(stackalloc byte[checked(unchecked((UIntPtr)num2) * (UIntPtr)sizeof(SubviewOcclusionTest))], num2);
			for (int i = 0; i < num; i++)
			{
				*span[i] = new SubviewOcclusionTest
				{
					cullingSplitIndex = 0,
					occluderSubviewIndex = i
				};
			}
			GPUResidentDrawer.InstanceOcclusionTest(renderGraph, occlusionCullingSettings, span);
		}

		private void RecordCustomPassesWithDepthCopyAndMotion(RenderGraph renderGraph, UniversalResourceData resourceData, RenderPassEvent earliestDepthReadEvent, RenderPassEvent currentEvent, bool renderMotionVectors)
		{
			RenderPassEvent eventStart;
			RenderPassEvent renderPassEvent;
			RenderPassEvent eventEnd;
			base.CalculateSplitEventRange(currentEvent, earliestDepthReadEvent, out eventStart, out renderPassEvent, out eventEnd);
			base.RecordCustomRenderGraphPassesInEventRange(renderGraph, eventStart, renderPassEvent);
			this.ExecuteScheduledDepthCopyWithMotion(renderGraph, resourceData, renderMotionVectors);
			base.RecordCustomRenderGraphPassesInEventRange(renderGraph, renderPassEvent, eventEnd);
		}

		private static bool AllowPartialDepthNormalsPrepass(bool isDeferred, RenderPassEvent requiresDepthNormalEvent, bool useDepthPriming)
		{
			return isDeferred && RenderPassEvent.AfterRenderingGbuffer <= requiresDepthNormalEvent && requiresDepthNormalEvent <= RenderPassEvent.BeforeRenderingOpaques && useDepthPriming;
		}

		private UniversalRenderer.DepthCopySchedule CalculateDepthCopySchedule(RenderPassEvent earliestDepthReadEvent, bool hasFullPrepass)
		{
			UniversalRenderer.DepthCopySchedule result;
			if (earliestDepthReadEvent < RenderPassEvent.AfterRenderingOpaques || this.m_CopyDepthMode == CopyDepthMode.ForcePrepass)
			{
				if (hasFullPrepass)
				{
					result = UniversalRenderer.DepthCopySchedule.AfterPrepass;
				}
				else
				{
					result = UniversalRenderer.DepthCopySchedule.AfterGBuffer;
				}
			}
			else if (earliestDepthReadEvent < RenderPassEvent.AfterRenderingTransparents || this.m_CopyDepthMode == CopyDepthMode.AfterOpaques)
			{
				if (earliestDepthReadEvent < RenderPassEvent.AfterRenderingSkybox)
				{
					result = UniversalRenderer.DepthCopySchedule.AfterOpaques;
				}
				else
				{
					result = UniversalRenderer.DepthCopySchedule.AfterSkybox;
				}
			}
			else if (earliestDepthReadEvent < RenderPassEvent.BeforeRenderingPostProcessing || this.m_CopyDepthMode == CopyDepthMode.AfterTransparents)
			{
				result = UniversalRenderer.DepthCopySchedule.AfterTransparents;
			}
			else
			{
				result = UniversalRenderer.DepthCopySchedule.None;
			}
			return result;
		}

		private UniversalRenderer.TextureCopySchedules CalculateTextureCopySchedules(UniversalCameraData cameraData, in UniversalRenderer.RenderPassInputSummary renderPassInputs, bool requiresDepthPrepass, bool hasFullPrepass, bool requireDepthTexture)
		{
			UniversalRenderer.DepthCopySchedule depth = UniversalRenderer.DepthCopySchedule.None;
			if (requireDepthTexture)
			{
				depth = ((!requiresDepthPrepass || base.useDepthPriming) ? this.CalculateDepthCopySchedule(renderPassInputs.requiresDepthTextureEarliestEvent, hasFullPrepass) : UniversalRenderer.DepthCopySchedule.DuringPrepass);
			}
			UniversalRenderer.ColorCopySchedule color = (cameraData.requiresOpaqueTexture || renderPassInputs.requiresColorTexture) ? UniversalRenderer.ColorCopySchedule.AfterSkybox : UniversalRenderer.ColorCopySchedule.None;
			UniversalRenderer.TextureCopySchedules result;
			result.depth = depth;
			result.color = color;
			return result;
		}

		private void CopyDepthToDepthTexture(RenderGraph renderGraph, UniversalResourceData resourceData)
		{
			this.m_CopyDepthPass.Render(renderGraph, base.frameData, resourceData.cameraDepthTexture, resourceData.activeDepthTexture, true, "Copy Depth");
		}

		private void RenderMotionVectors(RenderGraph renderGraph, UniversalResourceData resourceData)
		{
			this.m_MotionVectorPass.Render(renderGraph, base.frameData, resourceData.cameraDepthTexture, resourceData.motionVectorColor, resourceData.motionVectorDepth);
		}

		private void ExecuteScheduledDepthCopyWithMotion(RenderGraph renderGraph, UniversalResourceData resourceData, bool renderMotionVectors)
		{
			this.CopyDepthToDepthTexture(renderGraph, resourceData);
			if (renderMotionVectors)
			{
				this.RenderMotionVectors(renderGraph, resourceData);
			}
		}

		private void OnMainRendering(RenderGraph renderGraph, ScriptableRenderContext context, in UniversalRenderer.RenderPassInputSummary renderPassInputs, bool requiresPrepass, bool requireDepthTexture)
		{
			UniversalRenderingData universalRenderingData = base.frameData.Get<UniversalRenderingData>();
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = base.frameData.Get<UniversalLightData>();
			base.frameData.Get<UniversalPostProcessingData>();
			if (!renderGraph.nativeRenderPassesEnabled)
			{
				RTClearFlags cameraClearFlag = (RTClearFlags)ScriptableRenderer.GetCameraClearFlag(universalCameraData);
				if (cameraClearFlag != RTClearFlags.None)
				{
					ClearTargetsPass.Render(renderGraph, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, cameraClearFlag, universalCameraData.backgroundColor);
				}
			}
			if (universalRenderingData.stencilLodCrossFadeEnabled)
			{
				this.m_StencilCrossFadeRenderPass.Render(renderGraph, context, universalResourceData.activeDepthTexture);
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingPrePasses);
			bool flag = requiresPrepass && !renderPassInputs.requiresNormalsTexture;
			bool flag2 = requiresPrepass && renderPassInputs.requiresNormalsTexture;
			bool flag3 = flag || (flag2 && !UniversalRenderer.AllowPartialDepthNormalsPrepass(this.usesDeferredLighting, renderPassInputs.requiresDepthNormalAtEvent, base.useDepthPriming));
			UniversalRenderer.TextureCopySchedules textureCopySchedules = this.CalculateTextureCopySchedules(universalCameraData, renderPassInputs, requiresPrepass, flag3, requireDepthTexture);
			bool flag4 = RenderPassEvent.AfterRenderingGbuffer <= renderPassInputs.requiresDepthNormalAtEvent && renderPassInputs.requiresDepthNormalAtEvent <= RenderPassEvent.BeforeRenderingOpaques;
			bool flag5 = requiresPrepass && (!this.usesDeferredLighting || !flag4);
			UniversalRenderer.OccluderPass occluderPass = UniversalRenderer.OccluderPass.None;
			if (universalCameraData.useGPUOcclusionCulling)
			{
				if (flag5)
				{
					occluderPass = UniversalRenderer.OccluderPass.DepthPrepass;
				}
				else
				{
					occluderPass = (this.usesDeferredLighting ? UniversalRenderer.OccluderPass.GBuffer : UniversalRenderer.OccluderPass.ForwardOpaque);
				}
			}
			if (universalCameraData.xr.enabled && universalCameraData.xr.hasMotionVectorPass)
			{
				XRDepthMotionPass xrdepthMotionPass = this.m_XRDepthMotionPass;
				if (xrdepthMotionPass != null)
				{
					xrdepthMotionPass.Update(ref universalCameraData);
				}
				XRDepthMotionPass xrdepthMotionPass2 = this.m_XRDepthMotionPass;
				if (xrdepthMotionPass2 != null)
				{
					xrdepthMotionPass2.Render(renderGraph, base.frameData);
				}
			}
			if (requiresPrepass)
			{
				TextureHandle textureHandle = base.useDepthPriming ? universalResourceData.activeDepthTexture : universalResourceData.cameraDepthTexture;
				if (universalRenderingData.stencilLodCrossFadeEnabled && flag2 && !base.useDepthPriming)
				{
					this.m_StencilCrossFadeRenderPass.Render(renderGraph, context, universalResourceData.cameraDepthTexture);
				}
				bool flag6 = occluderPass == UniversalRenderer.OccluderPass.DepthPrepass;
				int num = flag6 ? 2 : 1;
				for (int i = 0; i < num; i++)
				{
					uint batchLayerMask = uint.MaxValue;
					if (flag6)
					{
						OcclusionTest occlusionTest = (i == 0) ? OcclusionTest.TestAll : OcclusionTest.TestCulled;
						this.InstanceOcclusionTest(renderGraph, universalCameraData, occlusionTest);
						batchLayerMask = occlusionTest.GetBatchLayerMask();
					}
					bool flag7 = i == num - 1;
					bool setGlobalDepth = flag7 && !base.useDepthPriming;
					bool setGlobalTextures = flag7 && flag3;
					if (flag2)
					{
						this.DepthNormalPrepassRender(renderGraph, renderPassInputs, textureHandle, batchLayerMask, setGlobalDepth, setGlobalTextures);
					}
					else
					{
						this.m_DepthPrepass.Render(renderGraph, base.frameData, ref textureHandle, batchLayerMask, setGlobalDepth);
					}
					if (flag6)
					{
						this.UpdateInstanceOccluders(renderGraph, universalCameraData, textureHandle);
						if (i != 0)
						{
							this.InstanceOcclusionTest(renderGraph, universalCameraData, OcclusionTest.TestAll);
						}
					}
				}
			}
			if (textureCopySchedules.depth == UniversalRenderer.DepthCopySchedule.AfterPrepass)
			{
				this.ExecuteScheduledDepthCopyWithMotion(renderGraph, universalResourceData, renderPassInputs.requiresMotionVectors);
			}
			else if (textureCopySchedules.depth == UniversalRenderer.DepthCopySchedule.DuringPrepass && renderPassInputs.requiresMotionVectors)
			{
				this.RenderMotionVectors(renderGraph, universalResourceData);
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingPrePasses);
			if (universalCameraData.xr.hasValidOcclusionMesh)
			{
				XROcclusionMeshPass xrocclusionMeshPass = this.m_XROcclusionMeshPass;
				ContextContainer frameData = base.frameData;
				TextureHandle activeColorTexture = universalResourceData.activeColorTexture;
				TextureHandle activeDepthTexture = universalResourceData.activeDepthTexture;
				xrocclusionMeshPass.Render(renderGraph, frameData, activeColorTexture, activeDepthTexture);
			}
			if (this.usesDeferredLighting)
			{
				this.m_DeferredLights.Setup(this.m_AdditionalLightsShadowCasterPass);
				this.m_DeferredLights.UseFramebufferFetch = renderGraph.nativeRenderPassesEnabled;
				this.m_DeferredLights.HasNormalPrepass = flag2;
				this.m_DeferredLights.HasDepthPrepass = requiresPrepass;
				this.m_DeferredLights.ResolveMixedLightingMode(lightData);
				this.m_DeferredLights.CreateGbufferResourcesRenderGraph(renderGraph, universalResourceData);
				universalResourceData.gBuffer = this.m_DeferredLights.GbufferTextureHandles;
				base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingGbuffer);
				bool flag8 = occluderPass == UniversalRenderer.OccluderPass.GBuffer;
				int num2 = flag8 ? 2 : 1;
				for (int j = 0; j < num2; j++)
				{
					uint batchLayerMask2 = uint.MaxValue;
					if (flag8)
					{
						OcclusionTest occlusionTest2 = (j == 0) ? OcclusionTest.TestAll : OcclusionTest.TestCulled;
						this.InstanceOcclusionTest(renderGraph, universalCameraData, occlusionTest2);
						batchLayerMask2 = occlusionTest2.GetBatchLayerMask();
					}
					bool setGlobalTextures2 = flag2 && !flag3;
					this.m_GBufferPass.Render(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, setGlobalTextures2, batchLayerMask2);
					if (flag8)
					{
						this.UpdateInstanceOccluders(renderGraph, universalCameraData, universalResourceData.activeDepthTexture);
						if (j != 0)
						{
							this.InstanceOcclusionTest(renderGraph, universalCameraData, OcclusionTest.TestAll);
						}
					}
				}
				if (textureCopySchedules.depth == UniversalRenderer.DepthCopySchedule.AfterGBuffer)
				{
					this.ExecuteScheduledDepthCopyWithMotion(renderGraph, universalResourceData, renderPassInputs.requiresMotionVectors);
				}
				else if (!renderGraph.nativeRenderPassesEnabled)
				{
					this.CopyDepthToDepthTexture(renderGraph, universalResourceData);
				}
				base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingGbuffer, RenderPassEvent.BeforeRenderingDeferredLights);
				this.m_DeferredPass.Render(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, universalResourceData.gBuffer);
				base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingDeferredLights, RenderPassEvent.BeforeRenderingOpaques);
				TextureHandle mainShadowsTexture = universalResourceData.mainShadowsTexture;
				TextureHandle additionalShadowsTexture = universalResourceData.additionalShadowsTexture;
				this.m_RenderOpaqueForwardOnlyPass.Render(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, mainShadowsTexture, additionalShadowsTexture, uint.MaxValue);
			}
			else
			{
				base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingGbuffer, RenderPassEvent.BeforeRenderingOpaques);
				bool flag9 = occluderPass == UniversalRenderer.OccluderPass.ForwardOpaque;
				int num3 = flag9 ? 2 : 1;
				for (int k = 0; k < num3; k++)
				{
					uint batchLayerMask3 = uint.MaxValue;
					if (flag9)
					{
						OcclusionTest occlusionTest3 = (k == 0) ? OcclusionTest.TestAll : OcclusionTest.TestCulled;
						this.InstanceOcclusionTest(renderGraph, universalCameraData, occlusionTest3);
						batchLayerMask3 = occlusionTest3.GetBatchLayerMask();
					}
					if (this.m_RenderingLayerProvidesRenderObjectPass)
					{
						this.m_RenderOpaqueForwardWithRenderingLayersPass.Render(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.renderingLayersTexture, universalResourceData.activeDepthTexture, universalResourceData.mainShadowsTexture, universalResourceData.additionalShadowsTexture, this.m_RenderingLayersMaskSize, batchLayerMask3);
						this.SetRenderingLayersGlobalTextures(renderGraph);
					}
					else
					{
						this.m_RenderOpaqueForwardPass.Render(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, universalResourceData.mainShadowsTexture, universalResourceData.additionalShadowsTexture, batchLayerMask3);
					}
					if (flag9)
					{
						this.UpdateInstanceOccluders(renderGraph, universalCameraData, universalResourceData.activeDepthTexture);
						if (k != 0)
						{
							this.InstanceOcclusionTest(renderGraph, universalCameraData, OcclusionTest.TestAll);
						}
					}
				}
			}
			if (textureCopySchedules.depth == UniversalRenderer.DepthCopySchedule.AfterOpaques)
			{
				this.RecordCustomPassesWithDepthCopyAndMotion(renderGraph, universalResourceData, renderPassInputs.requiresDepthTextureEarliestEvent, RenderPassEvent.AfterRenderingOpaques, renderPassInputs.requiresMotionVectors);
			}
			else
			{
				base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingOpaques);
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingSkybox);
			if (universalCameraData.camera.clearFlags == CameraClearFlags.Skybox && universalCameraData.renderType != CameraRenderType.Overlay)
			{
				Skybox skybox;
				universalCameraData.camera.TryGetComponent<Skybox>(out skybox);
				Material material = (skybox != null) ? skybox.material : RenderSettings.skybox;
				if (material != null)
				{
					this.m_DrawSkyboxPass.Render(renderGraph, base.frameData, context, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, material);
				}
			}
			if (textureCopySchedules.depth == UniversalRenderer.DepthCopySchedule.AfterSkybox)
			{
				this.ExecuteScheduledDepthCopyWithMotion(renderGraph, universalResourceData, renderPassInputs.requiresMotionVectors);
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingSkybox);
			if (textureCopySchedules.color == UniversalRenderer.ColorCopySchedule.AfterSkybox)
			{
				TextureHandle activeColorTexture2 = universalResourceData.activeColorTexture;
				Downsampling opaqueDownsampling = UniversalRenderPipeline.asset.opaqueDownsampling;
				TextureHandle cameraOpaqueTexture;
				this.m_CopyColorPass.Render(renderGraph, base.frameData, out cameraOpaqueTexture, activeColorTexture2, opaqueDownsampling);
				universalResourceData.cameraOpaqueTexture = cameraOpaqueTexture;
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingTransparents);
			this.m_RenderTransparentForwardPass.m_ShouldTransparentsReceiveShadows = !this.m_TransparentSettingsPass.Setup();
			this.m_RenderTransparentForwardPass.Render(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, universalResourceData.mainShadowsTexture, universalResourceData.additionalShadowsTexture, uint.MaxValue);
			if (textureCopySchedules.depth == UniversalRenderer.DepthCopySchedule.AfterTransparents)
			{
				this.RecordCustomPassesWithDepthCopyAndMotion(renderGraph, universalResourceData, renderPassInputs.requiresDepthTextureEarliestEvent, RenderPassEvent.AfterRenderingTransparents, renderPassInputs.requiresMotionVectors);
			}
			else
			{
				base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingTransparents);
			}
			if (context.HasInvokeOnRenderObjectCallbacks())
			{
				this.m_OnRenderObjectCallbackPass.Render(renderGraph, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture);
			}
			if (universalResourceData != null)
			{
				this.SetupVFXCameraBuffer(universalCameraData);
			}
			this.RenderRawColorDepthHistory(renderGraph, universalCameraData, universalResourceData);
			bool rendersOverlayUI = universalCameraData.rendersOverlayUI;
			bool isHDROutputActive = universalCameraData.isHDROutputActive;
			if (rendersOverlayUI && isHDROutputActive)
			{
				TextureHandle overlayUITexture;
				this.m_DrawOffscreenUIPass.RenderOffscreen(renderGraph, base.frameData, this.cameraDepthAttachmentFormat, out overlayUITexture);
				universalResourceData.overlayUITexture = overlayUITexture;
			}
		}

		private void OnAfterRendering(RenderGraph renderGraph, bool applyPostProcessing)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			base.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = base.frameData.Get<UniversalCameraData>();
			UniversalPostProcessingData universalPostProcessingData = base.frameData.Get<UniversalPostProcessingData>();
			if (universalCameraData.resolveFinalTarget)
			{
				this.SetupRenderGraphFinalPassDebug(renderGraph, base.frameData);
			}
			bool flag = DebugDisplaySettings<UniversalRenderPipelineDebugDisplaySettings>.Instance.renderingSettings.sceneOverrideMode == DebugSceneOverrideMode.None;
			if (flag)
			{
				base.DrawRenderGraphGizmos(renderGraph, base.frameData, universalResourceData.activeColorTexture, universalResourceData.activeDepthTexture, GizmoSubset.PreImageEffects);
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.BeforeRenderingPostProcessing);
			bool flag2 = universalPostProcessingData.isEnabled && this.m_PostProcessPasses.isCreated && universalCameraData.resolveFinalTarget && (universalCameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing || (universalCameraData.imageScalingMode == ImageScalingMode.Upscaling && universalCameraData.upscalingFilter != ImageUpscalingFilter.Linear) || (universalCameraData.IsTemporalAAEnabled() && universalCameraData.taaSettings.contrastAdaptiveSharpening > 0f));
			bool flag3 = universalCameraData.captureActions != null && universalCameraData.resolveFinalTarget;
			bool flag4 = base.activeRenderPassQueue.Find((ScriptableRenderPass x) => x.renderPassEvent >= RenderPassEvent.AfterRenderingPostProcessing && x.renderPassEvent < RenderPassEvent.AfterRendering) != null;
			bool flag5 = !flag3 && !flag4 && !flag2;
			bool flag6 = base.DebugHandler == null || !base.DebugHandler.HDRDebugViewIsActive(universalCameraData.resolveFinalTarget);
			bool flag7 = universalResourceData.activeDepthID == UniversalResourceDataBase.ActiveID.BackBuffer;
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(universalCameraData);
			bool flag8 = activeDebugHandler != null && activeDebugHandler.WriteToDebugScreenTexture(universalCameraData.resolveFinalTarget);
			if (flag8)
			{
				RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
				DebugHandler.ConfigureColorDescriptorForDebugScreen(ref cameraTargetDescriptor, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
				universalResourceData.debugScreenColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_DebugScreenColor", false, FilterMode.Point, TextureWrapMode.Clamp);
				RenderTextureDescriptor cameraTargetDescriptor2 = universalCameraData.cameraTargetDescriptor;
				DebugHandler.ConfigureDepthDescriptorForDebugScreen(ref cameraTargetDescriptor2, this.cameraDepthAttachmentFormat, universalCameraData.pixelWidth, universalCameraData.pixelHeight);
				universalResourceData.debugScreenDepth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor2, "_DebugScreenDepth", false, FilterMode.Point, TextureWrapMode.Clamp);
			}
			TextureHandle afterPostProcessColor = universalResourceData.afterPostProcessColor;
			if (applyPostProcessing)
			{
				TextureHandle activeColorTexture = universalResourceData.activeColorTexture;
				TextureHandle backBufferColor = universalResourceData.backBufferColor;
				TextureHandle internalColorLut = universalResourceData.internalColorLut;
				TextureHandle overlayUITexture = universalResourceData.overlayUITexture;
				bool flag9 = universalCameraData.resolveFinalTarget && !flag2 && !flag4;
				TextureHandle textureHandle;
				if (flag9)
				{
					textureHandle = backBufferColor;
				}
				else
				{
					ImportResourceParams importParams = default(ImportResourceParams);
					importParams.clearOnFirstUse = true;
					importParams.clearColor = Color.black;
					importParams.discardOnLastUse = universalCameraData.resolveFinalTarget;
					if (universalCameraData.IsSTPEnabled())
					{
						TextureDesc descriptor = universalResourceData.cameraColor.GetDescriptor(renderGraph);
						PostProcessPass.MakeCompatible(ref descriptor);
						descriptor.width = universalCameraData.pixelWidth;
						descriptor.height = universalCameraData.pixelHeight;
						descriptor.name = "_CameraColorUpscaled";
						universalResourceData.cameraColor = renderGraph.CreateTexture(descriptor);
					}
					else
					{
						bool flag10 = universalCameraData.resolveFinalTarget && universalCameraData.renderType == CameraRenderType.Base;
						universalResourceData.cameraColor = (flag10 ? renderGraph.CreateTexture(activeColorTexture, "_CameraColorAfterPostProcessing", false) : renderGraph.ImportTexture(this.nextRenderGraphCameraColorHandle, importParams));
					}
					textureHandle = universalResourceData.cameraColor;
				}
				if (flag8 && flag9)
				{
					textureHandle = universalResourceData.debugScreenColor;
				}
				bool enableColorEndingIfNeeded = flag5 && flag6;
				this.m_PostProcessPasses.postProcessPass.RenderPostProcessingRenderGraph(renderGraph, base.frameData, activeColorTexture, internalColorLut, overlayUITexture, textureHandle, flag2, flag8, enableColorEndingIfNeeded);
				if (universalCameraData.resolveFinalTarget)
				{
					this.SetupAfterPostRenderGraphFinalPassDebug(renderGraph, base.frameData);
				}
				if (flag9)
				{
					universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
					universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
				}
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRenderingPostProcessing);
			if (universalCameraData.captureActions != null)
			{
				this.m_CapturePass.RecordRenderGraph(renderGraph, base.frameData);
			}
			if (flag2)
			{
				TextureHandle backBufferColor2 = universalResourceData.backBufferColor;
				TextureHandle overlayUITexture2 = universalResourceData.overlayUITexture;
				TextureHandle textureHandle2 = backBufferColor2;
				if (flag8)
				{
					textureHandle2 = universalResourceData.debugScreenColor;
				}
				TextureHandle cameraColor = universalResourceData.cameraColor;
				this.m_PostProcessPasses.finalPostProcessPass.RenderFinalPassRenderGraph(renderGraph, base.frameData, cameraColor, overlayUITexture2, textureHandle2, flag6);
				universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
				universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
			}
			bool flag11 = flag2 || (applyPostProcessing && !flag4 && !flag3);
			if (!universalResourceData.isActiveTargetBackBuffer && universalCameraData.resolveFinalTarget && !flag11)
			{
				TextureHandle backBufferColor3 = universalResourceData.backBufferColor;
				TextureHandle overlayUITexture3 = universalResourceData.overlayUITexture;
				TextureHandle textureHandle3 = backBufferColor3;
				if (flag8)
				{
					textureHandle3 = universalResourceData.debugScreenColor;
				}
				TextureHandle cameraColor2 = universalResourceData.cameraColor;
				this.m_FinalBlitPass.Render(renderGraph, base.frameData, universalCameraData, cameraColor2, textureHandle3, overlayUITexture3);
				universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.BackBuffer;
				universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.BackBuffer;
			}
			base.RecordCustomRenderGraphPasses(renderGraph, RenderPassEvent.AfterRendering);
			bool flag12 = universalCameraData.rendersOverlayUI && universalCameraData.isLastBaseCamera;
			bool isHDROutputActive = universalCameraData.isHDROutputActive;
			if (flag12 && !isHDROutputActive)
			{
				TextureHandle textureHandle4 = universalResourceData.backBufferDepth;
				TextureHandle textureHandle5 = universalResourceData.backBufferColor;
				if (flag8)
				{
					textureHandle5 = universalResourceData.debugScreenColor;
					textureHandle4 = universalResourceData.debugScreenDepth;
				}
				this.m_DrawOverlayUIPass.RenderOverlay(renderGraph, base.frameData, textureHandle5, textureHandle4);
			}
			if (universalCameraData.xr.enabled && !flag7 && universalCameraData.xr.copyDepth)
			{
				this.m_XRCopyDepthPass.CopyToDepthXR = true;
				this.m_XRCopyDepthPass.MsaaSamples = 1;
				this.m_XRCopyDepthPass.Render(renderGraph, base.frameData, universalResourceData.backBufferDepth, universalResourceData.cameraDepth, false, "XR Depth Copy");
			}
			if (activeDebugHandler != null)
			{
				TextureHandle overlayUITexture4 = universalResourceData.overlayUITexture;
				TextureHandle debugScreenColor = universalResourceData.debugScreenColor;
			}
			if (universalCameraData.resolveFinalTarget)
			{
				if (universalCameraData.isSceneViewCamera)
				{
					base.DrawRenderGraphWireOverlay(renderGraph, base.frameData, universalResourceData.backBufferColor);
				}
				if (flag)
				{
					base.DrawRenderGraphGizmos(renderGraph, base.frameData, universalResourceData.backBufferColor, universalResourceData.activeDepthTexture, GizmoSubset.PostImageEffects);
				}
			}
		}

		private bool RequirePrepassForTextures(UniversalCameraData cameraData, in UniversalRenderer.RenderPassInputSummary renderPassInputs, bool requireDepthTexture)
		{
			return (requireDepthTexture && !UniversalRenderer.CanCopyDepth(cameraData)) | (cameraData.requiresDepthTexture && this.m_CopyDepthMode == CopyDepthMode.ForcePrepass) | renderPassInputs.requiresDepthPrepass | this.DebugHandlerRequireDepthPass(cameraData) | renderPassInputs.requiresNormalsTexture;
		}

		private static bool RequireDepthTexture(UniversalCameraData cameraData, in UniversalRenderer.RenderPassInputSummary renderPassInputs, bool applyPostProcessing)
		{
			bool flag = cameraData.requiresDepthTexture || renderPassInputs.requiresDepthTexture;
			bool flag2 = applyPostProcessing && cameraData.postProcessingRequiresDepthTexture;
			return flag || flag2;
		}

		private static bool IsDepthPrimingEnabledRenderGraph(UniversalCameraData cameraData, in UniversalRenderer.RenderPassInputSummary renderPassInputs, DepthPrimingMode depthPrimingMode, bool requireDepthTexture, bool requirePrepassForTextures, bool usesDeferredLighting)
		{
			bool flag = true;
			if (requireDepthTexture && !UniversalRenderer.CanCopyDepth(cameraData))
			{
				return false;
			}
			bool flag2 = !UniversalRenderer.IsWebGL();
			bool flag3 = (flag && depthPrimingMode == DepthPrimingMode.Auto) || depthPrimingMode == DepthPrimingMode.Forced;
			bool flag4 = cameraData.cameraTargetDescriptor.msaaSamples == 1;
			if (usesDeferredLighting && RenderPassEvent.AfterRenderingGbuffer <= renderPassInputs.requiresDepthNormalAtEvent && renderPassInputs.requiresDepthNormalAtEvent <= RenderPassEvent.BeforeRenderingOpaques && requirePrepassForTextures && flag4)
			{
				return true;
			}
			bool flag5 = cameraData.renderType == CameraRenderType.Base || cameraData.clearDepth;
			bool flag6 = !UniversalRenderer.IsOffscreenDepthTexture(cameraData);
			return flag3 && !usesDeferredLighting && flag5 && flag6 && flag2 && flag4;
		}

		internal void SetRenderingLayersGlobalTextures(RenderGraph renderGraph)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			if (universalResourceData.renderingLayersTexture.IsValid() && !this.usesDeferredLighting)
			{
				RenderGraphUtils.SetGlobalTexture(renderGraph, Shader.PropertyToID(this.m_RenderingLayersTextureName), universalResourceData.renderingLayersTexture, "Set Global Rendering Layers Texture", ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\UniversalRendererRenderGraph.cs", 1626);
			}
		}

		private void ImportBackBuffers(RenderGraph renderGraph, UniversalCameraData cameraData, Color clearBackgroundColor, bool isCameraTargetOffscreenDepth)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			bool flag = cameraData.renderType == CameraRenderType.Base && !UniversalRenderer.m_RequiresIntermediateAttachments;
			flag = (flag || isCameraTargetOffscreenDepth);
			bool flag2 = !SupportedRenderingFeatures.active.rendersUIOverlay && cameraData.resolveToScreen;
			bool flag3 = Watermark.IsVisible() || flag2;
			bool discardOnLastUse = !UniversalRenderer.m_RequiresIntermediateAttachments && !flag3 && cameraData.cameraTargetDescriptor.msaaSamples > 1;
			ImportResourceParams importParams = default(ImportResourceParams);
			importParams.clearOnFirstUse = flag;
			importParams.clearColor = clearBackgroundColor;
			importParams.discardOnLastUse = discardOnLastUse;
			ImportResourceParams importParams2 = default(ImportResourceParams);
			importParams2.clearOnFirstUse = flag;
			importParams2.clearColor = clearBackgroundColor;
			importParams2.discardOnLastUse = !isCameraTargetOffscreenDepth;
			if (cameraData.xr.enabled && cameraData.xr.copyDepth)
			{
				importParams2.discardOnLastUse = false;
			}
			RenderTargetInfo renderTargetInfo = default(RenderTargetInfo);
			RenderTargetInfo renderTargetInfo2 = default(RenderTargetInfo);
			bool flag4 = cameraData.targetTexture == null;
			if (cameraData.xr.enabled)
			{
				flag4 = false;
			}
			if (flag4)
			{
				int msaaSamples = base.AdjustAndGetScreenMSAASamples(renderGraph, UniversalRenderer.m_RequiresIntermediateAttachments);
				renderTargetInfo.width = Screen.width;
				renderTargetInfo.height = Screen.height;
				renderTargetInfo.volumeDepth = 1;
				renderTargetInfo.msaaSamples = msaaSamples;
				renderTargetInfo.format = cameraData.cameraTargetDescriptor.graphicsFormat;
				renderTargetInfo2 = renderTargetInfo;
				renderTargetInfo2.format = cameraData.cameraTargetDescriptor.depthStencilFormat;
			}
			else
			{
				if (cameraData.xr.enabled)
				{
					renderTargetInfo.width = cameraData.xr.renderTargetDesc.width;
					renderTargetInfo.height = cameraData.xr.renderTargetDesc.height;
					renderTargetInfo.volumeDepth = cameraData.xr.renderTargetDesc.volumeDepth;
					renderTargetInfo.msaaSamples = cameraData.xr.renderTargetDesc.msaaSamples;
					renderTargetInfo.format = cameraData.xr.renderTargetDesc.graphicsFormat;
					if (!UniversalRenderer.PlatformRequiresExplicitMsaaResolve())
					{
						renderTargetInfo.bindMS = (renderTargetInfo.msaaSamples > 1);
					}
					renderTargetInfo2 = renderTargetInfo;
					renderTargetInfo2.format = cameraData.xr.renderTargetDesc.depthStencilFormat;
				}
				else
				{
					renderTargetInfo.width = cameraData.targetTexture.width;
					renderTargetInfo.height = cameraData.targetTexture.height;
					renderTargetInfo.volumeDepth = cameraData.targetTexture.volumeDepth;
					renderTargetInfo.msaaSamples = cameraData.targetTexture.antiAliasing;
					renderTargetInfo.format = cameraData.targetTexture.graphicsFormat;
					renderTargetInfo2 = renderTargetInfo;
					renderTargetInfo2.format = cameraData.targetTexture.depthStencilFormat;
				}
				if (renderTargetInfo2.format == GraphicsFormat.None)
				{
					renderTargetInfo2.format = SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil);
					Debug.LogWarning("In the render graph API, the output Render Texture must have a depth buffer. When you select a Render Texture in any camera's Output Texture property, the Depth Stencil Format property of the texture must be set to a value other than None.");
				}
			}
			if (!isCameraTargetOffscreenDepth)
			{
				universalResourceData.backBufferColor = renderGraph.ImportTexture(this.m_TargetColorHandle, renderTargetInfo, importParams);
			}
			universalResourceData.backBufferDepth = renderGraph.ImportTexture(this.m_TargetDepthHandle, renderTargetInfo2, importParams2);
		}

		private void CreateIntermediateCameraColorAttachment(RenderGraph renderGraph, UniversalCameraData cameraData, bool clearColor, Color clearBackgroundColor)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
			cameraTargetDescriptor.useMipMap = false;
			cameraTargetDescriptor.autoGenerateMips = false;
			cameraTargetDescriptor.depthStencilFormat = GraphicsFormat.None;
			if (cameraData.resolveFinalTarget && cameraData.renderType == CameraRenderType.Base)
			{
				universalResourceData.cameraColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_CameraTargetAttachment", clearColor, clearBackgroundColor, FilterMode.Bilinear, TextureWrapMode.Clamp, cameraData.resolveFinalTarget);
				UniversalRenderer.m_CurrentColorHandle = -1;
			}
			else
			{
				RenderingUtils.ReAllocateHandleIfNeeded(ref UniversalRenderer.m_RenderGraphCameraColorHandles[0], cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_CameraTargetAttachmentA");
				RenderingUtils.ReAllocateHandleIfNeeded(ref UniversalRenderer.m_RenderGraphCameraColorHandles[1], cameraTargetDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_CameraTargetAttachmentB");
				if (cameraData.renderType == CameraRenderType.Base)
				{
					UniversalRenderer.m_CurrentColorHandle = 0;
				}
				ImportResourceParams importParams = default(ImportResourceParams);
				importParams.clearOnFirstUse = clearColor;
				importParams.clearColor = clearBackgroundColor;
				importParams.discardOnLastUse = cameraData.resolveFinalTarget;
				universalResourceData.cameraColor = renderGraph.ImportTexture(this.currentRenderGraphCameraColorHandle, importParams);
			}
			universalResourceData.activeColorID = UniversalResourceDataBase.ActiveID.Camera;
		}

		private void CreateIntermediateCameraDepthAttachment(RenderGraph renderGraph, UniversalCameraData cameraData, bool clearDepth, Color clearBackgroundDepth, bool depthTextureIsDepthFormat)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			bool resolveFinalTarget = cameraData.resolveFinalTarget;
			RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
			cameraTargetDescriptor.useMipMap = false;
			cameraTargetDescriptor.autoGenerateMips = false;
			bool flag = cameraTargetDescriptor.msaaSamples > 1;
			bool flag2 = RenderingUtils.MultisampleDepthResolveSupported() && renderGraph.nativeRenderPassesEnabled;
			cameraTargetDescriptor.bindMS = (!flag2 && flag);
			if (UniversalRenderer.IsGLESDevice())
			{
				cameraTargetDescriptor.bindMS = false;
			}
			cameraTargetDescriptor.graphicsFormat = GraphicsFormat.None;
			cameraTargetDescriptor.depthStencilFormat = this.cameraDepthAttachmentFormat;
			RenderingUtils.ReAllocateHandleIfNeeded(ref UniversalRenderer.m_RenderGraphCameraDepthHandle, cameraTargetDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_CameraDepthAttachment");
			ImportResourceParams importParams = default(ImportResourceParams);
			importParams.clearOnFirstUse = clearDepth;
			importParams.clearColor = clearBackgroundDepth;
			importParams.discardOnLastUse = resolveFinalTarget;
			universalResourceData.cameraDepth = renderGraph.ImportTexture(UniversalRenderer.m_RenderGraphCameraDepthHandle, importParams);
			universalResourceData.activeDepthID = UniversalResourceDataBase.ActiveID.Camera;
			this.m_CopyDepthPass.MsaaSamples = cameraTargetDescriptor.msaaSamples;
			this.m_CopyDepthPass.CopyToDepth = depthTextureIsDepthFormat;
			bool copyResolvedDepth = !cameraTargetDescriptor.bindMS;
			this.m_CopyDepthPass.m_CopyResolvedDepth = copyResolvedDepth;
			this.m_XRCopyDepthPass.m_CopyResolvedDepth = copyResolvedDepth;
		}

		private void CreateCameraDepthCopyTexture(RenderGraph renderGraph, RenderTextureDescriptor descriptor, bool isDepthTexture)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			RenderTextureDescriptor desc = descriptor;
			desc.msaaSamples = 1;
			if (isDepthTexture)
			{
				desc.graphicsFormat = GraphicsFormat.None;
				desc.depthStencilFormat = this.cameraDepthTextureFormat;
			}
			else
			{
				desc.graphicsFormat = GraphicsFormat.R32_SFloat;
				desc.depthStencilFormat = GraphicsFormat.None;
			}
			universalResourceData.cameraDepthTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_CameraDepthTexture", true, FilterMode.Point, TextureWrapMode.Clamp);
		}

		private void CreateMotionVectorTextures(RenderGraph renderGraph, RenderTextureDescriptor descriptor)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			RenderTextureDescriptor desc = descriptor;
			desc.msaaSamples = 1;
			desc.graphicsFormat = GraphicsFormat.R16G16_SFloat;
			desc.depthStencilFormat = GraphicsFormat.None;
			universalResourceData.motionVectorColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_MotionVectorTexture", true, FilterMode.Point, TextureWrapMode.Clamp);
			RenderTextureDescriptor desc2 = descriptor;
			desc2.msaaSamples = 1;
			desc2.graphicsFormat = GraphicsFormat.None;
			desc2.depthStencilFormat = this.cameraDepthAttachmentFormat;
			universalResourceData.motionVectorDepth = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc2, "_MotionVectorDepthTexture", true, FilterMode.Point, TextureWrapMode.Clamp);
		}

		private void CreateCameraNormalsTexture(RenderGraph renderGraph, RenderTextureDescriptor descriptor)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			RenderTextureDescriptor desc = descriptor;
			desc.depthStencilFormat = GraphicsFormat.None;
			desc.msaaSamples = 1;
			string name = (!this.usesDeferredLighting) ? DepthNormalOnlyPass.k_CameraNormalsTextureName : DeferredLights.k_GBufferNames[this.m_DeferredLights.GBufferNormalSmoothnessIndex];
			desc.graphicsFormat = ((!this.usesDeferredLighting) ? DepthNormalOnlyPass.GetGraphicsFormat() : this.m_DeferredLights.GetGBufferFormat(this.m_DeferredLights.GBufferNormalSmoothnessIndex));
			universalResourceData.cameraNormalsTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, name, true, FilterMode.Point, TextureWrapMode.Clamp);
		}

		private void CreateRenderingLayersTexture(RenderGraph renderGraph, RenderTextureDescriptor descriptor)
		{
			if (this.m_RequiresRenderingLayer)
			{
				UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
				this.m_RenderingLayersTextureName = "_CameraRenderingLayersTexture";
				if (this.usesDeferredLighting && this.m_DeferredLights.UseRenderingLayers)
				{
					this.m_RenderingLayersTextureName = DeferredLights.k_GBufferNames[this.m_DeferredLights.GBufferRenderingLayers];
				}
				RenderTextureDescriptor desc = descriptor;
				desc.depthStencilFormat = GraphicsFormat.None;
				if (!this.m_RenderingLayerProvidesRenderObjectPass)
				{
					desc.msaaSamples = 1;
				}
				if (this.usesDeferredLighting && this.m_RequiresRenderingLayer)
				{
					desc.graphicsFormat = this.m_DeferredLights.GetGBufferFormat(this.m_DeferredLights.GBufferRenderingLayers);
				}
				else
				{
					desc.graphicsFormat = RenderingLayerUtils.GetFormat(this.m_RenderingLayersMaskSize);
				}
				universalResourceData.renderingLayersTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, this.m_RenderingLayersTextureName, true, FilterMode.Point, TextureWrapMode.Clamp);
			}
		}

		private void CreateAfterPostProcessTexture(RenderGraph renderGraph, RenderTextureDescriptor descriptor)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			RenderTextureDescriptor compatibleDescriptor = PostProcessPass.GetCompatibleDescriptor(descriptor, descriptor.width, descriptor.height, descriptor.graphicsFormat, GraphicsFormat.None);
			universalResourceData.afterPostProcessColor = UniversalRenderer.CreateRenderGraphTexture(renderGraph, compatibleDescriptor, "_AfterPostProcessTexture", true, FilterMode.Point, TextureWrapMode.Clamp);
		}

		private void DepthNormalPrepassRender(RenderGraph renderGraph, UniversalRenderer.RenderPassInputSummary renderPassInputs, TextureHandle depthTarget, uint batchLayerMask, bool setGlobalDepth, bool setGlobalTextures)
		{
			UniversalResourceData universalResourceData = base.frameData.Get<UniversalResourceData>();
			if (this.m_RenderingLayerProvidesByDepthNormalPass)
			{
				this.m_DepthNormalPrepass.enableRenderingLayers = true;
				this.m_DepthNormalPrepass.renderingLayersMaskSize = this.m_RenderingLayersMaskSize;
			}
			else
			{
				this.m_DepthNormalPrepass.enableRenderingLayers = false;
			}
			if (this.usesDeferredLighting && UniversalRenderer.AllowPartialDepthNormalsPrepass(this.usesDeferredLighting, renderPassInputs.requiresDepthNormalAtEvent, base.useDepthPriming))
			{
				this.m_DepthNormalPrepass.shaderTagIds = UniversalRenderer.k_DepthNormalsOnly;
			}
			TextureHandle cameraNormalsTexture = universalResourceData.cameraNormalsTexture;
			TextureHandle renderingLayersTexture = universalResourceData.renderingLayersTexture;
			this.m_DepthNormalPrepass.Render(renderGraph, base.frameData, cameraNormalsTexture, depthTarget, renderingLayersTexture, batchLayerMask, setGlobalDepth, setGlobalTextures);
			if (this.m_RequiresRenderingLayer)
			{
				this.SetRenderingLayersGlobalTextures(renderGraph);
			}
		}

		private const int k_FinalBlitPassQueueOffset = 1;

		private const int k_AfterFinalBlitPassQueueOffset = 2;

		private static readonly List<ShaderTagId> k_DepthNormalsOnly = new List<ShaderTagId>
		{
			new ShaderTagId("DepthNormalsOnly")
		};

		private DepthOnlyPass m_DepthPrepass;

		private DepthNormalOnlyPass m_DepthNormalPrepass;

		private CopyDepthPass m_PrimedDepthCopyPass;

		private MotionVectorRenderPass m_MotionVectorPass;

		private MainLightShadowCasterPass m_MainLightShadowCasterPass;

		private AdditionalLightsShadowCasterPass m_AdditionalLightsShadowCasterPass;

		private GBufferPass m_GBufferPass;

		private CopyDepthPass m_GBufferCopyDepthPass;

		private DeferredPass m_DeferredPass;

		private DrawObjectsPass m_RenderOpaqueForwardOnlyPass;

		private DrawObjectsPass m_RenderOpaqueForwardPass;

		private DrawObjectsWithRenderingLayersPass m_RenderOpaqueForwardWithRenderingLayersPass;

		private DrawSkyboxPass m_DrawSkyboxPass;

		private CopyDepthPass m_CopyDepthPass;

		private CopyColorPass m_CopyColorPass;

		private TransparentSettingsPass m_TransparentSettingsPass;

		private DrawObjectsPass m_RenderTransparentForwardPass;

		private InvokeOnRenderObjectCallbackPass m_OnRenderObjectCallbackPass;

		private FinalBlitPass m_FinalBlitPass;

		private CapturePass m_CapturePass;

		private XROcclusionMeshPass m_XROcclusionMeshPass;

		private CopyDepthPass m_XRCopyDepthPass;

		private XRDepthMotionPass m_XRDepthMotionPass;

		private DrawScreenSpaceUIPass m_DrawOffscreenUIPass;

		private DrawScreenSpaceUIPass m_DrawOverlayUIPass;

		private CopyColorPass m_HistoryRawColorCopyPass;

		private CopyDepthPass m_HistoryRawDepthCopyPass;

		private StencilCrossFadeRenderPass m_StencilCrossFadeRenderPass;

		internal RenderTargetBufferSystem m_ColorBufferSystem;

		internal RTHandle m_ActiveCameraColorAttachment;

		private RTHandle m_ColorFrontBuffer;

		internal RTHandle m_ActiveCameraDepthAttachment;

		internal RTHandle m_CameraDepthAttachment;

		internal RTHandle m_CameraDepthAttachment_D3d_11;

		private RTHandle m_TargetColorHandle;

		private RTHandle m_TargetDepthHandle;

		internal RTHandle m_DepthTexture;

		private RTHandle m_NormalsTexture;

		private RTHandle m_DecalLayersTexture;

		private RTHandle m_OpaqueColor;

		private RTHandle m_MotionVectorColor;

		private RTHandle m_MotionVectorDepth;

		private ForwardLights m_ForwardLights;

		private DeferredLights m_DeferredLights;

		private RenderingMode m_RenderingMode;

		private DepthPrimingMode m_DepthPrimingMode;

		private CopyDepthMode m_CopyDepthMode;

		private DepthFormat m_CameraDepthAttachmentFormat;

		private DepthFormat m_CameraDepthTextureFormat;

		private bool m_DepthPrimingRecommended;

		private StencilState m_DefaultStencilState;

		private LightCookieManager m_LightCookieManager;

		private IntermediateTextureMode m_IntermediateTextureMode;

		private bool m_VulkanEnablePreTransform;

		private Material m_BlitMaterial;

		private Material m_BlitHDRMaterial;

		private Material m_SamplingMaterial;

		private Material m_StencilDeferredMaterial;

		private Material m_ClusterDeferredMaterial;

		private Material m_CameraMotionVecMaterial;

		private PostProcessPasses m_PostProcessPasses;

		private Material m_DebugBlitMaterial = Blitter.GetBlitMaterial(TextureXR.dimension, false);

		private static RTHandle[] m_RenderGraphCameraColorHandles = new RTHandle[2];

		private static RTHandle m_RenderGraphCameraDepthHandle;

		private static int m_CurrentColorHandle = 0;

		private static RTHandle m_RenderGraphDebugTextureHandle;

		private bool m_RequiresRenderingLayer;

		private RenderingLayerUtils.Event m_RenderingLayersEvent;

		private RenderingLayerUtils.MaskSize m_RenderingLayersMaskSize;

		private bool m_RenderingLayerProvidesRenderObjectPass;

		private bool m_RenderingLayerProvidesByDepthNormalPass;

		private string m_RenderingLayersTextureName;

		private const string _CameraTargetAttachmentAName = "_CameraTargetAttachmentA";

		private const string _CameraTargetAttachmentBName = "_CameraTargetAttachmentB";

		private const string _SingleCameraTargetAttachmentName = "_CameraTargetAttachment";

		private const string _CameraDepthAttachmentName = "_CameraDepthAttachment";

		private const string _CameraColorUpscaled = "_CameraColorUpscaled";

		private const string _CameraColorAfterPostProcessingName = "_CameraColorAfterPostProcessing";

		private bool m_IssuedGPUOcclusionUnsupportedMsg;

		private static bool m_RequiresIntermediateAttachments;

		private static class Profiling
		{
			private const string k_Name = "UniversalRenderer";

			public static readonly ProfilingSampler createCameraRenderTarget = new ProfilingSampler("UniversalRenderer.CreateCameraRenderTarget");
		}

		private struct RenderPassInputSummary
		{
			internal bool requiresDepthTexture;

			internal bool requiresDepthPrepass;

			internal bool requiresNormalsTexture;

			internal bool requiresColorTexture;

			internal bool requiresMotionVectors;

			internal RenderPassEvent requiresDepthNormalAtEvent;

			internal RenderPassEvent requiresDepthTextureEarliestEvent;
		}

		private class CopyToDebugTexturePassData
		{
			internal TextureHandle src;

			internal TextureHandle dest;
		}

		private readonly struct ClearCameraParams
		{
			internal ClearCameraParams(bool clearColor, bool clearDepth, Color clearVal)
			{
				this.mustClearColor = clearColor;
				this.mustClearDepth = clearDepth;
				this.clearValue = clearVal;
			}

			internal readonly bool mustClearColor;

			internal readonly bool mustClearDepth;

			internal readonly Color clearValue;
		}

		private enum OccluderPass
		{
			None,
			DepthPrepass,
			ForwardOpaque,
			GBuffer
		}

		private enum DepthCopySchedule
		{
			DuringPrepass,
			AfterPrepass,
			AfterGBuffer,
			AfterOpaques,
			AfterSkybox,
			AfterTransparents,
			None
		}

		private enum ColorCopySchedule
		{
			AfterSkybox,
			None
		}

		private struct TextureCopySchedules
		{
			internal UniversalRenderer.DepthCopySchedule depth;

			internal UniversalRenderer.ColorCopySchedule color;
		}
	}
}
