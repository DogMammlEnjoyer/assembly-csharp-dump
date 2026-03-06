using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public class HttpServer
	{
		public HttpServer()
		{
			this.init("*", IPAddress.Any, 80, false);
		}

		public HttpServer(int port) : this(port, port == 443)
		{
		}

		public HttpServer(string url)
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
			Uri uri;
			string message;
			bool flag3 = !HttpServer.tryCreateUri(url, out uri, out message);
			if (flag3)
			{
				throw new ArgumentException(message, "url");
			}
			string dnsSafeHost = uri.GetDnsSafeHost(true);
			IPAddress ipaddress = dnsSafeHost.ToIPAddress();
			bool flag4 = ipaddress == null;
			if (flag4)
			{
				message = "The host part could not be converted to an IP address.";
				throw new ArgumentException(message, "url");
			}
			bool flag5 = !ipaddress.IsLocal();
			if (flag5)
			{
				message = "The IP address of the host is not a local IP address.";
				throw new ArgumentException(message, "url");
			}
			this.init(dnsSafeHost, ipaddress, uri.Port, uri.Scheme == "https");
		}

		public HttpServer(int port, bool secure)
		{
			bool flag = !port.IsPortNumber();
			if (flag)
			{
				string message = "It is less than 1 or greater than 65535.";
				throw new ArgumentOutOfRangeException("port", message);
			}
			this.init("*", IPAddress.Any, port, secure);
		}

		public HttpServer(IPAddress address, int port) : this(address, port, port == 443)
		{
		}

		public HttpServer(IPAddress address, int port, bool secure)
		{
			bool flag = address == null;
			if (flag)
			{
				throw new ArgumentNullException("address");
			}
			bool flag2 = !address.IsLocal();
			if (flag2)
			{
				string message = "It is not a local IP address.";
				throw new ArgumentException(message, "address");
			}
			bool flag3 = !port.IsPortNumber();
			if (flag3)
			{
				string message2 = "It is less than 1 or greater than 65535.";
				throw new ArgumentOutOfRangeException("port", message2);
			}
			this.init(address.ToString(true), address, port, secure);
		}

		public IPAddress Address
		{
			get
			{
				return this._address;
			}
		}

		public WebSocketSharp.Net.AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return this._listener.AuthenticationSchemes;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._listener.AuthenticationSchemes = value;
					}
				}
			}
		}

		public string DocumentRootPath
		{
			get
			{
				return this._docRootPath;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				bool flag2 = value.Length == 0;
				if (flag2)
				{
					throw new ArgumentException("An empty string.", "value");
				}
				value = value.TrimSlashOrBackslashFromEnd();
				bool flag3 = value == "/";
				if (flag3)
				{
					throw new ArgumentException("An absolute root.", "value");
				}
				bool flag4 = value == "\\";
				if (flag4)
				{
					throw new ArgumentException("An absolute root.", "value");
				}
				bool flag5 = value.Length == 2 && value[1] == ':';
				if (flag5)
				{
					throw new ArgumentException("An absolute root.", "value");
				}
				string text = null;
				try
				{
					text = Path.GetFullPath(value);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException("An invalid path string.", "value", innerException);
				}
				bool flag6 = text == "/";
				if (flag6)
				{
					throw new ArgumentException("An absolute root.", "value");
				}
				text = text.TrimSlashOrBackslashFromEnd();
				bool flag7 = text.Length == 2 && text[1] == ':';
				if (flag7)
				{
					throw new ArgumentException("An absolute root.", "value");
				}
				object sync = this._sync;
				lock (sync)
				{
					bool flag8 = !this.canSet();
					if (!flag8)
					{
						this._docRootPath = value;
					}
				}
			}
		}

		public bool IsListening
		{
			get
			{
				return this._state == ServerState.Start;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public bool KeepClean
		{
			get
			{
				return this._services.KeepClean;
			}
			set
			{
				this._services.KeepClean = value;
			}
		}

		public Logger Log
		{
			get
			{
				return this._log;
			}
		}

		public int Port
		{
			get
			{
				return this._port;
			}
		}

		public string Realm
		{
			get
			{
				return this._listener.Realm;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._listener.Realm = value;
					}
				}
			}
		}

		public bool ReuseAddress
		{
			get
			{
				return this._listener.ReuseAddress;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._listener.ReuseAddress = value;
					}
				}
			}
		}

		public ServerSslConfiguration SslConfiguration
		{
			get
			{
				bool flag = !this._secure;
				if (flag)
				{
					string message = "The server does not provide secure connections.";
					throw new InvalidOperationException(message);
				}
				return this._listener.SslConfiguration;
			}
		}

		public Func<IIdentity, WebSocketSharp.Net.NetworkCredential> UserCredentialsFinder
		{
			get
			{
				return this._listener.UserCredentialsFinder;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._listener.UserCredentialsFinder = value;
					}
				}
			}
		}

		public TimeSpan WaitTime
		{
			get
			{
				return this._services.WaitTime;
			}
			set
			{
				this._services.WaitTime = value;
			}
		}

		public WebSocketServiceManager WebSocketServices
		{
			get
			{
				return this._services;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnConnect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnDelete;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnGet;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnHead;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnOptions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnPost;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnPut;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<HttpRequestEventArgs> OnTrace;

		private void abort()
		{
			object sync = this._sync;
			lock (sync)
			{
				bool flag = this._state != ServerState.Start;
				if (flag)
				{
					return;
				}
				this._state = ServerState.ShuttingDown;
			}
			try
			{
				this._services.Stop(1006, string.Empty);
			}
			catch (Exception ex)
			{
				this._log.Fatal(ex.Message);
				this._log.Debug(ex.ToString());
			}
			try
			{
				this._listener.Abort();
			}
			catch (Exception ex2)
			{
				this._log.Fatal(ex2.Message);
				this._log.Debug(ex2.ToString());
			}
			this._state = ServerState.Stop;
		}

		private bool canSet()
		{
			return this._state == ServerState.Ready || this._state == ServerState.Stop;
		}

		private bool checkCertificate(out string message)
		{
			message = null;
			bool flag = this._listener.SslConfiguration.ServerCertificate != null;
			string certificateFolderPath = this._listener.CertificateFolderPath;
			bool flag2 = EndPointListener.CertificateExists(this._port, certificateFolderPath);
			bool flag3 = flag || flag2;
			bool flag4 = !flag3;
			bool result;
			if (flag4)
			{
				message = "There is no server certificate for secure connection.";
				result = false;
			}
			else
			{
				bool flag5 = flag && flag2;
				bool flag6 = flag5;
				if (flag6)
				{
					string message2 = "The server certificate associated with the port is used.";
					this._log.Warn(message2);
				}
				result = true;
			}
			return result;
		}

		private static WebSocketSharp.Net.HttpListener createListener(string hostname, int port, bool secure)
		{
			WebSocketSharp.Net.HttpListener httpListener = new WebSocketSharp.Net.HttpListener();
			string arg = secure ? "https" : "http";
			string uriPrefix = string.Format("{0}://{1}:{2}/", arg, hostname, port);
			httpListener.Prefixes.Add(uriPrefix);
			return httpListener;
		}

		private void init(string hostname, IPAddress address, int port, bool secure)
		{
			this._hostname = hostname;
			this._address = address;
			this._port = port;
			this._secure = secure;
			this._docRootPath = "./Public";
			this._listener = HttpServer.createListener(this._hostname, this._port, this._secure);
			this._log = this._listener.Log;
			this._services = new WebSocketServiceManager(this._log);
			this._sync = new object();
		}

		private void processRequest(WebSocketSharp.Net.HttpListenerContext context)
		{
			string httpMethod = context.Request.HttpMethod;
			EventHandler<HttpRequestEventArgs> eventHandler = (httpMethod == "GET") ? this.OnGet : ((httpMethod == "HEAD") ? this.OnHead : ((httpMethod == "POST") ? this.OnPost : ((httpMethod == "PUT") ? this.OnPut : ((httpMethod == "DELETE") ? this.OnDelete : ((httpMethod == "CONNECT") ? this.OnConnect : ((httpMethod == "OPTIONS") ? this.OnOptions : ((httpMethod == "TRACE") ? this.OnTrace : null)))))));
			bool flag = eventHandler == null;
			if (flag)
			{
				context.ErrorStatusCode = 501;
				context.SendError();
			}
			else
			{
				HttpRequestEventArgs e = new HttpRequestEventArgs(context, this._docRootPath);
				eventHandler(this, e);
				context.Response.Close();
			}
		}

		private void processRequest(HttpListenerWebSocketContext context)
		{
			Uri requestUri = context.RequestUri;
			bool flag = requestUri == null;
			if (flag)
			{
				context.Close(WebSocketSharp.Net.HttpStatusCode.BadRequest);
			}
			else
			{
				string text = requestUri.AbsolutePath;
				bool flag2 = text.IndexOfAny(new char[]
				{
					'%',
					'+'
				}) > -1;
				if (flag2)
				{
					text = HttpUtility.UrlDecode(text, Encoding.UTF8);
				}
				WebSocketServiceHost webSocketServiceHost;
				bool flag3 = !this._services.InternalTryGetServiceHost(text, out webSocketServiceHost);
				if (flag3)
				{
					context.Close(WebSocketSharp.Net.HttpStatusCode.NotImplemented);
				}
				else
				{
					webSocketServiceHost.StartSession(context);
				}
			}
		}

		private void receiveRequest()
		{
			for (;;)
			{
				WebSocketSharp.Net.HttpListenerContext ctx = null;
				try
				{
					ctx = this._listener.GetContext();
					ThreadPool.QueueUserWorkItem(delegate(object state)
					{
						try
						{
							bool flag5 = ctx.Request.IsUpgradeRequest("websocket");
							if (flag5)
							{
								this.processRequest(ctx.GetWebSocketContext(null));
							}
							else
							{
								this.processRequest(ctx);
							}
						}
						catch (Exception ex4)
						{
							this._log.Error(ex4.Message);
							this._log.Debug(ex4.ToString());
							ctx.Connection.Close(true);
						}
					});
				}
				catch (WebSocketSharp.Net.HttpListenerException ex)
				{
					bool flag = this._state == ServerState.ShuttingDown;
					if (flag)
					{
						this._log.Info("The underlying listener is stopped.");
						return;
					}
					this._log.Fatal(ex.Message);
					this._log.Debug(ex.ToString());
					break;
				}
				catch (InvalidOperationException ex2)
				{
					bool flag2 = this._state == ServerState.ShuttingDown;
					if (flag2)
					{
						this._log.Info("The underlying listener is stopped.");
						return;
					}
					this._log.Fatal(ex2.Message);
					this._log.Debug(ex2.ToString());
					break;
				}
				catch (Exception ex3)
				{
					this._log.Fatal(ex3.Message);
					this._log.Debug(ex3.ToString());
					bool flag3 = ctx != null;
					if (flag3)
					{
						ctx.Connection.Close(true);
					}
					bool flag4 = this._state == ServerState.ShuttingDown;
					if (flag4)
					{
						return;
					}
					break;
				}
			}
			this.abort();
		}

		private void start()
		{
			object sync = this._sync;
			lock (sync)
			{
				bool flag = this._state == ServerState.Start || this._state == ServerState.ShuttingDown;
				if (!flag)
				{
					bool secure = this._secure;
					if (secure)
					{
						string message;
						bool flag2 = !this.checkCertificate(out message);
						if (flag2)
						{
							throw new InvalidOperationException(message);
						}
					}
					this._services.Start();
					try
					{
						this.startReceiving();
					}
					catch
					{
						this._services.Stop(1011, string.Empty);
						throw;
					}
					this._state = ServerState.Start;
				}
			}
		}

		private void startReceiving()
		{
			try
			{
				this._listener.Start();
			}
			catch (Exception innerException)
			{
				string message = "The underlying listener has failed to start.";
				throw new InvalidOperationException(message, innerException);
			}
			ThreadStart start = new ThreadStart(this.receiveRequest);
			this._receiveThread = new Thread(start);
			this._receiveThread.IsBackground = true;
			this._receiveThread.Start();
		}

		private void stop(ushort code, string reason)
		{
			object sync = this._sync;
			lock (sync)
			{
				bool flag = this._state != ServerState.Start;
				if (flag)
				{
					return;
				}
				this._state = ServerState.ShuttingDown;
			}
			try
			{
				this._services.Stop(code, reason);
			}
			catch (Exception ex)
			{
				this._log.Fatal(ex.Message);
				this._log.Debug(ex.ToString());
			}
			try
			{
				this.stopReceiving(5000);
			}
			catch (Exception ex2)
			{
				this._log.Fatal(ex2.Message);
				this._log.Debug(ex2.ToString());
			}
			this._state = ServerState.Stop;
		}

		private void stopReceiving(int millisecondsTimeout)
		{
			this._listener.Stop();
			this._receiveThread.Join(millisecondsTimeout);
		}

		private static bool tryCreateUri(string uriString, out Uri result, out string message)
		{
			result = null;
			message = null;
			Uri uri = uriString.ToUri();
			bool flag = uri == null;
			bool result2;
			if (flag)
			{
				message = "An invalid URI string.";
				result2 = false;
			}
			else
			{
				bool flag2 = !uri.IsAbsoluteUri;
				if (flag2)
				{
					message = "A relative URI.";
					result2 = false;
				}
				else
				{
					string scheme = uri.Scheme;
					bool flag3 = scheme == "http" || scheme == "https";
					bool flag4 = !flag3;
					if (flag4)
					{
						message = "The scheme part is not 'http' or 'https'.";
						result2 = false;
					}
					else
					{
						bool flag5 = uri.PathAndQuery != "/";
						if (flag5)
						{
							message = "It includes either or both path and query components.";
							result2 = false;
						}
						else
						{
							bool flag6 = uri.Fragment.Length > 0;
							if (flag6)
							{
								message = "It includes the fragment component.";
								result2 = false;
							}
							else
							{
								bool flag7 = uri.Port == 0;
								if (flag7)
								{
									message = "The port part is zero.";
									result2 = false;
								}
								else
								{
									result = uri;
									result2 = true;
								}
							}
						}
					}
				}
			}
			return result2;
		}

		public void AddWebSocketService<TBehavior>(string path) where TBehavior : WebSocketBehavior, new()
		{
			this._services.AddService<TBehavior>(path, null);
		}

		public void AddWebSocketService<TBehavior>(string path, Action<TBehavior> initializer) where TBehavior : WebSocketBehavior, new()
		{
			this._services.AddService<TBehavior>(path, initializer);
		}

		public bool RemoveWebSocketService(string path)
		{
			return this._services.RemoveService(path);
		}

		public void Start()
		{
			bool flag = this._state == ServerState.Start || this._state == ServerState.ShuttingDown;
			if (!flag)
			{
				this.start();
			}
		}

		public void Stop()
		{
			bool flag = this._state != ServerState.Start;
			if (!flag)
			{
				this.stop(1001, string.Empty);
			}
		}

		private IPAddress _address;

		private string _docRootPath;

		private string _hostname;

		private WebSocketSharp.Net.HttpListener _listener;

		private Logger _log;

		private int _port;

		private Thread _receiveThread;

		private bool _secure;

		private WebSocketServiceManager _services;

		private volatile ServerState _state;

		private object _sync;
	}
}
