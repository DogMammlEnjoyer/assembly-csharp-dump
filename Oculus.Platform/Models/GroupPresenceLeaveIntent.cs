using System;

namespace Oculus.Platform.Models
{
	public class GroupPresenceLeaveIntent
	{
		public GroupPresenceLeaveIntent(IntPtr o)
		{
			this.DestinationApiName = CAPI.ovr_GroupPresenceLeaveIntent_GetDestinationApiName(o);
			this.LobbySessionId = CAPI.ovr_GroupPresenceLeaveIntent_GetLobbySessionId(o);
			this.MatchSessionId = CAPI.ovr_GroupPresenceLeaveIntent_GetMatchSessionId(o);
		}

		public readonly string DestinationApiName;

		public readonly string LobbySessionId;

		public readonly string MatchSessionId;
	}
}
