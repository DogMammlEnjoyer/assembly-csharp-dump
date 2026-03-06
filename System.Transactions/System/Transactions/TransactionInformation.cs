using System;

namespace System.Transactions
{
	/// <summary>Provides additional information regarding a transaction.</summary>
	public class TransactionInformation
	{
		internal TransactionInformation()
		{
			this.status = TransactionStatus.Active;
			this.creation_time = DateTime.Now.ToUniversalTime();
			this.local_id = Guid.NewGuid().ToString() + ":1";
		}

		private TransactionInformation(TransactionInformation other)
		{
			this.local_id = other.local_id;
			this.dtcId = other.dtcId;
			this.creation_time = other.creation_time;
			this.status = other.status;
		}

		/// <summary>Gets the creation time of the transaction.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> that contains the creation time of the transaction.</returns>
		public DateTime CreationTime
		{
			get
			{
				return this.creation_time;
			}
		}

		/// <summary>Gets a unique identifier of the escalated transaction.</summary>
		/// <returns>A <see cref="T:System.Guid" /> that contains the unique identifier of the escalated transaction.</returns>
		public Guid DistributedIdentifier
		{
			get
			{
				return this.dtcId;
			}
			internal set
			{
				this.dtcId = value;
			}
		}

		/// <summary>Gets a unique identifier of the transaction.</summary>
		/// <returns>A unique identifier of the transaction.</returns>
		public string LocalIdentifier
		{
			get
			{
				return this.local_id;
			}
		}

		/// <summary>Gets the status of the transaction.</summary>
		/// <returns>A <see cref="T:System.Transactions.TransactionStatus" /> that contains the status of the transaction.</returns>
		public TransactionStatus Status
		{
			get
			{
				return this.status;
			}
			internal set
			{
				this.status = value;
			}
		}

		internal TransactionInformation Clone(TransactionInformation other)
		{
			return new TransactionInformation(other);
		}

		private string local_id;

		private Guid dtcId = Guid.Empty;

		private DateTime creation_time;

		private TransactionStatus status;
	}
}
