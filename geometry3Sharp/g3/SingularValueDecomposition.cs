using System;
using System.Collections.Generic;

namespace g3
{
	public class SingularValueDecomposition
	{
		public SingularValueDecomposition(int numRows, int numCols, int maxIterations)
		{
			this.mNumRows = (this.mNumCols = (this.mMaxIterations = 0));
			if (numCols > 1 && numRows >= numCols && maxIterations > 0)
			{
				this.mNumRows = numRows;
				this.mNumCols = numCols;
				this.mMaxIterations = maxIterations;
				this.mMatrix = new double[numRows * numCols];
				this.mDiagonal = new double[numCols];
				this.mSuperdiagonal = new double[numCols - 1];
				this.mRGivens = new List<SingularValueDecomposition.GivensRotation>(maxIterations * (numCols - 1));
				this.mLGivens = new List<SingularValueDecomposition.GivensRotation>(maxIterations * (numCols - 1));
				this.mFixupDiagonal = new double[numCols];
				this.mPermutation = new int[numCols];
				this.mVisited = new int[numCols];
				this.mTwoInvUTU = new double[numCols];
				this.mTwoInvVTV = new double[numCols - 2];
				this.mUVector = new double[numRows];
				this.mVVector = new double[numCols];
				this.mWVector = new double[numRows];
			}
		}

		public uint Solve(double[] input, int sortType = -1)
		{
			if (this.mNumRows > 0)
			{
				int num = this.mNumRows * this.mNumCols;
				Array.Copy(input, this.mMatrix, num);
				this.Bidiagonalize();
				double num2 = Math.Abs(input[0]);
				for (int i = 1; i < num; i++)
				{
					double num3 = Math.Abs(input[i]);
					if (num3 > num2)
					{
						num2 = num3;
					}
				}
				double num4 = 0.0;
				if (num2 > 0.0)
				{
					double num5 = 1.0 / num2;
					for (int j = 0; j < num; j++)
					{
						double num6 = input[j] * num5;
						num4 += num6 * num6;
					}
					num4 = num2 * Math.Sqrt(num4);
				}
				double num7 = 8.0;
				double epsilon = double.Epsilon;
				double threshold = num7 * epsilon * num4;
				this.mRGivens.Clear();
				this.mLGivens.Clear();
				uint num8 = 0U;
				while ((ulong)num8 < (ulong)((long)this.mMaxIterations))
				{
					int num9 = -1;
					int num10 = -1;
					for (int k = this.mNumCols - 2; k >= 0; k--)
					{
						double value = this.mDiagonal[k];
						double value2 = this.mSuperdiagonal[k];
						double value3 = this.mDiagonal[k + 1];
						double num11 = Math.Abs(value) + Math.Abs(value3);
						if (num11 + Math.Abs(value2) != num11)
						{
							if (num10 == -1)
							{
								num10 = k;
							}
							num9 = k;
						}
						else if (num9 >= 0)
						{
							break;
						}
					}
					if (num10 == -1)
					{
						this.EnsureNonnegativeDiagonal();
						this.ComputePermutation(sortType);
						return num8;
					}
					if (this.DiagonalEntriesNonzero(num9, num10, threshold))
					{
						this.DoGolubKahanStep(num9, num10);
					}
					num8 += 1U;
				}
				return uint.MaxValue;
			}
			return 0U;
		}

		public void GetSingularValues(double[] singularValues)
		{
			if (singularValues != null && this.mNumCols > 0)
			{
				if (this.mPermutation[0] >= 0)
				{
					for (int i = 0; i < this.mNumCols; i++)
					{
						int num = this.mPermutation[i];
						singularValues[i] = this.mDiagonal[num];
					}
					return;
				}
				for (int j = 0; j < this.mNumCols; j++)
				{
					singularValues[j] = this.mDiagonal[j];
				}
			}
		}

