using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class DroneGUI
	{
		public DroneGUI(DroneDataModel model, GUISkin skin)
		{
			this._model = model;
			this._guiSkin = skin;
			this._labelAlignRight = new GUIStyle(this._guiSkin.label);
			this._labelAlignRight.alignment = TextAnchor.MiddleRight;
			this._labelAlignRight.padding = new RectOffset(0, 0, -16, 0);
			this._labelAlignLeft = new GUIStyle(this._guiSkin.label);
			this._labelAlignLeft.alignment = TextAnchor.MiddleLeft;
			this._labelAlignLeft.padding = new RectOffset(0, 0, -16, 0);
			this._stepperSubButtonStyle = new GUIStyle(this._guiSkin.button);
			this._stepperSubButtonStyle.alignment = TextAnchor.MiddleCenter;
			this._stepperSubButtonStyle.normal.textColor = Color.white;
			this._stepperSubButtonStyle.normal.background = this._guiSkin.toggle.normal.background;
			this._stepperSubButtonStyle.hover.textColor = Color.white;
			this._stepperSubButtonStyle.hover.background = this._guiSkin.toggle.hover.background;
			this._stepperSubButtonStyle.active.textColor = Color.white;
			this._stepperSubButtonStyle.active.background = this._guiSkin.toggle.active.background;
			this._stepperSubButtonStyle.fontSize = 48;
			this._stepperSubButtonStyle.fixedWidth = 156f;
			this._stepperSubButtonStyle.contentOffset = new Vector2(4f, 0f);
			this._activateDeactivateButtonStyle = new GUIStyle(this._guiSkin.button);
			this._activateDeactivateButtonStyle.fixedWidth = 272f;
			this._activateDeactivateButtonStyle.contentOffset = new Vector2(8f, 0f);
			this._infoButtonStyle = new GUIStyle(this._stepperSubButtonStyle);
			this._infoButtonStyle.fixedWidth = 40f;
			this._infoButtonStyle.fixedHeight = 40f;
			this._infoButtonStyle.fontSize = 32;
			this._infoButtonStyle.contentOffset = new Vector2(2f, 0f);
			this._secondaryButtonStyle = new GUIStyle(this._infoButtonStyle);
			this._secondaryButtonStyle.fixedWidth = 320f;
			this._secondaryButtonStyle.contentOffset = new Vector2(8f, 0f);
			this._keyStyleNormal = new GUIStyle(this._guiSkin.label);
			this._keyStyleNormal.alignment = TextAnchor.MiddleCenter;
			this._keyStyleNormal.fontSize = 24;
			this._keyStyleNormal.fixedWidth = 24f;
			this._keyStyleNormal.fixedHeight = 24f;
			this._keyStyleNormal.contentOffset = new Vector2(3f, 0f);
			this._keyStyleNormal.normal.background = this._secondaryButtonStyle.normal.background;
			this._keyStyleActive = new GUIStyle(this._keyStyleNormal);
			this._keyStyleActive.normal.background = this._activateDeactivateButtonStyle.normal.background;
			this._keyStyleActive.normal.textColor = this._activateDeactivateButtonStyle.normal.textColor;
			this._keyStyleSpace = new GUIStyle(this._keyStyleActive);
			this._keyStyleSpace.fixedWidth = 144f;
			this._recordButtonStyle = new GUIStyle(this._activateDeactivateButtonStyle);
			this._recordButtonStyle.fixedWidth = 320f;
			DroneGUI.KeyData[] array = new DroneGUI.KeyData[]
			{
				new DroneGUI.KeyData
				{
					Letter = "Q",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = "W",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = "E",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = "A",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = "S",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = "D",
					IsActive = false,
					Show = true
				}
			};
			DroneGUI.KeyData[] array2 = new DroneGUI.KeyData[]
			{
				new DroneGUI.KeyData
				{
					Letter = " ",
					IsActive = false,
					Show = false
				},
				new DroneGUI.KeyData
				{
					Letter = "[",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = " ",
					IsActive = false,
					Show = false
				},
				new DroneGUI.KeyData
				{
					Letter = "<",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = "]",
					IsActive = false,
					Show = true
				},
				new DroneGUI.KeyData
				{
					Letter = ">",
					IsActive = false,
					Show = true
				}
			};
			Array.Copy(array, this._movementKeysW, array.Length);
			this._movementKeysW[1].IsActive = true;
			Array.Copy(array, this._movementKeysS, array.Length);
			this._movementKeysS[4].IsActive = true;
			Array.Copy(array, this._movementKeysA, array.Length);
			this._movementKeysA[3].IsActive = true;
			Array.Copy(array, this._movementKeysD, array.Length);
			this._movementKeysD[5].IsActive = true;
			Array.Copy(array, this._movementKeysQ, array.Length);
			this._movementKeysQ[0].IsActive = true;
			Array.Copy(array, this._movementKeysE, array.Length);
			this._movementKeysE[2].IsActive = true;
			Array.Copy(array2, this._rotationKeysLeft, array2.Length);
			this._rotationKeysLeft[3].IsActive = true;
			Array.Copy(array2, this._rotationKeysRight, array2.Length);
			this._rotationKeysRight[5].IsActive = true;
			Array.Copy(array2, this._rotationKeysUp, array2.Length);
			this._rotationKeysUp[1].IsActive = true;
			Array.Copy(array2, this._rotationKeysDown, array2.Length);
			this._rotationKeysDown[4].IsActive = true;
		}

		public void SetRecordButtonState(RecordingState state)
		{
			this._recordButtonStyle.normal = ((state == RecordingState.Recording) ? this._guiSkin.button.onNormal : this._guiSkin.button.normal);
			this._recordButtonStyle.hover = ((state == RecordingState.Recording) ? this._guiSkin.button.onHover : this._guiSkin.button.hover);
			this._recordButtonStyle.active = ((state == RecordingState.Recording) ? this._guiSkin.button.onActive : this._guiSkin.button.active);
		}

		public void Run()
		{
			GUI.skin = this._guiSkin;
			GUI.BeginGroup(new Rect((float)(Screen.width - 368), 0f, 368f, 1080f));
			GUI.Box(new Rect(0f, 0f, 368f, 1080f), "");
			if (this._currentUIMode == DroneGUI.UIMode.Settings)
			{
				this.RenderSettingsUIMode();
			}
			else
			{
				this.RenderHelpUIMode();
			}
			GUI.Label(new Rect(24f, 1032f, 368f, 48f), "PRESS <color=#FFC23F>TAB</color> TO SHOW/HIDE UI");
			GUI.EndGroup();
		}

		private void RenderSettingsUIMode()
		{
			GUI.Label(new Rect(24f, 8f, 368f, 40f), "LIV FREE CAMERA");
			if (GUI.Button(new Rect(24f, 72f, 272f, 40f), this._model.IsDroneModeActive ? "DEACTIVATE" : "ACTIVATE", this._activateDeactivateButtonStyle))
			{
				this.ToggleDroneMode();
				bool isDroneModeActive = this._model.IsDroneModeActive;
				this._activateDeactivateButtonStyle.normal = (isDroneModeActive ? this._guiSkin.button.onNormal : this._guiSkin.button.normal);
				this._activateDeactivateButtonStyle.hover = (isDroneModeActive ? this._guiSkin.button.onHover : this._guiSkin.button.hover);
				this._activateDeactivateButtonStyle.active = (isDroneModeActive ? this._guiSkin.button.onActive : this._guiSkin.button.active);
			}
			if (GUI.Button(new Rect(304f, 72f, 40f, 40f), "?", this._infoButtonStyle))
			{
				this._currentUIMode = DroneGUI.UIMode.Help;
			}
			this._model.UseKeyboard = this.ToggleUI(this._model.UseKeyboard, 136f, "KEYBOARD");
			this._model.UseMouse = this.ToggleUI(this._model.UseMouse, 176f, "MOUSE");
			this._model.UseGamepad = this.ToggleUI(this._model.UseGamepad, 216f, "GAMEPAD");
			this._model.MoveSpeed = this.StepperUI(this._model.MoveSpeed, this._model.MoveSpeedStep, 280f, "MOVE SPEED", 0.1f, this._model.MaxMoveSpeed);
			this._model.MoveSmoothness = this.StepperUI(this._model.MoveSmoothness, this._model.MoveSmoothnessStep, 362f, "MOVE SMOOTHNESS", 0f, this._model.MaxMoveSmoothness);
			this._model.RotationSpeed = this.StepperUI(this._model.RotationSpeed, this._model.RotationSpeedStep, 460f, "ROTATION SPEED", 1f, this._model.MaxRotationSpeed);
			this._model.RotationSmoothness = this.StepperUI(this._model.RotationSmoothness, this._model.RotationSmoothnessStep, 542f, "ROTATION SMOOTHNESS", 0f, this._model.MaxRotationSmoothness);
			this._model.Fov = this.StepperUI(this._model.Fov, this._model.FovStep, 640f, "FOV", this._model.MinFov, this._model.MaxFov);
			this._model.FovSmoothness = this.StepperUI(this._model.FovSmoothness, this._model.FovSmoothnessStep, 722f, "FOV SMOOTHNESS", 0f, this._model.MaxFovSmoothness);
			this._model.UseTiltAsDirection = this.ToggleUI(this._model.UseTiltAsDirection, 820f, "USE TILT AS DIRECTION");
			this._model.SnapAxis = this.ToggleUI(this._model.SnapAxis, 860f, "SNAP AXIS");
			this._model.IsMouseInverted = this.ToggleUI(this._model.IsMouseInverted, 900f, "INVERT MOUSE");
			if (this._model.IsDroneModeActive && GUI.Button(new Rect(24f, 964f, 272f, 40f), this.GetRecordingButtonText(this._model.DroneRecordingStateData.State), this._recordButtonStyle))
			{
				this._model.RecordButtonPressed();
			}
		}

		private void RenderHelpUIMode()
		{
			if (GUI.Button(new Rect(24f, 24f, 320f, 40f), "LEARN MORE", this._secondaryButtonStyle))
			{
				Application.OpenURL("https://gorillatag.fandom.com/wiki/LIV_Camera");
			}
			if (GUI.Button(new Rect(24f, 72f, 320f, 40f), "BACK TO SETTINGS", this._secondaryButtonStyle))
			{
				this._currentUIMode = DroneGUI.UIMode.Settings;
			}
			GUI.Label(new Rect(24f, 136f, 320f, 48f), "MOVEMENT");
			this.KeysGroup(this._movementKeysW, new Vector2(24f, 200f), "FORW");
			this.KeysGroup(this._movementKeysS, new Vector2(188f, 200f), "BACK");
			this.KeysGroup(this._movementKeysA, new Vector2(24f, 316f), "LEFT");
			this.KeysGroup(this._movementKeysD, new Vector2(188f, 316f), "RIGHT");
			this.KeysGroup(this._movementKeysQ, new Vector2(24f, 432f), "UP");
			this.KeysGroup(this._movementKeysE, new Vector2(188f, 432f), "DOWN");
			GUI.Label(new Rect(24f, 564f, 320f, 48f), "SPACE", this._keyStyleSpace);
			GUI.Label(new Rect(188f, 556f, 320f, 48f), "SPEED UP");
			GUI.Label(new Rect(24f, 628f, 320f, 48f), "ROTATION");
			this.KeysGroup(this._rotationKeysLeft, new Vector2(24f, 692f), "LEFT");
			this.KeysGroup(this._rotationKeysRight, new Vector2(188f, 692f), "RIGHT");
			this.KeysGroup(this._rotationKeysUp, new Vector2(24f, 808f), "UP");
			this.KeysGroup(this._rotationKeysDown, new Vector2(188f, 808f), "DOWN");
		}

		private void ToggleDroneMode()
		{
			this._model.IsDroneModeActive = !this._model.IsDroneModeActive;
		}

		private string GetRecordingButtonText(RecordingState state)
		{
			string result;
			switch (state)
			{
			case RecordingState.Idle:
				result = "RECORD";
				break;
			case RecordingState.Recording:
				result = this._model.DroneRecordingStateData.FormattedDuration;
				break;
			case RecordingState.Saving:
				result = "SAVING";
				break;
			default:
				result = "UNKNOWN";
				break;
			}
			return result;
		}

		private bool ToggleUI(bool value, float yOffset, string label)
		{
			value = GUI.Toggle(new Rect(24f, yOffset, 320f, 48f), value, label);
			return value;
		}

		private float StepperUI(float value, float step, float yOffset, string label, float min, float max)
		{
			GUI.BeginGroup(new Rect(24f, yOffset, 320f, 74f));
			GUI.Label(new Rect(0f, 0f, 320f, 32f), label, this._labelAlignLeft);
			GUI.Label(new Rect(0f, 0f, 160f, 32f), value.ToString("F1"), this._labelAlignRight);
			if (GUI.Button(new Rect(0f, 34f, 156f, 40f), "-", this._stepperSubButtonStyle))
			{
				value -= step;
				value = Mathf.Max(value, min);
			}
			if (GUI.Button(new Rect(164f, 34f, 156f, 40f), "+", this._stepperSubButtonStyle))
			{
				value += step;
				value = Mathf.Min(value, max);
			}
			GUI.EndGroup();
			return value;
		}

		private void Key(DroneGUI.KeyData data, Vector2 position)
		{
			if (!data.Show)
			{
				return;
			}
			GUI.Label(new Rect(position.x, position.y, 24f, 24f), data.Letter, data.IsActive ? this._keyStyleActive : this._keyStyleNormal);
		}

		private void KeysGroup(DroneGUI.KeyData[] keys, Vector2 position, string label)
		{
			GUI.BeginGroup(new Rect(position.x, position.y, 80f, 100f));
			this.Key(keys[0], new Vector2(0f, 0f));
			this.Key(keys[1], new Vector2(28f, 0f));
			this.Key(keys[2], new Vector2(56f, 0f));
			this.Key(keys[3], new Vector2(0f, 28f));
			this.Key(keys[4], new Vector2(28f, 28f));
			this.Key(keys[5], new Vector2(56f, 28f));
			GUI.Label(new Rect(0f, 56f, 80f, 48f), label);
			GUI.EndGroup();
		}

		private GUISkin _guiSkin;

		private DroneDataModel _model;

		private const int PANEL_WIDTH = 368;

		private const int PANEL_HEIGHT = 1080;

		private const int STEPPER_SUBBUTTON_WIDTH = 156;

		private const int BUTTON_HEIGHT = 40;

		private const int PRIMARY_OFFSET = 24;

		private const int KEYS_GROUP_SECONDARY_OFFSET = 188;

		private const int PADDING_TOP = 8;

		private bool _isDroneModeActive;

		private GUIStyle _labelAlignRight;

		private GUIStyle _labelAlignLeft;

		private GUIStyle _stepperSubButtonStyle;

		private GUIStyle _activateDeactivateButtonStyle;

		private GUIStyle _recordButtonStyle;

		private GUIStyle _infoButtonStyle;

		private GUIStyle _secondaryButtonStyle;

		private GUIStyle _keyStyleNormal;

		private GUIStyle _keyStyleActive;

		private GUIStyle _keyStyleSpace;

		private const string HELP_WEBSITE = "https://gorillatag.fandom.com/wiki/LIV_Camera";

		private DroneGUI.KeyData[] _movementKeysW = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _movementKeysS = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _movementKeysA = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _movementKeysD = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _movementKeysQ = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _movementKeysE = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _rotationKeysUp = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _rotationKeysDown = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _rotationKeysLeft = new DroneGUI.KeyData[6];

		private DroneGUI.KeyData[] _rotationKeysRight = new DroneGUI.KeyData[6];

		private DroneGUI.UIMode _currentUIMode;

		private enum UIMode
		{
			Settings,
			Help
		}

		private struct KeyData
		{
			public string Letter;

			public bool IsActive;

			public bool Show;
		}
	}
}
