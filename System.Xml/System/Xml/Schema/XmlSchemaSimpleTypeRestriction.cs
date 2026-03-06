using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="restriction" /> element for simple types from XML Schema as specified by the World Wide Web Consortium (W3C). This class can be used restricting <see langword="simpleType" /> element.</summary>
	public class XmlSchemaSimpleTypeRestriction : XmlSchemaSimpleTypeContent
	{
		/// <summary>Gets or sets the name of the qualified base type.</summary>
		/// <returns>The qualified name of the simple type restriction base type.</returns>
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

		/// <summary>Gets or sets information on the base type.</summary>
		/// <returns>The base type for the <see langword="simpleType" /> element.</returns>
		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		public XmlSchemaSimpleType BaseType
		{
			get
			{
				return this.baseType;
			}
			set
			{
				this.baseType = value;
			}
		}

		/// <summary>Gets or sets an Xml Schema facet. </summary>
		/// <returns>One of the following facet classes:
		///     <see cref="T:System.Xml.Schema.XmlSchemaLengthFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaMinLengthFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaMaxLengthFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaPatternFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaEnumerationFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaMaxInclusiveFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaMaxExclusiveFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaMinInclusiveFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaMinExclusiveFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaFractionDigitsFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaTotalDigitsFacet" />, <see cref="T:System.Xml.Schema.XmlSchemaWhiteSpaceFacet" />.</returns>
		[XmlElement("length", typeof(XmlSchemaLengthFacet))]
		[XmlElement("minLength", typeof(XmlSchemaMinLengthFacet))]
		[XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet))]
		[XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet))]
		[XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet))]
		[XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet))]
		[XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet))]
		[XmlElement("pattern", typeof(XmlSchemaPatternFacet))]
		[XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet))]
		[XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet))]
		[XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet))]
		[XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet))]
		public XmlSchemaObjectCollection Facets
		{
			get
			{
				return this.facets;
			}
		}

		internal override XmlSchemaObject Clone()
		{
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)base.MemberwiseClone();
			xmlSchemaSimpleTypeRestriction.BaseTypeName = this.baseTypeName.Clone();
			return xmlSchemaSimpleTypeRestriction;
		}

		private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;

		private XmlSchemaSimpleType baseType;

		private XmlSchemaObjectCollection facets = new XmlSchemaObjectCollection();
	}
}
