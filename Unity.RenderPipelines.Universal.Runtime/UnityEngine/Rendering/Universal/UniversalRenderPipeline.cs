using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.RenderPipelines.Core.Runtime.Shared;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.VFX;

namespace UnityEngine.Rendering.Universal
{
	public sealed class UniversalRenderPipeline : RenderPipeline
	{
		public static float maxShadowBias
		{
			get
			{
				return 10f;
			}
		}

		public static float minRenderScale
		{
			get
			{
				return 0.1f;
			}
		}

		public static float maxRenderScale
		{
			get
			{
				return 2f;
			}
		}

		public static int maxNumIterationsEnclosingSphere
		{
			get
			{
				return 1000;
			}
		}

		public static int maxPerObjectLights
		{
			get
			{
				return 8;
			}
		}

		public static int maxVisibleAdditionalLights
		{
			get
			{
				bool isShaderAPIMobileDefined = PlatformAutoDetect.isShaderAPIMobileDefined;
				if (isShaderAPIMobileDefined && SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 && Graphics.minOpenGLESVersion <= OpenGLESVersion.OpenGLES30)
				{
					return 16;
				}
				if (!isShaderAPIMobileDefined && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3 && SystemInfo.graphicsDeviceType != GraphicsDeviceType.WebGPU)
				{
					return 256;
				}
				return 32;
			}
		}

		internal static int lightsPerTile
		{
			get
			{
				return (UniversalRenderPipeline.maxVisibleAdditionalLights + 31) / 32 * 32;
			}
		}

		internal static int maxZBinWords
		{
			get
			{
				return 4096;
			}
		}

		internal static int maxTileWords
		{
			get
			{
				return ((UniversalRenderPipeline.maxVisibleAdditionalLights <= 32) ? 1024 : 4096) * 4;
			}
		}

		internal static int maxVisibleReflectionProbes
		{
			get
			{
				return Math.Min(UniversalRenderPipeline.maxVisibleAdditionalLights, 64);
			}
		}

		internal UniversalRenderPipelineRuntimeTextures runtimeTextures { get; private set; }

		public override RenderPipelineGlobalSettings defaultSettings
		{
			get
			{
				return this.m_GlobalSettings;
			}
		}

		internal static bool canOptimizeScreenMSAASamples { get; private set; }

		internal static int startFrameScreenMSAASamples { get; private set; }

		public override string ToString()
		{
			UniversalRenderPipelineAsset universalRenderPipelineAsset = this.pipelineAsset;
			if (universalRenderPipelineAsset == null)
			{
				return null;
			}
			return universalRenderPipelineAsset.ToString();
		}

		public UniversalRenderPipeline(UniversalRenderPipelineAsset asset)
		{
			this.pipelineAsset = asset;
			this.m_GlobalSettings = RenderPipelineGlobalSettings<UniversalRenderPipelineGlobalSettings, UniversalRenderPipeline>.instance;
			this.runtimeTextures = GraphicsSettings.GetRenderPipelineSettings<UniversalRenderPipelineRuntimeTextures>();
			UniversalRenderPipelineRuntimeShaders renderPipelineSettings = GraphicsSettings.GetRenderPipelineSettings<UniversalRenderPipelineRuntimeShaders>();
			Blitter.Initialize(renderPipelineSettings.coreBlitPS, renderPipelineSettings.coreBlitColorAndDepthPS);
			UniversalRenderPipeline.SetSupportedRenderingFeatures(this.pipelineAsset);
			RTHandles.Initialize(Screen.width, Screen.height);
			ShaderGlobalKeywords.InitializeShaderGlobalKeywords();
			GraphicsSettings.useScriptableRenderPipelineBatching = asset.useSRPBatcher;
			if (((QualitySettings.antiAliasing > 0) ? QualitySettings.antiAliasing : 1) != asset.msaaSampleCount)
			{
				QualitySettings.antiAliasing = asset.msaaSampleCount;
			}
			URPDefaultVolumeProfileSettings renderPipelineSettings2 = GraphicsSettings.GetRenderPipelineSettings<URPDefaultVolumeProfileSettings>();
			VolumeManager.instance.Initialize(renderPipelineSettings2.volumeProfile, asset.volumeProfile);
			XRSystem.SetDisplayMSAASamples((MSAASamples)Mathf.Clamp(Mathf.NextPowerOfTwo(QualitySettings.antiAliasing), 1, 8));
			XRSystem.SetRenderScale(asset.renderScale);
			Lightmapping.SetDelegate(UniversalRenderPipeline.lightsDelegate);
			CameraCaptureBridge.enabled = true;
			RenderingUtils.ClearSystemInfoCache();
			DecalProjector.defaultMaterial = asset.decalMaterial;
			UniversalRenderPipeline.s_RenderGraph = new RenderGraph("URPRenderGraph");
			UniversalRenderPipeline.useRenderGraph = !GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>().enableRenderCompatibilityMode;
			Debug.Log("RenderGraph is now " + (UniversalRenderPipeline.useRenderGraph ? "enabled" : "disabled") + ".");
			UniversalRenderPipeline.s_RTHandlePool = new RTHandleResourcePool();
			DebugManager.instance.RefreshEditor();
			QualitySettings.enableLODCrossFade = asset.enableLODCrossFade;
			this.apvIsEnabled = (asset != null && asset.lightProbeSystem == LightProbeSystem.ProbeVolumes);
			SupportedRenderingFeatures.active.overridesLightProbeSystem = this.apvIsEnabled;
			SupportedRenderingFeatures.active.skyOcclusion = this.apvIsEnabled;
			if (this.apvIsEnabled)
			{
				ProbeReferenceVolume instance = ProbeReferenceVolume.instance;
				ProbeVolumeSystemParameters probeVolumeSystemParameters = default(ProbeVolumeSystemParameters);
				probeVolumeSystemParameters.memoryBudget = asset.probeVolumeMemoryBudget;
				probeVolumeSystemParameters.blendingMemoryBudget = asset.probeVolumeBlendingMemoryBudget;
				probeVolumeSystemParameters.shBands = asset.probeVolumeSHBands;
				probeVolumeSystemParameters.supportGPUStreaming = asset.supportProbeVolumeGPUStreaming;
				probeVolumeSystemParameters.supportDiskStreaming = asset.supportProbeVolumeDiskStreaming;
				probeVolumeSystemParameters.supportScenarios = asset.supportProbeVolumeScenarios;
				probeVolumeSystemParameters.supportScenarioBlending = asset.supportProbeVolumeScenarioBlending;
				probeVolumeSystemParameters.sceneData = this.m_GlobalSettings.GetOrCreateAPVSceneData();
				instance.Initialize(probeVolumeSystemParameters);
			}
			Vrs.InitializeResources();
		}

		protected override void Dispose(bool disposing)
		{
			Vrs.DisposeResources();
			if (this.apvIsEnabled)
			{
				ProbeReferenceVolume.instance.Cleanup();
			}
			Blitter.Cleanup();
			base.Dispose(disposing);
			this.pipelineAsset.DestroyRenderers();
			SupportedRenderingFeatures.active = new SupportedRenderingFeatures();
			ShaderData.instance.Dispose();
			XRSystem.Dispose();
			UniversalRenderPipeline.s_RenderGraph.Cleanup();
			UniversalRenderPipeline.s_RenderGraph = null;
			UniversalRenderPipeline.s_RTHandlePool.Cleanup();
			UniversalRenderPipeline.s_RTHandlePool = null;
			Lightmapping.ResetDelegate();
			CameraCaptureBridge.enabled = false;
			ConstantBuffer.ReleaseAll();
			VolumeManager.instance.Deinitialize();
			this.DisposeAdditionalCameraData();
			AdditionalLightsShadowAtlasLayout.ClearStaticCaches();
		}

