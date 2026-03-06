using System;
using System.Collections.Generic;

namespace g3
{
	public class SymmetricEigenSolver
	{
		public SymmetricEigenSolver(int size, int maxIterations)
		{
			this.mSize = (this.mMaxIterations = 0);
			this.mIsRotation = -1;
			if (size > 1 && maxIterations > 0)
			{
				this.mSize = size;
				this.mMaxIterations = maxIterations;
				this.mMatrix = new double[size * size];
				this.mDiagonal = new double[size];
				this.mSuperdiagonal = new double[size - 1];
				this.mGivens = new List<SymmetricEigenSolver.GivensRotation>(maxIterations * (size - 1));
				this.mPermutation = new int[size];
				this.mVisited = new int[size];
				this.mPVector = new double[size];
				this.mVVector = new double[size];
				this.mWVector = new double[size];
			}
		}

		public int Solve(double[] input, SymmetricEigenSolver.SortType eSort)
		{
			if (this.mSize > 0)
			{
				Array.Copy(input, this.mMatrix, this.mSize * this.mSize);
				this.Tridiagonalize();
				this.mGivens.Clear();
				for (int i = 0; i < this.mMaxIterations; i++)
				{
					int num = -1;
					int num2 = -1;
					for (int j = this.mSize - 2; j >= 0; j--)
					{
						double value = this.mDiagonal[j];
						double value2 = this.mSuperdiagonal[j];
						double value3 = this.mDiagonal[j + 1];
						double num3 = Math.Abs(value) + Math.Abs(value3);
						if (num3 + Math.Abs(value2) != num3)
						{
							if (num2 == -1)
							{
								num2 = j;
							}
							num = j;
						}
						else if (num >= 0)
						{
							break;
						}
					}
					if (num2 == -1)
					{
						this.ComputePermutation((int)eSort);
						return i;
					}
					this.DoQRImplicitShift(num, num2);
				}
				return int.MaxValue;
			}
			return 0;
		}

		public void GetEigenvalues(double[] eigenvalues)
		{
			if (eigenvalues != null && this.mSize > 0)
			{
				if (this.mPermutation[0] >= 0)
				{
					for (int i = 0; i < this.mSize; i++)
					{
						int num = this.mPermutation[i];
						eigenvalues[i] = this.mDiagonal[num];
					}
					return;
				}
				Array.Copy(this.mDiagonal, eigenvalues, this.mSize);
			}
		}

		public double[] GetEigenvalues()
		{
			double[] array = new double[this.mSize];
			this.GetEigenvalues(array);
			return array;
		}

		public double GetEigenvalue(int c)
		{
			if (this.mSize <= 0)
			{
				return double.MaxValue;
			}
			if (this.mPermutation[0] >= 0)
			{
				return this.mDiagonal[this.mPermutation[c]];
			}
			return this.mDiagonal[c];
		}

