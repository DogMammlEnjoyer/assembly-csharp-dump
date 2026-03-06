using System;

namespace g3
{
	public class EllipseArc2d : IParametricCurve2d
	{
		public EllipseArc2d(Vector2d center, double rotationAngleDeg, double extent0, double extent1, double startDeg, double endDeg)
		{
			this.Center = center;
			Matrix2d m = new Matrix2d(rotationAngleDeg * 0.017453292519943295, false);
			this.Axis0 = m * Vector2d.AxisX;
			this.Axis1 = m * Vector2d.AxisY;
			this.Extent = new Vector2d(extent0, extent1);
			this.IsReversed = false;
			this.AngleStartDeg = startDeg;
			this.AngleEndDeg = endDeg;
			if (this.AngleEndDeg < this.AngleStartDeg)
			{
				this.AngleEndDeg += 360.0;
			}
		}

		public EllipseArc2d(Vector2d center, Vector2d axis0, Vector2d axis1, Vector2d extent, double startDeg, double endDeg)
		{
			this.Center = center;
			this.Axis0 = axis0;
			this.Axis1 = axis1;
			this.Extent = extent;
			this.IsReversed = false;
			this.AngleStartDeg = startDeg;
			this.AngleEndDeg = endDeg;
			if (this.AngleEndDeg < this.AngleStartDeg)
			{
				this.AngleEndDeg += 360.0;
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
			double x = this.Extent.x;
			double y = this.Extent.y;
			double num4 = x * num3;
			double num5 = y * num2;
			double num6 = x * y / Math.Sqrt(num5 * num5 + num4 * num4);
			Vector2d vector2d = new Vector2d(num6 * num2, num6 * num3);
			return this.Center + vector2d.x * this.Axis0 + vector2d.y * this.Axis1;
		}

		public Vector2d TangentT(double t)
		{
			double num = (this.IsReversed ? ((1.0 - t) * this.AngleEndDeg + t * this.AngleStartDeg) : ((1.0 - t) * this.AngleStartDeg + t * this.AngleEndDeg)) * 0.017453292519943295;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			double x = this.Extent.x;
			double y = this.Extent.y;
			double num4 = x * num3;
			double num5 = y * num2;
			double num6 = num4 * num4 + num5 * num5;
			double num7 = Math.Sqrt(num6);
			double num8 = 0.5 * (1.0 / num7) * (2.0 * x * x * num3 * num2 - 2.0 * y * y * num2 * num3);
			double f = (-x * y * num3 * num7 - num8 * (x * y * num2)) / num6;
			double f2 = (x * y * num2 * num7 - num8 * (x * y * num3)) / num6;
			Vector2d vector2d = f * this.Axis0 + f2 * this.Axis1;
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
				return false;
			}
		}

		public double ArcLength
		{
			get
			{
				throw new NotImplementedException("Ellipse2.ArcLength");
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			throw new NotImplementedException("Ellipse2.SampleArcLength");
		}

		public void Reverse()
		{
			this.IsReversed = !this.IsReversed;
		}

		public IParametricCurve2d Clone()
		{
			return new EllipseArc2d(this.Center, this.Axis0, this.Axis1, this.Extent, this.AngleStartDeg, this.AngleEndDeg)
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
			this.Center = xform.TransformP(this.Center);
			this.Axis0 = xform.TransformN(this.Axis0);
			this.Axis1 = xform.TransformN(this.Axis1);
			this.Extent.x = xform.TransformScalar(this.Extent.x);
			this.Extent.y = xform.TransformScalar(this.Extent.y);
		}

		public Vector2d Center;

		public Vector2d Axis0;

		public Vector2d Axis1;

		public Vector2d Extent;

		public double AngleStartDeg;

		public double AngleEndDeg;

		public bool IsReversed;
	}
}
