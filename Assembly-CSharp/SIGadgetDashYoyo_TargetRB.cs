using System;
using UnityEngine;

public class SIGadgetDashYoyo_TargetRB : MonoBehaviour
{
	protected void OnEnable()
	{
	}

	protected void OnTriggerEnter(Collider otherCollider)
	{
		if (base.isActiveAndEnabled && this.gadget.gameEntity.IsAuthority() && (this.gadget.gameEntity.heldByActorNumber != -1 || this.gadget.gameEntity.snappedByActorNumber != -1) && (otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) || otherCollider.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider)) && !ApplicationQuittingState.IsQuitting)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				VRRig componentInParent = otherCollider.GetComponentInParent<VRRig>();
				if (componentInParent == null)
				{
					return;
				}
				NetPlayer creator = componentInParent.creator;
				if (creator == null)
				{
					return;
				}
				if (SuperInfectionManager.GetSIManagerForZone(this.gadget.gameEntity.manager.zone) == null)
				{
					return;
				}
				this.gadget.OnHitPlayer_Authority(superInfectionGame, creator);
				return;
			}
		}
	}

	[SerializeField]
	private SIGadgetDashYoyo gadget;
}
