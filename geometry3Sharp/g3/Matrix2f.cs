using System;
using Unity.Mathematics;

namespace g3
{
	public class Matrix2f
	{
		public Matrix2f(bool bIdentity)
		{
			if (bIdentity)
			{
				this.m00 = (this.m11 = 1f);
				this.m01 = (this.m10 = 0f);
				return;
			}
			this.m00 = (this.m01 = (this.m10 = (this.m11 = 0f)));
		}

		public Matrix2f(float m00, float m01, float m10, float m11)
		{
			this.m00 = m00;
			this.m01 = m01;
			this.m10 = m10;
			this.m11 = m11;
		}

		public Matrix2f(float m00, float m11)
		{
			this.m00 = m00;
			this.m11 = m11;
			this.m01 = (this.m10 = 0f);
		}

		public Matrix2f(float radians)
		{
			this.SetToRotationRad(radians);
		}

		public Matrix2f(Vector2f u, Vector2f v, bool columns)
		{
			if (columns)
			{
				this.m00 = u.x;
				this.m01 = v.x;
				this.m10 = u.y;
				this.m11 = v.y;
				return;
			}
			this.m00 = u.x;
			this.m01 = u.y;
			this.m10 = v.x;
			this.m11 = v.y;
		}

		public Vector2f Row(int i)
		{
			if (i != 0)
			{
				return new Vector2f(this.m10, this.m11);
			}
			return new Vector2f(this.m00, this.m01);
		}

		public Vector2f Column(int i)
		{
			if (i != 0)
			{
				return new Vector2f(this.m01, this.m11);
			}
			return new Vector2f(this.m00, this.m10);
		}

		public Matrix2f(Vector2f u, Vector2f v)
		{
			this.m00 = u.x * v.x;
			this.m01 = u.x * v.y;
			this.m10 = u.y * v.x;
			this.m11 = u.y * v.y;
		}

		public void SetToDiagonal(float m00, float m11)
		{
			this.m00 = m00;
			this.m11 = m11;
			this.m01 = (this.m10 = 0f);
		}

		public void SetToRotationRad(float angleRad)
		{
			this.m11 = (this.m00 = (float)Math.Cos((double)angleRad));
			this.m10 = (float)Math.Sin((double)angleRad);
			this.m01 = -this.m10;
		}

		public void SetToRotationDeg(float angleDeg)
		{
			this.SetToRotationRad(0.017453292f * angleDeg);
		}

		public float QForm(Vector2f u, Vector2f v)
		{
			return u.Dot(this * v);
		}

		public Matrix2f Transpose()
		{
			return new Matrix2f(this.m00, this.m10, this.m01, this.m11);
		}

		public Matrix2f Inverse(float epsilon = 0f)
		{
			float num = this.m00 * this.m11 - this.m10 * this.m01;
			if (Math.Abs(num) > epsilon)
			{
				float num2 = 1f / num;
				return new Matrix2f(this.m11 * num2, -this.m01 * num2, -this.m10 * num2, this.m00 * num2);
			}
			return Matrix2f.Zero;
		}

		public Matrix2f Adjoint()
		{
			return new Matrix2f(this.m11, -this.m01, -this.m10, this.m00);
		}

		public float Determinant
		{
			get
			{
				return this.m00 * this.m11 - this.m01 * this.m10;
			}
		}

		public float ExtractAngle()
		{
			return (float)Math.Atan2((double)this.m10, (double)this.m00);
		}

		public void Orthonormalize()
		{
			float num = 1f / (float)Math.Sqrt((double)(this.m00 * this.m00 + this.m10 * this.m10));
			this.m00 *= num;
			this.m10 *= num;
			float num2 = this.m00 * this.m01 + this.m10 * this.m11;
			this.m01 -= num2 * this.m00;
			this.m11 -= num2 * this.m10;
			num = 1f / (float)Math.Sqrt((double)(this.m01 * this.m01 + this.m11 * this.m11));
			this.m01 *= num;
			this.m11 *= num;
		}

