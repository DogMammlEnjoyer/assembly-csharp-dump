using System;

namespace Oculus.Platform
{
	public class VoipOptions
	{
		public VoipOptions()
		{
			this.Handle = CAPI.ovr_VoipOptions_Create();
		}

		public void SetBitrateForNewConnections(VoipBitrate value)
		{
			CAPI.ovr_VoipOptions_SetBitrateForNewConnections(this.Handle, value);
		}

		public void SetCreateNewConnectionUseDtx(VoipDtxState value)
		{
			CAPI.ovr_VoipOptions_SetCreateNewConnectionUseDtx(this.Handle, value);
		}

		public static explicit operator IntPtr(VoipOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~VoipOptions()
		{
			CAPI.ovr_VoipOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
