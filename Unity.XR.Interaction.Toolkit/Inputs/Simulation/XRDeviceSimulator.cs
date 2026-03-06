using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[AddComponentMenu("XR/Debug/XR Device Simulator", 11)]
	[DefaultExecutionOrder(-29991)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator.html")]
	public class XRDeviceSimulator : MonoBehaviour
	{
		public InputActionAsset deviceSimulatorActionAsset
		{
			get
			{
				return this.m_DeviceSimulatorActionAsset;
			}
			set
			{
				this.m_DeviceSimulatorActionAsset = value;
			}
		}

		public InputActionAsset controllerActionAsset
		{
			get
			{
				return this.m_ControllerActionAsset;
			}
			set
			{
				this.m_ControllerActionAsset = value;
			}
		}

		public InputActionReference keyboardXTranslateAction
		{
			get
			{
				return this.m_KeyboardXTranslateAction;
			}
			set
			{
				this.UnsubscribeKeyboardXTranslateAction();
				this.m_KeyboardXTranslateAction = value;
				this.SubscribeKeyboardXTranslateAction();
			}
		}

		public InputActionReference keyboardYTranslateAction
		{
			get
			{
				return this.m_KeyboardYTranslateAction;
			}
			set
			{
				this.UnsubscribeKeyboardYTranslateAction();
				this.m_KeyboardYTranslateAction = value;
				this.SubscribeKeyboardYTranslateAction();
			}
		}

		public InputActionReference keyboardZTranslateAction
		{
			get
			{
				return this.m_KeyboardZTranslateAction;
			}
			set
			{
				this.UnsubscribeKeyboardZTranslateAction();
				this.m_KeyboardZTranslateAction = value;
				this.SubscribeKeyboardZTranslateAction();
			}
		}

		public InputActionReference manipulateLeftAction
		{
			get
			{
				return this.m_ManipulateLeftAction;
			}
			set
			{
				this.UnsubscribeManipulateLeftAction();
				this.m_ManipulateLeftAction = value;
				this.SubscribeManipulateLeftAction();
			}
		}

		public InputActionReference manipulateRightAction
		{
			get
			{
				return this.m_ManipulateRightAction;
			}
			set
			{
				this.UnsubscribeManipulateRightAction();
				this.m_ManipulateRightAction = value;
				this.SubscribeManipulateRightAction();
			}
		}

		public InputActionReference toggleManipulateLeftAction
		{
			get
			{
				return this.m_ToggleManipulateLeftAction;
			}
			set
			{
				this.UnsubscribeToggleManipulateLeftAction();
				this.m_ToggleManipulateLeftAction = value;
				this.SubscribeToggleManipulateLeftAction();
			}
		}

		public InputActionReference toggleManipulateRightAction
		{
			get
			{
				return this.m_ToggleManipulateRightAction;
			}
			set
			{
				this.UnsubscribeToggleManipulateRightAction();
				this.m_ToggleManipulateRightAction = value;
				this.SubscribeToggleManipulateRightAction();
			}
		}

		public InputActionReference toggleManipulateBodyAction
		{
			get
			{
				return this.m_ToggleManipulateBodyAction;
			}
			set
			{
				this.UnsubscribeToggleManipulateBodyAction();
				this.m_ToggleManipulateBodyAction = value;
				this.SubscribeToggleManipulateBodyAction();
			}
		}

		public InputActionReference manipulateHeadAction
		{
			get
			{
				return this.m_ManipulateHeadAction;
			}
			set
			{
				this.UnsubscribeManipulateHeadAction();
				this.m_ManipulateHeadAction = value;
				this.SubscribeManipulateHeadAction();
			}
		}

		public InputActionReference handControllerModeAction
		{
			get
			{
				return this.m_HandControllerModeAction;
			}
			set
			{
				this.UnsubscribeHandControllerModeAction();
				this.m_HandControllerModeAction = value;
				this.SubscribeHandControllerModeAction();
			}
		}

		public InputActionReference cycleDevicesAction
		{
			get
			{
				return this.m_CycleDevicesAction;
			}
			set
			{
				this.UnsubscribeCycleDevicesAction();
				this.m_CycleDevicesAction = value;
				this.SubscribeCycleDevicesAction();
			}
		}

		public InputActionReference stopManipulationAction
		{
			get
			{
				return this.m_StopManipulationAction;
			}
			set
			{
				this.UnsubscribeStopManipulationAction();
				this.m_StopManipulationAction = value;
				this.SubscribeStopManipulationAction();
			}
		}

		public InputActionReference mouseDeltaAction
		{
			get
			{
				return this.m_MouseDeltaAction;
			}
			set
			{
				this.UnsubscribeMouseDeltaAction();
				this.m_MouseDeltaAction = value;
				this.SubscribeMouseDeltaAction();
			}
		}

		public InputActionReference mouseScrollAction
		{
			get
			{
				return this.m_MouseScrollAction;
			}
			set
			{
				this.UnsubscribeMouseScrollAction();
				this.m_MouseScrollAction = value;
				this.SubscribeMouseScrollAction();
			}
		}

		public InputActionReference rotateModeOverrideAction
		{
			get
			{
				return this.m_RotateModeOverrideAction;
			}
			set
			{
				this.UnsubscribeRotateModeOverrideAction();
				this.m_RotateModeOverrideAction = value;
				this.SubscribeRotateModeOverrideAction();
			}
		}

		public InputActionReference toggleMouseTransformationModeAction
		{
			get
			{
				return this.m_ToggleMouseTransformationModeAction;
			}
			set
			{
				this.UnsubscribeToggleMouseTransformationModeAction();
				this.m_ToggleMouseTransformationModeAction = value;
				this.SubscribeToggleMouseTransformationModeAction();
			}
		}

		public InputActionReference negateModeAction
		{
			get
			{
				return this.m_NegateModeAction;
			}
			set
			{
				this.UnsubscribeNegateModeAction();
				this.m_NegateModeAction = value;
				this.SubscribeNegateModeAction();
			}
		}

		public InputActionReference xConstraintAction
		{
			get
			{
				return this.m_XConstraintAction;
			}
			set
			{
				this.UnsubscribeXConstraintAction();
				this.m_XConstraintAction = value;
				this.SubscribeXConstraintAction();
			}
		}

		public InputActionReference yConstraintAction
		{
			get
			{
				return this.m_YConstraintAction;
			}
			set
			{
				this.UnsubscribeYConstraintAction();
				this.m_YConstraintAction = value;
				this.SubscribeYConstraintAction();
			}
		}

		public InputActionReference zConstraintAction
		{
			get
			{
				return this.m_ZConstraintAction;
			}
			set
			{
				this.UnsubscribeZConstraintAction();
				this.m_ZConstraintAction = value;
				this.SubscribeZConstraintAction();
			}
		}

		public InputActionReference resetAction
		{
			get
			{
				return this.m_ResetAction;
			}
			set
			{
				this.UnsubscribeResetAction();
				this.m_ResetAction = value;
				this.SubscribeResetAction();
			}
		}

		public InputActionReference toggleCursorLockAction
		{
			get
			{
				return this.m_ToggleCursorLockAction;
			}
			set
			{
				this.UnsubscribeToggleCursorLockAction();
				this.m_ToggleCursorLockAction = value;
				this.SubscribeToggleCursorLockAction();
			}
		}

		public InputActionReference toggleDevicePositionTargetAction
		{
			get
			{
				return this.m_ToggleDevicePositionTargetAction;
			}
			set
			{
				this.UnsubscribeToggleDevicePositionTargetAction();
				this.m_ToggleDevicePositionTargetAction = value;
				this.SubscribeToggleDevicePositionTargetAction();
			}
		}

		public InputActionReference togglePrimary2DAxisTargetAction
		{
			get
			{
				return this.m_TogglePrimary2DAxisTargetAction;
			}
			set
			{
				this.UnsubscribeTogglePrimary2DAxisTargetAction();
				this.m_TogglePrimary2DAxisTargetAction = value;
				this.SubscribeTogglePrimary2DAxisTargetAction();
			}
		}

		public InputActionReference toggleSecondary2DAxisTargetAction
		{
			get
			{
				return this.m_ToggleSecondary2DAxisTargetAction;
			}
			set
			{
				this.UnsubscribeToggleSecondary2DAxisTargetAction();
				this.m_ToggleSecondary2DAxisTargetAction = value;
				this.SubscribeToggleSecondary2DAxisTargetAction();
			}
		}

		public InputActionReference axis2DAction
		{
			get
			{
				return this.m_Axis2DAction;
			}
			set
			{
				this.UnsubscribeAxis2DAction();
				this.m_Axis2DAction = value;
				this.SubscribeAxis2DAction();
			}
		}

		public InputActionReference restingHandAxis2DAction
		{
			get
			{
				return this.m_RestingHandAxis2DAction;
			}
			set
			{
				this.UnsubscribeRestingHandAxis2DAction();
				this.m_RestingHandAxis2DAction = value;
				this.SubscribeRestingHandAxis2DAction();
			}
		}

		public InputActionReference gripAction
		{
			get
			{
				return this.m_GripAction;
			}
			set
			{
				this.UnsubscribeGripAction();
				this.m_GripAction = value;
				this.SubscribeGripAction();
			}
		}

		public InputActionReference triggerAction
		{
			get
			{
				return this.m_TriggerAction;
			}
			set
			{
				this.UnsubscribeTriggerAction();
				this.m_TriggerAction = value;
				this.SubscribeTriggerAction();
			}
		}

		public InputActionReference primaryButtonAction
		{
			get
			{
				return this.m_PrimaryButtonAction;
			}
			set
			{
				this.UnsubscribePrimaryButtonAction();
				this.m_PrimaryButtonAction = value;
				this.SubscribePrimaryButtonAction();
			}
		}

		public InputActionReference secondaryButtonAction
		{
			get
			{
				return this.m_SecondaryButtonAction;
			}
			set
			{
				this.UnsubscribeSecondaryButtonAction();
				this.m_SecondaryButtonAction = value;
				this.SubscribeSecondaryButtonAction();
			}
		}

		public InputActionReference menuAction
		{
			get
			{
				return this.m_MenuAction;
			}
			set
			{
				this.UnsubscribeMenuAction();
				this.m_MenuAction = value;
				this.SubscribeMenuAction();
			}
		}

		public InputActionReference primary2DAxisClickAction
		{
			get
			{
				return this.m_Primary2DAxisClickAction;
			}
			set
			{
				this.UnsubscribePrimary2DAxisClickAction();
				this.m_Primary2DAxisClickAction = value;
				this.SubscribePrimary2DAxisClickAction();
			}
		}

		public InputActionReference secondary2DAxisClickAction
		{
			get
			{
				return this.m_Secondary2DAxisClickAction;
			}
			set
			{
				this.UnsubscribeSecondary2DAxisClickAction();
				this.m_Secondary2DAxisClickAction = value;
				this.SubscribeSecondary2DAxisClickAction();
			}
		}

		public InputActionReference primary2DAxisTouchAction
		{
			get
			{
				return this.m_Primary2DAxisTouchAction;
			}
			set
			{
				this.UnsubscribePrimary2DAxisTouchAction();
				this.m_Primary2DAxisTouchAction = value;
				this.SubscribePrimary2DAxisTouchAction();
			}
		}

		public InputActionReference secondary2DAxisTouchAction
		{
			get
			{
				return this.m_Secondary2DAxisTouchAction;
			}
			set
			{
				this.UnsubscribeSecondary2DAxisTouchAction();
				this.m_Secondary2DAxisTouchAction = value;
				this.SubscribeSecondary2DAxisTouchAction();
			}
		}

		public InputActionReference primaryTouchAction
		{
			get
			{
				return this.m_PrimaryTouchAction;
			}
			set
			{
				this.UnsubscribePrimaryTouchAction();
				this.m_PrimaryTouchAction = value;
				this.SubscribePrimaryTouchAction();
			}
		}

		public InputActionReference secondaryTouchAction
		{
			get
			{
				return this.m_SecondaryTouchAction;
			}
			set
			{
				this.UnsubscribeSecondaryTouchAction();
				this.m_SecondaryTouchAction = value;
				this.SubscribeSecondaryTouchAction();
			}
		}

		public InputActionAsset handActionAsset
		{
			get
			{
				return this.m_HandActionAsset;
			}
			set
			{
				this.m_HandActionAsset = value;
			}
		}

		public Transform cameraTransform
		{
			get
			{
				return this.m_CameraTransform;
			}
			set
			{
				this.m_CameraTransform = value;
			}
		}

		public XRDeviceSimulator.Space keyboardTranslateSpace
		{
			get
			{
				return this.m_KeyboardTranslateSpace;
			}
			set
			{
				this.m_KeyboardTranslateSpace = value;
			}
		}

		public XRDeviceSimulator.Space mouseTranslateSpace
		{
			get
			{
				return this.m_MouseTranslateSpace;
			}
			set
			{
				this.m_MouseTranslateSpace = value;
			}
		}

		public float keyboardXTranslateSpeed
		{
			get
			{
				return this.m_KeyboardXTranslateSpeed;
			}
			set
			{
				this.m_KeyboardXTranslateSpeed = value;
			}
		}

		public float keyboardYTranslateSpeed
		{
			get
			{
				return this.m_KeyboardYTranslateSpeed;
			}
			set
			{
				this.m_KeyboardYTranslateSpeed = value;
			}
		}

		public float keyboardZTranslateSpeed
		{
			get
			{
				return this.m_KeyboardZTranslateSpeed;
			}
			set
			{
				this.m_KeyboardZTranslateSpeed = value;
			}
		}

		public float keyboardBodyTranslateMultiplier
		{
			get
			{
				return this.m_KeyboardBodyTranslateMultiplier;
			}
			set
			{
				this.m_KeyboardBodyTranslateMultiplier = value;
			}
		}

		public float mouseXTranslateSensitivity
		{
			get
			{
				return this.m_MouseXTranslateSensitivity;
			}
			set
			{
				this.m_MouseXTranslateSensitivity = value;
			}
		}

		public float mouseYTranslateSensitivity
		{
			get
			{
				return this.m_MouseYTranslateSensitivity;
			}
			set
			{
				this.m_MouseYTranslateSensitivity = value;
			}
		}

		public float mouseScrollTranslateSensitivity
		{
			get
			{
				return this.m_MouseScrollTranslateSensitivity;
			}
			set
			{
				this.m_MouseScrollTranslateSensitivity = value;
			}
		}

		public float mouseXRotateSensitivity
		{
			get
			{
				return this.m_MouseXRotateSensitivity;
			}
			set
			{
				this.m_MouseXRotateSensitivity = value;
			}
		}

		public float mouseYRotateSensitivity
		{
			get
			{
				return this.m_MouseYRotateSensitivity;
			}
			set
			{
				this.m_MouseYRotateSensitivity = value;
			}
		}

		public float mouseScrollRotateSensitivity
		{
			get
			{
				return this.m_MouseScrollRotateSensitivity;
			}
			set
			{
				this.m_MouseScrollRotateSensitivity = value;
			}
		}

		public bool mouseYRotateInvert
		{
			get
			{
				return this.m_MouseYRotateInvert;
			}
			set
			{
				this.m_MouseYRotateInvert = value;
			}
		}

		public CursorLockMode desiredCursorLockMode
		{
			get
			{
				return this.m_DesiredCursorLockMode;
			}
			set
			{
				this.m_DesiredCursorLockMode = value;
			}
		}

		public GameObject deviceSimulatorUI
		{
			get
			{
				return this.m_DeviceSimulatorUI;
			}
			set
			{
				this.m_DeviceSimulatorUI = value;
			}
		}

		public float gripAmount
		{
			get
			{
				return this.m_GripAmount;
			}
			set
			{
				this.m_GripAmount = value;
			}
		}

		public float triggerAmount
		{
			get
			{
				return this.m_TriggerAmount;
			}
			set
			{
				this.m_TriggerAmount = value;
			}
		}

		public bool hmdIsTracked
		{
			get
			{
				return this.m_HMDIsTracked;
			}
			set
			{
				this.m_HMDIsTracked = value;
			}
		}

		public InputTrackingState hmdTrackingState
		{
			get
			{
				return this.m_HMDTrackingState;
			}
			set
			{
				this.m_HMDTrackingState = value;
			}
		}

		public bool leftControllerIsTracked
		{
			get
			{
				return this.m_LeftControllerIsTracked;
			}
			set
			{
				this.m_LeftControllerIsTracked = value;
			}
		}

		public InputTrackingState leftControllerTrackingState
		{
			get
			{
				return this.m_LeftControllerTrackingState;
			}
			set
			{
				this.m_LeftControllerTrackingState = value;
			}
		}

		public bool rightControllerIsTracked
		{
			get
			{
				return this.m_RightControllerIsTracked;
			}
			set
			{
				this.m_RightControllerIsTracked = value;
			}
		}

		public InputTrackingState rightControllerTrackingState
		{
			get
			{
				return this.m_RightControllerTrackingState;
			}
			set
			{
				this.m_RightControllerTrackingState = value;
			}
		}

		public bool leftHandIsTracked
		{
			get
			{
				return this.m_LeftHandIsTracked;
			}
			set
			{
				this.m_LeftHandIsTracked = value;
			}
		}

		public bool rightHandIsTracked
		{
			get
			{
				return this.m_RightHandIsTracked;
			}
			set
			{
				this.m_RightHandIsTracked = value;
			}
		}

		public XRDeviceSimulator.TransformationMode mouseTransformationMode { get; set; } = XRDeviceSimulator.TransformationMode.Rotate;

		public bool negateMode { get; private set; }

		public XRDeviceSimulator.Axis2DTargets axis2DTargets { get; set; } = XRDeviceSimulator.Axis2DTargets.Primary2DAxis;

		public bool manipulatingLeftDevice
		{
			get
			{
				return this.m_TargetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.LeftDevice);
			}
		}

		public bool manipulatingRightDevice
		{
			get
			{
				return this.m_TargetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.RightDevice);
			}
		}

		public bool manipulatingLeftController
		{
			get
			{
				return this.m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller && this.manipulatingLeftDevice;
			}
		}

		public bool manipulatingRightController
		{
			get
			{
				return this.m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller && this.manipulatingRightDevice;
			}
		}

		public bool manipulatingLeftHand
		{
			get
			{
				return this.m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand && this.manipulatingLeftDevice;
			}
		}

		public bool manipulatingRightHand
		{
			get
			{
				return this.m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand && this.manipulatingRightDevice;
			}
		}

		public bool manipulatingFPS
		{
			get
			{
				return this.m_TargetedDeviceInput == XRDeviceSimulator.TargetedDevices.FPS;
			}
		}

		public static XRDeviceSimulator instance { get; private set; }

		private XRDeviceSimulator.TargetedDevices targetedDeviceInput
		{
			get
			{
				return this.m_TargetedDeviceInput;
			}
			set
			{
				this.m_TargetedDeviceInput = value;
			}
		}

		protected virtual void Awake()
		{
			if (XRDeviceSimulator.instance == null)
			{
				XRDeviceSimulator.instance = this;
				Action<bool> action = XRDeviceSimulator.instanceChanged;
				if (action != null)
				{
					action(true);
				}
			}
			else if (XRDeviceSimulator.instance != this)
			{
				Debug.LogWarning(string.Format("Another instance of XR Device Simulator already exists ({0}), destroying {1}.", XRDeviceSimulator.instance, base.gameObject), this);
				Object.Destroy(base.gameObject);
				return;
			}
			this.m_DeviceLifecycleManager = XRSimulatorUtility.FindCreateSimulatedDeviceLifecycleManager(base.gameObject);
			this.m_HandExpressionManager = XRSimulatorUtility.FindCreateSimulatedHandExpressionManager(base.gameObject);
			if (this.m_DeviceSimulatorActionAsset == null)
			{
				if (this.m_ManipulateLeftAction != null)
				{
					this.m_DeviceSimulatorActionAsset = this.m_ManipulateLeftAction.asset;
				}
				if (this.m_DeviceSimulatorActionAsset == null && this.m_ManipulateRightAction != null)
				{
					this.m_DeviceSimulatorActionAsset = this.m_ManipulateRightAction.asset;
				}
				if (this.m_DeviceSimulatorActionAsset == null)
				{
					Debug.LogError("No Device Simulator Action Asset has been defined, please assign one for the XR Device Simulator to work.", this);
				}
				else
				{
					Debug.LogWarning("No Device Simulator Action Asset has been defined for the XR Device Simulator, using a default one: " + this.m_DeviceSimulatorActionAsset.name, this.m_DeviceSimulatorActionAsset);
				}
			}
			if (this.m_ControllerActionAsset == null)
			{
				if (this.gripAction != null)
				{
					this.m_ControllerActionAsset = this.gripAction.asset;
				}
				if (this.m_ControllerActionAsset == null)
				{
					Debug.LogError("No Controller Action Asset has been defined, please assign one for the XR Device Simulator to work.", this);
				}
				else
				{
					Debug.LogWarning("No Controller Action Asset has been defined for the XR Device Simulator, using a default one: " + this.m_ControllerActionAsset.name, this.m_ControllerActionAsset);
				}
			}
			if (this.m_HandActionAsset == null)
			{
				if (this.m_SimulatedHandExpressions.Count > 0)
				{
					if (this.m_SimulatedHandExpressions[0].toggleAction != null)
					{
						this.m_HandActionAsset = this.m_SimulatedHandExpressions[0].toggleAction.asset;
					}
				}
				else if (this.m_HandExpressionManager.simulatedHandExpressions.Count > 0 && this.m_HandExpressionManager.simulatedHandExpressions[0].toggleInput.inputActionReferencePerformed != null)
				{
					this.m_HandActionAsset = this.m_HandExpressionManager.simulatedHandExpressions[0].toggleInput.inputActionReferencePerformed.asset;
				}
				if (this.m_HandActionAsset == null)
				{
					Debug.LogError("No Hand Action Asset has been defined, please assign one for the XR Device Simulator to work.", this);
				}
				else
				{
					Debug.LogWarning("No Hand Action Asset has been defined for the XR Device Simulator, using a default one: " + this.m_HandActionAsset.name, this.m_HandActionAsset);
				}
			}
			this.m_HMDState.Reset();
			this.m_LeftControllerState.Reset();
			this.m_RightControllerState.Reset();
			this.m_LeftHandState.Reset();
			this.m_RightHandState.Reset();
			this.m_LeftControllerState.devicePosition = XRSimulatorUtility.leftDeviceDefaultInitialPosition;
			this.m_RightControllerState.devicePosition = XRSimulatorUtility.rightDeviceDefaultInitialPosition;
			this.m_LeftHandState.position = XRSimulatorUtility.leftDeviceDefaultInitialPosition;
			this.m_RightHandState.position = XRSimulatorUtility.rightDeviceDefaultInitialPosition;
			if (this.m_DeviceSimulatorUI != null)
			{
				Object.Instantiate<GameObject>(this.m_DeviceSimulatorUI, base.transform);
			}
		}

		protected virtual void OnEnable()
		{
			XRSimulatorUtility.FindCameraTransform(ref this.m_CachedCamera, ref this.m_CameraTransform);
			this.SubscribeKeyboardXTranslateAction();
			this.SubscribeKeyboardYTranslateAction();
			this.SubscribeKeyboardZTranslateAction();
			this.SubscribeManipulateLeftAction();
			this.SubscribeToggleManipulateLeftAction();
			this.SubscribeManipulateRightAction();
			this.SubscribeToggleManipulateRightAction();
			this.SubscribeToggleManipulateBodyAction();
			this.SubscribeManipulateHeadAction();
			this.SubscribeStopManipulationAction();
			this.SubscribeHandControllerModeAction();
			this.SubscribeCycleDevicesAction();
			this.SubscribeMouseDeltaAction();
			this.SubscribeMouseScrollAction();
			this.SubscribeRotateModeOverrideAction();
			this.SubscribeToggleMouseTransformationModeAction();
			this.SubscribeNegateModeAction();
			this.SubscribeXConstraintAction();
			this.SubscribeYConstraintAction();
			this.SubscribeZConstraintAction();
			this.SubscribeResetAction();
			this.SubscribeToggleCursorLockAction();
			this.SubscribeToggleDevicePositionTargetAction();
			this.SubscribeTogglePrimary2DAxisTargetAction();
			this.SubscribeToggleSecondary2DAxisTargetAction();
			this.SubscribeAxis2DAction();
			this.SubscribeRestingHandAxis2DAction();
			this.SubscribeGripAction();
			this.SubscribeTriggerAction();
			this.SubscribePrimaryButtonAction();
			this.SubscribeSecondaryButtonAction();
			this.SubscribeMenuAction();
			this.SubscribePrimary2DAxisClickAction();
			this.SubscribeSecondary2DAxisClickAction();
			this.SubscribePrimary2DAxisTouchAction();
			this.SubscribeSecondary2DAxisTouchAction();
			this.SubscribePrimaryTouchAction();
			this.SubscribeSecondaryTouchAction();
			if (this.m_ControllerActionAsset != null)
			{
				this.m_ControllerActionAsset.Enable();
			}
			if (this.m_DeviceSimulatorActionAsset != null)
			{
				this.m_DeviceSimulatorActionAsset.Enable();
			}
		}

		protected virtual void OnDisable()
		{
			this.UnsubscribeKeyboardXTranslateAction();
			this.UnsubscribeKeyboardYTranslateAction();
			this.UnsubscribeKeyboardZTranslateAction();
			this.UnsubscribeManipulateLeftAction();
			this.UnsubscribeToggleManipulateLeftAction();
			this.UnsubscribeManipulateRightAction();
			this.UnsubscribeToggleManipulateRightAction();
			this.UnsubscribeToggleManipulateBodyAction();
			this.UnsubscribeManipulateHeadAction();
			this.UnsubscribeStopManipulationAction();
			this.UnsubscribeHandControllerModeAction();
			this.UnsubscribeCycleDevicesAction();
			this.UnsubscribeMouseDeltaAction();
			this.UnsubscribeMouseScrollAction();
			this.UnsubscribeRotateModeOverrideAction();
			this.UnsubscribeToggleMouseTransformationModeAction();
			this.UnsubscribeNegateModeAction();
			this.UnsubscribeXConstraintAction();
			this.UnsubscribeYConstraintAction();
			this.UnsubscribeZConstraintAction();
			this.UnsubscribeResetAction();
			this.UnsubscribeToggleCursorLockAction();
			this.UnsubscribeToggleDevicePositionTargetAction();
			this.UnsubscribeTogglePrimary2DAxisTargetAction();
			this.UnsubscribeToggleSecondary2DAxisTargetAction();
			this.UnsubscribeAxis2DAction();
			this.UnsubscribeRestingHandAxis2DAction();
			this.UnsubscribeGripAction();
			this.UnsubscribeTriggerAction();
			this.UnsubscribePrimaryButtonAction();
			this.UnsubscribeSecondaryButtonAction();
			this.UnsubscribeMenuAction();
			this.UnsubscribePrimary2DAxisClickAction();
			this.UnsubscribeSecondary2DAxisClickAction();
			this.UnsubscribePrimary2DAxisTouchAction();
			this.UnsubscribeSecondary2DAxisTouchAction();
			this.UnsubscribePrimaryTouchAction();
			this.UnsubscribeSecondaryTouchAction();
			if (this.m_ControllerActionAsset != null)
			{
				this.m_ControllerActionAsset.Disable();
			}
			if (this.m_DeviceSimulatorActionAsset != null)
			{
				this.m_DeviceSimulatorActionAsset.Disable();
			}
		}

		protected virtual void OnDestroy()
		{
			if (XRDeviceSimulator.instance == this)
			{
				Action<bool> action = XRDeviceSimulator.instanceChanged;
				if (action == null)
				{
					return;
				}
				action(false);
			}
		}

		protected virtual void Start()
		{
			this.InitializeHandExpressions();
		}

		protected virtual void Update()
		{
			this.ProcessPoseInput();
			this.ProcessControlInput();
			this.ProcessHandExpressionInput();
			this.m_DeviceLifecycleManager.ApplyHandState(this.m_LeftHandState, this.m_RightHandState);
			this.m_DeviceLifecycleManager.ApplyHMDState(this.m_HMDState);
			this.m_DeviceLifecycleManager.ApplyControllerState(this.m_LeftControllerState, this.m_RightControllerState);
		}

		protected virtual void ProcessPoseInput()
		{
			this.m_LeftControllerState.isTracked = this.m_LeftControllerIsTracked;
			this.m_RightControllerState.isTracked = this.m_RightControllerIsTracked;
			this.m_LeftHandState.isTracked = this.m_LeftHandIsTracked;
			this.m_RightHandState.isTracked = this.m_RightHandIsTracked;
			this.m_HMDState.isTracked = this.m_HMDIsTracked;
			this.m_LeftControllerState.trackingState = (int)this.m_LeftControllerTrackingState;
			this.m_RightControllerState.trackingState = (int)this.m_RightControllerTrackingState;
			this.m_HMDState.trackingState = (int)this.m_HMDTrackingState;
			if (this.m_TargetedDeviceInput == XRDeviceSimulator.TargetedDevices.None)
			{
				return;
			}
			if (!XRSimulatorUtility.FindCameraTransform(ref this.m_CachedCamera, ref this.m_CameraTransform))
			{
				return;
			}
			Transform parent = this.m_CameraTransform.parent;
			Quaternion quaternion = (parent != null) ? parent.rotation : Quaternion.identity;
			Quaternion inverseCameraParentRotation = Quaternion.Inverse(quaternion);
			if (this.m_TargetedDeviceInput == XRDeviceSimulator.TargetedDevices.FPS && Time.time > 1f)
			{
				float xTranslateInput = this.m_KeyboardXTranslateInput * this.m_KeyboardXTranslateSpeed * this.m_KeyboardBodyTranslateMultiplier * Time.deltaTime;
				float yTranslateInput = this.m_KeyboardYTranslateInput * this.m_KeyboardYTranslateSpeed * this.m_KeyboardBodyTranslateMultiplier * Time.deltaTime;
				float zTranslateInput = this.m_KeyboardZTranslateInput * this.m_KeyboardZTranslateSpeed * this.m_KeyboardBodyTranslateMultiplier * Time.deltaTime;
				Vector3 translationInDeviceSpace = XRSimulatorUtility.GetTranslationInDeviceSpace(xTranslateInput, yTranslateInput, zTranslateInput, this.m_CameraTransform, quaternion, inverseCameraParentRotation);
				this.m_LeftControllerState.devicePosition = this.m_LeftControllerState.devicePosition + translationInDeviceSpace;
				this.m_RightControllerState.devicePosition = this.m_RightControllerState.devicePosition + translationInDeviceSpace;
				this.m_LeftHandState.position = this.m_LeftHandState.position + translationInDeviceSpace;
				this.m_RightHandState.position = this.m_RightHandState.position + translationInDeviceSpace;
				this.m_HMDState.centerEyePosition = this.m_HMDState.centerEyePosition + translationInDeviceSpace;
				this.m_HMDState.devicePosition = this.m_HMDState.centerEyePosition;
				Vector3 vector = new Vector3(this.m_MouseDeltaInput.x * this.m_MouseXRotateSensitivity, this.m_MouseDeltaInput.y * this.m_MouseYRotateSensitivity * (this.m_MouseYRotateInvert ? 1f : -1f), this.m_MouseScrollInput.y * this.m_MouseScrollRotateSensitivity);
				Vector3 vector2;
				if (this.m_XConstraintInput && !this.m_YConstraintInput && !this.m_ZConstraintInput)
				{
					vector2 = new Vector3(-vector.x + vector.y, 0f, 0f);
				}
				else if (!this.m_XConstraintInput && this.m_YConstraintInput && !this.m_ZConstraintInput)
				{
					vector2 = new Vector3(0f, vector.x + -vector.y, 0f);
				}
				else
				{
					vector2 = new Vector3(vector.y, vector.x, 0f);
				}
				this.m_CenterEyeEuler += vector2;
				this.m_CenterEyeEuler.x = Mathf.Clamp(this.m_CenterEyeEuler.x, -XRSimulatorUtility.cameraMaxXAngle, XRSimulatorUtility.cameraMaxXAngle);
				this.m_HMDState.centerEyeRotation = Quaternion.Euler(this.m_CenterEyeEuler);
				this.m_HMDState.deviceRotation = this.m_HMDState.centerEyeRotation;
				Quaternion quaternion2 = Quaternion.AngleAxis(vector2.y, Quaternion.Euler(0f, this.m_CenterEyeEuler.y, 0f) * Vector3.up);
				Vector3 centerEyePosition = this.m_HMDState.centerEyePosition;
				this.m_LeftControllerState.devicePosition = quaternion2 * (this.m_LeftControllerState.devicePosition - centerEyePosition) + centerEyePosition;
				this.m_LeftControllerState.deviceRotation = quaternion2 * this.m_LeftControllerState.deviceRotation;
				this.m_RightControllerState.devicePosition = quaternion2 * (this.m_RightControllerState.devicePosition - centerEyePosition) + centerEyePosition;
				this.m_RightControllerState.deviceRotation = quaternion2 * this.m_RightControllerState.deviceRotation;
				this.m_LeftControllerEuler = this.m_LeftControllerState.deviceRotation.eulerAngles;
				this.m_RightControllerEuler = this.m_RightControllerState.deviceRotation.eulerAngles;
				this.m_LeftHandState.position = quaternion2 * (this.m_LeftHandState.position - centerEyePosition) + centerEyePosition;
				this.m_LeftHandState.rotation = quaternion2 * this.m_LeftHandState.rotation;
				this.m_RightHandState.position = quaternion2 * (this.m_RightHandState.position - centerEyePosition) + centerEyePosition;
				this.m_RightHandState.rotation = quaternion2 * this.m_RightHandState.rotation;
				this.m_LeftHandState.euler = this.m_LeftHandState.rotation.eulerAngles;
				this.m_RightHandState.euler = this.m_RightHandState.rotation.eulerAngles;
				if (this.m_ResetInput)
				{
					this.m_LeftControllerState.devicePosition = XRSimulatorUtility.leftDeviceDefaultInitialPosition;
					this.m_RightControllerState.devicePosition = XRSimulatorUtility.rightDeviceDefaultInitialPosition;
					this.m_LeftControllerEuler = Vector3.zero;
					this.m_LeftControllerState.deviceRotation = Quaternion.Euler(this.m_LeftControllerEuler);
					this.m_RightControllerEuler = Vector3.zero;
					this.m_RightControllerState.deviceRotation = Quaternion.Euler(this.m_RightControllerEuler);
					this.m_LeftHandState.position = XRSimulatorUtility.leftDeviceDefaultInitialPosition;
					this.m_RightHandState.position = XRSimulatorUtility.rightDeviceDefaultInitialPosition;
					this.m_LeftHandState.euler = Vector3.zero;
					this.m_LeftHandState.rotation = Quaternion.Euler(this.m_LeftHandState.euler);
					this.m_RightHandState.euler = Vector3.zero;
					this.m_RightHandState.rotation = Quaternion.Euler(this.m_RightHandState.euler);
					this.m_HMDState.centerEyePosition = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
					this.m_HMDState.devicePosition = this.m_HMDState.centerEyePosition;
					this.m_CenterEyeEuler = Vector3.zero;
					this.m_HMDState.centerEyeRotation = Quaternion.Euler(this.m_CenterEyeEuler);
					this.m_HMDState.deviceRotation = this.m_HMDState.centerEyeRotation;
				}
			}
			if ((this.axis2DTargets & XRDeviceSimulator.Axis2DTargets.Position) != XRDeviceSimulator.Axis2DTargets.None)
			{
				Vector3 a;
				Vector3 a2;
				Vector3 a3;
				XRSimulatorUtility.GetAxes((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_KeyboardTranslateSpace, this.m_CameraTransform, out a, out a2, out a3);
				Vector3 point = a * (this.m_KeyboardXTranslateInput * this.m_KeyboardXTranslateSpeed * Time.deltaTime) + a2 * (this.m_KeyboardYTranslateInput * this.m_KeyboardYTranslateSpeed * Time.deltaTime) + a3 * (this.m_KeyboardZTranslateInput * this.m_KeyboardZTranslateSpeed * Time.deltaTime);
				if (this.manipulatingLeftController)
				{
					Quaternion deltaRotation = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_KeyboardTranslateSpace, this.m_LeftControllerState, inverseCameraParentRotation);
					this.m_LeftControllerState.devicePosition = this.m_LeftControllerState.devicePosition + deltaRotation * point;
				}
				if (this.manipulatingRightController)
				{
					Quaternion deltaRotation2 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_KeyboardTranslateSpace, this.m_RightControllerState, inverseCameraParentRotation);
					this.m_RightControllerState.devicePosition = this.m_RightControllerState.devicePosition + deltaRotation2 * point;
				}
				if (this.manipulatingLeftHand)
				{
					Quaternion deltaRotation3 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_KeyboardTranslateSpace, this.m_LeftHandState, inverseCameraParentRotation);
					this.m_LeftHandState.position = this.m_LeftHandState.position + deltaRotation3 * point;
				}
				if (this.manipulatingRightHand)
				{
					Quaternion deltaRotation4 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_KeyboardTranslateSpace, this.m_RightHandState, inverseCameraParentRotation);
					this.m_RightHandState.position = this.m_RightHandState.position + deltaRotation4 * point;
				}
				if (this.m_TargetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.HMD))
				{
					Quaternion deltaRotation5 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_KeyboardTranslateSpace, this.m_HMDState, inverseCameraParentRotation);
					this.m_HMDState.centerEyePosition = this.m_HMDState.centerEyePosition + deltaRotation5 * point;
					this.m_HMDState.devicePosition = this.m_HMDState.centerEyePosition;
				}
			}
			if ((this.mouseTransformationMode == XRDeviceSimulator.TransformationMode.Translate && !this.m_RotateModeOverrideInput && !this.negateMode) || ((this.mouseTransformationMode == XRDeviceSimulator.TransformationMode.Rotate || this.m_RotateModeOverrideInput) && this.negateMode))
			{
				Vector3 a4;
				Vector3 a5;
				Vector3 a6;
				XRSimulatorUtility.GetAxes((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_MouseTranslateSpace, this.m_CameraTransform, out a4, out a5, out a6);
				Vector3 vector3 = new Vector3(this.m_MouseDeltaInput.x * this.m_MouseXTranslateSensitivity, this.m_MouseDeltaInput.y * this.m_MouseYTranslateSensitivity, this.m_MouseScrollInput.y * this.m_MouseScrollTranslateSensitivity);
				Vector3 vector4;
				if (this.m_XConstraintInput && !this.m_YConstraintInput && this.m_ZConstraintInput)
				{
					vector4 = a4 * vector3.x + a6 * vector3.y;
				}
				else if (!this.m_XConstraintInput && this.m_YConstraintInput && this.m_ZConstraintInput)
				{
					vector4 = a5 * vector3.y + a6 * vector3.x;
				}
				else if (this.m_XConstraintInput && !this.m_YConstraintInput && !this.m_ZConstraintInput)
				{
					vector4 = a4 * (vector3.x + vector3.y);
				}
				else if (!this.m_XConstraintInput && this.m_YConstraintInput && !this.m_ZConstraintInput)
				{
					vector4 = a5 * (vector3.x + vector3.y);
				}
				else if (!this.m_XConstraintInput && !this.m_YConstraintInput && this.m_ZConstraintInput)
				{
					vector4 = a6 * (vector3.x + vector3.y);
				}
				else
				{
					vector4 = a4 * vector3.x + a5 * vector3.y;
				}
				vector4 += a6 * vector3.z;
				if (this.manipulatingLeftController)
				{
					Quaternion deltaRotation6 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_MouseTranslateSpace, this.m_LeftControllerState, inverseCameraParentRotation);
					this.m_LeftControllerState.devicePosition = this.m_LeftControllerState.devicePosition + deltaRotation6 * vector4;
				}
				if (this.manipulatingRightController)
				{
					Quaternion deltaRotation7 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_MouseTranslateSpace, this.m_RightControllerState, inverseCameraParentRotation);
					this.m_RightControllerState.devicePosition = this.m_RightControllerState.devicePosition + deltaRotation7 * vector4;
				}
				if (this.manipulatingLeftHand)
				{
					Quaternion deltaRotation8 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_MouseTranslateSpace, this.m_LeftHandState, inverseCameraParentRotation);
					this.m_LeftHandState.position = this.m_LeftHandState.position + deltaRotation8 * vector4;
				}
				if (this.manipulatingRightHand)
				{
					Quaternion deltaRotation9 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.mouseTranslateSpace, this.m_RightHandState, inverseCameraParentRotation);
					this.m_RightHandState.position = this.m_RightHandState.position + deltaRotation9 * vector4;
				}
				if (this.m_TargetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.HMD))
				{
					Quaternion deltaRotation10 = XRSimulatorUtility.GetDeltaRotation((UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Space)this.m_MouseTranslateSpace, this.m_HMDState, inverseCameraParentRotation);
					this.m_HMDState.centerEyePosition = this.m_HMDState.centerEyePosition + deltaRotation10 * vector4;
					this.m_HMDState.devicePosition = this.m_HMDState.centerEyePosition;
				}
				if (this.m_ResetInput)
				{
					Vector3 resetScale = this.GetResetScale();
					if (this.manipulatingLeftController)
					{
						Vector3 devicePosition = Vector3.Scale(this.m_LeftControllerState.devicePosition, resetScale);
						if (devicePosition.magnitude <= 0f)
						{
							devicePosition = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
						}
						this.m_LeftControllerState.devicePosition = devicePosition;
					}
					if (this.manipulatingRightController)
					{
						Vector3 devicePosition2 = Vector3.Scale(this.m_RightControllerState.devicePosition, resetScale);
						if (devicePosition2.magnitude <= 0f)
						{
							devicePosition2 = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
						}
						this.m_RightControllerState.devicePosition = devicePosition2;
					}
					if (this.manipulatingLeftHand)
					{
						Vector3 position = Vector3.Scale(this.m_LeftHandState.position, resetScale);
						if (position.magnitude <= 0f)
						{
							position = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
						}
						this.m_LeftHandState.position = position;
					}
					if (this.manipulatingRightHand)
					{
						Vector3 position2 = Vector3.Scale(this.m_RightHandState.position, resetScale);
						if (position2.magnitude <= 0f)
						{
							position2 = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
						}
						this.m_RightHandState.position = position2;
					}
					if (this.m_TargetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.HMD))
					{
						Vector3 centerEyePosition2 = Vector3.Scale(this.m_HMDState.centerEyePosition, resetScale);
						if (centerEyePosition2.magnitude <= 0f)
						{
							centerEyePosition2 = new Vector3(Mathf.Epsilon, Mathf.Epsilon, Mathf.Epsilon);
						}
						this.m_HMDState.centerEyePosition = centerEyePosition2;
						this.m_HMDState.devicePosition = this.m_HMDState.centerEyePosition;
						return;
					}
				}
			}
			else
			{
				Vector3 vector5 = new Vector3(this.m_MouseDeltaInput.x * this.m_MouseXRotateSensitivity, this.m_MouseDeltaInput.y * this.m_MouseYRotateSensitivity * (this.m_MouseYRotateInvert ? 1f : -1f), this.m_MouseScrollInput.y * this.m_MouseScrollRotateSensitivity);
				Vector3 vector6;
				if (this.m_XConstraintInput && !this.m_YConstraintInput && this.m_ZConstraintInput)
				{
					vector6 = new Vector3(vector5.y, 0f, -vector5.x);
				}
				else if (!this.m_XConstraintInput && this.m_YConstraintInput && this.m_ZConstraintInput)
				{
					vector6 = new Vector3(0f, vector5.x, -vector5.y);
				}
				else if (this.m_XConstraintInput && !this.m_YConstraintInput && !this.m_ZConstraintInput)
				{
					vector6 = new Vector3(-vector5.x + vector5.y, 0f, 0f);
				}
				else if (!this.m_XConstraintInput && this.m_YConstraintInput && !this.m_ZConstraintInput)
				{
					vector6 = new Vector3(0f, vector5.x + -vector5.y, 0f);
				}
				else if (!this.m_XConstraintInput && !this.m_YConstraintInput && this.m_ZConstraintInput)
				{
					vector6 = new Vector3(0f, 0f, -vector5.x + -vector5.y);
				}
				else
				{
					vector6 = new Vector3(vector5.y, vector5.x, 0f);
				}
				vector6 += new Vector3(0f, 0f, vector5.z);
				if (this.manipulatingLeftController)
				{
					this.m_LeftControllerEuler += vector6;
					this.m_LeftControllerState.deviceRotation = Quaternion.Euler(this.m_LeftControllerEuler);
				}
				if (this.manipulatingRightController)
				{
					this.m_RightControllerEuler += vector6;
					this.m_RightControllerState.deviceRotation = Quaternion.Euler(this.m_RightControllerEuler);
				}
				if (this.manipulatingLeftHand)
				{
					this.m_LeftHandState.euler = this.m_LeftHandState.euler + vector6;
					this.m_LeftHandState.rotation = Quaternion.Euler(this.m_LeftHandState.euler);
				}
				if (this.manipulatingRightHand)
				{
					this.m_RightHandState.euler = this.m_RightHandState.euler + vector6;
					this.m_RightHandState.rotation = Quaternion.Euler(this.m_RightHandState.euler);
				}
				if (this.m_TargetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.HMD))
				{
					this.m_CenterEyeEuler += vector6;
					this.m_HMDState.centerEyeRotation = Quaternion.Euler(this.m_CenterEyeEuler);
					this.m_HMDState.deviceRotation = this.m_HMDState.centerEyeRotation;
				}
				if (this.m_ResetInput)
				{
					Vector3 resetScale2 = this.GetResetScale();
					if (this.manipulatingLeftController)
					{
						this.m_LeftControllerEuler = Vector3.Scale(this.m_LeftControllerEuler, resetScale2);
						this.m_LeftControllerState.deviceRotation = Quaternion.Euler(this.m_LeftControllerEuler);
					}
					if (this.manipulatingRightController)
					{
						this.m_RightControllerEuler = Vector3.Scale(this.m_RightControllerEuler, resetScale2);
						this.m_RightControllerState.deviceRotation = Quaternion.Euler(this.m_RightControllerEuler);
					}
					if (this.manipulatingLeftHand)
					{
						this.m_LeftHandState.euler = Vector3.Scale(this.m_LeftHandState.euler, resetScale2);
						this.m_LeftHandState.rotation = Quaternion.Euler(this.m_LeftHandState.euler);
					}
					if (this.manipulatingRightHand)
					{
						this.m_RightHandState.euler = Vector3.Scale(this.m_RightHandState.euler, resetScale2);
						this.m_RightHandState.rotation = Quaternion.Euler(this.m_RightHandState.euler);
					}
					if (this.m_TargetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.HMD))
					{
						this.m_CenterEyeEuler = Vector3.Scale(this.m_CenterEyeEuler, resetScale2);
						this.m_HMDState.centerEyeRotation = Quaternion.Euler(this.m_CenterEyeEuler);
						this.m_HMDState.deviceRotation = this.m_HMDState.centerEyeRotation;
					}
				}
			}
		}

		protected virtual void ProcessControlInput()
		{
			if (this.m_DeviceLifecycleManager.deviceMode != SimulatedDeviceLifecycleManager.DeviceMode.Controller)
			{
				return;
			}
			this.ProcessAxis2DControlInput();
			if (this.manipulatingLeftController)
			{
				this.ProcessButtonControlInput(ref this.m_LeftControllerState);
			}
			else
			{
				this.ProcessAnalogButtonControlInput(ref this.m_LeftControllerState);
			}
			if (this.manipulatingRightController)
			{
				this.ProcessButtonControlInput(ref this.m_RightControllerState);
				return;
			}
			this.ProcessAnalogButtonControlInput(ref this.m_RightControllerState);
		}

		private void ProcessHandExpressionInput()
		{
		}

		private void ToggleHandExpression(UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedHandExpression simulatedExpression)
		{
		}

		protected virtual void ProcessAxis2DControlInput()
		{
			if ((this.m_TargetedDeviceInput & (XRDeviceSimulator.TargetedDevices.LeftDevice | XRDeviceSimulator.TargetedDevices.RightDevice)) == XRDeviceSimulator.TargetedDevices.None)
			{
				return;
			}
			if ((this.axis2DTargets & XRDeviceSimulator.Axis2DTargets.Primary2DAxis) != XRDeviceSimulator.Axis2DTargets.None)
			{
				if (this.manipulatingLeftController)
				{
					this.m_LeftControllerState.primary2DAxis = this.m_Axis2DInput;
				}
				if (this.manipulatingRightController)
				{
					this.m_RightControllerState.primary2DAxis = this.m_Axis2DInput;
				}
				if (this.manipulatingLeftController ^ this.manipulatingRightController)
				{
					if (this.m_RestingHandAxis2DInput != Vector2.zero || this.m_ManipulatedRestingHandAxis2D)
					{
						if (this.manipulatingLeftController)
						{
							this.m_RightControllerState.primary2DAxis = this.m_RestingHandAxis2DInput;
						}
						if (this.manipulatingRightController)
						{
							this.m_LeftControllerState.primary2DAxis = this.m_RestingHandAxis2DInput;
						}
						this.m_ManipulatedRestingHandAxis2D = (this.m_RestingHandAxis2DInput != Vector2.zero);
					}
					else
					{
						this.m_ManipulatedRestingHandAxis2D = false;
					}
				}
			}
			if ((this.axis2DTargets & XRDeviceSimulator.Axis2DTargets.Secondary2DAxis) != XRDeviceSimulator.Axis2DTargets.None)
			{
				if (this.manipulatingLeftController)
				{
					this.m_LeftControllerState.secondary2DAxis = this.m_Axis2DInput;
				}
				if (this.manipulatingRightController)
				{
					this.m_RightControllerState.secondary2DAxis = this.m_Axis2DInput;
				}
				if (this.manipulatingLeftController ^ this.manipulatingRightController)
				{
					if (this.m_RestingHandAxis2DInput != Vector2.zero || this.m_ManipulatedRestingHandAxis2D)
					{
						if (this.manipulatingLeftController)
						{
							this.m_RightControllerState.secondary2DAxis = this.m_RestingHandAxis2DInput;
						}
						if (this.manipulatingRightController)
						{
							this.m_LeftControllerState.secondary2DAxis = this.m_RestingHandAxis2DInput;
						}
						this.m_ManipulatedRestingHandAxis2D = (this.m_RestingHandAxis2DInput != Vector2.zero);
						return;
					}
					this.m_ManipulatedRestingHandAxis2D = false;
				}
			}
		}

		protected virtual void ProcessButtonControlInput(ref XRSimulatedControllerState controllerState)
		{
			controllerState.grip = (this.m_GripInput ? this.m_GripAmount : 0f);
			controllerState.WithButton(ControllerButton.GripButton, this.m_GripInput);
			controllerState.trigger = (this.m_TriggerInput ? this.m_TriggerAmount : 0f);
			controllerState.WithButton(ControllerButton.TriggerButton, this.m_TriggerInput);
			controllerState.WithButton(ControllerButton.PrimaryButton, this.m_PrimaryButtonInput);
			controllerState.WithButton(ControllerButton.SecondaryButton, this.m_SecondaryButtonInput);
			controllerState.WithButton(ControllerButton.MenuButton, this.m_MenuInput);
			controllerState.WithButton(ControllerButton.Primary2DAxisClick, this.m_Primary2DAxisClickInput);
			controllerState.WithButton(ControllerButton.Secondary2DAxisClick, this.m_Secondary2DAxisClickInput);
			controllerState.WithButton(ControllerButton.Primary2DAxisTouch, this.m_Primary2DAxisTouchInput);
			controllerState.WithButton(ControllerButton.Secondary2DAxisTouch, this.m_Secondary2DAxisTouchInput);
			controllerState.WithButton(ControllerButton.PrimaryTouch, this.m_PrimaryTouchInput);
			controllerState.WithButton(ControllerButton.SecondaryTouch, this.m_SecondaryTouchInput);
		}

		protected virtual void ProcessAnalogButtonControlInput(ref XRSimulatedControllerState controllerState)
		{
			if (controllerState.HasButton(ControllerButton.GripButton))
			{
				controllerState.grip = this.m_GripAmount;
			}
			if (controllerState.HasButton(ControllerButton.TriggerButton))
			{
				controllerState.trigger = this.m_TriggerAmount;
			}
		}

		protected Vector3 GetResetScale()
		{
			if (!this.m_XConstraintInput && !this.m_YConstraintInput && !this.m_ZConstraintInput)
			{
				return Vector3.zero;
			}
			return new Vector3(this.m_XConstraintInput ? 0f : 1f, this.m_YConstraintInput ? 0f : 1f, this.m_ZConstraintInput ? 0f : 1f);
		}

		public static XRDeviceSimulator.TransformationMode Negate(XRDeviceSimulator.TransformationMode mode)
		{
			if (mode == XRDeviceSimulator.TransformationMode.Translate)
			{
				return XRDeviceSimulator.TransformationMode.Rotate;
			}
			if (mode == XRDeviceSimulator.TransformationMode.Rotate)
			{
				return XRDeviceSimulator.TransformationMode.Translate;
			}
			return XRDeviceSimulator.TransformationMode.Rotate;
		}

		private CursorLockMode Negate(CursorLockMode mode)
		{
			if (mode == CursorLockMode.None)
			{
				return this.m_DesiredCursorLockMode;
			}
			if (mode - CursorLockMode.Locked > 1)
			{
				return CursorLockMode.None;
			}
			return CursorLockMode.None;
		}

		private void SubscribeKeyboardXTranslateAction()
		{
			XRSimulatorUtility.Subscribe(this.m_KeyboardXTranslateAction, new Action<InputAction.CallbackContext>(this.OnKeyboardXTranslatePerformed), new Action<InputAction.CallbackContext>(this.OnKeyboardXTranslateCanceled));
		}

		private void UnsubscribeKeyboardXTranslateAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_KeyboardXTranslateAction, new Action<InputAction.CallbackContext>(this.OnKeyboardXTranslatePerformed), new Action<InputAction.CallbackContext>(this.OnKeyboardXTranslateCanceled));
		}

		private void SubscribeKeyboardYTranslateAction()
		{
			XRSimulatorUtility.Subscribe(this.m_KeyboardYTranslateAction, new Action<InputAction.CallbackContext>(this.OnKeyboardYTranslatePerformed), new Action<InputAction.CallbackContext>(this.OnKeyboardYTranslateCanceled));
		}

		private void UnsubscribeKeyboardYTranslateAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_KeyboardYTranslateAction, new Action<InputAction.CallbackContext>(this.OnKeyboardYTranslatePerformed), new Action<InputAction.CallbackContext>(this.OnKeyboardYTranslateCanceled));
		}

		private void SubscribeKeyboardZTranslateAction()
		{
			XRSimulatorUtility.Subscribe(this.m_KeyboardZTranslateAction, new Action<InputAction.CallbackContext>(this.OnKeyboardZTranslatePerformed), new Action<InputAction.CallbackContext>(this.OnKeyboardZTranslateCanceled));
		}

		private void UnsubscribeKeyboardZTranslateAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_KeyboardZTranslateAction, new Action<InputAction.CallbackContext>(this.OnKeyboardZTranslatePerformed), new Action<InputAction.CallbackContext>(this.OnKeyboardZTranslateCanceled));
		}

		private void SubscribeManipulateLeftAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ManipulateLeftAction, new Action<InputAction.CallbackContext>(this.OnManipulateLeftPerformed), new Action<InputAction.CallbackContext>(this.OnManipulateLeftCanceled));
		}

		private void UnsubscribeManipulateLeftAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ManipulateLeftAction, new Action<InputAction.CallbackContext>(this.OnManipulateLeftPerformed), new Action<InputAction.CallbackContext>(this.OnManipulateLeftCanceled));
		}

		private void SubscribeManipulateRightAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ManipulateRightAction, new Action<InputAction.CallbackContext>(this.OnManipulateRightPerformed), new Action<InputAction.CallbackContext>(this.OnManipulateRightCanceled));
		}

		private void UnsubscribeManipulateRightAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ManipulateRightAction, new Action<InputAction.CallbackContext>(this.OnManipulateRightPerformed), new Action<InputAction.CallbackContext>(this.OnManipulateRightCanceled));
		}

		private void SubscribeToggleManipulateLeftAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ToggleManipulateLeftAction, new Action<InputAction.CallbackContext>(this.OnToggleManipulateLeftPerformed), null);
		}

		private void UnsubscribeToggleManipulateLeftAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ToggleManipulateLeftAction, new Action<InputAction.CallbackContext>(this.OnToggleManipulateLeftPerformed), null);
		}

		private void SubscribeToggleManipulateRightAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ToggleManipulateRightAction, new Action<InputAction.CallbackContext>(this.OnToggleManipulateRightPerformed), null);
		}

		private void UnsubscribeToggleManipulateRightAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ToggleManipulateRightAction, new Action<InputAction.CallbackContext>(this.OnToggleManipulateRightPerformed), null);
		}

		private void SubscribeToggleManipulateBodyAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ToggleManipulateBodyAction, new Action<InputAction.CallbackContext>(this.OnToggleManipulateBodyPerformed), null);
		}

		private void UnsubscribeToggleManipulateBodyAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ToggleManipulateBodyAction, new Action<InputAction.CallbackContext>(this.OnToggleManipulateBodyPerformed), null);
		}

		private void SubscribeManipulateHeadAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ManipulateHeadAction, new Action<InputAction.CallbackContext>(this.OnManipulateHeadPerformed), new Action<InputAction.CallbackContext>(this.OnManipulateHeadCanceled));
		}

		private void UnsubscribeManipulateHeadAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ManipulateHeadAction, new Action<InputAction.CallbackContext>(this.OnManipulateHeadPerformed), new Action<InputAction.CallbackContext>(this.OnManipulateHeadCanceled));
		}

		private void SubscribeHandControllerModeAction()
		{
			XRSimulatorUtility.Subscribe(this.m_HandControllerModeAction, new Action<InputAction.CallbackContext>(this.OnHandControllerModePerformed), null);
		}

		private void UnsubscribeHandControllerModeAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_HandControllerModeAction, new Action<InputAction.CallbackContext>(this.OnHandControllerModePerformed), null);
		}

		private void SubscribeCycleDevicesAction()
		{
			XRSimulatorUtility.Subscribe(this.m_CycleDevicesAction, new Action<InputAction.CallbackContext>(this.OnCycleDevicesPerformed), null);
		}

		private void UnsubscribeCycleDevicesAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_CycleDevicesAction, new Action<InputAction.CallbackContext>(this.OnCycleDevicesPerformed), null);
		}

		private void SubscribeStopManipulationAction()
		{
			XRSimulatorUtility.Subscribe(this.m_StopManipulationAction, new Action<InputAction.CallbackContext>(this.OnStopManipulationPerformed), null);
		}

		private void UnsubscribeStopManipulationAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_StopManipulationAction, new Action<InputAction.CallbackContext>(this.OnStopManipulationPerformed), null);
		}

		private void SubscribeMouseDeltaAction()
		{
			XRSimulatorUtility.Subscribe(this.m_MouseDeltaAction, new Action<InputAction.CallbackContext>(this.OnMouseDeltaPerformed), new Action<InputAction.CallbackContext>(this.OnMouseDeltaCanceled));
		}

		private void UnsubscribeMouseDeltaAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_MouseDeltaAction, new Action<InputAction.CallbackContext>(this.OnMouseDeltaPerformed), new Action<InputAction.CallbackContext>(this.OnMouseDeltaCanceled));
		}

		private void SubscribeMouseScrollAction()
		{
			XRSimulatorUtility.Subscribe(this.m_MouseScrollAction, new Action<InputAction.CallbackContext>(this.OnMouseScrollPerformed), new Action<InputAction.CallbackContext>(this.OnMouseScrollCanceled));
		}

		private void UnsubscribeMouseScrollAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_MouseScrollAction, new Action<InputAction.CallbackContext>(this.OnMouseScrollPerformed), new Action<InputAction.CallbackContext>(this.OnMouseScrollCanceled));
		}

		private void SubscribeRotateModeOverrideAction()
		{
			XRSimulatorUtility.Subscribe(this.m_RotateModeOverrideAction, new Action<InputAction.CallbackContext>(this.OnRotateModeOverridePerformed), new Action<InputAction.CallbackContext>(this.OnRotateModeOverrideCanceled));
		}

		private void UnsubscribeRotateModeOverrideAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_RotateModeOverrideAction, new Action<InputAction.CallbackContext>(this.OnRotateModeOverridePerformed), new Action<InputAction.CallbackContext>(this.OnRotateModeOverrideCanceled));
		}

		private void SubscribeToggleMouseTransformationModeAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ToggleMouseTransformationModeAction, new Action<InputAction.CallbackContext>(this.OnToggleMouseTransformationModePerformed), null);
		}

		private void UnsubscribeToggleMouseTransformationModeAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ToggleMouseTransformationModeAction, new Action<InputAction.CallbackContext>(this.OnToggleMouseTransformationModePerformed), null);
		}

		private void SubscribeNegateModeAction()
		{
			XRSimulatorUtility.Subscribe(this.m_NegateModeAction, new Action<InputAction.CallbackContext>(this.OnNegateModePerformed), new Action<InputAction.CallbackContext>(this.OnNegateModeCanceled));
		}

		private void UnsubscribeNegateModeAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_NegateModeAction, new Action<InputAction.CallbackContext>(this.OnNegateModePerformed), new Action<InputAction.CallbackContext>(this.OnNegateModeCanceled));
		}

		private void SubscribeXConstraintAction()
		{
			XRSimulatorUtility.Subscribe(this.m_XConstraintAction, new Action<InputAction.CallbackContext>(this.OnXConstraintPerformed), new Action<InputAction.CallbackContext>(this.OnXConstraintCanceled));
		}

		private void UnsubscribeXConstraintAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_XConstraintAction, new Action<InputAction.CallbackContext>(this.OnXConstraintPerformed), new Action<InputAction.CallbackContext>(this.OnXConstraintCanceled));
		}

		private void SubscribeYConstraintAction()
		{
			XRSimulatorUtility.Subscribe(this.m_YConstraintAction, new Action<InputAction.CallbackContext>(this.OnYConstraintPerformed), new Action<InputAction.CallbackContext>(this.OnYConstraintCanceled));
		}

		private void UnsubscribeYConstraintAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_YConstraintAction, new Action<InputAction.CallbackContext>(this.OnYConstraintPerformed), new Action<InputAction.CallbackContext>(this.OnYConstraintCanceled));
		}

		private void SubscribeZConstraintAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ZConstraintAction, new Action<InputAction.CallbackContext>(this.OnZConstraintPerformed), new Action<InputAction.CallbackContext>(this.OnZConstraintCanceled));
		}

		private void UnsubscribeZConstraintAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ZConstraintAction, new Action<InputAction.CallbackContext>(this.OnZConstraintPerformed), new Action<InputAction.CallbackContext>(this.OnZConstraintCanceled));
		}

		private void SubscribeResetAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ResetAction, new Action<InputAction.CallbackContext>(this.OnResetPerformed), new Action<InputAction.CallbackContext>(this.OnResetCanceled));
		}

		private void UnsubscribeResetAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ResetAction, new Action<InputAction.CallbackContext>(this.OnResetPerformed), new Action<InputAction.CallbackContext>(this.OnResetCanceled));
		}

		private void SubscribeToggleCursorLockAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ToggleCursorLockAction, new Action<InputAction.CallbackContext>(this.OnToggleCursorLockPerformed), null);
		}

		private void UnsubscribeToggleCursorLockAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ToggleCursorLockAction, new Action<InputAction.CallbackContext>(this.OnToggleCursorLockPerformed), null);
		}

		private void SubscribeToggleDevicePositionTargetAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ToggleDevicePositionTargetAction, new Action<InputAction.CallbackContext>(this.OnToggleDevicePositionTargetPerformed), null);
		}

		private void UnsubscribeToggleDevicePositionTargetAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ToggleDevicePositionTargetAction, new Action<InputAction.CallbackContext>(this.OnToggleDevicePositionTargetPerformed), null);
		}

		private void SubscribeTogglePrimary2DAxisTargetAction()
		{
			XRSimulatorUtility.Subscribe(this.m_TogglePrimary2DAxisTargetAction, new Action<InputAction.CallbackContext>(this.OnTogglePrimary2DAxisTargetPerformed), null);
		}

		private void UnsubscribeTogglePrimary2DAxisTargetAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_TogglePrimary2DAxisTargetAction, new Action<InputAction.CallbackContext>(this.OnTogglePrimary2DAxisTargetPerformed), null);
		}

		private void SubscribeToggleSecondary2DAxisTargetAction()
		{
			XRSimulatorUtility.Subscribe(this.m_ToggleSecondary2DAxisTargetAction, new Action<InputAction.CallbackContext>(this.OnToggleSecondary2DAxisTargetPerformed), null);
		}

		private void UnsubscribeToggleSecondary2DAxisTargetAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_ToggleSecondary2DAxisTargetAction, new Action<InputAction.CallbackContext>(this.OnToggleSecondary2DAxisTargetPerformed), null);
		}

		private void SubscribeAxis2DAction()
		{
			XRSimulatorUtility.Subscribe(this.m_Axis2DAction, new Action<InputAction.CallbackContext>(this.OnAxis2DPerformed), new Action<InputAction.CallbackContext>(this.OnAxis2DCanceled));
		}

		private void UnsubscribeAxis2DAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_Axis2DAction, new Action<InputAction.CallbackContext>(this.OnAxis2DPerformed), new Action<InputAction.CallbackContext>(this.OnAxis2DCanceled));
		}

		private void SubscribeRestingHandAxis2DAction()
		{
			XRSimulatorUtility.Subscribe(this.m_RestingHandAxis2DAction, new Action<InputAction.CallbackContext>(this.OnRestingHandAxis2DPerformed), new Action<InputAction.CallbackContext>(this.OnRestingHandAxis2DCanceled));
		}

		private void UnsubscribeRestingHandAxis2DAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_RestingHandAxis2DAction, new Action<InputAction.CallbackContext>(this.OnRestingHandAxis2DPerformed), new Action<InputAction.CallbackContext>(this.OnRestingHandAxis2DCanceled));
		}

		private void SubscribeGripAction()
		{
			XRSimulatorUtility.Subscribe(this.m_GripAction, new Action<InputAction.CallbackContext>(this.OnGripPerformed), new Action<InputAction.CallbackContext>(this.OnGripCanceled));
		}

		private void UnsubscribeGripAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_GripAction, new Action<InputAction.CallbackContext>(this.OnGripPerformed), new Action<InputAction.CallbackContext>(this.OnGripCanceled));
		}

		private void SubscribeTriggerAction()
		{
			XRSimulatorUtility.Subscribe(this.m_TriggerAction, new Action<InputAction.CallbackContext>(this.OnTriggerPerformed), new Action<InputAction.CallbackContext>(this.OnTriggerCanceled));
		}

		private void UnsubscribeTriggerAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_TriggerAction, new Action<InputAction.CallbackContext>(this.OnTriggerPerformed), new Action<InputAction.CallbackContext>(this.OnTriggerCanceled));
		}

		private void SubscribePrimaryButtonAction()
		{
			XRSimulatorUtility.Subscribe(this.m_PrimaryButtonAction, new Action<InputAction.CallbackContext>(this.OnPrimaryButtonPerformed), new Action<InputAction.CallbackContext>(this.OnPrimaryButtonCanceled));
		}

		private void UnsubscribePrimaryButtonAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_PrimaryButtonAction, new Action<InputAction.CallbackContext>(this.OnPrimaryButtonPerformed), new Action<InputAction.CallbackContext>(this.OnPrimaryButtonCanceled));
		}

		private void SubscribeSecondaryButtonAction()
		{
			XRSimulatorUtility.Subscribe(this.m_SecondaryButtonAction, new Action<InputAction.CallbackContext>(this.OnSecondaryButtonPerformed), new Action<InputAction.CallbackContext>(this.OnSecondaryButtonCanceled));
		}

		private void UnsubscribeSecondaryButtonAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_SecondaryButtonAction, new Action<InputAction.CallbackContext>(this.OnSecondaryButtonPerformed), new Action<InputAction.CallbackContext>(this.OnSecondaryButtonCanceled));
		}

		private void SubscribeMenuAction()
		{
			XRSimulatorUtility.Subscribe(this.m_MenuAction, new Action<InputAction.CallbackContext>(this.OnMenuPerformed), new Action<InputAction.CallbackContext>(this.OnMenuCanceled));
		}

		private void UnsubscribeMenuAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_MenuAction, new Action<InputAction.CallbackContext>(this.OnMenuPerformed), new Action<InputAction.CallbackContext>(this.OnMenuCanceled));
		}

		private void SubscribePrimary2DAxisClickAction()
		{
			XRSimulatorUtility.Subscribe(this.m_Primary2DAxisClickAction, new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisClickPerformed), new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisClickCanceled));
		}

		private void UnsubscribePrimary2DAxisClickAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_Primary2DAxisClickAction, new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisClickPerformed), new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisClickCanceled));
		}

		private void SubscribeSecondary2DAxisClickAction()
		{
			XRSimulatorUtility.Subscribe(this.m_Secondary2DAxisClickAction, new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisClickPerformed), new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisClickCanceled));
		}

		private void UnsubscribeSecondary2DAxisClickAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_Secondary2DAxisClickAction, new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisClickPerformed), new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisClickCanceled));
		}

		private void SubscribePrimary2DAxisTouchAction()
		{
			XRSimulatorUtility.Subscribe(this.m_Primary2DAxisTouchAction, new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisTouchPerformed), new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisTouchCanceled));
		}

		private void UnsubscribePrimary2DAxisTouchAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_Primary2DAxisTouchAction, new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisTouchPerformed), new Action<InputAction.CallbackContext>(this.OnPrimary2DAxisTouchCanceled));
		}

		private void SubscribeSecondary2DAxisTouchAction()
		{
			XRSimulatorUtility.Subscribe(this.m_Secondary2DAxisTouchAction, new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisTouchPerformed), new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisTouchCanceled));
		}

		private void UnsubscribeSecondary2DAxisTouchAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_Secondary2DAxisTouchAction, new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisTouchPerformed), new Action<InputAction.CallbackContext>(this.OnSecondary2DAxisTouchCanceled));
		}

		private void SubscribePrimaryTouchAction()
		{
			XRSimulatorUtility.Subscribe(this.m_PrimaryTouchAction, new Action<InputAction.CallbackContext>(this.OnPrimaryTouchPerformed), new Action<InputAction.CallbackContext>(this.OnPrimaryTouchCanceled));
		}

		private void UnsubscribePrimaryTouchAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_PrimaryTouchAction, new Action<InputAction.CallbackContext>(this.OnPrimaryTouchPerformed), new Action<InputAction.CallbackContext>(this.OnPrimaryTouchCanceled));
		}

		private void SubscribeSecondaryTouchAction()
		{
			XRSimulatorUtility.Subscribe(this.m_SecondaryTouchAction, new Action<InputAction.CallbackContext>(this.OnSecondaryTouchPerformed), new Action<InputAction.CallbackContext>(this.OnSecondaryTouchCanceled));
		}

		private void UnsubscribeSecondaryTouchAction()
		{
			XRSimulatorUtility.Unsubscribe(this.m_SecondaryTouchAction, new Action<InputAction.CallbackContext>(this.OnSecondaryTouchPerformed), new Action<InputAction.CallbackContext>(this.OnSecondaryTouchCanceled));
		}

		private void OnKeyboardXTranslatePerformed(InputAction.CallbackContext context)
		{
			this.m_KeyboardXTranslateInput = context.ReadValue<float>();
		}

		private void OnKeyboardXTranslateCanceled(InputAction.CallbackContext context)
		{
			this.m_KeyboardXTranslateInput = 0f;
		}

		private void OnKeyboardYTranslatePerformed(InputAction.CallbackContext context)
		{
			this.m_KeyboardYTranslateInput = context.ReadValue<float>();
		}

		private void OnKeyboardYTranslateCanceled(InputAction.CallbackContext context)
		{
			this.m_KeyboardYTranslateInput = 0f;
		}

		private void OnKeyboardZTranslatePerformed(InputAction.CallbackContext context)
		{
			this.m_KeyboardZTranslateInput = context.ReadValue<float>();
		}

		private void OnKeyboardZTranslateCanceled(InputAction.CallbackContext context)
		{
			this.m_KeyboardZTranslateInput = 0f;
		}

		private void OnManipulateLeftPerformed(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = this.targetedDeviceInput.WithDevice(XRDeviceSimulator.TargetedDevices.LeftDevice);
		}

		private void OnManipulateLeftCanceled(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(XRDeviceSimulator.TargetedDevices.LeftDevice);
		}

		private void OnManipulateRightPerformed(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = this.targetedDeviceInput.WithDevice(XRDeviceSimulator.TargetedDevices.RightDevice);
		}

		private void OnManipulateRightCanceled(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(XRDeviceSimulator.TargetedDevices.RightDevice);
		}

		private void OnToggleManipulateLeftPerformed(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = ((!this.targetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.LeftDevice)) ? this.targetedDeviceInput.WithDevice(XRDeviceSimulator.TargetedDevices.LeftDevice).WithoutDevice(XRDeviceSimulator.TargetedDevices.RightDevice) : XRDeviceSimulator.TargetedDevices.FPS);
		}

		private void OnToggleManipulateRightPerformed(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = ((!this.targetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.RightDevice)) ? this.targetedDeviceInput.WithDevice(XRDeviceSimulator.TargetedDevices.RightDevice).WithoutDevice(XRDeviceSimulator.TargetedDevices.LeftDevice) : XRDeviceSimulator.TargetedDevices.FPS);
		}

		private void OnToggleManipulateBodyPerformed(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = XRDeviceSimulator.TargetedDevices.FPS;
		}

		private void OnManipulateHeadPerformed(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = this.targetedDeviceInput.WithDevice(XRDeviceSimulator.TargetedDevices.HMD);
		}

		private void OnManipulateHeadCanceled(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(XRDeviceSimulator.TargetedDevices.HMD);
		}

		private void OnHandControllerModePerformed(InputAction.CallbackContext context)
		{
			if (this.m_DeviceLifecycleManager != null)
			{
				this.m_DeviceLifecycleManager.SwitchDeviceMode();
			}
		}

		private void OnCycleDevicesPerformed(InputAction.CallbackContext context)
		{
			if (this.targetedDeviceInput == XRDeviceSimulator.TargetedDevices.None)
			{
				this.targetedDeviceInput = XRDeviceSimulator.TargetedDevices.FPS;
				return;
			}
			if (this.targetedDeviceInput == XRDeviceSimulator.TargetedDevices.FPS)
			{
				this.targetedDeviceInput = XRDeviceSimulator.TargetedDevices.LeftDevice;
				return;
			}
			if (this.targetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.LeftDevice))
			{
				this.targetedDeviceInput = XRDeviceSimulator.TargetedDevices.RightDevice;
				return;
			}
			if (this.targetedDeviceInput.HasDevice(XRDeviceSimulator.TargetedDevices.RightDevice))
			{
				this.targetedDeviceInput = XRDeviceSimulator.TargetedDevices.FPS;
			}
		}

		private void OnStopManipulationPerformed(InputAction.CallbackContext context)
		{
			this.targetedDeviceInput = XRDeviceSimulator.TargetedDevices.None;
		}

		private void OnMouseDeltaPerformed(InputAction.CallbackContext context)
		{
			this.m_MouseDeltaInput = context.ReadValue<Vector2>();
		}

		private void OnMouseDeltaCanceled(InputAction.CallbackContext context)
		{
			this.m_MouseDeltaInput = Vector2.zero;
		}

		private void OnMouseScrollPerformed(InputAction.CallbackContext context)
		{
			this.m_MouseScrollInput = context.ReadValue<Vector2>();
		}

		private void OnMouseScrollCanceled(InputAction.CallbackContext context)
		{
			this.m_MouseScrollInput = Vector2.zero;
		}

		private void OnRotateModeOverridePerformed(InputAction.CallbackContext context)
		{
			this.m_RotateModeOverrideInput = true;
		}

		private void OnRotateModeOverrideCanceled(InputAction.CallbackContext context)
		{
			this.m_RotateModeOverrideInput = false;
		}

		private void OnToggleMouseTransformationModePerformed(InputAction.CallbackContext context)
		{
			this.mouseTransformationMode = XRDeviceSimulator.Negate(this.mouseTransformationMode);
		}

		private void OnNegateModePerformed(InputAction.CallbackContext context)
		{
			this.negateMode = true;
		}

		private void OnNegateModeCanceled(InputAction.CallbackContext context)
		{
			this.negateMode = false;
		}

		private void OnXConstraintPerformed(InputAction.CallbackContext context)
		{
			this.m_XConstraintInput = true;
		}

		private void OnXConstraintCanceled(InputAction.CallbackContext context)
		{
			this.m_XConstraintInput = false;
		}

		private void OnYConstraintPerformed(InputAction.CallbackContext context)
		{
			this.m_YConstraintInput = true;
		}

		private void OnYConstraintCanceled(InputAction.CallbackContext context)
		{
			this.m_YConstraintInput = false;
		}

		private void OnZConstraintPerformed(InputAction.CallbackContext context)
		{
			this.m_ZConstraintInput = true;
		}

		private void OnZConstraintCanceled(InputAction.CallbackContext context)
		{
			this.m_ZConstraintInput = false;
		}

		private void OnResetPerformed(InputAction.CallbackContext context)
		{
			this.m_ResetInput = true;
		}

		private void OnResetCanceled(InputAction.CallbackContext context)
		{
			this.m_ResetInput = false;
		}

		private void OnToggleCursorLockPerformed(InputAction.CallbackContext context)
		{
			Cursor.lockState = this.Negate(Cursor.lockState);
		}

		private void OnToggleDevicePositionTargetPerformed(InputAction.CallbackContext context)
		{
			this.axis2DTargets = (((this.axis2DTargets & XRDeviceSimulator.Axis2DTargets.Position) != XRDeviceSimulator.Axis2DTargets.None) ? XRDeviceSimulator.Axis2DTargets.None : XRDeviceSimulator.Axis2DTargets.Position);
		}

		private void OnTogglePrimary2DAxisTargetPerformed(InputAction.CallbackContext context)
		{
			this.axis2DTargets = (((this.axis2DTargets & XRDeviceSimulator.Axis2DTargets.Primary2DAxis) != XRDeviceSimulator.Axis2DTargets.None) ? XRDeviceSimulator.Axis2DTargets.None : XRDeviceSimulator.Axis2DTargets.Primary2DAxis);
		}

		private void OnToggleSecondary2DAxisTargetPerformed(InputAction.CallbackContext context)
		{
			this.axis2DTargets = (((this.axis2DTargets & XRDeviceSimulator.Axis2DTargets.Secondary2DAxis) != XRDeviceSimulator.Axis2DTargets.None) ? XRDeviceSimulator.Axis2DTargets.None : XRDeviceSimulator.Axis2DTargets.Secondary2DAxis);
		}

		private void OnAxis2DPerformed(InputAction.CallbackContext context)
		{
			this.m_Axis2DInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
		}

		private void OnAxis2DCanceled(InputAction.CallbackContext context)
		{
			this.m_Axis2DInput = Vector2.zero;
		}

		private void OnRestingHandAxis2DPerformed(InputAction.CallbackContext context)
		{
			this.m_RestingHandAxis2DInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
		}

		private void OnRestingHandAxis2DCanceled(InputAction.CallbackContext context)
		{
			this.m_RestingHandAxis2DInput = Vector2.zero;
		}

		private void OnGripPerformed(InputAction.CallbackContext context)
		{
			this.m_GripInput = true;
		}

		private void OnGripCanceled(InputAction.CallbackContext context)
		{
			this.m_GripInput = false;
		}

		private void OnTriggerPerformed(InputAction.CallbackContext context)
		{
			this.m_TriggerInput = true;
		}

		private void OnTriggerCanceled(InputAction.CallbackContext context)
		{
			this.m_TriggerInput = false;
		}

		private void OnPrimaryButtonPerformed(InputAction.CallbackContext context)
		{
			this.m_PrimaryButtonInput = true;
		}

		private void OnPrimaryButtonCanceled(InputAction.CallbackContext context)
		{
			this.m_PrimaryButtonInput = false;
		}

		private void OnSecondaryButtonPerformed(InputAction.CallbackContext context)
		{
			this.m_SecondaryButtonInput = true;
		}

		private void OnSecondaryButtonCanceled(InputAction.CallbackContext context)
		{
			this.m_SecondaryButtonInput = false;
		}

		private void OnMenuPerformed(InputAction.CallbackContext context)
		{
			this.m_MenuInput = true;
		}

		private void OnMenuCanceled(InputAction.CallbackContext context)
		{
			this.m_MenuInput = false;
		}

		private void OnPrimary2DAxisClickPerformed(InputAction.CallbackContext context)
		{
			this.m_Primary2DAxisClickInput = true;
		}

		private void OnPrimary2DAxisClickCanceled(InputAction.CallbackContext context)
		{
			this.m_Primary2DAxisClickInput = false;
		}

		private void OnSecondary2DAxisClickPerformed(InputAction.CallbackContext context)
		{
			this.m_Secondary2DAxisClickInput = true;
		}

		private void OnSecondary2DAxisClickCanceled(InputAction.CallbackContext context)
		{
			this.m_Secondary2DAxisClickInput = false;
		}

		private void OnPrimary2DAxisTouchPerformed(InputAction.CallbackContext context)
		{
			this.m_Primary2DAxisTouchInput = true;
		}

		private void OnPrimary2DAxisTouchCanceled(InputAction.CallbackContext context)
		{
			this.m_Primary2DAxisTouchInput = false;
		}

		private void OnSecondary2DAxisTouchPerformed(InputAction.CallbackContext context)
		{
			this.m_Secondary2DAxisTouchInput = true;
		}

		private void OnSecondary2DAxisTouchCanceled(InputAction.CallbackContext context)
		{
			this.m_Secondary2DAxisTouchInput = false;
		}

		private void OnPrimaryTouchPerformed(InputAction.CallbackContext context)
		{
			this.m_PrimaryTouchInput = true;
		}

		private void OnPrimaryTouchCanceled(InputAction.CallbackContext context)
		{
			this.m_PrimaryTouchInput = false;
		}

		private void OnSecondaryTouchPerformed(InputAction.CallbackContext context)
		{
			this.m_SecondaryTouchInput = true;
		}

		private void OnSecondaryTouchCanceled(InputAction.CallbackContext context)
		{
			this.m_SecondaryTouchInput = false;
		}

		[Obsolete("simulatedHandExpressions has been deprecated in XRI 3.1.0. Update the XR Device Simulator sample in Package Manager or use simulatedHandExpressions in the SimulatedHandExpressionManager instead.")]
		public List<XRDeviceSimulator.SimulatedHandExpression> simulatedHandExpressions
		{
			get
			{
				return this.m_SimulatedHandExpressions;
			}
		}

		[Obsolete("removeOtherHMDDevices has been deprecated in XRI 3.1.0. Use removeOtherHMDDevices in the SimulatedDeviceLifecycleManager instead.")]
		public bool removeOtherHMDDevices
		{
			get
			{
				return this.m_DeviceLifecycleManager != null && this.m_DeviceLifecycleManager.removeOtherHMDDevices;
			}
			set
			{
				if (this.m_DeviceLifecycleManager != null)
				{
					this.m_DeviceLifecycleManager.removeOtherHMDDevices = value;
				}
			}
		}

		[Obsolete("handTrackingCapability has been deprecated in XRI 3.1.0. Use handTrackingCapability in the SimulatedDeviceLifecycleManager instead.")]
		public bool handTrackingCapability
		{
			get
			{
				return this.m_DeviceLifecycleManager != null && this.m_DeviceLifecycleManager.handTrackingCapability;
			}
			set
			{
				if (this.m_DeviceLifecycleManager != null)
				{
					this.m_DeviceLifecycleManager.handTrackingCapability = value;
				}
			}
		}

		[Obsolete("deviceMode has been deprecated in XRI 3.1.0 due to being moved out XR Device Simulator. Use deviceMode in the SimulatedDeviceLifecycleManager instead.")]
		public XRDeviceSimulator.DeviceMode deviceMode
		{
			get
			{
				if (!(this.m_DeviceLifecycleManager != null))
				{
					return XRDeviceSimulator.DeviceMode.Controller;
				}
				return (XRDeviceSimulator.DeviceMode)this.m_DeviceLifecycleManager.deviceMode;
			}
		}

		[Obsolete("AddDevices has been deprecated in XRI 3.1.0 and will be removed in a future release. It has instead been moved to the SimulatedDeviceLifecycleManager.", false)]
		protected virtual void AddDevices()
		{
			if (this.m_DeviceLifecycleManager != null)
			{
				this.m_DeviceLifecycleManager.AddDevices();
				return;
			}
			Debug.LogError("No Simulated Device Lifecycle Manager has been found so AddDevices() will not be called.", this);
		}

		[Obsolete("RemoveDevices has been deprecated in XRI 3.1.0 and will be removed in a future release. It has instead been moved to the SimulatedDeviceLifecycleManager.", false)]
		protected virtual void RemoveDevices()
		{
			if (this.m_DeviceLifecycleManager != null)
			{
				this.m_DeviceLifecycleManager.RemoveDevices();
				return;
			}
			Debug.LogError("No Simulated Device Lifecycle Manager has been found so RemoveDevices() will not be called.", this);
		}

		[Obsolete("InitializeHandExpressions has been deprecated in XRI 3.1.0 and moved to SimulatedHandExpressionManager.")]
		private void InitializeHandExpressions()
		{
		}

		[Obsolete("ToggleHandExpressionDeprecated has been deprecated in XRI 3.1.0 and replaced with ToggleHandExpression.")]
		private void ToggleHandExpressionDeprecated(XRDeviceSimulator.SimulatedHandExpression simulatedExpression)
		{
		}

		[SerializeField]
		[Tooltip("Input Action asset containing controls for the simulator itself. Unity will automatically enable and disable it with this component.")]
		private InputActionAsset m_DeviceSimulatorActionAsset;

		[SerializeField]
		[Tooltip("Input Action asset containing controls for the simulated controllers. Unity will automatically enable and disable it as needed.")]
		private InputActionAsset m_ControllerActionAsset;

		[SerializeField]
		[Tooltip("The Input System Action used to translate in the x-axis (left/right) while held. Must be a Value Axis Control.")]
		private InputActionReference m_KeyboardXTranslateAction;

		[SerializeField]
		[Tooltip("The Input System Action used to translate in the y-axis (up/down) while held. Must be a Value Axis Control.")]
		private InputActionReference m_KeyboardYTranslateAction;

		[SerializeField]
		[Tooltip("The Input System Action used to translate in the z-axis (forward/back) while held. Must be a Value Axis Control.")]
		private InputActionReference m_KeyboardZTranslateAction;

		[SerializeField]
		[Tooltip("The Input System Action used to enable manipulation of the left-hand controller while held. Must be a Button Control.")]
		private InputActionReference m_ManipulateLeftAction;

		[SerializeField]
		[Tooltip("The Input System Action used to enable manipulation of the right-hand controller while held. Must be a Button Control.")]
		private InputActionReference m_ManipulateRightAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle enable manipulation of the left-hand controller when pressed. Must be a Button Control.")]
		private InputActionReference m_ToggleManipulateLeftAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle enable manipulation of the right-hand controller when pressed. Must be a Button Control.")]
		private InputActionReference m_ToggleManipulateRightAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle enable looking around with the HMD and controllers. Must be a Button Control.")]
		private InputActionReference m_ToggleManipulateBodyAction;

		[SerializeField]
		[Tooltip("The Input System Action used to enable manipulation of the HMD while held. Must be a Button Control.")]
		private InputActionReference m_ManipulateHeadAction;

		[SerializeField]
		[Tooltip("The Input System Action used to change between hand and controller mode. Must be a Button Control.")]
		private InputActionReference m_HandControllerModeAction;

		[SerializeField]
		[Tooltip("The Input System Action used to cycle between the different available devices. Must be a Button Control.")]
		private InputActionReference m_CycleDevicesAction;

		[SerializeField]
		[Tooltip("The Input System Action used to stop all manipulation. Must be a Button Control.")]
		private InputActionReference m_StopManipulationAction;

		[SerializeField]
		[Tooltip("The Input System Action used to translate or rotate by a scaled amount along or about the x- and y-axes. Must be a Value Vector2 Control.")]
		private InputActionReference m_MouseDeltaAction;

		[SerializeField]
		[Tooltip("The Input System Action used to translate or rotate by a scaled amount along or about the z-axis. Must be a Value Vector2 Control.")]
		private InputActionReference m_MouseScrollAction;

		[SerializeField]
		[Tooltip("The Input System Action used to cause the manipulated device(s) to rotate when moving the mouse when held. Must be a Button Control.")]
		private InputActionReference m_RotateModeOverrideAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle between translating or rotating the manipulated device(s) when moving the mouse when pressed. Must be a Button Control.")]
		private InputActionReference m_ToggleMouseTransformationModeAction;

		[SerializeField]
		[Tooltip("The Input System Action used to cause the manipulated device(s) to rotate when moving the mouse while held when it would normally translate, and vice-versa. Must be a Button Control.")]
		private InputActionReference m_NegateModeAction;

		[SerializeField]
		[Tooltip("The Input System Action used to constrain the translation or rotation to the x-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane. Must be a Button Control.")]
		private InputActionReference m_XConstraintAction;

		[SerializeField]
		[Tooltip("The Input System Action used to constrain the translation or rotation to the y-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane. Must be a Button Control.")]
		private InputActionReference m_YConstraintAction;

		[SerializeField]
		[Tooltip("The Input System Action used to constrain the translation or rotation to the z-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane. Must be a Button Control.")]
		private InputActionReference m_ZConstraintAction;

		[SerializeField]
		[Tooltip("The Input System Action used to cause the manipulated device(s) to reset position or rotation (depending on the effective manipulation mode). Must be a Button Control.")]
		private InputActionReference m_ResetAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle the cursor lock mode for the game window when pressed. Must be a Button Control.")]
		private InputActionReference m_ToggleCursorLockAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle enable translation from keyboard inputs when pressed. Must be a Button Control.")]
		private InputActionReference m_ToggleDevicePositionTargetAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle enable manipulation of the Primary2DAxis of the controllers when pressed. Must be a Button Control.")]
		private InputActionReference m_TogglePrimary2DAxisTargetAction;

		[SerializeField]
		[Tooltip("The Input System Action used to toggle enable manipulation of the Secondary2DAxis of the controllers when pressed. Must be a Button Control.")]
		private InputActionReference m_ToggleSecondary2DAxisTargetAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the value of one or more 2D Axis controls on the manipulated controller device(s). Must be a Value Vector2 Control.")]
		private InputActionReference m_Axis2DAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control one or more 2D Axis controls on the opposite hand of the exclusively manipulated controller device. Must be a Value Vector2 Control.")]
		private InputActionReference m_RestingHandAxis2DAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the Grip control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_GripAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the Trigger control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_TriggerAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the PrimaryButton control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_PrimaryButtonAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the SecondaryButton control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_SecondaryButtonAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the Menu control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_MenuAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the Primary2DAxisClick control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_Primary2DAxisClickAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the Secondary2DAxisClick control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_Secondary2DAxisClickAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the Primary2DAxisTouch control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_Primary2DAxisTouchAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the Secondary2DAxisTouch control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_Secondary2DAxisTouchAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the PrimaryTouch control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_PrimaryTouchAction;

		[SerializeField]
		[Tooltip("The Input System Action used to control the SecondaryTouch control of the manipulated controller device(s). Must be a Button Control.")]
		private InputActionReference m_SecondaryTouchAction;

		[SerializeField]
		[Tooltip("Input Action asset containing controls for the simulated hands. Unity will automatically enable and disable it as needed.")]
		private InputActionAsset m_HandActionAsset;

		[SerializeField]
		[Tooltip("The Transform that contains the Camera. This is usually the \"Head\" of XR Origins. Automatically set to the first enabled camera tagged MainCamera if unset.")]
		private Transform m_CameraTransform;

		[SerializeField]
		[Tooltip("The coordinate space in which keyboard translation should operate.")]
		private XRDeviceSimulator.Space m_KeyboardTranslateSpace;

		[SerializeField]
		[Tooltip("The coordinate space in which mouse translation should operate.")]
		private XRDeviceSimulator.Space m_MouseTranslateSpace = XRDeviceSimulator.Space.Screen;

		[SerializeField]
		[Tooltip("Speed of translation in the x-axis (left/right) when triggered by keyboard input.")]
		private float m_KeyboardXTranslateSpeed = 0.2f;

		[SerializeField]
		[Tooltip("Speed of translation in the y-axis (up/down) when triggered by keyboard input.")]
		private float m_KeyboardYTranslateSpeed = 0.2f;

		[SerializeField]
		[Tooltip("Speed of translation in the z-axis (forward/back) when triggered by keyboard input.")]
		private float m_KeyboardZTranslateSpeed = 0.2f;

		[SerializeField]
		[Tooltip("Speed multiplier applied for body translation when triggered by keyboard input.")]
		private float m_KeyboardBodyTranslateMultiplier = 5f;

		[SerializeField]
		[Tooltip("Sensitivity of translation in the x-axis (left/right) when triggered by mouse input.")]
		private float m_MouseXTranslateSensitivity = 0.0004f;

		[SerializeField]
		[Tooltip("Sensitivity of translation in the y-axis (up/down) when triggered by mouse input.")]
		private float m_MouseYTranslateSensitivity = 0.0004f;

		[SerializeField]
		[Tooltip("Sensitivity of translation in the z-axis (forward/back) when triggered by mouse scroll input.")]
		private float m_MouseScrollTranslateSensitivity = 0.0002f;

		[SerializeField]
		[Tooltip("Sensitivity of rotation along the x-axis (pitch) when triggered by mouse input.")]
		private float m_MouseXRotateSensitivity = 0.2f;

		[SerializeField]
		[Tooltip("Sensitivity of rotation along the y-axis (yaw) when triggered by mouse input.")]
		private float m_MouseYRotateSensitivity = 0.2f;

		[SerializeField]
		[Tooltip("Sensitivity of rotation along the z-axis (roll) when triggered by mouse scroll input.")]
		private float m_MouseScrollRotateSensitivity = 0.05f;

		[SerializeField]
		[Tooltip("A boolean value of whether to invert the y-axis of mouse input when rotating by mouse input.\nA false value (default) means typical FPS style where moving the mouse up/down pitches up/down.\nA true value means flight control style where moving the mouse up/down pitches down/up.")]
		private bool m_MouseYRotateInvert;

		[SerializeField]
		[Tooltip("The desired cursor lock mode to toggle to from None (either Locked or Confined).")]
		private CursorLockMode m_DesiredCursorLockMode = CursorLockMode.Locked;

		[SerializeField]
		[Tooltip("The optional Device Simulator UI prefab to use along with the XR Device Simulator.")]
		private GameObject m_DeviceSimulatorUI;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("The amount of the simulated grip on the controller when the Grip control is pressed.")]
		private float m_GripAmount = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("The amount of the simulated trigger pull on the controller when the Trigger control is pressed.")]
		private float m_TriggerAmount = 1f;

		[SerializeField]
		[Tooltip("Whether the HMD should report the pose as fully tracked or unavailable/inferred.")]
		private bool m_HMDIsTracked = true;

		[SerializeField]
		[Tooltip("Which tracking values the HMD should report as being valid or meaningful to use, which could mean either tracked or inferred.")]
		private InputTrackingState m_HMDTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

		[SerializeField]
		[Tooltip("Whether the left-hand controller should report the pose as fully tracked or unavailable/inferred.")]
		private bool m_LeftControllerIsTracked = true;

		[SerializeField]
		[Tooltip("Which tracking values the left-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.")]
		private InputTrackingState m_LeftControllerTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

		[SerializeField]
		[Tooltip("Whether the right-hand controller should report the pose as fully tracked or unavailable/inferred.")]
		private bool m_RightControllerIsTracked = true;

		[SerializeField]
		[Tooltip("Which tracking values the right-hand controller should report as being valid or meaningful to use, which could mean either tracked or inferred.")]
		private InputTrackingState m_RightControllerTrackingState = InputTrackingState.Position | InputTrackingState.Rotation;

		[SerializeField]
		[Tooltip("Whether the left hand should report the pose as fully tracked or unavailable/inferred.")]
		private bool m_LeftHandIsTracked = true;

		[SerializeField]
		[Tooltip("Whether the right hand should report the pose as fully tracked or unavailable/inferred.")]
		private bool m_RightHandIsTracked = true;

		internal static Action<bool> instanceChanged;

		private XRDeviceSimulator.TargetedDevices m_TargetedDeviceInput = XRDeviceSimulator.TargetedDevices.FPS;

		[TupleElementNames(new string[]
		{
			"transform",
			"camera"
		})]
		private ValueTuple<Transform, Camera> m_CachedCamera;

		private float m_KeyboardXTranslateInput;

		private float m_KeyboardYTranslateInput;

		private float m_KeyboardZTranslateInput;

		private Vector2 m_MouseDeltaInput;

		private Vector2 m_MouseScrollInput;

		private bool m_RotateModeOverrideInput;

		private bool m_XConstraintInput;

		private bool m_YConstraintInput;

		private bool m_ZConstraintInput;

		private bool m_ResetInput;

		private Vector2 m_Axis2DInput;

		private Vector2 m_RestingHandAxis2DInput;

		private bool m_GripInput;

		private bool m_TriggerInput;

		private bool m_PrimaryButtonInput;

		private bool m_SecondaryButtonInput;

		private bool m_MenuInput;

		private bool m_Primary2DAxisClickInput;

		private bool m_Secondary2DAxisClickInput;

		private bool m_Primary2DAxisTouchInput;

		private bool m_Secondary2DAxisTouchInput;

		private bool m_PrimaryTouchInput;

		private bool m_SecondaryTouchInput;

		private bool m_ManipulatedRestingHandAxis2D;

		private Vector3 m_LeftControllerEuler;

		private Vector3 m_RightControllerEuler;

		private Vector3 m_CenterEyeEuler;

		private XRSimulatedHMDState m_HMDState;

		private XRSimulatedControllerState m_LeftControllerState;

		private XRSimulatedControllerState m_RightControllerState;

		private XRSimulatedHandState m_LeftHandState;

		private XRSimulatedHandState m_RightHandState;

		private SimulatedDeviceLifecycleManager m_DeviceLifecycleManager;

		private SimulatedHandExpressionManager m_HandExpressionManager;

		[SerializeField]
		[Obsolete("m_RestingHandExpressionCapture has been deprecated in XRI 3.1.0 and moved to SimulatedHandExpressionManager.")]
		private HandExpressionCapture m_RestingHandExpressionCapture;

		[SerializeField]
		[Tooltip("The list of hand expressions to simulate.")]
		[Obsolete("m_SimulatedHandExpressions has been deprecated in XRI 3.1.0 and moved to SimulatedHandExpressionManager.")]
		private List<XRDeviceSimulator.SimulatedHandExpression> m_SimulatedHandExpressions = new List<XRDeviceSimulator.SimulatedHandExpression>();

		public enum Space
		{
			Local,
			Parent,
			Screen
		}

		public enum TransformationMode
		{
			Translate,
			Rotate
		}

		[Flags]
		internal enum TargetedDevices
		{
			None = 0,
			FPS = 1,
			LeftDevice = 2,
			RightDevice = 4,
			HMD = 8
		}

		[Flags]
		public enum Axis2DTargets
		{
			None = 0,
			Position = 1,
			Primary2DAxis = 2,
			Secondary2DAxis = 4
		}

		[Obsolete("XRDeviceSimulator.SimulatedHandExpression has been deprecated in XRI 3.1.0. Update the XR Device Simulator sample in Package Manager or use the unnested version of SimulatedHandExpression instead.")]
		[Serializable]
		public class SimulatedHandExpression : ISerializationCallbackReceiver
		{
			public string name
			{
				get
				{
					return this.m_ExpressionName.ToString();
				}
			}

			public InputActionReference toggleAction
			{
				get
				{
					return this.m_ToggleAction;
				}
			}

			internal HandExpressionCapture capture
			{
				get
				{
					return this.m_Capture;
				}
				set
				{
					this.m_Capture = value;
				}
			}

			internal HandExpressionName expressionName
			{
				get
				{
					return this.m_ExpressionName;
				}
				set
				{
					this.m_ExpressionName = value;
				}
			}

			public Sprite icon
			{
				get
				{
					return this.m_Capture.icon;
				}
			}

			public event Action<XRDeviceSimulator.SimulatedHandExpression, InputAction.CallbackContext> performed
			{
				add
				{
					this.m_Performed = (Action<XRDeviceSimulator.SimulatedHandExpression, InputAction.CallbackContext>)Delegate.Combine(this.m_Performed, value);
					if (!this.m_Subscribed)
					{
						this.m_Subscribed = true;
						this.m_ToggleAction.action.performed += this.OnActionPerformed;
					}
				}
				remove
				{
					this.m_Performed = (Action<XRDeviceSimulator.SimulatedHandExpression, InputAction.CallbackContext>)Delegate.Remove(this.m_Performed, value);
					if (this.m_Performed == null)
					{
						this.m_Subscribed = false;
						this.m_ToggleAction.action.performed -= this.OnActionPerformed;
					}
				}
			}

			void ISerializationCallbackReceiver.OnBeforeSerialize()
			{
				this.m_Name = this.m_ExpressionName.ToString();
			}

			void ISerializationCallbackReceiver.OnAfterDeserialize()
			{
				this.m_ExpressionName = new HandExpressionName(this.m_Name);
			}

			private void OnActionPerformed(InputAction.CallbackContext context)
			{
				Action<XRDeviceSimulator.SimulatedHandExpression, InputAction.CallbackContext> performed = this.m_Performed;
				if (performed == null)
				{
					return;
				}
				performed(this, context);
			}

			[SerializeField]
			[Tooltip("The unique name for the hand expression.")]
			[Delayed]
			private string m_Name;

			[SerializeField]
			[Tooltip("The input action to trigger the hand expression.")]
			private InputActionReference m_ToggleAction;

			[SerializeField]
			[Tooltip("The captured hand expression to simulate when the input action is performed.")]
			private HandExpressionCapture m_Capture;

			private HandExpressionName m_ExpressionName;

			private Action<XRDeviceSimulator.SimulatedHandExpression, InputAction.CallbackContext> m_Performed;

			private bool m_Subscribed;
		}

		[Obsolete("DeviceMode has been deprecated in XRI 3.1.0 due to being moved out XR Device Simulator. Use DeviceMode in the SimulatedDeviceLifecycleManager instead.")]
		public enum DeviceMode
		{
			Controller,
			Hand
		}
	}
}
