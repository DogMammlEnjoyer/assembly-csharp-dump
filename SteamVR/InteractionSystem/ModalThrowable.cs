using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ModalThrowable : Throwable
	{
		protected override void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting(GrabTypes.None);
			if (grabStarting != GrabTypes.None)
			{
				if (grabStarting == GrabTypes.Pinch)
				{
					hand.AttachObject(base.gameObject, grabStarting, this.attachmentFlags, this.pinchOffset);
				}
				else if (grabStarting == GrabTypes.Grip)
				{
					hand.AttachObject(base.gameObject, grabStarting, this.attachmentFlags, this.gripOffset);
				}
				else
				{
					hand.AttachObject(base.gameObject, grabStarting, this.attachmentFlags, this.attachmentOffset);
				}
				hand.HideGrabHint();
			}
		}

		protected override void HandAttachedUpdate(Hand hand)
		{
			if (this.interactable.skeletonPoser != null)
			{
				this.interactable.skeletonPoser.SetBlendingBehaviourEnabled("PinchPose", hand.currentAttachedObjectInfo.Value.grabbedWithType == GrabTypes.Pinch);
			}
			base.HandAttachedUpdate(hand);
		}

		[Tooltip("The local point which acts as a positional and rotational offset to use while held with a grip type grab")]
		public Transform gripOffset;

		[Tooltip("The local point which acts as a positional and rotational offset to use while held with a pinch type grab")]
		public Transform pinchOffset;
	}
}
