using System;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the enveloped signature transform for an XML digital signature as defined by the W3C.</summary>
	public class XmlDsigEnvelopedSignatureTransform : Transform
	{
		internal int SignaturePosition
		{
			set
			{
				this._signaturePosition = value;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> class.</summary>
		public XmlDsigEnvelopedSignatureTransform()
		{
			base.Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> class with comments, if specified.</summary>
		/// <param name="includeComments">
		///   <see langword="true" /> to include comments; otherwise, <see langword="false" />.</param>
		public XmlDsigEnvelopedSignatureTransform(bool includeComments)
		{
			this._includeComments = includeComments;
			base.Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
		}

		/// <summary>Gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
		/// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</returns>
		public override Type[] InputTypes
		{
			get
			{
				return this._inputTypes;
			}
		}

		/// <summary>Gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
		/// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object; only objects of one of these types are returned from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</returns>
		public override Type[] OutputTypes
		{
			get
			{
				return this._outputTypes;
			}
		}

		/// <summary>Parses the specified <see cref="T:System.Xml.XmlNodeList" /> as transform-specific content of a <see langword="&lt;Transform&gt;" /> element and configures the internal state of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object to match the <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</param>
		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList != null && nodeList.Count > 0)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
		}

		/// <summary>Returns an XML representation of the parameters of an <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		/// <summary>Loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
		/// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="obj" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The containing XML document is <see langword="null" />.</exception>
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
				return;
			}
		}

		private void LoadStreamInput(Stream stream)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			XmlResolver xmlResolver = base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI);
			XmlReader reader = Utils.PreProcessStreamInput(stream, xmlResolver, base.BaseURI);
			xmlDocument.Load(reader);
			this._containingDocument = xmlDocument;
			if (this._containingDocument == null)
			{
				throw new CryptographicException("An XmlDocument context is required for enveloped transforms.");
			}
			this._nsm = new XmlNamespaceManager(this._containingDocument.NameTable);
			this._nsm.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
		}

		private void LoadXmlNodeListInput(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new ArgumentNullException("nodeList");
			}
			this._containingDocument = Utils.GetOwnerDocument(nodeList);
			if (this._containingDocument == null)
			{
				throw new CryptographicException("An XmlDocument context is required for enveloped transforms.");
			}
			this._nsm = new XmlNamespaceManager(this._containingDocument.NameTable);
			this._nsm.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
			this._inputNodeList = nodeList;
		}

		private void LoadXmlDocumentInput(XmlDocument doc)
		{
			if (doc == null)
			{
				throw new ArgumentNullException("doc");
			}
			this._containingDocument = doc;
			this._nsm = new XmlNamespaceManager(this._containingDocument.NameTable);
			this._nsm.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</summary>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The containing XML document is <see langword="null" />.</exception>
		public override object GetOutput()
		{
			if (this._containingDocument == null)
			{
				throw new CryptographicException("An XmlDocument context is required for enveloped transforms.");
			}
			if (this._inputNodeList != null)
			{
				if (this._signaturePosition == 0)
				{
					return this._inputNodeList;
				}
				XmlNodeList xmlNodeList = this._containingDocument.SelectNodes("//dsig:Signature", this._nsm);
				if (xmlNodeList == null)
				{
					return this._inputNodeList;
				}
				CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
				foreach (object obj in this._inputNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode != null)
					{
						if (Utils.IsXmlNamespaceNode(xmlNode) || Utils.IsNamespaceNode(xmlNode))
						{
							canonicalXmlNodeList.Add(xmlNode);
						}
						else
						{
							try
							{
								XmlNode xmlNode2 = xmlNode.SelectSingleNode("ancestor-or-self::dsig:Signature[1]", this._nsm);
								int num = 0;
								foreach (object obj2 in xmlNodeList)
								{
									XmlNode xmlNode3 = (XmlNode)obj2;
									num++;
									if (xmlNode3 == xmlNode2)
									{
										break;
									}
								}
								if (xmlNode2 == null || (xmlNode2 != null && num != this._signaturePosition))
								{
									canonicalXmlNodeList.Add(xmlNode);
								}
							}
							catch
							{
							}
						}
					}
				}
				return canonicalXmlNodeList;
			}
			else
			{
				XmlNodeList xmlNodeList2 = this._containingDocument.SelectNodes("//dsig:Signature", this._nsm);
				if (xmlNodeList2 == null)
				{
					return this._containingDocument;
				}
				if (xmlNodeList2.Count < this._signaturePosition || this._signaturePosition <= 0)
				{
					return this._containingDocument;
				}
				xmlNodeList2[this._signaturePosition - 1].ParentNode.RemoveChild(xmlNodeList2[this._signaturePosition - 1]);
				return this._containingDocument;
			}
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object of type <see cref="T:System.Xml.XmlNodeList" />.</summary>
		/// <param name="type">The type of the output to return. <see cref="T:System.Xml.XmlNodeList" /> is the only valid type for this parameter.</param>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigEnvelopedSignatureTransform" /> object of type <see cref="T:System.Xml.XmlNodeList" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is not an <see cref="T:System.Xml.XmlNodeList" /> object.</exception>
		public override object GetOutput(Type type)
		{
			if (type == typeof(XmlNodeList) || type.IsSubclassOf(typeof(XmlNodeList)))
			{
				if (this._inputNodeList == null)
				{
					this._inputNodeList = Utils.AllDescendantNodes(this._containingDocument, true);
				}
				return (XmlNodeList)this.GetOutput();
			}
			if (!(type == typeof(XmlDocument)) && !type.IsSubclassOf(typeof(XmlDocument)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			if (this._inputNodeList != null)
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return (XmlDocument)this.GetOutput();
		}

		private Type[] _inputTypes = new Type[]
		{
			typeof(Stream),
			typeof(XmlNodeList),
			typeof(XmlDocument)
		};

		private Type[] _outputTypes = new Type[]
		{
			typeof(XmlNodeList),
			typeof(XmlDocument)
		};

		private XmlNodeList _inputNodeList;

		private bool _includeComments;

		private XmlNamespaceManager _nsm;

		private XmlDocument _containingDocument;

		private int _signaturePosition;
	}
}
