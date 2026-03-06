using System;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Recorder;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class DroneController : MonoBehaviour
	{
		private void Awake()
		{
			this._model = new DroneDataModel(false, true, false, false, 3f, 10f, 0.1f, 1f, 1f, 10f, 0.1f, 1f, 30f, 90f, 1f, 10f, 1f, 10f, 0.1f, 1f, 90f, 1f, 120f, 1f, 10f, 5f, 10f, 0.1f, 1f, false, true, false, false);
			this._droneGeneralKeyboard = new DroneGeneralKeyboard();
			this._droneKeyboard = new DroneKeyboard();
			this._droneMouse = new DroneMouse();
			this._droneGamepad = new DroneGamepad();
			this._droneMovement = new DroneMovement(this._droneTransform, this._gimbalTransform);
			this._droneCamera = new DroneCamera(this._lckCamera.GetCameraComponent());
			this._droneGUI = new DroneGUI(this._model, this._skin);
			this._droneMovement.SetMoveSpeedChanged(this._model.MoveSpeed);
			this._droneMovement.SetMoveSmoothness(this._model.MoveSmoothness);
			this._droneMovement.SetRotationSpeed(this._model.RotationSpeed);
			this._droneMovement.SetRotationSmoothness(this._model.RotationSmoothness);
			this._droneCamera.SetFov(this._model.Fov);
			this._droneCamera.SetSmoothness(this._model.FovSmoothness);
			this._droneMovement.SetSnapAxis(this._model.SnapAxis);
			this._droneMovement.SetUseTiltAsDirection(this._model.UseTiltAsDirection);
			this._droneMovement.SetIsMouseInverted(this._model.IsMouseInverted);
		}

		private void OnEnable()
		{
			this._droneKeyboard.OnMoveForward += this._droneMovement.MoveForward;
			this._droneKeyboard.OnMoveBackward += this._droneMovement.MoveBackward;
			this._droneKeyboard.OnMoveLeft += this._droneMovement.MoveLeft;
			this._droneKeyboard.OnMoveRight += this._droneMovement.MoveRight;
			this._droneKeyboard.OnMoveUp += this._droneMovement.MoveUp;
			this._droneKeyboard.OnMoveDown += this._droneMovement.MoveDown;
			this._droneKeyboard.OnRotateLeft += this._droneMovement.RotateLeft;
			this._droneKeyboard.OnRotateRight += this._droneMovement.RotateRight;
			this._droneKeyboard.OnTiltUp += this._droneMovement.TiltUp;
			this._droneKeyboard.OnTiltDown += this._droneMovement.TiltDown;
			this._droneKeyboard.OnBurstStarted += this._model.BurstStarted;
			this._droneKeyboard.OnBurstEnded += this._model.BurstEnded;
			this._droneGeneralKeyboard.OnShiftPressed += this._model.MinimizeStepping;
			this._droneGeneralKeyboard.OnShiftReleased += this._model.MaximizeStepping;
			this._droneGeneralKeyboard.OnShowUI += this._model.ToggleShowGUI;
			this._droneMouse.OnMouseMoveLeft += this._droneMovement.TiltAndRotateMouse;
			this._droneMouse.OnMouseMoveRight += this._droneMovement.Roll;
			this._droneMouse.OnReset += this._droneMovement.ResetTillAndRoll;
			this._droneMouse.OnMouseScrollUp += this._model.IncreaseFov;
			this._droneMouse.OnMouseScrollDown += this._model.DecreaseFov;
			this._droneGamepad.OnMove += this._droneMovement.MoveForwardBackwardLeftRight;
			this._droneGamepad.OnTiltAndRotate += this._droneMovement.TiltAndRotateGamePad;
			this._droneGamepad.OnMoveUpAndDown += this._droneMovement.MoveUpAndDown;
			this._model.OnIsDroneModeActive += this.ProcessDroneActiveState;
			this._model.OnMoveSpeedChanged += this._droneMovement.SetMoveSpeedChanged;
			this._model.OnMoveSmoothnessChanged += this.SetProcessedMovementSmoothness;
			this._model.OnRotationSpeedChanged += this._droneMovement.SetRotationSpeed;
			this._model.OnRotationSmoothnessChanged += this.SetProcessedRotationSmoothness;
			this._model.OnFovChanged += this._droneCamera.SetFov;
			this._model.OnFovSmoothnessChanged += this.SetProcessedFovSmoothness;
			this._model.OnSnapAxis += this._droneMovement.SetSnapAxis;
			this._model.OnUseTiltAsDirection += this._droneMovement.SetUseTiltAsDirection;
			this._model.OnIsMouseInverted += this._droneMovement.SetIsMouseInverted;
			this._model.OnRecordButtonPressed += this.ProcessRecordButtonBeingPressed;
			this._model.DroneRecordingStateData.OnDroneRecordingStateChanged += this._droneGUI.SetRecordButtonState;
			if (this._lckService != null)
			{
				this._lckService.OnRecordingStarted += this.OnRecordingStarted;
				this._lckService.OnRecordingStopped += this.OnRecordingStopped;
				this._lckService.OnRecordingSaved += this.OnRecordingSaved;
				return;
			}
			Debug.LogError("Unable to get LckService on DroneController");
		}

		private void OnDisable()
		{
			this._droneKeyboard.OnMoveForward -= this._droneMovement.MoveForward;
			this._droneKeyboard.OnMoveBackward -= this._droneMovement.MoveBackward;
			this._droneKeyboard.OnMoveLeft -= this._droneMovement.MoveLeft;
			this._droneKeyboard.OnMoveRight -= this._droneMovement.MoveRight;
			this._droneKeyboard.OnMoveUp -= this._droneMovement.MoveUp;
			this._droneKeyboard.OnMoveDown -= this._droneMovement.MoveDown;
			this._droneKeyboard.OnRotateLeft -= this._droneMovement.RotateLeft;
			this._droneKeyboard.OnRotateRight -= this._droneMovement.RotateRight;
			this._droneKeyboard.OnTiltUp -= this._droneMovement.TiltUp;
			this._droneKeyboard.OnTiltDown -= this._droneMovement.TiltDown;
			this._droneKeyboard.OnBurstStarted -= this._model.BurstStarted;
			this._droneKeyboard.OnBurstEnded -= this._model.BurstEnded;
			this._droneGeneralKeyboard.OnShiftPressed -= this._model.MinimizeStepping;
			this._droneGeneralKeyboard.OnShiftReleased -= this._model.MaximizeStepping;
			this._droneGeneralKeyboard.OnShowUI -= this._model.ToggleShowGUI;
			this._droneMouse.OnMouseMoveLeft -= this._droneMovement.TiltAndRotateMouse;
			this._droneMouse.OnMouseMoveRight -= this._droneMovement.Roll;
			this._droneMouse.OnReset -= this._droneMovement.ResetTillAndRoll;
			this._droneMouse.OnMouseScrollUp -= this._model.IncreaseFov;
			this._droneMouse.OnMouseScrollDown -= this._model.DecreaseFov;
			this._droneGamepad.OnMove -= this._droneMovement.MoveForwardBackwardLeftRight;
			this._droneGamepad.OnTiltAndRotate -= this._droneMovement.TiltAndRotateGamePad;
			this._droneGamepad.OnMoveUpAndDown -= this._droneMovement.MoveUpAndDown;
			this._model.OnIsDroneModeActive -= this.ProcessDroneActiveState;
			this._model.OnMoveSpeedChanged -= this._droneMovement.SetMoveSpeedChanged;
			this._model.OnMoveSmoothnessChanged -= this.SetProcessedMovementSmoothness;
			this._model.OnRotationSpeedChanged -= this._droneMovement.SetRotationSpeed;
			this._model.OnRotationSmoothnessChanged -= this.SetProcessedRotationSmoothness;
			this._model.OnFovChanged -= this._droneCamera.SetFov;
			this._model.OnFovSmoothnessChanged -= this.SetProcessedFovSmoothness;
			this._model.OnSnapAxis -= this._droneMovement.SetSnapAxis;
			this._model.OnUseTiltAsDirection -= this._droneMovement.SetUseTiltAsDirection;
			this._model.OnIsMouseInverted -= this._droneMovement.SetIsMouseInverted;
			this._model.OnRecordButtonPressed -= this.ProcessRecordButtonBeingPressed;
			this._model.DroneRecordingStateData.OnDroneRecordingStateChanged -= this._droneGUI.SetRecordButtonState;
			if (this._lckService != null)
			{
				this._lckService.OnRecordingStarted -= this.OnRecordingStarted;
				this._lckService.OnRecordingStopped -= this.OnRecordingStopped;
				this._lckService.OnRecordingSaved -= this.OnRecordingSaved;
			}
		}

		private void OnGUI()
		{
			if (this._model.ShowGUI)
			{
				this._droneGUI.Run();
			}
		}

		private void Update()
		{
			this._droneGeneralKeyboard.Run();
			if (!this._model.IsDroneModeActive)
			{
				return;
			}
			if (this._model.UseKeyboard)
			{
				this._droneKeyboard.Run();
			}
			if (this._model.UseMouse)
			{
				this._droneMouse.Run();
			}
			if (this._model.UseGamepad)
			{
				this._droneGamepad.Run();
			}
			this._droneMovement.Run();
			this._droneCamera.Run();
			if (this._model.DroneRecordingStateData.State == RecordingState.Recording)
			{
				TimeSpan result = this._lckService.GetRecordingDuration().Result;
				this._model.DroneRecordingStateData.Span = result;
			}
		}

		public LckCamera GetLckCamera()
		{
			return this._lckCamera;
		}

		public DroneDataModel GetModel()
		{
			return this._model;
		}

		public void SetDronePositionAndRotation(Vector3 position, Quaternion rotation)
		{
			this._droneMovement.MoveAndRotateDroneInstantly(position, rotation);
		}

		private void OnRecordingStarted(LckResult result)
		{
			this._model.DroneRecordingStateData.State = RecordingState.Recording;
			this._model.DroneRecordingStateData.Span = TimeSpan.Zero;
		}

		private void OnRecordingStopped(LckResult result)
		{
			this._model.DroneRecordingStateData.State = RecordingState.Saving;
		}

		private void OnRecordingSaved(LckResult<RecordingData> lckResult)
		{
			this._model.DroneRecordingStateData.State = RecordingState.Idle;
			this._model.DroneRecordingStateData.Span = TimeSpan.Zero;
		}

		private void ProcessRecordButtonBeingPressed()
		{
			if (!this._model.IsDroneModeActive)
			{
				return;
			}
			if (this._lckService.IsRecording().Result)
			{
				this._lckService.StopRecording();
				return;
			}
			if (this._model.DroneRecordingStateData.State == RecordingState.Saving)
			{
				return;
			}
			if (this._model.DroneRecordingStateData.State == RecordingState.Idle)
			{
				this._lckService.StartRecording();
			}
		}

		private void ProcessDroneActiveState(bool isActive)
		{
			if (isActive)
			{
				return;
			}
			if (this._lckService.IsRecording().Result)
			{
				this._lckService.StopRecording();
			}
		}

		private void SetProcessedMovementSmoothness(float value)
		{
			this._droneMovement.SetMoveSmoothness(value / this._model.MaxMoveSmoothness * 0.5f);
		}

		private void SetProcessedRotationSmoothness(float value)
		{
			this._droneMovement.SetRotationSmoothness(value / this._model.MaxRotationSmoothness * 0.5f);
		}

		private void SetProcessedFovSmoothness(float value)
		{
			this._droneCamera.SetSmoothness(value / this._model.MaxFovSmoothness);
		}

		[Header("UI Style")]
		[SerializeField]
		private GUISkin _skin;

		[Header("Drone Parts")]
		[SerializeField]
		private Transform _droneTransform;

		[SerializeField]
		private Transform _gimbalTransform;

		[Header("Cameras")]
		[SerializeField]
		private LckCamera _lckCamera;

		private DroneDataModel _model;

		private DroneGeneralKeyboard _droneGeneralKeyboard;

		private DroneKeyboard _droneKeyboard;

		private DroneMouse _droneMouse;

		private DroneGamepad _droneGamepad;

		private DroneMovement _droneMovement;

		private DroneCamera _droneCamera;

		private DroneGUI _droneGUI;

		[InjectLck]
		private ILckService _lckService;
	}
}
