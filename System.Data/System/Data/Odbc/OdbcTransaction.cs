using System;
using System.Data.Common;
using Unity;

namespace System.Data.Odbc
{
	/// <summary>Represents an SQL transaction to be made at a data source. This class cannot be inherited.</summary>
	public sealed class OdbcTransaction : DbTransaction
	{
		internal OdbcTransaction(OdbcConnection connection, IsolationLevel isolevel, OdbcConnectionHandle handle)
		{
			this._isolevel = IsolationLevel.Unspecified;
			base..ctor();
			this._connection = connection;
			this._isolevel = isolevel;
			this._handle = handle;
		}

		/// <summary>Gets the <see cref="T:System.Data.Odbc.OdbcConnection" /> object associated with the transaction, or <see langword="null" /> if the transaction is no longer valid.</summary>
		/// <returns>The <see cref="T:System.Data.Odbc.OdbcConnection" /> object associated with the transaction.</returns>
		public new OdbcConnection Connection
		{
			get
			{
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

		/// <summary>Specifies the <see cref="T:System.Data.IsolationLevel" /> for this transaction.</summary>
		/// <returns>The <see cref="T:System.Data.IsolationLevel" /> for this transaction. The default depends on the underlying ODBC driver.</returns>
		public override IsolationLevel IsolationLevel
		{
			get
			{
				OdbcConnection connection = this._connection;
				if (connection == null)
				{
					throw ADP.TransactionZombied(this);
				}
				if (IsolationLevel.Unspecified == this._isolevel)
				{
					int connectAttr = connection.GetConnectAttr(ODBC32.SQL_ATTR.TXN_ISOLATION, ODBC32.HANDLER.THROW);
					ODBC32.SQL_TRANSACTION sql_TRANSACTION = (ODBC32.SQL_TRANSACTION)connectAttr;
					switch (sql_TRANSACTION)
					{
					case ODBC32.SQL_TRANSACTION.READ_UNCOMMITTED:
						this._isolevel = IsolationLevel.ReadUncommitted;
						goto IL_91;
					case ODBC32.SQL_TRANSACTION.READ_COMMITTED:
						this._isolevel = IsolationLevel.ReadCommitted;
						goto IL_91;
					case (ODBC32.SQL_TRANSACTION)3:
						break;
					case ODBC32.SQL_TRANSACTION.REPEATABLE_READ:
						this._isolevel = IsolationLevel.RepeatableRead;
						goto IL_91;
					default:
						if (sql_TRANSACTION == ODBC32.SQL_TRANSACTION.SERIALIZABLE)
						{
							this._isolevel = IsolationLevel.Serializable;
							goto IL_91;
						}
						if (sql_TRANSACTION == ODBC32.SQL_TRANSACTION.SNAPSHOT)
						{
							this._isolevel = IsolationLevel.Snapshot;
							goto IL_91;
						}
						break;
					}
					throw ODBC.NoMappingForSqlTransactionLevel(connectAttr);
				}
				IL_91:
				return this._isolevel;
			}
		}

		/// <summary>Commits the database transaction.</summary>
		/// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction.</exception>
		/// <exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.  
		///  -or-  
		///  The connection is broken.</exception>
		public override void Commit()
		{
			OdbcConnection connection = this._connection;
			if (connection == null)
			{
				throw ADP.TransactionZombied(this);
			}
			connection.CheckState("CommitTransaction");
			if (this._handle == null)
			{
				throw ODBC.NotInTransaction();
			}
			ODBC32.RetCode retCode = this._handle.CompleteTransaction(0);
			if (retCode == ODBC32.RetCode.ERROR)
			{
				connection.HandleError(this._handle, retCode);
			}
			connection.LocalTransaction = null;
			this._connection = null;
			this._handle = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				OdbcConnectionHandle handle = this._handle;
				this._handle = null;
				if (handle != null)
				{
					try
					{
						ODBC32.RetCode retCode = handle.CompleteTransaction(1);
						if (retCode == ODBC32.RetCode.ERROR && this._connection != null)
						{
							ADP.TraceExceptionWithoutRethrow(this._connection.HandleErrorNoThrow(handle, retCode));
						}
					}
					catch (Exception e)
					{
						if (!ADP.IsCatchableExceptionType(e))
						{
							throw;
						}
					}
				}
				if (this._connection != null && this._connection.IsOpen)
				{
					this._connection.LocalTransaction = null;
				}
				this._connection = null;
				this._isolevel = IsolationLevel.Unspecified;
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
			OdbcConnection connection = this._connection;
			if (connection == null)
			{
				throw ADP.TransactionZombied(this);
			}
			connection.CheckState("RollbackTransaction");
			if (this._handle == null)
			{
				throw ODBC.NotInTransaction();
			}
			ODBC32.RetCode retCode = this._handle.CompleteTransaction(1);
			if (retCode == ODBC32.RetCode.ERROR)
			{
				connection.HandleError(this._handle, retCode);
			}
			connection.LocalTransaction = null;
			this._connection = null;
			this._handle = null;
		}

		internal OdbcTransaction()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private OdbcConnection _connection;

		private IsolationLevel _isolevel;

		private OdbcConnectionHandle _handle;
	}
}
