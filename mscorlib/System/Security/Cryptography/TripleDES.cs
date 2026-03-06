using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	/// <summary>Represents the base class for Triple Data Encryption Standard algorithms from which all <see cref="T:System.Security.Cryptography.TripleDES" /> implementations must derive.</summary>
	[ComVisible(true)]
	public abstract class TripleDES : SymmetricAlgorithm
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.TripleDES" /> class.</summary>
		protected TripleDES()
		{
			this.KeySizeValue = 192;
			this.BlockSizeValue = 64;
			this.FeedbackSizeValue = this.BlockSizeValue;
			this.LegalBlockSizesValue = TripleDES.s_legalBlockSizes;
			this.LegalKeySizesValue = TripleDES.s_legalKeySizes;
		}

		/// <summary>Gets or sets the secret key for the <see cref="T:System.Security.Cryptography.TripleDES" /> algorithm.</summary>
		/// <returns>The secret key for the <see cref="T:System.Security.Cryptography.TripleDES" /> algorithm.</returns>
		/// <exception cref="T:System.ArgumentNullException">An attempt was made to set the key to <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">An attempt was made to set a key whose length is invalid.  
		///  -or-  
		///  An attempt was made to set a weak key (see <see cref="M:System.Security.Cryptography.TripleDES.IsWeakKey(System.Byte[])" />).</exception>
		public override byte[] Key
		{
			get
			{
				if (this.KeyValue == null)
				{
					do
					{
						this.GenerateKey();
					}
					while (TripleDES.IsWeakKey(this.KeyValue));
				}
				return (byte[])this.KeyValue.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!base.ValidKeySize(value.Length * 8))
				{
					throw new CryptographicException(Environment.GetResourceString("Specified key is not a valid size for this algorithm."));
				}
				if (TripleDES.IsWeakKey(value))
				{
					throw new CryptographicException(Environment.GetResourceString("Specified key is a known weak key for '{0}' and cannot be used."), "TripleDES");
				}
				this.KeyValue = (byte[])value.Clone();
				this.KeySizeValue = value.Length * 8;
			}
		}

		/// <summary>Creates an instance of a cryptographic object to perform the <see cref="T:System.Security.Cryptography.TripleDES" /> algorithm.</summary>
		/// <returns>An instance of a cryptographic object.</returns>
		public new static TripleDES Create()
		{
			return TripleDES.Create("System.Security.Cryptography.TripleDES");
		}

		/// <summary>Creates an instance of a cryptographic object to perform the specified implementation of the <see cref="T:System.Security.Cryptography.TripleDES" /> algorithm.</summary>
		/// <param name="str">The name of the specific implementation of <see cref="T:System.Security.Cryptography.TripleDES" /> to use.</param>
		/// <returns>An instance of a cryptographic object.</returns>
		public new static TripleDES Create(string str)
		{
			return (TripleDES)CryptoConfig.CreateFromName(str);
		}

		/// <summary>Determines whether the specified key is weak.</summary>
		/// <param name="rgbKey">The secret key to test for weakness.</param>
		/// <returns>
		///   <see langword="true" /> if the key is weak; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The size of the <paramref name="rgbKey" /> parameter is not valid.</exception>
		public static bool IsWeakKey(byte[] rgbKey)
		{
			if (!TripleDES.IsLegalKeySize(rgbKey))
			{
				throw new CryptographicException(Environment.GetResourceString("Specified key is not a valid size for this algorithm."));
			}
			byte[] array = Utils.FixupKeyParity(rgbKey);
			return TripleDES.EqualBytes(array, 0, 8, 8) || (array.Length == 24 && TripleDES.EqualBytes(array, 8, 16, 8));
		}

		private static bool EqualBytes(byte[] rgbKey, int start1, int start2, int count)
		{
			if (start1 < 0)
			{
				throw new ArgumentOutOfRangeException("start1", Environment.GetResourceString("Non-negative number required."));
			}
			if (start2 < 0)
			{
				throw new ArgumentOutOfRangeException("start2", Environment.GetResourceString("Non-negative number required."));
			}
			if (start1 + count > rgbKey.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Value was invalid."));
			}
			if (start2 + count > rgbKey.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Value was invalid."));
			}
			for (int i = 0; i < count; i++)
			{
				if (rgbKey[start1 + i] != rgbKey[start2 + i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsLegalKeySize(byte[] rgbKey)
		{
			return rgbKey != null && (rgbKey.Length == 16 || rgbKey.Length == 24);
		}

		private static KeySizes[] s_legalBlockSizes = new KeySizes[]
		{
			new KeySizes(64, 64, 0)
		};

		private static KeySizes[] s_legalKeySizes = new KeySizes[]
		{
			new KeySizes(128, 192, 64)
		};
	}
}
