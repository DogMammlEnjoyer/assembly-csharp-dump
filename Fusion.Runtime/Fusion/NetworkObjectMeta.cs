using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public class NetworkObjectMeta
	{
		static NetworkObjectMeta()
		{
			NetworkObjectMeta._serializersStatic = new NetworkBufferSerializerInfo[29];
			NetworkObjectMeta._serializersStatic[22].Serializer = NetworkTransformSerializer.Instance;
			NetworkObjectMeta._serializersStatic[22].Offset = 0;
			NetworkObjectMeta._serializersStatic[23].Serializer = NetworkTransformSerializer.Instance;
			NetworkObjectMeta._serializersStatic[23].Offset = 1;
			NetworkObjectMeta._serializersStatic[24].Serializer = NetworkTransformSerializer.Instance;
			NetworkObjectMeta._serializersStatic[24].Offset = 2;
			NetworkObjectMeta._serializersStatic[25].Serializer = NetworkTransformSerializer.Instance;
			NetworkObjectMeta._serializersStatic[25].Offset = 3;
			NetworkObjectMeta._serializersStatic[26].Serializer = NetworkTransformSerializer.Instance;
			NetworkObjectMeta._serializersStatic[26].Offset = 4;
			NetworkObjectMeta._serializersStatic[27].Serializer = NetworkTransformSerializer.Instance;
			NetworkObjectMeta._serializersStatic[27].Offset = 5;
			NetworkObjectMeta._serializersStatic[28].Serializer = NetworkTransformSerializer.Instance;
			NetworkObjectMeta._serializersStatic[28].Offset = 6;
		}

		internal static NetworkBufferSerializerInfo[] GetSerializers(bool main)
		{
			return main ? NetworkObjectMeta._serializersStatic : NetworkObjectMeta._serializersNone;
		}

		internal NetworkBufferSerializerInfo[] Serializers
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return NetworkObjectMeta.GetSerializers(this.HasMainTRSP);
			}
		}

		internal Timeline Timeline
		{
			get
			{
				Assert.Check(this._simulation.HasRuntimeConfig);
				bool flag = this._timeline == null;
				if (flag)
				{
					this._timeline = new Timeline(this._simulation.TickRate);
				}
				return this._timeline;
			}
		}

		internal unsafe ref NetworkObjectHeader Header
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref *(NetworkObjectHeader*)this._ptr;
			}
		}

		public NetworkObjectHeaderFlags Flags
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._flags;
			}
		}

		internal bool HasMainTRSP
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Flags.CheckFlag(NetworkObjectHeaderFlags.HasMainNetworkTRSP);
			}
		}

		internal unsafe ref NetworkTRSPData MainTRSPData
		{
			get
			{
				Assert.Check(this.HasMainTRSP);
				return ref *(NetworkTRSPData*)(this._ptr + 20);
			}
		}

		internal unsafe Span<int> Raw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Span<int>((void*)this._ptr, (int)this.WordCount);
			}
		}

		internal Span<int> Data
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Span<int> raw = this.Raw;
				ref Span<int> ptr = ref raw;
				return ptr.Slice(20, ptr.Length - 20);
			}
		}

		internal Span<int> BehaviourChangedTickArray
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Span<int> raw = this.Raw;
				ref Span<int> ptr = ref raw;
				int num = (int)(this.WordCount - this.BehaviourCount);
				return ptr.Slice(num, ptr.Length - num);
			}
		}

		internal Span<int> GetBehaviourChangedTickArray(NetworkObjectHeaderSnapshotRef snapshot)
		{
			Span<int> raw = snapshot.Raw;
			ref Span<int> ptr = ref raw;
			int num = (int)(this.WordCount - this.BehaviourCount);
			return ptr.Slice(num, ptr.Length - num);
		}

		internal unsafe Tick GetMaxBehaviourChangedTick()
		{
			Tick tick = 0;
			Span<int> behaviourChangedTickArray = this.BehaviourChangedTickArray;
			for (int i = 0; i < behaviourChangedTickArray.Length; i++)
			{
				Tick value = *behaviourChangedTickArray[i];
				tick = Math.Max(tick, value);
			}
			return tick;
		}

		internal unsafe Tick GetMaxBehaviourChangedTick(NetworkObjectHeaderSnapshotRef snapshot)
		{
			Tick tick = 0;
			Span<int> behaviourChangedTickArray = this.GetBehaviourChangedTickArray(snapshot);
			for (int i = 0; i < behaviourChangedTickArray.Length; i++)
			{
				Tick value = *behaviourChangedTickArray[i];
				tick = Math.Max(tick, value);
			}
			return tick;
		}

		internal unsafe T* GetDataAs<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return (T*)(this._ptr + 20);
		}

		internal bool HasSnapshots
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._snapshots.Latest != null;
			}
		}

		internal NetworkObjectHeaderSnapshotRef SnapshotLatest
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new NetworkObjectHeaderSnapshotRef(this._snapshots.Latest);
			}
		}

		internal bool IsStruct
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.Flags & NetworkObjectHeaderFlags.Struct) == NetworkObjectHeaderFlags.Struct;
			}
		}

		internal bool IsObject
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.Flags & NetworkObjectHeaderFlags.Struct) == (NetworkObjectHeaderFlags)0;
			}
		}

		public NetworkObjectTypeId Type
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Header.Type;
			}
		}

		public NetworkId Id
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Header.Id;
			}
		}

		public NetworkId NestingRoot
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Header.NestingRoot;
			}
		}

		public NetworkObjectNestingKey NestingKey
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.Header.NestingKey;
			}
		}

		internal ref PlayerRef StateAuthority
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref this.Header.StateAuthority;
			}
		}

		public ref PlayerRef InputAuthority
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref this.Header.InputAuthority;
			}
		}

		internal NetworkObjectHeaderSnapshotRef Shadow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				NetworkObjectHeaderSnapshot snapshot;
				if ((snapshot = this._shadow) == null)
				{
					snapshot = (this._shadow = this.GetFirstShadowSnapshot());
				}
				return snapshot;
			}
		}

		internal NetworkObjectHeaderSnapshotRef Render
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				NetworkObjectHeaderSnapshot snapshot;
				if ((snapshot = this._render) == null)
				{
					snapshot = (this._render = this.GetSnapshot(false));
				}
				return snapshot;
			}
		}

		internal NetworkObjectHeaderSnapshotRef Previous
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				NetworkObjectHeaderSnapshot snapshot;
				if ((snapshot = this._previous) == null)
				{
					snapshot = (this._previous = this.GetSnapshot(true));
				}
				return snapshot;
			}
		}

		internal NetworkObjectHeaderSnapshotRef Migration
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				NetworkObjectHeaderSnapshot snapshot;
				if ((snapshot = this._migration) == null)
				{
					snapshot = (this._migration = this.GetSnapshot(false));
				}
				return snapshot;
			}
		}

		internal unsafe Span<int> ChangesSpan
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Span<int>((void*)this.Changes, (int)this.WordCount);
			}
		}

		internal unsafe int* Changes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = this._changes == null;
				if (flag)
				{
					this._changes = Allocator.AllocAndClearArray<int>(this._allocator, (int)this.WordCount);
				}
				return this._changes;
			}
		}

		private NetworkObjectHeaderSnapshot GetFirstShadowSnapshot()
		{
			NetworkObjectHeaderSnapshot snapshot = this.GetSnapshot(false);
			NetworkObjectHeaderSnapshotRef networkObjectHeaderSnapshotRef = new NetworkObjectHeaderSnapshotRef(snapshot);
			networkObjectHeaderSnapshotRef.Header.StateAuthority = PlayerRef.Invalid;
			return snapshot;
		}

		internal NetworkObjectMeta(Simulation simulation, Allocator allocator)
		{
			this._allocator = allocator;
			this._simulation = simulation;
		}

		private NetworkObjectHeaderSnapshot GetSnapshot(bool copyState)
		{
			NetworkObjectHeaderSnapshot snapshot = this._simulation.GetSnapshot();
			snapshot.Init(this, copyState);
			return snapshot;
		}

		internal unsafe void Release(Allocator objectAllocator)
		{
			*this.Header = default(NetworkObjectHeader);
			Allocator.Free<int>(objectAllocator, ref this._ptr);
			bool flag = this.Instance;
			if (flag)
			{
				this.Instance.Meta = null;
			}
			this._prev = null;
			this._next = null;
			this.Instance = null;
			this.PlayerData = default(NetworkObjectHeader.PlayerUniqueData);
			this.AreaOfInterestCell = 0;
			this.LocalFlags = NetworkObjectMetaFlags.None;
			this.ScannedTick = default(Tick);
			this.ChangedTick = default(Tick);
			this._simulation.SnapshotRelease(ref this._shadow);
			this._simulation.SnapshotRelease(ref this._render);
			this._simulation.SnapshotRelease(ref this._previous);
			this._simulation.SnapshotRelease(ref this._migration);
			Allocator.Free<int>(this._allocator, ref this._changes);
			bool flag2 = this._snapshotsByIndex != null;
			if (flag2)
			{
				Array.Clear(this._snapshotsByIndex, 0, this._snapshotsByIndex.Length);
			}
			while (this._snapshots.Count > 0)
			{
				this._simulation.SnapshotRelease(this._snapshots.RemoveLatest());
			}
			this._snapshots = default(NetworkObjectHeaderSnapshotList);
			Timeline timeline = this._timeline;
			if (timeline != null)
			{
				timeline.Clear();
			}
		}

		internal NetworkObjectHeaderSnapshotRef NextSnapshot(Tick tick)
		{
			bool flag = this._snapshots.Count == 0;
			if (flag)
			{
				this._snapshots.AddFirst(this.GetSnapshot(true));
			}
			else
			{
				bool flag2 = this._simulation.HasRuntimeConfig && this._snapshots.Count >= this._simulation.TickRate;
				if (flag2)
				{
					NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot = this._snapshots.RemoveOldest();
					networkObjectHeaderSnapshot.CopyFrom(this._snapshots.Latest);
					this._snapshots.AddFirst(networkObjectHeaderSnapshot);
				}
				else
				{
					this._snapshots.AddFirst(this._snapshots.Latest.Clone(this._simulation));
				}
			}
			this._snapshots.Latest.Tick = tick;
			bool flag3 = this._snapshotsByIndex == null;
			if (flag3)
			{
				this._snapshotsByIndex = new NetworkObjectHeaderSnapshot[64];
			}
			Array.Clear(this._snapshotsByIndex, 0, this._snapshotsByIndex.Length);
			this._snapshotsByIndexLatest = new Tick?(this._snapshots.Latest.Tick);
			NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot2 = this._snapshots.Latest;
			while (networkObjectHeaderSnapshot2 != null)
			{
				int num = this._snapshotsByIndexLatest.Value - networkObjectHeaderSnapshot2.Tick;
				bool flag4 = num >= this._snapshotsByIndex.Length;
				if (flag4)
				{
					break;
				}
				bool flag5 = num < 0;
				if (flag5)
				{
					NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot3 = networkObjectHeaderSnapshot2;
					networkObjectHeaderSnapshot2 = networkObjectHeaderSnapshot2.Next;
					this._snapshots.Remove(networkObjectHeaderSnapshot3);
					networkObjectHeaderSnapshot3.Release();
				}
				else
				{
					this._snapshotsByIndex[num] = networkObjectHeaderSnapshot2;
					networkObjectHeaderSnapshot2 = networkObjectHeaderSnapshot2.Next;
				}
			}
			return new NetworkObjectHeaderSnapshotRef(this._snapshots.Latest);
		}

		internal void AddLatestSnapshotToTimeline()
		{
			bool flag = !this._simulation.IsClient || !this._simulation.HasRuntimeConfig || !this.HasSnapshots;
			if (!flag)
			{
				Tick tick = this.SnapshotLatest.Tick;
				Tick maxBehaviourChangedTick = this.GetMaxBehaviourChangedTick(this.SnapshotLatest);
				this.Timeline.AddPoint(new TimelinePoint(tick, maxBehaviourChangedTick, this._simulation.TickDeltaDouble), this._simulation.TickDeltaDouble, true);
			}
		}

		internal NetworkObjectHeaderSnapshot FindSnapshot(Tick tick)
		{
			bool flag = this._snapshotsByIndexLatest != null;
			if (flag)
			{
				Assert.Check(this._snapshotsByIndex);
				int num = this._snapshotsByIndexLatest.Value - tick;
				bool flag2 = num < this._snapshotsByIndex.Length;
				if (flag2)
				{
					return this._snapshotsByIndex[num];
				}
			}
			for (NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot = this._snapshots.Latest; networkObjectHeaderSnapshot != null; networkObjectHeaderSnapshot = networkObjectHeaderSnapshot.Next)
			{
				bool flag3 = networkObjectHeaderSnapshot.Tick == tick;
				if (flag3)
				{
					return networkObjectHeaderSnapshot;
				}
			}
			return null;
		}

		internal bool TryFindSnapshot(Tick tick, out NetworkObjectHeaderSnapshot snapshot)
		{
			NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot;
			snapshot = (networkObjectHeaderSnapshot = this.FindSnapshot(tick));
			return networkObjectHeaderSnapshot != null;
		}

		internal unsafe void Init(int* words, short wordCount, short behaviourCount, NetworkObjectHeaderFlags flags)
		{
			Assert.Check(this._ptr == null);
			Assert.Check(BehaviourUtils.IsNull(this.Instance));
			this._ptr = words;
			this.WordCount = wordCount;
			this.BehaviourCount = behaviourCount;
			this._flags = flags;
			Assert.Check(this.Header.WordCount == wordCount);
			Assert.Check(this.Header.BehaviourCount == behaviourCount);
			Assert.Check(this.Header.Flags == flags);
		}

		internal unsafe int* GetBehaviourPtr(NetworkBehaviour behaviour)
		{
			return this._ptr + behaviour.WordOffset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int EncodePriorityLevel(int level)
		{
			return Maths.Clamp(level, 0, 4) + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int DecodePriorityLevel(int level)
		{
			return level - 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsIdle(int level)
		{
			return level < -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool IsActive(int level)
		{
			return level >= 1 && level <= 5;
		}

		internal int GetPriority(PlayerRef player)
		{
			bool flag = this._simulation.Topology == Topologies.ClientServer;
			if (flag)
			{
				bool flag2 = this.Header.InputAuthority == player && player.IsRealPlayer;
				if (flag2)
				{
					return NetworkObjectMeta.EncodePriorityLevel(0);
				}
				bool flag3 = BehaviourUtils.IsAlive(this.Instance);
				if (flag3)
				{
					bool flag4 = this.Instance.PriorityCallback != null;
					if (flag4)
					{
						try
						{
							return NetworkObjectMeta.EncodePriorityLevel(this.Instance.PriorityCallback(this.Instance, player) - PriorityLevel.Player);
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
			return NetworkObjectMeta.EncodePriorityLevel(4);
		}

		internal void LinkInstance(NetworkObject instance)
		{
			Assert.Check(BehaviourUtils.IsNotNull(instance));
			Assert.Check(BehaviourUtils.IsNull(this.Instance));
			Assert.Check(instance.Ptr == null);
			Assert.Check(this._ptr != null);
			instance.Ptr = this._ptr;
			instance.Meta = this;
			this.Instance = instance;
		}

		internal void UnlinkInstance(NetworkObject instance)
		{
			Assert.Check(this.Instance == instance);
			Assert.Check(this == instance.Meta);
			this.Instance = null;
			instance.Meta = null;
		}

		private static NetworkBufferSerializerInfo[] _serializersStatic;

		private static NetworkBufferSerializerInfo[] _serializersNone = Array.Empty<NetworkBufferSerializerInfo>();

		private Allocator _allocator;

		private unsafe int* _changes;

		private Simulation _simulation;

		private NetworkObjectHeaderSnapshot _shadow;

		private NetworkObjectHeaderSnapshot _render;

		private NetworkObjectHeaderSnapshot _previous;

		private NetworkObjectHeaderSnapshot _migration;

		private NetworkObjectHeaderSnapshotList _snapshots;

		private NetworkObjectHeaderSnapshot[] _snapshotsByIndex;

		private Tick? _snapshotsByIndexLatest;

		private Timeline _timeline;

		private NetworkObjectMeta _prev;

		private NetworkObjectMeta _next;

		private NetworkObjectMeta _prevMigration;

		private NetworkObjectMeta _nextMigration;

		internal Tick ScannedTick;

		internal Tick ChangedTick;

		internal int AreaOfInterestCell;

		internal NetworkObjectMetaFlags LocalFlags;

		internal NetworkObjectHeader.PlayerUniqueData PlayerData;

		private unsafe int* _ptr;

		internal NetworkObject Instance;

		internal short WordCount;

		internal short BehaviourCount;

		private NetworkObjectHeaderFlags _flags;

		internal const int PRIORITY_IDLE = -32768;

		internal const int PRIORITY_LEVEL_PLAYER = 0;

		internal const int PRIORITY_LEVEL_HIGH = 1;

		internal const int PRIORITY_LEVEL_MED = 2;

		internal const int PRIORITY_LEVEL_LOW = 3;

		internal const int PRIORITY_LEVEL_LOWEST = 4;

		internal const int PRIORITY_LEVEL_COUNT = 5;

		internal struct List
		{
			public static NetworkObjectMeta Next(NetworkObjectMeta item)
			{
				return item._next;
			}

			public void AddFirst(NetworkObjectMeta item)
			{
				Assert.Check(!this.IsInList(item));
				item._next = this.Head;
				item._prev = null;
				bool flag = this.Head != null;
				if (flag)
				{
					this.Head._prev = item;
					this.Head = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public void AddLast(NetworkObjectMeta item)
			{
				Assert.Check(!this.IsInList(item));
				item._next = null;
				item._prev = this.Tail;
				bool flag = this.Tail != null;
				if (flag)
				{
					this.Tail._next = item;
					this.Tail = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public void AddBefore(NetworkObjectMeta item, NetworkObjectMeta before)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.IsInList(before));
				Assert.Check(!this.IsInList(item));
				bool flag = before == this.Head;
				if (flag)
				{
					this.AddFirst(item);
				}
				else
				{
					Assert.Check(this.Count > 1);
					Assert.Check(before._prev != null);
					item._next = before;
					item._prev = before._prev;
					before._prev._next = item;
					before._prev = item;
					this.Count++;
				}
				Assert.Check(this.IsInList(before));
				Assert.Check(this.IsInList(item));
			}

			public void AddAfter(NetworkObjectMeta item, NetworkObjectMeta after)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.IsInList(after));
				Assert.Check(!this.IsInList(item));
				bool flag = after == this.Tail;
				if (flag)
				{
					this.AddLast(item);
				}
				else
				{
					Assert.Check(this.Count > 1);
					Assert.Check(after._next != null);
					item._next = after._next;
					item._prev = after;
					after._next._prev = item;
					after._next = item;
					this.Count++;
				}
				Assert.Check(this.IsInList(after));
				Assert.Check(this.IsInList(item));
			}

			public NetworkObjectMeta RemoveHead()
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.Head != null);
				Assert.Check(this.IsInList(this.Head));
				NetworkObjectMeta head = this.Head;
				this.Remove(head);
				return head;
			}

			public void Remove(NetworkObjectMeta item)
			{
				bool flag = !this.IsInList(item);
				if (!flag)
				{
					bool flag2 = item._prev != null;
					if (flag2)
					{
						item._prev._next = item._next;
					}
					bool flag3 = item._next != null;
					if (flag3)
					{
						item._next._prev = item._prev;
					}
					bool flag4 = item == this.Tail;
					if (flag4)
					{
						this.Tail = item._prev;
					}
					bool flag5 = item == this.Head;
					if (flag5)
					{
						this.Head = item._next;
					}
					item._prev = null;
					item._next = null;
					this.Count--;
				}
			}

			private bool IsInList(NetworkObjectMeta item)
			{
				for (NetworkObjectMeta networkObjectMeta = this.Head; networkObjectMeta != null; networkObjectMeta = networkObjectMeta._next)
				{
					bool flag = networkObjectMeta == item;
					if (flag)
					{
						return true;
					}
				}
				return false;
			}

			public NetworkObjectMeta.List RemoveAll()
			{
				NetworkObjectMeta.List result = this;
				this.Head = null;
				this.Tail = null;
				this.Count = 0;
				return result;
			}

			public void Concat(NetworkObjectMeta.List other)
			{
				bool flag = other.Count == 0;
				if (!flag)
				{
					bool flag2 = this.Count == 0;
					if (flag2)
					{
						this.Count = other.Count;
						this.Head = other.Head;
						this.Tail = other.Tail;
					}
					else
					{
						Assert.Check(!this.IsInList(other.Head));
						Assert.Check(this.Tail != null);
						Assert.Check(this.Tail._next == null);
						Assert.Check(other.Head != null);
						Assert.Check(other.Head._prev == null);
						this.Tail._next = other.Head;
						other.Head._prev = this.Tail;
						this.Tail = other.Tail;
						this.Count += other.Count;
					}
				}
			}

			public int Count;

			public NetworkObjectMeta Head;

			public NetworkObjectMeta Tail;
		}

		internal struct ListMigration
		{
			public static NetworkObjectMeta Next(NetworkObjectMeta item)
			{
				return item._nextMigration;
			}

			public void AddFirst(NetworkObjectMeta item)
			{
				Assert.Check(!this.IsInList(item));
				item._nextMigration = this.Head;
				item._prevMigration = null;
				bool flag = this.Head != null;
				if (flag)
				{
					this.Head._prevMigration = item;
					this.Head = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public void AddLast(NetworkObjectMeta item)
			{
				Assert.Check(!this.IsInList(item));
				item._nextMigration = null;
				item._prevMigration = this.Tail;
				bool flag = this.Tail != null;
				if (flag)
				{
					this.Tail._nextMigration = item;
					this.Tail = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public void AddBefore(NetworkObjectMeta item, NetworkObjectMeta before)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.IsInList(before));
				Assert.Check(!this.IsInList(item));
				bool flag = before == this.Head;
				if (flag)
				{
					this.AddFirst(item);
				}
				else
				{
					Assert.Check(this.Count > 1);
					Assert.Check(before._prevMigration != null);
					item._nextMigration = before;
					item._prevMigration = before._prevMigration;
					before._prevMigration._nextMigration = item;
					before._prevMigration = item;
					this.Count++;
				}
				Assert.Check(this.IsInList(before));
				Assert.Check(this.IsInList(item));
			}

			public void AddAfter(NetworkObjectMeta item, NetworkObjectMeta after)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.IsInList(after));
				Assert.Check(!this.IsInList(item));
				bool flag = after == this.Tail;
				if (flag)
				{
					this.AddLast(item);
				}
				else
				{
					Assert.Check(this.Count > 1);
					Assert.Check(after._nextMigration != null);
					item._nextMigration = after._nextMigration;
					item._prevMigration = after;
					after._nextMigration._prevMigration = item;
					after._nextMigration = item;
					this.Count++;
				}
				Assert.Check(this.IsInList(after));
				Assert.Check(this.IsInList(item));
			}

			public NetworkObjectMeta RemoveHead()
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.Head != null);
				Assert.Check(this.IsInList(this.Head));
				NetworkObjectMeta head = this.Head;
				this.Remove(head);
				return head;
			}

			public void Remove(NetworkObjectMeta item)
			{
				Assert.Check(this.IsInList(item));
				bool flag = item._prevMigration != null;
				if (flag)
				{
					item._prevMigration._nextMigration = item._nextMigration;
				}
				bool flag2 = item._nextMigration != null;
				if (flag2)
				{
					item._nextMigration._prevMigration = item._prevMigration;
				}
				bool flag3 = item == this.Tail;
				if (flag3)
				{
					this.Tail = item._prevMigration;
				}
				bool flag4 = item == this.Head;
				if (flag4)
				{
					this.Head = item._nextMigration;
				}
				item._prevMigration = null;
				item._nextMigration = null;
				this.Count--;
			}

			private bool IsInList(NetworkObjectMeta item)
			{
				for (NetworkObjectMeta networkObjectMeta = this.Head; networkObjectMeta != null; networkObjectMeta = networkObjectMeta._nextMigration)
				{
					bool flag = networkObjectMeta == item;
					if (flag)
					{
						return true;
					}
				}
				return false;
			}

			public int Count;

			public NetworkObjectMeta Head;

			public NetworkObjectMeta Tail;
		}
	}
}
