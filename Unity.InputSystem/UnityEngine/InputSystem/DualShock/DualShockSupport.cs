using System;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.DualShock
{
	internal static class DualShockSupport
	{
		public static void Initialize()
		{
			InputSystem.RegisterLayout<DualShockGamepad>(null, null);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 1356);
			InputSystem.RegisterLayout<DualSenseGamepadHID>(name, new InputDeviceMatcher?(inputDeviceMatcher.WithCapability<int>("productId", 3570)));
			string name2 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 1356);
			InputSystem.RegisterLayout<DualSenseGamepadHID>(name2, new InputDeviceMatcher?(inputDeviceMatcher.WithCapability<int>("productId", 3302)));
			string name3 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 1356);
			InputSystem.RegisterLayout<DualShock4GamepadHID>(name3, new InputDeviceMatcher?(inputDeviceMatcher.WithCapability<int>("productId", 2508)));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 1356);
			InputSystem.RegisterLayoutMatcher<DualShock4GamepadHID>(inputDeviceMatcher.WithCapability<int>("productId", 1476));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturerContains("Sony");
			InputSystem.RegisterLayoutMatcher<DualShock4GamepadHID>(inputDeviceMatcher.WithProduct("Wireless Controller", true));
			string name4 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 1356);
			InputSystem.RegisterLayout<DualShock3GamepadHID>(name4, new InputDeviceMatcher?(inputDeviceMatcher.WithCapability<int>("productId", 616)));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithManufacturerContains("Sony");
			InputSystem.RegisterLayoutMatcher<DualShock3GamepadHID>(inputDeviceMatcher.WithProduct("PLAYSTATION(R)3 Controller", false));
		}
	}
}
