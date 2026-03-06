using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SpawnAndAttachToHand : MonoBehaviour
	{
		public void SpawnAndAttach(Hand passedInhand)
		{
			Hand hand = passedInhand;
			if (passedInhand == null)
			{
				hand = this.hand;
			}
			if (hand == null)
			{
				return;
			}
			GameObject objectToAttach = Object.Instantiate<GameObject>(this.prefab);
			hand.AttachObject(objectToAttach, GrabTypes.Scripted, Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic, null);
		}

		public Hand hand;

		public GameObject prefab;
	}
}
