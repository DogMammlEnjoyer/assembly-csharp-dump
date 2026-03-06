using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.XR;

namespace UnityEngine.InputSystem
{
	public static class InputSystem
	{
		public static event Action<string, InputControlLayoutChange> onLayoutChange
		{
			add
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onLayoutChange += value;
				}
			}
			remove
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onLayoutChange -= value;
				}
			}
		}

		public static void RegisterLayout(Type type, string name = null, InputDeviceMatcher? matches = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
			}
			InputSystem.s_Manager.RegisterControlLayout(name, type);
			if (matches != null)
			{
				InputSystem.s_Manager.RegisterControlLayoutMatcher(name, matches.Value);
			}
		}

		public static void RegisterLayout<T>(string name = null, InputDeviceMatcher? matches = null) where T : InputControl
		{
			InputSystem.RegisterLayout(typeof(T), name, matches);
		}

		public static void RegisterLayout(string json, string name = null, InputDeviceMatcher? matches = null)
		{
			InputSystem.s_Manager.RegisterControlLayout(json, name, false);
			if (matches != null)
			{
				InputSystem.s_Manager.RegisterControlLayoutMatcher(name, matches.Value);
			}
		}

		public static void RegisterLayoutOverride(string json, string name = null)
		{
			InputSystem.s_Manager.RegisterControlLayout(json, name, true);
		}

		public static void RegisterLayoutMatcher(string layoutName, InputDeviceMatcher matcher)
		{
			InputSystem.s_Manager.RegisterControlLayoutMatcher(layoutName, matcher);
		}

		public static void RegisterLayoutMatcher<TDevice>(InputDeviceMatcher matcher) where TDevice : InputDevice
		{
			InputSystem.s_Manager.RegisterControlLayoutMatcher(typeof(TDevice), matcher);
		}

		public static void RegisterLayoutBuilder(Func<InputControlLayout> buildMethod, string name, string baseLayout = null, InputDeviceMatcher? matches = null)
		{
			if (buildMethod == null)
			{
				throw new ArgumentNullException("buildMethod");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			InputSystem.s_Manager.RegisterControlLayoutBuilder(buildMethod, name, baseLayout);
			if (matches != null)
			{
				InputSystem.s_Manager.RegisterControlLayoutMatcher(name, matches.Value);
			}
		}

		public static void RegisterPrecompiledLayout<TDevice>(string metadata) where TDevice : InputDevice, new()
		{
			InputSystem.s_Manager.RegisterPrecompiledLayout<TDevice>(metadata);
		}

		public static void RemoveLayout(string name)
		{
			InputSystem.s_Manager.RemoveControlLayout(name);
		}

		public static string TryFindMatchingLayout(InputDeviceDescription deviceDescription)
		{
			return InputSystem.s_Manager.TryFindMatchingControlLayout(ref deviceDescription, 0);
		}

		public static IEnumerable<string> ListLayouts()
		{
			return InputSystem.s_Manager.ListControlLayouts(null);
		}

		public static IEnumerable<string> ListLayoutsBasedOn(string baseLayout)
		{
			if (string.IsNullOrEmpty(baseLayout))
			{
				throw new ArgumentNullException("baseLayout");
			}
			return InputSystem.s_Manager.ListControlLayouts(baseLayout);
		}

		public static InputControlLayout LoadLayout(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return InputSystem.s_Manager.TryLoadControlLayout(new InternedString(name));
		}

		public static InputControlLayout LoadLayout<TControl>() where TControl : InputControl
		{
			return InputSystem.s_Manager.TryLoadControlLayout(typeof(TControl));
		}

		public static string GetNameOfBaseLayout(string layoutName)
		{
			if (string.IsNullOrEmpty(layoutName))
			{
				throw new ArgumentNullException("layoutName");
			}
			InternedString key = new InternedString(layoutName);
			InternedString str;
			if (InputControlLayout.s_Layouts.baseLayoutTable.TryGetValue(key, out str))
			{
				return str;
			}
			return null;
		}

		public static bool IsFirstLayoutBasedOnSecond(string firstLayoutName, string secondLayoutName)
		{
			if (string.IsNullOrEmpty(firstLayoutName))
			{
				throw new ArgumentNullException("firstLayoutName");
			}
			if (string.IsNullOrEmpty(secondLayoutName))
			{
				throw new ArgumentNullException("secondLayoutName");
			}
			InternedString internedString = new InternedString(firstLayoutName);
			InternedString internedString2 = new InternedString(secondLayoutName);
			return internedString == internedString2 || InputControlLayout.s_Layouts.IsBasedOn(internedString2, internedString);
		}

		public static void RegisterProcessor(Type type, string name = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
				if (name.EndsWith("Processor"))
				{
					name = name.Substring(0, name.Length - "Processor".Length);
				}
			}
			Dictionary<InternedString, InputControlLayout.Collection.PrecompiledLayout> precompiledLayouts = InputSystem.s_Manager.m_Layouts.precompiledLayouts;
			foreach (InternedString key in new List<InternedString>(precompiledLayouts.Keys))
			{
				if (StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(precompiledLayouts[key].metadata, name, ';'))
				{
					InputSystem.s_Manager.m_Layouts.precompiledLayouts.Remove(key);
				}
			}
			InputSystem.s_Manager.processors.AddTypeRegistration(name, type);
		}

		public static void RegisterProcessor<T>(string name = null)
		{
			InputSystem.RegisterProcessor(typeof(T), name);
		}

		public static Type TryGetProcessor(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return InputSystem.s_Manager.processors.LookupTypeRegistration(name);
		}

		public static IEnumerable<string> ListProcessors()
		{
			return InputSystem.s_Manager.processors.names;
		}

		public static ReadOnlyArray<InputDevice> devices
		{
			get
			{
				return InputSystem.s_Manager.devices;
			}
		}

		public static ReadOnlyArray<InputDevice> disconnectedDevices
		{
			get
			{
				return new ReadOnlyArray<InputDevice>(InputSystem.s_Manager.m_DisconnectedDevices, 0, InputSystem.s_Manager.m_DisconnectedDevicesCount);
			}
		}

		public static event Action<InputDevice, InputDeviceChange> onDeviceChange
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onDeviceChange += value;
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onDeviceChange -= value;
				}
			}
		}

		public static event InputDeviceCommandDelegate onDeviceCommand
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onDeviceCommand += value;
				}
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onDeviceCommand -= value;
				}
			}
		}

		public static event InputDeviceFindControlLayoutDelegate onFindLayoutForDevice
		{
			add
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onFindControlLayoutForDevice += value;
				}
			}
			remove
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onFindControlLayoutForDevice -= value;
				}
			}
		}

		public static float pollingFrequency
		{
			get
			{
				return InputSystem.s_Manager.pollingFrequency;
			}
			set
			{
				InputSystem.s_Manager.pollingFrequency = value;
			}
		}

		public static InputDevice AddDevice(string layout, string name = null, string variants = null)
		{
			if (string.IsNullOrEmpty(layout))
			{
				throw new ArgumentNullException("layout");
			}
			return InputSystem.s_Manager.AddDevice(layout, name, new InternedString(variants));
		}

		public static TDevice AddDevice<TDevice>(string name = null) where TDevice : InputDevice
		{
			InputDevice inputDevice = InputSystem.s_Manager.AddDevice(typeof(TDevice), name);
			TDevice tdevice = inputDevice as TDevice;
			if (tdevice == null)
			{
				if (inputDevice != null)
				{
					InputSystem.RemoveDevice(inputDevice);
				}
				throw new InvalidOperationException("Layout registered for type '" + typeof(TDevice).Name + "' did not produce a device of that type; layout probably has been overridden");
			}
			return tdevice;
		}

		public static InputDevice AddDevice(InputDeviceDescription description)
		{
			if (description.empty)
			{
				throw new ArgumentException("Description must not be empty", "description");
			}
			return InputSystem.s_Manager.AddDevice(description);
		}

		public static void AddDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			InputSystem.s_Manager.AddDevice(device);
		}

		public static void RemoveDevice(InputDevice device)
		{
			InputSystem.s_Manager.RemoveDevice(device, false);
		}

		public static void FlushDisconnectedDevices()
		{
			InputSystem.s_Manager.FlushDisconnectedDevices();
		}

		public static InputDevice GetDevice(string nameOrLayout)
		{
			return InputSystem.s_Manager.TryGetDevice(nameOrLayout);
		}

		public static TDevice GetDevice<TDevice>() where TDevice : InputDevice
		{
			return (TDevice)((object)InputSystem.GetDevice(typeof(TDevice)));
		}

		public static InputDevice GetDevice(Type type)
		{
			InputDevice inputDevice = null;
			double num = -1.0;
			foreach (InputDevice inputDevice2 in InputSystem.devices)
			{
				if (type.IsInstanceOfType(inputDevice2) && (inputDevice == null || inputDevice2.m_LastUpdateTimeInternal > num))
				{
					inputDevice = inputDevice2;
					num = inputDevice.m_LastUpdateTimeInternal;
				}
			}
			return inputDevice;
		}

		public static TDevice GetDevice<TDevice>(InternedString usage) where TDevice : InputDevice
		{
			TDevice tdevice = default(TDevice);
			double num = -1.0;
			foreach (InputDevice inputDevice in InputSystem.devices)
			{
				TDevice tdevice2 = inputDevice as TDevice;
				if (tdevice2 != null && tdevice2.usages.Contains(usage) && (tdevice == null || tdevice2.m_LastUpdateTimeInternal > num))
				{
					tdevice = tdevice2;
					num = tdevice.m_LastUpdateTimeInternal;
				}
			}
			return tdevice;
		}

		public static TDevice GetDevice<TDevice>(string usage) where TDevice : InputDevice
		{
			return InputSystem.GetDevice<TDevice>(new InternedString(usage));
		}

		public static InputDevice GetDeviceById(int deviceId)
		{
			return InputSystem.s_Manager.TryGetDeviceById(deviceId);
		}

		public static List<InputDeviceDescription> GetUnsupportedDevices()
		{
			List<InputDeviceDescription> list = new List<InputDeviceDescription>();
			InputSystem.GetUnsupportedDevices(list);
			return list;
		}

		public static int GetUnsupportedDevices(List<InputDeviceDescription> descriptions)
		{
			return InputSystem.s_Manager.GetUnsupportedDevices(descriptions);
		}

		public static void EnableDevice(InputDevice device)
		{
			InputSystem.s_Manager.EnableOrDisableDevice(device, true, InputManager.DeviceDisableScope.Everywhere);
		}

		public static void DisableDevice(InputDevice device, bool keepSendingEvents = false)
		{
			InputSystem.s_Manager.EnableOrDisableDevice(device, false, keepSendingEvents ? InputManager.DeviceDisableScope.InFrontendOnly : InputManager.DeviceDisableScope.Everywhere);
		}

		public static bool TrySyncDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!device.added)
			{
				throw new InvalidOperationException(string.Format("Device '{0}' has not been added", device));
			}
			return device.RequestSync();
		}

		public static void ResetDevice(InputDevice device, bool alsoResetDontResetControls = false)
		{
			InputSystem.s_Manager.ResetDevice(device, alsoResetDontResetControls, null);
		}

		[Obsolete("Use 'ResetDevice' instead.", false)]
		public static bool TryResetDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			return device.RequestReset();
		}

		public static void PauseHaptics()
		{
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				IHaptics haptics = devices[i] as IHaptics;
				if (haptics != null)
				{
					haptics.PauseHaptics();
				}
			}
		}

		public static void ResumeHaptics()
		{
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				IHaptics haptics = devices[i] as IHaptics;
				if (haptics != null)
				{
					haptics.ResumeHaptics();
				}
			}
		}

		public static void ResetHaptics()
		{
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				IHaptics haptics = devices[i] as IHaptics;
				if (haptics != null)
				{
					haptics.ResetHaptics();
				}
			}
		}

		public static void SetDeviceUsage(InputDevice device, string usage)
		{
			InputSystem.SetDeviceUsage(device, new InternedString(usage));
		}

		public static void SetDeviceUsage(InputDevice device, InternedString usage)
		{
			InputSystem.s_Manager.SetDeviceUsage(device, usage);
		}

		public static void AddDeviceUsage(InputDevice device, string usage)
		{
			InputSystem.s_Manager.AddDeviceUsage(device, new InternedString(usage));
		}

		public static void AddDeviceUsage(InputDevice device, InternedString usage)
		{
			InputSystem.s_Manager.AddDeviceUsage(device, usage);
		}

		public static void RemoveDeviceUsage(InputDevice device, string usage)
		{
			InputSystem.s_Manager.RemoveDeviceUsage(device, new InternedString(usage));
		}

		public static void RemoveDeviceUsage(InputDevice device, InternedString usage)
		{
			InputSystem.s_Manager.RemoveDeviceUsage(device, usage);
		}

		public static InputControl FindControl(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			ReadOnlyArray<InputDevice> devices = InputSystem.s_Manager.devices;
			int count = devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = InputControlPath.TryFindControl(devices[i], path, 0);
				if (inputControl != null)
				{
					return inputControl;
				}
			}
			return null;
		}

		public static InputControlList<InputControl> FindControls(string path)
		{
			return InputSystem.FindControls<InputControl>(path);
		}

		public static InputControlList<TControl> FindControls<TControl>(string path) where TControl : InputControl
		{
			InputControlList<TControl> result = default(InputControlList<TControl>);
			InputSystem.FindControls<TControl>(path, ref result);
			return result;
		}

		public static int FindControls<TControl>(string path, ref InputControlList<TControl> controls) where TControl : InputControl
		{
			return InputSystem.s_Manager.GetControls<TControl>(path, ref controls);
		}

		internal static bool isProcessingEvents
		{
			get
			{
				return InputSystem.s_Manager.isProcessingEvents;
			}
		}

		public static InputEventListener onEvent
		{
			get
			{
				return default(InputEventListener);
			}
			set
			{
			}
		}

		public static IObservable<InputControl> onAnyButtonPress
		{
			get
			{
				return from e in InputSystem.onEvent
				select e.GetFirstButtonPressOrNull(-1f, true) into c
				where c != null
				select c;
			}
		}

		public static void QueueEvent(InputEventPtr eventPtr)
		{
			if (!eventPtr.valid)
			{
				throw new ArgumentException("Received a null event pointer", "eventPtr");
			}
			InputSystem.s_Manager.QueueEvent(eventPtr);
		}

		public static void QueueEvent<TEvent>(ref TEvent inputEvent) where TEvent : struct, IInputEventTypeInfo
		{
			InputSystem.s_Manager.QueueEvent<TEvent>(ref inputEvent);
		}

		public unsafe static void QueueStateEvent<TState>(InputDevice device, TState state, double time = -1.0) where TState : struct, IInputStateTypeInfo
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.m_DeviceIndex == -1)
			{
				throw new InvalidOperationException(string.Format("Cannot queue state event for device '{0}' because device has not been added to system", device));
			}
			uint num = (uint)UnsafeUtility.SizeOf<TState>();
			if (num > 512U)
			{
				throw new ArgumentException(string.Format("Size of '{0}' exceeds maximum supported state size of {1}", typeof(TState).Name, 512), "state");
			}
			long num2 = (long)UnsafeUtility.SizeOf<StateEvent>() + (long)((ulong)num) - 1L;
			if (time < 0.0)
			{
				time = InputRuntime.s_Instance.currentTime;
			}
			else
			{
				time += InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
			InputSystem.StateEventBuffer stateEventBuffer;
			stateEventBuffer.stateEvent = new StateEvent
			{
				baseEvent = new InputEvent(1398030676, (int)num2, device.deviceId, time),
				stateFormat = state.format
			};
			UnsafeUtility.MemCpy((void*)(&stateEventBuffer.stateEvent.stateData.FixedElementField), UnsafeUtility.AddressOf<TState>(ref state), (long)((ulong)num));
			InputSystem.s_Manager.QueueEvent<StateEvent>(ref stateEventBuffer.stateEvent);
		}

		public unsafe static void QueueDeltaStateEvent<TDelta>(InputControl control, TDelta delta, double time = -1.0) where TDelta : struct
		{
			if (control == null)
			{
				throw new ArgumentNullException("control");
			}
			if (control.stateBlock.bitOffset != 0U)
			{
				throw new InvalidOperationException(string.Format("Cannot send delta state events against bitfield controls: {0}", control));
			}
			InputDevice device = control.device;
			if (device.m_DeviceIndex == -1)
			{
				throw new InvalidOperationException(string.Format("Cannot queue state event for control '{0}' on device '{1}' because device has not been added to system", control, device));
			}
			if (time < 0.0)
			{
				time = InputRuntime.s_Instance.currentTime;
			}
			else
			{
				time += InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
			uint num = (uint)UnsafeUtility.SizeOf<TDelta>();
			if (num > 512U)
			{
				throw new ArgumentException(string.Format("Size of state delta '{0}' exceeds maximum supported state size of {1}", typeof(TDelta).Name, 512), "delta");
			}
			if (num != control.stateBlock.alignedSizeInBytes)
			{
				throw new ArgumentException(string.Format("Size {0} of delta state of type {1} provided for control '{2}' does not match size {3} of control", new object[]
				{
					num,
					typeof(TDelta).Name,
					control,
					control.stateBlock.alignedSizeInBytes
				}), "delta");
			}
			long num2 = (long)UnsafeUtility.SizeOf<DeltaStateEvent>() + (long)((ulong)num) - 1L;
			InputSystem.DeltaStateEventBuffer deltaStateEventBuffer;
			deltaStateEventBuffer.stateEvent = new DeltaStateEvent
			{
				baseEvent = new InputEvent(1145852993, (int)num2, device.deviceId, time),
				stateFormat = device.stateBlock.format,
				stateOffset = control.m_StateBlock.byteOffset - device.m_StateBlock.byteOffset
			};
			UnsafeUtility.MemCpy((void*)(&deltaStateEventBuffer.stateEvent.stateData.FixedElementField), UnsafeUtility.AddressOf<TDelta>(ref delta), (long)((ulong)num));
			InputSystem.s_Manager.QueueEvent<DeltaStateEvent>(ref deltaStateEventBuffer.stateEvent);
		}

		public static void QueueConfigChangeEvent(InputDevice device, double time = -1.0)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.deviceId == 0)
			{
				throw new InvalidOperationException("Device has not been added");
			}
			if (time < 0.0)
			{
				time = InputRuntime.s_Instance.currentTime;
			}
			else
			{
				time += InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
			DeviceConfigurationEvent deviceConfigurationEvent = DeviceConfigurationEvent.Create(device.deviceId, time);
			InputSystem.s_Manager.QueueEvent<DeviceConfigurationEvent>(ref deviceConfigurationEvent);
		}

		public static void QueueTextEvent(InputDevice device, char character, double time = -1.0)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.deviceId == 0)
			{
				throw new InvalidOperationException("Device has not been added");
			}
			if (time < 0.0)
			{
				time = InputRuntime.s_Instance.currentTime;
			}
			else
			{
				time += InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			}
			TextEvent textEvent = TextEvent.Create(device.deviceId, character, time);
			InputSystem.s_Manager.QueueEvent<TextEvent>(ref textEvent);
		}

		public static void Update()
		{
			InputSystem.s_Manager.Update();
		}

		internal static void Update(InputUpdateType updateType)
		{
			if (updateType != InputUpdateType.None && (InputSystem.s_Manager.updateMask & updateType) == InputUpdateType.None)
			{
				throw new InvalidOperationException(string.Format("'{0}' updates are not enabled; InputSystem.settings.updateMode is set to '{1}'", updateType, InputSystem.settings.updateMode));
			}
			InputSystem.s_Manager.Update(updateType);
		}

		public static event Action onBeforeUpdate
		{
			add
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onBeforeUpdate += value;
				}
			}
			remove
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onBeforeUpdate -= value;
				}
			}
		}

		public static event Action onAfterUpdate
		{
			add
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onAfterUpdate += value;
				}
			}
			remove
			{
				InputManager obj = InputSystem.s_Manager;
				lock (obj)
				{
					InputSystem.s_Manager.onAfterUpdate -= value;
				}
			}
		}

		public static InputSettings settings
		{
			get
			{
				return InputSystem.s_Manager.settings;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (InputSystem.s_Manager.m_Settings == value)
				{
					return;
				}
				InputSystem.s_Manager.settings = value;
			}
		}

		public static event Action onSettingsChange
		{
			add
			{
				InputSystem.s_Manager.onSettingsChange += value;
			}
			remove
			{
				InputSystem.s_Manager.onSettingsChange -= value;
			}
		}

		private static void EnableActions()
		{
			if (InputSystem.actions == null)
			{
				return;
			}
			InputSystem.actions.Enable();
		}

		private static void DisableActions(bool triggerSetupChanged = false)
		{
			InputActionAsset actions = InputSystem.actions;
			if (actions == null)
			{
				return;
			}
			actions.Disable();
			if (triggerSetupChanged)
			{
				actions.OnSetupChanged();
			}
		}

		public static InputActionAsset actions
		{
			get
			{
				InputManager inputManager = InputSystem.s_Manager;
				if (inputManager == null)
				{
					return null;
				}
				return inputManager.actions;
			}
			set
			{
				if (Application.isPlaying)
				{
					throw new Exception("Attempted to set property InputSystem.actions during Play-mode which is not supported. Assigning this property is only allowed in Edit-mode.");
				}
				if (InputSystem.s_Manager.actions == value)
				{
					return;
				}
				value != null;
				InputSystem.s_Manager.actions = value;
			}
		}

		public static event Action onActionsChange
		{
			add
			{
				InputSystem.s_Manager.onActionsChange += value;
			}
			remove
			{
				InputSystem.s_Manager.onActionsChange -= value;
			}
		}

		public static event Action<object, InputActionChange> onActionChange
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputActionState.s_GlobalState.onActionChange.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				InputActionState.s_GlobalState.onActionChange.RemoveCallback(value);
			}
		}

		public static void RegisterInteraction(Type type, string name = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
				if (name.EndsWith("Interaction"))
				{
					name = name.Substring(0, name.Length - "Interaction".Length);
				}
			}
			InputSystem.s_Manager.interactions.AddTypeRegistration(name, type);
		}

		public static void RegisterInteraction<T>(string name = null)
		{
			InputSystem.RegisterInteraction(typeof(T), name);
		}

		public static Type TryGetInteraction(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return InputSystem.s_Manager.interactions.LookupTypeRegistration(name);
		}

		public static IEnumerable<string> ListInteractions()
		{
			return InputSystem.s_Manager.interactions.names;
		}

		public static void RegisterBindingComposite(Type type, string name)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (string.IsNullOrEmpty(name))
			{
				name = type.Name;
				if (name.EndsWith("Composite"))
				{
					name = name.Substring(0, name.Length - "Composite".Length);
				}
			}
			InputSystem.s_Manager.composites.AddTypeRegistration(name, type);
		}

		public static void RegisterBindingComposite<T>(string name = null)
		{
			InputSystem.RegisterBindingComposite(typeof(T), name);
		}

		public static Type TryGetBindingComposite(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			return InputSystem.s_Manager.composites.LookupTypeRegistration(name);
		}

		public static void DisableAllEnabledActions()
		{
			InputActionState.DisableAllActions();
		}

		public static List<InputAction> ListEnabledActions()
		{
			List<InputAction> list = new List<InputAction>();
			InputSystem.ListEnabledActions(list);
			return list;
		}

		public static int ListEnabledActions(List<InputAction> actions)
		{
			if (actions == null)
			{
				throw new ArgumentNullException("actions");
			}
			return InputActionState.FindAllEnabledActions(actions);
		}

		public static InputRemoting remoting
		{
			get
			{
				return InputSystem.s_Remote;
			}
		}

		public static Version version
		{
			get
			{
				return new Version("1.14.2");
			}
		}

		public static bool runInBackground
		{
			get
			{
				return InputSystem.s_Manager.m_Runtime.runInBackground;
			}
			set
			{
				InputSystem.s_Manager.m_Runtime.runInBackground = value;
			}
		}

		internal static float scrollWheelDeltaPerTick
		{
			get
			{
				return InputRuntime.s_Instance.scrollWheelDeltaPerTick;
			}
		}

		public static InputMetrics metrics
		{
			get
			{
				return InputSystem.s_Manager.metrics;
			}
		}

		static InputSystem()
		{
			InputSystem.InitializeInPlayer(null, null);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void RunInitializeInPlayer()
		{
			if (InputSystem.s_Manager == null)
			{
				InputSystem.InitializeInPlayer(null, null);
			}
		}

		internal static void EnsureInitialized()
		{
		}

		private static void InitializeInPlayer(IInputRuntime runtime = null, InputSettings settings = null)
		{
			if (settings == null)
			{
				settings = (Resources.FindObjectsOfTypeAll<InputSettings>().FirstOrDefault<InputSettings>() ?? ScriptableObject.CreateInstance<InputSettings>());
			}
			InputSystem.s_Manager = new InputManager();
			InputSystem.s_Manager.Initialize(runtime ?? NativeInputRuntime.instance, settings);
			InputSystem.PerformDefaultPluginInitialization();
			InputSystem.EnableActions();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RunInitialUpdate()
		{
			InputSystem.Update(InputUpdateType.None);
		}

		private static void PerformDefaultPluginInitialization()
		{
			UISupport.Initialize();
			XInputSupport.Initialize();
			DualShockSupport.Initialize();
			HIDSupport.Initialize();
			SwitchSupportHID.Initialize();
			XRSupport.Initialize();
		}

		internal const string kAssemblyVersion = "1.14.2";

		internal const string kDocUrl = "https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14";

		private static readonly ProfilerMarker k_InputResetMarker = new ProfilerMarker("InputSystem.Reset");

		internal static InputManager s_Manager;

		internal static InputRemoting s_Remote;

		private struct StateEventBuffer
		{
			public StateEvent stateEvent;

			public const int kMaxSize = 512;

			[FixedBuffer(typeof(byte), 511)]
			public InputSystem.StateEventBuffer.<data>e__FixedBuffer data;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 511)]
			public struct <data>e__FixedBuffer
			{
				public byte FixedElementField;
			}
		}

		private struct DeltaStateEventBuffer
		{
			public DeltaStateEvent stateEvent;

			public const int kMaxSize = 512;

			[FixedBuffer(typeof(byte), 511)]
			public InputSystem.DeltaStateEventBuffer.<data>e__FixedBuffer data;

			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 511)]
			public struct <data>e__FixedBuffer
			{
				public byte FixedElementField;
			}
		}
	}
}
