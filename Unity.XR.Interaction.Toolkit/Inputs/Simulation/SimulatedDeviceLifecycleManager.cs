using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.Hands;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[AddComponentMenu("XR/Debug/Simulated Device Lifecycle Manager", 11)]
	[DefaultExecutionOrder(-29995)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedDeviceLifecycleManager.html")]
	public class SimulatedDeviceLifecycleManager : MonoBehaviour
	{
		public bool removeOtherHMDDevices
		{
			get
			{
				return this.m_RemoveOtherHMDDevices;
			}
			set
			{
				this.m_RemoveOtherHMDDevices = value;
			}
		}

		public bool handTrackingCapability
		{
			get
			{
				return this.m_HandTrackingCapability;
			}
			set
			{
				this.m_HandTrackingCapability = value;
			}
		}

		public SimulatedDeviceLifecycleManager.DeviceMode deviceMode
		{
			get
			{
				return this.m_DeviceMode;
			}
		}

		public static SimulatedDeviceLifecycleManager instance { get; private set; }

		protected virtual void Awake()
		{
			if (SimulatedDeviceLifecycleManager.instance == null)
			{
				SimulatedDeviceLifecycleManager.instance = this;
			}
			else if (SimulatedDeviceLifecycleManager.instance != this)
			{
				Debug.LogWarning(string.Format("Another instance of Simulated Device Lifecycle Manager already exists ({0}), destroying {1}.", SimulatedDeviceLifecycleManager.instance, base.gameObject), this);
				Object.Destroy(base.gameObject);
				return;
			}
			this.InitializeHandSubsystem();
		}

		protected virtual void OnEnable()
		{
			if (this.m_RemoveOtherHMDDevices)
			{
				foreach (InputDevice inputDevice in InputSystem.devices.ToArray())
				{
					if (inputDevice is XRHMD && !(inputDevice is XRSimulatedHMD))
					{
						InputSystem.RemoveDevice(inputDevice);
					}
				}
				InputSystem.onDeviceChange += this.OnInputDeviceChange;
				this.m_OnInputDeviceChangeSubscribed = true;
			}
			this.AddDevices();
		}

		protected virtual void OnDisable()
		{
			if (this.m_OnInputDeviceChangeSubscribed)
			{
				InputSystem.onDeviceChange -= this.OnInputDeviceChange;
				this.m_OnInputDeviceChangeSubscribed = false;
			}
			this.RemoveDevices();
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual void Update()
		{
		}

		internal void ApplyHMDState(XRSimulatedHMDState state)
		{
			if (this.m_HMDDevice != null && this.m_HMDDevice.added)
			{
				InputState.Change<XRSimulatedHMDState>(this.m_HMDDevice, state, InputUpdateType.None, default(InputEventPtr));
			}
		}

		internal void ApplyControllerState(XRSimulatedControllerState leftControllerState, XRSimulatedControllerState rightControllerState)
		{
			if (this.m_LeftControllerDevice != null && this.m_LeftControllerDevice.added)
			{
				InputState.Change<XRSimulatedControllerState>(this.m_LeftControllerDevice, leftControllerState, InputUpdateType.None, default(InputEventPtr));
			}
			if (this.m_RightControllerDevice != null && this.m_RightControllerDevice.added)
			{
				InputState.Change<XRSimulatedControllerState>(this.m_RightControllerDevice, rightControllerState, InputUpdateType.None, default(InputEventPtr));
			}
		}

		internal void ApplyHandState(XRSimulatedHandState leftHandState, XRSimulatedHandState rightHandState)
		{
		}

		internal void SwitchDeviceMode()
		{
		}

		internal virtual void AddDevices()
		{
			if (this.m_HMDDevice == null)
			{
				InputDeviceDescription description = new InputDeviceDescription
				{
					product = "XRSimulatedHMD",
					capabilities = new XRDeviceDescriptor
					{
						characteristics = XRInputTrackingAggregator.Characteristics.hmd
					}.ToJson()
				};
				this.m_HMDDevice = (InputSystem.AddDevice(description) as XRSimulatedHMD);
				if (this.m_HMDDevice == null)
				{
					Debug.LogError("Failed to create XRSimulatedHMD.", this);
				}
			}
			else
			{
				InputSystem.AddDevice(this.m_HMDDevice);
			}
			if (this.m_DeviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
			{
				this.AddControllerDevices();
			}
		}

		internal virtual void RemoveDevices()
		{
			if (this.m_HMDDevice != null && this.m_HMDDevice.added)
			{
				InputSystem.RemoveDevice(this.m_HMDDevice);
			}
			this.RemoveControllerDevices();
		}

		private void AddControllerDevices()
		{
			if (this.m_LeftControllerDevice == null)
			{
				InputDeviceDescription description = new InputDeviceDescription
				{
					product = "XRSimulatedController",
					capabilities = new XRDeviceDescriptor
					{
						deviceName = string.Format("{0} - {1}", "XRSimulatedController", CommonUsages.LeftHand),
						characteristics = XRInputTrackingAggregator.Characteristics.leftController
					}.ToJson()
				};
				this.m_LeftControllerDevice = (InputSystem.AddDevice(description) as XRSimulatedController);
				if (this.m_LeftControllerDevice != null)
				{
					InputSystem.SetDeviceUsage(this.m_LeftControllerDevice, CommonUsages.LeftHand);
				}
				else
				{
					Debug.LogError(string.Format("Failed to create {0} for {1}.", "XRSimulatedController", CommonUsages.LeftHand), this);
				}
			}
			else
			{
				InputSystem.AddDevice(this.m_LeftControllerDevice);
			}
			if (this.m_RightControllerDevice != null)
			{
				InputSystem.AddDevice(this.m_RightControllerDevice);
				return;
			}
			InputDeviceDescription description2 = new InputDeviceDescription
			{
				product = "XRSimulatedController",
				capabilities = new XRDeviceDescriptor
				{
					deviceName = string.Format("{0} - {1}", "XRSimulatedController", CommonUsages.RightHand),
					characteristics = XRInputTrackingAggregator.Characteristics.rightController
				}.ToJson()
			};
			this.m_RightControllerDevice = (InputSystem.AddDevice(description2) as XRSimulatedController);
			if (this.m_RightControllerDevice != null)
			{
				InputSystem.SetDeviceUsage(this.m_RightControllerDevice, CommonUsages.RightHand);
				return;
			}
			Debug.LogError(string.Format("Failed to create {0} for {1}.", "XRSimulatedController", CommonUsages.RightHand), this);
		}

		private void RemoveControllerDevices()
		{
			if (this.m_LeftControllerDevice != null && this.m_LeftControllerDevice.added)
			{
				InputSystem.RemoveDevice(this.m_LeftControllerDevice);
			}
			if (this.m_RightControllerDevice != null && this.m_RightControllerDevice.added)
			{
				InputSystem.RemoveDevice(this.m_RightControllerDevice);
			}
		}

		private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (!this.m_RemoveOtherHMDDevices)
			{
				return;
			}
			if (change == InputDeviceChange.Added && device is XRHMD && !(device is XRSimulatedHMD))
			{
				InputSystem.RemoveDevice(device);
			}
		}

		private void InitializeHandSubsystem()
		{
		}

		private static SimulatedDeviceLifecycleManager.DeviceMode Negate(SimulatedDeviceLifecycleManager.DeviceMode mode)
		{
			if (mode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
			{
				return SimulatedDeviceLifecycleManager.DeviceMode.Hand;
			}
			if (mode != SimulatedDeviceLifecycleManager.DeviceMode.Hand)
			{
				return SimulatedDeviceLifecycleManager.DeviceMode.Controller;
			}
			return SimulatedDeviceLifecycleManager.DeviceMode.Controller;
		}

		[SerializeField]
		private bool m_RemoveOtherHMDDevices = true;

		[SerializeField]
		private bool m_HandTrackingCapability = true;

		private SimulatedDeviceLifecycleManager.DeviceMode m_DeviceMode;

		private XRSimulatedHMD m_HMDDevice;

		private XRSimulatedController m_LeftControllerDevice;

		private XRSimulatedController m_RightControllerDevice;

		private bool m_OnInputDeviceChangeSubscribed;

		private bool m_DeviceModeDirty;

		private bool m_StartedDeviceModeChange;

		public enum DeviceMode
		{
			Controller,
			Hand,
			None
		}
	}
}
