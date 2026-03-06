using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.DualShock.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	internal struct DualShock3HIDInputReport : IInputStateTypeInfo
	{
		public FourCC format
		{
			get
			{
				return new FourCC('H', 'I', 'D', ' ');
			}
		}

		[FieldOffset(0)]
		private ushort padding1;

		[InputControl(name = "select", displayName = "Share", bit = 0U)]
		[InputControl(name = "leftStickPress", bit = 1U)]
		[InputControl(name = "rightStickPress", bit = 2U)]
		[InputControl(name = "start", displayName = "Options", bit = 3U)]
		[InputControl(name = "dpad", format = "BIT", layout = "Dpad", bit = 4U, sizeInBits = 4U)]
		[InputControl(name = "dpad/up", bit = 4U)]
		[InputControl(name = "dpad/right", bit = 5U)]
		[InputControl(name = "dpad/down", bit = 6U)]
		[InputControl(name = "dpad/left", bit = 7U)]
		[FieldOffset(2)]
		public byte buttons1;

		[InputControl(name = "leftTriggerButton", layout = "Button", bit = 0U, synthetic = true)]
		[InputControl(name = "rightTriggerButton", layout = "Button", bit = 1U, synthetic = true)]
		[InputControl(name = "leftShoulder", bit = 2U)]
		[InputControl(name = "rightShoulder", bit = 3U)]
		[InputControl(name = "buttonNorth", displayName = "Triangle", bit = 4U)]
		[InputControl(name = "buttonEast", displayName = "Circle", bit = 5U)]
		[InputControl(name = "buttonSouth", displayName = "Cross", bit = 6U)]
		[InputControl(name = "buttonWest", displayName = "Square", bit = 7U)]
		[FieldOffset(3)]
		public byte buttons2;

		[InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0U)]
		[InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1U)]
		[FieldOffset(4)]
		public byte buttons3;

		[FieldOffset(5)]
		private byte padding2;

		[InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "leftStick/x", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "leftStick/left", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/right", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "leftStick/y", offset = 1U, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "leftStick/up", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/down", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
		[FieldOffset(6)]
		public byte leftStickX;

		[FieldOffset(7)]
		public byte leftStickY;

		[InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "rightStick/x", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "rightStick/left", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/right", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "rightStick/y", offset = 1U, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
		[InputControl(name = "rightStick/up", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/down", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
		[FieldOffset(8)]
		public byte rightStickX;

		[FieldOffset(9)]
		public byte rightStickY;

		[FixedBuffer(typeof(byte), 8)]
		[FieldOffset(10)]
		private DualShock3HIDInputReport.<padding3>e__FixedBuffer padding3;

		[InputControl(name = "leftTrigger", format = "BYTE")]
		[FieldOffset(18)]
		public byte leftTrigger;

		[InputControl(name = "rightTrigger", format = "BYTE")]
		[FieldOffset(19)]
		public byte rightTrigger;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		public struct <padding3>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
