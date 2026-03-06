using System;
using System.Threading;

namespace System.Transactions
{
	/// <summary>Makes a code block transactional. This class cannot be inherited.</summary>
	public sealed class TransactionScope : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class.</summary>
		public TransactionScope() : this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified asynchronous flow option.</summary>
		/// <param name="asyncFlowOption">An instance of the <see cref="T:System.Transactions.TransactionScopeAsyncFlowOption" /> enumeration that describes whether the ambient transaction associated with the transaction scope will flow across thread continuations when using Task or async/await .NET async programming patterns.</param>
		public TransactionScope(TransactionScopeAsyncFlowOption asyncFlowOption) : this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout, asyncFlowOption)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class and sets the specified transaction as the ambient transaction, so that transactional work done inside the scope uses this transaction.</summary>
		/// <param name="transactionToUse">The transaction to be set as the ambient transaction, so that transactional work done inside the scope uses this transaction.</param>
		public TransactionScope(Transaction transactionToUse) : this(transactionToUse, TransactionManager.DefaultTimeout)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified timeout value, and sets the specified transaction as the ambient transaction, so that transactional work done inside the scope uses this transaction.</summary>
		/// <param name="transactionToUse">The transaction to be set as the ambient transaction, so that transactional work done inside the scope uses this transaction.</param>
		/// <param name="scopeTimeout">The <see cref="T:System.TimeSpan" /> after which the transaction scope times out and aborts the transaction.</param>
		public TransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout) : this(transactionToUse, scopeTimeout, EnterpriseServicesInteropOption.None)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified timeout value and COM+ interoperability requirements, and sets the specified transaction as the ambient transaction, so that transactional work done inside the scope uses this transaction.</summary>
		/// <param name="transactionToUse">The transaction to be set as the ambient transaction, so that transactional work done inside the scope uses this transaction.</param>
		/// <param name="scopeTimeout">The <see cref="T:System.TimeSpan" /> after which the transaction scope times out and aborts the transaction.</param>
		/// <param name="interopOption">An instance of the <see cref="T:System.Transactions.EnterpriseServicesInteropOption" /> enumeration that describes how the associated transaction interacts with COM+ transactions.</param>
		[MonoTODO("EnterpriseServicesInteropOption not supported.")]
		public TransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout, EnterpriseServicesInteropOption interopOption)
		{
			this.Initialize(TransactionScopeOption.Required, transactionToUse, TransactionScope.defaultOptions, interopOption, scopeTimeout, TransactionScopeAsyncFlowOption.Suppress);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified requirements.</summary>
		/// <param name="scopeOption">An instance of the <see cref="T:System.Transactions.TransactionScopeOption" /> enumeration that describes the transaction requirements associated with this transaction scope.</param>
		public TransactionScope(TransactionScopeOption scopeOption) : this(scopeOption, TransactionManager.DefaultTimeout)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified timeout value and requirements.</summary>
		/// <param name="scopeOption">An instance of the <see cref="T:System.Transactions.TransactionScopeOption" /> enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="scopeTimeout">The <see cref="T:System.TimeSpan" /> after which the transaction scope times out and aborts the transaction.</param>
		public TransactionScope(TransactionScopeOption scopeOption, TimeSpan scopeTimeout) : this(scopeOption, scopeTimeout, TransactionScopeAsyncFlowOption.Suppress)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified requirements and asynchronous flow option.</summary>
		/// <param name="scopeOption">An instance of the <see cref="T:System.Transactions.TransactionScopeOption" /> enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="asyncFlowOption">An instance of the <see cref="T:System.Transactions.TransactionScopeAsyncFlowOption" /> enumeration that describes whether the ambient transaction associated with the transaction scope will flow across thread continuations when using Task or async/await .NET async programming patterns.</param>
		public TransactionScope(TransactionScopeOption option, TransactionScopeAsyncFlowOption asyncFlow) : this(option, TransactionManager.DefaultTimeout, asyncFlow)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified timeout value, requirements, and asynchronous flow option.</summary>
		/// <param name="scopeOption">An instance of the <see cref="T:System.Transactions.TransactionScopeOption" /> enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="scopeTimeout">The <see cref="T:System.TimeSpan" /> after which the transaction scope times out and aborts the transaction.</param>
		/// <param name="asyncFlowOption">An instance of the <see cref="T:System.Transactions.TransactionScopeAsyncFlowOption" /> enumeration that describes whether the ambient transaction associated with the transaction scope will flow across thread continuations when using Task or async/await .NET async programming patterns.</param>
		public TransactionScope(TransactionScopeOption scopeOption, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlow)
		{
			this.Initialize(scopeOption, null, TransactionScope.defaultOptions, EnterpriseServicesInteropOption.None, scopeTimeout, asyncFlow);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified requirements.</summary>
		/// <param name="scopeOption">An instance of the <see cref="T:System.Transactions.TransactionScopeOption" /> enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="transactionOptions">A <see cref="T:System.Transactions.TransactionOptions" /> structure that describes the transaction options to use if a new transaction is created. If an existing transaction is used, the timeout value in this parameter applies to the transaction scope. If that time expires before the scope is disposed, the transaction is aborted.</param>
		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions) : this(scopeOption, transactionOptions, EnterpriseServicesInteropOption.None)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified scope and COM+ interoperability requirements, and transaction options.</summary>
		/// <param name="scopeOption">An instance of the <see cref="T:System.Transactions.TransactionScopeOption" /> enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="transactionOptions">A <see cref="T:System.Transactions.TransactionOptions" /> structure that describes the transaction options to use if a new transaction is created. If an existing transaction is used, the timeout value in this parameter applies to the transaction scope. If that time expires before the scope is disposed, the transaction is aborted.</param>
		/// <param name="interopOption">An instance of the <see cref="T:System.Transactions.EnterpriseServicesInteropOption" /> enumeration that describes how the associated transaction interacts with COM+ transactions.</param>
		[MonoTODO("EnterpriseServicesInteropOption not supported")]
		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions, EnterpriseServicesInteropOption interopOption)
		{
			this.Initialize(scopeOption, null, transactionOptions, interopOption, transactionOptions.Timeout, TransactionScopeAsyncFlowOption.Suppress);
		}

		/// <summary>[Supported in the .NET Framework 4.5.1 and later versions]  
		///  Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class and sets the specified transaction as the ambient transaction, so that transactional work done inside the scope uses this transaction.</summary>
		/// <param name="transactionToUse">The transaction to be set as the ambient transaction, so that transactional work done inside the scope uses this transaction.</param>
		/// <param name="asyncFlowOption">An instance of the <see cref="T:System.Transactions.TransactionScopeAsyncFlowOption" /> enumeration that describes whether the ambient transaction associated with the transaction scope will flow across thread continuations when using Task or async/await .NET async programming patterns.</param>
		public TransactionScope(Transaction transactionToUse, TransactionScopeAsyncFlowOption asyncFlowOption)
		{
			throw new NotImplementedException();
		}

		/// <summary>[Supported in the .NET Framework 4.5.1 and later versions]  
		///  Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified timeout value, and sets the specified transaction as the ambient transaction, so that transactional work done inside the scope uses this transaction.</summary>
		/// <param name="transactionToUse">The transaction to be set as the ambient transaction, so that transactional work done inside the scope uses this transaction.</param>
		/// <param name="scopeTimeout">The <see cref="T:System.TimeSpan" /> after which the transaction scope times out and aborts the transaction.</param>
		/// <param name="asyncFlowOption">An instance of the <see cref="T:System.Transactions.TransactionScopeAsyncFlowOption" /> enumeration that describes whether the ambient transaction associated with the transaction scope will flow across thread continuations when using Task or async/await .NET async programming patterns.</param>
		public TransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlowOption)
		{
			throw new NotImplementedException();
		}

		/// <summary>[Supported in the .NET Framework 4.5.1 and later versions]  
		///  Initializes a new instance of the <see cref="T:System.Transactions.TransactionScope" /> class with the specified requirements and asynchronous flow option.</summary>
		/// <param name="scopeOption">An instance of the <see cref="T:System.Transactions.TransactionScopeOption" /> enumeration that describes the transaction requirements associated with this transaction scope.</param>
		/// <param name="transactionOptions">A <see cref="T:System.Transactions.TransactionOptions" /> structure that describes the transaction options to use if a new transaction is created. If an existing transaction is used, the timeout value in this parameter applies to the transaction scope. If that time expires before the scope is disposed, the transaction is aborted.</param>
		/// <param name="asyncFlowOption">An instance of the <see cref="T:System.Transactions.TransactionScopeAsyncFlowOption" /> enumeration that describes whether the ambient transaction associated with the transaction scope will flow across thread continuations when using Task or async/await .NET async programming patterns.</param>
		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions, TransactionScopeAsyncFlowOption asyncFlowOption)
		{
			throw new NotImplementedException();
		}

		private void Initialize(TransactionScopeOption scopeOption, Transaction tx, TransactionOptions options, EnterpriseServicesInteropOption interop, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlow)
		{
			this.completed = false;
			this.isRoot = false;
			this.nested = 0;
			this.asyncFlowEnabled = (asyncFlow == TransactionScopeAsyncFlowOption.Enabled);
			if (scopeTimeout < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("scopeTimeout");
			}
			this.timeout = scopeTimeout;
			this.oldTransaction = Transaction.CurrentInternal;
			Transaction.CurrentInternal = (this.transaction = this.InitTransaction(tx, scopeOption, options));
			if (this.transaction != null)
			{
				this.transaction.InitScope(this);
			}
			if (this.parentScope != null)
			{
				this.parentScope.nested++;
			}
			if (this.timeout != TimeSpan.Zero)
			{
				this.scopeTimer = new Timer(new TimerCallback(TransactionScope.TimerCallback), this, scopeTimeout, TimeSpan.Zero);
			}
		}

		private static void TimerCallback(object state)
		{
			TransactionScope transactionScope = state as TransactionScope;
			if (transactionScope == null)
			{
				throw new TransactionException("TransactionScopeTimerObjectInvalid", null);
			}
			transactionScope.TimeoutScope();
		}

		private void TimeoutScope()
		{
			if (!this.completed && this.transaction != null)
			{
				try
				{
					this.transaction.Rollback();
					this.aborted = true;
				}
				catch (ObjectDisposedException)
				{
				}
				catch (TransactionException)
				{
				}
			}
		}

		private Transaction InitTransaction(Transaction tx, TransactionScopeOption scopeOption, TransactionOptions options)
		{
			if (tx != null)
			{
				return tx;
			}
			if (scopeOption == TransactionScopeOption.Suppress)
			{
				if (Transaction.CurrentInternal != null)
				{
					this.parentScope = Transaction.CurrentInternal.Scope;
				}
				return null;
			}
			if (scopeOption != TransactionScopeOption.Required)
			{
				if (Transaction.CurrentInternal != null)
				{
					this.parentScope = Transaction.CurrentInternal.Scope;
				}
				this.isRoot = true;
				return new Transaction(options.IsolationLevel);
			}
			if (Transaction.CurrentInternal == null)
			{
				this.isRoot = true;
				return new Transaction(options.IsolationLevel);
			}
			this.parentScope = Transaction.CurrentInternal.Scope;
			return Transaction.CurrentInternal;
		}

		/// <summary>Indicates that all operations within the scope are completed successfully.</summary>
		/// <exception cref="T:System.InvalidOperationException">This method has already been called once.</exception>
		public void Complete()
		{
			if (this.completed)
			{
				throw new InvalidOperationException("The current TransactionScope is already complete. You should dispose the TransactionScope.");
			}
			this.completed = true;
		}

		internal bool IsAborted
		{
			get
			{
				return this.aborted;
			}
		}

		internal bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		internal bool IsComplete
		{
			get
			{
				return this.completed;
			}
		}

		internal TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		/// <summary>Ends the transaction scope.</summary>
		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (this.parentScope != null)
			{
				this.parentScope.nested--;
			}
			if (this.nested > 0)
			{
				this.transaction.Rollback();
				throw new InvalidOperationException("TransactionScope nested incorrectly");
			}
			if (Transaction.CurrentInternal != this.transaction && !this.asyncFlowEnabled)
			{
				if (this.transaction != null)
				{
					this.transaction.Rollback();
				}
				if (Transaction.CurrentInternal != null)
				{
					Transaction.CurrentInternal.Rollback();
				}
				throw new InvalidOperationException("Transaction.Current has changed inside of the TransactionScope");
			}
			if (this.scopeTimer != null)
			{
				this.scopeTimer.Dispose();
			}
			if (this.asyncFlowEnabled)
			{
				if (this.oldTransaction != null)
				{
					this.oldTransaction.Scope = this.parentScope;
				}
				Transaction currentInternal = Transaction.CurrentInternal;
				if (this.transaction == null && currentInternal == null)
				{
					return;
				}
				currentInternal.Scope = this.parentScope;
				Transaction.CurrentInternal = this.oldTransaction;
				this.transaction.Scope = null;
				if (this.IsAborted)
				{
					throw new TransactionAbortedException("Transaction has aborted");
				}
				if (!this.IsComplete)
				{
					this.transaction.Rollback();
					currentInternal.Rollback();
					return;
				}
				if (!this.isRoot)
				{
					return;
				}
				currentInternal.CommitInternal();
				this.transaction.CommitInternal();
				return;
			}
			else
			{
				if (Transaction.CurrentInternal == this.oldTransaction && this.oldTransaction != null)
				{
					this.oldTransaction.Scope = this.parentScope;
				}
				Transaction.CurrentInternal = this.oldTransaction;
				if (this.transaction == null)
				{
					return;
				}
				if (this.IsAborted)
				{
					this.transaction.Scope = null;
					throw new TransactionAbortedException("Transaction has aborted");
				}
				if (!this.IsComplete)
				{
					this.transaction.Rollback();
					return;
				}
				if (!this.isRoot)
				{
					return;
				}
				this.transaction.CommitInternal();
				this.transaction.Scope = null;
				return;
			}
		}

		private static TransactionOptions defaultOptions = new TransactionOptions(IsolationLevel.Serializable, TransactionManager.DefaultTimeout);

		private Timer scopeTimer;

		private Transaction transaction;

		private Transaction oldTransaction;

		private TransactionScope parentScope;

		private TimeSpan timeout;

		private int nested;

		private bool disposed;

		private bool completed;

		private bool aborted;

		private bool isRoot;

		private bool asyncFlowEnabled;
	}
}
