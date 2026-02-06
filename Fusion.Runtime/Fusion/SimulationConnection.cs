using System;
using System.Collections.Generic;
using Fusion.Sockets;

namespace Fusion
{
	internal class SimulationConnection
	{
		public unsafe int ConnectionIndex
		{
			get
			{
				return (int)this.Connection->LocalId.GroupIndex;
			}
		}

		public int DestroysPending
		{
			get
			{
				return this._objectsDestroyed.Count;
			}
		}

		public static implicit operator PlayerRef(SimulationConnection c)
		{
			Assert.Check(c.Player.IsRealPlayer);
			return c.Player;
		}

		internal SimulationConnection(Simulation simulation)
		{
			this._simulation = simulation;
			int num = TickRate.Resolve(simulation.Config.TickRateSelection).ClientSend;
			num = Math.Max(num, 5);
			this._inputs = new SimulationInput.Buffer(simulation.ProjectConfig);
			this._clientOffset = new TimeSeries(num);
			this._latestTickReceived = default(Tick);
			this._packetRecvDelta = new TimeSeries(num);
			this._packetRecvDeltaTimer = default(Timer);
			this._objects = new Dictionary<NetworkId, NetworkObjectConnectionData>(NetworkId.Comparer);
			this._objectsDestroyed = new Queue<NetworkId>();
			this.ObjectPriorityList = new NetworkObjectPriorityList();
			this.ObjectPriorityList.Player = this.Player;
		}

		public int GetPriority(NetworkObjectMeta meta)
		{
			return meta.GetPriority(this.Player);
		}

		public bool TryGetObjectData(NetworkId id, out NetworkObjectConnectionData data)
		{
			NetworkObjectConnectionData objectData;
			data = (objectData = this.GetObjectData(id, false, false));
			return objectData != null;
		}

		public NetworkObjectConnectionData GetObjectData(NetworkId id, bool create, bool allowFail = false)
		{
			NetworkObjectConnectionData networkObjectConnectionData;
			bool flag = !this._objects.TryGetValue(id, out networkObjectConnectionData) && create;
			if (flag)
			{
				NetworkObjectMeta networkObjectMeta;
				bool flag2 = this._simulation.TryGetMeta(id, out networkObjectMeta);
				if (flag2)
				{
					networkObjectConnectionData = new NetworkObjectConnectionData();
					networkObjectConnectionData.Id = id;
					networkObjectConnectionData.Filter = ulong.MaxValue;
					networkObjectConnectionData.MainTRSP = networkObjectMeta.HasMainTRSP;
					bool flag3 = networkObjectMeta.Instance && networkObjectMeta.Instance.NetworkedBehaviours != null;
					if (flag3)
					{
						NetworkBehaviour[] networkedBehaviours = networkObjectMeta.Instance.NetworkedBehaviours;
						for (int i = 0; i < networkedBehaviours.Length; i++)
						{
							bool flag4 = !networkedBehaviours[i].DefaultReplicated;
							if (flag4)
							{
								networkObjectConnectionData.Filter &= ~(1UL << networkedBehaviours[i].ObjectIndex);
							}
						}
					}
					this._objects.Add(id, networkObjectConnectionData);
					bool flag5 = !networkObjectMeta.IsStruct && this._simulation.Config.SchedulingEnabled;
					if (flag5)
					{
						this.ObjectPriorityList.SetActive(networkObjectConnectionData, networkObjectMeta);
						bool flag6 = this._simulation.IsClient || !this._simulation.Config.AreaOfInterestEnabled;
						if (flag6)
						{
							networkObjectConnectionData.SetPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.InAreaOfInterest, this._simulation);
						}
						bool flag7 = (networkObjectMeta.Flags & NetworkObjectHeaderFlags.GlobalObjectInterest) == NetworkObjectHeaderFlags.GlobalObjectInterest;
						if (flag7)
						{
							networkObjectConnectionData.SetPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.ForceInterest, this._simulation);
							try
							{
								this._simulation.Callbacks.ObjectEnterAOI(this.Player, id);
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
					if (allowFail)
					{
						return null;
					}
					throw new InvalidOperationException(string.Format("tried to get connection object data for {0} but it does not exist", id));
				}
			}
			return networkObjectConnectionData;
		}

		public bool DestroyedNextId(out NetworkId id)
		{
			bool flag = this._objectsDestroyed.Count > 0;
			bool result;
			if (flag)
			{
				id = this._objectsDestroyed.Dequeue();
				NetworkObjectConnectionData networkObjectConnectionData;
				bool flag2 = this._objects.TryGetValue(id, out networkObjectConnectionData);
				if (flag2)
				{
					Assert.Check(networkObjectConnectionData.Status == NetworkObjectConnectionDataStatus.DestroyUnconfirmed);
					networkObjectConnectionData.Status = NetworkObjectConnectionDataStatus.DestroyPending;
				}
				result = true;
			}
			else
			{
				id = default(NetworkId);
				result = false;
			}
			return result;
		}

		public void ObjectData_Remove(NetworkId id)
		{
			NetworkObjectConnectionData networkObjectConnectionData;
			bool flag = this._objects.TryGetValue(id, out networkObjectConnectionData);
			if (flag)
			{
				this._objects.Remove(id);
				this.PendingDeleteMainTRSP.Remove(id);
			}
		}

		public void ObjectData_Destroyed(NetworkId id, bool force = false)
		{
			NetworkObjectConnectionData networkObjectConnectionData;
			bool flag = this._objects.TryGetValue(id, out networkObjectConnectionData);
			if (flag)
			{
				bool flag2 = networkObjectConnectionData.Status != NetworkObjectConnectionDataStatus.DestroyUnconfirmed;
				if (flag2)
				{
					networkObjectConnectionData.Status = NetworkObjectConnectionDataStatus.DestroyUnconfirmed;
					this._objectsDestroyed.Enqueue(id);
					bool mainTRSP = networkObjectConnectionData.MainTRSP;
					if (mainTRSP)
					{
						this.PendingDeleteMainTRSP.Add(id);
					}
				}
			}
			else if (force)
			{
				this._objectsDestroyed.Enqueue(id);
			}
		}

		public bool? ObjectData_IsCreateUnconfirmed(NetworkId id)
		{
			NetworkObjectConnectionData objectData = this.GetObjectData(id, false, false);
			bool flag = objectData == null;
			bool? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new bool?(objectData.Status == NetworkObjectConnectionDataStatus.CreatedUnconfirmed);
			}
			return result;
		}

