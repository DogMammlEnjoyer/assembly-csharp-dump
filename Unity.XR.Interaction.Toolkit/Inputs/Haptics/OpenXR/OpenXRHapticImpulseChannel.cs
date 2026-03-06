using System;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.OpenXR
{
	public class OpenXRHapticImpulseChannel : IXRHapticImpulseChannel
	{
		public InputAction hapticAction { get; set; }

		public InputDevice device { get; set; }

		public bool SendHapticImpulse(float amplitude, float duration, float frequency)
		{
			if (OpenXRInput.GetActionHandle(this.hapticAction, null) == 0UL)
			{
				return false;
			}
			OpenXRInput.SendHapticImpulse(this.hapticAction, amplitude, frequency, duration, this.device);
			return true;
		}
	}
}
