using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public sealed class StandalonePlatform
	{
		public Request<PlatformInitialize> InitializeInEditor()
		{
			string text;
			if (PlatformSettings.UseMobileAppIDInEditor)
			{
				text = PlatformSettings.MobileAppID;
			}
			else
			{
				text = PlatformSettings.AppID;
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new UnityException("Update your App ID by selecting 'Oculus Platform' -> 'Edit Settings'");
			}
			if (string.IsNullOrEmpty(StandalonePlatformSettings.OculusPlatformTestUserAccessToken))
			{
				throw new UnityException("Update your standalone credentials by selecting 'Oculus Platform' -> 'Edit Settings'");
			}
			string oculusPlatformTestUserAccessToken = StandalonePlatformSettings.OculusPlatformTestUserAccessToken;
			return this.AsyncInitialize(ulong.Parse(text), oculusPlatformTestUserAccessToken);
		}

		public Request<PlatformInitialize> AsyncInitialize(ulong appID, string accessToken)
		{
			CAPI.ovr_UnityResetTestPlatform();
			CAPI.ovr_UnityInitGlobals(IntPtr.Zero);
			return new Request<PlatformInitialize>(CAPI.ovr_PlatformInitializeWithAccessToken(appID, accessToken));
		}

		public Request<PlatformInitialize> AsyncInitializeWithAccessTokenAndOptions(string appId, string accessToken, Dictionary<InitConfigOptions, bool> initConfigOptions)
		{
			UIntPtr numOptions = (UIntPtr)((ulong)((long)initConfigOptions.Count));
			CAPI.ovrKeyValuePair[] configOptions = CAPI.DictionaryToOVRKeyValuePairs(initConfigOptions);
			return new Request<PlatformInitialize>(CAPI.ovr_PlatformInitializeWithAccessTokenAndOptions(ulong.Parse(appId), accessToken, configOptions, numOptions));
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void UnityLogDelegate(IntPtr tag, IntPtr msg);
	}
}
