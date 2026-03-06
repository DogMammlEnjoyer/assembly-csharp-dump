using System;

namespace System.Security.Cryptography
{
	/// <summary>Provides a Cryptography Next Generation (CNG) implementation of the RSA algorithm. </summary>
	public sealed class RSACng : RSA
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.RSACng" /> class with a random 2,048-bit key pair. </summary>
		public RSACng() : this(2048)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.RSACng" /> class with a randomly generated key of the specified size. </summary>
		/// <param name="keySize">The size of the key to generate in bits. </param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <paramref name="keySize" /> is not valid. </exception>
		public RSACng(int keySize)
		{
			throw new NotImplementedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.RSACng" /> class with the specified key. </summary>
		/// <param name="key">The key to use for RSA operations. </param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="key" /> is not a valid RSA key. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="key" /> is <see langword="null" />. </exception>
		public RSACng(CngKey key)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets the key that will be used by the <see cref="T:System.Security.Cryptography.RSACng" /> object for any cryptographic operation that it performs. </summary>
		/// <returns>The key used by the <see cref="T:System.Security.Cryptography.RSACng" /> object. </returns>
		public CngKey Key
		{
			[SecuritySafeCritical]
			get
			{
				throw new NotImplementedException();
			}
			private set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Exports the key used by the RSA object into a <see cref="T:System.Security.Cryptography.RSAParameters" /> object. </summary>
		/// <param name="includePrivateParameters">
		///       <see langword="true" /> to include private parameters; otherwise, <see langword="false" />. </param>
		/// <returns>The key used by the RSA object. </returns>
		public override RSAParameters ExportParameters(bool includePrivateParameters)
		{
			throw new NotImplementedException();
		}

		/// <summary>Replaces the existing key that the current instance is working with by creating a new <see cref="T:System.Security.Cryptography.CngKey" /> for the parameters structure. </summary>
		/// <param name="parameters">The RSA parameters. </param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="parameters" /> contains neither an exponent nor a modulus. </exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <paramref name="parameters" /> is not a valid RSA key. -or-
		///         <paramref name="parameters" /> is a full key pair and the default KSP is used. </exception>
		public override void ImportParameters(RSAParameters parameters)
		{
			throw new NotImplementedException();
		}
	}
}
