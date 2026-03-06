using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
	internal sealed class SslOverTdsStream : Stream
	{
		public SslOverTdsStream(Stream stream)
		{
			this._stream = stream;
			this._encapsulate = true;
		}

		public void FinishHandshake()
		{
			this._encapsulate = false;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.ReadInternal(buffer, offset, count, CancellationToken.None, false).GetAwaiter().GetResult();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.WriteInternal(buffer, offset, count, CancellationToken.None, false).Wait();
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
		{
			return this.WriteInternal(buffer, offset, count, token, true);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
		{
			return this.ReadInternal(buffer, offset, count, token, true);
		}

		private Task<int> ReadInternal(byte[] buffer, int offset, int count, CancellationToken token, bool async)
		{
			SslOverTdsStream.<ReadInternal>d__11 <ReadInternal>d__;
			<ReadInternal>d__.<>4__this = this;
			<ReadInternal>d__.buffer = buffer;
			<ReadInternal>d__.offset = offset;
			<ReadInternal>d__.count = count;
			<ReadInternal>d__.token = token;
			<ReadInternal>d__.async = async;
			<ReadInternal>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadInternal>d__.<>1__state = -1;
			<ReadInternal>d__.<>t__builder.Start<SslOverTdsStream.<ReadInternal>d__11>(ref <ReadInternal>d__);
			return <ReadInternal>d__.<>t__builder.Task;
		}

		private Task WriteInternal(byte[] buffer, int offset, int count, CancellationToken token, bool async)
		{
			SslOverTdsStream.<WriteInternal>d__12 <WriteInternal>d__;
			<WriteInternal>d__.<>4__this = this;
			<WriteInternal>d__.buffer = buffer;
			<WriteInternal>d__.offset = offset;
			<WriteInternal>d__.count = count;
			<WriteInternal>d__.token = token;
			<WriteInternal>d__.async = async;
			<WriteInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteInternal>d__.<>1__state = -1;
			<WriteInternal>d__.<>t__builder.Start<SslOverTdsStream.<WriteInternal>d__12>(ref <WriteInternal>d__);
			return <WriteInternal>d__.<>t__builder.Task;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			if (!(this._stream is PipeStream))
			{
				this._stream.Flush();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override bool CanRead
		{
			get
			{
				return this._stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._stream.CanWrite;
			}
		}

		public override bool CanSeek
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
				throw new NotSupportedException();
			}
		}

		private readonly Stream _stream;

		private int _packetBytes;

		private bool _encapsulate;

		private const int PACKET_SIZE_WITHOUT_HEADER = 4088;

		private const int PRELOGIN_PACKET_TYPE = 18;
	}
}
