using System;
using System.Runtime.InteropServices;
using Fusion;

namespace Meta.XR.MultiplayerBlocks.Colocation.Fusion
{
	[NetworkStructWeaved(70)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 280)]
	internal struct FusionAnchor : INetworkStruct, IEquatable<FusionAnchor>
	{
		public FusionAnchor(Anchor anchor)
		{
			this.isAutomaticAnchor = anchor.isAutomaticAnchor;
			this.isAlignmentAnchor = anchor.isAlignmentAnchor;
			this.ownerOculusId = anchor.ownerOculusId;
			this.colocationGroupId = anchor.colocationGroupId;
			this.automaticAnchorUuid = anchor.automaticAnchorUuid.ToString();
		}

		public Anchor GetAnchor()
		{
			Guid guid;
			if (!Guid.TryParse(this.automaticAnchorUuid.ToString(), out guid))
			{
				Logger.Log("Failed to parse Anchor UUID string", LogLevel.Error);
			}
			return new Anchor(this.isAutomaticAnchor, this.isAlignmentAnchor, this.ownerOculusId, this.colocationGroupId, guid);
		}

		public bool Equals(FusionAnchor other)
		{
			return this.GetAnchor().Equals(other.GetAnchor());
		}

		[FieldOffset(0)]
		public NetworkBool isAutomaticAnchor;

		[FieldOffset(4)]
		public NetworkBool isAlignmentAnchor;

		[FieldOffset(8)]
		public ulong ownerOculusId;

		[FieldOffset(16)]
		public uint colocationGroupId;

		[FieldOffset(20)]
		public NetworkString<_64> automaticAnchorUuid;
	}
}
