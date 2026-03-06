using System;
using System.Threading;
using Unity;

namespace System.Transactions
{
	/// <summary>Facilitates communication between an enlisted transaction participant and the transaction manager during the Prepare phase of the transaction.</summary>
	public class PreparingEnlistment : Enlistment
	{
		internal PreparingEnlistment(Transaction tx, IEnlistmentNotification enlisted)
		{
			this.tx = tx;
			this.enlisted = enlisted;
			this.waitHandle = new ManualResetEvent(false);
		}

		/// <summary>Indicates that the transaction should be rolled back.</summary>
		public void ForceRollback()
		{
			this.ForceRollback(null);
		}

		internal override void InternalOnDone()
		{
			this.Prepared();
		}

		/// <summary>Indicates that the transaction should be rolled back.</summary>
		/// <param name="e">An explanation of why a rollback is triggered.</param>
		[MonoTODO]
		public void ForceRollback(Exception e)
		{
			this.tx.Rollback(e, this.enlisted);
			((ManualResetEvent)this.waitHandle).Set();
		}

		/// <summary>Indicates that the transaction can be committed.</summary>
		[MonoTODO]
		public void Prepared()
		{
			this.prepared = true;
			((ManualResetEvent)this.waitHandle).Set();
		}

		/// <summary>Gets the recovery information of an enlistment.</summary>
		/// <returns>The recovery information of an enlistment.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt to get recovery information inside a volatile enlistment, which does not generate any recovery information.</exception>
		[MonoTODO]
		public byte[] RecoveryInformation()
		{
			throw new NotImplementedException();
		}

		internal bool IsPrepared
		{
			get
			{
				return this.prepared;
			}
		}

		internal WaitHandle WaitHandle
		{
			get
			{
				return this.waitHandle;
			}
		}

		internal IEnlistmentNotification EnlistmentNotification
		{
			get
			{
				return this.enlisted;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.ex;
			}
			set
			{
				this.ex = value;
			}
		}

		internal PreparingEnlistment()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool prepared;

		private Transaction tx;

		private IEnlistmentNotification enlisted;

		private WaitHandle waitHandle;

		private Exception ex;
	}
}
