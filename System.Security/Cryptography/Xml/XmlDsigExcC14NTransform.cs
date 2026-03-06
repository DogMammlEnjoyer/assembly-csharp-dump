using System;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the exclusive C14N XML canonicalization transform for a digital signature as defined by the World Wide Web Consortium (W3C), without comments.</summary>
	public class XmlDsigExcC14NTransform : Transform
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> class.</summary>
		public XmlDsigExcC14NTransform() : this(false, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> class specifying a value that determines whether to include comments.</summary>
		/// <param name="includeComments">
		///   <see langword="true" /> to include comments; otherwise, <see langword="false" />.</param>
		public XmlDsigExcC14NTransform(bool includeComments) : this(includeComments, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> class specifying a list of namespace prefixes to canonicalize using the standard canonicalization algorithm.</summary>
		/// <param name="inclusiveNamespacesPrefixList">The namespace prefixes to canonicalize using the standard canonicalization algorithm.</param>
		public XmlDsigExcC14NTransform(string inclusiveNamespacesPrefixList) : this(false, inclusiveNamespacesPrefixList)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> class specifying whether to include comments, and specifying a list of namespace prefixes.</summary>
		/// <param name="includeComments">
		///   <see langword="true" /> to include comments; otherwise, <see langword="false" />.</param>
		/// <param name="inclusiveNamespacesPrefixList">The namespace prefixes to canonicalize using the standard canonicalization algorithm.</param>
		public XmlDsigExcC14NTransform(bool includeComments, string inclusiveNamespacesPrefixList)
		{
			this._includeComments = includeComments;
			this._inclusiveNamespacesPrefixList = inclusiveNamespacesPrefixList;
			base.Algorithm = (includeComments ? "http://www.w3.org/2001/10/xml-exc-c14n#WithComments" : "http://www.w3.org/2001/10/xml-exc-c14n#");
		}

		/// <summary>Gets or sets a string that contains namespace prefixes to canonicalize using the standard canonicalization algorithm.</summary>
		/// <returns>A string that contains namespace prefixes to canonicalize using the standard canonicalization algorithm.</returns>
		public string InclusiveNamespacesPrefixList
		{
			get
			{
				return this._inclusiveNamespacesPrefixList;
			}
			set
			{
				this._inclusiveNamespacesPrefixList = value;
			}
		}

		/// <summary>Gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</summary>
		/// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</returns>
		public override Type[] InputTypes
		{
			get
			{
				return this._inputTypes;
			}
		}

		/// <summary>Gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</summary>
		/// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object; the <see cref="Overload:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object return only objects of one of these types.</returns>
		public override Type[] OutputTypes
		{
			get
			{
				return this._outputTypes;
			}
		}

		/// <summary>Parses the specified <see cref="T:System.Xml.XmlNodeList" /> object as transform-specific content of a <see langword="&lt;Transform&gt;" /> element and configures the internal state of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object to match the <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> object that specifies transform-specific content for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</param>
		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList != null)
			{
				foreach (object obj in nodeList)
				{
					XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
					if (xmlElement != null)
					{
						if (!xmlElement.LocalName.Equals("InclusiveNamespaces") || !xmlElement.NamespaceURI.Equals("http://www.w3.org/2001/10/xml-exc-c14n#") || !Utils.HasAttribute(xmlElement, "PrefixList", "http://www.w3.org/2000/09/xmldsig#"))
						{
							throw new CryptographicException("Unknown transform has been encountered.");
						}
						if (!Utils.VerifyAttributes(xmlElement, "PrefixList"))
						{
							throw new CryptographicException("Unknown transform has been encountered.");
						}
						this.InclusiveNamespacesPrefixList = Utils.GetAttribute(xmlElement, "PrefixList", "http://www.w3.org/2000/09/xmldsig#");
						break;
					}
				}
			}
		}

		/// <summary>When overridden in a derived class, loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</summary>
		/// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="obj" /> parameter is not a <see cref="T:System.IO.Stream" /> object.  
		///  -or-  
		///  The <paramref name="obj" /> parameter is not an <see cref="T:System.Xml.XmlDocument" /> object.  
		///  -or-  
		///  The <paramref name="obj" /> parameter is not an <see cref="T:System.Xml.XmlNodeList" /> object.</exception>
		public override void LoadInput(object obj)
		{
			XmlResolver resolver = base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI);
			if (obj is Stream)
			{
				this._excCanonicalXml = new ExcCanonicalXml((Stream)obj, this._includeComments, this._inclusiveNamespacesPrefixList, resolver, base.BaseURI);
				return;
			}
			if (obj is XmlDocument)
			{
				this._excCanonicalXml = new ExcCanonicalXml((XmlDocument)obj, this._includeComments, this._inclusiveNamespacesPrefixList, resolver);
				return;
			}
			if (obj is XmlNodeList)
			{
				this._excCanonicalXml = new ExcCanonicalXml((XmlNodeList)obj, this._includeComments, this._inclusiveNamespacesPrefixList, resolver);
				return;
			}
			throw new ArgumentException("Type of input object is invalid.", "obj");
		}

		/// <summary>Returns an XML representation of the parameters of a <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
		protected override XmlNodeList GetInnerXml()
		{
			if (this.InclusiveNamespacesPrefixList == null)
			{
				return null;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("Transform", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(base.Algorithm))
			{
				xmlElement.SetAttribute("Algorithm", base.Algorithm);
			}
			XmlElement xmlElement2 = xmlDocument.CreateElement("InclusiveNamespaces", "http://www.w3.org/2001/10/xml-exc-c14n#");
			xmlElement2.SetAttribute("PrefixList", this.InclusiveNamespacesPrefixList);
			xmlElement.AppendChild(xmlElement2);
			return xmlElement.ChildNodes;
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object</summary>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</returns>
		public override object GetOutput()
		{
			return new MemoryStream(this._excCanonicalXml.GetBytes());
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object as an object of the specified type.</summary>
		/// <param name="type">The type of the output to return. This must be one of the types in the <see cref="P:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform.OutputTypes" /> property.</param>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object as an object of the specified type.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is not a <see cref="T:System.IO.Stream" /> object.  
		///  -or-  
		///  The <paramref name="type" /> parameter does not derive from a <see cref="T:System.IO.Stream" /> object.</exception>
		public override object GetOutput(Type type)
		{
			if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return new MemoryStream(this._excCanonicalXml.GetBytes());
		}

		/// <summary>Returns the digest associated with a <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</summary>
		/// <param name="hash">The <see cref="T:System.Security.Cryptography.HashAlgorithm" /> object used to create a digest.</param>
		/// <returns>The digest associated with a <see cref="T:System.Security.Cryptography.Xml.XmlDsigExcC14NTransform" /> object.</returns>
		public override byte[] GetDigestedOutput(HashAlgorithm hash)
		{
			return this._excCanonicalXml.GetDigestedBytes(hash);
		}

		private Type[] _inputTypes = new Type[]
		{
			typeof(Stream),
			typeof(XmlDocument),
			typeof(XmlNodeList)
		};

		private Type[] _outputTypes = new Type[]
		{
			typeof(Stream)
		};

		private bool _includeComments;

		private string _inclusiveNamespacesPrefixList;

		private ExcCanonicalXml _excCanonicalXml;
	}
}
