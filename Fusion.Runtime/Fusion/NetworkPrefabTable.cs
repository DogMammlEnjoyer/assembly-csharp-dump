using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public class NetworkPrefabTable
	{
		public IReadOnlyList<INetworkPrefabSource> Prefabs
		{
			get
			{
				return this._sources;
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
		}

		public IEnumerable<ValueTuple<NetworkPrefabId, INetworkPrefabSource>> GetEntries()
		{
			NetworkPrefabTable.<GetEntries>d__12 <GetEntries>d__ = new NetworkPrefabTable.<GetEntries>d__12(-2);
			<GetEntries>d__.<>4__this = this;
			return <GetEntries>d__;
		}

		public NetworkPrefabId AddSource(INetworkPrefabSource source)
		{
			NetworkPrefabId networkPrefabId;
			bool flag = !this.TryAddSource(source, out networkPrefabId);
			if (flag)
			{
				throw new ArgumentException(string.Format("Source with guid {0} already exists: {1}", source.AssetGuid, networkPrefabId), "source");
			}
			return networkPrefabId;
		}

		public bool TryAddSource(INetworkPrefabSource source, out NetworkPrefabId id)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			NetworkObjectGuid assetGuid = source.AssetGuid;
			bool isValid = assetGuid.IsValid;
			if (isValid)
			{
				int index;
				bool flag2 = this._guidToIndex.TryGetValue(assetGuid, out index);
				if (flag2)
				{
					id = NetworkPrefabId.FromIndex(index);
					return false;
				}
				this._guidToIndex.Add(assetGuid, this._sources.Count);
			}
			this._sources.Add(source);
			bool flag3 = this._acquireMask.Length < this.GetBitSetCapacity(this._sources.Capacity);
			if (flag3)
			{
				Array.Resize<BitSet64>(ref this._acquireMask, this.GetBitSetCapacity(this._sources.Capacity));
			}
			bool flag4 = this._acquireData.Length < this._sources.Capacity;
			if (flag4)
			{
				Array.Resize<NetworkPrefabTable.PrefabAcquireData>(ref this._acquireData, this._sources.Capacity);
			}
			id = NetworkPrefabId.FromIndex(this._sources.Count - 1);
			TraceLogStream logTracePrefab = InternalLogStreams.LogTracePrefab;
			if (logTracePrefab != null)
			{
				logTracePrefab.Log(string.Format("Added prefab source {0}: {1}", id, source.Description));
			}
			this._version++;
			return true;
		}

		public INetworkPrefabSource GetSource(NetworkObjectGuid guid)
		{
			int index;
			bool flag = this._guidToIndex.TryGetValue(guid, out index);
			INetworkPrefabSource result;
			if (flag)
			{
				result = this._sources[index];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public INetworkPrefabSource GetSource(NetworkPrefabId prefabId)
		{
			int index;
			bool flag = !this.TryDecodePrefabId(prefabId, out index);
			INetworkPrefabSource result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this._sources[index];
			}
			return result;
		}

		public NetworkPrefabId GetId(NetworkObjectGuid guid)
		{
			int index;
			bool flag = this._guidToIndex.TryGetValue(guid, out index);
			NetworkPrefabId result;
			if (flag)
			{
				result = NetworkPrefabId.FromIndex(index);
			}
			else
			{
				result = default(NetworkPrefabId);
			}
			return result;
		}

		public NetworkObjectGuid GetGuid(NetworkPrefabId prefabId)
		{
			int index;
			bool flag = !this.TryDecodePrefabId(prefabId, out index);
			NetworkObjectGuid result;
			if (flag)
			{
				result = default(NetworkObjectGuid);
			}
			else
			{
				result = this._sources[index].AssetGuid;
			}
			return result;
		}

		public int GetInstancesCount(NetworkPrefabId prefabId)
		{
			int num;
			bool flag = !this.TryDecodePrefabId(prefabId, out num) || !this.IsAcquired(num);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = this._acquireData[num].InstanceCount;
			}
			return result;
		}

		public int AddInstance(NetworkPrefabId prefabId)
		{
			int num;
			bool flag = !this.TryDecodePrefabId(prefabId, out num) || !this.IsAcquired(num);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				TraceLogStream logTracePrefab = InternalLogStreams.LogTracePrefab;
				if (logTracePrefab != null)
				{
					logTracePrefab.Log(string.Format("Increasing {0} instance count (to {1})", prefabId, this._acquireData[num].InstanceCount + 1));
				}
				this._version++;
				NetworkPrefabTable.PrefabAcquireData[] acquireData = this._acquireData;
				int num2 = num;
				int num3 = acquireData[num2].InstanceCount + 1;
				acquireData[num2].InstanceCount = num3;
				result = num3;
			}
			return result;
		}

		public int RemoveInstance(NetworkPrefabId prefabId)
		{
			int num;
			bool flag = !this.TryDecodePrefabId(prefabId, out num) || !this.IsAcquired(num);
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = this._acquireData[num].InstanceCount == 0;
				if (flag2)
				{
					TraceLogStream logTracePrefab = InternalLogStreams.LogTracePrefab;
					if (logTracePrefab != null)
					{
						logTracePrefab.Warn(string.Format("Can't decrease {0} instance count below zero", prefabId));
					}
					result = 0;
				}
				else
				{
					TraceLogStream logTracePrefab2 = InternalLogStreams.LogTracePrefab;
					if (logTracePrefab2 != null)
					{
						logTracePrefab2.Log(string.Format("Decreasing {0} instance count (to {1})", prefabId, this._acquireData[num].InstanceCount - 1));
					}
					NetworkPrefabTable.PrefabAcquireData[] acquireData = this._acquireData;
					int num2 = num;
					int num3 = acquireData[num2].InstanceCount - 1;
					acquireData[num2].InstanceCount = num3;
					int num4 = num3;
					bool flag3 = num4 == 0 && this.Options.UnloadPrefabOnReleasingLastInstance;
					if (flag3)
					{
						this.UnloadInternal(num);
					}
					this._version++;
					result = num4;
				}
			}
			return result;
		}

		public bool Contains(NetworkPrefabId prefabId)
		{
			int num;
			return this.TryDecodePrefabId(prefabId, out num);
		}

		public bool IsAcquired(NetworkPrefabId prefabId)
		{
			int index;
			bool flag = !this.TryDecodePrefabId(prefabId, out index);
			return !flag && this.IsAcquired(index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsAcquired(int index)
		{
			int num = index / 64;
			return this._acquireMask[num][index & 63];
		}

		private void SetAcquired(int index, bool value)
		{
			int num = index / 64;
			this._acquireMask[num][index & 63] = value;
			this._version++;
		}

		public NetworkObject Load(NetworkPrefabId prefabId, bool isSynchronous)
		{
			int num = this.DecodePrefabId(prefabId);
			INetworkPrefabSource networkPrefabSource = this._sources[num];
			bool flag = !this.IsAcquired(num);
			if (flag)
			{
				TraceLogStream logTracePrefab = InternalLogStreams.LogTracePrefab;
				if (logTracePrefab != null)
				{
					logTracePrefab.Log(string.Format("Acquiring {0} ({1})", prefabId, networkPrefabSource.Description));
				}
				networkPrefabSource.Acquire(isSynchronous);
				this.SetAcquired(num, true);
				this._acquireData[num] = new NetworkPrefabTable.PrefabAcquireData
				{
					IsSynchronous = isSynchronous
				};
			}
			bool flag2 = !networkPrefabSource.IsCompleted;
			if (flag2)
			{
				bool flag3 = isSynchronous && !this._acquireData[num].IsSynchronous;
				if (!flag3)
				{
					return null;
				}
				networkPrefabSource.Release();
				networkPrefabSource.Acquire(true);
				this._acquireData[num].IsSynchronous = true;
				bool flag4 = !networkPrefabSource.IsCompleted;
				if (flag4)
				{
					return null;
				}
			}
			NetworkObject networkObject = networkPrefabSource.WaitForResult();
			Assert.Always<NetworkPrefabId>(networkObject, "Source for {0} returned null", prefabId);
			return networkObject;
		}

		public bool Unload(NetworkPrefabId prefabId)
		{
			int index;
			bool flag = !this.TryDecodePrefabId(prefabId, out index);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.IsAcquired(index);
				if (flag2)
				{
					result = false;
				}
				else
				{
					this.UnloadInternal(index);
					result = true;
				}
			}
			return result;
		}

		public int UnloadUnreferenced(bool includeIncompleteLoads = false)
		{
			int result = 0;
			for (int i = 0; i < this._sources.Count; i += 64)
			{
				BitSet64 bitSet = this._acquireMask[i / 64];
				bool flag = !bitSet.Any();
				if (!flag)
				{
					foreach (int num in bitSet)
					{
						bool flag2 = i + num >= this._sources.Count;
						if (flag2)
						{
							break;
						}
						int index = i + num;
						bool flag3 = !includeIncompleteLoads && this._sources[index].IsCompleted;
						if (flag3)
						{
							TraceLogStream logTracePrefab = InternalLogStreams.LogTracePrefab;
							if (logTracePrefab != null)
							{
								logTracePrefab.Log(string.Format("Not unloading {0}: incomplete load", NetworkPrefabId.FromIndex(index)));
							}
						}
						else
						{
							this.UnloadInternal(i + num);
						}
					}
				}
			}
			return result;
		}

		public void UnloadAll()
		{
			for (int i = 0; i < this._sources.Count; i++)
			{
				this.Unload(NetworkPrefabId.FromIndex(i));
			}
		}

		public void Clear()
		{
			this.UnloadAll();
			Array.Clear(this._acquireData, 0, this._sources.Count);
			this._acquireMask = Array.Empty<BitSet64>();
			this._sources.Clear();
			this._guidToIndex.Clear();
			this._version = 0;
		}

		private void UnloadInternal(int index)
		{
			TraceLogStream logTracePrefab = InternalLogStreams.LogTracePrefab;
			if (logTracePrefab != null)
			{
				logTracePrefab.Log(string.Format("Unloading {0}", NetworkPrefabId.FromIndex(index)));
			}
			INetworkPrefabSource networkPrefabSource = this._sources[index];
			networkPrefabSource.Release();
			this.SetAcquired(index, false);
			this._version++;
		}

		private int DecodePrefabId(NetworkPrefabId prefabId)
		{
			bool flag = !prefabId.IsValid;
			if (flag)
			{
				throw new ArgumentException("Invalid prefab id", "prefabId");
			}
			bool flag2 = prefabId.AsIndex >= this._sources.Count;
			if (flag2)
			{
				throw new ArgumentException(string.Format("Unknown prefab id: {0}", prefabId), "prefabId");
			}
			return prefabId.AsIndex;
		}

		private bool TryDecodePrefabId(NetworkPrefabId prefabId, out int index)
		{
			bool flag = !prefabId.IsValid;
			bool result;
			if (flag)
			{
				index = 0;
				result = false;
			}
			else
			{
				bool flag2 = prefabId.AsIndex >= this._sources.Count;
				if (flag2)
				{
					index = 0;
					result = false;
				}
				else
				{
					index = prefabId.AsIndex;
					result = true;
				}
			}
			return result;
		}

		private int GetBitSetCapacity(int length)
		{
			return (this._sources.Capacity + 63) / 64;
		}

		public NetworkPrefabTableOptions Options = NetworkPrefabTableOptions.Default;

		private List<INetworkPrefabSource> _sources = new List<INetworkPrefabSource>();

		private BitSet64[] _acquireMask = Array.Empty<BitSet64>();

		private const int BitsPerMask = 64;

		private NetworkPrefabTable.PrefabAcquireData[] _acquireData = Array.Empty<NetworkPrefabTable.PrefabAcquireData>();

		private Dictionary<NetworkObjectGuid, int> _guidToIndex = new Dictionary<NetworkObjectGuid, int>();

		private int _version;

		private struct PrefabAcquireData
		{
			public int InstanceCount
			{
				get
				{
					return (int)(this.RawValue & 2147483647U);
				}
				set
				{
					this.RawValue = ((this.RawValue & 2147483648U) | (uint)(value & int.MaxValue));
				}
			}

			public bool IsSynchronous
			{
				get
				{
					return (this.RawValue & 2147483648U) > 0U;
				}
				set
				{
					this.RawValue = ((this.RawValue & 2147483647U) | (value ? 2147483648U : 0U));
				}
			}

			public uint RawValue;
		}
	}
}
