using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	/// <summary>Adds a buffering layer to read and write operations on another stream. This class cannot be inherited.</summary>
	public sealed class BufferedStream : Stream
	{
		internal SemaphoreSlim LazyEnsureAsyncActiveSemaphoreInitialized()
		{
			return LazyInitializer.EnsureInitialized<SemaphoreSlim>(ref this._asyncActiveSemaphore, () => new SemaphoreSlim(1, 1));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.BufferedStream" /> class with a default buffer size of 4096 bytes.</summary>
		/// <param name="stream">The current stream.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		public BufferedStream(Stream stream) : this(stream, 4096)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.BufferedStream" /> class with the specified buffer size.</summary>
		/// <param name="stream">The current stream.</param>
		/// <param name="bufferSize">The buffer size in bytes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="bufferSize" /> is negative.</exception>
		public BufferedStream(Stream stream, int bufferSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", SR.Format("'{0}' must be greater than zero.", "bufferSize"));
			}
			this._stream = stream;
			this._bufferSize = bufferSize;
			if (!this._stream.CanRead && !this._stream.CanWrite)
			{
				throw new ObjectDisposedException(null, "Cannot access a closed Stream.");
			}
		}

		private void EnsureNotClosed()
		{
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot access a closed Stream.");
			}
		}

		private void EnsureCanSeek()
		{
			if (!this._stream.CanSeek)
			{
				throw new NotSupportedException("Stream does not support seeking.");
			}
		}

		private void EnsureCanRead()
		{
			if (!this._stream.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading.");
			}
		}

		private void EnsureCanWrite()
		{
			if (!this._stream.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
		}

		private void EnsureShadowBufferAllocated()
		{
			if (this._buffer.Length != this._bufferSize || this._bufferSize >= 81920)
			{
				return;
			}
			byte[] array = new byte[Math.Min(this._bufferSize + this._bufferSize, 81920)];
			Buffer.BlockCopy(this._buffer, 0, array, 0, this._writePos);
			this._buffer = array;
		}

		private void EnsureBufferAllocated()
		{
			if (this._buffer == null)
			{
				this._buffer = new byte[this._bufferSize];
			}
		}

		public Stream UnderlyingStream
		{
			get
			{
				return this._stream;
			}
		}

		public int BufferSize
		{
			get
			{
				return this._bufferSize;
			}
		}

		/// <summary>Gets a value indicating whether the current stream supports reading.</summary>
		/// <returns>
		///   <see langword="true" /> if the stream supports reading; <see langword="false" /> if the stream is closed or was opened with write-only access.</returns>
		public override bool CanRead
		{
			get
			{
				return this._stream != null && this._stream.CanRead;
			}
		}

		/// <summary>Gets a value indicating whether the current stream supports writing.</summary>
		/// <returns>
		///   <see langword="true" /> if the stream supports writing; <see langword="false" /> if the stream is closed or was opened with read-only access.</returns>
		public override bool CanWrite
		{
			get
			{
				return this._stream != null && this._stream.CanWrite;
			}
		}

		/// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
		/// <returns>
		///   <see langword="true" /> if the stream supports seeking; <see langword="false" /> if the stream is closed or if the stream was constructed from an operating system handle such as a pipe or output to the console.</returns>
		public override bool CanSeek
		{
			get
			{
				return this._stream != null && this._stream.CanSeek;
			}
		}

		/// <summary>Gets the stream length in bytes.</summary>
		/// <returns>The stream length in bytes.</returns>
		/// <exception cref="T:System.IO.IOException">The underlying stream is <see langword="null" /> or closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override long Length
		{
			get
			{
				this.EnsureNotClosed();
				if (this._writePos > 0)
				{
					this.FlushWrite();
				}
				return this._stream.Length;
			}
		}

		/// <summary>Gets the position within the current stream.</summary>
		/// <returns>The position within the current stream.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value passed to <see cref="M:System.IO.BufferedStream.Seek(System.Int64,System.IO.SeekOrigin)" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs, such as the stream being closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override long Position
		{
			get
			{
				this.EnsureNotClosed();
				this.EnsureCanSeek();
				return this._stream.Position + (long)(this._readPos - this._readLen + this._writePos);
			}
			set
			{
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", "Non-negative number required.");
				}
				this.EnsureNotClosed();
				this.EnsureCanSeek();
				if (this._writePos > 0)
				{
					this.FlushWrite();
				}
				this._readPos = 0;
				this._readLen = 0;
				this._stream.Seek(value, SeekOrigin.Begin);
			}
		}

		public override ValueTask DisposeAsync()
		{
			BufferedStream.<DisposeAsync>d__34 <DisposeAsync>d__;
			<DisposeAsync>d__.<>4__this = this;
			<DisposeAsync>d__.<>t__builder = AsyncValueTaskMethodBuilder.Create();
			<DisposeAsync>d__.<>1__state = -1;
			<DisposeAsync>d__.<>t__builder.Start<BufferedStream.<DisposeAsync>d__34>(ref <DisposeAsync>d__);
			return <DisposeAsync>d__.<>t__builder.Task;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._stream != null)
				{
					try
					{
						this.Flush();
					}
					finally
					{
						this._stream.Dispose();
					}
				}
			}
			finally
			{
				this._stream = null;
				this._buffer = null;
				base.Dispose(disposing);
			}
		}

		/// <summary>Clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.IO.IOException">The data source or repository is not open.</exception>
		public override void Flush()
		{
			this.EnsureNotClosed();
			if (this._writePos > 0)
			{
				this.FlushWrite();
				return;
			}
			if (this._readPos < this._readLen)
			{
				if (this._stream.CanSeek)
				{
					this.FlushRead();
				}
				if (this._stream.CanWrite)
				{
					this._stream.Flush();
				}
				return;
			}
			if (this._stream.CanWrite)
			{
				this._stream.Flush();
			}
			this._writePos = (this._readPos = (this._readLen = 0));
		}

		/// <summary>Asynchronously clears all buffers for this stream, causes any buffered data to be written to the underlying device, and monitors cancellation requests.</summary>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous flush operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			this.EnsureNotClosed();
			return this.FlushAsyncInternal(cancellationToken);
		}

		private Task FlushAsyncInternal(CancellationToken cancellationToken)
		{
			BufferedStream.<FlushAsyncInternal>d__38 <FlushAsyncInternal>d__;
			<FlushAsyncInternal>d__.<>4__this = this;
			<FlushAsyncInternal>d__.cancellationToken = cancellationToken;
			<FlushAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FlushAsyncInternal>d__.<>1__state = -1;
			<FlushAsyncInternal>d__.<>t__builder.Start<BufferedStream.<FlushAsyncInternal>d__38>(ref <FlushAsyncInternal>d__);
			return <FlushAsyncInternal>d__.<>t__builder.Task;
		}

		private void FlushRead()
		{
			if (this._readPos - this._readLen != 0)
			{
				this._stream.Seek((long)(this._readPos - this._readLen), SeekOrigin.Current);
			}
			this._readPos = 0;
			this._readLen = 0;
		}

		private void ClearReadBufferBeforeWrite()
		{
			if (this._readPos == this._readLen)
			{
				this._readPos = (this._readLen = 0);
				return;
			}
			if (!this._stream.CanSeek)
			{
				throw new NotSupportedException("Cannot write to a BufferedStream while the read buffer is not empty if the underlying stream is not seekable. Ensure that the stream underlying this BufferedStream can seek or avoid interleaving read and write operations on this BufferedStream.");
			}
			this.FlushRead();
		}

		private void FlushWrite()
		{
			this._stream.Write(this._buffer, 0, this._writePos);
			this._writePos = 0;
			this._stream.Flush();
		}

		private Task FlushWriteAsync(CancellationToken cancellationToken)
		{
			BufferedStream.<FlushWriteAsync>d__42 <FlushWriteAsync>d__;
			<FlushWriteAsync>d__.<>4__this = this;
			<FlushWriteAsync>d__.cancellationToken = cancellationToken;
			<FlushWriteAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FlushWriteAsync>d__.<>1__state = -1;
			<FlushWriteAsync>d__.<>t__builder.Start<BufferedStream.<FlushWriteAsync>d__42>(ref <FlushWriteAsync>d__);
			return <FlushWriteAsync>d__.<>t__builder.Task;
		}

		private int ReadFromBuffer(byte[] array, int offset, int count)
		{
			int num = this._readLen - this._readPos;
			if (num == 0)
			{
				return 0;
			}
			if (num > count)
			{
				num = count;
			}
			Buffer.BlockCopy(this._buffer, this._readPos, array, offset, num);
			this._readPos += num;
			return num;
		}

		private int ReadFromBuffer(Span<byte> destination)
		{
			int num = Math.Min(this._readLen - this._readPos, destination.Length);
			if (num > 0)
			{
				new ReadOnlySpan<byte>(this._buffer, this._readPos, num).CopyTo(destination);
				this._readPos += num;
			}
			return num;
		}

		private int ReadFromBuffer(byte[] array, int offset, int count, out Exception error)
		{
			int result;
			try
			{
				error = null;
				result = this.ReadFromBuffer(array, offset, count);
			}
			catch (Exception ex)
			{
				error = ex;
				result = 0;
			}
			return result;
		}

		/// <summary>Copies bytes from the current buffered stream to an array.</summary>
		/// <param name="array">The buffer to which bytes are to be copied.</param>
		/// <param name="offset">The byte offset in the buffer at which to begin reading bytes.</param>
		/// <param name="count">The number of bytes to be read.</param>
		/// <returns>The total number of bytes read into <paramref name="array" />. This can be less than the number of bytes requested if that many bytes are not currently available, or 0 if the end of the stream has been reached before any data can be read.</returns>
		/// <exception cref="T:System.ArgumentException">Length of <paramref name="array" /> minus <paramref name="offset" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">The stream is not open or is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override int Read(byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			this.EnsureNotClosed();
			this.EnsureCanRead();
			int num = this.ReadFromBuffer(array, offset, count);
			if (num == count)
			{
				return num;
			}
			int num2 = num;
			if (num > 0)
			{
				count -= num;
				offset += num;
			}
			this._readPos = (this._readLen = 0);
			if (this._writePos > 0)
			{
				this.FlushWrite();
			}
			if (count >= this._bufferSize)
			{
				return this._stream.Read(array, offset, count) + num2;
			}
			this.EnsureBufferAllocated();
			this._readLen = this._stream.Read(this._buffer, 0, this._bufferSize);
			num = this.ReadFromBuffer(array, offset, count);
			return num + num2;
		}

		public override int Read(Span<byte> destination)
		{
			this.EnsureNotClosed();
			this.EnsureCanRead();
			int num = this.ReadFromBuffer(destination);
			if (num == destination.Length)
			{
				return num;
			}
			if (num > 0)
			{
				destination = destination.Slice(num);
			}
			this._readPos = (this._readLen = 0);
			if (this._writePos > 0)
			{
				this.FlushWrite();
			}
			if (destination.Length >= this._bufferSize)
			{
				return this._stream.Read(destination) + num;
			}
			this.EnsureBufferAllocated();
			this._readLen = this._stream.Read(this._buffer, 0, this._bufferSize);
			return this.ReadFromBuffer(destination) + num;
		}

		private Task<int> LastSyncCompletedReadTask(int val)
		{
			Task<int> task = this._lastSyncCompletedReadTask;
			if (task != null && task.Result == val)
			{
				return task;
			}
			task = Task.FromResult<int>(val);
			this._lastSyncCompletedReadTask = task;
			return task;
		}

		/// <summary>Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.</summary>
		/// <param name="buffer">The buffer to write the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			this.EnsureNotClosed();
			this.EnsureCanRead();
			int num = 0;
			SemaphoreSlim semaphoreSlim = this.LazyEnsureAsyncActiveSemaphoreInitialized();
			Task task = semaphoreSlim.WaitAsync();
			if (task.IsCompletedSuccessfully)
			{
				bool flag = true;
				try
				{
					Exception ex;
					num = this.ReadFromBuffer(buffer, offset, count, out ex);
					flag = (num == count || ex != null);
					if (flag)
					{
						return (ex == null) ? this.LastSyncCompletedReadTask(num) : Task.FromException<int>(ex);
					}
				}
				finally
				{
					if (flag)
					{
						semaphoreSlim.Release();
					}
				}
			}
			return this.ReadFromUnderlyingStreamAsync(new Memory<byte>(buffer, offset + num, count - num), cancellationToken, num, task).AsTask();
		}

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			this.EnsureNotClosed();
			this.EnsureCanRead();
			int num = 0;
			SemaphoreSlim semaphoreSlim = this.LazyEnsureAsyncActiveSemaphoreInitialized();
			Task task = semaphoreSlim.WaitAsync();
			if (task.IsCompletedSuccessfully)
			{
				bool flag = true;
				try
				{
					num = this.ReadFromBuffer(buffer.Span);
					flag = (num == buffer.Length);
					if (flag)
					{
						return new ValueTask<int>(num);
					}
				}
				finally
				{
					if (flag)
					{
						semaphoreSlim.Release();
					}
				}
			}
			return this.ReadFromUnderlyingStreamAsync(buffer.Slice(num), cancellationToken, num, task);
		}

		private ValueTask<int> ReadFromUnderlyingStreamAsync(Memory<byte> buffer, CancellationToken cancellationToken, int bytesAlreadySatisfied, Task semaphoreLockTask)
		{
			BufferedStream.<ReadFromUnderlyingStreamAsync>d__51 <ReadFromUnderlyingStreamAsync>d__;
			<ReadFromUnderlyingStreamAsync>d__.<>4__this = this;
			<ReadFromUnderlyingStreamAsync>d__.buffer = buffer;
			<ReadFromUnderlyingStreamAsync>d__.cancellationToken = cancellationToken;
			<ReadFromUnderlyingStreamAsync>d__.bytesAlreadySatisfied = bytesAlreadySatisfied;
			<ReadFromUnderlyingStreamAsync>d__.semaphoreLockTask = semaphoreLockTask;
			<ReadFromUnderlyingStreamAsync>d__.<>t__builder = AsyncValueTaskMethodBuilder<int>.Create();
			<ReadFromUnderlyingStreamAsync>d__.<>1__state = -1;
			<ReadFromUnderlyingStreamAsync>d__.<>t__builder.Start<BufferedStream.<ReadFromUnderlyingStreamAsync>d__51>(ref <ReadFromUnderlyingStreamAsync>d__);
			return <ReadFromUnderlyingStreamAsync>d__.<>t__builder.Task;
		}

		/// <summary>Begins an asynchronous read operation. (Consider using <see cref="M:System.IO.BufferedStream.ReadAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)" /> instead.)</summary>
		/// <param name="buffer">The buffer to read the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data read from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="callback">An optional asynchronous callback, to be called when the read is complete.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
		/// <returns>An object that represents the asynchronous read, which could still be pending.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">Attempted an asynchronous read past the end of the stream.</exception>
		/// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="offset" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The current stream does not support the read operation.</exception>
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(this.ReadAsync(buffer, offset, count, CancellationToken.None), callback, state);
		}

		/// <summary>Waits for the pending asynchronous read operation to complete. (Consider using <see cref="M:System.IO.BufferedStream.ReadAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)" /> instead.)</summary>
		/// <param name="asyncResult">The reference to the pending asynchronous request to wait for.</param>
		/// <returns>The number of bytes read from the stream, between 0 (zero) and the number of bytes you requested. Streams only return 0 only at the end of the stream, otherwise, they should block until at least 1 byte is available.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">This <see cref="T:System.IAsyncResult" /> object was not created by calling <see cref="M:System.IO.BufferedStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> on this class.</exception>
		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		/// <summary>Reads a byte from the underlying stream and returns the byte cast to an <see langword="int" />, or returns -1 if reading from the end of the stream.</summary>
		/// <returns>The byte cast to an <see langword="int" />, or -1 if reading from the end of the stream.</returns>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs, such as the stream being closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override int ReadByte()
		{
			if (this._readPos == this._readLen)
			{
				return this.ReadByteSlow();
			}
			byte[] buffer = this._buffer;
			int readPos = this._readPos;
			this._readPos = readPos + 1;
			return buffer[readPos];
		}

		private int ReadByteSlow()
		{
			this.EnsureNotClosed();
			this.EnsureCanRead();
			if (this._writePos > 0)
			{
				this.FlushWrite();
			}
			this.EnsureBufferAllocated();
			this._readLen = this._stream.Read(this._buffer, 0, this._bufferSize);
			this._readPos = 0;
			if (this._readLen == 0)
			{
				return -1;
			}
			byte[] buffer = this._buffer;
			int readPos = this._readPos;
			this._readPos = readPos + 1;
			return buffer[readPos];
		}

		private void WriteToBuffer(byte[] array, ref int offset, ref int count)
		{
			int num = Math.Min(this._bufferSize - this._writePos, count);
			if (num <= 0)
			{
				return;
			}
			this.EnsureBufferAllocated();
			Buffer.BlockCopy(array, offset, this._buffer, this._writePos, num);
			this._writePos += num;
			count -= num;
			offset += num;
		}

		private int WriteToBuffer(ReadOnlySpan<byte> buffer)
		{
			int num = Math.Min(this._bufferSize - this._writePos, buffer.Length);
			if (num > 0)
			{
				this.EnsureBufferAllocated();
				buffer.Slice(0, num).CopyTo(new Span<byte>(this._buffer, this._writePos, num));
				this._writePos += num;
			}
			return num;
		}

		private void WriteToBuffer(byte[] array, ref int offset, ref int count, out Exception error)
		{
			try
			{
				error = null;
				this.WriteToBuffer(array, ref offset, ref count);
			}
			catch (Exception ex)
			{
				error = ex;
			}
		}

		/// <summary>Copies bytes to the buffered stream and advances the current position within the buffered stream by the number of bytes written.</summary>
		/// <param name="array">The byte array from which to copy <paramref name="count" /> bytes to the current buffered stream.</param>
		/// <param name="offset">The offset in the buffer at which to begin copying bytes to the current buffered stream.</param>
		/// <param name="count">The number of bytes to be written to the current buffered stream.</param>
		/// <exception cref="T:System.ArgumentException">Length of <paramref name="array" /> minus <paramref name="offset" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">The stream is closed or <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override void Write(byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			this.EnsureNotClosed();
			this.EnsureCanWrite();
			if (this._writePos == 0)
			{
				this.ClearReadBufferBeforeWrite();
			}
			int num = checked(this._writePos + count);
			if (checked(num + count >= this._bufferSize + this._bufferSize))
			{
				if (this._writePos > 0)
				{
					if (num <= this._bufferSize + this._bufferSize && num <= 81920)
					{
						this.EnsureShadowBufferAllocated();
						Buffer.BlockCopy(array, offset, this._buffer, this._writePos, count);
						this._stream.Write(this._buffer, 0, num);
						this._writePos = 0;
						return;
					}
					this._stream.Write(this._buffer, 0, this._writePos);
					this._writePos = 0;
				}
				this._stream.Write(array, offset, count);
				return;
			}
			this.WriteToBuffer(array, ref offset, ref count);
			if (this._writePos < this._bufferSize)
			{
				return;
			}
			this._stream.Write(this._buffer, 0, this._writePos);
			this._writePos = 0;
			this.WriteToBuffer(array, ref offset, ref count);
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			this.EnsureNotClosed();
			this.EnsureCanWrite();
			if (this._writePos == 0)
			{
				this.ClearReadBufferBeforeWrite();
			}
			int num = checked(this._writePos + buffer.Length);
			if (checked(num + buffer.Length >= this._bufferSize + this._bufferSize))
			{
				if (this._writePos > 0)
				{
					if (num <= this._bufferSize + this._bufferSize && num <= 81920)
					{
						this.EnsureShadowBufferAllocated();
						buffer.CopyTo(new Span<byte>(this._buffer, this._writePos, buffer.Length));
						this._stream.Write(this._buffer, 0, num);
						this._writePos = 0;
						return;
					}
					this._stream.Write(this._buffer, 0, this._writePos);
					this._writePos = 0;
				}
				this._stream.Write(buffer);
				return;
			}
			int start = this.WriteToBuffer(buffer);
			if (this._writePos < this._bufferSize)
			{
				return;
			}
			buffer = buffer.Slice(start);
			this._stream.Write(this._buffer, 0, this._writePos);
			this._writePos = 0;
			start = this.WriteToBuffer(buffer);
		}

		/// <summary>Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.</summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> from which to begin copying bytes to the stream.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous write operation.</exception>
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			return this.WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
		}

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask(Task.FromCanceled<int>(cancellationToken));
			}
			this.EnsureNotClosed();
			this.EnsureCanWrite();
			SemaphoreSlim semaphoreSlim = this.LazyEnsureAsyncActiveSemaphoreInitialized();
			Task task = semaphoreSlim.WaitAsync();
			if (task.IsCompletedSuccessfully)
			{
				bool flag = true;
				try
				{
					if (this._writePos == 0)
					{
						this.ClearReadBufferBeforeWrite();
					}
					flag = (buffer.Length < this._bufferSize - this._writePos);
					if (flag)
					{
						this.WriteToBuffer(buffer.Span);
						return default(ValueTask);
					}
				}
				finally
				{
					if (flag)
					{
						semaphoreSlim.Release();
					}
				}
			}
			return new ValueTask(this.WriteToUnderlyingStreamAsync(buffer, cancellationToken, task));
		}

		private Task WriteToUnderlyingStreamAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken, Task semaphoreLockTask)
		{
			BufferedStream.<WriteToUnderlyingStreamAsync>d__63 <WriteToUnderlyingStreamAsync>d__;
			<WriteToUnderlyingStreamAsync>d__.<>4__this = this;
			<WriteToUnderlyingStreamAsync>d__.buffer = buffer;
			<WriteToUnderlyingStreamAsync>d__.cancellationToken = cancellationToken;
			<WriteToUnderlyingStreamAsync>d__.semaphoreLockTask = semaphoreLockTask;
			<WriteToUnderlyingStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteToUnderlyingStreamAsync>d__.<>1__state = -1;
			<WriteToUnderlyingStreamAsync>d__.<>t__builder.Start<BufferedStream.<WriteToUnderlyingStreamAsync>d__63>(ref <WriteToUnderlyingStreamAsync>d__);
			return <WriteToUnderlyingStreamAsync>d__.<>t__builder.Task;
		}

		/// <summary>Begins an asynchronous write operation. (Consider using <see cref="M:System.IO.BufferedStream.WriteAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)" /> instead.)</summary>
		/// <param name="buffer">The buffer containing data to write to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <param name="callback">The method to be called when the asynchronous write operation is completed.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
		/// <returns>An object that references the asynchronous write which could still be pending.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="buffer" /> length minus <paramref name="offset" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(this.WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);
		}

		/// <summary>Ends an asynchronous write operation and blocks until the I/O operation is complete. (Consider using <see cref="M:System.IO.BufferedStream.WriteAsync(System.Byte[],System.Int32,System.Int32,System.Threading.CancellationToken)" /> instead.)</summary>
		/// <param name="asyncResult">The pending asynchronous request.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">This <see cref="T:System.IAsyncResult" /> object was not created by calling <see cref="M:System.IO.BufferedStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> on this class.</exception>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		/// <summary>Writes a byte to the current position in the buffered stream.</summary>
		/// <param name="value">A byte to write to the stream.</param>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override void WriteByte(byte value)
		{
			this.EnsureNotClosed();
			if (this._writePos == 0)
			{
				this.EnsureCanWrite();
				this.ClearReadBufferBeforeWrite();
				this.EnsureBufferAllocated();
			}
			if (this._writePos >= this._bufferSize - 1)
			{
				this.FlushWrite();
			}
			byte[] buffer = this._buffer;
			int writePos = this._writePos;
			this._writePos = writePos + 1;
			buffer[writePos] = value;
		}

		/// <summary>Sets the position within the current buffered stream.</summary>
		/// <param name="offset">A byte offset relative to <paramref name="origin" />.</param>
		/// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point from which to obtain the new position.</param>
		/// <returns>The new position within the current buffered stream.</returns>
		/// <exception cref="T:System.IO.IOException">The stream is not open or is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			this.EnsureNotClosed();
			this.EnsureCanSeek();
			if (this._writePos > 0)
			{
				this.FlushWrite();
				return this._stream.Seek(offset, origin);
			}
			if (this._readLen - this._readPos > 0 && origin == SeekOrigin.Current)
			{
				offset -= (long)(this._readLen - this._readPos);
			}
			long position = this.Position;
			long num = this._stream.Seek(offset, origin);
			this._readPos = (int)(num - (position - (long)this._readPos));
			if (0 <= this._readPos && this._readPos < this._readLen)
			{
				this._stream.Seek((long)(this._readLen - this._readPos), SeekOrigin.Current);
			}
			else
			{
				this._readPos = (this._readLen = 0);
			}
			return num;
		}

		/// <summary>Sets the length of the buffered stream.</summary>
		/// <param name="value">An integer indicating the desired length of the current buffered stream in bytes.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">The stream is not open or is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public override void SetLength(long value)
		{
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("value", "Non-negative number required.");
			}
			this.EnsureNotClosed();
			this.EnsureCanSeek();
			this.EnsureCanWrite();
			this.Flush();
			this._stream.SetLength(value);
		}

		public override void CopyTo(Stream destination, int bufferSize)
		{
			StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
			int num = this._readLen - this._readPos;
			if (num > 0)
			{
				destination.Write(this._buffer, this._readPos, num);
				this._readPos = (this._readLen = 0);
			}
			else if (this._writePos > 0)
			{
				this.FlushWrite();
			}
			this._stream.CopyTo(destination, bufferSize);
		}

		public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
			if (!cancellationToken.IsCancellationRequested)
			{
				return this.CopyToAsyncCore(destination, bufferSize, cancellationToken);
			}
			return Task.FromCanceled<int>(cancellationToken);
		}

		private Task CopyToAsyncCore(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			BufferedStream.<CopyToAsyncCore>d__71 <CopyToAsyncCore>d__;
			<CopyToAsyncCore>d__.<>4__this = this;
			<CopyToAsyncCore>d__.destination = destination;
			<CopyToAsyncCore>d__.bufferSize = bufferSize;
			<CopyToAsyncCore>d__.cancellationToken = cancellationToken;
			<CopyToAsyncCore>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CopyToAsyncCore>d__.<>1__state = -1;
			<CopyToAsyncCore>d__.<>t__builder.Start<BufferedStream.<CopyToAsyncCore>d__71>(ref <CopyToAsyncCore>d__);
			return <CopyToAsyncCore>d__.<>t__builder.Task;
		}

		private const int MaxShadowBufferSize = 81920;

		private const int DefaultBufferSize = 4096;

		private Stream _stream;

		private byte[] _buffer;

		private readonly int _bufferSize;

		private int _readPos;

		private int _readLen;

		private int _writePos;

		private Task<int> _lastSyncCompletedReadTask;

		private SemaphoreSlim _asyncActiveSemaphore;
	}
}
