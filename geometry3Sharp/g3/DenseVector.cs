using System;

namespace g3
{
	public class DenseVector
	{
		public DenseVector(int N)
		{
			this.d = new double[N];
			Array.Clear(this.d, 0, this.d.Length);
			this.N = N;
		}

		public void Set(int i, double value)
		{
			this.d[i] = value;
		}

		public int Size
		{
			get
			{
				return this.N;
			}
		}

		public int Length
		{
			get
			{
				return this.N;
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

		public double[] Buffer
		{
			get
			{
				return this.d;
			}
		}

		public double Dot(DenseVector v2)
		{
			return this.Dot(v2.d);
		}

		public double Dot(double[] v2)
		{
			if (v2.Length != this.N)
			{
				throw new Exception("DenseVector.Dot: incompatible lengths");
			}
			double num = 0.0;
			for (int i = 0; i < v2.Length; i++)
			{
				num += this.d[i] * v2[i];
			}
			return num;
		}

		private double[] d;

		private int N;
	}
}
