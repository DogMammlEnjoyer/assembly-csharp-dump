using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[AddComponentMenu("Cinemachine/Cinemachine Brain")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineBrain.html")]
	public class CinemachineBrain : MonoBehaviour, ICameraOverrideStack, ICinemachineMixer, ICinemachineCamera
	{
		private void OnValidate()
		{
			this.DefaultBlend.Time = Mathf.Max(0f, this.DefaultBlend.Time);
		}

		private void Reset()
		{
			this.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);
			this.CustomBlends = null;
			this.ShowDebugText = false;
			this.ShowCameraFrustum = true;
			this.IgnoreTimeScale = false;
			this.WorldUpOverride = null;
			this.ChannelMask = (OutputChannels)(-1);
			this.UpdateMethod = CinemachineBrain.UpdateMethods.SmartUpdate;
			this.BlendUpdateMethod = CinemachineBrain.BrainUpdateMethods.LateUpdate;
			this.LensModeOverride = new CinemachineBrain.LensModeOverrideSettings
			{
				DefaultMode = LensSettings.OverrideModes.Perspective
			};
		}

		private void Awake()
		{
			this.ControlledObject.TryGetComponent<Camera>(out this.m_OutputCamera);
		}

		private void Start()
		{
			this.m_LastFrameUpdated = -1;
			this.UpdateVirtualCameras(CameraUpdateManager.UpdateFilter.Late, -1f);
		}

		private void OnEnable()
		{
			this.m_BlendManager.OnEnable();
			this.m_BlendManager.LookupBlendDelegate = new CinemachineBlendDefinition.LookupBlendDelegate(this.LookupBlend);
			CinemachineBrain.s_ActiveBrains.Add(this);
			this.m_PhysicsCoroutine = base.StartCoroutine(this.AfterPhysics());
			SceneManager.sceneLoaded += this.OnSceneLoaded;
			SceneManager.sceneUnloaded += this.OnSceneUnloaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneLoaded -= this.OnSceneLoaded;
			SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
			CinemachineBrain.s_ActiveBrains.Remove(this);
			this.m_BlendManager.OnDisable();
			base.StopCoroutine(this.m_PhysicsCoroutine);
			UpdateTracker.ForgetContext(this);
			CameraUpdateManager.ForgetContext(this);
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (Time.frameCount == this.m_LastFrameUpdated && this.m_BlendManager.IsInitialized && this.UpdateMethod != CinemachineBrain.UpdateMethods.ManualUpdate)
			{
				this.DoNonFixedUpdate(Time.frameCount);
			}
		}

		private void OnSceneUnloaded(Scene scene)
		{
			if (Time.frameCount == this.m_LastFrameUpdated && this.m_BlendManager.IsInitialized && this.UpdateMethod != CinemachineBrain.UpdateMethods.ManualUpdate)
			{
				this.DoNonFixedUpdate(Time.frameCount);
			}
		}

		private void LateUpdate()
		{
			if (this.UpdateMethod != CinemachineBrain.UpdateMethods.ManualUpdate)
			{
				this.DoNonFixedUpdate(Time.frameCount);
			}
		}

		private IEnumerator AfterPhysics()
		{
			for (;;)
			{
				yield return this.m_WaitForFixedUpdate;
				this.DoFixedUpdate();
			}
			yield break;
		}

		public int SetCameraOverride(int overrideId, int priority, ICinemachineCamera camA, ICinemachineCamera camB, float weightB, float deltaTime)
		{
			return this.m_BlendManager.SetCameraOverride(overrideId, priority, camA, camB, weightB, deltaTime);
		}

		public void ReleaseCameraOverride(int overrideId)
		{
			this.m_BlendManager.ReleaseCameraOverride(overrideId);
		}

		public Vector3 DefaultWorldUp
		{
			get
			{
				if (!(this.WorldUpOverride != null))
				{
					return Vector3.up;
				}
				return this.WorldUpOverride.transform.up;
			}
		}

		public string Name
		{
			get
			{
				return base.name;
			}
		}

		public string Description
		{
			get
			{
				if (this.ActiveVirtualCamera == null)
				{
					return "(none)";
				}
				if (this.IsBlending)
				{
					return this.ActiveBlend.Description;
				}
				StringBuilder stringBuilder = CinemachineDebug.SBFromPool();
				stringBuilder.Append(this.ActiveVirtualCamera.Name);
				stringBuilder.Append(" ");
				stringBuilder.Append(this.ActiveVirtualCamera.Description);
				string result = stringBuilder.ToString();
				CinemachineDebug.ReturnToPool(stringBuilder);
				return result;
			}
		}

		public CameraState State
		{
			get
			{
				return this.m_CameraState;
			}
		}

		public bool IsValid
		{
			get
			{
				return this != null;
			}
		}

		public ICinemachineMixer ParentCamera
		{
			get
			{
				return null;
			}
		}

		public void UpdateCameraState(Vector3 up, float deltaTime)
		{
		}

		public void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
		{
		}

		public bool IsLiveChild(ICinemachineCamera cam, bool dominantChildOnly = false)
		{
			if (CinemachineCore.SoloCamera == cam || this.m_BlendManager.IsLive(cam))
			{
				return true;
			}
			ICinemachineMixer parentCamera = cam.ParentCamera;
			return parentCamera != null && parentCamera.IsLiveChild(cam, dominantChildOnly) && this.IsLiveChild(parentCamera, dominantChildOnly);
		}

		public static int ActiveBrainCount
		{
			get
			{
				return CinemachineBrain.s_ActiveBrains.Count;
			}
		}

		public static CinemachineBrain GetActiveBrain(int index)
		{
			return CinemachineBrain.s_ActiveBrains[index];
		}

		public GameObject ControlledObject
		{
			get
			{
				if (!(this.m_TargetOverride == null))
				{
					return this.m_TargetOverride;
				}
				return base.gameObject;
			}
			set
			{
				if (this.m_TargetOverride != value)
				{
					this.m_TargetOverride = value;
					this.ControlledObject.TryGetComponent<Camera>(out this.m_OutputCamera);
				}
			}
		}

		public Camera OutputCamera
		{
			get
			{
				if (this.m_OutputCamera == null && !Application.isPlaying)
				{
					this.ControlledObject.TryGetComponent<Camera>(out this.m_OutputCamera);
				}
				return this.m_OutputCamera;
			}
		}

		public ICinemachineCamera ActiveVirtualCamera
		{
			get
			{
				return CinemachineCore.SoloCamera ?? this.m_BlendManager.ActiveVirtualCamera;
			}
		}

		public void ResetState()
		{
			this.m_BlendManager.ResetRootFrame();
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
				return this.m_BlendManager.ActiveBlend;
			}
			set
			{
				this.m_BlendManager.ActiveBlend = value;
			}
		}

		public bool IsValidChannel(CinemachineVirtualCameraBase vcam)
		{
			return vcam != null && (vcam.OutputChannel & this.ChannelMask) > (OutputChannels)0;
		}

		public bool IsLiveInBlend(ICinemachineCamera cam)
		{
			if (this.m_BlendManager.IsLiveInBlend(cam))
			{
				return true;
			}
			ICinemachineMixer parentCamera = cam.ParentCamera;
			return parentCamera != null && parentCamera.IsLiveChild(cam, false) && this.IsLiveInBlend(parentCamera);
		}

		public void ManualUpdate(int currentFrame, float deltaTime)
		{
			float uniformDeltaTimeOverride = CinemachineCore.UniformDeltaTimeOverride;
			CinemachineCore.UniformDeltaTimeOverride = deltaTime;
			this.DoNonFixedUpdate(currentFrame);
			CinemachineCore.UniformDeltaTimeOverride = uniformDeltaTimeOverride;
		}

		public void ManualUpdate()
		{
			this.DoNonFixedUpdate(Time.frameCount);
		}

		private void DoNonFixedUpdate(int updateFrame)
		{
			CinemachineCore.CurrentUpdateFrame = updateFrame;
			this.m_LastFrameUpdated = updateFrame;
			float effectiveDeltaTime = this.GetEffectiveDeltaTime(false);
			if (Application.isPlaying && (this.UpdateMethod == CinemachineBrain.UpdateMethods.FixedUpdate || Time.inFixedTimeStep))
			{
				CameraUpdateManager.s_CurrentUpdateFilter = CameraUpdateManager.UpdateFilter.Fixed;
				if (this.BlendUpdateMethod != CinemachineBrain.BrainUpdateMethods.FixedUpdate && CinemachineCore.SoloCamera == null)
				{
					this.m_BlendManager.RefreshCurrentCameraState(this.DefaultWorldUp, this.GetEffectiveDeltaTime(true));
				}
			}
			else
			{
				CameraUpdateManager.UpdateFilter updateFilter = CameraUpdateManager.UpdateFilter.Late;
				if (this.UpdateMethod == CinemachineBrain.UpdateMethods.SmartUpdate)
				{
					UpdateTracker.OnUpdate(UpdateTracker.UpdateClock.Late, this);
					updateFilter = CameraUpdateManager.UpdateFilter.SmartLate;
				}
				this.UpdateVirtualCameras(updateFilter, effectiveDeltaTime);
			}
			if (!Application.isPlaying || this.BlendUpdateMethod != CinemachineBrain.BrainUpdateMethods.FixedUpdate)
			{
				this.m_BlendManager.UpdateRootFrame(this, this.TopCameraFromPriorityQueue(), this.DefaultWorldUp, effectiveDeltaTime);
			}
			this.m_BlendManager.ComputeCurrentBlend();
			if (!Application.isPlaying || this.BlendUpdateMethod != CinemachineBrain.BrainUpdateMethods.FixedUpdate)
			{
				this.ProcessActiveCamera(effectiveDeltaTime);
			}
		}

		private void DoFixedUpdate()
		{
			if (this.UpdateMethod == CinemachineBrain.UpdateMethods.FixedUpdate || this.UpdateMethod == CinemachineBrain.UpdateMethods.SmartUpdate)
			{
				CameraUpdateManager.UpdateFilter updateFilter = CameraUpdateManager.UpdateFilter.Fixed;
				if (this.UpdateMethod == CinemachineBrain.UpdateMethods.SmartUpdate)
				{
					UpdateTracker.OnUpdate(UpdateTracker.UpdateClock.Fixed, this);
					updateFilter = CameraUpdateManager.UpdateFilter.SmartFixed;
				}
				this.UpdateVirtualCameras(updateFilter, this.GetEffectiveDeltaTime(true));
			}
			if (this.BlendUpdateMethod == CinemachineBrain.BrainUpdateMethods.FixedUpdate)
			{
				this.m_BlendManager.UpdateRootFrame(this, this.TopCameraFromPriorityQueue(), this.DefaultWorldUp, Time.fixedDeltaTime);
				this.ProcessActiveCamera(Time.fixedDeltaTime);
			}
		}

		private float GetEffectiveDeltaTime(bool fixedDelta)
		{
			if (CinemachineCore.UniformDeltaTimeOverride >= 0f)
			{
				return CinemachineCore.UniformDeltaTimeOverride;
			}
			if (CinemachineCore.SoloCamera != null)
			{
				return Time.unscaledDeltaTime;
			}
			if (!Application.isPlaying)
			{
				return this.m_BlendManager.GetDeltaTimeOverride();
			}
			if (this.IgnoreTimeScale)
			{
				if (!fixedDelta)
				{
					return Time.unscaledDeltaTime;
				}
				return Time.fixedDeltaTime;
			}
			else
			{
				if (!fixedDelta)
				{
					return Time.deltaTime;
				}
				return Time.fixedDeltaTime;
			}
		}

		private void UpdateVirtualCameras(CameraUpdateManager.UpdateFilter updateFilter, float deltaTime)
		{
			CameraUpdateManager.s_CurrentUpdateFilter = updateFilter;
			CameraUpdateManager.UpdateAllActiveVirtualCameras((uint)this.ChannelMask, this.DefaultWorldUp, deltaTime, this);
			if (CinemachineCore.SoloCamera != null)
			{
				CinemachineCore.SoloCamera.UpdateCameraState(this.DefaultWorldUp, deltaTime);
			}
			this.m_BlendManager.RefreshCurrentCameraState(this.DefaultWorldUp, deltaTime);
			updateFilter = CameraUpdateManager.UpdateFilter.Late;
			if (Application.isPlaying)
			{
				if (this.UpdateMethod == CinemachineBrain.UpdateMethods.SmartUpdate)
				{
					updateFilter |= CameraUpdateManager.UpdateFilter.Smart;
				}
				else if (this.UpdateMethod == CinemachineBrain.UpdateMethods.FixedUpdate)
				{
					updateFilter = CameraUpdateManager.UpdateFilter.Fixed;
				}
			}
			CameraUpdateManager.s_CurrentUpdateFilter = updateFilter;
		}

		protected virtual ICinemachineCamera TopCameraFromPriorityQueue()
		{
			int virtualCameraCount = CameraUpdateManager.VirtualCameraCount;
			for (int i = 0; i < virtualCameraCount; i++)
			{
				CinemachineVirtualCameraBase virtualCamera = CameraUpdateManager.GetVirtualCamera(i);
				if (this.IsValidChannel(virtualCamera))
				{
					return virtualCamera;
				}
			}
			return null;
		}

		private CinemachineBlendDefinition LookupBlend(ICinemachineCamera fromKey, ICinemachineCamera toKey)
		{
			return CinemachineBlenderSettings.LookupBlend(fromKey, toKey, this.DefaultBlend, this.CustomBlends, this);
		}

		private void ProcessActiveCamera(float deltaTime)
		{
			if (CinemachineCore.SoloCamera != null)
			{
				CameraState state = CinemachineCore.SoloCamera.State;
				this.PushStateToUnityCamera(ref state);
				return;
			}
			if (this.m_BlendManager.ProcessActiveCamera(this, this.DefaultWorldUp, deltaTime) != null)
			{
				CameraState cameraState = this.m_BlendManager.CameraState;
				this.PushStateToUnityCamera(ref cameraState);
				return;
			}
			CameraState @default = CameraState.Default;
			Transform transform = this.ControlledObject.transform;
			@default.RawPosition = transform.position;
			@default.RawOrientation = transform.rotation;
			@default.Lens = LensSettings.FromCamera(this.m_OutputCamera);
			@default.BlendHint |= (CameraState.BlendHints)458752;
			this.PushStateToUnityCamera(ref @default);
		}

		private void PushStateToUnityCamera(ref CameraState state)
		{
			this.m_CameraState = state;
			Transform transform = this.ControlledObject.transform;
			Vector3 pos = transform.position;
			Quaternion rot = transform.rotation;
			if ((state.BlendHint & CameraState.BlendHints.NoPosition) == CameraState.BlendHints.Nothing)
			{
				pos = state.GetFinalPosition();
			}
			if ((state.BlendHint & CameraState.BlendHints.NoOrientation) == CameraState.BlendHints.Nothing)
			{
				rot = state.GetFinalOrientation();
			}
			transform.ConservativeSetPositionAndRotation(pos, rot);
			if ((state.BlendHint & CameraState.BlendHints.NoLens) == CameraState.BlendHints.Nothing)
			{
				Camera outputCamera = this.OutputCamera;
				if (outputCamera != null)
				{
					bool flag = outputCamera.usePhysicalProperties;
					outputCamera.nearClipPlane = state.Lens.NearClipPlane;
					outputCamera.farClipPlane = state.Lens.FarClipPlane;
					outputCamera.orthographicSize = state.Lens.OrthographicSize;
					outputCamera.fieldOfView = state.Lens.FieldOfView;
					if (this.LensModeOverride.Enabled)
					{
						if (state.Lens.ModeOverride != LensSettings.OverrideModes.None)
						{
							flag = state.Lens.IsPhysicalCamera;
							outputCamera.orthographic = (state.Lens.ModeOverride == LensSettings.OverrideModes.Orthographic);
						}
						else if (this.LensModeOverride.DefaultMode != LensSettings.OverrideModes.None)
						{
							flag = (this.LensModeOverride.DefaultMode == LensSettings.OverrideModes.Physical);
							outputCamera.orthographic = (this.LensModeOverride.DefaultMode == LensSettings.OverrideModes.Orthographic);
						}
						outputCamera.usePhysicalProperties = flag;
					}
					if (flag)
					{
						outputCamera.sensorSize = state.Lens.PhysicalProperties.SensorSize;
						outputCamera.gateFit = state.Lens.PhysicalProperties.GateFit;
						outputCamera.focalLength = Camera.FieldOfViewToFocalLength(state.Lens.FieldOfView, state.Lens.PhysicalProperties.SensorSize.y);
						outputCamera.lensShift = state.Lens.PhysicalProperties.LensShift;
						outputCamera.focusDistance = state.Lens.PhysicalProperties.FocusDistance;
						outputCamera.iso = state.Lens.PhysicalProperties.Iso;
						outputCamera.shutterSpeed = state.Lens.PhysicalProperties.ShutterSpeed;
						outputCamera.aperture = state.Lens.PhysicalProperties.Aperture;
						outputCamera.bladeCount = state.Lens.PhysicalProperties.BladeCount;
						outputCamera.curvature = state.Lens.PhysicalProperties.Curvature;
						outputCamera.barrelClipping = state.Lens.PhysicalProperties.BarrelClipping;
						outputCamera.anamorphism = state.Lens.PhysicalProperties.Anamorphism;
					}
				}
			}
			CinemachineCore.CameraUpdatedEvent.Invoke(this);
		}

		[Tooltip("When enabled, the current camera and blend are indicated in the game window, for debugging")]
		[FormerlySerializedAs("m_ShowDebugText")]
		public bool ShowDebugText;

		[Tooltip("When enabled, shows the camera's frustum at all times in the Scene view")]
		[FormerlySerializedAs("m_ShowCameraFrustum")]
		public bool ShowCameraFrustum = true;

		[Tooltip("When enabled, the cameras always respond in real-time to user input and damping, even if the game is running in slow motion")]
		[FormerlySerializedAs("m_IgnoreTimeScale")]
		public bool IgnoreTimeScale;

		[Tooltip("If set, this GameObject's Y axis defines the world-space Up vector for all the CinemachineCameras.  This is useful for instance in top-down game environments.  If not set, Up is world-space Y.  Setting this appropriately is important, because CinemachineCameras don't like looking straight up or straight down.")]
		[FormerlySerializedAs("m_WorldUpOverride")]
		public Transform WorldUpOverride;

		[Tooltip("The CinemachineBrain finds the highest-priority CinemachineCamera that outputs to any of the channels selected.  CinemachineCameras that do not output to one of these channels are ignored.  Use this in situations where multiple CinemachineBrains are needed (for example, Split-screen).")]
		public OutputChannels ChannelMask = (OutputChannels)(-1);

		[Tooltip("The update time for the CinemachineCameras.  Use FixedUpdate if all your targets are animated during FixedUpdate (e.g. RigidBodies), LateUpdate if all your targets are animated during the normal Update loop, and SmartUpdate if you want Cinemachine to do the appropriate thing on a per-target basis.  SmartUpdate is the recommended setting")]
		[FormerlySerializedAs("m_UpdateMethod")]
		public CinemachineBrain.UpdateMethods UpdateMethod = CinemachineBrain.UpdateMethods.SmartUpdate;

		[Tooltip("The update time for the Brain, i.e. when the blends are evaluated and the brain's transform is updated")]
		[FormerlySerializedAs("m_BlendUpdateMethod")]
		public CinemachineBrain.BrainUpdateMethods BlendUpdateMethod = CinemachineBrain.BrainUpdateMethods.LateUpdate;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineBrain.LensModeOverrideSettings LensModeOverride = new CinemachineBrain.LensModeOverrideSettings
		{
			DefaultMode = LensSettings.OverrideModes.Perspective
		};

		[Tooltip("The blend that is used in cases where you haven't explicitly defined a blend between two CinemachineCameras")]
		[FormerlySerializedAs("m_DefaultBlend")]
		public CinemachineBlendDefinition DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 2f);

		[Tooltip("This is the asset that contains custom settings for blends between specific CinemachineCameras in your Scene")]
		[FormerlySerializedAs("m_CustomBlends")]
		[EmbeddedBlenderSettingsProperty]
		public CinemachineBlenderSettings CustomBlends;

		private Camera m_OutputCamera;

		private GameObject m_TargetOverride;

		private int m_LastFrameUpdated;

		private Coroutine m_PhysicsCoroutine;

		private readonly WaitForFixedUpdate m_WaitForFixedUpdate = new WaitForFixedUpdate();

		private readonly BlendManager m_BlendManager = new BlendManager();

		private static readonly List<CinemachineBrain> s_ActiveBrains = new List<CinemachineBrain>();

		private CameraState m_CameraState;

		public enum UpdateMethods
		{
			FixedUpdate,
			LateUpdate,
			SmartUpdate,
			ManualUpdate
		}

		public enum BrainUpdateMethods
		{
			FixedUpdate,
			LateUpdate
		}

		[Serializable]
		public struct LensModeOverrideSettings
		{
			[Tooltip("If set, enables CinemachineCameras to override the lens mode of the camera")]
			public bool Enabled;

			[Tooltip("Lens mode to use when no mode override is active")]
			public LensSettings.OverrideModes DefaultMode;
		}
	}
}
