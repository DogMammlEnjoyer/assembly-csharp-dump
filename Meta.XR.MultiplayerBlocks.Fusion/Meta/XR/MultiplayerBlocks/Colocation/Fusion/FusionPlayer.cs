using System;
using System.Runtime.InteropServices;
using Fusion;

namespace Meta.XR.MultiplayerBlocks.Colocation.Fusion
{
	[NetworkStructWeaved(5)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	internal struct FusionPlayer : INetworkStruct, IEquatable<FusionPlayer>
	{
		public FusionPlayer(Player player)
		{
			this.playerId = player.playerId;
			this.oculusId = player.oculusId;
			this.colocationGroupId = player.colocationGroupId;
		}

		public Player GetPlayer()
		{
			return new Player(this.playerId, this.oculusId, this.colocationGroupId);
		}

		public bool Equals(FusionPlayer other)
		{
			return this.GetPlayer().Equals(other.GetPlayer());
		}

		[FieldOffset(0)]
		public ulong playerId;

		[FieldOffset(8)]
		public ulong oculusId;

		[FieldOffset(16)]
		public uint colocationGroupId;
	}
}
