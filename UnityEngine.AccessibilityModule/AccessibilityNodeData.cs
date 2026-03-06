using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[RequiredByNativeCode]
	[NativeType(CodegenOptions.Custom, "MonoAccessibilityNodeData")]
	[NativeHeader("Modules/Accessibility/Bindings/AccessibilityNodeData.bindings.h")]
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeData.h")]
	internal struct AccessibilityNodeData
	{
		public int id { readonly get; set; }

		public bool isActive { readonly get; set; }

		public string label { readonly get; set; }

		public string value { readonly get; set; }

		public string hint { readonly get; set; }

		public AccessibilityRole role { readonly get; set; }

		public bool allowsDirectInteraction { readonly get; set; }

		public AccessibilityState state { readonly get; set; }

		public Rect frame { readonly get; set; }

		public int parentId { readonly get; set; }

		public int[] childIds { readonly get; set; }

		public readonly bool isFocused { get; }

		internal SystemLanguage language { readonly get; set; }

		public bool implementsSelected { readonly get; set; }

		public bool implementsDismissed { readonly get; set; }
	}
}
