using System;

namespace Oculus.Platform.Models
{
	public class NetSyncSetSessionPropertyResult
	{
		public NetSyncSetSessionPropertyResult(IntPtr o)
		{
			this.Session = new NetSyncSession(CAPI.ovr_NetSyncSetSessionPropertyResult_GetSession(o));
		}

		public readonly NetSyncSession Session;
	}
}
