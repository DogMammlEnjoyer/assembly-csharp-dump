using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	internal static class MRUKNative
	{
		[DllImport("kernel32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		private static IntPtr GetDllHandle(string path)
		{
			return MRUKNative.LoadLibrary(path);
		}

		private static IntPtr GetDllExport(IntPtr dllHandle, string name)
		{
			return MRUKNative.GetProcAddress(dllHandle, name);
		}

		private static bool FreeDllHandle(IntPtr dllHandle)
		{
			return MRUKNative.FreeLibrary(dllHandle);
		}

		internal static void LoadMRUKSharedLibrary()
		{
			if (MRUKNative._nativeLibraryPtr != IntPtr.Zero)
			{
				return;
			}
			string text = string.Empty;
			text = Path.Join(Application.dataPath, "Plugins/x86_64/mrutilitykitshared.dll");
			MRUKNative._nativeLibraryPtr = MRUKNative.GetDllHandle(text);
			if (MRUKNative._nativeLibraryPtr == IntPtr.Zero)
			{
				Debug.LogError("Failed to load mr utility kit shared library from '" + text + "'");
				return;
			}
			MRUKNativeFuncs.LoadNativeFunctions();
		}

		internal static void FreeMRUKSharedLibrary()
		{
			MRUKNativeFuncs.UnloadNativeFunctions();
			if (MRUKNative._nativeLibraryPtr == IntPtr.Zero)
			{
				return;
			}
			if (!MRUKNative.FreeDllHandle(MRUKNative._nativeLibraryPtr))
			{
				Debug.LogError("Failed to free mr utility kit shared library");
			}
			MRUKNative._nativeLibraryPtr = IntPtr.Zero;
		}

		internal static T LoadFunction<T>(string name)
		{
			if (MRUKNative._nativeLibraryPtr == IntPtr.Zero)
			{
				Debug.LogWarning("Failed to load " + name + " because mr utility kit shared library is not loaded");
				return default(T);
			}
			IntPtr dllExport = MRUKNative.GetDllExport(MRUKNative._nativeLibraryPtr, name);
			if (dllExport == IntPtr.Zero)
			{
				Debug.LogWarning("Could not find " + name + " in mr utility kit shared library");
				return default(T);
			}
			return Marshal.GetDelegateForFunctionPointer<T>(dllExport);
		}

		private static IntPtr _nativeLibraryPtr;
	}
}
