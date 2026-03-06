using System;

namespace Oculus.Platform.Models
{
	public class PaidOffer
	{
		public PaidOffer(IntPtr o)
		{
			this.Price = new Price(CAPI.ovr_PaidOffer_GetPrice(o));
			this.SubscriptionTerm = CAPI.ovr_PaidOffer_GetSubscriptionTerm(o);
		}

		public readonly Price Price;

		public readonly OfferTerm SubscriptionTerm;
	}
}
