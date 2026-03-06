using System;

namespace System.ComponentModel.Design
{
	/// <summary>Specifies methods for the designer host to report on the state of transactions.</summary>
	public interface IDesignerHostTransactionState
	{
		/// <summary>Gets a value indicating whether the designer host is closing a transaction.</summary>
		/// <returns>
		///   <see langword="true" /> if the designer is closing a transaction; otherwise, <see langword="false" />.</returns>
		bool IsClosingTransaction { get; }
	}
}
