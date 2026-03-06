using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilitySettings.h")]
	public static class AccessibilitySettings
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFontScale();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsBoldTextEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsClosedCaptioningEnabled();

		[RequiredByNativeCode]
		private static void Internal_OnFontScaleChanged(float newFontScale)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = AccessibilityNotification.FontScaleChanged,
				fontScale = newFontScale
			});
		}

		[RequiredByNativeCode]
		private static void Internal_OnBoldTextStatusChanged(bool enabled)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = AccessibilityNotification.BoldTextStatusChanged,
				isBoldTextEnabled = enabled
			});
		}

		[RequiredByNativeCode]
		private static void Internal_OnClosedCaptioningStatusChanged(bool enabled)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = AccessibilityNotification.ClosedCaptioningStatusChanged,
				isClosedCaptioningEnabled = enabled
			});
		}

		public static float fontScale
		{
			get
			{
				return AccessibilitySettings.GetFontScale();
			}
		}

		public static bool isBoldTextEnabled
		{
			get
			{
				return AccessibilitySettings.IsBoldTextEnabled();
			}
		}

		public static bool isClosedCaptioningEnabled
		{
			get
			{
				return AccessibilitySettings.IsClosedCaptioningEnabled();
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<float> fontScaleChanged;

		internal static void InvokeFontScaleChanged(float newFontScale)
		{
			Action<float> action = AccessibilitySettings.fontScaleChanged;
			if (action != null)
			{
				action(newFontScale);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> boldTextStatusChanged;

		internal static void InvokeBoldTextStatusChanged(bool enabled)
		{
			Action<bool> action = AccessibilitySettings.boldTextStatusChanged;
			if (action != null)
			{
				action(enabled);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> closedCaptioningStatusChanged;

		internal static void InvokeClosedCaptionStatusChanged(bool enabled)
		{
			Action<bool> action = AccessibilitySettings.closedCaptioningStatusChanged;
			if (action != null)
			{
				action(enabled);
			}
		}
	}
}
