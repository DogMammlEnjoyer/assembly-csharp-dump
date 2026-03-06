using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace Unity.XR.OpenVR
{
	[InputControlLayout(displayName = "Vive Tracker")]
	public class ViveTracker : TrackedDevice
	{
		[InputControl(noisy = true)]
		public Vector3Control deviceVelocity { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control deviceAngularVelocity { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.deviceVelocity = base.GetChildControl<Vector3Control>("deviceVelocity");
			this.deviceAngularVelocity = base.GetChildControl<Vector3Control>("deviceAngularVelocity");
		}
	}
}
