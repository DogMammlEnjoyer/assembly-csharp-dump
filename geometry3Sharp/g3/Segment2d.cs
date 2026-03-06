using System;

namespace g3
{
	public struct Segment2d : IParametricCurve2d
	{
		public Segment2d(Vector2d p0, Vector2d p1)
		{
			this.Center = 0.5 * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5 * this.Direction.Normalize(2.220446049250313E-16);
		}

		public Segment2d(Vector2d center, Vector2d direction, double extent)
		{
			this.Center = center;
			this.Direction = direction;
			this.Extent = extent;
		}

		public Vector2d P0
		{
			get
			{
				return this.Center - this.Extent * this.Direction;
			}
			set
			{
				this.update_from_endpoints(value, this.P1);
			}
		}

		public Vector2d P1
		{
			get
			{
				return this.Center + this.Extent * this.Direction;
			}
			set
			{
				this.update_from_endpoints(this.P0, value);
			}
		}

		public double Length
		{
			get
			{
				return 2.0 * this.Extent;
			}
		}

		public Vector2d Endpoint(int i)
		{
			if (i != 0)
			{
				return this.Center + this.Extent * this.Direction;
			}
			return this.Center - this.Extent * this.Direction;
		}

		public Vector2d PointAt(double d)
		{
			return this.Center + d * this.Direction;
		}

		public Vector2d PointBetween(double t)
		{
			return this.Center + (2.0 * t - 1.0) * this.Extent * this.Direction;
		}

		public double DistanceSquared(Vector2d p)
		{
			double num = (p - this.Center).Dot(this.Direction);
			if (num >= this.Extent)
			{
				return this.P1.DistanceSquared(p);
			}
			if (num <= -this.Extent)
			{
				return this.P0.DistanceSquared(p);
			}
			return (this.Center + num * this.Direction).DistanceSquared(p);
		}

		public double DistanceSquared(Vector2d p, out double t)
		{
			t = (p - this.Center).Dot(this.Direction);
			if (t >= this.Extent)
			{
				t = this.Extent;
				return this.P1.DistanceSquared(p);
			}
			if (t <= -this.Extent)
			{
				t = -this.Extent;
				return this.P0.DistanceSquared(p);
			}
			return (this.Center + t * this.Direction).DistanceSquared(p);
		}

		public Vector2d NearestPoint(Vector2d p)
		{
			double num = (p - this.Center).Dot(this.Direction);
			if (num >= this.Extent)
			{
				return this.P1;
			}
			if (num <= -this.Extent)
			{
				return this.P0;
			}
			return this.Center + num * this.Direction;
		}

		public double Project(Vector2d p)
		{
			return (p - this.Center).Dot(this.Direction);
		}

		private void update_from_endpoints(Vector2d p0, Vector2d p1)
		{
			this.Center = 0.5 * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5 * this.Direction.Normalize(2.220446049250313E-16);
		}

		public int WhichSide(Vector2d test, double tol = 0.0)
		{
			Vector2d vector2d = this.Center + this.Extent * this.Direction;
			Vector2d vector2d2 = this.Center - this.Extent * this.Direction;
			double num = test.x - vector2d.x;
			double num2 = test.y - vector2d.y;
			double num3 = vector2d2.x - vector2d.x;
			double num4 = vector2d2.y - vector2d.y;
			double num5 = num * num4 - num3 * num2;
			if (num5 > tol)
			{
				return 1;
			}
			if (num5 >= -tol)
			{
				return 0;
			}
			return -1;
		}

		public bool IsClosed
		{
			get
			{
				return false;
			}
		}

		public double ParamLength
		{
			get
			{
				return 1.0;
			}
		}

		public Vector2d SampleT(double t)
		{
			return this.Center + (2.0 * t - 1.0) * this.Extent * this.Direction;
		}

		public Vector2d TangentT(double t)
		{
			return this.Direction;
		}

		public bool HasArcLength
		{
			get
			{
				return true;
			}
		}

		public double ArcLength
		{
			get
			{
				return 2.0 * this.Extent;
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			return this.P0 + a * this.Direction;
		}

		public void Reverse()
		{
			this.update_from_endpoints(this.P1, this.P0);
		}

		public IParametricCurve2d Clone()
		{
			return new Segment2d(this.Center, this.Direction, this.Extent);
		}

		public bool IsTransformable
		{
			get
			{
				return true;
			}
		}

		public void Transform(ITransform2 xform)
		{
			this.Center = xform.TransformP(this.Center);
			this.Direction = xform.TransformN(this.Direction);
			this.Extent = xform.TransformScalar(this.Extent);
		}

		public static double FastDistanceSquared(ref Vector2d a, ref Vector2d b, ref Vector2d pt)
		{
			double num = b.x - a.x;
			double num2 = b.y - a.y;
			double num3 = num * num + num2 * num2;
			double num4 = pt.x - a.x;
			double num5 = pt.y - a.y;
			if (num3 < 1E-13)
			{
				return num4 * num4 + num5 * num5;
			}
			double num6 = num4 * num + num5 * num2;
			if (num6 <= 0.0)
			{
				return num4 * num4 + num5 * num5;
			}
			if (num6 >= num3)
			{
				num4 = pt.x - b.x;
				num5 = pt.y - b.y;
				return num4 * num4 + num5 * num5;
			}
			num4 = pt.x - (a.x + num6 * num / num3);
			num5 = pt.y - (a.y + num6 * num2 / num3);
			return num4 * num4 + num5 * num5;
		}

		public static int WhichSide(ref Vector2d a, ref Vector2d b, ref Vector2d test, double tol = 0.0)
		{
			double num = test.x - a.x;
			double num2 = test.y - a.y;
			double num3 = b.x - a.x;
			double num4 = b.y - a.y;
			double num5 = num * num4 - num3 * num2;
			if (num5 > tol)
			{
				return 1;
			}
			if (num5 >= -tol)
			{
				return 0;
			}
			return -1;
		}

		public bool Intersects(ref Segment2d seg2, double dotThresh = 5E-324, double intervalThresh = 0.0)
		{
			Vector2d v = seg2.Center - this.Center;
			double num = this.Direction.DotPerp(seg2.Direction);
			if (Math.Abs(num) > dotThresh)
			{
				double num2 = 1.0 / num;
				double num3 = v.DotPerp(this.Direction);
				double value = v.DotPerp(seg2.Direction) * num2;
				double value2 = num3 * num2;
				return Math.Abs(value) <= this.Extent + intervalThresh && Math.Abs(value2) <= seg2.Extent + intervalThresh;
			}
			v.Normalize(2.220446049250313E-16);
			if (Math.Abs(v.DotPerp(seg2.Direction)) <= dotThresh)
			{
				v = seg2.Center - this.Center;
				double num4 = this.Direction.Dot(v);
				double x = num4 - seg2.Extent;
				double y = num4 + seg2.Extent;
				Interval1d interval1d = new Interval1d(-this.Extent, this.Extent);
				return interval1d.Overlaps(new Interval1d(x, y));
			}
			return false;
		}

		public bool Intersects(Segment2d seg2, double dotThresh = 5E-324, double intervalThresh = 0.0)
		{
			return this.Intersects(ref seg2, dotThresh, intervalThresh);
		}

		public bool BiEquals(Segment2d seg)
		{
			return seg.Center == this.Center && seg.Extent == this.Extent;
		}

		public Vector2d Center;

		public Vector2d Direction;

		public double Extent;
	}
}
