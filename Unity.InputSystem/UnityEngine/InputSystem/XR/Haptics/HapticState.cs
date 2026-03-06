using System;

namespace UnityEngine.InputSystem.XR.Haptics
{
	public struct HapticState
	{
		public HapticState(uint samplesQueued, uint samplesAvailable)
		{
			this.samplesQueued = samplesQueued;
			this.samplesAvailable = samplesAvailable;
		}

		public uint samplesQueued { readonly get; private set; }

		public uint samplesAvailable { readonly get; private set; }
	}
}
