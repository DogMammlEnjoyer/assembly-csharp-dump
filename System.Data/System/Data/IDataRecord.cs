using System;

namespace System.Data
{
	/// <summary>Provides access to the column values within each row for a <see langword="DataReader" />, and is implemented by .NET Framework data providers that access relational databases.</summary>
	public interface IDataRecord
	{
		/// <summary>Gets the number of columns in the current row.</summary>
		/// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
		int FieldCount { get; }

		/// <summary>Gets the column located at the specified index.</summary>
		/// <param name="i">The zero-based index of the column to get.</param>
		/// <returns>The column located at the specified index as an <see cref="T:System.Object" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		object this[int i]
		{
			get;
		}

		/// <summary>Gets the column with the specified name.</summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The column with the specified name as an <see cref="T:System.Object" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		object this[string name]
		{
			get;
		}

		/// <summary>Gets the name for the field to find.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The name of the field or the empty string (""), if there is no value to return.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		string GetName(int i);

		/// <summary>Gets the data type information for the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The data type information for the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		string GetDataTypeName(int i);

		/// <summary>Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		Type GetFieldType(int i);

		/// <summary>Return the value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The <see cref="T:System.Object" /> which will contain the field value upon return.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		object GetValue(int i);

		/// <summary>Populates an array of objects with the column values of the current record.</summary>
		/// <param name="values">An array of <see cref="T:System.Object" /> to copy the attribute fields into.</param>
		/// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
		int GetValues(object[] values);

		/// <summary>Return the index of the named field.</summary>
		/// <param name="name">The name of the field to find.</param>
		/// <returns>The index of the named field.</returns>
		int GetOrdinal(string name);

		/// <summary>Gets the value of the specified column as a Boolean.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the column.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		bool GetBoolean(int i);

		/// <summary>Gets the 8-bit unsigned integer value of the specified column.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The 8-bit unsigned integer value of the specified column.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		byte GetByte(int i);

		/// <summary>Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to read the stream of bytes.</param>
		/// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <returns>The actual number of bytes read.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);

		/// <summary>Gets the character value of the specified column.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The character value of the specified column.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		char GetChar(int i);

		/// <summary>Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.</summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to read the stream of bytes.</param>
		/// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <returns>The actual number of characters read.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length);

		/// <summary>Returns the GUID value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The GUID value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		Guid GetGuid(int i);

		/// <summary>Gets the 16-bit signed integer value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The 16-bit signed integer value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		short GetInt16(int i);

		/// <summary>Gets the 32-bit signed integer value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The 32-bit signed integer value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		int GetInt32(int i);

		/// <summary>Gets the 64-bit signed integer value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The 64-bit signed integer value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		long GetInt64(int i);

		/// <summary>Gets the single-precision floating point number of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The single-precision floating point number of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		float GetFloat(int i);

		/// <summary>Gets the double-precision floating point number of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The double-precision floating point number of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		double GetDouble(int i);

		/// <summary>Gets the string value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The string value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		string GetString(int i);

		/// <summary>Gets the fixed-position numeric value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The fixed-position numeric value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		decimal GetDecimal(int i);

		/// <summary>Gets the date and time data value of the specified field.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The date and time data value of the specified field.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		DateTime GetDateTime(int i);

		/// <summary>Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		IDataReader GetData(int i);

		/// <summary>Return whether the specified field is set to null.</summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>
		///   <see langword="true" /> if the specified field is set to null; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount" />.</exception>
		bool IsDBNull(int i);
	}
}
