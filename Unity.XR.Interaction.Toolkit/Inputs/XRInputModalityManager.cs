using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
	[AddComponentMenu("XR/XR Input Modality Manager", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.XRInputModalityManager.html")]
	public class XRInputModalityManager : MonoBehaviour
	{
		public GameObject leftHand
		{
			get
			{
				return this.m_LeftHand;
			}
			set
			{
				this.m_LeftHand = value;
			}
		}

		public GameObject rightHand
		{
			get
			{
				return this.m_RightHand;
			}
			set
			{
				this.m_RightHand = value;
			}
		}

		public GameObject leftController
		{
			get
			{
				return this.m_LeftController;
			}
			set
			{
				this.m_LeftController = value;
			}
		}

		public GameObject rightController
		{
			get
			{
				return this.m_RightController;
			}
			set
			{
				this.m_RightController = value;
			}
		}

		public UnityEvent trackedHandModeStarted
		{
			get
			{
				return this.m_TrackedHandModeStarted;
			}
			set
			{
				this.m_TrackedHandModeStarted = value;
			}
		}

		public UnityEvent trackedHandModeEnded
		{
			get
			{
				return this.m_TrackedHandModeEnded;
			}
			set
			{
				this.m_TrackedHandModeEnded = value;
			}
		}

		public UnityEvent motionControllerModeStarted
		{
			get
			{
				return this.m_MotionControllerModeStarted;
			}
			set
			{
				this.m_MotionControllerModeStarted = value;
			}
		}

		public UnityEvent motionControllerModeEnded
		{
			get
			{
				return this.m_MotionControllerModeEnded;
			}
			set
			{
				this.m_MotionControllerModeEnded = value;
			}
		}

		public static IReadOnlyBindableVariable<XRInputModalityManager.InputMode> currentInputMode
		{
			get
			{
				return XRInputModalityManager.s_CurrentInputMode;
			}
		}

		internal XRInputModalityManager.InputMode leftInputMode
		{
			get
			{
				return this.m_LeftInputMode;
			}
		}

		internal XRInputModalityManager.InputMode rightInputMode
		{
			get
			{
				return this.m_RightInputMode;
			}
		}

		internal event Action<XRInputModalityManager, XRInputModalityManager.InputMode> leftInputModeChanged;

		internal event Action<XRInputModalityManager, XRInputModalityManager.InputMode> rightInputModeChanged;

		internal static List<XRInputModalityManager> activeModalityManagers { get; } = new List<XRInputModalityManager>();

		internal static event Action<XRInputModalityManager, bool> activeModalityManagersChanged;

		protected void OnEnable()
		{
			if (this.m_LeftHand != null || this.m_RightHand != null)
			{
				Debug.LogWarning("Script requires XR Hands (com.unity.xr.hands) package to switch to hand tracking groups. Install using Window > Package Manager or click Fix on the related issue in Edit > Project Settings > XR Plug-in Management > Project Validation.", this);
			}
			this.SubscribeHandSubsystem();
			InputSystem.onDeviceChange += this.OnDeviceChange;
			InputDevices.deviceConnected += this.OnDeviceConnected;
			InputDevices.deviceDisconnected += this.OnDeviceDisconnected;
			InputDevices.deviceConfigChanged += this.OnDeviceConfigChanged;
			this.m_TrackedDeviceMonitor.trackingAcquired += this.OnControllerTrackingAcquired;
			this.m_InputDeviceMonitor.trackingAcquired += this.OnControllerTrackingAcquired;
			this.UpdateLeftMode();
			this.UpdateRightMode();
			XRInputModalityManager.activeModalityManagers.Add(this);
			Action<XRInputModalityManager, bool> action = XRInputModalityManager.activeModalityManagersChanged;
			if (action == null)
			{
				return;
			}
			action(this, true);
		}

		protected void OnDisable()
		{
			this.UnsubscribeHandSubsystem();
			InputSystem.onDeviceChange -= this.OnDeviceChange;
			InputDevices.deviceConnected -= this.OnDeviceConnected;
			InputDevices.deviceDisconnected -= this.OnDeviceDisconnected;
			InputDevices.deviceConfigChanged -= this.OnDeviceConfigChanged;
			if (this.m_TrackedDeviceMonitor != null)
			{
				this.m_TrackedDeviceMonitor.trackingAcquired -= this.OnControllerTrackingAcquired;
				this.m_TrackedDeviceMonitor.ClearAllDevices();
			}
			if (this.m_InputDeviceMonitor != null)
			{
				this.m_InputDeviceMonitor.trackingAcquired -= this.OnControllerTrackingAcquired;
				this.m_InputDeviceMonitor.ClearAllDevices();
			}
			XRInputModalityManager.activeModalityManagers.Remove(this);
			Action<XRInputModalityManager, bool> action = XRInputModalityManager.activeModalityManagersChanged;
			if (action == null)
			{
				return;
			}
			action(this, false);
		}

		protected void Update()
		{
		}

		private void SubscribeHandSubsystem()
		{
		}

		private void UnsubscribeHandSubsystem()
		{
		}

		private void LogMissingHandSubsystem()
		{
		}

		private void SetLeftMode(XRInputModalityManager.InputMode inputMode)
		{
			XRInputModalityManager.SafeSetActive(this.m_LeftHand, inputMode == XRInputModalityManager.InputMode.TrackedHand);
			XRInputModalityManager.SafeSetActive(this.m_LeftController, inputMode == XRInputModalityManager.InputMode.MotionController);
			XRInputModalityManager.InputMode leftInputMode = this.m_LeftInputMode;
			this.m_LeftInputMode = inputMode;
			if (leftInputMode != inputMode)
			{
				this.OnModeChanged(leftInputMode, inputMode, this.m_RightInputMode);
				Action<XRInputModalityManager, XRInputModalityManager.InputMode> action = this.leftInputModeChanged;
				if (action == null)
				{
					return;
				}
				action(this, inputMode);
			}
		}

		private void SetRightMode(XRInputModalityManager.InputMode inputMode)
		{
			XRInputModalityManager.SafeSetActive(this.m_RightHand, inputMode == XRInputModalityManager.InputMode.TrackedHand);
			XRInputModalityManager.SafeSetActive(this.m_RightController, inputMode == XRInputModalityManager.InputMode.MotionController);
			XRInputModalityManager.InputMode rightInputMode = this.m_RightInputMode;
			this.m_RightInputMode = inputMode;
			if (rightInputMode != inputMode)
			{
				this.OnModeChanged(rightInputMode, inputMode, this.m_LeftInputMode);
				Action<XRInputModalityManager, XRInputModalityManager.InputMode> action = this.rightInputModeChanged;
				if (action == null)
				{
					return;
				}
				action(this, inputMode);
			}
		}

		private void OnModeChanged(XRInputModalityManager.InputMode oldInputMode, XRInputModalityManager.InputMode newInputMode, XRInputModalityManager.InputMode otherHandInputMode)
		{
			if (otherHandInputMode != XRInputModalityManager.InputMode.TrackedHand && oldInputMode == XRInputModalityManager.InputMode.TrackedHand)
			{
				UnityEvent trackedHandModeEnded = this.m_TrackedHandModeEnded;
				if (trackedHandModeEnded != null)
				{
					trackedHandModeEnded.Invoke();
				}
			}
			else if (otherHandInputMode != XRInputModalityManager.InputMode.MotionController && oldInputMode == XRInputModalityManager.InputMode.MotionController)
			{
				UnityEvent motionControllerModeEnded = this.m_MotionControllerModeEnded;
				if (motionControllerModeEnded != null)
				{
					motionControllerModeEnded.Invoke();
				}
			}
			if (otherHandInputMode != XRInputModalityManager.InputMode.TrackedHand && newInputMode == XRInputModalityManager.InputMode.TrackedHand)
			{
				UnityEvent trackedHandModeStarted = this.m_TrackedHandModeStarted;
				if (trackedHandModeStarted != null)
				{
					trackedHandModeStarted.Invoke();
				}
			}
			else if (otherHandInputMode != XRInputModalityManager.InputMode.MotionController && newInputMode == XRInputModalityManager.InputMode.MotionController)
			{
				UnityEvent motionControllerModeStarted = this.m_MotionControllerModeStarted;
				if (motionControllerModeStarted != null)
				{
					motionControllerModeStarted.Invoke();
				}
			}
			XRInputModalityManager.s_CurrentInputMode.Value = newInputMode;
		}

		private static void SafeSetActive(GameObject gameObject, bool active)
		{
			if (gameObject != null && gameObject.activeSelf != active)
			{
				gameObject.SetActive(active);
			}
		}

		private bool GetLeftHandIsTracked()
		{
			return false;
		}

		private bool GetRightHandIsTracked()
		{
			return false;
		}

		private void UpdateLeftMode()
		{
			XRController controllerDevice;
			if (XRInputModalityManager.TryGetControllerDevice(CommonUsages.LeftHand, out controllerDevice))
			{
				this.UpdateLeftMode(controllerDevice);
				return;
			}
			InputDevice controllerDevice2;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.leftController, out controllerDevice2))
			{
				this.UpdateMode(controllerDevice2, new Action<XRInputModalityManager.InputMode>(this.SetLeftMode));
				return;
			}
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.leftHandInteraction, out controllerDevice2) || XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.leftMicrosoftHandInteraction, out controllerDevice2))
			{
				if (this.GetLeftHandIsTracked())
				{
					this.SetLeftMode(XRInputModalityManager.InputMode.TrackedHand);
				}
				else
				{
					this.UpdateMode(controllerDevice2, new Action<XRInputModalityManager.InputMode>(this.SetLeftMode));
				}
			}
			XRInputModalityManager.InputMode leftMode = this.GetLeftHandIsTracked() ? XRInputModalityManager.InputMode.TrackedHand : XRInputModalityManager.InputMode.None;
			this.SetLeftMode(leftMode);
		}

		private void UpdateRightMode()
		{
			XRController controllerDevice;
			if (XRInputModalityManager.TryGetControllerDevice(CommonUsages.RightHand, out controllerDevice))
			{
				this.UpdateRightMode(controllerDevice);
				return;
			}
			InputDevice controllerDevice2;
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.rightController, out controllerDevice2))
			{
				this.UpdateMode(controllerDevice2, new Action<XRInputModalityManager.InputMode>(this.SetRightMode));
				return;
			}
			if (XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.rightHandInteraction, out controllerDevice2) || XRInputTrackingAggregator.TryGetDeviceWithExactCharacteristics(XRInputTrackingAggregator.Characteristics.rightMicrosoftHandInteraction, out controllerDevice2))
			{
				if (this.GetRightHandIsTracked())
				{
					this.SetRightMode(XRInputModalityManager.InputMode.TrackedHand);
				}
				else
				{
					this.UpdateMode(controllerDevice2, new Action<XRInputModalityManager.InputMode>(this.SetRightMode));
				}
			}
			XRInputModalityManager.InputMode rightMode = this.GetRightHandIsTracked() ? XRInputModalityManager.InputMode.TrackedHand : XRInputModalityManager.InputMode.None;
			this.SetRightMode(rightMode);
		}

		private void UpdateLeftMode(XRController controllerDevice)
		{
			if (!XRInputModalityManager.IsHandInteractionXRControllerType(controllerDevice))
			{
				this.UpdateMode(controllerDevice, new Action<XRInputModalityManager.InputMode>(this.SetLeftMode));
				return;
			}
			if (this.GetLeftHandIsTracked())
			{
				this.SetLeftMode(XRInputModalityManager.InputMode.TrackedHand);
				return;
			}
			this.UpdateMode(controllerDevice, new Action<XRInputModalityManager.InputMode>(this.SetLeftMode));
		}

		private void UpdateRightMode(XRController controllerDevice)
		{
			if (!XRInputModalityManager.IsHandInteractionXRControllerType(controllerDevice))
			{
				this.UpdateMode(controllerDevice, new Action<XRInputModalityManager.InputMode>(this.SetRightMode));
				return;
			}
			if (this.GetRightHandIsTracked())
			{
				this.SetRightMode(XRInputModalityManager.InputMode.TrackedHand);
				return;
			}
			this.UpdateMode(controllerDevice, new Action<XRInputModalityManager.InputMode>(this.SetRightMode));
		}

		private void UpdateMode(XRController controllerDevice, Action<XRInputModalityManager.InputMode> setModeMethod)
		{
			if (controllerDevice == null)
			{
				setModeMethod(XRInputModalityManager.InputMode.None);
				return;
			}
			if (XRInputModalityManager.IsTracked(controllerDevice))
			{
				setModeMethod(XRInputModalityManager.InputMode.MotionController);
				return;
			}
			setModeMethod(XRInputModalityManager.InputMode.None);
			this.m_TrackedDeviceMonitor.AddDevice(controllerDevice);
		}

		private void UpdateMode(InputDevice controllerDevice, Action<XRInputModalityManager.InputMode> setModeMethod)
		{
			if (!controllerDevice.isValid)
			{
				setModeMethod(XRInputModalityManager.InputMode.None);
				return;
			}
			if (XRInputModalityManager.IsTracked(controllerDevice))
			{
				setModeMethod(XRInputModalityManager.InputMode.MotionController);
				return;
			}
			setModeMethod(XRInputModalityManager.InputMode.None);
			this.m_InputDeviceMonitor.AddDevice(controllerDevice);
		}

		private static bool TryGetControllerDevice(InternedString usage, out XRController controllerDevice)
		{
			controllerDevice = null;
			double num = -1.0;
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			for (int i = 0; i < devices.Count; i++)
			{
				XRController xrcontroller = devices[i] as XRController;
				if (xrcontroller != null && !XRInputModalityManager.ShouldIgnoreXRControllerType(xrcontroller) && xrcontroller.usages.Contains(usage) && (controllerDevice == null || xrcontroller.lastUpdateTime > num))
				{
					controllerDevice = xrcontroller;
					num = xrcontroller.lastUpdateTime;
				}
			}
			return controllerDevice != null;
		}

		private static bool ShouldIgnoreXRControllerType(XRController device)
		{
			return device is DPadInteraction.DPad || device is PalmPoseInteraction.PalmPose;
		}

		private static bool IsHandInteractionXRControllerType(XRController device)
		{
			return device is HandInteractionProfile.HandInteraction || device is MicrosoftHandInteraction.HoloLensHand;
		}

		private unsafe static bool IsTracked(TrackedDevice device)
		{
			return device.isTracked.isPressed || (*device.trackingState.value & 3) == 3;
		}

		private static bool IsTracked(InputDevice device)
		{
			bool flag;
			InputTrackingState inputTrackingState;
			return (device.TryGetFeatureValue(CommonUsages.isTracked, out flag) && flag) || (device.TryGetFeatureValue(CommonUsages.trackingState, out inputTrackingState) && (inputTrackingState & (InputTrackingState.Position | InputTrackingState.Rotation)) == (InputTrackingState.Position | InputTrackingState.Rotation));
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			XRController xrcontroller = device as XRController;
			if (xrcontroller == null)
			{
				return;
			}
			if (XRInputModalityManager.ShouldIgnoreXRControllerType(xrcontroller))
			{
				return;
			}
			if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected || change == InputDeviceChange.Enabled || change == InputDeviceChange.UsageChanged)
			{
				if (!device.added)
				{
					return;
				}
				ReadOnlyArray<InternedString> usages = device.usages;
				if (usages.Contains(CommonUsages.LeftHand))
				{
					this.UpdateLeftMode(xrcontroller);
					return;
				}
				if (usages.Contains(CommonUsages.RightHand))
				{
					this.UpdateRightMode(xrcontroller);
					return;
				}
			}
			else if (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled)
			{
				this.m_TrackedDeviceMonitor.RemoveDevice(xrcontroller);
				ReadOnlyArray<InternedString> usages2 = device.usages;
				if (usages2.Contains(CommonUsages.LeftHand))
				{
					XRInputModalityManager.InputMode leftMode = this.GetLeftHandIsTracked() ? XRInputModalityManager.InputMode.TrackedHand : XRInputModalityManager.InputMode.None;
					this.SetLeftMode(leftMode);
					return;
				}
				if (usages2.Contains(CommonUsages.RightHand))
				{
					XRInputModalityManager.InputMode rightMode = this.GetRightHandIsTracked() ? XRInputModalityManager.InputMode.TrackedHand : XRInputModalityManager.InputMode.None;
					this.SetRightMode(rightMode);
				}
			}
		}

		private void OnDeviceConnected(InputDevice device)
		{
			InputDeviceCharacteristics characteristics = device.characteristics;
			if (characteristics == XRInputTrackingAggregator.Characteristics.leftHandInteraction || characteristics == XRInputTrackingAggregator.Characteristics.leftMicrosoftHandInteraction)
			{
				if (this.GetLeftHandIsTracked())
				{
					this.SetLeftMode(XRInputModalityManager.InputMode.TrackedHand);
					return;
				}
				this.UpdateMode(device, new Action<XRInputModalityManager.InputMode>(this.SetLeftMode));
				return;
			}
			else if (characteristics == XRInputTrackingAggregator.Characteristics.rightHandInteraction || characteristics == XRInputTrackingAggregator.Characteristics.rightMicrosoftHandInteraction)
			{
				if (this.GetRightHandIsTracked())
				{
					this.SetRightMode(XRInputModalityManager.InputMode.TrackedHand);
					return;
				}
				this.UpdateMode(device, new Action<XRInputModalityManager.InputMode>(this.SetRightMode));
				return;
			}
			else
			{
				if (characteristics == XRInputTrackingAggregator.Characteristics.leftController)
				{
					this.UpdateMode(device, new Action<XRInputModalityManager.InputMode>(this.SetLeftMode));
					return;
				}
				if (characteristics == XRInputTrackingAggregator.Characteristics.rightController)
				{
					this.UpdateMode(device, new Action<XRInputModalityManager.InputMode>(this.SetRightMode));
				}
				return;
			}
		}

		private void OnDeviceDisconnected(InputDevice device)
		{
			this.m_InputDeviceMonitor.RemoveDevice(device);
			InputDeviceCharacteristics characteristics = device.characteristics;
			if (characteristics == XRInputTrackingAggregator.Characteristics.leftController || characteristics == XRInputTrackingAggregator.Characteristics.leftHandInteraction || characteristics == XRInputTrackingAggregator.Characteristics.leftMicrosoftHandInteraction)
			{
				XRInputModalityManager.InputMode leftMode = this.GetLeftHandIsTracked() ? XRInputModalityManager.InputMode.TrackedHand : XRInputModalityManager.InputMode.None;
				this.SetLeftMode(leftMode);
				return;
			}
			if (characteristics == XRInputTrackingAggregator.Characteristics.rightController || characteristics == XRInputTrackingAggregator.Characteristics.rightHandInteraction || characteristics == XRInputTrackingAggregator.Characteristics.rightMicrosoftHandInteraction)
			{
				XRInputModalityManager.InputMode rightMode = this.GetRightHandIsTracked() ? XRInputModalityManager.InputMode.TrackedHand : XRInputModalityManager.InputMode.None;
				this.SetRightMode(rightMode);
			}
		}

		private void OnDeviceConfigChanged(InputDevice device)
		{
			this.OnDeviceConnected(device);
		}

		private void OnControllerTrackingAcquired(TrackedDevice device)
		{
			if (!(device is XRController))
			{
				return;
			}
			ReadOnlyArray<InternedString> usages = device.usages;
			if (this.m_LeftInputMode == XRInputModalityManager.InputMode.None && usages.Contains(CommonUsages.LeftHand))
			{
				this.SetLeftMode(XRInputModalityManager.InputMode.MotionController);
				return;
			}
			if (this.m_RightInputMode == XRInputModalityManager.InputMode.None && usages.Contains(CommonUsages.RightHand))
			{
				this.SetRightMode(XRInputModalityManager.InputMode.MotionController);
			}
		}

		private void OnControllerTrackingAcquired(InputDevice device)
		{
			InputDeviceCharacteristics characteristics = device.characteristics;
			if (this.m_LeftInputMode == XRInputModalityManager.InputMode.None && characteristics == XRInputTrackingAggregator.Characteristics.leftController)
			{
				this.SetLeftMode(XRInputModalityManager.InputMode.MotionController);
				return;
			}
			if (this.m_RightInputMode == XRInputModalityManager.InputMode.None && characteristics == XRInputTrackingAggregator.Characteristics.rightController)
			{
				this.SetRightMode(XRInputModalityManager.InputMode.MotionController);
			}
		}

		[HideInInspector]
		[SerializeField]
		[Tooltip("GameObject representing the left hand group of interactors. Will toggle on when using hand tracking and off when using motion controllers.")]
		private GameObject m_LeftHand;

		[HideInInspector]
		[SerializeField]
		[Tooltip("GameObject representing the right hand group of interactors. Will toggle on when using hand tracking and off when using motion controllers.")]
		private GameObject m_RightHand;

		[Header("Motion Controllers")]
		[SerializeField]
		[Tooltip("GameObject representing the left motion controller group of interactors. Will toggle on when using motion controllers and off when using hand tracking.")]
		private GameObject m_LeftController;

		[SerializeField]
		[Tooltip("GameObject representing the right motion controller group of interactors. Will toggle on when using motion controllers and off when using hand tracking.")]
		private GameObject m_RightController;

		[HideInInspector]
		[SerializeField]
		private UnityEvent m_TrackedHandModeStarted;

		[HideInInspector]
		[SerializeField]
		private UnityEvent m_TrackedHandModeEnded;

		[Header("Events")]
		[SerializeField]
		private UnityEvent m_MotionControllerModeStarted;

		[SerializeField]
		private UnityEvent m_MotionControllerModeEnded;

		private readonly XRInputModalityManager.TrackedDeviceMonitor m_TrackedDeviceMonitor = new XRInputModalityManager.TrackedDeviceMonitor();

		private readonly XRInputModalityManager.InputDeviceMonitor m_InputDeviceMonitor = new XRInputModalityManager.InputDeviceMonitor();

		private static BindableEnum<XRInputModalityManager.InputMode> s_CurrentInputMode = new BindableEnum<XRInputModalityManager.InputMode>(XRInputModalityManager.InputMode.None, true, null, false);

		private XRInputModalityManager.InputMode m_LeftInputMode;

		private XRInputModalityManager.InputMode m_RightInputMode;

		public enum InputMode
		{
			None,
			TrackedHand,
			MotionController
		}

		private class TrackedDeviceMonitor
		{
			public event Action<TrackedDevice> trackingAcquired;

			public void AddDevice(TrackedDevice device)
			{
				if (!this.m_MonitoredDevices.Contains(device.deviceId))
				{
					this.m_MonitoredDevices.Add(device.deviceId);
					this.Subscribe();
				}
			}

			public void RemoveDevice(TrackedDevice device)
			{
				if (this.m_MonitoredDevices.Remove(device.deviceId) && this.m_MonitoredDevices.Count == 0)
				{
					this.Unsubscribe();
				}
			}

			public void ClearAllDevices()
			{
				if (this.m_MonitoredDevices.Count > 0)
				{
					this.m_MonitoredDevices.Clear();
					this.Unsubscribe();
				}
			}

			private void Subscribe()
			{
				if (!this.m_Subscribed && this.m_MonitoredDevices.Count > 0)
				{
					InputSystem.onAfterUpdate += this.OnAfterInputUpdate;
					this.m_Subscribed = true;
				}
			}

			private void Unsubscribe()
			{
				if (this.m_Subscribed)
				{
					InputSystem.onAfterUpdate -= this.OnAfterInputUpdate;
					this.m_Subscribed = false;
				}
			}

			private void OnAfterInputUpdate()
			{
				for (int i = 0; i < this.m_MonitoredDevices.Count; i++)
				{
					TrackedDevice trackedDevice = InputSystem.GetDeviceById(this.m_MonitoredDevices[i]) as TrackedDevice;
					if (trackedDevice != null && XRInputModalityManager.IsTracked(trackedDevice))
					{
						this.m_MonitoredDevices.RemoveAt(i);
						i--;
						Action<TrackedDevice> action = this.trackingAcquired;
						if (action != null)
						{
							action(trackedDevice);
						}
					}
				}
				if (this.m_MonitoredDevices.Count == 0)
				{
					this.Unsubscribe();
				}
			}

			private readonly List<int> m_MonitoredDevices = new List<int>();

			private bool m_Subscribed;
		}

		private class InputDeviceMonitor
		{
			public event Action<InputDevice> trackingAcquired;

			public void AddDevice(InputDevice device)
			{
				if (!this.m_MonitoredDevices.Contains(device))
				{
					this.m_MonitoredDevices.Add(device);
					this.Subscribe();
				}
			}

			public void RemoveDevice(InputDevice device)
			{
				if (this.m_MonitoredDevices.Remove(device) && this.m_MonitoredDevices.Count == 0)
				{
					this.Unsubscribe();
				}
			}

			public void ClearAllDevices()
			{
				if (this.m_MonitoredDevices.Count > 0)
				{
					this.m_MonitoredDevices.Clear();
					this.Unsubscribe();
				}
			}

			private void Subscribe()
			{
				if (!this.m_Subscribed && this.m_MonitoredDevices.Count > 0)
				{
					InputTracking.trackingAcquired += this.OnTrackingAcquired;
					this.m_Subscribed = true;
				}
			}

			private void Unsubscribe()
			{
				if (this.m_Subscribed)
				{
					InputTracking.trackingAcquired -= this.OnTrackingAcquired;
					this.m_Subscribed = false;
				}
			}

			private void OnTrackingAcquired(XRNodeState nodeState)
			{
				for (int i = 0; i < this.m_MonitoredDevices.Count; i++)
				{
					InputDevice inputDevice = this.m_MonitoredDevices[i];
					if (XRInputModalityManager.IsTracked(inputDevice))
					{
						this.m_MonitoredDevices.RemoveAt(i);
						i--;
						Action<InputDevice> action = this.trackingAcquired;
						if (action != null)
						{
							action(inputDevice);
						}
					}
				}
				if (this.m_MonitoredDevices.Count == 0)
				{
					this.Unsubscribe();
				}
			}

			private readonly List<InputDevice> m_MonitoredDevices = new List<InputDevice>();

			private bool m_Subscribed;
		}
	}
}
