using System;

namespace g3
{
	public class DistSegment2Segment2
	{
		public Segment2d Segment1
		{
			get
			{
				return this.segment0;
			}
			set
			{
				this.segment0 = value;
				this.DistanceSquared = -1.0;
			}
		}

		public Segment2d Segment2
		{
			get
			{
				return this.segment1;
			}
			set
			{
				this.segment1 = value;
				this.DistanceSquared = -1.0;
			}
		}

		public DistSegment2Segment2(Segment2d Segment1, Segment2d Segment2)
		{
			this.segment1 = Segment2;
			this.segment0 = Segment1;
		}

		public static double MinDistance(Segment2d Segment1, Segment2d Segment2)
		{
			return new DistSegment2Segment2(Segment1, Segment2).Get();
		}

		public DistSegment2Segment2 Compute()
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
			Vector2d vector2d = this.segment0.Center - this.segment1.Center;
			double num = -this.segment0.Direction.Dot(this.segment1.Direction);
			double num2 = vector2d.Dot(this.segment0.Direction);
			double num3 = -vector2d.Dot(this.segment1.Direction);
			double lengthSquared = vector2d.LengthSquared;
			double num4 = Math.Abs(1.0 - num * num);
			double num5;
			double num6;
			double num10;
			if (num4 >= 1E-08)
			{
				num5 = num * num3 - num2;
				num6 = num * num2 - num3;
				double num7 = this.segment0.Extent * num4;
				double num8 = this.segment1.Extent * num4;
				if (num5 >= -num7)
				{
					if (num5 <= num7)
					{
						if (num6 >= -num8)
						{
							if (num6 <= num8)
							{
								double num9 = 1.0 / num4;
								num5 *= num9;
								num6 *= num9;
								num10 = 0.0;
							}
							else
							{
								num6 = this.segment1.Extent;
								double num11 = -(num * num6 + num2);
								if (num11 < -this.segment0.Extent)
								{
									num5 = -this.segment0.Extent;
									num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
								}
								else if (num11 <= this.segment0.Extent)
								{
									num5 = num11;
									num10 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
								}
								else
								{
									num5 = this.segment0.Extent;
									num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
								}
							}
						}
						else
						{
							num6 = -this.segment1.Extent;
							double num11 = -(num * num6 + num2);
							if (num11 < -this.segment0.Extent)
							{
								num5 = -this.segment0.Extent;
								num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
							else if (num11 <= this.segment0.Extent)
							{
								num5 = num11;
								num10 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
							else
							{
								num5 = this.segment0.Extent;
								num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
						}
					}
					else if (num6 >= -num8)
					{
						if (num6 <= num8)
						{
							num5 = this.segment0.Extent;
							double num12 = -(num * num5 + num3);
							if (num12 < -this.segment1.Extent)
							{
								num6 = -this.segment1.Extent;
								num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
							else if (num12 <= this.segment1.Extent)
							{
								num6 = num12;
								num10 = -num6 * num6 + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
							else
							{
								num6 = this.segment1.Extent;
								num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
						}
						else
						{
							num6 = this.segment1.Extent;
							double num11 = -(num * num6 + num2);
							if (num11 < -this.segment0.Extent)
							{
								num5 = -this.segment0.Extent;
								num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
							else if (num11 <= this.segment0.Extent)
							{
								num5 = num11;
								num10 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
							else
							{
								num5 = this.segment0.Extent;
								double num12 = -(num * num5 + num3);
								if (num12 < -this.segment1.Extent)
								{
									num6 = -this.segment1.Extent;
									num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
								}
								else if (num12 <= this.segment1.Extent)
								{
									num6 = num12;
									num10 = -num6 * num6 + num5 * (num5 + 2.0 * num2) + lengthSquared;
								}
								else
								{
									num6 = this.segment1.Extent;
									num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
								}
							}
						}
					}
					else
					{
						num6 = -this.segment1.Extent;
						double num11 = -(num * num6 + num2);
						if (num11 < -this.segment0.Extent)
						{
							num5 = -this.segment0.Extent;
							num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
						else if (num11 <= this.segment0.Extent)
						{
							num5 = num11;
							num10 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
						else
						{
							num5 = this.segment0.Extent;
							double num12 = -(num * num5 + num3);
							if (num12 > this.segment1.Extent)
							{
								num6 = this.segment1.Extent;
								num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
							else if (num12 >= -this.segment1.Extent)
							{
								num6 = num12;
								num10 = -num6 * num6 + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
							else
							{
								num6 = -this.segment1.Extent;
								num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
						}
					}
				}
				else if (num6 >= -num8)
				{
					if (num6 <= num8)
					{
						num5 = -this.segment0.Extent;
						double num12 = -(num * num5 + num3);
						if (num12 < -this.segment1.Extent)
						{
							num6 = -this.segment1.Extent;
							num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
						}
						else if (num12 <= this.segment1.Extent)
						{
							num6 = num12;
							num10 = -num6 * num6 + num5 * (num5 + 2.0 * num2) + lengthSquared;
						}
						else
						{
							num6 = this.segment1.Extent;
							num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
						}
					}
					else
					{
						num6 = this.segment1.Extent;
						double num11 = -(num * num6 + num2);
						if (num11 > this.segment0.Extent)
						{
							num5 = this.segment0.Extent;
							num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
						else if (num11 >= -this.segment0.Extent)
						{
							num5 = num11;
							num10 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
						else
						{
							num5 = -this.segment0.Extent;
							double num12 = -(num * num5 + num3);
							if (num12 < -this.segment1.Extent)
							{
								num6 = -this.segment1.Extent;
								num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
							else if (num12 <= this.segment1.Extent)
							{
								num6 = num12;
								num10 = -num6 * num6 + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
							else
							{
								num6 = this.segment1.Extent;
								num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
							}
						}
					}
				}
				else
				{
					num6 = -this.segment1.Extent;
					double num11 = -(num * num6 + num2);
					if (num11 > this.segment0.Extent)
					{
						num5 = this.segment0.Extent;
						num10 = num5 * (num5 - 2.0 * num11) + num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
					else if (num11 >= -this.segment0.Extent)
					{
						num5 = num11;
						num10 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
					else
					{
						num5 = -this.segment0.Extent;
						double num12 = -(num * num5 + num3);
						if (num12 < -this.segment1.Extent)
						{
							num6 = -this.segment1.Extent;
							num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
						}
						else if (num12 <= this.segment1.Extent)
						{
							num6 = num12;
							num10 = -num6 * num6 + num5 * (num5 + 2.0 * num2) + lengthSquared;
						}
						else
						{
							num6 = this.segment1.Extent;
							num10 = num6 * (num6 - 2.0 * num12) + num5 * (num5 + 2.0 * num2) + lengthSquared;
						}
					}
				}
			}
			else
			{
				double num13 = this.segment0.Extent + this.segment1.Extent;
				double num14 = (num > 0.0) ? -1.0 : 1.0;
				double num15 = 0.5 * (num2 - num14 * num3);
				double num16 = -num15;
				if (num16 < -num13)
				{
					num16 = -num13;
				}
				else if (num16 > num13)
				{
					num16 = num13;
				}
				num6 = -num14 * num16 * this.segment1.Extent / num13;
				num5 = num16 + num14 * num6;
				num10 = num16 * (num16 + 2.0 * num15) + lengthSquared;
			}
			if (num10 < 0.0)
			{
				num10 = 0.0;
			}
			this.Segment1Parameter = num5;
			this.Segment1Closest = this.segment0.Center + num5 * this.segment0.Direction;
			this.Segment2Parameter = num6;
			this.Segment2Closest = this.segment1.Center + num6 * this.segment1.Direction;
			this.DistanceSquared = num10;
			return num10;
		}

		private Segment2d segment0;

		private Segment2d segment1;

		public double DistanceSquared = -1.0;

		public Vector2d Segment1Closest;

		public Vector2d Segment2Closest;

		public double Segment1Parameter;

		public double Segment2Parameter;
	}
}
