using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Unity;

namespace System.Data.Odbc
{
	/// <summary>Provides a way of reading a forward-only stream of data rows from a data source. This class cannot be inherited.</summary>
	public sealed class OdbcDataReader : DbDataReader
	{
		internal OdbcDataReader(OdbcCommand command, CMDWrapper cmdWrapper, CommandBehavior commandbehavior)
		{
			this._recordAffected = -1;
			this._row = -1;
			this._column = -1;
			this.ObjectID = Interlocked.Increment(ref OdbcDataReader.s_objectTypeCount);
			base..ctor();
			this._command = command;
			this._commandBehavior = commandbehavior;
			this._cmdText = command.CommandText;
			this._cmdWrapper = cmdWrapper;
		}

		private CNativeBuffer Buffer
		{
			get
			{
				CNativeBuffer dataReaderBuf = this._cmdWrapper._dataReaderBuf;
				if (dataReaderBuf == null)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				return dataReaderBuf;
			}
		}

		private OdbcConnection Connection
		{
			get
			{
				if (this._cmdWrapper != null)
				{
					return this._cmdWrapper.Connection;
				}
				return null;
			}
		}

		internal OdbcCommand Command
		{
			get
			{
				return this._command;
			}
			set
			{
				this._command = value;
			}
		}

		private OdbcStatementHandle StatementHandle
		{
			get
			{
				return this._cmdWrapper.StatementHandle;
			}
		}

		private OdbcStatementHandle KeyInfoStatementHandle
		{
			get
			{
				return this._cmdWrapper.KeyInfoStatement;
			}
		}

		internal bool IsBehavior(CommandBehavior behavior)
		{
			return this.IsCommandBehavior(behavior);
		}

		internal bool IsCancelingCommand
		{
			get
			{
				return this._command != null && this._command.Canceling;
			}
		}

		internal bool IsNonCancelingCommand
		{
			get
			{
				return this._command != null && !this._command.Canceling;
			}
		}

		/// <summary>Gets a value that indicates the depth of nesting for the current row.</summary>
		/// <returns>The depth of nesting for the current row.</returns>
		public override int Depth
		{
			get
			{
				if (this.IsClosed)
				{
					throw ADP.DataReaderClosed("Depth");
				}
				return 0;
			}
		}

