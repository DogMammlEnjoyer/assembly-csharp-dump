using System;

namespace System.Data
{
	/// <summary>Provides a means of reading one or more forward-only streams of result sets obtained by executing a command at a data source, and is implemented by .NET Framework data providers that access relational databases.</summary>
	public interface IDataReader : IDisposable, IDataRecord
	{
		/// <summary>Gets a value indicating the depth of nesting for the current row.</summary>
		/// <returns>The level of nesting.</returns>
		int Depth { get; }

		/// <summary>Gets a value indicating whether the data reader is closed.</summary>
		/// <returns>
		///   <see langword="true" /> if the data reader is closed; otherwise, <see langword="false" />.</returns>
		bool IsClosed { get; }

		/// <summary>Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.</summary>
		/// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
		int RecordsAffected { get; }

		/// <summary>Closes the <see cref="T:System.Data.IDataReader" /> Object.</summary>
		void Close();

		/// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.IDataReader" /> is closed.</exception>
		DataTable GetSchemaTable();

		/// <summary>Advances the data reader to the next result, when reading the results of batch SQL statements.</summary>
		/// <returns>
		///   <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
		bool NextResult();

		/// <summary>Advances the <see cref="T:System.Data.IDataReader" /> to the next record.</summary>
		/// <returns>
		///   <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
		bool Read();
	}
}
