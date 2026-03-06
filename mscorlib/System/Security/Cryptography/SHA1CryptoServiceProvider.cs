using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	/// <summary>Computes the <see cref="T:System.Security.Cryptography.SHA1" /> hash value for the input data using the implementation provided by the cryptographic service provider (CSP). This class cannot be inherited.</summary>
	[ComVisible(true)]
	public sealed class SHA1CryptoServiceProvider : SHA1
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.SHA1CryptoServiceProvider" /> class.</summary>
		public SHA1CryptoServiceProvider()
		{
			this.sha = new SHA1Internal();
		}

		~SHA1CryptoServiceProvider()
		{
			this.Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
		{
			this.State = 1;
			this.sha.HashCore(rgb, ibStart, cbSize);
		}

		protected override byte[] HashFinal()
		{
			this.State = 0;
			return this.sha.HashFinal();
		}

		/// <summary>Initializes an instance of <see cref="T:System.Security.Cryptography.SHA1CryptoServiceProvider" />.</summary>
		public override void Initialize()
		{
			this.sha.Initialize();
		}

		private SHA1Internal sha;
	}
}
