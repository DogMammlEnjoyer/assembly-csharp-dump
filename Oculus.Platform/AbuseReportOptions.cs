using System;

namespace Oculus.Platform
{
	public class AbuseReportOptions
	{
		public AbuseReportOptions()
		{
			this.Handle = CAPI.ovr_AbuseReportOptions_Create();
		}

		public void SetPreventPeopleChooser(bool value)
		{
			CAPI.ovr_AbuseReportOptions_SetPreventPeopleChooser(this.Handle, value);
		}

		public void SetReportType(AbuseReportType value)
		{
			CAPI.ovr_AbuseReportOptions_SetReportType(this.Handle, value);
		}

		public static explicit operator IntPtr(AbuseReportOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~AbuseReportOptions()
		{
			CAPI.ovr_AbuseReportOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
