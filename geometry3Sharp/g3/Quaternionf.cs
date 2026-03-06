using System;
using Unity.Mathematics;
using UnityEngine;

namespace g3
{
	public struct Quaternionf : IComparable<Quaternionf>, IEquatable<Quaternionf>
	{
		public Quaternionf(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Quaternionf(float[] v2)
		{
			this.x = v2[0];
			this.y = v2[1];
			this.z = v2[2];
			this.w = v2[3];
		}

		public Quaternionf(Quaternionf q2)
		{
			this.x = q2.x;
			this.y = q2.y;
			this.z = q2.z;
			this.w = q2.w;
		}

		public Quaternionf(Vector3f axis, float AngleDeg)
		{
			this.x = (this.y = (this.z = 0f));
			this.w = 1f;
			this.SetAxisAngleD(axis, AngleDeg);
		}

		public Quaternionf(Vector3f vFrom, Vector3f vTo)
		{
			this.x = (this.y = (this.z = 0f));
			this.w = 1f;
			this.SetFromTo(vFrom, vTo);
		}

		public Quaternionf(Quaternionf p, Quaternionf q, float t)
		{
			this.x = (this.y = (this.z = 0f));
			this.w = 1f;
			this.SetToSlerp(p, q, t);
		}

		public Quaternionf(Matrix3f mat)
		{
			this.x = (this.y = (this.z = 0f));
			this.w = 1f;
			this.SetFromRotationMatrix(mat);
		}

		public float this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.x;
				}
				if (key == 1)
				{
					return this.y;
				}
				if (key == 2)
				{
					return this.z;
				}
				return this.w;
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
				if (key == 2)
				{
					this.z = value;
					return;
				}
				this.w = value;
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
				return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w));
			}
		}

		public float Normalize(float epsilon = 0f)
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

		public Quaternionf Normalized
		{
			get
			{
				Quaternionf result = new Quaternionf(this);
				result.Normalize(0f);
				return result;
			}
		}

		public float Dot(Quaternionf q2)
		{
			return this.x * q2.x + this.y * q2.y + this.z * q2.z + this.w * q2.w;
		}

		public static Quaternionf operator *(Quaternionf a, Quaternionf b)
		{
			float num = a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z;
			float num2 = a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y;
			float num3 = a.w * b.y + a.y * b.w + a.z * b.x - a.x * b.z;
			float num4 = a.w * b.z + a.z * b.w + a.x * b.y - a.y * b.x;
			return new Quaternionf(num2, num3, num4, num);
		}

		public static Quaternionf operator -(Quaternionf q1, Quaternionf q2)
		{
			return new Quaternionf(q1.x - q2.x, q1.y - q2.y, q1.z - q2.z, q1.w - q2.w);
		}

		public static Vector3f operator *(Quaternionf q, Vector3f v)
		{
			float num = 2f * q.x;
			float num2 = 2f * q.y;
			float num3 = 2f * q.z;
			float num4 = num * q.w;
			float num5 = num2 * q.w;
			float num6 = num3 * q.w;
			float num7 = num * q.x;
			float num8 = num2 * q.x;
			float num9 = num3 * q.x;
			float num10 = num2 * q.y;
			float num11 = num3 * q.y;
			float num12 = num3 * q.z;
			return new Vector3f(v.x * (1f - (num10 + num12)) + v.y * (num8 - num6) + v.z * (num9 + num5), v.x * (num8 + num6) + v.y * (1f - (num7 + num12)) + v.z * (num11 - num4), v.x * (num9 - num5) + v.y * (num11 + num4) + v.z * (1f - (num7 + num10)));
		}

		public static Vector3d operator *(Quaternionf q, Vector3d v)
		{
			double num = (double)(2f * q.x);
			double num2 = (double)(2f * q.y);
			double num3 = (double)(2f * q.z);
			double num4 = num * (double)q.w;
			double num5 = num2 * (double)q.w;
			double num6 = num3 * (double)q.w;
			double num7 = num * (double)q.x;
			double num8 = num2 * (double)q.x;
			double num9 = num3 * (double)q.x;
			double num10 = num2 * (double)q.y;
			double num11 = num3 * (double)q.y;
			double num12 = num3 * (double)q.z;
			return new Vector3d(v.x * (1.0 - (num10 + num12)) + v.y * (num8 - num6) + v.z * (num9 + num5), v.x * (num8 + num6) + v.y * (1.0 - (num7 + num12)) + v.z * (num11 - num4), v.x * (num9 - num5) + v.y * (num11 + num4) + v.z * (1.0 - (num7 + num10)));
		}

		public Vector3f InverseMultiply(ref Vector3f v)
		{
			float lengthSquared = this.LengthSquared;
			if (lengthSquared > 0f)
			{
				float num = 1f / lengthSquared;
				float num2 = -this.x * num;
				float num3 = -this.y * num;
				float num4 = -this.z * num;
				float num5 = this.w * num;
				float num6 = 2f * num2;
				float num7 = 2f * num3;
				float num8 = 2f * num4;
				float num9 = num6 * num5;
				float num10 = num7 * num5;
				float num11 = num8 * num5;
				float num12 = num6 * num2;
				float num13 = num7 * num2;
				float num14 = num8 * num2;
				float num15 = num7 * num3;
				float num16 = num8 * num3;
				float num17 = num8 * num4;
				return new Vector3f(v.x * (1f - (num15 + num17)) + v.y * (num13 - num11) + v.z * (num14 + num10), v.x * (num13 + num11) + v.y * (1f - (num12 + num17)) + v.z * (num16 - num9), v.x * (num14 - num10) + v.y * (num16 + num9) + v.z * (1f - (num12 + num15)));
			}
			return Vector3f.Zero;
		}

		public Vector3d InverseMultiply(ref Vector3d v)
		{
			float lengthSquared = this.LengthSquared;
			if (lengthSquared > 0f)
			{
				float num = 1f / lengthSquared;
				float num2 = -this.x * num;
				float num3 = -this.y * num;
				float num4 = -this.z * num;
				float num5 = this.w * num;
				double num6 = (double)(2f * num2);
				double num7 = (double)(2f * num3);
				double num8 = (double)(2f * num4);
				double num9 = num6 * (double)num5;
				double num10 = num7 * (double)num5;
				double num11 = num8 * (double)num5;
				double num12 = num6 * (double)num2;
				double num13 = num7 * (double)num2;
				double num14 = num8 * (double)num2;
				double num15 = num7 * (double)num3;
				double num16 = num8 * (double)num3;
				double num17 = num8 * (double)num4;
				return new Vector3d(v.x * (1.0 - (num15 + num17)) + v.y * (num13 - num11) + v.z * (num14 + num10), v.x * (num13 + num11) + v.y * (1.0 - (num12 + num17)) + v.z * (num16 - num9), v.x * (num14 - num10) + v.y * (num16 + num9) + v.z * (1.0 - (num12 + num15)));
			}
			return Vector3f.Zero;
		}

		public Vector3f AxisX
		{
			get
			{
				float num = 2f * this.y;
				float num2 = 2f * this.z;
				float num3 = num * this.w;
				float num4 = num2 * this.w;
				float num5 = num * this.x;
				float num6 = num2 * this.x;
				float num7 = num * this.y;
				float num8 = num2 * this.z;
				return new Vector3f(1f - (num7 + num8), num5 + num4, num6 - num3);
			}
		}

		public Vector3f AxisY
		{
			get
			{
				float num = 2f * this.x;
				float num2 = 2f * this.y;
				float num3 = 2f * this.z;
				float num4 = num * this.w;
				float num5 = num3 * this.w;
				float num6 = num * this.x;
				float num7 = num2 * this.x;
				float num8 = num3 * this.y;
				float num9 = num3 * this.z;
				return new Vector3f(num7 - num5, 1f - (num6 + num9), num8 + num4);
			}
		}

		public Vector3f AxisZ
		{
			get
			{
				float num = 2f * this.x;
				float num2 = 2f * this.y;
				float num3 = 2f * this.z;
				float num4 = num * this.w;
				float num5 = num2 * this.w;
				float num6 = num * this.x;
				float num7 = num3 * this.x;
				float num8 = num2 * this.y;
				float num9 = num3 * this.y;
				return new Vector3f(num7 + num5, num9 - num4, 1f - (num6 + num8));
			}
		}

		public Quaternionf Inverse()
		{
			float lengthSquared = this.LengthSquared;
			if (lengthSquared > 0f)
			{
				float num = 1f / lengthSquared;
				return new Quaternionf(-this.x * num, -this.y * num, -this.z * num, this.w * num);
			}
			return Quaternionf.Zero;
		}

		public static Quaternionf Inverse(Quaternionf q)
		{
			return q.Inverse();
		}

		public Matrix3f ToRotationMatrix()
		{
			float num = 2f * this.x;
			float num2 = 2f * this.y;
			float num3 = 2f * this.z;
			float num4 = num * this.w;
			float num5 = num2 * this.w;
			float num6 = num3 * this.w;
			float num7 = num * this.x;
			float num8 = num2 * this.x;
			float num9 = num3 * this.x;
			float num10 = num2 * this.y;
			float num11 = num3 * this.y;
			float num12 = num3 * this.z;
			Matrix3f zero = Matrix3f.Zero;
			zero[0, 0] = 1f - (num10 + num12);
			zero[0, 1] = num8 - num6;
			zero[0, 2] = num9 + num5;
			zero[1, 0] = num8 + num6;
			zero[1, 1] = 1f - (num7 + num12);
			zero[1, 2] = num11 - num4;
			zero[2, 0] = num9 - num5;
			zero[2, 1] = num11 + num4;
			zero[2, 2] = 1f - (num7 + num10);
			return zero;
		}

		public void SetAxisAngleD(Vector3f axis, float AngleDeg)
		{
			double num = 0.017453292519943295 * (double)AngleDeg;
			double num2 = 0.5 * num;
			double num3 = Math.Sin(num2);
			this.w = (float)Math.Cos(num2);
			this.x = (float)(num3 * (double)axis.x);
			this.y = (float)(num3 * (double)axis.y);
			this.z = (float)(num3 * (double)axis.z);
		}

		public static Quaternionf AxisAngleD(Vector3f axis, float angleDeg)
		{
			return new Quaternionf(axis, angleDeg);
		}

		public static Quaternionf AxisAngleR(Vector3f axis, float angleRad)
		{
			return new Quaternionf(axis, angleRad * 57.29578f);
		}

		public void SetFromTo(Vector3f vFrom, Vector3f vTo)
		{
			Vector3f normalized = vFrom.Normalized;
			Vector3f normalized2 = vTo.Normalized;
			Vector3f normalized3 = (normalized + normalized2).Normalized;
			this.w = normalized.Dot(normalized3);
			if (this.w != 0f)
			{
				Vector3f vector3f = normalized.Cross(normalized3);
				this.x = vector3f.x;
				this.y = vector3f.y;
				this.z = vector3f.z;
			}
			else if (Math.Abs(normalized.x) >= Math.Abs(normalized.y))
			{
				float num = (float)(1.0 / Math.Sqrt((double)(normalized.x * normalized.x + normalized.z * normalized.z)));
				this.x = -normalized.z * num;
				this.y = 0f;
				this.z = normalized.x * num;
			}
			else
			{
				float num = (float)(1.0 / Math.Sqrt((double)(normalized.y * normalized.y + normalized.z * normalized.z)));
				this.x = 0f;
				this.y = normalized.z * num;
				this.z = -normalized.y * num;
			}
			this.Normalize(0f);
		}

		public static Quaternionf FromTo(Vector3f vFrom, Vector3f vTo)
		{
			return new Quaternionf(vFrom, vTo);
		}

		public static Quaternionf FromToConstrained(Vector3f vFrom, Vector3f vTo, Vector3f vAround)
		{
			float angleDeg = MathUtil.PlaneAngleSignedD(vFrom, vTo, vAround);
			return Quaternionf.AxisAngleD(vAround, angleDeg);
		}

		public void SetToSlerp(Quaternionf p, Quaternionf q, float t)
		{
			float num = (float)Math.Acos((double)p.Dot(q));
			if ((double)Math.Abs(num) >= 1E-08)
			{
				float num2 = (float)Math.Sin((double)num);
				float num3 = 1f / num2;
				float num4 = t * num;
				float num5 = (float)Math.Sin((double)(num - num4)) * num3;
				float num6 = (float)Math.Sin((double)num4) * num3;
				this.x = num5 * p.x + num6 * q.x;
				this.y = num5 * p.y + num6 * q.y;
				this.z = num5 * p.z + num6 * q.z;
				this.w = num5 * p.w + num6 * q.w;
				return;
			}
			this.x = p.x;
			this.y = p.y;
			this.z = p.z;
			this.w = p.w;
		}

		public static Quaternionf Slerp(Quaternionf p, Quaternionf q, float t)
		{
			return new Quaternionf(p, q, t);
		}

		public void SetFromRotationMatrix(Matrix3f rot)
		{
			Index3i index3i = new Index3i(1, 2, 0);
			float num = rot[0, 0] + rot[1, 1] + rot[2, 2];
			if (num > 0f)
			{
				float num2 = (float)Math.Sqrt((double)(num + 1f));
				this.w = 0.5f * num2;
				num2 = 0.5f / num2;
				this.x = (rot[2, 1] - rot[1, 2]) * num2;
				this.y = (rot[0, 2] - rot[2, 0]) * num2;
				this.z = (rot[1, 0] - rot[0, 1]) * num2;
			}
			else
			{
				int num3 = 0;
				if (rot[1, 1] > rot[0, 0])
				{
					num3 = 1;
				}
				if (rot[2, 2] > rot[num3, num3])
				{
					num3 = 2;
				}
				int num4 = index3i[num3];
				int num5 = index3i[num4];
				float num2 = (float)Math.Sqrt((double)(rot[num3, num3] - rot[num4, num4] - rot[num5, num5] + 1f));
				Vector3f vector3f = new Vector3f(this.x, this.y, this.z);
				vector3f[num3] = 0.5f * num2;
				num2 = 0.5f / num2;
				this.w = (rot[num5, num4] - rot[num4, num5]) * num2;
				vector3f[num4] = (rot[num4, num3] + rot[num3, num4]) * num2;
				vector3f[num5] = (rot[num5, num3] + rot[num3, num5]) * num2;
				this.x = vector3f.x;
				this.y = vector3f.y;
				this.z = vector3f.z;
			}
			this.Normalize(0f);
		}

		public static bool operator ==(Quaternionf a, Quaternionf b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		}

		public static bool operator !=(Quaternionf a, Quaternionf b)
		{
			return a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
		}

		public override bool Equals(object obj)
		{
			return this == (Quaternionf)obj;
		}

		public override int GetHashCode()
		{
			return (((-2128831035 * 16777619 ^ this.x.GetHashCode()) * 16777619 ^ this.y.GetHashCode()) * 16777619 ^ this.z.GetHashCode()) * 16777619 ^ this.w.GetHashCode();
		}

		public int CompareTo(Quaternionf other)
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

		public bool Equals(Quaternionf other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w;
		}

		public bool EpsilonEqual(Quaternionf q2, float epsilon)
		{
			return Math.Abs(this.x - q2.x) <= epsilon && Math.Abs(this.y - q2.y) <= epsilon && Math.Abs(this.z - q2.z) <= epsilon && Math.Abs(this.w - q2.w) <= epsilon;
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

		public static implicit operator Quaternionf(Quaternion q)
		{
			return new Quaternionf(q.x, q.y, q.z, q.w);
		}

		public static implicit operator Quaternion(Quaternionf q)
		{
			return new Quaternion(q.x, q.y, q.z, q.w);
		}

		public static implicit operator Quaternionf(quaternion q)
		{
			float4 value = q.value;
			return new Quaternionf(value.x, value.y, value.z, value.w);
		}

		public static implicit operator quaternion(Quaternionf q)
		{
			return new quaternion(q.x, q.y, q.z, q.w);
		}

		public float x;

		public float y;

		public float z;

		public float w;

		public static readonly Quaternionf Zero = new Quaternionf(0f, 0f, 0f, 0f);

		public static readonly Quaternionf Identity = new Quaternionf(0f, 0f, 0f, 1f);
	}
}
