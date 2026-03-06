using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	internal struct ProbeDilationSettings
	{
		internal void SetDefaults()
		{
			this.enableDilation = false;
			this.dilationDistance = 1f;
			this.dilationValidityThreshold = 0.25f;
			this.dilationIterations = 1;
			this.squaredDistWeighting = true;
		}

		internal void UpgradeFromTo(ProbeVolumeBakingProcessSettings.SettingsVersion from, ProbeVolumeBakingProcessSettings.SettingsVersion to)
		{
		}

		public bool enableDilation;

		public float dilationDistance;

		public float dilationValidityThreshold;

		public int dilationIterations;

		public bool squaredDistWeighting;
	}
}
