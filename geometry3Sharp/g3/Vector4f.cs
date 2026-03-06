using System;
using Unity.Mathematics;
using UnityEngine;

namespace g3
{
	public struct Vector4f : IComparable<Vector4f>, IEquatable<Vector4f>
	{
		public Vector4f(float f)
		{
			this.w = f;
			this.z = f;
			this.y = f;
			this.x = f;
		}

		public Vector4f(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4f(float[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
			this.z = v2[2];
			this.w = v2[3];
		}

		public Vector4f(Vector4f copy)
		{
			this.x = copy.x;
			this.y = copy.y;
			this.z = copy.z;
			this.w = copy.w;
		}

		public float this[int key]
		{
			get
			{
				if (key >= 2)
				{
					if (key != 2)
					{
						return this.w;
					}
					return this.z;
				}
				else
				{
					if (key != 0)
					{
						return this.y;
					}
					return this.x;
				}
			}
			set
			{
				if (key < 2)
				{
					if (key == 0)
					{
						this.x = value;
						return;
					}
					this.y = value;
					return;
				}
				else
				{
					if (key == 2)
					{
						this.z = value;
						return;
					}
					this.w = value;
					return;
				}
			}
		}

		public float LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
			}
		}

		public float Length
		{
			get
			{
				return (float)Math.Sqrt((double)this.LengthSquared);
			}
		}

		public float LengthL1
		{
			get
			{
				return Math.Abs(this.x) + Math.Abs(this.y) + Math.Abs(this.z) + Math.Abs(this.w);
			}
		}

		public float Normalize(float epsilon = 1.1920929E-07f)
		{
			float num = this.Length;
			if (num > epsilon)
			{
				float num2 = 1f / num;
				this.x *= num2;
				this.y *= num2;
				this.z *= num2;
				this.w *= num2;
			}
			else
			{
				num = 0f;
				this.x = (this.y = (this.z = (this.w = 0f)));
			}
			return num;
		}

		public Vector4f Normalized
		{
			get
			{
				float length = this.Length;
				if ((double)length > 2.220446049250313E-16)
				{
					float num = 1f / length;
					return new Vector4f(this.x * num, this.y * num, this.z * num, this.w * num);
				}
				return Vector4f.Zero;
			}
		}

		public bool IsNormalized
		{
			get
			{
				return (double)Math.Abs(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w - 1f) < 1E-08;
			}
		}

		public bool IsFinite
		{
			get
			{
				float f = this.x + this.y + this.z + this.w;
				return !float.IsNaN(f) && !float.IsInfinity(f);
			}
		}

		public void Round(int nDecimals)
		{
			this.x = (float)Math.Round((double)this.x, nDecimals);
			this.y = (float)Math.Round((double)this.y, nDecimals);
			this.z = (float)Math.Round((double)this.z, nDecimals);
			this.w = (float)Math.Round((double)this.w, nDecimals);
		}

		public float Dot(Vector4f v2)
		{
			return this.x * v2.x + this.y * v2.y + this.z * v2.z + this.w * v2.w;
		}

		public float Dot(ref Vector4f v2)
		{
			return this.x * v2.x + this.y * v2.y + this.z * v2.z + this.w * v2.w;
		}

		public static float Dot(Vector4f v1, Vector4f v2)
		{
			return v1.Dot(v2);
		}

		public float AngleD(Vector4f v2)
		{
			return (float)Math.Acos((double)MathUtil.Clamp(this.Dot(v2), -1f, 1f)) * 57.29578f;
		}

		public static float AngleD(Vector4f v1, Vector4f v2)
		{
			return v1.AngleD(v2);
		}

		public float AngleR(Vector4f v2)
		{
			return (float)Math.Acos((double)MathUtil.Clamp(this.Dot(v2), -1f, 1f));
		}

		public static float AngleR(Vector4f v1, Vector4f v2)
		{
			return v1.AngleR(v2);
		}

		public float DistanceSquared(Vector4f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			float num3 = v2.z - this.z;
			float num4 = v2.w - this.w;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4;
		}

		public float DistanceSquared(ref Vector4f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			float num3 = v2.z - this.z;
			float num4 = v2.w - this.w;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4;
		}

		public float Distance(Vector4f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			float num3 = v2.z - this.z;
			float num4 = v2.w - this.w;
			return (float)Math.Sqrt((double)(num * num + num2 * num2 + num3 * num3 + num4 * num4));
		}

