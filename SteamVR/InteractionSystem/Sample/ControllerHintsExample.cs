using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class ControllerHintsExample : MonoBehaviour
	{
		public void ShowButtonHints(Hand hand)
		{
			if (this.buttonHintCoroutine != null)
			{
				base.StopCoroutine(this.buttonHintCoroutine);
			}
			this.buttonHintCoroutine = base.StartCoroutine(this.TestButtonHints(hand));
		}

		public void ShowTextHints(Hand hand)
		{
			if (this.textHintCoroutine != null)
			{
				base.StopCoroutine(this.textHintCoroutine);
			}
			this.textHintCoroutine = base.StartCoroutine(this.TestTextHints(hand));
		}

		public void DisableHints()
		{
			if (this.buttonHintCoroutine != null)
			{
				base.StopCoroutine(this.buttonHintCoroutine);
				this.buttonHintCoroutine = null;
			}
			if (this.textHintCoroutine != null)
			{
				base.StopCoroutine(this.textHintCoroutine);
				this.textHintCoroutine = null;
			}
			foreach (Hand hand in Player.instance.hands)
			{
				ControllerButtonHints.HideAllButtonHints(hand);
				ControllerButtonHints.HideAllTextHints(hand);
			}
		}

		private IEnumerator TestButtonHints(Hand hand)
		{
			ControllerButtonHints.HideAllButtonHints(hand);
			for (;;)
			{
				int num;
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex = num + 1)
				{
					ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
					if (action.GetActive(hand.handType))
					{
						ControllerButtonHints.ShowButtonHint(hand, new ISteamVR_Action_In_Source[]
						{
							action
						});
						yield return new WaitForSeconds(1f);
						ControllerButtonHints.HideButtonHint(hand, new ISteamVR_Action_In_Source[]
						{
							action
						});
						yield return new WaitForSeconds(0.5f);
					}
					yield return null;
					action = null;
					num = actionIndex;
				}
				ControllerButtonHints.HideAllButtonHints(hand);
				yield return new WaitForSeconds(1f);
			}
			yield break;
		}

		private IEnumerator TestTextHints(Hand hand)
		{
			ControllerButtonHints.HideAllTextHints(hand);
			for (;;)
			{
				int num;
				for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex = num + 1)
				{
					ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];
					if (action.GetActive(hand.handType))
					{
						ControllerButtonHints.ShowTextHint(hand, action, action.GetShortName(), true);
						yield return new WaitForSeconds(3f);
						ControllerButtonHints.HideTextHint(hand, action);
						yield return new WaitForSeconds(0.5f);
					}
					yield return null;
					action = null;
					num = actionIndex;
				}
				ControllerButtonHints.HideAllTextHints(hand);
				yield return new WaitForSeconds(3f);
			}
			yield break;
		}

		private Coroutine buttonHintCoroutine;

		private Coroutine textHintCoroutine;
	}
}
