using System;

namespace Oculus.Platform.Models
{
	public class UserReportID
	{
		public UserReportID(IntPtr o)
		{
			this.DidCancel = CAPI.ovr_UserReportID_GetDidCancel(o);
			this.ID = CAPI.ovr_UserReportID_GetID(o);
		}

		public readonly bool DidCancel;

		public readonly ulong ID;
	}
}
