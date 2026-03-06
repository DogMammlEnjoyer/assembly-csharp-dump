using System;
using Unity.Mathematics;

namespace UnityEngine.Splines.Interpolators
{
	public struct LerpFloat : IInterpolator<float>
	{
		public float Interpolate(float a, float b, float t)
		{
			return math.lerp(a, b, t);
		}
	}
}