		public float Distance(ref Vector4f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			float num3 = v2.z - this.z;
			float num4 = v2.w - this.w;
			return (float)Math.Sqrt((double)(num * num + num2 * num2 + num3 * num3 + num4 * num4));
		}

		public static Vector4f operator -(Vector4f v)
		{
			return new Vector4f(-v.x, -v.y, -v.z, -v.w);
		}

		public static Vector4f operator *(float f, Vector4f v)
		{
			return new Vector4f(f * v.x, f * v.y, f * v.z, f * v.w);
		}

		public static Vector4f operator *(Vector4f v, float f)
		{
			return new Vector4f(f * v.x, f * v.y, f * v.z, f * v.w);
		}

		public static Vector4f operator /(Vector4f v, float f)
		{
			return new Vector4f(v.x / f, v.y / f, v.z / f, v.w / f);
		}

		public static Vector4f operator /(float f, Vector4f v)
		{
			return new Vector4f(f / v.x, f / v.y, f / v.z, f / v.w);
		}

		public static Vector4f operator *(Vector4f a, Vector4f b)
		{
			return new Vector4f(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		public static Vector4f operator /(Vector4f a, Vector4f b)
		{
			return new Vector4f(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}

		public static Vector4f operator +(Vector4f v0, Vector4f v1)
		{
			return new Vector4f(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z, v0.w + v1.w);
		}

		public static Vector4f operator +(Vector4f v0, float f)
		{
			return new Vector4f(v0.x + f, v0.y + f, v0.z + f, v0.w + f);
		}

		public static Vector4f operator -(Vector4f v0, Vector4f v1)
		{
			return new Vector4f(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z, v0.w - v1.w);
		}

		public static Vector4f operator -(Vector4f v0, float f)
		{
			return new Vector4f(v0.x - f, v0.y - f, v0.z - f, v0.w - f);
		}

		public static bool operator ==(Vector4f a, Vector4f b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		public static bool operator !=(Vector4f a, Vector4f b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector4f)obj;
		}

		public override int GetHashCode()
		{
			return (((-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode()) * 16777619 ^ this.z.GetHashCode()) * 16777619 ^ this.w.GetHashCode();
		}

		public int CompareTo(Vector4f other)
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
			else if (this.z != other.z)
			{
				if (this.z >= other.z)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (this.w == other.w)
				{
					return 0;
				}
				if (this.w >= other.w)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(Vector4f other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w;
		}

		public bool EpsilonEqual(Vector4f v2, float epsilon)
		{
			return Math.Abs(this.x - v2.x) <= epsilon && Math.Abs(this.y - v2.y) <= epsilon && Math.Abs(this.z - v2.z) <= epsilon && Math.Abs(this.w - v2.w) <= epsilon;
		}

		public override string ToString()
		{
			return string.Format("{0:F8} {1:F8} {2:F8} {3:F8}", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.w
			});
		}

		public string ToString(string fmt)
		{
			return string.Format("{0} {1} {2} {3}", new object[]
			{
				this.x.ToString(fmt),
				this.y.ToString(fmt),
				this.z.ToString(fmt),
				this.w.ToString(fmt)
			});
		}

		public static implicit operator Vector4f(Vector4 v)
		{
			return new Vector4f(v.x, v.y, v.z, v.w);
		}

		public static implicit operator Vector4(Vector4f v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		public static implicit operator Color(Vector4f v)
		{
			return new Color(v.x, v.y, v.z, v.w);
		}

		public static implicit operator Vector4f(Color c)
		{
			return new Vector4f(c.r, c.g, c.b, c.a);
		}

		public static implicit operator Vector4f(float4 v)
		{
			return new Vector4f(v.x, v.y, v.z, v.w);
		}

		public static implicit operator float4(Vector4f v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		public static implicit operator double4(Vector4f v)
		{
			return new double4((double)v.x, (double)v.y, (double)v.z, (double)v.w);
		}

		public static explicit operator Vector4f(double4 v)
		{
			return new Vector4f((float)v.x, (float)v.y, (float)v.z, (float)v.w);
		}

		public float x;

		public float y;

		public float z;

		public float w;

		public static readonly Vector4f Zero = new Vector4f(0f, 0f, 0f, 0f);

		public static readonly Vector4f One = new Vector4f(1f, 1f, 1f, 1f);
	}
}
