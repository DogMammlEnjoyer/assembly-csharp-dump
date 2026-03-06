using System;

namespace Oculus.Platform.Models
{
	public class UserAccountAgeCategory
	{
		public UserAccountAgeCategory(IntPtr o)
		{
			this.AgeCategory = CAPI.ovr_UserAccountAgeCategory_GetAgeCategory(o);
		}

		public readonly AccountAgeCategory AgeCategory;
	}
}
