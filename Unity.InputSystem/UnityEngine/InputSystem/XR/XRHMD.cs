using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.InputSystem.XR
{
	[InputControlLayout(isGenericTypeOfDevice = true, displayName = "XR HMD", canRunInBackground = true)]
	public class XRHMD : TrackedDevice
	{
		[InputControl(noisy = true)]
		public Vector3Control leftEyePosition { get; protected set; }

		[InputControl(noisy = true)]
		public QuaternionControl leftEyeRotation { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control rightEyePosition { get; protected set; }

		[InputControl(noisy = true)]
		public QuaternionControl rightEyeRotation { get; protected set; }

		[InputControl(noisy = true)]
		public Vector3Control centerEyePosition { get; protected set; }

		[InputControl(noisy = true)]
		public QuaternionControl centerEyeRotation { get; protected set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.centerEyePosition = base.GetChildControl<Vector3Control>("centerEyePosition");
			this.centerEyeRotation = base.GetChildControl<QuaternionControl>("centerEyeRotation");
			this.leftEyePosition = base.GetChildControl<Vector3Control>("leftEyePosition");
			this.leftEyeRotation = base.GetChildControl<QuaternionControl>("leftEyeRotation");
			this.rightEyePosition = base.GetChildControl<Vector3Control>("rightEyePosition");
			this.rightEyeRotation = base.GetChildControl<QuaternionControl>("rightEyeRotation");
		}
	}
}
