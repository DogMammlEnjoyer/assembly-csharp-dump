using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.OpenXR.Input
{
	[Preserve]
	[InputControlLayout(displayName = "OpenXR Action Map")]
	public abstract class OpenXRDevice : InputDevice
	{
		protected override void FinishSetup()
		{
			base.FinishSetup();
			XRDeviceDescriptor xrdeviceDescriptor = XRDeviceDescriptor.FromJson(base.description.capabilities);
			if (xrdeviceDescriptor != null)
			{
				if ((xrdeviceDescriptor.characteristics & InputDeviceCharacteristics.Left) != InputDeviceCharacteristics.None)
				{
					InputSystem.SetDeviceUsage(this, CommonUsages.LeftHand);
					return;
				}
				if ((xrdeviceDescriptor.characteristics & InputDeviceCharacteristics.Right) != InputDeviceCharacteristics.None)
				{
					InputSystem.SetDeviceUsage(this, CommonUsages.RightHand);
				}
			}
		}
	}
}
