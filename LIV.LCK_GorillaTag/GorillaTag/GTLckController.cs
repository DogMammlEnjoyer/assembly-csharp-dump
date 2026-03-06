using System;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Smoothing;
using Liv.Lck.Tablet;
using Liv.Lck.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	[DefaultExecutionOrder(-790)]
	public class GTLckController : MonoBehaviour
	{
		public event GTLckController.CameraModeDelegate OnCameraModeChanged;

		public event UnityAction<bool> OnHorizontalModeChanged = delegate(bool <p0>)
		{
		};

		public GtSelectorsGroup GTSelectorsGroup { get; private set; }

		public float ThirdPersonHeightAngle { get; private set; } = 25f;

		public float ThirdPersonSideAngle { get; private set; }

		public bool IsThirdPersonFront { get; private set; } = true;

		public bool HorizontalMode
		{
			get
			{
				return this._isHorizontalMode;
			}
		}

		public CameraMode CurrentCameraMode
		{
			get
			{
				return this._currentCameraMode;
			}
		}

		private void OnValidate()
		{
			if (this._qualityConfig != null && !(this._qualityConfig is ILckQualityConfig))
			{
				Debug.LogError("LCK Quality Config must implement ILckQualityConfig interface");
			}
		}

		private void OnEnable()
		{
			if (!LckDiContainer.Instance.HasService<ILckService>())
			{
				LckServiceInitializer.ConfigureServices(LckDiContainer.Instance, (ILckQualityConfig)this._qualityConfig, null);
				this._lckService = LckService.GetService().Result;
			}
			this.CheckMicPermission();
			this.GTSelectorsGroup.onCameraModeChanged.AddListener(new UnityAction<CameraMode>(this._gtSettingsSectionGroup.EvaluateMode));
			this.GTSelectorsGroup.onCameraModeChanged.AddListener(new UnityAction<CameraMode>(this.ChangeCameraMode));
			this._selfieFovCounter.onValueChanged.AddListener(new UnityAction<int>(this.ProcessSelfieFov));
			this._selfieSmoothnessCounter.onValueChanged.AddListener(new UnityAction<int>(this.ProcessSelfieSmoothness));
			this._selfieFlipButton.onTapStarted.AddListener(new UnityAction(this.ProcessSelfieFlip));
			if (this._tabletFollowsPlayerToggle)
			{
				this._tabletFollowsPlayerToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SetFollowModeState));
			}
			this._firstPersonFovCounter.onValueChanged.AddListener(new UnityAction<int>(this.ProcessFirstPersonFov));
			this._firstPersonSmoothnessCounter.onValueChanged.AddListener(new UnityAction<int>(this.ProcessFirstPersonSmoothness));
			this._thirdPersonFovCounter.onValueChanged.AddListener(new UnityAction<int>(this.ProcessThirdPersonFov));
			if (this._thirdPersonSmoothnessCounter)
			{
				this._thirdPersonSmoothnessCounter.onValueChanged.AddListener(new UnityAction<int>(this.ProcessThirdPersonSmoothness));
			}
			this._thirdPersonDistanceCounter.onValueChanged.AddListener(new UnityAction<int>(this.ProcessThirdPersonDistance));
			if (this._thirdPersonPositionToggle)
			{
				this._thirdPersonPositionToggle.onValueChanged.AddListener(new UnityAction<bool>(this.ProcessThirdPersonPosition));
			}
			this._recordButton.onPressed += this.ToggleRecording;
			this._changeOrientation.onTap.AddListener(new UnityAction(this.ToggleOrientation));
			this._microphoneButton.onTap.AddListener(new UnityAction<UnityAction<bool>>(this.ToggleMicrophoneRecording));
			if (this._droneSystem)
			{
				this._droneSystem.OnRequestDroneModeState += this.ProcessDroneModeStateChangeRequest;
			}
		}

		private void OnQualityOptionSelected(QualityOption qualityOption)
		{
			CameraTrackDescriptor descriptorForCurrentOrientation = this.GetDescriptorForCurrentOrientation(qualityOption.RecordingCameraTrackDescriptor);
			this._lckService.SetTrackDescriptor(LckCaptureType.Recording, descriptorForCurrentOrientation);
			CameraTrackDescriptor descriptorForCurrentOrientation2 = this.GetDescriptorForCurrentOrientation(qualityOption.StreamingCameraTrackDescriptor);
			this._lckService.SetTrackDescriptor(LckCaptureType.Streaming, descriptorForCurrentOrientation2);
		}

		public void SetCameraMode(CameraMode mode)
		{
			this.ChangeCameraMode(mode);
		}

		private void ProcessDroneModeStateChangeRequest(bool isActive)
		{
			this._droneModeTabletUIAppearance.IsDroneModeActive = isActive;
			this.ChangeCameraMode(isActive ? CameraMode.Drone : CameraMode.Selfie);
		}

		private CameraTrackDescriptor GetDescriptorForCurrentOrientation(CameraTrackDescriptor descriptor)
		{
			CameraResolutionDescriptor cameraResolutionDescriptor = descriptor.CameraResolutionDescriptor;
			descriptor.CameraResolutionDescriptor = cameraResolutionDescriptor.GetResolutionInOrientation(this._currentCameraOrientation);
			return descriptor;
		}

		private bool IsQuest2()
		{
			return false;
		}

		private void OnDisable()
		{
			this.GTSelectorsGroup.onCameraModeChanged.RemoveListener(new UnityAction<CameraMode>(this._gtSettingsSectionGroup.EvaluateMode));
			this.GTSelectorsGroup.onCameraModeChanged.RemoveListener(new UnityAction<CameraMode>(this.ChangeCameraMode));
			this._selfieFovCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.ProcessSelfieFov));
			this._selfieSmoothnessCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.ProcessSelfieSmoothness));
			this._selfieFlipButton.onTapStarted.RemoveListener(new UnityAction(this.ProcessSelfieFlip));
			if (this._tabletFollowsPlayerToggle)
			{
				this._tabletFollowsPlayerToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.SetFollowModeState));
			}
			this._firstPersonFovCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.ProcessFirstPersonFov));
			this._firstPersonSmoothnessCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.ProcessFirstPersonSmoothness));
			this._thirdPersonFovCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.ProcessThirdPersonFov));
			if (this._thirdPersonSmoothnessCounter)
			{
				this._thirdPersonSmoothnessCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.ProcessThirdPersonSmoothness));
			}
			this._thirdPersonDistanceCounter.onValueChanged.RemoveListener(new UnityAction<int>(this.ProcessThirdPersonDistance));
			if (this._thirdPersonPositionToggle)
			{
				this._thirdPersonPositionToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.ProcessThirdPersonPosition));
			}
			this._recordButton.onPressed -= this.ToggleRecording;
			this._changeOrientation.onTap.RemoveListener(new UnityAction(this.ToggleOrientation));
			this._microphoneButton.onTap.RemoveListener(new UnityAction<UnityAction<bool>>(this.ToggleMicrophoneRecording));
			if (this._droneSystem)
			{
				this._droneSystem.OnRequestDroneModeState -= this.ProcessDroneModeStateChangeRequest;
			}
		}

		private void OnDestroy()
		{
			LckQualitySelector qualitySelector = this._qualitySelector;
			qualitySelector.OnQualityOptionChanged = (Action<QualityOption>)Delegate.Remove(qualitySelector.OnQualityOptionChanged, new Action<QualityOption>(this.OnQualityOptionSelected));
			if (this._lckService != null)
			{
				if (this._lckService.IsRecording().Result)
				{
					this._lckService.StopRecording();
				}
				this._lckService.OnRecordingStarted -= this.OnCaptureStart;
				this._lckService.OnRecordingStopped -= this.OnCaptureStopped;
				this._lckService.OnStreamingStarted -= this.OnCaptureStart;
				this._lckService.OnStreamingStopped -= this.OnCaptureStopped;
			}
		}

		private void Start()
		{
			LckQualitySelector qualitySelector = this._qualitySelector;
			qualitySelector.OnQualityOptionChanged = (Action<QualityOption>)Delegate.Combine(qualitySelector.OnQualityOptionChanged, new Action<QualityOption>(this.OnQualityOptionSelected));
			this._qualitySelector.InitializeOptions((this._qualityConfig as ILckQualityConfig).GetQualityOptionsForSystem());
			this.SetupCamera();
			this._lckService.OnRecordingStarted += this.OnCaptureStart;
			this._lckService.OnRecordingStopped += this.OnCaptureStopped;
			this._lckService.OnStreamingStarted += this.OnCaptureStart;
			this._lckService.OnStreamingStopped += this.OnCaptureStopped;
		}

		private void SetupCamera()
		{
			this.FindPlayerReferences();
			this.SetUpSelfieCamera();
			this.SetSelfieCameraOrientation(this._isSelfieFront ? this._selfieFrontTransform : this._selfieBackTransform);
			this._monitorTransform.sizeDelta = new Vector2(1109f, 624f);
			this._monitorTransform.localScale = Vector3.one;
			LckResult lckResult = this._lckService.SetMicrophoneCaptureActive(true);
			if (!lckResult.Success)
			{
				Debug.LogError(string.Format("LCK Could not enable microphone capture: {0}", lckResult.Error));
			}
		}

		private void Update()
		{
			switch (this._currentCameraMode)
			{
			case CameraMode.Selfie:
			case CameraMode.Drone:
				return;
			case CameraMode.FirstPerson:
				this.ProcessFirstCameraPosition();
				return;
			case CameraMode.ThirdPerson:
				this.ProcessThirdCameraPosition();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void SetSelfieCameraOrientation(GtCameraModeTransform t)
		{
			this._selfieStabilizer.transform.localPosition = t.position + this._selfieFollowModeOffset;
			this._selfieStabilizer.transform.localRotation = Quaternion.Euler(t.rotation);
			this._selfieStabilizer.ReachTargetInstantly();
		}

		private void ProcessFirstCameraPosition()
		{
			this._firstPersonStabilizer.transform.position = this._playerCamera.transform.position + this._playerCamera.transform.forward * 0.05f;
			this._firstPersonStabilizer.transform.rotation = this._playerCamera.transform.rotation;
			if (this._justTransitioned)
			{
				this._firstPersonStabilizer.ReachTargetInstantly();
				this._justTransitioned = false;
			}
		}

		public void UpdateThirdPersonHeightAngle(float value)
		{
			this.ThirdPersonHeightAngle = value;
		}

		public void UpdateThirdPersonSideAngle(float value)
		{
			this.ThirdPersonSideAngle = value;
		}

		private void ProcessThirdCameraPosition()
		{
			this._thirdPersonCameraBehaviour.heightOffsetAngle = this._thirdPersonHeightAngle;
			if (this._justTransitioned)
			{
				this._thirdPersonCameraBehaviour.UpdateCameraWithoutSmoothing();
				this._justTransitioned = false;
			}
		}

		private void FindPlayerReferences()
		{
			this._playerCamera = Camera.main;
			Transform playerHead;
			if (GtTag.TryGetTransform(GtTagType.Player, out playerHead))
			{
				this._playerHead = playerHead;
				return;
			}
			this._playerHead = Camera.main.transform;
		}

		private void CheckMicPermission()
		{
		}

		private void ChangeCameraMode(CameraMode newMode)
		{
			this._currentCameraMode = newMode;
			this._justTransitioned = true;
			float fov = this.CalculateCorrectFOV(this.GetCurrentModeFOV());
			this.SetFOV(this._currentCameraMode, fov);
			this.SetMonitorScale(newMode);
			if (this._coconutCamera)
			{
				this._coconutCamera.SetVisualsActive(newMode == CameraMode.ThirdPerson);
			}
			switch (newMode)
			{
			case CameraMode.Selfie:
			{
				this.SetUpSelfieCamera();
				GTLckController.CameraModeDelegate onCameraModeChanged = this.OnCameraModeChanged;
				if (onCameraModeChanged == null)
				{
					return;
				}
				onCameraModeChanged(newMode, this._selfieCamera);
				return;
			}
			case CameraMode.FirstPerson:
			{
				this.SetUpFirstPersonCamera();
				GTLckController.CameraModeDelegate onCameraModeChanged2 = this.OnCameraModeChanged;
				if (onCameraModeChanged2 == null)
				{
					return;
				}
				onCameraModeChanged2(newMode, this._firstPersonCamera);
				return;
			}
			case CameraMode.ThirdPerson:
			{
				this.SetUpThirdPersonCamera();
				GTLckController.CameraModeDelegate onCameraModeChanged3 = this.OnCameraModeChanged;
				if (onCameraModeChanged3 == null)
				{
					return;
				}
				onCameraModeChanged3(newMode, this._thirdPersonCamera);
				return;
			}
			case CameraMode.Drone:
			{
				this.SetUpDroneCamera();
				GTLckController.CameraModeDelegate onCameraModeChanged4 = this.OnCameraModeChanged;
				if (onCameraModeChanged4 == null)
				{
					return;
				}
				onCameraModeChanged4(newMode, this._droneSystem.GetLckCamera());
				return;
			}
			default:
				throw new ArgumentOutOfRangeException("newMode", newMode, null);
			}
		}

		private void SetUpDroneCamera()
		{
			if (this._droneSystem == null)
			{
				Debug.LogError("LCK Drone System is not set");
				return;
			}
			Vector3 normalized = Vector3.Scale(this._playerCamera.transform.forward, new Vector3(1f, 0f, 1f)).normalized;
			Vector3 position = this._playerCamera.transform.position + normalized * 2f;
			Quaternion rotation = Quaternion.LookRotation(-normalized, Vector3.up);
			this._droneSystem.SetDronePositionAndRotation(position, rotation);
			this.SetActiveLckCamera(this._droneSystem.GetLckCamera().CameraId);
			GTLckController.CameraModeDelegate onCameraModeChanged = this.OnCameraModeChanged;
			if (onCameraModeChanged == null)
			{
				return;
			}
			onCameraModeChanged(CameraMode.Drone, this._droneSystem.GetLckCamera());
		}

		private void SetUpSelfieCamera()
		{
			this.SetActiveLckCamera(this._selfieCamera.CameraId);
		}

		private void SetUpFirstPersonCamera()
		{
			this.SetActiveLckCamera(this._firstPersonCamera.CameraId);
		}

		private void SetUpThirdPersonCamera()
		{
			this.SetActiveLckCamera(this._thirdPersonCamera.CameraId);
		}

		private void SetActiveLckCamera(string cameraId)
		{
			if (this._lckService == null)
			{
				Debug.LogError("LCK Could not get Service");
				return;
			}
			this._lckService.SetActiveCamera(cameraId, null);
		}

		private void SetMonitorScale(CameraMode mode)
		{
			Vector3 vector = new Vector3(-1f, 1f, 1f);
			Vector3 one = Vector3.one;
			switch (mode)
			{
			case CameraMode.Selfie:
				this._monitorTransform.localScale = (this._isSelfieFront ? one : vector);
				return;
			case CameraMode.FirstPerson:
				this._monitorTransform.localScale = vector;
				return;
			case CameraMode.ThirdPerson:
				this._monitorTransform.localScale = vector;
				return;
			case CameraMode.Drone:
				this._monitorTransform.localScale = vector;
				return;
			default:
				return;
			}
		}

		public Camera GetActiveCamera()
		{
			switch (this._currentCameraMode)
			{
			case CameraMode.Selfie:
				return this._selfieCamera.GetCameraComponent();
			case CameraMode.FirstPerson:
				return this._firstPersonCamera.GetCameraComponent();
			case CameraMode.ThirdPerson:
				return this._thirdPersonCamera.GetCameraComponent();
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void SetFollowModeState(bool isFollowing)
		{
			this._isTabletFollowingPlayer = isFollowing;
			this._selfieFollowModeOffset = (isFollowing ? this._selfieFollowModeTransform.position : Vector3.zero);
			this.SetSelfieCameraOrientation(this._isSelfieFront ? this._selfieFrontTransform : this._selfieBackTransform);
			this._selfieStabilizer.AffectPosition = !isFollowing;
			this._selfieStabilizer.AffectRotation = !isFollowing;
			if (!isFollowing)
			{
				this.ProcessSelfieSmoothness(this._selfieSmoothnessCounter.Value);
				this._selfieStabilizer.ReachTargetInstantly();
			}
		}

		private void ProcessSelfieFov(int value)
		{
			this._selfieCamera.GetCameraComponent().fieldOfView = this.CalculateCorrectFOV((float)value);
			Action<CameraMode> onFOVUpdated = this.OnFOVUpdated;
			if (onFOVUpdated == null)
			{
				return;
			}
			onFOVUpdated(CameraMode.Selfie);
		}

		private void ProcessSelfieSmoothness(int value)
		{
			this._selfieSmoothness = (float)value;
			if (this._isTabletFollowingPlayer)
			{
				return;
			}
			this._selfieStabilizer.RotationalSmoothing = (200f + this._selfieSmoothness) * 0.001f * 0.6f;
		}

		private void ProcessSelfieFlip()
		{
			this._isSelfieFront = !this._isSelfieFront;
			this.SetMonitorScale(CameraMode.Selfie);
			this.SetSelfieCameraOrientation(this._isSelfieFront ? this._selfieFrontTransform : this._selfieBackTransform);
		}

		private void ProcessFirstPersonFov(int value)
		{
			this._firstPersonCamera.GetCameraComponent().fieldOfView = this.CalculateCorrectFOV((float)value);
			Action<CameraMode> onFOVUpdated = this.OnFOVUpdated;
			if (onFOVUpdated == null)
			{
				return;
			}
			onFOVUpdated(CameraMode.FirstPerson);
		}

		private void ProcessFirstPersonSmoothness(int value)
		{
			this._firstPersonStabilizer.PositionalSmoothing = (float)value * 0.001f * 0.3f;
			this._firstPersonStabilizer.RotationalSmoothing = (float)value * 0.001f * 0.8f;
		}

		private void ProcessThirdPersonFov(int value)
		{
			this._thirdPersonCamera.GetCameraComponent().fieldOfView = this.CalculateCorrectFOV((float)value);
		}

		private void ProcessThirdPersonSmoothness(int value)
		{
			this._thirdPersonCameraBehaviour.rotationalSmoothness = (float)value * 0.001f * 0.99f;
		}

		private void ProcessThirdPersonDistance(int value)
		{
			this._thirdPersonCameraBehaviour.distance = (float)value;
			this._justTransitioned = true;
		}

		private void ProcessThirdPersonPosition(bool isFirstSelected)
		{
			if (isFirstSelected != this._thirdPersonCameraBehaviour.front)
			{
				this._justTransitioned = true;
			}
			this._thirdPersonCameraBehaviour.front = isFirstSelected;
			this.SetMonitorScale(CameraMode.ThirdPerson);
		}

		private void ToggleRecording()
		{
			if (this._lckService == null)
			{
				Debug.LogWarning("LCK Could not get Service");
				return;
			}
			if (this._lckService.IsRecording().Result)
			{
				this._lckService.StopRecording();
				return;
			}
			this.SetOrientationQualityAndTopButtonsIsDisabledState(true);
			this._lckService.StartRecording();
		}

		private void OnCaptureStart(LckResult result)
		{
			if (!result.Success)
			{
				this.SetOrientationQualityAndTopButtonsIsDisabledState(false);
				return;
			}
			if (this._lckService.IsStreaming().Result)
			{
				this._coconutCamera.SetRecordingState(true);
			}
		}

		private void OnCaptureStopped(LckResult result)
		{
			this.SetOrientationQualityAndTopButtonsIsDisabledState(false);
			if (!this._lckService.IsStreaming().Result)
			{
				this._coconutCamera.SetRecordingState(false);
			}
		}

		public void SetOrientationQualityAndTopButtonsIsDisabledState(bool state)
		{
			this._topButtonsController.SetTopButtonsIsDisabledState(state);
			this._qualitySelector.SetQualityButtonIsDisabledState(state);
			this._changeOrientation.SetDisabled(state);
		}

		public bool StopRecording()
		{
			if (this._lckService == null)
			{
				Debug.LogWarning("LCK Could not get Service");
				return false;
			}
			LckResult<bool> lckResult = this._lckService.IsRecording();
			if (!lckResult.Success || !lckResult.Result)
			{
				return false;
			}
			this.ToggleRecording();
			return true;
		}

		public void ToggleMicrophoneRecording(UnityAction<bool> isOn)
		{
			if (this._lckService == null)
			{
				Debug.LogError("LCK Could not get Service");
				return;
			}
			this._micState = !this._micState;
			LckResult lckResult = this._lckService.SetMicrophoneCaptureActive(this._micState);
			isOn(this._micState);
			if (!lckResult.Success)
			{
				LckError? error = lckResult.Error;
				LckError lckError = LckError.MicrophonePermissionDenied;
				if (error.GetValueOrDefault() == lckError & error != null)
				{
					this._microphoneButton.SetActiveState(false);
				}
			}
		}

		private void ToggleOrientation()
		{
			if (this._lckService.IsCapturing().Result)
			{
				return;
			}
			this._isHorizontalMode = !this._isHorizontalMode;
			if (this._lckService.IsCapturing().Result)
			{
				return;
			}
			this._currentCameraOrientation = ((this._currentCameraOrientation == LckCameraOrientation.Landscape) ? LckCameraOrientation.Portrait : LckCameraOrientation.Landscape);
			this._lckService.SetCameraOrientation(this._currentCameraOrientation);
			this._monitorTransform.sizeDelta = ((this._currentCameraOrientation == LckCameraOrientation.Landscape) ? new Vector2(1109f, 624f) : new Vector2(352f, 624f));
			float fov = this.CalculateCorrectFOV(this.GetCurrentModeFOV());
			this.SetFOV(this._currentCameraMode, fov);
			UnityAction<bool> onHorizontalModeChanged = this.OnHorizontalModeChanged;
			if (onHorizontalModeChanged == null)
			{
				return;
			}
			onHorizontalModeChanged(this._isHorizontalMode);
		}

		private CameraTrackDescriptor GenerateVerticalCameraTrackDescriptor()
		{
			return new CameraTrackDescriptor(new CameraResolutionDescriptor(this._currentTrackDescriptor.CameraResolutionDescriptor.Height, this._currentTrackDescriptor.CameraResolutionDescriptor.Width), this._currentTrackDescriptor.Bitrate, this._currentTrackDescriptor.Framerate, this._currentTrackDescriptor.AudioBitrate);
		}

		private float GetCurrentModeFOV()
		{
			switch (this._currentCameraMode)
			{
			case CameraMode.Selfie:
				return (float)this._selfieFovCounter.Value;
			case CameraMode.FirstPerson:
				return (float)this._firstPersonFovCounter.Value;
			case CameraMode.ThirdPerson:
				return (float)this._thirdPersonFovCounter.Value;
			case CameraMode.Drone:
				return 0f;
			default:
				throw new Exception("Invalid Camera Mode");
			}
		}

		private void SetFOV(CameraMode mode, float fov)
		{
			switch (mode)
			{
			case CameraMode.Selfie:
				this._selfieCamera.GetCameraComponent().fieldOfView = fov;
				break;
			case CameraMode.FirstPerson:
				this._firstPersonCamera.GetCameraComponent().fieldOfView = fov;
				break;
			case CameraMode.ThirdPerson:
				this._thirdPersonCamera.GetCameraComponent().fieldOfView = fov;
				break;
			}
			Action<CameraMode> onFOVUpdated = this.OnFOVUpdated;
			if (onFOVUpdated == null)
			{
				return;
			}
			onFOVUpdated(mode);
		}

		private float CalculateCorrectFOV(float incomingVerticalFOV)
		{
			if (this._currentCameraOrientation == LckCameraOrientation.Landscape)
			{
				return incomingVerticalFOV;
			}
			CameraResolutionDescriptor cameraResolutionDescriptor = this._lckService.GetDescriptor().Result.cameraTrackDescriptor.CameraResolutionDescriptor;
			float aspectRatio = cameraResolutionDescriptor.Height / cameraResolutionDescriptor.Width;
			return Camera.VerticalToHorizontalFieldOfView(incomingVerticalFOV, aspectRatio);
		}

		internal void SetOverlayEnabled(bool value)
		{
			this._isOverlayActive = value;
			this.SetMonitorScale(this._currentCameraMode);
		}

		public void ApplyCameraSettings(GtCameraDockSettings settings)
		{
			CameraMode enforcedMode = settings.GetEnforcedMode();
			if (settings.forceCameraFacing)
			{
				this._isSelfieFront = settings.isFront;
			}
			this.GTSelectorsGroup.Select(enforcedMode);
			if (this._isSelfieFront != settings.isFront)
			{
				this._selfieFlipButton.onTapStarted.Invoke();
			}
			if (settings.forceOrientation && this._isHorizontalMode != settings.landscapeMode)
			{
				this.ToggleOrientation();
			}
			if (settings.forceFov)
			{
				this.SetFOV(enforcedMode, settings.fov);
				if (enforcedMode == CameraMode.Selfie)
				{
					this._selfieFovCounter.Value = (int)settings.fov;
				}
			}
			this.SetMonitorScale(enforcedMode);
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private GtUiSettings _settings;

		[SerializeField]
		private GtSettingsSectionGroup _gtSettingsSectionGroup;

		[SerializeField]
		public GtColliderTriggerProcessorsGroup GtColliderTriggerProcessorsGroup;

		[Space(10f)]
		[Header("Camera Settings References")]
		[Header("Selfie")]
		[SerializeField]
		private GtCounter _selfieFovCounter;

		[SerializeField]
		private GtCounter _selfieSmoothnessCounter;

		[SerializeField]
		private GtScreenButton _selfieFlipButton;

		[SerializeField]
		private GtToggle _tabletFollowsPlayerToggle;

		[Header("First Person")]
		[SerializeField]
		private GtCounter _firstPersonFovCounter;

		[SerializeField]
		private GtCounter _firstPersonSmoothnessCounter;

		[Header("Third Person")]
		[SerializeField]
		private GtCounter _thirdPersonFovCounter;

		[SerializeField]
		private GtCounter _thirdPersonSmoothnessCounter;

		[SerializeField]
		private GtCounter _thirdPersonDistanceCounter;

		[SerializeField]
		private GtToggle _thirdPersonPositionToggle;

		[Space(10f)]
		[Header("Camera Modes")]
		[Header("Selfie")]
		[SerializeField]
		private LckCamera _selfieCamera;

		[SerializeField]
		private LckStabilizer _selfieStabilizer;

		[SerializeField]
		private GtCameraModeTransform _selfieFrontTransform;

		[SerializeField]
		private GtCameraModeTransform _selfieBackTransform;

		[SerializeField]
		private GtCameraModeTransform _selfieFollowModeTransform;

		[Header("First Person")]
		[SerializeField]
		private LckCamera _firstPersonCamera;

		[SerializeField]
		private LckStabilizer _firstPersonStabilizer;

		[Header("Third Person")]
		[SerializeField]
		private LckCamera _thirdPersonCamera;

		[SerializeField]
		private CoconutCamera _coconutCamera;

		[SerializeField]
		private GtThirdPersonCameraBehaviour _thirdPersonCameraBehaviour;

		public Action<CameraMode> OnFOVUpdated;

		[Header("Drone")]
		[SerializeField]
		private DroneSystem _droneSystem;

		[SerializeField]
		private GtDroneModeTabletUIAppearance _droneModeTabletUIAppearance;

		[Space(10f)]
		[Header("Recording And Streaming Bar")]
		[Header("References")]
		[SerializeField]
		private GtRecordButton _recordButton;

		[SerializeReference]
		private ScriptableObject _qualityConfig;

		[SerializeField]
		private LckQualitySelector _qualitySelector;

		[SerializeField]
		private GtButton _changeOrientation;

		[SerializeField]
		private GtAudioButton _microphoneButton;

		[SerializeField]
		private RectTransform _monitorTransform;

		[SerializeField]
		private LckTopButtonsController _topButtonsController;

		private bool _isHorizontalMode = true;

		private CameraMode _currentCameraMode;

		private Camera _playerCamera;

		private Transform _playerHead;

		private bool _justTransitioned;

		private bool _isTabletFollowingPlayer;

		private float _selfieSmoothness;

		private bool _isSelfieFront = true;

		private LckCameraOrientation _currentCameraOrientation = LckCameraOrientation.Landscape;

		private Vector3 _selfieFollowModeOffset = Vector3.zero;

		private bool _micState = true;

		private float _thirdPersonHeightAngle = 25f;

		private CameraTrackDescriptor _currentTrackDescriptor;

		private bool _isOverlayActive;

		public delegate void CameraModeDelegate(CameraMode mode, ILckCamera camera);
	}
}
