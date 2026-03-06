using System;
using Unity.Mathematics;

namespace g3
{
	public struct Vector2i : IComparable<Vector2i>, IEquatable<Vector2i>
	{
		public Vector2i(int f)
		{
			this.y = f;
			this.x = f;
		}

		public Vector2i(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2i(int[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
		}

		public int this[int key]
		{
			get
			{
				if (key != 0)
				{
					return this.y;
				}
				return this.x;
			}
			set
			{
				if (key == 0)
				{
					this.x = value;
					return;
				}
				this.y = value;
			}
		}

		public int[] array
		{
			get
			{
				return new int[]
				{
					this.x,
					this.y
				};
			}
		}

		public void Add(int s)
		{
			this.x += s;
			this.y += s;
		}

		public int LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		public static Vector2i operator -(Vector2i v)
		{
			return new Vector2i(-v.x, -v.y);
		}

		public static Vector2i operator *(int f, Vector2i v)
		{
			return new Vector2i(f * v.x, f * v.y);
		}

		public static Vector2i operator *(Vector2i v, int f)
		{
			return new Vector2i(f * v.x, f * v.y);
		}

		public static Vector2i operator /(Vector2i v, int f)
		{
			return new Vector2i(v.x / f, v.y / f);
		}

		public static Vector2i operator /(int f, Vector2i v)
		{
			return new Vector2i(f / v.x, f / v.y);
		}

		public static Vector2i operator *(Vector2i a, Vector2i b)
		{
			return new Vector2i(a.x * b.x, a.y * b.y);
		}

		public static Vector2i operator /(Vector2i a, Vector2i b)
		{
			return new Vector2i(a.x / b.x, a.y / b.y);
		}

		public static Vector2i operator +(Vector2i v0, Vector2i v1)
		{
			return new Vector2i(v0.x + v1.x, v0.y + v1.y);
		}

		public static Vector2i operator +(Vector2i v0, int f)
		{
			return new Vector2i(v0.x + f, v0.y + f);
		}

		public static Vector2i operator -(Vector2i v0, Vector2i v1)
		{
			return new Vector2i(v0.x - v1.x, v0.y - v1.y);
		}

		public static Vector2i operator -(Vector2i v0, int f)
		{
			return new Vector2i(v0.x - f, v0.y - f);
		}

		public static bool operator ==(Vector2i a, Vector2i b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2i a, Vector2i b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector2i)obj;
		}

		public override int GetHashCode()
		{
			return (-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode();
		}

		public int CompareTo(Vector2i other)
		{
			if (this.x != other.x)
			{
				if (this.x >= other.x)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (this.y == other.y)
				{
					return 0;
				}
				if (this.y >= other.y)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(Vector2i other)
		{
			return this.x == other.x && this.y == other.y;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", this.x, this.y);
		}

		public static implicit operator Vector2i(int2 v)
		{
			return new Vector2i(v.x, v.y);
		}

		public static implicit operator int2(Vector2i v)
		{
			return new int2(v.x, v.y);
		}

		public int x;

		public int y;

		public static readonly Vector2i Zero = new Vector2i(0, 0);

		public static readonly Vector2i One = new Vector2i(1, 1);

		public static readonly Vector2i AxisX = new Vector2i(1, 0);

		public static readonly Vector2i AxisY = new Vector2i(0, 1);
	}
}
