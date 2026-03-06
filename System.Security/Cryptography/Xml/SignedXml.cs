using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	/// <summary>Provides a wrapper on a core XML signature object to facilitate creating XML signatures.</summary>
	public class SignedXml
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> class.</summary>
		public SignedXml()
		{
			this.Initialize(null);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> class from the specified XML document.</summary>
		/// <param name="document">The <see cref="T:System.Xml.XmlDocument" /> object to use to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.SignedXml" />.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="document" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="document" /> parameter contains a null <see cref="P:System.Xml.XmlDocument.DocumentElement" /> property.</exception>
		public SignedXml(XmlDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this.Initialize(document.DocumentElement);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> class from the specified <see cref="T:System.Xml.XmlElement" /> object.</summary>
		/// <param name="elem">The <see cref="T:System.Xml.XmlElement" /> object to use to initialize the new instance of <see cref="T:System.Security.Cryptography.Xml.SignedXml" />.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="elem" /> parameter is <see langword="null" />.</exception>
		public SignedXml(XmlElement elem)
		{
			if (elem == null)
			{
				throw new ArgumentNullException("elem");
			}
			this.Initialize(elem);
		}

		private void Initialize(XmlElement element)
		{
			this._containingDocument = ((element == null) ? null : element.OwnerDocument);
			this._context = element;
			this.m_signature = new Signature();
			this.m_signature.SignedXml = this;
			this.m_signature.SignedInfo = new SignedInfo();
			this._signingKey = null;
			this._safeCanonicalizationMethods = new Collection<string>(SignedXml.KnownCanonicalizationMethods);
		}

		/// <summary>Gets or sets the name of the installed key to be used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The name of the installed key to be used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public string SigningKeyName
		{
			get
			{
				return this.m_strSigningKeyName;
			}
			set
			{
				this.m_strSigningKeyName = value;
			}
		}

		/// <summary>Sets the current <see cref="T:System.Xml.XmlResolver" /> object.</summary>
		/// <returns>The current <see cref="T:System.Xml.XmlResolver" /> object. The defaults is a <see cref="T:System.Xml.XmlSecureResolver" /> object.</returns>
		public XmlResolver Resolver
		{
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

		/// <summary>Gets a delegate that will be called to validate the format (not the cryptographic security) of an XML signature.</summary>
		/// <returns>
		///   <see langword="true" /> if the format is acceptable; otherwise, <see langword="false" />.</returns>
		public Func<SignedXml, bool> SignatureFormatValidator
		{
			get
			{
				return this._signatureFormatValidator;
			}
			set
			{
				this._signatureFormatValidator = value;
			}
		}

		/// <summary>Gets the names of methods whose canonicalization algorithms are explicitly allowed.</summary>
		/// <returns>A collection of the names of methods that safely produce canonical XML.</returns>
		public Collection<string> SafeCanonicalizationMethods
		{
			get
			{
				return this._safeCanonicalizationMethods;
			}
		}

		/// <summary>Gets or sets the asymmetric algorithm key used for signing a <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The asymmetric algorithm key used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public AsymmetricAlgorithm SigningKey
		{
			get
			{
				return this._signingKey;
			}
			set
			{
				this._signingKey = value;
			}
		}

		/// <summary>Gets or sets an <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> object that defines the XML encryption processing rules.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.Xml.EncryptedXml" /> object that defines the XML encryption processing rules.</returns>
		public EncryptedXml EncryptedXml
		{
			get
			{
				if (this._exml == null)
				{
					this._exml = new EncryptedXml(this._containingDocument);
				}
				return this._exml;
			}
			set
			{
				this._exml = value;
			}
		}

		/// <summary>Gets the <see cref="T:System.Security.Cryptography.Xml.Signature" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The <see cref="T:System.Security.Cryptography.Xml.Signature" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public Signature Signature
		{
			get
			{
				return this.m_signature;
			}
		}

		/// <summary>Gets the <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The <see cref="T:System.Security.Cryptography.Xml.SignedInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public SignedInfo SignedInfo
		{
			get
			{
				return this.m_signature.SignedInfo;
			}
		}

		/// <summary>Gets the signature method of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The signature method of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public string SignatureMethod
		{
			get
			{
				return this.m_signature.SignedInfo.SignatureMethod;
			}
		}

		/// <summary>Gets the length of the signature for the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The length of the signature for the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public string SignatureLength
		{
			get
			{
				return this.m_signature.SignedInfo.SignatureLength;
			}
		}

		/// <summary>Gets the signature value of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>A byte array that contains the signature value of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public byte[] SignatureValue
		{
			get
			{
				return this.m_signature.SignatureValue;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The <see cref="T:System.Security.Cryptography.Xml.KeyInfo" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</returns>
		public KeyInfo KeyInfo
		{
			get
			{
				return this.m_signature.KeyInfo;
			}
			set
			{
				this.m_signature.KeyInfo = value;
			}
		}

		/// <summary>Returns the XML representation of a <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		/// <returns>The XML representation of the <see cref="T:System.Security.Cryptography.Xml.Signature" /> object.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignedInfo" /> property is <see langword="null" />.  
		///  -or-  
		///  The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureValue" /> property is <see langword="null" />.</exception>
		public XmlElement GetXml()
		{
			if (this._containingDocument != null)
			{
				return this.m_signature.GetXml(this._containingDocument);
			}
			return this.m_signature.GetXml();
		}

		/// <summary>Loads a <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> state from an XML element.</summary>
		/// <param name="value">The XML element to load the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> state from.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureValue" /> property.  
		///  -or-  
		///  The <paramref name="value" /> parameter does not contain a valid <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignedInfo" /> property.</exception>
		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.m_signature.LoadXml(value);
			if (this._context == null)
			{
				this._context = value;
			}
			this._bCacheValid = false;
		}

		/// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.Reference" /> object to the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object that describes a digest method, digest value, and transform to use for creating an XML digital signature.</summary>
		/// <param name="reference">The  <see cref="T:System.Security.Cryptography.Xml.Reference" /> object that describes a digest method, digest value, and transform to use for creating an XML digital signature.</param>
		public void AddReference(Reference reference)
		{
			this.m_signature.SignedInfo.AddReference(reference);
		}

		/// <summary>Adds a <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object to the list of objects to be signed.</summary>
		/// <param name="dataObject">The <see cref="T:System.Security.Cryptography.Xml.DataObject" /> object to add to the list of objects to be signed.</param>
		public void AddObject(DataObject dataObject)
		{
			this.m_signature.AddObject(dataObject);
		}

		/// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies using the public key in the signature.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.SignatureAlgorithm" /> property of the public key in the signature does not match the <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureMethod" /> property.  
		///  -or-  
		///  The signature description could not be created.  
		///  -or  
		///  The hash algorithm could not be created.</exception>
		public bool CheckSignature()
		{
			AsymmetricAlgorithm asymmetricAlgorithm;
			return this.CheckSignatureReturningKey(out asymmetricAlgorithm);
		}

		/// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies using the public key in the signature.</summary>
		/// <param name="signingKey">When this method returns, contains the implementation of <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> that holds the public key in the signature. This parameter is passed uninitialized.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies using the public key in the signature; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="signingKey" /> parameter is null.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.SignatureAlgorithm" /> property of the public key in the signature does not match the <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureMethod" /> property.  
		///  -or-  
		///  The signature description could not be created.  
		///  -or  
		///  The hash algorithm could not be created.</exception>
		public bool CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)
		{
			SignedXmlDebugLog.LogBeginSignatureVerification(this, this._context);
			signingKey = null;
			bool flag = false;
			if (!this.CheckSignatureFormat())
			{
				return false;
			}
			AsymmetricAlgorithm publicKey;
			do
			{
				publicKey = this.GetPublicKey();
				if (publicKey != null)
				{
					flag = this.CheckSignature(publicKey);
					SignedXmlDebugLog.LogVerificationResult(this, publicKey, flag);
				}
			}
			while (publicKey != null && !flag);
			signingKey = publicKey;
			return flag;
		}

		/// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified key.</summary>
		/// <param name="key">The implementation of the <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> property that holds the key to be used to verify the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="key" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.SignatureAlgorithm" /> property of the <paramref name="key" /> parameter does not match the <see cref="P:System.Security.Cryptography.Xml.SignedXml.SignatureMethod" /> property.  
		///  -or-  
		///  The signature description could not be created.  
		///  -or  
		///  The hash algorithm could not be created.</exception>
		public bool CheckSignature(AsymmetricAlgorithm key)
		{
			if (!this.CheckSignatureFormat())
			{
				return false;
			}
			if (!this.CheckSignedInfo(key))
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "SignedInfo");
				return false;
			}
			if (!this.CheckDigestedReferences())
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "references");
				return false;
			}
			SignedXmlDebugLog.LogVerificationResult(this, key, true);
			return true;
		}

		/// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified message authentication code (MAC) algorithm.</summary>
		/// <param name="macAlg">The implementation of <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> that holds the MAC to be used to verify the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified MAC; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="macAlg" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.HashAlgorithm.HashSize" /> property of the specified <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object is not valid.  
		///  -or-  
		///  The <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property is <see langword="null" />.  
		///  -or-  
		///  The cryptographic transform used to check the signature could not be created.</exception>
		public bool CheckSignature(KeyedHashAlgorithm macAlg)
		{
			if (!this.CheckSignatureFormat())
			{
				return false;
			}
			if (!this.CheckSignedInfo(macAlg))
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "SignedInfo");
				return false;
			}
			if (!this.CheckDigestedReferences())
			{
				SignedXmlDebugLog.LogVerificationFailure(this, "references");
				return false;
			}
			SignedXmlDebugLog.LogVerificationResult(this, macAlg, true);
			return true;
		}

		/// <summary>Determines whether the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property verifies for the specified <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> object and, optionally, whether the certificate is valid.</summary>
		/// <param name="certificate">The <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2" /> object to use to verify the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property.</param>
		/// <param name="verifySignatureOnly">
		///   <see langword="true" /> to verify the signature only; <see langword="false" /> to verify both the signature and certificate.</param>
		/// <returns>
		///   <see langword="true" /> if the signature is valid; otherwise, <see langword="false" />.  
		/// -or-  
		/// <see langword="true" /> if the signature and certificate are valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="certificate" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">A signature description could not be created for the <paramref name="certificate" /> parameter.</exception>
		public bool CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)
		{
			if (!verifySignatureOnly)
			{
				foreach (X509Extension x509Extension in certificate.Extensions)
				{
					if (string.Compare(x509Extension.Oid.Value, "2.5.29.15", StringComparison.OrdinalIgnoreCase) == 0)
					{
						X509KeyUsageExtension x509KeyUsageExtension = new X509KeyUsageExtension();
						x509KeyUsageExtension.CopyFrom(x509Extension);
						SignedXmlDebugLog.LogVerifyKeyUsage(this, certificate, x509KeyUsageExtension);
						if ((x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.None && (x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.NonRepudiation) <= X509KeyUsageFlags.None)
						{
							SignedXmlDebugLog.LogVerificationFailure(this, "X509 key usage verification");
							return false;
						}
						break;
					}
				}
				X509Chain x509Chain = new X509Chain();
				x509Chain.ChainPolicy.ExtraStore.AddRange(this.BuildBagOfCerts());
				bool flag = x509Chain.Build(certificate);
				SignedXmlDebugLog.LogVerifyX509Chain(this, x509Chain, certificate);
				if (!flag)
				{
					SignedXmlDebugLog.LogVerificationFailure(this, "X509 chain verification");
					return false;
				}
			}
			using (AsymmetricAlgorithm anyPublicKey = Utils.GetAnyPublicKey(certificate))
			{
				if (!this.CheckSignature(anyPublicKey))
				{
					return false;
				}
			}
			SignedXmlDebugLog.LogVerificationResult(this, certificate, true);
			return true;
		}

		/// <summary>Computes an XML digital signature.</summary>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SigningKey" /> property is <see langword="null" />.  
		///  -or-  
		///  The <see cref="P:System.Security.Cryptography.Xml.SignedXml.SigningKey" /> property is not a <see cref="T:System.Security.Cryptography.DSA" /> object or <see cref="T:System.Security.Cryptography.RSA" /> object.  
		///  -or-  
		///  The key could not be loaded.</exception>
		public void ComputeSignature()
		{
			SignedXmlDebugLog.LogBeginSignatureComputation(this, this._context);
			this.BuildDigestedReferences();
			AsymmetricAlgorithm signingKey = this.SigningKey;
			if (signingKey == null)
			{
				throw new CryptographicException("Signing key is not loaded.");
			}
			if (this.SignedInfo.SignatureMethod == null)
			{
				if (signingKey is DSA)
				{
					this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
				}
				else
				{
					if (!(signingKey is RSA))
					{
						throw new CryptographicException("Failed to create signing key.");
					}
					if (this.SignedInfo.SignatureMethod == null)
					{
						this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
					}
				}
			}
			SignatureDescription signatureDescription = CryptoHelpers.CreateFromName<SignatureDescription>(this.SignedInfo.SignatureMethod);
			if (signatureDescription == null)
			{
				throw new CryptographicException("SignatureDescription could not be created for the signature algorithm supplied.");
			}
			HashAlgorithm hashAlgorithm = signatureDescription.CreateDigest();
			if (hashAlgorithm == null)
			{
				throw new CryptographicException("Could not create hash algorithm object.");
			}
			this.GetC14NDigest(hashAlgorithm);
			AsymmetricSignatureFormatter asymmetricSignatureFormatter = signatureDescription.CreateFormatter(signingKey);
			SignedXmlDebugLog.LogSigning(this, signingKey, signatureDescription, hashAlgorithm, asymmetricSignatureFormatter);
			this.m_signature.SignatureValue = asymmetricSignatureFormatter.CreateSignature(hashAlgorithm);
		}

		/// <summary>Computes an XML digital signature using the specified message authentication code (MAC) algorithm.</summary>
		/// <param name="macAlg">A <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object that holds the MAC to be used to compute the value of the <see cref="P:System.Security.Cryptography.Xml.SignedXml.Signature" /> property.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="macAlg" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object specified by the <paramref name="macAlg" /> parameter is not an instance of <see cref="T:System.Security.Cryptography.HMACSHA1" />.  
		///  -or-  
		///  The <see cref="P:System.Security.Cryptography.HashAlgorithm.HashSize" /> property of the specified <see cref="T:System.Security.Cryptography.KeyedHashAlgorithm" /> object is not valid.  
		///  -or-  
		///  The cryptographic transform used to check the signature could not be created.</exception>
		public void ComputeSignature(KeyedHashAlgorithm macAlg)
		{
			if (macAlg == null)
			{
				throw new ArgumentNullException("macAlg");
			}
			HMAC hmac = macAlg as HMAC;
			if (hmac == null)
			{
				throw new CryptographicException("The key does not fit the SignatureMethod.");
			}
			int num;
			if (this.m_signature.SignedInfo.SignatureLength == null)
			{
				num = hmac.HashSize;
			}
			else
			{
				num = Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, null);
			}
			if (num < 0 || num > hmac.HashSize)
			{
				throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
			}
			if (num % 8 != 0)
			{
				throw new CryptographicException("The length in bits of the signature with a MAC should be a multiple of 8.");
			}
			this.BuildDigestedReferences();
			string hashName = hmac.HashName;
			if (!(hashName == "SHA1"))
			{
				if (!(hashName == "SHA256"))
				{
					if (!(hashName == "SHA384"))
					{
						if (!(hashName == "SHA512"))
						{
							if (!(hashName == "MD5"))
							{
								if (!(hashName == "RIPEMD160"))
								{
									throw new CryptographicException("The key does not fit the SignatureMethod.");
								}
								this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
							}
							else
							{
								this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";
							}
						}
						else
						{
							this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
						}
					}
					else
					{
						this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
					}
				}
				else
				{
					this.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
				}
			}
			else
			{
				this.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
			}
			Array c14NDigest = this.GetC14NDigest(hmac);
			SignedXmlDebugLog.LogSigning(this, hmac);
			this.m_signature.SignatureValue = new byte[num / 8];
			Buffer.BlockCopy(c14NDigest, 0, this.m_signature.SignatureValue, 0, num / 8);
		}

		/// <summary>Returns the public key of a signature.</summary>
		/// <returns>An <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> object that contains the public key of the signature, or <see langword="null" /> if the key cannot be found.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The <see cref="P:System.Security.Cryptography.Xml.SignedXml.KeyInfo" /> property is <see langword="null" />.</exception>
		protected virtual AsymmetricAlgorithm GetPublicKey()
		{
			if (this.KeyInfo == null)
			{
				throw new CryptographicException("A KeyInfo element is required to check the signature.");
			}
			if (this._x509Enum != null)
			{
				AsymmetricAlgorithm nextCertificatePublicKey = this.GetNextCertificatePublicKey();
				if (nextCertificatePublicKey != null)
				{
					return nextCertificatePublicKey;
				}
			}
			if (this._keyInfoEnum == null)
			{
				this._keyInfoEnum = this.KeyInfo.GetEnumerator();
			}
			while (this._keyInfoEnum.MoveNext())
			{
				RSAKeyValue rsakeyValue = this._keyInfoEnum.Current as RSAKeyValue;
				if (rsakeyValue != null)
				{
					return rsakeyValue.Key;
				}
				DSAKeyValue dsakeyValue = this._keyInfoEnum.Current as DSAKeyValue;
				if (dsakeyValue != null)
				{
					return dsakeyValue.Key;
				}
				KeyInfoX509Data keyInfoX509Data = this._keyInfoEnum.Current as KeyInfoX509Data;
				if (keyInfoX509Data != null)
				{
					this._x509Collection = Utils.BuildBagOfCerts(keyInfoX509Data, CertUsageType.Verification);
					if (this._x509Collection.Count > 0)
					{
						this._x509Enum = this._x509Collection.GetEnumerator();
						AsymmetricAlgorithm nextCertificatePublicKey2 = this.GetNextCertificatePublicKey();
						if (nextCertificatePublicKey2 != null)
						{
							return nextCertificatePublicKey2;
						}
					}
				}
			}
			return null;
		}

		private X509Certificate2Collection BuildBagOfCerts()
		{
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			if (this.KeyInfo != null)
			{
				foreach (object obj in this.KeyInfo)
				{
					KeyInfoX509Data keyInfoX509Data = ((KeyInfoClause)obj) as KeyInfoX509Data;
					if (keyInfoX509Data != null)
					{
						x509Certificate2Collection.AddRange(Utils.BuildBagOfCerts(keyInfoX509Data, CertUsageType.Verification));
					}
				}
			}
			return x509Certificate2Collection;
		}

		private AsymmetricAlgorithm GetNextCertificatePublicKey()
		{
			while (this._x509Enum.MoveNext())
			{
				X509Certificate2 x509Certificate = (X509Certificate2)this._x509Enum.Current;
				if (x509Certificate != null)
				{
					return Utils.GetAnyPublicKey(x509Certificate);
				}
			}
			return null;
		}

		/// <summary>Returns the <see cref="T:System.Xml.XmlElement" /> object with the specified ID from the specified <see cref="T:System.Xml.XmlDocument" /> object.</summary>
		/// <param name="document">The <see cref="T:System.Xml.XmlDocument" /> object to retrieve the <see cref="T:System.Xml.XmlElement" /> object from.</param>
		/// <param name="idValue">The ID of the <see cref="T:System.Xml.XmlElement" /> object to retrieve from the <see cref="T:System.Xml.XmlDocument" /> object.</param>
		/// <returns>The <see cref="T:System.Xml.XmlElement" /> object with the specified ID from the specified <see cref="T:System.Xml.XmlDocument" /> object, or <see langword="null" /> if it could not be found.</returns>
		public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
		{
			return SignedXml.DefaultGetIdElement(document, idValue);
		}

		internal static XmlElement DefaultGetIdElement(XmlDocument document, string idValue)
		{
			if (document == null)
			{
				return null;
			}
			try
			{
				XmlConvert.VerifyNCName(idValue);
			}
			catch (XmlException)
			{
				return null;
			}
			XmlElement xmlElement = document.GetElementById(idValue);
			if (xmlElement != null)
			{
				XmlDocument xmlDocument = (XmlDocument)document.CloneNode(true);
				XmlElement elementById = xmlDocument.GetElementById(idValue);
				if (elementById != null)
				{
					elementById.Attributes.RemoveAll();
					if (xmlDocument.GetElementById(idValue) != null)
					{
						throw new CryptographicException("Malformed reference element.");
					}
				}
				return xmlElement;
			}
			xmlElement = SignedXml.GetSingleReferenceTarget(document, "Id", idValue);
			if (xmlElement != null)
			{
				return xmlElement;
			}
			xmlElement = SignedXml.GetSingleReferenceTarget(document, "id", idValue);
			if (xmlElement != null)
			{
				return xmlElement;
			}
			return SignedXml.GetSingleReferenceTarget(document, "ID", idValue);
		}

		private static bool DefaultSignatureFormatValidator(SignedXml signedXml)
		{
			return !signedXml.DoesSignatureUseTruncatedHmac() && signedXml.DoesSignatureUseSafeCanonicalizationMethod();
		}

		private bool DoesSignatureUseTruncatedHmac()
		{
			if (this.SignedInfo.SignatureLength == null)
			{
				return false;
			}
			HMAC hmac = CryptoHelpers.CreateFromName<HMAC>(this.SignatureMethod);
			if (hmac == null)
			{
				return false;
			}
			int num = 0;
			return !int.TryParse(this.SignedInfo.SignatureLength, out num) || num != hmac.HashSize;
		}

		private bool DoesSignatureUseSafeCanonicalizationMethod()
		{
			using (IEnumerator<string> enumerator = this.SafeCanonicalizationMethods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (string.Equals(enumerator.Current, this.SignedInfo.CanonicalizationMethod, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			SignedXmlDebugLog.LogUnsafeCanonicalizationMethod(this, this.SignedInfo.CanonicalizationMethod, this.SafeCanonicalizationMethods);
			return false;
		}

		private bool ReferenceUsesSafeTransformMethods(Reference reference)
		{
			TransformChain transformChain = reference.TransformChain;
			int count = transformChain.Count;
			for (int i = 0; i < count; i++)
			{
				Transform transform = transformChain[i];
				if (!this.IsSafeTransform(transform.Algorithm))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsSafeTransform(string transformAlgorithm)
		{
			using (IEnumerator<string> enumerator = this.SafeCanonicalizationMethods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (string.Equals(enumerator.Current, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			using (IEnumerator<string> enumerator = SignedXml.DefaultSafeTransformMethods.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (string.Equals(enumerator.Current, transformAlgorithm, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			SignedXmlDebugLog.LogUnsafeTransformMethod(this, transformAlgorithm, this.SafeCanonicalizationMethods, SignedXml.DefaultSafeTransformMethods);
			return false;
		}

		private static IList<string> KnownCanonicalizationMethods
		{
			get
			{
				if (SignedXml.s_knownCanonicalizationMethods == null)
				{
					SignedXml.s_knownCanonicalizationMethods = new List<string>
					{
						"http://www.w3.org/TR/2001/REC-xml-c14n-20010315",
						"http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments",
						"http://www.w3.org/2001/10/xml-exc-c14n#",
						"http://www.w3.org/2001/10/xml-exc-c14n#WithComments"
					};
				}
				return SignedXml.s_knownCanonicalizationMethods;
			}
		}

		private static IList<string> DefaultSafeTransformMethods
		{
			get
			{
				if (SignedXml.s_defaultSafeTransformMethods == null)
				{
					SignedXml.s_defaultSafeTransformMethods = new List<string>
					{
						"http://www.w3.org/2000/09/xmldsig#enveloped-signature",
						"http://www.w3.org/2000/09/xmldsig#base64",
						"urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform",
						"http://www.w3.org/2002/07/decrypt#XML"
					};
				}
				return SignedXml.s_defaultSafeTransformMethods;
			}
		}

		private byte[] GetC14NDigest(HashAlgorithm hash)
		{
			bool flag = hash is KeyedHashAlgorithm;
			if (flag || !this._bCacheValid || !this.SignedInfo.CacheValid)
			{
				string text = (this._containingDocument == null) ? null : this._containingDocument.BaseURI;
				XmlResolver xmlResolver = this._bResolverSet ? this._xmlResolver : new XmlSecureResolver(new XmlUrlResolver(), text);
				XmlDocument xmlDocument = Utils.PreProcessElementInput(this.SignedInfo.GetXml(), xmlResolver, text);
				CanonicalXmlNodeList namespaces = (this._context == null) ? null : Utils.GetPropagatedAttributes(this._context);
				SignedXmlDebugLog.LogNamespacePropagation(this, namespaces);
				Utils.AddNamespaces(xmlDocument.DocumentElement, namespaces);
				Transform canonicalizationMethodObject = this.SignedInfo.CanonicalizationMethodObject;
				canonicalizationMethodObject.Resolver = xmlResolver;
				canonicalizationMethodObject.BaseURI = text;
				SignedXmlDebugLog.LogBeginCanonicalization(this, canonicalizationMethodObject);
				canonicalizationMethodObject.LoadInput(xmlDocument);
				SignedXmlDebugLog.LogCanonicalizedOutput(this, canonicalizationMethodObject);
				this._digestedSignedInfo = canonicalizationMethodObject.GetDigestedOutput(hash);
				this._bCacheValid = !flag;
			}
			return this._digestedSignedInfo;
		}

		private int GetReferenceLevel(int index, ArrayList references)
		{
			if (this._refProcessed[index])
			{
				return this._refLevelCache[index];
			}
			this._refProcessed[index] = true;
			Reference reference = (Reference)references[index];
			if (reference.Uri == null || reference.Uri.Length == 0 || (reference.Uri.Length > 0 && reference.Uri[0] != '#'))
			{
				this._refLevelCache[index] = 0;
				return 0;
			}
			if (reference.Uri.Length <= 0 || reference.Uri[0] != '#')
			{
				throw new CryptographicException("Malformed reference element.");
			}
			string text = Utils.ExtractIdFromLocalUri(reference.Uri);
			if (text == "xpointer(/)")
			{
				this._refLevelCache[index] = 0;
				return 0;
			}
			for (int i = 0; i < references.Count; i++)
			{
				if (((Reference)references[i]).Id == text)
				{
					this._refLevelCache[index] = this.GetReferenceLevel(i, references) + 1;
					return this._refLevelCache[index];
				}
			}
			this._refLevelCache[index] = 0;
			return 0;
		}

		private void BuildDigestedReferences()
		{
			ArrayList references = this.SignedInfo.References;
			this._refProcessed = new bool[references.Count];
			this._refLevelCache = new int[references.Count];
			SignedXml.ReferenceLevelSortOrder referenceLevelSortOrder = new SignedXml.ReferenceLevelSortOrder();
			referenceLevelSortOrder.References = references;
			ArrayList arrayList = new ArrayList();
			foreach (object obj in references)
			{
				Reference value = (Reference)obj;
				arrayList.Add(value);
			}
			arrayList.Sort(referenceLevelSortOrder);
			CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
			foreach (object obj2 in this.m_signature.ObjectList)
			{
				DataObject dataObject = (DataObject)obj2;
				canonicalXmlNodeList.Add(dataObject.GetXml());
			}
			foreach (object obj3 in arrayList)
			{
				Reference reference = (Reference)obj3;
				if (reference.DigestMethod == null)
				{
					reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
				}
				SignedXmlDebugLog.LogSigningReference(this, reference);
				reference.UpdateHashValue(this._containingDocument, canonicalXmlNodeList);
				if (reference.Id != null)
				{
					canonicalXmlNodeList.Add(reference.GetXml());
				}
			}
		}

		private bool CheckDigestedReferences()
		{
			ArrayList references = this.m_signature.SignedInfo.References;
			for (int i = 0; i < references.Count; i++)
			{
				Reference reference = (Reference)references[i];
				if (!this.ReferenceUsesSafeTransformMethods(reference))
				{
					return false;
				}
				SignedXmlDebugLog.LogVerifyReference(this, reference);
				byte[] array = null;
				try
				{
					array = reference.CalculateHashValue(this._containingDocument, this.m_signature.ReferencedItems);
				}
				catch (CryptoSignedXmlRecursionException)
				{
					SignedXmlDebugLog.LogSignedXmlRecursionLimit(this, reference);
					return false;
				}
				SignedXmlDebugLog.LogVerifyReferenceHash(this, reference, array, reference.DigestValue);
				if (!SignedXml.CryptographicEquals(array, reference.DigestValue))
				{
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static bool CryptographicEquals(byte[] a, byte[] b)
		{
			int num = 0;
			if (a.Length != b.Length)
			{
				return false;
			}
			int num2 = a.Length;
			for (int i = 0; i < num2; i++)
			{
				num |= (int)(a[i] - b[i]);
			}
			return num == 0;
		}

		private bool CheckSignatureFormat()
		{
			if (this._signatureFormatValidator == null)
			{
				return true;
			}
			SignedXmlDebugLog.LogBeginCheckSignatureFormat(this, this._signatureFormatValidator);
			bool result = this._signatureFormatValidator(this);
			SignedXmlDebugLog.LogFormatValidationResult(this, result);
			return result;
		}

		private bool CheckSignedInfo(AsymmetricAlgorithm key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			SignedXmlDebugLog.LogBeginCheckSignedInfo(this, this.m_signature.SignedInfo);
			SignatureDescription signatureDescription = CryptoHelpers.CreateFromName<SignatureDescription>(this.SignatureMethod);
			if (signatureDescription == null)
			{
				throw new CryptographicException("SignatureDescription could not be created for the signature algorithm supplied.");
			}
			Type type = Type.GetType(signatureDescription.KeyAlgorithm);
			if (!SignedXml.IsKeyTheCorrectAlgorithm(key, type))
			{
				return false;
			}
			HashAlgorithm hashAlgorithm = signatureDescription.CreateDigest();
			if (hashAlgorithm == null)
			{
				throw new CryptographicException("Could not create hash algorithm object.");
			}
			byte[] c14NDigest = this.GetC14NDigest(hashAlgorithm);
			AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = signatureDescription.CreateDeformatter(key);
			SignedXmlDebugLog.LogVerifySignedInfo(this, key, signatureDescription, hashAlgorithm, asymmetricSignatureDeformatter, c14NDigest, this.m_signature.SignatureValue);
			return asymmetricSignatureDeformatter.VerifySignature(c14NDigest, this.m_signature.SignatureValue);
		}

		private bool CheckSignedInfo(KeyedHashAlgorithm macAlg)
		{
			if (macAlg == null)
			{
				throw new ArgumentNullException("macAlg");
			}
			SignedXmlDebugLog.LogBeginCheckSignedInfo(this, this.m_signature.SignedInfo);
			int num;
			if (this.m_signature.SignedInfo.SignatureLength == null)
			{
				num = macAlg.HashSize;
			}
			else
			{
				num = Convert.ToInt32(this.m_signature.SignedInfo.SignatureLength, null);
			}
			if (num < 0 || num > macAlg.HashSize)
			{
				throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
			}
			if (num % 8 != 0)
			{
				throw new CryptographicException("The length in bits of the signature with a MAC should be a multiple of 8.");
			}
			if (this.m_signature.SignatureValue == null)
			{
				throw new CryptographicException("Signature requires a SignatureValue.");
			}
			if (this.m_signature.SignatureValue.Length != num / 8)
			{
				throw new CryptographicException("The length of the signature with a MAC should be less than the hash output length.");
			}
			byte[] c14NDigest = this.GetC14NDigest(macAlg);
			SignedXmlDebugLog.LogVerifySignedInfo(this, macAlg, c14NDigest, this.m_signature.SignatureValue);
			for (int i = 0; i < this.m_signature.SignatureValue.Length; i++)
			{
				if (this.m_signature.SignatureValue[i] != c14NDigest[i])
				{
					return false;
				}
			}
			return true;
		}

		private static XmlElement GetSingleReferenceTarget(XmlDocument document, string idAttributeName, string idValue)
		{
			string xpath = string.Concat(new string[]
			{
				"//*[@",
				idAttributeName,
				"=\"",
				idValue,
				"\"]"
			});
			XmlNodeList xmlNodeList = document.SelectNodes(xpath);
			if (xmlNodeList == null || xmlNodeList.Count == 0)
			{
				return null;
			}
			if (xmlNodeList.Count == 1)
			{
				return xmlNodeList[0] as XmlElement;
			}
			throw new CryptographicException("Malformed reference element.");
		}

		private static bool IsKeyTheCorrectAlgorithm(AsymmetricAlgorithm key, Type expectedType)
		{
			Type type = key.GetType();
			if (type == expectedType)
			{
				return true;
			}
			if (expectedType.IsSubclassOf(type))
			{
				return true;
			}
			while (expectedType != null && expectedType.BaseType != typeof(AsymmetricAlgorithm))
			{
				expectedType = expectedType.BaseType;
			}
			return !(expectedType == null) && type.IsSubclassOf(expectedType);
		}

		/// <summary>Represents the <see cref="T:System.Security.Cryptography.Xml.Signature" /> object of the current <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		protected Signature m_signature;

		/// <summary>Represents the name of the installed key to be used for signing the <see cref="T:System.Security.Cryptography.Xml.SignedXml" /> object.</summary>
		protected string m_strSigningKeyName;

		private AsymmetricAlgorithm _signingKey;

		private XmlDocument _containingDocument;

		private IEnumerator _keyInfoEnum;

		private X509Certificate2Collection _x509Collection;

		private IEnumerator _x509Enum;

		private bool[] _refProcessed;

		private int[] _refLevelCache;

		internal XmlResolver _xmlResolver;

		internal XmlElement _context;

		private bool _bResolverSet;

		private Func<SignedXml, bool> _signatureFormatValidator = new Func<SignedXml, bool>(SignedXml.DefaultSignatureFormatValidator);

		private Collection<string> _safeCanonicalizationMethods;

		private static IList<string> s_knownCanonicalizationMethods;

		private static IList<string> s_defaultSafeTransformMethods;

		private const string XmlDsigMoreHMACMD5Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-md5";

		private const string XmlDsigMoreHMACSHA256Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";

		private const string XmlDsigMoreHMACSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";

		private const string XmlDsigMoreHMACSHA512Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";

		private const string XmlDsigMoreHMACRIPEMD160Url = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";

		private EncryptedXml _exml;

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard namespace for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard minimal canonicalization algorithm for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard canonicalization algorithm for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigCanonicalizationUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard canonicalization algorithm for XML digital signatures and includes comments. This field is constant.</summary>
		public const string XmlDsigCanonicalizationWithCommentsUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA1" /> digest method for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.DSA" /> algorithm for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.RSA" /> signature method for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.HMACSHA1" /> algorithm for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA256" /> digest method for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the  <see cref="T:System.Security.Cryptography.RSA" /> SHA-256 signature method variation for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigRSASHA256Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA384" /> digest method for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigSHA384Url = "http://www.w3.org/2001/04/xmldsig-more#sha384";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the  <see cref="T:System.Security.Cryptography.RSA" /> SHA-384 signature method variation for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigRSASHA384Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the standard <see cref="T:System.Security.Cryptography.SHA512" /> digest method for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the  <see cref="T:System.Security.Cryptography.RSA" /> SHA-512 signature method variation for XML digital signatures. This field is constant.</summary>
		public const string XmlDsigRSASHA512Url = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the Canonical XML transformation. This field is constant.</summary>
		public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the Canonical XML transformation, with comments. This field is constant.</summary>
		public const string XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

		/// <summary>Represents the Uniform Resource Identifier (URI) for exclusive XML canonicalization. This field is constant.</summary>
		public const string XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";

		/// <summary>Represents the Uniform Resource Identifier (URI) for exclusive XML canonicalization, with comments. This field is constant.</summary>
		public const string XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the base 64 transformation. This field is constant.</summary>
		public const string XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the XML Path Language (XPath). This field is constant.</summary>
		public const string XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";

		/// <summary>Represents the Uniform Resource Identifier (URI) for XSLT transformations. This field is constant.</summary>
		public const string XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";

		/// <summary>Represents the Uniform Resource Identifier (URI) for enveloped signature transformation. This field is constant.</summary>
		public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the XML mode decryption transformation. This field is constant.</summary>
		public const string XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";

		/// <summary>Represents the Uniform Resource Identifier (URI) for the license transform algorithm used to normalize XrML licenses for signatures.</summary>
		public const string XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";

		private bool _bCacheValid;

		private byte[] _digestedSignedInfo;

		private class ReferenceLevelSortOrder : IComparer
		{
			public ArrayList References
			{
				get
				{
					return this._references;
				}
				set
				{
					this._references = value;
				}
			}

			public int Compare(object a, object b)
			{
				Reference reference = a as Reference;
				Reference reference2 = b as Reference;
				int index = 0;
				int index2 = 0;
				int num = 0;
				foreach (object obj in this.References)
				{
					Reference reference3 = (Reference)obj;
					if (reference3 == reference)
					{
						index = num;
					}
					if (reference3 == reference2)
					{
						index2 = num;
					}
					num++;
				}
				int referenceLevel = reference.SignedXml.GetReferenceLevel(index, this.References);
				int referenceLevel2 = reference2.SignedXml.GetReferenceLevel(index2, this.References);
				return referenceLevel.CompareTo(referenceLevel2);
			}

			private ArrayList _references;
		}
	}
}
