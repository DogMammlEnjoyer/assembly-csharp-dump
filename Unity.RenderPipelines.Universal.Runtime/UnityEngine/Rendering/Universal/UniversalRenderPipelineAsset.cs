using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.Universal
{
	[ExcludeFromPreset]
	public class UniversalRenderPipelineAsset : RenderPipelineAsset<UniversalRenderPipeline>, ISerializationCallbackReceiver, IProbeVolumeEnabledRenderPipeline, IGPUResidentRenderPipeline, IRenderGraphEnabledRenderPipeline, ISTPEnabledRenderPipeline
	{
		internal bool IsAtLastVersion()
		{
			return 12 == this.k_AssetVersion;
		}

		GPUResidentDrawerSettings IGPUResidentRenderPipeline.gpuResidentDrawerSettings
		{
			get
			{
				return new GPUResidentDrawerSettings
				{
					mode = this.m_GPUResidentDrawerMode,
					enableOcclusionCulling = this.m_GPUResidentDrawerEnableOcclusionCullingInCameras,
					supportDitheringCrossFade = this.m_EnableLODCrossFade,
					allowInEditMode = true,
					smallMeshScreenPercentage = this.m_SmallMeshScreenPercentage,
					errorShader = Shader.Find("Hidden/Universal Render Pipeline/FallbackError"),
					loadingShader = Shader.Find("Hidden/Universal Render Pipeline/FallbackLoading")
				};
			}
		}

		public ReadOnlySpan<ScriptableRendererData> rendererDataList
		{
			get
			{
				return this.m_RendererDataList;
			}
		}

		public ReadOnlySpan<ScriptableRenderer> renderers
		{
			get
			{
				return this.m_Renderers;
			}
		}

		public bool isImmediateModeSupported
		{
			get
			{
				return false;
			}
		}

		public ScriptableRendererData LoadBuiltinRendererData(RendererType type = RendererType.UniversalRenderer)
		{
			this.m_RendererDataList[0] = null;
			return this.m_RendererDataList[0];
		}

		protected override void EnsureGlobalSettings()
		{
			base.EnsureGlobalSettings();
		}

		protected override RenderPipeline CreatePipeline()
		{
			if (this.m_RendererDataList == null)
			{
				this.m_RendererDataList = new ScriptableRendererData[1];
			}
			if (this.m_DefaultRendererIndex < this.m_RendererDataList.Length && !(this.m_RendererDataList[this.m_DefaultRendererIndex] == null))
			{
				this.DestroyRenderers();
				RenderPipeline result = new UniversalRenderPipeline(this);
				this.CreateRenderers();
				IGPUResidentRenderPipeline.ReinitializeGPUResidentDrawer();
				return result;
			}
			if (this.k_AssetPreviousVersion != this.k_AssetVersion)
			{
				return null;
			}
			Debug.LogError("Default Renderer is missing, make sure there is a Renderer assigned as the default on the current Universal RP asset:" + UniversalRenderPipeline.asset.name, this);
			return null;
		}

		internal void DestroyRenderers()
		{
			if (this.m_Renderers == null)
			{
				return;
			}
			for (int i = 0; i < this.m_Renderers.Length; i++)
			{
				this.DestroyRenderer(ref this.m_Renderers[i]);
			}
		}

		private void DestroyRenderer(ref ScriptableRenderer renderer)
		{
			if (renderer != null)
			{
				renderer.Dispose();
				renderer = null;
			}
		}

		protected override void OnDisable()
		{
			this.DestroyRenderers();
			base.OnDisable();
		}

		private void CreateRenderers()
		{
			if (this.m_Renderers != null)
			{
				for (int i = 0; i < this.m_Renderers.Length; i++)
				{
					if (this.m_Renderers[i] != null)
					{
						Debug.LogError(string.Format("Creating renderers but previous instance wasn't properly destroyed: m_Renderers[{0}]", i));
					}
				}
			}
			if (this.m_Renderers == null || this.m_Renderers.Length != this.m_RendererDataList.Length)
			{
				this.m_Renderers = new ScriptableRenderer[this.m_RendererDataList.Length];
			}
			for (int j = 0; j < this.m_RendererDataList.Length; j++)
			{
				if (this.m_RendererDataList[j] != null)
				{
					this.m_Renderers[j] = this.m_RendererDataList[j].InternalCreateRenderer();
				}
			}
		}

		public ScriptableRenderer scriptableRenderer
		{
			get
			{
				ScriptableRendererData[] rendererDataList = this.m_RendererDataList;
				int? num = (rendererDataList != null) ? new int?(rendererDataList.Length) : null;
				int defaultRendererIndex = this.m_DefaultRendererIndex;
				if ((num.GetValueOrDefault() > defaultRendererIndex & num != null) && this.m_RendererDataList[this.m_DefaultRendererIndex] == null)
				{
					Debug.LogError("Default renderer is missing from the current Pipeline Asset.", this);
					return null;
				}
				if (this.scriptableRendererData.isInvalidated || this.m_Renderers[this.m_DefaultRendererIndex] == null)
				{
					this.DestroyRenderer(ref this.m_Renderers[this.m_DefaultRendererIndex]);
					this.m_Renderers[this.m_DefaultRendererIndex] = this.scriptableRendererData.InternalCreateRenderer();
					if (this.gpuResidentDrawerMode != GPUResidentDrawerMode.Disabled)
					{
						IGPUResidentRenderPipeline.ReinitializeGPUResidentDrawer();
					}
				}
				return this.m_Renderers[this.m_DefaultRendererIndex];
			}
		}

		public ScriptableRenderer GetRenderer(int index)
		{
			if (index == -1)
			{
				index = this.m_DefaultRendererIndex;
			}
			if (index >= this.m_RendererDataList.Length || index < 0 || this.m_RendererDataList[index] == null)
			{
				Debug.LogWarning("Renderer at index " + index.ToString() + " is missing, falling back to Default Renderer " + this.m_RendererDataList[this.m_DefaultRendererIndex].name, this);
				index = this.m_DefaultRendererIndex;
			}
			if (this.m_Renderers == null || this.m_Renderers.Length < this.m_RendererDataList.Length)
			{
				this.DestroyRenderers();
				this.CreateRenderers();
			}
			if (this.m_RendererDataList[index].isInvalidated || this.m_Renderers[index] == null)
			{
				this.DestroyRenderer(ref this.m_Renderers[index]);
				this.m_Renderers[index] = this.m_RendererDataList[index].InternalCreateRenderer();
				if (this.gpuResidentDrawerMode != GPUResidentDrawerMode.Disabled)
				{
					IGPUResidentRenderPipeline.ReinitializeGPUResidentDrawer();
				}
			}
			return this.m_Renderers[index];
		}

		internal ScriptableRendererData scriptableRendererData
		{
			get
			{
				if (this.m_RendererDataList[this.m_DefaultRendererIndex] == null)
				{
					this.CreatePipeline();
				}
				return this.m_RendererDataList[this.m_DefaultRendererIndex];
			}
		}

		internal GraphicsFormat additionalLightsCookieFormat
		{
			get
			{
				GraphicsFormat graphicsFormat = GraphicsFormat.None;
				foreach (GraphicsFormat graphicsFormat2 in UniversalRenderPipelineAsset.s_LightCookieFormatList[(int)this.m_AdditionalLightsCookieFormat])
				{
					if (SystemInfo.IsFormatSupported(graphicsFormat2, GraphicsFormatUsage.Render))
					{
						graphicsFormat = graphicsFormat2;
						break;
					}
				}
				if (QualitySettings.activeColorSpace == ColorSpace.Gamma)
				{
					graphicsFormat = GraphicsFormatUtility.GetLinearFormat(graphicsFormat);
				}
				if (graphicsFormat == GraphicsFormat.None)
				{
					graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
					Debug.LogWarning(string.Format("Additional Lights Cookie Format ({0}) is not supported by the platform. Falling back to {1}-bit format ({2})", this.m_AdditionalLightsCookieFormat.ToString(), GraphicsFormatUtility.GetBlockSize(graphicsFormat) * 8U, GraphicsFormatUtility.GetFormatString(graphicsFormat)));
				}
				return graphicsFormat;
			}
		}

		internal Vector2Int additionalLightsCookieResolution
		{
			get
			{
				return new Vector2Int((int)this.m_AdditionalLightsCookieResolution, (int)this.m_AdditionalLightsCookieResolution);
			}
		}

		internal int[] rendererIndexList
		{
			get
			{
				int[] array = new int[this.m_RendererDataList.Length + 1];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = i - 1;
				}
				return array;
			}
		}

		public bool supportsCameraDepthTexture
		{
			get
			{
				return this.m_RequireDepthTexture;
			}
			set
			{
				this.m_RequireDepthTexture = value;
			}
		}

		public bool supportsCameraOpaqueTexture
		{
			get
			{
				return this.m_RequireOpaqueTexture;
			}
			set
			{
				this.m_RequireOpaqueTexture = value;
			}
		}

		public Downsampling opaqueDownsampling
		{
			get
			{
				return this.m_OpaqueDownsampling;
			}
		}

		public bool supportsTerrainHoles
		{
			get
			{
				return this.m_SupportsTerrainHoles;
			}
		}

		public StoreActionsOptimization storeActionsOptimization
		{
			get
			{
				return this.m_StoreActionsOptimization;
			}
			set
			{
				this.m_StoreActionsOptimization = value;
			}
		}

		public bool supportsHDR
		{
			get
			{
				return this.m_SupportsHDR;
			}
			set
			{
				this.m_SupportsHDR = value;
			}
		}

		public HDRColorBufferPrecision hdrColorBufferPrecision
		{
			get
			{
				return this.m_HDRColorBufferPrecision;
			}
			set
			{
				this.m_HDRColorBufferPrecision = value;
			}
		}

		public int msaaSampleCount
		{
			get
			{
				return (int)this.m_MSAA;
			}
			set
			{
				this.m_MSAA = (MsaaQuality)value;
			}
		}

		public float renderScale
		{
			get
			{
				return this.m_RenderScale;
			}
			set
			{
				this.m_RenderScale = this.ValidateRenderScale(value);
			}
		}

		public bool enableLODCrossFade
		{
			get
			{
				return this.m_EnableLODCrossFade;
			}
		}

		public LODCrossFadeDitheringType lodCrossFadeDitheringType
		{
			get
			{
				return this.m_LODCrossFadeDitheringType;
			}
		}

		public UpscalingFilterSelection upscalingFilter
		{
			get
			{
				return this.m_UpscalingFilter;
			}
			set
			{
				this.m_UpscalingFilter = value;
			}
		}

		public bool fsrOverrideSharpness
		{
			get
			{
				return this.m_FsrOverrideSharpness;
			}
			set
			{
				this.m_FsrOverrideSharpness = value;
			}
		}

		public float fsrSharpness
		{
			get
			{
				return this.m_FsrSharpness;
			}
			set
			{
				this.m_FsrSharpness = value;
			}
		}

		public ShEvalMode shEvalMode
		{
			get
			{
				return this.m_ShEvalMode;
			}
			internal set
			{
				this.m_ShEvalMode = value;
			}
		}

		public LightProbeSystem lightProbeSystem
		{
			get
			{
				return this.m_LightProbeSystem;
			}
			internal set
			{
				this.m_LightProbeSystem = value;
			}
		}

		public ProbeVolumeTextureMemoryBudget probeVolumeMemoryBudget
		{
			get
			{
				return this.m_ProbeVolumeMemoryBudget;
			}
			internal set
			{
				this.m_ProbeVolumeMemoryBudget = value;
			}
		}

		public ProbeVolumeBlendingTextureMemoryBudget probeVolumeBlendingMemoryBudget
		{
			get
			{
				return this.m_ProbeVolumeBlendingMemoryBudget;
			}
			internal set
			{
				this.m_ProbeVolumeBlendingMemoryBudget = value;
			}
		}

		[Obsolete("This is obsolete, use supportProbeVolumeGPUStreaming instead.")]
		public bool supportProbeVolumeStreaming
		{
			get
			{
				return this.m_SupportProbeVolumeGPUStreaming;
			}
			internal set
			{
				this.m_SupportProbeVolumeGPUStreaming = value;
			}
		}

		public bool supportProbeVolumeGPUStreaming
		{
			get
			{
				return this.m_SupportProbeVolumeGPUStreaming;
			}
			internal set
			{
				this.m_SupportProbeVolumeGPUStreaming = value;
			}
		}

		public bool supportProbeVolumeDiskStreaming
		{
			get
			{
				return this.m_SupportProbeVolumeDiskStreaming;
			}
			internal set
			{
				this.m_SupportProbeVolumeDiskStreaming = value;
			}
		}

		public bool supportProbeVolumeScenarios
		{
			get
			{
				return this.m_SupportProbeVolumeScenarios;
			}
			internal set
			{
				this.m_SupportProbeVolumeScenarios = value;
			}
		}

		public bool supportProbeVolumeScenarioBlending
		{
			get
			{
				return this.m_SupportProbeVolumeScenarioBlending;
			}
			internal set
			{
				this.m_SupportProbeVolumeScenarioBlending = value;
			}
		}

		public ProbeVolumeSHBands probeVolumeSHBands
		{
			get
			{
				return this.m_ProbeVolumeSHBands;
			}
			internal set
			{
				this.m_ProbeVolumeSHBands = value;
			}
		}

		public LightRenderingMode mainLightRenderingMode
		{
			get
			{
				return this.m_MainLightRenderingMode;
			}
			internal set
			{
				this.m_MainLightRenderingMode = value;
			}
		}

		public bool supportsMainLightShadows
		{
			get
			{
				return this.m_MainLightShadowsSupported;
			}
			internal set
			{
				this.m_MainLightShadowsSupported = value;
			}
		}

		public int mainLightShadowmapResolution
		{
			get
			{
				return (int)this.m_MainLightShadowmapResolution;
			}
			set
			{
				this.m_MainLightShadowmapResolution = (ShadowResolution)value;
			}
		}

		public LightRenderingMode additionalLightsRenderingMode
		{
			get
			{
				return this.m_AdditionalLightsRenderingMode;
			}
			internal set
			{
				this.m_AdditionalLightsRenderingMode = value;
			}
		}

		public int maxAdditionalLightsCount
		{
			get
			{
				return this.m_AdditionalLightsPerObjectLimit;
			}
			set
			{
				this.m_AdditionalLightsPerObjectLimit = this.ValidatePerObjectLights(value);
			}
		}

		public bool supportsAdditionalLightShadows
		{
			get
			{
				return this.m_AdditionalLightShadowsSupported;
			}
			internal set
			{
				this.m_AdditionalLightShadowsSupported = value;
			}
		}

		public int additionalLightsShadowmapResolution
		{
			get
			{
				return (int)this.m_AdditionalLightsShadowmapResolution;
			}
			set
			{
				this.m_AdditionalLightsShadowmapResolution = (ShadowResolution)value;
			}
		}

		public int additionalLightsShadowResolutionTierLow
		{
			get
			{
				return this.m_AdditionalLightsShadowResolutionTierLow;
			}
			internal set
			{
				this.m_AdditionalLightsShadowResolutionTierLow = value;
			}
		}

		public int additionalLightsShadowResolutionTierMedium
		{
			get
			{
				return this.m_AdditionalLightsShadowResolutionTierMedium;
			}
			internal set
			{
				this.m_AdditionalLightsShadowResolutionTierMedium = value;
			}
		}

		public int additionalLightsShadowResolutionTierHigh
		{
			get
			{
				return this.m_AdditionalLightsShadowResolutionTierHigh;
			}
			internal set
			{
				this.m_AdditionalLightsShadowResolutionTierHigh = value;
			}
		}

		internal int GetAdditionalLightsShadowResolution(int additionalLightsShadowResolutionTier)
		{
			if (additionalLightsShadowResolutionTier <= UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow)
			{
				return this.additionalLightsShadowResolutionTierLow;
			}
			if (additionalLightsShadowResolutionTier == UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierMedium)
			{
				return this.additionalLightsShadowResolutionTierMedium;
			}
			if (additionalLightsShadowResolutionTier >= UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierHigh)
			{
				return this.additionalLightsShadowResolutionTierHigh;
			}
			return this.additionalLightsShadowResolutionTierMedium;
		}

		public bool reflectionProbeBlending
		{
			get
			{
				return this.m_ReflectionProbeBlending;
			}
			internal set
			{
				this.m_ReflectionProbeBlending = value;
			}
		}

		internal bool ShouldUseReflectionProbeBlending()
		{
			return this.gpuResidentDrawerMode != GPUResidentDrawerMode.Disabled || this.reflectionProbeBlending;
		}

		public bool reflectionProbeBoxProjection
		{
			get
			{
				return this.m_ReflectionProbeBoxProjection;
			}
			internal set
			{
				this.m_ReflectionProbeBoxProjection = value;
			}
		}

		public bool reflectionProbeAtlas
		{
			get
			{
				return this.m_ReflectionProbeAtlas;
			}
			internal set
			{
				this.m_ReflectionProbeAtlas = value;
			}
		}

		internal bool ShouldUseReflectionProbeAtlasBlending(RenderingMode renderingMode)
		{
			bool flag = this.ShouldUseReflectionProbeBlending();
			return this.gpuResidentDrawerMode != GPUResidentDrawerMode.Disabled || (flag && (this.reflectionProbeAtlas || renderingMode == RenderingMode.DeferredPlus));
		}

		public float shadowDistance
		{
			get
			{
				return this.m_ShadowDistance;
			}
			set
			{
				this.m_ShadowDistance = Mathf.Max(0f, value);
			}
		}

		public int shadowCascadeCount
		{
			get
			{
				return this.m_ShadowCascadeCount;
			}
			set
			{
				if (value < 1 || value > 4)
				{
					throw new ArgumentException(string.Format("Value ({0}) needs to be between {1} and {2}.", value, 1, 4));
				}
				this.m_ShadowCascadeCount = value;
			}
		}

		public float cascade2Split
		{
			get
			{
				return this.m_Cascade2Split;
			}
			set
			{
				this.m_Cascade2Split = value;
			}
		}

		public Vector2 cascade3Split
		{
			get
			{
				return this.m_Cascade3Split;
			}
			set
			{
				this.m_Cascade3Split = value;
			}
		}

		public Vector3 cascade4Split
		{
			get
			{
				return this.m_Cascade4Split;
			}
			set
			{
				this.m_Cascade4Split = value;
			}
		}

		public float cascadeBorder
		{
			get
			{
				return this.m_CascadeBorder;
			}
			set
			{
				this.m_CascadeBorder = value;
			}
		}

		public float shadowDepthBias
		{
			get
			{
				return this.m_ShadowDepthBias;
			}
			set
			{
				this.m_ShadowDepthBias = this.ValidateShadowBias(value);
			}
		}

		public float shadowNormalBias
		{
			get
			{
				return this.m_ShadowNormalBias;
			}
			set
			{
				this.m_ShadowNormalBias = this.ValidateShadowBias(value);
			}
		}

		public bool supportsSoftShadows
		{
			get
			{
				return this.m_SoftShadowsSupported;
			}
			internal set
			{
				this.m_SoftShadowsSupported = value;
			}
		}

		internal SoftShadowQuality softShadowQuality
		{
			get
			{
				return this.m_SoftShadowQuality;
			}
			set
			{
				this.m_SoftShadowQuality = value;
			}
		}

		public bool supportsDynamicBatching
		{
			get
			{
				return this.m_SupportsDynamicBatching;
			}
			set
			{
				this.m_SupportsDynamicBatching = value;
			}
		}

		public bool supportsMixedLighting
		{
			get
			{
				return this.m_MixedLightingSupported;
			}
		}

		public bool supportsLightCookies
		{
			get
			{
				return this.m_SupportsLightCookies;
			}
		}

		[Obsolete("This is obsolete, use useRenderingLayers instead.", true)]
		public bool supportsLightLayers
		{
			get
			{
				return this.m_SupportsLightLayers;
			}
		}

		public bool useRenderingLayers
		{
			get
			{
				return this.m_SupportsLightLayers;
			}
		}

		public VolumeFrameworkUpdateMode volumeFrameworkUpdateMode
		{
			get
			{
				return this.m_VolumeFrameworkUpdateMode;
			}
		}

		public VolumeProfile volumeProfile
		{
			get
			{
				return this.m_VolumeProfile;
			}
			set
			{
				this.m_VolumeProfile = value;
			}
		}

		[Obsolete("PipelineDebugLevel is deprecated and replaced to use the profiler. Calling debugLevel is not necessary.", true)]
		public PipelineDebugLevel debugLevel
		{
			get
			{
				return PipelineDebugLevel.Disabled;
			}
		}

		public bool useSRPBatcher
		{
			get
			{
				return this.m_UseSRPBatcher;
			}
			set
			{
				this.m_UseSRPBatcher = value;
			}
		}

		[Obsolete("This has been deprecated, please use GraphicsSettings.GetRenderPipelineSettings<RenderGraphSettings>().enableRenderCompatibilityMode instead.")]
		public bool enableRenderGraph
		{
			get
			{
				RenderGraphSettings renderGraphSettings;
				return RenderGraphGraphicsAutomatedTests.enabled || (GraphicsSettings.TryGetRenderPipelineSettings<RenderGraphSettings>(out renderGraphSettings) && !renderGraphSettings.enableRenderCompatibilityMode);
			}
		}

		internal void OnEnableRenderGraphChanged()
		{
			this.OnValidate();
		}

		public ColorGradingMode colorGradingMode
		{
			get
			{
				return this.m_ColorGradingMode;
			}
			set
			{
				this.m_ColorGradingMode = value;
			}
		}

		public int colorGradingLutSize
		{
			get
			{
				return this.m_ColorGradingLutSize;
			}
			set
			{
				this.m_ColorGradingLutSize = Mathf.Clamp(value, 16, 65);
			}
		}

		public bool allowPostProcessAlphaOutput
		{
			get
			{
				return this.m_AllowPostProcessAlphaOutput;
			}
		}

		public bool useFastSRGBLinearConversion
		{
			get
			{
				return this.m_UseFastSRGBLinearConversion;
			}
		}

		public bool supportScreenSpaceLensFlare
		{
			get
			{
				return this.m_SupportScreenSpaceLensFlare;
			}
		}

		public bool supportDataDrivenLensFlare
		{
			get
			{
				return this.m_SupportDataDrivenLensFlare;
			}
		}

		public bool useAdaptivePerformance
		{
			get
			{
				return this.m_UseAdaptivePerformance;
			}
			set
			{
				this.m_UseAdaptivePerformance = value;
			}
		}

		public bool conservativeEnclosingSphere
		{
			get
			{
				return this.m_ConservativeEnclosingSphere;
			}
			set
			{
				this.m_ConservativeEnclosingSphere = value;
			}
		}

		public int numIterationsEnclosingSphere
		{
			get
			{
				return this.m_NumIterationsEnclosingSphere;
			}
			set
			{
				this.m_NumIterationsEnclosingSphere = value;
			}
		}

		public override string renderPipelineShaderTag
		{
			get
			{
				return "UniversalPipeline";
			}
		}

		[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)", false)]
		public override string[] renderingLayerMaskNames
		{
			get
			{
				return RenderingLayerMask.GetDefinedRenderingLayerNames();
			}
		}

		[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)", false)]
		public override string[] prefixedRenderingLayerMaskNames
		{
			get
			{
				return Array.Empty<string>();
			}
		}

		[Obsolete("This is obsolete, please use renderingLayerMaskNames instead.", true)]
		public string[] lightLayerMaskNames
		{
			get
			{
				return new string[0];
			}
		}

		public GPUResidentDrawerMode gpuResidentDrawerMode
		{
			get
			{
				return this.m_GPUResidentDrawerMode;
			}
			set
			{
				if (value == this.m_GPUResidentDrawerMode)
				{
					return;
				}
				this.m_GPUResidentDrawerMode = value;
				this.OnValidate();
			}
		}

		public bool gpuResidentDrawerEnableOcclusionCullingInCameras
		{
			get
			{
				return this.m_GPUResidentDrawerEnableOcclusionCullingInCameras;
			}
			set
			{
				if (value == this.m_GPUResidentDrawerEnableOcclusionCullingInCameras)
				{
					return;
				}
				this.m_GPUResidentDrawerEnableOcclusionCullingInCameras = value;
				this.OnValidate();
			}
		}

		public bool IsGPUResidentDrawerSupportedBySRP(out string message, out LogType severity)
		{
			message = string.Empty;
			severity = LogType.Warning;
			ScriptableRendererData[] rendererDataList = this.m_RendererDataList;
			for (int i = 0; i < rendererDataList.Length; i++)
			{
				UniversalRendererData universalRendererData = rendererDataList[i] as UniversalRendererData;
				if (universalRendererData == null)
				{
					message = UniversalRenderPipelineAsset.Strings.notURPRenderer;
					return false;
				}
				if (!universalRendererData.usesClusterLightLoop)
				{
					message = UniversalRenderPipelineAsset.Strings.renderingModeIncompatible;
					return false;
				}
			}
			return true;
		}

		public float smallMeshScreenPercentage
		{
			get
			{
				return this.m_SmallMeshScreenPercentage;
			}
			set
			{
				if (Math.Abs(value - this.m_SmallMeshScreenPercentage) < 1E-45f)
				{
					return;
				}
				this.m_SmallMeshScreenPercentage = Mathf.Clamp(value, 0f, 20f);
				this.OnValidate();
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.k_AssetVersion < 3)
			{
				this.m_SoftShadowsSupported = (this.m_ShadowType == ShadowQuality.SoftShadows);
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 3;
			}
			if (this.k_AssetVersion < 4)
			{
				this.m_AdditionalLightShadowsSupported = this.m_LocalShadowsSupported;
				this.m_AdditionalLightsShadowmapResolution = this.m_LocalShadowsAtlasResolution;
				this.m_AdditionalLightsPerObjectLimit = this.m_MaxPixelLights;
				this.m_MainLightShadowmapResolution = this.m_ShadowAtlasResolution;
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 4;
			}
			if (this.k_AssetVersion < 5)
			{
				if (this.m_RendererType == RendererType.Custom)
				{
					this.m_RendererDataList[0] = this.m_RendererData;
				}
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 5;
			}
			if (this.k_AssetVersion < 6)
			{
				int shadowCascades = (int)this.m_ShadowCascades;
				if (shadowCascades == 2)
				{
					this.m_ShadowCascadeCount = 4;
				}
				else
				{
					this.m_ShadowCascadeCount = shadowCascades + 1;
				}
				this.k_AssetVersion = 6;
			}
			if (this.k_AssetVersion < 7)
			{
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 7;
			}
			if (this.k_AssetVersion < 8)
			{
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.m_CascadeBorder = 0.1f;
				this.k_AssetVersion = 8;
			}
			if (this.k_AssetVersion < 9)
			{
				if (this.m_AdditionalLightsShadowResolutionTierHigh == UniversalRenderPipelineAsset.AdditionalLightsDefaultShadowResolutionTierHigh && this.m_AdditionalLightsShadowResolutionTierMedium == UniversalRenderPipelineAsset.AdditionalLightsDefaultShadowResolutionTierMedium && this.m_AdditionalLightsShadowResolutionTierLow == UniversalRenderPipelineAsset.AdditionalLightsDefaultShadowResolutionTierLow)
				{
					this.m_AdditionalLightsShadowResolutionTierHigh = (int)this.m_AdditionalLightsShadowmapResolution;
					this.m_AdditionalLightsShadowResolutionTierMedium = Mathf.Max(this.m_AdditionalLightsShadowResolutionTierHigh / 2, UniversalAdditionalLightData.AdditionalLightsShadowMinimumResolution);
					this.m_AdditionalLightsShadowResolutionTierLow = Mathf.Max(this.m_AdditionalLightsShadowResolutionTierMedium / 2, UniversalAdditionalLightData.AdditionalLightsShadowMinimumResolution);
				}
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 9;
			}
			if (this.k_AssetVersion < 10)
			{
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 10;
			}
			if (this.k_AssetVersion < 11)
			{
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 11;
			}
			if (this.k_AssetVersion < 12)
			{
				this.k_AssetPreviousVersion = this.k_AssetVersion;
				this.k_AssetVersion = 12;
			}
		}

		private float ValidateShadowBias(float value)
		{
			return Mathf.Max(0f, Mathf.Min(value, UniversalRenderPipeline.maxShadowBias));
		}

		private int ValidatePerObjectLights(int value)
		{
			return Math.Max(0, Math.Min(value, UniversalRenderPipeline.maxPerObjectLights));
		}

		private float ValidateRenderScale(float value)
		{
			return Mathf.Max(UniversalRenderPipeline.minRenderScale, Mathf.Min(value, UniversalRenderPipeline.maxRenderScale));
		}

		internal bool ValidateRendererDataList(bool partial = false)
		{
			int num = 0;
			for (int i = 0; i < this.m_RendererDataList.Length; i++)
			{
				num += (this.ValidateRendererData(i) ? 0 : 1);
			}
			if (partial)
			{
				return num == 0;
			}
			return num != this.m_RendererDataList.Length;
		}

		internal bool ValidateRendererData(int index)
		{
			if (index == -1)
			{
				index = this.m_DefaultRendererIndex;
			}
			return index < this.m_RendererDataList.Length && this.m_RendererDataList[index] != null;
		}

		public bool supportProbeVolume
		{
			get
			{
				return this.lightProbeSystem == LightProbeSystem.ProbeVolumes;
			}
		}

		public ProbeVolumeSHBands maxSHBands
		{
			get
			{
				if (this.lightProbeSystem == LightProbeSystem.ProbeVolumes)
				{
					return this.probeVolumeSHBands;
				}
				return ProbeVolumeSHBands.SphericalHarmonicsL1;
			}
		}

		[Obsolete("This property is no longer necessary.")]
		public ProbeVolumeSceneData probeVolumeSceneData
		{
			get
			{
				return null;
			}
		}

		public bool isStpUsed
		{
			get
			{
				return this.m_UpscalingFilter == UpscalingFilterSelection.STP;
			}
		}

		private Material GetMaterial(DefaultMaterialType materialType)
		{
			return null;
		}

		public override Material defaultMaterial
		{
			get
			{
				return this.GetMaterial(DefaultMaterialType.Default);
			}
		}

		public override Material defaultParticleMaterial
		{
			get
			{
				return this.GetMaterial(DefaultMaterialType.Particle);
			}
		}

		public override Material defaultLineMaterial
		{
			get
			{
				return this.GetMaterial(DefaultMaterialType.Particle);
			}
		}

		public override Material defaultTerrainMaterial
		{
			get
			{
				return this.GetMaterial(DefaultMaterialType.Terrain);
			}
		}

		public override Material default2DMaterial
		{
			get
			{
				return this.GetMaterial(DefaultMaterialType.Sprite);
			}
		}

		public override Material default2DMaskMaterial
		{
			get
			{
				return this.GetMaterial(DefaultMaterialType.SpriteMask);
			}
		}

		public Material decalMaterial
		{
			get
			{
				return this.GetMaterial(DefaultMaterialType.Decal);
			}
		}

		public override Shader defaultShader
		{
			get
			{
				if (this.m_DefaultShader == null)
				{
					this.m_DefaultShader = Shader.Find(ShaderUtils.GetShaderPath(ShaderPathID.Lit));
				}
				return this.m_DefaultShader;
			}
		}

		public override Shader terrainDetailLitShader
		{
			get
			{
				UniversalRenderPipelineRuntimeShaders universalRenderPipelineRuntimeShaders;
				if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeShaders>(out universalRenderPipelineRuntimeShaders))
				{
					return universalRenderPipelineRuntimeShaders.terrainDetailLitShader;
				}
				return null;
			}
		}

		public override Shader terrainDetailGrassShader
		{
			get
			{
				UniversalRenderPipelineRuntimeShaders universalRenderPipelineRuntimeShaders;
				if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeShaders>(out universalRenderPipelineRuntimeShaders))
				{
					return universalRenderPipelineRuntimeShaders.terrainDetailGrassShader;
				}
				return null;
			}
		}

		public override Shader terrainDetailGrassBillboardShader
		{
			get
			{
				UniversalRenderPipelineRuntimeShaders universalRenderPipelineRuntimeShaders;
				if (GraphicsSettings.TryGetRenderPipelineSettings<UniversalRenderPipelineRuntimeShaders>(out universalRenderPipelineRuntimeShaders))
				{
					return universalRenderPipelineRuntimeShaders.terrainDetailGrassBillboardShader;
				}
				return null;
			}
		}

		[Obsolete("Use GraphicsSettings.GetRenderPipelineSettings<ShaderStrippingSetting>().shaderVariantLogLevel instead.", true)]
		public ShaderVariantLogLevel shaderVariantLogLevel
		{
			get
			{
				return (ShaderVariantLogLevel)GraphicsSettings.GetRenderPipelineSettings<ShaderStrippingSetting>().shaderVariantLogLevel;
			}
			set
			{
				GraphicsSettings.GetRenderPipelineSettings<ShaderStrippingSetting>().shaderVariantLogLevel = (ShaderVariantLogLevel)value;
			}
		}

		[Obsolete("This is obsolete, please use shadowCascadeCount instead.", true)]
		public ShadowCascadesOption shadowCascadeOption
		{
			get
			{
				switch (this.shadowCascadeCount)
				{
				case 1:
					return ShadowCascadesOption.NoCascades;
				case 2:
					return ShadowCascadesOption.TwoCascades;
				case 4:
					return ShadowCascadesOption.FourCascades;
				}
				throw new InvalidOperationException("Cascade count is not compatible with obsolete API, please use shadowCascadeCount instead.");
			}
			set
			{
				switch (value)
				{
				case ShadowCascadesOption.NoCascades:
					this.shadowCascadeCount = 1;
					return;
				case ShadowCascadesOption.TwoCascades:
					this.shadowCascadeCount = 2;
					return;
				case ShadowCascadesOption.FourCascades:
					this.shadowCascadeCount = 4;
					return;
				default:
					throw new InvalidOperationException("Cascade count is not compatible with obsolete API, please use shadowCascadeCount instead.");
				}
			}
		}

		[Obsolete("Moved to UniversalRenderPipelineRuntimeTextures on GraphicsSettings. #from(2023.3)", false)]
		public UniversalRenderPipelineAsset.TextureResources textures
		{
			get
			{
				if (this.m_Textures == null)
				{
					this.m_Textures = new UniversalRenderPipelineAsset.TextureResources();
				}
				return this.m_Textures;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static UniversalRenderPipelineAsset()
		{
			GraphicsFormat[][] array = new GraphicsFormat[5][];
			array[0] = new GraphicsFormat[]
			{
				GraphicsFormat.R8_UNorm
			};
			array[1] = new GraphicsFormat[]
			{
				GraphicsFormat.R16_UNorm
			};
			int num = 2;
			GraphicsFormat[] array2 = new GraphicsFormat[4];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.08243D32F28C35701F6EA57F52AE707302C8528E8D358F13C6E6915543D265C6).FieldHandle);
			array[num] = array2;
			int num2 = 3;
			GraphicsFormat[] array3 = new GraphicsFormat[3];
			RuntimeHelpers.InitializeArray(array3, fieldof(<PrivateImplementationDetails>.9D3A6E7E88415D8C1A0F3887B6384A9A8E4F44A036C5A24796C319751ACACCAD).FieldHandle);
			array[num2] = array3;
			array[4] = new GraphicsFormat[]
			{
				GraphicsFormat.B10G11R11_UFloatPack32
			};
			UniversalRenderPipelineAsset.s_LightCookieFormatList = array;
		}

		private ScriptableRenderer[] m_Renderers = new ScriptableRenderer[1];

		private const int k_LastVersion = 12;

		[SerializeField]
		private int k_AssetVersion = 12;

		[SerializeField]
		private int k_AssetPreviousVersion = 12;

		[SerializeField]
		private RendererType m_RendererType = RendererType.UniversalRenderer;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use m_RendererDataList instead.")]
		[SerializeField]
		internal ScriptableRendererData m_RendererData;

		[SerializeField]
		internal ScriptableRendererData[] m_RendererDataList = new ScriptableRendererData[1];

		[SerializeField]
		internal int m_DefaultRendererIndex;

		[SerializeField]
		private bool m_RequireDepthTexture;

		[SerializeField]
		private bool m_RequireOpaqueTexture;

		[SerializeField]
		private Downsampling m_OpaqueDownsampling = Downsampling._2xBilinear;

		[SerializeField]
		private bool m_SupportsTerrainHoles = true;

		[SerializeField]
		private bool m_SupportsHDR = true;

		[SerializeField]
		private HDRColorBufferPrecision m_HDRColorBufferPrecision;

		[SerializeField]
		private MsaaQuality m_MSAA = MsaaQuality.Disabled;

		[SerializeField]
		private float m_RenderScale = 1f;

		[SerializeField]
		private UpscalingFilterSelection m_UpscalingFilter;

		[SerializeField]
		private bool m_FsrOverrideSharpness;

		[SerializeField]
		private float m_FsrSharpness = 0.92f;

		[SerializeField]
		private bool m_EnableLODCrossFade = true;

		[SerializeField]
		private LODCrossFadeDitheringType m_LODCrossFadeDitheringType = LODCrossFadeDitheringType.BlueNoise;

		[SerializeField]
		private ShEvalMode m_ShEvalMode;

		[SerializeField]
		private LightProbeSystem m_LightProbeSystem;

		[SerializeField]
		private ProbeVolumeTextureMemoryBudget m_ProbeVolumeMemoryBudget = ProbeVolumeTextureMemoryBudget.MemoryBudgetMedium;

		[SerializeField]
		private ProbeVolumeBlendingTextureMemoryBudget m_ProbeVolumeBlendingMemoryBudget = ProbeVolumeBlendingTextureMemoryBudget.MemoryBudgetMedium;

		[SerializeField]
		[FormerlySerializedAs("m_SupportProbeVolumeStreaming")]
		private bool m_SupportProbeVolumeGPUStreaming;

		[SerializeField]
		private bool m_SupportProbeVolumeDiskStreaming;

		[SerializeField]
		private bool m_SupportProbeVolumeScenarios;

		[SerializeField]
		private bool m_SupportProbeVolumeScenarioBlending;

		[SerializeField]
		private ProbeVolumeSHBands m_ProbeVolumeSHBands = ProbeVolumeSHBands.SphericalHarmonicsL1;

		[SerializeField]
		private LightRenderingMode m_MainLightRenderingMode = LightRenderingMode.PerPixel;

		[SerializeField]
		private bool m_MainLightShadowsSupported = true;

		[SerializeField]
		private ShadowResolution m_MainLightShadowmapResolution = ShadowResolution._2048;

		[SerializeField]
		private LightRenderingMode m_AdditionalLightsRenderingMode = LightRenderingMode.PerPixel;

		[SerializeField]
		private int m_AdditionalLightsPerObjectLimit = 4;

		[SerializeField]
		private bool m_AdditionalLightShadowsSupported;

		[SerializeField]
		private ShadowResolution m_AdditionalLightsShadowmapResolution = ShadowResolution._2048;

		[SerializeField]
		private int m_AdditionalLightsShadowResolutionTierLow = UniversalRenderPipelineAsset.AdditionalLightsDefaultShadowResolutionTierLow;

		[SerializeField]
		private int m_AdditionalLightsShadowResolutionTierMedium = UniversalRenderPipelineAsset.AdditionalLightsDefaultShadowResolutionTierMedium;

		[SerializeField]
		private int m_AdditionalLightsShadowResolutionTierHigh = UniversalRenderPipelineAsset.AdditionalLightsDefaultShadowResolutionTierHigh;

		[SerializeField]
		private bool m_ReflectionProbeBlending;

		[SerializeField]
		private bool m_ReflectionProbeBoxProjection;

		[SerializeField]
		private bool m_ReflectionProbeAtlas = true;

		[SerializeField]
		private float m_ShadowDistance = 50f;

		[SerializeField]
		private int m_ShadowCascadeCount = 1;

		[SerializeField]
		private float m_Cascade2Split = 0.25f;

		[SerializeField]
		private Vector2 m_Cascade3Split = new Vector2(0.1f, 0.3f);

		[SerializeField]
		private Vector3 m_Cascade4Split = new Vector3(0.067f, 0.2f, 0.467f);

		[SerializeField]
		private float m_CascadeBorder = 0.2f;

		[SerializeField]
		private float m_ShadowDepthBias = 1f;

		[SerializeField]
		private float m_ShadowNormalBias = 1f;

		[SerializeField]
		private bool m_SoftShadowsSupported;

		[SerializeField]
		private bool m_ConservativeEnclosingSphere;

		[SerializeField]
		private int m_NumIterationsEnclosingSphere = 64;

		[SerializeField]
		private SoftShadowQuality m_SoftShadowQuality = SoftShadowQuality.Medium;

		[SerializeField]
		private LightCookieResolution m_AdditionalLightsCookieResolution = LightCookieResolution._2048;

		[SerializeField]
		private LightCookieFormat m_AdditionalLightsCookieFormat = LightCookieFormat.ColorHigh;

		[SerializeField]
		private bool m_UseSRPBatcher = true;

		[SerializeField]
		private bool m_SupportsDynamicBatching;

		[SerializeField]
		private bool m_MixedLightingSupported = true;

		[SerializeField]
		private bool m_SupportsLightCookies = true;

		[SerializeField]
		private bool m_SupportsLightLayers;

		[SerializeField]
		[Obsolete("", true)]
		private PipelineDebugLevel m_DebugLevel;

		[SerializeField]
		private StoreActionsOptimization m_StoreActionsOptimization;

		[SerializeField]
		private bool m_UseAdaptivePerformance = true;

		[SerializeField]
		private ColorGradingMode m_ColorGradingMode;

		[SerializeField]
		private int m_ColorGradingLutSize = 32;

		[SerializeField]
		private bool m_AllowPostProcessAlphaOutput;

		[SerializeField]
		private bool m_UseFastSRGBLinearConversion;

		[SerializeField]
		private bool m_SupportDataDrivenLensFlare = true;

		[SerializeField]
		private bool m_SupportScreenSpaceLensFlare = true;

		[FormerlySerializedAs("m_MacroBatcherMode")]
		[SerializeField]
		private GPUResidentDrawerMode m_GPUResidentDrawerMode;

		[SerializeField]
		private float m_SmallMeshScreenPercentage;

		[SerializeField]
		private bool m_GPUResidentDrawerEnableOcclusionCullingInCameras;

		[SerializeField]
		private ShadowQuality m_ShadowType = ShadowQuality.HardShadows;

		[SerializeField]
		private bool m_LocalShadowsSupported;

		[SerializeField]
		private ShadowResolution m_LocalShadowsAtlasResolution = ShadowResolution._256;

		[SerializeField]
		private int m_MaxPixelLights;

		[SerializeField]
		private ShadowResolution m_ShadowAtlasResolution = ShadowResolution._256;

		[SerializeField]
		private VolumeFrameworkUpdateMode m_VolumeFrameworkUpdateMode;

		[SerializeField]
		private VolumeProfile m_VolumeProfile;

		public const int k_MinLutSize = 16;

		public const int k_MaxLutSize = 65;

		internal const int k_ShadowCascadeMinCount = 1;

		internal const int k_ShadowCascadeMaxCount = 4;

		public static readonly int AdditionalLightsDefaultShadowResolutionTierLow = 256;

		public static readonly int AdditionalLightsDefaultShadowResolutionTierMedium = 512;

		public static readonly int AdditionalLightsDefaultShadowResolutionTierHigh = 1024;

		private static string[] s_Names;

		private static int[] s_Values;

		private static GraphicsFormat[][] s_LightCookieFormatList;

		[SerializeField]
		[Obsolete("Kept for migration. #from(2023.3")]
		internal ProbeVolumeSceneData apvScenesData;

		private Shader m_DefaultShader;

		[SerializeField]
		private int m_ShaderVariantLogLevel;

		[Obsolete("This is obsolete, please use shadowCascadeCount instead.", false)]
		[SerializeField]
		private ShadowCascadesOption m_ShadowCascades;

		[Obsolete("Moved to UniversalRenderPipelineRuntimeTextures on GraphicsSettings. #from(2023.3)", false)]
		[SerializeField]
		private UniversalRenderPipelineAsset.TextureResources m_Textures;

		private static class Strings
		{
			public static readonly string notURPRenderer = "GPUResidentDrawer Disabled due to some configured Universal Renderers not being UniversalRendererData.";

			public static readonly string renderingModeIncompatible = "GPUResidentDrawer Disabled due to some configured Universal Renderers not using the Forward+ or Deferred+ rendering paths.";
		}

		[ReloadGroup]
		[Obsolete("Moved to UniversalRenderPipelineRuntimeTextures on GraphicsSettings. #from(2023.3)", false)]
		[Serializable]
		public sealed class TextureResources
		{
			public bool NeedsReload()
			{
				return this.blueNoise64LTex == null || this.bayerMatrixTex == null;
			}

			[Reload("Textures/BlueNoise64/L/LDR_LLL1_0.png", ReloadAttribute.Package.Root)]
			public Texture2D blueNoise64LTex;

			[Reload("Textures/BayerMatrix.png", ReloadAttribute.Package.Root)]
			public Texture2D bayerMatrixTex;
		}
	}
}
