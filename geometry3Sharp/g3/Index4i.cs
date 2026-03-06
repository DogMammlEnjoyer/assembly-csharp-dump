using System;
using Unity.Mathematics;

namespace g3
{
	public struct Index4i
	{
		public Index4i(int z)
		{
			this.d = z;
			this.c = z;
			this.b = z;
			this.a = z;
		}

		public Index4i(int aa, int bb, int cc, int dd)
		{
			this.a = aa;
			this.b = bb;
			this.c = cc;
			this.d = dd;
		}

		public Index4i(int[] i2)
		{
			this.a = i2[0];
			this.b = i2[1];
			this.c = i2[2];
			this.d = i2[3];
		}

		public Index4i(Index4i copy)
		{
			this.a = copy.a;
			this.b = copy.b;
			this.c = copy.b;
			this.d = copy.d;
		}

		public int this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.a;
				}
				if (key == 1)
				{
					return this.b;
				}
				if (key != 2)
				{
					return this.d;
				}
				return this.c;
			}
			set
			{
				if (key == 0)
				{
					this.a = value;
					return;
				}
				if (key == 1)
				{
					this.b = value;
					return;
				}
				if (key == 2)
				{
					this.c = value;
					return;
				}
				this.d = value;
			}
		}

		public int[] array
		{
			get
			{
				return new int[]
				{
					this.a,
					this.b,
					this.c,
					this.d
				};
			}
		}

		public int LengthSquared
		{
			get
			{
				return this.a * this.a + this.b * this.b + this.c * this.c + this.d * this.d;
			}
		}

		public int Length
		{
			get
			{
				return (int)Math.Sqrt((double)this.LengthSquared);
			}
		}

		public void Set(Index4i o)
		{
			this.a = o[0];
			this.b = o[1];
			this.c = o[2];
			this.d = o[3];
		}

		public void Set(int aa, int bb, int cc, int dd)
		{
			this.a = aa;
			this.b = bb;
			this.c = cc;
			this.d = dd;
		}

		public bool Contains(int val)
		{
			return this.a == val || this.b == val || this.c == val || this.d == val;
		}

		public void Sort()
		{
			if (this.d < this.c)
			{
				int num = this.d;
				this.d = this.c;
				this.c = num;
			}
			if (this.c < this.b)
			{
				int num = this.c;
				this.c = this.b;
				this.b = num;
			}
			if (this.b < this.a)
			{
				int num = this.b;
				this.b = this.a;
				this.a = num;
			}
			if (this.b > this.c)
			{
				int num = this.c;
				this.c = this.b;
				this.b = num;
			}
			if (this.c > this.d)
			{
				int num = this.d;
				this.d = this.c;
				this.c = num;
			}
			if (this.b > this.c)
			{
				int num = this.c;
				this.c = this.b;
				this.b = num;
			}
		}

		public static Index4i operator -(Index4i v)
		{
			return new Index4i(-v.a, -v.b, -v.c, -v.d);
		}

		public static Index4i operator *(int f, Index4i v)
		{
			return new Index4i(f * v.a, f * v.b, f * v.c, f * v.d);
		}

		public static Index4i operator *(Index4i v, int f)
		{
			return new Index4i(f * v.a, f * v.b, f * v.c, f * v.d);
		}

		public static Index4i operator /(Index4i v, int f)
		{
			return new Index4i(v.a / f, v.b / f, v.c / f, v.d / f);
		}

		public static Index4i operator *(Index4i a, Index4i b)
		{
			return new Index4i(a.a * b.a, a.b * b.b, a.c * b.c, a.d * b.d);
		}

		public static Index4i operator /(Index4i a, Index4i b)
		{
			return new Index4i(a.a / b.a, a.b / b.b, a.c / b.c, a.d / b.d);
		}

		public static Index4i operator +(Index4i v0, Index4i v1)
		{
			return new Index4i(v0.a + v1.a, v0.b + v1.b, v0.c + v1.c, v0.d + v1.d);
		}

		public static Index4i operator +(Index4i v0, int f)
		{
			return new Index4i(v0.a + f, v0.b + f, v0.c + f, v0.d + f);
		}

		public static Index4i operator -(Index4i v0, Index4i v1)
		{
			return new Index4i(v0.a - v1.a, v0.b - v1.b, v0.c - v1.c, v0.d - v1.d);
		}

		public static Index4i operator -(Index4i v0, int f)
		{
			return new Index4i(v0.a - f, v0.b - f, v0.c - f, v0.d - f);
		}

		public static bool operator ==(Index4i a, Index4i b)
		{
			return a.a == b.a && a.b == b.b && a.c == b.c && a.d == b.d;
		}

		public static bool operator !=(Index4i a, Index4i b)
		{
			return a.a != b.a || a.b != b.b || a.c != b.c || a.d != b.d;
		}

		public override bool Equals(object obj)
		{
			return this == (Index4i)obj;
		}

		public override int GetHashCode()
		{
			return (((-2128831035 * 16777619 ^ this.a.GetHashCode()) * 16777619 ^ this.b.GetHashCode()) * 16777619 ^ this.c.GetHashCode()) * 16777619 ^ this.d.GetHashCode();
		}

		public int CompareTo(Index4i other)
		{
			if (this.a != other.a)
			{
				if (this.a >= other.a)
				{
					return 1;
				}
				return -1;
			}
			else if (this.b != other.b)
			{
				if (this.b >= other.b)
				{
					return 1;
				}
				return -1;
			}
			else if (this.c != other.c)
			{
				if (this.c >= other.c)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (this.d == other.d)
				{
					return 0;
				}
				if (this.d >= other.d)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(Index4i other)
		{
			return this.a == other.a && this.b == other.b && this.c == other.c && this.d == other.d;
		}

		public override string ToString()
		{
			return string.Format("[{0},{1},{2},{3}]", new object[]
			{
				this.a,
				this.b,
				this.c,
				this.d
			});
		}

		public static implicit operator Index4i(int4 v)
		{
			return new Index4i(v.x, v.y, v.z, v.w);
		}

		public static implicit operator int4(Index4i v)
		{
			return new int4(v.a, v.b, v.c, v.d);
		}

		public int a;

		public int b;

		public int c;

		public int d;

		public static readonly Index4i Zero = new Index4i(0, 0, 0, 0);

		public static readonly Index4i One = new Index4i(1, 1, 1, 1);

		public static readonly Index4i Max = new Index4i(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
	}
}
