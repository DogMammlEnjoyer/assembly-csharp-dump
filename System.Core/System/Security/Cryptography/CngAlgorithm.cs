using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	/// <summary>Encapsulates the name of an encryption algorithm. </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class CngAlgorithm : IEquatable<CngAlgorithm>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CngAlgorithm" /> class.</summary>
		/// <param name="algorithm">The name of the algorithm to initialize.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="algorithm" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="algorithm" /> parameter length is 0 (zero).</exception>
		public CngAlgorithm(string algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			if (algorithm.Length == 0)
			{
				throw new ArgumentException(SR.GetString("The algorithm name '{0}' is invalid.", new object[]
				{
					algorithm
				}), "algorithm");
			}
			this.m_algorithm = algorithm;
		}

		/// <summary>Gets the algorithm name that the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object specifies.</summary>
		/// <returns>The embedded algorithm name.</returns>
		public string Algorithm
		{
			get
			{
				return this.m_algorithm;
			}
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngAlgorithm" /> objects specify the same algorithm name.</summary>
		/// <param name="left">An object that specifies an algorithm name.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects specify the same algorithm name; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(CngAlgorithm left, CngAlgorithm right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngAlgorithm" /> objects do not specify the same algorithm.</summary>
		/// <param name="left">An object that specifies an algorithm name.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects do not specify the same algorithm name; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(CngAlgorithm left, CngAlgorithm right)
		{
			if (left == null)
			{
				return right != null;
			}
			return !left.Equals(right);
		}

		/// <summary>Compares the specified object to the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object.</summary>
		/// <param name="obj">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="obj" /> parameter is a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> that specifies the same algorithm as the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngAlgorithm);
		}

		/// <summary>Compares the specified <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object to the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object. </summary>
		/// <param name="other">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="other" /> parameter specifies the same algorithm as the current object; otherwise, <see langword="false" />.</returns>
		public bool Equals(CngAlgorithm other)
		{
			return other != null && this.m_algorithm.Equals(other.Algorithm);
		}

		/// <summary>Generates a hash value for the algorithm name that is embedded in the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object.</summary>
		/// <returns>The hash value of the embedded algorithm name.</returns>
		public override int GetHashCode()
		{
			return this.m_algorithm.GetHashCode();
		}

		/// <summary>Gets the name of the algorithm that the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object specifies.</summary>
		/// <returns>The embedded algorithm name.</returns>
		public override string ToString()
		{
			return this.m_algorithm;
		}

		/// <summary>Gets a new <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies the RSA hash algorithm.</summary>
		/// <returns>An object that specifies the RSA algorithm.</returns>
		public static CngAlgorithm Rsa
		{
			get
			{
				if (CngAlgorithm.s_rsa == null)
				{
					CngAlgorithm.s_rsa = new CngAlgorithm("RSA");
				}
				return CngAlgorithm.s_rsa;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Diffie-Hellman (ECDH) key exchange algorithm whose curve is described via a key property.</summary>
		/// <returns>An object that specifies an ECDH key exchange algorithm whose curve is described via a key property.</returns>
		public static CngAlgorithm ECDiffieHellman
		{
			get
			{
				if (CngAlgorithm.s_ecdh == null)
				{
					CngAlgorithm.s_ecdh = new CngAlgorithm("ECDH");
				}
				return CngAlgorithm.s_ecdh;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Diffie-Hellman (ECDH) key exchange algorithm that uses the P-256 curve.</summary>
		/// <returns>An object that specifies an ECDH algorithm that uses the P-256 curve.</returns>
		public static CngAlgorithm ECDiffieHellmanP256
		{
			get
			{
				if (CngAlgorithm.s_ecdhp256 == null)
				{
					CngAlgorithm.s_ecdhp256 = new CngAlgorithm("ECDH_P256");
				}
				return CngAlgorithm.s_ecdhp256;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Diffie-Hellman (ECDH) key exchange algorithm that uses the P-384 curve.</summary>
		/// <returns>An object that specifies an ECDH algorithm that uses the P-384 curve.</returns>
		public static CngAlgorithm ECDiffieHellmanP384
		{
			get
			{
				if (CngAlgorithm.s_ecdhp384 == null)
				{
					CngAlgorithm.s_ecdhp384 = new CngAlgorithm("ECDH_P384");
				}
				return CngAlgorithm.s_ecdhp384;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Diffie-Hellman (ECDH) key exchange algorithm that uses the P-521 curve.</summary>
		/// <returns>An object that specifies an ECDH algorithm that uses the P-521 curve.</returns>
		public static CngAlgorithm ECDiffieHellmanP521
		{
			get
			{
				if (CngAlgorithm.s_ecdhp521 == null)
				{
					CngAlgorithm.s_ecdhp521 = new CngAlgorithm("ECDH_P521");
				}
				return CngAlgorithm.s_ecdhp521;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Digital Signature Algorithm (ECDSA) whose curve is described via a key property.</summary>
		/// <returns>An object that specifies an ECDSA whose curve is described via a key property.</returns>
		public static CngAlgorithm ECDsa
		{
			get
			{
				if (CngAlgorithm.s_ecdsa == null)
				{
					CngAlgorithm.s_ecdsa = new CngAlgorithm("ECDSA");
				}
				return CngAlgorithm.s_ecdsa;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Digital Signature Algorithm (ECDSA) that uses the P-256 curve.</summary>
		/// <returns>An object that specifies an ECDSA algorithm that uses the P-256 curve.</returns>
		public static CngAlgorithm ECDsaP256
		{
			get
			{
				if (CngAlgorithm.s_ecdsap256 == null)
				{
					CngAlgorithm.s_ecdsap256 = new CngAlgorithm("ECDSA_P256");
				}
				return CngAlgorithm.s_ecdsap256;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Digital Signature Algorithm (ECDSA) that uses the P-384 curve.</summary>
		/// <returns>An object that specifies an ECDSA algorithm that uses the P-384 curve.</returns>
		public static CngAlgorithm ECDsaP384
		{
			get
			{
				if (CngAlgorithm.s_ecdsap384 == null)
				{
					CngAlgorithm.s_ecdsap384 = new CngAlgorithm("ECDSA_P384");
				}
				return CngAlgorithm.s_ecdsap384;
			}
		}

		/// <summary>Gets a new <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies an Elliptic Curve Digital Signature Algorithm (ECDSA) that uses the P-521 curve.</summary>
		/// <returns>An object that specifies an ECDSA algorithm that uses the P-521 curve.</returns>
		public static CngAlgorithm ECDsaP521
		{
			get
			{
				if (CngAlgorithm.s_ecdsap521 == null)
				{
					CngAlgorithm.s_ecdsap521 = new CngAlgorithm("ECDSA_P521");
				}
				return CngAlgorithm.s_ecdsap521;
			}
		}

		/// <summary>Gets a new <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies the Message Digest 5 (MD5) hash algorithm.</summary>
		/// <returns>An object that specifies the MD5 algorithm.</returns>
		public static CngAlgorithm MD5
		{
			get
			{
				if (CngAlgorithm.s_md5 == null)
				{
					CngAlgorithm.s_md5 = new CngAlgorithm("MD5");
				}
				return CngAlgorithm.s_md5;
			}
		}

		/// <summary>Gets a new <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies the Secure Hash Algorithm 1 (SHA-1) algorithm.</summary>
		/// <returns>An object that specifies the SHA-1 algorithm.</returns>
		public static CngAlgorithm Sha1
		{
			get
			{
				if (CngAlgorithm.s_sha1 == null)
				{
					CngAlgorithm.s_sha1 = new CngAlgorithm("SHA1");
				}
				return CngAlgorithm.s_sha1;
			}
		}

		/// <summary>Gets a new <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies the Secure Hash Algorithm 256 (SHA-256) algorithm.</summary>
		/// <returns>An object that specifies the SHA-256 algorithm.</returns>
		public static CngAlgorithm Sha256
		{
			get
			{
				if (CngAlgorithm.s_sha256 == null)
				{
					CngAlgorithm.s_sha256 = new CngAlgorithm("SHA256");
				}
				return CngAlgorithm.s_sha256;
			}
		}

		/// <summary>Gets a new <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies the Secure Hash Algorithm 384 (SHA-384) algorithm.</summary>
		/// <returns>An object that specifies the SHA-384 algorithm.</returns>
		public static CngAlgorithm Sha384
		{
			get
			{
				if (CngAlgorithm.s_sha384 == null)
				{
					CngAlgorithm.s_sha384 = new CngAlgorithm("SHA384");
				}
				return CngAlgorithm.s_sha384;
			}
		}

		/// <summary>Gets a new <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object that specifies the Secure Hash Algorithm 512 (SHA-512) algorithm.</summary>
		/// <returns>An object that specifies the SHA-512 algorithm.</returns>
		public static CngAlgorithm Sha512
		{
			get
			{
				if (CngAlgorithm.s_sha512 == null)
				{
					CngAlgorithm.s_sha512 = new CngAlgorithm("SHA512");
				}
				return CngAlgorithm.s_sha512;
			}
		}

		private static volatile CngAlgorithm s_ecdh;

		private static volatile CngAlgorithm s_ecdhp256;

		private static volatile CngAlgorithm s_ecdhp384;

		private static volatile CngAlgorithm s_ecdhp521;

		private static volatile CngAlgorithm s_ecdsa;

		private static volatile CngAlgorithm s_ecdsap256;

		private static volatile CngAlgorithm s_ecdsap384;

		private static volatile CngAlgorithm s_ecdsap521;

		private static volatile CngAlgorithm s_md5;

		private static volatile CngAlgorithm s_sha1;

		private static volatile CngAlgorithm s_sha256;

		private static volatile CngAlgorithm s_sha384;

		private static volatile CngAlgorithm s_sha512;

		private static volatile CngAlgorithm s_rsa;

		private string m_algorithm;
	}
}
