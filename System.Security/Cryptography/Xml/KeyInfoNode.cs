using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Handles <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> subelements that do not have specific implementations or handlers registered on the machine.</summary>
	public class KeyInfoNode : KeyInfoClause
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" /> class.</summary>
		public KeyInfoNode()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" /> class with content taken from the specified <see cref="T:System.Xml.XmlElement" />.</summary>
		/// <param name="node">An XML element from which to take the content used to create the new instance of <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" />.</param>
		public KeyInfoNode(XmlElement node)
		{
			this._node = node;
		}

		/// <summary>Gets or sets the XML content of the current <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" />.</summary>
		/// <returns>The XML content of the current <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" />.</returns>
		public XmlElement Value
		{
			get
			{
				return this._node;
			}
			set
			{
				this._node = value;
			}
		}

		/// <summary>Returns an XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" />.</summary>
		/// <returns>An XML representation of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" />.</returns>
		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal override XmlElement GetXml(XmlDocument xmlDocument)
		{
			return xmlDocument.ImportNode(this._node, true) as XmlElement;
		}

		/// <summary>Parses the input <see cref="T:System.Xml.XmlElement" /> and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" /> to match.</summary>
		/// <param name="value">The <see cref="T:System.Xml.XmlElement" /> that specifies the state of the <see cref="T:System.Security.Cryptography.Xml.KeyInfoNode" />.</param>
		public override void LoadXml(XmlElement value)
		{
			this._node = value;
		}

		private XmlElement _node;
	}
}
