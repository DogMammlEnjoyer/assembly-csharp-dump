using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal class TimeSeries
	{
		public int Count
		{
			get
			{
				return this._samples.Count;
			}
		}

		public int Capacity
		{
			get
			{
				return this._samples.Capacity;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._samples.IsEmpty;
			}
		}

		public bool IsFull
		{
			get
			{
				return this._samples.IsFull;
			}
		}

		public unsafe double Latest
		{
			get
			{
				return this.IsEmpty ? 0.0 : (*this._samples.Back());
			}
		}

		public double Avg
		{
			get
			{
				return this._mean;
			}
		}

		public double Var
		{
			get
			{
				bool flag = this.Count > 1;
				if (flag)
				{
					bool flag2 = this._varSum >= 0.0;
					if (flag2)
					{
						return this._varSum / (double)(this.Count - 1);
					}
				}
				return 0.0;
			}
		}

		public double Dev
		{
			get
			{
				double var = this.Var;
				return (var >= double.Epsilon) ? Math.Sqrt(var) : 0.0;
			}
		}

		public double Min
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.Count; i++)
				{
					num = Math.Min(num, this._samples[i]);
				}
				return num;
			}
		}

		public double Max
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.Count; i++)
				{
					num = Math.Max(num, this._samples[i]);
				}
				return num;
			}
		}

		public double Median
		{
			get
			{
				return TimeSeries.FindMedian(this._samples);
			}
		}

		public double MeanAbsDev
		{
			get
			{
				double mean = this._mean;
				double num = 0.0;
				for (int i = 0; i < this.Count; i++)
				{
					num += Math.Abs(this._samples[i] - mean);
				}
				return 1.2533 * (num / (double)this.Count);
			}
		}

		public double MedianAbsDev
		{
			get
			{
				double median = this.Median;
				List<double> list = new List<double>(this.Count);
				for (int i = 0; i < this.Count; i++)
				{
					list.Add(Math.Abs(this._samples[i] - median));
				}
				return 1.4826 * TimeSeries.FindMedian(list);
			}
		}

		public double Smoothed(double alpha)
		{
			Assert.Check(alpha >= 0.0 && alpha <= 1.0, "the input range of this method is [0.0, 1.0], inclusive");
			bool flag = this.Count > 0;
			double result;
			if (flag)
			{
				double num = this._samples[0];
				for (int i = 1; i < this.Count; i++)
				{
					num = (1.0 - alpha) * num + alpha * this._samples[i];
				}
				result = num;
			}
			else
			{
				result = 0.0;
			}
			return result;
		}

		public TimeSeries(int capacity)
		{
			this._mean = 0.0;
			this._varSum = 0.0;
			this._samples = new RingBuffer<double>(Math.Max(2, capacity));
		}

		public void Add(double value)
		{
			Assert.Check(!double.IsNaN(value));
			Assert.Check(!double.IsInfinity(value));
			double mean = this._mean;
			bool isFull = this.IsFull;
			if (isFull)
			{
				double num = this._samples.PopFront();
				this._samples.PushBack(value);
				double num2 = value - num;
				this._mean += num2 / (double)this.Capacity;
				this._varSum += num2 * (value - this._mean + (num - mean));
			}
			else
			{
				this._samples.PushBack(value);
				double num3 = value - mean;
				this._mean += num3 / (double)this.Count;
				this._varSum += num3 * (value - this._mean);
			}
		}

		public void Fill(double value)
		{
			this.Clear();
			for (int i = 0; i < this.Capacity; i++)
			{
				this.Add(value);
			}
		}

		public double QuantileNormal(double p)
		{
			return this.Avg + TimeSeries.InverseCdfNormal(p) * this.Dev;
		}

		public static double InverseCdfNormal(double p)
		{
			Assert.Check(p > 0.0 && p < 1.0, "the input range of this function is (0.0, 1.0), non-inclusive");
			bool flag = p < 0.5;
			double result;
			if (flag)
			{
				result = -TimeSeries.<InverseCdfNormal>g__Polynomial|34_0(p);
			}
			else
			{
				result = TimeSeries.<InverseCdfNormal>g__Polynomial|34_0(1.0 - p);
			}
			return result;
		}

		internal static double FindMedian(IEnumerable<double> values)
		{
			return TimeSeries.FindMedian(new List<double>(values));
		}

		internal static double FindMedian(List<double> list)
		{
			int count = list.Count;
			bool flag = count == 0;
			double result;
			if (flag)
			{
				result = 0.0;
			}
			else
			{
				list.Sort();
				bool flag2 = count % 2 == 1;
				if (flag2)
				{
					result = list[count / 2];
				}
				else
				{
					result = 0.5 * (list[count / 2 - 1] + list[count / 2]);
				}
			}
			return result;
		}

		public void Clear()
		{
			this._mean = 0.0;
			this._varSum = 0.0;
			this._samples.Clear();
		}

		[CompilerGenerated]
		internal static double <InverseCdfNormal>g__Polynomial|34_0(double x)
		{
			double num = Math.Sqrt(-2.0 * Math.Log(x));
			double num2 = (0.06114673576519699 * num + 1.5615337002120804) * num + 2.6539620026016846;
			double num3 = ((0.009547745327068945 * num + 0.4540555364442335) * num + 1.9048751828364987) * num + 1.0;
			return num - num2 / num3;
		}

		private double _mean;

		private double _varSum;

		private readonly RingBuffer<double> _samples;
	}
}
