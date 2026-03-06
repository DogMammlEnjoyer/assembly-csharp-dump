using System;

namespace Oculus.Platform.Models
{
	public class LaunchInvitePanelFlowResult
	{
		public LaunchInvitePanelFlowResult(IntPtr o)
		{
			this.InvitedUsers = new UserList(CAPI.ovr_LaunchInvitePanelFlowResult_GetInvitedUsers(o));
		}

		public readonly UserList InvitedUsers;
	}
}
