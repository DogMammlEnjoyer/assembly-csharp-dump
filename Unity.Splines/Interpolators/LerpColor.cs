using System;

namespace UnityEngine.Splines.Interpolators
{
	public struct LerpColor : IInterpolator<Color>
	{
		public Color Interpolate(Color a, Color b, float t)
		{
			return Color.Lerp(a, b, t);
		}
	}
}
