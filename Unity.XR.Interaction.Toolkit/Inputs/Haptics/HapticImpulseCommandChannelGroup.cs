using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	public class HapticImpulseCommandChannelGroup : IXRHapticImpulseChannelGroup
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
			if (device == null)
			{
				return;
			}
			GetHapticCapabilitiesCommand getHapticCapabilitiesCommand = GetHapticCapabilitiesCommand.Create();
			long num = device.ExecuteCommand<GetHapticCapabilitiesCommand>(ref getHapticCapabilitiesCommand);
			int num2;
			if (num < 0L)
			{
				Debug.LogWarning(string.Format("Failed to get haptic capabilities of {0}, error code {1}. Continuing assuming a single haptic channel.", device, num));
				num2 = 1;
			}
			else
			{
				num2 = (int)getHapticCapabilitiesCommand.numChannels;
			}
			for (int i = 0; i < num2; i++)
			{
				this.m_Channels.Add(new HapticImpulseCommandChannel
				{
					motorChannel = i,
					device = device
				});
			}
		}

		private readonly List<IXRHapticImpulseChannel> m_Channels = new List<IXRHapticImpulseChannel>();

		private InputDevice m_Device;
	}
}
