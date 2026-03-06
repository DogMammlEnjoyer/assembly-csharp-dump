using System;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	public static class PlatformInit
	{
		public static BBPlatformInitStatus status { get; private set; }

		public static void GetEntitlementInformation(Action<PlatformInfo> callback)
		{
			PlatformInit.<>c__DisplayClass5_0 CS$<>8__locals1 = new PlatformInit.<>c__DisplayClass5_0();
			CS$<>8__locals1.callback = callback;
			if (PlatformInit.status == BBPlatformInitStatus.Succeeded)
			{
				CS$<>8__locals1.callback(PlatformInit._info);
				return;
			}
			try
			{
				PlatformInit.status = BBPlatformInitStatus.Initializing;
				Core.AsyncInitialize(null).OnComplete(new Message<PlatformInitialize>.Callback(CS$<>8__locals1.<GetEntitlementInformation>g__InitializeComplete|0));
			}
			catch (Exception ex)
			{
				PlatformInit.status = BBPlatformInitStatus.Failed;
				Debug.LogError(ex.Message + "\n" + ex.StackTrace);
				CS$<>8__locals1.callback(PlatformInit._info);
			}
		}

		private static PlatformInfo _info;
	}
}
