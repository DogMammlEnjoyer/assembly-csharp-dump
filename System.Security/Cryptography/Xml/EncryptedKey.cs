using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the <see langword="&lt;EncryptedKey&gt;" /> element in XML encryption. This class cannot be inherited.</summary>
	public sealed class EncryptedKey : EncryptedType
	{
		/// <summary>Gets or sets the optional <see langword="Recipient" /> attribute in XML encryption.</summary>
		/// <returns>A string representing the value of the <see langword="Recipient" /> attribute.</returns>
		public string Recipient
		{
			get
			{
				if (this._recipient == null)
				{
					this._recipient = string.Empty;
				}
				return this._recipient;
			}
			set
			{
				this._recipient = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the optional <see langword="&lt;CarriedKeyName&gt;" /> element in XML encryption.</summary>
		/// <returns>A string that represents a name for the key value.</returns>
		public string CarriedKeyName
		{
			get
			{
				return this._carriedKeyName;
			}
			set
			{
				this._carriedKeyName = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the <see langword="&lt;ReferenceList&gt;" /> element in XML encryption.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Xml.ReferenceList" /> object.</returns>
		public ReferenceList ReferenceList
		{
			get
			{
				if (this._referenceList == null)
				{
					this._referenceList = new ReferenceList();
				}
				return this._referenceList;
			}
		}

		/// <summary>Adds a <see langword="&lt;DataReference&gt;" /> element to the <see langword="&lt;ReferenceList&gt;" /> element.</summary>
		/// <param name="dataReference">A <see cref="T:System.Security.Cryptography.Xml.DataReference" /> object to add to the <see cref="P:System.Security.Cryptography.Xml.EncryptedKey.ReferenceList" /> property.</param>
		public void AddReference(DataReference dataReference)
		{
			this.ReferenceList.Add(dataReference);
		}

		/// <summary>Adds a <see langword="&lt;KeyReference&gt;" /> element to the <see langword="&lt;ReferenceList&gt;" /> element.</summary>
		/// <param name="keyReference">A <see cref="T:System.Security.Cryptography.Xml.KeyReference" /> object to add to the <see cref="P:System.Security.Cryptography.Xml.EncryptedKey.ReferenceList" /> property.</param>
		public void AddReference(KeyReference keyReference)
		{
			this.ReferenceList.Add(keyReference);
		}

		/// <summary>Loads the specified XML information into the <see langword="&lt;EncryptedKey&gt;" /> element in XML encryption.</summary>
		/// <param name="value">An <see cref="T:System.Xml.XmlElement" /> representing an XML element to use for the <see langword="&lt;EncryptedKey&gt;" /> element.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter does not contain a <see cref="T:System.Security.Cryptography.Xml.CipherData" /> element.</exception>
		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(value.OwnerDocument.NameTable);
			xmlNamespaceManager.AddNamespace("enc", "http://www.w3.org/2001/04/xmlenc#");
			xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
			this.Id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2001/04/xmlenc#");
			this.Type = Utils.GetAttribute(value, "Type", "http://www.w3.org/2001/04/xmlenc#");
			this.MimeType = Utils.GetAttribute(value, "MimeType", "http://www.w3.org/2001/04/xmlenc#");
			this.Encoding = Utils.GetAttribute(value, "Encoding", "http://www.w3.org/2001/04/xmlenc#");
			this.Recipient = Utils.GetAttribute(value, "Recipient", "http://www.w3.org/2001/04/xmlenc#");
			XmlNode xmlNode = value.SelectSingleNode("enc:EncryptionMethod", xmlNamespaceManager);
			this.EncryptionMethod = new EncryptionMethod();
			if (xmlNode != null)
			{
				this.EncryptionMethod.LoadXml(xmlNode as XmlElement);
			}
			base.KeyInfo = new KeyInfo();
			XmlNode xmlNode2 = value.SelectSingleNode("ds:KeyInfo", xmlNamespaceManager);
			if (xmlNode2 != null)
			{
				base.KeyInfo.LoadXml(xmlNode2 as XmlElement);
			}
			XmlNode xmlNode3 = value.SelectSingleNode("enc:CipherData", xmlNamespaceManager);
			if (xmlNode3 == null)
			{
				throw new CryptographicException("Cipher data is not specified.");
			}
			this.CipherData = new CipherData();
			this.CipherData.LoadXml(xmlNode3 as XmlElement);
			XmlNode xmlNode4 = value.SelectSingleNode("enc:EncryptionProperties", xmlNamespaceManager);
			if (xmlNode4 != null)
			{
				XmlNodeList xmlNodeList = xmlNode4.SelectNodes("enc:EncryptionProperty", xmlNamespaceManager);
				if (xmlNodeList != null)
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode5 = (XmlNode)obj;
						EncryptionProperty encryptionProperty = new EncryptionProperty();
						encryptionProperty.LoadXml(xmlNode5 as XmlElement);
						this.EncryptionProperties.Add(encryptionProperty);
					}
				}
			}
			XmlNode xmlNode6 = value.SelectSingleNode("enc:CarriedKeyName", xmlNamespaceManager);
			if (xmlNode6 != null)
			{
				this.CarriedKeyName = xmlNode6.InnerText;
			}
			XmlNode xmlNode7 = value.SelectSingleNode("enc:ReferenceList", xmlNamespaceManager);
			if (xmlNode7 != null)
			{
				XmlNodeList xmlNodeList2 = xmlNode7.SelectNodes("enc:DataReference", xmlNamespaceManager);
				if (xmlNodeList2 != null)
				{
					foreach (object obj2 in xmlNodeList2)
					{
						XmlNode xmlNode8 = (XmlNode)obj2;
						DataReference dataReference = new DataReference();
						dataReference.LoadXml(xmlNode8 as XmlElement);
						this.ReferenceList.Add(dataReference);
					}
				}
				XmlNodeList xmlNodeList3 = xmlNode7.SelectNodes("enc:KeyReference", xmlNamespaceManager);
				if (xmlNodeList3 != null)
				{
					foreach (object obj3 in xmlNodeList3)
					{
						XmlNode xmlNode9 = (XmlNode)obj3;
						KeyReference keyReference = new KeyReference();
						keyReference.LoadXml(xmlNode9 as XmlElement);
						this.ReferenceList.Add(keyReference);
					}
				}
			}
			this._cachedXml = value;
		}

		/// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> object.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlElement" /> that represents the <see langword="&lt;EncryptedKey&gt;" /> element in XML encryption.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> value is <see langword="null" />.</exception>
		public override XmlElement GetXml()
		{
			if (base.CacheValid)
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
			XmlElement xmlElement = document.CreateElement("EncryptedKey", "http://www.w3.org/2001/04/xmlenc#");
			if (!string.IsNullOrEmpty(this.Id))
			{
				xmlElement.SetAttribute("Id", this.Id);
			}
			if (!string.IsNullOrEmpty(this.Type))
			{
				xmlElement.SetAttribute("Type", this.Type);
			}
			if (!string.IsNullOrEmpty(this.MimeType))
			{
				xmlElement.SetAttribute("MimeType", this.MimeType);
			}
			if (!string.IsNullOrEmpty(this.Encoding))
			{
				xmlElement.SetAttribute("Encoding", this.Encoding);
			}
			if (!string.IsNullOrEmpty(this.Recipient))
			{
				xmlElement.SetAttribute("Recipient", this.Recipient);
			}
			if (this.EncryptionMethod != null)
			{
				xmlElement.AppendChild(this.EncryptionMethod.GetXml(document));
			}
			if (base.KeyInfo.Count > 0)
			{
				xmlElement.AppendChild(base.KeyInfo.GetXml(document));
			}
			if (this.CipherData == null)
			{
				throw new CryptographicException("Cipher data is not specified.");
			}
			xmlElement.AppendChild(this.CipherData.GetXml(document));
			if (this.EncryptionProperties.Count > 0)
			{
				XmlElement xmlElement2 = document.CreateElement("EncryptionProperties", "http://www.w3.org/2001/04/xmlenc#");
				for (int i = 0; i < this.EncryptionProperties.Count; i++)
				{
					EncryptionProperty encryptionProperty = this.EncryptionProperties.Item(i);
					xmlElement2.AppendChild(encryptionProperty.GetXml(document));
				}
				xmlElement.AppendChild(xmlElement2);
			}
			if (this.ReferenceList.Count > 0)
			{
				XmlElement xmlElement3 = document.CreateElement("ReferenceList", "http://www.w3.org/2001/04/xmlenc#");
				for (int j = 0; j < this.ReferenceList.Count; j++)
				{
					xmlElement3.AppendChild(this.ReferenceList[j].GetXml(document));
				}
				xmlElement.AppendChild(xmlElement3);
			}
			if (this.CarriedKeyName != null)
			{
				XmlElement xmlElement4 = document.CreateElement("CarriedKeyName", "http://www.w3.org/2001/04/xmlenc#");
				XmlText newChild = document.CreateTextNode(this.CarriedKeyName);
				xmlElement4.AppendChild(newChild);
				xmlElement.AppendChild(xmlElement4);
			}
			return xmlElement;
		}

		private string _recipient;

		private string _carriedKeyName;

		private ReferenceList _referenceList;
	}
}
