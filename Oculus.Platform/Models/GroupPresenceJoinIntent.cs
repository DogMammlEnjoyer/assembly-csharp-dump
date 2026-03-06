using System;

namespace Oculus.Platform.Models
{
	public class GroupPresenceJoinIntent
	{
		public GroupPresenceJoinIntent(IntPtr o)
		{
			this.DeeplinkMessage = CAPI.ovr_GroupPresenceJoinIntent_GetDeeplinkMessage(o);
			this.DestinationApiName = CAPI.ovr_GroupPresenceJoinIntent_GetDestinationApiName(o);
			this.LobbySessionId = CAPI.ovr_GroupPresenceJoinIntent_GetLobbySessionId(o);
			this.MatchSessionId = CAPI.ovr_GroupPresenceJoinIntent_GetMatchSessionId(o);
		}

		public readonly string DeeplinkMessage;

		public readonly string DestinationApiName;

		public readonly string LobbySessionId;

		public readonly string MatchSessionId;
	}
}
