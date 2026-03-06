using System;
using System.Runtime.InteropServices;
using Fusion;

namespace Meta.XR.MultiplayerBlocks.Colocation.Fusion
{
	[NetworkStructWeaved(70)]
	[StructLayout(LayoutKind.Explicit, Size = 280)]
	internal struct FusionShareAndLocalizeParams : INetworkStruct
	{
		public FusionShareAndLocalizeParams(ShareAndLocalizeParams data)
		{
			this.requestingPlayerId = data.requestingPlayerId;
			this.requestingPlayerOculusId = data.requestingPlayerOculusId;
			this.anchorUUID = data.anchorUUID.ToString();
			this.anchorFlowSucceeded = data.anchorFlowSucceeded;
		}

		public ShareAndLocalizeParams GetShareAndLocalizeParams()
		{
			Guid guid;
			if (!Guid.TryParse(this.anchorUUID.ToString(), out guid))
			{
				Logger.Log("Failed to parse shared Anchor UUID string from network", LogLevel.Error);
			}
			return new ShareAndLocalizeParams(this.requestingPlayerId, this.requestingPlayerOculusId, guid, this.anchorFlowSucceeded);
		}

		[FieldOffset(0)]
		public ulong requestingPlayerId;

		[FieldOffset(8)]
		public ulong requestingPlayerOculusId;

		[FieldOffset(16)]
		public NetworkString<_64> anchorUUID;

		[FieldOffset(276)]
		public NetworkBool anchorFlowSucceeded;
	}
}
