using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.Pun
{
	public static class PhotonNetwork
	{
		public static string GameVersion
		{
			get
			{
				return PhotonNetwork.gameVersion;
			}
			set
			{
				PhotonNetwork.gameVersion = value;
				PhotonNetwork.NetworkingClient.AppVersion = string.Format("{0}_{1}", value, "2.40");
			}
		}

		public static string AppVersion
		{
			get
			{
				return PhotonNetwork.NetworkingClient.AppVersion;
			}
		}

		public static ServerSettings PhotonServerSettings
		{
			get
			{
				if (PhotonNetwork.photonServerSettings == null)
				{
					PhotonNetwork.LoadOrCreateSettings(false);
				}
				return PhotonNetwork.photonServerSettings;
			}
			private set
			{
				PhotonNetwork.photonServerSettings = value;
			}
		}

		public static string ServerAddress
		{
			get
			{
				if (PhotonNetwork.NetworkingClient == null)
				{
					return "<not connected>";
				}
				return PhotonNetwork.NetworkingClient.CurrentServerAddress;
			}
		}

		public static string CloudRegion
		{
			get
			{
				if (PhotonNetwork.NetworkingClient == null || !PhotonNetwork.IsConnected || PhotonNetwork.Server == ServerConnection.NameServer)
				{
					return null;
				}
				return PhotonNetwork.NetworkingClient.CloudRegion;
			}
		}

		public static string CurrentCluster
		{
			get
			{
				if (PhotonNetwork.NetworkingClient == null)
				{
					return null;
				}
				return PhotonNetwork.NetworkingClient.CurrentCluster;
			}
		}

		public static string BestRegionSummaryInPreferences
		{
			get
			{
				return PlayerPrefs.GetString("PUNCloudBestRegion", null);
			}
			internal set
			{
				if (string.IsNullOrEmpty(value))
				{
					PlayerPrefs.DeleteKey("PUNCloudBestRegion");
					return;
				}
				PlayerPrefs.SetString("PUNCloudBestRegion", value.ToString());
			}
		}

		public static bool IsConnected
		{
			get
			{
				return PhotonNetwork.OfflineMode || (PhotonNetwork.NetworkingClient != null && PhotonNetwork.NetworkingClient.IsConnected);
			}
		}

		public static bool IsConnectedAndReady
		{
			get
			{
				return PhotonNetwork.OfflineMode || (PhotonNetwork.NetworkingClient != null && PhotonNetwork.NetworkingClient.IsConnectedAndReady);
			}
		}

		public static ClientState NetworkClientState
		{
			get
			{
				if (PhotonNetwork.OfflineMode)
				{
					if (PhotonNetwork.offlineModeRoom == null)
					{
						return ClientState.ConnectedToMasterServer;
					}
					return ClientState.Joined;
				}
				else
				{
					if (PhotonNetwork.NetworkingClient == null)
					{
						return ClientState.Disconnected;
					}
					return PhotonNetwork.NetworkingClient.State;
				}
			}
		}

		public static ServerConnection Server
		{
			get
			{
				if (PhotonNetwork.OfflineMode)
				{
					if (PhotonNetwork.CurrentRoom != null)
					{
						return ServerConnection.GameServer;
					}
					return ServerConnection.MasterServer;
				}
				else
				{
					if (PhotonNetwork.NetworkingClient == null)
					{
						return ServerConnection.NameServer;
					}
					return PhotonNetwork.NetworkingClient.Server;
				}
			}
		}

		public static AuthenticationValues AuthValues
		{
			get
			{
				if (PhotonNetwork.NetworkingClient == null)
				{
					return null;
				}
				return PhotonNetwork.NetworkingClient.AuthValues;
			}
			set
			{
				if (PhotonNetwork.NetworkingClient != null)
				{
					PhotonNetwork.NetworkingClient.AuthValues = value;
				}
			}
		}

		public static TypedLobby CurrentLobby
		{
			get
			{
				return PhotonNetwork.NetworkingClient.CurrentLobby;
			}
		}

		public static Room CurrentRoom
		{
			get
			{
				if (PhotonNetwork.offlineMode)
				{
					return PhotonNetwork.offlineModeRoom;
				}
				if (PhotonNetwork.NetworkingClient != null)
				{
					return PhotonNetwork.NetworkingClient.CurrentRoom;
				}
				return null;
			}
		}

		public static Player LocalPlayer
		{
			get
			{
				if (PhotonNetwork.NetworkingClient == null)
				{
					return null;
				}
				return PhotonNetwork.NetworkingClient.LocalPlayer;
			}
		}

		public static string NickName
		{
			get
			{
				return PhotonNetwork.NetworkingClient.NickName;
			}
			set
			{
				PhotonNetwork.NetworkingClient.NickName = value;
			}
		}

		public static Player[] PlayerList
		{
			get
			{
				Room currentRoom = PhotonNetwork.CurrentRoom;
				if (currentRoom != null)
				{
					return (from x in currentRoom.Players.Values
					orderby x.ActorNumber
					select x).ToArray<Player>();
				}
				return new Player[0];
			}
		}

		public static Player[] PlayerListOthers
		{
			get
			{
				Room currentRoom = PhotonNetwork.CurrentRoom;
				if (currentRoom != null)
				{
					return (from x in currentRoom.Players.Values
					orderby x.ActorNumber
					where !x.IsLocal
					select x).ToArray<Player>();
				}
				return new Player[0];
			}
		}

		public static bool OfflineMode
		{
			get
			{
				return PhotonNetwork.offlineMode;
			}
			set
			{
				if (value == PhotonNetwork.offlineMode)
				{
					return;
				}
				if (value && PhotonNetwork.IsConnected)
				{
					Debug.LogError("Can't start OFFLINE mode while connected!");
					return;
				}
				if (PhotonNetwork.NetworkingClient.IsConnected)
				{
					PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByClientLogic);
				}
				PhotonNetwork.offlineMode = value;
				if (PhotonNetwork.offlineMode)
				{
					PhotonNetwork.NetworkingClient.ChangeLocalID(-1);
					PhotonNetwork.NetworkingClient.ConnectionCallbackTargets.OnConnectedToMaster();
					return;
				}
				bool flag = PhotonNetwork.offlineModeRoom != null;
				if (flag)
				{
					PhotonNetwork.LeftRoomCleanup();
				}
				PhotonNetwork.offlineModeRoom = null;
				PhotonNetwork.NetworkingClient.CurrentRoom = null;
				PhotonNetwork.NetworkingClient.ChangeLocalID(-1);
				if (flag)
				{
					PhotonNetwork.NetworkingClient.MatchMakingCallbackTargets.OnLeftRoom();
				}
			}
		}

		public static bool AutomaticallySyncScene
		{
			get
			{
				return PhotonNetwork.automaticallySyncScene;
			}
			set
			{
				PhotonNetwork.automaticallySyncScene = value;
				if (PhotonNetwork.automaticallySyncScene && PhotonNetwork.CurrentRoom != null)
				{
					PhotonNetwork.LoadLevelIfSynced();
				}
			}
		}

		public static bool EnableLobbyStatistics
		{
			get
			{
				return PhotonNetwork.NetworkingClient.EnableLobbyStatistics;
			}
		}

		public static bool InLobby
		{
			get
			{
				return PhotonNetwork.NetworkingClient.InLobby;
			}
		}

		public static int SendRate
		{
			get
			{
				return 1000 / PhotonNetwork.sendFrequency;
			}
			set
			{
				PhotonNetwork.sendFrequency = 1000 / value;
				if (PhotonHandler.Instance != null)
				{
					PhotonHandler.Instance.UpdateInterval = PhotonNetwork.sendFrequency;
				}
			}
		}

		public static int SerializationRate
		{
			get
			{
				return 1000 / PhotonNetwork.serializationFrequency;
			}
			set
			{
				PhotonNetwork.serializationFrequency = 1000 / value;
				if (PhotonHandler.Instance != null)
				{
					PhotonHandler.Instance.UpdateIntervalOnSerialize = PhotonNetwork.serializationFrequency;
				}
			}
		}

		public static bool IsMessageQueueRunning
		{
			get
			{
				return PhotonNetwork.isMessageQueueRunning;
			}
			set
			{
				PhotonNetwork.isMessageQueueRunning = value;
			}
		}

		public static double Time
		{
			get
			{
				if (UnityEngine.Time.frameCount == PhotonNetwork.frame)
				{
					return PhotonNetwork.frametime;
				}
				PhotonNetwork.frametime = PhotonNetwork.ServerTimestamp / 1000.0;
				PhotonNetwork.frame = UnityEngine.Time.frameCount;
				return PhotonNetwork.frametime;
			}
		}

		public static double CurrentTime
		{
			get
			{
				return PhotonNetwork.ServerTimestamp / 1000.0;
			}
		}

		public static int ServerTimestamp
		{
			get
			{
				if (!PhotonNetwork.OfflineMode)
				{
					return PhotonNetwork.NetworkingClient.LoadBalancingPeer.ServerTimeInMilliSeconds;
				}
				if (PhotonNetwork.StartupStopwatch != null && PhotonNetwork.StartupStopwatch.IsRunning)
				{
					return (int)PhotonNetwork.StartupStopwatch.ElapsedMilliseconds;
				}
				return Environment.TickCount;
			}
		}

		public static float KeepAliveInBackground
		{
			get
			{
				if (!(PhotonHandler.Instance != null))
				{
					return 60f;
				}
				return Mathf.Round((float)PhotonHandler.Instance.KeepAliveInBackground / 1000f);
			}
			set
			{
				if (PhotonHandler.Instance != null)
				{
					PhotonHandler.Instance.KeepAliveInBackground = (int)Mathf.Round(value * 1000f);
				}
			}
		}

		public static bool IsMasterClient
		{
			get
			{
				return PhotonNetwork.OfflineMode || (PhotonNetwork.NetworkingClient.CurrentRoom != null && PhotonNetwork.NetworkingClient.CurrentRoom.MasterClientId == PhotonNetwork.LocalPlayer.ActorNumber);
			}
		}

		public static Player MasterClient
		{
			get
			{
				if (PhotonNetwork.OfflineMode)
				{
					return PhotonNetwork.LocalPlayer;
				}
				if (PhotonNetwork.NetworkingClient == null || PhotonNetwork.NetworkingClient.CurrentRoom == null)
				{
					return null;
				}
				return PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(PhotonNetwork.NetworkingClient.CurrentRoom.MasterClientId, false);
			}
		}

		public static bool InRoom
		{
			get
			{
				return PhotonNetwork.NetworkClientState == ClientState.Joined;
			}
		}

		public static int CountOfPlayersOnMaster
		{
			get
			{
				return PhotonNetwork.NetworkingClient.PlayersOnMasterCount;
			}
		}

		public static int CountOfPlayersInRooms
		{
			get
			{
				return PhotonNetwork.NetworkingClient.PlayersInRoomsCount;
			}
		}

		public static int CountOfPlayers
		{
			get
			{
				return PhotonNetwork.NetworkingClient.PlayersInRoomsCount + PhotonNetwork.NetworkingClient.PlayersOnMasterCount;
			}
		}

		public static int CountOfRooms
		{
			get
			{
				return PhotonNetwork.NetworkingClient.RoomsCount;
			}
		}

		public static bool NetworkStatisticsEnabled
		{
			get
			{
				return PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled;
			}
			set
			{
				PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = value;
			}
		}

		public static int ResentReliableCommands
		{
			get
			{
				return PhotonNetwork.NetworkingClient.LoadBalancingPeer.ResentReliableCommands;
			}
		}

		public static bool CrcCheckEnabled
		{
			get
			{
				return PhotonNetwork.NetworkingClient.LoadBalancingPeer.CrcEnabled;
			}
			set
			{
				if (!PhotonNetwork.IsConnected)
				{
					PhotonNetwork.NetworkingClient.LoadBalancingPeer.CrcEnabled = value;
					return;
				}
				Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.CrcEnabled.ToString());
			}
		}

		public static int PacketLossByCrcCheck
		{
			get
			{
				return PhotonNetwork.NetworkingClient.LoadBalancingPeer.PacketLossByCrc;
			}
		}

		public static int MaxResendsBeforeDisconnect
		{
			get
			{
				return PhotonNetwork.NetworkingClient.LoadBalancingPeer.SentCountAllowance;
			}
			set
			{
				if (value < 3)
				{
					value = 3;
				}
				if (value > 10)
				{
					value = 10;
				}
				PhotonNetwork.NetworkingClient.LoadBalancingPeer.SentCountAllowance = value;
			}
		}

		public static int QuickResends
		{
			get
			{
				return (int)PhotonNetwork.NetworkingClient.LoadBalancingPeer.QuickResendAttempts;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				if (value > 3)
				{
					value = 3;
				}
				PhotonNetwork.NetworkingClient.LoadBalancingPeer.QuickResendAttempts = (byte)value;
			}
		}

		[Obsolete("Set port overrides in ServerPortOverrides. Not used anymore!")]
		public static bool UseAlternativeUdpPorts { get; set; }

		public static PhotonPortDefinition ServerPortOverrides
		{
			get
			{
				if (PhotonNetwork.NetworkingClient != null)
				{
					return PhotonNetwork.NetworkingClient.ServerPortOverrides;
				}
				return default(PhotonPortDefinition);
			}
			set
			{
				if (PhotonNetwork.NetworkingClient != null)
				{
					PhotonNetwork.NetworkingClient.ServerPortOverrides = value;
				}
			}
		}

		static PhotonNetwork()
		{
			PhotonNetwork.StaticReset();
		}

		private static void StaticReset()
		{
			PhotonNetwork.monoRPCMethodsCache.Clear();
			PhotonNetwork.OfflineMode = false;
			PhotonNetwork.NetworkingClient = new LoadBalancingClient(PhotonNetwork.PhotonServerSettings.AppSettings.Protocol);
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.QuickResendAttempts = 2;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.SentCountAllowance = 9;
			PhotonNetwork.NetworkingClient.EventReceived -= PhotonNetwork.OnEvent;
			PhotonNetwork.NetworkingClient.EventReceived += PhotonNetwork.OnEvent;
			PhotonNetwork.NetworkingClient.OpResponseReceived -= PhotonNetwork.OnOperation;
			PhotonNetwork.NetworkingClient.OpResponseReceived += PhotonNetwork.OnOperation;
			PhotonNetwork.NetworkingClient.StateChanged -= PhotonNetwork.OnClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged += PhotonNetwork.OnClientStateChanged;
			PhotonNetwork.StartupStopwatch = new Stopwatch();
			PhotonNetwork.StartupStopwatch.Start();
			PhotonHandler.Instance.Client = PhotonNetwork.NetworkingClient;
			Application.runInBackground = PhotonNetwork.PhotonServerSettings.RunInBackground;
			PhotonNetwork.PrefabPool = new DefaultPool();
			PhotonNetwork.rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
			for (int i = 0; i < PhotonNetwork.PhotonServerSettings.RpcList.Count; i++)
			{
				string key = PhotonNetwork.PhotonServerSettings.RpcList[i];
				PhotonNetwork.rpcShortcuts[key] = i;
			}
			CustomTypes.Register();
		}

		public static bool ConnectUsingSettings()
		{
			if (PhotonNetwork.PhotonServerSettings == null)
			{
				Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
				return false;
			}
			return PhotonNetwork.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings, PhotonNetwork.PhotonServerSettings.StartInOfflineMode);
		}

		public static bool ConnectUsingSettings(AppSettings appSettings, bool startInOfflineMode = false)
		{
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				Debug.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (ConnectionHandler.AppQuits)
			{
				Debug.LogWarning("Can't connect: Application is closing. Unity called OnApplicationQuit().");
				return false;
			}
			if (PhotonNetwork.PhotonServerSettings == null)
			{
				Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: PhotonServerSettings");
				return false;
			}
			PhotonNetwork.SetupLogging();
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.TransportProtocol = appSettings.Protocol;
			PhotonNetwork.NetworkingClient.ExpectedProtocol = null;
			PhotonNetwork.NetworkingClient.EnableProtocolFallback = appSettings.EnableProtocolFallback;
			PhotonNetwork.NetworkingClient.AuthMode = appSettings.AuthMode;
			PhotonNetwork.IsMessageQueueRunning = true;
			PhotonNetwork.NetworkingClient.AppId = appSettings.AppIdRealtime;
			PhotonNetwork.GameVersion = appSettings.AppVersion;
			if (startInOfflineMode)
			{
				PhotonNetwork.OfflineMode = true;
				return true;
			}
			if (PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.OfflineMode = false;
				Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
			}
			PhotonNetwork.NetworkingClient.EnableLobbyStatistics = appSettings.EnableLobbyStatistics;
			PhotonNetwork.NetworkingClient.ProxyServerAddress = appSettings.ProxyServer;
			if (appSettings.IsMasterServerAddress)
			{
				if (PhotonNetwork.AuthValues == null)
				{
					PhotonNetwork.AuthValues = new AuthenticationValues(Guid.NewGuid().ToString());
				}
				else if (string.IsNullOrEmpty(PhotonNetwork.AuthValues.UserId))
				{
					PhotonNetwork.AuthValues.UserId = Guid.NewGuid().ToString();
				}
				return PhotonNetwork.ConnectToMaster(appSettings.Server, appSettings.Port, appSettings.AppIdRealtime);
			}
			PhotonNetwork.NetworkingClient.NameServerPortInAppSettings = appSettings.Port;
			if (!appSettings.IsDefaultNameServer)
			{
				PhotonNetwork.NetworkingClient.NameServerHost = appSettings.Server;
			}
			if (appSettings.IsBestRegion)
			{
				return PhotonNetwork.ConnectToBestCloudServer();
			}
			return PhotonNetwork.ConnectToRegion(appSettings.FixedRegion);
		}

		public static bool ConnectToMaster(string masterServerAddress, int port, string appID)
		{
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (ConnectionHandler.AppQuits)
			{
				Debug.LogWarning("Can't connect: Application is closing. Unity called OnApplicationQuit().");
				return false;
			}
			if (PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.OfflineMode = false;
				Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
			}
			if (!PhotonNetwork.IsMessageQueueRunning)
			{
				PhotonNetwork.IsMessageQueueRunning = true;
				Debug.LogWarning("ConnectToMaster() enabled IsMessageQueueRunning. Needs to be able to dispatch incoming messages.");
			}
			PhotonNetwork.SetupLogging();
			PhotonNetwork.ConnectMethod = ConnectMethod.ConnectToMaster;
			PhotonNetwork.NetworkingClient.IsUsingNameServer = false;
			PhotonNetwork.NetworkingClient.MasterServerAddress = ((port == 0) ? masterServerAddress : (masterServerAddress + ":" + port.ToString()));
			PhotonNetwork.NetworkingClient.AppId = appID;
			return PhotonNetwork.NetworkingClient.ConnectToMasterServer();
		}

		public static bool ConnectToBestCloudServer()
		{
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				Debug.LogWarning("ConnectToBestCloudServer() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (ConnectionHandler.AppQuits)
			{
				Debug.LogWarning("Can't connect: Application is closing. Unity called OnApplicationQuit().");
				return false;
			}
			PhotonNetwork.SetupLogging();
			PhotonNetwork.ConnectMethod = ConnectMethod.ConnectToBest;
			return PhotonNetwork.NetworkingClient.ConnectToNameServer();
		}

		public static bool ConnectToRegion(string region)
		{
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected && PhotonNetwork.NetworkingClient.Server != ServerConnection.NameServer)
			{
				Debug.LogWarning("ConnectToRegion() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (ConnectionHandler.AppQuits)
			{
				Debug.LogWarning("Can't connect: Application is closing. Unity called OnApplicationQuit().");
				return false;
			}
			PhotonNetwork.SetupLogging();
			PhotonNetwork.ConnectMethod = ConnectMethod.ConnectToRegion;
			return !string.IsNullOrEmpty(region) && PhotonNetwork.NetworkingClient.ConnectToRegionMaster(region);
		}

		public static void Disconnect()
		{
			if (PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.OfflineMode = false;
				PhotonNetwork.offlineModeRoom = null;
				PhotonNetwork.NetworkingClient.State = ClientState.Disconnecting;
				PhotonNetwork.NetworkingClient.OnStatusChanged(StatusCode.Disconnect);
				return;
			}
			if (PhotonNetwork.NetworkingClient == null)
			{
				return;
			}
			PhotonNetwork.NetworkingClient.Disconnect(DisconnectCause.DisconnectByClientLogic);
		}

		public static bool Reconnect()
		{
			if (string.IsNullOrEmpty(PhotonNetwork.NetworkingClient.MasterServerAddress))
			{
				Debug.LogWarning("Reconnect() failed. It seems the client wasn't connected before?! Current state: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				Debug.LogWarning("Reconnect() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.OfflineMode = false;
				Debug.LogWarning("Reconnect() disabled the offline mode. No longer offline.");
			}
			if (!PhotonNetwork.IsMessageQueueRunning)
			{
				PhotonNetwork.IsMessageQueueRunning = true;
				Debug.LogWarning("Reconnect() enabled IsMessageQueueRunning. Needs to be able to dispatch incoming messages.");
			}
			PhotonNetwork.NetworkingClient.IsUsingNameServer = false;
			return PhotonNetwork.NetworkingClient.ReconnectToMaster();
		}

		public static void NetworkStatisticsReset()
		{
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsReset();
		}

		public static string NetworkStatisticsToString()
		{
			if (PhotonNetwork.NetworkingClient == null || PhotonNetwork.OfflineMode)
			{
				return "Offline or in OfflineMode. No VitalStats available.";
			}
			return PhotonNetwork.NetworkingClient.LoadBalancingPeer.VitalStatsToString(false);
		}

		private static bool VerifyCanUseNetwork()
		{
			if (PhotonNetwork.IsConnected)
			{
				return true;
			}
			Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
			return false;
		}

		public static int GetPing()
		{
			return PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime;
		}

		public static void FetchServerTimestamp()
		{
			if (PhotonNetwork.NetworkingClient != null)
			{
				PhotonNetwork.NetworkingClient.LoadBalancingPeer.FetchServerTimestamp();
			}
		}

		public static void SendAllOutgoingCommands()
		{
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return;
			}
			while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands())
			{
			}
		}

		public static bool CloseConnection(Player kickPlayer)
		{
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return false;
			}
			if (!PhotonNetwork.EnableCloseConnection)
			{
				Debug.LogError("CloseConnection is disabled. No need to call it.");
				return false;
			}
			if (!PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
				return false;
			}
			if (kickPlayer == null)
			{
				Debug.LogError("CloseConnection: No such player connected!");
				return false;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				TargetActors = new int[]
				{
					kickPlayer.ActorNumber
				}
			};
			return PhotonNetwork.NetworkingClient.OpRaiseEvent(203, null, raiseEventOptions, SendOptions.SendReliable);
		}

		public static bool SetMasterClient(Player masterClientPlayer)
		{
			if (!PhotonNetwork.InRoom || !PhotonNetwork.VerifyCanUseNetwork() || PhotonNetwork.OfflineMode)
			{
				if (PhotonNetwork.LogLevel == PunLogLevel.Informational)
				{
					Debug.Log("Can not SetMasterClient(). Not in room or in OfflineMode.");
				}
				return false;
			}
			return PhotonNetwork.CurrentRoom.SetMasterClient(masterClientPlayer);
		}

		public static bool JoinRandomRoom()
		{
			return PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null, null);
		}

		public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
		{
			return PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null, null);
		}

		public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter, string[] expectedUsers = null)
		{
			if (PhotonNetwork.OfflineMode)
			{
				if (PhotonNetwork.offlineModeRoom != null)
				{
					Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
					return false;
				}
				PhotonNetwork.EnterOfflineRoom("offline room", null, true);
				return true;
			}
			else
			{
				if (PhotonNetwork.NetworkingClient.Server != ServerConnection.MasterServer || !PhotonNetwork.IsConnectedAndReady)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"JoinRandomRoom failed. Client is on ",
						PhotonNetwork.NetworkingClient.Server.ToString(),
						" (must be Master Server for matchmaking)",
						PhotonNetwork.IsConnectedAndReady ? " and ready" : (" but not ready for operations (State: " + PhotonNetwork.NetworkingClient.State.ToString() + ")"),
						". Wait for callback: OnJoinedLobby or OnConnectedToMaster."
					}));
					return false;
				}
				typedLobby = (typedLobby ?? (PhotonNetwork.NetworkingClient.InLobby ? PhotonNetwork.NetworkingClient.CurrentLobby : null));
				OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
				opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
				opJoinRandomRoomParams.ExpectedMaxPlayers = expectedMaxPlayers;
				opJoinRandomRoomParams.MatchingType = matchingType;
				opJoinRandomRoomParams.TypedLobby = typedLobby;
				opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
				opJoinRandomRoomParams.ExpectedUsers = expectedUsers;
				return PhotonNetwork.NetworkingClient.OpJoinRandomRoom(opJoinRandomRoomParams);
			}
		}

		public static bool JoinRandomOrCreateRoom(Hashtable expectedCustomRoomProperties = null, byte expectedMaxPlayers = 0, MatchmakingMode matchingType = MatchmakingMode.FillRoom, TypedLobby typedLobby = null, string sqlLobbyFilter = null, string roomName = null, RoomOptions roomOptions = null, string[] expectedUsers = null)
		{
			if (PhotonNetwork.OfflineMode)
			{
				if (PhotonNetwork.offlineModeRoom != null)
				{
					Debug.LogError("JoinRandomOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
					return false;
				}
				PhotonNetwork.EnterOfflineRoom("offline room", null, true);
				return true;
			}
			else
			{
				if (PhotonNetwork.NetworkingClient.Server != ServerConnection.MasterServer || !PhotonNetwork.IsConnectedAndReady)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"JoinRandomOrCreateRoom failed. Client is on ",
						PhotonNetwork.NetworkingClient.Server.ToString(),
						" (must be Master Server for matchmaking)",
						PhotonNetwork.IsConnectedAndReady ? " and ready" : (" but not ready for operations (State: " + PhotonNetwork.NetworkingClient.State.ToString() + ")"),
						". Wait for callback: OnJoinedLobby or OnConnectedToMaster."
					}));
					return false;
				}
				typedLobby = (typedLobby ?? (PhotonNetwork.NetworkingClient.InLobby ? PhotonNetwork.NetworkingClient.CurrentLobby : null));
				OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
				opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
				opJoinRandomRoomParams.ExpectedMaxPlayers = expectedMaxPlayers;
				opJoinRandomRoomParams.MatchingType = matchingType;
				opJoinRandomRoomParams.TypedLobby = typedLobby;
				opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
				opJoinRandomRoomParams.ExpectedUsers = expectedUsers;
				EnterRoomParams enterRoomParams = new EnterRoomParams();
				enterRoomParams.RoomName = roomName;
				enterRoomParams.RoomOptions = roomOptions;
				enterRoomParams.Lobby = typedLobby;
				enterRoomParams.ExpectedUsers = expectedUsers;
				return PhotonNetwork.NetworkingClient.OpJoinRandomOrCreateRoom(opJoinRandomRoomParams, enterRoomParams);
			}
		}

		public static bool CreateRoom(string roomName, RoomOptions roomOptions = null, TypedLobby typedLobby = null, string[] expectedUsers = null)
		{
			if (PhotonNetwork.OfflineMode)
			{
				if (PhotonNetwork.offlineModeRoom != null)
				{
					Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
					return false;
				}
				PhotonNetwork.EnterOfflineRoom(roomName, roomOptions, true);
				return true;
			}
			else
			{
				if (PhotonNetwork.NetworkingClient.Server != ServerConnection.MasterServer || !PhotonNetwork.IsConnectedAndReady)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"CreateRoom failed. Client is on ",
						PhotonNetwork.NetworkingClient.Server.ToString(),
						" (must be Master Server for matchmaking)",
						PhotonNetwork.IsConnectedAndReady ? " and ready" : ("but not ready for operations (State: " + PhotonNetwork.NetworkingClient.State.ToString() + ")"),
						". Wait for callback: OnJoinedLobby or OnConnectedToMaster."
					}));
					return false;
				}
				typedLobby = (typedLobby ?? (PhotonNetwork.NetworkingClient.InLobby ? PhotonNetwork.NetworkingClient.CurrentLobby : null));
				EnterRoomParams enterRoomParams = new EnterRoomParams();
				enterRoomParams.RoomName = roomName;
				enterRoomParams.RoomOptions = roomOptions;
				enterRoomParams.Lobby = typedLobby;
				enterRoomParams.ExpectedUsers = expectedUsers;
				return PhotonNetwork.NetworkingClient.OpCreateRoom(enterRoomParams);
			}
		}

		public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers = null)
		{
			if (PhotonNetwork.OfflineMode)
			{
				if (PhotonNetwork.offlineModeRoom != null)
				{
					Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
					return false;
				}
				PhotonNetwork.EnterOfflineRoom(roomName, roomOptions, true);
				return true;
			}
			else
			{
				if (PhotonNetwork.NetworkingClient.Server != ServerConnection.MasterServer || !PhotonNetwork.IsConnectedAndReady)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"JoinOrCreateRoom failed. Client is on ",
						PhotonNetwork.NetworkingClient.Server.ToString(),
						" (must be Master Server for matchmaking)",
						PhotonNetwork.IsConnectedAndReady ? " and ready" : ("but not ready for operations (State: " + PhotonNetwork.NetworkingClient.State.ToString() + ")"),
						". Wait for callback: OnJoinedLobby or OnConnectedToMaster."
					}));
					return false;
				}
				if (string.IsNullOrEmpty(roomName))
				{
					Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
					return false;
				}
				typedLobby = (typedLobby ?? (PhotonNetwork.NetworkingClient.InLobby ? PhotonNetwork.NetworkingClient.CurrentLobby : null));
				EnterRoomParams enterRoomParams = new EnterRoomParams();
				enterRoomParams.RoomName = roomName;
				enterRoomParams.RoomOptions = roomOptions;
				enterRoomParams.Lobby = typedLobby;
				enterRoomParams.PlayerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
				enterRoomParams.ExpectedUsers = expectedUsers;
				return PhotonNetwork.NetworkingClient.OpJoinOrCreateRoom(enterRoomParams);
			}
		}

		public static bool JoinRoom(string roomName, string[] expectedUsers = null)
		{
			if (PhotonNetwork.OfflineMode)
			{
				if (PhotonNetwork.offlineModeRoom != null)
				{
					Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
					return false;
				}
				PhotonNetwork.EnterOfflineRoom(roomName, null, true);
				return true;
			}
			else
			{
				if (PhotonNetwork.NetworkingClient.Server != ServerConnection.MasterServer || !PhotonNetwork.IsConnectedAndReady)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"JoinRoom failed. Client is on ",
						PhotonNetwork.NetworkingClient.Server.ToString(),
						" (must be Master Server for matchmaking)",
						PhotonNetwork.IsConnectedAndReady ? " and ready" : ("but not ready for operations (State: " + PhotonNetwork.NetworkingClient.State.ToString() + ")"),
						". Wait for callback: OnJoinedLobby or OnConnectedToMaster."
					}));
					return false;
				}
				if (string.IsNullOrEmpty(roomName))
				{
					Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
					return false;
				}
				EnterRoomParams enterRoomParams = new EnterRoomParams();
				enterRoomParams.RoomName = roomName;
				enterRoomParams.ExpectedUsers = expectedUsers;
				return PhotonNetwork.NetworkingClient.OpJoinRoom(enterRoomParams);
			}
		}

		public static bool RejoinRoom(string roomName)
		{
			if (PhotonNetwork.OfflineMode)
			{
				Debug.LogError("RejoinRoom failed due to offline mode.");
				return false;
			}
			if (PhotonNetwork.NetworkingClient.Server != ServerConnection.MasterServer || !PhotonNetwork.IsConnectedAndReady)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"RejoinRoom failed. Client is on ",
					PhotonNetwork.NetworkingClient.Server.ToString(),
					" (must be Master Server for matchmaking)",
					PhotonNetwork.IsConnectedAndReady ? " and ready" : ("but not ready for operations (State: " + PhotonNetwork.NetworkingClient.State.ToString() + ")"),
					". Wait for callback: OnJoinedLobby or OnConnectedToMaster."
				}));
				return false;
			}
			if (string.IsNullOrEmpty(roomName))
			{
				Debug.LogError("RejoinRoom failed. A roomname is required. If you don't know one, how will you join?");
				return false;
			}
			return PhotonNetwork.NetworkingClient.OpRejoinRoom(roomName);
		}

		public static bool ReconnectAndRejoin()
		{
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
			{
				Debug.LogWarning("ReconnectAndRejoin() failed. Can only connect while in state 'Disconnected'. Current state: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState.ToString());
				return false;
			}
			if (PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.OfflineMode = false;
				Debug.LogWarning("ReconnectAndRejoin() disabled the offline mode. No longer offline.");
			}
			if (!PhotonNetwork.IsMessageQueueRunning)
			{
				PhotonNetwork.IsMessageQueueRunning = true;
				Debug.LogWarning("ReconnectAndRejoin() enabled IsMessageQueueRunning. Needs to be able to dispatch incoming messages.");
			}
			return PhotonNetwork.NetworkingClient.ReconnectAndRejoin();
		}

		public static bool LeaveRoom(bool becomeInactive = true)
		{
			if (PhotonNetwork.OfflineMode)
			{
				PhotonNetwork.offlineModeRoom = null;
				PhotonNetwork.NetworkingClient.MatchMakingCallbackTargets.OnLeftRoom();
				PhotonNetwork.NetworkingClient.ConnectionCallbackTargets.OnConnectedToMaster();
				return true;
			}
			if (PhotonNetwork.CurrentRoom == null)
			{
				Debug.LogWarning("PhotonNetwork.CurrentRoom is null. You don't have to call LeaveRoom() when you're not in one. State: " + PhotonNetwork.NetworkClientState.ToString());
			}
			else
			{
				becomeInactive = (becomeInactive && PhotonNetwork.CurrentRoom.PlayerTtl != 0);
			}
			return PhotonNetwork.NetworkingClient.OpLeaveRoom(becomeInactive, false);
		}

		private static void EnterOfflineRoom(string roomName, RoomOptions roomOptions, bool createdRoom)
		{
			PhotonNetwork.offlineModeRoom = new Room(roomName, roomOptions, true);
			PhotonNetwork.NetworkingClient.ChangeLocalID(1);
			PhotonNetwork.offlineModeRoom.masterClientId = 1;
			PhotonNetwork.offlineModeRoom.AddPlayer(PhotonNetwork.LocalPlayer);
			PhotonNetwork.offlineModeRoom.LoadBalancingClient = PhotonNetwork.NetworkingClient;
			PhotonNetwork.NetworkingClient.CurrentRoom = PhotonNetwork.offlineModeRoom;
			if (createdRoom)
			{
				PhotonNetwork.NetworkingClient.MatchMakingCallbackTargets.OnCreatedRoom();
			}
			PhotonNetwork.NetworkingClient.MatchMakingCallbackTargets.OnJoinedRoom();
		}

		public static bool JoinLobby()
		{
			return PhotonNetwork.JoinLobby(null);
		}

		public static bool JoinLobby(TypedLobby typedLobby)
		{
			return PhotonNetwork.IsConnected && PhotonNetwork.Server == ServerConnection.MasterServer && PhotonNetwork.NetworkingClient.OpJoinLobby(typedLobby);
		}

		public static bool LeaveLobby()
		{
			return PhotonNetwork.IsConnected && PhotonNetwork.Server == ServerConnection.MasterServer && PhotonNetwork.NetworkingClient.OpLeaveLobby();
		}

		public static bool FindFriends(string[] friendsToFind)
		{
			return PhotonNetwork.NetworkingClient != null && !PhotonNetwork.offlineMode && PhotonNetwork.NetworkingClient.OpFindFriends(friendsToFind, null);
		}

		public static bool GetCustomRoomList(TypedLobby typedLobby, string sqlLobbyFilter)
		{
			return PhotonNetwork.NetworkingClient.OpGetGameList(typedLobby, sqlLobbyFilter);
		}

		public static bool SetPlayerCustomProperties(Hashtable customProperties)
		{
			if (customProperties == null)
			{
				customProperties = new Hashtable();
				foreach (object obj in PhotonNetwork.LocalPlayer.CustomProperties.Keys)
				{
					customProperties[(string)obj] = null;
				}
			}
			return PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
		}

		public static void RemovePlayerCustomProperties(string[] customPropertiesToDelete)
		{
			if (customPropertiesToDelete == null || customPropertiesToDelete.Length == 0 || PhotonNetwork.LocalPlayer.CustomProperties == null)
			{
				PhotonNetwork.LocalPlayer.CustomProperties = new Hashtable();
				return;
			}
			foreach (string key in customPropertiesToDelete)
			{
				if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(key))
				{
					PhotonNetwork.LocalPlayer.CustomProperties.Remove(key);
				}
			}
		}

		public static bool RaiseEvent(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
		{
			if (PhotonNetwork.offlineMode)
			{
				if (raiseEventOptions.Receivers == ReceiverGroup.Others)
				{
					return true;
				}
				EventData eventData = new EventData
				{
					Code = eventCode
				};
				eventData.Parameters[245] = eventContent;
				eventData.Parameters[254] = 1;
				PhotonNetwork.NetworkingClient.OnEvent(eventData);
				return true;
			}
			else
			{
				if (!PhotonNetwork.InRoom || eventCode >= 200)
				{
					Debug.LogWarning("RaiseEvent(" + eventCode.ToString() + ") failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
					return false;
				}
				return PhotonNetwork.NetworkingClient.OpRaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
			}
		}

		private static bool RaiseEventInternal(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
		{
			if (PhotonNetwork.offlineMode)
			{
				return false;
			}
			if (!PhotonNetwork.InRoom)
			{
				Debug.LogWarning("RaiseEvent(" + eventCode.ToString() + ") failed. Your event is not being sent! Check if your are in a Room");
				return false;
			}
			return PhotonNetwork.NetworkingClient.OpRaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
		}

		public static bool AllocateViewID(PhotonView view)
		{
			if (view.ViewID != 0)
			{
				Debug.LogError("AllocateViewID() can't be used for PhotonViews that already have a viewID. This view is: " + view.ToString());
				return false;
			}
			int viewID = PhotonNetwork.AllocateViewID(PhotonNetwork.LocalPlayer.ActorNumber);
			view.ViewID = viewID;
			return true;
		}

		[Obsolete("Renamed. Use AllocateRoomViewID instead")]
		public static bool AllocateSceneViewID(PhotonView view)
		{
			return PhotonNetwork.AllocateRoomViewID(view);
		}

		public static bool AllocateRoomViewID(PhotonView view)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				Debug.LogError("Only the Master Client can AllocateRoomViewID(). Check PhotonNetwork.IsMasterClient!");
				return false;
			}
			if (view.ViewID != 0)
			{
				Debug.LogError("AllocateRoomViewID() can't be used for PhotonViews that already have a viewID. This view is: " + view.ToString());
				return false;
			}
			int viewID = PhotonNetwork.AllocateViewID(0);
			view.ViewID = viewID;
			return true;
		}

		public static int AllocateViewID(bool roomObject)
		{
			if (roomObject && !PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				Debug.LogError("Only a Master Client can AllocateViewID() for room objects. This client/player is not a Master Client. Returning an invalid viewID: -1.");
				return 0;
			}
			return PhotonNetwork.AllocateViewID(roomObject ? 0 : PhotonNetwork.LocalPlayer.ActorNumber);
		}

		public static int AllocateViewID(int ownerId)
		{
			if (ownerId == 0)
			{
				int num = PhotonNetwork.lastUsedViewSubIdStatic;
				int num2 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
				for (int i = 1; i < PhotonNetwork.MAX_VIEW_IDS; i++)
				{
					num = (num + 1) % PhotonNetwork.MAX_VIEW_IDS;
					if (num != 0)
					{
						int num3 = num + num2;
						if (!PhotonNetwork.photonViewList.ContainsKey(num3))
						{
							PhotonNetwork.lastUsedViewSubIdStatic = num;
							return num3;
						}
					}
				}
				throw new Exception(string.Format("AllocateViewID() failed. The room (user {0}) is out of 'room' viewIDs. It seems all available are in use.", ownerId));
			}
			int num4 = PhotonNetwork.lastUsedViewSubId;
			int num5 = ownerId * PhotonNetwork.MAX_VIEW_IDS;
			for (int j = 1; j <= PhotonNetwork.MAX_VIEW_IDS; j++)
			{
				num4 = (num4 + 1) % PhotonNetwork.MAX_VIEW_IDS;
				if (num4 != 0)
				{
					int num6 = num4 + num5;
					if (!PhotonNetwork.photonViewList.ContainsKey(num6))
					{
						PhotonNetwork.lastUsedViewSubId = num4;
						return num6;
					}
				}
			}
			throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of viewIDs. It seems all available are in use.", ownerId));
		}

		public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				Debug.LogError("Can not Instantiate before the client joined/created a room. State: " + PhotonNetwork.NetworkClientState.ToString());
				return null;
			}
			return PhotonNetwork.NetworkInstantiate(new InstantiateParameters(prefabName, position, rotation, group, data, PhotonNetwork.currentLevelPrefix, null, PhotonNetwork.LocalPlayer, PhotonNetwork.ServerTimestamp), false, false);
		}

		[Obsolete("Renamed. Use InstantiateRoomObject instead")]
		public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
		{
			return PhotonNetwork.InstantiateRoomObject(prefabName, position, rotation, group, data);
		}

		public static GameObject InstantiateRoomObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
		{
			if (PhotonNetwork.CurrentRoom == null)
			{
				Debug.LogError("Can not Instantiate before the client joined/created a room.");
				return null;
			}
			if (PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				return PhotonNetwork.NetworkInstantiate(new InstantiateParameters(prefabName, position, rotation, group, data, PhotonNetwork.currentLevelPrefix, null, PhotonNetwork.LocalPlayer, PhotonNetwork.ServerTimestamp), true, false);
			}
			return null;
		}

		private static GameObject NetworkInstantiate(Hashtable networkEvent, Player creator)
		{
			if (networkEvent == null)
			{
				return null;
			}
			string text = (string)networkEvent[PhotonNetwork.keyByteZero];
			if (text == null)
			{
				return null;
			}
			int timestamp = (int)networkEvent[PhotonNetwork.keyByteSix];
			int num = (int)networkEvent[PhotonNetwork.keyByteSeven];
			Vector3 position;
			if (networkEvent.ContainsKey(PhotonNetwork.keyByteOne))
			{
				position = (Vector3)networkEvent[PhotonNetwork.keyByteOne];
			}
			else
			{
				position = Vector3.zero;
			}
			Quaternion rotation = Quaternion.identity;
			if (networkEvent.ContainsKey(PhotonNetwork.keyByteTwo))
			{
				rotation = (Quaternion)networkEvent[PhotonNetwork.keyByteTwo];
			}
			byte b = 0;
			if (networkEvent.ContainsKey(PhotonNetwork.keyByteThree))
			{
				b = (byte)networkEvent[PhotonNetwork.keyByteThree];
			}
			byte objLevelPrefix = 0;
			if (networkEvent.ContainsKey(PhotonNetwork.keyByteEight))
			{
				objLevelPrefix = (byte)networkEvent[PhotonNetwork.keyByteEight];
			}
			int[] viewIDs;
			if (networkEvent.ContainsKey(PhotonNetwork.keyByteFour))
			{
				viewIDs = (int[])networkEvent[PhotonNetwork.keyByteFour];
			}
			else
			{
				viewIDs = new int[]
				{
					num
				};
			}
			object[] data;
			if (networkEvent.ContainsKey(PhotonNetwork.keyByteFive))
			{
				data = (object[])networkEvent[PhotonNetwork.keyByteFive];
			}
			else
			{
				data = null;
			}
			if (b != 0 && !PhotonNetwork.allowedReceivingGroups.Contains(b))
			{
				return null;
			}
			return PhotonNetwork.NetworkInstantiate(new InstantiateParameters(text, position, rotation, b, data, objLevelPrefix, viewIDs, creator, timestamp), false, true);
		}

		private static GameObject NetworkInstantiate(InstantiateParameters parameters, bool roomObject = false, bool instantiateEvent = false)
		{
			GameObject gameObject = null;
			bool flag = !instantiateEvent && PhotonNetwork.LocalPlayer.Equals(parameters.creator);
			IPunPrefabPoolVerify punPrefabPoolVerify = PhotonNetwork.prefabPool as IPunPrefabPoolVerify;
			bool flag2 = punPrefabPoolVerify != null;
			if (!flag && flag2)
			{
				Vector3 position = parameters.position;
				Quaternion rotation = parameters.rotation;
				GameObject prefab;
				if (punPrefabPoolVerify.VerifyInstantiation(parameters.creator, parameters.prefabName, position, rotation, parameters.viewIDs, out prefab))
				{
					gameObject = punPrefabPoolVerify.Instantiate(prefab, position, rotation);
				}
			}
			else
			{
				gameObject = PhotonNetwork.prefabPool.Instantiate(parameters.prefabName, parameters.position, parameters.rotation);
			}
			if (gameObject == null)
			{
				return null;
			}
			if (gameObject.activeSelf)
			{
				Debug.LogWarning("PrefabPool.Instantiate() should return an inactive GameObject. " + PhotonNetwork.prefabPool.GetType().Name + " returned an active object. PrefabId: " + parameters.prefabName);
			}
			PhotonView[] photonViewsInChildren = gameObject.GetPhotonViewsInChildren();
			if (photonViewsInChildren.Length == 0)
			{
				Debug.LogError("PhotonNetwork.Instantiate() can only instantiate objects with a PhotonView component. This prefab does not have one: " + parameters.prefabName);
				return null;
			}
			int[] array;
			if (flag)
			{
				array = new int[photonViewsInChildren.Length];
				parameters.viewIDs = array;
			}
			else
			{
				array = parameters.viewIDs;
				if (!flag2 && (array == null || array.Length != photonViewsInChildren.Length))
				{
					PhotonNetwork.prefabPool.Destroy(gameObject);
					return null;
				}
			}
			for (int i = 0; i < photonViewsInChildren.Length; i++)
			{
				if (flag)
				{
					array[i] = (roomObject ? PhotonNetwork.AllocateViewID(0) : PhotonNetwork.AllocateViewID(parameters.creator.ActorNumber));
				}
				PhotonView photonView = photonViewsInChildren[i];
				photonView.ViewID = 0;
				photonView.sceneViewId = 0;
				photonView.isRuntimeInstantiated = true;
				photonView.lastOnSerializeDataSent = null;
				photonView.lastOnSerializeDataReceived = null;
				photonView.Prefix = (int)parameters.objLevelPrefix;
				photonView.InstantiationId = array[0];
				photonView.InstantiationData = parameters.data;
				photonView.ViewID = array[i];
				photonView.Group = parameters.group;
			}
			if (flag)
			{
				PhotonNetwork.SendInstantiate(parameters, roomObject);
			}
			gameObject.SetActive(true);
			if (!PhotonNetwork.PrefabsWithoutMagicCallback.Contains(parameters.prefabName))
			{
				IPunInstantiateMagicCallback[] components = gameObject.GetComponents<IPunInstantiateMagicCallback>();
				if (components.Length != 0)
				{
					PhotonMessageInfo info = new PhotonMessageInfo(parameters.creator, parameters.timestamp, photonViewsInChildren[0]);
					IPunInstantiateMagicCallback[] array2 = components;
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j].OnPhotonInstantiate(info);
					}
				}
				else
				{
					PhotonNetwork.PrefabsWithoutMagicCallback.Add(parameters.prefabName);
				}
			}
			return gameObject;
		}

		internal static bool SendInstantiate(InstantiateParameters parameters, bool roomObject = false)
		{
			int num = parameters.viewIDs[0];
			PhotonNetwork.SendInstantiateEvHashtable.Clear();
			PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteZero] = parameters.prefabName;
			if (parameters.position != Vector3.zero)
			{
				PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteOne] = parameters.position;
			}
			if (parameters.rotation != Quaternion.identity)
			{
				PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteTwo] = parameters.rotation;
			}
			if (parameters.group != 0)
			{
				PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteThree] = parameters.group;
			}
			if (parameters.viewIDs.Length > 1)
			{
				PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteFour] = parameters.viewIDs;
			}
			if (parameters.data != null)
			{
				PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteFive] = parameters.data;
			}
			if (PhotonNetwork.currentLevelPrefix > 0)
			{
				PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteEight] = PhotonNetwork.currentLevelPrefix;
			}
			PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteSix] = PhotonNetwork.ServerTimestamp;
			PhotonNetwork.SendInstantiateEvHashtable[PhotonNetwork.keyByteSeven] = num;
			PhotonNetwork.SendInstantiateRaiseEventOptions.CachingOption = (roomObject ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache);
			return PhotonNetwork.RaiseEventInternal(202, PhotonNetwork.SendInstantiateEvHashtable, PhotonNetwork.SendInstantiateRaiseEventOptions, SendOptions.SendReliable);
		}

		public static void Destroy(PhotonView targetView)
		{
			if (targetView != null)
			{
				PhotonNetwork.RemoveInstantiatedGO(targetView.gameObject, !PhotonNetwork.InRoom);
				return;
			}
			Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
		}

		public static void Destroy(GameObject targetGo)
		{
			PhotonNetwork.RemoveInstantiatedGO(targetGo, !PhotonNetwork.InRoom);
		}

		public static void DestroyPlayerObjects(Player targetPlayer)
		{
			if (targetPlayer == null)
			{
				Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
			}
			PhotonNetwork.DestroyPlayerObjects(targetPlayer.ActorNumber);
		}

		public static void DestroyPlayerObjects(int targetPlayerId)
		{
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return;
			}
			if (PhotonNetwork.LocalPlayer.IsMasterClient || targetPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				PhotonNetwork.DestroyPlayerObjects(targetPlayerId, false);
				return;
			}
			Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + PhotonNetwork.IsMasterClient.ToString());
		}

		public static void DestroyAll()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonNetwork.DestroyAll(false);
				return;
			}
			Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
		}

		public static void RemoveRPCs(Player targetPlayer)
		{
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return;
			}
			if (!targetPlayer.IsLocal && !PhotonNetwork.IsMasterClient)
			{
				Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
				return;
			}
			PhotonNetwork.OpCleanActorRpcBuffer(targetPlayer.ActorNumber);
		}

		public static void RemoveRPCs(PhotonView targetPhotonView)
		{
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return;
			}
			PhotonNetwork.CleanRpcBufferIfMine(targetPhotonView);
		}

		internal static void RPC(PhotonView view, string methodName, RpcTarget target, bool encrypt, params object[] parameters)
		{
			if (string.IsNullOrEmpty(methodName))
			{
				Debug.LogError("RPC method name cannot be null or empty.");
				return;
			}
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return;
			}
			if (PhotonNetwork.CurrentRoom == null)
			{
				Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
				return;
			}
			if (PhotonNetwork.NetworkingClient != null)
			{
				PhotonNetwork.RPC(view, methodName, target, null, encrypt, parameters);
				return;
			}
			Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}

		internal static void RPC(PhotonView view, string methodName, Player targetPlayer, bool encrypt, params object[] parameters)
		{
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return;
			}
			if (PhotonNetwork.CurrentRoom == null)
			{
				Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
				return;
			}
			if (PhotonNetwork.LocalPlayer == null)
			{
				Debug.LogError("RPC can't be sent to target Player being null! Did not send \"" + methodName + "\" call.");
			}
			if (PhotonNetwork.NetworkingClient != null)
			{
				PhotonNetwork.RPC(view, methodName, RpcTarget.Others, targetPlayer, encrypt, parameters);
				return;
			}
			Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
		}

		public static HashSet<GameObject> FindGameObjectsWithComponent(Type type)
		{
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			Component[] array = (Component[])Object.FindObjectsOfType(type);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					hashSet.Add(array[i].gameObject);
				}
			}
			return hashSet;
		}

		public static void SetInterestGroups(byte group, bool enabled)
		{
			if (!PhotonNetwork.VerifyCanUseNetwork())
			{
				return;
			}
			if (enabled)
			{
				byte[] enableGroups = new byte[]
				{
					group
				};
				PhotonNetwork.SetInterestGroups(null, enableGroups);
				return;
			}
			PhotonNetwork.SetInterestGroups(new byte[]
			{
				group
			}, null);
		}

		public static void LoadLevel(int levelNumber)
		{
			if (ConnectionHandler.AppQuits)
			{
				return;
			}
			if (PhotonNetwork.AutomaticallySyncScene)
			{
				PhotonNetwork.SetLevelInPropsIfSynced(levelNumber);
			}
			PhotonNetwork.IsMessageQueueRunning = false;
			PhotonNetwork.loadingLevelAndPausedNetwork = true;
			PhotonNetwork._AsyncLevelLoadingOperation = SceneManager.LoadSceneAsync(levelNumber, LoadSceneMode.Single);
		}

		public static void LoadLevel(string levelName)
		{
			if (ConnectionHandler.AppQuits)
			{
				return;
			}
			if (PhotonNetwork.AutomaticallySyncScene)
			{
				PhotonNetwork.SetLevelInPropsIfSynced(levelName);
			}
			PhotonNetwork.IsMessageQueueRunning = false;
			PhotonNetwork.loadingLevelAndPausedNetwork = true;
			PhotonNetwork._AsyncLevelLoadingOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
		}

		public static bool WebRpc(string name, object parameters, bool sendAuthCookie = false)
		{
			return PhotonNetwork.NetworkingClient.OpWebRpc(name, parameters, sendAuthCookie);
		}

		private static void SetupLogging()
		{
			if (PhotonNetwork.LogLevel == PunLogLevel.ErrorsOnly)
			{
				PhotonNetwork.LogLevel = PhotonNetwork.PhotonServerSettings.PunLogging;
			}
			if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.DebugOut == DebugLevel.ERROR)
			{
				PhotonNetwork.NetworkingClient.LoadBalancingPeer.DebugOut = PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging;
			}
		}

		public static void LoadOrCreateSettings(bool reload = false)
		{
			if (reload)
			{
				PhotonNetwork.photonServerSettings = null;
			}
			else if (PhotonNetwork.photonServerSettings != null)
			{
				Debug.LogWarning("photonServerSettings is not null. Will not LoadOrCreateSettings().");
				return;
			}
			PhotonNetwork.photonServerSettings = (ServerSettings)Resources.Load("PhotonServerSettings", typeof(ServerSettings));
			if (PhotonNetwork.photonServerSettings != null)
			{
				return;
			}
			if (PhotonNetwork.photonServerSettings == null)
			{
				PhotonNetwork.photonServerSettings = (ServerSettings)ScriptableObject.CreateInstance("ServerSettings");
				if (PhotonNetwork.photonServerSettings == null)
				{
					Debug.LogError("Failed to create ServerSettings. PUN is unable to run this way. If you deleted it from the project, reload the Editor.");
					return;
				}
			}
		}

		[Obsolete("Use PhotonViewCollection instead for an iterable collection of current photonViews.")]
		public static PhotonView[] PhotonViews
		{
			get
			{
				PhotonView[] array = new PhotonView[PhotonNetwork.photonViewList.Count];
				int num = 0;
				foreach (PhotonView photonView in PhotonNetwork.photonViewList.Values)
				{
					array[num] = photonView;
					num++;
				}
				return array;
			}
		}

		public static NonAllocDictionary<int, PhotonView>.ValueIterator PhotonViewCollection
		{
			get
			{
				return PhotonNetwork.photonViewList.Values;
			}
		}

		public static int ViewCount
		{
			get
			{
				return PhotonNetwork.photonViewList.Count;
			}
		}

		private static event Action<PhotonView, Player> OnOwnershipRequestEv;

		private static event Action<PhotonView, Player> OnOwnershipTransferedEv;

		private static event Action<PhotonView, Player> OnOwnershipTransferFailedEv;

		public static void AddCallbackTarget(object target)
		{
			if (target is PhotonView)
			{
				return;
			}
			IPunOwnershipCallbacks punOwnershipCallbacks = target as IPunOwnershipCallbacks;
			if (punOwnershipCallbacks != null)
			{
				PhotonNetwork.OnOwnershipRequestEv += punOwnershipCallbacks.OnOwnershipRequest;
				PhotonNetwork.OnOwnershipTransferedEv += punOwnershipCallbacks.OnOwnershipTransfered;
				PhotonNetwork.OnOwnershipTransferFailedEv += punOwnershipCallbacks.OnOwnershipTransferFailed;
			}
			PhotonNetwork.NetworkingClient.AddCallbackTarget(target);
		}

		public static void RemoveCallbackTarget(object target)
		{
			if (target is PhotonView || PhotonNetwork.NetworkingClient == null)
			{
				return;
			}
			IPunOwnershipCallbacks punOwnershipCallbacks = target as IPunOwnershipCallbacks;
			if (punOwnershipCallbacks != null)
			{
				PhotonNetwork.OnOwnershipRequestEv -= punOwnershipCallbacks.OnOwnershipRequest;
				PhotonNetwork.OnOwnershipTransferedEv -= punOwnershipCallbacks.OnOwnershipTransfered;
				PhotonNetwork.OnOwnershipTransferFailedEv -= punOwnershipCallbacks.OnOwnershipTransferFailed;
			}
			PhotonNetwork.NetworkingClient.RemoveCallbackTarget(target);
		}

		internal static string CallbacksToString()
		{
			string[] value = (from m in PhotonNetwork.NetworkingClient.ConnectionCallbackTargets
			select m.ToString()).ToArray<string>();
			return string.Join(", ", value);
		}

		public static IPunPrefabPool PrefabPool
		{
			get
			{
				return PhotonNetwork.prefabPool;
			}
			set
			{
				if (value == null)
				{
					Debug.LogWarning("PhotonNetwork.PrefabPool cannot be set to null. It will default back to using the 'DefaultPool' Pool");
					PhotonNetwork.prefabPool = new DefaultPool();
					return;
				}
				PhotonNetwork.prefabPool = value;
			}
		}

		public static float LevelLoadingProgress
		{
			get
			{
				if (PhotonNetwork._AsyncLevelLoadingOperation != null)
				{
					PhotonNetwork._levelLoadingProgress = PhotonNetwork._AsyncLevelLoadingOperation.progress;
				}
				else if (PhotonNetwork._levelLoadingProgress > 0f)
				{
					PhotonNetwork._levelLoadingProgress = 1f;
				}
				return PhotonNetwork._levelLoadingProgress;
			}
		}

		private static void LeftRoomCleanup()
		{
			if (PhotonNetwork._AsyncLevelLoadingOperation != null)
			{
				PhotonNetwork._AsyncLevelLoadingOperation.allowSceneActivation = false;
				PhotonNetwork._AsyncLevelLoadingOperation = null;
			}
			bool flag = PhotonNetwork.NetworkingClient.CurrentRoom != null && PhotonNetwork.CurrentRoom.AutoCleanUp;
			PhotonNetwork.allowedReceivingGroups = new HashSet<byte>();
			PhotonNetwork.blockedSendingGroups = new HashSet<byte>();
			if (flag || PhotonNetwork.offlineModeRoom != null)
			{
				PhotonNetwork.LocalCleanupAnythingInstantiated(true);
			}
		}

		internal static void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
		{
			if (destroyInstantiatedGameObjects)
			{
				HashSet<GameObject> hashSet = new HashSet<GameObject>();
				foreach (PhotonView photonView in PhotonNetwork.photonViewList.Values)
				{
					if (photonView.isRuntimeInstantiated)
					{
						hashSet.Add(photonView.gameObject);
					}
					else
					{
						photonView.ResetPhotonView(true);
					}
				}
				foreach (GameObject go in hashSet)
				{
					PhotonNetwork.RemoveInstantiatedGO(go, true);
				}
			}
			PhotonNetwork.lastUsedViewSubId = 0;
			PhotonNetwork.lastUsedViewSubIdStatic = 0;
			PhotonNetwork.cachedData.Clear();
		}

		private static void ResetPhotonViewsOnSerialize()
		{
			foreach (PhotonView photonView in PhotonNetwork.photonViewList.Values)
			{
				photonView.lastOnSerializeDataSent = null;
			}
		}

		internal static void ExecuteRpc(Hashtable rpcData, Player sender)
		{
			if (rpcData == null || !rpcData.ContainsKey(PhotonNetwork.keyByteZero))
			{
				return;
			}
			int num = (int)rpcData[PhotonNetwork.keyByteZero];
			int num2 = 0;
			if (rpcData.ContainsKey(PhotonNetwork.keyByteOne))
			{
				num2 = (int)((short)rpcData[PhotonNetwork.keyByteOne]);
			}
			if (!rpcData.ContainsKey(PhotonNetwork.keyByteFive))
			{
				return;
			}
			int num3 = (int)((byte)rpcData[PhotonNetwork.keyByteFive]);
			if (num3 > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
			{
				return;
			}
			string text = PhotonNetwork.PhotonServerSettings.RpcList[num3];
			object[] array = null;
			if (rpcData.ContainsKey(PhotonNetwork.keyByteFour))
			{
				array = (object[])rpcData[PhotonNetwork.keyByteFour];
			}
			PhotonView photonView = PhotonNetwork.GetPhotonView(num);
			if (photonView == null)
			{
				int num4 = num / PhotonNetwork.MAX_VIEW_IDS;
				bool flag = num4 == PhotonNetwork.NetworkingClient.LocalPlayer.ActorNumber;
				if (sender != null)
				{
					bool flag2 = num4 == sender.ActorNumber;
				}
				return;
			}
			if (photonView.Prefix != num2)
			{
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
			{
				Debug.Log("Received RPC: " + text + ". Sender is " + sender.UserId);
			}
			if (photonView.Group != 0 && !PhotonNetwork.allowedReceivingGroups.Contains(photonView.Group))
			{
				return;
			}
			Type[] array2 = null;
			if (array != null && array.Length != 0)
			{
				array2 = new Type[array.Length];
				int num5 = 0;
				foreach (object obj in array)
				{
					if (obj == null)
					{
						array2[num5] = null;
					}
					else
					{
						array2[num5] = obj.GetType();
					}
					num5++;
				}
			}
			int num6 = 0;
			int num7 = 0;
			if (!PhotonNetwork.UseRpcMonoBehaviourCache || photonView.RpcMonoBehaviours == null || photonView.RpcMonoBehaviours.Length == 0)
			{
				photonView.RefreshRpcMonoBehaviourCache();
			}
			for (int j = 0; j < photonView.RpcMonoBehaviours.Length; j++)
			{
				MonoBehaviour monoBehaviour = photonView.RpcMonoBehaviours[j];
				if (!(monoBehaviour == null))
				{
					Type type = monoBehaviour.GetType();
					List<MethodInfo> list = null;
					if (!PhotonNetwork.monoRPCMethodsCache.TryGetValue(type, out list))
					{
						List<MethodInfo> methods = SupportClass.GetMethods(type, PhotonNetwork.typePunRPC);
						PhotonNetwork.monoRPCMethodsCache[type] = methods;
						list = methods;
					}
					if (list != null)
					{
						for (int k = 0; k < list.Count; k++)
						{
							MethodInfo methodInfo = list[k];
							if (methodInfo.Name.Equals(text))
							{
								ParameterInfo[] cachedParemeters = methodInfo.GetCachedParemeters();
								num7++;
								bool flag3 = false;
								int num8 = cachedParemeters.Length;
								if (num8 > 0 && cachedParemeters[num8 - 1].ParameterType == typeof(PhotonMessageInfo))
								{
									num8--;
									flag3 = true;
								}
								if (array == null)
								{
									if (num8 == 0)
									{
										if (!flag3)
										{
											num6++;
											object obj2 = methodInfo.Invoke(monoBehaviour, null);
											IEnumerator routine;
											if (PhotonNetwork.RunRpcCoroutines && (routine = (obj2 as IEnumerator)) != null)
											{
												PhotonHandler.Instance.StartCoroutine(routine);
											}
										}
										else
										{
											int timestamp = (int)rpcData[PhotonNetwork.keyByteTwo];
											num6++;
											object obj3 = methodInfo.Invoke(monoBehaviour, new object[]
											{
												new PhotonMessageInfo(sender, timestamp, photonView)
											});
											IEnumerator routine2;
											if (PhotonNetwork.RunRpcCoroutines && (routine2 = (obj3 as IEnumerator)) != null)
											{
												PhotonHandler.Instance.StartCoroutine(routine2);
											}
										}
									}
								}
								else if (num8 == array.Length && PhotonNetwork.CheckTypeMatch(cachedParemeters, array2))
								{
									object[] parameters = array;
									if (flag3)
									{
										int timestamp2 = (int)rpcData[PhotonNetwork.keyByteTwo];
										object[] array3 = new object[array.Length + 1];
										array.CopyTo(array3, 0);
										array3[array3.Length - 1] = new PhotonMessageInfo(sender, timestamp2, photonView);
										parameters = array3;
									}
									num6++;
									object obj4 = methodInfo.Invoke(monoBehaviour, parameters);
									IEnumerator routine3;
									if (PhotonNetwork.RunRpcCoroutines && (routine3 = (obj4 as IEnumerator)) != null)
									{
										PhotonHandler.Instance.StartCoroutine(routine3);
									}
								}
								else if (cachedParemeters.Length == 1 && cachedParemeters[0].ParameterType.IsArray)
								{
									num6++;
									object obj5 = methodInfo.Invoke(monoBehaviour, new object[]
									{
										array
									});
									IEnumerator routine4;
									if (PhotonNetwork.RunRpcCoroutines && (routine4 = (obj5 as IEnumerator)) != null)
									{
										PhotonHandler.Instance.StartCoroutine(routine4);
									}
								}
							}
						}
					}
				}
			}
			if (num6 != 1)
			{
				string text2 = string.Empty;
				if (array2 != null)
				{
					int num9 = array2.Length;
					foreach (Type type2 in array2)
					{
						if (text2 != string.Empty)
						{
							text2 += ", ";
						}
						if (type2 == null)
						{
							text2 += "null";
						}
						else
						{
							text2 += type2.Name;
						}
					}
				}
				GameObject context = (photonView != null) ? photonView.gameObject : null;
				if (num6 == 0)
				{
					if (num7 == 0)
					{
						Debug.LogErrorFormat(context, "RPC method '{0}({2})' not found on object with PhotonView {1}. Implement as non-static. Apply [PunRPC]. Components on children are not found. Return type must be void or IEnumerator (if you enable RunRpcCoroutines). RPCs are a one-way message.. Sender is " + sender.UserId, new object[]
						{
							text,
							num,
							text2
						});
						return;
					}
					Debug.LogErrorFormat(context, "RPC method '{0}' found on object with PhotonView {1} but has wrong parameters. Implement as '{0}({2})'. PhotonMessageInfo is optional as final parameter.Return type must be void or IEnumerator (if you enable RunRpcCoroutines).. Sender is " + sender.UserId, new object[]
					{
						text,
						num,
						text2
					});
					return;
				}
				else
				{
					Debug.LogErrorFormat(context, "RPC method '{0}({2})' found {3}x on object with PhotonView {1}. Only one component should implement it.Return type must be void or IEnumerator (if you enable RunRpcCoroutines).. Sender is " + sender.UserId, new object[]
					{
						text,
						num,
						text2,
						num7
					});
				}
			}
		}

		private static bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
		{
			if (methodParameters.Length < callParameterTypes.Length)
			{
				return false;
			}
			for (int i = 0; i < callParameterTypes.Length; i++)
			{
				Type parameterType = methodParameters[i].ParameterType;
				if (callParameterTypes[i] != null && !parameterType.IsAssignableFrom(callParameterTypes[i]) && (!parameterType.IsEnum || !Enum.GetUnderlyingType(parameterType).IsAssignableFrom(callParameterTypes[i])))
				{
					return false;
				}
			}
			return true;
		}

		public static void DestroyPlayerObjects(int playerId, bool localOnly)
		{
			if (playerId <= 0)
			{
				Debug.LogError("Failed to Destroy objects of playerId: " + playerId.ToString());
				return;
			}
			if (!localOnly)
			{
				PhotonNetwork.OpRemoveFromServerInstantiationsOfPlayer(playerId);
				PhotonNetwork.OpCleanActorRpcBuffer(playerId);
				PhotonNetwork.SendDestroyOfPlayer(playerId);
			}
			HashSet<GameObject> hashSet = new HashSet<GameObject>();
			foreach (PhotonView photonView in PhotonNetwork.photonViewList.Values)
			{
				if (photonView == null)
				{
					Debug.LogError("Null view");
				}
				else if (photonView.CreatorActorNr == playerId)
				{
					hashSet.Add(photonView.gameObject);
				}
				else if (photonView.OwnerActorNr == playerId)
				{
					Player owner = photonView.Owner;
					photonView.OwnerActorNr = photonView.CreatorActorNr;
					photonView.ControllerActorNr = photonView.CreatorActorNr;
					if (PhotonNetwork.OnOwnershipTransferedEv != null)
					{
						PhotonNetwork.OnOwnershipTransferedEv(photonView, owner);
					}
				}
			}
			foreach (GameObject go in hashSet)
			{
				PhotonNetwork.RemoveInstantiatedGO(go, true);
			}
		}

		public static void DestroyAll(bool localOnly)
		{
			if (!localOnly)
			{
				PhotonNetwork.OpRemoveCompleteCache();
				PhotonNetwork.SendDestroyOfAll();
			}
			PhotonNetwork.LocalCleanupAnythingInstantiated(true);
		}

		public static void RemoveInstantiatedGO(GameObject go, bool localOnly)
		{
			if (ConnectionHandler.AppQuits)
			{
				return;
			}
			if (go == null)
			{
				return;
			}
			go.GetComponentsInChildren<PhotonView>(true, PhotonNetwork.foundPVs);
			if (PhotonNetwork.foundPVs.Count <= 0)
			{
				Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + ((go != null) ? go.ToString() : null));
				return;
			}
			PhotonView photonView = PhotonNetwork.foundPVs[0];
			if (!localOnly && !photonView.IsMine)
			{
				string str = "Failed to 'network-remove' GameObject. Client is neither owner nor MasterClient taking over for owner who left: ";
				PhotonView photonView2 = photonView;
				Debug.LogError(str + ((photonView2 != null) ? photonView2.ToString() : null));
				PhotonNetwork.foundPVs.Clear();
				return;
			}
			if (!localOnly)
			{
				PhotonNetwork.ServerCleanInstantiateAndDestroy(photonView);
			}
			int creatorActorNr = photonView.CreatorActorNr;
			for (int i = PhotonNetwork.foundPVs.Count - 1; i >= 0; i--)
			{
				PhotonView photonView3 = PhotonNetwork.foundPVs[i];
				if (!(photonView3 == null))
				{
					if (i != 0 && photonView3.CreatorActorNr != creatorActorNr)
					{
						photonView3.transform.SetParent(null, true);
					}
					else
					{
						photonView3.OnPreNetDestroy(photonView);
						if (photonView3.InstantiationId >= 1)
						{
							PhotonNetwork.LocalCleanPhotonView(photonView3);
						}
						if (!localOnly)
						{
							PhotonNetwork.OpCleanRpcBuffer(photonView3);
						}
					}
				}
			}
			if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
			{
				Debug.Log("Network destroy Instantiated GO: " + go.name);
			}
			PhotonNetwork.foundPVs.Clear();
			go.SetActive(false);
			PhotonNetwork.prefabPool.Destroy(go);
		}

		private static void ServerCleanInstantiateAndDestroy(PhotonView photonView)
		{
			int num;
			if (photonView.isRuntimeInstantiated)
			{
				num = photonView.InstantiationId;
				PhotonNetwork.removeFilter[PhotonNetwork.keyByteSeven] = num;
				PhotonNetwork.ServerCleanOptions.CachingOption = EventCaching.RemoveFromRoomCache;
				PhotonNetwork.RaiseEventInternal(202, PhotonNetwork.removeFilter, PhotonNetwork.ServerCleanOptions, SendOptions.SendReliable);
			}
			else
			{
				num = photonView.ViewID;
			}
			PhotonNetwork.ServerCleanDestroyEvent[PhotonNetwork.keyByteZero] = num;
			PhotonNetwork.ServerCleanOptions.CachingOption = (photonView.isRuntimeInstantiated ? EventCaching.DoNotCache : EventCaching.AddToRoomCacheGlobal);
			PhotonNetwork.RaiseEventInternal(204, PhotonNetwork.ServerCleanDestroyEvent, PhotonNetwork.ServerCleanOptions, SendOptions.SendReliable);
		}

		private static void SendDestroyOfPlayer(int actorNr)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[PhotonNetwork.keyByteZero] = actorNr;
			PhotonNetwork.RaiseEventInternal(207, hashtable, null, SendOptions.SendReliable);
		}

		private static void SendDestroyOfAll()
		{
			Hashtable hashtable = new Hashtable();
			hashtable[PhotonNetwork.keyByteZero] = -1;
			PhotonNetwork.RaiseEventInternal(207, hashtable, null, SendOptions.SendReliable);
		}

		private static void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				CachingOption = EventCaching.RemoveFromRoomCache,
				TargetActors = new int[]
				{
					actorNr
				}
			};
			PhotonNetwork.RaiseEventInternal(202, null, raiseEventOptions, SendOptions.SendReliable);
		}

		internal static void RequestOwnership(int viewID, int fromOwner)
		{
			PhotonNetwork.RaiseEventInternal(209, new int[]
			{
				viewID,
				fromOwner
			}, PhotonNetwork.SendToAllOptions, SendOptions.SendReliable);
		}

		internal static void TransferOwnership(int viewID, int playerID)
		{
			PhotonNetwork.RaiseEventInternal(210, new int[]
			{
				viewID,
				playerID
			}, PhotonNetwork.SendToAllOptions, SendOptions.SendReliable);
		}

		internal static void OwnershipUpdate(int[] viewOwnerPairs, int targetActor = -1)
		{
			RaiseEventOptions raiseEventOptions;
			if (targetActor == -1)
			{
				raiseEventOptions = PhotonNetwork.SendToOthersOptions;
			}
			else
			{
				PhotonNetwork.SendToSingleOptions.TargetActors[0] = targetActor;
				raiseEventOptions = PhotonNetwork.SendToSingleOptions;
			}
			PhotonNetwork.RaiseEventInternal(212, viewOwnerPairs, raiseEventOptions, SendOptions.SendReliable);
		}

		public static bool LocalCleanPhotonView(PhotonView view)
		{
			view.removedFromLocalViewList = true;
			return PhotonNetwork.photonViewList.Remove(view.ViewID);
		}

		public static PhotonView GetPhotonView(int viewID)
		{
			PhotonView result = null;
			PhotonNetwork.photonViewList.TryGetValue(viewID, out result);
			return result;
		}

		public static bool ViewIDExists(int viewID)
		{
			return PhotonNetwork.photonViewList.ContainsKey(viewID);
		}

		public static void RegisterPhotonView(PhotonView netView)
		{
			if (!Application.isPlaying)
			{
				PhotonNetwork.photonViewList = new NonAllocDictionary<int, PhotonView>(29U);
				return;
			}
			if (netView.ViewID == 0)
			{
				Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + ((netView != null) ? netView.ToString() : null));
				return;
			}
			PhotonView photonView = null;
			if (PhotonNetwork.photonViewList.TryGetValue(netView.ViewID, out photonView))
			{
				if (!(netView != photonView))
				{
					return;
				}
				PhotonNetwork.RemoveInstantiatedGO(photonView.gameObject, true);
			}
			PhotonNetwork.photonViewList[netView.ViewID] = netView;
			netView.removedFromLocalViewList = false;
			if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
			{
				Debug.Log("Registered PhotonView: " + netView.ViewID.ToString());
			}
			Dictionary<int, Queue<object[]>> dictionary;
			Queue<object[]> queue;
			if (PhotonNetwork.cachedData.TryGetValue(netView.CreatorActorNr, out dictionary) && dictionary.TryGetValue(netView.ViewID, out queue))
			{
				dictionary.Remove(netView.ViewID);
				foreach (object[] array in queue)
				{
					Debug.LogWarning("Received OnSerialization for view ID " + netView.ViewID.ToString() + ". Found Cached data! Dumping cached state");
					PhotonNetwork.OnSerializeRead((object[])array[0], (Player)array[1], (int)array[2], (short)array[3]);
				}
			}
		}

		public static void OpCleanActorRpcBuffer(int actorNumber)
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				CachingOption = EventCaching.RemoveFromRoomCache,
				TargetActors = new int[]
				{
					actorNumber
				}
			};
			PhotonNetwork.RaiseEventInternal(200, null, raiseEventOptions, SendOptions.SendReliable);
		}

		public static void OpRemoveCompleteCacheOfPlayer(int actorNumber)
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				CachingOption = EventCaching.RemoveFromRoomCache,
				TargetActors = new int[]
				{
					actorNumber
				}
			};
			PhotonNetwork.RaiseEventInternal(0, null, raiseEventOptions, SendOptions.SendReliable);
		}

		public static void OpRemoveCompleteCache()
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions
			{
				CachingOption = EventCaching.RemoveFromRoomCache,
				Receivers = ReceiverGroup.MasterClient
			};
			PhotonNetwork.RaiseEventInternal(0, null, raiseEventOptions, SendOptions.SendReliable);
		}

		private static void RemoveCacheOfLeftPlayers()
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[244] = 0;
			dictionary[247] = 7;
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOperation(253, dictionary, SendOptions.SendReliable);
		}

		public static void CleanRpcBufferIfMine(PhotonView view)
		{
			if (view.OwnerActorNr != PhotonNetwork.NetworkingClient.LocalPlayer.ActorNumber && !PhotonNetwork.NetworkingClient.LocalPlayer.IsMasterClient)
			{
				string str = "Cannot remove cached RPCs on a PhotonView thats not ours! ";
				Player owner = view.Owner;
				Debug.LogError(str + ((owner != null) ? owner.ToString() : null) + " scene: " + view.IsRoomView.ToString());
				return;
			}
			PhotonNetwork.OpCleanRpcBuffer(view);
		}

		public static void OpCleanRpcBuffer(PhotonView view)
		{
			PhotonNetwork.rpcFilterByViewId[PhotonNetwork.keyByteZero] = view.ViewID;
			PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcFilterByViewId, PhotonNetwork.OpCleanRpcBufferOptions, SendOptions.SendReliable);
		}

		public static void RemoveRPCsInGroup(int group)
		{
			foreach (PhotonView photonView in PhotonNetwork.photonViewList.Values)
			{
				if ((int)photonView.Group == group)
				{
					PhotonNetwork.CleanRpcBufferIfMine(photonView);
				}
			}
		}

		public static bool RemoveBufferedRPCs(int viewId = 0, string methodName = null, int[] callersActorNumbers = null)
		{
			Hashtable hashtable = new Hashtable(2);
			if (viewId != 0)
			{
				hashtable[PhotonNetwork.keyByteZero] = viewId;
			}
			if (!string.IsNullOrEmpty(methodName))
			{
				int num;
				if (PhotonNetwork.rpcShortcuts.TryGetValue(methodName, out num))
				{
					hashtable[PhotonNetwork.keyByteFive] = (byte)num;
				}
				else
				{
					hashtable[PhotonNetwork.keyByteThree] = methodName;
				}
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.CachingOption = EventCaching.RemoveFromRoomCache;
			if (callersActorNumbers != null)
			{
				raiseEventOptions.TargetActors = callersActorNumbers;
			}
			return PhotonNetwork.RaiseEventInternal(200, hashtable, raiseEventOptions, SendOptions.SendReliable);
		}

		public static void SetLevelPrefix(byte prefix)
		{
			PhotonNetwork.currentLevelPrefix = prefix;
		}

		internal static void RPC(PhotonView view, string methodName, RpcTarget target, Player player, bool encrypt, params object[] parameters)
		{
			if (PhotonNetwork.blockedSendingGroups.Contains(view.Group))
			{
				return;
			}
			if (view.ViewID < 1)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Illegal view ID:",
					view.ViewID.ToString(),
					" method: ",
					methodName,
					" GO:",
					view.gameObject.name
				}));
			}
			if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Sending RPC \"",
					methodName,
					"\" to target: ",
					target.ToString(),
					" or player:",
					(player != null) ? player.ToString() : null,
					"."
				}));
			}
			PhotonNetwork.rpcEvent.Clear();
			PhotonNetwork.rpcEvent[PhotonNetwork.keyByteZero] = view.ViewID;
			if (view.Prefix > 0)
			{
				PhotonNetwork.rpcEvent[PhotonNetwork.keyByteOne] = (short)view.Prefix;
			}
			PhotonNetwork.rpcEvent[PhotonNetwork.keyByteTwo] = PhotonNetwork.ServerTimestamp;
			int num = 0;
			if (PhotonNetwork.rpcShortcuts.TryGetValue(methodName, out num))
			{
				PhotonNetwork.rpcEvent[PhotonNetwork.keyByteFive] = (byte)num;
			}
			else
			{
				PhotonNetwork.rpcEvent[PhotonNetwork.keyByteThree] = methodName;
			}
			if (parameters != null && parameters.Length != 0)
			{
				PhotonNetwork.rpcEvent[PhotonNetwork.keyByteFour] = parameters;
			}
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true,
				Encrypt = encrypt
			};
			if (player == null)
			{
				switch (target)
				{
				case RpcTarget.All:
					PhotonNetwork.RpcOptionsToAll.InterestGroup = view.Group;
					PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, PhotonNetwork.RpcOptionsToAll, sendOptions);
					PhotonNetwork.ExecuteRpc(PhotonNetwork.rpcEvent, PhotonNetwork.NetworkingClient.LocalPlayer);
					return;
				case RpcTarget.Others:
				{
					RaiseEventOptions raiseEventOptions = new RaiseEventOptions
					{
						InterestGroup = view.Group
					};
					PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, raiseEventOptions, sendOptions);
					return;
				}
				case RpcTarget.MasterClient:
				{
					if (PhotonNetwork.NetworkingClient.LocalPlayer.IsMasterClient)
					{
						PhotonNetwork.ExecuteRpc(PhotonNetwork.rpcEvent, PhotonNetwork.NetworkingClient.LocalPlayer);
						return;
					}
					RaiseEventOptions raiseEventOptions2 = new RaiseEventOptions
					{
						Receivers = ReceiverGroup.MasterClient
					};
					PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, raiseEventOptions2, sendOptions);
					return;
				}
				case RpcTarget.AllBuffered:
				{
					RaiseEventOptions raiseEventOptions3 = new RaiseEventOptions
					{
						CachingOption = EventCaching.AddToRoomCache
					};
					PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, raiseEventOptions3, sendOptions);
					PhotonNetwork.ExecuteRpc(PhotonNetwork.rpcEvent, PhotonNetwork.NetworkingClient.LocalPlayer);
					return;
				}
				case RpcTarget.OthersBuffered:
				{
					RaiseEventOptions raiseEventOptions4 = new RaiseEventOptions
					{
						CachingOption = EventCaching.AddToRoomCache
					};
					PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, raiseEventOptions4, sendOptions);
					return;
				}
				case RpcTarget.AllViaServer:
				{
					RaiseEventOptions raiseEventOptions5 = new RaiseEventOptions
					{
						InterestGroup = view.Group,
						Receivers = ReceiverGroup.All
					};
					PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, raiseEventOptions5, sendOptions);
					if (PhotonNetwork.OfflineMode)
					{
						PhotonNetwork.ExecuteRpc(PhotonNetwork.rpcEvent, PhotonNetwork.NetworkingClient.LocalPlayer);
						return;
					}
					break;
				}
				case RpcTarget.AllBufferedViaServer:
				{
					RaiseEventOptions raiseEventOptions6 = new RaiseEventOptions
					{
						InterestGroup = view.Group,
						Receivers = ReceiverGroup.All,
						CachingOption = EventCaching.AddToRoomCache
					};
					PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, raiseEventOptions6, sendOptions);
					if (PhotonNetwork.OfflineMode)
					{
						PhotonNetwork.ExecuteRpc(PhotonNetwork.rpcEvent, PhotonNetwork.NetworkingClient.LocalPlayer);
						return;
					}
					break;
				}
				default:
					Debug.LogError("Unsupported target enum: " + target.ToString());
					break;
				}
				return;
			}
			if (PhotonNetwork.NetworkingClient.LocalPlayer.ActorNumber == player.ActorNumber)
			{
				PhotonNetwork.ExecuteRpc(PhotonNetwork.rpcEvent, player);
				return;
			}
			RaiseEventOptions raiseEventOptions7 = new RaiseEventOptions
			{
				TargetActors = new int[]
				{
					player.ActorNumber
				}
			};
			PhotonNetwork.RaiseEventInternal(200, PhotonNetwork.rpcEvent, raiseEventOptions7, sendOptions);
		}

		public static void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
		{
			if (disableGroups != null)
			{
				if (disableGroups.Length == 0)
				{
					PhotonNetwork.allowedReceivingGroups.Clear();
				}
				else
				{
					foreach (byte b in disableGroups)
					{
						if (b <= 0)
						{
							Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + b.ToString() + ". The Group number should be at least 1.");
						}
						else if (PhotonNetwork.allowedReceivingGroups.Contains(b))
						{
							PhotonNetwork.allowedReceivingGroups.Remove(b);
						}
					}
				}
			}
			if (enableGroups != null)
			{
				if (enableGroups.Length == 0)
				{
					for (byte b2 = 0; b2 < 255; b2 += 1)
					{
						PhotonNetwork.allowedReceivingGroups.Add(b2);
					}
					PhotonNetwork.allowedReceivingGroups.Add(byte.MaxValue);
				}
				else
				{
					foreach (byte b3 in enableGroups)
					{
						if (b3 <= 0)
						{
							Debug.LogError("Error: PhotonNetwork.SetInterestGroups was called with an illegal group number: " + b3.ToString() + ". The Group number should be at least 1.");
						}
						else
						{
							PhotonNetwork.allowedReceivingGroups.Add(b3);
						}
					}
				}
			}
			if (!PhotonNetwork.offlineMode)
			{
				PhotonNetwork.NetworkingClient.OpChangeGroups(disableGroups, enableGroups);
			}
		}

		public static void SetSendingEnabled(byte group, bool enabled)
		{
			if (!enabled)
			{
				PhotonNetwork.blockedSendingGroups.Add(group);
				return;
			}
			PhotonNetwork.blockedSendingGroups.Remove(group);
		}

		public static void SetSendingEnabled(byte[] disableGroups, byte[] enableGroups)
		{
			if (disableGroups != null)
			{
				foreach (byte item in disableGroups)
				{
					PhotonNetwork.blockedSendingGroups.Add(item);
				}
			}
			if (enableGroups != null)
			{
				foreach (byte item2 in enableGroups)
				{
					PhotonNetwork.blockedSendingGroups.Remove(item2);
				}
			}
		}

		internal static void NewSceneLoaded()
		{
			if (PhotonNetwork.loadingLevelAndPausedNetwork)
			{
				PhotonNetwork._AsyncLevelLoadingOperation = null;
				PhotonNetwork.loadingLevelAndPausedNetwork = false;
				PhotonNetwork.IsMessageQueueRunning = true;
			}
			else
			{
				PhotonNetwork.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
			}
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, PhotonView> keyValuePair in PhotonNetwork.photonViewList)
			{
				if (keyValuePair.Value == null)
				{
					list.Add(keyValuePair.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				int key = list[i];
				PhotonNetwork.photonViewList.Remove(key);
			}
			if (list.Count > 0 && PhotonNetwork.LogLevel >= PunLogLevel.Informational)
			{
				Debug.Log("New level loaded. Removed " + list.Count.ToString() + " scene view IDs from last level.");
			}
		}

		internal static void RunViewUpdate()
		{
			if (PhotonNetwork.OfflineMode || PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
			{
				return;
			}
			if (PhotonNetwork.CurrentRoom.Players.Count <= 1)
			{
				return;
			}
			foreach (KeyValuePair<int, PhotonView> keyValuePair in PhotonNetwork.photonViewList)
			{
				PhotonView value = keyValuePair.Value;
				if (value.Synchronization != ViewSynchronization.Off && value.IsMine && value.isActiveAndEnabled && !PhotonNetwork.blockedSendingGroups.Contains(value.Group))
				{
					List<object> list = PhotonNetwork.OnSerializeWrite(value);
					if (list != null)
					{
						PhotonNetwork.RaiseEventBatch raiseEventBatch = default(PhotonNetwork.RaiseEventBatch);
						raiseEventBatch.Reliable = (value.Synchronization == ViewSynchronization.ReliableDeltaCompressed || value.mixedModeIsReliable);
						raiseEventBatch.Group = value.Group;
						PhotonNetwork.SerializeViewBatch serializeViewBatch = null;
						if (!PhotonNetwork.serializeViewBatches.TryGetValue(raiseEventBatch, out serializeViewBatch))
						{
							serializeViewBatch = new PhotonNetwork.SerializeViewBatch(raiseEventBatch, 2);
							PhotonNetwork.serializeViewBatches.Add(raiseEventBatch, serializeViewBatch);
						}
						serializeViewBatch.Add(list);
						if (serializeViewBatch.ObjectUpdates.Count == serializeViewBatch.ObjectUpdates.Capacity)
						{
							PhotonNetwork.SendSerializeViewBatch(serializeViewBatch);
						}
					}
				}
			}
			foreach (KeyValuePair<PhotonNetwork.RaiseEventBatch, PhotonNetwork.SerializeViewBatch> keyValuePair2 in PhotonNetwork.serializeViewBatches)
			{
				PhotonNetwork.SendSerializeViewBatch(keyValuePair2.Value);
			}
		}

		private static void SendSerializeViewBatch(PhotonNetwork.SerializeViewBatch batch)
		{
			if (batch == null || batch.ObjectUpdates.Count <= 2)
			{
				return;
			}
			PhotonNetwork.serializeRaiseEvOptions.InterestGroup = batch.Batch.Group;
			batch.ObjectUpdates[0] = PhotonNetwork.ServerTimestamp;
			batch.ObjectUpdates[1] = ((PhotonNetwork.currentLevelPrefix != 0) ? PhotonNetwork.currentLevelPrefix : null);
			PhotonNetwork.RaiseEventInternal(batch.Batch.Reliable ? 206 : 201, batch.ObjectUpdates, PhotonNetwork.serializeRaiseEvOptions, batch.Batch.Reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
			batch.Clear();
		}

		private static List<object> OnSerializeWrite(PhotonView view)
		{
			if (view.Synchronization == ViewSynchronization.Off)
			{
				return null;
			}
			PhotonMessageInfo info = new PhotonMessageInfo(PhotonNetwork.NetworkingClient.LocalPlayer, PhotonNetwork.ServerTimestamp, view);
			if (view.syncValues == null)
			{
				view.syncValues = new List<object>();
			}
			view.syncValues.Clear();
			PhotonNetwork.serializeStreamOut.SetWriteStream(view.syncValues, 0);
			PhotonNetwork.serializeStreamOut.SendNext(null);
			PhotonNetwork.serializeStreamOut.SendNext(null);
			PhotonNetwork.serializeStreamOut.SendNext(null);
			view.SerializeView(PhotonNetwork.serializeStreamOut, info);
			if (PhotonNetwork.serializeStreamOut.Count <= 3)
			{
				return null;
			}
			List<object> writeStream = PhotonNetwork.serializeStreamOut.GetWriteStream();
			writeStream[0] = view.ViewID;
			writeStream[1] = false;
			writeStream[2] = null;
			if (view.Synchronization == ViewSynchronization.Unreliable)
			{
				return writeStream;
			}
			if (view.Synchronization == ViewSynchronization.UnreliableOnChange)
			{
				if (PhotonNetwork.AlmostEquals(writeStream, view.lastOnSerializeDataSent))
				{
					if (view.mixedModeIsReliable)
					{
						return null;
					}
					view.mixedModeIsReliable = true;
					List<object> lastOnSerializeDataSent = view.lastOnSerializeDataSent;
					view.lastOnSerializeDataSent = writeStream;
					view.syncValues = lastOnSerializeDataSent;
				}
				else
				{
					view.mixedModeIsReliable = false;
					List<object> lastOnSerializeDataSent2 = view.lastOnSerializeDataSent;
					view.lastOnSerializeDataSent = writeStream;
					view.syncValues = lastOnSerializeDataSent2;
				}
				return writeStream;
			}
			if (view.Synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				List<object> result = PhotonNetwork.DeltaCompressionWrite(view.lastOnSerializeDataSent, writeStream);
				List<object> lastOnSerializeDataSent3 = view.lastOnSerializeDataSent;
				view.lastOnSerializeDataSent = writeStream;
				view.syncValues = lastOnSerializeDataSent3;
				return result;
			}
			return null;
		}

		private static void OnSerializeRead(object[] data, Player sender, int networkTime, short correctPrefix)
		{
			int num = (int)data[0];
			PhotonView photonView = PhotonNetwork.GetPhotonView(num);
			if (photonView == null)
			{
				int key = num / PhotonNetwork.MAX_VIEW_IDS;
				if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.Players.ContainsKey(key))
				{
					Dictionary<int, Queue<object[]>> dictionary;
					if (!PhotonNetwork.cachedData.TryGetValue(key, out dictionary))
					{
						dictionary = new Dictionary<int, Queue<object[]>>(5);
						PhotonNetwork.cachedData[key] = dictionary;
					}
					Queue<object[]> queue;
					if (!dictionary.TryGetValue(num, out queue))
					{
						queue = new Queue<object[]>();
						dictionary.Add(num, queue);
					}
					if (queue.Count < 10)
					{
						queue.Enqueue(new object[]
						{
							data,
							sender,
							networkTime,
							correctPrefix
						});
					}
				}
				return;
			}
			if (photonView.Prefix > 0 && (int)correctPrefix != photonView.Prefix)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Received OnSerialization for view ID ",
					num.ToString(),
					" with prefix ",
					correctPrefix.ToString(),
					". Our prefix is ",
					photonView.Prefix.ToString()
				}));
				return;
			}
			if (photonView.Group != 0 && !PhotonNetwork.allowedReceivingGroups.Contains(photonView.Group))
			{
				return;
			}
			if (photonView.Synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				object[] array = PhotonNetwork.DeltaCompressionRead(photonView.lastOnSerializeDataReceived, data);
				if (array == null)
				{
					if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
					{
						Debug.Log(string.Concat(new string[]
						{
							"Skipping packet for ",
							photonView.name,
							" [",
							photonView.ViewID.ToString(),
							"] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game."
						}));
					}
					return;
				}
				photonView.lastOnSerializeDataReceived = array;
				data = array;
			}
			PhotonNetwork.serializeStreamIn.SetReadStream(data, 3);
			PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, photonView);
			photonView.DeserializeView(PhotonNetwork.serializeStreamIn, info);
		}

		private static List<object> DeltaCompressionWrite(List<object> previousContent, List<object> currentContent)
		{
			if (currentContent == null || previousContent == null || previousContent.Count != currentContent.Count)
			{
				return currentContent;
			}
			if (currentContent.Count <= 3)
			{
				return null;
			}
			previousContent[1] = false;
			int num = 0;
			Queue<int> queue = null;
			for (int i = 3; i < currentContent.Count; i++)
			{
				object obj = currentContent[i];
				object two = previousContent[i];
				if (PhotonNetwork.AlmostEquals(obj, two))
				{
					num++;
					previousContent[i] = null;
				}
				else
				{
					previousContent[i] = obj;
					if (obj == null)
					{
						if (queue == null)
						{
							queue = new Queue<int>(currentContent.Count);
						}
						queue.Enqueue(i);
					}
				}
			}
			if (num > 0)
			{
				if (num == currentContent.Count - 3)
				{
					return null;
				}
				previousContent[1] = true;
				if (queue != null)
				{
					previousContent[2] = queue.ToArray();
				}
			}
			previousContent[0] = currentContent[0];
			return previousContent;
		}

		private static object[] DeltaCompressionRead(object[] lastOnSerializeDataReceived, object[] incomingData)
		{
			if (!(bool)incomingData[1])
			{
				return incomingData;
			}
			if (lastOnSerializeDataReceived == null)
			{
				return null;
			}
			int[] array = incomingData[2] as int[];
			int num = lastOnSerializeDataReceived.Length;
			for (int i = 3; i < incomingData.Length; i++)
			{
				if ((array == null || !array.Contains(i)) && incomingData[i] == null && i < num)
				{
					object obj = lastOnSerializeDataReceived[i];
					incomingData[i] = obj;
				}
			}
			return incomingData;
		}

		private static bool AlmostEquals(IList<object> lastData, IList<object> currentContent)
		{
			if (lastData == null && currentContent == null)
			{
				return true;
			}
			if (lastData == null || currentContent == null || lastData.Count != currentContent.Count)
			{
				return false;
			}
			for (int i = 0; i < currentContent.Count; i++)
			{
				object one = currentContent[i];
				object two = lastData[i];
				if (!PhotonNetwork.AlmostEquals(one, two))
				{
					return false;
				}
			}
			return true;
		}

		private static bool AlmostEquals(object one, object two)
		{
			if (one == null || two == null)
			{
				return one == null && two == null;
			}
			if (!one.Equals(two))
			{
				if (one is Vector3)
				{
					Vector3 target = (Vector3)one;
					Vector3 second = (Vector3)two;
					if (target.AlmostEquals(second, PhotonNetwork.PrecisionForVectorSynchronization))
					{
						return true;
					}
				}
				else if (one is Vector2)
				{
					Vector2 target2 = (Vector2)one;
					Vector2 second2 = (Vector2)two;
					if (target2.AlmostEquals(second2, PhotonNetwork.PrecisionForVectorSynchronization))
					{
						return true;
					}
				}
				else if (one is Quaternion)
				{
					Quaternion target3 = (Quaternion)one;
					Quaternion second3 = (Quaternion)two;
					if (target3.AlmostEquals(second3, PhotonNetwork.PrecisionForQuaternionSynchronization))
					{
						return true;
					}
				}
				else if (one is float)
				{
					float target4 = (float)one;
					float second4 = (float)two;
					if (target4.AlmostEquals(second4, PhotonNetwork.PrecisionForFloatSynchronization))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		internal static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
		{
			mi = null;
			if (monob == null || string.IsNullOrEmpty(methodType))
			{
				return false;
			}
			List<MethodInfo> methods = SupportClass.GetMethods(monob.GetType(), null);
			for (int i = 0; i < methods.Count; i++)
			{
				MethodInfo methodInfo = methods[i];
				if (methodInfo.Name.Equals(methodType))
				{
					mi = methodInfo;
					return true;
				}
			}
			return false;
		}

		internal static void LoadLevelIfSynced()
		{
			if (!PhotonNetwork.AutomaticallySyncScene || PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
			{
				return;
			}
			if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("curScn"))
			{
				return;
			}
			object obj = PhotonNetwork.CurrentRoom.CustomProperties["curScn"];
			if (obj is int)
			{
				if (SceneManagerHelper.ActiveSceneBuildIndex != (int)obj)
				{
					PhotonNetwork.LoadLevel((int)obj);
					return;
				}
			}
			else if (obj is string && SceneManagerHelper.ActiveSceneName != (string)obj)
			{
				PhotonNetwork.LoadLevel((string)obj);
			}
		}

		internal static void SetLevelInPropsIfSynced(object levelId)
		{
			if (!PhotonNetwork.AutomaticallySyncScene || !PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom == null)
			{
				return;
			}
			if (levelId == null)
			{
				Debug.LogError("Parameter levelId can't be null!");
				return;
			}
			if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("curScn"))
			{
				object obj = PhotonNetwork.CurrentRoom.CustomProperties["curScn"];
				if (levelId.Equals(obj))
				{
					return;
				}
				int activeSceneBuildIndex = SceneManagerHelper.ActiveSceneBuildIndex;
				string activeSceneName = SceneManagerHelper.ActiveSceneName;
				if ((levelId.Equals(activeSceneBuildIndex) && obj.Equals(activeSceneName)) || (levelId.Equals(activeSceneName) && obj.Equals(activeSceneBuildIndex)))
				{
					return;
				}
			}
			if (PhotonNetwork._AsyncLevelLoadingOperation != null)
			{
				if (!PhotonNetwork._AsyncLevelLoadingOperation.isDone)
				{
					Debug.LogWarning("PUN cancels an ongoing async level load, as another scene should be loaded. Next scene to load: " + ((levelId != null) ? levelId.ToString() : null));
				}
				PhotonNetwork._AsyncLevelLoadingOperation.allowSceneActivation = false;
				PhotonNetwork._AsyncLevelLoadingOperation = null;
			}
			Hashtable hashtable = new Hashtable();
			if (levelId is int)
			{
				hashtable["curScn"] = (int)levelId;
			}
			else if (levelId is string)
			{
				hashtable["curScn"] = (string)levelId;
			}
			else
			{
				Debug.LogError("Parameter levelId must be int or string!");
			}
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			PhotonNetwork.SendAllOutgoingCommands();
		}

		private static void OnEvent(EventData photonEvent)
		{
			try
			{
				int sender = photonEvent.Sender;
				Player player = null;
				if (sender > 0 && PhotonNetwork.NetworkingClient.CurrentRoom != null)
				{
					player = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(sender, false);
				}
				byte code = photonEvent.Code;
				switch (code)
				{
				case 200:
					PhotonNetwork.ExecuteRpc(photonEvent.CustomData as Hashtable, player);
					break;
				case 201:
				case 206:
				{
					object[] array = (object[])photonEvent[245];
					if (array != null)
					{
						int networkTime = (int)array[0];
						short correctPrefix = (short)((array[1] != null) ? ((byte)array[1]) : 0);
						for (int i = 2; i < array.Length; i++)
						{
							object[] array2 = array[i] as object[];
							if (array2 == null || array2.Length < 4)
							{
								break;
							}
							PhotonNetwork.OnSerializeRead(array2, player, networkTime, correctPrefix);
						}
					}
					break;
				}
				case 202:
					PhotonNetwork.NetworkInstantiate((Hashtable)photonEvent.CustomData, player);
					break;
				case 203:
					break;
				case 204:
					if (photonEvent.CustomData is Hashtable)
					{
						Hashtable hashtable = (Hashtable)photonEvent.CustomData;
						if (hashtable != null)
						{
							int key = (int)hashtable[PhotonNetwork.keyByteZero];
							PhotonView photonView = null;
							if (PhotonNetwork.photonViewList.TryGetValue(key, out photonView))
							{
								PhotonNetwork.RemoveInstantiatedGO(photonView.gameObject, true);
							}
						}
					}
					break;
				case 205:
				case 208:
				case 209:
				case 210:
				case 211:
				case 212:
					break;
				case 207:
					if (photonEvent.CustomData is Hashtable)
					{
						Hashtable hashtable = (Hashtable)photonEvent.CustomData;
						if (hashtable != null)
						{
							int num = (int)hashtable[PhotonNetwork.keyByteZero];
							if (num >= 0)
							{
								PhotonNetwork.DestroyPlayerObjects(num, true);
							}
							else
							{
								PhotonNetwork.DestroyAll(true);
							}
						}
					}
					break;
				default:
					if (code != 254)
					{
						if (code == 255)
						{
							PhotonNetwork.ResetPhotonViewsOnSerialize();
						}
					}
					else
					{
						if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.AutoCleanUp && (player == null || !player.IsInactive))
						{
							PhotonNetwork.DestroyPlayerObjects(sender, true);
						}
						if (PhotonNetwork.cachedData.ContainsKey(sender))
						{
							PhotonNetwork.cachedData.Remove(sender);
						}
					}
					break;
				}
			}
			catch (Exception arg)
			{
				Action<EventData, Exception> internalEventError = PhotonNetwork.InternalEventError;
				if (internalEventError != null)
				{
					internalEventError(photonEvent, arg);
				}
			}
		}

		private static void OnOperation(OperationResponse opResponse)
		{
			byte operationCode = opResponse.OperationCode;
			if (operationCode != 220)
			{
				if (operationCode != 226)
				{
					return;
				}
				if (PhotonNetwork.Server == ServerConnection.GameServer)
				{
					PhotonNetwork.LoadLevelIfSynced();
				}
			}
			else
			{
				if (opResponse.ReturnCode != 0)
				{
					if (PhotonNetwork.LogLevel >= PunLogLevel.Full)
					{
						Debug.Log("OpGetRegions failed. Will not ping any. ReturnCode: " + opResponse.ReturnCode.ToString());
					}
					return;
				}
				if (PhotonNetwork.ConnectMethod == ConnectMethod.ConnectToBest)
				{
					string bestRegionSummaryInPreferences = PhotonNetwork.BestRegionSummaryInPreferences;
					if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
					{
						Debug.Log("PUN got region list. Going to ping minimum regions, based on this previous result summary: " + bestRegionSummaryInPreferences);
					}
					PhotonNetwork.NetworkingClient.RegionHandler.PingMinimumOfRegions(new Action<RegionHandler>(PhotonNetwork.OnRegionsPinged), bestRegionSummaryInPreferences);
					return;
				}
			}
		}

		private static void OnClientStateChanged(ClientState previousState, ClientState state)
		{
			if ((previousState == ClientState.Joined && state == ClientState.Disconnected) || (PhotonNetwork.Server == ServerConnection.GameServer && (state == ClientState.Disconnecting || state == ClientState.DisconnectingFromGameServer)))
			{
				PhotonNetwork.LeftRoomCleanup();
			}
			if (state == ClientState.ConnectedToMasterServer && PhotonNetwork._cachedRegionHandler != null)
			{
				PhotonNetwork.BestRegionSummaryInPreferences = PhotonNetwork._cachedRegionHandler.SummaryToCache;
				PhotonNetwork._cachedRegionHandler = null;
			}
		}

		private static void OnRegionsPinged(RegionHandler regionHandler)
		{
			if (PhotonNetwork.LogLevel >= PunLogLevel.Informational)
			{
				Debug.Log(regionHandler.GetResults());
			}
			PhotonNetwork._cachedRegionHandler = regionHandler;
			if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToNameServer)
			{
				PhotonNetwork.NetworkingClient.ConnectToRegionMaster(regionHandler.BestRegion.Code);
			}
		}

		public const string PunVersion = "2.40";

		private static string gameVersion;

		public static LoadBalancingClient NetworkingClient;

		public static readonly int MAX_VIEW_IDS = 1000;

		public const string ServerSettingsFileName = "PhotonServerSettings";

		private static ServerSettings photonServerSettings;

		private const string PlayerPrefsKey = "PUNCloudBestRegion";

		public static ConnectMethod ConnectMethod = ConnectMethod.NotCalled;

		public static PunLogLevel LogLevel = PunLogLevel.ErrorsOnly;

		public static bool EnableCloseConnection = false;

		public static float PrecisionForVectorSynchronization = 9.9E-05f;

		public static float PrecisionForQuaternionSynchronization = 1f;

		public static float PrecisionForFloatSynchronization = 0.01f;

		private static bool offlineMode = false;

		private static Room offlineModeRoom = null;

		private static bool automaticallySyncScene = false;

		private static int sendFrequency = 33;

		private static int serializationFrequency = 100;

		private static bool isMessageQueueRunning = true;

		private static double frametime;

		private static int frame;

		private static Stopwatch StartupStopwatch;

		public static float MinimalTimeScaleToDispatchInFixedUpdate = -1f;

		private static int lastUsedViewSubId = 0;

		private static int lastUsedViewSubIdStatic = 0;

		private static readonly HashSet<string> PrefabsWithoutMagicCallback = new HashSet<string>();

		private static readonly Hashtable SendInstantiateEvHashtable = new Hashtable();

		private static readonly RaiseEventOptions SendInstantiateRaiseEventOptions = new RaiseEventOptions();

		private static HashSet<byte> allowedReceivingGroups = new HashSet<byte>();

		private static HashSet<byte> blockedSendingGroups = new HashSet<byte>();

		private static HashSet<PhotonView> reusablePVHashset = new HashSet<PhotonView>();

		private static NonAllocDictionary<int, PhotonView> photonViewList = new NonAllocDictionary<int, PhotonView>(29U);

		internal static byte currentLevelPrefix = 0;

		internal static bool loadingLevelAndPausedNetwork = false;

		internal const string CurrentSceneProperty = "curScn";

		internal const string CurrentScenePropertyLoadAsync = "curScnLa";

		private static IPunPrefabPool prefabPool;

		public static bool UseRpcMonoBehaviourCache;

		private static readonly Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

		private static Dictionary<string, int> rpcShortcuts;

		public static bool RunRpcCoroutines = true;

		private static AsyncOperation _AsyncLevelLoadingOperation;

		private static float _levelLoadingProgress = 0f;

		private static readonly Type typePunRPC = typeof(PunRPC);

		private static readonly Type typePhotonMessageInfo = typeof(PhotonMessageInfo);

		private static readonly object keyByteZero = 0;

		private static readonly object keyByteOne = 1;

		private static readonly object keyByteTwo = 2;

		private static readonly object keyByteThree = 3;

		private static readonly object keyByteFour = 4;

		private static readonly object keyByteFive = 5;

		private static readonly object keyByteSix = 6;

		private static readonly object keyByteSeven = 7;

		private static readonly object keyByteEight = 8;

		private static readonly object[] emptyObjectArray = new object[0];

		private static readonly Type[] emptyTypeArray = new Type[0];

		internal static List<PhotonView> foundPVs = new List<PhotonView>();

		private static readonly Hashtable removeFilter = new Hashtable();

		private static readonly Hashtable ServerCleanDestroyEvent = new Hashtable();

		private static readonly RaiseEventOptions ServerCleanOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache
		};

		internal static RaiseEventOptions SendToAllOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		};

		internal static RaiseEventOptions SendToOthersOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others
		};

		internal static RaiseEventOptions SendToSingleOptions = new RaiseEventOptions
		{
			TargetActors = new int[1]
		};

		private static readonly Hashtable rpcFilterByViewId = new Hashtable();

		private static readonly RaiseEventOptions OpCleanRpcBufferOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache
		};

		private static Hashtable rpcEvent = new Hashtable();

		private static RaiseEventOptions RpcOptionsToAll = new RaiseEventOptions();

		public static int ObjectsInOneUpdate = 20;

		private static readonly PhotonStream serializeStreamOut = new PhotonStream(true, null);

		private static readonly PhotonStream serializeStreamIn = new PhotonStream(false, null);

		private static RaiseEventOptions serializeRaiseEvOptions = new RaiseEventOptions();

		private static readonly Dictionary<PhotonNetwork.RaiseEventBatch, PhotonNetwork.SerializeViewBatch> serializeViewBatches = new Dictionary<PhotonNetwork.RaiseEventBatch, PhotonNetwork.SerializeViewBatch>();

		private static Dictionary<int, Dictionary<int, Queue<object[]>>> cachedData = new Dictionary<int, Dictionary<int, Queue<object[]>>>();

		public const int SyncViewId = 0;

		public const int SyncCompressed = 1;

		public const int SyncNullValues = 2;

		public const int SyncFirstValue = 3;

		public static Action<EventData, Exception> InternalEventError;

		private static RegionHandler _cachedRegionHandler;

		private struct RaiseEventBatch : IEquatable<PhotonNetwork.RaiseEventBatch>
		{
			public override int GetHashCode()
			{
				return ((int)this.Group << 1) + (this.Reliable ? 1 : 0);
			}

			public bool Equals(PhotonNetwork.RaiseEventBatch other)
			{
				return this.Reliable == other.Reliable && this.Group == other.Group;
			}

			public byte Group;

			public bool Reliable;
		}

		private class SerializeViewBatch : IEquatable<PhotonNetwork.SerializeViewBatch>, IEquatable<PhotonNetwork.RaiseEventBatch>
		{
			public SerializeViewBatch(PhotonNetwork.RaiseEventBatch batch, int offset)
			{
				this.Batch = batch;
				this.ObjectUpdates = new List<object>(this.defaultSize);
				this.offset = offset;
				for (int i = 0; i < offset; i++)
				{
					this.ObjectUpdates.Add(null);
				}
			}

			public override int GetHashCode()
			{
				return ((int)this.Batch.Group << 1) + (this.Batch.Reliable ? 1 : 0);
			}

			public bool Equals(PhotonNetwork.SerializeViewBatch other)
			{
				return this.Equals(other.Batch);
			}

			public bool Equals(PhotonNetwork.RaiseEventBatch other)
			{
				return this.Batch.Reliable == other.Reliable && this.Batch.Group == other.Group;
			}

			public override bool Equals(object obj)
			{
				PhotonNetwork.SerializeViewBatch serializeViewBatch = obj as PhotonNetwork.SerializeViewBatch;
				return serializeViewBatch != null && this.Batch.Equals(serializeViewBatch.Batch);
			}

			public void Clear()
			{
				this.ObjectUpdates.Clear();
				for (int i = 0; i < this.offset; i++)
				{
					this.ObjectUpdates.Add(null);
				}
			}

			public void Add(List<object> viewData)
			{
				if (this.ObjectUpdates.Count >= this.ObjectUpdates.Capacity)
				{
					throw new Exception("Can't add. Size exceeded.");
				}
				this.ObjectUpdates.Add(viewData);
			}

			public readonly PhotonNetwork.RaiseEventBatch Batch;

			public List<object> ObjectUpdates;

			private int defaultSize = PhotonNetwork.ObjectsInOneUpdate;

			private int offset;
		}
	}
}
