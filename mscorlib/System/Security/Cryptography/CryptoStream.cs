using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Security.Cryptography
{
	/// <summary>Defines a stream that links data streams to cryptographic transformations.</summary>
	public class CryptoStream : Stream, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptoStream" /> class with a target data stream, the transformation to use, and the mode of the stream.</summary>
		/// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
		/// <param name="transform">The cryptographic transformation that is to be performed on the stream.</param>
		/// <param name="mode">One of the <see cref="T:System.Security.Cryptography.CryptoStreamMode" /> values.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="stream" /> is invalid.</exception>
		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode) : this(stream, transform, mode, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CryptoStream" /> class.</summary>
		/// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
		/// <param name="transform">The cryptographic transformation that is to be performed on the stream.</param>
		/// <param name="mode">The mode of the stream.</param>
		/// <param name="leaveOpen">
		///   <see langword="true" /> to not close the underlying stream when the <see cref="T:System.Security.Cryptography.CryptoStream" /> object is disposed; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="mode" /> is invalid.</exception>
		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode, bool leaveOpen)
		{
			this._stream = stream;
			this._transformMode = mode;
			this._transform = transform;
			this._leaveOpen = leaveOpen;
			CryptoStreamMode transformMode = this._transformMode;
			if (transformMode != CryptoStreamMode.Read)
			{
				if (transformMode != CryptoStreamMode.Write)
				{
					throw new ArgumentException("Argument {0} should be larger than {1}.");
				}
				if (!this._stream.CanWrite)
				{
					throw new ArgumentException(SR.Format("Stream was not writable.", "stream"));
				}
				this._canWrite = true;
			}
			else
			{
				if (!this._stream.CanRead)
				{
					throw new ArgumentException(SR.Format("Stream was not readable.", "stream"));
				}
				this._canRead = true;
			}
			this.InitializeBuffer();
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Security.Cryptography.CryptoStream" /> is readable.</summary>
		/// <returns>
		///   <see langword="true" /> if the current stream is readable; otherwise, <see langword="false" />.</returns>
		public override bool CanRead
		{
			get
			{
				return this._canRead;
			}
		}

		/// <summary>Gets a value indicating whether you can seek within the current <see cref="T:System.Security.Cryptography.CryptoStream" />.</summary>
		/// <returns>Always <see langword="false" />.</returns>
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Security.Cryptography.CryptoStream" /> is writable.</summary>
		/// <returns>
		///   <see langword="true" /> if the current stream is writable; otherwise, <see langword="false" />.</returns>
		public override bool CanWrite
		{
			get
			{
				return this._canWrite;
			}
		}

		/// <summary>Gets the length in bytes of the stream.</summary>
		/// <returns>This property is not supported.</returns>
		/// <exception cref="T:System.NotSupportedException">This property is not supported.</exception>
		public override long Length
		{
			get
			{
				throw new NotSupportedException("Stream does not support seeking.");
			}
		}

		/// <summary>Gets or sets the position within the current stream.</summary>
		/// <returns>This property is not supported.</returns>
		/// <exception cref="T:System.NotSupportedException">This property is not supported.</exception>
		public override long Position
		{
			get
			{
				throw new NotSupportedException("Stream does not support seeking.");
			}
			set
			{
				throw new NotSupportedException("Stream does not support seeking.");
			}
		}

		/// <summary>Gets a value indicating whether the final buffer block has been written to the underlying stream.</summary>
		/// <returns>
		///   <see langword="true" /> if the final block has been flushed; otherwise, <see langword="false" />.</returns>
		public bool HasFlushedFinalBlock
		{
			get
			{
				return this._finalBlockTransformed;
			}
		}

		/// <summary>Updates the underlying data source or repository with the current state of the buffer, then clears the buffer.</summary>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The key is corrupt which can cause invalid padding to the stream.</exception>
		/// <exception cref="T:System.NotSupportedException">The current stream is not writable.  
		///  -or-  
		///  The final block has already been transformed.</exception>
		public void FlushFinalBlock()
		{
			if (this._finalBlockTransformed)
			{
				throw new NotSupportedException("FlushFinalBlock() method was called twice on a CryptoStream. It can only be called once.");
			}
			byte[] array = this._transform.TransformFinalBlock(this._inputBuffer, 0, this._inputBufferIndex);
			this._finalBlockTransformed = true;
			if (this._canWrite && this._outputBufferIndex > 0)
			{
				this._stream.Write(this._outputBuffer, 0, this._outputBufferIndex);
				this._outputBufferIndex = 0;
			}
			if (this._canWrite)
			{
				this._stream.Write(array, 0, array.Length);
			}
			CryptoStream cryptoStream = this._stream as CryptoStream;
			if (cryptoStream != null)
			{
				if (!cryptoStream.HasFlushedFinalBlock)
				{
					cryptoStream.FlushFinalBlock();
				}
			}
			else
			{
				this._stream.Flush();
			}
			if (this._inputBuffer != null)
			{
				Array.Clear(this._inputBuffer, 0, this._inputBuffer.Length);
			}
			if (this._outputBuffer != null)
			{
				Array.Clear(this._outputBuffer, 0, this._outputBuffer.Length);
			}
		}

		/// <summary>Clears all buffers for the current stream and causes any buffered data to be written to the underlying device.</summary>
		public override void Flush()
		{
		}

		/// <summary>Clears all buffers for the current stream asynchronously, causes any buffered data to be written to the underlying device, and monitors cancellation requests.</summary>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
		/// <returns>A task that represents the asynchronous flush operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			if (base.GetType() != typeof(CryptoStream))
			{
				return base.FlushAsync(cancellationToken);
			}
			if (!cancellationToken.IsCancellationRequested)
			{
				return Task.CompletedTask;
			}
			return Task.FromCanceled(cancellationToken);
		}

		/// <summary>Sets the position within the current stream.</summary>
		/// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
		/// <param name="origin">A <see cref="T:System.IO.SeekOrigin" /> object indicating the reference point used to obtain the new position.</param>
		/// <returns>This method is not supported.</returns>
		/// <exception cref="T:System.NotSupportedException">This method is not supported.</exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Stream does not support seeking.");
		}

		/// <summary>Sets the length of the current stream.</summary>
		/// <param name="value">The desired length of the current stream in bytes.</param>
		/// <exception cref="T:System.NotSupportedException">This property exists only to support inheritance from <see cref="T:System.IO.Stream" />, and cannot be used.</exception>
		public override void SetLength(long value)
		{
			throw new NotSupportedException("Stream does not support seeking.");
		}

		/// <summary>Reads a sequence of bytes from the current stream asynchronously, advances the position within the stream by the number of bytes read, and monitors cancellation requests.</summary>
		/// <param name="buffer">The buffer to write the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin writing data from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="P:System.Threading.CancellationToken.None" />.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the task object's <paramref name="TResult" /> parameter contains the total number of bytes read into the buffer. The result can be less than the number of bytes requested if the number of bytes currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
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
			this.CheckReadArguments(buffer, offset, count);
			return this.ReadAsyncInternal(buffer, offset, count, cancellationToken);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(this.ReadAsync(buffer, offset, count, CancellationToken.None), callback, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		private Task<int> ReadAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			CryptoStream.<ReadAsyncInternal>d__37 <ReadAsyncInternal>d__;
			<ReadAsyncInternal>d__.<>4__this = this;
			<ReadAsyncInternal>d__.buffer = buffer;
			<ReadAsyncInternal>d__.offset = offset;
			<ReadAsyncInternal>d__.count = count;
			<ReadAsyncInternal>d__.cancellationToken = cancellationToken;
			<ReadAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadAsyncInternal>d__.<>1__state = -1;
			<ReadAsyncInternal>d__.<>t__builder.Start<CryptoStream.<ReadAsyncInternal>d__37>(ref <ReadAsyncInternal>d__);
			return <ReadAsyncInternal>d__.<>t__builder.Task;
		}

		public override int ReadByte()
		{
			if (this._outputBufferIndex > 1)
			{
				int result = (int)this._outputBuffer[0];
				Buffer.BlockCopy(this._outputBuffer, 1, this._outputBuffer, 0, this._outputBufferIndex - 1);
				this._outputBufferIndex--;
				return result;
			}
			return base.ReadByte();
		}

		public override void WriteByte(byte value)
		{
			if (this._inputBufferIndex + 1 < this._inputBlockSize)
			{
				byte[] inputBuffer = this._inputBuffer;
				int inputBufferIndex = this._inputBufferIndex;
				this._inputBufferIndex = inputBufferIndex + 1;
				inputBuffer[inputBufferIndex] = value;
				return;
			}
			base.WriteByte(value);
		}

		/// <summary>Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
		/// <param name="buffer">An array of bytes. A maximum of <paramref name="count" /> bytes are read from the current stream and stored in <paramref name="buffer" />.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero if the end of the stream has been reached.</returns>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Security.Cryptography.CryptoStreamMode" /> associated with current <see cref="T:System.Security.Cryptography.CryptoStream" /> object does not match the underlying stream.  For example, this exception is thrown when using <see cref="F:System.Security.Cryptography.CryptoStreamMode.Read" /> with an underlying stream that is write only.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> parameter is less than zero.  
		///  -or-  
		///  The <paramref name="count" /> parameter is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">Thesum of the <paramref name="count" /> and <paramref name="offset" /> parameters is longer than the length of the buffer.</exception>
		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckReadArguments(buffer, offset, count);
			return this.ReadAsyncCore(buffer, offset, count, default(CancellationToken), false).GetAwaiter().GetResult();
		}

		private void CheckReadArguments(byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("Stream does not support reading.");
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
		}

		private Task<int> ReadAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool useAsync)
		{
			CryptoStream.<ReadAsyncCore>d__42 <ReadAsyncCore>d__;
			<ReadAsyncCore>d__.<>4__this = this;
			<ReadAsyncCore>d__.buffer = buffer;
			<ReadAsyncCore>d__.offset = offset;
			<ReadAsyncCore>d__.count = count;
			<ReadAsyncCore>d__.cancellationToken = cancellationToken;
			<ReadAsyncCore>d__.useAsync = useAsync;
			<ReadAsyncCore>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadAsyncCore>d__.<>1__state = -1;
			<ReadAsyncCore>d__.<>t__builder.Start<CryptoStream.<ReadAsyncCore>d__42>(ref <ReadAsyncCore>d__);
			return <ReadAsyncCore>d__.<>t__builder.Task;
		}

		/// <summary>Writes a sequence of bytes to the current stream asynchronously, advances the current position within the stream by the number of bytes written, and monitors cancellation requests.</summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> from which to begin writing bytes to the stream.</param>
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
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			this.CheckWriteArguments(buffer, offset, count);
			return this.WriteAsyncInternal(buffer, offset, count, cancellationToken);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			return TaskToApm.Begin(this.WriteAsync(buffer, offset, count, CancellationToken.None), callback, state);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		private Task WriteAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			CryptoStream.<WriteAsyncInternal>d__46 <WriteAsyncInternal>d__;
			<WriteAsyncInternal>d__.<>4__this = this;
			<WriteAsyncInternal>d__.buffer = buffer;
			<WriteAsyncInternal>d__.offset = offset;
			<WriteAsyncInternal>d__.count = count;
			<WriteAsyncInternal>d__.cancellationToken = cancellationToken;
			<WriteAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteAsyncInternal>d__.<>1__state = -1;
			<WriteAsyncInternal>d__.<>t__builder.Start<CryptoStream.<WriteAsyncInternal>d__46>(ref <WriteAsyncInternal>d__);
			return <WriteAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Writes a sequence of bytes to the current <see cref="T:System.Security.Cryptography.CryptoStream" /> and advances the current position within the stream by the number of bytes written.</summary>
		/// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Security.Cryptography.CryptoStreamMode" /> associated with current <see cref="T:System.Security.Cryptography.CryptoStream" /> object does not match the underlying stream.  For example, this exception is thrown when using <see cref="F:System.Security.Cryptography.CryptoStreamMode.Write" /> with an underlying stream that is read only.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="offset" /> parameter is less than zero.  
		///  -or-  
		///  The <paramref name="count" /> parameter is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of the <paramref name="count" /> and <paramref name="offset" /> parameters is longer than the length of the buffer.</exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.CheckWriteArguments(buffer, offset, count);
			this.WriteAsyncCore(buffer, offset, count, default(CancellationToken), false).GetAwaiter().GetResult();
		}

		private void CheckWriteArguments(byte[] buffer, int offset, int count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing.");
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
		}

		private Task WriteAsyncCore(byte[] buffer, int offset, int count, CancellationToken cancellationToken, bool useAsync)
		{
			CryptoStream.<WriteAsyncCore>d__49 <WriteAsyncCore>d__;
			<WriteAsyncCore>d__.<>4__this = this;
			<WriteAsyncCore>d__.buffer = buffer;
			<WriteAsyncCore>d__.offset = offset;
			<WriteAsyncCore>d__.count = count;
			<WriteAsyncCore>d__.cancellationToken = cancellationToken;
			<WriteAsyncCore>d__.useAsync = useAsync;
			<WriteAsyncCore>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteAsyncCore>d__.<>1__state = -1;
			<WriteAsyncCore>d__.<>t__builder.Start<CryptoStream.<WriteAsyncCore>d__49>(ref <WriteAsyncCore>d__);
			return <WriteAsyncCore>d__.<>t__builder.Task;
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Security.Cryptography.CryptoStream" />.</summary>
		public void Clear()
		{
			this.Close();
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Security.Cryptography.CryptoStream" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (!this._finalBlockTransformed)
					{
						this.FlushFinalBlock();
					}
					if (!this._leaveOpen)
					{
						this._stream.Dispose();
					}
				}
			}
			finally
			{
				try
				{
					this._finalBlockTransformed = true;
					if (this._inputBuffer != null)
					{
						Array.Clear(this._inputBuffer, 0, this._inputBuffer.Length);
					}
					if (this._outputBuffer != null)
					{
						Array.Clear(this._outputBuffer, 0, this._outputBuffer.Length);
					}
					this._inputBuffer = null;
					this._outputBuffer = null;
					this._canRead = false;
					this._canWrite = false;
				}
				finally
				{
					base.Dispose(disposing);
				}
			}
		}

		private void InitializeBuffer()
		{
			if (this._transform != null)
			{
				this._inputBlockSize = this._transform.InputBlockSize;
				this._inputBuffer = new byte[this._inputBlockSize];
				this._outputBlockSize = this._transform.OutputBlockSize;
				this._outputBuffer = new byte[this._outputBlockSize];
			}
		}

		private SemaphoreSlim AsyncActiveSemaphore
		{
			get
			{
				return LazyInitializer.EnsureInitialized<SemaphoreSlim>(ref this._lazyAsyncActiveSemaphore, () => new SemaphoreSlim(1, 1));
			}
		}

		private readonly Stream _stream;

		private readonly ICryptoTransform _transform;

		private readonly CryptoStreamMode _transformMode;

		private byte[] _inputBuffer;

		private int _inputBufferIndex;

		private int _inputBlockSize;

		private byte[] _outputBuffer;

		private int _outputBufferIndex;

		private int _outputBlockSize;

		private bool _canRead;

		private bool _canWrite;

		private bool _finalBlockTransformed;

		private SemaphoreSlim _lazyAsyncActiveSemaphore;

		private readonly bool _leaveOpen;
	}
}
