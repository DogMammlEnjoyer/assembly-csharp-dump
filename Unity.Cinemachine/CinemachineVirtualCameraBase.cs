using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	public abstract class CinemachineVirtualCameraBase : MonoBehaviour, ICinemachineCamera
	{
		protected internal virtual bool IsDprecated
		{
			get
			{
				return false;
			}
		}

		protected internal virtual void PerformLegacyUpgrade(int streamedVersion)
		{
			if (streamedVersion < 20220601 && this.m_LegacyPriority != 0)
			{
				this.Priority.Value = this.m_LegacyPriority;
				this.m_LegacyPriority = 0;
			}
		}

		public virtual float GetMaxDampTime()
		{
			float num = 0f;
			if (this.Extensions != null)
			{
				for (int i = 0; i < this.Extensions.Count; i++)
				{
					num = Mathf.Max(num, this.Extensions[i].GetMaxDampTime());
				}
			}
			return num;
		}

		public float DetachedFollowTargetDamp(float initial, float dampTime, float deltaTime)
		{
			dampTime = Mathf.Lerp(Mathf.Max(1f, dampTime), dampTime, this.FollowTargetAttachment);
			deltaTime = Mathf.Lerp(0f, deltaTime, this.FollowTargetAttachment);
			return Damper.Damp(initial, dampTime, deltaTime);
		}

		public Vector3 DetachedFollowTargetDamp(Vector3 initial, Vector3 dampTime, float deltaTime)
		{
			dampTime = Vector3.Lerp(Vector3.Max(Vector3.one, dampTime), dampTime, this.FollowTargetAttachment);
			deltaTime = Mathf.Lerp(0f, deltaTime, this.FollowTargetAttachment);
			return Damper.Damp(initial, dampTime, deltaTime);
		}

		public Vector3 DetachedFollowTargetDamp(Vector3 initial, float dampTime, float deltaTime)
		{
			dampTime = Mathf.Lerp(Mathf.Max(1f, dampTime), dampTime, this.FollowTargetAttachment);
			deltaTime = Mathf.Lerp(0f, deltaTime, this.FollowTargetAttachment);
			return Damper.Damp(initial, dampTime, deltaTime);
		}

		public float DetachedLookAtTargetDamp(float initial, float dampTime, float deltaTime)
		{
			dampTime = Mathf.Lerp(Mathf.Max(1f, dampTime), dampTime, this.LookAtTargetAttachment);
			deltaTime = Mathf.Lerp(0f, deltaTime, this.LookAtTargetAttachment);
			return Damper.Damp(initial, dampTime, deltaTime);
		}

		public Vector3 DetachedLookAtTargetDamp(Vector3 initial, Vector3 dampTime, float deltaTime)
		{
			dampTime = Vector3.Lerp(Vector3.Max(Vector3.one, dampTime), dampTime, this.LookAtTargetAttachment);
			deltaTime = Mathf.Lerp(0f, deltaTime, this.LookAtTargetAttachment);
			return Damper.Damp(initial, dampTime, deltaTime);
		}

		public Vector3 DetachedLookAtTargetDamp(Vector3 initial, float dampTime, float deltaTime)
		{
			dampTime = Mathf.Lerp(Mathf.Max(1f, dampTime), dampTime, this.LookAtTargetAttachment);
			deltaTime = Mathf.Lerp(0f, deltaTime, this.LookAtTargetAttachment);
			return Damper.Damp(initial, dampTime, deltaTime);
		}

		internal void AddExtension(CinemachineExtension extension)
		{
			if (this.Extensions == null)
			{
				this.Extensions = new List<CinemachineExtension>();
			}
			else
			{
				this.Extensions.Remove(extension);
			}
			this.Extensions.Add(extension);
		}

		internal void RemoveExtension(CinemachineExtension extension)
		{
			if (this.Extensions != null)
			{
				this.Extensions.Remove(extension);
			}
		}

		internal List<CinemachineExtension> Extensions { get; private set; }

		protected void InvokePostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState newState, float deltaTime)
		{
			if (this.Extensions != null)
			{
				for (int i = 0; i < this.Extensions.Count; i++)
				{
					CinemachineExtension cinemachineExtension = this.Extensions[i];
					if (cinemachineExtension == null)
					{
						this.Extensions.RemoveAt(i);
						i--;
					}
					else if (cinemachineExtension.enabled)
					{
						cinemachineExtension.InvokePostPipelineStageCallback(vcam, stage, ref newState, deltaTime);
					}
				}
			}
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = this.ParentCamera as CinemachineVirtualCameraBase;
			if (cinemachineVirtualCameraBase != null)
			{
				cinemachineVirtualCameraBase.InvokePostPipelineStageCallback(vcam, stage, ref newState, deltaTime);
			}
		}

		protected void InvokePrePipelineMutateCameraStateCallback(CinemachineVirtualCameraBase vcam, ref CameraState newState, float deltaTime)
		{
			if (this.Extensions != null)
			{
				for (int i = 0; i < this.Extensions.Count; i++)
				{
					CinemachineExtension cinemachineExtension = this.Extensions[i];
					if (cinemachineExtension == null)
					{
						this.Extensions.RemoveAt(i);
						i--;
					}
					else if (cinemachineExtension.enabled)
					{
						cinemachineExtension.PrePipelineMutateCameraStateCallback(vcam, ref newState, deltaTime);
					}
				}
			}
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = this.ParentCamera as CinemachineVirtualCameraBase;
			if (cinemachineVirtualCameraBase != null)
			{
				cinemachineVirtualCameraBase.InvokePrePipelineMutateCameraStateCallback(vcam, ref newState, deltaTime);
			}
		}

		protected bool InvokeOnTransitionInExtensions(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			bool result = false;
			if (this.Extensions != null)
			{
				for (int i = 0; i < this.Extensions.Count; i++)
				{
					CinemachineExtension cinemachineExtension = this.Extensions[i];
					if (cinemachineExtension == null)
					{
						this.Extensions.RemoveAt(i);
						i--;
					}
					else if (cinemachineExtension.enabled && cinemachineExtension.OnTransitionFromCamera(fromCam, worldUp, deltaTime))
					{
						result = true;
					}
				}
			}
			return result;
		}

		public string Name
		{
			get
			{
				if (this.m_CachedName == null)
				{
					this.m_CachedName = (this.IsValid ? base.name : "(deleted)");
				}
				return this.m_CachedName;
			}
		}

		public virtual string Description
		{
			get
			{
				return "";
			}
		}

		public bool IsValid
		{
			get
			{
				return !(this == null);
			}
		}

		public abstract CameraState State { get; }

		public ICinemachineMixer ParentCamera
		{
			get
			{
				if (!this.m_ChildStatusUpdated || !Application.isPlaying)
				{
					this.UpdateStatusAsChild();
				}
				return this.m_ParentVcam as ICinemachineMixer;
			}
		}

		public abstract Transform LookAt { get; set; }

		public abstract Transform Follow { get; set; }

		public virtual bool PreviousStateIsValid { get; set; }

		public void UpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			CameraUpdateManager.UpdateVirtualCamera(this, worldUp, deltaTime);
		}

		public abstract void InternalUpdateCameraState(Vector3 worldUp, float deltaTime);

		public virtual void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
		{
			if (evt.IncomingCamera == this)
			{
				this.OnTransitionFromCamera(evt.OutgoingCamera, evt.WorldUp, evt.DeltaTime);
			}
		}

		public virtual void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				this.PreviousStateIsValid = false;
			}
		}

		internal void EnsureStarted()
		{
			if (!this.m_WasStarted)
			{
				this.m_WasStarted = true;
				if (this.m_StreamingVersion < 20241001)
				{
					this.PerformLegacyUpgrade(this.m_StreamingVersion);
				}
				this.m_StreamingVersion = 20241001;
				CinemachineExtension[] componentsInChildren = base.GetComponentsInChildren<CinemachineExtension>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].EnsureStarted();
				}
			}
		}

		protected virtual void OnTransformParentChanged()
		{
			CameraUpdateManager.CameraDisabled(this);
			CameraUpdateManager.CameraEnabled(this);
			this.UpdateStatusAsChild();
			this.UpdateVcamPoolStatus();
		}

		protected virtual void OnDestroy()
		{
			CameraUpdateManager.CameraDestroyed(this);
		}

		protected virtual void Start()
		{
			this.m_WasStarted = true;
			if (this.m_StreamingVersion < 20241001)
			{
				this.PerformLegacyUpgrade(this.m_StreamingVersion);
			}
			this.m_StreamingVersion = 20241001;
		}

		protected virtual void OnEnable()
		{
			this.UpdateStatusAsChild();
			this.UpdateVcamPoolStatus();
			if (!CinemachineCore.IsLive(this))
			{
				this.PreviousStateIsValid = false;
			}
			CameraUpdateManager.CameraEnabled(this);
			this.InvalidateCachedTargets();
			CinemachineVirtualCameraBase[] components = base.GetComponents<CinemachineVirtualCameraBase>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].enabled && components[i] != this)
				{
					CinemachineVirtualCameraBase cinemachineVirtualCameraBase = components[i].IsDprecated ? components[i] : this;
					if (!cinemachineVirtualCameraBase.IsDprecated)
					{
						Debug.LogWarning(this.Name + " has multiple CinemachineVirtualCameraBase-derived components.  Disabling " + cinemachineVirtualCameraBase.GetType().Name);
					}
					cinemachineVirtualCameraBase.enabled = false;
				}
			}
		}

		protected virtual void OnDisable()
		{
			this.UpdateVcamPoolStatus();
			CameraUpdateManager.CameraDisabled(this);
		}

		protected virtual void Update()
		{
			if (this.Priority.Value != this.m_QueuePriority)
			{
				this.UpdateVcamPoolStatus();
			}
		}

		private void UpdateStatusAsChild()
		{
			this.m_ChildStatusUpdated = true;
			this.m_ParentVcam = null;
			Transform parent = base.transform.parent;
			if (parent != null)
			{
				parent.TryGetComponent<CinemachineVirtualCameraBase>(out this.m_ParentVcam);
			}
		}

		public Transform ResolveLookAt(Transform localLookAt)
		{
			Transform transform = localLookAt;
			if (transform == null)
			{
				CinemachineVirtualCameraBase cinemachineVirtualCameraBase = this.ParentCamera as CinemachineVirtualCameraBase;
				if (cinemachineVirtualCameraBase != null)
				{
					transform = cinemachineVirtualCameraBase.LookAt;
				}
			}
			return transform;
		}

		public Transform ResolveFollow(Transform localFollow)
		{
			Transform transform = localFollow;
			if (transform == null)
			{
				CinemachineVirtualCameraBase cinemachineVirtualCameraBase = this.ParentCamera as CinemachineVirtualCameraBase;
				if (cinemachineVirtualCameraBase != null)
				{
					transform = cinemachineVirtualCameraBase.Follow;
				}
			}
			return transform;
		}

		private void UpdateVcamPoolStatus()
		{
			CameraUpdateManager.RemoveActiveCamera(this);
			if (this.m_ParentVcam == null && base.isActiveAndEnabled)
			{
				CameraUpdateManager.AddActiveCamera(this);
			}
			this.m_QueuePriority = this.Priority.Value;
		}

		[Obsolete("Please use Prioritize()")]
		public void MoveToTopOfPrioritySubqueue()
		{
			this.Prioritize();
		}

		public void Prioritize()
		{
			this.UpdateVcamPoolStatus();
		}

		public virtual void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			this.OnTargetObjectWarped(this, target, positionDelta);
		}

		private void OnTargetObjectWarped(CinemachineVirtualCameraBase vcam, Transform target, Vector3 positionDelta)
		{
			List<CinemachineExtension> extensions = this.Extensions;
			int? num = (extensions != null) ? new int?(extensions.Count) : null;
			int num2 = 0;
			for (;;)
			{
				int num3 = num2;
				int? num4 = num;
				if (!(num3 < num4.GetValueOrDefault() & num4 != null))
				{
					break;
				}
				this.Extensions[num2].OnTargetObjectWarped(vcam, target, positionDelta);
				num2++;
			}
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = this.ParentCamera as CinemachineVirtualCameraBase;
			if (cinemachineVirtualCameraBase != null)
			{
				cinemachineVirtualCameraBase.OnTargetObjectWarped(vcam, target, positionDelta);
			}
		}

		public virtual void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			this.ForceCameraPosition(this, pos, rot);
		}

		private void ForceCameraPosition(CinemachineVirtualCameraBase vcam, Vector3 pos, Quaternion rot)
		{
			List<CinemachineExtension> extensions = this.Extensions;
			int? num = (extensions != null) ? new int?(extensions.Count) : null;
			int num2 = 0;
			for (;;)
			{
				int num3 = num2;
				int? num4 = num;
				if (!(num3 < num4.GetValueOrDefault() & num4 != null))
				{
					break;
				}
				this.Extensions[num2].ForceCameraPosition(vcam, pos, rot);
				this.Extensions[num2].ForceCameraPosition(pos, rot);
				num2++;
			}
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = this.ParentCamera as CinemachineVirtualCameraBase;
			if (cinemachineVirtualCameraBase != null)
			{
				cinemachineVirtualCameraBase.ForceCameraPosition(vcam, pos, rot);
			}
		}

		protected CameraState PullStateFromVirtualCamera(Vector3 worldUp, ref LensSettings lens)
		{
			CameraState @default = CameraState.Default;
			@default.RawPosition = TargetPositionCache.GetTargetPosition(base.transform);
			@default.RawOrientation = TargetPositionCache.GetTargetRotation(base.transform);
			@default.ReferenceUp = worldUp;
			CinemachineBrain cinemachineBrain = CinemachineCore.FindPotentialTargetBrain(this);
			if (cinemachineBrain != null && cinemachineBrain.OutputCamera != null)
			{
				lens.PullInheritedPropertiesFromCamera(cinemachineBrain.OutputCamera);
			}
			@default.Lens = lens;
			return @default;
		}

		private void InvalidateCachedTargets()
		{
			this.m_CachedFollowTarget = null;
			this.m_CachedFollowTargetVcam = null;
			this.m_CachedFollowTargetGroup = null;
			this.m_CachedLookAtTarget = null;
			this.m_CachedLookAtTargetVcam = null;
			this.m_CachedLookAtTargetGroup = null;
		}

		public bool FollowTargetChanged { get; private set; }

		public bool LookAtTargetChanged { get; private set; }

		public void UpdateTargetCache()
		{
			Transform transform = this.ResolveFollow(this.Follow);
			this.FollowTargetChanged = (transform != this.m_CachedFollowTarget);
			if (this.FollowTargetChanged)
			{
				this.m_CachedFollowTarget = transform;
				this.m_CachedFollowTargetVcam = null;
				this.m_CachedFollowTargetGroup = null;
				if (this.m_CachedFollowTarget != null)
				{
					transform.TryGetComponent<CinemachineVirtualCameraBase>(out this.m_CachedFollowTargetVcam);
					transform.TryGetComponent<ICinemachineTargetGroup>(out this.m_CachedFollowTargetGroup);
				}
			}
			transform = this.ResolveLookAt(this.LookAt);
			this.LookAtTargetChanged = (transform != this.m_CachedLookAtTarget);
			if (this.LookAtTargetChanged)
			{
				this.m_CachedLookAtTarget = transform;
				this.m_CachedLookAtTargetVcam = null;
				this.m_CachedLookAtTargetGroup = null;
				if (transform != null)
				{
					transform.TryGetComponent<CinemachineVirtualCameraBase>(out this.m_CachedLookAtTargetVcam);
					transform.TryGetComponent<ICinemachineTargetGroup>(out this.m_CachedLookAtTargetGroup);
				}
			}
		}

		public ICinemachineTargetGroup FollowTargetAsGroup
		{
			get
			{
				return this.m_CachedFollowTargetGroup;
			}
		}

		public CinemachineVirtualCameraBase FollowTargetAsVcam
		{
			get
			{
				return this.m_CachedFollowTargetVcam;
			}
		}

		public ICinemachineTargetGroup LookAtTargetAsGroup
		{
			get
			{
				return this.m_CachedLookAtTargetGroup;
			}
		}

		public CinemachineVirtualCameraBase LookAtTargetAsVcam
		{
			get
			{
				return this.m_CachedLookAtTargetVcam;
			}
		}

		public virtual CinemachineComponentBase GetCinemachineComponent(CinemachineCore.Stage stage)
		{
			return null;
		}

		public bool IsLive
		{
			get
			{
				return CinemachineCore.IsLive(this);
			}
		}

		public bool IsParticipatingInBlend()
		{
			if (this.IsLive)
			{
				CinemachineCameraManagerBase cinemachineCameraManagerBase = this.ParentCamera as CinemachineCameraManagerBase;
				if (cinemachineCameraManagerBase != null)
				{
					return (cinemachineCameraManagerBase.ActiveBlend != null && cinemachineCameraManagerBase.ActiveBlend.Uses(this)) || cinemachineCameraManagerBase.IsParticipatingInBlend();
				}
				CinemachineBrain cinemachineBrain = CinemachineCore.FindPotentialTargetBrain(this);
				if (cinemachineBrain != null)
				{
					return cinemachineBrain.ActiveBlend != null && cinemachineBrain.ActiveBlend.Uses(this);
				}
			}
			return false;
		}

		public void CancelDamping(bool updateNow = false)
		{
			this.PreviousStateIsValid = false;
			if (updateNow)
			{
				Vector3 worldUp = this.State.ReferenceUp;
				CinemachineBrain cinemachineBrain = CinemachineCore.FindPotentialTargetBrain(this);
				if (cinemachineBrain != null)
				{
					worldUp = cinemachineBrain.DefaultWorldUp;
				}
				this.InternalUpdateCameraState(worldUp, -1f);
			}
		}

		[NoSaveDuringPlay]
		[Tooltip("Priority can be used to control which Cm Camera is live when multiple CM Cameras are active simultaneously.  The most-recently-activated CinemachineCamera will take control, unless there is another Cm Camera active with a higher priority.  In general, the most-recently-activated highest-priority CinemachineCamera will control the main camera. \n\nThe default priority is value 0.  Often it is sufficient to leave the default setting.  In special cases where you want a CinemachineCamera to have a higher or lower priority value than 0, you can set it here.")]
		[EnabledProperty("Enabled", "(using default)")]
		public PrioritySettings Priority;

		[NoSaveDuringPlay]
		[Tooltip("The output channel functions like Unity layers.  Use it to filter the output of CinemachineCameras to different CinemachineBrains, for instance in a multi-screen environemnt.")]
		public OutputChannels OutputChannel = OutputChannels.Default;

		internal int ActivationId;

		private int m_QueuePriority = int.MaxValue;

		[NonSerialized]
		public float FollowTargetAttachment;

		[NonSerialized]
		public float LookAtTargetAttachment;

		[Tooltip("When the virtual camera is not live, this is how often the virtual camera will be updated.  Set this to tune for performance. Most of the time Never is fine, unless the virtual camera is doing shot evaluation.")]
		[FormerlySerializedAs("m_StandbyUpdate")]
		public CinemachineVirtualCameraBase.StandbyUpdateMode StandbyUpdate = CinemachineVirtualCameraBase.StandbyUpdateMode.RoundRobin;

		private string m_CachedName;

		private bool m_WasStarted;

		private bool m_ChildStatusUpdated;

		private CinemachineVirtualCameraBase m_ParentVcam;

		private Transform m_CachedFollowTarget;

		private CinemachineVirtualCameraBase m_CachedFollowTargetVcam;

		private ICinemachineTargetGroup m_CachedFollowTargetGroup;

		private Transform m_CachedLookAtTarget;

		private CinemachineVirtualCameraBase m_CachedLookAtTargetVcam;

		private ICinemachineTargetGroup m_CachedLookAtTargetGroup;

		[HideInInspector]
		[SerializeField]
		[NoSaveDuringPlay]
		private int m_StreamingVersion;

		[HideInInspector]
		[SerializeField]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_Priority")]
		private int m_LegacyPriority;

		public enum StandbyUpdateMode
		{
			Never,
			Always,
			RoundRobin
		}
	}
}
