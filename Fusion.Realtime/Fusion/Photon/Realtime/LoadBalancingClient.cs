using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class LoadBalancingClient : IPhotonPeerListener
	{
		public LoadBalancingPeer LoadBalancingPeer { get; private set; }

		public SerializationProtocol SerializationProtocol
		{
			get
			{
				return this.LoadBalancingPeer.SerializationProtocolType;
			}
			set
			{
				this.LoadBalancingPeer.SerializationProtocolType = value;
			}
		}

		public string AppVersion { get; set; }

		public string AppId { get; set; }

		public ClientAppType ClientType { get; set; }

		public AuthenticationValues AuthValues { get; set; }

		public ConnectionProtocol? ExpectedProtocol { get; set; }

		private object TokenForInit
		{
			get
			{
				bool flag = this.AuthMode == AuthModeOption.Auth;
				object result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = ((this.AuthValues != null) ? this.AuthValues.Token : null);
				}
				return result;
			}
		}

		public bool IsUsingNameServer { get; set; }

		public string NameServerAddress
		{
			get
			{
				return this.GetNameServerAddress();
			}
		}

		[Obsolete("Set port overrides in ServerPortOverrides. Not used anymore!")]
		public bool UseAlternativeUdpPorts { get; set; }

		public bool EnableProtocolFallback { get; set; }

		public string CurrentServerAddress
		{
			get
			{
				return this.LoadBalancingPeer.ServerAddress;
			}
		}

		public string MasterServerAddress { get; set; }

		public string GameServerAddress { get; protected internal set; }

		public ServerConnection Server { get; private set; }

		public int ConnectCount { get; private set; }

		public ClientState State
		{
			get
			{
				return this.state;
			}
			set
			{
				bool flag = this.state == value;
				if (!flag)
				{
					ClientState arg = this.state;
					this.state = value;
					bool flag2 = this.StateChanged != null;
					if (flag2)
					{
						this.StateChanged(arg, this.state);
					}
				}
			}
		}

		public bool IsConnected
		{
			get
			{
				return this.LoadBalancingPeer != null && this.State != ClientState.PeerCreated && this.State != ClientState.Disconnected;
			}
		}

		public bool IsConnectedAndReady
		{
			get
			{
				bool flag = this.LoadBalancingPeer == null;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					switch (this.State)
					{
					case ClientState.PeerCreated:
					case ClientState.Authenticating:
					case ClientState.DisconnectingFromMasterServer:
					case ClientState.ConnectingToGameServer:
					case ClientState.Joining:
					case ClientState.Leaving:
					case ClientState.DisconnectingFromGameServer:
					case ClientState.ConnectingToMasterServer:
					case ClientState.Disconnecting:
					case ClientState.Disconnected:
					case ClientState.ConnectingToNameServer:
					case ClientState.DisconnectingFromNameServer:
						return false;
					}
					result = true;
				}
				return result;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ClientState, ClientState> StateChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EventData> EventReceived;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<OperationResponse> OpResponseReceived;

		public DisconnectCause DisconnectedCause { get; protected set; }

		public bool InLobby
		{
			get
			{
				return this.State == ClientState.JoinedLobby;
			}
		}

		public TypedLobby CurrentLobby { get; internal set; }

		public Player LocalPlayer { get; internal set; }

		public string NickName
		{
			get
			{
				return this.LocalPlayer.NickName;
			}
			set
			{
				bool flag = this.LocalPlayer == null;
				if (!flag)
				{
					this.LocalPlayer.NickName = value;
				}
			}
		}

		public string UserId
		{
			get
			{
				bool flag = this.AuthValues != null;
				string result;
				if (flag)
				{
					result = this.AuthValues.UserId;
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				bool flag = this.AuthValues == null;
				if (flag)
				{
					this.AuthValues = new AuthenticationValues();
				}
				this.AuthValues.UserId = value;
			}
		}

		public Room CurrentRoom { get; set; }

		public bool InRoom
		{
			get
			{
				return this.state == ClientState.Joined && this.CurrentRoom != null;
			}
		}

		public int PlayersOnMasterCount { get; internal set; }

		public int PlayersInRoomsCount { get; internal set; }

		public int RoomsCount { get; internal set; }

		public bool IsFetchingFriendList
		{
			get
			{
				return this.friendListRequested != null;
			}
		}

		public string CloudRegion { get; private set; }

		public string CurrentCluster { get; private set; }

		public LoadBalancingClient(ConnectionProtocol protocol = ConnectionProtocol.Udp)
		{
			this.ConnectionCallbackTargets = new ConnectionCallbacksContainer(this);
			this.MatchMakingCallbackTargets = new MatchMakingCallbacksContainer(this);
			this.InRoomCallbackTargets = new InRoomCallbacksContainer(this);
			this.LobbyCallbackTargets = new LobbyCallbacksContainer(this);
			this.WebRpcCallbackTargets = new WebRpcCallbacksContainer(this);
			this.ErrorInfoCallbackTargets = new ErrorInfoCallbacksContainer(this);
			this.LoadBalancingPeer = new LoadBalancingPeer(this, protocol);
			this.LoadBalancingPeer.OnDisconnectMessage += this.OnDisconnectMessageReceived;
			this.SerializationProtocol = SerializationProtocol.GpBinaryV18;
			this.LocalPlayer = this.CreatePlayer(string.Empty, -1, true, null);
			CustomTypesUnity.Register();
			bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
			if (isUNITY_WEBGL)
			{
				bool flag = this.LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Tcp || this.LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Udp;
				if (flag)
				{
					this.LoadBalancingPeer.Listener.DebugReturn(DebugLevel.WARNING, "WebGL requires WebSockets. Switching TransportProtocol to WebSocketSecure.");
					this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
				}
			}
			this.State = ClientState.PeerCreated;
		}

		public LoadBalancingClient(string masterAddress, string appId, string gameVersion, ConnectionProtocol protocol = ConnectionProtocol.Udp) : this(protocol)
		{
			this.MasterServerAddress = masterAddress;
			this.AppId = appId;
			this.AppVersion = gameVersion;
		}

		private string GetNameServerAddress()
		{
			int port = 0;
			LoadBalancingClient.ProtocolToNameServerPort.TryGetValue(this.LoadBalancingPeer.TransportProtocol, out port);
			bool flag = this.NameServerPortInAppSettings != 0;
			if (flag)
			{
				this.DebugReturn(DebugLevel.INFO, string.Format("Using NameServerPortInAppSettings: {0}", this.NameServerPortInAppSettings));
				port = this.NameServerPortInAppSettings;
			}
			bool flag2 = this.ServerPortOverrides.NameServerPort > 0;
			if (flag2)
			{
				port = (int)this.ServerPortOverrides.NameServerPort;
			}
			return this.ToProtocolAddress(this.NameServerHost, port, this.LoadBalancingPeer.TransportProtocol);
		}

		private string ToProtocolAddress(string address, int port, ConnectionProtocol protocol)
		{
			string str = string.Empty;
			switch (protocol)
			{
			case ConnectionProtocol.Udp:
			case ConnectionProtocol.Tcp:
				return string.Format("{0}:{1}", address, port);
			case ConnectionProtocol.WebSocket:
				str = "ws://";
				goto IL_6B;
			case ConnectionProtocol.WebSocketSecure:
				str = "wss://";
				goto IL_6B;
			}
			throw new ArgumentOutOfRangeException(string.Format("Can not handle protocol: {0}.", protocol));
			IL_6B:
			Uri uri = new Uri(str + address);
			string text = string.Format("{0}://{1}:{2}{3}", new object[]
			{
				uri.Scheme,
				uri.Host,
				port,
				uri.AbsolutePath
			});
			bool flag = this.AddressRewriter != null;
			if (flag)
			{
				text = this.AddressRewriter(text, ServerConnection.NameServer);
			}
			return text;
		}

		public virtual bool ConnectUsingSettings(AppSettings appSettings)
		{
			bool flag = this.LoadBalancingPeer.PeerState > PeerStateValue.Disconnected;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.WARNING, "ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				result = false;
			}
			else
			{
				bool flag2 = appSettings == null;
				if (flag2)
				{
					this.DebugReturn(DebugLevel.ERROR, "ConnectUsingSettings failed. The appSettings can't be null.'");
					result = false;
				}
				else
				{
					switch (this.ClientType)
					{
					case ClientAppType.Realtime:
						this.AppId = appSettings.AppIdRealtime;
						break;
					case ClientAppType.Voice:
						this.AppId = appSettings.AppIdVoice;
						break;
					case ClientAppType.Fusion:
						this.AppId = appSettings.AppIdFusion;
						break;
					}
					this.AppVersion = appSettings.AppVersion;
					this.IsUsingNameServer = appSettings.UseNameServer;
					this.CloudRegion = appSettings.FixedRegion;
					this.connectToBestRegion = string.IsNullOrEmpty(this.CloudRegion);
					this.EnableLobbyStatistics = appSettings.EnableLobbyStatistics;
					this.LoadBalancingPeer.DebugOut = appSettings.NetworkLogging;
					this.AuthMode = appSettings.AuthMode;
					bool flag3 = appSettings.AuthMode == AuthModeOption.AuthOnceWss;
					if (flag3)
					{
						this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
						this.ExpectedProtocol = new ConnectionProtocol?(appSettings.Protocol);
					}
					else
					{
						this.LoadBalancingPeer.TransportProtocol = appSettings.Protocol;
						this.ExpectedProtocol = null;
					}
					this.EnableProtocolFallback = appSettings.EnableProtocolFallback;
					this.bestRegionSummaryFromStorage = appSettings.BestRegionSummaryFromStorage;
					this.DisconnectedCause = DisconnectCause.None;
					this.DisconnectMessage = null;
					this.SystemConnectionSummary = null;
					this.CheckConnectSetupWebGl();
					bool isUsingNameServer = this.IsUsingNameServer;
					if (isUsingNameServer)
					{
						this.Server = ServerConnection.NameServer;
						bool flag4 = !appSettings.IsDefaultNameServer;
						if (flag4)
						{
							this.NameServerHost = appSettings.Server;
						}
						this.ProxyServerAddress = appSettings.ProxyServer;
						this.NameServerPortInAppSettings = appSettings.Port;
						bool flag5 = !this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null);
						if (flag5)
						{
							return false;
						}
						this.State = ClientState.ConnectingToNameServer;
					}
					else
					{
						this.Server = ServerConnection.MasterServer;
						int port = appSettings.IsDefaultPort ? 5055 : appSettings.Port;
						this.MasterServerAddress = this.ToProtocolAddress(appSettings.Server, port, this.LoadBalancingPeer.TransportProtocol);
						bool flag6 = !this.LoadBalancingPeer.Connect(this.MasterServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null);
						if (flag6)
						{
							return false;
						}
						this.State = ClientState.ConnectingToMasterServer;
					}
					result = true;
				}
			}
			return result;
		}

		[Obsolete("Use ConnectToMasterServer() instead.")]
		public bool Connect()
		{
			return this.ConnectToMasterServer();
		}

		public virtual bool ConnectToMasterServer()
		{
			bool flag = this.LoadBalancingPeer.PeerState > PeerStateValue.Disconnected;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.WARNING, "ConnectToMasterServer() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				result = false;
			}
			else
			{
				bool flag2 = this.AuthMode != AuthModeOption.Auth && this.TokenForInit == null;
				if (flag2)
				{
					this.DebugReturn(DebugLevel.ERROR, "Connect() failed. Can't connect to MasterServer with Token == null in AuthMode: " + this.AuthMode.ToString());
					result = false;
				}
				else
				{
					this.CheckConnectSetupWebGl();
					this.DisconnectedCause = DisconnectCause.None;
					this.DisconnectMessage = null;
					this.SystemConnectionSummary = null;
					bool flag3 = this.LoadBalancingPeer.Connect(this.MasterServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null);
					if (flag3)
					{
						this.connectToBestRegion = false;
						this.State = ClientState.ConnectingToMasterServer;
						this.Server = ServerConnection.MasterServer;
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public bool ConnectToNameServer()
		{
			bool flag = this.LoadBalancingPeer.PeerState > PeerStateValue.Disconnected;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.WARNING, "ConnectToNameServer() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				result = false;
			}
			else
			{
				this.IsUsingNameServer = true;
				this.CloudRegion = null;
				this.CheckConnectSetupWebGl();
				bool flag2 = this.AuthMode == AuthModeOption.AuthOnceWss;
				if (flag2)
				{
					bool flag3 = this.ExpectedProtocol == null;
					if (flag3)
					{
						this.ExpectedProtocol = new ConnectionProtocol?(this.LoadBalancingPeer.TransportProtocol);
					}
					this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
				}
				this.DisconnectedCause = DisconnectCause.None;
				this.DisconnectMessage = null;
				this.SystemConnectionSummary = null;
				bool flag4 = this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, "NameServer", this.TokenForInit, null);
				if (flag4)
				{
					this.connectToBestRegion = false;
					this.State = ClientState.ConnectingToNameServer;
					this.Server = ServerConnection.NameServer;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool ConnectToRegionMaster(string region)
		{
			bool flag = string.IsNullOrEmpty(region);
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, "ConnectToRegionMaster() failed. The region can not be null or empty.");
				result = false;
			}
			else
			{
				this.IsUsingNameServer = true;
				bool flag2 = this.State == ClientState.Authenticating;
				if (flag2)
				{
					bool flag3 = this.LoadBalancingPeer.DebugOut >= DebugLevel.INFO;
					if (flag3)
					{
						this.DebugReturn(DebugLevel.INFO, "ConnectToRegionMaster() will skip calling authenticate, as the current state is 'Authenticating'. Just wait for the result.");
					}
					result = true;
				}
				else
				{
					bool flag4 = this.State == ClientState.ConnectedToNameServer;
					if (flag4)
					{
						this.CloudRegion = region;
						bool flag5 = this.CallAuthenticate();
						bool flag6 = flag5;
						if (flag6)
						{
							this.State = ClientState.Authenticating;
						}
						result = flag5;
					}
					else
					{
						this.LoadBalancingPeer.Disconnect();
						this.CloudRegion = region;
						this.CheckConnectSetupWebGl();
						bool flag7 = this.AuthMode == AuthModeOption.AuthOnceWss;
						if (flag7)
						{
							bool flag8 = this.ExpectedProtocol == null;
							if (flag8)
							{
								this.ExpectedProtocol = new ConnectionProtocol?(this.LoadBalancingPeer.TransportProtocol);
							}
							this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
						}
						this.connectToBestRegion = false;
						this.DisconnectedCause = DisconnectCause.None;
						this.DisconnectMessage = null;
						this.SystemConnectionSummary = null;
						bool flag9 = !this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, "NameServer", null, null);
						if (flag9)
						{
							result = false;
						}
						else
						{
							this.State = ClientState.ConnectingToNameServer;
							this.Server = ServerConnection.NameServer;
							result = true;
						}
					}
				}
			}
			return result;
		}

		private void CheckConnectSetupWebGl()
		{
			bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
			if (isUNITY_WEBGL)
			{
				bool flag = this.LoadBalancingPeer.TransportProtocol != ConnectionProtocol.WebSocket && this.LoadBalancingPeer.TransportProtocol != ConnectionProtocol.WebSocketSecure;
				if (flag)
				{
					this.DebugReturn(DebugLevel.WARNING, "WebGL requires WebSockets. Switching TransportProtocol to WebSocketSecure.");
					this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
				}
				this.EnableProtocolFallback = false;
			}
		}

		private bool Connect(string serverAddress, string proxyServerAddress, ServerConnection serverType)
		{
			bool flag = this.State == ClientState.Disconnecting;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, "Connect() failed. Can't connect while disconnecting (still). Current state: " + this.State.ToString());
				result = false;
			}
			else
			{
				bool flag2 = this.AuthMode != AuthModeOption.Auth && serverType != ServerConnection.NameServer && this.TokenForInit == null;
				if (flag2)
				{
					this.DebugReturn(DebugLevel.ERROR, "Connect() failed. Can't connect to " + serverType.ToString() + " with Token == null in AuthMode: " + this.AuthMode.ToString());
					result = false;
				}
				else
				{
					this.DisconnectedCause = DisconnectCause.None;
					this.SystemConnectionSummary = null;
					bool flag3 = this.LoadBalancingPeer.Connect(serverAddress, proxyServerAddress, this.AppId, this.TokenForInit, null);
					bool flag4 = flag3;
					if (flag4)
					{
						this.Server = serverType;
						switch (serverType)
						{
						case ServerConnection.MasterServer:
							this.State = ClientState.ConnectingToMasterServer;
							break;
						case ServerConnection.GameServer:
							this.State = ClientState.ConnectingToGameServer;
							break;
						case ServerConnection.NameServer:
							this.State = ClientState.ConnectingToNameServer;
							break;
						}
					}
					result = flag3;
				}
			}
			return result;
		}

		public bool ReconnectToMaster()
		{
			bool flag = this.LoadBalancingPeer.PeerState > PeerStateValue.Disconnected;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				result = false;
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(this.MasterServerAddress);
				if (flag2)
				{
					this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() failed. MasterServerAddress is null or empty.");
					result = false;
				}
				else
				{
					bool flag3 = this.tokenCache == null;
					if (flag3)
					{
						this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() failed. It seems the client doesn't have any previous authentication token to re-connect.");
						result = false;
					}
					else
					{
						bool flag4 = this.AuthValues == null;
						if (flag4)
						{
							this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() with AuthValues == null is not correct!");
							this.AuthValues = new AuthenticationValues();
						}
						this.AuthValues.Token = this.tokenCache;
						result = this.Connect(this.MasterServerAddress, this.ProxyServerAddress, ServerConnection.MasterServer);
					}
				}
			}
			return result;
		}

		public bool ReconnectAndRejoin()
		{
			bool flag = this.LoadBalancingPeer.PeerState > PeerStateValue.Disconnected;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				result = false;
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(this.GameServerAddress);
				if (flag2)
				{
					this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. It seems the client wasn't connected to a game server before (no address).");
					result = false;
				}
				else
				{
					bool flag3 = this.enterRoomParamsCache == null;
					if (flag3)
					{
						this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. It seems the client doesn't have any previous room to re-join.");
						result = false;
					}
					else
					{
						bool flag4 = this.tokenCache == null;
						if (flag4)
						{
							this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. It seems the client doesn't have any previous authentication token to re-connect.");
							result = false;
						}
						else
						{
							bool flag5 = this.AuthValues == null;
							if (flag5)
							{
								this.AuthValues = new AuthenticationValues();
							}
							this.AuthValues.Token = this.tokenCache;
							bool flag6 = !string.IsNullOrEmpty(this.GameServerAddress) && this.enterRoomParamsCache != null;
							if (flag6)
							{
								this.lastJoinType = JoinType.JoinRoom;
								this.enterRoomParamsCache.JoinMode = JoinMode.RejoinOnly;
								this.enterRoomParamsCache.Ticket = null;
								result = this.Connect(this.GameServerAddress, this.ProxyServerAddress, ServerConnection.GameServer);
							}
							else
							{
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		public void Disconnect()
		{
			this.Disconnect(DisconnectCause.DisconnectByClientLogic);
		}

		internal void Disconnect(DisconnectCause cause)
		{
			bool flag = this.State == ClientState.Disconnecting || this.State == ClientState.PeerCreated;
			if (flag)
			{
				this.DebugReturn(DebugLevel.INFO, string.Concat(new string[]
				{
					"Disconnect() call gets skipped due to State ",
					this.State.ToString(),
					". DisconnectedCause: ",
					this.DisconnectedCause.ToString(),
					" Parameter cause: ",
					cause.ToString()
				}));
			}
			else
			{
				bool flag2 = this.State != ClientState.Disconnected;
				if (flag2)
				{
					this.State = ClientState.Disconnecting;
					this.DisconnectedCause = cause;
					this.LoadBalancingPeer.Disconnect();
				}
			}
		}

		private void DisconnectToReconnect()
		{
			switch (this.Server)
			{
			case ServerConnection.MasterServer:
				this.State = ClientState.DisconnectingFromMasterServer;
				break;
			case ServerConnection.GameServer:
				this.State = ClientState.DisconnectingFromGameServer;
				break;
			case ServerConnection.NameServer:
				this.State = ClientState.DisconnectingFromNameServer;
				break;
			}
			this.LoadBalancingPeer.Disconnect();
		}

		public void SimulateConnectionLoss(bool simulateTimeout)
		{
			this.DebugReturn(DebugLevel.WARNING, "SimulateConnectionLoss() set to: " + simulateTimeout.ToString());
			bool flag = simulateTimeout;
			if (flag)
			{
				this.LoadBalancingPeer.NetworkSimulationSettings.IncomingLossPercentage = 100;
				this.LoadBalancingPeer.NetworkSimulationSettings.OutgoingLossPercentage = 100;
			}
			this.LoadBalancingPeer.IsSimulationEnabled = simulateTimeout;
		}

		private bool CallAuthenticate()
		{
			bool flag = this.IsUsingNameServer && this.Server != ServerConnection.NameServer && (this.AuthValues == null || this.AuthValues.Token == null);
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, string.Concat(new string[]
				{
					"Authenticate without Token is only allowed on Name Server. Connecting to: ",
					this.Server.ToString(),
					" on: ",
					this.CurrentServerAddress,
					". State: ",
					this.State.ToString()
				}));
				result = false;
			}
			else
			{
				bool flag2 = this.AuthMode == AuthModeOption.Auth;
				if (flag2)
				{
					bool flag3 = !this.CheckIfOpCanBeSent(230, this.Server, "Authenticate");
					result = (!flag3 && this.LoadBalancingPeer.OpAuthenticate(this.AppId, this.AppVersion, this.AuthValues, this.CloudRegion, this.EnableLobbyStatistics && this.Server == ServerConnection.MasterServer));
				}
				else
				{
					bool flag4 = !this.CheckIfOpCanBeSent(231, this.Server, "AuthenticateOnce");
					if (flag4)
					{
						result = false;
					}
					else
					{
						ConnectionProtocol expectedProtocol = (this.ExpectedProtocol != null) ? this.ExpectedProtocol.Value : this.LoadBalancingPeer.TransportProtocol;
						result = this.LoadBalancingPeer.OpAuthenticateOnce(this.AppId, this.AppVersion, this.AuthValues, this.CloudRegion, this.EncryptionMode, expectedProtocol);
					}
				}
			}
			return result;
		}

		public void Service()
		{
			bool flag = this.LoadBalancingPeer != null;
			if (flag)
			{
				this.LoadBalancingPeer.Service();
			}
		}

		private bool OpGetRegions()
		{
			bool flag = !this.CheckIfOpCanBeSent(220, this.Server, "GetRegions");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.LoadBalancingPeer.OpGetRegions(this.AppId);
				result = flag2;
			}
			return result;
		}

		public bool OpFindFriends(string[] friendsToFind, FindFriendsOptions options = null)
		{
			bool flag = !this.CheckIfOpCanBeSent(222, this.Server, "FindFriends");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isFetchingFriendList = this.IsFetchingFriendList;
				if (isFetchingFriendList)
				{
					this.DebugReturn(DebugLevel.WARNING, "OpFindFriends skipped: already fetching friends list.");
					result = false;
				}
				else
				{
					bool flag2 = friendsToFind == null || friendsToFind.Length == 0;
					if (flag2)
					{
						this.DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friendsToFind array is null or empty.");
						result = false;
					}
					else
					{
						bool flag3 = friendsToFind.Length > 512;
						if (flag3)
						{
							this.DebugReturn(DebugLevel.ERROR, string.Format("OpFindFriends skipped: friendsToFind array exceeds allowed length of {0}.", 512));
							result = false;
						}
						else
						{
							List<string> list = new List<string>(friendsToFind.Length);
							for (int i = 0; i < friendsToFind.Length; i++)
							{
								string text = friendsToFind[i];
								bool flag4 = string.IsNullOrEmpty(text);
								if (flag4)
								{
									this.DebugReturn(DebugLevel.WARNING, string.Format("friendsToFind array contains a null or empty UserId, element at position {0} skipped.", i));
								}
								else
								{
									bool flag5 = text.Equals(this.UserId);
									if (flag5)
									{
										this.DebugReturn(DebugLevel.WARNING, string.Format("friendsToFind array contains local player's UserId \"{0}\", element at position {1} skipped.", text, i));
									}
									else
									{
										bool flag6 = list.Contains(text);
										if (flag6)
										{
											this.DebugReturn(DebugLevel.WARNING, string.Format("friendsToFind array contains duplicate UserId \"{0}\", element at position {1} skipped.", text, i));
										}
										else
										{
											list.Add(text);
										}
									}
								}
							}
							bool flag7 = list.Count == 0;
							if (flag7)
							{
								this.DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friends list to find is empty.");
								result = false;
							}
							else
							{
								string[] array = list.ToArray();
								bool flag8 = this.LoadBalancingPeer.OpFindFriends(array, options);
								this.friendListRequested = (flag8 ? array : null);
								result = flag8;
							}
						}
					}
				}
			}
			return result;
		}

		public bool OpJoinLobby(TypedLobby lobby)
		{
			bool flag = !this.CheckIfOpCanBeSent(229, this.Server, "JoinLobby");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = lobby == null;
				if (flag2)
				{
					lobby = TypedLobby.Default;
				}
				bool flag3 = this.LoadBalancingPeer.OpJoinLobby(lobby);
				bool flag4 = flag3;
				if (flag4)
				{
					this.CurrentLobby = lobby;
					this.State = ClientState.JoiningLobby;
				}
				result = flag3;
			}
			return result;
		}

		public bool OpLeaveLobby()
		{
			bool flag = !this.CheckIfOpCanBeSent(228, this.Server, "LeaveLobby");
			return !flag && this.LoadBalancingPeer.OpLeaveLobby();
		}

		public bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams = null)
		{
			bool flag = !this.CheckIfOpCanBeSent(225, this.Server, "JoinRandomGame");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = opJoinRandomRoomParams == null;
				if (flag2)
				{
					opJoinRandomRoomParams = new OpJoinRandomRoomParams();
				}
				this.enterRoomParamsCache = new EnterRoomParams();
				this.enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;
				this.enterRoomParamsCache.ExpectedUsers = opJoinRandomRoomParams.ExpectedUsers;
				this.enterRoomParamsCache.Ticket = opJoinRandomRoomParams.Ticket;
				bool flag3 = this.LoadBalancingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
				bool flag4 = flag3;
				if (flag4)
				{
					this.lastJoinType = JoinType.JoinRandomRoom;
					this.State = ClientState.Joining;
				}
				result = flag3;
			}
			return result;
		}

		public bool OpJoinRandomOrCreateRoom(OpJoinRandomRoomParams opJoinRandomRoomParams, EnterRoomParams createRoomParams)
		{
			bool flag = !this.CheckIfOpCanBeSent(225, this.Server, "OpJoinRandomOrCreateRoom");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = opJoinRandomRoomParams == null;
				if (flag2)
				{
					opJoinRandomRoomParams = new OpJoinRandomRoomParams();
				}
				bool flag3 = createRoomParams == null;
				if (flag3)
				{
					createRoomParams = new EnterRoomParams();
				}
				createRoomParams.JoinMode = JoinMode.CreateIfNotExists;
				this.enterRoomParamsCache = createRoomParams;
				this.enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;
				this.enterRoomParamsCache.ExpectedUsers = opJoinRandomRoomParams.ExpectedUsers;
				bool flag4 = opJoinRandomRoomParams.Ticket != null;
				if (flag4)
				{
					this.enterRoomParamsCache.Ticket = opJoinRandomRoomParams.Ticket;
				}
				bool flag5 = this.LoadBalancingPeer.OpJoinRandomOrCreateRoom(opJoinRandomRoomParams, createRoomParams);
				bool flag6 = flag5;
				if (flag6)
				{
					this.lastJoinType = JoinType.JoinRandomOrCreateRoom;
					this.State = ClientState.Joining;
				}
				result = flag5;
			}
			return result;
		}

		public bool OpCreateRoom(EnterRoomParams enterRoomParams)
		{
			bool flag = !this.CheckIfOpCanBeSent(227, this.Server, "CreateGame");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.Server == ServerConnection.GameServer;
				enterRoomParams.OnGameServer = flag2;
				bool flag3 = !flag2;
				if (flag3)
				{
					this.enterRoomParamsCache = enterRoomParams;
				}
				bool flag4 = this.LoadBalancingPeer.OpCreateRoom(enterRoomParams);
				bool flag5 = flag4;
				if (flag5)
				{
					this.lastJoinType = JoinType.CreateRoom;
					this.State = ClientState.Joining;
				}
				result = flag4;
			}
			return result;
		}

		public bool OpJoinOrCreateRoom(EnterRoomParams enterRoomParams)
		{
			bool flag = !this.CheckIfOpCanBeSent(226, this.Server, "JoinOrCreateRoom");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.Server == ServerConnection.GameServer;
				enterRoomParams.JoinMode = JoinMode.CreateIfNotExists;
				enterRoomParams.OnGameServer = flag2;
				bool flag3 = !flag2;
				if (flag3)
				{
					this.enterRoomParamsCache = enterRoomParams;
				}
				bool flag4 = this.LoadBalancingPeer.OpJoinRoom(enterRoomParams);
				bool flag5 = flag4;
				if (flag5)
				{
					this.lastJoinType = JoinType.JoinOrCreateRoom;
					this.State = ClientState.Joining;
				}
				result = flag4;
			}
			return result;
		}

		public bool OpJoinRoom(EnterRoomParams enterRoomParams)
		{
			bool flag = !this.CheckIfOpCanBeSent(226, this.Server, "JoinRoom");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.Server == ServerConnection.GameServer;
				enterRoomParams.OnGameServer = flag2;
				bool flag3 = !flag2;
				if (flag3)
				{
					this.enterRoomParamsCache = enterRoomParams;
				}
				bool flag4 = this.LoadBalancingPeer.OpJoinRoom(enterRoomParams);
				bool flag5 = flag4;
				if (flag5)
				{
					this.lastJoinType = ((enterRoomParams.JoinMode == JoinMode.CreateIfNotExists) ? JoinType.JoinOrCreateRoom : JoinType.JoinRoom);
					this.State = ClientState.Joining;
				}
				result = flag4;
			}
			return result;
		}

		public bool OpRejoinRoom(string roomName, object ticket = null)
		{
			bool flag = !this.CheckIfOpCanBeSent(226, this.Server, "RejoinRoom");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool onGameServer = this.Server == ServerConnection.GameServer;
				EnterRoomParams enterRoomParams = new EnterRoomParams();
				enterRoomParams.RoomName = roomName;
				enterRoomParams.OnGameServer = onGameServer;
				enterRoomParams.JoinMode = JoinMode.RejoinOnly;
				enterRoomParams.Ticket = ticket;
				this.enterRoomParamsCache = enterRoomParams;
				bool flag2 = this.LoadBalancingPeer.OpJoinRoom(enterRoomParams);
				bool flag3 = flag2;
				if (flag3)
				{
					this.lastJoinType = JoinType.JoinRoom;
					this.State = ClientState.Joining;
				}
				result = flag2;
			}
			return result;
		}

		public bool OpLeaveRoom(bool becomeInactive, bool sendAuthCookie = false)
		{
			bool flag = !this.CheckIfOpCanBeSent(254, this.Server, "LeaveRoom");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.LoadBalancingPeer.OpLeaveRoom(becomeInactive, sendAuthCookie);
				if (flag2)
				{
					this.State = ClientState.Leaving;
					this.GameServerAddress = string.Empty;
					this.enterRoomParamsCache = null;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool OpGetGameList(TypedLobby typedLobby, string sqlLobbyFilter)
		{
			bool flag = !this.CheckIfOpCanBeSent(217, this.Server, "GetGameList");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(sqlLobbyFilter);
				if (flag2)
				{
					this.DebugReturn(DebugLevel.ERROR, "Operation GetGameList requires a filter.");
					result = false;
				}
				else
				{
					bool flag3 = typedLobby.Type != LobbyType.SqlLobby;
					if (flag3)
					{
						this.DebugReturn(DebugLevel.ERROR, "Operation GetGameList can only be used for lobbies of type SqlLobby.");
						result = false;
					}
					else
					{
						result = this.LoadBalancingPeer.OpGetGameList(typedLobby, sqlLobbyFilter);
					}
				}
			}
			return result;
		}

		public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			bool flag = propertiesToSet == null || propertiesToSet.Count == 0;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. propertiesToSet must not be null nor empty.");
				result = false;
			}
			else
			{
				bool flag2 = this.CurrentRoom == null;
				if (flag2)
				{
					bool flag3 = expectedProperties == null && webFlags == null && this.LocalPlayer != null && this.LocalPlayer.ActorNumber == actorNr;
					if (flag3)
					{
						result = this.LocalPlayer.SetCustomProperties(propertiesToSet, null, null);
					}
					else
					{
						bool flag4 = this.LoadBalancingPeer.DebugOut >= DebugLevel.ERROR;
						if (flag4)
						{
							this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. To use expectedProperties or webForward, you have to be in a room. State: " + this.State.ToString());
						}
						result = false;
					}
				}
				else
				{
					Hashtable hashtable = new Hashtable();
					hashtable.MergeStringKeys(propertiesToSet);
					bool flag5 = hashtable.Count == 0;
					if (flag5)
					{
						this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. Only string keys allowed for custom properties.");
						result = false;
					}
					else
					{
						result = this.OpSetPropertiesOfActor(actorNr, hashtable, expectedProperties, webFlags);
					}
				}
			}
			return result;
		}

		protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			bool flag = !this.CheckIfOpCanBeSent(252, this.Server, "SetProperties");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = actorProperties == null || actorProperties.Count == 0;
				if (flag2)
				{
					this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfActor() failed. actorProperties must not be null nor empty.");
					result = false;
				}
				else
				{
					bool flag3 = this.LoadBalancingPeer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties, webFlags);
					bool flag4 = flag3 && !this.CurrentRoom.BroadcastPropertiesChangeToAll && (expectedProperties == null || expectedProperties.Count == 0);
					if (flag4)
					{
						Player player = this.CurrentRoom.GetPlayer(actorNr, false);
						bool flag5 = player != null;
						if (flag5)
						{
							player.InternalCacheProperties(actorProperties);
							this.InRoomCallbackTargets.OnPlayerPropertiesUpdate(player, actorProperties);
						}
					}
					result = flag3;
				}
			}
			return result;
		}

		public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			bool flag = propertiesToSet == null || propertiesToSet.Count == 0;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfRoom() failed. propertiesToSet must not be null nor empty.");
				result = false;
			}
			else
			{
				Hashtable hashtable = new Hashtable();
				hashtable.MergeStringKeys(propertiesToSet);
				bool flag2 = hashtable.Count == 0;
				if (flag2)
				{
					this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfRoom() failed. Only string keys are allowed for custom properties.");
					result = false;
				}
				else
				{
					result = this.OpSetPropertiesOfRoom(hashtable, expectedProperties, webFlags);
				}
			}
			return result;
		}

		protected internal bool OpSetPropertyOfRoom(byte propCode, object value)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[propCode] = value;
			return this.OpSetPropertiesOfRoom(hashtable, null, null);
		}

		protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			bool flag = !this.CheckIfOpCanBeSent(252, this.Server, "SetProperties");
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = gameProperties == null || gameProperties.Count == 0;
				if (flag2)
				{
					this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfRoom() failed. gameProperties must not be null nor empty.");
					result = false;
				}
				else
				{
					bool flag3 = this.LoadBalancingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, webFlags);
					bool flag4 = flag3 && !this.CurrentRoom.BroadcastPropertiesChangeToAll && (expectedProperties == null || expectedProperties.Count == 0);
					if (flag4)
					{
						this.CurrentRoom.InternalCacheProperties(gameProperties);
						this.InRoomCallbackTargets.OnRoomPropertiesUpdate(gameProperties);
					}
					result = flag3;
				}
			}
			return result;
		}

		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
		{
			bool flag = !this.CheckIfOpCanBeSent(253, this.Server, "RaiseEvent");
			return !flag && this.LoadBalancingPeer.OpRaiseEvent(eventCode, customEventContent, raiseEventOptions, sendOptions);
		}

		public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
		{
			bool flag = !this.CheckIfOpCanBeSent(248, this.Server, "ChangeGroups");
			return !flag && this.LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
		}

		private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
		{
			bool flag = this.CurrentRoom != null && gameProperties != null;
			if (flag)
			{
				this.CurrentRoom.InternalCacheProperties(gameProperties);
				bool inRoom = this.InRoom;
				if (inRoom)
				{
					this.InRoomCallbackTargets.OnRoomPropertiesUpdate(gameProperties);
				}
			}
			bool flag2 = actorProperties != null && actorProperties.Count > 0;
			if (flag2)
			{
				bool flag3 = targetActorNr > 0;
				if (flag3)
				{
					Player player = this.CurrentRoom.GetPlayer(targetActorNr, false);
					bool flag4 = player != null;
					if (flag4)
					{
						Hashtable hashtable = this.ReadoutPropertiesForActorNr(actorProperties, targetActorNr);
						player.InternalCacheProperties(hashtable);
						this.InRoomCallbackTargets.OnPlayerPropertiesUpdate(player, hashtable);
					}
				}
				else
				{
					foreach (object obj in actorProperties.Keys)
					{
						int num = (int)obj;
						bool flag5 = num == 0;
						if (!flag5)
						{
							Hashtable hashtable2 = (Hashtable)actorProperties[obj];
							string actorName = (string)hashtable2[byte.MaxValue];
							Player player2 = this.CurrentRoom.GetPlayer(num, false);
							bool flag6 = player2 == null;
							if (flag6)
							{
								player2 = this.CreatePlayer(actorName, num, false, hashtable2);
								this.CurrentRoom.StorePlayer(player2);
							}
							player2.InternalCacheProperties(hashtable2);
						}
					}
				}
			}
		}

		private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
		{
			bool flag = actorProperties.ContainsKey(actorNr);
			Hashtable result;
			if (flag)
			{
				result = (Hashtable)actorProperties[actorNr];
			}
			else
			{
				result = actorProperties;
			}
			return result;
		}

		public void ChangeLocalID(int newId, bool applyUserId = false)
		{
			bool flag = this.LocalPlayer == null;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, "loadBalancingClient.LocalPlayer is null. It should be set in constructor and not changed. Failed to ChangeLocalID.");
			}
			else
			{
				bool flag2 = applyUserId && string.IsNullOrEmpty(this.LocalPlayer.UserId);
				if (flag2)
				{
					this.LocalPlayer.UserId = ((this.AuthValues == null || string.IsNullOrEmpty(this.AuthValues.UserId)) ? default(Guid).ToString() : this.AuthValues.UserId);
				}
				bool flag3 = this.CurrentRoom == null;
				if (flag3)
				{
					this.LocalPlayer.ChangeLocalID(newId);
					this.LocalPlayer.RoomReference = null;
				}
				else
				{
					this.CurrentRoom.RemovePlayer(this.LocalPlayer);
					this.LocalPlayer.ChangeLocalID(newId);
					this.CurrentRoom.StorePlayer(this.LocalPlayer);
				}
			}
		}

		private void GameEnteredOnGameServer(OperationResponse operationResponse)
		{
			this.CurrentRoom = this.CreateRoom(this.enterRoomParamsCache.RoomName, this.enterRoomParamsCache.RoomOptions);
			this.CurrentRoom.LoadBalancingClient = this;
			int newId = (int)operationResponse[254];
			this.ChangeLocalID(newId, false);
			bool flag = operationResponse.Parameters.ContainsKey(252);
			if (flag)
			{
				int[] actorsInGame = (int[])operationResponse.Parameters[252];
				this.UpdatedActorList(actorsInGame);
			}
			Hashtable actorProperties = (Hashtable)operationResponse[249];
			Hashtable gameProperties = (Hashtable)operationResponse[248];
			this.ReadoutProperties(gameProperties, actorProperties, 0);
			object obj;
			bool flag2 = operationResponse.Parameters.TryGetValue(191, out obj);
			if (flag2)
			{
				this.CurrentRoom.InternalCacheRoomFlags((int)obj);
			}
			bool suppressRoomEvents = this.CurrentRoom.SuppressRoomEvents;
			if (suppressRoomEvents)
			{
				this.State = ClientState.Joined;
				this.LocalPlayer.UpdateNickNameOnJoined();
				bool flag3 = this.lastJoinType == JoinType.CreateRoom || (this.lastJoinType == JoinType.JoinOrCreateRoom && this.LocalPlayer.ActorNumber == 1);
				if (flag3)
				{
					this.MatchMakingCallbackTargets.OnCreatedRoom();
				}
				this.MatchMakingCallbackTargets.OnJoinedRoom();
			}
		}

		private void UpdatedActorList(int[] actorsInGame)
		{
			bool flag = actorsInGame != null;
			if (flag)
			{
				foreach (int num in actorsInGame)
				{
					bool flag2 = num == 0;
					if (!flag2)
					{
						Player player = this.CurrentRoom.GetPlayer(num, false);
						bool flag3 = player == null;
						if (flag3)
						{
							this.CurrentRoom.StorePlayer(this.CreatePlayer(string.Empty, num, false, null));
						}
					}
				}
			}
		}

		protected internal virtual Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
		{
			return new Player(actorName, actorNumber, isLocal, actorProperties);
		}

		protected internal virtual Room CreateRoom(string roomName, RoomOptions opt)
		{
			return new Room(roomName, opt, false);
		}

		private bool CheckIfOpAllowedOnServer(byte opCode, ServerConnection serverConnection)
		{
			switch (serverConnection)
			{
			case ServerConnection.MasterServer:
				switch (opCode)
				{
				case 217:
				case 218:
				case 219:
				case 221:
				case 222:
				case 225:
				case 226:
				case 227:
				case 228:
				case 229:
				case 230:
				case 231:
					return true;
				}
				break;
			case ServerConnection.GameServer:
				if (opCode <= 227)
				{
					if (opCode - 218 > 1 && opCode - 226 > 1)
					{
						break;
					}
				}
				else if (opCode - 230 > 1 && opCode != 248 && opCode - 251 > 3)
				{
					break;
				}
				return true;
			case ServerConnection.NameServer:
				if (opCode == 218 || opCode == 220 || opCode - 230 <= 1)
				{
					return true;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("serverConnection", serverConnection, null);
			}
			return false;
		}

		private bool CheckIfOpCanBeSent(byte opCode, ServerConnection serverConnection, string opName)
		{
			bool flag = this.LoadBalancingPeer == null;
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) can't be sent because peer is null", opName, opCode));
				result = false;
			}
			else
			{
				bool flag2 = !this.CheckIfOpAllowedOnServer(opCode, serverConnection);
				if (flag2)
				{
					bool flag3 = this.LoadBalancingPeer.DebugOut >= DebugLevel.ERROR;
					if (flag3)
					{
						this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) not allowed on current server ({2})", opName, opCode, serverConnection));
					}
					result = false;
				}
				else
				{
					bool flag4 = !this.CheckIfClientIsReadyToCallOperation(opCode);
					if (flag4)
					{
						DebugLevel debugLevel = DebugLevel.ERROR;
						bool flag5 = opCode == 253 && (this.State == ClientState.Leaving || this.State == ClientState.Disconnecting || this.State == ClientState.DisconnectingFromGameServer);
						if (flag5)
						{
							debugLevel = DebugLevel.INFO;
						}
						bool flag6 = this.LoadBalancingPeer.DebugOut >= debugLevel;
						if (flag6)
						{
							this.DebugReturn(debugLevel, string.Format("Operation {0} ({1}) not called because client is not connected or not ready yet, client state: {2}", opName, opCode, Enum.GetName(typeof(ClientState), this.State)));
						}
						result = false;
					}
					else
					{
						bool flag7 = this.LoadBalancingPeer.PeerState != PeerStateValue.Connected;
						if (flag7)
						{
							this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) can't be sent because peer is not connected, peer state: {2}", opName, opCode, this.LoadBalancingPeer.PeerState));
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		private bool CheckIfClientIsReadyToCallOperation(byte opCode)
		{
			switch (opCode)
			{
			case 217:
			case 221:
			case 222:
			case 225:
			case 229:
				return this.State == ClientState.ConnectedToMasterServer || this.InLobby;
			case 218:
			case 219:
			case 223:
			case 224:
				break;
			case 220:
				return this.State == ClientState.ConnectedToNameServer;
			case 226:
			case 227:
				return this.State == ClientState.ConnectedToMasterServer || this.InLobby || this.State == ClientState.ConnectedToGameServer;
			case 228:
				return this.InLobby;
			case 230:
			case 231:
				return this.IsConnectedAndReady || this.State == ClientState.ConnectingToNameServer || this.State == ClientState.ConnectingToMasterServer || this.State == ClientState.ConnectingToGameServer;
			default:
				if (opCode == 248 || opCode - 251 <= 3)
				{
					return this.InRoom;
				}
				break;
			}
			return this.IsConnected;
		}

		public virtual void DebugReturn(DebugLevel level, string message)
		{
			bool flag = this.LoadBalancingPeer.DebugOut != DebugLevel.ALL && level > this.LoadBalancingPeer.DebugOut;
			if (!flag)
			{
				bool flag2 = level == DebugLevel.ERROR;
				if (flag2)
				{
					Debug_.LogError(message);
				}
				else
				{
					bool flag3 = level == DebugLevel.WARNING;
					if (flag3)
					{
						Debug_.LogWarning(message);
					}
					else
					{
						bool flag4 = level == DebugLevel.INFO;
						if (flag4)
						{
							Debug_.Log(message);
						}
						else
						{
							bool flag5 = level == DebugLevel.ALL;
							if (flag5)
							{
								Debug_.Log(message);
							}
						}
					}
				}
			}
		}

		private void CallbackRoomEnterFailed(OperationResponse operationResponse)
		{
			bool flag = operationResponse.ReturnCode != 0;
			if (flag)
			{
				bool flag2 = operationResponse.OperationCode == 226;
				if (flag2)
				{
					this.MatchMakingCallbackTargets.OnJoinRoomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
				}
				else
				{
					bool flag3 = operationResponse.OperationCode == 227;
					if (flag3)
					{
						this.MatchMakingCallbackTargets.OnCreateRoomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
					}
					else
					{
						bool flag4 = operationResponse.OperationCode == 225;
						if (flag4)
						{
							this.MatchMakingCallbackTargets.OnJoinRandomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
						}
					}
				}
			}
		}

		public virtual void OnOperationResponse(OperationResponse operationResponse)
		{
			bool flag = operationResponse.Parameters.ContainsKey(221);
			if (flag)
			{
				bool flag2 = this.AuthValues == null;
				if (flag2)
				{
					this.AuthValues = new AuthenticationValues();
				}
				this.AuthValues.Token = operationResponse.Parameters[221];
				this.tokenCache = this.AuthValues.Token;
			}
			bool flag3 = operationResponse.ReturnCode == 32743;
			if (flag3)
			{
				this.Disconnect(DisconnectCause.DisconnectByOperationLimit);
			}
			byte operationCode = operationResponse.OperationCode;
			byte b = operationCode;
			switch (b)
			{
			case 217:
			{
				bool flag4 = operationResponse.ReturnCode != 0;
				if (flag4)
				{
					this.DebugReturn(DebugLevel.ERROR, "GetGameList failed: " + operationResponse.ToStringFull());
				}
				else
				{
					List<RoomInfo> list = new List<RoomInfo>();
					Hashtable hashtable = (Hashtable)operationResponse[222];
					foreach (object obj in hashtable.Keys)
					{
						string text = (string)obj;
						list.Add(new RoomInfo(text, (Hashtable)hashtable[text]));
					}
					this.LobbyCallbackTargets.OnRoomListUpdate(list);
				}
				break;
			}
			case 218:
			case 221:
			case 223:
			case 224:
				break;
			case 219:
				this.WebRpcCallbackTargets.OnWebRpcResponse(operationResponse);
				break;
			case 220:
			{
				bool flag5 = operationResponse.ReturnCode == short.MaxValue;
				if (flag5)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("GetRegions failed. AppId is unknown on the (cloud) server. " + operationResponse.DebugMessage, Array.Empty<object>()));
					this.Disconnect(DisconnectCause.InvalidAuthentication);
				}
				else
				{
					bool flag6 = operationResponse.ReturnCode != 0;
					if (flag6)
					{
						this.DebugReturn(DebugLevel.ERROR, "GetRegions failed. Can't provide regions list. ReturnCode: " + operationResponse.ReturnCode.ToString() + ": " + operationResponse.DebugMessage);
						this.Disconnect(DisconnectCause.InvalidAuthentication);
					}
					else
					{
						bool flag7 = this.RegionHandler == null;
						if (flag7)
						{
							this.RegionHandler = new RegionHandler(this.ServerPortOverrides.MasterServerPort);
						}
						bool isPinging = this.RegionHandler.IsPinging;
						if (isPinging)
						{
							this.DebugReturn(DebugLevel.WARNING, "Received an response for OpGetRegions while the RegionHandler is pinging regions already. Skipping this response in favor of completing the current region-pinging.");
							return;
						}
						this.RegionHandler.SetRegions(operationResponse, this);
						this.ConnectionCallbackTargets.OnRegionListReceived(this.RegionHandler);
						bool flag8 = this.connectToBestRegion;
						if (flag8)
						{
							this.RegionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), this.bestRegionSummaryFromStorage);
						}
					}
				}
				break;
			}
			case 222:
			{
				bool flag9 = operationResponse.ReturnCode != 0;
				if (flag9)
				{
					this.DebugReturn(DebugLevel.ERROR, "OpFindFriends failed: " + operationResponse.ToStringFull());
					this.friendListRequested = null;
				}
				else
				{
					bool[] array = operationResponse[1] as bool[];
					string[] array2 = operationResponse[2] as string[];
					List<FriendInfo> list2 = new List<FriendInfo>(this.friendListRequested.Length);
					for (int i = 0; i < this.friendListRequested.Length; i++)
					{
						list2.Insert(i, new FriendInfo
						{
							UserId = this.friendListRequested[i],
							Room = array2[i],
							IsOnline = array[i]
						});
					}
					this.friendListRequested = null;
					this.MatchMakingCallbackTargets.OnFriendListUpdate(list2);
				}
				break;
			}
			case 225:
			case 226:
			case 227:
			{
				bool flag10 = operationResponse.ReturnCode != 0;
				if (flag10)
				{
					bool flag11 = this.Server == ServerConnection.GameServer;
					if (flag11)
					{
						this.failedRoomEntryOperation = operationResponse;
						this.DisconnectToReconnect();
					}
					else
					{
						this.State = (this.InLobby ? ClientState.JoinedLobby : ClientState.ConnectedToMasterServer);
						this.CallbackRoomEnterFailed(operationResponse);
					}
				}
				else
				{
					bool flag12 = this.Server == ServerConnection.GameServer;
					if (flag12)
					{
						this.GameEnteredOnGameServer(operationResponse);
					}
					else
					{
						this.GameServerAddress = (string)operationResponse[230];
						bool flag13 = this.ServerPortOverrides.GameServerPort > 0;
						if (flag13)
						{
							this.GameServerAddress = LoadBalancingClient.ReplacePortWithAlternative(this.GameServerAddress, this.ServerPortOverrides.GameServerPort);
						}
						bool flag14 = this.AddressRewriter != null;
						if (flag14)
						{
							this.GameServerAddress = this.AddressRewriter(this.GameServerAddress, ServerConnection.GameServer);
						}
						string text2 = operationResponse[byte.MaxValue] as string;
						bool flag15 = !string.IsNullOrEmpty(text2);
						if (flag15)
						{
							this.enterRoomParamsCache.RoomName = text2;
						}
						this.DisconnectToReconnect();
					}
				}
				break;
			}
			case 228:
				this.State = ClientState.ConnectedToMasterServer;
				this.LobbyCallbackTargets.OnLeftLobby();
				break;
			case 229:
				this.State = ClientState.JoinedLobby;
				this.LobbyCallbackTargets.OnJoinedLobby();
				break;
			case 230:
			case 231:
			{
				bool flag16 = operationResponse.Parameters.ContainsKey(187);
				if (flag16)
				{
					this.TelemetryEnabled = (bool)operationResponse[187];
				}
				bool flag17 = operationResponse.ReturnCode != 0;
				if (flag17)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Concat(new string[]
					{
						operationResponse.ToStringFull(),
						" Server: ",
						this.Server.ToString(),
						" Address: ",
						this.LoadBalancingPeer.ServerAddress
					}));
					short returnCode = operationResponse.ReturnCode;
					short num = returnCode;
					if (num - -3 > 1)
					{
						switch (num)
						{
						case 32753:
							this.DisconnectedCause = DisconnectCause.AuthenticationTicketExpired;
							break;
						case 32754:
							break;
						case 32755:
							this.DisconnectedCause = DisconnectCause.CustomAuthenticationFailed;
							this.ConnectionCallbackTargets.OnCustomAuthenticationFailed(operationResponse.DebugMessage);
							break;
						case 32756:
							this.DisconnectedCause = DisconnectCause.InvalidRegion;
							break;
						case 32757:
							this.DisconnectedCause = DisconnectCause.MaxCcuReached;
							break;
						default:
							if (num == 32767)
							{
								this.DisconnectedCause = DisconnectCause.InvalidAuthentication;
							}
							break;
						}
					}
					else
					{
						this.DisconnectedCause = DisconnectCause.OperationNotAllowedInCurrentState;
					}
					this.DisconnectMessage = string.Format("Op: {0} ReturnCode: {1} '{2}'", operationResponse.OperationCode, operationResponse.ReturnCode, operationResponse.DebugMessage);
					this.Disconnect(this.DisconnectedCause);
				}
				else
				{
					bool flag18 = this.Server == ServerConnection.NameServer || this.Server == ServerConnection.MasterServer;
					if (flag18)
					{
						bool flag19 = operationResponse.Parameters.ContainsKey(225);
						if (flag19)
						{
							string text3 = (string)operationResponse.Parameters[225];
							bool flag20 = !string.IsNullOrEmpty(text3);
							if (flag20)
							{
								this.UserId = text3;
								this.LocalPlayer.UserId = text3;
								this.DebugReturn(DebugLevel.INFO, string.Format("Received your UserID from server. Updating local value to: {0}", this.UserId));
							}
						}
						bool flag21 = operationResponse.Parameters.ContainsKey(202);
						if (flag21)
						{
							this.NickName = (string)operationResponse.Parameters[202];
							this.DebugReturn(DebugLevel.INFO, string.Format("Received your NickName from server. Updating local value to: {0}", this.NickName));
						}
						bool flag22 = operationResponse.Parameters.ContainsKey(192);
						if (flag22)
						{
							this.SetupEncryption((Dictionary<byte, object>)operationResponse.Parameters[192]);
						}
					}
					bool flag23 = this.Server == ServerConnection.NameServer;
					if (flag23)
					{
						bool flag24 = this.AuthMode == AuthModeOption.AuthOnceWss && this.ExpectedProtocol != null;
						if (flag24)
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("AuthOnceWss mode. Auth response switches TransportProtocol to ExpectedProtocol: {0}.", this.ExpectedProtocol));
							this.LoadBalancingPeer.TransportProtocol = this.ExpectedProtocol.Value;
							this.ExpectedProtocol = null;
						}
						string text4 = operationResponse[196] as string;
						bool flag25 = !string.IsNullOrEmpty(text4);
						if (flag25)
						{
							this.CurrentCluster = text4;
						}
						this.MasterServerAddress = (operationResponse[230] as string);
						bool flag26 = this.ServerPortOverrides.MasterServerPort > 0;
						if (flag26)
						{
							this.MasterServerAddress = LoadBalancingClient.ReplacePortWithAlternative(this.MasterServerAddress, this.ServerPortOverrides.MasterServerPort);
						}
						bool flag27 = this.AddressRewriter != null;
						if (flag27)
						{
							this.MasterServerAddress = this.AddressRewriter(this.MasterServerAddress, ServerConnection.MasterServer);
						}
						this.DisconnectToReconnect();
					}
					else
					{
						bool flag28 = this.Server == ServerConnection.MasterServer;
						if (flag28)
						{
							this.State = ClientState.ConnectedToMasterServer;
							bool flag29 = this.failedRoomEntryOperation == null;
							if (flag29)
							{
								this.ConnectionCallbackTargets.OnConnectedToMaster();
							}
							else
							{
								this.CallbackRoomEnterFailed(this.failedRoomEntryOperation);
								this.failedRoomEntryOperation = null;
							}
							bool flag30 = this.AuthMode > AuthModeOption.Auth;
							if (flag30)
							{
								this.LoadBalancingPeer.OpSettings(this.EnableLobbyStatistics);
							}
						}
						else
						{
							bool flag31 = this.Server == ServerConnection.GameServer;
							if (flag31)
							{
								this.State = ClientState.Joining;
								bool flag32 = this.enterRoomParamsCache.JoinMode == JoinMode.RejoinOnly;
								if (flag32)
								{
									this.enterRoomParamsCache.PlayerProperties = null;
								}
								else
								{
									Hashtable hashtable2 = new Hashtable();
									hashtable2.Merge(this.LocalPlayer.CustomProperties);
									bool flag33 = !string.IsNullOrEmpty(this.LocalPlayer.NickName);
									if (flag33)
									{
										hashtable2[byte.MaxValue] = this.LocalPlayer.NickName;
									}
									this.enterRoomParamsCache.PlayerProperties = hashtable2;
								}
								this.enterRoomParamsCache.OnGameServer = true;
								bool flag34 = this.lastJoinType == JoinType.JoinRoom || this.lastJoinType == JoinType.JoinRandomRoom || this.lastJoinType == JoinType.JoinRandomOrCreateRoom || this.lastJoinType == JoinType.JoinOrCreateRoom;
								if (flag34)
								{
									this.LoadBalancingPeer.OpJoinRoom(this.enterRoomParamsCache);
								}
								else
								{
									bool flag35 = this.lastJoinType == JoinType.CreateRoom;
									if (flag35)
									{
										this.LoadBalancingPeer.OpCreateRoom(this.enterRoomParamsCache);
									}
								}
								break;
							}
						}
					}
					Dictionary<string, object> dictionary = (Dictionary<string, object>)operationResponse[245];
					bool flag36 = dictionary != null;
					if (flag36)
					{
						this.ConnectionCallbackTargets.OnCustomAuthenticationResponse(dictionary);
					}
				}
				break;
			}
			default:
				if (b == 254)
				{
					this.DisconnectToReconnect();
				}
				break;
			}
			bool flag37 = this.OpResponseReceived != null;
			if (flag37)
			{
				this.OpResponseReceived(operationResponse);
			}
		}

		public virtual void OnStatusChanged(StatusCode statusCode)
		{
			switch (statusCode)
			{
			case StatusCode.SecurityExceptionOnConnect:
			case StatusCode.ExceptionOnConnect:
			case StatusCode.EncryptionFailedToEstablish:
			{
				this.SystemConnectionSummary = new SystemConnectionSummary(this);
				this.DebugReturn(DebugLevel.ERROR, string.Format("Connection lost. OnStatusChanged to {0}. Client state was: {1}. {2}", statusCode, this.State, this.SystemConnectionSummary.ToString()));
				this.DisconnectedCause = DisconnectCause.ExceptionOnConnect;
				ClientState clientState = ClientState.Disconnecting;
				bool flag = this.State == ClientState.ConnectingToNameServer;
				if (flag)
				{
					bool flag2 = this.EnableProtocolFallback && this.LoadBalancingPeer.UsedProtocol != ConnectionProtocol.WebSocketSecure;
					if (flag2)
					{
						clientState = ClientState.ConnectWithFallbackProtocol;
					}
				}
				this.State = clientState;
				return;
			}
			case StatusCode.Connect:
			{
				int connectCount = this.ConnectCount;
				this.ConnectCount = connectCount + 1;
				this.telemetrySent = false;
				bool flag3 = this.State == ClientState.ConnectingToNameServer;
				if (flag3)
				{
					bool flag4 = this.LoadBalancingPeer.DebugOut >= DebugLevel.ALL;
					if (flag4)
					{
						this.DebugReturn(DebugLevel.ALL, "Connected to nameserver.");
					}
					this.Server = ServerConnection.NameServer;
					bool flag5 = this.AuthValues != null;
					if (flag5)
					{
						this.AuthValues.Token = null;
					}
				}
				bool flag6 = this.State == ClientState.ConnectingToGameServer;
				if (flag6)
				{
					bool flag7 = this.LoadBalancingPeer.DebugOut >= DebugLevel.ALL;
					if (flag7)
					{
						this.DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
					}
					this.Server = ServerConnection.GameServer;
				}
				bool flag8 = this.State == ClientState.ConnectingToMasterServer;
				if (flag8)
				{
					bool flag9 = this.LoadBalancingPeer.DebugOut >= DebugLevel.ALL;
					if (flag9)
					{
						this.DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
					}
					this.Server = ServerConnection.MasterServer;
					this.ConnectionCallbackTargets.OnConnected();
				}
				bool flag10 = this.LoadBalancingPeer.TransportProtocol != ConnectionProtocol.WebSocketSecure;
				if (flag10)
				{
					bool flag11 = this.Server == ServerConnection.NameServer || this.AuthMode == AuthModeOption.Auth;
					if (flag11)
					{
						this.LoadBalancingPeer.EstablishEncryption();
					}
					return;
				}
				break;
			}
			case StatusCode.Disconnect:
			{
				this.friendListRequested = null;
				bool flag12 = this.CurrentRoom != null;
				this.CurrentRoom = null;
				this.ChangeLocalID(-1, false);
				bool flag13 = this.Server == ServerConnection.GameServer && flag12;
				if (flag13)
				{
					this.MatchMakingCallbackTargets.OnLeftRoom();
				}
				bool flag14;
				if (this.ExpectedProtocol != null)
				{
					ConnectionProtocol transportProtocol = this.LoadBalancingPeer.TransportProtocol;
					ConnectionProtocol? expectedProtocol = this.ExpectedProtocol;
					flag14 = !(transportProtocol == expectedProtocol.GetValueOrDefault() & expectedProtocol != null);
				}
				else
				{
					flag14 = false;
				}
				bool flag15 = flag14;
				if (flag15)
				{
					this.DebugReturn(DebugLevel.INFO, string.Format("On disconnect switches TransportProtocol to ExpectedProtocol: {0}.", this.ExpectedProtocol));
					this.LoadBalancingPeer.TransportProtocol = this.ExpectedProtocol.Value;
					this.ExpectedProtocol = null;
				}
				ClientState clientState2 = this.State;
				ClientState clientState3 = clientState2;
				if (clientState3 != ClientState.PeerCreated)
				{
					if (clientState3 != ClientState.DisconnectingFromMasterServer)
					{
						switch (clientState3)
						{
						case ClientState.DisconnectingFromGameServer:
						case ClientState.DisconnectingFromNameServer:
							this.ConnectToMasterServer();
							goto IL_523;
						case ClientState.Disconnecting:
							goto IL_40C;
						case ClientState.Disconnected:
							goto IL_523;
						case ClientState.ConnectWithFallbackProtocol:
						{
							this.EnableProtocolFallback = false;
							this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
							this.NameServerPortInAppSettings = 0;
							this.ServerPortOverrides = default(PhotonPortDefinition);
							bool flag16 = !this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null);
							if (flag16)
							{
								return;
							}
							this.State = ClientState.ConnectingToNameServer;
							goto IL_523;
						}
						}
						string text = new StackTrace(true).ToString();
						this.DebugReturn(DebugLevel.WARNING, string.Concat(new string[]
						{
							"Got a unexpected Disconnect in LoadBalancingClient State: ",
							this.State.ToString(),
							". Server: ",
							this.Server.ToString(),
							" Trace: ",
							text
						}));
						bool flag17 = this.AuthValues != null;
						if (flag17)
						{
							this.AuthValues.Token = null;
						}
						this.State = ClientState.Disconnected;
						this.ConnectionCallbackTargets.OnDisconnected(this.DisconnectedCause);
						goto IL_523;
					}
					this.Connect(this.GameServerAddress, this.ProxyServerAddress, ServerConnection.GameServer);
					goto IL_523;
				}
				IL_40C:
				bool flag18 = this.AuthValues != null;
				if (flag18)
				{
					this.AuthValues.Token = null;
				}
				this.State = ClientState.Disconnected;
				this.ConnectionCallbackTargets.OnDisconnected(this.DisconnectedCause);
				IL_523:
				return;
			}
			case StatusCode.Exception:
			case StatusCode.SendError:
			case StatusCode.ExceptionOnReceive:
				this.SystemConnectionSummary = new SystemConnectionSummary(this);
				this.DebugReturn(DebugLevel.ERROR, string.Format("Connection lost. OnStatusChanged to {0}. Client state was: {1}. {2}", statusCode, this.State, this.SystemConnectionSummary.ToString()));
				this.DisconnectedCause = DisconnectCause.Exception;
				this.State = ClientState.Disconnecting;
				return;
			case (StatusCode)1027:
			case (StatusCode)1028:
			case (StatusCode)1029:
			case (StatusCode)1031:
			case (StatusCode)1032:
			case (StatusCode)1033:
			case (StatusCode)1034:
			case (StatusCode)1035:
			case (StatusCode)1036:
			case (StatusCode)1037:
			case (StatusCode)1038:
			case (StatusCode)1045:
			case (StatusCode)1046:
			case (StatusCode)1047:
				return;
			case StatusCode.TimeoutDisconnect:
			{
				this.SystemConnectionSummary = new SystemConnectionSummary(this);
				this.DebugReturn(DebugLevel.ERROR, string.Format("Connection lost. OnStatusChanged to {0}. Client state was: {1}. {2}", statusCode, this.State, this.SystemConnectionSummary.ToString()));
				this.DisconnectedCause = DisconnectCause.ClientTimeout;
				ClientState clientState = ClientState.Disconnecting;
				bool flag19 = this.State == ClientState.ConnectingToNameServer;
				if (flag19)
				{
					bool flag20 = this.EnableProtocolFallback && this.LoadBalancingPeer.UsedProtocol != ConnectionProtocol.WebSocketSecure;
					if (flag20)
					{
						clientState = ClientState.ConnectWithFallbackProtocol;
					}
				}
				this.State = clientState;
				return;
			}
			case StatusCode.DisconnectByServerTimeout:
				this.SystemConnectionSummary = new SystemConnectionSummary(this);
				this.DebugReturn(DebugLevel.ERROR, string.Format("Connection lost. OnStatusChanged to {0}. Client state was: {1}. {2}", statusCode, this.State, this.SystemConnectionSummary.ToString()));
				this.DisconnectedCause = DisconnectCause.ServerTimeout;
				this.State = ClientState.Disconnecting;
				return;
			case StatusCode.DisconnectByServerUserLimit:
				this.DebugReturn(DebugLevel.ERROR, "This connection was rejected due to the apps CCU limit.");
				this.DisconnectedCause = DisconnectCause.MaxCcuReached;
				this.State = ClientState.Disconnecting;
				return;
			case StatusCode.DisconnectByServerLogic:
				this.DisconnectedCause = DisconnectCause.DisconnectByServerLogic;
				this.State = ClientState.Disconnecting;
				return;
			case StatusCode.DisconnectByServerReasonUnknown:
				this.DisconnectedCause = DisconnectCause.DisconnectByServerReasonUnknown;
				this.State = ClientState.Disconnecting;
				return;
			case StatusCode.EncryptionEstablished:
				break;
			case StatusCode.ServerAddressInvalid:
				this.DisconnectedCause = DisconnectCause.ServerAddressInvalid;
				this.State = ClientState.Disconnecting;
				return;
			case StatusCode.DnsExceptionOnConnect:
				this.DisconnectedCause = DisconnectCause.DnsExceptionOnConnect;
				this.State = ClientState.Disconnecting;
				return;
			default:
				return;
			}
			bool flag21 = this.Server == ServerConnection.NameServer;
			if (flag21)
			{
				this.State = ClientState.ConnectedToNameServer;
				bool flag22 = string.IsNullOrEmpty(this.CloudRegion);
				if (flag22)
				{
					this.OpGetRegions();
					return;
				}
			}
			else
			{
				bool flag23 = this.AuthMode == AuthModeOption.AuthOnce || this.AuthMode == AuthModeOption.AuthOnceWss;
				if (flag23)
				{
					return;
				}
			}
			bool flag24 = this.CallAuthenticate();
			bool flag25 = flag24;
			if (flag25)
			{
				this.State = ClientState.Authenticating;
			}
			else
			{
				this.DebugReturn(DebugLevel.ERROR, "OpAuthenticate failed. Check log output and AuthValues. State: " + this.State.ToString());
			}
		}

		public virtual void OnEvent(EventData photonEvent)
		{
			int sender = photonEvent.Sender;
			Player player = (this.CurrentRoom != null) ? this.CurrentRoom.GetPlayer(sender, false) : null;
			byte code = photonEvent.Code;
			byte b = code;
			switch (b)
			{
			case 223:
			{
				bool flag = this.AuthValues == null;
				if (flag)
				{
					this.AuthValues = new AuthenticationValues();
				}
				this.AuthValues.Token = photonEvent[221];
				this.tokenCache = this.AuthValues.Token;
				break;
			}
			case 224:
			{
				string[] array = photonEvent[213] as string[];
				int[] array2 = photonEvent[229] as int[];
				int[] array3 = photonEvent[228] as int[];
				ByteArraySlice byteArraySlice = photonEvent[212] as ByteArraySlice;
				bool flag2 = byteArraySlice != null;
				bool flag3 = flag2;
				byte[] array4;
				if (flag3)
				{
					array4 = byteArraySlice.Buffer;
				}
				else
				{
					array4 = (photonEvent[212] as byte[]);
				}
				this.lobbyStatistics.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					TypedLobbyInfo typedLobbyInfo = new TypedLobbyInfo();
					typedLobbyInfo.Name = array[i];
					typedLobbyInfo.Type = (LobbyType)array4[i];
					typedLobbyInfo.PlayerCount = array2[i];
					typedLobbyInfo.RoomCount = array3[i];
					this.lobbyStatistics.Add(typedLobbyInfo);
				}
				bool flag4 = flag2;
				if (flag4)
				{
					byteArraySlice.Release();
				}
				this.LobbyCallbackTargets.OnLobbyStatisticsUpdate(this.lobbyStatistics);
				break;
			}
			case 225:
			case 227:
			case 228:
				break;
			case 226:
				this.PlayersInRoomsCount = (int)photonEvent[229];
				this.RoomsCount = (int)photonEvent[228];
				this.PlayersOnMasterCount = (int)photonEvent[227];
				break;
			case 229:
			case 230:
			{
				List<RoomInfo> list = new List<RoomInfo>();
				Hashtable hashtable = (Hashtable)photonEvent[222];
				foreach (object obj in hashtable.Keys)
				{
					string text = (string)obj;
					list.Add(new RoomInfo(text, (Hashtable)hashtable[text]));
				}
				this.LobbyCallbackTargets.OnRoomListUpdate(list);
				break;
			}
			default:
				switch (b)
				{
				case 251:
					this.ErrorInfoCallbackTargets.OnErrorInfo(new ErrorInfo(photonEvent));
					break;
				case 253:
				{
					int num = 0;
					bool flag5 = photonEvent.Parameters.ContainsKey(253);
					if (flag5)
					{
						num = (int)photonEvent[253];
					}
					Hashtable gameProperties = null;
					Hashtable actorProperties = null;
					bool flag6 = num == 0;
					if (flag6)
					{
						gameProperties = (Hashtable)photonEvent[251];
					}
					else
					{
						actorProperties = (Hashtable)photonEvent[251];
					}
					this.ReadoutProperties(gameProperties, actorProperties, num);
					break;
				}
				case 254:
				{
					bool flag7 = player != null;
					if (flag7)
					{
						bool flag8 = false;
						bool flag9 = photonEvent.Parameters.ContainsKey(233);
						if (flag9)
						{
							flag8 = (bool)photonEvent.Parameters[233];
						}
						player.IsInactive = flag8;
						player.HasRejoined = false;
						bool flag10 = !flag8;
						if (flag10)
						{
							this.CurrentRoom.RemovePlayer(sender);
						}
					}
					bool flag11 = photonEvent.Parameters.ContainsKey(203);
					if (flag11)
					{
						int num2 = (int)photonEvent[203];
						bool flag12 = num2 != 0;
						if (flag12)
						{
							this.CurrentRoom.masterClientId = num2;
							this.InRoomCallbackTargets.OnMasterClientSwitched(this.CurrentRoom.GetPlayer(num2, false));
						}
					}
					this.InRoomCallbackTargets.OnPlayerLeftRoom(player);
					break;
				}
				case 255:
				{
					Hashtable hashtable2 = (Hashtable)photonEvent[249];
					bool flag13 = player == null;
					if (flag13)
					{
						bool flag14 = sender > 0;
						if (flag14)
						{
							player = this.CreatePlayer(string.Empty, sender, false, hashtable2);
							this.CurrentRoom.StorePlayer(player);
						}
					}
					else
					{
						player.InternalCacheProperties(hashtable2);
						player.IsInactive = false;
						player.HasRejoined = (sender != this.LocalPlayer.ActorNumber);
					}
					bool flag15 = sender == this.LocalPlayer.ActorNumber;
					if (flag15)
					{
						int[] actorsInGame = (int[])photonEvent[252];
						this.UpdatedActorList(actorsInGame);
						player.HasRejoined = (this.enterRoomParamsCache != null && this.enterRoomParamsCache.JoinMode == JoinMode.RejoinOnly);
						this.State = ClientState.Joined;
						this.LocalPlayer.UpdateNickNameOnJoined();
						bool flag16 = this.lastJoinType == JoinType.CreateRoom || (this.lastJoinType == JoinType.JoinOrCreateRoom && this.LocalPlayer.ActorNumber == 1);
						if (flag16)
						{
							this.MatchMakingCallbackTargets.OnCreatedRoom();
						}
						this.MatchMakingCallbackTargets.OnJoinedRoom();
					}
					else
					{
						this.InRoomCallbackTargets.OnPlayerEnteredRoom(player);
					}
					break;
				}
				}
				break;
			}
			this.UpdateCallbackTargets();
			bool flag17 = this.EventReceived != null;
			if (flag17)
			{
				this.EventReceived(photonEvent);
			}
		}

		public virtual void OnMessage(object message)
		{
			this.DebugReturn(DebugLevel.ALL, string.Format("got OnMessage {0}", message));
		}

		private void OnDisconnectMessageReceived(DisconnectMessage obj)
		{
			this.DebugReturn(DebugLevel.ERROR, string.Format("Got DisconnectMessage. Code: {0} Msg: \"{1}\". Debug Info: {2}", obj.Code, obj.DebugMessage, obj.Parameters.ToStringFull()));
			this.DisconnectMessage = string.Format("DisconnectMessage {0}: {1}", obj.Code, obj.DebugMessage);
			this.Disconnect(DisconnectCause.DisconnectByDisconnectMessage);
		}

		private void OnRegionPingCompleted(RegionHandler regionHandler)
		{
			foreach (Region arg in regionHandler.EnabledRegions)
			{
				Debug_.Log(string.Format("OnRegionPingCompleted: {0}", arg));
			}
			Debug_.Log(string.Format("OnRegionPingCompleted: Best Region={0}", regionHandler.BestRegion));
			Debug_.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
			this.SummaryToCache = regionHandler.SummaryToCache;
			this.ConnectToRegionMaster(regionHandler.BestRegion.Code);
		}

		protected internal static string ReplacePortWithAlternative(string address, ushort replacementPort)
		{
			bool flag = string.IsNullOrEmpty(address) || replacementPort == 0;
			string result;
			if (flag)
			{
				result = address;
			}
			else
			{
				bool flag2 = address.StartsWith("ws");
				bool flag3 = flag2;
				if (flag3)
				{
					result = new UriBuilder(address)
					{
						Port = (int)replacementPort
					}.ToString();
				}
				else
				{
					UriBuilder uriBuilder = new UriBuilder("scheme://" + address);
					result = string.Format("{0}:{1}", uriBuilder.Host, replacementPort);
				}
			}
			return result;
		}

		private void SetupEncryption(Dictionary<byte, object> encryptionData)
		{
			EncryptionMode encryptionMode = (EncryptionMode)((byte)encryptionData[0]);
			EncryptionMode encryptionMode2 = encryptionMode;
			EncryptionMode encryptionMode3 = encryptionMode2;
			if (encryptionMode3 != EncryptionMode.PayloadEncryption)
			{
				if (encryptionMode3 != EncryptionMode.DatagramEncryptionGCM)
				{
					throw new ArgumentOutOfRangeException();
				}
				byte[] encryptionSecret = (byte[])encryptionData[1];
				this.LoadBalancingPeer.InitDatagramEncryption(encryptionSecret, null, true, true);
			}
			else
			{
				byte[] secret = (byte[])encryptionData[1];
				this.LoadBalancingPeer.InitPayloadEncryption(secret);
			}
		}

		public bool OpWebRpc(string uriPath, object parameters, bool sendAuthCookie = false)
		{
			bool flag = string.IsNullOrEmpty(uriPath);
			bool result;
			if (flag)
			{
				this.DebugReturn(DebugLevel.ERROR, "WebRPC method name must not be null nor empty.");
				result = false;
			}
			else
			{
				bool flag2 = !this.CheckIfOpCanBeSent(219, this.Server, "WebRpc");
				if (flag2)
				{
					result = false;
				}
				else
				{
					Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
					dictionary.Add(209, uriPath);
					bool flag3 = parameters != null;
					if (flag3)
					{
						dictionary.Add(208, parameters);
					}
					if (sendAuthCookie)
					{
						dictionary.Add(234, 2);
					}
					result = this.LoadBalancingPeer.SendOperation(219, dictionary, SendOptions.SendReliable);
				}
			}
			return result;
		}

		public void AddCallbackTarget(object target)
		{
			this.callbackTargetChanges.Enqueue(new LoadBalancingClient.CallbackTargetChange(target, true));
		}

		public void RemoveCallbackTarget(object target)
		{
			this.callbackTargetChanges.Enqueue(new LoadBalancingClient.CallbackTargetChange(target, false));
		}

		protected internal void UpdateCallbackTargets()
		{
			while (this.callbackTargetChanges.Count > 0)
			{
				LoadBalancingClient.CallbackTargetChange callbackTargetChange = this.callbackTargetChanges.Dequeue();
				bool addTarget = callbackTargetChange.AddTarget;
				if (addTarget)
				{
					bool flag = this.callbackTargets.Contains(callbackTargetChange.Target);
					if (flag)
					{
						continue;
					}
					this.callbackTargets.Add(callbackTargetChange.Target);
				}
				else
				{
					bool flag2 = !this.callbackTargets.Contains(callbackTargetChange.Target);
					if (flag2)
					{
						continue;
					}
					this.callbackTargets.Remove(callbackTargetChange.Target);
				}
				this.UpdateCallbackTarget<IInRoomCallbacks>(callbackTargetChange, this.InRoomCallbackTargets);
				this.UpdateCallbackTarget<IConnectionCallbacks>(callbackTargetChange, this.ConnectionCallbackTargets);
				this.UpdateCallbackTarget<IMatchmakingCallbacks>(callbackTargetChange, this.MatchMakingCallbackTargets);
				this.UpdateCallbackTarget<ILobbyCallbacks>(callbackTargetChange, this.LobbyCallbackTargets);
				this.UpdateCallbackTarget<IWebRpcCallback>(callbackTargetChange, this.WebRpcCallbackTargets);
				this.UpdateCallbackTarget<IErrorInfoCallback>(callbackTargetChange, this.ErrorInfoCallbackTargets);
				IOnEventCallback onEventCallback = callbackTargetChange.Target as IOnEventCallback;
				bool flag3 = onEventCallback != null;
				if (flag3)
				{
					bool addTarget2 = callbackTargetChange.AddTarget;
					if (addTarget2)
					{
						this.EventReceived += onEventCallback.OnEvent;
					}
					else
					{
						this.EventReceived -= onEventCallback.OnEvent;
					}
				}
			}
		}

		private void UpdateCallbackTarget<T>(LoadBalancingClient.CallbackTargetChange change, List<T> container) where T : class
		{
			T t = change.Target as T;
			bool flag = t != null;
			if (flag)
			{
				bool addTarget = change.AddTarget;
				if (addTarget)
				{
					container.Add(t);
				}
				else
				{
					container.Remove(t);
				}
			}
		}

		public AuthModeOption AuthMode = AuthModeOption.Auth;

		public EncryptionMode EncryptionMode = EncryptionMode.PayloadEncryption;

		private object tokenCache;

		public string NameServerHost = "ns.photonengine.io";

		private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
		{
			{
				ConnectionProtocol.Udp,
				5058
			},
			{
				ConnectionProtocol.Tcp,
				4533
			},
			{
				ConnectionProtocol.WebSocket,
				80
			},
			{
				ConnectionProtocol.WebSocketSecure,
				443
			}
		};

		public PhotonPortDefinition ServerPortOverrides;

		public Func<string, ServerConnection, string> AddressRewriter;

		public string ProxyServerAddress;

		private ClientState state = ClientState.PeerCreated;

		public ConnectionCallbacksContainer ConnectionCallbackTargets;

		public MatchMakingCallbacksContainer MatchMakingCallbackTargets;

		internal InRoomCallbacksContainer InRoomCallbackTargets;

		internal LobbyCallbacksContainer LobbyCallbackTargets;

		internal WebRpcCallbacksContainer WebRpcCallbackTargets;

		internal ErrorInfoCallbacksContainer ErrorInfoCallbackTargets;

		public string DisconnectMessage;

		public bool TelemetryEnabled = false;

		private bool telemetrySent = false;

		public SystemConnectionSummary SystemConnectionSummary;

		public bool EnableLobbyStatistics;

		private readonly List<TypedLobbyInfo> lobbyStatistics = new List<TypedLobbyInfo>();

		private JoinType lastJoinType;

		private EnterRoomParams enterRoomParamsCache;

		private OperationResponse failedRoomEntryOperation;

		private const int FriendRequestListMax = 512;

		private string[] friendListRequested;

		public RegionHandler RegionHandler;

		private string bestRegionSummaryFromStorage;

		public string SummaryToCache;

		private bool connectToBestRegion = true;

		private readonly Queue<LoadBalancingClient.CallbackTargetChange> callbackTargetChanges = new Queue<LoadBalancingClient.CallbackTargetChange>();

		private readonly HashSet<object> callbackTargets = new HashSet<object>();

		public int NameServerPortInAppSettings;

		private class EncryptionDataParameters
		{
			public const byte Mode = 0;

			public const byte Secret1 = 1;

			public const byte Secret2 = 2;
		}

		private class CallbackTargetChange
		{
			public CallbackTargetChange(object target, bool addTarget)
			{
				this.Target = target;
				this.AddTarget = addTarget;
			}

			public readonly object Target;

			public readonly bool AddTarget;
		}
	}
}
