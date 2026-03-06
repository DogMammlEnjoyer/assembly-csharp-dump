using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
		private void Start()
		{
			this.hand = base.GetComponentInParent<Hand>();
		}

		private void Update()
		{
			if (this.itemPrefab != null && this.hand.isActive && this.hand.isPoseValid)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.itemPrefab);
				gameObject.SetActive(true);
				this.hand.AttachObject(gameObject, GrabTypes.Scripted, Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic, null);
				this.hand.TriggerHapticPulse(800);
				Object.Destroy(base.gameObject);
				gameObject.transform.localScale = this.itemPrefab.transform.localScale;
			}
		}

		private Hand hand;

		public GameObject itemPrefab;
	}
}
