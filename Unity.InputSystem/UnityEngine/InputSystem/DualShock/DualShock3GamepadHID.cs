using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.DualShock
{
	[InputControlLayout(stateType = typeof(DualShock3HIDInputReport), hideInUI = true, displayName = "PS3 Controller")]
	public class DualShock3GamepadHID : DualShockGamepad
	{
		public ButtonControl leftTriggerButton { get; protected set; }

		public ButtonControl rightTriggerButton { get; protected set; }

		public ButtonControl playStationButton { get; protected set; }

		protected override void FinishSetup()
		{
			this.leftTriggerButton = base.GetChildControl<ButtonControl>("leftTriggerButton");
			this.rightTriggerButton = base.GetChildControl<ButtonControl>("rightTriggerButton");
			this.playStationButton = base.GetChildControl<ButtonControl>("systemButton");
			base.FinishSetup();
		}
	}
}
