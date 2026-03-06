using System;

namespace g3
{
	public class DiagonalMatrix
	{
		public DiagonalMatrix(int N)
		{
			this.D = new double[N];
		}

		public void Clear()
		{
			Array.Clear(this.D, 0, this.D.Length);
		}

		public void Set(int r, int c, double value)
		{
			if (r == c)
			{
				this.D[r] = value;
				return;
			}
			throw new Exception("DiagonalMatrix.Set: tried to set off-diagonal entry!");
		}

		public int Rows
		{
			get
			{
				return this.D.Length;
			}
		}

		public int Columns
		{
			get
			{
				return this.D.Length;
			}
		}

		public Index2i Size
		{
			get
			{
				return new Index2i(this.D.Length, this.D.Length);
			}
		}

		public double this[int r, int c]
		{
			get
			{
				return this.D[r];
			}
			set
			{
				this.Set(r, c, value);
			}
		}

		public void Multiply(double[] X, double[] Result)
		{
			for (int i = 0; i < X.Length; i++)
			{
				Result[i] = this.D[i] * X[i];
			}
		}

		public double[] D;
	}
}
