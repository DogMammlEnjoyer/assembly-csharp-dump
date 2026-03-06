using System;

namespace Oculus.Platform
{
	public class AdvancedAbuseReportOptions
	{
		public AdvancedAbuseReportOptions()
		{
			this.Handle = CAPI.ovr_AdvancedAbuseReportOptions_Create();
		}

		public void SetDeveloperDefinedContext(string key, string value)
		{
			CAPI.ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString(this.Handle, key, value);
		}

		public void ClearDeveloperDefinedContext()
		{
			CAPI.ovr_AdvancedAbuseReportOptions_ClearDeveloperDefinedContext(this.Handle);
		}

		public void SetObjectType(string value)
		{
			CAPI.ovr_AdvancedAbuseReportOptions_SetObjectType(this.Handle, value);
		}

		public void SetReportType(AbuseReportType value)
		{
			CAPI.ovr_AdvancedAbuseReportOptions_SetReportType(this.Handle, value);
		}

		public void AddSuggestedUser(ulong userID)
		{
			CAPI.ovr_AdvancedAbuseReportOptions_AddSuggestedUser(this.Handle, userID);
		}

		public void ClearSuggestedUsers()
		{
			CAPI.ovr_AdvancedAbuseReportOptions_ClearSuggestedUsers(this.Handle);
		}

		public void SetVideoMode(AbuseReportVideoMode value)
		{
			CAPI.ovr_AdvancedAbuseReportOptions_SetVideoMode(this.Handle, value);
		}

		public static explicit operator IntPtr(AdvancedAbuseReportOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~AdvancedAbuseReportOptions()
		{
			CAPI.ovr_AdvancedAbuseReportOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