		public void GetEigenvectors(double[] eigenvectors)
		{
			if (eigenvectors != null && this.mSize > 0)
			{
				Array.Clear(eigenvectors, 0, this.mSize * this.mSize);
				for (int i = 0; i < this.mSize; i++)
				{
					eigenvectors[i + this.mSize * i] = 1.0;
				}
				int j = this.mSize - 3;
				int num = j + 1;
				while (j >= 0)
				{
					ArrayAlias<double> arrayAlias = new ArrayAlias<double>(this.mMatrix, j);
					double num2 = arrayAlias[this.mSize * (j + 1)];
					int k;
					for (k = 0; k < j + 1; k++)
					{
						this.mVVector[k] = 0.0;
					}
					this.mVVector[k] = 1.0;
					for (k++; k < this.mSize; k++)
					{
						this.mVVector[k] = arrayAlias[this.mSize * k];
					}
					for (k = 0; k < this.mSize; k++)
					{
						this.mWVector[k] = 0.0;
						for (int l = num; l < this.mSize; l++)
						{
							this.mWVector[k] += this.mVVector[l] * eigenvectors[k + this.mSize * l];
						}
						this.mWVector[k] *= num2;
					}
					for (k = num; k < this.mSize; k++)
					{
						for (int l = 0; l < this.mSize; l++)
						{
							eigenvectors[l + this.mSize * k] -= this.mVVector[k] * this.mWVector[l];
						}
					}
					j--;
					num--;
				}
				foreach (SymmetricEigenSolver.GivensRotation givensRotation in this.mGivens)
				{
					for (int k = 0; k < this.mSize; k++)
					{
						int num3 = givensRotation.index + this.mSize * k;
						double num4 = eigenvectors[num3];
						double num5 = eigenvectors[num3 + 1];
						double num6 = givensRotation.cs * num4 - givensRotation.sn * num5;
						double num7 = givensRotation.sn * num4 + givensRotation.cs * num5;
						eigenvectors[num3] = num6;
						eigenvectors[num3 + 1] = num7;
					}
				}
				this.mIsRotation = 1 - (this.mSize & 1);
				if (this.mPermutation[0] >= 0)
				{
					Array.Clear(this.mVisited, 0, this.mVisited.Length);
					for (int m = 0; m < this.mSize; m++)
					{
						if (this.mVisited[m] == 0 && this.mPermutation[m] != m)
						{
							this.mIsRotation = 1 - this.mIsRotation;
							int num8 = m;
							int num9 = m;
							for (int n = 0; n < this.mSize; n++)
							{
								this.mPVector[n] = eigenvectors[m + this.mSize * n];
							}
							int num10;
							while ((num10 = this.mPermutation[num9]) != num8)
							{
								this.mVisited[num9] = 1;
								for (int n = 0; n < this.mSize; n++)
								{
									eigenvectors[num9 + this.mSize * n] = eigenvectors[num10 + this.mSize * n];
								}
								num9 = num10;
							}
							this.mVisited[num9] = 1;
							for (int n = 0; n < this.mSize; n++)
							{
								eigenvectors[num9 + this.mSize * n] = this.mPVector[n];
							}
						}
					}
				}
			}
		}

		public double[] GetEigenvectors()
		{
			double[] array = new double[this.mSize * this.mSize];
			this.GetEigenvectors(array);
			return array;
		}

		public bool IsRotation()
		{
			if (this.mSize > 0)
			{
				if (this.mIsRotation == -1)
				{
					this.mIsRotation = 1 - (this.mSize & 1);
					if (this.mPermutation[0] >= 0)
					{
						Array.Clear(this.mVisited, 0, this.mVisited.Length);
						for (int i = 0; i < this.mSize; i++)
						{
							if (this.mVisited[i] == 0 && this.mPermutation[i] != i)
							{
								int num = i;
								int num2 = i;
								int num3;
								while ((num3 = this.mPermutation[num2]) != num)
								{
									this.mVisited[num2] = 1;
									num2 = num3;
								}
								this.mVisited[num2] = 1;
							}
						}
					}
				}
				return this.mIsRotation == 1;
			}
			return false;
		}

		public void GetEigenvector(int c, double[] eigenvector)
		{
			if (0 <= c && c < this.mSize)
			{
				double[] array = eigenvector;
				double[] array2 = this.mPVector;
				Array.Clear(array, 0, this.mSize);
				if (this.mPermutation[c] >= 0)
				{
					array[this.mPermutation[c]] = 1.0;
				}
				else
				{
					array[c] = 1.0;
				}
				for (int i = this.mGivens.Count - 1; i >= 0; i--)
				{
					SymmetricEigenSolver.GivensRotation givensRotation = this.mGivens[i];
					double num = array[givensRotation.index];
					double num2 = array[givensRotation.index + 1];
					double num3 = givensRotation.cs * num + givensRotation.sn * num2;
					double num4 = -givensRotation.sn * num + givensRotation.cs * num2;
					array[givensRotation.index] = num3;
					array[givensRotation.index + 1] = num4;
				}
				for (int j = this.mSize - 3; j >= 0; j--)
				{
					ArrayAlias<double> arrayAlias = new ArrayAlias<double>(this.mMatrix, j);
					double num5 = arrayAlias[this.mSize * (j + 1)];
					int k;
					for (k = 0; k < j + 1; k++)
					{
						array2[k] = array[k];
					}
					double num6 = array[k];
					for (int l = k + 1; l < this.mSize; l++)
					{
						num6 += array[l] * arrayAlias[this.mSize * l];
					}
					num6 *= num5;
					array2[k] = array[k] - num6;
					for (k++; k < this.mSize; k++)
					{
						array2[k] = array[k] - num6 * arrayAlias[this.mSize * k];
					}
					double[] array3 = array;
					array = array2;
					array2 = array3;
				}
				if (array != eigenvector)
				{
					Array.Copy(array, eigenvector, this.mSize);
				}
			}
		}

