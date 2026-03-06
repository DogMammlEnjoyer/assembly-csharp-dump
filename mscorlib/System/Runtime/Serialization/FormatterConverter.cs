using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Runtime.Serialization
{
	/// <summary>Represents a base implementation of the <see cref="T:System.Runtime.Serialization.IFormatterConverter" /> interface that uses the <see cref="T:System.Convert" /> class and the <see cref="T:System.IConvertible" /> interface.</summary>
	public class FormatterConverter : IFormatterConverter
	{
		/// <summary>Converts a value to the given <see cref="T:System.Type" />.</summary>
		/// <param name="value">The object to convert.</param>
		/// <param name="type">The <see cref="T:System.Type" /> into which <paramref name="value" /> is converted.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public object Convert(object value, Type type)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to the given <see cref="T:System.TypeCode" />.</summary>
		/// <param name="value">The object to convert.</param>
		/// <param name="typeCode">The <see cref="T:System.TypeCode" /> into which <paramref name="value" /> is converted.</param>
		/// <returns>The converted <paramref name="value" />, or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public object Convert(object value, TypeCode typeCode)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ChangeType(value, typeCode, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a <see cref="T:System.Boolean" />.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public bool ToBoolean(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a Unicode character.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public char ToChar(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToChar(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a <see cref="T:System.SByte" />.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public sbyte ToSByte(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToSByte(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to an 8-bit unsigned integer.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public byte ToByte(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToByte(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a 16-bit signed integer.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public short ToInt16(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToInt16(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a 16-bit unsigned integer.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public ushort ToUInt16(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToUInt16(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a 32-bit signed integer.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public int ToInt32(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a 32-bit unsigned integer.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public uint ToUInt32(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToUInt32(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a 64-bit signed integer.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public long ToInt64(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToInt64(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a 64-bit unsigned integer.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public ulong ToUInt64(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToUInt64(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a single-precision floating-point number.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public float ToSingle(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToSingle(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a double-precision floating-point number.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public double ToDouble(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a <see cref="T:System.Decimal" />.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public decimal ToDecimal(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToDecimal(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts a value to a <see cref="T:System.DateTime" />.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public DateTime ToDateTime(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToDateTime(value, CultureInfo.InvariantCulture);
		}

		/// <summary>Converts the specified object to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The object to convert.</param>
		/// <returns>The converted <paramref name="value" /> or <see langword="null" /> if the <paramref name="type" /> parameter is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public string ToString(object value)
		{
			if (value == null)
			{
				FormatterConverter.ThrowValueNullException();
			}
			return System.Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowValueNullException()
		{
			throw new ArgumentNullException("value");
		}
	}
}
