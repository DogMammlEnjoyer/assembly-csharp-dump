using System;

namespace Oculus.Platform.Models
{
	public class LaunchUnblockFlowResult
	{
		public LaunchUnblockFlowResult(IntPtr o)
		{
			this.DidCancel = CAPI.ovr_LaunchUnblockFlowResult_GetDidCancel(o);
			this.DidUnblock = CAPI.ovr_LaunchUnblockFlowResult_GetDidUnblock(o);
		}

		public readonly bool DidCancel;

		public readonly bool DidUnblock;
	}
}
