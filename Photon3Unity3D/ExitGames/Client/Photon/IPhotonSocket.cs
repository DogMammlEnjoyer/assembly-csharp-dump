using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ExitGames.Client.Photon
{
	public abstract class IPhotonSocket
	{
		protected IPhotonPeerListener Listener
		{
			get
			{
				return this.peerBase.Listener;
			}
		}

		protected internal int MTU
		{
			get
			{
				return this.peerBase.mtu;
			}
		}

		public PhotonSocketState State { get; protected set; }

		public int SocketErrorCode { get; protected set; }

		public bool Connected
		{
			get
			{
				return this.State == PhotonSocketState.Connected;
			}
		}

		public string ServerAddress { get; protected set; }

		public string ProxyServerAddress { get; protected set; }

		public static string ServerIpAddress { get; protected set; }

		public int ServerPort { get; protected set; }

		public bool AddressResolvedAsIpv6 { get; protected internal set; }

		public string UrlProtocol { get; protected set; }

		public string UrlPath { get; protected set; }

		protected internal string SerializationProtocol
		{
			get
			{
				bool flag = this.peerBase == null || this.peerBase.photonPeer == null;
				string result;
				if (flag)
				{
					result = "GpBinaryV18";
				}
				else
				{
					result = Enum.GetName(typeof(SerializationProtocol), this.peerBase.photonPeer.SerializationProtocolType);
				}
				return result;
			}
		}

		public IPhotonSocket(PeerBase peerBase)
		{
			bool flag = peerBase == null;
			if (flag)
			{
				throw new Exception("Can't init without peer");
			}
			this.Protocol = peerBase.usedTransportProtocol;
			this.peerBase = peerBase;
			this.ConnectAddress = this.peerBase.ServerAddress;
		}

		public virtual bool Connect()
		{
			bool flag = this.State > PhotonSocketState.Disconnected;
			bool result;
			if (flag)
			{
				bool flag2 = this.peerBase.debugOut >= DebugLevel.ERROR;
				if (flag2)
				{
					this.peerBase.Listener.DebugReturn(DebugLevel.ERROR, "Connect() failed: connection in State: " + this.State.ToString());
				}
				result = false;
			}
			else
			{
				bool flag3 = this.peerBase == null || this.Protocol != this.peerBase.usedTransportProtocol;
				if (flag3)
				{
					result = false;
				}
				else
				{
					string serverAddress;
					ushort serverPort;
					string urlProtocol;
					string urlPath;
					bool flag4 = !this.TryParseAddress(this.peerBase.ServerAddress, out serverAddress, out serverPort, out urlProtocol, out urlPath);
					if (flag4)
					{
						bool flag5 = this.peerBase.debugOut >= DebugLevel.ERROR;
						if (flag5)
						{
							this.peerBase.Listener.DebugReturn(DebugLevel.ERROR, "Failed parsing address: " + this.peerBase.ServerAddress);
						}
						result = false;
					}
					else
					{
						IPhotonSocket.ServerIpAddress = string.Empty;
						this.ServerAddress = serverAddress;
						this.ServerPort = (int)serverPort;
						this.UrlProtocol = urlProtocol;
						this.UrlPath = urlPath;
						bool flag6 = this.peerBase.debugOut >= DebugLevel.ALL;
						if (flag6)
						{
							this.Listener.DebugReturn(DebugLevel.ALL, string.Concat(new string[]
							{
								"IPhotonSocket.Connect() ",
								this.ServerAddress,
								":",
								this.ServerPort.ToString(),
								" this.Protocol: ",
								this.Protocol.ToString()
							}));
						}
						result = true;
					}
				}
			}
			return result;
		}

		public abstract bool Disconnect();

		public abstract PhotonSocketError Send(byte[] data, int length);

		public abstract PhotonSocketError Receive(out byte[] data);

		public void HandleReceivedDatagram(byte[] inBuffer, int length, bool willBeReused)
		{
			ITrafficRecorder trafficRecorder = this.peerBase.photonPeer.TrafficRecorder;
			bool flag = trafficRecorder != null && trafficRecorder.Enabled;
			if (flag)
			{
				trafficRecorder.Record(inBuffer, length, true, this.peerBase.peerID, this);
			}
			bool isSimulationEnabled = this.peerBase.NetworkSimulationSettings.IsSimulationEnabled;
			if (isSimulationEnabled)
			{
				if (willBeReused)
				{
					byte[] array = new byte[length];
					Buffer.BlockCopy(inBuffer, 0, array, 0, length);
					this.peerBase.ReceiveNetworkSimulated(array);
				}
				else
				{
					this.peerBase.ReceiveNetworkSimulated(inBuffer);
				}
			}
			else
			{
				this.peerBase.ReceiveIncomingCommands(inBuffer, length);
			}
		}

		public bool ReportDebugOfLevel(DebugLevel levelOfMessage)
		{
			return this.peerBase.debugOut >= levelOfMessage;
		}

		public void EnqueueDebugReturn(DebugLevel debugLevel, string message)
		{
			this.peerBase.EnqueueDebugReturn(debugLevel, message);
		}

		protected internal void HandleException(StatusCode statusCode)
		{
			this.State = PhotonSocketState.Disconnecting;
			this.peerBase.EnqueueStatusCallback(statusCode);
			this.peerBase.EnqueueActionForDispatch(delegate
			{
				this.peerBase.Disconnect();
			});
		}

		protected internal bool TryParseAddress(string url, out string host, out ushort port, out string scheme, out string absolutePath)
		{
			host = string.Empty;
			port = 0;
			scheme = string.Empty;
			absolutePath = string.Empty;
			bool flag = string.IsNullOrEmpty(url);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = url.Contains("://");
				string uriString = flag2 ? url : ("net.tcp://" + url);
				Uri uri;
				bool flag3 = Uri.TryCreate(uriString, UriKind.Absolute, out uri);
				bool flag4 = flag3;
				if (flag4)
				{
					host = uri.Host;
					port = ((!flag2 && !url.Contains(string.Format(":{0}", uri.Port))) ? 0 : ((ushort)uri.Port));
					scheme = (flag2 ? uri.Scheme : string.Empty);
					absolutePath = ("/".Equals(uri.AbsolutePath) ? string.Empty : uri.AbsolutePath);
				}
				result = flag3;
			}
			return result;
		}

		private bool IpAddressTryParse(string strIP, out IPAddress address)
		{
			address = null;
			bool flag = string.IsNullOrEmpty(strIP);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string[] array = strIP.Split(new char[]
				{
					'.'
				});
				bool flag2 = array.Length != 4;
				if (flag2)
				{
					result = false;
				}
				else
				{
					byte[] array2 = new byte[4];
					for (int i = 0; i < array.Length; i++)
					{
						string s = array[i];
						byte b = 0;
						bool flag3 = !byte.TryParse(s, out b);
						if (flag3)
						{
							return false;
						}
						array2[i] = b;
					}
					bool flag4 = array2[0] == 0;
					if (flag4)
					{
						result = false;
					}
					else
					{
						address = new IPAddress(array2);
						result = true;
					}
				}
			}
			return result;
		}

		protected internal IPAddress[] GetIpAddresses(string hostname)
		{
			IPAddress ipaddress = null;
			bool flag = IPAddress.TryParse(hostname, out ipaddress);
			IPAddress[] result;
			if (flag)
			{
				bool flag2 = ipaddress.AddressFamily == AddressFamily.InterNetworkV6 || this.IpAddressTryParse(hostname, out ipaddress);
				if (flag2)
				{
					result = new IPAddress[]
					{
						ipaddress
					};
				}
				else
				{
					this.HandleException(StatusCode.ServerAddressInvalid);
					result = null;
				}
			}
			else
			{
				IPAddress[] array;
				try
				{
					array = Dns.GetHostAddresses(this.ServerAddress);
				}
				catch (Exception ex)
				{
					try
					{
						IPHostEntry hostByName = Dns.GetHostByName(this.ServerAddress);
						array = hostByName.AddressList;
					}
					catch (Exception ex2)
					{
						bool flag3 = this.ReportDebugOfLevel(DebugLevel.WARNING);
						if (flag3)
						{
							DebugLevel debugLevel = DebugLevel.WARNING;
							string[] array2 = new string[6];
							array2[0] = "GetHostAddresses and GetHostEntry() failed for: ";
							array2[1] = this.ServerAddress;
							array2[2] = ". Caught and handled exceptions:\n";
							int num = 3;
							Exception ex3 = ex;
							array2[num] = ((ex3 != null) ? ex3.ToString() : null);
							array2[4] = "\n";
							int num2 = 5;
							Exception ex4 = ex2;
							array2[num2] = ((ex4 != null) ? ex4.ToString() : null);
							this.EnqueueDebugReturn(debugLevel, string.Concat(array2));
						}
						this.HandleException(StatusCode.DnsExceptionOnConnect);
						return null;
					}
				}
				Array.Sort<IPAddress>(array, new Comparison<IPAddress>(this.AddressSortComparer));
				bool flag4 = this.ReportDebugOfLevel(DebugLevel.INFO);
				if (flag4)
				{
					string[] array3 = (from x in array
					select string.Concat(new string[]
					{
						x.ToString(),
						" (",
						x.AddressFamily.ToString(),
						"(",
						((int)x.AddressFamily).ToString(),
						"))"
					})).ToArray<string>();
					string text = string.Join(", ", array3);
					bool flag5 = this.ReportDebugOfLevel(DebugLevel.INFO);
					if (flag5)
					{
						this.EnqueueDebugReturn(DebugLevel.INFO, string.Concat(new string[]
						{
							this.ServerAddress,
							" resolved to ",
							array3.Length.ToString(),
							" address(es): ",
							text
						}));
					}
				}
				result = array;
			}
			return result;
		}

		private int AddressSortComparer(IPAddress x, IPAddress y)
		{
			bool flag = x.AddressFamily == y.AddressFamily;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = ((x.AddressFamily == AddressFamily.InterNetworkV6) ? -1 : 1);
			}
			return result;
		}

		[Obsolete("Use GetIpAddresses instead.")]
		protected internal static IPAddress GetIpAddress(string address)
		{
			IPAddress ipaddress = null;
			bool flag = IPAddress.TryParse(address, out ipaddress);
			IPAddress result;
			if (flag)
			{
				result = ipaddress;
			}
			else
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(address);
				IPAddress[] addressList = hostEntry.AddressList;
				foreach (IPAddress ipaddress2 in addressList)
				{
					bool flag2 = ipaddress2.AddressFamily == AddressFamily.InterNetworkV6;
					if (flag2)
					{
						IPhotonSocket.ServerIpAddress = ipaddress2.ToString();
						return ipaddress2;
					}
					bool flag3 = ipaddress == null && ipaddress2.AddressFamily == AddressFamily.InterNetwork;
					if (flag3)
					{
						ipaddress = ipaddress2;
					}
				}
				IPhotonSocket.ServerIpAddress = ((ipaddress != null) ? ipaddress.ToString() : (address + " not resolved"));
				result = ipaddress;
			}
			return result;
		}

		protected internal PeerBase peerBase;

		protected readonly ConnectionProtocol Protocol;

		public bool PollReceive;

		public string ConnectAddress;
	}
}
