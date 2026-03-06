using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace System.Xml
{
	/// <summary>Encodes and decodes XML names, and provides methods for converting between common language runtime types and XML Schema definition language (XSD) types. When converting data types, the values returned are locale-independent.</summary>
	public class XmlConvert
	{
		/// <summary>Converts the name to a valid XML name.</summary>
		/// <param name="name">A name to be translated. </param>
		/// <returns>Returns the name with any invalid characters replaced by an escape string.</returns>
		public static string EncodeName(string name)
		{
			return XmlConvert.EncodeName(name, true, false);
		}

		/// <summary>Verifies the name is valid according to the XML specification.</summary>
		/// <param name="name">The name to be encoded. </param>
		/// <returns>The encoded name.</returns>
		public static string EncodeNmToken(string name)
		{
			return XmlConvert.EncodeName(name, false, false);
		}

		/// <summary>Converts the name to a valid XML local name.</summary>
		/// <param name="name">The name to be encoded. </param>
		/// <returns>The encoded name.</returns>
		public static string EncodeLocalName(string name)
		{
			return XmlConvert.EncodeName(name, true, true);
		}

		/// <summary>Decodes a name. This method does the reverse of the <see cref="M:System.Xml.XmlConvert.EncodeName(System.String)" /> and <see cref="M:System.Xml.XmlConvert.EncodeLocalName(System.String)" /> methods.</summary>
		/// <param name="name">The name to be transformed. </param>
		/// <returns>The decoded name.</returns>
		public static string DecodeName(string name)
		{
			if (name == null || name.Length == 0)
			{
				return name;
			}
			StringBuilder stringBuilder = null;
			int length = name.Length;
			int num = 0;
			int num2 = name.IndexOf('_');
			if (num2 < 0)
			{
				return name;
			}
			if (XmlConvert.c_DecodeCharPattern == null)
			{
				XmlConvert.c_DecodeCharPattern = new Regex("_[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
			}
			IEnumerator enumerator = XmlConvert.c_DecodeCharPattern.Matches(name, num2).GetEnumerator();
			int num3 = -1;
			if (enumerator != null && enumerator.MoveNext())
			{
				num3 = ((Match)enumerator.Current).Index;
			}
			for (int i = 0; i < length - XmlConvert.c_EncodedCharLength + 1; i++)
			{
				if (i == num3)
				{
					if (enumerator.MoveNext())
					{
						num3 = ((Match)enumerator.Current).Index;
					}
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(length + 20);
					}
					stringBuilder.Append(name, num, i - num);
					if (name[i + 6] != '_')
					{
						int num4 = XmlConvert.FromHex(name[i + 2]) * 268435456 + XmlConvert.FromHex(name[i + 3]) * 16777216 + XmlConvert.FromHex(name[i + 4]) * 1048576 + XmlConvert.FromHex(name[i + 5]) * 65536 + XmlConvert.FromHex(name[i + 6]) * 4096 + XmlConvert.FromHex(name[i + 7]) * 256 + XmlConvert.FromHex(name[i + 8]) * 16 + XmlConvert.FromHex(name[i + 9]);
						if (num4 >= 65536)
						{
							if (num4 <= 1114111)
							{
								num = i + XmlConvert.c_EncodedCharLength + 4;
								char value;
								char value2;
								XmlCharType.SplitSurrogateChar(num4, out value, out value2);
								stringBuilder.Append(value2);
								stringBuilder.Append(value);
							}
						}
						else
						{
							num = i + XmlConvert.c_EncodedCharLength + 4;
							stringBuilder.Append((char)num4);
						}
						i += XmlConvert.c_EncodedCharLength - 1 + 4;
					}
					else
					{
						num = i + XmlConvert.c_EncodedCharLength;
						stringBuilder.Append((char)(XmlConvert.FromHex(name[i + 2]) * 4096 + XmlConvert.FromHex(name[i + 3]) * 256 + XmlConvert.FromHex(name[i + 4]) * 16 + XmlConvert.FromHex(name[i + 5])));
						i += XmlConvert.c_EncodedCharLength - 1;
					}
				}
			}
			if (num == 0)
			{
				return name;
			}
			if (num < length)
			{
				stringBuilder.Append(name, num, length - num);
			}
			return stringBuilder.ToString();
		}

		private static string EncodeName(string name, bool first, bool local)
		{
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}
			StringBuilder stringBuilder = null;
			int length = name.Length;
			int num = 0;
			int i = 0;
			int num2 = name.IndexOf('_');
			IEnumerator enumerator = null;
			if (num2 >= 0)
			{
				if (XmlConvert.c_EncodeCharPattern == null)
				{
					XmlConvert.c_EncodeCharPattern = new Regex("(?<=_)[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
				}
				enumerator = XmlConvert.c_EncodeCharPattern.Matches(name, num2).GetEnumerator();
			}
			int num3 = -1;
			if (enumerator != null && enumerator.MoveNext())
			{
				num3 = ((Match)enumerator.Current).Index - 1;
			}
			if (first && ((!XmlConvert.xmlCharType.IsStartNCNameCharXml4e(name[0]) && (local || (!local && name[0] != ':'))) || num3 == 0))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(length + 20);
				}
				stringBuilder.Append("_x");
				if (length > 1 && XmlCharType.IsHighSurrogate((int)name[0]) && XmlCharType.IsLowSurrogate((int)name[1]))
				{
					int highChar = (int)name[0];
					stringBuilder.Append(XmlCharType.CombineSurrogateChar((int)name[1], highChar).ToString("X8", CultureInfo.InvariantCulture));
					i++;
					num = 2;
				}
				else
				{
					stringBuilder.Append(((int)name[0]).ToString("X4", CultureInfo.InvariantCulture));
					num = 1;
				}
				stringBuilder.Append("_");
				i++;
				if (num3 == 0 && enumerator.MoveNext())
				{
					num3 = ((Match)enumerator.Current).Index - 1;
				}
			}
			while (i < length)
			{
				if ((local && !XmlConvert.xmlCharType.IsNCNameCharXml4e(name[i])) || (!local && !XmlConvert.xmlCharType.IsNameCharXml4e(name[i])) || num3 == i)
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(length + 20);
					}
					if (num3 == i && enumerator.MoveNext())
					{
						num3 = ((Match)enumerator.Current).Index - 1;
					}
					stringBuilder.Append(name, num, i - num);
					stringBuilder.Append("_x");
					if (length > i + 1 && XmlCharType.IsHighSurrogate((int)name[i]) && XmlCharType.IsLowSurrogate((int)name[i + 1]))
					{
						int highChar2 = (int)name[i];
						stringBuilder.Append(XmlCharType.CombineSurrogateChar((int)name[i + 1], highChar2).ToString("X8", CultureInfo.InvariantCulture));
						num = i + 2;
						i++;
					}
					else
					{
						stringBuilder.Append(((int)name[i]).ToString("X4", CultureInfo.InvariantCulture));
						num = i + 1;
					}
					stringBuilder.Append("_");
				}
				i++;
			}
			if (num == 0)
			{
				return name;
			}
			if (num < length)
			{
				stringBuilder.Append(name, num, length - num);
			}
			return stringBuilder.ToString();
		}

		private static int FromHex(char digit)
		{
			if (digit > '9')
			{
				return (int)(((digit <= 'F') ? (digit - 'A') : (digit - 'a')) + '\n');
			}
			return (int)(digit - '0');
		}

		internal static byte[] FromBinHexString(string s)
		{
			return XmlConvert.FromBinHexString(s, true);
		}

		internal static byte[] FromBinHexString(string s, bool allowOddCount)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return BinHexDecoder.Decode(s.ToCharArray(), allowOddCount);
		}

		internal static string ToBinHexString(byte[] inArray)
		{
			if (inArray == null)
			{
				throw new ArgumentNullException("inArray");
			}
			return BinHexEncoder.Encode(inArray, 0, inArray.Length);
		}

		/// <summary>Verifies that the name is a valid name according to the W3C Extended Markup Language recommendation.</summary>
		/// <param name="name">The name to verify. </param>
		/// <returns>The name, if it is a valid XML name.</returns>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="name" /> is not a valid XML name. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="name" /> is <see langword="null" /> or String.Empty. </exception>
		public static string VerifyName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentNullException("name", Res.GetString("The empty string '' is not a valid name."));
			}
			int num = ValidateNames.ParseNameNoNamespaces(name, 0);
			if (num != name.Length)
			{
				throw XmlConvert.CreateInvalidNameCharException(name, num, ExceptionType.XmlException);
			}
			return name;
		}

		internal static Exception TryVerifyName(string name)
		{
			if (name == null || name.Length == 0)
			{
				return new XmlException("The empty string '' is not a valid name.", string.Empty);
			}
			int num = ValidateNames.ParseNameNoNamespaces(name, 0);
			if (num != name.Length)
			{
				return new XmlException((num == 0) ? "Name cannot begin with the '{0}' character, hexadecimal value {1}." : "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(name, num));
			}
			return null;
		}

		internal static string VerifyQName(string name)
		{
			return XmlConvert.VerifyQName(name, ExceptionType.XmlException);
		}

		internal static string VerifyQName(string name, ExceptionType exceptionType)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentNullException("name");
			}
			int num = -1;
			int num2 = ValidateNames.ParseQName(name, 0, out num);
			if (num2 != name.Length)
			{
				throw XmlConvert.CreateException("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(name, num2), exceptionType, 0, num2 + 1);
			}
			return name;
		}

		/// <summary>Verifies that the name is a valid <see langword="NCName" /> according to the W3C Extended Markup Language recommendation. An <see langword="NCName" /> is a name that cannot contain a colon.</summary>
		/// <param name="name">The name to verify. </param>
		/// <returns>The name, if it is a valid NCName.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="name" /> is <see langword="null" /> or String.Empty. </exception>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="name" /> is not a valid non-colon name. </exception>
		public static string VerifyNCName(string name)
		{
			return XmlConvert.VerifyNCName(name, ExceptionType.XmlException);
		}

		internal static string VerifyNCName(string name, ExceptionType exceptionType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentNullException("name", Res.GetString("The empty string '' is not a valid local name."));
			}
			int num = ValidateNames.ParseNCName(name, 0);
			if (num != name.Length)
			{
				throw XmlConvert.CreateInvalidNameCharException(name, num, exceptionType);
			}
			return name;
		}

		internal static Exception TryVerifyNCName(string name)
		{
			int num = ValidateNames.ParseNCName(name);
			if (num == 0 || num != name.Length)
			{
				return ValidateNames.GetInvalidNameException(name, 0, num);
			}
			return null;
		}

		/// <summary>Verifies that the string is a valid token according to the W3C XML Schema Part2: Datatypes recommendation.</summary>
		/// <param name="token">The string value you wish to verify.</param>
		/// <returns>The token, if it is a valid token.</returns>
		/// <exception cref="T:System.Xml.XmlException">The string value is not a valid token.</exception>
		public static string VerifyTOKEN(string token)
		{
			if (token == null || token.Length == 0)
			{
				return token;
			}
			if (token[0] == ' ' || token[token.Length - 1] == ' ' || token.IndexOfAny(XmlConvert.crt) != -1 || token.IndexOf("  ", StringComparison.Ordinal) != -1)
			{
				throw new XmlException("line-feed (#xA) or tab (#x9) characters, leading or trailing spaces and sequences of one or more spaces (#x20) are not allowed in 'xs:token'.", token);
			}
			return token;
		}

		internal static Exception TryVerifyTOKEN(string token)
		{
			if (token == null || token.Length == 0)
			{
				return null;
			}
			if (token[0] == ' ' || token[token.Length - 1] == ' ' || token.IndexOfAny(XmlConvert.crt) != -1 || token.IndexOf("  ", StringComparison.Ordinal) != -1)
			{
				return new XmlException("line-feed (#xA) or tab (#x9) characters, leading or trailing spaces and sequences of one or more spaces (#x20) are not allowed in 'xs:token'.", token);
			}
			return null;
		}

		/// <summary>Verifies that the string is a valid NMTOKEN according to the W3C XML Schema Part2: Datatypes recommendation</summary>
		/// <param name="name">The string you wish to verify.</param>
		/// <returns>The name token, if it is a valid NMTOKEN.</returns>
		/// <exception cref="T:System.Xml.XmlException">The string is not a valid name token.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="name" /> is <see langword="null" />.</exception>
		public static string VerifyNMTOKEN(string name)
		{
			return XmlConvert.VerifyNMTOKEN(name, ExceptionType.XmlException);
		}

		internal static string VerifyNMTOKEN(string name, ExceptionType exceptionType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw XmlConvert.CreateException("Invalid NmToken value '{0}'.", name, exceptionType);
			}
			int num = ValidateNames.ParseNmtokenNoNamespaces(name, 0);
			if (num != name.Length)
			{
				throw XmlConvert.CreateException("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(name, num), exceptionType, 0, num + 1);
			}
			return name;
		}

		internal static Exception TryVerifyNMTOKEN(string name)
		{
			if (name == null || name.Length == 0)
			{
				return new XmlException("The empty string '' is not a valid name.", string.Empty);
			}
			int num = ValidateNames.ParseNmtokenNoNamespaces(name, 0);
			if (num != name.Length)
			{
				return new XmlException("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(name, num));
			}
			return null;
		}

		internal static string VerifyNormalizedString(string str)
		{
			if (str.IndexOfAny(XmlConvert.crt) != -1)
			{
				throw new XmlSchemaException("Carriage return (#xD), line feed (#xA), and tab (#x9) characters are not allowed in xs:normalizedString.", str);
			}
			return str;
		}

		internal static Exception TryVerifyNormalizedString(string str)
		{
			if (str.IndexOfAny(XmlConvert.crt) != -1)
			{
				return new XmlSchemaException("Carriage return (#xD), line feed (#xA), and tab (#x9) characters are not allowed in xs:normalizedString.", str);
			}
			return null;
		}

		/// <summary>Returns the passed-in string if all the characters and surrogate pair characters in the string argument are valid XML characters, otherwise an <see langword="XmlException" /> is thrown with information on the first invalid character encountered. </summary>
		/// <param name="content">
		///       <see cref="T:System.String" /> that contains characters to verify.</param>
		/// <returns>Returns the passed-in string if all the characters and surrogate-pair characters in the string argument are valid XML characters, otherwise an <see langword="XmlException" /> is thrown with information on the first invalid character encountered.</returns>
		public static string VerifyXmlChars(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			XmlConvert.VerifyCharData(content, ExceptionType.XmlException);
			return content;
		}

		/// <summary>Returns the passed in string instance if all the characters in the string argument are valid public id characters.</summary>
		/// <param name="publicId">
		///       <see cref="T:System.String" /> that contains the id to validate.</param>
		/// <returns>Returns the passed-in string if all the characters in the argument are valid public id characters.</returns>
		public static string VerifyPublicId(string publicId)
		{
			if (publicId == null)
			{
				throw new ArgumentNullException("publicId");
			}
			int num = XmlConvert.xmlCharType.IsPublicId(publicId);
			if (num != -1)
			{
				throw XmlConvert.CreateInvalidCharException(publicId, num, ExceptionType.XmlException);
			}
			return publicId;
		}

		/// <summary>Returns the passed-in string instance if all the characters in the string argument are valid whitespace characters. </summary>
		/// <param name="content">
		///       <see cref="T:System.String" /> to verify.</param>
		/// <returns>Returns the passed-in string instance if all the characters in the string argument are valid whitespace characters, otherwise <see langword="null" />.</returns>
		public static string VerifyWhitespace(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			int num = XmlConvert.xmlCharType.IsOnlyWhitespaceWithPos(content);
			if (num != -1)
			{
				throw new XmlException("The Whitespace or SignificantWhitespace node can contain only XML white space characters. '{0}' is not an XML white space character.", XmlException.BuildCharExceptionArgs(content, num), 0, num + 1);
			}
			return content;
		}

		/// <summary>Checks if the passed-in character is a valid Start Name Character type.</summary>
		/// <param name="ch">The character to validate.</param>
		/// <returns>
		///     <see langword="true" /> if the character is a valid Start Name Character type; otherwise, <see langword="false" />. </returns>
		public static bool IsStartNCNameChar(char ch)
		{
			return (XmlConvert.xmlCharType.charProperties[(int)ch] & 4) > 0;
		}

		/// <summary>Checks whether the passed-in character is a valid non-colon character type.</summary>
		/// <param name="ch">The character to verify as a non-colon character.</param>
		/// <returns>Returns <see langword="true" /> if the character is a valid non-colon character type; otherwise, <see langword="false" />.</returns>
		public static bool IsNCNameChar(char ch)
		{
			return (XmlConvert.xmlCharType.charProperties[(int)ch] & 8) > 0;
		}

		/// <summary>Checks if the passed-in character is a valid XML character.</summary>
		/// <param name="ch">The character to validate.</param>
		/// <returns>
		///     <see langword="true" /> if the passed in character is a valid XML character; otherwise <see langword="false" />.</returns>
		public static bool IsXmlChar(char ch)
		{
			return (XmlConvert.xmlCharType.charProperties[(int)ch] & 16) > 0;
		}

		/// <summary>Checks if the passed-in surrogate pair of characters is a valid XML character.</summary>
		/// <param name="lowChar">The surrogate character to validate.</param>
		/// <param name="highChar">The surrogate character to validate.</param>
		/// <returns>
		///     <see langword="true" /> if the passed in surrogate pair of characters is a valid XML character; otherwise <see langword="false" />.</returns>
		public static bool IsXmlSurrogatePair(char lowChar, char highChar)
		{
			return XmlCharType.IsHighSurrogate((int)highChar) && XmlCharType.IsLowSurrogate((int)lowChar);
		}

		/// <summary>Returns the passed-in character instance if the character in the argument is a valid public id character, otherwise <see langword="null" />.</summary>
		/// <param name="ch">
		///       <see cref="T:System.Char" /> object to validate.</param>
		/// <returns>Returns the passed-in character if the character is a valid public id character, otherwise <see langword="null" />.</returns>
		public static bool IsPublicIdChar(char ch)
		{
			return XmlConvert.xmlCharType.IsPubidChar(ch);
		}

		/// <summary>Checks if the passed-in character is a valid XML whitespace character.</summary>
		/// <param name="ch">The character to validate.</param>
		/// <returns>
		///     <see langword="true" /> if the passed in character is a valid XML whitespace character; otherwise <see langword="false" />.</returns>
		public static bool IsWhitespaceChar(char ch)
		{
			return (XmlConvert.xmlCharType.charProperties[(int)ch] & 1) > 0;
		}

		/// <summary>Converts the <see cref="T:System.Boolean" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Boolean" />, that is, "true" or "false".</returns>
		public static string ToString(bool value)
		{
			if (!value)
			{
				return "false";
			}
			return "true";
		}

		/// <summary>Converts the <see cref="T:System.Char" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Char" />.</returns>
		public static string ToString(char value)
		{
			return value.ToString(null);
		}

		/// <summary>Converts the <see cref="T:System.Decimal" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Decimal" />.</returns>
		public static string ToString(decimal value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.SByte" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="SByte" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(sbyte value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.Int16" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Int16" />.</returns>
		public static string ToString(short value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.Int32" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Int32" />.</returns>
		public static string ToString(int value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.Int64" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Int64" />.</returns>
		public static string ToString(long value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.Byte" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Byte" />.</returns>
		public static string ToString(byte value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.UInt16" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="UInt16" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(ushort value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.UInt32" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="UInt32" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(uint value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.UInt64" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="UInt64" />.</returns>
		[CLSCompliant(false)]
		public static string ToString(ulong value)
		{
			return value.ToString(null, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.Single" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Single" />.</returns>
		public static string ToString(float value)
		{
			if (float.IsNegativeInfinity(value))
			{
				return "-INF";
			}
			if (float.IsPositiveInfinity(value))
			{
				return "INF";
			}
			if (XmlConvert.IsNegativeZero((double)value))
			{
				return "-0";
			}
			return value.ToString("R", NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.Double" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Double" />.</returns>
		public static string ToString(double value)
		{
			if (double.IsNegativeInfinity(value))
			{
				return "-INF";
			}
			if (double.IsPositiveInfinity(value))
			{
				return "INF";
			}
			if (XmlConvert.IsNegativeZero(value))
			{
				return "-0";
			}
			return value.ToString("R", NumberFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.TimeSpan" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="TimeSpan" />.</returns>
		public static string ToString(TimeSpan value)
		{
			return new XsdDuration(value).ToString();
		}

		/// <summary>Converts the <see cref="T:System.DateTime" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="DateTime" /> in the format yyyy-MM-ddTHH:mm:ss where 'T' is a constant literal.</returns>
		[Obsolete("Use XmlConvert.ToString() that takes in XmlDateTimeSerializationMode")]
		public static string ToString(DateTime value)
		{
			return XmlConvert.ToString(value, "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz");
		}

		/// <summary>Converts the <see cref="T:System.DateTime" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <param name="format">The format structure that defines how to display the converted string. Valid formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets. </param>
		/// <returns>A string representation of the <see langword="DateTime" /> in the specified format.</returns>
		public static string ToString(DateTime value, string format)
		{
			return value.ToString(format, DateTimeFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.DateTime" /> to a <see cref="T:System.String" /> using the <see cref="T:System.Xml.XmlDateTimeSerializationMode" /> specified.</summary>
		/// <param name="value">The <see cref="T:System.DateTime" /> value to convert.</param>
		/// <param name="dateTimeOption">One of the <see cref="T:System.Xml.XmlDateTimeSerializationMode" /> values that specify how to treat the <see cref="T:System.DateTime" /> value.</param>
		/// <returns>A <see cref="T:System.String" /> equivalent of the <see cref="T:System.DateTime" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="dateTimeOption" /> value is not valid.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> or <paramref name="dateTimeOption" /> value is <see langword="null" />.</exception>
		public static string ToString(DateTime value, XmlDateTimeSerializationMode dateTimeOption)
		{
			switch (dateTimeOption)
			{
			case XmlDateTimeSerializationMode.Local:
				value = XmlConvert.SwitchToLocalTime(value);
				break;
			case XmlDateTimeSerializationMode.Utc:
				value = XmlConvert.SwitchToUtcTime(value);
				break;
			case XmlDateTimeSerializationMode.Unspecified:
				value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
				break;
			case XmlDateTimeSerializationMode.RoundtripKind:
				break;
			default:
				throw new ArgumentException(Res.GetString("The '{0}' value for the 'dateTimeOption' parameter is not an allowed value for the 'XmlDateTimeSerializationMode' enumeration.", new object[]
				{
					dateTimeOption,
					"dateTimeOption"
				}));
			}
			XsdDateTime xsdDateTime = new XsdDateTime(value, XsdDateTimeFlags.DateTime);
			return xsdDateTime.ToString();
		}

		/// <summary>Converts the supplied <see cref="T:System.DateTimeOffset" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The <see cref="T:System.DateTimeOffset" /> to be converted.</param>
		/// <returns>A <see cref="T:System.String" /> representation of the supplied <see cref="T:System.DateTimeOffset" />.</returns>
		public static string ToString(DateTimeOffset value)
		{
			XsdDateTime xsdDateTime = new XsdDateTime(value);
			return xsdDateTime.ToString();
		}

		/// <summary>Converts the supplied <see cref="T:System.DateTimeOffset" /> to a <see cref="T:System.String" /> in the specified format.</summary>
		/// <param name="value">The <see cref="T:System.DateTimeOffset" /> to be converted.</param>
		/// <param name="format">The format to which <paramref name="s" /> is converted. The format parameter can be any subset of the W3C Recommendation for the XML dateTime type. (For more information see http://www.w3.org/TR/xmlschema-2/#dateTime.)</param>
		/// <returns>A <see cref="T:System.String" /> representation in the specified format of the supplied <see cref="T:System.DateTimeOffset" />.</returns>
		public static string ToString(DateTimeOffset value, string format)
		{
			return value.ToString(format, DateTimeFormatInfo.InvariantInfo);
		}

		/// <summary>Converts the <see cref="T:System.Guid" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="value">The value to convert. </param>
		/// <returns>A string representation of the <see langword="Guid" />.</returns>
		public static string ToString(Guid value)
		{
			return value.ToString();
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Boolean" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="Boolean" /> value, that is, <see langword="true" /> or <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> does not represent a <see langword="Boolean" /> value. </exception>
		public static bool ToBoolean(string s)
		{
			s = XmlConvert.TrimString(s);
			if (s == "1" || s == "true")
			{
				return true;
			}
			if (s == "0" || s == "false")
			{
				return false;
			}
			throw new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
			{
				s,
				"Boolean"
			}));
		}

		internal static Exception TryToBoolean(string s, out bool result)
		{
			s = XmlConvert.TrimString(s);
			if (s == "0" || s == "false")
			{
				result = false;
				return null;
			}
			if (s == "1" || s == "true")
			{
				result = true;
				return null;
			}
			result = false;
			return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
			{
				s,
				"Boolean"
			}));
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Char" /> equivalent.</summary>
		/// <param name="s">The string containing a single character to convert. </param>
		/// <returns>A <see langword="Char" /> representing the single character.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="s" /> parameter is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">The <paramref name="s" /> parameter contains more than one character. </exception>
		public static char ToChar(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (s.Length != 1)
			{
				throw new FormatException(Res.GetString("String must be exactly one character long."));
			}
			return s[0];
		}

		internal static Exception TryToChar(string s, out char result)
		{
			if (!char.TryParse(s, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Char"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Decimal" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="Decimal" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.Decimal.MinValue" /> or greater than <see cref="F:System.Decimal.MaxValue" />. </exception>
		public static decimal ToDecimal(string s)
		{
			return decimal.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToDecimal(string s, out decimal result)
		{
			if (!decimal.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Decimal"
				}));
			}
			return null;
		}

		internal static decimal ToInteger(string s)
		{
			return decimal.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToInteger(string s, out decimal result)
		{
			if (!decimal.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Integer"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.SByte" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>An <see langword="SByte" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.SByte.MinValue" /> or greater than <see cref="F:System.SByte.MaxValue" />. </exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(string s)
		{
			return sbyte.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToSByte(string s, out sbyte result)
		{
			if (!sbyte.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"SByte"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Int16" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>An <see langword="Int16" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.Int16.MinValue" /> or greater than <see cref="F:System.Int16.MaxValue" />. </exception>
		public static short ToInt16(string s)
		{
			return short.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToInt16(string s, out short result)
		{
			if (!short.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Int16"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Int32" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>An <see langword="Int32" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />. </exception>
		public static int ToInt32(string s)
		{
			return int.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToInt32(string s, out int result)
		{
			if (!int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Int32"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Int64" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>An <see langword="Int64" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.Int64.MinValue" /> or greater than <see cref="F:System.Int64.MaxValue" />. </exception>
		public static long ToInt64(string s)
		{
			return long.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToInt64(string s, out long result)
		{
			if (!long.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Int64"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Byte" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="Byte" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.Byte.MinValue" /> or greater than <see cref="F:System.Byte.MaxValue" />. </exception>
		public static byte ToByte(string s)
		{
			return byte.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToByte(string s, out byte result)
		{
			if (!byte.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Byte"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.UInt16" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="UInt16" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.UInt16.MinValue" /> or greater than <see cref="F:System.UInt16.MaxValue" />. </exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(string s)
		{
			return ushort.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToUInt16(string s, out ushort result)
		{
			if (!ushort.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"UInt16"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.UInt32" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="UInt32" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.UInt32.MinValue" /> or greater than <see cref="F:System.UInt32.MaxValue" />. </exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(string s)
		{
			return uint.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToUInt32(string s, out uint result)
		{
			if (!uint.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"UInt32"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.UInt64" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="UInt64" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.UInt64.MinValue" /> or greater than <see cref="F:System.UInt64.MaxValue" />. </exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(string s)
		{
			return ulong.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
		}

		internal static Exception TryToUInt64(string s, out ulong result)
		{
			if (!ulong.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"UInt64"
				}));
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Single" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="Single" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.Single.MinValue" /> or greater than <see cref="F:System.Single.MaxValue" />. </exception>
		public static float ToSingle(string s)
		{
			s = XmlConvert.TrimString(s);
			if (s == "-INF")
			{
				return float.NegativeInfinity;
			}
			if (s == "INF")
			{
				return float.PositiveInfinity;
			}
			float num = float.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
			if (num == 0f && s[0] == '-')
			{
				return --0f;
			}
			return num;
		}

		internal static Exception TryToSingle(string s, out float result)
		{
			s = XmlConvert.TrimString(s);
			if (s == "-INF")
			{
				result = float.NegativeInfinity;
				return null;
			}
			if (s == "INF")
			{
				result = float.PositiveInfinity;
				return null;
			}
			if (!float.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Single"
				}));
			}
			if (result == 0f && s[0] == '-')
			{
				result = --0f;
			}
			return null;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Double" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="Double" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in the correct format. </exception>
		/// <exception cref="T:System.OverflowException">
		///         <paramref name="s" /> represents a number less than <see cref="F:System.Double.MinValue" /> or greater than <see cref="F:System.Double.MaxValue" />. </exception>
		public static double ToDouble(string s)
		{
			s = XmlConvert.TrimString(s);
			if (s == "-INF")
			{
				return double.NegativeInfinity;
			}
			if (s == "INF")
			{
				return double.PositiveInfinity;
			}
			double num = double.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
			if (num == 0.0 && s[0] == '-')
			{
				return --0.0;
			}
			return num;
		}

		internal static Exception TryToDouble(string s, out double result)
		{
			s = XmlConvert.TrimString(s);
			if (s == "-INF")
			{
				result = double.NegativeInfinity;
				return null;
			}
			if (s == "INF")
			{
				result = double.PositiveInfinity;
				return null;
			}
			if (!double.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Double"
				}));
			}
			if (result == 0.0 && s[0] == '-')
			{
				result = --0.0;
			}
			return null;
		}

		internal static double ToXPathDouble(object o)
		{
			string text = o as string;
			if (text != null)
			{
				text = XmlConvert.TrimString(text);
				double result;
				if (text.Length != 0 && text[0] != '+' && double.TryParse(text, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out result))
				{
					return result;
				}
				return double.NaN;
			}
			else
			{
				if (o is double)
				{
					return (double)o;
				}
				if (!(o is bool))
				{
					try
					{
						return Convert.ToDouble(o, NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException)
					{
					}
					catch (OverflowException)
					{
					}
					catch (ArgumentNullException)
					{
					}
					return double.NaN;
				}
				if (!(bool)o)
				{
					return 0.0;
				}
				return 1.0;
			}
		}

		internal static string ToXPathString(object value)
		{
			string text = value as string;
			if (text != null)
			{
				return text;
			}
			if (value is double)
			{
				return ((double)value).ToString("R", NumberFormatInfo.InvariantInfo);
			}
			if (!(value is bool))
			{
				return Convert.ToString(value, NumberFormatInfo.InvariantInfo);
			}
			if (!(bool)value)
			{
				return "false";
			}
			return "true";
		}

		internal static double XPathRound(double value)
		{
			double num = Math.Round(value);
			if (value - num != 0.5)
			{
				return num;
			}
			return num + 1.0;
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.TimeSpan" /> equivalent.</summary>
		/// <param name="s">The string to convert. The string format must conform to the W3C XML Schema Part 2: Datatypes recommendation for duration.</param>
		/// <returns>A <see langword="TimeSpan" /> equivalent of the string.</returns>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is not in correct format to represent a <see langword="TimeSpan" /> value. </exception>
		public static TimeSpan ToTimeSpan(string s)
		{
			XsdDuration xsdDuration;
			try
			{
				xsdDuration = new XsdDuration(s);
			}
			catch (Exception)
			{
				throw new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"TimeSpan"
				}));
			}
			return xsdDuration.ToTimeSpan();
		}

		internal static Exception TryToTimeSpan(string s, out TimeSpan result)
		{
			XsdDuration xsdDuration;
			Exception ex = XsdDuration.TryParse(s, out xsdDuration);
			if (ex != null)
			{
				result = TimeSpan.MinValue;
				return ex;
			}
			return xsdDuration.TryToTimeSpan(out result);
		}

		private static string[] AllDateTimeFormats
		{
			get
			{
				if (XmlConvert.s_allDateTimeFormats == null)
				{
					XmlConvert.CreateAllDateTimeFormats();
				}
				return XmlConvert.s_allDateTimeFormats;
			}
		}

		private static void CreateAllDateTimeFormats()
		{
			if (XmlConvert.s_allDateTimeFormats == null)
			{
				XmlConvert.s_allDateTimeFormats = new string[]
				{
					"yyyy-MM-ddTHH:mm:ss.FFFFFFFzzzzzz",
					"yyyy-MM-ddTHH:mm:ss.FFFFFFF",
					"yyyy-MM-ddTHH:mm:ss.FFFFFFFZ",
					"HH:mm:ss.FFFFFFF",
					"HH:mm:ss.FFFFFFFZ",
					"HH:mm:ss.FFFFFFFzzzzzz",
					"yyyy-MM-dd",
					"yyyy-MM-ddZ",
					"yyyy-MM-ddzzzzzz",
					"yyyy-MM",
					"yyyy-MMZ",
					"yyyy-MMzzzzzz",
					"yyyy",
					"yyyyZ",
					"yyyyzzzzzz",
					"--MM-dd",
					"--MM-ddZ",
					"--MM-ddzzzzzz",
					"---dd",
					"---ddZ",
					"---ddzzzzzz",
					"--MM--",
					"--MM--Z",
					"--MM--zzzzzz"
				};
			}
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.DateTime" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="DateTime" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is an empty string or is not in the correct format. </exception>
		[Obsolete("Use XmlConvert.ToDateTime() that takes in XmlDateTimeSerializationMode")]
		public static DateTime ToDateTime(string s)
		{
			return XmlConvert.ToDateTime(s, XmlConvert.AllDateTimeFormats);
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.DateTime" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <param name="format">The format structure to apply to the converted <see langword="DateTime" />. Valid formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets. The string is validated against this format. </param>
		/// <returns>A <see langword="DateTime" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> or <paramref name="format" /> is String.Empty -or- 
		///         <paramref name="s" /> does not contain a date and time that corresponds to <paramref name="format" />. </exception>
		public static DateTime ToDateTime(string s, string format)
		{
			return DateTime.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.DateTime" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <param name="formats">An array containing the format structures to apply to the converted <see langword="DateTime" />. Valid formats include "yyyy-MM-ddTHH:mm:sszzzzzz" and its subsets. </param>
		/// <returns>A <see langword="DateTime" /> equivalent of the string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> or an element of <paramref name="formats" /> is String.Empty -or- 
		///         <paramref name="s" /> does not contain a date and time that corresponds to any of the elements of <paramref name="formats" />. </exception>
		public static DateTime ToDateTime(string s, string[] formats)
		{
			return DateTime.ParseExact(s, formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.DateTime" /> using the <see cref="T:System.Xml.XmlDateTimeSerializationMode" /> specified</summary>
		/// <param name="s">The <see cref="T:System.String" /> value to convert.</param>
		/// <param name="dateTimeOption">One of the <see cref="T:System.Xml.XmlDateTimeSerializationMode" /> values that specify whether the date should be converted to local time or preserved as Coordinated Universal Time (UTC), if it is a UTC date.</param>
		/// <returns>A <see cref="T:System.DateTime" /> equivalent of the <see cref="T:System.String" />.</returns>
		/// <exception cref="T:System.NullReferenceException">
		///         <paramref name="s" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="dateTimeOption" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> is an empty string or is not in a valid format.</exception>
		public static DateTime ToDateTime(string s, XmlDateTimeSerializationMode dateTimeOption)
		{
			DateTime dateTime = new XsdDateTime(s, XsdDateTimeFlags.AllXsd);
			switch (dateTimeOption)
			{
			case XmlDateTimeSerializationMode.Local:
				dateTime = XmlConvert.SwitchToLocalTime(dateTime);
				break;
			case XmlDateTimeSerializationMode.Utc:
				dateTime = XmlConvert.SwitchToUtcTime(dateTime);
				break;
			case XmlDateTimeSerializationMode.Unspecified:
				dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Unspecified);
				break;
			case XmlDateTimeSerializationMode.RoundtripKind:
				break;
			default:
				throw new ArgumentException(Res.GetString("The '{0}' value for the 'dateTimeOption' parameter is not an allowed value for the 'XmlDateTimeSerializationMode' enumeration.", new object[]
				{
					dateTimeOption,
					"dateTimeOption"
				}));
			}
			return dateTime;
		}

		/// <summary>Converts the supplied <see cref="T:System.String" /> to a <see cref="T:System.DateTimeOffset" /> equivalent.</summary>
		/// <param name="s">The string to convert.
		///       Note   The string must conform to a subset of the W3C Recommendation for the XML dateTime type. For more information see http://www.w3.org/TR/xmlschema-2/#dateTime.</param>
		/// <returns>The <see cref="T:System.DateTimeOffset" /> equivalent of the supplied string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The argument passed to this method is outside the range of allowable values. For information about allowable values, see <see cref="T:System.DateTimeOffset" />.</exception>
		/// <exception cref="T:System.FormatException">The argument passed to this method does not conform to a subset of the W3C Recommendations for the XML dateTime type. For more information see http://www.w3.org/TR/xmlschema-2/#dateTime.</exception>
		public static DateTimeOffset ToDateTimeOffset(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return new XsdDateTime(s, XsdDateTimeFlags.AllXsd);
		}

		/// <summary>Converts the supplied <see cref="T:System.String" /> to a <see cref="T:System.DateTimeOffset" /> equivalent.</summary>
		/// <param name="s">The string to convert.</param>
		/// <param name="format">The format from which <paramref name="s" /> is converted. The format parameter can be any subset of the W3C Recommendation for the XML dateTime type. (For more information see http://www.w3.org/TR/xmlschema-2/#dateTime.) The string <paramref name="s" /> is validated against this format.</param>
		/// <returns>The <see cref="T:System.DateTimeOffset" /> equivalent of the supplied string.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="s" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.FormatException">
		///         <paramref name="s" /> or <paramref name="format" /> is an empty string or is not in the specified format.</exception>
		public static DateTimeOffset ToDateTimeOffset(string s, string format)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return DateTimeOffset.ParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
		}

		/// <summary>Converts the supplied <see cref="T:System.String" /> to a <see cref="T:System.DateTimeOffset" /> equivalent.</summary>
		/// <param name="s">The string to convert.</param>
		/// <param name="formats">An array of formats from which <paramref name="s" /> can be converted. Each format in <paramref name="formats" /> can be any subset of the W3C Recommendation for the XML dateTime type. (For more information see http://www.w3.org/TR/xmlschema-2/#dateTime.) The string <paramref name="s" /> is validated against one of these formats.</param>
		/// <returns>The <see cref="T:System.DateTimeOffset" /> equivalent of the supplied string.</returns>
		public static DateTimeOffset ToDateTimeOffset(string s, string[] formats)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return DateTimeOffset.ParseExact(s, formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
		}

		/// <summary>Converts the <see cref="T:System.String" /> to a <see cref="T:System.Guid" /> equivalent.</summary>
		/// <param name="s">The string to convert. </param>
		/// <returns>A <see langword="Guid" /> equivalent of the string.</returns>
		public static Guid ToGuid(string s)
		{
			return new Guid(s);
		}

		internal static Exception TryToGuid(string s, out Guid result)
		{
			Exception result2 = null;
			result = Guid.Empty;
			try
			{
				result = new Guid(s);
			}
			catch (ArgumentException)
			{
				result2 = new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Guid"
				}));
			}
			catch (FormatException)
			{
				result2 = new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Guid"
				}));
			}
			return result2;
		}

		private static DateTime SwitchToLocalTime(DateTime value)
		{
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
				return new DateTime(value.Ticks, DateTimeKind.Local);
			case DateTimeKind.Utc:
				return value.ToLocalTime();
			case DateTimeKind.Local:
				return value;
			default:
				return value;
			}
		}

		private static DateTime SwitchToUtcTime(DateTime value)
		{
			switch (value.Kind)
			{
			case DateTimeKind.Unspecified:
				return new DateTime(value.Ticks, DateTimeKind.Utc);
			case DateTimeKind.Utc:
				return value;
			case DateTimeKind.Local:
				return value.ToUniversalTime();
			default:
				return value;
			}
		}

		internal static Uri ToUri(string s)
		{
			if (s != null && s.Length > 0)
			{
				s = XmlConvert.TrimString(s);
				if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1)
				{
					throw new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
					{
						s,
						"Uri"
					}));
				}
			}
			Uri result;
			if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result))
			{
				throw new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Uri"
				}));
			}
			return result;
		}

		internal static Exception TryToUri(string s, out Uri result)
		{
			result = null;
			if (s != null && s.Length > 0)
			{
				s = XmlConvert.TrimString(s);
				if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1)
				{
					return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
					{
						s,
						"Uri"
					}));
				}
			}
			if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result))
			{
				return new FormatException(Res.GetString("The string '{0}' is not a valid {1} value.", new object[]
				{
					s,
					"Uri"
				}));
			}
			return null;
		}

		internal static bool StrEqual(char[] chars, int strPos1, int strLen1, string str2)
		{
			if (strLen1 != str2.Length)
			{
				return false;
			}
			int num = 0;
			while (num < strLen1 && chars[strPos1 + num] == str2[num])
			{
				num++;
			}
			return num == strLen1;
		}

		internal static string TrimString(string value)
		{
			return value.Trim(XmlConvert.WhitespaceChars);
		}

		internal static string TrimStringStart(string value)
		{
			return value.TrimStart(XmlConvert.WhitespaceChars);
		}

		internal static string TrimStringEnd(string value)
		{
			return value.TrimEnd(XmlConvert.WhitespaceChars);
		}

		internal static string[] SplitString(string value)
		{
			return value.Split(XmlConvert.WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);
		}

		internal static string[] SplitString(string value, StringSplitOptions splitStringOptions)
		{
			return value.Split(XmlConvert.WhitespaceChars, splitStringOptions);
		}

		internal static bool IsNegativeZero(double value)
		{
			return value == 0.0 && XmlConvert.DoubleToInt64Bits(value) == XmlConvert.DoubleToInt64Bits(--0.0);
		}

		private unsafe static long DoubleToInt64Bits(double value)
		{
			return *(long*)(&value);
		}

		internal static void VerifyCharData(string data, ExceptionType exceptionType)
		{
			XmlConvert.VerifyCharData(data, exceptionType, exceptionType);
		}

		internal static void VerifyCharData(string data, ExceptionType invCharExceptionType, ExceptionType invSurrogateExceptionType)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			int num = 0;
			int length = data.Length;
			for (;;)
			{
				if (num >= length || (XmlConvert.xmlCharType.charProperties[(int)data[num]] & 16) == 0)
				{
					if (num == length)
					{
						break;
					}
					if (!XmlCharType.IsHighSurrogate((int)data[num]))
					{
						goto IL_90;
					}
					if (num + 1 == length)
					{
						goto Block_5;
					}
					if (!XmlCharType.IsLowSurrogate((int)data[num + 1]))
					{
						goto IL_75;
					}
					num += 2;
				}
				else
				{
					num++;
				}
			}
			return;
			Block_5:
			throw XmlConvert.CreateException("The surrogate pair is invalid. Missing a low surrogate character.", invSurrogateExceptionType, 0, num + 1);
			IL_75:
			throw XmlConvert.CreateInvalidSurrogatePairException(data[num + 1], data[num], invSurrogateExceptionType, 0, num + 1);
			IL_90:
			throw XmlConvert.CreateInvalidCharException(data, num, invCharExceptionType);
		}

		internal static void VerifyCharData(char[] data, int offset, int len, ExceptionType exceptionType)
		{
			if (data == null || len == 0)
			{
				return;
			}
			int num = offset;
			int num2 = offset + len;
			for (;;)
			{
				if (num >= num2 || (XmlConvert.xmlCharType.charProperties[(int)data[num]] & 16) == 0)
				{
					if (num == num2)
					{
						break;
					}
					if (!XmlCharType.IsHighSurrogate((int)data[num]))
					{
						goto IL_78;
					}
					if (num + 1 == num2)
					{
						goto Block_5;
					}
					if (!XmlCharType.IsLowSurrogate((int)data[num + 1]))
					{
						goto IL_63;
					}
					num += 2;
				}
				else
				{
					num++;
				}
			}
			return;
			Block_5:
			throw XmlConvert.CreateException("The surrogate pair is invalid. Missing a low surrogate character.", exceptionType, 0, offset - num + 1);
			IL_63:
			throw XmlConvert.CreateInvalidSurrogatePairException(data[num + 1], data[num], exceptionType, 0, offset - num + 1);
			IL_78:
			throw XmlConvert.CreateInvalidCharException(data, len, num, exceptionType);
		}

		internal static string EscapeValueForDebuggerDisplay(string value)
		{
			StringBuilder stringBuilder = null;
			int i = 0;
			int num = 0;
			while (i < value.Length)
			{
				char c = value[i];
				if (c < ' ' || c == '"')
				{
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder(value.Length + 4);
					}
					if (i - num > 0)
					{
						stringBuilder.Append(value, num, i - num);
					}
					num = i + 1;
					switch (c)
					{
					case '\t':
						stringBuilder.Append("\\t");
						goto IL_A9;
					case '\n':
						stringBuilder.Append("\\n");
						goto IL_A9;
					case '\v':
					case '\f':
						break;
					case '\r':
						stringBuilder.Append("\\r");
						goto IL_A9;
					default:
						if (c == '"')
						{
							stringBuilder.Append("\\\"");
							goto IL_A9;
						}
						break;
					}
					stringBuilder.Append(c);
				}
				IL_A9:
				i++;
			}
			if (stringBuilder == null)
			{
				return value;
			}
			if (i - num > 0)
			{
				stringBuilder.Append(value, num, i - num);
			}
			return stringBuilder.ToString();
		}

		internal static Exception CreateException(string res, ExceptionType exceptionType)
		{
			return XmlConvert.CreateException(res, exceptionType, 0, 0);
		}

		internal static Exception CreateException(string res, ExceptionType exceptionType, int lineNo, int linePos)
		{
			if (exceptionType != ExceptionType.ArgumentException)
			{
				if (exceptionType != ExceptionType.XmlException)
				{
				}
				return new XmlException(res, string.Empty, lineNo, linePos);
			}
			return new ArgumentException(Res.GetString(res));
		}

		internal static Exception CreateException(string res, string arg, ExceptionType exceptionType)
		{
			return XmlConvert.CreateException(res, arg, exceptionType, 0, 0);
		}

		internal static Exception CreateException(string res, string arg, ExceptionType exceptionType, int lineNo, int linePos)
		{
			if (exceptionType != ExceptionType.ArgumentException)
			{
				if (exceptionType != ExceptionType.XmlException)
				{
				}
				return new XmlException(res, arg, lineNo, linePos);
			}
			return new ArgumentException(Res.GetString(res, new object[]
			{
				arg
			}));
		}

		internal static Exception CreateException(string res, string[] args, ExceptionType exceptionType)
		{
			return XmlConvert.CreateException(res, args, exceptionType, 0, 0);
		}

		internal static Exception CreateException(string res, string[] args, ExceptionType exceptionType, int lineNo, int linePos)
		{
			if (exceptionType != ExceptionType.ArgumentException)
			{
				if (exceptionType != ExceptionType.XmlException)
				{
				}
				return new XmlException(res, args, lineNo, linePos);
			}
			return new ArgumentException(Res.GetString(res, args));
		}

		internal static Exception CreateInvalidSurrogatePairException(char low, char hi)
		{
			return XmlConvert.CreateInvalidSurrogatePairException(low, hi, ExceptionType.ArgumentException);
		}

		internal static Exception CreateInvalidSurrogatePairException(char low, char hi, ExceptionType exceptionType)
		{
			return XmlConvert.CreateInvalidSurrogatePairException(low, hi, exceptionType, 0, 0);
		}

		internal static Exception CreateInvalidSurrogatePairException(char low, char hi, ExceptionType exceptionType, int lineNo, int linePos)
		{
			string[] array = new string[2];
			int num = 0;
			uint num2 = (uint)hi;
			array[num] = num2.ToString("X", CultureInfo.InvariantCulture);
			int num3 = 1;
			num2 = (uint)low;
			array[num3] = num2.ToString("X", CultureInfo.InvariantCulture);
			string[] args = array;
			return XmlConvert.CreateException("The surrogate pair (0x{0}, 0x{1}) is invalid. A high surrogate character (0xD800 - 0xDBFF) must always be paired with a low surrogate character (0xDC00 - 0xDFFF).", args, exceptionType, lineNo, linePos);
		}

		internal static Exception CreateInvalidHighSurrogateCharException(char hi)
		{
			return XmlConvert.CreateInvalidHighSurrogateCharException(hi, ExceptionType.ArgumentException);
		}

		internal static Exception CreateInvalidHighSurrogateCharException(char hi, ExceptionType exceptionType)
		{
			return XmlConvert.CreateInvalidHighSurrogateCharException(hi, exceptionType, 0, 0);
		}

		internal static Exception CreateInvalidHighSurrogateCharException(char hi, ExceptionType exceptionType, int lineNo, int linePos)
		{
			string res = "Invalid high surrogate character (0x{0}). A high surrogate character must have a value from range (0xD800 - 0xDBFF).";
			uint num = (uint)hi;
			return XmlConvert.CreateException(res, num.ToString("X", CultureInfo.InvariantCulture), exceptionType, lineNo, linePos);
		}

		internal static Exception CreateInvalidCharException(char[] data, int length, int invCharPos)
		{
			return XmlConvert.CreateInvalidCharException(data, length, invCharPos, ExceptionType.ArgumentException);
		}

		internal static Exception CreateInvalidCharException(char[] data, int length, int invCharPos, ExceptionType exceptionType)
		{
			return XmlConvert.CreateException("'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(data, length, invCharPos), exceptionType, 0, invCharPos + 1);
		}

		internal static Exception CreateInvalidCharException(string data, int invCharPos)
		{
			return XmlConvert.CreateInvalidCharException(data, invCharPos, ExceptionType.ArgumentException);
		}

		internal static Exception CreateInvalidCharException(string data, int invCharPos, ExceptionType exceptionType)
		{
			return XmlConvert.CreateException("'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(data, invCharPos), exceptionType, 0, invCharPos + 1);
		}

		internal static Exception CreateInvalidCharException(char invChar, char nextChar)
		{
			return XmlConvert.CreateInvalidCharException(invChar, nextChar, ExceptionType.ArgumentException);
		}

		internal static Exception CreateInvalidCharException(char invChar, char nextChar, ExceptionType exceptionType)
		{
			return XmlConvert.CreateException("'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(invChar, nextChar), exceptionType);
		}

		internal static Exception CreateInvalidNameCharException(string name, int index, ExceptionType exceptionType)
		{
			return XmlConvert.CreateException((index == 0) ? "Name cannot begin with the '{0}' character, hexadecimal value {1}." : "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(name, index), exceptionType, 0, index + 1);
		}

		internal static ArgumentException CreateInvalidNameArgumentException(string name, string argumentName)
		{
			if (name != null)
			{
				return new ArgumentException(Res.GetString("The empty string '' is not a valid name."), argumentName);
			}
			return new ArgumentNullException(argumentName);
		}

		private static XmlCharType xmlCharType = XmlCharType.Instance;

		internal static char[] crt = new char[]
		{
			'\n',
			'\r',
			'\t'
		};

		private static readonly int c_EncodedCharLength = 7;

		private static volatile Regex c_EncodeCharPattern;

		private static volatile Regex c_DecodeCharPattern;

		private static volatile string[] s_allDateTimeFormats;

		internal static readonly char[] WhitespaceChars = new char[]
		{
			' ',
			'\t',
			'\n',
			'\r'
		};
	}
}
