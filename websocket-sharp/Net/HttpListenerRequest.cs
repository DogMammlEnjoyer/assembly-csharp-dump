using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerRequest
	{
		internal HttpListenerRequest(HttpListenerContext context)
		{
			this._context = context;
			this._connection = context.Connection;
			this._contentLength = -1L;
			this._headers = new WebHeaderCollection();
			this._requestTraceIdentifier = Guid.NewGuid();
		}

		public string[] AcceptTypes
		{
			get
			{
				string text = this._headers["Accept"];
				bool flag = text == null;
				string[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this._acceptTypes == null;
					if (flag2)
					{
						this._acceptTypes = text.SplitHeaderValue(new char[]
						{
							','
						}).TrimEach().ToList<string>().ToArray();
					}
					result = this._acceptTypes;
				}
				return result;
			}
		}

		public int ClientCertificateError
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				bool flag = this._contentEncoding == null;
				if (flag)
				{
					this._contentEncoding = this.getContentEncoding();
				}
				return this._contentEncoding;
			}
		}

		public long ContentLength64
		{
			get
			{
				return this._contentLength;
			}
		}

		public string ContentType
		{
			get
			{
				return this._headers["Content-Type"];
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				bool flag = this._cookies == null;
				if (flag)
				{
					this._cookies = this._headers.GetCookies(false);
				}
				return this._cookies;
			}
		}

		public bool HasEntityBody
		{
			get
			{
				return this._contentLength > 0L || this._chunked;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		public string HttpMethod
		{
			get
			{
				return this._httpMethod;
			}
		}

		public Stream InputStream
		{
			get
			{
				bool flag = this._inputStream == null;
				if (flag)
				{
					this._inputStream = ((this._contentLength > 0L || this._chunked) ? this._connection.GetRequestStream(this._contentLength, this._chunked) : Stream.Null);
				}
				return this._inputStream;
			}
		}

		public bool IsAuthenticated
		{
			get
			{
				return this._context.User != null;
			}
		}

		public bool IsLocal
		{
			get
			{
				return this._connection.IsLocal;
			}
		}

		public bool IsSecureConnection
		{
			get
			{
				return this._connection.IsSecure;
			}
		}

		public bool IsWebSocketRequest
		{
			get
			{
				return this._httpMethod == "GET" && this._headers.Upgrades("websocket");
			}
		}

		public bool KeepAlive
		{
			get
			{
				return this._headers.KeepsAlive(this._protocolVersion);
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this._connection.LocalEndPoint;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this._protocolVersion;
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				bool flag = this._queryString == null;
				if (flag)
				{
					Uri url = this.Url;
					this._queryString = QueryStringCollection.Parse((url != null) ? url.Query : null, Encoding.UTF8);
				}
				return this._queryString;
			}
		}

		public string RawUrl
		{
			get
			{
				return this._rawUrl;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this._connection.RemoteEndPoint;
			}
		}

		public Guid RequestTraceIdentifier
		{
			get
			{
				return this._requestTraceIdentifier;
			}
		}

		public Uri Url
		{
			get
			{
				bool flag = !this._urlSet;
				if (flag)
				{
					this._url = HttpUtility.CreateRequestUrl(this._rawUrl, this._userHostName ?? this.UserHostAddress, this.IsWebSocketRequest, this.IsSecureConnection);
					this._urlSet = true;
				}
				return this._url;
			}
		}

		public Uri UrlReferrer
		{
			get
			{
				string text = this._headers["Referer"];
				bool flag = text == null;
				Uri result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this._urlReferrer == null;
					if (flag2)
					{
						this._urlReferrer = text.ToUri();
					}
					result = this._urlReferrer;
				}
				return result;
			}
		}

		public string UserAgent
		{
			get
			{
				return this._headers["User-Agent"];
			}
		}

		public string UserHostAddress
		{
			get
			{
				return this._connection.LocalEndPoint.ToString();
			}
		}

		public string UserHostName
		{
			get
			{
				return this._userHostName;
			}
		}

		public string[] UserLanguages
		{
			get
			{
				string text = this._headers["Accept-Language"];
				bool flag = text == null;
				string[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = this._userLanguages == null;
					if (flag2)
					{
						this._userLanguages = text.Split(new char[]
						{
							','
						}).TrimEach().ToList<string>().ToArray();
					}
					result = this._userLanguages;
				}
				return result;
			}
		}

		private Encoding getContentEncoding()
		{
			string text = this._headers["Content-Type"];
			bool flag = text == null;
			Encoding result;
			if (flag)
			{
				result = Encoding.UTF8;
			}
			else
			{
				Encoding encoding;
				result = (HttpUtility.TryGetEncoding(text, out encoding) ? encoding : Encoding.UTF8);
			}
			return result;
		}

		internal void AddHeader(string headerField)
		{
			char c = headerField[0];
			bool flag = c == ' ' || c == '\t';
			if (flag)
			{
				this._context.ErrorMessage = "Invalid header field";
			}
			else
			{
				int num = headerField.IndexOf(':');
				bool flag2 = num < 1;
				if (flag2)
				{
					this._context.ErrorMessage = "Invalid header field";
				}
				else
				{
					string text = headerField.Substring(0, num).Trim();
					bool flag3 = text.Length == 0 || !text.IsToken();
					if (flag3)
					{
						this._context.ErrorMessage = "Invalid header name";
					}
					else
					{
						string text2 = (num < headerField.Length - 1) ? headerField.Substring(num + 1).Trim() : string.Empty;
						this._headers.InternalSet(text, text2, false);
						string a = text.ToLower(CultureInfo.InvariantCulture);
						bool flag4 = a == "host";
						if (flag4)
						{
							bool flag5 = this._userHostName != null;
							if (flag5)
							{
								this._context.ErrorMessage = "Invalid Host header";
							}
							else
							{
								bool flag6 = text2.Length == 0;
								if (flag6)
								{
									this._context.ErrorMessage = "Invalid Host header";
								}
								else
								{
									this._userHostName = text2;
								}
							}
						}
						else
						{
							bool flag7 = a == "content-length";
							if (flag7)
							{
								bool flag8 = this._contentLength > -1L;
								if (flag8)
								{
									this._context.ErrorMessage = "Invalid Content-Length header";
								}
								else
								{
									long num2;
									bool flag9 = !long.TryParse(text2, out num2);
									if (flag9)
									{
										this._context.ErrorMessage = "Invalid Content-Length header";
									}
									else
									{
										bool flag10 = num2 < 0L;
										if (flag10)
										{
											this._context.ErrorMessage = "Invalid Content-Length header";
										}
										else
										{
											this._contentLength = num2;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		internal void FinishInitialization()
		{
			bool flag = this._userHostName == null;
			if (flag)
			{
				this._context.ErrorMessage = "Host header required";
			}
			else
			{
				string text = this._headers["Transfer-Encoding"];
				bool flag2 = text != null;
				if (flag2)
				{
					StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
					bool flag3 = !text.Equals("chunked", comparisonType);
					if (flag3)
					{
						this._context.ErrorMessage = "Invalid Transfer-Encoding header";
						this._context.ErrorStatusCode = 501;
						return;
					}
					this._chunked = true;
				}
				bool flag4 = this._httpMethod == "POST" || this._httpMethod == "PUT";
				if (flag4)
				{
					bool flag5 = this._contentLength <= 0L && !this._chunked;
					if (flag5)
					{
						this._context.ErrorMessage = string.Empty;
						this._context.ErrorStatusCode = 411;
						return;
					}
				}
				string text2 = this._headers["Expect"];
				bool flag6 = text2 != null;
				if (flag6)
				{
					StringComparison comparisonType2 = StringComparison.OrdinalIgnoreCase;
					bool flag7 = !text2.Equals("100-continue", comparisonType2);
					if (flag7)
					{
						this._context.ErrorMessage = "Invalid Expect header";
					}
					else
					{
						ResponseStream responseStream = this._connection.GetResponseStream();
						responseStream.InternalWrite(HttpListenerRequest._100continue, 0, HttpListenerRequest._100continue.Length);
					}
				}
			}
		}

		internal bool FlushInput()
		{
			Stream inputStream = this.InputStream;
			bool flag = inputStream == Stream.Null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int num = 2048;
				bool flag2 = this._contentLength > 0L && this._contentLength < (long)num;
				if (flag2)
				{
					num = (int)this._contentLength;
				}
				byte[] buffer = new byte[num];
				for (;;)
				{
					try
					{
						IAsyncResult asyncResult = inputStream.BeginRead(buffer, 0, num, null, null);
						bool flag3 = !asyncResult.IsCompleted;
						if (flag3)
						{
							int millisecondsTimeout = 100;
							bool flag4 = !asyncResult.AsyncWaitHandle.WaitOne(millisecondsTimeout);
							if (flag4)
							{
								result = false;
								break;
							}
						}
						bool flag5 = inputStream.EndRead(asyncResult) <= 0;
						if (flag5)
						{
							result = true;
							break;
						}
					}
					catch
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		internal bool IsUpgradeRequest(string protocol)
		{
			return this._headers.Upgrades(protocol);
		}

		internal void SetRequestLine(string requestLine)
		{
			string[] array = requestLine.Split(new char[]
			{
				' '
			}, 3);
			bool flag = array.Length < 3;
			if (flag)
			{
				this._context.ErrorMessage = "Invalid request line (parts)";
			}
			else
			{
				string text = array[0];
				bool flag2 = text.Length == 0;
				if (flag2)
				{
					this._context.ErrorMessage = "Invalid request line (method)";
				}
				else
				{
					string text2 = array[1];
					bool flag3 = text2.Length == 0;
					if (flag3)
					{
						this._context.ErrorMessage = "Invalid request line (target)";
					}
					else
					{
						string text3 = array[2];
						bool flag4 = text3.Length != 8;
						if (flag4)
						{
							this._context.ErrorMessage = "Invalid request line (version)";
						}
						else
						{
							bool flag5 = !text3.StartsWith("HTTP/", StringComparison.Ordinal);
							if (flag5)
							{
								this._context.ErrorMessage = "Invalid request line (version)";
							}
							else
							{
								Version version;
								bool flag6 = !text3.Substring(5).TryCreateVersion(out version);
								if (flag6)
								{
									this._context.ErrorMessage = "Invalid request line (version)";
								}
								else
								{
									bool flag7 = version != HttpVersion.Version11;
									if (flag7)
									{
										this._context.ErrorMessage = "Invalid request line (version)";
										this._context.ErrorStatusCode = 505;
									}
									else
									{
										bool flag8 = !text.IsHttpMethod(version);
										if (flag8)
										{
											this._context.ErrorMessage = "Invalid request line (method)";
											this._context.ErrorStatusCode = 501;
										}
										else
										{
											this._httpMethod = text;
											this._rawUrl = text2;
											this._protocolVersion = version;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
		{
			throw new NotSupportedException();
		}

		public X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public X509Certificate2 GetClientCertificate()
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}\r\n", this._httpMethod, this._rawUrl, this._protocolVersion).Append(this._headers.ToString());
			return stringBuilder.ToString();
		}

		private static readonly byte[] _100continue = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");

		private string[] _acceptTypes;

		private bool _chunked;

		private HttpConnection _connection;

		private Encoding _contentEncoding;

		private long _contentLength;

		private HttpListenerContext _context;

		private CookieCollection _cookies;

		private WebHeaderCollection _headers;

		private string _httpMethod;

		private Stream _inputStream;

		private Version _protocolVersion;

		private NameValueCollection _queryString;

		private string _rawUrl;

		private Guid _requestTraceIdentifier;

		private Uri _url;

		private Uri _urlReferrer;

		private bool _urlSet;

		private string _userHostName;

		private string[] _userLanguages;
	}
}
