using System;
using Unity.Mathematics;

namespace g3
{
	public struct Index3i : IComparable<Index3i>, IEquatable<Index3i>
	{
		public Index3i(int z)
		{
			this.c = z;
			this.b = z;
			this.a = z;
		}

		public Index3i(int ii, int jj, int kk)
		{
			this.a = ii;
			this.b = jj;
			this.c = kk;
		}

		public Index3i(int[] i2)
		{
			this.a = i2[0];
			this.b = i2[1];
			this.c = i2[2];
		}

		public Index3i(Index3i copy)
		{
			this.a = copy.a;
			this.b = copy.b;
			this.c = copy.b;
		}

		public Index3i(int ii, int jj, int kk, bool cycle)
		{
			this.a = ii;
			if (cycle)
			{
				this.b = kk;
				this.c = jj;
				return;
			}
			this.b = jj;
			this.c = kk;
		}

		public int this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.a;
				}
				if (key != 1)
				{
					return this.c;
				}
				return this.b;
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
				this.c = value;
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
					this.c
				};
			}
		}

		public int LengthSquared
		{
			get
			{
				return this.a * this.a + this.b * this.b + this.c * this.c;
			}
		}

		public int Length
		{
			get
			{
				return (int)Math.Sqrt((double)this.LengthSquared);
			}
		}

		public void Set(Index3i o)
		{
			this.a = o[0];
			this.b = o[1];
			this.c = o[2];
		}

		public void Set(int ii, int jj, int kk)
		{
			this.a = ii;
			this.b = jj;
			this.c = kk;
		}

		public static Index3i operator -(Index3i v)
		{
			return new Index3i(-v.a, -v.b, -v.c);
		}

		public static Index3i operator *(int f, Index3i v)
		{
			return new Index3i(f * v.a, f * v.b, f * v.c);
		}

		public static Index3i operator *(Index3i v, int f)
		{
			return new Index3i(f * v.a, f * v.b, f * v.c);
		}

		public static Index3i operator /(Index3i v, int f)
		{
			return new Index3i(v.a / f, v.b / f, v.c / f);
		}

		public static Index3i operator *(Index3i a, Index3i b)
		{
			return new Index3i(a.a * b.a, a.b * b.b, a.c * b.c);
		}

		public static Index3i operator /(Index3i a, Index3i b)
		{
			return new Index3i(a.a / b.a, a.b / b.b, a.c / b.c);
		}

		public static Index3i operator +(Index3i v0, Index3i v1)
		{
			return new Index3i(v0.a + v1.a, v0.b + v1.b, v0.c + v1.c);
		}

		public static Index3i operator +(Index3i v0, int f)
		{
			return new Index3i(v0.a + f, v0.b + f, v0.c + f);
		}

		public static Index3i operator -(Index3i v0, Index3i v1)
		{
			return new Index3i(v0.a - v1.a, v0.b - v1.b, v0.c - v1.c);
		}

		public static Index3i operator -(Index3i v0, int f)
		{
			return new Index3i(v0.a - f, v0.b - f, v0.c - f);
		}

		public static bool operator ==(Index3i a, Index3i b)
		{
			return a.a == b.a && a.b == b.b && a.c == b.c;
		}

		public static bool operator !=(Index3i a, Index3i b)
		{
			return a.a != b.a || a.b != b.b || a.c != b.c;
		}

		public override bool Equals(object obj)
		{
			return this == (Index3i)obj;
		}

		public override int GetHashCode()
		{
			return ((-2128831035 * 16777619 ^ this.a.GetHashCode()) * 16777619 ^ this.b.GetHashCode()) * 16777619 ^ this.c.GetHashCode();
		}

		public int CompareTo(Index3i other)
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
			else
			{
				if (this.c == other.c)
				{
					return 0;
				}
				if (this.c >= other.c)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(Index3i other)
		{
			return this.a == other.a && this.b == other.b && this.c == other.c;
		}

		public override string ToString()
		{
			return string.Format("[{0},{1},{2}]", this.a, this.b, this.c);
		}

		public static implicit operator Index3i(int3 v)
		{
			return new Index3i(v.x, v.y, v.z);
		}

		public static implicit operator int3(Index3i v)
		{
			return new int3(v.a, v.b, v.c);
		}

		public int a;

		public int b;

		public int c;

		public static readonly Index3i Zero = new Index3i(0, 0, 0);

		public static readonly Index3i One = new Index3i(1, 1, 1);

		public static readonly Index3i Max = new Index3i(int.MaxValue, int.MaxValue, int.MaxValue);

		public static readonly Index3i Min = new Index3i(int.MinValue, int.MinValue, int.MinValue);
	}
}
