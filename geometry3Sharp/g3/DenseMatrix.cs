using System;

namespace g3
{
	public class DenseMatrix : IMatrix
	{
		public DenseMatrix(int Nrows, int Mcols)
		{
			this.d = new double[Nrows * Mcols];
			Array.Clear(this.d, 0, this.d.Length);
			this.N = Nrows;
			this.M = Mcols;
		}

		public DenseMatrix(DenseMatrix copy)
		{
			this.N = copy.N;
			this.M = copy.M;
			this.d = new double[this.N * this.M];
			Array.Copy(copy.d, this.d, copy.d.Length);
		}

		public double[] Buffer
		{
			get
			{
				return this.d;
			}
		}

		public void Set(int r, int c, double value)
		{
			this.d[r * this.M + c] = value;
		}

		public void Set(double[] values)
		{
			if (values.Length != this.N * this.M)
			{
				throw new Exception("DenseMatrix.Set: incorrect length");
			}
			Array.Copy(values, this.d, this.d.Length);
		}

		public int Rows
		{
			get
			{
				return this.N;
			}
		}

		public int Columns
		{
			get
			{
				return this.M;
			}
		}

		public Index2i Size
		{
			get
			{
				return new Index2i(this.N, this.M);
			}
		}

		public int Length
		{
			get
			{
				return this.M * this.N;
			}
		}

		public double this[int r, int c]
		{
			get
			{
				return this.d[r * this.M + c];
			}
			set
			{
				this.d[r * this.M + c] = value;
			}
		}

		public double this[int i]
		{
			get
			{
				return this.d[i];
			}
			set
			{
				this.d[i] = value;
			}
		}

		public DenseVector Row(int r)
		{
			DenseVector denseVector = new DenseVector(this.M);
			int num = r * this.M;
			for (int i = 0; i < this.M; i++)
			{
				denseVector[i] = this.d[num + i];
			}
			return denseVector;
		}

		public DenseVector Column(int c)
		{
			DenseVector denseVector = new DenseVector(this.N);
			for (int i = 0; i < this.N; i++)
			{
				denseVector[i] = this.d[i * this.M + c];
			}
			return denseVector;
		}

		public DenseVector Diagonal()
		{
			if (this.M != this.N)
			{
				throw new Exception("DenseMatrix.Diagonal: matrix is not square!");
			}
			DenseVector denseVector = new DenseVector(this.N);
			for (int i = 0; i < this.N; i++)
			{
				denseVector[i] = this.d[i * this.M + i];
			}
			return denseVector;
		}

		public DenseMatrix Transpose()
		{
			DenseMatrix denseMatrix = new DenseMatrix(this.M, this.N);
			for (int i = 0; i < this.N; i++)
			{
				for (int j = 0; j < this.M; j++)
				{
					denseMatrix.d[j * this.M + i] = this.d[i * this.M + j];
				}
			}
			return denseMatrix;
		}

		public void TransposeInPlace()
		{
			if (this.N != this.M)
			{
				double[] array = new double[this.M * this.N];
				for (int i = 0; i < this.N; i++)
				{
					for (int j = 0; j < this.M; j++)
					{
						array[j * this.M + i] = this.d[i * this.M + j];
					}
				}
				this.d = array;
				int m = this.M;
				this.M = this.N;
				this.N = m;
				return;
			}
			for (int k = 0; k < this.N; k++)
			{
				for (int l = 0; l < this.M; l++)
				{
					if (l != k)
					{
						int num = k * this.M + l;
						int num2 = l * this.M + k;
						double num3 = this.d[num];
						this.d[num] = this.d[num2];
						this.d[num2] = num3;
					}
				}
			}
		}

