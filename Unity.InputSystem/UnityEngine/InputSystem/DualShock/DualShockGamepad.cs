using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.DualShock
{
	[InputControlLayout(displayName = "PlayStation Controller")]
	public class DualShockGamepad : Gamepad, IDualShockHaptics, IDualMotorRumble, IHaptics
	{
		[InputControl(name = "buttonWest", displayName = "Square", shortDisplayName = "Square")]
		[InputControl(name = "buttonNorth", displayName = "Triangle", shortDisplayName = "Triangle")]
		[InputControl(name = "buttonEast", displayName = "Circle", shortDisplayName = "Circle")]
		[InputControl(name = "buttonSouth", displayName = "Cross", shortDisplayName = "Cross")]
		[InputControl]
		public ButtonControl touchpadButton { get; protected set; }

		[InputControl(name = "start", displayName = "Options")]
		public ButtonControl optionsButton { get; protected set; }

		[InputControl(name = "select", displayName = "Share")]
		public ButtonControl shareButton { get; protected set; }

		[InputControl(name = "leftShoulder", displayName = "L1", shortDisplayName = "L1")]
		public ButtonControl L1 { get; protected set; }

		[InputControl(name = "rightShoulder", displayName = "R1", shortDisplayName = "R1")]
		public ButtonControl R1 { get; protected set; }

		[InputControl(name = "leftTrigger", displayName = "L2", shortDisplayName = "L2")]
		public ButtonControl L2 { get; protected set; }

		[InputControl(name = "rightTrigger", displayName = "R2", shortDisplayName = "R2")]
		public ButtonControl R2 { get; protected set; }

		[InputControl(name = "leftStickPress", displayName = "L3", shortDisplayName = "L3")]
		public ButtonControl L3 { get; protected set; }

		[InputControl(name = "rightStickPress", displayName = "R3", shortDisplayName = "R3")]
		public ButtonControl R3 { get; protected set; }

		public new static DualShockGamepad current { get; private set; }

		internal HID.HIDDeviceDescriptor hidDescriptor { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			DualShockGamepad.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (DualShockGamepad.current == this)
			{
				DualShockGamepad.current = null;
			}
		}

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.touchpadButton = base.GetChildControl<ButtonControl>("touchpadButton");
			this.optionsButton = base.startButton;
			this.shareButton = base.selectButton;
			this.L1 = base.leftShoulder;
			this.R1 = base.rightShoulder;
			this.L2 = base.leftTrigger;
			this.R2 = base.rightTrigger;
			this.L3 = base.leftStickButton;
			this.R3 = base.rightStickButton;
			if (this.m_Description.capabilities != null && this.m_Description.interfaceName == "HID")
			{
				this.hidDescriptor = HID.HIDDeviceDescriptor.FromJson(this.m_Description.capabilities);
			}
		}

		public virtual void SetLightBarColor(Color color)
		{
		}
	}
}
