using System;
using System.Data;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Represents a single row of data and its metadata. This class cannot be inherited.</summary>
	public class SqlDataRecord : IDataRecord
	{
		/// <summary>Gets the number of columns in the data row. This property is read-only.</summary>
		/// <returns>The number of columns in the data row as an integer.</returns>
		public virtual int FieldCount
		{
			get
			{
				this.EnsureSubclassOverride();
				return this._columnMetaData.Length;
			}
		}

		/// <summary>Returns the name of the column specified by the ordinal argument.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>A <see cref="T:System.String" /> containing the column name.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual string GetName(int ordinal)
		{
			this.EnsureSubclassOverride();
			return this.GetSqlMetaData(ordinal).Name;
		}

		/// <summary>Returns the name of the data type for the column specified by the ordinal argument.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>A <see cref="T:System.String" /> that contains the data type of the column.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual string GetDataTypeName(int ordinal)
		{
			this.EnsureSubclassOverride();
			SqlMetaData sqlMetaData = this.GetSqlMetaData(ordinal);
			if (SqlDbType.Udt == sqlMetaData.SqlDbType)
			{
				return sqlMetaData.UdtTypeName;
			}
			return MetaType.GetMetaTypeFromSqlDbType(sqlMetaData.SqlDbType, false).TypeName;
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object representing the common language runtime (CLR) type that maps to the SQL Server type of the column specified by the <paramref name="ordinal" /> argument.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column type as a <see cref="T:System.Type" /> object.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.TypeLoadException">The column is of a user-defined type that is not available to the calling application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The column is of a user-defined type that is not available to the calling application domain.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual Type GetFieldType(int ordinal)
		{
			this.EnsureSubclassOverride();
			return MetaType.GetMetaTypeFromSqlDbType(this.GetSqlMetaData(ordinal).SqlDbType, false).ClassType;
		}

		/// <summary>Returns the common language runtime (CLR) type value for the column specified by the ordinal argument.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The CLR type value of the column specified by the ordinal.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual object GetValue(int ordinal)
		{
			this.EnsureSubclassOverride();
			SmiMetaData smiMetaData = this.GetSmiMetaData(ordinal);
			return ValueUtilsSmi.GetValue200(this._eventSink, this._recordBuffer, ordinal, smiMetaData);
		}

		/// <summary>Returns the values for all the columns in the record, expressed as common language runtime (CLR) types, in an array.</summary>
		/// <param name="values">The array into which to copy the values column values.</param>
		/// <returns>An <see cref="T:System.Int32" /> that indicates the number of columns copied.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="values" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual int GetValues(object[] values)
		{
			this.EnsureSubclassOverride();
			if (values == null)
			{
				throw ADP.ArgumentNull("values");
			}
			int num = (values.Length < this.FieldCount) ? values.Length : this.FieldCount;
			for (int i = 0; i < num; i++)
			{
				values[i] = this.GetValue(i);
			}
			return num;
		}

		/// <summary>Returns the column ordinal specified by the column name.</summary>
		/// <param name="name">The name of the column to look up.</param>
		/// <returns>The zero-based ordinal of the column as an integer.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IndexOutOfRangeException">The column name could not be found.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual int GetOrdinal(string name)
		{
			this.EnsureSubclassOverride();
			if (this._fieldNameLookup == null)
			{
				string[] array = new string[this.FieldCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.GetSqlMetaData(i).Name;
				}
				this._fieldNameLookup = new FieldNameLookup(array, -1);
			}
			return this._fieldNameLookup.GetOrdinal(name);
		}

		/// <summary>Gets the common language runtime (CLR) type value for the column specified by the column <paramref name="ordinal" /> argument.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The CLR type value of the column specified by the <paramref name="ordinal" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual object this[int ordinal]
		{
			get
			{
				this.EnsureSubclassOverride();
				return this.GetValue(ordinal);
			}
		}

		/// <summary>Gets the common language runtime (CLR) type value for the column specified by the column <paramref name="name" /> argument.</summary>
		/// <param name="name">The name of the column.</param>
		/// <returns>The CLR type value of the column specified by the <paramref name="name" />.</returns>
		public virtual object this[string name]
		{
			get
			{
				this.EnsureSubclassOverride();
				return this.GetValue(this.GetOrdinal(name));
			}
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Boolean" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Boolean" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual bool GetBoolean(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetBoolean(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Byte" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Byte" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual byte GetByte(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetByte(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as an array of <see cref="T:System.Byte" /> objects.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="fieldOffset">The offset into the field value to start retrieving bytes.</param>
		/// <param name="buffer">The target buffer to which to copy bytes.</param>
		/// <param name="bufferOffset">The offset into the buffer to which to start copying bytes.</param>
		/// <param name="length">The number of bytes to copy to the buffer.</param>
		/// <returns>The number of bytes copied.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual long GetBytes(int ordinal, long fieldOffset, byte[] buffer, int bufferOffset, int length)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetBytes(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), fieldOffset, buffer, bufferOffset, length, true);
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Char" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Char" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual char GetChar(int ordinal)
		{
			this.EnsureSubclassOverride();
			throw ADP.NotSupported();
		}

		/// <summary>Gets the value for the column specified by the ordinal as an array of <see cref="T:System.Char" /> objects.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="fieldOffset">The offset into the field value to start retrieving characters.</param>
		/// <param name="buffer">The target buffer to copy chars to.</param>
		/// <param name="bufferOffset">The offset into the buffer to start copying chars to.</param>
		/// <param name="length">The number of chars to copy to the buffer.</param>
		/// <returns>The number of characters copied.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual long GetChars(int ordinal, long fieldOffset, char[] buffer, int bufferOffset, int length)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetChars(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), fieldOffset, buffer, bufferOffset, length);
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Guid" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Guid" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual Guid GetGuid(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetGuid(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Int16" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Int16" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual short GetInt16(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetInt16(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Int32" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Int32" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual int GetInt32(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetInt32(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Int64" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Int64" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual long GetInt64(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetInt64(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see langword="float" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see langword="float" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual float GetFloat(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSingle(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Double" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Double" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual double GetDouble(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetDouble(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.String" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.String" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual string GetString(int ordinal)
		{
			this.EnsureSubclassOverride();
			SmiMetaData smiMetaData = this.GetSmiMetaData(ordinal);
			if (this._usesStringStorageForXml && SqlDbType.Xml == smiMetaData.SqlDbType)
			{
				return ValueUtilsSmi.GetString(this._eventSink, this._recordBuffer, ordinal, SqlDataRecord.s_maxNVarCharForXml);
			}
			return ValueUtilsSmi.GetString(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Decimal" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Decimal" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual decimal GetDecimal(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetDecimal(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.DateTime" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.DateTime" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The column specified by <paramref name="ordinal" /> is null.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual DateTime GetDateTime(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetDateTime(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Returns the specified column's data as a <see cref="T:System.DateTimeOffset" />.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a <see cref="T:System.DateTimeOffset" />.</returns>
		public virtual DateTimeOffset GetDateTimeOffset(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetDateTimeOffset(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Returns the specified column's data as a <see cref="T:System.TimeSpan" />.</summary>
		/// <param name="ordinal">The zero-based column ordinal.</param>
		/// <returns>The value of the specified column as a <see cref="T:System.TimeSpan" />.</returns>
		public virtual TimeSpan GetTimeSpan(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetTimeSpan(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Returns true if the column specified by the column ordinal parameter is null.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>
		///   <see langword="true" /> if the column is null; <see langword="false" /> otherwise.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual bool IsDBNull(int ordinal)
		{
			this.EnsureSubclassOverride();
			this.ThrowIfInvalidOrdinal(ordinal);
			return ValueUtilsSmi.IsDBNull(this._eventSink, this._recordBuffer, ordinal);
		}

		/// <summary>Returns a <see cref="T:Microsoft.SqlServer.Server.SqlMetaData" /> object, describing the metadata of the column specified by the column ordinal.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column metadata as a <see cref="T:Microsoft.SqlServer.Server.SqlMetaData" /> object.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlMetaData GetSqlMetaData(int ordinal)
		{
			this.EnsureSubclassOverride();
			return this._columnMetaData[ordinal];
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents the type (as a SQL Server type, defined in <see cref="N:System.Data.SqlTypes" />) that maps to the SQL Server type of the column.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column type as a <see cref="T:System.Type" /> object.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.TypeLoadException">The column is of a user-defined type that is not available to the calling application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The column is of a user-defined type that is not available to the calling application domain.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual Type GetSqlFieldType(int ordinal)
		{
			this.EnsureSubclassOverride();
			return MetaType.GetMetaTypeFromSqlDbType(this.GetSqlMetaData(ordinal).SqlDbType, false).SqlType;
		}

		/// <summary>Returns the data value stored in the column, expressed as a SQL Server type, specified by the column ordinal.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The value of the column, expressed as a SQL Server type, as a <see cref="T:System.Object" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual object GetSqlValue(int ordinal)
		{
			this.EnsureSubclassOverride();
			SmiMetaData smiMetaData = this.GetSmiMetaData(ordinal);
			return ValueUtilsSmi.GetSqlValue200(this._eventSink, this._recordBuffer, ordinal, smiMetaData);
		}

		/// <summary>Returns the values for all the columns in the record, expressed as SQL Server types, in an array.</summary>
		/// <param name="values">The array into which to copy the values column values.</param>
		/// <returns>An <see cref="T:System.Int32" /> that indicates the number of columns copied.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="values" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual int GetSqlValues(object[] values)
		{
			this.EnsureSubclassOverride();
			if (values == null)
			{
				throw ADP.ArgumentNull("values");
			}
			int num = (values.Length < this.FieldCount) ? values.Length : this.FieldCount;
			for (int i = 0; i < num; i++)
			{
				values[i] = this.GetSqlValue(i);
			}
			return num;
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlBinary" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlBinary" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlBinary GetSqlBinary(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlBinary(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlBytes" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlBytes" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlBytes GetSqlBytes(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlBytes(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlXml" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlXml" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlXml GetSqlXml(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlXml(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlBoolean" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlBoolean" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlBoolean GetSqlBoolean(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlBoolean(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlByte" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlByte" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlByte GetSqlByte(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlByte(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlChars" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlChars" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlChars GetSqlChars(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlChars(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlInt16" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlInt16" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlInt16 GetSqlInt16(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlInt16(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlInt32" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlInt32" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlInt32 GetSqlInt32(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlInt32(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlInt64" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlInt64" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlInt64 GetSqlInt64(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlInt64(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlSingle GetSqlSingle(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlSingle(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlDouble" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlDouble GetSqlDouble(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlDouble(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlMoney" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlMoney" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlMoney GetSqlMoney(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlMoney(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlDateTime" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlDateTime" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlDateTime GetSqlDateTime(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlDateTime(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlDecimal" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlDecimal" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlDecimal GetSqlDecimal(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlDecimal(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlString GetSqlString(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlString(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Gets the value for the column specified by the ordinal as a <see cref="T:System.Data.SqlTypes.SqlGuid" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>The column value as a <see cref="T:System.Data.SqlTypes.SqlGuid" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		/// <exception cref="T:System.InvalidCastException">There is a type mismatch.</exception>
		public virtual SqlGuid GetSqlGuid(int ordinal)
		{
			this.EnsureSubclassOverride();
			return ValueUtilsSmi.GetSqlGuid(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal));
		}

		/// <summary>Sets new values for all of the columns in the <see cref="T:Microsoft.SqlServer.Server.SqlDataRecord" />. These values are expressed as common language runtime (CLR) types.</summary>
		/// <param name="values">The array of new values, expressed as CLR types boxed as <see cref="T:System.Object" /> references, for the <see cref="T:Microsoft.SqlServer.Server.SqlDataRecord" /> instance.</param>
		/// <returns>The number of column values set as an integer.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="values" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The size of values does not match the number of columns in the <see cref="T:Microsoft.SqlServer.Server.SqlDataRecord" /> instance.</exception>
		public virtual int SetValues(params object[] values)
		{
			this.EnsureSubclassOverride();
			if (values == null)
			{
				throw ADP.ArgumentNull("values");
			}
			int num = (values.Length > this.FieldCount) ? this.FieldCount : values.Length;
			ExtendedClrTypeCode[] array = new ExtendedClrTypeCode[num];
			for (int i = 0; i < num; i++)
			{
				SqlMetaData sqlMetaData = this.GetSqlMetaData(i);
				array[i] = MetaDataUtilsSmi.DetermineExtendedTypeCodeForUseWithSqlDbType(sqlMetaData.SqlDbType, false, values[i], sqlMetaData.Type);
				if (ExtendedClrTypeCode.Invalid == array[i])
				{
					throw ADP.InvalidCast();
				}
			}
			for (int j = 0; j < num; j++)
			{
				ValueUtilsSmi.SetCompatibleValueV200(this._eventSink, this._recordBuffer, j, this.GetSmiMetaData(j), values[j], array[j], 0, 0, null);
			}
			return num;
		}

		/// <summary>Sets a new value, expressed as a common language runtime (CLR) type, for the column specified by the column ordinal.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value for the specified column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetValue(int ordinal, object value)
		{
			this.EnsureSubclassOverride();
			SqlMetaData sqlMetaData = this.GetSqlMetaData(ordinal);
			ExtendedClrTypeCode extendedClrTypeCode = MetaDataUtilsSmi.DetermineExtendedTypeCodeForUseWithSqlDbType(sqlMetaData.SqlDbType, false, value, sqlMetaData.Type);
			if (ExtendedClrTypeCode.Invalid == extendedClrTypeCode)
			{
				throw ADP.InvalidCast();
			}
			ValueUtilsSmi.SetCompatibleValueV200(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value, extendedClrTypeCode, 0, 0, null);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Boolean" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		public virtual void SetBoolean(int ordinal, bool value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetBoolean(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Byte" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetByte(int ordinal, byte value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetByte(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified array of <see cref="T:System.Byte" /> values.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="fieldOffset">The offset into the field value to start copying bytes.</param>
		/// <param name="buffer">The target buffer from which to copy bytes.</param>
		/// <param name="bufferOffset">The offset into the buffer from which to start copying bytes.</param>
		/// <param name="length">The number of bytes to copy from the buffer.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetBytes(int ordinal, long fieldOffset, byte[] buffer, int bufferOffset, int length)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetBytes(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), fieldOffset, buffer, bufferOffset, length);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Char" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetChar(int ordinal, char value)
		{
			this.EnsureSubclassOverride();
			throw ADP.NotSupported();
		}

		/// <summary>Sets the data stored in the column to the specified array of <see cref="T:System.Char" /> values.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="fieldOffset">The offset into the field value to start copying characters.</param>
		/// <param name="buffer">The target buffer from which to copy chars.</param>
		/// <param name="bufferOffset">The offset into the buffer from which to start copying chars.</param>
		/// <param name="length">The number of chars to copy from the buffer.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetChars(int ordinal, long fieldOffset, char[] buffer, int bufferOffset, int length)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetChars(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), fieldOffset, buffer, bufferOffset, length);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Int16" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetInt16(int ordinal, short value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetInt16(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Int32" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetInt32(int ordinal, int value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetInt32(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Int64" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetInt64(int ordinal, long value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetInt64(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see langword="float" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetFloat(int ordinal, float value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSingle(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Double" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetDouble(int ordinal, double value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetDouble(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.String" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetString(int ordinal, string value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetString(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetDecimal(int ordinal, decimal value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetDecimal(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.DateTime" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetDateTime(int ordinal, DateTime value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetDateTime(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the value of the column specified to the <see cref="T:System.TimeSpan" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> passed in is a negative number.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="T:System.TimeSpan" /> value passed in is greater than 24 hours in length.</exception>
		public virtual void SetTimeSpan(int ordinal, TimeSpan value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetTimeSpan(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the value of the column specified to the <see cref="T:System.DateTimeOffset" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		public virtual void SetDateTimeOffset(int ordinal, DateTimeOffset value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetDateTimeOffset(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the value in the specified column to <see cref="T:System.DBNull" />.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		public virtual void SetDBNull(int ordinal)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetDBNull(this._eventSink, this._recordBuffer, ordinal, true);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Guid" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetGuid(int ordinal, Guid value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetGuid(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlBoolean" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlBoolean(int ordinal, SqlBoolean value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlBoolean(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlByte" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlByte(int ordinal, SqlByte value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlByte(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlInt16" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlInt16(int ordinal, SqlInt16 value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlInt16(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlInt32" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlInt32(int ordinal, SqlInt32 value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlInt32(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlInt64" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlInt64(int ordinal, SqlInt64 value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlInt64(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlSingle(int ordinal, SqlSingle value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlSingle(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlDouble" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlDouble(int ordinal, SqlDouble value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlDouble(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlMoney" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlMoney(int ordinal, SqlMoney value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlMoney(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlDateTime" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlDateTime(int ordinal, SqlDateTime value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlDateTime(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlXml" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlXml(int ordinal, SqlXml value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlXml(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlDecimal" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlDecimal(int ordinal, SqlDecimal value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlDecimal(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlString" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlString(int ordinal, SqlString value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlString(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlBinary" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlBinary(int ordinal, SqlBinary value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlBinary(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlGuid" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlGuid(int ordinal, SqlGuid value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlGuid(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlChars" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlChars(int ordinal, SqlChars value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlChars(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Sets the data stored in the column to the specified <see cref="T:System.Data.SqlTypes.SqlBytes" /> value.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <param name="value">The new value of the column.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		public virtual void SetSqlBytes(int ordinal, SqlBytes value)
		{
			this.EnsureSubclassOverride();
			ValueUtilsSmi.SetSqlBytes(this._eventSink, this._recordBuffer, ordinal, this.GetSmiMetaData(ordinal), value);
		}

		/// <summary>Inititializes a new <see cref="T:Microsoft.SqlServer.Server.SqlDataRecord" /> instance with the schema based on the array of <see cref="T:Microsoft.SqlServer.Server.SqlMetaData" /> objects passed as an argument.</summary>
		/// <param name="metaData">An array of <see cref="T:Microsoft.SqlServer.Server.SqlMetaData" /> objects that describe each column in the <see cref="T:Microsoft.SqlServer.Server.SqlDataRecord" />.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="metaData" /> is <see langword="null" />.</exception>
		public SqlDataRecord(params SqlMetaData[] metaData)
		{
			if (metaData == null)
			{
				throw ADP.ArgumentNull("metaData");
			}
			this._columnMetaData = new SqlMetaData[metaData.Length];
			this._columnSmiMetaData = new SmiExtendedMetaData[metaData.Length];
			for (int i = 0; i < this._columnSmiMetaData.Length; i++)
			{
				if (metaData[i] == null)
				{
					throw ADP.ArgumentNull(string.Format("{0}[{1}]", "metaData", i));
				}
				this._columnMetaData[i] = metaData[i];
				this._columnSmiMetaData[i] = MetaDataUtilsSmi.SqlMetaDataToSmiExtendedMetaData(this._columnMetaData[i]);
			}
			this._eventSink = new SmiEventSink_Default();
			SmiMetaData[] columnSmiMetaData = this._columnSmiMetaData;
			this._recordBuffer = new MemoryRecordBuffer(columnSmiMetaData);
			this._usesStringStorageForXml = true;
			this._eventSink.ProcessMessagesAndThrow();
		}

		internal SqlDataRecord(SmiRecordBuffer recordBuffer, params SmiExtendedMetaData[] metaData)
		{
			this._columnMetaData = new SqlMetaData[metaData.Length];
			this._columnSmiMetaData = new SmiExtendedMetaData[metaData.Length];
			for (int i = 0; i < this._columnSmiMetaData.Length; i++)
			{
				this._columnSmiMetaData[i] = metaData[i];
				this._columnMetaData[i] = MetaDataUtilsSmi.SmiExtendedMetaDataToSqlMetaData(this._columnSmiMetaData[i]);
			}
			this._eventSink = new SmiEventSink_Default();
			this._recordBuffer = recordBuffer;
			this._eventSink.ProcessMessagesAndThrow();
		}

		internal SmiRecordBuffer RecordBuffer
		{
			get
			{
				return this._recordBuffer;
			}
		}

		internal SqlMetaData[] InternalGetMetaData()
		{
			return this._columnMetaData;
		}

		internal SmiExtendedMetaData[] InternalGetSmiMetaData()
		{
			return this._columnSmiMetaData;
		}

		internal SmiExtendedMetaData GetSmiMetaData(int ordinal)
		{
			return this._columnSmiMetaData[ordinal];
		}

		internal void ThrowIfInvalidOrdinal(int ordinal)
		{
			if (0 > ordinal || this.FieldCount <= ordinal)
			{
				throw ADP.IndexOutOfRange(ordinal);
			}
		}

		private void EnsureSubclassOverride()
		{
			if (this._recordBuffer == null)
			{
				throw SQL.SubclassMustOverride();
			}
		}

		/// <summary>Not supported in this release.</summary>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		/// <returns>
		///   <see cref="T:System.Data.IDataReader" />  
		///
		/// Always throws an exception.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="ordinal" /> is less than 0 or greater than the number of columns (that is, <see cref="P:Microsoft.SqlServer.Server.SqlDataRecord.FieldCount" />).</exception>
		IDataReader IDataRecord.GetData(int ordinal)
		{
			throw ADP.NotSupported();
		}

		private SmiRecordBuffer _recordBuffer;

		private SmiExtendedMetaData[] _columnSmiMetaData;

		private SmiEventSink_Default _eventSink;

		private SqlMetaData[] _columnMetaData;

		private FieldNameLookup _fieldNameLookup;

		private bool _usesStringStorageForXml;

		private static readonly SmiMetaData s_maxNVarCharForXml = new SmiMetaData(SqlDbType.NVarChar, -1L, SmiMetaData.DefaultNVarChar_NoCollation.Precision, SmiMetaData.DefaultNVarChar_NoCollation.Scale, SmiMetaData.DefaultNVarChar.LocaleId, SmiMetaData.DefaultNVarChar.CompareOptions, null);
	}
}
