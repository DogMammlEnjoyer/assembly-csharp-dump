using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="attributeGroup" /> element from the XML Schema as specified by the World Wide Web Consortium (W3C). AttributesGroups provides a mechanism to group a set of attribute declarations so that they can be incorporated as a group into complex type definitions.</summary>
	public class XmlSchemaAttributeGroup : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the name of the attribute group.</summary>
		/// <returns>The name of the attribute group.</returns>
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

		/// <summary>Gets the collection of attributes for the attribute group. Contains <see langword="XmlSchemaAttribute" /> and <see langword="XmlSchemaAttributeGroupRef" /> elements.</summary>
		/// <returns>The collection of attributes for the attribute group.</returns>
		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
		[XmlElement("attribute", typeof(XmlSchemaAttribute))]
		public XmlSchemaObjectCollection Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.Schema.XmlSchemaAnyAttribute" /> component of the attribute group.</summary>
		/// <returns>The World Wide Web Consortium (W3C) <see langword="anyAttribute" /> element.</returns>
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

		/// <summary>Gets the qualified name of the attribute group.</summary>
		/// <returns>The qualified name of the attribute group.</returns>
		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qname;
			}
		}

		[XmlIgnore]
		internal XmlSchemaObjectTable AttributeUses
		{
			get
			{
				if (this.attributeUses == null)
				{
					this.attributeUses = new XmlSchemaObjectTable();
				}
				return this.attributeUses;
			}
		}

		[XmlIgnore]
		internal XmlSchemaAnyAttribute AttributeWildcard
		{
			get
			{
				return this.attributeWildcard;
			}
			set
			{
				this.attributeWildcard = value;
			}
		}

		/// <summary>Gets the redefined attribute group property from the XML Schema.</summary>
		/// <returns>The redefined attribute group property.</returns>
		[XmlIgnore]
		public XmlSchemaAttributeGroup RedefinedAttributeGroup
		{
			get
			{
				return this.redefined;
			}
		}

		[XmlIgnore]
		internal XmlSchemaAttributeGroup Redefined
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

		[XmlIgnore]
		internal int SelfReferenceCount
		{
			get
			{
				return this.selfReferenceCount;
			}
			set
			{
				this.selfReferenceCount = value;
			}
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

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qname = value;
		}

		internal override XmlSchemaObject Clone()
		{
			XmlSchemaAttributeGroup xmlSchemaAttributeGroup = (XmlSchemaAttributeGroup)base.MemberwiseClone();
			if (XmlSchemaComplexType.HasAttributeQNameRef(this.attributes))
			{
				xmlSchemaAttributeGroup.attributes = XmlSchemaComplexType.CloneAttributes(this.attributes);
				xmlSchemaAttributeGroup.attributeUses = null;
			}
			return xmlSchemaAttributeGroup;
		}

		private string name;

		private XmlSchemaObjectCollection attributes = new XmlSchemaObjectCollection();

		private XmlSchemaAnyAttribute anyAttribute;

		private XmlQualifiedName qname = XmlQualifiedName.Empty;

		private XmlSchemaAttributeGroup redefined;

		private XmlSchemaObjectTable attributeUses;

		private XmlSchemaAnyAttribute attributeWildcard;

		private int selfReferenceCount;
	}
}
