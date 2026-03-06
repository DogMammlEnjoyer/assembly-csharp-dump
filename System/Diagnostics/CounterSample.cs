using System;

namespace System.Diagnostics
{
	/// <summary>Defines a structure that holds the raw data for a performance counter.</summary>
	public struct CounterSample
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CounterSample" /> structure and sets the <see cref="P:System.Diagnostics.CounterSample.CounterTimeStamp" /> property to 0 (zero).</summary>
		/// <param name="rawValue">The numeric value associated with the performance counter sample.</param>
		/// <param name="baseValue">An optional, base raw value for the counter, to use only if the sample is based on multiple counters.</param>
		/// <param name="counterFrequency">The frequency with which the counter is read.</param>
		/// <param name="systemFrequency">The frequency with which the system reads from the counter.</param>
		/// <param name="timeStamp">The raw time stamp.</param>
		/// <param name="timeStamp100nSec">The raw, high-fidelity time stamp.</param>
		/// <param name="counterType">A <see cref="T:System.Diagnostics.PerformanceCounterType" /> object that indicates the type of the counter for which this sample is a snapshot.</param>
		public CounterSample(long rawValue, long baseValue, long counterFrequency, long systemFrequency, long timeStamp, long timeStamp100nSec, PerformanceCounterType counterType)
		{
			this = new CounterSample(rawValue, baseValue, counterFrequency, systemFrequency, timeStamp, timeStamp100nSec, counterType, 0L);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CounterSample" /> structure and sets the <see cref="P:System.Diagnostics.CounterSample.CounterTimeStamp" /> property to the value that is passed in.</summary>
		/// <param name="rawValue">The numeric value associated with the performance counter sample.</param>
		/// <param name="baseValue">An optional, base raw value for the counter, to use only if the sample is based on multiple counters.</param>
		/// <param name="counterFrequency">The frequency with which the counter is read.</param>
		/// <param name="systemFrequency">The frequency with which the system reads from the counter.</param>
		/// <param name="timeStamp">The raw time stamp.</param>
		/// <param name="timeStamp100nSec">The raw, high-fidelity time stamp.</param>
		/// <param name="counterType">A <see cref="T:System.Diagnostics.PerformanceCounterType" /> object that indicates the type of the counter for which this sample is a snapshot.</param>
		/// <param name="counterTimeStamp">The time at which the sample was taken.</param>
		public CounterSample(long rawValue, long baseValue, long counterFrequency, long systemFrequency, long timeStamp, long timeStamp100nSec, PerformanceCounterType counterType, long counterTimeStamp)
		{
			this.rawValue = rawValue;
			this.baseValue = baseValue;
			this.counterFrequency = counterFrequency;
			this.systemFrequency = systemFrequency;
			this.timeStamp = timeStamp;
			this.timeStamp100nSec = timeStamp100nSec;
			this.counterType = counterType;
			this.counterTimeStamp = counterTimeStamp;
		}

		/// <summary>Gets an optional, base raw value for the counter.</summary>
		/// <returns>The base raw value, which is used only if the sample is based on multiple counters.</returns>
		public long BaseValue
		{
			get
			{
				return this.baseValue;
			}
		}

		/// <summary>Gets the raw counter frequency.</summary>
		/// <returns>The frequency with which the counter is read.</returns>
		public long CounterFrequency
		{
			get
			{
				return this.counterFrequency;
			}
		}

		/// <summary>Gets the counter's time stamp.</summary>
		/// <returns>The time at which the sample was taken.</returns>
		public long CounterTimeStamp
		{
			get
			{
				return this.counterTimeStamp;
			}
		}

		/// <summary>Gets the performance counter type.</summary>
		/// <returns>A <see cref="T:System.Diagnostics.PerformanceCounterType" /> object that indicates the type of the counter for which this sample is a snapshot.</returns>
		public PerformanceCounterType CounterType
		{
			get
			{
				return this.counterType;
			}
		}

		/// <summary>Gets the raw value of the counter.</summary>
		/// <returns>The numeric value that is associated with the performance counter sample.</returns>
		public long RawValue
		{
			get
			{
				return this.rawValue;
			}
		}

		/// <summary>Gets the raw system frequency.</summary>
		/// <returns>The frequency with which the system reads from the counter.</returns>
		public long SystemFrequency
		{
			get
			{
				return this.systemFrequency;
			}
		}

		/// <summary>Gets the raw time stamp.</summary>
		/// <returns>The system time stamp.</returns>
		public long TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
		}

		/// <summary>Gets the raw, high-fidelity time stamp.</summary>
		/// <returns>The system time stamp, represented within 0.1 millisecond.</returns>
		public long TimeStamp100nSec
		{
			get
			{
				return this.timeStamp100nSec;
			}
		}

