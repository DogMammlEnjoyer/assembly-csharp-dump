using System;

namespace g3
{
	public struct Line3d
	{
		public Line3d(Vector3d origin, Vector3d direction)
		{
			this.Origin = origin;
			this.Direction = direction;
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
			double f = (p - this.Origin).Dot(this.Direction);
			return (this.Origin + f * this.Direction - p).LengthSquared;
		}

		public Vector3d ClosestPoint(Vector3d p)
		{
			double f = (p - this.Origin).Dot(this.Direction);
			return this.Origin + f * this.Direction;
		}

		public static implicit operator Line3d(Line3f v)
		{
			return new Line3d(v.Origin, v.Direction);
		}

		public static explicit operator Line3f(Line3d v)
		{
			return new Line3f((Vector3f)v.Origin, (Vector3f)v.Direction);
		}

		public Vector3d Origin;

		public Vector3d Direction;
	}
}
