using System;

namespace System.Security.Cryptography
{
	/// <summary>Represents a (X,Y) coordinate pair for elliptic curve cryptography (ECC) structures.</summary>
	public struct ECPoint
	{
		/// <summary>Represents the X coordinate.</summary>
		/// <returns>The X coordinate.</returns>
		public byte[] X;

		/// <summary>Represents the Y coordinate.</summary>
		/// <returns>The Y coordinate.</returns>
		public byte[] Y;
	}
}
