using System;
using System.Security.Principal;
using System.Text;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Net
{
	public sealed class HttpListenerContext
	{
		internal HttpListenerContext(HttpConnection connection)
		{
			this._connection = connection;
			this._errorStatusCode = 400;
			this._request = new HttpListenerRequest(this);
			this._response = new HttpListenerResponse(this);
		}

		internal HttpConnection Connection
		{
			get
			{
				return this._connection;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return this._errorMessage;
			}
			set
			{
				this._errorMessage = value;
			}
		}

		internal int ErrorStatusCode
		{
			get
			{
				return this._errorStatusCode;
			}
			set
			{
				this._errorStatusCode = value;
			}
		}

		internal bool HasErrorMessage
		{
			get
			{
				return this._errorMessage != null;
			}
		}

		internal HttpListener Listener
		{
			get
			{
				return this._listener;
			}
			set
			{
				this._listener = value;
			}
		}

		public HttpListenerRequest Request
		{
			get
			{
				return this._request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return this._response;
			}
		}

		public IPrincipal User
		{
			get
			{
				return this._user;
			}
			internal set
			{
				this._user = value;
			}
		}

		private static string createErrorContent(int statusCode, string statusDescription, string message)
		{
			return (message != null && message.Length > 0) ? string.Format("<html><body><h1>{0} {1} ({2})</h1></body></html>", statusCode, statusDescription, message) : string.Format("<html><body><h1>{0} {1}</h1></body></html>", statusCode, statusDescription);
		}

		internal HttpListenerWebSocketContext GetWebSocketContext(string protocol)
		{
			this._websocketContext = new HttpListenerWebSocketContext(this, protocol);
			return this._websocketContext;
		}

		internal void SendAuthenticationChallenge(AuthenticationSchemes scheme, string realm)
		{
			this._response.StatusCode = 401;
			string value = new AuthenticationChallenge(scheme, realm).ToString();
			this._response.Headers.InternalSet("WWW-Authenticate", value, true);
			this._response.Close();
		}

		internal void SendError()
		{
			try
			{
				this._response.StatusCode = this._errorStatusCode;
				this._response.ContentType = "text/html";
				string s = HttpListenerContext.createErrorContent(this._errorStatusCode, this._response.StatusDescription, this._errorMessage);
				Encoding utf = Encoding.UTF8;
				byte[] bytes = utf.GetBytes(s);
				this._response.ContentEncoding = utf;
				this._response.ContentLength64 = (long)bytes.Length;
				this._response.Close(bytes, true);
			}
			catch
			{
				this._connection.Close(true);
			}
		}

		internal void SendError(int statusCode)
		{
			this._errorStatusCode = statusCode;
			this.SendError();
		}

		internal void SendError(int statusCode, string message)
		{
			this._errorStatusCode = statusCode;
			this._errorMessage = message;
			this.SendError();
		}

		internal void Unregister()
		{
			bool flag = this._listener == null;
			if (!flag)
			{
				this._listener.UnregisterContext(this);
			}
		}

		public HttpListenerWebSocketContext AcceptWebSocket(string protocol)
		{
			bool flag = this._websocketContext != null;
			if (flag)
			{
				string message = "The accepting is already in progress.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = protocol != null;
			if (flag2)
			{
				bool flag3 = protocol.Length == 0;
				if (flag3)
				{
					string message2 = "An empty string.";
					throw new ArgumentException(message2, "protocol");
				}
				bool flag4 = !protocol.IsToken();
				if (flag4)
				{
					string message3 = "It contains an invalid character.";
					throw new ArgumentException(message3, "protocol");
				}
			}
			return this.GetWebSocketContext(protocol);
		}

		private HttpConnection _connection;

		private string _errorMessage;

		private int _errorStatusCode;

		private HttpListener _listener;

		private HttpListenerRequest _request;

		private HttpListenerResponse _response;

		private IPrincipal _user;

		private HttpListenerWebSocketContext _websocketContext;
	}
}
