using System;

namespace Oculus.Platform.Models
{
	public class BillingPlan
	{
		public BillingPlan(IntPtr o)
		{
			this.PaidOffer = new PaidOffer(CAPI.ovr_BillingPlan_GetPaidOffer(o));
			IntPtr intPtr = CAPI.ovr_BillingPlan_GetTrialOffers(o);
			this.TrialOffers = new TrialOfferList(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.TrialOffersOptional = null;
				return;
			}
			this.TrialOffersOptional = this.TrialOffers;
		}

		public readonly PaidOffer PaidOffer;

		public readonly TrialOfferList TrialOffersOptional;

		[Obsolete("Deprecated in favor of TrialOffersOptional")]
		public readonly TrialOfferList TrialOffers;
	}
}
