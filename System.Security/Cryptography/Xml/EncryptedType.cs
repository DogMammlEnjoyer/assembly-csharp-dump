using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Represents the abstract base class from which the classes <see cref="T:System.Security.Cryptography.Xml.EncryptedData" /> and <see cref="T:System.Security.Cryptography.Xml.EncryptedKey" /> derive.</summary>
	public abstract class EncryptedType
	{
		internal bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		/// <summary>Gets or sets the <see langword="Id" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
		/// <returns>A string of the <see langword="Id" /> attribute of the <see langword="&lt;EncryptedType&gt;" /> element.</returns>
		public virtual string Id
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

		/// <summary>Gets or sets the <see langword="Type" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
		/// <returns>A string that describes the text form of the encrypted data.</returns>
		public virtual string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the <see langword="MimeType" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
		/// <returns>A string that describes the media type of the encrypted data.</returns>
		public virtual string MimeType
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

		/// <summary>Gets or sets the <see langword="Encoding" /> attribute of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> instance in XML encryption.</summary>
		/// <returns>A string that describes the encoding of the encrypted data.</returns>
		public virtual string Encoding
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

		/// <summary>Gets of sets the <see langword="&lt;KeyInfo&gt;" /> element in XML encryption.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object.</returns>
		public KeyInfo KeyInfo
		{
			get
			{
				if (this._keyInfo == null)
				{
					this._keyInfo = new KeyInfo();
				}
				return this._keyInfo;
			}
			set
			{
				this._keyInfo = value;
			}
		}

		/// <summary>Gets or sets the <see langword="&lt;EncryptionMethod&gt;" /> element for XML encryption.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptionMethod" /> object that represents the <see langword="&lt;EncryptionMethod&gt;" /> element.</returns>
		public virtual EncryptionMethod EncryptionMethod
		{
			get
			{
				return this._encryptionMethod;
			}
			set
			{
				this._encryptionMethod = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Gets or sets the <see langword="&lt;EncryptionProperties&gt;" /> element in XML encryption.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptionPropertyCollection" /> object.</returns>
		public virtual EncryptionPropertyCollection EncryptionProperties
		{
			get
			{
				if (this._props == null)
				{
					this._props = new EncryptionPropertyCollection();
				}
				return this._props;
			}
		}

		/// <summary>Adds an <see langword="&lt;EncryptionProperty&gt;" /> child element to the <see langword="&lt;EncryptedProperties&gt;" /> element in the current <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> object in XML encryption.</summary>
		/// <param name="ep">An <see cref="T:System.Security.Cryptography.Xml.EncryptionProperty" /> object.</param>
		public void AddProperty(EncryptionProperty ep)
		{
			this.EncryptionProperties.Add(ep);
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.CipherData" /> value for an instance of an <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> class.</summary>
		/// <returns>A <see cref="T:System.Security.Cryptography.Xml.CipherData" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Security.Cryptography.Xml.EncryptedType.CipherData" /> property was set to <see langword="null" />.</exception>
		public virtual CipherData CipherData
		{
			get
			{
				if (this._cipherData == null)
				{
					this._cipherData = new CipherData();
				}
				return this._cipherData;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._cipherData = value;
				this._cachedXml = null;
			}
		}

		/// <summary>Loads XML information into the <see langword="&lt;EncryptedType&gt;" /> element in XML encryption.</summary>
		/// <param name="value">An <see cref="T:System.Xml.XmlElement" /> object representing an XML element to use in the <see langword="&lt;EncryptedType&gt;" /> element.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> provided is <see langword="null" />.</exception>
		public abstract void LoadXml(XmlElement value);

		/// <summary>Returns the XML representation of the <see cref="T:System.Security.Cryptography.Xml.EncryptedType" /> object.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlElement" /> object that represents the <see langword="&lt;EncryptedType&gt;" /> element in XML encryption.</returns>
		public abstract XmlElement GetXml();

		private string _id;

		private string _type;

		private string _mimeType;

		private string _encoding;

		private EncryptionMethod _encryptionMethod;

		private CipherData _cipherData;

		private EncryptionPropertyCollection _props;

		private KeyInfo _keyInfo;

		internal XmlElement _cachedXml;
	}
}
