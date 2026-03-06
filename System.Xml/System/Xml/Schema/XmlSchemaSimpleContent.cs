using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="simpleContent" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class is for simple and complex types with simple content model.</summary>
	public class XmlSchemaSimpleContent : XmlSchemaContentModel
	{
		/// <summary>Gets one of the <see cref="T:System.Xml.Schema.XmlSchemaSimpleContentRestriction" /> or <see cref="T:System.Xml.Schema.XmlSchemaSimpleContentExtension" />.</summary>
		/// <returns>The content contained within the <see langword="XmlSchemaSimpleContentRestriction" /> or <see langword="XmlSchemaSimpleContentExtension" /> class.</returns>
		[XmlElement("restriction", typeof(XmlSchemaSimpleContentRestriction))]
		[XmlElement("extension", typeof(XmlSchemaSimpleContentExtension))]
		public override XmlSchemaContent Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		private XmlSchemaContent content;
	}
}
