using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace Unity.XR.Oculus.Input
{
	[InputControlLayout(displayName = "Oculus Headset", hideInUI = true)]
	public class OculusHMD : XRHMD
	{
		[InputControl]
		[InputControl(name = "trackingState", layout = "Integer", aliases = new string[]
		{
			"devicetrackingstate"
		})]
		[InputControl(name = "isTracked", layout = "Button", aliases = new string[]
		{
			"deviceistracked"
		})]
		public ButtonControl userPresence { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control leftEyeAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control leftEyeAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control leftEyeAngularAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control rightEyeAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control rightEyeAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control rightEyeAngularAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control centerEyeAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control centerEyeAcceleration { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control centerEyeAngularAcceleration { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.userPresence = base.GetChildControl<ButtonControl>("userPresence");
			this.deviceAngularVelocity = base.GetChildControl<Vector3Control>("deviceAngularVelocity");
			this.deviceAcceleration = base.GetChildControl<Vector3Control>("deviceAcceleration");
			this.deviceAngularAcceleration = base.GetChildControl<Vector3Control>("deviceAngularAcceleration");
			this.leftEyeAngularVelocity = base.GetChildControl<Vector3Control>("leftEyeAngularVelocity");
			this.leftEyeAcceleration = base.GetChildControl<Vector3Control>("leftEyeAcceleration");
			this.leftEyeAngularAcceleration = base.GetChildControl<Vector3Control>("leftEyeAngularAcceleration");
			this.rightEyeAngularVelocity = base.GetChildControl<Vector3Control>("rightEyeAngularVelocity");
			this.rightEyeAcceleration = base.GetChildControl<Vector3Control>("rightEyeAcceleration");
			this.rightEyeAngularAcceleration = base.GetChildControl<Vector3Control>("rightEyeAngularAcceleration");
			this.centerEyeAngularVelocity = base.GetChildControl<Vector3Control>("centerEyeAngularVelocity");
			this.centerEyeAcceleration = base.GetChildControl<Vector3Control>("centerEyeAcceleration");
			this.centerEyeAngularAcceleration = base.GetChildControl<Vector3Control>("centerEyeAngularAcceleration");
		}
	}
}
