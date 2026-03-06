using System;
using System.Text;
using Meta.WitAi;
using UnityEngine;

namespace Oculus.Voice
{
	public static class VoiceSDKConstants
	{
		static VoiceSDKConstants()
		{
			VoiceSDKConstants.Init();
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			if (VoiceSDKConstants._isInitialized)
			{
				return;
			}
			VoiceSDKConstants._isInitialized = true;
			WitRequestSettings.OnProvideCustomUserAgent = (Action<StringBuilder>)Delegate.Combine(WitRequestSettings.OnProvideCustomUserAgent, new Action<StringBuilder>(VoiceSDKConstants.OnCustomUserAgent));
		}

		public static string SdkVersion
		{
			get
			{
				if (string.IsNullOrEmpty(VoiceSDKConstants._sdkVersion))
				{
					VoiceSDKConstants._sdkVersion = "78.0.0";
				}
				return VoiceSDKConstants._sdkVersion;
			}
		}

		private static void OnCustomUserAgent(StringBuilder sb)
		{
			if (!sb.ToString().StartsWith("voice-sdk-"))
			{
				sb.Insert(0, "voice-sdk-" + VoiceSDKConstants.SdkVersion + ",");
			}
		}

		private static bool _isInitialized = false;

		private static string _sdkVersion = "78.0.0.8.295";

		private const string _userAgentPrefix = "voice-sdk-";
	}
}
