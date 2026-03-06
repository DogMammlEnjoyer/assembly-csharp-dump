using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct DeviceRemoveEvent : IInputEventTypeInfo
	{
		public FourCC typeStatic
		{
			get
			{
				return 1146242381;
			}
		}

		public unsafe InputEventPtr ToEventPtr()
		{
			fixed (DeviceRemoveEvent* ptr = &this)
			{
				return new InputEventPtr((InputEvent*)ptr);
			}
		}

		public static DeviceRemoveEvent Create(int deviceId, double time = -1.0)
		{
			return new DeviceRemoveEvent
			{
				baseEvent = new InputEvent(1146242381, 20, deviceId, time)
			};
		}

		public const int Type = 1146242381;

		[FieldOffset(0)]
		public InputEvent baseEvent;
	}
}
