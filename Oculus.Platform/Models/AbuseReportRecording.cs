using System;

namespace Oculus.Platform.Models
{
	public class AbuseReportRecording
	{
		public AbuseReportRecording(IntPtr o)
		{
			this.RecordingUuid = CAPI.ovr_AbuseReportRecording_GetRecordingUuid(o);
		}

		public readonly string RecordingUuid;
	}
}
