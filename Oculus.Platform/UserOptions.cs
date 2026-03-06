using System;

namespace Oculus.Platform
{
	public class UserOptions
	{
		public UserOptions()
		{
			this.Handle = CAPI.ovr_UserOptions_Create();
		}

		public void SetMaxUsers(uint value)
		{
			CAPI.ovr_UserOptions_SetMaxUsers(this.Handle, value);
		}

		public void AddServiceProvider(ServiceProvider value)
		{
			CAPI.ovr_UserOptions_AddServiceProvider(this.Handle, value);
		}

		public void ClearServiceProviders()
		{
			CAPI.ovr_UserOptions_ClearServiceProviders(this.Handle);
		}

		public void SetTimeWindow(TimeWindow value)
		{
			CAPI.ovr_UserOptions_SetTimeWindow(this.Handle, value);
		}

		public static explicit operator IntPtr(UserOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~UserOptions()
		{
			CAPI.ovr_UserOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
