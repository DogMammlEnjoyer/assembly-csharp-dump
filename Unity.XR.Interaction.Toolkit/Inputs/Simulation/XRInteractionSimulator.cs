using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[AddComponentMenu("XR/Debug/XR Interaction Simulator", 11)]
	[DefaultExecutionOrder(-29991)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRInteractionSimulator.html")]
	public class XRInteractionSimulator : MonoBehaviour
	{
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

		public SimulatedDeviceLifecycleManager deviceLifecycleManager
		{
			get
			{
				return this.m_DeviceLifecycleManager;
			}
			set
			{
				this.m_DeviceLifecycleManager = value;
			}
		}

		public SimulatedHandExpressionManager handExpressionManager
		{
			get
			{
				return this.m_HandExpressionManager;
			}
			set
			{
				this.m_HandExpressionManager = value;
			}
		}

		public GameObject interactionSimulatorUI
		{
			get
			{
				return this.m_InteractionSimulatorUI;
			}
			set
			{
				this.m_InteractionSimulatorUI = value;
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

		public XRInputValueReader<float> translateXInput
		{
			get
			{
				return this.m_TranslateXInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<float>(ref this.m_TranslateXInput, value, this);
			}
		}

		public XRInputValueReader<float> translateYInput
		{
			get
			{
				return this.m_TranslateYInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<float>(ref this.m_TranslateYInput, value, this);
			}
		}

		public XRInputValueReader<float> translateZInput
		{
			get
			{
				return this.m_TranslateZInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<float>(ref this.m_TranslateZInput, value, this);
			}
		}

		public XRInputButtonReader toggleManipulateLeftInput
		{
			get
			{
				return this.m_ToggleManipulateLeftInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_ToggleManipulateLeftInput, value, this);
			}
		}

		public XRInputButtonReader toggleManipulateRightInput
		{
			get
			{
				return this.m_ToggleManipulateRightInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_ToggleManipulateRightInput, value, this);
			}
		}

		public XRInputButtonReader leftDeviceActionsInput
		{
			get
			{
				return this.m_LeftDeviceActionsInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_LeftDeviceActionsInput, value, this);
			}
		}

		public XRInputButtonReader cycleDevicesInput
		{
			get
			{
				return this.m_CycleDevicesInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_CycleDevicesInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> keyboardRotationDeltaInput
		{
			get
			{
				return this.m_KeyboardRotationDeltaInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_KeyboardRotationDeltaInput, value, this);
			}
		}

		public XRInputButtonReader toggleMouseInput
		{
			get
			{
				return this.m_ToggleMouseInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_ToggleMouseInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> mouseRotationDeltaInput
		{
			get
			{
				return this.m_MouseRotationDeltaInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_MouseRotationDeltaInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> mouseScrollInput
		{
			get
			{
				return this.m_MouseScrollInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_MouseScrollInput, value, this);
			}
		}

		public XRInputButtonReader gripInput
		{
			get
			{
				return this.m_GripInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_GripInput, value, this);
			}
		}

		public XRInputButtonReader triggerInput
		{
			get
			{
				return this.m_TriggerInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_TriggerInput, value, this);
			}
		}

		public XRInputButtonReader primaryButtonInput
		{
			get
			{
				return this.m_PrimaryButtonInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_PrimaryButtonInput, value, this);
			}
		}

		public XRInputButtonReader secondaryButtonInput
		{
			get
			{
				return this.m_SecondaryButtonInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_SecondaryButtonInput, value, this);
			}
		}

		public XRInputButtonReader menuInput
		{
			get
			{
				return this.m_MenuInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_MenuInput, value, this);
			}
		}

		public XRInputButtonReader primary2DAxisClickInput
		{
			get
			{
				return this.m_Primary2DAxisClickInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_Primary2DAxisClickInput, value, this);
			}
		}

		public XRInputButtonReader secondary2DAxisClickInput
		{
			get
			{
				return this.m_Secondary2DAxisClickInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_Secondary2DAxisClickInput, value, this);
			}
		}

		public XRInputButtonReader primary2DAxisTouchInput
		{
			get
			{
				return this.m_Primary2DAxisTouchInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_Primary2DAxisTouchInput, value, this);
			}
		}

		public XRInputButtonReader secondary2DAxisTouchInput
		{
			get
			{
				return this.m_Secondary2DAxisTouchInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_Secondary2DAxisTouchInput, value, this);
			}
		}

		public XRInputButtonReader primaryTouchInput
		{
			get
			{
				return this.m_PrimaryTouchInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_PrimaryTouchInput, value, this);
			}
		}

		public XRInputButtonReader secondaryTouchInput
		{
			get
			{
				return this.m_SecondaryTouchInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_SecondaryTouchInput, value, this);
			}
		}

		public XRInputButtonReader xConstraintInput
		{
			get
			{
				return this.m_XConstraintInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_XConstraintInput, value, this);
			}
		}

		public XRInputButtonReader yConstraintInput
		{
			get
			{
				return this.m_YConstraintInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_YConstraintInput, value, this);
			}
		}

		public XRInputButtonReader zConstraintInput
		{
			get
			{
				return this.m_ZConstraintInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_ZConstraintInput, value, this);
			}
		}

		public XRInputButtonReader resetInput
		{
			get
			{
				return this.m_ResetInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_ResetInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> axis2DInput
		{
			get
			{
				return this.m_Axis2DInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_Axis2DInput, value, this);
			}
		}

		public XRInputButtonReader togglePrimary2DAxisTargetInput
		{
			get
			{
				return this.m_TogglePrimary2DAxisTargetInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_TogglePrimary2DAxisTargetInput, value, this);
			}
		}

		public XRInputButtonReader toggleSecondary2DAxisTargetInput
		{
			get
			{
				return this.m_ToggleSecondary2DAxisTargetInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_ToggleSecondary2DAxisTargetInput, value, this);
			}
		}

		public XRInputButtonReader cycleQuickActionInput
		{
			get
			{
				return this.m_CycleQuickActionInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_CycleQuickActionInput, value, this);
			}
		}

		public XRInputButtonReader togglePerformQuickActionInput
		{
			get
			{
				return this.m_TogglePerformQuickActionInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_TogglePerformQuickActionInput, value, this);
			}
		}

		public XRInputButtonReader toggleManipulateHeadInput
		{
			get
			{
				return this.m_ToggleManipulateHeadInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_ToggleManipulateHeadInput, value, this);
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

		public float translateXSpeed
		{
			get
			{
				return this.m_TranslateXSpeed;
			}
			set
			{
				this.m_TranslateXSpeed = value;
			}
		}

		public float translateYSpeed
		{
			get
			{
				return this.m_TranslateYSpeed;
			}
			set
			{
				this.m_TranslateYSpeed = value;
			}
		}

		public float translateZSpeed
		{
			get
			{
				return this.m_TranslateZSpeed;
			}
			set
			{
				this.m_TranslateZSpeed = value;
			}
		}

		public float bodyTranslateMultiplier
		{
			get
			{
				return this.m_BodyTranslateMultiplier;
			}
			set
			{
				this.m_BodyTranslateMultiplier = value;
			}
		}

		public float rotateXSensitivity
		{
			get
			{
				return this.m_RotateXSensitivity;
			}
			set
			{
				this.m_RotateXSensitivity = value;
			}
		}

		public float rotateYSensitivity
		{
			get
			{
				return this.m_RotateYSensitivity;
			}
			set
			{
				this.m_RotateYSensitivity = value;
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

		public bool rotateYInvert
		{
			get
			{
				return this.m_RotateYInvert;
			}
			set
			{
				this.m_RotateYInvert = value;
			}
		}

		public Space translateSpace
		{
			get
			{
				return this.m_TranslateSpace;
			}
			set
			{
				this.m_TranslateSpace = value;
			}
		}

		public List<ControllerInputMode> quickActionControllerInputModes
		{
			get
			{
				return this.m_QuickActionControllerInputModes;
			}
			set
			{
				this.m_QuickActionControllerInputModes = value;
			}
		}

		public TargetedDevices targetedDeviceInput
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

		public ControllerInputMode controllerInputMode
		{
			get
			{
				return this.m_ControllerInputMode;
			}
		}

		public SimulatedHandExpression currentHandExpression
		{
			get
			{
				return this.m_CurrentHandExpression;
			}
		}

		public Axis2DTargets axis2DTargets { get; set; } = Axis2DTargets.Primary2DAxis;

		public bool manipulatingLeftDevice
		{
			get
			{
				return this.m_TargetedDeviceInput.HasDevice(TargetedDevices.LeftDevice);
			}
		}

		public bool manipulatingRightDevice
		{
			get
			{
				return this.m_TargetedDeviceInput.HasDevice(TargetedDevices.RightDevice);
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

		public bool manipulatingHMD
		{
			get
			{
				return this.m_TargetedDeviceInput == TargetedDevices.HMD;
			}
		}

		public bool manipulatingFPS
		{
			get
			{
				return this.m_TargetedDeviceInput.HasDevice(TargetedDevices.FPS);
			}
		}

		public static XRInteractionSimulator instance { get; private set; }

		protected virtual void Awake()
		{
			if (XRInteractionSimulator.instance == null)
			{
				XRInteractionSimulator.instance = this;
				Action<bool> action = XRInteractionSimulator.instanceChanged;
				if (action != null)
				{
					action(true);
				}
			}
			else if (XRInteractionSimulator.instance != this)
			{
				Debug.LogWarning(string.Format("Another instance of XR Interaction Simulator already exists ({0}), destroying {1}.", XRInteractionSimulator.instance, base.gameObject), this);
				Object.Destroy(base.gameObject);
				return;
			}
			if (this.m_DeviceLifecycleManager == null)
			{
				this.m_DeviceLifecycleManager = XRSimulatorUtility.FindCreateSimulatedDeviceLifecycleManager(base.gameObject);
			}
			if (this.m_HandExpressionManager == null)
			{
				this.m_HandExpressionManager = XRSimulatorUtility.FindCreateSimulatedHandExpressionManager(base.gameObject);
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
			if (this.m_InteractionSimulatorUI != null)
			{
				Object.Instantiate<GameObject>(this.m_InteractionSimulatorUI, base.transform);
			}
		}

		protected virtual void OnEnable()
		{
			XRSimulatorUtility.FindCameraTransform(ref this.m_CachedCamera, ref this.m_CameraTransform);
			if (this.m_QuickActionControllerInputModes.Count > 0)
			{
				this.m_ControllerInputMode = this.m_QuickActionControllerInputModes[0];
			}
			if (this.m_HandExpressionManager.simulatedHandExpressions.Count > 0)
			{
				this.CycleQuickActionHandExpression();
			}
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void OnDestroy()
		{
			if (XRInteractionSimulator.instance == this)
			{
				Action<bool> action = XRInteractionSimulator.instanceChanged;
				if (action == null)
				{
					return;
				}
				action(false);
			}
		}

		protected virtual void Update()
		{
			this.ReadInputValues();
			this.HandleLeftOrRightDeviceToggle();
			if (this.m_CycleDevicesInput.ReadWasPerformedThisFrame())
			{
				this.CycleTargetDevices();
			}
			if (this.m_CycleQuickActionInput.ReadWasPerformedThisFrame() && !this.manipulatingFPS && !this.manipulatingHMD)
			{
				this.CycleQuickAction();
			}
			if (this.m_ToggleManipulateHeadInput.ReadWasPerformedThisFrame())
			{
				this.HandleHMDToggle();
			}
			if (this.m_TogglePerformQuickActionInput.ReadWasPerformedThisFrame())
			{
				this.PerformQuickAction();
			}
			this.ProcessPoseInput();
			this.ProcessControlInput();
			this.ProcessHandExpressionInput();
			this.m_DeviceLifecycleManager.ApplyHandState(this.m_LeftHandState, this.m_RightHandState);
			this.m_DeviceLifecycleManager.ApplyHMDState(this.m_HMDState);
			this.m_DeviceLifecycleManager.ApplyControllerState(this.m_LeftControllerState, this.m_RightControllerState);
		}

		protected virtual void ProcessPoseInput()
		{
			this.SetTrackedStates();
			if (this.m_TargetedDeviceInput == TargetedDevices.None)
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
			if (this.manipulatingFPS && Time.time > 1f)
			{
				float xTranslateInput = this.m_TranslateXValue * this.m_TranslateXSpeed * this.m_BodyTranslateMultiplier * Time.deltaTime;
				float yTranslateInput = this.m_TranslateYValue * this.m_TranslateYSpeed * this.m_BodyTranslateMultiplier * Time.deltaTime;
				float zTranslateInput = this.m_TranslateZValue * this.m_TranslateZSpeed * this.m_BodyTranslateMultiplier * Time.deltaTime;
				Vector3 translationInDeviceSpace = XRSimulatorUtility.GetTranslationInDeviceSpace(xTranslateInput, yTranslateInput, zTranslateInput, this.m_CameraTransform, quaternion, inverseCameraParentRotation);
				this.m_LeftControllerState.devicePosition = this.m_LeftControllerState.devicePosition + translationInDeviceSpace;
				this.m_RightControllerState.devicePosition = this.m_RightControllerState.devicePosition + translationInDeviceSpace;
				this.m_LeftHandState.position = this.m_LeftHandState.position + translationInDeviceSpace;
				this.m_RightHandState.position = this.m_RightHandState.position + translationInDeviceSpace;
				this.m_HMDState.centerEyePosition = this.m_HMDState.centerEyePosition + translationInDeviceSpace;
				this.m_HMDState.devicePosition = this.m_HMDState.centerEyePosition;
				Vector3 vector = new Vector3(this.m_RotationDeltaValue.x * this.m_RotateXSensitivity, this.m_RotationDeltaValue.y * this.m_RotateYSensitivity * (this.m_RotateYInvert ? 1f : -1f), this.m_MouseScrollValue.y * this.m_MouseScrollRotateSensitivity);
				Vector3 vector2;
				if (this.m_XConstraintValue && !this.m_YConstraintValue && !this.m_ZConstraintValue)
				{
					vector2 = new Vector3(-vector.x + vector.y, 0f, 0f);
				}
				else if (!this.m_XConstraintValue && this.m_YConstraintValue && !this.m_ZConstraintValue)
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
			}
			else if (!this.manipulatingFPS)
			{
				float xTranslateInput2 = this.m_TranslateXValue * this.m_TranslateXSpeed * this.m_BodyTranslateMultiplier * Time.deltaTime;
				float yTranslateInput2 = this.m_TranslateYValue * this.m_TranslateYSpeed * this.m_BodyTranslateMultiplier * Time.deltaTime;
				float zTranslateInput2 = this.m_TranslateZValue * this.m_TranslateZSpeed * this.m_BodyTranslateMultiplier * Time.deltaTime;
				Vector3 translationInDeviceSpace2 = XRSimulatorUtility.GetTranslationInDeviceSpace(xTranslateInput2, yTranslateInput2, zTranslateInput2, this.m_CameraTransform, quaternion, inverseCameraParentRotation);
				Vector3 vector3 = new Vector3(this.m_RotationDeltaValue.x * this.m_RotateXSensitivity, this.m_RotationDeltaValue.y * this.m_RotateYSensitivity * (this.m_RotateYInvert ? 1f : -1f), this.m_MouseScrollValue.y * this.m_MouseScrollRotateSensitivity);
				Vector3 vector4;
				if (this.m_XConstraintValue && !this.m_YConstraintValue && this.m_ZConstraintValue)
				{
					vector4 = new Vector3(vector3.y, 0f, -vector3.x);
				}
				else if (!this.m_XConstraintValue && this.m_YConstraintValue && this.m_ZConstraintValue)
				{
					vector4 = new Vector3(0f, vector3.x, -vector3.y);
				}
				else if (this.m_XConstraintValue && !this.m_YConstraintValue && !this.m_ZConstraintValue)
				{
					vector4 = new Vector3(-vector3.x + vector3.y, 0f, 0f);
				}
				else if (!this.m_XConstraintValue && this.m_YConstraintValue && !this.m_ZConstraintValue)
				{
					vector4 = new Vector3(0f, vector3.x + -vector3.y, 0f);
				}
				else if (!this.m_XConstraintValue && !this.m_YConstraintValue && this.m_ZConstraintValue)
				{
					vector4 = new Vector3(0f, 0f, -vector3.x + -vector3.y);
				}
				else
				{
					vector4 = new Vector3(vector3.y, vector3.x, 0f);
				}
				vector4 += new Vector3(0f, 0f, vector3.z);
				if (this.manipulatingLeftController)
				{
					Quaternion deltaRotation = XRSimulatorUtility.GetDeltaRotation(this.m_TranslateSpace, this.m_LeftControllerState, inverseCameraParentRotation);
					this.m_LeftControllerState.devicePosition = this.m_LeftControllerState.devicePosition + deltaRotation * translationInDeviceSpace2;
					this.m_LeftControllerEuler += vector4;
					this.m_LeftControllerState.deviceRotation = Quaternion.Euler(this.m_LeftControllerEuler);
				}
				if (this.manipulatingRightController)
				{
					Quaternion deltaRotation2 = XRSimulatorUtility.GetDeltaRotation(this.m_TranslateSpace, this.m_RightControllerState, inverseCameraParentRotation);
					this.m_RightControllerState.devicePosition = this.m_RightControllerState.devicePosition + deltaRotation2 * translationInDeviceSpace2;
					this.m_RightControllerEuler += vector4;
					this.m_RightControllerState.deviceRotation = Quaternion.Euler(this.m_RightControllerEuler);
				}
				if (this.manipulatingLeftHand)
				{
					Quaternion deltaRotation3 = XRSimulatorUtility.GetDeltaRotation(this.m_TranslateSpace, this.m_LeftHandState, inverseCameraParentRotation);
					this.m_LeftHandState.position = this.m_LeftHandState.position + deltaRotation3 * translationInDeviceSpace2;
					this.m_LeftHandState.euler = this.m_LeftHandState.euler + vector4;
					this.m_LeftHandState.rotation = Quaternion.Euler(this.m_LeftHandState.euler);
				}
				if (this.manipulatingRightHand)
				{
					Quaternion deltaRotation4 = XRSimulatorUtility.GetDeltaRotation(this.m_TranslateSpace, this.m_RightHandState, inverseCameraParentRotation);
					this.m_RightHandState.position = this.m_RightHandState.position + deltaRotation4 * translationInDeviceSpace2;
					this.m_RightHandState.euler = this.m_RightHandState.euler + vector4;
					this.m_RightHandState.rotation = Quaternion.Euler(this.m_RightHandState.euler);
				}
				if (this.m_TargetedDeviceInput.HasDevice(TargetedDevices.HMD))
				{
					Quaternion deltaRotation5 = XRSimulatorUtility.GetDeltaRotation(this.m_TranslateSpace, this.m_HMDState, inverseCameraParentRotation);
					this.m_HMDState.centerEyePosition = this.m_HMDState.centerEyePosition + deltaRotation5 * translationInDeviceSpace2;
					this.m_HMDState.devicePosition = this.m_HMDState.centerEyePosition;
					this.m_CenterEyeEuler += vector4;
					this.m_HMDState.centerEyeRotation = Quaternion.Euler(this.m_CenterEyeEuler);
					this.m_HMDState.deviceRotation = this.m_HMDState.centerEyeRotation;
				}
			}
			if (this.m_ResetValue)
			{
				Vector3 b = this.m_HMDState.deviceRotation * Vector3.forward * 0.3f;
				Vector3 b2 = this.m_HMDState.deviceRotation * Vector3.down * 0.045f;
				Vector3 b3 = this.m_HMDState.deviceRotation * Vector3.left * 0.1f;
				Vector3 b4 = this.m_HMDState.deviceRotation * Vector3.right * 0.1f;
				this.m_LeftControllerState.devicePosition = this.m_HMDState.devicePosition + b + b2 + b3;
				this.m_RightControllerState.devicePosition = this.m_HMDState.devicePosition + b + b2 + b4;
				this.m_LeftControllerEuler = this.m_HMDState.deviceRotation.eulerAngles;
				this.m_LeftControllerState.deviceRotation = this.m_HMDState.deviceRotation;
				this.m_RightControllerEuler = this.m_HMDState.deviceRotation.eulerAngles;
				this.m_RightControllerState.deviceRotation = this.m_HMDState.deviceRotation;
				this.m_LeftHandState.position = this.m_HMDState.devicePosition + b + b2 + b3;
				this.m_RightHandState.position = this.m_HMDState.devicePosition + b + b2 + b4;
				this.m_LeftHandState.euler = this.m_HMDState.deviceRotation.eulerAngles;
				this.m_LeftHandState.rotation = this.m_HMDState.deviceRotation;
				this.m_RightHandState.euler = this.m_HMDState.deviceRotation.eulerAngles;
				this.m_RightHandState.rotation = this.m_HMDState.deviceRotation;
			}
		}

		protected virtual void ProcessControlInput()
		{
			if (this.m_DeviceLifecycleManager.deviceMode != SimulatedDeviceLifecycleManager.DeviceMode.Controller)
			{
				return;
			}
			if (this.m_LeftDeviceActionsInput.ReadIsPerformed())
			{
				this.ProcessButtonControlInput(ref this.m_LeftControllerState);
				this.ProcessAxis2DControlInput(ref this.m_LeftControllerState);
			}
			else
			{
				this.ProcessButtonControlInput(ref this.m_RightControllerState);
				this.ProcessAxis2DControlInput(ref this.m_RightControllerState);
			}
			if (!this.manipulatingLeftController)
			{
				this.ProcessAnalogButtonControlInput(ref this.m_LeftControllerState);
			}
			if (!this.manipulatingRightController)
			{
				this.ProcessAnalogButtonControlInput(ref this.m_RightControllerState);
			}
		}

		private void ProcessHandExpressionInput()
		{
		}

		private void ToggleHandExpression(SimulatedHandExpression simulatedExpression, bool leftHand, bool rightHand)
		{
		}

		protected virtual void ProcessAxis2DControlInput(ref XRSimulatedControllerState controllerState)
		{
			if ((this.axis2DTargets & Axis2DTargets.Primary2DAxis) != Axis2DTargets.None)
			{
				controllerState.primary2DAxis = this.m_Axis2DValue;
			}
			if ((this.axis2DTargets & Axis2DTargets.Secondary2DAxis) != Axis2DTargets.None)
			{
				controllerState.secondary2DAxis = this.m_Axis2DValue;
			}
		}

		protected virtual void ProcessButtonControlInput(ref XRSimulatedControllerState controllerState)
		{
			if (this.m_GripInput.ReadIsPerformed())
			{
				controllerState.grip = this.m_GripAmount;
				controllerState.WithButton(ControllerButton.GripButton, true);
			}
			else if (this.m_GripInput.ReadWasCompletedThisFrame())
			{
				controllerState.grip = 0f;
				controllerState.WithButton(ControllerButton.GripButton, false);
			}
			if (this.m_TriggerInput.ReadIsPerformed())
			{
				controllerState.trigger = this.m_TriggerAmount;
				controllerState.WithButton(ControllerButton.TriggerButton, true);
			}
			else if (this.m_TriggerInput.ReadWasCompletedThisFrame())
			{
				controllerState.trigger = 0f;
				controllerState.WithButton(ControllerButton.TriggerButton, false);
			}
			if (this.m_PrimaryButtonInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.PrimaryButton, true);
			}
			else if (this.m_PrimaryButtonInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.PrimaryButton, false);
			}
			if (this.m_SecondaryButtonInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.SecondaryButton, true);
			}
			else if (this.m_SecondaryButtonInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.SecondaryButton, false);
			}
			if (this.m_MenuInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.MenuButton, true);
			}
			else if (this.m_MenuInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.MenuButton, false);
			}
			if (this.m_Primary2DAxisClickInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.Primary2DAxisClick, true);
			}
			else if (this.m_Primary2DAxisClickInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.Primary2DAxisClick, false);
			}
			if (this.m_Secondary2DAxisClickInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.Secondary2DAxisClick, true);
			}
			else if (this.m_Secondary2DAxisClickInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.Secondary2DAxisClick, false);
			}
			if (this.m_Primary2DAxisTouchInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.Primary2DAxisTouch, true);
			}
			else if (this.m_Primary2DAxisTouchInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.Primary2DAxisTouch, false);
			}
			if (this.m_Secondary2DAxisTouchInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.Secondary2DAxisTouch, true);
			}
			else if (this.m_Secondary2DAxisTouchInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.Secondary2DAxisTouch, false);
			}
			if (this.m_PrimaryTouchInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.PrimaryTouch, true);
			}
			else if (this.m_PrimaryTouchInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.PrimaryTouch, false);
			}
			if (this.m_SecondaryTouchInput.ReadIsPerformed())
			{
				controllerState.WithButton(ControllerButton.SecondaryTouch, true);
				return;
			}
			if (this.m_SecondaryTouchInput.ReadWasCompletedThisFrame())
			{
				controllerState.WithButton(ControllerButton.SecondaryTouch, false);
			}
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
			if (!this.m_XConstraintValue && !this.m_YConstraintValue && !this.m_ZConstraintValue)
			{
				return Vector3.zero;
			}
			return new Vector3(this.m_XConstraintValue ? 0f : 1f, this.m_YConstraintValue ? 0f : 1f, this.m_ZConstraintValue ? 0f : 1f);
		}

		protected virtual void ReadInputValues()
		{
			this.m_TranslateXValue = this.m_TranslateXInput.ReadValue();
			this.m_TranslateYValue = this.m_TranslateYInput.ReadValue();
			this.m_TranslateZValue = this.m_TranslateZInput.ReadValue();
			this.m_RotationDeltaValue = this.m_KeyboardRotationDeltaInput.ReadValue();
			if (this.m_ToggleMouseInput.ReadIsPerformed())
			{
				Vector2 vector = this.m_MouseRotationDeltaInput.ReadValue();
				if (vector != Vector2.zero)
				{
					this.m_RotationDeltaValue = vector;
				}
				this.m_MouseScrollValue = this.m_MouseScrollInput.ReadValue();
				if (this.m_MouseScrollValue.y != 0f)
				{
					this.m_TranslateZValue = this.m_MouseScrollValue.y;
				}
			}
			this.m_XConstraintValue = this.m_XConstraintInput.ReadIsPerformed();
			this.m_YConstraintValue = this.m_YConstraintInput.ReadIsPerformed();
			this.m_ZConstraintValue = this.m_ZConstraintInput.ReadIsPerformed();
			this.m_ResetValue = this.m_ResetInput.ReadWasPerformedThisFrame();
			this.m_Axis2DValue = Vector2.ClampMagnitude(this.m_Axis2DInput.ReadValue(), 1f);
			if (this.m_TogglePrimary2DAxisTargetInput.ReadWasPerformedThisFrame())
			{
				this.axis2DTargets = Axis2DTargets.Primary2DAxis;
			}
			if (this.m_ToggleSecondary2DAxisTargetInput.ReadWasPerformedThisFrame())
			{
				this.axis2DTargets = Axis2DTargets.Secondary2DAxis;
			}
		}

		private void CycleQuickAction()
		{
			if (this.m_DeviceLifecycleManager.deviceMode != SimulatedDeviceLifecycleManager.DeviceMode.Controller)
			{
				if (this.m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
				{
					this.CycleQuickActionHandExpression();
				}
				return;
			}
			if (this.m_QuickActionControllerInputModes.Count == 0)
			{
				Debug.LogWarning("The key to switch between controller inputs has been pressed, but there doesn't seem to be any inputs set in the quick-action controller input modes.", this);
				return;
			}
			XRInteractionSimulator.ClearControllerButtonInput(ref this.m_LeftControllerState);
			XRInteractionSimulator.ClearControllerButtonInput(ref this.m_RightControllerState);
			this.m_ControllerInputModeIndex = ((this.m_ControllerInputModeIndex < this.m_QuickActionControllerInputModes.Count - 1) ? (this.m_ControllerInputModeIndex + 1) : 0);
			this.m_ControllerInputMode = this.m_QuickActionControllerInputModes[this.m_ControllerInputModeIndex];
		}

		private void PerformQuickAction()
		{
			if (this.m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
			{
				if (this.manipulatingLeftController)
				{
					this.ToggleControllerButtonInput(ref this.m_LeftControllerState);
					return;
				}
				if (this.manipulatingRightController)
				{
					this.ToggleControllerButtonInput(ref this.m_RightControllerState);
					return;
				}
			}
			else if (this.m_DeviceLifecycleManager.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
			{
				this.ToggleHandExpression(this.m_CurrentHandExpression, this.manipulatingLeftHand, this.manipulatingRightHand);
			}
		}

		private void ToggleControllerButtonInput(ref XRSimulatedControllerState controllerState)
		{
			switch (this.m_ControllerInputMode)
			{
			case ControllerInputMode.None:
				break;
			case ControllerInputMode.Trigger:
				controllerState.ToggleButton(ControllerButton.TriggerButton);
				controllerState.trigger = (controllerState.HasButton(ControllerButton.TriggerButton) ? this.m_TriggerAmount : 0f);
				return;
			case ControllerInputMode.Grip:
				controllerState.ToggleButton(ControllerButton.GripButton);
				controllerState.grip = (controllerState.HasButton(ControllerButton.GripButton) ? this.m_GripAmount : 0f);
				return;
			case ControllerInputMode.PrimaryButton:
				controllerState.ToggleButton(ControllerButton.PrimaryButton);
				return;
			case ControllerInputMode.SecondaryButton:
				controllerState.ToggleButton(ControllerButton.SecondaryButton);
				return;
			case ControllerInputMode.Menu:
				controllerState.ToggleButton(ControllerButton.MenuButton);
				return;
			case ControllerInputMode.Primary2DAxisClick:
				controllerState.ToggleButton(ControllerButton.Primary2DAxisClick);
				return;
			case ControllerInputMode.Secondary2DAxisClick:
				controllerState.ToggleButton(ControllerButton.Secondary2DAxisClick);
				return;
			case ControllerInputMode.Primary2DAxisTouch:
				controllerState.ToggleButton(ControllerButton.Primary2DAxisTouch);
				return;
			case ControllerInputMode.Secondary2DAxisTouch:
				controllerState.ToggleButton(ControllerButton.Secondary2DAxisTouch);
				return;
			case ControllerInputMode.PrimaryTouch:
				controllerState.ToggleButton(ControllerButton.PrimaryTouch);
				return;
			case ControllerInputMode.SecondaryTouch:
				controllerState.ToggleButton(ControllerButton.SecondaryTouch);
				break;
			default:
				return;
			}
		}

		private static void ClearControllerButtonInput(ref XRSimulatedControllerState controllerState)
		{
			controllerState.trigger = 0f;
			controllerState.grip = 0f;
			controllerState.buttons = 0;
		}

		private void SetTrackedStates()
		{
			this.m_LeftControllerState.isTracked = this.m_LeftControllerIsTracked;
			this.m_RightControllerState.isTracked = this.m_RightControllerIsTracked;
			this.m_LeftHandState.isTracked = this.m_LeftHandIsTracked;
			this.m_RightHandState.isTracked = this.m_RightHandIsTracked;
			this.m_HMDState.isTracked = this.m_HMDIsTracked;
			this.m_LeftControllerState.trackingState = (int)this.m_LeftControllerTrackingState;
			this.m_RightControllerState.trackingState = (int)this.m_RightControllerTrackingState;
			this.m_HMDState.trackingState = (int)this.m_HMDTrackingState;
		}

		private void CycleTargetDevices()
		{
			if (this.targetedDeviceInput.HasDevice(TargetedDevices.HMD))
			{
				this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);
			}
			if (this.targetedDeviceInput == TargetedDevices.None)
			{
				this.targetedDeviceInput = TargetedDevices.FPS;
				return;
			}
			if (this.targetedDeviceInput.HasDevice(TargetedDevices.FPS))
			{
				this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(TargetedDevices.FPS);
				if (!this.targetedDeviceInput.HasDevice(TargetedDevices.LeftDevice) && !this.targetedDeviceInput.HasDevice(TargetedDevices.RightDevice))
				{
					this.targetedDeviceInput = (TargetedDevices.LeftDevice | TargetedDevices.RightDevice);
					return;
				}
			}
			else if (this.targetedDeviceInput.HasDevice(TargetedDevices.LeftDevice) || this.targetedDeviceInput.HasDevice(TargetedDevices.RightDevice))
			{
				this.targetedDeviceInput = this.targetedDeviceInput.WithDevice(TargetedDevices.FPS);
			}
		}

		private void HandleLeftOrRightDeviceToggle()
		{
			if (this.m_ToggleManipulateWaitingForReleaseBoth)
			{
				this.m_ToggleManipulateWaitingForReleaseBoth = (this.m_ToggleManipulateLeftInput.ReadIsPerformed() || this.m_ToggleManipulateRightInput.ReadIsPerformed());
				return;
			}
			if (this.m_ToggleManipulateLeftInput.ReadIsPerformed() && this.m_ToggleManipulateRightInput.ReadIsPerformed())
			{
				if (this.targetedDeviceInput.HasDevice(TargetedDevices.HMD))
				{
					this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);
				}
				this.m_ToggleManipulateWaitingForReleaseBoth = true;
				if (this.targetedDeviceInput == (TargetedDevices.LeftDevice | TargetedDevices.RightDevice))
				{
					this.m_DeviceLifecycleManager.SwitchDeviceMode();
					return;
				}
				this.targetedDeviceInput = this.targetedDeviceInput.WithDevice(TargetedDevices.LeftDevice).WithDevice(TargetedDevices.RightDevice).WithoutDevice(TargetedDevices.FPS);
				return;
			}
			else
			{
				if (!this.m_ToggleManipulateLeftInput.ReadWasCompletedThisFrame())
				{
					if (this.m_ToggleManipulateRightInput.ReadWasCompletedThisFrame())
					{
						if (this.targetedDeviceInput.HasDevice(TargetedDevices.HMD))
						{
							this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);
						}
						if (this.targetedDeviceInput == TargetedDevices.RightDevice)
						{
							this.m_DeviceLifecycleManager.SwitchDeviceMode();
							return;
						}
						this.targetedDeviceInput = this.targetedDeviceInput.WithDevice(TargetedDevices.RightDevice).WithoutDevice(TargetedDevices.LeftDevice).WithoutDevice(TargetedDevices.FPS);
					}
					return;
				}
				if (this.targetedDeviceInput.HasDevice(TargetedDevices.HMD))
				{
					this.targetedDeviceInput = this.targetedDeviceInput.WithoutDevice(TargetedDevices.HMD);
				}
				if (this.targetedDeviceInput == TargetedDevices.LeftDevice)
				{
					this.m_DeviceLifecycleManager.SwitchDeviceMode();
					return;
				}
				this.targetedDeviceInput = this.targetedDeviceInput.WithDevice(TargetedDevices.LeftDevice).WithoutDevice(TargetedDevices.RightDevice).WithoutDevice(TargetedDevices.FPS);
				return;
			}
		}

		private void HandleHMDToggle()
		{
			if (this.targetedDeviceInput != TargetedDevices.HMD)
			{
				this.m_PreviousTargetedDevices = this.targetedDeviceInput;
				this.targetedDeviceInput = TargetedDevices.HMD;
				return;
			}
			this.targetedDeviceInput = this.m_PreviousTargetedDevices;
		}

		private void CycleQuickActionHandExpression()
		{
			List<SimulatedHandExpression> simulatedHandExpressions = this.m_HandExpressionManager.simulatedHandExpressions;
			for (int i = 0; i < simulatedHandExpressions.Count; i++)
			{
				this.m_HandExpressionIndex = ((this.m_HandExpressionIndex < simulatedHandExpressions.Count - 1) ? (this.m_HandExpressionIndex + 1) : 0);
				if (simulatedHandExpressions[this.m_HandExpressionIndex].isQuickAction)
				{
					this.m_CurrentHandExpression = simulatedHandExpressions[this.m_HandExpressionIndex];
					return;
				}
			}
			this.m_HandExpressionIndex = -1;
			Debug.LogWarning("The key to switch between hand expressions has been pressed, but there doesn't seem to be any expressions set to quick-access in the Simulated Hand Expression Manager.", this);
		}

		private const float k_DeviceLeftRightOffsetAmount = 0.1f;

		private const float k_DeviceForwardOffsetAmount = 0.3f;

		private const float k_DeviceDownOffsetAmount = 0.045f;

		[SerializeField]
		[Tooltip("The Transform that contains the Camera. This is usually the \"Head\" of XR Origins. Automatically set to the first enabled camera tagged MainCamera if unset.")]
		private Transform m_CameraTransform;

		[SerializeField]
		[Tooltip("The corresponding manager for this simulator that handles the lifecycle of the simulated devices.")]
		private SimulatedDeviceLifecycleManager m_DeviceLifecycleManager;

		[SerializeField]
		[Tooltip("The corresponding manager for this simulator that handles the hand expressions.")]
		private SimulatedHandExpressionManager m_HandExpressionManager;

		[SerializeField]
		[Tooltip("The optional Interaction Simulator UI prefab to use along with the XR Interaction Simulator.")]
		private GameObject m_InteractionSimulatorUI;

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

		[SerializeField]
		[Tooltip("The input used to translate in the x-axis (left/right) while held.")]
		private XRInputValueReader<float> m_TranslateXInput = new XRInputValueReader<float>("Translate X Input", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to translate in the y-axis (up/down) while held.")]
		private XRInputValueReader<float> m_TranslateYInput = new XRInputValueReader<float>("Translate Y Input", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to translate in the z-axis (forward/back) while held.")]
		private XRInputValueReader<float> m_TranslateZInput = new XRInputValueReader<float>("Translate Z Input", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to toggle enable manipulation of the left-hand controller when pressed.")]
		private XRInputButtonReader m_ToggleManipulateLeftInput;

		[SerializeField]
		[Tooltip("The input used to toggle enable manipulation of the right-hand controller when pressed")]
		private XRInputButtonReader m_ToggleManipulateRightInput;

		[SerializeField]
		[Tooltip("The input used for controlling the left-hand device's actions for buttons or hand expressions.")]
		private XRInputButtonReader m_LeftDeviceActionsInput;

		[SerializeField]
		[Tooltip("The input used to cycle between the different available devices.")]
		private XRInputButtonReader m_CycleDevicesInput;

		[SerializeField]
		[Tooltip("The keyboard input used to rotate by a scaled amount along or about the x- and y-axes.")]
		private XRInputValueReader<Vector2> m_KeyboardRotationDeltaInput = new XRInputValueReader<Vector2>("Keyboard Rotation Delta Input", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to toggle associated inputs from a mouse device.")]
		private XRInputButtonReader m_ToggleMouseInput;

		[SerializeField]
		[Tooltip("The mouse input used to rotate by a scaled amount along or about the x- and y-axes.")]
		private XRInputValueReader<Vector2> m_MouseRotationDeltaInput = new XRInputValueReader<Vector2>("Mouse Rotation Delta Input", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("The input used to translate or rotate by a scaled amount along or about the z-axis.")]
		private XRInputValueReader<Vector2> m_MouseScrollInput;

		[SerializeField]
		[Tooltip("The input used to control the Grip control of the manipulated controller device(s).")]
		private XRInputButtonReader m_GripInput;

		[SerializeField]
		[Tooltip("The input used to control the Trigger control of the manipulated controller device(s).")]
		private XRInputButtonReader m_TriggerInput;

		[SerializeField]
		[Tooltip("The input used to control the PrimaryButton control of the manipulated controller device(s).")]
		private XRInputButtonReader m_PrimaryButtonInput;

		[SerializeField]
		[Tooltip("The input used to control the SecondaryButton control of the manipulated controller device(s).")]
		private XRInputButtonReader m_SecondaryButtonInput;

		[SerializeField]
		[Tooltip("The input used to control the Menu control of the manipulated controller device(s).")]
		private XRInputButtonReader m_MenuInput;

		[SerializeField]
		[Tooltip("The input used to control the Primary2DAxisClick control of the manipulated controller device(s).")]
		private XRInputButtonReader m_Primary2DAxisClickInput;

		[SerializeField]
		[Tooltip("The input used to control the Secondary2DAxisClick control of the manipulated controller device(s).")]
		private XRInputButtonReader m_Secondary2DAxisClickInput;

		[SerializeField]
		[Tooltip("The input used to control the Primary2DAxisTouch control of the manipulated controller device(s).")]
		private XRInputButtonReader m_Primary2DAxisTouchInput;

		[SerializeField]
		[Tooltip("The input used to control the Secondary2DAxisTouch control of the manipulated controller device(s).")]
		private XRInputButtonReader m_Secondary2DAxisTouchInput;

		[SerializeField]
		[Tooltip("The input used to control the PrimaryTouch control of the manipulated controller device(s).")]
		private XRInputButtonReader m_PrimaryTouchInput;

		[SerializeField]
		[Tooltip("The input used to control the SecondaryTouch control of the manipulated controller device(s).")]
		private XRInputButtonReader m_SecondaryTouchInput;

		[SerializeField]
		[Tooltip("The input used to constrain the translation or rotation to the x-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane.")]
		private XRInputButtonReader m_XConstraintInput;

		[SerializeField]
		[Tooltip("The input used to constrain the translation or rotation to the y-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane.")]
		private XRInputButtonReader m_YConstraintInput;

		[SerializeField]
		[Tooltip("The input used to constrain the translation or rotation to the z-axis when moving the mouse or resetting. May be combined with another axis constraint to constrain to a plane.")]
		private XRInputButtonReader m_ZConstraintInput;

		[SerializeField]
		[Tooltip("The input used to cause the manipulated device(s) to reset position or rotation (depending on the effective manipulation mode).")]
		private XRInputButtonReader m_ResetInput;

		[SerializeField]
		[Tooltip("The input used to control the value of one or more 2D Axis controls on the manipulated controller device(s).")]
		private XRInputValueReader<Vector2> m_Axis2DInput;

		[SerializeField]
		[Tooltip("The input used to toggle enable manipulation of the Primary2DAxis of the controllers when pressed.")]
		private XRInputButtonReader m_TogglePrimary2DAxisTargetInput;

		[SerializeField]
		[Tooltip("The input used to toggle enable manipulation of the Secondary2DAxis of the controllers when pressed.")]
		private XRInputButtonReader m_ToggleSecondary2DAxisTargetInput;

		[SerializeField]
		[Tooltip("The input used to cycle the quick-action for controller inputs or hand expressions.")]
		private XRInputButtonReader m_CycleQuickActionInput;

		[SerializeField]
		[Tooltip("The input used to perform the currently active quick-action controller input or hand expression.")]
		private XRInputButtonReader m_TogglePerformQuickActionInput;

		[SerializeField]
		[Tooltip("The input used to toggle manipulation of only the head pose.")]
		private XRInputButtonReader m_ToggleManipulateHeadInput;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("The amount of the simulated grip on the controller when the Grip control is pressed.")]
		private float m_GripAmount = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("The amount of the simulated trigger pull on the controller when the Trigger control is pressed.")]
		private float m_TriggerAmount = 1f;

		[SerializeField]
		[Tooltip("Speed of translation in the x-axis (left/right) when triggered by input.")]
		private float m_TranslateXSpeed = 0.2f;

		[SerializeField]
		[Tooltip("Speed of translation in the y-axis (up/down) when triggered by input.")]
		private float m_TranslateYSpeed = 0.2f;

		[SerializeField]
		[Tooltip("Speed of translation in the z-axis (forward/back) when triggered by input.")]
		private float m_TranslateZSpeed = 0.2f;

		[SerializeField]
		[Tooltip("Speed multiplier applied for body translation when triggered by input.")]
		private float m_BodyTranslateMultiplier = 5f;

		[SerializeField]
		[Tooltip("Sensitivity of rotation along the x-axis (pitch) when triggered by input.")]
		private float m_RotateXSensitivity = 0.2f;

		[SerializeField]
		[Tooltip("Sensitivity of rotation along the y-axis (yaw) when triggered by input.")]
		private float m_RotateYSensitivity = 0.2f;

		[SerializeField]
		[Tooltip("Sensitivity of rotation along the z-axis (roll) when triggered by mouse scroll input.")]
		private float m_MouseScrollRotateSensitivity = 0.05f;

		[SerializeField]
		[Tooltip("A boolean value of whether to invert the y-axis when rotating.\nA false value (default) means typical FPS style where moving up/down pitches up/down.\nA true value means flight control style where moving up/down pitches down/up.")]
		private bool m_RotateYInvert;

		[SerializeField]
		[Tooltip("The coordinate space in which translation should operate.")]
		private Space m_TranslateSpace = Space.Screen;

		[SerializeField]
		[Tooltip("The subset of quick-action controller buttons/inputs that a user can shift through in the simulator.")]
		private List<ControllerInputMode> m_QuickActionControllerInputModes = new List<ControllerInputMode>();

		private TargetedDevices m_TargetedDeviceInput = TargetedDevices.FPS;

		private ControllerInputMode m_ControllerInputMode = ControllerInputMode.Trigger;

		private SimulatedHandExpression m_CurrentHandExpression = new SimulatedHandExpression();

		internal static Action<bool> instanceChanged;

		[TupleElementNames(new string[]
		{
			"transform",
			"camera"
		})]
		private ValueTuple<Transform, Camera> m_CachedCamera;

		private float m_TranslateXValue;

		private float m_TranslateYValue;

		private float m_TranslateZValue;

		private Vector2 m_RotationDeltaValue;

		private Vector2 m_MouseScrollValue;

		private bool m_XConstraintValue;

		private bool m_YConstraintValue;

		private bool m_ZConstraintValue;

		private bool m_ResetValue;

		private Vector2 m_Axis2DValue;

		private int m_ControllerInputModeIndex;

		private int m_HandExpressionIndex = -1;

		private bool m_ToggleManipulateWaitingForReleaseBoth;

		private Vector3 m_LeftControllerEuler;

		private Vector3 m_RightControllerEuler;

		private Vector3 m_CenterEyeEuler;

		private XRSimulatedHMDState m_HMDState;

		private XRSimulatedControllerState m_LeftControllerState;

		private XRSimulatedControllerState m_RightControllerState;

		private XRSimulatedHandState m_LeftHandState;

		private XRSimulatedHandState m_RightHandState;

		private TargetedDevices m_PreviousTargetedDevices;
	}
}
