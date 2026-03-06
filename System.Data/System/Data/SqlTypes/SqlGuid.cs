using System;
using System.Data.Common;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	/// <summary>Represents a GUID to be stored in or retrieved from a database.</summary>
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlGuid : INullable, IComparable, IXmlSerializable
	{
		private SqlGuid(bool fNull)
		{
			this.m_value = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure using the supplied byte array parameter.</summary>
		/// <param name="value">A byte array.</param>
		public SqlGuid(byte[] value)
		{
			if (value == null || value.Length != SqlGuid.s_sizeOfGuid)
			{
				throw new ArgumentException(SQLResource.InvalidArraySizeMessage);
			}
			this.m_value = new byte[SqlGuid.s_sizeOfGuid];
			value.CopyTo(this.m_value, 0);
		}

		internal SqlGuid(byte[] value, bool ignored)
		{
			if (value == null || value.Length != SqlGuid.s_sizeOfGuid)
			{
				throw new ArgumentException(SQLResource.InvalidArraySizeMessage);
			}
			this.m_value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure using the specified <see cref="T:System.String" /> parameter.</summary>
		/// <param name="s">A <see cref="T:System.String" /> object.</param>
		public SqlGuid(string s)
		{
			this.m_value = new Guid(s).ToByteArray();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure using the specified <see cref="T:System.Guid" /> parameter.</summary>
		/// <param name="g">A <see cref="T:System.Guid" /></param>
		public SqlGuid(Guid g)
		{
			this.m_value = g.ToByteArray();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure using the specified values.</summary>
		/// <param name="a">The first four bytes of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="b">The next two bytes of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="c">The next two bytes of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="d">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="e">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="f">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="g">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="h">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="i">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="j">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		/// <param name="k">The next byte of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</param>
		public SqlGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			this = new SqlGuid(new Guid(a, b, c, d, e, f, g, h, i, j, k));
		}

		/// <summary>Gets a Boolean value that indicates whether this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure is null.</summary>
		/// <returns>
		///   <see langword="true" /> if <see langword="null" />. Otherwise, <see langword="false" />.</returns>
		public bool IsNull
		{
			get
			{
				return this.m_value == null;
			}
		}

		/// <summary>Gets the value of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure. This property is read-only.</summary>
		/// <returns>A <see cref="T:System.Guid" /> structure.</returns>
		public Guid Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return new Guid(this.m_value);
			}
		}

		/// <summary>Converts the supplied <see cref="T:System.Guid" /> parameter to <see cref="T:System.Data.SqlTypes.SqlGuid" />.</summary>
		/// <param name="x">A <see cref="T:System.Guid" />.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlGuid" /> whose <see cref="P:System.Data.SqlTypes.SqlGuid.Value" /> is equal to the <see cref="T:System.Guid" /> parameter.</returns>
		public static implicit operator SqlGuid(Guid x)
		{
			return new SqlGuid(x);
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlGuid" /> parameter to <see cref="T:System.Guid" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A new <see cref="T:System.Guid" /> equal to the <see cref="P:System.Data.SqlTypes.SqlGuid.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlGuid" />.</returns>
		public static explicit operator Guid(SqlGuid x)
		{
			return x.Value;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure to a byte array.</summary>
		/// <returns>An array of bytes representing the <see cref="P:System.Data.SqlTypes.SqlGuid.Value" /> of this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</returns>
		public byte[] ToByteArray()
		{
			byte[] array = new byte[SqlGuid.s_sizeOfGuid];
			this.m_value.CopyTo(array, 0);
			return array;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure to a <see cref="T:System.String" />.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</returns>
		public override string ToString()
		{
			if (this.IsNull)
			{
				return SQLResource.NullString;
			}
			Guid guid = new Guid(this.m_value);
			return guid.ToString();
		}

		/// <summary>Converts the specified <see cref="T:System.String" /> structure to <see cref="T:System.Data.SqlTypes.SqlGuid" />.</summary>
		/// <param name="s">The <see langword="String" /> to be parsed.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlGuid" /> equivalent to the value that is contained in the specified <see cref="T:System.String" />.</returns>
		public static SqlGuid Parse(string s)
		{
			if (s == SQLResource.NullString)
			{
				return SqlGuid.Null;
			}
			return new SqlGuid(s);
		}

		private static EComparison Compare(SqlGuid x, SqlGuid y)
		{
			int i = 0;
			while (i < SqlGuid.s_sizeOfGuid)
			{
				byte b = x.m_value[SqlGuid.s_rgiGuidOrder[i]];
				byte b2 = y.m_value[SqlGuid.s_rgiGuidOrder[i]];
				if (b != b2)
				{
					if (b >= b2)
					{
						return EComparison.GT;
					}
					return EComparison.LT;
				}
				else
				{
					i++;
				}
			}
			return EComparison.EQ;
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlString" /> structure to <see cref="T:System.Data.SqlTypes.SqlGuid" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" /> object.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlGuid" /> whose <see cref="P:System.Data.SqlTypes.SqlGuid.Value" /> equals the value represented by the <see cref="T:System.Data.SqlTypes.SqlString" /> parameter.</returns>
		public static explicit operator SqlGuid(SqlString x)
		{
			if (!x.IsNull)
			{
				return new SqlGuid(x.Value);
			}
			return SqlGuid.Null;
		}

		/// <summary>Converts the <see cref="T:System.Data.SqlTypes.SqlBinary" /> parameter to <see cref="T:System.Data.SqlTypes.SqlGuid" />.</summary>
		/// <param name="x">A <see langword="SqlBinary" /> object.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlGuid" /> whose <see cref="P:System.Data.SqlTypes.SqlGuid.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlBinary.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBinary" /> parameter.</returns>
		public static explicit operator SqlGuid(SqlBinary x)
		{
			if (!x.IsNull)
			{
				return new SqlGuid(x.Value);
			}
			return SqlGuid.Null;
		}

		/// <summary>Performs a logical comparison of two <see cref="T:System.Data.SqlTypes.SqlGuid" /> structures to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are not equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator ==(SqlGuid x, SqlGuid y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(SqlGuid.Compare(x, y) == EComparison.EQ);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Performs a logical comparison on two <see cref="T:System.Data.SqlTypes.SqlGuid" /> structures to determine whether they are not equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator !=(SqlGuid x, SqlGuid y)
		{
			return !(x == y);
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <(SqlGuid x, SqlGuid y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(SqlGuid.Compare(x, y) == EComparison.LT);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >(SqlGuid x, SqlGuid y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(SqlGuid.Compare(x, y) == EComparison.GT);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <=(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = SqlGuid.Compare(x, y);
			return new SqlBoolean(ecomparison == EComparison.LT || ecomparison == EComparison.EQ);
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is greater than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >=(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = SqlGuid.Compare(x, y);
			return new SqlBoolean(ecomparison == EComparison.GT || ecomparison == EComparison.EQ);
		}

		/// <summary>Performs a logical comparison of two <see cref="T:System.Data.SqlTypes.SqlGuid" /> structures to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>
		///   <see langword="true" /> if the two values are equal. Otherwise, <see langword="false" />. If either instance is null, then the <see langword="SqlGuid" /> will be null.</returns>
		public static SqlBoolean Equals(SqlGuid x, SqlGuid y)
		{
			return x == y;
		}

		/// <summary>Performs a logical comparison on two <see cref="T:System.Data.SqlTypes.SqlGuid" /> structures to determine whether they are not equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean NotEquals(SqlGuid x, SqlGuid y)
		{
			return x != y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThan(SqlGuid x, SqlGuid y)
		{
			return x < y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThan(SqlGuid x, SqlGuid y)
		{
			return x > y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThanOrEqual(SqlGuid x, SqlGuid y)
		{
			return x <= y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlGuid" /> to determine whether the first is greater than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThanOrEqual(SqlGuid x, SqlGuid y)
		{
			return x >= y;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlString" /> structure that contains the string representation of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</returns>
		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure to <see cref="T:System.Data.SqlTypes.SqlBinary" />.</summary>
		/// <returns>A <see langword="SqlBinary" /> structure that contains the bytes in the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</returns>
		public SqlBinary ToSqlBinary()
		{
			return (SqlBinary)this;
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure to the supplied object and returns an indication of their relative values. Compares more than the last 6 bytes, but treats the last 6 bytes as the most significant ones in comparisons.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to be compared.</param>
		/// <returns>A signed number that indicates the relative values of the instance and the object.  
		///   Return Value  
		///
		///   Condition  
		///
		///   Less than zero  
		///
		///   This instance is less than object.  
		///
		///   Zero  
		///
		///   This instance is the same as object.  
		///
		///   Greater than zero  
		///
		///   This instance is greater than object  
		///
		///  -or-  
		///
		///  object is a null reference (<see langword="Nothing" />)</returns>
		public int CompareTo(object value)
		{
			if (value is SqlGuid)
			{
				SqlGuid value2 = (SqlGuid)value;
				return this.CompareTo(value2);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlGuid));
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure to the supplied <see cref="T:System.Data.SqlTypes.SqlGuid" /> and returns an indication of their relative values. Compares more than the last 6 bytes, but treats the last 6 bytes as the most significant ones in comparisons.</summary>
		/// <param name="value">The <see cref="T:System.Data.SqlTypes.SqlGuid" /> to be compared.</param>
		/// <returns>A signed number that indicates the relative values of the instance and the object.  
		///   Return Value  
		///
		///   Condition  
		///
		///   Less than zero  
		///
		///   This instance is less than object.  
		///
		///   Zero  
		///
		///   This instance is the same as object.  
		///
		///   Greater than zero  
		///
		///   This instance is greater than object  
		///
		///  -or-  
		///
		///  object is a null reference (<see langword="Nothing" />).</returns>
		public int CompareTo(SqlGuid value)
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
				if (this < value)
				{
					return -1;
				}
				if (this > value)
				{
					return 1;
				}
				return 0;
			}
		}

		/// <summary>Compares the supplied object parameter to the <see cref="P:System.Data.SqlTypes.SqlGuid.Value" /> property of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> object.</summary>
		/// <param name="value">The object to be compared.</param>
		/// <returns>
		///   <see langword="true" /> if object is an instance of <see cref="T:System.Data.SqlTypes.SqlGuid" /> and the two are equal; otherwise <see langword="false" />.</returns>
		public override bool Equals(object value)
		{
			if (!(value is SqlGuid))
			{
				return false;
			}
			SqlGuid y = (SqlGuid)value;
			if (y.IsNull || this.IsNull)
			{
				return y.IsNull && this.IsNull;
			}
			return (this == y).Value;
		}

		/// <summary>Returns the hash code of this <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			if (!this.IsNull)
			{
				return this.Value.GetHashCode();
			}
			return 0;
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
				this.m_value = null;
				return;
			}
			this.m_value = new Guid(reader.ReadElementString()).ToByteArray();
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
			writer.WriteString(XmlConvert.ToString(new Guid(this.m_value)));
		}

		/// <summary>Returns the XML Schema definition language (XSD) of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</summary>
		/// <param name="schemaSet">A <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</param>
		/// <returns>A <see langword="string" /> value that indicates the XSD of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</returns>
		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		private static readonly int s_sizeOfGuid = 16;

		private static readonly int[] s_rgiGuidOrder = new int[]
		{
			10,
			11,
			12,
			13,
			14,
			15,
			8,
			9,
			6,
			7,
			4,
			5,
			0,
			1,
			2,
			3
		};

		private byte[] m_value;

		/// <summary>Represents a <see cref="T:System.DBNull" /> that can be assigned to this instance of the <see cref="T:System.Data.SqlTypes.SqlGuid" /> structure.</summary>
		public static readonly SqlGuid Null = new SqlGuid(true);
	}
}
