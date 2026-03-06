using System;

namespace Oculus.Platform.Models
{
	public class NetSyncSession
	{
		public NetSyncSession(IntPtr o)
		{
			this.ConnectionId = CAPI.ovr_NetSyncSession_GetConnectionId(o);
			this.Muted = CAPI.ovr_NetSyncSession_GetMuted(o);
			this.SessionId = CAPI.ovr_NetSyncSession_GetSessionId(o);
			this.UserId = CAPI.ovr_NetSyncSession_GetUserId(o);
			this.VoipGroup = CAPI.ovr_NetSyncSession_GetVoipGroup(o);
		}

		public readonly long ConnectionId;

		public readonly bool Muted;

		public readonly ulong SessionId;

		public readonly ulong UserId;

		public readonly string VoipGroup;
	}
}
