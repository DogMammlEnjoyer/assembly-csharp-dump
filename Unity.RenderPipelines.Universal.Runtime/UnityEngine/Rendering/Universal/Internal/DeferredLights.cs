using System;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	internal class DeferredLights
	{
		internal int GBufferAlbedoIndex
		{
			get
			{
				return 0;
			}
		}

		internal int GBufferSpecularMetallicIndex
		{
			get
			{
				return 1;
			}
		}

		internal int GBufferNormalSmoothnessIndex
		{
			get
			{
				return 2;
			}
		}

		internal int GBufferLightingIndex
		{
			get
			{
				return 3;
			}
		}

		internal int GbufferDepthIndex
		{
			get
			{
				if (!this.UseFramebufferFetch)
				{
					return -1;
				}
				return this.GBufferLightingIndex + 1;
			}
		}

		internal int GBufferRenderingLayers
		{
			get
			{
				if (!this.UseRenderingLayers)
				{
					return -1;
				}
				return this.GBufferLightingIndex + (this.UseFramebufferFetch ? 1 : 0) + 1;
			}
		}

		internal int GBufferShadowMask
		{
			get
			{
				if (!this.UseShadowMask)
				{
					return -1;
				}
				return this.GBufferLightingIndex + (this.UseFramebufferFetch ? 1 : 0) + (this.UseRenderingLayers ? 1 : 0) + 1;
			}
		}

		internal int GBufferSliceCount
		{
			get
			{
				return 4 + (this.UseFramebufferFetch ? 1 : 0) + (this.UseShadowMask ? 1 : 0) + (this.UseRenderingLayers ? 1 : 0);
			}
		}

		internal int GBufferInputAttachmentCount
		{
			get
			{
				return 4 + (this.UseShadowMask ? 1 : 0);
			}
		}

		internal GraphicsFormat GetGBufferFormat(int index)
		{
			if (index == this.GBufferAlbedoIndex)
			{
				if (QualitySettings.activeColorSpace != ColorSpace.Linear)
				{
					return GraphicsFormat.R8G8B8A8_UNorm;
				}
				return GraphicsFormat.R8G8B8A8_SRGB;
			}
			else
			{
				if (index == this.GBufferSpecularMetallicIndex)
				{
					return GraphicsFormat.R8G8B8A8_UNorm;
				}
				if (index == this.GBufferNormalSmoothnessIndex)
				{
					if (!this.AccurateGbufferNormals)
					{
						return DepthNormalOnlyPass.GetGraphicsFormat();
					}
					return GraphicsFormat.R8G8B8A8_UNorm;
				}
				else
				{
					if (index == this.GBufferLightingIndex)
					{
						return GraphicsFormat.None;
					}
					if (index == this.GbufferDepthIndex)
					{
						return GraphicsFormat.R32_SFloat;
					}
					if (index == this.GBufferShadowMask)
					{
						return GraphicsFormat.B8G8R8A8_UNorm;
					}
					if (index == this.GBufferRenderingLayers)
					{
						return RenderingLayerUtils.GetFormat(this.RenderingLayerMaskSize);
					}
					return GraphicsFormat.None;
				}
			}
		}

		internal bool UseShadowMask
		{
			get
			{
				return this.MixedLightingSetup > MixedLightingSetup.None;
			}
		}

		internal bool UseRenderingLayers
		{
			get
			{
				return this.UseLightLayers || this.UseDecalLayers;
			}
		}

		internal RenderingLayerUtils.MaskSize RenderingLayerMaskSize { get; set; }

		internal bool UseDecalLayers { get; set; }

		internal bool UseLightLayers
		{
			get
			{
				return UniversalRenderPipeline.asset.useRenderingLayers;
			}
		}

		internal bool UseFramebufferFetch { get; set; }

		internal bool HasDepthPrepass { get; set; }

		internal bool HasNormalPrepass { get; set; }

		internal bool HasRenderingLayerPrepass { get; set; }

		internal bool AccurateGbufferNormals { get; set; }

		internal MixedLightingSetup MixedLightingSetup { get; set; }

		internal bool UseJobSystem { get; set; }

		internal int RenderWidth { get; set; }

		internal int RenderHeight { get; set; }

		internal RTHandle[] GbufferAttachments { get; set; }

		internal TextureHandle[] GbufferTextureHandles { get; set; }

		internal RTHandle[] DeferredInputAttachments { get; set; }

		internal bool[] DeferredInputIsTransient { get; set; }

		internal RTHandle DepthAttachment { get; set; }

		internal RTHandle DepthCopyTexture { get; set; }

		internal GraphicsFormat[] GbufferFormats { get; set; }

		internal RTHandle DepthAttachmentHandle { get; set; }

		internal DeferredLights(DeferredLights.InitParams initParams, bool useNativeRenderPass = false)
		{
			DeferredConfig.IsOpenGL = (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3);
			DeferredConfig.IsDX10 = (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 && SystemInfo.graphicsShaderLevel <= 40);
			this.m_StencilDeferredMaterial = initParams.stencilDeferredMaterial;
			this.m_ClusterDeferredMaterial = initParams.clusterDeferredMaterial;
			if (initParams.deferredPlus)
			{
				this.m_ClusterDeferredPasses = new int[DeferredLights.k_ClusterDeferredPassNames.Length];
				this.InitClusterDeferredMaterial();
			}
			else
			{
				this.m_StencilDeferredPasses = new int[DeferredLights.k_StencilDeferredPassNames.Length];
				this.InitStencilDeferredMaterial();
			}
			this.AccurateGbufferNormals = true;
			this.UseJobSystem = true;
			this.UseFramebufferFetch = useNativeRenderPass;
			this.m_LightCookieManager = initParams.lightCookieManager;
			this.m_UseDeferredPlus = initParams.deferredPlus;
		}

		internal void SetupRenderGraphLights(RenderGraph renderGraph, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			DeferredLights.SetupLightPassData setupLightPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<DeferredLights.SetupLightPassData>(DeferredLights.s_SetupDeferredLights.name, out setupLightPassData, DeferredLights.s_SetupDeferredLights, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\DeferredLights.cs", 313))
			{
				setupLightPassData.cameraData = cameraData;
				setupLightPassData.cameraTargetSizeCopy = new Vector2Int(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
				setupLightPassData.lightData = lightData;
				setupLightPassData.deferredLights = this;
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				unsafeRenderGraphBuilder.SetRenderFunc<DeferredLights.SetupLightPassData>(delegate(DeferredLights.SetupLightPassData data, UnsafeGraphContext rgContext)
				{
					data.deferredLights.SetupLights(CommandBufferHelpers.GetNativeCommandBuffer(rgContext.cmd), data.cameraData, data.cameraTargetSizeCopy, data.lightData, true);
				});
			}
		}

		internal void SetupLights(CommandBuffer cmd, UniversalCameraData cameraData, Vector2Int cameraTargetSizeCopy, UniversalLightData lightData, bool isRenderGraph = false)
		{
			Camera camera = cameraData.camera;
			if (cameraData.xr.enabled)
			{
				this.RenderWidth = (camera.allowDynamicResolution ? cameraData.xr.renderTargetScaledWidth : cameraTargetSizeCopy.x);
				this.RenderHeight = (camera.allowDynamicResolution ? cameraData.xr.renderTargetScaledHeight : cameraTargetSizeCopy.y);
			}
			else
			{
				this.RenderWidth = (camera.allowDynamicResolution ? Mathf.CeilToInt(ScalableBufferManager.widthScaleFactor * (float)cameraTargetSizeCopy.x) : cameraTargetSizeCopy.x);
				this.RenderHeight = (camera.allowDynamicResolution ? Mathf.CeilToInt(ScalableBufferManager.heightScaleFactor * (float)cameraTargetSizeCopy.y) : cameraTargetSizeCopy.y);
			}
			if (!this.m_UseDeferredPlus)
			{
				this.PrecomputeLights(out this.m_stencilVisLights, out this.m_stencilVisLightOffsets, ref lightData.visibleLights, lightData.additionalLightsCount != 0 || lightData.mainLightIndex >= 0);
			}
			using (new ProfilingScope(cmd, DeferredLights.m_ProfilingSetupLightConstants))
			{
				if (!this.m_UseDeferredPlus)
				{
					this.SetupShaderLightConstants(cmd, lightData);
				}
				bool supportsMixedLighting = lightData.supportsMixedLighting;
				cmd.SetKeyword(ShaderGlobalKeywords._GBUFFER_NORMALS_OCT, this.AccurateGbufferNormals);
				bool flag = supportsMixedLighting && this.MixedLightingSetup == MixedLightingSetup.ShadowMask;
				bool flag2 = flag && QualitySettings.shadowmaskMode == ShadowmaskMode.Shadowmask;
				bool flag3 = supportsMixedLighting && this.MixedLightingSetup == MixedLightingSetup.Subtractive;
				cmd.SetKeyword(ShaderGlobalKeywords.LightmapShadowMixing, flag3 || flag2);
				cmd.SetKeyword(ShaderGlobalKeywords.ShadowsShadowMask, flag);
				cmd.SetKeyword(ShaderGlobalKeywords.MixedLightingSubtractive, flag3);
				cmd.SetKeyword(ShaderGlobalKeywords.RenderPassEnabled, this.UseFramebufferFetch && (cameraData.cameraType == CameraType.Game || camera.cameraType == CameraType.SceneView || isRenderGraph));
				cmd.SetKeyword(ShaderGlobalKeywords.LightLayers, this.UseLightLayers && !CoreUtils.IsSceneLightingDisabled(camera));
				RenderingLayerUtils.SetupProperties(cmd, this.RenderingLayerMaskSize);
			}
		}

		internal void ResolveMixedLightingMode(UniversalLightData lightData)
		{
			this.MixedLightingSetup = MixedLightingSetup.None;
			if (lightData.supportsMixedLighting)
			{
				NativeArray<VisibleLight> visibleLights = lightData.visibleLights;
				int num = 0;
				while (num < lightData.visibleLights.Length && this.MixedLightingSetup == MixedLightingSetup.None)
				{
					Light light = visibleLights.UnsafeElementAt(num).light;
					if (light != null && light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed && light.shadows != LightShadows.None)
					{
						MixedLightingMode mixedLightingMode = light.bakingOutput.mixedLightingMode;
						if (mixedLightingMode != MixedLightingMode.Subtractive)
						{
							if (mixedLightingMode == MixedLightingMode.Shadowmask)
							{
								this.MixedLightingSetup = MixedLightingSetup.ShadowMask;
							}
						}
						else
						{
							this.MixedLightingSetup = MixedLightingSetup.Subtractive;
						}
					}
					num++;
				}
			}
		}

		internal void DisableFramebufferFetchInput()
		{
			this.UseFramebufferFetch = false;
			this.CreateGbufferResources();
		}

		internal void ReleaseGbufferResources()
		{
			if (this.GbufferRTHandles != null)
			{
				for (int i = 0; i < this.GbufferRTHandles.Length; i++)
				{
					if (i != this.GBufferLightingIndex)
					{
						this.GbufferRTHandles[i].Release();
						this.GbufferAttachments[i].Release();
					}
				}
			}
		}

		internal void ReAllocateGBufferIfNeeded(RenderTextureDescriptor gbufferSlice, int gbufferIndex)
		{
			if (this.GbufferRTHandles != null)
			{
				if (this.GbufferRTHandles[gbufferIndex].GetInstanceID() != this.GbufferAttachments[gbufferIndex].GetInstanceID())
				{
					return;
				}
				gbufferSlice.depthStencilFormat = GraphicsFormat.None;
				gbufferSlice.stencilFormat = GraphicsFormat.None;
				gbufferSlice.graphicsFormat = this.GetGBufferFormat(gbufferIndex);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.GbufferRTHandles[gbufferIndex], gbufferSlice, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, DeferredLights.k_GBufferNames[gbufferIndex]);
				this.GbufferAttachments[gbufferIndex] = this.GbufferRTHandles[gbufferIndex];
			}
		}

		internal void CreateGbufferResources()
		{
			int gbufferSliceCount = this.GBufferSliceCount;
			if (this.GbufferRTHandles == null || this.GbufferRTHandles.Length != gbufferSliceCount)
			{
				this.ReleaseGbufferResources();
				this.GbufferAttachments = new RTHandle[gbufferSliceCount];
				this.GbufferRTHandles = new RTHandle[gbufferSliceCount];
				this.GbufferFormats = new GraphicsFormat[gbufferSliceCount];
				this.GbufferTextureHandles = new TextureHandle[gbufferSliceCount];
				for (int i = 0; i < gbufferSliceCount; i++)
				{
					this.GbufferRTHandles[i] = RTHandles.Alloc(DeferredLights.k_GBufferNames[i], DeferredLights.k_GBufferNames[i]);
					this.GbufferAttachments[i] = this.GbufferRTHandles[i];
					this.GbufferFormats[i] = this.GetGBufferFormat(i);
				}
			}
		}

		internal void CreateGbufferResourcesRenderGraph(RenderGraph renderGraph, UniversalResourceData resourceData)
		{
			int gbufferSliceCount = this.GBufferSliceCount;
			if (this.GbufferTextureHandles == null || this.GbufferTextureHandles.Length != gbufferSliceCount)
			{
				this.GbufferFormats = new GraphicsFormat[gbufferSliceCount];
				this.GbufferTextureHandles = new TextureHandle[gbufferSliceCount];
			}
			bool flag = this.UseRenderingLayers && !this.UseLightLayers;
			for (int i = 0; i < gbufferSliceCount; i++)
			{
				this.GbufferFormats[i] = this.GetGBufferFormat(i);
				if (i == this.GBufferNormalSmoothnessIndex && this.HasNormalPrepass)
				{
					this.GbufferTextureHandles[i] = resourceData.cameraNormalsTexture;
				}
				else if (i == this.GBufferRenderingLayers && flag)
				{
					this.GbufferTextureHandles[i] = resourceData.renderingLayersTexture;
				}
				else if (i != this.GBufferLightingIndex)
				{
					TextureDesc descriptor = resourceData.cameraColor.GetDescriptor(renderGraph);
					descriptor.format = this.GetGBufferFormat(i);
					descriptor.name = DeferredLights.k_GBufferNames[i];
					descriptor.clearBuffer = true;
					this.GbufferTextureHandles[i] = renderGraph.CreateTexture(descriptor);
				}
				else
				{
					this.GbufferTextureHandles[i] = resourceData.cameraColor;
				}
			}
		}

		internal void UpdateDeferredInputAttachments()
		{
			this.DeferredInputAttachments[0] = this.GbufferAttachments[0];
			this.DeferredInputAttachments[1] = this.GbufferAttachments[1];
			this.DeferredInputAttachments[2] = this.GbufferAttachments[2];
			this.DeferredInputAttachments[3] = this.GbufferAttachments[4];
			if (this.UseShadowMask && this.UseRenderingLayers)
			{
				this.DeferredInputAttachments[4] = this.GbufferAttachments[this.GBufferShadowMask];
				this.DeferredInputAttachments[5] = this.GbufferAttachments[this.GBufferRenderingLayers];
				return;
			}
			if (this.UseShadowMask)
			{
				this.DeferredInputAttachments[4] = this.GbufferAttachments[this.GBufferShadowMask];
				return;
			}
			if (this.UseRenderingLayers)
			{
				this.DeferredInputAttachments[4] = this.GbufferAttachments[this.GBufferRenderingLayers];
			}
		}

		internal bool IsRuntimeSupportedThisFrame()
		{
			return this.GBufferSliceCount <= SystemInfo.supportedRenderTargetCount && !DeferredConfig.IsOpenGL && !DeferredConfig.IsDX10;
		}

		public void Setup(AdditionalLightsShadowCasterPass additionalLightsShadowCasterPass, bool hasDepthPrepass, bool hasNormalPrepass, bool hasRenderingLayerPrepass, RTHandle depthCopyTexture, RTHandle depthAttachment, RTHandle colorAttachment)
		{
			this.m_AdditionalLightsShadowCasterPass = additionalLightsShadowCasterPass;
			this.HasDepthPrepass = hasDepthPrepass;
			this.HasNormalPrepass = hasNormalPrepass;
			this.HasRenderingLayerPrepass = hasRenderingLayerPrepass;
			this.DepthCopyTexture = depthCopyTexture;
			this.GbufferAttachments[this.GBufferLightingIndex] = colorAttachment;
			this.DepthAttachment = depthAttachment;
			int num = 4 + (this.UseShadowMask ? 1 : 0) + (this.UseRenderingLayers ? 1 : 0);
			if ((this.DeferredInputAttachments == null && this.UseFramebufferFetch && this.GbufferAttachments.Length >= 3) || (this.DeferredInputAttachments != null && num != this.DeferredInputAttachments.Length))
			{
				this.DeferredInputAttachments = new RTHandle[num];
				this.DeferredInputIsTransient = new bool[num];
				int num2 = 0;
				int i = 0;
				while (i < num)
				{
					if (num2 == this.GBufferLightingIndex)
					{
						num2++;
					}
					this.DeferredInputAttachments[i] = this.GbufferAttachments[num2];
					this.DeferredInputIsTransient[i] = (num2 != this.GbufferDepthIndex);
					i++;
					num2++;
				}
			}
			this.DepthAttachmentHandle = this.DepthAttachment;
		}

		internal void Setup(AdditionalLightsShadowCasterPass additionalLightsShadowCasterPass)
		{
			this.m_AdditionalLightsShadowCasterPass = additionalLightsShadowCasterPass;
		}

		public void OnCameraCleanup(CommandBuffer cmd)
		{
			cmd.SetKeyword(ShaderGlobalKeywords._GBUFFER_NORMALS_OCT, false);
			if (this.m_stencilVisLights.IsCreated)
			{
				this.m_stencilVisLights.Dispose();
			}
			if (this.m_stencilVisLightOffsets.IsCreated)
			{
				this.m_stencilVisLightOffsets.Dispose();
			}
		}

		internal static StencilState OverwriteStencil(StencilState s, int stencilWriteMask)
		{
			if (!s.enabled)
			{
				return new StencilState(true, 0, (byte)stencilWriteMask, CompareFunction.Always, StencilOp.Replace, StencilOp.Keep, StencilOp.Keep, CompareFunction.Always, StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);
			}
			CompareFunction compareFunctionFront = (s.compareFunctionFront != CompareFunction.Disabled) ? s.compareFunctionFront : CompareFunction.Always;
			CompareFunction compareFunctionBack = (s.compareFunctionBack != CompareFunction.Disabled) ? s.compareFunctionBack : CompareFunction.Always;
			StencilOp passOperationFront = s.passOperationFront;
			StencilOp failOperationFront = s.failOperationFront;
			StencilOp zFailOperationFront = s.zFailOperationFront;
			StencilOp passOperationBack = s.passOperationBack;
			StencilOp failOperationBack = s.failOperationBack;
			StencilOp zFailOperationBack = s.zFailOperationBack;
			return new StencilState(true, s.readMask & 15, (byte)((int)s.writeMask | stencilWriteMask), compareFunctionFront, passOperationFront, failOperationFront, zFailOperationFront, compareFunctionBack, passOperationBack, failOperationBack, zFailOperationBack);
		}

		internal static RenderStateBlock OverwriteStencil(RenderStateBlock block, int stencilWriteMask, int stencilRef)
		{
			if (!block.stencilState.enabled)
			{
				block.stencilState = new StencilState(true, 0, (byte)stencilWriteMask, CompareFunction.Always, StencilOp.Replace, StencilOp.Keep, StencilOp.Keep, CompareFunction.Always, StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);
			}
			else
			{
				StencilState stencilState = block.stencilState;
				CompareFunction compareFunctionFront = (stencilState.compareFunctionFront != CompareFunction.Disabled) ? stencilState.compareFunctionFront : CompareFunction.Always;
				CompareFunction compareFunctionBack = (stencilState.compareFunctionBack != CompareFunction.Disabled) ? stencilState.compareFunctionBack : CompareFunction.Always;
				StencilOp passOperationFront = stencilState.passOperationFront;
				StencilOp failOperationFront = stencilState.failOperationFront;
				StencilOp zFailOperationFront = stencilState.zFailOperationFront;
				StencilOp passOperationBack = stencilState.passOperationBack;
				StencilOp failOperationBack = stencilState.failOperationBack;
				StencilOp zFailOperationBack = stencilState.zFailOperationBack;
				block.stencilState = new StencilState(true, stencilState.readMask & 15, (byte)((int)stencilState.writeMask | stencilWriteMask), compareFunctionFront, passOperationFront, failOperationFront, zFailOperationFront, compareFunctionBack, passOperationBack, failOperationBack, zFailOperationBack);
			}
			block.mask |= RenderStateMask.Stencil;
			block.stencilReference = ((block.stencilReference & 15) | stencilRef);
			return block;
		}

		internal void ExecuteDeferredPass(RasterCommandBuffer cmd, UniversalCameraData cameraData, UniversalLightData lightData, UniversalShadowData shadowData)
		{
			if (this.m_UseDeferredPlus)
			{
				if (this.m_ClusterDeferredPasses[0] < 0)
				{
					this.InitClusterDeferredMaterial();
				}
			}
			else if (this.m_StencilDeferredPasses[0] < 0)
			{
				this.InitStencilDeferredMaterial();
			}
			if (!this.UseFramebufferFetch)
			{
				Material material = this.m_UseDeferredPlus ? this.m_ClusterDeferredMaterial : this.m_StencilDeferredMaterial;
				for (int i = 0; i < this.GbufferRTHandles.Length; i++)
				{
					if (i != this.GBufferLightingIndex)
					{
						material.SetTexture(DeferredLights.k_GBufferShaderPropertyIDs[i], this.GbufferRTHandles[i]);
					}
				}
			}
			using (new ProfilingScope(cmd, DeferredLights.m_ProfilingDeferredPass))
			{
				cmd.SetKeyword(ShaderGlobalKeywords._DEFERRED_MIXED_LIGHTING, this.UseShadowMask);
				this.SetupMatrixConstants(cmd, cameraData);
				if (!this.m_UseDeferredPlus && !this.HasStencilLightsOfType(LightType.Directional))
				{
					this.RenderSSAOBeforeShading(cmd);
				}
				if (this.m_UseDeferredPlus)
				{
					this.RenderClusterLights(cmd, shadowData);
				}
				else
				{
					this.RenderStencilLights(cmd, lightData, shadowData, cameraData.renderer.stripShadowsOffVariants);
				}
				cmd.SetKeyword(ShaderGlobalKeywords._DEFERRED_MIXED_LIGHTING, false);
				this.RenderFog(cmd, cameraData.camera.orthographic);
			}
			cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightShadows, shadowData.isKeywordAdditionalLightShadowsEnabled);
			ShadowUtils.SetSoftShadowQualityShaderKeywords(cmd, shadowData);
			cmd.SetKeyword(ShaderGlobalKeywords.LightCookies, this.m_LightCookieManager != null && this.m_LightCookieManager.IsKeywordLightCookieEnabled);
		}

		private void SetupShaderLightConstants(CommandBuffer cmd, UniversalLightData lightData)
		{
			this.SetupMainLightConstants(cmd, lightData);
		}

		private void SetupMainLightConstants(CommandBuffer cmd, UniversalLightData lightData)
		{
			if (lightData.mainLightIndex < 0)
			{
				return;
			}
			Vector4 value;
			Vector4 value2;
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			UniversalRenderPipeline.InitializeLightConstants_Common(lightData.visibleLights, lightData.mainLightIndex, out value, out value2, out vector, out vector2, out vector3);
			if (lightData.supportsLightLayers)
			{
				Light light = lightData.visibleLights[lightData.mainLightIndex].light;
				this.SetRenderingLayersMask(CommandBufferHelpers.GetRasterCommandBuffer(cmd), light, DeferredLights.ShaderConstants._MainLightLayerMask);
			}
			cmd.SetGlobalVector(DeferredLights.ShaderConstants._MainLightPosition, value);
			cmd.SetGlobalVector(DeferredLights.ShaderConstants._MainLightColor, value2);
		}

		internal Matrix4x4[] GetScreenToWorldMatrix(UniversalCameraData cameraData)
		{
			int num = (cameraData.xr.enabled && cameraData.xr.singlePassEnabled) ? 2 : 1;
			Matrix4x4[] screenToWorld = this.m_ScreenToWorld;
			Matrix4x4 rhs = new Matrix4x4(new Vector4(2f / (float)this.RenderWidth, 0f, 0f, 0f), new Vector4(0f, 2f / (float)this.RenderHeight, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(-1f, -1f, 0f, 1f));
			if (DeferredConfig.IsOpenGL)
			{
				rhs = new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 2f, 0f), new Vector4(0f, 0f, -1f, 1f)) * rhs;
			}
			for (int i = 0; i < num; i++)
			{
				Matrix4x4 viewMatrix = cameraData.GetViewMatrix(i);
				Matrix4x4 gpuprojectionMatrix = cameraData.GetGPUProjectionMatrix(false, i);
				screenToWorld[i] = Matrix4x4.Inverse(gpuprojectionMatrix * viewMatrix) * rhs;
			}
			return screenToWorld;
		}

		private void SetupMatrixConstants(RasterCommandBuffer cmd, UniversalCameraData cameraData)
		{
			cmd.SetGlobalMatrixArray(DeferredLights.ShaderConstants._ScreenToWorld, this.GetScreenToWorldMatrix(cameraData));
		}

		private void PrecomputeLights(out NativeArray<ushort> stencilVisLights, out NativeArray<ushort> stencilVisLightOffsets, ref NativeArray<VisibleLight> visibleLights, bool hasAdditionalLights)
		{
			if (!hasAdditionalLights)
			{
				stencilVisLights = new NativeArray<ushort>(0, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
				stencilVisLightOffsets = new NativeArray<ushort>(8, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
				for (int i = 0; i < 8; i++)
				{
					stencilVisLightOffsets[i] = DeferredLights.k_InvalidLightOffset;
				}
				return;
			}
			NativeArray<int> nativeArray = new NativeArray<int>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			stencilVisLightOffsets = new NativeArray<ushort>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int length = visibleLights.Length;
			ushort num = 0;
			while ((int)num < length)
			{
				ref VisibleLight ptr = ref visibleLights.UnsafeElementAtMutable((int)num);
				ref NativeArray<ushort> ptr2 = ref stencilVisLightOffsets;
				int lightType = (int)ptr.lightType;
				ushort value = ptr2[lightType] + 1;
				ptr2[lightType] = value;
				num += 1;
			}
			int length2 = (int)(stencilVisLightOffsets[0] + stencilVisLightOffsets[1] + stencilVisLightOffsets[2]);
			stencilVisLights = new NativeArray<ushort>(length2, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			int j = 0;
			int num2 = 0;
			while (j < stencilVisLightOffsets.Length)
			{
				if (stencilVisLightOffsets[j] == 0)
				{
					stencilVisLightOffsets[j] = DeferredLights.k_InvalidLightOffset;
				}
				else
				{
					int num3 = (int)stencilVisLightOffsets[j];
					stencilVisLightOffsets[j] = (ushort)num2;
					num2 += num3;
				}
				j++;
			}
			ushort num4 = 0;
			while ((int)num4 < length)
			{
				ref VisibleLight ptr3 = ref visibleLights.UnsafeElementAtMutable((int)num4);
				if (ptr3.lightType == LightType.Spot || ptr3.lightType == LightType.Directional || ptr3.lightType == LightType.Point)
				{
					int lightType = (int)ptr3.lightType;
					int num5 = nativeArray[lightType];
					nativeArray[lightType] = num5 + 1;
					int num6 = num5;
					stencilVisLights[(int)stencilVisLightOffsets[(int)ptr3.lightType] + num6] = num4;
				}
				num4 += 1;
			}
			nativeArray.Dispose();
		}

		private bool HasStencilLightsOfType(LightType type)
		{
			return this.m_stencilVisLightOffsets[(int)type] != DeferredLights.k_InvalidLightOffset;
		}

		private void RenderClusterLights(RasterCommandBuffer cmd, UniversalShadowData shadowData)
		{
			if (this.m_ClusterDeferredMaterial == null)
			{
				Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", new object[]
				{
					this.m_ClusterDeferredMaterial,
					base.GetType().Name
				});
				return;
			}
			using (new ProfilingScope(cmd, this.m_ProfilingSamplerDeferredShadingPass))
			{
				if (this.m_FullscreenMesh == null)
				{
					this.m_FullscreenMesh = DeferredLights.CreateFullscreenMesh();
				}
				ShadowUtils.SetSoftShadowQualityShaderKeywords(cmd, shadowData);
				cmd.DrawMesh(this.m_FullscreenMesh, Matrix4x4.identity, this.m_ClusterDeferredMaterial, 0, this.m_ClusterDeferredPasses[0]);
				cmd.DrawMesh(this.m_FullscreenMesh, Matrix4x4.identity, this.m_ClusterDeferredMaterial, 0, this.m_ClusterDeferredPasses[1]);
			}
		}

		private void RenderStencilLights(RasterCommandBuffer cmd, UniversalLightData lightData, UniversalShadowData shadowData, bool stripShadowsOffVariants)
		{
			if (this.m_stencilVisLights.Length == 0)
			{
				return;
			}
			if (this.m_StencilDeferredMaterial == null)
			{
				Debug.LogErrorFormat("Missing {0}. {1} render pass will not execute. Check for missing reference in the renderer resources.", new object[]
				{
					this.m_StencilDeferredMaterial,
					base.GetType().Name
				});
				return;
			}
			using (new ProfilingScope(cmd, this.m_ProfilingSamplerDeferredStencilPass))
			{
				NativeArray<VisibleLight> visibleLights = lightData.visibleLights;
				bool hasLightCookieManager = this.m_LightCookieManager != null;
				bool hasAdditionalLightPass = this.m_AdditionalLightsShadowCasterPass != null;
				if (this.HasStencilLightsOfType(LightType.Directional))
				{
					this.RenderStencilDirectionalLights(cmd, stripShadowsOffVariants, lightData, shadowData, visibleLights, hasAdditionalLightPass, hasLightCookieManager, lightData.mainLightIndex);
				}
				if (lightData.supportsAdditionalLights)
				{
					if (this.HasStencilLightsOfType(LightType.Point))
					{
						this.RenderStencilPointLights(cmd, stripShadowsOffVariants, lightData, shadowData, visibleLights, hasAdditionalLightPass, hasLightCookieManager);
					}
					if (this.HasStencilLightsOfType(LightType.Spot))
					{
						this.RenderStencilSpotLights(cmd, stripShadowsOffVariants, lightData, shadowData, visibleLights, hasAdditionalLightPass, hasLightCookieManager);
					}
				}
			}
		}

		private void RenderStencilDirectionalLights(RasterCommandBuffer cmd, bool stripShadowsOffVariants, UniversalLightData lightData, UniversalShadowData shadowData, NativeArray<VisibleLight> visibleLights, bool hasAdditionalLightPass, bool hasLightCookieManager, int mainLightIndex)
		{
			if (this.m_FullscreenMesh == null)
			{
				this.m_FullscreenMesh = DeferredLights.CreateFullscreenMesh();
			}
			cmd.SetKeyword(ShaderGlobalKeywords._DIRECTIONAL, true);
			int num = -1;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			for (int i = (int)this.m_stencilVisLightOffsets[1]; i < this.m_stencilVisLights.Length; i++)
			{
				ushort num2 = this.m_stencilVisLights[i];
				ref VisibleLight ptr = ref visibleLights.UnsafeElementAtMutable((int)num2);
				if (ptr.lightType != LightType.Directional)
				{
					break;
				}
				Light light = ptr.light;
				Vector4 value;
				Vector4 value2;
				Vector4 vector;
				Vector4 vector2;
				Vector4 vector3;
				UniversalRenderPipeline.InitializeLightConstants_Common(visibleLights, (int)num2, out value, out value2, out vector, out vector2, out vector3);
				int num3 = 0;
				if (light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed)
				{
					num3 |= 4;
				}
				if (lightData.supportsLightLayers)
				{
					this.SetRenderingLayersMask(cmd, light, DeferredLights.ShaderConstants._LightLayerMask);
				}
				bool hasDeferredShadows = light && light.shadows > LightShadows.None;
				bool flag5 = (int)num2 == mainLightIndex;
				if (!flag5)
				{
					int num4 = hasAdditionalLightPass ? this.m_AdditionalLightsShadowCasterPass.GetShadowLightIndexFromLightIndex((int)num2) : -1;
					hasDeferredShadows = (light && light.shadows != LightShadows.None && num4 >= 0);
					cmd.SetGlobalInt(DeferredLights.ShaderConstants._ShadowLightIndex, num4);
					this.SetLightCookiesKeyword(cmd, (int)num2, hasLightCookieManager, flag, ref flag2, ref num);
				}
				this.SetAdditionalLightsShadowsKeyword(ref cmd, stripShadowsOffVariants, shadowData.additionalLightShadowsEnabled, hasDeferredShadows, flag, ref flag3);
				this.SetSoftShadowsKeyword(cmd, shadowData, light, hasDeferredShadows, flag, ref flag4);
				cmd.SetKeyword(ShaderGlobalKeywords._DEFERRED_FIRST_LIGHT, flag);
				cmd.SetKeyword(ShaderGlobalKeywords._DEFERRED_MAIN_LIGHT, flag5);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightColor, value2);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightDirection, value);
				cmd.SetGlobalInt(DeferredLights.ShaderConstants._LightFlags, num3);
				cmd.DrawMesh(this.m_FullscreenMesh, Matrix4x4.identity, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[3]);
				cmd.DrawMesh(this.m_FullscreenMesh, Matrix4x4.identity, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[4]);
				flag = false;
			}
			cmd.SetKeyword(ShaderGlobalKeywords._DIRECTIONAL, false);
		}

		private void RenderStencilPointLights(RasterCommandBuffer cmd, bool stripShadowsOffVariants, UniversalLightData lightData, UniversalShadowData shadowData, NativeArray<VisibleLight> visibleLights, bool hasAdditionalLightPass, bool hasLightCookieManager)
		{
			if (this.m_SphereMesh == null)
			{
				this.m_SphereMesh = DeferredLights.CreateSphereMesh();
			}
			cmd.SetKeyword(ShaderGlobalKeywords._POINT, true);
			int num = -1;
			bool shouldOverride = true;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = (int)this.m_stencilVisLightOffsets[2]; i < this.m_stencilVisLights.Length; i++)
			{
				ushort num2 = this.m_stencilVisLights[i];
				ref VisibleLight ptr = ref visibleLights.UnsafeElementAtMutable((int)num2);
				if (ptr.lightType != LightType.Point)
				{
					break;
				}
				Light light = ptr.light;
				Vector3 vector = ptr.localToWorldMatrix.GetColumn(3);
				Matrix4x4 matrix = new Matrix4x4(new Vector4(ptr.range, 0f, 0f, 0f), new Vector4(0f, ptr.range, 0f, 0f), new Vector4(0f, 0f, ptr.range, 0f), new Vector4(vector.x, vector.y, vector.z, 1f));
				Vector4 value;
				Vector4 value2;
				Vector4 value3;
				Vector4 vector2;
				Vector4 value4;
				UniversalRenderPipeline.InitializeLightConstants_Common(visibleLights, (int)num2, out value, out value2, out value3, out vector2, out value4);
				if (lightData.supportsLightLayers)
				{
					this.SetRenderingLayersMask(cmd, light, DeferredLights.ShaderConstants._LightLayerMask);
				}
				int num3 = 0;
				if (light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed)
				{
					num3 |= 4;
				}
				int num4 = hasAdditionalLightPass ? this.m_AdditionalLightsShadowCasterPass.GetShadowLightIndexFromLightIndex((int)num2) : -1;
				bool hasDeferredShadows = light && light.shadows != LightShadows.None && num4 >= 0;
				this.SetAdditionalLightsShadowsKeyword(ref cmd, stripShadowsOffVariants, shadowData.additionalLightShadowsEnabled, hasDeferredShadows, shouldOverride, ref flag2);
				this.SetSoftShadowsKeyword(cmd, shadowData, light, hasDeferredShadows, shouldOverride, ref flag3);
				this.SetLightCookiesKeyword(cmd, (int)num2, hasLightCookieManager, shouldOverride, ref flag, ref num);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightPosWS, value);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightColor, value2);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightAttenuation, value3);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightOcclusionProbInfo, value4);
				cmd.SetGlobalInt(DeferredLights.ShaderConstants._LightFlags, num3);
				cmd.SetGlobalInt(DeferredLights.ShaderConstants._ShadowLightIndex, num4);
				cmd.DrawMesh(this.m_SphereMesh, matrix, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[0]);
				cmd.DrawMesh(this.m_SphereMesh, matrix, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[1]);
				cmd.DrawMesh(this.m_SphereMesh, matrix, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[2]);
				shouldOverride = false;
			}
			cmd.SetKeyword(ShaderGlobalKeywords._POINT, false);
		}

		private void RenderStencilSpotLights(RasterCommandBuffer cmd, bool stripShadowsOffVariants, UniversalLightData lightData, UniversalShadowData shadowData, NativeArray<VisibleLight> visibleLights, bool hasAdditionalLightPass, bool hasLightCookieManager)
		{
			if (this.m_HemisphereMesh == null)
			{
				this.m_HemisphereMesh = DeferredLights.CreateHemisphereMesh();
			}
			cmd.SetKeyword(ShaderGlobalKeywords._SPOT, true);
			int num = -1;
			bool shouldOverride = true;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = (int)this.m_stencilVisLightOffsets[0]; i < this.m_stencilVisLights.Length; i++)
			{
				ushort num2 = this.m_stencilVisLights[i];
				ref VisibleLight ptr = ref visibleLights.UnsafeElementAtMutable((int)num2);
				if (ptr.lightType != LightType.Spot)
				{
					break;
				}
				Light light = ptr.light;
				float f = 0.017453292f * ptr.spotAngle * 0.5f;
				float num3 = Mathf.Cos(f);
				float num4 = Mathf.Sin(f);
				float num5 = Mathf.Lerp(1f, DeferredLights.kStencilShapeGuard, num4);
				Vector4 value;
				Vector4 value2;
				Vector4 value3;
				Vector4 vector;
				Vector4 value4;
				UniversalRenderPipeline.InitializeLightConstants_Common(visibleLights, (int)num2, out value, out value2, out value3, out vector, out value4);
				if (lightData.supportsLightLayers)
				{
					this.SetRenderingLayersMask(cmd, light, DeferredLights.ShaderConstants._LightLayerMask);
				}
				int num6 = 0;
				if (light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed)
				{
					num6 |= 4;
				}
				int num7 = hasAdditionalLightPass ? this.m_AdditionalLightsShadowCasterPass.GetShadowLightIndexFromLightIndex((int)num2) : -1;
				bool hasDeferredShadows = light && light.shadows != LightShadows.None && num7 >= 0;
				this.SetAdditionalLightsShadowsKeyword(ref cmd, stripShadowsOffVariants, shadowData.additionalLightShadowsEnabled, hasDeferredShadows, shouldOverride, ref flag2);
				this.SetSoftShadowsKeyword(cmd, shadowData, light, hasDeferredShadows, shouldOverride, ref flag3);
				this.SetLightCookiesKeyword(cmd, (int)num2, hasLightCookieManager, shouldOverride, ref flag, ref num);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._SpotLightScale, new Vector4(num4, num4, 1f - num3, ptr.range));
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._SpotLightBias, new Vector4(0f, 0f, num3, 0f));
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._SpotLightGuard, new Vector4(num5, num5, num5, num3 * ptr.range));
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightPosWS, value);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightColor, value2);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightAttenuation, value3);
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightDirection, new Vector3(vector.x, vector.y, vector.z));
				cmd.SetGlobalVector(DeferredLights.ShaderConstants._LightOcclusionProbInfo, value4);
				cmd.SetGlobalInt(DeferredLights.ShaderConstants._LightFlags, num6);
				cmd.SetGlobalInt(DeferredLights.ShaderConstants._ShadowLightIndex, num7);
				cmd.DrawMesh(this.m_HemisphereMesh, ptr.localToWorldMatrix, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[0]);
				cmd.DrawMesh(this.m_HemisphereMesh, ptr.localToWorldMatrix, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[1]);
				cmd.DrawMesh(this.m_HemisphereMesh, ptr.localToWorldMatrix, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[2]);
				shouldOverride = false;
			}
			cmd.SetKeyword(ShaderGlobalKeywords._SPOT, false);
		}

		private void RenderSSAOBeforeShading(RasterCommandBuffer cmd)
		{
			if (this.m_FullscreenMesh == null)
			{
				this.m_FullscreenMesh = DeferredLights.CreateFullscreenMesh();
			}
			cmd.DrawMesh(this.m_FullscreenMesh, Matrix4x4.identity, this.m_StencilDeferredMaterial, 0, this.m_StencilDeferredPasses[6]);
		}

		private void RenderFog(RasterCommandBuffer cmd, bool isOrthographic)
		{
			if (!RenderSettings.fog || isOrthographic)
			{
				return;
			}
			if (this.m_FullscreenMesh == null)
			{
				this.m_FullscreenMesh = DeferredLights.CreateFullscreenMesh();
			}
			using (new ProfilingScope(cmd, this.m_ProfilingSamplerDeferredFogPass))
			{
				Material material = this.m_UseDeferredPlus ? this.m_ClusterDeferredMaterial : this.m_StencilDeferredMaterial;
				int shaderPass = this.m_UseDeferredPlus ? this.m_ClusterDeferredPasses[2] : this.m_StencilDeferredPasses[5];
				cmd.DrawMesh(this.m_FullscreenMesh, Matrix4x4.identity, material, 0, shaderPass);
			}
		}

		private void InitStencilDeferredMaterial()
		{
			if (this.m_StencilDeferredMaterial == null)
			{
				return;
			}
			for (int i = 0; i < DeferredLights.k_StencilDeferredPassNames.Length; i++)
			{
				this.m_StencilDeferredPasses[i] = this.m_StencilDeferredMaterial.FindPass(DeferredLights.k_StencilDeferredPassNames[i]);
			}
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._StencilRef, 0f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._StencilReadMask, 96f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._StencilWriteMask, 16f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitPunctualStencilRef, 48f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitPunctualStencilReadMask, 112f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitPunctualStencilWriteMask, 16f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitPunctualStencilRef, 80f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitPunctualStencilReadMask, 112f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitPunctualStencilWriteMask, 16f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitDirStencilRef, 32f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitDirStencilReadMask, 96f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitDirStencilWriteMask, 0f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitDirStencilRef, 64f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitDirStencilReadMask, 96f);
			this.m_StencilDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitDirStencilWriteMask, 0f);
		}

		private void InitClusterDeferredMaterial()
		{
			if (this.m_ClusterDeferredMaterial == null)
			{
				return;
			}
			for (int i = 0; i < DeferredLights.k_ClusterDeferredPassNames.Length; i++)
			{
				this.m_ClusterDeferredPasses[i] = this.m_ClusterDeferredMaterial.FindPass(DeferredLights.k_ClusterDeferredPassNames[i]);
			}
			this.m_ClusterDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitStencilRef, 32f);
			this.m_ClusterDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitStencilReadMask, 96f);
			this.m_ClusterDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._LitStencilWriteMask, 0f);
			this.m_ClusterDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitStencilRef, 64f);
			this.m_ClusterDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitStencilReadMask, 96f);
			this.m_ClusterDeferredMaterial.SetFloat(DeferredLights.ShaderConstants._SimpleLitStencilWriteMask, 0f);
		}

		private static Mesh CreateSphereMesh()
		{
			Vector3[] vertices = new Vector3[]
			{
				new Vector3(0f, 0f, -1.07f),
				new Vector3(0.174f, -0.535f, -0.91f),
				new Vector3(-0.455f, -0.331f, -0.91f),
				new Vector3(0.562f, 0f, -0.91f),
				new Vector3(-0.455f, 0.331f, -0.91f),
				new Vector3(0.174f, 0.535f, -0.91f),
				new Vector3(-0.281f, -0.865f, -0.562f),
				new Vector3(0.736f, -0.535f, -0.562f),
				new Vector3(0.296f, -0.91f, -0.468f),
				new Vector3(-0.91f, 0f, -0.562f),
				new Vector3(-0.774f, -0.562f, -0.478f),
				new Vector3(0f, -1.07f, 0f),
				new Vector3(-0.629f, -0.865f, 0f),
				new Vector3(0.629f, -0.865f, 0f),
				new Vector3(-1.017f, -0.331f, 0f),
				new Vector3(0.957f, 0f, -0.478f),
				new Vector3(0.736f, 0.535f, -0.562f),
				new Vector3(1.017f, -0.331f, 0f),
				new Vector3(1.017f, 0.331f, 0f),
				new Vector3(-0.296f, -0.91f, 0.478f),
				new Vector3(0.281f, -0.865f, 0.562f),
				new Vector3(0.774f, -0.562f, 0.478f),
				new Vector3(-0.736f, -0.535f, 0.562f),
				new Vector3(0.91f, 0f, 0.562f),
				new Vector3(0.455f, -0.331f, 0.91f),
				new Vector3(-0.174f, -0.535f, 0.91f),
				new Vector3(0.629f, 0.865f, 0f),
				new Vector3(0.774f, 0.562f, 0.478f),
				new Vector3(0.455f, 0.331f, 0.91f),
				new Vector3(0f, 0f, 1.07f),
				new Vector3(-0.562f, 0f, 0.91f),
				new Vector3(-0.957f, 0f, 0.478f),
				new Vector3(0.281f, 0.865f, 0.562f),
				new Vector3(-0.174f, 0.535f, 0.91f),
				new Vector3(0.296f, 0.91f, -0.478f),
				new Vector3(-1.017f, 0.331f, 0f),
				new Vector3(-0.736f, 0.535f, 0.562f),
				new Vector3(-0.296f, 0.91f, 0.478f),
				new Vector3(0f, 1.07f, 0f),
				new Vector3(-0.281f, 0.865f, -0.562f),
				new Vector3(-0.774f, 0.562f, -0.478f),
				new Vector3(-0.629f, 0.865f, 0f)
			};
			int[] triangles = new int[]
			{
				0,
				1,
				2,
				0,
				3,
				1,
				2,
				4,
				0,
				0,
				5,
				3,
				0,
				4,
				5,
				1,
				6,
				2,
				3,
				7,
				1,
				1,
				8,
				6,
				1,
				7,
				8,
				9,
				4,
				2,
				2,
				6,
				10,
				10,
				9,
				2,
				8,
				11,
				6,
				6,
				12,
				10,
				11,
				12,
				6,
				7,
				13,
				8,
				8,
				13,
				11,
				10,
				14,
				9,
				10,
				12,
				14,
				3,
				15,
				7,
				5,
				16,
				3,
				3,
				16,
				15,
				15,
				17,
				7,
				17,
				13,
				7,
				16,
				18,
				15,
				15,
				18,
				17,
				11,
				19,
				12,
				13,
				20,
				11,
				11,
				20,
				19,
				17,
				21,
				13,
				13,
				21,
				20,
				12,
				19,
				22,
				12,
				22,
				14,
				17,
				23,
				21,
				18,
				23,
				17,
				21,
				24,
				20,
				23,
				24,
				21,
				20,
				25,
				19,
				19,
				25,
				22,
				24,
				25,
				20,
				26,
				18,
				16,
				18,
				27,
				23,
				26,
				27,
				18,
				28,
				24,
				23,
				27,
				28,
				23,
				24,
				29,
				25,
				28,
				29,
				24,
				25,
				30,
				22,
				25,
				29,
				30,
				14,
				22,
				31,
				22,
				30,
				31,
				32,
				28,
				27,
				26,
				32,
				27,
				33,
				29,
				28,
				30,
				29,
				33,
				33,
				28,
				32,
				34,
				26,
				16,
				5,
				34,
				16,
				14,
				31,
				35,
				14,
				35,
				9,
				31,
				30,
				36,
				30,
				33,
				36,
				35,
				31,
				36,
				37,
				33,
				32,
				36,
				33,
				37,
				38,
				32,
				26,
				34,
				38,
				26,
				38,
				37,
				32,
				5,
				39,
				34,
				39,
				38,
				34,
				4,
				39,
				5,
				9,
				40,
				4,
				9,
				35,
				40,
				4,
				40,
				39,
				35,
				36,
				41,
				41,
				36,
				37,
				41,
				37,
				38,
				40,
				35,
				41,
				40,
				41,
				39,
				41,
				38,
				39
			};
			return new Mesh
			{
				indexFormat = IndexFormat.UInt16,
				vertices = vertices,
				triangles = triangles
			};
		}

		private static Mesh CreateHemisphereMesh()
		{
			Vector3[] vertices = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(0.92388f, 0.382683f, 0f),
				new Vector3(0.707107f, 0.707107f, 0f),
				new Vector3(0.382683f, 0.92388f, 0f),
				new Vector3(--0f, 1f, 0f),
				new Vector3(-0.382684f, 0.92388f, 0f),
				new Vector3(-0.707107f, 0.707107f, 0f),
				new Vector3(-0.92388f, 0.382683f, 0f),
				new Vector3(-1f, --0f, 0f),
				new Vector3(-0.92388f, -0.382683f, 0f),
				new Vector3(-0.707107f, -0.707107f, 0f),
				new Vector3(-0.382683f, -0.92388f, 0f),
				new Vector3(0f, -1f, 0f),
				new Vector3(0.382684f, -0.923879f, 0f),
				new Vector3(0.707107f, -0.707107f, 0f),
				new Vector3(0.92388f, -0.382683f, 0f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0.707107f, 0f, 0.707107f),
				new Vector3(0f, -0.707107f, 0.707107f),
				new Vector3(0f, 0.707107f, 0.707107f),
				new Vector3(-0.707107f, 0f, 0.707107f),
				new Vector3(0.816497f, -0.408248f, 0.408248f),
				new Vector3(0.408248f, -0.408248f, 0.816497f),
				new Vector3(0.408248f, -0.816497f, 0.408248f),
				new Vector3(0.408248f, 0.816497f, 0.408248f),
				new Vector3(0.408248f, 0.408248f, 0.816497f),
				new Vector3(0.816497f, 0.408248f, 0.408248f),
				new Vector3(-0.816497f, 0.408248f, 0.408248f),
				new Vector3(-0.408248f, 0.408248f, 0.816497f),
				new Vector3(-0.408248f, 0.816497f, 0.408248f),
				new Vector3(-0.408248f, -0.816497f, 0.408248f),
				new Vector3(-0.408248f, -0.408248f, 0.816497f),
				new Vector3(-0.816497f, -0.408248f, 0.408248f),
				new Vector3(0f, -0.92388f, 0.382683f),
				new Vector3(0.92388f, 0f, 0.382683f),
				new Vector3(0f, -0.382683f, 0.92388f),
				new Vector3(0.382683f, 0f, 0.92388f),
				new Vector3(0f, 0.92388f, 0.382683f),
				new Vector3(0f, 0.382683f, 0.92388f),
				new Vector3(-0.92388f, 0f, 0.382683f),
				new Vector3(-0.382683f, 0f, 0.92388f)
			};
			int[] triangles = new int[]
			{
				0,
				2,
				1,
				0,
				3,
				2,
				0,
				4,
				3,
				0,
				5,
				4,
				0,
				6,
				5,
				0,
				7,
				6,
				0,
				8,
				7,
				0,
				9,
				8,
				0,
				10,
				9,
				0,
				11,
				10,
				0,
				12,
				11,
				0,
				13,
				12,
				0,
				14,
				13,
				0,
				15,
				14,
				0,
				16,
				15,
				0,
				1,
				16,
				22,
				23,
				24,
				25,
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33,
				14,
				24,
				34,
				35,
				22,
				16,
				36,
				23,
				37,
				2,
				27,
				35,
				38,
				25,
				4,
				37,
				26,
				39,
				6,
				30,
				38,
				40,
				28,
				8,
				39,
				29,
				41,
				10,
				33,
				40,
				34,
				31,
				12,
				41,
				32,
				36,
				15,
				22,
				24,
				18,
				23,
				22,
				19,
				24,
				23,
				3,
				25,
				27,
				20,
				26,
				25,
				18,
				27,
				26,
				7,
				28,
				30,
				21,
				29,
				28,
				20,
				30,
				29,
				11,
				31,
				33,
				19,
				32,
				31,
				21,
				33,
				32,
				13,
				14,
				34,
				15,
				24,
				14,
				19,
				34,
				24,
				1,
				35,
				16,
				18,
				22,
				35,
				15,
				16,
				22,
				17,
				36,
				37,
				19,
				23,
				36,
				18,
				37,
				23,
				1,
				2,
				35,
				3,
				27,
				2,
				18,
				35,
				27,
				5,
				38,
				4,
				20,
				25,
				38,
				3,
				4,
				25,
				17,
				37,
				39,
				18,
				26,
				37,
				20,
				39,
				26,
				5,
				6,
				38,
				7,
				30,
				6,
				20,
				38,
				30,
				9,
				40,
				8,
				21,
				28,
				40,
				7,
				8,
				28,
				17,
				39,
				41,
				20,
				29,
				39,
				21,
				41,
				29,
				9,
				10,
				40,
				11,
				33,
				10,
				21,
				40,
				33,
				13,
				34,
				12,
				19,
				31,
				34,
				11,
				12,
				31,
				17,
				41,
				36,
				21,
				32,
				41,
				19,
				36,
				32
			};
			return new Mesh
			{
				indexFormat = IndexFormat.UInt16,
				vertices = vertices,
				triangles = triangles
			};
		}

		private static Mesh CreateFullscreenMesh()
		{
			Vector3[] vertices = new Vector3[]
			{
				new Vector3(-1f, 1f, 0f),
				new Vector3(-1f, -3f, 0f),
				new Vector3(3f, 1f, 0f)
			};
			int[] triangles = new int[]
			{
				0,
				1,
				2
			};
			return new Mesh
			{
				indexFormat = IndexFormat.UInt16,
				vertices = vertices,
				triangles = triangles
			};
		}

		private void SetRenderingLayersMask(RasterCommandBuffer cmd, Light light, int shaderPropertyID)
		{
			uint value = RenderingLayerUtils.ToValidRenderingLayers(light.GetUniversalAdditionalLightData().renderingLayers);
			cmd.SetGlobalInt(shaderPropertyID, (int)value);
		}

		private void SetAdditionalLightsShadowsKeyword(ref RasterCommandBuffer cmd, bool stripShadowsOffVariants, bool additionalLightShadowsEnabled, bool hasDeferredShadows, bool shouldOverride, ref bool lastShadowsKeyword)
		{
			bool flag = !stripShadowsOffVariants;
			bool flag2 = additionalLightShadowsEnabled && (!flag || hasDeferredShadows);
			if (!shouldOverride && lastShadowsKeyword == flag2)
			{
				return;
			}
			lastShadowsKeyword = flag2;
			cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightShadows, flag2);
		}

		private void SetSoftShadowsKeyword(RasterCommandBuffer cmd, UniversalShadowData shadowData, Light light, bool hasDeferredShadows, bool shouldOverride, ref bool lastHasSoftShadow)
		{
			bool flag = hasDeferredShadows && shadowData.supportsSoftShadows && light.shadows == LightShadows.Soft;
			if (!shouldOverride && lastHasSoftShadow == flag)
			{
				return;
			}
			lastHasSoftShadow = flag;
			ShadowUtils.SetPerLightSoftShadowKeyword(cmd, flag);
		}

		private void SetLightCookiesKeyword(RasterCommandBuffer cmd, int visLightIndex, bool hasLightCookieManager, bool shouldOverride, ref bool lastLightCookieState, ref int lastCookieLightIndex)
		{
			if (!hasLightCookieManager)
			{
				return;
			}
			int lightCookieShaderDataIndex = this.m_LightCookieManager.GetLightCookieShaderDataIndex(visLightIndex);
			bool flag = lightCookieShaderDataIndex >= 0;
			if (shouldOverride || flag != lastLightCookieState)
			{
				lastLightCookieState = flag;
				cmd.SetKeyword(ShaderGlobalKeywords.LightCookies, flag);
			}
			if (shouldOverride || lightCookieShaderDataIndex != lastCookieLightIndex)
			{
				lastCookieLightIndex = lightCookieShaderDataIndex;
				cmd.SetGlobalInt(DeferredLights.ShaderConstants._CookieLightIndex, lightCookieShaderDataIndex);
			}
		}

		internal static readonly string[] k_GBufferNames = new string[]
		{
			"_GBuffer0",
			"_GBuffer1",
			"_GBuffer2",
			"_GBuffer3",
			"_GBuffer4",
			"_GBuffer5",
			"_GBuffer6"
		};

		internal static readonly int[] k_GBufferShaderPropertyIDs = new int[]
		{
			Shader.PropertyToID(DeferredLights.k_GBufferNames[0]),
			Shader.PropertyToID(DeferredLights.k_GBufferNames[1]),
			Shader.PropertyToID(DeferredLights.k_GBufferNames[2]),
			Shader.PropertyToID(DeferredLights.k_GBufferNames[3]),
			Shader.PropertyToID(DeferredLights.k_GBufferNames[4]),
			Shader.PropertyToID(DeferredLights.k_GBufferNames[5]),
			Shader.PropertyToID(DeferredLights.k_GBufferNames[6])
		};

		private static readonly string[] k_StencilDeferredPassNames = new string[]
		{
			"Stencil Volume",
			"Deferred Punctual Light (Lit)",
			"Deferred Punctual Light (SimpleLit)",
			"Deferred Directional Light (Lit)",
			"Deferred Directional Light (SimpleLit)",
			"Fog",
			"SSAOOnly"
		};

		private static readonly string[] k_ClusterDeferredPassNames = new string[]
		{
			"Deferred Clustered Lights (Lit)",
			"Deferred Clustered Lights (SimpleLit)",
			"Fog"
		};

		private static readonly ushort k_InvalidLightOffset = ushort.MaxValue;

		private static readonly string k_SetupLights = "SetupLights";

		private static readonly string k_DeferredPass = "Deferred Pass";

		private static readonly string k_DeferredShadingPass = "Deferred Shading";

		private static readonly string k_DeferredStencilPass = "Deferred Shading (Stencil)";

		private static readonly string k_DeferredFogPass = "Deferred Fog";

		private static readonly string k_SetupLightConstants = "Setup Light Constants";

		private static readonly float kStencilShapeGuard = 1.06067f;

		private static readonly ProfilingSampler m_ProfilingSetupLights = new ProfilingSampler(DeferredLights.k_SetupLights);

		private static readonly ProfilingSampler m_ProfilingDeferredPass = new ProfilingSampler(DeferredLights.k_DeferredPass);

		private static readonly ProfilingSampler m_ProfilingSetupLightConstants = new ProfilingSampler(DeferredLights.k_SetupLightConstants);

		private RTHandle[] GbufferRTHandles;

		private NativeArray<ushort> m_stencilVisLights;

		private NativeArray<ushort> m_stencilVisLightOffsets;

		private AdditionalLightsShadowCasterPass m_AdditionalLightsShadowCasterPass;

		private Mesh m_SphereMesh;

		private Mesh m_HemisphereMesh;

		private Mesh m_FullscreenMesh;

		private Material m_StencilDeferredMaterial;

		private Material m_ClusterDeferredMaterial;

		private int[] m_StencilDeferredPasses;

		private int[] m_ClusterDeferredPasses;

		private Matrix4x4[] m_ScreenToWorld = new Matrix4x4[2];

		private ProfilingSampler m_ProfilingSamplerDeferredShadingPass = new ProfilingSampler(DeferredLights.k_DeferredShadingPass);

		private ProfilingSampler m_ProfilingSamplerDeferredStencilPass = new ProfilingSampler(DeferredLights.k_DeferredStencilPass);

		private ProfilingSampler m_ProfilingSamplerDeferredFogPass = new ProfilingSampler(DeferredLights.k_DeferredFogPass);

		private LightCookieManager m_LightCookieManager;

		private bool m_UseDeferredPlus;

		private static ProfilingSampler s_SetupDeferredLights = new ProfilingSampler("Setup Deferred lights");

		internal static class ShaderConstants
		{
			public static readonly int _LitStencilRef = Shader.PropertyToID("_LitStencilRef");

			public static readonly int _LitStencilReadMask = Shader.PropertyToID("_LitStencilReadMask");

			public static readonly int _LitStencilWriteMask = Shader.PropertyToID("_LitStencilWriteMask");

			public static readonly int _SimpleLitStencilRef = Shader.PropertyToID("_SimpleLitStencilRef");

			public static readonly int _SimpleLitStencilReadMask = Shader.PropertyToID("_SimpleLitStencilReadMask");

			public static readonly int _SimpleLitStencilWriteMask = Shader.PropertyToID("_SimpleLitStencilWriteMask");

			public static readonly int _StencilRef = Shader.PropertyToID("_StencilRef");

			public static readonly int _StencilReadMask = Shader.PropertyToID("_StencilReadMask");

			public static readonly int _StencilWriteMask = Shader.PropertyToID("_StencilWriteMask");

			public static readonly int _LitPunctualStencilRef = Shader.PropertyToID("_LitPunctualStencilRef");

			public static readonly int _LitPunctualStencilReadMask = Shader.PropertyToID("_LitPunctualStencilReadMask");

			public static readonly int _LitPunctualStencilWriteMask = Shader.PropertyToID("_LitPunctualStencilWriteMask");

			public static readonly int _SimpleLitPunctualStencilRef = Shader.PropertyToID("_SimpleLitPunctualStencilRef");

			public static readonly int _SimpleLitPunctualStencilReadMask = Shader.PropertyToID("_SimpleLitPunctualStencilReadMask");

			public static readonly int _SimpleLitPunctualStencilWriteMask = Shader.PropertyToID("_SimpleLitPunctualStencilWriteMask");

			public static readonly int _LitDirStencilRef = Shader.PropertyToID("_LitDirStencilRef");

			public static readonly int _LitDirStencilReadMask = Shader.PropertyToID("_LitDirStencilReadMask");

			public static readonly int _LitDirStencilWriteMask = Shader.PropertyToID("_LitDirStencilWriteMask");

			public static readonly int _SimpleLitDirStencilRef = Shader.PropertyToID("_SimpleLitDirStencilRef");

			public static readonly int _SimpleLitDirStencilReadMask = Shader.PropertyToID("_SimpleLitDirStencilReadMask");

			public static readonly int _SimpleLitDirStencilWriteMask = Shader.PropertyToID("_SimpleLitDirStencilWriteMask");

			public static readonly int _ScreenToWorld = Shader.PropertyToID("_ScreenToWorld");

			public static int _MainLightPosition = Shader.PropertyToID("_MainLightPosition");

			public static int _MainLightColor = Shader.PropertyToID("_MainLightColor");

			public static int _MainLightLayerMask = Shader.PropertyToID("_MainLightLayerMask");

			public static int _SpotLightScale = Shader.PropertyToID("_SpotLightScale");

			public static int _SpotLightBias = Shader.PropertyToID("_SpotLightBias");

			public static int _SpotLightGuard = Shader.PropertyToID("_SpotLightGuard");

			public static int _LightPosWS = Shader.PropertyToID("_LightPosWS");

			public static int _LightColor = Shader.PropertyToID("_LightColor");

			public static int _LightAttenuation = Shader.PropertyToID("_LightAttenuation");

			public static int _LightOcclusionProbInfo = Shader.PropertyToID("_LightOcclusionProbInfo");

			public static int _LightDirection = Shader.PropertyToID("_LightDirection");

			public static int _LightFlags = Shader.PropertyToID("_LightFlags");

			public static int _ShadowLightIndex = Shader.PropertyToID("_ShadowLightIndex");

			public static int _LightLayerMask = Shader.PropertyToID("_LightLayerMask");

			public static int _CookieLightIndex = Shader.PropertyToID("_CookieLightIndex");
		}

		internal enum StencilDeferredPasses
		{
			StencilVolume,
			PunctualLit,
			PunctualSimpleLit,
			DirectionalLit,
			DirectionalSimpleLit,
			Fog,
			SSAOOnly
		}

		internal enum ClusterDeferredPasses
		{
			ClusteredLightsLit,
			ClusteredLightsSimpleLit,
			Fog
		}

		internal struct InitParams
		{
			public Material stencilDeferredMaterial;

			public Material clusterDeferredMaterial;

			public LightCookieManager lightCookieManager;

			public bool deferredPlus;
		}

		private class SetupLightPassData
		{
			internal UniversalCameraData cameraData;

			internal UniversalLightData lightData;

			internal DeferredLights deferredLights;

			internal Vector2Int cameraTargetSizeCopy;
		}
	}
}
