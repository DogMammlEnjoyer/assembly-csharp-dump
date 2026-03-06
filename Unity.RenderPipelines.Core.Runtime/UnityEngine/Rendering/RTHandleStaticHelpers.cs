using System;

namespace UnityEngine.Rendering
{
	public struct RTHandleStaticHelpers
	{
		public static void SetRTHandleStaticWrapper(RenderTargetIdentifier rtId)
		{
			if (RTHandleStaticHelpers.s_RTHandleWrapper == null)
			{
				RTHandleStaticHelpers.s_RTHandleWrapper = RTHandles.Alloc(rtId);
				return;
			}
			RTHandleStaticHelpers.s_RTHandleWrapper.SetTexture(rtId);
		}

		public static void SetRTHandleUserManagedWrapper(ref RTHandle rtWrapper, RenderTargetIdentifier rtId)
		{
			if (rtWrapper == null)
			{
				return;
			}
			rtWrapper.SetTexture(rtId);
		}

		public static RTHandle s_RTHandleWrapper;
	}
}
