using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Sets the default value of a parameter when called from a language that supports default parameters. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class DefaultParameterValueAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.DefaultParameterValueAttribute" /> class with the default value of a parameter.</summary>
		/// <param name="value">An object that represents the default value of a parameter.</param>
		public DefaultParameterValueAttribute(object value)
		{
			this.value = value;
		}

		/// <summary>Gets the default value of a parameter.</summary>
		/// <returns>An object that represents the default value of a parameter.</returns>
		public object Value
		{
			get
			{
				return this.value;
			}
		}

		private object value;
	}
}
