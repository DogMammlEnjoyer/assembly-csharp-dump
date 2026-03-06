using System;

namespace Oculus.Platform.Models
{
	public class InvitePanelResultInfo
	{
		public InvitePanelResultInfo(IntPtr o)
		{
			this.InvitesSent = CAPI.ovr_InvitePanelResultInfo_GetInvitesSent(o);
		}

		public readonly bool InvitesSent;
	}
}
