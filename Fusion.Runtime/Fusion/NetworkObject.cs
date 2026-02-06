using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fusion
{
	[AddComponentMenu("Fusion/Network Object")]
	[DisallowMultipleComponent]
	[HelpURL("https://doc.photonengine.com/fusion/current/manual/network-object")]
	[ScriptHelp(Url = "https://doc.photonengine.com/fusion/current/manual/network-object", BackColor = ScriptHeaderBackColor.Orange)]
	public class NetworkObject : Behaviour
	{
		public unsafe NetworkId Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				NetworkObjectHeader* ptr = (NetworkObjectHeader*)this.Ptr;
				return (ptr != null) ? ptr->Id : default(NetworkId);
			}
		}

		public NetworkRunner Runner
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._runner;
			}
		}

		public Tick LastReceiveTick
		{
			get
			{
				NetworkObjectMeta meta = this.Meta;
				return (meta != null && meta.HasSnapshots) ? this.Meta.SnapshotLatest.Tick : default(Tick);
			}
		}

		public string Name
		{
			get
			{
				return this.Id.ToString() + (BehaviourUtils.IsAlive(this) ? ("(" + base.name + ")") : "");
			}
		}

		internal Simulation Simulation
		{
			get
			{
				return BehaviourUtils.IsAlive(this.Runner) ? this.Runner.Simulation : null;
			}
		}

		public bool IsValid
		{
			get
			{
				return BehaviourUtils.IsAlive(this.Runner) && this.Runner.Exists(this);
			}
		}

		public bool IsInSimulation
		{
			get
			{
				return (this.RuntimeFlags & NetworkObjectRuntimeFlags.InSimulation) == NetworkObjectRuntimeFlags.InSimulation;
			}
		}

		internal unsafe ref NetworkObjectHeader Header
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref *(NetworkObjectHeader*)this.Ptr;
			}
		}

		internal unsafe Span<int> Data
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.Ptr != null) ? new Span<int>((void*)(this.Ptr + 20), (int)(this.Header.WordCount - 20)) : default(Span<int>);
			}
		}

		internal ReadOnlySpan<int> BehaviourChangedTickArray
		{
			get
			{
				return (this.Meta != null) ? this.Meta.BehaviourChangedTickArray : default(Span<int>);
			}
		}

		public bool HasInputAuthority
		{
			get
			{
				Simulation simulation = this.Simulation;
				return simulation != null && simulation.IsLocalSimulationInputAuthority(this.Header);
			}
		}

		public bool HasStateAuthority
		{
			get
			{
				Simulation simulation = this.Simulation;
				return simulation != null && simulation.IsLocalSimulationStateAuthority(this.Header);
			}
		}

		public bool IsProxy
		{
			get
			{
				return this.Simulation != null && !this.Simulation.IsLocalSimulationInputAuthority(this.Header) && !this.Simulation.IsLocalSimulationStateAuthority(this.Header);
			}
		}

		public bool IsNested
		{
			get
			{
				return (this.RuntimeFlags & NetworkObjectRuntimeFlags.IsNested) == NetworkObjectRuntimeFlags.IsNested;
			}
		}

		public NetworkObject NestingRoot
		{
			get
			{
				bool flag = !this.IsNested || this.Runner == null || this.Ptr == null;
				NetworkObject result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this.Runner.FindObject(this.Header.NestingRoot);
				}
				return result;
			}
		}

		public RenderTimeframe RenderTimeframe
		{
			get
			{
				bool forceRemoteRenderTimeframe = this.ForceRemoteRenderTimeframe;
				RenderTimeframe result;
				if (forceRemoteRenderTimeframe)
				{
					result = RenderTimeframe.Remote;
				}
				else
				{
					RenderTimeframe renderTimeframe;
					if (!this.IsInSimulation)
					{
						Simulation simulation = this.Simulation;
						if (simulation == null || !simulation.IsLocalSimulationStateAuthority(this.Header))
						{
							renderTimeframe = RenderTimeframe.Remote;
							goto IL_36;
						}
					}
					renderTimeframe = RenderTimeframe.Local;
					IL_36:
					result = renderTimeframe;
				}
				return result;
			}
		}

		public RenderSource RenderSource
		{
			get
			{
				return this._renderSource;
			}
			set
			{
				this._renderSource = value;
			}
		}

		public float RenderTime
		{
			get
			{
				bool flag = !BehaviourUtils.IsAlive(this.Runner);
				float result;
				if (flag)
				{
					result = 0f;
				}
				else
				{
					bool flag2 = this.RenderTimeframe == RenderTimeframe.Local;
					if (flag2)
					{
						result = this.Runner.LocalRenderTime;
					}
					else
					{
						result = this.Runner.RemoteRenderTime;
					}
				}
				return result;
			}
		}

		public PlayerRef InputAuthority
		{
			get
			{
				return (this.Ptr == null) ? PlayerRef.None : this.Header.InputAuthority;
			}
		}

		public PlayerRef StateAuthority
		{
			get
			{
				return (this.Ptr == null) ? PlayerRef.None : this.Runner.Simulation.GetStateAuthority(this.Header.StateAuthority);
			}
		}

		public bool IsSpawnable
		{
			get
			{
				return !this.Flags.IsIgnored();
			}
			set
			{
				this.Flags.SetIgnored(!value);
			}
		}

		internal void PrepareBehaviourOrder()
		{
			bool flag = this.NetworkedBehaviours == null || this.NetworkedBehaviours.Length == 0;
			if (flag)
			{
				this.RuntimeFlags &= ~NetworkObjectRuntimeFlags.HasMainNetworkTRSP;
			}
			else
			{
				bool flag2 = false;
				for (int i = 0; i < this.NetworkedBehaviours.Length; i++)
				{
					NetworkTRSP networkTRSP = this.NetworkedBehaviours[i] as NetworkTRSP;
					bool flag3 = networkTRSP != null;
					if (flag3)
					{
						bool flag4 = this.NetworkedBehaviours[i].gameObject == base.gameObject && !flag2;
						if (flag4)
						{
							networkTRSP.IsMainTRSP = true;
							flag2 = true;
							Assert.Check(this.NetworkedBehaviours[i].ObjectIndex == i);
							this.RuntimeFlags |= NetworkObjectRuntimeFlags.HasMainNetworkTRSP;
							NetworkBehaviour networkBehaviour = this.NetworkedBehaviours[0];
							this.NetworkedBehaviours[0] = this.NetworkedBehaviours[i];
							this.NetworkedBehaviours[i] = networkBehaviour;
							this.NetworkedBehaviours[0].ObjectIndex = 0;
							this.NetworkedBehaviours[i].ObjectIndex = i;
						}
						else
						{
							networkTRSP.IsMainTRSP = false;
						}
					}
				}
				bool flag5 = !flag2;
				if (flag5)
				{
					this.RuntimeFlags &= ~NetworkObjectRuntimeFlags.HasMainNetworkTRSP;
				}
			}
		}

		protected virtual void Awake()
		{
			Assert.Check(!this.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.HadAwake));
			this.RuntimeFlags |= NetworkObjectRuntimeFlags.HadAwake;
			this.DebugAwake();
			bool flag = this.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.NotAwakeWhenAttaching);
			if (flag)
			{
				bool isValid = this.Id.IsValid;
				if (isValid)
				{
					bool flag2 = BehaviourUtils.IsAlive(this.Runner);
					if (flag2)
					{
						this.Runner.AttachActivatedByUser(this);
					}
					else
					{
						DebugLogStream logDebug = InternalLogStreams.LogDebug;
						if (logDebug != null)
						{
							logDebug.Warn(this, "Expected to be activated while the runner is active");
						}
					}
				}
			}
		}

		protected virtual void OnDestroy()
		{
			this.OnDestroyInternal();
			this.DebugOnDestroy(true);
		}

		internal void OnDestroyNeverActive()
		{
			Assert.Check<LogUtils.DumpDeferredClass>(!this.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.HadAwake), "Object was not supposed to be activated {0}", LogUtils.GetDump<NetworkObject>(this));
			Assert.Check<LogUtils.DumpDeferredClass>(this.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.NotAwakeWhenAttaching), "Expected to have the flag {0}", LogUtils.GetDump<NetworkObject>(this));
			this.OnDestroyInternal();
			Assert.Check((this.RuntimeFlags & NetworkObjectRuntimeFlags.Spawned) == NetworkObjectRuntimeFlags.None, "Never should have become active");
			this.DebugOnDestroy(false);
		}

		private void OnDestroyInternal()
		{
			bool flag = !BehaviourUtils.IsAlive(this);
			if (!flag)
			{
				this.RuntimeFlags |= NetworkObjectRuntimeFlags.IsDestroyed;
				bool flag2 = BehaviourUtils.IsAlive(this.Runner);
				if (flag2)
				{
					bool flag3 = this.Ptr != null && this.Runner.Simulation != null && (this.HasStateAuthority || (this.StateAuthority == PlayerRef.None && this.Runner.IsSharedModeMasterClient));
					this.Runner.Destroy(this, NetworkObjectDestroyFlags.DestroyedByEngine | (flag3 ? NetworkObjectDestroyFlags.DestroyState : NetworkObjectDestroyFlags.None));
				}
				else
				{
					bool flag4 = BehaviourUtils.IsNotNull(this.Runner);
					if (flag4)
					{
						bool isValid = this.Id.IsValid;
						if (isValid)
						{
							DebugLogStream logDebug = InternalLogStreams.LogDebug;
							if (logDebug != null)
							{
								logDebug.Warn(this, "Runner has been destroyed, but the object has not been despawned.");
							}
						}
					}
				}
				this.Ptr = null;
			}
		}

		internal unsafe void ResetNetworkState()
		{
			this.MakeUnowned();
			this.Ptr = default(int*);
			this.NetworkTypeId = default(NetworkObjectTypeId);
			this.RuntimeFlags &= ~NetworkObjectRuntimeFlags.ClearMask;
		}

		internal void Defaults()
		{
			Assert.Check(this.Ptr != null);
			this.Header.InputAuthority = default(PlayerRef);
			this.Header.StateAuthority = default(PlayerRef);
		}

		public static int GetWordCount(NetworkObject obj)
		{
			bool flag = BehaviourUtils.IsAlive(obj);
			int result;
			if (flag)
			{
				int num = NetworkStructUtils.GetWordCount<NetworkObjectHeader>();
				for (int i = 0; i < obj.NetworkedBehaviours.Length; i++)
				{
					bool flag2 = BehaviourUtils.IsAlive(obj.NetworkedBehaviours[i]);
					if (!flag2)
					{
						throw new Exception("Found missing NetworkBehaviour reference in NetworkBehaviours[] list on " + obj.Name + ". Re-baking of object required. Please check prefab or scene object and make sure NetworkBehaviour list is up to date.");
					}
					num += NetworkBehaviourUtils.GetWordCount(obj.NetworkedBehaviours[i]);
				}
				result = num + obj.NetworkedBehaviours.Length;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public int GetLocalAuthorityMask()
		{
			Simulation simulation = this.Simulation;
			return (simulation != null) ? simulation.GetLocalAuthorityMask(this.Header) : 0;
		}

		public void AssignInputAuthority(PlayerRef player)
		{
			Assert.Check(BehaviourUtils.IsAlive(this.Runner));
			Assert.Check(this.Runner.Exists(this));
			bool flag = this.Runner.Topology == Topologies.ClientServer || (this.HasStateAuthority && this.Runner.LocalPlayer == player);
			if (flag)
			{
				PlayerRef inputAuthority = this.Header.InputAuthority;
				this.Header.InputAuthority = player;
				bool flag2 = (this.Simulation.Config.ReplicationFeatures & NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement) == NetworkProjectConfig.ReplicationFeatures.SchedulingAndInterestManagement;
				bool flag3 = flag2;
				if (flag3)
				{
					SimulationConnection simulationConnection;
					bool flag4 = inputAuthority.IsRealPlayer && this.Simulation.TryGetSimulationConnectionForPlayer(inputAuthority, out simulationConnection);
					if (flag4)
					{
						simulationConnection.RemoveAlwaysInterested(this.Meta);
					}
					SimulationConnection simulationConnection2;
					bool flag5 = player.IsRealPlayer && this.Simulation.TryGetSimulationConnectionForPlayer(player, out simulationConnection2);
					if (flag5)
					{
						simulationConnection2.AddAlwaysInterested(this.Meta);
					}
				}
			}
		}

		public void RequestStateAuthority()
		{
			Assert.Check(BehaviourUtils.IsAlive(this.Runner));
			Assert.Check(this.Runner.Exists(this));
			bool flag = this.Runner.IsClient && !this.HasStateAuthority;
			if (flag)
			{
				this.Runner.Simulation.RequestStateAuthority(this.Id, true);
			}
		}

		public void ReleaseStateAuthority()
		{
			Assert.Check(BehaviourUtils.IsAlive(this.Runner));
			Assert.Check(this.Runner.Exists(this));
			bool flag = this.Runner.IsClient && this.HasStateAuthority;
			if (flag)
			{
				this.Runner.Simulation.RequestStateAuthority(this.Id, false);
			}
		}

		public void RemoveInputAuthority()
		{
			this.AssignInputAuthority(default(PlayerRef));
		}

		public static implicit operator NetworkId(NetworkObject obj)
		{
			return BehaviourUtils.IsNull(obj) ? default(NetworkId) : obj.Id;
		}

		public void SetPlayerAlwaysInterested(PlayerRef player, bool alwaysInterested)
		{
			bool flag = !this.HasStateAuthority;
			if (!flag)
			{
				this.Runner.Simulation.SetPlayerAlwaysInterested(player, this, alwaysInterested);
			}
		}

		public void CopyStateFrom(NetworkObject source)
		{
			Assert.Check(source.Id.IsValid, "Invalid NetworkId from source NetworkObject");
			Assert.Check(source.Id.Equals(this.Id), "NetworkObjects must have the same NetworkIds");
			Assert.Check(this.Ptr != null);
			Assert.Check(source.Ptr != null);
			Assert.Check(this.Header.Type.Equals(source.Header.Type), "NetworkObjects must be of the same type");
			Native.MemCpy(this.Data, source.Data);
			for (int i = 0; i < this.NestedObjects.Length; i++)
			{
				this.NestedObjects[i].CopyStateFrom(source.NestedObjects[i]);
			}
		}

		public unsafe void CopyStateFrom(NetworkObjectHeaderPtr source)
		{
			Assert.Check(this.Ptr != null);
			Assert.Check(this.Header.Type.Equals(source.Ptr->Type), "NetworkObjects must be of the same type");
			Native.MemCpy(this.Data, source.Data);
		}

		[Obsolete("Use NetworkWrap(NetworkObject) instead")]
		public static NetworkId NetworkWrap(NetworkRunner runner, NetworkObject obj)
		{
			return NetworkObject.NetworkWrap(obj);
		}

		[NetworkSerializeMethod]
		public static NetworkId NetworkWrap(NetworkObject obj)
		{
			bool flag = BehaviourUtils.IsNotAlive(obj);
			NetworkId result;
			if (flag)
			{
				result = default(NetworkId);
			}
			else
			{
				result = obj.Id;
			}
			return result;
		}

		[NetworkDeserializeMethod]
		public static void NetworkUnwrap(NetworkRunner runner, NetworkId wrapper, ref NetworkObject result)
		{
			bool flag = !wrapper.IsValid;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = !runner.TryFindObject(wrapper, out result);
				if (flag2)
				{
					Assert.Check(BehaviourUtils.IsNotAlive(result));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void MakeOwned(NetworkRunner runner)
		{
			Assert.Check<NetworkRunner>(this._runner == null, "Already owned {0}", this._runner);
			this._runner = runner;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void MakeUnowned()
		{
			this._runner = null;
		}

		private void DebugAwake()
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(this, string.Format("Awake ({0})", this.RuntimeFlags));
			}
			bool flag = (this.RuntimeFlags & NetworkObjectRuntimeFlags.Spawned) > NetworkObjectRuntimeFlags.None;
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(this, "Spawned before Awake");
				}
			}
		}

		private void DebugOnDestroy(bool wasActive)
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(this, string.Format("OnDestroy ({0})", this.RuntimeFlags));
			}
			if (wasActive)
			{
				bool flag = (this.RuntimeFlags & NetworkObjectRuntimeFlags.Spawned) > NetworkObjectRuntimeFlags.None;
				if (flag)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(this, "Not despawned before OnDestroy");
					}
				}
			}
			else
			{
				bool flag2 = (this.RuntimeFlags & NetworkObjectRuntimeFlags.HadAwake) > NetworkObjectRuntimeFlags.None;
				if (flag2)
				{
					LogStream logError2 = InternalLogStreams.LogError;
					if (logError2 != null)
					{
						logError2.Log(this, "Should not have been awoken");
					}
				}
			}
		}

		protected internal override void GetDumpString(StringBuilder builder)
		{
			builder.Append("[");
			builder.Append(base.DebugNameThreadSafe);
			bool isValid = this.Id.IsValid;
			if (isValid)
			{
				builder.Append(" ");
				builder.Append(this.Id.ToString());
			}
			int length = builder.Length;
			bool flag = NetworkRunner.TryGetPrettyRunnerName(builder, this.Runner);
			if (flag)
			{
				builder.Insert(length, "@");
			}
			builder.Append("]");
		}

		[NonSerialized]
		internal unsafe int* Ptr;

		[NonSerialized]
		public bool IsResume;

		private NetworkRunner _runner;

		internal NetworkObjectMeta Meta;

		[HideInInspector]
		[SerializeField]
		public uint SortKey;

		[Obsolete("not used anymore, use interest management instead")]
		[NonSerialized]
		public NetworkObject.ReplicateToDelegate ReplicateTo;

		[NonSerialized]
		public NetworkObject.PriorityLevelDelegate PriorityCallback;

		[InlineHelp]
		[SerializeField]
		[FormerlySerializedAs("AoiMode")]
		internal NetworkObject.ObjectInterestModes ObjectInterest = NetworkObject.ObjectInterestModes.Global;

		[InlineHelp]
		public NetworkObjectFlags Flags = NetworkObjectFlags.DestroyWhenStateAuthorityLeaves;

		[NonSerialized]
		internal NetworkObjectRuntimeFlags RuntimeFlags;

		[NonSerialized]
		public NetworkObjectTypeId NetworkTypeId;

		[InlineHelp]
		public NetworkObject[] NestedObjects;

		[InlineHelp]
		public NetworkBehaviour[] NetworkedBehaviours;

		private RenderSource _renderSource;

		public bool ForceRemoteRenderTimeframe = false;

		public delegate bool ReplicateToDelegate(NetworkObject networkObject, PlayerRef player);

		public delegate PriorityLevel PriorityLevelDelegate(NetworkObject networkObject, PlayerRef player);

		internal enum ObjectInterestModes
		{
			AreaOfInterest,
			Global,
			Explicit
		}
	}
}
