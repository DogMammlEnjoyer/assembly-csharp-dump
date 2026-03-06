using System;

namespace System.Configuration
{
	/// <summary>Declaratively instructs the .NET Framework to perform integer validation on a configuration property. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IntegerValidatorAttribute : ConfigurationValidatorAttribute
	{
		/// <summary>Gets or sets a value that indicates whether to include or exclude the integers in the range defined by the <see cref="P:System.Configuration.IntegerValidatorAttribute.MinValue" /> and <see cref="P:System.Configuration.IntegerValidatorAttribute.MaxValue" /> property values.</summary>
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

		/// <summary>Gets or sets the maximum value allowed for the property.</summary>
		/// <returns>An integer that indicates the allowed maximum value.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The selected value is less than <see cref="P:System.Configuration.IntegerValidatorAttribute.MinValue" />.</exception>
		public int MaxValue
		{
			get
			{
				return this.maxValue;
			}
			set
			{
				this.maxValue = value;
				this.instance = null;
			}
		}

		/// <summary>Gets or sets the minimum value allowed for the property.</summary>
		/// <returns>An integer that indicates the allowed minimum value.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The selected value is greater than <see cref="P:System.Configuration.IntegerValidatorAttribute.MaxValue" />.</exception>
		public int MinValue
		{
			get
			{
				return this.minValue;
			}
			set
			{
				this.minValue = value;
				this.instance = null;
			}
		}

		/// <summary>Gets an instance of the <see cref="T:System.Configuration.IntegerValidator" /> class.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationValidatorBase" /> validator instance.</returns>
		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new IntegerValidator(this.minValue, this.maxValue, this.excludeRange);
				}
				return this.instance;
			}
		}

		private bool excludeRange;

		private int maxValue;

		private int minValue;

		private ConfigurationValidatorBase instance;
	}
}
