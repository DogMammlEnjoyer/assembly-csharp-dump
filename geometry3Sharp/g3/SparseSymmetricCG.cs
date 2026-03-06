using System;

namespace g3
{
	public class SparseSymmetricCG
	{
		public bool Solve()
		{
			this.Iterations = 0;
			int num = this.B.Length;
			this.R = new double[num];
			this.P = new double[num];
			this.AP = new double[num];
			if (this.X == null || !this.UseXAsInitialGuess)
			{
				if (this.X == null)
				{
					this.X = new double[num];
				}
				Array.Clear(this.X, 0, this.X.Length);
				Array.Copy(this.B, this.R, this.B.Length);
			}
			else
			{
				this.InitializeR(this.R);
			}
			double num2 = Math.Sqrt(BufferUtil.Dot(this.B, this.B));
			double num3 = BufferUtil.Dot(this.R, this.R);
			if (num3 < 1E-08 * num2)
			{
				return true;
			}
			Array.Copy(this.R, this.P, this.R.Length);
			this.MultiplyF(this.P, this.AP);
			double alpha = num3 / BufferUtil.Dot(this.P, this.AP);
			BufferUtil.MultiplyAdd(this.X, alpha, this.P);
			BufferUtil.MultiplyAdd(this.R, -alpha, this.AP);
			double num4 = BufferUtil.Dot(this.R, this.R);
			Action <>9__0;
			int i;
			for (i = 1; i < this.MaxIterations; i++)
			{
				if (Math.Sqrt(num4) <= 1E-08 * num2)
				{
					break;
				}
				double beta = num4 / num3;
				this.UpdateP(this.P, beta, this.R);
				this.MultiplyF(this.P, this.AP);
				alpha = num4 / BufferUtil.Dot(this.P, this.AP);
				double RdotR = 0.0;
				Action[] array = new Action[2];
				int num5 = 0;
				Action action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate()
					{
						BufferUtil.MultiplyAdd(this.X, alpha, this.P);
					});
				}
				array[num5] = action;
				array[1] = delegate()
				{
					RdotR = BufferUtil.MultiplyAdd_GetSqrSum(this.R, -alpha, this.AP);
				};
				gParallel.Evaluate(array);
				num3 = num4;
				num4 = RdotR;
			}
			this.Iterations = i;
			return i < this.MaxIterations;
		}

		private void UpdateP(double[] P, double beta, double[] R)
		{
			for (int i = 0; i < P.Length; i++)
			{
				P[i] = R[i] + beta * P[i];
			}
		}

		private void InitializeR(double[] R)
		{
			this.MultiplyF(this.X, R);
			for (int i = 0; i < this.X.Length; i++)
			{
				R[i] = this.B[i] - R[i];
			}
		}

		public bool SolvePreconditioned()
		{
			this.Iterations = 0;
			int n = this.B.Length;
			this.R = new double[n];
			this.P = new double[n];
			this.AP = new double[n];
			this.Z = new double[n];
			if (this.X == null || !this.UseXAsInitialGuess)
			{
				if (this.X == null)
				{
					this.X = new double[n];
				}
				Array.Clear(this.X, 0, this.X.Length);
				Array.Copy(this.B, this.R, this.B.Length);
			}
			else
			{
				this.InitializeR(this.R);
			}
			double num = Math.Sqrt(BufferUtil.Dot(this.B, this.B));
			this.MultiplyF(this.X, this.R);
			for (int i = 0; i < n; i++)
			{
				this.R[i] = this.B[i] - this.R[i];
			}
			this.PreconditionMultiplyF(this.R, this.Z);
			Array.Copy(this.Z, this.P, n);
			double RdotZ_k = BufferUtil.Dot(this.R, this.Z);
			int num2 = 0;
			Action <>9__3;
			while (num2++ < this.MaxIterations)
			{
				if (Math.Sqrt(RdotZ_k) <= 1E-08 * num)
				{
					break;
				}
				this.MultiplyF(this.P, this.AP);
				double alpha_k = RdotZ_k / BufferUtil.Dot(this.P, this.AP);
				gParallel.Evaluate(new Action[]
				{
					delegate()
					{
						BufferUtil.MultiplyAdd(this.X, alpha_k, this.P);
					},
					delegate()
					{
						BufferUtil.MultiplyAdd(this.R, -alpha_k, this.AP);
					}
				});
				this.PreconditionMultiplyF(this.R, this.Z);
				double beta_k = BufferUtil.Dot(this.Z, this.R) / RdotZ_k;
				Action[] array = new Action[2];
				array[0] = delegate()
				{
					for (int j = 0; j < n; j++)
					{
						this.P[j] = this.Z[j] + beta_k * this.P[j];
					}
				};
				int num3 = 1;
				Action action;
				if ((action = <>9__3) == null)
				{
					action = (<>9__3 = delegate()
					{
						RdotZ_k = BufferUtil.Dot(this.R, this.Z);
					});
				}
				array[num3] = action;
				gParallel.Evaluate(array);
			}
			this.Iterations = num2;
			return num2 < this.MaxIterations;
		}

		public Action<double[], double[]> MultiplyF;

		public Action<double[], double[]> PreconditionMultiplyF;

		public double[] B;

		public double[] X;

		public bool UseXAsInitialGuess = true;

		public int MaxIterations = 1024;

		public int Iterations;

		private double[] R;

		private double[] P;

		private double[] AP;

		private double[] Z;
	}
}
