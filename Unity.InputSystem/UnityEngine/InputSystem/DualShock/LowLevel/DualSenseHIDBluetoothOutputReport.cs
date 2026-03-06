using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 86)]
	internal struct DualSenseHIDBluetoothOutputReport : IInputDeviceCommandInfo
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
				return DualSenseHIDBluetoothOutputReport.Type;
			}
		}

		public static DualSenseHIDBluetoothOutputReport Create(DualSenseHIDOutputReportPayload payload, byte outputSequenceId, int outputReportSize)
		{
			return new DualSenseHIDBluetoothOutputReport
			{
				baseCommand = new InputDeviceCommand(DualSenseHIDBluetoothOutputReport.Type, 8 + outputReportSize),
				reportId = 49,
				tag1 = (byte)((outputSequenceId & 15) << 4),
				tag2 = 16,
				payload = payload
			};
		}

		internal const int kSize = 86;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public byte reportId;

		[FieldOffset(9)]
		public byte tag1;

		[FieldOffset(10)]
		public byte tag2;

		[FieldOffset(11)]
		public DualSenseHIDOutputReportPayload payload;

		[FieldOffset(82)]
		public uint crc32;

		[FixedBuffer(typeof(byte), 74)]
		[FieldOffset(8)]
		public DualSenseHIDBluetoothOutputReport.<rawData>e__FixedBuffer rawData;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 74)]
		public struct <rawData>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
