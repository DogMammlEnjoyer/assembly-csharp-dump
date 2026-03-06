using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace Unity.XR.Oculus.Input
{
	[InputControlLayout(displayName = "Oculus Remote", hideInUI = true)]
	public class OculusRemote : InputDevice
	{
		[InputControl]
		public ButtonControl back { get; protected set; }

		[InputControl]
		public ButtonControl start { get; protected set; }

		[InputControl]
		public Vector2Control touchpad { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.back = base.GetChildControl<ButtonControl>("back");
			this.start = base.GetChildControl<ButtonControl>("start");
			this.touchpad = base.GetChildControl<Vector2Control>("touchpad");
		}
	}
}
