using System;
using System.Collections.Generic;

namespace g3
{
	public class SparseSymmetricCGMultipleRHS
	{
		public bool Solve()
		{
			this.Iterations = 0;
			if (this.B == null || this.MultiplyF == null)
			{
				throw new Exception("SparseSymmetricCGMultipleRHS.Solve(): Must set B and MultiplyF!");
			}
			int num = this.B.Length;
			if (num == 0)
			{
				throw new Exception("SparseSymmetricCGMultipleRHS.Solve(): Need at least one RHS vector in B");
			}
			int num2 = this.B[0].Length;
			this.R = BufferUtil.AllocNxM(num, num2);
			this.P = BufferUtil.AllocNxM(num, num2);
			this.W = BufferUtil.AllocNxM(num, num2);
			if (this.X == null || !this.UseXAsInitialGuess)
			{
				if (this.X == null)
				{
					this.X = BufferUtil.AllocNxM(num, num2);
				}
				for (int i = 0; i < num; i++)
				{
					Array.Clear(this.X[i], 0, num2);
					Array.Copy(this.B[i], this.R[i], num2);
				}
			}
			else
			{
				this.InitializeR(this.R);
			}
			double[] array = new double[num];
			for (int j2 = 0; j2 < num; j2++)
			{
				array[j2] = BufferUtil.Dot(this.B[j2], this.B[j2]);
			}
			double[] array2 = new double[num];
			for (int k = 0; k < num; k++)
			{
				array2[k] = Math.Sqrt(array[k]);
			}
			double[] rho0 = new double[num];
			for (int l = 0; l < num; l++)
			{
				rho0[l] = BufferUtil.Dot(this.R[l], this.R[l]);
			}
			bool[] converged = new bool[num];
			int num3 = 0;
			for (int m = 0; m < num; m++)
			{
				converged[m] = (rho0[m] < this.ConvergeTolerance * array2[m]);
				if (converged[m])
				{
					num3++;
				}
			}
			if (num3 == num)
			{
				return true;
			}
			for (int n = 0; n < num; n++)
			{
				Array.Copy(this.R[n], this.P[n], num2);
			}
			this.MultiplyF(this.P, this.W);
			double[] alpha = new double[num];
			for (int num4 = 0; num4 < num; num4++)
			{
				alpha[num4] = rho0[num4] / BufferUtil.Dot(this.P[num4], this.W[num4]);
			}
			for (int num5 = 0; num5 < num; num5++)
			{
				BufferUtil.MultiplyAdd(this.X[num5], alpha[num5], this.P[num5]);
			}
			for (int num6 = 0; num6 < num; num6++)
			{
				BufferUtil.MultiplyAdd(this.R[num6], -alpha[num6], this.W[num6]);
			}
			double[] rho1 = new double[num];
			for (int num7 = 0; num7 < num; num7++)
			{
				rho1[num7] = BufferUtil.Dot(this.R[num7], this.R[num7]);
			}
			double[] array3 = new double[num];
			Interval1i interval1i = Interval1i.Range(num);
			Action<int> <>9__0;
			Action<int> <>9__1;
			Action<int> <>9__2;
			int num8;
			for (num8 = 1; num8 < this.MaxIterations; num8++)
			{
				bool flag = true;
				for (int num9 = 0; num9 < num; num9++)
				{
					if (!converged[num9] && Math.Sqrt(rho1[num9]) <= this.ConvergeTolerance * array2[num9])
					{
						converged[num9] = true;
					}
					if (!converged[num9])
					{
						flag = false;
					}
				}
				if (flag)
				{
					break;
				}
				for (int num10 = 0; num10 < num; num10++)
				{
					array3[num10] = rho1[num10] / rho0[num10];
				}
				this.UpdateP(this.P, array3, this.R, converged);
				this.MultiplyF(this.P, this.W);
				IEnumerable<int> source = interval1i;
				Action<int> body;
				if ((body = <>9__0) == null)
				{
					body = (<>9__0 = delegate(int j)
					{
						if (!converged[j])
						{
							alpha[j] = rho1[j] / BufferUtil.Dot(this.P[j], this.W[j]);
						}
					});
				}
				gParallel.ForEach<int>(source, body);
				IEnumerable<int> source2 = interval1i;
				Action<int> body2;
				if ((body2 = <>9__1) == null)
				{
					body2 = (<>9__1 = delegate(int j)
					{
						if (!converged[j])
						{
							BufferUtil.MultiplyAdd(this.X[j], alpha[j], this.P[j]);
						}
					});
				}
				gParallel.ForEach<int>(source2, body2);
				IEnumerable<int> source3 = interval1i;
				Action<int> body3;
				if ((body3 = <>9__2) == null)
				{
					body3 = (<>9__2 = delegate(int j)
					{
						if (!converged[j])
						{
							rho0[j] = rho1[j];
							rho1[j] = BufferUtil.MultiplyAdd_GetSqrSum(this.R[j], -alpha[j], this.W[j]);
						}
					});
				}
				gParallel.ForEach<int>(source3, body3);
			}
			this.Iterations = num8;
			return num8 < this.MaxIterations;
		}

