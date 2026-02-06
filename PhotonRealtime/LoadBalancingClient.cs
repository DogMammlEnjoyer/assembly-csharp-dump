using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Realtime
{
	public class LoadBalancingClient : IPhotonPeerListener
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
				if (this.AuthMode == AuthModeOption.Auth)
				{
					return null;
				}
				if (this.AuthValues == null)
				{
					return null;
				}
				return this.AuthValues.Token;
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

		public ClientState State
		{
			get
			{
				return this.state;
			}
			set
			{
				if (this.state == value)
				{
					return;
				}
				ClientState arg = this.state;
				this.state = value;
				if (this.StateChanged != null)
				{
					this.StateChanged(arg, this.state);
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
				if (this.LoadBalancingPeer == null)
				{
					return false;
				}
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
				return true;
			}
		}

		public event Action<ClientState, ClientState> StateChanged;

		public event Action<EventData> EventReceived;

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
				if (this.LocalPlayer == null)
				{
					return;
				}
				this.LocalPlayer.NickName = value;
			}
		}

		public string UserId
		{
			get
			{
				if (this.AuthValues != null)
				{
					return this.AuthValues.UserId;
				}
				return null;
			}
			set
			{
				if (this.AuthValues == null)
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
			int num = 0;
			LoadBalancingClient.ProtocolToNameServerPort.TryGetValue(this.LoadBalancingPeer.TransportProtocol, out num);
			if (this.NameServerPortInAppSettings != 0)
			{
				this.DebugReturn(DebugLevel.INFO, string.Format("Using NameServerPortInAppSettings: {0}", this.NameServerPortInAppSettings));
				num = this.NameServerPortInAppSettings;
			}
			if (this.ServerPortOverrides.NameServerPort > 0)
			{
				num = (int)this.ServerPortOverrides.NameServerPort;
			}
			switch (this.LoadBalancingPeer.TransportProtocol)
			{
			case ConnectionProtocol.Udp:
			case ConnectionProtocol.Tcp:
				return string.Format("{0}:{1}", this.NameServerHost, num);
			case ConnectionProtocol.WebSocket:
				return string.Format("ws://{0}:{1}", this.NameServerHost, num);
			case ConnectionProtocol.WebSocketSecure:
				return string.Format("wss://{0}:{1}", this.NameServerHost, num);
			}
			throw new ArgumentOutOfRangeException();
		}

		public virtual bool ConnectUsingSettings(AppSettings appSettings)
		{
			if (this.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				this.DebugReturn(DebugLevel.WARNING, "ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (appSettings == null)
			{
				this.DebugReturn(DebugLevel.ERROR, "ConnectUsingSettings failed. The appSettings can't be null.'");
				return false;
			}
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
			if (appSettings.AuthMode == AuthModeOption.AuthOnceWss)
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
			if (this.IsUsingNameServer)
			{
				this.Server = ServerConnection.NameServer;
				if (!appSettings.IsDefaultNameServer)
				{
					this.NameServerHost = appSettings.Server;
				}
				this.ProxyServerAddress = appSettings.ProxyServer;
				this.NameServerPortInAppSettings = appSettings.Port;
				if (!this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null))
				{
					return false;
				}
				this.State = ClientState.ConnectingToNameServer;
			}
			else
			{
				this.Server = ServerConnection.MasterServer;
				int num = appSettings.IsDefaultPort ? 5055 : appSettings.Port;
				this.MasterServerAddress = string.Format("{0}:{1}", appSettings.Server, num);
				if (!this.LoadBalancingPeer.Connect(this.MasterServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null))
				{
					return false;
				}
				this.State = ClientState.ConnectingToMasterServer;
			}
			return true;
		}

		[Obsolete("Use ConnectToMasterServer() instead.")]
		public bool Connect()
		{
			return this.ConnectToMasterServer();
		}

		public virtual bool ConnectToMasterServer()
		{
			if (this.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				this.DebugReturn(DebugLevel.WARNING, "ConnectToMasterServer() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (this.AuthMode != AuthModeOption.Auth && this.TokenForInit == null)
			{
				this.DebugReturn(DebugLevel.ERROR, "Connect() failed. Can't connect to MasterServer with Token == null in AuthMode: " + this.AuthMode.ToString());
				return false;
			}
			if (this.LoadBalancingPeer.Connect(this.MasterServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null))
			{
				this.DisconnectedCause = DisconnectCause.None;
				this.connectToBestRegion = false;
				this.State = ClientState.ConnectingToMasterServer;
				this.Server = ServerConnection.MasterServer;
				return true;
			}
			return false;
		}

		public bool ConnectToNameServer()
		{
			if (this.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				this.DebugReturn(DebugLevel.WARNING, "ConnectToNameServer() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			this.IsUsingNameServer = true;
			this.CloudRegion = null;
			if (this.AuthMode == AuthModeOption.AuthOnceWss)
			{
				if (this.ExpectedProtocol == null)
				{
					this.ExpectedProtocol = new ConnectionProtocol?(this.LoadBalancingPeer.TransportProtocol);
				}
				this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
			}
			if (this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, "NameServer", this.TokenForInit, null))
			{
				this.DisconnectedCause = DisconnectCause.None;
				this.connectToBestRegion = false;
				this.State = ClientState.ConnectingToNameServer;
				this.Server = ServerConnection.NameServer;
				return true;
			}
			return false;
		}

		public bool ConnectToRegionMaster(string region)
		{
			if (string.IsNullOrEmpty(region))
			{
				this.DebugReturn(DebugLevel.ERROR, "ConnectToRegionMaster() failed. The region can not be null or empty.");
				return false;
			}
			this.IsUsingNameServer = true;
			if (this.State == ClientState.Authenticating)
			{
				if (this.LoadBalancingPeer.DebugOut >= DebugLevel.INFO)
				{
					this.DebugReturn(DebugLevel.INFO, "ConnectToRegionMaster() will skip calling authenticate, as the current state is 'Authenticating'. Just wait for the result.");
				}
				return true;
			}
			if (this.State == ClientState.ConnectedToNameServer)
			{
				this.CloudRegion = region;
				bool flag = this.CallAuthenticate();
				if (flag)
				{
					this.State = ClientState.Authenticating;
				}
				return flag;
			}
			this.LoadBalancingPeer.Disconnect();
			if (!string.IsNullOrEmpty(region) && !region.Contains("/"))
			{
				region += "/*";
			}
			this.CloudRegion = region;
			if (this.AuthMode == AuthModeOption.AuthOnceWss)
			{
				if (this.ExpectedProtocol == null)
				{
					this.ExpectedProtocol = new ConnectionProtocol?(this.LoadBalancingPeer.TransportProtocol);
				}
				this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
			}
			this.connectToBestRegion = false;
			this.DisconnectedCause = DisconnectCause.None;
			if (!this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, "NameServer", null, null))
			{
				return false;
			}
			this.State = ClientState.ConnectingToNameServer;
			this.Server = ServerConnection.NameServer;
			return true;
		}

		[Conditional("UNITY_WEBGL")]
		private void CheckConnectSetupWebGl()
		{
		}

		private bool Connect(string serverAddress, string proxyServerAddress, ServerConnection serverType)
		{
			if (this.State == ClientState.Disconnecting)
			{
				this.DebugReturn(DebugLevel.ERROR, "Connect() failed. Can't connect while disconnecting (still). Current state: " + this.State.ToString());
				return false;
			}
			if (this.AuthMode != AuthModeOption.Auth && serverType != ServerConnection.NameServer && this.TokenForInit == null)
			{
				this.DebugReturn(DebugLevel.ERROR, "Connect() failed. Can't connect to " + serverType.ToString() + " with Token == null in AuthMode: " + this.AuthMode.ToString());
				return false;
			}
			bool flag = this.LoadBalancingPeer.Connect(serverAddress, proxyServerAddress, this.AppId, this.TokenForInit, null);
			if (flag)
			{
				this.DisconnectedCause = DisconnectCause.None;
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
			return flag;
		}

		public bool ReconnectToMaster()
		{
			if (this.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (string.IsNullOrEmpty(this.MasterServerAddress))
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() failed. MasterServerAddress is null or empty.");
				return false;
			}
			if (this.tokenCache == null)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() failed. It seems the client doesn't have any previous authentication token to re-connect.");
				return false;
			}
			if (this.AuthValues == null)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() with AuthValues == null is not correct!");
				this.AuthValues = new AuthenticationValues();
			}
			this.AuthValues.Token = this.tokenCache;
			return this.Connect(this.MasterServerAddress, this.ProxyServerAddress, ServerConnection.MasterServer);
		}

		public bool ReconnectAndRejoin()
		{
			if (this.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. Can only connect while in state 'Disconnected'. Current state: " + this.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (string.IsNullOrEmpty(this.GameServerAddress))
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. It seems the client wasn't connected to a game server before (no address).");
				return false;
			}
			if (this.enterRoomParamsCache == null)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. It seems the client doesn't have any previous room to re-join.");
				return false;
			}
			if (this.tokenCache == null)
			{
				this.DebugReturn(DebugLevel.WARNING, "ReconnectAndRejoin() failed. It seems the client doesn't have any previous authentication token to re-connect.");
				return false;
			}
			if (this.AuthValues == null)
			{
				this.AuthValues = new AuthenticationValues();
			}
			this.AuthValues.Token = this.tokenCache;
			if (!string.IsNullOrEmpty(this.GameServerAddress) && this.enterRoomParamsCache != null)
			{
				this.lastJoinType = JoinType.JoinRoom;
				this.enterRoomParamsCache.JoinMode = JoinMode.RejoinOnly;
				return this.Connect(this.GameServerAddress, this.ProxyServerAddress, ServerConnection.GameServer);
			}
			return false;
		}

		public void Disconnect(DisconnectCause cause = DisconnectCause.DisconnectByClientLogic)
		{
			if (this.State == ClientState.Disconnecting || this.State == ClientState.PeerCreated)
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
				return;
			}
			if (this.State != ClientState.Disconnected)
			{
				this.MatchMakingCallbackTargets.OnPreLeavingRoom();
				this.State = ClientState.Disconnecting;
				this.DisconnectedCause = cause;
				this.LoadBalancingPeer.Disconnect();
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
			if (simulateTimeout)
			{
				this.LoadBalancingPeer.NetworkSimulationSettings.IncomingLossPercentage = 100;
				this.LoadBalancingPeer.NetworkSimulationSettings.OutgoingLossPercentage = 100;
			}
			this.LoadBalancingPeer.IsSimulationEnabled = simulateTimeout;
		}

		private bool CallAuthenticate()
		{
			if (this.IsUsingNameServer && this.Server != ServerConnection.NameServer && (this.AuthValues == null || this.AuthValues.Token == null))
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
				return false;
			}
			if (this.AuthMode == AuthModeOption.Auth)
			{
				return this.CheckIfOpCanBeSent(230, this.Server, "Authenticate") && this.LoadBalancingPeer.OpAuthenticate(this.AppId, this.AppVersion, this.AuthValues, this.CloudRegion, this.EnableLobbyStatistics && this.Server == ServerConnection.MasterServer);
			}
			if (!this.CheckIfOpCanBeSent(231, this.Server, "AuthenticateOnce"))
			{
				return false;
			}
			ConnectionProtocol expectedProtocol = (this.ExpectedProtocol != null) ? this.ExpectedProtocol.Value : this.LoadBalancingPeer.TransportProtocol;
			return this.LoadBalancingPeer.OpAuthenticateOnce(this.AppId, this.AppVersion, this.AuthValues, this.CloudRegion, this.EncryptionMode, expectedProtocol);
		}

		public void Service()
		{
			if (this.LoadBalancingPeer != null)
			{
				this.LoadBalancingPeer.Service();
			}
		}

		private bool OpGetRegions()
		{
			return this.CheckIfOpCanBeSent(220, this.Server, "GetRegions") && this.LoadBalancingPeer.OpGetRegions(this.AppId);
		}

		public bool OpFindFriends(string[] friendsToFind, FindFriendsOptions options = null)
		{
			if (!this.CheckIfOpCanBeSent(222, this.Server, "FindFriends"))
			{
				return false;
			}
			if (this.IsFetchingFriendList)
			{
				this.DebugReturn(DebugLevel.WARNING, "OpFindFriends skipped: already fetching friends list.");
				return false;
			}
			if (friendsToFind == null || friendsToFind.Length == 0)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friendsToFind array is null or empty.");
				return false;
			}
			if (friendsToFind.Length > 512)
			{
				this.DebugReturn(DebugLevel.ERROR, string.Format("OpFindFriends skipped: friendsToFind array exceeds allowed length of {0}.", 512));
				return false;
			}
			List<string> list = new List<string>(friendsToFind.Length);
			for (int i = 0; i < friendsToFind.Length; i++)
			{
				string text = friendsToFind[i];
				if (string.IsNullOrEmpty(text))
				{
					this.DebugReturn(DebugLevel.WARNING, string.Format("friendsToFind array contains a null or empty UserId, element at position {0} skipped.", i));
				}
				else if (text.Equals(this.UserId))
				{
					this.DebugReturn(DebugLevel.WARNING, string.Format("friendsToFind array contains local player's UserId \"{0}\", element at position {1} skipped.", text, i));
				}
				else if (list.Contains(text))
				{
					this.DebugReturn(DebugLevel.WARNING, string.Format("friendsToFind array contains duplicate UserId \"{0}\", element at position {1} skipped.", text, i));
				}
				else
				{
					list.Add(text);
				}
			}
			if (list.Count == 0)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friends list to find is empty.");
				return false;
			}
			string[] array = list.ToArray();
			bool flag = this.LoadBalancingPeer.OpFindFriends(array, options);
			this.friendListRequested = (flag ? array : null);
			return flag;
		}

		public bool OpJoinLobby(TypedLobby lobby)
		{
			if (!this.CheckIfOpCanBeSent(229, this.Server, "JoinLobby"))
			{
				return false;
			}
			if (lobby == null)
			{
				lobby = TypedLobby.Default;
			}
			bool flag = this.LoadBalancingPeer.OpJoinLobby(lobby);
			if (flag)
			{
				this.CurrentLobby = lobby;
				this.State = ClientState.JoiningLobby;
			}
			return flag;
		}

		public bool OpLeaveLobby()
		{
			return this.CheckIfOpCanBeSent(228, this.Server, "LeaveLobby") && this.LoadBalancingPeer.OpLeaveLobby();
		}

		public bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams = null)
		{
			if (!this.CheckIfOpCanBeSent(225, this.Server, "JoinRandomGame"))
			{
				return false;
			}
			if (opJoinRandomRoomParams == null)
			{
				opJoinRandomRoomParams = new OpJoinRandomRoomParams();
			}
			this.enterRoomParamsCache = new EnterRoomParams();
			this.enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;
			this.enterRoomParamsCache.ExpectedUsers = opJoinRandomRoomParams.ExpectedUsers;
			bool flag = this.LoadBalancingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
			if (flag)
			{
				this.lastJoinType = JoinType.JoinRandomRoom;
				this.State = ClientState.Joining;
			}
			return flag;
		}

		public bool OpJoinRandomOrCreateRoom(OpJoinRandomRoomParams opJoinRandomRoomParams, EnterRoomParams createRoomParams)
		{
			if (!this.CheckIfOpCanBeSent(225, this.Server, "OpJoinRandomOrCreateRoom"))
			{
				return false;
			}
			if (opJoinRandomRoomParams == null)
			{
				opJoinRandomRoomParams = new OpJoinRandomRoomParams();
			}
			if (createRoomParams == null)
			{
				createRoomParams = new EnterRoomParams();
			}
			createRoomParams.JoinMode = JoinMode.CreateIfNotExists;
			this.enterRoomParamsCache = createRoomParams;
			this.enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;
			this.enterRoomParamsCache.ExpectedUsers = opJoinRandomRoomParams.ExpectedUsers;
			bool flag = this.LoadBalancingPeer.OpJoinRandomOrCreateRoom(opJoinRandomRoomParams, createRoomParams);
			if (flag)
			{
				this.lastJoinType = JoinType.JoinRandomOrCreateRoom;
				this.State = ClientState.Joining;
			}
			return flag;
		}

		public bool OpCreateRoom(EnterRoomParams enterRoomParams)
		{
			if (!this.CheckIfOpCanBeSent(227, this.Server, "CreateGame"))
			{
				return false;
			}
			bool flag = this.Server == ServerConnection.GameServer;
			enterRoomParams.OnGameServer = flag;
			if (!flag)
			{
				this.enterRoomParamsCache = enterRoomParams;
			}
			bool flag2 = this.LoadBalancingPeer.OpCreateRoom(enterRoomParams);
			if (flag2)
			{
				this.lastJoinType = JoinType.CreateRoom;
				this.State = ClientState.Joining;
			}
			return flag2;
		}

		public bool OpJoinOrCreateRoom(EnterRoomParams enterRoomParams)
		{
			if (!this.CheckIfOpCanBeSent(226, this.Server, "JoinOrCreateRoom"))
			{
				return false;
			}
			bool flag = this.Server == ServerConnection.GameServer;
			enterRoomParams.JoinMode = JoinMode.CreateIfNotExists;
			enterRoomParams.OnGameServer = flag;
			if (!flag)
			{
				this.enterRoomParamsCache = enterRoomParams;
			}
			bool flag2 = this.LoadBalancingPeer.OpJoinRoom(enterRoomParams);
			if (flag2)
			{
				this.lastJoinType = JoinType.JoinOrCreateRoom;
				this.State = ClientState.Joining;
			}
			return flag2;
		}

		public bool OpJoinRoom(EnterRoomParams enterRoomParams)
		{
			if (!this.CheckIfOpCanBeSent(226, this.Server, "JoinRoom"))
			{
				return false;
			}
			bool flag = this.Server == ServerConnection.GameServer;
			enterRoomParams.OnGameServer = flag;
			if (!flag)
			{
				this.enterRoomParamsCache = enterRoomParams;
			}
			bool flag2 = this.LoadBalancingPeer.OpJoinRoom(enterRoomParams);
			if (flag2)
			{
				this.lastJoinType = ((enterRoomParams.JoinMode == JoinMode.CreateIfNotExists) ? JoinType.JoinOrCreateRoom : JoinType.JoinRoom);
				this.State = ClientState.Joining;
			}
			return flag2;
		}

		public bool OpRejoinRoom(string roomName)
		{
			if (!this.CheckIfOpCanBeSent(226, this.Server, "RejoinRoom"))
			{
				return false;
			}
			bool onGameServer = this.Server == ServerConnection.GameServer;
			EnterRoomParams enterRoomParams = new EnterRoomParams();
			this.enterRoomParamsCache = enterRoomParams;
			enterRoomParams.RoomName = roomName;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.JoinMode = JoinMode.RejoinOnly;
			bool flag = this.LoadBalancingPeer.OpJoinRoom(enterRoomParams);
			if (flag)
			{
				this.lastJoinType = JoinType.JoinRoom;
				this.State = ClientState.Joining;
			}
			return flag;
		}

		public bool OpLeaveRoom(bool becomeInactive, bool sendAuthCookie = false)
		{
			if (!this.CheckIfOpCanBeSent(254, this.Server, "LeaveRoom"))
			{
				return false;
			}
			this.State = ClientState.Leaving;
			this.GameServerAddress = string.Empty;
			this.enterRoomParamsCache = null;
			return this.LoadBalancingPeer.OpLeaveRoom(becomeInactive, sendAuthCookie);
		}

		public bool OpGetGameList(TypedLobby typedLobby, string sqlLobbyFilter)
		{
			if (!this.CheckIfOpCanBeSent(217, this.Server, "GetGameList"))
			{
				return false;
			}
			if (string.IsNullOrEmpty(sqlLobbyFilter))
			{
				this.DebugReturn(DebugLevel.ERROR, "Operation GetGameList requires a filter.");
				return false;
			}
			if (typedLobby.Type != LobbyType.SqlLobby)
			{
				this.DebugReturn(DebugLevel.ERROR, "Operation GetGameList can only be used for lobbies of type SqlLobby.");
				return false;
			}
			return this.LoadBalancingPeer.OpGetGameList(typedLobby, sqlLobbyFilter);
		}

		public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (propertiesToSet == null || propertiesToSet.Count == 0)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. propertiesToSet must not be null nor empty.");
				return false;
			}
			if (this.CurrentRoom == null)
			{
				if (expectedProperties == null && webFlags == null && this.LocalPlayer != null && this.LocalPlayer.ActorNumber == actorNr)
				{
					return this.LocalPlayer.SetCustomProperties(propertiesToSet, null, null);
				}
				if (this.LoadBalancingPeer.DebugOut >= DebugLevel.ERROR)
				{
					this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. To use expectedProperties or webForward, you have to be in a room. State: " + this.State.ToString());
				}
				return false;
			}
			else
			{
				Hashtable hashtable = new Hashtable();
				hashtable.MergeStringKeys(propertiesToSet);
				if (hashtable.Count == 0)
				{
					this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. Only string keys allowed for custom properties.");
					return false;
				}
				return this.OpSetPropertiesOfActor(actorNr, hashtable, expectedProperties, webFlags);
			}
		}

		protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (!this.CheckIfOpCanBeSent(252, this.Server, "SetProperties"))
			{
				return false;
			}
			if (actorProperties == null || actorProperties.Count == 0)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfActor() failed. actorProperties must not be null nor empty.");
				return false;
			}
			bool flag = this.LoadBalancingPeer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties, webFlags);
			if (flag && !this.CurrentRoom.BroadcastPropertiesChangeToAll && (expectedProperties == null || expectedProperties.Count == 0))
			{
				Player player = this.CurrentRoom.GetPlayer(actorNr, false);
				if (player != null)
				{
					player.InternalCacheProperties(actorProperties);
					this.InRoomCallbackTargets.OnPlayerPropertiesUpdate(player, actorProperties);
				}
			}
			return flag;
		}

		public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (propertiesToSet == null || propertiesToSet.Count == 0)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfRoom() failed. propertiesToSet must not be null nor empty.");
				return false;
			}
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(propertiesToSet);
			if (hashtable.Count == 0)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfRoom() failed. Only string keys are allowed for custom properties.");
				return false;
			}
			return this.OpSetPropertiesOfRoom(hashtable, expectedProperties, webFlags);
		}

		protected internal bool OpSetPropertyOfRoom(byte propCode, object value)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[propCode] = value;
			return this.OpSetPropertiesOfRoom(hashtable, null, null);
		}

		protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (!this.CheckIfOpCanBeSent(252, this.Server, "SetProperties"))
			{
				return false;
			}
			if (gameProperties == null || gameProperties.Count == 0)
			{
				this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfRoom() failed. gameProperties must not be null nor empty.");
				return false;
			}
			bool flag = this.LoadBalancingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, webFlags);
			if (flag && !this.CurrentRoom.BroadcastPropertiesChangeToAll && (expectedProperties == null || expectedProperties.Count == 0))
			{
				this.CurrentRoom.InternalCacheProperties(gameProperties);
				this.InRoomCallbackTargets.OnRoomPropertiesUpdate(gameProperties);
			}
			return flag;
		}

		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
		{
			return this.CheckIfOpCanBeSent(253, this.Server, "RaiseEvent") && this.LoadBalancingPeer.OpRaiseEvent(eventCode, customEventContent, raiseEventOptions, sendOptions);
		}

		public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
		{
			return this.CheckIfOpCanBeSent(248, this.Server, "ChangeGroups") && this.LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
		}

		private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
		{
			if (this.CurrentRoom != null && gameProperties != null)
			{
				this.CurrentRoom.InternalCacheProperties(gameProperties);
				if (this.InRoom)
				{
					this.InRoomCallbackTargets.OnRoomPropertiesUpdate(gameProperties);
				}
			}
			if (actorProperties != null && actorProperties.Count > 0)
			{
				if (targetActorNr > 0)
				{
					Player player = this.CurrentRoom.GetPlayer(targetActorNr, false);
					if (player != null)
					{
						Hashtable hashtable = this.ReadoutPropertiesForActorNr(actorProperties, targetActorNr);
						player.InternalCacheProperties(hashtable);
						this.InRoomCallbackTargets.OnPlayerPropertiesUpdate(player, hashtable);
						return;
					}
				}
				else
				{
					foreach (object obj in actorProperties.Keys)
					{
						int num = (int)obj;
						if (num != 0)
						{
							Hashtable hashtable2 = (Hashtable)actorProperties[obj];
							string actorName = (string)hashtable2[byte.MaxValue];
							Player player2 = this.CurrentRoom.GetPlayer(num, false);
							if (player2 == null)
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
			if (actorProperties.ContainsKey(actorNr))
			{
				return (Hashtable)actorProperties[actorNr];
			}
			return actorProperties;
		}

		public void ChangeLocalID(int newID)
		{
			if (this.LocalPlayer == null)
			{
				this.DebugReturn(DebugLevel.WARNING, string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", this.LocalPlayer, this.CurrentRoom.Players == null, newID));
			}
			if (this.CurrentRoom == null)
			{
				this.LocalPlayer.ChangeLocalID(newID);
				this.LocalPlayer.RoomReference = null;
				return;
			}
			this.CurrentRoom.RemovePlayer(this.LocalPlayer);
			this.LocalPlayer.ChangeLocalID(newID);
			this.CurrentRoom.StorePlayer(this.LocalPlayer);
		}

		private void GameEnteredOnGameServer(OperationResponse operationResponse)
		{
			this.CurrentRoom = this.CreateRoom(this.enterRoomParamsCache.RoomName, this.enterRoomParamsCache.RoomOptions);
			this.CurrentRoom.LoadBalancingClient = this;
			int newID = (int)operationResponse[254];
			this.ChangeLocalID(newID);
			if (operationResponse.Parameters.ContainsKey(252))
			{
				int[] actorsInGame = (int[])operationResponse.Parameters[252];
				this.UpdatedActorList(actorsInGame);
				if (this.LoadBalancingPeer.enterRoomParams != null)
				{
					if (this.LoadBalancingPeer.enterRoomParams.RoomOptions != null)
					{
						if (this.LoadBalancingPeer.enterRoomParams.RoomOptions.IsVisible)
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("Room option IsVisible is true", Array.Empty<object>()));
						}
						else
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("Room option IsVisible is false", Array.Empty<object>()));
						}
						if (this.LoadBalancingPeer.enterRoomParams.RoomOptions.IsOpen)
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("Room option IsOpen is true", Array.Empty<object>()));
						}
						else
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("Room option IsOpen is false", Array.Empty<object>()));
						}
						if (this.LoadBalancingPeer.enterRoomParams.RoomOptions.DeleteNullProperties)
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("Room option DeleteNullProperties is true", Array.Empty<object>()));
						}
						else
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("Room option DeleteNullProperties is false", Array.Empty<object>()));
						}
					}
					WebFlags webFlags = new WebFlags(1);
					Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
					dictionary.Add(244, 195);
					dictionary.Add(234, webFlags.WebhookFlags);
					this.LoadBalancingPeer.SendOperation(253, dictionary, SendOptions.SendReliable);
				}
			}
			Hashtable actorProperties = (Hashtable)operationResponse[249];
			Hashtable gameProperties = (Hashtable)operationResponse[248];
			this.ReadoutProperties(gameProperties, actorProperties, 0);
			object obj;
			if (operationResponse.Parameters.TryGetValue(191, out obj))
			{
				this.CurrentRoom.InternalCacheRoomFlags((int)obj);
			}
			this.State = ClientState.Joined;
			if (this.CurrentRoom.SuppressRoomEvents)
			{
				if (this.lastJoinType == JoinType.CreateRoom || (this.lastJoinType == JoinType.JoinOrCreateRoom && this.LocalPlayer.ActorNumber == 1))
				{
					this.MatchMakingCallbackTargets.OnCreatedRoom();
				}
				this.MatchMakingCallbackTargets.OnJoinedRoom();
			}
		}

		private void UpdatedActorList(int[] actorsInGame)
		{
			if (actorsInGame != null)
			{
				foreach (int num in actorsInGame)
				{
					if (num != 0 && this.CurrentRoom.GetPlayer(num, false) == null)
					{
						this.CurrentRoom.StorePlayer(this.CreatePlayer(string.Empty, num, false, null));
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
			if (this.LoadBalancingPeer == null)
			{
				this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) can't be sent because peer is null", opName, opCode));
				return false;
			}
			if (!this.CheckIfOpAllowedOnServer(opCode, serverConnection))
			{
				if (this.LoadBalancingPeer.DebugOut >= DebugLevel.ERROR)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) not allowed on current server ({2})", opName, opCode, serverConnection));
				}
				return false;
			}
			if (!this.CheckIfClientIsReadyToCallOperation(opCode))
			{
				DebugLevel debugLevel = DebugLevel.ERROR;
				if (opCode == 253 && (this.State == ClientState.Leaving || this.State == ClientState.Disconnecting || this.State == ClientState.DisconnectingFromGameServer))
				{
					debugLevel = DebugLevel.INFO;
				}
				if (this.LoadBalancingPeer.DebugOut >= debugLevel)
				{
					this.DebugReturn(debugLevel, string.Format("Operation {0} ({1}) not called because client is not connected or not ready yet, client state: {2}", opName, opCode, Enum.GetName(typeof(ClientState), this.State)));
				}
				return false;
			}
			if (this.LoadBalancingPeer.PeerState != PeerStateValue.Connected)
			{
				this.DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) can't be sent because peer is not connected, peer state: {2}", opName, opCode, this.LoadBalancingPeer.PeerState));
				return false;
			}
			return true;
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
				if (opCode == 222)
				{
					this.LoadBalancingPeer.enterRoomParams = new EnterRoomParams();
				}
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
			if (this.LoadBalancingPeer.DebugOut != DebugLevel.ALL && level > this.LoadBalancingPeer.DebugOut)
			{
				return;
			}
			if (level == DebugLevel.ERROR)
			{
				Debug.LogError(message);
				return;
			}
			if (level == DebugLevel.WARNING)
			{
				Debug.LogWarning(message);
				return;
			}
			if (level == DebugLevel.INFO)
			{
				Debug.Log(message);
				return;
			}
			if (level == DebugLevel.ALL)
			{
				Debug.Log(message);
			}
		}

		private void CallbackRoomEnterFailed(OperationResponse operationResponse)
		{
			if (operationResponse.ReturnCode != 0)
			{
				if (operationResponse.OperationCode == 226)
				{
					this.MatchMakingCallbackTargets.OnJoinRoomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
					return;
				}
				if (operationResponse.OperationCode == 227)
				{
					this.MatchMakingCallbackTargets.OnCreateRoomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
					return;
				}
				if (operationResponse.OperationCode == 225)
				{
					this.MatchMakingCallbackTargets.OnJoinRandomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
				}
			}
		}

		public virtual void OnOperationResponse(OperationResponse operationResponse)
		{
			if (operationResponse.Parameters.ContainsKey(221))
			{
				if (this.AuthValues == null)
				{
					this.AuthValues = new AuthenticationValues();
				}
				this.AuthValues.Token = (operationResponse[221] as string);
				this.tokenCache = this.AuthValues.Token;
			}
			if (operationResponse.ReturnCode == 32743)
			{
				this.Disconnect(DisconnectCause.DisconnectByOperationLimit);
			}
			byte operationCode = operationResponse.OperationCode;
			switch (operationCode)
			{
			case 217:
				if (operationResponse.ReturnCode != 0)
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
			case 218:
			case 221:
			case 222:
			case 224:
				break;
			case 219:
				this.WebRpcCallbackTargets.OnWebRpcResponse(operationResponse);
				break;
			case 220:
				if (operationResponse.ReturnCode == 32767)
				{
					this.DebugReturn(DebugLevel.ERROR, string.Format("GetRegions failed. AppId is unknown on the (cloud) server. " + operationResponse.DebugMessage, Array.Empty<object>()));
					this.Disconnect(DisconnectCause.InvalidAuthentication);
				}
				else if (operationResponse.ReturnCode != 0)
				{
					this.DebugReturn(DebugLevel.ERROR, "GetRegions failed. Can't provide regions list. ReturnCode: " + operationResponse.ReturnCode.ToString() + ": " + operationResponse.DebugMessage);
					this.Disconnect(DisconnectCause.InvalidAuthentication);
				}
				else
				{
					if (this.RegionHandler == null)
					{
						this.RegionHandler = new RegionHandler(this.ServerPortOverrides.MasterServerPort);
					}
					if (this.RegionHandler.IsPinging)
					{
						this.DebugReturn(DebugLevel.WARNING, "Received an response for OpGetRegions while the RegionHandler is pinging regions already. Skipping this response in favor of completing the current region-pinging.");
						return;
					}
					this.RegionHandler.SetRegions(operationResponse);
					this.ConnectionCallbackTargets.OnRegionListReceived(this.RegionHandler);
					if (this.connectToBestRegion)
					{
						this.RegionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), this.bestRegionSummaryFromStorage);
					}
				}
				break;
			case 223:
				if (operationResponse.ReturnCode != 0)
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
			case 225:
			case 226:
			case 227:
				if (operationResponse.ReturnCode != 0)
				{
					if (this.Server == ServerConnection.GameServer)
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
				else if (this.Server == ServerConnection.GameServer)
				{
					this.GameEnteredOnGameServer(operationResponse);
				}
				else
				{
					this.GameServerAddress = (string)operationResponse[230];
					if (this.ServerPortOverrides.GameServerPort != 0)
					{
						this.GameServerAddress = LoadBalancingClient.ReplacePortWithAlternative(this.GameServerAddress, this.ServerPortOverrides.GameServerPort);
					}
					string text2 = operationResponse[byte.MaxValue] as string;
					if (!string.IsNullOrEmpty(text2))
					{
						this.enterRoomParamsCache.RoomName = text2;
					}
					this.DisconnectToReconnect();
				}
				break;
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
				if (operationResponse.ReturnCode != 0)
				{
					this.DebugReturn(DebugLevel.WARNING, string.Concat(new string[]
					{
						operationResponse.ToStringFull(),
						" Server: ",
						this.Server.ToString(),
						" Address: ",
						this.LoadBalancingPeer.ServerAddress
					}));
					short returnCode = operationResponse.ReturnCode;
					if (returnCode - -3 > 1)
					{
						switch (returnCode)
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
							if (returnCode == 32767)
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
					this.Disconnect(this.DisconnectedCause);
				}
				else
				{
					if (this.Server == ServerConnection.NameServer || this.Server == ServerConnection.MasterServer)
					{
						if (operationResponse.Parameters.ContainsKey(225))
						{
							string text3 = (string)operationResponse.Parameters[225];
							if (!string.IsNullOrEmpty(text3))
							{
								this.UserId = text3;
								this.LocalPlayer.UserId = text3;
								this.DebugReturn(DebugLevel.INFO, string.Format("Received your UserID from server. Updating local value to: {0}", this.UserId));
							}
						}
						if (operationResponse.Parameters.ContainsKey(202))
						{
							this.NickName = (string)operationResponse.Parameters[202];
							this.DebugReturn(DebugLevel.INFO, string.Format("Received your NickName from server. Updating local value to: {0}", this.NickName));
						}
						if (operationResponse.Parameters.ContainsKey(192))
						{
							this.SetupEncryption((Dictionary<byte, object>)operationResponse.Parameters[192]);
						}
					}
					if (this.Server == ServerConnection.NameServer)
					{
						string text4 = operationResponse[196] as string;
						if (!string.IsNullOrEmpty(text4))
						{
							this.CurrentCluster = text4;
						}
						this.MasterServerAddress = (operationResponse[230] as string);
						if (this.ServerPortOverrides.MasterServerPort != 0)
						{
							this.MasterServerAddress = LoadBalancingClient.ReplacePortWithAlternative(this.MasterServerAddress, this.ServerPortOverrides.MasterServerPort);
						}
						if (this.AuthMode == AuthModeOption.AuthOnceWss && this.ExpectedProtocol != null)
						{
							this.DebugReturn(DebugLevel.INFO, string.Format("AuthOnceWss mode. Auth response switches TransportProtocol to ExpectedProtocol: {0}.", this.ExpectedProtocol));
							this.LoadBalancingPeer.TransportProtocol = this.ExpectedProtocol.Value;
							this.ExpectedProtocol = null;
						}
						this.DisconnectToReconnect();
					}
					else if (this.Server == ServerConnection.MasterServer)
					{
						this.State = ClientState.ConnectedToMasterServer;
						if (this.failedRoomEntryOperation == null)
						{
							this.ConnectionCallbackTargets.OnConnectedToMaster();
						}
						else
						{
							this.CallbackRoomEnterFailed(this.failedRoomEntryOperation);
							this.failedRoomEntryOperation = null;
						}
						if (this.AuthMode != AuthModeOption.Auth)
						{
							this.LoadBalancingPeer.OpSettings(this.EnableLobbyStatistics);
						}
					}
					else if (this.Server == ServerConnection.GameServer)
					{
						this.State = ClientState.Joining;
						if (this.enterRoomParamsCache.JoinMode == JoinMode.RejoinOnly)
						{
							this.enterRoomParamsCache.PlayerProperties = null;
						}
						else
						{
							Hashtable hashtable2 = new Hashtable();
							hashtable2.Merge(this.LocalPlayer.CustomProperties);
							if (!string.IsNullOrEmpty(this.LocalPlayer.NickName))
							{
								hashtable2[byte.MaxValue] = this.LocalPlayer.NickName;
							}
							this.enterRoomParamsCache.PlayerProperties = hashtable2;
						}
						this.enterRoomParamsCache.OnGameServer = true;
						if (this.lastJoinType == JoinType.JoinRoom || this.lastJoinType == JoinType.JoinRandomRoom || this.lastJoinType == JoinType.JoinRandomOrCreateRoom || this.lastJoinType == JoinType.JoinOrCreateRoom)
						{
							this.LoadBalancingPeer.OpJoinRoom(this.enterRoomParamsCache);
							break;
						}
						if (this.lastJoinType == JoinType.CreateRoom)
						{
							this.LoadBalancingPeer.OpCreateRoom(this.enterRoomParamsCache);
							break;
						}
						break;
					}
					Dictionary<string, object> dictionary = (Dictionary<string, object>)operationResponse[245];
					if (dictionary != null)
					{
						this.ConnectionCallbackTargets.OnCustomAuthenticationResponse(dictionary);
					}
				}
				break;
			default:
				if (operationCode == 254)
				{
					this.DisconnectToReconnect();
				}
				break;
			}
			if (this.OpResponseReceived != null)
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
				this.DisconnectedCause = DisconnectCause.ExceptionOnConnect;
				if (this.EnableProtocolFallback && this.State == ClientState.ConnectingToNameServer)
				{
					this.State = ClientState.ConnectWithFallbackProtocol;
					return;
				}
				this.State = ClientState.Disconnecting;
				return;
			case StatusCode.Connect:
				if (this.State == ClientState.ConnectingToNameServer)
				{
					if (this.LoadBalancingPeer.DebugOut >= DebugLevel.ALL)
					{
						this.DebugReturn(DebugLevel.ALL, "Connected to nameserver.");
					}
					this.Server = ServerConnection.NameServer;
					if (this.AuthValues != null)
					{
						this.AuthValues.Token = null;
					}
				}
				if (this.State == ClientState.ConnectingToGameServer)
				{
					if (this.LoadBalancingPeer.DebugOut >= DebugLevel.ALL)
					{
						this.DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
					}
					this.Server = ServerConnection.GameServer;
				}
				if (this.State == ClientState.ConnectingToMasterServer)
				{
					if (this.LoadBalancingPeer.DebugOut >= DebugLevel.ALL)
					{
						this.DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
					}
					this.Server = ServerConnection.MasterServer;
					this.ConnectionCallbackTargets.OnConnected();
				}
				if (this.LoadBalancingPeer.TransportProtocol != ConnectionProtocol.WebSocketSecure)
				{
					if (this.Server == ServerConnection.NameServer || this.AuthMode == AuthModeOption.Auth)
					{
						this.LoadBalancingPeer.EstablishEncryption();
						return;
					}
					return;
				}
				break;
			case StatusCode.Disconnect:
			{
				this.friendListRequested = null;
				bool flag = this.CurrentRoom != null;
				this.CurrentRoom = null;
				this.ChangeLocalID(-1);
				if (this.Server == ServerConnection.GameServer && flag)
				{
					this.MatchMakingCallbackTargets.OnLeftRoom();
				}
				if (this.ExpectedProtocol != null)
				{
					ConnectionProtocol transportProtocol = this.LoadBalancingPeer.TransportProtocol;
					ConnectionProtocol? expectedProtocol = this.ExpectedProtocol;
					if (!(transportProtocol == expectedProtocol.GetValueOrDefault() & expectedProtocol != null))
					{
						this.DebugReturn(DebugLevel.INFO, string.Format("On disconnect switches TransportProtocol to ExpectedProtocol: {0}.", this.ExpectedProtocol));
						this.LoadBalancingPeer.TransportProtocol = this.ExpectedProtocol.Value;
						this.ExpectedProtocol = null;
					}
				}
				ClientState clientState = this.State;
				if (clientState != ClientState.PeerCreated)
				{
					if (clientState != ClientState.DisconnectingFromMasterServer)
					{
						switch (clientState)
						{
						case ClientState.DisconnectingFromGameServer:
						case ClientState.DisconnectingFromNameServer:
							this.ConnectToMasterServer();
							return;
						case ClientState.Disconnecting:
							goto IL_324;
						case ClientState.Disconnected:
							return;
						case ClientState.ConnectWithFallbackProtocol:
							this.EnableProtocolFallback = false;
							this.LoadBalancingPeer.TransportProtocol = ((this.LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Tcp) ? ConnectionProtocol.Udp : ConnectionProtocol.Tcp);
							this.NameServerPortInAppSettings = 0;
							this.ServerPortOverrides = default(PhotonPortDefinition);
							if (!this.LoadBalancingPeer.Connect(this.NameServerAddress, this.ProxyServerAddress, this.AppId, this.TokenForInit, null))
							{
								return;
							}
							this.State = ClientState.ConnectingToNameServer;
							return;
						}
						string text = "";
						this.DebugReturn(DebugLevel.WARNING, string.Concat(new string[]
						{
							"Got a unexpected Disconnect in LoadBalancingClient State: ",
							this.State.ToString(),
							". Server: ",
							this.Server.ToString(),
							" Trace: ",
							text
						}));
						if (this.AuthValues != null)
						{
							this.AuthValues.Token = null;
						}
						this.State = ClientState.Disconnected;
						this.ConnectionCallbackTargets.OnDisconnected(this.DisconnectedCause);
						return;
					}
					this.Connect(this.GameServerAddress, this.ProxyServerAddress, ServerConnection.GameServer);
					return;
				}
				IL_324:
				if (this.AuthValues != null)
				{
					this.AuthValues.Token = null;
				}
				this.State = ClientState.Disconnected;
				this.ConnectionCallbackTargets.OnDisconnected(this.DisconnectedCause);
				return;
			}
			case StatusCode.Exception:
			case StatusCode.SendError:
			case StatusCode.ExceptionOnReceive:
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
				this.DisconnectedCause = DisconnectCause.ClientTimeout;
				if (this.EnableProtocolFallback && this.State == ClientState.ConnectingToNameServer)
				{
					this.State = ClientState.ConnectWithFallbackProtocol;
					return;
				}
				this.State = ClientState.Disconnecting;
				return;
			case StatusCode.DisconnectByServerTimeout:
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
			if (this.Server == ServerConnection.NameServer)
			{
				this.State = ClientState.ConnectedToNameServer;
				if (string.IsNullOrEmpty(this.CloudRegion))
				{
					this.OpGetRegions();
					return;
				}
			}
			else if (this.AuthMode == AuthModeOption.AuthOnce || this.AuthMode == AuthModeOption.AuthOnceWss)
			{
				return;
			}
			if (this.CallAuthenticate())
			{
				this.State = ClientState.Authenticating;
				return;
			}
			this.DebugReturn(DebugLevel.ERROR, "OpAuthenticate failed. Check log output and AuthValues. State: " + this.State.ToString());
		}

		public virtual void OnEvent(EventData photonEvent)
		{
			int sender = photonEvent.Sender;
			Player player = (this.CurrentRoom != null) ? this.CurrentRoom.GetPlayer(sender, false) : null;
			byte code = photonEvent.Code;
			switch (code)
			{
			case 223:
				if (this.AuthValues == null)
				{
					this.AuthValues = new AuthenticationValues();
				}
				this.AuthValues.Token = (photonEvent[221] as string);
				this.tokenCache = this.AuthValues.Token;
				break;
			case 224:
			{
				string[] array = photonEvent[213] as string[];
				int[] array2 = photonEvent[229] as int[];
				int[] array3 = photonEvent[228] as int[];
				ByteArraySlice byteArraySlice = photonEvent[212] as ByteArraySlice;
				bool flag = byteArraySlice != null;
				byte[] array4;
				if (flag)
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
				if (flag)
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
				switch (code)
				{
				case 251:
					this.ErrorInfoCallbackTargets.OnErrorInfo(new ErrorInfo(photonEvent));
					break;
				case 253:
				{
					int num = 0;
					if (photonEvent.Parameters.ContainsKey(253))
					{
						num = (int)photonEvent[253];
					}
					Hashtable gameProperties = null;
					Hashtable actorProperties = null;
					if (num == 0)
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
					if (player != null)
					{
						bool flag2 = false;
						if (photonEvent.Parameters.ContainsKey(233))
						{
							flag2 = (bool)photonEvent.Parameters[233];
						}
						if (flag2)
						{
							player.IsInactive = true;
						}
						else
						{
							player.IsInactive = false;
							this.CurrentRoom.RemovePlayer(sender);
						}
					}
					if (photonEvent.Parameters.ContainsKey(203))
					{
						int num2 = (int)photonEvent[203];
						if (num2 != 0)
						{
							this.CurrentRoom.masterClientId = num2;
							this.InRoomCallbackTargets.OnMasterClientSwitched(this.CurrentRoom.GetPlayer(num2, false));
						}
					}
					this.InRoomCallbackTargets.OnPlayerLeftRoom(player);
					break;
				case 255:
				{
					Hashtable hashtable2 = (Hashtable)photonEvent[249];
					if (player == null)
					{
						if (sender > 0)
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
					if (sender == this.LocalPlayer.ActorNumber)
					{
						int[] actorsInGame = (int[])photonEvent[252];
						this.UpdatedActorList(actorsInGame);
						player.HasRejoined = (this.enterRoomParamsCache.JoinMode == JoinMode.RejoinOnly);
						if (this.lastJoinType == JoinType.CreateRoom || (this.lastJoinType == JoinType.JoinOrCreateRoom && this.LocalPlayer.ActorNumber == 1))
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
			if (this.EventReceived != null)
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
			this.Disconnect(DisconnectCause.DisconnectByDisconnectMessage);
		}

		private void OnRegionPingCompleted(RegionHandler regionHandler)
		{
			this.SummaryToCache = regionHandler.SummaryToCache;
			this.ConnectToRegionMaster(regionHandler.BestRegion.Code);
		}

		protected internal static string ReplacePortWithAlternative(string address, ushort replacementPort)
		{
			if (address.StartsWith("ws"))
			{
				return new UriBuilder(address)
				{
					Port = (int)replacementPort
				}.ToString();
			}
			UriBuilder uriBuilder = new UriBuilder(string.Format("scheme://{0}", address));
			return string.Format("{0}:{1}", uriBuilder.Host, replacementPort);
		}

		private void SetupEncryption(Dictionary<byte, object> encryptionData)
		{
			EncryptionMode encryptionMode = (EncryptionMode)((byte)encryptionData[0]);
			if (encryptionMode == EncryptionMode.PayloadEncryption)
			{
				byte[] secret = (byte[])encryptionData[1];
				this.LoadBalancingPeer.InitPayloadEncryption(secret);
				return;
			}
			if (encryptionMode - EncryptionMode.DatagramEncryption <= 1)
			{
				byte[] encryptionSecret = (byte[])encryptionData[1];
				byte[] hmacSecret = (byte[])encryptionData[2];
				this.LoadBalancingPeer.InitDatagramEncryption(encryptionSecret, hmacSecret, encryptionMode == EncryptionMode.DatagramEncryptionRandomSequence, false);
				return;
			}
			if (encryptionMode != EncryptionMode.DatagramEncryptionGCM)
			{
				throw new ArgumentOutOfRangeException();
			}
			byte[] encryptionSecret2 = (byte[])encryptionData[1];
			this.LoadBalancingPeer.InitDatagramEncryption(encryptionSecret2, null, true, true);
		}

		public bool OpWebRpc(string uriPath, object parameters, bool sendAuthCookie = false)
		{
			if (string.IsNullOrEmpty(uriPath))
			{
				this.DebugReturn(DebugLevel.ERROR, "WebRPC method name must not be null nor empty.");
				return false;
			}
			if (!this.CheckIfOpCanBeSent(219, this.Server, "WebRpc"))
			{
				return false;
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(209, uriPath);
			if (parameters != null)
			{
				dictionary.Add(208, parameters);
			}
			if (sendAuthCookie)
			{
				dictionary.Add(234, 2);
			}
			return this.LoadBalancingPeer.SendOperation(219, dictionary, SendOptions.SendReliable);
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
				if (callbackTargetChange.AddTarget)
				{
					if (this.callbackTargets.Contains(callbackTargetChange.Target))
					{
						continue;
					}
					this.callbackTargets.Add(callbackTargetChange.Target);
				}
				else
				{
					if (!this.callbackTargets.Contains(callbackTargetChange.Target))
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
				if (onEventCallback != null)
				{
					if (callbackTargetChange.AddTarget)
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
			if (t != null)
			{
				if (change.AddTarget)
				{
					container.Add(t);
					return;
				}
				container.Remove(t);
			}
		}

		public AuthModeOption AuthMode;

		public EncryptionMode EncryptionMode;

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
				9093
			},
			{
				ConnectionProtocol.WebSocketSecure,
				19093
			}
		};

		public PhotonPortDefinition ServerPortOverrides;

		public string ProxyServerAddress;

		private ClientState state;

		public ConnectionCallbacksContainer ConnectionCallbackTargets;

		public MatchMakingCallbacksContainer MatchMakingCallbackTargets;

		internal InRoomCallbacksContainer InRoomCallbackTargets;

		internal LobbyCallbacksContainer LobbyCallbackTargets;

		internal WebRpcCallbacksContainer WebRpcCallbackTargets;

		internal ErrorInfoCallbacksContainer ErrorInfoCallbackTargets;

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
