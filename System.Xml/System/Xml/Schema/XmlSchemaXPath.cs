using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the World Wide Web Consortium (W3C) <see langword="selector" /> element.</summary>
	public class XmlSchemaXPath : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the attribute for the XPath expression.</summary>
		/// <returns>The string attribute value for the XPath expression.</returns>
		[XmlAttribute("xpath")]
		[DefaultValue("")]
		public string XPath
		{
			get
			{
				return this.xpath;
			}
			set
			{
				this.xpath = value;
			}
		}

		private string xpath;
	}
}
