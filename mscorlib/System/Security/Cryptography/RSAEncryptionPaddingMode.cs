using System;

namespace System.Security.Cryptography
{
	/// <summary>Specifies the padding mode to use with RSA encryption or decryption operations.</summary>
	public enum RSAEncryptionPaddingMode
	{
		/// <summary>PKCS #1 v1.5.</summary>
		Pkcs1,
		/// <summary>Optimal Asymmetric Encryption Padding.</summary>
		Oaep
	}
}
