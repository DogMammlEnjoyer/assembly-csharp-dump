using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal static class DisplayUtility
	{
		public static float screenDpi
		{
			get
			{
				DisplayUtility.CacheScreenDpi();
				return DisplayUtility.s_ScreenDpi;
			}
		}

		public static float screenDpiRatio
		{
			get
			{
				DisplayUtility.CacheScreenDpi();
				return DisplayUtility.s_OneOverScreenDpi;
			}
		}

		private static void CacheScreenDpi()
		{
			if (!DisplayUtility.s_ScreenDpiChecked)
			{
				if (Screen.dpi > 0f)
				{
					DisplayUtility.s_ScreenDpi = Screen.dpi;
				}
				else
				{
					Debug.LogWarning("Platform has reported a screen DPI of 0. Using default value of 100.");
				}
				DisplayUtility.s_OneOverScreenDpi = 1f / DisplayUtility.s_ScreenDpi;
				DisplayUtility.s_ScreenDpiChecked = true;
			}
		}

		public static float PixelsToInches(float pixels)
		{
			return pixels * DisplayUtility.screenDpiRatio;
		}

		public static float InchesToPixels(float inches)
		{
			return inches * DisplayUtility.screenDpi;
		}

		private static float s_ScreenDpi = 100f;

		private static float s_OneOverScreenDpi = 1f / DisplayUtility.s_ScreenDpi;

		private static bool s_ScreenDpiChecked;
	}
}
