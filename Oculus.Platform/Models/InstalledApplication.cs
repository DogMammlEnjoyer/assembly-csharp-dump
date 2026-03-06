using System;

namespace Oculus.Platform.Models
{
	public class InstalledApplication
	{
		public InstalledApplication(IntPtr o)
		{
			this.ApplicationId = CAPI.ovr_InstalledApplication_GetApplicationId(o);
			this.PackageName = CAPI.ovr_InstalledApplication_GetPackageName(o);
			this.Status = CAPI.ovr_InstalledApplication_GetStatus(o);
			this.VersionCode = CAPI.ovr_InstalledApplication_GetVersionCode(o);
			this.VersionName = CAPI.ovr_InstalledApplication_GetVersionName(o);
		}

		public readonly string ApplicationId;

		public readonly string PackageName;

		public readonly string Status;

		public readonly int VersionCode;

		public readonly string VersionName;
	}
}
