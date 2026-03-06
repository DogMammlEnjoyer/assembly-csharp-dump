using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	public abstract class CinemachineCameraManagerBase : CinemachineVirtualCameraBase, ICinemachineMixer, ICinemachineCamera
	{
		protected virtual void Reset()
		{
			this.Priority = default(PrioritySettings);
			this.OutputChannel = OutputChannels.Default;
			this.DefaultTarget = default(CinemachineCameraManagerBase.DefaultTargetSettings);
			this.InvalidateCameraCache();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_BlendManager.OnEnable();
			this.m_BlendManager.LookupBlendDelegate = new CinemachineBlendDefinition.LookupBlendDelegate(this.LookupBlend);
			this.InvalidateCameraCache();
		}

		protected override void OnDisable()
		{
			this.m_BlendManager.OnDisable();
			base.OnDisable();
		}

		public override string Description
		{
			get
			{
				return this.m_BlendManager.Description;
			}
		}

		public override CameraState State
		{
			get
			{
				return this.m_State;
			}
		}

		public virtual bool IsLiveChild(ICinemachineCamera cam, bool dominantChildOnly = false)
		{
			return this.m_BlendManager.IsLive(cam);
		}

		public List<CinemachineVirtualCameraBase> ChildCameras
		{
			get
			{
				this.UpdateCameraCache();
				return this.m_ChildCameras;
			}
		}

		public override bool PreviousStateIsValid
		{
			get
			{
				return base.PreviousStateIsValid;
			}
			set
			{
				base.PreviousStateIsValid = value;
				if (!value)
				{
					int num = 0;
					while (this.m_ChildCameras != null && num < this.m_ChildCameras.Count)
					{
						this.m_ChildCameras[num].PreviousStateIsValid = value;
						num++;
					}
				}
			}
		}

		public bool IsBlending
		{
			get
			{
				return this.m_BlendManager.IsBlending;
			}
		}

		public CinemachineBlend ActiveBlend
		{
			get
			{
				if (!this.PreviousStateIsValid)
				{
					return null;
				}
				return this.m_BlendManager.ActiveBlend;
			}
			set
			{
				this.m_BlendManager.ActiveBlend = value;
			}
		}

		public ICinemachineCamera LiveChild
		{
			get
			{
				if (!this.PreviousStateIsValid)
				{
					return null;
				}
				return this.m_BlendManager.ActiveVirtualCamera;
			}
		}

		public override Transform LookAt
		{
			get
			{
				if (!this.DefaultTarget.Enabled)
				{
					return null;
				}
				return base.ResolveLookAt(this.DefaultTarget.Target.CustomLookAtTarget ? this.DefaultTarget.Target.LookAtTarget : this.DefaultTarget.Target.TrackingTarget);
			}
			set
			{
				this.DefaultTarget.Enabled = true;
				this.DefaultTarget.Target.CustomLookAtTarget = true;
				this.DefaultTarget.Target.LookAtTarget = value;
			}
		}

		public override Transform Follow
		{
			get
			{
				if (!this.DefaultTarget.Enabled)
				{
					return null;
				}
				return base.ResolveFollow(this.DefaultTarget.Target.TrackingTarget);
			}
			set
			{
				this.DefaultTarget.Enabled = true;
				this.DefaultTarget.Target.TrackingTarget = value;
			}
		}

		public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			base.UpdateTargetCache();
			this.UpdateCameraCache();
			if (!this.PreviousStateIsValid)
			{
				this.ResetLiveChild();
			}
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = this.ChooseCurrentCamera(worldUp, deltaTime);
			if (cinemachineVirtualCameraBase != null && !cinemachineVirtualCameraBase.gameObject.activeInHierarchy)
			{
				cinemachineVirtualCameraBase.gameObject.SetActive(true);
				cinemachineVirtualCameraBase.UpdateCameraState(worldUp, deltaTime);
			}
			this.SetLiveChild(cinemachineVirtualCameraBase, worldUp, deltaTime);
			if (this.m_TransitioningFrom != null && !this.IsBlending && this.LiveChild != null)
			{
				this.LiveChild.OnCameraActivated(new ICinemachineCamera.ActivationEventParams
				{
					Origin = this,
					OutgoingCamera = this.m_TransitioningFrom,
					IncomingCamera = this.LiveChild,
					IsCut = false,
					WorldUp = worldUp,
					DeltaTime = deltaTime
				});
			}
			this.FinalizeCameraState(deltaTime);
			this.m_TransitioningFrom = null;
			this.PreviousStateIsValid = true;
		}

		protected virtual CinemachineBlendDefinition LookupBlend(ICinemachineCamera outgoing, ICinemachineCamera incoming)
		{
			return CinemachineBlenderSettings.LookupBlend(outgoing, incoming, this.DefaultBlend, this.CustomBlends, this);
		}

		protected abstract CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime);

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			this.UpdateCameraCache();
			for (int i = 0; i < this.m_ChildCameras.Count; i++)
			{
				this.m_ChildCameras[i].OnTargetObjectWarped(target, positionDelta);
			}
			base.OnTargetObjectWarped(target, positionDelta);
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			this.UpdateCameraCache();
			for (int i = 0; i < this.m_ChildCameras.Count; i++)
			{
				this.m_ChildCameras[i].ForceCameraPosition(pos, rot);
			}
			base.ForceCameraPosition(pos, rot);
		}

		public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
			this.m_TransitioningFrom = fromCam;
			base.InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
			this.InternalUpdateCameraState(worldUp, deltaTime);
		}

		public void InvalidateCameraCache()
		{
			this.m_ChildCameras = null;
			this.PreviousStateIsValid = false;
		}

		protected virtual bool UpdateCameraCache()
		{
			int childCount = base.transform.childCount;
			if (this.m_ChildCameras != null && this.m_ChildCountCache == childCount)
			{
				return false;
			}
			this.PreviousStateIsValid = false;
			this.m_ChildCameras = new List<CinemachineVirtualCameraBase>();
			this.m_ChildCountCache = childCount;
			base.GetComponentsInChildren<CinemachineVirtualCameraBase>(true, this.m_ChildCameras);
			for (int i = this.m_ChildCameras.Count - 1; i >= 0; i--)
			{
				if (this.m_ChildCameras[i].transform.parent != base.transform)
				{
					this.m_ChildCameras.RemoveAt(i);
				}
			}
			return true;
		}

		protected virtual void OnTransformChildrenChanged()
		{
			this.InvalidateCameraCache();
		}

		protected void SetLiveChild(ICinemachineCamera activeCamera, Vector3 worldUp, float deltaTime)
		{
			this.m_BlendManager.UpdateRootFrame(this, activeCamera, worldUp, deltaTime);
			this.m_BlendManager.ComputeCurrentBlend();
			this.m_BlendManager.ProcessActiveCamera(this, worldUp, deltaTime);
		}

		protected void ResetLiveChild()
		{
			this.m_BlendManager.ResetRootFrame();
		}

		protected void FinalizeCameraState(float deltaTime)
		{
			this.m_State = this.m_BlendManager.CameraState;
			base.InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Finalize, ref this.m_State, deltaTime);
		}

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineCameraManagerBase.DefaultTargetSettings DefaultTarget;

		[Tooltip("The blend which is used if you don't explicitly define a blend between two Virtual Camera children")]
		[FormerlySerializedAs("m_DefaultBlend")]
		public CinemachineBlendDefinition DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 0.5f);

		[Tooltip("This is the asset which contains custom settings for specific child blends")]
		[FormerlySerializedAs("m_CustomBlends")]
		[EmbeddedBlenderSettingsProperty]
		public CinemachineBlenderSettings CustomBlends;

		private List<CinemachineVirtualCameraBase> m_ChildCameras;

		private int m_ChildCountCache;

		private readonly BlendManager m_BlendManager = new BlendManager();

		private CameraState m_State = CameraState.Default;

		private ICinemachineCamera m_TransitioningFrom;

		[Serializable]
		public struct DefaultTargetSettings
		{
			[Tooltip("If enabled, a default target will be available.  It will be used if a child rig needs a target and doesn't specify one itself.")]
			public bool Enabled;

			[NoSaveDuringPlay]
			[Tooltip("Default target for the camera children, which may be used if the child rig does not specify a target of its own.")]
			public CameraTarget Target;
		}
	}
}
