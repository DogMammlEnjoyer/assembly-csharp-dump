using System;

namespace System.Security.Cryptography
{
	/// <summary>Defines a wrapper object to access the cryptographic service provider (CSP) implementation of the <see cref="T:System.Security.Cryptography.SHA512" /> algorithm. </summary>
	public sealed class SHA512CryptoServiceProvider : SHA512
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.SHA512CryptoServiceProvider" /> class. </summary>
		[SecurityCritical]
		public SHA512CryptoServiceProvider()
		{
			this.hash = new SHA512Managed();
		}

		/// <summary>Initializes, or reinitializes, an instance of a hash algorithm.</summary>
		[SecurityCritical]
		public override void Initialize()
		{
			this.hash.Initialize();
		}

		[SecurityCritical]
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			this.hash.TransformBlock(array, ibStart, cbSize, null, 0);
		}

		[SecurityCritical]
		protected override byte[] HashFinal()
		{
			this.hash.TransformFinalBlock(SHA512CryptoServiceProvider.Empty, 0, 0);
			this.HashValue = this.hash.Hash;
			return this.HashValue;
		}

		[SecurityCritical]
		protected override void Dispose(bool disposing)
		{
			((IDisposable)this.hash).Dispose();
			base.Dispose(disposing);
		}

		private static byte[] Empty = new byte[0];

		private SHA512 hash;
	}
}
