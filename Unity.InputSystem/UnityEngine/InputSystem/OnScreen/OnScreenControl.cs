using System;
using Unity.Collections;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.OnScreen
{
	public abstract class OnScreenControl : MonoBehaviour
	{
		public string controlPath
		{
			get
			{
				return this.controlPathInternal;
			}
			set
			{
				this.controlPathInternal = value;
				if (base.isActiveAndEnabled)
				{
					this.SetupInputControl();
				}
			}
		}

		public InputControl control
		{
			get
			{
				return this.m_Control;
			}
		}

		protected abstract string controlPathInternal { get; set; }

		private void SetupInputControl()
		{
			string controlPathInternal = this.controlPathInternal;
			if (string.IsNullOrEmpty(controlPathInternal))
			{
				return;
			}
			string text = InputControlPath.TryGetDeviceLayout(controlPathInternal);
			if (text == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Cannot determine device layout to use based on control path '",
					controlPathInternal,
					"' used in ",
					base.GetType().Name,
					" component"
				}), this);
				return;
			}
			InternedString b = new InternedString(text);
			int num = -1;
			for (int i = 0; i < OnScreenControl.s_OnScreenDevices.length; i++)
			{
				if (OnScreenControl.s_OnScreenDevices[i].device.m_Layout == b)
				{
					num = i;
					break;
				}
			}
			InputDevice inputDevice;
			if (num == -1)
			{
				try
				{
					inputDevice = InputSystem.AddDevice(text, null, null);
				}
				catch (Exception exception)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Could not create device with layout '",
						text,
						"' used in '",
						base.GetType().Name,
						"' component"
					}));
					Debug.LogException(exception);
					return;
				}
				InputSystem.AddDeviceUsage(inputDevice, "OnScreen");
				InputEventPtr eventPtr;
				NativeArray<byte> buffer = StateEvent.From(inputDevice, out eventPtr, Allocator.Persistent);
				num = OnScreenControl.s_OnScreenDevices.Append(new OnScreenControl.OnScreenDeviceInfo
				{
					eventPtr = eventPtr,
					buffer = buffer,
					device = inputDevice
				});
			}
			else
			{
				inputDevice = OnScreenControl.s_OnScreenDevices[num].device;
			}
			this.m_Control = InputControlPath.TryFindControl(inputDevice, controlPathInternal, 0);
			if (this.m_Control == null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Cannot find control with path '",
					controlPathInternal,
					"' on device of type '",
					text,
					"' referenced by component '",
					base.GetType().Name,
					"'"
				}), this);
				if (OnScreenControl.s_OnScreenDevices[num].firstControl == null)
				{
					OnScreenControl.s_OnScreenDevices[num].Destroy();
					OnScreenControl.s_OnScreenDevices.RemoveAt(num);
				}
				return;
			}
			this.m_InputEventPtr = OnScreenControl.s_OnScreenDevices[num].eventPtr;
			OnScreenControl.s_OnScreenDevices[num] = OnScreenControl.s_OnScreenDevices[num].AddControl(this);
		}

		protected void SendValueToControl<TValue>(TValue value) where TValue : struct
		{
			if (this.m_Control == null)
			{
				return;
			}
			InputControl<TValue> inputControl = this.m_Control as InputControl<TValue>;
			if (inputControl == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"The control path ",
					this.controlPath,
					" yields a control of type ",
					this.m_Control.GetType().Name,
					" which is not an InputControl with value type ",
					typeof(TValue).Name
				}), "value");
			}
			this.m_InputEventPtr.internalTime = InputRuntime.s_Instance.currentTime;
			inputControl.WriteValueIntoEvent(value, this.m_InputEventPtr);
			InputSystem.QueueEvent(this.m_InputEventPtr);
		}

		protected void SentDefaultValueToControl()
		{
			if (this.m_Control == null)
			{
				return;
			}
			this.m_InputEventPtr.internalTime = InputRuntime.s_Instance.currentTime;
			this.m_Control.ResetToDefaultStateInEvent(this.m_InputEventPtr);
			InputSystem.QueueEvent(this.m_InputEventPtr);
		}

		internal static bool HasAnyActive
		{
			get
			{
				return OnScreenControl.s_nbActiveInstances != 0;
			}
		}

		protected virtual void OnEnable()
		{
			OnScreenControl.s_nbActiveInstances++;
			this.SetupInputControl();
			if (this.m_Control == null)
			{
				return;
			}
			if (OnScreenControl.s_nbActiveInstances == 1 && PlayerInput.isSinglePlayer)
			{
				PlayerInput playerByIndex = PlayerInput.GetPlayerByIndex(0);
				if (playerByIndex != null && !playerByIndex.neverAutoSwitchControlSchemes)
				{
					ReadOnlyArray<InputDevice> devices = playerByIndex.devices;
					bool flag = false;
					foreach (InputDevice inputDevice in devices)
					{
						if (this.m_Control.device.deviceId == inputDevice.deviceId)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						playerByIndex.SwitchCurrentControlScheme(new InputDevice[]
						{
							this.m_Control.device
						});
					}
				}
			}
		}

		protected virtual void OnDisable()
		{
			OnScreenControl.s_nbActiveInstances--;
			if (this.m_Control == null)
			{
				return;
			}
			InputDevice device = this.m_Control.device;
			for (int i = 0; i < OnScreenControl.s_OnScreenDevices.length; i++)
			{
				if (OnScreenControl.s_OnScreenDevices[i].device == device)
				{
					OnScreenControl.OnScreenDeviceInfo onScreenDeviceInfo = OnScreenControl.s_OnScreenDevices[i].RemoveControl(this);
					if (onScreenDeviceInfo.firstControl == null)
					{
						OnScreenControl.s_OnScreenDevices[i].Destroy();
						OnScreenControl.s_OnScreenDevices.RemoveAt(i);
					}
					else
					{
						OnScreenControl.s_OnScreenDevices[i] = onScreenDeviceInfo;
						if (!this.m_Control.CheckStateIsAtDefault())
						{
							this.SentDefaultValueToControl();
						}
					}
					this.m_Control = null;
					this.m_InputEventPtr = default(InputEventPtr);
					return;
				}
			}
		}

		internal string GetWarningMessage()
		{
			return string.Format("{0} needs to be attached as a child to a UI Canvas and have a RectTransform component to function properly.", base.GetType());
		}

		private InputControl m_Control;

		private OnScreenControl m_NextControlOnDevice;

		private InputEventPtr m_InputEventPtr;

		private static int s_nbActiveInstances;

		private static InlinedArray<OnScreenControl.OnScreenDeviceInfo> s_OnScreenDevices;

		private struct OnScreenDeviceInfo
		{
			public OnScreenControl.OnScreenDeviceInfo AddControl(OnScreenControl control)
			{
				control.m_NextControlOnDevice = this.firstControl;
				this.firstControl = control;
				return this;
			}

			public OnScreenControl.OnScreenDeviceInfo RemoveControl(OnScreenControl control)
			{
				if (this.firstControl == control)
				{
					this.firstControl = control.m_NextControlOnDevice;
				}
				else
				{
					OnScreenControl nextControlOnDevice = this.firstControl.m_NextControlOnDevice;
					OnScreenControl onScreenControl = this.firstControl;
					while (nextControlOnDevice != null)
					{
						if (!(nextControlOnDevice != control))
						{
							onScreenControl.m_NextControlOnDevice = nextControlOnDevice.m_NextControlOnDevice;
							break;
						}
						onScreenControl = nextControlOnDevice;
						nextControlOnDevice = nextControlOnDevice.m_NextControlOnDevice;
					}
				}
				control.m_NextControlOnDevice = null;
				return this;
			}

			public void Destroy()
			{
				if (this.buffer.IsCreated)
				{
					this.buffer.Dispose();
				}
				if (this.device != null)
				{
					InputSystem.RemoveDevice(this.device);
				}
				this.device = null;
				this.buffer = default(NativeArray<byte>);
			}

			public InputEventPtr eventPtr;

			public NativeArray<byte> buffer;

			public InputDevice device;

			public OnScreenControl firstControl;
		}
	}
}
