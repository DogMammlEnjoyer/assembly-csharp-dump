using System;

namespace g3
{
	public class BSplineBasis
	{
		protected BSplineBasis()
		{
		}

		public BSplineBasis(int numCtrlPoints, int degree, bool open)
		{
			this.mUniform = true;
			int num = this.Initialize(numCtrlPoints, degree, open);
			double num2 = 1.0 / (double)(this.mNumCtrlPoints - this.mDegree);
			if (this.mOpen)
			{
				int i;
				for (i = 0; i <= this.mDegree; i++)
				{
					this.mKnot[i] = 0.0;
				}
				while (i < this.mNumCtrlPoints)
				{
					this.mKnot[i] = (double)(i - this.mDegree) * num2;
					i++;
				}
				while (i < num)
				{
					this.mKnot[i] = 1.0;
					i++;
				}
				return;
			}
			for (int i = 0; i < num; i++)
			{
				this.mKnot[i] = (double)(i - this.mDegree) * num2;
			}
		}

		public BSplineBasis(int numCtrlPoints, int degree, double[] knots, bool bIsInteriorKnots)
		{
			this.mUniform = false;
			int num = this.Initialize(numCtrlPoints, degree, true);
			if (bIsInteriorKnots)
			{
				if (knots.Length != this.mNumCtrlPoints - this.mDegree - 1)
				{
					throw new Exception("BSplineBasis nonuniform constructor: invalid interior knot vector");
				}
				int i;
				for (i = 0; i <= this.mDegree; i++)
				{
					this.mKnot[i] = 0.0;
				}
				int num2 = 0;
				while (i < this.mNumCtrlPoints)
				{
					this.mKnot[i] = knots[num2];
					i++;
					num2++;
				}
				while (i < num)
				{
					this.mKnot[i] = 1.0;
					i++;
				}
				return;
			}
			else
			{
				if (this.mKnot.Length != knots.Length)
				{
					throw new Exception("BSplineBasis nonuniform constructor: invalid knot vector");
				}
				Array.Copy(knots, this.mKnot, knots.Length);
				return;
			}
		}

		public BSplineBasis Clone()
		{
			return new BSplineBasis
			{
				mNumCtrlPoints = this.mNumCtrlPoints,
				mDegree = this.mDegree,
				mKnot = (double[])this.mKnot.Clone(),
				mOpen = this.mOpen,
				mUniform = this.mUniform
			};
		}

		public int GetNumCtrlPoints()
		{
			return this.mNumCtrlPoints;
		}

		public int GetDegree()
		{
			return this.mDegree;
		}

		public bool IsOpen()
		{
			return this.mOpen;
		}

		public bool IsUniform()
		{
			return this.mUniform;
		}

		public int KnotCount
		{
			get
			{
				return this.mNumCtrlPoints + this.mDegree + 1;
			}
		}

		public int InteriorKnotCount
		{
			get
			{
				return this.mNumCtrlPoints - this.mDegree - 1;
			}
		}

		public void SetInteriorKnot(int j, double value)
		{
			if (this.mUniform)
			{
				throw new Exception("BSplineBasis.SetKnot: knots cannot be set for uniform splines");
			}
			int num = j + this.mDegree + 1;
			if (this.mDegree + 1 <= num && num <= this.mNumCtrlPoints)
			{
				this.mKnot[num] = value;
				return;
			}
			throw new Exception("BSplineBasis.SetKnot: index out of range: " + j.ToString());
		}

		public double GetInteriorKnot(int j)
		{
			int num = j + this.mDegree + 1;
			if (this.mDegree + 1 <= num && num <= this.mNumCtrlPoints)
			{
				return this.mKnot[num];
			}
			throw new Exception("BSplineBasis.GetKnot: index out of range: " + j.ToString());
		}

		public void SetKnot(int j, double value)
		{
			this.mKnot[j] = value;
		}

		public double GetKnot(int j)
		{
			return this.mKnot[j];
		}

		public double GetD0(int i)
		{
			return this.mBD0[this.mDegree, i];
		}

		public double GetD1(int i)
		{
			return this.mBD1[this.mDegree, i];
		}

		public double GetD2(int i)
		{
			return this.mBD2[this.mDegree, i];
		}

		public double GetD3(int i)
		{
			return this.mBD3[this.mDegree, i];
		}

