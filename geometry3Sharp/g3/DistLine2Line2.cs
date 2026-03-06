using System;

namespace g3
{
	public class DistLine2Line2
	{
		public Line2d Line
		{
			get
			{
				return this.line1;
			}
			set
			{
				this.line1 = value;
				this.DistanceSquared = -1.0;
			}
		}

		public Line2d Line2
		{
			get
			{
				return this.line2;
			}
			set
			{
				this.line2 = value;
				this.DistanceSquared = -1.0;
			}
		}

		public DistLine2Line2(Line2d Line1, Line2d Line2)
		{
			this.line2 = Line2;
			this.line1 = Line1;
		}

		public static double MinDistance(Line2d line1, Line2d line2)
		{
			return new DistLine2Line2(line1, line2).Get();
		}

		public DistLine2Line2 Compute()
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
			Vector2d vector2d = this.line1.Origin - this.line2.Origin;
			double num = -this.line1.Direction.Dot(this.line2.Direction);
			double num2 = vector2d.Dot(this.line1.Direction);
			double lengthSquared = vector2d.LengthSquared;
			double num3 = Math.Abs(1.0 - num * num);
			double num6;
			double num7;
			double num8;
			if (num3 >= 1E-08)
			{
				double num4 = -vector2d.Dot(this.line2.Direction);
				double num5 = 1.0 / num3;
				num6 = (num * num4 - num2) * num5;
				num7 = (num * num2 - num4) * num5;
				num8 = 0.0;
			}
			else
			{
				num6 = -num2;
				num7 = 0.0;
				num8 = num2 * num6 + lengthSquared;
				if (num8 < 0.0)
				{
					num8 = 0.0;
				}
			}
			this.Line1Parameter = num6;
			this.Line1Closest = this.line1.Origin + num6 * this.line1.Direction;
			this.Line2Parameter = num7;
			this.Line2Closest = this.line2.Origin + num7 * this.line2.Direction;
			this.DistanceSquared = num8;
			return num8;
		}

		private Line2d line1;

		private Line2d line2;

		public double DistanceSquared = -1.0;

		public Vector2d Line1Closest;

		public Vector2d Line2Closest;

		public double Line1Parameter;

		public double Line2Parameter;
	}
}
