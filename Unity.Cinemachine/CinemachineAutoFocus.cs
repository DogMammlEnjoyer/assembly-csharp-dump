using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Auto Focus")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineAutoFocus.html")]
	public class CinemachineAutoFocus : CinemachineExtension
	{
		private void Reset()
		{
			this.Damping = 0.2f;
			this.FocusTarget = CinemachineAutoFocus.FocusTrackingMode.None;
			this.CustomTarget = null;
			this.FocusDepthOffset = 0f;
		}

		private void OnValidate()
		{
			this.Damping = Mathf.Max(0f, this.Damping);
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Finalize && this.FocusTarget != CinemachineAutoFocus.FocusTrackingMode.None)
			{
				CinemachineAutoFocus.VcamExtraState extraState = base.GetExtraState<CinemachineAutoFocus.VcamExtraState>(vcam);
				float num = 0f;
				Transform transform = null;
				switch (this.FocusTarget)
				{
				case CinemachineAutoFocus.FocusTrackingMode.LookAtTarget:
					if (state.HasLookAt())
					{
						num = (state.GetFinalPosition() - state.ReferenceLookAt).magnitude;
					}
					else
					{
						transform = vcam.LookAt;
					}
					break;
				case CinemachineAutoFocus.FocusTrackingMode.FollowTarget:
					transform = vcam.Follow;
					break;
				case CinemachineAutoFocus.FocusTrackingMode.CustomTarget:
					transform = this.CustomTarget;
					break;
				}
				if (transform != null)
				{
					num += (state.GetFinalPosition() - transform.position).magnitude;
				}
				num = Mathf.Max(0.1f, num + this.FocusDepthOffset);
				if (deltaTime >= 0f && vcam.PreviousStateIsValid)
				{
					num = extraState.CurrentFocusDistance + Damper.Damp(num - extraState.CurrentFocusDistance, this.Damping, deltaTime);
				}
				extraState.CurrentFocusDistance = num;
				state.Lens.PhysicalProperties.FocusDistance = num;
			}
		}

		[Tooltip("The camera's focus distance will be set to the distance from the camera to the selected target.  The Focus Offset field will then modify that distance.")]
		public CinemachineAutoFocus.FocusTrackingMode FocusTarget;

		[Tooltip("The target to use if Focus Target is set to Custom Target")]
		public Transform CustomTarget;

		[Tooltip("Offsets the sharpest point away in depth from the focus target location.")]
		public float FocusDepthOffset;

		[Tooltip("The value corresponds approximately to the time the focus will take to adjust to the new value.")]
		public float Damping;

		public enum FocusTrackingMode
		{
			None,
			LookAtTarget,
			FollowTarget,
			CustomTarget,
			Camera,
			ScreenCenter
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public float CurrentFocusDistance;
		}
	}
}
