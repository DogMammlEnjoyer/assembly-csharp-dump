using System;

namespace Oculus.Platform
{
	public class GroupPresenceOptions
	{
		public GroupPresenceOptions()
		{
			this.Handle = CAPI.ovr_GroupPresenceOptions_Create();
		}

		public void SetDeeplinkMessageOverride(string value)
		{
			CAPI.ovr_GroupPresenceOptions_SetDeeplinkMessageOverride(this.Handle, value);
		}

		public void SetDestinationApiName(string value)
		{
			CAPI.ovr_GroupPresenceOptions_SetDestinationApiName(this.Handle, value);
		}

		public void SetIsJoinable(bool value)
		{
			CAPI.ovr_GroupPresenceOptions_SetIsJoinable(this.Handle, value);
		}

		public void SetLobbySessionId(string value)
		{
			CAPI.ovr_GroupPresenceOptions_SetLobbySessionId(this.Handle, value);
		}

		public void SetMatchSessionId(string value)
		{
			CAPI.ovr_GroupPresenceOptions_SetMatchSessionId(this.Handle, value);
		}

		public static explicit operator IntPtr(GroupPresenceOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~GroupPresenceOptions()
		{
			CAPI.ovr_GroupPresenceOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
