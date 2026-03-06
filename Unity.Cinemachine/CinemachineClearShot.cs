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
	[AddComponentMenu("Cinemachine/Cinemachine ClearShot")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineClearShot.html")]
	public class CinemachineClearShot : CinemachineCameraManagerBase
	{
		protected override void Reset()
		{
			base.Reset();
			this.ActivateAfter = 0f;
			this.MinDuration = 0f;
			this.RandomizeChoice = false;
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

		public override void OnTransitionFromCamera(ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
		{
			if (this.RandomizeChoice && !base.IsBlending)
			{
				this.m_RandomizedChildren = null;
			}
			base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
		}

		public void ResetRandomization()
		{
			this.m_RandomizedChildren = null;
			this.m_RandomizeNow = true;
		}

		protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
		{
			if (!this.PreviousStateIsValid)
			{
				this.m_ActivationTime = 0f;
				this.m_PendingActivationTime = 0f;
				this.m_PendingCamera = null;
				this.m_RandomizedChildren = null;
			}
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = base.LiveChild as CinemachineVirtualCameraBase;
			if (base.ChildCameras == null || base.ChildCameras.Count == 0)
			{
				this.m_ActivationTime = 0f;
				return null;
			}
			List<CinemachineVirtualCameraBase> list = base.ChildCameras;
			if (!this.RandomizeChoice)
			{
				this.m_RandomizedChildren = null;
			}
			else if (list.Count > 1)
			{
				if (this.m_RandomizedChildren == null)
				{
					this.m_RandomizedChildren = CinemachineClearShot.Randomize(list);
				}
				list = this.m_RandomizedChildren;
			}
			if (cinemachineVirtualCameraBase != null && (!cinemachineVirtualCameraBase.IsValid || !cinemachineVirtualCameraBase.gameObject.activeSelf))
			{
				cinemachineVirtualCameraBase = null;
			}
			CinemachineVirtualCameraBase cinemachineVirtualCameraBase2 = cinemachineVirtualCameraBase;
			for (int i = 0; i < list.Count; i++)
			{
				CinemachineVirtualCameraBase cinemachineVirtualCameraBase3 = list[i];
				if (cinemachineVirtualCameraBase3 != null && cinemachineVirtualCameraBase3.gameObject.activeInHierarchy && (cinemachineVirtualCameraBase2 == null || cinemachineVirtualCameraBase3.State.ShotQuality > cinemachineVirtualCameraBase2.State.ShotQuality || (cinemachineVirtualCameraBase3.State.ShotQuality == cinemachineVirtualCameraBase2.State.ShotQuality && cinemachineVirtualCameraBase3.Priority.Value > cinemachineVirtualCameraBase2.Priority.Value) || (this.RandomizeChoice && this.m_RandomizeNow && cinemachineVirtualCameraBase3 != cinemachineVirtualCameraBase && cinemachineVirtualCameraBase3.State.ShotQuality == cinemachineVirtualCameraBase2.State.ShotQuality && cinemachineVirtualCameraBase3.Priority.Value == cinemachineVirtualCameraBase2.Priority.Value)))
				{
					cinemachineVirtualCameraBase2 = cinemachineVirtualCameraBase3;
				}
			}
			this.m_RandomizeNow = false;
			float currentTime = CinemachineCore.CurrentTime;
			if (this.m_ActivationTime != 0f)
			{
				if (cinemachineVirtualCameraBase == cinemachineVirtualCameraBase2)
				{
					this.m_PendingActivationTime = 0f;
					this.m_PendingCamera = null;
					return cinemachineVirtualCameraBase2;
				}
				if (this.PreviousStateIsValid && this.m_PendingActivationTime != 0f && this.m_PendingCamera == cinemachineVirtualCameraBase2)
				{
					if (currentTime - this.m_PendingActivationTime > this.ActivateAfter && currentTime - this.m_ActivationTime > this.MinDuration)
					{
						this.m_RandomizedChildren = null;
						this.m_ActivationTime = currentTime;
						this.m_PendingActivationTime = 0f;
						this.m_PendingCamera = null;
						return cinemachineVirtualCameraBase2;
					}
					return cinemachineVirtualCameraBase;
				}
			}
			this.m_PendingActivationTime = 0f;
			this.m_PendingCamera = null;
			if (this.PreviousStateIsValid && this.m_ActivationTime > 0f && (this.ActivateAfter > 0f || currentTime - this.m_ActivationTime < this.MinDuration))
			{
				this.m_PendingCamera = cinemachineVirtualCameraBase2;
				this.m_PendingActivationTime = currentTime;
				return cinemachineVirtualCameraBase;
			}
			this.m_RandomizedChildren = null;
			this.m_ActivationTime = currentTime;
			return cinemachineVirtualCameraBase2;
		}

		private static List<CinemachineVirtualCameraBase> Randomize(List<CinemachineVirtualCameraBase> src)
		{
			List<CinemachineClearShot.Pair> list = new List<CinemachineClearShot.Pair>();
			for (int i = 0; i < src.Count; i++)
			{
				CinemachineClearShot.Pair item = new CinemachineClearShot.Pair
				{
					a = i,
					b = Random.Range(0f, 1000f)
				};
				list.Add(item);
			}
			list.Sort((CinemachineClearShot.Pair p1, CinemachineClearShot.Pair p2) => (int)p1.b - (int)p2.b);
			List<CinemachineVirtualCameraBase> list2 = new List<CinemachineVirtualCameraBase>(src.Count);
			for (int j = 0; j < src.Count; j++)
			{
				list2.Add(src[list[j].a]);
			}
			return list2;
		}

		[Tooltip("Wait this many seconds before activating a new child camera")]
		[FormerlySerializedAs("m_ActivateAfter")]
		public float ActivateAfter;

		[Tooltip("An active camera must be active for at least this many seconds")]
		[FormerlySerializedAs("m_MinDuration")]
		public float MinDuration;

		[Tooltip("If checked, camera choice will be randomized if multiple cameras are equally desirable.  Otherwise, child list order and child camera priority will be used.")]
		[FormerlySerializedAs("m_RandomizeChoice")]
		public bool RandomizeChoice;

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

		private float m_PendingActivationTime;

		private CinemachineVirtualCameraBase m_PendingCamera;

		private bool m_RandomizeNow;

		private List<CinemachineVirtualCameraBase> m_RandomizedChildren;

		private struct Pair
		{
			public int a;

			public float b;
		}
	}
}
