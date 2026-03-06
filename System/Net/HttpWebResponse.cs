using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System.Net
{
	/// <summary>Provides an HTTP-specific implementation of the <see cref="T:System.Net.WebResponse" /> class.</summary>
	[Serializable]
	public class HttpWebResponse : WebResponse, ISerializable, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.HttpWebResponse" /> class.</summary>
		public HttpWebResponse()
		{
		}

		internal HttpWebResponse(Uri uri, string method, HttpStatusCode status, WebHeaderCollection headers)
		{
			this.uri = uri;
			this.method = method;
			this.statusCode = status;
			this.statusDescription = HttpStatusDescription.Get(status);
			this.webHeaders = headers;
			this.version = HttpVersion.Version10;
			this.contentLength = -1L;
		}

		internal HttpWebResponse(Uri uri, string method, WebResponseStream stream, CookieContainer container)
		{
			this.uri = uri;
			this.method = method;
			this.stream = stream;
			this.webHeaders = (stream.Headers ?? new WebHeaderCollection());
			this.version = stream.Version;
			this.statusCode = stream.StatusCode;
			this.statusDescription = (stream.StatusDescription ?? HttpStatusDescription.Get(this.statusCode));
			this.contentLength = -1L;
			try
			{
				string text = this.webHeaders["Content-Length"];
				if (string.IsNullOrEmpty(text) || !long.TryParse(text, out this.contentLength))
				{
					this.contentLength = -1L;
				}
			}
			catch (Exception)
			{
				this.contentLength = -1L;
			}
			if (container != null)
			{
				this.cookie_container = container;
				this.FillCookies();
			}
			string a = this.webHeaders["Content-Encoding"];
			if (a == "gzip" && (stream.Request.AutomaticDecompression & DecompressionMethods.GZip) != DecompressionMethods.None)
			{
				this.stream = new GZipStream(stream, CompressionMode.Decompress);
				this.webHeaders.Remove(HttpRequestHeader.ContentEncoding);
				return;
			}
			if (a == "deflate" && (stream.Request.AutomaticDecompression & DecompressionMethods.Deflate) != DecompressionMethods.None)
			{
				this.stream = new DeflateStream(stream, CompressionMode.Decompress);
				this.webHeaders.Remove(HttpRequestHeader.ContentEncoding);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.HttpWebResponse" /> class from the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" /> instances.</summary>
		/// <param name="serializationInfo">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that contains the information required to serialize the new <see cref="T:System.Net.HttpWebRequest" />.</param>
		/// <param name="streamingContext">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains the source of the serialized stream that is associated with the new <see cref="T:System.Net.HttpWebRequest" />.</param>
		[Obsolete("Serialization is obsoleted for this type", false)]
		protected HttpWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.uri = (Uri)serializationInfo.GetValue("uri", typeof(Uri));
			this.contentLength = serializationInfo.GetInt64("contentLength");
			this.contentType = serializationInfo.GetString("contentType");
			this.method = serializationInfo.GetString("method");
			this.statusDescription = serializationInfo.GetString("statusDescription");
			this.cookieCollection = (CookieCollection)serializationInfo.GetValue("cookieCollection", typeof(CookieCollection));
			this.version = (Version)serializationInfo.GetValue("version", typeof(Version));
			this.statusCode = (HttpStatusCode)serializationInfo.GetValue("statusCode", typeof(HttpStatusCode));
		}

		/// <summary>Gets the character set of the response.</summary>
		/// <returns>A string that contains the character set of the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public string CharacterSet
		{
			get
			{
				string text = this.ContentType;
				if (text == null)
				{
					return "ISO-8859-1";
				}
				string text2 = text.ToLower();
				int num = text2.IndexOf("charset=", StringComparison.Ordinal);
				if (num == -1)
				{
					return "ISO-8859-1";
				}
				num += 8;
				int num2 = text2.IndexOf(';', num);
				if (num2 != -1)
				{
					return text.Substring(num, num2 - num);
				}
				return text.Substring(num);
			}
		}

		/// <summary>Gets the method that is used to encode the body of the response.</summary>
		/// <returns>A string that describes the method that is used to encode the body of the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public string ContentEncoding
		{
			get
			{
				this.CheckDisposed();
				string text = this.webHeaders["Content-Encoding"];
				if (text == null)
				{
					return "";
				}
				return text;
			}
		}

		/// <summary>Gets the length of the content returned by the request.</summary>
		/// <returns>The number of bytes returned by the request. Content length does not include header information.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public override long ContentLength
		{
			get
			{
				return this.contentLength;
			}
		}

		/// <summary>Gets the content type of the response.</summary>
		/// <returns>A string that contains the content type of the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public override string ContentType
		{
			get
			{
				this.CheckDisposed();
				if (this.contentType == null)
				{
					this.contentType = this.webHeaders["Content-Type"];
				}
				if (this.contentType == null)
				{
					this.contentType = string.Empty;
				}
				return this.contentType;
			}
		}

		/// <summary>Gets or sets the cookies that are associated with this response.</summary>
		/// <returns>A <see cref="T:System.Net.CookieCollection" /> that contains the cookies that are associated with this response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public virtual CookieCollection Cookies
		{
			get
			{
				this.CheckDisposed();
				if (this.cookieCollection == null)
				{
					this.cookieCollection = new CookieCollection();
				}
				return this.cookieCollection;
			}
			set
			{
				this.CheckDisposed();
				this.cookieCollection = value;
			}
		}

		/// <summary>Gets the headers that are associated with this response from the server.</summary>
		/// <returns>A <see cref="T:System.Net.WebHeaderCollection" /> that contains the header information returned with the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public override WebHeaderCollection Headers
		{
			get
			{
				return this.webHeaders;
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether both client and server were authenticated.</summary>
		/// <returns>
		///   <see langword="true" /> if mutual authentication occurred; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		[MonoTODO]
		public override bool IsMutuallyAuthenticated
		{
			get
			{
				throw HttpWebResponse.GetMustImplement();
			}
		}

		/// <summary>Gets the last date and time that the contents of the response were modified.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> that contains the date and time that the contents of the response were modified.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public DateTime LastModified
		{
			get
			{
				this.CheckDisposed();
				DateTime result;
				try
				{
					result = MonoHttpDate.Parse(this.webHeaders["Last-Modified"]);
				}
				catch (Exception)
				{
					result = DateTime.Now;
				}
				return result;
			}
		}

		/// <summary>Gets the method that is used to return the response.</summary>
		/// <returns>A string that contains the HTTP method that is used to return the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public virtual string Method
		{
			get
			{
				this.CheckDisposed();
				return this.method;
			}
		}

		/// <summary>Gets the version of the HTTP protocol that is used in the response.</summary>
		/// <returns>A <see cref="T:System.Version" /> that contains the HTTP protocol version of the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public Version ProtocolVersion
		{
			get
			{
				this.CheckDisposed();
				return this.version;
			}
		}

		/// <summary>Gets the URI of the Internet resource that responded to the request.</summary>
		/// <returns>The URI of the Internet resource that responded to the request.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public override Uri ResponseUri
		{
			get
			{
				this.CheckDisposed();
				return this.uri;
			}
		}

		/// <summary>Gets the name of the server that sent the response.</summary>
		/// <returns>A string that contains the name of the server that sent the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public string Server
		{
			get
			{
				this.CheckDisposed();
				return this.webHeaders["Server"] ?? "";
			}
		}

		/// <summary>Gets the status of the response.</summary>
		/// <returns>One of the <see cref="T:System.Net.HttpStatusCode" /> values.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public virtual HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		/// <summary>Gets the status description returned with the response.</summary>
		/// <returns>A string that describes the status of the response.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public virtual string StatusDescription
		{
			get
			{
				this.CheckDisposed();
				return this.statusDescription;
			}
		}

		/// <summary>Gets a value that indicates whether headers are supported.</summary>
		/// <returns>
		///   <see langword="true" /> if headers are supported; otherwise, <see langword="false" />. Always returns <see langword="true" />.</returns>
		public override bool SupportsHeaders
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets the contents of a header that was returned with the response.</summary>
		/// <param name="headerName">The header value to return.</param>
		/// <returns>The contents of the specified header.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public string GetResponseHeader(string headerName)
		{
			this.CheckDisposed();
			string text = this.webHeaders[headerName];
			if (text == null)
			{
				return "";
			}
			return text;
		}

		/// <summary>Gets the stream that is used to read the body of the response from the server.</summary>
		/// <returns>A <see cref="T:System.IO.Stream" /> containing the body of the response.</returns>
		/// <exception cref="T:System.Net.ProtocolViolationException">There is no response stream.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public override Stream GetResponseStream()
		{
			this.CheckDisposed();
			if (this.stream == null)
			{
				return Stream.Null;
			}
			if (string.Equals(this.method, "HEAD", StringComparison.OrdinalIgnoreCase))
			{
				return Stream.Null;
			}
			return this.stream;
		}

		/// <summary>Serializes this instance into the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object.</summary>
		/// <param name="serializationInfo">The object into which this <see cref="T:System.Net.HttpWebResponse" /> will be serialized.</param>
		/// <param name="streamingContext">The destination of the serialization.</param>
		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.</summary>
		/// <param name="serializationInfo">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
		/// <param name="streamingContext">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that specifies the destination for this serialization.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("uri", this.uri);
			serializationInfo.AddValue("contentLength", this.contentLength);
			serializationInfo.AddValue("contentType", this.contentType);
			serializationInfo.AddValue("method", this.method);
			serializationInfo.AddValue("statusDescription", this.statusDescription);
			serializationInfo.AddValue("cookieCollection", this.cookieCollection);
			serializationInfo.AddValue("version", this.version);
			serializationInfo.AddValue("statusCode", this.statusCode);
		}

		/// <summary>Closes the response stream.</summary>
		/// <exception cref="T:System.ObjectDisposedException">.NET Core only: This <see cref="T:System.Net.HttpWebResponse" /> object has been disposed.</exception>
		public override void Close()
		{
			Stream stream = Interlocked.Exchange<Stream>(ref this.stream, null);
			if (stream != null)
			{
				stream.Close();
			}
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.HttpWebResponse" />, and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			this.disposed = true;
			base.Dispose(true);
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		private void FillCookies()
		{
			if (this.webHeaders == null)
			{
				return;
			}
			CookieCollection cookieCollection = null;
			try
			{
				string text = this.webHeaders.Get("Set-Cookie");
				if (text != null)
				{
					cookieCollection = this.cookie_container.CookieCutter(this.uri, "Set-Cookie", text, false);
				}
			}
			catch
			{
			}
			try
			{
				string text = this.webHeaders.Get("Set-Cookie2");
				if (text != null)
				{
					CookieCollection cookieCollection2 = this.cookie_container.CookieCutter(this.uri, "Set-Cookie2", text, false);
					if (cookieCollection != null && cookieCollection.Count != 0)
					{
						cookieCollection.Add(cookieCollection2);
					}
					else
					{
						cookieCollection = cookieCollection2;
					}
				}
			}
			catch
			{
			}
			this.cookieCollection = cookieCollection;
		}

		private Uri uri;

		private WebHeaderCollection webHeaders;

		private CookieCollection cookieCollection;

		private string method;

		private Version version;

		private HttpStatusCode statusCode;

		private string statusDescription;

		private long contentLength;

		private string contentType;

		private CookieContainer cookie_container;

		private bool disposed;

		private Stream stream;
	}
}
