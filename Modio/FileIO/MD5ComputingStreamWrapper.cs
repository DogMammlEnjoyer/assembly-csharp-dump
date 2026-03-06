using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Modio.FileIO
{
	public class MD5ComputingStreamWrapper : Stream
	{
		public int TotalBytesRead { get; private set; }

		public MD5ComputingStreamWrapper(Stream baseStream)
		{
			this._baseStream = baseStream;
			this._md5 = MD5.Create();
		}

		public override void Flush()
		{
			this._baseStream.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._md5.Dispose();
				this._baseStream.Dispose();
			}
			base.Dispose(disposing);
		}

		public Task<string> GetMD5HashAsync()
		{
			MD5ComputingStreamWrapper.<GetMD5HashAsync>d__10 <GetMD5HashAsync>d__;
			<GetMD5HashAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<GetMD5HashAsync>d__.<>4__this = this;
			<GetMD5HashAsync>d__.<>1__state = -1;
			<GetMD5HashAsync>d__.<>t__builder.Start<MD5ComputingStreamWrapper.<GetMD5HashAsync>d__10>(ref <GetMD5HashAsync>d__);
			return <GetMD5HashAsync>d__.<>t__builder.Task;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			MD5ComputingStreamWrapper.<ReadAsync>d__11 <ReadAsync>d__;
			<ReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadAsync>d__.<>4__this = this;
			<ReadAsync>d__.buffer = buffer;
			<ReadAsync>d__.offset = offset;
			<ReadAsync>d__.count = count;
			<ReadAsync>d__.cancellationToken = cancellationToken;
			<ReadAsync>d__.<>1__state = -1;
			<ReadAsync>d__.<>t__builder.Start<MD5ComputingStreamWrapper.<ReadAsync>d__11>(ref <ReadAsync>d__);
			return <ReadAsync>d__.<>t__builder.Task;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this._baseStream.Read(buffer, offset, count);
			if (num != 0)
			{
				this._md5.TransformBlock(buffer, offset, num, null, 0);
			}
			else if (!this._hasTransformedFinalBlock)
			{
				this._hasTransformedFinalBlock = true;
				this._md5.TransformFinalBlock(buffer, 0, 0);
			}
			this.TotalBytesRead += num;
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this._baseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return this._baseStream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this._baseStream.Position;
			}
			set
			{
				this._baseStream.Position = value;
			}
		}

		private Stream _baseStream;

		private MD5 _md5;

		private bool _hasTransformedFinalBlock;
	}
}
