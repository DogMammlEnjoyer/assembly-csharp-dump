using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Hard Look At")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Aim)]
	[RequiredTarget(RequiredTargetAttribute.RequiredTargets.LookAt)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineHardLookAt.html")]
	public class CinemachineHardLookAt : CinemachineComponentBase
	{
		public override bool IsValid
		{
			get
			{
				return base.enabled && base.LookAtTarget != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Aim;
			}
		}

		internal override bool CameraLooksAtTarget
		{
			get
			{
				return true;
			}
		}

		private void Reset()
		{
			this.LookAtOffset = Vector3.zero;
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (this.IsValid && curState.HasLookAt())
			{
				Vector3 b = base.LookAtTargetRotation * this.LookAtOffset;
				Vector3 vector = curState.ReferenceLookAt + b - curState.GetCorrectedPosition();
				if (vector.magnitude > 0.0001f)
				{
					if (Vector3.Cross(vector.normalized, curState.ReferenceUp).magnitude < 0.0001f)
					{
						curState.RawOrientation = Quaternion.FromToRotation(Vector3.forward, vector);
						return;
					}
					curState.RawOrientation = Quaternion.LookRotation(vector, curState.ReferenceUp);
				}
			}
		}

		[Tooltip("Offset from the LookAt target's origin, in target's local space.  The camera will look at this point.")]
		public Vector3 LookAtOffset = Vector3.zero;
	}
}