		public void GetU(double[] uMatrix)
		{
			if (uMatrix == null || this.mNumCols == 0)
			{
				return;
			}
			Array.Clear(uMatrix, 0, uMatrix.Length);
			for (int i = 0; i < this.mNumRows; i++)
			{
				uMatrix[i + this.mNumRows * i] = 1.0;
			}
			int j = this.mNumCols - 1;
			int num = j + 1;
			while (j >= 0)
			{
				double num2 = this.mTwoInvUTU[j];
				this.mUVector[j] = 1.0;
				for (int k = num; k < this.mNumRows; k++)
				{
					this.mUVector[k] = this.mMatrix[j + this.mNumCols * k];
				}
				this.mWVector[j] = num2;
				for (int k = num; k < this.mNumRows; k++)
				{
					this.mWVector[k] = 0.0;
					for (int l = num; l < this.mNumRows; l++)
					{
						this.mWVector[k] += this.mUVector[l] * uMatrix[k + this.mNumRows * l];
					}
					this.mWVector[k] *= num2;
				}
				for (int k = j; k < this.mNumRows; k++)
				{
					for (int l = j; l < this.mNumRows; l++)
					{
						uMatrix[l + this.mNumRows * k] -= this.mUVector[k] * this.mWVector[l];
					}
				}
				j--;
				num--;
			}
			foreach (SingularValueDecomposition.GivensRotation givensRotation in this.mLGivens)
			{
				int num3 = givensRotation.index0;
				int num4 = givensRotation.index1;
				int k = 0;
				while (k < this.mNumRows)
				{
					double num5 = uMatrix[num3];
					double num6 = uMatrix[num4];
					double num7 = givensRotation.cs * num5 - givensRotation.sn * num6;
					double num8 = givensRotation.sn * num5 + givensRotation.cs * num6;
					uMatrix[num3] = num7;
					uMatrix[num4] = num8;
					k++;
					num3 += this.mNumRows;
					num4 += this.mNumRows;
				}
			}
			if (this.mPermutation[0] >= 0)
			{
				Array.Clear(this.mVisited, 0, this.mVisited.Length);
				for (int l = 0; l < this.mNumCols; l++)
				{
					if (this.mVisited[l] == 0 && this.mPermutation[l] != l)
					{
						int num9 = l;
						int num10 = l;
						for (int k = 0; k < this.mNumRows; k++)
						{
							this.mWVector[k] = uMatrix[l + this.mNumRows * k];
						}
						int num11;
						while ((num11 = this.mPermutation[num10]) != num9)
						{
							this.mVisited[num10] = 1;
							for (int k = 0; k < this.mNumRows; k++)
							{
								uMatrix[num10 + this.mNumRows * k] = uMatrix[num11 + this.mNumRows * k];
							}
							num10 = num11;
						}
						this.mVisited[num10] = 1;
						for (int k = 0; k < this.mNumRows; k++)
						{
							uMatrix[num10 + this.mNumRows * k] = this.mWVector[k];
						}
					}
				}
			}
		}

