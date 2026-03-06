using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
	/// <summary>Provides methods and properties for compressing and decompressing streams by using the Deflate algorithm.</summary>
	public class DeflateStream : Stream
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.DeflateStream" /> class by using the specified stream and compression mode.</summary>
		/// <param name="stream">The stream to compress or decompress.</param>
		/// <param name="mode">One of the enumeration values that indicates whether to compress or decompress the stream.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="mode" /> is not a valid <see cref="T:System.IO.Compression.CompressionMode" /> value.  
		/// -or-  
		/// <see cref="T:System.IO.Compression.CompressionMode" /> is <see cref="F:System.IO.Compression.CompressionMode.Compress" /> and <see cref="P:System.IO.Stream.CanWrite" /> is <see langword="false" />.  
		/// -or-  
		/// <see cref="T:System.IO.Compression.CompressionMode" /> is <see cref="F:System.IO.Compression.CompressionMode.Decompress" /> and <see cref="P:System.IO.Stream.CanRead" /> is <see langword="false" />.</exception>
		public DeflateStream(Stream stream, CompressionMode mode) : this(stream, mode, false, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.DeflateStream" /> class by using the specified stream and compression mode, and optionally leaves the stream open.</summary>
		/// <param name="stream">The stream to compress or decompress.</param>
		/// <param name="mode">One of the enumeration values that indicates whether to compress or decompress the stream.</param>
		/// <param name="leaveOpen">
		///   <see langword="true" /> to leave the stream open after disposing the <see cref="T:System.IO.Compression.DeflateStream" /> object; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="mode" /> is not a valid <see cref="T:System.IO.Compression.CompressionMode" /> value.  
		/// -or-  
		/// <see cref="T:System.IO.Compression.CompressionMode" /> is <see cref="F:System.IO.Compression.CompressionMode.Compress" /> and <see cref="P:System.IO.Stream.CanWrite" /> is <see langword="false" />.  
		/// -or-  
		/// <see cref="T:System.IO.Compression.CompressionMode" /> is <see cref="F:System.IO.Compression.CompressionMode.Decompress" /> and <see cref="P:System.IO.Stream.CanRead" /> is <see langword="false" />.</exception>
		public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen) : this(stream, mode, leaveOpen, false)
		{
		}

		internal DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen, int windowsBits) : this(stream, mode, leaveOpen, true)
		{
		}

		internal DeflateStream(Stream compressedStream, CompressionMode mode, bool leaveOpen, bool gzip)
		{
			if (compressedStream == null)
			{
				throw new ArgumentNullException("compressedStream");
			}
			if (mode != CompressionMode.Compress && mode != CompressionMode.Decompress)
			{
				throw new ArgumentException("mode");
			}
			this.base_stream = compressedStream;
			this.native = DeflateStreamNative.Create(compressedStream, mode, gzip);
			if (this.native == null)
			{
				throw new NotImplementedException("Failed to initialize zlib. You probably have an old zlib installed. Version 1.2.0.4 or later is required.");
			}
			this.mode = mode;
			this.leaveOpen = leaveOpen;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.DeflateStream" /> class by using the specified stream and compression level.</summary>
		/// <param name="stream">The stream to compress.</param>
		/// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression efficiency when compressing the stream.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The stream does not support write operations such as compression. (The <see cref="P:System.IO.Stream.CanWrite" /> property on the stream object is <see langword="false" />.)</exception>
		public DeflateStream(Stream stream, CompressionLevel compressionLevel) : this(stream, compressionLevel, false, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Compression.DeflateStream" /> class by using the specified stream and compression level, and optionally leaves the stream open.</summary>
		/// <param name="stream">The stream to compress.</param>
		/// <param name="compressionLevel">One of the enumeration values that indicates whether to emphasize speed or compression efficiency when compressing the stream.</param>
		/// <param name="leaveOpen">
		///   <see langword="true" /> to leave the stream object open after disposing the <see cref="T:System.IO.Compression.DeflateStream" /> object; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The stream does not support write operations such as compression. (The <see cref="P:System.IO.Stream.CanWrite" /> property on the stream object is <see langword="false" />.)</exception>
		public DeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen) : this(stream, compressionLevel, leaveOpen, false)
		{
		}

		internal DeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen, int windowsBits) : this(stream, compressionLevel, leaveOpen, true)
		{
		}

		internal DeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen, bool gzip) : this(stream, CompressionMode.Compress, leaveOpen, gzip)
		{
		}

		~DeflateStream()
		{
			this.Dispose(false);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.Compression.DeflateStream" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
			DeflateStreamNative deflateStreamNative = this.native;
			if (deflateStreamNative != null)
			{
				deflateStreamNative.Dispose(disposing);
			}
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (!this.leaveOpen)
				{
					Stream stream = this.base_stream;
					if (stream != null)
					{
						stream.Close();
					}
					this.base_stream = null;
				}
			}
			base.Dispose(disposing);
		}

		private unsafe int ReadInternal(byte[] array, int offset, int count)
		{
			if (count == 0)
			{
				return 0;
			}
			byte* ptr;
			if (array == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			IntPtr buffer = new IntPtr((void*)(ptr + offset));
			return this.native.ReadZStream(buffer, count);
		}

		internal ValueTask<int> ReadAsyncMemory(Memory<byte> destination, CancellationToken cancellationToken)
		{
			return base.ReadAsync(destination, cancellationToken);
		}

		internal int ReadCore(Span<byte> destination)
		{
			byte[] array = new byte[destination.Length];
			int num = this.Read(array, 0, array.Length);
			array.AsSpan(0, num).CopyTo(destination);
			return num;
		}

		/// <summary>Reads a number of decompressed bytes into the specified byte array.</summary>
		/// <param name="array">The array to store decompressed bytes.</param>
		/// <param name="offset">The byte offset in <paramref name="array" /> at which the read bytes will be placed.</param>
		/// <param name="count">The maximum number of decompressed bytes to read.</param>
		/// <returns>The number of bytes that were read into the byte array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.IO.Compression.CompressionMode" /> value was <see langword="Compress" /> when the object was created.  
		/// -or-
		///  The underlying stream does not support reading.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> or <paramref name="count" /> is less than zero.  
		/// -or-  
		/// <paramref name="array" /> length minus the index starting point is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.IO.InvalidDataException">The data is in an invalid format.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream is closed.</exception>
		public override int Read(byte[] array, int offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (array == null)
			{
				throw new ArgumentNullException("Destination array is null.");
			}
			if (!this.CanRead)
			{
				throw new InvalidOperationException("Stream does not support reading.");
			}
			int num = array.Length;
			if (offset < 0 || count < 0)
			{
				throw new ArgumentException("Dest or count is negative.");
			}
			if (offset > num)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset + count > num)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			return this.ReadInternal(array, offset, count);
		}

		private unsafe void WriteInternal(byte[] array, int offset, int count)
		{
			if (count == 0)
			{
				return;
			}
			fixed (byte[] array2 = array)
			{
				byte* ptr;
				if (array == null || array2.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array2[0];
				}
				IntPtr buffer = new IntPtr((void*)(ptr + offset));
				this.native.WriteZStream(buffer, count);
			}
		}

		internal ValueTask WriteAsyncMemory(ReadOnlyMemory<byte> source, CancellationToken cancellationToken)
		{
			return base.WriteAsync(source, cancellationToken);
		}

		internal void WriteCore(ReadOnlySpan<byte> source)
		{
			this.Write(source.ToArray(), 0, source.Length);
		}

		/// <summary>Writes compressed bytes to the underlying stream from the specified byte array.</summary>
		/// <param name="array">The buffer that contains the data to compress.</param>
		/// <param name="offset">The byte offset in <paramref name="array" /> from which the bytes will be read.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		public override void Write(byte[] array, int offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing");
			}
			if (offset > array.Length - count)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			this.WriteInternal(array, offset, count);
		}

		/// <summary>The current implementation of this method has no functionality.</summary>
		/// <exception cref="T:System.ObjectDisposedException">The stream is closed.</exception>
		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (this.CanWrite)
			{
				this.native.Flush();
			}
		}

		/// <summary>Begins an asynchronous read operation. (Consider using the <see cref="M:System.IO.Stream.ReadAsync(System.Byte[],System.Int32,System.Int32)" /> method instead.)</summary>
		/// <param name="array">The byte array to read the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="array" /> at which to begin reading data from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="asyncCallback">An optional asynchronous callback, to be called when the read operation is complete.</param>
		/// <param name="asyncState">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
		/// <returns>An  object that represents the asynchronous read operation, which could still be pending.</returns>
		/// <exception cref="T:System.IO.IOException">The method tried to read asynchronously past the end of the stream, or a disk error occurred.</exception>
		/// <exception cref="T:System.ArgumentException">One or more of the arguments is invalid.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The current <see cref="T:System.IO.Compression.DeflateStream" /> implementation does not support the read operation.</exception>
		/// <exception cref="T:System.InvalidOperationException">This call cannot be completed.</exception>
		public override IAsyncResult BeginRead(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (!this.CanRead)
			{
				throw new NotSupportedException("This stream does not support reading");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (count + offset > array.Length)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			return new DeflateStream.ReadMethod(this.ReadInternal).BeginInvoke(array, offset, count, asyncCallback, asyncState);
		}

		/// <summary>Begins an asynchronous write operation. (Consider using the <see cref="M:System.IO.Stream.WriteAsync(System.Byte[],System.Int32,System.Int32)" /> method instead.)</summary>
		/// <param name="array">The buffer to write data from.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer" /> to begin writing from.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <param name="asyncCallback">An optional asynchronous callback, to be called when the write operation is complete.</param>
		/// <param name="asyncState">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
		/// <returns>An object that represents the asynchronous write operation, which could still be pending.</returns>
		/// <exception cref="T:System.IO.IOException">The method tried to write asynchronously past the end of the stream, or a disk error occurred.</exception>
		/// <exception cref="T:System.ArgumentException">One or more of the arguments is invalid.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The current <see cref="T:System.IO.Compression.DeflateStream" /> implementation does not support the write operation.</exception>
		/// <exception cref="T:System.InvalidOperationException">The write operation cannot be performed because the stream is closed.</exception>
		public override IAsyncResult BeginWrite(byte[] array, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (!this.CanWrite)
			{
				throw new InvalidOperationException("This stream does not support writing");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (count + offset > array.Length)
			{
				throw new ArgumentException("Buffer too small. count/offset wrong.");
			}
			return new DeflateStream.WriteMethod(this.WriteInternal).BeginInvoke(array, offset, count, asyncCallback, asyncState);
		}

		/// <summary>Waits for the pending asynchronous read to complete. (Consider using the <see cref="M:System.IO.Stream.ReadAsync(System.Byte[],System.Int32,System.Int32)" /> method instead.)</summary>
		/// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
		/// <returns>The number of bytes read from the stream, between 0 (zero) and the number of bytes you requested. <see cref="T:System.IO.Compression.DeflateStream" /> returns 0 only at the end of the stream; otherwise, it blocks until at least one byte is available.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> did not originate from a <see cref="M:System.IO.Compression.DeflateStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> method on the current stream.</exception>
		/// <exception cref="T:System.SystemException">An exception was thrown during a call to <see cref="M:System.Threading.WaitHandle.WaitOne" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The end call is invalid because asynchronous read operations for this stream are not yet complete.
		/// -or-
		/// The stream is <see langword="null" />.</exception>
		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			DeflateStream.ReadMethod readMethod = asyncResult2.AsyncDelegate as DeflateStream.ReadMethod;
			if (readMethod == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			return readMethod.EndInvoke(asyncResult);
		}

		/// <summary>Ends an asynchronous write operation. (Consider using the <see cref="M:System.IO.Stream.WriteAsync(System.Byte[],System.Int32,System.Int32)" /> method instead.)</summary>
		/// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> did not originate from a <see cref="M:System.IO.Compression.DeflateStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)" /> method on the current stream.</exception>
		/// <exception cref="T:System.Exception">An exception was thrown during a call to <see cref="M:System.Threading.WaitHandle.WaitOne" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The stream is <see langword="null" />.
		/// -or-
		/// The end write call is invalid.</exception>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			DeflateStream.WriteMethod writeMethod = asyncResult2.AsyncDelegate as DeflateStream.WriteMethod;
			if (writeMethod == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			writeMethod.EndInvoke(asyncResult);
		}

		/// <summary>This operation is not supported and always throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <param name="offset">The location in the stream.</param>
		/// <param name="origin">One of the <see cref="T:System.IO.SeekOrigin" /> values.</param>
		/// <returns>A long value.</returns>
		/// <exception cref="T:System.NotSupportedException">This property is not supported on this stream.</exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		/// <summary>This operation is not supported and always throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <param name="value">The length of the stream.</param>
		/// <exception cref="T:System.NotSupportedException">This property is not supported on this stream.</exception>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		/// <summary>Gets a reference to the underlying stream.</summary>
		/// <returns>A stream object that represents the underlying stream.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The underlying stream is closed.</exception>
		public Stream BaseStream
		{
			get
			{
				return this.base_stream;
			}
		}

		/// <summary>Gets a value indicating whether the stream supports reading while decompressing a file.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.IO.Compression.CompressionMode" /> value is <see langword="Decompress" />, and the underlying stream is opened and supports reading; otherwise, <see langword="false" />.</returns>
		public override bool CanRead
		{
			get
			{
				return !this.disposed && this.mode == CompressionMode.Decompress && this.base_stream.CanRead;
			}
		}

		/// <summary>Gets a value indicating whether the stream supports seeking.</summary>
		/// <returns>
		///   <see langword="false" /> in all cases.</returns>
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the stream supports writing.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.IO.Compression.CompressionMode" /> value is <see langword="Compress" />, and the underlying stream supports writing and is not closed; otherwise, <see langword="false" />.</returns>
		public override bool CanWrite
		{
			get
			{
				return !this.disposed && this.mode == CompressionMode.Compress && this.base_stream.CanWrite;
			}
		}

		/// <summary>This property is not supported and always throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <returns>A long value.</returns>
		/// <exception cref="T:System.NotSupportedException">This property is not supported on this stream.</exception>
		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>This property is not supported and always throws a <see cref="T:System.NotSupportedException" />.</summary>
		/// <returns>A long value.</returns>
		/// <exception cref="T:System.NotSupportedException">This property is not supported on this stream.</exception>
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

		private Stream base_stream;

		private CompressionMode mode;

		private bool leaveOpen;

		private bool disposed;

		private DeflateStreamNative native;

		private delegate int ReadMethod(byte[] array, int offset, int count);

		private delegate void WriteMethod(byte[] array, int offset, int count);
	}
}
