using System;

namespace Oculus.Platform.Models
{
	public class NetSyncSessionsChangedNotification
	{
		public NetSyncSessionsChangedNotification(IntPtr o)
		{
			this.ConnectionId = CAPI.ovr_NetSyncSessionsChangedNotification_GetConnectionId(o);
			this.Sessions = new NetSyncSessionList(CAPI.ovr_NetSyncSessionsChangedNotification_GetSessions(o));
		}

		public readonly long ConnectionId;

		public readonly NetSyncSessionList Sessions;
	}
}
