using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	/// <summary>Represents the standard parameters for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
	[ComVisible(true)]
	[Serializable]
	public struct RSAParameters
	{
		/// <summary>Represents the <see langword="Exponent" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		public byte[] Exponent;

		/// <summary>Represents the <see langword="Modulus" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		public byte[] Modulus;

		/// <summary>Represents the <see langword="P" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		[NonSerialized]
		public byte[] P;

		/// <summary>Represents the <see langword="Q" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		[NonSerialized]
		public byte[] Q;

		/// <summary>Represents the <see langword="DP" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		[NonSerialized]
		public byte[] DP;

		/// <summary>Represents the <see langword="DQ" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		[NonSerialized]
		public byte[] DQ;

		/// <summary>Represents the <see langword="InverseQ" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		[NonSerialized]
		public byte[] InverseQ;

		/// <summary>Represents the <see langword="D" /> parameter for the <see cref="T:System.Security.Cryptography.RSA" /> algorithm.</summary>
		[NonSerialized]
		public byte[] D;
	}
}
