using System;
using Unity.Cinemachine.TargetTracking;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Position Control/Cinemachine Follow")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineFollow.html")]
	public class CinemachineFollow : CinemachineComponentBase
	{
		private void OnValidate()
		{
			this.FollowOffset = this.EffectiveOffset;
			this.TrackerSettings.Validate();
		}

		private void Reset()
		{
			this.FollowOffset = Vector3.back * 10f;
			this.TrackerSettings = TrackerSettings.Default;
		}

		internal Vector3 EffectiveOffset
		{
			get
			{
				Vector3 followOffset = this.FollowOffset;
				if (this.TrackerSettings.BindingMode == BindingMode.LazyFollow)
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
			this.m_TargetTracker.InitStateInfo(this, deltaTime, this.TrackerSettings.BindingMode, curState.ReferenceUp);
			if (this.IsValid)
			{
				Vector3 vector = this.EffectiveOffset;
				Vector3 vector2;
				Quaternion rotation;
				this.m_TargetTracker.TrackTarget(this, deltaTime, curState.ReferenceUp, vector, this.TrackerSettings, ref curState, out vector2, out rotation);
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
			this.m_TargetTracker.OnForceCameraPosition(this, this.TrackerSettings.BindingMode, ref vcamState);
		}

		internal Quaternion GetReferenceOrientation(Vector3 up)
		{
			CameraState vcamState = base.VcamState;
			return this.m_TargetTracker.GetReferenceOrientation(this, this.TrackerSettings.BindingMode, up, ref vcamState);
		}

		internal Vector3 GetDesiredCameraPosition(Vector3 worldUp)
		{
			if (!this.IsValid)
			{
				return Vector3.zero;
			}
			CameraState vcamState = base.VcamState;
			return base.FollowTargetPosition + this.m_TargetTracker.GetReferenceOrientation(this, this.TrackerSettings.BindingMode, worldUp, ref vcamState) * this.EffectiveOffset;
		}

		public TrackerSettings TrackerSettings = TrackerSettings.Default;

		[Tooltip("The distance vector that the camera will attempt to maintain from the tracking target")]
		public Vector3 FollowOffset = Vector3.back * 10f;

		private Tracker m_TargetTracker;
	}
}
