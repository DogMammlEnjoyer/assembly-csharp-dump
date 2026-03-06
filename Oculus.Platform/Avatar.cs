using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Avatar
	{
		public static Request<AvatarEditorResult> LaunchAvatarEditor(AvatarEditorOptions options = null)
		{
			if (Core.IsInitialized())
			{
				return new Request<AvatarEditorResult>(CAPI.ovr_Avatar_LaunchAvatarEditor((IntPtr)options));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
