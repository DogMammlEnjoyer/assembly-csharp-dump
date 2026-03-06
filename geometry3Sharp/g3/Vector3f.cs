using System;
using Unity.Mathematics;
using UnityEngine;

namespace g3
{
	public struct Vector3f : IComparable<Vector3f>, IEquatable<Vector3f>
	{
		public Vector3f(float f)
		{
			this.z = f;
			this.y = f;
			this.x = f;
		}

		public Vector3f(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3f(float[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
			this.z = v2[2];
		}

		public Vector3f(Vector3f copy)
		{
			this.x = copy.x;
			this.y = copy.y;
			this.z = copy.z;
		}

		public Vector3f(double f)
		{
			this.x = (this.y = (this.z = (float)f));
		}

		public Vector3f(double x, double y, double z)
		{
			this.x = (float)x;
			this.y = (float)y;
			this.z = (float)z;
		}

		public Vector3f(double[] v2)
		{
			this.x = (float)v2[0];
			this.y = (float)v2[1];
			this.z = (float)v2[2];
		}

		public Vector3f(Vector3d copy)
		{
			this.x = (float)copy.x;
			this.y = (float)copy.y;
			this.z = (float)copy.z;
		}

		public float this[int key]
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

		public Vector2f xy
		{
			get
			{
				return new Vector2f(this.x, this.y);
			}
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		public Vector2f xz
		{
			get
			{
				return new Vector2f(this.x, this.z);
			}
			set
			{
				this.x = value.x;
				this.z = value.y;
			}
		}

		public Vector2f yz
		{
			get
			{
				return new Vector2f(this.y, this.z);
			}
			set
			{
				this.y = value.x;
				this.z = value.y;
			}
		}

		public float LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
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
				return Math.Abs(this.x) + Math.Abs(this.y) + Math.Abs(this.z);
			}
		}

		public float Max
		{
			get
			{
				return Math.Max(this.x, Math.Max(this.y, this.z));
			}
		}

		public float Min
		{
			get
			{
				return Math.Min(this.x, Math.Min(this.y, this.z));
			}
		}

		public float MaxAbs
		{
			get
			{
				return Math.Max(Math.Abs(this.x), Math.Max(Math.Abs(this.y), Math.Abs(this.z)));
			}
		}

		public float MinAbs
		{
			get
			{
				return Math.Min(Math.Abs(this.x), Math.Min(Math.Abs(this.y), Math.Abs(this.z)));
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
			}
			else
			{
				num = 0f;
				this.x = (this.y = (this.z = 0f));
			}
			return num;
		}

		public Vector3f Normalized
		{
			get
			{
				float length = this.Length;
				if (length > 1.1920929E-07f)
				{
					float num = 1f / length;
					return new Vector3f(this.x * num, this.y * num, this.z * num);
				}
				return Vector3f.Zero;
			}
		}

		public bool IsNormalized
		{
			get
			{
				return Math.Abs(this.x * this.x + this.y * this.y + this.z * this.z - 1f) < 1E-06f;
			}
		}

		public bool IsFinite
		{
			get
			{
				float f = this.x + this.y + this.z;
				return !float.IsNaN(f) && !float.IsInfinity(f);
			}
		}

		public void Round(int nDecimals)
		{
			this.x = (float)Math.Round((double)this.x, nDecimals);
			this.y = (float)Math.Round((double)this.y, nDecimals);
			this.z = (float)Math.Round((double)this.z, nDecimals);
		}

		public float Dot(Vector3f v2)
		{
			return this.x * v2[0] + this.y * v2[1] + this.z * v2[2];
		}

		public static float Dot(Vector3f v1, Vector3f v2)
		{
			return v1.Dot(v2);
		}

		public Vector3f Cross(Vector3f v2)
		{
			return new Vector3f(this.y * v2.z - this.z * v2.y, this.z * v2.x - this.x * v2.z, this.x * v2.y - this.y * v2.x);
		}

		public static Vector3f Cross(Vector3f v1, Vector3f v2)
		{
			return v1.Cross(v2);
		}

		public Vector3f UnitCross(Vector3f v2)
		{
			Vector3f result = new Vector3f(this.y * v2.z - this.z * v2.y, this.z * v2.x - this.x * v2.z, this.x * v2.y - this.y * v2.x);
			result.Normalize(1.1920929E-07f);
			return result;
		}

		public float AngleD(Vector3f v2)
		{
			return (float)(Math.Acos((double)MathUtil.Clamp(this.Dot(v2), -1f, 1f)) * 57.29577951308232);
		}

		public static float AngleD(Vector3f v1, Vector3f v2)
		{
			return v1.AngleD(v2);
		}

		public float AngleR(Vector3f v2)
		{
			return (float)Math.Acos((double)MathUtil.Clamp(this.Dot(v2), -1f, 1f));
		}

		public static float AngleR(Vector3f v1, Vector3f v2)
		{
			return v1.AngleR(v2);
		}

		public float DistanceSquared(Vector3f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			float num3 = v2.z - this.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		public float Distance(Vector3f v2)
		{
			float num = v2.x - this.x;
			float num2 = v2.y - this.y;
			float num3 = v2.z - this.z;
			return (float)Math.Sqrt((double)(num * num + num2 * num2 + num3 * num3));
		}

		public void Set(Vector3f o)
		{
			this.x = o[0];
			this.y = o[1];
			this.z = o[2];
		}

		public void Set(float fX, float fY, float fZ)
		{
			this.x = fX;
			this.y = fY;
			this.z = fZ;
		}

		public void Add(Vector3f o)
		{
			this.x += o[0];
			this.y += o[1];
			this.z += o[2];
		}

		public void Subtract(Vector3f o)
		{
			this.x -= o[0];
			this.y -= o[1];
			this.z -= o[2];
		}

		public static Vector3f operator -(Vector3f v)
		{
			return new Vector3f(-v.x, -v.y, -v.z);
		}

		public static Vector3f operator *(float f, Vector3f v)
		{
			return new Vector3f(f * v.x, f * v.y, f * v.z);
		}

		public static Vector3f operator *(Vector3f v, float f)
		{
			return new Vector3f(f * v.x, f * v.y, f * v.z);
		}

		public static Vector3f operator /(Vector3f v, float f)
		{
			return new Vector3f(v.x / f, v.y / f, v.z / f);
		}

		public static Vector3f operator /(float f, Vector3f v)
		{
			return new Vector3f(f / v.x, f / v.y, f / v.z);
		}

		public static Vector3f operator *(Vector3f a, Vector3f b)
		{
			return new Vector3f(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3f operator /(Vector3f a, Vector3f b)
		{
			return new Vector3f(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static Vector3f operator +(Vector3f v0, Vector3f v1)
		{
			return new Vector3f(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
		}

		public static Vector3f operator +(Vector3f v0, float f)
		{
			return new Vector3f(v0.x + f, v0.y + f, v0.z + f);
		}

		public static Vector3f operator -(Vector3f v0, Vector3f v1)
		{
			return new Vector3f(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
		}

		public static Vector3f operator -(Vector3f v0, float f)
		{
			return new Vector3f(v0.x - f, v0.y - f, v0.z - f);
		}

		public static bool operator ==(Vector3f a, Vector3f b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Vector3f a, Vector3f b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector3f)obj;
		}

		public override int GetHashCode()
		{
			return ((-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode()) * 16777619 ^ this.z.GetHashCode();
		}

		public int CompareTo(Vector3f other)
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

		public bool Equals(Vector3f other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z;
		}

		public bool EpsilonEqual(Vector3f v2, float epsilon)
		{
			return Math.Abs(this.x - v2.x) <= epsilon && Math.Abs(this.y - v2.y) <= epsilon && Math.Abs(this.z - v2.z) <= epsilon;
		}

		public static Vector3f Lerp(Vector3f a, Vector3f b, float t)
		{
			float num = 1f - t;
			return new Vector3f(num * a.x + t * b.x, num * a.y + t * b.y, num * a.z + t * b.z);
		}

		public override string ToString()
		{
			return string.Format("{0:F8} {1:F8} {2:F8}", this.x, this.y, this.z);
		}

		public string ToString(string fmt)
		{
			return string.Format("{0} {1} {2}", this.x.ToString(fmt), this.y.ToString(fmt), this.z.ToString(fmt));
		}

		public static implicit operator Vector3f(Vector3 v)
		{
			return new Vector3f(v.x, v.y, v.z);
		}

		public static implicit operator Vector3(Vector3f v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static explicit operator Vector3f(double3 v)
		{
			return new Vector3f((float)v.x, (float)v.y, (float)v.z);
		}

		public static implicit operator double3(Vector3f v)
		{
			return new double3((double)v.x, (double)v.y, (double)v.z);
		}

		public static implicit operator Vector3f(float3 v)
		{
			return new Vector3f(v.x, v.y, v.z);
		}

		public static implicit operator float3(Vector3f v)
		{
			return new float3(v.x, v.y, v.z);
		}

		public static implicit operator Color(Vector3f v)
		{
			return new Color(v.x, v.y, v.z, 1f);
		}

		public static implicit operator Vector3f(Color c)
		{
			return new Vector3f(c.r, c.g, c.b);
		}

		public float x;

		public float y;

		public float z;

		public static readonly Vector3f Zero = new Vector3f(0f, 0f, 0f);

		public static readonly Vector3f One = new Vector3f(1f, 1f, 1f);

		public static readonly Vector3f OneNormalized = new Vector3f(1f, 1f, 1f).Normalized;

		public static readonly Vector3f Invalid = new Vector3f(float.MaxValue, float.MaxValue, float.MaxValue);

		public static readonly Vector3f AxisX = new Vector3f(1f, 0f, 0f);

		public static readonly Vector3f AxisY = new Vector3f(0f, 1f, 0f);

		public static readonly Vector3f AxisZ = new Vector3f(0f, 0f, 1f);

		public static readonly Vector3f MaxValue = new Vector3f(float.MaxValue, float.MaxValue, float.MaxValue);

		public static readonly Vector3f MinValue = new Vector3f(float.MinValue, float.MinValue, float.MinValue);
	}
}
