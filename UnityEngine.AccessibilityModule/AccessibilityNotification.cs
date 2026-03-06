using System;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNotificationContext.h")]
	internal enum AccessibilityNotification
	{
		None,
		Announcement,
		AnnouncementFinished,
		ScreenReaderStatusChanged,
		ScreenChanged,
		LayoutChanged,
		PageScrolled,
		ElementFocused,
		ElementUnfocused,
		FontScaleChanged,
		BoldTextStatusChanged,
		ClosedCaptioningStatusChanged
	}
}
