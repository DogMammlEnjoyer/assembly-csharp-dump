using System;

namespace Oculus.Platform.Models
{
	public class ContentRating
	{
		public ContentRating(IntPtr o)
		{
			this.AgeRatingImageUri = CAPI.ovr_ContentRating_GetAgeRatingImageUri(o);
			this.AgeRatingText = CAPI.ovr_ContentRating_GetAgeRatingText(o);
			uint num = CAPI.ovr_ContentRating_GetDescriptorsSize(o);
			this.Descriptors = new string[num];
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				this.Descriptors[(int)num2] = CAPI.ovr_ContentRating_GetDescriptor(o, num2);
			}
			uint num3 = CAPI.ovr_ContentRating_GetInteractiveElementsSize(o);
			this.InteractiveElements = new string[num3];
			for (uint num4 = 0U; num4 < num3; num4 += 1U)
			{
				this.InteractiveElements[(int)num4] = CAPI.ovr_ContentRating_GetInteractiveElement(o, num4);
			}
			this.RatingDefinitionUri = CAPI.ovr_ContentRating_GetRatingDefinitionUri(o);
		}

		public readonly string AgeRatingImageUri;

		public readonly string AgeRatingText;

		public readonly string[] Descriptors;

		public readonly string[] InteractiveElements;

		public readonly string RatingDefinitionUri;
	}
}
