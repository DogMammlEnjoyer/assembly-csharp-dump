using System;
using Unity.Mathematics;

namespace g3
{
	public struct Index2i : IComparable<Index2i>, IEquatable<Index2i>
	{
		public Index2i(int z)
		{
			this.b = z;
			this.a = z;
		}

		public Index2i(int ii, int jj)
		{
			this.a = ii;
			this.b = jj;
		}

		public Index2i(int[] i2)
		{
			this.a = i2[0];
			this.b = i2[1];
		}

		public Index2i(Index2i copy)
		{
			this.a = copy.a;
			this.b = copy.b;
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

		public int[] array
		{
			get
			{
				return new int[]
				{
					this.a,
					this.b
				};
			}
		}

		public int LengthSquared
		{
			get
			{
				return this.a * this.a + this.b * this.b;
			}
		}

		public int Length
		{
			get
			{
				return (int)Math.Sqrt((double)this.LengthSquared);
			}
		}

		public void Set(Index2i o)
		{
			this.a = o[0];
			this.b = o[1];
		}

		public void Set(int ii, int jj)
		{
			this.a = ii;
			this.b = jj;
		}

		public static Index2i operator -(Index2i v)
		{
			return new Index2i(-v.a, -v.b);
		}

		public static Index2i operator *(int f, Index2i v)
		{
			return new Index2i(f * v.a, f * v.b);
		}

		public static Index2i operator *(Index2i v, int f)
		{
			return new Index2i(f * v.a, f * v.b);
		}

		public static Index2i operator /(Index2i v, int f)
		{
			return new Index2i(v.a / f, v.b / f);
		}

		public static Index2i operator *(Index2i a, Index2i b)
		{
			return new Index2i(a.a * b.a, a.b * b.b);
		}

		public static Index2i operator /(Index2i a, Index2i b)
		{
			return new Index2i(a.a / b.a, a.b / b.b);
		}

		public static Index2i operator +(Index2i v0, Index2i v1)
		{
			return new Index2i(v0.a + v1.a, v0.b + v1.b);
		}

		public static Index2i operator +(Index2i v0, int f)
		{
			return new Index2i(v0.a + f, v0.b + f);
		}

		public static Index2i operator -(Index2i v0, Index2i v1)
		{
			return new Index2i(v0.a - v1.a, v0.b - v1.b);
		}

		public static Index2i operator -(Index2i v0, int f)
		{
			return new Index2i(v0.a - f, v0.b - f);
		}

		public static bool operator ==(Index2i a, Index2i b)
		{
			return a.a == b.a && a.b == b.b;
		}

		public static bool operator !=(Index2i a, Index2i b)
		{
			return a.a != b.a || a.b != b.b;
		}

		public override bool Equals(object obj)
		{
			return this == (Index2i)obj;
		}

		public override int GetHashCode()
		{
			return (-2128831035 * 16777619 ^ this.a.GetHashCode()) * 16777619 ^ this.b.GetHashCode();
		}

		public int CompareTo(Index2i other)
		{
			if (this.a != other.a)
			{
				if (this.a >= other.a)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (this.b == other.b)
				{
					return 0;
				}
				if (this.b >= other.b)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(Index2i other)
		{
			return this.a == other.a && this.b == other.b;
		}

		public override string ToString()
		{
			return string.Format("[{0},{1}]", this.a, this.b);
		}

		public static implicit operator Index2i(int2 v)
		{
			return new Index2i(v.x, v.y);
		}

		public static implicit operator int2(Index2i v)
		{
			return new int2(v.a, v.b);
		}

		public int a;

		public int b;

		public static readonly Index2i Zero = new Index2i(0, 0);

		public static readonly Index2i One = new Index2i(1, 1);

		public static readonly Index2i Max = new Index2i(int.MaxValue, int.MaxValue);

		public static readonly Index2i Min = new Index2i(int.MinValue, int.MinValue);
	}
}
