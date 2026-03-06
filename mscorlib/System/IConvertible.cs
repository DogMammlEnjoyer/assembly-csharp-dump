using System;

namespace System
{
	/// <summary>Defines methods that convert the value of the implementing reference or value type to a common language runtime type that has an equivalent value.</summary>
	[CLSCompliant(false)]
	public interface IConvertible
	{
		/// <summary>Returns the <see cref="T:System.TypeCode" /> for this instance.</summary>
		/// <returns>The enumerated constant that is the <see cref="T:System.TypeCode" /> of the class or value type that implements this interface.</returns>
		TypeCode GetTypeCode();

		/// <summary>Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A Boolean value equivalent to the value of this instance.</returns>
		bool ToBoolean(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A Unicode character equivalent to the value of this instance.</returns>
		char ToChar(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 8-bit signed integer equivalent to the value of this instance.</returns>
		sbyte ToSByte(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 8-bit unsigned integer equivalent to the value of this instance.</returns>
		byte ToByte(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 16-bit signed integer equivalent to the value of this instance.</returns>
		short ToInt16(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 16-bit unsigned integer equivalent to the value of this instance.</returns>
		ushort ToUInt16(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 32-bit signed integer equivalent to the value of this instance.</returns>
		int ToInt32(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 32-bit unsigned integer equivalent to the value of this instance.</returns>
		uint ToUInt32(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 64-bit signed integer equivalent to the value of this instance.</returns>
		long ToInt64(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An 64-bit unsigned integer equivalent to the value of this instance.</returns>
		ulong ToUInt64(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A single-precision floating-point number equivalent to the value of this instance.</returns>
		float ToSingle(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A double-precision floating-point number equivalent to the value of this instance.</returns>
		double ToDouble(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent <see cref="T:System.Decimal" /> number using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A <see cref="T:System.Decimal" /> number equivalent to the value of this instance.</returns>
		decimal ToDecimal(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent <see cref="T:System.DateTime" /> using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A <see cref="T:System.DateTime" /> instance equivalent to the value of this instance.</returns>
		DateTime ToDateTime(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an equivalent <see cref="T:System.String" /> using the specified culture-specific formatting information.</summary>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>A <see cref="T:System.String" /> instance equivalent to the value of this instance.</returns>
		string ToString(IFormatProvider provider);

		/// <summary>Converts the value of this instance to an <see cref="T:System.Object" /> of the specified <see cref="T:System.Type" /> that has an equivalent value, using the specified culture-specific formatting information.</summary>
		/// <param name="conversionType">The <see cref="T:System.Type" /> to which the value of this instance is converted.</param>
		/// <param name="provider">An <see cref="T:System.IFormatProvider" /> interface implementation that supplies culture-specific formatting information.</param>
		/// <returns>An <see cref="T:System.Object" /> instance of type <paramref name="conversionType" /> whose value is equivalent to the value of this instance.</returns>
		object ToType(Type conversionType, IFormatProvider provider);
	}
}
