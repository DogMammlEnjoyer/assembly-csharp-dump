using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Confiner 3D")]
	[SaveDuringPlay]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineConfiner3D.html")]
	public class CinemachineConfiner3D : CinemachineExtension
	{
		public bool CameraWasDisplaced(CinemachineVirtualCameraBase vcam)
		{
			return this.GetCameraDisplacementDistance(vcam) > 0f;
		}

		public float GetCameraDisplacementDistance(CinemachineVirtualCameraBase vcam)
		{
			return base.GetExtraState<CinemachineConfiner3D.VcamExtraState>(vcam).PreviousDisplacement.magnitude;
		}

		private void Reset()
		{
			this.BoundingVolume = null;
			this.SlowingDistance = 0f;
		}

		private void OnValidate()
		{
			this.SlowingDistance = Mathf.Max(0f, this.SlowingDistance);
		}

		public bool IsValid
		{
			get
			{
				return this.BoundingVolume != null && this.BoundingVolume.enabled && this.BoundingVolume.gameObject.activeInHierarchy;
			}
		}

		public override float GetMaxDampTime()
		{
			return this.SlowingDistance * 0.2f;
		}

		public override void OnTargetObjectWarped(CinemachineVirtualCameraBase vcam, Transform target, Vector3 positionDelta)
		{
			CinemachineConfiner3D.VcamExtraState extraState = base.GetExtraState<CinemachineConfiner3D.VcamExtraState>(vcam);
			if (extraState.Vcam.Follow == target)
			{
				extraState.PreviousCameraPosition += positionDelta;
			}
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Body && this.IsValid)
			{
				CinemachineConfiner3D.VcamExtraState extraState = base.GetExtraState<CinemachineConfiner3D.VcamExtraState>(vcam);
				Vector3 correctedPosition = state.GetCorrectedPosition();
				Vector3 vector = this.ConfinePoint(correctedPosition);
				if (this.SlowingDistance > 0.0001f && deltaTime >= 0f && vcam.PreviousStateIsValid)
				{
					Vector3 previousCameraPosition = extraState.PreviousCameraPosition;
					Vector3 a = vector - previousCameraPosition;
					float magnitude = a.magnitude;
					if (magnitude > 0.0001f)
					{
						float num = this.GetDistanceFromEdge(previousCameraPosition, a / magnitude, this.SlowingDistance) / this.SlowingDistance;
						vector = Vector3.Lerp(previousCameraPosition, vector, num * num * num + 0.05f);
					}
				}
				Vector3 vector2 = vector - correctedPosition;
				state.PositionCorrection += vector2;
				extraState.PreviousCameraPosition = state.GetCorrectedPosition();
				extraState.PreviousDisplacement = vector2;
			}
		}

		private Vector3 ConfinePoint(Vector3 p)
		{
			MeshCollider meshCollider = this.BoundingVolume as MeshCollider;
			if (meshCollider != null && !meshCollider.convex)
			{
				return p;
			}
			return this.BoundingVolume.ClosestPoint(p);
		}

		private float GetDistanceFromEdge(Vector3 p, Vector3 dirUnit, float max)
		{
			p += dirUnit * max;
			return max - (this.ConfinePoint(p) - p).magnitude;
		}

		[Tooltip("The volume within which the camera is to be contained")]
		public Collider BoundingVolume;

		[Tooltip("Size of the slow-down zone at the edge of the bounding volume.")]
		public float SlowingDistance;

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public Vector3 PreviousDisplacement;

			public Vector3 PreviousCameraPosition;
		}
	}
}
