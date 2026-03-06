using System;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Transactions
{
	/// <summary>Describes a committable transaction.</summary>
	[Serializable]
	public sealed class CommittableTransaction : Transaction, ISerializable, IDisposable, IAsyncResult
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.CommittableTransaction" /> class.</summary>
		/// <exception cref="T:System.PlatformNotSupportedException">An attempt to create a transaction under Windows 98, Windows 98 Second Edition or Windows Millennium Edition.</exception>
		public CommittableTransaction() : this(default(TransactionOptions))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.CommittableTransaction" /> class with the specified <paramref name="timeout" /> value.</summary>
		/// <param name="timeout">The maximum amount of time the transaction can exist, before it is aborted.</param>
		/// <exception cref="T:System.PlatformNotSupportedException">An attempt to create a transaction under Windows 98, Windows 98 Second Edition or Windows Millennium Edition.</exception>
		public CommittableTransaction(TimeSpan timeout) : base(IsolationLevel.Serializable)
		{
			this.options = default(TransactionOptions);
			this.options.Timeout = timeout;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.CommittableTransaction" /> class with the specified transaction options.</summary>
		/// <param name="options">A <see cref="T:System.Transactions.TransactionOptions" /> structure that describes the transaction options to use for the new transaction.</param>
		/// <exception cref="T:System.PlatformNotSupportedException">An attempt to create a transaction under Windows 98, Windows 98 Second Edition or Windows Millennium Edition.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="options" /> is invalid.</exception>
		public CommittableTransaction(TransactionOptions options) : base(options.IsolationLevel)
		{
			this.options = options;
		}

		/// <summary>Begins an attempt to commit the transaction asynchronously.</summary>
		/// <param name="asyncCallback">The <see cref="T:System.AsyncCallback" /> delegate that is invoked when the transaction completes. This parameter can be <see langword="null" />, in which case the application is not notified of the transaction's completion. Instead, the application must use the <see cref="T:System.IAsyncResult" /> interface to check for completion and wait accordingly, or call <see cref="M:System.Transactions.CommittableTransaction.EndCommit(System.IAsyncResult)" /> to wait for completion.</param>
		/// <param name="asyncState">An object, which might contain arbitrary state information, associated with the asynchronous commitment. This object is passed to the callback, and is not interpreted by <see cref="N:System.Transactions" />. A null reference is permitted.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> interface that can be used by the caller to check the status of the asynchronous operation, or to wait for the operation to complete.</returns>
		public IAsyncResult BeginCommit(AsyncCallback asyncCallback, object asyncState)
		{
			this.callback = asyncCallback;
			this.user_defined_state = asyncState;
			AsyncCallback asyncCallback2 = null;
			if (asyncCallback != null)
			{
				asyncCallback2 = new AsyncCallback(this.CommitCallback);
			}
			this.asyncResult = base.BeginCommitInternal(asyncCallback2);
			return this;
		}

		/// <summary>Ends an attempt to commit the transaction asynchronously.</summary>
		/// <param name="asyncResult">The <see cref="T:System.IAsyncResult" /> object associated with the asynchronous commitment.</param>
		/// <exception cref="T:System.Transactions.TransactionAbortedException">
		///   <see cref="M:System.Transactions.CommittableTransaction.BeginCommit(System.AsyncCallback,System.Object)" /> is called and the transaction rolls back for the first time.</exception>
		public void EndCommit(IAsyncResult asyncResult)
		{
			if (asyncResult != this)
			{
				throw new ArgumentException("The IAsyncResult parameter must be the same parameter as returned by BeginCommit.", "asyncResult");
			}
			base.EndCommitInternal(this.asyncResult);
		}

		private void CommitCallback(IAsyncResult ar)
		{
			if (this.asyncResult == null && ar.CompletedSynchronously)
			{
				this.asyncResult = ar;
			}
			this.callback(this);
		}

		/// <summary>Attempts to commit the transaction.</summary>
		/// <exception cref="T:System.Transactions.TransactionInDoubtException">
		///   <see cref="M:System.Transactions.CommittableTransaction.Commit" /> is called on a transaction and the transaction becomes <see cref="F:System.Transactions.TransactionStatus.InDoubt" />.</exception>
		/// <exception cref="T:System.Transactions.TransactionAbortedException">
		///   <see cref="M:System.Transactions.CommittableTransaction.Commit" /> is called and the transaction rolls back for the first time.</exception>
		public void Commit()
		{
			base.CommitInternal();
		}

		[MonoTODO("Not implemented")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets the object provided as the last parameter of the <see cref="M:System.Transactions.CommittableTransaction.BeginCommit(System.AsyncCallback,System.Object)" /> method call.</summary>
		/// <returns>The object provided as the last parameter of the <see cref="M:System.Transactions.CommittableTransaction.BeginCommit(System.AsyncCallback,System.Object)" /> method call.</returns>
		object IAsyncResult.AsyncState
		{
			get
			{
				return this.user_defined_state;
			}
		}

		/// <summary>Gets a <see cref="T:System.Threading.WaitHandle" /> that is used to wait for an asynchronous operation to complete.</summary>
		/// <returns>A <see cref="T:System.Threading.WaitHandle" /> that is used to wait for an asynchronous operation to complete.</returns>
		WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get
			{
				return this.asyncResult.AsyncWaitHandle;
			}
		}

		/// <summary>Gets an indication of whether the asynchronous commit operation completed synchronously.</summary>
		/// <returns>
		///   <see langword="true" /> if the asynchronous commit operation completed synchronously; otherwise, <see langword="false" />. This property always returns <see langword="false" /> even if the operation completed synchronously.</returns>
		bool IAsyncResult.CompletedSynchronously
		{
			get
			{
				return this.asyncResult.CompletedSynchronously;
			}
		}

		/// <summary>Gets an indication whether the asynchronous commit operation has completed.</summary>
		/// <returns>
		///   <see langword="true" /> if the operation is complete; otherwise, <see langword="false" />.</returns>
		bool IAsyncResult.IsCompleted
		{
			get
			{
				return this.asyncResult.IsCompleted;
			}
		}

		private TransactionOptions options;

		private AsyncCallback callback;

		private object user_defined_state;

		private IAsyncResult asyncResult;
	}
}
