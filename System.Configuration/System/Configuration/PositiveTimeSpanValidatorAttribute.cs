using System;

namespace System.Configuration
{
	/// <summary>Declaratively instructs the .NET Framework to perform time validation on a configuration property. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class PositiveTimeSpanValidatorAttribute : ConfigurationValidatorAttribute
	{
		/// <summary>Gets an instance of the <see cref="T:System.Configuration.PositiveTimeSpanValidator" /> class.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationValidatorBase" /> validator instance.</returns>
		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new PositiveTimeSpanValidator();
				}
				return this.instance;
			}
		}

		private ConfigurationValidatorBase instance;
	}
}
