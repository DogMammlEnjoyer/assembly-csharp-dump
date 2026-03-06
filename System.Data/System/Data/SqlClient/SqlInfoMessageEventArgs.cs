using System;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Provides data for the <see cref="E:System.Data.SqlClient.SqlConnection.InfoMessage" /> event.</summary>
	public sealed class SqlInfoMessageEventArgs : EventArgs
	{
		internal SqlInfoMessageEventArgs(SqlException exception)
		{
			this._exception = exception;
		}

		/// <summary>Gets the collection of warnings sent from the server.</summary>
		/// <returns>The collection of warnings sent from the server.</returns>
		public SqlErrorCollection Errors
		{
			get
			{
				return this._exception.Errors;
			}
		}

		private bool ShouldSerializeErrors()
		{
			return this._exception != null && 0 < this._exception.Errors.Count;
		}

		/// <summary>Gets the full text of the error sent from the database.</summary>
		/// <returns>The full text of the error.</returns>
		public string Message
		{
			get
			{
				return this._exception.Message;
			}
		}

		/// <summary>Gets the name of the object that generated the error.</summary>
		/// <returns>The name of the object that generated the error.</returns>
		public string Source
		{
			get
			{
				return this._exception.Source;
			}
		}

		/// <summary>Retrieves a string representation of the <see cref="E:System.Data.SqlClient.SqlConnection.InfoMessage" /> event.</summary>
		/// <returns>A string representing the <see cref="E:System.Data.SqlClient.SqlConnection.InfoMessage" /> event.</returns>
		public override string ToString()
		{
			return this.Message;
		}

		internal SqlInfoMessageEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private SqlException _exception;
	}
}
