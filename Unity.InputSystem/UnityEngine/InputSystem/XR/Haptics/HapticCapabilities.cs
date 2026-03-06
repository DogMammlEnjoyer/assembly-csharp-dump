using System;

namespace UnityEngine.InputSystem.XR.Haptics
{
	public struct HapticCapabilities
	{
		public HapticCapabilities(uint numChannels, bool supportsImpulse, bool supportsBuffer, uint frequencyHz, uint maxBufferSize, uint optimalBufferSize)
		{
			this.numChannels = numChannels;
			this.supportsImpulse = supportsImpulse;
			this.supportsBuffer = supportsBuffer;
			this.frequencyHz = frequencyHz;
			this.maxBufferSize = maxBufferSize;
			this.optimalBufferSize = optimalBufferSize;
		}

		public HapticCapabilities(uint numChannels, uint frequencyHz, uint maxBufferSize)
		{
			this = new HapticCapabilities(numChannels, false, false, frequencyHz, maxBufferSize, 0U);
		}

		public readonly uint numChannels { get; }

		public readonly bool supportsImpulse { get; }

		public readonly bool supportsBuffer { get; }

		public readonly uint frequencyHz { get; }

		public readonly uint maxBufferSize { get; }

		public readonly uint optimalBufferSize { get; }
	}
}
