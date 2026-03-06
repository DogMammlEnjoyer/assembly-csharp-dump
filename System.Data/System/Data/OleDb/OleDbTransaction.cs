using System;
using System.Data.Common;

namespace System.Data.OleDb
{
	/// <summary>Represents an SQL transaction to be made at a data source. This class cannot be inherited.</summary>
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbTransaction : DbTransaction
	{
		internal OleDbTransaction()
		{
		}

		/// <summary>Gets the <see cref="T:System.Data.OleDb.OleDbConnection" /> object associated with the transaction, or <see langword="null" /> if the transaction is no longer valid.</summary>
		/// <returns>The <see cref="T:System.Data.OleDb.OleDbConnection" /> object associated with the transaction.</returns>
		public new OleDbConnection Connection
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Specifies the <see cref="T:System.Data.IsolationLevel" /> for this transaction.</summary>
		/// <returns>The <see cref="T:System.Data.IsolationLevel" /> for this transaction. The default is <see langword="ReadCommitted" />.</returns>
		public override IsolationLevel IsolationLevel
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Initiates a nested database transaction.</summary>
		/// <returns>A nested database transaction.</returns>
		/// <exception cref="T:System.InvalidOperationException">Nested transactions are not supported.</exception>
		public OleDbTransaction Begin()
		{
			throw ADP.OleDb();
		}

		/// <summary>Initiates a nested database transaction and specifies the isolation level to use for the new transaction.</summary>
		/// <param name="isolevel">The isolation level to use for the transaction.</param>
		/// <returns>A nested database transaction.</returns>
		/// <exception cref="T:System.InvalidOperationException">Nested transactions are not supported.</exception>
		public OleDbTransaction Begin(IsolationLevel isolevel)
		{
			throw ADP.OleDb();
		}

		/// <summary>Commits the database transaction.</summary>
		/// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction.</exception>
		/// <exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.  
		///  -or-  
		///  The connection is broken.</exception>
		public override void Commit()
		{
			throw ADP.OleDb();
		}

		protected override void Dispose(bool disposing)
		{
			throw ADP.OleDb();
		}

		/// <summary>Rolls back a transaction from a pending state.</summary>
		/// <exception cref="T:System.Exception">An error occurred while trying to commit the transaction.</exception>
		/// <exception cref="T:System.InvalidOperationException">The transaction has already been committed or rolled back.  
		///  -or-  
		///  The connection is broken.</exception>
		public override void Rollback()
		{
			throw ADP.OleDb();
		}
	}
}
