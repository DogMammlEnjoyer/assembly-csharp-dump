using System;
using UnityEngine.InputSystem.XR.Haptics;

namespace UnityEngine.InputSystem.XR
{
	public class XRControllerWithRumble : XRController
	{
		public void SendImpulse(float amplitude, float duration)
		{
			SendHapticImpulseCommand sendHapticImpulseCommand = SendHapticImpulseCommand.Create(0, amplitude, duration);
			base.ExecuteCommand<SendHapticImpulseCommand>(ref sendHapticImpulseCommand);
		}
	}
}
