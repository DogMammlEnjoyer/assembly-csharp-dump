using System;
using System.Threading;
using Unity;

namespace System.ComponentModel
{
	/// <summary>Tracks the lifetime of an asynchronous operation.</summary>
	public sealed class AsyncOperation
	{
		private AsyncOperation(object userSuppliedState, SynchronizationContext syncContext)
		{
			this._userSuppliedState = userSuppliedState;
			this._syncContext = syncContext;
			this._alreadyCompleted = false;
			this._syncContext.OperationStarted();
		}

		/// <summary>Finalizes the asynchronous operation.</summary>
		~AsyncOperation()
		{
			if (!this._alreadyCompleted && this._syncContext != null)
			{
				this._syncContext.OperationCompleted();
			}
		}

		/// <summary>Gets or sets an object used to uniquely identify an asynchronous operation.</summary>
		/// <returns>The state object passed to the asynchronous method invocation.</returns>
		public object UserSuppliedState
		{
			get
			{
				return this._userSuppliedState;
			}
		}

		/// <summary>Gets the <see cref="T:System.Threading.SynchronizationContext" /> object that was passed to the constructor.</summary>
		/// <returns>The <see cref="T:System.Threading.SynchronizationContext" /> object that was passed to the constructor.</returns>
		public SynchronizationContext SynchronizationContext
		{
			get
			{
				return this._syncContext;
			}
		}

		/// <summary>Invokes a delegate on the thread or context appropriate for the application model.</summary>
		/// <param name="d">A <see cref="T:System.Threading.SendOrPostCallback" /> object that wraps the delegate to be called when the operation ends.</param>
		/// <param name="arg">An argument for the delegate contained in the <paramref name="d" /> parameter.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="M:System.ComponentModel.AsyncOperation.PostOperationCompleted(System.Threading.SendOrPostCallback,System.Object)" /> method has been called previously for this task.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		public void Post(SendOrPostCallback d, object arg)
		{
			this.PostCore(d, arg, false);
		}

		/// <summary>Ends the lifetime of an asynchronous operation.</summary>
		/// <param name="d">A <see cref="T:System.Threading.SendOrPostCallback" /> object that wraps the delegate to be called when the operation ends.</param>
		/// <param name="arg">An argument for the delegate contained in the <paramref name="d" /> parameter.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.ComponentModel.AsyncOperation.OperationCompleted" /> has been called previously for this task.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="d" /> is <see langword="null" />.</exception>
		public void PostOperationCompleted(SendOrPostCallback d, object arg)
		{
			this.PostCore(d, arg, true);
			this.OperationCompletedCore();
		}

		/// <summary>Ends the lifetime of an asynchronous operation.</summary>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.ComponentModel.AsyncOperation.OperationCompleted" /> has been called previously for this task.</exception>
		public void OperationCompleted()
		{
			this.VerifyNotCompleted();
			this._alreadyCompleted = true;
			this.OperationCompletedCore();
		}

		private void PostCore(SendOrPostCallback d, object arg, bool markCompleted)
		{
			this.VerifyNotCompleted();
			this.VerifyDelegateNotNull(d);
			if (markCompleted)
			{
				this._alreadyCompleted = true;
			}
			this._syncContext.Post(d, arg);
		}

		private void OperationCompletedCore()
		{
			try
			{
				this._syncContext.OperationCompleted();
			}
			finally
			{
				GC.SuppressFinalize(this);
			}
		}

		private void VerifyNotCompleted()
		{
			if (this._alreadyCompleted)
			{
				throw new InvalidOperationException("This operation has already had OperationCompleted called on it and further calls are illegal.");
			}
		}

		private void VerifyDelegateNotNull(SendOrPostCallback d)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d", "A non-null SendOrPostCallback must be supplied.");
			}
		}

		internal static AsyncOperation CreateOperation(object userSuppliedState, SynchronizationContext syncContext)
		{
			return new AsyncOperation(userSuppliedState, syncContext);
		}

		internal AsyncOperation()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly SynchronizationContext _syncContext;

		private readonly object _userSuppliedState;

		private bool _alreadyCompleted;
	}
}
