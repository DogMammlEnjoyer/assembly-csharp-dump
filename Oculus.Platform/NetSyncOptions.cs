using System;

namespace Oculus.Platform
{
	public class NetSyncOptions
	{
		public NetSyncOptions()
		{
			this.Handle = CAPI.ovr_NetSyncOptions_Create();
		}

		public void SetVoipGroup(string value)
		{
			CAPI.ovr_NetSyncOptions_SetVoipGroup(this.Handle, value);
		}

		public void SetVoipStreamDefault(NetSyncVoipStreamMode value)
		{
			CAPI.ovr_NetSyncOptions_SetVoipStreamDefault(this.Handle, value);
		}

		public void SetZoneId(string value)
		{
			CAPI.ovr_NetSyncOptions_SetZoneId(this.Handle, value);
		}

		public static explicit operator IntPtr(NetSyncOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~NetSyncOptions()
		{
			CAPI.ovr_NetSyncOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
