using System;

namespace Oculus.Platform.Models
{
	public class LivestreamingStartResult
	{
		public LivestreamingStartResult(IntPtr o)
		{
			this.StreamingResult = CAPI.ovr_LivestreamingStartResult_GetStreamingResult(o);
		}

		public readonly LivestreamingStartStatus StreamingResult;
	}
}
