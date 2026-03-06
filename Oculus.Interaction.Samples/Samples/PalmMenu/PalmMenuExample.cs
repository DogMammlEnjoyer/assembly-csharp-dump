using System;
using UnityEngine;

namespace Oculus.Interaction.Samples.PalmMenu
{
	public class PalmMenuExample : MonoBehaviour
	{
		private void Start()
		{
			this._currentSelectedButtonIdx = this.CalculateNearestButtonIdx();
			this._selectionIndicatorDot.position = this._paginationDots[this._currentSelectedButtonIdx].position;
		}

		private void Update()
		{
			int num = this.CalculateNearestButtonIdx();
			if (num != this._currentSelectedButtonIdx)
			{
				this._currentSelectedButtonIdx = num;
				this._paginationSwipeAudio.Play();
				this._selectionIndicatorDot.position = this._paginationDots[this._currentSelectedButtonIdx].position;
			}
			if (this._menuInteractable.State != InteractableState.Select)
			{
				this.LerpToButton();
			}
		}

		private int CalculateNearestButtonIdx()
		{
			int result = 0;
			float num = float.PositiveInfinity;
			for (int i = 0; i < this._buttons.Length; i++)
			{
				float num2 = this._buttons[i].localPosition.x + this._menuPanel.anchoredPosition.x;
				int num3 = (num2 < 0f) ? (i + 1) : (i - 1);
				float num4 = Mathf.Abs(num2);
				if (num4 < num)
				{
					result = i;
					num = num4;
				}
				float num5 = this._defaultButtonDistance;
				if (num3 >= 0 && num3 < this._buttons.Length)
				{
					num5 = Mathf.Abs(this._buttons[num3].localPosition.x - this._buttons[i].localPosition.x);
				}
				float d = this._paginationButtonScaleCurve.Evaluate(num4 / num5);
				this._buttons[i].localScale = d * Vector3.one;
			}
			return result;
		}

		private void LerpToButton()
		{
			float num = this._buttons[0].localPosition.x;
			float num2 = Mathf.Abs(num + this._menuPanel.anchoredPosition.x);
			for (int i = 1; i < this._buttons.Length; i++)
			{
				float x = this._buttons[i].localPosition.x;
				float num3 = Mathf.Abs(x + this._menuPanel.anchoredPosition.x);
				if (num3 < num2)
				{
					num = x;
					num2 = num3;
				}
			}
			this._menuPanel.anchoredPosition = Vector2.Lerp(this._menuPanel.anchoredPosition, new Vector2(-num, 0f), 0.2f);
		}

		public void ToggleMenu()
		{
			if (this._menuParent.activeSelf)
			{
				this._hideMenuAudio.Play();
				this._menuParent.SetActive(false);
				return;
			}
			this._showMenuAudio.Play();
			this._menuParent.SetActive(true);
		}

		[SerializeField]
		private PokeInteractable _menuInteractable;

		[SerializeField]
		private GameObject _menuParent;

		[SerializeField]
		private RectTransform _menuPanel;

		[SerializeField]
		private RectTransform[] _buttons;

		[SerializeField]
		private RectTransform[] _paginationDots;

		[SerializeField]
		private RectTransform _selectionIndicatorDot;

		[SerializeField]
		private AnimationCurve _paginationButtonScaleCurve;

		[SerializeField]
		private float _defaultButtonDistance = 50f;

		[SerializeField]
		private AudioSource _paginationSwipeAudio;

		[SerializeField]
		private AudioSource _showMenuAudio;

		[SerializeField]
		private AudioSource _hideMenuAudio;

		private int _currentSelectedButtonIdx;
	}
}
