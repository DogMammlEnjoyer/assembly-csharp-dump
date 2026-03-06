using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal class HttpRequest : HttpBase
	{
		private HttpRequest(string method, string uri, Version version, NameValueCollection headers) : base(version, headers)
		{
			this._method = method;
			this._uri = uri;
		}

		internal HttpRequest(string method, string uri) : this(method, uri, HttpVersion.Version11, new NameValueCollection())
		{
			base.Headers["User-Agent"] = "websocket-sharp/1.0";
		}

		public AuthenticationResponse AuthenticationResponse
		{
			get
			{
				string text = base.Headers["Authorization"];
				return (text != null && text.Length > 0) ? AuthenticationResponse.Parse(text) : null;
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				bool flag = this._cookies == null;
				if (flag)
				{
					this._cookies = base.Headers.GetCookies(false);
				}
				return this._cookies;
			}
		}

		public string HttpMethod
		{
			get
			{
				return this._method;
			}
		}

		public bool IsWebSocketRequest
		{
			get
			{
				return this._method == "GET" && base.ProtocolVersion > HttpVersion.Version10 && base.Headers.Upgrades("websocket");
			}
		}

		public string RequestUri
		{
			get
			{
				return this._uri;
			}
		}

		internal static HttpRequest CreateConnectRequest(Uri uri)
		{
			string dnsSafeHost = uri.DnsSafeHost;
			int port = uri.Port;
			string text = string.Format("{0}:{1}", dnsSafeHost, port);
			HttpRequest httpRequest = new HttpRequest("CONNECT", text);
			httpRequest.Headers["Host"] = ((port == 80) ? dnsSafeHost : text);
			return httpRequest;
		}

		internal static HttpRequest CreateWebSocketRequest(Uri uri)
		{
			HttpRequest httpRequest = new HttpRequest("GET", uri.PathAndQuery);
			NameValueCollection headers = httpRequest.Headers;
			int port = uri.Port;
			string scheme = uri.Scheme;
			headers["Host"] = (((port == 80 && scheme == "ws") || (port == 443 && scheme == "wss")) ? uri.DnsSafeHost : uri.Authority);
			headers["Upgrade"] = "websocket";
			headers["Connection"] = "Upgrade";
			return httpRequest;
		}

		internal HttpResponse GetResponse(Stream stream, int millisecondsTimeout)
		{
			byte[] array = base.ToByteArray();
			stream.Write(array, 0, array.Length);
			return HttpBase.Read<HttpResponse>(stream, new Func<string[], HttpResponse>(HttpResponse.Parse), millisecondsTimeout);
		}

		internal static HttpRequest Parse(string[] headerParts)
		{
			string[] array = headerParts[0].Split(new char[]
			{
				' '
			}, 3);
			bool flag = array.Length != 3;
			if (flag)
			{
				throw new ArgumentException("Invalid request line: " + headerParts[0]);
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int i = 1; i < headerParts.Length; i++)
			{
				webHeaderCollection.InternalSet(headerParts[i], false);
			}
			return new HttpRequest(array[0], array[1], new Version(array[2].Substring(5)), webHeaderCollection);
		}

		internal static HttpRequest Read(Stream stream, int millisecondsTimeout)
		{
			return HttpBase.Read<HttpRequest>(stream, new Func<string[], HttpRequest>(HttpRequest.Parse), millisecondsTimeout);
		}

		public void SetCookies(CookieCollection cookies)
		{
			bool flag = cookies == null || cookies.Count == 0;
			if (!flag)
			{
				StringBuilder stringBuilder = new StringBuilder(64);
				foreach (Cookie cookie in cookies.Sorted)
				{
					bool flag2 = !cookie.Expired;
					if (flag2)
					{
						stringBuilder.AppendFormat("{0}; ", cookie.ToString());
					}
				}
				int length = stringBuilder.Length;
				bool flag3 = length > 2;
				if (flag3)
				{
					stringBuilder.Length = length - 2;
					base.Headers["Cookie"] = stringBuilder.ToString();
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}{3}", new object[]
			{
				this._method,
				this._uri,
				base.ProtocolVersion,
				"\r\n"
			});
			NameValueCollection headers = base.Headers;
			foreach (string text in headers.AllKeys)
			{
				stringBuilder.AppendFormat("{0}: {1}{2}", text, headers[text], "\r\n");
			}
			stringBuilder.Append("\r\n");
			string entityBody = base.EntityBody;
			bool flag = entityBody.Length > 0;
			if (flag)
			{
				stringBuilder.Append(entityBody);
			}
			return stringBuilder.ToString();
		}

		private CookieCollection _cookies;

		private string _method;

		private string _uri;
	}
}
