using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "Lightmap Sampling Settings", Order = 20)]
	[Serializable]
	public class LightmapSamplingSettings : IRenderPipelineGraphicsSettings
	{
		int IRenderPipelineGraphicsSettings.version
		{
			get
			{
				return this.m_Version;
			}
		}

		bool IRenderPipelineGraphicsSettings.isAvailableInPlayerBuild
		{
			get
			{
				return true;
			}
		}

		public bool useBicubicLightmapSampling
		{
			get
			{
				return this.m_UseBicubicLightmapSampling;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_UseBicubicLightmapSampling, value, "m_UseBicubicLightmapSampling");
			}
		}

		[SerializeField]
		[HideInInspector]
		private int m_Version = 1;

		[SerializeField]
		[Tooltip("Use Bicubic Lightmap Sampling. Enabling this will improve the appearance of lightmaps, but may worsen performance on lower end platforms.")]
		private bool m_UseBicubicLightmapSampling;
	}
}
