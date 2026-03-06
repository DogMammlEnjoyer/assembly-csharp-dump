using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.Selectables.Transitions
{
	[Serializable]
	public class SelectableTransitionColorTint : ISelectableTransition
	{
		public void OnSelectionStateChanged(IModioUISelectable.SelectionState state, bool instant)
		{
			if (this._target == null)
			{
				return;
			}
			Color color;
			switch (state)
			{
			case IModioUISelectable.SelectionState.Normal:
				color = this._colorBlock.normalColor;
				break;
			case IModioUISelectable.SelectionState.Highlighted:
				color = this._colorBlock.highlightedColor;
				break;
			case IModioUISelectable.SelectionState.Pressed:
				color = this._colorBlock.pressedColor;
				break;
			case IModioUISelectable.SelectionState.Selected:
				color = this._colorBlock.selectedColor;
				break;
			case IModioUISelectable.SelectionState.Disabled:
				color = this._colorBlock.disabledColor;
				break;
			default:
				color = this._colorBlock.normalColor;
				break;
			}
			Color color2 = color;
			if (this._target.gameObject.activeInHierarchy)
			{
				if (this._coroutine != null)
				{
					this._target.StopCoroutine(this._coroutine);
				}
				this._coroutine = this._target.StartCoroutine(this.CrossFadeColor(color2, (!instant) ? this._colorBlock.fadeDuration : 0f));
				return;
			}
			this._target.color = color2;
		}

		private IEnumerator CrossFadeColor(Color targetColor, float duration)
		{
			Color startColor = this._target.color;
			for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
			{
				this._target.color = Color.Lerp(startColor, targetColor, t);
				yield return null;
			}
			this._target.color = targetColor;
			this._coroutine = null;
			yield break;
		}

		[SerializeField]
		private Graphic _target;

		[SerializeField]
		private ColorBlock _colorBlock = ColorBlock.defaultColorBlock;

		private Coroutine _coroutine;
	}
}