		public void GetV(double[] vMatrix)
		{
			if (vMatrix == null || this.mNumCols == 0)
			{
				return;
			}
			Array.Clear(vMatrix, 0, vMatrix.Length);
			for (int i = 0; i < this.mNumCols; i++)
			{
				vMatrix[i + this.mNumCols * i] = 1.0;
			}
			int j = this.mNumCols - 3;
			int num = j + 1;
			int num2 = j + 2;
			while (j >= 0)
			{
				double num3 = this.mTwoInvVTV[j];
				this.mVVector[num] = 1.0;
				for (int k = num2; k < this.mNumCols; k++)
				{
					this.mVVector[k] = this.mMatrix[this.mNumCols * j + k];
				}
				this.mWVector[num] = num3;
				for (int k = num2; k < this.mNumCols; k++)
				{
					this.mWVector[k] = 0.0;
					for (int l = num2; l < this.mNumCols; l++)
					{
						this.mWVector[k] += this.mVVector[l] * vMatrix[k + this.mNumCols * l];
					}
					this.mWVector[k] *= num3;
				}
				for (int k = num; k < this.mNumCols; k++)
				{
					for (int l = num; l < this.mNumCols; l++)
					{
						vMatrix[l + this.mNumCols * k] -= this.mVVector[k] * this.mWVector[l];
					}
				}
				j--;
				num--;
				num2--;
			}
			foreach (SingularValueDecomposition.GivensRotation givensRotation in this.mRGivens)
			{
				int num4 = givensRotation.index0;
				int num5 = givensRotation.index1;
				int l = 0;
				while (l < this.mNumCols)
				{
					double num6 = vMatrix[num4];
					double num7 = vMatrix[num5];
					double num8 = givensRotation.cs * num6 - givensRotation.sn * num7;
					double num9 = givensRotation.sn * num6 + givensRotation.cs * num7;
					vMatrix[num4] = num8;
					vMatrix[num5] = num9;
					l++;
					num4 += this.mNumCols;
					num5 += this.mNumCols;
				}
			}
			for (int k = 0; k < this.mNumCols; k++)
			{
				for (int l = 0; l < this.mNumCols; l++)
				{
					vMatrix[l + this.mNumCols * k] *= this.mFixupDiagonal[l];
				}
			}
			if (this.mPermutation[0] >= 0)
			{
				Array.Clear(this.mVisited, 0, this.mVisited.Length);
				for (int l = 0; l < this.mNumCols; l++)
				{
					if (this.mVisited[l] == 0 && this.mPermutation[l] != l)
					{
						int num10 = l;
						int num11 = l;
						for (int k = 0; k < this.mNumCols; k++)
						{
							this.mWVector[k] = vMatrix[l + this.mNumCols * k];
						}
						int num12;
						while ((num12 = this.mPermutation[num11]) != num10)
						{
							this.mVisited[num11] = 1;
							for (int k = 0; k < this.mNumCols; k++)
							{
								vMatrix[num11 + this.mNumCols * k] = vMatrix[num12 + this.mNumCols * k];
							}
							num11 = num12;
						}
						this.mVisited[num11] = 1;
						for (int k = 0; k < this.mNumCols; k++)
						{
							vMatrix[num11 + this.mNumCols * k] = this.mWVector[k];
						}
					}
				}
			}
		}

