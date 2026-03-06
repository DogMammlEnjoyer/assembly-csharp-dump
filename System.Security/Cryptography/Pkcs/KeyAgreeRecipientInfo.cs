using System;
using System.Threading;
using Internal.Cryptography;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	/// <summary>The <see cref="T:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo" /> class defines key agreement recipient information. Key agreement algorithms typically use the Diffie-Hellman key agreement algorithm, in which the two parties that establish a shared cryptographic key both take part in its generation and, by definition, agree on that key. This is in contrast to key transport algorithms, in which one party generates the key unilaterally and sends, or transports it, to the other party.</summary>
	public sealed class KeyAgreeRecipientInfo : RecipientInfo
	{
		internal KeyAgreeRecipientInfo(KeyAgreeRecipientInfoPal pal) : base(RecipientInfoType.KeyAgreement, pal)
		{
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo.Version" /> property retrieves the version of the key agreement recipient. This is automatically set for  objects in this class, and the value  implies that the recipient is taking part in a key agreement algorithm.</summary>
		/// <returns>The version of the <see cref="T:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo" /> object.</returns>
		public override int Version
		{
			get
			{
				return this.Pal.Version;
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo.RecipientIdentifier" /> property retrieves the identifier of the recipient.</summary>
		/// <returns>The identifier of the recipient.</returns>
		public override SubjectIdentifier RecipientIdentifier
		{
			get
			{
				SubjectIdentifier result;
				if ((result = this._lazyRecipientIdentifier) == null)
				{
					result = (this._lazyRecipientIdentifier = this.Pal.RecipientIdentifier);
				}
				return result;
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo.KeyEncryptionAlgorithm" /> property retrieves the algorithm used to perform the key agreement.</summary>
		/// <returns>The value of the algorithm used to perform the key agreement.</returns>
		public override AlgorithmIdentifier KeyEncryptionAlgorithm
		{
			get
			{
				AlgorithmIdentifier result;
				if ((result = this._lazyKeyEncryptionAlgorithm) == null)
				{
					result = (this._lazyKeyEncryptionAlgorithm = this.Pal.KeyEncryptionAlgorithm);
				}
				return result;
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo.EncryptedKey" /> property retrieves the encrypted recipient keying material.</summary>
		/// <returns>An array of byte values that contain the encrypted recipient keying material.</returns>
		public override byte[] EncryptedKey
		{
			get
			{
				byte[] result;
				if ((result = this._lazyEncryptedKey) == null)
				{
					result = (this._lazyEncryptedKey = this.Pal.EncryptedKey);
				}
				return result;
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo.OriginatorIdentifierOrKey" /> property retrieves information about the originator of the key agreement for key agreement algorithms that warrant it.</summary>
		/// <returns>An object that contains information about the originator of the key agreement.</returns>
		public SubjectIdentifierOrKey OriginatorIdentifierOrKey
		{
			get
			{
				SubjectIdentifierOrKey result;
				if ((result = this._lazyOriginatorIdentifierKey) == null)
				{
					result = (this._lazyOriginatorIdentifierKey = this.Pal.OriginatorIdentifierOrKey);
				}
				return result;
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo.Date" /> property retrieves the date and time of the start of the key agreement protocol by the originator.</summary>
		/// <returns>The date and time of the start of the key agreement protocol by the originator.</returns>
		/// <exception cref="T:System.InvalidOperationException">The recipient identifier type is not a subject key identifier.</exception>
		public DateTime Date
		{
			get
			{
				if (this._lazyDate == null)
				{
					this._lazyDate = new DateTime?(this.Pal.Date);
					Interlocked.MemoryBarrier();
				}
				return this._lazyDate.Value;
			}
		}

		/// <summary>The <see cref="P:System.Security.Cryptography.Pkcs.KeyAgreeRecipientInfo.OtherKeyAttribute" /> property retrieves attributes of the keying material.</summary>
		/// <returns>The attributes of the keying material.</returns>
		/// <exception cref="T:System.InvalidOperationException">The recipient identifier type is not a subject key identifier.</exception>
		public CryptographicAttributeObject OtherKeyAttribute
		{
			get
			{
				CryptographicAttributeObject result;
				if ((result = this._lazyOtherKeyAttribute) == null)
				{
					result = (this._lazyOtherKeyAttribute = this.Pal.OtherKeyAttribute);
				}
				return result;
			}
		}

		private new KeyAgreeRecipientInfoPal Pal
		{
			get
			{
				return (KeyAgreeRecipientInfoPal)base.Pal;
			}
		}

		internal KeyAgreeRecipientInfo()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private volatile SubjectIdentifier _lazyRecipientIdentifier;

		private volatile AlgorithmIdentifier _lazyKeyEncryptionAlgorithm;

		private volatile byte[] _lazyEncryptedKey;

		private volatile SubjectIdentifierOrKey _lazyOriginatorIdentifierKey;

		private DateTime? _lazyDate;

		private volatile CryptographicAttributeObject _lazyOtherKeyAttribute;
	}
}
