using System;
using System.IO;

namespace WebSocketSharp.Net
{
	internal class RequestStream : Stream
	{
		internal RequestStream(Stream innerStream, byte[] initialBuffer, int offset, int count, long contentLength)
		{
			this._innerStream = innerStream;
			this._initialBuffer = initialBuffer;
			this._offset = offset;
			this._count = count;
			this._bodyLeft = contentLength;
		}

		internal int Count
		{
			get
			{
				return this._count;
			}
		}

		internal byte[] InitialBuffer
		{
			get
			{
				return this._initialBuffer;
			}
		}

		internal int Offset
		{
			get
			{
				return this._offset;
			}
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

		private int fillFromInitialBuffer(byte[] buffer, int offset, int count)
		{
			bool flag = this._bodyLeft == 0L;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = this._count == 0;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					bool flag3 = count > this._count;
					if (flag3)
					{
						count = this._count;
					}
					bool flag4 = this._bodyLeft > 0L && this._bodyLeft < (long)count;
					if (flag4)
					{
						count = (int)this._bodyLeft;
					}
					Buffer.BlockCopy(this._initialBuffer, this._offset, buffer, offset, count);
					this._offset += count;
					this._count -= count;
					bool flag5 = this._bodyLeft > 0L;
					if (flag5)
					{
						this._bodyLeft -= (long)count;
					}
					result = count;
				}
			}
			return result;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				string message = "A negative value.";
				throw new ArgumentOutOfRangeException("offset", message);
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				string message2 = "A negative value.";
				throw new ArgumentOutOfRangeException("count", message2);
			}
			int num = buffer.Length;
			bool flag4 = offset + count > num;
			if (flag4)
			{
				string message3 = "The sum of 'offset' and 'count' is greater than the length of 'buffer'.";
				throw new ArgumentException(message3);
			}
			bool flag5 = count == 0;
			IAsyncResult result;
			if (flag5)
			{
				result = this._innerStream.BeginRead(buffer, offset, 0, callback, state);
			}
			else
			{
				int num2 = this.fillFromInitialBuffer(buffer, offset, count);
				bool flag6 = num2 != 0;
				if (flag6)
				{
					HttpStreamAsyncResult httpStreamAsyncResult = new HttpStreamAsyncResult(callback, state);
					httpStreamAsyncResult.Buffer = buffer;
					httpStreamAsyncResult.Offset = offset;
					httpStreamAsyncResult.Count = count;
					httpStreamAsyncResult.SyncRead = ((num2 > 0) ? num2 : 0);
					httpStreamAsyncResult.Complete();
					result = httpStreamAsyncResult;
				}
				else
				{
					bool flag7 = this._bodyLeft > 0L && this._bodyLeft < (long)count;
					if (flag7)
					{
						count = (int)this._bodyLeft;
					}
					result = this._innerStream.BeginRead(buffer, offset, count, callback, state);
				}
			}
			return result;
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			this._disposed = true;
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			bool flag = asyncResult == null;
			if (flag)
			{
				throw new ArgumentNullException("asyncResult");
			}
			bool flag2 = asyncResult is HttpStreamAsyncResult;
			int result;
			if (flag2)
			{
				HttpStreamAsyncResult httpStreamAsyncResult = (HttpStreamAsyncResult)asyncResult;
				bool flag3 = !httpStreamAsyncResult.IsCompleted;
				if (flag3)
				{
					httpStreamAsyncResult.AsyncWaitHandle.WaitOne();
				}
				result = httpStreamAsyncResult.SyncRead;
			}
			else
			{
				int num = this._innerStream.EndRead(asyncResult);
				bool flag4 = num > 0 && this._bodyLeft > 0L;
				if (flag4)
				{
					this._bodyLeft -= (long)num;
				}
				result = num;
			}
			return result;
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				string message = "A negative value.";
				throw new ArgumentOutOfRangeException("offset", message);
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				string message2 = "A negative value.";
				throw new ArgumentOutOfRangeException("count", message2);
			}
			int num = buffer.Length;
			bool flag4 = offset + count > num;
			if (flag4)
			{
				string message3 = "The sum of 'offset' and 'count' is greater than the length of 'buffer'.";
				throw new ArgumentException(message3);
			}
			bool flag5 = count == 0;
			int result;
			if (flag5)
			{
				result = 0;
			}
			else
			{
				int num2 = this.fillFromInitialBuffer(buffer, offset, count);
				bool flag6 = num2 == -1;
				if (flag6)
				{
					result = 0;
				}
				else
				{
					bool flag7 = num2 > 0;
					if (flag7)
					{
						result = num2;
					}
					else
					{
						bool flag8 = this._bodyLeft > 0L && this._bodyLeft < (long)count;
						if (flag8)
						{
							count = (int)this._bodyLeft;
						}
						num2 = this._innerStream.Read(buffer, offset, count);
						bool flag9 = num2 > 0 && this._bodyLeft > 0L;
						if (flag9)
						{
							this._bodyLeft -= (long)num2;
						}
						result = num2;
					}
				}
			}
			return result;
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
			throw new NotSupportedException();
		}

		private long _bodyLeft;

		private int _count;

		private bool _disposed;

		private byte[] _initialBuffer;

		private Stream _innerStream;

		private int _offset;
	}
}
