using System;

namespace g3
{
	public class SphericalFibonacciPointSet
	{
		public SphericalFibonacciPointSet(int n = 64)
		{
			this.N = n;
		}

		public int Count
		{
			get
			{
				return this.N;
			}
		}

		public Vector3d Point(int i)
		{
			double num = (double)i / SphericalFibonacciPointSet.PHI;
			double num2 = 6.283185307179586 * (num - Math.Floor(num));
			double num3 = Math.Cos(num2);
			double num4 = Math.Sin(num2);
			double num5 = 1.0 - (2.0 * (double)i + 1.0) / (double)this.N;
			double num6 = Math.Sin(Math.Acos(num5));
			return new Vector3d(num3 * num6, num4 * num6, num5);
		}

		public Vector3d this[int i]
		{
			get
			{
				return this.Point(i);
			}
		}

		public int NearestPoint(Vector3d p, bool bIsNormalized = false)
		{
			if (bIsNormalized)
			{
				return this.inverseSF(ref p);
			}
			p.Normalize(2.220446049250313E-16);
			return this.inverseSF(ref p);
		}

		private double madfrac(double a, double b)
		{
			return a * b + -Math.Floor(a * b);
		}

		private int inverseSF(ref Vector3d p)
		{
			double num = Math.Min(Math.Atan2(p.y, p.x), 3.141592653589793);
			double num2 = p.z;
			double y = Math.Max(2.0, Math.Floor(Math.Log((double)this.N * 3.141592653589793 * Math.Sqrt(5.0) * (1.0 - num2 * num2)) / Math.Log(SphericalFibonacciPointSet.PHI * SphericalFibonacciPointSet.PHI)));
			double num3 = Math.Pow(SphericalFibonacciPointSet.PHI, y) / Math.Sqrt(5.0);
			double num4 = Math.Round(num3);
			double num5 = Math.Round(num3 * SphericalFibonacciPointSet.PHI);
			Matrix2d matrix2d = new Matrix2d(6.283185307179586 * this.madfrac(num4 + 1.0, SphericalFibonacciPointSet.PHI - 1.0) - 6.283185307179586 * (SphericalFibonacciPointSet.PHI - 1.0), 6.283185307179586 * this.madfrac(num5 + 1.0, SphericalFibonacciPointSet.PHI - 1.0) - 6.283185307179586 * (SphericalFibonacciPointSet.PHI - 1.0), -2.0 * num4 / (double)this.N, -2.0 * num5 / (double)this.N);
			Matrix2d m = matrix2d.Inverse(0.0);
			Vector2d vector2d = new Vector2d(num, num2 - (1.0 - 1.0 / (double)this.N));
			vector2d = m * vector2d;
			vector2d.x = Math.Floor(vector2d.x);
			vector2d.y = Math.Floor(vector2d.y);
			double num6 = double.PositiveInfinity;
			double num7 = 0.0;
			for (uint num8 = 0U; num8 < 4U; num8 += 1U)
			{
				Vector2d v = new Vector2d(num8 % 2U, num8 / 2U) + vector2d;
				num2 = matrix2d.Row(1).Dot(v) + (1.0 - 1.0 / (double)this.N);
				num2 = MathUtil.Clamp(num2, -1.0, 1.0) * 2.0 - num2;
				double num9 = Math.Floor((double)this.N * 0.5 - num2 * (double)this.N * 0.5);
				num = 6.283185307179586 * this.madfrac(num9, SphericalFibonacciPointSet.PHI - 1.0);
				num2 = 1.0 - (2.0 * num9 + 1.0) * (1.0 / (double)this.N);
				double num10 = Math.Sqrt(1.0 - num2 * num2);
				Vector3d v2 = new Vector3d(Math.Cos(num) * num10, Math.Sin(num) * num10, num2);
				double num11 = Vector3d.Dot(v2 - p, v2 - p);
				if (num11 < num6)
				{
					num6 = num11;
					num7 = num9;
				}
			}
			return (int)num7;
		}

		public int N = 64;

		private static readonly double PHI = (Math.Sqrt(5.0) + 1.0) / 2.0;
	}
}
