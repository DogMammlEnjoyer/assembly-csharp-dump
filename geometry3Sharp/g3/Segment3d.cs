using System;

namespace g3
{
	public struct Segment3d : IParametricCurve3d
	{
		public Segment3d(Vector3d p0, Vector3d p1)
		{
			this.Center = 0.5 * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5 * this.Direction.Normalize(2.220446049250313E-16);
		}

		public Segment3d(Vector3d center, Vector3d direction, double extent)
		{
			this.Center = center;
			this.Direction = direction;
			this.Extent = extent;
		}

		public void SetEndpoints(Vector3d p0, Vector3d p1)
		{
			this.update_from_endpoints(p0, p1);
		}

		public Vector3d P0
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

		public Vector3d P1
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

		public Vector3d PointAt(double d)
		{
			return this.Center + d * this.Direction;
		}

		public Vector3d PointBetween(double t)
		{
			return this.Center + (2.0 * t - 1.0) * this.Extent * this.Direction;
		}

		public double DistanceSquared(Vector3d p)
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
			return (this.Center + num * this.Direction - p).LengthSquared;
		}

		public double DistanceSquared(Vector3d p, out double t)
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
			return (this.Center + t * this.Direction - p).LengthSquared;
		}

		public Vector3d NearestPoint(Vector3d p)
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

		public double Project(Vector3d p)
		{
			return (p - this.Center).Dot(this.Direction);
		}

		private void update_from_endpoints(Vector3d p0, Vector3d p1)
		{
			this.Center = 0.5 * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5 * this.Direction.Normalize(2.220446049250313E-16);
		}

		public static implicit operator Segment3d(Segment3f v)
		{
			return new Segment3d(v.Center, v.Direction, (double)v.Extent);
		}

		public static explicit operator Segment3f(Segment3d v)
		{
			return new Segment3f((Vector3f)v.Center, (Vector3f)v.Direction, (float)v.Extent);
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

		public Vector3d SampleT(double t)
		{
			return this.Center + (2.0 * t - 1.0) * this.Extent * this.Direction;
		}

		public Vector3d TangentT(double t)
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

		public Vector3d SampleArcLength(double a)
		{
			return this.P0 + a * this.Direction;
		}

		public void Reverse()
		{
			this.update_from_endpoints(this.P1, this.P0);
		}

		public IParametricCurve3d Clone()
		{
			return new Segment3d(this.Center, this.Direction, this.Extent);
		}

		public Vector3d Center;

		public Vector3d Direction;

		public double Extent;
	}
}
