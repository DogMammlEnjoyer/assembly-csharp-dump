using System;

namespace g3
{
	public class DistPoint3Circle3
	{
		public Vector3d Point
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

		public Circle3d Circle
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

		public DistPoint3Circle3(Vector3d PointIn, Circle3d circleIn)
		{
			this.point = PointIn;
			this.circle = circleIn;
		}

		public DistPoint3Circle3 Compute()
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
			Vector3d vector3d = this.point - this.circle.Center;
			Vector3d v = vector3d - this.circle.Normal.Dot(vector3d) * this.circle.Normal;
			double length = v.Length;
			if (length > 2.220446049250313E-16)
			{
				this.CircleClosest = this.circle.Center + this.circle.Radius * v / length;
				this.AllCirclePointsEquidistant = false;
			}
			else
			{
				this.CircleClosest = this.circle.Center + this.circle.Radius * this.circle.PlaneX;
				this.AllCirclePointsEquidistant = true;
			}
			Vector3d v2 = this.point - this.CircleClosest;
			double num = v2.Dot(v2);
			if (num < 0.0)
			{
				num = 0.0;
			}
			this.DistanceSquared = num;
			return num;
		}

		private Vector3d point;

		private Circle3d circle;

		public double DistanceSquared = -1.0;

		public Vector3d CircleClosest;

		public bool AllCirclePointsEquidistant;
	}
}
