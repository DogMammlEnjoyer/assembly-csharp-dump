using System;

namespace Oculus.Platform.Models
{
	public class LaunchFriendRequestFlowResult
	{
		public LaunchFriendRequestFlowResult(IntPtr o)
		{
			this.DidCancel = CAPI.ovr_LaunchFriendRequestFlowResult_GetDidCancel(o);
			this.DidSendRequest = CAPI.ovr_LaunchFriendRequestFlowResult_GetDidSendRequest(o);
		}

		public readonly bool DidCancel;

		public readonly bool DidSendRequest;
	}
}
