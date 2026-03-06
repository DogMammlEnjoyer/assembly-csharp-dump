using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Volume Settings")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineVolumeSettings.html")]
	public class CinemachineVolumeSettings : CinemachineExtension
	{
		public float CalculatedFocusDistance { get; private set; }

		public bool IsValid
		{
			get
			{
				return this.Profile != null && this.Profile.components.Count > 0;
			}
		}

		public void InvalidateCachedProfile()
		{
			if (this.m_extraStateCache == null)
			{
				this.m_extraStateCache = new List<CinemachineVolumeSettings.VcamExtraState>();
			}
			base.GetAllExtraStates<CinemachineVolumeSettings.VcamExtraState>(this.m_extraStateCache);
			for (int i = 0; i < this.m_extraStateCache.Count; i++)
			{
				this.m_extraStateCache[i].DestroyProfileCopy();
			}
		}

		private void OnValidate()
		{
			this.Weight = Mathf.Max(0f, this.Weight);
		}

		private void Reset()
		{
			this.Weight = 1f;
			this.FocusTracking = CinemachineVolumeSettings.FocusTrackingMode.None;
			this.FocusTarget = null;
			this.FocusOffset = 0f;
			this.Profile = null;
		}

		protected override void OnEnable()
		{
			this.InvalidateCachedProfile();
		}

		protected override void OnDestroy()
		{
			this.InvalidateCachedProfile();
			base.OnDestroy();
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == CinemachineCore.Stage.Finalize)
			{
				CinemachineVolumeSettings.VcamExtraState extraState = base.GetExtraState<CinemachineVolumeSettings.VcamExtraState>(vcam);
				if (!this.IsValid)
				{
					extraState.DestroyProfileCopy();
					return;
				}
				VolumeProfile volumeProfile = this.Profile;
				if (this.FocusTracking == CinemachineVolumeSettings.FocusTrackingMode.None)
				{
					extraState.DestroyProfileCopy();
				}
				else
				{
					if (extraState.ProfileCopy == null)
					{
						extraState.CreateProfileCopy(this.Profile);
					}
					volumeProfile = extraState.ProfileCopy;
					DepthOfField depthOfField;
					if (volumeProfile.TryGet<DepthOfField>(out depthOfField))
					{
						float num = this.FocusOffset;
						if (this.FocusTracking == CinemachineVolumeSettings.FocusTrackingMode.LookAtTarget)
						{
							num += (state.GetFinalPosition() - state.ReferenceLookAt).magnitude;
						}
						else
						{
							Transform transform = null;
							CinemachineVolumeSettings.FocusTrackingMode focusTracking = this.FocusTracking;
							if (focusTracking != CinemachineVolumeSettings.FocusTrackingMode.FollowTarget)
							{
								if (focusTracking == CinemachineVolumeSettings.FocusTrackingMode.CustomTarget)
								{
									transform = this.FocusTarget;
								}
							}
							else
							{
								transform = vcam.Follow;
							}
							if (transform != null)
							{
								num += (state.GetFinalPosition() - transform.position).magnitude;
							}
						}
						num = (this.CalculatedFocusDistance = Mathf.Max(0f, num));
						depthOfField.focusDistance.value = num;
						state.Lens.PhysicalProperties.FocusDistance = num;
						volumeProfile.isDirty = true;
					}
				}
				state.AddCustomBlendable(new CameraState.CustomBlendableItems.Item
				{
					Custom = volumeProfile,
					Weight = this.Weight
				});
			}
		}

		private static void OnCameraCut(ICinemachineCamera.ActivationEventParams evt)
		{
			if (!evt.IsCut)
			{
				return;
			}
			CinemachineBrain cinemachineBrain = evt.Origin as CinemachineBrain;
			Camera camera = (cinemachineBrain == null) ? null : cinemachineBrain.OutputCamera;
			UniversalAdditionalCameraData universalAdditionalCameraData;
			if (camera != null && camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
			{
				universalAdditionalCameraData.resetHistory = true;
			}
		}

		private static void ApplyPostFX(CinemachineBrain brain)
		{
			CameraState state = brain.State;
			int numCustomBlendables = state.GetNumCustomBlendables();
			List<Volume> dynamicBrainVolumes = CinemachineVolumeSettings.GetDynamicBrainVolumes(brain, numCustomBlendables);
			for (int i = 0; i < dynamicBrainVolumes.Count; i++)
			{
				dynamicBrainVolumes[i].weight = 0f;
				dynamicBrainVolumes[i].sharedProfile = null;
				dynamicBrainVolumes[i].profile = null;
			}
			Volume volume = null;
			int num = 0;
			for (int j = 0; j < numCustomBlendables; j++)
			{
				CameraState.CustomBlendableItems.Item customBlendable = state.GetCustomBlendable(j);
				VolumeProfile volumeProfile = customBlendable.Custom as VolumeProfile;
				if (!(volumeProfile == null))
				{
					Volume volume2 = dynamicBrainVolumes[j];
					if (volume == null)
					{
						volume = volume2;
					}
					volume2.sharedProfile = volumeProfile;
					volume2.isGlobal = true;
					volume2.priority = CinemachineVolumeSettings.s_VolumePriority - (float)(numCustomBlendables - j) - 1f;
					volume2.weight = customBlendable.Weight;
					num++;
				}
				if (num > 1)
				{
					volume.weight = 1f;
				}
			}
		}

		private static List<Volume> GetDynamicBrainVolumes(CinemachineBrain brain, int minVolumes)
		{
			GameObject gameObject = null;
			Transform transform = brain.transform;
			int childCount = transform.childCount;
			CinemachineVolumeSettings.sVolumes.Clear();
			int num = 0;
			while (gameObject == null && num < childCount)
			{
				GameObject gameObject2 = transform.GetChild(num).gameObject;
				if (gameObject2.hideFlags == HideFlags.HideAndDontSave)
				{
					gameObject2.GetComponents<Volume>(CinemachineVolumeSettings.sVolumes);
					if (CinemachineVolumeSettings.sVolumes.Count > 0)
					{
						gameObject = gameObject2;
					}
				}
				num++;
			}
			if (minVolumes > 0)
			{
				if (gameObject == null)
				{
					gameObject = new GameObject("__CMVolumes");
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					gameObject.transform.parent = transform;
				}
				UniversalAdditionalCameraData universalAdditionalCameraData;
				brain.gameObject.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData);
				if (universalAdditionalCameraData != null)
				{
					int num2 = universalAdditionalCameraData.volumeLayerMask;
					for (int i = 0; i < 32; i++)
					{
						if ((num2 & 1 << i) != 0)
						{
							gameObject.layer = i;
							break;
						}
					}
				}
				while (CinemachineVolumeSettings.sVolumes.Count < minVolumes)
				{
					CinemachineVolumeSettings.sVolumes.Add(gameObject.gameObject.AddComponent<Volume>());
				}
			}
			return CinemachineVolumeSettings.sVolumes;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void InitializeModule()
		{
			CinemachineCore.CameraUpdatedEvent.RemoveListener(new UnityAction<CinemachineBrain>(CinemachineVolumeSettings.ApplyPostFX));
			CinemachineCore.CameraUpdatedEvent.AddListener(new UnityAction<CinemachineBrain>(CinemachineVolumeSettings.ApplyPostFX));
			CinemachineCore.CameraActivatedEvent.RemoveListener(new UnityAction<ICinemachineCamera.ActivationEventParams>(CinemachineVolumeSettings.OnCameraCut));
			CinemachineCore.CameraActivatedEvent.AddListener(new UnityAction<ICinemachineCamera.ActivationEventParams>(CinemachineVolumeSettings.OnCameraCut));
		}

		public static float s_VolumePriority = 1000f;

		public float Weight = 1f;

		[Tooltip("If the profile has the appropriate overrides, will set the base focus distance to be the distance from the selected target to the camera.The Focus Offset field will then modify that distance.")]
		[FormerlySerializedAs("m_FocusTracking")]
		public CinemachineVolumeSettings.FocusTrackingMode FocusTracking;

		[Tooltip("The target to use if Focus Tracks Target is set to Custom Target")]
		[FormerlySerializedAs("m_FocusTarget")]
		public Transform FocusTarget;

		[Tooltip("Offset from target distance, to be used with Focus Tracks Target.  Offsets the sharpest point away from the focus target.")]
		[FormerlySerializedAs("m_FocusOffset")]
		public float FocusOffset;

		[Tooltip("This profile will be applied whenever this virtual camera is live")]
		[FormerlySerializedAs("m_Profile")]
		public VolumeProfile Profile;

		private List<CinemachineVolumeSettings.VcamExtraState> m_extraStateCache;

		private const string sVolumeOwnerName = "__CMVolumes";

		private static List<Volume> sVolumes = new List<Volume>();

		public enum FocusTrackingMode
		{
			None,
			LookAtTarget,
			FollowTarget,
			CustomTarget,
			Camera
		}

		private class VcamExtraState : CinemachineExtension.VcamExtraStateBase
		{
			public void CreateProfileCopy(VolumeProfile source)
			{
				this.DestroyProfileCopy();
				VolumeProfile volumeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
				int num = 0;
				while (source != null && num < source.components.Count)
				{
					VolumeComponent item = Object.Instantiate<VolumeComponent>(source.components[num]);
					volumeProfile.components.Add(item);
					volumeProfile.isDirty = true;
					num++;
				}
				this.ProfileCopy = volumeProfile;
			}

			public void DestroyProfileCopy()
			{
				if (this.ProfileCopy != null)
				{
					RuntimeUtility.DestroyObject(this.ProfileCopy);
				}
				this.ProfileCopy = null;
			}

			public VolumeProfile ProfileCopy;
		}
	}
}
