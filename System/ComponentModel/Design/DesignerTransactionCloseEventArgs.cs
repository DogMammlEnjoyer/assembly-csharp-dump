using System;

namespace System.ComponentModel.Design
{
	/// <summary>Provides data for the <see cref="E:System.ComponentModel.Design.IDesignerHost.TransactionClosed" /> and <see cref="E:System.ComponentModel.Design.IDesignerHost.TransactionClosing" /> events.</summary>
	public class DesignerTransactionCloseEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.DesignerTransactionCloseEventArgs" /> class, using the specified value that indicates whether the designer called <see cref="M:System.ComponentModel.Design.DesignerTransaction.Commit" /> on the transaction.</summary>
		/// <param name="commit">A value indicating whether the transaction was committed.</param>
		[Obsolete("This constructor is obsolete. Use DesignerTransactionCloseEventArgs(bool, bool) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
		public DesignerTransactionCloseEventArgs(bool commit) : this(commit, true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.DesignerTransactionCloseEventArgs" /> class.</summary>
		/// <param name="commit">A value indicating whether the transaction was committed.</param>
		/// <param name="lastTransaction">
		///   <see langword="true" /> if this is the last transaction to close; otherwise, <see langword="false" />.</param>
		public DesignerTransactionCloseEventArgs(bool commit, bool lastTransaction)
		{
			this.TransactionCommitted = commit;
			this.LastTransaction = lastTransaction;
		}

		/// <summary>Indicates whether the designer called <see cref="M:System.ComponentModel.Design.DesignerTransaction.Commit" /> on the transaction.</summary>
		/// <returns>
		///   <see langword="true" /> if the designer called <see cref="M:System.ComponentModel.Design.DesignerTransaction.Commit" /> on the transaction; otherwise, <see langword="false" />.</returns>
		public bool TransactionCommitted { get; }

		/// <summary>Gets a value indicating whether this is the last transaction to close.</summary>
		/// <returns>
		///   <see langword="true" />, if this is the last transaction to close; otherwise, <see langword="false" />.</returns>
		public bool LastTransaction { get; }
	}
}
