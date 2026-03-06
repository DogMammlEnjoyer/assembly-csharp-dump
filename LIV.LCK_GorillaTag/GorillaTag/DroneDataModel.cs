using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class DroneDataModel
	{
		public event DroneDataModel.OnDroneModelBoolEvent OnIsDroneModeActive;

		public event DroneDataModel.OnDroneModelBoolEvent OnUseKeyboard;

		public event DroneDataModel.OnDroneModelBoolEvent OnUseMouse;

		public event DroneDataModel.OnDroneModelBoolEvent OnUseGamepad;

		public event DroneDataModel.OnDroneModelFloatEvent OnMoveSpeedChanged;

		public event DroneDataModel.OnDroneModelFloatEvent OnMoveSmoothnessChanged;

		public event DroneDataModel.OnDroneModelBoolEvent OnIsMoveSmooth;

		public event DroneDataModel.OnDroneModelFloatEvent OnRotationSpeedChanged;

		public event DroneDataModel.OnDroneModelFloatEvent OnRotationSmoothnessChanged;

		public event DroneDataModel.OnDroneModelBoolEvent OnIsRotationSmooth;

		public event DroneDataModel.OnDroneModelFloatEvent OnFovChanged;

		public event DroneDataModel.OnDroneModelFloatEvent OnFovSmoothnessChanged;

		public event DroneDataModel.OnDroneModelBoolEvent OnSnapAxis;

		public event DroneDataModel.OnDroneModelBoolEvent OnUseTiltAsDirection;

		public event DroneDataModel.OnDroneModelBoolEvent OnIsMouseInverted;

		public event DroneDataModel.OnDroneModelBoolEvent OnShowGUI;

		public event DroneDataModel.OnDroneModelEvent OnRecordButtonPressed;

		public event DroneDataModel.OnRecordingStateDataEvent OnRecordingStateChanged;

		public bool IsDroneModeActive
		{
			get
			{
				return this._isDroneModeActive;
			}
			set
			{
				this._isDroneModeActive = value;
				DroneDataModel.OnDroneModelBoolEvent onIsDroneModeActive = this.OnIsDroneModeActive;
				if (onIsDroneModeActive == null)
				{
					return;
				}
				onIsDroneModeActive(this._isDroneModeActive);
			}
		}

		public bool UseKeyboard
		{
			get
			{
				return this._useKeyboard;
			}
			set
			{
				this._useKeyboard = value;
				DroneDataModel.OnDroneModelBoolEvent onUseKeyboard = this.OnUseKeyboard;
				if (onUseKeyboard == null)
				{
					return;
				}
				onUseKeyboard(this._useKeyboard);
			}
		}

		public bool UseMouse
		{
			get
			{
				return this._useMouse;
			}
			set
			{
				this._useMouse = value;
				DroneDataModel.OnDroneModelBoolEvent onUseMouse = this.OnUseMouse;
				if (onUseMouse == null)
				{
					return;
				}
				onUseMouse(this._useMouse);
			}
		}

		public bool UseGamepad
		{
			get
			{
				return this._useGamepad;
			}
			set
			{
				this._useGamepad = value;
				DroneDataModel.OnDroneModelBoolEvent onUseGamepad = this.OnUseGamepad;
				if (onUseGamepad == null)
				{
					return;
				}
				onUseGamepad(this._useGamepad);
			}
		}

		public float MoveSpeed
		{
			get
			{
				return this._moveSpeed;
			}
			set
			{
				this._moveSpeed = value;
				DroneDataModel.OnDroneModelFloatEvent onMoveSpeedChanged = this.OnMoveSpeedChanged;
				if (onMoveSpeedChanged == null)
				{
					return;
				}
				onMoveSpeedChanged(this._moveSpeed);
			}
		}

		public float MaxMoveSpeed { get; }

		public float MoveSpeedStep { get; private set; }

		public float MoveSmoothness
		{
			get
			{
				return this._moveSmoothness;
			}
			set
			{
				this._moveSmoothness = value;
				DroneDataModel.OnDroneModelFloatEvent onMoveSmoothnessChanged = this.OnMoveSmoothnessChanged;
				if (onMoveSmoothnessChanged != null)
				{
					onMoveSmoothnessChanged(this._moveSmoothness);
				}
				if (Mathf.Approximately(this._moveSmoothness, 0f))
				{
					DroneDataModel.OnDroneModelBoolEvent onIsMoveSmooth = this.OnIsMoveSmooth;
					if (onIsMoveSmooth == null)
					{
						return;
					}
					onIsMoveSmooth(false);
					return;
				}
				else
				{
					DroneDataModel.OnDroneModelBoolEvent onIsMoveSmooth2 = this.OnIsMoveSmooth;
					if (onIsMoveSmooth2 == null)
					{
						return;
					}
					onIsMoveSmooth2(true);
					return;
				}
			}
		}

		public float MaxMoveSmoothness { get; }

		public float MoveSmoothnessStep { get; private set; }

		public float RotationSpeed
		{
			get
			{
				return this._rotationSpeed;
			}
			set
			{
				this._rotationSpeed = value;
				DroneDataModel.OnDroneModelFloatEvent onRotationSpeedChanged = this.OnRotationSpeedChanged;
				if (onRotationSpeedChanged == null)
				{
					return;
				}
				onRotationSpeedChanged(this._rotationSpeed);
			}
		}

		public float MaxRotationSpeed { get; }

		public float RotationSpeedStep { get; private set; }

		public float RotationSmoothness
		{
			get
			{
				return this._rotationSmoothness;
			}
			set
			{
				this._rotationSmoothness = value;
				DroneDataModel.OnDroneModelFloatEvent onRotationSmoothnessChanged = this.OnRotationSmoothnessChanged;
				if (onRotationSmoothnessChanged != null)
				{
					onRotationSmoothnessChanged(this._rotationSmoothness);
				}
				if (Mathf.Approximately(this._rotationSmoothness, 0f))
				{
					DroneDataModel.OnDroneModelBoolEvent onIsRotationSmooth = this.OnIsRotationSmooth;
					if (onIsRotationSmooth == null)
					{
						return;
					}
					onIsRotationSmooth(false);
					return;
				}
				else
				{
					DroneDataModel.OnDroneModelBoolEvent onIsRotationSmooth2 = this.OnIsRotationSmooth;
					if (onIsRotationSmooth2 == null)
					{
						return;
					}
					onIsRotationSmooth2(true);
					return;
				}
			}
		}

		public float MaxRotationSmoothness { get; }

		public float RotationSmoothnessStep { get; private set; }

		public float Fov
		{
			get
			{
				return this._fov;
			}
			set
			{
				this._fov = value;
				DroneDataModel.OnDroneModelFloatEvent onFovChanged = this.OnFovChanged;
				if (onFovChanged == null)
				{
					return;
				}
				onFovChanged(this._fov);
			}
		}

		public float MinFov { get; }

		public float MaxFov { get; }

		public float FovStep { get; private set; }

		public float FovSmoothness
		{
			get
			{
				return this._fovSmoothness;
			}
			set
			{
				this._fovSmoothness = value;
				DroneDataModel.OnDroneModelFloatEvent onFovSmoothnessChanged = this.OnFovSmoothnessChanged;
				if (onFovSmoothnessChanged == null)
				{
					return;
				}
				onFovSmoothnessChanged(this._fovSmoothness);
			}
		}

		public float MaxFovSmoothness { get; }

		public float FovSmoothnessStep { get; private set; }

		public bool SnapAxis
		{
			get
			{
				return this._snapAxis;
			}
			set
			{
				this._snapAxis = value;
				DroneDataModel.OnDroneModelBoolEvent onSnapAxis = this.OnSnapAxis;
				if (onSnapAxis == null)
				{
					return;
				}
				onSnapAxis(this._snapAxis);
			}
		}

		public bool UseTiltAsDirection
		{
			get
			{
				return this._useTiltAsDirection;
			}
			set
			{
				this._useTiltAsDirection = value;
				DroneDataModel.OnDroneModelBoolEvent onUseTiltAsDirection = this.OnUseTiltAsDirection;
				if (onUseTiltAsDirection == null)
				{
					return;
				}
				onUseTiltAsDirection(this._useTiltAsDirection);
			}
		}

		public bool IsMouseInverted
		{
			get
			{
				return this._isMouseInverted;
			}
			set
			{
				this._isMouseInverted = value;
				DroneDataModel.OnDroneModelBoolEvent onIsMouseInverted = this.OnIsMouseInverted;
				if (onIsMouseInverted == null)
				{
					return;
				}
				onIsMouseInverted(this._isMouseInverted);
			}
		}

		public bool ShowGUI
		{
			get
			{
				return this._showGUI;
			}
			set
			{
				this._showGUI = value;
				DroneDataModel.OnDroneModelBoolEvent onShowGUI = this.OnShowGUI;
				if (onShowGUI == null)
				{
					return;
				}
				onShowGUI(this._showGUI);
			}
		}

		public DroneRecordingStateData DroneRecordingStateData { get; set; }

		public DroneDataModel(bool isDroneModeActive, bool useKeyboard, bool useMouse, bool useGamepad, float moveSpeed, float maxMoveSpeed, float minMoveSpeedStep, float maxMoveSpeedStep, float moveSmoothness, float maxMoveSmoothness, float minMoveSmoothnessStep, float maxMoveSmoothnessStep, float rotationSpeed, float maxRotationSpeed, float minRotationSpeedStep, float maxRotationSpeedStep, float rotationSmoothness, float maxRotationSmoothness, float minRotationSmoothnessStep, float maxRotationSmoothnessStep, float fov, float minFov, float maxFov, float minFovStep, float maxFovStep, float fovSmoothness, float maxFovSmoothness, float minFovSmoothnessStep, float maxFovSmoothnessStep, bool snapAxis, bool useTiltAsDirection, bool isMouseInverted, bool showGUI)
		{
			this.IsDroneModeActive = isDroneModeActive;
			this.UseKeyboard = useKeyboard;
			this.UseMouse = useMouse;
			this.UseGamepad = useGamepad;
			this.MoveSpeed = moveSpeed;
			this.MaxMoveSpeed = maxMoveSpeed;
			this.MoveSpeedStep = maxMoveSpeedStep;
			this._minMoveSpeedStep = minMoveSpeedStep;
			this._maxMoveSpeedStep = maxMoveSpeedStep;
			this.MoveSmoothness = moveSmoothness;
			this.MaxMoveSmoothness = maxMoveSmoothness;
			this.MoveSmoothnessStep = maxMoveSmoothnessStep;
			this._minMoveSmoothnessStep = minMoveSmoothnessStep;
			this._maxMoveSmoothnessStep = maxMoveSmoothnessStep;
			this.RotationSpeed = rotationSpeed;
			this.MaxRotationSpeed = maxRotationSpeed;
			this.RotationSpeedStep = maxRotationSpeedStep;
			this._minRotationSpeedStep = minRotationSpeedStep;
			this._maxRotationSpeedStep = maxRotationSpeedStep;
			this.RotationSmoothness = rotationSmoothness;
			this.MaxRotationSmoothness = maxRotationSmoothness;
			this.RotationSmoothnessStep = maxRotationSmoothnessStep;
			this._minRotationSmoothnessStep = minRotationSmoothnessStep;
			this._maxRotationSmoothnessStep = maxRotationSmoothnessStep;
			this.Fov = fov;
			this.MinFov = minFov;
			this.MaxFov = maxFov;
			this.FovStep = maxFovStep;
			this._minFovStep = minFovStep;
			this._maxFovStep = maxFovStep;
			this._fovSmoothness = fovSmoothness;
			this.MaxFovSmoothness = maxFovSmoothness;
			this.FovSmoothnessStep = maxFovSmoothnessStep;
			this._minFovSmoothnessStep = minFovSmoothnessStep;
			this._maxFovSmoothnessStep = maxFovSmoothnessStep;
			this.SnapAxis = snapAxis;
			this.UseTiltAsDirection = useTiltAsDirection;
			this.IsMouseInverted = isMouseInverted;
			this.ShowGUI = showGUI;
			this.DroneRecordingStateData = new DroneRecordingStateData();
		}

		public void BurstStarted()
		{
			this._previousMoveSpeed = this.MoveSpeed;
			this.MoveSpeed = this.MaxMoveSpeed;
		}

		public void BurstEnded()
		{
			this.MoveSpeed = this._previousMoveSpeed;
		}

		public void IncreaseFov()
		{
			this.Fov += this.FovStep;
			this.Fov = Mathf.Clamp(this.Fov, this.MinFov, this.MaxFov);
		}

		public void DecreaseFov()
		{
			this.Fov -= this.FovStep;
			this.Fov = Mathf.Clamp(this.Fov, this.MinFov, this.MaxFov);
		}

		public void MinimizeStepping()
		{
			this.MoveSpeedStep = this._minMoveSpeedStep;
			this.RotationSpeedStep = this._minRotationSpeedStep;
			this.MoveSmoothnessStep = this._minMoveSmoothnessStep;
			this.RotationSmoothnessStep = this._minRotationSmoothnessStep;
			this.FovStep = this._minFovStep;
			this.FovSmoothnessStep = this._minFovSmoothnessStep;
		}

		public void MaximizeStepping()
		{
			this.MoveSpeedStep = this._maxMoveSpeedStep;
			this.RotationSpeedStep = this._maxRotationSpeedStep;
			this.MoveSmoothnessStep = this._maxMoveSmoothnessStep;
			this.RotationSmoothnessStep = this._maxRotationSmoothnessStep;
			this.FovStep = this._maxFovStep;
			this.FovSmoothnessStep = this._maxFovSmoothnessStep;
		}

		public void ToggleShowGUI()
		{
			this.ShowGUI = !this.ShowGUI;
		}

		public void RecordButtonPressed()
		{
			DroneDataModel.OnDroneModelEvent onRecordButtonPressed = this.OnRecordButtonPressed;
			if (onRecordButtonPressed == null)
			{
				return;
			}
			onRecordButtonPressed();
		}

		private bool _isDroneModeActive;

		private bool _useKeyboard;

		private bool _useMouse;

		private bool _useGamepad;

		private float _moveSpeed;

		private readonly float _minMoveSpeedStep;

		private readonly float _maxMoveSpeedStep;

		private float _previousMoveSpeed;

		private float _moveSmoothness;

		private readonly float _minMoveSmoothnessStep;

		private readonly float _maxMoveSmoothnessStep;

		private float _rotationSpeed;

		private readonly float _minRotationSpeedStep;

		private readonly float _maxRotationSpeedStep;

		private float _rotationSmoothness;

		private readonly float _maxRotationSmoothnessStep;

		private readonly float _minRotationSmoothnessStep;

		private float _fov;

		private readonly float _minFovStep;

		private readonly float _maxFovStep;

		private float _fovSmoothness;

		private readonly float _minFovSmoothnessStep;

		private readonly float _maxFovSmoothnessStep;

		private bool _snapAxis;

		private bool _useTiltAsDirection;

		private bool _isMouseInverted;

		private bool _showGUI;

		public delegate void OnDroneModelEvent();

		public delegate void OnDroneModelBoolEvent(bool value);

		public delegate void OnDroneModelFloatEvent(float value);

		public delegate void OnRecordingStateDataEvent(DroneRecordingStateData value);
	}
}
