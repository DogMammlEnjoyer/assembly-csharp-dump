using System;

namespace UnityEngine.XR.OpenXR.Features.Extensions.PerformanceSettings
{
	public enum PerformanceLevelHint
	{
		PowerSavings,
		SustainedLow = 25,
		SustainedHigh = 50,
		Boost = 75
	}
}
