using System;
using UnityEngine;

namespace g3
{
	public struct Ray3d
	{
		public Ray3d(Vector3d origin, Vector3d direction, bool bIsNormalized = false)
		{
			this.Origin = origin;
			this.Direction = direction;
			if (!bIsNormalized && !this.Direction.IsNormalized)
			{
				this.Direction.Normalize(2.220446049250313E-16);
			}
		}

		public Ray3d(Vector3f origin, Vector3f direction)
		{
			this.Origin = origin;
			this.Direction = direction;
			this.Direction.Normalize(2.220446049250313E-16);
		}

		public Vector3d PointAt(double d)
		{
			return this.Origin + d * this.Direction;
		}

		public double Project(Vector3d p)
		{
			return (p - this.Origin).Dot(this.Direction);
		}

		public double DistanceSquared(Vector3d p)
		{
			double num = (p - this.Origin).Dot(this.Direction);
			if (num < 0.0)
			{
				return this.Origin.DistanceSquared(p);
			}
			return (this.Origin + num * this.Direction - p).LengthSquared;
		}

		public Vector3d ClosestPoint(Vector3d p)
		{
			double num = (p - this.Origin).Dot(this.Direction);
			if (num < 0.0)
			{
				return this.Origin;
			}
			return this.Origin + num * this.Direction;
		}

		public static implicit operator Ray3d(Ray3f v)
		{
			return new Ray3d(v.Origin, v.Direction.Normalized, false);
		}

		public static explicit operator Ray3f(Ray3d v)
		{
			return new Ray3f((Vector3f)v.Origin, ((Vector3f)v.Direction).Normalized, false);
		}

		public static implicit operator Ray3d(Ray r)
		{
			return new Ray3d(r.origin, r.direction.Normalized, false);
		}

		public static explicit operator Ray(Ray3d r)
		{
			return new Ray((Vector3)r.Origin, ((Vector3)r.Direction).normalized);
		}

		public Vector3d Origin;

		public Vector3d Direction;
	}
}
