using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Specifies the values allowed for transaction outcome voting.</summary>
	[ComVisible(false)]
	[Serializable]
	public enum TransactionVote
	{
		/// <summary>Aborts the current transaction.</summary>
		Abort = 1,
		/// <summary>Commits the current transaction.</summary>
		Commit = 0
	}
}
