using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Switch.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 7)]
	internal struct SwitchProControllerHIDInputState : IInputStateTypeInfo
	{
		public FourCC format
		{
			get
			{
				return SwitchProControllerHIDInputState.Format;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SwitchProControllerHIDInputState WithButton(SwitchProControllerHIDInputState.Button button, bool value = true)
		{
			this.Set(button, value);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(SwitchProControllerHIDInputState.Button button, bool state)
		{
			if (button >= SwitchProControllerHIDInputState.Button.Capture)
			{
				if (button < (SwitchProControllerHIDInputState.Button)18)
				{
					byte b = (byte)(1 << button - SwitchProControllerHIDInputState.Button.Capture);
					if (state)
					{
						this.buttons2 |= b;
						return;
					}
					this.buttons2 &= ~b;
				}
				return;
			}
			ushort num = (ushort)(1 << (int)button);
			if (state)
			{
				this.buttons1 |= num;
				return;
			}
			this.buttons1 &= ~num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Press(SwitchProControllerHIDInputState.Button button)
		{
			this.Set(button, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Release(SwitchProControllerHIDInputState.Button button)
		{
			this.Set(button, false);
		}

		public static FourCC Format = new FourCC('S', 'P', 'V', 'S');

		[InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "leftStick/x", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "leftStick/left", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/right", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85")]
		[InputControl(name = "leftStick/y", offset = 1U, format = "BYTE", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "leftStick/up", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
		[InputControl(name = "leftStick/down", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
		[FieldOffset(0)]
		public byte leftStickX;

		[FieldOffset(1)]
		public byte leftStickY;

		[InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
		[InputControl(name = "rightStick/x", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "rightStick/left", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/right", offset = 0U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
		[InputControl(name = "rightStick/y", offset = 1U, format = "BYTE", parameters = "invert,normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5")]
		[InputControl(name = "rightStick/up", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.15,clampMax=0.5,invert")]
		[InputControl(name = "rightStick/down", offset = 1U, format = "BYTE", parameters = "normalize,normalizeMin=0.15,normalizeMax=0.85,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=0.85,invert=false")]
		[FieldOffset(2)]
		public byte rightStickX;

		[FieldOffset(3)]
		public byte rightStickY;

		[InputControl(name = "dpad", format = "BIT", bit = 0U, sizeInBits = 4U)]
		[InputControl(name = "dpad/up", bit = 0U)]
		[InputControl(name = "dpad/right", bit = 1U)]
		[InputControl(name = "dpad/down", bit = 2U)]
		[InputControl(name = "dpad/left", bit = 3U)]
		[InputControl(name = "buttonWest", displayName = "Y", shortDisplayName = "Y", bit = 4U, usage = "SecondaryAction")]
		[InputControl(name = "buttonNorth", displayName = "X", shortDisplayName = "X", bit = 5U)]
		[InputControl(name = "buttonSouth", displayName = "B", shortDisplayName = "B", bit = 6U, usages = new string[]
		{
			"Back",
			"Cancel"
		})]
		[InputControl(name = "buttonEast", displayName = "A", shortDisplayName = "A", bit = 7U, usages = new string[]
		{
			"PrimaryAction",
			"Submit"
		})]
		[InputControl(name = "leftShoulder", displayName = "L", shortDisplayName = "L", bit = 8U)]
		[InputControl(name = "rightShoulder", displayName = "R", shortDisplayName = "R", bit = 9U)]
		[InputControl(name = "leftStickPress", displayName = "Left Stick", bit = 10U)]
		[InputControl(name = "rightStickPress", displayName = "Right Stick", bit = 11U)]
		[InputControl(name = "leftTrigger", displayName = "ZL", shortDisplayName = "ZL", format = "BIT", bit = 12U)]
		[InputControl(name = "rightTrigger", displayName = "ZR", shortDisplayName = "ZR", format = "BIT", bit = 13U)]
		[InputControl(name = "start", displayName = "Plus", bit = 14U, usage = "Menu")]
		[InputControl(name = "select", displayName = "Minus", bit = 15U)]
		[FieldOffset(4)]
		public ushort buttons1;

		[InputControl(name = "capture", layout = "Button", displayName = "Capture", bit = 0U)]
		[InputControl(name = "home", layout = "Button", displayName = "Home", bit = 1U)]
		[FieldOffset(6)]
		public byte buttons2;

		public enum Button
		{
			Up,
			Right,
			Down,
			Left,
			West,
			North,
			South,
			East,
			L,
			R,
			StickL,
			StickR,
			ZL,
			ZR,
			Plus,
			Minus,
			Capture,
			Home,
			X = 5,
			B,
			Y = 4,
			A = 7
		}
	}
}
