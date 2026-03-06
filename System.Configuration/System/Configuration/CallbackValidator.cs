using System;

namespace System.Configuration
{
	/// <summary>Provides dynamic validation of an object.</summary>
	public sealed class CallbackValidator : ConfigurationValidatorBase
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.CallbackValidator" /> class.</summary>
		/// <param name="type">The type of object that will be validated.</param>
		/// <param name="callback">The <see cref="T:System.Configuration.ValidatorCallback" /> used as the delegate.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		public CallbackValidator(Type type, ValidatorCallback callback)
		{
			this.type = type;
			this.callback = callback;
		}

		/// <summary>Determines whether the type of the object can be validated.</summary>
		/// <param name="type">The type of object.</param>
		/// <returns>
		///   <see langword="true" /> if the <see langword="type" /> parameter matches the type used as the first parameter when creating an instance of <see cref="T:System.Configuration.CallbackValidator" />; otherwise, <see langword="false" />.</returns>
		public override bool CanValidate(Type type)
		{
			return type == this.type;
		}

		/// <summary>Determines whether the value of an object is valid.</summary>
		/// <param name="value">The value of an object.</param>
		public override void Validate(object value)
		{
			this.callback(value);
		}

		private Type type;

		private ValidatorCallback callback;
	}
}
