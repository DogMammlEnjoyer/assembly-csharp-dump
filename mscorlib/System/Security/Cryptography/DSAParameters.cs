using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	/// <summary>Contains the typical parameters for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
	[ComVisible(true)]
	[Serializable]
	public struct DSAParameters
	{
		/// <summary>Specifies the <see langword="P" /> parameter for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		public byte[] P;

		/// <summary>Specifies the <see langword="Q" /> parameter for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		public byte[] Q;

		/// <summary>Specifies the <see langword="G" /> parameter for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		public byte[] G;

		/// <summary>Specifies the <see langword="Y" /> parameter for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		public byte[] Y;

		/// <summary>Specifies the <see langword="J" /> parameter for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		public byte[] J;

		/// <summary>Specifies the <see langword="X" /> parameter for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		[NonSerialized]
		public byte[] X;

		/// <summary>Specifies the seed for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		public byte[] Seed;

		/// <summary>Specifies the counter for the <see cref="T:System.Security.Cryptography.DSA" /> algorithm.</summary>
		public int Counter;
	}
}
