using System;
using UnityEngine;

namespace Unity.Cinemachine.TargetTracking
{
	internal struct Tracker
	{
		public Vector3 PreviousTargetPosition { readonly get; private set; }

		public Quaternion PreviousReferenceOrientation { readonly get; private set; }

		public void InitStateInfo(CinemachineComponentBase component, float deltaTime, BindingMode bindingMode, Vector3 up)
		{
			bool flag = deltaTime >= 0f && component.VirtualCamera.PreviousStateIsValid;
			if (this.m_PreviousTarget != component.FollowTarget || !flag)
			{
				this.m_PreviousTarget = component.FollowTarget;
				this.m_TargetOrientationOnAssign = component.FollowTargetRotation;
			}
			if (!flag)
			{
				this.PreviousTargetPosition = component.FollowTargetPosition;
				CameraState vcamState = component.VcamState;
				this.PreviousReferenceOrientation = this.GetReferenceOrientation(component, bindingMode, up, ref vcamState);
			}
		}

		public Quaternion GetReferenceOrientation(CinemachineComponentBase component, BindingMode bindingMode, Vector3 worldUp, ref CameraState cameraState)
		{
			if (bindingMode == BindingMode.WorldSpace)
			{
				return Quaternion.identity;
			}
			if (component.FollowTarget != null)
			{
				Quaternion followTargetRotation = component.FollowTargetRotation;
				switch (bindingMode)
				{
				case BindingMode.LockToTargetOnAssign:
					return this.m_TargetOrientationOnAssign;
				case BindingMode.LockToTargetWithWorldUp:
				{
					Vector3 vector = (followTargetRotation * Vector3.forward).ProjectOntoPlane(worldUp);
					if (!vector.AlmostZero())
					{
						return Quaternion.LookRotation(vector, worldUp);
					}
					break;
				}
				case BindingMode.LockToTargetNoRoll:
					return Quaternion.LookRotation(followTargetRotation * Vector3.forward, worldUp);
				case BindingMode.LockToTarget:
					return followTargetRotation;
				case BindingMode.LazyFollow:
				{
					Vector3 vector2 = (component.FollowTargetPosition - cameraState.RawPosition).ProjectOntoPlane(worldUp);
					if (!vector2.AlmostZero())
					{
						return Quaternion.LookRotation(vector2, worldUp);
					}
					break;
				}
				}
			}
			if (this.PreviousReferenceOrientation == new Quaternion(0f, 0f, 0f, 0f))
			{
				return Quaternion.identity;
			}
			return this.PreviousReferenceOrientation.normalized;
		}

