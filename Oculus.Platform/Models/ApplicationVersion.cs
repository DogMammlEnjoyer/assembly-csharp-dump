using System;

namespace Oculus.Platform.Models
{
	public class ApplicationVersion
	{
		public ApplicationVersion(IntPtr o)
		{
			this.CurrentCode = CAPI.ovr_ApplicationVersion_GetCurrentCode(o);
			this.CurrentName = CAPI.ovr_ApplicationVersion_GetCurrentName(o);
			this.LatestCode = CAPI.ovr_ApplicationVersion_GetLatestCode(o);
			this.LatestName = CAPI.ovr_ApplicationVersion_GetLatestName(o);
			this.ReleaseDate = CAPI.ovr_ApplicationVersion_GetReleaseDate(o);
			this.Size = CAPI.ovr_ApplicationVersion_GetSize(o);
		}

		public readonly int CurrentCode;

		public readonly string CurrentName;

		public readonly int LatestCode;

		public readonly string LatestName;

		public readonly long ReleaseDate;

		public readonly string Size;
	}
}
