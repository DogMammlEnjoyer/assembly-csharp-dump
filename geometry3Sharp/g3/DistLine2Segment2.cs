using System;

namespace g3
{
	public class DistLine2Segment2
	{
		public Line2d Line
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
				this.DistanceSquared = -1.0;
			}
		}

		public Segment2d Segment
		{
			get
			{
				return this.segment;
			}
			set
			{
				this.segment = value;
				this.DistanceSquared = -1.0;
			}
		}

		public DistLine2Segment2(Line2d LineIn, Segment2d SegmentIn)
		{
			this.segment = SegmentIn;
			this.line = LineIn;
		}

		public static double MinDistance(Line2d line, Segment2d segment)
		{
			return new DistLine2Segment2(line, segment).Get();
		}

		public DistLine2Segment2 Compute()
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
			Vector2d vector2d = this.line.Origin - this.segment.Center;
			double num = -this.line.Direction.Dot(this.segment.Direction);
			double num2 = vector2d.Dot(this.line.Direction);
			double lengthSquared = vector2d.LengthSquared;
			double num3 = Math.Abs(1.0 - num * num);
			double num5;
			double num8;
			double num9;
			if (num3 >= 1E-08)
			{
				double num4 = -vector2d.Dot(this.segment.Direction);
				num5 = num * num2 - num4;
				double num6 = this.segment.Extent * num3;
				if (num5 >= -num6)
				{
					if (num5 <= num6)
					{
						double num7 = 1.0 / num3;
						num8 = (num * num4 - num2) * num7;
						num5 *= num7;
						num9 = 0.0;
					}
					else
					{
						num5 = this.segment.Extent;
						num8 = -(num * num5 + num2);
						num9 = -num8 * num8 + num5 * (num5 + 2.0 * num4) + lengthSquared;
					}
				}
				else
				{
					num5 = -this.segment.Extent;
					num8 = -(num * num5 + num2);
					num9 = -num8 * num8 + num5 * (num5 + 2.0 * num4) + lengthSquared;
				}
			}
			else
			{
				num5 = 0.0;
				num8 = -num2;
				num9 = num2 * num8 + lengthSquared;
			}
			this.LineParameter = num8;
			this.LineClosest = this.line.Origin + num8 * this.line.Direction;
			this.SegmentParameter = num5;
			this.SegmentClosest = this.segment.Center + num5 * this.segment.Direction;
			if (num9 < 0.0)
			{
				num9 = 0.0;
			}
			this.DistanceSquared = num9;
			return num9;
		}

		private Line2d line;

		private Segment2d segment;

		public double DistanceSquared = -1.0;

		public Vector2d LineClosest;

		public double LineParameter;

		public Vector2d SegmentClosest;

		public double SegmentParameter;
	}
}
