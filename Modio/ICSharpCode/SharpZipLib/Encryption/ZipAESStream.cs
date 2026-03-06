using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace ICSharpCode.SharpZipLib.Encryption
{
	internal class ZipAESStream : CryptoStream
	{
		public ZipAESStream(Stream stream, ZipAESTransform transform, CryptoStreamMode mode) : base(stream, transform, mode)
		{
			this._stream = stream;
			this._transform = transform;
			this._slideBuffer = new byte[1024];
			if (mode != CryptoStreamMode.Read)
			{
				throw new Exception("ZipAESStream only for read");
			}
		}

		private bool HasBufferedData
		{
			get
			{
				return this._transformBuffer != null && this._transformBufferStartPos < this._transformBufferFreePos;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count == 0)
			{
				return 0;
			}
			int num = 0;
			if (this.HasBufferedData)
			{
				num = this.ReadBufferedData(buffer, offset, count);
				if (num == count)
				{
					return num;
				}
				offset += num;
				count -= num;
			}
			if (this._slideBuffer != null)
			{
				num += this.ReadAndTransform(buffer, offset, count);
			}
			return num;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			return Task.FromResult<int>(this.Read(buffer, offset, count));
		}

		private int ReadAndTransform(byte[] buffer, int offset, int count)
		{
			int i = 0;
			while (i < count)
			{
				int count2 = count - i;
				int num = this._slideBufFreePos - this._slideBufStartPos;
				int num2 = 26 - num;
				if (this._slideBuffer.Length - this._slideBufFreePos < num2)
				{
					int num3 = 0;
					int j = this._slideBufStartPos;
					while (j < this._slideBufFreePos)
					{
						this._slideBuffer[num3] = this._slideBuffer[j];
						j++;
						num3++;
					}
					this._slideBufFreePos -= this._slideBufStartPos;
					this._slideBufStartPos = 0;
				}
				int num4 = StreamUtils.ReadRequestedBytes(this._stream, this._slideBuffer, this._slideBufFreePos, num2);
				this._slideBufFreePos += num4;
				num = this._slideBufFreePos - this._slideBufStartPos;
				if (num < 26)
				{
					if (num > 10)
					{
						int blockSize = num - 10;
						i += this.TransformAndBufferBlock(buffer, offset, count2, blockSize);
					}
					else if (num < 10)
					{
						throw new ZipException("Internal error missed auth code");
					}
					byte[] authCode = this._transform.GetAuthCode();
					for (int k = 0; k < 10; k++)
					{
						if (authCode[k] != this._slideBuffer[this._slideBufStartPos + k])
						{
							throw new ZipException("AES Authentication Code does not match. This is a super-CRC check on the data in the file after compression and encryption. \r\nThe file may be damaged.");
						}
					}
					this._slideBuffer = null;
					break;
				}
				int num5 = this.TransformAndBufferBlock(buffer, offset, count2, 16);
				i += num5;
				offset += num5;
			}
			return i;
		}

		private int ReadBufferedData(byte[] buffer, int offset, int count)
		{
			int num = Math.Min(count, this._transformBufferFreePos - this._transformBufferStartPos);
			Array.Copy(this._transformBuffer, this._transformBufferStartPos, buffer, offset, num);
			this._transformBufferStartPos += num;
			return num;
		}

		private int TransformAndBufferBlock(byte[] buffer, int offset, int count, int blockSize)
		{
			bool flag = blockSize > count;
			if (flag && this._transformBuffer == null)
			{
				this._transformBuffer = new byte[16];
			}
			byte[] outputBuffer = flag ? this._transformBuffer : buffer;
			int outputOffset = flag ? 0 : offset;
			this._transform.TransformBlock(this._slideBuffer, this._slideBufStartPos, blockSize, outputBuffer, outputOffset);
			this._slideBufStartPos += blockSize;
			if (!flag)
			{
				return blockSize;
			}
			Array.Copy(this._transformBuffer, 0, buffer, offset, count);
			this._transformBufferStartPos = count;
			this._transformBufferFreePos = blockSize;
			return count;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		private const int AUTH_CODE_LENGTH = 10;

		private const int CRYPTO_BLOCK_SIZE = 16;

		private const int BLOCK_AND_AUTH = 26;

		private Stream _stream;

		private ZipAESTransform _transform;

		private byte[] _slideBuffer;

		private int _slideBufStartPos;

		private int _slideBufFreePos;

		private byte[] _transformBuffer;

		private int _transformBufferFreePos;

		private int _transformBufferStartPos;
	}
}
