using System;
using Unity.Mathematics;
using UnityEngine;

namespace g3
{
	public struct Vector4d : IComparable<Vector4d>, IEquatable<Vector4d>
	{
		public Vector4d(double f)
		{
			this.w = f;
			this.z = f;
			this.y = f;
			this.x = f;
		}

		public Vector4d(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4d(double[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
			this.z = v2[2];
			this.w = v2[3];
		}

		public Vector4d(Vector4d copy)
		{
			this.x = copy.x;
			this.y = copy.y;
			this.z = copy.z;
			this.w = copy.w;
		}

		public double this[int key]
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

		public double LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
			}
		}

		public double Length
		{
			get
			{
				return Math.Sqrt(this.LengthSquared);
			}
		}

		public double LengthL1
		{
			get
			{
				return Math.Abs(this.x) + Math.Abs(this.y) + Math.Abs(this.z) + Math.Abs(this.w);
			}
		}

		public double Normalize(double epsilon = 2.220446049250313E-16)
		{
			double num = this.Length;
			if (num > epsilon)
			{
				double num2 = 1.0 / num;
				this.x *= num2;
				this.y *= num2;
				this.z *= num2;
				this.w *= num2;
			}
			else
			{
				num = 0.0;
				this.x = (this.y = (this.z = (this.w = 0.0)));
			}
			return num;
		}

		public Vector4d Normalized
		{
			get
			{
				double length = this.Length;
				if (length > 2.220446049250313E-16)
				{
					double num = 1.0 / length;
					return new Vector4d(this.x * num, this.y * num, this.z * num, this.w * num);
				}
				return Vector4d.Zero;
			}
		}

		public bool IsNormalized
		{
			get
			{
				return Math.Abs(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w - 1.0) < 1E-08;
			}
		}

		public bool IsFinite
		{
			get
			{
				double d = this.x + this.y + this.z + this.w;
				return !double.IsNaN(d) && !double.IsInfinity(d);
			}
		}

		public void Round(int nDecimals)
		{
			this.x = Math.Round(this.x, nDecimals);
			this.y = Math.Round(this.y, nDecimals);
			this.z = Math.Round(this.z, nDecimals);
			this.w = Math.Round(this.w, nDecimals);
		}

		public double Dot(Vector4d v2)
		{
			return this.x * v2.x + this.y * v2.y + this.z * v2.z + this.w * v2.w;
		}

		public double Dot(ref Vector4d v2)
		{
			return this.x * v2.x + this.y * v2.y + this.z * v2.z + this.w * v2.w;
		}

		public static double Dot(Vector4d v1, Vector4d v2)
		{
			return v1.Dot(v2);
		}

		public double AngleD(Vector4d v2)
		{
			return Math.Acos(MathUtil.Clamp(this.Dot(v2), -1.0, 1.0)) * 57.29577951308232;
		}

		public static double AngleD(Vector4d v1, Vector4d v2)
		{
			return v1.AngleD(v2);
		}

		public double AngleR(Vector4d v2)
		{
			return Math.Acos(MathUtil.Clamp(this.Dot(v2), -1.0, 1.0));
		}

		public static double AngleR(Vector4d v1, Vector4d v2)
		{
			return v1.AngleR(v2);
		}

		public double DistanceSquared(Vector4d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			double num4 = v2.w - this.w;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4;
		}

		public double DistanceSquared(ref Vector4d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			double num4 = v2.w - this.w;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4;
		}

		public double Distance(Vector4d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			double num4 = v2.w - this.w;
			return Math.Sqrt(num * num + num2 * num2 + num3 * num3 + num4 * num4);
		}

		public double Distance(ref Vector4d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			double num4 = v2.w - this.w;
			return Math.Sqrt(num * num + num2 * num2 + num3 * num3 + num4 * num4);
		}

		public static Vector4d operator -(Vector4d v)
		{
			return new Vector4d(-v.x, -v.y, -v.z, -v.w);
		}

		public static Vector4d operator *(double f, Vector4d v)
		{
			return new Vector4d(f * v.x, f * v.y, f * v.z, f * v.w);
		}

		public static Vector4d operator *(Vector4d v, double f)
		{
			return new Vector4d(f * v.x, f * v.y, f * v.z, f * v.w);
		}

		public static Vector4d operator /(Vector4d v, double f)
		{
			return new Vector4d(v.x / f, v.y / f, v.z / f, v.w / f);
		}

		public static Vector4d operator /(double f, Vector4d v)
		{
			return new Vector4d(f / v.x, f / v.y, f / v.z, f / v.w);
		}

		public static Vector4d operator *(Vector4d a, Vector4d b)
		{
			return new Vector4d(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		}

		public static Vector4d operator /(Vector4d a, Vector4d b)
		{
			return new Vector4d(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		}

		public static Vector4d operator +(Vector4d v0, Vector4d v1)
		{
			return new Vector4d(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z, v0.w + v1.w);
		}

		public static Vector4d operator +(Vector4d v0, double f)
		{
			return new Vector4d(v0.x + f, v0.y + f, v0.z + f, v0.w + f);
		}

		public static Vector4d operator -(Vector4d v0, Vector4d v1)
		{
			return new Vector4d(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z, v0.w - v1.w);
		}

		public static Vector4d operator -(Vector4d v0, double f)
		{
			return new Vector4d(v0.x - f, v0.y - f, v0.z - f, v0.w - f);
		}

		public static bool operator ==(Vector4d a, Vector4d b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		public static bool operator !=(Vector4d a, Vector4d b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector4d)obj;
		}

		public override int GetHashCode()
		{
			return (((-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode()) * 16777619 ^ this.z.GetHashCode()) * 16777619 ^ this.w.GetHashCode();
		}

		public int CompareTo(Vector4d other)
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

		public bool Equals(Vector4d other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w;
		}

		public bool EpsilonEqual(Vector4d v2, double epsilon)
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

		public static implicit operator Vector4d(Vector4f v)
		{
			return new Vector4d((double)v.x, (double)v.y, (double)v.z, (double)v.w);
		}

		public static explicit operator Vector4f(Vector4d v)
		{
			return new Vector4f((float)v.x, (float)v.y, (float)v.z, (float)v.w);
		}

		public static implicit operator Vector4d(Vector4 v)
		{
			return new Vector4d((double)v.x, (double)v.y, (double)v.z, (double)v.w);
		}

		public static explicit operator Vector4(Vector4d v)
		{
			return new Vector4((float)v.x, (float)v.y, (float)v.z, (float)v.w);
		}

		public static implicit operator Vector4d(float4 v)
		{
			return new Vector4d((double)v.x, (double)v.y, (double)v.z, (double)v.w);
		}

		public static explicit operator float4(Vector4d v)
		{
			return new float4((float)v.x, (float)v.y, (float)v.z, (float)v.w);
		}

		public static implicit operator Vector4d(double4 v)
		{
			return new Vector4d(v.x, v.y, v.z, v.w);
		}

		public static implicit operator double4(Vector4d v)
		{
			return new double4((double)((float)v.x), (double)((float)v.y), (double)((float)v.z), (double)((float)v.w));
		}

		public double x;

		public double y;

		public double z;

		public double w;

		public static readonly Vector4d Zero = new Vector4d(0.0, 0.0, 0.0, 0.0);

		public static readonly Vector4d One = new Vector4d(1.0, 1.0, 1.0, 1.0);
	}
}
