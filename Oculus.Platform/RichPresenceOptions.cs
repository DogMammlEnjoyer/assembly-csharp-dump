using System;

namespace Oculus.Platform
{
	public class RichPresenceOptions
	{
		public RichPresenceOptions()
		{
			this.Handle = CAPI.ovr_RichPresenceOptions_Create();
		}

		public static explicit operator IntPtr(RichPresenceOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~RichPresenceOptions()
		{
			CAPI.ovr_RichPresenceOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