		private void DisposeAdditionalCameraData()
		{
			Camera[] allCameras = Camera.allCameras;
			for (int i = 0; i < allCameras.Length; i++)
			{
				UniversalAdditionalCameraData universalAdditionalCameraData;
				if (allCameras[i].TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
				{
					universalAdditionalCameraData.historyManager.Dispose();
				}
			}
		}

		protected override void Render(ScriptableRenderContext renderContext, List<Camera> cameras)
		{
			this.SetHDRState(cameras);
			int count = cameras.Count;
			UniversalRenderPipeline.AdjustUIOverlayOwnership(count);
			UniversalRenderPipeline.SetupScreenMSAASamplesState(count);
			GPUResidentDrawer.ReinitializeIfNeeded();
			using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.UniversalRenderTotal)))
			{
				using (new UniversalRenderPipeline.ContextRenderingScope(renderContext, cameras))
				{
					GraphicsSettings.lightsUseLinearIntensity = (QualitySettings.activeColorSpace == ColorSpace.Linear);
					GraphicsSettings.lightsUseColorTemperature = true;
					this.SetupPerFrameShaderConstants();
					XRSystem.SetDisplayMSAASamples((MSAASamples)UniversalRenderPipeline.asset.msaaSampleCount);
					RTHandles.SetHardwareDynamicResolutionState(true);
					this.SortCameras(cameras);
					int lastBaseCameraIndex = this.GetLastBaseCameraIndex(cameras);
					for (int i = 0; i < count; i++)
					{
						Camera camera = cameras[i];
						bool isLastBaseCamera = i == lastBaseCameraIndex;
						if (UniversalRenderPipeline.IsGameCamera(camera))
						{
							UniversalRenderPipeline.RenderCameraStack(renderContext, camera, isLastBaseCamera);
						}
						else
						{
							using (new UniversalRenderPipeline.CameraRenderingScope(renderContext, camera))
							{
								VFXManager.PrepareCamera(camera);
								UniversalRenderPipeline.UpdateVolumeFramework(camera, null);
								UniversalRenderPipeline.RenderSingleCameraInternal(renderContext, camera, isLastBaseCamera);
							}
						}
					}
					UniversalRenderPipeline.s_RenderGraph.EndFrame();
					UniversalRenderPipeline.s_RTHandlePool.PurgeUnusedResources(Time.frameCount);
				}
			}
		}

		protected override bool IsRenderRequestSupported<RequestData>(Camera camera, RequestData data)
		{
			return data is RenderPipeline.StandardRequest || data is UniversalRenderPipeline.SingleCameraRequest;
		}

		protected override void ProcessRenderRequests<RequestData>(ScriptableRenderContext context, Camera camera, RequestData renderRequest)
		{
			RenderPipeline.StandardRequest standardRequest = renderRequest as RenderPipeline.StandardRequest;
			UniversalRenderPipeline.SingleCameraRequest singleCameraRequest = renderRequest as UniversalRenderPipeline.SingleCameraRequest;
			if (standardRequest == null && singleCameraRequest == null)
			{
				Debug.LogWarning("RenderRequest type: " + typeof(RequestData).FullName + " is either invalid or unsupported by the current pipeline");
				return;
			}
			RenderTexture renderTexture = (standardRequest != null) ? standardRequest.destination : singleCameraRequest.destination;
			if (renderTexture == null)
			{
				Debug.LogError("RenderRequest has no destination texture, set one before sending request");
				return;
			}
			int num = (standardRequest != null) ? standardRequest.mipLevel : singleCameraRequest.mipLevel;
			int num2 = (standardRequest != null) ? standardRequest.slice : singleCameraRequest.slice;
			int num3 = (int)((standardRequest != null) ? standardRequest.face : singleCameraRequest.face);
			RenderTexture targetTexture = camera.targetTexture;
			RenderTexture renderTexture2 = null;
			RenderTextureDescriptor desc = renderTexture.descriptor;
			if (renderTexture.dimension == TextureDimension.Cube)
			{
				desc = default(RenderTextureDescriptor);
			}
			desc.colorFormat = renderTexture.format;
			desc.volumeDepth = 1;
			desc.msaaSamples = renderTexture.descriptor.msaaSamples;
			desc.dimension = TextureDimension.Tex2D;
			desc.width = renderTexture.width / (int)Math.Pow(2.0, (double)num);
			desc.height = renderTexture.height / (int)Math.Pow(2.0, (double)num);
			desc.width = Mathf.Max(1, desc.width);
			desc.height = Mathf.Max(1, desc.height);
			if (renderTexture.dimension != TextureDimension.Tex2D || num != 0)
			{
				renderTexture2 = RenderTexture.GetTemporary(desc);
			}
			camera.targetTexture = (renderTexture2 ? renderTexture2 : renderTexture);
			if (standardRequest != null)
			{
				this.Render(context, new List<Camera>
				{
					camera
				});
			}
			else
			{
				List<Camera> list;
				using (ListPool<Camera>.Get(out list))
				{
					list.Add(camera);
					using (new UniversalRenderPipeline.ContextRenderingScope(context, list))
					{
						using (new UniversalRenderPipeline.CameraRenderingScope(context, camera))
						{
							UniversalAdditionalCameraData universalAdditionalCameraData;
							camera.gameObject.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
							UniversalRenderPipeline.RenderSingleCameraInternal(context, camera, ref universalAdditionalCameraData, true);
						}
					}
				}
			}
			if (renderTexture2)
			{
				bool flag = false;
				switch (renderTexture.dimension)
				{
				case TextureDimension.Tex2D:
					if ((SystemInfo.copyTextureSupport & CopyTextureSupport.Basic) != CopyTextureSupport.None)
					{
						flag = true;
						Graphics.CopyTexture(renderTexture2, 0, 0, renderTexture, 0, num);
					}
					break;
				case TextureDimension.Tex3D:
					if ((SystemInfo.copyTextureSupport & CopyTextureSupport.DifferentTypes) != CopyTextureSupport.None)
					{
						flag = true;
						Graphics.CopyTexture(renderTexture2, 0, 0, renderTexture, num2, num);
					}
					break;
				case TextureDimension.Cube:
					if ((SystemInfo.copyTextureSupport & CopyTextureSupport.DifferentTypes) != CopyTextureSupport.None)
					{
						flag = true;
						Graphics.CopyTexture(renderTexture2, 0, 0, renderTexture, num3, num);
					}
					break;
				case TextureDimension.Tex2DArray:
					if ((SystemInfo.copyTextureSupport & CopyTextureSupport.DifferentTypes) != CopyTextureSupport.None)
					{
						flag = true;
						Graphics.CopyTexture(renderTexture2, 0, 0, renderTexture, num2, num);
					}
					break;
				case TextureDimension.CubeArray:
					if ((SystemInfo.copyTextureSupport & CopyTextureSupport.DifferentTypes) != CopyTextureSupport.None)
					{
						flag = true;
						Graphics.CopyTexture(renderTexture2, 0, 0, renderTexture, num3 + num2 * 6, num);
					}
					break;
				}
				if (!flag)
				{
					Debug.LogError("RenderRequest cannot have destination texture of this format: " + Enum.GetName(typeof(TextureDimension), renderTexture.dimension));
				}
			}
			camera.targetTexture = targetTexture;
			Graphics.SetRenderTarget(targetTexture);
			RenderTexture.ReleaseTemporary(renderTexture2);
		}

		[Obsolete("RenderSingleCamera is obsolete, please use RenderPipeline.SubmitRenderRequest with UniversalRenderer.SingleCameraRequest as RequestData type")]
		public static void RenderSingleCamera(ScriptableRenderContext context, Camera camera)
		{
			UniversalRenderPipeline.RenderSingleCameraInternal(context, camera, true);
		}

		internal static void RenderSingleCameraInternal(ScriptableRenderContext context, Camera camera, bool isLastBaseCamera = true)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData = null;
			if (UniversalRenderPipeline.IsGameCamera(camera))
			{
				camera.gameObject.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
			}
			UniversalRenderPipeline.RenderSingleCameraInternal(context, camera, ref universalAdditionalCameraData, isLastBaseCamera);
		}

		internal static void RenderSingleCameraInternal(ScriptableRenderContext context, Camera camera, ref UniversalAdditionalCameraData additionalCameraData, bool isLastBaseCamera = true)
		{
			if (additionalCameraData != null && additionalCameraData.renderType != CameraRenderType.Base)
			{
				Debug.LogWarning("Only Base cameras can be rendered with standalone RenderSingleCamera. Camera will be skipped.");
				return;
			}
			if (camera.targetTexture.width == 0 || camera.targetTexture.height == 0 || camera.pixelWidth == 0 || camera.pixelHeight == 0)
			{
				Debug.LogWarning(string.Format("Camera '{0}' has an invalid render target size (width: {1}, height: {2}) or pixel dimensions (width: {3}, height: {4}). Camera will be skipped.", new object[]
				{
					camera.name,
					camera.targetTexture.width,
					camera.targetTexture.height,
					camera.pixelWidth,
					camera.pixelHeight
				}));
				return;
			}
			UniversalCameraData cameraData = UniversalRenderPipeline.CreateCameraData(UniversalRenderPipeline.GetRenderer(camera, additionalCameraData).frameData, camera, additionalCameraData);
			UniversalRenderPipeline.InitializeAdditionalCameraData(camera, additionalCameraData, true, isLastBaseCamera, cameraData);
			UniversalRenderPipeline.RenderSingleCamera(context, cameraData);
		}

		private static bool TryGetCullingParameters(UniversalCameraData cameraData, out ScriptableCullingParameters cullingParams)
		{
			if (cameraData.xr.enabled)
			{
				cullingParams = cameraData.xr.cullingParams;
				if (!cameraData.camera.usePhysicalProperties && !XRGraphicsAutomatedTests.enabled)
				{
					cameraData.camera.fieldOfView = 57.29578f * Mathf.Atan(1f / cullingParams.stereoProjectionMatrix.m11) * 2f;
				}
				return true;
			}
			return cameraData.camera.TryGetCullingParameters(false, out cullingParams);
		}

		private static void RenderSingleCamera(ScriptableRenderContext context, UniversalCameraData cameraData)
		{
			Camera camera = cameraData.camera;
			ScriptableRenderer renderer = cameraData.renderer;
			if (renderer == null)
			{
				Debug.LogWarning(string.Format("Trying to render {0} with an invalid renderer. Camera rendering will be skipped.", camera.name));
				return;
			}
			using (ContextContainer frameData = renderer.frameData)
			{
				ScriptableCullingParameters scriptableCullingParameters;
				if (UniversalRenderPipeline.TryGetCullingParameters(cameraData, out scriptableCullingParameters))
				{
					ScriptableRenderer.current = renderer;
					UniversalRenderPipeline.s_RenderGraph.nativeRenderPassesEnabled = renderer.supportsNativeRenderPassRendergraphCompiler;
					bool isSceneViewCamera = cameraData.isSceneViewCamera;
					CommandBuffer commandBuffer = CommandBufferPool.Get();
					CommandBuffer cmd = cameraData.xr.enabled ? null : commandBuffer;
					UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry cached = UniversalRenderPipeline.CameraMetadataCache.GetCached(camera);
					using (new ProfilingScope(cmd, cached.sampler))
					{
						renderer.Clear(cameraData.renderType);
						using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.Renderer.setupCullingParameters))
						{
							CameraData cameraData2 = new CameraData(frameData);
							renderer.OnPreCullRenderPasses(cameraData2);
							renderer.SetupCullingParameters(ref scriptableCullingParameters, ref cameraData2);
						}
						context.ExecuteCommandBuffer(commandBuffer);
						commandBuffer.Clear();
						UniversalRenderPipeline.SetupPerCameraShaderConstants(commandBuffer);
						ProbeVolumesOptions options = null;
						UniversalAdditionalCameraData universalAdditionalCameraData;
						if (camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
						{
							VolumeStack volumeStack = universalAdditionalCameraData.volumeStack;
							options = ((volumeStack != null) ? volumeStack.GetComponent<ProbeVolumesOptions>() : null);
						}
						bool flag = UniversalRenderPipeline.asset != null && UniversalRenderPipeline.asset.lightProbeSystem == LightProbeSystem.ProbeVolumes;
						ProbeReferenceVolume.instance.SetEnableStateFromSRP(flag);
						ProbeReferenceVolume.instance.SetVertexSamplingEnabled(UniversalRenderPipeline.asset.shEvalMode == ShEvalMode.PerVertex || UniversalRenderPipeline.asset.shEvalMode == ShEvalMode.Mixed);
						if (flag && ProbeReferenceVolume.instance.isInitialized)
						{
							ProbeReferenceVolume.instance.PerformPendingOperations();
							if (camera.cameraType != CameraType.Reflection && camera.cameraType != CameraType.Preview)
							{
								ProbeReferenceVolume.instance.UpdateCellStreaming(commandBuffer, camera, options);
							}
						}
						if (camera.cameraType == CameraType.Reflection || camera.cameraType == CameraType.Preview)
						{
							ScriptableRenderContext.EmitGeometryForCamera(camera);
						}
						if (flag)
						{
							ProbeReferenceVolume.instance.BindAPVRuntimeResources(commandBuffer, true);
						}
						ProbeReferenceVolume.instance.RenderDebug(camera, options, Texture2D.whiteTexture);
						if (universalAdditionalCameraData != null)
						{
							universalAdditionalCameraData.motionVectorsPersistentData.Update(cameraData);
						}
						if (cameraData.taaHistory != null)
						{
							UniversalRenderPipeline.UpdateTemporalAATargets(cameraData);
						}
						RTHandles.SetReferenceSize(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height);
						UniversalRenderingData universalRenderingData = frameData.Create<UniversalRenderingData>();
						universalRenderingData.cullResults = context.Cull(ref scriptableCullingParameters);
						GPUResidentDrawer.PostCullBeginCameraRendering(new RenderRequestBatcherContext
						{
							commandBuffer = commandBuffer
						});
						UniversalRenderer universalRenderer = cameraData.renderer as UniversalRenderer;
						RenderingMode? renderingMode = (universalRenderer != null) ? new RenderingMode?(universalRenderer.renderingModeActual) : null;
						UniversalLightData lightData;
						UniversalShadowData shadowData;
						using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.initializeRenderingData))
						{
							UniversalRenderPipeline.CreateUniversalResourceData(frameData);
							lightData = UniversalRenderPipeline.CreateLightData(frameData, UniversalRenderPipeline.asset, universalRenderingData.cullResults.visibleLights, renderingMode);
							shadowData = UniversalRenderPipeline.CreateShadowData(frameData, UniversalRenderPipeline.asset, renderingMode);
							UniversalRenderPipeline.CreatePostProcessingData(frameData, UniversalRenderPipeline.asset);
							UniversalRenderPipeline.CreateRenderingData(frameData, UniversalRenderPipeline.asset, commandBuffer, renderingMode, cameraData.renderer);
							UniversalRenderPipeline.CreateCullContextData(frameData, context);
						}
						RenderingData renderingData = new RenderingData(frameData);
						UniversalRenderPipeline.CheckAndApplyDebugSettings(ref renderingData);
						UniversalRenderPipeline.CreateShadowAtlasAndCullShadowCasters(lightData, shadowData, cameraData, ref universalRenderingData.cullResults, ref context);
						renderer.AddRenderPasses(ref renderingData);
						if (UniversalRenderPipeline.useRenderGraph)
						{
							UniversalRenderPipeline.RecordAndExecuteRenderGraph(UniversalRenderPipeline.s_RenderGraph, context, renderer, commandBuffer, cameraData.camera, cached.name);
							renderer.FinishRenderGraphRendering(commandBuffer);
						}
						else
						{
							using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.Renderer.setup))
							{
								renderer.Setup(context, ref renderingData);
							}
							renderer.Execute(context, ref renderingData);
						}
					}
					context.ExecuteCommandBuffer(commandBuffer);
					CommandBufferPool.Release(commandBuffer);
					using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.Context.submit))
					{
						if (!UniversalRenderPipeline.useRenderGraph && renderer.useRenderPassEnabled && !context.SubmitForRenderPassValidation())
						{
							renderer.useRenderPassEnabled = false;
							commandBuffer.SetKeyword(ShaderGlobalKeywords.RenderPassEnabled, false);
							Debug.LogWarning("Rendering command not supported inside a native RenderPass found. Falling back to non-RenderPass rendering path");
						}
						context.Submit();
					}
					ScriptableRenderer.current = null;
				}
			}
		}

		private static void CreateShadowAtlasAndCullShadowCasters(UniversalLightData lightData, UniversalShadowData shadowData, UniversalCameraData cameraData, ref CullingResults cullResults, ref ScriptableRenderContext context)
		{
			if (!shadowData.supportsMainLightShadows && !shadowData.supportsAdditionalLightShadows)
			{
				return;
			}
			if (shadowData.supportsMainLightShadows)
			{
				UniversalRenderPipeline.InitializeMainLightShadowResolution(shadowData);
			}
			if (shadowData.supportsAdditionalLightShadows)
			{
				shadowData.shadowAtlasLayout = UniversalRenderPipeline.BuildAdditionalLightsShadowAtlasLayout(lightData, shadowData, cameraData);
			}
			shadowData.visibleLightsShadowCullingInfos = ShadowCulling.CullShadowCasters(ref context, shadowData, ref shadowData.shadowAtlasLayout, ref cullResults);
		}

		private static void RenderCameraStack(ScriptableRenderContext context, Camera baseCamera, bool isLastBaseCamera)
		{
			using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RenderCameraStack)))
			{
				UniversalAdditionalCameraData universalAdditionalCameraData;
				baseCamera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
				if (!(universalAdditionalCameraData != null) || universalAdditionalCameraData.renderType != CameraRenderType.Overlay)
				{
					ScriptableRenderer renderer = UniversalRenderPipeline.GetRenderer(baseCamera, universalAdditionalCameraData);
					List<Camera> list = (renderer != null && renderer.SupportsCameraStackingType(CameraRenderType.Base)) ? ((universalAdditionalCameraData != null) ? universalAdditionalCameraData.cameraStack : null) : null;
					bool flag = universalAdditionalCameraData != null && universalAdditionalCameraData.renderPostProcessing;
					bool flag2 = UniversalRenderPipeline.HDROutputForMainDisplayIsActive();
					int num = -1;
					if (list != null)
					{
						Type type = renderer.GetType();
						bool flag3 = false;
						UniversalRenderPipeline.stackedOverlayCamerasRequireDepthForPostProcessing = false;
						for (int i = 0; i < list.Count; i++)
						{
							Camera camera = list[i];
							if (camera == null)
							{
								flag3 = true;
							}
							else if (camera.isActiveAndEnabled)
							{
								UniversalAdditionalCameraData universalAdditionalCameraData2;
								camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData2);
								ScriptableRenderer renderer2 = UniversalRenderPipeline.GetRenderer(camera, universalAdditionalCameraData2);
								Type type2 = renderer2.GetType();
								if (type2 != type)
								{
									Debug.LogWarning(string.Concat(new string[]
									{
										"Only cameras with compatible renderer types can be stacked. The camera: ",
										camera.name,
										" are using the renderer ",
										type2.Name,
										", but the base camera: ",
										baseCamera.name,
										" are using ",
										type.Name,
										". Will skip rendering"
									}));
								}
								else if ((renderer2.SupportedCameraStackingTypes() & 2) == 0)
								{
									Debug.LogWarning(string.Concat(new string[]
									{
										"The camera: ",
										camera.name,
										" is using a renderer of type ",
										renderer.GetType().Name,
										" which does not support Overlay cameras in it's current state."
									}));
								}
								else if (universalAdditionalCameraData2 == null || universalAdditionalCameraData2.renderType != CameraRenderType.Overlay)
								{
									Debug.LogWarning("Stack can only contain Overlay cameras. The camera: " + camera.name + " " + string.Format("has a type {0} that is not supported. Will skip rendering.", universalAdditionalCameraData2.renderType));
								}
								else
								{
									UniversalRenderPipeline.stackedOverlayCamerasRequireDepthForPostProcessing |= UniversalRenderPipeline.CheckPostProcessForDepth();
									flag |= universalAdditionalCameraData2.renderPostProcessing;
									num = i;
								}
							}
						}
						if (flag3)
						{
							universalAdditionalCameraData.UpdateCameraStack();
						}
					}
					bool flag4 = num != -1;
					bool flag5 = false;
					bool enableXR = universalAdditionalCameraData == null || universalAdditionalCameraData.allowXRRendering;
					XRLayout xrlayout = XRSystem.NewLayout();
					xrlayout.AddCamera(baseCamera, enableXR);
					foreach (ValueTuple<Camera, XRPass> valueTuple in xrlayout.GetActivePasses())
					{
						XRPass item = valueTuple.Item2;
						XRPassUniversal xrPass = item as XRPassUniversal;
						if (item.enabled)
						{
							flag5 = true;
							UniversalRenderPipeline.UpdateCameraStereoMatrices(baseCamera, item);
							float num2 = XRSystem.GetRenderViewportScale();
							if (XRSystem.GetDynamicResolutionScale() < 1f)
							{
								num2 = XRSystem.GetDynamicResolutionScale();
							}
							ScalableBufferManager.ResizeBuffers(num2, num2);
						}
						bool stackLastCameraOutputToHDR = false;
						VFXCameraXRSettings camXRSettings;
						using (new UniversalRenderPipeline.CameraRenderingScope(context, baseCamera))
						{
							UniversalRenderPipeline.UpdateVolumeFramework(baseCamera, universalAdditionalCameraData);
							UniversalCameraData universalCameraData = UniversalRenderPipeline.CreateCameraData(renderer.frameData, baseCamera, universalAdditionalCameraData);
							if (item.enabled)
							{
								universalCameraData.xr = item;
								UniversalRenderPipeline.UpdateCameraData(universalCameraData, item);
								xrlayout.ReconfigurePass(item, baseCamera);
								XRSystemUniversal.BeginLateLatching(baseCamera, xrPass);
							}
							UniversalRenderPipeline.InitializeAdditionalCameraData(baseCamera, universalAdditionalCameraData, !flag4, isLastBaseCamera, universalCameraData);
							camXRSettings.viewTotal = (universalCameraData.xr.enabled ? 2U : 1U);
							camXRSettings.viewCount = (uint)(universalCameraData.xr.enabled ? universalCameraData.xr.viewCount : 1);
							camXRSettings.viewOffset = (uint)universalCameraData.xr.multipassId;
							VFXManager.PrepareCamera(baseCamera, camXRSettings);
							universalCameraData.postProcessingRequiresDepthTexture |= UniversalRenderPipeline.stackedOverlayCamerasRequireDepthForPostProcessing;
							bool flag6 = flag2;
							if (item.enabled)
							{
								flag6 = item.isHDRDisplayOutputActive;
							}
							stackLastCameraOutputToHDR = (UniversalRenderPipeline.asset.supportsHDR && flag6 && baseCamera.targetTexture == null && (baseCamera.cameraType == CameraType.Game || baseCamera.cameraType == CameraType.VR) && universalCameraData.allowHDROutput);
							universalCameraData.stackAnyPostProcessingEnabled = flag;
							universalCameraData.stackLastCameraOutputToHDR = stackLastCameraOutputToHDR;
							UniversalRenderPipeline.RenderSingleCamera(context, universalCameraData);
						}
						if (item.enabled)
						{
							XRSystemUniversal.EndLateLatching(baseCamera, xrPass);
						}
						if (flag4)
						{
							for (int j = 0; j < list.Count; j++)
							{
								Camera camera2 = list[j];
								if (camera2.isActiveAndEnabled)
								{
									UniversalAdditionalCameraData universalAdditionalCameraData3;
									camera2.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData3);
									if (universalAdditionalCameraData3 != null)
									{
										UniversalCameraData universalCameraData2 = UniversalRenderPipeline.CreateCameraData(UniversalRenderPipeline.GetRenderer(camera2, universalAdditionalCameraData3).frameData, baseCamera, universalAdditionalCameraData);
										if (item.enabled)
										{
											universalCameraData2.xr = item;
											UniversalRenderPipeline.UpdateCameraData(universalCameraData2, item);
										}
										UniversalRenderPipeline.InitializeAdditionalCameraData(camera2, universalAdditionalCameraData3, false, isLastBaseCamera, universalCameraData2);
										universalCameraData2.camera = camera2;
										universalCameraData2.baseCamera = baseCamera;
										UniversalRenderPipeline.UpdateCameraStereoMatrices(universalAdditionalCameraData3.camera, item);
										using (new UniversalRenderPipeline.CameraRenderingScope(context, camera2))
										{
											VFXManager.PrepareCamera(camera2, camXRSettings);
											UniversalRenderPipeline.UpdateVolumeFramework(camera2, universalAdditionalCameraData3);
											bool resolveFinalTarget = j == num;
											UniversalRenderPipeline.InitializeAdditionalCameraData(camera2, universalAdditionalCameraData3, resolveFinalTarget, isLastBaseCamera, universalCameraData2);
											universalCameraData2.stackAnyPostProcessingEnabled = flag;
											universalCameraData2.stackLastCameraOutputToHDR = stackLastCameraOutputToHDR;
											xrlayout.ReconfigurePass(universalCameraData2.xr, camera2);
											UniversalRenderPipeline.RenderSingleCamera(context, universalCameraData2);
										}
									}
								}
							}
						}
					}
					if (flag5)
					{
						CommandBuffer commandBuffer = CommandBufferPool.Get();
						XRSystem.RenderMirrorView(commandBuffer, baseCamera);
						context.ExecuteCommandBuffer(commandBuffer);
						context.Submit();
						CommandBufferPool.Release(commandBuffer);
					}
					XRSystem.EndLayout();
				}
			}
		}

		private static void UpdateCameraData(UniversalCameraData baseCameraData, in XRPass xr)
		{
			Rect rect = baseCameraData.camera.rect;
			Rect viewport = xr.GetViewport(0);
			baseCameraData.pixelRect = new Rect(rect.x * viewport.width + viewport.x, rect.y * viewport.height + viewport.y, rect.width * viewport.width, rect.height * viewport.height);
			Rect pixelRect = baseCameraData.pixelRect;
			baseCameraData.pixelWidth = (int)Math.Round((double)(pixelRect.width + pixelRect.x)) - (int)Math.Round((double)pixelRect.x);
			baseCameraData.pixelHeight = (int)Math.Round((double)(pixelRect.height + pixelRect.y)) - (int)Math.Round((double)pixelRect.y);
			baseCameraData.aspectRatio = (float)baseCameraData.pixelWidth / (float)baseCameraData.pixelHeight;
			RenderTextureDescriptor cameraTargetDescriptor = baseCameraData.cameraTargetDescriptor;
			baseCameraData.cameraTargetDescriptor = xr.renderTargetDesc;
			if (baseCameraData.isHdrEnabled)
			{
				baseCameraData.cameraTargetDescriptor.graphicsFormat = cameraTargetDescriptor.graphicsFormat;
			}
			baseCameraData.cameraTargetDescriptor.msaaSamples = cameraTargetDescriptor.msaaSamples;
			if (baseCameraData.isDefaultViewport)
			{
				baseCameraData.cameraTargetDescriptor.useDynamicScale = true;
				return;
			}
			baseCameraData.cameraTargetDescriptor.width = baseCameraData.pixelWidth;
			baseCameraData.cameraTargetDescriptor.height = baseCameraData.pixelHeight;
			baseCameraData.cameraTargetDescriptor.useDynamicScale = false;
		}

		private static void UpdateVolumeFramework(Camera camera, UniversalAdditionalCameraData additionalCameraData)
		{
			using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.UpdateVolumeFramework)))
			{
				if (!(camera.cameraType == CameraType.SceneView | (additionalCameraData != null && additionalCameraData.requiresVolumeFrameworkUpdate)) && additionalCameraData)
				{
					if (additionalCameraData.volumeStack != null && !additionalCameraData.volumeStack.isValid)
					{
						camera.DestroyVolumeStack(additionalCameraData);
					}
					if (additionalCameraData.volumeStack == null)
					{
						camera.UpdateVolumeStack(additionalCameraData);
					}
					VolumeManager.instance.stack = additionalCameraData.volumeStack;
				}
				else
				{
					if (additionalCameraData && additionalCameraData.volumeStack != null)
					{
						camera.DestroyVolumeStack(additionalCameraData);
					}
					LayerMask layerMask;
					Transform trigger;
					camera.GetVolumeLayerMaskAndTrigger(additionalCameraData, out layerMask, out trigger);
					VolumeManager.instance.ResetMainStack();
					VolumeManager.instance.Update(trigger, layerMask);
				}
			}
		}

		private static bool CheckPostProcessForDepth(UniversalCameraData cameraData)
		{
			return cameraData.postProcessEnabled && ((cameraData.IsTemporalAAEnabled() && cameraData.renderType == CameraRenderType.Base) || UniversalRenderPipeline.CheckPostProcessForDepth());
		}

		private static bool CheckPostProcessForDepth()
		{
			VolumeStack stack = VolumeManager.instance.stack;
			return stack.GetComponent<DepthOfField>().IsActive() || stack.GetComponent<MotionBlur>().IsActive();
		}

		private static void SetSupportedRenderingFeatures(UniversalRenderPipelineAsset pipelineAsset)
		{
			SupportedRenderingFeatures.active.supportsHDR = pipelineAsset.supportsHDR;
			SupportedRenderingFeatures.active.rendersUIOverlay = true;
		}

		private static ScriptableRenderer GetRenderer(Camera camera, UniversalAdditionalCameraData additionalCameraData)
		{
			ScriptableRenderer scriptableRenderer = (additionalCameraData != null) ? additionalCameraData.scriptableRenderer : null;
			if (scriptableRenderer == null || camera.cameraType == CameraType.SceneView)
			{
				scriptableRenderer = UniversalRenderPipeline.asset.scriptableRenderer;
			}
			return scriptableRenderer;
		}

		private static UniversalCameraData CreateCameraData(ContextContainer frameData, Camera camera, UniversalAdditionalCameraData additionalCameraData)
		{
			UniversalCameraData result;
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.initializeCameraData))
			{
				ScriptableRenderer renderer = UniversalRenderPipeline.GetRenderer(camera, additionalCameraData);
				UniversalCameraData universalCameraData = frameData.Create<UniversalCameraData>();
				UniversalRenderPipeline.InitializeStackedCameraData(camera, additionalCameraData, universalCameraData);
				universalCameraData.camera = camera;
				universalCameraData.historyManager = ((additionalCameraData != null) ? additionalCameraData.historyManager : null);
				bool flag = renderer != null && renderer.supportedRenderingFeatures.msaa;
				int msaaSamples = 1;
				if (camera.allowMSAA && UniversalRenderPipeline.asset.msaaSampleCount > 1 && flag)
				{
					msaaSamples = ((camera.targetTexture != null) ? camera.targetTexture.antiAliasing : UniversalRenderPipeline.asset.msaaSampleCount);
				}
				if (universalCameraData.xrRendering && flag && camera.targetTexture == null)
				{
					msaaSamples = (int)XRSystem.GetDisplayMSAASamples();
				}
				bool preserveFramebufferAlpha = Graphics.preserveFramebufferAlpha;
				universalCameraData.hdrColorBufferPrecision = (UniversalRenderPipeline.asset ? UniversalRenderPipeline.asset.hdrColorBufferPrecision : HDRColorBufferPrecision._32Bits);
				universalCameraData.cameraTargetDescriptor = UniversalRenderPipeline.CreateRenderTextureDescriptor(camera, universalCameraData, universalCameraData.isHdrEnabled, universalCameraData.hdrColorBufferPrecision, msaaSamples, preserveFramebufferAlpha, universalCameraData.requiresOpaqueTexture);
				GraphicsFormatUtility.GetAlphaComponentCount(universalCameraData.cameraTargetDescriptor.graphicsFormat);
				universalCameraData.isAlphaOutputEnabled = GraphicsFormatUtility.HasAlphaChannel(universalCameraData.cameraTargetDescriptor.graphicsFormat);
				if (universalCameraData.camera.cameraType == CameraType.SceneView && CoreUtils.IsSceneFilteringEnabled())
				{
					universalCameraData.isAlphaOutputEnabled = true;
				}
				result = universalCameraData;
			}
			return result;
		}

		private static void InitializeStackedCameraData(Camera baseCamera, UniversalAdditionalCameraData baseAdditionalCameraData, UniversalCameraData cameraData)
		{
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.initializeStackedCameraData))
			{
				UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
				cameraData.targetTexture = baseCamera.targetTexture;
				cameraData.cameraType = baseCamera.cameraType;
				if (cameraData.isSceneViewCamera)
				{
					cameraData.volumeLayerMask = 1;
					cameraData.volumeTrigger = null;
					cameraData.isStopNaNEnabled = false;
					cameraData.isDitheringEnabled = false;
					cameraData.antialiasing = AntialiasingMode.None;
					cameraData.antialiasingQuality = AntialiasingQuality.High;
					cameraData.xrRendering = false;
					cameraData.allowHDROutput = false;
				}
				else if (baseAdditionalCameraData != null)
				{
					cameraData.volumeLayerMask = baseAdditionalCameraData.volumeLayerMask;
					cameraData.volumeTrigger = ((baseAdditionalCameraData.volumeTrigger == null) ? baseCamera.transform : baseAdditionalCameraData.volumeTrigger);
					cameraData.isStopNaNEnabled = (baseAdditionalCameraData.stopNaN && SystemInfo.graphicsShaderLevel >= 35);
					cameraData.isDitheringEnabled = baseAdditionalCameraData.dithering;
					cameraData.antialiasing = baseAdditionalCameraData.antialiasing;
					cameraData.antialiasingQuality = baseAdditionalCameraData.antialiasingQuality;
					cameraData.xrRendering = (baseAdditionalCameraData.allowXRRendering && XRSystem.displayActive);
					cameraData.allowHDROutput = baseAdditionalCameraData.allowHDROutput;
				}
				else
				{
					cameraData.volumeLayerMask = 1;
					cameraData.volumeTrigger = null;
					cameraData.isStopNaNEnabled = false;
					cameraData.isDitheringEnabled = false;
					cameraData.antialiasing = AntialiasingMode.None;
					cameraData.antialiasingQuality = AntialiasingQuality.High;
					cameraData.xrRendering = XRSystem.displayActive;
					cameraData.allowHDROutput = true;
				}
				cameraData.isHdrEnabled = (baseCamera.allowHDR && asset.supportsHDR);
				cameraData.allowHDROutput &= asset.supportsHDR;
				Rect rect = baseCamera.rect;
				cameraData.pixelRect = baseCamera.pixelRect;
				cameraData.pixelWidth = baseCamera.pixelWidth;
				cameraData.pixelHeight = baseCamera.pixelHeight;
				cameraData.aspectRatio = (float)cameraData.pixelWidth / (float)cameraData.pixelHeight;
				cameraData.isDefaultViewport = (Math.Abs(rect.x) <= 0f && Math.Abs(rect.y) <= 0f && Math.Abs(rect.width) >= 1f && Math.Abs(rect.height) >= 1f);
				bool flag = cameraData.cameraType == CameraType.SceneView || cameraData.cameraType == CameraType.Preview || cameraData.cameraType == CameraType.Reflection;
				cameraData.renderScale = ((Mathf.Abs(1f - asset.renderScale) < 0.05f || flag) ? 1f : asset.renderScale);
				RenderGraphSettings renderGraphSettings;
				bool enableRenderGraph = GraphicsSettings.TryGetRenderPipelineSettings<RenderGraphSettings>(out renderGraphSettings) && !renderGraphSettings.enableRenderCompatibilityMode;
				cameraData.upscalingFilter = UniversalRenderPipeline.ResolveUpscalingFilterSelection(new Vector2((float)cameraData.pixelWidth, (float)cameraData.pixelHeight), cameraData.renderScale, asset.upscalingFilter, enableRenderGraph);
				if (cameraData.renderScale > 1f)
				{
					cameraData.imageScalingMode = ImageScalingMode.Downscaling;
				}
				else if (cameraData.renderScale < 1f || (!flag && (cameraData.upscalingFilter == ImageUpscalingFilter.FSR || cameraData.upscalingFilter == ImageUpscalingFilter.STP)))
				{
					cameraData.imageScalingMode = ImageScalingMode.Upscaling;
					if (cameraData.upscalingFilter == ImageUpscalingFilter.STP)
					{
						cameraData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
					}
				}
				else
				{
					cameraData.imageScalingMode = ImageScalingMode.None;
				}
				cameraData.fsrOverrideSharpness = asset.fsrOverrideSharpness;
				cameraData.fsrSharpness = asset.fsrSharpness;
				cameraData.xr = XRSystem.emptyPass;
				XRSystem.SetRenderScale(cameraData.renderScale);
				SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
				SortingCriteria sortingCriteria2 = SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder;
				bool hasHiddenSurfaceRemovalOnGPU = SystemInfo.hasHiddenSurfaceRemovalOnGPU;
				cameraData.defaultOpaqueSortFlags = (((baseCamera.opaqueSortMode == OpaqueSortMode.Default && hasHiddenSurfaceRemovalOnGPU) || baseCamera.opaqueSortMode == OpaqueSortMode.NoDistanceSort) ? sortingCriteria2 : sortingCriteria);
				cameraData.captureActions = CameraCaptureBridge.GetCachedCaptureActionsEnumerator(baseCamera);
			}
		}

		private static void InitializeAdditionalCameraData(Camera camera, UniversalAdditionalCameraData additionalCameraData, bool resolveFinalTarget, bool isLastBaseCamera, UniversalCameraData cameraData)
		{
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.initializeAdditionalCameraData))
			{
				ScriptableRenderer renderer = UniversalRenderPipeline.GetRenderer(camera, additionalCameraData);
				UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
				bool flag = asset.supportsMainLightShadows || asset.supportsAdditionalLightShadows;
				cameraData.maxShadowDistance = Mathf.Min(asset.shadowDistance, camera.farClipPlane);
				cameraData.maxShadowDistance = ((flag && cameraData.maxShadowDistance >= camera.nearClipPlane) ? cameraData.maxShadowDistance : 0f);
				if (cameraData.isSceneViewCamera)
				{
					cameraData.renderType = CameraRenderType.Base;
					cameraData.clearDepth = true;
					cameraData.postProcessEnabled = CoreUtils.ArePostProcessesEnabled(camera);
					cameraData.requiresDepthTexture = asset.supportsCameraDepthTexture;
					cameraData.requiresOpaqueTexture = asset.supportsCameraOpaqueTexture;
					cameraData.useScreenCoordOverride = false;
					cameraData.screenSizeOverride = cameraData.pixelRect.size;
					cameraData.screenCoordScaleBias = Vector2.one;
				}
				else if (additionalCameraData != null)
				{
					cameraData.renderType = additionalCameraData.renderType;
					cameraData.clearDepth = (additionalCameraData.renderType == CameraRenderType.Base || additionalCameraData.clearDepth);
					cameraData.postProcessEnabled = additionalCameraData.renderPostProcessing;
					cameraData.maxShadowDistance = (additionalCameraData.renderShadows ? cameraData.maxShadowDistance : 0f);
					cameraData.requiresDepthTexture = additionalCameraData.requiresDepthTexture;
					cameraData.requiresOpaqueTexture = additionalCameraData.requiresColorTexture;
					cameraData.useScreenCoordOverride = additionalCameraData.useScreenCoordOverride;
					cameraData.screenSizeOverride = additionalCameraData.screenSizeOverride;
					cameraData.screenCoordScaleBias = additionalCameraData.screenCoordScaleBias;
				}
				else
				{
					cameraData.renderType = CameraRenderType.Base;
					cameraData.clearDepth = true;
					cameraData.postProcessEnabled = false;
					cameraData.requiresDepthTexture = asset.supportsCameraDepthTexture;
					cameraData.requiresOpaqueTexture = asset.supportsCameraOpaqueTexture;
					cameraData.useScreenCoordOverride = false;
					cameraData.screenSizeOverride = cameraData.pixelRect.size;
					cameraData.screenCoordScaleBias = Vector2.one;
				}
				cameraData.renderer = renderer;
				cameraData.postProcessingRequiresDepthTexture = UniversalRenderPipeline.CheckPostProcessForDepth(cameraData);
				cameraData.resolveFinalTarget = resolveFinalTarget;
				cameraData.isLastBaseCamera = isLastBaseCamera;
				bool useGPUOcclusionCulling;
				if (GPUResidentDrawer.IsInstanceOcclusionCullingEnabled() && renderer.supportsGPUOcclusion)
				{
					CameraType cameraType = camera.cameraType;
					useGPUOcclusionCulling = (cameraType == CameraType.SceneView || cameraType == CameraType.Game || cameraType == CameraType.Preview);
				}
				else
				{
					useGPUOcclusionCulling = false;
				}
				cameraData.useGPUOcclusionCulling = useGPUOcclusionCulling;
				cameraData.requiresDepthTexture |= cameraData.useGPUOcclusionCulling;
				bool flag2 = cameraData.renderType == CameraRenderType.Overlay;
				if (flag2)
				{
					cameraData.requiresOpaqueTexture = false;
				}
				if (additionalCameraData != null)
				{
					UniversalRenderPipeline.UpdateTemporalAAData(cameraData, additionalCameraData);
				}
				Matrix4x4 projectionMatrix = camera.projectionMatrix;
				if (flag2 && !camera.orthographic && cameraData.pixelRect != camera.pixelRect)
				{
					float m = camera.projectionMatrix.m00 * camera.aspect / cameraData.aspectRatio;
					projectionMatrix.m00 = m;
				}
				UniversalRenderPipeline.ApplyTaaRenderingDebugOverrides(ref cameraData.taaSettings);
				TemporalAA.JitterFunc jitterFunc = cameraData.IsSTPEnabled() ? StpUtils.s_JitterFunc : TemporalAA.s_JitterFunc;
				Matrix4x4 jitterMatrix = TemporalAA.CalculateJitterMatrix(cameraData, jitterFunc);
				cameraData.SetViewProjectionAndJitterMatrix(camera.worldToCameraMatrix, projectionMatrix, jitterMatrix);
				cameraData.worldSpaceCameraPos = camera.transform.position;
				Color backgroundColor = camera.backgroundColor;
				cameraData.backgroundColor = CoreUtils.ConvertSRGBToActiveColorSpace(backgroundColor);
				cameraData.stackAnyPostProcessingEnabled = cameraData.postProcessEnabled;
				cameraData.stackLastCameraOutputToHDR = cameraData.isHDROutputActive;
				bool flag3 = !cameraData.postProcessEnabled || (cameraData.postProcessEnabled && asset.allowPostProcessAlphaOutput);
				cameraData.isAlphaOutputEnabled = (cameraData.isAlphaOutputEnabled && flag3);
			}
		}

		private static UniversalRenderingData CreateRenderingData(ContextContainer frameData, UniversalRenderPipelineAsset settings, CommandBuffer cmd, RenderingMode? renderingMode, ScriptableRenderer renderer)
		{
			UniversalLightData universalLightData = frameData.Get<UniversalLightData>();
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			universalRenderingData.supportsDynamicBatching = settings.supportsDynamicBatching;
			universalRenderingData.perObjectData = UniversalRenderPipeline.GetPerObjectLightFlags(universalLightData, settings, renderingMode);
			if (UniversalRenderPipeline.useRenderGraph)
			{
				universalRenderingData.m_CommandBuffer = null;
			}
			else
			{
				universalRenderingData.m_CommandBuffer = cmd;
			}
			UniversalRenderer universalRenderer = renderer as UniversalRenderer;
			if (universalRenderer != null)
			{
				universalRenderingData.renderingMode = universalRenderer.renderingModeActual;
				universalRenderingData.prepassLayerMask = universalRenderer.prepassLayerMask;
				universalRenderingData.opaqueLayerMask = universalRenderer.opaqueLayerMask;
				universalRenderingData.transparentLayerMask = universalRenderer.transparentLayerMask;
			}
			universalRenderingData.stencilLodCrossFadeEnabled = (settings.enableLODCrossFade && settings.lodCrossFadeDitheringType == LODCrossFadeDitheringType.Stencil);
			return universalRenderingData;
		}

		private static UniversalShadowData CreateShadowData(ContextContainer frameData, UniversalRenderPipelineAsset urpAsset, RenderingMode? renderingMode)
		{
			UniversalShadowData result;
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.initializeShadowData))
			{
				UniversalShadowData universalShadowData = frameData.Create<UniversalShadowData>();
				UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
				UniversalLightData universalLightData = frameData.Get<UniversalLightData>();
				UniversalRenderPipeline.m_ShadowBiasData.Clear();
				UniversalRenderPipeline.m_ShadowResolutionData.Clear();
				universalShadowData.shadowmapDepthBufferBits = 16;
				universalShadowData.mainLightShadowCascadeBorder = urpAsset.cascadeBorder;
				universalShadowData.mainLightShadowCascadesCount = urpAsset.shadowCascadeCount;
				universalShadowData.mainLightShadowCascadesSplit = UniversalRenderPipeline.GetMainLightCascadeSplit(universalShadowData.mainLightShadowCascadesCount, urpAsset);
				universalShadowData.mainLightShadowmapWidth = urpAsset.mainLightShadowmapResolution;
				universalShadowData.mainLightShadowmapHeight = urpAsset.mainLightShadowmapResolution;
				universalShadowData.additionalLightsShadowmapWidth = (universalShadowData.additionalLightsShadowmapHeight = urpAsset.additionalLightsShadowmapResolution);
				universalShadowData.isKeywordAdditionalLightShadowsEnabled = false;
				universalShadowData.isKeywordSoftShadowsEnabled = false;
				universalShadowData.mainLightShadowResolution = 0;
				universalShadowData.mainLightRenderTargetWidth = 0;
				universalShadowData.mainLightRenderTargetHeight = 0;
				universalShadowData.shadowAtlasLayout = default(AdditionalLightsShadowAtlasLayout);
				universalShadowData.visibleLightsShadowCullingInfos = default(NativeArray<URPLightShadowCullingInfos>);
				int mainLightIndex = universalLightData.mainLightIndex;
				NativeArray<VisibleLight> visibleLights = universalLightData.visibleLights;
				bool flag = universalCameraData.maxShadowDistance > 0f;
				universalShadowData.mainLightShadowsEnabled = (urpAsset.supportsMainLightShadows && urpAsset.mainLightRenderingMode == LightRenderingMode.PerPixel);
				universalShadowData.supportsMainLightShadows = (SystemInfo.supportsShadows && universalShadowData.mainLightShadowsEnabled && flag);
				bool flag2 = renderingMode != null && renderingMode.Value == RenderingMode.ForwardPlus;
				universalShadowData.additionalLightShadowsEnabled = (urpAsset.supportsAdditionalLightShadows && (urpAsset.additionalLightsRenderingMode == LightRenderingMode.PerPixel || flag2));
				universalShadowData.supportsAdditionalLightShadows = (SystemInfo.supportsShadows && universalShadowData.additionalLightShadowsEnabled && !universalLightData.shadeAdditionalLightsPerVertex && flag);
				if (!universalShadowData.supportsMainLightShadows && !universalShadowData.supportsAdditionalLightShadows)
				{
					result = universalShadowData;
				}
				else
				{
					universalShadowData.supportsMainLightShadows &= (mainLightIndex != -1 && visibleLights[mainLightIndex].light != null && visibleLights[mainLightIndex].light.shadows > LightShadows.None);
					if (universalShadowData.supportsAdditionalLightShadows)
					{
						bool flag3 = false;
						for (int i = 0; i < visibleLights.Length; i++)
						{
							if (i != mainLightIndex)
							{
								ref VisibleLight ptr = ref visibleLights.UnsafeElementAtMutable(i);
								if (ptr.lightType == LightType.Spot || ptr.lightType == LightType.Point)
								{
									Light light = ptr.light;
									if (!(light == null) && light.shadows != LightShadows.None)
									{
										flag3 = true;
										break;
									}
								}
							}
						}
						universalShadowData.supportsAdditionalLightShadows = flag3;
					}
					if (!universalShadowData.supportsMainLightShadows && !universalShadowData.supportsAdditionalLightShadows)
					{
						result = universalShadowData;
					}
					else
					{
						for (int j = 0; j < visibleLights.Length; j++)
						{
							if (!universalShadowData.supportsMainLightShadows && j == mainLightIndex)
							{
								UniversalRenderPipeline.m_ShadowBiasData.Add(Vector4.zero);
								UniversalRenderPipeline.m_ShadowResolutionData.Add(0);
							}
							else if (!universalShadowData.supportsAdditionalLightShadows && j != mainLightIndex)
							{
								UniversalRenderPipeline.m_ShadowBiasData.Add(Vector4.zero);
								UniversalRenderPipeline.m_ShadowResolutionData.Add(0);
							}
							else
							{
								Light light2 = visibleLights.UnsafeElementAtMutable(j).light;
								UniversalAdditionalLightData universalAdditionalLightData = null;
								if (light2 != null)
								{
									light2.gameObject.TryGetComponent<UniversalAdditionalLightData>(out universalAdditionalLightData);
								}
								if (universalAdditionalLightData && !universalAdditionalLightData.usePipelineSettings)
								{
									UniversalRenderPipeline.m_ShadowBiasData.Add(new Vector4(light2.shadowBias, light2.shadowNormalBias, 0f, 0f));
								}
								else
								{
									UniversalRenderPipeline.m_ShadowBiasData.Add(new Vector4(urpAsset.shadowDepthBias, urpAsset.shadowNormalBias, 0f, 0f));
								}
								if (universalAdditionalLightData && universalAdditionalLightData.additionalLightsShadowResolutionTier == UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierCustom)
								{
									UniversalRenderPipeline.m_ShadowResolutionData.Add((int)light2.shadowResolution);
								}
								else if (universalAdditionalLightData && universalAdditionalLightData.additionalLightsShadowResolutionTier != UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierCustom)
								{
									int additionalLightsShadowResolutionTier = Mathf.Clamp(universalAdditionalLightData.additionalLightsShadowResolutionTier, UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow, UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierHigh);
									UniversalRenderPipeline.m_ShadowResolutionData.Add(urpAsset.GetAdditionalLightsShadowResolution(additionalLightsShadowResolutionTier));
								}
								else
								{
									UniversalRenderPipeline.m_ShadowResolutionData.Add(urpAsset.GetAdditionalLightsShadowResolution(UniversalAdditionalLightData.AdditionalLightsShadowDefaultResolutionTier));
								}
							}
						}
						universalShadowData.bias = UniversalRenderPipeline.m_ShadowBiasData;
						universalShadowData.resolution = UniversalRenderPipeline.m_ShadowResolutionData;
						universalShadowData.supportsSoftShadows = (urpAsset.supportsSoftShadows && (universalShadowData.supportsMainLightShadows || universalShadowData.supportsAdditionalLightShadows));
						result = universalShadowData;
					}
				}
			}
			return result;
		}

		private static CullContextData CreateCullContextData(ContextContainer frameData, ScriptableRenderContext context)
		{
			CullContextData cullContextData = frameData.Create<CullContextData>();
			cullContextData.SetRenderContext(context);
			return cullContextData;
		}

		private static Vector3 GetMainLightCascadeSplit(int mainLightShadowCascadesCount, UniversalRenderPipelineAsset urpAsset)
		{
			switch (mainLightShadowCascadesCount)
			{
			case 1:
				return new Vector3(1f, 0f, 0f);
			case 2:
				return new Vector3(urpAsset.cascade2Split, 1f, 0f);
			case 3:
				return urpAsset.cascade3Split;
			default:
				return urpAsset.cascade4Split;
			}
		}

		private static void InitializeMainLightShadowResolution(UniversalShadowData shadowData)
		{
			shadowData.mainLightShadowResolution = ShadowUtils.GetMaxTileResolutionInAtlas(shadowData.mainLightShadowmapWidth, shadowData.mainLightShadowmapHeight, shadowData.mainLightShadowCascadesCount);
			shadowData.mainLightRenderTargetWidth = shadowData.mainLightShadowmapWidth;
			shadowData.mainLightRenderTargetHeight = ((shadowData.mainLightShadowCascadesCount == 2) ? (shadowData.mainLightShadowmapHeight >> 1) : shadowData.mainLightShadowmapHeight);
		}

		private static UniversalPostProcessingData CreatePostProcessingData(ContextContainer frameData, UniversalRenderPipelineAsset settings)
		{
			UniversalPostProcessingData universalPostProcessingData = frameData.Create<UniversalPostProcessingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			universalPostProcessingData.isEnabled = universalCameraData.stackAnyPostProcessingEnabled;
			universalPostProcessingData.gradingMode = (settings.supportsHDR ? settings.colorGradingMode : ColorGradingMode.LowDynamicRange);
			if (universalCameraData.stackLastCameraOutputToHDR)
			{
				universalPostProcessingData.gradingMode = ColorGradingMode.HighDynamicRange;
			}
			universalPostProcessingData.lutSize = settings.colorGradingLutSize;
			universalPostProcessingData.useFastSRGBLinearConversion = settings.useFastSRGBLinearConversion;
			universalPostProcessingData.supportScreenSpaceLensFlare = settings.supportScreenSpaceLensFlare;
			universalPostProcessingData.supportDataDrivenLensFlare = settings.supportDataDrivenLensFlare;
			return universalPostProcessingData;
		}

		private static UniversalResourceData CreateUniversalResourceData(ContextContainer frameData)
		{
			return frameData.Create<UniversalResourceData>();
		}

		private static UniversalLightData CreateLightData(ContextContainer frameData, UniversalRenderPipelineAsset settings, NativeArray<VisibleLight> visibleLights, RenderingMode? renderingMode)
		{
			UniversalLightData result;
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.initializeLightData))
			{
				UniversalLightData universalLightData = frameData.Create<UniversalLightData>();
				universalLightData.visibleLights = visibleLights;
				universalLightData.mainLightIndex = UniversalRenderPipeline.GetMainLightIndex(settings, visibleLights);
				if (settings.additionalLightsRenderingMode != LightRenderingMode.Disabled)
				{
					universalLightData.additionalLightsCount = Math.Min((universalLightData.mainLightIndex != -1) ? (visibleLights.Length - 1) : visibleLights.Length, UniversalRenderPipeline.maxVisibleAdditionalLights);
					universalLightData.maxPerObjectAdditionalLightsCount = Math.Min(settings.maxAdditionalLightsCount, UniversalRenderPipeline.maxPerObjectLights);
				}
				else
				{
					universalLightData.additionalLightsCount = 0;
					universalLightData.maxPerObjectAdditionalLightsCount = 0;
				}
				if (settings.mainLightRenderingMode == LightRenderingMode.Disabled)
				{
					int brightestDirectionalLightIndex = UniversalRenderPipeline.GetBrightestDirectionalLightIndex(settings, visibleLights);
					if (brightestDirectionalLightIndex != -1)
					{
						universalLightData.additionalLightsCount--;
						universalLightData.mainLightIndex = brightestDirectionalLightIndex;
					}
				}
				universalLightData.supportsAdditionalLights = (settings.additionalLightsRenderingMode > LightRenderingMode.Disabled);
				universalLightData.shadeAdditionalLightsPerVertex = (settings.additionalLightsRenderingMode == LightRenderingMode.PerVertex);
				universalLightData.supportsMixedLighting = settings.supportsMixedLighting;
				universalLightData.reflectionProbeBoxProjection = settings.reflectionProbeBoxProjection;
				universalLightData.supportsLightLayers = (RenderingUtils.SupportsLightLayers(SystemInfo.graphicsDeviceType) && settings.useRenderingLayers);
				universalLightData.reflectionProbeBlending = settings.ShouldUseReflectionProbeBlending();
				universalLightData.reflectionProbeAtlas = (renderingMode != null && settings.ShouldUseReflectionProbeAtlasBlending(renderingMode.Value));
				result = universalLightData;
			}
			return result;
		}

		private static void ApplyTaaRenderingDebugOverrides(ref TemporalAA.Settings taaSettings)
		{
			switch (DebugDisplaySettings<UniversalRenderPipelineDebugDisplaySettings>.Instance.renderingSettings.taaDebugMode)
			{
			case DebugDisplaySettingsRendering.TaaDebugMode.ShowRawFrame:
				taaSettings.m_FrameInfluence = 1f;
				return;
			case DebugDisplaySettingsRendering.TaaDebugMode.ShowRawFrameNoJitter:
				taaSettings.m_FrameInfluence = 1f;
				taaSettings.jitterScale = 0f;
				return;
			case DebugDisplaySettingsRendering.TaaDebugMode.ShowClampedHistory:
				taaSettings.m_FrameInfluence = 0f;
				return;
			default:
				return;
			}
		}

		private static void UpdateTemporalAAData(UniversalCameraData cameraData, UniversalAdditionalCameraData additionalCameraData)
		{
			additionalCameraData.historyManager.RequestAccess<TaaHistory>();
			cameraData.taaHistory = additionalCameraData.historyManager.GetHistoryForWrite<TaaHistory>();
			if (cameraData.IsSTPEnabled())
			{
				additionalCameraData.historyManager.RequestAccess<StpHistory>();
				cameraData.stpHistory = additionalCameraData.historyManager.GetHistoryForWrite<StpHistory>();
			}
			ref TemporalAA.Settings taaSettings = ref additionalCameraData.taaSettings;
			cameraData.taaSettings = taaSettings;
			taaSettings.resetHistoryFrames -= ((taaSettings.resetHistoryFrames > 0) ? 1 : 0);
		}

		private static void UpdateTemporalAATargets(UniversalCameraData cameraData)
		{
			if (cameraData.IsTemporalAAEnabled())
			{
				bool flag = cameraData.xr.enabled && !cameraData.xr.singlePassEnabled;
				bool flag2;
				if (cameraData.IsSTPRequested())
				{
					cameraData.taaHistory.Reset();
					flag2 = cameraData.stpHistory.Update(cameraData);
				}
				else
				{
					flag2 = cameraData.taaHistory.Update(ref cameraData.cameraTargetDescriptor, flag);
				}
				if (flag2)
				{
					cameraData.taaSettings.resetHistoryFrames = cameraData.taaSettings.resetHistoryFrames + (flag ? 2 : 1);
					return;
				}
			}
			else
			{
				cameraData.taaHistory.Reset();
				if (cameraData.IsSTPRequested())
				{
					StpHistory stpHistory = cameraData.stpHistory;
					if (stpHistory == null)
					{
						return;
					}
					stpHistory.Reset();
				}
			}
		}

		private static void UpdateCameraStereoMatrices(Camera camera, XRPass xr)
		{
			if (xr.enabled)
			{
				if (xr.singlePassEnabled)
				{
					for (int i = 0; i < Mathf.Min(2, xr.viewCount); i++)
					{
						camera.SetStereoProjectionMatrix((Camera.StereoscopicEye)i, xr.GetProjMatrix(i));
						camera.SetStereoViewMatrix((Camera.StereoscopicEye)i, xr.GetViewMatrix(i));
					}
					return;
				}
				camera.SetStereoProjectionMatrix((Camera.StereoscopicEye)xr.multipassId, xr.GetProjMatrix(0));
				camera.SetStereoViewMatrix((Camera.StereoscopicEye)xr.multipassId, xr.GetViewMatrix(0));
			}
		}

		private static PerObjectData GetPerObjectLightFlags(UniversalLightData universalLightData, UniversalRenderPipelineAsset settings, RenderingMode? renderingMode)
		{
			PerObjectData result;
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.getPerObjectLightFlags))
			{
				bool flag = settings.ShouldUseReflectionProbeBlending();
				bool flag2 = false;
				if (renderingMode != null)
				{
					flag2 = (renderingMode.Value == RenderingMode.ForwardPlus);
				}
				PerObjectData perObjectData = PerObjectData.LightProbe | PerObjectData.Lightmaps | PerObjectData.OcclusionProbe | PerObjectData.ShadowMask;
				if (!flag2)
				{
					perObjectData |= (PerObjectData.ReflectionProbes | PerObjectData.LightData);
				}
				else if (!flag)
				{
					perObjectData |= PerObjectData.ReflectionProbes;
				}
				if (universalLightData.additionalLightsCount > 0 && !flag2 && !RenderingUtils.useStructuredBuffer)
				{
					perObjectData |= PerObjectData.LightIndices;
				}
				result = perObjectData;
			}
			return result;
		}

		private static int GetBrightestDirectionalLightIndex(UniversalRenderPipelineAsset settings, NativeArray<VisibleLight> visibleLights)
		{
			Light sun = RenderSettings.sun;
			int result = -1;
			float num = 0f;
			int length = visibleLights.Length;
			for (int i = 0; i < length; i++)
			{
				ref VisibleLight ptr = ref visibleLights.UnsafeElementAtMutable(i);
				Light light = ptr.light;
				if (light == null)
				{
					break;
				}
				if (ptr.lightType == LightType.Directional)
				{
					if (light == sun)
					{
						return i;
					}
					if (light.intensity > num)
					{
						num = light.intensity;
						result = i;
					}
				}
			}
			return result;
		}

		private static int GetMainLightIndex(UniversalRenderPipelineAsset settings, NativeArray<VisibleLight> visibleLights)
		{
			int result;
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.getMainLightIndex))
			{
				if (visibleLights.Length == 0 || settings.mainLightRenderingMode != LightRenderingMode.PerPixel)
				{
					result = -1;
				}
				else
				{
					result = UniversalRenderPipeline.GetBrightestDirectionalLightIndex(settings, visibleLights);
				}
			}
			return result;
		}

		private void SetupPerFrameShaderConstants()
		{
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.setupPerFrameShaderConstants))
			{
				Shader.SetGlobalColor(ShaderPropertyId.rendererColor, Color.white);
				Texture2D texture2D = null;
				switch (UniversalRenderPipeline.asset.lodCrossFadeDitheringType)
				{
				case LODCrossFadeDitheringType.BayerMatrix:
					texture2D = this.runtimeTextures.bayerMatrixTex;
					break;
				case LODCrossFadeDitheringType.BlueNoise:
					texture2D = this.runtimeTextures.blueNoise64LTex;
					break;
				case LODCrossFadeDitheringType.Stencil:
					texture2D = this.runtimeTextures.stencilDitherTex;
					break;
				default:
					Debug.LogWarning(string.Format("This Lod Cross Fade Dithering Type is not supported: {0}", UniversalRenderPipeline.asset.lodCrossFadeDitheringType));
					break;
				}
				if (texture2D != null)
				{
					Shader.SetGlobalFloat(ShaderPropertyId.ditheringTextureInvSize, 1f / (float)texture2D.width);
					Shader.SetGlobalTexture(ShaderPropertyId.ditheringTexture, texture2D);
				}
			}
		}

		private static void SetupPerCameraShaderConstants(CommandBuffer cmd)
		{
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.setupPerCameraShaderConstants))
			{
				SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
				Color c = CoreUtils.ConvertLinearToActiveColorSpace(new Color(ambientProbe[0, 0], ambientProbe[1, 0], ambientProbe[2, 0]) * RenderSettings.reflectionIntensity);
				cmd.SetGlobalVector(ShaderPropertyId.glossyEnvironmentColor, c);
				cmd.SetGlobalTexture(ShaderPropertyId.glossyEnvironmentCubeMap, ReflectionProbe.defaultTexture);
				cmd.SetGlobalVector(ShaderPropertyId.glossyEnvironmentCubeMapHDR, ReflectionProbe.defaultTextureHDRDecodeValues);
				cmd.SetGlobalVector(ShaderPropertyId.ambientSkyColor, CoreUtils.ConvertSRGBToActiveColorSpace(RenderSettings.ambientSkyColor));
				cmd.SetGlobalVector(ShaderPropertyId.ambientEquatorColor, CoreUtils.ConvertSRGBToActiveColorSpace(RenderSettings.ambientEquatorColor));
				cmd.SetGlobalVector(ShaderPropertyId.ambientGroundColor, CoreUtils.ConvertSRGBToActiveColorSpace(RenderSettings.ambientGroundColor));
				cmd.SetGlobalVector(ShaderPropertyId.subtractiveShadowColor, CoreUtils.ConvertSRGBToActiveColorSpace(RenderSettings.subtractiveShadowColor));
			}
		}

		private unsafe static void CheckAndApplyDebugSettings(ref RenderingData renderingData)
		{
			UniversalRenderPipelineDebugDisplaySettings instance = DebugDisplaySettings<UniversalRenderPipelineDebugDisplaySettings>.Instance;
			ref CameraData ptr = ref renderingData.cameraData;
			if (instance.AreAnySettingsActive && !ptr.isPreviewCamera)
			{
				DebugDisplaySettingsRendering renderingSettings = instance.renderingSettings;
				int msaaSamples = ptr.cameraTargetDescriptor.msaaSamples;
				if (!renderingSettings.enableMsaa)
				{
					msaaSamples = 1;
				}
				if (!renderingSettings.enableHDR)
				{
					*ptr.isHdrEnabled = false;
				}
				if (!instance.IsPostProcessingAllowed)
				{
					*ptr.postProcessEnabled = false;
				}
				*ptr.hdrColorBufferPrecision = (UniversalRenderPipeline.asset ? UniversalRenderPipeline.asset.hdrColorBufferPrecision : HDRColorBufferPrecision._32Bits);
				ptr.cameraTargetDescriptor.graphicsFormat = UniversalRenderPipeline.MakeRenderTextureGraphicsFormat(*ptr.isHdrEnabled, *ptr.hdrColorBufferPrecision, true);
				ptr.cameraTargetDescriptor.msaaSamples = msaaSamples;
			}
		}

		private static ImageUpscalingFilter ResolveUpscalingFilterSelection(Vector2 imageSize, float renderScale, UpscalingFilterSelection selection, bool enableRenderGraph)
		{
			ImageUpscalingFilter result = ImageUpscalingFilter.Linear;
			if ((selection == UpscalingFilterSelection.FSR && !FSRUtils.IsSupported()) || (selection == UpscalingFilterSelection.STP && (!STP.IsSupported() || !enableRenderGraph)))
			{
				selection = UpscalingFilterSelection.Auto;
			}
			switch (selection)
			{
			case UpscalingFilterSelection.Auto:
			{
				float num = 1f / renderScale;
				if (Mathf.Approximately(num - Mathf.Floor(num), 0f))
				{
					float num2 = imageSize.x / num;
					float num3 = imageSize.y / num;
					if (Mathf.Approximately(num2 - Mathf.Floor(num2), 0f) && Mathf.Approximately(num3 - Mathf.Floor(num3), 0f))
					{
						result = ImageUpscalingFilter.Point;
					}
				}
				break;
			}
			case UpscalingFilterSelection.Point:
				result = ImageUpscalingFilter.Point;
				break;
			case UpscalingFilterSelection.FSR:
				result = ImageUpscalingFilter.FSR;
				break;
			case UpscalingFilterSelection.STP:
				result = ImageUpscalingFilter.STP;
				break;
			}
			return result;
		}

		internal static bool HDROutputForMainDisplayIsActive()
		{
			bool flag = SystemInfo.hdrDisplaySupportFlags.HasFlag(HDRDisplaySupportFlags.Supported) && UniversalRenderPipeline.asset.supportsHDR;
			bool flag2 = HDROutputSettings.main.available && HDROutputSettings.main.active;
			return flag && flag2;
		}

		internal static bool HDROutputForAnyDisplayIsActive()
		{
			bool flag = UniversalRenderPipeline.HDROutputForMainDisplayIsActive();
			if (XRSystem.displayActive)
			{
				flag |= XRSystem.isHDRDisplayOutputActive;
			}
			return flag;
		}

		private void SetHDRState(List<Camera> cameras)
		{
			bool flag = HDROutputSettings.main.available && HDROutputSettings.main.active;
			if (SystemInfo.hdrDisplaySupportFlags.HasFlag(HDRDisplaySupportFlags.RuntimeSwitchable) && !UniversalRenderPipeline.asset.supportsHDR && flag)
			{
				HDROutputSettings.main.RequestHDRModeChange(false);
			}
			if (flag)
			{
				HDROutputSettings.main.automaticHDRTonemapping = false;
			}
		}

		internal static void GetHDROutputLuminanceParameters(HDROutputUtils.HDRDisplayInformation hdrDisplayInformation, ColorGamut hdrDisplayColorGamut, Tonemapping tonemapping, out Vector4 hdrOutputParameters)
		{
			float x = (float)hdrDisplayInformation.minToneMapLuminance;
			float y = (float)hdrDisplayInformation.maxToneMapLuminance;
			float num = hdrDisplayInformation.paperWhiteNits;
			if (!tonemapping.detectPaperWhite.value)
			{
				num = tonemapping.paperWhite.value;
			}
			if (!tonemapping.detectBrightnessLimits.value)
			{
				x = tonemapping.minNits.value;
				y = tonemapping.maxNits.value;
			}
			hdrOutputParameters = new Vector4(x, y, num, 1f / num);
		}

		internal static void GetHDROutputGradingParameters(Tonemapping tonemapping, out Vector4 hdrOutputParameters)
		{
			int num = 0;
			float y = 0f;
			TonemappingMode value = tonemapping.mode.value;
			if (value != TonemappingMode.Neutral)
			{
				if (value == TonemappingMode.ACES)
				{
					num = (int)tonemapping.acesPreset.value;
				}
			}
			else
			{
				num = (int)tonemapping.neutralHDRRangeReductionMode.value;
				y = tonemapping.hueShiftAmount.value;
			}
			hdrOutputParameters = new Vector4((float)num, y, 0f, 0f);
		}

		private static AdditionalLightsShadowAtlasLayout BuildAdditionalLightsShadowAtlasLayout(UniversalLightData lightData, UniversalShadowData shadowData, UniversalCameraData cameraData)
		{
			AdditionalLightsShadowAtlasLayout result;
			using (new ProfilingScope(UniversalRenderPipeline.Profiling.Pipeline.buildAdditionalLightsShadowAtlasLayout))
			{
				result = new AdditionalLightsShadowAtlasLayout(lightData, shadowData, cameraData);
			}
			return result;
		}

		private static void AdjustUIOverlayOwnership(int cameraCount)
		{
			if (XRSystem.displayActive || cameraCount == 0)
			{
				SupportedRenderingFeatures.active.rendersUIOverlay = false;
				return;
			}
			SupportedRenderingFeatures.active.rendersUIOverlay = true;
		}

		private static void SetupScreenMSAASamplesState(int cameraCount)
		{
			UniversalRenderPipeline.canOptimizeScreenMSAASamples = (cameraCount == 1);
			UniversalRenderPipeline.startFrameScreenMSAASamples = Screen.msaaSamples;
		}

		public static bool IsGameCamera(Camera camera)
		{
			if (camera == null)
			{
				throw new ArgumentNullException("camera");
			}
			return camera.cameraType == CameraType.Game || camera.cameraType == CameraType.VR;
		}

		public static UniversalRenderPipelineAsset asset
		{
			get
			{
				return GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			}
		}

		private void SortCameras(List<Camera> cameras)
		{
			if (cameras.Count > 1)
			{
				cameras.Sort(this.cameraComparison);
			}
		}

		private int GetLastBaseCameraIndex(List<Camera> cameras)
		{
			int result = 0;
			for (int i = 0; i < cameras.Count; i++)
			{
				UniversalAdditionalCameraData universalAdditionalCameraData;
				cameras[i].TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
				if (universalAdditionalCameraData != null && universalAdditionalCameraData.renderType == CameraRenderType.Base)
				{
					result = i;
				}
			}
			return result;
		}

		internal static GraphicsFormat MakeRenderTextureGraphicsFormat(bool isHdrEnabled, HDRColorBufferPrecision requestHDRColorBufferPrecision, bool needsAlpha)
		{
			if (!isHdrEnabled)
			{
				return SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
			}
			if (!needsAlpha && requestHDRColorBufferPrecision != HDRColorBufferPrecision._64Bits && SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, GraphicsFormatUsage.Blend))
			{
				return GraphicsFormat.B10G11R11_UFloatPack32;
			}
			if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormatUsage.Blend))
			{
				return GraphicsFormat.R16G16B16A16_SFloat;
			}
			return SystemInfo.GetGraphicsFormat(DefaultFormat.HDR);
		}

		internal static GraphicsFormat MakeUnormRenderTextureGraphicsFormat()
		{
			if (SystemInfo.IsFormatSupported(GraphicsFormat.A2B10G10R10_UNormPack32, GraphicsFormatUsage.Blend))
			{
				return GraphicsFormat.A2B10G10R10_UNormPack32;
			}
			return GraphicsFormat.R8G8B8A8_UNorm;
		}

		internal static RenderTextureDescriptor CreateRenderTextureDescriptor(Camera camera, UniversalCameraData cameraData, bool isHdrEnabled, HDRColorBufferPrecision requestHDRColorBufferPrecision, int msaaSamples, bool needsAlpha, bool requiresOpaqueTexture)
		{
			RenderTextureDescriptor descriptor;
			if (camera.targetTexture == null)
			{
				descriptor = new RenderTextureDescriptor(cameraData.scaledWidth, cameraData.scaledHeight);
				descriptor.graphicsFormat = UniversalRenderPipeline.MakeRenderTextureGraphicsFormat(isHdrEnabled, requestHDRColorBufferPrecision, needsAlpha);
				descriptor.depthBufferBits = (int)CoreUtils.GetDefaultDepthBufferBits();
				descriptor.depthStencilFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil);
				descriptor.msaaSamples = msaaSamples;
				descriptor.sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear);
			}
			else
			{
				descriptor = camera.targetTexture.descriptor;
				descriptor.msaaSamples = msaaSamples;
				descriptor.width = cameraData.scaledWidth;
				descriptor.height = cameraData.scaledHeight;
				if (camera.cameraType == CameraType.SceneView && !isHdrEnabled)
				{
					descriptor.graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
				}
			}
			descriptor.enableRandomWrite = false;
			descriptor.bindMS = false;
			descriptor.useDynamicScale = camera.allowDynamicResolution;
			descriptor.msaaSamples = SystemInfo.GetRenderTextureSupportedMSAASampleCount(descriptor);
			if (!SystemInfo.supportsStoreAndResolveAction)
			{
				descriptor.msaaSamples = 1;
			}
			return descriptor;
		}

		public static void GetLightAttenuationAndSpotDirection(LightType lightType, float lightRange, Matrix4x4 lightLocalToWorldMatrix, float spotAngle, float? innerSpotAngle, out Vector4 lightAttenuation, out Vector4 lightSpotDir)
		{
			lightAttenuation = UniversalRenderPipeline.k_DefaultLightAttenuation;
			lightSpotDir = UniversalRenderPipeline.k_DefaultLightSpotDirection;
			if (lightType != LightType.Directional)
			{
				UniversalRenderPipeline.GetPunctualLightDistanceAttenuation(lightRange, ref lightAttenuation);
				if (lightType == LightType.Spot)
				{
					UniversalRenderPipeline.GetSpotDirection(ref lightLocalToWorldMatrix, out lightSpotDir);
					UniversalRenderPipeline.GetSpotAngleAttenuation(spotAngle, innerSpotAngle, ref lightAttenuation);
				}
			}
		}

		internal static void GetPunctualLightDistanceAttenuation(float lightRange, ref Vector4 lightAttenuation)
		{
			float num = lightRange * lightRange;
			float num2 = 0.64000005f * num - num;
			float y = -num / num2;
			float x = 1f / Mathf.Max(0.0001f, num);
			lightAttenuation.x = x;
			lightAttenuation.y = y;
		}

		internal static void GetSpotAngleAttenuation(float spotAngle, float? innerSpotAngle, ref Vector4 lightAttenuation)
		{
			float num = Mathf.Cos(0.017453292f * spotAngle * 0.5f);
			float num2;
			if (innerSpotAngle != null)
			{
				num2 = Mathf.Cos(innerSpotAngle.Value * 0.017453292f * 0.5f);
			}
			else
			{
				num2 = Mathf.Cos(2f * Mathf.Atan(Mathf.Tan(spotAngle * 0.5f * 0.017453292f) * 46f / 64f) * 0.5f);
			}
			float num3 = Mathf.Max(0.001f, num2 - num);
			float num4 = 1f / num3;
			float w = -num * num4;
			lightAttenuation.z = num4;
			lightAttenuation.w = w;
		}

		internal static void GetSpotDirection(ref Matrix4x4 lightLocalToWorldMatrix, out Vector4 lightSpotDir)
		{
			Vector4 column = lightLocalToWorldMatrix.GetColumn(2);
			lightSpotDir = new Vector4(-column.x, -column.y, -column.z, 0f);
		}

		public static void InitializeLightConstants_Common(NativeArray<VisibleLight> lights, int lightIndex, out Vector4 lightPos, out Vector4 lightColor, out Vector4 lightAttenuation, out Vector4 lightSpotDir, out Vector4 lightOcclusionProbeChannel)
		{
			lightPos = UniversalRenderPipeline.k_DefaultLightPosition;
			lightColor = UniversalRenderPipeline.k_DefaultLightColor;
			lightOcclusionProbeChannel = UniversalRenderPipeline.k_DefaultLightsProbeChannel;
			lightAttenuation = UniversalRenderPipeline.k_DefaultLightAttenuation;
			lightSpotDir = UniversalRenderPipeline.k_DefaultLightSpotDirection;
			if (lightIndex < 0)
			{
				return;
			}
			ref VisibleLight ptr = ref lights.UnsafeElementAtMutable(lightIndex);
			Light light = ptr.light;
			Matrix4x4 localToWorldMatrix = ptr.localToWorldMatrix;
			LightType lightType = ptr.lightType;
			if (lightType == LightType.Directional)
			{
				Vector4 vector = -localToWorldMatrix.GetColumn(2);
				lightPos = new Vector4(vector.x, vector.y, vector.z, 0f);
			}
			else
			{
				Vector4 column = localToWorldMatrix.GetColumn(3);
				lightPos = new Vector4(column.x, column.y, column.z, 1f);
				UniversalRenderPipeline.GetPunctualLightDistanceAttenuation(ptr.range, ref lightAttenuation);
				if (lightType == LightType.Spot)
				{
					UniversalRenderPipeline.GetSpotAngleAttenuation(ptr.spotAngle, (light != null) ? new float?(light.innerSpotAngle) : null, ref lightAttenuation);
					UniversalRenderPipeline.GetSpotDirection(ref localToWorldMatrix, out lightSpotDir);
				}
			}
			lightColor = ptr.finalColor;
			if (light != null && light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed && 0 <= light.bakingOutput.occlusionMaskChannel && light.bakingOutput.occlusionMaskChannel < 4)
			{
				lightOcclusionProbeChannel[light.bakingOutput.occlusionMaskChannel] = 1f;
			}
		}

		private static void RecordAndExecuteRenderGraph(RenderGraph renderGraph, ScriptableRenderContext context, ScriptableRenderer renderer, CommandBuffer cmd, Camera camera, string cameraName)
		{
			RenderGraphParameters renderGraphParameters = new RenderGraphParameters
			{
				executionName = cameraName,
				commandBuffer = cmd,
				scriptableRenderContext = context,
				currentFrameIndex = Time.frameCount
			};
			try
			{
				renderGraph.BeginRecording(renderGraphParameters);
				renderer.RecordRenderGraph(renderGraph, context);
			}
			catch (Exception e)
			{
				if (renderGraph.ResetGraphAndLogException(e))
				{
					throw;
				}
				return;
			}
			renderGraph.EndRecordingAndExecute();
		}

		public const string k_ShaderTagName = "UniversalPipeline";

		internal const int k_DefaultRenderingLayerMask = 1;

		private readonly DebugDisplaySettingsUI m_DebugDisplaySettingsUI = new DebugDisplaySettingsUI();

		private UniversalRenderPipelineGlobalSettings m_GlobalSettings;

		internal static bool stackedOverlayCamerasRequireDepthForPostProcessing = false;

		internal static RenderGraph s_RenderGraph;

		internal static RTHandleResourcePool s_RTHandlePool;

		internal static bool useRenderGraph;

		internal bool apvIsEnabled;

		private readonly UniversalRenderPipelineAsset pipelineAsset;

		internal bool enableHDROnce = true;

		private static Vector4 k_DefaultLightPosition = new Vector4(0f, 0f, 1f, 0f);

		private static Vector4 k_DefaultLightColor = Color.black;

		private static Vector4 k_DefaultLightAttenuation = new Vector4(0f, 1f, 0f, 1f);

		private static Vector4 k_DefaultLightSpotDirection = new Vector4(0f, 0f, 1f, 0f);

		private static Vector4 k_DefaultLightsProbeChannel = new Vector4(0f, 0f, 0f, 0f);

		private static List<Vector4> m_ShadowBiasData = new List<Vector4>();

		private static List<int> m_ShadowResolutionData = new List<int>();

		private Comparison<Camera> cameraComparison = (Camera camera1, Camera camera2) => (int)camera1.depth - (int)camera2.depth;

		private static Lightmapping.RequestLightsDelegate lightsDelegate = delegate(Light[] requests, NativeArray<LightDataGI> lightsOutput)
		{
			LightDataGI value = default(LightDataGI);
			if (!SupportedRenderingFeatures.active.enlighten || (SupportedRenderingFeatures.active.lightmapBakeTypes | LightmapBakeType.Realtime) == (LightmapBakeType)0)
			{
				for (int i = 0; i < requests.Length; i++)
				{
					Light light = requests[i];
					value.InitNoBake(light.GetInstanceID());
					lightsOutput[i] = value;
				}
				return;
			}
			for (int j = 0; j < requests.Length; j++)
			{
				Light light2 = requests[j];
				switch (light2.type)
				{
				case LightType.Spot:
				{
					SpotLight spotLight = default(SpotLight);
					LightmapperUtils.Extract(light2, ref spotLight);
					spotLight.innerConeAngle = light2.innerSpotAngle * 0.017453292f;
					spotLight.angularFalloff = AngularFalloffType.AnalyticAndInnerAngle;
					value.Init(ref spotLight);
					break;
				}
				case LightType.Directional:
				{
					DirectionalLight directionalLight = default(DirectionalLight);
					LightmapperUtils.Extract(light2, ref directionalLight);
					value.Init(ref directionalLight);
					break;
				}
				case LightType.Point:
				{
					PointLight pointLight = default(PointLight);
					LightmapperUtils.Extract(light2, ref pointLight);
					value.Init(ref pointLight);
					break;
				}
				case LightType.Area:
					value.InitNoBake(light2.GetInstanceID());
					break;
				case LightType.Disc:
					value.InitNoBake(light2.GetInstanceID());
					break;
				default:
					value.InitNoBake(light2.GetInstanceID());
					break;
				}
				value.falloff = FalloffType.InverseSquared;
				lightsOutput[j] = value;
			}
		};

		internal static class CameraMetadataCache
		{
			public static UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry GetCached(Camera camera)
			{
				int hashCode = camera.GetHashCode();
				UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry cameraMetadataCacheEntry;
				if (!UniversalRenderPipeline.CameraMetadataCache.s_MetadataCache.TryGetValue(hashCode, out cameraMetadataCacheEntry))
				{
					string name = camera.name;
					cameraMetadataCacheEntry = new UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry
					{
						name = name,
						sampler = new ProfilingSampler("UniversalRenderPipeline.RenderSingleCameraInternal: " + name)
					};
					UniversalRenderPipeline.CameraMetadataCache.s_MetadataCache.Add(hashCode, cameraMetadataCacheEntry);
				}
				return cameraMetadataCacheEntry;
			}

			private static Dictionary<int, UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry> s_MetadataCache = new Dictionary<int, UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry>();

			private static readonly UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry k_NoAllocEntry = new UniversalRenderPipeline.CameraMetadataCache.CameraMetadataCacheEntry
			{
				name = "Unknown",
				sampler = new ProfilingSampler("Unknown")
			};

			public class CameraMetadataCacheEntry
			{
				public string name;

				public ProfilingSampler sampler;
			}
		}

		internal static class Profiling
		{
			public static class Pipeline
			{
				private const string k_Name = "UniversalRenderPipeline";

				public static readonly ProfilingSampler initializeCameraData = new ProfilingSampler("UniversalRenderPipeline.CreateCameraData");

				public static readonly ProfilingSampler initializeStackedCameraData = new ProfilingSampler("UniversalRenderPipeline.InitializeStackedCameraData");

				public static readonly ProfilingSampler initializeAdditionalCameraData = new ProfilingSampler("UniversalRenderPipeline.InitializeAdditionalCameraData");

				public static readonly ProfilingSampler initializeRenderingData = new ProfilingSampler("UniversalRenderPipeline.CreateRenderingData");

				public static readonly ProfilingSampler initializeShadowData = new ProfilingSampler("UniversalRenderPipeline.CreateShadowData");

				public static readonly ProfilingSampler initializeLightData = new ProfilingSampler("UniversalRenderPipeline.CreateLightData");

				public static readonly ProfilingSampler buildAdditionalLightsShadowAtlasLayout = new ProfilingSampler("UniversalRenderPipeline.BuildAdditionalLightsShadowAtlasLayout");

				public static readonly ProfilingSampler getPerObjectLightFlags = new ProfilingSampler("UniversalRenderPipeline.GetPerObjectLightFlags");

				public static readonly ProfilingSampler getMainLightIndex = new ProfilingSampler("UniversalRenderPipeline.GetMainLightIndex");

				public static readonly ProfilingSampler setupPerFrameShaderConstants = new ProfilingSampler("UniversalRenderPipeline.SetupPerFrameShaderConstants");

				public static readonly ProfilingSampler setupPerCameraShaderConstants = new ProfilingSampler("UniversalRenderPipeline.SetupPerCameraShaderConstants");

				public static class Renderer
				{
					private const string k_Name = "ScriptableRenderer";

					public static readonly ProfilingSampler setupCullingParameters = new ProfilingSampler("ScriptableRenderer.SetupCullingParameters");

					public static readonly ProfilingSampler setup = new ProfilingSampler("ScriptableRenderer.Setup");
				}

				public static class Context
				{
					private const string k_Name = "ScriptableRenderContext";

					public static readonly ProfilingSampler submit = new ProfilingSampler("ScriptableRenderContext.Submit");
				}
			}
		}

		private readonly struct CameraRenderingScope : IDisposable
		{
			public CameraRenderingScope(ScriptableRenderContext context, Camera camera)
			{
				using (new ProfilingScope(UniversalRenderPipeline.CameraRenderingScope.beginCameraRenderingSampler))
				{
					this.m_Context = context;
					this.m_Camera = camera;
					RenderPipeline.BeginCameraRendering(context, camera);
				}
			}

			public void Dispose()
			{
				using (new ProfilingScope(UniversalRenderPipeline.CameraRenderingScope.endCameraRenderingSampler))
				{
					RenderPipeline.EndCameraRendering(this.m_Context, this.m_Camera);
				}
			}

			private static readonly ProfilingSampler beginCameraRenderingSampler = new ProfilingSampler("RenderPipeline.BeginCameraRendering");

			private static readonly ProfilingSampler endCameraRenderingSampler = new ProfilingSampler("RenderPipeline.EndCameraRendering");

			private readonly ScriptableRenderContext m_Context;

			private readonly Camera m_Camera;
		}

		private readonly struct ContextRenderingScope : IDisposable
		{
			public ContextRenderingScope(ScriptableRenderContext context, List<Camera> cameras)
			{
				this.m_Context = context;
				this.m_Cameras = cameras;
				using (new ProfilingScope(UniversalRenderPipeline.ContextRenderingScope.beginContextRenderingSampler))
				{
					RenderPipeline.BeginContextRendering(this.m_Context, this.m_Cameras);
				}
			}

			public void Dispose()
			{
				using (new ProfilingScope(UniversalRenderPipeline.ContextRenderingScope.endContextRenderingSampler))
				{
					RenderPipeline.EndContextRendering(this.m_Context, this.m_Cameras);
				}
			}

			private static readonly ProfilingSampler beginContextRenderingSampler = new ProfilingSampler("RenderPipeline.BeginContextRendering");

			private static readonly ProfilingSampler endContextRenderingSampler = new ProfilingSampler("RenderPipeline.EndContextRendering");

			private readonly ScriptableRenderContext m_Context;

			private readonly List<Camera> m_Cameras;
		}

		public class SingleCameraRequest
		{
			public RenderTexture destination;

			public int mipLevel;

			public CubemapFace face = CubemapFace.Unknown;

			public int slice;
		}
	}
}
