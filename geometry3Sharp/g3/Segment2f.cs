using System;

namespace g3
{
	public struct Segment2f
	{
		public Segment2f(Vector2f p0, Vector2f p1)
		{
			this.Center = 0.5f * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5f * this.Direction.Normalize(1.1920929E-07f);
		}

		public Segment2f(Vector2f center, Vector2f direction, float extent)
		{
			this.Center = center;
			this.Direction = direction;
			this.Extent = extent;
		}

		public Vector2f P0
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

		public Vector2f P1
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

		public Vector2f PointAt(float d)
		{
			return this.Center + d * this.Direction;
		}

		public Vector2f PointBetween(float t)
		{
			return this.Center + (2f * t - 1f) * this.Extent * this.Direction;
		}

		public float DistanceSquared(Vector2f p)
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

		public Vector2f NearestPoint(Vector2f p)
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

		public float Project(Vector2f p)
		{
			return (p - this.Center).Dot(this.Direction);
		}

		private void update_from_endpoints(Vector2f p0, Vector2f p1)
		{
			this.Center = 0.5f * (p0 + p1);
			this.Direction = p1 - p0;
			this.Extent = 0.5f * this.Direction.Normalize(1.1920929E-07f);
		}

		public static float FastDistanceSquared(ref Vector2f a, ref Vector2f b, ref Vector2f pt)
		{
			float num = b.x - a.x;
			float num2 = b.y - a.y;
			float num3 = num * num + num2 * num2;
			float num4 = pt.x - a.x;
			float num5 = pt.y - a.y;
			if ((double)num3 < 1E-07)
			{
				return num4 * num4 + num5 * num5;
			}
			float num6 = num4 * num + num5 * num2;
			if (num6 <= 0f)
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

		public bool BiEquals(Segment2d seg)
		{
			return seg.Center == this.Center && seg.Extent == (double)this.Extent;
		}

		public Vector2f Center;

		public Vector2f Direction;

		public float Extent;
	}
}
