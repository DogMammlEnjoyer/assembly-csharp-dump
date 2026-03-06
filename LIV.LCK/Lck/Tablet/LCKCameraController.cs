using System;
using System.Collections.Generic;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Smoothing;
using Liv.Lck.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Liv.Lck.Tablet
{
	[DefaultExecutionOrder(-890)]
	public class LCKCameraController : MonoBehaviour
	{
		public Transform HmdTransform
		{
			get
			{
				if (this._hmdTransform == null)
				{
					this._hmdTransform = Camera.main.transform;
				}
				return this._hmdTransform;
			}
			set
			{
				this._hmdTransform = value;
			}
		}

		public UpdateTimingMode CameraPositionUpdateTimingMode
		{
			get
			{
				return this._cameraPositionUpdateTimingMode;
			}
			set
			{
				this._cameraPositionUpdateTimingMode = value;
			}
		}

		private void OnValidate()
		{
			if (this._qualityConfig != null && !(this._qualityConfig is ILckQualityConfig))
			{
				Debug.LogError("LCK Quality Config must implement ILckQualityConfig interface");
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				LCKCameraController.ColliderButtonsInUse = false;
			}
		}

		private void Start()
		{
			if (this._modifyRenderLayerAndCullingMasks)
			{
				this.SetTabletLayer();
			}
			LckQualitySelector qualitySelector = this._qualitySelector;
			qualitySelector.OnQualityOptionChanged = (Action<QualityOption>)Delegate.Combine(qualitySelector.OnQualityOptionChanged, new Action<QualityOption>(this.OnQualityOptionSelected));
			this._qualitySelector.InitializeOptions((this._qualityConfig as ILckQualityConfig).GetQualityOptionsForSystem());
			this.SetActiveLckCamera(this._selfieCamera.CameraId);
			this.SetSelfieCameraOrientation(Vector3.zero, Vector3.zero);
			this._lckService.OnRecordingStarted += this.OnCaptureStart;
			this._lckService.OnRecordingStopped += this.OnCaptureStopped;
			this._lckService.OnStreamingStarted += this.OnCaptureStart;
			this._lckService.OnStreamingStopped += this.OnCaptureStopped;
		}

		private void Awake()
		{
			if (!LckDiContainer.Instance.HasService<ILckService>())
			{
				LckServiceInitializer.ConfigureServices(LckDiContainer.Instance, (LckQualityConfig)this._qualityConfig, null);
				this._lckService = LckDiContainer.Instance.GetService<ILckService>();
			}
		}

		private void SetTabletLayer()
		{
			int num = LayerMask.NameToLayer(this._tabletRenderingLayer);
			if (num == -1)
			{
				LckLog.LogError("LCK Tablet layer '" + this._tabletRenderingLayer + "' not found. Please add it to Project Settings > Tags and Layers.");
				return;
			}
			foreach (GameObject gameObject in this._objectsHiddenFromSelfieCamera)
			{
				gameObject.layer = num;
			}
			this._selfieCamera.GetCameraComponent().cullingMask &= ~(1 << num);
			this._firstPersonCamera.GetCameraComponent().cullingMask |= 1 << num;
			this._thirdPersonCamera.GetCameraComponent().cullingMask |= 1 << num;
		}

		private void OnQualityOptionSelected(QualityOption qualityOption)
		{
			CameraTrackDescriptor descriptorForCurrentOrientation = this.GetDescriptorForCurrentOrientation(qualityOption.RecordingCameraTrackDescriptor);
			this._lckService.SetTrackDescriptor(LckCaptureType.Recording, descriptorForCurrentOrientation);
			CameraTrackDescriptor descriptorForCurrentOrientation2 = this.GetDescriptorForCurrentOrientation(qualityOption.StreamingCameraTrackDescriptor);
			this._lckService.SetTrackDescriptor(LckCaptureType.Streaming, descriptorForCurrentOrientation2);
		}

		private CameraTrackDescriptor GetDescriptorForCurrentOrientation(CameraTrackDescriptor descriptor)
		{
			CameraResolutionDescriptor cameraResolutionDescriptor = descriptor.CameraResolutionDescriptor;
			descriptor.CameraResolutionDescriptor = cameraResolutionDescriptor.GetResolutionInOrientation(this._currentCameraOrientation);
			return descriptor;
		}

		private void UpdateCameraPosition()
		{
			if (this._lckService == null)
			{
				return;
			}
			switch (this._currentCameraMode)
			{
			case CameraMode.Selfie:
				break;
			case CameraMode.FirstPerson:
				this.ProcessFirstCameraPosition();
				return;
			case CameraMode.ThirdPerson:
				this.ProcessThirdCameraPosition();
				break;
			default:
				return;
			}
		}

		private void OnEnable()
		{
			LCKSettingsButtonsController settingsButtonsController = this._settingsButtonsController;
			settingsButtonsController.OnCameraModeChanged = (Action<CameraMode>)Delegate.Combine(settingsButtonsController.OnCameraModeChanged, new Action<CameraMode>(this.CameraModeChanged));
			this._selfieFOVDoubleButton.OnValueChanged += this.ProcessSelfieFov;
			this._selfieSmoothingDoubleButton.OnValueChanged += this.ProcessSelfieSmoothness;
			this._firstPersonFOVDoubleButton.OnValueChanged += this.ProcessFirstPersonFov;
			this._firstPersonSmoothingDoubleButton.OnValueChanged += this.ProcessFirstPersonSmoothness;
			this._thirdPersonFOVDoubleButton.OnValueChanged += this.ProcessThirdPersonFov;
			this._thirdPersonSmoothingDoubleButton.OnValueChanged += this.ProcessThirdPersonSmoothness;
			this._thirdPersonDistanceDoubleButton.OnValueChanged += this.ProcessThirdPersonDistance;
		}

		private void OnDisable()
		{
			LCKSettingsButtonsController settingsButtonsController = this._settingsButtonsController;
			settingsButtonsController.OnCameraModeChanged = (Action<CameraMode>)Delegate.Remove(settingsButtonsController.OnCameraModeChanged, new Action<CameraMode>(this.CameraModeChanged));
			this._selfieFOVDoubleButton.OnValueChanged -= this.ProcessSelfieFov;
			this._selfieSmoothingDoubleButton.OnValueChanged -= this.ProcessSelfieSmoothness;
			this._firstPersonFOVDoubleButton.OnValueChanged -= this.ProcessFirstPersonFov;
			this._firstPersonSmoothingDoubleButton.OnValueChanged -= this.ProcessFirstPersonSmoothness;
			this._thirdPersonFOVDoubleButton.OnValueChanged -= this.ProcessThirdPersonFov;
			this._thirdPersonSmoothingDoubleButton.OnValueChanged -= this.ProcessThirdPersonSmoothness;
			this._thirdPersonDistanceDoubleButton.OnValueChanged -= this.ProcessThirdPersonDistance;
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

		private void LateUpdate()
		{
			if (this.CameraPositionUpdateTimingMode == UpdateTimingMode.LateUpdate)
			{
				this.UpdateCameraPosition();
			}
		}

		private void Update()
		{
			if (this.CameraPositionUpdateTimingMode == UpdateTimingMode.Update)
			{
				this.UpdateCameraPosition();
			}
		}

		private void FixedUpdate()
		{
			if (this.CameraPositionUpdateTimingMode == UpdateTimingMode.FixedUpdate)
			{
				this.UpdateCameraPosition();
			}
		}

		private void SetSelfieCameraOrientation(Vector3 position, Vector3 rotation)
		{
			this._selfieStabilizer.transform.localPosition = position;
			this._selfieStabilizer.transform.localRotation = Quaternion.Euler(rotation);
			this._selfieStabilizer.ReachTargetInstantly();
		}

		private void ProcessSelfieFov(float value)
		{
			this._selfieCamera.GetCameraComponent().fieldOfView = this.CalculateCorrectFOV(value);
		}

		private void ProcessSelfieSmoothness(float value)
		{
			this._selfieStabilizer.PositionalSmoothing = value * 0.1f * 0.3f;
			this._selfieStabilizer.RotationalSmoothing = value * 0.1f * 0.8f;
		}

		private void ProcessFirstPersonFov(float value)
		{
			this._firstPersonCamera.GetCameraComponent().fieldOfView = this.CalculateCorrectFOV(value);
		}

		private void ProcessFirstPersonSmoothness(float value)
		{
			this._firstPersonStabilizer.PositionalSmoothing = value * 0.1f * 0.3f;
			this._firstPersonStabilizer.RotationalSmoothing = value * 0.1f * 0.8f;
		}

		private void ProcessFirstCameraPosition()
		{
			this._firstPersonStabilizer.transform.position = this.HmdTransform.position + this.HmdTransform.forward * 0.05f;
			this._firstPersonStabilizer.transform.rotation = this.HmdTransform.rotation;
			if (this._justTransitioned)
			{
				this._firstPersonStabilizer.ReachTargetInstantly();
				this._justTransitioned = false;
			}
		}

		private void ProcessThirdPersonFov(float value)
		{
			this._thirdPersonCamera.GetCameraComponent().fieldOfView = this.CalculateCorrectFOV(value);
		}

		private void ProcessThirdPersonSmoothness(float value)
		{
			this._thirdPersonStabilizer.PositionalSmoothing = value * 0.1f * 0.3f;
			this._thirdPersonStabilizer.RotationalSmoothing = value * 0.1f * 0.8f;
		}

		private void ProcessThirdPersonDistance(float value)
		{
			this._thirdPersonDistance = value;
			this._justTransitioned = true;
		}

		private void ProcessThirdCameraPosition()
		{
			Vector3 vector = new Vector3(this.HmdTransform.forward.x, 0f, this.HmdTransform.forward.z);
			vector.Normalize();
			if (!this._isThirdPersonFront)
			{
				vector *= -1f;
			}
			Vector3 b = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward, vector, Vector3.up), Vector3.up) * Quaternion.AngleAxis(this._thirdPersonHeightAngle, -Vector3.right) * new Vector3(0f, 0f, this._thirdPersonDistance * this._thirdPersonDistanceMultiplier);
			this._thirdPersonStabilizer.transform.position = this.HmdTransform.position + b;
			this._thirdPersonStabilizer.transform.LookAt(this.HmdTransform.position);
			if (this._justTransitioned)
			{
				this._thirdPersonStabilizer.ReachTargetInstantly();
				this._justTransitioned = false;
			}
		}

		private void SetFOV(CameraMode mode, float fov)
		{
			switch (mode)
			{
			case CameraMode.Selfie:
				this._selfieCamera.GetCameraComponent().fieldOfView = fov;
				return;
			case CameraMode.FirstPerson:
				this._firstPersonCamera.GetCameraComponent().fieldOfView = fov;
				return;
			case CameraMode.ThirdPerson:
				this._thirdPersonCamera.GetCameraComponent().fieldOfView = fov;
				return;
			default:
				return;
			}
		}

		public void ToggleMicrophoneRecording(bool isMicOn)
		{
			if (this._lckService == null)
			{
				LckLog.LogError("No Lck Service found when trying to set mic state to: " + isMicOn.ToString());
				return;
			}
			LckResult lckResult = this._lckService.SetMicrophoneCaptureActive(isMicOn);
			if (!lckResult.Success)
			{
				LckLog.LogError(string.Format("LCK Could not enable microphone capture: {0}", lckResult.Error));
			}
		}

		public void ToggleGameAudio()
		{
			this._gameAudioRecordingEnabled = !this._gameAudioRecordingEnabled;
			this._lckService.SetGameAudioCaptureActive(this._gameAudioRecordingEnabled);
		}

		public void ToggleRecording()
		{
			if (this._lckService == null)
			{
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

		public void OnCaptureStart(LckResult result)
		{
			if (!result.Success)
			{
				this.SetOrientationQualityAndTopButtonsIsDisabledState(false);
			}
		}

		public void OnCaptureStopped(LckResult result)
		{
			this.SetOrientationQualityAndTopButtonsIsDisabledState(false);
		}

		public void SetOrientationQualityAndTopButtonsIsDisabledState(bool state)
		{
			this._topButtonsController.SetTopButtonsIsDisabledState(state);
			this._qualitySelector.SetQualityButtonIsDisabledState(state);
			this._orientationButton.SetIsDisabled(state);
		}

		public void ToggleOrientation()
		{
			if (this._lckService.IsCapturing().Result)
			{
				return;
			}
			this._currentCameraOrientation = ((this._currentCameraOrientation == LckCameraOrientation.Landscape) ? LckCameraOrientation.Portrait : LckCameraOrientation.Landscape);
			this._lckService.SetCameraOrientation(this._currentCameraOrientation);
			this._monitorTransform.sizeDelta = ((this._currentCameraOrientation == LckCameraOrientation.Landscape) ? new Vector2(1109f, 624f) : new Vector2(352f, 624f));
			this.GetCurrentModeCamera().fieldOfView = this.CalculateCorrectFOV(this.GetCurrentModeFOV());
		}

		private Camera GetCurrentModeCamera()
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
				throw new Exception("Invalid Camera Mode");
			}
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

		private float GetCurrentModeFOV()
		{
			switch (this._currentCameraMode)
			{
			case CameraMode.Selfie:
				return this._selfieFOVDoubleButton.Value;
			case CameraMode.FirstPerson:
				return this._firstPersonFOVDoubleButton.Value;
			case CameraMode.ThirdPerson:
				return this._thirdPersonFOVDoubleButton.Value;
			default:
				throw new Exception("Invalid Camera Mode");
			}
		}

		public void ProcessSelfieFlip()
		{
			this._isSelfieFront = !this._isSelfieFront;
			this.SetMonitorScale(CameraMode.Selfie);
			if (this._isSelfieFront)
			{
				this.SetSelfieCameraOrientation(Vector3.zero, Vector3.zero);
			}
			else
			{
				this.SetSelfieCameraOrientation(Vector3.zero, new Vector3(0f, 180f, 0f));
			}
			this._selfieStabilizer.ReachTargetInstantly();
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
				this._monitorTransform.localScale = (this._isThirdPersonFront ? one : vector);
				return;
			default:
				return;
			}
		}

		public void ProcessThirdPersonPosition()
		{
			this._isThirdPersonFront = !this._isThirdPersonFront;
			this._justTransitioned = true;
			this.SetMonitorScale(CameraMode.ThirdPerson);
		}

		private void CameraModeChanged(CameraMode mode)
		{
			this._currentCameraMode = mode;
			this._justTransitioned = true;
			float fov = this.CalculateCorrectFOV(this.GetCurrentModeFOV());
			this.SetFOV(this._currentCameraMode, fov);
			this.SetMonitorScale(mode);
			switch (mode)
			{
			case CameraMode.Selfie:
				this.SetActiveLckCamera(this._selfieCamera.CameraId);
				break;
			case CameraMode.FirstPerson:
				this.SetActiveLckCamera(this._firstPersonCamera.CameraId);
				break;
			case CameraMode.ThirdPerson:
				this.SetActiveLckCamera(this._thirdPersonCamera.CameraId);
				break;
			}
			Action<CameraMode> onCameraModeChanged = this.OnCameraModeChanged;
			if (onCameraModeChanged == null)
			{
				return;
			}
			onCameraModeChanged(this._currentCameraMode);
		}

		private void SetActiveLckCamera(string cameraId)
		{
			if (this._lckService == null)
			{
				return;
			}
			this._lckService.SetActiveCamera(cameraId, null);
		}

		[InjectLck]
		private ILckService _lckService;

		[Header("Options")]
		[SerializeField]
		[Tooltip("If true, the script will automatically manage layers and culling masks. It will move objects in the 'ObjectsHiddenFromSelfieCamera' list to the 'Tablet Rendering Layer' and adjust camera culling masks to hide this layer in Selfie mode and show it in other modes.")]
		private bool _modifyRenderLayerAndCullingMasks = true;

		[SerializeField]
		[Tooltip("The name of the Unity layer used to tag objects that should be hidden from the selfie camera. This layer must exist in the project's Tag and Layer Manager.")]
		private string _tabletRenderingLayer = "LCK Tablet";

		[FormerlySerializedAs("_objectsOnTabletRenderingLayer")]
		[SerializeField]
		[Tooltip("A list of all GameObjects (e.g., the tablet model itself) that should be made invisible to the selfie camera.")]
		private List<GameObject> _objectsHiddenFromSelfieCamera = new List<GameObject>();

		[SerializeReference]
		[Tooltip("A ScriptableObject that implements the ILckQualityConfig interface. This object defines the available quality levels (resolution, bitrate, etc.) for recording and streaming.")]
		private ScriptableObject _qualityConfig;

		[SerializeField]
		[Tooltip("The transform representing the user's head or HMD. This is the primary anchor for first-person and third-person camera positioning. If null, it will default to the main camera's transform.")]
		private Transform _hmdTransform;

		[SerializeField]
		[Tooltip("A multiplier applied to the value from the third-person distance UI button to determine the actual camera distance.")]
		private float _thirdPersonDistanceMultiplier = 0.75f;

		[SerializeField]
		[Tooltip("The default angle (in degrees) of the third-person camera, looking down at the player.")]
		private float _thirdPersonHeightAngle = 25f;

		[SerializeField]
		[Tooltip("Mode that is used to determine when the active camera's position is updated. Depending on update order / movement setup, changing this can fix tablet jitter in captures.")]
		private UpdateTimingMode _cameraPositionUpdateTimingMode = UpdateTimingMode.LateUpdate;

		[Header("Main References")]
		[SerializeField]
		private LCKSettingsButtonsController _settingsButtonsController;

		[SerializeField]
		private LckTopButtonsController _topButtonsController;

		[SerializeField]
		private RectTransform _monitorTransform;

		[SerializeField]
		private LckQualitySelector _qualitySelector;

		[Header("Button References")]
		[Header("Selfie")]
		[SerializeField]
		private LckDoubleButton _selfieFOVDoubleButton;

		[SerializeField]
		private LckDoubleButton _selfieSmoothingDoubleButton;

		[Header("First Person")]
		[SerializeField]
		private LckDoubleButton _firstPersonFOVDoubleButton;

		[SerializeField]
		private LckDoubleButton _firstPersonSmoothingDoubleButton;

		[Header("Third Person")]
		[SerializeField]
		private LckDoubleButton _thirdPersonFOVDoubleButton;

		[SerializeField]
		private LckDoubleButton _thirdPersonSmoothingDoubleButton;

		[SerializeField]
		private LckDoubleButton _thirdPersonDistanceDoubleButton;

		[Header("Portrait Landscape Toggle")]
		[SerializeField]
		private LckButton _orientationButton;

		[Header("Camera Modes")]
		[Header("Selfie")]
		[SerializeField]
		private LckCamera _selfieCamera;

		[SerializeField]
		private LckStabilizer _selfieStabilizer;

		[Header("First Person")]
		[SerializeField]
		private LckCamera _firstPersonCamera;

		[SerializeField]
		private LckStabilizer _firstPersonStabilizer;

		[Header("Third Person")]
		[SerializeField]
		private LckCamera _thirdPersonCamera;

		[SerializeField]
		private LckStabilizer _thirdPersonStabilizer;

		private float _thirdPersonDistance = 1f;

		private bool _isThirdPersonFront = true;

		private bool _isSelfieFront = true;

		private LckCameraOrientation _currentCameraOrientation = LckCameraOrientation.Landscape;

		private bool _justTransitioned;

		private bool _gameAudioRecordingEnabled = true;

		private CameraMode _currentCameraMode;

		public Action<CameraMode> OnCameraModeChanged;

		public static bool ColliderButtonsInUse;
	}
}
