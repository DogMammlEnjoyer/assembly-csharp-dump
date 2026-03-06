using System;

namespace System.Runtime.CompilerServices
{
	/// <summary>Specifies a destination <see cref="T:System.Type" /> in another assembly.</summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	public sealed class TypeForwardedToAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.TypeForwardedToAttribute" /> class specifying a destination <see cref="T:System.Type" />.</summary>
		/// <param name="destination">The destination <see cref="T:System.Type" /> in another assembly.</param>
		public TypeForwardedToAttribute(Type destination)
		{
			this.Destination = destination;
		}

		/// <summary>Gets the destination <see cref="T:System.Type" /> in another assembly.</summary>
		/// <returns>The destination <see cref="T:System.Type" /> in another assembly.</returns>
		public Type Destination { get; }
	}
}
