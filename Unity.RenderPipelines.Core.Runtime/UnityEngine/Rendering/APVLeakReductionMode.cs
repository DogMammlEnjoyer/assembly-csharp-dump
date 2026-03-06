using System;

namespace UnityEngine.Rendering
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\Lighting\\ProbeVolume\\ShaderVariablesProbeVolumes.cs")]
	public enum APVLeakReductionMode
	{
		None,
		Performance,
		Quality,
		[Obsolete("Performance")]
		ValidityBased = 1,
		[Obsolete("Quality")]
		ValidityAndNormalBased
	}
}