		private void Bidiagonalize()
		{
			int i = 0;
			int num = 1;
			while (i < this.mNumCols)
			{
				double num2 = 0.0;
				for (int j = i; j < this.mNumRows; j++)
				{
					double num3 = this.mMatrix[i + this.mNumCols * j];
					this.mUVector[j] = num3;
					num2 += num3 * num3;
				}
				double num4 = 1.0;
				num2 = Math.Sqrt(num2);
				if (num2 > 0.0)
				{
					double num5 = this.mUVector[i];
					double num6 = (num5 >= 0.0) ? 1.0 : -1.0;
					double num7 = 1.0 / (num5 + num6 * num2);
					this.mUVector[i] = 1.0;
					for (int j = num; j < this.mNumRows; j++)
					{
						this.mUVector[j] *= num7;
						num4 += this.mUVector[j] * this.mUVector[j];
					}
				}
				double num8 = 1.0 / num4 * 2.0;
				for (int k = i; k < this.mNumCols; k++)
				{
					this.mWVector[k] = 0.0;
					for (int j = i; j < this.mNumRows; j++)
					{
						this.mWVector[k] += this.mMatrix[k + this.mNumCols * j] * this.mUVector[j];
					}
					this.mWVector[k] *= num8;
				}
				for (int j = i; j < this.mNumRows; j++)
				{
					for (int k = i; k < this.mNumCols; k++)
					{
						this.mMatrix[k + this.mNumCols * j] -= this.mUVector[j] * this.mWVector[k];
					}
				}
				if (i < this.mNumCols - 2)
				{
					num2 = 0.0;
					for (int k = num; k < this.mNumCols; k++)
					{
						double num9 = this.mMatrix[k + this.mNumCols * i];
						this.mVVector[k] = num9;
						num2 += num9 * num9;
					}
					double num10 = 1.0;
					num2 = Math.Sqrt(num2);
					if (num2 > 0.0)
					{
						double num11 = this.mVVector[num];
						double num12 = (num11 >= 0.0) ? 1.0 : -1.0;
						double num13 = 1.0 / (num11 + num12 * num2);
						this.mVVector[num] = 1.0;
						for (int k = num + 1; k < this.mNumCols; k++)
						{
							this.mVVector[k] *= num13;
							num10 += this.mVVector[k] * this.mVVector[k];
						}
					}
					double num14 = 1.0 / num10 * 2.0;
					for (int j = i; j < this.mNumRows; j++)
					{
						this.mWVector[j] = 0.0;
						for (int k = num; k < this.mNumCols; k++)
						{
							this.mWVector[j] += this.mMatrix[k + this.mNumCols * j] * this.mVVector[k];
						}
						this.mWVector[j] *= num14;
					}
					for (int j = i; j < this.mNumRows; j++)
					{
						for (int k = num; k < this.mNumCols; k++)
						{
							this.mMatrix[k + this.mNumCols * j] -= this.mWVector[j] * this.mVVector[k];
						}
					}
					this.mTwoInvVTV[i] = num14;
					for (int k = i + 2; k < this.mNumCols; k++)
					{
						this.mMatrix[k + this.mNumCols * i] = this.mVVector[k];
					}
				}
				this.mTwoInvUTU[i] = num8;
				for (int j = num; j < this.mNumRows; j++)
				{
					this.mMatrix[i + this.mNumCols * j] = this.mUVector[j];
				}
				i++;
				num++;
			}
			int num15 = this.mNumCols - 1;
			int num16 = 0;
			int num17 = this.mNumCols + 1;
			int l = 0;
			while (l < num15)
			{
				this.mDiagonal[l] = this.mMatrix[num16];
				this.mSuperdiagonal[l] = this.mMatrix[num16 + 1];
				l++;
				num16 += num17;
			}
			this.mDiagonal[l] = this.mMatrix[num16];
		}

		private void GetSinCos(double x, double y, out double cs, out double sn)
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

		private bool DiagonalEntriesNonzero(int imin, int imax, double threshold)
		{
			for (int i = imin; i <= imax; i++)
			{
				if (Math.Abs(this.mDiagonal[i]) <= threshold)
				{
					double num = this.mSuperdiagonal[i];
					this.mSuperdiagonal[i] = 0.0;
					for (int j = i + 1; j <= imax + 1; j++)
					{
						double num2 = this.mDiagonal[j];
						double num3;
						double num4;
						this.GetSinCos(num2, num, out num3, out num4);
						this.mLGivens.Add(new SingularValueDecomposition.GivensRotation(i, j, num3, num4));
						this.mDiagonal[j] = num3 * num2 - num4 * num;
						if (j <= imax)
						{
							double num5 = this.mSuperdiagonal[j];
							this.mSuperdiagonal[j] = num3 * num5;
							num = num4 * num5;
						}
					}
					return false;
				}
			}
			return true;
		}

