using System;

namespace System.Reflection
{
	/// <summary>Defines a trademark custom attribute for an assembly manifest.</summary>
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyTrademarkAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.AssemblyTrademarkAttribute" /> class.</summary>
		/// <param name="trademark">The trademark information.</param>
		public AssemblyTrademarkAttribute(string trademark)
		{
			this.Trademark = trademark;
		}

		/// <summary>Gets trademark information.</summary>
		/// <returns>A <see langword="String" /> containing trademark information.</returns>
		public string Trademark { get; }
	}
}
