using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Obsolete("InputHelpers has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader or XRInputDeviceValueReader instead.")]
	public static class InputHelpers
	{
		[Obsolete("IsPressed has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader instead.")]
		public static bool IsPressed(this InputDevice device, InputHelpers.Button button, out bool isPressed, float pressThreshold = -1f)
		{
			if (button >= (InputHelpers.Button)InputHelpers.s_ButtonData.Length)
			{
				throw new ArgumentException("[InputHelpers.IsPressed] The value of <button> is out of the supported range.");
			}
			if (!device.isValid)
			{
				isPressed = false;
				return false;
			}
			InputHelpers.ButtonInfo buttonInfo = InputHelpers.s_ButtonData[(int)button];
			switch (buttonInfo.type)
			{
			case InputHelpers.ButtonReadType.Binary:
			{
				bool flag;
				if (device.TryGetFeatureValue(new InputFeatureUsage<bool>(buttonInfo.name), out flag))
				{
					isPressed = flag;
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis1D:
			{
				float num;
				if (device.TryGetFeatureValue(new InputFeatureUsage<float>(buttonInfo.name), out num))
				{
					float num2 = (pressThreshold >= 0f) ? pressThreshold : 0.1f;
					isPressed = (num >= num2);
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DUp:
			{
				Vector2 vector;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector))
				{
					float num3 = (pressThreshold >= 0f) ? pressThreshold : 0.1f;
					isPressed = (vector.y >= num3);
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DDown:
			{
				Vector2 vector2;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector2))
				{
					float num4 = (pressThreshold >= 0f) ? pressThreshold : 0.1f;
					isPressed = (vector2.y <= -num4);
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DLeft:
			{
				Vector2 vector3;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector3))
				{
					float num5 = (pressThreshold >= 0f) ? pressThreshold : 0.1f;
					isPressed = (vector3.x <= -num5);
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DRight:
			{
				Vector2 vector4;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector4))
				{
					float num6 = (pressThreshold >= 0f) ? pressThreshold : 0.1f;
					isPressed = (vector4.x >= num6);
					return true;
				}
				break;
			}
			}
			isPressed = false;
			return false;
		}

		[Obsolete("TryReadSingleValue has been deprecated in version 3.0.0. Use XRInputDeviceValueReader instead.")]
		public static bool TryReadSingleValue(this InputDevice device, InputHelpers.Button button, out float singleValue)
		{
			if (button >= (InputHelpers.Button)InputHelpers.s_ButtonData.Length)
			{
				throw new ArgumentException("[InputHelpers.TryReadSingleValue] The value of <button> is out of the supported range.");
			}
			if (!device.isValid)
			{
				singleValue = 0f;
				return false;
			}
			InputHelpers.ButtonInfo buttonInfo = InputHelpers.s_ButtonData[(int)button];
			switch (buttonInfo.type)
			{
			case InputHelpers.ButtonReadType.Binary:
			{
				bool flag;
				if (device.TryGetFeatureValue(new InputFeatureUsage<bool>(buttonInfo.name), out flag))
				{
					singleValue = (flag ? 1f : 0f);
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis1D:
			{
				float num;
				if (device.TryGetFeatureValue(new InputFeatureUsage<float>(buttonInfo.name), out num))
				{
					singleValue = num;
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DUp:
			{
				Vector2 vector;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector))
				{
					singleValue = vector.y;
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DDown:
			{
				Vector2 vector2;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector2))
				{
					singleValue = -vector2.y;
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DLeft:
			{
				Vector2 vector3;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector3))
				{
					singleValue = -vector3.x;
					return true;
				}
				break;
			}
			case InputHelpers.ButtonReadType.Axis2DRight:
			{
				Vector2 vector4;
				if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(buttonInfo.name), out vector4))
				{
					singleValue = vector4.x;
					return true;
				}
				break;
			}
			}
			singleValue = 0f;
			return false;
		}

		[Obsolete("TryReadAxis2DValue has been deprecated in version 3.0.0. Use XRInputDeviceValueReader instead.")]
		public static bool TryReadAxis2DValue(this InputDevice device, InputHelpers.Axis2D axis2D, out Vector2 value)
		{
			if (axis2D >= (InputHelpers.Axis2D)InputHelpers.s_Axis2DNames.Length)
			{
				throw new ArgumentException("[InputHelpers.TryReadAxis2DValue] The value of <axis2D> is out of the supported range.");
			}
			if (!device.isValid)
			{
				value = default(Vector2);
				return false;
			}
			string usageName = InputHelpers.s_Axis2DNames[(int)axis2D];
			if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(usageName), out value))
			{
				return true;
			}
			value = default(Vector2);
			return false;
		}

		private static readonly InputHelpers.ButtonInfo[] s_ButtonData = new InputHelpers.ButtonInfo[]
		{
			new InputHelpers.ButtonInfo("", InputHelpers.ButtonReadType.None),
			new InputHelpers.ButtonInfo("MenuButton", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("Trigger", InputHelpers.ButtonReadType.Axis1D),
			new InputHelpers.ButtonInfo("Grip", InputHelpers.ButtonReadType.Axis1D),
			new InputHelpers.ButtonInfo("TriggerButton", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("GripButton", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("PrimaryButton", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("PrimaryTouch", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("SecondaryButton", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("SecondaryTouch", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("Primary2DAxisTouch", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("Primary2DAxisClick", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("Secondary2DAxisTouch", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("Secondary2DAxisClick", InputHelpers.ButtonReadType.Binary),
			new InputHelpers.ButtonInfo("Primary2DAxis", InputHelpers.ButtonReadType.Axis2DUp),
			new InputHelpers.ButtonInfo("Primary2DAxis", InputHelpers.ButtonReadType.Axis2DDown),
			new InputHelpers.ButtonInfo("Primary2DAxis", InputHelpers.ButtonReadType.Axis2DLeft),
			new InputHelpers.ButtonInfo("Primary2DAxis", InputHelpers.ButtonReadType.Axis2DRight),
			new InputHelpers.ButtonInfo("Secondary2DAxis", InputHelpers.ButtonReadType.Axis2DUp),
			new InputHelpers.ButtonInfo("Secondary2DAxis", InputHelpers.ButtonReadType.Axis2DDown),
			new InputHelpers.ButtonInfo("Secondary2DAxis", InputHelpers.ButtonReadType.Axis2DLeft),
			new InputHelpers.ButtonInfo("Secondary2DAxis", InputHelpers.ButtonReadType.Axis2DRight)
		};

		private static readonly string[] s_Axis2DNames = new string[]
		{
			"",
			"Primary2DAxis",
			"Secondary2DAxis"
		};

		private const float k_DefaultPressThreshold = 0.1f;

		[Obsolete("Button has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader or XRInputDeviceValueReader instead.")]
		public enum Button
		{
			None,
			MenuButton,
			Trigger,
			Grip,
			TriggerButton,
			GripButton,
			PrimaryButton,
			PrimaryTouch,
			SecondaryButton,
			SecondaryTouch,
			Primary2DAxisTouch,
			Primary2DAxisClick,
			Secondary2DAxisTouch,
			Secondary2DAxisClick,
			PrimaryAxis2DUp,
			PrimaryAxis2DDown,
			PrimaryAxis2DLeft,
			PrimaryAxis2DRight,
			SecondaryAxis2DUp,
			SecondaryAxis2DDown,
			SecondaryAxis2DLeft,
			SecondaryAxis2DRight,
			[Obsolete("TriggerPressed has been deprecated. Use TriggerButton instead. (UnityUpgradable) -> TriggerButton", true)]
			TriggerPressed = 4,
			[Obsolete("GripPressed has been deprecated. Use GripButton instead. (UnityUpgradable) -> GripButton", true)]
			GripPressed
		}

		[Obsolete("Axis2D has been deprecated in version 3.0.0. Use XRInputDeviceButtonReader or XRInputDeviceValueReader instead.")]
		public enum Axis2D
		{
			None,
			PrimaryAxis2D,
			SecondaryAxis2D
		}

		private enum ButtonReadType
		{
			None,
			Binary,
			Axis1D,
			Axis2DUp,
			Axis2DDown,
			Axis2DLeft,
			Axis2DRight
		}

		private struct ButtonInfo
		{
			public ButtonInfo(string name, InputHelpers.ButtonReadType type)
			{
				this.name = name;
				this.type = type;
			}

			public string name;

			public InputHelpers.ButtonReadType type;
		}
	}
}
