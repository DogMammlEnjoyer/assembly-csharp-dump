using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	public class HapticImpulseSingleChannelGroup : IXRHapticImpulseChannelGroup
	{
		public int channelCount
		{
			get
			{
				return 1;
			}
		}

		public IXRHapticImpulseChannel impulseChannel { get; }

		public HapticImpulseSingleChannelGroup(IXRHapticImpulseChannel channel)
		{
			this.impulseChannel = channel;
		}

		public IXRHapticImpulseChannel GetChannel(int channel = 0)
		{
			if (channel < 0)
			{
				Debug.LogError("Haptic channel can't be negative.");
				return null;
			}
			return this.impulseChannel;
		}
	}
}