		public double[] GetEigenvector(int c)
		{
			double[] array = new double[this.mSize];
			this.GetEigenvector(c, array);
			return array;
		}

		private void Tridiagonalize()
		{
			int i = 0;
			int num = 1;
			while (i < this.mSize - 2)
			{
				double num2 = 0.0;
				for (int j = 0; j < num; j++)
				{
					this.mVVector[j] = 0.0;
				}
				for (int j = num; j < this.mSize; j++)
				{
					double num3 = this.mMatrix[j + this.mSize * i];
					this.mVVector[j] = num3;
					num2 += num3 * num3;
				}
				double num4 = 1.0;
				num2 = Math.Sqrt(num2);
				if (num2 > 0.0)
				{
					double num5 = this.mVVector[num];
					double num6 = (double)((num5 >= 0.0) ? 1 : -1);
					double num7 = 1.0 / (num5 + num6 * num2);
					this.mVVector[num] = 1.0;
					for (int j = num + 1; j < this.mSize; j++)
					{
						this.mVVector[j] *= num7;
						num4 += this.mVVector[j] * this.mVVector[j];
					}
				}
				double num8 = 1.0 / num4;
				double num9 = num8 * 2.0;
				double num10 = 0.0;
				for (int j = i; j < this.mSize; j++)
				{
					this.mPVector[j] = 0.0;
					int k;
					for (k = i; k < j; k++)
					{
						this.mPVector[j] += this.mMatrix[j + this.mSize * k] * this.mVVector[k];
					}
					while (k < this.mSize)
					{
						this.mPVector[j] += this.mMatrix[k + this.mSize * j] * this.mVVector[k];
						k++;
					}
					this.mPVector[j] *= num9;
					num10 += this.mPVector[j] * this.mVVector[j];
				}
				num10 *= num8;
				for (int j = i; j < this.mSize; j++)
				{
					this.mWVector[j] = this.mPVector[j] - num10 * this.mVVector[j];
				}
				for (int j = i; j < this.mSize; j++)
				{
					double num11 = this.mVVector[j];
					double num12 = this.mWVector[j];
					double num13 = num11 * num12 * 2.0;
					this.mMatrix[j + this.mSize * j] -= num13;
					for (int k = j + 1; k < this.mSize; k++)
					{
						num13 = num11 * this.mWVector[k] + num12 * this.mVVector[k];
						this.mMatrix[k + this.mSize * j] -= num13;
					}
				}
				this.mMatrix[i + this.mSize * num] = num9;
				for (int j = num + 1; j < this.mSize; j++)
				{
					this.mMatrix[i + this.mSize * j] = this.mVVector[j];
				}
				i++;
				num++;
			}
			int num14 = this.mSize - 1;
			int num15 = 0;
			int num16 = this.mSize + 1;
			int l = 0;
			while (l < num14)
			{
				this.mDiagonal[l] = this.mMatrix[num15];
				this.mSuperdiagonal[l] = this.mMatrix[num15 + 1];
				l++;
				num15 += num16;
			}
			this.mDiagonal[l] = this.mMatrix[num15];
		}

		private void GetSinCos(double x, double y, ref double cs, ref double sn)
		{
			if (y == 0.0)
			{
				cs = 1.0;
				sn = 0.0;
				return;
			}
			double num;
			if (Math.Abs(y) > Math.Abs(x))
			{
				num = -x / y;
				sn = 1.0 / Math.Sqrt(1.0 + num * num);
				cs = sn * num;
				return;
			}
			num = -y / x;
			cs = 1.0 / Math.Sqrt(1.0 + num * num);
			sn = cs * num;
		}

