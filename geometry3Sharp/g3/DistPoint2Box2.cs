using System;

namespace g3
{
	public class DistPoint2Box2
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

		public Box2d Box
		{
			get
			{
				return this.box;
			}
			set
			{
				this.box = value;
				this.DistanceSquared = -1.0;
			}
		}

		public DistPoint2Box2(Vector2d PointIn, Box2d boxIn)
		{
			this.point = PointIn;
			this.box = boxIn;
		}

		public DistPoint2Box2 Compute()
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
			Vector2d vector2d = this.point - this.box.Center;
			double num = 0.0;
			Vector2d zero = Vector2d.Zero;
			for (int i = 0; i < 2; i++)
			{
				zero[i] = vector2d.Dot(this.box.Axis(i));
				if (zero[i] < -this.box.Extent[i])
				{
					double num2 = zero[i] + this.box.Extent[i];
					num += num2 * num2;
					zero[i] = -this.box.Extent[i];
				}
				else if (zero[i] > this.box.Extent[i])
				{
					double num2 = zero[i] - this.box.Extent[i];
					num += num2 * num2;
					zero[i] = this.box.Extent[i];
				}
			}
			this.BoxClosest = this.box.Center;
			for (int i = 0; i < 2; i++)
			{
				this.BoxClosest += zero[i] * this.box.Axis(i);
			}
			this.DistanceSquared = num;
			return num;
		}

		private Vector2d point;

		private Box2d box;

		public double DistanceSquared = -1.0;

		public Vector2d BoxClosest;
	}
}
