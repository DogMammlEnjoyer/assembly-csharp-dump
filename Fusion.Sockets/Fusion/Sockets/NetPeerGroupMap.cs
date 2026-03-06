using System;

namespace Fusion.Sockets
{
	internal struct NetPeerGroupMap
	{
		public unsafe static void Dispose(ref NetPeerGroupMap* map)
		{
			Native.Free<NetPeerGroupMap>(ref map);
		}

		public unsafe static NetPeerGroupMap* Allocate(int capacity)
		{
			Assert.Check(capacity >= 0);
			int nextPrime = Primes.GetNextPrime(capacity * 2);
			int num = Native.RoundToMaxAlignment(sizeof(NetPeerGroupMap));
			int num2 = Native.RoundToMaxAlignment(sizeof(NetPeerGroupMap.Entry*) * nextPrime);
			int num3 = Native.RoundToMaxAlignment(sizeof(NetPeerGroupMap.Entry) * nextPrime);
			byte* ptr = (byte*)Native.MallocAndClear(num + num2 + num3);
			NetPeerGroupMap* ptr2 = (NetPeerGroupMap*)ptr;
			ptr2->Buckets = (NetPeerGroupMap.Entry**)(ptr + num);
			ptr2->Entries = (NetPeerGroupMap.Entry*)(ptr + num + num2);
			ptr2->UsedCount = 0UL;
			ptr2->FreeCount = 0UL;
			ptr2->CapacityUsable = (ulong)((long)capacity);
			ptr2->CapacityAllocated = (ulong)((long)nextPrime);
			for (int i = 0; i < capacity; i++)
			{
				ptr2->Entries[i].Group = -1;
			}
			return ptr2;
		}

		public ulong Count
		{
			get
			{
				return this.UsedCount - this.FreeCount;
			}
		}

		public bool Full
		{
			get
			{
				return this.UsedCount == this.CapacityAllocated;
			}
		}

		public unsafe int Remove(NetAddress address)
		{
			ulong num = NetAddress.Hash64(address);
			ulong num2 = num % this.CapacityAllocated;
			NetPeerGroupMap.Entry* ptr = *(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetPeerGroupMap.Entry*)) / (ulong)sizeof(NetPeerGroupMap.Entry*));
			NetPeerGroupMap.Entry* ptr2 = default(NetPeerGroupMap.Entry*);
			while (ptr != null)
			{
				bool flag = ptr->Hash == num && ptr->Address.Block0 == address.Block0 && ptr->Address.Block1 == address.Block1 && ptr->Address.Block2 == address.Block2;
				if (flag)
				{
					bool flag2 = ptr2 == null;
					if (flag2)
					{
						*(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetPeerGroupMap.Entry*)) / (ulong)sizeof(NetPeerGroupMap.Entry*)) = ptr->Next;
					}
					else
					{
						ptr2->Next = ptr->Next;
					}
					Assert.Check(ptr->State == NetPeerGroupMap.EntryState.Used);
					short group = ptr->Group;
					*ptr = default(NetPeerGroupMap.Entry);
					ptr->Group = -1;
					ptr->Next = this.FreeHead;
					ptr->State = NetPeerGroupMap.EntryState.Free;
					this.FreeHead = ptr;
					this.FreeCount += 1UL;
					return (int)group;
				}
				ptr2 = ptr;
				ptr = ptr->Next;
			}
			return -1;
		}

		public unsafe bool Insert(NetAddress address, short group)
		{
			Assert.Check(this.Find(address) == -1);
			ulong num = NetAddress.Hash64(address);
			ulong num2 = num % this.CapacityAllocated;
			bool flag = this.FreeHead != null;
			NetPeerGroupMap.Entry* ptr;
			if (flag)
			{
				Assert.Check(this.FreeCount > 0UL);
				ptr = this.FreeHead;
				this.FreeHead = ptr->Next;
				this.FreeCount -= 1UL;
				Assert.Check(ptr->State == NetPeerGroupMap.EntryState.Free);
			}
			else
			{
				bool flag2 = this.UsedCount == this.CapacityUsable;
				if (flag2)
				{
					TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
					if (logTraceNetwork != null)
					{
						logTraceNetwork.Log("NetPeerGroupMap is full");
					}
					return false;
				}
				NetPeerGroupMap.Entry* entries = this.Entries;
				ulong usedCount = this.UsedCount;
				this.UsedCount = usedCount + 1UL;
				ptr = entries + usedCount * (ulong)((long)sizeof(NetPeerGroupMap.Entry)) / (ulong)sizeof(NetPeerGroupMap.Entry);
				Assert.Check(ptr->Group == -1);
				Assert.Check(ptr->State == NetPeerGroupMap.EntryState.None);
				Assert.Check(ptr->Next == null);
			}
			TraceLogStream logTraceNetwork2 = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork2 != null)
			{
				logTraceNetwork2.Log(string.Format("{0} mapped to group {1}", address.ToString(), group));
			}
			ptr->Hash = num;
			ptr->Group = group;
			ptr->Address = address;
			ptr->State = NetPeerGroupMap.EntryState.Used;
			ptr->Next = *(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetPeerGroupMap.Entry*)) / (ulong)sizeof(NetPeerGroupMap.Entry*));
			*(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetPeerGroupMap.Entry*)) / (ulong)sizeof(NetPeerGroupMap.Entry*)) = ptr;
			return true;
		}

		public unsafe short Find(NetAddress address)
		{
			ulong num = NetAddress.Hash64(address);
			ulong num2 = num % this.CapacityAllocated;
			for (NetPeerGroupMap.Entry* ptr = *(IntPtr*)(this.Buckets + num2 * (ulong)((long)sizeof(NetPeerGroupMap.Entry*)) / (ulong)sizeof(NetPeerGroupMap.Entry*)); ptr != null; ptr = ptr->Next)
			{
				bool flag = ptr->Hash == num && ptr->Address.Block0 == address.Block0 && ptr->Address.Block1 == address.Block1 && ptr->Address.Block2 == address.Block2;
				if (flag)
				{
					return ptr->Group;
				}
			}
			return -1;
		}

		public unsafe NetPeerGroupMap.Entry** Buckets;

		public unsafe NetPeerGroupMap.Entry* Entries;

		public unsafe NetPeerGroupMap.Entry* FreeHead;

		public ulong UsedCount;

		public ulong FreeCount;

		public ulong CapacityUsable;

		public ulong CapacityAllocated;

		public enum EntryState
		{
			None,
			Free,
			Used
		}

		public struct Entry
		{
			public unsafe NetPeerGroupMap.Entry* Next;

			public ulong Hash;

			public NetPeerGroupMap.EntryState State;

			public NetAddress Address;

			public short Group;
		}
	}
}
