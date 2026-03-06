using System;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables.Transitions
{
	[Serializable]
	public class SelectableTransitionAnimation : ISelectableTransition
	{
		public void OnSelectionStateChanged(IModioUISelectable.SelectionState state, bool instant)
		{
			if (this._target == null || !this._target.isActiveAndEnabled || !this._target.hasBoundPlayables)
			{
				return;
			}
			string text;
			switch (state)
			{
			case IModioUISelectable.SelectionState.Normal:
				text = this._animationTriggers.normalTrigger;
				break;
			case IModioUISelectable.SelectionState.Highlighted:
				text = this._animationTriggers.highlightedTrigger;
				break;
			case IModioUISelectable.SelectionState.Pressed:
				text = this._animationTriggers.pressedTrigger;
				break;
			case IModioUISelectable.SelectionState.Selected:
				text = this._animationTriggers.selectedTrigger;
				break;
			case IModioUISelectable.SelectionState.Disabled:
				text = this._animationTriggers.disabledTrigger;
				break;
			default:
				text = null;
				break;
			}
			string text2 = text;
			if (string.IsNullOrEmpty(text2))
			{
				return;
			}
			this._target.ResetTrigger(this._animationTriggers.normalTrigger);
			this._target.ResetTrigger(this._animationTriggers.highlightedTrigger);
			this._target.ResetTrigger(this._animationTriggers.pressedTrigger);
			this._target.ResetTrigger(this._animationTriggers.selectedTrigger);
			this._target.ResetTrigger(this._animationTriggers.disabledTrigger);
			this._target.SetTrigger(text2);
		}

		[SerializeField]
		private Animator _target;

		[SerializeField]
		private AnimationTriggers _animationTriggers;
	}
}
