using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	[AddComponentMenu("Input/Debug/Touch Simulation")]
	[ExecuteInEditMode]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/Touch.html#touch-simulation")]
	public class TouchSimulation : MonoBehaviour, IInputStateChangeMonitor
	{
		public Touchscreen simulatedTouchscreen { get; private set; }

		public static TouchSimulation instance
		{
			get
			{
				return TouchSimulation.s_Instance;
			}
		}

		public static void Enable()
		{
			if (TouchSimulation.instance == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.SetActive(false);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				TouchSimulation.s_Instance = gameObject.AddComponent<TouchSimulation>();
				TouchSimulation.instance.gameObject.SetActive(true);
			}
			TouchSimulation.instance.enabled = true;
		}

		public static void Disable()
		{
			if (TouchSimulation.instance != null)
			{
				TouchSimulation.instance.enabled = false;
			}
		}

		public static void Destroy()
		{
			TouchSimulation.Disable();
			if (TouchSimulation.s_Instance != null)
			{
				Object.Destroy(TouchSimulation.s_Instance.gameObject);
				TouchSimulation.s_Instance = null;
			}
		}

		protected void AddPointer(Pointer pointer)
		{
			if (pointer == null)
			{
				throw new ArgumentNullException("pointer");
			}
			if (this.m_Pointers.ContainsReference(this.m_NumPointers, pointer))
			{
				return;
			}
			ArrayHelpers.AppendWithCapacity<Pointer>(ref this.m_Pointers, ref this.m_NumPointers, pointer, 10);
			ArrayHelpers.Append<Vector2>(ref this.m_CurrentPositions, default(Vector2));
			ArrayHelpers.Append<int>(ref this.m_CurrentDisplayIndices, 0);
			InputSystem.DisableDevice(pointer, true);
		}

		protected void RemovePointer(Pointer pointer)
		{
			if (pointer == null)
			{
				throw new ArgumentNullException("pointer");
			}
			int num = this.m_Pointers.IndexOfReference(pointer, this.m_NumPointers);
			if (num == -1)
			{
				return;
			}
			for (int i = 0; i < this.m_Touches.Length; i++)
			{
				ButtonControl buttonControl = this.m_Touches[i];
				if (buttonControl == null || buttonControl.device == pointer)
				{
					this.UpdateTouch(i, num, TouchPhase.Canceled, default(InputEventPtr));
				}
			}
			this.m_Pointers.EraseAtWithCapacity(ref this.m_NumPointers, num);
			ArrayHelpers.EraseAt<Vector2>(ref this.m_CurrentPositions, num);
			ArrayHelpers.EraseAt<int>(ref this.m_CurrentDisplayIndices, num);
			if (pointer.added)
			{
				InputSystem.EnableDevice(pointer);
			}
		}

		private unsafe void OnEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (device == this.simulatedTouchscreen)
			{
				return;
			}
			int num = this.m_Pointers.IndexOfReference(device, this.m_NumPointers);
			if (num < 0)
			{
				return;
			}
			FourCC type = eventPtr.type;
			if (type != 1398030676 && type != 1145852993)
			{
				return;
			}
			Pointer pointer = this.m_Pointers[num];
			Vector2Control position = pointer.position;
			void* statePtrFromStateEventUnchecked = position.GetStatePtrFromStateEventUnchecked(eventPtr, type);
			if (statePtrFromStateEventUnchecked != null)
			{
				this.m_CurrentPositions[num] = position.ReadValueFromState(statePtrFromStateEventUnchecked);
			}
			IntegerControl displayIndex = pointer.displayIndex;
			void* statePtrFromStateEventUnchecked2 = displayIndex.GetStatePtrFromStateEventUnchecked(eventPtr, type);
			if (statePtrFromStateEventUnchecked2 != null)
			{
				this.m_CurrentDisplayIndices[num] = displayIndex.ReadValueFromState(statePtrFromStateEventUnchecked2);
			}
			for (int i = 0; i < this.m_Touches.Length; i++)
			{
				ButtonControl buttonControl = this.m_Touches[i];
				if (buttonControl != null && buttonControl.device == device)
				{
					void* statePtrFromStateEventUnchecked3 = buttonControl.GetStatePtrFromStateEventUnchecked(eventPtr, type);
					if (statePtrFromStateEventUnchecked3 == null)
					{
						if (statePtrFromStateEventUnchecked != null)
						{
							this.UpdateTouch(i, num, TouchPhase.Moved, eventPtr);
						}
					}
					else if (buttonControl.ReadValueFromState(statePtrFromStateEventUnchecked3) < ButtonControl.s_GlobalDefaultButtonPressPoint * ButtonControl.s_GlobalDefaultButtonReleaseThreshold)
					{
						this.UpdateTouch(i, num, TouchPhase.Ended, eventPtr);
					}
				}
			}
			foreach (InputControl inputControl in eventPtr.EnumerateControls(InputControlExtensions.Enumerate.IgnoreControlsInDefaultState, device, 0f))
			{
				if (inputControl.isButton)
				{
					void* statePtrFromStateEventUnchecked4 = inputControl.GetStatePtrFromStateEventUnchecked(eventPtr, type);
					float num2 = 0f;
					inputControl.ReadValueFromStateIntoBuffer(statePtrFromStateEventUnchecked4, UnsafeUtility.AddressOf<float>(ref num2), 4);
					if (num2 > ButtonControl.s_GlobalDefaultButtonPressPoint)
					{
						int num3 = this.m_Touches.IndexOfReference(inputControl, -1);
						if (num3 < 0)
						{
							num3 = this.m_Touches.IndexOfReference(null, -1);
							if (num3 >= 0)
							{
								this.m_Touches[num3] = (ButtonControl)inputControl;
								this.UpdateTouch(num3, num, TouchPhase.Began, eventPtr);
							}
						}
						else
						{
							this.UpdateTouch(num3, num, TouchPhase.Moved, eventPtr);
						}
					}
				}
			}
			eventPtr.handled = true;
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			if (device == this.simulatedTouchscreen && change == InputDeviceChange.Removed)
			{
				TouchSimulation.Disable();
				return;
			}
			if (change != InputDeviceChange.Added)
			{
				if (change != InputDeviceChange.Removed)
				{
					return;
				}
				Pointer pointer = device as Pointer;
				if (pointer != null)
				{
					this.RemovePointer(pointer);
				}
			}
			else
			{
				Pointer pointer2 = device as Pointer;
				if (pointer2 != null)
				{
					if (device is Touchscreen)
					{
						return;
					}
					this.AddPointer(pointer2);
					return;
				}
			}
		}

		protected void OnEnable()
		{
			if (this.simulatedTouchscreen != null)
			{
				if (!this.simulatedTouchscreen.added)
				{
					InputSystem.AddDevice(this.simulatedTouchscreen);
				}
			}
			else
			{
				this.simulatedTouchscreen = (InputSystem.GetDevice("Simulated Touchscreen") as Touchscreen);
				if (this.simulatedTouchscreen == null)
				{
					this.simulatedTouchscreen = InputSystem.AddDevice<Touchscreen>("Simulated Touchscreen");
				}
			}
			if (this.m_Touches == null)
			{
				this.m_Touches = new ButtonControl[this.simulatedTouchscreen.touches.Count];
			}
			if (this.m_TouchIds == null)
			{
				this.m_TouchIds = new int[this.simulatedTouchscreen.touches.Count];
			}
			foreach (InputDevice device in InputSystem.devices)
			{
				this.OnDeviceChange(device, InputDeviceChange.Added);
			}
			if (this.m_OnDeviceChange == null)
			{
				this.m_OnDeviceChange = new Action<InputDevice, InputDeviceChange>(this.OnDeviceChange);
			}
			if (this.m_OnEvent == null)
			{
				this.m_OnEvent = new Action<InputEventPtr, InputDevice>(this.OnEvent);
			}
			InputSystem.onDeviceChange += this.m_OnDeviceChange;
			InputSystem.onEvent += this.m_OnEvent;
		}

		protected void OnDisable()
		{
			if (this.simulatedTouchscreen != null && this.simulatedTouchscreen.added)
			{
				InputSystem.RemoveDevice(this.simulatedTouchscreen);
			}
			for (int i = 0; i < this.m_NumPointers; i++)
			{
				InputSystem.EnableDevice(this.m_Pointers[i]);
			}
			this.m_Pointers.Clear(this.m_NumPointers);
			this.m_Touches.Clear<ButtonControl>();
			this.m_NumPointers = 0;
			this.m_LastTouchId = 0;
			InputSystem.onDeviceChange -= this.m_OnDeviceChange;
			InputSystem.onEvent -= this.m_OnEvent;
		}

		private void UpdateTouch(int touchIndex, int pointerIndex, TouchPhase phase, InputEventPtr eventPtr = default(InputEventPtr))
		{
			Vector2 vector = this.m_CurrentPositions[pointerIndex];
			byte displayIndex = (byte)this.m_CurrentDisplayIndices[pointerIndex];
			TouchState state = new TouchState
			{
				phase = phase,
				position = vector,
				displayIndex = displayIndex
			};
			if (phase == TouchPhase.Began)
			{
				state.startTime = (eventPtr.valid ? eventPtr.time : InputState.currentTime);
				state.startPosition = vector;
				int num = this.m_LastTouchId + 1;
				this.m_LastTouchId = num;
				state.touchId = num;
				this.m_TouchIds[touchIndex] = this.m_LastTouchId;
			}
			else
			{
				state.touchId = this.m_TouchIds[touchIndex];
			}
			InputSystem.QueueStateEvent<TouchState>(this.simulatedTouchscreen, state, -1.0);
			if (phase.IsEndedOrCanceled())
			{
				this.m_Touches[touchIndex] = null;
			}
		}

		void IInputStateChangeMonitor.NotifyControlStateChanged(InputControl control, double time, InputEventPtr eventPtr, long monitorIndex)
		{
		}

		void IInputStateChangeMonitor.NotifyTimerExpired(InputControl control, double time, long monitorIndex, int timerIndex)
		{
		}

		protected void InstallStateChangeMonitors(int startIndex = 0)
		{
		}

		protected void OnSourceControlChangedValue(InputControl control, double time, InputEventPtr eventPtr, long sourceDeviceAndButtonIndex)
		{
		}

		protected void UninstallStateChangeMonitors(int startIndex = 0)
		{
		}

		[NonSerialized]
		private int m_NumPointers;

		[NonSerialized]
		private Pointer[] m_Pointers;

		[NonSerialized]
		private Vector2[] m_CurrentPositions;

		[NonSerialized]
		private int[] m_CurrentDisplayIndices;

		[NonSerialized]
		private ButtonControl[] m_Touches;

		[NonSerialized]
		private int[] m_TouchIds;

		[NonSerialized]
		private int m_LastTouchId;

		[NonSerialized]
		private Action<InputDevice, InputDeviceChange> m_OnDeviceChange;

		[NonSerialized]
		private Action<InputEventPtr, InputDevice> m_OnEvent;

		internal static TouchSimulation s_Instance;
	}
}
