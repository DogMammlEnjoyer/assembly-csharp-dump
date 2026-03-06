using System;

namespace Oculus.Platform.Models
{
	public class SendInvitesResult
	{
		public SendInvitesResult(IntPtr o)
		{
			this.Invites = new ApplicationInviteList(CAPI.ovr_SendInvitesResult_GetInvites(o));
		}

		public readonly ApplicationInviteList Invites;
	}
}
