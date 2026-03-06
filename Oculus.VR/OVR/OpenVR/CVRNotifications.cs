using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	public class CVRNotifications
	{
		internal CVRNotifications(IntPtr pInterface)
		{
			this.FnTable = (IVRNotifications)Marshal.PtrToStructure(pInterface, typeof(IVRNotifications));
		}

		public EVRNotificationError CreateNotification(ulong ulOverlayHandle, ulong ulUserValue, EVRNotificationType type, string pchText, EVRNotificationStyle style, ref NotificationBitmap_t pImage, ref uint pNotificationId)
		{
			pNotificationId = 0U;
			return this.FnTable.CreateNotification(ulOverlayHandle, ulUserValue, type, pchText, style, ref pImage, ref pNotificationId);
		}

		public EVRNotificationError RemoveNotification(uint notificationId)
		{
			return this.FnTable.RemoveNotification(notificationId);
		}

		private IVRNotifications FnTable;
	}
}
