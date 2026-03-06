using System;
using System.Collections.Generic;

namespace g3
{
	public class SymmetricSparseMatrix : IMatrix
	{
		public SymmetricSparseMatrix(int setN = 0)
		{
			this.N = setN;
		}

		public SymmetricSparseMatrix(DenseMatrix m)
		{
			if (m.Rows != m.Columns)
			{
				throw new Exception("SymmetricSparseMatrix(DenseMatrix): Matrix is not square!");
			}
			if (!m.IsSymmetric(2.220446049250313E-16))
			{
				throw new Exception("SymmetricSparseMatrix(DenseMatrix): Matrix is not symmetric!");
			}
			this.N = m.Rows;
			for (int i = 0; i < this.N; i++)
			{
				for (int j = i; j < this.N; j++)
				{
					this.Set(i, j, m[i, j]);
				}
			}
		}

		public SymmetricSparseMatrix(SymmetricSparseMatrix m)
		{
			this.N = m.N;
			this.d = new Dictionary<Index2i, double>(m.d);
		}

		public void Set(int r, int c, double value)
		{
			Index2i key = new Index2i(Math.Min(r, c), Math.Max(r, c));
			this.d[key] = value;
			if (r >= this.N)
			{
				this.N = r + 1;
			}
			if (c >= this.N)
			{
				this.N = c + 1;
			}
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
				return this.N;
			}
		}

		public Index2i Size
		{
			get
			{
				return new Index2i(this.N, this.N);
			}
		}

		public double this[int r, int c]
		{
			get
			{
				Index2i key = new Index2i(Math.Min(r, c), Math.Max(r, c));
				double result;
				if (this.d.TryGetValue(key, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Set(r, c, value);
			}
		}

		public void Multiply(double[] X, double[] Result)
		{
			Array.Clear(Result, 0, Result.Length);
			foreach (KeyValuePair<Index2i, double> keyValuePair in this.d)
			{
				int a = keyValuePair.Key.a;
				int b = keyValuePair.Key.b;
				Result[a] += keyValuePair.Value * X[b];
				if (a != b)
				{
					Result[b] += keyValuePair.Value * X[a];
				}
			}
		}

		public SymmetricSparseMatrix Square(bool bParallel = true)
		{
			SymmetricSparseMatrix R = new SymmetricSparseMatrix(0);
			PackedSparseMatrix M = new PackedSparseMatrix(this, false);
			M.Sort(true);
			if (bParallel)
			{
				gParallel.ForEach<int>(Interval1i.Range(this.N), delegate(int r1i)
				{
					for (int k = r1i; k < this.N; k++)
					{
						double value2 = M.DotRowColumn(r1i, k, M);
						if (Math.Abs(value2) > 1E-08)
						{
							SymmetricSparseMatrix r = R;
							lock (r)
							{
								R[r1i, k] = value2;
							}
						}
					}
				});
			}
			else
			{
				for (int i = 0; i < this.N; i++)
				{
					for (int j = i; j < this.N; j++)
					{
						double value = M.DotRowColumn(i, j, M);
						if (Math.Abs(value) > 1E-08)
						{
							R[i, j] = value;
						}
					}
				}
			}
			return R;
		}

		public PackedSparseMatrix SquarePackedParallel()
		{
			PackedSparseMatrix packedSparseMatrix = new PackedSparseMatrix(this, false);
			packedSparseMatrix.Sort(true);
			return packedSparseMatrix.Square();
		}

		public SymmetricSparseMatrix Multiply(SymmetricSparseMatrix M2)
		{
			SymmetricSparseMatrix result = new SymmetricSparseMatrix(0);
			this.Multiply(M2, ref result, true);
			return result;
		}

		public void Multiply(SymmetricSparseMatrix M2, ref SymmetricSparseMatrix R, bool bParallel = true)
		{
			this.multiply_fast(M2, ref R, bParallel);
		}

		private void multiply_fast(SymmetricSparseMatrix M2in, ref SymmetricSparseMatrix Rin, bool bParallel)
		{
			int N = this.Rows;
			if (M2in.Rows != N)
			{
				throw new Exception("SymmetricSparseMatrix.Multiply: matrices have incompatible dimensions");
			}
			if (Rin == null)
			{
				Rin = new SymmetricSparseMatrix(0);
			}
			SymmetricSparseMatrix R = Rin;
			PackedSparseMatrix M = new PackedSparseMatrix(this, false);
			M.Sort(true);
			PackedSparseMatrix M2 = new PackedSparseMatrix(M2in, true);
			M2.Sort(true);
			if (bParallel)
			{
				gParallel.ForEach<int>(Interval1i.Range(N), delegate(int r1i)
				{
					for (int k = r1i; k < N; k++)
					{
						double value2 = M.DotRowColumn(r1i, k, M2);
						if (Math.Abs(value2) > 1E-08)
						{
							SymmetricSparseMatrix r = R;
							lock (r)
							{
								R[r1i, k] = value2;
							}
						}
					}
				});
				return;
			}
			for (int i = 0; i < N; i++)
			{
				for (int j = i; j < N; j++)
				{
					double value = M.DotRowColumn(i, j, M2);
					if (Math.Abs(value) > 1E-08)
					{
						R[i, j] = value;
					}
				}
			}
		}

		private void multiply_slow(SymmetricSparseMatrix M2, ref SymmetricSparseMatrix R)
		{
			int rows = this.Rows;
			if (M2.Rows != rows)
			{
				throw new Exception("SymmetricSparseMatrix.Multiply: matrices have incompatible dimensions");
			}
			if (R == null)
			{
				R = new SymmetricSparseMatrix(0);
			}
			List<SymmetricSparseMatrix.mval> list = new List<SymmetricSparseMatrix.mval>(128);
			for (int i = 0; i < rows; i++)
			{
				list.Clear();
				this.get_row_nonzeros(i, list);
				int count = list.Count;
				for (int j = i; j < rows; j++)
				{
					double num = 0.0;
					for (int k = 0; k < count; k++)
					{
						int k2 = list[k].k;
						num += list[k].v * M2[k2, j];
					}
					if (Math.Abs(num) > 1E-08)
					{
						R[i, j] = num;
					}
				}
			}
		}

		public IEnumerable<KeyValuePair<Index2i, double>> NonZeros()
		{
			return this.d;
		}

		public IEnumerable<Index2i> NonZeroIndices()
		{
			return this.d.Keys;
		}

		public bool EpsilonEqual(SymmetricSparseMatrix B, double eps = 2.220446049250313E-16)
		{
			foreach (KeyValuePair<Index2i, double> keyValuePair in this.d)
			{
				if (Math.Abs(B[keyValuePair.Key.a, keyValuePair.Key.b] - keyValuePair.Value) > eps)
				{
					return false;
				}
			}
			foreach (KeyValuePair<Index2i, double> keyValuePair2 in B.d)
			{
				if (Math.Abs(this[keyValuePair2.Key.a, keyValuePair2.Key.b] - keyValuePair2.Value) > eps)
				{
					return false;
				}
			}
			return true;
		}

		private void get_row_nonzeros(int r, List<SymmetricSparseMatrix.mval> buf)
		{
			int rows = this.Rows;
			for (int i = 0; i < rows; i++)
			{
				double num = this[r, i];
				if (num != 0.0)
				{
					buf.Add(new SymmetricSparseMatrix.mval
					{
						k = i,
						v = num
					});
				}
			}
		}

		private Dictionary<Index2i, double> d = new Dictionary<Index2i, double>();

		private int N;

		private struct mval
		{
			public int k;

			public double v;
		}
	}
}
