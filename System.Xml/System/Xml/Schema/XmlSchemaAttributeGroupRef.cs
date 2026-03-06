using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="attributeGroup" /> element with the <see langword="ref" /> attribute from the XML Schema as specified by the World Wide Web Consortium (W3C). AttributesGroupRef is the reference for an attributeGroup, name property contains the attribute group being referenced. </summary>
	public class XmlSchemaAttributeGroupRef : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the name of the referenced <see langword="attributeGroup" /> element.</summary>
		/// <returns>The name of the referenced attribute group. The value must be a QName.</returns>
		[XmlAttribute("ref")]
		public XmlQualifiedName RefName
		{
			get
			{
				return this.refName;
			}
			set
			{
				this.refName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		private XmlQualifiedName refName = XmlQualifiedName.Empty;
	}
}
