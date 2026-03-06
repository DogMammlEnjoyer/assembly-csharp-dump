using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics
{
	public class HapticImpulseCommandChannel : IXRHapticImpulseChannel
	{
		public int motorChannel { get; set; }

		public InputDevice device { get; set; }

		public bool SendHapticImpulse(float amplitude, float duration, float frequency)
		{
			if (this.device == null)
			{
				return false;
			}
			SendHapticImpulseCommand sendHapticImpulseCommand = SendHapticImpulseCommand.Create(this.motorChannel, amplitude, duration);
			return this.device.ExecuteCommand<SendHapticImpulseCommand>(ref sendHapticImpulseCommand) >= 0L;
		}
	}
}
