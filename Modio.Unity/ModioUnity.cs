using System;
using Modio.API;
using Modio.API.Interfaces;
using Modio.Extensions;
using Modio.FileIO;
using Modio.Platforms;
using UnityEngine;

namespace Modio.Unity
{
	internal static class ModioUnity
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void OnAfterAssembliesLoaded()
		{
			ModioUnitySettings modioUnitySettings = Resources.Load<ModioUnitySettings>("mod.io/v3_config_local");
			if (modioUnitySettings == null)
			{
				modioUnitySettings = Resources.Load<ModioUnitySettings>("mod.io/v3_config");
			}
			string s;
			if (ModioCommandLine.TryGet("gameid", out s))
			{
				modioUnitySettings.Settings.GameId = (long)int.Parse(s);
			}
			string apikey;
			if (ModioCommandLine.TryGet("apikey", out apikey))
			{
				modioUnitySettings.Settings.APIKey = apikey;
			}
			string serverURL;
			if (ModioCommandLine.TryGet("url", out serverURL))
			{
				modioUnitySettings.Settings.ServerURL = serverURL;
			}
			ModioServices.Bind<IModioLogHandler>().FromNew<ModioUnityLogger>(ModioServicePriority.EngineImplementation, null);
			string text = string.Format("Unity; {0}; {1}", Application.unityVersion, Application.platform);
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log(text);
			}
			Version.AddEnvironmentDetails(text);
			if (modioUnitySettings != null)
			{
				ModioServices.BindInstance<ModioSettings>(modioUnitySettings.Settings, ModioServicePriority.DeveloperOverride);
			}
			else
			{
				ModioLog message = ModioLog.Message;
				if (message != null)
				{
					message.Log("Couldn't find a ModioUnitySettings named 'mod.io/v3_config' to load in a Resources folder");
				}
			}
			ModioServices.Bind<IModioAPIInterface>().FromNew<ModioAPIUnityClient>(ModioServicePriority.EngineImplementation, null);
			ModioServices.Bind<IModioRootPathProvider>().FromNew<WindowsRootPathProvider>(ModioServicePriority.PlatformProvided, new Func<bool>(WindowsRootPathProvider.IsPublicEnvironmentVariableSet));
			if (Application.platform == RuntimePlatform.LinuxPlayer)
			{
				ModioServices.Bind<IModioDataStorage>().FromNew<LinuxDataStorage>(ModioServicePriority.PlatformProvided, null);
			}
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				ModioServices.Bind<IModioDataStorage>().FromNew<MacDataStorage>(ModioServicePriority.PlatformProvided, null);
			}
			ModioServices.Bind<IModioRootPathProvider>().FromNew<UnityRootPathProvider>(ModioServicePriority.Default, null);
			ModioServices.Bind<IWebBrowserHandler>().FromNew<UnityWebBrowserHandler>(ModioServicePriority.EngineImplementation, null);
			ModioServices.BindErrorMessage<ModioSettings>("Please ensure you've bound a ModioSettings. You can create one using the menu item 'Tools/mod.io/Edit Settings'", (ModioServicePriority)1);
			Application.quitting += delegate()
			{
				ModioClient.Shutdown().ForgetTaskSafely();
			};
			ModioUnity.InitPlatform();
		}

		private static void Log(LogLevel logLevel, object message)
		{
			Action<object> action;
			if (logLevel != LogLevel.Error)
			{
				if (logLevel != LogLevel.Warning)
				{
					action = new Action<object>(Debug.Log);
				}
				else
				{
					action = new Action<object>(Debug.LogWarning);
				}
			}
			else
			{
				action = new Action<object>(Debug.LogError);
			}
			action(message);
		}

		private static void InitPlatform()
		{
			RuntimePlatform platform = Application.platform;
			ModioAPI.Platform platform2;
			if (platform <= RuntimePlatform.LinuxPlayer)
			{
				switch (platform)
				{
				case RuntimePlatform.OSXEditor:
					platform2 = ModioAPI.Platform.Mac;
					goto IL_BA;
				case RuntimePlatform.OSXPlayer:
					platform2 = ModioAPI.Platform.Mac;
					goto IL_BA;
				case RuntimePlatform.WindowsPlayer:
					platform2 = ModioAPI.Platform.Windows;
					goto IL_BA;
				case RuntimePlatform.OSXWebPlayer:
				case RuntimePlatform.OSXDashboardPlayer:
				case RuntimePlatform.WindowsWebPlayer:
				case (RuntimePlatform)6:
					break;
				case RuntimePlatform.WindowsEditor:
					platform2 = ModioAPI.Platform.Windows;
					goto IL_BA;
				case RuntimePlatform.IPhonePlayer:
					platform2 = ModioAPI.Platform.IOS;
					goto IL_BA;
				default:
					if (platform == RuntimePlatform.Android)
					{
						platform2 = ModioAPI.Platform.Android;
						goto IL_BA;
					}
					if (platform == RuntimePlatform.LinuxPlayer)
					{
						platform2 = ModioAPI.Platform.Linux;
						goto IL_BA;
					}
					break;
				}
			}
			else if (platform <= RuntimePlatform.PS4)
			{
				if (platform == RuntimePlatform.LinuxEditor)
				{
					platform2 = ModioAPI.Platform.Linux;
					goto IL_BA;
				}
				if (platform == RuntimePlatform.PS4)
				{
					platform2 = ModioAPI.Platform.PlayStation4;
					goto IL_BA;
				}
			}
			else
			{
				if (platform == RuntimePlatform.XboxOne)
				{
					platform2 = ModioAPI.Platform.XboxOne;
					goto IL_BA;
				}
				switch (platform)
				{
				case RuntimePlatform.Switch:
					platform2 = ModioAPI.Platform.Switch;
					goto IL_BA;
				case RuntimePlatform.GameCoreXboxSeries:
					platform2 = ModioAPI.Platform.XboxSeriesX;
					goto IL_BA;
				case RuntimePlatform.GameCoreXboxOne:
					platform2 = ModioAPI.Platform.XboxOne;
					goto IL_BA;
				case RuntimePlatform.PS5:
					platform2 = ModioAPI.Platform.PlayStation5;
					goto IL_BA;
				}
			}
			platform2 = ModioAPI.Platform.None;
			IL_BA:
			ModioAPI.Platform platform3 = platform2;
			if (platform3 != ModioAPI.Platform.None)
			{
				ModioAPI.SetPlatform(platform3);
			}
		}
	}
}
