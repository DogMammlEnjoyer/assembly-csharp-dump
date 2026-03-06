using System;

namespace System.Configuration
{
	/// <summary>Provides validation of an <see cref="T:System.Int64" /> value.</summary>
	public class LongValidator : ConfigurationValidatorBase
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.LongValidator" /> class.</summary>
		/// <param name="minValue">An <see cref="T:System.Int64" /> value that specifies the minimum length of the <see langword="long" /> value.</param>
		/// <param name="maxValue">An <see cref="T:System.Int64" /> value that specifies the maximum length of the <see langword="long" /> value.</param>
		/// <param name="rangeIsExclusive">A <see cref="T:System.Boolean" /> value that specifies whether the validation range is exclusive.</param>
		/// <param name="resolution">An <see cref="T:System.Int64" /> value that specifies a specific value that must be matched.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="resolution" /> is equal to or less than <see langword="0" />.  
		/// -or-
		///  <paramref name="maxValue" /> is less than <paramref name="minValue" />.</exception>
		public LongValidator(long minValue, long maxValue, bool rangeIsExclusive, long resolution)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.rangeIsExclusive = rangeIsExclusive;
			this.resolution = resolution;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.LongValidator" /> class.</summary>
		/// <param name="minValue">An <see cref="T:System.Int64" /> value that specifies the minimum length of the <see langword="long" /> value.</param>
		/// <param name="maxValue">An <see cref="T:System.Int64" /> value that specifies the maximum length of the <see langword="long" /> value.</param>
		/// <param name="rangeIsExclusive">A <see cref="T:System.Boolean" /> value that specifies whether the validation range is exclusive.</param>
		public LongValidator(long minValue, long maxValue, bool rangeIsExclusive) : this(minValue, maxValue, rangeIsExclusive, 0L)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.LongValidator" /> class.</summary>
		/// <param name="minValue">An <see cref="T:System.Int64" /> value that specifies the minimum length of the <see langword="long" /> value.</param>
		/// <param name="maxValue">An <see cref="T:System.Int64" /> value that specifies the maximum length of the <see langword="long" /> value.</param>
		public LongValidator(long minValue, long maxValue) : this(minValue, maxValue, false, 0L)
		{
		}

		/// <summary>Determines whether the type of the object can be validated.</summary>
		/// <param name="type">The type of object.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="type" /> parameter matches an <see cref="T:System.Int64" /> value; otherwise, <see langword="false" />.</returns>
		public override bool CanValidate(Type type)
		{
			return type == typeof(long);
		}

		/// <summary>Determines whether the value of an object is valid.</summary>
		/// <param name="value">The value of an object.</param>
		public override void Validate(object value)
		{
			long num = (long)value;
			if (!this.rangeIsExclusive)
			{
				if (num < this.minValue || num > this.maxValue)
				{
					throw new ArgumentException("The value must be in the range " + this.minValue.ToString() + " - " + this.maxValue.ToString());
				}
			}
			else if (num >= this.minValue && num <= this.maxValue)
			{
				throw new ArgumentException("The value must not be in the range " + this.minValue.ToString() + " - " + this.maxValue.ToString());
			}
			if (this.resolution != 0L && num % this.resolution != 0L)
			{
				throw new ArgumentException("The value must have a resolution of " + this.resolution.ToString());
			}
		}

		private bool rangeIsExclusive;

		private long minValue;

		private long maxValue;

		private long resolution;
	}
}
