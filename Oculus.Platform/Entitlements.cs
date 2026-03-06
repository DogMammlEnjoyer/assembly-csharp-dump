using System;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Entitlements
	{
		public static Request IsUserEntitledToApplication()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Entitlement_GetIsViewerEntitled());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
