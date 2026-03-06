using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.Interpolators
{
	public struct SmoothStepFloat2 : IInterpolator<float2>
	{
		public float2 Interpolate(float2 a, float2 b, float t)
		{
			return math.smoothstep(a, b, t);
		}
	}
}
