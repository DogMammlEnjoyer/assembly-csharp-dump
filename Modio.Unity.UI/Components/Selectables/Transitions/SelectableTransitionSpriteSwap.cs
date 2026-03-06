using System;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables.Transitions
{
	[Serializable]
	public class SelectableTransitionSpriteSwap : ISelectableTransition
	{
		public void OnSelectionStateChanged(IModioUISelectable.SelectionState state, bool instant)
		{
			if (this._target == null)
			{
				return;
			}
			if (!this._isInitialised)
			{
				this._defaultSprite = this._target.sprite;
			}
			this._isInitialised = true;
			Image target = this._target;
			Sprite sprite;
			switch (state)
			{
			case IModioUISelectable.SelectionState.Normal:
				sprite = ((this._overrideDefault != null) ? this._overrideDefault : this._defaultSprite);
				break;
			case IModioUISelectable.SelectionState.Highlighted:
				sprite = this._spriteState.highlightedSprite;
				break;
			case IModioUISelectable.SelectionState.Pressed:
				sprite = this._spriteState.pressedSprite;
				break;
			case IModioUISelectable.SelectionState.Selected:
				sprite = this._spriteState.selectedSprite;
				break;
			case IModioUISelectable.SelectionState.Disabled:
				sprite = this._spriteState.disabledSprite;
				break;
			default:
				sprite = this._defaultSprite;
				break;
			}
			target.sprite = sprite;
		}

		[SerializeField]
		private Image _target;

		[SerializeField]
		private SpriteState _spriteState;

		[SerializeField]
		private Sprite _overrideDefault;

		private bool _isInitialised;

		private Sprite _defaultSprite;
	}
}