		private void DoQRImplicitShift(int imin, int imax)
		{
			double num = this.mDiagonal[imax];
			double num2 = this.mSuperdiagonal[imax];
			double num3 = this.mDiagonal[imax + 1];
			double num4 = (num - num3) * 0.5;
			double num5 = (double)((num4 >= 0.0) ? 1 : -1);
			double num6 = num2 * num2;
			double num7 = num3 - num6 / (num4 + num5 * Math.Sqrt(num4 * num4 + num6));
			double x = this.mDiagonal[imin] - num7;
			double y = this.mSuperdiagonal[imin];
			double num8 = 0.0;
			double num9 = 0.0;
			double num10 = 0.0;
			int num11 = imin - 1;
			int i = imin;
			int num12 = imin + 1;
			while (i <= imax)
			{
				this.GetSinCos(x, y, ref num8, ref num9);
				this.mGivens.Add(new SymmetricEigenSolver.GivensRotation(i, num8, num9));
				if (i > imin)
				{
					this.mSuperdiagonal[num11] = num8 * this.mSuperdiagonal[num11] - num9 * num10;
				}
				num3 = this.mDiagonal[i];
				double num13 = this.mSuperdiagonal[i];
				double num14 = this.mDiagonal[num12];
				double num15 = num8 * num3 - num9 * num13;
				double num16 = num8 * num13 - num9 * num14;
				double num17 = num9 * num3 + num8 * num13;
				double num18 = num9 * num13 + num8 * num14;
				this.mDiagonal[i] = num8 * num15 - num9 * num16;
				this.mSuperdiagonal[i] = num9 * num15 + num8 * num16;
				this.mDiagonal[num12] = num9 * num17 + num8 * num18;
				if (i < imax)
				{
					double num19 = this.mSuperdiagonal[num12];
					num10 = -num9 * num19;
					this.mSuperdiagonal[num12] = num8 * num19;
					x = this.mSuperdiagonal[i];
					y = num10;
				}
				num11++;
				i++;
				num12++;
			}
		}

		private void ComputePermutation(int sortType)
		{
			this.mIsRotation = -1;
			if (sortType == 0)
			{
				this.mPermutation[0] = -1;
				return;
			}
			SymmetricEigenSolver.SortItem[] array = new SymmetricEigenSolver.SortItem[this.mSize];
			for (int i = 0; i < this.mSize; i++)
			{
				array[i].eigenvalue = this.mDiagonal[i];
				array[i].index = i;
			}
			if (sortType > 0)
			{
				Array.Sort<SymmetricEigenSolver.SortItem>(array, delegate(SymmetricEigenSolver.SortItem a, SymmetricEigenSolver.SortItem b)
				{
					if (a.eigenvalue == b.eigenvalue)
					{
						return 0;
					}
					if (a.eigenvalue >= b.eigenvalue)
					{
						return 1;
					}
					return -1;
				});
			}
			else
			{
				Array.Sort<SymmetricEigenSolver.SortItem>(array, delegate(SymmetricEigenSolver.SortItem a, SymmetricEigenSolver.SortItem b)
				{
					if (a.eigenvalue == b.eigenvalue)
					{
						return 0;
					}
					if (a.eigenvalue <= b.eigenvalue)
					{
						return 1;
					}
					return -1;
				});
			}
			for (int j = 0; j < this.mSize; j++)
			{
				this.mPermutation[j] = array[j].index;
			}
		}

		public const int NO_CONVERGENCE = 2147483647;

		private int mSize;

		private int mMaxIterations;

		private double[] mMatrix;

		private double[] mDiagonal;

		private double[] mSuperdiagonal;

		private List<SymmetricEigenSolver.GivensRotation> mGivens;

		private int[] mPermutation;

		private int[] mVisited;

		private int mIsRotation;

		private double[] mPVector;

		private double[] mVVector;

		private double[] mWVector;

		public enum SortType
		{
			Decreasing = -1,
			NoSorting,
			Increasing
		}

		private struct GivensRotation
		{
			public GivensRotation(int inIndex, double inCs, double inSn)
			{
				this.index = inIndex;
				this.cs = inCs;
				this.sn = inSn;
			}

			public int index;

			public double cs;

			public double sn;
		}

		private struct SortItem
		{
			public double eigenvalue;

			public int index;
		}
	}
}
