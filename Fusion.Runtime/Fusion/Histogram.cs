using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal class Histogram
	{
		public double Count
		{
			get
			{
				return this.count;
			}
		}

		public Histogram() : this(256)
		{
		}

		public Histogram(double min, double max, double error) : this(Histogram.Contrast(min, max), Histogram.Resolution(error))
		{
			Assert.Check(error > 0.0);
		}

		public Histogram(double contrast, int resolution) : this(Histogram.Capacity(contrast, resolution))
		{
			Assert.Check(resolution <= 20);
		}

		public Histogram(int capacity)
		{
			bool flag = capacity > 2048;
			if (flag)
			{
				LogStream logWarn = InternalLogStreams.LogWarn;
				if (logWarn != null)
				{
					logWarn.Log(string.Format("The requested histogram capacity was more than is allowed: {0} > {1}. Limited capacity to the maximum.", capacity, 2048));
				}
				capacity = 2048;
			}
			this.resolution = 20;
			this.minExp = 0;
			this.maxExp = 0;
			this.count = 0.0;
			this.zeroCount = 0.0;
			this.bins = new RingBuffer<double>(capacity);
		}

		public void Print()
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(string.Format("resolution: {0} => base: {1}", this.resolution, this.Base()));
			}
			DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
			if (logDebug2 != null)
			{
				logDebug2.Log(string.Format("worst-case estimation error: {0}%", this.MaxRelativeSampleError() * 100.0));
			}
			DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
			if (logDebug3 != null)
			{
				logDebug3.Log(string.Format("worst-case quantile estimation error: {0}%", this.MaxRelativeQuantileError() * 100.0));
			}
			for (int i = 0; i < this.bins.Count; i++)
			{
				DebugLogStream logDebug4 = InternalLogStreams.LogDebug;
				if (logDebug4 != null)
				{
					logDebug4.Log(string.Format("bin[{0}] = [{1}, {2}) = {3}", new object[]
					{
						this.BinExponent(i),
						this.BinLowerBound(i),
						this.BinLowerBound(i + 1),
						this.bins[i]
					}));
				}
			}
		}

		public void Clear()
		{
			this.resolution = 20;
			this.minExp = 0;
			this.maxExp = 0;
			this.count = 0.0;
			this.zeroCount = 0.0;
			this.bins.Clear();
		}

		public void Record(double value)
		{
			this.Record(value, 1.0);
		}

		public void Record(double value, double count)
		{
			bool flag = double.IsNaN(value) || double.IsInfinity(value);
			if (!flag)
			{
				bool flag2 = value == 0.0;
				if (flag2)
				{
					this.zeroCount += count;
					this.count += count;
				}
				else
				{
					int num = this.Exponent(Math.Abs(value));
					bool isEmpty = this.bins.IsEmpty;
					if (isEmpty)
					{
						this.minExp = num;
						this.maxExp = num;
						this.bins.PushBack(0.0);
					}
					bool flag3 = num < this.minExp;
					if (flag3)
					{
						int num2 = this.bins.Capacity - this.bins.Count;
						bool flag4 = num < this.minExp - num2;
						if (flag4)
						{
							num >>= this.Downsample(num, this.maxExp);
						}
						this.minExp = num;
						int num3 = this.maxExp - this.minExp + 1;
						for (int i = this.bins.Count; i < num3; i++)
						{
							this.bins.PushFront(0.0);
						}
					}
					bool flag5 = num > this.maxExp;
					if (flag5)
					{
						int num4 = this.bins.Capacity - this.bins.Count;
						bool flag6 = num > this.maxExp + num4;
						if (flag6)
						{
							num >>= this.Downsample(this.minExp, num);
						}
						this.maxExp = num;
						int num5 = this.maxExp - this.minExp + 1;
						for (int j = this.bins.Count; j < num5; j++)
						{
							this.bins.PushBack(0.0);
						}
					}
					int num6 = this.BinIndex(num);
					RingBuffer<double> ringBuffer = this.bins;
					int index = num6;
					ringBuffer[index] += count;
					this.count += count;
				}
			}
		}

		public void Normalize()
		{
			this.Rescale(1.0 / this.count);
		}

		public void Rescale(double scaleFactor)
		{
			bool flag = double.IsNaN(scaleFactor) || double.IsInfinity(scaleFactor);
			if (!flag)
			{
				this.count *= scaleFactor;
				this.zeroCount *= scaleFactor;
				for (int i = 0; i < this.bins.Count; i++)
				{
					RingBuffer<double> ringBuffer = this.bins;
					int index = i;
					ringBuffer[index] *= scaleFactor;
				}
			}
		}

		public double Mean()
		{
			double result;
			try
			{
				double num = 0.0;
				for (int i = 0; i < this.bins.Count; i++)
				{
					num += this.bins[i] * this.BinMidpoint(i);
				}
				result = num / this.count;
			}
			catch
			{
				result = 0.0;
			}
			return result;
		}

		public double MeanGeometric()
		{
			double result;
			try
			{
				double num = 0.0;
				for (int i = 0; i < this.bins.Count; i++)
				{
					num += this.bins[i] * Math.Log(this.BinMidpoint(i));
				}
				result = Math.Exp(num / this.count);
			}
			catch
			{
				result = 0.0;
			}
			return result;
		}

		public double MeanHarmonic()
		{
			double result;
			try
			{
				double num = 0.0;
				for (int i = 0; i < this.bins.Count; i++)
				{
					num += this.bins[i] * (1.0 / this.BinMidpoint(i));
				}
				result = this.count / num;
			}
			catch
			{
				result = 0.0;
			}
			return result;
		}

		public double Variance()
		{
			double result;
			try
			{
				double num = this.Mean();
				double num2 = 0.0;
				for (int i = 0; i < this.bins.Count; i++)
				{
					double num3 = this.BinMidpoint(i) - num;
					num2 += this.bins[i] * (num3 * num3);
				}
				result = num2 / (this.count - 1.0);
			}
			catch
			{
				result = 0.0;
			}
			return result;
		}

		public double Quantile(double fraction)
		{
			return this.QuantileWithEstimator(fraction, Histogram.QuantileEstimator.HyndmanFanType7);
		}

		public double QuantileWithEstimator(double fraction, Histogram.QuantileEstimator estimator)
		{
			Assert.Check(fraction >= 0.0 && fraction <= 1.0);
			double result;
			try
			{
				double num;
				switch (estimator)
				{
				default:
					num = fraction * this.count;
					break;
				case Histogram.QuantileEstimator.HyndmanFanType2:
				case Histogram.QuantileEstimator.HyndmanFanType5:
					num = fraction * this.count + 0.5;
					break;
				case Histogram.QuantileEstimator.HyndmanFanType3:
					num = fraction * this.count - 0.5;
					break;
				case Histogram.QuantileEstimator.HyndmanFanType6:
					num = fraction * (this.count + 1.0);
					break;
				case Histogram.QuantileEstimator.HyndmanFanType7:
					num = fraction * (this.count - 1.0) + 1.0;
					break;
				case Histogram.QuantileEstimator.HyndmanFanType8:
					num = fraction * (this.count + 0.3333333333333333) + 0.3333333333333333;
					break;
				case Histogram.QuantileEstimator.HyndmanFanType9:
					num = fraction * (this.count + 0.25) + 0.375;
					break;
				}
				Histogram.<>c__DisplayClass26_0 CS$<>8__locals1;
				CS$<>8__locals1.priorCount = this.zeroCount;
				bool flag = num <= CS$<>8__locals1.priorCount;
				if (flag)
				{
					result = 0.0;
				}
				else
				{
					int i;
					for (i = 0; i < this.bins.Count; i++)
					{
						bool flag2 = num <= CS$<>8__locals1.priorCount + this.bins[i];
						if (flag2)
						{
							break;
						}
						CS$<>8__locals1.priorCount += this.bins[i];
					}
					i = Math.Min(i, this.bins.Count - 1);
					CS$<>8__locals1.binCount = this.bins[i];
					CS$<>8__locals1.binLowerBound = this.BinLowerBound(i);
					CS$<>8__locals1.binUpperBound = this.BinLowerBound(i + 1);
					double num2;
					switch (estimator)
					{
					default:
						num2 = Histogram.<QuantileWithEstimator>g__Upsample|26_0(Math.Ceiling(num), ref CS$<>8__locals1);
						break;
					case Histogram.QuantileEstimator.HyndmanFanType2:
						num2 = 0.5 * (Histogram.<QuantileWithEstimator>g__Upsample|26_0(Math.Ceiling(num - 0.5), ref CS$<>8__locals1) + Histogram.<QuantileWithEstimator>g__Upsample|26_0(Math.Floor(num + 0.5), ref CS$<>8__locals1));
						break;
					case Histogram.QuantileEstimator.HyndmanFanType3:
						num2 = Histogram.<QuantileWithEstimator>g__Upsample|26_0(Math.Round(num, MidpointRounding.ToEven), ref CS$<>8__locals1);
						break;
					case Histogram.QuantileEstimator.HyndmanFanType4:
					case Histogram.QuantileEstimator.HyndmanFanType5:
					case Histogram.QuantileEstimator.HyndmanFanType6:
					case Histogram.QuantileEstimator.HyndmanFanType7:
					case Histogram.QuantileEstimator.HyndmanFanType8:
					case Histogram.QuantileEstimator.HyndmanFanType9:
					{
						double num3 = num - Math.Floor(num);
						num2 = Histogram.<QuantileWithEstimator>g__Upsample|26_0(Math.Floor(num), ref CS$<>8__locals1) + num3 * (Histogram.<QuantileWithEstimator>g__Upsample|26_0(Math.Ceiling(num), ref CS$<>8__locals1) - Histogram.<QuantileWithEstimator>g__Upsample|26_0(Math.Floor(num), ref CS$<>8__locals1));
						break;
					}
					}
					result = num2;
				}
			}
			catch
			{
				result = 0.0;
			}
			return result;
		}

		public double MaxRelativeSampleError()
		{
			return Histogram.MaxRelativeSampleError(this.resolution);
		}

		public double MaxRelativeQuantileError()
		{
			return Histogram.MaxRelativeQuantileError(this.resolution);
		}

		private int Downsample(int desiredMinExp, int desiredMaxExp)
		{
			Assert.Check(desiredMaxExp >= desiredMinExp);
			int num = 0;
			while (desiredMinExp + this.bins.Capacity <= desiredMaxExp)
			{
				desiredMinExp >>= 1;
				desiredMaxExp >>= 1;
				num++;
			}
			Assert.Check(num >= 0);
			bool flag = num > 0;
			if (flag)
			{
				int val = this.minExp;
				int val2 = this.maxExp;
				int val3 = this.minExp >> num;
				int val4 = this.maxExp >> num;
				int num2 = Math.Min(val, -1);
				int num3 = Math.Min(val2, -1);
				int num4 = Math.Max(val, 1);
				int num5 = Math.Max(val2, 1);
				int num6 = Math.Min(val3, -1);
				int num7 = Math.Min(val4, -1);
				int num8 = Math.Max(val3, 1);
				int num9 = Math.Max(val4, 1);
				int num10 = num3 - num2;
				int num11 = num7 - num6;
				int num12 = num5 - num4;
				int num13 = num9 - num8;
				bool flag2 = num10 > 0;
				if (flag2)
				{
					for (int i = num3; i >= num2; i--)
					{
						int num14 = i >> num;
						int num15 = this.BinIndex(num3);
						int num16 = num3 - i;
						int num17 = num7 - num14;
						int index = num15 - num16;
						int num18 = num15 - num17;
						double num19 = this.bins[index];
						this.bins[index] = 0.0;
						RingBuffer<double> ringBuffer = this.bins;
						int index2 = num18;
						ringBuffer[index2] += num19;
					}
				}
				bool flag3 = num12 > 0;
				if (flag3)
				{
					for (int j = num4; j <= num5; j++)
					{
						int num20 = j >> num;
						int num21 = this.BinIndex(num4);
						int num22 = j - num4;
						int num23 = num20 - num8;
						int index3 = num21 + num22;
						int num24 = num21 + num23;
						double num25 = this.bins[index3];
						this.bins[index3] = 0.0;
						RingBuffer<double> ringBuffer = this.bins;
						int index2 = num24;
						ringBuffer[index2] += num25;
					}
				}
				for (int k = 0; k < num10 - num11; k++)
				{
					this.bins.PopFront();
				}
				for (int l = 0; l < num12 - num13; l++)
				{
					this.bins.PopBack();
				}
			}
			this.resolution -= num;
			this.minExp >>= num;
			this.maxExp >>= num;
			return num;
		}

		private int Exponent(double value)
		{
			return Histogram.Exponent(this.resolution, value);
		}

		private int BinExponent(int index)
		{
			return index + this.minExp;
		}

		public int BinIndex(int exponent)
		{
			int num = exponent - this.minExp;
			Assert.Check(num >= 0);
			return num;
		}

		private double BinLowerBound(int index)
		{
			return Histogram.LowerBound(this.resolution, this.BinExponent(index));
		}

		private double BinMidpoint(int index)
		{
			double num = this.BinLowerBound(index);
			double num2 = this.BinLowerBound(index + 1);
			return (num + num2) * 0.5;
		}

		private static int Resolution(double error)
		{
			double num = Math.Log(2.0);
			double num2 = 1.0 / num;
			double d = (1.0 + error) / (1.0 - error);
			double d2 = Math.Log(d);
			double a = (Math.Log(num) - Math.Log(d2)) * num2;
			return (int)Math.Ceiling(a);
		}

		private static double Contrast(double min, double max)
		{
			Assert.Check(max >= min);
			return Math.Log(max) - Math.Log(min);
		}

		private static int Capacity(double contrast, int resolution)
		{
			double num = Math.Floor(contrast / Math.Log(Histogram.Base(resolution)) + 1.0);
			Assert.Check(num >= 1.0);
			return (int)num;
		}

		private static double MaxRelativeSampleError(int resolution)
		{
			double num = Histogram.Base(resolution);
			return (num - 1.0) / (num + 1.0);
		}

		private static double MaxRelativeQuantileError(int resolution)
		{
			double num = Histogram.Base(resolution);
			return num - 1.0;
		}

		public double Base()
		{
			return Histogram.Base(this.resolution);
		}

		private static double Base(int resolution)
		{
			double num = Math.Log(2.0);
			return Math.Exp(num * Math.Exp(num * (double)(-(double)resolution)));
		}

		private static double CopySign(double x, double s)
		{
			long num = BitConverter.DoubleToInt64Bits(x);
			long num2 = BitConverter.DoubleToInt64Bits(s);
			num &= long.MaxValue;
			num2 &= long.MinValue;
			return BitConverter.Int64BitsToDouble(num | num2);
		}

		private static ValueTuple<double, int> FrExp(double value)
		{
			bool flag = value == 0.0 || double.IsInfinity(value) || double.IsNaN(value);
			ValueTuple<double, int> result;
			if (flag)
			{
				result = new ValueTuple<double, int>(value, 0);
			}
			else
			{
				double d = Math.Log(Math.Abs(value), 2.0);
				int num = (int)Math.Floor(d) + 1;
				double x = value * (1.0 / Math.Pow(2.0, (double)num));
				result = new ValueTuple<double, int>(Histogram.CopySign(x, value), num);
			}
			return result;
		}

		private static int Exponent(int resolution, double value)
		{
			Assert.Check(value != 0.0);
			double num = Math.Log(2.0);
			double num2 = 1.0 / num;
			double value2 = 9.313225746154785E-10;
			bool flag = double.IsSubnormal(value);
			int result;
			if (flag)
			{
				result = Histogram.Exponent(resolution, value2);
			}
			else
			{
				ValueTuple<double, int> valueTuple = Histogram.FrExp(value);
				double item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				double num3 = (double)item2 * Math.Pow(2.0, (double)resolution);
				double num4 = Math.Log(item) * num2 * Math.Pow(2.0, (double)resolution);
				result = (int)Math.Floor(num3 + num4);
			}
			return result;
		}

		private static double LowerBound(int resolution, int exp)
		{
			double num = Math.Log(2.0);
			double num2 = num * Math.Exp(num * (double)(-(double)resolution));
			return Math.Exp(num2 * (double)exp);
		}

		[CompilerGenerated]
		internal static double <QuantileWithEstimator>g__Upsample|26_0(double cutCount, ref Histogram.<>c__DisplayClass26_0 A_1)
		{
			double num = (cutCount - A_1.priorCount) / (A_1.binCount + 1.0);
			return A_1.binLowerBound + num * (A_1.binUpperBound - A_1.binLowerBound);
		}

		private int resolution;

		private int minExp;

		private int maxExp;

		private double count;

		private double zeroCount;

		private readonly RingBuffer<double> bins;

		internal const int MaxResolution = 20;

		internal const int MaxCapacity = 2048;

		public enum QuantileEstimator
		{
			HyndmanFanType1,
			HyndmanFanType2,
			HyndmanFanType3,
			HyndmanFanType4,
			HyndmanFanType5,
			HyndmanFanType6,
			HyndmanFanType7,
			HyndmanFanType8,
			HyndmanFanType9
		}
	}
}