		/// <summary>Calculates the performance data of the counter, using a single sample point. This method is generally used for uncalculated performance counter types.</summary>
		/// <param name="counterSample">The <see cref="T:System.Diagnostics.CounterSample" /> structure to use as a base point for calculating performance data.</param>
		/// <returns>The calculated performance value.</returns>
		public static float Calculate(CounterSample counterSample)
		{
			return CounterSampleCalculator.ComputeCounterValue(counterSample);
		}

		/// <summary>Calculates the performance data of the counter, using two sample points. This method is generally used for calculated performance counter types, such as averages.</summary>
		/// <param name="counterSample">The <see cref="T:System.Diagnostics.CounterSample" /> structure to use as a base point for calculating performance data.</param>
		/// <param name="nextCounterSample">The <see cref="T:System.Diagnostics.CounterSample" /> structure to use as an ending point for calculating performance data.</param>
		/// <returns>The calculated performance value.</returns>
		public static float Calculate(CounterSample counterSample, CounterSample nextCounterSample)
		{
			return CounterSampleCalculator.ComputeCounterValue(counterSample, nextCounterSample);
		}

		/// <summary>Indicates whether the specified structure is a <see cref="T:System.Diagnostics.CounterSample" /> structure and is identical to the current <see cref="T:System.Diagnostics.CounterSample" /> structure.</summary>
		/// <param name="o">The <see cref="T:System.Diagnostics.CounterSample" /> structure to be compared with the current structure.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="o" /> is a <see cref="T:System.Diagnostics.CounterSample" /> structure and is identical to the current instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			return o is CounterSample && this.Equals((CounterSample)o);
		}

		/// <summary>Indicates whether the specified <see cref="T:System.Diagnostics.CounterSample" /> structure is equal to the current <see cref="T:System.Diagnostics.CounterSample" /> structure.</summary>
		/// <param name="sample">The <see cref="T:System.Diagnostics.CounterSample" /> structure to be compared with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="sample" /> is equal to the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(CounterSample sample)
		{
			return this.rawValue == sample.rawValue && this.baseValue == sample.counterFrequency && this.counterFrequency == sample.counterFrequency && this.systemFrequency == sample.systemFrequency && this.timeStamp == sample.timeStamp && this.timeStamp100nSec == sample.timeStamp100nSec && this.counterTimeStamp == sample.counterTimeStamp && this.counterType == sample.counterType;
		}

		/// <summary>Returns a value that indicates whether two <see cref="T:System.Diagnostics.CounterSample" /> structures are equal.</summary>
		/// <param name="a">A <see cref="T:System.Diagnostics.CounterSample" /> structure.</param>
		/// <param name="b">Another <see cref="T:System.Diagnostics.CounterSample" /> structure to be compared to the structure specified by the <paramref name="a" /> parameter.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> and <paramref name="b" /> are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(CounterSample a, CounterSample b)
		{
			return a.Equals(b);
		}

		/// <summary>Returns a value that indicates whether two <see cref="T:System.Diagnostics.CounterSample" /> structures are not equal.</summary>
		/// <param name="a">A <see cref="T:System.Diagnostics.CounterSample" /> structure.</param>
		/// <param name="b">Another <see cref="T:System.Diagnostics.CounterSample" /> structure to be compared to the structure specified by the <paramref name="a" /> parameter.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> and <paramref name="b" /> are not equal; otherwise, <see langword="false" /></returns>
		public static bool operator !=(CounterSample a, CounterSample b)
		{
			return !a.Equals(b);
		}

		/// <summary>Gets a hash code for the current counter sample.</summary>
		/// <returns>A hash code for the current counter sample.</returns>
		public override int GetHashCode()
		{
			return (int)(this.rawValue << 28 ^ (this.baseValue << 24 ^ (this.counterFrequency << 20 ^ (this.systemFrequency << 16 ^ (this.timeStamp << 8 ^ (this.timeStamp100nSec << 4 ^ (this.counterTimeStamp ^ (long)this.counterType)))))));
		}

		private long rawValue;

		private long baseValue;

		private long counterFrequency;

		private long systemFrequency;

		private long timeStamp;

		private long timeStamp100nSec;

		private long counterTimeStamp;

		private PerformanceCounterType counterType;

		/// <summary>Defines an empty, uninitialized performance counter sample of type <see langword="NumberOfItems32" />.</summary>
		public static CounterSample Empty = new CounterSample(0L, 0L, 0L, 0L, 0L, 0L, PerformanceCounterType.NumberOfItems32, 0L);
	}
}
