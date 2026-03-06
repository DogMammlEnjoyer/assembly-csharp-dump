using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[RequireComponent(typeof(Camera))]
	[DisallowMultipleComponent]
	[AddComponentMenu("Cinemachine/Cinemachine External Camera")]
	[ExecuteAlways]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineExternalCamera.html")]
	public class CinemachineExternalCamera : CinemachineVirtualCameraBase
	{
		public override CameraState State
		{
			get
			{
				return this.m_State;
			}
		}

		public override Transform LookAt
		{
			get
			{
				return this.LookAtTarget;
			}
			set
			{
				this.LookAtTarget = value;
			}
		}

		[HideInInspector]
		public override Transform Follow { get; set; }

		public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			if (this.m_Camera == null)
			{
				base.TryGetComponent<Camera>(out this.m_Camera);
			}
			this.m_State = CameraState.Default;
			this.m_State.RawPosition = base.transform.position;
			this.m_State.RawOrientation = base.transform.rotation;
			this.m_State.ReferenceUp = this.m_State.RawOrientation * Vector3.up;
			if (this.m_Camera != null)
			{
				this.m_State.Lens = LensSettings.FromCamera(this.m_Camera);
			}
			if (this.LookAtTarget != null)
			{
				this.m_State.ReferenceLookAt = this.LookAtTarget.transform.position;
				Vector3 vector = this.m_State.ReferenceLookAt - this.State.RawPosition;
				if (!vector.AlmostZero())
				{
					this.m_State.ReferenceLookAt = this.m_State.RawPosition + Vector3.Project(vector, this.State.RawOrientation * Vector3.forward);
				}
			}
			this.m_State.BlendHint = (CameraState.BlendHints)this.BlendHint;
			base.InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref this.m_State, deltaTime);
		}

		[Tooltip("Hint for transitioning to and from this CinemachineCamera.  Hints can be combined, although not all combinations make sense.  In the case of conflicting hints, Cinemachine will make an arbitrary choice.")]
		[FormerlySerializedAs("m_PositionBlending")]
		[FormerlySerializedAs("m_BlendHint")]
		public CinemachineCore.BlendHints BlendHint;

		[Tooltip("The object that the camera is looking at.  Setting this may improve the quality of the blends to and from this camera")]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_LookAt")]
		public Transform LookAtTarget;

		private Camera m_Camera;

		private CameraState m_State = CameraState.Default;
	}
}
