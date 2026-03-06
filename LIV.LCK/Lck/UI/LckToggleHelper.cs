using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckToggleHelper : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this._lckToggle.IsDisabled)
			{
				return;
			}
			this._lckToggle.OnPointerEnter(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (this._lckToggle.IsDisabled)
			{
				return;
			}
			this._lckToggle.OnPointerDown(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this._lckToggle.IsDisabled)
			{
				return;
			}
			this._lckToggle.OnPointerUp(eventData);
			this._toggle.isOn = true;
			this._lckToggle.SetToggleVisualsOn();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this._lckToggle.IsDisabled)
			{
				return;
			}
			this._lckToggle.OnPointerExit(eventData);
		}

		[SerializeField]
		private LckToggle _lckToggle;

		[SerializeField]
		private Toggle _toggle;
	}
}
