using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="extension" /> element for simple content from XML Schema as specified by the World Wide Web Consortium (W3C). This class can be used to derive simple types by extension. Such derivations are used to extend the simple type content of the element by adding attributes.</summary>
	public class XmlSchemaSimpleContentExtension : XmlSchemaContent
	{
		/// <summary>Gets or sets the name of a built-in data type or simple type from which this type is extended.</summary>
		/// <returns>The base type name.</returns>
		[XmlAttribute("base")]
		public XmlQualifiedName BaseTypeName
		{
			get
			{
				return this.baseTypeName;
			}
			set
			{
				this.baseTypeName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		/// <summary>Gets the collection of <see cref="T:System.Xml.Schema.XmlSchemaAttribute" /> and <see cref="T:System.Xml.Schema.XmlSchemaAttributeGroupRef" />.</summary>
		/// <returns>The collection of attributes for the <see langword="simpleType" /> element.</returns>
		[XmlElement("attribute", typeof(XmlSchemaAttribute))]
		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
		public XmlSchemaObjectCollection Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		/// <summary>Gets or sets the <see langword="XmlSchemaAnyAttribute" /> to be used for the attribute value.</summary>
		/// <returns>The <see langword="XmlSchemaAnyAttribute" />.Optional.</returns>
		[XmlElement("anyAttribute")]
		public XmlSchemaAnyAttribute AnyAttribute
		{
			get
			{
				return this.anyAttribute;
			}
			set
			{
				this.anyAttribute = value;
			}
		}

		internal void SetAttributes(XmlSchemaObjectCollection newAttributes)
		{
			this.attributes = newAttributes;
		}

		private XmlSchemaObjectCollection attributes = new XmlSchemaObjectCollection();

		private XmlSchemaAnyAttribute anyAttribute;

		private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;
	}
}
