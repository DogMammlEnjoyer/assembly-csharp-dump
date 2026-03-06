using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "Vive Wand", commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, hideInUI = true)]
	public class ViveWand : XRControllerWithRumble
	{
		[InputControl]
		public AxisControl grip { get; protected set; }

		[InputControl]
		public ButtonControl gripPressed { get; protected set; }

		[InputControl]
		public ButtonControl primary { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"primary2DAxisClick",
			"joystickOrPadPressed"
		})]
		public ButtonControl trackpadPressed { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"primary2DAxisTouch",
			"joystickOrPadTouched"
		})]
		public ButtonControl trackpadTouched { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"Primary2DAxis"
		})]
		public Vector2Control trackpad { get; protected set; }

		[InputControl]
		public AxisControl trigger { get; protected set; }

		[InputControl]
		public ButtonControl triggerPressed { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularVelocity { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.grip = base.GetChildControl<AxisControl>("grip");
			this.primary = base.GetChildControl<ButtonControl>("primary");
			this.gripPressed = base.GetChildControl<ButtonControl>("gripPressed");
			this.trackpadPressed = base.GetChildControl<ButtonControl>("trackpadPressed");
			this.trackpadTouched = base.GetChildControl<ButtonControl>("trackpadTouched");
			this.trackpad = base.GetChildControl<Vector2Control>("trackpad");
			this.trigger = base.GetChildControl<AxisControl>("trigger");
			this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
			this.deviceVelocity = base.GetChildControl<Vector3Control>("deviceVelocity");
			this.deviceAngularVelocity = base.GetChildControl<Vector3Control>("deviceAngularVelocity");
		}
	}
}
