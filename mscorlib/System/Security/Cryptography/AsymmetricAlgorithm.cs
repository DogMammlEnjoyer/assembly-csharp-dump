using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	/// <summary>Represents the abstract base class from which all implementations of asymmetric algorithms must inherit.</summary>
	[ComVisible(true)]
	public abstract class AsymmetricAlgorithm : IDisposable
	{
		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> class.</summary>
		public void Dispose()
		{
			this.Clear();
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> class.</summary>
		public void Clear()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> class and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Gets or sets the size, in bits, of the key modulus used by the asymmetric algorithm.</summary>
		/// <returns>The size, in bits, of the key modulus used by the asymmetric algorithm.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The key modulus size is invalid.</exception>
		public virtual int KeySize
		{
			get
			{
				return this.KeySizeValue;
			}
			set
			{
				for (int i = 0; i < this.LegalKeySizesValue.Length; i++)
				{
					if (this.LegalKeySizesValue[i].SkipSize == 0)
					{
						if (this.LegalKeySizesValue[i].MinSize == value)
						{
							this.KeySizeValue = value;
							return;
						}
					}
					else
					{
						for (int j = this.LegalKeySizesValue[i].MinSize; j <= this.LegalKeySizesValue[i].MaxSize; j += this.LegalKeySizesValue[i].SkipSize)
						{
							if (j == value)
							{
								this.KeySizeValue = value;
								return;
							}
						}
					}
				}
				throw new CryptographicException(Environment.GetResourceString("Specified key is not a valid size for this algorithm."));
			}
		}

		/// <summary>Gets the key sizes that are supported by the asymmetric algorithm.</summary>
		/// <returns>An array that contains the key sizes supported by the asymmetric algorithm.</returns>
		public virtual KeySizes[] LegalKeySizes
		{
			get
			{
				return (KeySizes[])this.LegalKeySizesValue.Clone();
			}
		}

		/// <summary>When implemented in a derived class, gets the name of the signature algorithm. Otherwise, always throws a <see cref="T:System.NotImplementedException" />.</summary>
		/// <returns>The name of the signature algorithm.</returns>
		public virtual string SignatureAlgorithm
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>When overridden in a derived class, gets the name of the key exchange algorithm. Otherwise, throws an <see cref="T:System.NotImplementedException" />.</summary>
		/// <returns>The name of the key exchange algorithm.</returns>
		public virtual string KeyExchangeAlgorithm
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Creates a default cryptographic object used to perform the asymmetric algorithm.</summary>
		/// <returns>A new <see cref="T:System.Security.Cryptography.RSACryptoServiceProvider" /> instance, unless the default settings have been changed with the &lt;cryptoClass&gt; element.</returns>
		public static AsymmetricAlgorithm Create()
		{
			return AsymmetricAlgorithm.Create("System.Security.Cryptography.AsymmetricAlgorithm");
		}

		/// <summary>Creates an instance of the specified implementation of an asymmetric algorithm.</summary>
		/// <param name="algName">The asymmetric algorithm implementation to use. The following table shows the valid values for the <paramref name="algName" /> parameter and the algorithms they map to.  
		///   Parameter value  
		///
		///   Implements  
		///
		///   System.Security.Cryptography.AsymmetricAlgorithm  
		///
		///  <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> RSA  
		///
		///  <see cref="T:System.Security.Cryptography.RSA" /> System.Security.Cryptography.RSA  
		///
		///  <see cref="T:System.Security.Cryptography.RSA" /> DSA  
		///
		///  <see cref="T:System.Security.Cryptography.DSA" /> System.Security.Cryptography.DSA  
		///
		///  <see cref="T:System.Security.Cryptography.DSA" /> ECDsa  
		///
		///  <see cref="T:System.Security.Cryptography.ECDsa" /> ECDsaCng  
		///
		///  <see cref="T:System.Security.Cryptography.ECDsaCng" /> System.Security.Cryptography.ECDsaCng  
		///
		///  <see cref="T:System.Security.Cryptography.ECDsaCng" /> ECDH  
		///
		///  <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> ECDiffieHellman  
		///
		///  <see cref="T:System.Security.Cryptography.ECDiffieHellman" /> ECDiffieHellmanCng  
		///
		///  <see cref="T:System.Security.Cryptography.ECDiffieHellmanCng" /> System.Security.Cryptography.ECDiffieHellmanCng  
		///
		///  <see cref="T:System.Security.Cryptography.ECDiffieHellmanCng" /></param>
		/// <returns>A new instance of the specified asymmetric algorithm implementation.</returns>
		public static AsymmetricAlgorithm Create(string algName)
		{
			return (AsymmetricAlgorithm)CryptoConfig.CreateFromName(algName);
		}

		/// <summary>When overridden in a derived class, reconstructs an <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> object from an XML string. Otherwise, throws a <see cref="T:System.NotImplementedException" />.</summary>
		/// <param name="xmlString">The XML string to use to reconstruct the <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> object.</param>
		public virtual void FromXmlString(string xmlString)
		{
			throw new NotImplementedException();
		}

		/// <summary>When overridden in a derived class, creates and returns an XML string representation of the current <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> object. Otherwise, throws a <see cref="T:System.NotImplementedException" />.</summary>
		/// <param name="includePrivateParameters">
		///   <see langword="true" /> to include private parameters; otherwise, <see langword="false" />.</param>
		/// <returns>An XML string encoding of the current <see cref="T:System.Security.Cryptography.AsymmetricAlgorithm" /> object.</returns>
		public virtual string ToXmlString(bool includePrivateParameters)
		{
			throw new NotImplementedException();
		}

		public virtual byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual byte[] ExportPkcs8PrivateKey()
		{
			throw new PlatformNotSupportedException();
		}

		public virtual byte[] ExportSubjectPublicKeyInfo()
		{
			throw new PlatformNotSupportedException();
		}

		public virtual void ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, ReadOnlySpan<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual void ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, ReadOnlySpan<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual void ImportPkcs8PrivateKey(ReadOnlySpan<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual void ImportSubjectPublicKeyInfo(ReadOnlySpan<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual bool TryExportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual bool TryExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual bool TryExportPkcs8PrivateKey(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>Represents the size, in bits, of the key modulus used by the asymmetric algorithm.</summary>
		protected int KeySizeValue;

		/// <summary>Specifies the key sizes that are supported by the asymmetric algorithm.</summary>
		protected KeySizes[] LegalKeySizesValue;
	}
}
