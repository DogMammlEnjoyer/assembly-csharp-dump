using System;
using System.Data.Common;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Represents a Transact-SQL transaction to be made in a SQL Server database. This class cannot be inherited.</summary>
	public sealed class SqlTransaction : DbTransaction
	{
		internal SqlTransaction(SqlInternalConnection internalConnection, SqlConnection con, IsolationLevel iso, SqlInternalTransaction internalTransaction)
		{
			this._isolationLevel = IsolationLevel.ReadCommitted;
			base..ctor();
			this._isolationLevel = iso;
			this._connection = con;
			if (internalTransaction == null)
			{
				this._internalTransaction = new SqlInternalTransaction(internalConnection, TransactionType.LocalFromAPI, this);
				return;
			}
			this._internalTransaction = internalTransaction;
			this._internalTransaction.InitParent(this);
		}

		/// <summary>Gets the <see cref="T:System.Data.SqlClient.SqlConnection" /> object associated with the transaction, or <see langword="null" /> if the transaction is no longer valid.</summary>
		/// <returns>The <see cref="T:System.Data.SqlClient.SqlConnection" /> object associated with the transaction.</returns>
		public new SqlConnection Connection
		{
			get
			{
				if (this.IsZombied)
				{
					return null;
				}
				return this._connection;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
		}

		internal SqlInternalTransaction InternalTransaction
		{
			get
			{
				return this._internalTransaction;
			}
		}

		/// <summary>Specifies the <see cref="T:System.Data.IsolationLevel" /> for this transaction.</summary>
		/// <returns>The <see cref="T:System.Data.IsolationLevel" /> for this transaction. The default is <see langword="ReadCommitted" />.</returns>
		public override IsolationLevel IsolationLevel
		{
			get
			{
				this.ZombieCheck();
				return this._isolationLevel;
			}
		}

		private bool IsYukonPartialZombie
		{
			get
			{
				return this._internalTransaction != null && this._internalTransaction.IsCompleted;
			}
		}

		internal bool IsZombied
		{
			get
			{
				return this._internalTransaction == null || this._internalTransaction.IsCompleted;
			}
		}

		internal SqlStatistics Statistics
		{
			get
			{
				if (this._connection != null && this._connection.StatisticsEnabled)
				{
					return this._connection.Statistics;
				}
				return null;
			}
		}

		/// <summary>Commits the database transaction.</summary>
		/// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction.</exception>
		/// <exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.  
		///  -or-  
		///  The connection is broken.</exception>
		public override void Commit()
		{
			Exception ex = null;
			Guid operationId = SqlTransaction.s_diagnosticListener.WriteTransactionCommitBefore(this._isolationLevel, this._connection, "Commit");
			this.ZombieCheck();
			SqlStatistics statistics = null;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				this._isFromAPI = true;
				this._internalTransaction.Commit();
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				if (ex != null)
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionCommitError(operationId, this._isolationLevel, this._connection, ex, "Commit");
				}
				else
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionCommitAfter(operationId, this._isolationLevel, this._connection, "Commit");
				}
				this._isFromAPI = false;
				SqlStatistics.StopTimer(statistics);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.IsZombied && !this.IsYukonPartialZombie)
			{
				this._internalTransaction.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>Rolls back a transaction from a pending state.</summary>
		/// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction.</exception>
		/// <exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.  
		///  -or-  
		///  The connection is broken.</exception>
		public override void Rollback()
		{
			Exception ex = null;
			Guid operationId = SqlTransaction.s_diagnosticListener.WriteTransactionRollbackBefore(this._isolationLevel, this._connection, null, "Rollback");
			if (this.IsYukonPartialZombie)
			{
				this._internalTransaction = null;
				return;
			}
			this.ZombieCheck();
			SqlStatistics statistics = null;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				this._isFromAPI = true;
				this._internalTransaction.Rollback();
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				if (ex != null)
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackError(operationId, this._isolationLevel, this._connection, null, ex, "Rollback");
				}
				else
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackAfter(operationId, this._isolationLevel, this._connection, null, "Rollback");
				}
				this._isFromAPI = false;
				SqlStatistics.StopTimer(statistics);
			}
		}

		/// <summary>Rolls back a transaction from a pending state, and specifies the transaction or savepoint name.</summary>
		/// <param name="transactionName">The name of the transaction to roll back, or the savepoint to which to roll back.</param>
		/// <exception cref="T:System.ArgumentException">No transaction name was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.  
		///  -or-  
		///  The connection is broken.</exception>
		public void Rollback(string transactionName)
		{
			Exception ex = null;
			Guid operationId = SqlTransaction.s_diagnosticListener.WriteTransactionRollbackBefore(this._isolationLevel, this._connection, transactionName, "Rollback");
			this.ZombieCheck();
			SqlStatistics statistics = null;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				this._isFromAPI = true;
				this._internalTransaction.Rollback(transactionName);
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				if (ex != null)
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackError(operationId, this._isolationLevel, this._connection, transactionName, ex, "Rollback");
				}
				else
				{
					SqlTransaction.s_diagnosticListener.WriteTransactionRollbackAfter(operationId, this._isolationLevel, this._connection, transactionName, "Rollback");
				}
				this._isFromAPI = false;
				SqlStatistics.StopTimer(statistics);
			}
		}

		/// <summary>Creates a savepoint in the transaction that can be used to roll back a part of the transaction, and specifies the savepoint name.</summary>
		/// <param name="savePointName">The name of the savepoint.</param>
		/// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction.</exception>
		/// <exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.  
		///  -or-  
		///  The connection is broken.</exception>
		public void Save(string savePointName)
		{
			this.ZombieCheck();
			SqlStatistics statistics = null;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				this._internalTransaction.Save(savePointName);
			}
			finally
			{
				SqlStatistics.StopTimer(statistics);
			}
		}

		internal void Zombie()
		{
			if (!(this._connection.InnerConnection is SqlInternalConnection) || this._isFromAPI)
			{
				this._internalTransaction = null;
			}
		}

		private void ZombieCheck()
		{
			if (this.IsZombied)
			{
				if (this.IsYukonPartialZombie)
				{
					this._internalTransaction = null;
				}
				throw ADP.TransactionZombied(this);
			}
		}

		internal SqlTransaction()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private static readonly DiagnosticListener s_diagnosticListener = new DiagnosticListener("SqlClientDiagnosticListener");

		internal readonly IsolationLevel _isolationLevel;

		private SqlInternalTransaction _internalTransaction;

		private SqlConnection _connection;

		private bool _isFromAPI;
	}
}
