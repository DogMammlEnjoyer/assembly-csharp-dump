using System;

namespace Modio.Unity.UI.Components.Selectables
{
	public interface IModioUISelectable
	{
		event IModioUISelectable.SelectableStateChangeDelegate StateChanged;

		IModioUISelectable.SelectionState State { get; }

		public enum SelectionState
		{
			Normal,
			Highlighted,
			Pressed,
			Selected,
			Disabled
		}

		public delegate void SelectableStateChangeDelegate(IModioUISelectable.SelectionState state, bool instant);
	}
}
