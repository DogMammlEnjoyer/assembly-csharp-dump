using System;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
	/// <summary>A base class representing an HTTP entity body and content headers.</summary>
	public abstract class HttpContent : IDisposable
	{
		/// <summary>Gets the HTTP content headers as defined in RFC 2616.</summary>
		/// <returns>The content headers as defined in RFC 2616.</returns>
		public HttpContentHeaders Headers
		{
			get
			{
				HttpContentHeaders result;
				if ((result = this.headers) == null)
				{
					result = (this.headers = new HttpContentHeaders(this));
				}
				return result;
			}
		}

		internal long? LoadedBufferLength
		{
			get
			{
				if (this.buffer != null)
				{
					return new long?(this.buffer.Length);
				}
				return null;
			}
		}

		internal void CopyTo(Stream stream)
		{
			this.CopyToAsync(stream).Wait();
		}

		/// <summary>Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the <paramref name="stream" /> parameter.</summary>
		/// <param name="stream">The target stream.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task CopyToAsync(Stream stream)
		{
			return this.CopyToAsync(stream, null);
		}

		/// <summary>Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the <paramref name="stream" /> parameter.</summary>
		/// <param name="stream">The target stream.</param>
		/// <param name="context">Information about the transport (channel binding token, for example). This parameter may be <see langword="null" />.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task CopyToAsync(Stream stream, TransportContext context)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (this.buffer != null)
			{
				return this.buffer.CopyToAsync(stream);
			}
			return this.SerializeToStreamAsync(stream, context);
		}

		/// <summary>Serialize the HTTP content to a memory stream as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		protected virtual Task<Stream> CreateContentReadStreamAsync()
		{
			HttpContent.<CreateContentReadStreamAsync>d__12 <CreateContentReadStreamAsync>d__;
			<CreateContentReadStreamAsync>d__.<>4__this = this;
			<CreateContentReadStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Stream>.Create();
			<CreateContentReadStreamAsync>d__.<>1__state = -1;
			<CreateContentReadStreamAsync>d__.<>t__builder.Start<HttpContent.<CreateContentReadStreamAsync>d__12>(ref <CreateContentReadStreamAsync>d__);
			return <CreateContentReadStreamAsync>d__.<>t__builder.Task;
		}

		private static HttpContent.FixedMemoryStream CreateFixedMemoryStream(long maxBufferSize)
		{
			return new HttpContent.FixedMemoryStream(maxBufferSize);
		}

		/// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpContent" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpContent" /> and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (this.buffer != null)
				{
					this.buffer.Dispose();
				}
			}
		}

		/// <summary>Serialize the HTTP content to a memory buffer as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task LoadIntoBufferAsync()
		{
			return this.LoadIntoBufferAsync(2147483647L);
		}

		/// <summary>Serialize the HTTP content to a memory buffer as an asynchronous operation.</summary>
		/// <param name="maxBufferSize">The maximum size, in bytes, of the buffer to use.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task LoadIntoBufferAsync(long maxBufferSize)
		{
			HttpContent.<LoadIntoBufferAsync>d__17 <LoadIntoBufferAsync>d__;
			<LoadIntoBufferAsync>d__.<>4__this = this;
			<LoadIntoBufferAsync>d__.maxBufferSize = maxBufferSize;
			<LoadIntoBufferAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadIntoBufferAsync>d__.<>1__state = -1;
			<LoadIntoBufferAsync>d__.<>t__builder.Start<HttpContent.<LoadIntoBufferAsync>d__17>(ref <LoadIntoBufferAsync>d__);
			return <LoadIntoBufferAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize the HTTP content and return a stream that represents the content as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<Stream> ReadAsStreamAsync()
		{
			HttpContent.<ReadAsStreamAsync>d__18 <ReadAsStreamAsync>d__;
			<ReadAsStreamAsync>d__.<>4__this = this;
			<ReadAsStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Stream>.Create();
			<ReadAsStreamAsync>d__.<>1__state = -1;
			<ReadAsStreamAsync>d__.<>t__builder.Start<HttpContent.<ReadAsStreamAsync>d__18>(ref <ReadAsStreamAsync>d__);
			return <ReadAsStreamAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize the HTTP content to a byte array as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<byte[]> ReadAsByteArrayAsync()
		{
			HttpContent.<ReadAsByteArrayAsync>d__19 <ReadAsByteArrayAsync>d__;
			<ReadAsByteArrayAsync>d__.<>4__this = this;
			<ReadAsByteArrayAsync>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<ReadAsByteArrayAsync>d__.<>1__state = -1;
			<ReadAsByteArrayAsync>d__.<>t__builder.Start<HttpContent.<ReadAsByteArrayAsync>d__19>(ref <ReadAsByteArrayAsync>d__);
			return <ReadAsByteArrayAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize the HTTP content to a string as an asynchronous operation.</summary>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<string> ReadAsStringAsync()
		{
			HttpContent.<ReadAsStringAsync>d__20 <ReadAsStringAsync>d__;
			<ReadAsStringAsync>d__.<>4__this = this;
			<ReadAsStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadAsStringAsync>d__.<>1__state = -1;
			<ReadAsStringAsync>d__.<>t__builder.Start<HttpContent.<ReadAsStringAsync>d__20>(ref <ReadAsStringAsync>d__);
			return <ReadAsStringAsync>d__.<>t__builder.Task;
		}

		private static Encoding GetEncodingFromBuffer(byte[] buffer, int length, ref int preambleLength)
		{
			foreach (Encoding encoding in new Encoding[]
			{
				Encoding.UTF8,
				Encoding.UTF32,
				Encoding.Unicode
			})
			{
				if ((preambleLength = HttpContent.StartsWith(buffer, length, encoding.GetPreamble())) != 0)
				{
					return encoding;
				}
			}
			return null;
		}

		private static int StartsWith(byte[] array, int length, byte[] value)
		{
			if (length < value.Length)
			{
				return 0;
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (array[i] != value[i])
				{
					return 0;
				}
			}
			return value.Length;
		}

		internal Task SerializeToStreamAsync_internal(Stream stream, TransportContext context)
		{
			return this.SerializeToStreamAsync(stream, context);
		}

		/// <summary>Serialize the HTTP content to a stream as an asynchronous operation.</summary>
		/// <param name="stream">The target stream.</param>
		/// <param name="context">Information about the transport (channel binding token, for example). This parameter may be <see langword="null" />.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		protected abstract Task SerializeToStreamAsync(Stream stream, TransportContext context);

		/// <summary>Determines whether the HTTP content has a valid length in bytes.</summary>
		/// <param name="length">The length in bytes of the HTTP content.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="length" /> is a valid length; otherwise, <see langword="false" />.</returns>
		protected internal abstract bool TryComputeLength(out long length);

		private HttpContent.FixedMemoryStream buffer;

		private Stream stream;

		private bool disposed;

		private HttpContentHeaders headers;

		private sealed class FixedMemoryStream : MemoryStream
		{
			public FixedMemoryStream(long maxSize)
			{
				this.maxSize = maxSize;
			}

			private void CheckOverflow(int count)
			{
				if (this.Length + (long)count > this.maxSize)
				{
					throw new HttpRequestException(string.Format("Cannot write more bytes to the buffer than the configured maximum buffer size: {0}", this.maxSize));
				}
			}

			public override void WriteByte(byte value)
			{
				this.CheckOverflow(1);
				base.WriteByte(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.CheckOverflow(count);
				base.Write(buffer, offset, count);
			}

			private readonly long maxSize;
		}
	}
}
