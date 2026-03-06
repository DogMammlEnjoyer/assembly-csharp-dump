using System;

namespace Oculus.Platform.Models
{
	public class AppDownloadProgressResult
	{
		public AppDownloadProgressResult(IntPtr o)
		{
			this.DownloadBytes = CAPI.ovr_AppDownloadProgressResult_GetDownloadBytes(o);
			this.DownloadedBytes = CAPI.ovr_AppDownloadProgressResult_GetDownloadedBytes(o);
			this.StatusCode = CAPI.ovr_AppDownloadProgressResult_GetStatusCode(o);
		}

		public readonly long DownloadBytes;

		public readonly long DownloadedBytes;

		public readonly AppStatus StatusCode;
	}
}
