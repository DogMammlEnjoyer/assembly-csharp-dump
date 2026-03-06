using System;

namespace g3
{
	public struct Line2d
	{
		public Line2d(Vector2d origin, Vector2d direction)
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		public Line2d(ref Vector2d origin, ref Vector2d direction)
		{
			this.Origin = origin;
			this.Direction = direction;
		}

		public static Line2d FromPoints(Vector2d p0, Vector2d p1)
		{
			return new Line2d(p0, (p1 - p0).Normalized);
		}

		public static Line2d FromPoints(ref Vector2d p0, ref Vector2d p1)
		{
			return new Line2d(p0, (p1 - p0).Normalized);
		}

		public Vector2d PointAt(double d)
		{
			return this.Origin + d * this.Direction;
		}

		public double Project(Vector2d p)
		{
			return (p - this.Origin).Dot(this.Direction);
		}

		public double DistanceSquared(Vector2d p)
		{
			double f = (p - this.Origin).Dot(this.Direction);
			return (this.Origin + f * this.Direction - p).LengthSquared;
		}

		public int WhichSide(Vector2d test, double tol = 0.0)
		{
			double num = test.x - this.Origin.x;
			double num2 = test.y - this.Origin.y;
			double x = this.Direction.x;
			double y = this.Direction.y;
			double num3 = num * y - x * num2;
			if (num3 > tol)
			{
				return 1;
			}
			if (num3 >= -tol)
			{
				return 0;
			}
			return -1;
		}

		public int WhichSide(ref Vector2d test, double tol = 0.0)
		{
			double num = test.x - this.Origin.x;
			double num2 = test.y - this.Origin.y;
			double x = this.Direction.x;
			double y = this.Direction.y;
			double num3 = num * y - x * num2;
			if (num3 > tol)
			{
				return 1;
			}
			if (num3 >= -tol)
			{
				return 0;
			}
			return -1;
		}

		public Vector2d IntersectionPoint(ref Line2d other, double dotThresh = 1E-08)
		{
			Vector2d vector2d = other.Origin - this.Origin;
			double num = this.Direction.DotPerp(other.Direction);
			if (Math.Abs(num) > dotThresh)
			{
				double num2 = 1.0 / num;
				double f = vector2d.DotPerp(other.Direction) * num2;
				return this.Origin + f * this.Direction;
			}
			return Vector2d.MaxValue;
		}

		public static implicit operator Line2d(Line2f v)
		{
			return new Line2d(v.Origin, v.Direction);
		}

		public static explicit operator Line2f(Line2d v)
		{
			return new Line2f((Vector2f)v.Origin, (Vector2f)v.Direction);
		}

		public Vector2d Origin;

		public Vector2d Direction;
	}
}
