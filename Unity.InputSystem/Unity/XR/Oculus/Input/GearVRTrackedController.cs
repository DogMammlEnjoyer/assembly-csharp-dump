using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace Unity.XR.Oculus.Input
{
	[InputControlLayout(displayName = "GearVR Controller", commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, hideInUI = true)]
	public class GearVRTrackedController : XRController
	{
		[InputControl]
		public Vector2Control touchpad { get; protected set; }

		[InputControl]
		public AxisControl trigger { get; protected set; }

		[InputControl]
		public ButtonControl back { get; protected set; }

		[InputControl]
		public ButtonControl triggerPressed { get; protected set; }

		[InputControl]
		public ButtonControl touchpadClicked { get; protected set; }

		[InputControl]
		public ButtonControl touchpadTouched { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularAcceleration { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.touchpad = base.GetChildControl<Vector2Control>("touchpad");
			this.trigger = base.GetChildControl<AxisControl>("trigger");
			this.back = base.GetChildControl<ButtonControl>("back");
			this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
			this.touchpadClicked = base.GetChildControl<ButtonControl>("touchpadClicked");
			this.touchpadTouched = base.GetChildControl<ButtonControl>("touchpadTouched");
			this.deviceAngularVelocity = base.GetChildControl<Vector3Control>("deviceAngularVelocity");
			this.deviceAcceleration = base.GetChildControl<Vector3Control>("deviceAcceleration");
			this.deviceAngularAcceleration = base.GetChildControl<Vector3Control>("deviceAngularAcceleration");
		}
	}
}
