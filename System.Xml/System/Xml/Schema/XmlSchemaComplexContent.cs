using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="complexContent" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class represents the complex content model for complex types. It contains extensions or restrictions on a complex type that has either only elements or mixed content.</summary>
	public class XmlSchemaComplexContent : XmlSchemaContentModel
	{
		/// <summary>Gets or sets information that determines if the type has a mixed content model.</summary>
		/// <returns>If this property is <see langword="true" />, character data is allowed to appear between the child elements of the complex type (mixed content model). The default is <see langword="false" />.Optional.</returns>
		[XmlAttribute("mixed")]
		public bool IsMixed
		{
			get
			{
				return this.isMixed;
			}
			set
			{
				this.isMixed = value;
				this.hasMixedAttribute = true;
			}
		}

		/// <summary>Gets or sets the content.</summary>
		/// <returns>One of either the <see cref="T:System.Xml.Schema.XmlSchemaComplexContentRestriction" /> or <see cref="T:System.Xml.Schema.XmlSchemaComplexContentExtension" /> classes.</returns>
		[XmlElement("restriction", typeof(XmlSchemaComplexContentRestriction))]
		[XmlElement("extension", typeof(XmlSchemaComplexContentExtension))]
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

		[XmlIgnore]
		internal bool HasMixedAttribute
		{
			get
			{
				return this.hasMixedAttribute;
			}
		}

		private XmlSchemaContent content;

		private bool isMixed;

		private bool hasMixedAttribute;
	}
}
