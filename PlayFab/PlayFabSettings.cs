using System;
using System.Collections.Generic;
using System.Text;
using PlayFab.Internal;
using UnityEngine;

namespace PlayFab
{
	public static class PlayFabSettings
	{
		private static PlayFabSharedSettings PlayFabSharedPrivate
		{
			get
			{
				if (PlayFabSettings._playFabShared == null)
				{
					PlayFabSettings._playFabShared = PlayFabSettings.GetSharedSettingsObjectPrivate();
				}
				return PlayFabSettings._playFabShared;
			}
		}

		private static PlayFabSharedSettings GetSharedSettingsObjectPrivate()
		{
			PlayFabSharedSettings[] array = Resources.LoadAll<PlayFabSharedSettings>("PlayFabSharedSettings");
			if (array.Length != 1)
			{
				Debug.LogWarning("The number of PlayFabSharedSettings objects should be 1: " + array.Length.ToString());
				Debug.LogWarning("If you are upgrading your SDK, you can ignore this warning as PlayFabSharedSettings will be imported soon. If you are not upgrading your SDK and you see this message, you should re-download the latest PlayFab source code.");
			}
			return array[0];
		}

		public static string DeviceUniqueIdentifier
		{
			get
			{
				return SystemInfo.deviceUniqueIdentifier;
			}
		}

		public static string TitleId
		{
			get
			{
				return PlayFabSettings.staticSettings.TitleId;
			}
			set
			{
				PlayFabSettings.staticSettings.TitleId = value;
			}
		}

		internal static string VerticalName
		{
			get
			{
				return PlayFabSettings.staticSettings.VerticalName;
			}
			set
			{
				PlayFabSettings.staticSettings.VerticalName = value;
			}
		}

		public static bool DisableAdvertising
		{
			get
			{
				return PlayFabSettings.staticSettings.DisableAdvertising;
			}
			set
			{
				PlayFabSettings.staticSettings.DisableAdvertising = value;
			}
		}

		public static bool DisableDeviceInfo
		{
			get
			{
				return PlayFabSettings.staticSettings.DisableDeviceInfo;
			}
			set
			{
				PlayFabSettings.staticSettings.DisableDeviceInfo = value;
			}
		}

		public static bool DisableFocusTimeCollection
		{
			get
			{
				return PlayFabSettings.staticSettings.DisableFocusTimeCollection;
			}
			set
			{
				PlayFabSettings.staticSettings.DisableFocusTimeCollection = value;
			}
		}

		[Obsolete("LogLevel has been deprecated, please use UnityEngine.Debug.Log for your logging needs.")]
		public static PlayFabLogLevel LogLevel
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.LogLevel;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.LogLevel = value;
			}
		}

		public static WebRequestType RequestType
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.RequestType;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.RequestType = value;
			}
		}

		public static int RequestTimeout
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.RequestTimeout;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.RequestTimeout = value;
			}
		}

		public static bool RequestKeepAlive
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.RequestKeepAlive;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.RequestKeepAlive = value;
			}
		}

		public static bool CompressApiData
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.CompressApiData;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.CompressApiData = value;
			}
		}

		public static string LoggerHost
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.LoggerHost;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.LoggerHost = value;
			}
		}

		public static int LoggerPort
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.LoggerPort;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.LoggerPort = value;
			}
		}

		public static bool EnableRealTimeLogging
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.EnableRealTimeLogging;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.EnableRealTimeLogging = value;
			}
		}

		public static int LogCapLimit
		{
			get
			{
				return PlayFabSettings.PlayFabSharedPrivate.LogCapLimit;
			}
			set
			{
				PlayFabSettings.PlayFabSharedPrivate.LogCapLimit = value;
			}
		}

		public static string LocalApiServer
		{
			get
			{
				return PlayFabSettings._localApiServer ?? PlayFabUtil.GetLocalSettingsFileProperty("LocalApiServer");
			}
			set
			{
				PlayFabSettings._localApiServer = value;
			}
		}

		public static string GetFullUrl(string apiCall, Dictionary<string, string> getParams, PlayFabApiSettings apiSettings = null)
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			string text = null;
			string text2 = null;
			string text3 = null;
			if (apiSettings != null)
			{
				if (!string.IsNullOrEmpty(apiSettings.ProductionEnvironmentUrl))
				{
					text = apiSettings.ProductionEnvironmentUrl;
				}
				if (!string.IsNullOrEmpty(apiSettings.VerticalName))
				{
					text2 = apiSettings.VerticalName;
				}
				if (!string.IsNullOrEmpty(apiSettings.TitleId))
				{
					text3 = apiSettings.TitleId;
				}
			}
			if (text == null)
			{
				text = ((!string.IsNullOrEmpty(PlayFabSettings.PlayFabSharedPrivate.ProductionEnvironmentUrl)) ? PlayFabSettings.PlayFabSharedPrivate.ProductionEnvironmentUrl : "playfabapi.com");
			}
			if (text2 == null && apiSettings != null && !string.IsNullOrEmpty(apiSettings.VerticalName))
			{
				text2 = apiSettings.VerticalName;
			}
			if (text3 == null)
			{
				text3 = PlayFabSettings.PlayFabSharedPrivate.TitleId;
			}
			string text4 = text;
			if (!text4.StartsWith("http"))
			{
				stringBuilder.Append("https://");
				if (!string.IsNullOrEmpty(text3))
				{
					stringBuilder.Append(text3).Append(".");
				}
				if (!string.IsNullOrEmpty(text2))
				{
					stringBuilder.Append(text2).Append(".");
				}
			}
			stringBuilder.Append(text4).Append(apiCall);
			if (getParams != null)
			{
				bool flag = true;
				foreach (KeyValuePair<string, string> keyValuePair in getParams)
				{
					if (flag)
					{
						stringBuilder.Append("?");
						flag = false;
					}
					else
					{
						stringBuilder.Append("&");
					}
					stringBuilder.Append(keyValuePair.Key).Append("=").Append(keyValuePair.Value);
				}
			}
			return stringBuilder.ToString();
		}

		private static PlayFabSharedSettings _playFabShared = null;

		public static readonly PlayFabApiSettings staticSettings = new PlayFabSettingsRedirect(() => PlayFabSettings.PlayFabSharedPrivate);

		public static PlayFabAuthenticationContext staticPlayer = new PlayFabAuthenticationContext();

		public const string SdkVersion = "2.87.200602";

		public const string BuildIdentifier = "jbuild_unitysdk__sdk-unity-3-slave_0";

		public const string VersionString = "UnitySDK-2.87.200602";

		public const string AD_TYPE_IDFA = "Idfa";

		public const string AD_TYPE_ANDROID_ID = "Adid";

		public const string DefaultPlayFabApiUrl = "playfabapi.com";

		private static string _localApiServer;
	}
}
