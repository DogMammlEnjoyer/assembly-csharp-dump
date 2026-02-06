using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Fusion.Sockets;
using Fusion.Statistics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Fusion
{
	public abstract class Simulation : ILogSource, INetPeerGroupCallbacks
	{
		public void GetAreaOfInterestGizmoData([TupleElementNames(new string[]
		{
			"center",
			"size",
			"playerCount",
			"objectCount"
		})] List<ValueTuple<Vector3, Vector3, int, int>> result)
		{
			result.Clear();
			foreach (KeyValuePair<int, Simulation.AreaOfInterestCell> keyValuePair in this._aoiCells)
			{
				Vector3 item = Simulation.AreaOfInterest.ToCellCenter(keyValuePair.Key);
				Vector3 item2 = Vector3.one * (float)Simulation.AreaOfInterest.CELL_SIZE;
				result.Add(new ValueTuple<Vector3, Vector3, int, int>(item, item2, keyValuePair.Value.Connections.GetSetCount(), keyValuePair.Value.Objects.Count));
			}
		}

		public List<NetworkId> GetObjectsInAreaOfInterestForPlayer(PlayerRef player)
		{
			List<NetworkId> list = new List<NetworkId>();
			bool flag = !this.Runner.IsServer || this._config.ReplicationFeatures != NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
			List<NetworkId> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				bool flag2 = !player.IsRealPlayer || player.IsNone;
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Error(string.Format("Player {0} is not valid.", player));
					}
					result = list;
				}
				else
				{
					SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(player);
					bool flag3 = simulationConnectionForPlayer == null;
					if (flag3)
					{
						result = list;
					}
					else
					{
						foreach (int cellKey in simulationConnectionForPlayer.AreaOfInterestCells)
						{
							Simulation.AreaOfInterestCell areaOfInterestCell = this.AOI_GetCell(cellKey, false);
							bool flag4 = areaOfInterestCell != null;
							if (flag4)
							{
								for (NetworkObjectMeta networkObjectMeta = areaOfInterestCell.Objects.Head; networkObjectMeta != null; networkObjectMeta = NetworkObjectMeta.List.Next(networkObjectMeta))
								{
									list.Add(networkObjectMeta.Id);
								}
							}
						}
						result = list;
					}
				}
			}
			return result;
		}

		public void GetObjectsAndPlayersInAreaOfInterestCell(int cellKey, List<PlayerRef> players, List<NetworkId> objects)
		{
			players.Clear();
			objects.Clear();
			Simulation.AreaOfInterestCell areaOfInterestCell = this.AOI_GetCell(cellKey, false);
			bool flag = areaOfInterestCell != null;
			if (flag)
			{
				for (NetworkObjectMeta networkObjectMeta = areaOfInterestCell.Objects.Head; networkObjectMeta != null; networkObjectMeta = NetworkObjectMeta.List.Next(networkObjectMeta))
				{
					objects.Add(networkObjectMeta.Id);
				}
				bool flag2 = !areaOfInterestCell.Connections.Empty();
				if (flag2)
				{
					BitSet512.Iterator iterator = areaOfInterestCell.Connections.GetIterator();
					for (;;)
					{
						int index;
						bool flag3 = iterator.Next(out index);
						if (!flag3)
						{
							break;
						}
						players.Add(this.GetSimulationConnectionByIndex(index).Player);
					}
				}
			}
		}

		private Simulation.AreaOfInterestCell AOI_GetCell(int cellKey, bool create)
		{
			Assert.Check(cellKey > 0);
			Simulation.AreaOfInterestCell areaOfInterestCell;
			bool flag = !this._aoiCells.TryGetValue(cellKey, out areaOfInterestCell);
			if (flag)
			{
				if (!create)
				{
					return null;
				}
				bool flag2 = this._aoiCellsPool.Count > 0;
				if (flag2)
				{
					areaOfInterestCell = this._aoiCellsPool.Pop();
					Assert.Check(areaOfInterestCell.Objects.Count == 0);
					Assert.Check(areaOfInterestCell.Connections.Empty());
				}
				else
				{
					areaOfInterestCell = new Simulation.AreaOfInterestCell();
				}
				areaOfInterestCell.Key = cellKey;
				this._aoiCells.Add(cellKey, areaOfInterestCell);
			}
			Assert.Check<int, int>(areaOfInterestCell.Key == cellKey, areaOfInterestCell.Key, cellKey);
			return areaOfInterestCell;
		}

		private void AOI_ReleaseCell(Simulation.AreaOfInterestCell cell)
		{
			bool empty = cell.Empty;
			if (empty)
			{
				int key = cell.Key;
				cell.Key = 0;
				Assert.Check(this._aoiCells.ContainsKey(key));
				this._aoiCells.Remove(key);
				this._aoiCellsPool.Push(cell);
			}
		}

		internal void AOI_RemoveConnection(SimulationConnection sc)
		{
			int connectionIndex = sc.ConnectionIndex;
			HashSet<int> hashSet;
			bool flag = this._aoiConnections.TryGetValue(connectionIndex, out hashSet);
			if (flag)
			{
				foreach (int cellKey in hashSet)
				{
					Simulation.AreaOfInterestCell areaOfInterestCell = this.AOI_GetCell(cellKey, false);
					Assert.Check(areaOfInterestCell != null);
					areaOfInterestCell.Connections.Clear(connectionIndex);
					bool empty = areaOfInterestCell.Empty;
					if (empty)
					{
						this.AOI_ReleaseCell(areaOfInterestCell);
					}
				}
				hashSet.Clear();
			}
		}

		internal void AOI_UpdateAreaOfInterest(SimulationConnection sc)
		{
			bool flag = ((sc != null) ? sc.AreaOfInterestCells : null) == null;
			if (!flag)
			{
				bool flag2 = !sc.AreaOfInterestHasBeenUpdated && sc.AreaOfInterestCells.Count == 0;
				if (!flag2)
				{
					int connectionIndex = sc.ConnectionIndex;
					HashSet<int> hashSet;
					bool flag3 = !this._aoiConnections.TryGetValue(connectionIndex, out hashSet);
					if (flag3)
					{
						this._aoiConnections.Add(connectionIndex, hashSet = new HashSet<int>());
					}
					sc.AreaOfInterestHasBeenUpdated = true;
					HashSet<int> areaOfInterestCells = sc.AreaOfInterestCells;
					bool flag4 = false;
					foreach (int num in hashSet)
					{
						bool flag5 = areaOfInterestCells.Contains(num);
						if (!flag5)
						{
							flag4 = true;
							Simulation.AreaOfInterestCell areaOfInterestCell = this.AOI_GetCell(num, false);
							bool flag6 = areaOfInterestCell == null;
							if (!flag6)
							{
								Assert.Check(areaOfInterestCell.Connections.IsSet(connectionIndex));
								areaOfInterestCell.Connections.Clear(connectionIndex);
								NetworkObjectMeta networkObjectMeta = areaOfInterestCell.Objects.Head;
								while (networkObjectMeta != null)
								{
									NetworkObjectMeta networkObjectMeta2 = networkObjectMeta;
									networkObjectMeta = NetworkObjectMeta.List.Next(networkObjectMeta);
									this.ExitAreaOfInterest(sc, networkObjectMeta2.Id);
								}
								bool empty = areaOfInterestCell.Empty;
								if (empty)
								{
									this.AOI_ReleaseCell(areaOfInterestCell);
								}
							}
						}
					}
					bool flag7 = !flag4 && hashSet.Count == areaOfInterestCells.Count;
					if (!flag7)
					{
						foreach (int num2 in areaOfInterestCells)
						{
							bool flag8 = hashSet.Contains(num2);
							if (!flag8)
							{
								Simulation.AreaOfInterestCell areaOfInterestCell2 = this.AOI_GetCell(num2, true);
								Assert.Check(!areaOfInterestCell2.Connections.IsSet(connectionIndex));
								areaOfInterestCell2.Connections.Set(connectionIndex);
								NetworkObjectMeta networkObjectMeta3 = areaOfInterestCell2.Objects.Head;
								while (networkObjectMeta3 != null)
								{
									NetworkObjectMeta networkObjectMeta4 = networkObjectMeta3;
									networkObjectMeta3 = NetworkObjectMeta.List.Next(networkObjectMeta3);
									this.EnterAreaOfInterest(sc, networkObjectMeta4.Id);
								}
							}
						}
						hashSet.Clear();
						hashSet.UnionWith(areaOfInterestCells);
					}
				}
			}
		}

		internal bool AOI_Query(SimulationConnection sc, List<NetworkObjectMeta.List> result, bool clearResult)
		{
			if (clearResult)
			{
				result.Clear();
			}
			int count = result.Count;
			HashSet<int> hashSet;
			bool flag = this._aoiConnections.TryGetValue(sc.ConnectionIndex, out hashSet);
			if (flag)
			{
				foreach (int cellKey in hashSet)
				{
					Simulation.AreaOfInterestCell areaOfInterestCell = this.AOI_GetCell(cellKey, false);
					bool flag2 = areaOfInterestCell.Objects.Count > 0;
					if (flag2)
					{
						result.Add(areaOfInterestCell.Objects);
					}
				}
			}
			return result.Count > count;
		}

		internal void AOI_RemoveFromAreaOfInterest(NetworkObjectMeta meta, bool invokeExit = false)
		{
			Assert.Check(meta.Id.IsValid);
			bool flag = meta.AreaOfInterestCell == 0;
			if (!flag)
			{
				Simulation.AreaOfInterestCell areaOfInterestCell = this.AOI_GetCell(meta.AreaOfInterestCell, false);
				areaOfInterestCell.Objects.Remove(meta);
				if (invokeExit)
				{
					NetworkId id = meta.Id;
					BitSet512.Iterator iterator = areaOfInterestCell.Connections.GetIterator();
					for (;;)
					{
						int connection;
						bool flag2 = iterator.Next(out connection);
						if (!flag2)
						{
							break;
						}
						this.ExitAreaOfInterest(connection, id);
					}
				}
			}
		}

		internal void AOI_UpdateAreaOfInterest(NetworkObjectMeta meta, int newCellKey)
		{
			bool flag = meta.AreaOfInterestCell == newCellKey;
			if (!flag)
			{
				NetworkId id = meta.Id;
				bool flag2 = meta.AreaOfInterestCell > 0;
				Simulation.AreaOfInterestCell areaOfInterestCell;
				if (flag2)
				{
					areaOfInterestCell = this.AOI_GetCell(meta.AreaOfInterestCell, false);
					areaOfInterestCell.Objects.Remove(meta);
					bool empty = areaOfInterestCell.Empty;
					if (empty)
					{
						this.AOI_ReleaseCell(areaOfInterestCell);
						areaOfInterestCell = null;
					}
				}
				else
				{
					areaOfInterestCell = null;
				}
				Simulation.AreaOfInterestCell areaOfInterestCell2 = this.AOI_GetCell(newCellKey, true);
				meta.AreaOfInterestCell = newCellKey;
				areaOfInterestCell2.Objects.AddLast(meta);
				bool flag3 = areaOfInterestCell != null;
				if (flag3)
				{
					bool flag4 = !areaOfInterestCell.Connections.Equals(areaOfInterestCell2.Connections);
					if (flag4)
					{
						BitSet512.Iterator iterator = areaOfInterestCell.Connections.GetIterator();
						iterator._set.AndNot(areaOfInterestCell2.Connections);
						for (;;)
						{
							int connection;
							bool flag5 = iterator.Next(out connection);
							if (!flag5)
							{
								break;
							}
							this.ExitAreaOfInterest(connection, id);
						}
						BitSet512.Iterator iterator2 = areaOfInterestCell2.Connections.GetIterator();
						iterator2._set.AndNot(areaOfInterestCell.Connections);
						for (;;)
						{
							int connection2;
							bool flag6 = iterator2.Next(out connection2);
							if (!flag6)
							{
								break;
							}
							this.EnterAreaOfInterest(connection2, id);
						}
					}
				}
				else
				{
					BitSet512.Iterator iterator3 = areaOfInterestCell2.Connections.GetIterator();
					for (;;)
					{
						int connection3;
						bool flag7 = iterator3.Next(out connection3);
						if (!flag7)
						{
							break;
						}
						this.EnterAreaOfInterest(connection3, id);
					}
				}
			}
		}

		internal void EnterAreaOfInterest(int connection, NetworkId id)
		{
			this.EnterAreaOfInterest(this.GetSimulationConnectionByIndex(connection), id);
		}

		internal void EnterAreaOfInterest(SimulationConnection connection, NetworkId id)
		{
			NetworkObjectMeta networkObjectMeta;
			Assert.Always(this.TryGetMeta(id, out networkObjectMeta), "Object not found");
			Assert.Always(networkObjectMeta.AreaOfInterestCell > 0, "AOI Cell not correct");
			NetworkObjectConnectionData objectData = connection.GetObjectData(id, true, false);
			bool flag = (objectData.UniqueData.Flags & NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) == (NetworkObjectHeaderPlayerDataFlags)0;
			if (flag)
			{
				this._callbacks.ObjectEnterAOI(connection, id);
			}
			Assert.Check((objectData.UniqueData.Flags & NetworkObjectHeaderPlayerDataFlags.InAreaOfInterest) == (NetworkObjectHeaderPlayerDataFlags)0, "Already has interest in");
			connection.SetActive(objectData, networkObjectMeta);
			objectData.SetPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.InAreaOfInterest, this);
		}

		internal void ExitAreaOfInterest(int connection, NetworkId id)
		{
			this.ExitAreaOfInterest(this.GetSimulationConnectionByIndex(connection), id);
		}

		internal void ExitAreaOfInterest(SimulationConnection connection, NetworkId id)
		{
			NetworkObjectConnectionData objectData = connection.GetObjectData(id, false, false);
			bool flag = objectData == null;
			if (!flag)
			{
				bool flag2 = (objectData.UniqueData.Flags & NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) == NetworkObjectHeaderPlayerDataFlags.InAreaOfInterest;
				if (flag2)
				{
					this._callbacks.ObjectExitAOI(connection, id);
				}
				NetworkObjectMeta networkObjectMeta;
				bool flag3 = this.TryGetMeta(id, out networkObjectMeta) && networkObjectMeta.AreaOfInterestCell > 0;
				if (flag3)
				{
					connection.SetActive(objectData, networkObjectMeta);
				}
				bool flag4 = (objectData.UniqueData.Flags & NetworkObjectHeaderPlayerDataFlags.InAreaOfInterest) == NetworkObjectHeaderPlayerDataFlags.InAreaOfInterest;
				if (flag4)
				{
					objectData.ClearPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.InAreaOfInterest, this);
				}
			}
		}

		public abstract Tick LatestServerTick { get; }

		internal unsafe SimulationRuntimeConfig RuntimeConfig
		{
			get
			{
				return *this.RuntimeConfigPtr;
			}
		}

		internal unsafe SimulationRuntimeConfig* RuntimeConfigPtr
		{
			get
			{
				SimulationRuntimeConfig* result;
				bool flag = this.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out result);
				if (flag)
				{
					return result;
				}
				throw new InvalidOperationException("RuntimeConfig can only be read after the first state update form the server has arrived, add a guard check with Simulation.HasObject(NetworkId.RuntimeConfig)");
			}
		}

		internal void InterpolateSequenceIncrement()
		{
			this._interpolateSequence += 1UL;
		}

		internal ulong InterpolateSequence
		{
			get
			{
				return this._interpolateSequence;
			}
		}

		internal bool HasRuntimeConfig
		{
			get
			{
				NetworkObjectMeta networkObjectMeta;
				return this.TryGetStruct(NetworkId.RuntimeConfig, out networkObjectMeta);
			}
		}

		public int TickStride
		{
			get
			{
				int result;
				if (!this.IsServer)
				{
					SimulationRuntimeConfig runtimeConfig = this.RuntimeConfig;
					result = runtimeConfig.TickRate.ClientTickStride;
				}
				else
				{
					SimulationRuntimeConfig runtimeConfig = this.RuntimeConfig;
					result = runtimeConfig.TickRate.ServerTickStride;
				}
				return result;
			}
		}

		public int TickRate
		{
			get
			{
				return this.RuntimeConfig.TickRate.Client;
			}
		}

		public double TickDeltaDouble
		{
			get
			{
				return this.RuntimeConfig.TickRate.ClientTickDelta;
			}
		}

		public float TickDeltaFloat
		{
			get
			{
				return (float)this.TickDeltaDouble;
			}
		}

		public int SendRate
		{
			get
			{
				return this.IsServer ? this.RuntimeConfig.TickRate.ServerSend : this.RuntimeConfig.TickRate.ClientSend;
			}
		}

		public double SendDelta
		{
			get
			{
				double result;
				if (!this.IsServer)
				{
					SimulationRuntimeConfig runtimeConfig = this.RuntimeConfig;
					result = runtimeConfig.TickRate.ClientSendDelta;
				}
				else
				{
					SimulationRuntimeConfig runtimeConfig = this.RuntimeConfig;
					result = runtimeConfig.TickRate.ServerSendDelta;
				}
				return result;
			}
		}

		public float DeltaTime
		{
			get
			{
				float result;
				if (!this.IsServer)
				{
					SimulationRuntimeConfig runtimeConfig = this.RuntimeConfig;
					result = (float)runtimeConfig.TickRate.ClientTickDelta;
				}
				else
				{
					SimulationRuntimeConfig runtimeConfig = this.RuntimeConfig;
					result = (float)runtimeConfig.TickRate.ServerTickDelta;
				}
				return result;
			}
		}

		public bool IsShutdown
		{
			get
			{
				return this._isShutdown;
			}
		}

		public float LocalAlpha
		{
			get
			{
				return this._localAlpha;
			}
		}

		public bool IsResimulation
		{
			get
			{
				return this._isResimulation;
			}
		}

		public bool IsLastTick
		{
			get
			{
				return this._isLastTick;
			}
		}

		public bool IsFirstTick
		{
			get
			{
				return this._isFirstTick;
			}
		}

		public bool IsForward
		{
			get
			{
				return !this._isResimulation;
			}
		}

		public bool IsLocalPlayerFirstExecution
		{
			get
			{
				return this._stage == SimulationStages.Forward;
			}
		}

		public Tick Tick
		{
			get
			{
				return this._tick;
			}
		}

		public Tick TickPrevious
		{
			get
			{
				return Math.Max(0, this._tick - this.TickStride);
			}
		}

		public double Time
		{
			get
			{
				return (double)this._tick * this.TickDeltaDouble;
			}
		}

		public int InputCount
		{
			get
			{
				return this._inputCollection.Count;
			}
		}

		public Topologies Topology
		{
			get
			{
				return this._config.Topology;
			}
		}

		public SimulationModes Mode
		{
			get
			{
				return this._mode;
			}
		}

		public SimulationStages Stage
		{
			get
			{
				return this._stage;
			}
		}

		public SimulationConfig Config
		{
			get
			{
				return this._config;
			}
		}

		public NetworkProjectConfig ProjectConfig
		{
			get
			{
				return this._projectConfig;
			}
		}

		public float RemoteAlpha
		{
			get
			{
				return this._remoteAlpha;
			}
		}

		public Tick RemoteTickPrevious
		{
			get
			{
				return this._interpFrom;
			}
		}

		public Tick RemoteTick
		{
			get
			{
				return this._interpTo;
			}
		}

		public bool IsClient
		{
			get
			{
				return this is Simulation.Client;
			}
		}

		public bool IsServer
		{
			get
			{
				return this is Simulation.Server;
			}
		}

		public bool IsPlayer
		{
			get
			{
				return this._mode == SimulationModes.Client || this._mode == SimulationModes.Host;
			}
		}

		public bool IsSinglePlayer
		{
			get
			{
				return this._mode == SimulationModes.Host && this._config.PlayerCount == 1;
			}
		}

		public bool IsMasterClient
		{
			get
			{
				return this._callbacks.IsSharedModeMasterClient;
			}
		}

		public virtual IEnumerable<PlayerRef> ActivePlayers
		{
			get
			{
				Simulation.<get_ActivePlayers>d__138 <get_ActivePlayers>d__ = new Simulation.<get_ActivePlayers>d__138(-2);
				<get_ActivePlayers>d__.<>4__this = this;
				return <get_ActivePlayers>d__;
			}
		}

		public bool IsRunning
		{
			get
			{
				return !this._isShutdown;
			}
		}

		internal Simulation.StateReplicator Replicator
		{
			get
			{
				return this._stateReplicator;
			}
		}

		internal Simulation.ICallbacks Callbacks
		{
			get
			{
				return this._callbacks;
			}
		}

		internal bool IsResume
		{
			get
			{
				return this._isResume;
			}
		}

		internal bool IsInTick
		{
			get
			{
				return this._isInTick;
			}
		}

		internal bool IsPaused
		{
			get
			{
				return this._isPaused != null && this._isPaused.Value;
			}
		}

		internal bool IsWaitingForTheInitialTick
		{
			get
			{
				return this._isInitialLocalTick;
			}
		}

		internal bool IsSceneInfoReady
		{
			get
			{
				return !this.IsWaitingForTheInitialTick || this.Topology != Topologies.Shared;
			}
		}

		internal IEnumerable<SimulationConnection> Connections
		{
			get
			{
				Simulation.<get_Connections>d__156 <get_Connections>d__ = new Simulation.<get_Connections>d__156(-2);
				<get_Connections>d__.<>4__this = this;
				return <get_Connections>d__;
			}
		}

		public unsafe NetAddress LocalAddress
		{
			get
			{
				return this._netPeer->Address;
			}
		}

		public unsafe NetConfig* NetConfigPointer
		{
			get
			{
				return NetPeer.GetConfigPointer(this._netPeer);
			}
		}

		public abstract PlayerRef LocalPlayer { get; }

		internal abstract double GetPlayerRtt(PlayerRef player);

		internal abstract void RecvPacket();

		internal abstract void WritePackets();

		internal abstract SimulationInput GetInput(Tick tick, PlayerRef player);

		internal Simulation(SimulationArgs args)
		{
			Assert.Check(sizeof(NetworkObjectHeader) == 80, "NetworkObjectHeader size != WORDS * REPLICATE_WORD_SIZE");
			this._fusionStatsManager = new FusionStatisticsManager();
			this._mode = args.Mode;
			this._config = args.Config.Simulation;
			this._projectConfig = args.Config;
			this._allocator = Allocator.Create(this._projectConfig.Heap.ToAllocatorConfig());
			this._allocatorObjects = Allocator.Create(this._projectConfig.Heap.ToAllocatorConfig());
			this._callbacks = args.Callbacks;
			this._isShutdown = false;
			this._isWaitingForShutdown = false;
			this._isInitialLocalTick = true;
			this._inputPool = new SimulationInput.Pool(this._config, this._allocator);
			this._inputRoot = this._inputPool.Acquire();
			this._inputCollection = new SimulationInputCollection(this._config.PlayerCount);
			this._players = new HashSet<PlayerRef>(PlayerRef.Comparer);
			this._metaLookup = new Dictionary<NetworkId, NetworkObjectMeta>(NetworkId.Comparer);
			this._playerDataLookup = new Dictionary<PlayerRef, NetworkId>();
			this._playerLeftTempObjectCache = new Dictionary<PlayerRef, NetworkId>();
			this._structs = new HashSet<NetworkId>(NetworkId.Comparer);
			this._sendContext = new Simulation.SendContext(this);
			this._recvContext = new Simulation.RecvContext(this);
			this.NetworkInit(args.Socket, args.Address);
			this._stateReplicator = new Simulation.StateReplicator(this);
			bool flag = (this.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
			if (flag)
			{
				this._globalInterestObjects = new HashSet<NetworkId>();
			}
			this._tickUpdateTimes = new Dictionary<Tick, double>();
		}

		internal unsafe PlayerRef Connection2Player(SimulationConnection c)
		{
			Assert.Check(c);
			PlayerRef player = this._connections[(int)c.Connection->LocalId.GroupIndex].Player;
			Assert.Check(player == c.Player);
			Assert.Check(this._connections[(int)c.Connection->LocalId.GroupIndex] == c);
			return player;
		}

		internal unsafe virtual PlayerRef Connection2Player(NetConnection* c)
		{
			Assert.Check((void*)c);
			return this._connections[(int)c->LocalId.GroupIndex].Player;
		}

		internal virtual int Player2Connection(PlayerRef player)
		{
			SimulationConnection simulationConnection;
			bool flag = this._playersConnections.TryGetValue(player, out simulationConnection);
			int result;
			if (flag)
			{
				result = simulationConnection.ConnectionIndex;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		internal void RegisterUniqueIdPlayerMapping(int actorid, byte[] id, PlayerRef playerRef)
		{
			Assert.Check(id);
			Assert.Check(id.Length == 8);
			this._uniqueIdPlayerRefMapping[BitConverter.ToUInt64(id, 0)] = new Simulation.PlayerRefMapping
			{
				ActorId = actorid,
				PlayerRef = playerRef
			};
			LogStream logInfo = InternalLogStreams.LogInfo;
			if (logInfo != null)
			{
				logInfo.Log(this, string.Format("RegisterUniqueIdPlayerMapping actorid:{0} id:{1}, player:{2}", actorid, BitConverter.ToUInt64(id, 0), playerRef));
			}
		}

		internal Simulation.PlayerRefMapping? GetPlayerRefMapping(byte[] id)
		{
			Assert.Check(id);
			Assert.Check(id.Length == 8);
			Simulation.PlayerRefMapping value;
			bool flag = this._uniqueIdPlayerRefMapping.TryGetValue(BitConverter.ToUInt64(id, 0), out value);
			Simulation.PlayerRefMapping? result;
			if (flag)
			{
				result = new Simulation.PlayerRefMapping?(value);
			}
			else
			{
				LogStream logWarn = InternalLogStreams.LogWarn;
				if (logWarn != null)
				{
					logWarn.Log(string.Format("no player mapping for {0} exists", BitConverter.ToUInt64(id, 0)));
				}
				result = null;
			}
			return result;
		}

		internal unsafe Simulation.PlayerRefMapping? GetPlayerRefMapping(byte* id)
		{
			Assert.Check((void*)id);
			byte[] array = new byte[8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = id[i];
			}
			return this.GetPlayerRefMapping(array);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CalculateUpdateTime()
		{
			bool isClient = this.IsClient;
			if (!isClient)
			{
				double updateTime = this._updateTime;
				this._updateTime = this._time.Now().Local;
				Assert.Check<double, double>(this._updateTime > updateTime, "Current Update Time must be bigger than previous Update Time {0} {1}", this._updateTime, updateTime);
			}
		}

		private void StepSimulation(SimulationStages stage, bool lastTick, bool firstTick, bool freeInput)
		{
			EngineProfiler.Begin("Simulation.StepSimulation");
			try
			{
				bool isResimulation = stage == SimulationStages.Resimulate;
				this._isLastTick = lastTick;
				this._isFirstTick = firstTick;
				this._isResimulation = isResimulation;
				bool flag = this.IsLastTick && !this.IsResimulation;
				if (flag)
				{
					Assert.Check(!this.IsResimulation, "IsResimulation should be false");
					this._callbacks.OnBeforeCopyPreviousState();
					foreach (NetworkObjectMeta networkObjectMeta in this._metaLookup.Values)
					{
						bool isObject = networkObjectMeta.IsObject;
						if (isObject)
						{
							networkObjectMeta.Previous.CopyFrom(networkObjectMeta);
						}
					}
				}
				this._tick = this._tick.Next(this.TickStride);
				bool isServer = this.IsServer;
				if (isServer)
				{
					this._tickUpdateTimes.Remove(this._tick - this.TickRate);
					this._tickUpdateTimes.Add(this._tick, this._updateTime);
				}
				this.InvokeTick(stage, freeInput);
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			finally
			{
				this._isLastTick = false;
				this._isFirstTick = false;
				this._isResimulation = false;
				EngineProfiler.End();
			}
		}

		protected virtual void AfterUpdate()
		{
		}

		protected unsafe virtual void NetworkConnected(NetConnection* connection)
		{
		}

		protected unsafe virtual void NetworkDisconnected(NetConnection* connection, NetDisconnectReason reason)
		{
		}

		protected virtual void NetworkReceiveDone()
		{
		}

		protected virtual void NoSimulation()
		{
		}

		protected virtual int BeforeSimulation()
		{
			return 0;
		}

		protected virtual void BeforeFirstTick()
		{
		}

		internal void SinglePlayerSetPaused(bool paused)
		{
			bool isSinglePlayer = this.IsSinglePlayer;
			if (isSinglePlayer)
			{
				this._isPaused = new bool?(paused);
			}
		}

		internal void RequestStateAuthority(NetworkId id, bool wants)
		{
			bool flag = this.Topology == Topologies.Shared;
			if (flag)
			{
				Assert.Check(sizeof(SimulationMessageInternal_SharedModeRequestStateAuthority) == 8, "SharedModeRequestStateAuthority unexpected size");
				SimulationMessageInternal_SharedModeRequestStateAuthority buffer;
				buffer.Acquire = (wants ? 1 : 0);
				buffer.Object = id;
				this.SendInternalSimulationMessage<SimulationMessageInternal_SharedModeRequestStateAuthority>(SimulationMessageInternalTypes.SharedModeRequestStateAuthority, buffer, null);
			}
		}

		internal void SetPlayerAlwaysInterested(PlayerRef player, NetworkId id, bool alwaysInterested)
		{
			bool flag = (this.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
			if (flag)
			{
				bool flag2 = this.Topology == Topologies.Shared;
				if (flag2)
				{
					Assert.Check(sizeof(SimulationMessageInternal_SharedModeSetAlwaysInterested) == 12, "SharedModeSetAlwaysInterested unexpected size");
					SimulationMessageInternal_SharedModeSetAlwaysInterested buffer;
					buffer.Interested = (alwaysInterested ? 1 : 0);
					buffer.Object = id;
					buffer.Player = player.AsIndex;
					this.SendInternalSimulationMessage<SimulationMessageInternal_SharedModeSetAlwaysInterested>(SimulationMessageInternalTypes.SharedModeSetAlwaysInterested, buffer, null);
				}
				else
				{
					bool flag3 = this.LocalPlayer == player || !this.PlayerValid(player);
					if (!flag3)
					{
						NetworkObjectMeta meta;
						bool flag4 = this.TryGetMeta(id, out meta);
						if (flag4)
						{
							if (alwaysInterested)
							{
								SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(player);
								if (simulationConnectionForPlayer != null)
								{
									simulationConnectionForPlayer.AddAlwaysInterested(meta);
								}
							}
							else
							{
								SimulationConnection simulationConnectionForPlayer2 = this.GetSimulationConnectionForPlayer(player);
								if (simulationConnectionForPlayer2 != null)
								{
									simulationConnectionForPlayer2.RemoveAlwaysInterested(meta);
								}
							}
						}
					}
				}
			}
		}

		internal unsafe void TempFree<[IsUnmanaged] T>(ref T* ptr) where T : struct, ValueType
		{
			bool flag = this._allocator.IsPointerInHeap(ptr);
			if (flag)
			{
				Allocator.Free<T>(this._allocator, ref ptr);
			}
			else
			{
				Assert.AlwaysFail("Pointer not part of temp allocator");
			}
		}

		[return: NotNull]
		internal unsafe void* TempAlloc(int size)
		{
			return Allocator.AllocAndClear(this._allocator, size);
		}

		[return: NotNull]
		internal unsafe T* TempAlloc<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return (T*)this.TempAlloc(sizeof(T));
		}

		[return: NotNull]
		internal unsafe T* TempAllocArray<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return (T*)this.TempAlloc(sizeof(T) * length);
		}

		[return: NotNull]
		internal unsafe T* TempDoubleArray<[IsUnmanaged] T>(ref T* oldArray, int oldLength) where T : struct, ValueType
		{
			int length = oldLength * 2;
			T* ptr = this.TempAllocArray<T>(length);
			Native.MemCpy((void*)ptr, oldArray, sizeof(T) * oldLength);
			this.TempFree<T>(ref oldArray);
			return ptr;
		}

		internal void ShutdownNativeSocket()
		{
			bool isShutdown = this._isShutdown;
			if (!isShutdown)
			{
				this.NetworkShutdown();
			}
		}

		internal void Dispose()
		{
			bool isShutdown = this._isShutdown;
			if (!isShutdown)
			{
				this._isShutdown = true;
				this._inputPool.Dispose();
				this._inputPool = null;
				this._inputRoot = null;
				Allocator.Dispose(this._allocator);
				this._allocator = null;
				Allocator.Dispose(this._allocatorObjects);
				this._allocatorObjects = null;
				this.HostMigrationDispose();
			}
		}

		internal void Destroy(NetworkId id, NetworkObjectDestroyFlags flags, PlayerRef destroyingPlayer = default(PlayerRef))
		{
			NetworkObjectMeta networkObjectMeta;
			bool flag = this.TryGetMeta(id, out networkObjectMeta);
			if (flag)
			{
				this.AOI_RemoveFromAreaOfInterest(networkObjectMeta, false);
			}
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(this, string.Format("Destroy({0}, {1})", id, flags));
			}
			bool flag2 = !this.IsServer;
			if (flag2)
			{
				bool flag3 = flags.Get(NetworkObjectDestroyFlags.DestroyState);
				if (flag3)
				{
					SimulationConnection simulationConnectionByIndex = this.GetSimulationConnectionByIndex(0);
					if (simulationConnectionByIndex != null)
					{
						simulationConnectionByIndex.ObjectData_Destroyed(id, false);
					}
					this.FreeObject(id);
				}
				else
				{
					bool flag4 = flags.Get(NetworkObjectDestroyFlags.DestroyedByEngine);
					if (flag4)
					{
						bool flag5 = networkObjectMeta != null;
						if (flag5)
						{
							TraceLogStream logTraceObject2 = InternalLogStreams.LogTraceObject;
							if (logTraceObject2 != null)
							{
								logTraceObject2.Log(this, string.Format("Requeuing {0}", id));
							}
							this._callbacks.RemoteObjectCreated(networkObjectMeta);
						}
					}
					else
					{
						bool flag6 = flags.Get(NetworkObjectDestroyFlags.DestroyedByReplicator);
						if (flag6)
						{
							SimulationConnection simulationConnectionByIndex2 = this.GetSimulationConnectionByIndex(0);
							if (simulationConnectionByIndex2 != null)
							{
								simulationConnectionByIndex2.ObjectData_Destroyed(id, false);
							}
						}
					}
				}
			}
			else
			{
				Assert.Check(flags.Get(NetworkObjectDestroyFlags.DestroyState));
				foreach (SimulationConnection simulationConnection in this._connections.Values)
				{
					simulationConnection.ObjectData_Destroyed(id, false);
				}
				this.FreeObject(id);
			}
		}

		internal bool PlayerValid(PlayerRef player)
		{
			return this._players.Contains(player);
		}

		internal void PlayerAdd(PlayerRef player, SimulationConnection connection)
		{
			LogStream logInfo = InternalLogStreams.LogInfo;
			if (logInfo != null)
			{
				logInfo.Log(string.Format("adding player {0}", player));
			}
			Assert.Always(this._players.Add(player), "player can't exist");
			bool flag = connection != null;
			if (flag)
			{
				connection.Player = player;
				this._playersConnections.Add(player, connection);
			}
			else
			{
				Assert.Always(this.IsServer && this.Mode == SimulationModes.Host && this.LocalPlayer == player, "if no connection is given the playerref passed has to be the local player on host");
			}
		}

		internal void PlayerRemove(PlayerRef player)
		{
			Assert.Always(this._players.Remove(player), "player must exist");
			bool flag = this.IsServer && this.Mode == SimulationModes.Host && this.LocalPlayer == player;
			if (!flag)
			{
				Assert.Always(this._playersConnections.Remove(player), "player connection must exist");
			}
		}

		internal unsafe bool IsHostPlayer(PlayerRef player)
		{
			SimulationRuntimeConfig* ptr;
			return this.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out ptr) && ptr->HostPlayer == player;
		}

		public unsafe bool TryGetHostPlayer(out PlayerRef player)
		{
			SimulationRuntimeConfig* ptr;
			bool flag = this.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out ptr);
			bool result;
			if (flag)
			{
				player = ptr->HostPlayer;
				result = true;
			}
			else
			{
				player = default(PlayerRef);
				result = false;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool? IsInterestedIn(NetworkObjectMeta meta, PlayerRef player)
		{
			bool flag = !this.Config.AreaOfInterestEnabled;
			bool? result;
			if (flag)
			{
				result = new bool?(true);
			}
			else
			{
				bool isClient = this.IsClient;
				if (isClient)
				{
					bool flag2 = this.IsLocalSimulationStateAuthority(meta.Header);
					if (flag2)
					{
						result = new bool?(true);
					}
					else
					{
						bool flag3 = this.LocalPlayer != player;
						if (flag3)
						{
							bool flag4 = this.IsHostPlayer(player);
							if (flag4)
							{
								result = new bool?(true);
							}
							else
							{
								result = null;
							}
						}
						else
						{
							result = new bool?((meta.PlayerData.Flags & NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) > (NetworkObjectHeaderPlayerDataFlags)0);
						}
					}
				}
				else
				{
					bool flag5 = !this.PlayerValid(player);
					if (flag5)
					{
						result = null;
					}
					else
					{
						bool flag6 = this.IsHostPlayer(player);
						if (flag6)
						{
							result = new bool?(true);
						}
						else
						{
							SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(player);
							NetworkObjectConnectionData networkObjectConnectionData;
							bool flag7 = simulationConnectionForPlayer != null && simulationConnectionForPlayer.TryGetObjectData(meta.Id, out networkObjectConnectionData);
							if (flag7)
							{
								result = new bool?(networkObjectConnectionData.HasAnyPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.AllInterestFlags));
							}
							else
							{
								result = new bool?(false);
							}
						}
					}
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool IsInputAuthority(PlayerRef inputAuthority, PlayerRef playerRef)
		{
			bool isNone = inputAuthority.IsNone;
			bool result;
			if (isNone)
			{
				result = false;
			}
			else
			{
				bool flag = inputAuthority == playerRef;
				if (flag)
				{
					result = true;
				}
				else
				{
					bool isNone2 = playerRef.IsNone;
					if (isNone2)
					{
						PlayerRef a;
						result = (this.TryGetHostPlayer(out a) && a == inputAuthority);
					}
					else
					{
						bool isMasterClient = inputAuthority.IsMasterClient;
						SimulationRuntimeConfig* ptr;
						result = (isMasterClient && this.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out ptr) && ptr->MasterClient == playerRef);
					}
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool IsInputAuthority([NotNull] NetworkObjectMeta meta, PlayerRef playerRef)
		{
			return this.IsInputAuthority(*meta.InputAuthority, playerRef);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool IsStateAuthority(PlayerRef stateSource, PlayerRef playerRef)
		{
			bool flag = stateSource == playerRef;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool isNone = stateSource.IsNone;
				if (isNone)
				{
					PlayerRef a;
					result = (this.TryGetHostPlayer(out a) && a == playerRef);
				}
				else
				{
					bool isMasterClient = stateSource.IsMasterClient;
					SimulationRuntimeConfig* ptr;
					result = (isMasterClient && this.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out ptr) && ptr->MasterClient == playerRef);
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe bool IsStateAuthority([NotNull] NetworkObjectMeta meta, PlayerRef playerRef)
		{
			return this.IsStateAuthority(*meta.StateAuthority, playerRef);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsLocalSimulationInputAuthority([RequiresLocation] [In] ref NetworkObjectHeader obj)
		{
			return this.LocalPlayer.IsRealPlayer && obj.InputAuthority == this.LocalPlayer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsLocalSimulationStateAuthority([RequiresLocation] [In] ref NetworkObjectHeader obj)
		{
			return (this.Topology == Topologies.ClientServer) ? this.IsServer : this.IsStateAuthority(obj.StateAuthority, this.LocalPlayer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe PlayerRef GetStateAuthority(PlayerRef objectStateAuthority)
		{
			bool flag = objectStateAuthority == PlayerRef.None;
			PlayerRef result;
			if (flag)
			{
				result = PlayerRef.None;
			}
			else
			{
				bool flag2 = this.Topology == Topologies.ClientServer;
				if (flag2)
				{
					result = default(PlayerRef);
				}
				else
				{
					bool flag3 = this.Topology == Topologies.Shared;
					if (flag3)
					{
						bool isMasterClient = objectStateAuthority.IsMasterClient;
						if (isMasterClient)
						{
							SimulationRuntimeConfig* ptr;
							return this.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out ptr) ? ptr->MasterClient : default(PlayerRef);
						}
					}
					result = objectStateAuthority;
				}
			}
			return result;
		}

		internal unsafe byte[] GetPlayerConnectionToken(PlayerRef player)
		{
			bool flag = this.PlayerValid(player);
			if (flag)
			{
				SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(player);
				bool flag2 = simulationConnectionForPlayer == null;
				if (flag2)
				{
					return null;
				}
				bool flag3 = simulationConnectionForPlayer.Connection->ConnectionToken != null && simulationConnectionForPlayer.Connection->ConnectionTokenLength > 0;
				if (flag3)
				{
					byte[] array = new byte[simulationConnectionForPlayer.Connection->ConnectionTokenLength];
					byte[] array2;
					byte* destination;
					if ((array2 = array) == null || array2.Length == 0)
					{
						destination = null;
					}
					else
					{
						destination = &array2[0];
					}
					Native.MemCpy((void*)destination, (void*)simulationConnectionForPlayer.Connection->ConnectionToken, simulationConnectionForPlayer.Connection->ConnectionTokenLength);
					array2 = null;
					return array;
				}
			}
			return null;
		}

		internal unsafe NetAddress GetPlayerAddress(PlayerRef player)
		{
			bool flag = this.PlayerValid(player);
			if (flag)
			{
				SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(player);
				bool flag2 = simulationConnectionForPlayer != null;
				if (flag2)
				{
					return simulationConnectionForPlayer.Connection->Address;
				}
			}
			return default(NetAddress);
		}

		internal unsafe long GetPlayerUniqueId(PlayerRef player)
		{
			bool flag = this.PlayerValid(player);
			if (flag)
			{
				SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(player);
				bool flag2 = simulationConnectionForPlayer != null;
				if (flag2)
				{
					return simulationConnectionForPlayer.Connection->UniqueIdHash;
				}
			}
			return 0L;
		}

		public SimulationInput GetInputForPlayer(PlayerRef player)
		{
			return this._inputCollection.GetByPlayer(player);
		}

		private unsafe void DeletePlayerSimulationDataOnDisconnect(PlayerRef player)
		{
			bool isClient = this.IsClient;
			if (!isClient)
			{
				Simulation.PlayerSimulationData* playerSimulationData = this.GetPlayerSimulationData(player, false);
				bool flag = playerSimulationData != null;
				if (flag)
				{
					NetworkId id;
					bool flag2 = this._playerDataLookup.TryGetValue(player, out id);
					if (flag2)
					{
						this._playerDataLookup.Remove(player);
						this.Destroy(id, NetworkObjectDestroyFlags.DestroyState, default(PlayerRef));
					}
				}
			}
		}

		private unsafe Simulation.PlayerSimulationData* GetPlayerSimulationData(PlayerRef player, bool create)
		{
			SimulationRuntimeConfig* ptr;
			bool flag = player.IsMasterClient && this.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out ptr);
			if (flag)
			{
				player = ptr->MasterClient;
			}
			bool isNone = player.IsNone;
			Simulation.PlayerSimulationData* result;
			if (isNone)
			{
				result = null;
			}
			else
			{
				Assert.Always<PlayerRef>(player.IsRealPlayer, "invalid player {0}", player);
				NetworkId id;
				Simulation.PlayerSimulationData* ptr2;
				bool flag2 = this._playerDataLookup.TryGetValue(player, out id) && this.TryGetStructData<Simulation.PlayerSimulationData>(id, out ptr2);
				if (flag2)
				{
					result = ptr2;
				}
				else
				{
					foreach (NetworkId networkId in this._structs)
					{
						NetworkObjectMeta networkObjectMeta;
						bool flag3 = this.TryGetStruct(networkId, out networkObjectMeta) && networkObjectMeta.Type == NetworkObjectTypeId.PlayerData;
						if (flag3)
						{
							Simulation.PlayerSimulationData* dataAs = networkObjectMeta.GetDataAs<Simulation.PlayerSimulationData>();
							bool flag4 = dataAs->Player != player;
							if (!flag4)
							{
								this._playerDataLookup.Add(player, networkId);
								return dataAs;
							}
						}
					}
					Simulation.PlayerSimulationData* ptr3 = (create && this.IsServer) ? this.AllocateStruct<Simulation.PlayerSimulationData>(this.GetNextId(), 0, new NetworkObjectTypeId?(NetworkObjectTypeId.PlayerData)) : null;
					bool flag5 = ptr3 != null;
					if (flag5)
					{
						ptr3->Player = player;
						this._invokeJoinedLeaveQueue.Enqueue(new ValueTuple<PlayerRef, bool>(player, true));
					}
					result = ptr3;
				}
			}
			return result;
		}

		private void InvokePlayerJoinedLeft()
		{
			bool canReceivePlayerJoinLeaveCallbacks = this._callbacks.CanReceivePlayerJoinLeaveCallbacks;
			if (canReceivePlayerJoinLeaveCallbacks)
			{
				while (this._invokeJoinedLeaveQueue.Count > 0)
				{
					try
					{
						ValueTuple<PlayerRef, bool> valueTuple = this._invokeJoinedLeaveQueue.Dequeue();
						PlayerRef item = valueTuple.Item1;
						bool item2 = valueTuple.Item2;
						bool flag = item2;
						if (flag)
						{
							DebugLogStream logDebug = InternalLogStreams.LogDebug;
							if (logDebug != null)
							{
								logDebug.Log(this, string.Format("Player Joined: {0}", item));
							}
							this._callbacks.PlayerJoined(item);
							bool flag2 = this.IsServer && this.Config.SchedulingWithoutAOI && item != this.LocalPlayer;
							if (flag2)
							{
								foreach (NetworkObjectMeta networkObjectMeta in this._metaLookup.Values)
								{
									bool flag3 = !networkObjectMeta.IsStruct;
									if (flag3)
									{
										SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(item);
										if (simulationConnectionForPlayer != null)
										{
											simulationConnectionForPlayer.GetObjectData(networkObjectMeta.Id, true, false);
										}
									}
								}
							}
						}
						else
						{
							DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
							if (logDebug2 != null)
							{
								logDebug2.Log(this, string.Format("Player Left: {0}", item));
							}
							this._callbacks.PlayerLeft(item);
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
			}
		}

		internal unsafe int? GetPlayerActorId(PlayerRef player)
		{
			Simulation.PlayerSimulationData* playerSimulationData = this.GetPlayerSimulationData(player, true);
			return new int?((playerSimulationData != null) ? playerSimulationData->Actor : 0);
		}

		internal unsafe NetworkId GetPlayerObjectId(PlayerRef player)
		{
			Simulation.PlayerSimulationData* playerSimulationData = this.GetPlayerSimulationData(player, false);
			bool flag = playerSimulationData == null;
			NetworkId result;
			if (flag)
			{
				NetworkId networkId;
				result = (this._playerLeftTempObjectCache.TryGetValue(player, out networkId) ? networkId : default(NetworkId));
			}
			else
			{
				result = playerSimulationData->Object;
			}
			return result;
		}

		internal unsafe void SetPlayerObjectId(PlayerRef player, NetworkId id)
		{
			bool flag = this.Topology == Topologies.ClientServer;
			if (flag)
			{
				bool isClient = this.IsClient;
				if (!isClient)
				{
					bool flag2 = this.PlayerValid(player);
					if (flag2)
					{
						this.GetPlayerSimulationData(player, true)->Object = id;
					}
				}
			}
			else
			{
				bool isClient2 = this.IsClient;
				if (isClient2)
				{
					NetworkObjectMeta networkObjectMeta;
					bool flag3 = this.TryGetMeta(id, out networkObjectMeta) && this.IsStateAuthority(*networkObjectMeta.StateAuthority, this.LocalPlayer) && player == this.LocalPlayer;
					if (flag3)
					{
						SimulationMessageInternal_SetPlayerObject buffer;
						buffer.Object = id;
						this.SendInternalSimulationMessage<SimulationMessageInternal_SetPlayerObject>(SimulationMessageInternalTypes.SetPlayerObject, buffer, null);
						this.GetPlayerSimulationData(player, true)->Object = id;
					}
				}
				else
				{
					bool flag4 = this.PlayerValid(player);
					if (flag4)
					{
						this.GetPlayerSimulationData(player, true)->Object = id;
					}
				}
			}
		}

		public unsafe bool HasAnyActiveConnections()
		{
			NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(this._netPeerGroup);
			while (iterator.Next())
			{
				bool flag = iterator.Current->ConnectionStatus != NetConnectionStatus.Connected;
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}

		private void InvokeOnBeforeSimulation(int forwardTickCount)
		{
			EngineProfiler.Begin("InvokeOnBeforeSimulation");
			try
			{
				this._callbacks.OnBeforeSimulation(forwardTickCount);
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			EngineProfiler.End();
		}

		private void InvokeOnAfterSimulation()
		{
			EngineProfiler.Begin("InvokeOnAfterSimulation");
			try
			{
				this._callbacks.OnAfterSimulation();
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			EngineProfiler.End();
		}

		private void InvokeOnBeforeAllTicks(bool resimulation, int ticks)
		{
			EngineProfiler.Begin("InvokeOnBeforeAllTicks");
			try
			{
				this._isResimulation = resimulation;
				this._callbacks.OnBeforeAllTicks(resimulation, ticks);
				this._isResimulation = false;
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			EngineProfiler.End();
		}

		private void InvokeOnAfterAllTicks(bool resimulation, int ticks)
		{
			EngineProfiler.Begin("InvokeOnAfterAllTicks");
			try
			{
				this._isResimulation = resimulation;
				this._callbacks.OnAfterAllTicks(resimulation, ticks);
				this._isResimulation = false;
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
			EngineProfiler.End();
		}

		protected virtual void BeforeUpdate()
		{
		}

		protected virtual void AfterSimulation()
		{
		}

		private void UpdateSimulationStateForMasterClientObjects(bool isMasterClient)
		{
			SimulationConnection simulationConnectionForPlayer = this.GetSimulationConnectionForPlayer(this.LocalPlayer);
			bool flag = simulationConnectionForPlayer == null;
			if (!flag)
			{
				HashSet<NetworkId> hashSet = new HashSet<NetworkId>();
				foreach (KeyValuePair<NetworkId, NetworkObjectMeta> keyValuePair in this._metaLookup)
				{
					bool flag2 = keyValuePair.Key.IsReserved || (keyValuePair.Value.Instance && (keyValuePair.Value.Instance.Flags & NetworkObjectFlags.MasterClientObject) != NetworkObjectFlags.MasterClientObject);
					if (!flag2)
					{
						NetworkObjectConnectionData objectData = simulationConnectionForPlayer.GetObjectData(keyValuePair.Key, true, false);
						NetworkObjectMeta value = keyValuePair.Value;
						value.SnapshotLatest.CopyTo(value.Shadow);
						value.SnapshotLatest.CopyTo(value.Previous);
						value.SnapshotLatest.CopyTo(value);
						simulationConnectionForPlayer.SetActive(objectData, keyValuePair.Value);
						hashSet.Add(keyValuePair.Key);
					}
				}
				foreach (NetworkId id in hashSet)
				{
					this._callbacks.ObjectIsSimulatedChanged(id, isMasterClient);
					this._callbacks.ObjectStateAuthorityChanged(id, isMasterClient);
				}
			}
		}

		private int CalculateForwardTicks()
		{
			bool flag = this.HasRuntimeConfig && this._time.IsRunning();
			int result;
			if (flag)
			{
				int num = (int)(this._time.Now().Local * (double)this.TickRate);
				num = Math.Min(num, this.LatestServerTick + this.TickRate);
				int val = (num - this._tick) / this.TickStride;
				result = Math.Min(Math.Max(val, 0), this.TickRate);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public int Update(double dt)
		{
			bool flag = this._isShutdown || dt == 0.0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				EngineProfiler.Begin("Simulation.Update");
				this.BeforeUpdate();
				this.NetworkRecv();
				bool flag2 = this.HasRuntimeConfig && this._time.IsRunning();
				if (flag2)
				{
					this._time.Update(dt);
					bool flag3 = !this.Runner.OnGameStartedInvoked;
					if (flag3)
					{
						this.Runner.OnRuntimeConfigReady();
					}
					bool isServer = this.IsServer;
					if (isServer)
					{
						this.CalculateUpdateTime();
					}
				}
				int num = this.CalculateForwardTicks();
				bool flag4 = !this._isWaitingForShutdown && num > 0;
				if (flag4)
				{
					this.InvokeOnBeforeSimulation(num);
					EngineProfiler.Begin("BeforeSimulation");
					int value = this.BeforeSimulation();
					EngineProfiler.End();
					this._fusionStatsManager.PendingSnapshot.AddToResimulationStat(value, false);
					try
					{
						this.InvokeOnBeforeAllTicks(false, num);
						for (int i = 0; i < num; i++)
						{
							this.StepSimulation(SimulationStages.Forward, i == num - 1, i == 0, this.IsServer);
						}
						this.InvokeOnAfterAllTicks(false, num);
					}
					catch (Exception error)
					{
						LogStream logException = InternalLogStreams.LogException;
						if (logException != null)
						{
							logException.Log(this, error);
						}
					}
					this.InvokeOnAfterSimulation();
					EngineProfiler.Begin("AfterSimulation");
					this.AfterSimulation();
					EngineProfiler.End();
					this._fusionStatsManager.PendingSnapshot.AddToForwardTicksStat(num, false);
					try
					{
						this.PreparePackets();
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
				else
				{
					this.NoSimulation();
				}
				this.NetworkSend();
				Assert.Check<SimulationStages>(this._stage == (SimulationStages)0, "Invalid Simulation.Stage {0}", this._stage);
				EngineProfiler.End();
				this.AfterUpdate();
				bool flag5 = this.HasRuntimeConfig && this._time.IsRunning();
				if (flag5)
				{
					bool isPlayer = this.IsPlayer;
					if (isPlayer)
					{
						this._localAlphaPrev = this._localAlpha;
						this._localAlpha = (float)Maths.Clamp01((this._time.Now().Local - (double)this._tick * this.TickDeltaDouble) * (double)this.TickRate);
						bool flag6 = !this.IsClient;
						if (flag6)
						{
							this._remoteAlpha = this._localAlpha;
						}
						bool isClient = this.IsClient;
						if (isClient)
						{
							this._time.Log(this._fusionStatsManager);
						}
					}
				}
				result = num;
			}
			return result;
		}

		private void UpdateAreaOfInterest()
		{
			bool flag = !this.IsServer;
			if (!flag)
			{
				bool flag2 = (this.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) != NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
				if (!flag2)
				{
					EngineProfiler.Begin("UpdateAreaOfInterest");
					foreach (NetworkObjectMeta networkObjectMeta in this._metaLookup.Values)
					{
						bool flag3 = (networkObjectMeta.Flags & NetworkObjectHeaderFlags.AreaOfInterest) != NetworkObjectHeaderFlags.AreaOfInterest;
						if (!flag3)
						{
							Assert.Check(networkObjectMeta.HasMainTRSP, networkObjectMeta.Instance.gameObject.name);
							bool flag4 = !networkObjectMeta.HasMainTRSP;
							if (!flag4)
							{
								Vector3? vector = this.<UpdateAreaOfInterest>g__ResolveCellPosition|228_0(networkObjectMeta);
								bool flag5 = vector != null;
								if (flag5)
								{
									this.AOI_UpdateAreaOfInterest(networkObjectMeta, Simulation.AreaOfInterest.ToCell(vector.Value));
								}
								else
								{
									DebugLogStream logDebug = InternalLogStreams.LogDebug;
									if (logDebug != null)
									{
										logDebug.Error(string.Format("could not resolve aoi position for {0}", networkObjectMeta.Id));
									}
								}
							}
						}
					}
					EngineProfiler.End();
				}
			}
		}

		private unsafe void PreparePackets()
		{
			int num = this._tick.Raw - this._sendTick.Raw;
			bool flag = num < this.TickRate / this.SendRate;
			if (!flag)
			{
				this._sendTick = this._tick;
				bool flag2 = (this.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
				if (flag2)
				{
					this.UpdateAreaOfInterest();
				}
				EngineProfiler.Begin("SendPackets");
				this._stateReplicator.UpdateChangedStructSet();
				NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(this._netPeerGroup);
				while (iterator.Next())
				{
					NetConnection* c = iterator.Current;
					SimulationConnection simulationConnection = this.GetSimulationConnection(c);
					bool flag3 = simulationConnection == null;
					if (!flag3)
					{
						double connectionIdleTime = NetPeerGroup.GetConnectionIdleTime(this._netPeerGroup, c);
						bool flag4 = connectionIdleTime >= 1.0 && this._netPeerGroup->Time - simulationConnection.LastSend < 0.5;
						if (!flag4)
						{
							bool flag5 = this._sendContext.Init(simulationConnection, this._tick);
							if (flag5)
							{
								this.WriteMessages();
								this.WritePackets();
								this._fusionStatsManager.PendingSnapshot.AddToOutPacketsStat(1, false);
								this._fusionStatsManager.PendingSnapshot.AddToOutBandwidthStat((float)Maths.BytesRequiredForBits(this._sendContext.Buffer->OffsetBits), false);
								this._sendContext.Send();
								simulationConnection.LastSend = this._netPeerGroup->Time;
							}
						}
					}
				}
				EngineProfiler.End();
			}
		}

		private unsafe void WriteMessages()
		{
			NetBitBuffer.Offset offset = NetBitBuffer.GetOffset(this._sendContext.Buffer);
			bool flag = this._sendContext.Connection.MessagesOut.Count > 0;
			int num = 9088 - this._sendContext.Buffer->OffsetBits;
			bool flag2 = num > 0;
			if (flag2)
			{
				this.ConsumeAndWriteMessagesIntoBuffer(ref this._sendContext.Connection.MessagesOut, this._sendContext.Buffer, num, ref this._sendContext.Envelope->Messages, true);
				bool flag3 = flag;
				if (flag3)
				{
					bool flag4 = this._sendContext.Header.SimulationMessages == 0;
					if (flag4)
					{
						SimulationMessageEnvelope* head = this._sendContext.Connection.MessagesOut.Head;
						Assert.Always<int>(head->Message->Offset > 0, "Message offset invalid {0}", head->Message->Offset);
						Assert.Always(!head->Message->GetFlag(256), "Message has FLAG_DUMMY");
						LogStream logError = InternalLogStreams.LogError;
						if (logError != null)
						{
							logError.Log(this, string.Format("Message {0} (sequence: {1}) is too large to be serialized and will be discarded", *head->Message, head->Sequence));
						}
						head->Message->SetDummy();
					}
					else
					{
						TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
						if (logTraceSimulationMessage != null)
						{
							logTraceSimulationMessage.Log(this, string.Format("Consumed {0} messages, remaining: {1}, bit capacity: {2}, bit left: {3}", new object[]
							{
								this._sendContext.Header.SimulationMessages,
								this._sendContext.Connection.MessagesOut.Count,
								num,
								9088 - this._sendContext.Buffer->OffsetBits
							}));
						}
					}
				}
			}
			else
			{
				bool flag5 = flag;
				if (flag5)
				{
					TraceLogStream logTraceSimulationMessage2 = InternalLogStreams.LogTraceSimulationMessage;
					if (logTraceSimulationMessage2 != null)
					{
						logTraceSimulationMessage2.Log(this, string.Format("No space to consume messages after snapshot serialization ({0}).", num));
					}
				}
			}
		}

		private void InvokeTick(SimulationStages stage, bool releaseAllInputs)
		{
			try
			{
				Assert.Check(this._inputCollection.Count == 0, "InputCollection Size should be 0");
				this._stage = stage;
				this.InterpolateSequenceIncrement();
				foreach (PlayerRef player in this._players)
				{
					SimulationInput input = this.GetInput(this._tick, player);
					bool flag = input != null;
					if (flag)
					{
						this._inputCollection.AddInput(input);
					}
				}
				bool flag2 = this.IsClient && this.IsFirstTick && (this.IsResimulation || this.Config.Topology == Topologies.Shared);
				if (flag2)
				{
					SimulationStages stage2 = this._stage;
					try
					{
						this._stage = SimulationStages.Forward;
						this._callbacks.UpdateRemotePrefabs();
					}
					finally
					{
						this._stage = stage2;
					}
				}
				bool isInitialLocalTick = this._isInitialLocalTick;
				if (isInitialLocalTick)
				{
					this._isInitialLocalTick = false;
					this.BeforeFirstTick();
				}
				EngineProfiler.Begin("Simulation.BeforeTick");
				this._callbacks.OnBeforeTick();
				EngineProfiler.End();
				this.DeliverMessages(this._tick);
				try
				{
					this._isInTick = true;
					this.InvokePlayerJoinedLeft();
					this._playerLeftTempObjectCache.Clear();
					this._callbacks.OnTick();
				}
				catch (Exception error)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(this, "OnTick Threw Exception");
					}
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(this, error);
					}
				}
				finally
				{
					this._isInTick = false;
				}
				EngineProfiler.Begin("Simulation.AfterTick");
				this._callbacks.OnAfterTick();
				EngineProfiler.End();
			}
			catch (Exception error2)
			{
				LogStream logException2 = InternalLogStreams.LogException;
				if (logException2 != null)
				{
					logException2.Log(this, error2);
				}
			}
			finally
			{
				this._stage = (SimulationStages)0;
				try
				{
					if (releaseAllInputs)
					{
						for (int i = 0; i < this._inputCollection.Count; i++)
						{
							this._inputPool.Release(this._inputCollection.GetByIndex(i));
						}
					}
				}
				finally
				{
					this._inputCollection.Clear();
				}
			}
		}

		private unsafe static ref SimulationMessageInternalTypes GetMessageInternalType(SimulationMessage* message)
		{
			Assert.Check(true, "SimulationMessageInternalTypes size should be 4");
			Span<byte> rawData = SimulationMessage.GetRawData(message);
			return rawData.AsRef<SimulationMessageInternalTypes>();
		}

		private unsafe static ref T GetMessageInternalData<[IsUnmanaged] T>(SimulationMessage* message) where T : struct, ValueType
		{
			Assert.Check(true, "SimulationMessageInternalTypes size should be 4");
			Span<byte> rawData = SimulationMessage.GetRawData(message);
			ref Span<byte> ptr = ref rawData;
			return ptr.Slice(4, ptr.Length - 4).AsRef<T>();
		}

		private unsafe void OnMessageInternal(SimulationMessage* message)
		{
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(string.Format("OnMessageInternal({0})", *Simulation.GetMessageInternalType(message)));
			}
		}

		private unsafe void DeliverMessages(int tick)
		{
			EngineProfiler.Begin("Simulation.DeliverMessages");
			NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(this._netPeerGroup);
			while (iterator.Next())
			{
				bool flag = iterator.Current->ConnectionStatus != NetConnectionStatus.Connected;
				if (!flag)
				{
					SimulationConnection simulationConnection;
					bool flag2 = !this.TryGetSimulationConnectionLogErrorIfFailed(iterator.Current, out simulationConnection);
					if (!flag2)
					{
						SimulationMessageList simulationMessageList = default(SimulationMessageList);
						try
						{
							while (simulationConnection.MessagesIn.Count > 0)
							{
								SimulationMessageEnvelope* ptr = simulationConnection.MessagesIn.Head;
								bool flag3 = ptr->Message->Tick > tick;
								if (flag3)
								{
									bool flag4 = ptr->Message->GetFlag(128);
									if (!flag4)
									{
										TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
										if (logTraceSimulationMessage != null)
										{
											logTraceSimulationMessage.Log(this, string.Format("Not handling RPC: tick {0} > {1} (player: {2}) {3}", new object[]
											{
												ptr->Message->Tick,
												tick,
												this.LocalPlayer,
												LogUtils.GetDump<SimulationMessageEnvelope>(ptr)
											}));
										}
										break;
									}
									TraceLogStream logTraceSimulationMessage2 = InternalLogStreams.LogTraceSimulationMessage;
									if (logTraceSimulationMessage2 != null)
									{
										logTraceSimulationMessage2.Log(this, string.Format("Handling RPC ahead of time due to its flags (tick: {0}, player: {1}) {2}", tick, this.LocalPlayer, LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
									}
								}
								bool flag5 = ptr->Message->GetFlag(8);
								if (flag5)
								{
									Assert.Check(ptr->Sequence == 0UL, "Head Sequence must be 0");
								}
								else
								{
									bool flag6 = ptr->Sequence == simulationConnection.MessagesInSequence + 1UL;
									if (!flag6)
									{
										TraceLogStream logTraceSimulationMessage3 = InternalLogStreams.LogTraceSimulationMessage;
										if (logTraceSimulationMessage3 != null)
										{
											logTraceSimulationMessage3.Log(this, string.Format("Not handling RPC: sequence {0} != {1} (player: {2}) {3}", new object[]
											{
												ptr->Sequence,
												simulationConnection.MessagesInSequence + 1UL,
												this.LocalPlayer,
												LogUtils.GetDump<SimulationMessageEnvelope>(ptr)
											}));
										}
										break;
									}
									simulationConnection.MessagesInSequence += 1UL;
								}
								simulationConnection.MessagesIn.Remove(ptr);
								int count = simulationConnection.MessagesIn.Count;
								try
								{
									bool flag7 = ptr->Message->GetFlag(64);
									if (flag7)
									{
										this.OnMessageInternal(ptr->Message);
									}
									else
									{
										SimulationMessageResult simulationMessageResult = this._callbacks.OnMessage(ptr->Message);
										bool flag8 = simulationMessageResult == SimulationMessageResult.Retry;
										if (flag8)
										{
											bool flag9 = ptr->Message->GetFlag(8);
											if (!flag9)
											{
												TraceLogStream logTraceSimulationMessage4 = InternalLogStreams.LogTraceSimulationMessage;
												if (logTraceSimulationMessage4 != null)
												{
													logTraceSimulationMessage4.Log(this, string.Format("Reliable RPC {0} will be retried (player: {1}) {2}", ptr->Sequence, this.LocalPlayer, LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
												}
												simulationConnection.MessagesIn.AddFirst(ptr);
												simulationConnection.MessagesInSequence -= 1UL;
												ptr = null;
												break;
											}
											TraceLogStream logTraceSimulationMessage5 = InternalLogStreams.LogTraceSimulationMessage;
											if (logTraceSimulationMessage5 != null)
											{
												logTraceSimulationMessage5.Log(this, string.Format("Unreliable will be retried (player: {0}) {1}", this.LocalPlayer, LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
											}
											simulationMessageList.AddLast(ptr);
											ptr = null;
										}
									}
								}
								finally
								{
									bool flag10 = ptr != null;
									if (flag10)
									{
										Assert.Check(count == simulationConnection.MessagesIn.Count);
										SimulationMessageEnvelope.Free(this, ref ptr);
									}
								}
							}
						}
						finally
						{
							bool flag11 = simulationMessageList.Count > 0;
							if (flag11)
							{
								TraceLogStream logTraceSimulationMessage6 = InternalLogStreams.LogTraceSimulationMessage;
								if (logTraceSimulationMessage6 != null)
								{
									logTraceSimulationMessage6.Log(this, string.Format("Requeuing {0} messages for retry (player: {1})", simulationMessageList.Count, this.LocalPlayer));
								}
								simulationMessageList.Concat(simulationConnection.MessagesIn);
								simulationConnection.MessagesIn = simulationMessageList;
							}
						}
					}
				}
			}
			EngineProfiler.End();
		}

		internal unsafe void FreeMessages(ref SimulationMessageList list)
		{
			while (list.Count > 0)
			{
				SimulationMessageEnvelope* ptr = list.RemoveHead();
				SimulationMessageEnvelope.Free(this, ref ptr);
			}
			list = default(SimulationMessageList);
		}

		private unsafe void ConsumeAndWriteMessagesIntoBuffer(ref SimulationMessageList inList, NetBitBuffer* buffer, int bitCapacity, ref SimulationMessageList outList, bool allowFirstMessageOverflow = true)
		{
			int num = buffer->OffsetBits + bitCapacity;
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(string.Format("{0} Messages-Start", LogUtils.GetDump<NetBitBuffer>(buffer)));
			}
			buffer->PadToByteBoundary();
			bool flag = allowFirstMessageOverflow;
			bool isServer = this.IsServer;
			while (inList.Count > 0)
			{
				SimulationMessageEnvelope* head = inList.Head;
				int offsetBits = buffer->OffsetBits;
				PlayerRef? playerRef = null;
				bool flag2 = isServer && head->Message->IsTargeted();
				if (flag2)
				{
					playerRef = new PlayerRef?(head->Message->Target);
					head->Message->Target = default(PlayerRef);
				}
				TraceLogStream logTraceNetwork2 = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork2 != null)
				{
					logTraceNetwork2.Log(string.Format("{0} Message", LogUtils.GetDump<NetBitBuffer>(buffer)));
				}
				try
				{
					int bitCount = SimulationMessageEnvelope.GetBitCount(head, buffer);
					bool flag3 = !buffer->CheckBitCount(bitCount);
					if (flag3)
					{
						TraceLogStream logTraceNetwork3 = InternalLogStreams.LogTraceNetwork;
						if (logTraceNetwork3 != null)
						{
							logTraceNetwork3.Log(string.Format("{0} Message-Sequence:{1} would overflow by {2} bits", LogUtils.GetDump<NetBitBuffer>(buffer), head->Sequence, bitCount - (buffer->LengthBits - buffer->OffsetBits)));
						}
						break;
					}
					SimulationMessageEnvelope.Write(head, buffer);
					TraceLogStream logTraceNetwork4 = InternalLogStreams.LogTraceNetwork;
					if (logTraceNetwork4 != null)
					{
						logTraceNetwork4.Log(string.Format("{0} Message-Sequence:{1}", LogUtils.GetDump<NetBitBuffer>(buffer), head->Sequence));
					}
					bool flag4 = buffer->OffsetBits >= num;
					if (flag4)
					{
						Assert.Check(!buffer->Overflow, "Buffer should not overflow");
						bool flag5 = !flag;
						if (flag5)
						{
							buffer->OffsetBits = offsetBits;
							break;
						}
					}
				}
				finally
				{
					bool flag6 = playerRef != null;
					if (flag6)
					{
						head->Message->Target = playerRef.Value;
					}
				}
				flag = false;
				Simulation.SendContext sendContext = this._sendContext;
				sendContext.Header.SimulationMessages = sendContext.Header.SimulationMessages + 1;
				SimulationMessageEnvelope* ptr = inList.RemoveHead();
				Assert.Check(ptr == head, "SimulationMessageList Head != Msg Head");
				bool flag7 = head->Message->GetFlag(8);
				if (flag7)
				{
					SimulationMessageEnvelope.Free(this, ref head);
				}
				else
				{
					outList.AddLast(head);
				}
			}
			EngineProfiler.RpcOut((int)this._sendContext.Header.SimulationMessages);
			buffer->PadToByteBoundary();
		}

		private unsafe void ResolveMessageSourceAndTarget(SimulationMessage* msg, PlayerRef sourcePlayer)
		{
			bool isServer = this.IsServer;
			if (isServer)
			{
				Assert.Check(msg->Source.IsNone, "Messages arriving to server should not have Source set");
				msg->Source = sourcePlayer;
				bool flag = msg->GetFlag(32);
				if (flag)
				{
					Assert.Check(msg->Target.IsNone, "Messages to the server should not have target set");
				}
				else
				{
					bool flag2 = msg->GetFlag(16);
					if (flag2)
					{
						Assert.Check(msg->Target.IsRealPlayer, "Messages to a player should have target set");
					}
					else
					{
						Assert.Check(msg->Target.IsNone, "Messages without a target should not have target set");
					}
				}
			}
			else
			{
				Assert.Check(!msg->GetFlag(32), "Got forwarded to a client? With server?");
				Assert.Check(msg->Target.IsNone, "If a message reaches a client, it should have it's target set");
				bool flag3 = msg->GetFlag(16);
				if (flag3)
				{
					msg->Target = this.LocalPlayer;
				}
			}
		}

		private unsafe void RecvMessages()
		{
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(string.Format("{0} Messages-Start", LogUtils.GetDump<NetBitBuffer>(this._recvContext.Buffer)));
			}
			NetBitBuffer.Offset offset = NetBitBuffer.GetOffset(this._recvContext.Buffer);
			this._recvContext.Buffer->SeekToByteBoundary();
			Assert.Check(!this._recvContext.Buffer->Overflow, "Buffer should not overflow");
			int num = (int)this._recvContext.Header.SimulationMessages;
			EngineProfiler.RpcIn(num);
			bool flag = this._recvContext.Header.SimulationMessages > 0;
			if (flag)
			{
				TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
				if (logTraceSimulationMessage != null)
				{
					logTraceSimulationMessage.Log(string.Format("ReadMessagesFromBuffer: {0} messages", this._recvContext.Header.SimulationMessages));
				}
			}
			while (--num >= 0)
			{
				TraceLogStream logTraceNetwork2 = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork2 != null)
				{
					logTraceNetwork2.Log(string.Format("{0} Message", LogUtils.GetDump<NetBitBuffer>(this._recvContext.Buffer)));
				}
				SimulationMessageEnvelope* ptr = SimulationMessageEnvelope.Read(this, this._recvContext.Buffer);
				Assert.Always(!this._recvContext.Buffer->Overflow, "_recvContext.Buffer->Overflow == false");
				this.ResolveMessageSourceAndTarget(ptr->Message, this._recvContext.Player);
				TraceLogStream logTraceNetwork3 = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork3 != null)
				{
					logTraceNetwork3.Log(string.Format("{0} Message-Sequence:{1}", LogUtils.GetDump<NetBitBuffer>(this._recvContext.Buffer), ptr->Sequence));
				}
				bool isUnreliable = ptr->Message->IsUnreliable;
				if (isUnreliable)
				{
					TraceLogStream logTraceSimulationMessage2 = InternalLogStreams.LogTraceSimulationMessage;
					if (logTraceSimulationMessage2 != null)
					{
						logTraceSimulationMessage2.Log(this, string.Format("Enqueuing (unreliable) {0}", LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
					}
					this._recvContext.Connection.MessagesIn.AddLast(ptr);
				}
				else
				{
					bool flag2 = ptr->Sequence <= this._recvContext.Connection.MessagesInSequence;
					if (flag2)
					{
						TraceLogStream logTraceSimulationMessage3 = InternalLogStreams.LogTraceSimulationMessage;
						if (logTraceSimulationMessage3 != null)
						{
							logTraceSimulationMessage3.Log(this, string.Format("Dropping (fast, min number: {0}) {1}", this._recvContext.Connection.MessagesInSequence, LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
						}
						SimulationMessageEnvelope.Free(this, ref ptr);
					}
					else
					{
						SimulationMessageEnvelope* ptr2;
						bool flag3 = Simulation.<RecvMessages>g__CanAppendQueue|239_0(this._recvContext.Connection.MessagesIn, ptr, out ptr2);
						if (flag3)
						{
							TraceLogStream logTraceSimulationMessage4 = InternalLogStreams.LogTraceSimulationMessage;
							if (logTraceSimulationMessage4 != null)
							{
								logTraceSimulationMessage4.Log(this, string.Format("Enqueuing (fast) {0}", LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
							}
							this._recvContext.Connection.MessagesIn.AddLast(ptr);
						}
						else
						{
							bool flag4 = ptr2 != null;
							if (flag4)
							{
								TraceLogStream logTraceSimulationMessage5 = InternalLogStreams.LogTraceSimulationMessage;
								if (logTraceSimulationMessage5 != null)
								{
									logTraceSimulationMessage5.Log(this, string.Format("Enqueuing (slow, before {0}) {1}", ptr2->Sequence, LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
								}
								this._recvContext.Connection.MessagesIn.AddBefore(ptr, ptr2);
							}
							else
							{
								TraceLogStream logTraceSimulationMessage6 = InternalLogStreams.LogTraceSimulationMessage;
								if (logTraceSimulationMessage6 != null)
								{
									logTraceSimulationMessage6.Log(this, string.Format("Dropping (slow, already a message with this number) {0}", LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
								}
								SimulationMessageEnvelope.Free(this, ref ptr);
							}
						}
					}
				}
			}
			this._recvContext.Buffer->SeekToByteBoundary();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SimulationConnection GetSimulationConnectionByIndex(int index)
		{
			SimulationConnection simulationConnection;
			return this._connections.TryGetValue(index, out simulationConnection) ? simulationConnection : null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal SimulationConnection GetSimulationConnectionForPlayer(PlayerRef player)
		{
			SimulationConnection simulationConnection;
			bool flag = this._playersConnections.TryGetValue(player, out simulationConnection);
			SimulationConnection result;
			if (flag)
			{
				result = simulationConnection;
			}
			else
			{
				result = null;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool TryGetSimulationConnectionForPlayer(PlayerRef player, out SimulationConnection sc)
		{
			return this._playersConnections.TryGetValue(player, out sc);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int? GetConnectionIndexForPlayer(PlayerRef player)
		{
			SimulationConnection simulationConnection;
			bool flag = this._playersConnections.TryGetValue(player, out simulationConnection);
			int? result;
			if (flag)
			{
				result = new int?(simulationConnection.ConnectionIndex);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private unsafe bool TryGetSimulationConnectionLogErrorIfFailed(NetConnection* c, out SimulationConnection result)
		{
			SimulationConnection simulationConnection = this._connections[(int)c->LocalConnectionId.GroupIndex];
			bool flag = simulationConnection.ConnectionId == c->LocalConnectionId;
			bool result2;
			if (flag)
			{
				bool flag2 = simulationConnection.Connection != c;
				if (flag2)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("SimulationConnection.Connection != NetConnection for {0}", c->LocalConnectionId));
					}
				}
				result = simulationConnection;
				result2 = true;
			}
			else
			{
				LogStream logError2 = InternalLogStreams.LogError;
				if (logError2 != null)
				{
					logError2.Log(string.Format("Failed getting SimulationConnection for {0}", c->LocalConnectionId));
				}
				result = null;
				result2 = false;
			}
			return result2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe SimulationConnection GetSimulationConnection(NetConnection* c)
		{
			SimulationConnection simulationConnection = this._connections[(int)c->LocalConnectionId.GroupIndex];
			bool flag = simulationConnection.ConnectionId == c->LocalConnectionId;
			SimulationConnection result;
			if (flag)
			{
				Assert.Check(simulationConnection.Connection == c, "SimulationConnection.Connection != NetConnection");
				result = simulationConnection;
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal void AddToGlobalObjectInterest(NetworkObjectMeta meta)
		{
			bool flag = this._globalInterestObjects != null;
			if (flag)
			{
				Assert.Check((this.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement);
				this._globalInterestObjects.Add(meta.Id);
				foreach (SimulationConnection simulationConnection in this._connections.Values)
				{
					simulationConnection.GetObjectData(meta.Id, true, false).SetPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.ForceInterest, this);
				}
			}
		}

		internal void RemoveFromGlobalObjectInterest(NetworkId id)
		{
			bool flag = this._globalInterestObjects != null;
			if (flag)
			{
				Assert.Check((this.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement);
				this._globalInterestObjects.Remove(id);
				foreach (SimulationConnection simulationConnection in this._connections.Values)
				{
					NetworkObjectConnectionData objectData = simulationConnection.GetObjectData(id, false, false);
					bool flag2 = objectData != null;
					if (flag2)
					{
						objectData.ClearPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.ForceInterest, this);
					}
				}
			}
		}

		internal unsafe void SendReliableData(int connection, int target, ReliableKey key, byte[] data)
		{
			SimulationConnection simulationConnectionByIndex = this.GetSimulationConnectionByIndex(connection);
			ReliableId rid = default(ReliableId);
			rid.Key = key;
			rid.Target = target;
			rid.Source = this.LocalPlayer.AsIndex;
			int num = this._reliableSend + 1;
			this._reliableSend = num;
			rid.SourceSend = num;
			fixed (byte[] array = data)
			{
				byte* data2;
				if (data == null || array.Length == 0)
				{
					data2 = null;
				}
				else
				{
					data2 = &array[0];
				}
				NetPeerGroup.SendReliable(this._netPeerGroup, simulationConnectionByIndex.Connection, rid, data2, data.Length);
			}
		}

		internal void NotifyWaitingForShutdown()
		{
			this._isWaitingForShutdown = true;
		}

		Object ILogSource.GetUnityObject()
		{
			return this.Runner;
		}

		[Conditional("DEBUG")]
		internal void DumpObject(NetworkId id, StringBuilder sb)
		{
			bool isReserved = id.IsReserved;
			NetworkObjectMeta meta;
			if (isReserved)
			{
				bool flag = !this.TryGetStruct(id, out meta);
				if (flag)
				{
					return;
				}
			}
			else
			{
				bool flag2 = !this.TryGetMeta(id, out meta);
				if (flag2)
				{
					return;
				}
			}
			this.DumpObject(meta, sb);
		}

		[Conditional("DEBUG")]
		internal void DumpObject(NetworkObjectMeta meta, StringBuilder sb)
		{
			bool flag = meta == null;
			if (flag)
			{
				sb.AppendLine("null");
			}
			else
			{
				sb.AppendLine(meta.Header.ToString());
				sb.Append(BinUtils.WordsToHex(meta.Data, 4, "\n", " "));
			}
		}

		internal string DumpObject(NetworkId id)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.DumpObject(id, stringBuilder);
			return stringBuilder.ToString();
		}

		internal string DumpObject(NetworkObjectMeta meta)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.DumpObject(meta, stringBuilder);
			return stringBuilder.ToString();
		}

		internal unsafe int ReliableDataSendRate
		{
			get
			{
				return (this._netPeerGroup != null && this._netPeerGroup->ReliableSendInterval != 0.0) ? ((int)(1.0 / this._netPeerGroup->ReliableSendInterval)) : 0;
			}
			set
			{
				bool flag = this._netPeerGroup == null;
				if (!flag)
				{
					int sendRate = this.SendRate;
					bool flag2 = value < 1;
					if (flag2)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Warn(this, string.Format("Reliable Data Send Rate of {0}hz is too low, setting to {1}hz", value, 1));
						}
						value = 1;
					}
					bool flag3 = value > sendRate;
					if (flag3)
					{
						DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
						if (logDebug2 != null)
						{
							logDebug2.Warn(this, string.Format("Reliable Data Send Rate of {0}hz is too high, setting to {1}hz", value, sendRate));
						}
						value = sendRate;
					}
					this._netPeerGroup->ReliableSendInterval = (double)(1f / (float)value);
					DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
					if (logDebug3 != null)
					{
						logDebug3.Log(this, string.Format("Reliable Data Send Rate set to {0}hz", value));
					}
				}
			}
		}

		private void NetworkInit(INetSocket socket, NetAddress address)
		{
			NetConfig config = this._projectConfig.Network.ToNetConfig(address);
			config.Simulation = this._projectConfig.NetworkConditions.Create();
			config.PacketSize = 8192;
			config.ConnectionGroups = 1;
			bool isSinglePlayer = this.IsSinglePlayer;
			if (isSinglePlayer)
			{
				config.MaxConnections = 0;
			}
			else
			{
				bool isClient = this.IsClient;
				if (isClient)
				{
					config.MaxConnections = 1;
				}
				else
				{
					Assert.Check(this.IsServer);
					bool isPlayer = this.IsPlayer;
					if (isPlayer)
					{
						config.MaxConnections = this._config.PlayerCount - 1;
					}
					else
					{
						config.MaxConnections = this._config.PlayerCount;
					}
				}
			}
			this._netSocket = socket;
			this._netPeer = NetPeer.Initialize(config, this._netSocket);
			this._netPeerGroup = NetPeer.GetGroup(this._netPeer, 0);
			this._netPeerRng = new Random(Environment.TickCount);
		}

		private void NetworkSend()
		{
			bool flag = this._netPeer == null;
			if (!flag)
			{
				EngineProfiler.Begin("Simulation.NetworkSend");
				NetPeer.Send(this._netPeer, this._netSocket);
				EngineProfiler.End();
			}
		}

		private void NetworkRecv()
		{
			bool flag = this._netPeer == null;
			if (!flag)
			{
				EngineProfiler.Begin("Simulation.NetworkRecv");
				bool flag2 = this._netPeerRng == null;
				if (flag2)
				{
					this._netPeerRng = new Random(Environment.TickCount);
				}
				NetPeer.Recv(this._netPeer, this._netSocket, this._netPeerRng);
				NetPeerGroup.Update(this._netPeerGroup, this);
				this.NetworkReceiveDone();
				EngineProfiler.End();
			}
		}

		private void NetworkShutdown()
		{
			this.OnNetworkShutdown();
			foreach (SimulationConnection simulationConnection in this._connections.Values)
			{
				this.FreeMessages(ref simulationConnection.MessagesIn);
				this.FreeMessages(ref simulationConnection.MessagesOut);
			}
			NetPeer.Destroy(this._netPeer, this._netSocket, this);
			this._netPeer = null;
			this._netPeerGroup = null;
			this._netSocket = null;
		}

		internal virtual void OnNetworkShutdown()
		{
		}

		private unsafe bool NetworkGetBuffer(NetConnection* connection, out NetBitBuffer* buffer)
		{
			bool flag = this._netPeer == null;
			bool result;
			if (flag)
			{
				buffer = (IntPtr)((UIntPtr)0);
				result = false;
			}
			else
			{
				result = NetPeerGroup.GetNotifyDataBuffer(this._netPeerGroup, connection, out buffer);
			}
			return result;
		}

		private unsafe bool NetworkSendBuffer(NetConnection* connection, NetBitBuffer* buffer, SimulationPacketEnvelope* envelope)
		{
			bool flag = this._netPeer == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = NetPeerGroup.SendNotifyDataBuffer(this._netPeerGroup, connection, buffer, (void*)envelope);
				bool flag3 = !flag2;
				if (flag3)
				{
					Assert.AlwaysFail("SendNotifyDataBuffer failed");
				}
				result = flag2;
			}
			return result;
		}

		internal unsafe bool NetworkSendPing(NetAddress address, void* data, int length)
		{
			bool flag = this._netPeer == null;
			return !flag && NetPeerGroup.SendUnconnectedData(this._netPeerGroup, address, data, length);
		}

		unsafe void INetPeerGroupCallbacks.OnConnectionAttempt(NetConnection* connection, int attempt, int totalConnectionAttempts)
		{
			Assert.Check(this.IsClient);
			bool flag;
			NetAddress newAddress;
			this._callbacks.OnInternalConnectionAttempt(attempt, totalConnectionAttempts, out flag, out newAddress);
			bool flag2 = flag;
			if (flag2)
			{
				NetPeerGroup.ChangeConnectionAddressDuringConnecting(this._netPeerGroup, connection, newAddress);
			}
		}

		unsafe void INetPeerGroupCallbacks.OnUnconnectedData(NetBitBuffer* buffer)
		{
		}

		unsafe void INetPeerGroupCallbacks.OnConnected(NetConnection* connection)
		{
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(this, string.Format("OnConnected {0} / {1}", connection->LocalConnectionId.GroupIndex, connection->Address));
			}
			SimulationConnection simulationConnection = new SimulationConnection(this);
			this._connections.Add((int)connection->LocalConnectionId.GroupIndex, simulationConnection);
			simulationConnection.Connection = connection;
			simulationConnection.ConnectionId = connection->LocalConnectionId;
			bool isServer = this.IsServer;
			if (isServer)
			{
				Simulation.PlayerRefMapping? playerRefMapping = this.GetPlayerRefMapping(connection->UniqueId);
				bool flag = playerRefMapping == null;
				if (flag)
				{
					throw new Exception();
				}
				this.GetPlayerSimulationData(playerRefMapping.Value.PlayerRef, true);
				this.PlayerAdd(playerRefMapping.Value.PlayerRef, simulationConnection);
			}
			else
			{
				this.PlayerAdd(this._callbacks.LocalPlayerRef, simulationConnection);
			}
			this.NetworkConnected(connection);
			try
			{
				bool isClient = this.IsClient;
				if (isClient)
				{
					this._callbacks.OnConnectedToServer();
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

		unsafe void INetPeerGroupCallbacks.OnDisconnected(NetConnection* connection, NetDisconnectReason reason)
		{
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(this, string.Format("Disconnected: Address={0}, Reason={1}", connection->Address, reason));
			}
			SimulationConnection simulationConnection = this.GetSimulationConnection(connection);
			bool flag = simulationConnection == null;
			if (flag)
			{
				LogStream logWarn = InternalLogStreams.LogWarn;
				if (logWarn != null)
				{
					logWarn.Log(string.Format("got disconnect for {0} without a simulation connection, reason: {1}", connection->Address, reason));
				}
			}
			PlayerRef playerRef = this.Connection2Player(connection);
			this.NetworkDisconnected(connection, reason);
			bool isServer = this.IsServer;
			if (isServer)
			{
				bool flag2 = simulationConnection != null;
				if (flag2)
				{
					this.AOI_RemoveConnection(simulationConnection);
				}
				Simulation.PlayerRefMapping? playerRefMapping = this.GetPlayerRefMapping(connection->UniqueId);
				bool flag3 = playerRefMapping == null;
				if (flag3)
				{
					throw new Exception();
				}
				this.DeletePlayerSimulationDataOnDisconnect(playerRefMapping.Value.PlayerRef);
			}
			this.PlayerRemove(playerRef);
			this._playersConnections.Remove(playerRef);
			this._connections.Remove((int)connection->LocalConnectionId.GroupIndex);
			bool flag4 = simulationConnection != null;
			if (flag4)
			{
				simulationConnection.Free(this);
			}
		}

		unsafe void INetPeerGroupCallbacks.OnReliableData(NetConnection* connection, ReliableId id, byte* data)
		{
			bool isServer = this.IsServer;
			if (isServer)
			{
				bool flag = id.Target != -1 && id.Target != this.LocalPlayer.AsIndex;
				if (flag)
				{
					bool flag2 = (this.ProjectConfig.Network.ReliableDataTransferModes & NetworkConfiguration.ReliableDataTransfers.ClientToClientWithServerProxy) == NetworkConfiguration.ReliableDataTransfers.ClientToClientWithServerProxy;
					if (flag2)
					{
						SimulationConnection simulationConnection;
						bool flag3 = this._playersConnections.TryGetValue(PlayerRef.FromIndex(id.Target), out simulationConnection);
						if (flag3)
						{
							NetPeerGroup.SendReliable(this._netPeerGroup, simulationConnection.Connection, id, data, id.SliceLength);
						}
						else
						{
							DebugLogStream logDebug = InternalLogStreams.LogDebug;
							if (logDebug != null)
							{
								logDebug.Error(this, string.Format("Target client connection ({0}) not found to send reliable data", id.Target));
							}
						}
					}
					else
					{
						DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
						if (logDebug2 != null)
						{
							logDebug2.Error(this, "Disconnecting client for sending server-proxied reliable data when not allowed");
						}
						NetPeerGroup.Disconnect(this._netPeerGroup, connection, null);
					}
					return;
				}
				bool flag4 = (this.ProjectConfig.Network.ReliableDataTransferModes & NetworkConfiguration.ReliableDataTransfers.ClientToServer) != NetworkConfiguration.ReliableDataTransfers.ClientToServer;
				if (flag4)
				{
					NetPeerGroup.Disconnect(this._netPeerGroup, connection, null);
					DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
					if (logDebug3 != null)
					{
						logDebug3.Error(this, "Disconnecting client for sending reliable data when not allowed");
					}
					return;
				}
			}
			byte[] array = new byte[id.SliceLength];
			byte[] array2;
			byte* destination;
			if ((array2 = array) == null || array2.Length == 0)
			{
				destination = null;
			}
			else
			{
				destination = &array2[0];
			}
			Native.MemCpy((void*)destination, (void*)data, id.SliceLength);
			array2 = null;
			this._callbacks.OnReliableData(this.Connection2Player(connection), id, false, array);
		}

		OnConnectionRequestReply INetPeerGroupCallbacks.OnConnectionRequest(NetAddress remoteAddres, byte[] token, byte[] uniqueid)
		{
			ulong key = BitConverter.ToUInt64(uniqueid, 0);
			Simulation.PlayerRefMapping playerRefMapping;
			bool flag = this._uniqueIdPlayerRefMapping.TryGetValue(key, out playerRefMapping);
			OnConnectionRequestReply result;
			if (flag)
			{
				result = this._callbacks.OnConnectionRequest(remoteAddres, token);
			}
			else
			{
				result = OnConnectionRequestReply.Waiting;
			}
			return result;
		}

		void INetPeerGroupCallbacks.OnConnectionFailed(NetAddress address, NetConnectFailedReason reason)
		{
			try
			{
				this._callbacks.OnConnectionFailed(address, reason);
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

		unsafe void INetPeerGroupCallbacks.OnUnreliableData(NetConnection* connection, NetBitBuffer* buffer)
		{
			Assert.AlwaysFail("Not implemented");
		}

		unsafe void INetPeerGroupCallbacks.OnNotifyData(NetConnection* c, NetBitBuffer* buffer)
		{
			SimulationConnection connection;
			this.TryGetSimulationConnectionLogErrorIfFailed(c, out connection);
			this._recvContext.Init(connection, buffer);
			this._fusionStatsManager.PendingSnapshot.AddToInPacketsStat(1, false);
			this._fusionStatsManager.PendingSnapshot.AddToInBandwidthStat((float)Maths.BytesRequiredForBits(buffer->LengthBits), false);
			this._recvContext.Connection.PacketReceiveDelta();
			try
			{
				this.RecvMessages();
				this.RecvPacket();
			}
			catch (Exception error)
			{
				this._recvContext.Done();
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(this, error);
				}
			}
		}

		private unsafe void OnEnvelopeLost(NetConnection* connection, SimulationPacketEnvelope* envelope)
		{
			bool flag = connection->ConnectionStatus == NetConnectionStatus.Connected;
			if (flag)
			{
				SimulationConnection simulationConnection = this.GetSimulationConnection(connection);
				bool flag2 = envelope->Messages.Count > 0;
				if (flag2)
				{
					TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
					if (logTraceSimulationMessage != null)
					{
						logTraceSimulationMessage.Warn(this, string.Format("Lost {0} messages, requeing", envelope->Messages.Count));
					}
					while (envelope->Messages.Count > 0)
					{
						SimulationMessageEnvelope* ptr = envelope->Messages.RemoveHead();
						TraceLogStream logTraceSimulationMessage2 = InternalLogStreams.LogTraceSimulationMessage;
						if (logTraceSimulationMessage2 != null)
						{
							logTraceSimulationMessage2.Warn(this, string.Format("Requeued {0}", LogUtils.GetDump<SimulationMessageEnvelope>(ptr)));
						}
						simulationConnection.MessagesOut.AddFirst(ptr);
					}
				}
				this._stateReplicator.OnPacketLost(connection, envelope);
			}
			else
			{
				this.FreeMessages(ref envelope->Messages);
			}
		}

		private unsafe void OnEnvelopeDelivered(NetConnection* connection, SimulationPacketEnvelope* envelope)
		{
			this.FreeMessages(ref envelope->Messages);
			this._stateReplicator.OnPacketDelivered(connection, envelope);
		}

		unsafe void INetPeerGroupCallbacks.OnNotifyDispose(ref NetSendEnvelope envelope)
		{
			NetPacketType packetType = envelope.PacketType;
			NetPacketType netPacketType = packetType;
			if (netPacketType != NetPacketType.NotifyData)
			{
				if (netPacketType == NetPacketType.NotifyReliableData)
				{
					byte* ptr = envelope.TakeUserData<byte>();
					Native.Free<byte>(ref ptr);
				}
			}
			else
			{
				SimulationPacketEnvelope* ptr2 = envelope.TakeUserData<SimulationPacketEnvelope>();
				this.FreeMessages(ref ptr2->Messages);
				SimulationPacketEnvelope.Free(this, ref ptr2);
			}
		}

		unsafe void INetPeerGroupCallbacks.OnNotifyLost(NetConnection* connection, ref NetSendEnvelope envelope)
		{
			NetPacketType packetType = envelope.PacketType;
			NetPacketType netPacketType = packetType;
			if (netPacketType != NetPacketType.NotifyData)
			{
				if (netPacketType == NetPacketType.NotifyReliableData)
				{
					byte* ptr = envelope.TakeUserData<byte>();
					Native.Free<byte>(ref ptr);
				}
			}
			else
			{
				SimulationPacketEnvelope* envelope2 = envelope.TakeUserData<SimulationPacketEnvelope>();
				this.OnEnvelopeLost(connection, envelope2);
				SimulationPacketEnvelope.Free(this, ref envelope2);
			}
		}

		unsafe void INetPeerGroupCallbacks.OnNotifyDelivered(NetConnection* connection, ref NetSendEnvelope envelope)
		{
			NetPacketType packetType = envelope.PacketType;
			NetPacketType netPacketType = packetType;
			if (netPacketType != NetPacketType.NotifyData)
			{
				if (netPacketType == NetPacketType.NotifyReliableData)
				{
					byte* ptr = envelope.TakeUserData<byte>();
					Native.Free<byte>(ref ptr);
				}
			}
			else
			{
				SimulationPacketEnvelope* envelope2 = envelope.TakeUserData<SimulationPacketEnvelope>();
				this.OnEnvelopeDelivered(connection, envelope2);
				SimulationPacketEnvelope.Free(this, ref envelope2);
			}
		}

		internal uint IdCounter
		{
			get
			{
				return this._idCounter;
			}
		}

		public int ObjectCount
		{
			get
			{
				return this._metaLookup.Count;
			}
		}

		public Dictionary<NetworkId, NetworkObjectMeta> Objects
		{
			get
			{
				return this._metaLookup;
			}
		}

		internal NetworkObjectHeaderSnapshot GetSnapshot()
		{
			return (this._snapshotsPool.Count > 0) ? this._snapshotsPool.Pop() : new NetworkObjectHeaderSnapshot(this._allocator);
		}

		internal int GetObjectsAllocatorUsedSegmentsInBytes()
		{
			return this._allocatorObjects.GetTotalSegmentsUsedInBytes();
		}

		internal int GetGeneralAllocatorUsedSegmentsInBytes()
		{
			return this._allocator.GetTotalSegmentsUsedInBytes();
		}

		internal int GetObjectsAllocatorFreeSegmentsInBytes()
		{
			return this._allocatorObjects.GetFreeSegmentsInBytes();
		}

		internal int GetGeneralAllocatorFreeSegmentsInBytes()
		{
			return this._allocator.GetFreeSegmentsInBytes();
		}

		internal void GetMemorySnapshot(MemoryStatisticsSnapshot.TargetAllocator targetAllocator, ref MemoryStatisticsSnapshot snapshot)
		{
			Allocator allocator = (targetAllocator == MemoryStatisticsSnapshot.TargetAllocator.General) ? this._allocator : this._allocatorObjects;
			allocator.GetMemorySnapshot(ref snapshot);
		}

		internal void SnapshotRelease(NetworkObjectHeaderSnapshot snapshot)
		{
			snapshot.Release();
			this._snapshotsPool.Push(snapshot);
		}

		internal void SnapshotRelease(ref NetworkObjectHeaderSnapshot snapshot)
		{
			bool flag = snapshot == null;
			if (!flag)
			{
				NetworkObjectHeaderSnapshot snapshot2 = snapshot;
				snapshot = null;
				this.SnapshotRelease(snapshot2);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool IsSimulated(NetworkObjectMeta meta)
		{
			return meta != null && meta.Instance && meta.Instance.IsInSimulation;
		}

		internal bool HasObject(NetworkId id)
		{
			NetworkObjectMeta networkObjectMeta;
			bool flag = this._metaLookup.TryGetValue(id, out networkObjectMeta);
			bool result;
			if (flag)
			{
				Assert.Check(id == networkObjectMeta.Id);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal void LogAllObjectIds()
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn != null)
			{
				logWarn.Log(string.Join(", ", from x in this._metaLookup
				select string.Format("{0} == {1}", x.Key, x.Value.Id)));
			}
		}

		internal bool TryGetMeta(NetworkId id, out NetworkObjectMeta meta)
		{
			meta = null;
			bool flag = this._metaLookup.TryGetValue(id, out meta);
			bool result;
			if (flag)
			{
				Assert.Check(id == meta.Id);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal NetworkObjectMeta GetMeta(NetworkId id)
		{
			NetworkObjectMeta networkObjectMeta = this._metaLookup[id];
			Assert.Check(networkObjectMeta.Id == id);
			return networkObjectMeta;
		}

		internal unsafe NetworkId GetNextId()
		{
			uint num = this._idCounter + 1U;
			this._idCounter = num;
			NetworkId result;
			result.Raw = num;
			bool isClient = this.IsClient;
			if (isClient)
			{
				Assert.Check(this.Topology == Topologies.Shared);
				Assert.Check(this.LocalPlayer.IsRealPlayer);
				result.Raw &= 524287U;
				result.Raw |= (((Simulation.Client)this).ServerConnection->Counter << 19 & 4294443008U);
			}
			return result;
		}

		internal NetworkObjectHeaderSnapshotRef GetLatestSnapshot(NetworkId id)
		{
			NetworkObjectMeta networkObjectMeta;
			bool flag = this.TryGetMeta(id, out networkObjectMeta) && networkObjectMeta.HasSnapshots;
			NetworkObjectHeaderSnapshotRef result;
			if (flag)
			{
				result = networkObjectMeta.SnapshotLatest;
			}
			else
			{
				result = default(NetworkObjectHeaderSnapshotRef);
			}
			return result;
		}

		internal unsafe bool TryGetStructData<[IsUnmanaged] T>(NetworkId id, out T* data) where T : struct, ValueType
		{
			NetworkObjectMeta networkObjectMeta;
			bool flag = this.TryGetStruct(id, out networkObjectMeta);
			bool result;
			if (flag)
			{
				data = networkObjectMeta.GetDataAs<T>();
				result = true;
			}
			else
			{
				data = (IntPtr)((UIntPtr)0);
				result = false;
			}
			return result;
		}

		internal bool TryGetStruct(NetworkId id, out NetworkObjectMeta meta)
		{
			bool flag = this._metaLookup.TryGetValue(id, out meta);
			bool result;
			if (flag)
			{
				Assert.Check(id == meta.Id);
				Assert.Check((meta.Flags & NetworkObjectHeaderFlags.Struct) == NetworkObjectHeaderFlags.Struct);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal bool TryGetInstance(NetworkId id, out NetworkObject instance)
		{
			Assert.Check(!id.IsReserved);
			NetworkObjectMeta networkObjectMeta;
			bool flag = this._metaLookup.TryGetValue(id, out networkObjectMeta) && BehaviourUtils.IsAlive(networkObjectMeta.Instance);
			bool result;
			if (flag)
			{
				Assert.Check(id == networkObjectMeta.Id);
				instance = networkObjectMeta.Instance;
				result = true;
			}
			else
			{
				instance = null;
				result = false;
			}
			return result;
		}

		internal unsafe T* AllocateStruct<[IsUnmanaged] T>(NetworkId id, int extraWords = 0, NetworkObjectTypeId? objectTypeId = null) where T : struct, ValueType
		{
			NetworkObjectMeta networkObjectMeta = this.AllocateStruct(id, Native.RoundToAlignment(sizeof(T), 4) / 4 + extraWords, objectTypeId);
			return networkObjectMeta.GetDataAs<T>();
		}

		internal NetworkObjectMeta AllocateStruct(NetworkId id, int words, NetworkObjectTypeId? objectTypeId = null)
		{
			Assert.Check(id.IsValid);
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(this, string.Format("allocating struct {0} (words: {1})", id, words));
			}
			int wordCount = 20 + words;
			return this.AllocateObject(id, wordCount, objectTypeId.GetValueOrDefault(), 0, default(NetworkId), default(NetworkObjectNestingKey), NetworkObjectHeaderFlags.Struct);
		}

		internal unsafe NetworkObjectMeta AllocateObject(in NetworkObjectHeader header)
		{
			Assert.Check(header.Id.IsValid);
			int blockByteSize = this._projectConfig.Heap.ToAllocatorConfig().BlockByteSize;
			Assert.Always<short, int, int>(header.WordCount >= 20 && (int)(header.WordCount * 4) <= blockByteSize, "{0} >= NetworkObjectHeader.WORDS && {1} <= {2}", header.WordCount, (int)(header.WordCount * 4), blockByteSize);
			Assert.Always(!this.HasObject(header.Id), "id already exists");
			void* ptr = Allocator.AllocAndClear(this._allocatorObjects, (int)(header.WordCount * 4));
			NetworkObjectHeader* ptr2 = (NetworkObjectHeader*)ptr;
			*ptr2 = header;
			NetworkObjectMeta networkObjectMeta = new NetworkObjectMeta(this, this._allocator);
			networkObjectMeta.Init((int*)ptr, header.WordCount, header.BehaviourCount, header.Flags);
			this._metaLookup.Add(ptr2->Id, networkObjectMeta);
			this.HostMigrationAfterAllocateObject(networkObjectMeta);
			bool flag = (header.Flags & NetworkObjectHeaderFlags.Struct) == NetworkObjectHeaderFlags.Struct;
			if (flag)
			{
				this._structs.Add(header.Id);
				this._structsVersion++;
			}
			return networkObjectMeta;
		}

		internal NetworkObjectMeta AllocateObject(NetworkId id, int wordCount, NetworkObjectTypeId type = default(NetworkObjectTypeId), int behaviourCount = 0, NetworkId nestingRoot = default(NetworkId), NetworkObjectNestingKey nestingKey = default(NetworkObjectNestingKey), NetworkObjectHeaderFlags flags = (NetworkObjectHeaderFlags)0)
		{
			NetworkObjectHeader networkObjectHeader = new NetworkObjectHeader(id, (short)wordCount, (short)behaviourCount, type, nestingRoot, nestingKey, flags);
			return this.AllocateObject(networkObjectHeader);
		}

		internal unsafe void FreeObject(NetworkId id)
		{
			bool flag = !id.IsValid;
			if (!flag)
			{
				NetworkObjectMeta networkObjectMeta;
				bool flag2 = this._metaLookup.TryGetValue(id, out networkObjectMeta);
				if (flag2)
				{
					bool flag3 = id.Raw <= 1023U;
					if (flag3)
					{
						LogStream logError = InternalLogStreams.LogError;
						if (logError != null)
						{
							logError.Log(string.Format("Trying do free an internal object that never should be freed: {0}", id));
						}
					}
					else
					{
						this._metaLookup.Remove(id);
						bool flag4 = (this.IsServer || this.Config.Topology == Topologies.Shared) && this.Config.SchedulingEnabled;
						if (flag4)
						{
							foreach (SimulationConnection simulationConnection in this._connections.Values)
							{
								NetworkObjectConnectionData item;
								bool flag5 = simulationConnection.TryGetObjectData(id, out item);
								if (flag5)
								{
									simulationConnection.ObjectPriorityList.Remove(item);
								}
							}
						}
						bool flag6 = this.IsServer && this.Config.AreaOfInterestEnabled;
						if (flag6)
						{
							this.AOI_RemoveFromAreaOfInterest(networkObjectMeta, true);
						}
						this.HostMigrationAfterFreeObject(networkObjectMeta);
						bool flag7 = (networkObjectMeta.Flags & NetworkObjectHeaderFlags.Struct) == NetworkObjectHeaderFlags.Struct;
						if (flag7)
						{
							this._structs.Remove(id);
							this._structsVersion++;
							bool flag8 = networkObjectMeta.Type == NetworkObjectTypeId.PlayerData;
							if (flag8)
							{
								Simulation.PlayerSimulationData* dataAs = networkObjectMeta.GetDataAs<Simulation.PlayerSimulationData>();
								PlayerRef player = dataAs->Player;
								bool isRealPlayer = player.IsRealPlayer;
								if (isRealPlayer)
								{
									bool flag9 = dataAs->Object != default(NetworkId);
									if (flag9)
									{
										this._playerLeftTempObjectCache.Add(player, dataAs->Object);
									}
									this._invokeJoinedLeaveQueue.Enqueue(new ValueTuple<PlayerRef, bool>(player, false));
									bool flag10 = !this.IsServer;
									if (flag10)
									{
										this._players.Remove(player);
									}
								}
							}
						}
						networkObjectMeta.Release(this._allocatorObjects);
					}
				}
			}
		}

		internal unsafe int GetRpcSourceAuthorityMask(NetworkObjectMeta meta, PlayerRef player)
		{
			return AuthorityMasks.Create(this.IsStateAuthority(*meta.StateAuthority, player), this.IsInputAuthority(*meta.InputAuthority, player));
		}

		internal int GetLocalAuthorityMask([RequiresLocation] [In] ref NetworkObjectHeader obj)
		{
			return AuthorityMasks.Create(this.IsLocalSimulationStateAuthority(ref obj), this.IsLocalSimulationInputAuthority(ref obj));
		}

		internal unsafe RpcTargetStatus GetRpcTargetStatus(PlayerRef target)
		{
			bool flag = target == this.LocalPlayer;
			RpcTargetStatus result;
			if (flag)
			{
				result = RpcTargetStatus.Self;
			}
			else
			{
				bool isServer = this.IsServer;
				if (isServer)
				{
					bool isNone = target.IsNone;
					if (isNone)
					{
						result = RpcTargetStatus.Self;
					}
					else
					{
						NetConnection* ptr;
						bool flag2 = this.GetConnectionIndexForPlayer(target) != null && NetPeerGroup.TryGetConnectionByIndex(this._netPeerGroup, this.GetConnectionIndexForPlayer(target).Value, out ptr) && ptr->Active && ptr->ConnectionStatus == NetConnectionStatus.Connected;
						if (flag2)
						{
							result = RpcTargetStatus.Remote;
						}
						else
						{
							result = RpcTargetStatus.Unreachable;
						}
					}
				}
				else
				{
					bool flag3 = target.IsRealPlayer || target.IsNone;
					if (flag3)
					{
						result = RpcTargetStatus.Remote;
					}
					else
					{
						result = RpcTargetStatus.Unreachable;
					}
				}
			}
			return result;
		}

		internal unsafe RpcSendMessageResult SendMessage(ref SimulationMessage* message)
		{
			int num = 0;
			RpcSendMessageResult result;
			try
			{
				NetworkId messageTargetObjectIdForVerification = this.GetMessageTargetObjectIdForVerification(message);
				message.Tick = this.Tick;
				bool isClient = this.IsClient;
				if (isClient)
				{
					TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
					if (logTraceSimulationMessage != null)
					{
						logTraceSimulationMessage.Log(this, string.Format("Sending to server {0}", LogUtils.GetDump<SimulationMessage>(message)));
					}
					PlayerRef none = PlayerRef.None;
					NetConnection* connectionByIndex = NetPeerGroup.GetConnectionByIndex(this._netPeerGroup, 0);
					Assert.Check(this._netPeerGroup->ConnectionCount == 1);
					bool flag = !connectionByIndex->Active || connectionByIndex->ConnectionStatus != NetConnectionStatus.Connected;
					if (flag)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Warn(string.Format("Failed to send {0} to {1}: connection not active and/or connected", "SimulationMessage", none));
						}
						result = RpcSendMessageResult.NotSentTargetClientNotAvailable;
					}
					else
					{
						Simulation.TargetObjectVerificationResult targetObjectVerificationResult;
						bool flag2 = !this.VerifyMessageTargetObject(connectionByIndex, messageTargetObjectIdForVerification, out targetObjectVerificationResult);
						if (flag2)
						{
							DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
							if (logDebug2 != null)
							{
								logDebug2.Warn(this, string.Format("Message not sent to {0}. Reason: {1} {2}", none, targetObjectVerificationResult, LogUtils.GetDump<SimulationMessage>(message)));
							}
							result = Simulation.<SendMessage>g__VerifyResultToSendMessageResult|328_0(targetObjectVerificationResult);
						}
						else
						{
							this.SendMessageInternal(message, connectionByIndex);
							num = 1;
							result = RpcSendMessageResult.SentToServerForForwarding;
						}
					}
				}
				else
				{
					bool flag3 = message.IsTargeted();
					if (flag3)
					{
						TraceLogStream logTraceSimulationMessage2 = InternalLogStreams.LogTraceSimulationMessage;
						if (logTraceSimulationMessage2 != null)
						{
							logTraceSimulationMessage2.Log(this, string.Format("Server sending to a specific player {0}", LogUtils.GetDump<SimulationMessage>(message)));
						}
						Assert.Check(message.GetFlag(16));
						Assert.Check(message.Target.IsRealPlayer);
						PlayerRef target = message.Target;
						message.Target = default(PlayerRef);
						NetConnection* ptr;
						bool flag4 = this.GetConnectionIndexForPlayer(target) == null || !NetPeerGroup.TryGetConnectionByIndex(this._netPeerGroup, this.GetConnectionIndexForPlayer(target).Value, out ptr);
						if (flag4)
						{
							DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
							if (logDebug3 != null)
							{
								logDebug3.Warn(string.Format("Failed to send {0} to {1}: connection not found", "SimulationMessage", target));
							}
							result = RpcSendMessageResult.NotSentTargetClientNotAvailable;
						}
						else
						{
							bool flag5 = !ptr->Active || ptr->ConnectionStatus != NetConnectionStatus.Connected;
							if (flag5)
							{
								DebugLogStream logDebug4 = InternalLogStreams.LogDebug;
								if (logDebug4 != null)
								{
									logDebug4.Warn(string.Format("Failed to send {0} to {1}: connection not active and/or connected", "SimulationMessage", target));
								}
								result = RpcSendMessageResult.NotSentTargetClientNotAvailable;
							}
							else
							{
								Simulation.TargetObjectVerificationResult targetObjectVerificationResult2;
								bool flag6 = !this.VerifyMessageTargetObject(ptr, messageTargetObjectIdForVerification, out targetObjectVerificationResult2);
								if (flag6)
								{
									DebugLogStream logDebug5 = InternalLogStreams.LogDebug;
									if (logDebug5 != null)
									{
										logDebug5.Warn(this, string.Format("Message not sent to {0}. Reason: {1} {2}", target, targetObjectVerificationResult2, LogUtils.GetDump<SimulationMessage>(message)));
									}
									result = Simulation.<SendMessage>g__VerifyResultToSendMessageResult|328_0(targetObjectVerificationResult2);
								}
								else
								{
									this.SendMessageInternal(message, ptr);
									num = 1;
									result = RpcSendMessageResult.SentToTargetClient;
								}
							}
						}
					}
					else
					{
						TraceLogStream logTraceSimulationMessage3 = InternalLogStreams.LogTraceSimulationMessage;
						if (logTraceSimulationMessage3 != null)
						{
							logTraceSimulationMessage3.Log(this, string.Format("Server broadcasting to clients {0}", LogUtils.GetDump<SimulationMessage>(message)));
						}
						NetConnectionMap.Iterator iterator = NetPeerGroup.ConnectionIterator(this._netPeerGroup);
						bool flag7 = false;
						while (iterator.Next())
						{
							bool flag8 = iterator.Current->ConnectionStatus != NetConnectionStatus.Connected;
							if (!flag8)
							{
								flag7 = true;
								PlayerRef playerRef = this.Connection2Player(iterator.Current);
								Simulation.TargetObjectVerificationResult targetObjectVerificationResult3;
								bool flag9 = !this.VerifyMessageTargetObject(iterator.Current, messageTargetObjectIdForVerification, out targetObjectVerificationResult3);
								if (flag9)
								{
									TraceLogStream logTraceSimulationMessage4 = InternalLogStreams.LogTraceSimulationMessage;
									if (logTraceSimulationMessage4 != null)
									{
										logTraceSimulationMessage4.Log(this, string.Format("Server broadcast message not sent to {0}. Reason: {1} {2}", playerRef, targetObjectVerificationResult3, LogUtils.GetDump<SimulationMessage>(message)));
									}
								}
								else
								{
									this.SendMessageInternal(message, iterator.Current);
									num++;
								}
							}
						}
						result = (flag7 ? ((num == 0) ? RpcSendMessageResult.NotSentBroadcastNoConfirmedNorInterestedClients : RpcSendMessageResult.SentBroadcast) : RpcSendMessageResult.NotSentBroadcastNoActiveConnections);
					}
				}
			}
			finally
			{
				bool flag10 = num == 0;
				if (flag10)
				{
					SimulationMessage.Free(this, ref message);
				}
			}
			return result;
		}

		internal unsafe bool ForwardMessage(SimulationMessage* message, PlayerRef target, bool required)
		{
			Assert.Check(this.IsServer, "Only server can forward messages");
			Assert.Check(message->GetFlag(2), "Only received messages are to be forwarded");
			SimulationConnection simulationConnection;
			bool flag = !this.TryGetSimulationConnectionForPlayer(target, out simulationConnection);
			bool result;
			if (flag)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Error(this, string.Format("Failed to forward to {0}: simulation connection not found {1}", target, LogUtils.GetDump<SimulationMessage>(message)));
				}
				result = false;
			}
			else
			{
				NetConnection* ptr;
				bool flag2 = !NetPeerGroup.TryGetConnectionByIndex(this._netPeerGroup, simulationConnection.ConnectionIndex, out ptr);
				if (flag2)
				{
					if (required)
					{
						DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
						if (logDebug2 != null)
						{
							logDebug2.Error(this, string.Format("Failed to forward to {0}: connection not found {1}", target, LogUtils.GetDump<SimulationMessage>(message)));
						}
					}
					result = false;
				}
				else
				{
					bool flag3 = !ptr->Active || ptr->ConnectionStatus != NetConnectionStatus.Connected;
					if (flag3)
					{
						if (required)
						{
							DebugLogStream logDebug3 = InternalLogStreams.LogDebug;
							if (logDebug3 != null)
							{
								logDebug3.Error(this, string.Format("Failed to forward to {0}: connection not active and/or connected {1}", target, LogUtils.GetDump<SimulationMessage>(message)));
							}
						}
						result = false;
					}
					else
					{
						NetworkId messageTargetObjectIdForVerification = this.GetMessageTargetObjectIdForVerification(message);
						Simulation.TargetObjectVerificationResult targetObjectVerificationResult;
						bool flag4 = !this.VerifyMessageTargetObject(ptr, messageTargetObjectIdForVerification, out targetObjectVerificationResult);
						if (flag4)
						{
							if (required)
							{
								DebugLogStream logDebug4 = InternalLogStreams.LogDebug;
								if (logDebug4 != null)
								{
									logDebug4.Warn(this, string.Format("Failed to forward to {0} to {1}: {2} {3}", new object[]
									{
										target,
										messageTargetObjectIdForVerification,
										targetObjectVerificationResult,
										LogUtils.GetDump<SimulationMessage>(message)
									}));
								}
							}
							else
							{
								TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
								if (logTraceSimulationMessage != null)
								{
									logTraceSimulationMessage.Log(this, string.Format("Failed to forward to {0} to {1}: {2} {3}", new object[]
									{
										target,
										messageTargetObjectIdForVerification,
										targetObjectVerificationResult,
										LogUtils.GetDump<SimulationMessage>(message)
									}));
								}
							}
							result = false;
						}
						else
						{
							message->Tick = this.Tick;
							bool flag5 = message->IsTargeted();
							if (flag5)
							{
								Assert.Check(message->Target == target, "When forwarding a targeted message, target should match the target player");
								message->Target = PlayerRef.None;
							}
							bool flag6 = message->Offset == 0;
							if (flag6)
							{
								Assert.Check(message->Offset == 0 || message->Offset == message->Capacity);
								message->Offset = message->Capacity;
							}
							TraceLogStream logTraceSimulationMessage2 = InternalLogStreams.LogTraceSimulationMessage;
							if (logTraceSimulationMessage2 != null)
							{
								logTraceSimulationMessage2.Log(this, string.Format("Forwarding to {0} {1}", target, LogUtils.GetDump<SimulationMessage>(message)));
							}
							this.SendMessageInternal(message, ptr);
							result = true;
						}
					}
				}
			}
			return result;
		}

		internal unsafe NetworkId GetMessageTargetObjectIdForVerification(SimulationMessage* message)
		{
			bool flag = message->GetFlag(1) || message->GetFlag(4) || message->GetFlag(64);
			NetworkId result;
			if (flag)
			{
				result = default(NetworkId);
			}
			else
			{
				RpcHeader rpcHeader = SimulationMessage.GetRawData(message).Read<RpcHeader>();
				result = rpcHeader.Object;
			}
			return result;
		}

		internal unsafe void SendInternalSimulationMessage<[IsUnmanaged] T>(SimulationMessageInternalTypes type, T buffer, PlayerRef? target = null) where T : struct, ValueType
		{
			Assert.Check(type > (SimulationMessageInternalTypes)0);
			SimulationMessage* ptr = SimulationMessage.Allocate(this, 4 + sizeof(T));
			ptr->Flags |= 64;
			*Simulation.GetMessageInternalType(ptr) = type;
			*Simulation.GetMessageInternalData<T>(ptr) = buffer;
			bool flag = target != null;
			if (flag)
			{
				Assert.Check(this.IsServer);
				ptr->Target = target.Value;
				ptr->Flags |= 16;
			}
			else
			{
				ptr->Target = default(PlayerRef);
				bool isClient = this.IsClient;
				if (isClient)
				{
					ptr->Flags |= 32;
				}
			}
			ptr->Offset = ptr->Capacity;
			this.SendMessage(ref ptr);
		}

		private unsafe bool VerifyMessageTargetObject(NetConnection* netConnection, NetworkId id, out Simulation.TargetObjectVerificationResult result)
		{
			bool flag = !id.IsValid;
			bool result2;
			if (flag)
			{
				result = Simulation.TargetObjectVerificationResult.Ok;
				result2 = true;
			}
			else
			{
				SimulationConnection simulationConnection;
				bool flag2 = !this.TryGetSimulationConnectionLogErrorIfFailed(netConnection, out simulationConnection);
				if (flag2)
				{
					result = Simulation.TargetObjectVerificationResult.Ok;
					result2 = true;
				}
				else
				{
					bool valueOrDefault = simulationConnection.ObjectData_IsCreateUnconfirmed(id).GetValueOrDefault();
					if (valueOrDefault)
					{
						result = Simulation.TargetObjectVerificationResult.Ok;
						result2 = true;
					}
					else
					{
						bool flag3 = (this.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement && this.IsServer;
						if (flag3)
						{
							PlayerRef player = this.Connection2Player(netConnection);
							bool flag4 = !this.Replicator.HasObjectInterest(player, id);
							if (flag4)
							{
								result = Simulation.TargetObjectVerificationResult.TargetNotInterestedInObject;
								return false;
							}
						}
						result = Simulation.TargetObjectVerificationResult.Ok;
						result2 = true;
					}
				}
			}
			return result2;
		}

		private unsafe void SendMessageInternal(SimulationMessage* message, NetConnection* netConnection)
		{
			TraceLogStream logTraceSimulationMessage = InternalLogStreams.LogTraceSimulationMessage;
			if (logTraceSimulationMessage != null)
			{
				logTraceSimulationMessage.Log(this, string.Format("Sending to {0} {1}", this.Connection2Player(netConnection), LogUtils.GetDump<SimulationMessage>(message)));
			}
			SimulationConnection simulationConnection;
			bool flag = !this.TryGetSimulationConnectionLogErrorIfFailed(netConnection, out simulationConnection);
			if (!flag)
			{
				ulong num2;
				if (!message->GetFlag(8))
				{
					SimulationConnection simulationConnection2 = simulationConnection;
					ulong num = simulationConnection2.MessagesOutSequence + 1UL;
					simulationConnection2.MessagesOutSequence = num;
					num2 = num;
				}
				else
				{
					num2 = 0UL;
				}
				ulong sequence = num2;
				simulationConnection.MessagesOut.AddLast(SimulationMessageEnvelope.Allocate(this, message, sequence));
			}
		}

		private void HostMigrationAfterFreeObject(NetworkObjectMeta meta)
		{
			bool flag = this.Mode == SimulationModes.Host;
			if (flag)
			{
				this._metaMigration.Remove(meta);
				this._metaMigrationRemoved.Enqueue(meta.Id);
			}
			bool flag2 = meta.Type.IsSceneObject && this._metaSceneLookup.ContainsKey(meta.Type);
			if (flag2)
			{
				this._metaSceneLookup.Remove(meta.Type);
			}
		}

		private void HostMigrationAfterAllocateObject(NetworkObjectMeta meta)
		{
			bool isSceneObject = meta.Type.IsSceneObject;
			if (isSceneObject)
			{
				this._metaSceneLookup[meta.Type] = meta;
			}
			bool flag = this.Mode == SimulationModes.Host;
			if (flag)
			{
				this._metaMigration.AddLast(meta);
			}
		}

		private void HostMigrationDispose()
		{
			Simulation.Server server = this as Simulation.Server;
			bool flag = server != null;
			if (flag)
			{
				server.DisposeHostMigration();
			}
		}

		internal bool TryGetSceneInstance(NetworkObjectTypeId sceneObjectTypeId, out NetworkObject instance)
		{
			Assert.Check(sceneObjectTypeId.IsSceneObject);
			NetworkObjectMeta networkObjectMeta;
			bool flag = this._metaSceneLookup.TryGetValue(sceneObjectTypeId, out networkObjectMeta) && BehaviourUtils.IsAlive(networkObjectMeta.Instance);
			bool result;
			if (flag)
			{
				instance = networkObjectMeta.Instance;
				result = true;
			}
			else
			{
				instance = null;
				result = false;
			}
			return result;
		}

		[CompilerGenerated]
		private Vector3? <UpdateAreaOfInterest>g__ResolveCellPosition|228_0(NetworkObjectMeta m)
		{
			ref NetworkTRSPData mainTRSPData = ref m.MainTRSPData;
			while (mainTRSPData.AreaOfInterestOverride)
			{
				NetworkObjectMeta networkObjectMeta;
				bool flag = this.TryGetMeta(mainTRSPData.AreaOfInterestOverride, out networkObjectMeta) && networkObjectMeta.HasMainTRSP;
				if (!flag)
				{
					return null;
				}
				mainTRSPData = networkObjectMeta.MainTRSPData;
			}
			while (mainTRSPData.Parent.Object)
			{
				NetworkObjectMeta networkObjectMeta2;
				bool flag2 = this.TryGetMeta(mainTRSPData.Parent.Object, out networkObjectMeta2) && networkObjectMeta2.HasMainTRSP;
				if (!flag2)
				{
					return null;
				}
				mainTRSPData = networkObjectMeta2.MainTRSPData;
			}
			return new Vector3?(mainTRSPData.Position);
		}

		[CompilerGenerated]
		internal unsafe static bool <RecvMessages>g__CanAppendQueue|239_0(SimulationMessageList list, SimulationMessageEnvelope* messageEnvelope, out SimulationMessageEnvelope* followingMessage)
		{
			SimulationMessageEnvelope* ptr;
			for (ptr = list.Tail; ptr != null; ptr = ptr->Prev)
			{
				bool flag = !ptr->Message->IsUnreliable;
				if (flag)
				{
					break;
				}
			}
			bool flag2 = ptr == null || ptr->Sequence < messageEnvelope->Sequence;
			bool result;
			if (flag2)
			{
				followingMessage = (IntPtr)((UIntPtr)0);
				result = true;
			}
			else
			{
				SimulationMessageEnvelope* ptr2;
				for (ptr2 = list.Head; ptr2 != null; ptr2 = ptr2->Next)
				{
					bool flag3 = !ptr2->Message->IsUnreliable && ptr2->Sequence >= messageEnvelope->Sequence;
					if (flag3)
					{
						break;
					}
				}
				Assert.Always(ptr2 != null, "Expected next list element");
				followingMessage = ((ptr2->Sequence == messageEnvelope->Sequence) ? null : ptr2);
				result = false;
			}
			return result;
		}

		[CompilerGenerated]
		internal static RpcSendMessageResult <SendMessage>g__VerifyResultToSendMessageResult|328_0(Simulation.TargetObjectVerificationResult status)
		{
			bool flag = status == Simulation.TargetObjectVerificationResult.TargetNotInterestedInObject;
			if (flag)
			{
				return RpcSendMessageResult.NotSentTargetObjectNotInPlayerInterest;
			}
			throw new ArgumentOutOfRangeException("status");
		}

		private Dictionary<int, Simulation.AreaOfInterestCell> _aoiCells = new Dictionary<int, Simulation.AreaOfInterestCell>();

		private Stack<Simulation.AreaOfInterestCell> _aoiCellsPool = new Stack<Simulation.AreaOfInterestCell>();

		private Dictionary<int, HashSet<int>> _aoiConnections = new Dictionary<int, HashSet<int>>();

		private ulong _interpolateSequence;

		private bool _isShutdown;

		private bool _isWaitingForShutdown;

		internal NetworkRunner Runner;

		private Simulation.ICallbacks _callbacks;

		private Tick _tick;

		private SimulationModes _mode;

		private SimulationStages _stage;

		private SimulationConfig _config;

		private NetworkProjectConfig _projectConfig;

		internal ITimeProvider _time;

		private Tick _interpTo;

		private Tick _interpFrom;

		private float _remoteAlpha;

		private float _localAlpha;

		private Tick _interpToPrev;

		private Tick _interpFromPrev;

		private float _remoteAlphaPrev;

		private float _localAlphaPrev;

		private SimulationInput _inputRoot;

		private SimulationInput.Pool _inputPool;

		private SimulationInputCollection _inputCollection;

		private Simulation.StateReplicator _stateReplicator;

		internal Dictionary<int, SimulationConnection> _connections = new Dictionary<int, SimulationConnection>();

		private Dictionary<PlayerRef, SimulationConnection> _playersConnections = new Dictionary<PlayerRef, SimulationConnection>(PlayerRef.Comparer);

		private HashSet<PlayerRef> _players;

		private double _updateTime;

		private bool _isResume;

		private Tick _sendTick;

		private bool _isLastTick;

		private bool _isFirstTick;

		private bool _isResimulation;

		private bool _isInTick;

		private bool _isInitialLocalTick;

		private bool? _isPaused;

		internal FusionStatisticsManager _fusionStatsManager;

		private Dictionary<Tick, double> _tickUpdateTimes;

		private Queue<ValueTuple<PlayerRef, bool>> _invokeJoinedLeaveQueue = new Queue<ValueTuple<PlayerRef, bool>>();

		private HashSet<NetworkId> _globalInterestObjects;

		private Dictionary<ulong, Simulation.PlayerRefMapping> _uniqueIdPlayerRefMapping = new Dictionary<ulong, Simulation.PlayerRefMapping>();

		private Simulation.SendContext _sendContext;

		private Simulation.RecvContext _recvContext;

		private int _reliableSend;

		internal INetSocket _netSocket;

		internal unsafe NetPeer* _netPeer;

		private unsafe NetPeerGroup* _netPeerGroup;

		private Random _netPeerRng;

		private Stack<NetworkObjectHeaderSnapshot> _snapshotsPool = new Stack<NetworkObjectHeaderSnapshot>();

		private uint _idCounter = 1023U;

		private Dictionary<NetworkId, NetworkObjectMeta> _metaLookup;

		private Dictionary<PlayerRef, NetworkId> _playerDataLookup;

		private Dictionary<PlayerRef, NetworkId> _playerLeftTempObjectCache;

		private HashSet<NetworkId> _structs;

		private int _structsVersion = 1;

		private Allocator _allocator;

		private Allocator _allocatorObjects;

		private readonly Dictionary<NetworkObjectTypeId, NetworkObjectMeta> _metaSceneLookup = new Dictionary<NetworkObjectTypeId, NetworkObjectMeta>(NetworkObjectTypeId.Comparer);

		private NetworkObjectMeta.ListMigration _metaMigration;

		private readonly Queue<NetworkId> _metaMigrationRemoved = new Queue<NetworkId>();

		private class AreaOfInterestCell
		{
			public bool Empty
			{
				get
				{
					return this.Objects.Count == 0 && this.Connections.Empty();
				}
			}

			public int Key;

			public NetworkObjectMeta.List Objects;

			public BitSet512 Connections;
		}

		public struct AreaOfInterest
		{
			[return: TupleElementNames(new string[]
			{
				"x",
				"y",
				"z"
			})]
			public static ValueTuple<int, int, int> GetGridSize()
			{
				return new ValueTuple<int, int, int>(Simulation.AreaOfInterest.X_SIZE, Simulation.AreaOfInterest.Y_SIZE, Simulation.AreaOfInterest.Z_SIZE);
			}

			public static int GetCellSize()
			{
				return Simulation.AreaOfInterest.CELL_SIZE;
			}

			public static void SphereToCells(Vector3 position, float radius, HashSet<int> cells)
			{
				ValueTuple<int, int, int> valueTuple = Simulation.AreaOfInterest.ToCellCoords(position - new Vector3(radius, radius, radius));
				ValueTuple<int, int, int> valueTuple2 = Simulation.AreaOfInterest.ToCellCoords(position + new Vector3(radius, radius, radius));
				for (int i = valueTuple.Item1; i <= valueTuple2.Item1; i++)
				{
					for (int j = valueTuple.Item2; j <= valueTuple2.Item2; j++)
					{
						for (int k = valueTuple.Item3; k <= valueTuple2.Item3; k++)
						{
							cells.Add(Simulation.AreaOfInterest.ToCell(i, j, k));
						}
					}
				}
			}

			[return: TupleElementNames(new string[]
			{
				"x",
				"y",
				"z"
			})]
			public static ValueTuple<int, int, int> ToCellCoords(Vector3 position)
			{
				int num = (int)(position.x / (float)Simulation.AreaOfInterest.CELL_SIZE);
				int num2 = (int)(position.y / (float)Simulation.AreaOfInterest.CELL_SIZE);
				int num3 = (int)(position.z / (float)Simulation.AreaOfInterest.CELL_SIZE);
				bool flag = position.x < 0f;
				if (flag)
				{
					num--;
				}
				bool flag2 = position.y < 0f;
				if (flag2)
				{
					num2--;
				}
				bool flag3 = position.z < 0f;
				if (flag3)
				{
					num3--;
				}
				return Simulation.AreaOfInterest.ClampCellCoords(num + Simulation.AreaOfInterest.X_SIZE / 2, num2 + Simulation.AreaOfInterest.Y_SIZE / 2, num3 + Simulation.AreaOfInterest.Z_SIZE / 2);
			}

			[return: TupleElementNames(new string[]
			{
				"x",
				"y",
				"z"
			})]
			public static ValueTuple<int, int, int> ToCellCoords(int index)
			{
				index--;
				int num = index / (Simulation.AreaOfInterest.X_SIZE * Simulation.AreaOfInterest.Y_SIZE);
				index -= num * Simulation.AreaOfInterest.X_SIZE * Simulation.AreaOfInterest.Y_SIZE;
				int item = index / Simulation.AreaOfInterest.X_SIZE;
				int item2 = index % Simulation.AreaOfInterest.X_SIZE;
				return new ValueTuple<int, int, int>(item2, item, num);
			}

			public static Vector3 ToCellCenter(int index)
			{
				bool flag = index == -1;
				Vector3 result;
				if (flag)
				{
					result = default(Vector3);
				}
				else
				{
					ValueTuple<int, int, int> valueTuple = Simulation.AreaOfInterest.ToCellCoords(index);
					int item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					int item3 = valueTuple.Item3;
					result = new Vector3((float)((item - Simulation.AreaOfInterest.X_SIZE / 2) * Simulation.AreaOfInterest.CELL_SIZE + Simulation.AreaOfInterest.CELL_SIZE / 2), (float)((item2 - Simulation.AreaOfInterest.Y_SIZE / 2) * Simulation.AreaOfInterest.CELL_SIZE + Simulation.AreaOfInterest.CELL_SIZE / 2), (float)((item3 - Simulation.AreaOfInterest.Z_SIZE / 2) * Simulation.AreaOfInterest.CELL_SIZE + Simulation.AreaOfInterest.CELL_SIZE / 2));
				}
				return result;
			}

			public static int ToCell(Vector3 position)
			{
				ValueTuple<int, int, int> valueTuple = Simulation.AreaOfInterest.ToCellCoords(position);
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				int item3 = valueTuple.Item3;
				return Simulation.AreaOfInterest.ToCell(item, item2, item3);
			}

			public static int ToCell(int x, int y, int z)
			{
				ValueTuple<int, int, int> valueTuple = Simulation.AreaOfInterest.ClampCellCoords(x, y, z);
				x = valueTuple.Item1;
				y = valueTuple.Item2;
				z = valueTuple.Item3;
				return z * Simulation.AreaOfInterest.X_SIZE * Simulation.AreaOfInterest.Y_SIZE + y * Simulation.AreaOfInterest.X_SIZE + x + 1;
			}

			[return: TupleElementNames(new string[]
			{
				"x",
				"y",
				"z"
			})]
			public static ValueTuple<int, int, int> ClampCellCoords(int x, int y, int z)
			{
				return new ValueTuple<int, int, int>(Simulation.AreaOfInterest.<ClampCellCoords>g__Clamp|15_0(x, Simulation.AreaOfInterest.X_SIZE), Simulation.AreaOfInterest.<ClampCellCoords>g__Clamp|15_0(y, Simulation.AreaOfInterest.Y_SIZE), Simulation.AreaOfInterest.<ClampCellCoords>g__Clamp|15_0(z, Simulation.AreaOfInterest.Z_SIZE));
			}

			[CompilerGenerated]
			internal static int <ClampCellCoords>g__Clamp|15_0(int v, int max)
			{
				return (v < 0) ? 0 : ((v >= max) ? (max - 1) : v);
			}

			internal const int SIZE_DEFAULT = 32;

			internal const int GRID_DEFAULT = 1024;

			internal const int MAX_SHARED_RADIUS = 300;

			internal static int X_SIZE = 1024;

			internal static int Y_SIZE = 1024;

			internal static int Z_SIZE = 1024;

			public static int CELL_SIZE = 32;
		}

		internal class Client : Simulation
		{
			private TimeSyncConfiguration TimeSyncConfig
			{
				get
				{
					bool flag = base.Topology == Topologies.Shared;
					TimeSyncConfiguration result;
					if (flag)
					{
						result = TimeSyncConfiguration.GetFromTickrate(base.RuntimeConfig.TickRate);
					}
					else
					{
						result = (this._projectConfig.TimeSynchronizationOverride ?? TimeSyncConfiguration.GetFromTickrate(base.RuntimeConfig.TickRate));
					}
					return result;
				}
			}

			internal unsafe NetConnection* ServerConnection
			{
				get
				{
					return this._server;
				}
			}

			public override Tick LatestServerTick
			{
				get
				{
					return this._history.IsEmpty ? default(Tick) : this._history.Points.Back().Tick;
				}
			}

			public double LatestServerTime
			{
				get
				{
					return (double)this.LatestServerTick * base.TickDeltaDouble;
				}
			}

			public bool IsConnectedToServer
			{
				get
				{
					return this._server != null;
				}
			}

			public unsafe NetAddress ServerAddress
			{
				get
				{
					return this.IsConnectedToServer ? this._server->RemoteAddress : default(NetAddress);
				}
			}

			public unsafe double RttToServer
			{
				get
				{
					return (this._server == null) ? 0.0 : this._server->RoundTripTime;
				}
			}

			public override PlayerRef LocalPlayer
			{
				get
				{
					return this._callbacks.LocalPlayerRef;
				}
			}

			public override IEnumerable<PlayerRef> ActivePlayers
			{
				get
				{
					Simulation.Client.<get_ActivePlayers>d__24 <get_ActivePlayers>d__ = new Simulation.Client.<get_ActivePlayers>d__24(-2);
					<get_ActivePlayers>d__.<>4__this = this;
					return <get_ActivePlayers>d__;
				}
			}

			private static string NullableToString<T>(T? value) where T : struct
			{
				bool flag = value != null;
				string result;
				if (flag)
				{
					T value2 = value.Value;
					result = value2.ToString();
				}
				else
				{
					result = "null";
				}
				return result;
			}

			internal Client(SimulationArgs args) : base(args)
			{
				ClientTimeProvider clientTimeProvider = new ClientTimeProvider();
				clientTimeProvider.OnReset(Clock.Local, new TimeProviderCallback(this.ResetClientSimulationState));
				this._time = clientTimeProvider;
				this._history = new Timeline(128);
				this._inputBuffer = new SimulationInput.Buffer(this._projectConfig);
				this._inputArray = new SimulationInput[128];
			}

			public void Connect(NetAddress address, byte[] token = null, byte[] uniqueId = null)
			{
				NetPeerGroup.Connect(this._netPeerGroup, address, token, uniqueId);
			}

			public void Connect(string ip, ushort port, byte[] token = null, byte[] uniqueId = null)
			{
				NetPeerGroup.Connect(this._netPeerGroup, ip, port, token, uniqueId);
			}

			protected unsafe override void NetworkConnected(NetConnection* connection)
			{
				this._server = connection;
			}

			protected unsafe override void NetworkDisconnected(NetConnection* connection, NetDisconnectReason reason)
			{
				try
				{
					Assert.Check(this._server == connection);
					this._server = null;
					this._callbacks.OnDisconnectedFromServer(reason);
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

			internal override void OnNetworkShutdown()
			{
				bool flag = this._server != null;
				if (flag)
				{
					NetPeerGroup.Disconnect(this._netPeerGroup, this._server, null);
					base.NetworkSend();
				}
				this._server = null;
			}

			internal void ResetRttToServer(double rtt = 0.0)
			{
				NetConnection.SetRtt(this._server, rtt);
			}

			internal override double GetPlayerRtt(PlayerRef player)
			{
				bool flag = player == this.LocalPlayer || player == PlayerRef.None;
				double result;
				if (flag)
				{
					result = this.RttToServer;
				}
				else
				{
					result = 0.0;
				}
				return result;
			}

			internal unsafe override PlayerRef Connection2Player(NetConnection* c)
			{
				return this.LocalPlayer;
			}

			internal unsafe override int Player2Connection(PlayerRef player)
			{
				return (int)this._server->LocalId.GroupIndex;
			}

			internal override void RecvPacket()
			{
				int tick = this._recvContext.Header.Tick;
				Simulation.TimeFeedback feedback = default(Simulation.TimeFeedback);
				feedback.Read(this._recvContext.Buffer);
				this._stateReplicator.RecvPacket();
				this._stateReceived = true;
				bool hasRuntimeConfig = base.HasRuntimeConfig;
				if (hasRuntimeConfig)
				{
					this._history.AddPoint(new TimelinePoint(tick, tick, base.TickDeltaDouble), base.TickDeltaDouble, false);
					this.UpdateObjectTimelines();
					this._time.OnFeedbackReceived(feedback);
				}
			}

			protected override void NetworkReceiveDone()
			{
				bool flag = this.LatestServerTick != this.PreviousServerTick;
				if (flag)
				{
					bool flag2 = this._time.IsRunning();
					if (flag2)
					{
						double num = (double)(this.LatestServerTick - this.PreviousServerTick) * base.TickDeltaDouble;
						bool flag3 = num <= 1.0;
						if (flag3)
						{
							TraceLogStream logTraceSnapshots = InternalLogStreams.LogTraceSnapshots;
							if (logTraceSnapshots != null)
							{
								logTraceSnapshots.Log(this, string.Format("received snapshot {0}", this.LatestServerTick));
							}
							this._time.OnSnapshotReceived(this.RttToServer, this.LatestServerTick);
						}
						else
						{
							TraceLogStream logTraceSnapshots2 = InternalLogStreams.LogTraceSnapshots;
							if (logTraceSnapshots2 != null)
							{
								logTraceSnapshots2.Log(this, string.Format("connection was lost for a long time, received snapshot {0}", this.LatestServerTick));
							}
							this.ResetRttToServer(Maths.Clamp(this.RttToServer - num, 0.15, 0.25));
							this._time.Reset(this.RttToServer, this.LatestServerTick);
						}
					}
					else
					{
						TraceLogStream logTraceSnapshots3 = InternalLogStreams.LogTraceSnapshots;
						if (logTraceSnapshots3 != null)
						{
							logTraceSnapshots3.Log(this, string.Format("received first snapshot {0}", this.LatestServerTick));
						}
						Assert.Check(base.HasRuntimeConfig);
						this._time.Configure(base.RuntimeConfig);
						this._time.Configure(this.TimeSyncConfig);
						this._time.SetPlayerIndex(this.LocalPlayer.AsIndex);
						bool clientsRecordFrameAndPacketTimingTraces = base.ProjectConfig.ClientsRecordFrameAndPacketTimingTraces;
						if (clientsRecordFrameAndPacketTimingTraces)
						{
							this._time.StartTrace();
						}
						this._time.Reset(this.RttToServer, this.LatestServerTick);
					}
					this.PreviousServerTick = this.LatestServerTick;
				}
			}

			internal override void WritePackets()
			{
				Topologies topology = base.Topology;
				Topologies topologies = topology;
				if (topologies != Topologies.ClientServer)
				{
					if (topologies == Topologies.Shared)
					{
						this._stateReplicator.SendPacket();
					}
				}
				else
				{
					this.WriteInput();
				}
			}

			private unsafe void WriteInput()
			{
				NetBitBuffer.Offset offset = NetBitBuffer.GetOffset(this._sendContext.Buffer);
				int num = Maths.Clamp(Mathf.CeilToInt((float)(this._server->Rtt / base.TickDeltaDouble)), 3, 6);
				bool flag = base.Config.InputTransferMode == SimulationConfig.InputTransferModes.LatestState;
				if (flag)
				{
					num = 1;
				}
				ValueTuple<SimulationInput[], int> sortedInputs = this.GetSortedInputs();
				SimulationInput[] item = sortedInputs.Item1;
				int item2 = sortedInputs.Item2;
				int i = Math.Max(0, item2 - num);
				int num2 = i;
				EngineProfiler.InputQueue(item2 - num2);
				NetBitBufferSerializer serializer = NetBitBufferSerializer.Writer(this._sendContext.Buffer);
				bool flag2 = base.Config.InputTransferMode == SimulationConfig.InputTransferModes.RedundancyUncompressed;
				if (flag2)
				{
					Assert.Check(this._config.InputTotalWordCount >= 1 && this._config.InputTotalWordCount <= 255);
					serializer.Buffer->PadToByteBoundary();
					serializer.Buffer->WriteByte((byte)this._config.InputTotalWordCount, 8);
					while (i < item2)
					{
						for (;;)
						{
							int offsetBits = serializer.Buffer->OffsetBits;
							serializer.Buffer->WriteBytesAligned((void*)item[i]._ptr, this._config.InputTotalWordCount * 4);
							bool overflowOrLessThanOneByteRemaining = serializer.Buffer->OverflowOrLessThanOneByteRemaining;
							if (!overflowOrLessThanOneByteRemaining)
							{
								break;
							}
							serializer.Buffer->OffsetBits = offsetBits;
							serializer.Buffer->ReplaceDataFromBlockWithTemp(serializer.Buffer->LengthBytes * 2);
						}
						Simulation.SendContext sendContext = this._sendContext;
						sendContext.Header.Inputs = sendContext.Header.Inputs + 1;
						i++;
					}
				}
				else
				{
					while (i < item2)
					{
						for (;;)
						{
							int offsetBits2 = serializer.Buffer->OffsetBits;
							bool flag3 = i == num2;
							if (flag3)
							{
								item[i].Serialize(this._inputRoot, this._config, serializer);
							}
							else
							{
								item[i].Serialize(item[i - 1], this._config, serializer);
							}
							bool overflowOrLessThanOneByteRemaining2 = serializer.Buffer->OverflowOrLessThanOneByteRemaining;
							if (!overflowOrLessThanOneByteRemaining2)
							{
								break;
							}
							serializer.Buffer->OffsetBits = offsetBits2;
							serializer.Buffer->ReplaceDataFromBlockWithTemp(serializer.Buffer->LengthBytes * 2);
						}
						Simulation.SendContext sendContext2 = this._sendContext;
						sendContext2.Header.Inputs = sendContext2.Header.Inputs + 1;
						i++;
					}
				}
				int length = offset.GetLength(this._sendContext.Buffer);
				this._fusionStatsManager.PendingSnapshot.AddToInputOutBandwidthStat((float)Maths.BytesRequiredForBits(length), false);
				EngineProfiler.InputSize(Maths.BytesRequiredForBits(length));
			}

			private ValueTuple<SimulationInput[], int> GetSortedInputs()
			{
				int item = this._inputBuffer.CopySortedTo(this._inputArray);
				return new ValueTuple<SimulationInput[], int>(this._inputArray, item);
			}

			internal unsafe override SimulationInput GetInput(Tick tick, PlayerRef player)
			{
				bool flag = player != this.LocalPlayer;
				SimulationInput result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool isResimulation = base.IsResimulation;
					if (isResimulation)
					{
						result = this._inputBuffer.Get(tick);
					}
					else
					{
						bool full = this._inputBuffer.Full;
						if (full)
						{
							bool flag2 = base.Topology == Topologies.Shared;
							if (!flag2)
							{
								return null;
							}
							this._inputBuffer.Clear();
						}
						SimulationInput simulationInput = this._inputPool.Acquire();
						simulationInput.Player = this.LocalPlayer;
						simulationInput.Header->Tick = base.Tick;
						simulationInput.Header->InterpTo = this._interpToPrev;
						simulationInput.Header->InterpFrom = this._interpFromPrev;
						simulationInput.Header->InterpAlpha = this._remoteAlphaPrev;
						this._callbacks.InvokeOnInput(simulationInput);
						bool flag3 = this._inputBuffer.Add(simulationInput, null);
						if (flag3)
						{
							result = simulationInput;
						}
						else
						{
							this._inputPool.Release(simulationInput);
							result = null;
						}
					}
				}
				return result;
			}

			protected override void BeforeFirstTick()
			{
				this._callbacks.OnClientStart();
			}

			protected unsafe override void BeforeUpdate()
			{
				SimulationRuntimeConfig* ptr;
				bool flag = base.TryGetStructData<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, out ptr);
				if (flag)
				{
					bool flag2 = ptr->MasterClient == this.LocalPlayer;
					bool flag3 = base.Topology == Topologies.Shared && this._previousWasMC != null && this._previousWasMC.Value != flag2;
					if (flag3)
					{
						base.UpdateSimulationStateForMasterClientObjects(flag2);
					}
					this._previousWasMC = new bool?(flag2);
				}
			}

			protected unsafe override int BeforeSimulation()
			{
				int num = 0;
				EngineProfiler.Begin("Simulation.Client.BeforeSimulation");
				bool stateReceived = this._stateReceived;
				if (stateReceived)
				{
					this._stateReceived = false;
					bool isConnectedToServer = this.IsConnectedToServer;
					if (isConnectedToServer)
					{
						EngineProfiler.RoundTripTime((float)this.RttToServer);
						this._fusionStatsManager.PendingSnapshot.AddToRoundTripTimeStat((float)this.RttToServer, true);
					}
					ValueTuple<SimulationInput[], int> sortedInputs = this.GetSortedInputs();
					SimulationInput[] item = sortedInputs.Item1;
					int item2 = sortedInputs.Item2;
					for (int i = 0; i < item2; i++)
					{
						bool flag = item[i].Header->Tick <= this.LatestServerTick;
						if (flag)
						{
							SimulationInput input;
							bool flag2 = this._inputBuffer.Remove(item[i].Header->Tick, out input);
							if (flag2)
							{
								this._inputPool.Release(input);
							}
						}
					}
					bool flag3 = base.Topology == Topologies.ClientServer;
					if (flag3)
					{
						num = Math.Max(0, base.Tick - this.LatestServerTick);
						this._tick = this.LatestServerTick;
						try
						{
							this.ResetPredictedObjectsToLatestServerState();
							bool flag4 = num > 0;
							if (flag4)
							{
								this.RunClientSideResimulationLoop(num);
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
				EngineProfiler.End();
				this.UpdateInterpolation();
				return num;
			}

			internal void ResetClientSimulationState()
			{
				TraceLogStream logTraceSnapshots = InternalLogStreams.LogTraceSnapshots;
				if (logTraceSnapshots != null)
				{
					logTraceSnapshots.Log(this, string.Format("(re)setting client simulation state to {0}", this.LatestServerTick));
				}
				this._tick = this.LatestServerTick;
				bool flag = base.Topology == Topologies.ClientServer;
				if (flag)
				{
					this.ResetPredictedObjectsToLatestServerState();
				}
				this._inputBuffer.Clear();
				this._stateReceived = false;
			}

			internal void ResetPredictedObjectsToLatestServerState()
			{
				this._callbacks.OnBeforeClientSidePredictionReset();
				int num = 0;
				foreach (NetworkObjectMeta networkObjectMeta in this._metaLookup.Values)
				{
					bool hasSnapshots = networkObjectMeta.HasSnapshots;
					if (hasSnapshots)
					{
						networkObjectMeta.SnapshotLatest.CopyTo(networkObjectMeta);
						networkObjectMeta.SnapshotLatest.CopyTo(networkObjectMeta.Previous);
						num++;
					}
				}
				this._callbacks.OnAfterClientSidePredictionReset();
			}

			private void RunClientSideResimulationLoop(int ticks)
			{
				EngineProfiler.Begin("Simulation.Client.RunClientSideResimulationLoop");
				Assert.Check(base.Tick == this.LatestServerTick);
				base.InvokeOnBeforeAllTicks(true, ticks);
				for (int i = 0; i < ticks; i++)
				{
					base.StepSimulation(SimulationStages.Resimulate, i == ticks - 1, i == 0, false);
				}
				Assert.Check(base.Tick == this.LatestServerTick + ticks);
				base.InvokeOnAfterAllTicks(true, ticks);
				EngineProfiler.End();
			}

			protected override void NoSimulation()
			{
				this.UpdateInterpolation();
			}

			private void UpdateInterpolation()
			{
				bool flag = !base.HasRuntimeConfig || this._history.IsEmpty;
				if (!flag)
				{
					double remote = this._time.Now().Remote;
					this._history.UpdateInterpolationParams(remote);
					this.UpdateObjectInterpolationParams(remote);
					this._interpFromPrev = this._interpFrom;
					this._interpToPrev = this._interpTo;
					this._remoteAlphaPrev = this._remoteAlpha;
					this._interpFrom = this._history.Params.From;
					this._interpTo = this._history.Params.To;
					this._remoteAlpha = this._history.Params.Alpha;
				}
			}

			private void UpdateObjectInterpolationParams(double now)
			{
				foreach (KeyValuePair<NetworkId, NetworkObjectMeta> keyValuePair in this._metaLookup)
				{
					NetworkObjectMeta value = keyValuePair.Value;
					bool flag = value.Instance != null;
					if (flag)
					{
						value.Timeline.UpdateInterpolationParams(now);
					}
				}
			}

			private void UpdateObjectTimelines()
			{
				foreach (KeyValuePair<NetworkId, NetworkObjectMeta> keyValuePair in this._metaLookup)
				{
					NetworkObjectMeta value = keyValuePair.Value;
					bool flag = value.Instance != null;
					if (flag)
					{
						value.AddLatestSnapshotToTimeline();
					}
				}
			}

			private unsafe NetConnection* _server;

			private bool _stateReceived;

			private Timeline _history;

			private SimulationInput.Buffer _inputBuffer;

			private SimulationInput[] _inputArray;

			private bool? _previousWasMC;

			public Tick PreviousServerTick;
		}

		internal enum ObjectChangeType
		{
			Created,
			Updated,
			Destroyed
		}

		internal struct PlayerRefMapping
		{
			public int ActorId;

			public PlayerRef PlayerRef;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct PlayerSimulationData
		{
			[FieldOffset(0)]
			public PlayerRef Player;

			[FieldOffset(4)]
			public NetworkId Object;

			[FieldOffset(8)]
			public int Actor;
		}

		internal class History
		{
			public Simulation.History.Entry Latest
			{
				get
				{
					return this._entryList.Tail;
				}
			}

			public Simulation.History.Entry Oldest
			{
				get
				{
					return this._entryList.Head;
				}
			}

			public History(int capacity)
			{
				this._entryList = new SimulationHistoryEntryList();
				for (int i = 0; i < capacity; i++)
				{
					this._entryList.AddLast(new Simulation.History.Entry());
				}
			}

			public Simulation.History.Entry Add(Tick tick, double time)
			{
				Simulation.History.Entry entry = this._entryList.RemoveHead();
				entry.Tick = tick;
				entry.Time = time;
				this._entryList.AddLast(entry);
				return entry;
			}

			private SimulationHistoryEntryList _entryList;

			internal class Entry
			{
				public Simulation.History.Entry Prev;

				public Simulation.History.Entry Next;

				public Tick Tick;

				public double Time;
			}
		}

		internal interface ICallbacks
		{
			void OnTick();

			void OnServerStart();

			void OnClientStart();

			unsafe SimulationMessageResult OnMessage(SimulationMessage* message);

			void OnAfterClientSidePredictionReset();

			void OnBeforeClientSidePredictionReset();

			void OnAfterTick();

			void OnBeforeTick();

			void OnAfterAllTicks(bool resimulation, int tickCount);

			void OnBeforeAllTicks(bool resimulation, int tickCount);

			void OnAfterSimulation();

			void OnBeforeSimulation(int forwardTickCount);

			void OnBeforeCopyPreviousState();

			void OnConnectedToServer();

			void OnDisconnectedFromServer(NetDisconnectReason reason);

			OnConnectionRequestReply OnConnectionRequest(NetAddress remoteAddress, byte[] token);

			void OnConnectionFailed(NetAddress remoteAddress, NetConnectFailedReason reason);

			void OnReliableData(PlayerRef player, ReliableId id, bool local, byte[] dataArray);

			void PlayerJoined(PlayerRef player);

			void PlayerLeft(PlayerRef player);

			void OnInternalConnectionAttempt(int attempt, int totalConnectionAttempts, out bool shouldChange, out NetAddress newAddress);

			bool IsSharedModeMasterClient { get; }

			bool CanReceivePlayerJoinLeaveCallbacks { get; }

			void ObjectStateAuthorityChanged(NetworkId id, bool gained);

			void ObjectInputAuthorityChanged(NetworkId id, bool gained);

			void ObjectIsSimulatedChanged(NetworkId id, bool simulated);

			void ObjectEnterAOI(PlayerRef player, NetworkId id);

			void ObjectExitAOI(PlayerRef player, NetworkId id);

			void ObjectChanged(PlayerRef player, NetworkObjectMeta obj, Simulation.ObjectChangeType changeType);

			PlayerRef LocalPlayerRef { get; }

			void RemoteObjectCreated(NetworkObjectMeta obj);

			bool RemoteObjectDestroyed(NetworkId id);

			void UpdateRemotePrefabs();

			void OnInput(SimulationInput input);

			void OnInputMissing(SimulationInput input);
		}

		private enum TargetObjectVerificationResult
		{
			Ok,
			TargetNotInterestedInObject
		}

		internal struct TimeFeedback
		{
			public TimeFeedback(SimulationConnection sc)
			{
				this.OffsetAvg = (float)sc._clientOffset.Smoothed(0.5);
				this.OffsetDev = (float)sc._clientOffset.MedianAbsDev;
				this.RecvDeltaAvg = (float)sc._packetRecvDelta.Smoothed(0.5);
				this.RecvDeltaDev = (float)sc._packetRecvDelta.MedianAbsDev;
			}

			public TimeFeedback(double offsetAvg, double offsetDev, double recvDeltaAvg, double recvDeltaDev)
			{
				this.OffsetAvg = (float)offsetAvg;
				this.OffsetDev = (float)offsetDev;
				this.RecvDeltaAvg = (float)recvDeltaAvg;
				this.RecvDeltaDev = (float)recvDeltaDev;
			}

			public unsafe void Write(NetBitBuffer* buffer)
			{
				buffer->WriteInt32VarLength(FloatUtils.Compress(this.OffsetAvg, 256), 6);
				buffer->WriteInt32VarLength(FloatUtils.Compress(this.OffsetDev, 256), 6);
				buffer->WriteInt32VarLength(FloatUtils.Compress(this.RecvDeltaAvg, 256), 6);
				buffer->WriteInt32VarLength(FloatUtils.Compress(this.RecvDeltaDev, 256), 6);
			}

			public unsafe void Read(NetBitBuffer* buffer)
			{
				this.OffsetAvg = FloatUtils.Decompress(buffer->ReadInt32VarLength(6), 256f);
				this.OffsetDev = FloatUtils.Decompress(buffer->ReadInt32VarLength(6), 256f);
				this.RecvDeltaAvg = FloatUtils.Decompress(buffer->ReadInt32VarLength(6), 256f);
				this.RecvDeltaDev = FloatUtils.Decompress(buffer->ReadInt32VarLength(6), 256f);
			}

			public float OffsetAvg;

			public float OffsetDev;

			public float RecvDeltaAvg;

			public float RecvDeltaDev;

			public const double SUSPEND_THRESHOLD = 1.0;

			private const int ACCURACY = 256;

			private const int BLOCK = 6;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct SimulationPacketHeader
		{
			public bool Equals(Simulation.SimulationPacketHeader other)
			{
				return this.SimulationMessages == other.SimulationMessages && this.Cells == other.Cells && this.ObjectUpdates == other.ObjectUpdates && this.ObjectDestroys == other.ObjectDestroys && this.Inputs == other.Inputs;
			}

			public override bool Equals(object obj)
			{
				bool result;
				if (obj is Simulation.SimulationPacketHeader)
				{
					Simulation.SimulationPacketHeader other = (Simulation.SimulationPacketHeader)obj;
					result = this.Equals(other);
				}
				else
				{
					result = false;
				}
				return result;
			}

			public override int GetHashCode()
			{
				int num = 397;
				num = (num * 397 ^ this.SimulationMessages.GetHashCode());
				num = (num * 397 ^ this.Cells.GetHashCode());
				num = (num * 397 ^ this.ObjectUpdates.GetHashCode());
				num = (num * 397 ^ this.ObjectDestroys.GetHashCode());
				return num * 397 ^ this.Inputs.GetHashCode();
			}

			public unsafe void Write(NetBitBuffer* buffer)
			{
				Assert.Check(sizeof(Simulation.SimulationPacketHeader) == 8);
				Assert.Always(buffer->PacketType == NetPacketType.NotifyData, "buffer->PacketType   == NetPacketType.NotifyData");
				Simulation.SimulationPacketHeader simulationPacketHeader = this;
				buffer->WriteUInt64AtOffset((ulong)(*(long*)(&simulationPacketHeader)), 112, 64);
			}

			public unsafe void Read(NetBitBuffer* buffer)
			{
				Assert.Check(sizeof(Simulation.SimulationPacketHeader) == 8);
				Assert.Always(buffer->PacketType == NetPacketType.NotifyData, "buffer->PacketType   == NetPacketType.NotifyData");
				Assert.Always(buffer->OffsetBits == 112, "buffer->OffsetBits             ==  NetNotifyHeader.SIZE_IN_BITS");
				Simulation.SimulationPacketHeader simulationPacketHeader = this;
				*(long*)(&simulationPacketHeader) = (long)buffer->ReadUInt64(64);
				this = simulationPacketHeader;
			}

			public override string ToString()
			{
				return string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}, {8}: {9}, {10}: {11}", new object[]
				{
					"SimulationMessages",
					this.SimulationMessages,
					"Cells",
					this.Cells,
					"ObjectUpdates",
					this.ObjectUpdates,
					"ObjectDestroys",
					this.ObjectDestroys,
					"Inputs",
					this.Inputs,
					"Tick",
					this.Tick
				});
			}

			public const int WIRE_SIZE_IN_BYTES = 8;

			public const int WIRE_SIZE_IN_BITS = 64;

			[FieldOffset(0)]
			public byte SimulationMessages;

			[FieldOffset(1)]
			public byte Cells;

			[FieldOffset(1)]
			public byte Inputs;

			[FieldOffset(2)]
			public byte ObjectUpdates;

			[FieldOffset(3)]
			public byte ObjectDestroys;

			[FieldOffset(4)]
			public int Tick;
		}

		internal class RecvContext
		{
			public RecvContext(Simulation simulation)
			{
				this._simulation = simulation;
			}

			public unsafe void Init(SimulationConnection connection, NetBitBuffer* buffer)
			{
				Assert.Check(connection != null);
				Assert.Check(buffer != null);
				this.Connection = connection;
				this.Player = this._simulation.Connection2Player(connection);
				this.Buffer = buffer;
				this.Header.Read(this.Buffer);
			}

			public void Done()
			{
				this.Connection = null;
				this.Buffer = null;
			}

			private Simulation _simulation;

			public PlayerRef Player;

			public Simulation.SimulationPacketHeader Header;

			public unsafe NetBitBuffer* Buffer;

			public SimulationConnection Connection;
		}

		internal class SendContext
		{
			public bool IsWriting
			{
				get
				{
					return this.Buffer != null;
				}
			}

			public SendContext(Simulation simulation)
			{
				this._simulation = simulation;
			}

			public unsafe bool Init(SimulationConnection connection, Tick tick)
			{
				Assert.Check(this.Buffer == null, "Buffer should be null");
				Assert.Check(this.Envelope == null, "Envelope should be null");
				Assert.Check(this.Connection == null, "Connection should be null");
				this.Header = default(Simulation.SimulationPacketHeader);
				this.ObjPrev = 0;
				this.Tick = tick;
				this.Connection = connection;
				this.Player = this._simulation.Connection2Player(connection);
				bool flag = !this._simulation.NetworkGetBuffer(this.Connection.Connection, out this.Buffer);
				bool result;
				if (flag)
				{
					TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
					if (logTraceNetwork != null)
					{
						logTraceNetwork.Error("Out of packets");
					}
					this.Reset();
					result = false;
				}
				else
				{
					this.Buffer->OffsetBits += 64;
					this.Envelope = SimulationPacketEnvelope.Alloc(this._simulation);
					bool flag2 = this.Envelope == null;
					if (flag2)
					{
						TraceLogStream logTraceNetwork2 = InternalLogStreams.LogTraceNetwork;
						if (logTraceNetwork2 != null)
						{
							logTraceNetwork2.Error("Out of envelopes");
						}
						NetBitBuffer.ReleaseRef(ref this.Buffer);
						this.Reset();
						result = false;
					}
					else
					{
						this.Envelope->Tick = this.Tick;
						result = (this.Buffer != null);
					}
				}
				return result;
			}

			public void Send()
			{
				Assert.Check(this.Connection != null);
				this.Header.Write(this.Buffer);
				this._simulation.NetworkSendBuffer(this.Connection.Connection, this.Buffer, this.Envelope);
				this.Reset();
			}

			private void Reset()
			{
				this.Tick = default(Tick);
				this.Header = default(Simulation.SimulationPacketHeader);
				this.Player = default(PlayerRef);
				this.Buffer = null;
				this.Envelope = null;
				this.Connection = null;
			}

			private Simulation _simulation;

			public Simulation.SimulationPacketHeader Header;

			public unsafe NetBitBuffer* Buffer;

			public unsafe SimulationPacketEnvelope* Envelope;

			public Tick Tick;

			public PlayerRef Player;

			public SimulationConnection Connection;

			public int ObjPrev;
		}

		internal class Server : Simulation
		{
			public override Tick LatestServerTick
			{
				get
				{
					return this._tick;
				}
			}

			public override PlayerRef LocalPlayer
			{
				get
				{
					return base.IsPlayer ? this._callbacks.LocalPlayerRef : PlayerRef.None;
				}
			}

			internal unsafe override double GetPlayerRtt(PlayerRef player)
			{
				bool flag = this.LocalPlayer == player;
				double result;
				if (flag)
				{
					result = 0.0;
				}
				else
				{
					SimulationConnection simulationConnection;
					bool flag2 = this._playersConnections.TryGetValue(player, out simulationConnection);
					if (flag2)
					{
						result = simulationConnection.Connection->RoundTripTime;
					}
					else
					{
						result = 0.0;
					}
				}
				return result;
			}

			internal Server(SimulationArgs args) : base(args)
			{
				this._time = new ServerTimeProvider();
				this._time.Configure(this.CreateRuntimeConfiguration());
				bool flag = args.ResumeState != null && args.ResumeTick != 0 && args.ResumeNetworkId.IsValid;
				if (flag)
				{
					TraceLogStream logTraceHostMigration = InternalLogStreams.LogTraceHostMigration;
					if (logTraceHostMigration != null)
					{
						logTraceHostMigration.Log(string.Format("Received Remote state: Tick={0}, NetworkId={1}", args.ResumeTick, args.ResumeNetworkId));
					}
					this._time.Reset(0.0, args.ResumeTick);
					this._isResume = true;
					this._tick = args.ResumeTick;
					this._idCounter = Math.Max(1023U, args.ResumeNetworkId.Raw);
					this.ReadHostMigrationData(args.ResumeState);
				}
			}

			internal void Disconnect(PlayerRef player, byte[] token)
			{
				bool flag = base.PlayerValid(player);
				if (flag)
				{
					SimulationConnection simulationConnection;
					bool flag2 = this._playersConnections.TryGetValue(player, out simulationConnection);
					if (flag2)
					{
						NetPeerGroup.DisconnectInternal(this._netPeerGroup, simulationConnection.Connection, NetDisconnectReason.Requested, token);
					}
				}
			}

			internal unsafe void Disconnect(NetAddress address)
			{
				foreach (SimulationConnection simulationConnection in this._connections.Values)
				{
					bool flag = simulationConnection.Connection->Address.Equals(address);
					if (flag)
					{
						NetPeerGroup.DisconnectInternal(this._netPeerGroup, simulationConnection.Connection, NetDisconnectReason.Requested, null);
						break;
					}
				}
			}

			protected override void AfterSimulation()
			{
				bool flag = (base.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
				if (flag)
				{
					foreach (SimulationConnection sc in this._connections.Values)
					{
						base.AOI_UpdateAreaOfInterest(sc);
					}
				}
			}

			protected override int BeforeSimulation()
			{
				bool flag = base.IsPlayer && !base.PlayerValid(this.LocalPlayer);
				if (flag)
				{
					base.PlayerAdd(this.LocalPlayer, null);
				}
				this._interpFromPrev = this._interpFrom;
				this._interpToPrev = this._interpTo;
				this._remoteAlphaPrev = this._remoteAlpha;
				this._interpFrom = this._tick.Next(base.TickStride);
				this._interpTo = this._tick.Next(base.TickStride);
				this._remoteAlpha = 0f;
				return 0;
			}

			protected unsafe override void NetworkDisconnected(NetConnection* connection, NetDisconnectReason reason)
			{
			}

			internal override void RecvPacket()
			{
				Topologies topology = base.Topology;
				Topologies topologies = topology;
				if (topologies != Topologies.ClientServer)
				{
					if (topologies == Topologies.Shared)
					{
						this.ReadStateTick();
						this._stateReplicator.RecvPacket();
					}
				}
				else
				{
					this.ReadInput();
				}
			}

			protected unsafe override void NetworkConnected(NetConnection* connection)
			{
				SimulationConnection simulationConnectionByIndex = base.GetSimulationConnectionByIndex((int)connection->LocalConnectionId.GroupIndex);
				bool flag = this._globalInterestObjects != null;
				if (flag)
				{
					foreach (NetworkId id in this._globalInterestObjects)
					{
						simulationConnectionByIndex.GetObjectData(id, true, true);
					}
				}
			}

			internal override void WritePackets()
			{
				bool flag = this._time.IsRunning();
				if (flag)
				{
					double num = (double)(base.Tick - this._sendContext.Connection._latestTickAcknowledged) * base.TickDeltaDouble;
					bool flag2 = num > 1.0;
					if (flag2)
					{
						this._sendContext.Connection.ResetTimeFeedback();
					}
				}
				Simulation.TimeFeedback timeFeedback = new Simulation.TimeFeedback(this._sendContext.Connection);
				timeFeedback.Write(this._sendContext.Buffer);
				this._stateReplicator.SendPacket();
			}

			private SimulationRuntimeConfig CreateRuntimeConfiguration()
			{
				SimulationRuntimeConfig simulationRuntimeConfig = default(SimulationRuntimeConfig);
				simulationRuntimeConfig.ServerMode = this._mode;
				simulationRuntimeConfig.PlayerMaxCount = this._config.PlayerCount;
				simulationRuntimeConfig.Topology = this._config.Topology;
				simulationRuntimeConfig.TickRate = Fusion.TickRate.Resolve(this._config.TickRateSelection);
				bool isServer = base.IsServer;
				if (isServer)
				{
					simulationRuntimeConfig.HostPlayer = this.LocalPlayer;
				}
				bool flag = this._mode == SimulationModes.Host;
				if (flag)
				{
					simulationRuntimeConfig.TickRate.Server = simulationRuntimeConfig.TickRate.Client;
				}
				return simulationRuntimeConfig;
			}

			internal unsafe void SpawnRuntimeConfiguration()
			{
				SimulationRuntimeConfig* ptr = base.AllocateStruct<SimulationRuntimeConfig>(NetworkId.RuntimeConfig, 0, null);
				*ptr = this.CreateRuntimeConfiguration();
			}

			protected override void BeforeUpdate()
			{
				bool flag = !base.HasObject(NetworkId.RuntimeConfig);
				if (flag)
				{
					this.SpawnRuntimeConfiguration();
				}
			}

			internal unsafe void CreateInternalStateObjects(PlayerRef sceneInfoStateAuth)
			{
				Assert.Check(base.IsServer);
				NetworkObjectMeta networkObjectMeta = base.AllocateStruct(NetworkId.SceneInfo, 13, null);
				*networkObjectMeta.StateAuthority = sceneInfoStateAuth;
				NetworkObjectMeta networkObjectMeta2 = base.AllocateStruct(NetworkId.PhysicsInfo, 10, null);
				*networkObjectMeta2.StateAuthority = sceneInfoStateAuth;
			}

			protected override void BeforeFirstTick()
			{
				this.CreateInternalStateObjects(PlayerRef.None);
				bool flag = base.Mode == SimulationModes.Host;
				if (flag)
				{
					Assert.Check(this.LocalPlayer.IsRealPlayer);
					base.GetPlayerSimulationData(this.LocalPlayer, true);
				}
				this._callbacks.OnServerStart();
			}

			internal unsafe override SimulationInput GetInput(Tick tick, PlayerRef player)
			{
				bool flag = !base.PlayerValid(player);
				SimulationInput result;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = base.IsPlayer && this.LocalPlayer == player;
					if (flag2)
					{
						SimulationInput simulationInput = this._inputPool.Acquire();
						simulationInput.Player = this.LocalPlayer;
						simulationInput.Header->Tick = base.Tick;
						simulationInput.Header->InterpTo = base.TickPrevious;
						simulationInput.Header->InterpFrom = Math.Max(0, base.TickPrevious - base.TickStride);
						simulationInput.Header->InterpAlpha = this._localAlphaPrev;
						this._callbacks.InvokeOnInput(simulationInput);
						result = simulationInput;
					}
					else
					{
						SimulationConnection simulationConnectionForPlayer = base.GetSimulationConnectionForPlayer(player);
						bool flag3 = simulationConnectionForPlayer == null;
						if (flag3)
						{
							result = null;
						}
						else
						{
							SimulationInput simulationInput2 = simulationConnectionForPlayer._inputs.Get(tick);
							bool flag4 = simulationInput2 == null;
							if (flag4)
							{
								bool flag5 = this._config.Topology == Topologies.ClientServer;
								if (flag5)
								{
									simulationInput2 = this._inputPool.Acquire();
									simulationInput2.Player = player;
									simulationInput2.Header->Tick = tick;
									SimulationInputHeader lastUsedInputHeader = simulationConnectionForPlayer._inputs.GetLastUsedInputHeader();
									simulationInput2.Header->InterpTo = lastUsedInputHeader.InterpTo;
									simulationInput2.Header->InterpFrom = lastUsedInputHeader.InterpFrom;
									simulationInput2.Header->InterpAlpha = lastUsedInputHeader.InterpAlpha;
									try
									{
										this._callbacks.InvokeOnInputMissing(simulationInput2);
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
								double? insertTime = simulationConnectionForPlayer._inputs.GetInsertTime(tick);
								Assert.Check(insertTime != null);
								bool flag6 = insertTime != null;
								if (flag6)
								{
									simulationConnectionForPlayer.InputReceiveDelta(tick, insertTime.Value, this._updateTime);
								}
								SimulationInput simulationInput3;
								simulationConnectionForPlayer._inputs.Remove(tick, out simulationInput3);
							}
							bool flag7 = this._config.Topology == Topologies.Shared;
							if (flag7)
							{
								bool flag8 = simulationInput2 != null;
								if (flag8)
								{
									this._inputPool.Release(simulationInput2);
								}
								result = null;
							}
							else
							{
								result = simulationInput2;
							}
						}
					}
				}
				return result;
			}

			private unsafe void ReadInput()
			{
				SimulationConnection connection = this._recvContext.Connection;
				NetBitBuffer.Offset offset = NetBitBuffer.GetOffset(this._recvContext.Buffer);
				NetBitBufferSerializer serializer = NetBitBufferSerializer.Reader(this._recvContext.Buffer);
				int inputs = (int)this._recvContext.Header.Inputs;
				bool flag = inputs > 0;
				if (flag)
				{
					bool flag2 = base.Config.InputTransferMode == SimulationConfig.InputTransferModes.RedundancyUncompressed;
					if (flag2)
					{
						serializer.Buffer->SeekToByteBoundary();
						int num = (int)serializer.Buffer->ReadByte(8);
						int i = 0;
						while (i < inputs)
						{
							Assert.Check(serializer.Buffer->IsOnEvenByte);
							SimulationInputHeader simulationInputHeader = *(SimulationInputHeader*)(serializer.Buffer->Data + serializer.Buffer->OffsetBytes / 8);
							bool flag3 = simulationInputHeader.Tick > base.Tick;
							if (flag3)
							{
								bool flag4 = this._inputReadTarget == null;
								if (flag4)
								{
									this._inputReadTarget = this._inputPool.Acquire();
								}
								this._inputReadTarget.Player = this._recvContext.Player;
								serializer.Buffer->ReadBytesAligned((void*)this._inputReadTarget._ptr, num * 4);
								bool full = connection._inputs.Full;
								if (!full)
								{
									bool flag5 = connection._inputs.Add(this._inputReadTarget, new double?(this._updateTime));
									if (flag5)
									{
										this._inputReadTarget = null;
									}
								}
							}
							else
							{
								serializer.Buffer->Advance(num * 4 * 8, false);
								double expected;
								bool flag6 = this._tickUpdateTimes.TryGetValue(simulationInputHeader.Tick, out expected);
								if (flag6)
								{
									connection.InputReceiveDelta(simulationInputHeader.Tick, this._updateTime, expected);
								}
								else
								{
									connection.InputReceiveDelta(simulationInputHeader.Tick, this._updateTime, (double)simulationInputHeader.Tick * base.TickDeltaDouble);
								}
							}
							IL_1E8:
							i++;
							continue;
							goto IL_1E8;
						}
					}
					else
					{
						SimulationInput inputRoot = this._inputRoot;
						inputRoot.Clear(this._config.InputTotalWordCount);
						for (int j = 0; j < inputs; j++)
						{
							SimulationInput simulationInput = this._inputPool.Acquire();
							simulationInput.Player = this._recvContext.Player;
							simulationInput.Serialize(inputRoot, this._config, serializer);
							inputRoot.CopyFrom(simulationInput, this._config.InputTotalWordCount);
							bool flag7 = simulationInput.Header->Tick > base.Tick;
							if (flag7)
							{
								bool full2 = connection._inputs.Full;
								if (full2)
								{
									this._inputPool.Release(simulationInput);
								}
								else
								{
									bool flag8 = !connection._inputs.Add(simulationInput, new double?(this._updateTime));
									if (flag8)
									{
										this._inputPool.Release(simulationInput);
									}
								}
							}
							else
							{
								double expected2;
								bool flag9 = this._tickUpdateTimes.TryGetValue(simulationInput.Header->Tick, out expected2);
								if (flag9)
								{
									connection.InputReceiveDelta(simulationInput.Header->Tick, this._updateTime, expected2);
								}
								else
								{
									connection.InputReceiveDelta(simulationInput.Header->Tick, this._updateTime, (double)simulationInput.Header->Tick * base.TickDeltaDouble);
								}
								this._inputPool.Release(simulationInput);
							}
						}
					}
				}
				int length = offset.GetLength(this._recvContext.Buffer);
				this._fusionStatsManager.PendingSnapshot.AddToInputInBandwidthStat((float)Maths.BytesRequiredForBits(length), false);
			}

			private void ReadStateTick()
			{
				SimulationConnection connection = this._recvContext.Connection;
				int tick = this._recvContext.Header.Tick;
				double expected;
				bool flag = this._tickUpdateTimes.TryGetValue(tick, out expected);
				if (flag)
				{
					connection.InputReceiveDelta(tick, this._updateTime, expected);
				}
				else
				{
					connection.InputReceiveDelta(tick, this._updateTime, (double)tick * base.TickDeltaDouble);
				}
			}

			private Dictionary<NetworkId, NetworkObjectHeaderSnapshot> NetworkObjectMap { get; } = new Dictionary<NetworkId, NetworkObjectHeaderSnapshot>();

			internal unsafe ValueTuple<Dictionary<NetworkId, NetworkObjectHeaderPtr>, Dictionary<NetworkId, List<NetworkId>>> GetResumeObjectHeader()
			{
				Dictionary<NetworkId, NetworkObjectHeaderPtr> dictionary = new Dictionary<NetworkId, NetworkObjectHeaderPtr>();
				Dictionary<NetworkId, List<NetworkId>> dictionary2 = new Dictionary<NetworkId, List<NetworkId>>();
				foreach (KeyValuePair<NetworkId, NetworkObjectHeaderSnapshot> keyValuePair in this.NetworkObjectMap)
				{
					NetworkObjectHeader networkObjectHeader = *keyValuePair.Value.Header;
					bool flag = !networkObjectHeader.Id.IsValid || networkObjectHeader.Id.Raw <= 1023U;
					if (!flag)
					{
						dictionary.Add(networkObjectHeader.Id, keyValuePair.Value.HeaderPtr);
						bool flag2 = !networkObjectHeader.NestingRoot.IsValid;
						if (!flag2)
						{
							List<NetworkId> list;
							bool flag3 = !dictionary2.TryGetValue(networkObjectHeader.NestingRoot, out list);
							if (flag3)
							{
								dictionary2.Add(networkObjectHeader.NestingRoot, list = new List<NetworkId>());
							}
							list.Add(networkObjectHeader.Id);
						}
					}
				}
				return new ValueTuple<Dictionary<NetworkId, NetworkObjectHeaderPtr>, Dictionary<NetworkId, List<NetworkId>>>(dictionary, dictionary2);
			}

			internal void DisposeHostMigration()
			{
				object hmLock = this._hmLock;
				lock (hmLock)
				{
					foreach (NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot in this.NetworkObjectMap.Values)
					{
						networkObjectHeaderSnapshot.Release();
					}
					this.NetworkObjectMap.Clear();
					NetBitBuffer.ReleaseRef(ref this._hostMigrationWriteBuffer);
				}
			}

			internal unsafe int WriteHostMigrationData(ref byte[] target, int targetBytes)
			{
				Assert.Check(target.Length <= 32768);
				Assert.Check(targetBytes <= 32768);
				bool flag = this._metaMigration.Count == 0 && this._metaMigrationRemoved.Count == 0;
				int result;
				if (flag)
				{
					TraceLogStream logTraceHostMigration = InternalLogStreams.LogTraceHostMigration;
					if (logTraceHostMigration != null)
					{
						logTraceHostMigration.Log("No migration data to write");
					}
					result = 0;
				}
				else
				{
					bool flag2 = this._hostMigrationWriteBuffer == null;
					if (flag2)
					{
						this._hostMigrationWriteBuffer = NetBitBuffer.Allocate(0, 65536);
					}
					this._hostMigrationWriteBuffer->Clear();
					this._hostMigrationWriteBuffer->OffsetBits = 0;
					NetworkId id = this._metaMigration.Head.Id;
					bool flag3 = true;
					int num = 0;
					while (this._metaMigration.Count > 0 && (flag3 || this._metaMigration.Head.Id != id) && this._hostMigrationWriteBuffer->OffsetBits < targetBytes * 8)
					{
						flag3 = false;
						int offsetBits = this._hostMigrationWriteBuffer->OffsetBits;
						NetworkObjectMeta networkObjectMeta = this._metaMigration.RemoveHead();
						NetworkObjectHeaderSnapshotRef migration = networkObjectMeta.Migration;
						NetworkId.Write(this._hostMigrationWriteBuffer, networkObjectMeta.Id);
						this._hostMigrationWriteBuffer->WriteInt32((int)networkObjectMeta.WordCount, 32);
						Span<int> raw = networkObjectMeta.Raw;
						Span<int> raw2 = migration.Raw;
						*raw2[0] = *raw[0];
						int num2 = 1;
						for (int i = num2; i < (int)networkObjectMeta.WordCount; i++)
						{
							bool flag4 = *raw[i] == *raw2[i];
							if (!flag4)
							{
								this._hostMigrationWriteBuffer->WriteInt32VarLength(i - num2);
								this._hostMigrationWriteBuffer->WriteInt32VarLength(*raw[i]);
								*raw2[i] = *raw[i];
								num2 = i;
							}
						}
						TraceLogStream logTraceHostMigration2 = InternalLogStreams.LogTraceHostMigration;
						if (logTraceHostMigration2 != null)
						{
							logTraceHostMigration2.Log(string.Format("Migration {0}/{1}: ", networkObjectMeta.Id, networkObjectMeta.Migration.Header.Id) + string.Format("WordCount={0}/{1}, ", networkObjectMeta.WordCount, networkObjectMeta.Migration.Header.WordCount) + string.Format("Type={0}/{1}, ", networkObjectMeta.Type, networkObjectMeta.Migration.Header.Type) + string.Format("CRC={0}", migration.SnapshotCRC));
						}
						bool flag5 = num2 == 1;
						if (flag5)
						{
							this._hostMigrationWriteBuffer->OffsetBits = offsetBits;
						}
						else
						{
							this._hostMigrationWriteBuffer->WriteInt32VarLength(int.MaxValue);
							TraceLogStream logTraceHostMigration3 = InternalLogStreams.LogTraceHostMigration;
							if (logTraceHostMigration3 != null)
							{
								logTraceHostMigration3.Log(string.Format("Write end mark at {0}", this._hostMigrationWriteBuffer->OffsetBits));
							}
							num++;
						}
						this._metaMigration.AddLast(networkObjectMeta);
					}
					while (this._metaMigrationRemoved.Count > 0 && this._hostMigrationWriteBuffer->OffsetBits < targetBytes * 8)
					{
						NetworkId networkId = this._metaMigrationRemoved.Dequeue();
						NetworkId.Write(this._hostMigrationWriteBuffer, networkId);
						this._hostMigrationWriteBuffer->WriteInt32(0, 32);
						TraceLogStream logTraceHostMigration4 = InternalLogStreams.LogTraceHostMigration;
						if (logTraceHostMigration4 != null)
						{
							logTraceHostMigration4.Log(string.Format("Migration {0}: Removed", networkId));
						}
						num++;
					}
					bool flag6 = num == 0;
					if (flag6)
					{
						TraceLogStream logTraceHostMigration5 = InternalLogStreams.LogTraceHostMigration;
						if (logTraceHostMigration5 != null)
						{
							logTraceHostMigration5.Log("No migration data written");
						}
						result = 0;
					}
					else
					{
						this._hostMigrationWriteBuffer->PadToByteBoundary();
						bool flag7 = target.Length < this._hostMigrationWriteBuffer->OffsetBytes;
						if (flag7)
						{
							target = new byte[this._hostMigrationWriteBuffer->OffsetBytes * 2];
						}
						byte[] array;
						byte* destination;
						if ((array = target) == null || array.Length == 0)
						{
							destination = null;
						}
						else
						{
							destination = &array[0];
						}
						Native.MemCpy((void*)destination, (void*)this._hostMigrationWriteBuffer->Data, this._hostMigrationWriteBuffer->OffsetBytes);
						array = null;
						TraceLogStream logTraceHostMigration6 = InternalLogStreams.LogTraceHostMigration;
						if (logTraceHostMigration6 != null)
						{
							logTraceHostMigration6.Log(string.Format("WriteHostMigrationData: {0}={1}, {2}={3}", new object[]
							{
								"_metaMigration",
								this._metaMigration.Count,
								"count",
								num
							}));
						}
						result = this._hostMigrationWriteBuffer->OffsetBytes;
					}
				}
				return result;
			}

			private void ReadHostMigrationData(byte[] data)
			{
				foreach (NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot in this.NetworkObjectMap.Values)
				{
					networkObjectHeaderSnapshot.Release();
				}
				this.NetworkObjectMap.Clear();
				Simulation.Server.ProcessHostMigrationData(data, this.NetworkObjectMap, this._allocator);
			}

			internal unsafe static void ProcessHostMigrationData(byte[] data, Dictionary<NetworkId, NetworkObjectHeaderSnapshot> networkObjectMap, Allocator allocator)
			{
				bool flag = data == null || data.Length == 0;
				if (!flag)
				{
					NetBitBuffer netBitBuffer = default(NetBitBuffer);
					TraceLogStream logTraceHostMigration = InternalLogStreams.LogTraceHostMigration;
					if (logTraceHostMigration != null)
					{
						logTraceHostMigration.Log(string.Format("ProcessHostMigrationData: {0}", data.Length));
					}
					fixed (byte[] array = data)
					{
						byte* data2;
						if (data == null || array.Length == 0)
						{
							data2 = null;
						}
						else
						{
							data2 = &array[0];
						}
						netBitBuffer.Data = (ulong*)data2;
						netBitBuffer.OffsetBits = 0;
						netBitBuffer.LengthBytes = data.Length;
						while (!netBitBuffer.DoneOrOverflow && netBitBuffer.CanRead(36))
						{
							NetworkId networkId = NetworkId.Read(&netBitBuffer);
							int num = netBitBuffer.ReadInt32(32);
							bool flag2 = num > 0;
							bool flag3 = flag2 && !allocator.CanAllocSize(num * 4);
							if (flag3)
							{
								TraceLogStream logTraceHostMigration2 = InternalLogStreams.LogTraceHostMigration;
								if (logTraceHostMigration2 != null)
								{
									logTraceHostMigration2.Error(string.Format("Migration {0}: Invalid WordCount {1}", networkId, num));
								}
								break;
							}
							try
							{
								NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot;
								bool flag4 = networkObjectMap.TryGetValue(networkId, out networkObjectHeaderSnapshot);
								bool flag5 = flag4 && !flag2;
								if (flag5)
								{
									networkObjectHeaderSnapshot.Release();
									networkObjectMap.Remove(networkId);
									TraceLogStream logTraceHostMigration3 = InternalLogStreams.LogTraceHostMigration;
									if (logTraceHostMigration3 != null)
									{
										logTraceHostMigration3.Log(string.Format("Migration {0}: Removed", networkId));
									}
								}
								else
								{
									bool flag6 = !flag4;
									if (flag6)
									{
										networkObjectHeaderSnapshot = new NetworkObjectHeaderSnapshot(allocator);
										networkObjectHeaderSnapshot.Init(num);
										*networkObjectHeaderSnapshot.Header = new NetworkObjectHeader(networkId, checked((short)num), 0, default(NetworkObjectTypeId), default(NetworkId), default(NetworkObjectNestingKey), (NetworkObjectHeaderFlags)0);
										networkObjectMap[networkId] = networkObjectHeaderSnapshot;
									}
									Span<int> raw = networkObjectHeaderSnapshot.Raw;
									int num2 = 1;
									while (!netBitBuffer.DoneOrOverflow)
									{
										bool flag7 = !netBitBuffer.CanRead(32);
										if (flag7)
										{
											TraceLogStream logTraceHostMigration4 = InternalLogStreams.LogTraceHostMigration;
											if (logTraceHostMigration4 != null)
											{
												logTraceHostMigration4.Warn(string.Format("Migration {0}: Not enough data to read offset", networkId));
											}
											break;
										}
										int num3 = netBitBuffer.ReadInt32VarLength();
										bool flag8 = num3 == int.MaxValue;
										if (flag8)
										{
											TraceLogStream logTraceHostMigration5 = InternalLogStreams.LogTraceHostMigration;
											if (logTraceHostMigration5 != null)
											{
												logTraceHostMigration5.Log(string.Format("Migration {0}: End Mark at {1}", networkId, netBitBuffer.OffsetBits));
											}
											break;
										}
										num2 += num3;
										bool flag9 = num2 < 0 || num2 >= (int)networkObjectHeaderSnapshot.Header.WordCount;
										if (flag9)
										{
											TraceLogStream logTraceHostMigration6 = InternalLogStreams.LogTraceHostMigration;
											if (logTraceHostMigration6 != null)
											{
												logTraceHostMigration6.Error(string.Format("WordOffset {0} exceeds WordCount {1} for {2}", num3, networkObjectHeaderSnapshot.Header.WordCount, networkObjectHeaderSnapshot.Header.Id));
											}
											while (!netBitBuffer.DoneOrOverflow && netBitBuffer.CanRead(32) && netBitBuffer.ReadInt32VarLength() != 2147483647)
											{
											}
											break;
										}
										bool flag10 = !netBitBuffer.CanRead(32);
										if (flag10)
										{
											TraceLogStream logTraceHostMigration7 = InternalLogStreams.LogTraceHostMigration;
											if (logTraceHostMigration7 != null)
											{
												logTraceHostMigration7.Warn(string.Format("Migration {0}: Not enough data to read data", networkId));
											}
											break;
										}
										int num4 = netBitBuffer.ReadInt32VarLength();
										*raw[num2] = num4;
									}
									TraceLogStream logTraceHostMigration8 = InternalLogStreams.LogTraceHostMigration;
									if (logTraceHostMigration8 != null)
									{
										logTraceHostMigration8.Log(string.Format("Migration {0}: ", networkObjectHeaderSnapshot.Header.Id) + string.Format("WordCount={0}, ", networkObjectHeaderSnapshot.Header.WordCount) + string.Format("Type={0}, ", networkObjectHeaderSnapshot.Header.Type) + string.Format("CRC={0}", networkObjectHeaderSnapshot.BuildCRC()));
									}
								}
							}
							catch (Exception message)
							{
								TraceLogStream logTraceHostMigration9 = InternalLogStreams.LogTraceHostMigration;
								if (logTraceHostMigration9 != null)
								{
									logTraceHostMigration9.Log(string.Format("Migration {0}: Failed to read data", networkId));
								}
								TraceLogStream logTraceHostMigration10 = InternalLogStreams.LogTraceHostMigration;
								if (logTraceHostMigration10 != null)
								{
									logTraceHostMigration10.Error(message);
								}
								break;
							}
						}
						TraceLogStream logTraceHostMigration11 = InternalLogStreams.LogTraceHostMigration;
						if (logTraceHostMigration11 != null)
						{
							logTraceHostMigration11.Log(string.Format("Total {0} on storage", networkObjectMap.Count));
						}
					}
				}
			}

			private SimulationInput _inputReadTarget = null;

			private unsafe NetBitBuffer* _hostMigrationWriteBuffer;

			private const int HostMigrationBufferSize = 65536;

			private const int HostMigrationMaxTransferBufferSize = 32768;

			private readonly object _hmLock = new object();
		}

		internal class StateReplicator
		{
			public StateReplicator(Simulation simulation)
			{
				this._simulation = simulation;
				this._dataConsistency = this._simulation._config.ObjectDataConsistency;
				this._aoiQuery = new List<NetworkObjectMeta.List>();
				this._notUsingAreaOfInterest = ((this._simulation.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.None);
			}

			public void SendPacket()
			{
				bool flag = this._simulation.Topology == Topologies.Shared || this._simulation.Mode != SimulationModes.Client;
				if (flag)
				{
					Simulation.SendContext sendContext = this._simulation._sendContext;
					NetBitBuffer.Offset offset = NetBitBuffer.GetOffset(sendContext.Buffer);
					sendContext.Header.Tick = this._simulation.Tick;
					this.WriteObjectDestroys();
					this.WriteStructs();
					bool schedulingEnabled = this._simulation.Config.SchedulingEnabled;
					if (schedulingEnabled)
					{
						Profiler.BeginSample("WriteUsingScheduling");
						this.WriteUsingScheduling();
						Profiler.EndSample();
					}
					else
					{
						this.WriteUsingAllObjects();
					}
					this._simulation._fusionStatsManager.PendingSnapshot.AddToOutObjectUpdatesStat((int)sendContext.Header.ObjectUpdates, false);
				}
			}

			public void RecvPacket()
			{
				bool flag = this._simulation.Topology == Topologies.Shared || this._simulation.Mode == SimulationModes.Client;
				if (flag)
				{
					Simulation.RecvContext recvContext = this._simulation._recvContext;
					this.ReadObjectDestroys();
					this.ReadObjectUpdates();
					this._simulation._fusionStatsManager.PendingSnapshot.AddToInObjectUpdatesStat((int)recvContext.Header.ObjectUpdates, false);
				}
			}

			private unsafe NetworkObjectHeader ReadHeader(Simulation.RecvContext rc, NetworkId id)
			{
				NetworkObjectHeader result = default(NetworkObjectHeader);
				Span<int> span = new Span<int>((void*)(&result), 20);
				*span[0] = (int)id.Raw;
				for (int i = 1; i < 20; i++)
				{
					bool flag = rc.Buffer->ReadBoolean();
					if (flag)
					{
						*span[i] = (int)Maths.ZigZagDecode(rc.Buffer->ReadInt64VarLength(6));
						this._simulation._fusionStatsManager.PendingSnapshot.AddToWordsReadCountStat(1, false);
					}
				}
				return result;
			}

			private unsafe void SkipObjectData(Simulation.RecvContext rc, NetworkBufferSerializerInfo[] serializers, int word, bool clearChangedWords)
			{
				Assert.Check(serializers != null);
				if (clearChangedWords)
				{
					this._changedWords.Clear();
				}
				int num;
				while ((num = rc.Buffer->ReadInt32VarLength(4)) > 0)
				{
					word += num;
					this._changedWords.Add(word);
					bool flag = word >= 0 && word < serializers.Length && serializers[word].Serializer != null;
					if (flag)
					{
						word = serializers[word].Serializer.Skip(rc, word);
					}
					else
					{
						rc.Buffer->ReadInt64VarLength(6);
					}
				}
			}

			private unsafe void SkipObject(Simulation.RecvContext rc, NetworkId id, NetworkObjectMeta meta, bool skipHeader)
			{
				this._changedWords.Clear();
				NetworkBufferSerializerInfo[] array = (meta != null) ? meta.Serializers : null;
				bool flag = array == null || array.Length == 0;
				if (flag)
				{
					array = NetworkObjectMeta.GetSerializers(rc.Connection.PendingDeleteMainTRSP.Contains(id));
				}
				int word = 0;
				if (skipHeader)
				{
					for (int i = 1; i < 20; i++)
					{
						bool flag2 = rc.Buffer->ReadBoolean();
						if (flag2)
						{
							this._changedWords.Add(i);
							rc.Buffer->ReadInt64VarLength(6);
						}
					}
					word = 19;
				}
				this.SkipObjectData(rc, array, word, false);
			}

			private void ForceResendChangedWords(Simulation.RecvContext rc, NetworkId id)
			{
				Assert.Check(this._simulation.Topology == Topologies.Shared);
				this._changedWords.Clear();
			}

			private unsafe bool ReadObjectDataIntoPtr(NetworkObjectMeta meta, Span<int> p, int word)
			{
				Simulation.RecvContext recvContext = this._simulation._recvContext;
				NetworkBufferSerializerInfo[] serializers = meta.Serializers;
				NetBitBuffer* buffer = recvContext.Buffer;
				int num = 0;
				int num2 = serializers.Length;
				int num3 = 0;
				bool flag = false;
				while (buffer->MoreToRead && (num = buffer->ReadInt32VarLength(4)) > 0)
				{
					Assert.Check(!buffer->DoneOrOverflow);
					bool doneOrOverflow = buffer->DoneOrOverflow;
					bool result;
					if (doneOrOverflow)
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Error("Buffer DoneOrOverflow");
						}
						result = false;
					}
					else
					{
						word += num;
						bool flag2 = word < 0 || word >= (int)meta.WordCount;
						if (flag2)
						{
							LogStream logError = InternalLogStreams.LogError;
							if (logError != null)
							{
								LogStream logStream = logError.Once(ref this._loggedWordCheck);
								if (logStream != null)
								{
									logStream.Log(string.Format("Word check: Id={0}, Type={1}, SA={2}, {3} > {4}. {5}", new object[]
									{
										meta.Id,
										meta.Type,
										*meta.StateAuthority,
										word,
										meta.WordCount,
										recvContext.Player
									}));
								}
							}
							result = false;
						}
						else
						{
							bool flag3 = word < 7;
							if (flag3)
							{
								int num4 = (int)Maths.ZigZagDecode(buffer->ReadInt64VarLength(6));
								bool flag4 = num4 != *p[word];
								if (flag4)
								{
									LogStream logError2 = InternalLogStreams.LogError;
									if (logError2 != null)
									{
										LogStream logStream2 = logError2.Once(ref flag);
										if (logStream2 != null)
										{
											logStream2.Log(string.Format("tried to write over header's read-only part (word:{0}, value:{1}, orig-value:{2}, header: {3})", new object[]
											{
												word,
												num4,
												*p[word],
												*meta.Header
											}));
										}
									}
								}
								continue;
							}
							bool flag5 = word < num2 && serializers[word].Serializer != null;
							if (flag5)
							{
								word = serializers[word].Serializer.Read(recvContext, meta, serializers[word], p, word);
							}
							else
							{
								int num5 = (int)Maths.ZigZagDecode(buffer->ReadInt64VarLength(6));
								bool overflow = buffer->Overflow;
								if (overflow)
								{
									DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
									if (logDebug2 != null)
									{
										logDebug2.Error("Buffer Overflow");
									}
									return false;
								}
								*p[word] = num5;
							}
							num3++;
							continue;
						}
					}
					return result;
				}
				this._simulation._fusionStatsManager.PendingSnapshot.AddToWordsReadCountStat(num3, false);
				return true;
			}

			private unsafe void ReadObjectUpdates()
			{
				Simulation.RecvContext recvContext = this._simulation._recvContext;
				int num = 0;
				for (int i = 0; i < (int)recvContext.Header.ObjectUpdates; i++)
				{
					int offsetBits = recvContext.Buffer->OffsetBits;
					Assert.Check(!recvContext.Buffer->DoneOrOverflow);
					int word = 0;
					int num2 = num + Maths.ZigZagDecode(recvContext.Buffer->ReadInt32VarLength(6));
					NetworkId networkId = new NetworkId((uint)num2);
					num = num2;
					bool flag = recvContext.Buffer->ReadBoolean();
					NetworkObjectMeta networkObjectMeta;
					bool flag2 = this._simulation.TryGetMeta(networkId, out networkObjectMeta);
					Assert.Check(flag2 == (networkObjectMeta != null));
					bool flag3 = false;
					int offsetBits2 = recvContext.Buffer->OffsetBits;
					bool flag4 = (networkObjectMeta != null && networkId == NetworkId.SceneInfo && this._simulation.IsLocalSimulationStateAuthority(networkObjectMeta.Header)) || (this._simulation.Topology == Topologies.ClientServer && recvContext.Connection.ObjectData_IsDestroyUnconfirmed(networkId).GetValueOrDefault());
					if (flag4)
					{
						this.SkipObject(recvContext, networkId, networkObjectMeta, flag);
					}
					else
					{
						NetworkObjectHeader networkObjectHeader = default(NetworkObjectHeader);
						bool flag5 = flag;
						if (flag5)
						{
							networkObjectHeader = this.ReadHeader(recvContext, networkId);
							word = 19;
							bool flag6 = !flag2;
							if (flag6)
							{
								networkObjectMeta = this._simulation.AllocateObject(networkObjectHeader);
								this._simulation._callbacks.RemoteObjectCreated(networkObjectMeta);
								flag3 = (flag2 = true);
								bool flag7 = this._simulation.Topology == Topologies.Shared;
								if (flag7)
								{
									recvContext.Connection.GetObjectData(networkObjectMeta.Id, true, false).Status = NetworkObjectConnectionDataStatus.CreatedConfirmed;
								}
							}
							else
							{
								bool flag8 = networkObjectHeader.WordCount != networkObjectMeta.WordCount;
								if (flag8)
								{
									LogStream logError = InternalLogStreams.LogError;
									if (logError != null)
									{
										logError.Log(string.Format("Unconfirmed header word count mismatch: {0} != {1}", networkObjectHeader.WordCount, networkObjectMeta.WordCount));
									}
								}
							}
						}
						else
						{
							bool flag9 = !flag2;
							if (flag9)
							{
								LogStream logWarn = InternalLogStreams.LogWarn;
								if (logWarn != null)
								{
									LogStream logStream = logWarn.Once(ref this._logged0);
									if (logStream != null)
									{
										logStream.Log(string.Format("Object does not exist: {0}, but is marked as confirmed. This indicates invalid data from the remote host", networkId));
									}
								}
								this.SkipObject(recvContext, networkId, null, flag);
								goto IL_929;
							}
						}
						Assert.Check(flag2);
						NetworkObjectHeaderSnapshotRef snapshot = networkObjectMeta.NextSnapshot(recvContext.Header.Tick);
						bool flag10 = flag;
						if (flag10)
						{
							*snapshot.Header = networkObjectHeader;
						}
						bool flag11 = !this.ReadObjectDataIntoPtr(networkObjectMeta, snapshot.Raw, word);
						if (flag11)
						{
							recvContext.Buffer->OffsetBits = offsetBits2;
							this.SkipObject(recvContext, networkId, networkObjectMeta, flag);
						}
						else
						{
							bool flag12 = this._simulation.Topology != Topologies.Shared;
							if (flag12)
							{
								Span<int> behaviourChangedTickArray = networkObjectMeta.GetBehaviourChangedTickArray(snapshot);
								for (int j = 0; j < (int)snapshot.Header.BehaviourCount; j++)
								{
									*behaviourChangedTickArray[j] = recvContext.Header.Tick;
								}
							}
							this._simulation._fusionStatsManager.ObjectStatisticsManager.AddToNetworkObjectInBandwidth(networkObjectMeta.Id, (float)Maths.BytesRequiredForBits(recvContext.Buffer->OffsetBits - offsetBits), false);
							this._simulation._fusionStatsManager.ObjectStatisticsManager.AddToNetworkObjectInPackets(networkObjectMeta.Id, 1, false);
							bool? flag13 = null;
							bool? flag14 = null;
							bool flag15 = this._simulation.Topology == Topologies.Shared;
							if (flag15)
							{
								SimulationConnection simulationConnectionForPlayer = this._simulation.GetSimulationConnectionForPlayer(this._simulation.LocalPlayer);
								NetworkObjectConnectionData objectData = simulationConnectionForPlayer.GetObjectData(networkObjectMeta.Id, true, false);
								bool flag16 = this._simulation.IsStateAuthority(snapshot.Header.StateAuthority, this._simulation.LocalPlayer);
								bool flag17 = this._simulation.IsStateAuthority(*networkObjectMeta.StateAuthority, this._simulation.LocalPlayer);
								bool flag18 = *networkObjectMeta.StateAuthority != snapshot.Header.StateAuthority;
								bool flag19 = flag16 && (!flag17 || flag3);
								if (flag19)
								{
									snapshot.CopyTo(networkObjectMeta);
									snapshot.CopyTo(networkObjectMeta.Previous);
									snapshot.CopyTo(networkObjectMeta.Shadow);
									simulationConnectionForPlayer.SetActive(objectData, networkObjectMeta);
									bool flag20 = !networkObjectMeta.IsStruct;
									if (flag20)
									{
										flag14 = new bool?(true);
										this._simulation._callbacks.ObjectIsSimulatedChanged(networkObjectMeta.Id, true);
									}
								}
								else
								{
									bool flag21 = !flag16 && flag17;
									if (flag21)
									{
										networkObjectMeta.Timeline.Clear();
										simulationConnectionForPlayer.SetIdle(objectData);
										bool flag22 = !networkObjectMeta.IsStruct;
										if (flag22)
										{
											snapshot.CopyTo(networkObjectMeta);
											snapshot.CopyTo(networkObjectMeta.Previous);
											flag14 = new bool?(false);
											this._simulation._callbacks.ObjectIsSimulatedChanged(networkObjectMeta.Id, false);
										}
									}
									else
									{
										bool flag23 = flag18;
										if (flag23)
										{
											flag14 = new bool?(false);
										}
									}
								}
							}
							else
							{
								bool flag24 = snapshot.Header.InputAuthority == this._simulation.LocalPlayer && (*networkObjectMeta.InputAuthority != this._simulation.LocalPlayer || flag3);
								if (flag24)
								{
									flag13 = new bool?(true);
									this._simulation._callbacks.ObjectIsSimulatedChanged(networkObjectMeta.Id, true);
								}
								else
								{
									bool flag25 = snapshot.Header.InputAuthority != this._simulation.LocalPlayer && *networkObjectMeta.InputAuthority == this._simulation.LocalPlayer;
									if (flag25)
									{
										flag13 = new bool?(false);
										this._simulation._callbacks.ObjectIsSimulatedChanged(networkObjectMeta.Id, false);
									}
								}
							}
							bool flag26 = flag3;
							if (flag26)
							{
								snapshot.CopyTo(networkObjectMeta);
								snapshot.CopyTo(networkObjectMeta.Previous);
								bool flag27 = this._simulation.Topology == Topologies.Shared && this._simulation.IsStateAuthority(snapshot.Header.StateAuthority, this._simulation.LocalPlayer);
								if (flag27)
								{
									snapshot.CopyTo(networkObjectMeta.Shadow);
								}
							}
							else
							{
								bool flag28 = snapshot.Header.Type.IsStruct || !this._simulation.IsSimulated(networkObjectMeta);
								if (flag28)
								{
									snapshot.CopyTo(networkObjectMeta);
								}
							}
							bool flag29 = flag3 && networkObjectMeta.Type == NetworkObjectTypeId.PlayerData;
							if (flag29)
							{
								Simulation.PlayerSimulationData* dataAs = networkObjectMeta.GetDataAs<Simulation.PlayerSimulationData>();
								Assert.Check<PlayerRef>(dataAs->Player.IsRealPlayer, dataAs->Player);
								this._simulation._invokeJoinedLeaveQueue.Enqueue(new ValueTuple<PlayerRef, bool>(dataAs->Player, true));
								bool flag30 = !this._simulation.IsServer;
								if (flag30)
								{
									this._simulation._players.Add(dataAs->Player);
								}
							}
							this._simulation._callbacks.ObjectChanged(recvContext.Player, networkObjectMeta, flag3 ? Simulation.ObjectChangeType.Created : Simulation.ObjectChangeType.Updated);
							bool flag31 = networkObjectMeta.Instance;
							if (flag31)
							{
								NetworkBehaviour[] networkedBehaviours = networkObjectMeta.Instance.NetworkedBehaviours;
								bool flag32 = (networkObjectMeta.PlayerData.Flags & NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) > (NetworkObjectHeaderPlayerDataFlags)0;
								bool flag33 = (snapshot.Header.PlayerData.Flags & NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) > (NetworkObjectHeaderPlayerDataFlags)0;
								bool flag34 = flag32 != flag33;
								if (flag34)
								{
									bool flag35 = flag33;
									if (flag35)
									{
										try
										{
											bool flag36 = this._simulation.IsClient && this._simulation.HasRuntimeConfig;
											if (flag36)
											{
												networkObjectMeta.Timeline.Clear();
											}
											for (int k = 0; k < networkedBehaviours.Length; k++)
											{
												IInterestEnter interestEnter = networkedBehaviours[k] as IInterestEnter;
												bool flag37 = interestEnter != null;
												if (flag37)
												{
													interestEnter.InterestEnter(this._simulation.LocalPlayer);
												}
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
									else
									{
										try
										{
											for (int l = 0; l < networkedBehaviours.Length; l++)
											{
												IInterestExit interestExit = networkedBehaviours[l] as IInterestExit;
												bool flag38 = interestExit != null;
												if (flag38)
												{
													interestExit.InterestExit(this._simulation.LocalPlayer);
												}
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
									}
								}
							}
							networkObjectMeta.PlayerData = networkObjectMeta.SnapshotLatest.Header.PlayerData;
							bool flag39 = flag14 != null;
							if (flag39)
							{
								this._simulation._callbacks.ObjectStateAuthorityChanged(networkObjectMeta.Id, flag14.Value);
							}
							bool flag40 = flag13 != null;
							if (flag40)
							{
								this._simulation._callbacks.ObjectInputAuthorityChanged(networkObjectMeta.Id, flag13.Value);
							}
						}
					}
					IL_929:;
				}
			}

			public void OnObjectSpawnedLocal(NetworkId id)
			{
				bool flag = this._simulation.IsClient || (this._simulation.Config.SchedulingEnabled && !this._simulation.Config.AreaOfInterestEnabled);
				if (flag)
				{
					bool isClient = this._simulation.IsClient;
					if (isClient)
					{
						Assert.Check(this._simulation.Topology == Topologies.Shared);
						SimulationConnection simulationConnectionForPlayer = this._simulation.GetSimulationConnectionForPlayer(this._simulation.LocalPlayer);
						if (simulationConnectionForPlayer != null)
						{
							simulationConnectionForPlayer.GetObjectData(id, true, false);
						}
					}
					else
					{
						foreach (SimulationConnection simulationConnection in this._simulation.Connections)
						{
							simulationConnection.GetObjectData(id, true, false);
						}
					}
				}
			}

			private unsafe void WriteObjectDestroys()
			{
				Simulation.SendContext sendContext = this._simulation._sendContext;
				int num = Math.Min((int)this._simulation._config.MaxObjectDestroysSentPerPacket, sendContext.Connection.DestroysPending);
				int num2 = 0;
				for (;;)
				{
					NetworkId id;
					bool flag = num2 < num && sendContext.Connection.DestroyedNextId(out id);
					if (!flag)
					{
						break;
					}
					id.Write(sendContext.Buffer);
					Simulation.SendContext sendContext2 = sendContext;
					sendContext2.Header.ObjectDestroys = sendContext2.Header.ObjectDestroys + 1;
					sendContext.Envelope->AddObjectPacketData(this._simulation, id, default(Tick), NetworkObjectPacketFlags.Destroy);
					num2++;
				}
			}

			private void ReadObjectDestroys()
			{
				Simulation.RecvContext recvContext = this._simulation._recvContext;
				int objectDestroys = (int)recvContext.Header.ObjectDestroys;
				for (int i = 0; i < objectDestroys; i++)
				{
					NetworkId id = NetworkId.Read(recvContext.Buffer);
					bool flag = this._simulation._callbacks.RemoteObjectDestroyed(id);
					if (!flag)
					{
						this._simulation.Destroy(id, NetworkObjectDestroyFlags.DestroyState, recvContext.Player);
					}
				}
			}

			private void WriteUsingAllObjects()
			{
				EngineProfiler.Begin("WriteUsingAllObjects");
				foreach (KeyValuePair<NetworkId, NetworkObjectMeta> keyValuePair in this._simulation._metaLookup)
				{
					Assert.Check<NetworkId, NetworkId>(keyValuePair.Key == keyValuePair.Value.Id, keyValuePair.Key, keyValuePair.Value.Id);
					bool flag = (keyValuePair.Value.Flags & NetworkObjectHeaderFlags.Struct) != NetworkObjectHeaderFlags.Struct;
					if (flag)
					{
						Simulation.StateReplicator.WriteResult writeResult = this.ScanAndWriteObject(keyValuePair.Value, null);
						bool flag2 = writeResult == Simulation.StateReplicator.WriteResult.PacketFull;
						if (flag2)
						{
							break;
						}
					}
				}
				EngineProfiler.End();
			}

			private void WriteLevelUsingScheduling(int level, ref NetworkObjectConnectionData sent)
			{
				Simulation.SendContext sendContext = this._simulation._sendContext;
				Tick tick = this._simulation.Tick;
				NetworkObjectConnectionData networkObjectConnectionData = sendContext.Connection.ObjectPriorityList.GetLevelList(level).Head;
				bool isClient = this._simulation.IsClient;
				while (networkObjectConnectionData != null)
				{
					NetworkObjectConnectionData networkObjectConnectionData2 = networkObjectConnectionData;
					networkObjectConnectionData = networkObjectConnectionData.Next;
					bool flag = networkObjectConnectionData2.MetaCache == null || networkObjectConnectionData2.MetaCache.Id != networkObjectConnectionData2.Id;
					if (flag)
					{
						networkObjectConnectionData2.MetaCache = this._simulation.GetMeta(networkObjectConnectionData2.Id);
					}
					NetworkObjectMeta metaCache = networkObjectConnectionData2.MetaCache;
					bool flag2 = isClient && !this._simulation.IsLocalSimulationStateAuthority(metaCache.Header);
					if (flag2)
					{
						sendContext.Connection.SetIdle(networkObjectConnectionData2);
					}
					else
					{
						bool flag3 = metaCache.ScannedTick == tick;
						if (flag3)
						{
							int num = networkObjectConnectionData2.UniqueDataChanges.MaxTick;
							bool flag4 = num < metaCache.ChangedTick;
							if (flag4)
							{
								num = metaCache.ChangedTick;
							}
							bool flag5 = num > 0 && num <= networkObjectConnectionData2.TickSent;
							if (flag5)
							{
								continue;
							}
						}
						Simulation.StateReplicator.WriteResult writeResult = this.ScanAndWriteObject(metaCache, networkObjectConnectionData2);
						bool flag6 = writeResult == Simulation.StateReplicator.WriteResult.Written;
						if (flag6)
						{
							sendContext.Connection.ObjectPriorityList.RemoveSent(networkObjectConnectionData2);
							networkObjectConnectionData2.PriorityLevel = metaCache.GetPriority(sendContext.Player);
							networkObjectConnectionData2.Next = sent;
							sent = networkObjectConnectionData2;
						}
					}
				}
			}

			private void WriteUsingScheduling()
			{
				Simulation.SendContext sendContext = this._simulation._sendContext;
				NetworkObjectConnectionData networkObjectConnectionData = null;
				for (int i = 0; i < 5; i++)
				{
					this.WriteLevelUsingScheduling(i, ref networkObjectConnectionData);
				}
				sendContext.Connection.ObjectPriorityList.IncreasePriorities();
				while (networkObjectConnectionData != null)
				{
					NetworkObjectConnectionData networkObjectConnectionData2 = networkObjectConnectionData;
					networkObjectConnectionData = networkObjectConnectionData.Next;
					networkObjectConnectionData2.Next = null;
					networkObjectConnectionData2.Prev = null;
					sendContext.Connection.ObjectPriorityList.Add(networkObjectConnectionData2);
				}
			}

			internal bool HasObjectInterest(PlayerRef player, NetworkId id)
			{
				bool notUsingAreaOfInterest = this._notUsingAreaOfInterest;
				bool result;
				if (notUsingAreaOfInterest)
				{
					result = true;
				}
				else
				{
					Simulation simulation = this._simulation;
					SimulationConnection simulationConnection = (simulation != null) ? simulation.GetSimulationConnectionForPlayer(player) : null;
					bool flag = simulationConnection != null && !simulationConnection.AreaOfInterestHasBeenUpdated;
					if (flag)
					{
						result = true;
					}
					else
					{
						NetworkObjectConnectionData networkObjectConnectionData;
						bool flag2 = simulationConnection.TryGetObjectData(id, out networkObjectConnectionData) && networkObjectConnectionData.HasAnyPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.AllInterestFlags);
						result = (flag2 && NetworkObjectMeta.IsActive(networkObjectConnectionData.PriorityLevel));
					}
				}
				return result;
			}

			internal void UpdateChangedStructSet()
			{
				this._simulation.TryGetMeta(NetworkId.RuntimeConfig, out this._runtimeConfig);
				this._simulation.TryGetMeta(NetworkId.SceneInfo, out this._sceneInfo);
				this._simulation.TryGetMeta(NetworkId.PhysicsInfo, out this._physicsInfo);
				foreach (NetworkId id in this._simulation._structs)
				{
					NetworkObjectMeta meta;
					bool flag = this._simulation.TryGetMeta(id, out meta);
					if (flag)
					{
						this.ScanStructForChanges(meta);
					}
				}
			}

			private void WriteStructs()
			{
				Simulation.SendContext sendContext = this._simulation._sendContext;
				SimulationConnection connection = sendContext.Connection;
				bool flag = this._runtimeConfig != null;
				if (flag)
				{
					NetworkObjectConnectionData objectData = connection.GetObjectData(this._runtimeConfig.Id, true, false);
					bool flag2 = !Simulation.StateReplicator.CheckNothingToSendTicks(this._runtimeConfig, objectData);
					if (flag2)
					{
						this.WriteObject(this._runtimeConfig, objectData);
					}
				}
				bool flag3 = this._sceneInfo != null;
				if (flag3)
				{
					NetworkObjectConnectionData objectData2 = connection.GetObjectData(this._sceneInfo.Id, true, false);
					bool flag4 = !Simulation.StateReplicator.CheckNothingToSendTicks(this._sceneInfo, objectData2);
					if (flag4)
					{
						this.WriteObject(this._sceneInfo, objectData2);
					}
				}
				bool flag5 = this._physicsInfo != null;
				if (flag5)
				{
					NetworkObjectConnectionData objectData3 = connection.GetObjectData(this._physicsInfo.Id, true, false);
					bool flag6 = !Simulation.StateReplicator.CheckNothingToSendTicks(this._physicsInfo, objectData3);
					if (flag6)
					{
						this.WriteObject(this._physicsInfo, objectData3);
					}
				}
				bool flag7 = this._simulation._structsVersion != connection.ActiveStructsVersion;
				if (flag7)
				{
					connection.ActiveStructsVersion = this._simulation._structsVersion;
					connection.ActiveStructs.Clear();
					foreach (NetworkId id in this._simulation._structs)
					{
						NetworkObjectMeta networkObjectMeta;
						bool flag8 = this._simulation.TryGetMeta(id, out networkObjectMeta) && networkObjectMeta.Type == NetworkObjectTypeId.PlayerData;
						if (flag8)
						{
							NetworkObjectConnectionData objectData4 = connection.GetObjectData(id, true, false);
							objectData4.MetaCache = networkObjectMeta;
							connection.ActiveStructs.Add(objectData4);
						}
					}
				}
				int num = 0;
				while (num < 5 && connection.ActiveStructs.Count > 0)
				{
					List<NetworkObjectConnectionData> activeStructs = connection.ActiveStructs;
					SimulationConnection simulationConnection = connection;
					int num2 = simulationConnection.ActiveStructsIndex + 1;
					simulationConnection.ActiveStructsIndex = num2;
					NetworkObjectConnectionData networkObjectConnectionData = activeStructs[num2 % connection.ActiveStructs.Count];
					int num3 = networkObjectConnectionData.UniqueDataChanges.MaxTick;
					bool flag9 = num3 < networkObjectConnectionData.MetaCache.ChangedTick;
					if (flag9)
					{
						num3 = networkObjectConnectionData.MetaCache.ChangedTick;
					}
					bool flag10 = num3 > 0 && num3 <= networkObjectConnectionData.TickSent;
					if (!flag10)
					{
						bool flag11 = this.WriteObject(networkObjectConnectionData.MetaCache, networkObjectConnectionData) == Simulation.StateReplicator.WriteResult.PacketFull;
						if (flag11)
						{
							break;
						}
					}
					num++;
				}
			}

			private unsafe bool ScanStructForChanges(NetworkObjectMeta meta)
			{
				bool result = false;
				bool flag = meta.ScannedTick < this._simulation.Tick;
				if (flag)
				{
					meta.ScannedTick = this._simulation.Tick;
					int* changes = meta.Changes;
					Span<int> raw = meta.Shadow.Raw;
					Span<int> raw2 = meta.Raw;
					short wordCount = meta.WordCount;
					for (int i = 0; i < (int)wordCount; i++)
					{
						bool flag2 = *raw[i] != *raw2[i];
						if (flag2)
						{
							*raw[i] = *raw2[i];
							changes[i] = this._simulation.Tick;
							result = true;
						}
					}
				}
				return result;
			}

			private unsafe Simulation.StateReplicator.WriteResult ScanAndWriteObject(NetworkObjectMeta meta, NetworkObjectConnectionData data)
			{
				bool flag = !(this._simulation is Simulation.Server) && !this._simulation.IsLocalSimulationStateAuthority(meta.Header);
				Simulation.StateReplicator.WriteResult result;
				if (flag)
				{
					result = Simulation.StateReplicator.WriteResult.NothingToSend;
				}
				else
				{
					int num = 0;
					bool flag2 = (meta.Flags & NetworkObjectHeaderFlags.Struct) == (NetworkObjectHeaderFlags)0 && BehaviourUtils.IsAlive(meta.Instance) && meta.Instance.NetworkedBehaviours != null && meta.Instance.NetworkedBehaviours.Length != 0;
					bool flag3 = meta.ScannedTick < this._simulation.Tick;
					if (flag3)
					{
						meta.ScannedTick = this._simulation.Tick;
						int* changes = meta.Changes;
						Tick changedTick = meta.ChangedTick;
						Span<int> raw = meta.Shadow.Raw;
						Span<int> raw2 = meta.Raw;
						int i;
						for (i = 0; i < 20; i++)
						{
							bool flag4 = *raw[i] != *raw2[i];
							if (flag4)
							{
								*raw[i] = *raw2[i];
								changes[i] = (changedTick = this._simulation.Tick);
								num++;
							}
						}
						bool flag5 = flag2;
						if (flag5)
						{
							bool flag6 = this._simulation.Topology == Topologies.Shared;
							NetworkBehaviour[] networkedBehaviours = meta.Instance.NetworkedBehaviours;
							Span<int> behaviourChangedTickArray = meta.BehaviourChangedTickArray;
							Assert.Check(networkedBehaviours.Length == (int)meta.BehaviourCount);
							for (int j = 0; j < networkedBehaviours.Length; j++)
							{
								Assert.Check(networkedBehaviours[j].WordOffset >= i);
								i = networkedBehaviours[j].WordOffset;
								int num2 = networkedBehaviours[j].WordOffset + networkedBehaviours[j].WordCount;
								while (i < num2)
								{
									bool flag7 = *raw[i] != *raw2[i];
									if (flag7)
									{
										*raw[i] = *raw2[i];
										changes[i] = (changedTick = this._simulation.Tick);
										num++;
										bool flag8 = flag6;
										if (flag8)
										{
											*behaviourChangedTickArray[j] = this._simulation.Tick;
										}
									}
									i++;
								}
							}
						}
						short wordCount = meta.WordCount;
						while (i < (int)wordCount)
						{
							bool flag9 = *raw[i] != *raw2[i];
							if (flag9)
							{
								*raw[i] = *raw2[i];
								changes[i] = (changedTick = this._simulation.Tick);
								num++;
							}
							i++;
						}
						meta.ChangedTick = changedTick;
					}
					this._simulation._fusionStatsManager.PendingSnapshot.AddToWordsWrittenCountStat(num, false);
					result = this.WriteObject(meta, data);
				}
				return result;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private unsafe static void WriteWord(NetBitBuffer* buffer, ReadOnlySpan<int> ptr, int word, int previous)
			{
				Assert.Check(word - previous > 0);
				long num = (long)(*ptr[word]);
				num = (num >> 63 ^ num << 1);
				uint num2 = (uint)(word - previous);
				ulong num3 = 0UL;
				int num4 = 0;
				int num5 = (Maths.BitScanReverse(num2) + 4) / 4;
				num3 |= 1UL << (num5 - 1 & 31) << (num4 & 31);
				num4 += num5;
				num3 |= (ulong)((ulong)num2 << num4);
				num4 += num5 * 4;
				num5 = (Maths.BitScanReverse(num) + 6) / 6;
				num3 |= 1UL << (num5 - 1 & 31) << (num4 & 31);
				num4 += num5;
				num3 |= (ulong)((ulong)num << num4);
				num4 += num5 * 6;
				bool flag = num4 > 64;
				if (flag)
				{
					buffer->WriteInt32VarLength(word - previous, 4);
					buffer->WriteInt64VarLength(Maths.ZigZagEncode((long)(*ptr[word])), 6);
				}
				else
				{
					buffer->WriteUInt64(num3, num4);
				}
				Assert.Check(!buffer->Overflow);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool CheckNothingToSendTicks(NetworkObjectMeta meta, NetworkObjectConnectionData data)
			{
				int num = data.UniqueDataChanges.MaxTick;
				bool flag = num < meta.ChangedTick;
				if (flag)
				{
					num = meta.ChangedTick;
				}
				return num > 0 && num <= data.TickSent;
			}

			private unsafe Simulation.StateReplicator.WriteResult WriteObject(NetworkObjectMeta meta, NetworkObjectConnectionData data)
			{
				Simulation.SendContext sendContext = this._simulation._sendContext;
				bool flag = sendContext.Header.ObjectUpdates == byte.MaxValue;
				Simulation.StateReplicator.WriteResult result;
				if (flag)
				{
					result = Simulation.StateReplicator.WriteResult.PacketFull;
				}
				else
				{
					bool flag2 = data == null;
					if (flag2)
					{
						data = sendContext.Connection.GetObjectData(meta.Id, true, false);
					}
					bool flag3 = Simulation.StateReplicator.CheckNothingToSendTicks(meta, data);
					if (flag3)
					{
						result = Simulation.StateReplicator.WriteResult.NothingToSend;
					}
					else
					{
						Span<int> raw = meta.Raw;
						int* changes = meta.Changes;
						int offsetBits = sendContext.Buffer->OffsetBits;
						Assert.Check(!sendContext.Buffer->Overflow);
						short wordCount = meta.WordCount;
						int i = 1;
						int num = 0;
						bool flag4 = this._simulation is Simulation.Server;
						if (flag4)
						{
							ValueTuple<NetworkObjectHeader.PlayerUniqueData, NetworkObjectHeader.PlayerUniqueDataChanges> playerData = data.GetPlayerData();
							NetworkObjectHeader.PlayerUniqueData item = playerData.Item1;
							NetworkObjectHeader.PlayerUniqueDataChanges item2 = playerData.Item2;
							meta.Header.PlayerData = item;
							for (int j = 0; j < 1; j++)
							{
								(changes + 9)[j] = *(ref item2.Changes.FixedElementField + (IntPtr)j * 4);
							}
						}
						Assert.Check<int, short>(num <= (int)wordCount, num, wordCount);
						int value = Maths.ZigZagEncode((int)(meta.Id.Raw - (uint)sendContext.ObjPrev));
						sendContext.Buffer->WriteInt32VarLength(value, 6);
						bool value2 = data.Status == NetworkObjectConnectionDataStatus.CreatedUnconfirmed;
						NetworkBufferSerializerInfo[] serializers = meta.Serializers;
						int num2 = serializers.Length;
						Tick b = (this._dataConsistency == SimulationConfig.DataConsistency.Eventual) ? data.TickSent : data.TickAcknowledged;
						bool flag5 = sendContext.Buffer->WriteBoolean(value2);
						if (flag5)
						{
							while (i < 20)
							{
								bool flag6 = sendContext.Buffer->WriteBoolean(*raw[i] != 0);
								if (flag6)
								{
									sendContext.Buffer->WriteInt64VarLength(Maths.ZigZagEncode((long)(*raw[i])), 6);
								}
								i++;
							}
							i = 20;
							num = i - 1;
						}
						else
						{
							while (i < 20)
							{
								bool flag7 = changes[i] > b;
								if (flag7)
								{
									Simulation.StateReplicator.WriteWord(sendContext.Buffer, raw, i, num);
									num = i;
								}
								i++;
							}
						}
						Assert.Check<int, short>(num <= (int)wordCount, num, wordCount);
						ulong filter = data.Filter;
						bool flag8 = filter != ulong.MaxValue && meta.Instance && meta.Instance.NetworkedBehaviours != null;
						if (flag8)
						{
							NetworkBehaviour[] networkedBehaviours = meta.Instance.NetworkedBehaviours;
							for (int k = 0; k < networkedBehaviours.Length; k++)
							{
								int num3 = networkedBehaviours[k].WordOffset + networkedBehaviours[k].WordCount;
								bool flag9 = (filter & 1UL << networkedBehaviours[k].ObjectIndex) == 0UL;
								if (flag9)
								{
									i = num3;
								}
								else
								{
									for (i = networkedBehaviours[k].WordOffset; i < num3; i++)
									{
										bool flag10 = changes[i] > b;
										if (flag10)
										{
											bool flag11 = i < num2 && serializers[i].Serializer != null;
											if (flag11)
											{
												i = serializers[i].Serializer.Write(sendContext, meta, serializers[i], raw, i, num);
											}
											else
											{
												Simulation.StateReplicator.WriteWord(sendContext.Buffer, raw, i, num);
											}
											num = i;
										}
									}
								}
							}
						}
						Assert.Check<int, short>(num <= (int)wordCount, num, wordCount);
						while (i < (int)wordCount)
						{
							bool flag12 = changes[i] > b;
							if (flag12)
							{
								bool flag13 = i < num2 && serializers[i].Serializer != null;
								if (flag13)
								{
									i = serializers[i].Serializer.Write(sendContext, meta, serializers[i], raw, i, num);
								}
								else
								{
									Simulation.StateReplicator.WriteWord(sendContext.Buffer, raw, i, num);
								}
								num = i;
							}
							i++;
						}
						Assert.Check<int, short>(num <= (int)wordCount, num, wordCount);
						bool isServer = this._simulation.IsServer;
						if (isServer)
						{
							meta.Header.PlayerData = default(NetworkObjectHeader.PlayerUniqueData);
							for (int l = 0; l < 1; l++)
							{
								(changes + 9)[l] = 0;
							}
						}
						bool flag14 = num == 0;
						if (flag14)
						{
							sendContext.Buffer->OffsetBits = offsetBits;
							data.TickSent = this._simulation.Tick;
							result = Simulation.StateReplicator.WriteResult.NothingToSend;
						}
						else
						{
							sendContext.Buffer->WriteInt32VarLength(0, 4);
							bool flag15 = Maths.BytesRequiredForBits(sendContext.Buffer->OffsetBits) > 44880;
							if (flag15)
							{
								sendContext.Buffer->OffsetBits = offsetBits;
								result = Simulation.StateReplicator.WriteResult.PacketFull;
							}
							else
							{
								Simulation.SendContext sendContext2 = sendContext;
								sendContext2.Header.ObjectUpdates = sendContext2.Header.ObjectUpdates + 1;
								sendContext.Envelope->AddObjectPacketData(this._simulation, meta.Id, data.TickSent, (NetworkObjectPacketFlags)0);
								sendContext.ObjPrev = (int)meta.Id.Raw;
								data.TickSent = this._simulation.Tick;
								this._simulation._fusionStatsManager.ObjectStatisticsManager.AddToNetworkObjectOutBandwidth(meta.Id, (float)Maths.BytesRequiredForBits(sendContext.Buffer->OffsetBits - offsetBits), false);
								this._simulation._fusionStatsManager.ObjectStatisticsManager.AddToNetworkObjectOutPackets(meta.Id, 1, false);
								bool flag16 = this._simulation._config.Topology == Topologies.Shared && meta.Instance && meta.Instance.HasStateAuthority;
								if (flag16)
								{
									meta.NextSnapshot(this._simulation.Tick).CopyFrom(meta);
									meta.AddLatestSnapshotToTimeline();
								}
								result = Simulation.StateReplicator.WriteResult.Written;
							}
						}
					}
				}
				return result;
			}

			public unsafe void OnPacketLost(NetConnection* c, SimulationPacketEnvelope* envelope)
			{
				bool flag = envelope->ObjectDataCount > 0;
				if (flag)
				{
					SimulationConnection simulationConnection = this._simulation.GetSimulationConnection(c);
					for (int i = 0; i < envelope->ObjectDataCount; i++)
					{
						NetworkObjectPacketData networkObjectPacketData = envelope->ObjectData[i];
						bool flag2 = (networkObjectPacketData.Flags & NetworkObjectPacketFlags.Destroy) == NetworkObjectPacketFlags.Destroy;
						if (flag2)
						{
							simulationConnection.ObjectData_Destroyed(networkObjectPacketData.Id, false);
						}
						else
						{
							NetworkObjectConnectionData objectData = simulationConnection.GetObjectData(networkObjectPacketData.Id, false, false);
							bool flag3 = objectData != null;
							if (flag3)
							{
								bool flag4 = objectData.TickSent > networkObjectPacketData.ResetTick;
								if (flag4)
								{
									objectData.TickSent = networkObjectPacketData.ResetTick;
								}
								bool flag5 = objectData.TickAcknowledged > networkObjectPacketData.ResetTick;
								if (flag5)
								{
									objectData.TickAcknowledged = networkObjectPacketData.ResetTick;
								}
							}
						}
					}
				}
			}

			public unsafe void OnPacketDelivered(NetConnection* c, SimulationPacketEnvelope* envelope)
			{
				SimulationConnection simulationConnection = this._simulation.GetSimulationConnection(c);
				bool flag = simulationConnection != null && envelope->Tick > 0;
				if (flag)
				{
					simulationConnection._latestTickAcknowledged = envelope->Tick;
				}
				bool flag2 = envelope->ObjectDataCount > 0;
				if (flag2)
				{
					bool flag3 = (this._simulation.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.Scheduling) == NetworkProjectConfig.ReplicationFeatures.Scheduling;
					for (int i = 0; i < envelope->ObjectDataCount; i++)
					{
						NetworkObjectPacketData networkObjectPacketData = envelope->ObjectData[i];
						bool flag4 = (networkObjectPacketData.Flags & NetworkObjectPacketFlags.Destroy) == NetworkObjectPacketFlags.Destroy;
						if (flag4)
						{
							simulationConnection.ObjectData_Remove(networkObjectPacketData.Id);
						}
						else
						{
							NetworkObjectConnectionData objectData = simulationConnection.GetObjectData(networkObjectPacketData.Id, false, false);
							bool flag5 = objectData != null;
							if (flag5)
							{
								objectData.TickAcknowledged = Math.Max(objectData.TickAcknowledged, envelope->Tick);
							}
							bool flag6 = objectData != null && objectData.Status <= NetworkObjectConnectionDataStatus.CreatedConfirmed;
							if (flag6)
							{
								objectData.Status = NetworkObjectConnectionDataStatus.CreatedConfirmed;
								bool flag7 = flag3 && !objectData.HasAnyPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) && networkObjectPacketData.ResetTick >= objectData.TickMin;
								if (flag7)
								{
									NetworkObjectMeta networkObjectMeta;
									bool flag8 = this._simulation.TryGetMeta(networkObjectPacketData.Id, out networkObjectMeta);
									if (flag8)
									{
										simulationConnection.SetIdle(objectData);
									}
								}
							}
						}
					}
				}
			}

			[Conditional("STATE_REPLICATOR_DEBUG")]
			private void AddDebugWord(int word, int value = 0)
			{
			}

			[Conditional("STATE_REPLICATOR_DEBUG")]
			private void ClearDebugWords()
			{
			}

			[Conditional("STATE_REPLICATOR_DEBUG")]
			private void DumpDebugWordsReceive(NetworkObjectMeta meta, bool unconfirmed, bool created)
			{
			}

			[Conditional("STATE_REPLICATOR_DEBUG")]
			private void DumpDebugWordsSend(NetworkObjectMeta meta)
			{
			}

			internal const int DATA_BLOCK_SIZE = 6;

			internal const int OFFSET_BLOCK_SIZE = 4;

			protected const int HEADER_BLOCK_SIZE = 8;

			protected const int GLOBAL_BLOCK_SIZE = 8;

			private readonly Simulation _simulation;

			private readonly List<NetworkObjectMeta.List> _aoiQuery;

			private readonly bool _notUsingAreaOfInterest;

			private readonly SimulationConfig.DataConsistency _dataConsistency;

			private HashSet<int> _changedWords = new HashSet<int>();

			private bool _loggedWordCheck;

			private bool _logged0 = false;

			private NetworkObjectMeta _runtimeConfig;

			private NetworkObjectMeta _sceneInfo;

			private NetworkObjectMeta _physicsInfo;

			private enum WriteResult
			{
				Written,
				NothingToSend,
				PacketFull
			}
		}
	}
}
