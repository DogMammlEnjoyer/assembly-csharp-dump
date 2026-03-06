using System;
using UnityEngine.EventSystems;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class PointerHandler : UIBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public InteractableController Controller { get; set; }

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!PanelInputModule.Processing)
			{
				return;
			}
			if (this.Controller != null)
			{
				this.Controller.OnPointerClick();
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!PanelInputModule.Processing)
			{
				return;
			}
			if (this.Controller != null)
			{
				this.Controller.OnPointerEnter();
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (!PanelInputModule.Processing)
			{
				return;
			}
			if (this.Controller != null)
			{
				this.Controller.OnPointerExit();
			}
		}
	}
}
