using System;

namespace g3
{
	public class DistRay3Segment3
	{
		public Ray3d Ray
		{
			get
			{
				return this.ray;
			}
			set
			{
				this.ray = value;
				this.DistanceSquared = -1.0;
			}
		}

		public Segment3d Segment
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

		public DistRay3Segment3(Ray3d rayIn, Segment3d segmentIn)
		{
			this.ray = rayIn;
			this.segment = segmentIn;
		}

		public static double MinDistance(Ray3d r, Segment3d s)
		{
			double num;
			double num2;
			return Math.Sqrt(DistRay3Segment3.SquaredDistance(ref r, ref s, out num, out num2));
		}

		public static double MinDistanceSegmentParam(Ray3d r, Segment3d s)
		{
			double num;
			double result;
			DistRay3Segment3.SquaredDistance(ref r, ref s, out num, out result);
			return result;
		}

		public DistRay3Segment3 Compute()
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
			Vector3d vector3d = this.ray.Origin - this.segment.Center;
			double num = -this.ray.Direction.Dot(this.segment.Direction);
			double num2 = vector3d.Dot(this.ray.Direction);
			double num3 = -vector3d.Dot(this.segment.Direction);
			double lengthSquared = vector3d.LengthSquared;
			double num4 = Math.Abs(1.0 - num * num);
			double num5;
			double num6;
			double num9;
			if (num4 >= 1E-08)
			{
				num5 = num * num3 - num2;
				num6 = num * num2 - num3;
				double num7 = this.segment.Extent * num4;
				if (num5 >= 0.0)
				{
					if (num6 >= -num7)
					{
						if (num6 <= num7)
						{
							double num8 = 1.0 / num4;
							num5 *= num8;
							num6 *= num8;
							num9 = num5 * (num5 + num * num6 + 2.0 * num2) + num6 * (num * num5 + num6 + 2.0 * num3) + lengthSquared;
						}
						else
						{
							num6 = this.segment.Extent;
							num5 = -(num * num6 + num2);
							if (num5 > 0.0)
							{
								num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
							else
							{
								num5 = 0.0;
								num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
						}
					}
					else
					{
						num6 = -this.segment.Extent;
						num5 = -(num * num6 + num2);
						if (num5 > 0.0)
						{
							num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
						else
						{
							num5 = 0.0;
							num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
					}
				}
				else if (num6 <= -num7)
				{
					num5 = -(-num * this.segment.Extent + num2);
					if (num5 > 0.0)
					{
						num6 = -this.segment.Extent;
						num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
					else
					{
						num5 = 0.0;
						num6 = -num3;
						if (num6 < -this.segment.Extent)
						{
							num6 = -this.segment.Extent;
						}
						else if (num6 > this.segment.Extent)
						{
							num6 = this.segment.Extent;
						}
						num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
				}
				else if (num6 <= num7)
				{
					num5 = 0.0;
					num6 = -num3;
					if (num6 < -this.segment.Extent)
					{
						num6 = -this.segment.Extent;
					}
					else if (num6 > this.segment.Extent)
					{
						num6 = this.segment.Extent;
					}
					num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
				}
				else
				{
					num5 = -(num * this.segment.Extent + num2);
					if (num5 > 0.0)
					{
						num6 = this.segment.Extent;
						num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
					else
					{
						num5 = 0.0;
						num6 = -num3;
						if (num6 < -this.segment.Extent)
						{
							num6 = -this.segment.Extent;
						}
						else if (num6 > this.segment.Extent)
						{
							num6 = this.segment.Extent;
						}
						num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
				}
			}
			else
			{
				if (num > 0.0)
				{
					num6 = -this.segment.Extent;
				}
				else
				{
					num6 = this.segment.Extent;
				}
				num5 = -(num * num6 + num2);
				if (num5 > 0.0)
				{
					num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
				}
				else
				{
					num5 = 0.0;
					num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
				}
			}
			this.RayClosest = this.ray.Origin + num5 * this.ray.Direction;
			this.SegmentClosest = this.segment.Center + num6 * this.segment.Direction;
			this.RayParameter = num5;
			this.SegmentParameter = num6;
			if (num9 < 0.0)
			{
				num9 = 0.0;
			}
			this.DistanceSquared = num9;
			return this.DistanceSquared;
		}

		public static double SquaredDistance(ref Ray3d ray, ref Segment3d segment, out double rayT, out double segT)
		{
			Vector3d vector3d = ray.Origin - segment.Center;
			double num = -ray.Direction.Dot(segment.Direction);
			double num2 = vector3d.Dot(ray.Direction);
			double num3 = -vector3d.Dot(segment.Direction);
			double lengthSquared = vector3d.LengthSquared;
			double num4 = Math.Abs(1.0 - num * num);
			double num5;
			double num6;
			double num9;
			if (num4 >= 1E-08)
			{
				num5 = num * num3 - num2;
				num6 = num * num2 - num3;
				double num7 = segment.Extent * num4;
				if (num5 >= 0.0)
				{
					if (num6 >= -num7)
					{
						if (num6 <= num7)
						{
							double num8 = 1.0 / num4;
							num5 *= num8;
							num6 *= num8;
							num9 = num5 * (num5 + num * num6 + 2.0 * num2) + num6 * (num * num5 + num6 + 2.0 * num3) + lengthSquared;
						}
						else
						{
							num6 = segment.Extent;
							num5 = -(num * num6 + num2);
							if (num5 > 0.0)
							{
								num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
							else
							{
								num5 = 0.0;
								num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
							}
						}
					}
					else
					{
						num6 = -segment.Extent;
						num5 = -(num * num6 + num2);
						if (num5 > 0.0)
						{
							num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
						else
						{
							num5 = 0.0;
							num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
						}
					}
				}
				else if (num6 <= -num7)
				{
					num5 = -(-num * segment.Extent + num2);
					if (num5 > 0.0)
					{
						num6 = -segment.Extent;
						num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
					else
					{
						num5 = 0.0;
						num6 = -num3;
						if (num6 < -segment.Extent)
						{
							num6 = -segment.Extent;
						}
						else if (num6 > segment.Extent)
						{
							num6 = segment.Extent;
						}
						num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
				}
				else if (num6 <= num7)
				{
					num5 = 0.0;
					num6 = -num3;
					if (num6 < -segment.Extent)
					{
						num6 = -segment.Extent;
					}
					else if (num6 > segment.Extent)
					{
						num6 = segment.Extent;
					}
					num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
				}
				else
				{
					num5 = -(num * segment.Extent + num2);
					if (num5 > 0.0)
					{
						num6 = segment.Extent;
						num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
					else
					{
						num5 = 0.0;
						num6 = -num3;
						if (num6 < -segment.Extent)
						{
							num6 = -segment.Extent;
						}
						else if (num6 > segment.Extent)
						{
							num6 = segment.Extent;
						}
						num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
					}
				}
			}
			else
			{
				if (num > 0.0)
				{
					num6 = -segment.Extent;
				}
				else
				{
					num6 = segment.Extent;
				}
				num5 = -(num * num6 + num2);
				if (num5 > 0.0)
				{
					num9 = -num5 * num5 + num6 * (num6 + 2.0 * num3) + lengthSquared;
				}
				else
				{
					num5 = 0.0;
					num9 = num6 * (num6 + 2.0 * num3) + lengthSquared;
				}
			}
			rayT = num5;
			segT = num6;
			if (num9 < 0.0)
			{
				num9 = 0.0;
			}
			return num9;
		}

		private Ray3d ray;

		private Segment3d segment;

		public double DistanceSquared = -1.0;

		public Vector3d RayClosest;

		public double RayParameter;

		public Vector3d SegmentClosest;

		public double SegmentParameter;
	}
}
