using System;

namespace System.Configuration
{
	/// <summary>Declaratively instructs the .NET Framework to perform time validation on a configuration property. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class TimeSpanValidatorAttribute : ConfigurationValidatorAttribute
	{
		/// <summary>Gets or sets the relative maximum <see cref="T:System.TimeSpan" /> value.</summary>
		/// <returns>The allowed maximum <see cref="T:System.TimeSpan" /> value.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The selected value represents less than <see cref="P:System.Configuration.TimeSpanValidatorAttribute.MinValue" />.</exception>
		public string MaxValueString
		{
			get
			{
				return this.maxValueString;
			}
			set
			{
				this.maxValueString = value;
				this.instance = null;
			}
		}

		/// <summary>Gets or sets the relative minimum <see cref="T:System.TimeSpan" /> value.</summary>
		/// <returns>The minimum allowed <see cref="T:System.TimeSpan" /> value.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The selected value represents more than <see cref="P:System.Configuration.TimeSpanValidatorAttribute.MaxValue" />.</exception>
		public string MinValueString
		{
			get
			{
				return this.minValueString;
			}
			set
			{
				this.minValueString = value;
				this.instance = null;
			}
		}

		/// <summary>Gets the absolute maximum <see cref="T:System.TimeSpan" /> value.</summary>
		/// <returns>The allowed maximum <see cref="T:System.TimeSpan" /> value.</returns>
		public TimeSpan MaxValue
		{
			get
			{
				return TimeSpan.Parse(this.maxValueString);
			}
		}

		/// <summary>Gets the absolute minimum <see cref="T:System.TimeSpan" /> value.</summary>
		/// <returns>The allowed minimum <see cref="T:System.TimeSpan" /> value.</returns>
		public TimeSpan MinValue
		{
			get
			{
				return TimeSpan.Parse(this.minValueString);
			}
		}

		/// <summary>Gets or sets a value that indicates whether to include or exclude the integers in the range as defined by <see cref="P:System.Configuration.TimeSpanValidatorAttribute.MinValueString" /> and <see cref="P:System.Configuration.TimeSpanValidatorAttribute.MaxValueString" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the value must be excluded; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool ExcludeRange
		{
			get
			{
				return this.excludeRange;
			}
			set
			{
				this.excludeRange = value;
				this.instance = null;
			}
		}

		/// <summary>Gets an instance of the <see cref="T:System.Configuration.TimeSpanValidator" /> class.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationValidatorBase" /> validator instance.</returns>
		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new TimeSpanValidator(this.MinValue, this.MaxValue, this.excludeRange);
				}
				return this.instance;
			}
		}

		private bool excludeRange;

		private string maxValueString = "10675199.02:48:05.4775807";

		private string minValueString = "-10675199.02:48:05.4775808";

		/// <summary>Gets the absolute maximum value allowed.</summary>
		public const string TimeSpanMaxValue = "10675199.02:48:05.4775807";

		/// <summary>Gets the absolute minimum value allowed.</summary>
		public const string TimeSpanMinValue = "-10675199.02:48:05.4775808";

		private ConfigurationValidatorBase instance;
	}
}
