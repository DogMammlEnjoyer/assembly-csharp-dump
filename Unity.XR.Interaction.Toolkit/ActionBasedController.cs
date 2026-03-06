using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("/XR Controller (Action-based)", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.ActionBasedController.html")]
	[Obsolete("ActionBasedController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.")]
	public class ActionBasedController : XRBaseController
	{
		[Obsolete("Deprecated, this obsolete property is not used when Input System version is 1.1.0 or higher. Configure press point on the action or binding instead.", true)]
		public float buttonPressPoint
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public InputActionProperty positionAction
		{
			get
			{
				return this.m_PositionAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_PositionAction, value);
			}
		}

		public InputActionProperty rotationAction
		{
			get
			{
				return this.m_RotationAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_RotationAction, value);
			}
		}

		public InputActionProperty isTrackedAction
		{
			get
			{
				return this.m_IsTrackedAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_IsTrackedAction, value);
			}
		}

		public InputActionProperty trackingStateAction
		{
			get
			{
				return this.m_TrackingStateAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_TrackingStateAction, value);
			}
		}

		public InputActionProperty selectAction
		{
			get
			{
				return this.m_SelectAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_SelectAction, value);
			}
		}

		public InputActionProperty selectActionValue
		{
			get
			{
				return this.m_SelectActionValue;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_SelectActionValue, value);
			}
		}

		public InputActionProperty activateAction
		{
			get
			{
				return this.m_ActivateAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_ActivateAction, value);
			}
		}

		public InputActionProperty activateActionValue
		{
			get
			{
				return this.m_ActivateActionValue;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_ActivateActionValue, value);
			}
		}

		public InputActionProperty uiPressAction
		{
			get
			{
				return this.m_UIPressAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_UIPressAction, value);
			}
		}

		public InputActionProperty uiPressActionValue
		{
			get
			{
				return this.m_UIPressActionValue;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_UIPressActionValue, value);
			}
		}

		public InputActionProperty uiScrollAction
		{
			get
			{
				return this.m_UIScrollAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_UIScrollAction, value);
			}
		}

		public InputActionProperty hapticDeviceAction
		{
			get
			{
				return this.m_HapticDeviceAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_HapticDeviceAction, value);
			}
		}

		public InputActionProperty rotateAnchorAction
		{
			get
			{
				return this.m_RotateAnchorAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_RotateAnchorAction, value);
			}
		}

		public InputActionProperty directionalAnchorRotationAction
		{
			get
			{
				return this.m_DirectionalAnchorRotationAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_DirectionalAnchorRotationAction, value);
			}
		}

		public InputActionProperty translateAnchorAction
		{
			get
			{
				return this.m_TranslateAnchorAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_TranslateAnchorAction, value);
			}
		}

		public InputActionProperty scaleToggleAction
		{
			get
			{
				return this.m_ScaleToggleAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_ScaleToggleAction, value);
			}
		}

		public InputActionProperty scaleDeltaAction
		{
			get
			{
				return this.m_ScaleDeltaAction;
			}
			set
			{
				this.SetInputActionProperty(ref this.m_ScaleDeltaAction, value);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.EnableAllDirectActions();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.DisableAllDirectActions();
		}

		protected override void UpdateTrackingInput(XRControllerState controllerState)
		{
			base.UpdateTrackingInput(controllerState);
			if (controllerState == null)
			{
				return;
			}
			InputAction action = this.m_PositionAction.action;
			InputAction action2 = this.m_RotationAction.action;
			InputAction action3 = this.m_IsTrackedAction.action;
			InputAction action4 = this.m_TrackingStateAction.action;
			if (!this.m_HasCheckedDisabledTrackingInputReferenceActions && (action != null || action2 != null))
			{
				if (ActionBasedController.IsDisabledReferenceAction(this.m_PositionAction) || ActionBasedController.IsDisabledReferenceAction(this.m_RotationAction))
				{
					Debug.LogWarning("'Enable Input Tracking' is enabled, but Position and/or Rotation Action is disabled. The pose of the controller will not be updated correctly until the Input Actions are enabled. Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action. The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.", this);
				}
				this.m_HasCheckedDisabledTrackingInputReferenceActions = true;
			}
			controllerState.isTracked = false;
			controllerState.inputTrackingState = InputTrackingState.None;
			if (action3 != null && action3.bindings.Count > 0)
			{
				controllerState.isTracked = this.IsPressed(action3);
			}
			else
			{
				object obj;
				if (action4 == null)
				{
					obj = null;
				}
				else
				{
					InputControl activeControl = action4.activeControl;
					obj = ((activeControl != null) ? activeControl.device : null);
				}
				TrackedDevice trackedDevice = obj as TrackedDevice;
				if (trackedDevice != null)
				{
					controllerState.isTracked = trackedDevice.isTracked.isPressed;
				}
				else
				{
					object obj2;
					if (action == null)
					{
						obj2 = null;
					}
					else
					{
						InputControl activeControl2 = action.activeControl;
						obj2 = ((activeControl2 != null) ? activeControl2.device : null);
					}
					TrackedDevice trackedDevice2 = obj2 as TrackedDevice;
					object obj3;
					if (action2 == null)
					{
						obj3 = null;
					}
					else
					{
						InputControl activeControl3 = action2.activeControl;
						obj3 = ((activeControl3 != null) ? activeControl3.device : null);
					}
					TrackedDevice trackedDevice3 = obj3 as TrackedDevice;
					bool flag = trackedDevice2 != null && trackedDevice2.isTracked.isPressed;
					if (trackedDevice2 != trackedDevice3)
					{
						bool flag2 = trackedDevice3 != null && trackedDevice3.isTracked.isPressed;
						controllerState.isTracked = (flag && flag2);
					}
					else
					{
						controllerState.isTracked = flag;
					}
				}
			}
			if (action4 != null && action4.bindings.Count > 0)
			{
				controllerState.inputTrackingState = (InputTrackingState)action4.ReadValue<int>();
			}
			else
			{
				object obj4;
				if (action3 == null)
				{
					obj4 = null;
				}
				else
				{
					InputControl activeControl4 = action3.activeControl;
					obj4 = ((activeControl4 != null) ? activeControl4.device : null);
				}
				TrackedDevice trackedDevice4 = obj4 as TrackedDevice;
				if (trackedDevice4 != null)
				{
					controllerState.inputTrackingState = (InputTrackingState)trackedDevice4.trackingState.ReadValue();
				}
				else
				{
					object obj5;
					if (action == null)
					{
						obj5 = null;
					}
					else
					{
						InputControl activeControl5 = action.activeControl;
						obj5 = ((activeControl5 != null) ? activeControl5.device : null);
					}
					TrackedDevice trackedDevice5 = obj5 as TrackedDevice;
					object obj6;
					if (action2 == null)
					{
						obj6 = null;
					}
					else
					{
						InputControl activeControl6 = action2.activeControl;
						obj6 = ((activeControl6 != null) ? activeControl6.device : null);
					}
					TrackedDevice trackedDevice6 = obj6 as TrackedDevice;
					InputTrackingState inputTrackingState = (InputTrackingState)((trackedDevice5 != null) ? trackedDevice5.trackingState.ReadValue() : 0);
					if (trackedDevice5 != trackedDevice6)
					{
						InputTrackingState inputTrackingState2 = (InputTrackingState)((trackedDevice6 != null) ? trackedDevice6.trackingState.ReadValue() : 0);
						controllerState.inputTrackingState = ((inputTrackingState & InputTrackingState.Position) | (inputTrackingState2 & InputTrackingState.Rotation));
					}
					else
					{
						controllerState.inputTrackingState = inputTrackingState;
					}
				}
			}
			if (action != null && (controllerState.inputTrackingState & InputTrackingState.Position) != InputTrackingState.None)
			{
				controllerState.position = action.ReadValue<Vector3>();
			}
			if (action2 != null && (controllerState.inputTrackingState & InputTrackingState.Rotation) != InputTrackingState.None)
			{
				controllerState.rotation = action2.ReadValue<Quaternion>();
			}
		}

		protected override void UpdateInput(XRControllerState controllerState)
		{
			base.UpdateInput(controllerState);
			if (controllerState == null)
			{
				return;
			}
			if (!this.m_HasCheckedDisabledInputReferenceActions && (this.m_SelectAction.action != null || this.m_ActivateAction.action != null || this.m_UIPressAction.action != null))
			{
				if (ActionBasedController.IsDisabledReferenceAction(this.m_SelectAction) || ActionBasedController.IsDisabledReferenceAction(this.m_ActivateAction) || ActionBasedController.IsDisabledReferenceAction(this.m_UIPressAction))
				{
					Debug.LogWarning("'Enable Input Actions' is enabled, but Select, Activate, and/or UI Press Action is disabled. The controller input will not be handled correctly until the Input Actions are enabled. Input Actions in an Input Action Asset must be explicitly enabled to read the current value of the action. The Input Action Manager behavior can be added to a GameObject in a Scene and used to enable all Input Actions in a referenced Input Action Asset.", this);
				}
				this.m_HasCheckedDisabledInputReferenceActions = true;
			}
			controllerState.ResetFrameDependentStates();
			InputAction action = this.m_SelectActionValue.action;
			if (action == null || action.bindings.Count <= 0)
			{
				action = this.m_SelectAction.action;
			}
			controllerState.selectInteractionState.SetFrameState(this.IsPressed(this.m_SelectAction.action), this.ReadValue(action));
			InputAction action2 = this.m_ActivateActionValue.action;
			if (action2 == null || action2.bindings.Count <= 0)
			{
				action2 = this.m_ActivateAction.action;
			}
			controllerState.activateInteractionState.SetFrameState(this.IsPressed(this.m_ActivateAction.action), this.ReadValue(action2));
			InputAction action3 = this.m_UIPressActionValue.action;
			if (action3 == null || action3.bindings.Count <= 0)
			{
				action3 = this.m_UIPressAction.action;
			}
			controllerState.uiPressInteractionState.SetFrameState(this.IsPressed(this.m_UIPressAction.action), this.ReadValue(action3));
			InputAction action4 = this.m_UIScrollAction.action;
			if (action4 != null)
			{
				controllerState.uiScrollValue = action4.ReadValue<Vector2>();
			}
		}

		protected virtual bool IsPressed(InputAction action)
		{
			if (action == null)
			{
				return false;
			}
			InputActionPhase phase = action.phase;
			return phase == InputActionPhase.Performed || (phase != InputActionPhase.Disabled && action.WasPerformedThisFrame());
		}

		protected virtual float ReadValue(InputAction action)
		{
			if (action == null)
			{
				return 0f;
			}
			if (action.activeControl is AxisControl)
			{
				return action.ReadValue<float>();
			}
			if (action.activeControl is Vector2Control)
			{
				return action.ReadValue<Vector2>().magnitude;
			}
			if (!this.IsPressed(action))
			{
				return 0f;
			}
			return 1f;
		}

		public override bool SendHapticImpulse(float amplitude, float duration)
		{
			IXRHapticImpulseChannelGroup channelGroup = this.m_HapticControlActionManager.GetChannelGroup(this.m_HapticDeviceAction.action);
			bool? flag;
			if (channelGroup == null)
			{
				flag = null;
			}
			else
			{
				IXRHapticImpulseChannel channel = channelGroup.GetChannel(0);
				flag = ((channel != null) ? new bool?(channel.SendHapticImpulse(amplitude, duration, 0f)) : null);
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}

		private void EnableAllDirectActions()
		{
			this.m_PositionAction.EnableDirectAction();
			this.m_RotationAction.EnableDirectAction();
			this.m_IsTrackedAction.EnableDirectAction();
			this.m_TrackingStateAction.EnableDirectAction();
			this.m_SelectAction.EnableDirectAction();
			this.m_SelectActionValue.EnableDirectAction();
			this.m_ActivateAction.EnableDirectAction();
			this.m_ActivateActionValue.EnableDirectAction();
			this.m_UIPressAction.EnableDirectAction();
			this.m_UIPressActionValue.EnableDirectAction();
			this.m_UIScrollAction.EnableDirectAction();
			this.m_HapticDeviceAction.EnableDirectAction();
			this.m_RotateAnchorAction.EnableDirectAction();
			this.m_DirectionalAnchorRotationAction.EnableDirectAction();
			this.m_TranslateAnchorAction.EnableDirectAction();
			this.m_ScaleToggleAction.EnableDirectAction();
			this.m_ScaleDeltaAction.EnableDirectAction();
		}

		private void DisableAllDirectActions()
		{
			this.m_PositionAction.DisableDirectAction();
			this.m_RotationAction.DisableDirectAction();
			this.m_IsTrackedAction.DisableDirectAction();
			this.m_TrackingStateAction.DisableDirectAction();
			this.m_SelectAction.DisableDirectAction();
			this.m_SelectActionValue.DisableDirectAction();
			this.m_ActivateAction.DisableDirectAction();
			this.m_ActivateActionValue.DisableDirectAction();
			this.m_UIPressAction.DisableDirectAction();
			this.m_UIPressActionValue.DisableDirectAction();
			this.m_UIScrollAction.DisableDirectAction();
			this.m_HapticDeviceAction.DisableDirectAction();
			this.m_RotateAnchorAction.DisableDirectAction();
			this.m_DirectionalAnchorRotationAction.DisableDirectAction();
			this.m_TranslateAnchorAction.DisableDirectAction();
			this.m_ScaleToggleAction.DisableDirectAction();
			this.m_ScaleDeltaAction.DisableDirectAction();
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

		[SerializeField]
		private InputActionProperty m_PositionAction = new InputActionProperty(new InputAction("Position", InputActionType.Value, null, null, null, "Vector3"));

		[SerializeField]
		private InputActionProperty m_RotationAction = new InputActionProperty(new InputAction("Rotation", InputActionType.Value, null, null, null, "Quaternion"));

		[SerializeField]
		private InputActionProperty m_IsTrackedAction = new InputActionProperty(new InputAction("Is Tracked", InputActionType.Button, null, null, null, null)
		{
			wantsInitialStateCheck = true
		});

		[SerializeField]
		private InputActionProperty m_TrackingStateAction = new InputActionProperty(new InputAction("Tracking State", InputActionType.Value, null, null, null, "Integer"));

		[SerializeField]
		private InputActionProperty m_SelectAction = new InputActionProperty(new InputAction("Select", InputActionType.Button, null, null, null, null));

		[SerializeField]
		private InputActionProperty m_SelectActionValue = new InputActionProperty(new InputAction("Select Value", InputActionType.Value, null, null, null, "Axis"));

		[SerializeField]
		private InputActionProperty m_ActivateAction = new InputActionProperty(new InputAction("Activate", InputActionType.Button, null, null, null, null));

		[SerializeField]
		private InputActionProperty m_ActivateActionValue = new InputActionProperty(new InputAction("Activate Value", InputActionType.Value, null, null, null, "Axis"));

		[SerializeField]
		private InputActionProperty m_UIPressAction = new InputActionProperty(new InputAction("UI Press", InputActionType.Button, null, null, null, null));

		[SerializeField]
		private InputActionProperty m_UIPressActionValue = new InputActionProperty(new InputAction("UI Press Value", InputActionType.Value, null, null, null, "Axis"));

		[SerializeField]
		private InputActionProperty m_UIScrollAction = new InputActionProperty(new InputAction("UI Scroll", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		private InputActionProperty m_HapticDeviceAction = new InputActionProperty(new InputAction("Haptic Device", InputActionType.PassThrough, null, null, null, null));

		[SerializeField]
		private InputActionProperty m_RotateAnchorAction = new InputActionProperty(new InputAction("Rotate Anchor", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		private InputActionProperty m_DirectionalAnchorRotationAction = new InputActionProperty(new InputAction("Directional Anchor Rotation", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		private InputActionProperty m_TranslateAnchorAction = new InputActionProperty(new InputAction("Translate Anchor", InputActionType.Value, null, null, null, "Vector2"));

		[SerializeField]
		private InputActionProperty m_ScaleToggleAction = new InputActionProperty(new InputAction("Scale Toggle", InputActionType.Button, null, null, null, null));

		[SerializeField]
		private InputActionProperty m_ScaleDeltaAction = new InputActionProperty(new InputAction("Scale Delta", InputActionType.Value, null, null, null, "Vector2"));

		private bool m_HasCheckedDisabledTrackingInputReferenceActions;

		private bool m_HasCheckedDisabledInputReferenceActions;

		private readonly HapticControlActionManager m_HapticControlActionManager = new HapticControlActionManager();
	}
}
