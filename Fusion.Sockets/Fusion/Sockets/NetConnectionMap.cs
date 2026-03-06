using System;

namespace Fusion.Sockets
{
	public struct NetConnectionMap
	{
		public unsafe static void Dispose(ref NetConnectionMap* map, INetPeerGroupCallbacks callbacks)
		{
			bool flag = map == (IntPtr)((UIntPtr)0);
			if (!flag)
			{
				for (int i = 0; i < (int)map.CapacityUsable; i++)
				{
					NetConnection* ptr = map.Connections + i;
					while (ptr->NotifySendWindow.Count > 0)
					{
						NetSendEnvelope netSendEnvelope = ptr->NotifySendWindow.Peek();
						ptr->NotifySendWindow.Pop();
						callbacks.OnNotifyDispose(ref netSendEnvelope);
					}
					ptr->NotifySendWindow.Dispose();
					Native.Free<byte>(ref ptr->NotifyRecvFragmentBuffer);
					Native.Free<byte>(ref ptr->ConnectionToken);
					ptr->ConnectionTokenLength = 0;
					Native.Free<byte>(ref ptr->UniqueId);
					ptr->UniqueIdHash = 0L;
					ptr->ReliableSendList.Dispose();
					ptr->ReliableBuffer.Dispose();
				}
				Native.Free<NetConnectionMap>(ref map);
			}
		}

		public unsafe static NetConnectionMap* Allocate(int capacity, short groupIndex, in NetConfig* config)
		{
			Assert.Check(capacity >= 0);
			int nextPrime = Primes.GetNextPrime(capacity);
			int num = Native.RoundToMaxAlignment(sizeof(NetConnectionMap));
			int num2 = Native.RoundToMaxAlignment(sizeof(NetConnection*) * nextPrime);
			int num3 = Native.RoundToMaxAlignment(sizeof(NetConnection) * nextPrime);
			int num4 = Native.RoundToMaxAlignment(sizeof(NetConnectionMap.UniqueIdMapping) * nextPrime);
			byte* ptr = (byte*)Native.MallocAndClear(num + num2 + num3 + num4);
			NetConnectionMap* ptr2 = (NetConnectionMap*)ptr;
			ptr2->Buckets = (NetConnection**)(ptr + num);
			ptr2->Connections = (NetConnection*)(ptr + num + num2);
			ptr2->UniqueIdHashes = (NetConnectionMap.UniqueIdMapping*)(ptr + num + num2 + num3);
			ptr2->Group = groupIndex;
			ptr2->UsedCount = 0UL;
			ptr2->FreeCount = 0UL;
			ptr2->IdsCount = 0UL;
			ptr2->CapacityAllocated = (ulong)((long)nextPrime);
			ptr2->CapacityUsable = (ulong)((long)capacity);
			for (int i = 0; i < nextPrime; i++)
			{
				ptr2->UniqueIdHashes[i] = default(NetConnectionMap.UniqueIdMapping);
			}
			short num5 = 0;
			while ((int)num5 < capacity)
			{
				NetConnection.Initialize(ptr2->Connections + num5, groupIndex, num5, config);
				num5 += 1;
			}
			return ptr2;
		}

		public int Count
		{
			get
			{
				return (int)(this.UsedCount - this.FreeCount);
			}
		}

		public int CountUsed
		{
			get
			{
				return (int)this.UsedCount;
			}
		}

		public unsafe NetConnection* ConnectionsBuffer
		{
			get
			{
				return this.Connections;
			}
		}

		public bool Full
		{
			get
			{
				return this.UsedCount == this.CapacityAllocated;
			}
		}

