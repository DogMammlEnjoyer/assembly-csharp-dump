using System;

namespace Oculus.Platform.Models
{
	public class ShareMediaResult
	{
		public ShareMediaResult(IntPtr o)
		{
			this.Status = CAPI.ovr_ShareMediaResult_GetStatus(o);
		}

		public readonly ShareMediaStatus Status;
	}
}
