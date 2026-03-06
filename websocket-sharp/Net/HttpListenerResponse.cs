using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerResponse : IDisposable
	{
		internal HttpListenerResponse(HttpListenerContext context)
		{
			this._context = context;
			this._keepAlive = true;
			this._statusCode = 200;
			this._statusDescription = "OK";
			this._version = HttpVersion.Version11;
		}

		internal bool CloseConnection
		{
			get
			{
				return this._closeConnection;
			}
			set
			{
				this._closeConnection = value;
			}
		}

		internal WebHeaderCollection FullHeaders
		{
			get
			{
				WebHeaderCollection webHeaderCollection = new WebHeaderCollection(HttpHeaderType.Response, true);
				bool flag = this._headers != null;
				if (flag)
				{
					webHeaderCollection.Add(this._headers);
				}
				bool flag2 = this._contentType != null;
				if (flag2)
				{
					webHeaderCollection.InternalSet("Content-Type", HttpListenerResponse.createContentTypeHeaderText(this._contentType, this._contentEncoding), true);
				}
				bool flag3 = webHeaderCollection["Server"] == null;
				if (flag3)
				{
					webHeaderCollection.InternalSet("Server", "websocket-sharp/1.0", true);
				}
				bool flag4 = webHeaderCollection["Date"] == null;
				if (flag4)
				{
					webHeaderCollection.InternalSet("Date", DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture), true);
				}
				bool sendChunked = this._sendChunked;
				if (sendChunked)
				{
					webHeaderCollection.InternalSet("Transfer-Encoding", "chunked", true);
				}
				else
				{
					webHeaderCollection.InternalSet("Content-Length", this._contentLength.ToString(CultureInfo.InvariantCulture), true);
				}
				bool flag5 = !this._context.Request.KeepAlive || !this._keepAlive || this._statusCode == 400 || this._statusCode == 408 || this._statusCode == 411 || this._statusCode == 413 || this._statusCode == 414 || this._statusCode == 500 || this._statusCode == 503;
				int reuses = this._context.Connection.Reuses;
				bool flag6 = flag5 || reuses >= 100;
				if (flag6)
				{
					webHeaderCollection.InternalSet("Connection", "close", true);
				}
				else
				{
					webHeaderCollection.InternalSet("Keep-Alive", string.Format("timeout=15,max={0}", 100 - reuses), true);
					bool flag7 = this._context.Request.ProtocolVersion < HttpVersion.Version11;
					if (flag7)
					{
						webHeaderCollection.InternalSet("Connection", "keep-alive", true);
					}
				}
				bool flag8 = this._redirectLocation != null;
				if (flag8)
				{
					webHeaderCollection.InternalSet("Location", this._redirectLocation.AbsoluteUri, true);
				}
				bool flag9 = this._cookies != null;
				if (flag9)
				{
					foreach (Cookie cookie in this._cookies)
					{
						webHeaderCollection.InternalSet("Set-Cookie", cookie.ToResponseString(), true);
					}
				}
				return webHeaderCollection;
			}
		}

		internal bool HeadersSent
		{
			get
			{
				return this._headersSent;
			}
			set
			{
				this._headersSent = value;
			}
		}

		internal string StatusLine
		{
			get
			{
				return string.Format("HTTP/{0} {1} {2}\r\n", this._version, this._statusCode, this._statusDescription);
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				return this._contentEncoding;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				this._contentEncoding = value;
			}
		}

		public long ContentLength64
		{
			get
			{
				return this._contentLength;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				bool flag = value < 0L;
				if (flag)
				{
					string paramName = "Less than zero.";
					throw new ArgumentOutOfRangeException(paramName, "value");
				}
				this._contentLength = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this._contentType;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				bool flag = value == null;
				if (flag)
				{
					this._contentType = null;
				}
				else
				{
					bool flag2 = value.Length == 0;
					if (flag2)
					{
						string message2 = "An empty string.";
						throw new ArgumentException(message2, "value");
					}
					bool flag3 = !HttpListenerResponse.isValidForContentType(value);
					if (flag3)
					{
						string message3 = "It contains an invalid character.";
						throw new ArgumentException(message3, "value");
					}
					this._contentType = value;
				}
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				bool flag = this._cookies == null;
				if (flag)
				{
					this._cookies = new CookieCollection();
				}
				return this._cookies;
			}
			set
			{
				this._cookies = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				bool flag = this._headers == null;
				if (flag)
				{
					this._headers = new WebHeaderCollection(HttpHeaderType.Response, false);
				}
				return this._headers;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this._headers = null;
				}
				else
				{
					bool flag2 = value.State != HttpHeaderType.Response;
					if (flag2)
					{
						string message = "The value is not valid for a response.";
						throw new InvalidOperationException(message);
					}
					this._headers = value;
				}
			}
		}

		public bool KeepAlive
		{
			get
			{
				return this._keepAlive;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				this._keepAlive = value;
			}
		}

		public Stream OutputStream
		{
			get
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool flag = this._outputStream == null;
				if (flag)
				{
					this._outputStream = this._context.Connection.GetResponseStream();
				}
				return this._outputStream;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this._version;
			}
		}

		public string RedirectLocation
		{
			get
			{
				return (this._redirectLocation != null) ? this._redirectLocation.OriginalString : null;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				bool flag = value == null;
				if (flag)
				{
					this._redirectLocation = null;
				}
				else
				{
					bool flag2 = value.Length == 0;
					if (flag2)
					{
						string message2 = "An empty string.";
						throw new ArgumentException(message2, "value");
					}
					Uri redirectLocation;
					bool flag3 = !Uri.TryCreate(value, UriKind.Absolute, out redirectLocation);
					if (flag3)
					{
						string message3 = "Not an absolute URL.";
						throw new ArgumentException(message3, "value");
					}
					this._redirectLocation = redirectLocation;
				}
			}
		}

		public bool SendChunked
		{
			get
			{
				return this._sendChunked;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				this._sendChunked = value;
			}
		}

		public int StatusCode
		{
			get
			{
				return this._statusCode;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				bool flag = value < 100 || value > 999;
				if (flag)
				{
					string message2 = "A value is not between 100 and 999 inclusive.";
					throw new ProtocolViolationException(message2);
				}
				this._statusCode = value;
				this._statusDescription = value.GetStatusDescription();
			}
		}

		public string StatusDescription
		{
			get
			{
				return this._statusDescription;
			}
			set
			{
				bool disposed = this._disposed;
				if (disposed)
				{
					string objectName = base.GetType().ToString();
					throw new ObjectDisposedException(objectName);
				}
				bool headersSent = this._headersSent;
				if (headersSent)
				{
					string message = "The response is already being sent.";
					throw new InvalidOperationException(message);
				}
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				bool flag2 = value.Length == 0;
				if (flag2)
				{
					this._statusDescription = this._statusCode.GetStatusDescription();
				}
				else
				{
					bool flag3 = !HttpListenerResponse.isValidForStatusDescription(value);
					if (flag3)
					{
						string message2 = "It contains an invalid character.";
						throw new ArgumentException(message2, "value");
					}
					this._statusDescription = value;
				}
			}
		}

		private bool canSetCookie(Cookie cookie)
		{
			List<Cookie> list = this.findCookie(cookie).ToList<Cookie>();
			bool flag = list.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int version = cookie.Version;
				foreach (Cookie cookie2 in list)
				{
					bool flag2 = cookie2.Version == version;
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private void close(bool force)
		{
			this._disposed = true;
			this._context.Connection.Close(force);
		}

		private void close(byte[] responseEntity, int bufferLength, bool willBlock)
		{
			Stream outputStream = this.OutputStream;
			if (willBlock)
			{
				outputStream.WriteBytes(responseEntity, bufferLength);
				this.close(false);
			}
			else
			{
				outputStream.WriteBytesAsync(responseEntity, bufferLength, delegate
				{
					this.close(false);
				}, null);
			}
		}

		private static string createContentTypeHeaderText(string value, Encoding encoding)
		{
			bool flag = value.IndexOf("charset=", StringComparison.Ordinal) > -1;
			string result;
			if (flag)
			{
				result = value;
			}
			else
			{
				bool flag2 = encoding == null;
				if (flag2)
				{
					result = value;
				}
				else
				{
					result = string.Format("{0}; charset={1}", value, encoding.WebName);
				}
			}
			return result;
		}

		private IEnumerable<Cookie> findCookie(Cookie cookie)
		{
			bool flag = this._cookies == null || this._cookies.Count == 0;
			if (flag)
			{
				yield break;
			}
			foreach (Cookie c in this._cookies)
			{
				bool flag2 = c.EqualsWithoutValueAndVersion(cookie);
				if (flag2)
				{
					yield return c;
				}
				c = null;
			}
			IEnumerator<Cookie> enumerator = null;
			yield break;
			yield break;
		}

		private static bool isValidForContentType(string value)
		{
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				bool flag = c < ' ';
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = c > '~';
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3 = "()<>@:\\[]?{}".IndexOf(c) > -1;
						if (!flag3)
						{
							i++;
							continue;
						}
						result = false;
					}
				}
				return result;
			}
			return true;
		}

		private static bool isValidForStatusDescription(string value)
		{
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				bool flag = c < ' ';
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = c > '~';
					if (!flag2)
					{
						i++;
						continue;
					}
					result = false;
				}
				return result;
			}
			return true;
		}

		public void Abort()
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				this.close(true);
			}
		}

		public void AppendCookie(Cookie cookie)
		{
			this.Cookies.Add(cookie);
		}

		public void AppendHeader(string name, string value)
		{
			this.Headers.Add(name, value);
		}

		public void Close()
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				this.close(false);
			}
		}

		public void Close(byte[] responseEntity, bool willBlock)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			bool flag = responseEntity == null;
			if (flag)
			{
				throw new ArgumentNullException("responseEntity");
			}
			long num = (long)responseEntity.Length;
			bool flag2 = num > 2147483647L;
			if (flag2)
			{
				this.close(responseEntity, 1024, willBlock);
			}
			else
			{
				Stream stream = this.OutputStream;
				if (willBlock)
				{
					stream.Write(responseEntity, 0, (int)num);
					this.close(false);
				}
				else
				{
					stream.BeginWrite(responseEntity, 0, (int)num, delegate(IAsyncResult ar)
					{
						stream.EndWrite(ar);
						this.close(false);
					}, null);
				}
			}
		}

		public void CopyFrom(HttpListenerResponse templateResponse)
		{
			bool flag = templateResponse == null;
			if (flag)
			{
				throw new ArgumentNullException("templateResponse");
			}
			WebHeaderCollection headers = templateResponse._headers;
			bool flag2 = headers != null;
			if (flag2)
			{
				bool flag3 = this._headers != null;
				if (flag3)
				{
					this._headers.Clear();
				}
				this.Headers.Add(headers);
			}
			else
			{
				this._headers = null;
			}
			this._contentLength = templateResponse._contentLength;
			this._statusCode = templateResponse._statusCode;
			this._statusDescription = templateResponse._statusDescription;
			this._keepAlive = templateResponse._keepAlive;
			this._version = templateResponse._version;
		}

		public void Redirect(string url)
		{
			bool disposed = this._disposed;
			if (disposed)
			{
				string objectName = base.GetType().ToString();
				throw new ObjectDisposedException(objectName);
			}
			bool headersSent = this._headersSent;
			if (headersSent)
			{
				string message = "The response is already being sent.";
				throw new InvalidOperationException(message);
			}
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			bool flag2 = url.Length == 0;
			if (flag2)
			{
				string message2 = "An empty string.";
				throw new ArgumentException(message2, "url");
			}
			Uri redirectLocation;
			bool flag3 = !Uri.TryCreate(url, UriKind.Absolute, out redirectLocation);
			if (flag3)
			{
				string message3 = "Not an absolute URL.";
				throw new ArgumentException(message3, "url");
			}
			this._redirectLocation = redirectLocation;
			this._statusCode = 302;
			this._statusDescription = "Found";
		}

		public void SetCookie(Cookie cookie)
		{
			bool flag = cookie == null;
			if (flag)
			{
				throw new ArgumentNullException("cookie");
			}
			bool flag2 = !this.canSetCookie(cookie);
			if (flag2)
			{
				string message = "It cannot be updated.";
				throw new ArgumentException(message, "cookie");
			}
			this.Cookies.Add(cookie);
		}

		public void SetHeader(string name, string value)
		{
			this.Headers.Set(name, value);
		}

		void IDisposable.Dispose()
		{
			bool disposed = this._disposed;
			if (!disposed)
			{
				this.close(true);
			}
		}

		private bool _closeConnection;

		private Encoding _contentEncoding;

		private long _contentLength;

		private string _contentType;

		private HttpListenerContext _context;

		private CookieCollection _cookies;

		private bool _disposed;

		private WebHeaderCollection _headers;

		private bool _headersSent;

		private bool _keepAlive;

		private ResponseStream _outputStream;

		private Uri _redirectLocation;

		private bool _sendChunked;

		private int _statusCode;

		private string _statusDescription;

		private Version _version;
	}
}
