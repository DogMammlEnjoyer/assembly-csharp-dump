using System;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	[Serializable]
	internal struct Anchor : IEquatable<Anchor>
	{
		public Anchor(bool isAutomaticAnchor, bool isAlignmentAnchor, ulong ownerOculusId, uint colocationGroupId, Guid automaticAnchorUuid)
		{
			this.isAutomaticAnchor = isAutomaticAnchor;
			this.isAlignmentAnchor = isAlignmentAnchor;
			this.ownerOculusId = ownerOculusId;
			this.colocationGroupId = colocationGroupId;
			this.automaticAnchorUuid = automaticAnchorUuid;
		}

		public bool Equals(Anchor other)
		{
			return this.isAutomaticAnchor == other.isAutomaticAnchor && this.isAlignmentAnchor == other.isAlignmentAnchor && this.ownerOculusId == other.ownerOculusId && this.colocationGroupId == other.colocationGroupId && this.automaticAnchorUuid == other.automaticAnchorUuid;
		}

		public bool isAutomaticAnchor;

		public bool isAlignmentAnchor;

		public ulong ownerOculusId;

		public uint colocationGroupId;

		public Guid automaticAnchorUuid;
	}
}
