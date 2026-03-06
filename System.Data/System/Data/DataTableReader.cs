using System;
using System.Collections;
using System.Data.Common;
using System.Globalization;

namespace System.Data
{
	/// <summary>The <see cref="T:System.Data.DataTableReader" /> obtains the contents of one or more <see cref="T:System.Data.DataTable" /> objects in the form of one or more read-only, forward-only result sets.</summary>
	public sealed class DataTableReader : DbDataReader
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataTableReader" /> class by using data from the supplied <see cref="T:System.Data.DataTable" />.</summary>
		/// <param name="dataTable">The <see cref="T:System.Data.DataTable" /> from which the new <see cref="T:System.Data.DataTableReader" /> obtains its result set.</param>
		public DataTableReader(DataTable dataTable)
		{
			if (dataTable == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTable");
			}
			this._tables = new DataTable[]
			{
				dataTable
			};
			this.Init();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataTableReader" /> class using the supplied array of <see cref="T:System.Data.DataTable" /> objects.</summary>
		/// <param name="dataTables">The array of <see cref="T:System.Data.DataTable" /> objects that supplies the results for the new <see cref="T:System.Data.DataTableReader" /> object.</param>
		public DataTableReader(DataTable[] dataTables)
		{
			if (dataTables == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTable");
			}
			if (dataTables.Length == 0)
			{
				throw ExceptionBuilder.DataTableReaderArgumentIsEmpty();
			}
			this._tables = new DataTable[dataTables.Length];
			for (int i = 0; i < dataTables.Length; i++)
			{
				if (dataTables[i] == null)
				{
					throw ExceptionBuilder.ArgumentNull("DataTable");
				}
				this._tables[i] = dataTables[i];
			}
			this.Init();
		}

		private bool ReaderIsInvalid
		{
			get
			{
				return this._readerIsInvalid;
			}
			set
			{
				if (this._readerIsInvalid == value)
				{
					return;
				}
				this._readerIsInvalid = value;
				if (this._readerIsInvalid && this._listener != null)
				{
					this._listener.CleanUp();
				}
			}
		}

		private bool IsSchemaChanged
		{
			get
			{
				return this._schemaIsChanged;
			}
			set
			{
				if (!value || this._schemaIsChanged == value)
				{
					return;
				}
				this._schemaIsChanged = value;
				if (this._listener != null)
				{
					this._listener.CleanUp();
				}
			}
		}

		internal DataTable CurrentDataTable
		{
			get
			{
				return this._currentDataTable;
			}
		}

		private void Init()
		{
			this._tableCounter = 0;
			this._reachEORows = false;
			this._schemaIsChanged = false;
			this._currentDataTable = this._tables[this._tableCounter];
			this._hasRows = (this._currentDataTable.Rows.Count > 0);
			this.ReaderIsInvalid = false;
			this._listener = new DataTableReaderListener(this);
		}

		/// <summary>Closes the current <see cref="T:System.Data.DataTableReader" />.</summary>
		public override void Close()
		{
			if (!this._isOpen)
			{
				return;
			}
			if (this._listener != null)
			{
				this._listener.CleanUp();
			}
			this._listener = null;
			this._schemaTable = null;
			this._isOpen = false;
		}

		/// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.DataTableReader" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.DataTableReader" /> is closed.</exception>
		public override DataTable GetSchemaTable()
		{
			this.ValidateOpen("GetSchemaTable");
			this.ValidateReader();
			if (this._schemaTable == null)
			{
				this._schemaTable = DataTableReader.GetSchemaTableFromDataTable(this._currentDataTable);
			}
			return this._schemaTable;
		}

		/// <summary>Advances the <see cref="T:System.Data.DataTableReader" /> to the next result set, if any.</summary>
		/// <returns>
		///   <see langword="true" /> if there was another result set; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to navigate within a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override bool NextResult()
		{
			this.ValidateOpen("NextResult");
			if (this._tableCounter == this._tables.Length - 1)
			{
				return false;
			}
			DataTable[] tables = this._tables;
			int num = this._tableCounter + 1;
			this._tableCounter = num;
			this._currentDataTable = tables[num];
			if (this._listener != null)
			{
				this._listener.UpdataTable(this._currentDataTable);
			}
			this._schemaTable = null;
			this._rowCounter = -1;
			this._currentRowRemoved = false;
			this._reachEORows = false;
			this._schemaIsChanged = false;
			this._started = false;
			this.ReaderIsInvalid = false;
			this._tableCleared = false;
			this._hasRows = (this._currentDataTable.Rows.Count > 0);
			return true;
		}

		/// <summary>Advances the <see cref="T:System.Data.DataTableReader" /> to the next record.</summary>
		/// <returns>
		///   <see langword="true" /> if there was another row to read; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" /> .</exception>
		public override bool Read()
		{
			if (!this._started)
			{
				this._started = true;
			}
			this.ValidateOpen("Read");
			this.ValidateReader();
			if (this._reachEORows)
			{
				return false;
			}
			if (this._rowCounter >= this._currentDataTable.Rows.Count - 1)
			{
				this._reachEORows = true;
				if (this._listener != null)
				{
					this._listener.CleanUp();
				}
				return false;
			}
			this._rowCounter++;
			this.ValidateRow(this._rowCounter);
			this._currentDataRow = this._currentDataTable.Rows[this._rowCounter];
			while (this._currentDataRow.RowState == DataRowState.Deleted)
			{
				this._rowCounter++;
				if (this._rowCounter == this._currentDataTable.Rows.Count)
				{
					this._reachEORows = true;
					if (this._listener != null)
					{
						this._listener.CleanUp();
					}
					return false;
				}
				this.ValidateRow(this._rowCounter);
				this._currentDataRow = this._currentDataTable.Rows[this._rowCounter];
			}
			if (this._currentRowRemoved)
			{
				this._currentRowRemoved = false;
			}
			return true;
		}

		/// <summary>The depth of nesting for the current row of the <see cref="T:System.Data.DataTableReader" />.</summary>
		/// <returns>The depth of nesting for the current row; always zero.</returns>
		public override int Depth
		{
			get
			{
				this.ValidateOpen("Depth");
				this.ValidateReader();
				return 0;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.DataTableReader" /> is closed.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.DataTableReader" /> is closed; otherwise, <see langword="false" />.</returns>
		public override bool IsClosed
		{
			get
			{
				return !this._isOpen;
			}
		}

		/// <summary>Gets the number of rows inserted, changed, or deleted by execution of the SQL statement.</summary>
		/// <returns>The <see cref="T:System.Data.DataTableReader" /> does not support this property and always returns 0.</returns>
		public override int RecordsAffected
		{
			get
			{
				this.ValidateReader();
				return 0;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.DataTableReader" /> contains one or more rows.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.DataTableReader" /> contains one or more rows; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to retrieve information about a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override bool HasRows
		{
			get
			{
				this.ValidateOpen("HasRows");
				this.ValidateReader();
				return this._hasRows;
			}
		}

		/// <summary>Gets the value of the specified column in its native format given the column ordinal.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column in its native format.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		public override object this[int ordinal]
		{
			get
			{
				this.ValidateOpen("Item");
				this.ValidateReader();
				if (this._currentDataRow == null || this._currentDataRow.RowState == DataRowState.Deleted)
				{
					this.ReaderIsInvalid = true;
					throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
				}
				object result;
				try
				{
					result = this._currentDataRow[ordinal];
				}
				catch (IndexOutOfRangeException e)
				{
					ExceptionBuilder.TraceExceptionWithoutRethrow(e);
					throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
				}
				return result;
			}
		}

		/// <summary>Gets the value of the specified column in its native format given the column name.</summary>
		/// <param name="name">The name of the column.</param>
		/// <returns>The value of the specified column in its native format.</returns>
		/// <exception cref="T:System.ArgumentException">The name specified is not a valid column name.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override object this[string name]
		{
			get
			{
				this.ValidateOpen("Item");
				this.ValidateReader();
				if (this._currentDataRow == null || this._currentDataRow.RowState == DataRowState.Deleted)
				{
					this.ReaderIsInvalid = true;
					throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
				}
				return this._currentDataRow[name];
			}
		}

		/// <summary>Returns the number of columns in the current row.</summary>
		/// <returns>When not positioned in a valid result set, 0; otherwise the number of columns in the current row.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to retrieve the field count in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override int FieldCount
		{
			get
			{
				this.ValidateOpen("FieldCount");
				this.ValidateReader();
				return this._currentDataTable.Columns.Count;
			}
		}

		/// <summary>Gets the type of the specified column in provider-specific format.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The <see cref="T:System.Type" /> that is the data type of the object.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override Type GetProviderSpecificFieldType(int ordinal)
		{
			this.ValidateOpen("GetProviderSpecificFieldType");
			this.ValidateReader();
			return this.GetFieldType(ordinal);
		}

		/// <summary>Gets the value of the specified column in provider-specific format.</summary>
		/// <param name="ordinal">The zero-based number of the column whose value is retrieved.</param>
		/// <returns>The value of the specified column in provider-specific format.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" /></exception>
		public override object GetProviderSpecificValue(int ordinal)
		{
			this.ValidateOpen("GetProviderSpecificValue");
			this.ValidateReader();
			return this.GetValue(ordinal);
		}

		/// <summary>Fills the supplied array with provider-specific type information for all the columns in the <see cref="T:System.Data.DataTableReader" />.</summary>
		/// <param name="values">An array of objects to be filled in with type information for the columns in the <see cref="T:System.Data.DataTableReader" />.</param>
		/// <returns>The number of column values copied into the array.</returns>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override int GetProviderSpecificValues(object[] values)
		{
			this.ValidateOpen("GetProviderSpecificValues");
			this.ValidateReader();
			return this.GetValues(values);
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.Boolean" />.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a <see langword="Boolean" />.</exception>
		public override bool GetBoolean(int ordinal)
		{
			this.ValidateState("GetBoolean");
			this.ValidateReader();
			bool result;
			try
			{
				result = (bool)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column as a byte.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see langword="DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a byte.</exception>
		public override byte GetByte(int ordinal)
		{
			this.ValidateState("GetByte");
			this.ValidateReader();
			byte result;
			try
			{
				result = (byte)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Reads a stream of bytes starting at the specified column offset into the buffer as an array starting at the specified buffer offset.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <param name="dataIndex">The index within the field from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to read the stream of bytes.</param>
		/// <param name="bufferIndex">The index within the buffer at which to start placing the data.</param>
		/// <param name="length">The maximum length to copy into the buffer.</param>
		/// <returns>The actual number of bytes read.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see langword="DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a byte array.</exception>
		public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			this.ValidateState("GetBytes");
			this.ValidateReader();
			byte[] array;
			try
			{
				array = (byte[])this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			if (buffer == null)
			{
				return (long)array.Length;
			}
			int num = (int)dataIndex;
			int num2 = Math.Min(array.Length - num, length);
			if (num < 0)
			{
				throw ADP.InvalidSourceBufferIndex(array.Length, (long)num, "dataIndex");
			}
			if (bufferIndex < 0 || (bufferIndex > 0 && bufferIndex >= buffer.Length))
			{
				throw ADP.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
			}
			if (0 < num2)
			{
				Array.Copy(array, dataIndex, buffer, (long)bufferIndex, (long)num2);
			}
			else
			{
				if (length < 0)
				{
					throw ADP.InvalidDataLength((long)length);
				}
				num2 = 0;
			}
			return (long)num2;
		}

		/// <summary>Gets the value of the specified column as a character.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see langword="DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified field does not contain a character.</exception>
		public override char GetChar(int ordinal)
		{
			this.ValidateState("GetChar");
			this.ValidateReader();
			char result;
			try
			{
				result = (char)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Returns the value of the specified column as a character array.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <param name="dataIndex">The index within the field from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to read the stream of chars.</param>
		/// <param name="bufferIndex">The index within the buffer at which to start placing the data.</param>
		/// <param name="length">The maximum length to copy into the buffer.</param>
		/// <returns>The actual number of characters read.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see langword="DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a character array.</exception>
		public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			this.ValidateState("GetChars");
			this.ValidateReader();
			char[] array;
			try
			{
				array = (char[])this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			if (buffer == null)
			{
				return (long)array.Length;
			}
			int num = (int)dataIndex;
			int num2 = Math.Min(array.Length - num, length);
			if (num < 0)
			{
				throw ADP.InvalidSourceBufferIndex(array.Length, (long)num, "dataIndex");
			}
			if (bufferIndex < 0 || (bufferIndex > 0 && bufferIndex >= buffer.Length))
			{
				throw ADP.InvalidDestinationBufferIndex(buffer.Length, bufferIndex, "bufferIndex");
			}
			if (0 < num2)
			{
				Array.Copy(array, dataIndex, buffer, (long)bufferIndex, (long)num2);
			}
			else
			{
				if (length < 0)
				{
					throw ADP.InvalidDataLength((long)length);
				}
				num2 = 0;
			}
			return (long)num2;
		}

		/// <summary>Gets a string representing the data type of the specified column.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>A string representing the column's data type.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override string GetDataTypeName(int ordinal)
		{
			this.ValidateOpen("GetDataTypeName");
			this.ValidateReader();
			return this.GetFieldType(ordinal).Name;
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see langword="DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a DateTime value.</exception>
		public override DateTime GetDateTime(int ordinal)
		{
			this.ValidateState("GetDateTime");
			this.ValidateReader();
			DateTime result;
			try
			{
				result = (DateTime)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.Decimal" />.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see langword="DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a <see langword="Decimal" /> value.</exception>
		public override decimal GetDecimal(int ordinal)
		{
			this.ValidateState("GetDecimal");
			this.ValidateReader();
			decimal result;
			try
			{
				result = (decimal)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the column as a double-precision floating point number.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see langword="DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a double-precision floating point number.</exception>
		public override double GetDouble(int ordinal)
		{
			this.ValidateState("GetDouble");
			this.ValidateReader();
			double result;
			try
			{
				result = (double)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the <see cref="T:System.Type" /> that is the data type of the object.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The <see cref="T:System.Type" /> that is the data type of the object.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" /> .</exception>
		public override Type GetFieldType(int ordinal)
		{
			this.ValidateOpen("GetFieldType");
			this.ValidateReader();
			Type dataType;
			try
			{
				dataType = this._currentDataTable.Columns[ordinal].DataType;
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return dataType;
		}

		/// <summary>Gets the value of the specified column as a single-precision floating point number.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a single-precision floating point number.</exception>
		public override float GetFloat(int ordinal)
		{
			this.ValidateState("GetFloat");
			this.ValidateReader();
			float result;
			try
			{
				result = (float)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column as a globally-unique identifier (GUID).</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a GUID.</exception>
		public override Guid GetGuid(int ordinal)
		{
			this.ValidateState("GetGuid");
			this.ValidateReader();
			Guid result;
			try
			{
				result = (Guid)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column as a 16-bit signed integer.</summary>
		/// <param name="ordinal">The zero-based column ordinal</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a 16-bit signed integer.</exception>
		public override short GetInt16(int ordinal)
		{
			this.ValidateState("GetInt16");
			this.ValidateReader();
			short result;
			try
			{
				result = (short)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column as a 32-bit signed integer.</summary>
		/// <param name="ordinal">The zero-based column ordinal</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" /> .</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a 32-bit signed integer value.</exception>
		public override int GetInt32(int ordinal)
		{
			this.ValidateState("GetInt32");
			this.ValidateReader();
			int result;
			try
			{
				result = (int)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column as a 64-bit signed integer.</summary>
		/// <param name="ordinal">The zero-based column ordinal</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" /> .</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a 64-bit signed integer value.</exception>
		public override long GetInt64(int ordinal)
		{
			this.ValidateState("GetInt64");
			this.ValidateReader();
			long result;
			try
			{
				result = (long)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.String" />.</summary>
		/// <param name="ordinal">The zero-based column ordinal</param>
		/// <returns>The name of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override string GetName(int ordinal)
		{
			this.ValidateOpen("GetName");
			this.ValidateReader();
			string columnName;
			try
			{
				columnName = this._currentDataTable.Columns[ordinal].ColumnName;
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return columnName;
		}

		/// <summary>Gets the column ordinal, given the name of the column.</summary>
		/// <param name="name">The name of the column.</param>
		/// <returns>The zero-based column ordinal.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		/// <exception cref="T:System.ArgumentException">The name specified is not a valid column name.</exception>
		public override int GetOrdinal(string name)
		{
			this.ValidateOpen("GetOrdinal");
			this.ValidateReader();
			DataColumn dataColumn = this._currentDataTable.Columns[name];
			if (dataColumn != null)
			{
				return dataColumn.Ordinal;
			}
			throw ExceptionBuilder.ColumnNotInTheTable(name, this._currentDataTable.TableName);
		}

		/// <summary>Gets the value of the specified column as a string.</summary>
		/// <param name="ordinal">The zero-based column ordinal</param>
		/// <returns>The value of the specified column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The specified column does not contain a string.</exception>
		public override string GetString(int ordinal)
		{
			this.ValidateState("GetString");
			this.ValidateReader();
			string result;
			try
			{
				result = (string)this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Gets the value of the specified column in its native format.</summary>
		/// <param name="ordinal">The zero-based column ordinal</param>
		/// <returns>The value of the specified column. This method returns <see langword="DBNull" /> for null columns.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access columns in a closed <see cref="T:System.Data.DataTableReader" /> .</exception>
		public override object GetValue(int ordinal)
		{
			this.ValidateState("GetValue");
			this.ValidateReader();
			object result;
			try
			{
				result = this._currentDataRow[ordinal];
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Populates an array of objects with the column values of the current row.</summary>
		/// <param name="values">An array of <see cref="T:System.Object" /> into which to copy the column values from the <see cref="T:System.Data.DataTableReader" />.</param>
		/// <returns>The number of column values copied into the array.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" /> .</exception>
		public override int GetValues(object[] values)
		{
			this.ValidateState("GetValues");
			this.ValidateReader();
			if (values == null)
			{
				throw ExceptionBuilder.ArgumentNull("values");
			}
			Array.Copy(this._currentDataRow.ItemArray, values, (this._currentDataRow.ItemArray.Length > values.Length) ? values.Length : this._currentDataRow.ItemArray.Length);
			if (this._currentDataRow.ItemArray.Length <= values.Length)
			{
				return this._currentDataRow.ItemArray.Length;
			}
			return values.Length;
		}

		/// <summary>Gets a value that indicates whether the column contains non-existent or missing values.</summary>
		/// <param name="ordinal">The zero-based column ordinal</param>
		/// <returns>
		///   <see langword="true" /> if the specified column value is equivalent to <see cref="T:System.DBNull" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to <see cref="P:System.Data.DataTableReader.FieldCount" /> - 1.</exception>
		/// <exception cref="T:System.Data.DeletedRowInaccessibleException">An attempt was made to retrieve data from a deleted row.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" /> .</exception>
		public override bool IsDBNull(int ordinal)
		{
			this.ValidateState("IsDBNull");
			this.ValidateReader();
			bool result;
			try
			{
				result = this._currentDataRow.IsNull(ordinal);
			}
			catch (IndexOutOfRangeException e)
			{
				ExceptionBuilder.TraceExceptionWithoutRethrow(e);
				throw ExceptionBuilder.ArgumentOutOfRange("ordinal");
			}
			return result;
		}

		/// <summary>Returns an enumerator that can be used to iterate through the item collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that represents the item collection.</returns>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to read or access a column in a closed <see cref="T:System.Data.DataTableReader" />.</exception>
		public override IEnumerator GetEnumerator()
		{
			this.ValidateOpen("GetEnumerator");
			return new DbEnumerator(this);
		}

		internal static DataTable GetSchemaTableFromDataTable(DataTable table)
		{
			if (table == null)
			{
				throw ExceptionBuilder.ArgumentNull("DataTable");
			}
			DataTable dataTable = new DataTable("SchemaTable");
			dataTable.Locale = CultureInfo.InvariantCulture;
			DataColumn column = new DataColumn(SchemaTableColumn.ColumnName, typeof(string));
			DataColumn column2 = new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int));
			DataColumn dataColumn = new DataColumn(SchemaTableColumn.ColumnSize, typeof(int));
			DataColumn column3 = new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short));
			DataColumn column4 = new DataColumn(SchemaTableColumn.NumericScale, typeof(short));
			DataColumn column5 = new DataColumn(SchemaTableColumn.DataType, typeof(Type));
			DataColumn column6 = new DataColumn(SchemaTableColumn.ProviderType, typeof(int));
			DataColumn dataColumn2 = new DataColumn(SchemaTableColumn.IsLong, typeof(bool));
			DataColumn column7 = new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool));
			DataColumn dataColumn3 = new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
			DataColumn dataColumn4 = new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
			DataColumn column8 = new DataColumn(SchemaTableColumn.IsUnique, typeof(bool));
			DataColumn dataColumn5 = new DataColumn(SchemaTableColumn.IsKey, typeof(bool));
			DataColumn dataColumn6 = new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
			DataColumn column9 = new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string));
			DataColumn dataColumn7 = new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
			DataColumn dataColumn8 = new DataColumn(SchemaTableColumn.BaseTableName, typeof(string));
			DataColumn column10 = new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string));
			DataColumn dataColumn9 = new DataColumn(SchemaTableOptionalColumn.AutoIncrementSeed, typeof(long));
			DataColumn dataColumn10 = new DataColumn(SchemaTableOptionalColumn.AutoIncrementStep, typeof(long));
			DataColumn column11 = new DataColumn(SchemaTableOptionalColumn.DefaultValue, typeof(object));
			DataColumn column12 = new DataColumn(SchemaTableOptionalColumn.Expression, typeof(string));
			DataColumn column13 = new DataColumn(SchemaTableOptionalColumn.ColumnMapping, typeof(MappingType));
			DataColumn dataColumn11 = new DataColumn(SchemaTableOptionalColumn.BaseTableNamespace, typeof(string));
			DataColumn column14 = new DataColumn(SchemaTableOptionalColumn.BaseColumnNamespace, typeof(string));
			dataColumn.DefaultValue = -1;
			if (table.DataSet != null)
			{
				dataColumn7.DefaultValue = table.DataSet.DataSetName;
			}
			dataColumn8.DefaultValue = table.TableName;
			dataColumn11.DefaultValue = table.Namespace;
			dataColumn4.DefaultValue = false;
			dataColumn2.DefaultValue = false;
			dataColumn3.DefaultValue = false;
			dataColumn5.DefaultValue = false;
			dataColumn6.DefaultValue = false;
			dataColumn9.DefaultValue = 0;
			dataColumn10.DefaultValue = 1;
			dataTable.Columns.Add(column);
			dataTable.Columns.Add(column2);
			dataTable.Columns.Add(dataColumn);
			dataTable.Columns.Add(column3);
			dataTable.Columns.Add(column4);
			dataTable.Columns.Add(column5);
			dataTable.Columns.Add(column6);
			dataTable.Columns.Add(dataColumn2);
			dataTable.Columns.Add(column7);
			dataTable.Columns.Add(dataColumn3);
			dataTable.Columns.Add(dataColumn4);
			dataTable.Columns.Add(column8);
			dataTable.Columns.Add(dataColumn5);
			dataTable.Columns.Add(dataColumn6);
			dataTable.Columns.Add(dataColumn7);
			dataTable.Columns.Add(column9);
			dataTable.Columns.Add(dataColumn8);
			dataTable.Columns.Add(column10);
			dataTable.Columns.Add(dataColumn9);
			dataTable.Columns.Add(dataColumn10);
			dataTable.Columns.Add(column11);
			dataTable.Columns.Add(column12);
			dataTable.Columns.Add(column13);
			dataTable.Columns.Add(dataColumn11);
			dataTable.Columns.Add(column14);
			foreach (object obj in table.Columns)
			{
				DataColumn dataColumn12 = (DataColumn)obj;
				DataRow dataRow = dataTable.NewRow();
				dataRow[column] = dataColumn12.ColumnName;
				dataRow[column2] = dataColumn12.Ordinal;
				dataRow[column5] = dataColumn12.DataType;
				if (dataColumn12.DataType == typeof(string))
				{
					dataRow[dataColumn] = dataColumn12.MaxLength;
				}
				dataRow[column7] = dataColumn12.AllowDBNull;
				dataRow[dataColumn3] = dataColumn12.ReadOnly;
				dataRow[column8] = dataColumn12.Unique;
				if (dataColumn12.AutoIncrement)
				{
					dataRow[dataColumn6] = true;
					dataRow[dataColumn9] = dataColumn12.AutoIncrementSeed;
					dataRow[dataColumn10] = dataColumn12.AutoIncrementStep;
				}
				if (dataColumn12.DefaultValue != DBNull.Value)
				{
					dataRow[column11] = dataColumn12.DefaultValue;
				}
				if (dataColumn12.Expression.Length != 0)
				{
					bool flag = false;
					DataColumn[] dependency = dataColumn12.DataExpression.GetDependency();
					for (int i = 0; i < dependency.Length; i++)
					{
						if (dependency[i].Table != table)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						dataRow[column12] = dataColumn12.Expression;
					}
				}
				dataRow[column13] = dataColumn12.ColumnMapping;
				dataRow[column10] = dataColumn12.ColumnName;
				dataRow[column14] = dataColumn12.Namespace;
				dataTable.Rows.Add(dataRow);
			}
			foreach (DataColumn dataColumn13 in table.PrimaryKey)
			{
				dataTable.Rows[dataColumn13.Ordinal][dataColumn5] = true;
			}
			dataTable.AcceptChanges();
			return dataTable;
		}

		private void ValidateOpen(string caller)
		{
			if (!this._isOpen)
			{
				throw ADP.DataReaderClosed(caller);
			}
		}

		private void ValidateReader()
		{
			if (this.ReaderIsInvalid)
			{
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
			if (this.IsSchemaChanged)
			{
				throw ExceptionBuilder.DataTableReaderSchemaIsInvalid(this._currentDataTable.TableName);
			}
		}

		private void ValidateState(string caller)
		{
			this.ValidateOpen(caller);
			if (this._tableCleared)
			{
				throw ExceptionBuilder.EmptyDataTableReader(this._currentDataTable.TableName);
			}
			if (this._currentDataRow == null || this._currentDataTable == null)
			{
				this.ReaderIsInvalid = true;
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
			if (this._currentDataRow.RowState == DataRowState.Deleted || this._currentDataRow.RowState == DataRowState.Detached || this._currentRowRemoved)
			{
				throw ExceptionBuilder.InvalidCurrentRowInDataTableReader();
			}
			if (0 > this._rowCounter || this._currentDataTable.Rows.Count <= this._rowCounter)
			{
				this.ReaderIsInvalid = true;
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
		}

		private void ValidateRow(int rowPosition)
		{
			if (this.ReaderIsInvalid)
			{
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
			if (0 > rowPosition || this._currentDataTable.Rows.Count <= rowPosition)
			{
				this.ReaderIsInvalid = true;
				throw ExceptionBuilder.InvalidDataTableReader(this._currentDataTable.TableName);
			}
		}

		internal void SchemaChanged()
		{
			this.IsSchemaChanged = true;
		}

		internal void DataTableCleared()
		{
			if (!this._started)
			{
				return;
			}
			this._rowCounter = -1;
			if (!this._reachEORows)
			{
				this._currentRowRemoved = true;
			}
		}

		internal void DataChanged(DataRowChangeEventArgs args)
		{
			if (!this._started || (this._rowCounter == -1 && !this._tableCleared))
			{
				return;
			}
			DataRowAction action = args.Action;
			if (action <= DataRowAction.Rollback)
			{
				if (action != DataRowAction.Delete && action != DataRowAction.Rollback)
				{
					return;
				}
			}
			else if (action != DataRowAction.Commit)
			{
				if (action != DataRowAction.Add)
				{
					return;
				}
				this.ValidateRow(this._rowCounter + 1);
				if (this._currentDataRow == this._currentDataTable.Rows[this._rowCounter + 1])
				{
					this._rowCounter++;
					return;
				}
				return;
			}
			if (args.Row.RowState == DataRowState.Detached)
			{
				if (args.Row != this._currentDataRow)
				{
					if (this._rowCounter != 0)
					{
						this.ValidateRow(this._rowCounter - 1);
						if (this._currentDataRow == this._currentDataTable.Rows[this._rowCounter - 1])
						{
							this._rowCounter--;
							return;
						}
					}
				}
				else
				{
					this._currentRowRemoved = true;
					if (this._rowCounter > 0)
					{
						this._rowCounter--;
						this._currentDataRow = this._currentDataTable.Rows[this._rowCounter];
						return;
					}
					this._rowCounter = -1;
					this._currentDataRow = null;
				}
			}
		}

		private readonly DataTable[] _tables;

		private bool _isOpen = true;

		private DataTable _schemaTable;

		private int _tableCounter = -1;

		private int _rowCounter = -1;

		private DataTable _currentDataTable;

		private DataRow _currentDataRow;

		private bool _hasRows = true;

		private bool _reachEORows;

		private bool _currentRowRemoved;

		private bool _schemaIsChanged;

		private bool _started;

		private bool _readerIsInvalid;

		private DataTableReaderListener _listener;

		private bool _tableCleared;
	}
}
