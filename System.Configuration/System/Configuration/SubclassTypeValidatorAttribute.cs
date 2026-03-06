using System;

namespace System.Configuration
{
	/// <summary>Declaratively instructs the .NET Framework to perform validation on a configuration property. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SubclassTypeValidatorAttribute : ConfigurationValidatorAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SubclassTypeValidatorAttribute" /> class.</summary>
		/// <param name="baseClass">The base class type.</param>
		public SubclassTypeValidatorAttribute(Type baseClass)
		{
			this.baseClass = baseClass;
		}

		/// <summary>Gets the base type of the object being validated.</summary>
		/// <returns>The base type of the object being validated.</returns>
		public Type BaseClass
		{
			get
			{
				return this.baseClass;
			}
		}

		/// <summary>Gets the validator attribute instance.</summary>
		/// <returns>The current <see cref="T:System.Configuration.ConfigurationValidatorBase" /> instance.</returns>
		public override ConfigurationValidatorBase ValidatorInstance
		{
			get
			{
				if (this.instance == null)
				{
					this.instance = new SubclassTypeValidator(this.baseClass);
				}
				return this.instance;
			}
		}

		private Type baseClass;

		private ConfigurationValidatorBase instance;
	}
}
