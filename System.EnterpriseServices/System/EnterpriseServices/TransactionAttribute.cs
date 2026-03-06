using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Specifies the type of transaction that is available to the attributed object. Permissible values are members of the <see cref="T:System.EnterpriseServices.TransactionOption" /> enumeration.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class TransactionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.TransactionAttribute" /> class, setting the component's requested transaction type to <see cref="F:System.EnterpriseServices.TransactionOption.Required" />.</summary>
		public TransactionAttribute() : this(TransactionOption.Required)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.TransactionAttribute" /> class, specifying the transaction type.</summary>
		/// <param name="val">The specified transaction type, a <see cref="T:System.EnterpriseServices.TransactionOption" /> value.</param>
		public TransactionAttribute(TransactionOption val)
		{
			this.isolation = TransactionIsolationLevel.Serializable;
			this.timeout = -1;
			this.val = val;
		}

		/// <summary>Gets or sets the transaction isolation level.</summary>
		/// <returns>One of the <see cref="T:System.EnterpriseServices.TransactionIsolationLevel" /> values.</returns>
		public TransactionIsolationLevel Isolation
		{
			get
			{
				return this.isolation;
			}
			set
			{
				this.isolation = value;
			}
		}

		/// <summary>Gets or sets the time-out for this transaction.</summary>
		/// <returns>The transaction time-out in seconds.</returns>
		public int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		/// <summary>Gets the <see cref="T:System.EnterpriseServices.TransactionOption" /> value for the transaction, optionally disabling the transaction service.</summary>
		/// <returns>The specified transaction type, a <see cref="T:System.EnterpriseServices.TransactionOption" /> value.</returns>
		public TransactionOption Value
		{
			get
			{
				return this.val;
			}
		}

		private TransactionIsolationLevel isolation;

		private int timeout;

		private TransactionOption val;
	}
}
