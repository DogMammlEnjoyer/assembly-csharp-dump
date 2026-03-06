using System;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
	/// <summary>Represents a HTTP response message including the status code and data.</summary>
	public class HttpResponseMessage : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpResponseMessage" /> class.</summary>
		public HttpResponseMessage() : this(HttpStatusCode.OK)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpResponseMessage" /> class with a specific <see cref="P:System.Net.Http.HttpResponseMessage.StatusCode" />.</summary>
		/// <param name="statusCode">The status code of the HTTP response.</param>
		public HttpResponseMessage(HttpStatusCode statusCode)
		{
			this.StatusCode = statusCode;
		}

		/// <summary>Gets or sets the content of a HTTP response message.</summary>
		/// <returns>The content of the HTTP response message.</returns>
		public HttpContent Content { get; set; }

		/// <summary>Gets the collection of HTTP response headers.</summary>
		/// <returns>The collection of HTTP response headers.</returns>
		public HttpResponseHeaders Headers
		{
			get
			{
				HttpResponseHeaders result;
				if ((result = this.headers) == null)
				{
					result = (this.headers = new HttpResponseHeaders());
				}
				return result;
			}
		}

		/// <summary>Gets a value that indicates if the HTTP response was successful.</summary>
		/// <returns>
		///   <see langword="true" /> if <see cref="P:System.Net.Http.HttpResponseMessage.StatusCode" /> was in the range 200-299; otherwise, <see langword="false" />.</returns>
		public bool IsSuccessStatusCode
		{
			get
			{
				return this.statusCode >= HttpStatusCode.OK && this.statusCode < HttpStatusCode.MultipleChoices;
			}
		}

		/// <summary>Gets or sets the reason phrase which typically is sent by servers together with the status code.</summary>
		/// <returns>The reason phrase sent by the server.</returns>
		public string ReasonPhrase
		{
			get
			{
				return this.reasonPhrase ?? HttpStatusDescription.Get(this.statusCode);
			}
			set
			{
				this.reasonPhrase = value;
			}
		}

		/// <summary>Gets or sets the request message which led to this response message.</summary>
		/// <returns>The request message which led to this response message.</returns>
		public HttpRequestMessage RequestMessage { get; set; }

		/// <summary>Gets or sets the status code of the HTTP response.</summary>
		/// <returns>The status code of the HTTP response.</returns>
		public HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
			set
			{
				if (value < (HttpStatusCode)0)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.statusCode = value;
			}
		}

		/// <summary>Gets or sets the HTTP message version.</summary>
		/// <returns>The HTTP message version. The default is 1.1.</returns>
		public Version Version
		{
			get
			{
				return this.version ?? HttpVersion.Version11;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Version");
				}
				this.version = value;
			}
		}

		/// <summary>Releases the unmanaged resources and disposes of unmanaged resources used by the <see cref="T:System.Net.Http.HttpResponseMessage" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpResponseMessage" /> and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (this.Content != null)
				{
					this.Content.Dispose();
				}
			}
		}

		/// <summary>Throws an exception if the <see cref="P:System.Net.Http.HttpResponseMessage.IsSuccessStatusCode" /> property for the HTTP response is <see langword="false" />.</summary>
		/// <returns>The HTTP response message if the call is successful.</returns>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The HTTP response is unsuccessful.</exception>
		public HttpResponseMessage EnsureSuccessStatusCode()
		{
			if (this.IsSuccessStatusCode)
			{
				return this;
			}
			throw new HttpRequestException(string.Format("{0} ({1})", (int)this.statusCode, this.ReasonPhrase));
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string representation of the current object.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StatusCode: ").Append((int)this.StatusCode);
			stringBuilder.Append(", ReasonPhrase: '").Append(this.ReasonPhrase ?? "<null>");
			stringBuilder.Append("', Version: ").Append(this.Version);
			stringBuilder.Append(", Content: ").Append((this.Content != null) ? this.Content.ToString() : "<null>");
			stringBuilder.Append(", Headers:\r\n{\r\n").Append(this.Headers);
			if (this.Content != null)
			{
				stringBuilder.Append(this.Content.Headers);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public HttpResponseHeaders TrailingHeaders
		{
			get
			{
				if (this.trailingHeaders == null)
				{
					this.trailingHeaders = new HttpResponseHeaders();
				}
				return this.trailingHeaders;
			}
		}

		private HttpResponseHeaders headers;

		private HttpResponseHeaders trailingHeaders;

		private string reasonPhrase;

		private HttpStatusCode statusCode;

		private Version version;

		private bool disposed;
	}
}
