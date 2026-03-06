using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal abstract class WebReadStream : Stream
	{
		public WebOperation Operation { get; }

		protected Stream InnerStream { get; }

		internal string ME
		{
			get
			{
				return null;
			}
		}

		public WebReadStream(WebOperation operation, Stream innerStream)
		{
			this.Operation = operation;
			this.InnerStream = innerStream;
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
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

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		protected Exception GetException(Exception e)
		{
			e = HttpWebRequest.FlattenException(e);
			if (e is WebException)
			{
				return e;
			}
			if (this.Operation.Aborted || e is OperationCanceledException || e is ObjectDisposedException)
			{
				return HttpWebRequest.CreateRequestAbortedException();
			}
			return e;
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("The stream does not support reading.");
			}
			this.Operation.ThrowIfClosedOrDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || num < offset)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || num - offset < size)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			int result;
			try
			{
				result = this.ReadAsync(buffer, offset, size, CancellationToken.None).Result;
			}
			catch (Exception e)
			{
				throw this.GetException(e);
			}
			return result;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("The stream does not support reading.");
			}
			this.Operation.ThrowIfClosedOrDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || num < offset)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || num - offset < size)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return TaskToApm.Begin(this.ReadAsync(buffer, offset, size, CancellationToken.None), cb, state);
		}

		public override int EndRead(IAsyncResult r)
		{
			if (r == null)
			{
				throw new ArgumentNullException("r");
			}
			int result;
			try
			{
				result = TaskToApm.End<int>(r);
			}
			catch (Exception e)
			{
				throw this.GetException(e);
			}
			return result;
		}

		public sealed override Task<int> ReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			WebReadStream.<ReadAsync>d__28 <ReadAsync>d__;
			<ReadAsync>d__.<>4__this = this;
			<ReadAsync>d__.buffer = buffer;
			<ReadAsync>d__.offset = offset;
			<ReadAsync>d__.size = size;
			<ReadAsync>d__.cancellationToken = cancellationToken;
			<ReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadAsync>d__.<>1__state = -1;
			<ReadAsync>d__.<>t__builder.Start<WebReadStream.<ReadAsync>d__28>(ref <ReadAsync>d__);
			return <ReadAsync>d__.<>t__builder.Task;
		}

		protected abstract Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken);

		internal virtual Task FinishReading(CancellationToken cancellationToken)
		{
			this.Operation.ThrowIfDisposed(cancellationToken);
			WebReadStream webReadStream = this.InnerStream as WebReadStream;
			if (webReadStream != null)
			{
				return webReadStream.FinishReading(cancellationToken);
			}
			return Task.CompletedTask;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (this.InnerStream != null)
				{
					this.InnerStream.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private bool disposed;
	}
}
