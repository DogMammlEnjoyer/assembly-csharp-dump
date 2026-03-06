using System;

namespace System.Diagnostics.Contracts
{
	/// <summary>Marks a method as being the invariant method for a class.</summary>
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class ContractInvariantMethodAttribute : Attribute
	{
	}
}
