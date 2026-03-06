using System;
using UnityEngine;

namespace g3
{
	public struct Ray3f
	{
		public Ray3f(Vector3f origin, Vector3f direction, bool bIsNormalized = false)
		{
			this.Origin = origin;
			this.Direction = direction;
			if (!bIsNormalized && !this.Direction.IsNormalized)
			{
				this.Direction.Normalize(1.1920929E-07f);
			}
		}

		public Vector3f PointAt(float d)
		{
			return this.Origin + d * this.Direction;
		}

		public float Project(Vector3f p)
		{
			return (p - this.Origin).Dot(this.Direction);
		}

		public float DistanceSquared(Vector3f p)
		{
			float f = (p - this.Origin).Dot(this.Direction);
			return (this.Origin + f * this.Direction - p).LengthSquared;
		}

		public static implicit operator Ray3f(Ray r)
		{
			return new Ray3f(r.origin, r.direction, false);
		}

		public static implicit operator Ray(Ray3f r)
		{
			return new Ray(r.Origin, r.Direction);
		}

		public Vector3f Origin;

		public Vector3f Direction;
	}
}
