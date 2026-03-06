using System;

namespace UnityEngine.Rendering
{
	[Flags]
	internal enum InstanceComponentGroup : uint
	{
		Default = 1U,
		Wind = 2U,
		LightProbe = 4U,
		Lightmap = 8U,
		DefaultWind = 3U,
		DefaultLightProbe = 5U,
		DefaultLightmap = 9U,
		DefaultWindLightProbe = 7U,
		DefaultWindLightmap = 11U
	}
}
