using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace Unity.XR.GoogleVr
{
	[InputControlLayout(displayName = "Daydream Controller", commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, hideInUI = true)]
	public class DaydreamController : XRController
	{
		[InputControl]
		public Vector2Control touchpad { get; protected set; }

		[InputControl]
		public ButtonControl volumeUp { get; protected set; }

		[InputControl]
		public ButtonControl recentered { get; protected set; }

		[InputControl]
		public ButtonControl volumeDown { get; protected set; }

		[InputControl]
		public ButtonControl recentering { get; protected set; }

		[InputControl]
		public ButtonControl app { get; protected set; }

		[InputControl]
		public ButtonControl home { get; protected set; }

		[InputControl]
		public ButtonControl touchpadClicked { get; protected set; }

		[InputControl]
		public ButtonControl touchpadTouched { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAcceleration { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.touchpad = base.GetChildControl<Vector2Control>("touchpad");
			this.volumeUp = base.GetChildControl<ButtonControl>("volumeUp");
			this.recentered = base.GetChildControl<ButtonControl>("recentered");
			this.volumeDown = base.GetChildControl<ButtonControl>("volumeDown");
			this.recentering = base.GetChildControl<ButtonControl>("recentering");
			this.app = base.GetChildControl<ButtonControl>("app");
			this.home = base.GetChildControl<ButtonControl>("home");
			this.touchpadClicked = base.GetChildControl<ButtonControl>("touchpadClicked");
			this.touchpadTouched = base.GetChildControl<ButtonControl>("touchpadTouched");
			this.deviceVelocity = base.GetChildControl<Vector3Control>("deviceVelocity");
			this.deviceAcceleration = base.GetChildControl<Vector3Control>("deviceAcceleration");
		}
	}
}
