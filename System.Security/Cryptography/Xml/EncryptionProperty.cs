using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the <see langword="&lt;EncryptionProperty&gt;" /> element used in XML encryption. This class cannot be inherited.</summary>
	public sealed class EncryptionProperty
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> class.</summary>
		public EncryptionProperty()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> class using an <see cref="T:System.Xml.XmlElement" /> object.</summary>
		/// <param name="elementProperty">An <see cref="T:System.Xml.XmlElement" /> object to use for initialization.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="elementProperty" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Xml.XmlElement.LocalName" /> property of the <paramref name="elementProperty" /> parameter is not "EncryptionProperty".  
		///  -or-  
		///  The <see cref="P:System.Xml.XmlElement.NamespaceURI" /> property of the <paramref name="elementProperty" /> parameter is not "http://www.w3.org/2001/04/xmlenc#".</exception>
		public EncryptionProperty(XmlElement elementProperty)
		{
			if (elementProperty == null)
			{
				throw new ArgumentNullException("elementProperty");
			}
			if (elementProperty.LocalName != "EncryptionProperty" || elementProperty.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed encryption property element.");
			}
			this._elemProp = elementProperty;
			this._cachedXml = null;
		}

		/// <summary>Gets the ID of the current <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</summary>
		/// <returns>The ID of the current <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</returns>
		public string Id
		{
			get
			{
				return this._id;
			}
		}

		/// <summary>Gets the target of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</summary>
		/// <returns>The target of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</returns>
		public string Target
		{
			get
			{
				return this._target;
			}
		}

		/// <summary>Gets or sets an <see cref="T:System.Xml.XmlElement" /> object that represents an <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlElement" /> object that represents an <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.EncryptionProperty.PropertyElement" /> property was set to <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Xml.XmlElement.LocalName" /> property of the value set to the <see cref="P:System.Security.Cryptography.Xml.EncryptionProperty.PropertyElement" /> property is not "EncryptionProperty".  
		///  -or-  
		///  The <see cref="P:System.Xml.XmlElement.NamespaceURI" /> property of the value set to the <see cref="P:System.Security.Cryptography.Xml.EncryptionProperty.PropertyElement" /> property is not "http://www.w3.org/2001/04/xmlenc#".</exception>
		public XmlElement PropertyElement
		{
			get
			{
				return this._elemProp;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.LocalName != "EncryptionProperty" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
				{
					throw new CryptographicException("Malformed encryption property element.");
				}
				this._elemProp = value;
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

		/// <summary>Returns an <see cref="T:System.Xml.XmlElement" /> object that encapsulates an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> class.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlElement" /> object that encapsulates an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> class.</returns>
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
			return document.ImportNode(this._elemProp, true) as XmlElement;
		}

		/// <summary>Parses the input <see cref="T:System.Xml.XmlElement" /> and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object to match.</summary>
		/// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object to parse.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Xml.XmlElement.LocalName" /> property of the <paramref name="value" /> parameter is not "EncryptionProperty".  
		///  -or-  
		///  The <see cref="P:System.Xml.XmlElement.NamespaceURI" /> property of the <paramref name="value" /> parameter is not "http://www.w3.org/2001/04/xmlenc#".</exception>
		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "EncryptionProperty" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed encryption property element.");
			}
			this._cachedXml = value;
			this._id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2001/04/xmlenc#");
			this._target = Utils.GetAttribute(value, "Target", "http://www.w3.org/2001/04/xmlenc#");
			this._elemProp = value;
		}

		private string _target;

		private string _id;

		private XmlElement _elemProp;

		private XmlElement _cachedXml;
	}
}
