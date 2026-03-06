using System;
using System.Diagnostics;

namespace Fusion
{
	public static class RuntimeUnityFlagsSetup
	{
		internal static bool IsUNITY_WEBGL
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildFlags & RuntimeFlagsBuildFlags.UNITY_WEBGL) == RuntimeFlagsBuildFlags.UNITY_WEBGL;
			}
		}

		internal static bool IsUNITY_XBOXONE
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildFlags & RuntimeFlagsBuildFlags.UNITY_XBOXONE) == RuntimeFlagsBuildFlags.UNITY_XBOXONE;
			}
		}

		internal static bool IsUNITY_GAMECORE
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildFlags & RuntimeFlagsBuildFlags.UNITY_GAMECORE) == RuntimeFlagsBuildFlags.UNITY_GAMECORE;
			}
		}

		internal static bool IsUNITY_EDITOR
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildFlags & RuntimeFlagsBuildFlags.UNITY_EDITOR) == RuntimeFlagsBuildFlags.UNITY_EDITOR;
			}
		}

		internal static bool IsUNITY_SWITCH
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildFlags & RuntimeFlagsBuildFlags.UNITY_SWITCH) == RuntimeFlagsBuildFlags.UNITY_SWITCH;
			}
		}

		internal static bool IsUNITY_2019_4_OR_NEWER
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildFlags & RuntimeFlagsBuildFlags.UNITY_2019_4_OR_NEWER) == RuntimeFlagsBuildFlags.UNITY_2019_4_OR_NEWER;
			}
		}

		[Conditional("UNITY_WEBGL")]
		public static void Check_UNITY_WEBGL()
		{
			RuntimeUnityFlagsSetup.flagsBuildFlags |= RuntimeFlagsBuildFlags.UNITY_WEBGL;
		}

		[Conditional("UNITY_XBOXONE")]
		public static void Check_UNITY_XBOXONE()
		{
			RuntimeUnityFlagsSetup.flagsBuildFlags |= RuntimeFlagsBuildFlags.UNITY_XBOXONE;
		}

		[Conditional("UNITY_GAMECORE")]
		public static void Check_UNITY_GAMECORE()
		{
			RuntimeUnityFlagsSetup.flagsBuildFlags |= RuntimeFlagsBuildFlags.UNITY_GAMECORE;
		}

		[Conditional("UNITY_EDITOR")]
		public static void Check_UNITY_EDITOR()
		{
			RuntimeUnityFlagsSetup.flagsBuildFlags |= RuntimeFlagsBuildFlags.UNITY_EDITOR;
		}

		[Conditional("UNITY_SWITCH")]
		public static void Check_UNITY_SWITCH()
		{
			RuntimeUnityFlagsSetup.flagsBuildFlags |= RuntimeFlagsBuildFlags.UNITY_SWITCH;
		}

		[Conditional("UNITY_2019_4_OR_NEWER")]
		public static void Check_UNITY_2019_4_OR_NEWER()
		{
			RuntimeUnityFlagsSetup.flagsBuildFlags |= RuntimeFlagsBuildFlags.UNITY_2019_4_OR_NEWER;
		}

		internal static bool IsENABLE_MONO
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildTypes & RuntimeFlagsBuildTypes.ENABLE_MONO) == RuntimeFlagsBuildTypes.ENABLE_MONO;
			}
		}

		internal static bool IsENABLE_IL2CPP
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsBuildTypes & RuntimeFlagsBuildTypes.ENABLE_IL2CPP) == RuntimeFlagsBuildTypes.ENABLE_IL2CPP;
			}
		}

		[Conditional("ENABLE_MONO")]
		public static void Check_ENABLE_MONO()
		{
			RuntimeUnityFlagsSetup.flagsBuildTypes |= RuntimeFlagsBuildTypes.ENABLE_MONO;
		}

		[Conditional("ENABLE_IL2CPP")]
		public static void Check_ENABLE_IL2CPP()
		{
			RuntimeUnityFlagsSetup.flagsBuildTypes |= RuntimeFlagsBuildTypes.ENABLE_IL2CPP;
		}

		internal static bool IsNET_4_6
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsDotNetVersion & RuntimeFlagsDotNetVersion.NET_4_6) == RuntimeFlagsDotNetVersion.NET_4_6;
			}
		}

		internal static bool IsNETFX_CORE
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsDotNetVersion & RuntimeFlagsDotNetVersion.NETFX_CORE) == RuntimeFlagsDotNetVersion.NETFX_CORE;
			}
		}

		internal static bool IsNET_STANDARD_2_0
		{
			get
			{
				return (RuntimeUnityFlagsSetup.flagsDotNetVersion & RuntimeFlagsDotNetVersion.NET_STANDARD_2_0) == RuntimeFlagsDotNetVersion.NET_STANDARD_2_0;
			}
		}

		[Conditional("NET_4_6")]
		public static void Check_NET_4_6()
		{
			RuntimeUnityFlagsSetup.flagsDotNetVersion |= RuntimeFlagsDotNetVersion.NET_4_6;
		}

		[Conditional("NETFX_CORE")]
		public static void Check_NETFX_CORE()
		{
			RuntimeUnityFlagsSetup.flagsDotNetVersion |= RuntimeFlagsDotNetVersion.NETFX_CORE;
		}

		[Conditional("NET_STANDARD_2_0")]
		public static void Check_NET_STANDARD_2_0()
		{
			RuntimeUnityFlagsSetup.flagsDotNetVersion |= RuntimeFlagsDotNetVersion.NET_STANDARD_2_0;
		}

		public static void Reset()
		{
			RuntimeUnityFlagsSetup.flagsBuildFlags = RuntimeFlagsBuildFlags.NONE;
			RuntimeUnityFlagsSetup.flagsBuildTypes = RuntimeFlagsBuildTypes.NONE;
			RuntimeUnityFlagsSetup.flagsDotNetVersion = RuntimeFlagsDotNetVersion.NONE;
		}

		private static RuntimeFlagsBuildFlags flagsBuildFlags;

		private static RuntimeFlagsBuildTypes flagsBuildTypes;

		private static RuntimeFlagsDotNetVersion flagsDotNetVersion;
	}
}
