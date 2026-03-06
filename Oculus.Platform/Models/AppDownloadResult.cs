using System;

namespace Oculus.Platform.Models
{
	public class AppDownloadResult
	{
		public AppDownloadResult(IntPtr o)
		{
			this.AppInstallResult = CAPI.ovr_AppDownloadResult_GetAppInstallResult(o);
			this.Timestamp = CAPI.ovr_AppDownloadResult_GetTimestamp(o);
		}

		public readonly AppInstallResult AppInstallResult;

		public readonly long Timestamp;
	}
}
