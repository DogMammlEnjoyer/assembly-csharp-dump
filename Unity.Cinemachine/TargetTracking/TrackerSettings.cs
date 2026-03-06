using System;
using UnityEngine;

namespace Unity.Cinemachine.TargetTracking
{
	[Serializable]
	public struct TrackerSettings
	{
		public static TrackerSettings Default
		{
			get
			{
				return new TrackerSettings
				{
					BindingMode = BindingMode.WorldSpace,
					PositionDamping = Vector3.one,
					AngularDampingMode = AngularDampingMode.Euler,
					RotationDamping = Vector3.one,
					QuaternionDamping = 1f
				};
			}
		}

		public void Validate()
		{
			this.PositionDamping.x = Mathf.Max(0f, this.PositionDamping.x);
			this.PositionDamping.y = Mathf.Max(0f, this.PositionDamping.y);
			this.PositionDamping.z = Mathf.Max(0f, this.PositionDamping.z);
			this.RotationDamping.x = Mathf.Max(0f, this.RotationDamping.x);
			this.RotationDamping.y = Mathf.Max(0f, this.RotationDamping.y);
			this.RotationDamping.z = Mathf.Max(0f, this.RotationDamping.z);
			this.QuaternionDamping = Mathf.Max(0f, this.QuaternionDamping);
		}

		[Tooltip("The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.")]
		public BindingMode BindingMode;

		[Tooltip("How aggressively the camera tries to maintain the offset, per axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
		public Vector3 PositionDamping;

		public AngularDampingMode AngularDampingMode;

		[Tooltip("How aggressively the camera tries to track the target's rotation, per axis.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public Vector3 RotationDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target's rotation.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float QuaternionDamping;
	}
}
