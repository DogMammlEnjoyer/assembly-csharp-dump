using System;

namespace System.Security.Authentication
{
	/// <summary>Specifies the algorithm used for generating message authentication codes (MACs).</summary>
	public enum HashAlgorithmType
	{
		/// <summary>No hashing algorithm is used.</summary>
		None,
		/// <summary>The Message Digest 5 (MD5) hashing algorithm.</summary>
		Md5 = 32771,
		/// <summary>The Secure Hashing Algorithm (SHA1).</summary>
		Sha1,
		/// <summary>The Secure Hashing Algorithm 2 (SHA-2), using a 256-bit digest.</summary>
		Sha256 = 32780,
		/// <summary>The Secure Hashing Algorithm 2 (SHA-2), using a 384-bit digest.</summary>
		Sha384,
		/// <summary>The Secure Hashing Algorithm 2 (SHA-2), using a 512-bit digest.</summary>
		Sha512
	}
}
