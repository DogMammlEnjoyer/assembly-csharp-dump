using System;

namespace g3
{
	public class Intersector1
	{
		public Intersector1(double u0, double u1, double v0, double v1)
		{
			this.U = new Interval1d(u0, u1);
			this.V = new Interval1d(v0, v1);
		}

		public Intersector1(Interval1d u, Interval1d v)
		{
			this.U = u;
			this.V = v;
		}

		public bool Test
		{
			get
			{
				return this.U.a <= this.V.b && this.U.b >= this.V.a;
			}
		}

		public double GetIntersection(int i)
		{
			return this.Intersections[i];
		}

		public bool Find()
		{
			if (this.U.b < this.V.a || this.U.a > this.V.b)
			{
				this.NumIntersections = 0;
			}
			else if (this.U.b > this.V.a)
			{
				if (this.U.a < this.V.b)
				{
					this.NumIntersections = 2;
					this.Intersections.a = ((this.U.a < this.V.a) ? this.V.a : this.U.a);
					this.Intersections.b = ((this.U.b > this.V.b) ? this.V.b : this.U.b);
					if (this.Intersections.a == this.Intersections.b)
					{
						this.NumIntersections = 1;
					}
				}
				else
				{
					this.NumIntersections = 1;
					this.Intersections.a = this.U.a;
				}
			}
			else
			{
				this.NumIntersections = 1;
				this.Intersections.a = this.U.b;
			}
			return this.NumIntersections > 0;
		}

		public Interval1d U;

		public Interval1d V;

		public int NumIntersections;

		private Interval1d Intersections = Interval1d.Zero;
	}
}
