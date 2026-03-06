using System;

namespace Oculus.Platform.Models
{
	public class Purchase
	{
		public Purchase(IntPtr o)
		{
			this.DeveloperPayload = CAPI.ovr_Purchase_GetDeveloperPayload(o);
			this.ExpirationTime = CAPI.ovr_Purchase_GetExpirationTime(o);
			this.GrantTime = CAPI.ovr_Purchase_GetGrantTime(o);
			this.ID = CAPI.ovr_Purchase_GetPurchaseStrID(o);
			this.ReportingId = CAPI.ovr_Purchase_GetReportingId(o);
			this.Sku = CAPI.ovr_Purchase_GetSKU(o);
			this.Type = CAPI.ovr_Purchase_GetType(o);
		}

		public readonly string DeveloperPayload;

		public readonly DateTime ExpirationTime;

		public readonly DateTime GrantTime;

		public readonly string ID;

		public readonly string ReportingId;

		public readonly string Sku;

		public readonly ProductType Type;
	}
}
