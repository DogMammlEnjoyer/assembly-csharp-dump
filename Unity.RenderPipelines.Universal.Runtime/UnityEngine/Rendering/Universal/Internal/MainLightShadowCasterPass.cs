using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class MainLightShadowCasterPass : ScriptableRenderPass
	{
		public MainLightShadowCasterPass(RenderPassEvent evt)
		{
			base.profilingSampler = new ProfilingSampler("Draw Main Light Shadowmap");
			base.renderPassEvent = evt;
			this.m_PassData = new MainLightShadowCasterPass.PassData();
			this.m_MainLightShadowMatrices = new Matrix4x4[5];
			this.m_CascadeSlices = new ShadowSliceData[4];
			this.m_CascadeSplitDistances = new Vector4[4];
			this.m_EmptyShadowmapNeedsClear = true;
		}

		public void Dispose()
		{
			RTHandle mainLightShadowmapTexture = this.m_MainLightShadowmapTexture;
			if (mainLightShadowmapTexture != null)
			{
				mainLightShadowmapTexture.Release();
			}
			RTHandle emptyMainLightShadowmapTexture = this.m_EmptyMainLightShadowmapTexture;
			if (emptyMainLightShadowmapTexture == null)
			{
				return;
			}
			emptyMainLightShadowmapTexture.Release();
		}

		public bool Setup(ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();
			return this.Setup(renderingData2, cameraData, lightData, shadowData);
		}

		public bool Setup(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, UniversalShadowData shadowData)
		{
			bool mainLightShadowsEnabled = shadowData.mainLightShadowsEnabled;
			bool supportsMainLightShadows = shadowData.supportsMainLightShadows;
			bool result;
			using (new ProfilingScope(this.m_ProfilingSetupSampler))
			{
				bool stripShadowsOffVariants = cameraData.renderer.stripShadowsOffVariants;
				this.Clear();
				int mainLightIndex = lightData.mainLightIndex;
				if (mainLightIndex == -1)
				{
					if (mainLightShadowsEnabled)
					{
						result = this.SetupForEmptyRendering(stripShadowsOffVariants, mainLightShadowsEnabled, null, cameraData, shadowData);
					}
					else
					{
						result = false;
					}
				}
				else
				{
					VisibleLight visibleLight = lightData.visibleLights[mainLightIndex];
					Light light = visibleLight.light;
					if (supportsMainLightShadows && light.shadows == LightShadows.None)
					{
						result = this.SetupForEmptyRendering(stripShadowsOffVariants, mainLightShadowsEnabled, light, cameraData, shadowData);
					}
					else if (!mainLightShadowsEnabled)
					{
						if (light.shadows != LightShadows.None && light.bakingOutput.isBaked && light.bakingOutput.mixedLightingMode != MixedLightingMode.IndirectOnly && light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed)
						{
							result = this.SetupForEmptyRendering(stripShadowsOffVariants, mainLightShadowsEnabled, light, cameraData, shadowData);
						}
						else
						{
							result = false;
						}
					}
					else if (!supportsMainLightShadows)
					{
						result = this.SetupForEmptyRendering(stripShadowsOffVariants, mainLightShadowsEnabled, null, cameraData, shadowData);
					}
					else
					{
						if (visibleLight.lightType != LightType.Directional)
						{
							Debug.LogWarning("Only directional lights are supported as main light.");
						}
						Bounds bounds;
						if (!renderingData.cullResults.GetShadowCasterBounds(mainLightIndex, out bounds))
						{
							result = this.SetupForEmptyRendering(stripShadowsOffVariants, mainLightShadowsEnabled, light, cameraData, shadowData);
						}
						else
						{
							this.m_ShadowCasterCascadesCount = shadowData.mainLightShadowCascadesCount;
							this.renderTargetWidth = shadowData.mainLightRenderTargetWidth;
							this.renderTargetHeight = shadowData.mainLightRenderTargetHeight;
							ref URPLightShadowCullingInfos ptr = ref shadowData.visibleLightsShadowCullingInfos.UnsafeElementAt(mainLightIndex);
							for (int i = 0; i < this.m_ShadowCasterCascadesCount; i++)
							{
								ref ShadowSliceData ptr2 = ref ptr.slices.UnsafeElementAt(i);
								Vector4[] cascadeSplitDistances = this.m_CascadeSplitDistances;
								int num = i;
								ShadowSplitData splitData = ptr2.splitData;
								cascadeSplitDistances[num] = splitData.cullingSphere;
								this.m_CascadeSlices[i] = ptr2;
								if (!ptr.IsSliceValid(i))
								{
									return this.SetupForEmptyRendering(stripShadowsOffVariants, mainLightShadowsEnabled, light, cameraData, shadowData);
								}
							}
							this.UpdateTextureDescriptorIfNeeded();
							this.m_MaxShadowDistanceSq = cameraData.maxShadowDistance * cameraData.maxShadowDistance;
							this.m_CascadeBorder = shadowData.mainLightShadowCascadeBorder;
							this.m_CreateEmptyShadowmap = false;
							base.useNativeRenderPass = true;
							result = true;
						}
					}
				}
			}
			return result;
		}

		private void UpdateTextureDescriptorIfNeeded()
		{
			if (this.m_MainLightShadowDescriptor.width != this.renderTargetWidth || this.m_MainLightShadowDescriptor.height != this.renderTargetHeight || this.m_MainLightShadowDescriptor.depthBufferBits != 16 || this.m_MainLightShadowDescriptor.colorFormat != RenderTextureFormat.Shadowmap)
			{
				this.m_MainLightShadowDescriptor = new RenderTextureDescriptor(this.renderTargetWidth, this.renderTargetHeight, RenderTextureFormat.Shadowmap, 16);
			}
		}

		private bool SetupForEmptyRendering(bool stripShadowsOffVariants, bool shadowsEnabled, Light light, UniversalCameraData cameraData, UniversalShadowData shadowData)
		{
			if (!stripShadowsOffVariants)
			{
				return false;
			}
			this.m_CreateEmptyShadowmap = true;
			base.useNativeRenderPass = false;
			this.m_SetKeywordForEmptyShadowmap = shadowsEnabled;
			if (light == null)
			{
				MainLightShadowCasterPass.s_EmptyShadowParams = new Vector4(0f, 0f, 1f, 0f);
			}
			else
			{
				bool supportsSoftShadows = shadowData.supportsSoftShadows;
				float maxShadowDistance = cameraData.maxShadowDistance;
				float mainLightShadowCascadeBorder = shadowData.mainLightShadowCascadeBorder;
				bool softShadowsEnabled = light.shadows == LightShadows.Soft && supportsSoftShadows;
				float y = ShadowUtils.SoftShadowQualityToShaderProperty(light, softShadowsEnabled);
				float z;
				float w;
				ShadowUtils.GetScaleAndBiasForLinearDistanceFade(maxShadowDistance, mainLightShadowCascadeBorder, out z, out w);
				MainLightShadowCasterPass.s_EmptyShadowParams = new Vector4(light.shadowStrength, y, z, w);
			}
			return true;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			if (this.m_CreateEmptyShadowmap)
			{
				if (ShadowUtils.ShadowRTReAllocateIfNeeded(ref this.m_EmptyMainLightShadowmapTexture, 1, 1, 16, 1, 0f, "_EmptyMainLightShadowmapTexture"))
				{
					this.m_EmptyShadowmapNeedsClear = true;
				}
				if (!this.m_EmptyShadowmapNeedsClear)
				{
					return;
				}
				base.ConfigureTarget(this.m_EmptyMainLightShadowmapTexture);
				this.m_EmptyShadowmapNeedsClear = false;
			}
			else
			{
				ShadowUtils.ShadowRTReAllocateIfNeeded(ref this.m_MainLightShadowmapTexture, this.renderTargetWidth, this.renderTargetHeight, 16, 1, 0f, "_MainLightShadowmapTexture");
				base.ConfigureTarget(this.m_MainLightShadowmapTexture);
			}
			base.ConfigureClear(ClearFlag.All, Color.black);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();
			RasterCommandBuffer rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer);
			if (this.m_CreateEmptyShadowmap)
			{
				if (this.m_SetKeywordForEmptyShadowmap)
				{
					rasterCommandBuffer.EnableKeyword(ShaderGlobalKeywords.MainLightShadows);
				}
				MainLightShadowCasterPass.SetShadowParamsForEmptyShadowmap(rasterCommandBuffer);
				universalRenderingData.commandBuffer.SetGlobalTexture(MainLightShadowCasterPass.MainLightShadowConstantBuffer._MainLightShadowmapID, this.m_EmptyMainLightShadowmapTexture.nameID);
				return;
			}
			this.InitPassData(ref this.m_PassData, universalRenderingData, cameraData, lightData, shadowData);
			this.InitRendererLists(ref this.m_PassData, context, null, false);
			this.RenderMainLightCascadeShadowmap(rasterCommandBuffer, ref this.m_PassData, false);
			universalRenderingData.commandBuffer.SetGlobalTexture(MainLightShadowCasterPass.MainLightShadowConstantBuffer._MainLightShadowmapID, this.m_MainLightShadowmapTexture.nameID);
		}

		private void Clear()
		{
			for (int i = 0; i < this.m_MainLightShadowMatrices.Length; i++)
			{
				this.m_MainLightShadowMatrices[i] = Matrix4x4.identity;
			}
			for (int j = 0; j < this.m_CascadeSplitDistances.Length; j++)
			{
				this.m_CascadeSplitDistances[j] = new Vector4(0f, 0f, 0f, 0f);
			}
			for (int k = 0; k < this.m_CascadeSlices.Length; k++)
			{
				this.m_CascadeSlices[k].Clear();
			}
		}

		internal static void SetShadowParamsForEmptyShadowmap(RasterCommandBuffer rasterCommandBuffer)
		{
			rasterCommandBuffer.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._ShadowmapSize, MainLightShadowCasterPass.s_EmptyShadowmapSize);
			rasterCommandBuffer.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._ShadowParams, MainLightShadowCasterPass.s_EmptyShadowParams);
		}

		private void RenderMainLightCascadeShadowmap(RasterCommandBuffer cmd, ref MainLightShadowCasterPass.PassData data, bool isRenderGraph)
		{
			UniversalLightData lightData = data.lightData;
			int mainLightIndex = lightData.mainLightIndex;
			if (mainLightIndex == -1)
			{
				return;
			}
			VisibleLight visibleLight = lightData.visibleLights[mainLightIndex];
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.MainLightShadow)))
			{
				ShadowUtils.SetCameraPosition(cmd, data.cameraData.worldSpaceCameraPos);
				if (!isRenderGraph)
				{
					ShadowUtils.SetWorldToCameraAndCameraToWorldMatrices(cmd, data.cameraData.GetViewMatrix(0));
				}
				for (int i = 0; i < this.m_ShadowCasterCascadesCount; i++)
				{
					Vector4 shadowBias = ShadowUtils.GetShadowBias(ref visibleLight, mainLightIndex, data.shadowData, this.m_CascadeSlices[i].projectionMatrix, (float)this.m_CascadeSlices[i].resolution);
					ShadowUtils.SetupShadowCasterConstantBuffer(cmd, ref visibleLight, shadowBias);
					cmd.SetKeyword(ShaderGlobalKeywords.CastingPunctualLightShadow, false);
					RendererList rendererList = isRenderGraph ? data.shadowRendererListsHandle[i] : data.shadowRendererLists[i];
					ShadowUtils.RenderShadowSlice(cmd, ref this.m_CascadeSlices[i], ref rendererList, this.m_CascadeSlices[i].projectionMatrix, this.m_CascadeSlices[i].viewMatrix);
				}
				data.shadowData.isKeywordSoftShadowsEnabled = (visibleLight.light.shadows == LightShadows.Soft && data.shadowData.supportsSoftShadows);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadows, data.shadowData.mainLightShadowCascadesCount == 1);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowCascades, data.shadowData.mainLightShadowCascadesCount > 1);
				ShadowUtils.SetSoftShadowQualityShaderKeywords(cmd, data.shadowData);
				this.SetupMainLightShadowReceiverConstants(cmd, ref visibleLight, data.shadowData);
			}
		}

		private void SetupMainLightShadowReceiverConstants(RasterCommandBuffer cmd, ref VisibleLight shadowLight, UniversalShadowData shadowData)
		{
			Light light = shadowLight.light;
			bool softShadowsEnabled = shadowLight.light.shadows == LightShadows.Soft && shadowData.supportsSoftShadows;
			int shadowCasterCascadesCount = this.m_ShadowCasterCascadesCount;
			for (int i = 0; i < shadowCasterCascadesCount; i++)
			{
				this.m_MainLightShadowMatrices[i] = this.m_CascadeSlices[i].shadowTransform;
			}
			Matrix4x4 zero = Matrix4x4.zero;
			zero.m22 = (SystemInfo.usesReversedZBuffer ? 1f : 0f);
			for (int j = shadowCasterCascadesCount; j <= 4; j++)
			{
				this.m_MainLightShadowMatrices[j] = zero;
			}
			float num = 1f / (float)this.renderTargetWidth;
			float num2 = 1f / (float)this.renderTargetHeight;
			float num3 = 0.5f * num;
			float num4 = 0.5f * num2;
			float y = ShadowUtils.SoftShadowQualityToShaderProperty(light, softShadowsEnabled);
			float z;
			float w;
			ShadowUtils.GetScaleAndBiasForLinearDistanceFade(this.m_MaxShadowDistanceSq, this.m_CascadeBorder, out z, out w);
			cmd.SetGlobalMatrixArray(MainLightShadowCasterPass.MainLightShadowConstantBuffer._WorldToShadow, this.m_MainLightShadowMatrices);
			cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._ShadowParams, new Vector4(light.shadowStrength, y, z, w));
			if (this.m_ShadowCasterCascadesCount > 1)
			{
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._CascadeShadowSplitSpheres0, this.m_CascadeSplitDistances[0]);
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._CascadeShadowSplitSpheres1, this.m_CascadeSplitDistances[1]);
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._CascadeShadowSplitSpheres2, this.m_CascadeSplitDistances[2]);
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._CascadeShadowSplitSpheres3, this.m_CascadeSplitDistances[3]);
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._CascadeShadowSplitSphereRadii, new Vector4(this.m_CascadeSplitDistances[0].w * this.m_CascadeSplitDistances[0].w, this.m_CascadeSplitDistances[1].w * this.m_CascadeSplitDistances[1].w, this.m_CascadeSplitDistances[2].w * this.m_CascadeSplitDistances[2].w, this.m_CascadeSplitDistances[3].w * this.m_CascadeSplitDistances[3].w));
			}
			if (shadowData.supportsSoftShadows)
			{
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._ShadowOffset0, new Vector4(-num3, -num4, num3, -num4));
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._ShadowOffset1, new Vector4(-num3, num4, num3, num4));
				cmd.SetGlobalVector(MainLightShadowCasterPass.MainLightShadowConstantBuffer._ShadowmapSize, new Vector4(num, num2, (float)this.renderTargetWidth, (float)this.renderTargetHeight));
			}
		}

		private void InitPassData(ref MainLightShadowCasterPass.PassData passData, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, UniversalShadowData shadowData)
		{
			passData.pass = this;
			passData.emptyShadowmap = this.m_CreateEmptyShadowmap;
			passData.setKeywordForEmptyShadowmap = this.m_SetKeywordForEmptyShadowmap;
			passData.renderingData = renderingData;
			passData.cameraData = cameraData;
			passData.lightData = lightData;
			passData.shadowData = shadowData;
		}

		private void InitRendererLists(ref MainLightShadowCasterPass.PassData passData, ScriptableRenderContext context, RenderGraph renderGraph, bool useRenderGraph)
		{
			int mainLightIndex = passData.lightData.mainLightIndex;
			if (!this.m_CreateEmptyShadowmap && mainLightIndex != -1)
			{
				ShadowDrawingSettings shadowDrawingSettings = new ShadowDrawingSettings(passData.renderingData.cullResults, mainLightIndex)
				{
					useRenderingLayerMaskTest = UniversalRenderPipeline.asset.useRenderingLayers
				};
				for (int i = 0; i < this.m_ShadowCasterCascadesCount; i++)
				{
					if (useRenderGraph)
					{
						passData.shadowRendererListsHandle[i] = renderGraph.CreateShadowRendererList(ref shadowDrawingSettings);
					}
					else
					{
						passData.shadowRendererLists[i] = context.CreateShadowRendererList(ref shadowDrawingSettings);
					}
				}
			}
		}

		internal TextureHandle Render(RenderGraph graph, ContextContainer frameData)
		{
			UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();
			MainLightShadowCasterPass.PassData passData;
			TextureHandle textureHandle;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<MainLightShadowCasterPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\MainLightShadowCasterPass.cs", 468))
			{
				this.InitPassData(ref passData, renderingData, cameraData, lightData, shadowData);
				this.InitRendererLists(ref passData, default(ScriptableRenderContext), graph, true);
				if (!this.m_CreateEmptyShadowmap)
				{
					for (int i = 0; i < this.m_ShadowCasterCascadesCount; i++)
					{
						rasterRenderGraphBuilder.UseRendererList(passData.shadowRendererListsHandle[i]);
					}
					textureHandle = UniversalRenderer.CreateRenderGraphTexture(graph, this.m_MainLightShadowDescriptor, "_MainLightShadowmapTexture", true, ShadowUtils.m_ForceShadowPointSampling ? FilterMode.Point : FilterMode.Bilinear, TextureWrapMode.Clamp);
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(textureHandle, AccessFlags.Write);
				}
				else
				{
					textureHandle = graph.defaultResources.defaultShadowTexture;
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (textureHandle.IsValid())
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(textureHandle, MainLightShadowCasterPass.MainLightShadowConstantBuffer._MainLightShadowmapID);
				}
				rasterRenderGraphBuilder.SetRenderFunc<MainLightShadowCasterPass.PassData>(delegate(MainLightShadowCasterPass.PassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					if (!data.emptyShadowmap)
					{
						data.pass.RenderMainLightCascadeShadowmap(cmd, ref data, true);
						return;
					}
					if (data.setKeywordForEmptyShadowmap)
					{
						cmd.EnableKeyword(ShaderGlobalKeywords.MainLightShadows);
					}
					MainLightShadowCasterPass.SetShadowParamsForEmptyShadowmap(cmd);
				});
			}
			return textureHandle;
		}

		internal RTHandle m_MainLightShadowmapTexture;

		private int renderTargetWidth;

		private int renderTargetHeight;

		private int m_ShadowCasterCascadesCount;

		private bool m_CreateEmptyShadowmap;

		private bool m_SetKeywordForEmptyShadowmap;

		private bool m_EmptyShadowmapNeedsClear;

		private float m_CascadeBorder;

		private float m_MaxShadowDistanceSq;

		private MainLightShadowCasterPass.PassData m_PassData;

		private RTHandle m_EmptyMainLightShadowmapTexture;

		private RenderTextureDescriptor m_MainLightShadowDescriptor;

		private readonly Vector4[] m_CascadeSplitDistances;

		private readonly Matrix4x4[] m_MainLightShadowMatrices;

		private readonly ProfilingSampler m_ProfilingSetupSampler = new ProfilingSampler("Setup Main Shadowmap");

		private readonly ShadowSliceData[] m_CascadeSlices;

		private const int k_EmptyShadowMapDimensions = 1;

		private const int k_MaxCascades = 4;

		private const int k_ShadowmapBufferBits = 16;

		private const string k_MainLightShadowMapTextureName = "_MainLightShadowmapTexture";

		private const string k_EmptyMainLightShadowMapTextureName = "_EmptyMainLightShadowmapTexture";

		private static Vector4 s_EmptyShadowParams = new Vector4(0f, 0f, 1f, 0f);

		private static readonly Vector4 s_EmptyShadowmapSize = new Vector4(1f, 1f, 1f, 1f);

		private static class MainLightShadowConstantBuffer
		{
			public static readonly int _WorldToShadow = Shader.PropertyToID("_MainLightWorldToShadow");

			public static readonly int _ShadowParams = Shader.PropertyToID("_MainLightShadowParams");

			public static readonly int _CascadeShadowSplitSpheres0 = Shader.PropertyToID("_CascadeShadowSplitSpheres0");

			public static readonly int _CascadeShadowSplitSpheres1 = Shader.PropertyToID("_CascadeShadowSplitSpheres1");

			public static readonly int _CascadeShadowSplitSpheres2 = Shader.PropertyToID("_CascadeShadowSplitSpheres2");

			public static readonly int _CascadeShadowSplitSpheres3 = Shader.PropertyToID("_CascadeShadowSplitSpheres3");

			public static readonly int _CascadeShadowSplitSphereRadii = Shader.PropertyToID("_CascadeShadowSplitSphereRadii");

			public static readonly int _ShadowOffset0 = Shader.PropertyToID("_MainLightShadowOffset0");

			public static readonly int _ShadowOffset1 = Shader.PropertyToID("_MainLightShadowOffset1");

			public static readonly int _ShadowmapSize = Shader.PropertyToID("_MainLightShadowmapSize");

			public static readonly int _MainLightShadowmapID = Shader.PropertyToID("_MainLightShadowmapTexture");
		}

		private class PassData
		{
			internal bool emptyShadowmap;

			internal bool setKeywordForEmptyShadowmap;

			internal UniversalRenderingData renderingData;

			internal UniversalCameraData cameraData;

			internal UniversalLightData lightData;

			internal UniversalShadowData shadowData;

			internal MainLightShadowCasterPass pass;

			internal TextureHandle shadowmapTexture;

			internal readonly RendererList[] shadowRendererLists = new RendererList[4];

			internal readonly RendererListHandle[] shadowRendererListsHandle = new RendererListHandle[4];
		}
	}
}
