using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
	/// <summary>Represents a HTTP request message.</summary>
	public class HttpRequestMessage : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class.</summary>
		public HttpRequestMessage()
		{
			this.method = HttpMethod.Get;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
		/// <param name="method">The HTTP method.</param>
		/// <param name="requestUri">A string that represents the request  <see cref="T:System.Uri" />.</param>
		public HttpRequestMessage(HttpMethod method, string requestUri) : this(method, string.IsNullOrEmpty(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpRequestMessage" /> class with an HTTP method and a request <see cref="T:System.Uri" />.</summary>
		/// <param name="method">The HTTP method.</param>
		/// <param name="requestUri">The <see cref="T:System.Uri" /> to request.</param>
		public HttpRequestMessage(HttpMethod method, Uri requestUri)
		{
			this.Method = method;
			this.RequestUri = requestUri;
		}

		/// <summary>Gets or sets the contents of the HTTP message.</summary>
		/// <returns>The content of a message</returns>
		public HttpContent Content { get; set; }

		/// <summary>Gets the collection of HTTP request headers.</summary>
		/// <returns>The collection of HTTP request headers.</returns>
		public HttpRequestHeaders Headers
		{
			get
			{
				HttpRequestHeaders result;
				if ((result = this.headers) == null)
				{
					result = (this.headers = new HttpRequestHeaders());
				}
				return result;
			}
		}

		/// <summary>Gets or sets the HTTP method used by the HTTP request message.</summary>
		/// <returns>The HTTP method used by the request message. The default is the GET method.</returns>
		public HttpMethod Method
		{
			get
			{
				return this.method;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("method");
				}
				this.method = value;
			}
		}

		/// <summary>Gets a set of properties for the HTTP request.</summary>
		/// <returns>Returns <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
		public IDictionary<string, object> Properties
		{
			get
			{
				Dictionary<string, object> result;
				if ((result = this.properties) == null)
				{
					result = (this.properties = new Dictionary<string, object>());
				}
				return result;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Uri" /> used for the HTTP request.</summary>
		/// <returns>The <see cref="T:System.Uri" /> used for the HTTP request.</returns>
		public Uri RequestUri
		{
			get
			{
				return this.uri;
			}
			set
			{
				if (value != null && value.IsAbsoluteUri && !HttpRequestMessage.IsAllowedAbsoluteUri(value))
				{
					throw new ArgumentException("Only http or https scheme is allowed");
				}
				this.uri = value;
			}
		}

		private static bool IsAllowedAbsoluteUri(Uri uri)
		{
			return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeFile && uri.OriginalString.StartsWith("/", StringComparison.Ordinal));
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

		/// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpRequestMessage" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpRequestMessage" /> and optionally disposes of the managed resources.</summary>
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

		internal bool SetIsUsed()
		{
			if (this.is_used)
			{
				return true;
			}
			this.is_used = true;
			return false;
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string representation of the current object.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Method: ").Append(this.method);
			stringBuilder.Append(", RequestUri: '").Append((this.RequestUri != null) ? this.RequestUri.ToString() : "<null>");
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

		private HttpRequestHeaders headers;

		private HttpMethod method;

		private Version version;

		private Dictionary<string, object> properties;

		private Uri uri;

		private bool is_used;

		private bool disposed;
	}
}
