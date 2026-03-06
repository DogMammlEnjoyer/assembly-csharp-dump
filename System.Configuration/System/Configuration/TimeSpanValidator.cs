using System;

namespace System.Configuration
{
	/// <summary>Provides validation of a <see cref="T:System.TimeSpan" /> object.</summary>
	public class TimeSpanValidator : ConfigurationValidatorBase
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.TimeSpanValidator" /> class, based on supplied parameters.</summary>
		/// <param name="minValue">A <see cref="T:System.TimeSpan" /> object that specifies the minimum time allowed to pass validation.</param>
		/// <param name="maxValue">A <see cref="T:System.TimeSpan" /> object that specifies the maximum time allowed to pass validation.</param>
		public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue) : this(minValue, maxValue, false, 0L)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.TimeSpanValidator" /> class, based on supplied parameters.</summary>
		/// <param name="minValue">A <see cref="T:System.TimeSpan" /> object that specifies the minimum time allowed to pass validation.</param>
		/// <param name="maxValue">A <see cref="T:System.TimeSpan" /> object that specifies the maximum time allowed to pass validation.</param>
		/// <param name="rangeIsExclusive">A <see cref="T:System.Boolean" /> value that specifies whether the validation range is exclusive.</param>
		public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive) : this(minValue, maxValue, rangeIsExclusive, 0L)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.TimeSpanValidator" /> class, based on supplied parameters.</summary>
		/// <param name="minValue">A <see cref="T:System.TimeSpan" /> object that specifies the minimum time allowed to pass validation.</param>
		/// <param name="maxValue">A <see cref="T:System.TimeSpan" /> object that specifies the maximum time allowed to pass validation.</param>
		/// <param name="rangeIsExclusive">A <see cref="T:System.Boolean" /> value that specifies whether the validation range is exclusive.</param>
		/// <param name="resolutionInSeconds">An <see cref="T:System.Int64" /> value that specifies a number of seconds.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="resolutionInSeconds" /> is less than <see langword="0" />.  
		/// -or-
		///  <paramref name="minValue" /> is greater than <paramref name="maxValue" />.</exception>
		public TimeSpanValidator(TimeSpan minValue, TimeSpan maxValue, bool rangeIsExclusive, long resolutionInSeconds)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.rangeIsExclusive = rangeIsExclusive;
			this.resolutionInSeconds = resolutionInSeconds;
		}

		/// <summary>Determines whether the type of the object can be validated.</summary>
		/// <param name="type">The type of the object.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="type" /> parameter matches a <see cref="T:System.TimeSpan" /> value; otherwise, <see langword="false" />.</returns>
		public override bool CanValidate(Type type)
		{
			return type == typeof(TimeSpan);
		}

		/// <summary>Determines whether the value of an object is valid.</summary>
		/// <param name="value">The value of an object.</param>
		public override void Validate(object value)
		{
			TimeSpan t = (TimeSpan)value;
			if (!this.rangeIsExclusive)
			{
				if (t < this.minValue || t > this.maxValue)
				{
					throw new ArgumentException("The value must be in the range " + this.minValue.ToString() + " - " + this.maxValue.ToString());
				}
			}
			else if (t >= this.minValue && t <= this.maxValue)
			{
				throw new ArgumentException("The value must not be in the range " + this.minValue.ToString() + " - " + this.maxValue.ToString());
			}
			if (this.resolutionInSeconds != 0L && t.Ticks % (10000000L * this.resolutionInSeconds) != 0L)
			{
				throw new ArgumentException("The value must have a resolution of " + TimeSpan.FromTicks(10000000L * this.resolutionInSeconds).ToString());
			}
		}

		private bool rangeIsExclusive;

		private TimeSpan minValue;

		private TimeSpan maxValue;

		private long resolutionInSeconds;
	}
}
