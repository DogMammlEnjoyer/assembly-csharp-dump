using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	internal struct ProbeVolumeBakingProcessSettings
	{
		internal static ProbeVolumeBakingProcessSettings Default
		{
			get
			{
				ProbeVolumeBakingProcessSettings result = default(ProbeVolumeBakingProcessSettings);
				result.SetDefaults();
				return result;
			}
		}

		internal ProbeVolumeBakingProcessSettings(ProbeDilationSettings dilationSettings, VirtualOffsetSettings virtualOffsetSettings)
		{
			this.m_Version = ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset;
			this.dilationSettings = dilationSettings;
			this.virtualOffsetSettings = virtualOffsetSettings;
		}

		internal void SetDefaults()
		{
			this.m_Version = ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset;
			this.dilationSettings.SetDefaults();
			this.virtualOffsetSettings.SetDefaults();
		}

		internal void Upgrade()
		{
			if (this.m_Version != ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset)
			{
				this.dilationSettings.UpgradeFromTo(this.m_Version, ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset);
				this.virtualOffsetSettings.UpgradeFromTo(this.m_Version, ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset);
				this.m_Version = ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset;
			}
		}

		[SerializeField]
		private ProbeVolumeBakingProcessSettings.SettingsVersion m_Version;

		public ProbeDilationSettings dilationSettings;

		public VirtualOffsetSettings virtualOffsetSettings;

		internal enum SettingsVersion
		{
			Initial,
			ThreadedVirtualOffset,
			Max,
			Current = 1
		}
	}
}
