using System;

namespace System.EnterpriseServices
{
	/// <summary>Wraps the COM+ <see langword="ByotServerEx" /> class and the COM+ DTC interfaces <see langword="ICreateWithTransactionEx" /> and <see langword="ICreateWithTipTransactionEx" />. This class cannot be inherited.</summary>
	public sealed class BYOT
	{
		private BYOT()
		{
		}

		/// <summary>Creates an object that is enlisted within a manual transaction using the Transaction Internet Protocol (TIP).</summary>
		/// <param name="url">A TIP URL that specifies a transaction.</param>
		/// <param name="t">The type.</param>
		/// <returns>The requested transaction.</returns>
		[MonoTODO]
		public static object CreateWithTipTransaction(string url, Type t)
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates an object that is enlisted within a manual transaction.</summary>
		/// <param name="transaction">The <see cref="T:System.EnterpriseServices.ITransaction" /> or <see cref="T:System.Transactions.Transaction" /> object that specifies a transaction.</param>
		/// <param name="t">The specified type.</param>
		/// <returns>The requested transaction.</returns>
		[MonoTODO]
		public static object CreateWithTransaction(object transaction, Type t)
		{
			throw new NotImplementedException();
		}
	}
}
