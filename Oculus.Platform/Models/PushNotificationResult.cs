using System;

namespace Oculus.Platform.Models
{
	public class PushNotificationResult
	{
		public PushNotificationResult(IntPtr o)
		{
			this.Id = CAPI.ovr_PushNotificationResult_GetId(o);
		}

		public readonly string Id;
	}
}
