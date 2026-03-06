using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngineInternal.Input;

namespace UnityEngine.InputSystem.LowLevel
{
	internal class NativeInputRuntime : IInputRuntime
	{
		public int AllocateDeviceId()
		{
			return NativeInputSystem.AllocateDeviceId();
		}

		public void Update(InputUpdateType updateType)
		{
			NativeInputSystem.Update((NativeInputUpdateType)updateType);
		}

		public unsafe void QueueEvent(InputEvent* ptr)
		{
			NativeInputSystem.QueueInputEvent((IntPtr)((void*)ptr));
		}

		public unsafe long DeviceCommand(int deviceId, InputDeviceCommand* commandPtr)
		{
			if (commandPtr == null)
			{
				throw new ArgumentNullException("commandPtr");
			}
			return NativeInputSystem.IOCTL(deviceId, commandPtr->type, new IntPtr(commandPtr->payloadPtr), commandPtr->payloadSizeInBytes);
		}

		public unsafe InputUpdateDelegate onUpdate
		{
			get
			{
				return this.m_OnUpdate;
			}
			set
			{
				if (value != null)
				{
					NativeInputSystem.onUpdate = delegate(NativeInputUpdateType updateType, NativeInputEventBuffer* eventBufferPtr)
					{
						InputEventBuffer inputEventBuffer = new InputEventBuffer((InputEvent*)eventBufferPtr->eventBuffer, eventBufferPtr->eventCount, eventBufferPtr->sizeInBytes, eventBufferPtr->capacityInBytes);
						try
						{
							value((InputUpdateType)updateType, ref inputEventBuffer);
						}
						catch (Exception ex)
						{
							Debug.LogException(ex);
							Debug.LogError(string.Format("{0} during event processing of {1} update; resetting event buffer", ex.GetType().Name, updateType));
							inputEventBuffer.Reset();
						}
						if (inputEventBuffer.eventCount > 0)
						{
							eventBufferPtr->eventCount = inputEventBuffer.eventCount;
							eventBufferPtr->sizeInBytes = (int)inputEventBuffer.sizeInBytes;
							eventBufferPtr->capacityInBytes = (int)inputEventBuffer.capacityInBytes;
							eventBufferPtr->eventBuffer = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<byte>(inputEventBuffer.data);
							return;
						}
						eventBufferPtr->eventCount = 0;
						eventBufferPtr->sizeInBytes = 0;
					};
				}
				else
				{
					NativeInputSystem.onUpdate = null;
				}
				this.m_OnUpdate = value;
			}
		}

		public Action<InputUpdateType> onBeforeUpdate
		{
			get
			{
				return this.m_OnBeforeUpdate;
			}
			set
			{
				if (value != null)
				{
					NativeInputSystem.onBeforeUpdate = delegate(NativeInputUpdateType updateType)
					{
						value((InputUpdateType)updateType);
					};
				}
				else
				{
					NativeInputSystem.onBeforeUpdate = null;
				}
				this.m_OnBeforeUpdate = value;
			}
		}

		public Func<InputUpdateType, bool> onShouldRunUpdate
		{
			get
			{
				return this.m_OnShouldRunUpdate;
			}
			set
			{
				if (value != null)
				{
					NativeInputSystem.onShouldRunUpdate = ((NativeInputUpdateType updateType) => value((InputUpdateType)updateType));
				}
				else
				{
					NativeInputSystem.onShouldRunUpdate = null;
				}
				this.m_OnShouldRunUpdate = value;
			}
		}

		public Action<int, string> onDeviceDiscovered
		{
			get
			{
				return NativeInputSystem.onDeviceDiscovered;
			}
			set
			{
				NativeInputSystem.onDeviceDiscovered = value;
			}
		}

		public Action onShutdown
		{
			get
			{
				return this.m_ShutdownMethod;
			}
			set
			{
				if (value == null)
				{
					Application.quitting -= this.OnShutdown;
				}
				else if (this.m_ShutdownMethod == null)
				{
					Application.quitting += this.OnShutdown;
				}
				this.m_ShutdownMethod = value;
			}
		}

		public Action<bool> onPlayerFocusChanged
		{
			get
			{
				return this.m_FocusChangedMethod;
			}
			set
			{
				if (value == null)
				{
					Application.focusChanged -= this.OnFocusChanged;
				}
				else if (this.m_FocusChangedMethod == null)
				{
					Application.focusChanged += this.OnFocusChanged;
				}
				this.m_FocusChangedMethod = value;
			}
		}

		public bool isPlayerFocused
		{
			get
			{
				return Application.isFocused;
			}
		}

		public float pollingFrequency
		{
			get
			{
				return this.m_PollingFrequency;
			}
			set
			{
				this.m_PollingFrequency = value;
				NativeInputSystem.SetPollingFrequency(value);
			}
		}

		public double currentTime
		{
			get
			{
				return NativeInputSystem.currentTime;
			}
		}

		public double currentTimeForFixedUpdate
		{
			get
			{
				return (double)Time.fixedUnscaledTime + this.currentTimeOffsetToRealtimeSinceStartup;
			}
		}

		public double currentTimeOffsetToRealtimeSinceStartup
		{
			get
			{
				return NativeInputSystem.currentTimeOffsetToRealtimeSinceStartup;
			}
		}

		public float unscaledGameTime
		{
			get
			{
				return Time.unscaledTime;
			}
		}

		public bool runInBackground
		{
			get
			{
				return Application.runInBackground || this.m_RunInBackground;
			}
			set
			{
				this.m_RunInBackground = value;
			}
		}

		private void OnShutdown()
		{
			this.m_ShutdownMethod();
		}

		private bool OnWantsToShutdown()
		{
			if (!this.m_DidCallOnShutdown)
			{
				this.OnShutdown();
				this.m_DidCallOnShutdown = true;
			}
			return true;
		}

		private void OnFocusChanged(bool focus)
		{
			this.m_FocusChangedMethod(focus);
		}

		public Vector2 screenSize
		{
			get
			{
				return new Vector2((float)Screen.width, (float)Screen.height);
			}
		}

		public ScreenOrientation screenOrientation
		{
			get
			{
				return Screen.orientation;
			}
		}

		public bool normalizeScrollWheelDelta
		{
			get
			{
				return NativeInputSystem.normalizeScrollWheelDelta;
			}
			set
			{
				NativeInputSystem.normalizeScrollWheelDelta = value;
			}
		}

		public float scrollWheelDeltaPerTick
		{
			get
			{
				return NativeInputSystem.GetScrollWheelDeltaPerTick();
			}
		}

		public static readonly NativeInputRuntime instance = new NativeInputRuntime();

		private bool m_RunInBackground;

		private Action m_ShutdownMethod;

		private InputUpdateDelegate m_OnUpdate;

		private Action<InputUpdateType> m_OnBeforeUpdate;

		private Func<InputUpdateType, bool> m_OnShouldRunUpdate;

		private float m_PollingFrequency = 60f;

		private bool m_DidCallOnShutdown;

		private Action<bool> m_FocusChangedMethod;
	}
}
