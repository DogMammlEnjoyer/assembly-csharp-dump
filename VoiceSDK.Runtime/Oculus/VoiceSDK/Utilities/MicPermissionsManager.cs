using System;

namespace Oculus.VoiceSDK.Utilities
{
	public static class MicPermissionsManager
	{
		public static bool HasMicPermission()
		{
			return true;
		}

		public static void RequestMicPermission(Action<string> permissionGrantedCallback = null)
		{
			if (permissionGrantedCallback != null)
			{
				permissionGrantedCallback("android.permission.RECORD_AUDIO");
			}
		}
	}
}
