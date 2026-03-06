using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.XInput.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	internal struct XInputControllerWindowsState : IInputStateTypeInfo
	{
		public FourCC format
		{
			get
			{
				return new FourCC('X', 'I', 'N', 'P');
			}
		}

		public XInputControllerWindowsState WithButton(XInputControllerWindowsState.Button button)
		{
			this.buttons |= (ushort)(1 << (int)button);
			return this;
		}

		[InputControl(name = "dpad", layout = "Dpad", sizeInBits = 4U, bit = 0U)]
		[InputControl(name = "dpad/up", bit = 0U)]
		[InputControl(name = "dpad/down", bit = 1U)]
		[InputControl(name = "dpad/left", bit = 2U)]
		[InputControl(name = "dpad/right", bit = 3U)]
		[InputControl(name = "start", bit = 4U, displayName = "Start")]
		[InputControl(name = "select", bit = 5U, displayName = "Select")]
		[InputControl(name = "leftStickPress", bit = 6U)]
		[InputControl(name = "rightStickPress", bit = 7U)]
		[InputControl(name = "leftShoulder", bit = 8U)]
		[InputControl(name = "rightShoulder", bit = 9U)]
		[InputControl(name = "buttonSouth", bit = 12U, displayName = "A")]
		[InputControl(name = "buttonEast", bit = 13U, displayName = "B")]
		[InputControl(name = "buttonWest", bit = 14U, displayName = "X")]
		[InputControl(name = "buttonNorth", bit = 15U, displayName = "Y")]
		[FieldOffset(0)]
		public ushort buttons;

		[InputControl(name = "leftTrigger", format = "BYTE")]
		[FieldOffset(2)]
		public byte leftTrigger;

		[InputControl(name = "rightTrigger", format = "BYTE")]
		[FieldOffset(3)]
		public byte rightTrigger;

		[InputControl(name = "leftStick", layout = "Stick", format = "VC2S")]
		[InputControl(name = "leftStick/x", offset = 0U, format = "SHRT", parameters = "clamp=false,invert=false,normalize=false")]
		[InputControl(name = "leftStick/left", offset = 0U, format = "SHRT")]
		[InputControl(name = "leftStick/right", offset = 0U, format = "SHRT")]
		[InputControl(name = "leftStick/y", offset = 2U, format = "SHRT", parameters = "clamp=false,invert=false,normalize=false")]
		[InputControl(name = "leftStick/up", offset = 2U, format = "SHRT")]
		[InputControl(name = "leftStick/down", offset = 2U, format = "SHRT")]
		[FieldOffset(4)]
		public short leftStickX;

		[FieldOffset(6)]
		public short leftStickY;

		[InputControl(name = "rightStick", layout = "Stick", format = "VC2S")]
		[InputControl(name = "rightStick/x", offset = 0U, format = "SHRT", parameters = "clamp=false,invert=false,normalize=false")]
		[InputControl(name = "rightStick/left", offset = 0U, format = "SHRT")]
		[InputControl(name = "rightStick/right", offset = 0U, format = "SHRT")]
		[InputControl(name = "rightStick/y", offset = 2U, format = "SHRT", parameters = "clamp=false,invert=false,normalize=false")]
		[InputControl(name = "rightStick/up", offset = 2U, format = "SHRT")]
		[InputControl(name = "rightStick/down", offset = 2U, format = "SHRT")]
		[FieldOffset(8)]
		public short rightStickX;

		[FieldOffset(10)]
		public short rightStickY;

		public enum Button
		{
			DPadUp,
			DPadDown,
			DPadLeft,
			DPadRight,
			Start,
			Select,
			LeftThumbstickPress,
			RightThumbstickPress,
			LeftShoulder,
			RightShoulder,
			A = 12,
			B,
			X,
			Y
		}
	}
}
