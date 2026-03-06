using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering
{
	[SupportedOnRenderPipeline(new Type[]
	{

	})]
	[CategoryInfo(Name = "Adaptive Probe Volumes", Order = 20)]
	[Serializable]
	internal class ProbeVolumeGlobalSettings : IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return this.m_Version;
			}
		}

		public bool probeVolumeDisableStreamingAssets
		{
			get
			{
				return this.m_ProbeVolumeDisableStreamingAssets;
			}
			set
			{
				this.SetValueAndNotify(ref this.m_ProbeVolumeDisableStreamingAssets, value, "m_ProbeVolumeDisableStreamingAssets");
			}
		}

		[SerializeField]
		[HideInInspector]
		private int m_Version = 1;

		[SerializeField]
		[Tooltip("Enabling this will make APV baked data assets compatible with Addressables and Asset Bundles. This will also make Disk Streaming unavailable. After changing this setting, a clean rebuild may be required for data assets to be included in Adressables and Asset Bundles.")]
		private bool m_ProbeVolumeDisableStreamingAssets;
	}
}
