using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Recomposer")]
	[ExecuteAlways]
	[SaveDuringPlay]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineRecomposer.html")]
	public class CinemachineRecomposer : CinemachineExtension
	{
		private void Reset()
		{
			this.ApplyAfter = CinemachineCore.Stage.Finalize;
			this.Tilt = 0f;
			this.Pan = 0f;
			this.Dutch = 0f;
			this.ZoomScale = 1f;
			this.FollowAttachment = 1f;
			this.LookAtAttachment = 1f;
		}

		private void OnValidate()
		{
			this.ZoomScale = Mathf.Max(0.01f, this.ZoomScale);
			this.FollowAttachment = Mathf.Clamp01(this.FollowAttachment);
			this.LookAtAttachment = Mathf.Clamp01(this.LookAtAttachment);
		}

		public override void PrePipelineMutateCameraStateCallback(CinemachineVirtualCameraBase vcam, ref CameraState curState, float deltaTime)
		{
			vcam.FollowTargetAttachment = this.FollowAttachment;
			vcam.LookAtTargetAttachment = this.LookAtAttachment;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == this.ApplyAfter)
			{
				LensSettings lens = state.Lens;
				Quaternion rhs = state.RawOrientation * Quaternion.AngleAxis(this.Tilt, Vector3.right);
				Quaternion rhs2 = Quaternion.AngleAxis(this.Pan, state.ReferenceUp) * rhs;
				state.OrientationCorrection = Quaternion.Inverse(state.GetCorrectedOrientation()) * rhs2;
				lens.Dutch += this.Dutch;
				if (this.ZoomScale != 1f)
				{
					lens.OrthographicSize *= this.ZoomScale;
					lens.FieldOfView *= this.ZoomScale;
				}
				state.Lens = lens;
			}
		}

		[Tooltip("When to apply the adjustment")]
		[FormerlySerializedAs("m_ApplyAfter")]
		public CinemachineCore.Stage ApplyAfter;

		[Tooltip("Tilt the camera by this much")]
		[FormerlySerializedAs("m_Tilt")]
		public float Tilt;

		[Tooltip("Pan the camera by this much")]
		[FormerlySerializedAs("m_Pan")]
		public float Pan;

		[Tooltip("Roll the camera by this much")]
		[FormerlySerializedAs("m_Dutch")]
		public float Dutch;

		[Tooltip("Scale the zoom by this amount (normal = 1)")]
		[FormerlySerializedAs("m_ZoomScale")]
		public float ZoomScale;

		[Range(0f, 1f)]
		[Tooltip("Lowering this value relaxes the camera's attention to the Follow target (normal = 1)")]
		[FormerlySerializedAs("m_FollowAttachment")]
		public float FollowAttachment;

		[Range(0f, 1f)]
		[Tooltip("Lowering this value relaxes the camera's attention to the LookAt target (normal = 1)")]
		[FormerlySerializedAs("m_LookAtAttachment")]
		public float LookAtAttachment;
	}
}