		public bool SolvePreconditioned()
		{
			this.Iterations = 0;
			if (this.B == null || this.MultiplyF == null || this.PreconditionMultiplyF == null)
			{
				throw new Exception("SparseSymmetricCGMultipleRHS.SolvePreconditioned(): Must set B and MultiplyF and PreconditionMultiplyF!");
			}
			int num = this.B.Length;
			if (num == 0)
			{
				throw new Exception("SparseSymmetricCGMultipleRHS.SolvePreconditioned(): Need at least one RHS vector in B");
			}
			int n = this.B[0].Length;
			this.R = BufferUtil.AllocNxM(num, n);
			this.P = BufferUtil.AllocNxM(num, n);
			this.AP = BufferUtil.AllocNxM(num, n);
			this.Z = BufferUtil.AllocNxM(num, n);
			if (this.X == null || !this.UseXAsInitialGuess)
			{
				if (this.X == null)
				{
					this.X = BufferUtil.AllocNxM(num, n);
				}
				for (int i = 0; i < num; i++)
				{
					Array.Clear(this.X[i], 0, n);
					Array.Copy(this.B[i], this.R[i], n);
				}
			}
			else
			{
				this.InitializeR(this.R);
			}
			double[] array = new double[num];
			for (int j2 = 0; j2 < num; j2++)
			{
				array[j2] = BufferUtil.Dot(this.B[j2], this.B[j2]);
			}
			double[] array2 = new double[num];
			for (int k = 0; k < num; k++)
			{
				array2[k] = Math.Sqrt(array[k]);
			}
			this.MultiplyF(this.X, this.R);
			for (int l = 0; l < num; l++)
			{
				for (int m = 0; m < n; m++)
				{
					this.R[l][m] = this.B[l][m] - this.R[l][m];
				}
			}
			this.PreconditionMultiplyF(this.R, this.Z);
			for (int n2 = 0; n2 < num; n2++)
			{
				Array.Copy(this.Z[n2], this.P[n2], n);
			}
			double[] RdotZ_k = new double[num];
			for (int num2 = 0; num2 < num; num2++)
			{
				RdotZ_k[num2] = BufferUtil.Dot(this.R[num2], this.Z[num2]);
			}
			double[] alpha_k = new double[num];
			double[] beta_k = new double[num];
			bool[] converged = new bool[num];
			Interval1i interval1i = Interval1i.Range(num);
			int num3 = 0;
			Action<int> <>9__0;
			Action<int> <>9__1;
			Action<int> <>9__2;
			Action<int> <>9__3;
			Action<int> <>9__4;
			Action<int> <>9__5;
			while (num3++ < this.MaxIterations)
			{
				bool flag = true;
				for (int num4 = 0; num4 < num; num4++)
				{
					if (!converged[num4] && Math.Sqrt(RdotZ_k[num4]) <= this.ConvergeTolerance * array2[num4])
					{
						converged[num4] = true;
					}
					if (!converged[num4])
					{
						flag = false;
					}
				}
				if (flag)
				{
					break;
				}
				this.MultiplyF(this.P, this.AP);
				IEnumerable<int> source = interval1i;
				Action<int> body;
				if ((body = <>9__0) == null)
				{
					body = (<>9__0 = delegate(int j)
					{
						if (!converged[j])
						{
							alpha_k[j] = RdotZ_k[j] / BufferUtil.Dot(this.P[j], this.AP[j]);
						}
					});
				}
				gParallel.ForEach<int>(source, body);
				IEnumerable<int> source2 = interval1i;
				Action<int> body2;
				if ((body2 = <>9__1) == null)
				{
					body2 = (<>9__1 = delegate(int j)
					{
						if (!converged[j])
						{
							BufferUtil.MultiplyAdd(this.X[j], alpha_k[j], this.P[j]);
						}
					});
				}
				gParallel.ForEach<int>(source2, body2);
				IEnumerable<int> source3 = interval1i;
				Action<int> body3;
				if ((body3 = <>9__2) == null)
				{
					body3 = (<>9__2 = delegate(int j)
					{
						if (!converged[j])
						{
							BufferUtil.MultiplyAdd(this.R[j], -alpha_k[j], this.AP[j]);
						}
					});
				}
				gParallel.ForEach<int>(source3, body3);
				this.PreconditionMultiplyF(this.R, this.Z);
				IEnumerable<int> source4 = interval1i;
				Action<int> body4;
				if ((body4 = <>9__3) == null)
				{
					body4 = (<>9__3 = delegate(int j)
					{
						if (!converged[j])
						{
							beta_k[j] = BufferUtil.Dot(this.Z[j], this.R[j]) / RdotZ_k[j];
						}
					});
				}
				gParallel.ForEach<int>(source4, body4);
				IEnumerable<int> source5 = interval1i;
				Action<int> body5;
				if ((body5 = <>9__4) == null)
				{
					body5 = (<>9__4 = delegate(int j)
					{
						if (!converged[j])
						{
							for (int num5 = 0; num5 < n; num5++)
							{
								this.P[j][num5] = this.Z[j][num5] + beta_k[j] * this.P[j][num5];
							}
						}
					});
				}
				gParallel.ForEach<int>(source5, body5);
				IEnumerable<int> source6 = interval1i;
				Action<int> body6;
				if ((body6 = <>9__5) == null)
				{
					body6 = (<>9__5 = delegate(int j)
					{
						if (!converged[j])
						{
							RdotZ_k[j] = BufferUtil.Dot(this.R[j], this.Z[j]);
						}
					});
				}
				gParallel.ForEach<int>(source6, body6);
			}
			this.Iterations = num3;
			return num3 < this.MaxIterations;
		}

		private void UpdateP(double[][] P, double[] beta, double[][] R, bool[] converged)
		{
			gParallel.ForEach<int>(Interval1i.Range(P.Length), delegate(int j)
			{
				if (!converged[j])
				{
					int num = P[j].Length;
					for (int i = 0; i < num; i++)
					{
						P[j][i] = R[j][i] + beta[j] * P[j][i];
					}
				}
			});
		}

		private void InitializeR(double[][] R)
		{
			this.MultiplyF(this.X, R);
			for (int i = 0; i < this.X.Length; i++)
			{
				int num = R[i].Length;
				for (int j = 0; j < num; j++)
				{
					R[i][j] = this.B[i][j] - R[i][j];
				}
			}
		}

		public Action<double[][], double[][]> MultiplyF;

		public Action<double[][], double[][]> PreconditionMultiplyF;

		public double[][] B;

		public double ConvergeTolerance = 1E-08;

		public double[][] X;

		public bool UseXAsInitialGuess = true;

		public int MaxIterations = 1024;

		public int Iterations;

		private double[][] R;

		private double[][] P;

		private double[][] W;

		private double[][] AP;

		private double[][] Z;
	}
}
