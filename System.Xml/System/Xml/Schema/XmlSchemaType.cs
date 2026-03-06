using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>The base class for all simple types and complex types.</summary>
	public class XmlSchemaType : XmlSchemaAnnotated
	{
		/// <summary>Returns an <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> that represents the built-in simple type of the simple type that is specified by the qualified name.</summary>
		/// <param name="qualifiedName">The <see cref="T:System.Xml.XmlQualifiedName" /> of the simple type.</param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> that represents the built-in simple type.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.Xml.XmlQualifiedName" /> parameter is <see langword="null" />.</exception>
		public static XmlSchemaSimpleType GetBuiltInSimpleType(XmlQualifiedName qualifiedName)
		{
			if (qualifiedName == null)
			{
				throw new ArgumentNullException("qualifiedName");
			}
			return DatatypeImplementation.GetSimpleTypeFromXsdType(qualifiedName);
		}

		/// <summary>Returns an <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> that represents the built-in simple type of the specified simple type.</summary>
		/// <param name="typeCode">One of the <see cref="T:System.Xml.Schema.XmlTypeCode" /> values representing the simple type.</param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> that represents the built-in simple type.</returns>
		public static XmlSchemaSimpleType GetBuiltInSimpleType(XmlTypeCode typeCode)
		{
			return DatatypeImplementation.GetSimpleTypeFromTypeCode(typeCode);
		}

		/// <summary>Returns an <see cref="T:System.Xml.Schema.XmlSchemaComplexType" /> that represents the built-in complex type of the complex type specified.</summary>
		/// <param name="typeCode">One of the <see cref="T:System.Xml.Schema.XmlTypeCode" /> values representing the complex type.</param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaComplexType" /> that represents the built-in complex type.</returns>
		public static XmlSchemaComplexType GetBuiltInComplexType(XmlTypeCode typeCode)
		{
			if (typeCode == XmlTypeCode.Item)
			{
				return XmlSchemaComplexType.AnyType;
			}
			return null;
		}

		/// <summary>Returns an <see cref="T:System.Xml.Schema.XmlSchemaComplexType" /> that represents the built-in complex type of the complex type specified by qualified name.</summary>
		/// <param name="qualifiedName">The <see cref="T:System.Xml.XmlQualifiedName" /> of the complex type.</param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaComplexType" /> that represents the built-in complex type.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.Xml.XmlQualifiedName" /> parameter is <see langword="null" />.</exception>
		public static XmlSchemaComplexType GetBuiltInComplexType(XmlQualifiedName qualifiedName)
		{
			if (qualifiedName == null)
			{
				throw new ArgumentNullException("qualifiedName");
			}
			if (qualifiedName.Equals(XmlSchemaComplexType.AnyType.QualifiedName))
			{
				return XmlSchemaComplexType.AnyType;
			}
			if (qualifiedName.Equals(XmlSchemaComplexType.UntypedAnyType.QualifiedName))
			{
				return XmlSchemaComplexType.UntypedAnyType;
			}
			return null;
		}

		/// <summary>Gets or sets the name of the type.</summary>
		/// <returns>The name of the type.</returns>
		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>Gets or sets the final attribute of the type derivation that indicates if further derivations are allowed.</summary>
		/// <returns>One of the valid <see cref="T:System.Xml.Schema.XmlSchemaDerivationMethod" /> values. The default is <see cref="F:System.Xml.Schema.XmlSchemaDerivationMethod.None" />.</returns>
		[XmlAttribute("final")]
		[DefaultValue(XmlSchemaDerivationMethod.None)]
		public XmlSchemaDerivationMethod Final
		{
			get
			{
				return this.final;
			}
			set
			{
				this.final = value;
			}
		}

		/// <summary>Gets the qualified name for the type built from the <see langword="Name" /> attribute of this type. This is a post-schema-compilation property.</summary>
		/// <returns>The <see cref="T:System.Xml.XmlQualifiedName" /> for the type built from the <see langword="Name" /> attribute of this type.</returns>
		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qname;
			}
		}

		/// <summary>Gets the post-compilation value of the <see cref="P:System.Xml.Schema.XmlSchemaType.Final" /> property.</summary>
		/// <returns>The post-compilation value of the <see cref="P:System.Xml.Schema.XmlSchemaType.Final" /> property. The default is the <see langword="finalDefault" /> attribute value of the <see langword="schema" /> element.</returns>
		[XmlIgnore]
		public XmlSchemaDerivationMethod FinalResolved
		{
			get
			{
				return this.finalResolved;
			}
		}

		/// <summary>Gets the post-compilation object type or the built-in XML Schema Definition Language (XSD) data type, simpleType element, or complexType element. This is a post-schema-compilation infoset property.</summary>
		/// <returns>The built-in XSD data type, simpleType element, or complexType element.</returns>
		[Obsolete("This property has been deprecated. Please use BaseXmlSchemaType property that returns a strongly typed base schema type. http://go.microsoft.com/fwlink/?linkid=14202")]
		[XmlIgnore]
		public object BaseSchemaType
		{
			get
			{
				if (this.baseSchemaType == null)
				{
					return null;
				}
				if (this.baseSchemaType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
				{
					return this.baseSchemaType.Datatype;
				}
				return this.baseSchemaType;
			}
		}

		/// <summary>Gets the post-compilation value for the base type of this schema type.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaType" /> object representing the base type of this schema type.</returns>
		[XmlIgnore]
		public XmlSchemaType BaseXmlSchemaType
		{
			get
			{
				return this.baseSchemaType;
			}
		}

		/// <summary>Gets the post-compilation information on how this element was derived from its base type.</summary>
		/// <returns>One of the valid <see cref="T:System.Xml.Schema.XmlSchemaDerivationMethod" /> values.</returns>
		[XmlIgnore]
		public XmlSchemaDerivationMethod DerivedBy
		{
			get
			{
				return this.derivedBy;
			}
		}

		/// <summary>Gets the post-compilation value for the data type of the complex type.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaDatatype" /> post-schema-compilation value.</returns>
		[XmlIgnore]
		public XmlSchemaDatatype Datatype
		{
			get
			{
				return this.datatype;
			}
		}

		/// <summary>Gets or sets a value indicating if this type has a mixed content model. This property is only valid in a complex type.</summary>
		/// <returns>
		///     <see langword="true" /> if the type has a mixed content model; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[XmlIgnore]
		public virtual bool IsMixed
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.Schema.XmlTypeCode" /> of the type.</summary>
		/// <returns>One of the <see cref="T:System.Xml.Schema.XmlTypeCode" /> values.</returns>
		[XmlIgnore]
		public XmlTypeCode TypeCode
		{
			get
			{
				if (this == XmlSchemaComplexType.AnyType)
				{
					return XmlTypeCode.Item;
				}
				if (this.datatype == null)
				{
					return XmlTypeCode.None;
				}
				return this.datatype.TypeCode;
			}
		}

		[XmlIgnore]
		internal XmlValueConverter ValueConverter
		{
			get
			{
				if (this.datatype == null)
				{
					return XmlUntypedConverter.Untyped;
				}
				return this.datatype.ValueConverter;
			}
		}

		internal XmlReader Validate(XmlReader reader, XmlResolver resolver, XmlSchemaSet schemaSet, ValidationEventHandler valEventHandler)
		{
			if (schemaSet != null)
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.Schemas = schemaSet;
				xmlReaderSettings.ValidationEventHandler += valEventHandler;
				return new XsdValidatingReader(reader, resolver, xmlReaderSettings, this);
			}
			return null;
		}

		internal XmlSchemaContentType SchemaContentType
		{
			get
			{
				return this.contentType;
			}
		}

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qname = value;
		}

		internal void SetFinalResolved(XmlSchemaDerivationMethod value)
		{
			this.finalResolved = value;
		}

		internal void SetBaseSchemaType(XmlSchemaType value)
		{
			this.baseSchemaType = value;
		}

		internal void SetDerivedBy(XmlSchemaDerivationMethod value)
		{
			this.derivedBy = value;
		}

		internal void SetDatatype(XmlSchemaDatatype value)
		{
			this.datatype = value;
		}

		internal SchemaElementDecl ElementDecl
		{
			get
			{
				return this.elementDecl;
			}
			set
			{
				this.elementDecl = value;
			}
		}

		[XmlIgnore]
		internal XmlSchemaType Redefined
		{
			get
			{
				return this.redefined;
			}
			set
			{
				this.redefined = value;
			}
		}

		internal virtual XmlQualifiedName DerivedFrom
		{
			get
			{
				return XmlQualifiedName.Empty;
			}
		}

		internal void SetContentType(XmlSchemaContentType value)
		{
			this.contentType = value;
		}

		/// <summary>Returns a value indicating if the derived schema type specified is derived from the base schema type specified</summary>
		/// <param name="derivedType">The derived <see cref="T:System.Xml.Schema.XmlSchemaType" /> to test.</param>
		/// <param name="baseType">The base <see cref="T:System.Xml.Schema.XmlSchemaType" /> to test the derived <see cref="T:System.Xml.Schema.XmlSchemaType" /> against.</param>
		/// <param name="except">One of the <see cref="T:System.Xml.Schema.XmlSchemaDerivationMethod" /> values representing a type derivation method to exclude from testing.</param>
		/// <returns>
		///     <see langword="true" /> if the derived type is derived from the base type; otherwise, <see langword="false" />.</returns>
		public static bool IsDerivedFrom(XmlSchemaType derivedType, XmlSchemaType baseType, XmlSchemaDerivationMethod except)
		{
			if (derivedType == null || baseType == null)
			{
				return false;
			}
			if (derivedType == baseType)
			{
				return true;
			}
			if (baseType == XmlSchemaComplexType.AnyType)
			{
				return true;
			}
			XmlSchemaSimpleType xmlSchemaSimpleType;
			XmlSchemaSimpleType xmlSchemaSimpleType2;
			for (;;)
			{
				xmlSchemaSimpleType = (derivedType as XmlSchemaSimpleType);
				xmlSchemaSimpleType2 = (baseType as XmlSchemaSimpleType);
				if (xmlSchemaSimpleType2 != null && xmlSchemaSimpleType != null)
				{
					break;
				}
				if ((except & derivedType.DerivedBy) != XmlSchemaDerivationMethod.Empty)
				{
					return false;
				}
				derivedType = derivedType.BaseXmlSchemaType;
				if (derivedType == baseType)
				{
					return true;
				}
				if (derivedType == null)
				{
					return false;
				}
			}
			return xmlSchemaSimpleType2 == DatatypeImplementation.AnySimpleType || ((except & derivedType.DerivedBy) == XmlSchemaDerivationMethod.Empty && xmlSchemaSimpleType.Datatype.IsDerivedFrom(xmlSchemaSimpleType2.Datatype));
		}

		internal static bool IsDerivedFromDatatype(XmlSchemaDatatype derivedDataType, XmlSchemaDatatype baseDataType, XmlSchemaDerivationMethod except)
		{
			return DatatypeImplementation.AnySimpleType.Datatype == baseDataType || derivedDataType.IsDerivedFrom(baseDataType);
		}

		[XmlIgnore]
		internal override string NameAttribute
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		private string name;

		private XmlSchemaDerivationMethod final = XmlSchemaDerivationMethod.None;

		private XmlSchemaDerivationMethod derivedBy;

		private XmlSchemaType baseSchemaType;

		private XmlSchemaDatatype datatype;

		private XmlSchemaDerivationMethod finalResolved;

		private volatile SchemaElementDecl elementDecl;

		private volatile XmlQualifiedName qname = XmlQualifiedName.Empty;

		private XmlSchemaType redefined;

		private XmlSchemaContentType contentType;
	}
}
