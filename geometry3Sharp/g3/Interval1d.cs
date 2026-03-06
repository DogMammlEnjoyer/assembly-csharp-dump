using System;

namespace g3
{
	public struct Interval1d
	{
		public Interval1d(double f)
		{
			this.b = f;
			this.a = f;
		}

		public Interval1d(double x, double y)
		{
			this.a = x;
			this.b = y;
		}

		public Interval1d(double[] v2)
		{
			this.a = v2[0];
			this.b = v2[1];
		}

		public Interval1d(float f)
		{
			this.a = (this.b = (double)f);
		}

		public Interval1d(float x, float y)
		{
			this.a = (double)x;
			this.b = (double)y;
		}

		public Interval1d(float[] v2)
		{
			this.a = (double)v2[0];
			this.b = (double)v2[1];
		}

		public Interval1d(Interval1d copy)
		{
			this.a = copy.a;
			this.b = copy.b;
		}

		public static Interval1d Unsorted(double x, double y)
		{
			if (x >= y)
			{
				return new Interval1d(y, x);
			}
			return new Interval1d(x, y);
		}

		public double this[int key]
		{
			get
			{
				if (key != 0)
				{
					return this.b;
				}
				return this.a;
			}
			set
			{
				if (key == 0)
				{
					this.a = value;
					return;
				}
				this.b = value;
			}
		}

		public double LengthSquared
		{
			get
			{
				return (this.a - this.b) * (this.a - this.b);
			}
		}

		public double Length
		{
			get
			{
				return this.b - this.a;
			}
		}

		public bool IsConstant
		{
			get
			{
				return this.b == this.a;
			}
		}

		public double Center
		{
			get
			{
				return (this.b + this.a) * 0.5;
			}
		}

		public void Contain(double d)
		{
			if (d < this.a)
			{
				this.a = d;
			}
			if (d > this.b)
			{
				this.b = d;
			}
		}

		public bool Contains(double d)
		{
			return d >= this.a && d <= this.b;
		}

		public bool Overlaps(Interval1d o)
		{
			return o.a <= this.b && o.b >= this.a;
		}

		public double SquaredDist(Interval1d o)
		{
			if (this.b < o.a)
			{
				return (o.a - this.b) * (o.a - this.b);
			}
			if (this.a > o.b)
			{
				return (this.a - o.b) * (this.a - o.b);
			}
			return 0.0;
		}

		public double Dist(Interval1d o)
		{
			if (this.b < o.a)
			{
				return o.a - this.b;
			}
			if (this.a > o.b)
			{
				return this.a - o.b;
			}
			return 0.0;
		}

		public Interval1d IntersectionWith(ref Interval1d o)
		{
			if (o.a > this.b || o.b < this.a)
			{
				return Interval1d.Empty;
			}
			return new Interval1d(Math.Max(this.a, o.a), Math.Min(this.b, o.b));
		}

		public double Clamp(double f)
		{
			if (f < this.a)
			{
				return this.a;
			}
			if (f <= this.b)
			{
				return f;
			}
			return this.b;
		}

		public double Interpolate(double t)
		{
			return (1.0 - t) * this.a + t * this.b;
		}

		public double GetT(double value)
		{
			if (value <= this.a)
			{
				return 0.0;
			}
			if (value >= this.b)
			{
				return 1.0;
			}
			if (this.a == this.b)
			{
				return 0.5;
			}
			return (value - this.a) / (this.b - this.a);
		}

		public void Set(Interval1d o)
		{
			this.a = o.a;
			this.b = o.b;
		}

		public void Set(double fA, double fB)
		{
			this.a = fA;
			this.b = fB;
		}

		public static Interval1d operator -(Interval1d v)
		{
			return new Interval1d(-v.a, -v.b);
		}

		public static Interval1d operator +(Interval1d a, double f)
		{
			return new Interval1d(a.a + f, a.b + f);
		}

		public static Interval1d operator -(Interval1d a, double f)
		{
			return new Interval1d(a.a - f, a.b - f);
		}

		public static Interval1d operator *(Interval1d a, double f)
		{
			return new Interval1d(a.a * f, a.b * f);
		}

		public override string ToString()
		{
			return string.Format("[{0:F8},{1:F8}]", this.a, this.b);
		}

		public double a;

		public double b;

		public static readonly Interval1d Zero = new Interval1d(0f, 0f);

		public static readonly Interval1d Empty = new Interval1d(double.MaxValue, double.MinValue);

		public static readonly Interval1d Infinite = new Interval1d(double.MinValue, double.MaxValue);
	}
}
