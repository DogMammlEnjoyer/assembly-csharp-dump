using System;

namespace Oculus.Platform.Models
{
	public class TrialOffer
	{
		public TrialOffer(IntPtr o)
		{
			this.MaxTermCount = CAPI.ovr_TrialOffer_GetMaxTermCount(o);
			this.Price = new Price(CAPI.ovr_TrialOffer_GetPrice(o));
			this.TrialTerm = CAPI.ovr_TrialOffer_GetTrialTerm(o);
			this.TrialType = CAPI.ovr_TrialOffer_GetTrialType(o);
		}

		public readonly int MaxTermCount;

		public readonly Price Price;

		public readonly OfferTerm TrialTerm;

		public readonly OfferType TrialType;
	}
}
