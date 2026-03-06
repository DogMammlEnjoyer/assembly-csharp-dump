using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the object element of an XML signature that holds data to be signed.</summary>
	public class DataObject
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> class.</summary>
		public DataObject()
		{
			this._cachedXml = null;
			this._elData = new CanonicalXmlNodeList();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> class with the specified identification, MIME type, encoding, and data.</summary>
		/// <param name="id">The identification to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" /> with.</param>
		/// <param name="mimeType">The MIME type of the data used to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" />.</param>
		/// <param name="encoding">The encoding of the data used to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" />.</param>
		/// <param name="data">The data to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.DataObject" /> with.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="data" /> parameter is <see langword="null" />.</exception>
		public DataObject(string id, string mimeType, string encoding, XmlElement data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this._id = id;
			this._mimeType = mimeType;
			this._encoding = encoding;
			this._elData = new CanonicalXmlNodeList();
			this._elData.Add(data);
			this._cachedXml = null;
		}

		/// <summary>Gets or sets the identification of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
		/// <returns>The name of the element that contains data to be used.</returns>
		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the MIME type of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
		/// <returns>The MIME type of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object. The default is <see langword="null" />.</returns>
		public string MimeType
		{
			get
			{
				return this._mimeType;
			}
			set
			{
				this._mimeType = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the encoding of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
		/// <returns>The type of encoding of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</returns>
		public string Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				this._encoding = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the data value of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
		/// <returns>The data of the current <see cref="T:System.Security.Cryptography.Xml.DataObject" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value used to set the property is <see langword="null" />.</exception>
		public XmlNodeList Data
		{
			get
			{
				return this._elData;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._elData = new CanonicalXmlNodeList();
				foreach (object obj in value)
				{
					XmlNode value2 = (XmlNode)obj;
					this._elData.Add(value2);
				}
				this._cachedXml = null;
			}
		}

		private bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		/// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</summary>
		/// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object.</returns>
		public XmlElement GetXml()
		{
			if (this.CacheValid)
			{
				return this._cachedXml;
			}
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this._id))
			{
				xmlElement.SetAttribute("Id", this._id);
			}
			if (!string.IsNullOrEmpty(this._mimeType))
			{
				xmlElement.SetAttribute("MimeType", this._mimeType);
			}
			if (!string.IsNullOrEmpty(this._encoding))
			{
				xmlElement.SetAttribute("Encoding", this._encoding);
			}
			if (this._elData != null)
			{
				foreach (object obj in this._elData)
				{
					XmlNode node = (XmlNode)obj;
					xmlElement.AppendChild(document.ImportNode(node, true));
				}
			}
			return xmlElement;
		}

		/// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.DataObject" /> state from an XML element.</summary>
		/// <param name="value">The XML element to load the <see cref="T:System.Security.Cryptography.Xml.DataObject" /> state from.</param>
		/// <exception cref="T:System.ArgumentNullException">The value from the XML element is <see langword="null" />.</exception>
		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this._id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
			this._mimeType = Utils.GetAttribute(value, "MimeType", "http://www.w3.org/2000/09/xmldsig#");
			this._encoding = Utils.GetAttribute(value, "Encoding", "http://www.w3.org/2000/09/xmldsig#");
			foreach (object obj in value.ChildNodes)
			{
				XmlNode value2 = (XmlNode)obj;
				this._elData.Add(value2);
			}
			this._cachedXml = value;
		}

		private string _id;

		private string _mimeType;

		private string _encoding;

		private CanonicalXmlNodeList _elData;

		private XmlElement _cachedXml;
	}
}
