using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[StructLayout(LayoutKind.Explicit, Size = 63)]
	public struct XRSimulatedControllerState : IInputStateTypeInfo
	{
		public static FourCC formatId
		{
			get
			{
				return new FourCC('X', 'R', 'S', 'C');
			}
		}

		public FourCC format
		{
			get
			{
				return XRSimulatedControllerState.formatId;
			}
		}

		public XRSimulatedControllerState WithButton(ControllerButton button, bool state = true)
		{
			int num = 1 << (int)button;
			if (state)
			{
				this.buttons |= (ushort)num;
			}
			else
			{
				this.buttons &= (ushort)(~(ushort)num);
			}
			return this;
		}

		public XRSimulatedControllerState ToggleButton(ControllerButton button)
		{
			int num = 1 << (int)button;
			this.buttons ^= (ushort)num;
			return this;
		}

		public bool HasButton(ControllerButton button)
		{
			int num = 1 << (int)button;
			return ((int)this.buttons & num) != 0;
		}

		public void Reset()
		{
			this.primary2DAxis = default(Vector2);
			this.trigger = 0f;
			this.grip = 0f;
			this.secondary2DAxis = default(Vector2);
			this.buttons = 0;
			this.batteryLevel = 0f;
			this.trackingState = 0;
			this.isTracked = false;
			this.devicePosition = default(Vector3);
			this.deviceRotation = Quaternion.identity;
		}

		[InputControl(usage = "Primary2DAxis", aliases = new string[]
		{
			"thumbstick",
			"joystick"
		}, offset = 0U)]
		[FieldOffset(0)]
		public Vector2 primary2DAxis;

		[InputControl(usage = "Trigger", layout = "Axis", offset = 8U)]
		[FieldOffset(8)]
		public float trigger;

		[InputControl(usage = "Grip", layout = "Axis", offset = 12U)]
		[FieldOffset(12)]
		public float grip;

		[InputControl(usage = "Secondary2DAxis", offset = 16U)]
		[FieldOffset(16)]
		public Vector2 secondary2DAxis;

		[InputControl(name = "primaryButton", usage = "PrimaryButton", layout = "Button", bit = 0U, offset = 24U)]
		[InputControl(name = "primaryTouch", usage = "PrimaryTouch", layout = "Button", bit = 1U, offset = 24U)]
		[InputControl(name = "secondaryButton", usage = "SecondaryButton", layout = "Button", bit = 2U, offset = 24U)]
		[InputControl(name = "secondaryTouch", usage = "SecondaryTouch", layout = "Button", bit = 3U, offset = 24U)]
		[InputControl(name = "gripButton", usage = "GripButton", layout = "Button", bit = 4U, offset = 24U, alias = "gripPressed")]
		[InputControl(name = "triggerButton", usage = "TriggerButton", layout = "Button", bit = 5U, offset = 24U, alias = "triggerPressed")]
		[InputControl(name = "menuButton", usage = "MenuButton", layout = "Button", bit = 6U, offset = 24U)]
		[InputControl(name = "primary2DAxisClick", usage = "Primary2DAxisClick", layout = "Button", bit = 7U, offset = 24U)]
		[InputControl(name = "primary2DAxisTouch", usage = "Primary2DAxisTouch", layout = "Button", bit = 8U, offset = 24U)]
		[InputControl(name = "secondary2DAxisClick", usage = "Secondary2DAxisClick", layout = "Button", bit = 9U, offset = 24U)]
		[InputControl(name = "secondary2DAxisTouch", usage = "Secondary2DAxisTouch", layout = "Button", bit = 10U, offset = 24U)]
		[InputControl(name = "userPresence", usage = "UserPresence", layout = "Button", bit = 11U, offset = 24U)]
		[FieldOffset(24)]
		public ushort buttons;

		[InputControl(usage = "BatteryLevel", layout = "Axis", offset = 26U)]
		[FieldOffset(26)]
		public float batteryLevel;

		[InputControl(usage = "TrackingState", layout = "Integer", offset = 30U)]
		[FieldOffset(30)]
		public int trackingState;

		[InputControl(usage = "IsTracked", layout = "Button", offset = 34U)]
		[FieldOffset(34)]
		public bool isTracked;

		[InputControl(usage = "DevicePosition", offset = 35U)]
		[FieldOffset(35)]
		public Vector3 devicePosition;

		[InputControl(usage = "DeviceRotation", offset = 47U)]
		[FieldOffset(47)]
		public Quaternion deviceRotation;
	}
}
