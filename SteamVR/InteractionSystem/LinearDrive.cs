using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class LinearDrive : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.mappingChangeSamples = new float[this.numMappingChangeSamples];
			this.interactable = base.GetComponent<Interactable>();
		}

		protected virtual void Start()
		{
			if (this.linearMapping == null)
			{
				this.linearMapping = base.GetComponent<LinearMapping>();
			}
			if (this.linearMapping == null)
			{
				this.linearMapping = base.gameObject.AddComponent<LinearMapping>();
			}
			this.initialMappingOffset = this.linearMapping.value;
			if (this.repositionGameObject)
			{
				this.UpdateLinearMapping(base.transform);
			}
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting(GrabTypes.None);
			if (this.interactable.attachedToHand == null && grabStarting != GrabTypes.None)
			{
				this.initialMappingOffset = this.linearMapping.value - this.CalculateLinearMapping(hand.transform);
				this.sampleCount = 0;
				this.mappingChangeRate = 0f;
				hand.AttachObject(base.gameObject, grabStarting, this.attachmentFlags, null);
			}
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			this.UpdateLinearMapping(hand.transform);
			if (hand.IsGrabEnding(base.gameObject))
			{
				hand.DetachObject(base.gameObject, true);
			}
		}

		protected virtual void OnDetachedFromHand(Hand hand)
		{
			this.CalculateMappingChangeRate();
		}

		protected void CalculateMappingChangeRate()
		{
			this.mappingChangeRate = 0f;
			int num = Mathf.Min(this.sampleCount, this.mappingChangeSamples.Length);
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.mappingChangeRate += this.mappingChangeSamples[i];
				}
				this.mappingChangeRate /= (float)num;
			}
		}

		protected void UpdateLinearMapping(Transform updateTransform)
		{
			this.prevMapping = this.linearMapping.value;
			this.linearMapping.value = Mathf.Clamp01(this.initialMappingOffset + this.CalculateLinearMapping(updateTransform));
			this.mappingChangeSamples[this.sampleCount % this.mappingChangeSamples.Length] = 1f / Time.deltaTime * (this.linearMapping.value - this.prevMapping);
			this.sampleCount++;
			if (this.repositionGameObject)
			{
				base.transform.position = Vector3.Lerp(this.startPosition.position, this.endPosition.position, this.linearMapping.value);
			}
		}

		protected float CalculateLinearMapping(Transform updateTransform)
		{
			Vector3 rhs = this.endPosition.position - this.startPosition.position;
			float magnitude = rhs.magnitude;
			rhs.Normalize();
			return Vector3.Dot(updateTransform.position - this.startPosition.position, rhs) / magnitude;
		}

		protected virtual void Update()
		{
			if (this.maintainMomemntum && this.mappingChangeRate != 0f)
			{
				this.mappingChangeRate = Mathf.Lerp(this.mappingChangeRate, 0f, this.momemtumDampenRate * Time.deltaTime);
				this.linearMapping.value = Mathf.Clamp01(this.linearMapping.value + this.mappingChangeRate * Time.deltaTime);
				if (this.repositionGameObject)
				{
					base.transform.position = Vector3.Lerp(this.startPosition.position, this.endPosition.position, this.linearMapping.value);
				}
			}
		}

		public Transform startPosition;

		public Transform endPosition;

		public LinearMapping linearMapping;

		public bool repositionGameObject = true;

		public bool maintainMomemntum = true;

		public float momemtumDampenRate = 5f;

		protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;

		protected float initialMappingOffset;

		protected int numMappingChangeSamples = 5;

		protected float[] mappingChangeSamples;

		protected float prevMapping;

		protected float mappingChangeRate;

		protected int sampleCount;

		protected Interactable interactable;
	}
}
