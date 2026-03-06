using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	/// <summary>Encapsulates the name of an encryption algorithm group. </summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class CngAlgorithmGroup : IEquatable<CngAlgorithmGroup>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> class.</summary>
		/// <param name="algorithmGroup">The name of the algorithm group to initialize.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="algorithmGroup" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="algorithmGroup" /> parameter length is 0 (zero).</exception>
		public CngAlgorithmGroup(string algorithmGroup)
		{
			if (algorithmGroup == null)
			{
				throw new ArgumentNullException("algorithmGroup");
			}
			if (algorithmGroup.Length == 0)
			{
				throw new ArgumentException(SR.GetString("The algorithm group '{0}' is invalid.", new object[]
				{
					algorithmGroup
				}), "algorithmGroup");
			}
			this.m_algorithmGroup = algorithmGroup;
		}

		/// <summary>Gets the name of the algorithm group that the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object specifies.</summary>
		/// <returns>The embedded algorithm group name.</returns>
		public string AlgorithmGroup
		{
			get
			{
				return this.m_algorithmGroup;
			}
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> objects specify the same algorithm group.</summary>
		/// <param name="left">An object that specifies an algorithm group.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects specify the same algorithm group; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(CngAlgorithmGroup left, CngAlgorithmGroup right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> objects do not specify the same algorithm group.</summary>
		/// <param name="left">An object that specifies an algorithm group.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects do not specify the same algorithm group; otherwise, <see langword="false" />. </returns>
		public static bool operator !=(CngAlgorithmGroup left, CngAlgorithmGroup right)
		{
			if (left == null)
			{
				return right != null;
			}
			return !left.Equals(right);
		}

		/// <summary>Compares the specified object to the current <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object.</summary>
		/// <param name="obj">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="obj" /> parameter is a <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> that specifies the same algorithm group as the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngAlgorithmGroup);
		}

		/// <summary>Compares the specified <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object to the current <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object.</summary>
		/// <param name="other">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="other" /> parameter specifies the same algorithm group as the current object; otherwise, <see langword="false" />.</returns>
		public bool Equals(CngAlgorithmGroup other)
		{
			return other != null && this.m_algorithmGroup.Equals(other.AlgorithmGroup);
		}

		/// <summary>Generates a hash value for the algorithm group name that is embedded in the current <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object.</summary>
		/// <returns>The hash value of the embedded algorithm group name.</returns>
		public override int GetHashCode()
		{
			return this.m_algorithmGroup.GetHashCode();
		}

		/// <summary>Gets the name of the algorithm group that the current <see cref="T:System.Security.Cryptography.CngAlgorithm" /> object specifies.</summary>
		/// <returns>The embedded algorithm group name.</returns>
		public override string ToString()
		{
			return this.m_algorithmGroup;
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object that specifies the Diffie-Hellman family of algorithms.</summary>
		/// <returns>An object that specifies the Diffie-Hellman family of algorithms.</returns>
		public static CngAlgorithmGroup DiffieHellman
		{
			get
			{
				if (CngAlgorithmGroup.s_dh == null)
				{
					CngAlgorithmGroup.s_dh = new CngAlgorithmGroup("DH");
				}
				return CngAlgorithmGroup.s_dh;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object that specifies the Digital Signature Algorithm (DSA) family of algorithms.</summary>
		/// <returns>An object that specifies the DSA family of algorithms.</returns>
		public static CngAlgorithmGroup Dsa
		{
			get
			{
				if (CngAlgorithmGroup.s_dsa == null)
				{
					CngAlgorithmGroup.s_dsa = new CngAlgorithmGroup("DSA");
				}
				return CngAlgorithmGroup.s_dsa;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object that specifies the Elliptic Curve Diffie-Hellman (ECDH) family of algorithms.</summary>
		/// <returns>An object that specifies the ECDH family of algorithms.</returns>
		public static CngAlgorithmGroup ECDiffieHellman
		{
			get
			{
				if (CngAlgorithmGroup.s_ecdh == null)
				{
					CngAlgorithmGroup.s_ecdh = new CngAlgorithmGroup("ECDH");
				}
				return CngAlgorithmGroup.s_ecdh;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object that specifies the Elliptic Curve Digital Signature Algorithm (ECDSA) family of algorithms.</summary>
		/// <returns>An object that specifies the ECDSA family of algorithms.</returns>
		public static CngAlgorithmGroup ECDsa
		{
			get
			{
				if (CngAlgorithmGroup.s_ecdsa == null)
				{
					CngAlgorithmGroup.s_ecdsa = new CngAlgorithmGroup("ECDSA");
				}
				return CngAlgorithmGroup.s_ecdsa;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngAlgorithmGroup" /> object that specifies the Rivest-Shamir-Adleman (RSA) family of algorithms.</summary>
		/// <returns>An object that specifies the RSA family of algorithms.</returns>
		public static CngAlgorithmGroup Rsa
		{
			get
			{
				if (CngAlgorithmGroup.s_rsa == null)
				{
					CngAlgorithmGroup.s_rsa = new CngAlgorithmGroup("RSA");
				}
				return CngAlgorithmGroup.s_rsa;
			}
		}

		private static volatile CngAlgorithmGroup s_dh;

		private static volatile CngAlgorithmGroup s_dsa;

		private static volatile CngAlgorithmGroup s_ecdh;

		private static volatile CngAlgorithmGroup s_ecdsa;

		private static volatile CngAlgorithmGroup s_rsa;

		private string m_algorithmGroup;
	}
}
