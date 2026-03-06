using System;

namespace Oculus.Platform
{
	public class MultiplayerErrorOptions
	{
		public MultiplayerErrorOptions()
		{
			this.Handle = CAPI.ovr_MultiplayerErrorOptions_Create();
		}

		public void SetErrorKey(MultiplayerErrorErrorKey value)
		{
			CAPI.ovr_MultiplayerErrorOptions_SetErrorKey(this.Handle, value);
		}

		public static explicit operator IntPtr(MultiplayerErrorOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~MultiplayerErrorOptions()
		{
			CAPI.ovr_MultiplayerErrorOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
