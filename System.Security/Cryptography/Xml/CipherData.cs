using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the <see langword="&lt;CipherData&gt;" /> element in XML encryption. This class cannot be inherited.</summary>
	public sealed class CipherData
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> class.</summary>
		public CipherData()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> class using a byte array as the <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> value.</summary>
		/// <param name="cipherValue">The encrypted data to use for the <see langword="&lt;CipherValue&gt;" /> element.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="cipherValue" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property has already been set.</exception>
		public CipherData(byte[] cipherValue)
		{
			this.CipherValue = cipherValue;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> class using a <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> object.</summary>
		/// <param name="cipherReference">The <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> object to use.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="cipherValue" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property has already been set.</exception>
		public CipherData(CipherReference cipherReference)
		{
			this.CipherReference = cipherReference;
		}

		private bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		/// <summary>Gets or sets the <see langword="&lt;CipherReference&gt;" /> element.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Xml.CipherReference" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" /> property was set to <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" /> property was set more than once.</exception>
		public CipherReference CipherReference
		{
			get
			{
				return this._cipherReference;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (this.CipherValue != null)
				{
					throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
				}
				this._cipherReference = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the <see langword="&lt;CipherValue&gt;" /> element.</summary>
		/// <returns>A byte array that represents the <see langword="&lt;CipherValue&gt;" /> element.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property was set to <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property was set more than once.</exception>
		public byte[] CipherValue
		{
			get
			{
				return this._cipherValue;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (this.CipherReference != null)
				{
					throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
				}
				this._cipherValue = (byte[])value.Clone();
				this._cachedXml = null;
			}
		}

		/// <summary>Gets the XML values for the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</summary>
		/// <returns>A <see cref="T:System.Xml.XmlElement" /> object that represents the XML information for the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property and the <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" /> property are <see langword="null" />.</exception>
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
			XmlElement xmlElement = document.CreateElement("CipherData", "http://www.w3.org/2001/04/xmlenc#");
			if (this.CipherValue != null)
			{
				XmlElement xmlElement2 = document.CreateElement("CipherValue", "http://www.w3.org/2001/04/xmlenc#");
				xmlElement2.AppendChild(document.CreateTextNode(Convert.ToBase64String(this.CipherValue)));
				xmlElement.AppendChild(xmlElement2);
			}
			else
			{
				if (this.CipherReference == null)
				{
					throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
				}
				xmlElement.AppendChild(this.CipherReference.GetXml(document));
			}
			return xmlElement;
		}

		/// <summary>Loads XML data from an <see cref="T:System.Xml.XmlElement" /> into a <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</summary>
		/// <param name="value">An <see cref="T:System.Xml.XmlElement" /> that represents the XML data to load.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherValue" /> property and the <see cref="P:System.Security.Cryptography.Xml.CipherData.CipherReference" /> property are <see langword="null" />.</exception>
		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			XmlNode xmlNode = value.SelectSingleNode("enc:CipherValue", xmlNamespaceManager);
			XmlNode xmlNode2 = value.SelectSingleNode("enc:CipherReference", xmlNamespaceManager);
			if (xmlNode != null)
			{
				if (xmlNode2 != null)
				{
					throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
				}
				this._cipherValue = Convert.FromBase64String(Utils.DiscardWhiteSpaces(xmlNode.InnerText));
			}
			else
			{
				if (xmlNode2 == null)
				{
					throw new CryptographicException("A Cipher Data element should have either a CipherValue or a CipherReference element.");
				}
				this._cipherReference = new CipherReference();
				this._cipherReference.LoadXml((XmlElement)xmlNode2);
			}
			this._cachedXml = value;
		}

		private XmlElement _cachedXml;

		private CipherReference _cipherReference;

		private byte[] _cipherValue;
	}
}