		public void EigenDecomposition(ref Matrix2f rot, ref Matrix2f diag)
		{
			float num = Math.Abs(this.m00) + Math.Abs(this.m11);
			if (Math.Abs(this.m01) + num == num)
			{
				rot.m00 = 1f;
				rot.m01 = 0f;
				rot.m10 = 0f;
				rot.m11 = 1f;
				diag.m00 = this.m00;
				diag.m01 = 0f;
				diag.m10 = 0f;
				diag.m11 = this.m11;
				return;
			}
			float num2 = this.m00 + this.m11;
			float num3 = this.m00 - this.m11;
			float num4 = (float)Math.Sqrt((double)(num3 * num3 + 4f * this.m01 * this.m01));
			float num5 = 0.5f * (num2 - num4);
			float num6 = 0.5f * (num2 + num4);
			diag.SetToDiagonal(num5, num6);
			float num7;
			float num8;
			if ((double)num3 >= 0.0)
			{
				num7 = this.m01;
				num8 = num5 - this.m00;
			}
			else
			{
				num7 = num5 - this.m11;
				num8 = this.m01;
			}
			float num9 = 1f / (float)Math.Sqrt((double)(num7 * num7 + num8 * num8));
			num7 *= num9;
			num8 *= num9;
			rot.m00 = num7;
			rot.m01 = -num8;
			rot.m10 = num8;
			rot.m11 = num7;
		}

		public static Matrix2f operator -(Matrix2f v)
		{
			return new Matrix2f(-v.m00, -v.m01, -v.m10, -v.m11);
		}

		public static Matrix2f operator +(Matrix2f a, Matrix2f o)
		{
			return new Matrix2f(a.m00 + o.m00, a.m01 + o.m01, a.m10 + o.m10, a.m11 + o.m11);
		}

		public static Matrix2f operator +(Matrix2f a, float f)
		{
			return new Matrix2f(a.m00 + f, a.m01 + f, a.m10 + f, a.m11 + f);
		}

		public static Matrix2f operator -(Matrix2f a, Matrix2f o)
		{
			return new Matrix2f(a.m00 - o.m00, a.m01 - o.m01, a.m10 - o.m10, a.m11 - o.m11);
		}

		public static Matrix2f operator -(Matrix2f a, float f)
		{
			return new Matrix2f(a.m00 - f, a.m01 - f, a.m10 - f, a.m11 - f);
		}

		public static Matrix2f operator *(Matrix2f a, float f)
		{
			return new Matrix2f(a.m00 * f, a.m01 * f, a.m10 * f, a.m11 * f);
		}

		public static Matrix2f operator *(float f, Matrix2f a)
		{
			return new Matrix2f(a.m00 * f, a.m01 * f, a.m10 * f, a.m11 * f);
		}

		public static Matrix2f operator /(Matrix2f a, float f)
		{
			return new Matrix2f(a.m00 / f, a.m01 / f, a.m10 / f, a.m11 / f);
		}

		public static Vector2f operator *(Matrix2f m, Vector2f v)
		{
			return new Vector2f(m.m00 * v.x + m.m01 * v.y, m.m10 * v.x + m.m11 * v.y);
		}

		public static Vector2f operator *(Vector2f v, Matrix2f m)
		{
			return new Vector2f(v.x * m.m00 + v.y * m.m10, v.x * m.m01 + v.y * m.m11);
		}

		public static implicit operator Matrix2f(float2x2 m)
		{
			return new Matrix2f(m.c0, m.c1, true);
		}

		public static explicit operator float2x2(Matrix2f m)
		{
			return new float2x2(m.Column(0), m.Column(1));
		}

		public float m00;

		public float m01;

		public float m10;

		public float m11;

		public static readonly Matrix2f Identity = new Matrix2f(true);

		public static readonly Matrix2f Zero = new Matrix2f(false);

		public static readonly Matrix2f One = new Matrix2f(1f, 1f, 1f, 1f);
	}
}
