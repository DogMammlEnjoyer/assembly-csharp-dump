using System;

namespace Oculus.Platform
{
	public class ApplicationOptions
	{
		public ApplicationOptions()
		{
			this.Handle = CAPI.ovr_ApplicationOptions_Create();
		}

		public void SetDeeplinkMessage(string value)
		{
			CAPI.ovr_ApplicationOptions_SetDeeplinkMessage(this.Handle, value);
		}

		public void SetDestinationApiName(string value)
		{
			CAPI.ovr_ApplicationOptions_SetDestinationApiName(this.Handle, value);
		}

		public void SetLobbySessionId(string value)
		{
			CAPI.ovr_ApplicationOptions_SetLobbySessionId(this.Handle, value);
		}

		public void SetMatchSessionId(string value)
		{
			CAPI.ovr_ApplicationOptions_SetMatchSessionId(this.Handle, value);
		}

		public void SetRoomId(ulong value)
		{
			CAPI.ovr_ApplicationOptions_SetRoomId(this.Handle, value);
		}

		public static explicit operator IntPtr(ApplicationOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~ApplicationOptions()
		{
			CAPI.ovr_ApplicationOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
