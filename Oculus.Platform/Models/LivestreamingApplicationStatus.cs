using System;

namespace Oculus.Platform.Models
{
	public class LivestreamingApplicationStatus
	{
		public LivestreamingApplicationStatus(IntPtr o)
		{
			this.StreamingEnabled = CAPI.ovr_LivestreamingApplicationStatus_GetStreamingEnabled(o);
		}

		public readonly bool StreamingEnabled;
	}
}
