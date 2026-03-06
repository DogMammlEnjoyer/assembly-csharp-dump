using System;
using System.ComponentModel;

namespace UnityEngine
{
	public enum EventType
	{
		MouseDown,
		MouseUp,
		MouseMove,
		MouseDrag,
		KeyDown,
		KeyUp,
		ScrollWheel,
		Repaint,
		Layout,
		DragUpdated,
		DragPerform,
		DragExited = 15,
		Ignore = 11,
		Used,
		ValidateCommand,
		ExecuteCommand,
		ContextClick = 16,
		MouseEnterWindow = 20,
		MouseLeaveWindow,
		TouchDown = 30,
		TouchUp,
		TouchMove,
		TouchEnter,
		TouchLeave,
		TouchStationary,
		[Obsolete("Use MouseDown instead (UnityUpgradable) -> MouseDown", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		mouseDown = 0,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use MouseUp instead (UnityUpgradable) -> MouseUp", true)]
		mouseUp,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use MouseMove instead (UnityUpgradable) -> MouseMove", true)]
		mouseMove,
		[Obsolete("Use MouseDrag instead (UnityUpgradable) -> MouseDrag", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		mouseDrag,
		[Obsolete("Use KeyDown instead (UnityUpgradable) -> KeyDown", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		keyDown,
		[Obsolete("Use KeyUp instead (UnityUpgradable) -> KeyUp", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		keyUp,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use ScrollWheel instead (UnityUpgradable) -> ScrollWheel", true)]
		scrollWheel,
		[Obsolete("Use Repaint instead (UnityUpgradable) -> Repaint", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		repaint,
		[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		layout,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DragUpdated instead (UnityUpgradable) -> DragUpdated", true)]
		dragUpdated,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use DragPerform instead (UnityUpgradable) -> DragPerform", true)]
		dragPerform,
		[Obsolete("Use Ignore instead (UnityUpgradable) -> Ignore", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		ignore,
		[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		used
	}
}
