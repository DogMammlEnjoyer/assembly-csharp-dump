using System;

namespace Oculus.Platform.Models
{
	public class SystemVoipState
	{
		public SystemVoipState(IntPtr o)
		{
			this.MicrophoneMuted = CAPI.ovr_SystemVoipState_GetMicrophoneMuted(o);
			this.Status = CAPI.ovr_SystemVoipState_GetStatus(o);
		}

		public readonly VoipMuteState MicrophoneMuted;

		public readonly SystemVoipStatus Status;
	}
}
