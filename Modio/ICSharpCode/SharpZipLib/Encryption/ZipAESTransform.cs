using System;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Core;

namespace ICSharpCode.SharpZipLib.Encryption
{
	internal class ZipAESTransform : ICryptoTransform, IDisposable
	{
		public ZipAESTransform(string key, byte[] saltBytes, int blockSize, bool writeMode)
		{
			if (blockSize != 16 && blockSize != 32)
			{
				throw new Exception("Invalid blocksize " + blockSize.ToString() + ". Must be 16 or 32.");
			}
			if (saltBytes.Length != blockSize / 2)
			{
				throw new Exception("Invalid salt len. Must be " + (blockSize / 2).ToString() + " for blocksize " + blockSize.ToString());
			}
			this._blockSize = blockSize;
			this._encryptBuffer = new byte[this._blockSize];
			this._encrPos = 16;
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, saltBytes, 1000);
			Aes aes = Aes.Create();
			aes.Mode = CipherMode.ECB;
			this._counterNonce = new byte[this._blockSize];
			byte[] bytes = rfc2898DeriveBytes.GetBytes(this._blockSize);
			byte[] bytes2 = rfc2898DeriveBytes.GetBytes(this._blockSize);
			this._encryptor = aes.CreateEncryptor(bytes, new byte[16]);
			this._pwdVerifier = rfc2898DeriveBytes.GetBytes(2);
			this._hmacsha1 = IncrementalHash.CreateHMAC(HashAlgorithmName.SHA1, bytes2);
			this._writeMode = writeMode;
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (!this._writeMode)
			{
				this._hmacsha1.AppendData(inputBuffer, inputOffset, inputCount);
			}
			for (int i = 0; i < inputCount; i++)
			{
				if (this._encrPos == 16)
				{
					int num = 0;
					for (;;)
					{
						byte[] counterNonce = this._counterNonce;
						int num2 = num;
						byte b = counterNonce[num2] + 1;
						counterNonce[num2] = b;
						if (b != 0)
						{
							break;
						}
						num++;
					}
					this._encryptor.TransformBlock(this._counterNonce, 0, this._blockSize, this._encryptBuffer, 0);
					this._encrPos = 0;
				}
				int num3 = i + outputOffset;
				byte b2 = inputBuffer[i + inputOffset];
				byte[] encryptBuffer = this._encryptBuffer;
				int encrPos = this._encrPos;
				this._encrPos = encrPos + 1;
				outputBuffer[num3] = (b2 ^ encryptBuffer[encrPos]);
			}
			if (this._writeMode)
			{
				this._hmacsha1.AppendData(outputBuffer, outputOffset, inputCount);
			}
			return inputCount;
		}

		public byte[] PwdVerifier
		{
			get
			{
				return this._pwdVerifier;
			}
		}

		public byte[] GetAuthCode()
		{
			if (this._authCode == null)
			{
				this._authCode = this._hmacsha1.GetHashAndReset();
			}
			return this._authCode;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputCount > 0)
			{
				throw new NotImplementedException("TransformFinalBlock is not implemented and inputCount is greater than 0");
			}
			return Empty.Array<byte>();
		}

		public int InputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		public void Dispose()
		{
			this._encryptor.Dispose();
		}

		private const int PWD_VER_LENGTH = 2;

		private const int KEY_ROUNDS = 1000;

		private const int ENCRYPT_BLOCK = 16;

		private int _blockSize;

		private readonly ICryptoTransform _encryptor;

		private readonly byte[] _counterNonce;

		private byte[] _encryptBuffer;

		private int _encrPos;

		private byte[] _pwdVerifier;

		private IncrementalHash _hmacsha1;

		private byte[] _authCode;

		private bool _writeMode;
	}
}
