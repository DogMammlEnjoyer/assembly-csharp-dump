using System;
using UnityEngine;

namespace Modio.Unity.UI.Components.Selectables.Transitions
{
	[Serializable]
	public class SelectableTransitionActive : ISelectableTransition
	{
		public void OnSelectionStateChanged(IModioUISelectable.SelectionState state, bool instant)
		{
			if (this._target == null)
			{
				return;
			}
			GameObject target = this._target;
			bool active;
			switch (state)
			{
			case IModioUISelectable.SelectionState.Normal:
				active = this._normal;
				break;
			case IModioUISelectable.SelectionState.Highlighted:
				active = this._highlighted;
				break;
			case IModioUISelectable.SelectionState.Pressed:
				active = this._pressed;
				break;
			case IModioUISelectable.SelectionState.Selected:
				active = this._selected;
				break;
			case IModioUISelectable.SelectionState.Disabled:
				active = this._disabled;
				break;
			default:
				active = this._normal;
				break;
			}
			target.SetActive(active);
		}

		[SerializeField]
		private GameObject _target;

		[SerializeField]
		private bool _normal;

		[SerializeField]
		private bool _highlighted;

		[SerializeField]
		private bool _pressed;

		[SerializeField]
		private bool _selected;

		[SerializeField]
		private bool _disabled;
	}
}
