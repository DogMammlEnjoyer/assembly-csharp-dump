using System;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.Switch
{
	internal static class SwitchSupportHID
	{
		public static void Initialize()
		{
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 1406);
			InputSystem.RegisterLayout<SwitchProControllerHID>(name, new InputDeviceMatcher?(inputDeviceMatcher.WithCapability<int>("productId", 8201)));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3853);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 146));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3853);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 170));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3853);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 193));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3853);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 220));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3853);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 246));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3695);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 384));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3695);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 385));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3695);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 389));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3695);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 390));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3695);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 391));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 8406);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 42770));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 8406);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 42774));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3695);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 388));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 3695);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 392));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 8406);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 42772));
			inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("HID", true);
			inputDeviceMatcher = inputDeviceMatcher.WithCapability<int>("vendorId", 8406);
			InputSystem.RegisterLayoutMatcher<SwitchProControllerHID>(inputDeviceMatcher.WithCapability<int>("productId", 42773));
		}
	}
}
