using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Common
{
	/// <summary>The base class for a transaction.</summary>
	public abstract class DbTransaction : MarshalByRefObject, IDbTransaction, IDisposable, IAsyncDisposable
	{
		/// <summary>Specifies the <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.</summary>
		/// <returns>The <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.</returns>
		public DbConnection Connection
		{
			get
			{
				return this.DbConnection;
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction, or a null reference if the transaction is no longer valid.</summary>
		/// <returns>The <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.</returns>
		IDbConnection IDbTransaction.Connection
		{
			get
			{
				return this.DbConnection;
			}
		}

		/// <summary>Specifies the <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.</summary>
		/// <returns>The <see cref="T:System.Data.Common.DbConnection" /> object associated with the transaction.</returns>
		protected abstract DbConnection DbConnection { get; }

		/// <summary>Specifies the <see cref="T:System.Data.IsolationLevel" /> for this transaction.</summary>
		/// <returns>The <see cref="T:System.Data.IsolationLevel" /> for this transaction.</returns>
		public abstract IsolationLevel IsolationLevel { get; }

		/// <summary>Commits the database transaction.</summary>
		public abstract void Commit();

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Data.Common.DbTransaction" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Data.Common.DbTransaction" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">If <see langword="true" />, this method releases all resources held by any managed objects that this <see cref="T:System.Data.Common.DbTransaction" /> references.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Rolls back a transaction from a pending state.</summary>
		public abstract void Rollback();

		public virtual Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			Task result;
			try
			{
				this.Commit();
				result = Task.CompletedTask;
			}
			catch (Exception exception)
			{
				result = Task.FromException(exception);
			}
			return result;
		}

		public virtual ValueTask DisposeAsync()
		{
			this.Dispose();
			return default(ValueTask);
		}

		public virtual Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			Task result;
			try
			{
				this.Rollback();
				result = Task.CompletedTask;
			}
			catch (Exception exception)
			{
				result = Task.FromException(exception);
			}
			return result;
		}
	}
}
