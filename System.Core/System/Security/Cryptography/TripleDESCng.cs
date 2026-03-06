using System;

namespace System.Security.Cryptography
{
	/// <summary>Provides a Cryptography Next Generation (CNG) implementation of the Triple Data Encryption Standard (3DES) algorithm.</summary>
	public sealed class TripleDESCng : TripleDES
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.TripleDESCng" /> class with an ephemeral key.</summary>
		public TripleDESCng()
		{
			throw new NotImplementedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.TripleDESCng" /> class with the specified key name, which represents an existing persisted 3DES key.</summary>
		/// <param name="keyName">The name of the key.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="keyName" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.PlatformNotSupportedException">
		///         Cryptography Next Generation (CNG) is not supported on this system.
		///  </exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">All other errors.
		/// </exception>
		public TripleDESCng(string keyName)
		{
			throw new NotImplementedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.TripleDESCng" /> class with the specified key name, which represents an existing persisted 3DES key, and the specified key storage provider (KSP).</summary>
		/// <param name="keyName">The name of the key.</param>
		/// <param name="provider">The KSP that contains the key.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="keyName" /> is <see langword="null" />. -or-
		///         <paramref name="provider" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.PlatformNotSupportedException">
		///         Cryptography Next Generation (CNG) is not supported on this system.
		///  </exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">All other errors.
		/// </exception>
		public TripleDESCng(string keyName, CngProvider provider)
		{
			throw new NotImplementedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.TripleDESCng" /> class with the specified key name, which represents an existing persisted 3DES key,  the specified key storage provider (KSP) and key open options.</summary>
		/// <param name="keyName">The name of the key.</param>
		/// <param name="provider">The KSP that contains the key.</param>
		/// <param name="openOptions">A bitwise combination of the enumeration values that specify options for opening the key, such as where the key is opened from (machine or user storage) and whether to suppress UI prompting.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="keyName" /> is <see langword="null" />. -or-
		///         <paramref name="provider" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.PlatformNotSupportedException">
		///         Cryptography Next Generation (CNG) is not supported on this system.
		///  </exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">All other errors.
		/// </exception>
		public TripleDESCng(string keyName, CngProvider provider, CngKeyOpenOptions openOptions)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets or sets the key for the <see cref="T:System.Security.Cryptography.TripleDESCng" /> algorithm.</summary>
		/// <returns>The key for the <see cref="T:System.Security.Cryptography.TripleDESCng" /> algorithm.</returns>
		public override byte[] Key
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the size, in bits, of the secret key used by the <see cref="T:System.Security.Cryptography.TripleDESCng" /> algorithm.</summary>
		/// <returns>The size, in bits, of the secret key used by the <see cref="T:System.Security.Cryptography.TripleDESCng" /> algorithm.</returns>
		public override int KeySize
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Creates a symmetric 3DES decryptor object with the current key and initialization vector (<see cref="P:System.Security.Cryptography.SymmetricAlgorithm.IV" />).</summary>
		/// <returns>A symmetric 3DES decryptor object.</returns>
		public override ICryptoTransform CreateDecryptor()
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a symmetric 3DES decryptor object with the specified key and initialization vector (IV).</summary>
		/// <param name="rgbKey">The secret key to use for the 3DES algorithm. The key size must be 192 bits.</param>
		/// <param name="rgbIV">The initialization vector to use for the 3DES algorithm.</param>
		/// <returns>A symmetric 3DES decryptor object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="rgbKey" />  is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="rgbKey" />  is not a valid size for this algorithm.-or-<paramref name="rgbIV" /> size does not match the block size for this algorithm.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///   <paramref name="rgbKey" /> is a known weak key for this algorithm and cannot be used.-or-
		///         <paramref name="rgbIV" />  is <see langword="null" />.</exception>
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a symmetric 3DES encryptor object using the current key and initialization vector (<see cref="P:System.Security.Cryptography.SymmetricAlgorithm.IV" />).</summary>
		/// <returns>A symmetric 3DES encryptor object.</returns>
		public override ICryptoTransform CreateEncryptor()
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a symmetric 3DES encryptor object with the specified key and initialization vector (IV).</summary>
		/// <param name="rgbKey">The secret key to use for the 3DES algorithm. The key size must be 192 bits.</param>
		/// <param name="rgbIV">The initialization vector to use for the 3DES algorithm.</param>
		/// <returns>A symmetric 3DES encryptor object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="rgbKey" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="rgbKey" /> is not a valid size for this algorithm.-or-<paramref name="rgbIV" /> size does not match the block size for this algorithm.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///   <paramref name="rgbKey" /> is a known weak key for this algorithm and cannot be used.-or-
		///         <paramref name="rgbIV" />  is <see langword="null" />.</exception>
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			return null;
		}

		protected override void Dispose(bool disposing)
		{
			throw new NotImplementedException();
		}

		/// <summary>Generates a random initialization vector (IV) to use for the 3DES algorithm.</summary>
		public override void GenerateIV()
		{
			throw new NotImplementedException();
		}

		/// <summary>Generates a random key to use for the 3DES algorithm.</summary>
		public override void GenerateKey()
		{
			throw new NotImplementedException();
		}
	}
}
