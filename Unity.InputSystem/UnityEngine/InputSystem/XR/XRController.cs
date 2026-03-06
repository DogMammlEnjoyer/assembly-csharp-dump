using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.XR;

namespace UnityEngine.InputSystem.XR
{
	[InputControlLayout(commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, isGenericTypeOfDevice = true, displayName = "XR Controller")]
	public class XRController : TrackedDevice
	{
		public static XRController leftHand
		{
			get
			{
				return InputSystem.GetDevice<XRController>(CommonUsages.LeftHand);
			}
		}

		public static XRController rightHand
		{
			get
			{
				return InputSystem.GetDevice<XRController>(CommonUsages.RightHand);
			}
		}

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
