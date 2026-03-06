using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Rotation Control/Cinemachine Third Person Aim")]
	[ExecuteAlways]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineThirdPersonAim.html")]
	public class CinemachineThirdPersonAim : CinemachineExtension
	{
		public Vector3 AimTarget { get; private set; }

		private void OnValidate()
		{
			this.AimDistance = Mathf.Max(1f, this.AimDistance);
		}

		private void Reset()
		{
			this.AimCollisionFilter = 1;
			this.IgnoreTag = string.Empty;
			this.AimDistance = 200f;
			this.NoiseCancellation = true;
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage != CinemachineCore.Stage.Body)
			{
				if (stage != CinemachineCore.Stage.Finalize)
				{
					return;
				}
				if (this.NoiseCancellation)
				{
					Vector3 forward = state.ReferenceLookAt - state.GetFinalPosition();
					if (forward.sqrMagnitude > 0.01f)
					{
						state.RawOrientation = Quaternion.LookRotation(forward, state.ReferenceUp);
						state.OrientationCorrection = Quaternion.identity;
						return;
					}
				}
				else
				{
					Transform follow = vcam.Follow;
					if (follow != null)
					{
						state.ReferenceLookAt = this.ComputeLookAtPoint(state.GetCorrectedPosition(), follow, state.GetCorrectedOrientation() * Vector3.forward);
						this.AimTarget = this.ComputeAimTarget(state.ReferenceLookAt, follow);
					}
				}
			}
			else if (this.NoiseCancellation)
			{
				Transform follow2 = vcam.Follow;
				if (follow2 != null)
				{
					state.ReferenceLookAt = this.ComputeLookAtPoint(state.GetCorrectedPosition(), follow2, follow2.forward);
					this.AimTarget = this.ComputeAimTarget(state.ReferenceLookAt, follow2);
					return;
				}
			}
		}

		private Vector3 ComputeLookAtPoint(Vector3 camPos, Transform player, Vector3 fwd)
		{
			float num = this.AimDistance;
			Vector3 vector = Quaternion.Inverse(player.rotation) * (player.position - camPos);
			if (vector.z > 0f)
			{
				camPos += fwd * vector.z;
				num -= vector.z;
			}
			num = Mathf.Max(1f, num);
			RaycastHit raycastHit;
			if (!RuntimeUtility.RaycastIgnoreTag(new Ray(camPos, fwd), out raycastHit, num, this.AimCollisionFilter, this.IgnoreTag))
			{
				return camPos + fwd * num;
			}
			return raycastHit.point;
		}

		private Vector3 ComputeAimTarget(Vector3 cameraLookAt, Transform player)
		{
			Vector3 position = player.position;
			Vector3 direction = cameraLookAt - position;
			RaycastHit raycastHit;
			if (RuntimeUtility.RaycastIgnoreTag(new Ray(position, direction), out raycastHit, direction.magnitude, this.AimCollisionFilter, this.IgnoreTag))
			{
				return raycastHit.point;
			}
			return cameraLookAt;
		}

		[Header("Aim Target Detection")]
		[Tooltip("Objects on these layers will be detected")]
		public LayerMask AimCollisionFilter;

		[TagField]
		[Tooltip("Objects with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
		public string IgnoreTag = string.Empty;

		[Tooltip("How far to project the object detection ray")]
		public float AimDistance;

		[Tooltip("If set, camera noise will be adjusted to stabilize target on screen")]
		public bool NoiseCancellation = true;
	}
}
