using System;
using System.Runtime.Serialization;
using Unity;

namespace System.Transactions
{
	/// <summary>Describes a clone of a transaction providing guarantee that the transaction cannot be committed until the application comes to rest regarding work on the transaction. This class cannot be inherited.</summary>
	[MonoTODO("Not supported yet")]
	[Serializable]
	public sealed class DependentTransaction : Transaction, ISerializable
	{
		internal DependentTransaction(Transaction parent, DependentCloneOption option) : base(parent.IsolationLevel)
		{
		}

		internal bool Completed
		{
			get
			{
				return this.completed;
			}
		}

		/// <summary>Attempts to complete the dependent transaction.</summary>
		/// <exception cref="T:System.Transactions.TransactionException">Any attempt for additional work on the transaction after this method is called. These include invoking methods such as <see cref="Overload:System.Transactions.Transaction.EnlistVolatile" />, <see cref="Overload:System.Transactions.Transaction.EnlistDurable" />, <see cref="M:System.Transactions.Transaction.Clone" />, <see cref="M:System.Transactions.Transaction.DependentClone(System.Transactions.DependentCloneOption)" /> , or any serialization operations on the transaction.</exception>
		[MonoTODO]
		public void Complete()
		{
			throw new NotImplementedException();
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.completed = info.GetBoolean("completed");
		}

		internal DependentTransaction()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool completed;
	}
}
