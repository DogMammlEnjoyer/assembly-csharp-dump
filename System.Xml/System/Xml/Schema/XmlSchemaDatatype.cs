using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace System.Xml.Schema
{
	/// <summary>The <see cref="T:System.Xml.Schema.XmlSchemaDatatype" /> class is an abstract class for mapping XML Schema definition language (XSD) types to Common Language Runtime (CLR) types.</summary>
	public abstract class XmlSchemaDatatype
	{
		/// <summary>When overridden in a derived class, gets the Common Language Runtime (CLR) type of the item.</summary>
		/// <returns>The Common Language Runtime (CLR) type of the item.</returns>
		public abstract Type ValueType { get; }

		/// <summary>When overridden in a derived class, gets the type for the <see langword="string" /> as specified in the World Wide Web Consortium (W3C) XML 1.0 specification.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlTokenizedType" /> value for the <see langword="string" />.</returns>
		public abstract XmlTokenizedType TokenizedType { get; }

		/// <summary>When overridden in a derived class, validates the <see langword="string" /> specified against a built-in or user-defined simple type.</summary>
		/// <param name="s">The <see langword="string" /> to validate against the simple type.</param>
		/// <param name="nameTable">The <see cref="T:System.Xml.XmlNameTable" /> to use for atomization while parsing the <see langword="string" /> if this <see cref="T:System.Xml.Schema.XmlSchemaDatatype" /> object represents the xs:NCName type. </param>
		/// <param name="nsmgr">The <see cref="T:System.Xml.IXmlNamespaceResolver" /> object to use while parsing the <see langword="string" /> if this <see cref="T:System.Xml.Schema.XmlSchemaDatatype" /> object represents the xs:QName type.</param>
		/// <returns>An <see cref="T:System.Object" /> that can be cast safely to the type returned by the <see cref="P:System.Xml.Schema.XmlSchemaDatatype.ValueType" /> property.</returns>
		/// <exception cref="T:System.Xml.Schema.XmlSchemaValidationException">The input value is not a valid instance of this W3C XML Schema type.</exception>
		/// <exception cref="T:System.ArgumentNullException">The value to parse cannot be <see langword="null" />.</exception>
		public abstract object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr);

		/// <summary>Gets the <see cref="T:System.Xml.Schema.XmlSchemaDatatypeVariety" /> value for the simple type.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaDatatypeVariety" /> value for the simple type.</returns>
		public virtual XmlSchemaDatatypeVariety Variety
		{
			get
			{
				return XmlSchemaDatatypeVariety.Atomic;
			}
		}

		/// <summary>Converts the value specified, whose type is one of the valid Common Language Runtime (CLR) representations of the XML schema type represented by the <see cref="T:System.Xml.Schema.XmlSchemaDatatype" />, to the CLR type specified.</summary>
		/// <param name="value">The input value to convert to the specified type.</param>
		/// <param name="targetType">The target type to convert the input value to.</param>
		/// <returns>The converted input value.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.Object" /> or <see cref="T:System.Type" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The type represented by the <see cref="T:System.Xml.Schema.XmlSchemaDatatype" />   does not support a conversion from type of the value specified to the type specified.</exception>
		public virtual object ChangeType(object value, Type targetType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			return this.ValueConverter.ChangeType(value, targetType);
		}

		/// <summary>Converts the value specified, whose type is one of the valid Common Language Runtime (CLR) representations of the XML schema type represented by the <see cref="T:System.Xml.Schema.XmlSchemaDatatype" />, to the CLR type specified using the <see cref="T:System.Xml.IXmlNamespaceResolver" /> if the <see cref="T:System.Xml.Schema.XmlSchemaDatatype" /> represents the xs:QName type or a type derived from it.</summary>
		/// <param name="value">The input value to convert to the specified type.</param>
		/// <param name="targetType">The target type to convert the input value to.</param>
		/// <param name="namespaceResolver">An <see cref="T:System.Xml.IXmlNamespaceResolver" /> used for resolving namespace prefixes. This is only of use if the <see cref="T:System.Xml.Schema.XmlSchemaDatatype" />  represents the xs:QName type or a type derived from it.</param>
		/// <returns>The converted input value.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.Object" /> or <see cref="T:System.Type" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The type represented by the <see cref="T:System.Xml.Schema.XmlSchemaDatatype" />   does not support a conversion from type of the value specified to the type specified.</exception>
		public virtual object ChangeType(object value, Type targetType, IXmlNamespaceResolver namespaceResolver)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (namespaceResolver == null)
			{
				throw new ArgumentNullException("namespaceResolver");
			}
			return this.ValueConverter.ChangeType(value, targetType, namespaceResolver);
		}

		/// <summary>Gets the <see cref="T:System.Xml.Schema.XmlTypeCode" /> value for the simple type.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlTypeCode" /> value for the simple type.</returns>
		public virtual XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.None;
			}
		}

		/// <summary>The <see cref="M:System.Xml.Schema.XmlSchemaDatatype.IsDerivedFrom(System.Xml.Schema.XmlSchemaDatatype)" /> method always returns <see langword="false" />.</summary>
		/// <param name="datatype">The <see cref="T:System.Xml.Schema.XmlSchemaDatatype" />.</param>
		/// <returns>Always returns <see langword="false" />.</returns>
		public virtual bool IsDerivedFrom(XmlSchemaDatatype datatype)
		{
			return false;
		}

		internal abstract bool HasLexicalFacets { get; }

		internal abstract bool HasValueFacets { get; }

		internal abstract XmlValueConverter ValueConverter { get; }

		internal abstract RestrictionFacets Restriction { get; set; }

		internal abstract int Compare(object value1, object value2);

		internal abstract object ParseValue(string s, Type typDest, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr);

		internal abstract object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, bool createAtomicValue);

		internal abstract Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue);

		internal abstract Exception TryParseValue(object value, XmlNameTable nameTable, IXmlNamespaceResolver namespaceResolver, out object typedValue);

		internal abstract FacetsChecker FacetsChecker { get; }

		internal abstract XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get; }

		internal abstract XmlSchemaDatatype DeriveByRestriction(XmlSchemaObjectCollection facets, XmlNameTable nameTable, XmlSchemaType schemaType);

		internal abstract XmlSchemaDatatype DeriveByList(XmlSchemaType schemaType);

		internal abstract void VerifySchemaValid(XmlSchemaObjectTable notations, XmlSchemaObject caller);

		internal abstract bool IsEqual(object o1, object o2);

		internal abstract bool IsComparable(XmlSchemaDatatype dtype);

		internal string TypeCodeString
		{
			get
			{
				string result = string.Empty;
				XmlTypeCode typeCode = this.TypeCode;
				switch (this.Variety)
				{
				case XmlSchemaDatatypeVariety.Atomic:
					if (typeCode == XmlTypeCode.AnyAtomicType)
					{
						result = "anySimpleType";
					}
					else
					{
						result = this.TypeCodeToString(typeCode);
					}
					break;
				case XmlSchemaDatatypeVariety.List:
					if (typeCode == XmlTypeCode.AnyAtomicType)
					{
						result = "List of Union";
					}
					else
					{
						result = "List of " + this.TypeCodeToString(typeCode);
					}
					break;
				case XmlSchemaDatatypeVariety.Union:
					result = "Union";
					break;
				}
				return result;
			}
		}

		internal string TypeCodeToString(XmlTypeCode typeCode)
		{
			switch (typeCode)
			{
			case XmlTypeCode.None:
				return "None";
			case XmlTypeCode.Item:
				return "AnyType";
			case XmlTypeCode.AnyAtomicType:
				return "AnyAtomicType";
			case XmlTypeCode.String:
				return "String";
			case XmlTypeCode.Boolean:
				return "Boolean";
			case XmlTypeCode.Decimal:
				return "Decimal";
			case XmlTypeCode.Float:
				return "Float";
			case XmlTypeCode.Double:
				return "Double";
			case XmlTypeCode.Duration:
				return "Duration";
			case XmlTypeCode.DateTime:
				return "DateTime";
			case XmlTypeCode.Time:
				return "Time";
			case XmlTypeCode.Date:
				return "Date";
			case XmlTypeCode.GYearMonth:
				return "GYearMonth";
			case XmlTypeCode.GYear:
				return "GYear";
			case XmlTypeCode.GMonthDay:
				return "GMonthDay";
			case XmlTypeCode.GDay:
				return "GDay";
			case XmlTypeCode.GMonth:
				return "GMonth";
			case XmlTypeCode.HexBinary:
				return "HexBinary";
			case XmlTypeCode.Base64Binary:
				return "Base64Binary";
			case XmlTypeCode.AnyUri:
				return "AnyUri";
			case XmlTypeCode.QName:
				return "QName";
			case XmlTypeCode.Notation:
				return "Notation";
			case XmlTypeCode.NormalizedString:
				return "NormalizedString";
			case XmlTypeCode.Token:
				return "Token";
			case XmlTypeCode.Language:
				return "Language";
			case XmlTypeCode.NmToken:
				return "NmToken";
			case XmlTypeCode.Name:
				return "Name";
			case XmlTypeCode.NCName:
				return "NCName";
			case XmlTypeCode.Id:
				return "Id";
			case XmlTypeCode.Idref:
				return "Idref";
			case XmlTypeCode.Entity:
				return "Entity";
			case XmlTypeCode.Integer:
				return "Integer";
			case XmlTypeCode.NonPositiveInteger:
				return "NonPositiveInteger";
			case XmlTypeCode.NegativeInteger:
				return "NegativeInteger";
			case XmlTypeCode.Long:
				return "Long";
			case XmlTypeCode.Int:
				return "Int";
			case XmlTypeCode.Short:
				return "Short";
			case XmlTypeCode.Byte:
				return "Byte";
			case XmlTypeCode.NonNegativeInteger:
				return "NonNegativeInteger";
			case XmlTypeCode.UnsignedLong:
				return "UnsignedLong";
			case XmlTypeCode.UnsignedInt:
				return "UnsignedInt";
			case XmlTypeCode.UnsignedShort:
				return "UnsignedShort";
			case XmlTypeCode.UnsignedByte:
				return "UnsignedByte";
			case XmlTypeCode.PositiveInteger:
				return "PositiveInteger";
			}
			return typeCode.ToString();
		}

		internal static string ConcatenatedToString(object value)
		{
			Type type = value.GetType();
			string result = string.Empty;
			if (type == typeof(IEnumerable) && type != typeof(string))
			{
				StringBuilder stringBuilder = new StringBuilder();
				IEnumerator enumerator = (value as IEnumerable).GetEnumerator();
				if (enumerator.MoveNext())
				{
					stringBuilder.Append("{");
					object obj = enumerator.Current;
					if (obj is IFormattable)
					{
						stringBuilder.Append(((IFormattable)obj).ToString("", CultureInfo.InvariantCulture));
					}
					else
					{
						stringBuilder.Append(obj.ToString());
					}
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(" , ");
						obj = enumerator.Current;
						if (obj is IFormattable)
						{
							stringBuilder.Append(((IFormattable)obj).ToString("", CultureInfo.InvariantCulture));
						}
						else
						{
							stringBuilder.Append(obj.ToString());
						}
					}
					stringBuilder.Append("}");
					result = stringBuilder.ToString();
				}
			}
			else if (value is IFormattable)
			{
				result = ((IFormattable)value).ToString("", CultureInfo.InvariantCulture);
			}
			else
			{
				result = value.ToString();
			}
			return result;
		}

		internal static XmlSchemaDatatype FromXmlTokenizedType(XmlTokenizedType token)
		{
			return DatatypeImplementation.FromXmlTokenizedType(token);
		}

		internal static XmlSchemaDatatype FromXmlTokenizedTypeXsd(XmlTokenizedType token)
		{
			return DatatypeImplementation.FromXmlTokenizedTypeXsd(token);
		}

		internal static XmlSchemaDatatype FromXdrName(string name)
		{
			return DatatypeImplementation.FromXdrName(name);
		}

		internal static XmlSchemaDatatype DeriveByUnion(XmlSchemaSimpleType[] types, XmlSchemaType schemaType)
		{
			return DatatypeImplementation.DeriveByUnion(types, schemaType);
		}

		internal static string XdrCanonizeUri(string uri, XmlNameTable nameTable, SchemaNames schemaNames)
		{
			int num = 5;
			bool flag = false;
			if (uri.Length > 5 && uri.StartsWith("uuid:", StringComparison.Ordinal))
			{
				flag = true;
			}
			else if (uri.Length > 9 && uri.StartsWith("urn:uuid:", StringComparison.Ordinal))
			{
				flag = true;
				num = 9;
			}
			string text;
			if (flag)
			{
				text = nameTable.Add(uri.Substring(0, num) + uri.Substring(num, uri.Length - num).ToUpper(CultureInfo.InvariantCulture));
			}
			else
			{
				text = uri;
			}
			if (Ref.Equal(schemaNames.NsDataTypeAlias, text) || Ref.Equal(schemaNames.NsDataTypeOld, text))
			{
				text = schemaNames.NsDataType;
			}
			else if (Ref.Equal(schemaNames.NsXdrAlias, text))
			{
				text = schemaNames.NsXdr;
			}
			return text;
		}
	}
}
