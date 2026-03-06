using System;

namespace Oculus.Platform.Models
{
	public class MicrophoneAvailabilityState
	{
		public MicrophoneAvailabilityState(IntPtr o)
		{
			this.MicrophoneAvailable = CAPI.ovr_MicrophoneAvailabilityState_GetMicrophoneAvailable(o);
		}

		public readonly bool MicrophoneAvailable;
	}
}
