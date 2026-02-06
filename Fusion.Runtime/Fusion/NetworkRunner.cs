using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fusion.Async;
using Fusion.Encryption;
using Fusion.Photon.Realtime;
using Fusion.Protocol;
using Fusion.Sockets;
using Fusion.Sockets.Stun;
using Fusion.Statistics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion
{
	[AddComponentMenu("Fusion/Network Runner")]
	[DisallowMultipleComponent]
	[HelpURL("https://doc.photonengine.com/fusion/current/manual/prebuilt-components#networkrunner")]
	[ScriptHelp(BackColor = ScriptHeaderBackColor.Red)]
	public sealed class NetworkRunner : Behaviour, Simulation.ICallbacks
	{
		public bool IsResume
		{
			get
			{
				return this._simulation != null && this._simulation.IsResume && this._initializeOperation != null && !this._initializeOperation.Task.IsCompleted;
			}
		}

		[DebuggerStepThrough]
		public Task<bool> PushHostMigrationSnapshot()
		{
			NetworkRunner.<PushHostMigrationSnapshot>d__2 <PushHostMigrationSnapshot>d__ = new NetworkRunner.<PushHostMigrationSnapshot>d__2();
			<PushHostMigrationSnapshot>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<PushHostMigrationSnapshot>d__.<>4__this = this;
			<PushHostMigrationSnapshot>d__.<>1__state = -1;
			<PushHostMigrationSnapshot>d__.<>t__builder.Start<NetworkRunner.<PushHostMigrationSnapshot>d__2>(ref <PushHostMigrationSnapshot>d__);
			return <PushHostMigrationSnapshot>d__.<>t__builder.Task;
		}

		public IEnumerable<NetworkObject> GetResumeSnapshotNetworkObjects()
		{
			NetworkRunner.<GetResumeSnapshotNetworkObjects>d__3 <GetResumeSnapshotNetworkObjects>d__ = new NetworkRunner.<GetResumeSnapshotNetworkObjects>d__3(-2);
			<GetResumeSnapshotNetworkObjects>d__.<>4__this = this;
			return <GetResumeSnapshotNetworkObjects>d__;
		}

		public IEnumerable<ValueTuple<NetworkObject, NetworkObjectHeaderPtr>> GetResumeSnapshotNetworkSceneObjects()
		{
			NetworkRunner.<GetResumeSnapshotNetworkSceneObjects>d__4 <GetResumeSnapshotNetworkSceneObjects>d__ = new NetworkRunner.<GetResumeSnapshotNetworkSceneObjects>d__4(-2);
			<GetResumeSnapshotNetworkSceneObjects>d__.<>4__this = this;
			return <GetResumeSnapshotNetworkSceneObjects>d__;
		}

		private void SetHostMigrationBandwidth(int bytePerSecond)
		{
		}

		private IEnumerator RunHostMigrationResume(NetworkRunnerInitializeArgs args)
		{
			NetworkRunner.<RunHostMigrationResume>d__11 <RunHostMigrationResume>d__ = new NetworkRunner.<RunHostMigrationResume>d__11(0);
			<RunHostMigrationResume>d__.<>4__this = this;
			<RunHostMigrationResume>d__.args = args;
			return <RunHostMigrationResume>d__;
		}

		private unsafe NetworkObject GetNetworkObjectFromResumeSnapshot(NetworkObjectHeaderPtr networkObjectPtr, Dictionary<NetworkId, NetworkObjectHeaderPtr> headerList, Dictionary<NetworkId, List<NetworkId>> nestedMapping)
		{
			bool isSceneObject = networkObjectPtr.Ptr->Type.IsSceneObject;
			NetworkObject result;
			if (isSceneObject)
			{
				result = null;
			}
			else
			{
				bool dontDestroyOnLoad = (networkObjectPtr.Ptr->Flags & NetworkObjectHeaderFlags.DontDestroyOnLoad) == NetworkObjectHeaderFlags.DontDestroyOnLoad;
				NetworkObject networkObject;
				bool flag = this.TryAcquireInstance(networkObjectPtr.Ptr->Type, null, out networkObject, true, dontDestroyOnLoad) == NetworkRunner.CreateInstanceResult.Success;
				if (flag)
				{
					this.InitializeTempNetworkObjectInstance(networkObjectPtr.Ptr, networkObject);
					List<NetworkId> list;
					bool flag2 = nestedMapping.TryGetValue(networkObjectPtr.Ptr->Id, out list);
					if (flag2)
					{
						for (int i = 0; i < list.Count; i++)
						{
							NetworkId key = list[i];
							NetworkObjectHeaderPtr networkObjectHeaderPtr = headerList[key];
							NetworkObject instance = networkObject.NestedObjects[i];
							Assert.Check(networkObjectHeaderPtr.Ptr->NestingRoot.Equals(networkObjectPtr.Ptr->Id), "Nested NetworkObject with wrong NetworkId for the Nesting Root");
							this.InitializeTempNetworkObjectInstance(networkObjectHeaderPtr.Ptr, instance);
						}
					}
				}
				else
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("Failed to create instance for {0}", *networkObjectPtr.Ptr));
					}
				}
				result = networkObject;
			}
			return result;
		}

		private unsafe void InitializeTempNetworkObjectInstance(NetworkObjectHeader* header, NetworkObject instance)
		{
			instance.Ptr = (int*)header;
			int num = NetworkStructUtils.GetWordCount<NetworkObjectHeader>();
			for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
			{
				instance.NetworkedBehaviours[i].WordOffset = num;
				instance.NetworkedBehaviours[i].WordCount = NetworkBehaviourUtils.GetWordCount(instance.NetworkedBehaviours[i]);
				instance.NetworkedBehaviours[i].MakeOwned(this, instance, i);
				instance.NetworkedBehaviours[i].Ptr = instance.Ptr + num;
				num += NetworkBehaviourUtils.GetWordCount(instance.NetworkedBehaviours[i]);
			}
		}

		internal void SetupHostMigration(HostMigration hostMigration)
		{
			this._lastHostMigrationInfo = hostMigration;
		}

		internal void StartHostMigration(Snapshot snapshot = null)
		{
			TraceLogStream logTraceHostMigration = InternalLogStreams.LogTraceHostMigration;
			if (logTraceHostMigration != null)
			{
				logTraceHostMigration.Log(string.Format("StartHostMigration: Has Snapshot? {0}", snapshot != null));
			}
			Assert.Always(this._lastHostMigrationInfo != null, "Invalid Host Migration info");
			GameMode gameMode = GameMode.Client;
			PeerMode peerMode = this._lastHostMigrationInfo.PeerMode;
			PeerMode peerMode2 = peerMode;
			if (peerMode2 != PeerMode.Server)
			{
				if (peerMode2 != PeerMode.Client)
				{
					Assert.Fail("Invalid New Game Mode on Host Migration.");
				}
				else
				{
					gameMode = GameMode.Client;
				}
			}
			else
			{
				gameMode = GameMode.Host;
			}
			CloudCommunicator cloudCommunicator = this._cloudServices.ExtractCommunicator();
			HostMigrationToken migrationToken = new HostMigrationToken(snapshot, cloudCommunicator, gameMode);
			this.InvokeHostMigration(migrationToken);
		}

		private void InvokeHostMigration(HostMigrationToken migrationToken)
		{
			try
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					this._callbacks[i].OnHostMigration(this, migrationToken);
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
		}

		internal Task<bool> SendHostMigrationSnapshot()
		{
			bool flag = !this.IsServer || this.GameMode != GameMode.Host || !this.IsInitialized || !this.IsCloudReady;
			Task<bool> result;
			if (flag)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Warn("Fusion peer is not running or is not connected to the Photon Cloud. Ignore.");
				}
				result = Task.FromResult<bool>(false);
			}
			else
			{
				switch (this._cloudServices.CurrentJoinStage)
				{
				case JoinProcessStage.Idle:
				case JoinProcessStage.Joining:
				{
					DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
					if (logDebug2 != null)
					{
						logDebug2.Warn("Fusion peer is waiting for Join confirmation. Ignore.");
					}
					return Task.FromResult<bool>(true);
				}
				case JoinProcessStage.Fail:
				{
					DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
					if (logDebug3 != null)
					{
						logDebug3.Warn("Fusion peer failed to join Session. Ignore.");
					}
					return Task.FromResult<bool>(false);
				}
				}
				bool flag2 = this._cloudServices.CurrentProtocolMessageVersion < ProtocolMessageVersion.V1_4_0;
				if (flag2)
				{
					DebugLogStream logDebug4 = InternalLogStreams.LogDebug;
					if (logDebug4 != null)
					{
						logDebug4.Warn("Fusion Plugin does not support Host Migration. Ignore.");
					}
					result = Task.FromResult<bool>(false);
				}
				else
				{
					bool flag3 = this.LastConfirmedSnapshotTick < this.LastSnapshotTick;
					if (flag3)
					{
						DebugLogStream logDebug5 = InternalLogStreams.LogDebug;
						if (logDebug5 != null)
						{
							logDebug5.Warn(string.Format("Host Snapshot Confirmed for Tick {0} was not confirmed yet. Ignore.", this.LastSnapshotTick));
						}
						result = Task.FromResult<bool>(false);
					}
					else
					{
						bool flag4 = this._hostSnapshotTempData == null;
						if (flag4)
						{
							this._hostSnapshotTempData = new byte[4096];
						}
						result = Task.Run<bool>(delegate()
						{
							int lastSnapshotTick = this.LastSnapshotTick;
							Tick value;
							uint lastId;
							int snapshotSize;
							bool flag5 = this.GetServerSnapshot(ref this._hostSnapshotTempData, out value, out lastId, out snapshotSize) && lastSnapshotTick == Interlocked.CompareExchange(ref this.LastSnapshotTick, value, lastSnapshotTick);
							bool result2;
							if (flag5)
							{
								this._cloudServices.SendStateSnapshot(this._hostSnapshotTempData, snapshotSize, value, lastId);
								result2 = true;
							}
							else
							{
								result2 = false;
							}
							return result2;
						}, this.OperationsCancellationToken);
					}
				}
			}
			return result;
		}

		private bool GetServerSnapshot(ref byte[] data, out Tick tick, out uint idCounter, out int length)
		{
			Simulation.Server server = this._simulation as Simulation.Server;
			bool flag = server != null;
			bool result;
			if (flag)
			{
				length = server.WriteHostMigrationData(ref data, data.Length);
				tick = server.Tick;
				idCounter = server.IdCounter;
				result = (length > 0);
			}
			else
			{
				tick = 0;
				idCounter = 0U;
				length = 0;
				result = false;
			}
			return result;
		}

		public static NetworkRunner.BuildTypes BuildType
		{
			get
			{
				return NetworkRunner.BuildTypes.Debug;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event NetworkRunner.ObjectDelegate ObjectAcquired;

		internal static void ResetStatics()
		{
			NetworkRunner._instances.Clear();
		}

		public bool IsSimulationUpdating
		{
			get
			{
				return this._simulationPhase == NetworkRunner.SimulationPhase.Update;
			}
		}

		private void OnValidate()
		{
			bool flag = base.GetComponent<NetworkObject>();
			if (flag)
			{
				Debug.LogWarning("NetworkRunner will not work properly with NetworkObject on the same GameObject.");
			}
		}

		internal bool IsInitialized
		{
			get
			{
				return this._initializeOperation != null && this._initializeOperation.Task.IsCompleted && this._initializeOperation.Task.Result;
			}
		}

		public bool ProvideInput
		{
			get
			{
				return this._provideInput.GetValueOrDefault();
			}
			set
			{
				this._provideInput = new bool?(value);
			}
		}

		public Topologies Topology
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.Config.Topology : ((Topologies)0);
			}
		}

		internal Simulation Simulation
		{
			get
			{
				return this._simulation;
			}
		}

		public SimulationModes Mode
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.Mode : ((SimulationModes)0);
			}
		}

		public SimulationStages Stage
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.Stage : ((SimulationStages)0);
			}
		}

		public float DeltaTime
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.DeltaTime : 0f;
			}
		}

		public float SimulationTime
		{
			get
			{
				Simulation simulation = this._simulation;
				return (float)((simulation != null) ? simulation.Time : 0.0);
			}
		}

		public float LocalRenderTime
		{
			get
			{
				Simulation simulation = this._simulation;
				float val = (float)(((simulation != null) ? new double?(simulation._time.Now().Local) : null) - (double)this.DeltaTime).GetValueOrDefault();
				return Math.Max(val, 0f);
			}
		}

		public float RemoteRenderTime
		{
			get
			{
				Simulation simulation = this._simulation;
				return (float)((simulation != null) ? simulation._time.Now().Remote : 0.0);
			}
		}

		public bool IsRunning
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsRunning;
			}
		}

		public bool IsShutdown
		{
			get
			{
				return this._simulationShutdown > (NetworkRunner.ShutdownFlags)0;
			}
		}

		internal bool IsShutdownDeferred
		{
			get
			{
				return this._deferredShutdownParams.ShutdownRequested;
			}
		}

		private bool IsRegularShutdown
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this._simulationShutdown & NetworkRunner.ShutdownFlags.Regular) > (NetworkRunner.ShutdownFlags)0;
			}
		}

		public float LocalAlpha
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.LocalAlpha : 0f;
			}
		}

		public Tick LatestServerTick
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.LatestServerTick : 0;
			}
		}

		public bool IsStarting
		{
			get
			{
				return !this.IsRunning && !this.IsShutdown;
			}
		}

		public bool IsClient
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsClient;
			}
		}

		public bool IsConnectedToServer
		{
			get
			{
				return this.IsClient && ((Simulation.Client)this._simulation).IsConnectedToServer;
			}
		}

		public bool IsServer
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsServer;
			}
		}

		public bool IsPlayer
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsPlayer;
			}
		}

		public bool IsSinglePlayer
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsSinglePlayer;
			}
		}

		public bool IsLastTick
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsLastTick;
			}
		}

		public bool IsFirstTick
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsFirstTick;
			}
		}

		public bool IsForward
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsForward;
			}
		}

		public bool IsResimulation
		{
			get
			{
				Simulation simulation = this._simulation;
				return simulation != null && simulation.IsResimulation;
			}
		}

		public int TickRate
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.TickRate : 0;
			}
		}

		public NetworkRunner.States State
		{
			get
			{
				return this.IsShutdown ? NetworkRunner.States.Shutdown : (this.IsRunning ? NetworkRunner.States.Running : NetworkRunner.States.Starting);
			}
		}

		public PlayerRef LocalPlayer
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.LocalPlayer : default(PlayerRef);
			}
		}

		public Tick Tick
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.Tick : default(Tick);
			}
		}

		public NetworkProjectConfig Config
		{
			get
			{
				return this._config;
			}
		}

		public NetworkPrefabTable Prefabs
		{
			get
			{
				NetworkProjectConfig config = this._config;
				return (config != null) ? config.PrefabTable : null;
			}
		}

		public int TicksExecuted
		{
			get
			{
				return this._ticksExecuted;
			}
		}

		public IEnumerable<PlayerRef> ActivePlayers
		{
			get
			{
				Simulation simulation = this._simulation;
				return ((simulation != null) ? simulation.ActivePlayers : null) ?? Enumerable.Empty<PlayerRef>();
			}
		}

		public INetworkObjectProvider ObjectProvider
		{
			get
			{
				return this._objectProvider;
			}
		}

		public int ReliableDataSendRate
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.ReliableDataSendRate : 0;
			}
			set
			{
				bool flag = this._simulation != null;
				if (flag)
				{
					this._simulation.ReliableDataSendRate = value;
				}
				else
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("Cannot set ReliableDataSendRate before NetworkRunner has started. Await the completion of the StartGame task before accessing this property.");
					}
				}
			}
		}

		public NetAddress LocalAddress
		{
			get
			{
				Simulation simulation = this._simulation;
				return (simulation != null) ? simulation.LocalAddress : default(NetAddress);
			}
		}

		public INetworkSceneManager SceneManager
		{
			get
			{
				return this._sceneManager;
			}
		}

		internal CancellationToken OperationsCancellationToken
		{
			get
			{
				bool flag = this.OperationsCancellationTokenSource == null || this.OperationsCancellationTokenSource.IsCancellationRequested;
				CancellationToken result;
				if (flag)
				{
					LogStream logWarn = InternalLogStreams.LogWarn;
					if (logWarn != null)
					{
						logWarn.Log("Trying to access an invalid OperationsCancellationTokenSource");
					}
					result = CancellationToken.None;
				}
				else
				{
					result = this.OperationsCancellationTokenSource.Token;
				}
				return result;
			}
		}

		public HitboxManager LagCompensation
		{
			get
			{
				return base.GetBehaviour<HitboxManager>();
			}
		}

		public void Disconnect(PlayerRef player, byte[] token = null)
		{
			bool flag = this._simulation != null;
			if (flag)
			{
				Simulation.Server server = this._simulation as Simulation.Server;
				bool flag2 = server != null;
				if (flag2)
				{
					server.Disconnect(player, token);
				}
				else
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(this, "Only server can disconnect players");
					}
				}
			}
		}

		internal void Disconnect(NetAddress address)
		{
			bool flag = this._simulation != null;
			if (flag)
			{
				Simulation.Server server = this._simulation as Simulation.Server;
				bool flag2 = server != null;
				if (flag2)
				{
					server.Disconnect(address);
				}
				else
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(this, "Only server can disconnect players");
					}
				}
			}
		}

		internal void Connect(NetAddress address, byte[] token, byte[] uniqueId)
		{
			bool isServer = this.IsServer;
			if (isServer)
			{
				throw new InvalidOperationException("Only clients can connect");
			}
			((Simulation.Client)this.Simulation).Connect(address, token, uniqueId);
		}

		[EditorButton("Shutdown", EditorButtonVisibility.PlayMode, 0, false)]
		internal void ShutdownAction()
		{
			this.Shutdown(true, ShutdownReason.Ok, false);
		}

		public Task Shutdown(bool destroyGameObject = true, ShutdownReason shutdownReason = ShutdownReason.Ok, bool forceShutdownProcedure = false)
		{
			NetworkRunner.<>c__DisplayClass145_0 CS$<>8__locals1 = new NetworkRunner.<>c__DisplayClass145_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.destroyGameObject = destroyGameObject;
			CS$<>8__locals1.shutdownReason = shutdownReason;
			bool flag = this._simulationPhase > NetworkRunner.SimulationPhase.None;
			Task result;
			if (flag)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Log(this, string.Format("Deferring shutdown with ({0}, {1}, {2}) due to phase being {3}", new object[]
					{
						CS$<>8__locals1.destroyGameObject,
						CS$<>8__locals1.shutdownReason,
						forceShutdownProcedure,
						this._simulationPhase
					}));
				}
				this._deferredShutdownParams = new NetworkRunner.DeferredShutdownParams
				{
					ShutdownRequested = true,
					ShutdownReason = CS$<>8__locals1.shutdownReason,
					DestroyGO = CS$<>8__locals1.destroyGameObject
				};
				Simulation simulation = this._simulation;
				if (simulation != null)
				{
					simulation.NotifyWaitingForShutdown();
				}
				result = Task.CompletedTask;
			}
			else
			{
				this._deferredShutdownParams = default(NetworkRunner.DeferredShutdownParams);
				DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
				if (logDebug2 != null)
				{
					logDebug2.Log(this, string.Format("Starting to Shutdown with ({0}, {1}, {2})", CS$<>8__locals1.destroyGameObject, CS$<>8__locals1.shutdownReason, forceShutdownProcedure));
				}
				this.RegisterNetworkCallbacks();
				bool isShutdown = this.IsShutdown;
				if (isShutdown)
				{
					NetworkRunner.RemoveInstance(this);
					bool flag2 = !this.IsRegularShutdown && forceShutdownProcedure;
					if (flag2)
					{
						CS$<>8__locals1.<Shutdown>g__InvokeOnShutdownCallbacks|1();
						result = CS$<>8__locals1.<Shutdown>g__ContinueTasksWithDestroy|0(new Task[]
						{
							this.DisconnectFromCloud()
						});
					}
					else
					{
						result = Task.CompletedTask;
					}
				}
				else
				{
					this._simulationShutdown |= NetworkRunner.ShutdownFlags.Regular;
					try
					{
						Simulation simulation2 = this._simulation;
						if (simulation2 != null)
						{
							simulation2.ShutdownNativeSocket();
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
					try
					{
						INetworkRunnerUpdater updater = this._updater;
						if (updater != null)
						{
							updater.Shutdown(this);
						}
					}
					catch (Exception error2)
					{
						LogStream logException2 = InternalLogStreams.LogException;
						if (logException2 != null)
						{
							logException2.Log(error2);
						}
					}
					NetworkRunner.RemoveInstance(this);
					CS$<>8__locals1.<Shutdown>g__InvokeOnShutdownCallbacks|1();
					Simulation simulation3 = this._simulation;
					bool flag3 = ((simulation3 != null) ? simulation3.Objects : null) != null;
					if (flag3)
					{
						foreach (NetworkObjectMeta networkObjectMeta in this._simulation.Objects.Values.ToList<NetworkObjectMeta>())
						{
							bool flag4 = BehaviourUtils.IsAlive(networkObjectMeta.Instance);
							if (flag4)
							{
								this.DetachInstance(networkObjectMeta.Instance, false, false);
							}
						}
					}
					List<SimulationBehaviour> list = new List<SimulationBehaviour>();
					SimulationBehaviourUpdater behaviourUpdater = this._behaviourUpdater;
					if (behaviourUpdater != null)
					{
						behaviourUpdater.GetAllSimulationBehaviours(list);
					}
					foreach (SimulationBehaviour simulationBehaviour in list)
					{
						IDespawned despawned = simulationBehaviour as IDespawned;
						bool flag5 = despawned != null;
						if (flag5)
						{
							try
							{
								despawned.Despawned(this, false);
							}
							catch (Exception error3)
							{
								LogStream logException3 = InternalLogStreams.LogException;
								if (logException3 != null)
								{
									logException3.Log(error3);
								}
							}
						}
					}
					INetworkObjectProvider objectProvider = this._objectProvider;
					if (objectProvider != null)
					{
						objectProvider.Shutdown(this);
					}
					Simulation simulation4 = this._simulation;
					if (simulation4 != null)
					{
						simulation4.Dispose();
					}
					this._simulation = null;
					INetworkSceneManager sceneManager = this._sceneManager;
					if (sceneManager != null)
					{
						sceneManager.Shutdown();
					}
					this._sceneManager = null;
					this.GameMode = (GameMode)0;
					this.SessionInfo = new SessionInfo(null);
					Task task = this.DisconnectFromCloud();
					result = CS$<>8__locals1.<Shutdown>g__ContinueTasksWithDestroy|0(new Task[]
					{
						task
					});
				}
			}
			return result;
		}

		private INetSocket CreateCloudSocket()
		{
			bool flag = this._cloudServices == null || !this._cloudServices.IsCloudReady;
			if (flag)
			{
				throw new InvalidOperationException("Fusion Relay Client is not ready. Make sure the call Runner.ConnectToCloud before start with Runner.StartGame");
			}
			bool flag2 = !this._cloudServices.IsNATPunchthroughEnabled || RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
			INetSocket result;
			if (flag2)
			{
				result = new NetSocketRelay(this._cloudServices.Communicator);
			}
			else
			{
				result = new NetSocketHybrid(this._cloudServices.Communicator);
			}
			return result;
		}

		private void SetInitializationDone(NetworkRunnerInitializeArgs args)
		{
			TaskCompletionSource<bool> initializeOperation = this._initializeOperation;
			if (initializeOperation != null)
			{
				initializeOperation.TrySetResult(true);
			}
			CloudServices cloudServices = this._cloudServices;
			if (cloudServices != null)
			{
				cloudServices.StartBackgroundCloudServices();
			}
		}

		internal void OnRuntimeConfigReady()
		{
			this.OnGameStartedInvoked = true;
			while (this._spawnedSimBehaviourQueue.Count > 0)
			{
				ISpawned spawned = this._spawnedSimBehaviourQueue.Dequeue();
				try
				{
					spawned.Spawned();
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(error);
					}
				}
			}
			AsyncOperationHandler<ShutdownReason> startGameOperation = this._startGameOperation;
			if (startGameOperation != null)
			{
				startGameOperation.SetResult(ShutdownReason.Ok);
			}
			this.InvokeOnGameStartedCallback();
		}

		private bool TryGetInterfaceWithDefaultType<T>(string defaultTypeName, out T result) where T : class
		{
			result = base.GetComponent<T>();
			bool flag = result != null;
			bool result2;
			if (flag)
			{
				result2 = true;
			}
			else
			{
				Type type = Type.GetType(defaultTypeName);
				bool flag2 = type != null;
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log(this, "No " + typeof(T).FullName + " provided and there is no matching component, creating " + defaultTypeName);
					}
					result = (T)((object)base.gameObject.AddComponent(type));
					result2 = true;
				}
				else
				{
					result = default(T);
					result2 = false;
				}
			}
			return result2;
		}

		internal void InvokeOnGameStartedCallback()
		{
			try
			{
				Action<NetworkRunner> onGameStartAction = this._onGameStartAction;
				if (onGameStartAction != null)
				{
					onGameStartAction(this);
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
		}

		internal Task<bool> Initialize(NetworkRunnerInitializeArgs args)
		{
			this._initializeOperation = new TaskCompletionSource<bool>();
			this._onGameStartAction = args.OnGameStarted;
			this.OnGameStartedInvoked = false;
			this._spawnedSimBehaviourQueue = new Queue<ISpawned>();
			bool flag = args.SimulationMode == null;
			if (flag)
			{
				throw new InvalidOperationException("SimulationMode must have a value");
			}
			bool flag2 = args.Address == null && !args.IsSinglePlayer;
			if (flag2)
			{
				throw new InvalidOperationException("Address must have a value");
			}
			bool flag3 = args.Config == null;
			if (flag3)
			{
				throw new InvalidOperationException("Config must have a value");
			}
			bool flag4 = this._callbacks == null;
			if (flag4)
			{
				this._callbacks = new List<INetworkRunnerCallbacks>();
			}
			bool isSinglePlayer = args.IsSinglePlayer;
			INetSocket netSocket;
			if (isSinglePlayer)
			{
				netSocket = new NetSocketNull();
			}
			else
			{
				netSocket = this.CreateCloudSocket();
			}
			Assert.Check(netSocket);
			this._config = NetworkRunner.SetupNetworkProjectConfig(args);
			this._connectionToken = args.ConnectionToken;
			this._spawnQueue = new Queue<NetworkRunner.SpawnArgs>();
			Object.DontDestroyOnLoad(base.gameObject);
			bool flag5 = args.ObjectProvider == null;
			if (flag5)
			{
				string text = "Fusion.NetworkObjectProviderDefault, Fusion.Unity";
				bool flag6 = !this.TryGetInterfaceWithDefaultType<INetworkObjectProvider>(text, out this._objectProvider);
				if (flag6)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(this, "No ObjectProvider passed and the default provider component type (" + text + ") was not found. Fusion will not be able to spawn prefabs.");
					}
					this._objectProvider = new NetworkObjectProviderDummy();
				}
			}
			else
			{
				this._objectProvider = args.ObjectProvider;
			}
			SimulationArgs args2;
			args2.Mode = args.SimulationMode.Value;
			args2.Config = this._config;
			args2.Callbacks = this;
			args2.Socket = netSocket;
			args2.Address = args.Address.GetValueOrDefault();
			args2.ResumeTick = args.ResumeTick.GetValueOrDefault();
			args2.ResumeState = args.ResumeState;
			args2.ResumeNetworkId = args.ResumeId.GetValueOrDefault();
			bool isServer = args2.IsServer;
			if (isServer)
			{
				this._simulation = new Simulation.Server(args2);
			}
			else
			{
				args2.ResumeTick = default(Tick);
				args2.ResumeState = null;
				args2.ResumeNetworkId = default(NetworkId);
				this._simulation = new Simulation.Client(args2);
			}
			this._simulation.Runner = this;
			this._behaviourUpdater = new SimulationBehaviourUpdater(this._config);
			this._behaviourUpdater.BuildTypeOrder(args.CustomCallbackInterfaces);
			this._simulationShutdown = (NetworkRunner.ShutdownFlags)0;
			this._deferredShutdownParams = default(NetworkRunner.DeferredShutdownParams);
			bool flag7 = args.SceneManager == null;
			if (flag7)
			{
				string text2 = "Fusion.NetworkSceneManagerDefault, Fusion.Unity";
				bool flag8 = !this.TryGetInterfaceWithDefaultType<INetworkSceneManager>(text2, out this._sceneManager);
				if (flag8)
				{
					LogStream logError2 = InternalLogStreams.LogError;
					if (logError2 != null)
					{
						logError2.Log(this, "No SceneManager passed and the default provider component type (" + text2 + ") was not found. Fusion will not be able to attach to scene NetworkObjects.");
					}
					this._sceneManager = new NetworkSceneManagerDummy();
				}
			}
			else
			{
				this._sceneManager = args.SceneManager;
			}
			this._sceneInfoInitial = (this._sceneInfoSnapshot = args.Scene.GetValueOrDefault());
			bool flag9 = args.Updater == null;
			if (flag9)
			{
				this._updater = new NetworkRunnerUpdaterDefault();
			}
			else
			{
				this._updater = args.Updater;
			}
			bool flag10 = args.ObjectInitializer == null;
			if (flag10)
			{
				this._objectInitializer = new NetworkObjectInitializerUnity();
			}
			else
			{
				this._objectInitializer = args.ObjectInitializer;
			}
			INetworkObjectProvider objectProvider = this._objectProvider;
			if (objectProvider != null)
			{
				objectProvider.Initialize(this);
			}
			try
			{
				this._sceneManager.Initialize(this);
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			foreach (SimulationBehaviour simulationBehaviour in base.GetComponentsInChildren<SimulationBehaviour>())
			{
				bool enabled = simulationBehaviour.enabled;
				if (enabled)
				{
					this.AddSimulationBehaviour(simulationBehaviour);
				}
			}
			bool enabled2 = this._config.LagCompensation.Enabled;
			if (enabled2)
			{
				this.GetSingleton<HitboxManager>();
			}
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this, string.Format("Starting with {0}:\n{1}", "NetworkProjectConfig", this._config));
			}
			NetworkRunner.AddInstance(this);
			bool flag11 = this._provideInput == null;
			if (flag11)
			{
				this.ProvideInput = this.Simulation.IsPlayer;
			}
			try
			{
				this._updater.Initialize(this);
			}
			catch (Exception error2)
			{
				LogStream logException2 = InternalLogStreams.LogException;
				if (logException2 != null)
				{
					logException2.Log(this, error2);
				}
			}
			CloudServices cloudServices = this._cloudServices;
			NetworkRunner._cachedRegionSummary = (((cloudServices != null) ? cloudServices.CachedRegionSummary : null) ?? string.Empty);
			bool flag12 = this.Simulation.IsServer && this.Simulation.IsResume;
			if (flag12)
			{
				base.StartCoroutine(this.RunHostMigrationResume(args));
			}
			else
			{
				this.SetInitializationDone(args);
			}
			return this._initializeOperation.Task;
		}

		public void SinglePlayerPause()
		{
			this.Simulation.SinglePlayerSetPaused(true);
		}

		public void SinglePlayerContinue()
		{
			this.Simulation.SinglePlayerSetPaused(false);
		}

		public void SinglePlayerPause(bool paused)
		{
			this.Simulation.SinglePlayerSetPaused(paused);
		}

		public int GetInterfaceListsCount(Type type)
		{
			Assert.Check(type.IsInterface);
			return this._behaviourUpdater.GetCallbackCount(type);
		}

		public SimulationBehaviourListScope GetInterfaceListHead(Type type, int index, out SimulationBehaviour head)
		{
			return this._behaviourUpdater.GetCallbackHead(type, index, out head);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SimulationBehaviour GetInterfaceListPrev(SimulationBehaviour behaviour)
		{
			return behaviour.Prev;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SimulationBehaviour GetInterfaceListNext(SimulationBehaviour behaviour)
		{
			return behaviour.Next;
		}

		public static Task<List<RegionInfo>> GetAvailableRegions(string appId = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return FusionRealtimeProxy.GetEnabledRegions(appId, cancellationToken);
		}

		public int? GetPlayerActorId(PlayerRef player)
		{
			Topologies topology = this.Simulation.Config.Topology;
			Topologies topologies = topology;
			if (topologies != Topologies.ClientServer)
			{
				if (topologies == Topologies.Shared)
				{
					return this.Simulation.GetPlayerActorId(player);
				}
			}
			else
			{
				int value;
				bool flag = this.Simulation.IsServer && this._cloudServices != null && this._cloudServices.TryGetActorIdByUniqueId(this.Simulation.GetPlayerUniqueId(player), out value);
				if (flag)
				{
					return new int?(value);
				}
			}
			return null;
		}

		public string GetPlayerUserId(PlayerRef player = default(PlayerRef))
		{
			bool flag = !this.IsCloudReady;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this.LocalPlayer == player || player == default(PlayerRef);
				if (flag2)
				{
					result = this.UserId;
				}
				else
				{
					int? playerActorId = this.GetPlayerActorId(player);
					string text;
					if (playerActorId == null)
					{
						text = null;
					}
					else
					{
						CloudServices cloudServices = this._cloudServices;
						text = ((cloudServices != null) ? cloudServices.GetActorUserID(playerActorId.Value) : null);
					}
					result = text;
				}
			}
			return result;
		}

		public void SetPlayerObject(PlayerRef player, NetworkObject networkObject)
		{
			bool flag = BehaviourUtils.IsNull(networkObject) || this.Exists(networkObject);
			if (flag)
			{
				this.Simulation.SetPlayerObjectId(player, networkObject);
			}
			else
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Error(this, string.Format("Invalid {0}", networkObject));
				}
			}
		}

		public NetworkObject GetPlayerObject(PlayerRef player)
		{
			NetworkObject result;
			this.TryGetPlayerObject(player, out result);
			return result;
		}

		public bool TryGetPlayerObject(PlayerRef player, out NetworkObject networkObject)
		{
			NetworkObjectMeta networkObjectMeta;
			bool flag = player.IsRealPlayer && this.Simulation.TryGetMeta(this.Simulation.GetPlayerObjectId(player), out networkObjectMeta) && BehaviourUtils.IsAlive(networkObjectMeta.Instance);
			bool result;
			if (flag)
			{
				networkObject = networkObjectMeta.Instance;
				result = true;
			}
			else
			{
				networkObject = null;
				result = false;
			}
			return result;
		}

		public List<T> GetAllBehaviours<T>() where T : SimulationBehaviour
		{
			List<T> result = new List<T>();
			this.GetAllBehaviours<T>(result);
			return result;
		}

		public List<NetworkObject> GetAllNetworkObjects()
		{
			List<NetworkObject> list = new List<NetworkObject>();
			bool flag = this._simulation == null;
			List<NetworkObject> result;
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(this, "Simulation is not initialized.");
				}
				result = list;
			}
			else
			{
				this.GetAllNetworkObjects(list);
				result = list;
			}
			return result;
		}

		public void GetAllNetworkObjects(List<NetworkObject> result)
		{
			result.Clear();
			foreach (KeyValuePair<NetworkId, NetworkObjectMeta> keyValuePair in this._simulation.Objects)
			{
				bool flag = keyValuePair.Value.Instance;
				if (flag)
				{
					result.Add(keyValuePair.Value.Instance);
				}
			}
		}

		public void GetAllBehaviours<T>(List<T> result) where T : SimulationBehaviour
		{
			foreach (SimulationBehaviour simulationBehaviour in this.GetAllBehaviours(typeof(T)))
			{
				while (BehaviourUtils.IsNotNull(simulationBehaviour))
				{
					result.Add((T)((object)simulationBehaviour));
					simulationBehaviour = simulationBehaviour.Next;
				}
			}
		}

		public double GetPlayerRtt(PlayerRef playerRef)
		{
			return this.Simulation.GetPlayerRtt(playerRef);
		}

		public unsafe void SendRpc(SimulationMessage* message)
		{
			this.Simulation.SendMessage(ref message);
		}

		public unsafe void SendRpc(SimulationMessage* message, out RpcSendResult info)
		{
			info = new RpcSendResult
			{
				MessageSize = message->Offset,
				Result = this.Simulation.SendMessage(ref message)
			};
		}

		public bool IsPlayerValid(PlayerRef player)
		{
			return this.Simulation.PlayerValid(player);
		}

		public byte[] GetPlayerConnectionToken(PlayerRef player = default(PlayerRef))
		{
			bool flag = player == this.LocalPlayer || player == PlayerRef.None;
			byte[] result;
			if (flag)
			{
				result = this._connectionToken;
			}
			else
			{
				bool isServer = this.IsServer;
				if (isServer)
				{
					result = this.Simulation.GetPlayerConnectionToken(player);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public ConnectionType GetPlayerConnectionType(PlayerRef player)
		{
			bool flag = this.IsServer && player != this.LocalPlayer;
			if (flag)
			{
				NetAddress playerAddress = this.Simulation.GetPlayerAddress(player);
				bool flag2 = !playerAddress.Equals(default(NetAddress));
				if (flag2)
				{
					return playerAddress.IsRelayAddr ? ConnectionType.Relayed : ConnectionType.Direct;
				}
			}
			return ConnectionType.None;
		}

		public SimulationBehaviour[] GetAllBehaviours(Type type)
		{
			return this._behaviourUpdater.GetTypeHeads(type);
		}

		public void AddCallbacks(params INetworkRunnerCallbacks[] callbacks)
		{
			bool flag = this._callbacks == null;
			if (flag)
			{
				this._callbacks = new List<INetworkRunnerCallbacks>();
			}
			foreach (INetworkRunnerCallbacks item in callbacks)
			{
				bool flag2 = !this._callbacks.Contains(item);
				if (flag2)
				{
					this._callbacks.Add(item);
				}
			}
		}

		public void RemoveCallbacks(params INetworkRunnerCallbacks[] callbacks)
		{
			bool flag = this._callbacks == null;
			if (flag)
			{
				this._callbacks = new List<INetworkRunnerCallbacks>();
			}
			foreach (INetworkRunnerCallbacks item in callbacks)
			{
				bool flag2 = this._callbacks.Contains(item);
				if (flag2)
				{
					this._callbacks.Remove(item);
				}
			}
		}

		public void GetMemorySnapshot(MemoryStatisticsSnapshot.TargetAllocator targetAllocator, ref MemoryStatisticsSnapshot snapshot)
		{
			this._simulation.GetMemorySnapshot(targetAllocator, ref snapshot);
		}

		internal void OnApplicationQuit()
		{
			StunClient.Reset();
			this.Shutdown(true, ShutdownReason.Ok, false);
		}

		public void RenderInternal()
		{
			bool flag = !this.Simulation.HasObject(NetworkId.RuntimeConfig);
			if (!flag)
			{
				bool flag2 = this.IsRegularShutdown || this._simulation == null;
				if (!flag2)
				{
					bool flag3 = this._config.InvokeRenderInBatchMode || !Application.isBatchMode;
					if (flag3)
					{
						try
						{
							this._simulationPhase = NetworkRunner.SimulationPhase.Render;
							this._simulation.InterpolateSequenceIncrement();
							this._behaviourUpdater.InvokeRender();
							CallbackInterfaceInvoker.IAfterRender(this._behaviourUpdater);
						}
						finally
						{
							this._simulationPhase = NetworkRunner.SimulationPhase.None;
						}
					}
					bool shutdownRequested = this._deferredShutdownParams.ShutdownRequested;
					if (shutdownRequested)
					{
						this.Shutdown(this._deferredShutdownParams.DestroyGO, this._deferredShutdownParams.ShutdownReason, false);
					}
				}
			}
		}

		private void Awake()
		{
			bool flag = this._callbacks == null;
			if (flag)
			{
				this._callbacks = new List<INetworkRunnerCallbacks>();
			}
			this.RegisterNetworkCallbacks();
			NetworkRunner.AddInstance(this);
			TaskManager.Setup();
		}

		private void OnDisable()
		{
			this.DebugOnDisable();
		}

		private void OnDestroy()
		{
			this.DebugOnDestroy();
			this.Shutdown(false, ShutdownReason.Ok, false);
		}

		private void Update()
		{
			CloudServices cloudServices = this._cloudServices;
			if (cloudServices != null)
			{
				cloudServices.Update();
			}
		}

		public void SetMasterClient(PlayerRef player)
		{
			bool flag = this.Topology != Topologies.Shared || !this.IsSharedModeMasterClient || this.Simulation == null || player == this.LocalPlayer;
			if (!flag)
			{
				bool flag2 = !player.IsRealPlayer;
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Error(string.Format("{0} is not a valid player index.", player));
					}
				}
				else
				{
					int? playerActorId = this.Simulation.GetPlayerActorId(player);
					bool flag3 = playerActorId == null;
					if (flag3)
					{
						DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
						if (logDebug2 != null)
						{
							logDebug2.Error(string.Format("Was not possible to get the actor id for {0}.", player));
						}
					}
					else
					{
						this._cloudServices.SendChangeMasterClient(playerActorId.Value);
					}
				}
			}
		}

		public void UpdateInternal(double dt)
		{
			Assert.Check(!this._deferredShutdownParams.ShutdownRequested);
			bool flag = dt != 0.0;
			if (flag)
			{
				bool isRegularShutdown = this.IsRegularShutdown;
				if (isRegularShutdown)
				{
					return;
				}
				bool flag2 = this._simulation != null;
				if (flag2)
				{
					try
					{
						bool isPaused = this._simulation.IsPaused;
						if (isPaused)
						{
							Assert.Check(this._simulation.IsSinglePlayer, "Simulation is paused, but is not running in SinglePlayer Mode");
							return;
						}
						this._simulationPhase = NetworkRunner.SimulationPhase.Update;
						this.RegisterNetworkCallbacks();
						this.InvokeBeforeUpdate();
						this.ProcessSpawnQueue();
						this._ticksExecuted = this._simulation.Update(dt);
						int objectsAllocatorUsedSegmentsInBytes = this._simulation.GetObjectsAllocatorUsedSegmentsInBytes();
						int generalAllocatorUsedSegmentsInBytes = this._simulation.GetGeneralAllocatorUsedSegmentsInBytes();
						int objectsAllocatorFreeSegmentsInBytes = this._simulation.GetObjectsAllocatorFreeSegmentsInBytes();
						int generalAllocatorFreeSegmentsInBytes = this._simulation.GetGeneralAllocatorFreeSegmentsInBytes();
						this._simulation._fusionStatsManager.PendingSnapshot.AddToObjectsAllocMemoryUsedInBytesStat(objectsAllocatorUsedSegmentsInBytes, true);
						this._simulation._fusionStatsManager.PendingSnapshot.AddToGeneralAllocMemoryUsedInBytesStat(generalAllocatorUsedSegmentsInBytes, true);
						this._simulation._fusionStatsManager.PendingSnapshot.AddToObjectsAllocMemoryFreeInBytesStat(objectsAllocatorFreeSegmentsInBytes, true);
						this._simulation._fusionStatsManager.PendingSnapshot.AddToGeneralAllocMemoryFreeInBytesStat(generalAllocatorFreeSegmentsInBytes, true);
						this.InvokeAfterUpdate();
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(this, error);
						}
						this.Shutdown(true, ShutdownReason.Error, false);
					}
					finally
					{
						this._simulationPhase = NetworkRunner.SimulationPhase.None;
					}
				}
			}
			bool shutdownRequested = this._deferredShutdownParams.ShutdownRequested;
			if (shutdownRequested)
			{
				this.Shutdown(this._deferredShutdownParams.DestroyGO, this._deferredShutdownParams.ShutdownReason, false);
			}
			bool onGameStartedInvoked = this.OnGameStartedInvoked;
			if (onGameStartedInvoked)
			{
				Simulation simulation = this._simulation;
				if (simulation != null)
				{
					simulation._fusionStatsManager.FinishPendingSnapshot();
				}
				this._behaviourUpdater.FinishBehaviourStatisticsPendingSnapshot();
			}
		}

		private void RegisterNetworkCallbacks()
		{
			bool flag = this && base.gameObject && this._callbacks != null && this._callbacks.Count == 0;
			if (flag)
			{
				INetworkRunnerCallbacks[] componentsInChildren = base.gameObject.GetComponentsInChildren<INetworkRunnerCallbacks>();
				foreach (INetworkRunnerCallbacks networkRunnerCallbacks in componentsInChildren)
				{
					MonoBehaviour monoBehaviour = networkRunnerCallbacks as MonoBehaviour;
					bool flag2 = monoBehaviour && monoBehaviour.enabled;
					if (flag2)
					{
						this.AddCallbacks(new INetworkRunnerCallbacks[]
						{
							networkRunnerCallbacks
						});
					}
				}
			}
		}

		public void SendReliableDataToPlayer(PlayerRef player, ReliableKey key, byte[] data)
		{
			bool flag = this.Simulation.IsPlayer && this.Simulation.LocalPlayer == player;
			if (flag)
			{
				this.Simulation.Callbacks.OnReliableData(player, new ReliableId
				{
					Key = key
				}, true, data);
			}
			else
			{
				bool isServer = this.IsServer;
				if (isServer)
				{
					int? connectionIndexForPlayer = this.Simulation.GetConnectionIndexForPlayer(player);
					bool flag2 = connectionIndexForPlayer != null;
					if (flag2)
					{
						this.Simulation.SendReliableData(connectionIndexForPlayer.Value, player.AsIndex, key, data);
					}
				}
				else
				{
					this.Simulation.SendReliableData(0, player.AsIndex, key, data);
				}
			}
		}

		public void SendReliableDataToServer(ReliableKey key, byte[] data)
		{
			bool isClient = this.IsClient;
			if (isClient)
			{
				this.Simulation.SendReliableData(0, PlayerRef.None.AsIndex, key, data);
			}
			else
			{
				this.Simulation.Callbacks.OnReliableData(PlayerRef.None, new ReliableId
				{
					Key = key
				}, true, data);
			}
		}

		public void SetPlayerAlwaysInterested(PlayerRef player, NetworkObject networkObject, bool alwaysInterested)
		{
			bool flag = this.Exists(networkObject) && networkObject.HasStateAuthority;
			if (flag)
			{
				this.Simulation.SetPlayerAlwaysInterested(player, networkObject, alwaysInterested);
			}
		}

		public T? GetInputForPlayer<[IsUnmanaged] T>(PlayerRef player) where T : struct, ValueType, INetworkInput
		{
			SimulationInput inputForPlayer = this._simulation.GetInputForPlayer(player);
			bool flag = inputForPlayer != null;
			if (flag)
			{
				T value;
				bool flag2 = NetworkInput.FromRaw(inputForPlayer.Data, this.Simulation.Config.InputDataWordCount).TryGet<T>(out value);
				if (flag2)
				{
					return new T?(value);
				}
			}
			return null;
		}

		public NetworkInput? GetRawInputForPlayer(PlayerRef player)
		{
			SimulationInput inputForPlayer = this._simulation.GetInputForPlayer(player);
			bool flag = inputForPlayer != null;
			NetworkInput? result;
			if (flag)
			{
				result = new NetworkInput?(NetworkInput.FromRaw(inputForPlayer.Data, this.Simulation.Config.InputDataWordCount));
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void RequestStateAuthority(NetworkId id)
		{
			Assert.Check<NetworkId>(id.IsValid, "{0} is not a valid NetworkID.", id);
			this.Simulation.RequestStateAuthority(id, true);
		}

		public void ReleaseStateAuthority(NetworkId id)
		{
			Assert.Check<NetworkId>(id.IsValid, "{0} is not a valid NetworkID.", id);
			this.Simulation.RequestStateAuthority(id, false);
		}

		public bool TryGetInputForPlayer<[IsUnmanaged] T>(PlayerRef player, out T input) where T : struct, ValueType, INetworkInput
		{
			T? inputForPlayer = this.GetInputForPlayer<T>(player);
			bool flag = inputForPlayer != null;
			bool result;
			if (flag)
			{
				input = inputForPlayer.Value;
				result = true;
			}
			else
			{
				input = default(T);
				result = false;
			}
			return result;
		}

		public NetworkObject FindObject(NetworkId networkId)
		{
			NetworkObject result;
			this.TryFindObject(networkId, out result);
			return result;
		}

		public bool TryFindObject(NetworkId objectId, out NetworkObject networkObject)
		{
			NetworkObjectMeta networkObjectMeta;
			bool flag = this.Simulation.TryGetMeta(objectId, out networkObjectMeta) && BehaviourUtils.IsAlive(networkObjectMeta.Instance);
			bool result;
			if (flag)
			{
				networkObject = networkObjectMeta.Instance;
				result = true;
			}
			else
			{
				networkObject = null;
				result = false;
			}
			return result;
		}

		public bool TryFindBehaviour(NetworkBehaviourId behaviourId, out NetworkBehaviour behaviour)
		{
			NetworkObject networkObject;
			bool flag = this.TryFindObject(behaviourId.Object, out networkObject);
			if (flag)
			{
				bool flag2 = behaviourId.Behaviour >= 0 && behaviourId.Behaviour < networkObject.NetworkedBehaviours.Length;
				if (flag2)
				{
					behaviour = networkObject.NetworkedBehaviours[behaviourId.Behaviour];
					return true;
				}
			}
			behaviour = null;
			return false;
		}

		public bool TryFindBehaviour<T>(NetworkBehaviourId id, out T behaviour) where T : NetworkBehaviour
		{
			NetworkBehaviour networkBehaviour;
			bool flag = this.TryFindBehaviour(id, out networkBehaviour);
			bool result;
			if (flag)
			{
				result = BehaviourUtils.IsAlive(behaviour = (networkBehaviour as T));
			}
			else
			{
				behaviour = default(T);
				result = false;
			}
			return result;
		}

		public T TryGetNetworkedBehaviourFromNetworkedObjectRef<T>(NetworkId networkId) where T : NetworkBehaviour
		{
			NetworkObject networkObject;
			bool flag = this.TryFindObject(networkId, out networkObject);
			T result;
			if (flag)
			{
				T t;
				bool flag2 = networkObject.TryGetBehaviour<T>(out t);
				if (flag2)
				{
					result = t;
				}
				else
				{
					result = networkObject.GetBehaviour<T>();
				}
			}
			else
			{
				result = default(T);
			}
			return result;
		}

		public NetworkId TryGetObjectRefFromNetworkedBehaviour(NetworkBehaviour behaviour)
		{
			bool flag = BehaviourUtils.IsAlive(behaviour) && behaviour.Object.IsValid;
			NetworkId result;
			if (flag)
			{
				result = behaviour.Object.Id;
			}
			else
			{
				result = default(NetworkId);
			}
			return result;
		}

		public NetworkBehaviourId TryGetNetworkedBehaviourId(NetworkBehaviour behaviour)
		{
			bool flag = BehaviourUtils.IsAlive(behaviour) && behaviour.Object.IsValid;
			NetworkBehaviourId result;
			if (flag)
			{
				NetworkBehaviourId networkBehaviourId;
				networkBehaviourId.Behaviour = behaviour.ObjectIndex;
				networkBehaviourId.Object = behaviour.Object.Id;
				result = networkBehaviourId;
			}
			else
			{
				result = default(NetworkBehaviourId);
			}
			return result;
		}

		public bool SetIsSimulated(NetworkObject obj, bool simulate)
		{
			bool flag = this.Exists(obj);
			bool result;
			if (flag)
			{
				bool flag2 = this.Simulation.Topology == Topologies.Shared && !this.Simulation.IsLocalSimulationStateAuthority(obj.Header);
				if (flag2)
				{
					LogStream logWarn = InternalLogStreams.LogWarn;
					if (logWarn != null)
					{
						logWarn.Log("Can't set simulation state for objects you don't have state authority over in shared mode");
					}
					result = false;
				}
				else
				{
					((Simulation.ICallbacks)this).ObjectIsSimulatedChanged(obj.Id, simulate);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void SetAreaOfInterestGrid(int x, int y, int z)
		{
			bool flag = this.Topology == Topologies.Shared;
			if (flag)
			{
				throw new Exception("Can't change grid size in shared mode");
			}
			Assert.Check(x > 0);
			Assert.Check(y > 0);
			Assert.Check(z > 0);
			Simulation.AreaOfInterest.X_SIZE = x;
			Simulation.AreaOfInterest.Y_SIZE = y;
			Simulation.AreaOfInterest.Z_SIZE = z;
		}

		public void SetAreaOfInterestCellSize(int size)
		{
			bool flag = this.Topology == Topologies.Shared;
			if (flag)
			{
				throw new Exception("Can't change cell size in shared mode");
			}
			Assert.Check(size >= 4);
			Simulation.AreaOfInterest.CELL_SIZE = size;
		}

		public List<NetworkId> GetObjectsInAreaOfInterestForPlayer(PlayerRef player)
		{
			return this._simulation.GetObjectsInAreaOfInterestForPlayer(player);
		}

		public void GetAreaOfInterestGizmoData([TupleElementNames(new string[]
		{
			"center",
			"size",
			"playerCount",
			"objectCount"
		})] List<ValueTuple<Vector3, Vector3, int, int>> result)
		{
			bool flag = this._simulation != null;
			if (flag)
			{
				this._simulation.GetAreaOfInterestGizmoData(result);
			}
		}

		public bool TryGetFusionStatistics(out FusionStatisticsManager statisticsManager)
		{
			bool flag = this._simulation != null;
			bool result;
			if (flag)
			{
				statisticsManager = this._simulation._fusionStatsManager;
				result = true;
			}
			else
			{
				statisticsManager = null;
				result = false;
			}
			return result;
		}

		public bool TryGetBehaviourStatistics(Type behaviourType, out BehaviourStatisticsSnapshot behaviourStatisticsSnapshot)
		{
			bool flag = this._behaviourUpdater != null;
			if (flag)
			{
				bool flag2 = this._behaviourUpdater.TryGetBehaviourStatisticsSnapshot(behaviourType, out behaviourStatisticsSnapshot);
				if (flag2)
				{
					return true;
				}
			}
			behaviourStatisticsSnapshot = null;
			return false;
		}

		public bool Exists(NetworkObject obj)
		{
			NetworkObjectMeta networkObjectMeta;
			return this._simulation != null && BehaviourUtils.IsNotNull(obj) && this._simulation.TryGetMeta(obj.Id, out networkObjectMeta) && obj == networkObjectMeta.Instance;
		}

		public bool Exists(NetworkId id)
		{
			return id.IsValid && this._simulation != null && this._simulation.HasObject(id);
		}

		public void Despawn(NetworkObject networkObject)
		{
			bool flag = BehaviourUtils.IsNull(networkObject) || networkObject.Meta == null;
			if (!flag)
			{
				Simulation simulation = this.Simulation;
				bool? flag2 = (simulation != null) ? new bool?(simulation.IsLocalSimulationStateAuthority(networkObject.Header)) : null;
				bool flag3;
				if (networkObject.StateAuthority == PlayerRef.None)
				{
					Simulation simulation2 = this.Simulation;
					flag3 = (simulation2 != null && simulation2.IsMasterClient);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				bool? flag5 = flag2;
				bool flag6 = false;
				bool flag7 = (flag5.GetValueOrDefault() == flag6 & flag5 != null) && !flag4;
				if (!flag7)
				{
					bool flag8 = this.Exists(networkObject);
					if (flag8)
					{
						bool flag9 = !BehaviourUtils.IsSame(this, networkObject.Runner);
						if (flag9)
						{
							throw new InvalidOperationException("Object does not belong to this runner");
						}
						this.Destroy(networkObject, NetworkObjectDestroyFlags.DestroyState | NetworkObjectDestroyFlags.DestroyedByDespawn);
					}
				}
			}
		}

		public T GetSingleton<T>() where T : SimulationBehaviour
		{
			T result;
			bool flag = !base.TryGetBehaviour<T>(out result);
			if (flag)
			{
				this.AddGlobal(result = base.AddBehaviour<T>());
			}
			return result;
		}

		public bool HasSingleton<T>() where T : SimulationBehaviour
		{
			T t;
			return base.TryGetBehaviour<T>(out t);
		}

		public void DestroySingleton<T>() where T : SimulationBehaviour
		{
			T t;
			bool flag = base.TryGetBehaviour<T>(out t);
			if (flag)
			{
				t.MakeUnowned();
				this.RemoveGlobal(t);
				Behaviour.DestroyBehaviour(t);
			}
		}

		public void AddGlobal(SimulationBehaviour instance)
		{
			Assert.Check(instance.Runner == null);
			Assert.Check(instance.Object == null);
			Assert.Check(!(instance is NetworkBehaviour));
			this.AddSimulationBehaviour(instance);
		}

		public void RemoveGlobal(SimulationBehaviour instance)
		{
			Assert.Check(instance.Runner == this);
			Assert.Check(instance.Object == null);
			Assert.Check(!(instance is NetworkBehaviour));
			this.RemoveSimulationBehavior(instance);
		}

		internal void AddSimulationBehaviour(SimulationBehaviour behaviour)
		{
			Assert.Always(BehaviourUtils.IsAlive(behaviour), "Behaviour is not alive");
			Assert.Always(!(behaviour is NetworkBehaviour), "NetworkBehaviour should not be added to SimulationBehaviour list");
			behaviour.Flags |= SimulationBehaviourRuntimeFlags.IsGlobal;
			behaviour.Flags &= ~SimulationBehaviourRuntimeFlags.IsUnityDisabled;
			behaviour.MakeOwned(this, null);
			bool flag = this._behaviourUpdater == null;
			if (flag)
			{
				throw new NullReferenceException("SimulationBehaviourUpdater is null. Are you trying to AddSimulationBehaviour on a NetworkRunner which has not yet been started?");
			}
			this._behaviourUpdater.AddBehaviour(behaviour, false);
			ISpawned spawned = behaviour as ISpawned;
			bool flag2 = spawned != null;
			if (flag2)
			{
				bool onGameStartedInvoked = this.OnGameStartedInvoked;
				if (onGameStartedInvoked)
				{
					try
					{
						spawned.Spawned();
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(error);
						}
					}
				}
				else
				{
					this._spawnedSimBehaviourQueue.Enqueue(spawned);
				}
			}
		}

		internal void RemoveSimulationBehavior(SimulationBehaviour behaviour)
		{
			behaviour.MakeUnowned();
			bool flag = this._behaviourUpdater == null;
			if (flag)
			{
				throw new NullReferenceException("SimulationBehaviourUpdater is null. Are you trying to RemoveSimulationBehavior on a NetworkRunner which has not yet been started?");
			}
			this._behaviourUpdater.RemoveBehaviour(behaviour);
			IDespawned despawned = behaviour as IDespawned;
			bool flag2 = despawned != null;
			if (flag2)
			{
				try
				{
					despawned.Despawned(this, false);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(error);
					}
				}
			}
		}

		internal void Destroy(NetworkObject networkObject, NetworkObjectDestroyFlags flags)
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(networkObject, string.Format("Destroy flags:{0}", flags));
			}
			bool flag = this.Exists(networkObject);
			int count = this._destroyIdsBuffer.Count;
			bool flag2 = flag;
			if (flag2)
			{
				this._destroyIdsBuffer.Add(networkObject.Id);
				bool flag3 = networkObject.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.OwnsNestedObjects);
				if (flag3)
				{
					foreach (NetworkObject networkObject2 in networkObject.NestedObjects)
					{
						this._destroyIdsBuffer.Add(networkObject2.Id);
					}
				}
			}
			int count2 = this._destroyIdsBuffer.Count;
			this.DetachInstance(networkObject, flags.Get(NetworkObjectDestroyFlags.DestroyedByEngine), flag);
			bool flag4 = flag;
			if (flag4)
			{
				Assert.Check(count2 <= this._destroyIdsBuffer.Count);
				Assert.Check(count <= this._destroyIdsBuffer.Count);
				for (int j = count; j < count2; j++)
				{
					NetworkId id = this._destroyIdsBuffer[j];
					bool flag5 = this.Exists(id);
					if (flag5)
					{
						this._simulation.Destroy(id, flags, default(PlayerRef));
					}
				}
				this._destroyIdsBuffer.RemoveRange(count, count2 - count);
			}
			NetworkObjectMeta meta = networkObject.Meta;
			if (meta != null)
			{
				meta.UnlinkInstance(networkObject);
			}
			Assert.Check(networkObject.Meta == null);
		}

		internal void DetachInstance(NetworkObject obj, bool destroyedByEngine, bool hasState)
		{
			bool flag = BehaviourUtils.IsNull(obj);
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			Assert.Check<NetworkRunner, NetworkRunner>(BehaviourUtils.IsSame(obj.Runner, this) || BehaviourUtils.IsNull(obj.Runner), "Runner mismatch; expected {0} or null and being disabled, but was {1}", this, obj.Runner);
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(obj, string.Format("PerformPrefabCleanup destroyedByEngine: {0}, hasState: {1}", destroyedByEngine, hasState));
			}
			NetworkId id = obj.Id;
			NetworkObjectTypeId networkTypeId = obj.NetworkTypeId;
			bool isNested = obj.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.IsNested);
			NetworkObject.ObjectInterestModes objectInterest = obj.ObjectInterest;
			NetworkObject.ObjectInterestModes objectInterestModes = objectInterest;
			if (objectInterestModes != NetworkObject.ObjectInterestModes.AreaOfInterest)
			{
				if (objectInterestModes == NetworkObject.ObjectInterestModes.Global)
				{
					Simulation simulation = this._simulation;
					if (simulation != null)
					{
						simulation.RemoveFromGlobalObjectInterest(id);
					}
				}
			}
			else
			{
				Simulation simulation2 = this._simulation;
				NetworkObjectMeta meta;
				bool flag2 = simulation2 != null && simulation2.TryGetMeta(obj.Id, out meta);
				if (flag2)
				{
					this._simulation.AOI_RemoveFromAreaOfInterest(meta, false);
				}
			}
			bool isValid = id.IsValid;
			if (isValid)
			{
				bool flag3 = obj.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.Spawned);
				if (flag3)
				{
					this.InvokeDespawnedCallback(obj, hasState);
				}
				else
				{
					TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
					if (logTraceObject2 != null)
					{
						logTraceObject2.Log(obj, "Not despawning when cleaning up, not spawned");
					}
				}
				bool flag4 = !destroyedByEngine && obj.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.OwnsNestedObjects);
				bool flag5 = flag4;
				if (flag5)
				{
					foreach (NetworkObject networkObject in obj.NestedObjects)
					{
						bool flag6 = networkObject.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.Spawned);
						if (flag6)
						{
							this.InvokeDespawnedCallback(networkObject, hasState);
						}
					}
				}
				this.FreeObject(obj);
				bool flag7 = flag4;
				if (flag7)
				{
					foreach (NetworkObject networkObject2 in obj.NestedObjects)
					{
						bool flag8 = BehaviourUtils.IsAlive(networkObject2) && networkObject2.Id.IsValid;
						if (flag8)
						{
							this.FreeObject(networkObject2);
						}
					}
				}
			}
			else
			{
				obj.ResetNetworkState();
			}
			bool flag9 = this._attachableInstances.Remove(networkTypeId);
			NetworkObjectReleaseContext networkObjectReleaseContext = new NetworkObjectReleaseContext(obj, networkTypeId, destroyedByEngine, isNested);
			TraceLogStream logTraceObject3 = InternalLogStreams.LogTraceObject;
			if (logTraceObject3 != null)
			{
				logTraceObject3.Log(obj, string.Format("Releasing {0} (preexisting: {1})", networkObjectReleaseContext, flag9));
			}
			this._objectProvider.ReleaseInstance(this, networkObjectReleaseContext);
		}

		private unsafe void FreeObject(NetworkObject obj)
		{
			for (int i = 0; i < obj.NetworkedBehaviours.Length; i++)
			{
				NetworkBehaviour networkBehaviour = obj.NetworkedBehaviours[i];
				this._behaviourUpdater.RemoveBehaviour(networkBehaviour);
				networkBehaviour.MakeUnowned();
				networkBehaviour.Ptr = default(int*);
			}
			obj.ResetNetworkState();
		}

		public void Attach(NetworkObject networkObject, PlayerRef? inputAuthority = null, bool allocate = true, bool? masterClientObjectOverride = null)
		{
			bool flag = BehaviourUtils.IsNull(networkObject);
			if (flag)
			{
				throw new ArgumentNullException("networkObject");
			}
			bool flag2 = !networkObject.NetworkTypeId.IsValid;
			if (flag2)
			{
				throw new ArgumentException("NetworkObject has invalid NetworkTypeId", "networkObject");
			}
			if (allocate)
			{
				this.InvokeObjectAcquired(networkObject);
			}
			this.InitializeNetworkObjectAssignRunner(networkObject, null, false);
			this._attachableInstances.Add(networkObject.NetworkTypeId, networkObject);
			if (allocate)
			{
				Simulation simulation = this.Simulation;
				NetworkId nextId = this.Simulation.GetNextId();
				int wordCount = NetworkObject.GetWordCount(networkObject);
				NetworkObjectTypeId networkTypeId = networkObject.NetworkTypeId;
				int behaviourCount = networkObject.NetworkedBehaviours.Length;
				NetworkObjectHeaderFlags flags = this.FlagsFromInstance(networkObject);
				NetworkObjectMeta meta = simulation.AllocateObject(nextId, wordCount, networkTypeId, behaviourCount, default(NetworkId), default(NetworkObjectNestingKey), flags);
				this.InitializeNetworkObjectInstance(meta, networkObject, inputAuthority, NetworkRunner.AttachOptions.LocalSpawn, masterClientObjectOverride);
				bool flag3 = NetworkRunner.IsAwakeAtInitialization(networkObject);
				if (flag3)
				{
					this.InitializeNetworkObjectState(networkObject);
					this.InvokeBeforeSpawnedCallbacks(networkObject, NetworkRunner.AttachOptions.LocalSpawn, null);
					this.InvokeSpawnedCallback(networkObject);
					this.InvokeAfterSpawnedCallback(networkObject);
				}
			}
			else
			{
				networkObject.gameObject.SetActive(false);
			}
		}

		public void AddPlayerAreaOfInterest(PlayerRef player, Vector3 center, float radius)
		{
			bool flag = this.IsServer && this.LocalPlayer == player;
			if (!flag)
			{
				bool isClient = this.IsClient;
				if (isClient)
				{
					bool flag2 = this.Topology == Topologies.ClientServer || player != this.LocalPlayer;
					if (!flag2)
					{
						bool flag3 = radius > 300f;
						if (flag3)
						{
							DebugLogStream logDebug = InternalLogStreams.LogDebug;
							if (logDebug != null)
							{
								logDebug.Warn(string.Format("Area of Interest Radius has been exceeded. Clamping to {0}", 300));
							}
							radius = 300f;
						}
						SimulationMessageInternal_SetAreaOfInterest buffer;
						buffer.Center = center;
						buffer.Radius = radius;
						this.Simulation.SendInternalSimulationMessage<SimulationMessageInternal_SetAreaOfInterest>(SimulationMessageInternalTypes.SetAreaOfInterest, buffer, null);
					}
				}
				else
				{
					SimulationConnection simulationConnection;
					bool flag4 = this.Simulation.TryGetSimulationConnectionForPlayer(player, out simulationConnection);
					if (flag4)
					{
						Simulation.AreaOfInterest.SphereToCells(center, radius, simulationConnection.AreaOfInterestCells);
					}
				}
			}
		}

		public void ClearPlayerAreaOfInterest(PlayerRef player)
		{
			bool flag = this.IsServer && this.LocalPlayer == player;
			if (!flag)
			{
				bool isClient = this.IsClient;
				if (!isClient)
				{
					SimulationConnection simulationConnectionForPlayer = this.Simulation.GetSimulationConnectionForPlayer(player);
					if (simulationConnectionForPlayer != null)
					{
						simulationConnectionForPlayer.AreaOfInterestCells.Clear();
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool? IsInterestedIn(NetworkObject obj, PlayerRef player)
		{
			bool flag = this._simulation != null && BehaviourUtils.IsNotNull(obj) && obj.Meta != null;
			bool? result;
			if (flag)
			{
				result = this._simulation.IsInterestedIn(obj.Meta, player);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void SetBehaviourReplicateToAll(NetworkBehaviour behaviour, bool replicate)
		{
			bool isClient = this.IsClient;
			if (!isClient)
			{
				behaviour.DefaultReplicated = replicate;
				foreach (SimulationConnection sc in this._simulation._connections.Values)
				{
					this.SetBehaviourReplicateTo(behaviour, sc, replicate, false);
				}
			}
		}

		public void SetBehaviourReplicateTo(NetworkBehaviour behaviour, PlayerRef player, bool replicate)
		{
			bool isClient = this.IsClient;
			if (!isClient)
			{
				SimulationConnection sc;
				bool flag = this._simulation.TryGetSimulationConnectionForPlayer(player, out sc);
				if (flag)
				{
					this.SetBehaviourReplicateTo(behaviour, sc, replicate, true);
				}
			}
		}

		private void SetBehaviourReplicateTo(NetworkBehaviour behaviour, SimulationConnection sc, bool replicate, bool forceCreate)
		{
			bool flag = this.Exists(behaviour.Object);
			if (flag)
			{
				NetworkObjectConnectionData objectData = sc.GetObjectData(behaviour.Object.Id, forceCreate, false);
				bool flag2 = objectData == null;
				if (!flag2)
				{
					ulong num = 1UL << behaviour.ObjectIndex;
					bool flag3 = (objectData.Filter & num) == num;
					bool flag4 = flag3 == replicate;
					if (!flag4)
					{
						if (replicate)
						{
							objectData.Filter |= num;
						}
						else
						{
							objectData.Filter &= ~num;
						}
						objectData.TickSent = 0;
						objectData.TickAcknowledged = 0;
					}
				}
			}
		}

		public void Attach(NetworkObject[] networkObjects, PlayerRef? inputAuthority = null, bool allocate = true, bool? masterClientObjectOverride = null)
		{
			bool flag = networkObjects == null;
			if (flag)
			{
				throw new ArgumentNullException("networkObjects");
			}
			for (int i = 0; i < networkObjects.Length; i++)
			{
				NetworkObject networkObject = networkObjects[i];
				bool flag2 = networkObject == null;
				if (flag2)
				{
					throw new ArgumentException(string.Format("NetworkObject[{0}] is null", i), "networkObjects");
				}
				bool flag3 = !networkObject.NetworkTypeId.IsValid;
				if (flag3)
				{
					throw new ArgumentException(string.Format("NetworkObject[{0}] has an invalid type id", i), "networkObjects");
				}
				if (allocate)
				{
					this.InvokeObjectAcquired(networkObject);
				}
				bool flag4 = !NetworkRunner.IsPreexistingAtInitialization(networkObject);
				if (flag4)
				{
					this.InitializeNetworkObjectAssignRunner(networkObject, null, false);
				}
			}
			foreach (NetworkObject networkObject2 in networkObjects)
			{
				bool flag5 = this._attachableInstances.ContainsKey(networkObject2.NetworkTypeId);
				if (flag5)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(this, string.Format("Object with type id {0} has already been attached or is waiting to be attached to", networkObject2.NetworkTypeId));
					}
				}
				else
				{
					this._attachableInstances.Add(networkObject2.NetworkTypeId, networkObject2);
				}
			}
			if (allocate)
			{
				foreach (NetworkObject networkObject3 in networkObjects)
				{
					bool flag6 = NetworkRunner.IsPreexistingAtInitialization(networkObject3);
					if (flag6)
					{
						Assert.Check(networkObject3.Meta != null);
					}
					else
					{
						Simulation simulation = this.Simulation;
						NetworkId nextId = this.Simulation.GetNextId();
						int wordCount = NetworkObject.GetWordCount(networkObject3);
						NetworkObjectTypeId networkTypeId = networkObject3.NetworkTypeId;
						int behaviourCount = networkObject3.NetworkedBehaviours.Length;
						NetworkObjectHeaderFlags flags = this.FlagsFromInstance(networkObject3);
						NetworkObjectMeta meta = simulation.AllocateObject(nextId, wordCount, networkTypeId, behaviourCount, default(NetworkId), default(NetworkObjectNestingKey), flags);
						this.InitializeNetworkObjectInstance(meta, networkObject3, inputAuthority, NetworkRunner.AttachOptions.LocalSpawn, masterClientObjectOverride);
					}
				}
				foreach (NetworkObject networkObject4 in networkObjects)
				{
					bool flag7 = NetworkRunner.IsAwakeAtInitialization(networkObject4) && !NetworkRunner.IsPreexistingAtInitialization(networkObject4);
					if (flag7)
					{
						this.InitializeNetworkObjectState(networkObject4);
					}
				}
				foreach (NetworkObject networkObject5 in networkObjects)
				{
					bool flag8 = NetworkRunner.IsAwakeAtInitialization(networkObject5);
					if (flag8)
					{
						this.InvokeBeforeSpawnedCallbacks(networkObject5, NetworkRunner.IsPreexistingAtInitialization(networkObject5) ? ((NetworkRunner.AttachOptions)0) : NetworkRunner.AttachOptions.LocalSpawn, null);
					}
				}
				foreach (NetworkObject networkObject6 in networkObjects)
				{
					bool flag9 = NetworkRunner.IsAwakeAtInitialization(networkObject6);
					if (flag9)
					{
						this.InvokeSpawnedCallback(networkObject6);
					}
				}
				foreach (NetworkObject networkObject7 in networkObjects)
				{
					bool flag10 = NetworkRunner.IsAwakeAtInitialization(networkObject7);
					if (flag10)
					{
						this.InvokeAfterSpawnedCallback(networkObject7);
					}
				}
				bool flag11 = this.IsSharedModeMasterClient && (this._simulation.Config.SchedulingEnabled || this._simulation.Config.AreaOfInterestEnabled);
				if (flag11)
				{
					foreach (NetworkObject obj in networkObjects)
					{
						this._simulation.GetSimulationConnectionForPlayer(this._simulation.LocalPlayer).GetObjectData(obj, true, false);
					}
				}
			}
			else
			{
				foreach (NetworkObject networkObject8 in networkObjects)
				{
					networkObject8.gameObject.SetActive(false);
				}
			}
		}

		internal void AttachActivatedByUser(NetworkObject networkObject)
		{
			NetworkRunner.AttachOptions attachOptions = NetworkRunner.NetworkObjectFlagsToAttachOptions(networkObject.RuntimeFlags);
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(networkObject, string.Format("AttachActivatedByUser ({0})", attachOptions));
			}
			Assert.Check<LogUtils.DumpDeferredClass>(networkObject.IsValid, "Expected object to be valid {0}", LogUtils.GetDump<NetworkObject>(networkObject));
			Assert.Check<LogUtils.DumpDeferredClass>(networkObject.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.NotAwakeWhenAttaching), "Expected not awake when attaching {0}", LogUtils.GetDump<NetworkObject>(networkObject));
			bool flag = (attachOptions & NetworkRunner.AttachOptions.LocalSpawn) == NetworkRunner.AttachOptions.LocalSpawn;
			if (flag)
			{
				this.InitializeNetworkObjectState(networkObject);
			}
			this.InvokeBeforeSpawnedCallbacks(networkObject, attachOptions, null);
			this.InvokeSpawnedCallback(networkObject);
			this.InvokeAfterSpawnedCallback(networkObject);
		}

		public int RegisterSceneObjects(SceneRef scene, NetworkObject[] objects, NetworkSceneLoadId loadId = default(NetworkSceneLoadId))
		{
			bool flag = !scene.IsValid;
			if (flag)
			{
				throw new ArgumentException("scene");
			}
			bool flag2 = objects == null;
			if (flag2)
			{
				throw new ArgumentNullException("objects");
			}
			objects = (from o in objects
			where !o.IsValid
			select o).ToArray<NetworkObject>();
			int result = 0;
			foreach (NetworkObject networkObject in objects)
			{
				networkObject.NetworkTypeId = NetworkObjectTypeId.FromSceneRefAndObjectIndex(scene, result++, loadId);
			}
			bool isSharedModeMasterClient = this.IsSharedModeMasterClient;
			if (isSharedModeMasterClient)
			{
				Assert.Check(this.IsSceneAuthority);
				foreach (NetworkId id in this._remoteCreateQueue)
				{
					NetworkObjectMeta networkObjectMeta;
					bool flag3 = !this._simulation.TryGetMeta(id, out networkObjectMeta);
					if (!flag3)
					{
						bool flag4 = !networkObjectMeta.Type.IsSceneObject;
						if (!flag4)
						{
							NetworkSceneObjectId asSceneObjectId = networkObjectMeta.Type.AsSceneObjectId;
							bool flag5 = asSceneObjectId.Scene != scene || asSceneObjectId.LoadId != loadId || asSceneObjectId.ObjectId >= objects.Length;
							if (!flag5)
							{
								NetworkObject networkObject2 = objects[asSceneObjectId.ObjectId];
								bool flag6 = NetworkRunner.IsPreexistingAtInitialization(networkObject2);
								if (flag6)
								{
									TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
									if (logTraceObject != null)
									{
										logTraceObject.Warn(networkObject2, string.Format("Object already marked as preexisting for {0}, ignoring {1}", networkObjectMeta.Type, networkObjectMeta.Id));
									}
								}
								else
								{
									this.InitializeNetworkObjectAssignRunner(networkObject2, null, false);
									networkObject2.RuntimeFlags |= NetworkObjectRuntimeFlags.PreexistingObject;
									this.InitializeNetworkObjectInstance(networkObjectMeta, networkObject2, null, (NetworkRunner.AttachOptions)0, null);
									TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
									if (logTraceObject2 != null)
									{
										logTraceObject2.Log(networkObject2, string.Format("Preexisting object {0} found and initialized", networkObjectMeta.Type));
									}
								}
							}
						}
					}
				}
			}
			NetworkObject[] networkObjects = objects;
			bool isSceneAuthority = this.IsSceneAuthority;
			this.Attach(networkObjects, null, isSceneAuthority, null);
			return result;
		}

		internal void InvokeOnBeforeHitboxRegistration()
		{
			EngineProfiler.Begin("NetworkRunner.InvokeOnBeforeHitboxRegistration");
			CallbackInterfaceInvoker.IBeforeHitboxRegistration(this._behaviourUpdater);
			EngineProfiler.End();
		}

		private unsafe NetworkRunner.CreateInstanceResult TryAcquireInstance(NetworkObjectTypeId typeId, NetworkObjectMeta meta, out NetworkObject result, bool synchronous = true, bool dontDestroyOnLoad = false)
		{
			bool flag = meta != null;
			if (flag)
			{
				Assert.Check<NetworkObjectTypeId, NetworkObjectTypeId>(meta.Type == typeId, "Header's type mismatch {0} vs {1}", meta.Type, typeId);
			}
			bool isNone = typeId.IsNone;
			NetworkRunner.CreateInstanceResult result2;
			if (isNone)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(string.Format("Invalid type id: {0}, header: {1}", typeId, (meta != null) ? (*meta.Header) : "null"));
				}
				result = null;
				result2 = NetworkRunner.CreateInstanceResult.Failed;
			}
			else
			{
				result = null;
				bool flag2 = true;
				NetworkObject networkObject;
				bool flag3 = this._attachableInstances.TryGetValue(typeId, out networkObject);
				NetworkObjectAcquireResult networkObjectAcquireResult;
				if (flag3)
				{
					networkObjectAcquireResult = NetworkObjectAcquireResult.Success;
					flag2 = false;
				}
				else
				{
					bool isSceneObject = typeId.IsSceneObject;
					if (isSceneObject)
					{
						networkObjectAcquireResult = NetworkObjectAcquireResult.Retry;
					}
					else
					{
						NetworkPrefabAcquireContext networkPrefabAcquireContext = new NetworkPrefabAcquireContext(typeId.AsPrefabId, meta, synchronous, dontDestroyOnLoad);
						try
						{
							networkObjectAcquireResult = this._objectProvider.AcquirePrefabInstance(this, networkPrefabAcquireContext, out networkObject);
						}
						catch (Exception ex)
						{
							LogStream logError2 = InternalLogStreams.LogError;
							if (logError2 != null)
							{
								logError2.Log(string.Format("{0}.{1} threw an exception for {2}: {3}", new object[]
								{
									"INetworkObjectProvider",
									"AcquirePrefabInstance",
									typeId,
									ex
								}));
							}
							return NetworkRunner.CreateInstanceResult.Failed;
						}
					}
				}
				switch (networkObjectAcquireResult)
				{
				case NetworkObjectAcquireResult.Success:
				{
					bool flag4 = BehaviourUtils.IsAlive(networkObject);
					if (flag4)
					{
						TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
						if (logTraceObject != null)
						{
							logTraceObject.Log(networkObject, string.Format("Acquired instance of {0} for {1}", typeId, (meta != null) ? meta.Id : default(NetworkId)));
						}
						result = networkObject;
						bool flag5 = flag2;
						if (flag5)
						{
							this.InitializeNetworkObjectAssignRunner(networkObject, new NetworkObjectTypeId?(typeId), false);
						}
						else
						{
							Assert.Always<LogUtils.DumpDeferredClass, NetworkRunner, NetworkRunner>(networkObject.Runner == this, "Instance is not owned by this runner {0} {1} {2}", LogUtils.GetDump<NetworkObject>(networkObject), this, networkObject.Runner);
						}
						this.InvokeObjectAcquired(result);
						Assert.Check(networkObject.Runner == this);
						result2 = NetworkRunner.CreateInstanceResult.Success;
					}
					else
					{
						LogStream logError3 = InternalLogStreams.LogError;
						if (logError3 != null)
						{
							logError3.Log(string.Format("{0}.{1} returned {2}, but the instance is not alive", "INetworkObjectProvider", "AcquirePrefabInstance", NetworkObjectAcquireResult.Success));
						}
						result2 = NetworkRunner.CreateInstanceResult.Failed;
					}
					break;
				}
				case NetworkObjectAcquireResult.Failed:
					result2 = NetworkRunner.CreateInstanceResult.Failed;
					break;
				case NetworkObjectAcquireResult.Retry:
					result2 = NetworkRunner.CreateInstanceResult.InProgress;
					break;
				case NetworkObjectAcquireResult.Ignore:
					result2 = NetworkRunner.CreateInstanceResult.Ignore;
					break;
				default:
				{
					LogStream logError4 = InternalLogStreams.LogError;
					if (logError4 != null)
					{
						logError4.Log(string.Format("Unknown result from {0}.{1}: {2}", "INetworkObjectProvider", "AcquirePrefabInstance", networkObjectAcquireResult));
					}
					result2 = NetworkRunner.CreateInstanceResult.Failed;
					break;
				}
				}
			}
			return result2;
		}

		private void InitializeNetworkObjectAssignRunner(NetworkObject instance, NetworkObjectTypeId? typeId = null, bool isNestedObject = false)
		{
			Assert.Always<LogUtils.DumpDeferredClass>(!instance.Id.IsValid, "The instance has already been initialized {0}", LogUtils.GetDump<NetworkObject>(instance));
			Assert.Check(instance.Ptr == null);
			Assert.Always<LogUtils.DumpDeferredClass, NetworkRunner>(instance.Runner == null, "The {0} is already owned {1}", LogUtils.GetDump<NetworkObject>(instance), instance.Runner);
			bool flag = typeId != null;
			if (flag)
			{
				Assert.Check<LogUtils.DumpDeferredClass>(instance.NetworkTypeId == default(NetworkObjectTypeId) || instance.NetworkTypeId == typeId.Value, LogUtils.GetDump<NetworkObject>(instance));
				instance.NetworkTypeId = typeId.Value;
			}
			else
			{
				bool flag2 = !isNestedObject;
				if (flag2)
				{
					Assert.Always<LogUtils.DumpDeferredClass>(instance.NetworkTypeId != default(NetworkObjectTypeId), "The instance has no type id {0}", LogUtils.GetDump<NetworkObject>(instance));
				}
			}
			instance.MakeOwned(this);
			for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
			{
				instance.NetworkedBehaviours[i].MakeOwned(this, instance, i);
			}
			bool flag3 = !instance.NetworkTypeId.IsSceneObject && !isNestedObject;
			if (flag3)
			{
				foreach (NetworkObject instance2 in instance.NestedObjects)
				{
					this.InitializeNetworkObjectAssignRunner(instance2, null, true);
				}
			}
			Assert.Check<LogUtils.DumpDeferredClass, NetworkObjectRuntimeFlags>((instance.RuntimeFlags & NetworkObjectRuntimeFlags.ClearMask) == NetworkObjectRuntimeFlags.None, "Had some leftover runtime flags {0} {1}", LogUtils.GetDump<NetworkObject>(instance), instance.RuntimeFlags & NetworkObjectRuntimeFlags.ClearMask);
			instance.RuntimeFlags &= ~NetworkObjectRuntimeFlags.ClearMask;
			instance.PrepareBehaviourOrder();
			bool flag4 = !instance.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.HadAwake);
			if (flag4)
			{
				Assert.Check(!instance.gameObject.activeInHierarchy);
				bool flag5 = !instance.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.NotAwakeWhenAttaching);
				if (flag5)
				{
					instance.RuntimeFlags |= NetworkObjectRuntimeFlags.NotAwakeWhenAttaching;
					this.AddInactiveObjectGuard(instance);
				}
			}
		}

		private NetworkObjectHeaderFlags FlagsFromInstance(NetworkObject instance)
		{
			NetworkObjectHeaderFlags networkObjectHeaderFlags = (NetworkObjectHeaderFlags)0;
			bool flag = (instance.Flags & NetworkObjectFlags.AllowStateAuthorityOverride) == NetworkObjectFlags.AllowStateAuthorityOverride;
			if (flag)
			{
				bool flag2 = (instance.Flags & NetworkObjectFlags.MasterClientObject) == NetworkObjectFlags.None;
				if (flag2)
				{
					networkObjectHeaderFlags |= NetworkObjectHeaderFlags.AllowStateAuthorityOverride;
				}
			}
			bool flag3 = (instance.Flags & NetworkObjectFlags.DestroyWhenStateAuthorityLeaves) == NetworkObjectFlags.DestroyWhenStateAuthorityLeaves;
			if (flag3)
			{
				bool flag4 = (instance.Flags & NetworkObjectFlags.MasterClientObject) == NetworkObjectFlags.None;
				if (flag4)
				{
					networkObjectHeaderFlags |= NetworkObjectHeaderFlags.DestroyWhenStateAuthorityLeaves;
				}
			}
			NetworkObject.ObjectInterestModes objectInterestModes = instance.ObjectInterest;
			bool flag5 = objectInterestModes == NetworkObject.ObjectInterestModes.AreaOfInterest;
			if (flag5)
			{
				bool flag6 = (instance.RuntimeFlags & NetworkObjectRuntimeFlags.HasMainNetworkTRSP) == NetworkObjectRuntimeFlags.HasMainNetworkTRSP;
				if (flag6)
				{
					networkObjectHeaderFlags |= NetworkObjectHeaderFlags.AreaOfInterest;
				}
				else
				{
					objectInterestModes = NetworkObject.ObjectInterestModes.Global;
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Warn(instance, "Networked object does not have a main NetworkTRSP behaviour but has ObjectInterest set to AreaOfInterest, forcing ObjectInterest to AllPlayers to ensure consistency");
					}
				}
			}
			bool flag7 = (instance.RuntimeFlags & NetworkObjectRuntimeFlags.HasMainNetworkTRSP) == NetworkObjectRuntimeFlags.HasMainNetworkTRSP;
			if (flag7)
			{
				Assert.Check(instance.NetworkedBehaviours.Any((NetworkBehaviour x) => x is NetworkTRSP));
				networkObjectHeaderFlags |= NetworkObjectHeaderFlags.HasMainNetworkTRSP;
			}
			bool flag8 = objectInterestModes == NetworkObject.ObjectInterestModes.Global;
			if (flag8)
			{
				networkObjectHeaderFlags |= NetworkObjectHeaderFlags.GlobalObjectInterest;
			}
			return networkObjectHeaderFlags;
		}

		private unsafe void InitializeNetworkObjectInstance(NetworkObjectMeta meta, NetworkObject instance, PlayerRef? inputAuthority, NetworkRunner.AttachOptions options, bool? masterClientObjectOverride)
		{
			Assert.Always<LogUtils.DumpDeferredClass, NetworkId>(!instance.Id.IsValid, "The instance has already been initialized {0} {1}", LogUtils.GetDump<NetworkObject>(instance), meta.Id);
			Assert.Check(instance.Ptr == null);
			Assert.Check(instance.Runner == this, "Should have called InitializeNetworkObjectAssignRunner before");
			Assert.Check(BehaviourUtils.IsNull(meta.Instance));
			bool flag = (options & NetworkRunner.AttachOptions.LocalSpawn) == NetworkRunner.AttachOptions.LocalSpawn;
			meta.LinkInstance(instance);
			this.UnityPreInitialize(meta, options);
			Assert.Check<short, int>((int)meta.BehaviourCount == instance.NetworkedBehaviours.Length, "Behaviour count mismatch {0} {1}", meta.BehaviourCount, instance.NetworkedBehaviours.Length);
			int num = 20;
			for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
			{
				instance.NetworkedBehaviours[i].Flags &= ~(SimulationBehaviourRuntimeFlags.IsGlobal | SimulationBehaviourRuntimeFlags.InSimulation | SimulationBehaviourRuntimeFlags.PendingRemoval | SimulationBehaviourRuntimeFlags.SkipNextUpdate);
				int wordCount = NetworkBehaviourUtils.GetWordCount(instance.NetworkedBehaviours[i]);
				instance.NetworkedBehaviours[i].WordOffset = num;
				instance.NetworkedBehaviours[i].WordCount = wordCount;
				instance.NetworkedBehaviours[i].Ptr = instance.Ptr + num;
				num += wordCount;
			}
			bool flag2 = flag;
			if (flag2)
			{
				instance.Defaults();
				bool flag3 = this._simulation.Topology == Topologies.Shared;
				if (flag3)
				{
					bool valueOrDefault = masterClientObjectOverride.GetValueOrDefault((instance.Flags & NetworkObjectFlags.MasterClientObject) == NetworkObjectFlags.MasterClientObject);
					if (valueOrDefault)
					{
						bool isMasterClient = this.Simulation.IsMasterClient;
						if (isMasterClient)
						{
							*meta.StateAuthority = PlayerRef.MasterClient;
						}
						else
						{
							LogStream logError = InternalLogStreams.LogError;
							if (logError != null)
							{
								logError.Log(instance, "Non-master clients cannot spawn with MasterClient authority, spawning with local authority instead. This is caused by passing SharedModeStateAuthMasterClient or when \"Is Master Client Object\" is checked in the inspector and SharedModeStateAuthLocalPlayer flag is not passed.");
							}
							*meta.StateAuthority = this.LocalPlayer;
						}
					}
					else
					{
						*meta.StateAuthority = this.LocalPlayer;
					}
				}
				this.Simulation.Replicator.OnObjectSpawnedLocal(meta.Id);
			}
			bool flag4 = inputAuthority != null;
			if (flag4)
			{
				instance.AssignInputAuthority(inputAuthority.Value);
			}
			bool flag5 = flag;
			if (flag5)
			{
				bool flag6 = (meta.Flags & NetworkObjectHeaderFlags.GlobalObjectInterest) == NetworkObjectHeaderFlags.GlobalObjectInterest;
				if (flag6)
				{
					this._simulation.AddToGlobalObjectInterest(meta);
				}
			}
			this._behaviourUpdater.AddObject(this, instance, this._simulation.IsInTick, this.Simulation.IsLocalSimulationStateAuthority(meta.Header) || this.Simulation.IsLocalSimulationInputAuthority(meta.Header));
		}

		private void UnityPreInitialize(NetworkObjectMeta meta, NetworkRunner.AttachOptions options)
		{
			NetworkObject instance = meta.Instance;
			bool networkIdIsObjectName = this._config.NetworkIdIsObjectName;
			if (networkIdIsObjectName)
			{
				instance.gameObject.name = meta.Id.ToNamePrefixString() + instance.gameObject.name;
			}
			bool flag = instance.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.HadAwake);
			if (flag)
			{
				bool flag2 = !instance.gameObject.activeSelf;
				if (flag2)
				{
					TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
					if (logTraceObject != null)
					{
						logTraceObject.Log(instance, "Activating when initializing");
					}
					instance.gameObject.SetActive(true);
				}
			}
			else
			{
				Assert.Check<LogUtils.DumpDeferredClass>(instance.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.NotAwakeWhenAttaching), "Expected not to be awoken {0}", LogUtils.GetDump<NetworkObject>(instance));
				Assert.Check<LogUtils.DumpDeferredClass>(!instance.gameObject.activeInHierarchy, "Expected not be active {0}", LogUtils.GetDump<NetworkObject>(instance));
				TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
				if (logTraceObject2 != null)
				{
					logTraceObject2.Log(instance, "Delaying activation");
				}
				instance.RuntimeFlags |= NetworkRunner.AttachOptionsToNetworkObjectFlags(options);
			}
		}

		private void InitializeNetworkObjectState(NetworkObject instance)
		{
			Assert.Check<LogUtils.DumpDeferredClass>(instance.Id.IsValid, "Already despawned {0}", LogUtils.GetDump<NetworkObject>(instance));
			Assert.Check<LogUtils.DumpDeferredClass>(instance.Ptr != null, "Already despawned {0}", LogUtils.GetDump<NetworkObject>(instance));
			Assert.Check(this._objectInitializer != null);
			this._objectInitializer.InitializeNetworkState(instance);
		}

		private void InvokeBeforeSpawnedCallbacks(NetworkObject instance, NetworkRunner.AttachOptions options, NetworkRunner.OnBeforeSpawned onBeforeSpawned)
		{
			bool flag = (options & NetworkRunner.AttachOptions.LocalSpawn) == NetworkRunner.AttachOptions.LocalSpawn;
			if (flag)
			{
				for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
				{
					ILocalPrefabCreated localPrefabCreated = instance.NetworkedBehaviours[i] as ILocalPrefabCreated;
					bool flag2 = localPrefabCreated != null;
					if (flag2)
					{
						try
						{
							localPrefabCreated.LocalPrefabCreated();
						}
						catch (Exception error)
						{
							LogStream logException = InternalLogStreams.LogException;
							if (logException != null)
							{
								logException.Log(error);
							}
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < instance.NetworkedBehaviours.Length; j++)
				{
					IRemotePrefabCreated remotePrefabCreated = instance.NetworkedBehaviours[j] as IRemotePrefabCreated;
					bool flag3 = remotePrefabCreated != null;
					if (flag3)
					{
						try
						{
							remotePrefabCreated.RemotePrefabCreated();
						}
						catch (Exception error2)
						{
							LogStream logException2 = InternalLogStreams.LogException;
							if (logException2 != null)
							{
								logException2.Log(error2);
							}
						}
					}
				}
			}
			bool flag4 = onBeforeSpawned != null;
			if (flag4)
			{
				try
				{
					onBeforeSpawned(this, instance);
				}
				catch (Exception error3)
				{
					LogStream logException3 = InternalLogStreams.LogException;
					if (logException3 != null)
					{
						logException3.Log(this, error3);
					}
				}
			}
		}

		private void InvokeSpawnedCallback(NetworkObject instance)
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(instance, "Spawning");
			}
			Assert.Check<BehaviourUtils.NameDeferred, NetworkId, int, BehaviourUtils.NameDeferred>((instance.RuntimeFlags & NetworkObjectRuntimeFlags.Spawned) == NetworkObjectRuntimeFlags.None, "Already spawned {0} {1} {2} {3}", BehaviourUtils.GetName(this), instance.Id, instance.GetHashCode(), BehaviourUtils.GetName(instance));
			instance.RuntimeFlags |= NetworkObjectRuntimeFlags.Spawned;
			for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
			{
				instance.NetworkedBehaviours[i].DebugNotifySpawned();
				instance.NetworkedBehaviours[i].PreSpawned();
				try
				{
					instance.NetworkedBehaviours[i].Spawned();
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(error);
					}
				}
			}
		}

		internal void InvokeDespawnedCallback(NetworkObject instance, bool hasState)
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(instance, string.Format("Despawning {0}", hasState));
			}
			Assert.Check<string>((instance.RuntimeFlags & NetworkObjectRuntimeFlags.Spawned) > NetworkObjectRuntimeFlags.None, "Not spawned {0}", instance.Name);
			instance.RuntimeFlags &= ~NetworkObjectRuntimeFlags.Spawned;
			for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
			{
				instance.NetworkedBehaviours[i].DebugNotifyDespawned();
				try
				{
					instance.NetworkedBehaviours[i].Despawned(this, hasState);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(error);
					}
				}
			}
		}

		private void InvokeAfterSpawnedCallback(NetworkObject instance)
		{
			for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
			{
				IAfterSpawned afterSpawned = instance.NetworkedBehaviours[i] as IAfterSpawned;
				bool flag = afterSpawned != null;
				if (flag)
				{
					try
					{
						afterSpawned.AfterSpawned();
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(error);
						}
					}
				}
			}
		}

		private void InvokeObjectAcquired(NetworkObject instance)
		{
			NetworkRunner.ObjectDelegate objectAcquired = this.ObjectAcquired;
			if (objectAcquired != null)
			{
				objectAcquired(this, instance);
			}
		}

		private void InvokeBeforeUpdate()
		{
			CallbackInterfaceInvoker.IBeforeUpdate(this._behaviourUpdater);
		}

		private void InvokeAfterUpdate()
		{
			CallbackInterfaceInvoker.IAfterUpdate(this._behaviourUpdater);
		}

		internal static NetworkProjectConfig SetupNetworkProjectConfig(NetworkRunnerInitializeArgs args)
		{
			return args.Config.Init(8, args.PlayerCount, new int?(Math.Max(NetworkInputUtils.GetMaxWordCount(), args.InputWordCount.GetValueOrDefault() + 1)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RpcTargetStatus GetRpcTargetStatus(PlayerRef target)
		{
			return this.Simulation.GetRpcTargetStatus(target);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasAnyActiveConnections()
		{
			return this.Simulation.HasAnyActiveConnections();
		}

		private static NetworkObjectRuntimeFlags AttachOptionsToNetworkObjectFlags(NetworkRunner.AttachOptions options)
		{
			NetworkObjectRuntimeFlags networkObjectRuntimeFlags = NetworkObjectRuntimeFlags.None;
			bool flag = (options & NetworkRunner.AttachOptions.LocalSpawn) == NetworkRunner.AttachOptions.LocalSpawn;
			if (flag)
			{
				networkObjectRuntimeFlags |= NetworkObjectRuntimeFlags.AttachOptionLocalSpawn;
			}
			return networkObjectRuntimeFlags;
		}

		private static NetworkRunner.AttachOptions NetworkObjectFlagsToAttachOptions(NetworkObjectRuntimeFlags flags)
		{
			NetworkRunner.AttachOptions attachOptions = (NetworkRunner.AttachOptions)0;
			bool flag = (flags & NetworkObjectRuntimeFlags.AttachOptionLocalSpawn) == NetworkObjectRuntimeFlags.AttachOptionLocalSpawn;
			if (flag)
			{
				attachOptions |= NetworkRunner.AttachOptions.LocalSpawn;
			}
			return attachOptions;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsAwakeAtInitialization(NetworkObject obj)
		{
			return obj.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.HadAwake);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsPreexistingAtInitialization(NetworkObject obj)
		{
			return obj.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.PreexistingObject);
		}

		private void DebugOnDestroy()
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace != null)
			{
				logTrace.Log(this, "OnDestroy");
			}
		}

		private void DebugOnDisable()
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace != null)
			{
				logTrace.Log(this, "OnDisable");
			}
		}

		internal static bool TryGetPrettyRunnerName(StringBuilder output, NetworkRunner runner)
		{
			bool flag;
			if (runner == null)
			{
				flag = true;
			}
			else
			{
				NetworkProjectConfig config = runner.Config;
				flag = (((config != null) ? new NetworkProjectConfig.PeerModes?(config.PeerMode) : null).GetValueOrDefault() != NetworkProjectConfig.PeerModes.Multiple);
			}
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				Simulation simulation = runner.Simulation;
				PlayerRef playerRef = (simulation != null) ? simulation.LocalPlayer : default(PlayerRef);
				bool isRealPlayer = playerRef.IsRealPlayer;
				if (isRealPlayer)
				{
					output.Append("[P").Append(playerRef.AsIndex).Append("] ");
				}
				else
				{
					output.Append("[P-] ");
				}
				output.Append(runner.DebugNameThreadSafe);
				result = true;
			}
			return result;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetAllSimulationStatics()
		{
			NetworkBehaviourUtils.ResetStatics();
			NetworkInputUtils.ResetStatics();
			NetworkRunner.ResetStatics();
			NetworkStructUtils.ResetStatics();
		}

		internal void SetupEncryption(EncryptionToken token)
		{
			bool flag = !this._config.EncryptionConfig.EnableEncryption;
			if (!flag)
			{
				bool flag2 = token == null;
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Warn("Setup Encryption: no token, ignoring...");
					}
				}
				else
				{
					Simulation simulation = this.Simulation;
					bool flag3 = ((simulation != null) ? simulation._netSocket : null) == null;
					if (flag3)
					{
						DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
						if (logDebug2 != null)
						{
							logDebug2.Warn("Setup Encryption: no socket, ignoring...");
						}
					}
					else
					{
						bool flag4 = !this._cloudServices.IsEncryptionEnabled;
						if (flag4)
						{
							DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
							if (logDebug3 != null)
							{
								logDebug3.Error("Setup Encryption: Photon Cloud Encryption is disabled, make sure to use any Datagram Encryption mode, ignoring...");
							}
						}
						else
						{
							Simulation simulation2 = this.Simulation;
							if (simulation2 != null)
							{
								INetSocket netSocket = simulation2._netSocket;
								if (netSocket != null)
								{
									netSocket.SetupEncryption(token.Key, token.KeyEncrypted);
								}
							}
						}
					}
				}
			}
		}

		private void AddInactiveObjectGuard(NetworkObject obj)
		{
			bool flag = this._inactivityGuardPool.Count > 0;
			NetworkObjectInactivityGuard networkObjectInactivityGuard;
			if (flag)
			{
				networkObjectInactivityGuard = this._inactivityGuardPool.Pop();
				Assert.Check(networkObjectInactivityGuard);
				TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
				if (logTraceObject != null)
				{
					logTraceObject.Log(obj, "NetworkObjectInactivityGuard: reusing a guard from the pool");
				}
			}
			else
			{
				GameObject gameObject = new GameObject("NetworkObjectInactivityGuard");
				networkObjectInactivityGuard = gameObject.AddComponent<NetworkObjectInactivityGuard>();
				gameObject.hideFlags = (this.Config.HideNetworkObjectInactivityGuard ? HideFlags.HideAndDontSave : (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset));
				TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
				if (logTraceObject2 != null)
				{
					logTraceObject2.Log(obj, "NetworkObjectInactivityGuard: allocated a new guard");
				}
			}
			Assert.Check(networkObjectInactivityGuard.Object == null);
			networkObjectInactivityGuard.Object = obj;
			networkObjectInactivityGuard.transform.SetParent(obj.transform);
		}

		public static List<NetworkRunner>.Enumerator GetInstancesEnumerator()
		{
			return NetworkRunner._instances.GetEnumerator();
		}

		public static IReadOnlyList<NetworkRunner> Instances
		{
			get
			{
				return NetworkRunner._instances;
			}
		}

		private static bool AddInstance(NetworkRunner runner)
		{
			bool flag = !NetworkRunner._instances.Contains(runner);
			bool result;
			if (flag)
			{
				NetworkRunner._instances.Add(runner);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static bool RemoveInstance(NetworkRunner runner)
		{
			return NetworkRunner._instances.Remove(runner);
		}

		private void SimulatePhysicsScenes(float fixedDeltaTime)
		{
			PhysicsScene physicsScene = this.GetPhysicsScene();
			PhysicsScene2D physicsScene2D = this.GetPhysicsScene2D();
			bool flag = physicsScene.IsValid() && Physics.autoSimulation;
			if (flag)
			{
				physicsScene.Simulate(fixedDeltaTime);
			}
			bool flag2 = physicsScene2D.IsValid() && Physics2D.simulationMode == SimulationMode2D.FixedUpdate && physicsScene2D != Physics2D.defaultPhysicsScene;
			if (flag2)
			{
				physicsScene2D.Simulate(fixedDeltaTime);
			}
		}

		private void FixedUpdate()
		{
			bool flag = this._simulateMultiPeerPhysicsScenes && this.IsRunning && this.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
			if (flag)
			{
				this.SimulatePhysicsScenes(Time.fixedDeltaTime);
			}
		}

		public void SetSimulateMultiPeerPhysics(bool value)
		{
			this._simulateMultiPeerPhysicsScenes = value;
		}

		public unsafe bool TryGetPhysicsInfo(out NetworkPhysicsInfo info)
		{
			bool flag = this._simulation != null;
			if (flag)
			{
				NetworkPhysicsInfo* ptr;
				bool flag2 = this._simulation.TryGetStructData<NetworkPhysicsInfo>(NetworkId.PhysicsInfo, out ptr);
				if (flag2)
				{
					info = *ptr;
					return true;
				}
			}
			info = default(NetworkPhysicsInfo);
			return false;
		}

		public unsafe bool TrySetPhysicsInfo(NetworkPhysicsInfo info)
		{
			bool flag = !this.IsSceneAuthority;
			if (flag)
			{
				throw new InvalidOperationException("The runner does not have the scene authority");
			}
			bool flag2 = this._simulation != null;
			if (flag2)
			{
				NetworkPhysicsInfo* ptr;
				bool flag3 = this._simulation.TryGetStructData<NetworkPhysicsInfo>(NetworkId.PhysicsInfo, out ptr);
				if (flag3)
				{
					*ptr = info;
					return true;
				}
			}
			return false;
		}

		public bool IsSceneAuthority
		{
			get
			{
				return this.IsServer || this.IsSharedModeMasterClient;
			}
		}

		public bool IsSceneManagerBusy
		{
			get
			{
				bool flag = this._sceneLoadInitialTCS != null;
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					INetworkSceneManager sceneManager = this._sceneManager;
					bool flag2 = sceneManager != null && sceneManager.IsBusy;
					result = flag2;
				}
				return result;
			}
		}

		public bool TryGetSceneInfo(out NetworkSceneInfo sceneInfo)
		{
			return this.TryGetSceneInfo(out sceneInfo, true);
		}

		private unsafe bool TryGetSceneInfo(out NetworkSceneInfo sceneInfo, bool allowFallback)
		{
			bool flag = this._simulation != null;
			if (flag)
			{
				NetworkSceneInfo* ptr;
				bool flag2 = this._simulation.IsSceneInfoReady && this._sceneLoadInitialTCS == null && this._simulation.TryGetStructData<NetworkSceneInfo>(NetworkId.SceneInfo, out ptr);
				if (flag2)
				{
					sceneInfo = *ptr;
					return true;
				}
				if (allowFallback)
				{
					sceneInfo = this._sceneInfoSnapshot;
					return true;
				}
			}
			sceneInfo = default(NetworkSceneInfo);
			return false;
		}

		private SceneRef ValidateSceneName(string sceneName)
		{
			bool flag = string.IsNullOrEmpty(sceneName);
			if (flag)
			{
				throw new ArgumentException("Scene name is null or empty", "sceneName");
			}
			bool flag2 = this.SceneManager == null;
			if (flag2)
			{
				throw new InvalidOperationException("SceneManager not initialized");
			}
			SceneRef sceneRef = this.SceneManager.GetSceneRef(sceneName);
			bool flag3 = !sceneRef.IsValid;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("Failed to get a SceneRef for \"" + sceneName + "\"");
			}
			return sceneRef;
		}

		private SceneRef ValidateSceneRef(SceneRef sceneRef)
		{
			bool flag = !sceneRef.IsValid;
			if (flag)
			{
				throw new ArgumentException("Invalid scene reference", "sceneRef");
			}
			bool flag2 = this.SceneManager == null;
			if (flag2)
			{
				throw new InvalidOperationException("SceneManager not initialized");
			}
			return sceneRef;
		}

		private NetworkSceneAsyncOp ValidateSceneOp(NetworkSceneAsyncOp op)
		{
			bool flag = !op.IsValid;
			if (flag)
			{
				throw new ArgumentException("Invalid scene operation", "op");
			}
			return op;
		}

		public SceneRef GetSceneRef(string sceneNameOrPath)
		{
			bool flag = this.SceneManager == null;
			SceneRef result;
			if (flag)
			{
				result = default(SceneRef);
			}
			else
			{
				result = this.SceneManager.GetSceneRef(sceneNameOrPath);
			}
			return result;
		}

		public SceneRef GetSceneRef(GameObject gameObj)
		{
			bool flag = this.SceneManager == null;
			SceneRef result;
			if (flag)
			{
				result = default(SceneRef);
			}
			else
			{
				result = this.SceneManager.GetSceneRef(gameObj);
			}
			return result;
		}

		public bool MoveGameObjectToScene(GameObject gameObj, SceneRef sceneRef)
		{
			INetworkSceneManager sceneManager = this.SceneManager;
			return sceneManager != null && sceneManager.MoveGameObjectToScene(gameObj, sceneRef);
		}

		public bool MoveGameObjectToSameScene(GameObject gameObj, GameObject other)
		{
			SceneRef sceneRef = this.GetSceneRef(other);
			bool flag = !sceneRef.IsValid;
			return !flag && this.MoveGameObjectToScene(gameObj, sceneRef);
		}

		public NetworkSceneAsyncOp LoadScene(string sceneName, LoadSceneParameters parameters, bool setActiveOnLoad = false)
		{
			return this.LoadScene(this.ValidateSceneName(sceneName), parameters, setActiveOnLoad);
		}

		public NetworkSceneAsyncOp LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.None, bool setActiveOnLoad = false)
		{
			return this.LoadScene(sceneName, new LoadSceneParameters(loadSceneMode, localPhysicsMode), setActiveOnLoad);
		}

		public NetworkSceneAsyncOp LoadScene(SceneRef sceneRef, LoadSceneMode loadSceneMode = LoadSceneMode.Single, LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.None, bool setActiveOnLoad = false)
		{
			return this.LoadScene(sceneRef, new LoadSceneParameters(loadSceneMode, localPhysicsMode), setActiveOnLoad);
		}

		public NetworkSceneAsyncOp UnloadScene(string sceneName)
		{
			return this.UnloadScene(this.ValidateSceneName(sceneName));
		}

		private unsafe ref NetworkSceneInfo GetSceneInfoRef(bool allowFallback = true)
		{
			bool flag = !this.IsSceneAuthority;
			if (flag)
			{
				throw new InvalidOperationException("The runner does not have the scene authority");
			}
			bool isShutdown = this.IsShutdown;
			if (isShutdown)
			{
				throw new InvalidOperationException("The runner is shutting down. Scene info changes would never reach clients.");
			}
			Simulation simulation = this._simulation;
			bool flag2 = simulation != null && simulation.IsSceneInfoReady && this._sceneLoadInitialTCS == null;
			if (flag2)
			{
				NetworkSceneInfo* result;
				bool flag3 = this._simulation.TryGetStructData<NetworkSceneInfo>(NetworkId.SceneInfo, out result);
				if (flag3)
				{
					return ref *result;
				}
			}
			bool flag4 = !allowFallback;
			if (flag4)
			{
				throw new InvalidOperationException("Failed to get scene info");
			}
			return ref this._sceneInfoSnapshot;
		}

		public unsafe NetworkSceneAsyncOp LoadScene(SceneRef sceneRef, LoadSceneParameters parameters, bool setActiveOnLoad = false)
		{
			Assert.Check(this._simulation != null);
			sceneRef = this.ValidateSceneRef(sceneRef);
			ref NetworkSceneInfo sceneInfoRef = ref this.GetSceneInfoRef(true);
			int num = sceneInfoRef.AddSceneRef(sceneRef, parameters.loadSceneMode, parameters.localPhysicsMode, setActiveOnLoad);
			bool flag = num < 0;
			NetworkSceneAsyncOp result;
			if (flag)
			{
				result = NetworkSceneAsyncOp.FromError(sceneRef, new ArgumentException(string.Format("Failed to add {0}", sceneRef), "sceneRef"));
			}
			else
			{
				Assert.Check(*sceneInfoRef.Scenes[num] == sceneRef);
				this._sceneInfoSnapshot = sceneInfoRef;
				bool flag2 = this._sceneLoadInitialTCS == null;
				if (flag2)
				{
					TraceLogStream logTraceSceneInfo = InternalLogStreams.LogTraceSceneInfo;
					if (logTraceSceneInfo != null)
					{
						logTraceSceneInfo.Log(string.Format("Load scene {0} with {1}", sceneRef, parameters));
					}
					result = this.ValidateSceneOp(this.SceneManager.LoadScene(sceneRef, *sceneInfoRef.SceneParams[num]));
				}
				else
				{
					TraceLogStream logTraceSceneInfo2 = InternalLogStreams.LogTraceSceneInfo;
					if (logTraceSceneInfo2 != null)
					{
						logTraceSceneInfo2.Log(string.Format("Load scene {0} with {1} deferred", sceneRef, parameters));
					}
					NetworkLoadSceneParameters sceneParameters = *sceneInfoRef.SceneParams[num];
					result = this.ValidateSceneOp(NetworkSceneAsyncOp.FromDeferred(sceneRef, this._sceneLoadInitialTCS.Task, (SceneRef x) => this.SceneManager.LoadScene(x, sceneParameters)));
				}
			}
			return result;
		}

		public NetworkSceneAsyncOp UnloadScene(SceneRef sceneRef)
		{
			Assert.Check(this._simulation != null);
			sceneRef = this.ValidateSceneRef(sceneRef);
			TraceLogStream logTraceSceneInfo = InternalLogStreams.LogTraceSceneInfo;
			if (logTraceSceneInfo != null)
			{
				logTraceSceneInfo.Log(this, string.Format("Unload scene {0} called", sceneRef));
			}
			ref NetworkSceneInfo sceneInfoRef = ref this.GetSceneInfoRef(true);
			bool flag = !sceneInfoRef.RemoveSceneRef(sceneRef);
			NetworkSceneAsyncOp result;
			if (flag)
			{
				result = NetworkSceneAsyncOp.FromError(sceneRef, new ArgumentException(string.Format("Failed to remove {0}", sceneRef), "sceneRef"));
			}
			else
			{
				this._sceneInfoSnapshot = sceneInfoRef;
				bool flag2 = this._sceneLoadInitialTCS == null;
				if (flag2)
				{
					result = this.ValidateSceneOp(this.SceneManager.UnloadScene(sceneRef));
				}
				else
				{
					result = this.ValidateSceneOp(NetworkSceneAsyncOp.FromDeferred(sceneRef, this._sceneLoadInitialTCS.Task, (SceneRef x) => this.SceneManager.UnloadScene(x)));
				}
			}
			return result;
		}

		public void InvokeSceneLoadStart(SceneRef sceneRef)
		{
			try
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					this._callbacks[i].OnSceneLoadStart(this);
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			CallbackInterfaceInvoker.ISceneLoadStart(this._behaviourUpdater, sceneRef);
		}

		public void InvokeSceneLoadDone(in SceneLoadDoneArgs info)
		{
			try
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					this._callbacks[i].OnSceneLoadDone(this);
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			CallbackInterfaceInvoker.ISceneLoadDone(this._behaviourUpdater, info);
		}

		public Scene SimulationUnityScene
		{
			get
			{
				INetworkSceneManager sceneManager = this.SceneManager;
				return (sceneManager != null) ? sceneManager.MainRunnerScene : default(Scene);
			}
		}

		public static NetworkRunner GetRunnerForGameObject(GameObject gameObject)
		{
			return NetworkRunner.GetRunnerForScene(gameObject.scene);
		}

		public static NetworkRunner GetRunnerForScene(Scene scene)
		{
			foreach (NetworkRunner networkRunner in NetworkRunner.Instances)
			{
				bool flag = BehaviourUtils.IsNull(networkRunner);
				if (!flag)
				{
					INetworkSceneManager sceneManager = networkRunner.SceneManager;
					bool flag2 = sceneManager != null && sceneManager.IsRunnerScene(scene);
					if (flag2)
					{
						return networkRunner;
					}
				}
			}
			return null;
		}

		public PhysicsScene GetPhysicsScene()
		{
			bool isRunning = this.IsRunning;
			PhysicsScene result;
			if (isRunning)
			{
				PhysicsScene physicsScene;
				bool flag = this.SceneManager.TryGetPhysicsScene3D(out physicsScene);
				if (flag)
				{
					result = physicsScene;
				}
				else
				{
					result = default(PhysicsScene);
				}
			}
			else
			{
				result = Physics.defaultPhysicsScene;
			}
			return result;
		}

		public PhysicsScene2D GetPhysicsScene2D()
		{
			bool isRunning = this.IsRunning;
			PhysicsScene2D result;
			if (isRunning)
			{
				PhysicsScene2D physicsScene2D;
				bool flag = this.SceneManager.TryGetPhysicsScene2D(out physicsScene2D);
				if (flag)
				{
					result = physicsScene2D;
				}
				else
				{
					result = default(PhysicsScene2D);
				}
			}
			else
			{
				result = Physics2D.defaultPhysicsScene;
			}
			return result;
		}

		public GameObject InstantiateInRunnerScene(GameObject original, Vector3 position, Quaternion rotation)
		{
			Scene activeScene;
			bool flag = this.EnsureRunnerSceneIsActive(out activeScene);
			GameObject gameObject = Object.Instantiate<GameObject>(original, position, rotation);
			this.MoveToRunnerScene(gameObject, null);
			bool flag2 = flag;
			if (flag2)
			{
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
			}
			return gameObject;
		}

		public GameObject InstantiateInRunnerScene(GameObject original)
		{
			Scene activeScene;
			bool flag = this.EnsureRunnerSceneIsActive(out activeScene);
			GameObject gameObject = Object.Instantiate<GameObject>(original);
			this.MoveToRunnerScene(gameObject, null);
			bool flag2 = flag;
			if (flag2)
			{
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
			}
			return gameObject;
		}

		public T InstantiateInRunnerScene<T>(T original) where T : Component
		{
			Scene activeScene;
			bool flag = this.EnsureRunnerSceneIsActive(out activeScene);
			T t = Object.Instantiate<T>(original);
			this.MoveToRunnerScene(t.gameObject, null);
			bool flag2 = flag;
			if (flag2)
			{
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
			}
			return t;
		}

		public T InstantiateInRunnerScene<T>(T original, Vector3 position, Quaternion rotation) where T : Component
		{
			Scene activeScene;
			bool flag = this.EnsureRunnerSceneIsActive(out activeScene);
			T t = Object.Instantiate<T>(original, position, rotation);
			this.MoveToRunnerScene(t.gameObject, null);
			bool flag2 = flag;
			if (flag2)
			{
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(activeScene);
			}
			return t;
		}

		public bool EnsureRunnerSceneIsActive(out Scene previousActiveScene)
		{
			INetworkSceneManager sceneManager = this.SceneManager;
			Scene scene = (sceneManager != null) ? sceneManager.MainRunnerScene : default(Scene);
			bool flag = !scene.IsValid() || scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			bool result;
			if (flag)
			{
				previousActiveScene = default(Scene);
				result = false;
			}
			else
			{
				previousActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
				bool flag2 = !UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
				if (flag2)
				{
					previousActiveScene = default(Scene);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public void MoveToRunnerScene<T>(T component) where T : Component
		{
			this.MoveToRunnerScene(component.gameObject, null);
		}

		public void MoveToRunnerScene(GameObject instance, SceneRef? targetSceneRef = null)
		{
			INetworkSceneManager sceneManager = this.SceneManager;
			if (sceneManager != null)
			{
				sceneManager.MoveGameObjectToScene(instance, targetSceneRef.GetValueOrDefault());
			}
		}

		public void MakeDontDestroyOnLoad(GameObject obj)
		{
			INetworkSceneManager sceneManager = this.SceneManager;
			if (sceneManager != null)
			{
				sceneManager.MakeDontDestroyOnLoad(obj);
			}
		}

		private unsafe void ConsumeInitialSceneInfo(bool isSceneAuthority)
		{
			if (isSceneAuthority)
			{
				NetworkObjectMeta networkObjectMeta;
				bool flag = !this._simulation.TryGetStruct(NetworkId.SceneInfo, out networkObjectMeta);
				if (flag)
				{
					Assert.AlwaysFail("Failed to find scene info state");
				}
				bool isSharedModeMasterClient = this.IsSharedModeMasterClient;
				if (isSharedModeMasterClient)
				{
					Assert.Always<PlayerRef, PlayerRef>(*networkObjectMeta.StateAuthority == PlayerRef.MasterClient || *networkObjectMeta.StateAuthority == this.LocalPlayer, "Expected scene info state auth to match {0} vs {1}", *networkObjectMeta.StateAuthority, this.LocalPlayer);
				}
				*networkObjectMeta.GetDataAs<NetworkSceneInfo>() = this._sceneInfoSnapshot;
				this._sceneInfoChangeSource = NetworkSceneInfoChangeSource.Initial;
				TraceLogStream logTraceSceneInfo = InternalLogStreams.LogTraceSceneInfo;
				if (logTraceSceneInfo != null)
				{
					logTraceSceneInfo.Log(this, "Consumed initial scene info: " + this.Simulation.DumpObject(NetworkId.SceneInfo));
				}
			}
			else
			{
				this._sceneInfoSnapshot = default(NetworkSceneInfo);
				TraceLogStream logTraceSceneInfo2 = InternalLogStreams.LogTraceSceneInfo;
				if (logTraceSceneInfo2 != null)
				{
					logTraceSceneInfo2.Log(this, string.Format("This is a non-scene-authority client, so not consuming initial scene info. Setting snapshot to empty from {0}", this._sceneInfoSnapshot));
				}
			}
		}

		private unsafe void SceneInfoUpdate()
		{
			bool flag = this._sceneInfoChangeSource == NetworkSceneInfoChangeSource.None;
			if (!flag)
			{
				NetworkSceneInfoChangeSource sceneInfoChangeSource = this._sceneInfoChangeSource;
				this._sceneInfoChangeSource = NetworkSceneInfoChangeSource.None;
				TraceLogStream logTraceSceneInfo = InternalLogStreams.LogTraceSceneInfo;
				if (logTraceSceneInfo != null)
				{
					logTraceSceneInfo.Log(this, string.Format("Handling pending scene info change {0}. Data: {1}", sceneInfoChangeSource, this.Simulation.DumpObject(NetworkId.SceneInfo)));
				}
				Assert.Check(this._simulation);
				NetworkSceneInfo* ptr;
				bool flag2 = !this._simulation.TryGetStructData<NetworkSceneInfo>(NetworkId.SceneInfo, out ptr);
				if (flag2)
				{
					Assert.AlwaysFail("Expected to be able to get scene info");
				}
				bool flag3 = sceneInfoChangeSource == NetworkSceneInfoChangeSource.Initial;
				if (flag3)
				{
					Assert.Check(ptr->Equals(this._sceneInfoSnapshot));
					Assert.Check(this._sceneLoadInitialTCS != null);
					NetworkSceneInfo networkSceneInfo = default(NetworkSceneInfo);
					this.SceneInfoSyncSceneManager(sceneInfoChangeSource, ref this._sceneInfoInitial, ref networkSceneInfo);
				}
				else
				{
					NetworkSceneInfo sceneInfoSnapshot = this._sceneInfoSnapshot;
					this._sceneInfoSnapshot = *ptr;
					this.SceneInfoSyncSceneManager(sceneInfoChangeSource, ref *ptr, ref sceneInfoSnapshot);
				}
				bool flag4 = this._sceneLoadInitialTCS != null;
				if (flag4)
				{
					TraceLogStream logTraceSceneInfo2 = InternalLogStreams.LogTraceSceneInfo;
					if (logTraceSceneInfo2 != null)
					{
						logTraceSceneInfo2.Log(this, "Initial scene load, completing tcs");
					}
					this._sceneLoadInitialTCS.SetResult(0);
					this._sceneLoadInitialTCS = null;
				}
			}
		}

		private unsafe void SceneInfoSyncSceneManager(NetworkSceneInfoChangeSource changeSource, ref NetworkSceneInfo sceneInfo, ref NetworkSceneInfo prevInfo)
		{
			bool flag = this._sceneManager.OnSceneInfoChanged(sceneInfo, changeSource);
			if (flag)
			{
				TraceLogStream logTraceSceneManager = InternalLogStreams.LogTraceSceneManager;
				if (logTraceSceneManager != null)
				{
					logTraceSceneManager.Log(this, "Scene manager handled scene change event");
				}
			}
			else
			{
				bool flag2 = prevInfo.Equals(sceneInfo);
				if (flag2)
				{
					TraceLogStream logTraceSceneManager2 = InternalLogStreams.LogTraceSceneManager;
					if (logTraceSceneManager2 != null)
					{
						logTraceSceneManager2.Log(this, "Ignoring scene info change as it is the same as the last one");
					}
				}
				else
				{
					for (int i = 0; i < prevInfo.SceneCount; i++)
					{
						Assert.Check<int>(prevInfo.Scenes[i].IsValid, "Invalid previous scene at {0}", i);
					}
					for (int j = 0; j < sceneInfo.SceneCount; j++)
					{
						Assert.Check<int>(sceneInfo.Scenes[j].IsValid, "Invalid scene at {0}", j);
					}
					bool flag3 = false;
					bool flag4 = sceneInfo.SceneCount > 0 && sceneInfo.SceneParams[0].IsSingleLoad;
					if (flag4)
					{
						bool flag5 = prevInfo.SceneCount == 0 || *prevInfo.Scenes[0] != *sceneInfo.Scenes[0] || *prevInfo.SceneParams[0] != *sceneInfo.SceneParams[0];
						if (flag5)
						{
							flag3 = true;
						}
					}
					bool flag6 = !flag3;
					if (flag6)
					{
						for (int k = 0; k < prevInfo.SceneCount; k++)
						{
							bool flag7 = sceneInfo.IndexOf(*prevInfo.Scenes[k], *prevInfo.SceneParams[k]) >= 0;
							if (!flag7)
							{
								TraceLogStream logTraceSceneInfo = InternalLogStreams.LogTraceSceneInfo;
								if (logTraceSceneInfo != null)
								{
									logTraceSceneInfo.Log(string.Format("Unloading scene {0}", *prevInfo.Scenes[k]));
								}
								this.ValidateSceneOp(this.SceneManager.UnloadScene(*prevInfo.Scenes[k])).AddOnCompleted(delegate(NetworkSceneAsyncOp op)
								{
									this.OnRemoteSceneLoadCompleted(op);
								});
							}
						}
					}
					for (int l = 0; l < sceneInfo.SceneCount; l++)
					{
						bool flag8 = prevInfo.IndexOf(*sceneInfo.Scenes[l], *sceneInfo.SceneParams[l]) >= 0;
						if (!flag8)
						{
							TraceLogStream logTraceSceneInfo2 = InternalLogStreams.LogTraceSceneInfo;
							if (logTraceSceneInfo2 != null)
							{
								logTraceSceneInfo2.Log(string.Format("Loading scene {0} with {1}", *sceneInfo.Scenes[l], *sceneInfo.SceneParams[l]));
							}
							this.ValidateSceneOp(this.SceneManager.LoadScene(*sceneInfo.Scenes[l], *sceneInfo.SceneParams[l])).AddOnCompleted(delegate(NetworkSceneAsyncOp op)
							{
								this.OnRemoteSceneUnloadCompleted(op);
							});
						}
					}
				}
			}
		}

		private void OnRemoteSceneLoadCompleted(NetworkSceneAsyncOp asyncOp)
		{
			bool flag = asyncOp.Error != null;
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(this, string.Format("Failed to load scene {0} with error {1}", asyncOp.SceneRef, asyncOp.Error));
				}
			}
		}

		private void OnRemoteSceneUnloadCompleted(NetworkSceneAsyncOp asyncOp)
		{
			bool flag = asyncOp.Error != null;
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(this, string.Format("Failed to unload scene {0} with error {1}", asyncOp.SceneRef, asyncOp.Error));
				}
			}
		}

		bool Simulation.ICallbacks.CanReceivePlayerJoinLeaveCallbacks
		{
			get
			{
				return this.IsInitialized && !this.IsSceneManagerBusy;
			}
		}

		PlayerRef Simulation.ICallbacks.LocalPlayerRef
		{
			get
			{
				PlayerRef result;
				if (!this.IsSinglePlayer)
				{
					CloudServices cloudServices = this._cloudServices;
					result = ((cloudServices != null) ? cloudServices.LocalPlayerRef : default(PlayerRef));
				}
				else
				{
					result = PlayerRef.FromIndex(1);
				}
				return result;
			}
		}

		bool Simulation.ICallbacks.IsSharedModeMasterClient
		{
			get
			{
				return this.IsSharedModeMasterClient;
			}
		}

		void Simulation.ICallbacks.ObjectIsSimulatedChanged(NetworkId id, bool simulated)
		{
			NetworkObject networkObject;
			bool flag = this.TryFindObject(id, out networkObject) && networkObject.IsInSimulation != simulated;
			if (flag)
			{
				if (simulated)
				{
					networkObject.RuntimeFlags |= NetworkObjectRuntimeFlags.InSimulation;
					for (int i = 0; i < networkObject.NetworkedBehaviours.Length; i++)
					{
						try
						{
							networkObject.NetworkedBehaviours[i].Flags |= SimulationBehaviourRuntimeFlags.InSimulation;
							ISimulationEnter simulationEnter = networkObject.NetworkedBehaviours[i] as ISimulationEnter;
							bool flag2 = simulationEnter != null;
							if (flag2)
							{
								simulationEnter.SimulationEnter();
							}
						}
						catch (Exception error)
						{
							LogStream logException = InternalLogStreams.LogException;
							if (logException != null)
							{
								logException.Log(this, error);
							}
						}
					}
				}
				else
				{
					networkObject.RuntimeFlags &= ~NetworkObjectRuntimeFlags.InSimulation;
					for (int j = 0; j < networkObject.NetworkedBehaviours.Length; j++)
					{
						try
						{
							networkObject.NetworkedBehaviours[j].Flags &= ~SimulationBehaviourRuntimeFlags.InSimulation;
							ISimulationExit simulationExit = networkObject.NetworkedBehaviours[j] as ISimulationExit;
							bool flag3 = simulationExit != null;
							if (flag3)
							{
								simulationExit.SimulationExit();
							}
						}
						catch (Exception error2)
						{
							LogStream logException2 = InternalLogStreams.LogException;
							if (logException2 != null)
							{
								logException2.Log(this, error2);
							}
						}
					}
				}
			}
		}

		void Simulation.ICallbacks.ObjectInputAuthorityChanged(NetworkId id, bool gained)
		{
			NetworkObject networkObject;
			bool flag = this.TryFindObject(id, out networkObject);
			if (flag)
			{
				if (gained)
				{
					for (int i = 0; i < networkObject.NetworkedBehaviours.Length; i++)
					{
						try
						{
							IInputAuthorityGained inputAuthorityGained = networkObject.NetworkedBehaviours[i] as IInputAuthorityGained;
							bool flag2 = inputAuthorityGained != null;
							if (flag2)
							{
								inputAuthorityGained.InputAuthorityGained();
							}
						}
						catch (Exception error)
						{
							LogStream logException = InternalLogStreams.LogException;
							if (logException != null)
							{
								logException.Log(this, error);
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < networkObject.NetworkedBehaviours.Length; j++)
					{
						try
						{
							IInputAuthorityLost inputAuthorityLost = networkObject.NetworkedBehaviours[j] as IInputAuthorityLost;
							bool flag3 = inputAuthorityLost != null;
							if (flag3)
							{
								inputAuthorityLost.InputAuthorityLost();
							}
						}
						catch (Exception error2)
						{
							LogStream logException2 = InternalLogStreams.LogException;
							if (logException2 != null)
							{
								logException2.Log(this, error2);
							}
						}
					}
				}
			}
		}

		void Simulation.ICallbacks.ObjectStateAuthorityChanged(NetworkId id, bool gained)
		{
			NetworkObject networkObject;
			bool flag = this.TryFindObject(id, out networkObject);
			if (flag)
			{
				for (int i = 0; i < networkObject.NetworkedBehaviours.Length; i++)
				{
					try
					{
						IStateAuthorityChanged stateAuthorityChanged = networkObject.NetworkedBehaviours[i] as IStateAuthorityChanged;
						bool flag2 = stateAuthorityChanged != null;
						if (flag2)
						{
							stateAuthorityChanged.StateAuthorityChanged();
						}
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(this, error);
						}
					}
				}
			}
		}

		void Simulation.ICallbacks.ObjectChanged(PlayerRef player, NetworkObjectMeta obj, Simulation.ObjectChangeType change)
		{
			bool flag = obj.Id == NetworkId.SceneInfo;
			if (flag)
			{
				this._sceneInfoChangeSource = NetworkSceneInfoChangeSource.Remote;
				TraceLogStream logTraceSceneInfo = InternalLogStreams.LogTraceSceneInfo;
				if (logTraceSceneInfo != null)
				{
					logTraceSceneInfo.Log(this, "PendingSceneInfoChanges set to true for " + this.Simulation.DumpObject(NetworkId.SceneInfo));
				}
			}
		}

		void Simulation.ICallbacks.RemoteObjectCreated(NetworkObjectMeta meta)
		{
			bool flag = (meta.Flags & NetworkObjectHeaderFlags.Struct) <= (NetworkObjectHeaderFlags)0 && !meta.Id.IsReserved;
			if (flag)
			{
				bool isValid = meta.NestingRoot.IsValid;
				if (isValid)
				{
					this._remoteCreateNestedQueue.Enqueue(meta.Id);
				}
				else
				{
					this._remoteCreateQueue.Enqueue(meta.Id);
				}
			}
		}

		bool Simulation.ICallbacks.RemoteObjectDestroyed(NetworkId id)
		{
			this._remoteDestroyQueue.Enqueue(id);
			return true;
		}

		void Simulation.ICallbacks.UpdateRemotePrefabs()
		{
			bool isSceneManagerBusy = this.IsSceneManagerBusy;
			if (isSceneManagerBusy)
			{
				TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
				if (logTraceObject != null)
				{
					logTraceObject.Log(this, "Not updating remote prefabs, scene manager is busy");
				}
			}
			else
			{
				bool flag = this.IsClient && !this.IsSharedModeMasterClient;
				if (flag)
				{
					bool flag2 = !this._simulation.HasObject(NetworkId.SceneInfo);
					if (flag2)
					{
						TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
						if (logTraceObject2 != null)
						{
							logTraceObject2.Log(this, "Not updating remote prefabs because scene info is not valid");
						}
						return;
					}
				}
				Assert.Check(this._remotePrefabsWaitingForSpawnedCallback.Count == 0);
				CallbackInterfaceInvoker.IBeforeUpdateRemotePrefabs(this._behaviourUpdater);
				try
				{
					bool flag3 = this._remoteDestroyQueue.Count > 0 || this._remoteCreateQueue.Count > 0 || this._remoteCreateNestedQueue.Count > 0;
					if (flag3)
					{
						TraceLogStream logTraceObject3 = InternalLogStreams.LogTraceObject;
						if (logTraceObject3 != null)
						{
							logTraceObject3.Log(this, string.Concat(new string[]
							{
								"UpdateRemotePrefabs will do some work. Destroys: ",
								string.Join<NetworkId>(",", this._remoteDestroyQueue),
								"; Creates: ",
								string.Join<NetworkId>(",", this._remoteCreateQueue),
								"; Create nested: ",
								string.Join<NetworkId>(",", this._remoteCreateNestedQueue),
								";"
							}));
						}
					}
					int count = this._remoteCreateQueue.Count;
					while (count-- > 0)
					{
						NetworkId networkId = this._remoteCreateQueue.Dequeue();
						NetworkObjectMeta networkObjectMeta;
						bool flag4 = !this._simulation.TryGetMeta(networkId, out networkObjectMeta) || BehaviourUtils.IsNotNull(networkObjectMeta.Instance);
						if (!flag4)
						{
							NetworkObject networkObject;
							NetworkRunner.CreateInstanceResult createInstanceResult = this.TryAcquireInstance(networkObjectMeta.Type, networkObjectMeta, out networkObject, false, (networkObjectMeta.Flags & NetworkObjectHeaderFlags.DontDestroyOnLoad) == NetworkObjectHeaderFlags.DontDestroyOnLoad);
							bool flag5 = createInstanceResult == NetworkRunner.CreateInstanceResult.InProgress;
							if (flag5)
							{
								this._remoteCreateQueue.Enqueue(networkId);
							}
							else
							{
								bool flag6 = createInstanceResult == NetworkRunner.CreateInstanceResult.Ignore;
								if (flag6)
								{
									TraceLogStream logTraceObject4 = InternalLogStreams.LogTraceObject;
									if (logTraceObject4 != null)
									{
										logTraceObject4.Log(string.Format("Ignoring {0}", networkObjectMeta.Id));
									}
									networkObjectMeta.LocalFlags |= NetworkObjectMetaFlags.InstanceWillNotBeCreated;
								}
								else
								{
									bool flag7 = createInstanceResult == NetworkRunner.CreateInstanceResult.Failed;
									if (flag7)
									{
										LogStream logError = InternalLogStreams.LogError;
										if (logError != null)
										{
											logError.Log(string.Format("Failed to create instance for {0}, not going to retry", networkObjectMeta.Id));
										}
										networkObjectMeta.LocalFlags |= NetworkObjectMetaFlags.InstanceWillNotBeCreated;
									}
									else
									{
										Assert.Check(createInstanceResult == NetworkRunner.CreateInstanceResult.Success);
										Assert.Check(BehaviourUtils.IsNotNull(networkObject));
										bool flag8 = networkObject.Id.IsValid && networkObject.Id != networkObjectMeta.Id;
										if (flag8)
										{
											LogStream logWarn = InternalLogStreams.LogWarn;
											if (logWarn != null)
											{
												logWarn.Log(networkObject, string.Format("Object (type: {0} is already attached to a different id: {1}", networkObjectMeta.Type, networkObjectMeta.Id));
											}
											this._remoteCreateQueue.Enqueue(networkId);
										}
										else
										{
											foreach (NetworkObject networkObject2 in networkObject.NestedObjects)
											{
												bool flag9 = !networkObject2.IsValid && networkObject2.gameObject.activeSelf;
												if (flag9)
												{
													TraceLogStream logTraceObject5 = InternalLogStreams.LogTraceObject;
													if (logTraceObject5 != null)
													{
														logTraceObject5.Log(networkObject2, string.Format("Deactivating unattached nested object of {0} ({1})", networkObjectMeta.Id, networkObject.name));
													}
													networkObject2.gameObject.SetActive(false);
												}
											}
											this.<Fusion.Simulation.ICallbacks.UpdateRemotePrefabs>g__InstanceAcquired|337_0(networkObjectMeta, networkObject);
										}
									}
								}
							}
						}
					}
					int count2 = this._remoteCreateNestedQueue.Count;
					while (count2-- > 0)
					{
						NetworkId networkId2 = this._remoteCreateNestedQueue.Dequeue();
						NetworkObjectMeta networkObjectMeta2;
						bool flag10 = !this._simulation.TryGetMeta(networkId2, out networkObjectMeta2) || BehaviourUtils.IsNotNull(networkObjectMeta2.Instance);
						if (!flag10)
						{
							NetworkObjectMeta networkObjectMeta3;
							bool flag11 = !this._simulation.TryGetMeta(networkObjectMeta2.NestingRoot, out networkObjectMeta3);
							if (flag11)
							{
								this._remoteCreateNestedQueue.Enqueue(networkId2);
							}
							else
							{
								NetworkObject instance = networkObjectMeta3.Instance;
								bool flag12 = BehaviourUtils.IsNull(instance);
								if (flag12)
								{
									this._remoteCreateNestedQueue.Enqueue(networkId2);
								}
								else
								{
									int num = networkObjectMeta2.NestingKey.Value - 1;
									bool flag13 = num < 0 || num >= instance.NestedObjects.Length;
									if (flag13)
									{
										LogStream logError2 = InternalLogStreams.LogError;
										if (logError2 != null)
										{
											logError2.Log(this, string.Format("Nesting key out of bounds: {0} {1}, won't try to create nested object", networkObjectMeta2.NestingKey, instance.NestedObjects.Length));
										}
									}
									else
									{
										NetworkObject networkObject3 = instance.NestedObjects[num];
										bool flag14 = BehaviourUtils.IsNull(networkObject3);
										if (flag14)
										{
											LogStream logError3 = InternalLogStreams.LogError;
											if (logError3 != null)
											{
												logError3.Log(this, string.Format("Nesting key {0} is valid for {1}, but the instance is null - won't try to create nested object", networkObjectMeta2.NestingKey, networkObjectMeta2.Id));
											}
										}
										else
										{
											bool flag15 = networkObject3.Id.IsValid && networkObject3.Id != networkObjectMeta2.Id;
											if (flag15)
											{
												LogStream logWarn2 = InternalLogStreams.LogWarn;
												if (logWarn2 != null)
												{
													logWarn2.Log(networkObject3, string.Format("Object (type: {0} is already attached to a different id: {1}", networkObjectMeta2.Type, networkObjectMeta2.Id));
												}
												this._remoteCreateNestedQueue.Enqueue(networkId2);
											}
											else
											{
												networkObject3.RuntimeFlags |= NetworkObjectRuntimeFlags.IsNested;
												this.<Fusion.Simulation.ICallbacks.UpdateRemotePrefabs>g__InstanceAcquired|337_0(networkObjectMeta2, networkObject3);
											}
										}
									}
								}
							}
						}
					}
					while (this._remoteDestroyQueue.Count > 0)
					{
						NetworkId networkId3 = this._remoteDestroyQueue.Dequeue();
						NetworkObjectMeta networkObjectMeta4;
						bool flag16 = !this._simulation.TryGetMeta(networkId3, out networkObjectMeta4);
						if (flag16)
						{
							TraceLogStream logTraceObject6 = InternalLogStreams.LogTraceObject;
							if (logTraceObject6 != null)
							{
								logTraceObject6.Log(string.Format("DeleteRemotePrefab for {0}, but it doesn't exist", networkId3));
							}
						}
						else
						{
							bool flag17 = BehaviourUtils.IsAlive(networkObjectMeta4.Instance);
							if (flag17)
							{
								TraceLogStream logTraceObject7 = InternalLogStreams.LogTraceObject;
								if (logTraceObject7 != null)
								{
									logTraceObject7.Log(this, string.Format("Destroy remote prefab: {0} tick: {1} {2}", networkObjectMeta4.Id, this.Simulation.Tick, this.Simulation.Stage));
								}
								Assert.Check<BehaviourUtils.NameDeferred, NetworkId, NetworkObjectMeta>(networkObjectMeta4.Id == networkObjectMeta4.Instance.Id, "Object seem to have been attached to a different id already {0} {1} {2}", BehaviourUtils.GetName(networkObjectMeta4.Instance), networkObjectMeta4.Instance.Id, networkObjectMeta4);
								this.Destroy(networkObjectMeta4.Instance, NetworkObjectDestroyFlags.DestroyedByReplicator);
							}
							this._simulation.Destroy(networkId3, NetworkObjectDestroyFlags.DestroyState, default(PlayerRef));
						}
					}
				}
				finally
				{
					try
					{
						for (int j = 0; j < this._remotePrefabsWaitingForSpawnedCallback.Count; j++)
						{
							NetworkObject networkObject4 = this._remotePrefabsWaitingForSpawnedCallback[j];
							Assert.Check(BehaviourUtils.IsAlive(networkObject4), "Remote prefab destroyed before having a chance to invoke Spawned");
							bool flag18 = !networkObject4.Id.IsValid;
							if (flag18)
							{
								LogStream logWarn3 = InternalLogStreams.LogWarn;
								if (logWarn3 != null)
								{
									logWarn3.Log(networkObject4, "This object has been spawned and despawned in the same tick");
								}
								this._remotePrefabsWaitingForSpawnedCallback[j] = null;
							}
						}
						foreach (NetworkObject networkObject5 in this._remotePrefabsWaitingForSpawnedCallback)
						{
							bool flag19 = BehaviourUtils.IsNull(networkObject5);
							if (!flag19)
							{
								bool flag20 = NetworkRunner.IsAwakeAtInitialization(networkObject5);
								if (flag20)
								{
									this.InvokeBeforeSpawnedCallbacks(networkObject5, (NetworkRunner.AttachOptions)0, null);
								}
							}
						}
						foreach (NetworkObject networkObject6 in this._remotePrefabsWaitingForSpawnedCallback)
						{
							bool flag21 = BehaviourUtils.IsNull(networkObject6);
							if (!flag21)
							{
								Assert.Check(BehaviourUtils.IsAlive(networkObject6), "Remote prefab destroyed before having a chance to invoke Spawned");
								bool flag22 = NetworkRunner.IsAwakeAtInitialization(networkObject6);
								if (flag22)
								{
									this.InvokeSpawnedCallback(networkObject6);
								}
							}
						}
						foreach (NetworkObject networkObject7 in this._remotePrefabsWaitingForSpawnedCallback)
						{
							bool flag23 = BehaviourUtils.IsNull(networkObject7);
							if (!flag23)
							{
								bool flag24 = NetworkRunner.IsAwakeAtInitialization(networkObject7);
								if (flag24)
								{
									this.InvokeAfterSpawnedCallback(networkObject7);
								}
							}
						}
					}
					finally
					{
						this._remotePrefabsWaitingForSpawnedCallback.Clear();
					}
					CallbackInterfaceInvoker.IAfterUpdateRemotePrefabs(this._behaviourUpdater);
				}
			}
		}

		private void ProcessSpawnQueue()
		{
			bool isSceneManagerBusy = this.IsSceneManagerBusy;
			if (!isSceneManagerBusy)
			{
				bool flag = this.Topology == Topologies.Shared && !this.LocalPlayer.IsRealPlayer;
				if (!flag)
				{
					int count = this._spawnQueue.Count;
					while (count-- > 0)
					{
						NetworkRunner.SpawnArgs spawnArgs = this._spawnQueue.Dequeue();
						NetworkSpawnOp networkSpawnOp = this.SpawnInternal(spawnArgs);
						bool isSpawned = networkSpawnOp.IsSpawned;
						if (isSpawned)
						{
							TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
							if (logTraceObject != null)
							{
								logTraceObject.Log(networkSpawnOp.Object, string.Format("Queued spawn completed for {0}", spawnArgs));
							}
						}
						else
						{
							bool isQueued = networkSpawnOp.IsQueued;
							if (isQueued)
							{
								TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
								if (logTraceObject2 != null)
								{
									logTraceObject2.Log(this, string.Format("Queued spawn for {0} has been requeued", spawnArgs));
								}
							}
							else
							{
								bool flag2 = spawnArgs.Spawned == null;
								if (flag2)
								{
									LogStream logError = InternalLogStreams.LogError;
									if (logError != null)
									{
										logError.Log(this, string.Format("Queued spawn failed for {0}: {1}", spawnArgs, networkSpawnOp.Status));
									}
								}
								else
								{
									TraceLogStream logTraceObject3 = InternalLogStreams.LogTraceObject;
									if (logTraceObject3 != null)
									{
										logTraceObject3.Log(this, string.Format("Queued spawn failed for {0}: {1}", spawnArgs, networkSpawnOp.Status));
									}
								}
							}
						}
					}
				}
			}
		}

		void Simulation.ICallbacks.OnBeforeCopyPreviousState()
		{
			CallbackInterfaceInvoker.IBeforeCopyPreviousState(this._behaviourUpdater);
		}

		void Simulation.ICallbacks.OnTick()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			try
			{
				Time.fixedDeltaTime = this._simulation.DeltaTime;
				this._behaviourUpdater.InvokeFixedUpdateNetwork(this._simulation.Stage, this._simulation.Mode, this._simulation.Topology);
			}
			finally
			{
				Time.fixedDeltaTime = fixedDeltaTime;
			}
		}

		void Simulation.ICallbacks.OnServerStart()
		{
			this.ConsumeInitialSceneInfo(true);
		}

		void Simulation.ICallbacks.OnClientStart()
		{
			this.ConsumeInitialSceneInfo(this.IsSharedModeMasterClient);
		}

		void Simulation.ICallbacks.OnInputMissing(SimulationInput input)
		{
			for (int i = 0; i < this._callbacks.Count; i++)
			{
				try
				{
					this._callbacks[i].OnInputMissing(this, input.Player, NetworkInput.FromRaw(input.Data, this.Simulation.Config.InputDataWordCount));
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(this, error);
					}
				}
			}
		}

		void Simulation.ICallbacks.OnInput(SimulationInput input)
		{
			bool flag = !this.ProvideInput;
			if (!flag)
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					try
					{
						this._callbacks[i].OnInput(this, NetworkInput.FromRaw(input.Data, this.Simulation.Config.InputDataWordCount));
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(this, error);
						}
					}
				}
			}
		}

		private unsafe void OnMessageUser(SimulationMessage* message)
		{
			SimulationMessagePtr message2;
			message2.Message = message;
			try
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					this._callbacks[i].OnUserSimulationMessage(this, message2);
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
		}

		unsafe SimulationMessageResult Simulation.ICallbacks.OnMessage(SimulationMessage* message)
		{
			try
			{
				bool flag = message->GetFlag(1);
				if (flag)
				{
					this.OnMessageUser(message);
					return SimulationMessageResult.Handled;
				}
				bool flag2 = message->GetFlag(256);
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Warn(this, "Dummy message received; likely the sender tried to send " + string.Format("a message that was too large to be serialized. {0}", LogUtils.GetDump<SimulationMessage>(message)));
					}
					return SimulationMessageResult.Handled;
				}
				Span<byte> rawData = SimulationMessage.GetRawData(message);
				RpcHeader rpcHeader = rawData.Read<RpcHeader>();
				bool flag3 = message->IsTargeted();
				TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
				if (logTraceSimulationMessage != null)
				{
					logTraceSimulationMessage.Log(this, string.Format("OnMessage {0}", LogUtils.GetDump<SimulationMessage>(message)));
				}
				PlayerRef playerRef = PlayerRef.None;
				bool flag4 = false;
				bool flag5 = flag3;
				if (flag5)
				{
					playerRef = message->Target;
					flag4 = (playerRef == this.LocalPlayer || (playerRef.IsNone && this.Simulation.IsServer));
				}
				bool flag6 = message->GetFlag(4);
				if (flag6)
				{
					bool flag7 = flag4 || !flag3;
					if (flag7)
					{
						RpcStaticInvokeDelegate rpcStaticInvokeDelegate;
						bool flag8 = NetworkBehaviourUtils.TryGetRpcStaticInvokeDelegate((int)rpcHeader.Method, out rpcStaticInvokeDelegate);
						if (flag8)
						{
							rpcStaticInvokeDelegate(this, message);
						}
						else
						{
							LogStream logError = InternalLogStreams.LogError;
							if (logError != null)
							{
								logError.Log(this, string.Format("Could not find static RPC invoke delegate for index: {0}.", rpcHeader.Method));
							}
						}
					}
					bool flag9 = !flag4 && this.IsServer;
					if (flag9)
					{
						bool flag10 = flag3;
						if (flag10)
						{
							bool flag11 = playerRef == message->Source;
							if (flag11)
							{
								DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
								if (logDebug2 != null)
								{
									logDebug2.Error(this, string.Format("Target player {0} same as the source, not forwarding (static). {1}", playerRef, LogUtils.GetDump<SimulationMessage>(message)));
								}
							}
							else
							{
								this.Simulation.ForwardMessage(message, playerRef, true);
							}
						}
						else
						{
							foreach (SimulationConnection c in this.Simulation.Connections)
							{
								PlayerRef playerRef2 = this.Simulation.Connection2Player(c);
								bool flag12 = playerRef2 != message->Source;
								if (flag12)
								{
									this.Simulation.ForwardMessage(message, playerRef2, false);
								}
							}
						}
					}
				}
				else
				{
					NetworkObjectMeta networkObjectMeta;
					bool flag13 = !this.Simulation.TryGetMeta(rpcHeader.Object, out networkObjectMeta);
					if (flag13)
					{
						DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
						if (logDebug3 != null)
						{
							logDebug3.Warn(this, string.Format("Simulation message target object not found: {0} {1}", rpcHeader.Object, LogUtils.GetDump<SimulationMessage>(message)));
						}
						return SimulationMessageResult.Ignored;
					}
					NetworkObject instance = networkObjectMeta.Instance;
					bool flag14 = BehaviourUtils.IsNull(instance);
					if (flag14)
					{
						bool flag15 = (networkObjectMeta.LocalFlags & NetworkObjectMetaFlags.InstanceWillNotBeCreated) == NetworkObjectMetaFlags.None;
						if (flag15)
						{
							return SimulationMessageResult.Retry;
						}
						return SimulationMessageResult.Ignored;
					}
					else
					{
						NetworkBehaviour networkBehaviour = instance.NetworkedBehaviours[(int)rpcHeader.Behaviour];
						bool flag16 = BehaviourUtils.IsNotAlive(networkBehaviour);
						if (flag16)
						{
							DebugLogStream logDebug4 = InternalLogStreams.LogDebug;
							if (logDebug4 != null)
							{
								logDebug4.Warn(this, string.Format("Behaviour {0} not found on {1} {2}", rpcHeader.Behaviour, rpcHeader.Object, LogUtils.GetDump<SimulationMessage>(message)));
							}
							return SimulationMessageResult.Ignored;
						}
						bool flag17 = networkBehaviour.RpcCache == null;
						if (flag17)
						{
							bool flag18 = !NetworkBehaviourUtils.TryGetRpcInvokeDelegateArray(networkBehaviour.GetType(), out networkBehaviour.RpcCache);
							if (flag18)
							{
								LogStream logError2 = InternalLogStreams.LogError;
								if (logError2 != null)
								{
									logError2.Log(this, string.Format("Could not find RPC invoke array for {0} on {1}.", networkBehaviour.GetType(), instance.Name));
								}
								return SimulationMessageResult.Ignored;
							}
						}
						Assert.Check<ushort, int, ushort, BehaviourUtils.NameDeferred>((int)rpcHeader.Method < networkBehaviour.RpcCache.Length, rpcHeader.Method, networkBehaviour.RpcCache.Length, rpcHeader.Behaviour, BehaviourUtils.GetName(networkBehaviour));
						RpcInvokeData rpcInvokeData = networkBehaviour.RpcCache[(int)rpcHeader.Method];
						int num = AuthorityMasks.Create(this.Simulation.IsStateAuthority(networkObjectMeta, message->Source), this.Simulation.IsInputAuthority(networkObjectMeta, message->Source));
						bool flag19 = (rpcInvokeData.Sources & num) == 0;
						if (flag19)
						{
							DebugLogStream logDebug5 = InternalLogStreams.LogDebug;
							if (logDebug5 != null)
							{
								logDebug5.Error(this, string.Format("{0} sent rpc {1} to {2} but is not allowed.", message->Source, rpcInvokeData.Delegate.Method, instance.Name));
							}
							return SimulationMessageResult.Ignored;
						}
						int localAuthorityMask = this._simulation.GetLocalAuthorityMask(networkObjectMeta.Header);
						bool flag20 = (rpcInvokeData.Targets & localAuthorityMask) != 0;
						if (flag20)
						{
							bool flag21 = flag4 || !flag3;
							if (flag21)
							{
								Assert.Check(!networkBehaviour.InvokeRpc);
								rpcInvokeData.Delegate(networkBehaviour, message);
								Assert.Check(!networkBehaviour.InvokeRpc);
							}
						}
						else
						{
							bool flag22 = flag4;
							if (flag22)
							{
								DebugLogStream logDebug6 = InternalLogStreams.LogDebug;
								if (logDebug6 != null)
								{
									logDebug6.Error(this, string.Format("Not invoked locally because masks don't match: {0} vs {1} {2}", rpcInvokeData.Targets, localAuthorityMask, LogUtils.GetDump<SimulationMessage>(message)));
								}
							}
						}
						int num2 = rpcInvokeData.Targets & ~localAuthorityMask;
						bool flag23 = (rpcInvokeData.Targets & 4) == 4;
						if (flag23)
						{
							num2 |= 4;
						}
						Assert.Check((num2 & 1) == 0 || this.Simulation.IsClient);
						bool flag24 = !flag4 && this.IsServer && num2 != 0;
						if (flag24)
						{
							bool flag25 = flag3;
							if (flag25)
							{
								bool flag26 = ((num2 & 2) != 0 && this.Simulation.IsInputAuthority(networkObjectMeta, playerRef)) || ((num2 & 4) != 0 && !this.Simulation.IsInputAuthority(networkObjectMeta, playerRef));
								if (flag26)
								{
									bool flag27 = playerRef == message->Source;
									if (flag27)
									{
										DebugLogStream logDebug7 = InternalLogStreams.LogDebug;
										if (logDebug7 != null)
										{
											logDebug7.Error(this, string.Format("Target player {0} same as the source, not forwarding ({1} {2}). {3}", new object[]
											{
												playerRef,
												*networkObjectMeta.InputAuthority,
												num2,
												LogUtils.GetDump<SimulationMessage>(message)
											}));
										}
									}
									else
									{
										this.Simulation.ForwardMessage(message, playerRef, true);
									}
								}
								else
								{
									DebugLogStream logDebug8 = InternalLogStreams.LogDebug;
									if (logDebug8 != null)
									{
										logDebug8.Error(this, string.Format("Can't be forwarded to {0} - excluded with authority masks {1}", playerRef, LogUtils.GetDump<SimulationMessage>(message)));
									}
								}
							}
							else
							{
								bool flag28 = (num2 & 2) != 0 && *networkObjectMeta.InputAuthority != default(PlayerRef);
								if (flag28)
								{
									Assert.Check(!this.Simulation.IsInputAuthority(networkObjectMeta, this._simulation.LocalPlayer));
									bool flag29 = this.Simulation.IsInputAuthority(networkObjectMeta, message->Source);
									if (flag29)
									{
										bool flag30 = rpcInvokeData.Targets == 2;
										if (flag30)
										{
											DebugLogStream logDebug9 = InternalLogStreams.LogDebug;
											if (logDebug9 != null)
											{
												logDebug9.Error(this, string.Format("InputAuthority is same as the sender {0}, not forwarding. {1}", *networkObjectMeta.InputAuthority, LogUtils.GetDump<SimulationMessage>(message)));
											}
										}
									}
									else
									{
										this.Simulation.ForwardMessage(message, *networkObjectMeta.InputAuthority, true);
									}
								}
								bool flag31 = (num2 & 4) != 0;
								if (flag31)
								{
									foreach (SimulationConnection c2 in this.Simulation.Connections)
									{
										PlayerRef playerRef3 = this.Simulation.Connection2Player(c2);
										bool flag32 = playerRef3 != *networkObjectMeta.InputAuthority && playerRef3 != message->Source;
										if (flag32)
										{
											this.Simulation.ForwardMessage(message, playerRef3, false);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			return SimulationMessageResult.Handled;
		}

		void Simulation.ICallbacks.OnBeforeSimulation(int forwardTickCount)
		{
			CallbackInterfaceInvoker.IBeforeSimulation(this._behaviourUpdater, forwardTickCount);
		}

		void Simulation.ICallbacks.OnAfterSimulation()
		{
		}

		void Simulation.ICallbacks.OnBeforeClientSidePredictionReset()
		{
			CallbackInterfaceInvoker.IBeforeClientPredictionReset(this._behaviourUpdater);
		}

		void Simulation.ICallbacks.OnAfterClientSidePredictionReset()
		{
			CallbackInterfaceInvoker.IAfterClientPredictionReset(this._behaviourUpdater);
		}

		void Simulation.ICallbacks.OnBeforeAllTicks(bool resimulation, int tickCount)
		{
			CallbackInterfaceInvoker.IBeforeAllTicks(this._behaviourUpdater, resimulation, tickCount);
		}

		void Simulation.ICallbacks.OnAfterAllTicks(bool resimulation, int tickCount)
		{
			CallbackInterfaceInvoker.IAfterAllTicks(this._behaviourUpdater, resimulation, tickCount);
		}

		void Simulation.ICallbacks.OnBeforeTick()
		{
			this.SceneInfoUpdate();
			CallbackInterfaceInvoker.IBeforeTick(this._behaviourUpdater);
		}

		void Simulation.ICallbacks.OnAfterTick()
		{
			CallbackInterfaceInvoker.IAfterTick(this._behaviourUpdater);
		}

		void Simulation.ICallbacks.ObjectEnterAOI(PlayerRef player, NetworkId id)
		{
			NetworkObject networkObject;
			bool flag = this.TryFindObject(id, out networkObject);
			if (flag)
			{
				for (int i = 0; i < networkObject.NetworkedBehaviours.Length; i++)
				{
					IInterestEnter interestEnter = networkObject.NetworkedBehaviours[i] as IInterestEnter;
					bool flag2 = interestEnter != null;
					if (flag2)
					{
						try
						{
							interestEnter.InterestEnter(player);
						}
						catch (Exception error)
						{
							LogStream logException = InternalLogStreams.LogException;
							if (logException != null)
							{
								logException.Log(error);
							}
						}
					}
				}
				for (int j = 0; j < this._callbacks.Count; j++)
				{
					try
					{
						this._callbacks[j].OnObjectEnterAOI(this, networkObject, player);
					}
					catch (Exception error2)
					{
						LogStream logException2 = InternalLogStreams.LogException;
						if (logException2 != null)
						{
							logException2.Log(this, error2);
						}
					}
				}
			}
		}

		void Simulation.ICallbacks.ObjectExitAOI(PlayerRef player, NetworkId id)
		{
			NetworkObject networkObject;
			bool flag = this.TryFindObject(id, out networkObject);
			if (flag)
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					try
					{
						this._callbacks[i].OnObjectExitAOI(this, networkObject, player);
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(this, error);
						}
					}
				}
				for (int j = 0; j < networkObject.NetworkedBehaviours.Length; j++)
				{
					IInterestExit interestExit = networkObject.NetworkedBehaviours[j] as IInterestExit;
					bool flag2 = interestExit != null;
					if (flag2)
					{
						try
						{
							interestExit.InterestExit(player);
						}
						catch (Exception error2)
						{
							LogStream logException2 = InternalLogStreams.LogException;
							if (logException2 != null)
							{
								logException2.Log(error2);
							}
						}
					}
				}
			}
		}

		void Simulation.ICallbacks.OnConnectedToServer()
		{
			for (int i = 0; i < this._callbacks.Count; i++)
			{
				try
				{
					this._callbacks[i].OnConnectedToServer(this);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(this, error);
					}
				}
			}
		}

		void Simulation.ICallbacks.OnDisconnectedFromServer(NetDisconnectReason reason)
		{
			for (int i = 0; i < this._callbacks.Count; i++)
			{
				try
				{
					this._callbacks[i].OnDisconnectedFromServer(this, reason);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(this, error);
					}
				}
			}
		}

		void Simulation.ICallbacks.OnConnectionFailed(NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			ShutdownReason shutdownReason = ShutdownReason.Error;
			string customMsg = string.Empty;
			switch (reason)
			{
			case NetConnectFailedReason.Timeout:
				shutdownReason = ShutdownReason.ConnectionTimeout;
				customMsg = "Connection Timeout";
				break;
			case NetConnectFailedReason.ServerFull:
				shutdownReason = ShutdownReason.GameIsFull;
				customMsg = "Game Is Full";
				break;
			case NetConnectFailedReason.ServerRefused:
				shutdownReason = ShutdownReason.ConnectionRefused;
				customMsg = "Connection Refused";
				break;
			}
			AsyncOperationHandler<ShutdownReason> startGameOperation = this._startGameOperation;
			if (startGameOperation != null)
			{
				startGameOperation.SetException(new StartGameException(shutdownReason, customMsg));
			}
			for (int i = 0; i < this._callbacks.Count; i++)
			{
				try
				{
					this._callbacks[i].OnConnectFailed(this, remoteAddress, reason);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(this, error);
					}
				}
			}
		}

		void Simulation.ICallbacks.OnReliableData(PlayerRef player, ReliableId id, bool local, byte[] dataArray)
		{
			if (local)
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					try
					{
						this._callbacks[i].OnReliableDataReceived(this, player, id.Key, new ArraySegment<byte>(dataArray));
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(this, error);
						}
					}
				}
			}
			else
			{
				List<byte[]> list;
				bool flag = !this._reliableTransfers.TryGetValue(id.SourceCombined, out list);
				if (flag)
				{
					this._reliableTransfers.Add(id.SourceCombined, list = new List<byte[]>());
				}
				list.Add(dataArray);
				int num = (from x in list
				select x.Length).Sum();
				bool flag2 = num == id.TotalLength;
				if (flag2)
				{
					this._reliableTransfers.Remove(id.SourceCombined);
					MemoryStream memoryStream = new MemoryStream(new byte[id.TotalLength], true);
					foreach (byte[] array in list)
					{
						memoryStream.Write(array, 0, array.Length);
					}
					byte[] array2 = memoryStream.ToArray();
					for (int j = 0; j < this._callbacks.Count; j++)
					{
						try
						{
							this._callbacks[j].OnReliableDataReceived(this, player, id.Key, new ArraySegment<byte>(array2));
						}
						catch (Exception error2)
						{
							LogStream logException2 = InternalLogStreams.LogException;
							if (logException2 != null)
							{
								logException2.Log(this, error2);
							}
						}
					}
				}
				else
				{
					for (int k = 0; k < this._callbacks.Count; k++)
					{
						try
						{
							this._callbacks[k].OnReliableDataProgress(this, player, id.Key, (float)num / (float)id.TotalLength);
						}
						catch (Exception error3)
						{
							LogStream logException3 = InternalLogStreams.LogException;
							if (logException3 != null)
							{
								logException3.Log(this, error3);
							}
						}
					}
				}
			}
		}

		void Simulation.ICallbacks.PlayerJoined(PlayerRef player)
		{
			Assert.Check(!this.IsSceneManagerBusy);
			for (int i = 0; i < this._callbacks.Count; i++)
			{
				try
				{
					this._callbacks[i].OnPlayerJoined(this, player);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(this, error);
					}
				}
			}
			CallbackInterfaceInvoker.IPlayerJoined(this._behaviourUpdater, player);
		}

		void Simulation.ICallbacks.PlayerLeft(PlayerRef player)
		{
			Assert.Check(!this.IsSceneManagerBusy);
			CallbackInterfaceInvoker.IPlayerLeft(this._behaviourUpdater, player);
			for (int i = 0; i < this._callbacks.Count; i++)
			{
				try
				{
					this._callbacks[i].OnPlayerLeft(this, player);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(this, error);
					}
				}
			}
		}

		OnConnectionRequestReply Simulation.ICallbacks.OnConnectionRequest(NetAddress remoteAddress, byte[] token)
		{
			bool flag = this._callbacks.Count > 0;
			if (flag)
			{
				NetworkRunnerCallbackArgs.ConnectRequest connectRequest = new NetworkRunnerCallbackArgs.ConnectRequest();
				connectRequest.RemoteAddress = remoteAddress;
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					try
					{
						this._callbacks[i].OnConnectRequest(this, connectRequest, token);
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(this, error);
						}
					}
				}
				bool flag2 = connectRequest.Result != null;
				if (flag2)
				{
					return connectRequest.Result.Value;
				}
			}
			return OnConnectionRequestReply.Ok;
		}

		void Simulation.ICallbacks.OnInternalConnectionAttempt(int attempt, int totalConnectionAttempts, out bool shouldChange, out NetAddress newAddress)
		{
			shouldChange = false;
			newAddress = default(NetAddress);
			bool isCloudReady = this.IsCloudReady;
			if (isCloudReady)
			{
				this._cloudServices.OnInternalConnectionAttempt(attempt, totalConnectionAttempts, out shouldChange, out newAddress);
			}
		}

		public bool CanSpawn
		{
			get
			{
				return this.IsServer || this.Topology == Topologies.Shared;
			}
		}

		private NetworkSpawnOp SpawnInternal(in NetworkRunner.SpawnArgs args)
		{
			NetworkRunner.<>c__DisplayClass370_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.spawnedCallback = args.Spawned;
			bool flag = this.IsClient && this.Topology == Topologies.ClientServer;
			NetworkSpawnOp result;
			if (flag)
			{
				result = this.<SpawnInternal>g__Failed|370_1(NetworkSpawnStatus.FailedClientCantSpawn, ref CS$<>8__locals1);
			}
			else
			{
				Assert.Check(args.TypeId.IsValid);
				bool flag2 = this.Topology == Topologies.Shared && !this.LocalPlayer.IsRealPlayer;
				if (flag2)
				{
					bool synchronous = args.Synchronous;
					if (synchronous)
					{
						result = this.<SpawnInternal>g__Failed|370_1(NetworkSpawnStatus.FailedLocalPlayerNotYetSet, ref CS$<>8__locals1);
					}
					else
					{
						result = this.<SpawnInternal>g__Incomplete|370_3(args, ref CS$<>8__locals1);
					}
				}
				else
				{
					NetworkObject networkObject;
					NetworkRunner.CreateInstanceResult createInstanceResult = this.TryAcquireInstance(args.TypeId, null, out networkObject, args.Synchronous, args.DontDestroyOnLoad);
					bool flag3 = createInstanceResult == NetworkRunner.CreateInstanceResult.InProgress;
					if (flag3)
					{
						bool synchronous2 = args.Synchronous;
						if (synchronous2)
						{
							bool flag4 = !this._config.EnqueueIncompleteSynchronousSpawns;
							if (flag4)
							{
								return this.<SpawnInternal>g__Failed|370_1(NetworkSpawnStatus.FailedToLoadPrefabSynchronously, ref CS$<>8__locals1);
							}
						}
						result = this.<SpawnInternal>g__Incomplete|370_3(args, ref CS$<>8__locals1);
					}
					else
					{
						bool flag5 = createInstanceResult == NetworkRunner.CreateInstanceResult.Success;
						if (flag5)
						{
							NetworkRunner.ApplySpawnArgs(networkObject, args);
							networkObject.IsResume = this.IsResume;
							NetworkId networkId = this.IsResume ? this.<SpawnInternal>g__CheckIdOrGetNewId|370_0(args.ResumeNO, ref CS$<>8__locals1) : this.Simulation.GetNextId();
							Simulation simulation = this.Simulation;
							NetworkId id = networkId;
							int wordCount = NetworkObject.GetWordCount(networkObject);
							NetworkObjectTypeId typeId = args.TypeId;
							int behaviourCount = networkObject.NetworkedBehaviours.Length;
							NetworkObjectHeaderFlags flags = this.FlagsFromInstance(networkObject) | (args.DontDestroyOnLoad ? NetworkObjectHeaderFlags.DontDestroyOnLoad : ((NetworkObjectHeaderFlags)0)) | (this.IsClient ? NetworkObjectHeaderFlags.SpawnedByClient : ((NetworkObjectHeaderFlags)0));
							NetworkId nestingRoot = default(NetworkId);
							NetworkObjectNestingKey nestingKey = default(NetworkObjectNestingKey);
							NetworkObjectMeta networkObjectMeta = simulation.AllocateObject(id, wordCount, typeId, behaviourCount, nestingRoot, nestingKey, flags);
							NetworkRunner.AttachOptions options = NetworkRunner.AttachOptions.LocalSpawn;
							this.InitializeNetworkObjectInstance(networkObjectMeta, networkObject, args.InputAuthority, options, args.MasterClientOverride);
							networkObject.RuntimeFlags |= NetworkObjectRuntimeFlags.OwnsNestedObjects;
							for (int i = 0; i < networkObject.NestedObjects.Length; i++)
							{
								Assert.Check(args.TypeId.IsPrefab);
								NetworkObject networkObject2 = networkObject.NestedObjects[i];
								networkObject2.RuntimeFlags |= NetworkObjectRuntimeFlags.IsNested;
								networkObject2.RuntimeFlags |= NetworkObjectRuntimeFlags.OwnsNestedObjects;
								NetworkId networkId2 = this.IsResume ? this.<SpawnInternal>g__CheckIdOrGetNewId|370_0((args.ResumeNO != null) ? args.ResumeNO.NestedObjects[i] : null, ref CS$<>8__locals1) : this.Simulation.GetNextId();
								Simulation simulation2 = this.Simulation;
								NetworkId id2 = networkId2;
								int wordCount2 = NetworkObject.GetWordCount(networkObject2);
								int behaviourCount2 = networkObject2.NetworkedBehaviours.Length;
								NetworkId id3 = networkObjectMeta.Id;
								nestingKey = new NetworkObjectNestingKey(i + 1);
								flags = this.FlagsFromInstance(networkObject2);
								NetworkObjectMeta meta = simulation2.AllocateObject(id2, wordCount2, default(NetworkObjectTypeId), behaviourCount2, id3, nestingKey, flags);
								this.InitializeNetworkObjectInstance(meta, networkObject2, args.InputAuthority, options, args.MasterClientOverride);
							}
							bool flag6 = NetworkRunner.IsAwakeAtInitialization(networkObject);
							if (flag6)
							{
								this.InitializeNetworkObjectState(networkObject);
								foreach (NetworkObject networkObject3 in networkObject.NestedObjects)
								{
									bool flag7 = NetworkRunner.IsAwakeAtInitialization(networkObject3);
									if (flag7)
									{
										this.InitializeNetworkObjectState(networkObject3);
									}
								}
								this.InvokeBeforeSpawnedCallbacks(networkObject, options, args.OnBeforeSpawned as NetworkRunner.OnBeforeSpawned);
								foreach (NetworkObject networkObject4 in networkObject.NestedObjects)
								{
									bool flag8 = NetworkRunner.IsAwakeAtInitialization(networkObject4);
									if (flag8)
									{
										this.InvokeBeforeSpawnedCallbacks(networkObject4, options, null);
									}
								}
								this.InvokeSpawnedCallback(networkObject);
								foreach (NetworkObject networkObject5 in networkObject.NestedObjects)
								{
									bool flag9 = NetworkRunner.IsAwakeAtInitialization(networkObject5);
									if (flag9)
									{
										this.InvokeSpawnedCallback(networkObject5);
									}
								}
								this.InvokeAfterSpawnedCallback(networkObject);
								foreach (NetworkObject networkObject6 in networkObject.NestedObjects)
								{
									bool flag10 = NetworkRunner.IsAwakeAtInitialization(networkObject6);
									if (flag10)
									{
										this.InvokeAfterSpawnedCallback(networkObject6);
									}
								}
							}
							else
							{
								Assert.Check<string>(!networkObject.gameObject.activeInHierarchy, "Expected to be inactive {0}", networkObject.Name);
							}
							bool isClient = this.IsClient;
							if (isClient)
							{
								bool flag11 = this._config.Simulation.Topology == Topologies.Shared;
								if (flag11)
								{
									this.Simulation.Replicator.OnObjectSpawnedLocal(networkObjectMeta.Id);
									foreach (NetworkObject networkObject7 in networkObject.NestedObjects)
									{
										Assert.Check(BehaviourUtils.IsNotNull(networkObject7));
										bool flag12 = BehaviourUtils.IsAlive(networkObject7);
										if (flag12)
										{
											bool isValid = networkObject7.Id.IsValid;
											if (isValid)
											{
												this.Simulation.Replicator.OnObjectSpawnedLocal(networkObject7.Id);
											}
											else
											{
												TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
												if (logTraceObject != null)
												{
													logTraceObject.Warn(networkObject7, "Not invoking OnObjectSpawnedLocal for nested object because it has an invalid ID");
												}
											}
										}
										else
										{
											TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
											if (logTraceObject2 != null)
											{
												logTraceObject2.Warn(networkObject7, "Not invoking OnObjectSpawnedLocal for nested object because it is not alive");
											}
										}
									}
								}
							}
							result = this.<SpawnInternal>g__Complete|370_2(networkObject, ref CS$<>8__locals1);
						}
						else
						{
							result = this.<SpawnInternal>g__Failed|370_1(NetworkSpawnStatus.FailedToCreateInstance, ref CS$<>8__locals1);
						}
					}
				}
			}
			return result;
		}

		private static void ApplySpawnArgs(NetworkObject obj, in NetworkRunner.SpawnArgs spawnArgs)
		{
			bool flag = spawnArgs.Position != null;
			bool flag2 = spawnArgs.Rotation != null;
			bool flag3 = flag;
			if (flag3)
			{
				obj.transform.position = spawnArgs.Position.Value;
			}
			bool flag4 = flag2;
			if (flag4)
			{
				obj.transform.rotation = ((spawnArgs.Rotation.Value == default(Quaternion)) ? Quaternion.identity : spawnArgs.Rotation.Value);
			}
		}

		public T Spawn<T>(T prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0) where T : SimulationBehaviour
		{
			bool flag = BehaviourUtils.IsNull(prefab);
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObject prefab2;
			bool flag2 = !prefab.TryGetBehaviour<NetworkObject>(out prefab2);
			if (flag2)
			{
				throw new ArgumentException("No NetworkObject component", "prefab");
			}
			NetworkObject networkObject = this.Spawn(prefab2, position, rotation, inputAuthority, onBeforeSpawned, flags);
			T t = default(T);
			bool flag3 = BehaviourUtils.IsNotNull(networkObject) && !networkObject.TryGetComponent<T>(out t);
			if (flag3)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(this, string.Concat(new string[]
					{
						"Found no ",
						typeof(T).FullName,
						" on the GameObject ",
						t.name,
						". The prefab was instantiated."
					}));
				}
			}
			return t;
		}

		public NetworkObject Spawn(GameObject prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = prefab == null;
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObject prefab2;
			bool flag2 = !prefab.TryGetComponent<NetworkObject>(out prefab2);
			if (flag2)
			{
				throw new ArgumentException("No NetworkObject component", "prefab");
			}
			return this.Spawn(prefab2, position, rotation, inputAuthority, onBeforeSpawned, flags);
		}

		public NetworkObject Spawn(NetworkObject prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = BehaviourUtils.IsNull(prefab);
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObjectTypeId networkObjectTypeId = prefab.NetworkTypeId;
			bool flag2 = !networkObjectTypeId.IsValid;
			if (flag2)
			{
				NetworkObjectPrefabData networkObjectPrefabData;
				bool flag3 = prefab.TryGetComponent<NetworkObjectPrefabData>(out networkObjectPrefabData);
				if (!flag3)
				{
					throw new InvalidOperationException(string.Format("Prefab {0} has not been added to the PrefabTable.", prefab));
				}
				NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, networkObjectPrefabData.Guid);
				bool isValid = prefabId.IsValid;
				if (!isValid)
				{
					throw new InvalidOperationException(string.Format("Prefab {0} has been baked with a guid {1}, but such guid failed to be translated into a prefab id by the object provider.", prefab, networkObjectPrefabData.Guid));
				}
				networkObjectTypeId = prefabId;
				prefab.NetworkTypeId = networkObjectTypeId;
			}
			NetworkObject resumeNO = (this.IsResume && prefab.Id.IsValid) ? prefab : null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(networkObjectTypeId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(networkObjectTypeId);
		}

		public NetworkObject Spawn(NetworkPrefabRef prefabRef, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = !prefabRef.IsValid;
			if (flag)
			{
				throw new ArgumentException("Not valid.", "prefabRef");
			}
			NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, (NetworkObjectGuid)prefabRef);
			bool flag2 = !prefabId.IsValid;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Prefab {0} failed to be translated into a prefab id by the object provider.", prefabRef));
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(prefabId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(prefabId);
		}

		public NetworkObject Spawn(NetworkObjectGuid prefabGuid, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = !prefabGuid.IsValid;
			if (flag)
			{
				throw new ArgumentException("Not valid.", "prefabGuid");
			}
			NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, prefabGuid);
			bool flag2 = !prefabId.IsValid;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Prefab {0} failed to be translated into a prefab id by the object provider.", prefabGuid));
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(prefabId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(prefabId);
		}

		public NetworkObject Spawn(NetworkPrefabId typeId, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = !typeId.IsValid;
			if (flag)
			{
				throw new ArgumentException("typeId");
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(typeId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(typeId);
		}

		public NetworkSpawnStatus TrySpawn<T>(T prefab, out T obj, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0) where T : SimulationBehaviour
		{
			bool flag = BehaviourUtils.IsNull(prefab);
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObject prefab2;
			bool flag2 = !prefab.TryGetBehaviour<NetworkObject>(out prefab2);
			if (flag2)
			{
				throw new ArgumentException("No NetworkObject component", "prefab");
			}
			NetworkObject networkObject;
			NetworkSpawnStatus result = this.TrySpawn(prefab2, out networkObject, position, rotation, inputAuthority, onBeforeSpawned, flags);
			obj = default(T);
			bool flag3 = BehaviourUtils.IsNotNull(networkObject) && !networkObject.TryGetComponent<T>(out obj);
			if (flag3)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(this, string.Concat(new string[]
					{
						"Found no ",
						typeof(T).FullName,
						" on the GameObject ",
						obj.name,
						". The prefab was instantiated."
					}));
				}
			}
			return result;
		}

		public NetworkSpawnStatus TrySpawn(GameObject prefab, out NetworkObject obj, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = prefab == null;
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObject prefab2;
			bool flag2 = !prefab.TryGetComponent<NetworkObject>(out prefab2);
			if (flag2)
			{
				throw new ArgumentException("No NetworkObject component", "prefab");
			}
			return this.TrySpawn(prefab2, out obj, position, rotation, inputAuthority, onBeforeSpawned, flags);
		}

		public NetworkSpawnStatus TrySpawn(NetworkObject prefab, out NetworkObject obj, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = BehaviourUtils.IsNull(prefab);
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObjectTypeId networkObjectTypeId = prefab.NetworkTypeId;
			bool flag2 = !networkObjectTypeId.IsValid;
			if (flag2)
			{
				NetworkObjectPrefabData networkObjectPrefabData;
				bool flag3 = prefab.TryGetComponent<NetworkObjectPrefabData>(out networkObjectPrefabData);
				if (!flag3)
				{
					throw new InvalidOperationException(string.Format("Prefab {0} has not been added to the PrefabTable.", prefab));
				}
				NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, networkObjectPrefabData.Guid);
				bool isValid = prefabId.IsValid;
				if (!isValid)
				{
					throw new InvalidOperationException(string.Format("Prefab {0} has been baked with a guid {1}, but such guid failed to be translated into a prefab id by the object provider.", prefab, networkObjectPrefabData.Guid));
				}
				networkObjectTypeId = prefabId;
				prefab.NetworkTypeId = networkObjectTypeId;
			}
			NetworkObject resumeNO = (this.IsResume && prefab.Id.IsValid) ? prefab : null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(networkObjectTypeId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(out obj);
		}

		public NetworkSpawnStatus TrySpawn(NetworkPrefabRef prefabRef, out NetworkObject obj, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = !prefabRef.IsValid;
			if (flag)
			{
				throw new ArgumentException("Not valid.", "prefabRef");
			}
			NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, (NetworkObjectGuid)prefabRef);
			bool flag2 = !prefabId.IsValid;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Prefab {0} failed to be translated into a prefab id by the object provider.", prefabRef));
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(prefabId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(out obj);
		}

		public NetworkSpawnStatus TrySpawn(NetworkObjectGuid prefabGuid, out NetworkObject obj, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = !prefabGuid.IsValid;
			if (flag)
			{
				throw new ArgumentException("Not valid.", "prefabGuid");
			}
			NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, prefabGuid);
			bool flag2 = !prefabId.IsValid;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Prefab {0} failed to be translated into a prefab id by the object provider.", prefabGuid));
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(prefabId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(out obj);
		}

		public NetworkSpawnStatus TrySpawn(NetworkPrefabId typeId, out NetworkObject obj, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0)
		{
			bool flag = !typeId.IsValid;
			if (flag)
			{
				throw new ArgumentException("typeId");
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(typeId, position, rotation, inputAuthority, onBeforeSpawned, flags, null, true, resumeNO);
			return this.SpawnInternal(spawnArgs).ConsumeSyncSpawn(out obj);
		}

		public NetworkSpawnOp SpawnAsync<T>(T prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0, NetworkObjectSpawnDelegate onCompleted = null) where T : SimulationBehaviour
		{
			bool flag = BehaviourUtils.IsNull(prefab);
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObject prefab2;
			bool flag2 = !prefab.TryGetBehaviour<NetworkObject>(out prefab2);
			if (flag2)
			{
				throw new ArgumentException("No NetworkObject component", "prefab");
			}
			T t = default(T);
			NetworkSpawnOp result = this.SpawnAsync(prefab2, position, rotation, inputAuthority, onBeforeSpawned, flags, onCompleted);
			NetworkObject networkObject = null;
			bool flag3 = result.Status == NetworkSpawnStatus.Spawned;
			if (flag3)
			{
				networkObject = result.Object;
			}
			bool flag4 = BehaviourUtils.IsNotNull(networkObject) && !networkObject.TryGetComponent<T>(out t);
			if (flag4)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(this, string.Concat(new string[]
					{
						"Found no ",
						typeof(T).FullName,
						" on the GameObject ",
						t.name,
						". The prefab was instantiated."
					}));
				}
			}
			return result;
		}

		public NetworkSpawnOp SpawnAsync(GameObject prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0, NetworkObjectSpawnDelegate onCompleted = null)
		{
			bool flag = prefab == null;
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObject prefab2;
			bool flag2 = !prefab.TryGetComponent<NetworkObject>(out prefab2);
			if (flag2)
			{
				throw new ArgumentException("No NetworkObject component", "prefab");
			}
			return this.SpawnAsync(prefab2, position, rotation, inputAuthority, onBeforeSpawned, flags, onCompleted);
		}

		public NetworkSpawnOp SpawnAsync(NetworkObject prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0, NetworkObjectSpawnDelegate onCompleted = null)
		{
			bool flag = BehaviourUtils.IsNull(prefab);
			if (flag)
			{
				throw new ArgumentNullException("prefab");
			}
			NetworkObjectTypeId networkObjectTypeId = prefab.NetworkTypeId;
			bool flag2 = !networkObjectTypeId.IsValid;
			if (flag2)
			{
				NetworkObjectPrefabData networkObjectPrefabData;
				bool flag3 = prefab.TryGetComponent<NetworkObjectPrefabData>(out networkObjectPrefabData);
				if (!flag3)
				{
					throw new InvalidOperationException(string.Format("Prefab {0} has not been added to the PrefabTable.", prefab));
				}
				NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, networkObjectPrefabData.Guid);
				bool isValid = prefabId.IsValid;
				if (!isValid)
				{
					throw new InvalidOperationException(string.Format("Prefab {0} has been baked with a guid {1}, but such guid failed to be translated into a prefab id by the object provider.", prefab, networkObjectPrefabData.Guid));
				}
				networkObjectTypeId = prefabId;
				prefab.NetworkTypeId = networkObjectTypeId;
			}
			NetworkObject resumeNO = (this.IsResume && prefab.Id.IsValid) ? prefab : null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(networkObjectTypeId, position, rotation, inputAuthority, onBeforeSpawned, flags, onCompleted, false, resumeNO);
			return this.SpawnInternal(spawnArgs);
		}

		public NetworkSpawnOp SpawnAsync(NetworkPrefabRef prefabRef, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0, NetworkObjectSpawnDelegate onCompleted = null)
		{
			bool flag = !prefabRef.IsValid;
			if (flag)
			{
				throw new ArgumentException("Not valid.", "prefabRef");
			}
			NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, (NetworkObjectGuid)prefabRef);
			bool flag2 = !prefabId.IsValid;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Prefab {0} failed to be translated into a prefab id by the object provider.", prefabRef));
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(prefabId, position, rotation, inputAuthority, onBeforeSpawned, flags, onCompleted, false, resumeNO);
			return this.SpawnInternal(spawnArgs);
		}

		public NetworkSpawnOp SpawnAsync(NetworkObjectGuid prefabGuid, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0, NetworkObjectSpawnDelegate onCompleted = null)
		{
			bool flag = !prefabGuid.IsValid;
			if (flag)
			{
				throw new ArgumentException("Not valid.", "prefabGuid");
			}
			NetworkPrefabId prefabId = this._objectProvider.GetPrefabId(this, prefabGuid);
			bool flag2 = !prefabId.IsValid;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Prefab {0} failed to be translated into a prefab id by the object provider.", prefabGuid));
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(prefabId, position, rotation, inputAuthority, onBeforeSpawned, flags, onCompleted, false, resumeNO);
			return this.SpawnInternal(spawnArgs);
		}

		public NetworkSpawnOp SpawnAsync(NetworkPrefabId typeId, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, NetworkRunner.OnBeforeSpawned onBeforeSpawned = null, NetworkSpawnFlags flags = (NetworkSpawnFlags)0, NetworkObjectSpawnDelegate onCompleted = null)
		{
			bool flag = !typeId.IsValid;
			if (flag)
			{
				throw new ArgumentException("typeId");
			}
			NetworkObject resumeNO = null;
			NetworkRunner.SpawnArgs spawnArgs = new NetworkRunner.SpawnArgs(typeId, position, rotation, inputAuthority, onBeforeSpawned, flags, onCompleted, false, resumeNO);
			return this.SpawnInternal(spawnArgs);
		}

		public bool IsCloudReady
		{
			get
			{
				CloudServices cloudServices = this._cloudServices;
				return ((cloudServices != null) ? new bool?(cloudServices.IsCloudReady) : null).GetValueOrDefault();
			}
		}

		public bool IsInSession
		{
			get
			{
				CloudServices cloudServices = this._cloudServices;
				return ((cloudServices != null) ? new bool?(cloudServices.IsInRoom) : null).GetValueOrDefault();
			}
		}

		public string UserId
		{
			get
			{
				return this.IsCloudReady ? this._cloudServices.UserId : null;
			}
		}

		public AuthenticationValues AuthenticationValues
		{
			get
			{
				return this.IsCloudReady ? this._cloudServices.AuthenticationValues : null;
			}
		}

		public GameMode GameMode { get; private set; }

		public SessionInfo SessionInfo { get; private set; } = new SessionInfo(null);

		public LobbyInfo LobbyInfo { get; private set; } = new LobbyInfo();

		public ConnectionType CurrentConnectionType
		{
			get
			{
				bool isConnectedToServer = this.IsConnectedToServer;
				ConnectionType result;
				if (isConnectedToServer)
				{
					bool isRelayAddr = ((Simulation.Client)this._simulation).ServerAddress.IsRelayAddr;
					if (isRelayAddr)
					{
						result = ConnectionType.Relayed;
					}
					else
					{
						result = ConnectionType.Direct;
					}
				}
				else
				{
					result = ConnectionType.None;
				}
				return result;
			}
		}

		public NATType NATType
		{
			get
			{
				return (this._cloudServices != null) ? this._cloudServices.NATType : NATType.Invalid;
			}
		}

		public bool IsSharedModeMasterClient
		{
			get
			{
				return this.GameMode == GameMode.Shared && this.IsClient && this._cloudServices != null && this._cloudServices.IsMasterClient;
			}
		}

		[DebuggerStepThrough]
		public Task<StartGameResult> JoinSessionLobby(SessionLobby sessionLobby, string lobbyID = null, AuthenticationValues authentication = null, FusionAppSettings customAppSettings = null, bool? useDefaultCloudPorts = false, CancellationToken cancellationToken = default(CancellationToken), bool useCachedRegions = true)
		{
			NetworkRunner.<JoinSessionLobby>d__423 <JoinSessionLobby>d__ = new NetworkRunner.<JoinSessionLobby>d__423();
			<JoinSessionLobby>d__.<>t__builder = AsyncTaskMethodBuilder<StartGameResult>.Create();
			<JoinSessionLobby>d__.<>4__this = this;
			<JoinSessionLobby>d__.sessionLobby = sessionLobby;
			<JoinSessionLobby>d__.lobbyID = lobbyID;
			<JoinSessionLobby>d__.authentication = authentication;
			<JoinSessionLobby>d__.customAppSettings = customAppSettings;
			<JoinSessionLobby>d__.useDefaultCloudPorts = useDefaultCloudPorts;
			<JoinSessionLobby>d__.cancellationToken = cancellationToken;
			<JoinSessionLobby>d__.useCachedRegions = useCachedRegions;
			<JoinSessionLobby>d__.<>1__state = -1;
			<JoinSessionLobby>d__.<>t__builder.Start<NetworkRunner.<JoinSessionLobby>d__423>(ref <JoinSessionLobby>d__);
			return <JoinSessionLobby>d__.<>t__builder.Task;
		}

		public Task<StartGameResult> StartGame(StartGameArgs args)
		{
			bool alreadyInitialized = this._alreadyInitialized;
			Task<StartGameResult> result;
			if (alreadyInitialized)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log("Failed: NetworkRunner should not be reused.");
				}
				result = Task.FromResult<StartGameResult>(new StartGameResult(ShutdownReason.OperationCanceled, null, null));
			}
			else
			{
				this._alreadyInitialized = true;
				CloudServices cloudServices = this._cloudServices;
				bool flag = !((cloudServices != null) ? new bool?(cloudServices.IsInLobby) : null).GetValueOrDefault() && (this.IsStarting || this.IsRunning);
				if (flag)
				{
					result = Task.FromResult<StartGameResult>(new StartGameResult(ShutdownReason.AlreadyRunning, null, null));
				}
				else
				{
					HostMigrationToken hostMigrationToken = args.HostMigrationToken;
					args.GameMode = ((hostMigrationToken != null) ? hostMigrationToken.GameMode : args.GameMode);
					args.DisableNATPunchthrough |= RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
					args.Config = (args.Config ?? NetworkProjectConfig.Global).Copy();
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log(this, string.Format("SDK: {0}", Versioning.GetCurrentVersion));
					}
					DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
					if (logDebug2 != null)
					{
						logDebug2.Log(this, string.Format("StartGame: {0}", args));
					}
					this.GameMode = args.GameMode;
					this._simulationShutdown = (NetworkRunner.ShutdownFlags)0;
					GameMode gameMode = args.GameMode;
					GameMode gameMode2 = gameMode;
					if (gameMode2 != GameMode.Single)
					{
						if (gameMode2 - GameMode.Shared > 4)
						{
							this.GameMode = (GameMode)0;
							result = Task.FromResult<StartGameResult>(new StartGameResult(ShutdownReason.IncompatibleConfiguration, null, null));
						}
						else
						{
							bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
							if (isUNITY_WEBGL)
							{
								GameMode gameMode3 = args.GameMode;
								GameMode gameMode4 = gameMode3;
								if (gameMode4 - GameMode.Server <= 3)
								{
									DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
									if (logDebug3 != null)
									{
										logDebug3.Warn(this, string.Format("The GameMode {0} is not intended for use on WebGL builds. ", args.GameMode) + "For more information, please refer to the Fusion Introduction Page at https://doc.photonengine.com/fusion/v2/fusion-intro. Consider using Shared Mode or switching to a platform that fully supports the selected GameMode.");
									}
									bool flag2 = !args.Config.AllowClientServerModesInWebGL && args.GameMode != GameMode.Client;
									if (flag2)
									{
										DebugLogStream logDebug4 = InternalLogStreams.LogDebug;
										if (logDebug4 != null)
										{
											logDebug4.Error(this, string.Format("The GameMode {0} is not allowed in WebGL builds by default. If you still want to use it, please enable it in the NetworkProjectConfig.", args.GameMode));
										}
										return Task.FromResult<StartGameResult>(new StartGameResult(ShutdownReason.IncompatibleConfiguration, null, null));
									}
								}
							}
							result = this.StartGameModeCloud(args);
						}
					}
					else
					{
						result = this.StartGameModeSinglePlayer(args);
					}
				}
			}
			return result;
		}

		internal Task ConnectToCloud(AuthenticationValues authentication = null, FusionAppSettings customAppSettings = null, CloudCommunicator externalCommunicator = null, CancellationToken externalCancellationToken = default(CancellationToken), bool? useDefaultCloudPorts = false, bool useCachedRegions = false)
		{
			FusionAppSettings fusionAppSettings = customAppSettings;
			PhotonAppSettings photonAppSettings;
			bool flag = fusionAppSettings == null && PhotonAppSettings.TryGetGlobal(out photonAppSettings);
			if (flag)
			{
				fusionAppSettings = photonAppSettings.AppSettings;
			}
			bool flag2 = fusionAppSettings == null;
			if (flag2)
			{
				throw new InvalidOperationException("Photon Application Settings not found.");
			}
			bool flag3 = useCachedRegions && !string.IsNullOrEmpty(NetworkRunner._cachedRegionSummary);
			if (flag3)
			{
				fusionAppSettings.BestRegionSummaryFromStorage = NetworkRunner._cachedRegionSummary;
			}
			bool flag4 = this._cloudServices == null;
			if (flag4)
			{
				this._cloudServices = new CloudServices(this, fusionAppSettings, externalCommunicator);
			}
			this.SessionInfo = new SessionInfo(this);
			bool isCloudReady = this._cloudServices.IsCloudReady;
			Task result;
			if (isCloudReady)
			{
				result = Task.CompletedTask;
			}
			else
			{
				result = this._cloudServices.ConnectToCloud(fusionAppSettings, authentication, externalCancellationToken, useDefaultCloudPorts);
			}
			return result;
		}

		[DebuggerStepThrough]
		private Task DisconnectFromCloud()
		{
			NetworkRunner.<DisconnectFromCloud>d__426 <DisconnectFromCloud>d__ = new NetworkRunner.<DisconnectFromCloud>d__426();
			<DisconnectFromCloud>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DisconnectFromCloud>d__.<>4__this = this;
			<DisconnectFromCloud>d__.<>1__state = -1;
			<DisconnectFromCloud>d__.<>t__builder.Start<NetworkRunner.<DisconnectFromCloud>d__426>(ref <DisconnectFromCloud>d__);
			return <DisconnectFromCloud>d__.<>t__builder.Task;
		}

		[DebuggerStepThrough]
		private Task<StartGameResult> StartGameModeSinglePlayer(StartGameArgs args)
		{
			NetworkRunner.<StartGameModeSinglePlayer>d__427 <StartGameModeSinglePlayer>d__ = new NetworkRunner.<StartGameModeSinglePlayer>d__427();
			<StartGameModeSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder<StartGameResult>.Create();
			<StartGameModeSinglePlayer>d__.<>4__this = this;
			<StartGameModeSinglePlayer>d__.args = args;
			<StartGameModeSinglePlayer>d__.<>1__state = -1;
			<StartGameModeSinglePlayer>d__.<>t__builder.Start<NetworkRunner.<StartGameModeSinglePlayer>d__427>(ref <StartGameModeSinglePlayer>d__);
			return <StartGameModeSinglePlayer>d__.<>t__builder.Task;
		}

		[DebuggerStepThrough]
		private Task<StartGameResult> StartGameModeCloud(StartGameArgs args)
		{
			NetworkRunner.<StartGameModeCloud>d__428 <StartGameModeCloud>d__ = new NetworkRunner.<StartGameModeCloud>d__428();
			<StartGameModeCloud>d__.<>t__builder = AsyncTaskMethodBuilder<StartGameResult>.Create();
			<StartGameModeCloud>d__.<>4__this = this;
			<StartGameModeCloud>d__.args = args;
			<StartGameModeCloud>d__.<>1__state = -1;
			<StartGameModeCloud>d__.<>t__builder.Start<NetworkRunner.<StartGameModeCloud>d__428>(ref <StartGameModeCloud>d__);
			return <StartGameModeCloud>d__.<>t__builder.Task;
		}

		[DebuggerStepThrough]
		private Task<StartGameResult> ShutdownAndBuildResult(Exception e)
		{
			NetworkRunner.<ShutdownAndBuildResult>d__429 <ShutdownAndBuildResult>d__ = new NetworkRunner.<ShutdownAndBuildResult>d__429();
			<ShutdownAndBuildResult>d__.<>t__builder = AsyncTaskMethodBuilder<StartGameResult>.Create();
			<ShutdownAndBuildResult>d__.<>4__this = this;
			<ShutdownAndBuildResult>d__.e = e;
			<ShutdownAndBuildResult>d__.<>1__state = -1;
			<ShutdownAndBuildResult>d__.<>t__builder.Start<NetworkRunner.<ShutdownAndBuildResult>d__429>(ref <ShutdownAndBuildResult>d__);
			return <ShutdownAndBuildResult>d__.<>t__builder.Task;
		}

		internal void InvokeSessionListUpdated(List<SessionInfo> sessionList)
		{
			try
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					this._callbacks[i].OnSessionListUpdated(this, sessionList);
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
		}

		internal void InvokeCustomAuthenticationResponse(Dictionary<string, object> data)
		{
			try
			{
				for (int i = 0; i < this._callbacks.Count; i++)
				{
					this._callbacks[i].OnCustomAuthenticationResponse(this, data);
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
		}

		[CompilerGenerated]
		private void <Fusion.Simulation.ICallbacks.UpdateRemotePrefabs>g__InstanceAcquired|337_0(NetworkObjectMeta meta, NetworkObject instance)
		{
			bool flag = instance.Id.IsValid && instance.Id == meta.Id;
			if (flag)
			{
				bool flag2 = instance.Id == meta.Id;
				if (flag2)
				{
					Assert.Fail("Already initialized for the same ID: {0}", new object[]
					{
						LogUtils.GetDump<NetworkObject>(instance)
					});
				}
			}
			Assert.Check<NetworkObjectTypeId, NetworkId>(BehaviourUtils.IsAlive(instance), "The instance has been destroyed {0} {1}", meta.Type, meta.Id);
			this.InitializeNetworkObjectInstance(meta, instance, null, (NetworkRunner.AttachOptions)0, null);
			this._remotePrefabsWaitingForSpawnedCallback.Add(instance);
		}

		[CompilerGenerated]
		private NetworkId <SpawnInternal>g__CheckIdOrGetNewId|370_0(NetworkObject obj, ref NetworkRunner.<>c__DisplayClass370_0 A_2)
		{
			return (obj != null && obj.Id.IsValid) ? obj.Id : this.Simulation.GetNextId();
		}

		[CompilerGenerated]
		private NetworkSpawnOp <SpawnInternal>g__Failed|370_1(NetworkSpawnStatus status, ref NetworkRunner.<>c__DisplayClass370_0 A_2)
		{
			NetworkSpawnOp result = new NetworkSpawnOp(this, status, null);
			NetworkObjectSpawnDelegate spawnedCallback = A_2.spawnedCallback;
			if (spawnedCallback != null)
			{
				spawnedCallback(result);
			}
			return result;
		}

		[CompilerGenerated]
		private NetworkSpawnOp <SpawnInternal>g__Complete|370_2(NetworkObject instance, ref NetworkRunner.<>c__DisplayClass370_0 A_2)
		{
			NetworkSpawnOp result = new NetworkSpawnOp(this, NetworkSpawnStatus.Spawned, instance);
			NetworkObjectSpawnDelegate spawnedCallback = A_2.spawnedCallback;
			if (spawnedCallback != null)
			{
				spawnedCallback(result);
			}
			instance.IsResume = false;
			return result;
		}

		[CompilerGenerated]
		private NetworkSpawnOp <SpawnInternal>g__Incomplete|370_3(in NetworkRunner.SpawnArgs spawnArgs, ref NetworkRunner.<>c__DisplayClass370_0 A_2)
		{
			bool synchronous = spawnArgs.Synchronous;
			NetworkSpawnOp result;
			if (synchronous)
			{
				this._spawnQueue.Enqueue(spawnArgs);
				result = new NetworkSpawnOp(this, NetworkSpawnStatus.Queued, null);
			}
			else
			{
				NetworkSpawnOp.AsyncOpData asyncOp = new NetworkSpawnOp.AsyncOpData
				{
					Status = NetworkSpawnStatus.Queued,
					Object = null
				};
				this._spawnQueue.Enqueue(new NetworkRunner.SpawnArgs(ref spawnArgs, delegate(NetworkSpawnOp op)
				{
					asyncOp.Complete(op);
				}));
				result = new NetworkSpawnOp(this, NetworkSpawnStatus.Queued, asyncOp);
			}
			return result;
		}

		private HostMigration _lastHostMigrationInfo;

		private byte[] _hostSnapshotTempData;

		private const int HostSnapshotTransferDataSize = 4096;

		internal volatile int LastSnapshotTick = -1;

		internal volatile int LastConfirmedSnapshotTick = -1;

		[NonSerialized]
		private NetworkRunner.DeferredShutdownParams _deferredShutdownParams = default(NetworkRunner.DeferredShutdownParams);

		[NonSerialized]
		internal Simulation _simulation;

		[NonSerialized]
		private NetworkRunner.SimulationPhase _simulationPhase;

		[NonSerialized]
		private NetworkRunner.ShutdownFlags _simulationShutdown = NetworkRunner.ShutdownFlags.Regular;

		[NonSerialized]
		private SimulationBehaviourUpdater _behaviourUpdater;

		[NonSerialized]
		private List<INetworkRunnerCallbacks> _callbacks;

		[NonSerialized]
		private List<NetworkId> _destroyIdsBuffer = new List<NetworkId>();

		[NonSerialized]
		private Queue<NetworkRunner.SpawnArgs> _spawnQueue;

		internal TaskCompletionSource<bool> _initializeOperation;

		internal bool OnGameStartedInvoked;

		private Queue<ISpawned> _spawnedSimBehaviourQueue;

		[NonSerialized]
		private NetworkProjectConfig _config;

		[NonSerialized]
		private int _ticksExecuted;

		[NonSerialized]
		private INetworkRunnerUpdater _updater;

		[NonSerialized]
		private INetworkObjectInitializer _objectInitializer;

		[NonSerialized]
		private INetworkObjectProvider _objectProvider;

		[NonSerialized]
		internal byte[] _connectionToken;

		[NonSerialized]
		private Dictionary<NetworkObjectTypeId, NetworkObject> _attachableInstances = new Dictionary<NetworkObjectTypeId, NetworkObject>();

		[NonSerialized]
		private bool? _provideInput;

		private CancellationTokenSource OperationsCancellationTokenSource = new CancellationTokenSource();

		private readonly List<NetworkObject> _remotePrefabsWaitingForSpawnedCallback = new List<NetworkObject>();

		private readonly Queue<NetworkId> _remoteCreateQueue = new Queue<NetworkId>();

		private readonly Queue<NetworkId> _remoteCreateNestedQueue = new Queue<NetworkId>();

		private readonly Queue<NetworkId> _remoteDestroyQueue = new Queue<NetworkId>();

		private Action<NetworkRunner> _onGameStartAction;

		internal Stack<NetworkObjectInactivityGuard> _inactivityGuardPool = new Stack<NetworkObjectInactivityGuard>();

		private static List<NetworkRunner> _instances = new List<NetworkRunner>();

		private bool _simulateMultiPeerPhysicsScenes = true;

		private INetworkSceneManager _sceneManager;

		[NonSerialized]
		private NetworkSceneInfo _sceneInfoInitial;

		[NonSerialized]
		private NetworkSceneInfoChangeSource _sceneInfoChangeSource;

		[NonSerialized]
		private NetworkSceneInfo _sceneInfoSnapshot;

		[NonSerialized]
		private TaskCompletionSource<int> _sceneLoadInitialTCS = new TaskCompletionSource<int>();

		private const bool DefaultSetActiveOnLoad = false;

		private Dictionary<long, List<byte[]>> _reliableTransfers = new Dictionary<long, List<byte[]>>();

		public static NetworkRunner.CloudConnectionLostHandler CloudConnectionLost;

		private bool _alreadyInitialized = false;

		public Func<string, ServerConnection, string> CloudAddressRewriter = null;

		internal AsyncOperationHandler<ShutdownReason> _startGameOperation;

		internal CloudServices _cloudServices;

		private static string _cachedRegionSummary = string.Empty;

		public enum BuildTypes
		{
			Debug,
			Release
		}

		public enum States
		{
			Starting = 1,
			Running,
			Shutdown
		}

		[Flags]
		private enum ShutdownFlags
		{
			Regular = 1
		}

		public delegate void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj);

		public delegate void ObjectDelegate(NetworkRunner runner, NetworkObject obj);

		private struct DeferredShutdownParams
		{
			public bool ShutdownRequested;

			public ShutdownReason ShutdownReason;

			public bool DestroyGO;
		}

		private enum SimulationPhase
		{
			None,
			Update,
			Render
		}

		private enum CreateInstanceResult
		{
			Success,
			Failed,
			InProgress,
			Ignore
		}

		[Flags]
		private enum AttachOptions
		{
			LocalSpawn = 1,
			AttachExisting = 2
		}

		[Flags]
		private enum SpawnFlagsInternal
		{
			DontDestroyOnLoad = 1,
			SharedModeStateAuthMasterClient = 2,
			SharedModeStateAuthLocalPlayer = 4,
			Synchronous = 65536
		}

		private readonly struct SpawnArgs
		{
			public SpawnArgs(in NetworkRunner.SpawnArgs other, NetworkObjectSpawnDelegate del)
			{
				this = other;
				bool flag = this.Spawned != null;
				if (flag)
				{
					this.Spawned = (NetworkObjectSpawnDelegate)Delegate.Combine(this.Spawned, del);
				}
				else
				{
					this.Spawned = del;
				}
			}

			public SpawnArgs(NetworkObjectTypeId typeId, Vector3? position, Quaternion? rotation, PlayerRef? inputAuthority, object onBeforeSpawned, NetworkSpawnFlags flags, NetworkObjectSpawnDelegate spawned, bool synchronous, NetworkObject resumeNO)
			{
				this.TypeId = typeId;
				this.Position = position;
				this.Rotation = rotation;
				this.InputAuthority = inputAuthority;
				this.OnBeforeSpawned = onBeforeSpawned;
				this.Spawned = spawned;
				this.ResumeNO = resumeNO;
				this.SpawnFlags = (NetworkRunner.SpawnFlagsInternal)flags;
				if (synchronous)
				{
					this.SpawnFlags |= NetworkRunner.SpawnFlagsInternal.Synchronous;
				}
			}

			public bool Synchronous
			{
				get
				{
					return (this.SpawnFlags & NetworkRunner.SpawnFlagsInternal.Synchronous) > (NetworkRunner.SpawnFlagsInternal)0;
				}
			}

			public bool DontDestroyOnLoad
			{
				get
				{
					return (this.SpawnFlags & NetworkRunner.SpawnFlagsInternal.DontDestroyOnLoad) > (NetworkRunner.SpawnFlagsInternal)0;
				}
			}

			public bool? MasterClientOverride
			{
				get
				{
					bool flag = (this.SpawnFlags & NetworkRunner.SpawnFlagsInternal.SharedModeStateAuthMasterClient) > (NetworkRunner.SpawnFlagsInternal)0;
					bool? result;
					if (flag)
					{
						result = new bool?(true);
					}
					else
					{
						bool flag2 = (this.SpawnFlags & NetworkRunner.SpawnFlagsInternal.SharedModeStateAuthLocalPlayer) > (NetworkRunner.SpawnFlagsInternal)0;
						if (flag2)
						{
							result = new bool?(false);
						}
						else
						{
							result = null;
						}
					}
					return result;
				}
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[").Append("TypeId").Append(": ").Append(this.TypeId);
				bool flag = this.Position != null;
				if (flag)
				{
					stringBuilder.Append(", ").Append("Position").Append(": ").Append(this.Position);
				}
				bool flag2 = this.Rotation != null;
				if (flag2)
				{
					stringBuilder.Append(", ").Append("Rotation").Append(": ").Append(this.Rotation);
				}
				bool flag3 = this.InputAuthority != null;
				if (flag3)
				{
					stringBuilder.Append(", ").Append("InputAuthority").Append(": ").Append(this.InputAuthority);
				}
				bool flag4 = this.OnBeforeSpawned != null;
				if (flag4)
				{
					stringBuilder.Append(", ").Append("OnBeforeSpawned").Append(": ").Append(this.OnBeforeSpawned);
				}
				bool flag5 = this.Spawned != null;
				if (flag5)
				{
					stringBuilder.Append(", ").Append("Spawned").Append(": ").Append(this.Spawned);
				}
				bool flag6 = this.SpawnFlags > (NetworkRunner.SpawnFlagsInternal)0;
				if (flag6)
				{
					stringBuilder.Append(", ").Append("SpawnFlags").Append(": ").Append(this.SpawnFlags);
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}

			public readonly NetworkObjectTypeId TypeId;

			public readonly Vector3? Position;

			public readonly Quaternion? Rotation;

			public readonly PlayerRef? InputAuthority;

			public readonly object OnBeforeSpawned;

			public readonly NetworkObjectSpawnDelegate Spawned;

			public readonly NetworkRunner.SpawnFlagsInternal SpawnFlags;

			public readonly NetworkObject ResumeNO;
		}

		public delegate void CloudConnectionLostHandler(NetworkRunner networkRunner, ShutdownReason shutdownReason, bool reconnecting);
	}
}
