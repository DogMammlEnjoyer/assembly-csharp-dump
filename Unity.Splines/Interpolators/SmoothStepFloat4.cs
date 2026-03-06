using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.Interpolators
{
	public struct SmoothStepFloat4 : IInterpolator<float4>
	{
		public float4 Interpolate(float4 a, float4 b, float t)
		{
			return math.smoothstep(a, b, t);
		}
	}
}
