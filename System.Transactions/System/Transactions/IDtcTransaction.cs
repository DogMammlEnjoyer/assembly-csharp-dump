using System;
using System.Runtime.InteropServices;

namespace System.Transactions
{
	/// <summary>Describes a DTC transaction.</summary>
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDtcTransaction
	{
		/// <summary>Aborts a transaction.</summary>
		/// <param name="reason">An optional <see cref="T:System.EnterpriseServices.BOID" /> that indicates why the transaction is being aborted. This parameter can be <see langword="null" />, indicating that no reason for the abort is provided.</param>
		/// <param name="retaining">This value must be <see langword="false" />.</param>
		/// <param name="async">When <paramref name="async" /> is <see langword="true" />, an asynchronous abort is performed and the caller must use <see langword="ITransactionOutcomeEvents" /> to learn about the outcome of the transaction.</param>
		void Abort(IntPtr reason, int retaining, int async);

		/// <summary>Commits a transaction.</summary>
		/// <param name="retaining">This value must be <see langword="false" />.</param>
		/// <param name="commitType">A value taken from the OLE DB enumeration <see langword="XACTTC" />.</param>
		/// <param name="reserved">This value must be zero.</param>
		void Commit(int retaining, int commitType, int reserved);

		/// <summary>Retrieves information about a transaction.</summary>
		/// <param name="transactionInformation">Pointer to the caller-allocated <see cref="T:System.EnterpriseServices.XACTTRANSINFO" /> structure that will receive information about the transaction. This value must not be <see langword="null" />.</param>
		void GetTransactionInfo(IntPtr transactionInformation);
	}
}
