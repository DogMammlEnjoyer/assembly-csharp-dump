using System;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineTransposer has been deprecated. Use CinemachineFollow instead")]
	[AddComponentMenu("")]
	[SaveDuringPlay]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	public class CinemachineTransposer : CinemachineComponentBase
	{
		protected TrackerSettings TrackerSettings
		{
			get
			{
				return new TrackerSettings
				{
					BindingMode = this.m_BindingMode,
					PositionDamping = new Vector3(this.m_XDamping, this.m_YDamping, this.m_ZDamping),
					RotationDamping = new Vector3(this.m_PitchDamping, this.m_YawDamping, this.m_RollDamping),
					AngularDampingMode = this.m_AngularDampingMode,
					QuaternionDamping = this.m_AngularDamping
				};
			}
		}

		protected virtual void OnValidate()
		{
			this.m_FollowOffset = this.EffectiveOffset;
		}

		internal bool HideOffsetInInspector { get; set; }

		public Vector3 EffectiveOffset
		{
			get
			{
				Vector3 followOffset = this.m_FollowOffset;
				if (this.m_BindingMode == BindingMode.LazyFollow)
				{
					followOffset.x = 0f;
					followOffset.z = -Mathf.Abs(followOffset.z);
				}
				return followOffset;
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled && base.FollowTarget != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Body;
			}
		}

		public override float GetMaxDampTime()
		{
			return this.TrackerSettings.GetMaxDampTime();
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			this.m_TargetTracker.InitStateInfo(this, deltaTime, this.m_BindingMode, curState.ReferenceUp);
			if (this.IsValid)
			{
				Vector3 vector = this.EffectiveOffset;
				Vector3 referenceUp = curState.ReferenceUp;
				Vector3 desiredCameraOffset = vector;
				TrackerSettings trackerSettings = this.TrackerSettings;
				Vector3 vector2;
				Quaternion rotation;
				this.m_TargetTracker.TrackTarget(this, deltaTime, referenceUp, desiredCameraOffset, trackerSettings, ref curState, out vector2, out rotation);
				vector = rotation * vector;
				curState.ReferenceUp = rotation * Vector3.up;
				Vector3 followTargetPosition = base.FollowTargetPosition;
				vector2 += this.m_TargetTracker.GetOffsetForMinimumTargetDistance(this, vector2, vector, curState.RawOrientation * Vector3.forward, curState.ReferenceUp, followTargetPosition);
				curState.RawPosition = vector2 + vector;
			}
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			base.OnTargetObjectWarped(target, positionDelta);
			if (target == base.FollowTarget)
			{
				this.m_TargetTracker.OnTargetObjectWarped(positionDelta);
			}
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			base.ForceCameraPosition(pos, rot);
			CameraState vcamState = base.VcamState;
			vcamState.RawPosition = pos;
			vcamState.RawOrientation = rot;
			vcamState.PositionCorrection = Vector3.zero;
			vcamState.OrientationCorrection = Quaternion.identity;
			this.m_TargetTracker.OnForceCameraPosition(this, this.m_BindingMode, ref vcamState);
		}

		internal Quaternion GetReferenceOrientation(Vector3 up)
		{
			CameraState vcamState = base.VcamState;
			return this.m_TargetTracker.GetReferenceOrientation(this, this.m_BindingMode, up, ref vcamState);
		}

		internal virtual Vector3 GetTargetCameraPosition(Vector3 worldUp)
		{
			if (!this.IsValid)
			{
				return Vector3.zero;
			}
			CameraState vcamState = base.VcamState;
			return base.FollowTargetPosition + this.m_TargetTracker.GetReferenceOrientation(this, this.m_BindingMode, worldUp, ref vcamState) * this.EffectiveOffset;
		}

		internal void UpgradeToCm3(CinemachineFollow c)
		{
			c.FollowOffset = this.m_FollowOffset;
			c.TrackerSettings = this.TrackerSettings;
		}

		[Tooltip("The coordinate space to use when interpreting the offset from the target.  This is also used to set the camera's Up vector, which will be maintained when aiming the camera.")]
		public BindingMode m_BindingMode = BindingMode.LockToTargetWithWorldUp;

		[Tooltip("The distance vector that the transposer will attempt to maintain from the Follow target")]
		public Vector3 m_FollowOffset = Vector3.back * 10f;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain the offset in the X-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's x-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_XDamping = 1f;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain the offset in the Y-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's y-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_YDamping = 1f;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to maintain the offset in the Z-axis.  Small numbers are more responsive, rapidly translating the camera to keep the target's z-axis offset.  Larger numbers give a more heavy slowly responding camera. Using different settings per axis can yield a wide range of camera behaviors.")]
		public float m_ZDamping = 1f;

		public AngularDampingMode m_AngularDampingMode;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target rotation's X angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float m_PitchDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target rotation's Y angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float m_YawDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target rotation's Z angle.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float m_RollDamping;

		[Range(0f, 20f)]
		[Tooltip("How aggressively the camera tries to track the target's orientation.  Small numbers are more responsive.  Larger numbers give a more heavy slowly responding camera.")]
		public float m_AngularDamping;

		private Tracker m_TargetTracker;
	}
}
