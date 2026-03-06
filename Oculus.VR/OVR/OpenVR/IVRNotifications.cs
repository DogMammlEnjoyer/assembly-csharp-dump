using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	public struct IVRNotifications
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRNotifications._CreateNotification CreateNotification;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRNotifications._RemoveNotification RemoveNotification;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRNotificationError _CreateNotification(ulong ulOverlayHandle, ulong ulUserValue, EVRNotificationType type, string pchText, EVRNotificationStyle style, ref NotificationBitmap_t pImage, ref uint pNotificationId);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRNotificationError _RemoveNotification(uint notificationId);
	}
}
