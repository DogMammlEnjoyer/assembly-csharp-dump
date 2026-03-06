using System;

namespace System.Diagnostics.Contracts
{
	/// <summary>Instructs analysis tools to assume the correctness of an assembly, type, or member without performing static verification.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	[Conditional("CONTRACTS_FULL")]
	public sealed class ContractVerificationAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Contracts.ContractVerificationAttribute" /> class.</summary>
		/// <param name="value">
		///   <see langword="true" /> to require verification; otherwise, <see langword="false" />.</param>
		public ContractVerificationAttribute(bool value)
		{
			this._value = value;
		}

		/// <summary>Gets the value that indicates whether to verify the contract of the target.</summary>
		/// <returns>
		///   <see langword="true" /> if verification is required; otherwise, <see langword="false" />.</returns>
		public bool Value
		{
			get
			{
				return this._value;
			}
		}

		private bool _value;
	}
}
