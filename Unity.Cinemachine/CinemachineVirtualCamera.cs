using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Obsolete("CinemachineVirtualCamera is deprecated. Use CinemachineCamera instead.")]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[ExcludeFromPreset]
	[AddComponentMenu("")]
	public class CinemachineVirtualCamera : CinemachineVirtualCameraBase, AxisState.IRequiresInput
	{
		protected internal override void PerformLegacyUpgrade(int streamedVersion)
		{
			base.PerformLegacyUpgrade(streamedVersion);
			if (streamedVersion < 20221011)
			{
				if (this.m_LegacyTransitions.m_BlendHint != 0)
				{
					if (this.m_LegacyTransitions.m_BlendHint == 3)
					{
						this.BlendHint = CinemachineCore.BlendHints.ScreenSpaceAimWhenTargetsDiffer;
					}
					else
					{
						this.BlendHint = (CinemachineCore.BlendHints)this.m_LegacyTransitions.m_BlendHint;
					}
					this.m_LegacyTransitions.m_BlendHint = 0;
				}
				if (this.m_LegacyTransitions.m_InheritPosition)
				{
					this.BlendHint |= CinemachineCore.BlendHints.InheritPosition;
					this.m_LegacyTransitions.m_InheritPosition = false;
				}
				if (this.m_LegacyTransitions.m_OnCameraLive != null)
				{
					this.m_OnCameraLiveEvent = this.m_LegacyTransitions.m_OnCameraLive;
					this.m_LegacyTransitions.m_OnCameraLive = null;
				}
			}
		}

		protected internal override bool IsDprecated
		{
			get
			{
				return true;
			}
		}

		public override CameraState State
		{
			get
			{
				return this.m_State;
			}
		}

		public override Transform LookAt
		{
			get
			{
				return base.ResolveLookAt(this.m_LookAt);
			}
			set
			{
				this.m_LookAt = value;
			}
		}

		public override Transform Follow
		{
			get
			{
				return base.ResolveFollow(this.m_Follow);
			}
			set
			{
				this.m_Follow = value;
			}
		}

		public override float GetMaxDampTime()
		{
			float num = base.GetMaxDampTime();
			this.UpdateComponentPipeline();
			if (this.m_ComponentPipeline != null)
			{
				for (int i = 0; i < this.m_ComponentPipeline.Length; i++)
				{
					num = Mathf.Max(num, this.m_ComponentPipeline[i].GetMaxDampTime());
				}
			}
			return num;
		}

		public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			base.UpdateTargetCache();
			if (deltaTime < 0f)
			{
				this.PreviousStateIsValid = false;
			}
			this.m_State = this.CalculateNewState(worldUp, deltaTime);
			this.m_State.BlendHint = (CameraState.BlendHints)this.BlendHint;
			if (this.Follow != null)
			{
				base.transform.position = this.State.RawPosition;
			}
			if (this.LookAt != null)
			{
				base.transform.rotation = this.State.RawOrientation;
			}
			this.PreviousStateIsValid = true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_LensSettings = this.m_Lens.ToLensSettings();
			this.m_State = base.PullStateFromVirtualCamera(Vector3.up, ref this.m_LensSettings);
			this.InvalidateComponentPipeline();
		}

		protected override void OnDestroy()
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.GetComponent<CinemachinePipeline>() != null)
				{
					transform.gameObject.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
				}
			}
			base.OnDestroy();
		}

		protected void OnValidate()
		{
			this.m_Lens.Validate();
		}

		private void OnTransformChildrenChanged()
		{
			this.InvalidateComponentPipeline();
		}

		private void Reset()
		{
			this.DestroyPipeline();
			this.UpdateComponentPipeline();
			this.Priority = default(PrioritySettings);
			this.OutputChannel = OutputChannels.Default;
		}

		internal void DestroyPipeline()
		{
			List<Transform> list = new List<Transform>();
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.GetComponent<CinemachinePipeline>() != null)
				{
					list.Add(transform);
				}
			}
			foreach (Transform transform2 in list)
			{
				if (CinemachineVirtualCamera.DestroyPipelineOverride != null)
				{
					CinemachineVirtualCamera.DestroyPipelineOverride(transform2.gameObject);
				}
				else
				{
					CinemachineComponentBase[] components = transform2.GetComponents<CinemachineComponentBase>();
					for (int i = 0; i < components.Length; i++)
					{
						Object.Destroy(components[i]);
					}
					if (!RuntimeUtility.IsPrefab(base.gameObject))
					{
						Object.Destroy(transform2.gameObject);
					}
				}
			}
			this.m_ComponentOwner = null;
			this.InvalidateComponentPipeline();
			this.PreviousStateIsValid = false;
		}

		internal Transform CreatePipeline(CinemachineVirtualCamera copyFrom)
		{
			CinemachineComponentBase[] copyFrom2 = null;
			if (copyFrom != null)
			{
				copyFrom.InvalidateComponentPipeline();
				copyFrom2 = copyFrom.GetComponentPipeline();
			}
			Transform result = null;
			if (CinemachineVirtualCamera.CreatePipelineOverride != null)
			{
				result = CinemachineVirtualCamera.CreatePipelineOverride(this, "cm", copyFrom2);
			}
			else if (!RuntimeUtility.IsPrefab(base.gameObject))
			{
				GameObject gameObject = new GameObject("cm");
				gameObject.transform.parent = base.transform;
				gameObject.AddComponent<CinemachinePipeline>();
				result = gameObject.transform;
			}
			this.PreviousStateIsValid = false;
			return result;
		}

		public void InvalidateComponentPipeline()
		{
			this.m_ComponentPipeline = null;
		}

		public Transform GetComponentOwner()
		{
			this.UpdateComponentPipeline();
			return this.m_ComponentOwner;
		}

		public CinemachineComponentBase[] GetComponentPipeline()
		{
			this.UpdateComponentPipeline();
			return this.m_ComponentPipeline;
		}

		public override CinemachineComponentBase GetCinemachineComponent(CinemachineCore.Stage stage)
		{
			CinemachineComponentBase[] componentPipeline = this.GetComponentPipeline();
			if (componentPipeline != null)
			{
				foreach (CinemachineComponentBase cinemachineComponentBase in componentPipeline)
				{
					if (cinemachineComponentBase.Stage == stage)
					{
						return cinemachineComponentBase;
					}
				}
			}
			return null;
		}

		public T GetCinemachineComponent<T>() where T : CinemachineComponentBase
		{
			CinemachineComponentBase[] componentPipeline = this.GetComponentPipeline();
			if (componentPipeline != null)
			{
				foreach (CinemachineComponentBase cinemachineComponentBase in componentPipeline)
				{
					if (cinemachineComponentBase is T)
					{
						return cinemachineComponentBase as T;
					}
				}
			}
			return default(T);
		}

		public T AddCinemachineComponent<T>() where T : CinemachineComponentBase
		{
			Transform componentOwner = this.GetComponentOwner();
			if (componentOwner == null)
			{
				return default(T);
			}
			CinemachineComponentBase[] components = componentOwner.GetComponents<CinemachineComponentBase>();
			T t = componentOwner.gameObject.AddComponent<T>();
			if (t != null && components != null)
			{
				CinemachineCore.Stage stage = t.Stage;
				for (int i = components.Length - 1; i >= 0; i--)
				{
					if (components[i].Stage == stage)
					{
						components[i].enabled = false;
						RuntimeUtility.DestroyObject(components[i]);
					}
				}
			}
			this.InvalidateComponentPipeline();
			return t;
		}

		public void DestroyCinemachineComponent<T>() where T : CinemachineComponentBase
		{
			CinemachineComponentBase[] componentPipeline = this.GetComponentPipeline();
			if (componentPipeline != null)
			{
				foreach (CinemachineComponentBase cinemachineComponentBase in componentPipeline)
				{
					if (cinemachineComponentBase is T)
					{
						cinemachineComponentBase.enabled = false;
						RuntimeUtility.DestroyObject(cinemachineComponentBase);
						this.InvalidateComponentPipeline();
					}
				}
			}
		}

		private void UpdateComponentPipeline()
		{
			if (this.m_ComponentOwner != null && this.m_ComponentPipeline != null)
			{
				return;
			}
			this.m_ComponentOwner = null;
			List<CinemachineComponentBase> list = new List<CinemachineComponentBase>();
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.GetComponent<CinemachinePipeline>() != null)
				{
					foreach (CinemachineComponentBase cinemachineComponentBase in transform.GetComponents<CinemachineComponentBase>())
					{
						if (cinemachineComponentBase != null && cinemachineComponentBase.enabled)
						{
							list.Add(cinemachineComponentBase);
						}
					}
					this.m_ComponentOwner = transform;
					break;
				}
			}
			if (this.m_ComponentOwner == null)
			{
				this.m_ComponentOwner = this.CreatePipeline(null);
			}
			if (this.m_ComponentOwner != null && this.m_ComponentOwner.gameObject != null)
			{
				list.Sort((CinemachineComponentBase c1, CinemachineComponentBase c2) => c1.Stage - c2.Stage);
				this.m_ComponentPipeline = list.ToArray();
			}
		}

		internal static void SetFlagsForHiddenChild(GameObject child)
		{
			if (child != null)
			{
				child.hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.HideInInspector);
			}
		}

		private CameraState CalculateNewState(Vector3 worldUp, float deltaTime)
		{
			this.FollowTargetAttachment = 1f;
			this.LookAtTargetAttachment = 1f;
			this.m_LensSettings = this.m_Lens.ToLensSettings();
			CameraState result = base.PullStateFromVirtualCamera(worldUp, ref this.m_LensSettings);
			Transform lookAt = this.LookAt;
			if (lookAt != this.mCachedLookAtTarget)
			{
				this.mCachedLookAtTarget = lookAt;
				this.mCachedLookAtTargetVcam = null;
				if (lookAt != null)
				{
					this.mCachedLookAtTargetVcam = lookAt.GetComponent<CinemachineVirtualCameraBase>();
				}
			}
			if (lookAt != null)
			{
				if (this.mCachedLookAtTargetVcam != null)
				{
					result.ReferenceLookAt = this.mCachedLookAtTargetVcam.State.GetFinalPosition();
				}
				else
				{
					result.ReferenceLookAt = TargetPositionCache.GetTargetPosition(lookAt);
				}
			}
			this.UpdateComponentPipeline();
			base.InvokePrePipelineMutateCameraStateCallback(this, ref result, deltaTime);
			if (this.m_ComponentPipeline == null)
			{
				for (CinemachineCore.Stage stage = CinemachineCore.Stage.Body; stage <= CinemachineCore.Stage.Finalize; stage++)
				{
					base.InvokePostPipelineStageCallback(this, stage, ref result, deltaTime);
				}
			}
			else
			{
				for (int i = 0; i < this.m_ComponentPipeline.Length; i++)
				{
					if (this.m_ComponentPipeline[i] != null)
					{
						this.m_ComponentPipeline[i].PrePipelineMutateCameraState(ref result, deltaTime);
					}
				}
				int num = 0;
				CinemachineComponentBase cinemachineComponentBase = null;
				CinemachineCore.Stage stage2 = CinemachineCore.Stage.Body;
				while (stage2 <= CinemachineCore.Stage.Finalize)
				{
					CinemachineComponentBase cinemachineComponentBase2 = (num < this.m_ComponentPipeline.Length) ? this.m_ComponentPipeline[num] : null;
					if (!(cinemachineComponentBase2 != null) || stage2 != cinemachineComponentBase2.Stage)
					{
						goto IL_172;
					}
					num++;
					if (stage2 != CinemachineCore.Stage.Body || !cinemachineComponentBase2.BodyAppliesAfterAim)
					{
						cinemachineComponentBase2.MutateCameraState(ref result, deltaTime);
						goto IL_172;
					}
					cinemachineComponentBase = cinemachineComponentBase2;
					IL_1A2:
					stage2++;
					continue;
					IL_172:
					base.InvokePostPipelineStageCallback(this, stage2, ref result, deltaTime);
					if (stage2 == CinemachineCore.Stage.Aim && cinemachineComponentBase != null)
					{
						cinemachineComponentBase.MutateCameraState(ref result, deltaTime);
						base.InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Body, ref result, deltaTime);
						goto IL_1A2;
					}
					goto IL_1A2;
				}
			}
			return result;
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			if (target == this.Follow)
			{
				base.transform.position += positionDelta;
				this.m_State.RawPosition = this.m_State.RawPosition + positionDelta;
			}
			this.UpdateComponentPipeline();
			if (this.m_ComponentPipeline != null)
			{
				for (int i = 0; i < this.m_ComponentPipeline.Length; i++)
				{
					this.m_ComponentPipeline[i].OnTargetObjectWarped(target, positionDelta);
				}
			}
			base.OnTargetObjectWarped(target, positionDelta);
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			this.PreviousStateIsValid = true;
			base.transform.SetPositionAndRotation(pos, rot);
			this.m_State.RawPosition = pos;
			this.m_State.RawOrientation = rot;
			this.UpdateComponentPipeline();
			if (this.m_ComponentPipeline != null)
			{
				for (int i = 0; i < this.m_ComponentPipeline.Length; i++)
				{
					this.m_ComponentPipeline[i].ForceCameraPosition(pos, rot);
				}
			}
			base.ForceCameraPosition(pos, rot);
		}

		internal void SetStateRawPosition(Vector3 pos)
		{
			this.m_State.RawPosition = pos;
		}

		public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
			base.InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
			bool flag = false;
			if (fromCam != null && (this.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && !CinemachineCore.IsLiveInBlend(this))
			{
				this.ForceCameraPosition(fromCam.State.GetFinalPosition(), fromCam.State.GetFinalOrientation());
			}
			this.UpdateComponentPipeline();
			if (this.m_ComponentPipeline != null)
			{
				for (int i = 0; i < this.m_ComponentPipeline.Length; i++)
				{
					if (this.m_ComponentPipeline[i].OnTransitionFromCamera(fromCam, worldUp, deltaTime))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.InternalUpdateCameraState(worldUp, deltaTime);
				this.InternalUpdateCameraState(worldUp, deltaTime);
			}
			else
			{
				base.UpdateCameraState(worldUp, deltaTime);
			}
			CinemachineLegacyCameraEvents.OnCameraLiveEvent onCameraLiveEvent = this.m_OnCameraLiveEvent;
			if (onCameraLiveEvent == null)
			{
				return;
			}
			onCameraLiveEvent.Invoke(this, fromCam);
		}

		bool AxisState.IRequiresInput.RequiresInput()
		{
			if (base.Extensions != null)
			{
				for (int i = 0; i < base.Extensions.Count; i++)
				{
					if (base.Extensions[i] is AxisState.IRequiresInput)
					{
						return true;
					}
				}
			}
			this.UpdateComponentPipeline();
			if (this.m_ComponentPipeline != null)
			{
				for (int j = 0; j < this.m_ComponentPipeline.Length; j++)
				{
					if (this.m_ComponentPipeline[j] is AxisState.IRequiresInput)
					{
						return true;
					}
				}
			}
			return false;
		}

		[Tooltip("The object that the camera wants to look at (the Aim target).  If this is null, then the vcam's Transform orientation will define the camera's orientation.")]
		[NoSaveDuringPlay]
		[VcamTargetProperty]
		public Transform m_LookAt;

		[Tooltip("The object that the camera wants to move with (the Body target).  If this is null, then the vcam's Transform position will define the camera's position.")]
		[NoSaveDuringPlay]
		[VcamTargetProperty]
		public Transform m_Follow;

		[Tooltip("Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active.")]
		[FormerlySerializedAs("m_LensAttributes")]
		public LegacyLensSettings m_Lens = LegacyLensSettings.Default;

		[Tooltip("Hint for transitioning to and from this CinemachineCamera.  Hints can be combined, although not all combinations make sense.  In the case of conflicting hints, Cinemachine will make an arbitrary choice.")]
		public CinemachineCore.BlendHints BlendHint;

		[Tooltip("This event fires when a transition occurs")]
		public CinemachineLegacyCameraEvents.OnCameraLiveEvent m_OnCameraLiveEvent = new CinemachineLegacyCameraEvents.OnCameraLiveEvent();

		[HideInInspector]
		[SerializeField]
		[NoSaveDuringPlay]
		internal string[] m_ExcludedPropertiesInInspector = new string[]
		{
			"m_Script"
		};

		[HideInInspector]
		[SerializeField]
		[NoSaveDuringPlay]
		internal CinemachineCore.Stage[] m_LockStageInInspector;

		[FormerlySerializedAs("m_Transitions")]
		[SerializeField]
		[HideInInspector]
		private CinemachineVirtualCamera.LegacyTransitionParams m_LegacyTransitions;

		private const string PipelineName = "cm";

		public static CinemachineVirtualCamera.CreatePipelineDelegate CreatePipelineOverride;

		public static CinemachineVirtualCamera.DestroyPipelineDelegate DestroyPipelineOverride;

		private CameraState m_State = CameraState.Default;

		private CinemachineComponentBase[] m_ComponentPipeline;

		[SerializeField]
		[HideInInspector]
		private Transform m_ComponentOwner;

		private LensSettings m_LensSettings;

		private Transform mCachedLookAtTarget;

		private CinemachineVirtualCameraBase mCachedLookAtTargetVcam;

		[Serializable]
		private struct LegacyTransitionParams
		{
			[FormerlySerializedAs("m_PositionBlending")]
			public int m_BlendHint;

			public bool m_InheritPosition;

			public CinemachineLegacyCameraEvents.OnCameraLiveEvent m_OnCameraLive;
		}

		public delegate Transform CreatePipelineDelegate(CinemachineVirtualCamera vcam, string name, CinemachineComponentBase[] copyFrom);

		public delegate void DestroyPipelineDelegate(GameObject pipeline);
	}
}
