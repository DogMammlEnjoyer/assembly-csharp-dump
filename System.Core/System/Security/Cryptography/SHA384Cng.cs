using System;

namespace System.Security.Cryptography
{
	/// <summary>Provides a Cryptography Next Generation (CNG) implementation of the Secure Hash Algorithm (SHA) for 384-bit hash values.</summary>
	public sealed class SHA384Cng : SHA384
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.SHA384Cng" /> class. </summary>
		[SecurityCritical]
		public SHA384Cng()
		{
			this.hash = new SHA384Managed();
		}

		/// <summary>Initializes, or re-initializes, the instance of the hash algorithm. </summary>
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
			this.hash.TransformFinalBlock(SHA384Cng.Empty, 0, 0);
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

		private SHA384 hash;
	}
}
