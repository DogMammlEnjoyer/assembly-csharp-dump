using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal class NetworkObjectConnectionData
	{
		[return: TupleElementNames(new string[]
		{
			"values",
			"changes"
		})]
		public ValueTuple<NetworkObjectHeader.PlayerUniqueData, NetworkObjectHeader.PlayerUniqueDataChanges> GetPlayerData()
		{
			return new ValueTuple<NetworkObjectHeader.PlayerUniqueData, NetworkObjectHeader.PlayerUniqueDataChanges>(this.UniqueData, this.UniqueDataChanges);
		}

		public void SetPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags flags, Simulation simulation)
		{
			this.UniqueData.Flags = (this.UniqueData.Flags | flags);
			this.UniqueDataChanges.Changes.FixedElementField = simulation.Tick.Raw;
			this.TickMin = simulation.Tick;
		}

		public void ClearPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags flags, Simulation simulation)
		{
			this.UniqueData.Flags = (this.UniqueData.Flags & ~flags);
			this.UniqueDataChanges.Changes.FixedElementField = simulation.Tick.Raw;
			this.TickMin = simulation.Tick;
		}

		public bool HasPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags flags)
		{
			return (this.UniqueData.Flags & flags) == flags;
		}

		public bool HasAnyPlayerDataFlag(NetworkObjectHeaderPlayerDataFlags flags)
		{
			return (this.UniqueData.Flags & flags) > (NetworkObjectHeaderPlayerDataFlags)0;
		}

		public NetworkObjectConnectionData Prev;

		public NetworkObjectConnectionData Next;

		public NetworkId Id;

		public NetworkObjectMeta MetaCache;

		public int PriorityLevel;

		public NetworkObjectConnectionDataStatus Status;

		public bool MainTRSP;

		public Tick TickSent;

		public Tick TickAcknowledged;

		public Tick TickMin;

		public ulong Filter = ulong.MaxValue;

		public NetworkObjectHeader.PlayerUniqueData UniqueData;

		public NetworkObjectHeader.PlayerUniqueDataChanges UniqueDataChanges;
	}
}
