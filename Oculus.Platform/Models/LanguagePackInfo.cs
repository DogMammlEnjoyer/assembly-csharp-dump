using System;

namespace Oculus.Platform.Models
{
	public class LanguagePackInfo
	{
		public LanguagePackInfo(IntPtr o)
		{
			this.EnglishName = CAPI.ovr_LanguagePackInfo_GetEnglishName(o);
			this.NativeName = CAPI.ovr_LanguagePackInfo_GetNativeName(o);
			this.Tag = CAPI.ovr_LanguagePackInfo_GetTag(o);
		}

		public readonly string EnglishName;

		public readonly string NativeName;

		public readonly string Tag;
	}
}
