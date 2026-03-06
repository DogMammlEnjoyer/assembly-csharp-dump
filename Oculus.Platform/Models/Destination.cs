using System;

namespace Oculus.Platform.Models
{
	public class Destination
	{
		public Destination(IntPtr o)
		{
			this.ApiName = CAPI.ovr_Destination_GetApiName(o);
			this.DeeplinkMessage = CAPI.ovr_Destination_GetDeeplinkMessage(o);
			this.DisplayName = CAPI.ovr_Destination_GetDisplayName(o);
			this.ShareableUri = CAPI.ovr_Destination_GetShareableUri(o);
		}

		public readonly string ApiName;

		public readonly string DeeplinkMessage;

		public readonly string DisplayName;

		public readonly string ShareableUri;
	}
}
