using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Pun.UtilityScripts
{
	public class ButtonInsideScrollList : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		private void Start()
		{
			this.scrollRect = base.GetComponentInParent<ScrollRect>();
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (this.scrollRect != null)
			{
				this.scrollRect.StopMovement();
				this.scrollRect.enabled = false;
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (this.scrollRect != null && !this.scrollRect.enabled)
			{
				this.scrollRect.enabled = true;
			}
		}

		private ScrollRect scrollRect;
	}
}
