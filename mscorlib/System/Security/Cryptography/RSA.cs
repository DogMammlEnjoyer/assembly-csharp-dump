using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Util;
using System.Text;

namespace System.Security.Cryptography
{
	/// <summary>Represents the base class from which all implementations of the <see cref="T:System.Security.Cryptography.RSA" /> algorithm inherit.</summary>
	[ComVisible(true)]
	public abstract class RSA : AsymmetricAlgorithm
	{
		/// <summary>Creates an instance of the default implementation of the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		/// <returns>A new instance of the default implementation of <see cref="T:System.Security.Cryptography.RSA" />.</returns>
		public new static RSA Create()
		{
			return RSA.Create("System.Security.Cryptography.RSA");
		}

		/// <summary>Creates an instance of the specified implementation of <see cref="T:System.Security.Cryptography.RSA" />.</summary>
		/// <param name="algName">The name of the implementation of <see cref="T:System.Security.Cryptography.RSA" /> to use.</param>
		/// <returns>A new instance of the specified implementation of <see cref="T:System.Security.Cryptography.RSA" />.</returns>
		public new static RSA Create(string algName)
		{
			return (RSA)CryptoConfig.CreateFromName(algName);
		}

		/// <summary>When overridden in a derived class, encrypts the input data using the specified padding mode.</summary>
		/// <param name="data">The data to encrypt.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>The encrypted data.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		public virtual byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)
		{
			throw RSA.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, decrypts the input data using the specified padding mode.</summary>
		/// <param name="data">The data to decrypt.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>The decrypted data.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		public virtual byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)
		{
			throw RSA.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, computes the signature for the specified hash value by encrypting it with the private key using the specified padding.</summary>
		/// <param name="hash">The hash value of the data to be signed.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value of the data.</param>
		/// <param name="padding">The padding.</param>
		/// <returns>The RSA signature for the specified hash value.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		public virtual byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			throw RSA.DerivedClassMustOverride();
		}

		/// <summary>Verifies that a digital signature is valid by determining the hash value in the signature using the specified hash algorithm and padding, and comparing it to the provided hash value.</summary>
		/// <param name="hash">The hash value of the signed data.</param>
		/// <param name="signature">The signature data to be verified.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>
		///   <see langword="true" /> if the signature is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		public virtual bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			throw RSA.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, computes the hash value of a specified portion of a byte array by using a specified hashing algorithm.</summary>
		/// <param name="data">The data to be hashed.</param>
		/// <param name="offset">The index of the first byte in <paramref name="data" /> that is to be hashed.</param>
		/// <param name="count">The number of bytes to hash.</param>
		/// <param name="hashAlgorithm">The algorithm to use in hash the data.</param>
		/// <returns>The hashed data.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		protected virtual byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			throw RSA.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, computes the hash value of a specified binary stream by using a specified hashing algorithm.</summary>
		/// <param name="data">The binary stream to hash.</param>
		/// <param name="hashAlgorithm">The hash algorithm.</param>
		/// <returns>The hashed data.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		protected virtual byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			throw RSA.DerivedClassMustOverride();
		}

		/// <summary>Computes the hash value of the specified byte array using the specified hash algorithm and padding mode, and signs the resulting hash value.</summary>
		/// <param name="data">The input data for which to compute the hash.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>The RSA signature for the specified data.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="padding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return this.SignData(data, 0, data.Length, hashAlgorithm, padding);
		}

