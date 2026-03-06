using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class WebRequestStream : WebConnectionStream
	{
		public WebRequestStream(WebConnection connection, WebOperation operation, Stream stream, WebConnectionTunnel tunnel) : base(connection, operation)
		{
			this.InnerStream = stream;
			this.allowBuffering = operation.Request.InternalAllowBuffering;
			this.sendChunked = (operation.Request.SendChunked && operation.WriteBuffer == null);
			if (!this.sendChunked && this.allowBuffering && operation.WriteBuffer == null)
			{
				this.writeBuffer = new MemoryStream();
			}
			this.KeepAlive = base.Request.KeepAlive;
			if (((tunnel != null) ? tunnel.ProxyVersion : null) != null && ((tunnel != null) ? tunnel.ProxyVersion : null) != HttpVersion.Version11)
			{
				this.KeepAlive = 0;
			}
		}

		internal Stream InnerStream { get; }

		public bool KeepAlive { get; }

		public override bool CanRead
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
				return true;
			}
		}

		internal bool SendChunked
		{
			get
			{
				return this.sendChunked;
			}
			set
			{
				this.sendChunked = value;
			}
		}

		internal bool HasWriteBuffer
		{
			get
			{
				return base.Operation.WriteBuffer != null || this.writeBuffer != null;
			}
		}

		internal int WriteBufferLength
		{
			get
			{
				if (base.Operation.WriteBuffer != null)
				{
					return base.Operation.WriteBuffer.Size;
				}
				if (this.writeBuffer != null)
				{
					return (int)this.writeBuffer.Length;
				}
				return -1;
			}
		}

		internal BufferOffsetSize GetWriteBuffer()
		{
			if (base.Operation.WriteBuffer != null)
			{
				return base.Operation.WriteBuffer;
			}
			if (this.writeBuffer == null || this.writeBuffer.Length == 0L)
			{
				return null;
			}
			return new BufferOffsetSize(this.writeBuffer.GetBuffer(), 0, (int)this.writeBuffer.Length, false);
		}

		private Task FinishWriting(CancellationToken cancellationToken)
		{
			WebRequestStream.<FinishWriting>d__31 <FinishWriting>d__;
			<FinishWriting>d__.<>4__this = this;
			<FinishWriting>d__.cancellationToken = cancellationToken;
			<FinishWriting>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishWriting>d__.<>1__state = -1;
			<FinishWriting>d__.<>t__builder.Start<WebRequestStream.<FinishWriting>d__31>(ref <FinishWriting>d__);
			return <FinishWriting>d__.<>t__builder.Task;
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || num < offset)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || num - offset < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			base.Operation.ThrowIfClosedOrDisposed(cancellationToken);
			if (base.Operation.WriteBuffer != null)
			{
				throw new InvalidOperationException();
			}
			WebCompletionSource webCompletionSource = new WebCompletionSource();
			if (Interlocked.CompareExchange<WebCompletionSource>(ref this.pendingWrite, webCompletionSource, null) != null)
			{
				throw new InvalidOperationException(SR.GetString("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress."));
			}
			return this.WriteAsyncInner(buffer, offset, count, webCompletionSource, cancellationToken);
		}

		private Task WriteAsyncInner(byte[] buffer, int offset, int size, WebCompletionSource completion, CancellationToken cancellationToken)
		{
			WebRequestStream.<WriteAsyncInner>d__33 <WriteAsyncInner>d__;
			<WriteAsyncInner>d__.<>4__this = this;
			<WriteAsyncInner>d__.buffer = buffer;
			<WriteAsyncInner>d__.offset = offset;
			<WriteAsyncInner>d__.size = size;
			<WriteAsyncInner>d__.completion = completion;
			<WriteAsyncInner>d__.cancellationToken = cancellationToken;
			<WriteAsyncInner>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteAsyncInner>d__.<>1__state = -1;
			<WriteAsyncInner>d__.<>t__builder.Start<WebRequestStream.<WriteAsyncInner>d__33>(ref <WriteAsyncInner>d__);
			return <WriteAsyncInner>d__.<>t__builder.Task;
		}

		private Task ProcessWrite(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			WebRequestStream.<ProcessWrite>d__34 <ProcessWrite>d__;
			<ProcessWrite>d__.<>4__this = this;
			<ProcessWrite>d__.buffer = buffer;
			<ProcessWrite>d__.offset = offset;
			<ProcessWrite>d__.size = size;
			<ProcessWrite>d__.cancellationToken = cancellationToken;
			<ProcessWrite>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessWrite>d__.<>1__state = -1;
			<ProcessWrite>d__.<>t__builder.Start<WebRequestStream.<ProcessWrite>d__34>(ref <ProcessWrite>d__);
			return <ProcessWrite>d__.<>t__builder.Task;
		}

		private void CheckWriteOverflow(long contentLength, long totalWritten, long size)
		{
			if (contentLength == -1L)
			{
				return;
			}
			long num = contentLength - totalWritten;
			if (size > num)
			{
				this.KillBuffer();
				this.closed = true;
				ProtocolViolationException ex = new ProtocolViolationException("The number of bytes to be written is greater than the specified ContentLength.");
				base.Operation.CompleteRequestWritten(this, ex);
				throw ex;
			}
		}

		internal Task Initialize(CancellationToken cancellationToken)
		{
			WebRequestStream.<Initialize>d__36 <Initialize>d__;
			<Initialize>d__.<>4__this = this;
			<Initialize>d__.cancellationToken = cancellationToken;
			<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Initialize>d__.<>1__state = -1;
			<Initialize>d__.<>t__builder.Start<WebRequestStream.<Initialize>d__36>(ref <Initialize>d__);
			return <Initialize>d__.<>t__builder.Task;
		}

		private Task SetHeadersAsync(bool setInternalLength, CancellationToken cancellationToken)
		{
			WebRequestStream.<SetHeadersAsync>d__37 <SetHeadersAsync>d__;
			<SetHeadersAsync>d__.<>4__this = this;
			<SetHeadersAsync>d__.setInternalLength = setInternalLength;
			<SetHeadersAsync>d__.cancellationToken = cancellationToken;
			<SetHeadersAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetHeadersAsync>d__.<>1__state = -1;
			<SetHeadersAsync>d__.<>t__builder.Start<WebRequestStream.<SetHeadersAsync>d__37>(ref <SetHeadersAsync>d__);
			return <SetHeadersAsync>d__.<>t__builder.Task;
		}

		internal Task WriteRequestAsync(CancellationToken cancellationToken)
		{
			WebRequestStream.<WriteRequestAsync>d__38 <WriteRequestAsync>d__;
			<WriteRequestAsync>d__.<>4__this = this;
			<WriteRequestAsync>d__.cancellationToken = cancellationToken;
			<WriteRequestAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteRequestAsync>d__.<>1__state = -1;
			<WriteRequestAsync>d__.<>t__builder.Start<WebRequestStream.<WriteRequestAsync>d__38>(ref <WriteRequestAsync>d__);
			return <WriteRequestAsync>d__.<>t__builder.Task;
		}

		private Task WriteChunkTrailer_inner(CancellationToken cancellationToken)
		{
			WebRequestStream.<WriteChunkTrailer_inner>d__39 <WriteChunkTrailer_inner>d__;
			<WriteChunkTrailer_inner>d__.<>4__this = this;
			<WriteChunkTrailer_inner>d__.cancellationToken = cancellationToken;
			<WriteChunkTrailer_inner>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteChunkTrailer_inner>d__.<>1__state = -1;
			<WriteChunkTrailer_inner>d__.<>t__builder.Start<WebRequestStream.<WriteChunkTrailer_inner>d__39>(ref <WriteChunkTrailer_inner>d__);
			return <WriteChunkTrailer_inner>d__.<>t__builder.Task;
		}

		private Task WriteChunkTrailer()
		{
			WebRequestStream.<WriteChunkTrailer>d__40 <WriteChunkTrailer>d__;
			<WriteChunkTrailer>d__.<>4__this = this;
			<WriteChunkTrailer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteChunkTrailer>d__.<>1__state = -1;
			<WriteChunkTrailer>d__.<>t__builder.Start<WebRequestStream.<WriteChunkTrailer>d__40>(ref <WriteChunkTrailer>d__);
			return <WriteChunkTrailer>d__.<>t__builder.Task;
		}

		internal void KillBuffer()
		{
			this.writeBuffer = null;
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			return Task.FromException<int>(new NotSupportedException("The stream does not support reading."));
		}

		protected override bool TryReadFromBufferedContent(byte[] buffer, int offset, int count, out int result)
		{
			throw new InvalidOperationException();
		}

		protected override void Close_internal(ref bool disposed)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (this.sendChunked)
			{
				this.WriteChunkTrailer().Wait();
				return;
			}
			if (!this.allowBuffering || this.requestWritten)
			{
				base.Operation.CompleteRequestWritten(this, null);
				return;
			}
			long contentLength = base.Request.ContentLength;
			if (!this.sendChunked && !base.Operation.IsNtlmChallenge && contentLength != -1L && this.totalWritten != contentLength)
			{
				IOException innerException = new IOException("Cannot close the stream until all bytes are written");
				this.closed = true;
				disposed = true;
				WebException ex = new WebException("Request was cancelled.", WebExceptionStatus.RequestCanceled, WebExceptionInternalStatus.RequestFatal, innerException);
				base.Operation.CompleteRequestWritten(this, ex);
				throw ex;
			}
			disposed = true;
			base.Operation.CompleteRequestWritten(this, null);
		}

		private static byte[] crlf = new byte[]
		{
			13,
			10
		};

		private MemoryStream writeBuffer;

		private bool requestWritten;

		private bool allowBuffering;

		private bool sendChunked;

		private WebCompletionSource pendingWrite;

		private long totalWritten;

		private byte[] headers;

		private bool headersSent;

		private int completeRequestWritten;

		private int chunkTrailerWritten;

		internal readonly string ME;
	}
}
