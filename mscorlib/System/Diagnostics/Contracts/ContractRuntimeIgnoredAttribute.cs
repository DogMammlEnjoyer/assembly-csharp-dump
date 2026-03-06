using System;

namespace System.Diagnostics.Contracts
{
	/// <summary>Identifies a member that has no run-time behavior.</summary>
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ContractRuntimeIgnoredAttribute : Attribute
	{
	}
}
