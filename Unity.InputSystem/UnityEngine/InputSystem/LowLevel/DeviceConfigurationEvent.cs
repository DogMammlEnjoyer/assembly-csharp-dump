using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct DeviceConfigurationEvent : IInputEventTypeInfo
	{
		public FourCC typeStatic
		{
			get
			{
				return 1145259591;
			}
		}

		public unsafe InputEventPtr ToEventPtr()
		{
			fixed (DeviceConfigurationEvent* ptr = &this)
			{
				return new InputEventPtr((InputEvent*)ptr);
			}
		}

		public static DeviceConfigurationEvent Create(int deviceId, double time)
		{
			return new DeviceConfigurationEvent
			{
				baseEvent = new InputEvent(1145259591, 20, deviceId, time)
			};
		}

		public const int Type = 1145259591;

		[FieldOffset(0)]
		public InputEvent baseEvent;
	}
}
