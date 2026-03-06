using System;
using System.IO;
using System.Security.Cryptography;

namespace Fusion.Encryption
{
	public class DataEncryptor : IDataEncryption, IDisposable
	{
		public void Setup(byte[] key)
		{
			Assert.Check(key.Length == 64, "key.Length == AesKeySize + HMACKeySize");
			this._aesKey = new byte[32];
			byte[] array = new byte[32];
			Buffer.BlockCopy(key, 0, this._aesKey, 0, 32);
			Buffer.BlockCopy(key, 32, array, 0, 32);
			this._cryptoProvider = DataEncryptor.BuildAesProvider(this._aesKey);
			this._hmacsha256 = DataEncryptor.BuildHMACSHA256(array);
			this._rng = RandomNumberGenerator.Create();
			this._encryptBufferEncrypt = new byte[4096];
			this._encryptBufferDecrypt = new byte[4096];
		}

		public byte[] GenerateKey()
		{
			byte[] array = new byte[64];
			using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
			{
				randomNumberGenerator.GetBytes(array);
			}
			return array;
		}

		public unsafe bool EncryptData(byte* buffer, ref int bufferLength, int capacity)
		{
			bool flag = buffer == null || bufferLength == 0 || capacity == 0;
			bool result;
			if (flag)
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn("Unable to encrypt data, invalid buffer");
				}
				result = false;
			}
			else
			{
				bool flag2 = this._cryptoProvider == null;
				if (flag2)
				{
					TraceLogStream logTraceEncryption2 = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption2 != null)
					{
						logTraceEncryption2.Warn("Encryption Provider was not initialized");
					}
					result = false;
				}
				else
				{
					byte[] bufferEncrypt = this.GetBufferEncrypt();
					bool flag3 = bufferEncrypt == null;
					if (flag3)
					{
						TraceLogStream logTraceEncryption3 = InternalLogStreams.LogTraceEncryption;
						if (logTraceEncryption3 != null)
						{
							logTraceEncryption3.Warn("Unable to allocate memory for encryption");
						}
						result = false;
					}
					else
					{
						int num;
						using (UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream(buffer, (long)bufferLength))
						{
							using (MemoryStream memoryStream = new MemoryStream(bufferEncrypt, true))
							{
								this._rng.GetBytes(this._ivEncryptBuffer);
								memoryStream.Write(this._ivEncryptBuffer, 0, this._ivEncryptBuffer.Length);
								Assert.Check(16L == memoryStream.Position, "IVSize == memoryStream.Position");
								using (ICryptoTransform cryptoTransform = this._cryptoProvider.CreateEncryptor(this._aesKey, this._ivEncryptBuffer))
								{
									using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
									{
										unmanagedMemoryStream.CopyTo(cryptoStream, bufferLength);
										cryptoStream.FlushFinalBlock();
										num = (int)memoryStream.Position;
									}
								}
							}
						}
						Assert.Check<int, int>(capacity >= num, "Unable to copy result, original buffer is too short. {0} vs {1}", capacity, num);
						byte[] array;
						byte* source;
						if ((array = bufferEncrypt) == null || array.Length == 0)
						{
							source = null;
						}
						else
						{
							source = &array[0];
						}
						Native.MemCpy((void*)buffer, (void*)source, num);
						array = null;
						bufferLength = num;
						result = true;
					}
				}
			}
			return result;
		}

		public unsafe bool DecryptData(byte* buffer, ref int bufferLength, int capacity)
		{
			bool flag = buffer == null || bufferLength == 0 || capacity == 0;
			bool result;
			if (flag)
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn("Unable to encrypt data, invalid buffer");
				}
				result = false;
			}
			else
			{
				bool flag2 = this._cryptoProvider == null;
				if (flag2)
				{
					TraceLogStream logTraceEncryption2 = InternalLogStreams.LogTraceEncryption;
					if (logTraceEncryption2 != null)
					{
						logTraceEncryption2.Warn("Encryption Provider was not initialized");
					}
					result = false;
				}
				else
				{
					byte[] bufferDecrypt = this.GetBufferDecrypt();
					bool flag3 = bufferDecrypt == null;
					if (flag3)
					{
						TraceLogStream logTraceEncryption3 = InternalLogStreams.LogTraceEncryption;
						if (logTraceEncryption3 != null)
						{
							logTraceEncryption3.Warn("Unable to allocate memory for encryption");
						}
						result = false;
					}
					else
					{
						int num2;
						using (UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream(buffer, (long)bufferLength))
						{
							int num = unmanagedMemoryStream.Read(this._ivDecryptBuffer, 0, 16);
							Assert.Check(num == 16, "read == IVSize");
							bufferLength -= 16;
							using (MemoryStream memoryStream = new MemoryStream(bufferDecrypt, true))
							{
								using (ICryptoTransform cryptoTransform = this._cryptoProvider.CreateDecryptor(this._aesKey, this._ivDecryptBuffer))
								{
									using (CryptoStream cryptoStream = new CryptoStream(unmanagedMemoryStream, cryptoTransform, CryptoStreamMode.Read))
									{
										cryptoStream.CopyTo(memoryStream, bufferLength);
										num2 = (int)memoryStream.Position;
									}
								}
							}
						}
						Assert.Check(capacity >= num2, "Unable to copy result, original buffer is too short");
						byte[] array;
						byte* source;
						if ((array = bufferDecrypt) == null || array.Length == 0)
						{
							source = null;
						}
						else
						{
							source = &array[0];
						}
						Native.MemCpy((void*)buffer, (void*)source, num2);
						array = null;
						bufferLength = num2;
						result = true;
					}
				}
			}
			return result;
		}

		public unsafe bool ComputeHash(byte* buffer, ref int bufferLength, int capacity)
		{
			bool flag = this._hmacsha256 == null;
			bool result;
			if (flag)
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn("Hasher was not initialized");
				}
				result = false;
			}
			else
			{
				this._hmacsha256.Initialize();
				using (UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream(buffer, (long)bufferLength))
				{
					byte[] array = this._hmacsha256.ComputeHash(unmanagedMemoryStream);
					Assert.Check(array.Length == 32, "hash.Length == HASHSize");
					Assert.Check(capacity >= bufferLength + 32, "Unable to copy hash, original buffer is too short");
					Native.CopyFromArray<byte>((void*)(buffer + bufferLength), array);
				}
				bufferLength += 32;
				result = true;
			}
			return result;
		}

		public unsafe bool VerifyHash(byte* buffer, ref int bufferLength, int capacity)
		{
			bool flag = this._hmacsha256 == null;
			bool result;
			if (flag)
			{
				TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
				if (logTraceEncryption != null)
				{
					logTraceEncryption.Warn("Hasher was not initialized");
				}
				result = false;
			}
			else
			{
				this._hmacsha256.Initialize();
				using (UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream(buffer, (long)(bufferLength - 32)))
				{
					byte[] array = this._hmacsha256.ComputeHash(unmanagedMemoryStream);
					bufferLength -= 32;
					try
					{
						byte[] array2;
						byte* ptr;
						if ((array2 = array) == null || array2.Length == 0)
						{
							ptr = null;
						}
						else
						{
							ptr = &array2[0];
						}
						result = (Native.MemCmp((void*)(buffer + bufferLength), (void*)ptr, 32) == 0);
					}
					finally
					{
						byte[] array2 = null;
					}
				}
			}
			return result;
		}

		private static Aes BuildAesProvider(byte[] key)
		{
			Aes aes = Aes.Create();
			aes.Key = key;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			return aes;
		}

		private static HMACSHA256 BuildHMACSHA256(byte[] key)
		{
			return new HMACSHA256(key);
		}

		private byte[] GetBufferEncrypt()
		{
			Array.Clear(this._encryptBufferEncrypt, 0, 4096);
			return this._encryptBufferEncrypt;
		}

		private byte[] GetBufferDecrypt()
		{
			Array.Clear(this._encryptBufferDecrypt, 0, 4096);
			return this._encryptBufferDecrypt;
		}

		public void Dispose()
		{
			TraceLogStream logTraceEncryption = InternalLogStreams.LogTraceEncryption;
			if (logTraceEncryption != null)
			{
				logTraceEncryption.Log("Disposing DataEncryptor...");
			}
			Aes cryptoProvider = this._cryptoProvider;
			if (cryptoProvider != null)
			{
				cryptoProvider.Dispose();
			}
			this._cryptoProvider = null;
			HMACSHA256 hmacsha = this._hmacsha256;
			if (hmacsha != null)
			{
				hmacsha.Dispose();
			}
			this._hmacsha256 = null;
			RandomNumberGenerator rng = this._rng;
			if (rng != null)
			{
				rng.Dispose();
			}
			this._rng = null;
			this._encryptBufferEncrypt = null;
			this._encryptBufferDecrypt = null;
		}

		private const int TempBufferLength = 4096;

		private const int AesKeySize = 32;

		private const int HMACKeySize = 32;

		private const int IVSize = 16;

		private const int HASHSize = 32;

		private Aes _cryptoProvider;

		private HMACSHA256 _hmacsha256;

		private RandomNumberGenerator _rng;

		private byte[] _encryptBufferEncrypt;

		private byte[] _encryptBufferDecrypt;

		private byte[] _aesKey;

		private readonly byte[] _ivEncryptBuffer = new byte[16];

		private readonly byte[] _ivDecryptBuffer = new byte[16];
	}
}
