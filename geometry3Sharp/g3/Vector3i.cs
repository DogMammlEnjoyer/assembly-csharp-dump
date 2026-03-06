using System;
using Unity.Mathematics;

namespace g3
{
	public struct Vector3i : IComparable<Vector3i>, IEquatable<Vector3i>
	{
		public Vector3i(int f)
		{
			this.z = f;
			this.y = f;
			this.x = f;
		}

		public Vector3i(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3i(int[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
			this.z = v2[2];
		}

		public int this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.x;
				}
				if (key != 1)
				{
					return this.z;
				}
				return this.y;
			}
			set
			{
				if (key == 0)
				{
					this.x = value;
					return;
				}
				if (key == 1)
				{
					this.y = value;
					return;
				}
				this.z = value;
			}
		}

		public int[] array
		{
			get
			{
				return new int[]
				{
					this.x,
					this.y,
					this.z
				};
			}
		}

		public void Set(Vector3i o)
		{
			this.x = o.x;
			this.y = o.y;
			this.z = o.z;
		}

		public void Set(int fX, int fY, int fZ)
		{
			this.x = fX;
			this.y = fY;
			this.z = fZ;
		}

		public void Add(Vector3i o)
		{
			this.x += o.x;
			this.y += o.y;
			this.z += o.z;
		}

		public void Subtract(Vector3i o)
		{
			this.x -= o.x;
			this.y -= o.y;
			this.z -= o.z;
		}

		public void Add(int s)
		{
			this.x += s;
			this.y += s;
			this.z += s;
		}

		public int LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}

		public static Vector3i operator -(Vector3i v)
		{
			return new Vector3i(-v.x, -v.y, -v.z);
		}

		public static Vector3i operator *(int f, Vector3i v)
		{
			return new Vector3i(f * v.x, f * v.y, f * v.z);
		}

		public static Vector3i operator *(Vector3i v, int f)
		{
			return new Vector3i(f * v.x, f * v.y, f * v.z);
		}

		public static Vector3i operator /(Vector3i v, int f)
		{
			return new Vector3i(v.x / f, v.y / f, v.z / f);
		}

		public static Vector3i operator /(int f, Vector3i v)
		{
			return new Vector3i(f / v.x, f / v.y, f / v.z);
		}

		public static Vector3i operator *(Vector3i a, Vector3i b)
		{
			return new Vector3i(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3i operator /(Vector3i a, Vector3i b)
		{
			return new Vector3i(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static Vector3i operator +(Vector3i v0, Vector3i v1)
		{
			return new Vector3i(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
		}

		public static Vector3i operator +(Vector3i v0, int f)
		{
			return new Vector3i(v0.x + f, v0.y + f, v0.z + f);
		}

		public static Vector3i operator -(Vector3i v0, Vector3i v1)
		{
			return new Vector3i(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
		}

		public static Vector3i operator -(Vector3i v0, int f)
		{
			return new Vector3i(v0.x - f, v0.y - f, v0.z - f);
		}

		public static bool operator ==(Vector3i a, Vector3i b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Vector3i a, Vector3i b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector3i)obj;
		}

		public override int GetHashCode()
		{
			return ((-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode()) * 16777619 ^ this.z.GetHashCode();
		}

		public int CompareTo(Vector3i other)
		{
			if (this.x != other.x)
			{
				if (this.x >= other.x)
				{
					return 1;
				}
				return -1;
			}
			else if (this.y != other.y)
			{
				if (this.y >= other.y)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (this.z == other.z)
				{
					return 0;
				}
				if (this.z >= other.z)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(Vector3i other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", this.x, this.y, this.z);
		}

		public static implicit operator Vector3i(Index3i v)
		{
			return new Vector3i(v.a, v.b, v.c);
		}

		public static implicit operator Index3i(Vector3i v)
		{
			return new Index3i(v.x, v.y, v.z);
		}

		public static implicit operator Vector3i(int3 v)
		{
			return new Vector3i(v.x, v.y, v.z);
		}

		public static implicit operator int3(Vector3i v)
		{
			return new int3(v.x, v.y, v.z);
		}

		public static explicit operator Vector3i(Vector3f v)
		{
			return new Vector3i((int)v.x, (int)v.y, (int)v.z);
		}

		public static explicit operator Vector3f(Vector3i v)
		{
			return new Vector3f((float)v.x, (float)v.y, (float)v.z);
		}

		public static explicit operator Vector3i(Vector3d v)
		{
			return new Vector3i((int)v.x, (int)v.y, (int)v.z);
		}

		public static explicit operator Vector3d(Vector3i v)
		{
			return new Vector3d((double)v.x, (double)v.y, (double)v.z);
		}

		public int x;

		public int y;

		public int z;

		public static readonly Vector3i Zero = new Vector3i(0, 0, 0);

		public static readonly Vector3i One = new Vector3i(1, 1, 1);

		public static readonly Vector3i AxisX = new Vector3i(1, 0, 0);

		public static readonly Vector3i AxisY = new Vector3i(0, 1, 0);

		public static readonly Vector3i AxisZ = new Vector3i(0, 0, 1);
	}
}
