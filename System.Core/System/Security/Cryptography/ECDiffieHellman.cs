using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	/// <summary>Provides an abstract base class that Elliptic Curve Diffie-Hellman (ECDH) algorithm implementations can derive from. This class provides the basic set of operations that all ECDH implementations must support.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public abstract class ECDiffieHellman : AsymmetricAlgorithm
	{
		/// <summary>Gets the name of the key exchange algorithm.</summary>
		/// <returns>The name of the key exchange algorithm. </returns>
		public override string KeyExchangeAlgorithm
		{
			get
			{
				return "ECDiffieHellman";
			}
		}

		/// <summary>Gets the name of the signature algorithm.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		public override string SignatureAlgorithm
		{
			get
			{
				return null;
			}
		}

		/// <summary>Creates a new instance of the default implementation of the Elliptic Curve Diffie-Hellman (ECDH) algorithm.</summary>
		/// <returns>A new instance of the default implementation of this class.</returns>
		public new static ECDiffieHellman Create()
		{
			throw new NotImplementedException();
		}

		/// <summary>Creates a new instance of the specified implementation of the Elliptic Curve Diffie-Hellman (ECDH) algorithm.</summary>
		/// <param name="algorithm">The name of an implementation of the ECDH algorithm.</param>
		/// <returns>A new instance of the specified implementation of this class. If the specified algorithm name does not map to an ECDH implementation, this method returns <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="algorithm" /> parameter is <see langword="null" />. </exception>
		public new static ECDiffieHellman Create(string algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			return CryptoConfig.CreateFromName(algorithm) as ECDiffieHellman;
		}

		/// <summary>Creates a new instance of the default implementation of the Elliptic Curve Diffie-Hellman (ECDH) algorithm with a new public/private key-pair generated over the specified curve. </summary>
		/// <param name="curve">The curve to use to generate a new public/private key-pair. </param>
		/// <returns>A new instance of the default implementation of the Elliptic Curve Diffie-Hellman (ECDH) algorithm. </returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <paramref name="curve" /> does not validate. </exception>
		public static ECDiffieHellman Create(ECCurve curve)
		{
			ECDiffieHellman ecdiffieHellman = ECDiffieHellman.Create();
			if (ecdiffieHellman != null)
			{
				try
				{
					ecdiffieHellman.GenerateKey(curve);
				}
				catch
				{
					ecdiffieHellman.Dispose();
					throw;
				}
			}
			return ecdiffieHellman;
		}

		/// <summary>Creates a new instance of the default implementation of the Elliptic Curve Diffie-Hellman (ECDH) algorithm with the key described by the specified  <see cref="T:System.Security.Cryptography.ECParameters" /> object. </summary>
		/// <param name="parameters">The parameters  for the elliptic curve cryptography (ECC) algorithm. </param>
		/// <returns>A new instance of the default implementation of the Elliptic Curve Diffie-Hellman (ECDH) algorithm. </returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <paramref name="parameters" /> does not validate. </exception>
		public static ECDiffieHellman Create(ECParameters parameters)
		{
			ECDiffieHellman ecdiffieHellman = ECDiffieHellman.Create();
			if (ecdiffieHellman != null)
			{
				try
				{
					ecdiffieHellman.ImportParameters(parameters);
				}
				catch
				{
					ecdiffieHellman.Dispose();
					throw;
				}
			}
			return ecdiffieHellman;
		}

		/// <summary>Gets the public key that is being used by the current Elliptic Curve Diffie-Hellman (ECDH) instance.</summary>
		/// <returns>The public part of the ECDH key pair that is being used by this <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> instance.</returns>
		public abstract ECDiffieHellmanPublicKey PublicKey { get; }

		/// <summary>Derives bytes that can be used as a key, given another party's public key.</summary>
		/// <param name="otherPartyPublicKey">The other party's public key.</param>
		/// <returns>The key material from the key exchange with the other party’s public key.</returns>
		public virtual byte[] DeriveKeyMaterial(ECDiffieHellmanPublicKey otherPartyPublicKey)
		{
			throw ECDiffieHellman.DerivedClassMustOverride();
		}

		/// <summary>Derives bytes that can be used as a key using a hash function, given another party's public key and hash algorithm's name.</summary>
		/// <param name="otherPartyPublicKey">The other party's public key.</param>
		/// <param name="hashAlgorithm">The hash algorithm  to use to derive the key material.</param>
		/// <returns>The key material from the key exchange with the other party’s public key.</returns>
		public byte[] DeriveKeyFromHash(ECDiffieHellmanPublicKey otherPartyPublicKey, HashAlgorithmName hashAlgorithm)
		{
			return this.DeriveKeyFromHash(otherPartyPublicKey, hashAlgorithm, null, null);
		}

		/// <summary>When implemented in a derived class, derives bytes that can be used as a key using a hash function, given another party's public key, hash algorithm's name, a prepend value and an append value.</summary>
		/// <param name="otherPartyPublicKey">The other party's public key.</param>
		/// <param name="hashAlgorithm">The hash algorithm  to use to derive the key material.</param>
		/// <param name="secretPrepend">A value to prepend to the derived secret before hashing.</param>
		/// <param name="secretAppend">A value to append to the derived secret before hashing.</param>
		/// <returns>The key material from the key exchange with the other party’s public key.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		public virtual byte[] DeriveKeyFromHash(ECDiffieHellmanPublicKey otherPartyPublicKey, HashAlgorithmName hashAlgorithm, byte[] secretPrepend, byte[] secretAppend)
		{
			throw ECDiffieHellman.DerivedClassMustOverride();
		}

		/// <summary>Derives bytes that can be used as a key using a Hash-based Message Authentication Code (HMAC).</summary>
		/// <param name="otherPartyPublicKey">The other party's public key.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to derive the key material.</param>
		/// <param name="hmacKey">The key for the HMAC.</param>
		/// <returns>The key material from the key exchange with the other party’s public key.</returns>
		public byte[] DeriveKeyFromHmac(ECDiffieHellmanPublicKey otherPartyPublicKey, HashAlgorithmName hashAlgorithm, byte[] hmacKey)
		{
			return this.DeriveKeyFromHmac(otherPartyPublicKey, hashAlgorithm, hmacKey, null, null);
		}

		/// <summary>When implemented in a derived class, derives bytes that can be used as a key using a Hash-based Message Authentication Code (HMAC).</summary>
		/// <param name="otherPartyPublicKey">The other party's public key.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to derive the key material.</param>
		/// <param name="hmacKey">The key for the HMAC.</param>
		/// <param name="secretPrepend">A value to prepend to the derived secret before hashing.</param>
		/// <param name="secretAppend">A value to append to the derived secret before hashing.</param>
		/// <returns>The key material from the key exchange with the other party’s public key.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		public virtual byte[] DeriveKeyFromHmac(ECDiffieHellmanPublicKey otherPartyPublicKey, HashAlgorithmName hashAlgorithm, byte[] hmacKey, byte[] secretPrepend, byte[] secretAppend)
		{
			throw ECDiffieHellman.DerivedClassMustOverride();
		}

		/// <summary>When implemented in a derived class, derives bytes that can be used as a key using a Transport Layer Security (TLS) Pseudo-Random Function (PRF) derivation algorithm.</summary>
		/// <param name="otherPartyPublicKey">The other party's public key.</param>
		/// <param name="prfLabel">The ASCII-encoded PRF label.</param>
		/// <param name="prfSeed">The 64-byte PRF seed.</param>
		/// <returns>The key material from the key exchange with the other party’s public key.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		public virtual byte[] DeriveKeyTls(ECDiffieHellmanPublicKey otherPartyPublicKey, byte[] prfLabel, byte[] prfSeed)
		{
			throw ECDiffieHellman.DerivedClassMustOverride();
		}

		private static Exception DerivedClassMustOverride()
		{
			return new NotImplementedException(SR.GetString("Method not supported. Derived class must override."));
		}

		/// <summary>When overridden in a derived class, exports either the public or the public and private key information from a working <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> key to an <see cref="T:System.Security.Cryptography.ECParameters" /> structure so that it can be passed to the <see cref="M:System.Security.Cryptography.ECDiffieHellman.ImportParameters(System.Security.Cryptography.ECParameters)" />   method. </summary>
		/// <param name="includePrivateParameters">
		///       <see langword="true" /> to include private parameters; otehrwise,  <see langword="false" /> to include public parameters only.</param>
		/// <returns>An object that represents the point on the curve for this key. It can be passed to the <see cref="M:System.Security.Cryptography.ECDiffieHellman.ImportParameters(System.Security.Cryptography.ECParameters)" /> method. </returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method. </exception>
		public virtual ECParameters ExportParameters(bool includePrivateParameters)
		{
			throw ECDiffieHellman.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, exports either the public or the public and private key information using the explicit curve form from a working <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> key to an <see cref="T:System.Security.Cryptography.ECParameters" /> structure so that it can be passed to the <see cref="M:System.Security.Cryptography.ECDiffieHellman.ImportParameters(System.Security.Cryptography.ECParameters)" />   method. </summary>
		/// <param name="includePrivateParameters">
		///       <see langword="true" /> to include private parameters; otherwise, <see langword="false" />. </param>
		/// <returns>An object that represents the point on the curve for this key, using the explicit curve format. </returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method. </exception>
		public virtual ECParameters ExportExplicitParameters(bool includePrivateParameters)
		{
			throw ECDiffieHellman.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, imports the specified parameters for an <see cref="T:System.Security.Cryptography.ECCurve" /> as an ephemeral key into the current <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> object. </summary>
		/// <param name="parameters">The curve's parameters to import. </param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <paramref name="parameters" /> does not validate. </exception>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method. </exception>
		public virtual void ImportParameters(ECParameters parameters)
		{
			throw ECDiffieHellman.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, generates a new ephemeral public/private key pair for the specified curve. </summary>
		/// <param name="curve">The curve used to generate an ephemeral public/private key pair. </param>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///         <paramref name="curve" /> does not validate. </exception>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method. </exception>
		public virtual void GenerateKey(ECCurve curve)
		{
			throw new NotSupportedException(SR.GetString("Method not supported. Derived class must override."));
		}

		public virtual byte[] ExportECPrivateKey()
		{
			throw new PlatformNotSupportedException();
		}

		public virtual bool TryExportECPrivateKey(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual void ImportECPrivateKey(ReadOnlySpan<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
