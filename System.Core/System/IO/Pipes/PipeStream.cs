using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	/// <summary>Exposes a <see cref="T:System.IO.Stream" /> object around a pipe, which supports both anonymous and named pipes.</summary>
	public abstract class PipeStream : Stream
	{
		internal static string GetPipePath(string serverName, string pipeName)
		{
			string fullPath = Path.GetFullPath("\\\\" + serverName + "\\pipe\\" + pipeName);
			if (string.Equals(fullPath, "\\\\.\\pipe\\anonymous", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentOutOfRangeException("pipeName", "The pipeName \\\"anonymous\\\" is reserved.");
			}
			return fullPath;
		}

		internal void ValidateHandleIsPipe(SafePipeHandle safePipeHandle)
		{
			if (Interop.Kernel32.GetFileType(safePipeHandle) != 3)
			{
				throw new IOException("Invalid pipe handle.");
			}
		}

		private void InitializeAsyncHandle(SafePipeHandle handle)
		{
			this._threadPoolBinding = ThreadPoolBoundHandle.BindHandle(handle);
		}

		private void DisposeCore(bool disposing)
		{
			if (disposing)
			{
				ThreadPoolBoundHandle threadPoolBinding = this._threadPoolBinding;
				if (threadPoolBinding == null)
				{
					return;
				}
				threadPoolBinding.Dispose();
			}
		}

		private int ReadCore(Span<byte> buffer)
		{
			int num = 0;
			int num2 = this.ReadFileNative(this._handle, buffer, null, out num);
			if (num2 == -1)
			{
				if (num != 109 && num != 233)
				{
					throw Win32Marshal.GetExceptionForWin32Error(num, string.Empty);
				}
				this.State = PipeState.Broken;
				num2 = 0;
			}
			this._isMessageComplete = (num != 234);
			return num2;
		}

		private unsafe Task<int> ReadAsyncCore(Memory<byte> buffer, CancellationToken cancellationToken)
		{
			ReadWriteCompletionSource readWriteCompletionSource = new ReadWriteCompletionSource(this, buffer, false);
			int num = 0;
			if (this.ReadFileNative(this._handle, buffer.Span, readWriteCompletionSource.Overlapped, out num) == -1)
			{
				if (num == 109 || num == 233)
				{
					this.State = PipeState.Broken;
					readWriteCompletionSource.Overlapped->InternalLow = IntPtr.Zero;
					readWriteCompletionSource.ReleaseResources();
					this.UpdateMessageCompletion(true);
					return PipeStream.s_zeroTask;
				}
				if (num != 997)
				{
					throw Win32Marshal.GetExceptionForWin32Error(num, "");
				}
			}
			readWriteCompletionSource.RegisterForCancellation(cancellationToken);
			return readWriteCompletionSource.Task;
		}

		private void WriteCore(ReadOnlySpan<byte> buffer)
		{
			int errorCode = 0;
			if (this.WriteFileNative(this._handle, buffer, null, out errorCode) == -1)
			{
				throw this.WinIOError(errorCode);
			}
		}

		private Task WriteAsyncCore(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
		{
			ReadWriteCompletionSource readWriteCompletionSource = new ReadWriteCompletionSource(this, buffer, true);
			int num = 0;
			if (this.WriteFileNative(this._handle, buffer.Span, readWriteCompletionSource.Overlapped, out num) == -1 && num != 997)
			{
				readWriteCompletionSource.ReleaseResources();
				throw this.WinIOError(num);
			}
			readWriteCompletionSource.RegisterForCancellation(cancellationToken);
			return readWriteCompletionSource.Task;
		}

		/// <summary>Waits for the other end of the pipe to read all sent bytes.</summary>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support write operations.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public void WaitForPipeDrain()
		{
			this.CheckWriteOperations();
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			if (!Interop.Kernel32.FlushFileBuffers(this._handle))
			{
				throw this.WinIOError(Marshal.GetLastWin32Error());
			}
		}

		/// <summary>Gets the pipe transmission mode supported by the current pipe.</summary>
		/// <returns>One of the <see cref="T:System.IO.Pipes.PipeTransmissionMode" /> values that indicates the transmission mode supported by the current pipe.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The handle has not been set.-or-The pipe is waiting to connect in an anonymous client/server operation or with a named client. </exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public virtual PipeTransmissionMode TransmissionMode
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (!this._isFromExistingHandle)
				{
					return this._transmissionMode;
				}
				int num;
				if (!Interop.Kernel32.GetNamedPipeInfo(this._handle, out num, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				if ((num & 4) != 0)
				{
					return PipeTransmissionMode.Message;
				}
				return PipeTransmissionMode.Byte;
			}
		}

		/// <summary>Gets the size, in bytes, of the inbound buffer for a pipe.</summary>
		/// <returns>An integer value that represents the inbound buffer size, in bytes.</returns>
		/// <exception cref="T:System.NotSupportedException">The stream is unreadable.</exception>
		/// <exception cref="T:System.InvalidOperationException">The pipe is waiting to connect.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public virtual int InBufferSize
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (!this.CanRead)
				{
					throw new NotSupportedException("Stream does not support reading.");
				}
				int result;
				if (!Interop.Kernel32.GetNamedPipeInfo(this._handle, IntPtr.Zero, IntPtr.Zero, out result, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				return result;
			}
		}

		/// <summary>Gets the size, in bytes, of the outbound buffer for a pipe.</summary>
		/// <returns>The outbound buffer size, in bytes.</returns>
		/// <exception cref="T:System.NotSupportedException">The stream is unwriteable.</exception>
		/// <exception cref="T:System.InvalidOperationException">The pipe is waiting to connect.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public virtual int OutBufferSize
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (!this.CanWrite)
				{
					throw new NotSupportedException("Stream does not support writing.");
				}
				int outBufferSize;
				if (this._pipeDirection == PipeDirection.Out)
				{
					outBufferSize = this._outBufferSize;
				}
				else if (!Interop.Kernel32.GetNamedPipeInfo(this._handle, IntPtr.Zero, out outBufferSize, IntPtr.Zero, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				return outBufferSize;
			}
		}

		/// <summary>Gets or sets the reading mode for a <see cref="T:System.IO.Pipes.PipeStream" /> object.</summary>
		/// <returns>One of the <see cref="T:System.IO.Pipes.PipeTransmissionMode" /> values that indicates how the <see cref="T:System.IO.Pipes.PipeStream" /> object reads from the pipe.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The supplied value is not a valid <see cref="T:System.IO.Pipes.PipeTransmissionMode" /> value.</exception>
		/// <exception cref="T:System.NotSupportedException">The supplied value is not a supported <see cref="T:System.IO.Pipes.PipeTransmissionMode" /> value for this pipe stream.</exception>
		/// <exception cref="T:System.InvalidOperationException">The handle has not been set.-or-The pipe is waiting to connect with a named client.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or an I/O error occurred with a named client.</exception>
		public unsafe virtual PipeTransmissionMode ReadMode
		{
			get
			{
				this.CheckPipePropertyOperations();
				if (this._isFromExistingHandle || this.IsHandleExposed)
				{
					this.UpdateReadMode();
				}
				return this._readMode;
			}
			set
			{
				this.CheckPipePropertyOperations();
				if (value < PipeTransmissionMode.Byte || value > PipeTransmissionMode.Message)
				{
					throw new ArgumentOutOfRangeException("value", "For named pipes, transmission mode can be TransmissionMode.Byte or PipeTransmissionMode.Message. For anonymous pipes, transmission mode can be TransmissionMode.Byte.");
				}
				int num = (int)((int)value << 1);
				if (!Interop.Kernel32.SetNamedPipeHandleState(this._handle, &num, IntPtr.Zero, IntPtr.Zero))
				{
					throw this.WinIOError(Marshal.GetLastWin32Error());
				}
				this._readMode = value;
			}
		}

		private unsafe int ReadFileNative(SafePipeHandle handle, Span<byte> buffer, NativeOverlapped* overlapped, out int errorCode)
		{
			if (buffer.Length == 0)
			{
				errorCode = 0;
				return 0;
			}
			int result = 0;
			bool flag;
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(buffer))
			{
				byte* bytes = reference;
				flag = ((this._isAsync ? Interop.Kernel32.ReadFile(handle, bytes, buffer.Length, IntPtr.Zero, overlapped) : Interop.Kernel32.ReadFile(handle, bytes, buffer.Length, out result, IntPtr.Zero)) != 0);
			}
			if (flag)
			{
				errorCode = 0;
				return result;
			}
			errorCode = Marshal.GetLastWin32Error();
			if (errorCode != 234)
			{
				return -1;
			}
			return result;
		}

		private unsafe int WriteFileNative(SafePipeHandle handle, ReadOnlySpan<byte> buffer, NativeOverlapped* overlapped, out int errorCode)
		{
			if (buffer.Length == 0)
			{
				errorCode = 0;
				return 0;
			}
			int result = 0;
			bool flag;
			fixed (byte* reference = MemoryMarshal.GetReference<byte>(buffer))
			{
				byte* bytes = reference;
				flag = ((this._isAsync ? Interop.Kernel32.WriteFile(handle, bytes, buffer.Length, IntPtr.Zero, overlapped) : Interop.Kernel32.WriteFile(handle, bytes, buffer.Length, out result, IntPtr.Zero)) != 0);
			}
			if (!flag)
			{
				errorCode = Marshal.GetLastWin32Error();
				return -1;
			}
			errorCode = 0;
			return result;
		}

		internal static Interop.Kernel32.SECURITY_ATTRIBUTES GetSecAttrs(HandleInheritability inheritability)
		{
			Interop.Kernel32.SECURITY_ATTRIBUTES result = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
			if ((inheritability & HandleInheritability.Inheritable) != HandleInheritability.None)
			{
				result = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
				result.nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES);
				result.bInheritHandle = Interop.BOOL.TRUE;
			}
			return result;
		}

		internal unsafe static Interop.Kernel32.SECURITY_ATTRIBUTES GetSecAttrs(HandleInheritability inheritability, PipeSecurity pipeSecurity, ref GCHandle pinningHandle)
		{
			Interop.Kernel32.SECURITY_ATTRIBUTES result = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
			result.nLength = (uint)sizeof(Interop.Kernel32.SECURITY_ATTRIBUTES);
			if ((inheritability & HandleInheritability.Inheritable) != HandleInheritability.None)
			{
				result.bInheritHandle = Interop.BOOL.TRUE;
			}
			if (pipeSecurity != null)
			{
				byte[] securityDescriptorBinaryForm = pipeSecurity.GetSecurityDescriptorBinaryForm();
				pinningHandle = GCHandle.Alloc(securityDescriptorBinaryForm, GCHandleType.Pinned);
				byte[] array;
				byte* value;
				if ((array = securityDescriptorBinaryForm) == null || array.Length == 0)
				{
					value = null;
				}
				else
				{
					value = &array[0];
				}
				result.lpSecurityDescriptor = (IntPtr)((void*)value);
				array = null;
			}
			return result;
		}

		private void UpdateReadMode()
		{
			int num;
			if (!Interop.Kernel32.GetNamedPipeHandleState(this.SafePipeHandle, out num, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0))
			{
				throw this.WinIOError(Marshal.GetLastWin32Error());
			}
			if ((num & 2) != 0)
			{
				this._readMode = PipeTransmissionMode.Message;
				return;
			}
			this._readMode = PipeTransmissionMode.Byte;
		}

		internal Exception WinIOError(int errorCode)
		{
			if (errorCode <= 38)
			{
				if (errorCode != 6)
				{
					if (errorCode == 38)
					{
						return Error.GetEndOfFile();
					}
				}
				else
				{
					this._handle.SetHandleAsInvalid();
					this._state = PipeState.Broken;
				}
			}
			else if (errorCode == 109 || errorCode - 232 <= 1)
			{
				this._state = PipeState.Broken;
				return new IOException("Pipe is broken.", Win32Marshal.MakeHRFromErrorCode(errorCode));
			}
			return Win32Marshal.GetExceptionForWin32Error(errorCode, "");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Pipes.PipeStream" /> class using the specified <see cref="T:System.IO.Pipes.PipeDirection" /> value and buffer size.</summary>
		/// <param name="direction">One of the <see cref="T:System.IO.Pipes.PipeDirection" /> values that indicates the direction of the pipe object.</param>
		/// <param name="bufferSize">A positive <see cref="T:System.Int32" /> value greater than or equal to 0 that indicates the buffer size.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="direction" /> is not a valid <see cref="T:System.IO.Pipes.PipeDirection" /> value.-or-
		///         <paramref name="bufferSize" /> is less than 0.</exception>
		protected PipeStream(PipeDirection direction, int bufferSize)
		{
			if (direction < PipeDirection.In || direction > PipeDirection.InOut)
			{
				throw new ArgumentOutOfRangeException("direction", "For named pipes, the pipe direction can be PipeDirection.In, PipeDirection.Out or PipeDirection.InOut. For anonymous pipes, the pipe direction can be PipeDirection.In or PipeDirection.Out.");
			}
			if (bufferSize < 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Non negative number is required.");
			}
			this.Init(direction, PipeTransmissionMode.Byte, bufferSize);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Pipes.PipeStream" /> class using the specified <see cref="T:System.IO.Pipes.PipeDirection" />, <see cref="T:System.IO.Pipes.PipeTransmissionMode" />, and buffer size.</summary>
		/// <param name="direction">One of the <see cref="T:System.IO.Pipes.PipeDirection" /> values that indicates the direction of the pipe object.</param>
		/// <param name="transmissionMode">One of the <see cref="T:System.IO.Pipes.PipeTransmissionMode" /> values that indicates the transmission mode of the pipe object.</param>
		/// <param name="outBufferSize">A positive <see cref="T:System.Int32" /> value greater than or equal to 0 that indicates the buffer size.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="direction" /> is not a valid <see cref="T:System.IO.Pipes.PipeDirection" /> value.-or-
		///         <paramref name="transmissionMode" /> is not a valid <see cref="T:System.IO.Pipes.PipeTransmissionMode" /> value.-or-
		///         <paramref name="bufferSize" /> is less than 0.</exception>
		protected PipeStream(PipeDirection direction, PipeTransmissionMode transmissionMode, int outBufferSize)
		{
			if (direction < PipeDirection.In || direction > PipeDirection.InOut)
			{
				throw new ArgumentOutOfRangeException("direction", "For named pipes, the pipe direction can be PipeDirection.In, PipeDirection.Out or PipeDirection.InOut. For anonymous pipes, the pipe direction can be PipeDirection.In or PipeDirection.Out.");
			}
			if (transmissionMode < PipeTransmissionMode.Byte || transmissionMode > PipeTransmissionMode.Message)
			{
				throw new ArgumentOutOfRangeException("transmissionMode", "For named pipes, transmission mode can be TransmissionMode.Byte or PipeTransmissionMode.Message. For anonymous pipes, transmission mode can be TransmissionMode.Byte.");
			}
			if (outBufferSize < 0)
			{
				throw new ArgumentOutOfRangeException("outBufferSize", "Non negative number is required.");
			}
			this.Init(direction, transmissionMode, outBufferSize);
		}

		private void Init(PipeDirection direction, PipeTransmissionMode transmissionMode, int outBufferSize)
		{
			this._readMode = transmissionMode;
			this._transmissionMode = transmissionMode;
			this._pipeDirection = direction;
			if ((this._pipeDirection & PipeDirection.In) != (PipeDirection)0)
			{
				this._canRead = true;
			}
			if ((this._pipeDirection & PipeDirection.Out) != (PipeDirection)0)
			{
				this._canWrite = true;
			}
			this._outBufferSize = outBufferSize;
			this._isMessageComplete = true;
			this._state = PipeState.WaitingToConnect;
		}

		/// <summary>Initializes a <see cref="T:System.IO.Pipes.PipeStream" /> object from the specified <see cref="T:Microsoft.Win32.SafeHandles.SafePipeHandle" /> object.</summary>
		/// <param name="handle">The <see cref="T:Microsoft.Win32.SafeHandles.SafePipeHandle" /> object of the pipe to initialize.</param>
		/// <param name="isExposed">
		///       <see langword="true" /> to expose the handle; otherwise, <see langword="false" />.</param>
		/// <param name="isAsync">
		///       <see langword="true" /> to indicate that the handle was opened asynchronously; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.IO.IOException">A handle cannot be bound to the pipe.</exception>
		protected void InitializeHandle(SafePipeHandle handle, bool isExposed, bool isAsync)
		{
			if (isAsync && handle != null)
			{
				this.InitializeAsyncHandle(handle);
			}
			this._handle = handle;
			this._isAsync = isAsync;
			this._isHandleExposed = isExposed;
			this._isFromExistingHandle = isExposed;
		}

		/// <summary>Reads a block of bytes from a stream and writes the data to a specified buffer.</summary>
		/// <param name="buffer">When this method returns, contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The byte offset in the <paramref name="buffer" /> array at which the bytes that are read will be placed.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <returns>The total number of bytes that are read into <paramref name="buffer" />. This might be less than the number of bytes requested if that number of bytes is not currently available, or 0 if the end of the stream is reached.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="offset" /> is less than 0.-or-
		///         <paramref name="count" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="count" /> is greater than the number of bytes available in <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support read operations.</exception>
		/// <exception cref="T:System.InvalidOperationException">The pipe is disconnected, waiting to connect, or the handle has not been set.</exception>
		/// <exception cref="T:System.IO.IOException">Any I/O error occurred.</exception>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this._isAsync)
			{
				return this.ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
			}
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			this.CheckReadOperations();
			return this.ReadCore(new Span<byte>(buffer, offset, count));
		}

		public override int Read(Span<byte> buffer)
		{
			if (this._isAsync)
			{
				return base.Read(buffer);
			}
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			this.CheckReadOperations();
			return this.ReadCore(buffer);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			this.CheckReadOperations();
			if (!this._isAsync)
			{
				return base.ReadAsync(buffer, offset, count, cancellationToken);
			}
			if (count == 0)
			{
				this.UpdateMessageCompletion(false);
				return PipeStream.s_zeroTask;
			}
			return this.ReadAsyncCore(new Memory<byte>(buffer, offset, count), cancellationToken);
		}

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._isAsync)
			{
				return base.ReadAsync(buffer, cancellationToken);
			}
			if (!this.CanRead)
			{
				throw Error.GetReadNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			this.CheckReadOperations();
			if (buffer.Length == 0)
			{
				this.UpdateMessageCompletion(false);
				return new ValueTask<int>(0);
			}
			return new ValueTask<int>(this.ReadAsyncCore(buffer, cancellationToken));
		}

		/// <summary>Begins an asynchronous read operation.</summary>
		/// <param name="buffer">The buffer to read data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin reading.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="callback">The method to call when the asynchronous read operation is completed.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that references the asynchronous read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="offset" /> is less than 0.-or-
		///         <paramref name="count" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="count" /> is greater than the number of bytes available in <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support read operations.</exception>
		/// <exception cref="T:System.InvalidOperationException">The pipe is disconnected, waiting to connect, or the handle has not been set.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this._isAsync)
			{
				return TaskToApm.Begin(this.ReadAsync(buffer, offset, count, CancellationToken.None), callback, state);
			}
			return base.BeginRead(buffer, offset, count, callback, state);
		}

		/// <summary>Ends a pending asynchronous read request.</summary>
		/// <param name="asyncResult">The reference to the pending asynchronous request.</param>
		/// <returns>The number of bytes that were read. A return value of 0 indicates the end of the stream (the pipe has been closed).</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="asyncResult" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="asyncResult" /> did not originate from a <see cref="M:System.IO.Pipes.PipeStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> method on the current stream. </exception>
		/// <exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception>
		public override int EndRead(IAsyncResult asyncResult)
		{
			if (this._isAsync)
			{
				return TaskToApm.End<int>(asyncResult);
			}
			return base.EndRead(asyncResult);
		}

		/// <summary>Writes a block of bytes to the current stream using data from a buffer.</summary>
		/// <param name="buffer">The buffer that contains data to write to the pipe.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The maximum number of bytes to write to the current stream.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="offset" /> is less than 0.-or-
		///         <paramref name="count" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="count" /> is greater than the number of bytes available in <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support write operations.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this._isAsync)
			{
				this.WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
				return;
			}
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			this.CheckWriteOperations();
			this.WriteCore(new ReadOnlySpan<byte>(buffer, offset, count));
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			if (this._isAsync)
			{
				base.Write(buffer);
				return;
			}
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			this.CheckWriteOperations();
			this.WriteCore(buffer);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckReadWriteArgs(buffer, offset, count);
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			this.CheckWriteOperations();
			if (!this._isAsync)
			{
				return base.WriteAsync(buffer, offset, count, cancellationToken);
			}
			if (count == 0)
			{
				return Task.CompletedTask;
			}
			return this.WriteAsyncCore(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken);
		}

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!this._isAsync)
			{
				return base.WriteAsync(buffer, cancellationToken);
			}
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask(Task.FromCanceled<int>(cancellationToken));
			}
			this.CheckWriteOperations();
			if (buffer.Length == 0)
			{
				return default(ValueTask);
			}
			return new ValueTask(this.WriteAsyncCore(buffer, cancellationToken));
		}

		/// <summary>Begins an asynchronous write operation.</summary>
		/// <param name="buffer">The buffer that contains the data to write to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <param name="callback">The method to call when the asynchronous write operation is completed.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that references the asynchronous write operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="offset" /> is less than 0.-or-
		///         <paramref name="count" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="count" /> is greater than the number of bytes available in <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support write operations.</exception>
		/// <exception cref="T:System.InvalidOperationException">The pipe is disconnected, waiting to connect, or the handle has not been set.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (this._isAsync)
			{
				return TaskToApm.Begin(this.WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);
			}
			return base.BeginWrite(buffer, offset, count, callback, state);
		}

		/// <summary>Ends a pending asynchronous write request.</summary>
		/// <param name="asyncResult">The reference to the pending asynchronous request.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="asyncResult" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="asyncResult" /> did not originate from a <see cref="M:System.IO.Pipes.PipeStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> method on the current stream. </exception>
		/// <exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (this._isAsync)
			{
				TaskToApm.End(asyncResult);
				return;
			}
			base.EndWrite(asyncResult);
		}

		private void CheckReadWriteArgs(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non negative number is required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non negative number is required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
		}

		[Conditional("DEBUG")]
		private static void DebugAssertHandleValid(SafePipeHandle handle)
		{
		}

		[Conditional("DEBUG")]
		private static void DebugAssertReadWriteArgs(byte[] buffer, int offset, int count, SafePipeHandle handle)
		{
		}

		/// <summary>Reads a byte from a pipe.</summary>
		/// <returns>The byte, cast to <see cref="T:System.Int32" />, or -1 indicates the end of the stream (the pipe has been closed).</returns>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support read operations.</exception>
		/// <exception cref="T:System.InvalidOperationException">The pipe is disconnected, waiting to connect, or the handle has not been set.</exception>
		/// <exception cref="T:System.IO.IOException">Any I/O error occurred.</exception>
		public unsafe override int ReadByte()
		{
			byte result;
			if (this.Read(new Span<byte>((void*)(&result), 1)) <= 0)
			{
				return -1;
			}
			return (int)result;
		}

		/// <summary>Writes a byte to the current stream.</summary>
		/// <param name="value">The byte to write to the stream.</param>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support write operations.</exception>
		/// <exception cref="T:System.InvalidOperationException">The pipe is disconnected, waiting to connect, or the handle has not been set.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public unsafe override void WriteByte(byte value)
		{
			this.Write(new ReadOnlySpan<byte>((void*)(&value), 1));
		}

		/// <summary>Clears the buffer for the current stream and causes any buffered data to be written to the underlying device.</summary>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The pipe does not support write operations.</exception>
		/// <exception cref="T:System.IO.IOException">The pipe is broken or another I/O error occurred.</exception>
		public override void Flush()
		{
			this.CheckWriteOperations();
			if (!this.CanWrite)
			{
				throw Error.GetWriteNotSupported();
			}
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Pipes.PipeStream" /> class and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///       <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this._handle != null && !this._handle.IsClosed)
				{
					this._handle.Dispose();
				}
				this.DisposeCore(disposing);
			}
			finally
			{
				base.Dispose(disposing);
			}
			this._state = PipeState.Closed;
		}

		/// <summary>Gets or sets a value indicating whether a <see cref="T:System.IO.Pipes.PipeStream" /> object is connected.</summary>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.IO.Pipes.PipeStream" /> object is connected; otherwise, <see langword="false" />.</returns>
		public bool IsConnected
		{
			get
			{
				return this.State == PipeState.Connected;
			}
			protected set
			{
				this._state = (value ? PipeState.Connected : PipeState.Disconnected);
			}
		}

		/// <summary>Gets a value indicating whether a <see cref="T:System.IO.Pipes.PipeStream" /> object was opened asynchronously or synchronously.</summary>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.IO.Pipes.PipeStream" /> object was opened asynchronously; otherwise, <see langword="false" />.</returns>
		public bool IsAsync
		{
			get
			{
				return this._isAsync;
			}
		}

		/// <summary>Gets a value indicating whether there is more data in the message returned from the most recent read operation.</summary>
		/// <returns>
		///     <see langword="true" /> if there are no more characters to read in the message; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The pipe is not connected.-or-The pipe handle has not been set.-or-The pipe's <see cref="P:System.IO.Pipes.PipeStream.ReadMode" /> property value is not <see cref="F:System.IO.Pipes.PipeTransmissionMode.Message" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		public bool IsMessageComplete
		{
			get
			{
				if (this._state == PipeState.WaitingToConnect)
				{
					throw new InvalidOperationException("Pipe hasn't been connected yet.");
				}
				if (this._state == PipeState.Disconnected)
				{
					throw new InvalidOperationException("Pipe is in a disconnected state.");
				}
				if (this._handle == null)
				{
					throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
				}
				if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
				{
					throw Error.GetPipeNotOpen();
				}
				if (this._readMode != PipeTransmissionMode.Message)
				{
					throw new InvalidOperationException("ReadMode is not of PipeTransmissionMode.Message.");
				}
				return this._isMessageComplete;
			}
		}

		internal void UpdateMessageCompletion(bool completion)
		{
			this._isMessageComplete = (completion || this._state == PipeState.Broken);
		}

		/// <summary>Gets the safe handle for the local end of the pipe that the current <see cref="T:System.IO.Pipes.PipeStream" /> object encapsulates.</summary>
		/// <returns>A <see cref="T:Microsoft.Win32.SafeHandles.SafePipeHandle" /> object for the pipe that is encapsulated by the current <see cref="T:System.IO.Pipes.PipeStream" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The pipe handle has not been set.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		public SafePipeHandle SafePipeHandle
		{
			get
			{
				if (this._handle == null)
				{
					throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
				}
				if (this._handle.IsClosed)
				{
					throw Error.GetPipeNotOpen();
				}
				this._isHandleExposed = true;
				return this._handle;
			}
		}

		internal SafePipeHandle InternalHandle
		{
			get
			{
				return this._handle;
			}
		}

		/// <summary>Gets a value indicating whether a handle to a <see cref="T:System.IO.Pipes.PipeStream" /> object is exposed.</summary>
		/// <returns>
		///     <see langword="true" /> if a handle to the <see cref="T:System.IO.Pipes.PipeStream" /> object is exposed; otherwise, <see langword="false" />.</returns>
		protected bool IsHandleExposed
		{
			get
			{
				return this._isHandleExposed;
			}
		}

		/// <summary>Gets a value indicating whether the current stream supports read operations.</summary>
		/// <returns>
		///     <see langword="true" /> if the stream supports read operations; otherwise, <see langword="false" />.</returns>
		public override bool CanRead
		{
			get
			{
				return this._canRead;
			}
		}

		/// <summary>Gets a value indicating whether the current stream supports write operations.</summary>
		/// <returns>
		///     <see langword="true" /> if the stream supports write operations; otherwise, <see langword="false" />.</returns>
		public override bool CanWrite
		{
			get
			{
				return this._canWrite;
			}
		}

		/// <summary>Gets a value indicating whether the current stream supports seek operations.</summary>
		/// <returns>
		///     <see langword="false" /> in all cases.</returns>
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets the length of a stream, in bytes.</summary>
		/// <returns>0 in all cases.</returns>
		/// <exception cref="T:System.NotSupportedException">Always thrown.</exception>
		public override long Length
		{
			get
			{
				throw Error.GetSeekNotSupported();
			}
		}

		/// <summary>Gets or sets the current position of the current stream.</summary>
		/// <returns>0 in all cases.</returns>
		/// <exception cref="T:System.NotSupportedException">Always thrown.</exception>
		public override long Position
		{
			get
			{
				throw Error.GetSeekNotSupported();
			}
			set
			{
				throw Error.GetSeekNotSupported();
			}
		}

		/// <summary>Sets the length of the current stream to the specified value.</summary>
		/// <param name="value">The new length of the stream.</param>
		public override void SetLength(long value)
		{
			throw Error.GetSeekNotSupported();
		}

		/// <summary>Sets the current position of the current stream to the specified value.</summary>
		/// <param name="offset">The point, relative to <paramref name="origin" />, to begin seeking from.</param>
		/// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for <paramref name="offset" />, using a value of type <see cref="T:System.IO.SeekOrigin" />.</param>
		/// <returns>The new position in the stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw Error.GetSeekNotSupported();
		}

		/// <summary>Verifies that the pipe is in a proper state for getting or setting properties.</summary>
		protected internal virtual void CheckPipePropertyOperations()
		{
			if (this._handle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
			{
				throw Error.GetPipeNotOpen();
			}
		}

		/// <summary>Verifies that the pipe is in a connected state for read operations.</summary>
		protected internal void CheckReadOperations()
		{
			if (this._state == PipeState.WaitingToConnect)
			{
				throw new InvalidOperationException("Pipe hasn't been connected yet.");
			}
			if (this._state == PipeState.Disconnected)
			{
				throw new InvalidOperationException("Pipe is in a disconnected state.");
			}
			if (this._handle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
			{
				throw Error.GetPipeNotOpen();
			}
		}

		/// <summary>Verifies that the pipe is in a connected state for write operations.</summary>
		protected internal void CheckWriteOperations()
		{
			if (this._state == PipeState.WaitingToConnect)
			{
				throw new InvalidOperationException("Pipe hasn't been connected yet.");
			}
			if (this._state == PipeState.Disconnected)
			{
				throw new InvalidOperationException("Pipe is in a disconnected state.");
			}
			if (this._handle == null)
			{
				throw new InvalidOperationException("Pipe handle has not been set.  Did your PipeStream implementation call InitializeHandle?");
			}
			if (this._state == PipeState.Broken)
			{
				throw new IOException("Pipe is broken.");
			}
			if (this._state == PipeState.Closed || (this._handle != null && this._handle.IsClosed))
			{
				throw Error.GetPipeNotOpen();
			}
		}

		internal PipeState State
		{
			get
			{
				return this._state;
			}
			set
			{
				this._state = value;
			}
		}

		internal bool IsCurrentUserOnly
		{
			get
			{
				return this._isCurrentUserOnly;
			}
			set
			{
				this._isCurrentUserOnly = value;
			}
		}

		/// <summary>Gets a <see cref="T:System.IO.Pipes.PipeSecurity" /> object that encapsulates the access control list (ACL) entries for the pipe described by the current <see cref="T:System.IO.Pipes.PipeStream" /> object.</summary>
		/// <returns>A <see cref="T:System.IO.Pipes.PipeSecurity" /> object that encapsulates the access control list (ACL) entries for the pipe described by the current <see cref="T:System.IO.Pipes.PipeStream" /> object.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The underlying call to set security information failed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying call to set security information failed.</exception>
		/// <exception cref="T:System.NotSupportedException">The underlying call to set security information failed.</exception>
		public PipeSecurity GetAccessControl()
		{
			if (this.State == PipeState.Closed)
			{
				throw Error.GetPipeNotOpen();
			}
			return new PipeSecurity(this.SafePipeHandle, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		/// <summary>Applies the access control list (ACL) entries specified by a <see cref="T:System.IO.Pipes.PipeSecurity" /> object to the pipe specified by the current <see cref="T:System.IO.Pipes.PipeStream" /> object.</summary>
		/// <param name="pipeSecurity">A <see cref="T:System.IO.Pipes.PipeSecurity" /> object that specifies an access control list (ACL) entry to apply to the current pipe.</param>
		/// <exception cref="T:System.ObjectDisposedException">The pipe is closed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="pipeSecurity" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The underlying call to set security information failed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying call to set security information failed.</exception>
		/// <exception cref="T:System.NotSupportedException">The underlying call to set security information failed.</exception>
		public void SetAccessControl(PipeSecurity pipeSecurity)
		{
			if (pipeSecurity == null)
			{
				throw new ArgumentNullException("pipeSecurity");
			}
			this.CheckPipePropertyOperations();
			pipeSecurity.Persist(this.SafePipeHandle);
		}

		internal const bool CheckOperationsRequiresSetHandle = true;

		internal ThreadPoolBoundHandle _threadPoolBinding;

		internal const string AnonymousPipeName = "anonymous";

		private static readonly Task<int> s_zeroTask = Task.FromResult<int>(0);

		private SafePipeHandle _handle;

		private bool _canRead;

		private bool _canWrite;

		private bool _isAsync;

		private bool _isCurrentUserOnly;

		private bool _isMessageComplete;

		private bool _isFromExistingHandle;

		private bool _isHandleExposed;

		private PipeTransmissionMode _readMode;

		private PipeTransmissionMode _transmissionMode;

		private PipeDirection _pipeDirection;

		private int _outBufferSize;

		private PipeState _state;
	}
}
