using System;
using System.IO;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class ResponseStream : Stream
	{
		internal ResponseStream(Stream innerStream, HttpListenerResponse response, bool ignoreWriteExceptions)
		{
			this._innerStream = innerStream;
			this._response = response;
			if (ignoreWriteExceptions)
			{
				this._write = new Action<byte[], int, int>(this.writeWithoutThrowingException);
				this._writeChunked = new Action<byte[], int, int>(this.writeChunkedWithoutThrowingException);
			}
			else
			{
				this._write = new Action<byte[], int, int>(innerStream.Write);
				this._writeChunked = new Action<byte[], int, int>(this.writeChunked);
			}
			this._bodyBuffer = new MemoryStream();
		}

		public override bool CanRead
		{
			get
			{
				return false;
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
				return !this._disposed;
			}
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

		private bool flush(bool closing)
		{
			bool flag = !this._response.HeadersSent;
			if (flag)
			{
				bool flag2 = !this.flushHeaders();
				if (flag2)
				{
					return false;
				}
				this._response.HeadersSent = true;
				this._sendChunked = this._response.SendChunked;
				this._writeBody = (this._sendChunked ? this._writeChunked : this._write);
			}
			this.flushBody(closing);
			return true;
		}

		private void flushBody(bool closing)
		{
			using (this._bodyBuffer)
			{
				long length = this._bodyBuffer.Length;
				bool flag = length > 2147483647L;
				if (flag)
				{
					this._bodyBuffer.Position = 0L;
					int num = 1024;
					byte[] array = new byte[num];
					for (;;)
					{
						int num2 = this._bodyBuffer.Read(array, 0, num);
						bool flag2 = num2 <= 0;
						if (flag2)
						{
							break;
						}
						this._writeBody(array, 0, num2);
					}
				}
				else
				{
					bool flag3 = length > 0L;
					if (flag3)
					{
						this._writeBody(this._bodyBuffer.GetBuffer(), 0, (int)length);
					}
				}
			}
			bool flag4 = !closing;
			if (flag4)
			{
				this._bodyBuffer = new MemoryStream();
			}
			else
			{
				bool sendChunked = this._sendChunked;
				if (sendChunked)
				{
					this._write(ResponseStream._lastChunk, 0, 5);
				}
				this._bodyBuffer = null;
			}
		}

		private bool flushHeaders()
		{
			bool flag = !this._response.SendChunked;
			if (flag)
			{
				bool flag2 = this._response.ContentLength64 != this._bodyBuffer.Length;
				if (flag2)
				{
					return false;
				}
			}
			string statusLine = this._response.StatusLine;
			WebHeaderCollection fullHeaders = this._response.FullHeaders;
			MemoryStream memoryStream = new MemoryStream();
			Encoding utf = Encoding.UTF8;
			using (StreamWriter streamWriter = new StreamWriter(memoryStream, utf, 256))
			{
				streamWriter.Write(statusLine);
				streamWriter.Write(fullHeaders.ToStringMultiValue(true));
				streamWriter.Flush();
				int num = utf.GetPreamble().Length;
				long num2 = memoryStream.Length - (long)num;
				bool flag3 = num2 > (long)ResponseStream._maxHeadersLength;
				if (flag3)
				{
					return false;
				}
				this._write(memoryStream.GetBuffer(), num, (int)num2);
			}
			this._response.CloseConnection = (fullHeaders["Connection"] == "close");
			return true;
		}

		private static byte[] getChunkSizeBytes(int size)
		{
			string s = string.Format("{0:x}\r\n", size);
			return Encoding.ASCII.GetBytes(s);
		}

		private void writeChunked(byte[] buffer, int offset, int count)
		{
			byte[] chunkSizeBytes = ResponseStream.getChunkSizeBytes(count);
			this._innerStream.Write(chunkSizeBytes, 0, chunkSizeBytes.Length);
			this._innerStream.Write(buffer, offset, count);
			this._innerStream.Write(ResponseStream._crlf, 0, 2);
		}

		private void writeChunkedWithoutThrowingException(byte[] buffer, int offset, int count)
		{
			try
			{
				this.writeChunked(buffer, offset, count);
			}
			catch
			{
			}
		}

		private void writeWithoutThrowingException(byte[] buffer, int offset, int count)
		{
			try
			{
				this._innerStream.Write(buffer, offset, count);
			}
			catch
			{
			}
		}

		internal void Close(bool force)
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				this._disposed = true;
				bool flag = !force;
				if (flag)
				{
					bool flag2 = this.flush(true);
					if (flag2)
					{
						this._response.Close();
						this._response = null;
						this._innerStream = null;
						return;
					}
					this._response.CloseConnection = true;
				}
				bool sendChunked = this._sendChunked;
				if (sendChunked)
				{
					this._write(ResponseStream._lastChunk, 0, 5);
				}
				this._bodyBuffer.Dispose();
				this._response.Abort();
				this._bodyBuffer = null;
				this._response = null;
				this._innerStream = null;
			}
		}

		internal void InternalWrite(byte[] buffer, int offset, int count)
		{
			this._write(buffer, offset, count);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			return this._bodyBuffer.BeginWrite(buffer, offset, count, callback, state);
		}

		public override void Close()
		{
			this.Close(false);
		}

		protected override void Dispose(bool disposing)
		{
			this.Close(!disposing);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			this._bodyBuffer.EndWrite(asyncResult);
		}

		public override void Flush()
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				bool flag = this._sendChunked || this._response.SendChunked;
				bool flag2 = !flag;
				if (!flag2)
				{
					this.flush(false);
				}
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			this._bodyBuffer.Write(buffer, offset, count);
		}

		private MemoryStream _bodyBuffer;

		private static readonly byte[] _crlf = new byte[]
		{
			13,
			10
		};

		private bool _disposed;

		private Stream _innerStream;

		private static readonly byte[] _lastChunk = new byte[]
		{
			48,
			13,
			10,
			13,
			10
		};

		private static readonly int _maxHeadersLength = 32768;

		private HttpListenerResponse _response;

		private bool _sendChunked;

		private Action<byte[], int, int> _write;

		private Action<byte[], int, int> _writeBody;

		private Action<byte[], int, int> _writeChunked;
	}
}
