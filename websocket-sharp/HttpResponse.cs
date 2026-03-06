using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal class HttpResponse : HttpBase
	{
		private HttpResponse(string code, string reason, Version version, NameValueCollection headers) : base(version, headers)
		{
			this._code = code;
			this._reason = reason;
		}

		internal HttpResponse(HttpStatusCode code) : this(code, code.GetDescription())
		{
		}

		internal HttpResponse(HttpStatusCode code, string reason)
		{
			int num = (int)code;
			this..ctor(num.ToString(), reason, HttpVersion.Version11, new NameValueCollection());
			base.Headers["Server"] = "websocket-sharp/1.0";
		}

		public CookieCollection Cookies
		{
			get
			{
				return base.Headers.GetCookies(true);
			}
		}

		public bool HasConnectionClose
		{
			get
			{
				StringComparison comparisonTypeForValue = StringComparison.OrdinalIgnoreCase;
				return base.Headers.Contains("Connection", "close", comparisonTypeForValue);
			}
		}

		public bool IsProxyAuthenticationRequired
		{
			get
			{
				return this._code == "407";
			}
		}

		public bool IsRedirect
		{
			get
			{
				return this._code == "301" || this._code == "302";
			}
		}

		public bool IsUnauthorized
		{
			get
			{
				return this._code == "401";
			}
		}

		public bool IsWebSocketResponse
		{
			get
			{
				return base.ProtocolVersion > HttpVersion.Version10 && this._code == "101" && base.Headers.Upgrades("websocket");
			}
		}

		public string Reason
		{
			get
			{
				return this._reason;
			}
		}

		public string StatusCode
		{
			get
			{
				return this._code;
			}
		}

		internal static HttpResponse CreateCloseResponse(HttpStatusCode code)
		{
			HttpResponse httpResponse = new HttpResponse(code);
			httpResponse.Headers["Connection"] = "close";
			return httpResponse;
		}

		internal static HttpResponse CreateUnauthorizedResponse(string challenge)
		{
			HttpResponse httpResponse = new HttpResponse(HttpStatusCode.Unauthorized);
			httpResponse.Headers["WWW-Authenticate"] = challenge;
			return httpResponse;
		}

		internal static HttpResponse CreateWebSocketResponse()
		{
			HttpResponse httpResponse = new HttpResponse(HttpStatusCode.SwitchingProtocols);
			NameValueCollection headers = httpResponse.Headers;
			headers["Upgrade"] = "websocket";
			headers["Connection"] = "Upgrade";
			return httpResponse;
		}

		internal static HttpResponse Parse(string[] headerParts)
		{
			string[] array = headerParts[0].Split(new char[]
			{
				' '
			}, 3);
			bool flag = array.Length != 3;
			if (flag)
			{
				throw new ArgumentException("Invalid status line: " + headerParts[0]);
			}
			WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
			for (int i = 1; i < headerParts.Length; i++)
			{
				webHeaderCollection.InternalSet(headerParts[i], true);
			}
			return new HttpResponse(array[1], array[2], new Version(array[0].Substring(5)), webHeaderCollection);
		}

		internal static HttpResponse Read(Stream stream, int millisecondsTimeout)
		{
			return HttpBase.Read<HttpResponse>(stream, new Func<string[], HttpResponse>(HttpResponse.Parse), millisecondsTimeout);
		}

		public void SetCookies(CookieCollection cookies)
		{
			bool flag = cookies == null || cookies.Count == 0;
			if (!flag)
			{
				NameValueCollection headers = base.Headers;
				foreach (Cookie cookie in cookies.Sorted)
				{
					headers.Add("Set-Cookie", cookie.ToResponseString());
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("HTTP/{0} {1} {2}{3}", new object[]
			{
				base.ProtocolVersion,
				this._code,
				this._reason,
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

		private string _code;

		private string _reason;
	}
}
