using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Camera Offset")]
	[ExecuteAlways]
	[SaveDuringPlay]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineCameraOffset.html")]
	public class CinemachineCameraOffset : CinemachineExtension
	{
		private void Reset()
		{
			this.Offset = Vector3.zero;
			this.ApplyAfter = CinemachineCore.Stage.Aim;
			this.PreserveComposition = false;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == this.ApplyAfter)
			{
				object obj = this.PreserveComposition && state.HasLookAt() && stage > CinemachineCore.Stage.Body;
				Vector3 a = Vector2.zero;
				object obj2 = obj;
				if (obj2 != null)
				{
					a = state.RawOrientation.GetCameraRotationToTarget(state.ReferenceLookAt - state.GetCorrectedPosition(), state.ReferenceUp);
				}
				Vector3 b = state.RawOrientation * this.Offset;
				state.PositionCorrection += b;
				if (obj2 == null)
				{
					state.ReferenceLookAt += b;
					return;
				}
				Quaternion quaternion = Quaternion.LookRotation(state.ReferenceLookAt - state.GetCorrectedPosition(), state.ReferenceUp);
				quaternion = quaternion.ApplyCameraRotation(-a, state.ReferenceUp);
				state.RawOrientation = quaternion;
			}
		}

		[Tooltip("Offset the camera's position by this much (camera space)")]
		[FormerlySerializedAs("m_Offset")]
		public Vector3 Offset = Vector3.zero;

		[Tooltip("When to apply the offset")]
		[FormerlySerializedAs("m_ApplyAfter")]
		public CinemachineCore.Stage ApplyAfter = CinemachineCore.Stage.Aim;

		[Tooltip("If applying offset after aim, re-adjust the aim to preserve the screen position of the LookAt target as much as possible")]
		[FormerlySerializedAs("m_PreserveComposition")]
		public bool PreserveComposition;
	}
}
