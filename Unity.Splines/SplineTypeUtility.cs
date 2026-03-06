using System;

namespace UnityEngine.Splines
{
	internal static class SplineTypeUtility
	{
		internal static TangentMode GetTangentMode(this SplineType splineType)
		{
			if (splineType == SplineType.Bezier)
			{
				return TangentMode.Mirrored;
			}
			if (splineType != SplineType.Linear)
			{
				return TangentMode.AutoSmooth;
			}
			return TangentMode.Linear;
		}
	}
}
