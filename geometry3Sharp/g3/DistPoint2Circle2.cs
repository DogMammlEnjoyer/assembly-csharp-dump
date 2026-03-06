using System;

namespace g3
{
	public class DistPoint2Circle2
	{
		public Vector2d Point
		{
			get
			{
				return this.point;
			}
			set
			{
				this.point = value;
				this.DistanceSquared = -1.0;
			}
		}

		public Circle2d Circle
		{
			get
			{
				return this.circle;
			}
			set
			{
				this.circle = value;
				this.DistanceSquared = -1.0;
			}
		}

		public DistPoint2Circle2(Vector2d PointIn, Circle2d circleIn)
		{
			this.point = PointIn;
			this.circle = circleIn;
		}

		public DistPoint2Circle2 Compute()
		{
			this.GetSquared();
			return this;
		}

		public double Get()
		{
			return Math.Sqrt(this.GetSquared());
		}

		public double GetSquared()
		{
			if (this.DistanceSquared >= 0.0)
			{
				return this.DistanceSquared;
			}
			Vector2d a = this.point - this.circle.Center;
			double length = a.Length;
			if (length > 2.220446049250313E-16)
			{
				this.CircleClosest = this.circle.Center + this.circle.Radius * a / length;
				this.AllCirclePointsEquidistant = false;
			}
			else
			{
				this.CircleClosest = this.circle.Center + this.circle.Radius;
				this.AllCirclePointsEquidistant = true;
			}
			Vector2d v = this.point - this.CircleClosest;
			double num = v.Dot(v);
			if (num < 0.0)
			{
				num = 0.0;
			}
			this.DistanceSquared = num;
			return num;
		}

		private Vector2d point;

		private Circle2d circle;

		public double DistanceSquared = -1.0;

		public Vector2d CircleClosest;

		public bool AllCirclePointsEquidistant;
	}
}
