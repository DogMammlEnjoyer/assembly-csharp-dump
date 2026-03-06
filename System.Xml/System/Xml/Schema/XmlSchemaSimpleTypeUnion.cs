using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="union" /> element for simple types from XML Schema as specified by the World Wide Web Consortium (W3C). A <see langword="union" /> datatype can be used to specify the content of a <see langword="simpleType" />. The value of the <see langword="simpleType" /> element must be any one of a set of alternative datatypes specified in the union. Union types are always derived types and must comprise at least two alternative datatypes.</summary>
	public class XmlSchemaSimpleTypeUnion : XmlSchemaSimpleTypeContent
	{
		/// <summary>Gets the collection of base types.</summary>
		/// <returns>The collection of simple type base values.</returns>
		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		public XmlSchemaObjectCollection BaseTypes
		{
			get
			{
				return this.baseTypes;
			}
		}

		/// <summary>Gets or sets the array of qualified member names of built-in data types or <see langword="simpleType" /> elements defined in this schema (or another schema indicated by the specified namespace).</summary>
		/// <returns>An array that consists of a list of members of built-in data types or simple types.</returns>
		[XmlAttribute("memberTypes")]
		public XmlQualifiedName[] MemberTypes
		{
			get
			{
				return this.memberTypes;
			}
			set
			{
				this.memberTypes = value;
			}
		}

		/// <summary>Gets an array of <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> objects representing the type of the <see langword="simpleType" /> element based on the <see cref="P:System.Xml.Schema.XmlSchemaSimpleTypeUnion.BaseTypes" /> and <see cref="P:System.Xml.Schema.XmlSchemaSimpleTypeUnion.MemberTypes" /> values of the simple type.</summary>
		/// <returns>An array of <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> objects representing the type of the <see langword="simpleType" /> element.</returns>
		[XmlIgnore]
		public XmlSchemaSimpleType[] BaseMemberTypes
		{
			get
			{
				return this.baseMemberTypes;
			}
		}

		internal void SetBaseMemberTypes(XmlSchemaSimpleType[] baseMemberTypes)
		{
			this.baseMemberTypes = baseMemberTypes;
		}

		internal override XmlSchemaObject Clone()
		{
			if (this.memberTypes != null && this.memberTypes.Length != 0)
			{
				XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = (XmlSchemaSimpleTypeUnion)base.MemberwiseClone();
				XmlQualifiedName[] array = new XmlQualifiedName[this.memberTypes.Length];
				for (int i = 0; i < this.memberTypes.Length; i++)
				{
					array[i] = this.memberTypes[i].Clone();
				}
				xmlSchemaSimpleTypeUnion.MemberTypes = array;
				return xmlSchemaSimpleTypeUnion;
			}
			return this;
		}

		private XmlSchemaObjectCollection baseTypes = new XmlSchemaObjectCollection();

		private XmlQualifiedName[] memberTypes;

		private XmlSchemaSimpleType[] baseMemberTypes;
	}
}
