using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace g3
{
	public struct Vector2d : IComparable<Vector2d>, IEquatable<Vector2d>
	{
		public Vector2d(double f)
		{
			this.y = f;
			this.x = f;
		}

		public Vector2d(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2d(double[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
		}

		public Vector2d(float f)
		{
			this.x = (this.y = (double)f);
		}

		public Vector2d(float x, float y)
		{
			this.x = (double)x;
			this.y = (double)y;
		}

		public Vector2d(float[] v2)
		{
			this.x = (double)v2[0];
			this.y = (double)v2[1];
		}

		public Vector2d(Vector2d copy)
		{
			this.x = copy.x;
			this.y = copy.y;
		}

		public Vector2d(Vector2f copy)
		{
			this.x = (double)copy.x;
			this.y = (double)copy.y;
		}

		public static Vector2d FromAngleRad(double angle)
		{
			return new Vector2d(Math.Cos(angle), Math.Sin(angle));
		}

		public static Vector2d FromAngleDeg(double angle)
		{
			angle *= 0.017453292519943295;
			return new Vector2d(Math.Cos(angle), Math.Sin(angle));
		}

		public double this[int key]
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

		public double LengthSquared
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}

		public double Length
		{
			get
			{
				return Math.Sqrt(this.LengthSquared);
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
			}
			else
			{
				num = 0.0;
				this.x = (this.y = 0.0);
			}
			return num;
		}

		public Vector2d Normalized
		{
			get
			{
				double length = this.Length;
				if (length > 2.220446049250313E-16)
				{
					double num = 1.0 / length;
					return new Vector2d(this.x * num, this.y * num);
				}
				return Vector2d.Zero;
			}
		}

		public bool IsNormalized
		{
			get
			{
				return Math.Abs(this.x * this.x + this.y * this.y - 1.0) < 1E-08;
			}
		}

		public bool IsFinite
		{
			get
			{
				double d = this.x + this.y;
				return !double.IsNaN(d) && !double.IsInfinity(d);
			}
		}

		public void Round(int nDecimals)
		{
			this.x = Math.Round(this.x, nDecimals);
			this.y = Math.Round(this.y, nDecimals);
		}

		public double Dot(Vector2d v2)
		{
			return this.x * v2.x + this.y * v2.y;
		}

		public double Cross(Vector2d v2)
		{
			return this.x * v2.y - this.y * v2.x;
		}

		public Vector2d Perp
		{
			get
			{
				return new Vector2d(this.y, -this.x);
			}
		}

		public Vector2d UnitPerp
		{
			get
			{
				return new Vector2d(this.y, -this.x).Normalized;
			}
		}

		public double DotPerp(Vector2d v2)
		{
			return this.x * v2.y - this.y * v2.x;
		}

		public double AngleD(Vector2d v2)
		{
			return Math.Acos(MathUtil.Clamp(this.Dot(v2), -1.0, 1.0)) * 57.29577951308232;
		}

		public static double AngleD(Vector2d v1, Vector2d v2)
		{
			return v1.AngleD(v2);
		}

		public double AngleR(Vector2d v2)
		{
			return Math.Acos(MathUtil.Clamp(this.Dot(v2), -1.0, 1.0));
		}

		public static double AngleR(Vector2d v1, Vector2d v2)
		{
			return v1.AngleR(v2);
		}

		public double DistanceSquared(Vector2d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			return num * num + num2 * num2;
		}

		public double Distance(Vector2d v2)
		{
			double num = v2.x - this.x;
			double num2 = v2.y - this.y;
			return Math.Sqrt(num * num + num2 * num2);
		}

		public void Set(Vector2d o)
		{
			this.x = o.x;
			this.y = o.y;
		}

		public void Set(double fX, double fY)
		{
			this.x = fX;
			this.y = fY;
		}

		public void Add(Vector2d o)
		{
			this.x += o.x;
			this.y += o.y;
		}

		public void Subtract(Vector2d o)
		{
			this.x -= o.x;
			this.y -= o.y;
		}

		public static Vector2d operator -(Vector2d v)
		{
			return new Vector2d(-v.x, -v.y);
		}

		public static Vector2d operator +(Vector2d a, Vector2d o)
		{
			return new Vector2d(a.x + o.x, a.y + o.y);
		}

		public static Vector2d operator +(Vector2d a, double f)
		{
			return new Vector2d(a.x + f, a.y + f);
		}

		public static Vector2d operator -(Vector2d a, Vector2d o)
		{
			return new Vector2d(a.x - o.x, a.y - o.y);
		}

		public static Vector2d operator -(Vector2d a, double f)
		{
			return new Vector2d(a.x - f, a.y - f);
		}

		public static Vector2d operator *(Vector2d a, double f)
		{
			return new Vector2d(a.x * f, a.y * f);
		}

		public static Vector2d operator *(double f, Vector2d a)
		{
			return new Vector2d(a.x * f, a.y * f);
		}

		public static Vector2d operator /(Vector2d v, double f)
		{
			return new Vector2d(v.x / f, v.y / f);
		}

		public static Vector2d operator /(double f, Vector2d v)
		{
			return new Vector2d(f / v.x, f / v.y);
		}

		public static Vector2d operator *(Vector2d a, Vector2d b)
		{
			return new Vector2d(a.x * b.x, a.y * b.y);
		}

		public static Vector2d operator /(Vector2d a, Vector2d b)
		{
			return new Vector2d(a.x / b.x, a.y / b.y);
		}

		public static bool operator ==(Vector2d a, Vector2d b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vector2d a, Vector2d b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public override bool Equals(object obj)
		{
			return this == (Vector2d)obj;
		}

		public override int GetHashCode()
		{
			return (-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode();
		}

		public int CompareTo(Vector2d other)
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

		public bool Equals(Vector2d other)
		{
			return this.x == other.x && this.y == other.y;
		}

		public bool EpsilonEqual(Vector2d v2, double epsilon)
		{
			return Math.Abs(this.x - v2.x) <= epsilon && Math.Abs(this.y - v2.y) <= epsilon;
		}

		public static Vector2d Lerp(Vector2d a, Vector2d b, double t)
		{
			double num = 1.0 - t;
			return new Vector2d(num * a.x + t * b.x, num * a.y + t * b.y);
		}

		public static Vector2d Lerp(ref Vector2d a, ref Vector2d b, double t)
		{
			double num = 1.0 - t;
			return new Vector2d(num * a.x + t * b.x, num * a.y + t * b.y);
		}

		public override string ToString()
		{
			return string.Format("{0:F8} {1:F8}", this.x, this.y);
		}

		public static implicit operator Vector2d(Vector2f v)
		{
			return new Vector2d(v.x, v.y);
		}

		public static explicit operator Vector2f(Vector2d v)
		{
			return new Vector2f((float)v.x, (float)v.y);
		}

		public static explicit operator float2(Vector2d v)
		{
			return new float2((float)v.x, (float)v.y);
		}

		public static implicit operator Vector2d(float2 v)
		{
			return new Vector2d(v.x, v.y);
		}

		public static implicit operator Vector2d(double2 v)
		{
			return new Vector2d(v.x, v.y);
		}

		public static implicit operator double2(Vector2d v)
		{
			return new double2(v.x, v.y);
		}

		public static implicit operator Vector2d(Vector2 v)
		{
			return new Vector2d(v.x, v.y);
		}

		public static explicit operator Vector2(Vector2d v)
		{
			return new Vector2((float)v.x, (float)v.y);
		}

		public static void GetInformation(IList<Vector2d> points, double epsilon, out Vector2d.Information info)
		{
			info = default(Vector2d.Information);
			int count = points.Count;
			if (count == 0 || points == null || epsilon <= 0.0)
			{
				return;
			}
			info.mExtremeCCW = false;
			Vector2i zero = Vector2i.Zero;
			Vector2i zero2 = Vector2i.Zero;
			for (int i = 0; i < 2; i++)
			{
				info.mMin[i] = points[0][i];
				info.mMax[i] = info.mMin[i];
				zero[i] = 0;
				zero2[i] = 0;
			}
			for (int j = 1; j < count; j++)
			{
				for (int i = 0; i < 2; i++)
				{
					if (points[j][i] < info.mMin[i])
					{
						info.mMin[i] = points[j][i];
						zero[i] = j;
					}
					else if (points[j][i] > info.mMax[i])
					{
						info.mMax[i] = points[j][i];
						zero2[i] = j;
					}
				}
			}
			info.mMaxRange = info.mMax[0] - info.mMin[0];
			info.mExtreme[0] = zero[0];
			info.mExtreme[1] = zero2[0];
			double num = info.mMax[1] - info.mMin[1];
			if (num > info.mMaxRange)
			{
				info.mMaxRange = num;
				info.mExtreme[0] = zero[1];
				info.mExtreme[1] = zero2[1];
			}
			info.mOrigin = points[info.mExtreme[0]];
			if (info.mMaxRange < epsilon)
			{
				info.mDimension = 0;
				info.mDirection0 = Vector2d.Zero;
				info.mDirection1 = Vector2d.Zero;
				for (int i = 0; i < 2; i++)
				{
					info.mExtreme[i + 1] = info.mExtreme[0];
				}
				return;
			}
			info.mDirection0 = points[info.mExtreme[1]] - info.mOrigin;
			info.mDirection0.Normalize(2.220446049250313E-16);
			info.mDirection1 = -info.mDirection0.Perp;
			double num2 = 0.0;
			double num3 = 0.0;
			info.mExtreme[2] = info.mExtreme[0];
			for (int j = 0; j < count; j++)
			{
				Vector2d v = points[j] - info.mOrigin;
				double num4 = info.mDirection1.Dot(v);
				double num5 = (double)Math.Sign(num4);
				num4 = Math.Abs(num4);
				if (num4 > num2)
				{
					num2 = num4;
					num3 = num5;
					info.mExtreme[2] = j;
				}
			}
			if (num2 < epsilon * info.mMaxRange)
			{
				info.mDimension = 1;
				info.mExtreme[2] = info.mExtreme[1];
				return;
			}
			info.mDimension = 2;
			info.mExtremeCCW = (num3 > 0.0);
		}

		public double x;

		public double y;

		public static readonly Vector2d Zero = new Vector2d(0f, 0f);

		public static readonly Vector2d One = new Vector2d(1f, 1f);

		public static readonly Vector2d AxisX = new Vector2d(1f, 0f);

		public static readonly Vector2d AxisY = new Vector2d(0f, 1f);

		public static readonly Vector2d MaxValue = new Vector2d(double.MaxValue, double.MaxValue);

		public static readonly Vector2d MinValue = new Vector2d(double.MinValue, double.MinValue);

		public struct Information
		{
			public int mDimension;

			public Vector2d mMin;

			public Vector2d mMax;

			public double mMaxRange;

			public Vector2d mOrigin;

			public Vector2d mDirection0;

			public Vector2d mDirection1;

			public Vector3i mExtreme;

			public bool mExtremeCCW;
		}
	}
}
