using System;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables
{
	public class ModioUIButton : Button, IModioUISelectable
	{
		public event IModioUISelectable.SelectableStateChangeDelegate StateChanged;

		public IModioUISelectable.SelectionState State { get; private set; }

		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			this.State = (IModioUISelectable.SelectionState)state;
			IModioUISelectable.SelectableStateChangeDelegate stateChanged = this.StateChanged;
			if (stateChanged == null)
			{
				return;
			}
			stateChanged(this.State, instant);
		}

		public void DoVisualOnlyStateTransition(IModioUISelectable.SelectionState state, bool instant)
		{
			this.DoStateTransition((Selectable.SelectionState)state, instant);
		}
	}
}
