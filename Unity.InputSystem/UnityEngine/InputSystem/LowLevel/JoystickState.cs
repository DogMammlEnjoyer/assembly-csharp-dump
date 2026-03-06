using System;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	internal struct JoystickState : IInputStateTypeInfo
	{
		public static FourCC kFormat
		{
			get
			{
				return new FourCC('J', 'O', 'Y', ' ');
			}
		}

		public FourCC format
		{
			get
			{
				return JoystickState.kFormat;
			}
		}

		[InputControl(name = "trigger", displayName = "Trigger", layout = "Button", usages = new string[]
		{
			"PrimaryTrigger",
			"PrimaryAction",
			"Submit"
		}, bit = 4U)]
		public int buttons;

		[InputControl(displayName = "Stick", layout = "Stick", usage = "Primary2DMotion", processors = "stickDeadzone")]
		public Vector2 stick;

		public enum Button
		{
			HatSwitchUp,
			HatSwitchDown,
			HatSwitchLeft,
			HatSwitchRight,
			Trigger
		}
	}
}
