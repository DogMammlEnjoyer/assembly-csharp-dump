using System;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the C14N XML canonicalization transform for a digital signature as defined by the World Wide Web Consortium (W3C), without comments.</summary>
	public class XmlDsigC14NTransform : Transform
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> class.</summary>
		public XmlDsigC14NTransform()
		{
			base.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> class with comments, if specified.</summary>
		/// <param name="includeComments">
		///   <see langword="true" /> to include comments; otherwise, <see langword="false" />.</param>
		public XmlDsigC14NTransform(bool includeComments)
		{
			this._includeComments = includeComments;
			base.Algorithm = (includeComments ? "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments" : "http://www.w3.org/TR/2001/REC-xml-c14n-20010315");
		}

		/// <summary>Gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigC14NTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</summary>
		/// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigC14NTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</returns>
		public override Type[] InputTypes
		{
			get
			{
				return this._inputTypes;
			}
		}

		/// <summary>Gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigC14NTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</summary>
		/// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object; the <see cref="M:System.Security.Cryptography.Xml.XmlDsigC14NTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object return only objects of one of these types.</returns>
		public override Type[] OutputTypes
		{
			get
			{
				return this._outputTypes;
			}
		}

		/// <summary>Parses the specified <see cref="T:System.Xml.XmlNodeList" /> object as transform-specific content of a <see langword="&lt;Transform&gt;" /> element; this method is not supported because this element has no inner XML elements.</summary>
		/// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> object to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</param>
		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList != null && nodeList.Count > 0)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
		}

		/// <summary>Returns an XML representation of the parameters of an <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
		protected override XmlNodeList GetInnerXml()
		{
			return null;
		}

		/// <summary>Loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</summary>
		/// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="obj" /> parameter is a <see cref="T:System.IO.Stream" /> object and it is <see langword="null" />.</exception>
		public override void LoadInput(object obj)
		{
			XmlResolver resolver = base.ResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), base.BaseURI);
			if (obj is Stream)
			{
				this._cXml = new CanonicalXml((Stream)obj, this._includeComments, resolver, base.BaseURI);
				return;
			}
			if (obj is XmlDocument)
			{
				this._cXml = new CanonicalXml((XmlDocument)obj, resolver, this._includeComments);
				return;
			}
			if (obj is XmlNodeList)
			{
				this._cXml = new CanonicalXml((XmlNodeList)obj, resolver, this._includeComments);
				return;
			}
			throw new ArgumentException("Type of input object is invalid.", "obj");
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</summary>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</returns>
		public override object GetOutput()
		{
			return new MemoryStream(this._cXml.GetBytes());
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object of type <see cref="T:System.IO.Stream" />.</summary>
		/// <param name="type">The type of the output to return. <see cref="T:System.IO.Stream" /> is the only valid type for this parameter.</param>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object of type <see cref="T:System.IO.Stream" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is not a <see cref="T:System.IO.Stream" /> object.</exception>
		public override object GetOutput(Type type)
		{
			if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return new MemoryStream(this._cXml.GetBytes());
		}

		/// <summary>Returns the digest associated with an <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</summary>
		/// <param name="hash">The <see cref="T:System.Security.Cryptography.HashAlgorithm" /> object used to create a digest.</param>
		/// <returns>The digest associated with an <see cref="T:System.Security.Cryptography.Xml.XmlDsigC14NTransform" /> object.</returns>
		public override byte[] GetDigestedOutput(HashAlgorithm hash)
		{
			return this._cXml.GetDigestedBytes(hash);
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

		private CanonicalXml _cXml;

		private bool _includeComments;
	}
}
