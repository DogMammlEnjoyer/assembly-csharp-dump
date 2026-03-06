using System;

namespace System.Drawing
{
	internal struct GdiplusStartupOutput
	{
		internal static GdiplusStartupOutput MakeGdiplusStartupOutput()
		{
			GdiplusStartupOutput result = default(GdiplusStartupOutput);
			result.NotificationHook = (result.NotificationUnhook = IntPtr.Zero);
			return result;
		}

		internal IntPtr NotificationHook;

		internal IntPtr NotificationUnhook;
	}
}
