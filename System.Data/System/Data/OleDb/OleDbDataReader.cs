using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb
{
	/// <summary>Provides a way of reading a forward-only stream of data rows from a data source. This class cannot be inherited.</summary>
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbDataReader : DbDataReader
	{
		internal OleDbDataReader()
		{
		}

		/// <summary>Gets a value that indicates the depth of nesting for the current row.</summary>
		/// <returns>The depth of nesting for the current row.</returns>
		public override int Depth
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Gets the number of columns in the current row.</summary>
		/// <returns>When not positioned in a valid recordset, 0; otherwise the number of columns in the current record. The default is -1.</returns>
		/// <exception cref="T:System.NotSupportedException">There is no current connection to a data source.</exception>
		public override int FieldCount
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.OleDb.OleDbDataReader" /> contains one or more rows.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.OleDb.OleDbDataReader" /> contains one or more rows; otherwise <see langword="false" />.</returns>
		public override bool HasRows
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Indicates whether the data reader is closed.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.OleDb.OleDbDataReader" /> is closed; otherwise, <see langword="false" />.</returns>
		public override bool IsClosed
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.</summary>
		/// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
		public override int RecordsAffected
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Gets the number of fields in the <see cref="T:System.Data.OleDb.OleDbDataReader" /> that are not hidden.</summary>
		/// <returns>The number of fields that are not hidden.</returns>
		public override int VisibleFieldCount
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Closes the <see cref="T:System.Data.OleDb.OleDbDataReader" /> object.</summary>
		public override void Close()
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a Boolean.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override bool GetBoolean(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a byte.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a byte.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override byte GetByte(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Reads a stream of bytes from the specified column offset into the buffer as an array starting at the given buffer offset.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <param name="dataIndex">The index within the field from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to read the stream of bytes.</param>
		/// <param name="bufferIndex">The index within the <paramref name="buffer" /> where the write operation is to start.</param>
		/// <param name="length">The maximum length to copy into the buffer.</param>
		/// <returns>The actual number of bytes read.</returns>
		public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a character.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Reads a stream of characters from the specified column offset into the buffer as an array starting at the given buffer offset.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <param name="dataIndex">The index within the row from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to copy data.</param>
		/// <param name="bufferIndex">The index within the <paramref name="buffer" /> where the write operation is to start.</param>
		/// <param name="length">The number of characters to read.</param>
		/// <returns>The actual number of characters read.</returns>
		public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns an <see cref="T:System.Data.OleDb.OleDbDataReader" /> object for the requested column ordinal.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>An <see cref="T:System.Data.OleDb.OleDbDataReader" /> object.</returns>
		public new OleDbDataReader GetData(int ordinal)
		{
			throw ADP.OleDb();
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the name of the source data type.</summary>
		/// <param name="index">The zero-based column ordinal.</param>
		/// <returns>The name of the back-end data type. For more information, see SQL Server data types or Access data types.</returns>
		public override string GetDataTypeName(int index)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override DateTime GetDateTime(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.Decimal" /> object.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override decimal GetDecimal(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a double-precision floating-point number.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override double GetDouble(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
		public override IEnumerator GetEnumerator()
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the <see cref="T:System.Type" /> that is the data type of the object.</summary>
		/// <param name="index">The zero-based column ordinal.</param>
		/// <returns>The <see cref="T:System.Type" /> that is the data type of the object.</returns>
		public override Type GetFieldType(int index)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a single-precision floating-point number.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override float GetFloat(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a globally unique identifier (GUID).</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override Guid GetGuid(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a 16-bit signed integer.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override short GetInt16(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a 32-bit signed integer.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override int GetInt32(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a 64-bit signed integer.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override long GetInt64(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the name of the specified column.</summary>
		/// <param name="index">The zero-based column ordinal.</param>
		/// <returns>The name of the specified column.</returns>
		public override string GetName(int index)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the column ordinal, given the name of the column.</summary>
		/// <param name="name">The name of the column.</param>
		/// <returns>The zero-based column ordinal.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The name specified is not a valid column name.</exception>
		public override int GetOrdinal(string name)
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.OleDb.OleDbDataReader" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.OleDb.OleDbDataReader" /> is closed.</exception>
		public override DataTable GetSchemaTable()
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a string.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override string GetString(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.TimeSpan" /> object.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public TimeSpan GetTimeSpan(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the column at the specified ordinal in its native format.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value to return.</returns>
		public override object GetValue(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Populates an array of objects with the column values of the current row.</summary>
		/// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
		/// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
		public override int GetValues(object[] values)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets a value that indicates whether the column contains nonexistent or missing values.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>
		///   <see langword="true" /> if the specified column value is equivalent to <see cref="T:System.DBNull" />; otherwise, <see langword="false" />.</returns>
		public override bool IsDBNull(int ordinal)
		{
			throw ADP.OleDb();
		}

		/// <summary>Advances the data reader to the next result, when reading the results of batch SQL statements.</summary>
		/// <returns>
		///   <see langword="true" /> if there are more result sets; otherwise, <see langword="false" />.</returns>
		public override bool NextResult()
		{
			throw ADP.OleDb();
		}

		/// <summary>Advances the <see cref="T:System.Data.OleDb.OleDbDataReader" /> to the next record.</summary>
		/// <returns>
		///   <see langword="true" /> if there are more rows; otherwise, <see langword="false" />.</returns>
		public override bool Read()
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets the value of the specified column in its native format given the column ordinal.</summary>
		/// <param name="index">The column ordinal.</param>
		/// <returns>The value of the specified column in its native format.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		public override object this[int index]
		{
			get
			{
				throw ADP.OleDb();
			}
		}

		/// <summary>Gets the value of the specified column in its native format given the column name.</summary>
		/// <param name="name">The column name.</param>
		/// <returns>The value of the specified column in its native format.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public override object this[string name]
		{
			get
			{
				throw ADP.OleDb();
			}
		}
	}
}
