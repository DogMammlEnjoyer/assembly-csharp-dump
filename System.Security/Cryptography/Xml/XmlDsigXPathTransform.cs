using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the XPath transform for a digital signature as defined by the W3C.</summary>
	public class XmlDsigXPathTransform : Transform
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> class.</summary>
		public XmlDsigXPathTransform()
		{
			base.Algorithm = "http://www.w3.org/TR/1999/REC-xpath-19991116";
		}

		/// <summary>Gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXPathTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</summary>
		/// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXPathTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</returns>
		public override Type[] InputTypes
		{
			get
			{
				return this._inputTypes;
			}
		}

		/// <summary>Gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXPathTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</summary>
		/// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object; the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXPathTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object return only objects of one of these types.</returns>
		public override Type[] OutputTypes
		{
			get
			{
				return this._outputTypes;
			}
		}

		/// <summary>Parses the specified <see cref="T:System.Xml.XmlNodeList" /> object as transform-specific content of a <see langword="&lt;Transform&gt;" /> element and configures the internal state of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object to match the <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> object to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="nodeList" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="nodeList" /> parameter does not contain an <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> element.</exception>
		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
			foreach (object obj in nodeList)
			{
				XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
				if (xmlElement != null)
				{
					if (xmlElement.LocalName == "XPath")
					{
						this._xpathexpr = xmlElement.InnerXml.Trim(null);
						XmlNameTable nameTable = new XmlNodeReader(xmlElement).NameTable;
						this._nsm = new XmlNamespaceManager(nameTable);
						if (!Utils.VerifyAttributes(xmlElement, null))
						{
							throw new CryptographicException("Unknown transform has been encountered.");
						}
						using (IEnumerator enumerator2 = xmlElement.Attributes.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								XmlAttribute xmlAttribute = (XmlAttribute)obj2;
								if (xmlAttribute.Prefix == "xmlns")
								{
									string text = xmlAttribute.LocalName;
									string uri = xmlAttribute.Value;
									if (text == null)
									{
										text = xmlElement.Prefix;
										uri = xmlElement.NamespaceURI;
									}
									this._nsm.AddNamespace(text, uri);
								}
							}
							break;
						}
					}
					throw new CryptographicException("Unknown transform has been encountered.");
				}
			}
			if (this._xpathexpr == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
		}

		/// <summary>Returns an XML representation of the parameters of a <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
		protected override XmlNodeList GetInnerXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement(null, "XPath", "http://www.w3.org/2000/09/xmldsig#");
			if (this._nsm != null)
			{
				foreach (object obj in this._nsm)
				{
					string text = (string)obj;
					if (!(text == "xml") && !(text == "xmlns") && text != null && text.Length > 0)
					{
						xmlElement.SetAttribute("xmlns:" + text, this._nsm.LookupNamespace(text));
					}
				}
			}
			xmlElement.InnerXml = this._xpathexpr;
			xmlDocument.AppendChild(xmlElement);
			return xmlDocument.ChildNodes;
		}

		/// <summary>Loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</summary>
		/// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</param>
		public override void LoadInput(object obj)
		{
			if (obj is Stream)
			{
				this.LoadStreamInput((Stream)obj);
				return;
			}
			if (obj is XmlNodeList)
			{
				this.LoadXmlNodeListInput((XmlNodeList)obj);
				return;
			}
			if (obj is XmlDocument)
			{
				this.LoadXmlDocumentInput((XmlDocument)obj);
			}
		}

		private void LoadStreamInput(Stream stream)
		{
			XmlResolver xmlResolver = base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI);
			XmlReader reader = Utils.PreProcessStreamInput(stream, xmlResolver, base.BaseURI);
			this._document = new XmlDocument();
			this._document.PreserveWhitespace = true;
			this._document.Load(reader);
		}

		private void LoadXmlNodeListInput(XmlNodeList nodeList)
		{
			XmlResolver resolver = base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI);
			using (MemoryStream memoryStream = new MemoryStream(new CanonicalXml(nodeList, resolver, true).GetBytes()))
			{
				this.LoadStreamInput(memoryStream);
			}
		}

		private void LoadXmlDocumentInput(XmlDocument doc)
		{
			this._document = doc;
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</summary>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object.</returns>
		public override object GetOutput()
		{
			CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
			if (!string.IsNullOrEmpty(this._xpathexpr))
			{
				XPathNavigator xpathNavigator = this._document.CreateNavigator();
				XPathNodeIterator xpathNodeIterator = xpathNavigator.Select("//. | //@*");
				XPathExpression xpathExpression = xpathNavigator.Compile("boolean(" + this._xpathexpr + ")");
				xpathExpression.SetContext(this._nsm);
				while (xpathNodeIterator.MoveNext())
				{
					XPathNavigator xpathNavigator2 = xpathNodeIterator.Current;
					XmlNode node = ((IHasXmlNode)xpathNavigator2).GetNode();
					if ((bool)xpathNodeIterator.Current.Evaluate(xpathExpression))
					{
						canonicalXmlNodeList.Add(node);
					}
				}
				xpathNodeIterator = xpathNavigator.Select("//namespace::*");
				while (xpathNodeIterator.MoveNext())
				{
					XPathNavigator xpathNavigator3 = xpathNodeIterator.Current;
					XmlNode node2 = ((IHasXmlNode)xpathNavigator3).GetNode();
					canonicalXmlNodeList.Add(node2);
				}
			}
			return canonicalXmlNodeList;
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object of type <see cref="T:System.Xml.XmlNodeList" />.</summary>
		/// <param name="type">The type of the output to return. <see cref="T:System.Xml.XmlNodeList" /> is the only valid type for this parameter.</param>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXPathTransform" /> object of type <see cref="T:System.Xml.XmlNodeList" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is not an <see cref="T:System.Xml.XmlNodeList" /> object.</exception>
		public override object GetOutput(Type type)
		{
			if (type != typeof(XmlNodeList) && !type.IsSubclassOf(typeof(XmlNodeList)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return (XmlNodeList)this.GetOutput();
		}

		private Type[] _inputTypes = new Type[]
		{
			typeof(Stream),
			typeof(XmlNodeList),
			typeof(XmlDocument)
		};

		private Type[] _outputTypes = new Type[]
		{
			typeof(XmlNodeList)
		};

		private string _xpathexpr;

		private XmlDocument _document;

		private XmlNamespaceManager _nsm;
	}
}
