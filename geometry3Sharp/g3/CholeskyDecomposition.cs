using System;
using System.Collections.Generic;

namespace g3
{
	public class CholeskyDecomposition
	{
		public CholeskyDecomposition(DenseMatrix m)
		{
			this.A = m;
		}

		public bool Compute()
		{
			if (this.A.Rows != this.A.Columns)
			{
				throw new Exception("CholeskyDecomposition.Compute(): cannot be applied to non-square matrix");
			}
			int rows = this.A.Rows;
			this.L = new DenseMatrix(rows, rows);
			double[] buffer = this.L.Buffer;
			this.L[0, 0] = Math.Sqrt(this.A[0, 0]);
			for (int i = 1; i < rows; i++)
			{
				this.L[i, 0] = this.A[i, 0] / this.L[0, 0];
				double num = this.L[i, 0] * this.L[i, 0];
				for (int j = 1; j < i; j++)
				{
					double num2 = 0.0;
					int num3 = i * rows;
					int k = j * rows;
					int num4 = k + j;
					while (k < num4)
					{
						num2 += buffer[num3++] * buffer[k++];
					}
					this.L[i, j] = 1.0 / this.L[j, j] * (this.A[i, j] - num2);
					num += this.L[i, j] * this.L[i, j];
				}
				this.L[i, i] = Math.Sqrt(this.A[i, i] - num);
			}
			return true;
		}

		public bool ComputeParallel()
		{
			CholeskyDecomposition.<>c__DisplayClass4_0 CS$<>8__locals1 = new CholeskyDecomposition.<>c__DisplayClass4_0();
			CS$<>8__locals1.<>4__this = this;
			if (this.A.Rows != this.A.Columns)
			{
				throw new Exception("CholeskyDecomposition.ComputeParallel(): cannot be applied to non-square matrix");
			}
			CS$<>8__locals1.N = this.A.Rows;
			this.L = new DenseMatrix(CS$<>8__locals1.N, CS$<>8__locals1.N);
			CS$<>8__locals1.Lbuf = this.L.Buffer;
			CS$<>8__locals1.compute_diag = delegate(int r)
			{
				double num2 = 0.0;
				int num3 = r * CS$<>8__locals1.N;
				int num4 = num3 + r;
				do
				{
					num2 += CS$<>8__locals1.Lbuf[num3] * CS$<>8__locals1.Lbuf[num3];
				}
				while (num3++ < num4);
				CS$<>8__locals1.<>4__this.L[r, r] = Math.Sqrt(CS$<>8__locals1.<>4__this.A[r, r] - num2);
			};
			this.L[0, 0] = Math.Sqrt(this.A[0, 0]);
			for (int i = 1; i < CS$<>8__locals1.N; i++)
			{
				this.L[i, 0] = this.A[i, 0] / this.L[0, 0];
			}
			CS$<>8__locals1.compute_diag(1);
			int c;
			int c2;
			for (c = 1; c < CS$<>8__locals1.N; c = c2)
			{
				int num = CS$<>8__locals1.N - 1 - (c + 1);
				gParallel.BlockStartEnd(c + 1, CS$<>8__locals1.N - 1, delegate(int a, int b)
				{
					for (int j = a; j <= b; j++)
					{
						double num2 = 0.0;
						int num3 = j * CS$<>8__locals1.N;
						int k = c * CS$<>8__locals1.N;
						int num4 = k + c;
						while (k < num4)
						{
							num2 += CS$<>8__locals1.Lbuf[num3++] * CS$<>8__locals1.Lbuf[k++];
						}
						CS$<>8__locals1.<>4__this.L[j, c] = 1.0 / CS$<>8__locals1.<>4__this.L[c, c] * (CS$<>8__locals1.<>4__this.A[j, c] - num2);
						if (j == c + 1)
						{
							CS$<>8__locals1.compute_diag(j);
						}
					}
				}, Math.Max(num / 20, 1), false);
				c2 = c + 1;
			}
			return true;
		}

		private IEnumerable<Vector2i> diag_itr()
		{
			int N = this.A.Rows;
			int num;
			for (int r = 2; r < N; r = num + 1)
			{
				Vector2i rj = new Vector2i(r - 1, 1);
				while (rj.y <= rj.x)
				{
					yield return rj;
					rj.x--;
					rj.y++;
				}
				num = r;
			}
			for (int r = 1; r < N; r = num + 1)
			{
				Vector2i rj = new Vector2i(N - 1, r);
				while (rj.y <= rj.x)
				{
					yield return rj;
					rj.x--;
					rj.y++;
				}
				num = r;
			}
			yield break;
		}

		public void Solve(double[] B, double[] X, double[] Y)
		{
			int rows = this.A.Rows;
			if (Y == null)
			{
				Y = new double[rows];
			}
			Y[0] = B[0] / this.L[0, 0];
			for (int i = 1; i < rows; i++)
			{
				double num = 0.0;
				for (int j = 0; j < i; j++)
				{
					num += this.L[i, j] * Y[j];
				}
				Y[i] = (B[i] - num) / this.L[i, i];
			}
			X[rows - 1] = Y[rows - 1] / this.L[rows - 1, rows - 1];
			for (int k = rows - 2; k >= 0; k--)
			{
				double num2 = 0.0;
				for (int l = k + 1; l < rows; l++)
				{
					num2 += this.L[l, k] * X[l];
				}
				X[k] = (Y[k] - num2) / this.L[k, k];
			}
		}

		public DenseMatrix A;

		public DenseMatrix L;
	}
}
