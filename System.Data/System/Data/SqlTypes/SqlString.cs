using System;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	/// <summary>Represents a variable-length stream of characters to be stored in or retrieved from the database. <see cref="T:System.Data.SqlTypes.SqlString" /> has a different underlying data structure from its corresponding .NET Framework <see cref="T:System.String" /> data type.</summary>
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlString : INullable, IComparable, IXmlSerializable
	{
		private SqlString(bool fNull)
		{
			this.m_value = null;
			this.m_cmpInfo = null;
			this.m_lcid = 0;
			this.m_flag = SqlCompareOptions.None;
			this.m_fNotNull = false;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> class.</summary>
		/// <param name="lcid">Specifies the geographical locale and language for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="compareOptions">Specifies the compare options for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="data">The data array to store.</param>
		/// <param name="index">The starting index within the array.</param>
		/// <param name="count">The number of characters from index to copy.</param>
		/// <param name="fUnicode">
		///   <see langword="true" /> if Unicode encoded. Otherwise, <see langword="false" />.</param>
		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data, int index, int count, bool fUnicode)
		{
			this.m_lcid = lcid;
			SqlString.ValidateSqlCompareOptions(compareOptions);
			this.m_flag = compareOptions;
			if (data == null)
			{
				this.m_fNotNull = false;
				this.m_value = null;
				this.m_cmpInfo = null;
				return;
			}
			this.m_fNotNull = true;
			this.m_cmpInfo = null;
			if (fUnicode)
			{
				this.m_value = SqlString.s_unicodeEncoding.GetString(data, index, count);
				return;
			}
			Encoding encoding = Encoding.GetEncoding(new CultureInfo(this.m_lcid).TextInfo.ANSICodePage);
			this.m_value = encoding.GetString(data, index, count);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> class.</summary>
		/// <param name="lcid">Specifies the geographical locale and language for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="compareOptions">Specifies the compare options for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="data">The data array to store.</param>
		/// <param name="fUnicode">
		///   <see langword="true" /> if Unicode encoded. Otherwise, <see langword="false" />.</param>
		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data, bool fUnicode)
		{
			this = new SqlString(lcid, compareOptions, data, 0, data.Length, fUnicode);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> class.</summary>
		/// <param name="lcid">Specifies the geographical locale and language for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="compareOptions">Specifies the compare options for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="data">The data array to store.</param>
		/// <param name="index">The starting index within the array.</param>
		/// <param name="count">The number of characters from index to copy.</param>
		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data, int index, int count)
		{
			this = new SqlString(lcid, compareOptions, data, index, count, true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> structure using the specified locale id, compare options, and data.</summary>
		/// <param name="lcid">Specifies the geographical locale and language for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="compareOptions">Specifies the compare options for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="data">The data array to store.</param>
		public SqlString(int lcid, SqlCompareOptions compareOptions, byte[] data)
		{
			this = new SqlString(lcid, compareOptions, data, 0, data.Length, true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> structure using the specified string, locale id, and compare option values.</summary>
		/// <param name="data">The string to store.</param>
		/// <param name="lcid">Specifies the geographical locale and language for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <param name="compareOptions">Specifies the compare options for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		public SqlString(string data, int lcid, SqlCompareOptions compareOptions)
		{
			this.m_lcid = lcid;
			SqlString.ValidateSqlCompareOptions(compareOptions);
			this.m_flag = compareOptions;
			this.m_cmpInfo = null;
			if (data == null)
			{
				this.m_fNotNull = false;
				this.m_value = null;
				return;
			}
			this.m_fNotNull = true;
			this.m_value = data;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> structure using the specified string and locale id values.</summary>
		/// <param name="data">The string to store.</param>
		/// <param name="lcid">Specifies the geographical locale and language for the new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		public SqlString(string data, int lcid)
		{
			this = new SqlString(data, lcid, SqlString.s_iDefaultFlag);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> structure using the specified string.</summary>
		/// <param name="data">The string to store.</param>
		public SqlString(string data)
		{
			this = new SqlString(data, CultureInfo.CurrentCulture.LCID, SqlString.s_iDefaultFlag);
		}

		private SqlString(int lcid, SqlCompareOptions compareOptions, string data, CompareInfo cmpInfo)
		{
			this.m_lcid = lcid;
			SqlString.ValidateSqlCompareOptions(compareOptions);
			this.m_flag = compareOptions;
			if (data == null)
			{
				this.m_fNotNull = false;
				this.m_value = null;
				this.m_cmpInfo = null;
				return;
			}
			this.m_value = data;
			this.m_cmpInfo = cmpInfo;
			this.m_fNotNull = true;
		}

		/// <summary>Indicates whether this <see cref="T:System.Data.SqlTypes.SqlString" /> structure is null.</summary>
		/// <returns>
		///   <see langword="true" /> if <see cref="P:System.Data.SqlTypes.SqlString.Value" /> is <see cref="F:System.Data.SqlTypes.SqlString.Null" />. Otherwise, <see langword="false" />.</returns>
		public bool IsNull
		{
			get
			{
				return !this.m_fNotNull;
			}
		}

		/// <summary>Gets the string that is stored in this <see cref="T:System.Data.SqlTypes.SqlString" /> structure. This property is read-only.</summary>
		/// <returns>The string that is stored.</returns>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">The value of the string is <see cref="F:System.Data.SqlTypes.SqlString.Null" />.</exception>
		public string Value
		{
			get
			{
				if (!this.IsNull)
				{
					return this.m_value;
				}
				throw new SqlNullValueException();
			}
		}

		/// <summary>Specifies the geographical locale and language for the <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</summary>
		/// <returns>The locale id for the string stored in the <see cref="P:System.Data.SqlTypes.SqlString.Value" /> property.</returns>
		public int LCID
		{
			get
			{
				if (!this.IsNull)
				{
					return this.m_lcid;
				}
				throw new SqlNullValueException();
			}
		}

		/// <summary>Gets the <see cref="T:System.Globalization.CultureInfo" /> structure that represents information about the culture of this <see cref="T:System.Data.SqlTypes.SqlString" /> object.</summary>
		/// <returns>A <see cref="T:System.Globalization.CultureInfo" /> structure that describes information about the culture of this SqlString structure including the names of the culture, the writing system, and the calendar used, and also access to culture-specific objects that provide methods for common operations, such as formatting dates and sorting strings.</returns>
		public CultureInfo CultureInfo
		{
			get
			{
				if (!this.IsNull)
				{
					return CultureInfo.GetCultureInfo(this.m_lcid);
				}
				throw new SqlNullValueException();
			}
		}

		private void SetCompareInfo()
		{
			if (this.m_cmpInfo == null)
			{
				this.m_cmpInfo = CultureInfo.GetCultureInfo(this.m_lcid).CompareInfo;
			}
		}

		/// <summary>Gets the <see cref="T:System.Globalization.CompareInfo" /> object that defines how string comparisons should be performed for this <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</summary>
		/// <returns>A <see langword="CompareInfo" /> object that defines string comparison for this <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</returns>
		public CompareInfo CompareInfo
		{
			get
			{
				if (!this.IsNull)
				{
					this.SetCompareInfo();
					return this.m_cmpInfo;
				}
				throw new SqlNullValueException();
			}
		}

		/// <summary>A combination of one or more of the <see cref="T:System.Data.SqlTypes.SqlCompareOptions" /> enumeration values that represent the way in which this <see cref="T:System.Data.SqlTypes.SqlString" /> should be compared to other <see cref="T:System.Data.SqlTypes.SqlString" /> structures.</summary>
		/// <returns>A value specifying how this <see cref="T:System.Data.SqlTypes.SqlString" /> should be compared to other <see cref="T:System.Data.SqlTypes.SqlString" /> structures.</returns>
		public SqlCompareOptions SqlCompareOptions
		{
			get
			{
				if (!this.IsNull)
				{
					return this.m_flag;
				}
				throw new SqlNullValueException();
			}
		}

		/// <summary>Converts the <see cref="T:System.String" /> parameter to a <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.String" /> to be converted.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the value of the specified <see langword="String" />.</returns>
		public static implicit operator SqlString(string x)
		{
			return new SqlString(x);
		}

		/// <summary>Converts a <see cref="T:System.Data.SqlTypes.SqlString" /> to a <see cref="T:System.String" /></summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlString" /> to be converted.</param>
		/// <returns>A <see langword="String" />, whose contents are the same as the <see cref="P:System.Data.SqlTypes.SqlString.Value" /> property of the <see cref="T:System.Data.SqlTypes.SqlString" /> parameter.</returns>
		public static explicit operator string(SqlString x)
		{
			return x.Value;
		}

		/// <summary>Converts a <see cref="T:System.Data.SqlTypes.SqlString" /> object to a <see cref="T:System.String" />.</summary>
		/// <returns>A <see cref="T:System.String" /> with the same value as this <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</returns>
		public override string ToString()
		{
			if (!this.IsNull)
			{
				return this.m_value;
			}
			return SQLResource.NullString;
		}

		/// <summary>Gets an array of bytes, that contains the contents of the <see cref="T:System.Data.SqlTypes.SqlString" /> in Unicode format.</summary>
		/// <returns>An byte array, that contains the contents of the <see cref="T:System.Data.SqlTypes.SqlString" /> in Unicode format.</returns>
		public byte[] GetUnicodeBytes()
		{
			if (this.IsNull)
			{
				return null;
			}
			return SqlString.s_unicodeEncoding.GetBytes(this.m_value);
		}

		/// <summary>Gets an array of bytes, that contains the contents of the <see cref="T:System.Data.SqlTypes.SqlString" /> in ANSI format.</summary>
		/// <returns>An byte array, that contains the contents of the <see cref="T:System.Data.SqlTypes.SqlString" /> in ANSI format.</returns>
		public byte[] GetNonUnicodeBytes()
		{
			if (this.IsNull)
			{
				return null;
			}
			return Encoding.GetEncoding(new CultureInfo(this.m_lcid).TextInfo.ANSICodePage).GetBytes(this.m_value);
		}

		/// <summary>Concatenates the two specified <see cref="T:System.Data.SqlTypes.SqlString" /> structures.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the newly concatenated value representing the contents of the two <see cref="T:System.Data.SqlTypes.SqlString" /> parameters.</returns>
		public static SqlString operator +(SqlString x, SqlString y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlString.Null;
			}
			if (x.m_lcid != y.m_lcid || x.m_flag != y.m_flag)
			{
				throw new SqlTypeException(SQLResource.ConcatDiffCollationMessage);
			}
			return new SqlString(x.m_lcid, x.m_flag, x.m_value + y.m_value, (x.m_cmpInfo == null) ? y.m_cmpInfo : x.m_cmpInfo);
		}

		private static int StringCompare(SqlString x, SqlString y)
		{
			if (x.m_lcid != y.m_lcid || x.m_flag != y.m_flag)
			{
				throw new SqlTypeException(SQLResource.CompareDiffCollationMessage);
			}
			x.SetCompareInfo();
			y.SetCompareInfo();
			int result;
			if ((x.m_flag & SqlCompareOptions.BinarySort) != SqlCompareOptions.None)
			{
				result = SqlString.CompareBinary(x, y);
			}
			else if ((x.m_flag & SqlCompareOptions.BinarySort2) != SqlCompareOptions.None)
			{
				result = SqlString.CompareBinary2(x, y);
			}
			else
			{
				string value = x.m_value;
				string value2 = y.m_value;
				int i = value.Length;
				int num = value2.Length;
				while (i > 0)
				{
					if (value[i - 1] != ' ')
					{
						break;
					}
					i--;
				}
				while (num > 0 && value2[num - 1] == ' ')
				{
					num--;
				}
				CompareOptions options = SqlString.CompareOptionsFromSqlCompareOptions(x.m_flag);
				result = x.m_cmpInfo.Compare(x.m_value, 0, i, y.m_value, 0, num, options);
			}
			return result;
		}

		private static SqlBoolean Compare(SqlString x, SqlString y, EComparison ecExpectedResult)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			int num = SqlString.StringCompare(x, y);
			bool value;
			switch (ecExpectedResult)
			{
			case EComparison.LT:
				value = (num < 0);
				break;
			case EComparison.LE:
				value = (num <= 0);
				break;
			case EComparison.EQ:
				value = (num == 0);
				break;
			case EComparison.GE:
				value = (num >= 0);
				break;
			case EComparison.GT:
				value = (num > 0);
				break;
			default:
				return SqlBoolean.Null;
			}
			return new SqlBoolean(value);
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlBoolean" /> structure to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlBoolean" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> parameter.</returns>
		public static explicit operator SqlString(SqlBoolean x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.Value.ToString());
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlByte" /> structure to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlByte" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> object that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlByte" /> parameter.</returns>
		public static explicit operator SqlString(SqlByte x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.Value.ToString(null));
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlInt16" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlInt16" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> object that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlInt16" /> parameter.</returns>
		public static explicit operator SqlString(SqlInt16 x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.Value.ToString(null));
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlInt32" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The SqlInt32 structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> object that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlInt32" /> parameter.</returns>
		public static explicit operator SqlString(SqlInt32 x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.Value.ToString(null));
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlInt64" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlInt64" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> object that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlInt64" /> parameter.</returns>
		public static explicit operator SqlString(SqlInt64 x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.Value.ToString(null));
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameter.</returns>
		public static explicit operator SqlString(SqlSingle x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.Value.ToString(null));
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlDouble" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> parameter.</returns>
		public static explicit operator SqlString(SqlDouble x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.Value.ToString(null));
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlDecimal" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see langword="SqlDecimal" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the string representation of the <see langword="SqlDecimal" /> parameter.</returns>
		public static explicit operator SqlString(SqlDecimal x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.ToString());
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlMoney" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlMoney" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlMoney" /> parameter.</returns>
		public static explicit operator SqlString(SqlMoney x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.ToString());
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlDateTime" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlDateTime" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlDateTime" /> parameter.</returns>
		public static explicit operator SqlString(SqlDateTime x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.ToString());
			}
			return SqlString.Null;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlGuid" /> parameter to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure to be converted.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlString" /> whose value is the string representation of the specified <see cref="T:System.Data.SqlTypes.SqlGuid" />.</returns>
		public static explicit operator SqlString(SqlGuid x)
		{
			if (!x.IsNull)
			{
				return new SqlString(x.ToString());
			}
			return SqlString.Null;
		}

		/// <summary>Creates a copy of this <see cref="T:System.Data.SqlTypes.SqlString" /> object.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlString" /> object in which all property values are the same as the original.</returns>
		public SqlString Clone()
		{
			if (this.IsNull)
			{
				return new SqlString(true);
			}
			return new SqlString(this.m_value, this.m_lcid, this.m_flag);
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are not equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator ==(SqlString x, SqlString y)
		{
			return SqlString.Compare(x, y, EComparison.EQ);
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether they are not equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator !=(SqlString x, SqlString y)
		{
			return !(x == y);
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <(SqlString x, SqlString y)
		{
			return SqlString.Compare(x, y, EComparison.LT);
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >(SqlString x, SqlString y)
		{
			return SqlString.Compare(x, y, EComparison.GT);
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <=(SqlString x, SqlString y)
		{
			return SqlString.Compare(x, y, EComparison.LE);
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is greater than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >=(SqlString x, SqlString y)
		{
			return SqlString.Compare(x, y, EComparison.GE);
		}

		/// <summary>Concatenates the two specified <see cref="T:System.Data.SqlTypes.SqlString" /> structures.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlString" /> that contains the newly concatenated value representing the contents of the two <see cref="T:System.Data.SqlTypes.SqlString" /> parameters.</returns>
		public static SqlString Concat(SqlString x, SqlString y)
		{
			return x + y;
		}

		/// <summary>Concatenates two specified <see cref="T:System.Data.SqlTypes.SqlString" /> values to create a new <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlString" /> that is the concatenated value of <paramref name="x" /> and <paramref name="y" />.</returns>
		public static SqlString Add(SqlString x, SqlString y)
		{
			return x + y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>
		///   <see langword="true" /> if the two values are equal. Otherwise, <see langword="false" />. If either instance is null, then the <see langword="SqlString" /> will be null.</returns>
		public static SqlBoolean Equals(SqlString x, SqlString y)
		{
			return x == y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether they are not equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean NotEquals(SqlString x, SqlString y)
		{
			return x != y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThan(SqlString x, SqlString y)
		{
			return x < y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThan(SqlString x, SqlString y)
		{
			return x > y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThanOrEqual(SqlString x, SqlString y)
		{
			return x <= y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlString" /> operands to determine whether the first is greater than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlString" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlString" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThanOrEqual(SqlString x, SqlString y)
		{
			return x >= y;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlBoolean" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Data.SqlTypes.SqlString.Value" /> is non-zero; <see langword="false" /> if zero; otherwise Null.</returns>
		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlByte" />.</summary>
		/// <returns>A new <see langword="SqlByte" /> structure whose <see cref="P:System.Data.SqlTypes.SqlByte.Value" /> equals the number represented by this <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</returns>
		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlDateTime" />.</summary>
		/// <returns>A new <see langword="SqlDateTime" /> structure that contains the date value represented by this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlDateTime ToSqlDateTime()
		{
			return (SqlDateTime)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> that is equal to the numeric value of this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlDouble ToSqlDouble()
		{
			return (SqlDouble)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt16" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt16" /> that is equal to the numeric value of this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt32" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt32" /> that is equal to the numeric value of this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt64" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt64" /> that is equal to the numeric value of this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlMoney" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlMoney" /> that is equal to the numeric value of this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlMoney ToSqlMoney()
		{
			return (SqlMoney)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlDecimal" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDecimal" /> that contains the value of this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlDecimal ToSqlDecimal()
		{
			return (SqlDecimal)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> that is equal to the numeric value of this <see cref="T:System.Data.SqlTypes.SqlString" />.</returns>
		public SqlSingle ToSqlSingle()
		{
			return (SqlSingle)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlGuid" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure whose <see cref="P:System.Data.SqlTypes.SqlGuid.Value" /> is the <see langword="Guid" /> represented by this <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</returns>
		public SqlGuid ToSqlGuid()
		{
			return (SqlGuid)this;
		}

		private static void ValidateSqlCompareOptions(SqlCompareOptions compareOptions)
		{
			if ((compareOptions & SqlString.s_iValidSqlCompareOptionMask) != compareOptions)
			{
				throw new ArgumentOutOfRangeException("compareOptions");
			}
		}

		/// <summary>Gets the <see cref="T:System.Globalization.CompareOptions" /> enumeration equilvalent of the specified <see cref="T:System.Data.SqlTypes.SqlCompareOptions" /> value.</summary>
		/// <param name="compareOptions">A <see cref="T:System.Data.SqlTypes.SqlCompareOptions" /> value that describes the comparison options for this <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</param>
		/// <returns>A <see langword="CompareOptions" /> value that corresponds to the <see langword="SqlCompareOptions" /> for this <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</returns>
		public static CompareOptions CompareOptionsFromSqlCompareOptions(SqlCompareOptions compareOptions)
		{
			CompareOptions compareOptions2 = CompareOptions.None;
			SqlString.ValidateSqlCompareOptions(compareOptions);
			if ((compareOptions & (SqlCompareOptions.BinarySort | SqlCompareOptions.BinarySort2)) != SqlCompareOptions.None)
			{
				throw ADP.ArgumentOutOfRange("compareOptions");
			}
			if ((compareOptions & SqlCompareOptions.IgnoreCase) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreCase;
			}
			if ((compareOptions & SqlCompareOptions.IgnoreNonSpace) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreNonSpace;
			}
			if ((compareOptions & SqlCompareOptions.IgnoreKanaType) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreKanaType;
			}
			if ((compareOptions & SqlCompareOptions.IgnoreWidth) != SqlCompareOptions.None)
			{
				compareOptions2 |= CompareOptions.IgnoreWidth;
			}
			return compareOptions2;
		}

		private bool FBinarySort()
		{
			return !this.IsNull && (this.m_flag & (SqlCompareOptions.BinarySort | SqlCompareOptions.BinarySort2)) > SqlCompareOptions.None;
		}

		private static int CompareBinary(SqlString x, SqlString y)
		{
			byte[] bytes = SqlString.s_unicodeEncoding.GetBytes(x.m_value);
			byte[] bytes2 = SqlString.s_unicodeEncoding.GetBytes(y.m_value);
			int num = bytes.Length;
			int num2 = bytes2.Length;
			int num3 = (num < num2) ? num : num2;
			int i;
			for (i = 0; i < num3; i++)
			{
				if (bytes[i] < bytes2[i])
				{
					return -1;
				}
				if (bytes[i] > bytes2[i])
				{
					return 1;
				}
			}
			i = num3;
			int num4 = 32;
			if (num < num2)
			{
				while (i < num2)
				{
					int num5 = (int)bytes2[i + 1] << (int)(8 + bytes2[i]);
					if (num5 != num4)
					{
						if (num4 <= num5)
						{
							return -1;
						}
						return 1;
					}
					else
					{
						i += 2;
					}
				}
			}
			else
			{
				while (i < num)
				{
					int num5 = (int)bytes[i + 1] << (int)(8 + bytes[i]);
					if (num5 != num4)
					{
						if (num5 <= num4)
						{
							return -1;
						}
						return 1;
					}
					else
					{
						i += 2;
					}
				}
			}
			return 0;
		}

		private static int CompareBinary2(SqlString x, SqlString y)
		{
			string value = x.m_value;
			string value2 = y.m_value;
			int length = value.Length;
			int length2 = value2.Length;
			int num = (length < length2) ? length : length2;
			for (int i = 0; i < num; i++)
			{
				if (value[i] < value2[i])
				{
					return -1;
				}
				if (value[i] > value2[i])
				{
					return 1;
				}
			}
			char c = ' ';
			if (length < length2)
			{
				int i = num;
				while (i < length2)
				{
					if (value2[i] != c)
					{
						if (c <= value2[i])
						{
							return -1;
						}
						return 1;
					}
					else
					{
						i++;
					}
				}
			}
			else
			{
				int i = num;
				while (i < length)
				{
					if (value[i] != c)
					{
						if (value[i] <= c)
						{
							return -1;
						}
						return 1;
					}
					else
					{
						i++;
					}
				}
			}
			return 0;
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlString" /> object to the supplied <see cref="T:System.Object" /> and returns an indication of their relative values.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to be compared.</param>
		/// <returns>A signed number that indicates the relative values of the instance and the object.  
		///   Return Value  
		///
		///   Condition  
		///
		///   Less than zero  
		///
		///   This instance is less than the object.  
		///
		///   Zero  
		///
		///   This instance is the same as the object.  
		///
		///   Greater than zero  
		///
		///   This instance is greater than the object  
		///
		///  -or-  
		///
		///  The object is a null reference (<see langword="Nothing" /> in Visual Basic)</returns>
		public int CompareTo(object value)
		{
			if (value is SqlString)
			{
				SqlString value2 = (SqlString)value;
				return this.CompareTo(value2);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlString));
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlString" /> instance to the supplied <see cref="T:System.Data.SqlTypes.SqlString" /> and returns an indication of their relative values.</summary>
		/// <param name="value">The <see cref="T:System.Data.SqlTypes.SqlString" /> to be compared.</param>
		/// <returns>A signed number that indicates the relative values of the instance and the object.  
		///   Return value  
		///
		///   Condition  
		///
		///   Less than zero  
		///
		///   This instance is less than the object.  
		///
		///   Zero  
		///
		///   This instance is the same as the object.  
		///
		///   Greater than zero  
		///
		///   This instance is greater than the object  
		///
		///  -or-  
		///
		///  The object is a null reference (<see langword="Nothing" /> in Visual Basic).</returns>
		public int CompareTo(SqlString value)
		{
			if (this.IsNull)
			{
				if (!value.IsNull)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (value.IsNull)
				{
					return 1;
				}
				int num = SqlString.StringCompare(this, value);
				if (num < 0)
				{
					return -1;
				}
				if (num > 0)
				{
					return 1;
				}
				return 0;
			}
		}

		/// <summary>Compares the supplied object parameter to the <see cref="P:System.Data.SqlTypes.SqlString.Value" /> property of the <see cref="T:System.Data.SqlTypes.SqlString" /> object.</summary>
		/// <param name="value">The object to be compared.</param>
		/// <returns>
		///   <see langword="true" /> if the object is an instance of <see cref="T:System.Data.SqlTypes.SqlString" /> and the two are equal; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object value)
		{
			if (!(value is SqlString))
			{
				return false;
			}
			SqlString y = (SqlString)value;
			if (y.IsNull || this.IsNull)
			{
				return y.IsNull && this.IsNull;
			}
			return (this == y).Value;
		}

		/// <summary>Gets the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			if (this.IsNull)
			{
				return 0;
			}
			byte[] array;
			if (this.FBinarySort())
			{
				array = SqlString.s_unicodeEncoding.GetBytes(this.m_value.TrimEnd());
			}
			else
			{
				CompareInfo compareInfo;
				CompareOptions options;
				try
				{
					this.SetCompareInfo();
					compareInfo = this.m_cmpInfo;
					options = SqlString.CompareOptionsFromSqlCompareOptions(this.m_flag);
				}
				catch (ArgumentException)
				{
					compareInfo = CultureInfo.InvariantCulture.CompareInfo;
					options = CompareOptions.None;
				}
				array = compareInfo.GetSortKey(this.m_value.TrimEnd(), options).KeyData;
			}
			return SqlBinary.HashByteArray(array, array.Length);
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <returns>An <see langword="XmlSchema" />.</returns>
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <param name="reader">
		///   <see langword="XmlReader" />
		/// </param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string attribute = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute != null && XmlConvert.ToBoolean(attribute))
			{
				reader.ReadElementString();
				this.m_fNotNull = false;
				return;
			}
			this.m_value = reader.ReadElementString();
			this.m_fNotNull = true;
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <param name="writer">
		///   <see langword="XmlWriter" />
		/// </param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(this.m_value);
		}

		/// <summary>Returns the XML Schema definition language (XSD) of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</summary>
		/// <param name="schemaSet">A <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</param>
		/// <returns>A <see langword="string" /> value that indicates the XSD of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</returns>
		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		private string m_value;

		private CompareInfo m_cmpInfo;

		private int m_lcid;

		private SqlCompareOptions m_flag;

		private bool m_fNotNull;

		/// <summary>Represents a <see cref="T:System.DBNull" /> that can be assigned to this instance of the <see cref="T:System.Data.SqlTypes.SqlString" /> structure.</summary>
		public static readonly SqlString Null = new SqlString(true);

		internal static readonly UnicodeEncoding s_unicodeEncoding = new UnicodeEncoding();

		/// <summary>Specifies that <see cref="T:System.Data.SqlTypes.SqlString" /> comparisons should ignore case.</summary>
		public static readonly int IgnoreCase = 1;

		/// <summary>Specifies that the string comparison must ignore the character width.</summary>
		public static readonly int IgnoreWidth = 16;

		/// <summary>Specifies that the string comparison must ignore non-space combining characters, such as diacritics.</summary>
		public static readonly int IgnoreNonSpace = 2;

		/// <summary>Specifies that the string comparison must ignore the Kana type.</summary>
		public static readonly int IgnoreKanaType = 8;

		/// <summary>Specifies that sorts should be based on a characters numeric value instead of its alphabetical value.</summary>
		public static readonly int BinarySort = 32768;

		/// <summary>Specifies that sorts should be based on a character's numeric value instead of its alphabetical value.</summary>
		public static readonly int BinarySort2 = 16384;

		private static readonly SqlCompareOptions s_iDefaultFlag = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth;

		private static readonly CompareOptions s_iValidCompareOptionMask = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth;

		internal static readonly SqlCompareOptions s_iValidSqlCompareOptionMask = SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreNonSpace | SqlCompareOptions.IgnoreKanaType | SqlCompareOptions.IgnoreWidth | SqlCompareOptions.BinarySort | SqlCompareOptions.BinarySort2;

		internal static readonly int s_lcidUSEnglish = 1033;

		private static readonly int s_lcidBinary = 33280;
	}
}
