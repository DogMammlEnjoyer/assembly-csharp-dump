using System;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeData.h")]
	[Flags]
	public enum AccessibilityRole : ushort
	{
		None = 0,
		Button = 1,
		Image = 2,
		StaticText = 4,
		SearchField = 8,
		KeyboardKey = 16,
		Header = 32,
		TabBar = 64,
		Slider = 128,
		Toggle = 256
	}
}
