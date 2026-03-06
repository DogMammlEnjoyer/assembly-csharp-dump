using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 28)]
	public struct GamepadState : IInputStateTypeInfo
	{
		public static FourCC Format
		{
			get
			{
				return new FourCC('G', 'P', 'A', 'D');
			}
		}

		public FourCC format
		{
			get
			{
				return GamepadState.Format;
			}
		}

		public GamepadState(params GamepadButton[] buttons)
		{
			this = default(GamepadState);
			if (buttons == null)
			{
				throw new ArgumentNullException("buttons");
			}
			foreach (GamepadButton gamepadButton in buttons)
			{
				uint num = 1U << (int)gamepadButton;
				this.buttons |= num;
			}
		}

		public GamepadState WithButton(GamepadButton button, bool value = true)
		{
			uint num = 1U << (int)button;
			if (value)
			{
				this.buttons |= num;
			}
			else
			{
				this.buttons &= ~num;
			}
			return this;
		}

		internal const string ButtonSouthShortDisplayName = "A";

		internal const string ButtonNorthShortDisplayName = "Y";

		internal const string ButtonWestShortDisplayName = "X";

		internal const string ButtonEastShortDisplayName = "B";

		[InputControl(name = "dpad", layout = "Dpad", usage = "Hatswitch", displayName = "D-Pad", format = "BIT", sizeInBits = 4U, bit = 0U)]
		[InputControl(name = "buttonSouth", layout = "Button", bit = 6U, usages = new string[]
		{
			"PrimaryAction",
			"Submit"
		}, aliases = new string[]
		{
			"a",
			"cross"
		}, displayName = "Button South", shortDisplayName = "A")]
		[InputControl(name = "buttonWest", layout = "Button", bit = 7U, usage = "SecondaryAction", aliases = new string[]
		{
			"x",
			"square"
		}, displayName = "Button West", shortDisplayName = "X")]
		[InputControl(name = "buttonNorth", layout = "Button", bit = 4U, aliases = new string[]
		{
			"y",
			"triangle"
		}, displayName = "Button North", shortDisplayName = "Y")]
		[InputControl(name = "buttonEast", layout = "Button", bit = 5U, usages = new string[]
		{
			"Back",
			"Cancel"
		}, aliases = new string[]
		{
			"b",
			"circle"
		}, displayName = "Button East", shortDisplayName = "B")]
		[InputControl(name = "leftStickPress", layout = "Button", bit = 8U, displayName = "Left Stick Press")]
		[InputControl(name = "rightStickPress", layout = "Button", bit = 9U, displayName = "Right Stick Press")]
		[InputControl(name = "leftShoulder", layout = "Button", bit = 10U, displayName = "Left Shoulder", shortDisplayName = "LB")]
		[InputControl(name = "rightShoulder", layout = "Button", bit = 11U, displayName = "Right Shoulder", shortDisplayName = "RB")]
		[InputControl(name = "start", layout = "Button", bit = 12U, usage = "Menu", displayName = "Start")]
		[InputControl(name = "select", layout = "Button", bit = 13U, displayName = "Select")]
		[FieldOffset(0)]
		public uint buttons;

		[InputControl(layout = "Stick", usage = "Primary2DMotion", processors = "stickDeadzone", displayName = "Left Stick", shortDisplayName = "LS")]
		[FieldOffset(4)]
		public Vector2 leftStick;

		[InputControl(layout = "Stick", usage = "Secondary2DMotion", processors = "stickDeadzone", displayName = "Right Stick", shortDisplayName = "RS")]
		[FieldOffset(12)]
		public Vector2 rightStick;

		[InputControl(layout = "Button", format = "FLT", usage = "SecondaryTrigger", displayName = "Left Trigger", shortDisplayName = "LT")]
		[FieldOffset(20)]
		public float leftTrigger;

		[InputControl(layout = "Button", format = "FLT", usage = "SecondaryTrigger", displayName = "Right Trigger", shortDisplayName = "RT")]
		[FieldOffset(24)]
		public float rightTrigger;
	}
}
