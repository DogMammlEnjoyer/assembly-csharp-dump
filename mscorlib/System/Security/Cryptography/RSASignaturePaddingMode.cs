using System;

namespace System.Security.Cryptography
{
	/// <summary>Specifies the padding mode to use with RSA signature creation or verification operations.</summary>
	public enum RSASignaturePaddingMode
	{
		/// <summary>PKCS #1 v1.5</summary>
		Pkcs1,
		/// <summary>Probabilistic Signature Scheme</summary>
		Pss
	}
}