		private void DoGolubKahanStep(int imin, int imax)
		{
			double num = ((double)imax >= 1.0) ? this.mSuperdiagonal[imax - 1] : 0.0;
			double num2 = this.mDiagonal[imax];
			double num3 = this.mSuperdiagonal[imax];
			double num4 = this.mDiagonal[imax + 1];
			double num5 = num2 * num2 + num * num;
			double num6 = num2 * num3;
			double num7 = num4 * num4 + num3 * num3;
			double num8 = (num5 - num7) * 0.5;
			double num9 = (num8 >= 0.0) ? 1.0 : -1.0;
			double num10 = num6 * num6;
			double num11 = num7 - num10 / (num8 + num9 * Math.Sqrt(num8 * num8 + num10));
			double x = this.mDiagonal[imin] * this.mDiagonal[imin] - num11;
			double y = this.mDiagonal[imin] * this.mSuperdiagonal[imin];
			double num12 = 0.0;
			int num13 = imin - 1;
			int i = imin;
			int num14 = imin + 1;
			while (i <= imax)
			{
				double num15;
				double num16;
				this.GetSinCos(x, y, out num15, out num16);
				this.mRGivens.Add(new SingularValueDecomposition.GivensRotation(i, num14, num15, num16));
				if (i > imin)
				{
					this.mSuperdiagonal[num13] = num15 * this.mSuperdiagonal[num13] - num16 * num12;
				}
				num7 = this.mDiagonal[i];
				double num17 = this.mSuperdiagonal[i];
				double num18 = this.mDiagonal[num14];
				this.mDiagonal[i] = num15 * num7 - num16 * num17;
				this.mSuperdiagonal[i] = num16 * num7 + num15 * num17;
				this.mDiagonal[num14] = num15 * num18;
				double num19 = -num16 * num18;
				x = this.mDiagonal[i];
				y = num19;
				this.GetSinCos(x, y, out num15, out num16);
				this.mLGivens.Add(new SingularValueDecomposition.GivensRotation(i, num14, num15, num16));
				num7 = this.mDiagonal[i];
				num17 = this.mSuperdiagonal[i];
				num18 = this.mDiagonal[num14];
				this.mDiagonal[i] = num15 * num7 - num16 * num19;
				this.mSuperdiagonal[i] = num15 * num17 - num16 * num18;
				this.mDiagonal[num14] = num16 * num17 + num15 * num18;
				if (i < imax)
				{
					double num20 = this.mSuperdiagonal[num14];
					num12 = -num16 * num20;
					this.mSuperdiagonal[num14] = num15 * num20;
					x = this.mSuperdiagonal[i];
					y = num12;
				}
				num13++;
				i++;
				num14++;
			}
		}

		private void EnsureNonnegativeDiagonal()
		{
			for (int i = 0; i < this.mNumCols; i++)
			{
				if (this.mDiagonal[i] >= 0.0)
				{
					this.mFixupDiagonal[i] = 1.0;
				}
				else
				{
					this.mDiagonal[i] = -this.mDiagonal[i];
					this.mFixupDiagonal[i] = -1.0;
				}
			}
		}

		private void ComputePermutation(int sortType)
		{
			if (sortType == 0)
			{
				this.mPermutation[0] = -1;
				return;
			}
			double[] array = new double[this.mNumCols];
			int[] array2 = new int[this.mNumCols];
			for (int i = 0; i < this.mNumCols; i++)
			{
				array[i] = this.mDiagonal[i];
				array2[i] = i;
			}
			Array.Sort<double, int>(array, array2);
			if (sortType < 0)
			{
				Array.Reverse<int>(array2);
			}
			this.mPermutation = array2;
		}

		private int mNumRows;

		private int mNumCols;

		private int mMaxIterations;

		private double[] mMatrix;

		private double[] mDiagonal;

		private double[] mSuperdiagonal;

		private List<SingularValueDecomposition.GivensRotation> mRGivens;

		private List<SingularValueDecomposition.GivensRotation> mLGivens;

		private double[] mFixupDiagonal;

		private int[] mPermutation;

		private int[] mVisited;

		private double[] mTwoInvUTU;

		private double[] mTwoInvVTV;

		private double[] mUVector;

		private double[] mVVector;

		private double[] mWVector;

		private struct GivensRotation
		{
			public GivensRotation(int inIndex0, int inIndex1, double inCs, double inSn)
			{
				this.index0 = inIndex0;
				this.index1 = inIndex1;
				this.cs = inCs;
				this.sn = inSn;
			}

			public int index0;

			public int index1;

			public double cs;

			public double sn;
		}
	}
}
