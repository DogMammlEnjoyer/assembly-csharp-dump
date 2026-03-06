using System;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
	/// <summary>Provides client connections for TCP network services.</summary>
	public class TcpClient : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.TcpClient" /> class and binds it to the specified local endpoint.</summary>
		/// <param name="localEP">The <see cref="T:System.Net.IPEndPoint" /> to which you bind the TCP <see cref="T:System.Net.Sockets.Socket" />.</param>
		/// <exception cref="T:System.ArgumentNullException">The  <paramref name="localEP" /> parameter is <see langword="null" />.</exception>
		public TcpClient(IPEndPoint localEP)
		{
			bool on = Logging.On;
			if (localEP == null)
			{
				throw new ArgumentNullException("localEP");
			}
			this.m_Family = localEP.AddressFamily;
			this.initialize();
			this.Client.Bind(localEP);
			bool on2 = Logging.On;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.TcpClient" /> class.</summary>
		public TcpClient() : this(AddressFamily.InterNetwork)
		{
			bool on = Logging.On;
			bool on2 = Logging.On;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.TcpClient" /> class with the specified family.</summary>
		/// <param name="family">The <see cref="P:System.Net.IPAddress.AddressFamily" /> of the IP protocol.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="family" /> parameter is not equal to AddressFamily.InterNetwork  
		///  -or-  
		///  The <paramref name="family" /> parameter is not equal to AddressFamily.InterNetworkV6</exception>
		public TcpClient(AddressFamily family)
		{
			bool on = Logging.On;
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException(SR.GetString("'{0}' Client can only accept InterNetwork or InterNetworkV6 addresses.", new object[]
				{
					"TCP"
				}), "family");
			}
			this.m_Family = family;
			this.initialize();
			bool on2 = Logging.On;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.TcpClient" /> class and connects to the specified port on the specified host.</summary>
		/// <param name="hostname">The DNS name of the remote host to which you intend to connect.</param>
		/// <param name="port">The port number of the remote host to which you intend to connect.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="hostname" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="port" /> parameter is not between <see cref="F:System.Net.IPEndPoint.MinPort" /> and <see cref="F:System.Net.IPEndPoint.MaxPort" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
		public TcpClient(string hostname, int port)
		{
			bool on = Logging.On;
			if (hostname == null)
			{
				throw new ArgumentNullException("hostname");
			}
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			try
			{
				this.Connect(hostname, port);
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
				{
					throw;
				}
				if (this.m_ClientSocket != null)
				{
					this.m_ClientSocket.Close();
				}
				throw ex;
			}
			bool on2 = Logging.On;
		}

		internal TcpClient(Socket acceptedSocket)
		{
			bool on = Logging.On;
			this.Client = acceptedSocket;
			this.m_Active = true;
			bool on2 = Logging.On;
		}

		/// <summary>Gets or sets the underlying <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>The underlying network <see cref="T:System.Net.Sockets.Socket" />.</returns>
		public Socket Client
		{
			get
			{
				return this.m_ClientSocket;
			}
			set
			{
				this.m_ClientSocket = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether a connection has been made.</summary>
		/// <returns>
		///   <see langword="true" /> if the connection has been made; otherwise, <see langword="false" />.</returns>
		protected bool Active
		{
			get
			{
				return this.m_Active;
			}
			set
			{
				this.m_Active = value;
			}
		}

		/// <summary>Gets the amount of data that has been received from the network and is available to be read.</summary>
		/// <returns>The number of bytes of data received from the network and available to be read.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Available
		{
			get
			{
				return this.m_ClientSocket.Available;
			}
		}

		/// <summary>Gets a value indicating whether the underlying <see cref="T:System.Net.Sockets.Socket" /> for a <see cref="T:System.Net.Sockets.TcpClient" /> is connected to a remote host.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Net.Sockets.TcpClient.Client" /> socket was connected to a remote resource as of the most recent operation; otherwise, <see langword="false" />.</returns>
		public bool Connected
		{
			get
			{
				return this.m_ClientSocket.Connected;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether the <see cref="T:System.Net.Sockets.TcpClient" /> allows only one client to use a port.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.TcpClient" /> allows only one client to use a specific port; otherwise, <see langword="false" />. The default is <see langword="true" /> for Windows Server 2003 and Windows XP Service Pack 2 and later, and <see langword="false" /> for all other versions.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the underlying socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The underlying <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool ExclusiveAddressUse
		{
			get
			{
				return this.m_ClientSocket.ExclusiveAddressUse;
			}
			set
			{
				this.m_ClientSocket.ExclusiveAddressUse = value;
			}
		}

		/// <summary>Connects the client to the specified port on the specified host.</summary>
		/// <param name="hostname">The DNS name of the remote host to which you intend to connect.</param>
		/// <param name="port">The port number of the remote host to which you intend to connect.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="hostname" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="port" /> parameter is not between <see cref="F:System.Net.IPEndPoint.MinPort" /> and <see cref="F:System.Net.IPEndPoint.MaxPort" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.TcpClient" /> is closed.</exception>
		public void Connect(string hostname, int port)
		{
			bool on = Logging.On;
			if (this.m_CleanedUp)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (hostname == null)
			{
				throw new ArgumentNullException("hostname");
			}
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			if (this.m_Active)
			{
				throw new SocketException(SocketError.IsConnected);
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
			Exception ex = null;
			Socket socket = null;
			Socket socket2 = null;
			try
			{
				if (this.m_ClientSocket == null)
				{
					if (Socket.OSSupportsIPv4)
					{
						socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					}
					if (Socket.OSSupportsIPv6)
					{
						socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
					}
				}
				foreach (IPAddress ipaddress in hostAddresses)
				{
					try
					{
						if (this.m_ClientSocket == null)
						{
							if (ipaddress.AddressFamily == AddressFamily.InterNetwork && socket2 != null)
							{
								socket2.Connect(ipaddress, port);
								this.m_ClientSocket = socket2;
								if (socket != null)
								{
									socket.Close();
								}
							}
							else if (socket != null)
							{
								socket.Connect(ipaddress, port);
								this.m_ClientSocket = socket;
								if (socket2 != null)
								{
									socket2.Close();
								}
							}
							this.m_Family = ipaddress.AddressFamily;
							this.m_Active = true;
							break;
						}
						if (ipaddress.AddressFamily == this.m_Family)
						{
							this.Connect(new IPEndPoint(ipaddress, port));
							this.m_Active = true;
							break;
						}
					}
					catch (Exception ex2)
					{
						if (ex2 is ThreadAbortException || ex2 is StackOverflowException || ex2 is OutOfMemoryException)
						{
							throw;
						}
						ex = ex2;
					}
				}
			}
			catch (Exception ex3)
			{
				if (ex3 is ThreadAbortException || ex3 is StackOverflowException || ex3 is OutOfMemoryException)
				{
					throw;
				}
				ex = ex3;
			}
			finally
			{
				if (!this.m_Active)
				{
					if (socket != null)
					{
						socket.Close();
					}
					if (socket2 != null)
					{
						socket2.Close();
					}
					if (ex != null)
					{
						throw ex;
					}
					throw new SocketException(SocketError.NotConnected);
				}
			}
			bool on2 = Logging.On;
		}

		/// <summary>Connects the client to a remote TCP host using the specified IP address and port number.</summary>
		/// <param name="address">The <see cref="T:System.Net.IPAddress" /> of the host to which you intend to connect.</param>
		/// <param name="port">The port number to which you intend to connect.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="port" /> is not between <see cref="F:System.Net.IPEndPoint.MinPort" /> and <see cref="F:System.Net.IPEndPoint.MaxPort" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.TcpClient" /> is closed.</exception>
		public void Connect(IPAddress address, int port)
		{
			bool on = Logging.On;
			if (this.m_CleanedUp)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			IPEndPoint remoteEP = new IPEndPoint(address, port);
			this.Connect(remoteEP);
			bool on2 = Logging.On;
		}

		/// <summary>Connects the client to a remote TCP host using the specified remote network endpoint.</summary>
		/// <param name="remoteEP">The <see cref="T:System.Net.IPEndPoint" /> to which you intend to connect.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="remoteEp" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.TcpClient" /> is closed.</exception>
		public void Connect(IPEndPoint remoteEP)
		{
			bool on = Logging.On;
			if (this.m_CleanedUp)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			this.Client.Connect(remoteEP);
			this.m_Active = true;
			bool on2 = Logging.On;
		}

		/// <summary>Connects the client to a remote TCP host using the specified IP addresses and port number.</summary>
		/// <param name="ipAddresses">The <see cref="T:System.Net.IPAddress" /> array of the host to which you intend to connect.</param>
		/// <param name="port">The port number to which you intend to connect.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="ipAddresses" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		/// <exception cref="T:System.NotSupportedException">This method is valid for sockets that use the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> flag or the <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> flag.</exception>
		public void Connect(IPAddress[] ipAddresses, int port)
		{
			bool on = Logging.On;
			this.Client.Connect(ipAddresses, port);
			this.m_Active = true;
			bool on2 = Logging.On;
		}

		/// <summary>Begins an asynchronous request for a remote host connection. The remote host is specified by a host name (<see cref="T:System.String" />) and a port number (<see cref="T:System.Int32" />).</summary>
		/// <param name="host">The name of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the connect operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that references the asynchronous connection.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="host" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
		{
			bool on = Logging.On;
			IAsyncResult result = this.Client.BeginConnect(host, port, requestCallback, state);
			bool on2 = Logging.On;
			return result;
		}

		/// <summary>Begins an asynchronous request for a remote host connection. The remote host is specified by an <see cref="T:System.Net.IPAddress" /> and a port number (<see cref="T:System.Int32" />).</summary>
		/// <param name="address">The <see cref="T:System.Net.IPAddress" /> of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the connect operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that references the asynchronous connection.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
		{
			bool on = Logging.On;
			IAsyncResult result = this.Client.BeginConnect(address, port, requestCallback, state);
			bool on2 = Logging.On;
			return result;
		}

		/// <summary>Begins an asynchronous request for a remote host connection. The remote host is specified by an <see cref="T:System.Net.IPAddress" /> array and a port number (<see cref="T:System.Int32" />).</summary>
		/// <param name="addresses">At least one <see cref="T:System.Net.IPAddress" /> that designates the remote hosts.</param>
		/// <param name="port">The port number of the remote hosts.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the connect operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that references the asynchronous connection.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="addresses" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
		{
			bool on = Logging.On;
			IAsyncResult result = this.Client.BeginConnect(addresses, port, requestCallback, state);
			bool on2 = Logging.On;
			return result;
		}

		/// <summary>Ends a pending asynchronous connection attempt.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> object returned by a call to <see cref="Overload:System.Net.Sockets.TcpClient.BeginConnect" />.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="asyncResult" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="asyncResult" /> parameter was not returned by a call to a <see cref="Overload:System.Net.Sockets.TcpClient.BeginConnect" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="M:System.Net.Sockets.TcpClient.EndConnect(System.IAsyncResult)" /> method was previously called for the asynchronous connection.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The underlying <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void EndConnect(IAsyncResult asyncResult)
		{
			bool on = Logging.On;
			this.Client.EndConnect(asyncResult);
			this.m_Active = true;
			bool on2 = Logging.On;
		}

		/// <summary>Connects the client to a remote TCP host using the specified IP address and port number as an asynchronous operation.</summary>
		/// <param name="address">The <see cref="T:System.Net.IPAddress" /> of the host to which you intend to connect.</param>
		/// <param name="port">The port number to which you intend to connect.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="address" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="port" /> is not between <see cref="F:System.Net.IPEndPoint.MinPort" /> and <see cref="F:System.Net.IPEndPoint.MaxPort" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.TcpClient" /> is closed.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public Task ConnectAsync(IPAddress address, int port)
		{
			return Task.Factory.FromAsync<IPAddress, int>(new Func<IPAddress, int, AsyncCallback, object, IAsyncResult>(this.BeginConnect), new Action<IAsyncResult>(this.EndConnect), address, port, null);
		}

		/// <summary>Connects the client to the specified TCP port on the specified host as an asynchronous operation.</summary>
		/// <param name="host">The DNS name of the remote host to which you intend to connect.</param>
		/// <param name="port">The port number of the remote host to which you intend to connect.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="hostname" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="port" /> parameter is not between <see cref="F:System.Net.IPEndPoint.MinPort" /> and <see cref="F:System.Net.IPEndPoint.MaxPort" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when accessing the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.TcpClient" /> is closed.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public Task ConnectAsync(string host, int port)
		{
			return Task.Factory.FromAsync<string, int>(new Func<string, int, AsyncCallback, object, IAsyncResult>(this.BeginConnect), new Action<IAsyncResult>(this.EndConnect), host, port, null);
		}

		/// <summary>Connects the client to a remote TCP host using the specified IP addresses and port number as an asynchronous operation.</summary>
		/// <param name="addresses">The <see cref="T:System.Net.IPAddress" /> array of the host to which you intend to connect.</param>
		/// <param name="port">The port number to which you intend to connect.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="ipAddresses" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		/// <exception cref="T:System.NotSupportedException">This method is valid for sockets that use the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> flag or the <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> flag.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public Task ConnectAsync(IPAddress[] addresses, int port)
		{
			return Task.Factory.FromAsync<IPAddress[], int>(new Func<IPAddress[], int, AsyncCallback, object, IAsyncResult>(this.BeginConnect), new Action<IAsyncResult>(this.EndConnect), addresses, port, null);
		}

		/// <summary>Returns the <see cref="T:System.Net.Sockets.NetworkStream" /> used to send and receive data.</summary>
		/// <returns>The underlying <see cref="T:System.Net.Sockets.NetworkStream" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.TcpClient" /> is not connected to a remote host.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.TcpClient" /> has been closed.</exception>
		public NetworkStream GetStream()
		{
			bool on = Logging.On;
			if (this.m_CleanedUp)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (!this.Client.Connected)
			{
				throw new InvalidOperationException(SR.GetString("The operation is not allowed on non-connected sockets."));
			}
			if (this.m_DataStream == null)
			{
				this.m_DataStream = new NetworkStream(this.Client, true);
			}
			bool on2 = Logging.On;
			return this.m_DataStream;
		}

		/// <summary>Disposes this <see cref="T:System.Net.Sockets.TcpClient" /> instance and requests that the underlying TCP connection be closed.</summary>
		public void Close()
		{
			bool on = Logging.On;
			((IDisposable)this).Dispose();
			bool on2 = Logging.On;
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.TcpClient" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">Set to <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			bool on = Logging.On;
			if (this.m_CleanedUp)
			{
				bool on2 = Logging.On;
				return;
			}
			if (disposing)
			{
				IDisposable dataStream = this.m_DataStream;
				if (dataStream != null)
				{
					dataStream.Dispose();
				}
				else
				{
					Socket client = this.Client;
					if (client != null)
					{
						try
						{
							client.InternalShutdown(SocketShutdown.Both);
						}
						finally
						{
							client.Close();
							this.Client = null;
						}
					}
				}
				GC.SuppressFinalize(this);
			}
			this.m_CleanedUp = true;
			bool on3 = Logging.On;
		}

		/// <summary>Releases the managed and unmanaged resources used by the <see cref="T:System.Net.Sockets.TcpClient" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Frees resources used by the <see cref="T:System.Net.Sockets.TcpClient" /> class.</summary>
		~TcpClient()
		{
			this.Dispose(false);
		}

		/// <summary>Gets or sets the size of the receive buffer.</summary>
		/// <returns>The size of the receive buffer, in bytes. The default value is 8192 bytes.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when setting the buffer size.  
		///  -or-  
		///  In .NET Compact Framework applications, you cannot set this property. For a workaround, see the Platform Note in Remarks.</exception>
		public int ReceiveBufferSize
		{
			get
			{
				return this.numericOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}

		/// <summary>Gets or sets the size of the send buffer.</summary>
		/// <returns>The size of the send buffer, in bytes. The default value is 8192 bytes.</returns>
		public int SendBufferSize
		{
			get
			{
				return this.numericOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}

		/// <summary>Gets or sets the amount of time a <see cref="T:System.Net.Sockets.TcpClient" /> will wait to receive data once a read operation is initiated.</summary>
		/// <returns>The time-out value of the connection in milliseconds. The default value is 0.</returns>
		public int ReceiveTimeout
		{
			get
			{
				return this.numericOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}

		/// <summary>Gets or sets the amount of time a <see cref="T:System.Net.Sockets.TcpClient" /> will wait for a send operation to complete successfully.</summary>
		/// <returns>The send time-out value, in milliseconds. The default is 0.</returns>
		public int SendTimeout
		{
			get
			{
				return this.numericOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}

		/// <summary>Gets or sets information about the linger state of the associated socket.</summary>
		/// <returns>A <see cref="T:System.Net.Sockets.LingerOption" />. By default, lingering is disabled.</returns>
		public LingerOption LingerState
		{
			get
			{
				return (LingerOption)this.Client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}

		/// <summary>Gets or sets a value that disables a delay when send or receive buffers are not full.</summary>
		/// <returns>
		///   <see langword="true" /> if the delay is disabled; otherwise, <see langword="false" />. The default value is <see langword="false" />.</returns>
		public bool NoDelay
		{
			get
			{
				return this.numericOption(SocketOptionLevel.Tcp, SocketOptionName.Debug) != 0;
			}
			set
			{
				this.Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, value ? 1 : 0);
			}
		}

		private void initialize()
		{
			this.Client = new Socket(this.m_Family, SocketType.Stream, ProtocolType.Tcp);
			this.m_Active = false;
		}

		private int numericOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
		{
			return (int)this.Client.GetSocketOption(optionLevel, optionName);
		}

		private Socket m_ClientSocket;

		private bool m_Active;

		private NetworkStream m_DataStream;

		private AddressFamily m_Family = AddressFamily.InterNetwork;

		private bool m_CleanedUp;
	}
}
