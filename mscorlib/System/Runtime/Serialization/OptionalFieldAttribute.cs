using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	/// <summary>Specifies that a field can be missing from a serialization stream so that the <see cref="T:System.Runtime.Serialization.Formatters.Binary.BinaryFormatter" /> and the <see cref="T:System.Runtime.Serialization.Formatters.Soap.SoapFormatter" /> does not throw an exception.</summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	[ComVisible(true)]
	public sealed class OptionalFieldAttribute : Attribute
	{
		/// <summary>Gets or sets a version number to indicate when the optional field was added.</summary>
		/// <returns>The version of the <see cref="T:System.Runtime.Serialization.OptionalFieldAttribute" />.</returns>
		public int VersionAdded
		{
			get
			{
				return this.versionAdded;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException(Environment.GetResourceString("Version value must be positive."));
				}
				this.versionAdded = value;
			}
		}

		private int versionAdded = 1;
	}
}
