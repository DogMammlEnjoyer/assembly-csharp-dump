using System;

namespace g3
{
	public class Ellipse2d : IParametricCurve2d
	{
		public Ellipse2d(Vector2d center, Vector2d axis0, Vector2d axis1, Vector2d extent)
		{
			this.Center = center;
			this.Axis0 = axis0;
			this.Axis1 = axis1;
			this.Extent.x = extent.x;
			this.Extent.y = extent.y;
			this.IsReversed = false;
		}

		public Ellipse2d(Vector2d center, Vector2d axis0, Vector2d axis1, double extent0, double extent1)
		{
			this.Center = center;
			this.Axis0 = axis0;
			this.Axis1 = axis1;
			this.Extent.x = extent0;
			this.Extent.y = extent1;
			this.IsReversed = false;
		}

		public Ellipse2d(Vector2d center, double rotationAngleDeg, double extent0, double extent1)
		{
			this.Center = center;
			Matrix2d m = new Matrix2d(rotationAngleDeg * 0.017453292519943295, false);
			this.Axis0 = m * Vector2d.AxisX;
			this.Axis1 = m * Vector2d.AxisY;
			this.Extent = new Vector2d(extent0, extent1);
			this.IsReversed = false;
		}

		public Matrix2d GetM()
		{
			Vector2d vector2d = this.Axis0 / this.Extent[0];
			Vector2d vector2d2 = this.Axis1 / this.Extent[1];
			return new Matrix2d(vector2d, vector2d) + new Matrix2d(vector2d2, vector2d2);
		}

		public Matrix2d GetMInverse()
		{
			Vector2d vector2d = this.Axis0 * this.Extent[0];
			Vector2d vector2d2 = this.Axis1 * this.Extent[1];
			return new Matrix2d(vector2d, vector2d) + new Matrix2d(vector2d2, vector2d2);
		}

		public double[] ToCoefficients()
		{
			Matrix2d zero = Matrix2d.Zero;
			Vector2d zero2 = Vector2d.Zero;
			double c = 0.0;
			this.ToCoefficients(ref zero, ref zero2, ref c);
			double[] array = Ellipse2d.Convert(zero, zero2, c);
			double num = Math.Abs(array[3]);
			int num2 = 3;
			double num3 = Math.Abs(array[5]);
			if (num3 > num)
			{
				num = num3;
				num2 = 5;
			}
			double num4 = 1.0 / num;
			for (int i = 0; i < 6; i++)
			{
				if (i != num2)
				{
					array[i] *= num4;
				}
				else
				{
					array[i] = 1.0;
				}
			}
			return array;
		}

		public void ToCoefficients(ref Matrix2d A, ref Vector2d B, ref double C)
		{
			Vector2d vector2d = this.Axis0 / this.Extent[0];
			Vector2d vector2d2 = this.Axis1 / this.Extent[1];
			A = new Matrix2d(vector2d, vector2d) + new Matrix2d(vector2d2, vector2d2);
			B = -2.0 * (A * this.Center);
			C = A.QForm(this.Center, this.Center) - 1.0;
		}

		public bool FromCoefficients(double[] coeff)
		{
			Matrix2d zero = Matrix2d.Zero;
			Vector2d zero2 = Vector2d.Zero;
			double c = 0.0;
			Ellipse2d.Convert(coeff, ref zero, ref zero2, ref c);
			return this.FromCoefficients(zero, zero2, c);
		}

		public bool FromCoefficients(Matrix2d A, Vector2d B, double C)
		{
			throw new NotImplementedException("Ellipse2.FromCoefficients: need EigenDecomposition");
		}

		public double Evaluate(Vector2d point)
		{
			Vector2d v = point - this.Center;
			double num = this.Axis0.Dot(v) / this.Extent[0];
			double num2 = this.Axis1.Dot(v) / this.Extent[1];
			return num * num + num2 * num2 - 1.0;
		}

		public bool Contains(Vector2d point)
		{
			return this.Evaluate(point) <= 0.0;
		}

		private static void Convert(double[] coeff, ref Matrix2d A, ref Vector2d B, ref double C)
		{
			C = coeff[0];
			B.x = coeff[1];
			B.y = coeff[2];
			A.m00 = coeff[3];
			A.m01 = 0.5 * coeff[4];
			A.m10 = A.m01;
			A.m11 = coeff[5];
		}

		private static double[] Convert(Matrix2d A, Vector2d B, double C)
		{
			return new double[]
			{
				C,
				B.x,
				B.y,
				A.m00,
				2.0 * A.m01,
				A.m11
			};
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
			return new Ellipse2d(this.Center, this.Axis0, this.Axis1, this.Extent)
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

		public Vector2d SampleDeg(double degrees)
		{
			double num = degrees * 0.017453292519943295;
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			return this.Center + this.Extent.x * num2 * this.Axis0 + this.Extent.y * num3 * this.Axis1;
		}

		public Vector2d SampleRad(double radians)
		{
			double num = Math.Cos(radians);
			double num2 = Math.Sin(radians);
			return this.Center + this.Extent.x * num * this.Axis0 + this.Extent.y * num2 * this.Axis1;
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
			return this.Center + this.Extent.x * num2 * this.Axis0 + this.Extent.y * num3 * this.Axis1;
		}

		public Vector2d TangentT(double t)
		{
			double num = this.IsReversed ? (-t * 6.283185307179586) : (t * 6.283185307179586);
			double num2 = Math.Cos(num);
			double num3 = Math.Sin(num);
			Vector2d vector2d = -this.Extent.x * num3 * this.Axis0 + this.Extent.y * num2 * this.Axis1;
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

		public double Area
		{
			get
			{
				return 3.141592653589793 * this.Extent.x * this.Extent.y;
			}
		}

		public double ApproxArcLen
		{
			get
			{
				double num = Math.Max(this.Extent.x, this.Extent.y);
				double num2 = Math.Min(this.Extent.x, this.Extent.y);
				double num3 = (num - num2) / (num + num2);
				double num4 = 3.0 * num3 * num3;
				double num5 = 10.0 + Math.Sqrt(4.0 - num4);
				return 3.141592653589793 * (num + num2) * (1.0 + num4 / num5);
			}
		}

		public Vector2d Center;

		public Vector2d Axis0;

		public Vector2d Axis1;

		public Vector2d Extent;

		public bool IsReversed;
	}
}
