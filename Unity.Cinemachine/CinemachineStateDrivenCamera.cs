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
	[AddComponentMenu("Cinemachine/Cinemachine State Driven Camera")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineStateDrivenCamera.html")]
	public class CinemachineStateDrivenCamera : CinemachineCameraManagerBase
	{
		internal void SetParentHash(List<CinemachineStateDrivenCamera.ParentHash> list)
		{
			this.HashOfParent.Clear();
			this.HashOfParent.AddRange(list);
		}

		protected override void Reset()
		{
			base.Reset();
			this.Instructions = null;
			this.AnimatedTarget = null;
			this.LayerIndex = 0;
			this.Instructions = null;
			this.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 0.5f);
			this.CustomBlends = null;
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

		internal static int CreateFakeHash(int parentHash, AnimationClip clip)
		{
			return Animator.StringToHash(parentHash.ToString() + "_" + clip.name);
		}

		private int LookupFakeHash(int parentHash, AnimationClip clip)
		{
			if (this.m_HashCache == null)
			{
				this.m_HashCache = new Dictionary<AnimationClip, List<CinemachineStateDrivenCamera.HashPair>>();
			}
			List<CinemachineStateDrivenCamera.HashPair> list;
			if (!this.m_HashCache.TryGetValue(clip, out list))
			{
				list = new List<CinemachineStateDrivenCamera.HashPair>();
				this.m_HashCache[clip] = list;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].parentHash == parentHash)
				{
					return list[i].hash;
				}
			}
			int num = CinemachineStateDrivenCamera.CreateFakeHash(parentHash, clip);
			list.Add(new CinemachineStateDrivenCamera.HashPair
			{
				parentHash = parentHash,
				hash = num
			});
			this.m_StateParentLookup[num] = parentHash;
			return num;
		}

		internal void ValidateInstructions()
		{
			if (this.Instructions == null)
			{
				this.Instructions = Array.Empty<CinemachineStateDrivenCamera.Instruction>();
			}
			this.m_InstructionDictionary = new Dictionary<int, List<int>>();
			for (int i = 0; i < this.Instructions.Length; i++)
			{
				List<int> list;
				if (!this.m_InstructionDictionary.TryGetValue(this.Instructions[i].FullHash, out list))
				{
					list = new List<int>();
					this.m_InstructionDictionary[this.Instructions[i].FullHash] = list;
				}
				list.Add(i);
			}
			this.m_StateParentLookup = new Dictionary<int, int>();
			int num = 0;
			while (this.HashOfParent != null && num < this.HashOfParent.Count)
			{
				this.m_StateParentLookup[this.HashOfParent[num].Hash] = this.HashOfParent[num].HashOfParent;
				num++;
			}
			this.m_HashCache = null;
			this.m_ActivationTime = (this.m_PendingActivationTime = 0f);
			base.ResetLiveChild();
		}

		protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
		{
			if (!this.PreviousStateIsValid)
			{
				this.ValidateInstructions();
			}
			List<CinemachineVirtualCameraBase> childCameras = base.ChildCameras;
			if (childCameras == null || childCameras.Count == 0)
			{
				this.m_ActivationTime = 0f;
				return null;
			}
			CinemachineVirtualCameraBase result = childCameras[0];
			if (this.AnimatedTarget == null || !this.AnimatedTarget.gameObject.activeSelf || this.AnimatedTarget.runtimeAnimatorController == null || this.LayerIndex < 0 || !this.AnimatedTarget.hasBoundPlayables || this.LayerIndex >= this.AnimatedTarget.layerCount)
			{
				this.m_ActivationTime = 0f;
				return result;
			}
			if (this.m_ActiveInstructionIndex < 0 || this.m_ActiveInstructionIndex >= this.Instructions.Length)
			{
				this.m_ActiveInstructionIndex = 0;
				this.m_ActivationTime = 0f;
			}
			if (!this.PreviousStateIsValid || this.m_PendingInstructionIndex < 0 || this.m_PendingInstructionIndex >= this.Instructions.Length)
			{
				this.m_PendingInstructionIndex = 0;
				this.m_PendingActivationTime = 0f;
			}
			int num;
			if (this.AnimatedTarget.IsInTransition(this.LayerIndex))
			{
				AnimatorStateInfo nextAnimatorStateInfo = this.AnimatedTarget.GetNextAnimatorStateInfo(this.LayerIndex);
				this.AnimatedTarget.GetNextAnimatorClipInfo(this.LayerIndex, this.m_ClipInfoList);
				num = this.GetClipHash(nextAnimatorStateInfo.fullPathHash, this.m_ClipInfoList);
			}
			else
			{
				AnimatorStateInfo currentAnimatorStateInfo = this.AnimatedTarget.GetCurrentAnimatorStateInfo(this.LayerIndex);
				this.AnimatedTarget.GetCurrentAnimatorClipInfo(this.LayerIndex, this.m_ClipInfoList);
				num = this.GetClipHash(currentAnimatorStateInfo.fullPathHash, this.m_ClipInfoList);
			}
			while (num != 0 && !this.m_InstructionDictionary.ContainsKey(num))
			{
				num = (this.m_StateParentLookup.ContainsKey(num) ? this.m_StateParentLookup[num] : 0);
			}
			int num2 = -1;
			if (this.m_InstructionDictionary.ContainsKey(num))
			{
				List<int> list = this.m_InstructionDictionary[num];
				int num3 = int.MinValue;
				for (int i = 0; i < list.Count; i++)
				{
					int num4 = list[i];
					CinemachineVirtualCameraBase cinemachineVirtualCameraBase = (num4 < this.Instructions.Length) ? this.Instructions[num4].Camera : null;
					if (cinemachineVirtualCameraBase != null && cinemachineVirtualCameraBase.isActiveAndEnabled && cinemachineVirtualCameraBase.Priority.Value > num3)
					{
						num2 = num4;
						num3 = cinemachineVirtualCameraBase.Priority.Value;
					}
				}
			}
			float currentTime = CinemachineCore.CurrentTime;
			if (num2 >= 0)
			{
				if (this.m_ActivationTime == 0f)
				{
					this.m_ActiveInstructionIndex = num2;
					this.m_ActivationTime = currentTime;
					this.m_PendingActivationTime = 0f;
				}
				else if (this.m_ActiveInstructionIndex != num2 && (this.m_PendingActivationTime == 0f || this.m_PendingInstructionIndex != num2))
				{
					this.m_PendingInstructionIndex = num2;
					this.m_PendingActivationTime = currentTime;
				}
			}
			if (this.m_PendingActivationTime != 0f && currentTime - this.m_PendingActivationTime > this.Instructions[this.m_PendingInstructionIndex].ActivateAfter && currentTime - this.m_ActivationTime > this.Instructions[this.m_ActiveInstructionIndex].MinDuration)
			{
				this.m_ActiveInstructionIndex = this.m_PendingInstructionIndex;
				this.m_ActivationTime = currentTime;
				this.m_PendingActivationTime = 0f;
			}
			if (this.m_ActivationTime != 0f)
			{
				return this.Instructions[this.m_ActiveInstructionIndex].Camera;
			}
			return result;
		}

		private int GetClipHash(int hash, List<AnimatorClipInfo> clips)
		{
			int num = -1;
			for (int i = 0; i < clips.Count; i++)
			{
				if (num < 0 || clips[i].weight > clips[num].weight)
				{
					num = i;
				}
			}
			if (num >= 0 && clips[num].weight > 0f)
			{
				hash = this.LookupFakeHash(hash, clips[num].clip);
			}
			return hash;
		}

		public void CancelWait()
		{
			if (this.m_PendingActivationTime != 0f && this.m_PendingInstructionIndex >= 0 && this.m_PendingInstructionIndex < this.Instructions.Length)
			{
				this.m_ActiveInstructionIndex = this.m_PendingInstructionIndex;
				this.m_ActivationTime = CinemachineCore.CurrentTime;
				this.m_PendingActivationTime = 0f;
			}
		}

		[Space]
		[Tooltip("The state machine whose state changes will drive this camera's choice of active child")]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_AnimatedTarget")]
		public Animator AnimatedTarget;

		[Tooltip("Which layer in the target state machine to observe")]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("m_LayerIndex")]
		public int LayerIndex;

		[Tooltip("The set of instructions associating cameras with states.  These instructions are used to choose the live child at any given moment")]
		[FormerlySerializedAs("m_Instructions")]
		public CinemachineStateDrivenCamera.Instruction[] Instructions;

		[HideInInspector]
		[SerializeField]
		[NoSaveDuringPlay]
		private List<CinemachineStateDrivenCamera.ParentHash> HashOfParent = new List<CinemachineStateDrivenCamera.ParentHash>();

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

		private float m_ActivationTime;

		private int m_ActiveInstructionIndex;

		private float m_PendingActivationTime;

		private int m_PendingInstructionIndex;

		private Dictionary<int, List<int>> m_InstructionDictionary;

		private Dictionary<int, int> m_StateParentLookup;

		private readonly List<AnimatorClipInfo> m_ClipInfoList = new List<AnimatorClipInfo>();

		private Dictionary<AnimationClip, List<CinemachineStateDrivenCamera.HashPair>> m_HashCache;

		[Serializable]
		public struct Instruction
		{
			[Tooltip("The full hash of the animation state")]
			[FormerlySerializedAs("m_FullHash")]
			public int FullHash;

			[Tooltip("The virtual camera to activate when the animation state becomes active")]
			[FormerlySerializedAs("m_VirtualCamera")]
			[ChildCameraProperty]
			public CinemachineVirtualCameraBase Camera;

			[Tooltip("How long to wait (in seconds) before activating the camera. This filters out very short state durations")]
			[FormerlySerializedAs("m_ActivateAfter")]
			public float ActivateAfter;

			[Tooltip("The minimum length of time (in seconds) to keep a camera active")]
			[FormerlySerializedAs("m_MinDuration")]
			public float MinDuration;
		}

		[Serializable]
		internal struct ParentHash
		{
			public int Hash;

			public int HashOfParent;
		}

		private struct HashPair
		{
			public int parentHash;

			public int hash;
		}
	}
}
