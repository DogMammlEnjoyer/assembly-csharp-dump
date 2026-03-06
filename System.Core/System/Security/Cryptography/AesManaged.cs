using System;

namespace System.Security.Cryptography
{
	/// <summary>Provides a managed implementation of the Advanced Encryption Standard (AES) symmetric algorithm. </summary>
	public sealed class AesManaged : Aes
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.AesManaged" /> class. </summary>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The Windows security policy setting for FIPS is enabled.</exception>
		/// <exception cref="T:System.InvalidOperationException">This implementation is not part of the Windows Platform FIPS-validated cryptographic algorithms.</exception>
		public AesManaged()
		{
			if (CryptoConfig.AllowOnlyFipsAlgorithms)
			{
				throw new InvalidOperationException(SR.GetString("This implementation is not part of the Windows Platform FIPS validated cryptographic algorithms."));
			}
			this.m_rijndael = new RijndaelManaged();
			this.m_rijndael.BlockSize = this.BlockSize;
			this.m_rijndael.KeySize = this.KeySize;
		}

		/// <summary>Gets or sets the number of bits to use as feedback. </summary>
		/// <returns>The feedback size, in bits.</returns>
		public override int FeedbackSize
		{
			get
			{
				return this.m_rijndael.FeedbackSize;
			}
			set
			{
				this.m_rijndael.FeedbackSize = value;
			}
		}

		/// <summary>Gets or sets the initialization vector (IV) to use for the symmetric algorithm. </summary>
		/// <returns>The initialization vector to use for the symmetric algorithm</returns>
		public override byte[] IV
		{
			get
			{
				return this.m_rijndael.IV;
			}
			set
			{
				this.m_rijndael.IV = value;
			}
		}

		/// <summary>Gets or sets the secret key used for the symmetric algorithm.</summary>
		/// <returns>The key for the symmetric algorithm.</returns>
		public override byte[] Key
		{
			get
			{
				return this.m_rijndael.Key;
			}
			set
			{
				this.m_rijndael.Key = value;
			}
		}

		/// <summary>Gets or sets the size, in bits, of the secret key used for the symmetric algorithm. </summary>
		/// <returns>The size, in bits, of the key used by the symmetric algorithm.</returns>
		public override int KeySize
		{
			get
			{
				return this.m_rijndael.KeySize;
			}
			set
			{
				this.m_rijndael.KeySize = value;
			}
		}

		/// <summary>Gets or sets the mode for operation of the symmetric algorithm.</summary>
		/// <returns>One of the enumeration values that specifies the block cipher mode to use for encryption. The default is <see cref="F:System.Security.Cryptography.CipherMode.CBC" />.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <see cref="P:System.Security.Cryptography.AesManaged.Mode" /> is set to <see cref="F:System.Security.Cryptography.CipherMode.CFB" /> or <see cref="F:System.Security.Cryptography.CipherMode.OFB" />.</exception>
		public override CipherMode Mode
		{
			get
			{
				return this.m_rijndael.Mode;
			}
			set
			{
				if (value == CipherMode.CFB || value == CipherMode.OFB)
				{
					throw new CryptographicException(SR.GetString("Specified cipher mode is not valid for this algorithm."));
				}
				this.m_rijndael.Mode = value;
			}
		}

		/// <summary>Gets or sets the padding mode used in the symmetric algorithm. </summary>
		/// <returns>One of the enumeration values that specifies the type of padding to apply. The default is <see cref="F:System.Security.Cryptography.PaddingMode.PKCS7" />.</returns>
		public override PaddingMode Padding
		{
			get
			{
				return this.m_rijndael.Padding;
			}
			set
			{
				this.m_rijndael.Padding = value;
			}
		}

		/// <summary>Creates a symmetric decryptor object using the current key and initialization vector (IV).</summary>
		/// <returns>A symmetric decryptor object.</returns>
		public override ICryptoTransform CreateDecryptor()
		{
			return this.m_rijndael.CreateDecryptor();
		}

		/// <summary>Creates a symmetric decryptor object using the specified key and initialization vector (IV).</summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>A symmetric decryptor object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="key" /> or <paramref name="iv" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="key" /> is invalid.</exception>
		public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!base.ValidKeySize(key.Length * 8))
			{
				throw new ArgumentException(SR.GetString("Specified key is not a valid size for this algorithm."), "key");
			}
			if (iv != null && iv.Length * 8 != this.BlockSizeValue)
			{
				throw new ArgumentException(SR.GetString("Specified initialization vector (IV) does not match the block size for this algorithm."), "iv");
			}
			return this.m_rijndael.CreateDecryptor(key, iv);
		}

		/// <summary>Creates a symmetric encryptor object using the current key and initialization vector (IV).</summary>
		/// <returns>A symmetric encryptor object.</returns>
		public override ICryptoTransform CreateEncryptor()
		{
			return this.m_rijndael.CreateEncryptor();
		}

		/// <summary>Creates a symmetric encryptor object using the specified key and initialization vector (IV).</summary>
		/// <param name="key">The secret key to use for the symmetric algorithm.</param>
		/// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
		/// <returns>A symmetric encryptor object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="key" /> or <paramref name="iv" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="key" /> is invalid.</exception>
		public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (!base.ValidKeySize(key.Length * 8))
			{
				throw new ArgumentException(SR.GetString("Specified key is not a valid size for this algorithm."), "key");
			}
			if (iv != null && iv.Length * 8 != this.BlockSizeValue)
			{
				throw new ArgumentException(SR.GetString("Specified initialization vector (IV) does not match the block size for this algorithm."), "iv");
			}
			return this.m_rijndael.CreateEncryptor(key, iv);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					((IDisposable)this.m_rijndael).Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>Generates a random initialization vector (IV) to use for the symmetric algorithm.</summary>
		public override void GenerateIV()
		{
			this.m_rijndael.GenerateIV();
		}

		/// <summary>Generates a random key to use for the symmetric algorithm. </summary>
		public override void GenerateKey()
		{
			this.m_rijndael.GenerateKey();
		}

		private RijndaelManaged m_rijndael;
	}
}
