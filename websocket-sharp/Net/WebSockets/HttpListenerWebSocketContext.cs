using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Principal;

namespace WebSocketSharp.Net.WebSockets
{
	public class HttpListenerWebSocketContext : WebSocketContext
	{
		internal HttpListenerWebSocketContext(HttpListenerContext context, string protocol)
		{
			this._context = context;
			this._websocket = new WebSocket(this, protocol);
		}

		internal Logger Log
		{
			get
			{
				return this._context.Listener.Log;
			}
		}

		internal Stream Stream
		{
			get
			{
				return this._context.Connection.Stream;
			}
		}

		public override CookieCollection CookieCollection
		{
			get
			{
				return this._context.Request.Cookies;
			}
		}

		public override NameValueCollection Headers
		{
			get
			{
				return this._context.Request.Headers;
			}
		}

		public override string Host
		{
			get
			{
				return this._context.Request.UserHostName;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return this._context.Request.IsAuthenticated;
			}
		}

		public override bool IsLocal
		{
			get
			{
				return this._context.Request.IsLocal;
			}
		}

		public override bool IsSecureConnection
		{
			get
			{
				return this._context.Request.IsSecureConnection;
			}
		}

		public override bool IsWebSocketRequest
		{
			get
			{
				return this._context.Request.IsWebSocketRequest;
			}
		}

		public override string Origin
		{
			get
			{
				return this._context.Request.Headers["Origin"];
			}
		}

		public override NameValueCollection QueryString
		{
			get
			{
				return this._context.Request.QueryString;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return this._context.Request.Url;
			}
		}

		public override string SecWebSocketKey
		{
			get
			{
				return this._context.Request.Headers["Sec-WebSocket-Key"];
			}
		}

		public override IEnumerable<string> SecWebSocketProtocols
		{
			get
			{
				string val = this._context.Request.Headers["Sec-WebSocket-Protocol"];
				bool flag = val == null || val.Length == 0;
				if (flag)
				{
					yield break;
				}
				foreach (string elm in val.Split(new char[]
				{
					','
				}))
				{
					string protocol = elm.Trim();
					bool flag2 = protocol.Length == 0;
					if (!flag2)
					{
						yield return protocol;
						protocol = null;
						elm = null;
					}
				}
				string[] array = null;
				yield break;
			}
		}

		public override string SecWebSocketVersion
		{
			get
			{
				return this._context.Request.Headers["Sec-WebSocket-Version"];
			}
		}

		public override IPEndPoint ServerEndPoint
		{
			get
			{
				return this._context.Request.LocalEndPoint;
			}
		}

		public override IPrincipal User
		{
			get
			{
				return this._context.User;
			}
		}

		public override IPEndPoint UserEndPoint
		{
			get
			{
				return this._context.Request.RemoteEndPoint;
			}
		}

		public override WebSocket WebSocket
		{
			get
			{
				return this._websocket;
			}
		}

		internal void Close()
		{
			this._context.Connection.Close(true);
		}

		internal void Close(HttpStatusCode code)
		{
			this._context.Response.StatusCode = (int)code;
			this._context.Response.Close();
		}

		public override string ToString()
		{
			return this._context.Request.ToString();
		}

		private HttpListenerContext _context;

		private WebSocket _websocket;
	}
}