		public bool? ObjectData_IsDestroyUnconfirmed(NetworkId id)
		{
			NetworkObjectConnectionData objectData = this.GetObjectData(id, false, false);
			bool flag = objectData == null;
			bool? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new bool?(objectData.Status == NetworkObjectConnectionDataStatus.DestroyUnconfirmed);
			}
			return result;
		}

		public void Free(Simulation simulation)
		{
			simulation.FreeMessages(ref this.MessagesIn);
			simulation.FreeMessages(ref this.MessagesOut);
			this.LastSend = 0.0;
		}

		public void PacketReceiveDelta()
		{
			bool flag = !this._packetRecvDeltaTimer.IsRunning;
			if (flag)
			{
				this._packetRecvDeltaTimer = Timer.StartNew();
				Assert.Check(this._packetRecvDelta.IsEmpty);
				bool hasRuntimeConfig = this._simulation.HasRuntimeConfig;
				if (hasRuntimeConfig)
				{
					bool isClient = this._simulation.IsClient;
					double num;
					if (isClient)
					{
						SimulationRuntimeConfig runtimeConfig = this._simulation.RuntimeConfig;
						num = runtimeConfig.TickRate.ServerSendDelta;
					}
					else
					{
						SimulationRuntimeConfig runtimeConfig = this._simulation.RuntimeConfig;
						num = runtimeConfig.TickRate.ClientSendDelta;
					}
					this._packetRecvDelta.Add(num);
				}
			}
			else
			{
				double num = this._packetRecvDeltaTimer.ElapsedInSeconds;
				bool flag2 = num < 0.001;
				if (!flag2)
				{
					this._packetRecvDelta.Add(num);
					this._packetRecvDeltaTimer.Restart();
				}
			}
		}

