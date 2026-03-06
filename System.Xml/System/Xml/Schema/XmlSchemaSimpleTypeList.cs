using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="list" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class can be used to define a <see langword="simpleType" /> element as a list of values of a specified data type.</summary>
	public class XmlSchemaSimpleTypeList : XmlSchemaSimpleTypeContent
	{
		/// <summary>Gets or sets the name of a built-in data type or <see langword="simpleType" /> element defined in this schema (or another schema indicated by the specified namespace).</summary>
		/// <returns>The type name of the simple type list.</returns>
		[XmlAttribute("itemType")]
		public XmlQualifiedName ItemTypeName
		{
			get
			{
				return this.itemTypeName;
			}
			set
			{
				this.itemTypeName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		/// <summary>Gets or sets the <see langword="simpleType" /> element that is derived from the type specified by the base value.</summary>
		/// <returns>The item type for the simple type element.</returns>
		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		public XmlSchemaSimpleType ItemType
		{
			get
			{
				return this.itemType;
			}
			set
			{
				this.itemType = value;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> representing the type of the <see langword="simpleType" /> element based on the <see cref="P:System.Xml.Schema.XmlSchemaSimpleTypeList.ItemType" /> and <see cref="P:System.Xml.Schema.XmlSchemaSimpleTypeList.ItemTypeName" /> values of the simple type.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> representing the type of the <see langword="simpleType" /> element.</returns>
		[XmlIgnore]
		public XmlSchemaSimpleType BaseItemType
		{
			get
			{
				return this.baseItemType;
			}
			set
			{
				this.baseItemType = value;
			}
		}

		internal override XmlSchemaObject Clone()
		{
			XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = (XmlSchemaSimpleTypeList)base.MemberwiseClone();
			xmlSchemaSimpleTypeList.ItemTypeName = this.itemTypeName.Clone();
			return xmlSchemaSimpleTypeList;
		}

		private XmlQualifiedName itemTypeName = XmlQualifiedName.Empty;

		private XmlSchemaSimpleType itemType;

		private XmlSchemaSimpleType baseItemType;
	}
}
