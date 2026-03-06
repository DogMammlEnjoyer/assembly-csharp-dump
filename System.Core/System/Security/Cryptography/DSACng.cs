using System;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	/// <summary>Provides a Cryptography Next Generation (CNG) implementation of the Digital Signature Algorithm (DSA).</summary>
	public sealed class DSACng : DSA
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.DSACng" /> class with a random 2,048-bit key pair. </summary>
		public DSACng()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.DSACng" /> class with a randomly generated key of the specified size. </summary>
		/// <param name="keySize">The size of the key to generate in bits. </param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <paramref name="keySize" /> is not valid. </exception>
		public DSACng(int keySize)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.DSACng" /> class with the specified key. </summary>
		/// <param name="key">The key to use for DSA operations. </param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="key" /> is not a valid DSA key. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="key" /> is <see langword="null" />. </exception>
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public DSACng(CngKey key)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets the key that will be used by the <see cref="T:System.Security.Cryptography.DSACng" /> object for any cryptographic operation that it performs. </summary>
		/// <returns>The key used by the <see cref="T:System.Security.Cryptography.DSACng" /> object to perform cryptographic operations. </returns>
		public CngKey Key
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Creates the digital signature for the specified data.</summary>
		/// <param name="rgbHash">The data to be signed.</param>
		/// <returns>The digital signature for the specified data.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="rgbHash" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.PlatformNotSupportedException">
		///         <paramref name="rgbHash" /> is shorter in length than the Q value of the DSA key . </exception>
		[SecuritySafeCritical]
		public override byte[] CreateSignature(byte[] rgbHash)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Exports the DSA algorithm parameters. </summary>
		/// <param name="includePrivateParameters">
		///       <see langword="true" /> to include private parameters; otherwise, <see langword="false" />. </param>
		/// <returns>The DSA algorithm parameters. </returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">DSA key is not a valid public or private key.</exception>
		public override DSAParameters ExportParameters(bool includePrivateParameters)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(DSAParameters);
		}

		/// <summary>Replaces the existing key that the current instance is working with by creating a new <see cref="T:System.Security.Cryptography.CngKey" /> for the parameters structure. </summary>
		/// <param name="parameters">The DSA parameters. </param>
		/// <exception cref="T:System.ArgumentException">The specified DSA parameters are not valid. </exception>
		public override void ImportParameters(DSAParameters parameters)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Verifies if the specified digital signature matches the specified data. </summary>
		/// <param name="rgbHash">The signed data.</param>
		/// <param name="rgbSignature">The digital signature to be verified.</param>
		/// <returns>
		///     <see langword="true" /> if <paramref name="rgbSignature" /> matches the signature computed using the specified data; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="rgbHash" /> parameter is <see langword="null" />.-or- The <paramref name="rgbSignature" /> parameter is <see langword="null" />. </exception>
		/// <exception cref="T:System.PlatformNotSupportedException">
		///         <paramref name="rgbHash" /> is shorter in length than the Q value of the DSA key . </exception>
		[SecuritySafeCritical]
		public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}
	}
}
