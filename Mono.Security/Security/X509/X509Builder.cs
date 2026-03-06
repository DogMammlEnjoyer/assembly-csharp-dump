using System;
using System.Globalization;
using System.Security.Cryptography;

namespace Mono.Security.X509
{
	public abstract class X509Builder
	{
		protected X509Builder()
		{
			this.hashName = "SHA1";
		}

		protected abstract ASN1 ToBeSigned(string hashName);

		protected string GetOid(string hashName)
		{
			string text = hashName.ToLower(CultureInfo.InvariantCulture);
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2393554675U)
			{
				if (num != 2070555668U)
				{
					if (num != 2376777056U)
					{
						if (num == 2393554675U)
						{
							if (text == "md5")
							{
								return "1.2.840.113549.1.1.4";
							}
						}
					}
					else if (text == "md4")
					{
						return "1.2.840.113549.1.1.3";
					}
				}
				else if (text == "sha1")
				{
					return "1.2.840.113549.1.1.5";
				}
			}
			else if (num <= 2631153146U)
			{
				if (num != 2477442770U)
				{
					if (num == 2631153146U)
					{
						if (text == "sha256")
						{
							return "1.2.840.113549.1.1.11";
						}
					}
				}
				else if (text == "md2")
				{
					return "1.2.840.113549.1.1.2";
				}
			}
			else if (num != 2694049387U)
			{
				if (num == 2700614742U)
				{
					if (text == "sha384")
					{
						return "1.2.840.113549.1.1.12";
					}
				}
			}
			else if (text == "sha512")
			{
				return "1.2.840.113549.1.1.13";
			}
			throw new NotSupportedException("Unknown hash algorithm " + hashName);
		}

		public string Hash
		{
			get
			{
				return this.hashName;
			}
			set
			{
				if (this.hashName == null)
				{
					this.hashName = "SHA1";
					return;
				}
				this.hashName = value;
			}
		}

		public virtual byte[] Sign(AsymmetricAlgorithm aa)
		{
			if (aa is RSA)
			{
				return this.Sign(aa as RSA);
			}
			if (aa is DSA)
			{
				return this.Sign(aa as DSA);
			}
			throw new NotSupportedException("Unknown Asymmetric Algorithm " + aa.ToString());
		}

		private byte[] Build(ASN1 tbs, string hashoid, byte[] signature)
		{
			ASN1 asn = new ASN1(48);
			asn.Add(tbs);
			asn.Add(PKCS7.AlgorithmIdentifier(hashoid));
			byte[] array = new byte[signature.Length + 1];
			Buffer.BlockCopy(signature, 0, array, 1, signature.Length);
			asn.Add(new ASN1(3, array));
			return asn.GetBytes();
		}

		public virtual byte[] Sign(RSA key)
		{
			string oid = this.GetOid(this.hashName);
			ASN1 asn = this.ToBeSigned(oid);
			byte[] rgbHash = HashAlgorithm.Create(this.hashName).ComputeHash(asn.GetBytes());
			RSAPKCS1SignatureFormatter rsapkcs1SignatureFormatter = new RSAPKCS1SignatureFormatter(key);
			rsapkcs1SignatureFormatter.SetHashAlgorithm(this.hashName);
			byte[] signature = rsapkcs1SignatureFormatter.CreateSignature(rgbHash);
			return this.Build(asn, oid, signature);
		}

		public virtual byte[] Sign(DSA key)
		{
			string hashoid = "1.2.840.10040.4.3";
			ASN1 asn = this.ToBeSigned(hashoid);
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(this.hashName);
			if (!(hashAlgorithm is SHA1))
			{
				throw new NotSupportedException("Only SHA-1 is supported for DSA");
			}
			byte[] rgbHash = hashAlgorithm.ComputeHash(asn.GetBytes());
			DSASignatureFormatter dsasignatureFormatter = new DSASignatureFormatter(key);
			dsasignatureFormatter.SetHashAlgorithm(this.hashName);
			byte[] src = dsasignatureFormatter.CreateSignature(rgbHash);
			byte[] array = new byte[20];
			Buffer.BlockCopy(src, 0, array, 0, 20);
			byte[] array2 = new byte[20];
			Buffer.BlockCopy(src, 20, array2, 0, 20);
			ASN1 asn2 = new ASN1(48);
			asn2.Add(new ASN1(2, array));
			asn2.Add(new ASN1(2, array2));
			return this.Build(asn, hashoid, asn2.GetBytes());
		}

		private const string defaultHash = "SHA1";

		private string hashName;
	}
}
