using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Cinemachine Camera")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineCamera.html")]
	public sealed class CinemachineCamera : CinemachineVirtualCameraBase
	{
		private void Reset()
		{
			this.Priority = default(PrioritySettings);
			this.OutputChannel = OutputChannels.Default;
			this.Target = default(CameraTarget);
			this.Lens = LensSettings.Default;
		}

		private void OnValidate()
		{
			this.Lens.Validate();
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
				return base.ResolveLookAt(this.Target.CustomLookAtTarget ? this.Target.LookAtTarget : this.Target.TrackingTarget);
			}
			set
			{
				this.Target.CustomLookAtTarget = true;
				this.Target.LookAtTarget = value;
			}
		}

		public override Transform Follow
		{
			get
			{
				return base.ResolveFollow(this.Target.TrackingTarget);
			}
			set
			{
				this.Target.TrackingTarget = value;
			}
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			if (target == this.Follow)
			{
				base.transform.position += positionDelta;
				this.m_State.RawPosition = this.m_State.RawPosition + positionDelta;
			}
			this.UpdatePipelineCache();
			for (int i = 0; i < this.m_Pipeline.Length; i++)
			{
				if (this.m_Pipeline[i] != null)
				{
					this.m_Pipeline[i].OnTargetObjectWarped(target, positionDelta);
				}
			}
			base.OnTargetObjectWarped(target, positionDelta);
		}

		public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
		{
			this.PreviousStateIsValid = false;
			this.UpdatePipelineCache();
			for (int i = 0; i < this.m_Pipeline.Length; i++)
			{
				if (this.m_Pipeline[i] != null)
				{
					this.m_Pipeline[i].ForceCameraPosition(pos, rot);
				}
			}
			this.m_State.RawPosition = pos;
			this.m_State.RawOrientation = rot;
			base.ForceCameraPosition(pos, rot);
		}

		public override float GetMaxDampTime()
		{
			float num = base.GetMaxDampTime();
			this.UpdatePipelineCache();
			for (int i = 0; i < this.m_Pipeline.Length; i++)
			{
				if (this.m_Pipeline[i] != null)
				{
					num = Mathf.Max(num, this.m_Pipeline[i].GetMaxDampTime());
				}
			}
			return num;
		}

		public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
			base.InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
			bool flag = false;
			if ((this.State.BlendHint & CameraState.BlendHints.InheritPosition) != CameraState.BlendHints.Nothing && fromCam != null && !CinemachineCore.IsLiveInBlend(this))
			{
				CameraState state = fromCam.State;
				this.ForceCameraPosition(state.GetFinalPosition(), state.GetFinalOrientation());
			}
			this.UpdatePipelineCache();
			for (int i = 0; i < this.m_Pipeline.Length; i++)
			{
				if (this.m_Pipeline[i] != null && this.m_Pipeline[i].OnTransitionFromCamera(fromCam, worldUp, deltaTime))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				base.UpdateCameraState(worldUp, deltaTime);
				return;
			}
			this.InternalUpdateCameraState(worldUp, deltaTime);
			this.InternalUpdateCameraState(worldUp, deltaTime);
		}

		public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			base.UpdateTargetCache();
			this.FollowTargetAttachment = 1f;
			this.LookAtTargetAttachment = 1f;
			if (deltaTime < 0f)
			{
				this.PreviousStateIsValid = false;
			}
			this.m_State = base.PullStateFromVirtualCamera(worldUp, ref this.Lens);
			Transform lookAt = this.LookAt;
			if (lookAt != null)
			{
				this.m_State.ReferenceLookAt = ((base.LookAtTargetAsVcam != null) ? base.LookAtTargetAsVcam.State.GetFinalPosition() : TargetPositionCache.GetTargetPosition(lookAt));
			}
			this.m_State.BlendHint = (CameraState.BlendHints)this.BlendHint;
			this.InvokeComponentPipeline(ref this.m_State, deltaTime);
			base.transform.ConservativeSetPositionAndRotation(this.m_State.RawPosition, this.m_State.RawOrientation);
			this.PreviousStateIsValid = true;
		}

		private CameraState InvokeComponentPipeline(ref CameraState state, float deltaTime)
		{
			base.InvokePrePipelineMutateCameraStateCallback(this, ref state, deltaTime);
			this.UpdatePipelineCache();
			for (int i = 0; i < this.m_Pipeline.Length; i++)
			{
				CinemachineComponentBase cinemachineComponentBase = this.m_Pipeline[i];
				if (cinemachineComponentBase != null && cinemachineComponentBase.IsValid)
				{
					cinemachineComponentBase.PrePipelineMutateCameraState(ref state, deltaTime);
				}
			}
			CinemachineComponentBase cinemachineComponentBase2 = null;
			int j = 0;
			while (j < this.m_Pipeline.Length)
			{
				CinemachineCore.Stage stage = (CinemachineCore.Stage)j;
				CinemachineComponentBase cinemachineComponentBase3 = this.m_Pipeline[j];
				if (!(cinemachineComponentBase3 != null) || !cinemachineComponentBase3.IsValid)
				{
					goto IL_85;
				}
				if (stage != CinemachineCore.Stage.Body || !cinemachineComponentBase3.BodyAppliesAfterAim)
				{
					cinemachineComponentBase3.MutateCameraState(ref state, deltaTime);
					goto IL_85;
				}
				cinemachineComponentBase2 = cinemachineComponentBase3;
				IL_B0:
				j++;
				continue;
				IL_85:
				base.InvokePostPipelineStageCallback(this, stage, ref state, deltaTime);
				if (stage == CinemachineCore.Stage.Aim && cinemachineComponentBase2 != null)
				{
					cinemachineComponentBase2.MutateCameraState(ref state, deltaTime);
					base.InvokePostPipelineStageCallback(this, CinemachineCore.Stage.Body, ref state, deltaTime);
					goto IL_B0;
				}
				goto IL_B0;
			}
			return state;
		}

		internal void InvalidatePipelineCache()
		{
			this.m_Pipeline = null;
		}

		internal bool PipelineCacheInvalidated
		{
			get
			{
				return this.m_Pipeline == null;
			}
		}

		internal Type PeekPipelineCacheType(CinemachineCore.Stage stage)
		{
			if (!(this.m_Pipeline[(int)stage] == null))
			{
				return this.m_Pipeline[(int)stage].GetType();
			}
			return null;
		}

		private void UpdatePipelineCache()
		{
			if (this.m_Pipeline == null || this.m_Pipeline.Length != 4)
			{
				this.m_Pipeline = new CinemachineComponentBase[4];
				CinemachineComponentBase[] components = base.GetComponents<CinemachineComponentBase>();
				for (int i = 0; i < components.Length; i++)
				{
					if (this.m_Pipeline[(int)components[i].Stage] == null)
					{
						this.m_Pipeline[(int)components[i].Stage] = components[i];
					}
				}
			}
		}

		public override CinemachineComponentBase GetCinemachineComponent(CinemachineCore.Stage stage)
		{
			this.UpdatePipelineCache();
			if (stage < CinemachineCore.Stage.Body || stage >= (CinemachineCore.Stage)this.m_Pipeline.Length)
			{
				return null;
			}
			return this.m_Pipeline[(int)stage];
		}

		[NoSaveDuringPlay]
		[Tooltip("Specifies the Tracking and LookAt targets for this camera.")]
		public CameraTarget Target;

		[Tooltip("Specifies the lens properties of this Virtual Camera.  This generally mirrors the Unity Camera's lens settings, and will be used to drive the Unity camera when the vcam is active.")]
		public LensSettings Lens = LensSettings.Default;

		[Tooltip("Hint for transitioning to and from this CinemachineCamera.  Hints can be combined, although not all combinations make sense.  In the case of conflicting hints, Cinemachine will make an arbitrary choice.")]
		public CinemachineCore.BlendHints BlendHint;

		private CameraState m_State = CameraState.Default;

		private CinemachineComponentBase[] m_Pipeline;
	}
}
