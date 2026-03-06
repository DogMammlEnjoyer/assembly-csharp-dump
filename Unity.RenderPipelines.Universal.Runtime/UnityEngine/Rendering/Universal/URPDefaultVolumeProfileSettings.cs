using System;
using UnityEngine.Categorization;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
	[CategoryInfo(Name = "Volume", Order = 0)]
	[Serializable]
	public class URPDefaultVolumeProfileSettings : IDefaultVolumeProfileSettings, IRenderPipelineGraphicsSettings
	{
		public int version
		{
			get
			{
				return (int)this.m_Version;
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
				this.SetValueAndNotify(ref this.m_VolumeProfile, value, "volumeProfile");
			}
		}

		[SerializeField]
		[HideInInspector]
		private URPDefaultVolumeProfileSettings.Version m_Version;

		[SerializeField]
		private VolumeProfile m_VolumeProfile;

		internal enum Version
		{
			Initial
		}
	}
}
