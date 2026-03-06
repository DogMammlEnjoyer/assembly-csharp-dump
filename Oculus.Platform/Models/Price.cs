using System;

namespace Oculus.Platform.Models
{
	public class Price
	{
		public Price(IntPtr o)
		{
			this.AmountInHundredths = CAPI.ovr_Price_GetAmountInHundredths(o);
			this.Currency = CAPI.ovr_Price_GetCurrency(o);
			this.Formatted = CAPI.ovr_Price_GetFormatted(o);
		}

		public readonly uint AmountInHundredths;

		public readonly string Currency;

		public readonly string Formatted;
	}
}
