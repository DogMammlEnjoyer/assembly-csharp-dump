using System;

namespace Oculus.Platform
{
	public class RosterOptions
	{
		public RosterOptions()
		{
			this.Handle = CAPI.ovr_RosterOptions_Create();
		}

		public void AddSuggestedUser(ulong userID)
		{
			CAPI.ovr_RosterOptions_AddSuggestedUser(this.Handle, userID);
		}

		public void ClearSuggestedUsers()
		{
			CAPI.ovr_RosterOptions_ClearSuggestedUsers(this.Handle);
		}

		public static explicit operator IntPtr(RosterOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~RosterOptions()
		{
			CAPI.ovr_RosterOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
