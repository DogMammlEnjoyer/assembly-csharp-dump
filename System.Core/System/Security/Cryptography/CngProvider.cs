using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	/// <summary>Encapsulates the name of a key storage provider (KSP) for use with Cryptography Next Generation (CNG) objects.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class CngProvider : IEquatable<CngProvider>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CngProvider" /> class.</summary>
		/// <param name="provider">The name of the key storage provider (KSP) to initialize.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="provider" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="provider" /> parameter length is 0 (zero).</exception>
		public CngProvider(string provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (provider.Length == 0)
			{
				throw new ArgumentException(SR.GetString("The provider name '{0}' is invalid.", new object[]
				{
					provider
				}), "provider");
			}
			this.m_provider = provider;
		}

		/// <summary>Gets the name of the key storage provider (KSP) that the current <see cref="T:System.Security.Cryptography.CngProvider" /> object specifies.</summary>
		/// <returns>The embedded KSP name.</returns>
		public string Provider
		{
			get
			{
				return this.m_provider;
			}
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngProvider" /> objects specify the same key storage provider (KSP).</summary>
		/// <param name="left">An object that specifies a KSP.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects represent the same KSP; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(CngProvider left, CngProvider right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngProvider" /> objects do not represent the same key storage provider (KSP).</summary>
		/// <param name="left">An object that specifies a KSP.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects do not represent the same KSP; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(CngProvider left, CngProvider right)
		{
			if (left == null)
			{
				return right != null;
			}
			return !left.Equals(right);
		}

		/// <summary>Compares the specified object to the current <see cref="T:System.Security.Cryptography.CngProvider" /> object.</summary>
		/// <param name="obj">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngProvider" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="obj" /> parameter is a <see cref="T:System.Security.Cryptography.CngProvider" /> that specifies the same key storage provider(KSP) as the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngProvider);
		}

		/// <summary>Compares the specified <see cref="T:System.Security.Cryptography.CngProvider" /> object to the current <see cref="T:System.Security.Cryptography.CngProvider" /> object.</summary>
		/// <param name="other">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngProvider" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="other" /> parameter specifies the same key storage provider (KSP) as the current object; otherwise, <see langword="false" />.</returns>
		public bool Equals(CngProvider other)
		{
			return other != null && this.m_provider.Equals(other.Provider);
		}

		/// <summary>Generates a hash value for the name of the key storage provider (KSP) that is embedded in the current <see cref="T:System.Security.Cryptography.CngProvider" /> object.</summary>
		/// <returns>The hash value of the embedded KSP name.</returns>
		public override int GetHashCode()
		{
			return this.m_provider.GetHashCode();
		}

		/// <summary>Gets the name of the key storage provider (KSP) that the current <see cref="T:System.Security.Cryptography.CngProvider" /> object specifies.</summary>
		/// <returns>The embedded KSP name.</returns>
		public override string ToString()
		{
			return this.m_provider.ToString();
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngProvider" /> object that specifies the Microsoft Smart Card Key Storage Provider.</summary>
		/// <returns>An object that specifies the Microsoft Smart Card Key Storage Provider.</returns>
		public static CngProvider MicrosoftSmartCardKeyStorageProvider
		{
			get
			{
				if (CngProvider.s_msSmartCardKsp == null)
				{
					CngProvider.s_msSmartCardKsp = new CngProvider("Microsoft Smart Card Key Storage Provider");
				}
				return CngProvider.s_msSmartCardKsp;
			}
		}

		/// <summary>Gets a <see cref="T:System.Security.Cryptography.CngProvider" /> object that specifies the Microsoft Software Key Storage Provider.</summary>
		/// <returns>An object that specifies the Microsoft Software Key Storage Provider.</returns>
		public static CngProvider MicrosoftSoftwareKeyStorageProvider
		{
			get
			{
				if (CngProvider.s_msSoftwareKsp == null)
				{
					CngProvider.s_msSoftwareKsp = new CngProvider("Microsoft Software Key Storage Provider");
				}
				return CngProvider.s_msSoftwareKsp;
			}
		}

		private static volatile CngProvider s_msSmartCardKsp;

		private static volatile CngProvider s_msSoftwareKsp;

		private string m_provider;
	}
}
