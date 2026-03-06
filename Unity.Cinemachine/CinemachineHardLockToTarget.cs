using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Position Control/Cinemachine Hard Lock to Target")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.Tracking)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineHardLockToTarget.html")]
	public class CinemachineHardLockToTarget : CinemachineComponentBase
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
				return CinemachineCore.Stage.Body;
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
			Vector3 vector = base.FollowTargetPosition;
			if (base.VirtualCamera.PreviousStateIsValid && deltaTime >= 0f)
			{
				vector = this.m_PreviousTargetPosition + base.VirtualCamera.DetachedFollowTargetDamp(vector - this.m_PreviousTargetPosition, this.Damping, deltaTime);
			}
			this.m_PreviousTargetPosition = vector;
			curState.RawPosition = vector;
		}

		[Tooltip("How much time it takes for the position to catch up to the target's position")]
		[FormerlySerializedAs("m_Damping")]
		public float Damping;

		private Vector3 m_PreviousTargetPosition;
	}
}
