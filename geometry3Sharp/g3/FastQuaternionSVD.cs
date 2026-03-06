using System;

namespace g3
{
	public class FastQuaternionSVD
	{
		public FastQuaternionSVD()
		{
		}

		public FastQuaternionSVD(Matrix3d matrix, double epsilon = 2.220446049250313E-16, int jacobiIters = 4)
		{
			this.Solve(matrix, epsilon, jacobiIters);
		}

		public void Solve(Matrix3d matrix, double epsilon = 2.220446049250313E-16, int jacobiIters = -1)
		{
			if (jacobiIters != -1)
			{
				this.NumJacobiIterations = jacobiIters;
			}
			if (this.ATA == null)
			{
				this.ATA = new SymmetricMatrix3d();
			}
			this.ATA.SetATA(ref matrix);
			Vector4d vector4d = this.jacobiDiagonalize(this.ATA);
			if (this.AV == null)
			{
				this.AV = new double[9];
			}
			this.computeAV(ref matrix, ref vector4d, this.AV);
			Vector4d zero = Vector4d.Zero;
			this.QRFactorize(this.AV, ref vector4d, epsilon, ref this.S, ref zero);
			this.U = new Quaterniond(zero[1], zero[2], zero[3], zero[0]);
			this.V = new Quaterniond(vector4d[1], vector4d[2], vector4d[3], vector4d[0]);
		}

		public Matrix3d ReconstructMatrix()
		{
			Matrix3d mat = new Matrix3d(this.S[0], this.S[1], this.S[2]);
			return this.U.ToRotationMatrix() * mat * this.V.Conjugate().ToRotationMatrix();
		}

		private Vector4d jacobiDiagonalize(SymmetricMatrix3d ATA)
		{
			Vector4d result = new Vector4d(1.0, 0.0, 0.0, 0.0);
			for (int i = 0; i < this.NumJacobiIterations; i++)
			{
				Vector2d vector2d = this.givensAngles(ATA, 0, 1);
				ATA.quatConjugate01(vector2d.x, vector2d.y);
				this.quatTimesEqualCoordinateAxis(ref result, vector2d.x, vector2d.y, 2);
				vector2d = this.givensAngles(ATA, 1, 2);
				ATA.quatConjugate12(vector2d.x, vector2d.y);
				this.quatTimesEqualCoordinateAxis(ref result, vector2d.x, vector2d.y, 0);
				vector2d = this.givensAngles(ATA, 0, 2);
				ATA.quatConjugate02(vector2d.x, vector2d.y);
				this.quatTimesEqualCoordinateAxis(ref result, vector2d.x, vector2d.y, 1);
			}
			return result;
		}

		private Vector2d givensAngles(SymmetricMatrix3d B, int p, int q)
		{
			double num = 0.0;
			double num2 = 0.0;
			if (p == 0)
			{
				if (q == 1)
				{
					num = B.entries[p] - B.entries[q];
					num2 = 0.5 * B.entries[3];
				}
				else
				{
					num = B.entries[q] - B.entries[p];
					num2 = 0.5 * B.entries[4];
				}
			}
			else if (p == 1)
			{
				num = B.entries[p] - B.entries[q];
				num2 = 0.5 * B.entries[5];
			}
			double num3 = 1.0 / Math.Sqrt(num * num + num2 * num2);
			num *= num3;
			num2 *= num3;
			bool flag = 5.82842712474619 * num2 * num2 < num * num;
			num = (flag ? num : 0.9238795325112867);
			num2 = (flag ? num2 : 0.3826834323650897);
			return new Vector2d(num, num2);
		}

		private void computeAV(ref Matrix3d matrix, ref Vector4d V, double[] buf)
		{
			Quaterniond quaterniond = new Quaterniond(V[1], V[2], V[3], V[0]);
			Matrix3d mat = quaterniond.ToRotationMatrix();
			(matrix * mat).ToBuffer(buf);
		}

		private void QRFactorize(double[] AV, ref Vector4d V, double eps, ref Vector3d S, ref Vector4d U)
		{
			this.permuteColumns(AV, ref V);
			U = new Vector4d(1.0, 0.0, 0.0, 0.0);
			Vector2d vector2d = this.computeGivensQR(AV, eps, 1, 0);
			this.givensQTB2(AV, vector2d.x, vector2d.y);
			this.quatTimesEqualCoordinateAxis(ref U, vector2d.x, vector2d.y, 2);
			Vector2d vector2d2 = this.computeGivensQR(AV, eps, 2, 0);
			this.givensQTB1(AV, vector2d2.x, -vector2d2.y);
			this.quatTimesEqualCoordinateAxis(ref U, vector2d2.x, -vector2d2.y, 1);
			Vector2d vector2d3 = this.computeGivensQR(AV, eps, 2, 1);
			this.givensQTB0(AV, vector2d3.x, vector2d3.y);
			this.quatTimesEqualCoordinateAxis(ref U, vector2d3.x, vector2d3.y, 0);
			S = new Vector3d(AV[0], AV[4], AV[8]);
		}

