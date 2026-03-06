using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "Oculus Touch Controller (OpenVR)", commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, hideInUI = true)]
	public class OpenVROculusTouchController : XRControllerWithRumble
	{
		[InputControl]
		public Vector2Control thumbstick { get; protected set; }

		[InputControl]
		public AxisControl trigger { get; protected set; }

		[InputControl]
		public AxisControl grip { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"Alternate"
		})]
		public ButtonControl primaryButton { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"Primary"
		})]
		public ButtonControl secondaryButton { get; protected set; }

		[InputControl]
		public ButtonControl gripPressed { get; protected set; }

		[InputControl]
		public ButtonControl triggerPressed { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"primary2DAxisClicked"
		})]
		public ButtonControl thumbstickClicked { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"primary2DAxisTouch"
		})]
		public ButtonControl thumbstickTouched { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularVelocity { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.thumbstick = base.GetChildControl<Vector2Control>("thumbstick");
			this.trigger = base.GetChildControl<AxisControl>("trigger");
			this.grip = base.GetChildControl<AxisControl>("grip");
			this.primaryButton = base.GetChildControl<ButtonControl>("primaryButton");
			this.secondaryButton = base.GetChildControl<ButtonControl>("secondaryButton");
			this.gripPressed = base.GetChildControl<ButtonControl>("gripPressed");
			this.thumbstickClicked = base.GetChildControl<ButtonControl>("thumbstickClicked");
			this.thumbstickTouched = base.GetChildControl<ButtonControl>("thumbstickTouched");
			this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
			this.deviceVelocity = base.GetChildControl<Vector3Control>("deviceVelocity");
			this.deviceAngularVelocity = base.GetChildControl<Vector3Control>("deviceAngularVelocity");
		}
	}
}
