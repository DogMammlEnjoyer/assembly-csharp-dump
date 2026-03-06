using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/XR Screen Space Controller", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.XRScreenSpaceController.html")]
	[Obsolete("XRScreenSpaceController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.")]
	public class XRScreenSpaceController : XRBaseController
	{
		public bool enableTouchscreenGestureInputController
		{
			get
			{
				return this.m_EnableTouchscreenGestureInputController;
			}
			set
			{
				this.m_EnableTouchscreenGestureInputController = value;
			}
		}

		public InputActionProperty tapStartPositionAction
		{
			get
			{
				return this.m_TapStartPositionAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_TapStartPositionAction, value);
			}
		}

		public InputActionProperty dragCurrentPositionAction
		{
			get
			{
				return this.m_DragCurrentPositionAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_DragCurrentPositionAction, value);
			}
		}

		public InputActionProperty dragDeltaAction
		{
			get
			{
				return this.m_DragDeltaAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_DragDeltaAction, value);
			}
		}

		public InputActionProperty pinchStartPositionAction
		{
			get
			{
				return this.m_PinchStartPositionAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_PinchStartPositionAction, value);
			}
		}

		public InputActionProperty pinchGapAction
		{
			get
			{
				return this.m_PinchGapAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_PinchGapAction, value);
			}
		}

		public InputActionProperty pinchGapDeltaAction
		{
			get
			{
				return this.m_PinchGapDeltaAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_PinchGapDeltaAction, value);
			}
		}

		public InputActionProperty twistStartPositionAction
		{
			get
			{
				return this.m_TwistStartPositionAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_TwistStartPositionAction, value);
			}
		}

		public InputActionProperty twistDeltaRotationAction
		{
			get
			{
				return this.m_TwistDeltaRotationAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_TwistDeltaRotationAction, value);
			}
		}

		public InputActionProperty screenTouchCountAction
		{
			get
			{
				return this.m_ScreenTouchCountAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_ScreenTouchCountAction, value);
			}
		}

		public Camera controllerCamera
		{
			get
			{
				return this.m_ControllerCamera;
			}
			set
			{
				this.m_ControllerCamera = value;
			}
		}

		public bool blockInteractionsWithScreenSpaceUI
		{
			get
			{
				return this.m_BlockInteractionsWithScreenSpaceUI;
			}
			set
			{
				this.m_BlockInteractionsWithScreenSpaceUI = value;
			}
		}

		public bool useRotationThreshold
		{
			get
			{
				return this.m_UseRotationThreshold;
			}
			set
			{
				this.m_UseRotationThreshold = value;
			}
		}

		public float rotationThreshold
		{
			get
			{
				return this.m_RotationThreshold;
			}
			set
			{
				this.m_RotationThreshold = value;
			}
		}

		public float scaleDelta { get; private set; }

		[Obsolete("pinchStartPosition has been deprecated. Use pinchStartPositionAction instead. (UnityUpgradable) -> pinchStartPositionAction", true)]
		public InputActionProperty pinchStartPosition
		{
			get
			{
				return default(InputActionProperty);
			}
			set
			{
			}
		}

		[Obsolete("pinchGapDelta has been deprecated. Use pinchGapDeltaAction instead. (UnityUpgradable) -> pinchGapDeltaAction", true)]
		public InputActionProperty pinchGapDelta
		{
			get
			{
				return default(InputActionProperty);
			}
			set
			{
			}
		}

		[Obsolete("twistStartPosition has been deprecated. Use twistStartPositionAction instead. (UnityUpgradable) -> twistStartPositionAction", true)]
		public InputActionProperty twistStartPosition
		{
			get
			{
				return default(InputActionProperty);
			}
			set
			{
			}
		}

		[Obsolete("twistRotationDeltaAction has been deprecated. Use twistDeltaRotationAction instead. (UnityUpgradable) -> twistDeltaRotationAction", true)]
		public InputActionProperty twistRotationDeltaAction
		{
			get
			{
				return default(InputActionProperty);
			}
			set
			{
			}
		}

		[Obsolete("screenTouchCount has been deprecated. Use screenTouchCountAction instead. (UnityUpgradable) -> screenTouchCountAction", true)]
		public InputActionProperty screenTouchCount
		{
			get
			{
				return default(InputActionProperty);
			}
			set
			{
			}
		}

		protected void Start()
		{
			if (this.m_ControllerCamera == null)
			{
				this.m_ControllerCamera = Camera.main;
				if (this.m_ControllerCamera == null)
				{
					Debug.LogWarning("Could not find associated Camera in scene.This XRScreenSpaceController will be disabled.", this);
					base.enabled = false;
				}
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.EnableAllDirectActions();
			this.InitializeTouchscreenGestureController();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.DisableAllDirectActions();
			this.RemoveTouchscreenGestureController();
			this.m_UIInputModule = null;
		}

		protected override void UpdateTrackingInput(XRControllerState controllerState)
		{
			base.UpdateTrackingInput(controllerState);
			if (controllerState == null || this.IsPointerOverScreenSpaceCanvas())
			{
				return;
			}
			if (!this.m_HasCheckedDisabledTrackingInputReferenceActions && (this.m_DragCurrentPositionAction.action != null || this.m_TapStartPositionAction.action != null || this.m_TwistStartPositionAction.action != null))
			{
				if (XRScreenSpaceController.IsDisabledReferenceAction(this.m_DragCurrentPositionAction) || XRScreenSpaceController.IsDisabledReferenceAction(this.m_TapStartPositionAction) || XRScreenSpaceController.IsDisabledReferenceAction(this.m_TwistStartPositionAction))
				{
					Debug.LogWarning("'Enable Input Tracking' is enabled, but the Tap, Drag, Pinch, and/or Twist Action is disabled. The pose of the controller will not be updated correctly until the Input Actions are enabled. Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action. The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.", this);
				}
				this.m_HasCheckedDisabledTrackingInputReferenceActions = true;
			}
			InputAction action = this.m_ScreenTouchCountAction.action;
			int num = (action != null) ? action.ReadValue<int>() : 0;
			InputAction inputAction;
			if (this.TryGetCurrentPositionAction(num, out inputAction))
			{
				Vector2 vector = inputAction.ReadValue<Vector2>();
				Vector3 vector2 = this.m_ControllerCamera.ScreenToWorldPoint(new Vector3(vector.x, vector.y, this.m_ControllerCamera.nearClipPlane));
				Vector3 normalized = (vector2 - this.m_ControllerCamera.transform.position).normalized;
				controllerState.position = ((base.transform.parent != null) ? base.transform.parent.InverseTransformPoint(vector2) : vector2);
				controllerState.rotation = Quaternion.LookRotation(normalized);
				controllerState.inputTrackingState = (InputTrackingState.Position | InputTrackingState.Rotation);
				controllerState.isTracked = (num > 0);
				return;
			}
			controllerState.inputTrackingState = InputTrackingState.None;
			controllerState.isTracked = false;
		}

		protected override void UpdateInput(XRControllerState controllerState)
		{
			base.UpdateInput(controllerState);
			if (controllerState == null || this.IsPointerOverScreenSpaceCanvas())
			{
				return;
			}
			if (!this.m_HasCheckedDisabledInputReferenceActions && (this.m_TwistDeltaRotationAction.action != null || this.m_DragCurrentPositionAction.action != null || this.m_TapStartPositionAction.action != null))
			{
				if (XRScreenSpaceController.IsDisabledReferenceAction(this.m_TwistDeltaRotationAction) || XRScreenSpaceController.IsDisabledReferenceAction(this.m_DragCurrentPositionAction) || XRScreenSpaceController.IsDisabledReferenceAction(this.m_TapStartPositionAction))
				{
					Debug.LogWarning("'Enable Input Actions' is enabled, but the Tap, Drag, Pinch, and/or Twist Action is disabled. The controller input will not be handled correctly until the Input Actions are enabled. Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action. The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.", this);
				}
				this.m_HasCheckedDisabledInputReferenceActions = true;
			}
			controllerState.ResetFrameDependentStates();
			InputAction inputAction;
			InputAction inputAction2;
			if (this.TryGetCurrentTwoInputSelectAction(out inputAction))
			{
				controllerState.selectInteractionState.SetFrameState(true, inputAction.ReadValue<float>());
			}
			else if (this.TryGetCurrentOneInputSelectAction(out inputAction2))
			{
				controllerState.selectInteractionState.SetFrameState(true, inputAction2.ReadValue<Vector2>().magnitude);
			}
			else
			{
				controllerState.selectInteractionState.SetFrameState(false, 0f);
			}
			float num;
			if (this.m_UseRotationThreshold && XRScreenSpaceController.TryGetAbsoluteValue(this.m_TwistDeltaRotationAction.action, out num) && num >= this.m_RotationThreshold)
			{
				this.scaleDelta = 0f;
				return;
			}
			this.scaleDelta = ((this.m_PinchGapDeltaAction.action != null) ? (this.m_PinchGapDeltaAction.action.ReadValue<float>() / Screen.dpi) : 0f);
		}

		private bool TryGetCurrentPositionAction(int touchCount, out InputAction action)
		{
			if (touchCount <= 1)
			{
				if (this.m_DragCurrentPositionAction.action != null && this.m_DragCurrentPositionAction.action.IsInProgress())
				{
					action = this.m_DragCurrentPositionAction.action;
					return true;
				}
				if (this.m_TapStartPositionAction.action != null && this.m_TapStartPositionAction.action.WasPerformedThisFrame())
				{
					action = this.m_TapStartPositionAction.action;
					return true;
				}
			}
			action = null;
			return false;
		}

		private bool TryGetCurrentOneInputSelectAction(out InputAction action)
		{
			if (this.m_DragCurrentPositionAction.action != null && this.m_DragCurrentPositionAction.action.IsInProgress())
			{
				action = this.m_DragCurrentPositionAction.action;
				return true;
			}
			if (this.m_TapStartPositionAction.action != null && this.m_TapStartPositionAction.action.WasPerformedThisFrame())
			{
				action = this.m_TapStartPositionAction.action;
				return true;
			}
			action = null;
			return false;
		}

		private bool TryGetCurrentTwoInputSelectAction(out InputAction action)
		{
			if (this.m_PinchGapAction.action != null && this.m_PinchGapAction.action.IsInProgress())
			{
				action = this.m_PinchGapAction.action;
				return true;
			}
			if (this.m_PinchGapDeltaAction.action != null && this.m_PinchGapDeltaAction.action.IsInProgress())
			{
				action = this.m_PinchGapDeltaAction.action;
				return true;
			}
			if (this.m_TwistDeltaRotationAction.action != null && this.m_TwistDeltaRotationAction.action.IsInProgress())
			{
				action = this.m_TwistDeltaRotationAction.action;
				return true;
			}
			action = null;
			return false;
		}

		private static bool TryGetAbsoluteValue(InputAction action, out float value)
		{
			if (action != null && action.IsInProgress())
			{
				value = Mathf.Abs(action.ReadValue<float>());
				return true;
			}
			value = 0f;
			return false;
		}

		private bool FindUIInputModule()
		{
			EventSystem current = EventSystem.current;
			if (current != null && current.currentInputModule != null)
			{
				this.m_UIInputModule = (current.currentInputModule as UIInputModule);
			}
			return this.m_UIInputModule != null;
		}

		private bool IsPointerOverScreenSpaceCanvas()
		{
			if (!this.m_BlockInteractionsWithScreenSpaceUI || (!(this.m_UIInputModule != null) && !this.FindUIInputModule()))
			{
				return false;
			}
			GameObject currentGameObject = this.m_UIInputModule.GetCurrentGameObject(-1);
			if (currentGameObject == null)
			{
				return false;
			}
			RenderMode renderMode = currentGameObject.GetComponentInParent<Canvas>().renderMode;
			return renderMode == RenderMode.ScreenSpaceOverlay || renderMode == RenderMode.ScreenSpaceCamera;
		}

		private void InitializeTouchscreenGestureController()
		{
		}

		private void RemoveTouchscreenGestureController()
		{
		}

		private void EnableAllDirectActions()
		{
			this.m_TapStartPositionAction.EnableDirectAction();
			this.m_DragCurrentPositionAction.EnableDirectAction();
			this.m_DragDeltaAction.EnableDirectAction();
			this.m_PinchStartPositionAction.EnableDirectAction();
			this.m_PinchGapAction.EnableDirectAction();
			this.m_PinchGapDeltaAction.EnableDirectAction();
			this.m_TwistStartPositionAction.EnableDirectAction();
			this.m_TwistDeltaRotationAction.EnableDirectAction();
			this.m_ScreenTouchCountAction.EnableDirectAction();
		}

		private void DisableAllDirectActions()
		{
			this.m_TapStartPositionAction.DisableDirectAction();
			this.m_DragCurrentPositionAction.DisableDirectAction();
			this.m_DragDeltaAction.DisableDirectAction();
			this.m_PinchStartPositionAction.DisableDirectAction();
			this.m_PinchGapAction.DisableDirectAction();
			this.m_PinchGapDeltaAction.DisableDirectAction();
			this.m_TwistStartPositionAction.DisableDirectAction();
			this.m_TwistDeltaRotationAction.DisableDirectAction();
			this.m_ScreenTouchCountAction.DisableDirectAction();
		}

		private void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
		{
			if (Application.isPlaying)
			{
				property.DisableDirectAction();
			}
			property = value;
			if (Application.isPlaying && base.isActiveAndEnabled)
			{
				property.EnableDirectAction();
			}
		}

		private static bool IsDisabledReferenceAction(InputActionProperty property)
		{
			return property.reference != null && property.reference.action != null && !property.reference.action.enabled;
		}

		[Header("Touchscreen Gesture Actions")]
		[SerializeField]
		[Tooltip("When enabled, a Touchscreen Gesture Input Controller will be added to the Input System device list to detect touch gestures.")]
		private bool m_EnableTouchscreenGestureInputController = true;

		[SerializeField]
		[Tooltip("The action to use for the screen tap position. (Vector 2 Control).")]
		private InputActionProperty m_TapStartPositionAction = new InputActionProperty(new InputAction("Tap Start Position", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[Tooltip("The action to use for the current screen drag position. (Vector 2 Control).")]
		private InputActionProperty m_DragCurrentPositionAction = new InputActionProperty(new InputAction("Drag Current Position", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[Tooltip("The action to use for the delta of the screen drag. (Vector 2 Control).")]
		private InputActionProperty m_DragDeltaAction = new InputActionProperty(new InputAction("Drag Delta", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[FormerlySerializedAs("m_PinchStartPosition")]
		[Tooltip("The action to use for the screen pinch gesture start position. (Vector 2 Control).")]
		private InputActionProperty m_PinchStartPositionAction = new InputActionProperty(new InputAction("Pinch Start Position", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[Tooltip("The action to use for the gap of the screen pinch gesture. (Axis Control).")]
		private InputActionProperty m_PinchGapAction = new InputActionProperty(new InputAction(null, InputActionType.Value, null, null, null, "Axis"));

		[SerializeField]
		[Tooltip("The action to use for the delta of the screen pinch gesture. (Axis Control).")]
		private InputActionProperty m_PinchGapDeltaAction = new InputActionProperty(new InputAction("Pinch Gap Delta", InputActionType.Value, null, null, null, "Axis"));

		[SerializeField]
		[FormerlySerializedAs("m_TwistStartPosition")]
		[Tooltip("The action to use for the screen twist gesture start position. (Vector 2 Control).")]
		private InputActionProperty m_TwistStartPositionAction = new InputActionProperty(new InputAction("Twist Start Position", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		[FormerlySerializedAs("m_TwistRotationDeltaAction")]
		[Tooltip("The action to use for the delta of the screen twist gesture. (Axis Control).")]
		private InputActionProperty m_TwistDeltaRotationAction = new InputActionProperty(new InputAction("Twist Delta Rotation", InputActionType.Value, null, null, null, "Axis"));

		[SerializeField]
		[FormerlySerializedAs("m_ScreenTouchCount")]
		[Tooltip("The number of concurrent touches on the screen. (Integer Control).")]
		private InputActionProperty m_ScreenTouchCountAction = new InputActionProperty(new InputAction("Screen Touch Count", InputActionType.Value, null, null, null, "Integer"));

		[SerializeField]
		[Tooltip("The camera associated with the screen, and through which screen presses/touches will be interpreted.")]
		private Camera m_ControllerCamera;

		[SerializeField]
		[Tooltip("Tells the XR Screen Space Controller to ignore interactions when hitting a screen space canvas.")]
		private bool m_BlockInteractionsWithScreenSpaceUI = true;

		[SerializeField]
		[Tooltip("Enables a rotation threshold that blocks pinch scale gestures when surpassed.")]
		private bool m_UseRotationThreshold = true;

		[SerializeField]
		[Tooltip("The threshold at which a gestures will be interpreted only as rotation and not a pinch scale gesture.")]
		private float m_RotationThreshold = 0.02f;

		private bool m_HasCheckedDisabledTrackingInputReferenceActions;

		private bool m_HasCheckedDisabledInputReferenceActions;

		private UIInputModule m_UIInputModule;
	}
}
