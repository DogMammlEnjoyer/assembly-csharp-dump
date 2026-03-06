using System;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	[Serializable]
	internal struct Player : IEquatable<Player>
	{
		public Player(ulong playerId, ulong oculusId, uint colocationGroupId)
		{
			this.playerId = playerId;
			this.oculusId = oculusId;
			this.colocationGroupId = colocationGroupId;
		}

		public bool Equals(Player other)
		{
			return this.playerId == other.playerId && this.oculusId == other.oculusId && this.colocationGroupId == other.colocationGroupId;
		}

		public ulong playerId;

		public ulong oculusId;

		public uint colocationGroupId;
	}
}
