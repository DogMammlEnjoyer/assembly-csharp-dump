using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public struct Interval1i : IEnumerable<int>, IEnumerable
	{
		public Interval1i(int f)
		{
			this.b = f;
			this.a = f;
		}

		public Interval1i(int x, int y)
		{
			this.a = x;
			this.b = y;
		}

		public Interval1i(int[] v2)
		{
			this.a = v2[0];
			this.b = v2[1];
		}

		public Interval1i(Interval1i copy)
		{
			this.a = copy.a;
			this.b = copy.b;
		}

		public static Interval1i Range(int N)
		{
			return new Interval1i(0, N - 1);
		}

		public static Interval1i RangeInclusive(int N)
		{
			return new Interval1i(0, N);
		}

		public static Interval1i Range(int start, int N)
		{
			return new Interval1i(start, start + N - 1);
		}

		public static Interval1i FromToInclusive(int a, int b)
		{
			return new Interval1i(a, b);
		}

		public int this[int key]
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

		public int LengthSquared
		{
			get
			{
				return (this.a - this.b) * (this.a - this.b);
			}
		}

		public int Length
		{
			get
			{
				return this.b - this.a;
			}
		}

		public int Center
		{
			get
			{
				return (this.b + this.a) / 2;
			}
		}

		public void Contain(int d)
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

		public bool Contains(int d)
		{
			return d >= this.a && d <= this.b;
		}

		public bool Overlaps(Interval1i o)
		{
			return o.a <= this.b && o.b >= this.a;
		}

		public int SquaredDist(Interval1i o)
		{
			if (this.b < o.a)
			{
				return (o.a - this.b) * (o.a - this.b);
			}
			if (this.a > o.b)
			{
				return (this.a - o.b) * (this.a - o.b);
			}
			return 0;
		}

		public int Dist(Interval1i o)
		{
			if (this.b < o.a)
			{
				return o.a - this.b;
			}
			if (this.a > o.b)
			{
				return this.a - o.b;
			}
			return 0;
		}

		public void Set(Interval1i o)
		{
			this.a = o.a;
			this.b = o.b;
		}

		public void Set(int fA, int fB)
		{
			this.a = fA;
			this.b = fB;
		}

		public static Interval1i operator -(Interval1i v)
		{
			return new Interval1i(-v.a, -v.b);
		}

		public static Interval1i operator +(Interval1i a, int f)
		{
			return new Interval1i(a.a + f, a.b + f);
		}

		public static Interval1i operator -(Interval1i a, int f)
		{
			return new Interval1i(a.a - f, a.b - f);
		}

		public static Interval1i operator *(Interval1i a, int f)
		{
			return new Interval1i(a.a * f, a.b * f);
		}

		public IEnumerator<int> GetEnumerator()
		{
			int num;
			for (int i = this.a; i <= this.b; i = num)
			{
				yield return i;
				num = i + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override string ToString()
		{
			return string.Format("[{0},{1}]", this.a, this.b);
		}

		public int a;

		public int b;

		public static readonly Interval1i Zero = new Interval1i(0, 0);

		public static readonly Interval1i Empty = new Interval1i(int.MaxValue, -2147483647);

		public static readonly Interval1i Infinite = new Interval1i(-2147483647, int.MaxValue);
	}
}