		private Vector2d computeGivensQR(double[] B, double eps, int r, int c)
		{
			double num = B[4 * c];
			double num2 = B[3 * r + c];
			double num3 = Math.Sqrt(num * num + num2 * num2);
			double num4 = (num3 > eps) ? num2 : 0.0;
			double num5 = Math.Abs(num) + Math.Max(num3, eps);
			if (num < 0.0)
			{
				double num6 = num4;
				num4 = num5;
				num5 = num6;
			}
			double num7 = 1.0 / Math.Sqrt(num5 * num5 + num4 * num4);
			num5 *= num7;
			num4 *= num7;
			return new Vector2d(num5, num4);
		}

		private void givensQTB2(double[] B, double ch, double sh)
		{
			double num = ch * ch - sh * sh;
			double num2 = 2.0 * sh * ch;
			double num3 = B[0] * num + B[3] * num2;
			double num4 = B[1] * num + B[4] * num2;
			double num5 = B[2] * num + B[5] * num2;
			double num6 = 0.0;
			double num7 = B[4] * num - B[1] * num2;
			double num8 = B[5] * num - B[2] * num2;
			B[0] = num3;
			B[1] = num4;
			B[2] = num5;
			B[3] = num6;
			B[4] = num7;
			B[5] = num8;
		}

		private void givensQTB1(double[] B, double ch, double sh)
		{
			double num = ch * ch - sh * sh;
			double num2 = 2.0 * sh * ch;
			double num3 = B[0] * num - B[6] * num2;
			double num4 = B[1] * num - B[7] * num2;
			double num5 = B[2] * num - B[8] * num2;
			double num6 = 0.0;
			double num7 = B[1] * num2 + B[7] * num;
			double num8 = B[2] * num2 + B[8] * num;
			B[0] = num3;
			B[1] = num4;
			B[2] = num5;
			B[6] = num6;
			B[7] = num7;
			B[8] = num8;
		}

		private void givensQTB0(double[] B, double ch, double sh)
		{
			double num = ch * ch - sh * sh;
			double num2 = 2.0 * ch * sh;
			double num3 = B[4] * num + B[7] * num2;
			double num4 = B[8] * num - B[5] * num2;
			B[4] = num3;
			B[8] = num4;
		}

		private void quatTimesEqualCoordinateAxis(ref Vector4d lhs, double c, double s, int i)
		{
			double x = lhs.x * c - lhs[i + 1] * s;
			Vector3d vector3d = new Vector3d(c * lhs.y, c * lhs.z, c * lhs.w);
			ref Vector3d ptr = ref vector3d;
			ptr[i] += lhs.x * s;
			ptr = ref vector3d;
			int key = (i + 1) % 3;
			ptr[key] += s * lhs[1 + (i + 2) % 3];
			ptr = ref vector3d;
			key = (i + 2) % 3;
			ptr[key] -= s * lhs[1 + (i + 1) % 3];
			lhs.x = x;
			lhs.y = vector3d.x;
			lhs.z = vector3d.y;
			lhs.w = vector3d.z;
		}

		private void permuteColumns(double[] B, ref Vector4d V)
		{
			double num = B[0] * B[0] + B[3] * B[3] + B[6] * B[6];
			double num2 = B[1] * B[1] + B[4] * B[4] + B[7] * B[7];
			double num3 = B[2] * B[2] + B[5] * B[5] + B[8] * B[8];
			if (num < num2)
			{
				this.swapColsNeg(B, 0, 1);
				this.quatTimesEqualCoordinateAxis(ref V, 0.7071067811865475, 0.7071067811865475, 2);
				double num4 = num;
				num = num2;
				num2 = num4;
			}
			if (num < num3)
			{
				this.swapColsNeg(B, 0, 2);
				this.quatTimesEqualCoordinateAxis(ref V, 0.7071067811865475, -0.7071067811865475, 1);
				num3 = num;
			}
			if (num2 < num3)
			{
				this.swapColsNeg(B, 1, 2);
				this.quatTimesEqualCoordinateAxis(ref V, 0.7071067811865475, 0.7071067811865475, 0);
			}
		}

		private void swapColsNeg(double[] B, int i, int j)
		{
			double num = -B[i];
			B[i] = B[j];
			B[j] = num;
			num = -B[i + 3];
			B[i + 3] = B[j + 3];
			B[j + 3] = num;
			num = -B[i + 6];
			B[i + 6] = B[j + 6];
			B[j + 6] = num;
		}

		private int NumJacobiIterations = 4;

		public Quaterniond U;

		public Quaterniond V;

		public Vector3d S;

		private SymmetricMatrix3d ATA;

		private double[] AV;

		private const double gamma = 5.82842712474619;

		private const double sinBackup = 0.3826834323650897;

		private const double cosBackup = 0.9238795325112867;
	}
}
