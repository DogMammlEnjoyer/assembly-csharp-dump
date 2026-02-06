using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Fusion.Async;
using Fusion.Photon.Realtime;
using Fusion.Photon.Realtime.Async;
using Fusion.Photon.Realtime.Extension;
using Fusion.Protocol;
using Fusion.Sockets;
using Fusion.Sockets.Stun;

namespace Fusion
{
	internal class CloudServices : IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks, IDisposable
	{
		public void OnConnected()
		{
		}

		public void OnConnectedToMaster()
		{
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
			this.OperationFailHandler(32755, debugMessage);
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
			NetworkRunner runner = this._runner;
			if (runner != null)
			{
				runner.InvokeCustomAuthenticationResponse(data);
			}
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			ShutdownReason shutdownReason = DisconnectCauseExt.ConvertToShutdownReason(cause);
			bool flag = this.HandlePhotonCloudDisconnect(shutdownReason);
			if (!flag)
			{
				bool flag2 = shutdownReason > ShutdownReason.Ok;
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log(this._runner, string.Format("Disconnected from Photon Cloud: {0}/{1}", cause, shutdownReason));
					}
				}
				string text = null;
				bool flag3 = this._metadata.LastDisconnectMsg != null;
				if (flag3)
				{
					shutdownReason = DisconnectReasonExt.ConvertToShutdownReason(this._metadata.LastDisconnectMsg.DisconnectReason);
					text = this._metadata.LastDisconnectMsg.CustomData;
					DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
					if (logDebug2 != null)
					{
						logDebug2.Warn(this._runner, string.Format("Fusion Disconnect: {0}={1}, Message={2}", "DisconnectReason", this._metadata.LastDisconnectMsg.DisconnectReason, text));
					}
				}
				StartGameException exception = new StartGameException(shutdownReason, text);
				AsyncOperationHandler<Join> joinAsyncHandler = this._joinAsyncHandler;
				if (joinAsyncHandler != null)
				{
					joinAsyncHandler.SetException(exception);
				}
				bool flag4 = this._runner._startGameOperation != null;
				if (flag4)
				{
					this._runner._startGameOperation.SetException(exception);
				}
				else
				{
					this._runner.Shutdown(true, shutdownReason, true);
				}
			}
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			string str = string.Join(", ", from region in regionHandler.EnabledRegions.AsEnumerable<Region>()
			select region.Code + "[" + region.HostAndPort + "]");
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log("OnRegionListReceived: EnabledRegions=" + str);
			}
		}

		public void OnCreatedRoom()
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this._runner, "Created Session: " + this._communicator.Client.CurrentRoom.Name);
			}
		}

		public void OnJoinedRoom()
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this._runner, "Joined Session: " + this._communicator.Client.CurrentRoom.Name);
			}
			this._runner.LobbyInfo.Reset();
		}

		public void OnLeftRoom()
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this._runner, "Left Session");
			}
			this._runner.LobbyInfo.Reset();
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			this.OperationFailHandler(returnCode, message);
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
			this.OperationFailHandler(returnCode, message);
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			this.OperationFailHandler(returnCode, message);
		}

		public void OnJoinedLobby()
		{
			this._runner.LobbyInfo.IsValid = true;
			this._runner.LobbyInfo.Name = this._communicator.Client.CurrentLobby.Name;
			this._runner.LobbyInfo.Region = this._communicator.Client.CloudRegion.Replace("/*", "");
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this._runner, "Joined Lobby: " + this._runner.LobbyInfo.Name + ", Region=" + this._runner.LobbyInfo.Region);
			}
		}

		public void OnLeftLobby()
		{
			this._runner.LobbyInfo.Reset();
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this._runner, "Left Lobby");
			}
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			this.OnRoomListChanged(roomList);
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		private void OperationFailHandler(short returnCode, string message)
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Warn(this._runner, string.Format("Photon Cloud Operation failed [{0}]: '{1}'", returnCode, message));
			}
			ShutdownReason shutdownReason = ErrorCodeExt.ConvertToShutdownReason(returnCode);
			bool flag = this._runner._startGameOperation != null;
			if (flag)
			{
				this._runner._startGameOperation.SetException(new StartGameException(shutdownReason, message));
			}
			else
			{
				bool flag2 = this.HandlePhotonCloudDisconnect(shutdownReason);
				if (!flag2)
				{
					this._runner.Shutdown(true, shutdownReason, true);
				}
			}
		}

		private bool HandlePhotonCloudDisconnect(ShutdownReason shutdownReason)
		{
			bool flag = this._runner._startGameOperation == null && shutdownReason != ShutdownReason.DisconnectedByPluginLogic && (this._runner.IsServer || (this._runner.IsClient && this._runner.CurrentConnectionType == ConnectionType.Direct)) && this._runner.GameMode >= GameMode.Server;
			bool result;
			if (flag)
			{
				this._tryingToReconnect = (this._rejoinAttempts > 0 && shutdownReason == ShutdownReason.PhotonCloudTimeout && this._communicator.Client.ReconnectAndRejoin());
				bool tryingToReconnect = this._tryingToReconnect;
				if (tryingToReconnect)
				{
					this._rejoinAttempts--;
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log(this._runner, string.Format("Attempting to reconnect to Photon Cloud. Previous disconnect: {0}", shutdownReason));
					}
				}
				else
				{
					LogStream logWarn = InternalLogStreams.LogWarn;
					if (logWarn != null)
					{
						logWarn.Log(this._runner, this._runner.IsServer ? "Unable to re-establish a connection to the Photon Cloud. Matchmaking is currently disabled, and new clients will be unable to connect. The match will continue for all direct connections." : "Unable to re-establish a connection to the Photon Cloud. The match will continue for the local player due to the direct connection.");
					}
					this._cloudServerDisconnected = true;
				}
				try
				{
					NetworkRunner.CloudConnectionLostHandler cloudConnectionLost = NetworkRunner.CloudConnectionLost;
					if (cloudConnectionLost != null)
					{
						cloudConnectionLost(this._runner, shutdownReason, this._tryingToReconnect);
					}
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(error);
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool IsCloudReady
		{
			get
			{
				CloudCommunicator communicator = this._communicator;
				return ((communicator != null) ? communicator.Client : null) != null && this._communicator.Client.IsConnectedAndReady;
			}
		}

		public string UserId
		{
			get
			{
				return this.IsCloudReady ? this._communicator.Client.UserId : null;
			}
		}

		public bool IsInRoom
		{
			get
			{
				return this.IsCloudReady && this._communicator.Client.IsReadyAndInRoom;
			}
		}

		public bool IsInLobby
		{
			get
			{
				return this.IsCloudReady && this._communicator.Client.InLobby;
			}
		}

		public JoinProcessStage CurrentJoinStage
		{
			get
			{
				return this._metadata.CurrentJoinStage;
			}
		}

		public ProtocolMessageVersion CurrentProtocolMessageVersion
		{
			get
			{
				return this._metadata.CurrentProtocolMessageVersion;
			}
		}

		public int SessionSlots
		{
			get
			{
				return this.IsInRoom ? this._communicator.Client.CurrentRoom.MaxPlayers : -1;
			}
		}

		public bool IsMasterClient
		{
			get
			{
				return this.IsInRoom && this._communicator.Client.LocalPlayer.IsMasterClient;
			}
		}

		public AuthenticationValues AuthenticationValues
		{
			get
			{
				return this.IsCloudReady ? this._communicator.Client.AuthValues : null;
			}
		}

		public ICommunicator Communicator
		{
			get
			{
				return this._communicator;
			}
		}

		public string CachedRegionSummary
		{
			get
			{
				return this._communicator.Client.SummaryToCache;
			}
		}

		public bool IsNATPunchthroughEnabled { get; internal set; } = true;

		public bool IsEncryptionEnabled
		{
			get
			{
				return this.IsCloudReady && this._communicator.Client.IsEncryptionEnabled;
			}
		}

		public string CustomSTUNServer { get; internal set; } = null;

		public NATType NATType
		{
			get
			{
				CloudServicesMetadata metadata = this._metadata;
				return (((metadata != null) ? metadata.LocalReflexiveInfo : null) != null) ? this._metadata.LocalReflexiveInfo.NatType : NATType.Invalid;
			}
		}

		public PlayerRef LocalPlayerRef
		{
			get
			{
				CloudServicesMetadata metadata = this._metadata;
				return PlayerRef.FromIndex(((metadata != null) ? new int?(metadata.PlayerRef) : null).GetValueOrDefault());
			}
		}

		private bool IsServerOrMasterClient
		{
			get
			{
				return this._runner != null && (this._runner.IsServer || this._runner.IsSharedModeMasterClient);
			}
		}

		public CloudServices(NetworkRunner runner, FusionAppSettings customAppSettings, CloudCommunicator communicator = null)
		{
			this._runner = runner;
			TaskManager.Setup();
			this._communicator = (communicator ?? new CloudCommunicator(customAppSettings));
			this._communicator.Client.AddCallbackTarget(this);
			this._communicator.Client.OnRoomChanged += this.OnRoomChanged;
			this._communicator.Client.AddressRewriter = runner.CloudAddressRewriter;
			this._communicator.WasExtracted = false;
			bool isConnected = this._communicator.Client.IsConnected;
			if (isConnected)
			{
				this._communicator.Client.StartFallbackSendAck();
			}
			this._communicator.RegisterPackageCallback<Join>(new Action<int, Join>(this.HandleJoinMessage));
			this._communicator.RegisterPackageCallback<Start>(new Action<int, Start>(this.HandleStartMessage));
			this._communicator.RegisterPackageCallback<Disconnect>(new Action<int, Disconnect>(this.HandleDisconnectMessage));
			this._communicator.RegisterPackageCallback<ReflexiveInfo>(new Action<int, ReflexiveInfo>(this.HandleReflexiveInfoMessage));
			this._communicator.RegisterPackageCallback<NetworkConfigSync>(new Action<int, NetworkConfigSync>(this.HandleNetworkConfigMessage));
			this._communicator.RegisterPackageCallback<HostMigration>(new Action<int, HostMigration>(this.HandleHostMigrationMessage));
			this._communicator.RegisterPackageCallback<Snapshot>(new Action<int, Snapshot>(this.HandleSnapshotMessage));
			this._communicator.RegisterPackageCallback<PlayerRefMapping>(new Action<int, PlayerRefMapping>(this.HandlePlayerRefMapping));
			this._communicator.RegisterPackageCallback<DummyTrafficSync>(new Action<int, DummyTrafficSync>(this.HandleDummyTrafficSync));
			this._metadata = new CloudServicesMetadata();
		}

		public CloudCommunicator ExtractCommunicator()
		{
			this._communicator.Client.RemoveCallbackTarget(this);
			this._communicator.Client.OnRoomChanged -= this.OnRoomChanged;
			this._communicator.Client.StopFallbackSendAck();
			this._communicator.Reset();
			this._communicator.WasExtracted = true;
			return this._communicator;
		}

		public void Update()
		{
			bool flag = this._communicator != null && !this._communicator.WasExtracted && !this._cloudServerDisconnected;
			if (flag)
			{
				this._communicator.Service();
			}
		}

		[DebuggerStepThrough]
		public Task ConnectToCloud(AppSettings appSettings, AuthenticationValues authentication = null, CancellationToken externalCancellationToken = default(CancellationToken), bool? useDefaultCloudPorts = null)
		{
			CloudServices.<ConnectToCloud>d__67 <ConnectToCloud>d__ = new CloudServices.<ConnectToCloud>d__67();
			<ConnectToCloud>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ConnectToCloud>d__.<>4__this = this;
			<ConnectToCloud>d__.appSettings = appSettings;
			<ConnectToCloud>d__.authentication = authentication;
			<ConnectToCloud>d__.externalCancellationToken = externalCancellationToken;
			<ConnectToCloud>d__.useDefaultCloudPorts = useDefaultCloudPorts;
			<ConnectToCloud>d__.<>1__state = -1;
			<ConnectToCloud>d__.<>t__builder.Start<CloudServices.<ConnectToCloud>d__67>(ref <ConnectToCloud>d__);
			return <ConnectToCloud>d__.<>t__builder.Task;
		}

		public Task<short> JoinSessionLobby(SessionLobby sessionLobby, string lobbyID = null, LobbyType lobbyType = LobbyType.Default)
		{
			bool flag = !this.IsCloudReady;
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new InvalidOperationException("Fusion Relay Client is not ready. Make sure the call ConnectToCloud before start with StartGame"));
			}
			else
			{
				TypedLobby lobby;
				switch (sessionLobby)
				{
				case SessionLobby.ClientServer:
					lobby = CloudServicesMetadata.LobbyClientServer;
					break;
				case SessionLobby.Shared:
					lobby = CloudServicesMetadata.LobbyShared;
					break;
				case SessionLobby.Custom:
				{
					bool flag2 = string.IsNullOrEmpty((lobbyID != null) ? lobbyID.Trim() : null);
					if (flag2)
					{
						return Task.FromException<short>(new InvalidOperationException("Invalid Lobby Name: Empty or Null"));
					}
					lobby = new TypedLobby(lobbyID.Trim(), lobbyType);
					break;
				}
				default:
					return Task.FromException<short>(new InvalidOperationException("Invalid Lobby Type"));
				}
				result = this._communicator.Client.JoinLobbyAsync(lobby, true, true, default(CancellationToken));
			}
			return result;
		}

		public Task<short> EnterRoom(StartGameArgs args, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = !this.IsCloudReady;
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new InvalidOperationException("Fusion Relay Client is not ready. Make sure the call ConnectToCloud before start with StartGame"));
			}
			else
			{
				bool isInRoom = this.IsInRoom;
				if (isInRoom)
				{
					result = Task.FromResult<short>(0);
				}
				else
				{
					bool flag2 = args.GameMode == GameMode.Client || args.GameMode == GameMode.Host || args.GameMode == GameMode.Server || args.GameMode == GameMode.AutoHostOrClient;
					bool flag3 = flag2;
					if (flag3)
					{
						this._communicator.Client.DisconnectTimeout = 5000;
					}
					bool flag4 = args.GameMode == GameMode.Host || args.GameMode == GameMode.Server;
					bool flag5 = args.EnableClientSessionCreation == null;
					if (flag5)
					{
						bool flag6 = args.GameMode == GameMode.Shared || args.GameMode == GameMode.AutoHostOrClient;
						if (flag6)
						{
							args.EnableClientSessionCreation = new bool?(true);
						}
						bool flag7 = args.GameMode == GameMode.Client;
						if (flag7)
						{
							args.EnableClientSessionCreation = new bool?(false);
						}
					}
					bool flag8 = args.GameMode == GameMode.Shared || args.GameMode == GameMode.AutoHostOrClient || args.GameMode == GameMode.Client;
					bool flag9 = flag8 && args.EnableClientSessionCreation.Value;
					bool flag10 = args.GameMode == GameMode.Server;
					bool inLobby = this._communicator.Client.InLobby;
					TypedLobby typedLobby;
					if (inLobby)
					{
						typedLobby = this._communicator.Client.CurrentLobby;
					}
					else
					{
						string customLobbyName = args.CustomLobbyName;
						bool flag11 = !string.IsNullOrEmpty((customLobbyName != null) ? customLobbyName.Trim() : null);
						if (flag11)
						{
							typedLobby = new TypedLobby(args.CustomLobbyName.Trim(), LobbyType.Default);
						}
						else
						{
							typedLobby = ((args.GameMode == GameMode.Shared) ? CloudServicesMetadata.LobbyShared : CloudServicesMetadata.LobbyClientServer);
						}
					}
					string sessionName = args.SessionName;
					string text = (sessionName != null) ? sessionName.Trim() : null;
					bool flag12 = string.IsNullOrEmpty(text);
					bool flag13 = flag12;
					if (flag13)
					{
						Func<string> sessionNameGenerator = args.SessionNameGenerator;
						text = ((sessionNameGenerator != null) ? sessionNameGenerator() : null);
						text = ((text != null) ? text.Trim() : null);
						bool flag14 = string.IsNullOrEmpty(text);
						if (flag14)
						{
							text = Guid.NewGuid().ToString();
						}
					}
					int? playerCount = args.PlayerCount;
					int num2;
					if (playerCount == null)
					{
						NetworkProjectConfig config = args.Config;
						int? num;
						if (config == null)
						{
							num = null;
						}
						else
						{
							SimulationConfig simulation = config.Simulation;
							num = ((simulation != null) ? new int?(simulation.PlayerCount) : null);
						}
						num2 = (num ?? NetworkProjectConfig.Global.Simulation.PlayerCount);
					}
					else
					{
						num2 = playerCount.GetValueOrDefault();
					}
					int maxPlayers = num2 + (flag10 ? 1 : 0);
					EnterRoomParams enterRoomParams = this._communicator.Client.BuildEnterRoomParams(typedLobby, text, maxPlayers, args.SessionProperties, args.IsOpen.GetValueOrDefault(true), args.IsVisible.GetValueOrDefault(true), args.GameMode != GameMode.Shared, flag2);
					OpJoinRandomRoomParams joinRandomRoomParams = this._communicator.Client.BuildJoinParams(typedLobby, args.SessionProperties, args.MatchmakingMode.GetValueOrDefault());
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						ILogSource runner = this._runner;
						string[] array = new string[8];
						array[0] = "Joining Session: ";
						array[1] = enterRoomParams.RoomName;
						array[2] = ", Lobby=";
						array[3] = enterRoomParams.Lobby.Name;
						array[4] = ", Region=";
						int num3 = 5;
						string cloudRegion = this._communicator.Client.CloudRegion;
						array[num3] = ((cloudRegion != null) ? cloudRegion.Replace("/*", "") : null);
						array[6] = ",";
						array[7] = string.Format("Random Join={0}", flag12);
						logDebug.Log(runner, string.Concat(array));
					}
					bool flag15 = flag4;
					if (flag15)
					{
						result = this._communicator.Client.CreateOrJoinRoomAsync(enterRoomParams, true, true, externalCancellationToken);
					}
					else
					{
						bool flag16 = flag12;
						if (flag16)
						{
							bool flag17 = flag9;
							if (flag17)
							{
								result = this._communicator.Client.JoinRandomOrCreateRoomAsync(joinRandomRoomParams, enterRoomParams, true, true, externalCancellationToken);
							}
							else
							{
								result = this._communicator.Client.JoinRandomRoomAsync(joinRandomRoomParams, true, true, externalCancellationToken);
							}
						}
						else
						{
							bool flag18 = flag9;
							if (flag18)
							{
								result = this._communicator.Client.CreateOrJoinRoomAsync(enterRoomParams, true, true, externalCancellationToken);
							}
							else
							{
								result = this._communicator.Client.JoinRoomAsync(enterRoomParams, true, true, externalCancellationToken);
							}
						}
					}
				}
			}
			return result;
		}

		[DebuggerStepThrough]
		public Task DisconnectFromCloud()
		{
			CloudServices.<DisconnectFromCloud>d__70 <DisconnectFromCloud>d__ = new CloudServices.<DisconnectFromCloud>d__70();
			<DisconnectFromCloud>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DisconnectFromCloud>d__.<>4__this = this;
			<DisconnectFromCloud>d__.<>1__state = -1;
			<DisconnectFromCloud>d__.<>t__builder.Start<CloudServices.<DisconnectFromCloud>d__70>(ref <DisconnectFromCloud>d__);
			return <DisconnectFromCloud>d__.<>t__builder.Task;
		}

		public string GetActorUserID(int actorID)
		{
			Player player;
			bool flag = this.IsInRoom && this._communicator.Client.CurrentRoom.Players.TryGetValue(actorID, out player);
			string result;
			if (flag)
			{
				result = player.UserId;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public bool TryGetActorIdByUniqueId(long uniqueId, out int actorId)
		{
			ReflexiveInfo reflexiveInfo;
			bool flag = this._metadata.UniqueIdToReflexiveInfoTable.TryGetValue(uniqueId, out reflexiveInfo);
			bool result;
			if (flag)
			{
				actorId = reflexiveInfo.ActorNr;
				result = true;
			}
			else
			{
				actorId = -1;
				result = false;
			}
			return result;
		}

		internal void OnInternalConnectionAttempt(int attempt, int totalConnectionAttempts, out bool shouldChange, out NetAddress newAddress)
		{
			shouldChange = false;
			newAddress = default(NetAddress);
			bool flag = this._runner.GameMode != GameMode.Client;
			if (!flag)
			{
				switch (this._metadata.CurrentPunchStage)
				{
				case NATPunchStage.None:
					Assert.AlwaysFail(string.Format("CloudServices should not be in Stage {0}", this._metadata.CurrentPunchStage));
					break;
				case NATPunchStage.Local:
				{
					bool flag2 = attempt > 2;
					if (flag2)
					{
						shouldChange = true;
						newAddress = this._metadata.RemoteReflexiveInfo.PublicAddr;
						this._metadata.CurrentPunchStage = NATPunchStage.Public;
					}
					break;
				}
				case NATPunchStage.Public:
				{
					bool flag3 = (float)attempt >= (float)totalConnectionAttempts * 2f / 3f;
					if (flag3)
					{
						shouldChange = true;
						newAddress = NetAddress.FromActorId(this._metadata.RemoteReflexiveInfo.ActorNr);
						this._metadata.CurrentPunchStage = NATPunchStage.Relay;
					}
					break;
				}
				}
			}
		}

		private void Connect(NATPunchStage punchStage, NetAddress endPoint)
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this._runner, string.Format("Connecting to {0}", endPoint));
			}
			this._metadata.CurrentPunchStage = punchStage;
			this._runner.Connect(endPoint, this._metadata.RunnerInitializeArgs.ConnectionToken, this._metadata.UniqueId);
		}

		public void Dispose()
		{
			CloudCommunicator communicator = this._communicator;
			if (communicator != null)
			{
				communicator.Dispose();
			}
			this._communicator = null;
		}

		internal void OnRoomChanged()
		{
			bool flag = this.IsInRoom && this._runner.SessionInfo != null;
			if (flag)
			{
				this.UpdateSessionInfo(this._runner.SessionInfo, this._communicator.Client.CurrentRoom, this._communicator.Client.CloudRegion);
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Log(this._runner, string.Format("SessionInfo Update: {0}", this._runner.SessionInfo));
				}
			}
		}

		internal bool UpdateRoomProperties(Dictionary<string, SessionProperty> customProperties)
		{
			return this.IsServerOrMasterClient && this.IsInRoom && this._communicator.Client.UpdateRoomProperties(customProperties);
		}

		internal bool UpdateRoomIsOpen(bool status)
		{
			return this.IsServerOrMasterClient && this.IsInRoom && this._communicator.Client.UpdateRoomIsOpen(status);
		}

		internal bool UpdateRoomIsVisible(bool status)
		{
			return this.IsServerOrMasterClient && this.IsInRoom && this._communicator.Client.UpdateRoomIsVisible(status);
		}

		private void OnRoomListChanged(List<RoomInfo> roomList)
		{
			foreach (RoomInfo roomInfo in roomList)
			{
				bool removedFromList = roomInfo.RemovedFromList;
				if (removedFromList)
				{
					this._cachedSessionList.Remove(roomInfo.Name);
				}
				else
				{
					bool flag = !this._cachedSessionList.ContainsKey(roomInfo.Name);
					if (flag)
					{
						this._cachedSessionList[roomInfo.Name] = new SessionInfo(null);
					}
					this.UpdateSessionInfo(this._cachedSessionList[roomInfo.Name], roomInfo, this._communicator.Client.CloudRegion);
				}
			}
			this._runner.InvokeSessionListUpdated(new List<SessionInfo>(this._cachedSessionList.Values));
		}

		[DebuggerStepThrough]
		internal Task Join(CancellationToken externalCancellationToken = default(CancellationToken))
		{
			CloudServices.<Join>d__83 <Join>d__ = new CloudServices.<Join>d__83();
			<Join>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Join>d__.<>4__this = this;
			<Join>d__.externalCancellationToken = externalCancellationToken;
			<Join>d__.<>1__state = -1;
			<Join>d__.<>t__builder.Start<CloudServices.<Join>d__83>(ref <Join>d__);
			return <Join>d__.<>t__builder.Task;
		}

		private void SendNetworkSyncMessage(NetworkProjectConfig projectConfig)
		{
			string text = NetworkProjectConfig.SerializeMinimal(projectConfig);
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this._runner, "Sending serialized NetworkProjectConfig:\n" + text);
			}
			NetworkConfigSync message = new NetworkConfigSync(SyncType.Response, text, this._metadata.CurrentProtocolMessageVersion, null);
			this._communicator.SendMessage(0, message);
		}

		private void SendReflexiveInfo(StunResult stunResult)
		{
			ReflexiveInfo message = new ReflexiveInfo(this._communicator.CommunicatorID, stunResult.PublicEndPoint, stunResult.PrivateEndPoint, stunResult.NatType, null, this._metadata.CurrentProtocolMessageVersion, null);
			this._communicator.SendMessage(0, message);
		}

		public void SendChangeMasterClient(int newCandidate)
		{
			ChangeMasterClient message = new ChangeMasterClient(newCandidate, this._metadata.CurrentProtocolMessageVersion, null);
			this._communicator.SendMessage(0, message);
		}

		public void SendStateSnapshot(byte[] data, int snapshotSize, int tick, uint lastId)
		{
			try
			{
				Snapshot snapshot = new Snapshot(tick, lastId, SnapshotType.Data, snapshotSize, data, this._metadata.CurrentProtocolMessageVersion, null);
				Assert.Check(snapshot.IsValid);
				this._communicator.SendMessage(0, snapshot);
			}
			catch (Exception message)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Error(message);
				}
			}
		}

		private void HandleJoinMessage(int sender, Join join)
		{
			Assert.Check<int>(sender == 0, "Invalid Sender of Join Confirmation {0}", sender);
			AsyncOperationHandler<Join> joinAsyncHandler = this._joinAsyncHandler;
			if (joinAsyncHandler != null)
			{
				joinAsyncHandler.SetResult(join);
			}
		}

		[DebuggerStepThrough]
		private void HandleStartMessage(int sender, Start start)
		{
			CloudServices.<HandleStartMessage>d__89 <HandleStartMessage>d__ = new CloudServices.<HandleStartMessage>d__89();
			<HandleStartMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleStartMessage>d__.<>4__this = this;
			<HandleStartMessage>d__.sender = sender;
			<HandleStartMessage>d__.start = start;
			<HandleStartMessage>d__.<>1__state = -1;
			<HandleStartMessage>d__.<>t__builder.Start<CloudServices.<HandleStartMessage>d__89>(ref <HandleStartMessage>d__);
		}

		private void HandleDisconnectMessage(int sender, Disconnect disconnect)
		{
			Assert.Check<int>(sender == 0, "Invalid Sender of Disconnect Message: {0}", sender);
			this._metadata.LastDisconnectMsg = disconnect;
		}

		private void HandleNetworkConfigMessage(int sender, NetworkConfigSync configSync)
		{
		}

		[DebuggerStepThrough]
		private void HandleReflexiveInfoMessage(int sender, ReflexiveInfo reflexiveInfo)
		{
			CloudServices.<HandleReflexiveInfoMessage>d__92 <HandleReflexiveInfoMessage>d__ = new CloudServices.<HandleReflexiveInfoMessage>d__92();
			<HandleReflexiveInfoMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleReflexiveInfoMessage>d__.<>4__this = this;
			<HandleReflexiveInfoMessage>d__.sender = sender;
			<HandleReflexiveInfoMessage>d__.reflexiveInfo = reflexiveInfo;
			<HandleReflexiveInfoMessage>d__.<>1__state = -1;
			<HandleReflexiveInfoMessage>d__.<>t__builder.Start<CloudServices.<HandleReflexiveInfoMessage>d__92>(ref <HandleReflexiveInfoMessage>d__);
		}

		private void HandleHostMigrationMessage(int sender, HostMigration hostMigration)
		{
			Assert.Check<int>(sender == 0, "Invalid Sender of HostMigration: {0}", sender);
			this._runner.SetupHostMigration(hostMigration);
			bool flag = hostMigration.PeerMode == PeerMode.Client;
			if (flag)
			{
				this._runner.StartHostMigration(null);
			}
		}

		private void HandleSnapshotMessage(int sender, Snapshot snapshot)
		{
			Assert.Check<int>(sender == 0, "Invalid Sender of Snapshot: {0}", sender);
			SnapshotType snapshotType = snapshot.SnapshotType;
			SnapshotType snapshotType2 = snapshotType;
			if (snapshotType2 != SnapshotType.Data)
			{
				if (snapshotType2 == SnapshotType.Confirmation)
				{
					bool flag = this._runner.LastSnapshotTick != snapshot.Tick;
					if (flag)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Warn(string.Format("Expecting Snapshot: {0}", this._runner.LastSnapshotTick));
						}
					}
					bool flag2 = this._runner.LastConfirmedSnapshotTick < snapshot.Tick;
					if (flag2)
					{
						Interlocked.Exchange(ref this._runner.LastConfirmedSnapshotTick, snapshot.Tick);
						DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
						if (logDebug2 != null)
						{
							logDebug2.Log(string.Format("Host Snapshot for Tick {0} confirmed.", this._runner.LastConfirmedSnapshotTick));
						}
					}
				}
			}
			else
			{
				this._runner.StartHostMigration(snapshot);
			}
		}

		private void HandleDummyTrafficSync(int sender, DummyTrafficSync dummyTrafficSync)
		{
			Assert.Check<int>(sender == 0, "Invalid Sender of DummyTrafficSync: {0}", sender);
			this.SetupDummyTraffic(dummyTrafficSync);
		}

		[DebuggerStepThrough]
		private Task<bool> ConfirmJoin()
		{
			CloudServices.<ConfirmJoin>d__96 <ConfirmJoin>d__ = new CloudServices.<ConfirmJoin>d__96();
			<ConfirmJoin>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ConfirmJoin>d__.<>4__this = this;
			<ConfirmJoin>d__.<>1__state = -1;
			<ConfirmJoin>d__.<>t__builder.Start<CloudServices.<ConfirmJoin>d__96>(ref <ConfirmJoin>d__);
			return <ConfirmJoin>d__.<>t__builder.Task;
		}

		private void HandlePlayerRefMapping(int sender, PlayerRefMapping msg)
		{
			this._runner._simulation.RegisterUniqueIdPlayerMapping(msg.ActorId, msg.UniqueId, PlayerRef.FromIndex(msg.PlayerRef));
		}

		internal void StartBackgroundCloudServices()
		{
			TaskManager.Service(new Func<Task<bool>>(this.Service_KeepAlive), this._runner.OperationsCancellationToken, 30000, null);
			bool enableAutoUpdate = this._runner.Config.HostMigration.EnableAutoUpdate;
			if (enableAutoUpdate)
			{
				TaskManager.Service(new Func<Task<bool>>(this.Service_HostMigrationSnapshot), this._runner.OperationsCancellationToken, 1000 * this._runner.Config.HostMigration.UpdateDelay, null);
			}
		}

		private Task<bool> Service_KeepAlive()
		{
			bool flag = this._runner == null || this._communicator == null;
			Task<bool> result;
			if (flag)
			{
				result = Task.FromResult<bool>(false);
			}
			else
			{
				bool flag2 = this._runner.IsRunning && this._communicator.Client.IsConnectedAndReady;
				if (flag2)
				{
					bool isClient = this._runner.IsClient;
					if (isClient)
					{
						return Task.FromResult<bool>(false);
					}
					bool flag3 = this._runner.IsServer && this._runner.HasAnyActiveConnections();
					if (flag3)
					{
						this._communicator.SendPackage(101, this._communicator.CommunicatorID, false, null, 0);
					}
				}
				result = Task.FromResult<bool>(true);
			}
			return result;
		}

		[DebuggerStepThrough]
		private Task<bool> Service_HostMigrationSnapshot()
		{
			CloudServices.<Service_HostMigrationSnapshot>d__100 <Service_HostMigrationSnapshot>d__ = new CloudServices.<Service_HostMigrationSnapshot>d__100();
			<Service_HostMigrationSnapshot>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<Service_HostMigrationSnapshot>d__.<>4__this = this;
			<Service_HostMigrationSnapshot>d__.<>1__state = -1;
			<Service_HostMigrationSnapshot>d__.<>t__builder.Start<CloudServices.<Service_HostMigrationSnapshot>d__100>(ref <Service_HostMigrationSnapshot>d__);
			return <Service_HostMigrationSnapshot>d__.<>t__builder.Task;
		}

		private void Run_ReversePing(NetAddress remoteAddr)
		{
			CloudServices.<>c__DisplayClass101_0 CS$<>8__locals1 = new CloudServices.<>c__DisplayClass101_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.remoteAddr = remoteAddr;
			bool flag = !CS$<>8__locals1.remoteAddr.IsValid;
			if (!flag)
			{
				TaskManager.Run(delegate(CancellationToken token)
				{
					CloudServices.<>c__DisplayClass101_0.<<Run_ReversePing>b__0>d <<Run_ReversePing>b__0>d = new CloudServices.<>c__DisplayClass101_0.<<Run_ReversePing>b__0>d();
					<<Run_ReversePing>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<Run_ReversePing>b__0>d.<>4__this = CS$<>8__locals1;
					<<Run_ReversePing>b__0>d.token = token;
					<<Run_ReversePing>b__0>d.<>1__state = -1;
					<<Run_ReversePing>b__0>d.<>t__builder.Start<CloudServices.<>c__DisplayClass101_0.<<Run_ReversePing>b__0>d>(ref <<Run_ReversePing>b__0>d);
					return <<Run_ReversePing>b__0>d.<>t__builder.Task;
				}, this._runner.OperationsCancellationToken, TaskCreationOptions.None);
			}
		}

		private void SetupDummyTraffic(DummyTrafficSync dummyTrafficSyncMessage)
		{
			bool flag = dummyTrafficSyncMessage == null || !dummyTrafficSyncMessage.IsValid;
			if (flag)
			{
				TraceLogStream logTraceDummyTraffic = InternalLogStreams.LogTraceDummyTraffic;
				if (logTraceDummyTraffic != null)
				{
					logTraceDummyTraffic.Warn(this._runner, "Invalid Dummy Traffic Message, ignore.");
				}
			}
			else
			{
				bool flag2 = this._dummyTrafficCts != null;
				if (flag2)
				{
					this._dummyTrafficCts.Cancel();
					this._dummyTrafficCts.Dispose();
				}
				this._dummyTrafficCts = new CancellationTokenSource();
				this._dummyTrafficLinkCts = CancellationTokenSource.CreateLinkedTokenSource(this._dummyTrafficCts.Token, this._runner.OperationsCancellationToken);
				bool flag3 = this._dummyData == null || this._dummyData.Length != dummyTrafficSyncMessage.Size;
				if (flag3)
				{
					this._dummyData = new byte[dummyTrafficSyncMessage.Size];
					new Random().NextBytes(this._dummyData);
				}
				TaskManager.Service(delegate()
				{
					bool flag4 = this._runner.IsRunning && this._runner.Topology != Topologies.ClientServer;
					Task<bool> result;
					if (flag4)
					{
						result = Task.FromResult<bool>(false);
					}
					else
					{
						this.<SetupDummyTraffic>g__SendDummyTraffic|105_1(this._dummyData);
						result = Task.FromResult<bool>(true);
					}
					return result;
				}, this._dummyTrafficLinkCts.Token, dummyTrafficSyncMessage.SendInterval, "DummyTraffic");
			}
		}

		[DebuggerStepThrough]
		private Task<StunResult> QueryReflexiveInfo()
		{
			CloudServices.<QueryReflexiveInfo>d__106 <QueryReflexiveInfo>d__ = new CloudServices.<QueryReflexiveInfo>d__106();
			<QueryReflexiveInfo>d__.<>t__builder = AsyncTaskMethodBuilder<StunResult>.Create();
			<QueryReflexiveInfo>d__.<>4__this = this;
			<QueryReflexiveInfo>d__.<>1__state = -1;
			<QueryReflexiveInfo>d__.<>t__builder.Start<CloudServices.<QueryReflexiveInfo>d__106>(ref <QueryReflexiveInfo>d__);
			return <QueryReflexiveInfo>d__.<>t__builder.Task;
		}

		public void UpdateInitializeArgs(NetworkRunnerInitializeArgs newArgs)
		{
			this._metadata.RunnerInitializeArgs = newArgs;
		}

		private bool CheckSubnet(NetAddress remotePrivateEndPoint)
		{
			return this._metadata.LocalReflexiveInfo != null && (remotePrivateEndPoint.IsIPv6 || NetAddress.SubnetMask.IsSameSubNet(this._metadata.LocalReflexiveInfo.PrivateEndPoint, remotePrivateEndPoint));
		}

		private void UpdateSessionInfo(SessionInfo sessionInfo, RoomInfo roomInfo, string region)
		{
			Room room = roomInfo as Room;
			bool flag = room != null;
			if (flag)
			{
				sessionInfo.Name = room.Name;
				sessionInfo._isOpen = room.IsOpen;
				sessionInfo._isVisible = room.IsVisible;
				sessionInfo.MaxPlayers = room.MaxPlayers;
				sessionInfo.PlayerCount = room.PlayerCount;
			}
			else
			{
				sessionInfo.Name = roomInfo.Name;
				sessionInfo._isOpen = roomInfo.IsOpen;
				sessionInfo._isVisible = roomInfo.IsVisible;
				sessionInfo.MaxPlayers = roomInfo.MaxPlayers;
				sessionInfo.PlayerCount = roomInfo.PlayerCount;
			}
			sessionInfo.Region = region;
			sessionInfo.Properties = new ReadOnlyDictionary<string, SessionProperty>(roomInfo.GetCustomProperties());
			sessionInfo._isValid = true;
		}

		[CompilerGenerated]
		private unsafe void <SetupDummyTraffic>g__SendDummyTraffic|105_1(byte[] buffer)
		{
			bool flag = this._runner.IsRunning && this._communicator.Client.IsConnectedAndReady;
			if (flag)
			{
				fixed (byte[] array = buffer)
				{
					byte* buffer2;
					if (buffer == null || array.Length == 0)
					{
						buffer2 = null;
					}
					else
					{
						buffer2 = &array[0];
					}
					this._communicator.SendPackage(102, this._communicator.CommunicatorID, false, buffer2, buffer.Length);
					TraceLogStream logTraceDummyTraffic = InternalLogStreams.LogTraceDummyTraffic;
					if (logTraceDummyTraffic != null)
					{
						logTraceDummyTraffic.Log(string.Format("Sent to {0} with {1} bytes.", this._communicator.CommunicatorID, this._dummyData.Length));
					}
				}
			}
		}

		[CompilerGenerated]
		private bool <QueryReflexiveInfo>g__KeepRunning|106_0()
		{
			return !this._runner.IsShutdown;
		}

		[CompilerGenerated]
		private unsafe bool <QueryReflexiveInfo>g__SendAnyData|106_1(byte[] requestBytes, NetAddress target)
		{
			bool flag = !target.IsValid;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				NetworkRunner runner = this._runner;
				bool flag2 = ((runner != null) ? runner._simulation : null) == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					try
					{
						NetPeer* netPeer = this._runner._simulation._netPeer;
						INetSocket netSocket = this._runner._simulation._netSocket;
						bool flag3 = netPeer == null || netSocket == null;
						if (flag3)
						{
							return false;
						}
						try
						{
							fixed (byte[] array = requestBytes)
							{
								byte* buffer;
								if (requestBytes == null || array.Length == 0)
								{
									buffer = null;
								}
								else
								{
									buffer = &array[0];
								}
								return netSocket.Send(netPeer->_socket, &target, buffer, requestBytes.Length, false) > 0;
							}
						}
						finally
						{
							byte[] array = null;
						}
					}
					catch (Exception arg)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Warn(string.Format("Error while sending STUN Message: {0}", arg));
						}
					}
					result = false;
				}
			}
			return result;
		}

		private readonly CloudServicesMetadata _metadata;

		private readonly NetworkRunner _runner;

		private CloudCommunicator _communicator;

		private readonly Dictionary<string, SessionInfo> _cachedSessionList = new Dictionary<string, SessionInfo>();

		private bool _cloudServerDisconnected;

		private bool _tryingToReconnect;

		private int _rejoinAttempts = 5;

		private AsyncOperationHandler<Join> _joinAsyncHandler;

		private byte[] _dummyData;

		private CancellationTokenSource _dummyTrafficCts;

		private CancellationTokenSource _dummyTrafficLinkCts;

		private static class ErrorMessages
		{
			public const string StartBeforeJoin = "Received Start Message, but never a Join Confirmation. Shutdown.";

			public const string RunnerFailInit = "Runner failed to Initialize. Shutdown.";

			public const string JoinTimeout = "Join Confirmation timeout. Shutdown.";
		}
	}
}
