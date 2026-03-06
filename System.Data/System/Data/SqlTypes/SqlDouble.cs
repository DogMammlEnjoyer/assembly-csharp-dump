using System;
using System.Data.Common;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	/// <summary>Represents a floating-point number within the range of -1.79E +308 through 1.79E +308 to be stored in or retrieved from a database.</summary>
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlDouble : INullable, IComparable, IXmlSerializable
	{
		private SqlDouble(bool fNull)
		{
			this.m_fNotNull = false;
			this.m_value = 0.0;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure using the supplied double parameter to set the new <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure's <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> property.</summary>
		/// <param name="value">A double whose value will be used for the new <see cref="T:System.Data.SqlTypes.SqlDouble" />.</param>
		public SqlDouble(double value)
		{
			if (!double.IsFinite(value))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			this.m_value = value;
			this.m_fNotNull = true;
		}

		/// <summary>Returns a Boolean value that indicates whether this <see cref="T:System.Data.SqlTypes.SqlDouble" /> instance is null.</summary>
		/// <returns>
		///   <see langword="true" /> if <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is null. Otherwise, <see langword="false" />.</returns>
		public bool IsNull
		{
			get
			{
				return !this.m_fNotNull;
			}
		}

		/// <summary>Gets the value of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure. This property is read-only.</summary>
		/// <returns>The value of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</returns>
		public double Value
		{
			get
			{
				if (this.m_fNotNull)
				{
					return this.m_value;
				}
				throw new SqlNullValueException();
			}
		}

		/// <summary>Converts the supplied double value to a <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">The double value to convert.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlDouble" /> with the same value as the specified double parameter.</returns>
		public static implicit operator SqlDouble(double x)
		{
			return new SqlDouble(x);
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to double.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A double equivalent to the specified <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure's value.</returns>
		public static explicit operator double(SqlDouble x)
		{
			return x.Value;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to a string.</summary>
		/// <returns>A string representing the <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> of this <see cref="T:System.Data.SqlTypes.SqlDouble" />.</returns>
		public override string ToString()
		{
			if (!this.IsNull)
			{
				return this.m_value.ToString(null);
			}
			return SQLResource.NullString;
		}

		/// <summary>Converts the <see cref="T:System.String" /> representation of a number to its double-precision floating point number equivalent.</summary>
		/// <param name="s">The <see langword="String" /> to be parsed.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlDouble" /> that contains the value represented by the <see langword="String" />.</returns>
		public static SqlDouble Parse(string s)
		{
			if (s == SQLResource.NullString)
			{
				return SqlDouble.Null;
			}
			return new SqlDouble(double.Parse(s, CultureInfo.InvariantCulture));
		}

		/// <summary>Returns the negated value of the specified <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure that contains the negated value.</returns>
		public static SqlDouble operator -(SqlDouble x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble(-x.m_value);
			}
			return SqlDouble.Null;
		}

		/// <summary>The addition operator computes the sum of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>The sum of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</returns>
		public static SqlDouble operator +(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDouble.Null;
			}
			double num = x.m_value + y.m_value;
			if (double.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlDouble(num);
		}

		/// <summary>The subtraction operator the second <see cref="T:System.Data.SqlTypes.SqlDouble" /> operand from the first.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>The results of the subtraction operation.</returns>
		public static SqlDouble operator -(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDouble.Null;
			}
			double num = x.m_value - y.m_value;
			if (double.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlDouble(num);
		}

		/// <summary>The multiplication operator computes the product of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>The product of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</returns>
		public static SqlDouble operator *(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDouble.Null;
			}
			double num = x.m_value * y.m_value;
			if (double.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlDouble(num);
		}

		/// <summary>The division operator divides the first <see cref="T:System.Data.SqlTypes.SqlDouble" /> operand by the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure that contains the results of the division operation.</returns>
		public static SqlDouble operator /(SqlDouble x, SqlDouble y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlDouble.Null;
			}
			if (y.m_value == 0.0)
			{
				throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
			}
			double num = x.m_value / y.m_value;
			if (double.IsInfinity(num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlDouble(num);
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlBoolean" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">The <see cref="T:System.Data.SqlTypes.SqlBoolean" /> to be converted.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is either 0 or 1, depending on the <see cref="P:System.Data.SqlTypes.SqlBoolean.ByteValue" /> property of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> is <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static explicit operator SqlDouble(SqlBoolean x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble((double)x.ByteValue);
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlByte" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlByte.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlByte" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlByte" /> is <see cref="F:System.Data.SqlTypes.SqlByte.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static implicit operator SqlDouble(SqlByte x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble((double)x.Value);
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlInt16" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlInt16" /> structure.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlInt16.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlInt16" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlInt16" /> is <see cref="F:System.Data.SqlTypes.SqlInt16.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static implicit operator SqlDouble(SqlInt16 x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble((double)x.Value);
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlInt32" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlInt32" /> structure.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlInt32.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlInt32" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlInt32" /> is <see cref="F:System.Data.SqlTypes.SqlInt32.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static implicit operator SqlDouble(SqlInt32 x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble((double)x.Value);
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlInt64" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlInt64" /> structure.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlInt64.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlInt64" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlInt64" /> is <see cref="F:System.Data.SqlTypes.SqlInt64.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static implicit operator SqlDouble(SqlInt64 x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble((double)x.Value);
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlSingle" /> structure.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlSingle" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlSingle" /> is <see cref="F:System.Data.SqlTypes.SqlSingle.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static implicit operator SqlDouble(SqlSingle x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble((double)x.Value);
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlMoney" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlMoney" /> structure.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlMoney.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlMoney" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlMoney" /> is <see cref="F:System.Data.SqlTypes.SqlMoney.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static implicit operator SqlDouble(SqlMoney x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble(x.ToDouble());
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlDecimal" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDecimal" /> structure.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlDecimal.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlDecimal" /> parameter. If the <see cref="T:System.Data.SqlTypes.SqlDecimal" /> is <see cref="F:System.Data.SqlTypes.SqlDecimal.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static implicit operator SqlDouble(SqlDecimal x)
		{
			if (!x.IsNull)
			{
				return new SqlDouble(x.ToDouble());
			}
			return SqlDouble.Null;
		}

		/// <summary>Converts the supplied <see cref="T:System.Data.SqlTypes.SqlString" /> parameter to <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlString" /> object.</param>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlDouble" /> whose <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is equal to the value of the number represented by the <see cref="T:System.Data.SqlTypes.SqlString" />. If the <see cref="T:System.Data.SqlTypes.SqlString" /> is <see cref="F:System.Data.SqlTypes.SqlString.Null" />, the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure will be <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public static explicit operator SqlDouble(SqlString x)
		{
			if (x.IsNull)
			{
				return SqlDouble.Null;
			}
			return SqlDouble.Parse(x.Value);
		}

		/// <summary>Performs a logical comparison on two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>
		///   <see langword="true" /> if the two values are equal. Otherwise, <see langword="false" />.</returns>
		public static SqlBoolean operator ==(SqlDouble x, SqlDouble y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value == y.m_value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether they are not equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator !=(SqlDouble x, SqlDouble y)
		{
			return !(x == y);
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <(SqlDouble x, SqlDouble y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value < y.m_value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >(SqlDouble x, SqlDouble y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value > y.m_value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator <=(SqlDouble x, SqlDouble y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value <= y.m_value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is greater than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean operator >=(SqlDouble x, SqlDouble y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value >= y.m_value);
			}
			return SqlBoolean.Null;
		}

		/// <summary>The addition operator computes the sum of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>The sum of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</returns>
		public static SqlDouble Add(SqlDouble x, SqlDouble y)
		{
			return x + y;
		}

		/// <summary>The subtraction operator the second <see cref="T:System.Data.SqlTypes.SqlDouble" /> operand from the first.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>The results of the subtraction operation.</returns>
		public static SqlDouble Subtract(SqlDouble x, SqlDouble y)
		{
			return x - y;
		}

		/// <summary>The multiplication operator computes the product of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>The product of the two <see cref="T:System.Data.SqlTypes.SqlDouble" /> operands.</returns>
		public static SqlDouble Multiply(SqlDouble x, SqlDouble y)
		{
			return x * y;
		}

		/// <summary>The division operator divides the first <see cref="T:System.Data.SqlTypes.SqlDouble" /> operand by the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure that contains the results of the division operation.</returns>
		public static SqlDouble Divide(SqlDouble x, SqlDouble y)
		{
			return x / y;
		}

		/// <summary>Performs a logical comparison on two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether they are equal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>
		///   <see langword="true" /> if the two values are equal. Otherwise, <see langword="false" />.</returns>
		public static SqlBoolean Equals(SqlDouble x, SqlDouble y)
		{
			return x == y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether they are notequal.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the two instances are not equal or <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the two instances are equal. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean NotEquals(SqlDouble x, SqlDouble y)
		{
			return x != y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is less than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThan(SqlDouble x, SqlDouble y)
		{
			return x < y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is greater than the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThan(SqlDouble x, SqlDouble y)
		{
			return x > y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is less than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is less than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean LessThanOrEqual(SqlDouble x, SqlDouble y)
		{
			return x <= y;
		}

		/// <summary>Compares two instances of <see cref="T:System.Data.SqlTypes.SqlDouble" /> to determine whether the first is greater than or equal to the second.</summary>
		/// <param name="x">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <param name="y">A <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</param>
		/// <returns>A <see cref="T:System.Data.SqlTypes.SqlBoolean" /> that is <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the first instance is greater than or equal to the second instance. Otherwise, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" />. If either instance of <see cref="T:System.Data.SqlTypes.SqlDouble" /> is null, the <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> of the <see cref="T:System.Data.SqlTypes.SqlBoolean" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" />.</returns>
		public static SqlBoolean GreaterThanOrEqual(SqlDouble x, SqlDouble y)
		{
			return x >= y;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlBoolean" />.</summary>
		/// <returns>A <see langword="SqlBoolean" /> structure whose <see cref="P:System.Data.SqlTypes.SqlBoolean.Value" /> will be <see cref="F:System.Data.SqlTypes.SqlBoolean.True" /> if the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure's <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> is non-zero, <see cref="F:System.Data.SqlTypes.SqlBoolean.False" /> if the <see cref="T:System.Data.SqlTypes.SqlDouble" /> is zero and <see cref="F:System.Data.SqlTypes.SqlBoolean.Null" /> if the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure is <see cref="F:System.Data.SqlTypes.SqlDouble.Null" />.</returns>
		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlByte" />.</summary>
		/// <returns>A <see langword="SqlByte" /> structure whose <see langword="Value" /> equals the <see langword="Value" /> of this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</returns>
		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt16" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt16" /> structure whose <see langword="Value" /> equals the integer part of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure's value.</returns>
		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt32" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt32" /> structure whose <see langword="Value" /> equals the integer part of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure's value.</returns>
		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlInt64" />.</summary>
		/// <returns>A new <see cref="T:System.Data.SqlTypes.SqlInt64" /> structure whose <see langword="Value" /> equals the integer part of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure's value.</returns>
		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlMoney" />.</summary>
		/// <returns>A new <see langword="SqlMoney" /> structure whose <see cref="P:System.Data.SqlTypes.SqlMoney.Value" /> is equal to the value of this <see cref="T:System.Data.SqlTypes.SqlDouble" />.</returns>
		public SqlMoney ToSqlMoney()
		{
			return (SqlMoney)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlDecimal" />.</summary>
		/// <returns>A new <see langword="SqlDecimal" /> structure whose converted value equals the rounded value of this <see langword="SqlDouble" />.</returns>
		public SqlDecimal ToSqlDecimal()
		{
			return (SqlDecimal)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlSingle" />.</summary>
		/// <returns>A new <see langword="SqlSingle" /> structure whose <see cref="P:System.Data.SqlTypes.SqlSingle.Value" /> is equal to the <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> of this <see cref="T:System.Data.SqlTypes.SqlDouble" />.</returns>
		public SqlSingle ToSqlSingle()
		{
			return (SqlSingle)this;
		}

		/// <summary>Converts this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure to <see cref="T:System.Data.SqlTypes.SqlString" />.</summary>
		/// <returns>A <see langword="SqlString" /> representing the <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> of this <see cref="T:System.Data.SqlTypes.SqlDouble" />.</returns>
		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlDouble" /> instance to the supplied <see cref="T:System.Object" /> and returns an indication of their relative values.</summary>
		/// <param name="value">The <see cref="T:System.Object" /> to compare.</param>
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
		public int CompareTo(object value)
		{
			if (value is SqlDouble)
			{
				SqlDouble value2 = (SqlDouble)value;
				return this.CompareTo(value2);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlDouble));
		}

		/// <summary>Compares this <see cref="T:System.Data.SqlTypes.SqlDouble" /> instance to the supplied <see cref="T:System.Data.SqlTypes.SqlDouble" /> and returns an indication of their relative values.</summary>
		/// <param name="value">The <see cref="T:System.Data.SqlTypes.SqlDouble" /> to be compared.</param>
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
		public int CompareTo(SqlDouble value)
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

		/// <summary>Compares the supplied object parameter to the <see cref="P:System.Data.SqlTypes.SqlDateTime.Value" /> property of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> object.</summary>
		/// <param name="value">The object to be compared.</param>
		/// <returns>
		///   <see langword="true" /> if the two values are equal. Otherwise, <see langword="false" />.</returns>
		public override bool Equals(object value)
		{
			if (!(value is SqlDouble))
			{
				return false;
			}
			SqlDouble y = (SqlDouble)value;
			if (y.IsNull || this.IsNull)
			{
				return y.IsNull && this.IsNull;
			}
			return (this == y).Value;
		}

		/// <summary>Returns the hash code for this <see cref="T:System.Data.SqlTypes.SqlDouble" /> structre.</summary>
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
		/// <returns>An XML schema consumed by .NET Framework.</returns>
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <param name="reader">A <see cref="T:System.Xml.XmlReader" />.</param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string attribute = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute != null && XmlConvert.ToBoolean(attribute))
			{
				reader.ReadElementString();
				this.m_fNotNull = false;
				return;
			}
			this.m_value = XmlConvert.ToDouble(reader.ReadElementString());
			this.m_fNotNull = true;
		}

		/// <summary>This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		/// <param name="writer">A <see cref="T:System.Xml.XmlWriter" />.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(XmlConvert.ToString(this.m_value));
		}

		/// <summary>Returns the XML Schema definition language (XSD) of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</summary>
		/// <param name="schemaSet">An <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</param>
		/// <returns>A <see langword="string" /> value that indicates the XSD of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</returns>
		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("double", "http://www.w3.org/2001/XMLSchema");
		}

		private bool m_fNotNull;

		private double m_value;

		/// <summary>Represents a <see cref="T:System.DBNull" /> that can be assigned to this instance of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</summary>
		public static readonly SqlDouble Null = new SqlDouble(true);

		/// <summary>Represents a zero value that can be assigned to the <see cref="P:System.Data.SqlTypes.SqlDouble.Value" /> property of an instance of the <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</summary>
		public static readonly SqlDouble Zero = new SqlDouble(0.0);

		/// <summary>A constant representing the minimum possible value of <see cref="T:System.Data.SqlTypes.SqlDouble" />.</summary>
		public static readonly SqlDouble MinValue = new SqlDouble(double.MinValue);

		/// <summary>A constant representing the maximum value for a <see cref="T:System.Data.SqlTypes.SqlDouble" /> structure.</summary>
		public static readonly SqlDouble MaxValue = new SqlDouble(double.MaxValue);
	}
}
