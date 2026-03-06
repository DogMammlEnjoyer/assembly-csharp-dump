using System;

namespace UnityEngine.Rendering.Universal
{
	internal struct LightStats
	{
		public bool useLights
		{
			get
			{
				return this.totalLights > 0;
			}
		}

		public bool useShadows
		{
			get
			{
				return this.totalShadows > 0;
			}
		}

		public bool useVolumetricLights
		{
			get
			{
				return this.totalVolumetricUsage > 0;
			}
		}

		public bool useVolumetricShadowLights
		{
			get
			{
				return this.totalVolumetricShadowUsage > 0;
			}
		}

		public bool useNormalMap
		{
			get
			{
				return this.totalNormalMapUsage > 0;
			}
		}

		public int totalLights;

		public int totalShadowLights;

		public int totalShadows;

		public int totalNormalMapUsage;

		public int totalVolumetricUsage;

		public int totalVolumetricShadowUsage;

		public uint blendStylesUsed;

		public uint blendStylesWithLights;
	}
}
