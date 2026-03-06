using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp
{
	public class WebSocket : IDisposable
	{
		internal WebSocket(HttpListenerWebSocketContext context, string protocol)
		{
			this._context = context;
			this._protocol = protocol;
			this._closeContext = new Action(context.Close);
			this._logger = context.Log;
			this._message = new Action<MessageEventArgs>(this.messages);
			this._secure = context.IsSecureConnection;
			this._stream = context.Stream;
			this._waitTime = TimeSpan.FromSeconds(1.0);
			this.init();
		}

		internal WebSocket(TcpListenerWebSocketContext context, string protocol)
		{
			this._context = context;
			this._protocol = protocol;
			this._closeContext = new Action(context.Close);
			this._logger = context.Log;
			this._message = new Action<MessageEventArgs>(this.messages);
			this._secure = context.IsSecureConnection;
			this._stream = context.Stream;
			this._waitTime = TimeSpan.FromSeconds(1.0);
			this.init();
		}

		public WebSocket(string url, params string[] protocols)
		{
			bool flag = url == null;
			if (flag)
			{
				throw new ArgumentNullException("url");
			}
			bool flag2 = url.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "url");
			}
			string message;
			bool flag3 = !url.TryCreateWebSocketUri(out this._uri, out message);
			if (flag3)
			{
				throw new ArgumentException(message, "url");
			}
			bool flag4 = protocols != null && protocols.Length != 0;
			if (flag4)
			{
				bool flag5 = !WebSocket.checkProtocols(protocols, out message);
				if (flag5)
				{
					throw new ArgumentException(message, "protocols");
				}
				this._protocols = protocols;
			}
			this._base64Key = WebSocket.CreateBase64Key();
			this._client = true;
			this._logger = new Logger();
			this._message = new Action<MessageEventArgs>(this.messagec);
			this._secure = (this._uri.Scheme == "wss");
			this._waitTime = TimeSpan.FromSeconds(5.0);
			this.init();
		}

		internal CookieCollection CookieCollection
		{
			get
			{
				return this._cookies;
			}
		}

		internal Func<WebSocketContext, string> CustomHandshakeRequestChecker
		{
			get
			{
				return this._handshakeRequestChecker;
			}
			set
			{
				this._handshakeRequestChecker = value;
			}
		}

		internal bool HasMessage
		{
			get
			{
				object forMessageEventQueue = this._forMessageEventQueue;
				bool result;
				lock (forMessageEventQueue)
				{
					result = (this._messageEventQueue.Count > 0);
				}
				return result;
			}
		}

		internal bool IgnoreExtensions
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

		internal bool IsConnected
		{
			get
			{
				return this._readyState == WebSocketState.Open || this._readyState == WebSocketState.Closing;
			}
		}

		public CompressionMethod Compression
		{
			get
			{
				return this._compression;
			}
			set
			{
				string message = null;
				bool flag = !this._client;
				if (flag)
				{
					message = "This instance is not a client.";
					throw new InvalidOperationException(message);
				}
				bool flag2 = !this.canSet(out message);
				if (flag2)
				{
					this._logger.Warn(message);
				}
				else
				{
					object forState = this._forState;
					lock (forState)
					{
						bool flag3 = !this.canSet(out message);
						if (flag3)
						{
							this._logger.Warn(message);
						}
						else
						{
							this._compression = value;
						}
					}
				}
			}
		}

		public IEnumerable<Cookie> Cookies
		{
			get
			{
				object obj = this._cookies.SyncRoot;
				lock (obj)
				{
					foreach (Cookie cookie in this._cookies)
					{
						yield return cookie;
						cookie = null;
					}
					IEnumerator<Cookie> enumerator = null;
				}
				obj = null;
				yield break;
				yield break;
			}
		}

		public NetworkCredential Credentials
		{
			get
			{
				return this._credentials;
			}
		}

		public bool EmitOnPing
		{
			get
			{
				return this._emitOnPing;
			}
			set
			{
				this._emitOnPing = value;
			}
		}

		public bool EnableRedirection
		{
			get
			{
				return this._enableRedirection;
			}
			set
			{
				string message = null;
				bool flag = !this._client;
				if (flag)
				{
					message = "This instance is not a client.";
					throw new InvalidOperationException(message);
				}
				bool flag2 = !this.canSet(out message);
				if (flag2)
				{
					this._logger.Warn(message);
				}
				else
				{
					object forState = this._forState;
					lock (forState)
					{
						bool flag3 = !this.canSet(out message);
						if (flag3)
						{
							this._logger.Warn(message);
						}
						else
						{
							this._enableRedirection = value;
						}
					}
				}
			}
		}

		public string Extensions
		{
			get
			{
				return this._extensions ?? string.Empty;
			}
		}

		public bool IsAlive
		{
			get
			{
				return this.ping(WebSocket.EmptyBytes);
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public Logger Log
		{
			get
			{
				return this._logger;
			}
			internal set
			{
				this._logger = value;
			}
		}

		public string Origin
		{
			get
			{
				return this._origin;
			}
			set
			{
				string message = null;
				bool flag = !this._client;
				if (flag)
				{
					message = "This instance is not a client.";
					throw new InvalidOperationException(message);
				}
				bool flag2 = !value.IsNullOrEmpty();
				if (flag2)
				{
					Uri uri;
					bool flag3 = !Uri.TryCreate(value, UriKind.Absolute, out uri);
					if (flag3)
					{
						message = "Not an absolute URI string.";
						throw new ArgumentException(message, "value");
					}
					bool flag4 = uri.Segments.Length > 1;
					if (flag4)
					{
						message = "It includes the path segments.";
						throw new ArgumentException(message, "value");
					}
				}
				bool flag5 = !this.canSet(out message);
				if (flag5)
				{
					this._logger.Warn(message);
				}
				else
				{
					object forState = this._forState;
					lock (forState)
					{
						bool flag6 = !this.canSet(out message);
						if (flag6)
						{
							this._logger.Warn(message);
						}
						else
						{
							this._origin = ((!value.IsNullOrEmpty()) ? value.TrimEnd(new char[]
							{
								'/'
							}) : value);
						}
					}
				}
			}
		}

		public string Protocol
		{
			get
			{
				return this._protocol ?? string.Empty;
			}
			internal set
			{
				this._protocol = value;
			}
		}

		public WebSocketState ReadyState
		{
			get
			{
				return this._readyState;
			}
		}

		public ClientSslConfiguration SslConfiguration
		{
			get
			{
				bool flag = !this._client;
				if (flag)
				{
					string message = "This instance is not a client.";
					throw new InvalidOperationException(message);
				}
				bool flag2 = !this._secure;
				if (flag2)
				{
					string message2 = "This instance does not use a secure connection.";
					throw new InvalidOperationException(message2);
				}
				return this.getSslConfiguration();
			}
		}

		public Uri Url
		{
			get
			{
				return this._client ? this._uri : this._context.RequestUri;
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._waitTime;
			}
			set
			{
				bool flag = value <= TimeSpan.Zero;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", "Zero or less.");
				}
				string message;
				bool flag2 = !this.canSet(out message);
				if (flag2)
				{
					this._logger.Warn(message);
				}
				else
				{
					object forState = this._forState;
					lock (forState)
					{
						bool flag3 = !this.canSet(out message);
						if (flag3)
						{
							this._logger.Warn(message);
						}
						else
						{
							this._waitTime = value;
						}
					}
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<CloseEventArgs> OnClose;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<ErrorEventArgs> OnError;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<MessageEventArgs> OnMessage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler OnOpen;

		private bool accept()
		{
			bool flag = this._readyState == WebSocketState.Open;
			bool result;
			if (flag)
			{
				string message = "The handshake request has already been accepted.";
				this._logger.Warn(message);
				result = false;
			}
			else
			{
				object forState = this._forState;
				lock (forState)
				{
					bool flag2 = this._readyState == WebSocketState.Open;
					if (flag2)
					{
						string message2 = "The handshake request has already been accepted.";
						this._logger.Warn(message2);
						result = false;
					}
					else
					{
						bool flag3 = this._readyState == WebSocketState.Closing;
						if (flag3)
						{
							string message3 = "The close process has set in.";
							this._logger.Error(message3);
							message3 = "An interruption has occurred while attempting to accept.";
							this.error(message3, null);
							result = false;
						}
						else
						{
							bool flag4 = this._readyState == WebSocketState.Closed;
							if (flag4)
							{
								string message4 = "The connection has been closed.";
								this._logger.Error(message4);
								message4 = "An interruption has occurred while attempting to accept.";
								this.error(message4, null);
								result = false;
							}
							else
							{
								try
								{
									bool flag5 = !this.acceptHandshake();
									if (flag5)
									{
										return false;
									}
								}
								catch (Exception ex)
								{
									this._logger.Fatal(ex.Message);
									this._logger.Debug(ex.ToString());
									string message5 = "An exception has occurred while attempting to accept.";
									this.fatal(message5, ex);
									return false;
								}
								this._readyState = WebSocketState.Open;
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private bool acceptHandshake()
		{
			this._logger.Debug(string.Format("A handshake request from {0}:\n{1}", this._context.UserEndPoint, this._context));
			string message;
			bool flag = !this.checkHandshakeRequest(this._context, out message);
			bool result;
			if (flag)
			{
				this._logger.Error(message);
				this.refuseHandshake(CloseStatusCode.ProtocolError, "A handshake error has occurred while attempting to accept.");
				result = false;
			}
			else
			{
				bool flag2 = !this.customCheckHandshakeRequest(this._context, out message);
				if (flag2)
				{
					this._logger.Error(message);
					this.refuseHandshake(CloseStatusCode.PolicyViolation, "A handshake error has occurred while attempting to accept.");
					result = false;
				}
				else
				{
					this._base64Key = this._context.Headers["Sec-WebSocket-Key"];
					bool flag3 = this._protocol != null;
					if (flag3)
					{
						IEnumerable<string> secWebSocketProtocols = this._context.SecWebSocketProtocols;
						this.processSecWebSocketProtocolClientHeader(secWebSocketProtocols);
					}
					bool flag4 = !this._ignoreExtensions;
					if (flag4)
					{
						string value = this._context.Headers["Sec-WebSocket-Extensions"];
						this.processSecWebSocketExtensionsClientHeader(value);
					}
					result = this.sendHttpResponse(this.createHandshakeResponse());
				}
			}
			return result;
		}

		private bool canSet(out string message)
		{
			message = null;
			bool flag = this._readyState == WebSocketState.Open;
			bool result;
			if (flag)
			{
				message = "The connection has already been established.";
				result = false;
			}
			else
			{
				bool flag2 = this._readyState == WebSocketState.Closing;
				if (flag2)
				{
					message = "The connection is closing.";
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private bool checkHandshakeRequest(WebSocketContext context, out string message)
		{
			message = null;
			bool flag = !context.IsWebSocketRequest;
			bool result;
			if (flag)
			{
				message = "Not a handshake request.";
				result = false;
			}
			else
			{
				bool flag2 = context.RequestUri == null;
				if (flag2)
				{
					message = "It specifies an invalid Request-URI.";
					result = false;
				}
				else
				{
					NameValueCollection headers = context.Headers;
					string text = headers["Sec-WebSocket-Key"];
					bool flag3 = text == null;
					if (flag3)
					{
						message = "It includes no Sec-WebSocket-Key header.";
						result = false;
					}
					else
					{
						bool flag4 = text.Length == 0;
						if (flag4)
						{
							message = "It includes an invalid Sec-WebSocket-Key header.";
							result = false;
						}
						else
						{
							string text2 = headers["Sec-WebSocket-Version"];
							bool flag5 = text2 == null;
							if (flag5)
							{
								message = "It includes no Sec-WebSocket-Version header.";
								result = false;
							}
							else
							{
								bool flag6 = text2 != "13";
								if (flag6)
								{
									message = "It includes an invalid Sec-WebSocket-Version header.";
									result = false;
								}
								else
								{
									string text3 = headers["Sec-WebSocket-Protocol"];
									bool flag7 = text3 != null && text3.Length == 0;
									if (flag7)
									{
										message = "It includes an invalid Sec-WebSocket-Protocol header.";
										result = false;
									}
									else
									{
										bool flag8 = !this._ignoreExtensions;
										if (flag8)
										{
											string text4 = headers["Sec-WebSocket-Extensions"];
											bool flag9 = text4 != null && text4.Length == 0;
											if (flag9)
											{
												message = "It includes an invalid Sec-WebSocket-Extensions header.";
												return false;
											}
										}
										result = true;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private bool checkHandshakeResponse(HttpResponse response, out string message)
		{
			message = null;
			bool isRedirect = response.IsRedirect;
			bool result;
			if (isRedirect)
			{
				message = "Indicates the redirection.";
				result = false;
			}
			else
			{
				bool isUnauthorized = response.IsUnauthorized;
				if (isUnauthorized)
				{
					message = "Requires the authentication.";
					result = false;
				}
				else
				{
					bool flag = !response.IsWebSocketResponse;
					if (flag)
					{
						message = "Not a WebSocket handshake response.";
						result = false;
					}
					else
					{
						NameValueCollection headers = response.Headers;
						bool flag2 = !this.validateSecWebSocketAcceptHeader(headers["Sec-WebSocket-Accept"]);
						if (flag2)
						{
							message = "Includes no Sec-WebSocket-Accept header, or it has an invalid value.";
							result = false;
						}
						else
						{
							bool flag3 = !this.validateSecWebSocketProtocolServerHeader(headers["Sec-WebSocket-Protocol"]);
							if (flag3)
							{
								message = "Includes no Sec-WebSocket-Protocol header, or it has an invalid value.";
								result = false;
							}
							else
							{
								bool flag4 = !this.validateSecWebSocketExtensionsServerHeader(headers["Sec-WebSocket-Extensions"]);
								if (flag4)
								{
									message = "Includes an invalid Sec-WebSocket-Extensions header.";
									result = false;
								}
								else
								{
									bool flag5 = !this.validateSecWebSocketVersionServerHeader(headers["Sec-WebSocket-Version"]);
									if (flag5)
									{
										message = "Includes an invalid Sec-WebSocket-Version header.";
										result = false;
									}
									else
									{
										result = true;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private static bool checkProtocols(string[] protocols, out string message)
		{
			message = null;
			Func<string, bool> condition = (string protocol) => protocol.IsNullOrEmpty() || !protocol.IsToken();
			bool flag = protocols.Contains(condition);
			bool result;
			if (flag)
			{
				message = "It contains a value that is not a token.";
				result = false;
			}
			else
			{
				bool flag2 = protocols.ContainsTwice();
				if (flag2)
				{
					message = "It contains a value twice.";
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private bool checkReceivedFrame(WebSocketFrame frame, out string message)
		{
			message = null;
			bool isMasked = frame.IsMasked;
			bool flag = this._client && isMasked;
			bool result;
			if (flag)
			{
				message = "A frame from the server is masked.";
				result = false;
			}
			else
			{
				bool flag2 = !this._client && !isMasked;
				if (flag2)
				{
					message = "A frame from a client is not masked.";
					result = false;
				}
				else
				{
					bool flag3 = this._inContinuation && frame.IsData;
					if (flag3)
					{
						message = "A data frame has been received while receiving continuation frames.";
						result = false;
					}
					else
					{
						bool flag4 = frame.IsCompressed && this._compression == CompressionMethod.None;
						if (flag4)
						{
							message = "A compressed frame has been received without any agreement for it.";
							result = false;
						}
						else
						{
							bool flag5 = frame.Rsv2 == Rsv.On;
							if (flag5)
							{
								message = "The RSV2 of a frame is non-zero without any negotiation for it.";
								result = false;
							}
							else
							{
								bool flag6 = frame.Rsv3 == Rsv.On;
								if (flag6)
								{
									message = "The RSV3 of a frame is non-zero without any negotiation for it.";
									result = false;
								}
								else
								{
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private void close(ushort code, string reason)
		{
			bool flag = this._readyState == WebSocketState.Closing;
			if (flag)
			{
				this._logger.Info("The closing is already in progress.");
			}
			else
			{
				bool flag2 = this._readyState == WebSocketState.Closed;
				if (flag2)
				{
					this._logger.Info("The connection has already been closed.");
				}
				else
				{
					bool flag3 = code == 1005;
					if (flag3)
					{
						this.close(PayloadData.Empty, true, true, false);
					}
					else
					{
						bool flag4 = !code.IsReserved();
						this.close(new PayloadData(code, reason), flag4, flag4, false);
					}
				}
			}
		}

		private void close(PayloadData payloadData, bool send, bool receive, bool received)
		{
			object forState = this._forState;
			lock (forState)
			{
				bool flag = this._readyState == WebSocketState.Closing;
				if (flag)
				{
					this._logger.Info("The closing is already in progress.");
					return;
				}
				bool flag2 = this._readyState == WebSocketState.Closed;
				if (flag2)
				{
					this._logger.Info("The connection has already been closed.");
					return;
				}
				send = (send && this._readyState == WebSocketState.Open);
				receive = (send && receive);
				this._readyState = WebSocketState.Closing;
			}
			this._logger.Trace("Begin closing the connection.");
			bool clean = this.closeHandshake(payloadData, send, receive, received);
			this.releaseResources();
			this._logger.Trace("End closing the connection.");
			this._readyState = WebSocketState.Closed;
			CloseEventArgs e = new CloseEventArgs(payloadData, clean);
			try
			{
				this.OnClose.Emit(this, e);
			}
			catch (Exception ex)
			{
				this._logger.Error(ex.Message);
				this._logger.Debug(ex.ToString());
			}
		}

		private void closeAsync(ushort code, string reason)
		{
			bool flag = this._readyState == WebSocketState.Closing;
			if (flag)
			{
				this._logger.Info("The closing is already in progress.");
			}
			else
			{
				bool flag2 = this._readyState == WebSocketState.Closed;
				if (flag2)
				{
					this._logger.Info("The connection has already been closed.");
				}
				else
				{
					bool flag3 = code == 1005;
					if (flag3)
					{
						this.closeAsync(PayloadData.Empty, true, true, false);
					}
					else
					{
						bool flag4 = !code.IsReserved();
						this.closeAsync(new PayloadData(code, reason), flag4, flag4, false);
					}
				}
			}
		}

		private void closeAsync(PayloadData payloadData, bool send, bool receive, bool received)
		{
			Action<PayloadData, bool, bool, bool> closer = new Action<PayloadData, bool, bool, bool>(this.close);
			closer.BeginInvoke(payloadData, send, receive, received, delegate(IAsyncResult ar)
			{
				closer.EndInvoke(ar);
			}, null);
		}

		private bool closeHandshake(byte[] frameAsBytes, bool receive, bool received)
		{
			bool flag = frameAsBytes != null && this.sendBytes(frameAsBytes);
			bool flag2 = !received && flag && receive && this._receivingExited != null;
			bool flag3 = flag2;
			if (flag3)
			{
				received = this._receivingExited.WaitOne(this._waitTime);
			}
			bool flag4 = flag && received;
			this._logger.Debug(string.Format("Was clean?: {0}\n  sent: {1}\n  received: {2}", flag4, flag, received));
			return flag4;
		}

		private bool closeHandshake(PayloadData payloadData, bool send, bool receive, bool received)
		{
			bool flag = false;
			if (send)
			{
				WebSocketFrame webSocketFrame = WebSocketFrame.CreateCloseFrame(payloadData, this._client);
				flag = this.sendBytes(webSocketFrame.ToArray());
				bool client = this._client;
				if (client)
				{
					webSocketFrame.Unmask();
				}
			}
			bool flag2 = !received && flag && receive && this._receivingExited != null;
			bool flag3 = flag2;
			if (flag3)
			{
				received = this._receivingExited.WaitOne(this._waitTime);
			}
			bool flag4 = flag && received;
			this._logger.Debug(string.Format("Was clean?: {0}\n  sent: {1}\n  received: {2}", flag4, flag, received));
			return flag4;
		}

		private bool connect()
		{
			bool flag = this._readyState == WebSocketState.Open;
			bool result;
			if (flag)
			{
				string message = "The connection has already been established.";
				this._logger.Warn(message);
				result = false;
			}
			else
			{
				object forState = this._forState;
				lock (forState)
				{
					bool flag2 = this._readyState == WebSocketState.Open;
					if (flag2)
					{
						string message2 = "The connection has already been established.";
						this._logger.Warn(message2);
						result = false;
					}
					else
					{
						bool flag3 = this._readyState == WebSocketState.Closing;
						if (flag3)
						{
							string message3 = "The close process has set in.";
							this._logger.Error(message3);
							message3 = "An interruption has occurred while attempting to connect.";
							this.error(message3, null);
							result = false;
						}
						else
						{
							bool flag4 = this._retryCountForConnect > WebSocket._maxRetryCountForConnect;
							if (flag4)
							{
								string message4 = "An opportunity for reconnecting has been lost.";
								this._logger.Error(message4);
								message4 = "An interruption has occurred while attempting to connect.";
								this.error(message4, null);
								result = false;
							}
							else
							{
								this._readyState = WebSocketState.Connecting;
								try
								{
									this.doHandshake();
								}
								catch (Exception ex)
								{
									this._retryCountForConnect++;
									this._logger.Fatal(ex.Message);
									this._logger.Debug(ex.ToString());
									string message5 = "An exception has occurred while attempting to connect.";
									this.fatal(message5, ex);
									return false;
								}
								this._retryCountForConnect = 1;
								this._readyState = WebSocketState.Open;
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private string createExtensions()
		{
			StringBuilder stringBuilder = new StringBuilder(80);
			bool flag = this._compression > CompressionMethod.None;
			if (flag)
			{
				string arg = this._compression.ToExtensionString(new string[]
				{
					"server_no_context_takeover",
					"client_no_context_takeover"
				});
				stringBuilder.AppendFormat("{0}, ", arg);
			}
			int length = stringBuilder.Length;
			bool flag2 = length > 2;
			string result;
			if (flag2)
			{
				stringBuilder.Length = length - 2;
				result = stringBuilder.ToString();
			}
			else
			{
				result = null;
			}
			return result;
		}

		private HttpResponse createHandshakeFailureResponse(HttpStatusCode code)
		{
			HttpResponse httpResponse = HttpResponse.CreateCloseResponse(code);
			httpResponse.Headers["Sec-WebSocket-Version"] = "13";
			return httpResponse;
		}

		private HttpRequest createHandshakeRequest()
		{
			HttpRequest httpRequest = HttpRequest.CreateWebSocketRequest(this._uri);
			NameValueCollection headers = httpRequest.Headers;
			bool flag = !this._origin.IsNullOrEmpty();
			if (flag)
			{
				headers["Origin"] = this._origin;
			}
			headers["Sec-WebSocket-Key"] = this._base64Key;
			this._protocolsRequested = (this._protocols != null);
			bool protocolsRequested = this._protocolsRequested;
			if (protocolsRequested)
			{
				headers["Sec-WebSocket-Protocol"] = this._protocols.ToString(", ");
			}
			this._extensionsRequested = (this._compression > CompressionMethod.None);
			bool extensionsRequested = this._extensionsRequested;
			if (extensionsRequested)
			{
				headers["Sec-WebSocket-Extensions"] = this.createExtensions();
			}
			headers["Sec-WebSocket-Version"] = "13";
			AuthenticationResponse authenticationResponse = null;
			bool flag2 = this._authChallenge != null && this._credentials != null;
			if (flag2)
			{
				authenticationResponse = new AuthenticationResponse(this._authChallenge, this._credentials, this._nonceCount);
				this._nonceCount = authenticationResponse.NonceCount;
			}
			else
			{
				bool preAuth = this._preAuth;
				if (preAuth)
				{
					authenticationResponse = new AuthenticationResponse(this._credentials);
				}
			}
			bool flag3 = authenticationResponse != null;
			if (flag3)
			{
				headers["Authorization"] = authenticationResponse.ToString();
			}
			bool flag4 = this._cookies.Count > 0;
			if (flag4)
			{
				httpRequest.SetCookies(this._cookies);
			}
			return httpRequest;
		}

		private HttpResponse createHandshakeResponse()
		{
			HttpResponse httpResponse = HttpResponse.CreateWebSocketResponse();
			NameValueCollection headers = httpResponse.Headers;
			headers["Sec-WebSocket-Accept"] = WebSocket.CreateResponseKey(this._base64Key);
			bool flag = this._protocol != null;
			if (flag)
			{
				headers["Sec-WebSocket-Protocol"] = this._protocol;
			}
			bool flag2 = this._extensions != null;
			if (flag2)
			{
				headers["Sec-WebSocket-Extensions"] = this._extensions;
			}
			bool flag3 = this._cookies.Count > 0;
			if (flag3)
			{
				httpResponse.SetCookies(this._cookies);
			}
			return httpResponse;
		}

		private bool customCheckHandshakeRequest(WebSocketContext context, out string message)
		{
			message = null;
			bool flag = this._handshakeRequestChecker == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				message = this._handshakeRequestChecker(context);
				result = (message == null);
			}
			return result;
		}

		private MessageEventArgs dequeueFromMessageEventQueue()
		{
			object forMessageEventQueue = this._forMessageEventQueue;
			MessageEventArgs result;
			lock (forMessageEventQueue)
			{
				result = ((this._messageEventQueue.Count > 0) ? this._messageEventQueue.Dequeue() : null);
			}
			return result;
		}

		private void doHandshake()
		{
			this.setClientStream();
			HttpResponse httpResponse = this.sendHandshakeRequest();
			string message;
			bool flag = !this.checkHandshakeResponse(httpResponse, out message);
			if (flag)
			{
				throw new WebSocketException(CloseStatusCode.ProtocolError, message);
			}
			bool protocolsRequested = this._protocolsRequested;
			if (protocolsRequested)
			{
				this._protocol = httpResponse.Headers["Sec-WebSocket-Protocol"];
			}
			bool extensionsRequested = this._extensionsRequested;
			if (extensionsRequested)
			{
				this.processSecWebSocketExtensionsServerHeader(httpResponse.Headers["Sec-WebSocket-Extensions"]);
			}
			this.processCookies(httpResponse.Cookies);
		}

		private void enqueueToMessageEventQueue(MessageEventArgs e)
		{
			object forMessageEventQueue = this._forMessageEventQueue;
			lock (forMessageEventQueue)
			{
				this._messageEventQueue.Enqueue(e);
			}
		}

		private void error(string message, Exception exception)
		{
			try
			{
				this.OnError.Emit(this, new ErrorEventArgs(message, exception));
			}
			catch (Exception ex)
			{
				this._logger.Error(ex.Message);
				this._logger.Debug(ex.ToString());
			}
		}

		private void fatal(string message, Exception exception)
		{
			CloseStatusCode code = (exception is WebSocketException) ? ((WebSocketException)exception).Code : CloseStatusCode.Abnormal;
			this.fatal(message, (ushort)code);
		}

		private void fatal(string message, ushort code)
		{
			PayloadData payloadData = new PayloadData(code, message);
			this.close(payloadData, !code.IsReserved(), false, false);
		}

		private void fatal(string message, CloseStatusCode code)
		{
			this.fatal(message, (ushort)code);
		}

		private ClientSslConfiguration getSslConfiguration()
		{
			bool flag = this._sslConfig == null;
			if (flag)
			{
				this._sslConfig = new ClientSslConfiguration(this._uri.DnsSafeHost);
			}
			return this._sslConfig;
		}

		private void init()
		{
			this._compression = CompressionMethod.None;
			this._cookies = new CookieCollection();
			this._forPing = new object();
			this._forSend = new object();
			this._forState = new object();
			this._messageEventQueue = new Queue<MessageEventArgs>();
			this._forMessageEventQueue = ((ICollection)this._messageEventQueue).SyncRoot;
			this._readyState = WebSocketState.Connecting;
		}

		private void message()
		{
			MessageEventArgs obj = null;
			object forMessageEventQueue = this._forMessageEventQueue;
			lock (forMessageEventQueue)
			{
				bool flag = this._inMessage || this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open;
				if (flag)
				{
					return;
				}
				this._inMessage = true;
				obj = this._messageEventQueue.Dequeue();
			}
			this._message(obj);
		}

		private void messagec(MessageEventArgs e)
		{
			for (;;)
			{
				try
				{
					this.OnMessage.Emit(this, e);
				}
				catch (Exception ex)
				{
					this._logger.Error(ex.ToString());
					this.error("An error has occurred during an OnMessage event.", ex);
				}
				object forMessageEventQueue = this._forMessageEventQueue;
				lock (forMessageEventQueue)
				{
					bool flag = this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open;
					if (flag)
					{
						this._inMessage = false;
						break;
					}
					e = this._messageEventQueue.Dequeue();
				}
			}
		}

		private void messages(MessageEventArgs e)
		{
			try
			{
				this.OnMessage.Emit(this, e);
			}
			catch (Exception ex)
			{
				this._logger.Error(ex.ToString());
				this.error("An error has occurred during an OnMessage event.", ex);
			}
			object forMessageEventQueue = this._forMessageEventQueue;
			lock (forMessageEventQueue)
			{
				bool flag = this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open;
				if (flag)
				{
					this._inMessage = false;
					return;
				}
				e = this._messageEventQueue.Dequeue();
			}
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				this.messages(e);
			});
		}

		private void open()
		{
			this._inMessage = true;
			this.startReceiving();
			try
			{
				this.OnOpen.Emit(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				this._logger.Error(ex.ToString());
				this.error("An error has occurred during the OnOpen event.", ex);
			}
			MessageEventArgs obj = null;
			object forMessageEventQueue = this._forMessageEventQueue;
			lock (forMessageEventQueue)
			{
				bool flag = this._messageEventQueue.Count == 0 || this._readyState != WebSocketState.Open;
				if (flag)
				{
					this._inMessage = false;
					return;
				}
				obj = this._messageEventQueue.Dequeue();
			}
			this._message.BeginInvoke(obj, delegate(IAsyncResult ar)
			{
				this._message.EndInvoke(ar);
			}, null);
		}

		private bool ping(byte[] data)
		{
			bool flag = this._readyState != WebSocketState.Open;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ManualResetEvent pongReceived = this._pongReceived;
				bool flag2 = pongReceived == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					object forPing = this._forPing;
					lock (forPing)
					{
						try
						{
							pongReceived.Reset();
							bool flag3 = !this.send(Fin.Final, Opcode.Ping, data, false);
							if (flag3)
							{
								result = false;
							}
							else
							{
								result = pongReceived.WaitOne(this._waitTime);
							}
						}
						catch (ObjectDisposedException)
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		private bool processCloseFrame(WebSocketFrame frame)
		{
			PayloadData payloadData = frame.PayloadData;
			this.close(payloadData, !payloadData.HasReservedCode, false, true);
			return false;
		}

		private void processCookies(CookieCollection cookies)
		{
			bool flag = cookies.Count == 0;
			if (!flag)
			{
				this._cookies.SetOrRemove(cookies);
			}
		}

		private bool processDataFrame(WebSocketFrame frame)
		{
			this.enqueueToMessageEventQueue(frame.IsCompressed ? new MessageEventArgs(frame.Opcode, frame.PayloadData.ApplicationData.Decompress(this._compression)) : new MessageEventArgs(frame));
			return true;
		}

		private bool processFragmentFrame(WebSocketFrame frame)
		{
			bool flag = !this._inContinuation;
			if (flag)
			{
				bool isContinuation = frame.IsContinuation;
				if (isContinuation)
				{
					return true;
				}
				this._fragmentsOpcode = frame.Opcode;
				this._fragmentsCompressed = frame.IsCompressed;
				this._fragmentsBuffer = new MemoryStream();
				this._inContinuation = true;
			}
			this._fragmentsBuffer.WriteBytes(frame.PayloadData.ApplicationData, 1024);
			bool isFinal = frame.IsFinal;
			if (isFinal)
			{
				using (this._fragmentsBuffer)
				{
					byte[] rawData = this._fragmentsCompressed ? this._fragmentsBuffer.DecompressToArray(this._compression) : this._fragmentsBuffer.ToArray();
					this.enqueueToMessageEventQueue(new MessageEventArgs(this._fragmentsOpcode, rawData));
				}
				this._fragmentsBuffer = null;
				this._inContinuation = false;
			}
			return true;
		}

		private bool processPingFrame(WebSocketFrame frame)
		{
			this._logger.Trace("A ping was received.");
			WebSocketFrame webSocketFrame = WebSocketFrame.CreatePongFrame(frame.PayloadData, this._client);
			object forState = this._forState;
			lock (forState)
			{
				bool flag = this._readyState != WebSocketState.Open;
				if (flag)
				{
					this._logger.Error("The connection is closing.");
					return true;
				}
				bool flag2 = !this.sendBytes(webSocketFrame.ToArray());
				if (flag2)
				{
					return false;
				}
			}
			this._logger.Trace("A pong to this ping has been sent.");
			bool emitOnPing = this._emitOnPing;
			if (emitOnPing)
			{
				bool client = this._client;
				if (client)
				{
					webSocketFrame.Unmask();
				}
				this.enqueueToMessageEventQueue(new MessageEventArgs(frame));
			}
			return true;
		}

		private bool processPongFrame(WebSocketFrame frame)
		{
			this._logger.Trace("A pong was received.");
			try
			{
				this._pongReceived.Set();
			}
			catch (NullReferenceException ex)
			{
				this._logger.Error(ex.Message);
				this._logger.Debug(ex.ToString());
				return false;
			}
			catch (ObjectDisposedException ex2)
			{
				this._logger.Error(ex2.Message);
				this._logger.Debug(ex2.ToString());
				return false;
			}
			this._logger.Trace("It has been signaled.");
			return true;
		}

		private bool processReceivedFrame(WebSocketFrame frame)
		{
			string message;
			bool flag = !this.checkReceivedFrame(frame, out message);
			if (flag)
			{
				throw new WebSocketException(CloseStatusCode.ProtocolError, message);
			}
			frame.Unmask();
			return frame.IsFragment ? this.processFragmentFrame(frame) : (frame.IsData ? this.processDataFrame(frame) : (frame.IsPing ? this.processPingFrame(frame) : (frame.IsPong ? this.processPongFrame(frame) : (frame.IsClose ? this.processCloseFrame(frame) : this.processUnsupportedFrame(frame)))));
		}

		private void processSecWebSocketExtensionsClientHeader(string value)
		{
			bool flag = value == null;
			if (!flag)
			{
				StringBuilder stringBuilder = new StringBuilder(80);
				bool flag2 = false;
				foreach (string text in value.SplitHeaderValue(new char[]
				{
					','
				}))
				{
					string text2 = text.Trim();
					bool flag3 = text2.Length == 0;
					if (!flag3)
					{
						bool flag4 = !flag2;
						if (flag4)
						{
							bool flag5 = text2.IsCompressionExtension(CompressionMethod.Deflate);
							if (flag5)
							{
								this._compression = CompressionMethod.Deflate;
								stringBuilder.AppendFormat("{0}, ", this._compression.ToExtensionString(new string[]
								{
									"client_no_context_takeover",
									"server_no_context_takeover"
								}));
								flag2 = true;
							}
						}
					}
				}
				int length = stringBuilder.Length;
				bool flag6 = length <= 2;
				if (!flag6)
				{
					stringBuilder.Length = length - 2;
					this._extensions = stringBuilder.ToString();
				}
			}
		}

		private void processSecWebSocketExtensionsServerHeader(string value)
		{
			bool flag = value == null;
			if (flag)
			{
				this._compression = CompressionMethod.None;
			}
			else
			{
				this._extensions = value;
			}
		}

		private void processSecWebSocketProtocolClientHeader(IEnumerable<string> values)
		{
			bool flag = values.Contains((string val) => val == this._protocol);
			if (!flag)
			{
				this._protocol = null;
			}
		}

		private bool processUnsupportedFrame(WebSocketFrame frame)
		{
			this._logger.Fatal("An unsupported frame:" + frame.PrintToString(false));
			this.fatal("There is no way to handle it.", CloseStatusCode.PolicyViolation);
			return false;
		}

		private void refuseHandshake(CloseStatusCode code, string reason)
		{
			this._readyState = WebSocketState.Closing;
			HttpResponse response = this.createHandshakeFailureResponse(HttpStatusCode.BadRequest);
			this.sendHttpResponse(response);
			this.releaseServerResources();
			this._readyState = WebSocketState.Closed;
			CloseEventArgs e = new CloseEventArgs((ushort)code, reason, false);
			try
			{
				this.OnClose.Emit(this, e);
			}
			catch (Exception ex)
			{
				this._logger.Error(ex.Message);
				this._logger.Debug(ex.ToString());
			}
		}

		private void releaseClientResources()
		{
			bool flag = this._stream != null;
			if (flag)
			{
				this._stream.Dispose();
				this._stream = null;
			}
			bool flag2 = this._tcpClient != null;
			if (flag2)
			{
				this._tcpClient.Close();
				this._tcpClient = null;
			}
		}

		private void releaseCommonResources()
		{
			bool flag = this._fragmentsBuffer != null;
			if (flag)
			{
				this._fragmentsBuffer.Dispose();
				this._fragmentsBuffer = null;
				this._inContinuation = false;
			}
			bool flag2 = this._pongReceived != null;
			if (flag2)
			{
				this._pongReceived.Close();
				this._pongReceived = null;
			}
			bool flag3 = this._receivingExited != null;
			if (flag3)
			{
				this._receivingExited.Close();
				this._receivingExited = null;
			}
		}

		private void releaseResources()
		{
			bool client = this._client;
			if (client)
			{
				this.releaseClientResources();
			}
			else
			{
				this.releaseServerResources();
			}
			this.releaseCommonResources();
		}

		private void releaseServerResources()
		{
			bool flag = this._closeContext == null;
			if (!flag)
			{
				this._closeContext();
				this._closeContext = null;
				this._stream = null;
				this._context = null;
			}
		}

		private bool send(Opcode opcode, Stream stream)
		{
			object forSend = this._forSend;
			bool result;
			lock (forSend)
			{
				Stream stream2 = stream;
				bool flag = false;
				bool flag2 = false;
				try
				{
					bool flag3 = this._compression > CompressionMethod.None;
					if (flag3)
					{
						stream = stream.Compress(this._compression);
						flag = true;
					}
					flag2 = this.send(opcode, stream, flag);
					bool flag4 = !flag2;
					if (flag4)
					{
						this.error("A send has been interrupted.", null);
					}
				}
				catch (Exception ex)
				{
					this._logger.Error(ex.ToString());
					this.error("An error has occurred during a send.", ex);
				}
				finally
				{
					bool flag5 = flag;
					if (flag5)
					{
						stream.Dispose();
					}
					stream2.Dispose();
				}
				result = flag2;
			}
			return result;
		}

		private bool send(Opcode opcode, Stream stream, bool compressed)
		{
			long length = stream.Length;
			bool flag = length == 0L;
			bool result;
			if (flag)
			{
				result = this.send(Fin.Final, opcode, WebSocket.EmptyBytes, false);
			}
			else
			{
				long num = length / (long)WebSocket.FragmentLength;
				int num2 = (int)(length % (long)WebSocket.FragmentLength);
				bool flag2 = num == 0L;
				if (flag2)
				{
					byte[] array = new byte[num2];
					result = (stream.Read(array, 0, num2) == num2 && this.send(Fin.Final, opcode, array, compressed));
				}
				else
				{
					bool flag3 = num == 1L && num2 == 0;
					if (flag3)
					{
						byte[] array = new byte[WebSocket.FragmentLength];
						result = (stream.Read(array, 0, WebSocket.FragmentLength) == WebSocket.FragmentLength && this.send(Fin.Final, opcode, array, compressed));
					}
					else
					{
						byte[] array = new byte[WebSocket.FragmentLength];
						bool flag4 = stream.Read(array, 0, WebSocket.FragmentLength) == WebSocket.FragmentLength && this.send(Fin.More, opcode, array, compressed);
						bool flag5 = !flag4;
						if (flag5)
						{
							result = false;
						}
						else
						{
							long num3 = (num2 == 0) ? (num - 2L) : (num - 1L);
							for (long num4 = 0L; num4 < num3; num4 += 1L)
							{
								flag4 = (stream.Read(array, 0, WebSocket.FragmentLength) == WebSocket.FragmentLength && this.send(Fin.More, Opcode.Cont, array, false));
								bool flag6 = !flag4;
								if (flag6)
								{
									return false;
								}
							}
							bool flag7 = num2 == 0;
							if (flag7)
							{
								num2 = WebSocket.FragmentLength;
							}
							else
							{
								array = new byte[num2];
							}
							result = (stream.Read(array, 0, num2) == num2 && this.send(Fin.Final, Opcode.Cont, array, false));
						}
					}
				}
			}
			return result;
		}

		private bool send(Fin fin, Opcode opcode, byte[] data, bool compressed)
		{
			object forState = this._forState;
			bool result;
			lock (forState)
			{
				bool flag = this._readyState != WebSocketState.Open;
				if (flag)
				{
					this._logger.Error("The connection is closing.");
					result = false;
				}
				else
				{
					WebSocketFrame webSocketFrame = new WebSocketFrame(fin, opcode, data, compressed, this._client);
					result = this.sendBytes(webSocketFrame.ToArray());
				}
			}
			return result;
		}

		private void sendAsync(Opcode opcode, Stream stream, Action<bool> completed)
		{
			Func<Opcode, Stream, bool> sender = new Func<Opcode, Stream, bool>(this.send);
			sender.BeginInvoke(opcode, stream, delegate(IAsyncResult ar)
			{
				try
				{
					bool obj = sender.EndInvoke(ar);
					bool flag = completed != null;
					if (flag)
					{
						completed(obj);
					}
				}
				catch (Exception ex)
				{
					this._logger.Error(ex.ToString());
					this.error("An error has occurred during the callback for an async send.", ex);
				}
			}, null);
		}

		private bool sendBytes(byte[] bytes)
		{
			try
			{
				this._stream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
				this._logger.Error(ex.Message);
				this._logger.Debug(ex.ToString());
				return false;
			}
			return true;
		}

		private HttpResponse sendHandshakeRequest()
		{
			HttpRequest httpRequest = this.createHandshakeRequest();
			HttpResponse httpResponse = this.sendHttpRequest(httpRequest, 90000);
			bool isUnauthorized = httpResponse.IsUnauthorized;
			if (isUnauthorized)
			{
				string text = httpResponse.Headers["WWW-Authenticate"];
				this._logger.Warn(string.Format("Received an authentication requirement for '{0}'.", text));
				bool flag = text.IsNullOrEmpty();
				if (flag)
				{
					this._logger.Error("No authentication challenge is specified.");
					return httpResponse;
				}
				this._authChallenge = AuthenticationChallenge.Parse(text);
				bool flag2 = this._authChallenge == null;
				if (flag2)
				{
					this._logger.Error("An invalid authentication challenge is specified.");
					return httpResponse;
				}
				bool flag3 = this._credentials != null && (!this._preAuth || this._authChallenge.Scheme == AuthenticationSchemes.Digest);
				if (flag3)
				{
					bool hasConnectionClose = httpResponse.HasConnectionClose;
					if (hasConnectionClose)
					{
						this.releaseClientResources();
						this.setClientStream();
					}
					AuthenticationResponse authenticationResponse = new AuthenticationResponse(this._authChallenge, this._credentials, this._nonceCount);
					this._nonceCount = authenticationResponse.NonceCount;
					httpRequest.Headers["Authorization"] = authenticationResponse.ToString();
					httpResponse = this.sendHttpRequest(httpRequest, 15000);
				}
			}
			bool isRedirect = httpResponse.IsRedirect;
			if (isRedirect)
			{
				string text2 = httpResponse.Headers["Location"];
				this._logger.Warn(string.Format("Received a redirection to '{0}'.", text2));
				bool enableRedirection = this._enableRedirection;
				if (enableRedirection)
				{
					bool flag4 = text2.IsNullOrEmpty();
					if (flag4)
					{
						this._logger.Error("No url to redirect is located.");
						return httpResponse;
					}
					Uri uri;
					string str;
					bool flag5 = !text2.TryCreateWebSocketUri(out uri, out str);
					if (flag5)
					{
						this._logger.Error("An invalid url to redirect is located: " + str);
						return httpResponse;
					}
					this.releaseClientResources();
					this._uri = uri;
					this._secure = (uri.Scheme == "wss");
					this.setClientStream();
					return this.sendHandshakeRequest();
				}
			}
			return httpResponse;
		}

		private HttpResponse sendHttpRequest(HttpRequest request, int millisecondsTimeout)
		{
			this._logger.Debug("A request to the server:\n" + request.ToString());
			HttpResponse response = request.GetResponse(this._stream, millisecondsTimeout);
			this._logger.Debug("A response to this request:\n" + response.ToString());
			return response;
		}

		private bool sendHttpResponse(HttpResponse response)
		{
			this._logger.Debug(string.Format("A response to {0}:\n{1}", this._context.UserEndPoint, response));
			return this.sendBytes(response.ToByteArray());
		}

		private void sendProxyConnectRequest()
		{
			HttpRequest httpRequest = HttpRequest.CreateConnectRequest(this._uri);
			HttpResponse httpResponse = this.sendHttpRequest(httpRequest, 90000);
			bool isProxyAuthenticationRequired = httpResponse.IsProxyAuthenticationRequired;
			if (isProxyAuthenticationRequired)
			{
				string text = httpResponse.Headers["Proxy-Authenticate"];
				this._logger.Warn(string.Format("Received a proxy authentication requirement for '{0}'.", text));
				bool flag = text.IsNullOrEmpty();
				if (flag)
				{
					throw new WebSocketException("No proxy authentication challenge is specified.");
				}
				AuthenticationChallenge authenticationChallenge = AuthenticationChallenge.Parse(text);
				bool flag2 = authenticationChallenge == null;
				if (flag2)
				{
					throw new WebSocketException("An invalid proxy authentication challenge is specified.");
				}
				bool flag3 = this._proxyCredentials != null;
				if (flag3)
				{
					bool hasConnectionClose = httpResponse.HasConnectionClose;
					if (hasConnectionClose)
					{
						this.releaseClientResources();
						this._tcpClient = new TcpClient(this._proxyUri.DnsSafeHost, this._proxyUri.Port);
						this._stream = this._tcpClient.GetStream();
					}
					AuthenticationResponse authenticationResponse = new AuthenticationResponse(authenticationChallenge, this._proxyCredentials, 0U);
					httpRequest.Headers["Proxy-Authorization"] = authenticationResponse.ToString();
					httpResponse = this.sendHttpRequest(httpRequest, 15000);
				}
				bool isProxyAuthenticationRequired2 = httpResponse.IsProxyAuthenticationRequired;
				if (isProxyAuthenticationRequired2)
				{
					throw new WebSocketException("A proxy authentication is required.");
				}
			}
			bool flag4 = httpResponse.StatusCode[0] != '2';
			if (flag4)
			{
				throw new WebSocketException("The proxy has failed a connection to the requested host and port.");
			}
		}

		private void setClientStream()
		{
			bool flag = this._proxyUri != null;
			if (flag)
			{
				this._tcpClient = new TcpClient(this._proxyUri.DnsSafeHost, this._proxyUri.Port);
				this._stream = this._tcpClient.GetStream();
				this.sendProxyConnectRequest();
			}
			else
			{
				this._tcpClient = new TcpClient(this._uri.DnsSafeHost, this._uri.Port);
				this._stream = this._tcpClient.GetStream();
			}
			bool secure = this._secure;
			if (secure)
			{
				ClientSslConfiguration sslConfiguration = this.getSslConfiguration();
				string targetHost = sslConfiguration.TargetHost;
				bool flag2 = targetHost != this._uri.DnsSafeHost;
				if (flag2)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, "An invalid host name is specified.");
				}
				try
				{
					SslStream sslStream = new SslStream(this._stream, false, sslConfiguration.ServerCertificateValidationCallback, sslConfiguration.ClientCertificateSelectionCallback);
					sslStream.AuthenticateAsClient(targetHost, sslConfiguration.ClientCertificates, sslConfiguration.EnabledSslProtocols, sslConfiguration.CheckCertificateRevocation);
					this._stream = sslStream;
				}
				catch (Exception innerException)
				{
					throw new WebSocketException(CloseStatusCode.TlsHandshakeFailure, innerException);
				}
			}
		}

		private void startReceiving()
		{
			bool flag = this._messageEventQueue.Count > 0;
			if (flag)
			{
				this._messageEventQueue.Clear();
			}
			this._pongReceived = new ManualResetEvent(false);
			this._receivingExited = new ManualResetEvent(false);
			Action receive = null;
			Action<WebSocketFrame> <>9__1;
			Action<Exception> <>9__2;
			receive = delegate()
			{
				Stream stream = this._stream;
				bool unmask = false;
				Action<WebSocketFrame> completed;
				if ((completed = <>9__1) == null)
				{
					completed = (<>9__1 = delegate(WebSocketFrame frame)
					{
						bool flag2 = !this.processReceivedFrame(frame) || this._readyState == WebSocketState.Closed;
						if (flag2)
						{
							ManualResetEvent receivingExited = this._receivingExited;
							bool flag3 = receivingExited != null;
							if (flag3)
							{
								receivingExited.Set();
							}
						}
						else
						{
							receive();
							bool flag4 = this._inMessage || !this.HasMessage || this._readyState != WebSocketState.Open;
							if (!flag4)
							{
								this.message();
							}
						}
					});
				}
				Action<Exception> error;
				if ((error = <>9__2) == null)
				{
					error = (<>9__2 = delegate(Exception ex)
					{
						this._logger.Fatal(ex.ToString());
						this.fatal("An exception has occurred while receiving.", ex);
					});
				}
				WebSocketFrame.ReadFrameAsync(stream, unmask, completed, error);
			};
			receive();
		}

		private bool validateSecWebSocketAcceptHeader(string value)
		{
			return value != null && value == WebSocket.CreateResponseKey(this._base64Key);
		}

		private bool validateSecWebSocketExtensionsServerHeader(string value)
		{
			bool flag = value == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = value.Length == 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this._extensionsRequested;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this._compression > CompressionMethod.None;
						foreach (string text in value.SplitHeaderValue(new char[]
						{
							','
						}))
						{
							string text2 = text.Trim();
							bool flag5 = flag4 && text2.IsCompressionExtension(this._compression);
							if (!flag5)
							{
								return false;
							}
							bool flag6 = !text2.Contains("server_no_context_takeover");
							if (flag6)
							{
								this._logger.Error("The server hasn't sent back 'server_no_context_takeover'.");
								return false;
							}
							bool flag7 = !text2.Contains("client_no_context_takeover");
							if (flag7)
							{
								this._logger.Warn("The server hasn't sent back 'client_no_context_takeover'.");
							}
							string method = this._compression.ToExtensionString(new string[0]);
							bool flag8 = text2.SplitHeaderValue(new char[]
							{
								';'
							}).Contains(delegate(string t)
							{
								t = t.Trim();
								return t != method && t != "server_no_context_takeover" && t != "client_no_context_takeover";
							});
							bool flag9 = flag8;
							if (flag9)
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		private bool validateSecWebSocketProtocolServerHeader(string value)
		{
			bool flag = value == null;
			bool result;
			if (flag)
			{
				result = !this._protocolsRequested;
			}
			else
			{
				bool flag2 = value.Length == 0;
				result = (!flag2 && this._protocolsRequested && this._protocols.Contains((string p) => p == value));
			}
			return result;
		}

		private bool validateSecWebSocketVersionServerHeader(string value)
		{
			return value == null || value == "13";
		}

		internal void Close(HttpResponse response)
		{
			this._readyState = WebSocketState.Closing;
			this.sendHttpResponse(response);
			this.releaseServerResources();
			this._readyState = WebSocketState.Closed;
		}

		internal void Close(HttpStatusCode code)
		{
			this.Close(this.createHandshakeFailureResponse(code));
		}

		internal void Close(PayloadData payloadData, byte[] frameAsBytes)
		{
			object forState = this._forState;
			lock (forState)
			{
				bool flag = this._readyState == WebSocketState.Closing;
				if (flag)
				{
					this._logger.Info("The closing is already in progress.");
					return;
				}
				bool flag2 = this._readyState == WebSocketState.Closed;
				if (flag2)
				{
					this._logger.Info("The connection has already been closed.");
					return;
				}
				this._readyState = WebSocketState.Closing;
			}
			this._logger.Trace("Begin closing the connection.");
			bool flag3 = frameAsBytes != null && this.sendBytes(frameAsBytes);
			bool flag4 = flag3 && this._receivingExited != null && this._receivingExited.WaitOne(this._waitTime);
			bool flag5 = flag3 && flag4;
			this._logger.Debug(string.Format("Was clean?: {0}\n  sent: {1}\n  received: {2}", flag5, flag3, flag4));
			this.releaseServerResources();
			this.releaseCommonResources();
			this._logger.Trace("End closing the connection.");
			this._readyState = WebSocketState.Closed;
			CloseEventArgs e = new CloseEventArgs(payloadData, flag5);
			try
			{
				this.OnClose.Emit(this, e);
			}
			catch (Exception ex)
			{
				this._logger.Error(ex.Message);
				this._logger.Debug(ex.ToString());
			}
		}

		internal static string CreateBase64Key()
		{
			byte[] array = new byte[16];
			WebSocket.RandomNumber.GetBytes(array);
			return Convert.ToBase64String(array);
		}

		internal static string CreateResponseKey(string base64Key)
		{
			StringBuilder stringBuilder = new StringBuilder(base64Key, 64);
			stringBuilder.Append("258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
			SHA1 sha = new SHA1CryptoServiceProvider();
			byte[] inArray = sha.ComputeHash(stringBuilder.ToString().GetUTF8EncodedBytes());
			return Convert.ToBase64String(inArray);
		}

		internal void InternalAccept()
		{
			try
			{
				bool flag = !this.acceptHandshake();
				if (flag)
				{
					return;
				}
			}
			catch (Exception ex)
			{
				this._logger.Fatal(ex.Message);
				this._logger.Debug(ex.ToString());
				string message = "An exception has occurred while attempting to accept.";
				this.fatal(message, ex);
				return;
			}
			this._readyState = WebSocketState.Open;
			this.open();
		}

		internal bool Ping(byte[] frameAsBytes, TimeSpan timeout)
		{
			bool flag = this._readyState != WebSocketState.Open;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ManualResetEvent pongReceived = this._pongReceived;
				bool flag2 = pongReceived == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					object forPing = this._forPing;
					lock (forPing)
					{
						try
						{
							pongReceived.Reset();
							object forState = this._forState;
							lock (forState)
							{
								bool flag3 = this._readyState != WebSocketState.Open;
								if (flag3)
								{
									return false;
								}
								bool flag4 = !this.sendBytes(frameAsBytes);
								if (flag4)
								{
									return false;
								}
							}
							result = pongReceived.WaitOne(timeout);
						}
						catch (ObjectDisposedException)
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		internal void Send(Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache)
		{
			object forSend = this._forSend;
			lock (forSend)
			{
				object forState = this._forState;
				lock (forState)
				{
					bool flag = this._readyState != WebSocketState.Open;
					if (flag)
					{
						this._logger.Error("The connection is closing.");
					}
					else
					{
						byte[] array;
						bool flag2 = !cache.TryGetValue(this._compression, out array);
						if (flag2)
						{
							array = new WebSocketFrame(Fin.Final, opcode, data.Compress(this._compression), this._compression > CompressionMethod.None, false).ToArray();
							cache.Add(this._compression, array);
						}
						this.sendBytes(array);
					}
				}
			}
		}

		internal void Send(Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache)
		{
			object forSend = this._forSend;
			lock (forSend)
			{
				Stream stream2;
				bool flag = !cache.TryGetValue(this._compression, out stream2);
				if (flag)
				{
					stream2 = stream.Compress(this._compression);
					cache.Add(this._compression, stream2);
				}
				else
				{
					stream2.Position = 0L;
				}
				this.send(opcode, stream2, this._compression > CompressionMethod.None);
			}
		}

		public void Accept()
		{
			bool client = this._client;
			if (client)
			{
				string message = "This instance is a client.";
				throw new InvalidOperationException(message);
			}
			bool flag = this._readyState == WebSocketState.Closing;
			if (flag)
			{
				string message2 = "The close process is in progress.";
				throw new InvalidOperationException(message2);
			}
			bool flag2 = this._readyState == WebSocketState.Closed;
			if (flag2)
			{
				string message3 = "The connection has already been closed.";
				throw new InvalidOperationException(message3);
			}
			bool flag3 = this.accept();
			if (flag3)
			{
				this.open();
			}
		}

		public void AcceptAsync()
		{
			bool client = this._client;
			if (client)
			{
				string message = "This instance is a client.";
				throw new InvalidOperationException(message);
			}
			bool flag = this._readyState == WebSocketState.Closing;
			if (flag)
			{
				string message2 = "The close process is in progress.";
				throw new InvalidOperationException(message2);
			}
			bool flag2 = this._readyState == WebSocketState.Closed;
			if (flag2)
			{
				string message3 = "The connection has already been closed.";
				throw new InvalidOperationException(message3);
			}
			Func<bool> acceptor = new Func<bool>(this.accept);
			acceptor.BeginInvoke(delegate(IAsyncResult ar)
			{
				bool flag3 = acceptor.EndInvoke(ar);
				if (flag3)
				{
					this.open();
				}
			}, null);
		}

		public void Close()
		{
			this.close(1005, string.Empty);
		}

		public void Close(ushort code)
		{
			bool flag = !code.IsCloseStatusCode();
			if (flag)
			{
				string message = "Less than 1000 or greater than 4999.";
				throw new ArgumentOutOfRangeException("code", message);
			}
			bool flag2 = this._client && code == 1011;
			if (flag2)
			{
				string message2 = "1011 cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			bool flag3 = !this._client && code == 1010;
			if (flag3)
			{
				string message3 = "1010 cannot be used.";
				throw new ArgumentException(message3, "code");
			}
			this.close(code, string.Empty);
		}

		public void Close(CloseStatusCode code)
		{
			bool flag = this._client && code == CloseStatusCode.ServerError;
			if (flag)
			{
				string message = "ServerError cannot be used.";
				throw new ArgumentException(message, "code");
			}
			bool flag2 = !this._client && code == CloseStatusCode.MandatoryExtension;
			if (flag2)
			{
				string message2 = "MandatoryExtension cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			this.close((ushort)code, string.Empty);
		}

		public void Close(ushort code, string reason)
		{
			bool flag = !code.IsCloseStatusCode();
			if (flag)
			{
				string message = "Less than 1000 or greater than 4999.";
				throw new ArgumentOutOfRangeException("code", message);
			}
			bool flag2 = this._client && code == 1011;
			if (flag2)
			{
				string message2 = "1011 cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			bool flag3 = !this._client && code == 1010;
			if (flag3)
			{
				string message3 = "1010 cannot be used.";
				throw new ArgumentException(message3, "code");
			}
			bool flag4 = reason.IsNullOrEmpty();
			if (flag4)
			{
				this.close(code, string.Empty);
			}
			else
			{
				bool flag5 = code == 1005;
				if (flag5)
				{
					string message4 = "1005 cannot be used.";
					throw new ArgumentException(message4, "code");
				}
				byte[] array;
				bool flag6 = !reason.TryGetUTF8EncodedBytes(out array);
				if (flag6)
				{
					string message5 = "It could not be UTF-8-encoded.";
					throw new ArgumentException(message5, "reason");
				}
				bool flag7 = array.Length > 123;
				if (flag7)
				{
					string message6 = "Its size is greater than 123 bytes.";
					throw new ArgumentOutOfRangeException("reason", message6);
				}
				this.close(code, reason);
			}
		}

		public void Close(CloseStatusCode code, string reason)
		{
			bool flag = this._client && code == CloseStatusCode.ServerError;
			if (flag)
			{
				string message = "ServerError cannot be used.";
				throw new ArgumentException(message, "code");
			}
			bool flag2 = !this._client && code == CloseStatusCode.MandatoryExtension;
			if (flag2)
			{
				string message2 = "MandatoryExtension cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			bool flag3 = reason.IsNullOrEmpty();
			if (flag3)
			{
				this.close((ushort)code, string.Empty);
			}
			else
			{
				bool flag4 = code == CloseStatusCode.NoStatus;
				if (flag4)
				{
					string message3 = "NoStatus cannot be used.";
					throw new ArgumentException(message3, "code");
				}
				byte[] array;
				bool flag5 = !reason.TryGetUTF8EncodedBytes(out array);
				if (flag5)
				{
					string message4 = "It could not be UTF-8-encoded.";
					throw new ArgumentException(message4, "reason");
				}
				bool flag6 = array.Length > 123;
				if (flag6)
				{
					string message5 = "Its size is greater than 123 bytes.";
					throw new ArgumentOutOfRangeException("reason", message5);
				}
				this.close((ushort)code, reason);
			}
		}

		public void CloseAsync()
		{
			this.closeAsync(1005, string.Empty);
		}

		public void CloseAsync(ushort code)
		{
			bool flag = !code.IsCloseStatusCode();
			if (flag)
			{
				string message = "Less than 1000 or greater than 4999.";
				throw new ArgumentOutOfRangeException("code", message);
			}
			bool flag2 = this._client && code == 1011;
			if (flag2)
			{
				string message2 = "1011 cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			bool flag3 = !this._client && code == 1010;
			if (flag3)
			{
				string message3 = "1010 cannot be used.";
				throw new ArgumentException(message3, "code");
			}
			this.closeAsync(code, string.Empty);
		}

		public void CloseAsync(CloseStatusCode code)
		{
			bool flag = this._client && code == CloseStatusCode.ServerError;
			if (flag)
			{
				string message = "ServerError cannot be used.";
				throw new ArgumentException(message, "code");
			}
			bool flag2 = !this._client && code == CloseStatusCode.MandatoryExtension;
			if (flag2)
			{
				string message2 = "MandatoryExtension cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			this.closeAsync((ushort)code, string.Empty);
		}

		public void CloseAsync(ushort code, string reason)
		{
			bool flag = !code.IsCloseStatusCode();
			if (flag)
			{
				string message = "Less than 1000 or greater than 4999.";
				throw new ArgumentOutOfRangeException("code", message);
			}
			bool flag2 = this._client && code == 1011;
			if (flag2)
			{
				string message2 = "1011 cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			bool flag3 = !this._client && code == 1010;
			if (flag3)
			{
				string message3 = "1010 cannot be used.";
				throw new ArgumentException(message3, "code");
			}
			bool flag4 = reason.IsNullOrEmpty();
			if (flag4)
			{
				this.closeAsync(code, string.Empty);
			}
			else
			{
				bool flag5 = code == 1005;
				if (flag5)
				{
					string message4 = "1005 cannot be used.";
					throw new ArgumentException(message4, "code");
				}
				byte[] array;
				bool flag6 = !reason.TryGetUTF8EncodedBytes(out array);
				if (flag6)
				{
					string message5 = "It could not be UTF-8-encoded.";
					throw new ArgumentException(message5, "reason");
				}
				bool flag7 = array.Length > 123;
				if (flag7)
				{
					string message6 = "Its size is greater than 123 bytes.";
					throw new ArgumentOutOfRangeException("reason", message6);
				}
				this.closeAsync(code, reason);
			}
		}

		public void CloseAsync(CloseStatusCode code, string reason)
		{
			bool flag = this._client && code == CloseStatusCode.ServerError;
			if (flag)
			{
				string message = "ServerError cannot be used.";
				throw new ArgumentException(message, "code");
			}
			bool flag2 = !this._client && code == CloseStatusCode.MandatoryExtension;
			if (flag2)
			{
				string message2 = "MandatoryExtension cannot be used.";
				throw new ArgumentException(message2, "code");
			}
			bool flag3 = reason.IsNullOrEmpty();
			if (flag3)
			{
				this.closeAsync((ushort)code, string.Empty);
			}
			else
			{
				bool flag4 = code == CloseStatusCode.NoStatus;
				if (flag4)
				{
					string message3 = "NoStatus cannot be used.";
					throw new ArgumentException(message3, "code");
				}
				byte[] array;
				bool flag5 = !reason.TryGetUTF8EncodedBytes(out array);
				if (flag5)
				{
					string message4 = "It could not be UTF-8-encoded.";
					throw new ArgumentException(message4, "reason");
				}
				bool flag6 = array.Length > 123;
				if (flag6)
				{
					string message5 = "Its size is greater than 123 bytes.";
					throw new ArgumentOutOfRangeException("reason", message5);
				}
				this.closeAsync((ushort)code, reason);
			}
		}

		public void Connect()
		{
			bool flag = !this._client;
			if (flag)
			{
				string message = "This instance is not a client.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = this._readyState == WebSocketState.Closing;
			if (flag2)
			{
				string message2 = "The close process is in progress.";
				throw new InvalidOperationException(message2);
			}
			bool flag3 = this._retryCountForConnect > WebSocket._maxRetryCountForConnect;
			if (flag3)
			{
				string message3 = "A series of reconnecting has failed.";
				throw new InvalidOperationException(message3);
			}
			bool flag4 = this.connect();
			if (flag4)
			{
				this.open();
			}
		}

		public void ConnectAsync()
		{
			bool flag = !this._client;
			if (flag)
			{
				string message = "This instance is not a client.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = this._readyState == WebSocketState.Closing;
			if (flag2)
			{
				string message2 = "The close process is in progress.";
				throw new InvalidOperationException(message2);
			}
			bool flag3 = this._retryCountForConnect > WebSocket._maxRetryCountForConnect;
			if (flag3)
			{
				string message3 = "A series of reconnecting has failed.";
				throw new InvalidOperationException(message3);
			}
			Func<bool> connector = new Func<bool>(this.connect);
			connector.BeginInvoke(delegate(IAsyncResult ar)
			{
				bool flag4 = connector.EndInvoke(ar);
				if (flag4)
				{
					this.open();
				}
			}, null);
		}

		public bool Ping()
		{
			return this.ping(WebSocket.EmptyBytes);
		}

		public bool Ping(string message)
		{
			bool flag = message.IsNullOrEmpty();
			bool result;
			if (flag)
			{
				result = this.ping(WebSocket.EmptyBytes);
			}
			else
			{
				byte[] array;
				bool flag2 = !message.TryGetUTF8EncodedBytes(out array);
				if (flag2)
				{
					string message2 = "It could not be UTF-8-encoded.";
					throw new ArgumentException(message2, "message");
				}
				bool flag3 = array.Length > 125;
				if (flag3)
				{
					string message3 = "Its size is greater than 125 bytes.";
					throw new ArgumentOutOfRangeException("message", message3);
				}
				result = this.ping(array);
			}
			return result;
		}

		public void Send(byte[] data)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			this.send(Opcode.Binary, new MemoryStream(data));
		}

		public void Send(FileInfo fileInfo)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = fileInfo == null;
			if (flag2)
			{
				throw new ArgumentNullException("fileInfo");
			}
			bool flag3 = !fileInfo.Exists;
			if (flag3)
			{
				string message2 = "The file does not exist.";
				throw new ArgumentException(message2, "fileInfo");
			}
			FileStream stream;
			bool flag4 = !fileInfo.TryOpenRead(out stream);
			if (flag4)
			{
				string message3 = "The file could not be opened.";
				throw new ArgumentException(message3, "fileInfo");
			}
			this.send(Opcode.Binary, stream);
		}

		public void Send(string data)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			byte[] buffer;
			bool flag3 = !data.TryGetUTF8EncodedBytes(out buffer);
			if (flag3)
			{
				string message2 = "It could not be UTF-8-encoded.";
				throw new ArgumentException(message2, "data");
			}
			this.send(Opcode.Text, new MemoryStream(buffer));
		}

		public void Send(Stream stream, int length)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag3 = !stream.CanRead;
			if (flag3)
			{
				string message2 = "It cannot be read.";
				throw new ArgumentException(message2, "stream");
			}
			bool flag4 = length < 1;
			if (flag4)
			{
				string message3 = "Less than 1.";
				throw new ArgumentException(message3, "length");
			}
			byte[] array = stream.ReadBytes(length);
			int num = array.Length;
			bool flag5 = num == 0;
			if (flag5)
			{
				string message4 = "No data could be read from it.";
				throw new ArgumentException(message4, "stream");
			}
			bool flag6 = num < length;
			if (flag6)
			{
				this._logger.Warn(string.Format("Only {0} byte(s) of data could be read from the stream.", num));
			}
			this.send(Opcode.Binary, new MemoryStream(array));
		}

		public void SendAsync(byte[] data, Action<bool> completed)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			this.sendAsync(Opcode.Binary, new MemoryStream(data), completed);
		}

		public void SendAsync(FileInfo fileInfo, Action<bool> completed)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = fileInfo == null;
			if (flag2)
			{
				throw new ArgumentNullException("fileInfo");
			}
			bool flag3 = !fileInfo.Exists;
			if (flag3)
			{
				string message2 = "The file does not exist.";
				throw new ArgumentException(message2, "fileInfo");
			}
			FileStream stream;
			bool flag4 = !fileInfo.TryOpenRead(out stream);
			if (flag4)
			{
				string message3 = "The file could not be opened.";
				throw new ArgumentException(message3, "fileInfo");
			}
			this.sendAsync(Opcode.Binary, stream, completed);
		}

		public void SendAsync(string data, Action<bool> completed)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = data == null;
			if (flag2)
			{
				throw new ArgumentNullException("data");
			}
			byte[] buffer;
			bool flag3 = !data.TryGetUTF8EncodedBytes(out buffer);
			if (flag3)
			{
				string message2 = "It could not be UTF-8-encoded.";
				throw new ArgumentException(message2, "data");
			}
			this.sendAsync(Opcode.Text, new MemoryStream(buffer), completed);
		}

		public void SendAsync(Stream stream, int length, Action<bool> completed)
		{
			bool flag = this._readyState != WebSocketState.Open;
			if (flag)
			{
				string message = "The current state of the connection is not Open.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = stream == null;
			if (flag2)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag3 = !stream.CanRead;
			if (flag3)
			{
				string message2 = "It cannot be read.";
				throw new ArgumentException(message2, "stream");
			}
			bool flag4 = length < 1;
			if (flag4)
			{
				string message3 = "Less than 1.";
				throw new ArgumentException(message3, "length");
			}
			byte[] array = stream.ReadBytes(length);
			int num = array.Length;
			bool flag5 = num == 0;
			if (flag5)
			{
				string message4 = "No data could be read from it.";
				throw new ArgumentException(message4, "stream");
			}
			bool flag6 = num < length;
			if (flag6)
			{
				this._logger.Warn(string.Format("Only {0} byte(s) of data could be read from the stream.", num));
			}
			this.sendAsync(Opcode.Binary, new MemoryStream(array), completed);
		}

		public void SetCookie(Cookie cookie)
		{
			string message = null;
			bool flag = !this._client;
			if (flag)
			{
				message = "This instance is not a client.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = cookie == null;
			if (flag2)
			{
				throw new ArgumentNullException("cookie");
			}
			bool flag3 = !this.canSet(out message);
			if (flag3)
			{
				this._logger.Warn(message);
			}
			else
			{
				object forState = this._forState;
				lock (forState)
				{
					bool flag4 = !this.canSet(out message);
					if (flag4)
					{
						this._logger.Warn(message);
					}
					else
					{
						object syncRoot = this._cookies.SyncRoot;
						lock (syncRoot)
						{
							this._cookies.SetOrRemove(cookie);
						}
					}
				}
			}
		}

		public void SetCredentials(string username, string password, bool preAuth)
		{
			string message = null;
			bool flag = !this._client;
			if (flag)
			{
				message = "This instance is not a client.";
				throw new InvalidOperationException(message);
			}
			bool flag2 = !username.IsNullOrEmpty();
			if (flag2)
			{
				bool flag3 = username.Contains(new char[]
				{
					':'
				}) || !username.IsText();
				if (flag3)
				{
					message = "It contains an invalid character.";
					throw new ArgumentException(message, "username");
				}
			}
			bool flag4 = !password.IsNullOrEmpty();
			if (flag4)
			{
				bool flag5 = !password.IsText();
				if (flag5)
				{
					message = "It contains an invalid character.";
					throw new ArgumentException(message, "password");
				}
			}
			bool flag6 = !this.canSet(out message);
			if (flag6)
			{
				this._logger.Warn(message);
			}
			else
			{
				object forState = this._forState;
				lock (forState)
				{
					bool flag7 = !this.canSet(out message);
					if (flag7)
					{
						this._logger.Warn(message);
					}
					else
					{
						bool flag8 = username.IsNullOrEmpty();
						if (flag8)
						{
							this._credentials = null;
							this._preAuth = false;
						}
						else
						{
							this._credentials = new NetworkCredential(username, password, this._uri.PathAndQuery, new string[0]);
							this._preAuth = preAuth;
						}
					}
				}
			}
		}

		public void SetProxy(string url, string username, string password)
		{
			string message = null;
			bool flag = !this._client;
			if (flag)
			{
				message = "This instance is not a client.";
				throw new InvalidOperationException(message);
			}
			Uri uri = null;
			bool flag2 = !url.IsNullOrEmpty();
			if (flag2)
			{
				bool flag3 = !Uri.TryCreate(url, UriKind.Absolute, out uri);
				if (flag3)
				{
					message = "Not an absolute URI string.";
					throw new ArgumentException(message, "url");
				}
				bool flag4 = uri.Scheme != "http";
				if (flag4)
				{
					message = "The scheme part is not http.";
					throw new ArgumentException(message, "url");
				}
				bool flag5 = uri.Segments.Length > 1;
				if (flag5)
				{
					message = "It includes the path segments.";
					throw new ArgumentException(message, "url");
				}
			}
			bool flag6 = !username.IsNullOrEmpty();
			if (flag6)
			{
				bool flag7 = username.Contains(new char[]
				{
					':'
				}) || !username.IsText();
				if (flag7)
				{
					message = "It contains an invalid character.";
					throw new ArgumentException(message, "username");
				}
			}
			bool flag8 = !password.IsNullOrEmpty();
			if (flag8)
			{
				bool flag9 = !password.IsText();
				if (flag9)
				{
					message = "It contains an invalid character.";
					throw new ArgumentException(message, "password");
				}
			}
			bool flag10 = !this.canSet(out message);
			if (flag10)
			{
				this._logger.Warn(message);
			}
			else
			{
				object forState = this._forState;
				lock (forState)
				{
					bool flag11 = !this.canSet(out message);
					if (flag11)
					{
						this._logger.Warn(message);
					}
					else
					{
						bool flag12 = url.IsNullOrEmpty();
						if (flag12)
						{
							this._proxyUri = null;
							this._proxyCredentials = null;
						}
						else
						{
							this._proxyUri = uri;
							this._proxyCredentials = ((!username.IsNullOrEmpty()) ? new NetworkCredential(username, password, string.Format("{0}:{1}", this._uri.DnsSafeHost, this._uri.Port), new string[0]) : null);
						}
					}
				}
			}
		}

		void IDisposable.Dispose()
		{
			this.close(1001, string.Empty);
		}

		private AuthenticationChallenge _authChallenge;

		private string _base64Key;

		private bool _client;

		private Action _closeContext;

		private CompressionMethod _compression;

		private WebSocketContext _context;

		private CookieCollection _cookies;

		private NetworkCredential _credentials;

		private bool _emitOnPing;

		private bool _enableRedirection;

		private string _extensions;

		private bool _extensionsRequested;

		private object _forMessageEventQueue;

		private object _forPing;

		private object _forSend;

		private object _forState;

		private MemoryStream _fragmentsBuffer;

		private bool _fragmentsCompressed;

		private Opcode _fragmentsOpcode;

		private const string _guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private Func<WebSocketContext, string> _handshakeRequestChecker;

		private bool _ignoreExtensions;

		private bool _inContinuation;

		private volatile bool _inMessage;

		private volatile Logger _logger;

		private static readonly int _maxRetryCountForConnect = 10;

		private Action<MessageEventArgs> _message;

		private Queue<MessageEventArgs> _messageEventQueue;

		private uint _nonceCount;

		private string _origin;

		private ManualResetEvent _pongReceived;

		private bool _preAuth;

		private string _protocol;

		private string[] _protocols;

		private bool _protocolsRequested;

		private NetworkCredential _proxyCredentials;

		private Uri _proxyUri;

		private volatile WebSocketState _readyState;

		private ManualResetEvent _receivingExited;

		private int _retryCountForConnect;

		private bool _secure;

		private ClientSslConfiguration _sslConfig;

		private Stream _stream;

		private TcpClient _tcpClient;

		private Uri _uri;

		private const string _version = "13";

		private TimeSpan _waitTime;

		internal static readonly byte[] EmptyBytes = new byte[0];

		internal static readonly int FragmentLength = 1016;

		internal static readonly RandomNumberGenerator RandomNumber = new RNGCryptoServiceProvider();
	}
}
