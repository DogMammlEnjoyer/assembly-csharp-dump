using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Backtrace.Unity.Common
{
	internal static class SystemHelper
	{
		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern uint GetCurrentThreadId();

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string lpFileName);

		internal static bool IsLibraryAvailable(string libraryName)
		{
			try
			{
				return SystemHelper.LoadLibrary(libraryName) != IntPtr.Zero;
			}
			catch (TypeLoadException)
			{
			}
			catch (Exception)
			{
			}
			return false;
		}

		internal static bool IsLibraryAvailable(string[] libraries)
		{
			if (libraries == null || libraries.Length == 0)
			{
				return true;
			}
			return !libraries.Any((string n) => !SystemHelper.IsLibraryAvailable(n));
		}

		internal static string Name()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				return "Mac OS";
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
			case RuntimePlatform.MetroPlayerARM:
				return "Windows";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.PS3:
				return "ps3";
			case RuntimePlatform.XBOX360:
			case RuntimePlatform.XboxOne:
				return "Xbox";
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				return "Linux";
			case RuntimePlatform.WebGLPlayer:
				return "WebGL";
			case RuntimePlatform.TizenPlayer:
			case RuntimePlatform.SamsungTVPlayer:
				return "Samsung TV";
			case RuntimePlatform.PS4:
				return "ps4";
			case RuntimePlatform.WiiU:
				return "WiiU";
			case RuntimePlatform.tvOS:
				return "tvOS";
			case RuntimePlatform.Switch:
				return "switch";
			}
			return Application.platform.ToString();
		}

		internal static string CpuArchitecture()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			if (environmentVariable == null)
			{
				return null;
			}
			return environmentVariable.ToLower();
		}
	}
}
