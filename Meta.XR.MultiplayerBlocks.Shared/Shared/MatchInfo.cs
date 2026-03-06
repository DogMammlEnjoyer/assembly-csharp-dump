using System;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	[Serializable]
	internal struct MatchInfo
	{
		public MatchInfo(string roomId, string roomPassword, string extra = "")
		{
			this.RoomId = roomId;
			this.RoomPassword = roomPassword;
			this.Extra = extra;
		}

		internal string RoomId;

		internal string RoomPassword;

		internal string Extra;
	}
}
