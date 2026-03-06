using System;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables
{
	public class ModioUIToggle : Toggle, IModioUISelectable
	{
		public event IModioUISelectable.SelectableStateChangeDelegate StateChanged;

		public IModioUISelectable.SelectionState State { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			this.onValueChanged.AddListener(delegate(bool value)
			{
				this.DoStateTransition((Selectable.SelectionState)this.State, false);
			});
		}

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

		public void FakeClicked()
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			base.isOn = !base.isOn;
		}
	}
}
