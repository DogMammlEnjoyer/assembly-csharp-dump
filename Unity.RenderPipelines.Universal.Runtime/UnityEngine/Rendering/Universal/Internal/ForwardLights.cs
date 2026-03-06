using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class ForwardLights
	{
		public ForwardLights() : this(ForwardLights.InitParams.Create())
		{
		}

		internal ForwardLights(ForwardLights.InitParams initParams)
		{
			this.m_UseStructuredBuffer = RenderingUtils.useStructuredBuffer;
			this.m_UseForwardPlus = initParams.forwardPlus;
			ForwardLights.LightConstantBuffer._MainLightPosition = Shader.PropertyToID("_MainLightPosition");
			ForwardLights.LightConstantBuffer._MainLightColor = Shader.PropertyToID("_MainLightColor");
			ForwardLights.LightConstantBuffer._MainLightOcclusionProbesChannel = Shader.PropertyToID("_MainLightOcclusionProbes");
			ForwardLights.LightConstantBuffer._MainLightLayerMask = Shader.PropertyToID("_MainLightLayerMask");
			ForwardLights.LightConstantBuffer._AdditionalLightsCount = Shader.PropertyToID("_AdditionalLightsCount");
			if (this.m_UseStructuredBuffer)
			{
				this.m_AdditionalLightsBufferId = Shader.PropertyToID("_AdditionalLightsBuffer");
				this.m_AdditionalLightsIndicesId = Shader.PropertyToID("_AdditionalLightsIndices");
			}
			else
			{
				ForwardLights.LightConstantBuffer._AdditionalLightsPosition = Shader.PropertyToID("_AdditionalLightsPosition");
				ForwardLights.LightConstantBuffer._AdditionalLightsColor = Shader.PropertyToID("_AdditionalLightsColor");
				ForwardLights.LightConstantBuffer._AdditionalLightsAttenuation = Shader.PropertyToID("_AdditionalLightsAttenuation");
				ForwardLights.LightConstantBuffer._AdditionalLightsSpotDir = Shader.PropertyToID("_AdditionalLightsSpotDir");
				ForwardLights.LightConstantBuffer._AdditionalLightOcclusionProbeChannel = Shader.PropertyToID("_AdditionalLightsOcclusionProbes");
				ForwardLights.LightConstantBuffer._AdditionalLightsLayerMasks = Shader.PropertyToID("_AdditionalLightsLayerMasks");
				int maxVisibleAdditionalLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
				this.m_AdditionalLightPositions = new Vector4[maxVisibleAdditionalLights];
				this.m_AdditionalLightColors = new Vector4[maxVisibleAdditionalLights];
				this.m_AdditionalLightAttenuations = new Vector4[maxVisibleAdditionalLights];
				this.m_AdditionalLightSpotDirections = new Vector4[maxVisibleAdditionalLights];
				this.m_AdditionalLightOcclusionProbeChannels = new Vector4[maxVisibleAdditionalLights];
				this.m_AdditionalLightsLayerMasks = new float[maxVisibleAdditionalLights];
			}
			if (this.m_UseForwardPlus)
			{
				this.CreateForwardPlusBuffers();
				this.m_ReflectionProbeManager = ReflectionProbeManager.Create();
			}
			this.m_LightCookieManager = initParams.lightCookieManager;
		}

		private void CreateForwardPlusBuffers()
		{
			this.m_ZBins = new NativeArray<uint>(UniversalRenderPipeline.maxZBinWords, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_ZBinsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Constant, UniversalRenderPipeline.maxZBinWords / 4, UnsafeUtility.SizeOf<float4>());
			this.m_ZBinsBuffer.name = "URP Z-Bin Buffer";
			this.m_TileMasks = new NativeArray<uint>(UniversalRenderPipeline.maxTileWords, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_TileMasksBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Constant, UniversalRenderPipeline.maxTileWords / 4, UnsafeUtility.SizeOf<float4>());
			this.m_TileMasksBuffer.name = "URP Tile Buffer";
		}

		internal ReflectionProbeManager reflectionProbeManager
		{
			get
			{
				return this.m_ReflectionProbeManager;
			}
		}

		private static int AlignByteCount(int count, int align)
		{
			return align * ((count + align - 1) / align);
		}

		private static void GetViewParams(bool isOrthographic, float4x4 viewToClip, out float viewPlaneBot, out float viewPlaneTop, out float4 viewToViewportScaleBias)
		{
			float2 @float = math.float2(viewToClip[0][0], viewToClip[1][1]);
			float2 float2 = math.rcp(@float);
			float2 float3 = isOrthographic ? (-math.float2(viewToClip[3][0], viewToClip[3][1])) : math.float2(viewToClip[2][0], viewToClip[2][1]);
			viewPlaneBot = float3.y * float2.y - float2.y;
			viewPlaneTop = float3.y * float2.y + float2.y;
			viewToViewportScaleBias = math.float4(@float * 0.5f, -float3 * 0.5f + 0.5f);
		}

		internal static JobHandle ScheduleClusteringJobs(NativeArray<VisibleLight> lights, NativeArray<VisibleReflectionProbe> probes, NativeArray<uint> zBins, NativeArray<uint> tileMasks, Fixed2<float4x4> worldToViews, Fixed2<float4x4> viewToClips, int viewCount, int2 screenResolution, float nearClipPlane, float farClipPlane, bool isOrthographic, out int localLightCount, out int directionalLightCount, out int binCount, out float zBinScale, out float zBinOffset, out int2 tileResolution, out int actualTileWidth, out int wordsPerTile)
		{
			localLightCount = lights.Length;
			int num = 0;
			while (num < localLightCount && lights[num].lightType == LightType.Directional)
			{
				num++;
			}
			localLightCount -= num;
			directionalLightCount = ((num > 0) ? (num - 1) : 0);
			NativeArray<VisibleLight> subArray = lights.GetSubArray(num, localLightCount);
			int num2 = math.min(probes.Length, UniversalRenderPipeline.maxVisibleReflectionProbes);
			int num3 = subArray.Length + num2;
			wordsPerTile = (num3 + 31) / 32;
			actualTileWidth = 4;
			do
			{
				actualTileWidth <<= 1;
				tileResolution = (screenResolution + actualTileWidth - 1) / actualTileWidth;
			}
			while (tileResolution.x * tileResolution.y * wordsPerTile * viewCount > UniversalRenderPipeline.maxTileWords);
			if (!isOrthographic)
			{
				zBinScale = (float)(UniversalRenderPipeline.maxZBinWords / viewCount) / ((math.log2(farClipPlane) - math.log2(nearClipPlane)) * (float)(2 + wordsPerTile));
				zBinOffset = -math.log2(nearClipPlane) * zBinScale;
				binCount = (int)(math.log2(farClipPlane) * zBinScale + zBinOffset);
			}
			else
			{
				zBinScale = (float)(UniversalRenderPipeline.maxZBinWords / viewCount) / ((farClipPlane - nearClipPlane) * (float)(2 + wordsPerTile));
				zBinOffset = -nearClipPlane * zBinScale;
				binCount = (int)(farClipPlane * zBinScale + zBinOffset);
			}
			binCount = Math.Max(binCount, 0);
			for (int i = 1; i < num2; i++)
			{
				VisibleReflectionProbe visibleReflectionProbe = probes[i];
				int num4 = i - 1;
				while (num4 >= 0 && ForwardLights.<ScheduleClusteringJobs>g__IsProbeGreater|40_0(probes[num4], visibleReflectionProbe))
				{
					probes[num4 + 1] = probes[num4];
					num4--;
				}
				probes[num4 + 1] = visibleReflectionProbe;
			}
			NativeArray<float2> minMaxZs = new NativeArray<float2>(num3 * viewCount, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			JobHandle dependency = new LightMinMaxZJob
			{
				worldToViews = worldToViews,
				lights = subArray,
				minMaxZs = minMaxZs.GetSubArray(0, localLightCount * viewCount)
			}.ScheduleParallel(localLightCount * viewCount, 32, default(JobHandle));
			JobHandle dependency2 = new ReflectionProbeMinMaxZJob
			{
				worldToViews = worldToViews,
				reflectionProbes = probes,
				minMaxZs = minMaxZs.GetSubArray(localLightCount * viewCount, num2 * viewCount)
			}.ScheduleParallel(num2 * viewCount, 32, dependency);
			int num5 = (binCount + 128 - 1) / 128;
			JobHandle inputDeps = new ZBinningJob
			{
				bins = zBins,
				minMaxZs = minMaxZs,
				zBinScale = zBinScale,
				zBinOffset = zBinOffset,
				binCount = binCount,
				wordsPerTile = wordsPerTile,
				lightCount = localLightCount,
				reflectionProbeCount = num2,
				batchCount = num5,
				viewCount = viewCount,
				isOrthographic = isOrthographic
			}.ScheduleParallel(num5 * viewCount, 1, dependency2);
			dependency2.Complete();
			float item;
			float item2;
			float4 item3;
			ForwardLights.GetViewParams(isOrthographic, viewToClips[0], out item, out item2, out item3);
			float item4;
			float item5;
			float4 item6;
			ForwardLights.GetViewParams(isOrthographic, viewToClips[1], out item4, out item5, out item6);
			int num6 = ForwardLights.AlignByteCount((1 + tileResolution.y) * UnsafeUtility.SizeOf<InclusiveRange>(), 128) / UnsafeUtility.SizeOf<InclusiveRange>();
			NativeArray<InclusiveRange> tileRanges = new NativeArray<InclusiveRange>(num6 * num3 * viewCount, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			JobHandle dependency3 = new TilingJob
			{
				lights = subArray,
				reflectionProbes = probes,
				tileRanges = tileRanges,
				itemsPerTile = num3,
				rangesPerItem = num6,
				worldToViews = worldToViews,
				tileScale = screenResolution / (float)actualTileWidth,
				tileScaleInv = (float)actualTileWidth / screenResolution,
				viewPlaneBottoms = new Fixed2<float>(item, item4),
				viewPlaneTops = new Fixed2<float>(item2, item5),
				viewToViewportScaleBiases = new Fixed2<float4>(item3, item6),
				tileCount = tileResolution,
				near = nearClipPlane,
				isOrthographic = isOrthographic
			}.ScheduleParallel(num3 * viewCount, 1, dependency2);
			JobHandle inputDeps2 = new TileRangeExpansionJob
			{
				tileRanges = tileRanges,
				tileMasks = tileMasks,
				rangesPerItem = num6,
				itemsPerTile = num3,
				wordsPerTile = wordsPerTile,
				tileResolution = tileResolution
			}.ScheduleParallel(tileResolution.y * viewCount, 1, dependency3);
			return JobHandle.CombineDependencies(minMaxZs.Dispose(inputDeps), tileRanges.Dispose(inputDeps2));
		}

		internal void PreSetup(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			if (this.m_UseForwardPlus)
			{
				using (new ProfilingScope(ForwardLights.m_ProfilingSamplerFPSetup))
				{
					if (!this.m_CullingHandle.IsCompleted)
					{
						throw new InvalidOperationException("Forward+ jobs have not completed yet.");
					}
					if (this.m_TileMasks.Length != UniversalRenderPipeline.maxTileWords)
					{
						this.m_ZBins.Dispose();
						this.m_ZBinsBuffer.Dispose();
						this.m_TileMasks.Dispose();
						this.m_TileMasksBuffer.Dispose();
						this.CreateForwardPlusBuffers();
					}
					else
					{
						UnsafeUtility.MemClear(this.m_ZBins.GetUnsafePtr<uint>(), (long)(this.m_ZBins.Length * 4));
						UnsafeUtility.MemClear(this.m_TileMasks.GetUnsafePtr<uint>(), (long)(this.m_TileMasks.Length * 4));
					}
					int num = (cameraData.xr.enabled && cameraData.xr.singlePassEnabled) ? 2 : 1;
					Fixed2<float4x4> worldToViews = new Fixed2<float4x4>(cameraData.GetViewMatrix(0), cameraData.GetViewMatrix(math.min(1, num - 1)));
					Fixed2<float4x4> viewToClips = new Fixed2<float4x4>(cameraData.GetProjectionMatrix(0), cameraData.GetProjectionMatrix(math.min(1, num - 1)));
					this.m_CullingHandle = ForwardLights.ScheduleClusteringJobs(lightData.visibleLights, renderingData.cullResults.visibleReflectionProbes, this.m_ZBins, this.m_TileMasks, worldToViews, viewToClips, num, math.int2(cameraData.pixelWidth, cameraData.pixelHeight), cameraData.camera.nearClipPlane, cameraData.camera.farClipPlane, cameraData.camera.orthographic, out this.m_LightCount, out this.m_DirectionalLightCount, out this.m_BinCount, out this.m_ZBinScale, out this.m_ZBinOffset, out this.m_TileResolution, out this.m_ActualTileWidth, out this.m_WordsPerTile);
					JobHandle.ScheduleBatchedJobs();
				}
			}
		}

		public unsafe void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			this.SetupLights(CommandBufferHelpers.GetUnsafeCommandBuffer(*renderingData.commandBuffer), renderingData2, cameraData, lightData);
		}

		internal void SetupRenderGraphLights(RenderGraph renderGraph, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			ForwardLights.SetupLightPassData setupLightPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<ForwardLights.SetupLightPassData>(ForwardLights.s_SetupForwardLights.name, out setupLightPassData, ForwardLights.s_SetupForwardLights, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\ForwardLights.cs", 448))
			{
				setupLightPassData.renderingData = renderingData;
				setupLightPassData.cameraData = cameraData;
				setupLightPassData.lightData = lightData;
				setupLightPassData.forwardLights = this;
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				unsafeRenderGraphBuilder.SetRenderFunc<ForwardLights.SetupLightPassData>(delegate(ForwardLights.SetupLightPassData data, UnsafeGraphContext rgContext)
				{
					data.forwardLights.SetupLights(rgContext.cmd, data.renderingData, data.cameraData, data.lightData);
				});
			}
		}

		internal void SetupLights(UnsafeCommandBuffer cmd, UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			int additionalLightsCount = lightData.additionalLightsCount;
			bool shadeAdditionalLightsPerVertex = lightData.shadeAdditionalLightsPerVertex;
			using (new ProfilingScope(ForwardLights.m_ProfilingSampler))
			{
				if (this.m_UseForwardPlus)
				{
					if (lightData.reflectionProbeAtlas)
					{
						this.m_ReflectionProbeManager.UpdateGpuData(CommandBufferHelpers.GetNativeCommandBuffer(cmd), ref renderingData.cullResults);
					}
					using (new ProfilingScope(ForwardLights.m_ProfilingSamplerFPComplete))
					{
						this.m_CullingHandle.Complete();
					}
					using (new ProfilingScope(ForwardLights.m_ProfilingSamplerFPUpload))
					{
						this.m_ZBinsBuffer.SetData<float4>(this.m_ZBins.Reinterpret<float4>(UnsafeUtility.SizeOf<uint>()));
						this.m_TileMasksBuffer.SetData<float4>(this.m_TileMasks.Reinterpret<float4>(UnsafeUtility.SizeOf<uint>()));
						cmd.SetGlobalConstantBuffer(this.m_ZBinsBuffer, "urp_ZBinBuffer", 0, UniversalRenderPipeline.maxZBinWords * 4);
						cmd.SetGlobalConstantBuffer(this.m_TileMasksBuffer, "urp_TileBuffer", 0, UniversalRenderPipeline.maxTileWords * 4);
					}
					cmd.SetGlobalVector("_FPParams0", math.float4(this.m_ZBinScale, this.m_ZBinOffset, (float)this.m_LightCount, (float)this.m_DirectionalLightCount));
					cmd.SetGlobalVector("_FPParams1", math.float4(cameraData.pixelRect.size / (float)this.m_ActualTileWidth, (float)this.m_TileResolution.x, (float)this.m_WordsPerTile));
					cmd.SetGlobalVector("_FPParams2", math.float4((float)this.m_BinCount, (float)(this.m_TileResolution.x * this.m_TileResolution.y), 0f, 0f));
				}
				this.SetupShaderLightConstants(cmd, ref renderingData.cullResults, lightData);
				bool flag = (cameraData.renderer.stripAdditionalLightOffVariants && lightData.supportsAdditionalLights) || additionalLightsCount > 0;
				cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightsVertex, flag && shadeAdditionalLightsPerVertex && !this.m_UseForwardPlus);
				cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightsPixel, flag && !shadeAdditionalLightsPerVertex && !this.m_UseForwardPlus);
				cmd.SetKeyword(ShaderGlobalKeywords.ClusterLightLoop, this.m_UseForwardPlus);
				cmd.SetKeyword(ShaderGlobalKeywords.ForwardPlus, this.m_UseForwardPlus);
				bool flag2 = lightData.supportsMixedLighting && this.m_MixedLightingSetup == MixedLightingSetup.ShadowMask;
				bool flag3 = flag2 && QualitySettings.shadowmaskMode == ShadowmaskMode.Shadowmask;
				bool flag4 = lightData.supportsMixedLighting && this.m_MixedLightingSetup == MixedLightingSetup.Subtractive;
				cmd.SetKeyword(ShaderGlobalKeywords.LightmapShadowMixing, flag4 || flag3);
				cmd.SetKeyword(ShaderGlobalKeywords.ShadowsShadowMask, flag2);
				cmd.SetKeyword(ShaderGlobalKeywords.MixedLightingSubtractive, flag4);
				cmd.SetKeyword(ShaderGlobalKeywords.ReflectionProbeBlending, lightData.reflectionProbeBlending);
				cmd.SetKeyword(ShaderGlobalKeywords.ReflectionProbeBoxProjection, lightData.reflectionProbeBoxProjection);
				cmd.SetKeyword(ShaderGlobalKeywords.ReflectionProbeAtlas, lightData.reflectionProbeAtlas);
				UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
				bool flag5 = asset != null && asset.lightProbeSystem == LightProbeSystem.ProbeVolumes;
				ProbeVolumeSHBands probeVolumeSHBands = asset.probeVolumeSHBands;
				cmd.SetKeyword(ShaderGlobalKeywords.ProbeVolumeL1, flag5 && probeVolumeSHBands == ProbeVolumeSHBands.SphericalHarmonicsL1);
				cmd.SetKeyword(ShaderGlobalKeywords.ProbeVolumeL2, flag5 && probeVolumeSHBands == ProbeVolumeSHBands.SphericalHarmonicsL2);
				ShEvalMode shEvalMode = PlatformAutoDetect.ShAutoDetect(asset.shEvalMode);
				cmd.SetKeyword(ShaderGlobalKeywords.EVALUATE_SH_MIXED, shEvalMode == ShEvalMode.Mixed);
				cmd.SetKeyword(ShaderGlobalKeywords.EVALUATE_SH_VERTEX, shEvalMode == ShEvalMode.PerVertex);
				VolumeStack stack = VolumeManager.instance.stack;
				bool flag6 = ProbeReferenceVolume.instance.UpdateShaderVariablesProbeVolumes(CommandBufferHelpers.GetNativeCommandBuffer(cmd), stack.GetComponent<ProbeVolumesOptions>(), cameraData.IsTemporalAAEnabled() ? Time.frameCount : 0, lightData.supportsLightLayers);
				cmd.SetGlobalInt("_EnableProbeVolumes", flag6 ? 1 : 0);
				cmd.SetKeyword(ShaderGlobalKeywords.LightLayers, lightData.supportsLightLayers && !CoreUtils.IsSceneLightingDisabled(cameraData.camera));
				if (this.m_LightCookieManager != null)
				{
					this.m_LightCookieManager.Setup(CommandBufferHelpers.GetNativeCommandBuffer(cmd), lightData);
				}
				else
				{
					cmd.SetKeyword(ShaderGlobalKeywords.LightCookies, false);
				}
				LightmapSamplingSettings lightmapSamplingSettings;
				if (GraphicsSettings.TryGetRenderPipelineSettings<LightmapSamplingSettings>(out lightmapSamplingSettings))
				{
					cmd.SetKeyword(ShaderGlobalKeywords.LIGHTMAP_BICUBIC_SAMPLING, lightmapSamplingSettings.useBicubicLightmapSampling);
				}
				else
				{
					cmd.SetKeyword(ShaderGlobalKeywords.LIGHTMAP_BICUBIC_SAMPLING, false);
				}
			}
		}

		internal void Cleanup()
		{
			if (this.m_UseForwardPlus)
			{
				this.m_CullingHandle.Complete();
				this.m_ZBins.Dispose();
				this.m_TileMasks.Dispose();
				this.m_ZBinsBuffer.Dispose();
				this.m_ZBinsBuffer = null;
				this.m_TileMasksBuffer.Dispose();
				this.m_TileMasksBuffer = null;
				this.m_ReflectionProbeManager.Dispose();
			}
			LightCookieManager lightCookieManager = this.m_LightCookieManager;
			if (lightCookieManager != null)
			{
				lightCookieManager.Dispose();
			}
			this.m_LightCookieManager = null;
		}

		private void InitializeLightConstants(NativeArray<VisibleLight> lights, int lightIndex, bool supportsLightLayers, out Vector4 lightPos, out Vector4 lightColor, out Vector4 lightAttenuation, out Vector4 lightSpotDir, out Vector4 lightOcclusionProbeChannel, out uint lightLayerMask, out bool isSubtractive)
		{
			UniversalRenderPipeline.InitializeLightConstants_Common(lights, lightIndex, out lightPos, out lightColor, out lightAttenuation, out lightSpotDir, out lightOcclusionProbeChannel);
			lightLayerMask = 0U;
			isSubtractive = false;
			if (lightIndex < 0)
			{
				return;
			}
			ref VisibleLight ptr = ref lights.UnsafeElementAtMutable(lightIndex);
			Light light = ptr.light;
			LightBakingOutput bakingOutput = light.bakingOutput;
			isSubtractive = (bakingOutput.isBaked && bakingOutput.lightmapBakeType == LightmapBakeType.Mixed && bakingOutput.mixedLightingMode == MixedLightingMode.Subtractive);
			if (light == null)
			{
				return;
			}
			if (bakingOutput.lightmapBakeType == LightmapBakeType.Mixed && ptr.light.shadows != LightShadows.None && this.m_MixedLightingSetup == MixedLightingSetup.None)
			{
				MixedLightingMode mixedLightingMode = bakingOutput.mixedLightingMode;
				if (mixedLightingMode != MixedLightingMode.Subtractive)
				{
					if (mixedLightingMode == MixedLightingMode.Shadowmask)
					{
						this.m_MixedLightingSetup = MixedLightingSetup.ShadowMask;
					}
				}
				else
				{
					this.m_MixedLightingSetup = MixedLightingSetup.Subtractive;
				}
			}
			if (supportsLightLayers)
			{
				UniversalAdditionalLightData universalAdditionalLightData = light.GetUniversalAdditionalLightData();
				lightLayerMask = RenderingLayerUtils.ToValidRenderingLayers(universalAdditionalLightData.renderingLayers);
			}
		}

		private void SetupShaderLightConstants(UnsafeCommandBuffer cmd, ref CullingResults cullResults, UniversalLightData lightData)
		{
			this.m_MixedLightingSetup = MixedLightingSetup.None;
			this.SetupMainLightConstants(cmd, lightData);
			this.SetupAdditionalLightConstants(cmd, ref cullResults, lightData);
		}

		private void SetupMainLightConstants(UnsafeCommandBuffer cmd, UniversalLightData lightData)
		{
			bool supportsLightLayers = lightData.supportsLightLayers;
			Vector4 value;
			Vector4 value2;
			Vector4 vector;
			Vector4 vector2;
			Vector4 value3;
			uint value4;
			bool flag;
			this.InitializeLightConstants(lightData.visibleLights, lightData.mainLightIndex, supportsLightLayers, out value, out value2, out vector, out vector2, out value3, out value4, out flag);
			value2.w = (flag ? 0f : 1f);
			cmd.SetGlobalVector(ForwardLights.LightConstantBuffer._MainLightPosition, value);
			cmd.SetGlobalVector(ForwardLights.LightConstantBuffer._MainLightColor, value2);
			cmd.SetGlobalVector(ForwardLights.LightConstantBuffer._MainLightOcclusionProbesChannel, value3);
			if (supportsLightLayers)
			{
				cmd.SetGlobalInt(ForwardLights.LightConstantBuffer._MainLightLayerMask, (int)value4);
			}
		}

		private void SetupAdditionalLightConstants(UnsafeCommandBuffer cmd, ref CullingResults cullResults, UniversalLightData lightData)
		{
			bool supportsLightLayers = lightData.supportsLightLayers;
			NativeArray<VisibleLight> visibleLights = lightData.visibleLights;
			int maxVisibleAdditionalLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
			int num = this.SetupPerObjectLightIndices(cullResults, lightData);
			if (num > 0)
			{
				if (this.m_UseStructuredBuffer)
				{
					NativeArray<ShaderInput.LightData> data = new NativeArray<ShaderInput.LightData>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
					int num2 = 0;
					int num3 = 0;
					while (num2 < visibleLights.Length && num3 < maxVisibleAdditionalLights)
					{
						if (lightData.mainLightIndex != num2)
						{
							ShaderInput.LightData value;
							bool flag;
							this.InitializeLightConstants(visibleLights, num2, supportsLightLayers, out value.position, out value.color, out value.attenuation, out value.spotDirection, out value.occlusionProbeChannels, out value.layerMask, out flag);
							data[num3] = value;
							num3++;
						}
						num2++;
					}
					ComputeBuffer lightDataBuffer = ShaderData.instance.GetLightDataBuffer(num);
					lightDataBuffer.SetData<ShaderInput.LightData>(data);
					int lightAndReflectionProbeIndexCount = cullResults.lightAndReflectionProbeIndexCount;
					ComputeBuffer lightIndicesBuffer = ShaderData.instance.GetLightIndicesBuffer(lightAndReflectionProbeIndexCount);
					cmd.SetGlobalBuffer(this.m_AdditionalLightsBufferId, lightDataBuffer);
					cmd.SetGlobalBuffer(this.m_AdditionalLightsIndicesId, lightIndicesBuffer);
					data.Dispose();
				}
				else
				{
					int num4 = 0;
					int num5 = 0;
					while (num4 < visibleLights.Length && num5 < maxVisibleAdditionalLights)
					{
						if (lightData.mainLightIndex != num4)
						{
							uint x;
							bool flag2;
							this.InitializeLightConstants(visibleLights, num4, supportsLightLayers, out this.m_AdditionalLightPositions[num5], out this.m_AdditionalLightColors[num5], out this.m_AdditionalLightAttenuations[num5], out this.m_AdditionalLightSpotDirections[num5], out this.m_AdditionalLightOcclusionProbeChannels[num5], out x, out flag2);
							if (supportsLightLayers)
							{
								this.m_AdditionalLightsLayerMasks[num5] = math.asfloat(x);
							}
							this.m_AdditionalLightColors[num5].w = (flag2 ? 1f : 0f);
							num5++;
						}
						num4++;
					}
					cmd.SetGlobalVectorArray(ForwardLights.LightConstantBuffer._AdditionalLightsPosition, this.m_AdditionalLightPositions);
					cmd.SetGlobalVectorArray(ForwardLights.LightConstantBuffer._AdditionalLightsColor, this.m_AdditionalLightColors);
					cmd.SetGlobalVectorArray(ForwardLights.LightConstantBuffer._AdditionalLightsAttenuation, this.m_AdditionalLightAttenuations);
					cmd.SetGlobalVectorArray(ForwardLights.LightConstantBuffer._AdditionalLightsSpotDir, this.m_AdditionalLightSpotDirections);
					cmd.SetGlobalVectorArray(ForwardLights.LightConstantBuffer._AdditionalLightOcclusionProbeChannel, this.m_AdditionalLightOcclusionProbeChannels);
					if (supportsLightLayers)
					{
						cmd.SetGlobalFloatArray(ForwardLights.LightConstantBuffer._AdditionalLightsLayerMasks, this.m_AdditionalLightsLayerMasks);
					}
				}
				cmd.SetGlobalVector(ForwardLights.LightConstantBuffer._AdditionalLightsCount, new Vector4((float)lightData.maxPerObjectAdditionalLightsCount, 0f, 0f, 0f));
				return;
			}
			cmd.SetGlobalVector(ForwardLights.LightConstantBuffer._AdditionalLightsCount, Vector4.zero);
		}

		private int SetupPerObjectLightIndices(CullingResults cullResults, UniversalLightData lightData)
		{
			if (lightData.additionalLightsCount == 0 || this.m_UseForwardPlus)
			{
				return lightData.additionalLightsCount;
			}
			NativeArray<int> lightIndexMap = cullResults.GetLightIndexMap(Allocator.Temp);
			int num = 0;
			int num2 = 0;
			int maxVisibleAdditionalLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
			int length = lightData.visibleLights.Length;
			int num3 = 0;
			while (num3 < length && num2 < maxVisibleAdditionalLights)
			{
				if (num3 == lightData.mainLightIndex)
				{
					lightIndexMap[num3] = -1;
					num++;
				}
				else
				{
					if (lightData.visibleLights[num3].lightType == LightType.Directional || lightData.visibleLights[num3].lightType == LightType.Spot || lightData.visibleLights[num3].lightType == LightType.Point)
					{
						ref NativeArray<int> ptr = ref lightIndexMap;
						int index = num3;
						ptr[index] -= num;
					}
					else
					{
						lightIndexMap[num3] = -1;
					}
					num2++;
				}
				num3++;
			}
			for (int i = num + num2; i < lightIndexMap.Length; i++)
			{
				lightIndexMap[i] = -1;
			}
			cullResults.SetLightIndexMap(lightIndexMap);
			if (this.m_UseStructuredBuffer && num2 > 0)
			{
				int lightAndReflectionProbeIndexCount = cullResults.lightAndReflectionProbeIndexCount;
				cullResults.FillLightAndReflectionProbeIndices(ShaderData.instance.GetLightIndicesBuffer(lightAndReflectionProbeIndexCount));
			}
			lightIndexMap.Dispose();
			return num2;
		}

		[CompilerGenerated]
		internal static bool <ScheduleClusteringJobs>g__IsProbeGreater|40_0(VisibleReflectionProbe probe, VisibleReflectionProbe otherProbe)
		{
			return probe.importance < otherProbe.importance || (probe.importance == otherProbe.importance && probe.bounds.extents.sqrMagnitude > otherProbe.bounds.extents.sqrMagnitude);
		}

		private int m_AdditionalLightsBufferId;

		private int m_AdditionalLightsIndicesId;

		private const string k_SetupLightConstants = "Setup Light Constants";

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Setup Light Constants");

		private static readonly ProfilingSampler m_ProfilingSamplerFPSetup = new ProfilingSampler("Forward+ Setup");

		private static readonly ProfilingSampler m_ProfilingSamplerFPComplete = new ProfilingSampler("Forward+ Complete");

		private static readonly ProfilingSampler m_ProfilingSamplerFPUpload = new ProfilingSampler("Forward+ Upload");

		private MixedLightingSetup m_MixedLightingSetup;

		private Vector4[] m_AdditionalLightPositions;

		private Vector4[] m_AdditionalLightColors;

		private Vector4[] m_AdditionalLightAttenuations;

		private Vector4[] m_AdditionalLightSpotDirections;

		private Vector4[] m_AdditionalLightOcclusionProbeChannels;

		private float[] m_AdditionalLightsLayerMasks;

		private bool m_UseStructuredBuffer;

		private bool m_UseForwardPlus;

		private int m_DirectionalLightCount;

		private int m_ActualTileWidth;

		private int2 m_TileResolution;

		private JobHandle m_CullingHandle;

		private NativeArray<uint> m_ZBins;

		private GraphicsBuffer m_ZBinsBuffer;

		private NativeArray<uint> m_TileMasks;

		private GraphicsBuffer m_TileMasksBuffer;

		private LightCookieManager m_LightCookieManager;

		private ReflectionProbeManager m_ReflectionProbeManager;

		private int m_WordsPerTile;

		private float m_ZBinScale;

		private float m_ZBinOffset;

		private int m_LightCount;

		private int m_BinCount;

		private static ProfilingSampler s_SetupForwardLights = new ProfilingSampler("Setup Forward Lights");

		private static class LightConstantBuffer
		{
			public static int _MainLightPosition;

			public static int _MainLightColor;

			public static int _MainLightOcclusionProbesChannel;

			public static int _MainLightLayerMask;

			public static int _AdditionalLightsCount;

			public static int _AdditionalLightsPosition;

			public static int _AdditionalLightsColor;

			public static int _AdditionalLightsAttenuation;

			public static int _AdditionalLightsSpotDir;

			public static int _AdditionalLightOcclusionProbeChannel;

			public static int _AdditionalLightsLayerMasks;
		}

		internal struct InitParams
		{
			internal static ForwardLights.InitParams Create()
			{
				LightCookieManager.Settings settings = LightCookieManager.Settings.Create();
				UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
				if (asset)
				{
					settings.atlas.format = asset.additionalLightsCookieFormat;
					settings.atlas.resolution = asset.additionalLightsCookieResolution;
				}
				ForwardLights.InitParams result;
				result.lightCookieManager = new LightCookieManager(ref settings);
				result.forwardPlus = false;
				return result;
			}

			public LightCookieManager lightCookieManager;

			public bool forwardPlus;
		}

		private class SetupLightPassData
		{
			internal UniversalRenderingData renderingData;

			internal UniversalCameraData cameraData;

			internal UniversalLightData lightData;

			internal ForwardLights forwardLights;
		}
	}
}