		public unsafe NetConnection* Remap(NetAddress oldAddress, NetAddress newAddress)
		{
			ulong num = NetAddress.Hash64(oldAddress);
			ulong num2 = NetAddress.Hash64(newAddress);
			ulong num3 = num % this.CapacityAllocated;
			NetConnection* ptr = *(IntPtr*)(this.Buckets + num3 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*));
			NetConnection* ptr2 = default(NetConnection*);
			ulong num4 = num2 % this.CapacityAllocated;
			while (ptr != null)
			{
				bool flag = ptr->MapHash == num && ptr->Address.Block0 == oldAddress.Block0 && ptr->Address.Block1 == oldAddress.Block1 && ptr->Address.Block2 == oldAddress.Block2;
				if (flag)
				{
					Assert.Check(ptr->MapState == NetConnectionMap.EntryState.Used);
					bool flag2 = ptr2 == null;
					if (flag2)
					{
						*(IntPtr*)(this.Buckets + num3 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*)) = ptr->MapNext;
					}
					else
					{
						ptr2->MapNext = ptr->MapNext;
					}
					ptr->Address = newAddress;
					ptr->MapHash = num2;
					ptr->MapNext = *(IntPtr*)(this.Buckets + num4 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*));
					*(IntPtr*)(this.Buckets + num4 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*)) = ptr;
					return ptr;
				}
				ptr2 = ptr;
				ptr = ptr->MapNext;
			}
			Assert.AlwaysFail(string.Format("Remap failed from {0} to {1}", oldAddress, newAddress));
			return null;
		}

		public unsafe bool Remove(NetAddress address)
		{
			ulong num = NetAddress.Hash64(address);
			ulong num2 = num % this.CapacityAllocated;
			NetConnection* ptr = *(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*));
			NetConnection* ptr2 = default(NetConnection*);
			while (ptr != null)
			{
				bool flag = ptr->MapHash == num && ptr->Address.Block0 == address.Block0 && ptr->Address.Block1 == address.Block1 && ptr->Address.Block2 == address.Block2;
				if (flag)
				{
					bool flag2 = ptr2 == null;
					if (flag2)
					{
						*(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*)) = ptr->MapNext;
					}
					else
					{
						ptr2->MapNext = ptr->MapNext;
					}
					Assert.Check(ptr->MapState == NetConnectionMap.EntryState.Used);
					this.RemoveUniqueId(ptr->UniqueIdHash);
					NetConnection.Reset(ptr);
					ptr->MapNext = this.FreeHead;
					ptr->MapState = NetConnectionMap.EntryState.Free;
					this.FreeHead = ptr;
					this.FreeCount += 1UL;
					return true;
				}
				ptr2 = ptr;
				ptr = ptr->MapNext;
			}
			return false;
		}

		public unsafe NetConnection* Insert(NetAddress address, byte[] uniqueId)
		{
			Assert.Check(this.Find(address) == null);
			Assert.Check(!address.Equals(default(NetAddress)));
			long num = BitConverter.ToInt64(uniqueId, 0);
			short index;
			bool flag = this.ContainsUniqueId(num, out index);
			NetConnection* result;
			if (flag)
			{
				NetAddress address2 = this.FindByIndex((int)index)->Address;
				NetConnection* ptr = this.Remap(address2, address);
				this.StoreUniqueId(num, ptr->LocalId.GroupIndex);
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Log(string.Format("UniqueId ({0}) already used. Update connection address from {1} to {2}", num, address2, address));
				}
				result = null;
			}
			else
			{
				ulong num2 = NetAddress.Hash64(address);
				ulong num3 = num2 % this.CapacityAllocated;
				bool flag2 = this.FreeHead != null;
				NetConnection* ptr2;
				if (flag2)
				{
					Assert.Check(this.FreeCount > 0UL);
					ptr2 = this.FreeHead;
					this.FreeHead = ptr2->MapNext;
					this.FreeCount -= 1UL;
					Assert.Check(ptr2->MapState == NetConnectionMap.EntryState.Free);
				}
				else
				{
					bool flag3 = this.UsedCount == this.CapacityUsable;
					if (flag3)
					{
						return null;
					}
					NetConnection* connections = this.Connections;
					ulong usedCount = this.UsedCount;
					this.UsedCount = usedCount + 1UL;
					ptr2 = connections + usedCount * (ulong)((long)sizeof(NetConnection)) / (ulong)sizeof(NetConnection);
					Assert.Check(ptr2->MapState == NetConnectionMap.EntryState.None);
					Assert.Check(ptr2->MapNext == null);
				}
				Assert.Check(ptr2 == this.Connections + ptr2->LocalId.GroupIndex);
				ptr2->Address = address;
				ptr2->MapHash = num2;
				ptr2->MapState = NetConnectionMap.EntryState.Used;
				ptr2->MapNext = *(IntPtr*)(this.Buckets + num3 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*));
				bool flag4 = ptr2->UniqueId == null;
				if (flag4)
				{
					ptr2->UniqueId = (byte*)Native.MallocAndClear(uniqueId.Length);
				}
				ptr2->UniqueIdHash = num;
				fixed (byte[] array = uniqueId)
				{
					byte* source;
					if (uniqueId == null || array.Length == 0)
					{
						source = null;
					}
					else
					{
						source = &array[0];
					}
					Native.MemCpy((void*)ptr2->UniqueId, (void*)source, 8);
				}
				this.StoreUniqueId(num, ptr2->LocalId.GroupIndex);
				*(IntPtr*)(this.Buckets + num3 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*)) = ptr2;
				result = ptr2;
			}
			return result;
		}

		public unsafe NetConnection* FindByIndex(int index)
		{
			bool flag = index >= 0 && index < (int)this.CapacityUsable;
			if (flag)
			{
				return this.Connections + index;
			}
			throw new IndexOutOfRangeException();
		}

		public unsafe bool TryFindByIndex(int index, out NetConnection* connection)
		{
			bool flag = index >= 0 && index < (int)this.CapacityUsable;
			bool result;
			if (flag)
			{
				connection = this.Connections + index;
				result = true;
			}
			else
			{
				connection = (IntPtr)((UIntPtr)0);
				result = false;
			}
			return result;
		}

		public unsafe NetConnection* Find(NetConnectionId id)
		{
			Assert.Check(this.Group == id.Group);
			NetConnection* ptr = this.Connections + id.GroupIndex;
			bool flag = ptr->LocalId.Raw == id.Raw;
			NetConnection* result;
			if (flag)
			{
				result = ptr;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public unsafe NetConnection* Find(NetAddress address)
		{
			ulong num = NetAddress.Hash64(address);
			ulong num2 = num % this.CapacityAllocated;
			for (NetConnection* ptr = *(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetConnection*)) / (ulong)sizeof(NetConnection*)); ptr != null; ptr = ptr->MapNext)
			{
				bool flag = ptr->MapHash == num && ptr->Address.Block0 == address.Block0 && ptr->Address.Block1 == address.Block1 && ptr->Address.Block2 == address.Block2;
				if (flag)
				{
					return ptr;
				}
			}
			return null;
		}

		private unsafe bool ContainsUniqueId(long value, out short groupIndex)
		{
			groupIndex = -1;
			ulong num = this.FindInsertionIndex(value);
			bool flag = num < this.IdsCount && this.UniqueIdHashes[num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].UniqueId == value;
			bool result;
			if (flag)
			{
				groupIndex = this.UniqueIdHashes[num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].Index;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private unsafe void StoreUniqueId(long value, short groupIndex)
		{
			ulong num = this.FindInsertionIndex(value);
			bool flag = this.UniqueIdHashes[num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].UniqueId == value;
			if (flag)
			{
				this.UniqueIdHashes[num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].Index = groupIndex;
			}
			else
			{
				Native.MemMove((void*)(this.UniqueIdHashes + num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping) + 1), (void*)(this.UniqueIdHashes + num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)), (int)(this.IdsCount - num) * sizeof(NetConnectionMap.UniqueIdMapping));
				this.UniqueIdHashes[num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].UniqueId = value;
				this.UniqueIdHashes[num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].Index = groupIndex;
				Assert.Check(this.IdsCount + 1UL <= this.CapacityUsable, "Unique Ids count exceeds capacity");
				this.IdsCount += 1UL;
			}
		}

		private unsafe bool RemoveUniqueId(long value)
		{
			ulong num = this.FindInsertionIndex(value);
			bool flag = num >= this.IdsCount || this.UniqueIdHashes[num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].UniqueId != value;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Native.MemMove((void*)(this.UniqueIdHashes + num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)), (void*)(this.UniqueIdHashes + num * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping) + 1), (int)(this.IdsCount - num - 1UL) * sizeof(NetConnectionMap.UniqueIdMapping));
				Assert.Check(this.IdsCount > 0UL, "Unique Ids count is already 0");
				this.IdsCount -= 1UL;
				result = true;
			}
			return result;
		}

		private unsafe ulong FindInsertionIndex(long value)
		{
			ulong num = 0UL;
			ulong num2 = this.IdsCount;
			while (num < num2)
			{
				ulong num3 = (num + num2) / 2UL;
				bool flag = this.UniqueIdHashes[num3 * (ulong)((long)sizeof(NetConnectionMap.UniqueIdMapping)) / (ulong)sizeof(NetConnectionMap.UniqueIdMapping)].UniqueId < value;
				if (flag)
				{
					num = num3 + 1UL;
				}
				else
				{
					num2 = num3;
				}
			}
			return num;
		}

		private unsafe NetConnection** Buckets;

		private unsafe NetConnection* FreeHead;

		internal unsafe NetConnection* Connections;

		private unsafe NetConnectionMap.UniqueIdMapping* UniqueIdHashes;

		private short Group;

		private ulong UsedCount;

		private ulong FreeCount;

		private ulong IdsCount;

		private ulong CapacityAllocated;

		internal ulong CapacityUsable;

		public enum EntryState
		{
			None,
			Free,
			Used
		}

		private struct UniqueIdMapping
		{
			public long UniqueId;

			public short Index;
		}

		public struct Iterator
		{
			public unsafe NetConnection* Current
			{
				get
				{
					return this.IsValid ? (this._map->Connections + this._index) : null;
				}
			}

			public unsafe Iterator(NetConnectionMap* map)
			{
				this._map = map;
				this._index = -1;
				this._count = (int)this._map->UsedCount;
			}

			public bool IsValid
			{
				get
				{
					return this._index >= 0 && this._index < this._count;
				}
			}

			public unsafe bool Next()
			{
				bool flag;
				do
				{
					int num = this._index + 1;
					this._index = num;
					if (num >= this._count)
					{
						goto Block_3;
					}
					flag = (this._map->Connections[this._index].MapState == NetConnectionMap.EntryState.Used && this._map->Connections[this._index].Status < NetConnectionStatus.Disconnected);
				}
				while (!flag);
				return true;
				Block_3:
				return false;
			}

			private unsafe NetConnectionMap* _map;

			private int _index;

			private int _count;
		}
	}
}