		/// <summary>Gets the number of columns in the current row.</summary>
		/// <returns>When not positioned in a valid record set, 0; otherwise the number of columns in the current record. The default is -1.</returns>
		/// <exception cref="T:System.NotSupportedException">There is no current connection to a data source.</exception>
		public override int FieldCount
		{
			get
			{
				if (this.IsClosed)
				{
					throw ADP.DataReaderClosed("FieldCount");
				}
				if (this._noMoreResults)
				{
					return 0;
				}
				if (this._dataCache == null)
				{
					short num;
					ODBC32.RetCode retCode = this.FieldCountNoThrow(out num);
					if (retCode != ODBC32.RetCode.SUCCESS)
					{
						this.Connection.HandleError(this.StatementHandle, retCode);
					}
				}
				if (this._dataCache == null)
				{
					return 0;
				}
				return this._dataCache._count;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.Odbc.OdbcDataReader" /> contains one or more rows.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Odbc.OdbcDataReader" /> contains one or more rows; otherwise <see langword="false" />.</returns>
		public override bool HasRows
		{
			get
			{
				if (this.IsClosed)
				{
					throw ADP.DataReaderClosed("HasRows");
				}
				if (this._hasRows == OdbcDataReader.HasRowsStatus.DontKnow)
				{
					this.Read();
					this._skipReadOnce = true;
				}
				return this._hasRows == OdbcDataReader.HasRowsStatus.HasRows;
			}
		}

		internal ODBC32.RetCode FieldCountNoThrow(out short cColsAffected)
		{
			if (this.IsCancelingCommand)
			{
				cColsAffected = 0;
				return ODBC32.RetCode.ERROR;
			}
			ODBC32.RetCode retCode = this.StatementHandle.NumberOfResultColumns(out cColsAffected);
			if (retCode == ODBC32.RetCode.SUCCESS)
			{
				this._hiddenColumns = 0;
				if (this.IsCommandBehavior(CommandBehavior.KeyInfo) && !this.Connection.ProviderInfo.NoSqlSoptSSNoBrowseTable && !this.Connection.ProviderInfo.NoSqlSoptSSHiddenColumns)
				{
					for (int i = 0; i < (int)cColsAffected; i++)
					{
						if (this.GetColAttribute(i, (ODBC32.SQL_DESC)1211, (ODBC32.SQL_COLUMN)(-1), ODBC32.HANDLER.IGNORE).ToInt64() == 1L)
						{
							this._hiddenColumns = (int)cColsAffected - i;
							cColsAffected = (short)i;
							break;
						}
					}
				}
				this._dataCache = new DbCache(this, (int)cColsAffected);
			}
			else
			{
				cColsAffected = 0;
			}
			return retCode;
		}

		/// <summary>Indicates whether the <see cref="T:System.Data.Odbc.OdbcDataReader" /> is closed.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Odbc.OdbcDataReader" /> is closed; otherwise <see langword="false" />.</returns>
		public override bool IsClosed
		{
			get
			{
				return this._isClosed;
			}
		}

		private SQLLEN GetRowCount()
		{
			if (!this.IsClosed)
			{
				SQLLEN result;
				ODBC32.RetCode retCode = this.StatementHandle.RowCount(out result);
				if (retCode == ODBC32.RetCode.SUCCESS || ODBC32.RetCode.SUCCESS_WITH_INFO == retCode)
				{
					return result;
				}
			}
			return -1;
		}

		internal int CalculateRecordsAffected(int cRowsAffected)
		{
			if (0 <= cRowsAffected)
			{
				if (-1 == this._recordAffected)
				{
					this._recordAffected = cRowsAffected;
				}
				else
				{
					this._recordAffected += cRowsAffected;
				}
			}
			return this._recordAffected;
		}

		/// <summary>Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.</summary>
		/// <returns>The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0 if no rows were affected, or the statement failed.</returns>
		public override int RecordsAffected
		{
			get
			{
				return this._recordAffected;
			}
		}

		/// <summary>Gets the value of the specified column in its native format given the column ordinal.</summary>
		/// <param name="i">The column ordinal.</param>
		/// <returns>The value of the specified column in its native format.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		public override object this[int i]
		{
			get
			{
				return this.GetValue(i);
			}
		}

		/// <summary>Gets the value of the specified column in its native format given the column name.</summary>
		/// <param name="value">The column name.</param>
		/// <returns>The value of the specified column in its native format.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public override object this[string value]
		{
			get
			{
				return this.GetValue(this.GetOrdinal(value));
			}
		}

		/// <summary>Closes the <see cref="T:System.Data.Odbc.OdbcDataReader" /> object.</summary>
		public override void Close()
		{
			this.Close(false);
		}

		private void Close(bool disposing)
		{
			Exception ex = null;
			CMDWrapper cmdWrapper = this._cmdWrapper;
			if (cmdWrapper != null && cmdWrapper.StatementHandle != null)
			{
				if (this.IsNonCancelingCommand)
				{
					this.NextResult(disposing, !disposing);
					if (this._command != null)
					{
						if (this._command.HasParameters)
						{
							this._command.Parameters.GetOutputValues(this._cmdWrapper);
						}
						cmdWrapper.FreeStatementHandle(ODBC32.STMT.CLOSE);
						this._command.CloseFromDataReader();
					}
				}
				cmdWrapper.FreeKeyInfoStatementHandle(ODBC32.STMT.CLOSE);
			}
			if (this._command != null)
			{
				this._command.CloseFromDataReader();
				if (this.IsCommandBehavior(CommandBehavior.CloseConnection))
				{
					this._command.Parameters.RebindCollection = true;
					this.Connection.Close();
				}
			}
			else if (cmdWrapper != null)
			{
				cmdWrapper.Dispose();
			}
			this._command = null;
			this._isClosed = true;
			this._dataCache = null;
			this._metadata = null;
			this._schemaTable = null;
			this._isRead = false;
			this._hasRows = OdbcDataReader.HasRowsStatus.DontKnow;
			this._isValidResult = false;
			this._noMoreResults = true;
			this._noMoreRows = true;
			this._fieldNameLookup = null;
			this.SetCurrentRowColumnInfo(-1, 0);
			if (ex != null && !disposing)
			{
				throw ex;
			}
			this._cmdWrapper = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close(true);
			}
		}

		/// <summary>Gets the name of the source data type.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The name of the source data type.</returns>
		public override string GetDataTypeName(int i)
		{
			if (this._dataCache != null)
			{
				DbSchemaInfo schema = this._dataCache.GetSchema(i);
				if (schema._typename == null)
				{
					schema._typename = this.GetColAttributeStr(i, ODBC32.SQL_DESC.TYPE_NAME, ODBC32.SQL_COLUMN.TYPE_NAME, ODBC32.HANDLER.THROW);
				}
				return schema._typename;
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the rows in the data reader.</returns>
		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this, this.IsCommandBehavior(CommandBehavior.CloseConnection));
		}

		/// <summary>Gets the <see cref="T:System.Type" /> that is the data type of the object.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The <see cref="T:System.Type" /> that is the data type of the object.</returns>
		public override Type GetFieldType(int i)
		{
			if (this._dataCache != null)
			{
				DbSchemaInfo schema = this._dataCache.GetSchema(i);
				if (schema._type == null)
				{
					schema._type = this.GetSqlType(i)._type;
				}
				return schema._type;
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the name of the specified column.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>A string that is the name of the specified column.</returns>
		public override string GetName(int i)
		{
			if (this._dataCache != null)
			{
				DbSchemaInfo schema = this._dataCache.GetSchema(i);
				if (schema._name == null)
				{
					schema._name = this.GetColAttributeStr(i, ODBC32.SQL_DESC.NAME, ODBC32.SQL_COLUMN.NAME, ODBC32.HANDLER.THROW);
					if (schema._name == null)
					{
						schema._name = "";
					}
				}
				return schema._name;
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the column ordinal, given the name of the column.</summary>
		/// <param name="value">The name of the column.</param>
		/// <returns>The zero-based column ordinal.</returns>
		public override int GetOrdinal(string value)
		{
			if (this._fieldNameLookup == null)
			{
				if (this._dataCache == null)
				{
					throw ADP.DataReaderNoData();
				}
				this._fieldNameLookup = new FieldNameLookup(this, -1);
			}
			return this._fieldNameLookup.GetOrdinal(value);
		}

		private int IndexOf(string value)
		{
			if (this._fieldNameLookup == null)
			{
				if (this._dataCache == null)
				{
					throw ADP.DataReaderNoData();
				}
				this._fieldNameLookup = new FieldNameLookup(this, -1);
			}
			return this._fieldNameLookup.IndexOf(value);
		}

		private bool IsCommandBehavior(CommandBehavior condition)
		{
			return condition == (condition & this._commandBehavior);
		}

		internal object GetValue(int i, TypeMap typemap)
		{
			ODBC32.SQL_TYPE sql_type = typemap._sql_type;
			if (sql_type != ODBC32.SQL_TYPE.SS_VARIANT)
			{
				switch (sql_type)
				{
				case ODBC32.SQL_TYPE.GUID:
					return this.internalGetGuid(i);
				case ODBC32.SQL_TYPE.WLONGVARCHAR:
				case ODBC32.SQL_TYPE.WVARCHAR:
				case ODBC32.SQL_TYPE.WCHAR:
				case ODBC32.SQL_TYPE.LONGVARCHAR:
				case ODBC32.SQL_TYPE.CHAR:
				case ODBC32.SQL_TYPE.VARCHAR:
					return this.internalGetString(i);
				case ODBC32.SQL_TYPE.BIT:
					return this.internalGetBoolean(i);
				case ODBC32.SQL_TYPE.TINYINT:
					return this.internalGetByte(i);
				case ODBC32.SQL_TYPE.BIGINT:
					return this.internalGetInt64(i);
				case ODBC32.SQL_TYPE.LONGVARBINARY:
				case ODBC32.SQL_TYPE.VARBINARY:
				case ODBC32.SQL_TYPE.BINARY:
					return this.internalGetBytes(i);
				case (ODBC32.SQL_TYPE)0:
				case (ODBC32.SQL_TYPE)9:
				case (ODBC32.SQL_TYPE)10:
				case ODBC32.SQL_TYPE.TIMESTAMP:
					break;
				case ODBC32.SQL_TYPE.NUMERIC:
				case ODBC32.SQL_TYPE.DECIMAL:
					return this.internalGetDecimal(i);
				case ODBC32.SQL_TYPE.INTEGER:
					return this.internalGetInt32(i);
				case ODBC32.SQL_TYPE.SMALLINT:
					return this.internalGetInt16(i);
				case ODBC32.SQL_TYPE.FLOAT:
				case ODBC32.SQL_TYPE.DOUBLE:
					return this.internalGetDouble(i);
				case ODBC32.SQL_TYPE.REAL:
					return this.internalGetFloat(i);
				default:
					switch (sql_type)
					{
					case ODBC32.SQL_TYPE.TYPE_DATE:
						return this.internalGetDate(i);
					case ODBC32.SQL_TYPE.TYPE_TIME:
						return this.internalGetTime(i);
					case ODBC32.SQL_TYPE.TYPE_TIMESTAMP:
						return this.internalGetDateTime(i);
					}
					break;
				}
				return this.internalGetBytes(i);
			}
			if (!this._isRead)
			{
				throw ADP.DataReaderNoData();
			}
			int num;
			if (this._dataCache.AccessIndex(i) == null && this.QueryFieldInfo(i, ODBC32.SQL_C.BINARY, out num))
			{
				ODBC32.SQL_TYPE sqltype = (ODBC32.SQL_TYPE)this.GetColAttribute(i, (ODBC32.SQL_DESC)1216, (ODBC32.SQL_COLUMN)(-1), ODBC32.HANDLER.THROW);
				return this.GetValue(i, TypeMap.FromSqlType(sqltype));
			}
			return this._dataCache[i];
		}

		/// <summary>Gets the value of the column at the specified ordinal in its native format.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value to return.</returns>
		public override object GetValue(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null)
				{
					this._dataCache[i] = this.GetValue(i, this.GetSqlType(i));
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Populates an array of objects with the column values of the current row.</summary>
		/// <param name="values">An array of type <see cref="T:System.Object" /> into which to copy the attribute columns.</param>
		/// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
		public override int GetValues(object[] values)
		{
			if (this._isRead)
			{
				int num = Math.Min(values.Length, this.FieldCount);
				for (int i = 0; i < num; i++)
				{
					values[i] = this.GetValue(i);
				}
				return num;
			}
			throw ADP.DataReaderNoData();
		}

		private TypeMap GetSqlType(int i)
		{
			DbSchemaInfo schema = this._dataCache.GetSchema(i);
			TypeMap typeMap;
			if (schema._dbtype == null)
			{
				schema._dbtype = new ODBC32.SQL_TYPE?((ODBC32.SQL_TYPE)this.GetColAttribute(i, ODBC32.SQL_DESC.CONCISE_TYPE, ODBC32.SQL_COLUMN.TYPE, ODBC32.HANDLER.THROW));
				typeMap = TypeMap.FromSqlType(schema._dbtype.Value);
				if (typeMap._signType)
				{
					bool unsigned = this.GetColAttribute(i, ODBC32.SQL_DESC.UNSIGNED, ODBC32.SQL_COLUMN.UNSIGNED, ODBC32.HANDLER.THROW).ToInt64() != 0L;
					typeMap = TypeMap.UpgradeSignedType(typeMap, unsigned);
					schema._dbtype = new ODBC32.SQL_TYPE?(typeMap._sql_type);
				}
			}
			else
			{
				typeMap = TypeMap.FromSqlType(schema._dbtype.Value);
			}
			this.Connection.SetSupportedType(schema._dbtype.Value);
			return typeMap;
		}

		/// <summary>Gets a value that indicates whether the column contains nonexistent or missing values.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>
		///   <see langword="true" /> if the specified column value is equivalent to <see cref="T:System.DBNull" />; otherwise <see langword="false" />.</returns>
		public override bool IsDBNull(int i)
		{
			if (!this.IsCommandBehavior(CommandBehavior.SequentialAccess))
			{
				return Convert.IsDBNull(this.GetValue(i));
			}
			object obj = this._dataCache[i];
			if (obj != null)
			{
				return Convert.IsDBNull(obj);
			}
			TypeMap sqlType = this.GetSqlType(i);
			if (sqlType._bufferSize > 0)
			{
				return Convert.IsDBNull(this.GetValue(i));
			}
			int num;
			return !this.QueryFieldInfo(i, sqlType._sql_c, out num);
		}

		/// <summary>Gets the value of the specified column as a byte.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a byte.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override byte GetByte(int i)
		{
			return (byte)this.internalGetByte(i);
		}

		private object internalGetByte(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.UTINYINT))
				{
					this._dataCache[i] = this.Buffer.ReadByte(0);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a character.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a character.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override char GetChar(int i)
		{
			return (char)this.internalGetChar(i);
		}

		private object internalGetChar(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.WCHAR))
				{
					this._dataCache[i] = this.Buffer.ReadChar(0);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a 16-bit signed integer.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a 16-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override short GetInt16(int i)
		{
			return (short)this.internalGetInt16(i);
		}

		private object internalGetInt16(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.SSHORT))
				{
					this._dataCache[i] = this.Buffer.ReadInt16(0);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a 32-bit signed integer.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a 32-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override int GetInt32(int i)
		{
			return (int)this.internalGetInt32(i);
		}

		private object internalGetInt32(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.SLONG))
				{
					this._dataCache[i] = this.Buffer.ReadInt32(0);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a 64-bit signed integer.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a 64-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override long GetInt64(int i)
		{
			return (long)this.internalGetInt64(i);
		}

		private object internalGetInt64(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.WCHAR))
				{
					string s = (string)this.Buffer.MarshalToManaged(0, ODBC32.SQL_C.WCHAR, -3);
					this._dataCache[i] = long.Parse(s, CultureInfo.InvariantCulture);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a Boolean.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>A Boolean that is the value of the column.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override bool GetBoolean(int i)
		{
			return (bool)this.internalGetBoolean(i);
		}

		private object internalGetBoolean(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.BIT))
				{
					this._dataCache[i] = this.Buffer.MarshalToManaged(0, ODBC32.SQL_C.BIT, -1);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a single-precision floating-point number.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a single-precision floating-point number.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override float GetFloat(int i)
		{
			return (float)this.internalGetFloat(i);
		}

		private object internalGetFloat(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.REAL))
				{
					this._dataCache[i] = this.Buffer.ReadSingle(0);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a <see cref="T:System.DateTime" /> object.</returns>
		public DateTime GetDate(int i)
		{
			return (DateTime)this.internalGetDate(i);
		}

		private object internalGetDate(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.TYPE_DATE))
				{
					this._dataCache[i] = this.Buffer.MarshalToManaged(0, ODBC32.SQL_C.TYPE_DATE, -1);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.DateTime" /> object.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a <see cref="T:System.DateTime" /> object.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override DateTime GetDateTime(int i)
		{
			return (DateTime)this.internalGetDateTime(i);
		}

		private object internalGetDateTime(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.TYPE_TIMESTAMP))
				{
					this._dataCache[i] = this.Buffer.MarshalToManaged(0, ODBC32.SQL_C.TYPE_TIMESTAMP, -1);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.Decimal" /> object.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a <see cref="T:System.Decimal" /> object.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override decimal GetDecimal(int i)
		{
			return (decimal)this.internalGetDecimal(i);
		}

		private object internalGetDecimal(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.WCHAR))
				{
					string text = null;
					try
					{
						text = (string)this.Buffer.MarshalToManaged(0, ODBC32.SQL_C.WCHAR, -3);
						this._dataCache[i] = decimal.Parse(text, CultureInfo.InvariantCulture);
					}
					catch (OverflowException ex)
					{
						this._dataCache[i] = text;
						throw ex;
					}
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a double-precision floating-point number.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a double-precision floating-point number.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override double GetDouble(int i)
		{
			return (double)this.internalGetDouble(i);
		}

		private object internalGetDouble(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.DOUBLE))
				{
					this._dataCache[i] = this.Buffer.ReadDouble(0);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a globally unique identifier (GUID).</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a GUID.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override Guid GetGuid(int i)
		{
			return (Guid)this.internalGetGuid(i);
		}

		private object internalGetGuid(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.GUID))
				{
					this._dataCache[i] = this.Buffer.ReadGuid(0);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.String" />.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a <see cref="T:System.String" />.</returns>
		/// <exception cref="T:System.InvalidCastException">The specified cast is not valid.</exception>
		public override string GetString(int i)
		{
			return (string)this.internalGetString(i);
		}

		private object internalGetString(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null)
				{
					CNativeBuffer buffer = this.Buffer;
					int num = buffer.Length - 4;
					int num2;
					if (this.GetData(i, ODBC32.SQL_C.WCHAR, buffer.Length - 2, out num2))
					{
						if (num2 <= num && -4 != num2)
						{
							string text = buffer.PtrToStringUni(0, Math.Min(num2, num) / 2);
							this._dataCache[i] = text;
							return text;
						}
						char[] array = new char[num / 2];
						StringBuilder stringBuilder = new StringBuilder(((num2 == -4) ? num : num2) / 2);
						int num3 = num;
						int num4 = (-4 == num2) ? -1 : (num2 - num3);
						bool data;
						do
						{
							int num5 = num3 / 2;
							buffer.ReadChars(0, array, 0, num5);
							stringBuilder.Append(array, 0, num5);
							if (num4 == 0)
							{
								break;
							}
							data = this.GetData(i, ODBC32.SQL_C.WCHAR, buffer.Length - 2, out num2);
							if (-4 != num2)
							{
								num3 = Math.Min(num2, num);
								if (0 < num4)
								{
									num4 -= num3;
								}
								else
								{
									num4 = 0;
								}
							}
						}
						while (data);
						this._dataCache[i] = stringBuilder.ToString();
					}
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		/// <summary>Gets the value of the specified column as a <see cref="T:System.TimeSpan" /> object.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a <see cref="T:System.TimeSpan" /> object.</returns>
		public TimeSpan GetTime(int i)
		{
			return (TimeSpan)this.internalGetTime(i);
		}

		private object internalGetTime(int i)
		{
			if (this._isRead)
			{
				if (this._dataCache.AccessIndex(i) == null && this.GetData(i, ODBC32.SQL_C.TYPE_TIME))
				{
					this._dataCache[i] = this.Buffer.MarshalToManaged(0, ODBC32.SQL_C.TYPE_TIME, -1);
				}
				return this._dataCache[i];
			}
			throw ADP.DataReaderNoData();
		}

		private void SetCurrentRowColumnInfo(int row, int column)
		{
			if (this._row != row || this._column != column)
			{
				this._row = row;
				this._column = column;
				this._sequentialBytesRead = 0L;
			}
		}

		/// <summary>Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the particular buffer offset.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <param name="dataIndex">The index within the field where the read operation is to start.</param>
		/// <param name="buffer">The buffer into which to read the stream of bytes.</param>
		/// <param name="bufferIndex">The index within the <paramref name="buffer" /> where the write operation is to start.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <returns>The actual number of bytes read.</returns>
		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			return this.GetBytesOrChars(i, dataIndex, buffer, false, bufferIndex, length);
		}

		/// <summary>Reads a stream of characters from the specified column offset into the buffer as an array, starting at the particular buffer offset.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <param name="dataIndex">The index within the row where the read operation is to start.</param>
		/// <param name="buffer">The buffer into which to copy data.</param>
		/// <param name="bufferIndex">The index within the <paramref name="buffer" /> where the write operation is to start.</param>
		/// <param name="length">The number of characters to read.</param>
		/// <returns>The actual number of characters read.</returns>
		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			return this.GetBytesOrChars(i, dataIndex, buffer, true, bufferIndex, length);
		}

		private long GetBytesOrChars(int i, long dataIndex, Array buffer, bool isCharsBuffer, int bufferIndex, int length)
		{
			if (this.IsClosed)
			{
				throw ADP.DataReaderNoData();
			}
			if (!this._isRead)
			{
				throw ADP.DataReaderNoData();
			}
			if (dataIndex < 0L)
			{
				throw ADP.ArgumentOutOfRange("dataIndex");
			}
			if (bufferIndex < 0)
			{
				throw ADP.ArgumentOutOfRange("bufferIndex");
			}
			if (length < 0)
			{
				throw ADP.ArgumentOutOfRange("length");
			}
			string method = isCharsBuffer ? "GetChars" : "GetBytes";
			this.SetCurrentRowColumnInfo(this._row, i);
			object obj;
			if (isCharsBuffer)
			{
				obj = (string)this._dataCache[i];
			}
			else
			{
				obj = (byte[])this._dataCache[i];
			}
			if (!this.IsCommandBehavior(CommandBehavior.SequentialAccess) || obj != null)
			{
				if (2147483647L < dataIndex)
				{
					throw ADP.ArgumentOutOfRange("dataIndex");
				}
				if (obj == null)
				{
					if (isCharsBuffer)
					{
						obj = (string)this.internalGetString(i);
					}
					else
					{
						obj = (byte[])this.internalGetBytes(i);
					}
				}
				int num = isCharsBuffer ? ((string)obj).Length : ((byte[])obj).Length;
				if (buffer == null)
				{
					return (long)num;
				}
				if (length == 0)
				{
					return 0L;
				}
				if (dataIndex >= (long)num)
				{
					return 0L;
				}
				int num2 = Math.Min(num - (int)dataIndex, length);
				num2 = Math.Min(num2, buffer.Length - bufferIndex);
				if (num2 <= 0)
				{
					return 0L;
				}
				if (isCharsBuffer)
				{
					((string)obj).CopyTo((int)dataIndex, (char[])buffer, bufferIndex, num2);
				}
				else
				{
					Array.Copy((byte[])obj, (int)dataIndex, (byte[])buffer, bufferIndex, num2);
				}
				return (long)num2;
			}
			else if (buffer == null)
			{
				ODBC32.SQL_C sqlctype = isCharsBuffer ? ODBC32.SQL_C.WCHAR : ODBC32.SQL_C.BINARY;
				int num3;
				if (!this.QueryFieldInfo(i, sqlctype, out num3))
				{
					if (isCharsBuffer)
					{
						throw ADP.InvalidCast();
					}
					return -1L;
				}
				else
				{
					if (isCharsBuffer)
					{
						return (long)(num3 / 2);
					}
					return (long)num3;
				}
			}
			else
			{
				if ((isCharsBuffer && dataIndex < this._sequentialBytesRead / 2L) || (!isCharsBuffer && dataIndex < this._sequentialBytesRead))
				{
					throw ADP.NonSeqByteAccess(dataIndex, this._sequentialBytesRead, method);
				}
				if (isCharsBuffer)
				{
					dataIndex -= this._sequentialBytesRead / 2L;
				}
				else
				{
					dataIndex -= this._sequentialBytesRead;
				}
				if (dataIndex > 0L && (long)this.readBytesOrCharsSequentialAccess(i, null, isCharsBuffer, 0, dataIndex) < dataIndex)
				{
					return 0L;
				}
				length = Math.Min(length, buffer.Length - bufferIndex);
				if (length > 0)
				{
					return (long)this.readBytesOrCharsSequentialAccess(i, buffer, isCharsBuffer, bufferIndex, (long)length);
				}
				int num4;
				if (isCharsBuffer && !this.QueryFieldInfo(i, ODBC32.SQL_C.WCHAR, out num4))
				{
					throw ADP.InvalidCast();
				}
				return 0L;
			}
		}

		private int readBytesOrCharsSequentialAccess(int i, Array buffer, bool isCharsBuffer, int bufferIndex, long bytesOrCharsLength)
		{
			int num = 0;
			long num2 = isCharsBuffer ? checked(bytesOrCharsLength * 2L) : bytesOrCharsLength;
			CNativeBuffer buffer2 = this.Buffer;
			while (num2 > 0L)
			{
				int num3;
				int num4;
				bool data;
				if (isCharsBuffer)
				{
					num3 = (int)Math.Min(num2, (long)(buffer2.Length - 4));
					data = this.GetData(i, ODBC32.SQL_C.WCHAR, num3 + 2, out num4);
				}
				else
				{
					num3 = (int)Math.Min(num2, (long)(buffer2.Length - 2));
					data = this.GetData(i, ODBC32.SQL_C.BINARY, num3, out num4);
				}
				if (!data)
				{
					throw ADP.InvalidCast();
				}
				bool flag = false;
				if (num4 == 0)
				{
					break;
				}
				int num5;
				if (-4 == num4)
				{
					num5 = num3;
				}
				else if (num4 > num3)
				{
					num5 = num3;
				}
				else
				{
					num5 = num4;
					flag = true;
				}
				this._sequentialBytesRead += (long)num5;
				if (isCharsBuffer)
				{
					int num6 = num5 / 2;
					if (buffer != null)
					{
						buffer2.ReadChars(0, (char[])buffer, bufferIndex, num6);
						bufferIndex += num6;
					}
					num += num6;
				}
				else
				{
					if (buffer != null)
					{
						buffer2.ReadBytes(0, (byte[])buffer, bufferIndex, num5);
						bufferIndex += num5;
					}
					num += num5;
				}
				num2 -= (long)num5;
				if (flag)
				{
					break;
				}
			}
			return num;
		}

		private object internalGetBytes(int i)
		{
			if (this._dataCache.AccessIndex(i) == null)
			{
				int num = this.Buffer.Length - 4;
				int num2 = 0;
				int j;
				if (this.GetData(i, ODBC32.SQL_C.BINARY, num, out j))
				{
					CNativeBuffer buffer = this.Buffer;
					byte[] array;
					if (-4 != j)
					{
						array = new byte[j];
						this.Buffer.ReadBytes(0, array, num2, Math.Min(j, num));
						while (j > num)
						{
							this.GetData(i, ODBC32.SQL_C.BINARY, num, out j);
							num2 += num;
							buffer.ReadBytes(0, array, num2, Math.Min(j, num));
						}
					}
					else
					{
						List<byte[]> list = new List<byte[]>();
						int num3 = 0;
						do
						{
							int num4 = (-4 != j) ? j : num;
							array = new byte[num4];
							num3 += num4;
							buffer.ReadBytes(0, array, 0, num4);
							list.Add(array);
						}
						while (-4 == j && this.GetData(i, ODBC32.SQL_C.BINARY, num, out j));
						array = new byte[num3];
						foreach (byte[] array2 in list)
						{
							array2.CopyTo(array, num2);
							num2 += array2.Length;
						}
					}
					this._dataCache[i] = array;
				}
			}
			return this._dataCache[i];
		}

		private SQLLEN GetColAttribute(int iColumn, ODBC32.SQL_DESC v3FieldId, ODBC32.SQL_COLUMN v2FieldId, ODBC32.HANDLER handler)
		{
			short num = 0;
			if (this.Connection == null || this._cmdWrapper.Canceling)
			{
				return -1;
			}
			OdbcStatementHandle statementHandle = this.StatementHandle;
			SQLLEN result;
			ODBC32.RetCode retCode;
			if (this.Connection.IsV3Driver)
			{
				retCode = statementHandle.ColumnAttribute(iColumn + 1, (short)v3FieldId, this.Buffer, out num, out result);
			}
			else
			{
				if (v2FieldId == (ODBC32.SQL_COLUMN)(-1))
				{
					return 0;
				}
				retCode = statementHandle.ColumnAttribute(iColumn + 1, (short)v2FieldId, this.Buffer, out num, out result);
			}
			if (retCode != ODBC32.RetCode.SUCCESS)
			{
				if (retCode == ODBC32.RetCode.ERROR && "HY091" == this.Command.GetDiagSqlState())
				{
					this.Connection.FlagUnsupportedColAttr(v3FieldId, v2FieldId);
				}
				if (handler == ODBC32.HANDLER.THROW)
				{
					this.Connection.HandleError(statementHandle, retCode);
				}
				return -1;
			}
			return result;
		}

		private string GetColAttributeStr(int i, ODBC32.SQL_DESC v3FieldId, ODBC32.SQL_COLUMN v2FieldId, ODBC32.HANDLER handler)
		{
			short num = 0;
			CNativeBuffer buffer = this.Buffer;
			buffer.WriteInt16(0, 0);
			OdbcStatementHandle statementHandle = this.StatementHandle;
			if (this.Connection == null || this._cmdWrapper.Canceling || statementHandle == null)
			{
				return "";
			}
			ODBC32.RetCode retCode;
			if (this.Connection.IsV3Driver)
			{
				SQLLEN sqllen;
				retCode = statementHandle.ColumnAttribute(i + 1, (short)v3FieldId, buffer, out num, out sqllen);
			}
			else
			{
				if (v2FieldId == (ODBC32.SQL_COLUMN)(-1))
				{
					return null;
				}
				SQLLEN sqllen;
				retCode = statementHandle.ColumnAttribute(i + 1, (short)v2FieldId, buffer, out num, out sqllen);
			}
			if (retCode != ODBC32.RetCode.SUCCESS || num == 0)
			{
				if (retCode == ODBC32.RetCode.ERROR && "HY091" == this.Command.GetDiagSqlState())
				{
					this.Connection.FlagUnsupportedColAttr(v3FieldId, v2FieldId);
				}
				if (handler == ODBC32.HANDLER.THROW)
				{
					this.Connection.HandleError(statementHandle, retCode);
				}
				return null;
			}
			return buffer.PtrToStringUni(0, (int)(num / 2));
		}

		private string GetDescFieldStr(int i, ODBC32.SQL_DESC attribute, ODBC32.HANDLER handler)
		{
			int num = 0;
			if (this.Connection == null || this._cmdWrapper.Canceling)
			{
				return "";
			}
			if (!this.Connection.IsV3Driver)
			{
				return null;
			}
			CNativeBuffer buffer = this.Buffer;
			using (OdbcDescriptorHandle odbcDescriptorHandle = new OdbcDescriptorHandle(this.StatementHandle, ODBC32.SQL_ATTR.APP_PARAM_DESC))
			{
				ODBC32.RetCode descriptionField = odbcDescriptorHandle.GetDescriptionField(i + 1, attribute, buffer, out num);
				if (descriptionField != ODBC32.RetCode.SUCCESS || num == 0)
				{
					if (descriptionField == ODBC32.RetCode.ERROR && "HY091" == this.Command.GetDiagSqlState())
					{
						this.Connection.FlagUnsupportedColAttr(attribute, ODBC32.SQL_COLUMN.COUNT);
					}
					if (handler == ODBC32.HANDLER.THROW)
					{
						this.Connection.HandleError(this.StatementHandle, descriptionField);
					}
					return null;
				}
			}
			return buffer.PtrToStringUni(0, num / 2);
		}

		private bool QueryFieldInfo(int i, ODBC32.SQL_C sqlctype, out int cbLengthOrIndicator)
		{
			int cb = 0;
			if (sqlctype == ODBC32.SQL_C.WCHAR)
			{
				cb = 2;
			}
			return this.GetData(i, sqlctype, cb, out cbLengthOrIndicator);
		}

		private bool GetData(int i, ODBC32.SQL_C sqlctype)
		{
			int num;
			return this.GetData(i, sqlctype, this.Buffer.Length - 4, out num);
		}

		private bool GetData(int i, ODBC32.SQL_C sqlctype, int cb, out int cbLengthOrIndicator)
		{
			IntPtr intPtr = IntPtr.Zero;
			if (this.IsCancelingCommand)
			{
				throw ADP.DataReaderNoData();
			}
			CNativeBuffer buffer = this.Buffer;
			ODBC32.RetCode data = this.StatementHandle.GetData(i + 1, sqlctype, buffer, cb, out intPtr);
			if (data != ODBC32.RetCode.SUCCESS)
			{
				if (data != ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					if (data != ODBC32.RetCode.NO_DATA)
					{
						this.Connection.HandleError(this.StatementHandle, data);
					}
					else
					{
						if (sqlctype != ODBC32.SQL_C.WCHAR && sqlctype != ODBC32.SQL_C.BINARY)
						{
							this.Connection.HandleError(this.StatementHandle, data);
						}
						if (intPtr == (IntPtr)(-4))
						{
							intPtr = (IntPtr)0;
						}
					}
				}
				else if ((int)intPtr == -4)
				{
				}
			}
			this.SetCurrentRowColumnInfo(this._row, i);
			if (intPtr == (IntPtr)(-1))
			{
				this._dataCache[i] = DBNull.Value;
				cbLengthOrIndicator = 0;
				return false;
			}
			cbLengthOrIndicator = (int)intPtr;
			return true;
		}

		/// <summary>Advances the <see cref="T:System.Data.Odbc.OdbcDataReader" /> to the next record.</summary>
		/// <returns>
		///   <see langword="true" /> if there are more rows; otherwise <see langword="false" />.</returns>
		public override bool Read()
		{
			if (this.IsClosed)
			{
				throw ADP.DataReaderClosed("Read");
			}
			if (this.IsCancelingCommand)
			{
				this._isRead = false;
				return false;
			}
			if (this._skipReadOnce)
			{
				this._skipReadOnce = false;
				return this._isRead;
			}
			if (this._noMoreRows || this._noMoreResults || this.IsCommandBehavior(CommandBehavior.SchemaOnly))
			{
				return false;
			}
			if (!this._isValidResult)
			{
				return false;
			}
			ODBC32.RetCode retCode = this.StatementHandle.Fetch();
			if (retCode != ODBC32.RetCode.SUCCESS)
			{
				if (retCode != ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					if (retCode != ODBC32.RetCode.NO_DATA)
					{
						this.Connection.HandleError(this.StatementHandle, retCode);
					}
					else
					{
						this._isRead = false;
						if (this._hasRows == OdbcDataReader.HasRowsStatus.DontKnow)
						{
							this._hasRows = OdbcDataReader.HasRowsStatus.HasNoRows;
						}
					}
				}
				else
				{
					this.Connection.HandleErrorNoThrow(this.StatementHandle, retCode);
					this._hasRows = OdbcDataReader.HasRowsStatus.HasRows;
					this._isRead = true;
				}
			}
			else
			{
				this._hasRows = OdbcDataReader.HasRowsStatus.HasRows;
				this._isRead = true;
			}
			this._dataCache.FlushValues();
			if (this.IsCommandBehavior(CommandBehavior.SingleRow))
			{
				this._noMoreRows = true;
				this.SetCurrentRowColumnInfo(-1, 0);
			}
			else
			{
				this.SetCurrentRowColumnInfo(this._row + 1, 0);
			}
			return this._isRead;
		}

		internal void FirstResult()
		{
			SQLLEN rowCount = this.GetRowCount();
			this.CalculateRecordsAffected(rowCount);
			short num;
			if (this.FieldCountNoThrow(out num) == ODBC32.RetCode.SUCCESS && num == 0)
			{
				this.NextResult();
				return;
			}
			this._isValidResult = true;
		}

		/// <summary>Advances the <see cref="T:System.Data.Odbc.OdbcDataReader" /> to the next result when reading the results of batch SQL statements.</summary>
		/// <returns>
		///   <see langword="true" /> if there are more result sets; otherwise <see langword="false" />.</returns>
		public override bool NextResult()
		{
			return this.NextResult(false, false);
		}

		private bool NextResult(bool disposing, bool allresults)
		{
			ODBC32.RetCode retcode = ODBC32.RetCode.SUCCESS;
			bool flag = false;
			bool flag2 = this.IsCommandBehavior(CommandBehavior.SingleResult);
			if (this.IsClosed)
			{
				throw ADP.DataReaderClosed("NextResult");
			}
			this._fieldNameLookup = null;
			if (this.IsCancelingCommand || this._noMoreResults)
			{
				return false;
			}
			this._isRead = false;
			this._hasRows = OdbcDataReader.HasRowsStatus.DontKnow;
			this._fieldNameLookup = null;
			this._metadata = null;
			this._schemaTable = null;
			int num = 0;
			OdbcErrorCollection odbcErrorCollection = null;
			ODBC32.RetCode retCode;
			bool flag3;
			do
			{
				this._isValidResult = false;
				retCode = this.StatementHandle.MoreResults();
				flag3 = (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO);
				if (retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					this.Connection.HandleErrorNoThrow(this.StatementHandle, retCode);
				}
				else if (!disposing && retCode != ODBC32.RetCode.NO_DATA && retCode != ODBC32.RetCode.SUCCESS)
				{
					if (odbcErrorCollection == null)
					{
						retcode = retCode;
						odbcErrorCollection = new OdbcErrorCollection();
					}
					ODBC32.GetDiagErrors(odbcErrorCollection, null, this.StatementHandle, retCode);
					num++;
				}
				if (!disposing && flag3)
				{
					num = 0;
					SQLLEN rowCount = this.GetRowCount();
					this.CalculateRecordsAffected(rowCount);
					if (!flag2)
					{
						short num2;
						this.FieldCountNoThrow(out num2);
						flag = (num2 != 0);
						this._isValidResult = flag;
					}
				}
			}
			while ((!flag2 && flag3 && !flag) || (ODBC32.RetCode.NO_DATA != retCode && allresults && num < 2000) || (flag2 && flag3));
			if (retCode == ODBC32.RetCode.NO_DATA)
			{
				this._dataCache = null;
				this._noMoreResults = true;
			}
			if (odbcErrorCollection != null)
			{
				odbcErrorCollection.SetSource(this.Connection.Driver);
				OdbcException ex = OdbcException.CreateException(odbcErrorCollection, retcode);
				this.Connection.ConnectionIsAlive(ex);
				throw ex;
			}
			return flag3;
		}

		private void BuildMetaDataInfo()
		{
			int fieldCount = this.FieldCount;
			OdbcDataReader.MetaData[] array = new OdbcDataReader.MetaData[fieldCount];
			bool flag = this.IsCommandBehavior(CommandBehavior.KeyInfo);
			List<string> list;
			if (flag)
			{
				list = new List<string>();
			}
			else
			{
				list = null;
			}
			for (int i = 0; i < fieldCount; i++)
			{
				array[i] = new OdbcDataReader.MetaData();
				array[i].ordinal = i;
				TypeMap typeMap = TypeMap.FromSqlType((ODBC32.SQL_TYPE)this.GetColAttribute(i, ODBC32.SQL_DESC.CONCISE_TYPE, ODBC32.SQL_COLUMN.TYPE, ODBC32.HANDLER.THROW));
				if (typeMap._signType)
				{
					bool unsigned = this.GetColAttribute(i, ODBC32.SQL_DESC.UNSIGNED, ODBC32.SQL_COLUMN.UNSIGNED, ODBC32.HANDLER.THROW).ToInt64() != 0L;
					typeMap = TypeMap.UpgradeSignedType(typeMap, unsigned);
				}
				array[i].typemap = typeMap;
				array[i].size = this.GetColAttribute(i, ODBC32.SQL_DESC.OCTET_LENGTH, ODBC32.SQL_COLUMN.LENGTH, ODBC32.HANDLER.IGNORE);
				ODBC32.SQL_TYPE sql_type = array[i].typemap._sql_type;
				if (sql_type - ODBC32.SQL_TYPE.WLONGVARCHAR <= 2)
				{
					OdbcDataReader.MetaData metaData = array[i];
					metaData.size /= 2;
				}
				array[i].precision = (byte)this.GetColAttribute(i, (ODBC32.SQL_DESC)4, ODBC32.SQL_COLUMN.PRECISION, ODBC32.HANDLER.IGNORE);
				array[i].scale = (byte)this.GetColAttribute(i, (ODBC32.SQL_DESC)5, ODBC32.SQL_COLUMN.SCALE, ODBC32.HANDLER.IGNORE);
				array[i].isAutoIncrement = (this.GetColAttribute(i, ODBC32.SQL_DESC.AUTO_UNIQUE_VALUE, ODBC32.SQL_COLUMN.AUTO_INCREMENT, ODBC32.HANDLER.IGNORE) == 1);
				array[i].isReadOnly = (this.GetColAttribute(i, ODBC32.SQL_DESC.UPDATABLE, ODBC32.SQL_COLUMN.UPDATABLE, ODBC32.HANDLER.IGNORE) == 0);
				ODBC32.SQL_NULLABILITY sql_NULLABILITY = (ODBC32.SQL_NULLABILITY)this.GetColAttribute(i, ODBC32.SQL_DESC.NULLABLE, ODBC32.SQL_COLUMN.NULLABLE, ODBC32.HANDLER.IGNORE);
				array[i].isNullable = (sql_NULLABILITY == ODBC32.SQL_NULLABILITY.NULLABLE);
				sql_type = array[i].typemap._sql_type;
				if (sql_type == ODBC32.SQL_TYPE.WLONGVARCHAR || sql_type == ODBC32.SQL_TYPE.LONGVARBINARY || sql_type == ODBC32.SQL_TYPE.LONGVARCHAR)
				{
					array[i].isLong = true;
				}
				else
				{
					array[i].isLong = false;
				}
				if (this.IsCommandBehavior(CommandBehavior.KeyInfo))
				{
					if (!this.Connection.ProviderInfo.NoSqlCASSColumnKey)
					{
						bool flag2 = this.GetColAttribute(i, (ODBC32.SQL_DESC)1212, (ODBC32.SQL_COLUMN)(-1), ODBC32.HANDLER.IGNORE) == 1;
						if (flag2)
						{
							array[i].isKeyColumn = flag2;
							array[i].isUnique = true;
							flag = false;
						}
					}
					array[i].baseSchemaName = this.GetColAttributeStr(i, ODBC32.SQL_DESC.SCHEMA_NAME, ODBC32.SQL_COLUMN.OWNER_NAME, ODBC32.HANDLER.IGNORE);
					array[i].baseCatalogName = this.GetColAttributeStr(i, ODBC32.SQL_DESC.CATALOG_NAME, (ODBC32.SQL_COLUMN)(-1), ODBC32.HANDLER.IGNORE);
					array[i].baseTableName = this.GetColAttributeStr(i, ODBC32.SQL_DESC.BASE_TABLE_NAME, ODBC32.SQL_COLUMN.TABLE_NAME, ODBC32.HANDLER.IGNORE);
					array[i].baseColumnName = this.GetColAttributeStr(i, ODBC32.SQL_DESC.BASE_COLUMN_NAME, ODBC32.SQL_COLUMN.NAME, ODBC32.HANDLER.IGNORE);
					if (this.Connection.IsV3Driver)
					{
						if (array[i].baseTableName == null || array[i].baseTableName.Length == 0)
						{
							array[i].baseTableName = this.GetDescFieldStr(i, ODBC32.SQL_DESC.BASE_TABLE_NAME, ODBC32.HANDLER.IGNORE);
						}
						if (array[i].baseColumnName == null || array[i].baseColumnName.Length == 0)
						{
							array[i].baseColumnName = this.GetDescFieldStr(i, ODBC32.SQL_DESC.BASE_COLUMN_NAME, ODBC32.HANDLER.IGNORE);
						}
					}
					if (array[i].baseTableName != null && !list.Contains(array[i].baseTableName))
					{
						list.Add(array[i].baseTableName);
					}
				}
				if ((array[i].isKeyColumn || array[i].isAutoIncrement) && sql_NULLABILITY == ODBC32.SQL_NULLABILITY.UNKNOWN)
				{
					array[i].isNullable = false;
				}
			}
			if (!this.Connection.ProviderInfo.NoSqlCASSColumnKey)
			{
				for (int j = fieldCount; j < fieldCount + this._hiddenColumns; j++)
				{
					bool flag2 = this.GetColAttribute(j, (ODBC32.SQL_DESC)1212, (ODBC32.SQL_COLUMN)(-1), ODBC32.HANDLER.IGNORE) == 1;
					if (flag2 && this.GetColAttribute(j, (ODBC32.SQL_DESC)1211, (ODBC32.SQL_COLUMN)(-1), ODBC32.HANDLER.IGNORE) == 1)
					{
						for (int k = 0; k < fieldCount; k++)
						{
							array[k].isKeyColumn = false;
							array[k].isUnique = false;
						}
					}
				}
			}
			this._metadata = array;
			if (this.IsCommandBehavior(CommandBehavior.KeyInfo))
			{
				if (list != null && list.Count > 0)
				{
					List<string>.Enumerator enumerator = list.GetEnumerator();
					OdbcDataReader.QualifiedTableName qualifiedTableName = new OdbcDataReader.QualifiedTableName(this.Connection.QuoteChar("GetSchemaTable"));
					while (enumerator.MoveNext())
					{
						string table = enumerator.Current;
						qualifiedTableName.Table = table;
						if (this.RetrieveKeyInfo(flag, qualifiedTableName, false) <= 0)
						{
							this.RetrieveKeyInfo(flag, qualifiedTableName, true);
						}
					}
					return;
				}
				OdbcDataReader.QualifiedTableName qualifiedTableName2 = new OdbcDataReader.QualifiedTableName(this.Connection.QuoteChar("GetSchemaTable"), this.GetTableNameFromCommandText());
				if (!string.IsNullOrEmpty(qualifiedTableName2.Table))
				{
					this.SetBaseTableNames(qualifiedTableName2);
					if (this.RetrieveKeyInfo(flag, qualifiedTableName2, false) <= 0)
					{
						this.RetrieveKeyInfo(flag, qualifiedTableName2, true);
					}
				}
			}
		}

		private DataTable NewSchemaTable()
		{
			DataTable dataTable = new DataTable("SchemaTable");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.MinimumCapacity = this.FieldCount;
			DataColumnCollection columns = dataTable.Columns;
			columns.Add(new DataColumn("ColumnName", typeof(string)));
			columns.Add(new DataColumn("ColumnOrdinal", typeof(int)));
			columns.Add(new DataColumn("ColumnSize", typeof(int)));
			columns.Add(new DataColumn("NumericPrecision", typeof(short)));
			columns.Add(new DataColumn("NumericScale", typeof(short)));
			columns.Add(new DataColumn("DataType", typeof(object)));
			columns.Add(new DataColumn("ProviderType", typeof(int)));
			columns.Add(new DataColumn("IsLong", typeof(bool)));
			columns.Add(new DataColumn("AllowDBNull", typeof(bool)));
			columns.Add(new DataColumn("IsReadOnly", typeof(bool)));
			columns.Add(new DataColumn("IsRowVersion", typeof(bool)));
			columns.Add(new DataColumn("IsUnique", typeof(bool)));
			columns.Add(new DataColumn("IsKey", typeof(bool)));
			columns.Add(new DataColumn("IsAutoIncrement", typeof(bool)));
			columns.Add(new DataColumn("BaseSchemaName", typeof(string)));
			columns.Add(new DataColumn("BaseCatalogName", typeof(string)));
			columns.Add(new DataColumn("BaseTableName", typeof(string)));
			columns.Add(new DataColumn("BaseColumnName", typeof(string)));
			foreach (object obj in columns)
			{
				((DataColumn)obj).ReadOnly = true;
			}
			return dataTable;
		}

		/// <summary>Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.Odbc.OdbcDataReader" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that describes the column metadata.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.Odbc.OdbcDataReader" /> is closed.</exception>
		public override DataTable GetSchemaTable()
		{
			if (this.IsClosed)
			{
				throw ADP.DataReaderClosed("GetSchemaTable");
			}
			if (this._noMoreResults)
			{
				return null;
			}
			if (this._schemaTable != null)
			{
				return this._schemaTable;
			}
			DataTable dataTable = this.NewSchemaTable();
			if (this.FieldCount == 0)
			{
				return dataTable;
			}
			if (this._metadata == null)
			{
				this.BuildMetaDataInfo();
			}
			DataColumn column = dataTable.Columns["ColumnName"];
			DataColumn column2 = dataTable.Columns["ColumnOrdinal"];
			DataColumn column3 = dataTable.Columns["ColumnSize"];
			DataColumn column4 = dataTable.Columns["NumericPrecision"];
			DataColumn column5 = dataTable.Columns["NumericScale"];
			DataColumn column6 = dataTable.Columns["DataType"];
			DataColumn column7 = dataTable.Columns["ProviderType"];
			DataColumn column8 = dataTable.Columns["IsLong"];
			DataColumn column9 = dataTable.Columns["AllowDBNull"];
			DataColumn column10 = dataTable.Columns["IsReadOnly"];
			DataColumn column11 = dataTable.Columns["IsRowVersion"];
			DataColumn column12 = dataTable.Columns["IsUnique"];
			DataColumn column13 = dataTable.Columns["IsKey"];
			DataColumn column14 = dataTable.Columns["IsAutoIncrement"];
			DataColumn column15 = dataTable.Columns["BaseSchemaName"];
			DataColumn column16 = dataTable.Columns["BaseCatalogName"];
			DataColumn column17 = dataTable.Columns["BaseTableName"];
			DataColumn column18 = dataTable.Columns["BaseColumnName"];
			int fieldCount = this.FieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[column] = this.GetName(i);
				dataRow[column2] = i;
				dataRow[column3] = (int)Math.Min(Math.Max(-2147483648L, this._metadata[i].size.ToInt64()), 2147483647L);
				dataRow[column4] = (short)this._metadata[i].precision;
				dataRow[column5] = (short)this._metadata[i].scale;
				dataRow[column6] = this._metadata[i].typemap._type;
				dataRow[column7] = this._metadata[i].typemap._odbcType;
				dataRow[column8] = this._metadata[i].isLong;
				dataRow[column9] = this._metadata[i].isNullable;
				dataRow[column10] = this._metadata[i].isReadOnly;
				dataRow[column11] = this._metadata[i].isRowVersion;
				dataRow[column12] = this._metadata[i].isUnique;
				dataRow[column13] = this._metadata[i].isKeyColumn;
				dataRow[column14] = this._metadata[i].isAutoIncrement;
				dataRow[column15] = this._metadata[i].baseSchemaName;
				dataRow[column16] = this._metadata[i].baseCatalogName;
				dataRow[column17] = this._metadata[i].baseTableName;
				dataRow[column18] = this._metadata[i].baseColumnName;
				dataTable.Rows.Add(dataRow);
				dataRow.AcceptChanges();
			}
			this._schemaTable = dataTable;
			return dataTable;
		}

		internal int RetrieveKeyInfo(bool needkeyinfo, OdbcDataReader.QualifiedTableName qualifiedTableName, bool quoted)
		{
			int num = 0;
			IntPtr value = IntPtr.Zero;
			if (this.IsClosed || this._cmdWrapper == null)
			{
				return 0;
			}
			this._cmdWrapper.CreateKeyInfoStatementHandle();
			CNativeBuffer buffer = this.Buffer;
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				buffer.DangerousAddRef(ref flag);
				ODBC32.RetCode retCode;
				if (needkeyinfo)
				{
					if (!this.Connection.ProviderInfo.NoSqlPrimaryKeys)
					{
						retCode = this.KeyInfoStatementHandle.PrimaryKeys(qualifiedTableName.Catalog, qualifiedTableName.Schema, qualifiedTableName.GetTable(quoted));
						if (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
						{
							bool flag2 = false;
							buffer.WriteInt16(0, 0);
							retCode = this.KeyInfoStatementHandle.BindColumn2(4, ODBC32.SQL_C.WCHAR, buffer.PtrOffset(0, 256), (IntPtr)256, buffer.PtrOffset(256, IntPtr.Size).Handle);
							while (this.KeyInfoStatementHandle.Fetch() == ODBC32.RetCode.SUCCESS)
							{
								value = buffer.ReadIntPtr(256);
								string text = buffer.PtrToStringUni(0, (int)value / 2);
								int ordinalFromBaseColName = this.GetOrdinalFromBaseColName(text);
								if (ordinalFromBaseColName == -1)
								{
									flag2 = true;
									break;
								}
								num++;
								this._metadata[ordinalFromBaseColName].isKeyColumn = true;
								this._metadata[ordinalFromBaseColName].isUnique = true;
								this._metadata[ordinalFromBaseColName].isNullable = false;
								this._metadata[ordinalFromBaseColName].baseTableName = qualifiedTableName.Table;
								if (this._metadata[ordinalFromBaseColName].baseColumnName == null)
								{
									this._metadata[ordinalFromBaseColName].baseColumnName = text;
								}
							}
							if (flag2)
							{
								OdbcDataReader.MetaData[] metadata = this._metadata;
								for (int i = 0; i < metadata.Length; i++)
								{
									metadata[i].isKeyColumn = false;
								}
							}
							retCode = this.KeyInfoStatementHandle.BindColumn3(4, ODBC32.SQL_C.WCHAR, buffer.DangerousGetHandle());
						}
						else if ("IM001" == this.Command.GetDiagSqlState())
						{
							this.Connection.ProviderInfo.NoSqlPrimaryKeys = true;
						}
					}
					if (num == 0)
					{
						this.KeyInfoStatementHandle.MoreResults();
						num += this.RetrieveKeyInfoFromStatistics(qualifiedTableName, quoted);
					}
					this.KeyInfoStatementHandle.MoreResults();
				}
				retCode = this.KeyInfoStatementHandle.SpecialColumns(qualifiedTableName.GetTable(quoted));
				if (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					value = IntPtr.Zero;
					buffer.WriteInt16(0, 0);
					retCode = this.KeyInfoStatementHandle.BindColumn2(2, ODBC32.SQL_C.WCHAR, buffer.PtrOffset(0, 256), (IntPtr)256, buffer.PtrOffset(256, IntPtr.Size).Handle);
					while (this.KeyInfoStatementHandle.Fetch() == ODBC32.RetCode.SUCCESS)
					{
						value = buffer.ReadIntPtr(256);
						string text = buffer.PtrToStringUni(0, (int)value / 2);
						int ordinalFromBaseColName = this.GetOrdinalFromBaseColName(text);
						if (ordinalFromBaseColName != -1)
						{
							this._metadata[ordinalFromBaseColName].isRowVersion = true;
							if (this._metadata[ordinalFromBaseColName].baseColumnName == null)
							{
								this._metadata[ordinalFromBaseColName].baseColumnName = text;
							}
						}
					}
					retCode = this.KeyInfoStatementHandle.BindColumn3(2, ODBC32.SQL_C.WCHAR, buffer.DangerousGetHandle());
					retCode = this.KeyInfoStatementHandle.MoreResults();
				}
			}
			finally
			{
				if (flag)
				{
					buffer.DangerousRelease();
				}
			}
			return num;
		}

		private int RetrieveKeyInfoFromStatistics(OdbcDataReader.QualifiedTableName qualifiedTableName, bool quoted)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string currentindexname = string.Empty;
			int[] array = new int[16];
			int[] array2 = new int[16];
			int num = 0;
			int num2 = 0;
			bool flag = false;
			IntPtr value = IntPtr.Zero;
			IntPtr value2 = IntPtr.Zero;
			int num3 = 0;
			string tableName = string.Copy(qualifiedTableName.GetTable(quoted));
			ODBC32.RetCode retCode = this.KeyInfoStatementHandle.Statistics(tableName);
			if (retCode != ODBC32.RetCode.SUCCESS)
			{
				return 0;
			}
			CNativeBuffer buffer = this.Buffer;
			bool flag2 = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				buffer.DangerousAddRef(ref flag2);
				HandleRef buffer2 = buffer.PtrOffset(0, 256);
				HandleRef buffer3 = buffer.PtrOffset(256, 256);
				HandleRef buffer4 = buffer.PtrOffset(512, 4);
				IntPtr handle = buffer.PtrOffset(520, IntPtr.Size).Handle;
				IntPtr handle2 = buffer.PtrOffset(528, IntPtr.Size).Handle;
				IntPtr handle3 = buffer.PtrOffset(536, IntPtr.Size).Handle;
				buffer.WriteInt16(256, 0);
				retCode = this.KeyInfoStatementHandle.BindColumn2(6, ODBC32.SQL_C.WCHAR, buffer3, (IntPtr)256, handle2);
				retCode = this.KeyInfoStatementHandle.BindColumn2(8, ODBC32.SQL_C.SSHORT, buffer4, (IntPtr)4, handle3);
				buffer.WriteInt16(512, 0);
				retCode = this.KeyInfoStatementHandle.BindColumn2(9, ODBC32.SQL_C.WCHAR, buffer2, (IntPtr)256, handle);
				while (this.KeyInfoStatementHandle.Fetch() == ODBC32.RetCode.SUCCESS)
				{
					value2 = buffer.ReadIntPtr(520);
					value = buffer.ReadIntPtr(528);
					if (buffer.ReadInt16(256) != 0)
					{
						text = buffer.PtrToStringUni(0, (int)value2 / 2);
						text2 = buffer.PtrToStringUni(256, (int)value / 2);
						int num4 = (int)buffer.ReadInt16(512);
						if (this.SameIndexColumn(currentindexname, text2, num4, num2))
						{
							if (!flag)
							{
								num4 = this.GetOrdinalFromBaseColName(text, qualifiedTableName.Table);
								if (num4 == -1)
								{
									flag = true;
								}
								else if (num2 < 16)
								{
									array[num2++] = num4;
								}
								else
								{
									flag = true;
								}
							}
						}
						else
						{
							if (!flag && num2 != 0 && (num == 0 || num > num2))
							{
								num = num2;
								for (int i = 0; i < num2; i++)
								{
									array2[i] = array[i];
								}
							}
							num2 = 0;
							currentindexname = text2;
							flag = false;
							num4 = this.GetOrdinalFromBaseColName(text, qualifiedTableName.Table);
							if (num4 == -1)
							{
								flag = true;
							}
							else
							{
								array[num2++] = num4;
							}
						}
					}
				}
				if (!flag && num2 != 0 && (num == 0 || num > num2))
				{
					num = num2;
					for (int j = 0; j < num2; j++)
					{
						array2[j] = array[j];
					}
				}
				if (num != 0)
				{
					for (int k = 0; k < num; k++)
					{
						int num5 = array2[k];
						num3++;
						this._metadata[num5].isKeyColumn = true;
						this._metadata[num5].isNullable = false;
						this._metadata[num5].isUnique = true;
						if (this._metadata[num5].baseTableName == null)
						{
							this._metadata[num5].baseTableName = qualifiedTableName.Table;
						}
						if (this._metadata[num5].baseColumnName == null)
						{
							this._metadata[num5].baseColumnName = text;
						}
					}
				}
				this._cmdWrapper.FreeKeyInfoStatementHandle(ODBC32.STMT.UNBIND);
			}
			finally
			{
				if (flag2)
				{
					buffer.DangerousRelease();
				}
			}
			return num3;
		}

		internal bool SameIndexColumn(string currentindexname, string indexname, int ordinal, int ncols)
		{
			return !string.IsNullOrEmpty(currentindexname) && (currentindexname == indexname && ordinal == ncols + 1);
		}

		internal int GetOrdinalFromBaseColName(string columnname)
		{
			return this.GetOrdinalFromBaseColName(columnname, null);
		}

		internal int GetOrdinalFromBaseColName(string columnname, string tablename)
		{
			if (string.IsNullOrEmpty(columnname))
			{
				return -1;
			}
			if (this._metadata != null)
			{
				int fieldCount = this.FieldCount;
				for (int i = 0; i < fieldCount; i++)
				{
					if (this._metadata[i].baseColumnName != null && columnname == this._metadata[i].baseColumnName)
					{
						if (string.IsNullOrEmpty(tablename))
						{
							return i;
						}
						if (tablename == this._metadata[i].baseTableName)
						{
							return i;
						}
					}
				}
			}
			return this.IndexOf(columnname);
		}

		internal string GetTableNameFromCommandText()
		{
			if (this._command == null)
			{
				return null;
			}
			string text = this._cmdText;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			CStringTokenizer cstringTokenizer = new CStringTokenizer(text, this.Connection.QuoteChar("GetSchemaTable")[0], this.Connection.EscapeChar("GetSchemaTable"));
			int num;
			if (cstringTokenizer.StartsWith("select"))
			{
				num = cstringTokenizer.FindTokenIndex("from");
			}
			else if (cstringTokenizer.StartsWith("insert") || cstringTokenizer.StartsWith("update") || cstringTokenizer.StartsWith("delete"))
			{
				num = cstringTokenizer.CurrentPosition;
			}
			else
			{
				num = -1;
			}
			if (num == -1)
			{
				return null;
			}
			string result = cstringTokenizer.NextToken();
			text = cstringTokenizer.NextToken();
			if (text.Length > 0 && text[0] == ',')
			{
				return null;
			}
			if (text.Length == 2 && (text[0] == 'a' || text[0] == 'A') && (text[1] == 's' || text[1] == 'S'))
			{
				text = cstringTokenizer.NextToken();
				text = cstringTokenizer.NextToken();
				if (text.Length > 0 && text[0] == ',')
				{
					return null;
				}
			}
			return result;
		}

		internal void SetBaseTableNames(OdbcDataReader.QualifiedTableName qualifiedTableName)
		{
			int fieldCount = this.FieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				if (this._metadata[i].baseTableName == null)
				{
					this._metadata[i].baseTableName = qualifiedTableName.Table;
					this._metadata[i].baseSchemaName = qualifiedTableName.Schema;
					this._metadata[i].baseCatalogName = qualifiedTableName.Catalog;
				}
			}
		}

		internal OdbcDataReader()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private OdbcCommand _command;

		private int _recordAffected;

		private FieldNameLookup _fieldNameLookup;

		private DbCache _dataCache;

		private OdbcDataReader.HasRowsStatus _hasRows;

		private bool _isClosed;

		private bool _isRead;

		private bool _isValidResult;

		private bool _noMoreResults;

		private bool _noMoreRows;

		private bool _skipReadOnce;

		private int _hiddenColumns;

		private CommandBehavior _commandBehavior;

		private int _row;

		private int _column;

		private long _sequentialBytesRead;

		private static int s_objectTypeCount;

		internal readonly int ObjectID;

		private OdbcDataReader.MetaData[] _metadata;

		private DataTable _schemaTable;

		private string _cmdText;

		private CMDWrapper _cmdWrapper;

		private enum HasRowsStatus
		{
			DontKnow,
			HasRows,
			HasNoRows
		}

		internal sealed class QualifiedTableName
		{
			internal string Catalog
			{
				get
				{
					return this._catalogName;
				}
			}

			internal string Schema
			{
				get
				{
					return this._schemaName;
				}
			}

			internal string Table
			{
				get
				{
					return this._tableName;
				}
				set
				{
					this._quotedTableName = value;
					this._tableName = this.UnQuote(value);
				}
			}

			internal string QuotedTable
			{
				get
				{
					return this._quotedTableName;
				}
			}

			internal string GetTable(bool flag)
			{
				if (!flag)
				{
					return this.Table;
				}
				return this.QuotedTable;
			}

			internal QualifiedTableName(string quoteChar)
			{
				this._quoteChar = quoteChar;
			}

			internal QualifiedTableName(string quoteChar, string qualifiedname)
			{
				this._quoteChar = quoteChar;
				string[] array = OdbcDataReader.QualifiedTableName.ParseProcedureName(qualifiedname, quoteChar, quoteChar);
				this._catalogName = this.UnQuote(array[1]);
				this._schemaName = this.UnQuote(array[2]);
				this._quotedTableName = array[3];
				this._tableName = this.UnQuote(array[3]);
			}

			private string UnQuote(string str)
			{
				if (str != null && str.Length > 0)
				{
					char c = this._quoteChar[0];
					if (str[0] == c && str.Length > 1 && str[str.Length - 1] == c)
					{
						str = str.Substring(1, str.Length - 2);
					}
				}
				return str;
			}

			internal static string[] ParseProcedureName(string name, string quotePrefix, string quoteSuffix)
			{
				string[] array = new string[4];
				if (!string.IsNullOrEmpty(name))
				{
					bool flag = !string.IsNullOrEmpty(quotePrefix) && !string.IsNullOrEmpty(quoteSuffix);
					int i = 0;
					int num = 0;
					while (num < array.Length && i < name.Length)
					{
						int num2 = i;
						if (flag && name.IndexOf(quotePrefix, i, quotePrefix.Length, StringComparison.Ordinal) == i)
						{
							for (i += quotePrefix.Length; i < name.Length; i += quoteSuffix.Length)
							{
								i = name.IndexOf(quoteSuffix, i, StringComparison.Ordinal);
								if (i < 0)
								{
									i = name.Length;
									break;
								}
								i += quoteSuffix.Length;
								if (i >= name.Length || name.IndexOf(quoteSuffix, i, quoteSuffix.Length, StringComparison.Ordinal) != i)
								{
									break;
								}
							}
						}
						if (i < name.Length)
						{
							i = name.IndexOf(".", i, StringComparison.Ordinal);
							if (i < 0 || num == array.Length - 1)
							{
								i = name.Length;
							}
						}
						array[num] = name.Substring(num2, i - num2);
						i += ".".Length;
						num++;
					}
					int num3 = array.Length - 1;
					while (0 <= num3)
					{
						array[num3] = ((0 < num) ? array[--num] : null);
						num3--;
					}
				}
				return array;
			}

			private string _catalogName;

			private string _schemaName;

			private string _tableName;

			private string _quotedTableName;

			private string _quoteChar;
		}

		private sealed class MetaData
		{
			internal int ordinal;

			internal TypeMap typemap;

			internal SQLLEN size;

			internal byte precision;

			internal byte scale;

			internal bool isAutoIncrement;

			internal bool isUnique;

			internal bool isReadOnly;

			internal bool isNullable;

			internal bool isRowVersion;

			internal bool isLong;

			internal bool isKeyColumn;

			internal string baseSchemaName;

			internal string baseCatalogName;

			internal string baseTableName;

			internal string baseColumnName;
		}
	}
}