		public bool IsSymmetric(double dTolerance = 2.220446049250313E-16)
		{
			if (this.M != this.N)
			{
				throw new Exception("DenseMatrix.IsSymmetric: matrix is not square!");
			}
			for (int i = 0; i < this.N; i++)
			{
				for (int j = 0; j < i; j++)
				{
					if (Math.Abs(this.d[i * this.M + j] - this.d[j * this.M + i]) > dTolerance)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsPositiveDefinite()
		{
			if (this.M != this.N)
			{
				throw new Exception("DenseMatrix.IsPositiveDefinite: matrix is not square!");
			}
			if (!this.IsSymmetric(2.220446049250313E-16))
			{
				throw new Exception("DenseMatrix.IsPositiveDefinite: matrix is not symmetric!");
			}
			for (int i = 0; i < this.N; i++)
			{
				double num = this.d[i * this.M + i];
				double num2 = 0.0;
				for (int j = 0; j < this.N; j++)
				{
					if (j != i)
					{
						num2 += Math.Abs(this.d[i * this.M + j]);
					}
				}
				if (num < 0.0 || num < num2)
				{
					return false;
				}
			}
			return true;
		}

		public bool EpsilonEquals(DenseMatrix m2, double epsilon = 1E-08)
		{
			if (this.N != m2.N || this.M != m2.M)
			{
				throw new Exception("DenseMatrix.Equals: matrices are not the same size!");
			}
			for (int i = 0; i < this.d.Length; i++)
			{
				if (Math.Abs(this.d[i] - m2.d[i]) > epsilon)
				{
					return false;
				}
			}
			return true;
		}

		public DenseVector Multiply(DenseVector X)
		{
			DenseVector denseVector = new DenseVector(X.Length);
			this.Multiply(X.Buffer, denseVector.Buffer);
			return denseVector;
		}

		public void Multiply(DenseVector X, DenseVector R)
		{
			this.Multiply(X.Buffer, R.Buffer);
		}

		public void Multiply(double[] X, double[] Result)
		{
			for (int i = 0; i < this.N; i++)
			{
				Result[i] = 0.0;
				int num = i * this.M;
				for (int j = 0; j < this.M; j++)
				{
					Result[i] += this.d[num + j] * X[j];
				}
			}
		}

		public void Add(DenseMatrix M2)
		{
			if (this.N != M2.N || this.M != M2.M)
			{
				throw new Exception("DenseMatrix.Add: matrices have incompatible dimensions");
			}
			for (int i = 0; i < this.d.Length; i++)
			{
				this.d[i] += M2.d[i];
			}
		}

		public void Add(IMatrix M2)
		{
			if (this.N != M2.Rows || this.M != M2.Columns)
			{
				throw new Exception("DenseMatrix.Add: matrices have incompatible dimensions");
			}
			for (int i = 0; i < this.N; i++)
			{
				for (int j = 0; j < this.M; j++)
				{
					this.d[i * this.M + j] += M2[i, j];
				}
			}
		}

		public void MulAdd(DenseMatrix M2, double s)
		{
			if (this.N != M2.N || this.M != M2.M)
			{
				throw new Exception("DenseMatrix.MulAdd: matrices have incompatible dimensions");
			}
			for (int i = 0; i < this.d.Length; i++)
			{
				this.d[i] += s * M2.d[i];
			}
		}

		public void MulAdd(IMatrix M2, double s)
		{
			if (this.N != M2.Rows || this.M != M2.Columns)
			{
				throw new Exception("DenseMatrix.MulAdd: matrices have incompatible dimensions");
			}
			for (int i = 0; i < this.N; i++)
			{
				for (int j = 0; j < this.M; j++)
				{
					this.d[i * this.M + j] += s * M2[i, j];
				}
			}
		}

		public DenseMatrix Multiply(DenseMatrix M2, bool bParallel = true)
		{
			DenseMatrix result = new DenseMatrix(this.Rows, M2.Columns);
			this.Multiply(M2, ref result, bParallel);
			return result;
		}

		public void Multiply(DenseMatrix M2, ref DenseMatrix R, bool bParallel = true)
		{
			int n = this.N;
			int cols1 = this.M;
			int n2 = M2.N;
			int cols2 = M2.M;
			if (cols1 != n2)
			{
				throw new Exception("DenseMatrix.Multiply: matrices have incompatible dimensions");
			}
			if (R == null)
			{
				R = new DenseMatrix(this.Rows, M2.Columns);
			}
			if (R.Rows != n || R.Columns != cols2)
			{
				throw new Exception("DenseMatrix.Multiply: Result matrix has incorrect dimensions");
			}
			if (bParallel)
			{
				DenseMatrix Rt = R;
				gParallel.ForEach<int>(Interval1i.Range(0, n), delegate(int r1i)
				{
					int num3 = r1i * this.M;
					for (int l = 0; l < cols2; l++)
					{
						double num4 = 0.0;
						for (int m = 0; m < cols1; m++)
						{
							num4 += this.d[num3 + m] * M2.d[m * this.M + l];
						}
						Rt[num3 + l] = num4;
					}
				});
				return;
			}
			for (int i = 0; i < n; i++)
			{
				int num = i * this.M;
				for (int j = 0; j < cols2; j++)
				{
					double num2 = 0.0;
					for (int k = 0; k < cols1; k++)
					{
						num2 += this.d[num + k] * M2.d[k * this.M + j];
					}
					R[num + j] = num2;
				}
			}
		}

		private double[] d;

		private int N;

		private int M;
	}
}
