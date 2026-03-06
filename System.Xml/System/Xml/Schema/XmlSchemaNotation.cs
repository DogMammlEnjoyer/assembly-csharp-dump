using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="notation" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). An XML Schema <see langword="notation" /> declaration is a reconstruction of <see langword="XML 1.0 NOTATION" /> declarations. The purpose of notations is to describe the format of non-XML data within an XML document.</summary>
	public class XmlSchemaNotation : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the name of the notation.</summary>
		/// <returns>The name of the notation.</returns>
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

		/// <summary>Gets or sets the <see langword="public" /> identifier.</summary>
		/// <returns>The <see langword="public" /> identifier. The value must be a valid Uniform Resource Identifier (URI).</returns>
		[XmlAttribute("public")]
		public string Public
		{
			get
			{
				return this.publicId;
			}
			set
			{
				this.publicId = value;
			}
		}

		/// <summary>Gets or sets the <see langword="system" /> identifier.</summary>
		/// <returns>The <see langword="system" /> identifier. The value must be a valid URI.</returns>
		[XmlAttribute("system")]
		public string System
		{
			get
			{
				return this.systemId;
			}
			set
			{
				this.systemId = value;
			}
		}

		[XmlIgnore]
		internal XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qname;
			}
			set
			{
				this.qname = value;
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

		private string name;

		private string publicId;

		private string systemId;

		private XmlQualifiedName qname = XmlQualifiedName.Empty;
	}
}
