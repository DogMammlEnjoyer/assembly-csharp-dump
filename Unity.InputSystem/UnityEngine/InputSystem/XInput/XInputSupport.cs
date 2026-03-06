using System;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.XInput
{
	internal static class XInputSupport
	{
		public static void Initialize()
		{
			InputSystem.RegisterLayout<XInputController>(null, null);
			InputSystem.RegisterLayout<XInputControllerWindows>(null, new InputDeviceMatcher?(default(InputDeviceMatcher).WithInterface("XInput", true)));
		}
	}
}
