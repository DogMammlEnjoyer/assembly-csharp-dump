using System;
using System.Collections.Specialized;
using System.IO;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public abstract class WebSocketBehavior : IWebSocketSession
	{
		protected WebSocketBehavior()
		{
			this._startTime = DateTime.MaxValue;
		}

		protected NameValueCollection Headers
		{
			get
			{
				return (this._context != null) ? this._context.Headers : null;
			}
		}

		protected NameValueCollection QueryString
		{
			get
			{
				return (this._context != null) ? this._context.QueryString : null;
			}
		}

		protected WebSocketSessionManager Sessions
		{
			get
			{
				return this._sessions;
			}
		}

		public WebSocketState ConnectionState
		{
			get
			{
				return (this._websocket != null) ? this._websocket.ReadyState : WebSocketState.Connecting;
			}
		}

		public WebSocketContext Context
		{
			get
			{
				return this._context;
			}
		}

		public Func<CookieCollection, CookieCollection, bool> CookiesValidator
		{
			get
			{
				return this._cookiesValidator;
			}
			set
			{
				this._cookiesValidator = value;
			}
		}

		public bool EmitOnPing
		{
			get
			{
				return (this._websocket != null) ? this._websocket.EmitOnPing : this._emitOnPing;
			}
			set
			{
				bool flag = this._websocket != null;
				if (flag)
				{
					this._websocket.EmitOnPing = value;
				}
				else
				{
					this._emitOnPing = value;
				}
			}
		}

		public string ID
		{
			get
			{
				return this._id;
			}
		}

		public bool IgnoreExtensions
		{
			get
			{
				return this._ignoreExtensions;
			}
			set
			{
				this._ignoreExtensions = value;
			}
		}

		public Func<string, bool> OriginValidator
		{
			get
			{
				return this._originValidator;
			}
			set
			{
				this._originValidator = value;
			}
		}

		public string Protocol
		{
			get
			{
				return (this._websocket != null) ? this._websocket.Protocol : (this._protocol ?? string.Empty);
			}
			set
			{
				bool flag = this._websocket != null;
				if (flag)
				{
					string message = "The session has already started.";
					throw new InvalidOperationException(message);
				}
				bool flag2 = value == null || value.Length == 0;
				if (flag2)
				{
					this._protocol = null;
				}
				else
				{
					bool flag3 = !value.IsToken();
					if (flag3)
					{
						string message2 = "It is not a token.";
						throw new ArgumentException(message2, "value");
					}
					this._protocol = value;
				}
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this._startTime;
			}
		}

		private string checkHandshakeRequest(WebSocketContext context)
		{
			bool flag = this._originValidator != null;
			if (flag)
			{
				bool flag2 = !this._originValidator(context.Origin);
				if (flag2)
				{
					return "It includes no Origin header or an invalid one.";
				}
			}
			bool flag3 = this._cookiesValidator != null;
			if (flag3)
			{
				CookieCollection cookieCollection = context.CookieCollection;
				CookieCollection cookieCollection2 = context.WebSocket.CookieCollection;
				bool flag4 = !this._cookiesValidator(cookieCollection, cookieCollection2);
				if (flag4)
				{
					return "It includes no cookie or an invalid one.";
				}
			}
			return null;
		}

		private void onClose(object sender, CloseEventArgs e)
		{
			bool flag = this._id == null;
			if (!flag)
			{
				this._sessions.Remove(this._id);
				this.OnClose(e);
			}
		}

		private void onError(object sender, ErrorEventArgs e)
		{
			this.OnError(e);
		}

		private void onMessage(object sender, MessageEventArgs e)
		{
			this.OnMessage(e);
		}

		private void onOpen(object sender, EventArgs e)
		{
			this._id = this._sessions.Add(this);
			bool flag = this._id == null;
			if (flag)
			{
				this._websocket.Close(CloseStatusCode.Away);
			}
			else
			{
				this._startTime = DateTime.Now;
				this.OnOpen();
			}
		}

		internal void Start(WebSocketContext context, WebSocketSessionManager sessions)
		{
			this._context = context;
			this._sessions = sessions;
			this._websocket = context.WebSocket;
			this._websocket.CustomHandshakeRequestChecker = new Func<WebSocketContext, string>(this.checkHandshakeRequest);
			this._websocket.EmitOnPing = this._emitOnPing;
			this._websocket.IgnoreExtensions = this._ignoreExtensions;
			this._websocket.Protocol = this._protocol;
			TimeSpan waitTime = sessions.WaitTime;
			bool flag = waitTime != this._websocket.WaitTime;
			if (flag)
			{
				this._websocket.WaitTime = waitTime;
			}
			this._websocket.OnOpen += this.onOpen;
			this._websocket.OnMessage += this.onMessage;
			this._websocket.OnError += this.onError;
			this._websocket.OnClose += this.onClose;
			this._websocket.InternalAccept();
		}

		protected void Close()
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The session has not started yet.";
				throw new InvalidOperationException(message);
			}
			this._websocket.Close();
		}

		protected void Close(ushort code, string reason)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The session has not started yet.";
				throw new InvalidOperationException(message);
			}
			this._websocket.Close(code, reason);
		}

		protected void Close(CloseStatusCode code, string reason)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The session has not started yet.";
				throw new InvalidOperationException(message);
			}
			this._websocket.Close(code, reason);
		}

		protected void CloseAsync()
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The session has not started yet.";
				throw new InvalidOperationException(message);
			}
			this._websocket.CloseAsync();
		}

		protected void CloseAsync(ushort code, string reason)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The session has not started yet.";
				throw new InvalidOperationException(message);
			}
			this._websocket.CloseAsync(code, reason);
		}

		protected void CloseAsync(CloseStatusCode code, string reason)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The session has not started yet.";
				throw new InvalidOperationException(message);
			}
			this._websocket.CloseAsync(code, reason);
		}

		protected virtual void OnClose(CloseEventArgs e)
		{
		}

		protected virtual void OnError(ErrorEventArgs e)
		{
		}

		protected virtual void OnMessage(MessageEventArgs e)
		{
		}

		protected virtual void OnOpen()
		{
		}

		protected bool Ping()
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The session has not started yet.";
				throw new InvalidOperationException(message);
			}
			return this._websocket.Ping();
		}

		protected bool Ping(string message)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message2 = "The session has not started yet.";
				throw new InvalidOperationException(message2);
			}
			return this._websocket.Ping(message);
		}

		protected void Send(byte[] data)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.Send(data);
		}

		protected void Send(FileInfo fileInfo)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.Send(fileInfo);
		}

		protected void Send(string data)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.Send(data);
		}

		protected void Send(Stream stream, int length)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.Send(stream, length);
		}

		protected void SendAsync(byte[] data, Action<bool> completed)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.SendAsync(data, completed);
		}

		protected void SendAsync(FileInfo fileInfo, Action<bool> completed)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.SendAsync(fileInfo, completed);
		}

		protected void SendAsync(string data, Action<bool> completed)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.SendAsync(data, completed);
		}

		protected void SendAsync(Stream stream, int length, Action<bool> completed)
		{
			bool flag = this._websocket == null;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			this._websocket.SendAsync(stream, length, completed);
		}

		private WebSocketContext _context;

		private Func<CookieCollection, CookieCollection, bool> _cookiesValidator;

		private bool _emitOnPing;

		private string _id;

		private bool _ignoreExtensions;

		private Func<string, bool> _originValidator;

		private string _protocol;

		private WebSocketSessionManager _sessions;

		private DateTime _startTime;

		private WebSocket _websocket;
	}
}
