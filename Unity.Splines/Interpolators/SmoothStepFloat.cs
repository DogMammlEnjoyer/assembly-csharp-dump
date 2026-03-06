using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.Interpolators
{
	public struct SmoothStepFloat : IInterpolator<float>
	{
		public float Interpolate(float a, float b, float t)
		{
			return math.smoothstep(a, b, t);
		}
	}
}
