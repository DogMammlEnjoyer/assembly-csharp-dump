using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	public abstract class CinemachineComponentBase : MonoBehaviour
	{
		public CinemachineVirtualCameraBase VirtualCamera
		{
			get
			{
				if (this.m_VcamOwner == null)
				{
					base.TryGetComponent<CinemachineVirtualCameraBase>(out this.m_VcamOwner);
				}
				if (this.m_VcamOwner == null && base.transform.parent != null)
				{
					base.transform.parent.TryGetComponent<CinemachineVirtualCameraBase>(out this.m_VcamOwner);
				}
				return this.m_VcamOwner;
			}
		}

		protected virtual void OnEnable()
		{
			CinemachineCamera cinemachineCamera = this.VirtualCamera as CinemachineCamera;
			if (cinemachineCamera != null)
			{
				cinemachineCamera.InvalidatePipelineCache();
			}
		}

		protected virtual void OnDisable()
		{
			CinemachineCamera cinemachineCamera = this.VirtualCamera as CinemachineCamera;
			if (cinemachineCamera != null)
			{
				cinemachineCamera.InvalidatePipelineCache();
			}
		}

		public Transform FollowTarget
		{
			get
			{
				CinemachineVirtualCameraBase virtualCamera = this.VirtualCamera;
				if (!(virtualCamera == null))
				{
					return virtualCamera.ResolveFollow(virtualCamera.Follow);
				}
				return null;
			}
		}

		public Transform LookAtTarget
		{
			get
			{
				CinemachineVirtualCameraBase virtualCamera = this.VirtualCamera;
				if (!(virtualCamera == null))
				{
					return virtualCamera.ResolveLookAt(virtualCamera.LookAt);
				}
				return null;
			}
		}

		public ICinemachineTargetGroup FollowTargetAsGroup
		{
			get
			{
				CinemachineVirtualCameraBase virtualCamera = this.VirtualCamera;
				if (!(virtualCamera == null))
				{
					return virtualCamera.FollowTargetAsGroup;
				}
				return null;
			}
		}

		public Vector3 FollowTargetPosition
		{
			get
			{
				CinemachineVirtualCameraBase followTargetAsVcam = this.VirtualCamera.FollowTargetAsVcam;
				if (followTargetAsVcam != null)
				{
					return followTargetAsVcam.State.GetFinalPosition();
				}
				Transform followTarget = this.FollowTarget;
				if (followTarget != null)
				{
					return TargetPositionCache.GetTargetPosition(followTarget);
				}
				return Vector3.zero;
			}
		}

		public Quaternion FollowTargetRotation
		{
			get
			{
				CinemachineVirtualCameraBase followTargetAsVcam = this.VirtualCamera.FollowTargetAsVcam;
				if (followTargetAsVcam != null)
				{
					return followTargetAsVcam.State.GetFinalOrientation();
				}
				Transform followTarget = this.FollowTarget;
				if (followTarget != null)
				{
					return TargetPositionCache.GetTargetRotation(followTarget);
				}
				return Quaternion.identity;
			}
		}

		public ICinemachineTargetGroup LookAtTargetAsGroup
		{
			get
			{
				return this.VirtualCamera.LookAtTargetAsGroup;
			}
		}

		public Vector3 LookAtTargetPosition
		{
			get
			{
				CinemachineVirtualCameraBase lookAtTargetAsVcam = this.VirtualCamera.LookAtTargetAsVcam;
				if (lookAtTargetAsVcam != null)
				{
					return lookAtTargetAsVcam.State.GetFinalPosition();
				}
				Transform lookAtTarget = this.LookAtTarget;
				if (lookAtTarget != null)
				{
					return TargetPositionCache.GetTargetPosition(lookAtTarget);
				}
				return Vector3.zero;
			}
		}

		public Quaternion LookAtTargetRotation
		{
			get
			{
				CinemachineVirtualCameraBase lookAtTargetAsVcam = this.VirtualCamera.LookAtTargetAsVcam;
				if (lookAtTargetAsVcam != null)
				{
					return lookAtTargetAsVcam.State.GetFinalOrientation();
				}
				Transform lookAtTarget = this.LookAtTarget;
				if (lookAtTarget != null)
				{
					return TargetPositionCache.GetTargetRotation(lookAtTarget);
				}
				return Quaternion.identity;
			}
		}

		public CameraState VcamState
		{
			get
			{
				CinemachineVirtualCameraBase virtualCamera = this.VirtualCamera;
				if (!(virtualCamera == null))
				{
					return virtualCamera.State;
				}
				return CameraState.Default;
			}
		}

		public abstract bool IsValid { get; }

		public virtual void PrePipelineMutateCameraState(ref CameraState curState, float deltaTime)
		{
		}

		public abstract CinemachineCore.Stage Stage { get; }

		public virtual bool BodyAppliesAfterAim
		{
			get
			{
				return false;
			}
		}

		public abstract void MutateCameraState(ref CameraState curState, float deltaTime);

		public virtual bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			return false;
		}

		public virtual void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
		}

		public virtual void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
		}

		public virtual float GetMaxDampTime()
		{
			return 0f;
		}

		internal virtual bool CameraLooksAtTarget
		{
			get
			{
				return false;
			}
		}

		protected const float Epsilon = 0.0001f;

		private CinemachineVirtualCameraBase m_VcamOwner;
	}
}
