using System;
using System.Linq;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.HID
{
	public static class HIDSupport
	{
		public static ReadOnlyArray<HIDSupport.HIDPageUsage> supportedHIDUsages
		{
			get
			{
				return HIDSupport.s_SupportedHIDUsages;
			}
			set
			{
				HIDSupport.s_SupportedHIDUsages = value.ToArray();
				InputSystem.s_Manager.AddAvailableDevicesThatAreNowRecognized();
				for (int i = 0; i < InputSystem.devices.Count; i++)
				{
					InputDevice inputDevice = InputSystem.devices[i];
					HID hid = inputDevice as HID;
					if (hid != null && !HIDSupport.s_SupportedHIDUsages.Contains(new HIDSupport.HIDPageUsage(hid.hidDescriptor.usagePage, hid.hidDescriptor.usage)))
					{
						InputSystem.RemoveLayout(inputDevice.layout);
						i--;
					}
				}
			}
		}

		internal static void Initialize()
		{
			HIDSupport.s_SupportedHIDUsages = new HIDSupport.HIDPageUsage[]
			{
				new HIDSupport.HIDPageUsage(HID.GenericDesktop.Joystick),
				new HIDSupport.HIDPageUsage(HID.GenericDesktop.Gamepad),
				new HIDSupport.HIDPageUsage(HID.GenericDesktop.MultiAxisController)
			};
			InputSystem.RegisterLayout<HID>(null, null);
			InputSystem.onFindLayoutForDevice += HID.OnFindLayoutForDevice;
		}

		private static HIDSupport.HIDPageUsage[] s_SupportedHIDUsages;

		public struct HIDPageUsage
		{
			public HIDPageUsage(HID.UsagePage page, int usage)
			{
				this.page = page;
				this.usage = usage;
			}

			public HIDPageUsage(HID.GenericDesktop usage)
			{
				this.page = HID.UsagePage.GenericDesktop;
				this.usage = (int)usage;
			}

			public HID.UsagePage page;

			public int usage;
		}
	}
}
