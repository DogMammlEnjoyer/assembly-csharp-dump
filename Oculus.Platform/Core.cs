using System;
using System.Collections.Generic;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public sealed class Core
	{
		public static bool IsInitialized()
		{
			return Core.IsPlatformInitialized;
		}

		internal static void ForceInitialized()
		{
			Core.IsPlatformInitialized = true;
		}

		private static string getAppID(string appId = null)
		{
			string appIDFromConfig = Core.GetAppIDFromConfig();
			if (string.IsNullOrEmpty(appId))
			{
				if (string.IsNullOrEmpty(appIDFromConfig))
				{
					throw new UnityException("Update your app id by selecting 'Oculus Platform' -> 'Edit Settings'");
				}
				appId = appIDFromConfig;
			}
			else if (!string.IsNullOrEmpty(appIDFromConfig))
			{
				Debug.LogWarningFormat("The 'Oculus App Id ({0})' field in 'Oculus Platform/Edit Settings' is being overridden by the App Id ({1}) that you passed in to Platform.Core.Initialize.  You should only specify this in one place.  We recommend the menu location.", new object[]
				{
					appIDFromConfig,
					appId
				});
			}
			return appId;
		}

		public static Request<PlatformInitialize> AsyncInitialize(string appId = null)
		{
			appId = Core.getAppID(appId);
			Request<PlatformInitialize> request;
			if (Application.isEditor && PlatformSettings.UseStandalonePlatform)
			{
				request = new StandalonePlatform().InitializeInEditor();
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				request = new WindowsPlatform().AsyncInitialize(appId);
			}
			else
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					throw new NotImplementedException("Oculus platform is not implemented on this platform yet.");
				}
				request = new AndroidPlatform().AsyncInitialize(appId);
			}
			Core.IsPlatformInitialized = (request != null);
			if (!Core.IsPlatformInitialized)
			{
				throw new UnityException("Oculus Platform failed to initialize.");
			}
			if (Core.LogMessages)
			{
				Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
			}
			new GameObject("Oculus.Platform.CallbackRunner").AddComponent<CallbackRunner>();
			return request;
		}

		public static Request<PlatformInitialize> AsyncInitialize(string accessToken, Dictionary<InitConfigOptions, bool> initConfigOptions, string appId = null)
		{
			appId = Core.getAppID(appId);
			if (!Application.isEditor && Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer)
			{
				throw new NotImplementedException("Initializing with access token is not implemented on this platform yet.");
			}
			Request<PlatformInitialize> request = new StandalonePlatform().AsyncInitializeWithAccessTokenAndOptions(appId, accessToken, initConfigOptions);
			Core.IsPlatformInitialized = (request != null);
			if (!Core.IsPlatformInitialized)
			{
				throw new UnityException("Oculus Standalone Platform failed to initialize. Check if the access token or app id is correct.");
			}
			if (Core.LogMessages)
			{
				Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
			}
			new GameObject("Oculus.Platform.CallbackRunner").AddComponent<CallbackRunner>();
			return request;
		}

		public static void Initialize(string appId = null)
		{
			appId = Core.getAppID(appId);
			if (Application.isEditor && PlatformSettings.UseStandalonePlatform)
			{
				Core.IsPlatformInitialized = (new StandalonePlatform().InitializeInEditor() != null);
			}
			else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				Core.IsPlatformInitialized = new WindowsPlatform().Initialize(appId);
			}
			else
			{
				if (Application.platform != RuntimePlatform.Android)
				{
					throw new NotImplementedException("Oculus platform is not implemented on this platform yet.");
				}
				Core.IsPlatformInitialized = new AndroidPlatform().Initialize(appId);
			}
			if (!Core.IsPlatformInitialized)
			{
				throw new UnityException("Oculus Platform failed to initialize.");
			}
			if (Core.LogMessages)
			{
				Debug.LogWarning("Oculus.Platform.Core.LogMessages is set to true. This will cause extra heap allocations, and should not be used outside of testing and debugging.");
			}
			new GameObject("Oculus.Platform.CallbackRunner").AddComponent<CallbackRunner>();
		}

		private static string GetAppIDFromConfig()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return PlatformSettings.MobileAppID;
			}
			if (PlatformSettings.UseMobileAppIDInEditor)
			{
				return PlatformSettings.MobileAppID;
			}
			return PlatformSettings.AppID;
		}

		private static bool IsPlatformInitialized = false;

		public static bool LogMessages = false;

		public static string PlatformUninitializedError = "This function requires an initialized Oculus Platform. Run Oculus.Platform.Core.[Initialize|AsyncInitialize] and try again.";
	}
}