		public void ResetTimeFeedback()
		{
			this._clientOffset.Clear();
			this._packetRecvDelta.Clear();
			this._packetRecvDeltaTimer.Reset();
		}

		public void InputReceiveDelta(Tick tick, double receive, double expected)
		{
			bool flag = tick <= this._latestTickReceived;
			if (!flag)
			{
				this._latestTickReceived = tick;
				this._clientOffset.Add(expected - receive);
			}
		}

		public void SetActive(NetworkObjectConnectionData data, NetworkObjectMeta meta)
		{
			this.ObjectPriorityList.SetActive(data, meta);
		}

		public void SetIdle(NetworkObjectConnectionData data)
		{
			this.ObjectPriorityList.SetIdle(data);
		}

		public void AddAlwaysInterested(NetworkObjectMeta meta)
		{
			bool flag = meta == null || (meta.Instance && meta.Instance.ObjectInterest == NetworkObject.ObjectInterestModes.Global);
			if (!flag)
			{
				Assert.Check((this._simulation.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement);
				NetworkObjectConnectionData objectData = this.GetObjectData(meta.Id, true, false);
				bool flag2 = (objectData.UniqueData.Flags & NetworkObjectHeaderPlayerDataFlags.ForceInterest) == NetworkObjectHeaderPlayerDataFlags.ForceInterest;
				if (!flag2)
				{
					bool flag3 = (objectData.UniqueData.Flags & NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) == (NetworkObjectHeaderPlayerDataFlags)0;
					if (flag3)
					{
						this._simulation.Callbacks.ObjectEnterAOI(this.Player, meta.Id);
					}
					this.SetActive(objectData, meta);
					objectData.SetPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.ForceInterest, this._simulation);
				}
			}
		}

		public void RemoveAlwaysInterested(NetworkObjectMeta meta)
		{
			bool flag;
			if (meta != null)
			{
				NetworkObject instance = meta.Instance;
				flag = (instance != null && instance.ObjectInterest == NetworkObject.ObjectInterestModes.Global);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				Assert.Check((this._simulation.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement);
				NetworkObjectConnectionData objectData = this.GetObjectData(meta.Id, false, false);
				bool flag3 = objectData != null;
				if (flag3)
				{
					bool flag4 = !objectData.UniqueData.Flags.HasFlag(NetworkObjectHeaderPlayerDataFlags.ForceInterest);
					if (!flag4)
					{
						bool flag5 = (objectData.UniqueData.Flags & NetworkObjectHeaderPlayerDataFlags.AllInterestFlags) == NetworkObjectHeaderPlayerDataFlags.ForceInterest;
						if (flag5)
						{
							this._simulation.Callbacks.ObjectExitAOI(this.Player, meta.Id);
						}
						objectData.ClearPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags.ForceInterest, this._simulation);
					}
				}
			}
		}

		public const int INTEGRATOR_HISTORY_MULT = 10;

		private Simulation _simulation;

		private Dictionary<NetworkId, NetworkObjectConnectionData> _objects;

		private Queue<NetworkId> _objectsDestroyed;

		public PlayerRef Player;

		public bool AreaOfInterestHasBeenUpdated = false;

		public HashSet<int> AreaOfInterestCells = new HashSet<int>();

		public ulong MessagesInSequence;

		public ulong MessagesOutSequence;

		public SimulationMessageList MessagesIn;

		public SimulationMessageList MessagesOut;

		internal double LastSend;

		internal int ActiveStructsVersion = 0;

		internal int ActiveStructsIndex = 0;

		internal List<NetworkObjectConnectionData> ActiveStructs = new List<NetworkObjectConnectionData>();

		internal TimeSeries _packetRecvDelta;

		internal Timer _packetRecvDeltaTimer;

		internal SimulationInput.Buffer _inputs;

		internal TimeSeries _clientOffset;

		internal Tick _latestTickReceived;

		internal Tick _latestTickAcknowledged;

		internal NetworkObjectPriorityList ObjectPriorityList;

		internal unsafe NetConnection* Connection;

		internal NetConnectionId ConnectionId;

		internal HashSet<NetworkId> PendingDeleteMainTRSP = new HashSet<NetworkId>(NetworkId.Comparer);
	}
}
