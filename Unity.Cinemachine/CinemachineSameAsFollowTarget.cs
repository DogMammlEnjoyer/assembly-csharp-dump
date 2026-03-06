using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineSameAsFollowTarget has been deprecated. Use CinemachineRotateWithFollowTarget instead")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineSameAsFollowTarget.html")]
	public class CinemachineSameAsFollowTarget : CinemachineComponentBase
	{
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
				return CinemachineCore.Stage.Aim;
			}
		}

		public override float GetMaxDampTime()
		{
			return this.Damping;
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (!this.IsValid)
			{
				return;
			}
			Quaternion quaternion = base.FollowTargetRotation;
			if (deltaTime >= 0f)
			{
				float t = base.VirtualCamera.DetachedFollowTargetDamp(1f, this.Damping, deltaTime);
				quaternion = Quaternion.Slerp(this.m_PreviousReferenceOrientation, base.FollowTargetRotation, t);
			}
			this.m_PreviousReferenceOrientation = quaternion;
			curState.RawOrientation = quaternion;
			curState.ReferenceUp = quaternion * Vector3.up;
		}

		internal void UpgradeToCm3(CinemachineRotateWithFollowTarget c)
		{
			c.Damping = this.Damping;
		}

		[Tooltip("How much time it takes for the aim to catch up to the target's rotation")]
		[FormerlySerializedAs("m_AngularDamping")]
		[FormerlySerializedAs("m_Damping")]
		public float Damping;

		private Quaternion m_PreviousReferenceOrientation = Quaternion.identity;
	}
}
