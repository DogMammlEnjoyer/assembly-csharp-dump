using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OVR.Input
{
	public class UIButtonSquishHelper : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerExitHandler, IPointerUpHandler, IPointerEnterHandler
	{
		private void Start()
		{
			this._originalScale = base.transform.localScale;
			this._button = base.GetComponent<Button>();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!this._button || this._button.interactable)
			{
				base.transform.localScale = this._originalScale * 1.05f;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!this._button || this._button.interactable)
			{
				base.transform.localScale = this._originalScale * 1.1f;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!this._button || this._button.interactable)
			{
				base.transform.localScale = this._originalScale;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!this._button || this._button.interactable)
			{
				base.transform.localScale = this._originalScale;
			}
		}

		private const float _squishAmount = 1.1f;

		private const float _highlightAmount = 1.05f;

		private Vector3 _originalScale;

		private Button _button;
	}
}
