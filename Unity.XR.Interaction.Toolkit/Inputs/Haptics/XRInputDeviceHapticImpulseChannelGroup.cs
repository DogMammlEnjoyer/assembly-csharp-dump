using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	public class XRInputDeviceHapticImpulseChannelGroup : IXRHapticImpulseChannelGroup
	{
		public int channelCount
		{
			get
			{
				return this.m_Channels.Count;
			}
		}

		public IXRHapticImpulseChannel GetChannel(int channel = 0)
		{
			if (channel < 0)
			{
				Debug.LogError("Haptic channel can't be negative.");
				return null;
			}
			if (channel >= this.m_Channels.Count)
			{
				return null;
			}
			return this.m_Channels[channel];
		}

		public void Initialize(InputDevice device)
		{
			if (this.m_Device == device)
			{
				return;
			}
			this.m_Device = device;
			this.m_Channels.Clear();
			if (!device.isValid)
			{
				return;
			}
			HapticCapabilities hapticCapabilities;
			if (!device.TryGetHapticCapabilities(out hapticCapabilities))
			{
				Debug.LogWarning(string.Format("Failed to get haptic capabilities of {0}", device));
				return;
			}
			if (!hapticCapabilities.supportsImpulse)
			{
				Debug.LogWarning(string.Format("{0} does not support haptic impulse.", device));
				return;
			}
			int numChannels = (int)hapticCapabilities.numChannels;
			for (int i = 0; i < numChannels; i++)
			{
				this.m_Channels.Add(new XRInputDeviceHapticImpulseChannel
				{
					motorChannel = i,
					device = device
				});
			}
		}

		private InputDevice m_Device;

		private readonly List<IXRHapticImpulseChannel> m_Channels = new List<IXRHapticImpulseChannel>();
	}
}
