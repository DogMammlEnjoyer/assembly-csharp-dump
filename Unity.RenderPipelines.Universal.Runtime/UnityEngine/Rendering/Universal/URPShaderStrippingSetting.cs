using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "Additional Shader Stripping Settings", Order = 40)]
	[ElementInfo(Order = 10)]
	[Serializable]
	public class URPShaderStrippingSetting : IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return (int)this.m_Version;
			}
		}

		public bool stripUnusedPostProcessingVariants
		{
			get
			{
				return this.m_StripUnusedPostProcessingVariants;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_StripUnusedPostProcessingVariants, value, "stripUnusedPostProcessingVariants");
			}
		}

		public bool stripUnusedVariants
		{
			get
			{
				return this.m_StripUnusedVariants;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_StripUnusedVariants, value, "stripUnusedVariants");
			}
		}

		public bool stripScreenCoordOverrideVariants
		{
			get
			{
				return this.m_StripScreenCoordOverrideVariants;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_StripScreenCoordOverrideVariants, value, "stripScreenCoordOverrideVariants");
			}
		}

		[SerializeField]
		[HideInInspector]
		private URPShaderStrippingSetting.Version m_Version;

		[SerializeField]
		[Tooltip("Controls whether to automatically strip post processing shader variants based on VolumeProfile components. Stripping is done based on VolumeProfiles in project, their usage in scenes is not considered.")]
		private bool m_StripUnusedPostProcessingVariants;

		[SerializeField]
		[Tooltip("Controls whether to strip variants if the feature is disabled.")]
		private bool m_StripUnusedVariants = true;

		[SerializeField]
		[Tooltip("Controls whether Screen Coordinates Override shader variants are automatically stripped.")]
		private bool m_StripScreenCoordOverrideVariants = true;

		internal enum Version
		{
			Initial
		}
	}
}
