using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 40)]
	internal struct DualShockHIDOutputReport : IInputDeviceCommandInfo
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
				return DualShockHIDOutputReport.Type;
			}
		}

		public void SetMotorSpeeds(float lowFreq, float highFreq)
		{
			this.flags |= 1;
			this.lowFrequencyMotorSpeed = (byte)Mathf.Clamp(lowFreq * 255f, 0f, 255f);
			this.highFrequencyMotorSpeed = (byte)Mathf.Clamp(highFreq * 255f, 0f, 255f);
		}

		public void SetColor(Color color)
		{
			this.flags |= 2;
			this.redColor = (byte)Mathf.Clamp(color.r * 255f, 0f, 255f);
			this.greenColor = (byte)Mathf.Clamp(color.g * 255f, 0f, 255f);
			this.blueColor = (byte)Mathf.Clamp(color.b * 255f, 0f, 255f);
		}

		public static DualShockHIDOutputReport Create(int outputReportSize)
		{
			return new DualShockHIDOutputReport
			{
				baseCommand = new InputDeviceCommand(DualShockHIDOutputReport.Type, 8 + outputReportSize),
				reportId = 5
			};
		}

		internal const int kSize = 40;

		internal const int kReportId = 5;

		[FieldOffset(0)]
		public InputDeviceCommand baseCommand;

		[FieldOffset(8)]
		public byte reportId;

		[FieldOffset(9)]
		public byte flags;

		[FixedBuffer(typeof(byte), 2)]
		[FieldOffset(10)]
		public DualShockHIDOutputReport.<unknown1>e__FixedBuffer unknown1;

		[FieldOffset(12)]
		public byte highFrequencyMotorSpeed;

		[FieldOffset(13)]
		public byte lowFrequencyMotorSpeed;

		[FieldOffset(14)]
		public byte redColor;

		[FieldOffset(15)]
		public byte greenColor;

		[FieldOffset(16)]
		public byte blueColor;

		[FixedBuffer(typeof(byte), 23)]
		[FieldOffset(17)]
		public DualShockHIDOutputReport.<unknown2>e__FixedBuffer unknown2;

		[Flags]
		public enum Flags
		{
			Rumble = 1,
			Color = 2
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 2)]
		public struct <unknown1>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 23)]
		public struct <unknown2>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
