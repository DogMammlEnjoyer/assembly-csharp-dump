using System;

namespace Oculus.Platform.Models
{
	public class LaunchReportFlowResult
	{
		public LaunchReportFlowResult(IntPtr o)
		{
			this.DidCancel = CAPI.ovr_LaunchReportFlowResult_GetDidCancel(o);
			this.UserReportId = CAPI.ovr_LaunchReportFlowResult_GetUserReportId(o);
		}

		public readonly bool DidCancel;

		public readonly ulong UserReportId;
	}
}
