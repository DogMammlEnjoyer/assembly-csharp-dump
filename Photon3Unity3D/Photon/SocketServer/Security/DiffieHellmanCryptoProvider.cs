using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Photon.SocketServer.Security
{
	internal class DiffieHellmanCryptoProvider : ICryptoProvider, IDisposable
	{
		public DiffieHellmanCryptoProvider()
		{
			this.prime = new BigInteger(OakleyGroups.OakleyPrime768);
			this.secret = this.GenerateRandomSecret(160);
			this.publicKey = this.CalculatePublicKey();
		}

		public DiffieHellmanCryptoProvider(byte[] cryptoKey)
		{
			this.crypto = new RijndaelManaged();
			this.crypto.Key = cryptoKey;
			this.crypto.IV = new byte[16];
			this.crypto.Padding = PaddingMode.PKCS7;
		}

		public bool IsInitialized
		{
			get
			{
				return this.crypto != null;
			}
		}

		public byte[] PublicKey
		{
			get
			{
				return this.MsBigIntArrayToPhotonBigIntArray(this.publicKey.ToByteArray());
			}
		}

		public void DeriveSharedKey(byte[] otherPartyPublicKey)
		{
			otherPartyPublicKey = this.PhotonBigIntArrayToMsBigIntArray(otherPartyPublicKey);
			BigInteger otherPartyPublicKey2 = new BigInteger(otherPartyPublicKey);
			this.sharedKey = this.MsBigIntArrayToPhotonBigIntArray(this.CalculateSharedKey(otherPartyPublicKey2).ToByteArray());
			byte[] key;
			using (SHA256 sha = new SHA256Managed())
			{
				key = sha.ComputeHash(this.sharedKey);
			}
			this.crypto = new RijndaelManaged();
			this.crypto.Key = key;
			this.crypto.IV = new byte[16];
			this.crypto.Padding = PaddingMode.PKCS7;
		}

		private byte[] PhotonBigIntArrayToMsBigIntArray(byte[] array)
		{
			Array.Reverse(array);
			bool flag = (array[array.Length - 1] & 128) == 128;
			byte[] result;
			if (flag)
			{
				byte[] array2 = new byte[array.Length + 1];
				Buffer.BlockCopy(array, 0, array2, 0, array.Length);
				result = array2;
			}
			else
			{
				result = array;
			}
			return result;
		}

		private byte[] MsBigIntArrayToPhotonBigIntArray(byte[] array)
		{
			Array.Reverse(array);
			bool flag = array[0] == 0;
			byte[] result;
			if (flag)
			{
				byte[] array2 = new byte[array.Length - 1];
				Buffer.BlockCopy(array, 1, array2, 0, array.Length - 1);
				result = array2;
			}
			else
			{
				result = array;
			}
			return result;
		}

		public byte[] Encrypt(byte[] data)
		{
			return this.Encrypt(data, 0, data.Length);
		}

		public byte[] Encrypt(byte[] data, int offset, int count)
		{
			byte[] result;
			using (ICryptoTransform cryptoTransform = this.crypto.CreateEncryptor())
			{
				result = cryptoTransform.TransformFinalBlock(data, offset, count);
			}
			return result;
		}

		public byte[] Decrypt(byte[] data)
		{
			return this.Decrypt(data, 0, data.Length);
		}

		public byte[] Decrypt(byte[] data, int offset, int count)
		{
			byte[] result;
			using (ICryptoTransform cryptoTransform = this.crypto.CreateDecryptor())
			{
				result = cryptoTransform.TransformFinalBlock(data, offset, count);
			}
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool flag = !disposing;
			if (flag)
			{
			}
		}

		private BigInteger CalculatePublicKey()
		{
			return BigInteger.ModPow(DiffieHellmanCryptoProvider.primeRoot, this.secret, this.prime);
		}

		private BigInteger CalculateSharedKey(BigInteger otherPartyPublicKey)
		{
			return BigInteger.ModPow(otherPartyPublicKey, this.secret, this.prime);
		}

		private BigInteger GenerateRandomSecret(int secretLength)
		{
			RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider();
			byte[] array = new byte[secretLength / 8];
			BigInteger bigInteger;
			do
			{
				rngcryptoServiceProvider.GetBytes(array);
				bigInteger = new BigInteger(array);
			}
			while (bigInteger >= this.prime - 1 || bigInteger < 2L);
			return bigInteger;
		}

		private static readonly BigInteger primeRoot = new BigInteger(OakleyGroups.Generator);

		private readonly BigInteger prime;

		private readonly BigInteger secret;

		private readonly BigInteger publicKey;

		private Rijndael crypto;

		private byte[] sharedKey;
	}
}
