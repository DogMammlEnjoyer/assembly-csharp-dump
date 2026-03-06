using System;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderer(typeof(UniversalRendererData))]
	[DisallowMultipleRendererFeature("Screen Space Ambient Occlusion")]
	[Tooltip("The Ambient Occlusion effect darkens creases, holes, intersections and surfaces that are close to each other.")]
	public class ScreenSpaceAmbientOcclusion : ScriptableRendererFeature
	{
		internal ref ScreenSpaceAmbientOcclusionSettings settings
		{
			get
			{
				return ref this.m_Settings;
			}
		}

		public override void Create()
		{
			if (this.m_SSAOPass == null)
			{
				this.m_SSAOPass = new ScreenSpaceAmbientOcclusionPass();
			}
			if (this.m_Settings.SampleCount > 0)
			{
				this.m_Settings.AOMethod = ScreenSpaceAmbientOcclusionSettings.AOMethodOptions.InterleavedGradient;
				if (this.m_Settings.SampleCount > 11)
				{
					this.m_Settings.Samples = ScreenSpaceAmbientOcclusionSettings.AOSampleOption.High;
				}
				else if (this.m_Settings.SampleCount > 8)
				{
					this.m_Settings.Samples = ScreenSpaceAmbientOcclusionSettings.AOSampleOption.Medium;
				}
				else
				{
					this.m_Settings.Samples = ScreenSpaceAmbientOcclusionSettings.AOSampleOption.Low;
				}
				this.m_Settings.SampleCount = -1;
			}
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
			{
				return;
			}
			if (!this.TryPrepareResources())
			{
				return;
			}
			if (this.m_SSAOPass.Setup(ref this.m_Settings, ref renderer, ref this.m_Material, ref this.m_BlueNoise256Textures))
			{
				renderer.EnqueuePass(this.m_SSAOPass);
			}
		}

		protected override void Dispose(bool disposing)
		{
			ScreenSpaceAmbientOcclusionPass ssaopass = this.m_SSAOPass;
			if (ssaopass != null)
			{
				ssaopass.Dispose();
			}
			this.m_SSAOPass = null;
			CoreUtils.Destroy(this.m_Material);
		}

		private bool TryPrepareResources()
		{
			if (this.m_Shader == null)
			{
				ScreenSpaceAmbientOcclusionPersistentResources screenSpaceAmbientOcclusionPersistentResources;
				if (!GraphicsSettings.TryGetRenderPipelineSettings<ScreenSpaceAmbientOcclusionPersistentResources>(out screenSpaceAmbientOcclusionPersistentResources))
				{
					Debug.LogErrorFormat("Couldn't find the required resources for the ScreenSpaceAmbientOcclusion render feature. If this exception appears in the Player, make sure at least one ScreenSpaceAmbientOcclusion render feature is enabled or adjust your stripping settings.", Array.Empty<object>());
					return false;
				}
				this.m_Shader = screenSpaceAmbientOcclusionPersistentResources.Shader;
			}
			if (this.m_Settings.AOMethod == ScreenSpaceAmbientOcclusionSettings.AOMethodOptions.BlueNoise && (this.m_BlueNoise256Textures == null || this.m_BlueNoise256Textures.Length == 0))
			{
				ScreenSpaceAmbientOcclusionDynamicResources screenSpaceAmbientOcclusionDynamicResources;
				if (!GraphicsSettings.TryGetRenderPipelineSettings<ScreenSpaceAmbientOcclusionDynamicResources>(out screenSpaceAmbientOcclusionDynamicResources))
				{
					Debug.LogErrorFormat("Couldn't load BlueNoise256Textures. If this exception appears in the Player, please check the SSAO options for ScreenSpaceAmbientOcclusion or adjust your stripping settings", Array.Empty<object>());
					return false;
				}
				this.m_BlueNoise256Textures = screenSpaceAmbientOcclusionDynamicResources.BlueNoise256Textures;
			}
			if (this.m_Material == null && this.m_Shader != null)
			{
				this.m_Material = CoreUtils.CreateEngineMaterial(this.m_Shader);
			}
			if (this.m_Material == null)
			{
				Debug.LogError(base.GetType().Name + ".AddRenderPasses(): Missing material. " + base.name + " render pass will not be added.");
				return false;
			}
			return true;
		}

		[SerializeField]
		private ScreenSpaceAmbientOcclusionSettings m_Settings = new ScreenSpaceAmbientOcclusionSettings();

		private Material m_Material;

		private ScreenSpaceAmbientOcclusionPass m_SSAOPass;

		private Shader m_Shader;

		private Texture2D[] m_BlueNoise256Textures;

		internal const string k_AOInterleavedGradientKeyword = "_INTERLEAVED_GRADIENT";

		internal const string k_AOBlueNoiseKeyword = "_BLUE_NOISE";

		internal const string k_OrthographicCameraKeyword = "_ORTHOGRAPHIC";

		internal const string k_SourceDepthLowKeyword = "_SOURCE_DEPTH_LOW";

		internal const string k_SourceDepthMediumKeyword = "_SOURCE_DEPTH_MEDIUM";

		internal const string k_SourceDepthHighKeyword = "_SOURCE_DEPTH_HIGH";

		internal const string k_SourceDepthNormalsKeyword = "_SOURCE_DEPTH_NORMALS";

		internal const string k_SampleCountLowKeyword = "_SAMPLE_COUNT_LOW";

		internal const string k_SampleCountMediumKeyword = "_SAMPLE_COUNT_MEDIUM";

		internal const string k_SampleCountHighKeyword = "_SAMPLE_COUNT_HIGH";
	}
}
