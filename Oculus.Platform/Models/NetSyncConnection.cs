using System;

namespace Oculus.Platform.Models
{
	public class NetSyncConnection
	{
		public NetSyncConnection(IntPtr o)
		{
			this.ConnectionId = CAPI.ovr_NetSyncConnection_GetConnectionId(o);
			this.DisconnectReason = CAPI.ovr_NetSyncConnection_GetDisconnectReason(o);
			this.SessionId = CAPI.ovr_NetSyncConnection_GetSessionId(o);
			this.Status = CAPI.ovr_NetSyncConnection_GetStatus(o);
			this.ZoneId = CAPI.ovr_NetSyncConnection_GetZoneId(o);
		}

		public readonly long ConnectionId;

		public readonly NetSyncDisconnectReason DisconnectReason;

		public readonly ulong SessionId;

		public readonly NetSyncConnectionStatus Status;

		public readonly string ZoneId;
	}
}
