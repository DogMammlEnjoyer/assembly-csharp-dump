using System;

namespace System.Data
{
	/// <summary>Represents an open connection to a data source, and is implemented by .NET Framework data providers that access relational databases.</summary>
	public interface IDbConnection : IDisposable
	{
		/// <summary>Gets or sets the string used to open a database.</summary>
		/// <returns>A string containing connection settings.</returns>
		string ConnectionString { get; set; }

		/// <summary>Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.</summary>
		/// <returns>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</returns>
		int ConnectionTimeout { get; }

		/// <summary>Gets the name of the current database or the database to be used after a connection is opened.</summary>
		/// <returns>The name of the current database or the name of the database to be used once a connection is open. The default value is an empty string.</returns>
		string Database { get; }

		/// <summary>Gets the current state of the connection.</summary>
		/// <returns>One of the <see cref="T:System.Data.ConnectionState" /> values.</returns>
		ConnectionState State { get; }

		/// <summary>Begins a database transaction.</summary>
		/// <returns>An object representing the new transaction.</returns>
		IDbTransaction BeginTransaction();

		/// <summary>Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel" /> value.</summary>
		/// <param name="il">One of the <see cref="T:System.Data.IsolationLevel" /> values.</param>
		/// <returns>An object representing the new transaction.</returns>
		IDbTransaction BeginTransaction(IsolationLevel il);

		/// <summary>Closes the connection to the database.</summary>
		void Close();

		/// <summary>Changes the current database for an open <see langword="Connection" /> object.</summary>
		/// <param name="databaseName">The name of the database to use in place of the current database.</param>
		void ChangeDatabase(string databaseName);

		/// <summary>Creates and returns a Command object associated with the connection.</summary>
		/// <returns>A Command object associated with the connection.</returns>
		IDbCommand CreateCommand();

		/// <summary>Opens a database connection with the settings specified by the <see langword="ConnectionString" /> property of the provider-specific Connection object.</summary>
		void Open();
	}
}
