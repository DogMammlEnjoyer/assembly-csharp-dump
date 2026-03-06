using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class PackedSparseMatrix
	{
		public PackedSparseMatrix(PackedSparseMatrix copy)
		{
			int num = copy.Rows.Length;
			this.Rows = new PackedSparseMatrix.nonzero[num][];
			for (int i = 0; i < num; i++)
			{
				this.Rows[i] = new PackedSparseMatrix.nonzero[copy.Rows[i].Length];
				Array.Copy(copy.Rows[i], this.Rows[i], this.Rows[i].Length);
			}
			this.Columns = copy.Columns;
			this.Sorted = copy.Sorted;
			this.NumNonZeros = copy.NumNonZeros;
			this.StorageMode = copy.StorageMode;
			this.IsSymmetric = copy.IsSymmetric;
		}

		public PackedSparseMatrix(SymmetricSparseMatrix m, bool bTranspose = false)
		{
			int num = bTranspose ? m.Columns : m.Rows;
			this.Columns = (bTranspose ? m.Columns : m.Rows);
			this.Rows = new PackedSparseMatrix.nonzero[num][];
			int[] array = new int[num];
			foreach (Index2i index2i in m.NonZeroIndices())
			{
				array[index2i.a]++;
				if (index2i.a != index2i.b)
				{
					array[index2i.b]++;
				}
			}
			this.NumNonZeros = 0;
			for (int i = 0; i < num; i++)
			{
				this.Rows[i] = new PackedSparseMatrix.nonzero[array[i]];
				this.NumNonZeros += array[i];
			}
			int[] array2 = new int[num];
			foreach (KeyValuePair<Index2i, double> keyValuePair in m.NonZeros())
			{
				int num2 = keyValuePair.Key.a;
				int num3 = keyValuePair.Key.b;
				if (bTranspose)
				{
					int num4 = num2;
					num2 = num3;
					num3 = num4;
				}
				int[] array3 = array2;
				int num5 = num2;
				int num6 = array3[num5];
				array3[num5] = num6 + 1;
				int num7 = num6;
				this.Rows[num2][num7].j = num3;
				this.Rows[num2][num7].d = keyValuePair.Value;
				if (num2 != num3)
				{
					int[] array4 = array2;
					int num8 = num3;
					num6 = array4[num8];
					array4[num8] = num6 + 1;
					num7 = num6;
					this.Rows[num3][num7].j = num2;
					this.Rows[num3][num7].d = keyValuePair.Value;
				}
			}
			this.Sorted = false;
			this.IsSymmetric = true;
			this.StorageMode = PackedSparseMatrix.StorageModes.Full;
		}

		public PackedSparseMatrix(DVector<matrix_entry> entries, int numRows, int numCols, bool bSymmetric = true)
		{
			this.Columns = numCols;
			this.Rows = new PackedSparseMatrix.nonzero[numRows][];
			int size = entries.size;
			int[] array = new int[numRows];
			for (int i = 0; i < size; i++)
			{
				array[entries[i].r]++;
				if (bSymmetric && entries[i].r != entries[i].c)
				{
					array[entries[i].c]++;
				}
			}
			this.NumNonZeros = 0;
			for (int j = 0; j < numRows; j++)
			{
				this.Rows[j] = new PackedSparseMatrix.nonzero[array[j]];
				this.NumNonZeros += array[j];
			}
			int[] array2 = new int[numRows];
			for (int k = 0; k < size; k++)
			{
				matrix_entry matrix_entry = entries[k];
				int[] array3 = array2;
				int r = matrix_entry.r;
				int num = array3[r];
				array3[r] = num + 1;
				int num2 = num;
				this.Rows[matrix_entry.r][num2].j = matrix_entry.c;
				this.Rows[matrix_entry.r][num2].d = matrix_entry.value;
				if (bSymmetric && matrix_entry.c != matrix_entry.r)
				{
					int[] array4 = array2;
					int c = matrix_entry.c;
					num = array4[c];
					array4[c] = num + 1;
					num2 = num;
					this.Rows[matrix_entry.c][num2].j = matrix_entry.r;
					this.Rows[matrix_entry.c][num2].d = matrix_entry.value;
				}
			}
			this.Sorted = false;
			this.IsSymmetric = bSymmetric;
			this.StorageMode = PackedSparseMatrix.StorageModes.Full;
		}

		public static PackedSparseMatrix FromDense(DenseMatrix m, bool bSymmetric)
		{
			DVector<matrix_entry> dvector = new DVector<matrix_entry>();
			for (int i = 0; i < m.Rows; i++)
			{
				int num = bSymmetric ? (i + 1) : m.Columns;
				for (int j = 0; j < num; j++)
				{
					if (m[i, j] != 0.0)
					{
						dvector.Add(new matrix_entry
						{
							r = i,
							c = j,
							value = m[i, j]
						});
					}
				}
			}
			return new PackedSparseMatrix(dvector, m.Rows, m.Columns, bSymmetric);
		}

		public double this[int r, int c]
		{
			get
			{
				PackedSparseMatrix.nonzero[] array = this.Rows[r];
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					if (array[i].j == c)
					{
						return array[i].d;
					}
				}
				return 0.0;
			}
			set
			{
				PackedSparseMatrix.nonzero[] array = this.Rows[r];
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					if (array[i].j == c)
					{
						array[i].d = value;
						return;
					}
				}
				throw new Exception(string.Concat(new string[]
				{
					"PackedSparseMatrix[r,c]: value at index ",
					r.ToString(),
					",",
					c.ToString(),
					" does not exist!"
				}));
			}
		}

		public void Sort(bool bParallel = true)
		{
			if (bParallel)
			{
				gParallel.BlockStartEnd(0, this.Rows.Length - 1, delegate(int a, int b)
				{
					for (int j = a; j <= b; j++)
					{
						Array.Sort<PackedSparseMatrix.nonzero>(this.Rows[j], (PackedSparseMatrix.nonzero x, PackedSparseMatrix.nonzero y) => x.j.CompareTo(y.j));
					}
				}, -1, false);
			}
			else
			{
				for (int i = 0; i < this.Rows.Length; i++)
				{
					Array.Sort<PackedSparseMatrix.nonzero>(this.Rows[i], (PackedSparseMatrix.nonzero x, PackedSparseMatrix.nonzero y) => x.j.CompareTo(y.j));
				}
			}
			this.Sorted = true;
		}

		public Interval1i NonZerosRange(int r)
		{
			PackedSparseMatrix.nonzero[] array = this.Rows[r];
			if (array.Length == 0)
			{
				return Interval1i.Empty;
			}
			if (!this.Sorted)
			{
				Interval1i empty = Interval1i.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					empty.Contain(array[i].j);
				}
				return empty;
			}
			return new Interval1i(array[0].j, array[array.Length - 1].j);
		}

		public IEnumerable<Vector2i> NonZeroIndicesByRow(bool bWantSorted = true)
		{
			if (bWantSorted && !this.Sorted)
			{
				throw new Exception("PackedSparseMatrix.NonZeroIndicesByRow: sorting requested but not available");
			}
			int N = this.Rows.Length;
			int num;
			for (int r = 0; r < N; r = num)
			{
				PackedSparseMatrix.nonzero[] Row = this.Rows[r];
				for (int i = 0; i < Row.Length; i = num)
				{
					yield return new Vector2i(r, Row[i].j);
					num = i + 1;
				}
				Row = null;
				num = r + 1;
			}
			yield break;
		}

		public IEnumerable<Vector2i> NonZeroIndicesForRow(int r, bool bWantSorted = true)
		{
			if (bWantSorted && !this.Sorted)
			{
				throw new Exception("PackedSparseMatrix.NonZeroIndicesByRow: sorting requested but not available");
			}
			PackedSparseMatrix.nonzero[] Row = this.Rows[r];
			int num;
			for (int i = 0; i < Row.Length; i = num)
			{
				yield return new Vector2i(r, Row[i].j);
				num = i + 1;
			}
			yield break;
		}

		public void Multiply(double[] X, double[] Result)
		{
			Array.Clear(Result, 0, Result.Length);
			for (int i = 0; i < this.Rows.Length; i++)
			{
				int num = this.Rows[i].Length;
				for (int j = 0; j < num; j++)
				{
					int j2 = this.Rows[i][j].j;
					Result[i] += this.Rows[i][j].d * X[j2];
				}
			}
		}

		public void Multiply_Parallel(double[] X, double[] Result)
		{
			gParallel.BlockStartEnd(0, this.Rows.Length - 1, delegate(int i_start, int i_end)
			{
				for (int i = i_start; i <= i_end; i++)
				{
					Result[i] = 0.0;
					PackedSparseMatrix.nonzero[] array = this.Rows[i];
					int num = array.Length;
					for (int j = 0; j < num; j++)
					{
						Result[i] += array[j].d * X[array[j].j];
					}
				}
			}, -1, false);
		}

		public void Multiply_Parallel_3(double[][] X, double[][] Result)
		{
			int num = X.Length;
			gParallel.BlockStartEnd(0, this.Rows.Length - 1, delegate(int i_start, int i_end)
			{
				for (int i = i_start; i <= i_end; i++)
				{
					Result[0][i] = (Result[1][i] = (Result[2][i] = 0.0));
					PackedSparseMatrix.nonzero[] array = this.Rows[i];
					int num2 = array.Length;
					for (int j = 0; j < num2; j++)
					{
						int j2 = array[j].j;
						double d = array[j].d;
						Result[0][i] += d * X[0][j2];
						Result[1][i] += d * X[1][j2];
						Result[2][i] += d * X[2][j2];
					}
				}
			}, -1, false);
		}

		public double DotRowColumn(int r, int c, PackedSparseMatrix MTranspose)
		{
			if (!this.Sorted || !MTranspose.Sorted)
			{
				throw new Exception("PackedSparseMatrix.DotRowColumn: matrices must be sorted!");
			}
			if (this.Rows.Length != MTranspose.Rows.Length)
			{
				throw new Exception("PackedSparseMatrix.DotRowColumn: matrices are not the same size!");
			}
			int num = 0;
			int num2 = 0;
			PackedSparseMatrix.nonzero[] array = this.Rows[r];
			PackedSparseMatrix.nonzero[] array2 = MTranspose.Rows[c];
			int num3 = array.Length;
			int num4 = array2.Length;
			int j = array2[num4 - 1].j;
			int j2 = array[num3 - 1].j;
			double num5 = 0.0;
			while (num < num3 && num2 < num4 && array[num].j <= j && array2[num2].j <= j2)
			{
				if (array[num].j == array2[num2].j)
				{
					num5 += array[num].d * array2[num2].d;
					num++;
					num2++;
				}
				else if (array[num].j < array2[num2].j)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
			return num5;
		}

		public double DotRowSelf(int r)
		{
			PackedSparseMatrix.nonzero[] array = this.Rows[r];
			double num = 0.0;
			for (int i = 0; i < array.Length; i++)
			{
				num += array[i].d * array[i].d;
			}
			return num;
		}

		public void DotRowAllColumns(int r, double[] sums, int[] col_indices, PackedSparseMatrix MTranspose)
		{
			int num = this.Rows.Length;
			int i = 0;
			PackedSparseMatrix.nonzero[] array = this.Rows[r];
			int num2 = array.Length;
			Array.Clear(sums, 0, num);
			Array.Clear(col_indices, 0, num);
			while (i < num2)
			{
				int j = array[i].j;
				for (int k = 0; k < num; k++)
				{
					PackedSparseMatrix.nonzero[] array2 = MTranspose.Rows[k];
					int num3 = col_indices[k];
					if (num3 < array2.Length)
					{
						while (num3 < array2.Length && array2[num3].j < j)
						{
							num3++;
						}
						if (num3 < array2.Length && j == array2[num3].j)
						{
							sums[k] += array[i].d * array2[num3].d;
							num3++;
						}
						col_indices[k] = num3;
					}
				}
				i++;
			}
		}

		public double DotRows(int r1, int r2, int MaxCol = 2147483647)
		{
			if (!this.Sorted)
			{
				throw new Exception("PackedSparseMatrix.DotRows: matrices must be sorted!");
			}
			MaxCol = Math.Min(MaxCol, this.Columns);
			int num = 0;
			int num2 = 0;
			PackedSparseMatrix.nonzero[] array = this.Rows[r1];
			PackedSparseMatrix.nonzero[] array2 = this.Rows[r2];
			int num3 = array.Length;
			int num4 = array2.Length;
			double num5 = 0.0;
			while (num < num3 && num2 < num4 && array[num].j <= MaxCol && array2[num2].j <= MaxCol)
			{
				if (array[num].j == array2[num2].j)
				{
					num5 += array[num].d * array2[num2].d;
					num++;
					num2++;
				}
				else if (array[num].j < array2[num2].j)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
			return num5;
		}

		public double DotRowVector(int r, double[] vec, int MaxCol = 2147483647)
		{
			if (!this.Sorted && MaxCol < 2147483647)
			{
				throw new Exception("PackedSparseMatrix.DotRows: matrices must be sorted if MaxCol is specified!");
			}
			MaxCol = Math.Min(MaxCol, this.Columns);
			PackedSparseMatrix.nonzero[] array = this.Rows[r];
			double num = 0.0;
			int num2 = 0;
			while (num2 < array.Length && array[num2].j <= MaxCol)
			{
				num += array[num2].d * vec[array[num2].j];
				num2++;
			}
			return num;
		}

		public double DotColumnVector(int c, double[] vec, int start_row = 0, int end_row = 2147483647)
		{
			int num = this.Rows.Length;
			double num2 = 0.0;
			if (this.Sorted)
			{
				for (int i = start_row; i <= end_row; i++)
				{
					PackedSparseMatrix.nonzero[] array = this.Rows[i];
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].j == c)
						{
							num2 += array[j].d * vec[i];
							break;
						}
						if (array[j].j > c)
						{
							break;
						}
					}
				}
			}
			else
			{
				for (int k = start_row; k <= end_row; k++)
				{
					PackedSparseMatrix.nonzero[] array2 = this.Rows[k];
					for (int l = 0; l < array2.Length; l++)
					{
						if (array2[l].j == c)
						{
							num2 += array2[l].d * vec[k];
							break;
						}
					}
				}
			}
			return num2;
		}

		public PackedSparseMatrix Square()
		{
			if (this.Rows.Length != this.Columns)
			{
				throw new Exception("PackedSparseMatrix.Square: matrix is not square!");
			}
			int columns = this.Columns;
			DVector<matrix_entry> entries = new DVector<matrix_entry>();
			SpinLock entries_lock = default(SpinLock);
			gParallel.BlockStartEnd(0, columns - 1, delegate(int r_start, int r_end)
			{
				for (int i = r_start; i <= r_end; i++)
				{
					HashSet<int> hashSet = new HashSet<int>();
					hashSet.Add(i);
					PackedSparseMatrix.nonzero[] array = this.Rows[i];
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].j > i)
						{
							hashSet.Add(array[j].j);
						}
						PackedSparseMatrix.nonzero[] array2 = this.Rows[array[j].j];
						for (int k = 0; k < array2.Length; k++)
						{
							if (array2[k].j > i)
							{
								hashSet.Add(array2[k].j);
							}
						}
					}
					foreach (int c in hashSet)
					{
						double value = this.DotRowColumn(i, c, this);
						if (Math.Abs(value) > 1E-08)
						{
							bool flag = false;
							entries_lock.Enter(ref flag);
							entries.Add(new matrix_entry
							{
								r = i,
								c = c,
								value = value
							});
							entries_lock.Exit();
						}
					}
				}
			}, -1, false);
			return new PackedSparseMatrix(entries, columns, columns, true);
		}

		public double FrobeniusNorm
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.Rows.Length; i++)
				{
					PackedSparseMatrix.nonzero[] array = this.Rows[i];
					for (int j = 0; j < array.Length; j++)
					{
						num += array[j].d * array[j].d;
					}
				}
				return Math.Sqrt(num);
			}
		}

		public double MaxNorm
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.Rows.Length; i++)
				{
					PackedSparseMatrix.nonzero[] array = this.Rows[i];
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].d > num)
						{
							num = array[j].d;
						}
					}
				}
				return num;
			}
		}

		public double Trace
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.Rows.Length; i++)
				{
					PackedSparseMatrix.nonzero[] array = this.Rows[i];
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].j == i)
						{
							num += array[j].d;
						}
					}
				}
				return num;
			}
		}

		public string MatrixInfo(bool bExtended = false)
		{
			string text = string.Format("Rows {0}  Cols {1}   NonZeros {2}  Sorted {3}", new object[]
			{
				this.Rows.Length,
				this.Columns,
				this.NumNonZeros,
				this.Sorted
			});
			if (bExtended)
			{
				double num = 0.0;
				foreach (PackedSparseMatrix.nonzero[] array in this.Rows)
				{
					foreach (PackedSparseMatrix.nonzero nonzero in array)
					{
						num += nonzero.d;
					}
				}
				text += string.Format("  Sum {0}  Frobenius {1}  Max {2}  Trace {3}", new object[]
				{
					num,
					this.FrobeniusNorm,
					this.MaxNorm,
					this.Trace
				});
			}
			return text;
		}

		public PackedSparseMatrix.nonzero[][] Rows;

		public int Columns;

		public bool Sorted;

		public int NumNonZeros;

		public PackedSparseMatrix.StorageModes StorageMode;

		public bool IsSymmetric;

		public struct nonzero
		{
			public int j;

			public double d;
		}

		public enum StorageModes
		{
			Full
		}
	}
}
