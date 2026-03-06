using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	public abstract class ScriptableRendererData : ScriptableObject
	{
		internal bool isInvalidated { get; set; }

		internal virtual bool stripShadowsOffVariants
		{
			get
			{
				return this.m_StripShadowsOffVariants;
			}
			set
			{
				this.m_StripShadowsOffVariants = value;
			}
		}

		internal virtual bool stripAdditionalLightOffVariants
		{
			get
			{
				return this.m_StripAdditionalLightOffVariants;
			}
			set
			{
				this.m_StripAdditionalLightOffVariants = value;
			}
		}

		protected abstract ScriptableRenderer Create();

		public List<ScriptableRendererFeature> rendererFeatures
		{
			get
			{
				return this.m_RendererFeatures;
			}
		}

		public new void SetDirty()
		{
			this.isInvalidated = true;
		}

		internal ScriptableRenderer InternalCreateRenderer()
		{
			this.isInvalidated = false;
			return this.Create();
		}

		protected virtual void OnValidate()
		{
			this.SetDirty();
		}

		protected virtual void OnEnable()
		{
			this.SetDirty();
		}

		public bool useNativeRenderPass
		{
			get
			{
				return this.m_UseNativeRenderPass;
			}
			set
			{
				this.SetDirty();
				this.m_UseNativeRenderPass = value;
			}
		}

		public bool TryGetRendererFeature<T>(out T rendererFeature) where T : ScriptableRendererFeature
		{
			foreach (ScriptableRendererFeature scriptableRendererFeature in this.rendererFeatures)
			{
				if (scriptableRendererFeature.GetType() == typeof(T))
				{
					rendererFeature = (scriptableRendererFeature as T);
					return true;
				}
			}
			rendererFeature = default(T);
			return false;
		}

		[Obsolete("Moved to UniversalRenderPipelineDebugShaders on GraphicsSettings. #from(2023.3)", false)]
		public ScriptableRendererData.DebugShaderResources debugShaders;

		[Obsolete("Probe volume debug resource are now in the ProbeVolumeDebugResources class.")]
		public ScriptableRendererData.ProbeVolumeResources probeVolumeResources;

		[SerializeField]
		internal List<ScriptableRendererFeature> m_RendererFeatures = new List<ScriptableRendererFeature>(10);

		[SerializeField]
		internal List<long> m_RendererFeatureMap = new List<long>(10);

		[SerializeField]
		private bool m_UseNativeRenderPass;

		[NonSerialized]
		private bool m_StripShadowsOffVariants;

		[NonSerialized]
		private bool m_StripAdditionalLightOffVariants;

		[Obsolete("Moved to UniversalRenderPipelineDebugShaders on GraphicsSettings. #from(2023.3)", false)]
		[ReloadGroup]
		[Serializable]
		public sealed class DebugShaderResources
		{
			[Obsolete("Moved to UniversalRenderPipelineDebugShaders on GraphicsSettings. #from(2023.3)", false)]
			[Reload("Shaders/Debug/DebugReplacement.shader", ReloadAttribute.Package.Root)]
			public Shader debugReplacementPS;

			[Obsolete("Moved to UniversalRenderPipelineDebugShaders on GraphicsSettings. #from(2023.3)", false)]
			[Reload("Shaders/Debug/HDRDebugView.shader", ReloadAttribute.Package.Root)]
			public Shader hdrDebugViewPS;
		}

		[ReloadGroup]
		[Obsolete("Probe volume debug resource are now in the ProbeVolumeDebugResources class.")]
		[Serializable]
		public sealed class ProbeVolumeResources
		{
			[Obsolete("This shader is now in the ProbeVolumeDebugResources class.")]
			public Shader probeVolumeDebugShader;

			[Obsolete("This shader is now in the ProbeVolumeDebugResources class.")]
			public Shader probeVolumeFragmentationDebugShader;

			[Obsolete("This shader is now in the ProbeVolumeDebugResources class.")]
			public Shader probeVolumeOffsetDebugShader;

			[Obsolete("This shader is now in the ProbeVolumeDebugResources class.")]
			public Shader probeVolumeSamplingDebugShader;

			[Obsolete("This shader is now in the ProbeVolumeDebugResources class.")]
			public Mesh probeSamplingDebugMesh;

			[Obsolete("This shader is now in the ProbeVolumeDebugResources class.")]
			public Texture2D probeSamplingDebugTexture;

			[Obsolete("This shader is now in the ProbeVolumeRuntimeResources class.")]
			public ComputeShader probeVolumeBlendStatesCS;
		}
	}
}
