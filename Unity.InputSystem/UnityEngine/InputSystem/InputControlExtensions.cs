using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public static class InputControlExtensions
	{
		public static TControl FindInParentChain<TControl>(this InputControl control) where TControl : InputControl
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			for (InputControl inputControl = control; inputControl != null; inputControl = inputControl.parent)
			{
				TControl tcontrol = inputControl as TControl;
				if (tcontrol != null)
				{
					return tcontrol;
				}
			}
			return default(TControl);
		}

		public static bool IsPressed(this InputControl control, float buttonPressPoint = 0f)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (Mathf.Approximately(0f, buttonPressPoint))
			{
				ButtonControl buttonControl = control as ButtonControl;
				if (buttonControl != null)
				{
					buttonPressPoint = buttonControl.pressPointOrDefault;
				}
				else
				{
					buttonPressPoint = ButtonControl.s_GlobalDefaultButtonPressPoint;
				}
			}
			return control.IsActuated(buttonPressPoint);
		}

		public static bool IsActuated(this InputControl control, float threshold = 0f)
		{
			if (control.CheckStateIsAtDefault())
			{
				return false;
			}
			float magnitude = control.magnitude;
			if (magnitude < 0f)
			{
				return Mathf.Approximately(threshold, 0f);
			}
			if (Mathf.Approximately(threshold, 0f))
			{
				return magnitude > 0f;
			}
			return magnitude >= threshold;
		}

		public static object ReadValueAsObject(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.ReadValueFromStateAsObject(control.currentStatePtr);
		}

		public unsafe static void ReadValueIntoBuffer(this InputControl control, void* buffer, int bufferSize)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			control.ReadValueFromStateIntoBuffer(control.currentStatePtr, buffer, bufferSize);
		}

		public static object ReadDefaultValueAsObject(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.ReadValueFromStateAsObject(control.defaultStatePtr);
		}

		public static TValue ReadValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr inputEvent) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			TValue result;
			if (!control.ReadValueFromEvent(inputEvent, out result))
			{
				return default(TValue);
			}
			return result;
		}

		public unsafe static bool ReadValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr inputEvent, out TValue value) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(inputEvent);
			if (statePtrFromStateEvent == null)
			{
				value = control.ReadDefaultValue();
				return false;
			}
			value = control.ReadValueFromState(statePtrFromStateEvent);
			return true;
		}

		public unsafe static object ReadValueFromEventAsObject(this InputControl control, InputEventPtr inputEvent)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(inputEvent);
			if (statePtrFromStateEvent == null)
			{
				return control.ReadDefaultValueAsObject();
			}
			return control.ReadValueFromStateAsObject(statePtrFromStateEvent);
		}

		public static TValue ReadUnprocessedValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr eventPtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			TValue result = default(TValue);
			control.ReadUnprocessedValueFromEvent(eventPtr, out result);
			return result;
		}

		public unsafe static bool ReadUnprocessedValueFromEvent<TValue>(this InputControl<TValue> control, InputEventPtr inputEvent, out TValue value) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(inputEvent);
			if (statePtrFromStateEvent == null)
			{
				value = control.ReadDefaultValue();
				return false;
			}
			value = control.ReadUnprocessedValueFromState(statePtrFromStateEvent);
			return true;
		}

		public unsafe static void WriteValueFromObjectIntoEvent(this InputControl control, InputEventPtr eventPtr, object value)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(eventPtr);
			if (statePtrFromStateEvent == null)
			{
				return;
			}
			control.WriteValueFromObjectIntoState(value, statePtrFromStateEvent);
		}

		public unsafe static void WriteValueIntoState(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			int valueSizeInBytes = control.valueSizeInBytes;
			void* ptr = UnsafeUtility.Malloc((long)valueSizeInBytes, 8, Allocator.Temp);
			try
			{
				control.ReadValueFromStateIntoBuffer(control.currentStatePtr, ptr, valueSizeInBytes);
				control.WriteValueFromBufferIntoState(ptr, valueSizeInBytes, statePtr);
			}
			finally
			{
				UnsafeUtility.Free(ptr, Allocator.Temp);
			}
		}

		public unsafe static void WriteValueIntoState<TValue>(this InputControl control, TValue value, void* statePtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputControl<TValue> inputControl = control as InputControl<TValue>;
			if (inputControl == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Expecting control of type '",
					typeof(TValue).Name,
					"' but got '",
					control.GetType().Name,
					"'"
				}));
			}
			inputControl.WriteValueIntoState(value, statePtr);
		}

		public unsafe static void WriteValueIntoState<TValue>(this InputControl<TValue> control, TValue value, void* statePtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			void* bufferPtr = UnsafeUtility.AddressOf<TValue>(ref value);
			int bufferSize = UnsafeUtility.SizeOf<TValue>();
			control.WriteValueFromBufferIntoState(bufferPtr, bufferSize, statePtr);
		}

		public unsafe static void WriteValueIntoState<TValue>(this InputControl<TValue> control, void* statePtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			control.WriteValueIntoState(control.ReadValue(), statePtr);
		}

		public unsafe static void WriteValueIntoState<TValue, TState>(this InputControl<TValue> control, TValue value, ref TState state) where TValue : struct where TState : struct, IInputStateTypeInfo
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			int num = UnsafeUtility.SizeOf<TState>();
			if ((ulong)(control.stateOffsetRelativeToDeviceRoot + control.m_StateBlock.alignedSizeInBytes) >= (ulong)((long)num))
			{
				throw new ArgumentException(string.Format("Control {0} with offset {1} and size of {2} bits is out of bounds for state of type {3} with size {4}", new object[]
				{
					control.path,
					control.stateOffsetRelativeToDeviceRoot,
					control.m_StateBlock.sizeInBits,
					typeof(TState).Name,
					num
				}), "state");
			}
			byte* statePtr = (byte*)UnsafeUtility.AddressOf<TState>(ref state);
			control.WriteValueIntoState(value, (void*)statePtr);
		}

		public static void WriteValueIntoEvent<TValue>(this InputControl control, TValue value, InputEventPtr eventPtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			InputControl<TValue> inputControl = control as InputControl<TValue>;
			if (inputControl == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Expecting control of type '",
					typeof(TValue).Name,
					"' but got '",
					control.GetType().Name,
					"'"
				}));
			}
			inputControl.WriteValueIntoEvent(value, eventPtr);
		}

		public unsafe static void WriteValueIntoEvent<TValue>(this InputControl<TValue> control, TValue value, InputEventPtr eventPtr) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(eventPtr);
			if (statePtrFromStateEvent == null)
			{
				return;
			}
			control.WriteValueIntoState(value, statePtrFromStateEvent);
		}

		public unsafe static void CopyState(this InputDevice device, void* buffer, int bufferSizeInBytes)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (bufferSizeInBytes <= 0)
			{
				throw new ArgumentException("bufferSizeInBytes must be positive", "bufferSizeInBytes");
			}
			InputStateBlock stateBlock = device.m_StateBlock;
			long size = Math.Min((long)bufferSizeInBytes, (long)((ulong)stateBlock.alignedSizeInBytes));
			UnsafeUtility.MemCpy(buffer, (void*)((byte*)device.currentStatePtr + stateBlock.byteOffset), size);
		}

		public unsafe static void CopyState<TState>(this InputDevice device, out TState state) where TState : struct, IInputStateTypeInfo
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			state = default(TState);
			if (device.stateBlock.format != state.format)
			{
				throw new ArgumentException(string.Format("Struct '{0}' has state format '{1}' which doesn't match device '{2}' with state format '{3}'", new object[]
				{
					typeof(TState).Name,
					state.format,
					device,
					device.stateBlock.format
				}), "TState");
			}
			int bufferSizeInBytes = UnsafeUtility.SizeOf<TState>();
			void* buffer = UnsafeUtility.AddressOf<TState>(ref state);
			device.CopyState(buffer, bufferSizeInBytes);
		}

		public static bool CheckStateIsAtDefault(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.CheckStateIsAtDefault(control.currentStatePtr, null);
		}

		public unsafe static bool CheckStateIsAtDefault(this InputControl control, void* statePtr, void* maskPtr = null)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareState(statePtr, control.defaultStatePtr, maskPtr);
		}

		public static bool CheckStateIsAtDefaultIgnoringNoise(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			return control.CheckStateIsAtDefaultIgnoringNoise(control.currentStatePtr);
		}

		public unsafe static bool CheckStateIsAtDefaultIgnoringNoise(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CheckStateIsAtDefault(statePtr, InputStateBuffers.s_NoiseMaskBuffer);
		}

		public unsafe static bool CompareStateIgnoringNoise(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareState(control.currentStatePtr, statePtr, control.noiseMaskPtr);
		}

		public unsafe static bool CompareState(this InputControl control, void* firstStatePtr, void* secondStatePtr, void* maskPtr = null)
		{
			byte* ptr = (byte*)firstStatePtr + control.m_StateBlock.byteOffset;
			byte* ptr2 = (byte*)secondStatePtr + control.m_StateBlock.byteOffset;
			byte* ptr3 = (maskPtr != null) ? ((byte*)maskPtr + control.m_StateBlock.byteOffset) : null;
			if (control.m_StateBlock.sizeInBits == 1U)
			{
				return (ptr3 != null && MemoryHelpers.ReadSingleBit((void*)ptr3, control.m_StateBlock.bitOffset)) || MemoryHelpers.ReadSingleBit((void*)ptr2, control.m_StateBlock.bitOffset) == MemoryHelpers.ReadSingleBit((void*)ptr, control.m_StateBlock.bitOffset);
			}
			return MemoryHelpers.MemCmpBitRegion((void*)ptr, (void*)ptr2, control.m_StateBlock.bitOffset, control.m_StateBlock.sizeInBits, (void*)ptr3);
		}

		public unsafe static bool CompareState(this InputControl control, void* statePtr, void* maskPtr = null)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareState(control.currentStatePtr, statePtr, maskPtr);
		}

		public unsafe static bool HasValueChangeInState(this InputControl control, void* statePtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (statePtr == null)
			{
				throw new ArgumentNullException("statePtr");
			}
			return control.CompareValue(control.currentStatePtr, statePtr);
		}

		public unsafe static bool HasValueChangeInEvent(this InputControl control, InputEventPtr eventPtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			void* statePtrFromStateEvent = control.GetStatePtrFromStateEvent(eventPtr);
			return statePtrFromStateEvent != null && control.CompareValue(control.currentStatePtr, statePtrFromStateEvent);
		}

		public unsafe static void* GetStatePtrFromStateEvent(this InputControl control, InputEventPtr eventPtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			return control.GetStatePtrFromStateEventUnchecked(eventPtr, eventPtr.type);
		}

		internal unsafe static void* GetStatePtrFromStateEventUnchecked(this InputControl control, InputEventPtr eventPtr, FourCC eventType)
		{
			uint num;
			FourCC stateFormat;
			uint num2;
			void* ptr2;
			if (eventType == 1398030676)
			{
				StateEvent* ptr = StateEvent.FromUnchecked(eventPtr);
				num = 0U;
				stateFormat = ptr->stateFormat;
				num2 = ptr->stateSizeInBytes;
				ptr2 = ptr->state;
			}
			else
			{
				if (!(eventType == 1145852993))
				{
					throw new ArgumentException(string.Format("Event must be a StateEvent or DeltaStateEvent but is a {0} instead", eventType), "eventPtr");
				}
				DeltaStateEvent* ptr3 = DeltaStateEvent.FromUnchecked(eventPtr);
				num = ptr3->stateOffset;
				stateFormat = ptr3->stateFormat;
				num2 = ptr3->deltaStateSizeInBytes;
				ptr2 = ptr3->deltaState;
			}
			InputDevice device = control.device;
			if (stateFormat != device.m_StateBlock.format && (!device.hasStateCallbacks || !((IInputStateCallbackReceiver)device).GetStateOffsetForEvent(control, eventPtr, ref num)))
			{
				return null;
			}
			num += device.m_StateBlock.byteOffset;
			ref InputStateBlock ptr4 = ref control.m_StateBlock;
			long num3 = (long)ptr4.effectiveByteOffset - (long)((ulong)num);
			if (num3 < 0L || num3 + (long)((ulong)ptr4.alignedSizeInBytes) > (long)((ulong)num2))
			{
				return null;
			}
			return (void*)((byte*)ptr2 - num);
		}

		public unsafe static bool ResetToDefaultStateInEvent(this InputControl control, InputEventPtr eventPtr)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr");
			}
			FourCC type = eventPtr.type;
			if (type != 1398030676 && type != 1145852993)
			{
				throw new ArgumentException("Given event is not a StateEvent or a DeltaStateEvent", "eventPtr");
			}
			byte* statePtrFromStateEvent = (byte*)control.GetStatePtrFromStateEvent(eventPtr);
			if (statePtrFromStateEvent == null)
			{
				return false;
			}
			byte* defaultStatePtr = (byte*)control.defaultStatePtr;
			ref InputStateBlock ptr = ref control.m_StateBlock;
			uint byteOffset = ptr.byteOffset;
			MemoryHelpers.MemCpyBitRegion((void*)(statePtrFromStateEvent + byteOffset), (void*)(defaultStatePtr + byteOffset), ptr.bitOffset, ptr.sizeInBits);
			return true;
		}

		public static void QueueValueChange<TValue>(this InputControl<TValue> control, TValue value, double time = -1.0) where TValue : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			InputEventPtr eventPtr;
			using (StateEvent.From(control.device, out eventPtr, Allocator.Temp))
			{
				if (time >= 0.0)
				{
					eventPtr.time = time;
				}
				control.WriteValueIntoEvent(value, eventPtr);
				InputSystem.QueueEvent(eventPtr);
			}
		}

		public unsafe static void AccumulateValueInEvent(this InputControl<float> control, void* currentStatePtr, InputEventPtr newState)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			float num;
			if (!control.ReadUnprocessedValueFromEvent(newState, out num))
			{
				return;
			}
			float num2 = control.ReadUnprocessedValueFromState(currentStatePtr);
			control.WriteValueIntoEvent(num2 + num, newState);
		}

		internal unsafe static void AccumulateValueInEvent(this InputControl<Vector2> control, void* currentStatePtr, InputEventPtr newState)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			Vector2 b;
			if (!control.ReadUnprocessedValueFromEvent(newState, out b))
			{
				return;
			}
			Vector2 a = control.ReadUnprocessedValueFromState(currentStatePtr);
			control.WriteValueIntoEvent(a + b, newState);
		}

		public static void FindControlsRecursive<TControl>(this InputControl parent, IList<TControl> controls, Func<TControl, bool> predicate) where TControl : InputControl
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (controls == null)
			{
				throw new ArgumentNullException("controls");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			TControl tcontrol = parent as TControl;
			if (tcontrol != null && predicate(tcontrol))
			{
				controls.Add(tcontrol);
			}
			int count = parent.children.Count;
			for (int i = 0; i < count; i++)
			{
				parent.children[i].FindControlsRecursive(controls, predicate);
			}
		}

		internal static string BuildPath(this InputControl control, string deviceLayout, StringBuilder builder = null)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (string.IsNullOrEmpty(deviceLayout))
			{
				throw new ArgumentNullException("deviceLayout");
			}
			if (builder == null)
			{
				builder = new StringBuilder();
			}
			InputDevice device = control.device;
			builder.Append('<');
			builder.Append(deviceLayout.Escape("\\>", "\\>"));
			builder.Append('>');
			ReadOnlyArray<InternedString> usages = device.usages;
			for (int i = 0; i < usages.Count; i++)
			{
				builder.Append('{');
				builder.Append(usages[i].ToString().Escape("\\}", "\\}"));
				builder.Append('}');
			}
			builder.Append('/');
			string text = device.path.Replace("\\", "\\\\");
			string text2 = control.path.Replace("\\", "\\\\");
			builder.Append(text2, text.Length + 1, text2.Length - text.Length - 1);
			return builder.ToString();
		}

		public static InputControlExtensions.InputEventControlCollection EnumerateControls(this InputEventPtr eventPtr, InputControlExtensions.Enumerate flags, InputDevice device = null, float magnitudeThreshold = 0f)
		{
			if (!eventPtr.valid)
			{
				throw new ArgumentNullException("eventPtr", "Given event pointer must not be null");
			}
			FourCC type = eventPtr.type;
			if (type != 1398030676 && type != 1145852993)
			{
				throw new ArgumentException(string.Format("Event must be a StateEvent or DeltaStateEvent but is a {0} instead", type), "eventPtr");
			}
			if (device == null)
			{
				int deviceId = eventPtr.deviceId;
				device = InputSystem.GetDeviceById(deviceId);
				if (device == null)
				{
					throw new ArgumentException(string.Format("Cannot find device with ID {0} referenced by event", deviceId), "eventPtr");
				}
			}
			return new InputControlExtensions.InputEventControlCollection
			{
				m_Device = device,
				m_EventPtr = eventPtr,
				m_Flags = flags,
				m_MagnitudeThreshold = magnitudeThreshold
			};
		}

		public static InputControlExtensions.InputEventControlCollection EnumerateChangedControls(this InputEventPtr eventPtr, InputDevice device = null, float magnitudeThreshold = 0f)
		{
			return eventPtr.EnumerateControls(InputControlExtensions.Enumerate.IgnoreControlsInCurrentState, device, magnitudeThreshold);
		}

		public static bool HasButtonPress(this InputEventPtr eventPtr, float magnitude = -1f, bool buttonControlsOnly = true)
		{
			return eventPtr.GetFirstButtonPressOrNull(magnitude, buttonControlsOnly) != null;
		}

		public static InputControl GetFirstButtonPressOrNull(this InputEventPtr eventPtr, float magnitude = -1f, bool buttonControlsOnly = true)
		{
			if (eventPtr.type != 1398030676 && eventPtr.type != 1145852993)
			{
				return null;
			}
			if (magnitude < 0f)
			{
				magnitude = InputSystem.settings.defaultButtonPressPoint;
			}
			foreach (InputControl inputControl in eventPtr.EnumerateControls(InputControlExtensions.Enumerate.IgnoreControlsInDefaultState, null, magnitude))
			{
				if (!buttonControlsOnly || inputControl.isButton)
				{
					return inputControl;
				}
			}
			return null;
		}

		public static IEnumerable<InputControl> GetAllButtonPresses(this InputEventPtr eventPtr, float magnitude = -1f, bool buttonControlsOnly = true)
		{
			if (eventPtr.type != 1398030676 && eventPtr.type != 1145852993)
			{
				yield break;
			}
			if (magnitude < 0f)
			{
				magnitude = InputSystem.settings.defaultButtonPressPoint;
			}
			foreach (InputControl inputControl in eventPtr.EnumerateControls(InputControlExtensions.Enumerate.IgnoreControlsInDefaultState, null, magnitude))
			{
				if (!buttonControlsOnly || inputControl.isButton)
				{
					yield return inputControl;
				}
			}
			InputControlExtensions.InputEventControlEnumerator inputEventControlEnumerator = default(InputControlExtensions.InputEventControlEnumerator);
			yield break;
			yield break;
		}

		public static InputControlExtensions.ControlBuilder Setup(this InputControl control)
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (control.isSetupFinished)
			{
				throw new InvalidOperationException(string.Format("The setup of {0} cannot be modified; control is already in use", control));
			}
			return new InputControlExtensions.ControlBuilder
			{
				control = control
			};
		}

		public static InputControlExtensions.DeviceBuilder Setup(this InputDevice device, int controlCount, int usageCount, int aliasCount)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.isSetupFinished)
			{
				throw new InvalidOperationException(string.Format("The setup of {0} cannot be modified; control is already in use", device));
			}
			if (controlCount < 1)
			{
				throw new ArgumentOutOfRangeException("controlCount");
			}
			if (usageCount < 0)
			{
				throw new ArgumentOutOfRangeException("usageCount");
			}
			if (aliasCount < 0)
			{
				throw new ArgumentOutOfRangeException("aliasCount");
			}
			device.m_Device = device;
			device.m_ChildrenForEachControl = new InputControl[controlCount];
			if (usageCount > 0)
			{
				device.m_UsagesForEachControl = new InternedString[usageCount];
				device.m_UsageToControl = new InputControl[usageCount];
			}
			if (aliasCount > 0)
			{
				device.m_AliasesForEachControl = new InternedString[aliasCount];
			}
			return new InputControlExtensions.DeviceBuilder
			{
				device = device
			};
		}

		[Flags]
		public enum Enumerate
		{
			IgnoreControlsInDefaultState = 1,
			IgnoreControlsInCurrentState = 2,
			IncludeSyntheticControls = 4,
			IncludeNoisyControls = 8,
			IncludeNonLeafControls = 16
		}

		public struct InputEventControlCollection : IEnumerable<InputControl>, IEnumerable
		{
			public InputEventPtr eventPtr
			{
				get
				{
					return this.m_EventPtr;
				}
			}

			public InputControlExtensions.InputEventControlEnumerator GetEnumerator()
			{
				return new InputControlExtensions.InputEventControlEnumerator(this.m_EventPtr, this.m_Device, this.m_Flags, this.m_MagnitudeThreshold);
			}

			IEnumerator<InputControl> IEnumerable<InputControl>.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			internal InputDevice m_Device;

			internal InputEventPtr m_EventPtr;

			internal InputControlExtensions.Enumerate m_Flags;

			internal float m_MagnitudeThreshold;
		}

		public struct InputEventControlEnumerator : IEnumerator<InputControl>, IEnumerator, IDisposable
		{
			internal unsafe InputEventControlEnumerator(InputEventPtr eventPtr, InputDevice device, InputControlExtensions.Enumerate flags, float magnitudeThreshold = 0f)
			{
				this.m_Device = device;
				this.m_StateOffsetToControlIndex = device.m_StateOffsetToControlMap;
				this.m_StateOffsetToControlIndexLength = this.m_StateOffsetToControlIndex.LengthSafe<uint>();
				this.m_AllControls = device.m_ChildrenForEachControl;
				this.m_EventPtr = eventPtr;
				this.m_Flags = flags;
				this.m_CurrentControl = null;
				this.m_CurrentIndexInStateOffsetToControlIndexMap = 0;
				this.m_CurrentControlStateBitOffset = 0U;
				this.m_EventState = default(byte*);
				this.m_CurrentBitOffset = 0U;
				this.m_EndBitOffset = 0U;
				this.m_MagnitudeThreshold = magnitudeThreshold;
				if ((flags & InputControlExtensions.Enumerate.IncludeNoisyControls) == (InputControlExtensions.Enumerate)0)
				{
					this.m_NoiseMask = (byte*)device.noiseMaskPtr + device.m_StateBlock.byteOffset;
				}
				else
				{
					this.m_NoiseMask = default(byte*);
				}
				if ((flags & InputControlExtensions.Enumerate.IgnoreControlsInDefaultState) != (InputControlExtensions.Enumerate)0)
				{
					this.m_DefaultState = (byte*)device.defaultStatePtr + device.m_StateBlock.byteOffset;
				}
				else
				{
					this.m_DefaultState = default(byte*);
				}
				if ((flags & InputControlExtensions.Enumerate.IgnoreControlsInCurrentState) != (InputControlExtensions.Enumerate)0)
				{
					this.m_CurrentState = (byte*)device.currentStatePtr + device.m_StateBlock.byteOffset;
				}
				else
				{
					this.m_CurrentState = default(byte*);
				}
				this.Reset();
			}

			private unsafe bool CheckDefault(uint numBits)
			{
				return MemoryHelpers.MemCmpBitRegion((void*)this.m_EventState, (void*)this.m_DefaultState, this.m_CurrentBitOffset, numBits, (void*)this.m_NoiseMask);
			}

			private unsafe bool CheckCurrent(uint numBits)
			{
				return MemoryHelpers.MemCmpBitRegion((void*)this.m_EventState, (void*)this.m_CurrentState, this.m_CurrentBitOffset, numBits, (void*)this.m_NoiseMask);
			}

			public unsafe bool MoveNext()
			{
				if (!this.m_EventPtr.valid)
				{
					throw new ObjectDisposedException("Enumerator has already been disposed");
				}
				if (this.m_CurrentControl != null && (this.m_Flags & InputControlExtensions.Enumerate.IncludeNonLeafControls) != (InputControlExtensions.Enumerate)0)
				{
					InputControl parent = this.m_CurrentControl.parent;
					if (parent != this.m_Device)
					{
						this.m_CurrentControl = parent;
						return true;
					}
				}
				bool flag = this.m_DefaultState != null;
				bool flag2 = this.m_CurrentState != null;
				for (;;)
				{
					this.m_CurrentControl = null;
					if (flag2 || flag)
					{
						if ((this.m_CurrentBitOffset & 7U) != 0U)
						{
							uint num = this.m_CurrentBitOffset + 8U & 7U;
							if ((flag2 && this.CheckCurrent(num)) || (flag && this.CheckDefault(num)))
							{
								this.m_CurrentBitOffset += num;
							}
						}
						while (this.m_CurrentBitOffset < this.m_EndBitOffset)
						{
							uint num2 = this.m_CurrentBitOffset >> 3;
							byte b = this.m_EventState[num2];
							int num3 = (int)((this.m_NoiseMask != null) ? this.m_NoiseMask[num2] : byte.MaxValue);
							if (flag2 && ((int)this.m_CurrentState[num2] & num3) == ((int)b & num3))
							{
								this.m_CurrentBitOffset += 8U;
							}
							else
							{
								if (!flag || ((int)this.m_DefaultState[num2] & num3) != ((int)b & num3))
								{
									break;
								}
								this.m_CurrentBitOffset += 8U;
							}
						}
					}
					if (this.m_CurrentBitOffset >= this.m_EndBitOffset || this.m_CurrentIndexInStateOffsetToControlIndexMap >= this.m_StateOffsetToControlIndexLength)
					{
						break;
					}
					while (this.m_CurrentIndexInStateOffsetToControlIndexMap < this.m_StateOffsetToControlIndexLength)
					{
						uint num4;
						uint num5;
						uint num6;
						InputDevice.DecodeStateOffsetToControlMapEntry(this.m_StateOffsetToControlIndex[this.m_CurrentIndexInStateOffsetToControlIndexMap], out num4, out num5, out num6);
						if (num5 >= this.m_CurrentControlStateBitOffset && this.m_CurrentBitOffset < num5 + num6 - this.m_CurrentControlStateBitOffset)
						{
							if (num5 - this.m_CurrentControlStateBitOffset >= this.m_CurrentBitOffset + 8U)
							{
								this.m_CurrentBitOffset = num5 - this.m_CurrentControlStateBitOffset;
								break;
							}
							if (num5 + num6 - this.m_CurrentControlStateBitOffset <= this.m_EndBitOffset)
							{
								if ((num5 & 7U) == 0U && (num6 & 7U) == 0U)
								{
									this.m_CurrentControl = this.m_AllControls[(int)num4];
								}
								else
								{
									if ((flag2 && MemoryHelpers.MemCmpBitRegion((void*)this.m_EventState, (void*)this.m_CurrentState, num5 - this.m_CurrentControlStateBitOffset, num6, (void*)this.m_NoiseMask)) || (flag && MemoryHelpers.MemCmpBitRegion((void*)this.m_EventState, (void*)this.m_DefaultState, num5 - this.m_CurrentControlStateBitOffset, num6, (void*)this.m_NoiseMask)))
									{
										goto IL_2BF;
									}
									this.m_CurrentControl = this.m_AllControls[(int)num4];
								}
								if ((this.m_Flags & InputControlExtensions.Enumerate.IncludeNoisyControls) == (InputControlExtensions.Enumerate)0 && this.m_CurrentControl.noisy)
								{
									this.m_CurrentControl = null;
								}
								else
								{
									if ((this.m_Flags & InputControlExtensions.Enumerate.IncludeSyntheticControls) != (InputControlExtensions.Enumerate)0 || (this.m_CurrentControl.m_ControlFlags & (InputControl.ControlFlags.IsSynthetic | InputControl.ControlFlags.UsesStateFromOtherControl)) <= (InputControl.ControlFlags)0)
									{
										this.m_CurrentIndexInStateOffsetToControlIndexMap++;
										break;
									}
									this.m_CurrentControl = null;
								}
							}
						}
						IL_2BF:
						this.m_CurrentIndexInStateOffsetToControlIndexMap++;
					}
					if (this.m_CurrentControl != null)
					{
						if (this.m_MagnitudeThreshold == 0f)
						{
							return true;
						}
						byte* statePtr = this.m_EventState - (this.m_CurrentControlStateBitOffset >> 3) - this.m_Device.m_StateBlock.byteOffset;
						float num7 = this.m_CurrentControl.EvaluateMagnitude((void*)statePtr);
						if (num7 < 0f || num7 >= this.m_MagnitudeThreshold)
						{
							return true;
						}
					}
				}
				return false;
			}

			public unsafe void Reset()
			{
				if (!this.m_EventPtr.valid)
				{
					throw new ObjectDisposedException("Enumerator has already been disposed");
				}
				FourCC type = this.m_EventPtr.type;
				FourCC stateFormat;
				if (type == 1398030676)
				{
					StateEvent* ptr = StateEvent.FromUnchecked(this.m_EventPtr);
					this.m_EventState = (byte*)ptr->state;
					this.m_EndBitOffset = ptr->stateSizeInBytes * 8U;
					this.m_CurrentBitOffset = 0U;
					stateFormat = ptr->stateFormat;
				}
				else
				{
					if (!(type == 1145852993))
					{
						throw new NotSupportedException(string.Format("Cannot iterate over controls in event of type '{0}'", type));
					}
					DeltaStateEvent* ptr2 = DeltaStateEvent.FromUnchecked(this.m_EventPtr);
					this.m_EventState = (byte*)ptr2->deltaState - ptr2->stateOffset;
					this.m_CurrentBitOffset = ptr2->stateOffset * 8U;
					this.m_EndBitOffset = this.m_CurrentBitOffset + ptr2->deltaStateSizeInBytes * 8U;
					stateFormat = ptr2->stateFormat;
				}
				this.m_CurrentIndexInStateOffsetToControlIndexMap = 0;
				this.m_CurrentControlStateBitOffset = 0U;
				this.m_CurrentControl = null;
				if (stateFormat != this.m_Device.m_StateBlock.format)
				{
					uint num = 0U;
					if (this.m_Device.hasStateCallbacks && ((IInputStateCallbackReceiver)this.m_Device).GetStateOffsetForEvent(null, this.m_EventPtr, ref num))
					{
						this.m_CurrentControlStateBitOffset = num * 8U;
						if (this.m_CurrentState != null)
						{
							this.m_CurrentState += num;
						}
						if (this.m_DefaultState != null)
						{
							this.m_DefaultState += num;
						}
						if (this.m_NoiseMask != null)
						{
							this.m_NoiseMask += num;
							return;
						}
					}
					else if (!(this.m_Device is Touchscreen) || !this.m_EventPtr.IsA<StateEvent>() || !(StateEvent.FromUnchecked(this.m_EventPtr)->stateFormat == TouchState.Format))
					{
						throw new InvalidOperationException(string.Format("{0} event with state format {1} cannot be used with device '{2}'", type, stateFormat, this.m_Device));
					}
				}
			}

			public void Dispose()
			{
				this.m_EventPtr = default(InputEventPtr);
			}

			public InputControl Current
			{
				get
				{
					return this.m_CurrentControl;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private InputControlExtensions.Enumerate m_Flags;

			private readonly InputDevice m_Device;

			private readonly uint[] m_StateOffsetToControlIndex;

			private readonly int m_StateOffsetToControlIndexLength;

			private readonly InputControl[] m_AllControls;

			private unsafe byte* m_DefaultState;

			private unsafe byte* m_CurrentState;

			private unsafe byte* m_NoiseMask;

			private InputEventPtr m_EventPtr;

			private InputControl m_CurrentControl;

			private int m_CurrentIndexInStateOffsetToControlIndexMap;

			private uint m_CurrentControlStateBitOffset;

			private unsafe byte* m_EventState;

			private uint m_CurrentBitOffset;

			private uint m_EndBitOffset;

			private float m_MagnitudeThreshold;
		}

		public struct ControlBuilder
		{
			public InputControl control { readonly get; internal set; }

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder At(InputDevice device, int index)
			{
				device.m_ChildrenForEachControl[index] = this.control;
				this.control.m_Device = device;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithParent(InputControl parent)
			{
				this.control.m_Parent = parent;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithName(string name)
			{
				this.control.m_Name = new InternedString(name);
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithDisplayName(string displayName)
			{
				this.control.m_DisplayNameFromLayout = new InternedString(displayName);
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithShortDisplayName(string shortDisplayName)
			{
				this.control.m_ShortDisplayNameFromLayout = new InternedString(shortDisplayName);
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithLayout(InternedString layout)
			{
				this.control.m_Layout = layout;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithUsages(int startIndex, int count)
			{
				this.control.m_UsageStartIndex = startIndex;
				this.control.m_UsageCount = count;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithAliases(int startIndex, int count)
			{
				this.control.m_AliasStartIndex = startIndex;
				this.control.m_AliasCount = count;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithChildren(int startIndex, int count)
			{
				this.control.m_ChildStartIndex = startIndex;
				this.control.m_ChildCount = count;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithStateBlock(InputStateBlock stateBlock)
			{
				this.control.m_StateBlock = stateBlock;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithDefaultState(PrimitiveValue value)
			{
				this.control.m_DefaultState = value;
				this.control.m_Device.hasControlsWithDefaultState = true;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithMinAndMax(PrimitiveValue min, PrimitiveValue max)
			{
				this.control.m_MinValue = min;
				this.control.m_MaxValue = max;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder WithProcessor<TProcessor, TValue>(TProcessor processor) where TProcessor : InputProcessor<TValue> where TValue : struct
			{
				((InputControl<TValue>)this.control).m_ProcessorStack.Append(processor);
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder IsNoisy(bool value)
			{
				this.control.noisy = value;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder IsSynthetic(bool value)
			{
				this.control.synthetic = value;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder DontReset(bool value)
			{
				this.control.dontReset = value;
				if (value)
				{
					this.control.m_Device.hasDontResetControls = true;
				}
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.ControlBuilder IsButton(bool value)
			{
				this.control.isButton = value;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Finish()
			{
				this.control.isSetupFinished = true;
			}
		}

		public struct DeviceBuilder
		{
			public InputDevice device { readonly get; internal set; }

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithName(string name)
			{
				this.device.m_Name = new InternedString(name);
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithDisplayName(string displayName)
			{
				this.device.m_DisplayNameFromLayout = new InternedString(displayName);
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithShortDisplayName(string shortDisplayName)
			{
				this.device.m_ShortDisplayNameFromLayout = new InternedString(shortDisplayName);
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithLayout(InternedString layout)
			{
				this.device.m_Layout = layout;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithChildren(int startIndex, int count)
			{
				this.device.m_ChildStartIndex = startIndex;
				this.device.m_ChildCount = count;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithStateBlock(InputStateBlock stateBlock)
			{
				this.device.m_StateBlock = stateBlock;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder IsNoisy(bool value)
			{
				this.device.noisy = value;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithControlUsage(int controlIndex, InternedString usage, InputControl control)
			{
				this.device.m_UsagesForEachControl[controlIndex] = usage;
				this.device.m_UsageToControl[controlIndex] = control;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithControlAlias(int controlIndex, InternedString alias)
			{
				this.device.m_AliasesForEachControl[controlIndex] = alias;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public InputControlExtensions.DeviceBuilder WithStateOffsetToControlIndexMap(uint[] map)
			{
				this.device.m_StateOffsetToControlMap = map;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public unsafe InputControlExtensions.DeviceBuilder WithControlTree(byte[] controlTreeNodes, ushort[] controlTreeIndicies)
			{
				int num = UnsafeUtility.SizeOf<InputDevice.ControlBitRangeNode>();
				int num2 = controlTreeNodes.Length / num;
				this.device.m_ControlTreeNodes = new InputDevice.ControlBitRangeNode[num2];
				fixed (byte[] array = controlTreeNodes)
				{
					byte* ptr;
					if (controlTreeNodes == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					for (int i = 0; i < num2; i++)
					{
						this.device.m_ControlTreeNodes[i] = *(InputDevice.ControlBitRangeNode*)(ptr + i * num);
					}
				}
				this.device.m_ControlTreeIndices = controlTreeIndicies;
				return this;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Finish()
			{
				int num = 0;
				using (ReadOnlyArray<InputControl>.Enumerator enumerator = this.device.allControls.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current is ButtonControl)
						{
							num++;
						}
					}
				}
				this.device.m_ButtonControlsCheckingPressState = new List<ButtonControl>(num);
				this.device.m_UpdatedButtons = new HashSet<int>(num);
				this.device.isSetupFinished = true;
			}
		}
	}
}
