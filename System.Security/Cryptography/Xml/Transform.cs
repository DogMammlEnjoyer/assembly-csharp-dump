using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the abstract base class from which all <see langword="&lt;Transform&gt;" /> elements that can be used in an XML digital signature derive.</summary>
	public abstract class Transform
	{
		internal string BaseURI
		{
			get
			{
				return this._baseUri;
			}
			set
			{
				this._baseUri = value;
			}
		}

		internal SignedXml SignedXml
		{
			get
			{
				return this._signedXml;
			}
			set
			{
				this._signedXml = value;
			}
		}

		internal Reference Reference
		{
			get
			{
				return this._reference;
			}
			set
			{
				this._reference = value;
			}
		}

		/// <summary>Gets or sets the Uniform Resource Identifier (URI) that identifies the algorithm performed by the current transform.</summary>
		/// <returns>The URI that identifies the algorithm performed by the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
		public string Algorithm
		{
			get
			{
				return this._algorithm;
			}
			set
			{
				this._algorithm = value;
			}
		}

		/// <summary>Sets the current <see cref="T:System.Xml.XmlResolver" /> object.</summary>
		/// <returns>The current <see cref="T:System.Xml.XmlResolver" /> object. This property defaults to an <see cref="T:System.Xml.XmlSecureResolver" /> object.</returns>
		public XmlResolver Resolver
		{
			internal get
			{
				return this._xmlResolver;
			}
			set
			{
				this._xmlResolver = value;
				this._bResolverSet = true;
			}
		}

		internal bool ResolverSet
		{
			get
			{
				return this._bResolverSet;
			}
		}

		/// <summary>When overridden in a derived class, gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.Transform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
		/// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.Transform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
		public abstract Type[] InputTypes { get; }

		/// <summary>When overridden in a derived class, gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.Transform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
		/// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object; only objects of one of these types are returned from the <see cref="M:System.Security.Cryptography.Xml.Transform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
		public abstract Type[] OutputTypes { get; }

		internal bool AcceptsType(Type inputType)
		{
			if (this.InputTypes != null)
			{
				for (int i = 0; i < this.InputTypes.Length; i++)
				{
					if (inputType == this.InputTypes[i] || inputType.IsSubclassOf(this.InputTypes[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>Returns the XML representation of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
		/// <returns>The XML representation of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
		public XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			return this.GetXml(document, "Transform");
		}

		internal XmlElement GetXml(XmlDocument document, string name)
		{
			XmlElement xmlElement = document.CreateElement(name, "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this.Algorithm))
			{
				xmlElement.SetAttribute("Algorithm", this.Algorithm);
			}
			XmlNodeList innerXml = this.GetInnerXml();
			if (innerXml != null)
			{
				foreach (object obj in innerXml)
				{
					XmlNode node = (XmlNode)obj;
					xmlElement.AppendChild(document.ImportNode(node, true));
				}
			}
			return xmlElement;
		}

		/// <summary>When overridden in a derived class, parses the specified <see cref="T:System.Xml.XmlNodeList" /> object as transform-specific content of a <see langword="&lt;Transform&gt;" /> element and configures the internal state of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object to match the <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> object that specifies transform-specific content for the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</param>
		public abstract void LoadInnerXml(XmlNodeList nodeList);

		/// <summary>When overridden in a derived class, returns an XML representation of the parameters of the <see cref="T:System.Security.Cryptography.Xml.Transform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
		protected abstract XmlNodeList GetInnerXml();

		/// <summary>When overridden in a derived class, loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
		/// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</param>
		public abstract void LoadInput(object obj);

		/// <summary>When overridden in a derived class, returns the output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
		public abstract object GetOutput();

		/// <summary>When overridden in a derived class, returns the output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object of the specified type.</summary>
		/// <param name="type">The type of the output to return. This must be one of the types in the <see cref="P:System.Security.Cryptography.Xml.Transform.OutputTypes" /> property.</param>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object as an object of the specified type.</returns>
		public abstract object GetOutput(Type type);

		/// <summary>When overridden in a derived class, returns the digest associated with a <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</summary>
		/// <param name="hash">The <see cref="T:System.Security.Cryptography.HashAlgorithm" /> object used to create a digest.</param>
		/// <returns>The digest associated with a <see cref="T:System.Security.Cryptography.Xml.Transform" /> object.</returns>
		public virtual byte[] GetDigestedOutput(HashAlgorithm hash)
		{
			return hash.ComputeHash((Stream)this.GetOutput(typeof(Stream)));
		}

		/// <summary>Gets or sets an <see cref="T:System.Xml.XmlElement" /> object that represents the document context under which the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object is running.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlElement" /> object that represents the document context under which the current <see cref="T:System.Security.Cryptography.Xml.Transform" /> object is running.</returns>
		public XmlElement Context
		{
			get
			{
				if (this._context != null)
				{
					return this._context;
				}
				Reference reference = this.Reference;
				SignedXml signedXml = (reference == null) ? this.SignedXml : reference.SignedXml;
				if (signedXml == null)
				{
					return null;
				}
				return signedXml._context;
			}
			set
			{
				this._context = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Collections.Hashtable" /> object that contains the namespaces that are propagated into the signature.</summary>
		/// <returns>A <see cref="T:System.Collections.Hashtable" /> object that contains the namespaces that are propagated into the signature.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.Transform.PropagatedNamespaces" /> property was set to <see langword="null" />.</exception>
		public Hashtable PropagatedNamespaces
		{
			get
			{
				if (this._propagatedNamespaces != null)
				{
					return this._propagatedNamespaces;
				}
				Reference reference = this.Reference;
				SignedXml signedXml = (reference == null) ? this.SignedXml : reference.SignedXml;
				if (reference != null && (reference.ReferenceTargetType != ReferenceTargetType.UriReference || string.IsNullOrEmpty(reference.Uri) || reference.Uri[0] != '#'))
				{
					this._propagatedNamespaces = new Hashtable(0);
					return this._propagatedNamespaces;
				}
				CanonicalXmlNodeList canonicalXmlNodeList = null;
				if (reference != null)
				{
					canonicalXmlNodeList = reference._namespaces;
				}
				else if (((signedXml != null) ? signedXml._context : null) != null)
				{
					canonicalXmlNodeList = Utils.GetPropagatedAttributes(signedXml._context);
				}
				if (canonicalXmlNodeList == null)
				{
					this._propagatedNamespaces = new Hashtable(0);
					return this._propagatedNamespaces;
				}
				this._propagatedNamespaces = new Hashtable(canonicalXmlNodeList.Count);
				foreach (object obj in canonicalXmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string key = (xmlNode.Prefix.Length > 0) ? (xmlNode.Prefix + ":" + xmlNode.LocalName) : xmlNode.LocalName;
					if (!this._propagatedNamespaces.Contains(key))
					{
						this._propagatedNamespaces.Add(key, xmlNode.Value);
					}
				}
				return this._propagatedNamespaces;
			}
		}

		private string _algorithm;

		private string _baseUri;

		internal XmlResolver _xmlResolver;

		private bool _bResolverSet;

		private SignedXml _signedXml;

		private Reference _reference;

		private Hashtable _propagatedNamespaces;

		private XmlElement _context;
	}
}
