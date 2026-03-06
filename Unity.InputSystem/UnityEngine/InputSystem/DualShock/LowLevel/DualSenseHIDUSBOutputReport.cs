using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 56)]
	internal struct DualSenseHIDUSBOutputReport : IInputDeviceCommandInfo
	{
		public static FourCC Type
		{
			get
			{
				return new FourCC('H', 'I', 'D', 'O');
			}
		}

		public FourCC typeStatic
		{
			get
			{
				return DualSenseHIDUSBOutputReport.Type;
			}
		}

		public static DualSenseHIDUSBOutputReport Create(DualSenseHIDOutputReportPayload payload, int outputReportSize)
		{
			return new DualSenseHIDUSBOutputReport
			{
				baseCommand = new InputDeviceCommand(DualSenseHIDUSBOutputReport.Type, 8 + outputReportSize),
				reportId = 2,
				payload = payload
			};
		}

		internal const int kSize = 56;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public byte reportId;

		[FieldOffset(9)]
		public DualSenseHIDOutputReportPayload payload;
	}
}
