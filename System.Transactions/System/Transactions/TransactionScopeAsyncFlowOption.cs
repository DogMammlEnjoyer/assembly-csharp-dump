using System;

namespace System.Transactions
{
	/// <summary>Specifies whether transaction flow across thread continuations is enabled for <see cref="T:System.Transactions.TransactionScope" />.</summary>
	public enum TransactionScopeAsyncFlowOption
	{
		/// <summary>Specifies that transaction flow across thread continuations is suppressed.</summary>
		Suppress,
		/// <summary>Specifies that transaction flow across thread continuations is enabled.</summary>
		Enabled
	}
}
