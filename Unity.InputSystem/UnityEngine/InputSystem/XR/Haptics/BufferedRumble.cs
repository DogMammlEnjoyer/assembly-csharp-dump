using System;

namespace UnityEngine.InputSystem.XR.Haptics
{
	public struct BufferedRumble
	{
		public HapticCapabilities capabilities { readonly get; private set; }

		private InputDevice device { readonly get; set; }

		public BufferedRumble(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			this.device = device;
			GetHapticCapabilitiesCommand getHapticCapabilitiesCommand = GetHapticCapabilitiesCommand.Create();
			device.ExecuteCommand<GetHapticCapabilitiesCommand>(ref getHapticCapabilitiesCommand);
			this.capabilities = getHapticCapabilitiesCommand.capabilities;
		}

		public void EnqueueRumble(byte[] samples)
		{
			SendBufferedHapticCommand sendBufferedHapticCommand = SendBufferedHapticCommand.Create(samples);
			this.device.ExecuteCommand<SendBufferedHapticCommand>(ref sendBufferedHapticCommand);
		}
	}
}
