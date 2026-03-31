using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

public class LckSocialCameraManager : MonoBehaviour
{
	public LckDirectGrabbable lckDirectGrabbable
	{
		get
		{
			return this._lckDirectGrabbable;
		}
	}

	public static LckSocialCameraManager Instance
	{
		get
		{
			return LckSocialCameraManager._instance;
		}
	}

	private void Awake()
	{
		this.SetManagerInstance();
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	private void OnEnable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted += this.OnRecordingStarted;
			service.Result.OnStreamingStarted += this.OnRecordingStarted;
			service.Result.OnRecordingStopped += this.OnRecordingStopped;
			service.Result.OnStreamingStopped += this.OnRecordingStopped;
		}
		LckBodyCameraSpawner.OnCameraStateChange += this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
		this._cameraMode = this._gtLckController.CurrentCameraMode;
	}

	private void Update()
	{
		if (this._lckCamera != null)
		{
			Transform transform = this._lckCamera.transform;
			if (this._networkedCococam != null)
			{
				this._networkedCococam.transform.position = transform.position;
				this._networkedCococam.transform.rotation = transform.rotation;
			}
			if (this._networkedTablet != null)
			{
				if (this._networkedTablet.IsOnNeck)
				{
					this._networkedTablet.transform.position = base.transform.position;
				}
				else
				{
					this._networkedTablet.transform.position = base.transform.position + this._tabletPositionOffset * this._networkedTablet.VrRig.scaleFactor;
				}
				this._networkedTablet.transform.rotation = base.transform.rotation;
			}
		}
		if (this._needsUpdate)
		{
			this.UpdateCococamVisibility(this._cameraState, this._cameraMode, this._isForceHidden, this.cameraActive);
			this.UpdateTabletVisibility(this._cameraState, this._isForceHidden, this.cameraActive);
			this.UpdateCococamRecording(this._isRecording);
			this.UpdateTabletRecording(this._isRecording);
			this._needsUpdate = false;
		}
	}

	private void OnDisable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted -= this.OnRecordingStarted;
			service.Result.OnStreamingStarted -= this.OnRecordingStarted;
			service.Result.OnRecordingStopped -= this.OnRecordingStopped;
			service.Result.OnStreamingStopped -= this.OnRecordingStopped;
		}
		LckBodyCameraSpawner.OnCameraStateChange -= this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
	}

	public void SetForceHidden(bool hidden)
	{
		if (this._isForceHidden == hidden)
		{
			return;
		}
		this._isForceHidden = hidden;
		this._needsUpdate = true;
	}

	public void SetLckSocialCococamCamera(LckSocialCamera socialCamera)
	{
		if (this._networkedCococam == socialCamera)
		{
			return;
		}
		this._networkedCococam = socialCamera;
		this._needsUpdate = true;
	}

	public void SetLckSocialTabletCamera(LckSocialCamera socialCameraTablet)
	{
		if (this._networkedTablet == socialCameraTablet)
		{
			return;
		}
		this._networkedTablet = socialCameraTablet;
		this._needsUpdate = true;
	}

	public bool cameraActive
	{
		get
		{
			return this._localCameras.activeSelf;
		}
		set
		{
			if (this._localCameras.activeSelf == value)
			{
				return;
			}
			this._localCameras.SetActive(value);
			this._needsUpdate = true;
		}
	}

	public bool uiVisible
	{
		get
		{
			return this._localUi.activeSelf;
		}
		set
		{
			this._localUi.SetActive(value);
		}
	}

	private void SetManagerInstance()
	{
		LckSocialCameraManager._instance = this;
		Action<LckSocialCameraManager> onManagerSpawned = LckSocialCameraManager.OnManagerSpawned;
		if (onManagerSpawned == null)
		{
			return;
		}
		onManagerSpawned(this);
	}

	private void OnBodyCameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		if (this._cameraState == state)
		{
			return;
		}
		this._cameraState = state;
		this._needsUpdate = true;
	}

	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		if (this._cameraMode == mode)
		{
			return;
		}
		this._cameraMode = mode;
		this._needsUpdate = true;
	}

	private void OnRecordingStarted(LckResult result)
	{
		if (this._isRecording == result.Success)
		{
			return;
		}
		this._isRecording = result.Success;
		this._needsUpdate = true;
	}

	private void OnRecordingStopped(LckResult result)
	{
		if (!this._isRecording)
		{
			return;
		}
		this._isRecording = false;
		this._needsUpdate = true;
	}

	private void UpdateCococamRecording(bool recording)
	{
		this.CoconutCamera.SetRecordingState(recording);
		if (this._networkedCococam == null)
		{
			return;
		}
		this._networkedCococam.recording = recording;
	}

	private void UpdateCococamVisibility(LckBodyCameraSpawner.CameraState cameraState, CameraMode cameraMode, bool forceHidden, bool cameraActive)
	{
		if (cameraMode - CameraMode.ThirdPerson <= 1)
		{
			this.CoconutCamera.SetVisualsActive(cameraActive);
		}
		else
		{
			this.CoconutCamera.SetVisualsActive(false);
		}
		if (this._networkedCococam == null)
		{
			return;
		}
		if (cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || forceHidden || !cameraActive)
		{
			this._networkedCococam.visible = false;
			return;
		}
		this._networkedCococam.visible = (cameraMode == CameraMode.ThirdPerson || cameraMode == CameraMode.Drone);
	}

	private void UpdateTabletRecording(bool recording)
	{
		if (this._networkedTablet == null)
		{
			return;
		}
		this._networkedTablet.recording = recording;
	}

	private void UpdateTabletVisibility(LckBodyCameraSpawner.CameraState cameraState, bool forceHidden, bool cameraActive)
	{
		if (this._networkedTablet == null)
		{
			return;
		}
		if (cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || forceHidden)
		{
			this._networkedTablet.visible = false;
			this._networkedTablet.IsOnNeck = false;
			return;
		}
		this._networkedTablet.visible = cameraActive;
		this._networkedTablet.IsOnNeck = (cameraState == LckBodyCameraSpawner.CameraState.CameraOnNeck);
	}

	[SerializeField]
	private GameObject _localUi;

	[SerializeField]
	private GameObject _localCameras;

	[SerializeField]
	private GTLckController _gtLckController;

	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	[SerializeField]
	public CoconutCamera CoconutCamera;

	private LckSocialCamera _networkedCococam;

	private LckSocialCamera _networkedTablet;

	private Camera _lckCamera;

	private CameraMode _cameraMode;

	private LckBodyCameraSpawner.CameraState _cameraState;

	[OnEnterPlay_SetNull]
	private static LckSocialCameraManager _instance;

	public static Action<LckSocialCameraManager> OnManagerSpawned;

	private bool _isRecording;

	private bool _isForceHidden;

	private bool _needsUpdate = true;

	private Vector3 _tabletPositionOffset = new Vector3(0f, 0.11f, -0.08f);
}