		public void Compute(double t, int order, ref int minIndex, ref int maxIndex)
		{
			if (order > 3)
			{
				throw new Exception("BSplineBasis.Compute: cannot compute order " + order.ToString());
			}
			if (order >= 1)
			{
				if (this.mBD1 == null)
				{
					this.mBD1 = this.Allocate();
				}
				if (order >= 2)
				{
					if (this.mBD2 == null)
					{
						this.mBD2 = this.Allocate();
					}
					if (order >= 3 && this.mBD3 == null)
					{
						this.mBD3 = this.Allocate();
					}
				}
			}
			int key = this.GetKey(ref t);
			this.mBD0[0, key] = 1.0;
			if (order >= 1)
			{
				this.mBD1[0, key] = 0.0;
				if (order >= 2)
				{
					this.mBD2[0, key] = 0.0;
					if (order >= 3)
					{
						this.mBD3[0, key] = 0.0;
					}
				}
			}
			double num = t - this.mKnot[key];
			double num2 = this.mKnot[key + 1] - t;
			for (int i = 1; i <= this.mDegree; i++)
			{
				double num3 = 1.0 / (this.mKnot[key + i] - this.mKnot[key]);
				double num4 = 1.0 / (this.mKnot[key + 1] - this.mKnot[key - i + 1]);
				if (this.mKnot[key + i] == this.mKnot[key])
				{
					num3 = 0.0;
				}
				if (this.mKnot[key + 1] == this.mKnot[key - i + 1])
				{
					num4 = 0.0;
				}
				this.mBD0[i, key] = num * this.mBD0[i - 1, key] * num3;
				this.mBD0[i, key - i] = num2 * this.mBD0[i - 1, key - i + 1] * num4;
				if (order >= 1)
				{
					this.mBD1[i, key] = (num * this.mBD1[i - 1, key] + this.mBD0[i - 1, key]) * num3;
					this.mBD1[i, key - i] = (num2 * this.mBD1[i - 1, key - i + 1] - this.mBD0[i - 1, key - i + 1]) * num4;
					if (order >= 2)
					{
						this.mBD2[i, key] = (num * this.mBD2[i - 1, key] + 2.0 * this.mBD1[i - 1, key]) * num3;
						this.mBD2[i, key - i] = (num2 * this.mBD2[i - 1, key - i + 1] - 2.0 * this.mBD1[i - 1, key - i + 1]) * num4;
						if (order >= 3)
						{
							this.mBD3[i, key] = (num * this.mBD3[i - 1, key] + 3.0 * this.mBD2[i - 1, key]) * num3;
							this.mBD3[i, key - i] = (num2 * this.mBD3[i - 1, key - i + 1] - 3.0 * this.mBD2[i - 1, key - i + 1]) * num4;
						}
					}
				}
			}
			for (int i = 2; i <= this.mDegree; i++)
			{
				for (int j = key - i + 1; j < key; j++)
				{
					num = t - this.mKnot[j];
					num2 = this.mKnot[j + i + 1] - t;
					double num3 = 1.0 / (this.mKnot[j + i] - this.mKnot[j]);
					double num4 = 1.0 / (this.mKnot[j + i + 1] - this.mKnot[j + 1]);
					if (this.mKnot[j + i] == this.mKnot[j])
					{
						num3 = 0.0;
					}
					if (this.mKnot[j + i + 1] == this.mKnot[j + 1])
					{
						num4 = 0.0;
					}
					this.mBD0[i, j] = num * this.mBD0[i - 1, j] * num3 + num2 * this.mBD0[i - 1, j + 1] * num4;
					if (order >= 1)
					{
						this.mBD1[i, j] = (num * this.mBD1[i - 1, j] + this.mBD0[i - 1, j]) * num3 + (num2 * this.mBD1[i - 1, j + 1] - this.mBD0[i - 1, j + 1]) * num4;
						if (order >= 2)
						{
							this.mBD2[i, j] = (num * this.mBD2[i - 1, j] + 2.0 * this.mBD1[i - 1, j]) * num3 + (num2 * this.mBD2[i - 1, j + 1] - 2.0 * this.mBD1[i - 1, j + 1]) * num4;
							if (order >= 3)
							{
								this.mBD3[i, j] = (num * this.mBD3[i - 1, j] + 3.0 * this.mBD2[i - 1, j]) * num3 + (num2 * this.mBD3[i - 1, j + 1] - 3.0 * this.mBD2[i - 1, j + 1]) * num4;
							}
						}
					}
				}
			}
			minIndex = key - this.mDegree;
			maxIndex = key;
		}

		protected int Initialize(int numCtrlPoints, int degree, bool open)
		{
			if (numCtrlPoints < 2)
			{
				throw new Exception("BSplineBasis.Initialize: only received " + numCtrlPoints.ToString() + " control points!");
			}
			if (degree < 1 || degree > numCtrlPoints - 1)
			{
				throw new Exception("BSplineBasis.Initialize: invalid degree " + degree.ToString());
			}
			this.mNumCtrlPoints = numCtrlPoints;
			this.mDegree = degree;
			this.mOpen = open;
			int num = this.mNumCtrlPoints + this.mDegree + 1;
			this.mKnot = new double[num];
			this.mBD0 = this.Allocate();
			this.mBD1 = null;
			this.mBD2 = null;
			this.mBD3 = null;
			return num;
		}

		protected double[,] Allocate()
		{
			int num = this.mDegree + 1;
			int num2 = this.mNumCtrlPoints + this.mDegree;
			double[,] array = new double[num, num2];
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					array[i, j] = 0.0;
				}
			}
			return array;
		}

		protected int GetKey(ref double t)
		{
			if (this.mOpen)
			{
				if (t <= 0.0)
				{
					t = 0.0;
					return this.mDegree;
				}
				if (t >= 1.0)
				{
					t = 1.0;
					return this.mNumCtrlPoints - 1;
				}
			}
			else if (t < 0.0 || t >= 1.0)
			{
				t -= Math.Floor(t);
			}
			int num;
			if (this.mUniform)
			{
				num = this.mDegree + (int)((double)(this.mNumCtrlPoints - this.mDegree) * t);
			}
			else
			{
				num = this.mDegree + 1;
				while (num <= this.mNumCtrlPoints && t >= this.mKnot[num])
				{
					num++;
				}
				num--;
			}
			return num;
		}

		protected int mNumCtrlPoints;

		protected int mDegree;

		protected double[] mKnot;

		protected bool mOpen;

		protected bool mUniform;

		protected double[,] mBD0;

		protected double[,] mBD1;

		protected double[,] mBD2;

		protected double[,] mBD3;
	}
}
