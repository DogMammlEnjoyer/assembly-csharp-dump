using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.OpenXR
{
	public static class OpenXRRuntime
	{
		public static string name
		{
			get
			{
				IntPtr ptr;
				if (!OpenXRRuntime.Internal_GetRuntimeName(out ptr))
				{
					return "";
				}
				return Marshal.PtrToStringAnsi(ptr);
			}
		}

		public static string version
		{
			get
			{
				ushort num;
				ushort num2;
				uint num3;
				if (!OpenXRRuntime.Internal_GetRuntimeVersion(out num, out num2, out num3))
				{
					return "";
				}
				return string.Format("{0}.{1}.{2}", num, num2, num3);
			}
		}

		public static string apiVersion
		{
			get
			{
				ushort num;
				ushort num2;
				uint num3;
				if (!OpenXRRuntime.Internal_GetAPIVersion(out num, out num2, out num3))
				{
					return "";
				}
				return string.Format("{0}.{1}.{2}", num, num2, num3);
			}
		}

		public static string pluginVersion
		{
			get
			{
				IntPtr ptr;
				if (!OpenXRRuntime.Internal_GetPluginVersion(out ptr))
				{
					return "";
				}
				return Marshal.PtrToStringAnsi(ptr);
			}
		}

		internal static bool isRuntimeAPIVersionGreaterThan1_1()
		{
			ushort num;
			ushort num2;
			uint num3;
			return OpenXRRuntime.Internal_GetAPIVersion(out num, out num2, out num3) && num >= 1 && num2 >= 1;
		}

		public static bool IsExtensionEnabled(string extensionName)
		{
			return OpenXRRuntime.Internal_IsExtensionEnabled(extensionName);
		}

		public static uint GetExtensionVersion(string extensionName)
		{
			return OpenXRRuntime.Internal_GetExtensionVersion(extensionName);
		}

		public static string[] GetEnabledExtensions()
		{
			string[] array = new string[OpenXRRuntime.Internal_GetEnabledExtensionCount()];
			for (int i = 0; i < array.Length; i++)
			{
				string text;
				OpenXRRuntime.Internal_GetEnabledExtensionName((uint)i, out text);
				array[i] = (text ?? "");
			}
			return array;
		}

		public static string[] GetAvailableExtensions()
		{
			string[] array = new string[OpenXRRuntime.Internal_GetAvailableExtensionCount()];
			for (int i = 0; i < array.Length; i++)
			{
				string text;
				OpenXRRuntime.Internal_GetAvailableExtensionName((uint)i, out text);
				array[i] = (text ?? "");
			}
			return array;
		}

		public static event Func<bool> wantsToQuit;

		public static event Func<bool> wantsToRestart;

		public static bool retryInitializationOnFormFactorErrors
		{
			get
			{
				return OpenXRRuntime.Internal_GetSoftRestartLoopAtInitialization();
			}
			set
			{
				OpenXRRuntime.Internal_SetSoftRestartLoopAtInitialization(value);
			}
		}

		private static bool InvokeEvent(Func<bool> func)
		{
			if (func == null)
			{
				return true;
			}
			foreach (Func<bool> func2 in func.GetInvocationList())
			{
				try
				{
					if (!func2())
					{
						return false;
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			return true;
		}

		internal static bool ShouldQuit()
		{
			return OpenXRRuntime.InvokeEvent(OpenXRRuntime.wantsToQuit);
		}

		internal static bool ShouldRestart()
		{
			return OpenXRRuntime.InvokeEvent(OpenXRRuntime.wantsToRestart);
		}

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetRuntimeName")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetRuntimeName(out IntPtr runtimeNamePtr);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetRuntimeVersion")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetRuntimeVersion(out ushort major, out ushort minor, out uint patch);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetAPIVersion")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetAPIVersion(out ushort major, out ushort minor, out uint patch);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetPluginVersion")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetPluginVersion(out IntPtr pluginVersionPtr);

		[DllImport("UnityOpenXR", EntryPoint = "unity_ext_IsExtensionEnabled")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_IsExtensionEnabled(string extensionName);

		[DllImport("UnityOpenXR", EntryPoint = "unity_ext_GetExtensionVersion")]
		private static extern uint Internal_GetExtensionVersion(string extensionName);

		[DllImport("UnityOpenXR", EntryPoint = "unity_ext_GetEnabledExtensionCount")]
		private static extern uint Internal_GetEnabledExtensionCount();

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "unity_ext_GetEnabledExtensionName")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetEnabledExtensionNamePtr(uint index, out IntPtr outName);

		[DllImport("UnityOpenXR", EntryPoint = "session_SetSoftRestartLoopAtInitialization")]
		private static extern void Internal_SetSoftRestartLoopAtInitialization([MarshalAs(UnmanagedType.I1)] bool value);

		[DllImport("UnityOpenXR", EntryPoint = "session_GetSoftRestartLoopAtInitialization")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetSoftRestartLoopAtInitialization();

		private static bool Internal_GetEnabledExtensionName(uint index, out string extensionName)
		{
			IntPtr ptr;
			if (!OpenXRRuntime.Internal_GetEnabledExtensionNamePtr(index, out ptr))
			{
				extensionName = "";
				return false;
			}
			extensionName = Marshal.PtrToStringAnsi(ptr);
			return true;
		}

		[DllImport("UnityOpenXR", EntryPoint = "unity_ext_GetAvailableExtensionCount")]
		private static extern uint Internal_GetAvailableExtensionCount();

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "unity_ext_GetAvailableExtensionName")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetAvailableExtensionNamePtr(uint index, out IntPtr extensionName);

		private static bool Internal_GetAvailableExtensionName(uint index, out string extensionName)
		{
			IntPtr ptr;
			if (!OpenXRRuntime.Internal_GetAvailableExtensionNamePtr(index, out ptr))
			{
				extensionName = "";
				return false;
			}
			extensionName = Marshal.PtrToStringAnsi(ptr);
			return true;
		}

		[DllImport("UnityOpenXR", CharSet = CharSet.Ansi, EntryPoint = "session_GetLastError")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetLastError(out IntPtr error);

		internal static bool GetLastError(out string error)
		{
			IntPtr ptr;
			if (!OpenXRRuntime.Internal_GetLastError(out ptr))
			{
				error = "";
				return false;
			}
			error = Marshal.PtrToStringAnsi(ptr);
			return true;
		}

		internal static void LogLastError()
		{
			string message;
			if (OpenXRRuntime.GetLastError(out message))
			{
				Debug.LogError(message);
			}
		}

		private const string LibraryName = "UnityOpenXR";
	}
}
