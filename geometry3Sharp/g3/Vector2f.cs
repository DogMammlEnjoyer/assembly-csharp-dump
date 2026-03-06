using System;
using Unity.Mathematics;
using UnityEngine;

namespace g3
{
	public struct Vector2f : IComparable<Vector2f>, IEquatable<Vector2f>
	{
		public Vector2f(float f)
		{
			this.y = f;
			this.x = f;
		}

		public Vector2f(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2f(float[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
		}

		public Vector2f(double f)
		{
			this.x = (this.y = (float)f);
		}

		public Vector2f(double x, double y)
		{
			this.x = (float)x;
			this.y = (float)y;
		}

		public Vector2f(double[] v2)
		{
			this.x = (float)v2[0];
			this.y = (float)v2[1];
		}

		public Vector2f(Vector2f copy)
		{
			this.x = copy[0];
			this.y = copy[1];
		}

		public Vector2f(Vector2d copy)
		{
			this.x = (float)copy[0];
			this.y = (float)copy[1];
		}

		public float this[int key]
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

		public float LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		public float Length
		{
			get
			{
				return (float)Math.Sqrt((double)this.LengthSquared);
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
			}
			else
			{
				num = 0f;
				this.x = (this.y = 0f);
			}
			return num;
		}

		public Vector2f Normalized
		{
			get
			{
				float length = this.Length;
				if (length > 1.1920929E-07f)
				{
					float num = 1f / length;
					return new Vector2f(this.x * num, this.y * num);
				}
				return Vector2f.Zero;
			}
		}

		public bool IsNormalized
		{
			get
			{
				return Math.Abs(this.x * this.x + this.y * this.y - 1f) < 1E-06f;
			}
		}

		public bool IsFinite
		{
			get
			{
				float f = this.x + this.y;
				return !float.IsNaN(f) && !float.IsInfinity(f);
			}
		}

		public void Round(int nDecimals)
		{
			this.x = (float)Math.Round((double)this.x, nDecimals);
			this.y = (float)Math.Round((double)this.y, nDecimals);
		}

		public float Dot(Vector2f v2)
		{
			return this.x * v2.x + this.y * v2.y;
		}

		public float Cross(Vector2f v2)
		{
			return this.x * v2.y - this.y * v2.x;
		}

		public Vector2f Perp
		{
			get
			{
				return new Vector2f(this.y, -this.x);
			}
		}

		public Vector2f UnitPerp
		{
			get
			{
				return new Vector2f(this.y, -this.x).Normalized;
			}
		}

		public float DotPerp(Vector2f v2)
		{
			return this.x * v2.y - this.y * v2.x;
		}

		public float AngleD(Vector2f v2)
		{
			return (float)(Math.Acos((double)MathUtil.Clamp(this.Dot(v2), -1f, 1f)) * 57.29577951308232);
		}

		public static float AngleD(Vector2f v1, Vector2f v2)
		{
			return v1.AngleD(v2);
		}

		public float AngleR(Vector2f v2)
		{
			return (float)Math.Acos((double)MathUtil.Clamp(this.Dot(v2), -1f, 1f));
		}

		public static float AngleR(Vector2f v1, Vector2f v2)
		{
			return v1.AngleR(v2);
		}

		public float DistanceSquared(Vector2f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			return num * num + num2 * num2;
		}

		public float Distance(Vector2f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			return (float)Math.Sqrt((double)(num * num + num2 * num2));
		}

		public void Set(Vector2f o)
		{
			this.x = o.x;
			this.y = o.y;
		}

		public void Set(float fX, float fY)
		{
			this.x = fX;
			this.y = fY;
		}

		public void Add(Vector2f o)
		{
			this.x += o.x;
			this.y += o.y;
		}

		public void Subtract(Vector2f o)
		{
			this.x -= o.x;
			this.y -= o.y;
		}

		public static Vector2f operator -(Vector2f v)
		{
			return new Vector2f(-v.x, -v.y);
		}

		public static Vector2f operator +(Vector2f a, Vector2f o)
		{
			return new Vector2f(a.x + o.x, a.y + o.y);
		}

		public static Vector2f operator +(Vector2f a, float f)
		{
			return new Vector2f(a.x + f, a.y + f);
		}

		public static Vector2f operator -(Vector2f a, Vector2f o)
		{
			return new Vector2f(a.x - o.x, a.y - o.y);
		}

		public static Vector2f operator -(Vector2f a, float f)
		{
			return new Vector2f(a.x - f, a.y - f);
		}

		public static Vector2f operator *(Vector2f a, float f)
		{
			return new Vector2f(a.x * f, a.y * f);
		}

		public static Vector2f operator *(float f, Vector2f a)
		{
			return new Vector2f(a.x * f, a.y * f);
		}

		public static Vector2f operator /(Vector2f v, float f)
		{
			return new Vector2f(v.x / f, v.y / f);
		}

		public static Vector2f operator /(float f, Vector2f v)
		{
			return new Vector2f(f / v.x, f / v.y);
		}

		public static Vector2f operator *(Vector2f a, Vector2f b)
		{
			return new Vector2f(a.x * b.x, a.y * b.y);
		}

		public static Vector2f operator /(Vector2f a, Vector2f b)
		{
			return new Vector2f(a.x / b.x, a.y / b.y);
		}

		public static bool operator ==(Vector2f a, Vector2f b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2f a, Vector2f b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector2f)obj;
		}

		public override int GetHashCode()
		{
			return (-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode();
		}

		public int CompareTo(Vector2f other)
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

		public bool Equals(Vector2f other)
		{
			return this.x == other.x && this.y == other.y;
		}

		public bool EpsilonEqual(Vector2f v2, float epsilon)
		{
			return Math.Abs(this.x - v2.x) <= epsilon && Math.Abs(this.y - v2.y) <= epsilon;
		}

		public static Vector2f Lerp(Vector2f a, Vector2f b, float t)
		{
			float num = 1f - t;
			return new Vector2f(num * a.x + t * b.x, num * a.y + t * b.y);
		}

		public static Vector2f Lerp(ref Vector2f a, ref Vector2f b, float t)
		{
			float num = 1f - t;
			return new Vector2f(num * a.x + t * b.x, num * a.y + t * b.y);
		}

		public override string ToString()
		{
			return string.Format("{0:F8} {1:F8}", this.x, this.y);
		}

		public static implicit operator Vector2f(Vector2 v)
		{
			return new Vector2f(v.x, v.y);
		}

		public static implicit operator Vector2(Vector2f v)
		{
			return new Vector2(v.x, v.y);
		}

		public static implicit operator Vector2f(float2 v)
		{
			return new Vector2f(v.x, v.y);
		}

		public static implicit operator float2(Vector2f v)
		{
			return new float2(v.x, v.y);
		}

		public static explicit operator Vector2f(double2 v)
		{
			return new Vector2f((float)v.x, (float)v.y);
		}

		public static implicit operator double2(Vector2f v)
		{
			return new double2((double)v.x, (double)v.y);
		}

		public float x;

		public float y;

		public static readonly Vector2f Zero = new Vector2f(0f, 0f);

		public static readonly Vector2f One = new Vector2f(1f, 1f);

		public static readonly Vector2f AxisX = new Vector2f(1f, 0f);

		public static readonly Vector2f AxisY = new Vector2f(0f, 1f);

		public static readonly Vector2f MaxValue = new Vector2f(float.MaxValue, float.MaxValue);

		public static readonly Vector2f MinValue = new Vector2f(float.MinValue, float.MinValue);
	}
}
