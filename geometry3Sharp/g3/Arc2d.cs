using System;

namespace g3
{
	public class Arc2d : IParametricCurve2d
	{
		public Arc2d(Vector2d center, double radius, double startDeg, double endDeg)
		{
			this.IsReversed = false;
			this.Center = center;
			this.Radius = radius;
			this.AngleStartDeg = startDeg;
			this.AngleEndDeg = endDeg;
			if (this.AngleEndDeg < this.AngleStartDeg)
			{
				this.AngleEndDeg += 360.0;
			}
		}

		public Arc2d(Vector2d vCenter, Vector2d vStart, Vector2d vEnd)
		{
			this.IsReversed = false;
			this.SetFromCenterAndPoints(vCenter, vStart, vEnd);
		}

		public void SetFromCenterAndPoints(Vector2d vCenter, Vector2d vStart, Vector2d vEnd)
		{
			Vector2d vector2d = vStart - vCenter;
			Vector2d vector2d2 = vEnd - vCenter;
			this.AngleStartDeg = Math.Atan2(vector2d.y, vector2d.x) * 57.29577951308232;
			this.AngleEndDeg = Math.Atan2(vector2d2.y, vector2d2.x) * 57.29577951308232;
			if (this.AngleEndDeg < this.AngleStartDeg)
			{
				this.AngleEndDeg += 360.0;
			}
			this.Center = vCenter;
			this.Radius = vector2d.Length;
		}

		public Vector2d P0
		{
			get
			{
				return this.SampleT(0.0);
			}
		}

		public Vector2d P1
		{
			get
			{
				return this.SampleT(1.0);
			}
		}

		public double Curvature
		{
			get
			{
				return 1.0 / this.Radius;
			}
		}

		public double SignedCurvature
		{
			get
			{
				if (!this.IsReversed)
				{
					return 1.0 / this.Radius;
				}
				return -1.0 / this.Radius;
			}
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

		public Vector2d SampleT(double t)
		{
			double num = (this.IsReversed ? ((1.0 - t) * this.AngleEndDeg + t * this.AngleStartDeg) : ((1.0 - t) * this.AngleStartDeg + t * this.AngleEndDeg)) * 0.017453292519943295;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			return new Vector2d(this.Center.x + this.Radius * num2, this.Center.y + this.Radius * num3);
		}

		public Vector2d TangentT(double t)
		{
			double num = this.IsReversed ? ((1.0 - t) * this.AngleEndDeg + t * this.AngleStartDeg) : ((1.0 - t) * this.AngleStartDeg + t * this.AngleEndDeg);
			num *= 0.017453292519943295;
			Vector2d vector2d = new Vector2d(-Math.Sin(num), Math.Cos(num));
			if (this.IsReversed)
			{
				vector2d = -vector2d;
			}
			vector2d.Normalize(2.220446049250313E-16);
			return vector2d;
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
				return (this.AngleEndDeg - this.AngleStartDeg) * 0.017453292519943295 * this.Radius;
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			if (this.ArcLength >= 2.220446049250313E-16)
			{
				double num = a / this.ArcLength;
				double num2 = (this.IsReversed ? ((1.0 - num) * this.AngleEndDeg + num * this.AngleStartDeg) : ((1.0 - num) * this.AngleStartDeg + num * this.AngleEndDeg)) * 0.017453292519943295;
				double num3 = Math.Cos(num2);
				double num4 = Math.Sin(num2);
				return new Vector2d(this.Center.x + this.Radius * num3, this.Center.y + this.Radius * num4);
			}
			if (a >= 0.5)
			{
				return this.SampleT(1.0);
			}
			return this.SampleT(0.0);
		}

		public void Reverse()
		{
			this.IsReversed = !this.IsReversed;
		}

		public IParametricCurve2d Clone()
		{
			return new Arc2d(this.Center, this.Radius, this.AngleStartDeg, this.AngleEndDeg)
			{
				IsReversed = this.IsReversed
			};
		}

		public bool IsTransformable
		{
			get
			{
				return true;
			}
		}

		public void Transform(ITransform2 xform)
		{
			Vector2d vCenter = xform.TransformP(this.Center);
			Vector2d vStart = xform.TransformP(this.IsReversed ? this.P1 : this.P0);
			Vector2d vEnd = xform.TransformP(this.IsReversed ? this.P0 : this.P1);
			this.SetFromCenterAndPoints(vCenter, vStart, vEnd);
		}

		public AxisAlignedBox2d Bounds
		{
			get
			{
				int i = (int)(this.AngleStartDeg / 90.0);
				if ((double)(i * 90) < this.AngleStartDeg)
				{
					i++;
				}
				int num = (int)(this.AngleEndDeg / 90.0);
				if ((double)(num * 90) > this.AngleEndDeg)
				{
					num--;
				}
				AxisAlignedBox2d empty = AxisAlignedBox2d.Empty;
				while (i <= num)
				{
					int num2 = i++ % 4;
					empty.Contain(Arc2d.bounds_dirs[num2]);
				}
				empty.Scale(this.Radius);
				empty.Translate(this.Center);
				empty.Contain(this.P0);
				empty.Contain(this.P1);
				return empty;
			}
		}

		public double Distance(Vector2d point)
		{
			Vector2d v = point - this.Center;
			double length = v.Length;
			if (length <= 2.220446049250313E-16)
			{
				return this.Radius;
			}
			Vector2d vector2d = v / length;
			double num = Math.Atan2(vector2d.y, vector2d.x) * 57.29577951308232;
			if (num < this.AngleStartDeg || num > this.AngleEndDeg)
			{
				double num2 = MathUtil.ClampAngleDeg(num, this.AngleStartDeg, this.AngleEndDeg) * 0.017453292519943295;
				double num3 = Math.Cos(num2);
				double num4 = Math.Sin(num2);
				Vector2d vector2d2 = new Vector2d(this.Center.x + this.Radius * num3, this.Center.y + this.Radius * num4);
				return vector2d2.Distance(point);
			}
			return Math.Abs(length - this.Radius);
		}

		public Vector2d NearestPoint(Vector2d point)
		{
			Vector2d v = point - this.Center;
			double length = v.Length;
			if (length > 2.220446049250313E-16)
			{
				Vector2d vector2d = v / length;
				double num = Math.Atan2(vector2d.y, vector2d.x);
				num *= 57.29577951308232;
				num = MathUtil.ClampAngleDeg(num, this.AngleStartDeg, this.AngleEndDeg);
				num = 0.017453292519943295 * num;
				double num2 = Math.Cos(num);
				double num3 = Math.Sin(num);
				return new Vector2d(this.Center.x + this.Radius * num2, this.Center.y + this.Radius * num3);
			}
			return this.SampleT(0.5);
		}

		public Vector2d Center;

		public double Radius;

		public double AngleStartDeg;

		public double AngleEndDeg;

		public bool IsReversed;

		private static readonly Vector2d[] bounds_dirs = new Vector2d[]
		{
			Vector2d.AxisX,
			Vector2d.AxisY,
			-Vector2d.AxisX,
			-Vector2d.AxisY
		};
	}
}
