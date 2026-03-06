using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine
{
	public abstract class CinemachineExtension : MonoBehaviour
	{
		public CinemachineVirtualCameraBase ComponentOwner
		{
			get
			{
				if (this.m_VcamOwner == null)
				{
					base.TryGetComponent<CinemachineVirtualCameraBase>(out this.m_VcamOwner);
				}
				return this.m_VcamOwner;
			}
		}

		protected virtual void Awake()
		{
			this.ConnectToVcam(true);
		}

		protected virtual void OnDestroy()
		{
			this.ConnectToVcam(false);
		}

		protected virtual void OnEnable()
		{
		}

		internal void EnsureStarted()
		{
			this.ConnectToVcam(true);
		}

		protected virtual void ConnectToVcam(bool connect)
		{
			if (this.ComponentOwner != null)
			{
				if (connect)
				{
					this.ComponentOwner.AddExtension(this);
				}
				else
				{
					this.ComponentOwner.RemoveExtension(this);
				}
			}
			this.m_ExtraState = null;
		}

		public virtual void PrePipelineMutateCameraStateCallback(CinemachineVirtualCameraBase vcam, ref CameraState curState, float deltaTime)
		{
		}

		public void InvokePostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			this.PostPipelineStageCallback(vcam, stage, ref state, deltaTime);
		}

		protected virtual void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
		}

		public virtual void OnTargetObjectWarped(CinemachineVirtualCameraBase vcam, Transform target, Vector3 positionDelta)
		{
		}

		public virtual void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
		}

		public virtual void ForceCameraPosition(CinemachineVirtualCameraBase vcam, Vector3 pos, Quaternion rot)
		{
		}

		public virtual bool OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			return false;
		}

		public virtual float GetMaxDampTime()
		{
			return 0f;
		}

		protected T GetExtraState<T>(CinemachineVirtualCameraBase vcam) where T : CinemachineExtension.VcamExtraStateBase, new()
		{
			if (this.m_ExtraState == null)
			{
				this.m_ExtraState = new Dictionary<CinemachineVirtualCameraBase, CinemachineExtension.VcamExtraStateBase>();
			}
			CinemachineExtension.VcamExtraStateBase vcamExtraStateBase;
			if (!this.m_ExtraState.TryGetValue(vcam, out vcamExtraStateBase))
			{
				Dictionary<CinemachineVirtualCameraBase, CinemachineExtension.VcamExtraStateBase> extraState = this.m_ExtraState;
				T t = Activator.CreateInstance<T>();
				t.Vcam = vcam;
				vcamExtraStateBase = (extraState[vcam] = t);
			}
			return vcamExtraStateBase as T;
		}

		protected void GetAllExtraStates<T>(List<T> list) where T : CinemachineExtension.VcamExtraStateBase, new()
		{
			list.Clear();
			if (this.m_ExtraState != null)
			{
				foreach (KeyValuePair<CinemachineVirtualCameraBase, CinemachineExtension.VcamExtraStateBase> keyValuePair in this.m_ExtraState)
				{
					list.Add(keyValuePair.Value as T);
				}
			}
		}

		private CinemachineVirtualCameraBase m_VcamOwner;

		private Dictionary<CinemachineVirtualCameraBase, CinemachineExtension.VcamExtraStateBase> m_ExtraState;

		protected const float Epsilon = 0.0001f;

		protected class VcamExtraStateBase
		{
			public CinemachineVirtualCameraBase Vcam;
		}
	}
}