		public void TrackTarget(CinemachineComponentBase component, float deltaTime, Vector3 up, Vector3 desiredCameraOffset, in TrackerSettings settings, ref CameraState cameraState, out Vector3 outTargetPosition, out Quaternion outTargetOrient)
		{
			Quaternion referenceOrientation = this.GetReferenceOrientation(component, settings.BindingMode, up, ref cameraState);
			Quaternion quaternion = referenceOrientation;
			bool flag = deltaTime >= 0f && component.VirtualCamera.PreviousStateIsValid;
			if (flag)
			{
				if (settings.AngularDampingMode == AngularDampingMode.Quaternion && settings.BindingMode == BindingMode.LockToTarget)
				{
					float t = component.VirtualCamera.DetachedFollowTargetDamp(1f, settings.QuaternionDamping, deltaTime);
					quaternion = Quaternion.Slerp(this.PreviousReferenceOrientation, referenceOrientation, t);
				}
				else if (settings.BindingMode != BindingMode.LazyFollow)
				{
					Vector3 vector = (Quaternion.Inverse(this.PreviousReferenceOrientation) * referenceOrientation).eulerAngles;
					for (int i = 0; i < 3; i++)
					{
						if (vector[i] > 180f)
						{
							ref Vector3 ptr = ref vector;
							int index = i;
							ptr[index] -= 360f;
						}
						if (Mathf.Abs(vector[i]) < 0.01f)
						{
							vector[i] = 0f;
						}
					}
					vector = component.VirtualCamera.DetachedFollowTargetDamp(vector, settings.GetEffectiveRotationDamping(), deltaTime);
					quaternion = this.PreviousReferenceOrientation * Quaternion.Euler(vector);
				}
			}
			this.PreviousReferenceOrientation = quaternion;
			Vector3 followTargetPosition = component.FollowTargetPosition;
			Vector3 vector2 = this.PreviousTargetPosition;
			Vector3 b = flag ? this.m_PreviousOffset : desiredCameraOffset;
			if ((desiredCameraOffset - b).sqrMagnitude > 0.01f)
			{
				Quaternion rotation = UnityVectorExtensions.SafeFromToRotation(this.m_PreviousOffset, desiredCameraOffset, up);
				vector2 = followTargetPosition + rotation * (this.PreviousTargetPosition - followTargetPosition);
			}
			this.m_PreviousOffset = desiredCameraOffset;
			Vector3 vector3 = followTargetPosition - vector2;
			if (flag)
			{
				Quaternion rotation2 = desiredCameraOffset.AlmostZero() ? component.VcamState.RawOrientation : Quaternion.LookRotation(quaternion * desiredCameraOffset, up);
				Vector3 vector4 = Quaternion.Inverse(rotation2) * vector3;
				vector4 = component.VirtualCamera.DetachedFollowTargetDamp(vector4, settings.GetEffectivePositionDamping(), deltaTime);
				vector3 = rotation2 * vector4;
			}
			vector2 += vector3;
			outTargetPosition = (this.PreviousTargetPosition = vector2);
			outTargetOrient = quaternion;
		}

		public Vector3 GetOffsetForMinimumTargetDistance(CinemachineComponentBase component, Vector3 dampedTargetPos, Vector3 cameraOffset, Vector3 cameraFwd, Vector3 up, Vector3 actualTargetPos)
		{
			Vector3 vector = Vector3.zero;
			if (component.VirtualCamera.FollowTargetAttachment > 0.9999f)
			{
				cameraOffset = cameraOffset.ProjectOntoPlane(up);
				float num = cameraOffset.magnitude * 0.2f;
				if (num > 0f)
				{
					actualTargetPos = actualTargetPos.ProjectOntoPlane(up);
					dampedTargetPos = dampedTargetPos.ProjectOntoPlane(up);
					Vector3 b = dampedTargetPos + cameraOffset;
					float num2 = Vector3.Dot(actualTargetPos - b, (dampedTargetPos - b).normalized);
					if (num2 < num)
					{
						Vector3 a = actualTargetPos - dampedTargetPos;
						float magnitude = a.magnitude;
						if (magnitude < 0.01f)
						{
							a = -cameraFwd.ProjectOntoPlane(up);
						}
						else
						{
							a /= magnitude;
						}
						vector = a * (num - num2);
					}
					this.PreviousTargetPosition += vector;
				}
			}
			return vector;
		}

		public void OnTargetObjectWarped(Vector3 positionDelta)
		{
			this.PreviousTargetPosition += positionDelta;
		}

		public void OnForceCameraPosition(CinemachineComponentBase component, BindingMode bindingMode, ref CameraState newState)
		{
			CameraState vcamState = component.VcamState;
			Vector3 vector = Quaternion.Inverse(vcamState.GetFinalOrientation()) * (this.PreviousTargetPosition - vcamState.GetFinalPosition());
			vector = newState.GetFinalOrientation() * vector;
			this.PreviousTargetPosition = newState.GetFinalPosition() + vector;
			this.PreviousReferenceOrientation = this.GetReferenceOrientation(component, bindingMode, newState.ReferenceUp, ref newState);
			this.m_PreviousOffset = -vector;
		}

		private Quaternion m_TargetOrientationOnAssign;

		private Vector3 m_PreviousOffset;

		private Transform m_PreviousTarget;
	}
}
