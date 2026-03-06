using System;
using Unity.Mathematics;
using UnityEngine;

namespace g3
{
	public struct Vector3d : IComparable<Vector3d>, IEquatable<Vector3d>
	{
		public Vector3d(double f)
		{
			this.z = f;
			this.y = f;
			this.x = f;
		}

		public Vector3d(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3d(double[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
			this.z = v2[2];
		}

		public Vector3d(Vector3d copy)
		{
			this.x = copy.x;
			this.y = copy.y;
			this.z = copy.z;
		}

		public Vector3d(Vector3f copy)
		{
			this.x = (double)copy.x;
			this.y = (double)copy.y;
			this.z = (double)copy.z;
		}

		public double this[int key]
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

		public Vector2d xy
		{
			get
			{
				return new Vector2d(this.x, this.y);
			}
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		public Vector2d xz
		{
			get
			{
				return new Vector2d(this.x, this.z);
			}
			set
			{
				this.x = value.x;
				this.z = value.y;
			}
		}

		public Vector2d yz
		{
			get
			{
				return new Vector2d(this.y, this.z);
			}
			set
			{
				this.y = value.x;
				this.z = value.y;
			}
		}

		public double LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
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
				return Math.Abs(this.x) + Math.Abs(this.y) + Math.Abs(this.z);
			}
		}

		public double Max
		{
			get
			{
				return Math.Max(this.x, Math.Max(this.y, this.z));
			}
		}

		public double Min
		{
			get
			{
				return Math.Min(this.x, Math.Min(this.y, this.z));
			}
		}

		public double MaxAbs
		{
			get
			{
				return Math.Max(Math.Abs(this.x), Math.Max(Math.Abs(this.y), Math.Abs(this.z)));
			}
		}

		public double MinAbs
		{
			get
			{
				return Math.Min(Math.Abs(this.x), Math.Min(Math.Abs(this.y), Math.Abs(this.z)));
			}
		}

		public Vector3d Abs
		{
			get
			{
				return new Vector3d(Math.Abs(this.x), Math.Abs(this.y), Math.Abs(this.z));
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
			}
			else
			{
				num = 0.0;
				this.x = (this.y = (this.z = 0.0));
			}
			return num;
		}

		public Vector3d Normalized
		{
			get
			{
				double length = this.Length;
				if (length > 2.220446049250313E-16)
				{
					double num = 1.0 / length;
					return new Vector3d(this.x * num, this.y * num, this.z * num);
				}
				return Vector3d.Zero;
			}
		}

		public bool IsNormalized
		{
			get
			{
				return Math.Abs(this.x * this.x + this.y * this.y + this.z * this.z - 1.0) < 1E-08;
			}
		}

		public bool IsFinite
		{
			get
			{
				double d = this.x + this.y + this.z;
				return !double.IsNaN(d) && !double.IsInfinity(d);
			}
		}

		public void Round(int nDecimals)
		{
			this.x = Math.Round(this.x, nDecimals);
			this.y = Math.Round(this.y, nDecimals);
			this.z = Math.Round(this.z, nDecimals);
		}

		public double Dot(Vector3d v2)
		{
			return this.x * v2.x + this.y * v2.y + this.z * v2.z;
		}

		public double Dot(ref Vector3d v2)
		{
			return this.x * v2.x + this.y * v2.y + this.z * v2.z;
		}

		public static double Dot(Vector3d v1, Vector3d v2)
		{
			return v1.Dot(ref v2);
		}

		public Vector3d Cross(Vector3d v2)
		{
			return new Vector3d(this.y * v2.z - this.z * v2.y, this.z * v2.x - this.x * v2.z, this.x * v2.y - this.y * v2.x);
		}

		public Vector3d Cross(ref Vector3d v2)
		{
			return new Vector3d(this.y * v2.z - this.z * v2.y, this.z * v2.x - this.x * v2.z, this.x * v2.y - this.y * v2.x);
		}

		public static Vector3d Cross(Vector3d v1, Vector3d v2)
		{
			return v1.Cross(ref v2);
		}

		public Vector3d UnitCross(ref Vector3d v2)
		{
			Vector3d result = new Vector3d(this.y * v2.z - this.z * v2.y, this.z * v2.x - this.x * v2.z, this.x * v2.y - this.y * v2.x);
			result.Normalize(2.220446049250313E-16);
			return result;
		}

		public Vector3d UnitCross(Vector3d v2)
		{
			return this.UnitCross(ref v2);
		}

		public double AngleD(Vector3d v2)
		{
			return Math.Acos(MathUtil.Clamp(this.Dot(v2), -1.0, 1.0)) * 57.29577951308232;
		}

		public static double AngleD(Vector3d v1, Vector3d v2)
		{
			return v1.AngleD(v2);
		}

		public double AngleR(Vector3d v2)
		{
			return Math.Acos(MathUtil.Clamp(this.Dot(v2), -1.0, 1.0));
		}

		public static double AngleR(Vector3d v1, Vector3d v2)
		{
			return v1.AngleR(v2);
		}

		public double DistanceSquared(Vector3d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		public double DistanceSquared(ref Vector3d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			return num * num + num2 * num2 + num3 * num3;
		}

		public double Distance(Vector3d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
		}

		public double Distance(ref Vector3d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			double num3 = v2.z - this.z;
			return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
		}

		public void Set(Vector3d o)
		{
			this.x = o.x;
			this.y = o.y;
			this.z = o.z;
		}

		public void Set(double fX, double fY, double fZ)
		{
			this.x = fX;
			this.y = fY;
			this.z = fZ;
		}

		public void Add(Vector3d o)
		{
			this.x += o.x;
			this.y += o.y;
			this.z += o.z;
		}

		public void Subtract(Vector3d o)
		{
			this.x -= o.x;
			this.y -= o.y;
			this.z -= o.z;
		}

		public static Vector3d operator -(Vector3d v)
		{
			return new Vector3d(-v.x, -v.y, -v.z);
		}

		public static Vector3d operator *(double f, Vector3d v)
		{
			return new Vector3d(f * v.x, f * v.y, f * v.z);
		}

		public static Vector3d operator *(Vector3d v, double f)
		{
			return new Vector3d(f * v.x, f * v.y, f * v.z);
		}

		public static Vector3d operator /(Vector3d v, double f)
		{
			return new Vector3d(v.x / f, v.y / f, v.z / f);
		}

		public static Vector3d operator /(double f, Vector3d v)
		{
			return new Vector3d(f / v.x, f / v.y, f / v.z);
		}

		public static Vector3d operator *(Vector3d a, Vector3d b)
		{
			return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3d operator /(Vector3d a, Vector3d b)
		{
			return new Vector3d(a.x / b.x, a.y / b.y, a.z / b.z);
		}

		public static Vector3d operator +(Vector3d v0, Vector3d v1)
		{
			return new Vector3d(v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
		}

		public static Vector3d operator +(Vector3d v0, double f)
		{
			return new Vector3d(v0.x + f, v0.y + f, v0.z + f);
		}

		public static Vector3d operator -(Vector3d v0, Vector3d v1)
		{
			return new Vector3d(v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
		}

		public static Vector3d operator -(Vector3d v0, double f)
		{
			return new Vector3d(v0.x - f, v0.y - f, v0.z - f);
		}

		public static bool operator ==(Vector3d a, Vector3d b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Vector3d a, Vector3d b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector3d)obj;
		}

		public override int GetHashCode()
		{
			return ((-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode()) * 16777619 ^ this.z.GetHashCode();
		}

		public int CompareTo(Vector3d other)
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

		public bool Equals(Vector3d other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z;
		}

		public bool EpsilonEqual(Vector3d v2, double epsilon)
		{
			return Math.Abs(this.x - v2.x) <= epsilon && Math.Abs(this.y - v2.y) <= epsilon && Math.Abs(this.z - v2.z) <= epsilon;
		}

		public static Vector3d Lerp(Vector3d a, Vector3d b, double t)
		{
			double num = 1.0 - t;
			return new Vector3d(num * a.x + t * b.x, num * a.y + t * b.y, num * a.z + t * b.z);
		}

		public static Vector3d Lerp(ref Vector3d a, ref Vector3d b, double t)
		{
			double num = 1.0 - t;
			return new Vector3d(num * a.x + t * b.x, num * a.y + t * b.y, num * a.z + t * b.z);
		}

		public override string ToString()
		{
			return string.Format("{0:F8} {1:F8} {2:F8}", this.x, this.y, this.z);
		}

		public string ToString(string fmt)
		{
			return string.Format("{0} {1} {2}", this.x.ToString(fmt), this.y.ToString(fmt), this.z.ToString(fmt));
		}

		public static implicit operator Vector3d(Vector3f v)
		{
			return new Vector3d((double)v.x, (double)v.y, (double)v.z);
		}

		public static explicit operator Vector3f(Vector3d v)
		{
			return new Vector3f((float)v.x, (float)v.y, (float)v.z);
		}

		public static implicit operator Vector3d(Vector3 v)
		{
			return new Vector3d((double)v.x, (double)v.y, (double)v.z);
		}

		public static explicit operator Vector3(Vector3d v)
		{
			return new Vector3((float)v.x, (float)v.y, (float)v.z);
		}

		public static implicit operator Vector3d(float3 v)
		{
			return new Vector3d((double)v.x, (double)v.y, (double)v.z);
		}

		public static explicit operator float3(Vector3d v)
		{
			return new float3((float)v.x, (float)v.y, (float)v.z);
		}

		public static implicit operator Vector3d(double3 v)
		{
			return new Vector3d(v.x, v.y, v.z);
		}

		public static implicit operator double3(Vector3d v)
		{
			return new double3(v.x, v.y, v.z);
		}

		public static double Orthonormalize(ref Vector3d u, ref Vector3d v, ref Vector3d w)
		{
			double num = u.Normalize(2.220446049250313E-16);
			double f = u.Dot(v);
			v -= f * u;
			double num2 = v.Normalize(2.220446049250313E-16);
			if (num2 < num)
			{
				num = num2;
			}
			double f2 = v.Dot(w);
			f = u.Dot(w);
			w -= f * u + f2 * v;
			num2 = w.Normalize(2.220446049250313E-16);
			if (num2 < num)
			{
				num = num2;
			}
			return num;
		}

		public static void GenerateComplementBasis(ref Vector3d u, ref Vector3d v, Vector3d w)
		{
			double num;
			if (Math.Abs(w.x) >= Math.Abs(w.y))
			{
				num = MathUtil.InvSqrt(w.x * w.x + w.z * w.z);
				u.x = -w.z * num;
				u.y = 0.0;
				u.z = w.x * num;
				v.x = w.y * u.z;
				v.y = w.z * u.x - w.x * u.z;
				v.z = -w.y * u.x;
				return;
			}
			num = MathUtil.InvSqrt(w.y * w.y + w.z * w.z);
			u.x = 0.0;
			u.y = w.z * num;
			u.z = -w.y * num;
			v.x = w.y * u.z - w.z * u.y;
			v.y = -w.x * u.z;
			v.z = w.x * u.y;
		}

		public static double ComputeOrthogonalComplement(int numInputs, Vector3d v0, ref Vector3d v1, ref Vector3d v2)
		{
			if (numInputs == 1)
			{
				if (Math.Abs(v0[0]) > Math.Abs(v0[1]))
				{
					v1 = new Vector3d(-v0[2], 0.0, v0[0]);
				}
				else
				{
					v1 = new Vector3d(0.0, v0[2], -v0[1]);
				}
				numInputs = 2;
			}
			if (numInputs == 2)
			{
				v2 = Vector3d.Cross(v0, v1);
				return Vector3d.Orthonormalize(ref v0, ref v1, ref v2);
			}
			return 0.0;
		}

		public static void MakePerpVectors(ref Vector3d n, out Vector3d b1, out Vector3d b2)
		{
			if (n.z < 0.0)
			{
				double num = 1.0 / (1.0 - n.z);
				double num2 = n.x * n.y * num;
				b1.x = 1.0 - n.x * n.x * num;
				b1.y = -num2;
				b1.z = n.x;
				b2.x = num2;
				b2.y = n.y * n.y * num - 1.0;
				b2.z = -n.y;
				return;
			}
			double num3 = 1.0 / (1.0 + n.z);
			double num4 = -n.x * n.y * num3;
			b1.x = 1.0 - n.x * n.x * num3;
			b1.y = num4;
			b1.z = -n.x;
			b2.x = num4;
			b2.y = 1.0 - n.y * n.y * num3;
			b2.z = -n.y;
		}

		public double x;

		public double y;

		public double z;

		public static readonly Vector3d Zero = new Vector3d(0.0, 0.0, 0.0);

		public static readonly Vector3d One = new Vector3d(1.0, 1.0, 1.0);

		public static readonly Vector3d AxisX = new Vector3d(1.0, 0.0, 0.0);

		public static readonly Vector3d AxisY = new Vector3d(0.0, 1.0, 0.0);

		public static readonly Vector3d AxisZ = new Vector3d(0.0, 0.0, 1.0);

		public static readonly Vector3d MaxValue = new Vector3d(double.MaxValue, double.MaxValue, double.MaxValue);

		public static readonly Vector3d MinValue = new Vector3d(double.MinValue, double.MinValue, double.MinValue);
	}
}
