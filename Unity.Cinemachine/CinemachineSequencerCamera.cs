using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[ExcludeFromPreset]
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Cinemachine Sequencer Camera")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineSequencerCamera.html")]
	public class CinemachineSequencerCamera : CinemachineCameraManagerBase
	{
		protected override void Reset()
		{
			base.Reset();
			this.Loop = false;
			this.Instructions = null;
		}

		private void OnValidate()
		{
			if (this.Instructions != null)
			{
				for (int i = 0; i < this.Instructions.Count; i++)
				{
					CinemachineSequencerCamera.Instruction value = this.Instructions[i];
					value.Validate();
					this.Instructions[i] = value;
				}
			}
		}

		protected internal override void PerformLegacyUpgrade(int streamedVersion)
		{
			base.PerformLegacyUpgrade(streamedVersion);
			if (streamedVersion < 20220721 && (this.m_LegacyLookAt != null || this.m_LegacyFollow != null))
			{
				this.DefaultTarget = new CinemachineCameraManagerBase.DefaultTargetSettings
				{
					Enabled = true,
					Target = new CameraTarget
					{
						LookAtTarget = this.m_LegacyLookAt,
						TrackingTarget = this.m_LegacyFollow,
						CustomLookAtTarget = (this.m_LegacyLookAt != this.m_LegacyFollow)
					}
				};
				this.m_LegacyLookAt = (this.m_LegacyFollow = null);
			}
		}

		public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			this.m_ActivationTime = CinemachineCore.CurrentTime;
			this.m_CurrentInstruction = 0;
		}

		protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
		{
			if (!this.PreviousStateIsValid)
			{
				this.m_CurrentInstruction = -1;
			}
			this.AdvanceCurrentInstruction(deltaTime);
			if (this.m_CurrentInstruction < 0 || this.m_CurrentInstruction >= this.Instructions.Count)
			{
				return null;
			}
			return this.Instructions[this.m_CurrentInstruction].Camera;
		}

		protected override CinemachineBlendDefinition LookupBlend(ICinemachineCamera outgoing, ICinemachineCamera incoming)
		{
			return this.Instructions[this.m_CurrentInstruction].Blend;
		}

		protected override bool UpdateCameraCache()
		{
			if (this.Instructions == null)
			{
				this.Instructions = new List<CinemachineSequencerCamera.Instruction>();
			}
			return base.UpdateCameraCache();
		}

		private void AdvanceCurrentInstruction(float deltaTime)
		{
			if (base.ChildCameras == null || base.ChildCameras.Count == 0 || this.m_ActivationTime < 0f || this.Instructions.Count == 0)
			{
				this.m_ActivationTime = -1f;
				this.m_CurrentInstruction = -1;
				return;
			}
			float currentTime = CinemachineCore.CurrentTime;
			if (this.m_CurrentInstruction < 0 || deltaTime < 0f)
			{
				this.m_ActivationTime = currentTime;
				this.m_CurrentInstruction = 0;
			}
			if (this.m_CurrentInstruction > this.Instructions.Count - 1)
			{
				this.m_ActivationTime = currentTime;
				this.m_CurrentInstruction = this.Instructions.Count - 1;
			}
			float b = this.Instructions[this.m_CurrentInstruction].Hold + this.Instructions[this.m_CurrentInstruction].Blend.BlendTime;
			float a = (this.m_CurrentInstruction < this.Instructions.Count - 1 || this.Loop) ? 0f : float.MaxValue;
			if (currentTime - this.m_ActivationTime > Mathf.Max(a, b))
			{
				this.m_ActivationTime = currentTime;
				this.m_CurrentInstruction++;
				if (this.Loop && this.m_CurrentInstruction == this.Instructions.Count)
				{
					this.m_CurrentInstruction = 0;
				}
			}
		}

		[Tooltip("When enabled, the child vcams will cycle indefinitely instead of just stopping at the last one")]
		[FormerlySerializedAs("m_Loop")]
		public bool Loop;

		[Tooltip("The set of instructions for enabling child cameras.")]
		[FormerlySerializedAs("m_Instructions")]
		public List<CinemachineSequencerCamera.Instruction> Instructions = new List<CinemachineSequencerCamera.Instruction>();

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_LookAt")]
		private Transform m_LegacyLookAt;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_Follow")]
		private Transform m_LegacyFollow;

		private float m_ActivationTime = -1f;

		private int m_CurrentInstruction;

		[Serializable]
		public struct Instruction
		{
			public void Validate()
			{
				this.Hold = Mathf.Max(this.Hold, 0f);
			}

			[Tooltip("The camera to activate when this instruction becomes active")]
			[FormerlySerializedAs("m_VirtualCamera")]
			[ChildCameraProperty]
			public CinemachineVirtualCameraBase Camera;

			[Tooltip("How to blend to the next camera in the list (if any)")]
			[FormerlySerializedAs("m_Blend")]
			public CinemachineBlendDefinition Blend;

			[Tooltip("How long to wait (in seconds) before activating the next camera in the list (if any)")]
			[FormerlySerializedAs("m_Hold")]
			public float Hold;
		}
	}
}
