using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "OpenVR Headset", hideInUI = true)]
	public class OpenVRHMD : XRHMD
	{
		[InputControl(noisy = true)]
		public Vector3Control deviceVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control leftEyeVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control leftEyeAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control rightEyeVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control rightEyeAngularVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control centerEyeVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control centerEyeAngularVelocity { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.deviceVelocity = base.GetChildControl<Vector3Control>("deviceVelocity");
			this.deviceAngularVelocity = base.GetChildControl<Vector3Control>("deviceAngularVelocity");
			this.leftEyeVelocity = base.GetChildControl<Vector3Control>("leftEyeVelocity");
			this.leftEyeAngularVelocity = base.GetChildControl<Vector3Control>("leftEyeAngularVelocity");
			this.rightEyeVelocity = base.GetChildControl<Vector3Control>("rightEyeVelocity");
			this.rightEyeAngularVelocity = base.GetChildControl<Vector3Control>("rightEyeAngularVelocity");
			this.centerEyeVelocity = base.GetChildControl<Vector3Control>("centerEyeVelocity");
			this.centerEyeAngularVelocity = base.GetChildControl<Vector3Control>("centerEyeAngularVelocity");
		}
	}
}
