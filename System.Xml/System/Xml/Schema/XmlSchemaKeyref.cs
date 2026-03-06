using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>This class represents the <see langword="keyref" /> element from XMLSchema as specified by the World Wide Web Consortium (W3C).</summary>
	public class XmlSchemaKeyref : XmlSchemaIdentityConstraint
	{
		/// <summary>Gets or sets the name of the key that this constraint refers to in another simple or complex type.</summary>
		/// <returns>The QName of the key that this constraint refers to.</returns>
		[XmlAttribute("refer")]
		public XmlQualifiedName Refer
		{
			get
			{
				return this.refer;
			}
			set
			{
				this.refer = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		private XmlQualifiedName refer = XmlQualifiedName.Empty;
	}
}
