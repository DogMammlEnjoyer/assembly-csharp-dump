using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	/// <summary>Provides a generic view of a sequence of bytes. This is an abstract class.</summary>
	[Serializable]
	public abstract class Stream : MarshalByRefObject, IDisposable, IAsyncDisposable
	{
		internal SemaphoreSlim EnsureAsyncActiveSemaphoreInitialized()
		{
			return LazyInitializer.EnsureInitialized<SemaphoreSlim>(ref this._asyncActiveSemaphore, () => new SemaphoreSlim(1, 1));
		}

		/// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports reading.</summary>
		/// <returns>
		///   <see langword="true" /> if the stream supports reading; otherwise, <see langword="false" />.</returns>
		public abstract bool CanRead { get; }

		/// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports seeking.</summary>
		/// <returns>
		///   <see langword="true" /> if the stream supports seeking; otherwise, <see langword="false" />.</returns>
		public abstract bool CanSeek { get; }

		/// <summary>Gets a value that determines whether the current stream can time out.</summary>
		/// <returns>A value that determines whether the current stream can time out.</returns>
		public virtual bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		/// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports writing.</summary>
		/// <returns>
		///   <see langword="true" /> if the stream supports writing; otherwise, <see langword="false" />.</returns>
		public abstract bool CanWrite { get; }

		/// <summary>When overridden in a derived class, gets the length in bytes of the stream.</summary>
		/// <returns>A long value representing the length of the stream in bytes.</returns>
		/// <exception cref="T:System.NotSupportedException">A class derived from <see langword="Stream" /> does not support seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public abstract long Length { get; }

		/// <summary>When overridden in a derived class, gets or sets the position within the current stream.</summary>
		/// <returns>The current position within the stream.</returns>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public abstract long Position { get; set; }

		/// <summary>Gets or sets a value, in miliseconds, that determines how long the stream will attempt to read before timing out.</summary>
		/// <returns>A value, in miliseconds, that determines how long the stream will attempt to read before timing out.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.IO.Stream.ReadTimeout" /> method always throws an <see cref="T:System.InvalidOperationException" />.</exception>
		public virtual int ReadTimeout
		{
			get
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
			set
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
		}

		/// <summary>Gets or sets a value, in miliseconds, that determines how long the stream will attempt to write before timing out.</summary>
		/// <returns>A value, in miliseconds, that determines how long the stream will attempt to write before timing out.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.IO.Stream.WriteTimeout" /> method always throws an <see cref="T:System.InvalidOperationException" />.</exception>
		public virtual int WriteTimeout
		{
			get
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
			set
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
		}

		/// <summary>Asynchronously reads the bytes from the current stream and writes them to another stream.</summary>
		/// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
		/// <returns>A task that represents the asynchronous copy operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destination" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Either the current stream or the destination stream is disposed.</exception>
		/// <exception cref="T:System.NotSupportedException">The current stream does not support reading, or the destination stream does not support writing.</exception>
		public Task CopyToAsync(Stream destination)
		{
			int copyBufferSize = this.GetCopyBufferSize();
			return this.CopyToAsync(destination, copyBufferSize);
		}

		/// <summary>Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified buffer size.</summary>
		/// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
		/// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
		/// <returns>A task that represents the asynchronous copy operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destination" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="buffersize" /> is negative or zero.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Either the current stream or the destination stream is disposed.</exception>
		/// <exception cref="T:System.NotSupportedException">The current stream does not support reading, or the destination stream does not support writing.</exception>
		public Task CopyToAsync(Stream destination, int bufferSize)
		{
			return this.CopyToAsync(destination, bufferSize, CancellationToken.None);
		}

		public Task CopyToAsync(Stream destination, CancellationToken cancellationToken)
		{
			int copyBufferSize = this.GetCopyBufferSize();
			return this.CopyToAsync(destination, copyBufferSize, cancellationToken);
		}

		/// <summary>Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified buffer size and cancellation token.</summary>
		/// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
		/// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero. The default size is 81920.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
		/// <returns>A task that represents the asynchronous copy operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destination" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="buffersize" /> is negative or zero.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Either the current stream or the destination stream is disposed.</exception>
		/// <exception cref="T:System.NotSupportedException">The current stream does not support reading, or the destination stream does not support writing.</exception>
		public virtual Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
			return this.CopyToAsyncInternal(destination, bufferSize, cancellationToken);
		}

		private Task CopyToAsyncInternal(Stream destination, int bufferSize, CancellationToken cancellationToken)
		{
			Stream.<CopyToAsyncInternal>d__28 <CopyToAsyncInternal>d__;
			<CopyToAsyncInternal>d__.<>4__this = this;
			<CopyToAsyncInternal>d__.destination = destination;
			<CopyToAsyncInternal>d__.bufferSize = bufferSize;
			<CopyToAsyncInternal>d__.cancellationToken = cancellationToken;
			<CopyToAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CopyToAsyncInternal>d__.<>1__state = -1;
			<CopyToAsyncInternal>d__.<>t__builder.Start<Stream.<CopyToAsyncInternal>d__28>(ref <CopyToAsyncInternal>d__);
			return <CopyToAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Reads the bytes from the current stream and writes them to another stream.</summary>
		/// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destination" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The current stream does not support reading.  
		///  -or-  
		///  <paramref name="destination" /> does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Either the current stream or <paramref name="destination" /> were closed before the <see cref="M:System.IO.Stream.CopyTo(System.IO.Stream)" /> method was called.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
		public void CopyTo(Stream destination)
		{
			int copyBufferSize = this.GetCopyBufferSize();
			this.CopyTo(destination, copyBufferSize);
		}

		/// <summary>Reads the bytes from the current stream and writes them to another stream, using a specified buffer size.</summary>
		/// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
		/// <param name="bufferSize">The size of the buffer. This value must be greater than zero. The default size is 81920.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destination" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="bufferSize" /> is negative or zero.</exception>
		/// <exception cref="T:System.NotSupportedException">The current stream does not support reading.  
		///  -or-  
		///  <paramref name="destination" /> does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Either the current stream or <paramref name="destination" /> were closed before the <see cref="M:System.IO.Stream.CopyTo(System.IO.Stream)" /> method was called.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
		public virtual void CopyTo(Stream destination, int bufferSize)
		{
			StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
			byte[] array = ArrayPool<byte>.Shared.Rent(bufferSize);
			try
			{
				int count;
				while ((count = this.Read(array, 0, array.Length)) != 0)
				{
					destination.Write(array, 0, count);
				}
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
		}

		private int GetCopyBufferSize()
		{
			int num = 81920;
			if (this.CanSeek)
			{
				long length = this.Length;
				long position = this.Position;
				if (length <= position)
				{
					num = 1;
				}
				else
				{
					long num2 = length - position;
					if (num2 > 0L)
					{
						num = (int)Math.Min((long)num, num2);
					}
				}
			}
			return num;
		}

		/// <summary>Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream. Instead of calling this method, ensure that the stream is properly disposed.</summary>
		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases all resources used by the <see cref="T:System.IO.Stream" />.</summary>
		public void Dispose()
		{
			this.Close();
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Stream" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public abstract void Flush();

		/// <summary>Asynchronously clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
		/// <returns>A task that represents the asynchronous flush operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		public Task FlushAsync()
		{
			return this.FlushAsync(CancellationToken.None);
		}

		/// <summary>Asynchronously clears all buffers for this stream, causes any buffered data to be written to the underlying device, and monitors cancellation requests.</summary>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
		/// <returns>A task that represents the asynchronous flush operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		public virtual Task FlushAsync(CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(delegate(object state)
			{
				((Stream)state).Flush();
			}, this, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		/// <summary>Allocates a <see cref="T:System.Threading.WaitHandle" /> object.</summary>
		/// <returns>A reference to the allocated <see langword="WaitHandle" />.</returns>
		[Obsolete("CreateWaitHandle will be removed eventually.  Please use \"new ManualResetEvent(false)\" instead.")]
		protected virtual WaitHandle CreateWaitHandle()
		{
			return new ManualResetEvent(false);
		}

		/// <summary>Begins an asynchronous read operation. (Consider using <see cref="M:System.IO.Stream.ReadAsync(System.Byte[],System.Int32,System.Int32)" /> instead.)</summary>
		/// <param name="buffer">The buffer to read the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data read from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="callback">An optional asynchronous callback, to be called when the read is complete.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that represents the asynchronous read, which could still be pending.</returns>
		/// <exception cref="T:System.IO.IOException">Attempted an asynchronous read past the end of the stream, or a disk error occurs.</exception>
		/// <exception cref="T:System.ArgumentException">One or more of the arguments is invalid.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The current <see langword="Stream" /> implementation does not support the read operation.</exception>
		public virtual IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return this.BeginReadInternal(buffer, offset, count, callback, state, false, true);
		}

		internal IAsyncResult BeginReadInternal(byte[] buffer, int offset, int count, AsyncCallback callback, object state, bool serializeAsynchronously, bool apm)
		{
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			SemaphoreSlim semaphoreSlim = this.EnsureAsyncActiveSemaphoreInitialized();
			Task task = null;
			if (serializeAsynchronously)
			{
				task = semaphoreSlim.WaitAsync();
			}
			else
			{
				semaphoreSlim.Wait();
			}
			Stream.ReadWriteTask readWriteTask = new Stream.ReadWriteTask(true, apm, delegate(object <p0>)
			{
				Stream.ReadWriteTask readWriteTask2 = Task.InternalCurrent as Stream.ReadWriteTask;
				int result;
				try
				{
					result = readWriteTask2._stream.Read(readWriteTask2._buffer, readWriteTask2._offset, readWriteTask2._count);
				}
				finally
				{
					if (!readWriteTask2._apm)
					{
						readWriteTask2._stream.FinishTrackingAsyncOperation();
					}
					readWriteTask2.ClearBeginState();
				}
				return result;
			}, state, this, buffer, offset, count, callback);
			if (task != null)
			{
				this.RunReadWriteTaskWhenReady(task, readWriteTask);
			}
			else
			{
				this.RunReadWriteTask(readWriteTask);
			}
			return readWriteTask;
		}

		/// <summary>Waits for the pending asynchronous read to complete. (Consider using <see cref="M:System.IO.Stream.ReadAsync(System.Byte[],System.Int32,System.Int32)" /> instead.)</summary>
		/// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
		/// <returns>The number of bytes read from the stream, between zero (0) and the number of bytes you requested. Streams return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is available.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A handle to the pending read operation is not available.  
		///  -or-  
		///  The pending operation does not support reading.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="asyncResult" /> did not originate from a <see cref="M:System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> method on the current stream.</exception>
		/// <exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception>
		public virtual int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Stream.ReadWriteTask activeReadWriteTask = this._activeReadWriteTask;
			if (activeReadWriteTask == null)
			{
				throw new ArgumentException("Either the IAsyncResult object did not come from the corresponding async method on this type, or EndRead was called multiple times with the same IAsyncResult.");
			}
			if (activeReadWriteTask != asyncResult)
			{
				throw new InvalidOperationException("Either the IAsyncResult object did not come from the corresponding async method on this type, or EndRead was called multiple times with the same IAsyncResult.");
			}
			if (!activeReadWriteTask._isRead)
			{
				throw new ArgumentException("Either the IAsyncResult object did not come from the corresponding async method on this type, or EndRead was called multiple times with the same IAsyncResult.");
			}
			int result;
			try
			{
				result = activeReadWriteTask.GetAwaiter().GetResult();
			}
			finally
			{
				this.FinishTrackingAsyncOperation();
			}
			return result;
		}

		/// <summary>Asynchronously reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
		/// <param name="buffer">The buffer to write the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
		public Task<int> ReadAsync(byte[] buffer, int offset, int count)
		{
			return this.ReadAsync(buffer, offset, count, CancellationToken.None);
		}

		/// <summary>Asynchronously reads a sequence of bytes from the current stream, advances the position within the stream by the number of bytes read, and monitors cancellation requests.</summary>
		/// <param name="buffer">The buffer to write the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains the total number of bytes read into the buffer. The result value can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
		public virtual Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return this.BeginEndReadAsync(buffer, offset, count);
			}
			return Task.FromCanceled<int>(cancellationToken);
		}

		public virtual ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ArraySegment<byte> arraySegment;
			if (MemoryMarshal.TryGetArray<byte>(buffer, out arraySegment))
			{
				return new ValueTask<int>(this.ReadAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count, cancellationToken));
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
			return Stream.<ReadAsync>g__FinishReadAsync|44_0(this.ReadAsync(array, 0, buffer.Length, cancellationToken), array, buffer);
		}

		private Task<int> BeginEndReadAsync(byte[] buffer, int offset, int count)
		{
			if (!this.HasOverriddenBeginEndRead())
			{
				return (Task<int>)this.BeginReadInternal(buffer, offset, count, null, null, true, false);
			}
			return TaskFactory<int>.FromAsyncTrim<Stream, Stream.ReadWriteParameters>(this, new Stream.ReadWriteParameters
			{
				Buffer = buffer,
				Offset = offset,
				Count = count
			}, (Stream stream, Stream.ReadWriteParameters args, AsyncCallback callback, object state) => stream.BeginRead(args.Buffer, args.Offset, args.Count, callback, state), (Stream stream, IAsyncResult asyncResult) => stream.EndRead(asyncResult));
		}

		/// <summary>Begins an asynchronous write operation. (Consider using <see cref="M:System.IO.Stream.WriteAsync(System.Byte[],System.Int32,System.Int32)" /> instead.)</summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> from which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <param name="callback">An optional asynchronous callback, to be called when the write is complete.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
		/// <returns>An <see langword="IAsyncResult" /> that represents the asynchronous write, which could still be pending.</returns>
		/// <exception cref="T:System.IO.IOException">Attempted an asynchronous write past the end of the stream, or a disk error occurs.</exception>
		/// <exception cref="T:System.ArgumentException">One or more of the arguments is invalid.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The current <see langword="Stream" /> implementation does not support the write operation.</exception>
		public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return this.BeginWriteInternal(buffer, offset, count, callback, state, false, true);
		}

		internal IAsyncResult BeginWriteInternal(byte[] buffer, int offset, int count, AsyncCallback callback, object state, bool serializeAsynchronously, bool apm)
		{
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			SemaphoreSlim semaphoreSlim = this.EnsureAsyncActiveSemaphoreInitialized();
			Task task = null;
			if (serializeAsynchronously)
			{
				task = semaphoreSlim.WaitAsync();
			}
			else
			{
				semaphoreSlim.Wait();
			}
			Stream.ReadWriteTask readWriteTask = new Stream.ReadWriteTask(false, apm, delegate(object <p0>)
			{
				Stream.ReadWriteTask readWriteTask2 = Task.InternalCurrent as Stream.ReadWriteTask;
				int result;
				try
				{
					readWriteTask2._stream.Write(readWriteTask2._buffer, readWriteTask2._offset, readWriteTask2._count);
					result = 0;
				}
				finally
				{
					if (!readWriteTask2._apm)
					{
						readWriteTask2._stream.FinishTrackingAsyncOperation();
					}
					readWriteTask2.ClearBeginState();
				}
				return result;
			}, state, this, buffer, offset, count, callback);
			if (task != null)
			{
				this.RunReadWriteTaskWhenReady(task, readWriteTask);
			}
			else
			{
				this.RunReadWriteTask(readWriteTask);
			}
			return readWriteTask;
		}

		private void RunReadWriteTaskWhenReady(Task asyncWaiter, Stream.ReadWriteTask readWriteTask)
		{
			if (asyncWaiter.IsCompleted)
			{
				this.RunReadWriteTask(readWriteTask);
				return;
			}
			asyncWaiter.ContinueWith(delegate(Task t, object state)
			{
				Stream.ReadWriteTask readWriteTask2 = (Stream.ReadWriteTask)state;
				readWriteTask2._stream.RunReadWriteTask(readWriteTask2);
			}, readWriteTask, default(CancellationToken), TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
		}

		private void RunReadWriteTask(Stream.ReadWriteTask readWriteTask)
		{
			this._activeReadWriteTask = readWriteTask;
			readWriteTask.m_taskScheduler = TaskScheduler.Default;
			readWriteTask.ScheduleAndStart(false);
		}

		private void FinishTrackingAsyncOperation()
		{
			this._activeReadWriteTask = null;
			this._asyncActiveSemaphore.Release();
		}

		/// <summary>Ends an asynchronous write operation. (Consider using <see cref="M:System.IO.Stream.WriteAsync(System.Byte[],System.Int32,System.Int32)" /> instead.)</summary>
		/// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A handle to the pending write operation is not available.  
		///  -or-  
		///  The pending operation does not support writing.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="asyncResult" /> did not originate from a <see cref="M:System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> method on the current stream.</exception>
		/// <exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception>
		public virtual void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Stream.ReadWriteTask activeReadWriteTask = this._activeReadWriteTask;
			if (activeReadWriteTask == null)
			{
				throw new ArgumentException("Either the IAsyncResult object did not come from the corresponding async method on this type, or EndWrite was called multiple times with the same IAsyncResult.");
			}
			if (activeReadWriteTask != asyncResult)
			{
				throw new InvalidOperationException("Either the IAsyncResult object did not come from the corresponding async method on this type, or EndWrite was called multiple times with the same IAsyncResult.");
			}
			if (activeReadWriteTask._isRead)
			{
				throw new ArgumentException("Either the IAsyncResult object did not come from the corresponding async method on this type, or EndWrite was called multiple times with the same IAsyncResult.");
			}
			try
			{
				activeReadWriteTask.GetAwaiter().GetResult();
			}
			finally
			{
				this.FinishTrackingAsyncOperation();
			}
		}

		/// <summary>Asynchronously writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> from which to begin copying bytes to the stream.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous write operation.</exception>
		public Task WriteAsync(byte[] buffer, int offset, int count)
		{
			return this.WriteAsync(buffer, offset, count, CancellationToken.None);
		}

		/// <summary>Asynchronously writes a sequence of bytes to the current stream, advances the current position within this stream by the number of bytes written, and monitors cancellation requests.</summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> from which to begin copying bytes to the stream.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is currently in use by a previous write operation.</exception>
		public virtual Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				return this.BeginEndWriteAsync(buffer, offset, count);
			}
			return Task.FromCanceled(cancellationToken);
		}

		public virtual ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ArraySegment<byte> arraySegment;
			if (MemoryMarshal.TryGetArray<byte>(buffer, out arraySegment))
			{
				return new ValueTask(this.WriteAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count, cancellationToken));
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
			buffer.Span.CopyTo(array);
			return new ValueTask(this.FinishWriteAsync(this.WriteAsync(array, 0, buffer.Length, cancellationToken), array));
		}

		private Task FinishWriteAsync(Task writeTask, byte[] localBuffer)
		{
			Stream.<FinishWriteAsync>d__57 <FinishWriteAsync>d__;
			<FinishWriteAsync>d__.writeTask = writeTask;
			<FinishWriteAsync>d__.localBuffer = localBuffer;
			<FinishWriteAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishWriteAsync>d__.<>1__state = -1;
			<FinishWriteAsync>d__.<>t__builder.Start<Stream.<FinishWriteAsync>d__57>(ref <FinishWriteAsync>d__);
			return <FinishWriteAsync>d__.<>t__builder.Task;
		}

		private Task BeginEndWriteAsync(byte[] buffer, int offset, int count)
		{
			if (!this.HasOverriddenBeginEndWrite())
			{
				return (Task)this.BeginWriteInternal(buffer, offset, count, null, null, true, false);
			}
			return TaskFactory<VoidTaskResult>.FromAsyncTrim<Stream, Stream.ReadWriteParameters>(this, new Stream.ReadWriteParameters
			{
				Buffer = buffer,
				Offset = offset,
				Count = count
			}, (Stream stream, Stream.ReadWriteParameters args, AsyncCallback callback, object state) => stream.BeginWrite(args.Buffer, args.Offset, args.Count, callback, state), delegate(Stream stream, IAsyncResult asyncResult)
			{
				stream.EndWrite(asyncResult);
				return default(VoidTaskResult);
			});
		}

		/// <summary>When overridden in a derived class, sets the position within the current stream.</summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
		/// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
		/// <returns>The new position within the current stream.</returns>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public abstract long Seek(long offset, SeekOrigin origin);

		/// <summary>When overridden in a derived class, sets the length of the current stream.</summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public abstract void SetLength(long value);

		/// <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public abstract int Read(byte[] buffer, int offset, int count);

		public virtual int Read(Span<byte> buffer)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
			int result;
			try
			{
				int num = this.Read(array, 0, buffer.Length);
				if ((ulong)num > (ulong)((long)buffer.Length))
				{
					throw new IOException("Stream was too long.");
				}
				new Span<byte>(array, 0, num).CopyTo(buffer);
				result = num;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
			return result;
		}

		/// <summary>Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.</summary>
		/// <returns>The unsigned byte cast to an <see langword="Int32" />, or -1 if at the end of the stream.</returns>
		/// <exception cref="T:System.NotSupportedException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public virtual int ReadByte()
		{
			byte[] array = new byte[1];
			if (this.Read(array, 0, 1) == 0)
			{
				return -1;
			}
			return (int)array[0];
		}

		/// <summary>When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
		/// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occured, such as the specified file cannot be found.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="M:System.IO.Stream.Write(System.Byte[],System.Int32,System.Int32)" /> was called after the stream was closed.</exception>
		public abstract void Write(byte[] buffer, int offset, int count);

		public virtual void Write(ReadOnlySpan<byte> buffer)
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
			try
			{
				buffer.CopyTo(array);
				this.Write(array, 0, buffer.Length);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array, false);
			}
		}

		/// <summary>Writes a byte to the current position in the stream and advances the position within the stream by one byte.</summary>
		/// <param name="value">The byte to write to the stream.</param>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.NotSupportedException">The stream does not support writing, or the stream is already closed.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		public virtual void WriteByte(byte value)
		{
			this.Write(new byte[]
			{
				value
			}, 0, 1);
		}

		/// <summary>Creates a thread-safe (synchronized) wrapper around the specified <see cref="T:System.IO.Stream" /> object.</summary>
		/// <param name="stream">The <see cref="T:System.IO.Stream" /> object to synchronize.</param>
		/// <returns>A thread-safe <see cref="T:System.IO.Stream" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		public static Stream Synchronized(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (stream is Stream.SyncStream)
			{
				return stream;
			}
			return new Stream.SyncStream(stream);
		}

		/// <summary>Provides support for a <see cref="T:System.Diagnostics.Contracts.Contract" />.</summary>
		[Obsolete("Do not call or override this method.")]
		protected virtual void ObjectInvariant()
		{
		}

		internal IAsyncResult BlockingBeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			Stream.SynchronousAsyncResult synchronousAsyncResult;
			try
			{
				synchronousAsyncResult = new Stream.SynchronousAsyncResult(this.Read(buffer, offset, count), state);
			}
			catch (IOException ex)
			{
				synchronousAsyncResult = new Stream.SynchronousAsyncResult(ex, state, false);
			}
			if (callback != null)
			{
				callback(synchronousAsyncResult);
			}
			return synchronousAsyncResult;
		}

		internal static int BlockingEndRead(IAsyncResult asyncResult)
		{
			return Stream.SynchronousAsyncResult.EndRead(asyncResult);
		}

		internal IAsyncResult BlockingBeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			Stream.SynchronousAsyncResult synchronousAsyncResult;
			try
			{
				this.Write(buffer, offset, count);
				synchronousAsyncResult = new Stream.SynchronousAsyncResult(state);
			}
			catch (IOException ex)
			{
				synchronousAsyncResult = new Stream.SynchronousAsyncResult(ex, state, true);
			}
			if (callback != null)
			{
				callback(synchronousAsyncResult);
			}
			return synchronousAsyncResult;
		}

		internal static void BlockingEndWrite(IAsyncResult asyncResult)
		{
			Stream.SynchronousAsyncResult.EndWrite(asyncResult);
		}

		private bool HasOverriddenBeginEndRead()
		{
			return true;
		}

		private bool HasOverriddenBeginEndWrite()
		{
			return true;
		}

		public virtual ValueTask DisposeAsync()
		{
			ValueTask valueTask;
			try
			{
				this.Dispose();
				valueTask = default(ValueTask);
				valueTask = valueTask;
			}
			catch (Exception exception)
			{
				valueTask = new ValueTask(Task.FromException(exception));
			}
			return valueTask;
		}

		[CompilerGenerated]
		internal static ValueTask<int> <ReadAsync>g__FinishReadAsync|44_0(Task<int> readTask, byte[] localBuffer, Memory<byte> localDestination)
		{
			Stream.<<ReadAsync>g__FinishReadAsync|44_0>d <<ReadAsync>g__FinishReadAsync|44_0>d;
			<<ReadAsync>g__FinishReadAsync|44_0>d.readTask = readTask;
			<<ReadAsync>g__FinishReadAsync|44_0>d.localBuffer = localBuffer;
			<<ReadAsync>g__FinishReadAsync|44_0>d.localDestination = localDestination;
			<<ReadAsync>g__FinishReadAsync|44_0>d.<>t__builder = AsyncValueTaskMethodBuilder<int>.Create();
			<<ReadAsync>g__FinishReadAsync|44_0>d.<>1__state = -1;
			<<ReadAsync>g__FinishReadAsync|44_0>d.<>t__builder.Start<Stream.<<ReadAsync>g__FinishReadAsync|44_0>d>(ref <<ReadAsync>g__FinishReadAsync|44_0>d);
			return <<ReadAsync>g__FinishReadAsync|44_0>d.<>t__builder.Task;
		}

		/// <summary>A <see langword="Stream" /> with no backing store.</summary>
		public static readonly Stream Null = new Stream.NullStream();

		private const int DefaultCopyBufferSize = 81920;

		[NonSerialized]
		private Stream.ReadWriteTask _activeReadWriteTask;

		[NonSerialized]
		private SemaphoreSlim _asyncActiveSemaphore;

		private struct ReadWriteParameters
		{
			internal byte[] Buffer;

			internal int Offset;

			internal int Count;
		}

		private sealed class ReadWriteTask : Task<int>, ITaskCompletionAction
		{
			internal void ClearBeginState()
			{
				this._stream = null;
				this._buffer = null;
			}

			public ReadWriteTask(bool isRead, bool apm, Func<object, int> function, object state, Stream stream, byte[] buffer, int offset, int count, AsyncCallback callback) : base(function, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach)
			{
				this._isRead = isRead;
				this._apm = apm;
				this._stream = stream;
				this._buffer = buffer;
				this._offset = offset;
				this._count = count;
				if (callback != null)
				{
					this._callback = callback;
					this._context = ExecutionContext.Capture();
					base.AddCompletionAction(this);
				}
			}

			private static void InvokeAsyncCallback(object completedTask)
			{
				Stream.ReadWriteTask readWriteTask = (Stream.ReadWriteTask)completedTask;
				AsyncCallback callback = readWriteTask._callback;
				readWriteTask._callback = null;
				callback(readWriteTask);
			}

			void ITaskCompletionAction.Invoke(Task completingTask)
			{
				ExecutionContext context = this._context;
				if (context == null)
				{
					AsyncCallback callback = this._callback;
					this._callback = null;
					callback(completingTask);
					return;
				}
				this._context = null;
				ContextCallback contextCallback = Stream.ReadWriteTask.s_invokeAsyncCallback;
				if (contextCallback == null)
				{
					contextCallback = (Stream.ReadWriteTask.s_invokeAsyncCallback = new ContextCallback(Stream.ReadWriteTask.InvokeAsyncCallback));
				}
				ExecutionContext.RunInternal(context, contextCallback, this);
			}

			bool ITaskCompletionAction.InvokeMayRunArbitraryCode
			{
				get
				{
					return true;
				}
			}

			internal readonly bool _isRead;

			internal readonly bool _apm;

			internal Stream _stream;

			internal byte[] _buffer;

			internal readonly int _offset;

			internal readonly int _count;

			private AsyncCallback _callback;

			private ExecutionContext _context;

			private static ContextCallback s_invokeAsyncCallback;
		}

		private sealed class NullStream : Stream
		{
			internal NullStream()
			{
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
					return true;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return true;
				}
			}

			public override long Length
			{
				get
				{
					return 0L;
				}
			}

			public override long Position
			{
				get
				{
					return 0L;
				}
				set
				{
				}
			}

			public override void CopyTo(Stream destination, int bufferSize)
			{
				StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
			}

			public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
			{
				StreamHelpers.ValidateCopyToArgs(this, destination, bufferSize);
				if (!cancellationToken.IsCancellationRequested)
				{
					return Task.CompletedTask;
				}
				return Task.FromCanceled(cancellationToken);
			}

			protected override void Dispose(bool disposing)
			{
			}

			public override void Flush()
			{
			}

			public override Task FlushAsync(CancellationToken cancellationToken)
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					return Task.CompletedTask;
				}
				return Task.FromCanceled(cancellationToken);
			}

			public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				if (!this.CanRead)
				{
					throw Error.GetReadNotSupported();
				}
				return base.BlockingBeginRead(buffer, offset, count, callback, state);
			}

			public override int EndRead(IAsyncResult asyncResult)
			{
				if (asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				return Stream.BlockingEndRead(asyncResult);
			}

			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				if (!this.CanWrite)
				{
					throw Error.GetWriteNotSupported();
				}
				return base.BlockingBeginWrite(buffer, offset, count, callback, state);
			}

			public override void EndWrite(IAsyncResult asyncResult)
			{
				if (asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				Stream.BlockingEndWrite(asyncResult);
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return 0;
			}

			public override int Read(Span<byte> buffer)
			{
				return 0;
			}

			public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				return Stream.NullStream.s_zeroTask;
			}

			public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
			{
				return new ValueTask<int>(0);
			}

			public override int ReadByte()
			{
				return -1;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
			}

			public override void Write(ReadOnlySpan<byte> buffer)
			{
			}

			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					return Task.CompletedTask;
				}
				return Task.FromCanceled(cancellationToken);
			}

			public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
			{
				if (!cancellationToken.IsCancellationRequested)
				{
					return default(ValueTask);
				}
				return new ValueTask(Task.FromCanceled(cancellationToken));
			}

			public override void WriteByte(byte value)
			{
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return 0L;
			}

			public override void SetLength(long length)
			{
			}

			private static readonly Task<int> s_zeroTask = Task.FromResult<int>(0);
		}

		private sealed class SynchronousAsyncResult : IAsyncResult
		{
			internal SynchronousAsyncResult(int bytesRead, object asyncStateObject)
			{
				this._bytesRead = bytesRead;
				this._stateObject = asyncStateObject;
			}

			internal SynchronousAsyncResult(object asyncStateObject)
			{
				this._stateObject = asyncStateObject;
				this._isWrite = true;
			}

			internal SynchronousAsyncResult(Exception ex, object asyncStateObject, bool isWrite)
			{
				this._exceptionInfo = ExceptionDispatchInfo.Capture(ex);
				this._stateObject = asyncStateObject;
				this._isWrite = isWrite;
			}

			public bool IsCompleted
			{
				get
				{
					return true;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return LazyInitializer.EnsureInitialized<ManualResetEvent>(ref this._waitHandle, () => new ManualResetEvent(true));
				}
			}

			public object AsyncState
			{
				get
				{
					return this._stateObject;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return true;
				}
			}

			internal void ThrowIfError()
			{
				if (this._exceptionInfo != null)
				{
					this._exceptionInfo.Throw();
				}
			}

			internal static int EndRead(IAsyncResult asyncResult)
			{
				Stream.SynchronousAsyncResult synchronousAsyncResult = asyncResult as Stream.SynchronousAsyncResult;
				if (synchronousAsyncResult == null || synchronousAsyncResult._isWrite)
				{
					throw new ArgumentException("IAsyncResult object did not come from the corresponding async method on this type.");
				}
				if (synchronousAsyncResult._endXxxCalled)
				{
					throw new ArgumentException("EndRead can only be called once for each asynchronous operation.");
				}
				synchronousAsyncResult._endXxxCalled = true;
				synchronousAsyncResult.ThrowIfError();
				return synchronousAsyncResult._bytesRead;
			}

			internal static void EndWrite(IAsyncResult asyncResult)
			{
				Stream.SynchronousAsyncResult synchronousAsyncResult = asyncResult as Stream.SynchronousAsyncResult;
				if (synchronousAsyncResult == null || !synchronousAsyncResult._isWrite)
				{
					throw new ArgumentException("IAsyncResult object did not come from the corresponding async method on this type.");
				}
				if (synchronousAsyncResult._endXxxCalled)
				{
					throw new ArgumentException("EndWrite can only be called once for each asynchronous operation.");
				}
				synchronousAsyncResult._endXxxCalled = true;
				synchronousAsyncResult.ThrowIfError();
			}

			private readonly object _stateObject;

			private readonly bool _isWrite;

			private ManualResetEvent _waitHandle;

			private ExceptionDispatchInfo _exceptionInfo;

			private bool _endXxxCalled;

			private int _bytesRead;
		}

		private sealed class SyncStream : Stream, IDisposable
		{
			internal SyncStream(Stream stream)
			{
				if (stream == null)
				{
					throw new ArgumentNullException("stream");
				}
				this._stream = stream;
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
					return this._stream.CanSeek;
				}
			}

			public override bool CanTimeout
			{
				get
				{
					return this._stream.CanTimeout;
				}
			}

			public override long Length
			{
				get
				{
					Stream stream = this._stream;
					long length;
					lock (stream)
					{
						length = this._stream.Length;
					}
					return length;
				}
			}

			public override long Position
			{
				get
				{
					Stream stream = this._stream;
					long position;
					lock (stream)
					{
						position = this._stream.Position;
					}
					return position;
				}
				set
				{
					Stream stream = this._stream;
					lock (stream)
					{
						this._stream.Position = value;
					}
				}
			}

			public override int ReadTimeout
			{
				get
				{
					return this._stream.ReadTimeout;
				}
				set
				{
					this._stream.ReadTimeout = value;
				}
			}

			public override int WriteTimeout
			{
				get
				{
					return this._stream.WriteTimeout;
				}
				set
				{
					this._stream.WriteTimeout = value;
				}
			}

			public override void Close()
			{
				Stream stream = this._stream;
				lock (stream)
				{
					try
					{
						this._stream.Close();
					}
					finally
					{
						base.Dispose(true);
					}
				}
			}

			protected override void Dispose(bool disposing)
			{
				Stream stream = this._stream;
				lock (stream)
				{
					try
					{
						if (disposing)
						{
							((IDisposable)this._stream).Dispose();
						}
					}
					finally
					{
						base.Dispose(disposing);
					}
				}
			}

			public override void Flush()
			{
				Stream stream = this._stream;
				lock (stream)
				{
					this._stream.Flush();
				}
			}

			public override int Read(byte[] bytes, int offset, int count)
			{
				Stream stream = this._stream;
				int result;
				lock (stream)
				{
					result = this._stream.Read(bytes, offset, count);
				}
				return result;
			}

			public override int Read(Span<byte> buffer)
			{
				Stream stream = this._stream;
				int result;
				lock (stream)
				{
					result = this._stream.Read(buffer);
				}
				return result;
			}

			public override int ReadByte()
			{
				Stream stream = this._stream;
				int result;
				lock (stream)
				{
					result = this._stream.ReadByte();
				}
				return result;
			}

			public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				bool flag = this._stream.HasOverriddenBeginEndRead();
				Stream stream = this._stream;
				IAsyncResult result;
				lock (stream)
				{
					result = (flag ? this._stream.BeginRead(buffer, offset, count, callback, state) : this._stream.BeginReadInternal(buffer, offset, count, callback, state, true, true));
				}
				return result;
			}

			public override int EndRead(IAsyncResult asyncResult)
			{
				if (asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				Stream stream = this._stream;
				int result;
				lock (stream)
				{
					result = this._stream.EndRead(asyncResult);
				}
				return result;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				Stream stream = this._stream;
				long result;
				lock (stream)
				{
					result = this._stream.Seek(offset, origin);
				}
				return result;
			}

			public override void SetLength(long length)
			{
				Stream stream = this._stream;
				lock (stream)
				{
					this._stream.SetLength(length);
				}
			}

			public override void Write(byte[] bytes, int offset, int count)
			{
				Stream stream = this._stream;
				lock (stream)
				{
					this._stream.Write(bytes, offset, count);
				}
			}

			public override void Write(ReadOnlySpan<byte> buffer)
			{
				Stream stream = this._stream;
				lock (stream)
				{
					this._stream.Write(buffer);
				}
			}

			public override void WriteByte(byte b)
			{
				Stream stream = this._stream;
				lock (stream)
				{
					this._stream.WriteByte(b);
				}
			}

			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				bool flag = this._stream.HasOverriddenBeginEndWrite();
				Stream stream = this._stream;
				IAsyncResult result;
				lock (stream)
				{
					result = (flag ? this._stream.BeginWrite(buffer, offset, count, callback, state) : this._stream.BeginWriteInternal(buffer, offset, count, callback, state, true, true));
				}
				return result;
			}

			public override void EndWrite(IAsyncResult asyncResult)
			{
				if (asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				Stream stream = this._stream;
				lock (stream)
				{
					this._stream.EndWrite(asyncResult);
				}
			}

			private Stream _stream;
		}
	}
}
