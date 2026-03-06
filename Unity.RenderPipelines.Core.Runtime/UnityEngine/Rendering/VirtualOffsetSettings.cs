using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	internal struct VirtualOffsetSettings
	{
		internal void SetDefaults()
		{
			this.useVirtualOffset = true;
			this.validityThreshold = 0.25f;
			this.outOfGeoOffset = 0.01f;
			this.searchMultiplier = 0.2f;
			this.UpgradeFromTo(ProbeVolumeBakingProcessSettings.SettingsVersion.Initial, ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset);
		}

		internal void UpgradeFromTo(ProbeVolumeBakingProcessSettings.SettingsVersion from, ProbeVolumeBakingProcessSettings.SettingsVersion to)
		{
			if (from < ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset && to >= ProbeVolumeBakingProcessSettings.SettingsVersion.ThreadedVirtualOffset)
			{
				this.rayOriginBias = -0.001f;
				this.collisionMask = -5;
			}
		}

		public bool useVirtualOffset;

		[Range(0f, 0.95f)]
		public float validityThreshold;

		[Range(0f, 1f)]
		public float outOfGeoOffset;

		[Range(0f, 2f)]
		public float searchMultiplier;

		[Range(-0.05f, 0f)]
		public float rayOriginBias;

		public LayerMask collisionMask;
	}
}
