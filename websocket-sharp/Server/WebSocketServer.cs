using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public class WebSocketServer
	{
		public WebSocketServer()
		{
			IPAddress any = IPAddress.Any;
			this.init(any.ToString(), any, 80, false);
		}

		public WebSocketServer(int port) : this(port, port == 443)
		{
		}

		public WebSocketServer(string url)
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
			bool flag3 = !WebSocketServer.tryCreateUri(url, out uri, out message);
			if (flag3)
			{
				throw new ArgumentException(message, "url");
			}
			string dnsSafeHost = uri.DnsSafeHost;
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
			this.init(dnsSafeHost, ipaddress, uri.Port, uri.Scheme == "wss");
		}

		public WebSocketServer(int port, bool secure)
		{
			bool flag = !port.IsPortNumber();
			if (flag)
			{
				string message = "It is less than 1 or greater than 65535.";
				throw new ArgumentOutOfRangeException("port", message);
			}
			IPAddress any = IPAddress.Any;
			this.init(any.ToString(), any, port, secure);
		}

		public WebSocketServer(IPAddress address, int port) : this(address, port, port == 443)
		{
		}

		public WebSocketServer(IPAddress address, int port, bool secure)
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
			this.init(address.ToString(), address, port, secure);
		}

		public IPAddress Address
		{
			get
			{
				return this._address;
			}
		}

		public bool AllowForwardedRequest
		{
			get
			{
				return this._allowForwardedRequest;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._allowForwardedRequest = value;
					}
				}
			}
		}

		public WebSocketSharp.Net.AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return this._authSchemes;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._authSchemes = value;
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
				return this._realm;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._realm = value;
					}
				}
			}
		}

		public bool ReuseAddress
		{
			get
			{
				return this._reuseAddress;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._reuseAddress = value;
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
				return this.getSslConfiguration();
			}
		}

		public Func<IIdentity, WebSocketSharp.Net.NetworkCredential> UserCredentialsFinder
		{
			get
			{
				return this._userCredFinder;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					bool flag = !this.canSet();
					if (!flag)
					{
						this._userCredFinder = value;
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
				this._listener.Stop();
			}
			catch (Exception ex)
			{
				this._log.Fatal(ex.Message);
				this._log.Debug(ex.ToString());
			}
			try
			{
				this._services.Stop(1006, string.Empty);
			}
			catch (Exception ex2)
			{
				this._log.Fatal(ex2.Message);
				this._log.Debug(ex2.ToString());
			}
			this._state = ServerState.Stop;
		}

		private bool authenticateClient(TcpListenerWebSocketContext context)
		{
			bool flag = this._authSchemes == WebSocketSharp.Net.AuthenticationSchemes.Anonymous;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = this._authSchemes == WebSocketSharp.Net.AuthenticationSchemes.None;
				result = (!flag2 && context.Authenticate(this._authSchemes, this._realmInUse, this._userCredFinder));
			}
			return result;
		}

		private bool canSet()
		{
			return this._state == ServerState.Ready || this._state == ServerState.Stop;
		}

		private bool checkHostNameForRequest(string name)
		{
			return !this._dnsStyle || Uri.CheckHostName(name) != UriHostNameType.Dns || name == this._hostname;
		}

		private string getRealm()
		{
			string realm = this._realm;
			return (realm != null && realm.Length > 0) ? realm : WebSocketServer._defaultRealm;
		}

		private ServerSslConfiguration getSslConfiguration()
		{
			bool flag = this._sslConfig == null;
			if (flag)
			{
				this._sslConfig = new ServerSslConfiguration();
			}
			return this._sslConfig;
		}

		private void init(string hostname, IPAddress address, int port, bool secure)
		{
			this._hostname = hostname;
			this._address = address;
			this._port = port;
			this._secure = secure;
			this._authSchemes = WebSocketSharp.Net.AuthenticationSchemes.Anonymous;
			this._dnsStyle = (Uri.CheckHostName(hostname) == UriHostNameType.Dns);
			this._listener = new TcpListener(address, port);
			this._log = new Logger();
			this._services = new WebSocketServiceManager(this._log);
			this._sync = new object();
		}

		private void processRequest(TcpListenerWebSocketContext context)
		{
			bool flag = !this.authenticateClient(context);
			if (flag)
			{
				context.Close(WebSocketSharp.Net.HttpStatusCode.Forbidden);
			}
			else
			{
				Uri requestUri = context.RequestUri;
				bool flag2 = requestUri == null;
				if (flag2)
				{
					context.Close(WebSocketSharp.Net.HttpStatusCode.BadRequest);
				}
				else
				{
					bool flag3 = !this._allowForwardedRequest;
					if (flag3)
					{
						bool flag4 = requestUri.Port != this._port;
						if (flag4)
						{
							context.Close(WebSocketSharp.Net.HttpStatusCode.BadRequest);
							return;
						}
						bool flag5 = !this.checkHostNameForRequest(requestUri.DnsSafeHost);
						if (flag5)
						{
							context.Close(WebSocketSharp.Net.HttpStatusCode.NotFound);
							return;
						}
					}
					string text = requestUri.AbsolutePath;
					bool flag6 = text.IndexOfAny(new char[]
					{
						'%',
						'+'
					}) > -1;
					if (flag6)
					{
						text = HttpUtility.UrlDecode(text, Encoding.UTF8);
					}
					WebSocketServiceHost webSocketServiceHost;
					bool flag7 = !this._services.InternalTryGetServiceHost(text, out webSocketServiceHost);
					if (flag7)
					{
						context.Close(WebSocketSharp.Net.HttpStatusCode.NotImplemented);
					}
					else
					{
						webSocketServiceHost.StartSession(context);
					}
				}
			}
		}

		private void receiveRequest()
		{
			for (;;)
			{
				TcpClient cl = null;
				try
				{
					cl = this._listener.AcceptTcpClient();
					ThreadPool.QueueUserWorkItem(delegate(object state)
					{
						try
						{
							TcpListenerWebSocketContext context = new TcpListenerWebSocketContext(cl, null, this._secure, this._sslConfigInUse, this._log);
							this.processRequest(context);
						}
						catch (Exception ex4)
						{
							this._log.Error(ex4.Message);
							this._log.Debug(ex4.ToString());
							cl.Close();
						}
					});
				}
				catch (SocketException ex)
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
					bool flag3 = cl != null;
					if (flag3)
					{
						cl.Close();
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
						ServerSslConfiguration sslConfiguration = this.getSslConfiguration();
						ServerSslConfiguration serverSslConfiguration = new ServerSslConfiguration(sslConfiguration);
						bool flag2 = serverSslConfiguration.ServerCertificate == null;
						if (flag2)
						{
							string message = "There is no server certificate for secure connection.";
							throw new InvalidOperationException(message);
						}
						this._sslConfigInUse = serverSslConfiguration;
					}
					this._realmInUse = this.getRealm();
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
			bool reuseAddress = this._reuseAddress;
			if (reuseAddress)
			{
				this._listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			}
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
				this.stopReceiving(5000);
			}
			catch (Exception ex)
			{
				this._log.Fatal(ex.Message);
				this._log.Debug(ex.ToString());
			}
			try
			{
				this._services.Stop(code, reason);
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
			bool flag = !uriString.TryCreateWebSocketUri(out result, out message);
			bool result2;
			if (flag)
			{
				result2 = false;
			}
			else
			{
				bool flag2 = result.PathAndQuery != "/";
				if (flag2)
				{
					result = null;
					message = "It includes either or both path and query components.";
					result2 = false;
				}
				else
				{
					result2 = true;
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

		private bool _allowForwardedRequest;

		private WebSocketSharp.Net.AuthenticationSchemes _authSchemes;

		private static readonly string _defaultRealm = "SECRET AREA";

		private bool _dnsStyle;

		private string _hostname;

		private TcpListener _listener;

		private Logger _log;

		private int _port;

		private string _realm;

		private string _realmInUse;

		private Thread _receiveThread;

		private bool _reuseAddress;

		private bool _secure;

		private WebSocketServiceManager _services;

		private ServerSslConfiguration _sslConfig;

		private ServerSslConfiguration _sslConfigInUse;

		private volatile ServerState _state;

		private object _sync;

		private Func<IIdentity, WebSocketSharp.Net.NetworkCredential> _userCredFinder;
	}
}
