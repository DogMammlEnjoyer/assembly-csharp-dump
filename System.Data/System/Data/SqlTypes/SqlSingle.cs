using System;
using System.Data.Common;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	/// <summary>Represents a floating point number within the range of -3.40E +38 through 3.40E +38 to be stored in or retrieved from a database.</summary>
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlSingle : INullable, IComparable, IXmlSerializable
	{
		private SqlSingle(bool fNull)
		{
			this._fNotNull = false;
			this._value = 0f;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</summary>
		/// <param name="value">A floating point number which will be used as the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> of the new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		public SqlSingle(float value)
		{
			if (!float.IsFinite(value))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			this._fNotNull = true;
			this._value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure using the supplied double parameter.</summary>
		/// <param name="value">A double value which will be used as the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> of the new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		public SqlSingle(double value)
		{
			this = new SqlSingle((float)value);
		}

		/// <summary>Indicates whether this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure is null.</summary>
		/// <returns>
		///   <see langword="true" /> if null. Otherwise, <see langword="false" />.</returns>
		public bool IsNull
		{
			get
			{
				return !this._fNotNull;
			}
		}

		/// <summary>Gets the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure. This property is read-only.</summary>
		/// <returns>A floating point value in the range -3.40E+38 through 3.40E+38.</returns>
		public float Value
		{
			get
			{
				if (this._fNotNull)
				{
					return this._value;
				}
				throw new SqlNullValueException();
			}
		}

		/// <summary>Converts the specified floating point value to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The float value to be converted to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the value of the specified float.</returns>
		public static implicit operator SqlSingle(float x)
		{
			return new SqlSingle(x);
		}

		/// <summary>Converts the specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to float.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlSingle" /> value to be converted to float.</param>
		/// <returns>A float that contains the value of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</returns>
		public static explicit operator float(SqlSingle x)
		{
			return x.Value;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.String" />.</summary>
		/// <returns>A <see langword="String" /> object representing the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public override string ToString()
		{
			if (!this.IsNull)
			{
				return this._value.ToString(null);
			}
			return SQLResource.NullString;
		}

		/// <summary>Converts the specified <see cref="T:System.String" /> to a <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</summary>
		/// <param name="s">The <see cref="T:System.String" /> to be parsed.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> equivalent to the value that is contained in the specified <see cref="T:System.String" />.</returns>
		public static SqlSingle Parse(string s)
		{
			if (s == SQLResource.NullString)
			{
				return SqlSingle.Null;
			}
			return new SqlSingle(float.Parse(s, CultureInfo.InvariantCulture));
		}

		/// <summary>Negates the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> of the specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the negated value.</returns>
		public static SqlSingle operator -(SqlSingle x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle(-x._value);
			}
			return SqlSingle.Null;
		}

		/// <summary>Computes the sum of the two specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the sum of the two specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures.</returns>
		public static SqlSingle operator +(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlSingle.Null;
			}
			float num = x._value + y._value;
			if (float.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlSingle(num);
		}

		/// <summary>Subtracts the second <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure from the first.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the results of the subtraction.</returns>
		public static SqlSingle operator -(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlSingle.Null;
			}
			float num = x._value - y._value;
			if (float.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlSingle(num);
		}

		/// <summary>Computes the product of the two specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the product of the multiplication.</returns>
		public static SqlSingle operator *(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlSingle.Null;
			}
			float num = x._value * y._value;
			if (float.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlSingle(num);
		}

		/// <summary>Divides the first <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure by the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the results of the division.</returns>
		public static SqlSingle operator /(SqlSingle x, SqlSingle y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlSingle.Null;
			}
			if (y._value == 0f)
			{
				throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
			}
			float num = x._value / y._value;
			if (float.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlSingle(num);
		}

		/// <summary>This implicit operator converts the supplied <see cref="T:System.Data.SqlTypes.SqlBoolean" /> to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlBoolean" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlBoolean.ByteValue" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> parameter.</returns>
		public static explicit operator SqlSingle(SqlBoolean x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle((float)x.ByteValue);
			}
			return SqlSingle.Null;
		}

		/// <summary>This implicit operator converts the <see cref="T:System.Data.SqlTypes.SqlByte" /> parameter to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlByte" /> to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> property equals the <see cref="P:System.Data.SqlTypes.SqlByte.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlByte" /> parameter.</returns>
		public static implicit operator SqlSingle(SqlByte x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle((float)x.Value);
			}
			return SqlSingle.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlInt16" /> parameter to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlInt16" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlInt16.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlInt16" /> parameter.</returns>
		public static implicit operator SqlSingle(SqlInt16 x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle((float)x.Value);
			}
			return SqlSingle.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlInt32" /> structure to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlInt32" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlInt32.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlInt32" /> parameter.</returns>
		public static implicit operator SqlSingle(SqlInt32 x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle((float)x.Value);
			}
			return SqlSingle.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlInt64" /> parameter to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlInt64" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlInt64.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlInt64" /> parameter.</returns>
		public static implicit operator SqlSingle(SqlInt64 x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle((float)x.Value);
			}
			return SqlSingle.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlMoney" /> structure to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlMoney" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlMoney.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlMoney" /> parameter.</returns>
		public static implicit operator SqlSingle(SqlMoney x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle(x.ToDouble());
			}
			return SqlSingle.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlDecimal" /> parameter to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlDecimal" /> structure to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlDecimal.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlDecimal" /> parameter.</returns>
		public static implicit operator SqlSingle(SqlDecimal x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle(x.ToDouble());
			}
			return SqlSingle.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlDouble" /> parameter to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlDouble" /> parameter to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> parameter.</returns>
		public static explicit operator SqlSingle(SqlDouble x)
		{
			if (!x.IsNull)
			{
				return new SqlSingle(x.Value);
			}
			return SqlSingle.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlString" /> parameter to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlString" /> object to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the value represented by the <see cref="T:System.Data.SqlTypes.SqlString" /> parameter.</returns>
		public static explicit operator SqlSingle(SqlString x)
		{
			if (x.IsNull)
			{
				return SqlSingle.Null;
			}
			return SqlSingle.Parse(x.Value);
		}

		/// <summary>Performs a logical comparison of the two SqlSingle parameters to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are not equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator ==(SqlSingle x, SqlSingle y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value == y._value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameters to determine whether they are not equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator !=(SqlSingle x, SqlSingle y)
		{
			return !(x == y);
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameters to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <(SqlSingle x, SqlSingle y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value < y._value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> operands to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >(SqlSingle x, SqlSingle y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value > y._value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameters to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <=(SqlSingle x, SqlSingle y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value <= y._value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Performs a logical comparison of two <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures to determine whether the first is greater than or equl to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >=(SqlSingle x, SqlSingle y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value >= y._value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Computes the sum of the two specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the sum of the two specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures.</returns>
		public static SqlSingle Add(SqlSingle x, SqlSingle y)
		{
			return x + y;
		}

		/// <summary>Subtracts the second <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure from the first.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the results of the subtraction.</returns>
		public static SqlSingle Subtract(SqlSingle x, SqlSingle y)
		{
			return x - y;
		}

		/// <summary>Computes the product of the two specified <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure that contains the product of the multiplication.</returns>
		public static SqlSingle Multiply(SqlSingle x, SqlSingle y)
		{
			return x * y;
		}

		/// <summary>Divides the first <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure by the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see langword="SqlInt64" /> structure that contains the results of the division.</returns>
		public static SqlSingle Divide(SqlSingle x, SqlSingle y)
		{
			return x / y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameters to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>
		///   <see langword="true" /> if the two values are equal. Otherwise, <see langword="false" />. If either instance is null, then the <see langword="SqlSingle" /> will be null.</returns>
		public static SqlBoolean Equals(SqlSingle x, SqlSingle y)
		{
			return x == y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameters to determine whether they are not equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean NotEquals(SqlSingle x, SqlSingle y)
		{
			return x != y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameters to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThan(SqlSingle x, SqlSingle y)
		{
			return x < y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> operands to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThan(SqlSingle x, SqlSingle y)
		{
			return x > y;
		}

		/// <summary>Performs a logical comparison of the two <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameters to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThanOrEqual(SqlSingle x, SqlSingle y)
		{
			return x <= y;
		}

		/// <summary>Performs a logical comparison of two <see cref="T:System.Data.SqlTypes.SqlSingle" /> structures to determine whether the first is greater than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThanOrEqual(SqlSingle x, SqlSingle y)
		{
			return x >= y;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlBoolean" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is non-zero; <see langword="false" /> if zero; otherwise Null.</returns>
		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlByte" />.</summary>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlByte" /> structure whose <see langword="Value" /> equals the <see langword="Value" /> of this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure. If the <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure's Value is <see langword="true" />, the <see cref="T:System.Data.SqlTypes.SqlByte" /> structure's <see langword="Value" /> will be 1. Otherwise, the <see cref="T:System.Data.SqlTypes.SqlByte" /> structure's <see langword="Value" /> will be 0.</returns>
		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <returns>A new <see langword="SqlDouble" /> equal to the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public SqlDouble ToSqlDouble()
		{
			return this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt16" />.</summary>
		/// <returns>A new <see langword="SqlInt16" /> equal to the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt32" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt32" /> equal to the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt64" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt64" /> equal to the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlMoney" />.</summary>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlMoney" /> equal to the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public SqlMoney ToSqlMoney()
		{
			return (SqlMoney)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlDecimal" />.</summary>
		/// <returns>A new <see langword="SqlDecimal" /> equal to the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public SqlDecimal ToSqlDecimal()
		{
			return (SqlDecimal)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlString" /> representing the value of this <see cref="T:System.Data.SqlTypes.SqlSingle" />.</returns>
		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlSingle" /> instance to the supplied <see cref="T:System.Object" /> and returns an indication of their relative values.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to be compared.</param>
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
		///  The object is a null reference (<see langword="Nothing" /> in Visual Basic)</returns>
		public int CompareTo(object value)
		{
			if (value is SqlSingle)
			{
				SqlSingle value2 = (SqlSingle)value;
				return this.CompareTo(value2);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlSingle));
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlSingle" /> instance to the supplied <see cref="T:System.Data.SqlTypes.SqlSingle" /> and returns an indication of their relative values.</summary>
		/// <param name="value">The <see cref="T:System.Data.SqlTypes.SqlSingle" /> to be compared.</param>
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
		public int CompareTo(SqlSingle value)
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

		/// <summary>Compares the supplied object parameter to the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> property of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> object.</summary>
		/// <param name="value">The object to be compared.</param>
		/// <returns>
		///   <see langword="true" /> if the object is an instance of <see cref="T:System.Data.SqlTypes.SqlSingle" /> and the two are equal. Otherwise, <see langword="false" />.</returns>
		public override bool Equals(object value)
		{
			if (!(value is SqlSingle))
			{
				return false;
			}
			SqlSingle y = (SqlSingle)value;
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
				this._fNotNull = false;
				return;
			}
			this._value = XmlConvert.ToSingle(reader.ReadElementString());
			this._fNotNull = true;
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
			writer.WriteString(XmlConvert.ToString(this._value));
		}

		/// <summary>Returns the XML Schema definition language (XSD) of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</summary>
		/// <param name="schemaSet">A <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</param>
		/// <returns>A <see langword="string" /> value that indicates the XSD of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</returns>
		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("float", "http://www.w3.org/2001/XMLSchema");
		}

		private bool _fNotNull;

		private float _value;

		/// <summary>Represents a <see cref="T:System.DBNull" /> that can be assigned to this instance of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</summary>
		public static readonly SqlSingle Null = new SqlSingle(true);

		/// <summary>Represents the zero value that can be assigned to the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> property of an instance of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> class.</summary>
		public static readonly SqlSingle Zero = new SqlSingle(0f);

		/// <summary>Represents the minimum value that can be assigned to <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> property of an instance of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> class.</summary>
		public static readonly SqlSingle MinValue = new SqlSingle(float.MinValue);

		/// <summary>Represents the maximum value that can be assigned to the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> property of an instance of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> class.</summary>
		public static readonly SqlSingle MaxValue = new SqlSingle(float.MaxValue);
	}
}
