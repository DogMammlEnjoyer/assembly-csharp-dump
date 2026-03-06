using System;

namespace g3
{
	public struct Segment3f
	{
		public Segment3f(Vector3f p0, Vector3f p1)
		{
			this.Center = 0.5f * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5f * this.Direction.Normalize(1.1920929E-07f);
		}

		public Segment3f(Vector3f center, Vector3f direction, float extent)
		{
			this.Center = center;
			this.Direction = direction;
			this.Extent = extent;
		}

		public void SetEndpoints(Vector3f p0, Vector3f p1)
		{
			this.update_from_endpoints(p0, p1);
		}

		public Vector3f P0
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

		public Vector3f P1
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

		public float Length
		{
			get
			{
				return 2f * this.Extent;
			}
		}

		public Vector3f PointAt(float d)
		{
			return this.Center + d * this.Direction;
		}

		public Vector3f PointBetween(float t)
		{
			return this.Center + (2f * t - 1f) * this.Extent * this.Direction;
		}

		public float DistanceSquared(Vector3f p)
		{
			float num = (p - this.Center).Dot(this.Direction);
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

		public Vector3f NearestPoint(Vector3f p)
		{
			float num = (p - this.Center).Dot(this.Direction);
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

		public float Project(Vector3f p)
		{
			return (p - this.Center).Dot(this.Direction);
		}

		private void update_from_endpoints(Vector3f p0, Vector3f p1)
		{
			this.Center = 0.5f * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5f * this.Direction.Normalize(1.1920929E-07f);
		}

		public Vector3f Center;

		public Vector3f Direction;

		public float Extent;
	}
}
