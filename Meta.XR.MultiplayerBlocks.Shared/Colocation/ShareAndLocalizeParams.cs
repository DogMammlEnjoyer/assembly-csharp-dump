using System;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	[Serializable]
	internal struct ShareAndLocalizeParams
	{
		public ShareAndLocalizeParams(ulong requestingPlayerId, ulong requestingPlayerOculusId, Guid anchorUUID)
		{
			this.requestingPlayerId = requestingPlayerId;
			this.requestingPlayerOculusId = requestingPlayerOculusId;
			this.anchorUUID = anchorUUID;
			this.anchorFlowSucceeded = true;
		}

		public ShareAndLocalizeParams(ulong requestingPlayerId, ulong requestingPlayerOculusId, Guid anchorUUID, bool anchorFlowSucceeded)
		{
			this.requestingPlayerId = requestingPlayerId;
			this.requestingPlayerOculusId = requestingPlayerOculusId;
			this.anchorUUID = anchorUUID;
			this.anchorFlowSucceeded = anchorFlowSucceeded;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}: {3}, {4}: {5}, {6}: {7}", new object[]
			{
				"requestingPlayerId",
				this.requestingPlayerId,
				"requestingPlayerOculusId",
				this.requestingPlayerOculusId,
				"anchorUUID",
				this.anchorUUID,
				"anchorFlowSucceeded",
				this.anchorFlowSucceeded
			});
		}

		public ulong requestingPlayerId;

		public ulong requestingPlayerOculusId;

		public Guid anchorUUID;

		public bool anchorFlowSucceeded;
	}
}
