using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class LanguagePack
	{
		public static Request<AssetDetails> GetCurrent()
		{
			if (Core.IsInitialized())
			{
				return new Request<AssetDetails>(CAPI.ovr_LanguagePack_GetCurrent());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<AssetFileDownloadResult> SetCurrent(string tag)
		{
			if (Core.IsInitialized())
			{
				return new Request<AssetFileDownloadResult>(CAPI.ovr_LanguagePack_SetCurrent(tag));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
