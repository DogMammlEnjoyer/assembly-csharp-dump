using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Util;
using System.Text;

namespace System.Security.Cryptography
{
	/// <summary>Represents the abstract base class from which all implementations of the Digital Signature Algorithm (<see cref="T:System.Security.Cryptography.DSA" />) must inherit.</summary>
	[ComVisible(true)]
	public abstract class DSA : AsymmetricAlgorithm
	{
		/// <summary>Creates the default cryptographic object used to perform the asymmetric algorithm.</summary>
		/// <returns>A cryptographic object used to perform the asymmetric algorithm.</returns>
		public new static DSA Create()
		{
			return DSA.Create("System.Security.Cryptography.DSA");
		}

		/// <summary>Creates the specified cryptographic object used to perform the asymmetric algorithm.</summary>
		/// <param name="algName">The name of the specific implementation of <see cref="T:System.Security.Cryptography.DSA" /> to use.</param>
		/// <returns>A cryptographic object used to perform the asymmetric algorithm.</returns>
		public new static DSA Create(string algName)
		{
			return (DSA)CryptoConfig.CreateFromName(algName);
		}

		/// <summary>When overridden in a derived class, creates the <see cref="T:System.Security.Cryptography.DSA" /> signature for the specified data.</summary>
		/// <param name="rgbHash">The data to be signed.</param>
		/// <returns>The digital signature for the specified data.</returns>
		public abstract byte[] CreateSignature(byte[] rgbHash);

		/// <summary>When overridden in a derived class, verifies the <see cref="T:System.Security.Cryptography.DSA" /> signature for the specified data.</summary>
		/// <param name="rgbHash">The hash of the data signed with <paramref name="rgbSignature" />.</param>
		/// <param name="rgbSignature">The signature to be verified for rgbData.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="rgbSignature" /> matches the signature computed using the specified hash algorithm and key on <paramref name="rgbHash" />; otherwise, <see langword="false" />.</returns>
		public abstract bool VerifySignature(byte[] rgbHash, byte[] rgbSignature);

		/// <summary>When overridden in a derived class, computes the hash value of a specified portion of a byte array by using a specified hashing algorithm.</summary>
		/// <param name="data">The data to be hashed.</param>
		/// <param name="offset">The index of the first byte in <paramref name="data" /> that is to be hashed.</param>
		/// <param name="count">The number of bytes to hash.</param>
		/// <param name="hashAlgorithm">The algorithm to use to hash the data.</param>
		/// <returns>The hashed data.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		protected virtual byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			throw DSA.DerivedClassMustOverride();
		}

