using System;

namespace Oculus.Platform.Models
{
	public class Product
	{
		public Product(IntPtr o)
		{
			IntPtr intPtr = CAPI.ovr_Product_GetBillingPlans(o);
			this.BillingPlans = new BillingPlanList(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.BillingPlansOptional = null;
			}
			else
			{
				this.BillingPlansOptional = this.BillingPlans;
			}
			IntPtr intPtr2 = CAPI.ovr_Product_GetContentRating(o);
			this.ContentRating = new ContentRating(intPtr2);
			if (intPtr2 == IntPtr.Zero)
			{
				this.ContentRatingOptional = null;
			}
			else
			{
				this.ContentRatingOptional = this.ContentRating;
			}
			this.CoverUrl = CAPI.ovr_Product_GetCoverUrl(o);
			this.Description = CAPI.ovr_Product_GetDescription(o);
			this.FormattedPrice = CAPI.ovr_Product_GetFormattedPrice(o);
			this.IconUrl = CAPI.ovr_Product_GetIconUrl(o);
			this.Name = CAPI.ovr_Product_GetName(o);
			this.Price = new Price(CAPI.ovr_Product_GetPrice(o));
			this.ShortDescription = CAPI.ovr_Product_GetShortDescription(o);
			this.Sku = CAPI.ovr_Product_GetSKU(o);
			this.Type = CAPI.ovr_Product_GetType(o);
		}

		public readonly BillingPlanList BillingPlansOptional;

		[Obsolete("Deprecated in favor of BillingPlansOptional")]
		public readonly BillingPlanList BillingPlans;

		public readonly ContentRating ContentRatingOptional;

		[Obsolete("Deprecated in favor of ContentRatingOptional")]
		public readonly ContentRating ContentRating;

		public readonly string CoverUrl;

		public readonly string Description;

		public readonly string FormattedPrice;

		public readonly string IconUrl;

		public readonly string Name;

		public readonly Price Price;

		public readonly string ShortDescription;

		public readonly string Sku;

		public readonly ProductType Type;
	}
}