		/// <summary>Computes the hash value of a portion of the specified byte array using the specified hash algorithm and padding mode, and signs the resulting hash value.</summary>
		/// <param name="data">The input data for which to compute the hash.</param>
		/// <param name="offset">The offset into the array at which to begin using data.</param>
		/// <param name="count">The number of bytes in the array to use as data.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>The RSA signature for the specified data.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="padding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than zero.  
		/// -or-  
		/// <paramref name="count" /> is less than zero.  
		/// -or-  
		/// <paramref name="offset" /> + <paramref name="count" /> - 1 results in an index that is beyond the upper bound of <paramref name="data" />.</exception>
		public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0 || offset > data.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > data.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = this.HashData(data, offset, count, hashAlgorithm);
			return this.SignHash(hash, hashAlgorithm, padding);
		}

		/// <summary>Computes the hash value of the specified stream using the specified hash algorithm and padding mode, and signs the resulting hash value.</summary>
		/// <param name="data">The input stream for which to compute the hash.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>The RSA signature for the specified data.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="padding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = this.HashData(data, hashAlgorithm);
			return this.SignHash(hash, hashAlgorithm, padding);
		}

		/// <summary>Verifies that a digital signature is valid by calculating the hash value of the specified data using the specified hash algorithm and padding, and comparing it to the provided signature.</summary>
		/// <param name="data">The signed data.</param>
		/// <param name="signature">The signature data to be verified.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value of the data.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>
		///   <see langword="true" /> if the signature is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="signature" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="padding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return this.VerifyData(data, 0, data.Length, signature, hashAlgorithm, padding);
		}

		/// <summary>Verifies that a digital signature is valid by calculating the hash value of the data in a portion of a byte array using the specified hash algorithm and padding, and comparing it to the provided signature.</summary>
		/// <param name="data">The signed data.</param>
		/// <param name="offset">The starting index at which to compute the hash.</param>
		/// <param name="count">The number of bytes to hash.</param>
		/// <param name="signature">The signature data to be verified.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value of the data.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>
		///   <see langword="true" /> if the signature is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="signature" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="padding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than zero.  
		/// -or-  
		/// <paramref name="count" /> is less than zero.  
		/// -or-  
		/// <paramref name="offset" /> + <paramref name="count" /> - 1 results in an index that is beyond the upper bound of <paramref name="data" />.</exception>
		public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0 || offset > data.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || count > data.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = this.HashData(data, offset, count, hashAlgorithm);
			return this.VerifyHash(hash, signature, hashAlgorithm, padding);
		}

		/// <summary>Verifies that a digital signature is valid by calculating the hash value of the specified stream using the specified hash algorithm and padding, and comparing it to the provided signature.</summary>
		/// <param name="data">The signed data.</param>
		/// <param name="signature">The signature data to be verified.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value of the data.</param>
		/// <param name="padding">The padding mode.</param>
		/// <returns>
		///   <see langword="true" /> if the signature is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="signature" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="padding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			byte[] hash = this.HashData(data, hashAlgorithm);
			return this.VerifyHash(hash, signature, hashAlgorithm, padding);
		}

		private static Exception DerivedClassMustOverride()
		{
			return new NotImplementedException(Environment.GetResourceString("Derived classes must provide an implementation."));
		}

		internal static Exception HashAlgorithmNameNullOrEmpty()
		{
			return new ArgumentException(Environment.GetResourceString("The hash algorithm name cannot be null or empty."), "hashAlgorithm");
		}

		/// <summary>When overridden in a derived class, decrypts the input data using the private key.</summary>
		/// <param name="rgb">The cipher text to be decrypted.</param>
		/// <returns>The resulting decryption of the <paramref name="rgb" /> parameter in plain text.</returns>
		/// <exception cref="T:System.NotSupportedException">This method call is not supported. This exception is thrown starting with the .NET Framework 4.6.</exception>
		public virtual byte[] DecryptValue(byte[] rgb)
		{
			throw new NotSupportedException(Environment.GetResourceString("Method is not supported."));
		}

		/// <summary>When overridden in a derived class, encrypts the input data using the public key.</summary>
		/// <param name="rgb">The plain text to be encrypted.</param>
		/// <returns>The resulting encryption of the <paramref name="rgb" /> parameter as cipher text.</returns>
		/// <exception cref="T:System.NotSupportedException">This method call is not supported. This exception is thrown starting with the .NET Framework 4.6.</exception>
		public virtual byte[] EncryptValue(byte[] rgb)
		{
			throw new NotSupportedException(Environment.GetResourceString("Method is not supported."));
		}

		/// <summary>Gets the name of the key exchange algorithm available with this implementation of <see cref="T:System.Security.Cryptography.RSA" />.</summary>
		/// <returns>Returns "RSA".</returns>
		public override string KeyExchangeAlgorithm
		{
			get
			{
				return "RSA";
			}
		}

		/// <summary>Gets the name of the signature algorithm available with this implementation of <see cref="T:System.Security.Cryptography.RSA" />.</summary>
		/// <returns>Returns "RSA".</returns>
		public override string SignatureAlgorithm
		{
			get
			{
				return "RSA";
			}
		}

		/// <summary>Initializes an <see cref="T:System.Security.Cryptography.RSA" /> object from the key information from an XML string.</summary>
		/// <param name="xmlString">The XML string containing <see cref="T:System.Security.Cryptography.RSA" /> key information.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="xmlString" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The format of the <paramref name="xmlString" /> parameter is not valid.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported.</exception>
		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			RSAParameters parameters = default(RSAParameters);
			SecurityElement topElement = new Parser(xmlString).GetTopElement();
			string text = topElement.SearchForTextOfLocalName("Modulus");
			if (text == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
				{
					"RSA",
					"Modulus"
				}));
			}
			parameters.Modulus = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text));
			string text2 = topElement.SearchForTextOfLocalName("Exponent");
			if (text2 == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
				{
					"RSA",
					"Exponent"
				}));
			}
			parameters.Exponent = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text2));
			string text3 = topElement.SearchForTextOfLocalName("P");
			if (text3 != null)
			{
				parameters.P = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text3));
			}
			string text4 = topElement.SearchForTextOfLocalName("Q");
			if (text4 != null)
			{
				parameters.Q = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text4));
			}
			string text5 = topElement.SearchForTextOfLocalName("DP");
			if (text5 != null)
			{
				parameters.DP = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text5));
			}
			string text6 = topElement.SearchForTextOfLocalName("DQ");
			if (text6 != null)
			{
				parameters.DQ = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text6));
			}
			string text7 = topElement.SearchForTextOfLocalName("InverseQ");
			if (text7 != null)
			{
				parameters.InverseQ = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text7));
			}
			string text8 = topElement.SearchForTextOfLocalName("D");
			if (text8 != null)
			{
				parameters.D = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text8));
			}
			this.ImportParameters(parameters);
		}

		/// <summary>Creates and returns an XML string containing the key of the current <see cref="T:System.Security.Cryptography.RSA" /> object.</summary>
		/// <param name="includePrivateParameters">
		///   <see langword="true" /> to include a public and private RSA key; <see langword="false" /> to include only the public key.</param>
		/// <returns>An XML string containing the key of the current <see cref="T:System.Security.Cryptography.RSA" /> object.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Core only: This member is not supported.</exception>
		public override string ToXmlString(bool includePrivateParameters)
		{
			RSAParameters rsaparameters = this.ExportParameters(includePrivateParameters);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<RSAKeyValue>");
			stringBuilder.Append("<Modulus>" + Convert.ToBase64String(rsaparameters.Modulus) + "</Modulus>");
			stringBuilder.Append("<Exponent>" + Convert.ToBase64String(rsaparameters.Exponent) + "</Exponent>");
			if (includePrivateParameters)
			{
				stringBuilder.Append("<P>" + Convert.ToBase64String(rsaparameters.P) + "</P>");
				stringBuilder.Append("<Q>" + Convert.ToBase64String(rsaparameters.Q) + "</Q>");
				stringBuilder.Append("<DP>" + Convert.ToBase64String(rsaparameters.DP) + "</DP>");
				stringBuilder.Append("<DQ>" + Convert.ToBase64String(rsaparameters.DQ) + "</DQ>");
				stringBuilder.Append("<InverseQ>" + Convert.ToBase64String(rsaparameters.InverseQ) + "</InverseQ>");
				stringBuilder.Append("<D>" + Convert.ToBase64String(rsaparameters.D) + "</D>");
			}
			stringBuilder.Append("</RSAKeyValue>");
			return stringBuilder.ToString();
		}

		/// <summary>When overridden in a derived class, exports the <see cref="T:System.Security.Cryptography.RSAParameters" />.</summary>
		/// <param name="includePrivateParameters">
		///   <see langword="true" /> to include private parameters; otherwise, <see langword="false" />.</param>
		/// <returns>The parameters for <see cref="T:System.Security.Cryptography.RSA" />.</returns>
		public abstract RSAParameters ExportParameters(bool includePrivateParameters);

		/// <summary>When overridden in a derived class, imports the specified <see cref="T:System.Security.Cryptography.RSAParameters" />.</summary>
		/// <param name="parameters">The parameters for <see cref="T:System.Security.Cryptography.RSA" />.</param>
		public abstract void ImportParameters(RSAParameters parameters);

		/// <summary>Creates a new ephemeral RSA key with the specified key size.</summary>
		/// <param name="keySizeInBits">The key size, in bits.</param>
		/// <returns>A new ephemeral RSA key with the specified key size.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///   <paramref name="keySizeInBits" /> is different than <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.KeySize" />.</exception>
		public static RSA Create(int keySizeInBits)
		{
			RSA rsa = RSA.Create();
			RSA result;
			try
			{
				rsa.KeySize = keySizeInBits;
				result = rsa;
			}
			catch
			{
				rsa.Dispose();
				throw;
			}
			return result;
		}

		/// <summary>Creates a new ephemeral RSA key with the specified RSA key parameters.</summary>
		/// <param name="parameters">The parameters for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</param>
		/// <returns>A new ephemeral RSA key.</returns>
		public static RSA Create(RSAParameters parameters)
		{
			RSA rsa = RSA.Create();
			RSA result;
			try
			{
				rsa.ImportParameters(parameters);
				result = rsa;
			}
			catch
			{
				rsa.Dispose();
				throw;
			}
			return result;
		}

		public virtual bool TryDecrypt(ReadOnlySpan<byte> data, Span<byte> destination, RSAEncryptionPadding padding, out int bytesWritten)
		{
			byte[] array = this.Decrypt(data.ToArray(), padding);
			if (destination.Length >= array.Length)
			{
				new ReadOnlySpan<byte>(array).CopyTo(destination);
				bytesWritten = array.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool TryEncrypt(ReadOnlySpan<byte> data, Span<byte> destination, RSAEncryptionPadding padding, out int bytesWritten)
		{
			byte[] array = this.Encrypt(data.ToArray(), padding);
			if (destination.Length >= array.Length)
			{
				new ReadOnlySpan<byte>(array).CopyTo(destination);
				bytesWritten = array.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		protected virtual bool TryHashData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(data.Length);
			byte[] array2;
			try
			{
				data.CopyTo(array);
				array2 = this.HashData(array, 0, data.Length, hashAlgorithm);
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array, false);
			}
			if (destination.Length >= array2.Length)
			{
				new ReadOnlySpan<byte>(array2).CopyTo(destination);
				bytesWritten = array2.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool TrySignHash(ReadOnlySpan<byte> hash, Span<byte> destination, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding, out int bytesWritten)
		{
			byte[] array = this.SignHash(hash.ToArray(), hashAlgorithm, padding);
			if (destination.Length >= array.Length)
			{
				new ReadOnlySpan<byte>(array).CopyTo(destination);
				bytesWritten = array.Length;
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool TrySignData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding, out int bytesWritten)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			int length;
			if (this.TryHashData(data, destination, hashAlgorithm, out length) && this.TrySignHash(destination.Slice(0, length), destination, hashAlgorithm, padding, out bytesWritten))
			{
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			int num = 256;
			checked
			{
				bool result;
				for (;;)
				{
					int length = 0;
					byte[] array = ArrayPool<byte>.Shared.Rent(num);
					try
					{
						if (this.TryHashData(data, array, hashAlgorithm, out length))
						{
							result = this.VerifyHash(new ReadOnlySpan<byte>(array, 0, length), signature, hashAlgorithm, padding);
							break;
						}
					}
					finally
					{
						Array.Clear(array, 0, length);
						ArrayPool<byte>.Shared.Return(array, false);
					}
					num *= 2;
				}
				return result;
			}
		}

		public virtual bool VerifyHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			return this.VerifyHash(hash.ToArray(), signature.ToArray(), hashAlgorithm, padding);
		}

		public virtual byte[] ExportRSAPrivateKey()
		{
			throw new PlatformNotSupportedException();
		}

		public virtual byte[] ExportRSAPublicKey()
		{
			throw new PlatformNotSupportedException();
		}

		public virtual void ImportRSAPrivateKey(ReadOnlySpan<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual void ImportRSAPublicKey(ReadOnlySpan<byte> source, out int bytesRead)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual bool TryExportRSAPrivateKey(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}

		public virtual bool TryExportRSAPublicKey(Span<byte> destination, out int bytesWritten)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
