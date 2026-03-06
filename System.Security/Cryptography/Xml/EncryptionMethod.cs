using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Encapsulates the encryption algorithm used for XML encryption.</summary>
	public class EncryptionMethod
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class.</summary>
		public EncryptionMethod()
		{
			this._cachedXml = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class specifying an algorithm Uniform Resource Identifier (URI).</summary>
		/// <param name="algorithm">The Uniform Resource Identifier (URI) that describes the algorithm represented by an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class.</param>
		public EncryptionMethod(string algorithm)
		{
			this._algorithm = algorithm;
			this._cachedXml = null;
		}

		private bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		/// <summary>Gets or sets the algorithm key size used for XML encryption.</summary>
		/// <returns>The algorithm key size, in bits, used for XML encryption.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.Security.Cryptography.Xml.EncryptionMethod.KeySize" /> property was set to a value that was less than 0.</exception>
		public int KeySize
		{
			get
			{
				return this._keySize;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", "The key size should be a non negative integer.");
				}
				this._keySize = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets a Uniform Resource Identifier (URI) that describes the algorithm to use for XML encryption.</summary>
		/// <returns>A Uniform Resource Identifier (URI) that describes the algorithm to use for XML encryption.</returns>
		public string KeyAlgorithm
		{
			get
			{
				return this._algorithm;
			}
			set
			{
				this._algorithm = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Returns an <see cref="T:System.Xml.XmlElement" /> object that encapsulates an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlElement" /> object that encapsulates an instance of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> class.</returns>
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
			XmlElement xmlElement = document.CreateElement("EncryptionMethod", "http://www.w3.org/2001/04/xmlenc#");
			if (!string.IsNullOrEmpty(this._algorithm))
			{
				xmlElement.SetAttribute("Algorithm", this._algorithm);
			}
			if (this._keySize > 0)
			{
				XmlElement xmlElement2 = document.CreateElement("KeySize", "http://www.w3.org/2001/04/xmlenc#");
				xmlElement2.AppendChild(document.CreateTextNode(this._keySize.ToString(null, null)));
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement;
		}

		/// <summary>Parses the specified <see cref="T:System.Xml.XmlElement" /> object and configures the internal state of the <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> object to match.</summary>
		/// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object to parse.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The key size expressed in the <paramref name="value" /> parameter was less than 0.</exception>
		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			this._algorithm = Utils.GetAttribute(value, "Algorithm", "http://www.w3.org/2001/04/xmlenc#");
			XmlNode xmlNode = value.SelectSingleNode("enc:KeySize", xmlNamespaceManager);
			if (xmlNode != null)
			{
				this.KeySize = Convert.ToInt32(Utils.DiscardWhiteSpaces(xmlNode.InnerText), null);
			}
			this._cachedXml = value;
		}

		private XmlElement _cachedXml;

		private int _keySize;

		private string _algorithm;
	}
}
