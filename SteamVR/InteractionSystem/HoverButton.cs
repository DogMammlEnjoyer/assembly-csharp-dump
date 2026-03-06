using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class HoverButton : MonoBehaviour
	{
		private void Start()
		{
			if (this.movingPart == null && base.transform.childCount > 0)
			{
				this.movingPart = base.transform.GetChild(0);
			}
			this.startPosition = this.movingPart.localPosition;
			this.endPosition = this.startPosition + this.localMoveDistance;
			this.handEnteredPosition = this.endPosition;
		}

		private void HandHoverUpdate(Hand hand)
		{
			this.hovering = true;
			this.lastHoveredHand = hand;
			bool wasEngaged = this.engaged;
			float num = Vector3.Distance(this.movingPart.parent.InverseTransformPoint(hand.transform.position), this.endPosition);
			float num2 = Vector3.Distance(this.handEnteredPosition, this.endPosition);
			if (num > num2)
			{
				num2 = num;
				this.handEnteredPosition = this.movingPart.parent.InverseTransformPoint(hand.transform.position);
			}
			float value = num2 - num;
			float num3 = Mathf.InverseLerp(0f, this.localMoveDistance.magnitude, value);
			if (num3 > this.engageAtPercent)
			{
				this.engaged = true;
			}
			else if (num3 < this.disengageAtPercent)
			{
				this.engaged = false;
			}
			this.movingPart.localPosition = Vector3.Lerp(this.startPosition, this.endPosition, num3);
			this.InvokeEvents(wasEngaged, this.engaged);
		}

		private void LateUpdate()
		{
			if (!this.hovering)
			{
				this.movingPart.localPosition = this.startPosition;
				this.handEnteredPosition = this.endPosition;
				this.InvokeEvents(this.engaged, false);
				this.engaged = false;
			}
			this.hovering = false;
		}

		private void InvokeEvents(bool wasEngaged, bool isEngaged)
		{
			this.buttonDown = (!wasEngaged && isEngaged);
			this.buttonUp = (wasEngaged && !isEngaged);
			if (this.buttonDown && this.onButtonDown != null)
			{
				this.onButtonDown.Invoke(this.lastHoveredHand);
			}
			if (this.buttonUp && this.onButtonUp != null)
			{
				this.onButtonUp.Invoke(this.lastHoveredHand);
			}
			if (isEngaged && this.onButtonIsPressed != null)
			{
				this.onButtonIsPressed.Invoke(this.lastHoveredHand);
			}
		}

		public Transform movingPart;

		public Vector3 localMoveDistance = new Vector3(0f, -0.1f, 0f);

		[Range(0f, 1f)]
		public float engageAtPercent = 0.95f;

		[Range(0f, 1f)]
		public float disengageAtPercent = 0.9f;

		public HandEvent onButtonDown;

		public HandEvent onButtonUp;

		public HandEvent onButtonIsPressed;

		public bool engaged;

		public bool buttonDown;

		public bool buttonUp;

		private Vector3 startPosition;

		private Vector3 endPosition;

		private Vector3 handEnteredPosition;

		private bool hovering;

		private Hand lastHoveredHand;
	}
}
