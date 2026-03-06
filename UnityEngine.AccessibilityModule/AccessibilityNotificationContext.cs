using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[RequiredByNativeCode]
	[NativeType(CodegenOptions.Custom, "MonoAccessibilityNotificationContext")]
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNotificationContext.h")]
	[NativeHeader("Modules/Accessibility/Bindings/AccessibilityNotificationContext.bindings.h")]
	internal struct AccessibilityNotificationContext
	{
		public AccessibilityNotification notification { readonly get; set; }

		public readonly bool isScreenReaderEnabled { get; }

		public string announcement { readonly get; set; }

		public readonly bool wasAnnouncementSuccessful { get; }

		public readonly int currentNodeId { get; }

		public int nextNodeId { readonly get; set; }
	}
}
