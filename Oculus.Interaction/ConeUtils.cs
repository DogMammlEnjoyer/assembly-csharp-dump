using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ConeUtils
	{
		public static bool RayWithinCone(Ray ray, Vector3 position, float apertureDegrees)
		{
			float num = Mathf.Cos(apertureDegrees * 0.017453292f);
			Vector3 vector = position - ray.origin;
			float magnitude = vector.magnitude;
			if (Mathf.Abs(magnitude) < 0.001f)
			{
				return true;
			}
			vector /= magnitude;
			return Vector3.Dot(vector, ray.direction) >= num;
		}
	}
}
