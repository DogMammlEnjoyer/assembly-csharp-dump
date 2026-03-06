using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.Interpolators
{
	public struct SmoothStepFloat3 : IInterpolator<float3>
	{
		public float3 Interpolate(float3 a, float3 b, float t)
		{
			return math.smoothstep(a, b, t);
		}
	}
}
