using System;

namespace g3
{
	public abstract class BaseCurve2
	{
		public BaseCurve2(double tmin, double tmax)
		{
			this.mTMin = tmin;
			this.mTMax = tmax;
		}

		public double GetMinTime()
		{
			return this.mTMax;
		}

		public double GetMaxTime()
		{
			return this.mTMax;
		}

		public void SetTimeInterval(double tmin, double tmax)
		{
			if (tmin >= tmax)
			{
				throw new Exception("Curve2.SetTimeInterval: invalid min/max");
			}
			this.mTMin = tmin;
			this.mTMax = tmax;
		}

		public abstract Vector2d GetPosition(double t);

		public abstract Vector2d GetFirstDerivative(double t);

		public abstract Vector2d GetSecondDerivative(double t);

		public abstract Vector2d GetThirdDerivative(double t);

		public double GetSpeed(double t)
		{
			return this.GetFirstDerivative(t).Length;
		}

		private double GetSpeedWithData(double t, object data)
		{
			return (data as BaseCurve2).GetSpeed(t);
		}

		public virtual double GetLength(double t0, double t1)
		{
			if (t0 < this.mTMin || t0 > this.mTMax)
			{
				throw new Exception("BaseCurve2.GetLength: min t out of bounds: " + t0.ToString());
			}
			if (t1 < this.mTMin || t1 > this.mTMax)
			{
				throw new Exception("BaseCurve2.GetLength: max t out of bounds: " + t1.ToString());
			}
			if (t0 > t1)
			{
				throw new Exception("BaseCurve2.GetLength: inverted t-range\n " + t0.ToString() + " " + t1.ToString());
			}
			return Integrate1d.RombergIntegral(8, t0, t1, new Func<double, object, double>(this.GetSpeedWithData), this);
		}

		public double GetTotalLength()
		{
			return this.GetLength(this.mTMin, this.mTMax);
		}

		public Vector2d GetTangent(double t)
		{
			return this.GetFirstDerivative(t).Normalized;
		}

		public Vector2d GetNormal(double t)
		{
			return this.GetFirstDerivative(t).Normalized.Perp;
		}

		public void GetFrame(double t, ref Vector2d position, ref Vector2d tangent, ref Vector2d normal)
		{
			position = this.GetPosition(t);
			tangent = this.GetFirstDerivative(t).Normalized;
			normal = tangent.Perp;
		}

		public double GetCurvature(double t)
		{
			Vector2d firstDerivative = this.GetFirstDerivative(t);
			Vector2d secondDerivative = this.GetSecondDerivative(t);
			double lengthSquared = firstDerivative.LengthSquared;
			if (lengthSquared >= 1E-08)
			{
				double num = firstDerivative.DotPerp(secondDerivative);
				double num2 = Math.Pow(lengthSquared, 1.5);
				return num / num2;
			}
			return 0.0;
		}

		public virtual double GetTime(double length, int iterations = 32, double tolerance = 1E-06)
		{
			if (length <= 0.0)
			{
				return this.mTMin;
			}
			if (length >= this.GetTotalLength())
			{
				return this.mTMax;
			}
			double num = length / this.GetTotalLength();
			double num2 = (1.0 - num) * this.mTMin + num * this.mTMax;
			double num3 = this.mTMin;
			double num4 = this.mTMax;
			for (int i = 0; i < iterations; i++)
			{
				double num5 = this.GetLength(this.mTMin, num2) - length;
				if (Math.Abs(num5) < tolerance)
				{
					return num2;
				}
				double num6 = num2 - num5 / this.GetSpeed(num2);
				if (num5 > 0.0)
				{
					num4 = num2;
					if (num6 <= num3)
					{
						num2 = 0.5 * (num4 + num3);
					}
					else
					{
						num2 = num6;
					}
				}
				else
				{
					num3 = num2;
					if (num6 >= num4)
					{
						num2 = 0.5 * (num4 + num3);
					}
					else
					{
						num2 = num6;
					}
				}
			}
			return num2;
		}

		private Vector2d[] SubdivideByTime(int numPoints)
		{
			if (numPoints < 2)
			{
				throw new Exception("BaseCurve2.SubdivideByTime: Subdivision requires at least two points, requested " + numPoints.ToString());
			}
			Vector2d[] array = new Vector2d[numPoints];
			double num = (this.mTMax - this.mTMin) / (double)(numPoints - 1);
			for (int i = 0; i < numPoints; i++)
			{
				double t = this.mTMin + num * (double)i;
				array[i] = this.GetPosition(t);
			}
			return array;
		}

		private Vector2d[] SubdivieByLength(int numPoints)
		{
			if (numPoints < 2)
			{
				throw new Exception("BaseCurve2.SubdivideByTime: Subdivision requires at least two points, requested " + numPoints.ToString());
			}
			Vector2d[] array = new Vector2d[numPoints];
			double num = this.GetTotalLength() / (double)(numPoints - 1);
			for (int i = 0; i < numPoints; i++)
			{
				double length = num * (double)i;
				double time = this.GetTime(length, 32, 1E-06);
				array[i] = this.GetPosition(time);
			}
			return array;
		}

		protected double mTMin;

		protected double mTMax;
	}
}
