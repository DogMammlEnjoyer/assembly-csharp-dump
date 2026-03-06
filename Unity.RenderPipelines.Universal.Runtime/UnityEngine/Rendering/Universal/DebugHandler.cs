using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DebugHandler : IDebugDisplaySettingsQuery
	{
		private DebugDisplaySettingsLighting LightingSettings
		{
			get
			{
				return this.m_DebugDisplaySettings.lightingSettings;
			}
		}

		private DebugDisplaySettingsMaterial MaterialSettings
		{
			get
			{
				return this.m_DebugDisplaySettings.materialSettings;
			}
		}

		private DebugDisplaySettingsRendering RenderingSettings
		{
			get
			{
				return this.m_DebugDisplaySettings.renderingSettings;
			}
		}

		public bool AreAnySettingsActive
		{
			get
			{
				return this.m_DebugDisplaySettings.AreAnySettingsActive;
			}
		}

		public bool IsPostProcessingAllowed
		{
			get
			{
				return this.m_DebugDisplaySettings.IsPostProcessingAllowed;
			}
		}

		public bool IsLightingActive
		{
			get
			{
				return this.m_DebugDisplaySettings.IsLightingActive;
			}
		}

		internal bool IsActiveModeUnsupportedForDeferred
		{
			get
			{
				return this.m_DebugDisplaySettings.lightingSettings.lightingDebugMode != DebugLightingMode.None || this.m_DebugDisplaySettings.lightingSettings.lightingFeatureFlags != DebugLightingFeatureFlags.None || this.m_DebugDisplaySettings.renderingSettings.sceneOverrideMode != DebugSceneOverrideMode.None || this.m_DebugDisplaySettings.materialSettings.materialDebugMode != DebugMaterialMode.None || this.m_DebugDisplaySettings.materialSettings.vertexAttributeDebugMode != DebugVertexAttributeMode.None || this.m_DebugDisplaySettings.materialSettings.materialValidationMode != DebugMaterialValidationMode.None || this.m_DebugDisplaySettings.renderingSettings.mipInfoMode > DebugMipInfoMode.None;
			}
		}

		public bool TryGetScreenClearColor(ref Color color)
		{
			return this.m_DebugDisplaySettings.TryGetScreenClearColor(ref color);
		}

		internal Material ReplacementMaterial
		{
			get
			{
				return this.m_ReplacementMaterial;
			}
		}

		internal UniversalRenderPipelineDebugDisplaySettings DebugDisplaySettings
		{
			get
			{
				return this.m_DebugDisplaySettings;
			}
		}

		internal ref RTHandle DebugScreenColorHandle
		{
			get
			{
				return ref this.m_DebugScreenColorHandle;
			}
		}

		internal ref RTHandle DebugScreenDepthHandle
		{
			get
			{
				return ref this.m_DebugScreenDepthHandle;
			}
		}

		internal HDRDebugViewPass hdrDebugViewPass
		{
			get
			{
				return this.m_HDRDebugViewPass;
			}
		}

		internal bool HDRDebugViewIsActive(bool resolveFinalTarget)
		{
			return this.DebugDisplaySettings.lightingSettings.hdrDebugMode > HDRDebugMode.None && resolveFinalTarget;
		}

		internal bool WriteToDebugScreenTexture(bool resolveFinalTarget)
		{
			return this.HDRDebugViewIsActive(resolveFinalTarget);
		}

		internal bool IsScreenClearNeeded
		{
			get
			{
				Color black = Color.black;
				return this.TryGetScreenClearColor(ref black);
			}
		}

		internal bool IsRenderPassSupported
		{
			get
			{
				return this.RenderingSettings.sceneOverrideMode == DebugSceneOverrideMode.None || this.RenderingSettings.sceneOverrideMode == DebugSceneOverrideMode.Overdraw;
			}
		}

		internal bool IsDepthPrimingCompatible
		{
			get
			{
				return this.RenderingSettings.sceneOverrideMode != DebugSceneOverrideMode.Wireframe;
			}
		}

		internal int stpDebugViewIndex
		{
			get
			{
				return this.RenderingSettings.stpDebugViewIndex;
			}
		}

		internal DebugHandler()
		{
			this.m_DebugDisplaySettings = DebugDisplaySettings<UniversalRenderPipelineDebugDisplaySettings>.Instance;
			UniversalRenderPipelineDebugShaders universalRenderPipelineDebugShaders;
			if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineDebugShaders>(out universalRenderPipelineDebugShaders))
			{
				this.m_ReplacementMaterial = ((universalRenderPipelineDebugShaders.debugReplacementPS != null) ? CoreUtils.CreateEngineMaterial(universalRenderPipelineDebugShaders.debugReplacementPS) : null);
				this.m_HDRDebugViewMaterial = ((universalRenderPipelineDebugShaders.hdrDebugViewPS != null) ? CoreUtils.CreateEngineMaterial(universalRenderPipelineDebugShaders.hdrDebugViewPS) : null);
			}
			this.m_HDRDebugViewPass = new HDRDebugViewPass(this.m_HDRDebugViewMaterial);
			this.m_RuntimeTextures = GraphicsSettings.GetRenderPipelineSettings<UniversalRenderPipelineRuntimeTextures>();
			if (this.m_RuntimeTextures != null)
			{
				this.m_DebugFontTexture = RTHandles.Alloc(this.m_RuntimeTextures.debugFontTexture);
			}
			this.m_debugDisplayConstant = new GraphicsBuffer(GraphicsBuffer.Target.Constant, 32, Marshal.SizeOf(typeof(Vector4)));
		}

		public void Dispose()
		{
			this.m_HDRDebugViewPass.Dispose();
			RTHandle debugScreenColorHandle = this.m_DebugScreenColorHandle;
			if (debugScreenColorHandle != null)
			{
				debugScreenColorHandle.Release();
			}
			RTHandle debugScreenDepthHandle = this.m_DebugScreenDepthHandle;
			if (debugScreenDepthHandle != null)
			{
				debugScreenDepthHandle.Release();
			}
			RTHandle debugFontTexture = this.m_DebugFontTexture;
			if (debugFontTexture != null)
			{
				debugFontTexture.Release();
			}
			this.m_debugDisplayConstant.Dispose();
			CoreUtils.Destroy(this.m_HDRDebugViewMaterial);
			CoreUtils.Destroy(this.m_ReplacementMaterial);
		}

		internal bool IsActiveForCamera(bool isPreviewCamera)
		{
			return !isPreviewCamera && this.AreAnySettingsActive;
		}

		internal bool TryGetFullscreenDebugMode(out DebugFullScreenMode debugFullScreenMode)
		{
			int num;
			return this.TryGetFullscreenDebugMode(out debugFullScreenMode, out num);
		}

		internal bool TryGetFullscreenDebugMode(out DebugFullScreenMode debugFullScreenMode, out int textureHeightPercent)
		{
			debugFullScreenMode = this.RenderingSettings.fullScreenDebugMode;
			textureHeightPercent = this.RenderingSettings.fullScreenDebugModeOutputSizeScreenPercent;
			return debugFullScreenMode > DebugFullScreenMode.None;
		}

		internal static void ConfigureColorDescriptorForDebugScreen(ref RenderTextureDescriptor descriptor, int cameraWidth, int cameraHeight)
		{
			descriptor.width = cameraWidth;
			descriptor.height = cameraHeight;
			descriptor.useMipMap = false;
			descriptor.autoGenerateMips = false;
			descriptor.useDynamicScale = true;
			descriptor.depthStencilFormat = GraphicsFormat.None;
		}

		internal static void ConfigureDepthDescriptorForDebugScreen(ref RenderTextureDescriptor descriptor, GraphicsFormat depthStencilFormat, int cameraWidth, int cameraHeight)
		{
			descriptor.width = cameraWidth;
			descriptor.height = cameraHeight;
			descriptor.useMipMap = false;
			descriptor.autoGenerateMips = false;
			descriptor.useDynamicScale = true;
			descriptor.depthStencilFormat = depthStencilFormat;
			descriptor.graphicsFormat = GraphicsFormat.None;
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal void SetupShaderProperties(RasterCommandBuffer cmd, int passIndex = 0)
		{
			if (this.LightingSettings.lightingDebugMode == DebugLightingMode.ShadowCascades)
			{
				cmd.EnableShaderKeyword("_DEBUG_ENVIRONMENTREFLECTIONS_OFF");
			}
			else
			{
				cmd.DisableShaderKeyword("_DEBUG_ENVIRONMENTREFLECTIONS_OFF");
			}
			this.m_debugDisplayConstant.SetData(this.MaterialSettings.debugRenderingLayersColors, 0, 0, 32);
			cmd.SetGlobalConstantBuffer(this.m_debugDisplayConstant, "_DebugDisplayConstant", 0, this.m_debugDisplayConstant.count * this.m_debugDisplayConstant.stride);
			if (this.MaterialSettings.renderingLayersSelectedLight)
			{
				cmd.SetGlobalInt("_DebugRenderingLayerMask", (int)this.MaterialSettings.GetDebugLightLayersMask());
			}
			else
			{
				cmd.SetGlobalInt("_DebugRenderingLayerMask", (int)this.MaterialSettings.renderingLayerMask);
			}
			switch (this.RenderingSettings.sceneOverrideMode)
			{
			case DebugSceneOverrideMode.Overdraw:
			{
				float num = 1f / (float)this.RenderingSettings.maxOverdrawCount;
				cmd.SetGlobalColor(DebugHandler.k_DebugColorPropertyId, new Color(num, num, num, 1f));
				break;
			}
			case DebugSceneOverrideMode.Wireframe:
				cmd.SetGlobalColor(DebugHandler.k_DebugColorPropertyId, Color.black);
				break;
			case DebugSceneOverrideMode.SolidWireframe:
				cmd.SetGlobalColor(DebugHandler.k_DebugColorPropertyId, (passIndex == 0) ? Color.white : Color.black);
				break;
			case DebugSceneOverrideMode.ShadedWireframe:
				if (passIndex == 0)
				{
					cmd.SetKeyword(ShaderGlobalKeywords.DEBUG_DISPLAY, false);
				}
				else if (passIndex == 1)
				{
					cmd.SetGlobalColor(DebugHandler.k_DebugColorPropertyId, Color.black);
					cmd.SetKeyword(ShaderGlobalKeywords.DEBUG_DISPLAY, true);
				}
				break;
			}
			DebugMaterialValidationMode materialValidationMode = this.MaterialSettings.materialValidationMode;
			if (materialValidationMode == DebugMaterialValidationMode.Albedo)
			{
				cmd.SetGlobalFloat(DebugHandler.k_DebugValidateAlbedoMinLuminanceId, this.MaterialSettings.albedoMinLuminance);
				cmd.SetGlobalFloat(DebugHandler.k_DebugValidateAlbedoMaxLuminanceId, this.MaterialSettings.albedoMaxLuminance);
				cmd.SetGlobalFloat(DebugHandler.k_DebugValidateAlbedoSaturationToleranceId, this.MaterialSettings.albedoSaturationTolerance);
				cmd.SetGlobalFloat(DebugHandler.k_DebugValidateAlbedoHueToleranceId, this.MaterialSettings.albedoHueTolerance);
				cmd.SetGlobalColor(DebugHandler.k_DebugValidateAlbedoCompareColorId, this.MaterialSettings.albedoCompareColor.linear);
				return;
			}
			if (materialValidationMode != DebugMaterialValidationMode.Metallic)
			{
				return;
			}
			cmd.SetGlobalFloat(DebugHandler.k_DebugValidateMetallicMinValueId, this.MaterialSettings.metallicMinValue);
			cmd.SetGlobalFloat(DebugHandler.k_DebugValidateMetallicMaxValueId, this.MaterialSettings.metallicMaxValue);
		}

		internal void SetDebugRenderTarget(RTHandle renderTarget, Rect displayRect, bool supportsStereo, Vector4 dataRangeRemap)
		{
			this.m_HasDebugRenderTarget = true;
			this.m_DebugRenderTargetSupportsStereo = supportsStereo;
			this.m_DebugRenderTarget = renderTarget;
			this.m_DebugRenderTargetPixelRect = new Vector4(displayRect.x, displayRect.y, displayRect.width, displayRect.height);
			this.m_DebugRenderTargetRangeRemap = dataRangeRemap;
		}

		internal void ResetDebugRenderTarget()
		{
			this.m_HasDebugRenderTarget = false;
		}

		private DebugHandler.DebugFinalValidationPassData InitDebugFinalValidationPassData(DebugHandler.DebugFinalValidationPassData passData, UniversalCameraData cameraData, bool isFinalPass)
		{
			passData.isFinalPass = isFinalPass;
			passData.resolveFinalTarget = cameraData.resolveFinalTarget;
			passData.isActiveForCamera = this.IsActiveForCamera(cameraData.isPreviewCamera);
			passData.hasDebugRenderTarget = this.m_HasDebugRenderTarget;
			passData.debugRenderTargetHandle = TextureHandle.nullHandle;
			passData.debugTexturePropertyId = (this.m_DebugRenderTargetSupportsStereo ? DebugHandler.k_DebugTexturePropertyId : DebugHandler.k_DebugTextureNoStereoPropertyId);
			passData.debugRenderTargetPixelRect = this.m_DebugRenderTargetPixelRect;
			passData.debugRenderTargetSupportsStereo = (this.m_DebugRenderTargetSupportsStereo ? 1 : 0);
			passData.debugRenderTargetRangeRemap = this.m_DebugRenderTargetRangeRemap;
			passData.debugFontTextureHandle = TextureHandle.nullHandle;
			passData.renderingSettings = this.RenderingSettings;
			return passData;
		}

		private static void UpdateShaderGlobalPropertiesForFinalValidationPass(RasterCommandBuffer cmd, DebugHandler.DebugFinalValidationPassData data)
		{
			if (!data.isFinalPass || !data.resolveFinalTarget)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.DEBUG_DISPLAY, false);
				return;
			}
			if (data.isActiveForCamera)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.DEBUG_DISPLAY, true);
			}
			else
			{
				cmd.SetKeyword(ShaderGlobalKeywords.DEBUG_DISPLAY, false);
			}
			if (data.hasDebugRenderTarget)
			{
				if (data.debugRenderTargetHandle.IsValid())
				{
					cmd.SetGlobalTexture(data.debugTexturePropertyId, data.debugRenderTargetHandle);
				}
				cmd.SetGlobalVector(DebugHandler.k_DebugTextureDisplayRect, data.debugRenderTargetPixelRect);
				cmd.SetGlobalInteger(DebugHandler.k_DebugRenderTargetSupportsStereo, data.debugRenderTargetSupportsStereo);
				cmd.SetGlobalVector(DebugHandler.k_DebugRenderTargetRangeRemap, data.debugRenderTargetRangeRemap);
			}
			DebugDisplaySettingsRendering renderingSettings = data.renderingSettings;
			if (renderingSettings.validationMode == DebugValidationMode.HighlightOutsideOfRange)
			{
				cmd.SetGlobalInteger(DebugHandler.k_ValidationChannelsId, (int)renderingSettings.validationChannels);
				cmd.SetGlobalFloat(DebugHandler.k_RangeMinimumId, renderingSettings.validationRangeMin);
				cmd.SetGlobalFloat(DebugHandler.k_RangeMaximumId, renderingSettings.validationRangeMax);
			}
			if (renderingSettings.mipInfoMode != DebugMipInfoMode.None)
			{
				cmd.SetGlobalTexture(DebugHandler.k_DebugFontId, data.debugFontTextureHandle);
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal void UpdateShaderGlobalPropertiesForFinalValidationPass(CommandBuffer cmd, UniversalCameraData cameraData, bool isFinalPass)
		{
			DebugHandler.UpdateShaderGlobalPropertiesForFinalValidationPass(CommandBufferHelpers.GetRasterCommandBuffer(cmd), this.InitDebugFinalValidationPassData(this.s_DebugFinalValidationPassData, cameraData, isFinalPass));
			cmd.SetGlobalTexture(this.s_DebugFinalValidationPassData.debugTexturePropertyId, this.m_DebugRenderTarget);
			if (this.RenderingSettings.mipInfoMode != DebugMipInfoMode.None)
			{
				cmd.SetGlobalTexture(DebugHandler.k_DebugFontId, this.m_RuntimeTextures.debugFontTexture);
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal void UpdateShaderGlobalPropertiesForFinalValidationPass(RenderGraph renderGraph, UniversalCameraData cameraData, bool isFinalPass)
		{
			DebugHandler.DebugFinalValidationPassData debugFinalValidationPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DebugHandler.DebugFinalValidationPassData>("UpdateShaderGlobalPropertiesForFinalValidationPass", out debugFinalValidationPassData, DebugHandler.s_DebugFinalValidationSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Debug\\DebugHandler.cs", 434))
			{
				this.InitDebugFinalValidationPassData(debugFinalValidationPassData, cameraData, isFinalPass);
				if (this.m_DebugRenderTarget != null)
				{
					debugFinalValidationPassData.debugRenderTargetHandle = renderGraph.ImportTexture(this.m_DebugRenderTarget);
				}
				if (this.m_DebugFontTexture != null)
				{
					debugFinalValidationPassData.debugFontTextureHandle = renderGraph.ImportTexture(this.m_DebugFontTexture);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (debugFinalValidationPassData.debugRenderTargetHandle.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(debugFinalValidationPassData.debugRenderTargetHandle, AccessFlags.Read);
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(debugFinalValidationPassData.debugRenderTargetHandle, debugFinalValidationPassData.debugTexturePropertyId);
				}
				if (debugFinalValidationPassData.debugFontTextureHandle.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(debugFinalValidationPassData.debugFontTextureHandle, AccessFlags.Read);
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(debugFinalValidationPassData.debugFontTextureHandle, DebugHandler.k_DebugFontId);
				}
				rasterRenderGraphBuilder.SetRenderFunc<DebugHandler.DebugFinalValidationPassData>(delegate(DebugHandler.DebugFinalValidationPassData data, RasterGraphContext context)
				{
					DebugHandler.UpdateShaderGlobalPropertiesForFinalValidationPass(context.cmd, data);
				});
			}
		}

		private DebugHandler.DebugSetupPassData InitDebugSetupPassData(DebugHandler.DebugSetupPassData passData, bool isPreviewCamera)
		{
			passData.isActiveForCamera = this.IsActiveForCamera(isPreviewCamera);
			passData.materialSettings = this.MaterialSettings;
			passData.renderingSettings = this.RenderingSettings;
			passData.lightingSettings = this.LightingSettings;
			return passData;
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private static void Setup(RasterCommandBuffer cmd, DebugHandler.DebugSetupPassData passData)
		{
			if (passData.isActiveForCamera)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.DEBUG_DISPLAY, true);
				cmd.SetGlobalFloat(DebugHandler.k_DebugMaterialModeId, (float)passData.materialSettings.materialDebugMode);
				cmd.SetGlobalFloat(DebugHandler.k_DebugVertexAttributeModeId, (float)passData.materialSettings.vertexAttributeDebugMode);
				cmd.SetGlobalInteger(DebugHandler.k_DebugMaterialValidationModeId, (int)passData.materialSettings.materialValidationMode);
				cmd.SetGlobalInteger(DebugHandler.k_DebugMipInfoModeId, (int)passData.renderingSettings.mipInfoMode);
				cmd.SetGlobalInteger(DebugHandler.k_DebugMipMapStatusModeId, (int)passData.renderingSettings.mipDebugStatusMode);
				cmd.SetGlobalInteger(DebugHandler.k_DebugMipMapShowStatusCodeId, passData.renderingSettings.mipDebugStatusShowCode ? 1 : 0);
				cmd.SetGlobalFloat(DebugHandler.k_DebugMipMapOpacityId, passData.renderingSettings.mipDebugOpacity);
				cmd.SetGlobalFloat(DebugHandler.k_DebugMipMapRecentlyUpdatedCooldownId, passData.renderingSettings.mipDebugRecentUpdateCooldown);
				cmd.SetGlobalFloat(DebugHandler.k_DebugMipMapTerrainTextureModeId, (float)passData.renderingSettings.mipDebugTerrainTexture);
				cmd.SetGlobalInteger(DebugHandler.k_DebugSceneOverrideModeId, (int)passData.renderingSettings.sceneOverrideMode);
				cmd.SetGlobalInteger(DebugHandler.k_DebugFullScreenModeId, (int)passData.renderingSettings.fullScreenDebugMode);
				cmd.SetGlobalInteger(DebugHandler.k_DebugMaxPixelCost, passData.renderingSettings.maxOverdrawCount);
				cmd.SetGlobalInteger(DebugHandler.k_DebugValidationModeId, (int)passData.renderingSettings.validationMode);
				cmd.SetGlobalColor(DebugHandler.k_DebugValidateBelowMinThresholdColorPropertyId, Color.red);
				cmd.SetGlobalColor(DebugHandler.k_DebugValidateAboveMaxThresholdColorPropertyId, Color.blue);
				cmd.SetGlobalFloat(DebugHandler.k_DebugLightingModeId, (float)passData.lightingSettings.lightingDebugMode);
				cmd.SetGlobalInteger(DebugHandler.k_DebugLightingFeatureFlagsId, (int)passData.lightingSettings.lightingFeatureFlags);
				cmd.SetGlobalColor(DebugHandler.k_DebugColorInvalidModePropertyId, Color.red);
				cmd.SetGlobalFloat(DebugHandler.k_DebugCurrentRealTimeId, Time.realtimeSinceStartup);
				return;
			}
			cmd.SetKeyword(ShaderGlobalKeywords.DEBUG_DISPLAY, false);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal void Setup(CommandBuffer cmd, bool isPreviewCamera)
		{
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal void Setup(RenderGraph renderGraph, bool isPreviewCamera)
		{
			DebugHandler.DebugSetupPassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DebugHandler.DebugSetupPassData>(DebugHandler.s_DebugSetupSampler.name, out passData, DebugHandler.s_DebugSetupSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Debug\\DebugHandler.cs", 540))
			{
				this.InitDebugSetupPassData(passData, isPreviewCamera);
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<DebugHandler.DebugSetupPassData>(delegate(DebugHandler.DebugSetupPassData data, RasterGraphContext context)
				{
				});
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal void Render(RenderGraph renderGraph, UniversalCameraData cameraData, TextureHandle srcColor, TextureHandle overlayTexture, TextureHandle dstColor)
		{
			if (this.IsActiveForCamera(cameraData.isPreviewCamera) && this.HDRDebugViewIsActive(cameraData.resolveFinalTarget))
			{
				this.m_HDRDebugViewPass.RenderHDRDebug(renderGraph, cameraData, srcColor, overlayTexture, dstColor, this.LightingSettings.hdrDebugMode);
			}
		}

		internal DebugRendererLists CreateRendererListsWithDebugRenderState(ScriptableRenderContext context, ref CullingResults cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock renderStateBlock)
		{
			DebugRendererLists debugRendererLists = new DebugRendererLists(this, filteringSettings);
			debugRendererLists.CreateRendererListsWithDebugRenderState(context, ref cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
			return debugRendererLists;
		}

		internal DebugRendererLists CreateRendererListsWithDebugRenderState(RenderGraph renderGraph, ref CullingResults cullResults, ref DrawingSettings drawingSettings, ref FilteringSettings filteringSettings, ref RenderStateBlock renderStateBlock)
		{
			DebugRendererLists debugRendererLists = new DebugRendererLists(this, filteringSettings);
			debugRendererLists.CreateRendererListsWithDebugRenderState(renderGraph, ref cullResults, ref drawingSettings, ref filteringSettings, ref renderStateBlock);
			return debugRendererLists;
		}

		private static readonly int k_DebugColorInvalidModePropertyId = Shader.PropertyToID("_DebugColorInvalidMode");

		private static readonly int k_DebugCurrentRealTimeId = Shader.PropertyToID("_DebugCurrentRealTime");

		private static readonly int k_DebugColorPropertyId = Shader.PropertyToID("_DebugColor");

		private static readonly int k_DebugTexturePropertyId = Shader.PropertyToID("_DebugTexture");

		private static readonly int k_DebugFontId = Shader.PropertyToID("_DebugFont");

		private static readonly int k_DebugTextureNoStereoPropertyId = Shader.PropertyToID("_DebugTextureNoStereo");

		private static readonly int k_DebugTextureDisplayRect = Shader.PropertyToID("_DebugTextureDisplayRect");

		private static readonly int k_DebugRenderTargetSupportsStereo = Shader.PropertyToID("_DebugRenderTargetSupportsStereo");

		private static readonly int k_DebugRenderTargetRangeRemap = Shader.PropertyToID("_DebugRenderTargetRangeRemap");

		private static readonly int k_DebugMaterialModeId = Shader.PropertyToID("_DebugMaterialMode");

		private static readonly int k_DebugVertexAttributeModeId = Shader.PropertyToID("_DebugVertexAttributeMode");

		private static readonly int k_DebugMaterialValidationModeId = Shader.PropertyToID("_DebugMaterialValidationMode");

		private static readonly int k_DebugMipInfoModeId = Shader.PropertyToID("_DebugMipInfoMode");

		private static readonly int k_DebugMipMapStatusModeId = Shader.PropertyToID("_DebugMipMapStatusMode");

		private static readonly int k_DebugMipMapShowStatusCodeId = Shader.PropertyToID("_DebugMipMapShowStatusCode");

		private static readonly int k_DebugMipMapOpacityId = Shader.PropertyToID("_DebugMipMapOpacity");

		private static readonly int k_DebugMipMapRecentlyUpdatedCooldownId = Shader.PropertyToID("_DebugMipMapRecentlyUpdatedCooldown");

		private static readonly int k_DebugMipMapTerrainTextureModeId = Shader.PropertyToID("_DebugMipMapTerrainTextureMode");

		private static readonly int k_DebugSceneOverrideModeId = Shader.PropertyToID("_DebugSceneOverrideMode");

		private static readonly int k_DebugFullScreenModeId = Shader.PropertyToID("_DebugFullScreenMode");

		private static readonly int k_DebugValidationModeId = Shader.PropertyToID("_DebugValidationMode");

		private static readonly int k_DebugValidateBelowMinThresholdColorPropertyId = Shader.PropertyToID("_DebugValidateBelowMinThresholdColor");

		private static readonly int k_DebugValidateAboveMaxThresholdColorPropertyId = Shader.PropertyToID("_DebugValidateAboveMaxThresholdColor");

		private static readonly int k_DebugMaxPixelCost = Shader.PropertyToID("_DebugMaxPixelCost");

		private static readonly int k_DebugLightingModeId = Shader.PropertyToID("_DebugLightingMode");

		private static readonly int k_DebugLightingFeatureFlagsId = Shader.PropertyToID("_DebugLightingFeatureFlags");

		private static readonly int k_DebugValidateAlbedoMinLuminanceId = Shader.PropertyToID("_DebugValidateAlbedoMinLuminance");

		private static readonly int k_DebugValidateAlbedoMaxLuminanceId = Shader.PropertyToID("_DebugValidateAlbedoMaxLuminance");

		private static readonly int k_DebugValidateAlbedoSaturationToleranceId = Shader.PropertyToID("_DebugValidateAlbedoSaturationTolerance");

		private static readonly int k_DebugValidateAlbedoHueToleranceId = Shader.PropertyToID("_DebugValidateAlbedoHueTolerance");

		private static readonly int k_DebugValidateAlbedoCompareColorId = Shader.PropertyToID("_DebugValidateAlbedoCompareColor");

		private static readonly int k_DebugValidateMetallicMinValueId = Shader.PropertyToID("_DebugValidateMetallicMinValue");

		private static readonly int k_DebugValidateMetallicMaxValueId = Shader.PropertyToID("_DebugValidateMetallicMaxValue");

		private static readonly int k_ValidationChannelsId = Shader.PropertyToID("_ValidationChannels");

		private static readonly int k_RangeMinimumId = Shader.PropertyToID("_RangeMinimum");

		private static readonly int k_RangeMaximumId = Shader.PropertyToID("_RangeMaximum");

		private static readonly ProfilingSampler s_DebugSetupSampler = new ProfilingSampler("Setup Debug Properties");

		private static readonly ProfilingSampler s_DebugFinalValidationSampler = new ProfilingSampler("UpdateShaderGlobalPropertiesForFinalValidationPass");

		private DebugHandler.DebugSetupPassData s_DebugSetupPassData = new DebugHandler.DebugSetupPassData();

		private DebugHandler.DebugFinalValidationPassData s_DebugFinalValidationPassData = new DebugHandler.DebugFinalValidationPassData();

		private readonly Material m_ReplacementMaterial;

		private readonly Material m_HDRDebugViewMaterial;

		private HDRDebugViewPass m_HDRDebugViewPass;

		private RTHandle m_DebugScreenColorHandle;

		private RTHandle m_DebugScreenDepthHandle;

		private readonly UniversalRenderPipelineRuntimeTextures m_RuntimeTextures;

		private bool m_HasDebugRenderTarget;

		private bool m_DebugRenderTargetSupportsStereo;

		private Vector4 m_DebugRenderTargetPixelRect;

		private Vector4 m_DebugRenderTargetRangeRemap;

		private RTHandle m_DebugRenderTarget;

		private RTHandle m_DebugFontTexture;

		private GraphicsBuffer m_debugDisplayConstant;

		private readonly UniversalRenderPipelineDebugDisplaySettings m_DebugDisplaySettings;

		private class DebugFinalValidationPassData
		{
			public bool isFinalPass;

			public bool resolveFinalTarget;

			public bool isActiveForCamera;

			public bool hasDebugRenderTarget;

			public TextureHandle debugRenderTargetHandle;

			public int debugTexturePropertyId;

			public Vector4 debugRenderTargetPixelRect;

			public int debugRenderTargetSupportsStereo;

			public Vector4 debugRenderTargetRangeRemap;

			public TextureHandle debugFontTextureHandle;

			public DebugDisplaySettingsRendering renderingSettings;
		}

		private class DebugSetupPassData
		{
			public bool isActiveForCamera;

			public DebugDisplaySettingsMaterial materialSettings;

			public DebugDisplaySettingsRendering renderingSettings;

			public DebugDisplaySettingsLighting lightingSettings;
		}
	}
}
