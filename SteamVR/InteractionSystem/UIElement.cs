using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class UIElement : MonoBehaviour
	{
		protected virtual void Awake()
		{
			Button component = base.GetComponent<Button>();
			if (component)
			{
				component.onClick.AddListener(new UnityAction(this.OnButtonClick));
			}
		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			this.currentHand = hand;
			InputModule.instance.HoverBegin(base.gameObject);
			ControllerButtonHints.ShowButtonHint(hand, new ISteamVR_Action_In_Source[]
			{
				hand.uiInteractAction
			});
		}

		protected virtual void OnHandHoverEnd(Hand hand)
		{
			InputModule.instance.HoverEnd(base.gameObject);
			ControllerButtonHints.HideButtonHint(hand, new ISteamVR_Action_In_Source[]
			{
				hand.uiInteractAction
			});
			this.currentHand = null;
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			if (hand.uiInteractAction != null && hand.uiInteractAction.GetStateDown(hand.handType))
			{
				InputModule.instance.Submit(base.gameObject);
				ControllerButtonHints.HideButtonHint(hand, new ISteamVR_Action_In_Source[]
				{
					hand.uiInteractAction
				});
			}
		}

		protected virtual void OnButtonClick()
		{
			this.onHandClick.Invoke(this.currentHand);
		}

		public CustomEvents.UnityEventHand onHandClick;

		protected Hand currentHand;
	}
}
