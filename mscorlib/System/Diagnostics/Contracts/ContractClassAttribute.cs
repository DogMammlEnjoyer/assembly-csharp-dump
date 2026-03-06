using System;

namespace System.Diagnostics.Contracts
{
	/// <summary>Specifies that a separate type contains the code contracts for this type.</summary>
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[Conditional("DEBUG")]
	public sealed class ContractClassAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Contracts.ContractClassAttribute" /> class.</summary>
		/// <param name="typeContainingContracts">The type that contains the code contracts for this type.</param>
		public ContractClassAttribute(Type typeContainingContracts)
		{
			this._typeWithContracts = typeContainingContracts;
		}

		/// <summary>Gets the type that contains the code contracts for this type.</summary>
		/// <returns>The type that contains the code contracts for this type.</returns>
		public Type TypeContainingContracts
		{
			get
			{
				return this._typeWithContracts;
			}
		}

		private Type _typeWithContracts;
	}
}
