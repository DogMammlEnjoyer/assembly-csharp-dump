using System;

namespace Oculus.Platform.Models
{
	public class UserCapability
	{
		public UserCapability(IntPtr o)
		{
			this.Description = CAPI.ovr_UserCapability_GetDescription(o);
			this.IsEnabled = CAPI.ovr_UserCapability_GetIsEnabled(o);
			this.Name = CAPI.ovr_UserCapability_GetName(o);
			this.ReasonCode = CAPI.ovr_UserCapability_GetReasonCode(o);
		}

		public readonly string Description;

		public readonly bool IsEnabled;

		public readonly string Name;

		public readonly string ReasonCode;
	}
}
