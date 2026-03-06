using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the XSLT transform for a digital signature as defined by the W3C.</summary>
	public class XmlDsigXsltTransform : Transform
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> class.</summary>
		public XmlDsigXsltTransform()
		{
			base.Algorithm = "http://www.w3.org/TR/1999/REC-xslt-19991116";
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> class with comments, if specified.</summary>
		/// <param name="includeComments">
		///   <see langword="true" /> to include comments; otherwise, <see langword="false" />.</param>
		public XmlDsigXsltTransform(bool includeComments)
		{
			this._includeComments = includeComments;
			base.Algorithm = "http://www.w3.org/TR/1999/REC-xslt-19991116";
		}

		/// <summary>Gets an array of types that are valid inputs to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXsltTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</summary>
		/// <returns>An array of valid input types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object; you can pass only objects of one of these types to the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXsltTransform.LoadInput(System.Object)" /> method of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</returns>
		public override Type[] InputTypes
		{
			get
			{
				return this._inputTypes;
			}
		}

		/// <summary>Gets an array of types that are possible outputs from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXsltTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</summary>
		/// <returns>An array of valid output types for the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object; only objects of one of these types are returned from the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXsltTransform.GetOutput" /> methods of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</returns>
		public override Type[] OutputTypes
		{
			get
			{
				return this._outputTypes;
			}
		}

		/// <summary>Parses the specified <see cref="T:System.Xml.XmlNodeList" /> object as transform-specific content of a <see langword="&lt;Transform&gt;" /> element and configures the internal state of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object to match the <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <param name="nodeList">An <see cref="T:System.Xml.XmlNodeList" /> object that encapsulates an XSLT style sheet to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object. This style sheet is applied to the document loaded by the <see cref="M:System.Security.Cryptography.Xml.XmlDsigXsltTransform.LoadInput(System.Object)" /> method.</param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="nodeList" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="nodeList" /> parameter does not contain an <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</exception>
		public override void LoadInnerXml(XmlNodeList nodeList)
		{
			if (nodeList == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
			XmlElement xmlElement = null;
			int num = 0;
			foreach (object obj in nodeList)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode is XmlWhitespace))
				{
					if (xmlNode is XmlElement)
					{
						if (num != 0)
						{
							throw new CryptographicException("Unknown transform has been encountered.");
						}
						xmlElement = (xmlNode as XmlElement);
						num++;
					}
					else
					{
						num++;
					}
				}
			}
			if (num != 1 || xmlElement == null)
			{
				throw new CryptographicException("Unknown transform has been encountered.");
			}
			this._xslNodes = nodeList;
			this._xslFragment = xmlElement.OuterXml.Trim(null);
		}

		/// <summary>Returns an XML representation of the parameters of the <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object that are suitable to be included as subelements of an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</summary>
		/// <returns>A list of the XML nodes that represent the transform-specific content needed to describe the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object in an XMLDSIG <see langword="&lt;Transform&gt;" /> element.</returns>
		protected override XmlNodeList GetInnerXml()
		{
			return this._xslNodes;
		}

		/// <summary>Loads the specified input into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</summary>
		/// <param name="obj">The input to load into the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</param>
		public override void LoadInput(object obj)
		{
			if (this._inputStream != null)
			{
				this._inputStream.Close();
			}
			this._inputStream = new MemoryStream();
			if (obj is Stream)
			{
				this._inputStream = (Stream)obj;
				return;
			}
			if (!(obj is XmlNodeList))
			{
				if (obj is XmlDocument)
				{
					byte[] bytes = new CanonicalXml((XmlDocument)obj, null, this._includeComments).GetBytes();
					if (bytes == null)
					{
						return;
					}
					this._inputStream.Write(bytes, 0, bytes.Length);
					this._inputStream.Flush();
					this._inputStream.Position = 0L;
				}
				return;
			}
			byte[] bytes2 = new CanonicalXml((XmlNodeList)obj, null, this._includeComments).GetBytes();
			if (bytes2 == null)
			{
				return;
			}
			this._inputStream.Write(bytes2, 0, bytes2.Length);
			this._inputStream.Flush();
			this._inputStream.Position = 0L;
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</summary>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object.</returns>
		public override object GetOutput()
		{
			XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.XmlResolver = null;
			xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
			xmlReaderSettings.MaxCharactersInDocument = 0L;
			object result;
			using (StringReader stringReader = new StringReader(this._xslFragment))
			{
				XmlReader stylesheet = XmlReader.Create(stringReader, xmlReaderSettings, null);
				xslCompiledTransform.Load(stylesheet, XsltSettings.Default, null);
				XPathDocument input = new XPathDocument(XmlReader.Create(this._inputStream, xmlReaderSettings, base.BaseURI), XmlSpace.Preserve);
				MemoryStream memoryStream = new MemoryStream();
				XmlWriter results = new XmlTextWriter(memoryStream, null);
				xslCompiledTransform.Transform(input, null, results);
				memoryStream.Position = 0L;
				result = memoryStream;
			}
			return result;
		}

		/// <summary>Returns the output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object of type <see cref="T:System.IO.Stream" />.</summary>
		/// <param name="type">The type of the output to return. <see cref="T:System.IO.Stream" /> is the only valid type for this parameter.</param>
		/// <returns>The output of the current <see cref="T:System.Security.Cryptography.Xml.XmlDsigXsltTransform" /> object of type <see cref="T:System.IO.Stream" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="type" /> parameter is not a <see cref="T:System.IO.Stream" /> object.</exception>
		public override object GetOutput(Type type)
		{
			if (type != typeof(Stream) && !type.IsSubclassOf(typeof(Stream)))
			{
				throw new ArgumentException("The input type was invalid for this transform.", "type");
			}
			return (Stream)this.GetOutput();
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

		private XmlNodeList _xslNodes;

		private string _xslFragment;

		private Stream _inputStream;

		private bool _includeComments;
	}
}
