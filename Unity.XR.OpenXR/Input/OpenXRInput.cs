using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace UnityEngine.XR.OpenXR.Input
{
	public static class OpenXRInput
	{
		internal static void RegisterLayouts()
		{
			InputSystem.RegisterLayout<HapticControl>("Haptic", null);
			InputSystem.RegisterLayout<OpenXRDevice>(null, null);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			inputDeviceMatcher = inputDeviceMatcher.WithProduct("Head Tracking - OpenXR", true);
			InputSystem.RegisterLayout<OpenXRHmd>(name, new InputDeviceMatcher?(inputDeviceMatcher.WithManufacturer("OpenXR", true)));
			OpenXRInteractionFeature.RegisterLayouts();
		}

		private static bool ValidateActionMapConfig(OpenXRInteractionFeature interactionFeature, OpenXRInteractionFeature.ActionMapConfig actionMapConfig)
		{
			bool result = true;
			if (actionMapConfig.deviceInfos == null || actionMapConfig.deviceInfos.Count == 0)
			{
				Debug.LogError(string.Format("ActionMapConfig contains no `deviceInfos` in InteractionFeature '{0}'", interactionFeature.GetType()));
				result = false;
			}
			if (actionMapConfig.actions == null || actionMapConfig.actions.Count == 0)
			{
				Debug.LogError(string.Format("ActionMapConfig contains no `actions` in InteractionFeature '{0}'", interactionFeature.GetType()));
				result = false;
			}
			return result;
		}

		internal static void AttachActionSets()
		{
			List<OpenXRInteractionFeature.ActionMapConfig> list = new List<OpenXRInteractionFeature.ActionMapConfig>();
			List<OpenXRInteractionFeature.ActionMapConfig> list2 = new List<OpenXRInteractionFeature.ActionMapConfig>();
			foreach (OpenXRInteractionFeature openXRInteractionFeature in from f in OpenXRSettings.Instance.features.OfType<OpenXRInteractionFeature>()
			where f.enabled && !f.IsAdditive
			select f)
			{
				int count = list.Count;
				openXRInteractionFeature.CreateActionMaps(list);
				for (int i = list.Count - 1; i >= count; i--)
				{
					if (!OpenXRInput.ValidateActionMapConfig(openXRInteractionFeature, list[i]))
					{
						list.RemoveAt(i);
					}
				}
			}
			if (!OpenXRInput.RegisterDevices(list, false))
			{
				return;
			}
			foreach (OpenXRInteractionFeature openXRInteractionFeature2 in from f in OpenXRSettings.Instance.features.OfType<OpenXRInteractionFeature>()
			where f.enabled && f.IsAdditive
			select f)
			{
				openXRInteractionFeature2.CreateActionMaps(list2);
				openXRInteractionFeature2.AddAdditiveActions(list, list2[list2.Count - 1]);
			}
			Dictionary<string, List<OpenXRInput.SerializedBinding>> dictionary = new Dictionary<string, List<OpenXRInput.SerializedBinding>>();
			if (!OpenXRInput.CreateActions(list, dictionary))
			{
				return;
			}
			if (list2.Count > 0)
			{
				OpenXRInput.RegisterDevices(list2, true);
				OpenXRInput.CreateActions(list2, dictionary);
			}
			OpenXRInput.SetDpadBindingCustomValues();
			foreach (KeyValuePair<string, List<OpenXRInput.SerializedBinding>> keyValuePair in dictionary)
			{
				if (!OpenXRInput.Internal_SuggestBindings(keyValuePair.Key, keyValuePair.Value.ToArray(), (uint)keyValuePair.Value.Count))
				{
					OpenXRRuntime.LogLastError();
				}
			}
			if (!OpenXRInput.Internal_AttachActionSets())
			{
				OpenXRRuntime.LogLastError();
			}
		}

		private static bool RegisterDevices(List<OpenXRInteractionFeature.ActionMapConfig> actionMaps, bool isAdditive)
		{
			foreach (OpenXRInteractionFeature.ActionMapConfig actionMapConfig in actionMaps)
			{
				foreach (OpenXRInteractionFeature.DeviceConfig deviceConfig in actionMapConfig.deviceInfos)
				{
					string name = (actionMapConfig.desiredInteractionProfile == null) ? OpenXRInput.UserPathToDeviceName(deviceConfig.userPath) : actionMapConfig.localizedName;
					if (OpenXRInput.Internal_RegisterDeviceDefinition(deviceConfig.userPath, actionMapConfig.desiredInteractionProfile, isAdditive, (uint)deviceConfig.characteristics, name, actionMapConfig.manufacturer, actionMapConfig.serialNumber) == 0UL)
					{
						OpenXRRuntime.LogLastError();
						return false;
					}
				}
			}
			return true;
		}

		private static bool CreateActions(List<OpenXRInteractionFeature.ActionMapConfig> actionMaps, Dictionary<string, List<OpenXRInput.SerializedBinding>> interactionProfiles)
		{
			foreach (OpenXRInteractionFeature.ActionMapConfig actionMapConfig in actionMaps)
			{
				string localizedName = OpenXRInput.SanitizeStringForOpenXRPath(actionMapConfig.localizedName);
				ulong num = OpenXRInput.Internal_CreateActionSet(OpenXRInput.SanitizeStringForOpenXRPath(actionMapConfig.name), localizedName, default(OpenXRInput.SerializedGuid));
				if (num == 0UL)
				{
					OpenXRRuntime.LogLastError();
					return false;
				}
				List<string> list = (from d in actionMapConfig.deviceInfos
				select d.userPath).ToList<string>();
				foreach (OpenXRInteractionFeature.ActionConfig actionConfig in actionMapConfig.actions)
				{
					string[] array = (from b in actionConfig.bindings
					where b.userPaths != null
					select b).SelectMany((OpenXRInteractionFeature.ActionBinding b) => b.userPaths).Distinct<string>().ToList<string>().Union(list).ToArray<string>();
					ulong actionSetId = num;
					string name = OpenXRInput.SanitizeStringForOpenXRPath(actionConfig.name);
					string localizedName2 = actionConfig.localizedName;
					uint type = (uint)actionConfig.type;
					OpenXRInput.SerializedGuid guid = default(OpenXRInput.SerializedGuid);
					string[] userPaths = array;
					uint userPathCount = (uint)array.Length;
					bool isAdditive = actionConfig.isAdditive;
					List<string> usages = actionConfig.usages;
					string[] usages2 = (usages != null) ? usages.ToArray() : null;
					List<string> usages3 = actionConfig.usages;
					ulong num2 = OpenXRInput.Internal_CreateAction(actionSetId, name, localizedName2, type, guid, userPaths, userPathCount, isAdditive, usages2, (uint)((usages3 != null) ? usages3.Count : 0));
					if (num2 == 0UL)
					{
						OpenXRRuntime.LogLastError();
						return false;
					}
					foreach (OpenXRInteractionFeature.ActionBinding actionBinding in actionConfig.bindings)
					{
						foreach (string str in (actionBinding.userPaths ?? list))
						{
							string key = actionConfig.isAdditive ? actionMapConfig.desiredInteractionProfile : (actionBinding.interactionProfileName ?? actionMapConfig.desiredInteractionProfile);
							List<OpenXRInput.SerializedBinding> list2;
							if (!interactionProfiles.TryGetValue(key, out list2))
							{
								list2 = new List<OpenXRInput.SerializedBinding>();
								interactionProfiles[key] = list2;
							}
							list2.Add(new OpenXRInput.SerializedBinding
							{
								actionId = num2,
								path = str + actionBinding.interactionPath
							});
						}
					}
				}
			}
			return true;
		}

		private static void SetDpadBindingCustomValues()
		{
			DPadInteraction feature = OpenXRSettings.Instance.GetFeature<DPadInteraction>();
			if (feature != null && feature.enabled)
			{
				OpenXRInput.Internal_SetDpadBindingCustomValues(true, feature.forceThresholdLeft, feature.forceThresholdReleaseLeft, feature.centerRegionLeft, feature.wedgeAngleLeft, feature.isStickyLeft);
				OpenXRInput.Internal_SetDpadBindingCustomValues(false, feature.forceThresholdRight, feature.forceThresholdReleaseRight, feature.centerRegionRight, feature.wedgeAngleRight, feature.isStickyRight);
			}
		}

		private static char SanitizeCharForOpenXRPath(char c)
		{
			if (char.IsLower(c) || char.IsDigit(c))
			{
				return c;
			}
			if (char.IsUpper(c))
			{
				return char.ToLower(c);
			}
			if (c == '-' || c == '.' || c == '_' || c == '/')
			{
				return c;
			}
			return '\0';
		}

		private static string SanitizeStringForOpenXRPath(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return "";
			}
			int i = 0;
			while (i < input.Length && OpenXRInput.SanitizeCharForOpenXRPath(input[i]) == input[i])
			{
				i++;
			}
			if (i == input.Length)
			{
				return input;
			}
			StringBuilder stringBuilder = new StringBuilder(input, 0, i, input.Length);
			while (i < input.Length)
			{
				char c = OpenXRInput.SanitizeCharForOpenXRPath(input[i]);
				if (c != '\0')
				{
					stringBuilder.Append(c);
				}
				i++;
			}
			return stringBuilder.ToString();
		}

		private static string GetActionHandleName(InputControl control)
		{
			InputControl inputControl = control;
			while (inputControl.parent != null && inputControl.parent.parent != null)
			{
				inputControl = inputControl.parent;
			}
			string text = OpenXRInput.SanitizeStringForOpenXRPath(inputControl.name);
			string result;
			if (OpenXRInput.kVirtualControlMap.TryGetValue(text, out result))
			{
				return result;
			}
			return text;
		}

		public static void SendHapticImpulse(InputActionReference actionRef, float amplitude, float duration, InputDevice inputDevice = null)
		{
			OpenXRInput.SendHapticImpulse(actionRef, amplitude, 0f, duration, inputDevice);
		}

		public static void SendHapticImpulse(InputActionReference actionRef, float amplitude, float frequency, float duration, InputDevice inputDevice = null)
		{
			OpenXRInput.SendHapticImpulse(actionRef.action, amplitude, frequency, duration, inputDevice);
		}

		public static void SendHapticImpulse(InputAction action, float amplitude, float duration, InputDevice inputDevice = null)
		{
			OpenXRInput.SendHapticImpulse(action, amplitude, 0f, duration, inputDevice);
		}

		public static void SendHapticImpulse(InputAction action, float amplitude, float frequency, float duration, InputDevice inputDevice = null)
		{
			if (action == null)
			{
				return;
			}
			ulong actionHandle = OpenXRInput.GetActionHandle(action, inputDevice);
			if (actionHandle == 0UL)
			{
				return;
			}
			amplitude = Mathf.Clamp(amplitude, 0f, 1f);
			duration = Mathf.Max(duration, 0f);
			OpenXRInput.Internal_SendHapticImpulse(OpenXRInput.GetDeviceId(inputDevice), actionHandle, amplitude, frequency, duration);
		}

		public static void StopHaptics(InputActionReference actionRef, InputDevice inputDevice = null)
		{
			if (actionRef == null)
			{
				return;
			}
			OpenXRInput.StopHaptics(actionRef.action, inputDevice);
		}

		public static void SendHapticImpulse(InputDevice device, float amplitude, float frequency, float duration)
		{
			if (!device.isValid)
			{
				return;
			}
			OpenXRInput.Internal_SendHapticImpulseNoISX(OpenXRInput.GetDeviceId(device), amplitude, frequency, duration);
		}

		public static void StopHapticImpulse(InputDevice device)
		{
			if (!device.isValid)
			{
				return;
			}
			OpenXRInput.Internal_StopHapticsNoISX(OpenXRInput.GetDeviceId(device));
		}

		public static void StopHaptics(InputAction inputAction, InputDevice inputDevice = null)
		{
			if (inputAction == null)
			{
				return;
			}
			ulong actionHandle = OpenXRInput.GetActionHandle(inputAction, inputDevice);
			if (actionHandle == 0UL)
			{
				return;
			}
			OpenXRInput.Internal_StopHaptics(OpenXRInput.GetDeviceId(inputDevice), actionHandle);
		}

		public static bool TryGetInputSourceName(InputAction inputAction, int index, out string name, OpenXRInput.InputSourceNameFlags flags = OpenXRInput.InputSourceNameFlags.All, InputDevice inputDevice = null)
		{
			name = "";
			if (index < 0)
			{
				return false;
			}
			ulong actionHandle = OpenXRInput.GetActionHandle(inputAction, inputDevice);
			return actionHandle != 0UL && OpenXRInput.Internal_TryGetInputSourceName(OpenXRInput.GetDeviceId(inputDevice), actionHandle, (uint)index, (uint)flags, out name);
		}

		public static bool GetActionIsActive(InputAction inputAction)
		{
			if (inputAction != null && inputAction.controls.Count > 0 && inputAction.controls[0].device != null)
			{
				for (int i = 0; i < inputAction.controls.Count; i++)
				{
					uint deviceId = OpenXRInput.GetDeviceId(inputAction.controls[i].device);
					if (deviceId != 0U)
					{
						string actionHandleName = OpenXRInput.GetActionHandleName(inputAction.controls[i]);
						if (OpenXRInput.Internal_GetActionIsActive(deviceId, actionHandleName))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool GetActionIsActive(InputDevice device, InputFeatureUsage usage)
		{
			return OpenXRInput.GetActionIsActive(device, usage.name);
		}

		public static bool GetActionIsActive(InputDevice device, string usageName)
		{
			uint deviceId = OpenXRInput.GetDeviceId(device);
			return deviceId != 0U && OpenXRInput.Internal_GetActionIsActiveNoISX(deviceId, usageName);
		}

		public static bool TrySetControllerLateLatchAction(InputAction inputAction)
		{
			if (inputAction == null || inputAction.controls.Count != 1)
			{
				return false;
			}
			if (inputAction.controls[0].device == null)
			{
				return false;
			}
			uint deviceId = OpenXRInput.GetDeviceId(inputAction.controls[0].device);
			if (deviceId == 0U)
			{
				return false;
			}
			ulong actionHandle = OpenXRInput.GetActionHandle(inputAction, null);
			return actionHandle != 0UL && OpenXRInput.Internal_TrySetControllerLateLatchAction(deviceId, actionHandle);
		}

		public static bool TrySetControllerLateLatchAction(InputDevice device, InputFeatureUsage usage)
		{
			return OpenXRInput.TrySetControllerLateLatchAction(device, usage.name);
		}

		public static bool TrySetControllerLateLatchAction(InputDevice device, string usageName)
		{
			uint deviceId = OpenXRInput.GetDeviceId(device);
			if (deviceId == 0U)
			{
				return false;
			}
			ulong actionHandle = OpenXRInput.GetActionHandle(device, usageName);
			return actionHandle != 0UL && OpenXRInput.Internal_TrySetControllerLateLatchAction(deviceId, actionHandle);
		}

		public static ulong GetActionHandle(InputDevice device, InputFeatureUsage usage)
		{
			return OpenXRInput.GetActionHandle(device, usage.name);
		}

		public static ulong GetActionHandle(InputDevice device, string usageName)
		{
			uint deviceId = OpenXRInput.GetDeviceId(device);
			if (deviceId == 0U)
			{
				return 0UL;
			}
			return OpenXRInput.Internal_GetActionIdNoISX(deviceId, usageName);
		}

		public static ulong GetActionHandle(InputAction inputAction, InputDevice inputDevice = null)
		{
			if (inputAction == null || inputAction.controls.Count == 0)
			{
				return 0UL;
			}
			foreach (InputControl inputControl in inputAction.controls)
			{
				if ((inputDevice == null || inputControl.device == inputDevice) && inputControl.device != null)
				{
					uint deviceId = OpenXRInput.GetDeviceId(inputControl.device);
					if (deviceId != 0U)
					{
						string actionHandleName = OpenXRInput.GetActionHandleName(inputControl);
						ulong num = OpenXRInput.Internal_GetActionId(deviceId, actionHandleName);
						if (num != 0UL)
						{
							return num;
						}
					}
				}
			}
			return 0UL;
		}

		private static uint GetDeviceId(InputDevice inputDevice)
		{
			if (inputDevice == null)
			{
				return 0U;
			}
			OpenXRInput.GetInternalDeviceIdCommand getInternalDeviceIdCommand = OpenXRInput.GetInternalDeviceIdCommand.Create();
			if (inputDevice.ExecuteCommand<OpenXRInput.GetInternalDeviceIdCommand>(ref getInternalDeviceIdCommand) != 0L)
			{
				return getInternalDeviceIdCommand.deviceId;
			}
			return 0U;
		}

		private static uint GetDeviceId(InputDevice inputDevice)
		{
			return OpenXRInput.Internal_GetDeviceId(inputDevice.characteristics, inputDevice.name);
		}

		private static string UserPathToDeviceName(string userPath)
		{
			string[] array = userPath.Split(new char[]
			{
				'/',
				'_'
			});
			StringBuilder stringBuilder = new StringBuilder("OXR");
			foreach (string text in array)
			{
				if (text.Length != 0)
				{
					string text2 = OpenXRInput.SanitizeStringForOpenXRPath(text);
					stringBuilder.Append(char.ToUpper(text2[0]));
					stringBuilder.Append(text2.Substring(1));
				}
			}
			return stringBuilder.ToString();
		}

		[DllImport("UnityOpenXR", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OpenXRInputProvider_SetDpadBindingCustomValues")]
		private static extern void Internal_SetDpadBindingCustomValues([MarshalAs(UnmanagedType.I1)] bool isLeft, float forceThreshold, float forceThresholdReleased, float centerRegion, float wedgeAngle, [MarshalAs(UnmanagedType.I1)] bool isSticky);

		[DllImport("UnityOpenXR", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OpenXRInputProvider_SendHapticImpulse")]
		private static extern void Internal_SendHapticImpulse(uint deviceId, ulong actionId, float amplitude, float frequency, float duration);

		[DllImport("UnityOpenXR", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OpenXRInputProvider_SendHapticImpulseNoISX")]
		private static extern void Internal_SendHapticImpulseNoISX(uint deviceId, float amplitude, float frequency, float duration);

		[DllImport("UnityOpenXR", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OpenXRInputProvider_StopHaptics")]
		private static extern void Internal_StopHaptics(uint deviceId, ulong actionId);

		[DllImport("UnityOpenXR", CallingConvention = CallingConvention.Cdecl, EntryPoint = "OpenXRInputProvider_StopHapticsNoISX")]
		private static extern void Internal_StopHapticsNoISX(uint deviceId);

		[DllImport("UnityOpenXR", EntryPoint = "OpenXRInputProvider_GetActionIdByControl")]
		private static extern ulong Internal_GetActionId(uint deviceId, string name);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_GetActionIdByUsageName")]
		private static extern ulong Internal_GetActionIdNoISX(uint deviceId, string usageName);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_TryGetInputSourceName")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_TryGetInputSourceNamePtr(uint deviceId, ulong actionId, uint index, uint flags, out IntPtr outName);

		internal static bool Internal_TryGetInputSourceName(uint deviceId, ulong actionId, uint index, uint flags, out string outName)
		{
			IntPtr ptr;
			if (!OpenXRInput.Internal_TryGetInputSourceNamePtr(deviceId, actionId, index, flags, out ptr))
			{
				outName = "";
				return false;
			}
			outName = Marshal.PtrToStringAnsi(ptr);
			return true;
		}

		[DllImport("UnityOpenXR", EntryPoint = "OpenXRInputProvider_TrySetControllerLateLatchAction")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_TrySetControllerLateLatchAction(uint deviceId, ulong actionId);

		[DllImport("UnityOpenXR", EntryPoint = "OpenXRInputProvider_GetActionIsActive")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetActionIsActive(uint deviceId, string name);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_GetActionIsActiveNoISX")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetActionIsActiveNoISX(uint deviceId, string name);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_RegisterDeviceDefinition")]
		private static extern ulong Internal_RegisterDeviceDefinition(string userPath, string interactionProfile, [MarshalAs(UnmanagedType.I1)] bool isAdditive, uint characteristics, string name, string manufacturer, string serialNumber);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_CreateActionSet")]
		private static extern ulong Internal_CreateActionSet(string name, string localizedName, OpenXRInput.SerializedGuid guid);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_CreateAction")]
		private static extern ulong Internal_CreateAction(ulong actionSetId, string name, string localizedName, uint actionType, OpenXRInput.SerializedGuid guid, string[] userPaths, uint userPathCount, [MarshalAs(UnmanagedType.I1)] bool isAdditive, string[] usages, uint usageCount);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_SuggestBindings")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_SuggestBindings(string interactionProfile, OpenXRInput.SerializedBinding[] serializedBindings, uint serializedBindingCount);

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_AttachActionSets")]
		[return: MarshalAs(UnmanagedType.U1)]
		internal static extern bool Internal_AttachActionSets();

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "OpenXRInputProvider_GetDeviceId")]
		private static extern uint Internal_GetDeviceId(InputDeviceCharacteristics characteristics, string name);

		// Note: this type is marked as 'beforefieldinit'.
		static OpenXRInput()
		{
			Dictionary<string, OpenXRInteractionFeature.ActionType> dictionary = new Dictionary<string, OpenXRInteractionFeature.ActionType>();
			dictionary["Digital"] = OpenXRInteractionFeature.ActionType.Binary;
			dictionary["Button"] = OpenXRInteractionFeature.ActionType.Binary;
			dictionary["Axis"] = OpenXRInteractionFeature.ActionType.Axis1D;
			dictionary["Integer"] = OpenXRInteractionFeature.ActionType.Axis1D;
			dictionary["Analog"] = OpenXRInteractionFeature.ActionType.Axis1D;
			dictionary["Vector2"] = OpenXRInteractionFeature.ActionType.Axis2D;
			dictionary["Dpad"] = OpenXRInteractionFeature.ActionType.Axis2D;
			dictionary["Stick"] = OpenXRInteractionFeature.ActionType.Axis2D;
			dictionary["Pose"] = OpenXRInteractionFeature.ActionType.Pose;
			dictionary["Vector3"] = OpenXRInteractionFeature.ActionType.Pose;
			dictionary["Quaternion"] = OpenXRInteractionFeature.ActionType.Pose;
			dictionary["Haptic"] = OpenXRInteractionFeature.ActionType.Vibrate;
			OpenXRInput.ExpectedControlTypeToActionType = dictionary;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2["deviceposition"] = "devicepose";
			dictionary2["devicerotation"] = "devicepose";
			dictionary2["trackingstate"] = "devicepose";
			dictionary2["istracked"] = "devicepose";
			dictionary2["pointerposition"] = "pointer";
			dictionary2["pointerrotation"] = "pointer";
			OpenXRInput.kVirtualControlMap = dictionary2;
		}

		private static readonly Dictionary<string, OpenXRInteractionFeature.ActionType> ExpectedControlTypeToActionType;

		private const string s_devicePoseActionName = "devicepose";

		private const string s_pointerActionName = "pointer";

		private static readonly Dictionary<string, string> kVirtualControlMap;

		private const string Library = "UnityOpenXR";

		[StructLayout(LayoutKind.Explicit)]
		private struct SerializedGuid
		{
			[FieldOffset(0)]
			public Guid guid;

			[FieldOffset(0)]
			public ulong ulong1;

			[FieldOffset(8)]
			public ulong ulong2;
		}

		internal struct SerializedBinding
		{
			public ulong actionId;

			public string path;
		}

		[Flags]
		public enum InputSourceNameFlags
		{
			UserPath = 1,
			InteractionProfile = 2,
			Component = 4,
			All = 7
		}

		[StructLayout(LayoutKind.Explicit, Size = 12)]
		private struct GetInternalDeviceIdCommand : IInputDeviceCommandInfo
		{
			private static FourCC Type
			{
				get
				{
					return new FourCC('X', 'R', 'D', 'I');
				}
			}

			public FourCC typeStatic
			{
				get
				{
					return OpenXRInput.GetInternalDeviceIdCommand.Type;
				}
			}

			public static OpenXRInput.GetInternalDeviceIdCommand Create()
			{
				return new OpenXRInput.GetInternalDeviceIdCommand
				{
					baseCommand = new InputDeviceCommand(OpenXRInput.GetInternalDeviceIdCommand.Type, 12)
				};
			}

			private const int k_BaseCommandSizeSize = 8;

			private const int k_Size = 12;

			[FieldOffset(0)]
			private InputDeviceCommand baseCommand;

			[FieldOffset(8)]
			public readonly uint deviceId;
		}
	}
}
