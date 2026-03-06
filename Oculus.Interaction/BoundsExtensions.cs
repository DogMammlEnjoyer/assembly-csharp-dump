using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class BoundsExtensions
	{
		public static bool Clip(this Bounds bounds, in Bounds clipper, out Bounds result)
		{
			result = default(Bounds);
			Vector3 min = bounds.min;
			Bounds bounds2 = clipper;
			Vector3 vector = Vector3.Max(min, bounds2.min);
			Vector3 max = bounds.max;
			bounds2 = clipper;
			Vector3 vector2 = Vector3.Min(max, bounds2.max);
			if (vector.x > vector2.x || vector.y > vector2.y || vector.z > vector2.z)
			{
				return false;
			}
			result.SetMinMax(vector, vector2);
			return true;
		}
	}
}
