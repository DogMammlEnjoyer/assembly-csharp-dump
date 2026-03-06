using System;

namespace Oculus.Platform
{
	public class InviteOptions
	{
		public InviteOptions()
		{
			this.Handle = CAPI.ovr_InviteOptions_Create();
		}

		public void AddSuggestedUser(ulong userID)
		{
			CAPI.ovr_InviteOptions_AddSuggestedUser(this.Handle, userID);
		}

		public void ClearSuggestedUsers()
		{
			CAPI.ovr_InviteOptions_ClearSuggestedUsers(this.Handle);
		}

		public static explicit operator IntPtr(InviteOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~InviteOptions()
		{
			CAPI.ovr_InviteOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
