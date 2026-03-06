using System;
using System.Diagnostics;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderer(typeof(UniversalRendererData))]
	[DisallowMultipleRendererFeature("Decal")]
	[Tooltip("With this Renderer Feature, Unity can project specific Materials (decals) onto other objects in the Scene.")]
	public class DecalRendererFeature : ScriptableRendererFeature
	{
		private static SharedDecalEntityManager sharedDecalEntityManager { get; } = new SharedDecalEntityManager();

		internal ref DecalSettings settings
		{
			get
			{
				return ref this.m_Settings;
			}
		}

		internal bool intermediateRendering
		{
			get
			{
				return this.m_Technique == DecalTechnique.DBuffer;
			}
		}

		internal bool requiresDecalLayers
		{
			get
			{
				return this.m_Settings.decalLayers;
			}
		}

		internal static bool isGLDevice
		{
			get
			{
				return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore;
			}
		}

		public override void Create()
		{
			this.m_DecalPreviewPass = new DecalPreviewPass();
			this.m_RecreateSystems = true;
		}

		internal override bool RequireRenderingLayers(bool isDeferred, bool needsGBufferAccurateNormals, out RenderingLayerUtils.Event atEvent, out RenderingLayerUtils.MaskSize maskSize)
		{
			bool isPlaying = Application.isPlaying;
			DecalTechnique technique = this.GetTechnique(isDeferred, needsGBufferAccurateNormals, isPlaying);
			atEvent = ((technique == DecalTechnique.DBuffer) ? RenderingLayerUtils.Event.DepthNormalPrePass : RenderingLayerUtils.Event.Opaque);
			maskSize = RenderingLayerUtils.MaskSize.Bits8;
			return this.requiresDecalLayers;
		}

		internal DBufferSettings GetDBufferSettings()
		{
			if (this.m_Settings.technique == DecalTechniqueOption.Automatic)
			{
				return new DBufferSettings
				{
					surfaceData = DecalSurfaceData.AlbedoNormalMAOS
				};
			}
			return this.m_Settings.dBufferSettings;
		}

		internal DecalScreenSpaceSettings GetScreenSpaceSettings()
		{
			if (this.m_Settings.technique == DecalTechniqueOption.Automatic)
			{
				return new DecalScreenSpaceSettings
				{
					normalBlend = DecalNormalBlend.Low
				};
			}
			return this.m_Settings.screenSpaceSettings;
		}

		internal DecalTechnique GetTechnique(ScriptableRendererData renderer)
		{
			UniversalRendererData universalRendererData = renderer as UniversalRendererData;
			if (universalRendererData == null)
			{
				Debug.LogError("Only universal renderer supports Decal renderer feature.");
				return DecalTechnique.Invalid;
			}
			bool flag = universalRendererData.renderingMode == RenderingMode.Deferred;
			flag |= (universalRendererData.renderingMode == RenderingMode.DeferredPlus);
			return this.GetTechnique(flag, universalRendererData.accurateGbufferNormals, true);
		}

		internal DecalTechnique GetTechnique(ScriptableRenderer renderer)
		{
			UniversalRenderer universalRenderer = renderer as UniversalRenderer;
			if (universalRenderer == null)
			{
				Debug.LogError("Only universal renderer supports Decal renderer feature.");
				return DecalTechnique.Invalid;
			}
			return this.GetTechnique(universalRenderer.usesDeferredLighting, universalRenderer.accurateGbufferNormals, true);
		}

		internal DecalTechnique GetTechnique(bool isDeferred, bool needsGBufferAccurateNormals, bool checkForInvalidTechniques = true)
		{
			DecalTechnique decalTechnique = DecalTechnique.Invalid;
			switch (this.m_Settings.technique)
			{
			case DecalTechniqueOption.Automatic:
				if (DecalRendererFeature.isGLDevice)
				{
					decalTechnique = (isDeferred ? DecalTechnique.GBuffer : DecalTechnique.ScreenSpace);
				}
				else if (this.IsAutomaticDBuffer() || (isDeferred && needsGBufferAccurateNormals))
				{
					decalTechnique = DecalTechnique.DBuffer;
				}
				else if (isDeferred)
				{
					decalTechnique = DecalTechnique.GBuffer;
				}
				else
				{
					decalTechnique = DecalTechnique.ScreenSpace;
				}
				break;
			case DecalTechniqueOption.DBuffer:
				decalTechnique = DecalTechnique.DBuffer;
				break;
			case DecalTechniqueOption.ScreenSpace:
				if (isDeferred)
				{
					decalTechnique = DecalTechnique.GBuffer;
				}
				else
				{
					decalTechnique = DecalTechnique.ScreenSpace;
				}
				break;
			}
			if (!checkForInvalidTechniques)
			{
				return decalTechnique;
			}
			if (decalTechnique == DecalTechnique.DBuffer && DecalRendererFeature.isGLDevice)
			{
				Debug.LogError("Decal DBuffer technique is not supported with OpenGL.");
				return DecalTechnique.Invalid;
			}
			bool flag = SystemInfo.supportedRenderTargetCount >= 4;
			if (decalTechnique == DecalTechnique.DBuffer && !flag)
			{
				Debug.LogError("Decal DBuffer technique requires MRT4 support.");
				return DecalTechnique.Invalid;
			}
			if (decalTechnique == DecalTechnique.GBuffer && !flag)
			{
				Debug.LogError("Decal useGBuffer option requires MRT4 support.");
				return DecalTechnique.Invalid;
			}
			return decalTechnique;
		}

		private bool IsAutomaticDBuffer()
		{
			return Application.platform != RuntimePlatform.WebGLPlayer && !PlatformAutoDetect.isShaderAPIMobileDefined;
		}

		private bool RecreateSystemsIfNeeded(ScriptableRenderer renderer, in CameraData cameraData)
		{
			if (!this.m_RecreateSystems)
			{
				return true;
			}
			this.m_Technique = this.GetTechnique(renderer);
			if (this.m_Technique == DecalTechnique.Invalid)
			{
				return false;
			}
			this.m_DBufferSettings = this.GetDBufferSettings();
			this.m_ScreenSpaceSettings = this.GetScreenSpaceSettings();
			UniversalRendererResources renderPipelineSettings = GraphicsSettings.GetRenderPipelineSettings<UniversalRendererResources>();
			if (renderPipelineSettings == null)
			{
				return false;
			}
			this.m_DBufferClearMaterial = CoreUtils.CreateEngineMaterial(renderPipelineSettings.decalDBufferClear);
			if (this.m_DecalEntityManager == null)
			{
				this.m_DecalEntityManager = DecalRendererFeature.sharedDecalEntityManager.Get();
			}
			this.m_DecalUpdateCachedSystem = new DecalUpdateCachedSystem(this.m_DecalEntityManager);
			this.m_DecalUpdateCulledSystem = new DecalUpdateCulledSystem(this.m_DecalEntityManager);
			this.m_DecalCreateDrawCallSystem = new DecalCreateDrawCallSystem(this.m_DecalEntityManager, this.m_Settings.maxDrawDistance);
			if (this.intermediateRendering)
			{
				this.m_DecalUpdateCullingGroupSystem = new DecalUpdateCullingGroupSystem(this.m_DecalEntityManager, this.m_Settings.maxDrawDistance);
			}
			else
			{
				this.m_DecalSkipCulledSystem = new DecalSkipCulledSystem(this.m_DecalEntityManager);
			}
			this.m_DrawErrorSystem = new DecalDrawErrorSystem(this.m_DecalEntityManager, this.m_Technique);
			UniversalRenderer universalRenderer = renderer as UniversalRenderer;
			switch (this.m_Technique)
			{
			case DecalTechnique.DBuffer:
				this.m_CopyDepthPass = new DBufferCopyDepthPass((RenderPassEvent)201, renderPipelineSettings.copyDepthPS, false, !universalRenderer.usesDeferredLighting, false);
				this.m_DecalDrawDBufferSystem = new DecalDrawDBufferSystem(this.m_DecalEntityManager);
				this.m_DBufferRenderPass = new DBufferRenderPass(this.m_DBufferClearMaterial, this.m_DBufferSettings, this.m_DecalDrawDBufferSystem, this.m_Settings.decalLayers);
				this.m_DecalDrawForwardEmissiveSystem = new DecalDrawFowardEmissiveSystem(this.m_DecalEntityManager);
				this.m_ForwardEmissivePass = new DecalForwardEmissivePass(this.m_DecalDrawForwardEmissiveSystem);
				break;
			case DecalTechnique.ScreenSpace:
				this.m_DecalDrawScreenSpaceSystem = new DecalDrawScreenSpaceSystem(this.m_DecalEntityManager);
				this.m_ScreenSpaceDecalRenderPass = new DecalScreenSpaceRenderPass(this.m_ScreenSpaceSettings, this.intermediateRendering ? this.m_DecalDrawScreenSpaceSystem : null, this.m_Settings.decalLayers);
				break;
			case DecalTechnique.GBuffer:
				this.m_DeferredLights = universalRenderer.deferredLights;
				this.m_DrawGBufferSystem = new DecalDrawGBufferSystem(this.m_DecalEntityManager);
				this.m_GBufferRenderPass = new DecalGBufferRenderPass(this.m_ScreenSpaceSettings, this.intermediateRendering ? this.m_DrawGBufferSystem : null, this.m_Settings.decalLayers);
				break;
			}
			this.m_RecreateSystems = false;
			return true;
		}

		public unsafe override void OnCameraPreCull(ScriptableRenderer renderer, in CameraData cameraData)
		{
			CameraData cameraData2 = cameraData;
			if (*cameraData2.cameraType == CameraType.Preview)
			{
				return;
			}
			if (!this.RecreateSystemsIfNeeded(renderer, cameraData))
			{
				return;
			}
			this.m_DecalEntityManager.Update();
			this.m_DecalUpdateCachedSystem.Execute();
			if (this.intermediateRendering)
			{
				DecalUpdateCullingGroupSystem decalUpdateCullingGroupSystem = this.m_DecalUpdateCullingGroupSystem;
				cameraData2 = cameraData;
				decalUpdateCullingGroupSystem.Execute(*cameraData2.camera);
			}
			else
			{
				DecalSkipCulledSystem decalSkipCulledSystem = this.m_DecalSkipCulledSystem;
				cameraData2 = cameraData;
				decalSkipCulledSystem.Execute(*cameraData2.camera);
				this.m_DecalCreateDrawCallSystem.Execute();
				if (this.m_Technique == DecalTechnique.ScreenSpace)
				{
					this.m_DecalDrawScreenSpaceSystem.Execute(cameraData);
				}
				else if (this.m_Technique == DecalTechnique.GBuffer)
				{
					this.m_DrawGBufferSystem.Execute(cameraData);
				}
			}
			this.m_DrawErrorSystem.Execute(cameraData);
		}

		public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
			{
				return;
			}
			if (*renderingData.cameraData.cameraType == CameraType.Preview)
			{
				renderer.EnqueuePass(this.m_DecalPreviewPass);
				return;
			}
			if (!this.RecreateSystemsIfNeeded(renderer, renderingData.cameraData))
			{
				return;
			}
			if (this.intermediateRendering)
			{
				this.m_DecalUpdateCulledSystem.Execute();
				this.m_DecalCreateDrawCallSystem.Execute();
			}
			if (this.m_Technique == DecalTechnique.DBuffer)
			{
				if ((renderer as UniversalRenderer).usesDeferredLighting)
				{
					this.m_CopyDepthPass.CopyToDepth = false;
				}
				else
				{
					this.m_CopyDepthPass.CopyToDepth = true;
					this.m_CopyDepthPass.MsaaSamples = 1;
				}
			}
			switch (this.m_Technique)
			{
			case DecalTechnique.DBuffer:
				renderer.EnqueuePass(this.m_CopyDepthPass);
				renderer.EnqueuePass(this.m_DBufferRenderPass);
				renderer.EnqueuePass(this.m_ForwardEmissivePass);
				return;
			case DecalTechnique.ScreenSpace:
				renderer.EnqueuePass(this.m_ScreenSpaceDecalRenderPass);
				return;
			case DecalTechnique.GBuffer:
				this.m_GBufferRenderPass.Setup(this.m_DeferredLights);
				renderer.EnqueuePass(this.m_GBufferRenderPass);
				return;
			default:
				return;
			}
		}

		internal override bool SupportsNativeRenderPass()
		{
			return this.m_Technique == DecalTechnique.GBuffer || this.m_Technique == DecalTechnique.ScreenSpace;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
		{
			if (renderer.cameraColorTargetHandle == null)
			{
				return;
			}
			if (this.m_Technique != DecalTechnique.DBuffer)
			{
				if (this.m_Technique == DecalTechnique.GBuffer && this.m_DeferredLights.UseFramebufferFetch)
				{
					ScriptableRenderPass gbufferRenderPass = this.m_GBufferRenderPass;
					CommandBuffer cmd = null;
					CameraData cameraData = renderingData.cameraData;
					gbufferRenderPass.Configure(cmd, *cameraData.cameraTargetDescriptor);
				}
				return;
			}
			this.m_DBufferRenderPass.Setup(renderingData.cameraData);
			UniversalRenderer universalRenderer = renderer as UniversalRenderer;
			if (universalRenderer.usesDeferredLighting)
			{
				this.m_DBufferRenderPass.Setup(renderingData.cameraData, renderer.cameraDepthTargetHandle);
				this.m_CopyDepthPass.Setup(renderer.cameraDepthTargetHandle, universalRenderer.m_DepthTexture);
				return;
			}
			this.m_DBufferRenderPass.Setup(renderingData.cameraData);
			this.m_CopyDepthPass.Setup(universalRenderer.m_DepthTexture, this.m_DBufferRenderPass.dBufferDepth);
			this.m_CopyDepthPass.CopyToDepth = true;
			this.m_CopyDepthPass.MsaaSamples = 1;
		}

		protected override void Dispose(bool disposing)
		{
			DBufferRenderPass dbufferRenderPass = this.m_DBufferRenderPass;
			if (dbufferRenderPass != null)
			{
				dbufferRenderPass.Dispose();
			}
			DBufferCopyDepthPass copyDepthPass = this.m_CopyDepthPass;
			if (copyDepthPass != null)
			{
				copyDepthPass.Dispose();
			}
			CoreUtils.Destroy(this.m_DBufferClearMaterial);
			if (this.m_DecalEntityManager != null)
			{
				this.m_DecalEntityManager = null;
				DecalRendererFeature.sharedDecalEntityManager.Release(this.m_DecalEntityManager);
			}
		}

		[Conditional("ADAPTIVE_PERFORMANCE_4_0_0_OR_NEWER")]
		private void ChangeAdaptivePerformanceDrawDistances()
		{
		}

		[SerializeField]
		private DecalSettings m_Settings = new DecalSettings();

		private DecalTechnique m_Technique;

		private DBufferSettings m_DBufferSettings;

		private DecalScreenSpaceSettings m_ScreenSpaceSettings;

		private bool m_RecreateSystems;

		private DecalPreviewPass m_DecalPreviewPass;

		private DecalEntityManager m_DecalEntityManager;

		private DecalUpdateCachedSystem m_DecalUpdateCachedSystem;

		private DecalUpdateCullingGroupSystem m_DecalUpdateCullingGroupSystem;

		private DecalUpdateCulledSystem m_DecalUpdateCulledSystem;

		private DecalCreateDrawCallSystem m_DecalCreateDrawCallSystem;

		private DecalDrawErrorSystem m_DrawErrorSystem;

		private DBufferCopyDepthPass m_CopyDepthPass;

		private DBufferRenderPass m_DBufferRenderPass;

		private DecalForwardEmissivePass m_ForwardEmissivePass;

		private DecalDrawDBufferSystem m_DecalDrawDBufferSystem;

		private DecalDrawFowardEmissiveSystem m_DecalDrawForwardEmissiveSystem;

		private Material m_DBufferClearMaterial;

		private DecalScreenSpaceRenderPass m_ScreenSpaceDecalRenderPass;

		private DecalDrawScreenSpaceSystem m_DecalDrawScreenSpaceSystem;

		private DecalSkipCulledSystem m_DecalSkipCulledSystem;

		private DecalGBufferRenderPass m_GBufferRenderPass;

		private DecalDrawGBufferSystem m_DrawGBufferSystem;

		private DeferredLights m_DeferredLights;
	}
}
