using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	/// <summary>Converts a base data type to another base data type.</summary>
	public static class Convert
	{
		private unsafe static bool TryDecodeFromUtf16(ReadOnlySpan<char> utf16, Span<byte> bytes, out int consumed, out int written)
		{
			ref char reference = ref MemoryMarshal.GetReference<char>(utf16);
			ref byte reference2 = ref MemoryMarshal.GetReference<byte>(bytes);
			int num = utf16.Length & -4;
			int length = bytes.Length;
			int i = 0;
			int num2 = 0;
			if (utf16.Length != 0)
			{
				ref sbyte ptr = ref Convert.s_decodingMap[0];
				int num3;
				if (length >= (num >> 2) * 3)
				{
					num3 = num - 4;
				}
				else
				{
					num3 = length / 3 * 4;
				}
				while (i < num3)
				{
					int num4 = Convert.Decode(Unsafe.Add<char>(ref reference, i), ref ptr);
					if (num4 < 0)
					{
						IL_201:
						consumed = i;
						written = num2;
						return false;
					}
					Convert.WriteThreeLowOrderBytes(Unsafe.Add<byte>(ref reference2, num2), num4);
					num2 += 3;
					i += 4;
				}
				if (num3 != num - 4 || i == num)
				{
					goto IL_201;
				}
				int num5 = (int)(*Unsafe.Add<char>(ref reference, num - 4));
				int num6 = (int)(*Unsafe.Add<char>(ref reference, num - 3));
				int num7 = (int)(*Unsafe.Add<char>(ref reference, num - 2));
				int num8 = (int)(*Unsafe.Add<char>(ref reference, num - 1));
				if (((long)(num5 | num6 | num7 | num8) & (long)((ulong)-256)) != 0L)
				{
					goto IL_201;
				}
				num5 = (int)(*Unsafe.Add<sbyte>(ref ptr, num5));
				num6 = (int)(*Unsafe.Add<sbyte>(ref ptr, num6));
				num5 <<= 18;
				num6 <<= 12;
				num5 |= num6;
				if (num8 != 61)
				{
					num7 = (int)(*Unsafe.Add<sbyte>(ref ptr, num7));
					num8 = (int)(*Unsafe.Add<sbyte>(ref ptr, num8));
					num7 <<= 6;
					num5 |= num8;
					num5 |= num7;
					if (num5 < 0 || num2 > length - 3)
					{
						goto IL_201;
					}
					Convert.WriteThreeLowOrderBytes(Unsafe.Add<byte>(ref reference2, num2), num5);
					num2 += 3;
				}
				else if (num7 != 61)
				{
					num7 = (int)(*Unsafe.Add<sbyte>(ref ptr, num7));
					num7 <<= 6;
					num5 |= num7;
					if (num5 < 0 || num2 > length - 2)
					{
						goto IL_201;
					}
					*Unsafe.Add<byte>(ref reference2, num2) = (byte)(num5 >> 16);
					*Unsafe.Add<byte>(ref reference2, num2 + 1) = (byte)(num5 >> 8);
					num2 += 2;
				}
				else
				{
					if (num5 < 0 || num2 > length - 1)
					{
						goto IL_201;
					}
					*Unsafe.Add<byte>(ref reference2, num2) = (byte)(num5 >> 16);
					num2++;
				}
				i += 4;
				if (num != utf16.Length)
				{
					goto IL_201;
				}
			}
			consumed = i;
			written = num2;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static int Decode(ref char encodedChars, ref sbyte decodingMap)
		{
			int num = (int)encodedChars;
			int num2 = (int)(*Unsafe.Add<char>(ref encodedChars, 1));
			int num3 = (int)(*Unsafe.Add<char>(ref encodedChars, 2));
			int num4 = (int)(*Unsafe.Add<char>(ref encodedChars, 3));
			if (((long)(num | num2 | num3 | num4) & (long)((ulong)-256)) != 0L)
			{
				return -1;
			}
			num = (int)(*Unsafe.Add<sbyte>(ref decodingMap, num));
			num2 = (int)(*Unsafe.Add<sbyte>(ref decodingMap, num2));
			num3 = (int)(*Unsafe.Add<sbyte>(ref decodingMap, num3));
			num4 = (int)(*Unsafe.Add<sbyte>(ref decodingMap, num4));
			num <<= 18;
			num2 <<= 12;
			num3 <<= 6;
			num |= num4;
			num2 |= num3;
			return num | num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static void WriteThreeLowOrderBytes(ref byte destination, int value)
		{
			destination = (byte)(value >> 16);
			*Unsafe.Add<byte>(ref destination, 1) = (byte)(value >> 8);
			*Unsafe.Add<byte>(ref destination, 2) = (byte)value;
		}

		/// <summary>Returns the <see cref="T:System.TypeCode" /> for the specified object.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <returns>The <see cref="T:System.TypeCode" /> for <paramref name="value" />, or <see cref="F:System.TypeCode.Empty" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		public static TypeCode GetTypeCode(object value)
		{
			if (value == null)
			{
				return TypeCode.Empty;
			}
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				return convertible.GetTypeCode();
			}
			return TypeCode.Object;
		}

		/// <summary>Returns an indication whether the specified object is of type <see cref="T:System.DBNull" />.</summary>
		/// <param name="value">An object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is of type <see cref="T:System.DBNull" />; otherwise, <see langword="false" />.</returns>
		public static bool IsDBNull(object value)
		{
			if (value == System.DBNull.Value)
			{
				return true;
			}
			IConvertible convertible = value as IConvertible;
			return convertible != null && convertible.GetTypeCode() == TypeCode.DBNull;
		}

		/// <summary>Returns an object of the specified type whose value is equivalent to the specified object.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="typeCode">The type of object to return.</param>
		/// <returns>An object whose underlying type is <paramref name="typeCode" /> and whose value is equivalent to <paramref name="value" />.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), if <paramref name="value" /> is <see langword="null" /> and <paramref name="typeCode" /> is <see cref="F:System.TypeCode.Empty" />, <see cref="F:System.TypeCode.String" />, or <see cref="F:System.TypeCode.Object" />.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.  
		///  -or-  
		///  <paramref name="value" /> is <see langword="null" /> and <paramref name="typeCode" /> specifies a value type.  
		///  -or-  
		///  <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in a format recognized by the <paramref name="typeCode" /> type.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is out of the range of the <paramref name="typeCode" /> type.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="typeCode" /> is invalid.</exception>
		public static object ChangeType(object value, TypeCode typeCode)
		{
			return Convert.ChangeType(value, typeCode, CultureInfo.CurrentCulture);
		}

		/// <summary>Returns an object of the specified type whose value is equivalent to the specified object. A parameter supplies culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="typeCode">The type of object to return.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>An object whose underlying type is <paramref name="typeCode" /> and whose value is equivalent to <paramref name="value" />.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), if <paramref name="value" /> is <see langword="null" /> and <paramref name="typeCode" /> is <see cref="F:System.TypeCode.Empty" />, <see cref="F:System.TypeCode.String" />, or <see cref="F:System.TypeCode.Object" />.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.  
		///  -or-  
		///  <paramref name="value" /> is <see langword="null" /> and <paramref name="typeCode" /> specifies a value type.  
		///  -or-  
		///  <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in a format for the <paramref name="typeCode" /> type recognized by <paramref name="provider" />.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is out of the range of the <paramref name="typeCode" /> type.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="typeCode" /> is invalid.</exception>
		public static object ChangeType(object value, TypeCode typeCode, IFormatProvider provider)
		{
			if (value == null && (typeCode == TypeCode.Empty || typeCode == TypeCode.String || typeCode == TypeCode.Object))
			{
				return null;
			}
			IConvertible convertible = value as IConvertible;
			if (convertible == null)
			{
				throw new InvalidCastException("Object must implement IConvertible.");
			}
			switch (typeCode)
			{
			case TypeCode.Empty:
				throw new InvalidCastException("Object cannot be cast to Empty.");
			case TypeCode.Object:
				return value;
			case TypeCode.DBNull:
				throw new InvalidCastException("Object cannot be cast to DBNull.");
			case TypeCode.Boolean:
				return convertible.ToBoolean(provider);
			case TypeCode.Char:
				return convertible.ToChar(provider);
			case TypeCode.SByte:
				return convertible.ToSByte(provider);
			case TypeCode.Byte:
				return convertible.ToByte(provider);
			case TypeCode.Int16:
				return convertible.ToInt16(provider);
			case TypeCode.UInt16:
				return convertible.ToUInt16(provider);
			case TypeCode.Int32:
				return convertible.ToInt32(provider);
			case TypeCode.UInt32:
				return convertible.ToUInt32(provider);
			case TypeCode.Int64:
				return convertible.ToInt64(provider);
			case TypeCode.UInt64:
				return convertible.ToUInt64(provider);
			case TypeCode.Single:
				return convertible.ToSingle(provider);
			case TypeCode.Double:
				return convertible.ToDouble(provider);
			case TypeCode.Decimal:
				return convertible.ToDecimal(provider);
			case TypeCode.DateTime:
				return convertible.ToDateTime(provider);
			case TypeCode.String:
				return convertible.ToString(provider);
			}
			throw new ArgumentException("Unknown TypeCode value.");
		}

		internal static object DefaultToType(IConvertible value, Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (value.GetType() == targetType)
			{
				return value;
			}
			if (targetType == Convert.ConvertTypes[3])
			{
				return value.ToBoolean(provider);
			}
			if (targetType == Convert.ConvertTypes[4])
			{
				return value.ToChar(provider);
			}
			if (targetType == Convert.ConvertTypes[5])
			{
				return value.ToSByte(provider);
			}
			if (targetType == Convert.ConvertTypes[6])
			{
				return value.ToByte(provider);
			}
			if (targetType == Convert.ConvertTypes[7])
			{
				return value.ToInt16(provider);
			}
			if (targetType == Convert.ConvertTypes[8])
			{
				return value.ToUInt16(provider);
			}
			if (targetType == Convert.ConvertTypes[9])
			{
				return value.ToInt32(provider);
			}
			if (targetType == Convert.ConvertTypes[10])
			{
				return value.ToUInt32(provider);
			}
			if (targetType == Convert.ConvertTypes[11])
			{
				return value.ToInt64(provider);
			}
			if (targetType == Convert.ConvertTypes[12])
			{
				return value.ToUInt64(provider);
			}
			if (targetType == Convert.ConvertTypes[13])
			{
				return value.ToSingle(provider);
			}
			if (targetType == Convert.ConvertTypes[14])
			{
				return value.ToDouble(provider);
			}
			if (targetType == Convert.ConvertTypes[15])
			{
				return value.ToDecimal(provider);
			}
			if (targetType == Convert.ConvertTypes[16])
			{
				return value.ToDateTime(provider);
			}
			if (targetType == Convert.ConvertTypes[18])
			{
				return value.ToString(provider);
			}
			if (targetType == Convert.ConvertTypes[1])
			{
				return value;
			}
			if (targetType == Convert.EnumType)
			{
				return (Enum)value;
			}
			if (targetType == Convert.ConvertTypes[2])
			{
				throw new InvalidCastException("Object cannot be cast to DBNull.");
			}
			if (targetType == Convert.ConvertTypes[0])
			{
				throw new InvalidCastException("Object cannot be cast to Empty.");
			}
			throw new InvalidCastException(string.Format("Invalid cast from '{0}' to '{1}'.", value.GetType().FullName, targetType.FullName));
		}

		/// <summary>Returns an object of the specified type and whose value is equivalent to the specified object.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="conversionType">The type of object to return.</param>
		/// <returns>An object whose type is <paramref name="conversionType" /> and whose value is equivalent to <paramref name="value" />.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), if <paramref name="value" /> is <see langword="null" /> and <paramref name="conversionType" /> is not a value type.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.  
		///  -or-  
		///  <paramref name="value" /> is <see langword="null" /> and <paramref name="conversionType" /> is a value type.  
		///  -or-  
		///  <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in a format recognized by <paramref name="conversionType" />.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is out of the range of <paramref name="conversionType" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="conversionType" /> is <see langword="null" />.</exception>
		public static object ChangeType(object value, Type conversionType)
		{
			return Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);
		}

		/// <summary>Returns an object of the specified type whose value is equivalent to the specified object. A parameter supplies culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="conversionType">The type of object to return.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>An object whose type is <paramref name="conversionType" /> and whose value is equivalent to <paramref name="value" />.  
		///  -or-  
		///  <paramref name="value" />, if the <see cref="T:System.Type" /> of <paramref name="value" /> and <paramref name="conversionType" /> are equal.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), if <paramref name="value" /> is <see langword="null" /> and <paramref name="conversionType" /> is not a value type.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.  
		///  -or-  
		///  <paramref name="value" /> is <see langword="null" /> and <paramref name="conversionType" /> is a value type.  
		///  -or-  
		///  <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in a format for <paramref name="conversionType" /> recognized by <paramref name="provider" />.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is out of the range of <paramref name="conversionType" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="conversionType" /> is <see langword="null" />.</exception>
		public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
		{
			if (conversionType == null)
			{
				throw new ArgumentNullException("conversionType");
			}
			if (value == null)
			{
				if (conversionType.IsValueType)
				{
					throw new InvalidCastException("Null object cannot be converted to a value type.");
				}
				return null;
			}
			else
			{
				IConvertible convertible = value as IConvertible;
				if (convertible == null)
				{
					if (value.GetType() == conversionType)
					{
						return value;
					}
					throw new InvalidCastException("Object must implement IConvertible.");
				}
				else
				{
					if (conversionType == Convert.ConvertTypes[3])
					{
						return convertible.ToBoolean(provider);
					}
					if (conversionType == Convert.ConvertTypes[4])
					{
						return convertible.ToChar(provider);
					}
					if (conversionType == Convert.ConvertTypes[5])
					{
						return convertible.ToSByte(provider);
					}
					if (conversionType == Convert.ConvertTypes[6])
					{
						return convertible.ToByte(provider);
					}
					if (conversionType == Convert.ConvertTypes[7])
					{
						return convertible.ToInt16(provider);
					}
					if (conversionType == Convert.ConvertTypes[8])
					{
						return convertible.ToUInt16(provider);
					}
					if (conversionType == Convert.ConvertTypes[9])
					{
						return convertible.ToInt32(provider);
					}
					if (conversionType == Convert.ConvertTypes[10])
					{
						return convertible.ToUInt32(provider);
					}
					if (conversionType == Convert.ConvertTypes[11])
					{
						return convertible.ToInt64(provider);
					}
					if (conversionType == Convert.ConvertTypes[12])
					{
						return convertible.ToUInt64(provider);
					}
					if (conversionType == Convert.ConvertTypes[13])
					{
						return convertible.ToSingle(provider);
					}
					if (conversionType == Convert.ConvertTypes[14])
					{
						return convertible.ToDouble(provider);
					}
					if (conversionType == Convert.ConvertTypes[15])
					{
						return convertible.ToDecimal(provider);
					}
					if (conversionType == Convert.ConvertTypes[16])
					{
						return convertible.ToDateTime(provider);
					}
					if (conversionType == Convert.ConvertTypes[18])
					{
						return convertible.ToString(provider);
					}
					if (conversionType == Convert.ConvertTypes[1])
					{
						return value;
					}
					return convertible.ToType(conversionType, provider);
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowCharOverflowException()
		{
			throw new OverflowException("Value was either too large or too small for a character.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowByteOverflowException()
		{
			throw new OverflowException("Value was either too large or too small for an unsigned byte.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowSByteOverflowException()
		{
			throw new OverflowException("Value was either too large or too small for a signed byte.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowInt16OverflowException()
		{
			throw new OverflowException("Value was either too large or too small for an Int16.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowUInt16OverflowException()
		{
			throw new OverflowException("Value was either too large or too small for a UInt16.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowInt32OverflowException()
		{
			throw new OverflowException("Value was either too large or too small for an Int32.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowUInt32OverflowException()
		{
			throw new OverflowException("Value was either too large or too small for a UInt32.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowInt64OverflowException()
		{
			throw new OverflowException("Value was either too large or too small for an Int64.");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowUInt64OverflowException()
		{
			throw new OverflowException("Value was either too large or too small for a UInt64.");
		}

		/// <summary>Converts the value of a specified object to an equivalent Boolean value.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> or <see langword="false" />, which reflects the value returned by invoking the <see cref="M:System.IConvertible.ToBoolean(System.IFormatProvider)" /> method for the underlying type of <paramref name="value" />. If <paramref name="value" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is a string that does not equal <see cref="F:System.Boolean.TrueString" /> or <see cref="F:System.Boolean.FalseString" />.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion of <paramref name="value" /> to a <see cref="T:System.Boolean" /> is not supported.</exception>
		public static bool ToBoolean(object value)
		{
			return value != null && ((IConvertible)value).ToBoolean(null);
		}

		/// <summary>Converts the value of the specified object to an equivalent Boolean value, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>
		///   <see langword="true" /> or <see langword="false" />, which reflects the value returned by invoking the <see cref="M:System.IConvertible.ToBoolean(System.IFormatProvider)" /> method for the underlying type of <paramref name="value" />. If <paramref name="value" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is a string that does not equal <see cref="F:System.Boolean.TrueString" /> or <see cref="F:System.Boolean.FalseString" />.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion of <paramref name="value" /> to a <see cref="T:System.Boolean" /> is not supported.</exception>
		public static bool ToBoolean(object value, IFormatProvider provider)
		{
			return value != null && ((IConvertible)value).ToBoolean(provider);
		}

		/// <summary>Returns the specified Boolean value; no actual conversion is performed.</summary>
		/// <param name="value">The Boolean value to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static bool ToBoolean(bool value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool ToBoolean(sbyte value)
		{
			return value != 0;
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static bool ToBoolean(char value)
		{
			return ((IConvertible)value).ToBoolean(null);
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		public static bool ToBoolean(byte value)
		{
			return value > 0;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		public static bool ToBoolean(short value)
		{
			return value != 0;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool ToBoolean(ushort value)
		{
			return value > 0;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		public static bool ToBoolean(int value)
		{
			return value != 0;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool ToBoolean(uint value)
		{
			return value > 0U;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		public static bool ToBoolean(long value)
		{
			return value != 0L;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent Boolean value.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		[CLSCompliant(false)]
		public static bool ToBoolean(ulong value)
		{
			return value > 0UL;
		}

		/// <summary>Converts the specified string representation of a logical value to its Boolean equivalent.</summary>
		/// <param name="value">A string that contains the value of either <see cref="F:System.Boolean.TrueString" /> or <see cref="F:System.Boolean.FalseString" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> equals <see cref="F:System.Boolean.TrueString" />, or <see langword="false" /> if <paramref name="value" /> equals <see cref="F:System.Boolean.FalseString" /> or <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not equal to <see cref="F:System.Boolean.TrueString" /> or <see cref="F:System.Boolean.FalseString" />.</exception>
		public static bool ToBoolean(string value)
		{
			return value != null && bool.Parse(value);
		}

		/// <summary>Converts the specified string representation of a logical value to its Boolean equivalent, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the value of either <see cref="F:System.Boolean.TrueString" /> or <see cref="F:System.Boolean.FalseString" />.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information. This parameter is ignored.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> equals <see cref="F:System.Boolean.TrueString" />, or <see langword="false" /> if <paramref name="value" /> equals <see cref="F:System.Boolean.FalseString" /> or <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not equal to <see cref="F:System.Boolean.TrueString" /> or <see cref="F:System.Boolean.FalseString" />.</exception>
		public static bool ToBoolean(string value, IFormatProvider provider)
		{
			return value != null && bool.Parse(value);
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent Boolean value.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		public static bool ToBoolean(float value)
		{
			return value != 0f;
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent Boolean value.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		public static bool ToBoolean(double value)
		{
			return value != 0.0;
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent Boolean value.</summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is not zero; otherwise, <see langword="false" />.</returns>
		public static bool ToBoolean(decimal value)
		{
			return value != 0m;
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static bool ToBoolean(DateTime value)
		{
			return ((IConvertible)value).ToBoolean(null);
		}

		/// <summary>Converts the value of the specified object to a Unicode character.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <returns>A Unicode character that is equivalent to value, or <see cref="F:System.Char.MinValue" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is a null string.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion of <paramref name="value" /> to a <see cref="T:System.Char" /> is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Char.MinValue" /> or greater than <see cref="F:System.Char.MaxValue" />.</exception>
		public static char ToChar(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToChar(null);
			}
			return '\0';
		}

		/// <summary>Converts the value of the specified object to its equivalent Unicode character, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />, or <see cref="F:System.Char.MinValue" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is a null string.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion of <paramref name="value" /> to a <see cref="T:System.Char" /> is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Char.MinValue" /> or greater than <see cref="F:System.Char.MaxValue" />.</exception>
		public static char ToChar(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToChar(provider);
			}
			return '\0';
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static char ToChar(bool value)
		{
			return ((IConvertible)value).ToChar(null);
		}

		/// <summary>Returns the specified Unicode character value; no actual conversion is performed.</summary>
		/// <param name="value">The Unicode character to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static char ToChar(char value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Char.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static char ToChar(sbyte value)
		{
			if (value < 0)
			{
				Convert.ThrowCharOverflowException();
			}
			return (char)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		public static char ToChar(byte value)
		{
			return (char)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Char.MinValue" />.</exception>
		public static char ToChar(short value)
		{
			if (value < 0)
			{
				Convert.ThrowCharOverflowException();
			}
			return (char)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static char ToChar(ushort value)
		{
			return (char)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Char.MinValue" /> or greater than <see cref="F:System.Char.MaxValue" />.</exception>
		public static char ToChar(int value)
		{
			if (value < 0 || value > 65535)
			{
				Convert.ThrowCharOverflowException();
			}
			return (char)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Char.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static char ToChar(uint value)
		{
			if (value > 65535U)
			{
				Convert.ThrowCharOverflowException();
			}
			return (char)value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Char.MinValue" /> or greater than <see cref="F:System.Char.MaxValue" />.</exception>
		public static char ToChar(long value)
		{
			if (value < 0L || value > 65535L)
			{
				Convert.ThrowCharOverflowException();
			}
			return (char)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to its equivalent Unicode character.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A Unicode character that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Char.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static char ToChar(ulong value)
		{
			if (value > 65535UL)
			{
				Convert.ThrowCharOverflowException();
			}
			return (char)value;
		}

		/// <summary>Converts the first character of a specified string to a Unicode character.</summary>
		/// <param name="value">A string of length 1.</param>
		/// <returns>A Unicode character that is equivalent to the first and only character in <paramref name="value" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">The length of <paramref name="value" /> is not 1.</exception>
		public static char ToChar(string value)
		{
			return Convert.ToChar(value, null);
		}

		/// <summary>Converts the first character of a specified string to a Unicode character, using specified culture-specific formatting information.</summary>
		/// <param name="value">A string of length 1 or <see langword="null" />.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information. This parameter is ignored.</param>
		/// <returns>A Unicode character that is equivalent to the first and only character in <paramref name="value" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">The length of <paramref name="value" /> is not 1.</exception>
		public static char ToChar(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length != 1)
			{
				throw new FormatException("String must be exactly one character long.");
			}
			return value[0];
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static char ToChar(float value)
		{
			return ((IConvertible)value).ToChar(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static char ToChar(double value)
		{
			return ((IConvertible)value).ToChar(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static char ToChar(decimal value)
		{
			return ((IConvertible)value).ToChar(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static char ToChar(DateTime value)
		{
			return ((IConvertible)value).ToChar(null);
		}

		/// <summary>Converts the value of the specified object to an 8-bit signed integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.SByte.MinValue" /> or greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToSByte(null);
			}
			return 0;
		}

		/// <summary>Converts the value of the specified object to an 8-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.SByte.MinValue" /> or greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToSByte(provider);
			}
			return 0;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 8-bit signed integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		[CLSCompliant(false)]
		public static sbyte ToSByte(bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		/// <summary>Returns the specified 8-bit signed integer; no actual conversion is performed.</summary>
		/// <param name="value">The 8-bit signed integer to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		[CLSCompliant(false)]
		public static sbyte ToSByte(sbyte value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 8-bit signed integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(char value)
		{
			if (value > '\u007f')
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent 8-bit signed integer.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(byte value)
		{
			if (value > 127)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to the equivalent 8-bit signed integer.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(short value)
		{
			if (value < -128 || value > 127)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent 8-bit signed integer.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(ushort value)
		{
			if (value > 127)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(int value)
		{
			if (value < -128 || value > 127)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(uint value)
		{
			if ((ulong)value > 127UL)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(long value)
		{
			if (value < -128L || value > 127L)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(ulong value)
		{
			if (value > 127UL)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 8-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(float value)
		{
			return Convert.ToSByte((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 8-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(double value)
		{
			return Convert.ToSByte(Convert.ToInt32(value));
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 8-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.SByte.MaxValue" /> or less than <see cref="F:System.SByte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(decimal value)
		{
			return decimal.ToSByte(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if value is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.SByte.MinValue" /> or greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return sbyte.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 8-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.SByte.MinValue" /> or greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(string value, IFormatProvider provider)
		{
			return sbyte.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(DateTime value)
		{
			return ((IConvertible)value).ToSByte(null);
		}

		/// <summary>Converts the value of the specified object to an 8-bit unsigned integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in the property format for a <see cref="T:System.Byte" /> value.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement <see cref="T:System.IConvertible" />.  
		/// -or-  
		/// Conversion from <paramref name="value" /> to the <see cref="T:System.Byte" /> type is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToByte(null);
			}
			return 0;
		}

		/// <summary>Converts the value of the specified object to an 8-bit unsigned integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in the property format for a <see cref="T:System.Byte" /> value.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement <see cref="T:System.IConvertible" />.  
		/// -or-  
		/// Conversion from <paramref name="value" /> to the <see cref="T:System.Byte" /> type is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToByte(provider);
			}
			return 0;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		public static byte ToByte(bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		/// <summary>Returns the specified 8-bit unsigned integer; no actual conversion is performed.</summary>
		/// <param name="value">The 8-bit unsigned integer to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static byte ToByte(byte value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(char value)
		{
			if (value > 'ÿ')
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The 8-bit signed integer to be converted.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Byte.MinValue" />.</exception>
		[CLSCompliant(false)]
		public static byte ToByte(sbyte value)
		{
			if (value < 0)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(short value)
		{
			if (value < 0 || value > 255)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static byte ToByte(ushort value)
		{
			if (value > 255)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(int value)
		{
			if (value < 0 || value > 255)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static byte ToByte(uint value)
		{
			if (value > 255U)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(long value)
		{
			if (value < 0L || value > 255L)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static byte ToByte(ulong value)
		{
			if (value > 255UL)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">A single-precision floating-point number.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 8-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Byte.MaxValue" /> or less than <see cref="F:System.Byte.MinValue" />.</exception>
		public static byte ToByte(float value)
		{
			return Convert.ToByte((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 8-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Byte.MaxValue" /> or less than <see cref="F:System.Byte.MinValue" />.</exception>
		public static byte ToByte(double value)
		{
			return Convert.ToByte(Convert.ToInt32(value));
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 8-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Byte.MaxValue" /> or less than <see cref="F:System.Byte.MinValue" />.</exception>
		public static byte ToByte(decimal value)
		{
			return decimal.ToByte(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return byte.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 8-bit unsigned integer, using specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return byte.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static byte ToByte(DateTime value)
		{
			return ((IConvertible)value).ToByte(null);
		}

		/// <summary>Converts the value of the specified object to a 16-bit signed integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format for an <see cref="T:System.Int16" /> type.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int16.MinValue" /> or greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		public static short ToInt16(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToInt16(null);
			}
			return 0;
		}

		/// <summary>Converts the value of the specified object to a 16-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format for an <see cref="T:System.Int16" /> type.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement <see cref="T:System.IConvertible" />.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int16.MinValue" /> or greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		public static short ToInt16(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToInt16(provider);
			}
			return 0;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 16-bit signed integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		public static short ToInt16(bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 16-bit signed integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		public static short ToInt16(char value)
		{
			if (value > '翿')
			{
				Convert.ThrowInt16OverflowException();
			}
			return (short)value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent 16-bit signed integer.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static short ToInt16(sbyte value)
		{
			return (short)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent 16-bit signed integer.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static short ToInt16(byte value)
		{
			return (short)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent 16-bit signed integer.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static short ToInt16(ushort value)
		{
			if (value > 32767)
			{
				Convert.ThrowInt16OverflowException();
			}
			return (short)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>The 16-bit signed integer equivalent of <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" /> or less than <see cref="F:System.Int16.MinValue" />.</exception>
		public static short ToInt16(int value)
		{
			if (value < -32768 || value > 32767)
			{
				Convert.ThrowInt16OverflowException();
			}
			return (short)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static short ToInt16(uint value)
		{
			if ((ulong)value > 32767UL)
			{
				Convert.ThrowInt16OverflowException();
			}
			return (short)value;
		}

		/// <summary>Returns the specified 16-bit signed integer; no actual conversion is performed.</summary>
		/// <param name="value">The 16-bit signed integer to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static short ToInt16(short value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" /> or less than <see cref="F:System.Int16.MinValue" />.</exception>
		public static short ToInt16(long value)
		{
			if (value < -32768L || value > 32767L)
			{
				Convert.ThrowInt16OverflowException();
			}
			return (short)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A 16-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static short ToInt16(ulong value)
		{
			if (value > 32767UL)
			{
				Convert.ThrowInt16OverflowException();
			}
			return (short)value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 16-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" /> or less than <see cref="F:System.Int16.MinValue" />.</exception>
		public static short ToInt16(float value)
		{
			return Convert.ToInt16((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 16-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" /> or less than <see cref="F:System.Int16.MinValue" />.</exception>
		public static short ToInt16(double value)
		{
			return Convert.ToInt16(Convert.ToInt32(value));
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 16-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int16.MaxValue" /> or less than <see cref="F:System.Int16.MinValue" />.</exception>
		public static short ToInt16(decimal value)
		{
			return decimal.ToInt16(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A 16-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int16.MinValue" /> or greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		public static short ToInt16(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return short.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 16-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 16-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int16.MinValue" /> or greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		public static short ToInt16(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return short.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static short ToInt16(DateTime value)
		{
			return ((IConvertible)value).ToInt16(null);
		}

		/// <summary>Converts the value of the specified object to a 16-bit unsigned integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the  <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt16.MinValue" /> or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToUInt16(null);
			}
			return 0;
		}

		/// <summary>Converts the value of the specified object to a 16-bit unsigned integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the  <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt16.MinValue" /> or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToUInt16(provider);
			}
			return 0;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		[CLSCompliant(false)]
		public static ushort ToUInt16(bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>The 16-bit unsigned integer equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static ushort ToUInt16(char value)
		{
			return (ushort)value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(sbyte value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt16OverflowException();
			}
			return (ushort)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static ushort ToUInt16(byte value)
		{
			return (ushort)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to the equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(short value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt16OverflowException();
			}
			return (ushort)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(int value)
		{
			if (value < 0 || value > 65535)
			{
				Convert.ThrowUInt16OverflowException();
			}
			return (ushort)value;
		}

		/// <summary>Returns the specified 16-bit unsigned integer; no actual conversion is performed.</summary>
		/// <param name="value">The 16-bit unsigned integer to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		[CLSCompliant(false)]
		public static ushort ToUInt16(ushort value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(uint value)
		{
			if (value > 65535U)
			{
				Convert.ThrowUInt16OverflowException();
			}
			return (ushort)value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(long value)
		{
			if (value < 0L || value > 65535L)
			{
				Convert.ThrowUInt16OverflowException();
			}
			return (ushort)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(ulong value)
		{
			if (value > 65535UL)
			{
				Convert.ThrowUInt16OverflowException();
			}
			return (ushort)value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 16-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(float value)
		{
			return Convert.ToUInt16((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 16-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(double value)
		{
			return Convert.ToUInt16(Convert.ToInt32(value));
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 16-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(decimal value)
		{
			return decimal.ToUInt16(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt16.MinValue" /> or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return ushort.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 16-bit unsigned integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt16.MinValue" /> or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return ushort.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(DateTime value)
		{
			return ((IConvertible)value).ToUInt16(null);
		}

		/// <summary>Converts the value of the specified object to a 32-bit signed integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A 32-bit signed integer equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the  <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int ToInt32(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToInt32(null);
			}
			return 0;
		}

		/// <summary>Converts the value of the specified object to a 32-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement <see cref="T:System.IConvertible" />.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int ToInt32(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToInt32(provider);
			}
			return 0;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 32-bit signed integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		public static int ToInt32(bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 32-bit signed integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static int ToInt32(char value)
		{
			return (int)value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent 32-bit signed integer.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static int ToInt32(sbyte value)
		{
			return (int)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent 32-bit signed integer.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static int ToInt32(byte value)
		{
			return (int)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static int ToInt32(short value)
		{
			return (int)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent 32-bit signed integer.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static int ToInt32(ushort value)
		{
			return (int)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static int ToInt32(uint value)
		{
			if (value > 2147483647U)
			{
				Convert.ThrowInt32OverflowException();
			}
			return (int)value;
		}

		/// <summary>Returns the specified 32-bit signed integer; no actual conversion is performed.</summary>
		/// <param name="value">The 32-bit signed integer to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static int ToInt32(int value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int32.MaxValue" /> or less than <see cref="F:System.Int32.MinValue" />.</exception>
		public static int ToInt32(long value)
		{
			if (value < -2147483648L || value > 2147483647L)
			{
				Convert.ThrowInt32OverflowException();
			}
			return (int)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static int ToInt32(ulong value)
		{
			if (value > 2147483647UL)
			{
				Convert.ThrowInt32OverflowException();
			}
			return (int)value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 32-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int32.MaxValue" /> or less than <see cref="F:System.Int32.MinValue" />.</exception>
		public static int ToInt32(float value)
		{
			return Convert.ToInt32((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 32-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int32.MaxValue" /> or less than <see cref="F:System.Int32.MinValue" />.</exception>
		public static int ToInt32(double value)
		{
			if (value >= 0.0)
			{
				if (value < 2147483647.5)
				{
					int num = (int)value;
					double num2 = value - (double)num;
					if (num2 > 0.5 || (num2 == 0.5 && (num & 1) != 0))
					{
						num++;
					}
					return num;
				}
			}
			else if (value >= -2147483648.5)
			{
				int num3 = (int)value;
				double num4 = value - (double)num3;
				if (num4 < -0.5 || (num4 == -0.5 && (num3 & 1) != 0))
				{
					num3--;
				}
				return num3;
			}
			throw new OverflowException("Value was either too large or too small for an Int32.");
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 32-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int32.MaxValue" /> or less than <see cref="F:System.Int32.MinValue" />.</exception>
		public static int ToInt32(decimal value)
		{
			return decimal.ToInt32(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A 32-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int ToInt32(string value)
		{
			if (value == null)
			{
				return 0;
			}
			return int.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 32-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 32-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int ToInt32(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0;
			}
			return int.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static int ToInt32(DateTime value)
		{
			return ((IConvertible)value).ToInt32(null);
		}

		/// <summary>Converts the value of the specified object to a 32-bit unsigned integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt32.MinValue" /> or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToUInt32(null);
			}
			return 0U;
		}

		/// <summary>Converts the value of the specified object to a 32-bit unsigned integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt32.MinValue" /> or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToUInt32(provider);
			}
			return 0U;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		[CLSCompliant(false)]
		public static uint ToUInt32(bool value)
		{
			if (!value)
			{
				return 0U;
			}
			return 1U;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static uint ToUInt32(char value)
		{
			return (uint)value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(sbyte value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt32OverflowException();
			}
			return (uint)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static uint ToUInt32(byte value)
		{
			return (uint)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to the equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(short value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt32OverflowException();
			}
			return (uint)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static uint ToUInt32(ushort value)
		{
			return (uint)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(int value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt32OverflowException();
			}
			return (uint)value;
		}

		/// <summary>Returns the specified 32-bit unsigned integer; no actual conversion is performed.</summary>
		/// <param name="value">The 32-bit unsigned integer to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		[CLSCompliant(false)]
		public static uint ToUInt32(uint value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(long value)
		{
			if (value < 0L || value > (long)((ulong)-1))
			{
				Convert.ThrowUInt32OverflowException();
			}
			return (uint)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(ulong value)
		{
			if (value > (ulong)-1)
			{
				Convert.ThrowUInt32OverflowException();
			}
			return (uint)value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 32-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(float value)
		{
			return Convert.ToUInt32((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 32-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(double value)
		{
			if (value >= -0.5 && value < 4294967295.5)
			{
				uint num = (uint)value;
				double num2 = value - num;
				if (num2 > 0.5 || (num2 == 0.5 && (num & 1U) != 0U))
				{
					num += 1U;
				}
				return num;
			}
			throw new OverflowException("Value was either too large or too small for a UInt32.");
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 32-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(decimal value)
		{
			return decimal.ToUInt32(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt32.MinValue" /> or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(string value)
		{
			if (value == null)
			{
				return 0U;
			}
			return uint.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 32-bit unsigned integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt32.MinValue" /> or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0U;
			}
			return uint.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(DateTime value)
		{
			return ((IConvertible)value).ToUInt32(null);
		}

		/// <summary>Converts the value of the specified object to a 64-bit signed integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int64.MinValue" /> or greater than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long ToInt64(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToInt64(null);
			}
			return 0L;
		}

		/// <summary>Converts the value of the specified object to a 64-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int64.MinValue" /> or greater than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long ToInt64(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToInt64(provider);
			}
			return 0L;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 64-bit signed integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		public static long ToInt64(bool value)
		{
			return value ? 1L : 0L;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 64-bit signed integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static long ToInt64(char value)
		{
			return (long)((ulong)value);
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent 64-bit signed integer.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static long ToInt64(sbyte value)
		{
			return (long)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent 64-bit signed integer.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static long ToInt64(byte value)
		{
			return (long)((ulong)value);
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static long ToInt64(short value)
		{
			return (long)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent 64-bit signed integer.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static long ToInt64(ushort value)
		{
			return (long)((ulong)value);
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		public static long ToInt64(int value)
		{
			return (long)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static long ToInt64(uint value)
		{
			return (long)((ulong)value);
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static long ToInt64(ulong value)
		{
			if (value > 9223372036854775807UL)
			{
				Convert.ThrowInt64OverflowException();
			}
			return (long)value;
		}

		/// <summary>Returns the specified 64-bit signed integer; no actual conversion is performed.</summary>
		/// <param name="value">A 64-bit signed integer.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static long ToInt64(long value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 64-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int64.MaxValue" /> or less than <see cref="F:System.Int64.MinValue" />.</exception>
		public static long ToInt64(float value)
		{
			return Convert.ToInt64((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 64-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int64.MaxValue" /> or less than <see cref="F:System.Int64.MinValue" />.</exception>
		public static long ToInt64(double value)
		{
			return checked((long)Math.Round(value));
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 64-bit signed integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Int64.MaxValue" /> or less than <see cref="F:System.Int64.MinValue" />.</exception>
		public static long ToInt64(decimal value)
		{
			return decimal.ToInt64(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">A string that contains a number to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int64.MinValue" /> or greater than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long ToInt64(string value)
		{
			if (value == null)
			{
				return 0L;
			}
			return long.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 64-bit signed integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 64-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Int64.MinValue" /> or greater than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long ToInt64(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0L;
			}
			return long.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static long ToInt64(DateTime value)
		{
			return ((IConvertible)value).ToInt64(null);
		}

		/// <summary>Converts the value of the specified object to a 64-bit unsigned integer.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt64.MinValue" /> or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToUInt64(null);
			}
			return 0UL;
		}

		/// <summary>Converts the value of the specified object to a 64-bit unsigned integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt64.MinValue" /> or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToUInt64(provider);
			}
			return 0UL;
		}

		/// <summary>Converts the specified Boolean value to the equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		[CLSCompliant(false)]
		public static ulong ToUInt64(bool value)
		{
			if (!value)
			{
				return 0UL;
			}
			return 1UL;
		}

		/// <summary>Converts the value of the specified Unicode character to the equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static ulong ToUInt64(char value)
		{
			return (ulong)value;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(sbyte value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt64OverflowException();
			}
			return (ulong)((long)value);
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static ulong ToUInt64(byte value)
		{
			return (ulong)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to the equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(short value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt64OverflowException();
			}
			return (ulong)((long)value);
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static ulong ToUInt64(ushort value)
		{
			return (ulong)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(int value)
		{
			if (value < 0)
			{
				Convert.ThrowUInt64OverflowException();
			}
			return (ulong)((long)value);
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static ulong ToUInt64(uint value)
		{
			return (ulong)value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(long value)
		{
			if (value < 0L)
			{
				Convert.ThrowUInt64OverflowException();
			}
			return (ulong)value;
		}

		/// <summary>Returns the specified 64-bit unsigned integer; no actual conversion is performed.</summary>
		/// <param name="value">The 64-bit unsigned integer to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		[CLSCompliant(false)]
		public static ulong ToUInt64(ulong value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 64-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(float value)
		{
			return Convert.ToUInt64((double)value);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 64-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(double value)
		{
			return checked((ulong)Math.Round(value));
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>
		///   <paramref name="value" />, rounded to the nearest 64-bit unsigned integer. If <paramref name="value" /> is halfway between two whole numbers, the even number is returned; that is, 4.5 is converted to 4, and 5.5 is converted to 6.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is less than zero or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(decimal value)
		{
			return decimal.ToUInt64(decimal.Round(value, 0));
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A 64-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt64.MinValue" /> or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(string value)
		{
			if (value == null)
			{
				return 0UL;
			}
			return ulong.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent 64-bit unsigned integer, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> does not consist of an optional sign followed by a sequence of digits (0 through 9).</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt64.MinValue" /> or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0UL;
			}
			return ulong.Parse(value, NumberStyles.Integer, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(DateTime value)
		{
			return ((IConvertible)value).ToUInt64(null);
		}

		/// <summary>Converts the value of the specified object to a single-precision floating-point number.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Single.MinValue" /> or greater than <see cref="F:System.Single.MaxValue" />.</exception>
		public static float ToSingle(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToSingle(null);
			}
			return 0f;
		}

		/// <summary>Converts the value of the specified object to an single-precision floating-point number, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement <see cref="T:System.IConvertible" />.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Single.MinValue" /> or greater than <see cref="F:System.Single.MaxValue" />.</exception>
		public static float ToSingle(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToSingle(provider);
			}
			return 0f;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>An 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static float ToSingle(sbyte value)
		{
			return (float)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static float ToSingle(byte value)
		{
			return (float)value;
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static float ToSingle(char value)
		{
			return ((IConvertible)value).ToSingle(null);
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static float ToSingle(short value)
		{
			return (float)value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static float ToSingle(ushort value)
		{
			return (float)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static float ToSingle(int value)
		{
			return (float)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static float ToSingle(uint value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static float ToSingle(long value)
		{
			return (float)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static float ToSingle(ulong value)
		{
			return value;
		}

		/// <summary>Returns the specified single-precision floating-point number; no actual conversion is performed.</summary>
		/// <param name="value">The single-precision floating-point number to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static float ToSingle(float value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.  
		///  <paramref name="value" /> is rounded using rounding to nearest. For example, when rounded to two decimals, the value 2.345 becomes 2.34 and the value 2.355 becomes 2.36.</returns>
		public static float ToSingle(double value)
		{
			return (float)value;
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to <paramref name="value" />.  
		///  <paramref name="value" /> is rounded using rounding to nearest. For example, when rounded to two decimals, the value 2.345 becomes 2.34 and the value 2.355 becomes 2.36.</returns>
		public static float ToSingle(decimal value)
		{
			return (float)value;
		}

		/// <summary>Converts the specified string representation of a number to an equivalent single-precision floating-point number.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A single-precision floating-point number that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a number in a valid format.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Single.MinValue" /> or greater than <see cref="F:System.Single.MaxValue" />.</exception>
		public static float ToSingle(string value)
		{
			if (value == null)
			{
				return 0f;
			}
			return float.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent single-precision floating-point number, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A single-precision floating-point number that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a number in a valid format.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Single.MinValue" /> or greater than <see cref="F:System.Single.MaxValue" />.</exception>
		public static float ToSingle(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0f;
			}
			return float.Parse(value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, provider);
		}

		/// <summary>Converts the specified Boolean value to the equivalent single-precision floating-point number.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		public static float ToSingle(bool value)
		{
			return (float)(value ? 1 : 0);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static float ToSingle(DateTime value)
		{
			return ((IConvertible)value).ToSingle(null);
		}

		/// <summary>Converts the value of the specified object to a double-precision floating-point number.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format for a <see cref="T:System.Double" /> type.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Double.MinValue" /> or greater than <see cref="F:System.Double.MaxValue" />.</exception>
		public static double ToDouble(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToDouble(null);
			}
			return 0.0;
		}

		/// <summary>Converts the value of the specified object to an double-precision floating-point number, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />, or zero if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format for a <see cref="T:System.Double" /> type.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Double.MinValue" /> or greater than <see cref="F:System.Double.MaxValue" />.</exception>
		public static double ToDouble(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToDouble(provider);
			}
			return 0.0;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>The 8-bit signed integer that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static double ToDouble(sbyte value)
		{
			return (double)value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>The double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static double ToDouble(byte value)
		{
			return (double)value;
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A double-precision floating-point number equivalent to <paramref name="value" />.</returns>
		public static double ToDouble(short value)
		{
			return (double)value;
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static double ToDouble(char value)
		{
			return ((IConvertible)value).ToDouble(null);
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to the equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static double ToDouble(ushort value)
		{
			return (double)value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static double ToDouble(int value)
		{
			return (double)value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static double ToDouble(uint value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static double ToDouble(long value)
		{
			return (double)value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static double ToDouble(ulong value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">The single-precision floating-point number.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static double ToDouble(float value)
		{
			return (double)value;
		}

		/// <summary>Returns the specified double-precision floating-point number; no actual conversion is performed.</summary>
		/// <param name="value">The double-precision floating-point number to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static double ToDouble(double value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified decimal number to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to <paramref name="value" />.</returns>
		public static double ToDouble(decimal value)
		{
			return (double)value;
		}

		/// <summary>Converts the specified string representation of a number to an equivalent double-precision floating-point number.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a number in a valid format.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Double.MinValue" /> or greater than <see cref="F:System.Double.MaxValue" />.</exception>
		public static double ToDouble(string value)
		{
			if (value == null)
			{
				return 0.0;
			}
			return double.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent double-precision floating-point number, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A double-precision floating-point number that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a number in a valid format.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Double.MinValue" /> or greater than <see cref="F:System.Double.MaxValue" />.</exception>
		public static double ToDouble(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0.0;
			}
			return double.Parse(value, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, provider);
		}

		/// <summary>Converts the specified Boolean value to the equivalent double-precision floating-point number.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		public static double ToDouble(bool value)
		{
			return (double)(value ? 1 : 0);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static double ToDouble(DateTime value)
		{
			return ((IConvertible)value).ToDouble(null);
		}

		/// <summary>Converts the value of the specified object to an equivalent decimal number.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format for a <see cref="T:System.Decimal" /> type.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Decimal.MinValue" /> or greater than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal ToDecimal(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToDecimal(null);
			}
			return 0m;
		}

		/// <summary>Converts the value of the specified object to an equivalent decimal number, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not in an appropriate format for a <see cref="T:System.Decimal" /> type.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Decimal.MinValue" /> or greater than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal ToDecimal(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToDecimal(provider);
			}
			return 0m;
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to the equivalent decimal number.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static decimal ToDecimal(sbyte value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to the equivalent decimal number.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>The decimal number that is equivalent to <paramref name="value" />.</returns>
		public static decimal ToDecimal(byte value)
		{
			return value;
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static decimal ToDecimal(char value)
		{
			return ((IConvertible)value).ToDecimal(null);
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to an equivalent decimal number.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		public static decimal ToDecimal(short value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to an equivalent decimal number.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>The decimal number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static decimal ToDecimal(ushort value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to an equivalent decimal number.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		public static decimal ToDecimal(int value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to an equivalent decimal number.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static decimal ToDecimal(uint value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to an equivalent decimal number.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		public static decimal ToDecimal(long value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to an equivalent decimal number.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static decimal ToDecimal(ulong value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to the equivalent decimal number.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Decimal.MaxValue" /> or less than <see cref="F:System.Decimal.MinValue" />.</exception>
		public static decimal ToDecimal(float value)
		{
			return (decimal)value;
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to an equivalent decimal number.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>A decimal number that is equivalent to <paramref name="value" />.</returns>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> is greater than <see cref="F:System.Decimal.MaxValue" /> or less than <see cref="F:System.Decimal.MinValue" />.</exception>
		public static decimal ToDecimal(double value)
		{
			return (decimal)value;
		}

		/// <summary>Converts the specified string representation of a number to an equivalent decimal number.</summary>
		/// <param name="value">A string that contains a number to convert.</param>
		/// <returns>A decimal number that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a number in a valid format.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Decimal.MinValue" /> or greater than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal ToDecimal(string value)
		{
			if (value == null)
			{
				return 0m;
			}
			return decimal.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent decimal number, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains a number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>A decimal number that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a number in a valid format.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" /> represents a number that is less than <see cref="F:System.Decimal.MinValue" /> or greater than <see cref="F:System.Decimal.MaxValue" />.</exception>
		public static decimal ToDecimal(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return 0m;
			}
			return decimal.Parse(value, NumberStyles.Number, provider);
		}

		/// <summary>Returns the specified decimal number; no actual conversion is performed.</summary>
		/// <param name="value">A decimal number.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static decimal ToDecimal(decimal value)
		{
			return value;
		}

		/// <summary>Converts the specified Boolean value to the equivalent decimal number.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The number 1 if <paramref name="value" /> is <see langword="true" />; otherwise, 0.</returns>
		public static decimal ToDecimal(bool value)
		{
			return value ? 1 : 0;
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static decimal ToDecimal(DateTime value)
		{
			return ((IConvertible)value).ToDecimal(null);
		}

		/// <summary>Returns the specified <see cref="T:System.DateTime" /> object; no actual conversion is performed.</summary>
		/// <param name="value">A date and time value.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static DateTime ToDateTime(DateTime value)
		{
			return value;
		}

		/// <summary>Converts the value of the specified object to a <see cref="T:System.DateTime" /> object.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface, or <see langword="null" />.</param>
		/// <returns>The date and time equivalent of the value of <paramref name="value" />, or a date and time equivalent of <see cref="F:System.DateTime.MinValue" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a valid date and time value.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		public static DateTime ToDateTime(object value)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
			return DateTime.MinValue;
		}

		/// <summary>Converts the value of the specified object to a <see cref="T:System.DateTime" /> object, using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that implements the <see cref="T:System.IConvertible" /> interface.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The date and time equivalent of the value of <paramref name="value" />, or the date and time equivalent of <see cref="F:System.DateTime.MinValue" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a valid date and time value.</exception>
		/// <exception cref="T:System.InvalidCastException">
		///   <paramref name="value" /> does not implement the <see cref="T:System.IConvertible" /> interface.  
		/// -or-  
		/// The conversion is not supported.</exception>
		public static DateTime ToDateTime(object value, IFormatProvider provider)
		{
			if (value != null)
			{
				return ((IConvertible)value).ToDateTime(provider);
			}
			return DateTime.MinValue;
		}

		/// <summary>Converts the specified string representation of a date and time to an equivalent date and time value.</summary>
		/// <param name="value">The string representation of a date and time.</param>
		/// <returns>The date and time equivalent of the value of <paramref name="value" />, or the date and time equivalent of <see cref="F:System.DateTime.MinValue" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a properly formatted date and time string.</exception>
		public static DateTime ToDateTime(string value)
		{
			if (value == null)
			{
				return new DateTime(0L);
			}
			return DateTime.Parse(value, CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the specified string representation of a number to an equivalent date and time, using the specified culture-specific formatting information.</summary>
		/// <param name="value">A string that contains a date and time to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The date and time equivalent of the value of <paramref name="value" />, or the date and time equivalent of <see cref="F:System.DateTime.MinValue" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> is not a properly formatted date and time string.</exception>
		public static DateTime ToDateTime(string value, IFormatProvider provider)
		{
			if (value == null)
			{
				return new DateTime(0L);
			}
			return DateTime.Parse(value, provider);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static DateTime ToDateTime(sbyte value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(byte value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(short value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static DateTime ToDateTime(ushort value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(int value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static DateTime ToDateTime(uint value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(long value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		[CLSCompliant(false)]
		public static DateTime ToDateTime(ulong value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(bool value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(char value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The single-precision floating-point value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(float value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The double-precision floating-point value to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(double value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Calling this method always throws <see cref="T:System.InvalidCastException" />.</summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>This conversion is not supported. No value is returned.</returns>
		/// <exception cref="T:System.InvalidCastException">This conversion is not supported.</exception>
		public static DateTime ToDateTime(decimal value)
		{
			return ((IConvertible)value).ToDateTime(null);
		}

		/// <summary>Converts the value of the specified object to its equivalent string representation.</summary>
		/// <param name="value">An object that supplies the value to convert, or <see langword="null" />.</param>
		/// <returns>The string representation of <paramref name="value" />, or <see cref="F:System.String.Empty" /> if <paramref name="value" /> is <see langword="null" />.</returns>
		public static string ToString(object value)
		{
			return Convert.ToString(value, null);
		}

		/// <summary>Converts the value of the specified object to its equivalent string representation using the specified culture-specific formatting information.</summary>
		/// <param name="value">An object that supplies the value to convert, or <see langword="null" />.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />, or <see cref="F:System.String.Empty" /> if <paramref name="value" /> is an object whose value is <see langword="null" />. If <paramref name="value" /> is <see langword="null" />, the method returns <see langword="null" />.</returns>
		public static string ToString(object value, IFormatProvider provider)
		{
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				return convertible.ToString(provider);
			}
			IFormattable formattable = value as IFormattable;
			if (formattable != null)
			{
				return formattable.ToString(null, provider);
			}
			if (value != null)
			{
				return value.ToString();
			}
			return string.Empty;
		}

		/// <summary>Converts the specified Boolean value to its equivalent string representation.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(bool value)
		{
			return value.ToString();
		}

		/// <summary>Converts the specified Boolean value to its equivalent string representation.</summary>
		/// <param name="value">The Boolean value to convert.</param>
		/// <param name="provider">An instance of an object. This parameter is ignored.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(bool value, IFormatProvider provider)
		{
			return value.ToString();
		}

		/// <summary>Converts the value of the specified Unicode character to its equivalent string representation.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(char value)
		{
			return char.ToString(value);
		}

		/// <summary>Converts the value of the specified Unicode character to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The Unicode character to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information. This parameter is ignored.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(char value, IFormatProvider provider)
		{
			return value.ToString();
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to its equivalent string representation.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(sbyte value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 8-bit signed integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 8-bit signed integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(sbyte value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to its equivalent string representation.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(byte value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 8-bit unsigned integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(byte value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to its equivalent string representation.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(short value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 16-bit signed integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(short value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to its equivalent string representation.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(ushort value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 16-bit unsigned integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 16-bit unsigned integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(ushort value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to its equivalent string representation.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(int value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 32-bit signed integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(int value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to its equivalent string representation.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(uint value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 32-bit unsigned integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 32-bit unsigned integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(uint value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to its equivalent string representation.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(long value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 64-bit signed integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(long value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to its equivalent string representation.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(ulong value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified 64-bit unsigned integer to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The 64-bit unsigned integer to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(ulong value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to its equivalent string representation.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(float value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified single-precision floating-point number to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The single-precision floating-point number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(float value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to its equivalent string representation.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(double value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified double-precision floating-point number to its equivalent string representation.</summary>
		/// <param name="value">The double-precision floating-point number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(double value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified decimal number to its equivalent string representation.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(decimal value)
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		/// <summary>Converts the value of the specified decimal number to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The decimal number to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(decimal value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Converts the value of the specified <see cref="T:System.DateTime" /> to its equivalent string representation.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(DateTime value)
		{
			return value.ToString();
		}

		/// <summary>Converts the value of the specified <see cref="T:System.DateTime" /> to its equivalent string representation, using the specified culture-specific formatting information.</summary>
		/// <param name="value">The date and time value to convert.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <returns>The string representation of <paramref name="value" />.</returns>
		public static string ToString(DateTime value, IFormatProvider provider)
		{
			return value.ToString(provider);
		}

		/// <summary>Returns the specified string instance; no actual conversion is performed.</summary>
		/// <param name="value">The string to return.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static string ToString(string value)
		{
			return value;
		}

		/// <summary>Returns the specified string instance; no actual conversion is performed.</summary>
		/// <param name="value">The string to return.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information. This parameter is ignored.</param>
		/// <returns>
		///   <paramref name="value" /> is returned unchanged.</returns>
		public static string ToString(string value, IFormatProvider provider)
		{
			return value;
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 8-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>An 8-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 unsigned number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a base 10 unsigned number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />.</exception>
		public static byte ToByte(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0;
			}
			int num = ParseNumbers.StringToInt(value.AsSpan(), fromBase, 4608);
			if (num < 0 || num > 255)
			{
				Convert.ThrowByteOverflowException();
			}
			return (byte)num;
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 8-bit signed integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>An 8-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.SByte.MinValue" /> or greater than <see cref="F:System.SByte.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0;
			}
			int num = ParseNumbers.StringToInt(value.AsSpan(), fromBase, 5120);
			if (fromBase != 10 && num <= 255)
			{
				return (sbyte)num;
			}
			if (num < -128 || num > 127)
			{
				Convert.ThrowSByteOverflowException();
			}
			return (sbyte)num;
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 16-bit signed integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>A 16-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.Int16.MinValue" /> or greater than <see cref="F:System.Int16.MaxValue" />.</exception>
		public static short ToInt16(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0;
			}
			int num = ParseNumbers.StringToInt(value.AsSpan(), fromBase, 6144);
			if (fromBase != 10 && num <= 65535)
			{
				return (short)num;
			}
			if (num < -32768 || num > 32767)
			{
				Convert.ThrowInt16OverflowException();
			}
			return (short)num;
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 16-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>A 16-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 unsigned number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a non-base 10 unsigned number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt16.MinValue" /> or greater than <see cref="F:System.UInt16.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0;
			}
			int num = ParseNumbers.StringToInt(value.AsSpan(), fromBase, 4608);
			if (num < 0 || num > 65535)
			{
				Convert.ThrowUInt16OverflowException();
			}
			return (ushort)num;
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 32-bit signed integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>A 32-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />.</exception>
		public static int ToInt32(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0;
			}
			return ParseNumbers.StringToInt(value.AsSpan(), fromBase, 4096);
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 32-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>A 32-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 unsigned number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a non-base 10 unsigned number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt32.MinValue" /> or greater than <see cref="F:System.UInt32.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0U;
			}
			return (uint)ParseNumbers.StringToInt(value.AsSpan(), fromBase, 4608);
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 64-bit signed integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>A 64-bit signed integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a non-base 10 signed number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.Int64.MinValue" /> or greater than <see cref="F:System.Int64.MaxValue" />.</exception>
		public static long ToInt64(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0L;
			}
			return ParseNumbers.StringToLong(value.AsSpan(), fromBase, 4096);
		}

		/// <summary>Converts the string representation of a number in a specified base to an equivalent 64-bit unsigned integer.</summary>
		/// <param name="value">A string that contains the number to convert.</param>
		/// <param name="fromBase">The base of the number in <paramref name="value" />, which must be 2, 8, 10, or 16.</param>
		/// <returns>A 64-bit unsigned integer that is equivalent to the number in <paramref name="value" />, or 0 (zero) if <paramref name="value" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="fromBase" /> is not 2, 8, 10, or 16.  
		/// -or-  
		/// <paramref name="value" />, which represents a non-base 10 unsigned number, is prefixed with a negative sign.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="value" /> contains a character that is not a valid digit in the base specified by <paramref name="fromBase" />. The exception message indicates that there are no digits to convert if the first character in <paramref name="value" /> is invalid; otherwise, the message indicates that <paramref name="value" /> contains invalid trailing characters.</exception>
		/// <exception cref="T:System.OverflowException">
		///   <paramref name="value" />, which represents a non-base 10 unsigned number, is prefixed with a negative sign.  
		/// -or-  
		/// <paramref name="value" /> represents a number that is less than <see cref="F:System.UInt64.MinValue" /> or greater than <see cref="F:System.UInt64.MaxValue" />.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(string value, int fromBase)
		{
			if (fromBase != 2 && fromBase != 8 && fromBase != 10 && fromBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			if (value == null)
			{
				return 0UL;
			}
			return (ulong)ParseNumbers.StringToLong(value.AsSpan(), fromBase, 4608);
		}

		/// <summary>Converts the value of an 8-bit unsigned integer to its equivalent string representation in a specified base.</summary>
		/// <param name="value">The 8-bit unsigned integer to convert.</param>
		/// <param name="toBase">The base of the return value, which must be 2, 8, 10, or 16.</param>
		/// <returns>The string representation of <paramref name="value" /> in base <paramref name="toBase" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="toBase" /> is not 2, 8, 10, or 16.</exception>
		public static string ToString(byte value, int toBase)
		{
			if (toBase != 2 && toBase != 8 && toBase != 10 && toBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			return ParseNumbers.IntToString((int)value, toBase, -1, ' ', 64);
		}

		/// <summary>Converts the value of a 16-bit signed integer to its equivalent string representation in a specified base.</summary>
		/// <param name="value">The 16-bit signed integer to convert.</param>
		/// <param name="toBase">The base of the return value, which must be 2, 8, 10, or 16.</param>
		/// <returns>The string representation of <paramref name="value" /> in base <paramref name="toBase" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="toBase" /> is not 2, 8, 10, or 16.</exception>
		public static string ToString(short value, int toBase)
		{
			if (toBase != 2 && toBase != 8 && toBase != 10 && toBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			return ParseNumbers.IntToString((int)value, toBase, -1, ' ', 128);
		}

		/// <summary>Converts the value of a 32-bit signed integer to its equivalent string representation in a specified base.</summary>
		/// <param name="value">The 32-bit signed integer to convert.</param>
		/// <param name="toBase">The base of the return value, which must be 2, 8, 10, or 16.</param>
		/// <returns>The string representation of <paramref name="value" /> in base <paramref name="toBase" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="toBase" /> is not 2, 8, 10, or 16.</exception>
		public static string ToString(int value, int toBase)
		{
			if (toBase != 2 && toBase != 8 && toBase != 10 && toBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			return ParseNumbers.IntToString(value, toBase, -1, ' ', 0);
		}

		/// <summary>Converts the value of a 64-bit signed integer to its equivalent string representation in a specified base.</summary>
		/// <param name="value">The 64-bit signed integer to convert.</param>
		/// <param name="toBase">The base of the return value, which must be 2, 8, 10, or 16.</param>
		/// <returns>The string representation of <paramref name="value" /> in base <paramref name="toBase" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="toBase" /> is not 2, 8, 10, or 16.</exception>
		public static string ToString(long value, int toBase)
		{
			if (toBase != 2 && toBase != 8 && toBase != 10 && toBase != 16)
			{
				throw new ArgumentException("Invalid Base.");
			}
			return ParseNumbers.LongToString(value, toBase, -1, ' ', 0);
		}

		/// <summary>Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64 digits.</summary>
		/// <param name="inArray">An array of 8-bit unsigned integers.</param>
		/// <returns>The string representation, in base 64, of the contents of <paramref name="inArray" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="inArray" /> is <see langword="null" />.</exception>
		public static string ToBase64String(byte[] inArray)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			return Convert.ToBase64String(new ReadOnlySpan<byte>(inArray), Base64FormattingOptions.None);
		}

		/// <summary>Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64 digits. A parameter specifies whether to insert line breaks in the return value.</summary>
		/// <param name="inArray">An array of 8-bit unsigned integers.</param>
		/// <param name="options">
		///   <see cref="F:System.Base64FormattingOptions.InsertLineBreaks" /> to insert a line break every 76 characters, or <see cref="F:System.Base64FormattingOptions.None" /> to not insert line breaks.</param>
		/// <returns>The string representation in base 64 of the elements in <paramref name="inArray" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="inArray" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="options" /> is not a valid <see cref="T:System.Base64FormattingOptions" /> value.</exception>
		public static string ToBase64String(byte[] inArray, Base64FormattingOptions options)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			return Convert.ToBase64String(new ReadOnlySpan<byte>(inArray), options);
		}

		/// <summary>Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64 digits. Parameters specify the subset as an offset in the input array, and the number of elements in the array to convert.</summary>
		/// <param name="inArray">An array of 8-bit unsigned integers.</param>
		/// <param name="offset">An offset in <paramref name="inArray" />.</param>
		/// <param name="length">The number of elements of <paramref name="inArray" /> to convert.</param>
		/// <returns>The string representation in base 64 of <paramref name="length" /> elements of <paramref name="inArray" />, starting at position <paramref name="offset" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="inArray" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="length" /> is negative.  
		/// -or-  
		/// <paramref name="offset" /> plus <paramref name="length" /> is greater than the length of <paramref name="inArray" />.</exception>
		public static string ToBase64String(byte[] inArray, int offset, int length)
		{
			return Convert.ToBase64String(inArray, offset, length, Base64FormattingOptions.None);
		}

		/// <summary>Converts a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base-64 digits. Parameters specify the subset as an offset in the input array, the number of elements in the array to convert, and whether to insert line breaks in the return value.</summary>
		/// <param name="inArray">An array of 8-bit unsigned integers.</param>
		/// <param name="offset">An offset in <paramref name="inArray" />.</param>
		/// <param name="length">The number of elements of <paramref name="inArray" /> to convert.</param>
		/// <param name="options">
		///   <see cref="F:System.Base64FormattingOptions.InsertLineBreaks" /> to insert a line break every 76 characters, or <see cref="F:System.Base64FormattingOptions.None" /> to not insert line breaks.</param>
		/// <returns>The string representation in base 64 of <paramref name="length" /> elements of <paramref name="inArray" />, starting at position <paramref name="offset" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="inArray" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="length" /> is negative.  
		/// -or-  
		/// <paramref name="offset" /> plus <paramref name="length" /> is greater than the length of <paramref name="inArray" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="options" /> is not a valid <see cref="T:System.Base64FormattingOptions" /> value.</exception>
		public static string ToBase64String(byte[] inArray, int offset, int length, Base64FormattingOptions options)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Value must be positive.");
			}
			if (offset > inArray.Length - length)
			{
				throw new ArgumentOutOfRangeException("offset", "Offset and length must refer to a position in the string.");
			}
			return Convert.ToBase64String(new ReadOnlySpan<byte>(inArray, offset, length), options);
		}

		public unsafe static string ToBase64String(ReadOnlySpan<byte> bytes, Base64FormattingOptions options = Base64FormattingOptions.None)
		{
			if (options < Base64FormattingOptions.None || options > Base64FormattingOptions.InsertLineBreaks)
			{
				throw new ArgumentException(string.Format("Illegal enum value: {0}.", (int)options), "options");
			}
			if (bytes.Length == 0)
			{
				return string.Empty;
			}
			bool insertLineBreaks = options == Base64FormattingOptions.InsertLineBreaks;
			string text = string.FastAllocateString(Convert.ToBase64_CalculateAndValidateOutputLength(bytes.Length, insertLineBreaks));
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(bytes))
			{
				byte* inData = reference;
				fixed (string text2 = text)
				{
					char* ptr = text2;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					Convert.ConvertToBase64Array(ptr, inData, 0, bytes.Length, insertLineBreaks);
				}
			}
			return text;
		}

		/// <summary>Converts a subset of an 8-bit unsigned integer array to an equivalent subset of a Unicode character array encoded with base-64 digits. Parameters specify the subsets as offsets in the input and output arrays, and the number of elements in the input array to convert.</summary>
		/// <param name="inArray">An input array of 8-bit unsigned integers.</param>
		/// <param name="offsetIn">A position within <paramref name="inArray" />.</param>
		/// <param name="length">The number of elements of <paramref name="inArray" /> to convert.</param>
		/// <param name="outArray">An output array of Unicode characters.</param>
		/// <param name="offsetOut">A position within <paramref name="outArray" />.</param>
		/// <returns>A 32-bit signed integer containing the number of bytes in <paramref name="outArray" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="inArray" /> or <paramref name="outArray" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offsetIn" />, <paramref name="offsetOut" />, or <paramref name="length" /> is negative.  
		/// -or-  
		/// <paramref name="offsetIn" /> plus <paramref name="length" /> is greater than the length of <paramref name="inArray" />.  
		/// -or-  
		/// <paramref name="offsetOut" /> plus the number of elements to return is greater than the length of <paramref name="outArray" />.</exception>
		public static int ToBase64CharArray(byte[] inArray, int offsetIn, int length, char[] outArray, int offsetOut)
		{
			return Convert.ToBase64CharArray(inArray, offsetIn, length, outArray, offsetOut, Base64FormattingOptions.None);
		}

		/// <summary>Converts a subset of an 8-bit unsigned integer array to an equivalent subset of a Unicode character array encoded with base-64 digits. Parameters specify the subsets as offsets in the input and output arrays, the number of elements in the input array to convert, and whether line breaks are inserted in the output array.</summary>
		/// <param name="inArray">An input array of 8-bit unsigned integers.</param>
		/// <param name="offsetIn">A position within <paramref name="inArray" />.</param>
		/// <param name="length">The number of elements of <paramref name="inArray" /> to convert.</param>
		/// <param name="outArray">An output array of Unicode characters.</param>
		/// <param name="offsetOut">A position within <paramref name="outArray" />.</param>
		/// <param name="options">
		///   <see cref="F:System.Base64FormattingOptions.InsertLineBreaks" /> to insert a line break every 76 characters, or <see cref="F:System.Base64FormattingOptions.None" /> to not insert line breaks.</param>
		/// <returns>A 32-bit signed integer containing the number of bytes in <paramref name="outArray" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="inArray" /> or <paramref name="outArray" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offsetIn" />, <paramref name="offsetOut" />, or <paramref name="length" /> is negative.  
		/// -or-  
		/// <paramref name="offsetIn" /> plus <paramref name="length" /> is greater than the length of <paramref name="inArray" />.  
		/// -or-  
		/// <paramref name="offsetOut" /> plus the number of elements to return is greater than the length of <paramref name="outArray" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="options" /> is not a valid <see cref="T:System.Base64FormattingOptions" /> value.</exception>
		public unsafe static int ToBase64CharArray(byte[] inArray, int offsetIn, int length, char[] outArray, int offsetOut, Base64FormattingOptions options)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (outArray == null)
			{
				throw new ArgumentNullException("outArray");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (offsetIn < 0)
			{
				throw new ArgumentOutOfRangeException("offsetIn", "Value must be positive.");
			}
			if (offsetOut < 0)
			{
				throw new ArgumentOutOfRangeException("offsetOut", "Value must be positive.");
			}
			if (options < Base64FormattingOptions.None || options > Base64FormattingOptions.InsertLineBreaks)
			{
				throw new ArgumentException(string.Format("Illegal enum value: {0}.", (int)options), "options");
			}
			int num = inArray.Length;
			if (offsetIn > num - length)
			{
				throw new ArgumentOutOfRangeException("offsetIn", "Offset and length must refer to a position in the string.");
			}
			if (num == 0)
			{
				return 0;
			}
			bool insertLineBreaks = options == Base64FormattingOptions.InsertLineBreaks;
			int num2 = outArray.Length;
			int num3 = Convert.ToBase64_CalculateAndValidateOutputLength(length, insertLineBreaks);
			if (offsetOut > num2 - num3)
			{
				throw new ArgumentOutOfRangeException("offsetOut", "Either offset did not refer to a position in the string, or there is an insufficient length of destination character array.");
			}
			int result;
			fixed (char* ptr = &outArray[offsetOut])
			{
				char* outChars = ptr;
				fixed (byte* ptr2 = &inArray[0])
				{
					byte* inData = ptr2;
					result = Convert.ConvertToBase64Array(outChars, inData, offsetIn, length, insertLineBreaks);
				}
			}
			return result;
		}

		public unsafe static bool TryToBase64Chars(ReadOnlySpan<byte> bytes, Span<char> chars, out int charsWritten, Base64FormattingOptions options = Base64FormattingOptions.None)
		{
			if (options < Base64FormattingOptions.None || options > Base64FormattingOptions.InsertLineBreaks)
			{
				throw new ArgumentException(string.Format("Illegal enum value: {0}.", (int)options), "options");
			}
			if (bytes.Length == 0)
			{
				charsWritten = 0;
				return true;
			}
			bool insertLineBreaks = options == Base64FormattingOptions.InsertLineBreaks;
			if (Convert.ToBase64_CalculateAndValidateOutputLength(bytes.Length, insertLineBreaks) > chars.Length)
			{
				charsWritten = 0;
				return false;
			}
			fixed (char* reference = MemoryMarshal.GetReference<char>(chars))
			{
				char* outChars = reference;
				fixed (byte* reference2 = MemoryMarshal.GetReference<byte>(bytes))
				{
					byte* inData = reference2;
					charsWritten = Convert.ConvertToBase64Array(outChars, inData, 0, bytes.Length, insertLineBreaks);
					return true;
				}
			}
		}

		private unsafe static int ConvertToBase64Array(char* outChars, byte* inData, int offset, int length, bool insertLineBreaks)
		{
			int num = length % 3;
			int num2 = offset + (length - num);
			int num3 = 0;
			int num4 = 0;
			fixed (char* ptr = &Convert.base64Table[0])
			{
				char* ptr2 = ptr;
				int i;
				for (i = offset; i < num2; i += 3)
				{
					if (insertLineBreaks)
					{
						if (num4 == 76)
						{
							outChars[num3++] = '\r';
							outChars[num3++] = '\n';
							num4 = 0;
						}
						num4 += 4;
					}
					outChars[num3] = ptr2[(inData[i] & 252) >> 2];
					outChars[num3 + 1] = ptr2[(int)(inData[i] & 3) << 4 | (inData[i + 1] & 240) >> 4];
					outChars[num3 + 2] = ptr2[(int)(inData[i + 1] & 15) << 2 | (inData[i + 2] & 192) >> 6];
					outChars[num3 + 3] = ptr2[inData[i + 2] & 63];
					num3 += 4;
				}
				i = num2;
				if (insertLineBreaks && num != 0 && num4 == 76)
				{
					outChars[num3++] = '\r';
					outChars[num3++] = '\n';
				}
				if (num != 1)
				{
					if (num == 2)
					{
						outChars[num3] = ptr2[(inData[i] & 252) >> 2];
						outChars[num3 + 1] = ptr2[(int)(inData[i] & 3) << 4 | (inData[i + 1] & 240) >> 4];
						outChars[num3 + 2] = ptr2[(inData[i + 1] & 15) << 2];
						outChars[num3 + 3] = ptr2[64];
						num3 += 4;
					}
				}
				else
				{
					outChars[num3] = ptr2[(inData[i] & 252) >> 2];
					outChars[num3 + 1] = ptr2[(inData[i] & 3) << 4];
					outChars[num3 + 2] = ptr2[64];
					outChars[num3 + 3] = ptr2[64];
					num3 += 4;
				}
			}
			return num3;
		}

		private static int ToBase64_CalculateAndValidateOutputLength(int inputLength, bool insertLineBreaks)
		{
			long num = (long)inputLength / 3L * 4L;
			num += ((inputLength % 3 != 0) ? 4L : 0L);
			if (num == 0L)
			{
				return 0;
			}
			if (insertLineBreaks)
			{
				long num2 = num / 76L;
				if (num % 76L == 0L)
				{
					num2 -= 1L;
				}
				num += num2 * 2L;
			}
			if (num > 2147483647L)
			{
				throw new OutOfMemoryException();
			}
			return (int)num;
		}

		/// <summary>Converts the specified string, which encodes binary data as base-64 digits, to an equivalent 8-bit unsigned integer array.</summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="s" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="s" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">The length of <paramref name="s" />, ignoring white-space characters, is not zero or a multiple of 4.  
		///  -or-  
		///  The format of <paramref name="s" /> is invalid. <paramref name="s" /> contains a non-base-64 character, more than two padding characters, or a non-white space-character among the padding characters.</exception>
		public unsafe static byte[] FromBase64String(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			char* ptr = s;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Convert.FromBase64CharPtr(ptr, s.Length);
		}

		public static bool TryFromBase64String(string s, Span<byte> bytes, out int bytesWritten)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return Convert.TryFromBase64Chars(s.AsSpan(), bytes, out bytesWritten);
		}

		public unsafe static bool TryFromBase64Chars(ReadOnlySpan<char> chars, Span<byte> bytes, out int bytesWritten)
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)8], 4);
			bytesWritten = 0;
			while (chars.Length != 0)
			{
				int start;
				int num;
				bool flag = Convert.TryDecodeFromUtf16(chars, bytes, out start, out num);
				bytesWritten += num;
				if (flag)
				{
					return true;
				}
				chars = chars.Slice(start);
				bytes = bytes.Slice(num);
				if (((char)(*chars[0])).IsSpace())
				{
					int num2 = 1;
					while (num2 != chars.Length && ((char)(*chars[num2])).IsSpace())
					{
						num2++;
					}
					chars = chars.Slice(num2);
					if (num % 3 != 0 && chars.Length != 0)
					{
						bytesWritten = 0;
						return false;
					}
				}
				else
				{
					int start2;
					int num3;
					Convert.CopyToTempBufferWithoutWhiteSpace(chars, span, out start2, out num3);
					if ((num3 & 3) != 0)
					{
						bytesWritten = 0;
						return false;
					}
					span = span.Slice(0, num3);
					int num4;
					int num5;
					if (!Convert.TryDecodeFromUtf16(span, bytes, out num4, out num5))
					{
						bytesWritten = 0;
						return false;
					}
					bytesWritten += num5;
					chars = chars.Slice(start2);
					bytes = bytes.Slice(num5);
					if (num5 % 3 != 0)
					{
						for (int i = 0; i < chars.Length; i++)
						{
							if (!((char)(*chars[i])).IsSpace())
							{
								bytesWritten = 0;
								return false;
							}
						}
						return true;
					}
				}
			}
			return true;
		}

		private unsafe static void CopyToTempBufferWithoutWhiteSpace(ReadOnlySpan<char> chars, Span<char> tempBuffer, out int consumed, out int charsWritten)
		{
			charsWritten = 0;
			for (int i = 0; i < chars.Length; i++)
			{
				char c = (char)(*chars[i]);
				if (!c.IsSpace())
				{
					int num = charsWritten;
					charsWritten = num + 1;
					*tempBuffer[num] = c;
					if (charsWritten == tempBuffer.Length)
					{
						consumed = i + 1;
						return;
					}
				}
			}
			consumed = chars.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsSpace(this char c)
		{
			return c == ' ' || c == '\t' || c == '\r' || c == '\n';
		}

		/// <summary>Converts a subset of a Unicode character array, which encodes binary data as base-64 digits, to an equivalent 8-bit unsigned integer array. Parameters specify the subset in the input array and the number of elements to convert.</summary>
		/// <param name="inArray">A Unicode character array.</param>
		/// <param name="offset">A position within <paramref name="inArray" />.</param>
		/// <param name="length">The number of elements in <paramref name="inArray" /> to convert.</param>
		/// <returns>An array of 8-bit unsigned integers equivalent to <paramref name="length" /> elements at position <paramref name="offset" /> in <paramref name="inArray" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="inArray" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="length" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> plus <paramref name="length" /> indicates a position not within <paramref name="inArray" />.</exception>
		/// <exception cref="T:System.FormatException">The length of <paramref name="inArray" />, ignoring white-space characters, is not zero or a multiple of 4.  
		///  -or-  
		///  The format of <paramref name="inArray" /> is invalid. <paramref name="inArray" /> contains a non-base-64 character, more than two padding characters, or a non-white-space character among the padding characters.</exception>
		public unsafe static byte[] FromBase64CharArray(char[] inArray, int offset, int length)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Value must be positive.");
			}
			if (offset > inArray.Length - length)
			{
				throw new ArgumentOutOfRangeException("offset", "Offset and length must refer to a position in the string.");
			}
			if (inArray.Length == 0)
			{
				return Array.Empty<byte>();
			}
			fixed (char* ptr = &inArray[0])
			{
				return Convert.FromBase64CharPtr(ptr + offset, length);
			}
		}

		private unsafe static byte[] FromBase64CharPtr(char* inputPtr, int inputLength)
		{
			while (inputLength > 0)
			{
				int num = (int)inputPtr[inputLength - 1];
				if (num != 32 && num != 10 && num != 13 && num != 9)
				{
					break;
				}
				inputLength--;
			}
			byte[] array = new byte[Convert.FromBase64_ComputeResultLength(inputPtr, inputLength)];
			int num2;
			if (!Convert.TryFromBase64Chars(new ReadOnlySpan<char>((void*)inputPtr, inputLength), array, out num2))
			{
				throw new FormatException("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.");
			}
			return array;
		}

		private unsafe static int FromBase64_ComputeResultLength(char* inputPtr, int inputLength)
		{
			char* ptr = inputPtr + inputLength;
			int num = inputLength;
			int num2 = 0;
			while (inputPtr < ptr)
			{
				uint num3 = (uint)(*inputPtr);
				inputPtr++;
				if (num3 <= 32U)
				{
					num--;
				}
				else if (num3 == 61U)
				{
					num--;
					num2++;
				}
			}
			if (num2 != 0)
			{
				if (num2 == 1)
				{
					num2 = 2;
				}
				else
				{
					if (num2 != 2)
					{
						throw new FormatException("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.");
					}
					num2 = 1;
				}
			}
			return num / 4 * 3 + num2;
		}

		private static readonly sbyte[] s_decodingMap = new sbyte[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			62,
			-1,
			-1,
			-1,
			63,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		private const byte EncodingPad = 61;

		internal static readonly Type[] ConvertTypes = new Type[]
		{
			typeof(Empty),
			typeof(object),
			typeof(DBNull),
			typeof(bool),
			typeof(char),
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(object),
			typeof(string)
		};

		private static readonly Type EnumType = typeof(Enum);

		internal static readonly char[] base64Table = new char[]
		{
			'A',
			'B',
			'C',
			'D',
			'E',
			'F',
			'G',
			'H',
			'I',
			'J',
			'K',
			'L',
			'M',
			'N',
			'O',
			'P',
			'Q',
			'R',
			'S',
			'T',
			'U',
			'V',
			'W',
			'X',
			'Y',
			'Z',
			'a',
			'b',
			'c',
			'd',
			'e',
			'f',
			'g',
			'h',
			'i',
			'j',
			'k',
			'l',
			'm',
			'n',
			'o',
			'p',
			'q',
			'r',
			's',
			't',
			'u',
			'v',
			'w',
			'x',
			'y',
			'z',
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'+',
			'/',
			'='
		};

		private const int base64LineBreakPosition = 76;

		/// <summary>A constant that represents a database column that is absent of data; that is, database null.</summary>
		public static readonly object DBNull = System.DBNull.Value;
	}
}
