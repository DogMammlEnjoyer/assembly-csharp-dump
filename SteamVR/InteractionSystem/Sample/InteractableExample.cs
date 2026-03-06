using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	[RequireComponent(typeof(Interactable))]
	public class InteractableExample : MonoBehaviour
	{
		private void Awake()
		{
			TextMesh[] componentsInChildren = base.GetComponentsInChildren<TextMesh>();
			this.generalText = componentsInChildren[0];
			this.hoveringText = componentsInChildren[1];
			this.generalText.text = "No Hand Hovering";
			this.hoveringText.text = "Hovering: False";
			this.interactable = base.GetComponent<Interactable>();
		}

		private void OnHandHoverBegin(Hand hand)
		{
			this.generalText.text = "Hovering hand: " + hand.name;
		}

		private void OnHandHoverEnd(Hand hand)
		{
			this.generalText.text = "No Hand Hovering";
		}

		private void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting(GrabTypes.None);
			bool flag = hand.IsGrabEnding(base.gameObject);
			if (this.interactable.attachedToHand == null && grabStarting != GrabTypes.None)
			{
				this.oldPosition = base.transform.position;
				this.oldRotation = base.transform.rotation;
				hand.HoverLock(this.interactable);
				hand.AttachObject(base.gameObject, grabStarting, this.attachmentFlags, null);
				return;
			}
			if (flag)
			{
				hand.DetachObject(base.gameObject, true);
				hand.HoverUnlock(this.interactable);
				base.transform.position = this.oldPosition;
				base.transform.rotation = this.oldRotation;
			}
		}

		private void OnAttachedToHand(Hand hand)
		{
			this.generalText.text = string.Format("Attached: {0}", hand.name);
			this.attachTime = Time.time;
		}

		private void OnDetachedFromHand(Hand hand)
		{
			this.generalText.text = string.Format("Detached: {0}", hand.name);
		}

		private void HandAttachedUpdate(Hand hand)
		{
			this.generalText.text = string.Format("Attached: {0} :: Time: {1:F2}", hand.name, Time.time - this.attachTime);
		}

		private void Update()
		{
			if (this.interactable.isHovering != this.lastHovering)
			{
				this.hoveringText.text = string.Format("Hovering: {0}", this.interactable.isHovering);
				this.lastHovering = this.interactable.isHovering;
			}
		}

		private void OnHandFocusAcquired(Hand hand)
		{
		}

		private void OnHandFocusLost(Hand hand)
		{
		}

		private TextMesh generalText;

		private TextMesh hoveringText;

		private Vector3 oldPosition;

		private Quaternion oldRotation;

		private float attachTime;

		private Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;

		private Interactable interactable;

		private bool lastHovering;
	}
}