		/// <summary>When overridden in a derived class, computes the hash value of a specified binary stream by using a specified hashing algorithm.</summary>
		/// <param name="data">The binary stream to hash.</param>
		/// <param name="hashAlgorithm">The algorithm to use to hash the data.</param>
		/// <returns>The hashed data.</returns>
		/// <exception cref="T:System.NotImplementedException">A derived class must override this method.</exception>
		protected virtual byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			throw DSA.DerivedClassMustOverride();
		}

		/// <summary>Computes the hash value of the specified byte array using the specified hash algorithm and signs the resulting hash value.</summary>
		/// <param name="data">The input data for which to compute the hash.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
		/// <returns>The DSA signature for the specified data.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return this.SignData(data, 0, data.Length, hashAlgorithm);
		}

		/// <summary>Computes the hash value of a portion of the specified byte array using the specified hash algorithm and signs the resulting hash value.</summary>
		/// <param name="data">The input data for which to compute the hash.</param>
		/// <param name="offset">The offset into the array at which to begin using data.</param>
		/// <param name="count">The number of bytes in the array to use as data.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
		/// <returns>The DSA signature for the specified data.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than zero.  
		/// -or-  
		/// <paramref name="count" /> is less than zero.  
		/// -or-  
		/// <paramref name="offset" /> + <paramref name="count" /> - 1 results in an index that is beyond the upper bound of <paramref name="data" />.</exception>
		public virtual byte[] SignData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
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
				throw DSA.HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = this.HashData(data, offset, count, hashAlgorithm);
			return this.CreateSignature(rgbHash);
		}

		/// <summary>Computes the hash value of the specified stream using the specified hash algorithm and signs the resulting hash value.</summary>
		/// <param name="data">The input stream for which to compute the hash.</param>
		/// <param name="hashAlgorithm">The hash algorithm to use to create the hash value.</param>
		/// <returns>The DSA signature for the specified data.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public virtual byte[] SignData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw DSA.HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = this.HashData(data, hashAlgorithm);
			return this.CreateSignature(rgbHash);
		}

		/// <summary>Verifies that a digital signature is valid by calculating the hash value of the specified data using the specified hash algorithm and comparing it to the provided signature.</summary>
		/// <param name="data">The signed data.</param>
		/// <param name="signature">The signature data to be verified.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value of the data.</param>
		/// <returns>
		///   <see langword="true" /> if the digital signature is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="signature" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName hashAlgorithm)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			return this.VerifyData(data, 0, data.Length, signature, hashAlgorithm);
		}

		/// <summary>Verifies that a digital signature is valid by calculating the hash value of the data in a portion of a byte array using the specified hash algorithm and comparing it to the provided signature.</summary>
		/// <param name="data">The signed data.</param>
		/// <param name="offset">The starting index at which to compute the hash.</param>
		/// <param name="count">The number of bytes to hash.</param>
		/// <param name="signature">The signature data to be verified.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value of the data.</param>
		/// <returns>
		///   <see langword="true" /> if the digital signature is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="signature" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than zero.  
		/// -or-  
		/// <paramref name="count" /> is less than zero.  
		/// -or-  
		/// <paramref name="offset" /> + <paramref name="count" /> - 1 results in an index that is beyond the upper bound of <paramref name="data" />.</exception>
		public virtual bool VerifyData(byte[] data, int offset, int count, byte[] signature, HashAlgorithmName hashAlgorithm)
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
				throw DSA.HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = this.HashData(data, offset, count, hashAlgorithm);
			return this.VerifySignature(rgbHash, signature);
		}

		/// <summary>Verifies that a digital signature is valid by calculating the hash value of the specified stream using the specified hash algorithm and comparing it to the provided signature.</summary>
		/// <param name="data">The signed data.</param>
		/// <param name="signature">The signature data to be verified.</param>
		/// <param name="hashAlgorithm">The hash algorithm used to create the hash value of the data.</param>
		/// <returns>
		///   <see langword="true" /> if the digital signature is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="data" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="signature" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="hashAlgorithm" />.<see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> is <see langword="null" /> or <see cref="F:System.String.Empty" />.</exception>
		public virtual bool VerifyData(Stream data, byte[] signature, HashAlgorithmName hashAlgorithm)
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
				throw DSA.HashAlgorithmNameNullOrEmpty();
			}
			byte[] rgbHash = this.HashData(data, hashAlgorithm);
			return this.VerifySignature(rgbHash, signature);
		}

		/// <summary>Reconstructs a <see cref="T:System.Security.Cryptography.DSA" /> object from an XML string.</summary>
		/// <param name="xmlString">The XML string to use to reconstruct the <see cref="T:System.Security.Cryptography.DSA" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="xmlString" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The format of the <paramref name="xmlString" /> parameter is not valid.</exception>
		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			DSAParameters parameters = default(DSAParameters);
			SecurityElement topElement = new Parser(xmlString).GetTopElement();
			string text = topElement.SearchForTextOfLocalName("P");
			if (text == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
				{
					"DSA",
					"P"
				}));
			}
			parameters.P = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text));
			string text2 = topElement.SearchForTextOfLocalName("Q");
			if (text2 == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
				{
					"DSA",
					"Q"
				}));
			}
			parameters.Q = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text2));
			string text3 = topElement.SearchForTextOfLocalName("G");
			if (text3 == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
				{
					"DSA",
					"G"
				}));
			}
			parameters.G = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text3));
			string text4 = topElement.SearchForTextOfLocalName("Y");
			if (text4 == null)
			{
				throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
				{
					"DSA",
					"Y"
				}));
			}
			parameters.Y = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text4));
			string text5 = topElement.SearchForTextOfLocalName("J");
			if (text5 != null)
			{
				parameters.J = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text5));
			}
			string text6 = topElement.SearchForTextOfLocalName("X");
			if (text6 != null)
			{
				parameters.X = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text6));
			}
			string text7 = topElement.SearchForTextOfLocalName("Seed");
			string text8 = topElement.SearchForTextOfLocalName("PgenCounter");
			if (text7 != null && text8 != null)
			{
				parameters.Seed = Convert.FromBase64String(Utils.DiscardWhiteSpaces(text7));
				parameters.Counter = Utils.ConvertByteArrayToInt(Convert.FromBase64String(Utils.DiscardWhiteSpaces(text8)));
			}
			else if (text7 != null || text8 != null)
			{
				if (text7 == null)
				{
					throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
					{
						"DSA",
						"Seed"
					}));
				}
				throw new CryptographicException(Environment.GetResourceString("Input string does not contain a valid encoding of the '{0}' '{1}' parameter.", new object[]
				{
					"DSA",
					"PgenCounter"
				}));
			}
			this.ImportParameters(parameters);
		}

		/// <summary>Creates and returns an XML string representation of the current <see cref="T:System.Security.Cryptography.DSA" /> object.</summary>
		/// <param name="includePrivateParameters">
		///   <see langword="true" /> to include private parameters; otherwise, <see langword="false" />.</param>
		/// <returns>An XML string encoding of the current <see cref="T:System.Security.Cryptography.DSA" /> object.</returns>
		public override string ToXmlString(bool includePrivateParameters)
		{
			DSAParameters dsaparameters = this.ExportParameters(includePrivateParameters);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<DSAKeyValue>");
			stringBuilder.Append("<P>" + Convert.ToBase64String(dsaparameters.P) + "</P>");
			stringBuilder.Append("<Q>" + Convert.ToBase64String(dsaparameters.Q) + "</Q>");
			stringBuilder.Append("<G>" + Convert.ToBase64String(dsaparameters.G) + "</G>");
			stringBuilder.Append("<Y>" + Convert.ToBase64String(dsaparameters.Y) + "</Y>");
			if (dsaparameters.J != null)
			{
				stringBuilder.Append("<J>" + Convert.ToBase64String(dsaparameters.J) + "</J>");
			}
			if (dsaparameters.Seed != null)
			{
				stringBuilder.Append("<Seed>" + Convert.ToBase64String(dsaparameters.Seed) + "</Seed>");
				stringBuilder.Append("<PgenCounter>" + Convert.ToBase64String(Utils.ConvertIntToByteArray(dsaparameters.Counter)) + "</PgenCounter>");
			}
			if (includePrivateParameters)
			{
				stringBuilder.Append("<X>" + Convert.ToBase64String(dsaparameters.X) + "</X>");
			}
			stringBuilder.Append("</DSAKeyValue>");
			return stringBuilder.ToString();
		}

		/// <summary>When overridden in a derived class, exports the <see cref="T:System.Security.Cryptography.DSAParameters" />.</summary>
		/// <param name="includePrivateParameters">
		///   <see langword="true" /> to include private parameters; otherwise, <see langword="false" />.</param>
		/// <returns>The parameters for <see cref="T:System.Security.Cryptography.DSA" />.</returns>
		public abstract DSAParameters ExportParameters(bool includePrivateParameters);

		/// <summary>When overridden in a derived class, imports the specified <see cref="T:System.Security.Cryptography.DSAParameters" />.</summary>
		/// <param name="parameters">The parameters for <see cref="T:System.Security.Cryptography.DSA" />.</param>
		public abstract void ImportParameters(DSAParameters parameters);

		private static Exception DerivedClassMustOverride()
		{
			return new NotImplementedException(Environment.GetResourceString("Derived classes must provide an implementation."));
		}

		internal static Exception HashAlgorithmNameNullOrEmpty()
		{
			return new ArgumentException(Environment.GetResourceString("The hash algorithm name cannot be null or empty."), "hashAlgorithm");
		}

		/// <summary>Creates a new ephemeral DSA key with the specified key size.</summary>
		/// <param name="keySizeInBits">The key size, in bits.</param>
		/// <returns>A new ephemeral DSA key with the specified key size.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">
		///   <paramref name="keySizeInBits" /> is different than <see cref="P:System.Security.Cryptography.AsymmetricAlgorithm.KeySize" />.</exception>
		public static DSA Create(int keySizeInBits)
		{
			DSA dsa = DSA.Create();
			DSA result;
			try
			{
				dsa.KeySize = keySizeInBits;
				result = dsa;
			}
			catch
			{
				dsa.Dispose();
				throw;
			}
			return result;
		}

		/// <summary>Creates a new ephemeral DSA key with the specified DSA key parameters.</summary>
		/// <param name="parameters">The parameters for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</param>
		/// <returns>A new ephemeral DSA key.</returns>
		public static DSA Create(DSAParameters parameters)
		{
			DSA dsa = DSA.Create();
			DSA result;
			try
			{
				dsa.ImportParameters(parameters);
				result = dsa;
			}
			catch
			{
				dsa.Dispose();
				throw;
			}
			return result;
		}

		public virtual bool TryCreateSignature(ReadOnlySpan<byte> hash, Span<byte> destination, out int bytesWritten)
		{
			byte[] array = this.CreateSignature(hash.ToArray());
			if (array.Length <= destination.Length)
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
			bool result;
			try
			{
				data.CopyTo(array);
				byte[] array2 = this.HashData(array, 0, data.Length, hashAlgorithm);
				if (destination.Length >= array2.Length)
				{
					new ReadOnlySpan<byte>(array2).CopyTo(destination);
					bytesWritten = array2.Length;
					result = true;
				}
				else
				{
					bytesWritten = 0;
					result = false;
				}
			}
			finally
			{
				Array.Clear(array, 0, data.Length);
				ArrayPool<byte>.Shared.Return(array, false);
			}
			return result;
		}

		public virtual bool TrySignData(ReadOnlySpan<byte> data, Span<byte> destination, HashAlgorithmName hashAlgorithm, out int bytesWritten)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw DSA.HashAlgorithmNameNullOrEmpty();
			}
			int length;
			if (this.TryHashData(data, destination, hashAlgorithm, out length) && this.TryCreateSignature(destination.Slice(0, length), destination, out bytesWritten))
			{
				return true;
			}
			bytesWritten = 0;
			return false;
		}

		public virtual bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, HashAlgorithmName hashAlgorithm)
		{
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw DSA.HashAlgorithmNameNullOrEmpty();
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
							result = this.VerifySignature(new ReadOnlySpan<byte>(array, 0, length), signature);
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

		public virtual bool VerifySignature(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature)
		{
			return this.VerifySignature(hash.ToArray(), signature.ToArray());
		}
	}
}
