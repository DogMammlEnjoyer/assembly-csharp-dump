using System;

namespace g3
{
	public class Circle2d : IParametricCurve2d
	{
		public Circle2d(double radius)
		{
			this.IsReversed = false;
			this.Center = Vector2d.Zero;
			this.Radius = radius;
		}

		public Circle2d(Vector2d center, double radius)
		{
			this.IsReversed = false;
			this.Center = center;
			this.Radius = radius;
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
				return true;
			}
		}

		public void Reverse()
		{
			this.IsReversed = !this.IsReversed;
		}

		public IParametricCurve2d Clone()
		{
			return new Circle2d(this.Center, this.Radius)
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
			this.Radius = xform.TransformScalar(this.Radius);
		}

		public Vector2d SampleDeg(double degrees)
		{
			double num = degrees * 0.017453292519943295;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			return new Vector2d(this.Center.x + this.Radius * num2, this.Center.y + this.Radius * num3);
		}

		public Vector2d SampleRad(double radians)
		{
			double num = Math.Cos(radians);
			double num2 = Math.Sin(radians);
			return new Vector2d(this.Center.x + this.Radius * num, this.Center.y + this.Radius * num2);
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
			double num = this.IsReversed ? (-t * 6.283185307179586) : (t * 6.283185307179586);
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			return new Vector2d(this.Center.x + this.Radius * num2, this.Center.y + this.Radius * num3);
		}

		public Vector2d TangentT(double t)
		{
			double num = this.IsReversed ? (-t * 6.283185307179586) : (t * 6.283185307179586);
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
				return 6.283185307179586 * this.Radius;
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			double num = a / this.ArcLength;
			double num2 = this.IsReversed ? (-num * 6.283185307179586) : (num * 6.283185307179586);
			double num3 = Math.Cos(num2);
			double num4 = Math.Sin(num2);
			return new Vector2d(this.Center.x + this.Radius * num3, this.Center.y + this.Radius * num4);
		}

		public bool Contains(Vector2d p)
		{
			return this.Center.DistanceSquared(p) <= this.Radius * this.Radius;
		}

		public double Circumference
		{
			get
			{
				return 6.283185307179586 * this.Radius;
			}
			set
			{
				this.Radius = value / 6.283185307179586;
			}
		}

		public double Diameter
		{
			get
			{
				return 2.0 * this.Radius;
			}
			set
			{
				this.Radius = value / 2.0;
			}
		}

		public double Area
		{
			get
			{
				return 3.141592653589793 * this.Radius * this.Radius;
			}
			set
			{
				this.Radius = Math.Sqrt(value / 3.141592653589793);
			}
		}

		public AxisAlignedBox2d Bounds
		{
			get
			{
				return new AxisAlignedBox2d(this.Center, this.Radius, this.Radius);
			}
		}

		public double SignedDistance(Vector2d pt)
		{
			return this.Center.Distance(pt) - this.Radius;
		}

		public double Distance(Vector2d pt)
		{
			return Math.Abs(this.Center.Distance(pt) - this.Radius);
		}

		public static double RadiusArea(double r)
		{
			return 3.141592653589793 * r * r;
		}

		public static double RadiusCircumference(double r)
		{
			return 6.283185307179586 * r;
		}

		public static double BoundingPolygonRadius(double r, int n)
		{
			double d = 6.283185307179586 / (double)n / 2.0;
			return r / Math.Cos(d);
		}

		public Vector2d Center;

		public double Radius;

		public bool IsReversed;
	}
}
