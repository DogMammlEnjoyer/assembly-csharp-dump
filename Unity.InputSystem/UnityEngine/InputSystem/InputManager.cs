using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal class InputManager
	{
		public ReadOnlyArray<InputDevice> devices
		{
			get
			{
				return new ReadOnlyArray<InputDevice>(this.m_Devices, 0, this.m_DevicesCount);
			}
		}

		public TypeTable processors
		{
			get
			{
				return this.m_Processors;
			}
		}

		public TypeTable interactions
		{
			get
			{
				return this.m_Interactions;
			}
		}

		public TypeTable composites
		{
			get
			{
				return this.m_Composites;
			}
		}

		public InputMetrics metrics
		{
			get
			{
				InputMetrics metrics = this.m_Metrics;
				metrics.currentNumDevices = this.m_DevicesCount;
				metrics.currentStateSizeInBytes = (int)this.m_StateBuffers.totalSize;
				metrics.currentControlCount = this.m_DevicesCount;
				for (int i = 0; i < this.m_DevicesCount; i++)
				{
					metrics.currentControlCount += this.m_Devices[i].allControls.Count;
				}
				metrics.currentLayoutCount = this.m_Layouts.layoutTypes.Count;
				metrics.currentLayoutCount += this.m_Layouts.layoutStrings.Count;
				metrics.currentLayoutCount += this.m_Layouts.layoutBuilders.Count;
				metrics.currentLayoutCount += this.m_Layouts.layoutOverrides.Count;
				return metrics;
			}
		}

		public InputSettings settings
		{
			get
			{
				return this.m_Settings;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (this.m_Settings == value)
				{
					return;
				}
				this.m_Settings = value;
				this.ApplySettings();
			}
		}

		public InputActionAsset actions
		{
			get
			{
				return this.m_Actions;
			}
			set
			{
				this.m_Actions = value;
				this.ApplyActions();
			}
		}

		public InputUpdateType updateMask
		{
			get
			{
				return this.m_UpdateMask;
			}
			set
			{
				if (this.m_UpdateMask == value)
				{
					return;
				}
				this.m_UpdateMask = value;
				if (this.m_DevicesCount > 0)
				{
					this.ReallocateStateBuffers();
				}
			}
		}

		public InputUpdateType defaultUpdateType
		{
			get
			{
				if (this.m_CurrentUpdate != InputUpdateType.None)
				{
					return this.m_CurrentUpdate;
				}
				return this.m_UpdateMask.GetUpdateTypeForPlayer();
			}
		}

		public InputSettings.ScrollDeltaBehavior scrollDeltaBehavior
		{
			get
			{
				return this.m_ScrollDeltaBehavior;
			}
			set
			{
				if (this.m_ScrollDeltaBehavior == value)
				{
					return;
				}
				this.m_ScrollDeltaBehavior = value;
				InputRuntime.s_Instance.normalizeScrollWheelDelta = (this.m_ScrollDeltaBehavior == InputSettings.ScrollDeltaBehavior.UniformAcrossAllPlatforms);
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
				if (value <= 0f)
				{
					throw new ArgumentException("Polling frequency must be greater than zero", "value");
				}
				this.m_PollingFrequency = value;
				if (this.m_Runtime != null)
				{
					this.m_Runtime.pollingFrequency = value;
				}
			}
		}

		public event Action<InputDevice, InputDeviceChange> onDeviceChange
		{
			add
			{
				this.m_DeviceChangeListeners.AddCallback(value);
			}
			remove
			{
				this.m_DeviceChangeListeners.RemoveCallback(value);
			}
		}

		public event Action<InputDevice, InputEventPtr> onDeviceStateChange
		{
			add
			{
				this.m_DeviceStateChangeListeners.AddCallback(value);
			}
			remove
			{
				this.m_DeviceStateChangeListeners.RemoveCallback(value);
			}
		}

		public event InputDeviceCommandDelegate onDeviceCommand
		{
			add
			{
				this.m_DeviceCommandCallbacks.AddCallback(value);
			}
			remove
			{
				this.m_DeviceCommandCallbacks.RemoveCallback(value);
			}
		}

		public event InputDeviceFindControlLayoutDelegate onFindControlLayoutForDevice
		{
			add
			{
				this.m_DeviceFindLayoutCallbacks.AddCallback(value);
				this.AddAvailableDevicesThatAreNowRecognized();
			}
			remove
			{
				this.m_DeviceFindLayoutCallbacks.RemoveCallback(value);
			}
		}

		public event Action<string, InputControlLayoutChange> onLayoutChange
		{
			add
			{
				this.m_LayoutChangeListeners.AddCallback(value);
			}
			remove
			{
				this.m_LayoutChangeListeners.RemoveCallback(value);
			}
		}

		public event Action<InputEventPtr, InputDevice> onEvent
		{
			add
			{
				this.m_EventListeners.AddCallback(value);
			}
			remove
			{
				this.m_EventListeners.RemoveCallback(value);
			}
		}

		public event Action onBeforeUpdate
		{
			add
			{
				this.InstallBeforeUpdateHookIfNecessary();
				this.m_BeforeUpdateListeners.AddCallback(value);
			}
			remove
			{
				this.m_BeforeUpdateListeners.RemoveCallback(value);
			}
		}

		public event Action onAfterUpdate
		{
			add
			{
				this.m_AfterUpdateListeners.AddCallback(value);
			}
			remove
			{
				this.m_AfterUpdateListeners.RemoveCallback(value);
			}
		}

		public event Action onSettingsChange
		{
			add
			{
				this.m_SettingsChangedListeners.AddCallback(value);
			}
			remove
			{
				this.m_SettingsChangedListeners.RemoveCallback(value);
			}
		}

		public event Action onActionsChange
		{
			add
			{
				this.m_ActionsChangedListeners.AddCallback(value);
			}
			remove
			{
				this.m_ActionsChangedListeners.RemoveCallback(value);
			}
		}

		public bool isProcessingEvents
		{
			get
			{
				return this.m_InputEventStream.isOpen;
			}
		}

		private bool gameIsPlaying
		{
			get
			{
				return true;
			}
		}

		private bool gameHasFocus
		{
			get
			{
				return this.m_HasFocus || this.gameShouldGetInputRegardlessOfFocus;
			}
		}

		private bool gameShouldGetInputRegardlessOfFocus
		{
			get
			{
				return this.m_Settings.backgroundBehavior == InputSettings.BackgroundBehavior.IgnoreFocus;
			}
		}

		public void RegisterControlLayout(string name, Type type)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = typeof(InputDevice).IsAssignableFrom(type);
			bool flag2 = typeof(InputControl).IsAssignableFrom(type);
			if (!flag && !flag2)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Types used as layouts have to be InputControls or InputDevices; '",
					type.Name,
					"' is a '",
					type.BaseType.Name,
					"'"
				}), "type");
			}
			InternedString internedString = new InternedString(name);
			bool isReplacement = this.m_Layouts.HasLayout(internedString);
			this.m_Layouts.layoutTypes[internedString] = type;
			string text = null;
			Type baseType = type.BaseType;
			while (text == null && baseType != typeof(InputControl))
			{
				foreach (KeyValuePair<InternedString, Type> keyValuePair in this.m_Layouts.layoutTypes)
				{
					if (keyValuePair.Value == baseType)
					{
						text = keyValuePair.Key;
						break;
					}
				}
				baseType = baseType.BaseType;
			}
			this.PerformLayoutPostRegistration(internedString, new InlinedArray<InternedString>(new InternedString(text)), isReplacement, flag, false);
		}

		public void RegisterControlLayout(string json, string name = null, bool isOverride = false)
		{
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException("json");
			}
			InternedString internedString;
			InlinedArray<InternedString> inlinedArray;
			InputDeviceMatcher matcher;
			InputControlLayout.ParseHeaderFieldsFromJson(json, out internedString, out inlinedArray, out matcher);
			InternedString internedString2 = new InternedString(name);
			if (internedString2.IsEmpty())
			{
				internedString2 = internedString;
				if (internedString2.IsEmpty())
				{
					throw new ArgumentException("Layout name has not been given and is not set in JSON layout", "name");
				}
			}
			if (isOverride && inlinedArray.length == 0)
			{
				throw new ArgumentException(string.Format("Layout override '{0}' must have 'extend' property mentioning layout to which to apply the overrides", internedString2), "json");
			}
			bool flag = this.m_Layouts.HasLayout(internedString2);
			if (flag && isOverride && !this.m_Layouts.layoutOverrideNames.Contains(internedString2))
			{
				throw new ArgumentException(string.Format("Failed to register layout override '{0}'", internedString2) + string.Format("since a layout named '{0}' already exist. Layout overrides must ", internedString2) + "have unique names with respect to existing layouts.");
			}
			this.m_Layouts.layoutStrings[internedString2] = json;
			if (isOverride)
			{
				this.m_Layouts.layoutOverrideNames.Add(internedString2);
				for (int i = 0; i < inlinedArray.length; i++)
				{
					InternedString key = inlinedArray[i];
					InternedString[] value;
					this.m_Layouts.layoutOverrides.TryGetValue(key, out value);
					if (!flag)
					{
						ArrayHelpers.Append<InternedString>(ref value, internedString2);
					}
					this.m_Layouts.layoutOverrides[key] = value;
				}
			}
			this.PerformLayoutPostRegistration(internedString2, inlinedArray, flag, false, isOverride);
			if (!matcher.empty)
			{
				this.RegisterControlLayoutMatcher(internedString2, matcher);
			}
		}

		public void RegisterControlLayoutBuilder(Func<InputControlLayout> method, string name, string baseLayout = null)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			InternedString internedString = new InternedString(name);
			InternedString value = new InternedString(baseLayout);
			bool isReplacement = this.m_Layouts.HasLayout(internedString);
			this.m_Layouts.layoutBuilders[internedString] = method;
			this.PerformLayoutPostRegistration(internedString, new InlinedArray<InternedString>(value), isReplacement, false, false);
		}

		private void PerformLayoutPostRegistration(InternedString layoutName, InlinedArray<InternedString> baseLayouts, bool isReplacement, bool isKnownToBeDeviceLayout = false, bool isOverride = false)
		{
			this.m_LayoutRegistrationVersion++;
			InputControlLayout.s_CacheInstance.Clear();
			if (!isOverride && baseLayouts.length > 0)
			{
				if (baseLayouts.length > 1)
				{
					throw new NotSupportedException(string.Format("Layout '{0}' has multiple base layouts; this is only supported on layout overrides", layoutName));
				}
				InternedString value = baseLayouts[0];
				if (!value.IsEmpty())
				{
					this.m_Layouts.baseLayoutTable[layoutName] = value;
				}
			}
			this.m_Layouts.precompiledLayouts.Remove(layoutName);
			if (this.m_Layouts.precompiledLayouts.Count > 0)
			{
				foreach (InternedString internedString in this.m_Layouts.precompiledLayouts.Keys.ToArray<InternedString>())
				{
					string metadata = this.m_Layouts.precompiledLayouts[internedString].metadata;
					if (isOverride)
					{
						for (int j = 0; j < baseLayouts.length; j++)
						{
							if (internedString == baseLayouts[j] || StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(metadata, baseLayouts[j], ';'))
							{
								this.m_Layouts.precompiledLayouts.Remove(internedString);
							}
						}
					}
					else if (StringHelpers.CharacterSeparatedListsHaveAtLeastOneCommonElement(metadata, layoutName, ';'))
					{
						this.m_Layouts.precompiledLayouts.Remove(internedString);
					}
				}
			}
			if (isOverride)
			{
				for (int k = 0; k < baseLayouts.length; k++)
				{
					this.RecreateDevicesUsingLayout(baseLayouts[k], isKnownToBeDeviceLayout);
				}
			}
			else
			{
				this.RecreateDevicesUsingLayout(layoutName, isKnownToBeDeviceLayout);
			}
			InputControlLayoutChange argument = isReplacement ? InputControlLayoutChange.Replaced : InputControlLayoutChange.Added;
			DelegateHelpers.InvokeCallbacksSafe<string, InputControlLayoutChange>(ref this.m_LayoutChangeListeners, layoutName.ToString(), argument, InputManager.k_InputOnLayoutChangeMarker, "InputSystem.onLayoutChange", null);
		}

		public void RegisterPrecompiledLayout<TDevice>(string metadata) where TDevice : InputDevice, new()
		{
			if (metadata == null)
			{
				throw new ArgumentNullException("metadata");
			}
			Type baseType = typeof(TDevice).BaseType;
			InternedString internedString = this.FindOrRegisterDeviceLayoutForType(baseType);
			Dictionary<InternedString, InputControlLayout.Collection.PrecompiledLayout> precompiledLayouts = this.m_Layouts.precompiledLayouts;
			InternedString key = internedString;
			InputControlLayout.Collection.PrecompiledLayout value = default(InputControlLayout.Collection.PrecompiledLayout);
			value.factoryMethod = (() => Activator.CreateInstance<TDevice>());
			value.metadata = metadata;
			precompiledLayouts[key] = value;
		}

		private void RecreateDevicesUsingLayout(InternedString layout, bool isKnownToBeDeviceLayout = false)
		{
			if (this.m_DevicesCount == 0)
			{
				return;
			}
			List<InputDevice> list = null;
			for (int i = 0; i < this.m_DevicesCount; i++)
			{
				InputDevice inputDevice = this.m_Devices[i];
				bool flag;
				if (isKnownToBeDeviceLayout)
				{
					flag = this.IsControlUsingLayout(inputDevice, layout);
				}
				else
				{
					flag = this.IsControlOrChildUsingLayoutRecursive(inputDevice, layout);
				}
				if (flag)
				{
					if (list == null)
					{
						list = new List<InputDevice>();
					}
					list.Add(inputDevice);
				}
			}
			if (list == null)
			{
				return;
			}
			using (InputDeviceBuilder.Ref())
			{
				for (int j = 0; j < list.Count; j++)
				{
					InputDevice inputDevice2 = list[j];
					this.RecreateDevice(inputDevice2, inputDevice2.m_Layout);
				}
			}
		}

		private bool IsControlOrChildUsingLayoutRecursive(InputControl control, InternedString layout)
		{
			if (this.IsControlUsingLayout(control, layout))
			{
				return true;
			}
			ReadOnlyArray<InputControl> children = control.children;
			for (int i = 0; i < children.Count; i++)
			{
				if (this.IsControlOrChildUsingLayoutRecursive(children[i], layout))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsControlUsingLayout(InputControl control, InternedString layout)
		{
			if (control.layout == layout)
			{
				return true;
			}
			InternedString layout2 = control.m_Layout;
			while (this.m_Layouts.baseLayoutTable.TryGetValue(layout2, out layout2))
			{
				if (layout2 == layout)
				{
					return true;
				}
			}
			return false;
		}

		public void RegisterControlLayoutMatcher(string layoutName, InputDeviceMatcher matcher)
		{
			if (string.IsNullOrEmpty(layoutName))
			{
				throw new ArgumentNullException("layoutName");
			}
			if (matcher.empty)
			{
				throw new ArgumentException("Matcher cannot be empty", "matcher");
			}
			InternedString layout = new InternedString(layoutName);
			this.m_Layouts.AddMatcher(layout, matcher);
			this.RecreateDevicesUsingLayoutWithInferiorMatch(matcher);
			this.AddAvailableDevicesMatchingDescription(matcher, layout);
		}

		public void RegisterControlLayoutMatcher(Type type, InputDeviceMatcher matcher)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (matcher.empty)
			{
				throw new ArgumentException("Matcher cannot be empty", "matcher");
			}
			InternedString str = this.m_Layouts.TryFindLayoutForType(type);
			if (str.IsEmpty())
			{
				throw new ArgumentException("Type '" + type.Name + "' has not been registered as a control layout", "type");
			}
			this.RegisterControlLayoutMatcher(str, matcher);
		}

		private void RecreateDevicesUsingLayoutWithInferiorMatch(InputDeviceMatcher deviceMatcher)
		{
			if (this.m_DevicesCount == 0)
			{
				return;
			}
			using (InputDeviceBuilder.Ref())
			{
				int num = this.m_DevicesCount;
				for (int i = 0; i < num; i++)
				{
					InputDevice inputDevice = this.m_Devices[i];
					InputDeviceDescription description = inputDevice.description;
					if (!description.empty && deviceMatcher.MatchPercentage(description) > 0f)
					{
						InternedString internedString = this.TryFindMatchingControlLayout(ref description, inputDevice.deviceId);
						if (internedString != inputDevice.m_Layout)
						{
							inputDevice.m_Description = description;
							this.RecreateDevice(inputDevice, internedString);
							i--;
							num--;
						}
					}
				}
			}
		}

		private void RecreateDevice(InputDevice oldDevice, InternedString newLayout)
		{
			this.RemoveDevice(oldDevice, true);
			InputDevice inputDevice = InputDevice.Build<InputDevice>(newLayout, oldDevice.m_Variants, oldDevice.m_Description, false);
			inputDevice.m_DeviceId = oldDevice.m_DeviceId;
			inputDevice.m_Description = oldDevice.m_Description;
			if (oldDevice.native)
			{
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Native;
			}
			if (oldDevice.remote)
			{
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Remote;
			}
			if (!oldDevice.enabled)
			{
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledStateHasBeenQueriedFromRuntime;
				inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.DisabledInFrontend;
			}
			this.AddDevice(inputDevice);
		}

		private void AddAvailableDevicesMatchingDescription(InputDeviceMatcher matcher, InternedString layout)
		{
			for (int i = 0; i < this.m_AvailableDeviceCount; i++)
			{
				if (!this.m_AvailableDevices[i].isRemoved)
				{
					int deviceId = this.m_AvailableDevices[i].deviceId;
					if (this.TryGetDeviceById(deviceId) == null && matcher.MatchPercentage(this.m_AvailableDevices[i].description) > 0f)
					{
						try
						{
							this.AddDevice(layout, deviceId, null, this.m_AvailableDevices[i].description, this.m_AvailableDevices[i].isNative ? InputDevice.DeviceFlags.Native : ((InputDevice.DeviceFlags)0), default(InternedString));
						}
						catch (Exception ex)
						{
							Debug.LogError(string.Format("Layout '{0}' matches existing device '{1}' but failed to instantiate: {2}", layout, this.m_AvailableDevices[i].description, ex));
							Debug.LogException(ex);
							goto IL_E8;
						}
						EnableDeviceCommand enableDeviceCommand = EnableDeviceCommand.Create();
						this.m_Runtime.DeviceCommand(deviceId, ref enableDeviceCommand);
					}
				}
				IL_E8:;
			}
		}

		public void RemoveControlLayout(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			InternedString internedString = new InternedString(name);
			int i = 0;
			while (i < this.m_DevicesCount)
			{
				InputDevice inputDevice = this.m_Devices[i];
				if (this.IsControlOrChildUsingLayoutRecursive(inputDevice, internedString))
				{
					this.RemoveDevice(inputDevice, true);
				}
				else
				{
					i++;
				}
			}
			this.m_Layouts.layoutTypes.Remove(internedString);
			this.m_Layouts.layoutStrings.Remove(internedString);
			this.m_Layouts.layoutBuilders.Remove(internedString);
			this.m_Layouts.baseLayoutTable.Remove(internedString);
			this.m_LayoutRegistrationVersion++;
			DelegateHelpers.InvokeCallbacksSafe<string, InputControlLayoutChange>(ref this.m_LayoutChangeListeners, name, InputControlLayoutChange.Removed, InputManager.k_InputOnLayoutChangeMarker, "InputSystem.onLayoutChange", null);
		}

		public InputControlLayout TryLoadControlLayout(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!typeof(InputControl).IsAssignableFrom(type))
			{
				throw new ArgumentException("Type '" + type.Name + "' is not an InputControl", "type");
			}
			InternedString name = this.m_Layouts.TryFindLayoutForType(type);
			if (name.IsEmpty())
			{
				throw new ArgumentException("Type '" + type.Name + "' has not been registered as a control layout", "type");
			}
			return this.m_Layouts.TryLoadLayout(name, null);
		}

		public InputControlLayout TryLoadControlLayout(InternedString name)
		{
			return this.m_Layouts.TryLoadLayout(name, null);
		}

		public InternedString TryFindMatchingControlLayout(ref InputDeviceDescription deviceDescription, int deviceId = 0)
		{
			InternedString internedString = new InternedString(string.Empty);
			try
			{
				internedString = this.m_Layouts.TryFindMatchingLayout(deviceDescription);
				if (internedString.IsEmpty() && !string.IsNullOrEmpty(deviceDescription.deviceClass))
				{
					InternedString layoutName = new InternedString(deviceDescription.deviceClass);
					Type controlTypeForLayout = this.m_Layouts.GetControlTypeForLayout(layoutName);
					if (controlTypeForLayout != null && typeof(InputDevice).IsAssignableFrom(controlTypeForLayout))
					{
						internedString = new InternedString(deviceDescription.deviceClass);
					}
				}
				if (this.m_DeviceFindLayoutCallbacks.length > 0)
				{
					if (this.m_DeviceFindExecuteCommandDelegate == null)
					{
						this.m_DeviceFindExecuteCommandDelegate = delegate(ref InputDeviceCommand commandRef)
						{
							if (this.m_DeviceFindExecuteCommandDeviceId == 0)
							{
								return -1L;
							}
							return this.m_Runtime.DeviceCommand(this.m_DeviceFindExecuteCommandDeviceId, ref commandRef);
						};
					}
					this.m_DeviceFindExecuteCommandDeviceId = deviceId;
					bool flag = false;
					this.m_DeviceFindLayoutCallbacks.LockForChanges();
					for (int i = 0; i < this.m_DeviceFindLayoutCallbacks.length; i++)
					{
						try
						{
							string text = this.m_DeviceFindLayoutCallbacks[i](ref deviceDescription, internedString, this.m_DeviceFindExecuteCommandDelegate);
							if (!string.IsNullOrEmpty(text) && !flag)
							{
								internedString = new InternedString(text);
								flag = true;
							}
						}
						catch (Exception ex)
						{
							Debug.LogError(ex.GetType().Name + " while executing 'InputSystem.onFindLayoutForDevice' callbacks");
							Debug.LogException(ex);
						}
					}
					this.m_DeviceFindLayoutCallbacks.UnlockForChanges();
				}
			}
			finally
			{
			}
			return internedString;
		}

		private InternedString FindOrRegisterDeviceLayoutForType(Type type)
		{
			InternedString result = this.m_Layouts.TryFindLayoutForType(type);
			if (result.IsEmpty() && result.IsEmpty())
			{
				result = new InternedString(type.Name);
				this.RegisterControlLayout(type.Name, type);
			}
			return result;
		}

		private bool IsDeviceLayoutMarkedAsSupportedInSettings(InternedString layoutName)
		{
			ReadOnlyArray<string> supportedDevices = this.m_Settings.supportedDevices;
			if (supportedDevices.Count == 0)
			{
				return true;
			}
			for (int i = 0; i < supportedDevices.Count; i++)
			{
				InternedString internedString = new InternedString(supportedDevices[i]);
				if (layoutName == internedString || this.m_Layouts.IsBasedOn(internedString, layoutName))
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerable<string> ListControlLayouts(string basedOn = null)
		{
			if (!string.IsNullOrEmpty(basedOn))
			{
				InternedString internedBasedOn = new InternedString(basedOn);
				foreach (KeyValuePair<InternedString, Type> keyValuePair in this.m_Layouts.layoutTypes)
				{
					if (this.m_Layouts.IsBasedOn(internedBasedOn, keyValuePair.Key))
					{
						yield return keyValuePair.Key;
					}
				}
				Dictionary<InternedString, Type>.Enumerator enumerator = default(Dictionary<InternedString, Type>.Enumerator);
				foreach (KeyValuePair<InternedString, string> keyValuePair2 in this.m_Layouts.layoutStrings)
				{
					if (this.m_Layouts.IsBasedOn(internedBasedOn, keyValuePair2.Key))
					{
						yield return keyValuePair2.Key;
					}
				}
				Dictionary<InternedString, string>.Enumerator enumerator2 = default(Dictionary<InternedString, string>.Enumerator);
				foreach (KeyValuePair<InternedString, Func<InputControlLayout>> keyValuePair3 in this.m_Layouts.layoutBuilders)
				{
					if (this.m_Layouts.IsBasedOn(internedBasedOn, keyValuePair3.Key))
					{
						yield return keyValuePair3.Key;
					}
				}
				Dictionary<InternedString, Func<InputControlLayout>>.Enumerator enumerator3 = default(Dictionary<InternedString, Func<InputControlLayout>>.Enumerator);
				internedBasedOn = default(InternedString);
			}
			else
			{
				foreach (KeyValuePair<InternedString, Type> keyValuePair4 in this.m_Layouts.layoutTypes)
				{
					yield return keyValuePair4.Key;
				}
				Dictionary<InternedString, Type>.Enumerator enumerator = default(Dictionary<InternedString, Type>.Enumerator);
				foreach (KeyValuePair<InternedString, string> keyValuePair5 in this.m_Layouts.layoutStrings)
				{
					yield return keyValuePair5.Key;
				}
				Dictionary<InternedString, string>.Enumerator enumerator2 = default(Dictionary<InternedString, string>.Enumerator);
				foreach (KeyValuePair<InternedString, Func<InputControlLayout>> keyValuePair6 in this.m_Layouts.layoutBuilders)
				{
					yield return keyValuePair6.Key;
				}
				Dictionary<InternedString, Func<InputControlLayout>>.Enumerator enumerator3 = default(Dictionary<InternedString, Func<InputControlLayout>>.Enumerator);
			}
			yield break;
			yield break;
		}

		public int GetControls<TControl>(string path, ref InputControlList<TControl> controls) where TControl : InputControl
		{
			if (string.IsNullOrEmpty(path))
			{
				return 0;
			}
			if (this.m_DevicesCount == 0)
			{
				return 0;
			}
			int devicesCount = this.m_DevicesCount;
			int num = 0;
			for (int i = 0; i < devicesCount; i++)
			{
				InputDevice control = this.m_Devices[i];
				num += InputControlPath.TryFindControls<TControl>(control, path, 0, ref controls);
			}
			return num;
		}

		public void SetDeviceUsage(InputDevice device, InternedString usage)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.usages.Count == 1 && device.usages[0] == usage)
			{
				return;
			}
			if (device.usages.Count == 0 && usage.IsEmpty())
			{
				return;
			}
			device.ClearDeviceUsages();
			if (!usage.IsEmpty())
			{
				device.AddDeviceUsage(usage);
			}
			this.NotifyUsageChanged(device);
		}

		public void AddDeviceUsage(InputDevice device, InternedString usage)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (usage.IsEmpty())
			{
				throw new ArgumentException("Usage string cannot be empty", "usage");
			}
			if (device.usages.Contains(usage))
			{
				return;
			}
			device.AddDeviceUsage(usage);
			this.NotifyUsageChanged(device);
		}

		public void RemoveDeviceUsage(InputDevice device, InternedString usage)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (usage.IsEmpty())
			{
				throw new ArgumentException("Usage string cannot be empty", "usage");
			}
			if (!device.usages.Contains(usage))
			{
				return;
			}
			device.RemoveDeviceUsage(usage);
			this.NotifyUsageChanged(device);
		}

		private void NotifyUsageChanged(InputDevice device)
		{
			InputActionState.OnDeviceChange(device, InputDeviceChange.UsageChanged);
			DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, device, InputDeviceChange.UsageChanged, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
			device.MakeCurrent();
		}

		public InputDevice AddDevice(Type type, string name = null)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			InternedString str = this.FindOrRegisterDeviceLayoutForType(type);
			return this.AddDevice(str, name, default(InternedString));
		}

		public InputDevice AddDevice(string layout, string name = null, InternedString variants = default(InternedString))
		{
			if (string.IsNullOrEmpty(layout))
			{
				throw new ArgumentNullException("layout");
			}
			InputDevice inputDevice = InputDevice.Build<InputDevice>(layout, variants, default(InputDeviceDescription), false);
			if (!string.IsNullOrEmpty(name))
			{
				inputDevice.m_Name = new InternedString(name);
			}
			this.AddDevice(inputDevice);
			return inputDevice;
		}

		private InputDevice AddDevice(InternedString layout, int deviceId, string deviceName = null, InputDeviceDescription deviceDescription = default(InputDeviceDescription), InputDevice.DeviceFlags deviceFlags = (InputDevice.DeviceFlags)0, InternedString variants = default(InternedString))
		{
			string layoutName = new InternedString(layout);
			InputDeviceDescription deviceDescription2 = deviceDescription;
			InputDevice inputDevice = InputDevice.Build<InputDevice>(layoutName, variants, deviceDescription2, false);
			inputDevice.m_DeviceId = deviceId;
			inputDevice.m_Description = deviceDescription;
			inputDevice.m_DeviceFlags |= deviceFlags;
			if (!string.IsNullOrEmpty(deviceName))
			{
				inputDevice.m_Name = new InternedString(deviceName);
			}
			if (!string.IsNullOrEmpty(deviceDescription.product))
			{
				inputDevice.m_DisplayName = deviceDescription.product;
			}
			this.AddDevice(inputDevice);
			return inputDevice;
		}

		public void AddDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (string.IsNullOrEmpty(device.layout))
			{
				throw new InvalidOperationException("Device has no associated layout");
			}
			if (ArrayHelpers.Contains<InputDevice>(this.m_Devices, device))
			{
				return;
			}
			this.MakeDeviceNameUnique(device);
			this.AssignUniqueDeviceId(device);
			device.m_DeviceIndex = ArrayHelpers.AppendWithCapacity<InputDevice>(ref this.m_Devices, ref this.m_DevicesCount, device, 10);
			this.m_DevicesById[device.deviceId] = device;
			device.m_StateBlock.byteOffset = uint.MaxValue;
			this.ReallocateStateBuffers();
			this.InitializeDeviceState(device);
			this.m_Metrics.maxNumDevices = Mathf.Max(this.m_DevicesCount, this.m_Metrics.maxNumDevices);
			this.m_Metrics.maxStateSizeInBytes = Mathf.Max((int)this.m_StateBuffers.totalSize, this.m_Metrics.maxStateSizeInBytes);
			for (int i = 0; i < this.m_AvailableDeviceCount; i++)
			{
				if (this.m_AvailableDevices[i].deviceId == device.deviceId)
				{
					this.m_AvailableDevices[i].isRemoved = false;
				}
			}
			if (true && !this.gameHasFocus && this.m_Settings.backgroundBehavior != InputSettings.BackgroundBehavior.IgnoreFocus && this.m_Runtime.runInBackground && device.QueryEnabledStateFromRuntime() && !this.ShouldRunDeviceInBackground(device))
			{
				this.EnableOrDisableDevice(device, false, InputManager.DeviceDisableScope.TemporaryWhilePlayerIsInBackground);
			}
			InputActionState.OnDeviceChange(device, InputDeviceChange.Added);
			IInputUpdateCallbackReceiver inputUpdateCallbackReceiver = device as IInputUpdateCallbackReceiver;
			if (inputUpdateCallbackReceiver != null)
			{
				this.onBeforeUpdate += inputUpdateCallbackReceiver.OnUpdate;
			}
			if (device is IInputStateCallbackReceiver)
			{
				this.InstallBeforeUpdateHookIfNecessary();
				device.m_DeviceFlags |= InputDevice.DeviceFlags.HasStateCallbacks;
				this.m_HaveDevicesWithStateCallbackReceivers = true;
			}
			if (device is IEventMerger)
			{
				device.hasEventMerger = true;
			}
			if (device is IEventPreProcessor)
			{
				device.hasEventPreProcessor = true;
			}
			if (device.updateBeforeRender)
			{
				this.updateMask |= InputUpdateType.BeforeRender;
			}
			device.NotifyAdded();
			device.MakeCurrent();
			DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, device, InputDeviceChange.Added, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
			if (device.enabled)
			{
				device.RequestSync();
			}
			device.SetOptimizedControlDataTypeRecursively();
		}

		public InputDevice AddDevice(InputDeviceDescription description)
		{
			return this.AddDevice(description, true, null, 0, (InputDevice.DeviceFlags)0);
		}

		public InputDevice AddDevice(InputDeviceDescription description, bool throwIfNoLayoutFound, string deviceName = null, int deviceId = 0, InputDevice.DeviceFlags deviceFlags = (InputDevice.DeviceFlags)0)
		{
			InternedString layout = this.TryFindMatchingControlLayout(ref description, deviceId);
			if (!layout.IsEmpty())
			{
				InputDevice inputDevice = this.AddDevice(layout, deviceId, deviceName, description, deviceFlags, default(InternedString));
				inputDevice.m_Description = description;
				return inputDevice;
			}
			if (throwIfNoLayoutFound)
			{
				throw new ArgumentException(string.Format("Cannot find layout matching device description '{0}'", description), "description");
			}
			if (deviceId != 0)
			{
				DisableDeviceCommand disableDeviceCommand = DisableDeviceCommand.Create();
				this.m_Runtime.DeviceCommand(deviceId, ref disableDeviceCommand);
			}
			return null;
		}

		public InputDevice AddDevice(InputDeviceDescription description, InternedString layout, string deviceName = null, int deviceId = 0, InputDevice.DeviceFlags deviceFlags = (InputDevice.DeviceFlags)0)
		{
			InputDevice result;
			try
			{
				InputDevice inputDevice = this.AddDevice(layout, deviceId, deviceName, description, deviceFlags, default(InternedString));
				inputDevice.m_Description = description;
				result = inputDevice;
			}
			finally
			{
			}
			return result;
		}

		public void RemoveDevice(InputDevice device, bool keepOnListOfAvailableDevices = false)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (device.m_DeviceIndex == -1)
			{
				return;
			}
			this.RemoveStateChangeMonitors(device);
			int deviceIndex = device.m_DeviceIndex;
			int deviceId = device.deviceId;
			if (deviceIndex < this.m_StateChangeMonitors.LengthSafe<InputManager.StateChangeMonitorsForDevice>())
			{
				int num = this.m_StateChangeMonitors.Length;
				this.m_StateChangeMonitors.EraseAtWithCapacity(ref num, deviceIndex);
			}
			this.m_Devices.EraseAtWithCapacity(ref this.m_DevicesCount, deviceIndex);
			this.m_DevicesById.Remove(deviceId);
			if (this.m_Devices != null)
			{
				this.ReallocateStateBuffers();
			}
			else
			{
				this.m_StateBuffers.FreeAll();
			}
			for (int i = deviceIndex; i < this.m_DevicesCount; i++)
			{
				this.m_Devices[i].m_DeviceIndex--;
			}
			device.m_DeviceIndex = -1;
			int j = 0;
			while (j < this.m_AvailableDeviceCount)
			{
				if (this.m_AvailableDevices[j].deviceId == deviceId)
				{
					if (keepOnListOfAvailableDevices)
					{
						this.m_AvailableDevices[j].isRemoved = true;
						break;
					}
					this.m_AvailableDevices.EraseAtWithCapacity(ref this.m_AvailableDeviceCount, j);
					break;
				}
				else
				{
					j++;
				}
			}
			device.BakeOffsetIntoStateBlockRecursive((uint)(-(uint)((ulong)device.m_StateBlock.byteOffset)));
			InputActionState.OnDeviceChange(device, InputDeviceChange.Removed);
			IInputUpdateCallbackReceiver inputUpdateCallbackReceiver = device as IInputUpdateCallbackReceiver;
			if (inputUpdateCallbackReceiver != null)
			{
				this.onBeforeUpdate -= inputUpdateCallbackReceiver.OnUpdate;
			}
			if (device.updateBeforeRender)
			{
				bool flag = false;
				for (int k = 0; k < this.m_DevicesCount; k++)
				{
					if (this.m_Devices[k].updateBeforeRender)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.updateMask &= ~InputUpdateType.BeforeRender;
				}
			}
			device.NotifyRemoved();
			DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, device, InputDeviceChange.Removed, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
			InputDevice device2 = InputSystem.GetDevice(device.GetType());
			if (device2 == null)
			{
				return;
			}
			device2.MakeCurrent();
		}

		public void FlushDisconnectedDevices()
		{
			this.m_DisconnectedDevices.Clear(this.m_DisconnectedDevicesCount);
			this.m_DisconnectedDevicesCount = 0;
		}

		public unsafe void ResetDevice(InputDevice device, bool alsoResetDontResetControls = false, bool? issueResetCommand = null)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (!device.added)
			{
				throw new InvalidOperationException(string.Format("Device '{0}' has not been added to the system", device));
			}
			bool flag = alsoResetDontResetControls || !device.hasDontResetControls;
			InputDeviceChange inputDeviceChange = flag ? InputDeviceChange.HardReset : InputDeviceChange.SoftReset;
			InputActionState.OnDeviceChange(device, inputDeviceChange);
			DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, device, inputDeviceChange, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
			if (!alsoResetDontResetControls)
			{
				ICustomDeviceReset customDeviceReset = device as ICustomDeviceReset;
				if (customDeviceReset != null)
				{
					customDeviceReset.Reset();
					goto IL_1BA;
				}
			}
			void* defaultStatePtr = device.defaultStatePtr;
			uint alignedSizeInBytes = device.stateBlock.alignedSizeInBytes;
			using (NativeArray<byte> nativeArray = new NativeArray<byte>((int)(24U + alignedSizeInBytes), Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				StateEvent* unsafePtr = (StateEvent*)nativeArray.GetUnsafePtr<byte>();
				void* state = unsafePtr->state;
				double currentTime = this.m_Runtime.currentTime;
				ref InputStateBlock ptr = ref device.m_StateBlock;
				unsafePtr->baseEvent.type = 1398030676;
				unsafePtr->baseEvent.sizeInBytes = 24U + alignedSizeInBytes;
				unsafePtr->baseEvent.time = currentTime;
				unsafePtr->baseEvent.deviceId = device.deviceId;
				unsafePtr->baseEvent.eventId = -1;
				unsafePtr->stateFormat = device.m_StateBlock.format;
				if (flag)
				{
					UnsafeUtility.MemCpy(state, (void*)((byte*)defaultStatePtr + ptr.byteOffset), (long)((ulong)alignedSizeInBytes));
				}
				else
				{
					void* currentStatePtr = device.currentStatePtr;
					void* resetMaskBuffer = this.m_StateBuffers.resetMaskBuffer;
					UnsafeUtility.MemCpy(state, (void*)((byte*)currentStatePtr + ptr.byteOffset), (long)((ulong)alignedSizeInBytes));
					MemoryHelpers.MemCpyMasked(state, (void*)((byte*)defaultStatePtr + ptr.byteOffset), (int)alignedSizeInBytes, (void*)((byte*)resetMaskBuffer + ptr.byteOffset));
				}
				this.UpdateState(device, this.defaultUpdateType, state, 0U, alignedSizeInBytes, currentTime, new InputEventPtr((InputEvent*)unsafePtr));
			}
			IL_1BA:
			bool flag2 = flag;
			if (issueResetCommand != null)
			{
				flag2 = issueResetCommand.Value;
			}
			if (flag2)
			{
				device.RequestReset();
			}
		}

		public InputDevice TryGetDevice(string nameOrLayout)
		{
			if (string.IsNullOrEmpty(nameOrLayout))
			{
				throw new ArgumentException("Name is null or empty.", "nameOrLayout");
			}
			if (this.m_DevicesCount == 0)
			{
				return null;
			}
			string b = nameOrLayout.ToLower();
			for (int i = 0; i < this.m_DevicesCount; i++)
			{
				InputDevice inputDevice = this.m_Devices[i];
				if (inputDevice.m_Name.ToLower() == b || inputDevice.m_Layout.ToLower() == b)
				{
					return inputDevice;
				}
			}
			return null;
		}

		public InputDevice GetDevice(string nameOrLayout)
		{
			InputDevice inputDevice = this.TryGetDevice(nameOrLayout);
			if (inputDevice == null)
			{
				throw new ArgumentException("Cannot find device with name or layout '" + nameOrLayout + "'", "nameOrLayout");
			}
			return inputDevice;
		}

		public InputDevice TryGetDevice(Type layoutType)
		{
			InternedString str = this.m_Layouts.TryFindLayoutForType(layoutType);
			if (str.IsEmpty())
			{
				return null;
			}
			return this.TryGetDevice(str);
		}

		public InputDevice TryGetDeviceById(int id)
		{
			InputDevice result;
			if (this.m_DevicesById.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		public int GetUnsupportedDevices(List<InputDeviceDescription> descriptions)
		{
			if (descriptions == null)
			{
				throw new ArgumentNullException("descriptions");
			}
			int num = 0;
			for (int i = 0; i < this.m_AvailableDeviceCount; i++)
			{
				if (this.TryGetDeviceById(this.m_AvailableDevices[i].deviceId) == null)
				{
					descriptions.Add(this.m_AvailableDevices[i].description);
					num++;
				}
			}
			return num;
		}

		public void EnableOrDisableDevice(InputDevice device, bool enable, InputManager.DeviceDisableScope scope = InputManager.DeviceDisableScope.Everywhere)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (enable)
			{
				switch (scope)
				{
				case InputManager.DeviceDisableScope.Everywhere:
					device.disabledWhileInBackground = false;
					if (!device.disabledInFrontend && !device.disabledInRuntime)
					{
						return;
					}
					if (device.disabledInRuntime)
					{
						device.ExecuteEnableCommand();
						device.disabledInRuntime = false;
					}
					if (device.disabledInFrontend)
					{
						if (!device.RequestSync())
						{
							this.ResetDevice(device, false, null);
						}
						device.disabledInFrontend = false;
					}
					break;
				case InputManager.DeviceDisableScope.InFrontendOnly:
					device.disabledWhileInBackground = false;
					if (!device.disabledInFrontend && device.disabledInRuntime)
					{
						return;
					}
					if (!device.disabledInRuntime)
					{
						device.ExecuteDisableCommand();
						device.disabledInRuntime = true;
					}
					if (device.disabledInFrontend)
					{
						if (!device.RequestSync())
						{
							this.ResetDevice(device, false, null);
						}
						device.disabledInFrontend = false;
					}
					break;
				case InputManager.DeviceDisableScope.TemporaryWhilePlayerIsInBackground:
					if (device.disabledWhileInBackground)
					{
						if (device.disabledInRuntime)
						{
							device.ExecuteEnableCommand();
							device.disabledInRuntime = false;
						}
						if (!device.RequestSync())
						{
							this.ResetDevice(device, false, null);
						}
						device.disabledWhileInBackground = false;
					}
					break;
				}
			}
			else
			{
				switch (scope)
				{
				case InputManager.DeviceDisableScope.Everywhere:
					device.disabledWhileInBackground = false;
					if (device.disabledInFrontend && device.disabledInRuntime)
					{
						return;
					}
					if (!device.disabledInRuntime)
					{
						device.ExecuteDisableCommand();
						device.disabledInRuntime = true;
					}
					if (!device.disabledInFrontend)
					{
						this.ResetDevice(device, false, new bool?(false));
						device.disabledInFrontend = true;
					}
					break;
				case InputManager.DeviceDisableScope.InFrontendOnly:
					device.disabledWhileInBackground = false;
					if (!device.disabledInRuntime && device.disabledInFrontend)
					{
						return;
					}
					if (device.disabledInRuntime)
					{
						device.ExecuteEnableCommand();
						device.disabledInRuntime = false;
					}
					if (!device.disabledInFrontend)
					{
						this.ResetDevice(device, false, new bool?(false));
						device.disabledInFrontend = true;
					}
					break;
				case InputManager.DeviceDisableScope.TemporaryWhilePlayerIsInBackground:
					if (device.disabledInFrontend || device.disabledWhileInBackground)
					{
						return;
					}
					device.disabledWhileInBackground = true;
					this.ResetDevice(device, false, new bool?(false));
					device.ExecuteDisableCommand();
					device.disabledInRuntime = true;
					break;
				}
			}
			InputDeviceChange argument = enable ? InputDeviceChange.Enabled : InputDeviceChange.Disabled;
			DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, device, argument, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
		}

		private unsafe void QueueEvent(InputEvent* eventPtr)
		{
			if (this.m_InputEventStream.isOpen)
			{
				this.m_InputEventStream.Write(eventPtr);
				return;
			}
			this.m_Runtime.QueueEvent(eventPtr);
		}

		public void QueueEvent(InputEventPtr ptr)
		{
			this.QueueEvent(ptr.data);
		}

		public unsafe void QueueEvent<TEvent>(ref TEvent inputEvent) where TEvent : struct, IInputEventTypeInfo
		{
			this.QueueEvent((InputEvent*)UnsafeUtility.AddressOf<TEvent>(ref inputEvent));
		}

		public void Update()
		{
			this.Update(this.defaultUpdateType);
		}

		public void Update(InputUpdateType updateType)
		{
			this.m_Runtime.Update(updateType);
		}

		internal void Initialize(IInputRuntime runtime, InputSettings settings)
		{
			this.m_Settings = settings;
			this.InitializeActions();
			this.InitializeData();
			this.InstallRuntime(runtime);
			this.InstallGlobals();
			this.ApplySettings();
			this.ApplyActions();
		}

		internal void Destroy()
		{
			for (int i = 0; i < this.m_DevicesCount; i++)
			{
				this.m_Devices[i].NotifyRemoved();
			}
			this.m_StateBuffers.FreeAll();
			this.UninstallGlobals();
			if (this.m_Settings != null && this.m_Settings.hideFlags == HideFlags.HideAndDontSave)
			{
				Object.DestroyImmediate(this.m_Settings);
			}
		}

		private void InitializeActions()
		{
			this.m_Actions = null;
			foreach (InputActionAsset inputActionAsset in Resources.FindObjectsOfTypeAll<InputActionAsset>())
			{
				if (inputActionAsset.m_IsProjectWide)
				{
					this.m_Actions = inputActionAsset;
					return;
				}
			}
		}

		internal void InitializeData()
		{
			this.m_Layouts.Allocate();
			this.m_Processors.Initialize();
			this.m_Interactions.Initialize();
			this.m_Composites.Initialize();
			this.m_DevicesById = new Dictionary<int, InputDevice>();
			this.m_UpdateMask = (InputUpdateType.Dynamic | InputUpdateType.Fixed);
			this.m_HasFocus = Application.isFocused;
			this.m_ScrollDeltaBehavior = InputSettings.ScrollDeltaBehavior.UniformAcrossAllPlatforms;
			this.m_PollingFrequency = 60f;
			this.RegisterControlLayout("Axis", typeof(AxisControl));
			this.RegisterControlLayout("Button", typeof(ButtonControl));
			this.RegisterControlLayout("DiscreteButton", typeof(DiscreteButtonControl));
			this.RegisterControlLayout("Key", typeof(KeyControl));
			this.RegisterControlLayout("Analog", typeof(AxisControl));
			this.RegisterControlLayout("Integer", typeof(IntegerControl));
			this.RegisterControlLayout("Digital", typeof(IntegerControl));
			this.RegisterControlLayout("Double", typeof(DoubleControl));
			this.RegisterControlLayout("Vector2", typeof(Vector2Control));
			this.RegisterControlLayout("Vector3", typeof(Vector3Control));
			this.RegisterControlLayout("Delta", typeof(DeltaControl));
			this.RegisterControlLayout("Quaternion", typeof(QuaternionControl));
			this.RegisterControlLayout("Stick", typeof(StickControl));
			this.RegisterControlLayout("Dpad", typeof(DpadControl));
			this.RegisterControlLayout("DpadAxis", typeof(DpadControl.DpadAxisControl));
			this.RegisterControlLayout("AnyKey", typeof(AnyKeyControl));
			this.RegisterControlLayout("Touch", typeof(TouchControl));
			this.RegisterControlLayout("TouchPhase", typeof(TouchPhaseControl));
			this.RegisterControlLayout("TouchPress", typeof(TouchPressControl));
			this.RegisterControlLayout("Gamepad", typeof(Gamepad));
			this.RegisterControlLayout("Joystick", typeof(Joystick));
			this.RegisterControlLayout("Keyboard", typeof(Keyboard));
			this.RegisterControlLayout("Pointer", typeof(Pointer));
			this.RegisterControlLayout("Mouse", typeof(Mouse));
			this.RegisterControlLayout("Pen", typeof(Pen));
			this.RegisterControlLayout("Touchscreen", typeof(Touchscreen));
			this.RegisterControlLayout("Sensor", typeof(Sensor));
			this.RegisterControlLayout("Accelerometer", typeof(Accelerometer));
			this.RegisterControlLayout("Gyroscope", typeof(Gyroscope));
			this.RegisterControlLayout("GravitySensor", typeof(GravitySensor));
			this.RegisterControlLayout("AttitudeSensor", typeof(AttitudeSensor));
			this.RegisterControlLayout("LinearAccelerationSensor", typeof(LinearAccelerationSensor));
			this.RegisterControlLayout("MagneticFieldSensor", typeof(MagneticFieldSensor));
			this.RegisterControlLayout("LightSensor", typeof(LightSensor));
			this.RegisterControlLayout("PressureSensor", typeof(PressureSensor));
			this.RegisterControlLayout("HumiditySensor", typeof(HumiditySensor));
			this.RegisterControlLayout("AmbientTemperatureSensor", typeof(AmbientTemperatureSensor));
			this.RegisterControlLayout("StepCounter", typeof(StepCounter));
			this.RegisterControlLayout("TrackedDevice", typeof(TrackedDevice));
			this.RegisterPrecompiledLayout<FastKeyboard>(";AnyKey;Button;Axis;Key;DiscreteButton;Keyboard");
			this.RegisterPrecompiledLayout<FastTouchscreen>("AutoWindowSpace;Touch;Vector2;Delta;Analog;TouchPress;Button;Axis;Integer;TouchPhase;Double;Touchscreen;Pointer");
			this.RegisterPrecompiledLayout<FastMouse>("AutoWindowSpace;Vector2;Delta;Button;Axis;Digital;Integer;Mouse;Pointer");
			this.processors.AddTypeRegistration("Invert", typeof(InvertProcessor));
			this.processors.AddTypeRegistration("InvertVector2", typeof(InvertVector2Processor));
			this.processors.AddTypeRegistration("InvertVector3", typeof(InvertVector3Processor));
			this.processors.AddTypeRegistration("Clamp", typeof(ClampProcessor));
			this.processors.AddTypeRegistration("Normalize", typeof(NormalizeProcessor));
			this.processors.AddTypeRegistration("NormalizeVector2", typeof(NormalizeVector2Processor));
			this.processors.AddTypeRegistration("NormalizeVector3", typeof(NormalizeVector3Processor));
			this.processors.AddTypeRegistration("Scale", typeof(ScaleProcessor));
			this.processors.AddTypeRegistration("ScaleVector2", typeof(ScaleVector2Processor));
			this.processors.AddTypeRegistration("ScaleVector3", typeof(ScaleVector3Processor));
			this.processors.AddTypeRegistration("StickDeadzone", typeof(StickDeadzoneProcessor));
			this.processors.AddTypeRegistration("AxisDeadzone", typeof(AxisDeadzoneProcessor));
			this.processors.AddTypeRegistration("CompensateDirection", typeof(CompensateDirectionProcessor));
			this.processors.AddTypeRegistration("CompensateRotation", typeof(CompensateRotationProcessor));
			this.interactions.AddTypeRegistration("Hold", typeof(HoldInteraction));
			this.interactions.AddTypeRegistration("Tap", typeof(TapInteraction));
			this.interactions.AddTypeRegistration("SlowTap", typeof(SlowTapInteraction));
			this.interactions.AddTypeRegistration("MultiTap", typeof(MultiTapInteraction));
			this.interactions.AddTypeRegistration("Press", typeof(PressInteraction));
			this.composites.AddTypeRegistration("1DAxis", typeof(AxisComposite));
			this.composites.AddTypeRegistration("2DVector", typeof(Vector2Composite));
			this.composites.AddTypeRegistration("3DVector", typeof(Vector3Composite));
			this.composites.AddTypeRegistration("Axis", typeof(AxisComposite));
			this.composites.AddTypeRegistration("Dpad", typeof(Vector2Composite));
			this.composites.AddTypeRegistration("ButtonWithOneModifier", typeof(ButtonWithOneModifier));
			this.composites.AddTypeRegistration("ButtonWithTwoModifiers", typeof(ButtonWithTwoModifiers));
			this.composites.AddTypeRegistration("OneModifier", typeof(OneModifierComposite));
			this.composites.AddTypeRegistration("TwoModifiers", typeof(TwoModifiersComposite));
			this.RegisterCustomTypes();
		}

		private void RegisterCustomTypes(Type[] types)
		{
			foreach (Type type in types)
			{
				if (type.IsClass && !type.IsAbstract && !type.IsGenericType)
				{
					if (typeof(InputProcessor).IsAssignableFrom(type))
					{
						InputSystem.RegisterProcessor(type, null);
					}
					else if (typeof(IInputInteraction).IsAssignableFrom(type))
					{
						InputSystem.RegisterInteraction(type, null);
					}
					else if (typeof(InputBindingComposite).IsAssignableFrom(type))
					{
						InputSystem.RegisterBindingComposite(type, null);
					}
				}
			}
		}

		private void RegisterCustomTypes()
		{
			Assembly assembly = typeof(InputProcessor).Assembly;
			string name = assembly.GetName().Name;
			foreach (Assembly assembly2 in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					if (!(assembly2 == assembly))
					{
						AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
						for (int j = 0; j < referencedAssemblies.Length; j++)
						{
							if (referencedAssemblies[j].Name == name)
							{
								this.RegisterCustomTypes(assembly2.GetTypes());
								break;
							}
						}
					}
				}
				catch (ReflectionTypeLoadException)
				{
				}
			}
		}

		internal void InstallRuntime(IInputRuntime runtime)
		{
			if (this.m_Runtime != null)
			{
				this.m_Runtime.onUpdate = null;
				this.m_Runtime.onBeforeUpdate = null;
				this.m_Runtime.onDeviceDiscovered = null;
				this.m_Runtime.onPlayerFocusChanged = null;
				this.m_Runtime.onShouldRunUpdate = null;
			}
			this.m_Runtime = runtime;
			this.m_Runtime.onUpdate = new InputUpdateDelegate(this.OnUpdate);
			this.m_Runtime.onDeviceDiscovered = new Action<int, string>(this.OnNativeDeviceDiscovered);
			this.m_Runtime.onPlayerFocusChanged = new Action<bool>(this.OnFocusChanged);
			this.m_Runtime.onShouldRunUpdate = new Func<InputUpdateType, bool>(this.ShouldRunUpdate);
			this.m_Runtime.pollingFrequency = this.pollingFrequency;
			this.m_HasFocus = this.m_Runtime.isPlayerFocused;
			if (this.m_BeforeUpdateListeners.length > 0 || this.m_HaveDevicesWithStateCallbackReceivers)
			{
				this.m_Runtime.onBeforeUpdate = new Action<InputUpdateType>(this.OnBeforeUpdate);
				this.m_NativeBeforeUpdateHooked = true;
			}
		}

		internal void InstallGlobals()
		{
			InputControlLayout.s_Layouts = this.m_Layouts;
			InputProcessor.s_Processors = this.m_Processors;
			InputInteraction.s_Interactions = this.m_Interactions;
			InputBindingComposite.s_Composites = this.m_Composites;
			InputRuntime.s_Instance = this.m_Runtime;
			InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup = this.m_Runtime.currentTimeOffsetToRealtimeSinceStartup;
			InputUpdate.Restore(default(InputUpdate.SerializedState));
			InputStateBuffers.SwitchTo(this.m_StateBuffers, InputUpdateType.Dynamic);
			InputStateBuffers.s_DefaultStateBuffer = this.m_StateBuffers.defaultStateBuffer;
			InputStateBuffers.s_NoiseMaskBuffer = this.m_StateBuffers.noiseMaskBuffer;
			InputStateBuffers.s_ResetMaskBuffer = this.m_StateBuffers.resetMaskBuffer;
		}

		internal void UninstallGlobals()
		{
			if (InputControlLayout.s_Layouts.baseLayoutTable == this.m_Layouts.baseLayoutTable)
			{
				InputControlLayout.s_Layouts = default(InputControlLayout.Collection);
			}
			if (InputProcessor.s_Processors.table == this.m_Processors.table)
			{
				InputProcessor.s_Processors = default(TypeTable);
			}
			if (InputInteraction.s_Interactions.table == this.m_Interactions.table)
			{
				InputInteraction.s_Interactions = default(TypeTable);
			}
			if (InputBindingComposite.s_Composites.table == this.m_Composites.table)
			{
				InputBindingComposite.s_Composites = default(TypeTable);
			}
			InputControlLayout.s_CacheInstance = default(InputControlLayout.Cache);
			InputControlLayout.s_CacheInstanceRef = 0;
			if (this.m_Runtime != null)
			{
				this.m_Runtime.onUpdate = null;
				this.m_Runtime.onDeviceDiscovered = null;
				this.m_Runtime.onBeforeUpdate = null;
				this.m_Runtime.onPlayerFocusChanged = null;
				this.m_Runtime.onShouldRunUpdate = null;
				if (InputRuntime.s_Instance == this.m_Runtime)
				{
					InputRuntime.s_Instance = null;
				}
			}
		}

		internal bool optimizedControlsFeatureEnabled
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_OptimizedControlsFeatureEnabled;
			}
			set
			{
				this.m_OptimizedControlsFeatureEnabled = value;
			}
		}

		internal bool readValueCachingFeatureEnabled
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_ReadValueCachingFeatureEnabled;
			}
			set
			{
				this.m_ReadValueCachingFeatureEnabled = value;
			}
		}

		internal bool paranoidReadValueCachingChecksEnabled
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_ParanoidReadValueCachingChecksEnabled;
			}
			set
			{
				this.m_ParanoidReadValueCachingChecksEnabled = value;
			}
		}

		private void MakeDeviceNameUnique(InputDevice device)
		{
			if (this.m_DevicesCount == 0)
			{
				return;
			}
			string text = StringHelpers.MakeUniqueName<InputDevice>(device.name, this.m_Devices, delegate(InputDevice x)
			{
				if (x == null)
				{
					return string.Empty;
				}
				return x.name;
			});
			if (text != device.name)
			{
				InputManager.ResetControlPathsRecursive(device);
				device.m_Name = new InternedString(text);
			}
		}

		private static void ResetControlPathsRecursive(InputControl control)
		{
			control.m_Path = null;
			ReadOnlyArray<InputControl> children = control.children;
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
				InputManager.ResetControlPathsRecursive(children[i]);
			}
		}

		private void AssignUniqueDeviceId(InputDevice device)
		{
			if (device.deviceId != 0)
			{
				InputDevice inputDevice = this.TryGetDeviceById(device.deviceId);
				if (inputDevice != null)
				{
					throw new InvalidOperationException(string.Format("Duplicate device ID {0} detected for devices '{1}' and '{2}'", device.deviceId, device.name, inputDevice.name));
				}
			}
			else
			{
				device.m_DeviceId = this.m_Runtime.AllocateDeviceId();
			}
		}

		private void ReallocateStateBuffers()
		{
			InputStateBuffers stateBuffers = this.m_StateBuffers;
			InputStateBuffers inputStateBuffers = default(InputStateBuffers);
			inputStateBuffers.AllocateAll(this.m_Devices, this.m_DevicesCount);
			inputStateBuffers.MigrateAll(this.m_Devices, this.m_DevicesCount, stateBuffers);
			stateBuffers.FreeAll();
			this.m_StateBuffers = inputStateBuffers;
			InputStateBuffers.s_DefaultStateBuffer = inputStateBuffers.defaultStateBuffer;
			InputStateBuffers.s_NoiseMaskBuffer = inputStateBuffers.noiseMaskBuffer;
			InputStateBuffers.s_ResetMaskBuffer = inputStateBuffers.resetMaskBuffer;
			InputStateBuffers.SwitchTo(this.m_StateBuffers, (InputUpdate.s_LatestUpdateType != InputUpdateType.None) ? InputUpdate.s_LatestUpdateType : this.defaultUpdateType);
		}

		private unsafe void InitializeDefaultState(InputDevice device)
		{
			if (!device.hasControlsWithDefaultState)
			{
				return;
			}
			ReadOnlyArray<InputControl> allControls = device.allControls;
			int count = allControls.Count;
			void* defaultStateBuffer = this.m_StateBuffers.defaultStateBuffer;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = allControls[i];
				if (inputControl.hasDefaultState)
				{
					inputControl.m_StateBlock.Write(defaultStateBuffer, inputControl.m_DefaultState);
				}
			}
			InputStateBlock stateBlock = device.m_StateBlock;
			int deviceIndex = device.m_DeviceIndex;
			if (this.m_StateBuffers.m_PlayerStateBuffers.valid)
			{
				stateBlock.CopyToFrom(this.m_StateBuffers.m_PlayerStateBuffers.GetFrontBuffer(deviceIndex), defaultStateBuffer);
				stateBlock.CopyToFrom(this.m_StateBuffers.m_PlayerStateBuffers.GetBackBuffer(deviceIndex), defaultStateBuffer);
			}
		}

		private unsafe void InitializeDeviceState(InputDevice device)
		{
			ReadOnlyArray<InputControl> allControls = device.allControls;
			int count = allControls.Count;
			void* resetMaskBuffer = this.m_StateBuffers.resetMaskBuffer;
			bool hasControlsWithDefaultState = device.hasControlsWithDefaultState;
			void* noiseMaskBuffer = this.m_StateBuffers.noiseMaskBuffer;
			MemoryHelpers.SetBitsInBuffer(noiseMaskBuffer, (int)device.stateBlock.byteOffset, 0, (int)device.stateBlock.sizeInBits, false);
			MemoryHelpers.SetBitsInBuffer(resetMaskBuffer, (int)device.stateBlock.byteOffset, 0, (int)device.stateBlock.sizeInBits, true);
			void* defaultStateBuffer = this.m_StateBuffers.defaultStateBuffer;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = allControls[i];
				if (!inputControl.usesStateFromOtherControl)
				{
					if (!inputControl.noisy || inputControl.dontReset)
					{
						ref InputStateBlock ptr = ref inputControl.m_StateBlock;
						if (!inputControl.noisy)
						{
							MemoryHelpers.SetBitsInBuffer(noiseMaskBuffer, (int)ptr.byteOffset, (int)ptr.bitOffset, (int)ptr.sizeInBits, true);
						}
						if (inputControl.dontReset)
						{
							MemoryHelpers.SetBitsInBuffer(resetMaskBuffer, (int)ptr.byteOffset, (int)ptr.bitOffset, (int)ptr.sizeInBits, false);
						}
					}
					if (hasControlsWithDefaultState && inputControl.hasDefaultState)
					{
						inputControl.m_StateBlock.Write(defaultStateBuffer, inputControl.m_DefaultState);
					}
				}
			}
			if (hasControlsWithDefaultState)
			{
				ref InputStateBlock ptr2 = ref device.m_StateBlock;
				int deviceIndex = device.m_DeviceIndex;
				if (this.m_StateBuffers.m_PlayerStateBuffers.valid)
				{
					ptr2.CopyToFrom(this.m_StateBuffers.m_PlayerStateBuffers.GetFrontBuffer(deviceIndex), defaultStateBuffer);
					ptr2.CopyToFrom(this.m_StateBuffers.m_PlayerStateBuffers.GetBackBuffer(deviceIndex), defaultStateBuffer);
				}
			}
		}

		private void OnNativeDeviceDiscovered(int deviceId, string deviceDescriptor)
		{
			this.RestoreDevicesAfterDomainReloadIfNecessary();
			InputDevice inputDevice = this.TryMatchDisconnectedDevice(deviceDescriptor);
			InputDeviceDescription inputDeviceDescription = (inputDevice != null) ? inputDevice.description : InputDeviceDescription.FromJson(deviceDescriptor);
			bool isRemoved = false;
			try
			{
				if (this.m_Settings.supportedDevices.Count > 0)
				{
					InternedString layoutName = (inputDevice != null) ? inputDevice.m_Layout : this.TryFindMatchingControlLayout(ref inputDeviceDescription, deviceId);
					if (!this.IsDeviceLayoutMarkedAsSupportedInSettings(layoutName))
					{
						isRemoved = true;
						return;
					}
				}
				if (inputDevice != null)
				{
					inputDevice.m_DeviceId = deviceId;
					inputDevice.m_DeviceFlags |= InputDevice.DeviceFlags.Native;
					inputDevice.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledInFrontend;
					inputDevice.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledWhileInBackground;
					inputDevice.m_DeviceFlags &= ~InputDevice.DeviceFlags.DisabledStateHasBeenQueriedFromRuntime;
					this.AddDevice(inputDevice);
					DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, inputDevice, InputDeviceChange.Reconnected, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
				}
				else
				{
					this.AddDevice(inputDeviceDescription, false, null, deviceId, InputDevice.DeviceFlags.Native);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Could not create a device for '{0}' (exception: {1})", inputDeviceDescription, arg));
			}
			finally
			{
				ArrayHelpers.AppendWithCapacity<InputManager.AvailableDevice>(ref this.m_AvailableDevices, ref this.m_AvailableDeviceCount, new InputManager.AvailableDevice
				{
					description = inputDeviceDescription,
					deviceId = deviceId,
					isNative = true,
					isRemoved = isRemoved
				}, 10);
			}
		}

		private JsonParser.JsonString MakeEscapedJsonString(string theString)
		{
			if (string.IsNullOrEmpty(theString))
			{
				return new JsonParser.JsonString
				{
					text = string.Empty,
					hasEscapes = false
				};
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length = theString.Length;
			bool hasEscapes = false;
			for (int i = 0; i < length; i++)
			{
				char c = theString[i];
				if (c == '\\' || c == '"')
				{
					stringBuilder.Append('\\');
					hasEscapes = true;
				}
				stringBuilder.Append(c);
			}
			return new JsonParser.JsonString
			{
				text = stringBuilder.ToString(),
				hasEscapes = hasEscapes
			};
		}

		private InputDevice TryMatchDisconnectedDevice(string deviceDescriptor)
		{
			for (int i = 0; i < this.m_DisconnectedDevicesCount; i++)
			{
				InputDevice inputDevice = this.m_DisconnectedDevices[i];
				InputDeviceDescription description = inputDevice.description;
				if (InputDeviceDescription.ComparePropertyToDeviceDescriptor("interface", description.interfaceName, deviceDescriptor) && InputDeviceDescription.ComparePropertyToDeviceDescriptor("product", description.product, deviceDescriptor) && InputDeviceDescription.ComparePropertyToDeviceDescriptor("manufacturer", description.manufacturer, deviceDescriptor) && InputDeviceDescription.ComparePropertyToDeviceDescriptor("type", description.deviceClass, deviceDescriptor) && InputDeviceDescription.ComparePropertyToDeviceDescriptor("capabilities", this.MakeEscapedJsonString(description.capabilities), deviceDescriptor) && InputDeviceDescription.ComparePropertyToDeviceDescriptor("serial", description.serial, deviceDescriptor))
				{
					this.m_DisconnectedDevices.EraseAtWithCapacity(ref this.m_DisconnectedDevicesCount, i);
					return inputDevice;
				}
			}
			return null;
		}

		private void InstallBeforeUpdateHookIfNecessary()
		{
			if (this.m_NativeBeforeUpdateHooked || this.m_Runtime == null)
			{
				return;
			}
			this.m_Runtime.onBeforeUpdate = new Action<InputUpdateType>(this.OnBeforeUpdate);
			this.m_NativeBeforeUpdateHooked = true;
		}

		private void RestoreDevicesAfterDomainReloadIfNecessary()
		{
		}

		private void WarnAboutDevicesFailingToRecreateAfterDomainReload()
		{
		}

		private void OnBeforeUpdate(InputUpdateType updateType)
		{
			this.RestoreDevicesAfterDomainReloadIfNecessary();
			if ((updateType & this.m_UpdateMask) == InputUpdateType.None)
			{
				return;
			}
			InputStateBuffers.SwitchTo(this.m_StateBuffers, updateType);
			InputUpdate.OnBeforeUpdate(updateType);
			if (this.m_HaveDevicesWithStateCallbackReceivers && updateType != InputUpdateType.BeforeRender)
			{
				for (int i = 0; i < this.m_DevicesCount; i++)
				{
					InputDevice inputDevice = this.m_Devices[i];
					if (inputDevice.hasStateCallbacks)
					{
						((IInputStateCallbackReceiver)inputDevice).OnNextUpdate();
					}
				}
			}
			DelegateHelpers.InvokeCallbacksSafe(ref this.m_BeforeUpdateListeners, InputManager.k_InputOnBeforeUpdateMarker, "InputSystem.onBeforeUpdate", null);
		}

		internal void ApplySettings()
		{
			InputUpdateType inputUpdateType = InputUpdateType.Editor;
			if ((this.m_UpdateMask & InputUpdateType.BeforeRender) != InputUpdateType.None)
			{
				inputUpdateType |= InputUpdateType.BeforeRender;
			}
			if (this.m_Settings.updateMode == (InputSettings.UpdateMode)0)
			{
				this.m_Settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
			}
			switch (this.m_Settings.updateMode)
			{
			case InputSettings.UpdateMode.ProcessEventsInDynamicUpdate:
				inputUpdateType |= InputUpdateType.Dynamic;
				break;
			case InputSettings.UpdateMode.ProcessEventsInFixedUpdate:
				inputUpdateType |= InputUpdateType.Fixed;
				break;
			case InputSettings.UpdateMode.ProcessEventsManually:
				inputUpdateType |= InputUpdateType.Manual;
				break;
			default:
				throw new NotSupportedException("Invalid input update mode: " + this.m_Settings.updateMode.ToString());
			}
			this.updateMask = inputUpdateType;
			this.scrollDeltaBehavior = this.m_Settings.scrollDeltaBehavior;
			this.AddAvailableDevicesThatAreNowRecognized();
			if (this.settings.supportedDevices.Count > 0)
			{
				for (int i = 0; i < this.m_DevicesCount; i++)
				{
					InputDevice inputDevice = this.m_Devices[i];
					InternedString layout = inputDevice.m_Layout;
					bool flag = false;
					for (int j = 0; j < this.m_AvailableDeviceCount; j++)
					{
						if (this.m_AvailableDevices[j].deviceId == inputDevice.deviceId)
						{
							flag = true;
							break;
						}
					}
					if (flag && !this.IsDeviceLayoutMarkedAsSupportedInSettings(layout))
					{
						this.RemoveDevice(inputDevice, true);
						i--;
					}
				}
			}
			if (this.m_Settings.m_FeatureFlags != null)
			{
				this.m_ReadValueCachingFeatureEnabled = this.m_Settings.IsFeatureEnabled("USE_READ_VALUE_CACHING");
				this.m_OptimizedControlsFeatureEnabled = this.m_Settings.IsFeatureEnabled("USE_OPTIMIZED_CONTROLS");
				this.m_ParanoidReadValueCachingChecksEnabled = this.m_Settings.IsFeatureEnabled("PARANOID_READ_VALUE_CACHING_CHECKS");
			}
			Touchscreen.s_TapTime = this.settings.defaultTapTime;
			Touchscreen.s_TapDelayTime = this.settings.multiTapDelayTime;
			Touchscreen.s_TapRadiusSquared = this.settings.tapRadius * this.settings.tapRadius;
			ButtonControl.s_GlobalDefaultButtonPressPoint = Mathf.Clamp(this.settings.defaultButtonPressPoint, 0.0001f, float.MaxValue);
			ButtonControl.s_GlobalDefaultButtonReleaseThreshold = this.settings.buttonReleaseThreshold;
			foreach (InputDevice inputDevice2 in this.devices)
			{
				inputDevice2.SetOptimizedControlDataTypeRecursively();
			}
			foreach (InputDevice inputDevice3 in this.devices)
			{
				inputDevice3.MarkAsStaleRecursively();
			}
			DelegateHelpers.InvokeCallbacksSafe(ref this.m_SettingsChangedListeners, InputManager.k_InputOnSettingsChangeMarker, "InputSystem.onSettingsChange", null);
		}

		internal void ApplyActions()
		{
			DelegateHelpers.InvokeCallbacksSafe(ref this.m_ActionsChangedListeners, InputManager.k_InputOnActionsChangeMarker, "InputSystem.onActionsChange", null);
		}

		internal unsafe long ExecuteGlobalCommand<TCommand>(ref TCommand command) where TCommand : struct, IInputDeviceCommandInfo
		{
			InputDeviceCommand* commandPtr = (InputDeviceCommand*)UnsafeUtility.AddressOf<TCommand>(ref command);
			return InputRuntime.s_Instance.DeviceCommand(0, commandPtr);
		}

		internal void AddAvailableDevicesThatAreNowRecognized()
		{
			for (int i = 0; i < this.m_AvailableDeviceCount; i++)
			{
				int deviceId = this.m_AvailableDevices[i].deviceId;
				if (this.TryGetDeviceById(deviceId) == null)
				{
					InternedString internedString = this.TryFindMatchingControlLayout(ref this.m_AvailableDevices[i].description, deviceId);
					if (this.IsDeviceLayoutMarkedAsSupportedInSettings(internedString))
					{
						if (internedString.IsEmpty())
						{
							if (deviceId != 0)
							{
								DisableDeviceCommand disableDeviceCommand = DisableDeviceCommand.Create();
								this.m_Runtime.DeviceCommand(deviceId, ref disableDeviceCommand);
							}
						}
						else
						{
							try
							{
								this.AddDevice(this.m_AvailableDevices[i].description, internedString, null, deviceId, this.m_AvailableDevices[i].isNative ? InputDevice.DeviceFlags.Native : ((InputDevice.DeviceFlags)0));
							}
							catch (Exception)
							{
							}
						}
					}
				}
			}
		}

		private bool ShouldRunDeviceInBackground(InputDevice device)
		{
			return this.m_Settings.backgroundBehavior != InputSettings.BackgroundBehavior.ResetAndDisableAllDevices && device.canRunInBackground;
		}

		internal void OnFocusChanged(bool focus)
		{
			bool runInBackground = this.m_Runtime.runInBackground;
			if (this.m_Settings.backgroundBehavior == InputSettings.BackgroundBehavior.IgnoreFocus && runInBackground)
			{
				this.m_HasFocus = focus;
				return;
			}
			if (!focus)
			{
				if (runInBackground)
				{
					for (int i = 0; i < this.m_DevicesCount; i++)
					{
						InputDevice inputDevice = this.m_Devices[i];
						if (inputDevice.enabled && !this.ShouldRunDeviceInBackground(inputDevice))
						{
							this.EnableOrDisableDevice(inputDevice, false, InputManager.DeviceDisableScope.TemporaryWhilePlayerIsInBackground);
							int num = this.m_Devices.IndexOfReference(inputDevice, this.m_DevicesCount);
							if (num == -1)
							{
								i--;
							}
							else
							{
								i = num;
							}
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < this.m_DevicesCount; j++)
				{
					InputDevice inputDevice2 = this.m_Devices[j];
					if (inputDevice2.disabledWhileInBackground)
					{
						this.EnableOrDisableDevice(inputDevice2, true, InputManager.DeviceDisableScope.TemporaryWhilePlayerIsInBackground);
					}
					else if (inputDevice2.enabled && !runInBackground && !inputDevice2.RequestSync())
					{
						this.ResetDevice(inputDevice2, false, null);
					}
				}
			}
			this.m_HasFocus = focus;
		}

		internal bool ShouldRunUpdate(InputUpdateType updateType)
		{
			if (updateType == InputUpdateType.None)
			{
				return true;
			}
			InputUpdateType updateMask = this.m_UpdateMask;
			return (updateType & updateMask) > InputUpdateType.None;
		}

		private unsafe void OnUpdate(InputUpdateType updateType, ref InputEventBuffer eventBuffer)
		{
			if (this.m_InputEventStream.isOpen)
			{
				throw new InvalidOperationException("Already have an event buffer set! Was OnUpdate() called recursively?");
			}
			this.RestoreDevicesAfterDomainReloadIfNecessary();
			if ((updateType & this.m_UpdateMask) == InputUpdateType.None)
			{
				return;
			}
			this.WarnAboutDevicesFailingToRecreateAfterDomainReload();
			int num = this.m_Metrics.totalUpdateCount + 1;
			this.m_Metrics.totalUpdateCount = num;
			InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup = this.m_Runtime.currentTimeOffsetToRealtimeSinceStartup;
			InputStateBuffers.SwitchTo(this.m_StateBuffers, updateType);
			this.m_CurrentUpdate = updateType;
			InputUpdate.OnUpdate(updateType);
			bool flag = updateType.IsPlayerUpdate() && this.gameIsPlaying;
			double num2 = (updateType == InputUpdateType.Fixed) ? this.m_Runtime.currentTimeForFixedUpdate : this.m_Runtime.currentTime;
			bool flag2 = (updateType == InputUpdateType.Fixed || updateType == InputUpdateType.BeforeRender) && InputSystem.settings.updateMode == InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
			bool flag3 = !this.gameHasFocus && !this.m_Runtime.runInBackground;
			if (eventBuffer.eventCount == 0 || flag3)
			{
				if (flag)
				{
					this.ProcessStateChangeMonitorTimeouts();
				}
				this.InvokeAfterUpdateCallback(updateType);
				if (flag3)
				{
					eventBuffer.Reset();
				}
				this.m_CurrentUpdate = InputUpdateType.None;
				return;
			}
			long timestamp = Stopwatch.GetTimestamp();
			double num3 = 0.0;
			try
			{
				this.m_InputEventStream = new InputEventStream(ref eventBuffer, this.m_Settings.maxQueuedEventsPerUpdate);
				uint num4 = 0U;
				InputEvent* ptr = null;
				while (this.m_InputEventStream.remainingEventCount > 0)
				{
					InputDevice inputDevice = null;
					InputEvent* ptr2 = this.m_InputEventStream.currentEventPtr;
					if (updateType == InputUpdateType.BeforeRender)
					{
						while (this.m_InputEventStream.remainingEventCount > 0)
						{
							inputDevice = this.TryGetDeviceById(ptr2->deviceId);
							if (inputDevice != null && inputDevice.updateBeforeRender && (ptr2->type == 1398030676 || ptr2->type == 1145852993))
							{
								break;
							}
							ptr2 = this.m_InputEventStream.Advance(true);
						}
					}
					if (this.m_InputEventStream.remainingEventCount == 0)
					{
						break;
					}
					double internalTime = ptr2->internalTime;
					FourCC type = ptr2->type;
					if (flag2 && internalTime >= num2)
					{
						this.m_InputEventStream.Advance(true);
					}
					else
					{
						if (inputDevice == null)
						{
							inputDevice = this.TryGetDeviceById(ptr2->deviceId);
						}
						if (inputDevice == null)
						{
							this.m_InputEventStream.Advance(false);
						}
						else if (!inputDevice.enabled && type != 1146242381 && type != 1145259591 && (inputDevice.m_DeviceFlags & (InputDevice.DeviceFlags.DisabledInRuntime | InputDevice.DeviceFlags.DisabledWhileInBackground)) != (InputDevice.DeviceFlags)0)
						{
							this.m_InputEventStream.Advance(false);
						}
						else
						{
							if (!this.settings.disableRedundantEventsMerging && inputDevice.hasEventMerger && ptr2 != ptr)
							{
								InputEvent* ptr3 = this.m_InputEventStream.Peek();
								if (ptr3 != null && ptr2->deviceId == ptr3->deviceId && (!flag2 || ptr3->internalTime < num2))
								{
									if (((IEventMerger)inputDevice).MergeForward(ptr2, ptr3))
									{
										this.m_InputEventStream.Advance(false);
										continue;
									}
									ptr = ptr3;
								}
							}
							if (inputDevice.hasEventPreProcessor && !((IEventPreProcessor)inputDevice).PreProcessEvent(ptr2))
							{
								this.m_InputEventStream.Advance(false);
							}
							else
							{
								if (this.m_EventListeners.length > 0)
								{
									DelegateHelpers.InvokeCallbacksSafe<InputEventPtr, InputDevice>(ref this.m_EventListeners, new InputEventPtr(ptr2), inputDevice, InputManager.k_InputOnEventMarker, "InputSystem.onEvent", null);
									if (ptr2->handled)
									{
										this.m_InputEventStream.Advance(false);
										continue;
									}
								}
								if (internalTime <= num2)
								{
									num3 += num2 - internalTime;
								}
								num = this.m_Metrics.totalEventCount + 1;
								this.m_Metrics.totalEventCount = num;
								this.m_Metrics.totalEventBytes = this.m_Metrics.totalEventBytes + (int)ptr2->sizeInBytes;
								num = type;
								if (num <= 1146242381)
								{
									if (num != 1145259591)
									{
										if (num == 1145852993)
										{
											goto IL_418;
										}
										if (num == 1146242381)
										{
											this.RemoveDevice(inputDevice, false);
											if (inputDevice.native && !inputDevice.description.empty)
											{
												ArrayHelpers.AppendWithCapacity<InputDevice>(ref this.m_DisconnectedDevices, ref this.m_DisconnectedDevicesCount, inputDevice, 10);
												DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, inputDevice, InputDeviceChange.Disconnected, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
											}
										}
									}
									else
									{
										inputDevice.NotifyConfigurationChanged();
										InputActionState.OnDeviceChange(inputDevice, InputDeviceChange.ConfigurationChanged);
										DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputDeviceChange>(ref this.m_DeviceChangeListeners, inputDevice, InputDeviceChange.ConfigurationChanged, InputManager.k_InputOnDeviceChangeMarker, "InputSystem.onDeviceChange", null);
									}
								}
								else if (num <= 1229800787)
								{
									if (num != 1146245972)
									{
										if (num == 1229800787)
										{
											IMECompositionEvent* ptr4 = (IMECompositionEvent*)ptr2;
											ITextInputReceiver textInputReceiver = inputDevice as ITextInputReceiver;
											if (textInputReceiver != null)
											{
												textInputReceiver.OnIMECompositionChanged(ptr4->compositionString);
											}
										}
									}
									else
									{
										this.ResetDevice(inputDevice, ((DeviceResetEvent*)ptr2)->hardReset, null);
									}
								}
								else
								{
									if (num == 1398030676)
									{
										goto IL_418;
									}
									if (num == 1413830740)
									{
										TextEvent* ptr5 = (TextEvent*)ptr2;
										ITextInputReceiver textInputReceiver2 = inputDevice as ITextInputReceiver;
										if (textInputReceiver2 != null)
										{
											int num5 = ptr5->character;
											if (num5 >= 65536)
											{
												num5 -= 65536;
												int num6 = 55296 + (num5 >> 10 & 1023);
												int num7 = 56320 + (num5 & 1023);
												textInputReceiver2.OnTextInput((char)num6);
												textInputReceiver2.OnTextInput((char)num7);
											}
											else
											{
												textInputReceiver2.OnTextInput((char)num5);
											}
										}
									}
								}
								IL_643:
								this.m_InputEventStream.Advance(false);
								if (!this.AreMaximumEventBytesPerUpdateExceeded(num4))
								{
									continue;
								}
								break;
								IL_418:
								InputEventPtr eventPtr = new InputEventPtr(ptr2);
								bool hasStateCallbacks = inputDevice.hasStateCallbacks;
								if (internalTime < inputDevice.m_LastUpdateTimeInternal && (!hasStateCallbacks || !(inputDevice.stateBlock.format != eventPtr.stateFormat)))
								{
									goto IL_643;
								}
								bool flag4;
								if (hasStateCallbacks)
								{
									this.m_ShouldMakeCurrentlyUpdatingDeviceCurrent = true;
									((IInputStateCallbackReceiver)inputDevice).OnStateEvent(eventPtr);
									flag4 = this.m_ShouldMakeCurrentlyUpdatingDeviceCurrent;
								}
								else
								{
									if (inputDevice.stateBlock.format != eventPtr.stateFormat)
									{
										goto IL_643;
									}
									flag4 = this.UpdateState(inputDevice, eventPtr, updateType);
								}
								num4 += eventPtr.sizeInBytes;
								inputDevice.m_CurrentProcessedEventBytesOnUpdate += eventPtr.sizeInBytes;
								if (inputDevice.m_LastUpdateTimeInternal <= eventPtr.internalTime)
								{
									inputDevice.m_LastUpdateTimeInternal = eventPtr.internalTime;
								}
								if (flag4)
								{
									inputDevice.MakeCurrent();
									goto IL_643;
								}
								goto IL_643;
							}
						}
					}
				}
				this.m_Metrics.totalEventProcessingTime = this.m_Metrics.totalEventProcessingTime + (double)(Stopwatch.GetTimestamp() - timestamp) / (double)Stopwatch.Frequency;
				this.m_Metrics.totalEventLagTime = this.m_Metrics.totalEventLagTime + num3;
				this.ResetCurrentProcessedEventBytesForDevices();
				this.m_InputEventStream.Close(ref eventBuffer);
			}
			catch (Exception)
			{
				this.m_InputEventStream.CleanUpAfterException();
				throw;
			}
			if (flag)
			{
				this.ProcessStateChangeMonitorTimeouts();
			}
			this.InvokeAfterUpdateCallback(updateType);
			this.m_CurrentUpdate = InputUpdateType.None;
		}

		private bool AreMaximumEventBytesPerUpdateExceeded(uint totalEventBytesProcessed)
		{
			if (this.m_Settings.maxEventBytesPerUpdate > 0 && (ulong)totalEventBytesProcessed >= (ulong)((long)this.m_Settings.maxEventBytesPerUpdate))
			{
				string str = string.Empty;
				if (Debug.isDebugBuild)
				{
					str = "Total events processed by devices in last update call:\n" + this.MakeStringWithEventsProcessedByDevice();
				}
				Debug.LogError("Exceeded budget for maximum input event throughput per InputSystem.Update(). Discarding remaining events. Increase InputSystem.settings.maxEventBytesPerUpdate or set it to 0 to remove the limit.\n" + str);
				return true;
			}
			return false;
		}

		private string MakeStringWithEventsProcessedByDevice()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.m_DevicesCount; i++)
			{
				InputDevice inputDevice = this.devices[i];
				if (inputDevice != null && inputDevice.m_CurrentProcessedEventBytesOnUpdate > 0U)
				{
					stringBuilder.Append(string.Format(" - {0} bytes processed by {1}\n", inputDevice.m_CurrentProcessedEventBytesOnUpdate, inputDevice));
				}
			}
			return stringBuilder.ToString();
		}

		private void ResetCurrentProcessedEventBytesForDevices()
		{
			if (Debug.isDebugBuild)
			{
				for (int i = 0; i < this.m_DevicesCount; i++)
				{
					InputDevice inputDevice = this.m_Devices[i];
					if (inputDevice != null && inputDevice.m_CurrentProcessedEventBytesOnUpdate > 0U)
					{
						inputDevice.m_CurrentProcessedEventBytesOnUpdate = 0U;
					}
				}
			}
		}

		[Conditional("UNITY_EDITOR")]
		private void CheckAllDevicesOptimizedControlsHaveValidState()
		{
			if (!InputSystem.s_Manager.m_OptimizedControlsFeatureEnabled)
			{
				return;
			}
			foreach (InputDevice inputDevice in this.devices)
			{
			}
		}

		private void InvokeAfterUpdateCallback(InputUpdateType updateType)
		{
			if (updateType == InputUpdateType.Editor && this.gameIsPlaying)
			{
				return;
			}
			DelegateHelpers.InvokeCallbacksSafe(ref this.m_AfterUpdateListeners, InputManager.k_InputOnAfterUpdateMarker, "InputSystem.onAfterUpdate", null);
		}

		internal void DontMakeCurrentlyUpdatingDeviceCurrent()
		{
			this.m_ShouldMakeCurrentlyUpdatingDeviceCurrent = false;
		}

		internal unsafe bool UpdateState(InputDevice device, InputEvent* eventPtr, InputUpdateType updateType)
		{
			InputStateBlock stateBlock = device.m_StateBlock;
			uint num = stateBlock.sizeInBits / 8U;
			uint num2 = 0U;
			byte* statePtr;
			uint num3;
			if (eventPtr->type == 1398030676)
			{
				StateEvent stateEvent = *(StateEvent*)eventPtr;
				uint stateSizeInBytes = ((StateEvent*)eventPtr)->stateSizeInBytes;
				statePtr = (byte*)((StateEvent*)eventPtr)->state;
				num3 = stateSizeInBytes;
				if (num3 > num)
				{
					num3 = num;
				}
			}
			else
			{
				DeltaStateEvent deltaStateEvent = *(DeltaStateEvent*)eventPtr;
				uint deltaStateSizeInBytes = ((DeltaStateEvent*)eventPtr)->deltaStateSizeInBytes;
				statePtr = (byte*)((DeltaStateEvent*)eventPtr)->deltaState;
				num2 = ((DeltaStateEvent*)eventPtr)->stateOffset;
				num3 = deltaStateSizeInBytes;
				if (num2 + num3 > num)
				{
					if (num2 >= num)
					{
						return false;
					}
					num3 = num - num2;
				}
			}
			return this.UpdateState(device, updateType, (void*)statePtr, num2, num3, eventPtr->internalTime, eventPtr);
		}

		internal unsafe bool UpdateState(InputDevice device, InputUpdateType updateType, void* statePtr, uint stateOffsetInDevice, uint stateSize, double internalTime, InputEventPtr eventPtr = default(InputEventPtr))
		{
			int deviceIndex = device.m_DeviceIndex;
			ref InputStateBlock ptr = ref device.m_StateBlock;
			byte* frontBufferForDevice = (byte*)InputStateBuffers.GetFrontBufferForDevice(deviceIndex);
			this.SortStateChangeMonitorsIfNecessary(deviceIndex);
			bool flag = this.ProcessStateChangeMonitors(deviceIndex, statePtr, (void*)(frontBufferForDevice + ptr.byteOffset), stateSize, stateOffsetInDevice);
			uint num = device.m_StateBlock.byteOffset + stateOffsetInDevice;
			void* ptr2 = (void*)(frontBufferForDevice + num);
			byte* mask = device.noisy ? ((byte*)InputStateBuffers.s_NoiseMaskBuffer + num) : null;
			bool flag2 = !MemoryHelpers.MemCmpBitRegion(ptr2, statePtr, 0U, stateSize * 8U, (void*)mask);
			bool flippedBuffers = this.FlipBuffersForDeviceIfNecessary(device, updateType);
			this.WriteStateChange(this.m_StateBuffers.m_PlayerStateBuffers, deviceIndex, ref ptr, stateOffsetInDevice, statePtr, stateSize, flippedBuffers);
			if (flag2)
			{
				if (InputSystem.s_Manager.m_ReadValueCachingFeatureEnabled || device.m_UseCachePathForButtonPresses)
				{
					using (HashSet<int>.Enumerator enumerator = device.m_UpdatedButtons.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int index = enumerator.Current;
							((ButtonControl)device.allControls[index]).UpdateWasPressed();
						}
						goto IL_145;
					}
				}
				int num2 = 0;
				foreach (ButtonControl buttonControl in device.m_ButtonControlsCheckingPressState)
				{
					buttonControl.UpdateWasPressed();
					num2++;
				}
				if (num2 > 45)
				{
					device.m_UseCachePathForButtonPresses = true;
				}
			}
			IL_145:
			DelegateHelpers.InvokeCallbacksSafe<InputDevice, InputEventPtr>(ref this.m_DeviceStateChangeListeners, device, eventPtr, InputManager.k_InputOnDeviceSettingsChangeMarker, "InputSystem.onDeviceStateChange", null);
			if (flag)
			{
				this.FireStateChangeNotifications(deviceIndex, internalTime, eventPtr);
			}
			return flag2;
		}

		private unsafe void WriteStateChange(InputStateBuffers.DoubleBuffers buffers, int deviceIndex, ref InputStateBlock deviceStateBlock, uint stateOffsetInDevice, void* statePtr, uint stateSizeInBytes, bool flippedBuffers)
		{
			void* frontBuffer = buffers.GetFrontBuffer(deviceIndex);
			uint num = deviceStateBlock.sizeInBits / 8U;
			if (flippedBuffers && num != stateSizeInBytes)
			{
				void* backBuffer = buffers.GetBackBuffer(deviceIndex);
				UnsafeUtility.MemCpy((void*)((byte*)frontBuffer + deviceStateBlock.byteOffset), (void*)((byte*)backBuffer + deviceStateBlock.byteOffset), (long)((ulong)num));
			}
			if (InputSystem.s_Manager.m_ReadValueCachingFeatureEnabled || this.m_Devices[deviceIndex].m_UseCachePathForButtonPresses)
			{
				byte* ptr = (byte*)frontBuffer;
				if (flippedBuffers && num == stateSizeInBytes)
				{
					ptr = (byte*)buffers.GetBackBuffer(deviceIndex);
				}
				this.m_Devices[deviceIndex].WriteChangedControlStates(ptr + deviceStateBlock.byteOffset, statePtr, stateSizeInBytes, stateOffsetInDevice);
			}
			UnsafeUtility.MemCpy((void*)((byte*)((byte*)frontBuffer + deviceStateBlock.byteOffset) + stateOffsetInDevice), statePtr, (long)((ulong)stateSizeInBytes));
		}

		private bool FlipBuffersForDeviceIfNecessary(InputDevice device, InputUpdateType updateType)
		{
			if (updateType == InputUpdateType.BeforeRender)
			{
				return false;
			}
			if (device.m_CurrentUpdateStepCount != InputUpdate.s_UpdateStepCount)
			{
				this.m_StateBuffers.m_PlayerStateBuffers.SwapBuffers(device.m_DeviceIndex);
				device.m_CurrentUpdateStepCount = InputUpdate.s_UpdateStepCount;
				return true;
			}
			return false;
		}

		public void AddStateChangeMonitor(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex, uint groupIndex)
		{
			if (this.m_DevicesCount <= 0)
			{
				return;
			}
			int deviceIndex = control.device.m_DeviceIndex;
			if (this.m_StateChangeMonitors == null)
			{
				this.m_StateChangeMonitors = new InputManager.StateChangeMonitorsForDevice[this.m_DevicesCount];
			}
			else if (this.m_StateChangeMonitors.Length <= deviceIndex)
			{
				Array.Resize<InputManager.StateChangeMonitorsForDevice>(ref this.m_StateChangeMonitors, this.m_DevicesCount);
			}
			if (!this.isProcessingEvents && this.m_StateChangeMonitors[deviceIndex].needToCompactArrays)
			{
				this.m_StateChangeMonitors[deviceIndex].CompactArrays();
			}
			this.m_StateChangeMonitors[deviceIndex].Add(control, monitor, monitorIndex, groupIndex);
		}

		private void RemoveStateChangeMonitors(InputDevice device)
		{
			if (this.m_StateChangeMonitors == null)
			{
				return;
			}
			int deviceIndex = device.m_DeviceIndex;
			if (deviceIndex >= this.m_StateChangeMonitors.Length)
			{
				return;
			}
			this.m_StateChangeMonitors[deviceIndex].Clear();
			for (int i = 0; i < this.m_StateChangeMonitorTimeouts.length; i++)
			{
				InputControl control = this.m_StateChangeMonitorTimeouts[i].control;
				if (((control != null) ? control.device : null) == device)
				{
					this.m_StateChangeMonitorTimeouts[i] = default(InputManager.StateChangeMonitorTimeout);
				}
			}
		}

		public void RemoveStateChangeMonitor(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex)
		{
			if (this.m_StateChangeMonitors == null)
			{
				return;
			}
			int deviceIndex = control.device.m_DeviceIndex;
			if (deviceIndex == -1)
			{
				return;
			}
			if (deviceIndex >= this.m_StateChangeMonitors.Length)
			{
				return;
			}
			this.m_StateChangeMonitors[deviceIndex].Remove(monitor, monitorIndex, this.isProcessingEvents);
			for (int i = 0; i < this.m_StateChangeMonitorTimeouts.length; i++)
			{
				if (this.m_StateChangeMonitorTimeouts[i].monitor == monitor && this.m_StateChangeMonitorTimeouts[i].monitorIndex == monitorIndex)
				{
					this.m_StateChangeMonitorTimeouts[i] = default(InputManager.StateChangeMonitorTimeout);
				}
			}
		}

		public void AddStateChangeMonitorTimeout(InputControl control, IInputStateChangeMonitor monitor, double time, long monitorIndex, int timerIndex)
		{
			this.m_StateChangeMonitorTimeouts.Append(new InputManager.StateChangeMonitorTimeout
			{
				control = control,
				time = time,
				monitor = monitor,
				monitorIndex = monitorIndex,
				timerIndex = timerIndex
			});
		}

		public void RemoveStateChangeMonitorTimeout(IInputStateChangeMonitor monitor, long monitorIndex, int timerIndex)
		{
			int length = this.m_StateChangeMonitorTimeouts.length;
			for (int i = 0; i < length; i++)
			{
				if (this.m_StateChangeMonitorTimeouts[i].monitor == monitor && this.m_StateChangeMonitorTimeouts[i].monitorIndex == monitorIndex && this.m_StateChangeMonitorTimeouts[i].timerIndex == timerIndex)
				{
					this.m_StateChangeMonitorTimeouts[i] = default(InputManager.StateChangeMonitorTimeout);
					return;
				}
			}
		}

		private void SortStateChangeMonitorsIfNecessary(int deviceIndex)
		{
			if (this.m_StateChangeMonitors != null && deviceIndex < this.m_StateChangeMonitors.Length && this.m_StateChangeMonitors[deviceIndex].needToUpdateOrderingOfMonitors)
			{
				this.m_StateChangeMonitors[deviceIndex].SortMonitorsByIndex();
			}
		}

		public void SignalStateChangeMonitor(InputControl control, IInputStateChangeMonitor monitor)
		{
			int deviceIndex = control.device.m_DeviceIndex;
			ref InputManager.StateChangeMonitorsForDevice ptr = ref this.m_StateChangeMonitors[deviceIndex];
			for (int i = 0; i < ptr.signalled.length; i++)
			{
				this.SortStateChangeMonitorsIfNecessary(i);
				ref InputManager.StateChangeMonitorListener ptr2 = ref ptr.listeners[i];
				if (ptr2.control == control && ptr2.monitor == monitor)
				{
					ptr.signalled.SetBit(i);
				}
			}
		}

		public void FireStateChangeNotifications()
		{
			double currentTime = this.m_Runtime.currentTime;
			int num = Math.Min(this.m_StateChangeMonitors.LengthSafe<InputManager.StateChangeMonitorsForDevice>(), this.m_DevicesCount);
			for (int i = 0; i < num; i++)
			{
				this.FireStateChangeNotifications(i, currentTime, null);
			}
		}

		private unsafe bool ProcessStateChangeMonitors(int deviceIndex, void* newStateFromEvent, void* oldStateOfDevice, uint newStateSizeInBytes, uint newStateOffsetInBytes)
		{
			if (this.m_StateChangeMonitors == null)
			{
				return false;
			}
			if (deviceIndex >= this.m_StateChangeMonitors.Length)
			{
				return false;
			}
			MemoryHelpers.BitRegion[] memoryRegions = this.m_StateChangeMonitors[deviceIndex].memoryRegions;
			if (memoryRegions == null)
			{
				return false;
			}
			int num = this.m_StateChangeMonitors[deviceIndex].count;
			bool result = false;
			DynamicBitfield signalled = this.m_StateChangeMonitors[deviceIndex].signalled;
			bool flag = false;
			MemoryHelpers.BitRegion bitRegion = new MemoryHelpers.BitRegion(newStateOffsetInBytes, 0U, newStateSizeInBytes * 8U);
			for (int i = 0; i < num; i++)
			{
				MemoryHelpers.BitRegion bitRegion2 = memoryRegions[i];
				if (bitRegion2.sizeInBits == 0U)
				{
					int num2 = num;
					int num3 = num;
					this.m_StateChangeMonitors[deviceIndex].listeners.EraseAtWithCapacity(ref num2, i);
					memoryRegions.EraseAtWithCapacity(ref num3, i);
					signalled.SetLength(num - 1);
					flag = true;
					num--;
					i--;
				}
				else
				{
					MemoryHelpers.BitRegion region = bitRegion.Overlap(bitRegion2);
					if (!region.isEmpty && !MemoryHelpers.Compare(oldStateOfDevice, (void*)((byte*)newStateFromEvent - newStateOffsetInBytes), region))
					{
						signalled.SetBit(i);
						flag = true;
						result = true;
					}
				}
			}
			if (flag)
			{
				this.m_StateChangeMonitors[deviceIndex].signalled = signalled;
			}
			this.m_StateChangeMonitors[deviceIndex].needToCompactArrays = false;
			return result;
		}

		internal unsafe void FireStateChangeNotifications(int deviceIndex, double internalTime, InputEvent* eventPtr)
		{
			if (this.m_StateChangeMonitors == null)
			{
				return;
			}
			if (this.m_StateChangeMonitors.Length <= deviceIndex)
			{
				return;
			}
			ref DynamicBitfield ptr = ref this.m_StateChangeMonitors[deviceIndex].signalled;
			if (ptr.AnyBitIsSet() && this.m_StateChangeMonitors[deviceIndex].listeners == null)
			{
				return;
			}
			ref InputManager.StateChangeMonitorListener[] ptr2 = ref this.m_StateChangeMonitors[deviceIndex].listeners;
			double time = internalTime - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			InputEvent inputEvent = new InputEvent(new FourCC('F', 'A', 'K', 'E'), 20, -1, internalTime);
			if (eventPtr == null)
			{
				eventPtr = (InputEvent*)UnsafeUtility.AddressOf<InputEvent>(ref inputEvent);
			}
			eventPtr->handled = false;
			for (int i = 0; i < ptr.length; i++)
			{
				if (ptr.TestBit(i))
				{
					InputManager.StateChangeMonitorListener stateChangeMonitorListener = ptr2[i];
					try
					{
						stateChangeMonitorListener.monitor.NotifyControlStateChanged(stateChangeMonitorListener.control, time, eventPtr, stateChangeMonitorListener.monitorIndex);
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("Exception '{0}' thrown from state change monitor '{1}' on '{2}'", ex.GetType().Name, stateChangeMonitorListener.monitor.GetType().Name, stateChangeMonitorListener.control));
						Debug.LogException(ex);
					}
					if (eventPtr->handled)
					{
						uint groupIndex = ptr2[i].groupIndex;
						for (int j = i + 1; j < ptr.length; j++)
						{
							if (ptr2[j].groupIndex == groupIndex && ptr2[j].monitor == stateChangeMonitorListener.monitor)
							{
								ptr.ClearBit(j);
							}
						}
						eventPtr->handled = false;
					}
					ptr.ClearBit(i);
				}
			}
		}

		private void ProcessStateChangeMonitorTimeouts()
		{
			if (this.m_StateChangeMonitorTimeouts.length == 0)
			{
				return;
			}
			double num = this.m_Runtime.currentTime - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
			int num2 = 0;
			for (int i = 0; i < this.m_StateChangeMonitorTimeouts.length; i++)
			{
				if (this.m_StateChangeMonitorTimeouts[i].control != null)
				{
					if (this.m_StateChangeMonitorTimeouts[i].time <= num)
					{
						InputManager.StateChangeMonitorTimeout stateChangeMonitorTimeout = this.m_StateChangeMonitorTimeouts[i];
						stateChangeMonitorTimeout.monitor.NotifyTimerExpired(stateChangeMonitorTimeout.control, num, stateChangeMonitorTimeout.monitorIndex, stateChangeMonitorTimeout.timerIndex);
					}
					else
					{
						if (i != num2)
						{
							this.m_StateChangeMonitorTimeouts[num2] = this.m_StateChangeMonitorTimeouts[i];
						}
						num2++;
					}
				}
			}
			this.m_StateChangeMonitorTimeouts.SetLength(num2);
		}

		private static readonly ProfilerMarker k_InputUpdateProfilerMarker = new ProfilerMarker("InputUpdate");

		private static readonly ProfilerMarker k_InputTryFindMatchingControllerMarker = new ProfilerMarker("InputSystem.TryFindMatchingControlLayout");

		private static readonly ProfilerMarker k_InputAddDeviceMarker = new ProfilerMarker("InputSystem.AddDevice");

		private static readonly ProfilerMarker k_InputRestoreDevicesAfterReloadMarker = new ProfilerMarker("InputManager.RestoreDevicesAfterDomainReload");

		private static readonly ProfilerMarker k_InputRegisterCustomTypesMarker = new ProfilerMarker("InputManager.RegisterCustomTypes");

		private static readonly ProfilerMarker k_InputOnBeforeUpdateMarker = new ProfilerMarker("InputSystem.onBeforeUpdate");

		private static readonly ProfilerMarker k_InputOnAfterUpdateMarker = new ProfilerMarker("InputSystem.onAfterUpdate");

		private static readonly ProfilerMarker k_InputOnSettingsChangeMarker = new ProfilerMarker("InputSystem.onSettingsChange");

		private static readonly ProfilerMarker k_InputOnDeviceSettingsChangeMarker = new ProfilerMarker("InputSystem.onDeviceSettingsChange");

		private static readonly ProfilerMarker k_InputOnEventMarker = new ProfilerMarker("InputSystem.onEvent");

		private static readonly ProfilerMarker k_InputOnLayoutChangeMarker = new ProfilerMarker("InputSystem.onLayoutChange");

		private static readonly ProfilerMarker k_InputOnDeviceChangeMarker = new ProfilerMarker("InpustSystem.onDeviceChange");

		private static readonly ProfilerMarker k_InputOnActionsChangeMarker = new ProfilerMarker("InpustSystem.onActionsChange");

		internal int m_LayoutRegistrationVersion;

		private float m_PollingFrequency;

		internal InputControlLayout.Collection m_Layouts;

		private TypeTable m_Processors;

		private TypeTable m_Interactions;

		private TypeTable m_Composites;

		private int m_DevicesCount;

		private InputDevice[] m_Devices;

		private Dictionary<int, InputDevice> m_DevicesById;

		internal int m_AvailableDeviceCount;

		internal InputManager.AvailableDevice[] m_AvailableDevices;

		internal int m_DisconnectedDevicesCount;

		internal InputDevice[] m_DisconnectedDevices;

		internal InputUpdateType m_UpdateMask;

		private InputUpdateType m_CurrentUpdate;

		internal InputStateBuffers m_StateBuffers;

		private InputSettings.ScrollDeltaBehavior m_ScrollDeltaBehavior;

		private CallbackArray<Action<InputDevice, InputDeviceChange>> m_DeviceChangeListeners;

		private CallbackArray<Action<InputDevice, InputEventPtr>> m_DeviceStateChangeListeners;

		private CallbackArray<InputDeviceFindControlLayoutDelegate> m_DeviceFindLayoutCallbacks;

		internal CallbackArray<InputDeviceCommandDelegate> m_DeviceCommandCallbacks;

		private CallbackArray<Action<string, InputControlLayoutChange>> m_LayoutChangeListeners;

		private CallbackArray<Action<InputEventPtr, InputDevice>> m_EventListeners;

		private CallbackArray<Action> m_BeforeUpdateListeners;

		private CallbackArray<Action> m_AfterUpdateListeners;

		private CallbackArray<Action> m_SettingsChangedListeners;

		private CallbackArray<Action> m_ActionsChangedListeners;

		private bool m_NativeBeforeUpdateHooked;

		private bool m_HaveDevicesWithStateCallbackReceivers;

		private bool m_HasFocus;

		private InputEventStream m_InputEventStream;

		private InputDeviceExecuteCommandDelegate m_DeviceFindExecuteCommandDelegate;

		private int m_DeviceFindExecuteCommandDeviceId;

		internal IInputRuntime m_Runtime;

		internal InputMetrics m_Metrics;

		internal InputSettings m_Settings;

		private bool m_OptimizedControlsFeatureEnabled;

		private bool m_ReadValueCachingFeatureEnabled;

		private bool m_ParanoidReadValueCachingChecksEnabled;

		private InputActionAsset m_Actions;

		private bool m_ShouldMakeCurrentlyUpdatingDeviceCurrent;

		internal InputManager.StateChangeMonitorsForDevice[] m_StateChangeMonitors;

		private InlinedArray<InputManager.StateChangeMonitorTimeout> m_StateChangeMonitorTimeouts;

		internal enum DeviceDisableScope
		{
			Everywhere,
			InFrontendOnly,
			TemporaryWhilePlayerIsInBackground
		}

		[Serializable]
		internal struct AvailableDevice
		{
			public InputDeviceDescription description;

			public int deviceId;

			public bool isNative;

			public bool isRemoved;
		}

		private struct StateChangeMonitorTimeout
		{
			public InputControl control;

			public double time;

			public IInputStateChangeMonitor monitor;

			public long monitorIndex;

			public int timerIndex;
		}

		internal struct StateChangeMonitorListener
		{
			public InputControl control;

			public IInputStateChangeMonitor monitor;

			public long monitorIndex;

			public uint groupIndex;
		}

		internal struct StateChangeMonitorsForDevice
		{
			public int count
			{
				get
				{
					return this.signalled.length;
				}
			}

			public void Add(InputControl control, IInputStateChangeMonitor monitor, long monitorIndex, uint groupIndex)
			{
				int length = this.signalled.length;
				ArrayHelpers.AppendWithCapacity<InputManager.StateChangeMonitorListener>(ref this.listeners, ref length, new InputManager.StateChangeMonitorListener
				{
					monitor = monitor,
					monitorIndex = monitorIndex,
					groupIndex = groupIndex,
					control = control
				}, 10);
				ref InputStateBlock ptr = ref control.m_StateBlock;
				int length2 = this.signalled.length;
				ArrayHelpers.AppendWithCapacity<MemoryHelpers.BitRegion>(ref this.memoryRegions, ref length2, new MemoryHelpers.BitRegion(ptr.byteOffset - control.device.stateBlock.byteOffset, ptr.bitOffset, ptr.sizeInBits), 10);
				this.signalled.SetLength(this.signalled.length + 1);
				this.needToUpdateOrderingOfMonitors = true;
			}

			public void Remove(IInputStateChangeMonitor monitor, long monitorIndex, bool deferRemoval)
			{
				if (this.listeners == null)
				{
					return;
				}
				int i = 0;
				while (i < this.signalled.length)
				{
					if (this.listeners[i].monitor == monitor && this.listeners[i].monitorIndex == monitorIndex)
					{
						if (deferRemoval)
						{
							this.listeners[i] = default(InputManager.StateChangeMonitorListener);
							this.memoryRegions[i] = default(MemoryHelpers.BitRegion);
							this.signalled.ClearBit(i);
							this.needToCompactArrays = true;
							return;
						}
						this.RemoveAt(i);
						return;
					}
					else
					{
						i++;
					}
				}
			}

			public void Clear()
			{
				this.listeners.Clear(this.count);
				this.signalled.SetLength(0);
				this.needToCompactArrays = false;
			}

			public void CompactArrays()
			{
				for (int i = this.count - 1; i >= 0; i--)
				{
					if (this.memoryRegions[i].sizeInBits == 0U)
					{
						this.RemoveAt(i);
					}
				}
				this.needToCompactArrays = false;
			}

			private void RemoveAt(int i)
			{
				int count = this.count;
				int count2 = this.count;
				this.listeners.EraseAtWithCapacity(ref count, i);
				this.memoryRegions.EraseAtWithCapacity(ref count2, i);
				this.signalled.SetLength(this.count - 1);
			}

			public void SortMonitorsByIndex()
			{
				for (int i = 1; i < this.signalled.length; i++)
				{
					for (int j = i; j > 0; j--)
					{
						int complexityFromMonitorIndex = InputActionState.GetComplexityFromMonitorIndex(this.listeners[j - 1].monitorIndex);
						int complexityFromMonitorIndex2 = InputActionState.GetComplexityFromMonitorIndex(this.listeners[j].monitorIndex);
						if (complexityFromMonitorIndex >= complexityFromMonitorIndex2)
						{
							break;
						}
						this.listeners.SwapElements(j, j - 1);
						this.memoryRegions.SwapElements(j, j - 1);
					}
				}
				this.needToUpdateOrderingOfMonitors = false;
			}

			public MemoryHelpers.BitRegion[] memoryRegions;

			public InputManager.StateChangeMonitorListener[] listeners;

			public DynamicBitfield signalled;

			public bool needToUpdateOrderingOfMonitors;

			public bool needToCompactArrays;
		}
	}
}
