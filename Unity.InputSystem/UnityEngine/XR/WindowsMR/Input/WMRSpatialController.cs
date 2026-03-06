using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.XR.WindowsMR.Input
{
	[InputControlLayout(displayName = "Windows MR Controller", commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, hideInUI = true)]
	public class WMRSpatialController : XRControllerWithRumble
	{
		[InputControl(aliases = new string[]
		{
			"Primary2DAxis",
			"thumbstickaxes"
		})]
		public Vector2Control joystick { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"Secondary2DAxis",
			"touchpadaxes"
		})]
		public Vector2Control touchpad { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"gripaxis"
		})]
		public AxisControl grip { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"gripbutton"
		})]
		public ButtonControl gripPressed { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"Primary",
			"menubutton"
		})]
		public ButtonControl menu { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"triggeraxis"
		})]
		public AxisControl trigger { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"triggerbutton"
		})]
		public ButtonControl triggerPressed { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"thumbstickpressed"
		})]
		public ButtonControl joystickClicked { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"joystickorpadpressed",
			"touchpadpressed"
		})]
		public ButtonControl touchpadClicked { get; protected set; }

		[InputControl(aliases = new string[]
		{
			"joystickorpadtouched",
			"touchpadtouched"
		})]
		public ButtonControl touchpadTouched { get; protected set; }

		[InputControl(noisy = true, aliases = new string[]
		{
			"gripVelocity"
		})]
		public Vector3Control deviceVelocity { get; protected set; }

		[InputControl(noisy = true, aliases = new string[]
		{
			"gripAngularVelocity"
		})]
		public Vector3Control deviceAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public AxisControl batteryLevel { get; protected set; }

		[InputControl(noisy = true)]
		public AxisControl sourceLossRisk { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control sourceLossMitigationDirection { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control pointerPosition { get; protected set; }

		[InputControl(noisy = true, aliases = new string[]
		{
			"PointerOrientation"
		})]
		public QuaternionControl pointerRotation { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.joystick = base.GetChildControl<Vector2Control>("joystick");
			this.trigger = base.GetChildControl<AxisControl>("trigger");
			this.touchpad = base.GetChildControl<Vector2Control>("touchpad");
			this.grip = base.GetChildControl<AxisControl>("grip");
			this.gripPressed = base.GetChildControl<ButtonControl>("gripPressed");
			this.menu = base.GetChildControl<ButtonControl>("menu");
			this.joystickClicked = base.GetChildControl<ButtonControl>("joystickClicked");
			this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
			this.touchpadClicked = base.GetChildControl<ButtonControl>("touchpadClicked");
			this.touchpadTouched = base.GetChildControl<ButtonControl>("touchPadTouched");
			this.deviceVelocity = base.GetChildControl<Vector3Control>("deviceVelocity");
			this.deviceAngularVelocity = base.GetChildControl<Vector3Control>("deviceAngularVelocity");
			this.batteryLevel = base.GetChildControl<AxisControl>("batteryLevel");
			this.sourceLossRisk = base.GetChildControl<AxisControl>("sourceLossRisk");
			this.sourceLossMitigationDirection = base.GetChildControl<Vector3Control>("sourceLossMitigationDirection");
			this.pointerPosition = base.GetChildControl<Vector3Control>("pointerPosition");
			this.pointerRotation = base.GetChildControl<QuaternionControl>("pointerRotation");
		}
	}
}
