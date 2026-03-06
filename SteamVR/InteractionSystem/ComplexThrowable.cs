using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class ComplexThrowable : MonoBehaviour
	{
		private void Awake()
		{
			base.GetComponentsInChildren<Rigidbody>(this.rigidBodies);
		}

		private void Update()
		{
			for (int i = 0; i < this.holdingHands.Count; i++)
			{
				if (this.holdingHands[i].IsGrabEnding(base.gameObject))
				{
					this.PhysicsDetach(this.holdingHands[i]);
				}
			}
		}

		private void OnHandHoverBegin(Hand hand)
		{
			if (this.holdingHands.IndexOf(hand) == -1 && hand.isActive)
			{
				hand.TriggerHapticPulse(800);
			}
		}

		private void OnHandHoverEnd(Hand hand)
		{
			if (this.holdingHands.IndexOf(hand) == -1 && hand.isActive)
			{
				hand.TriggerHapticPulse(500);
			}
		}

		private void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting(GrabTypes.None);
			if (grabStarting != GrabTypes.None)
			{
				this.PhysicsAttach(hand, grabStarting);
			}
		}

		private void PhysicsAttach(Hand hand, GrabTypes startingGrabType)
		{
			this.PhysicsDetach(hand);
			Rigidbody rigidbody = null;
			Vector3 item = Vector3.zero;
			float num = float.MaxValue;
			for (int i = 0; i < this.rigidBodies.Count; i++)
			{
				float num2 = Vector3.Distance(this.rigidBodies[i].worldCenterOfMass, hand.transform.position);
				if (num2 < num)
				{
					rigidbody = this.rigidBodies[i];
					num = num2;
				}
			}
			if (rigidbody == null)
			{
				return;
			}
			if (this.attachMode == ComplexThrowable.AttachMode.FixedJoint)
			{
				Util.FindOrAddComponent<Rigidbody>(hand.gameObject).isKinematic = true;
				hand.gameObject.AddComponent<FixedJoint>().connectedBody = rigidbody;
			}
			hand.HoverLock(null);
			Vector3 b = hand.transform.position - rigidbody.worldCenterOfMass;
			b = Mathf.Min(b.magnitude, 1f) * b.normalized;
			item = rigidbody.transform.InverseTransformPoint(rigidbody.worldCenterOfMass + b);
			hand.AttachObject(base.gameObject, startingGrabType, this.attachmentFlags, null);
			this.holdingHands.Add(hand);
			this.holdingBodies.Add(rigidbody);
			this.holdingPoints.Add(item);
		}

		private bool PhysicsDetach(Hand hand)
		{
			int num = this.holdingHands.IndexOf(hand);
			if (num != -1)
			{
				this.holdingHands[num].DetachObject(base.gameObject, false);
				this.holdingHands[num].HoverUnlock(null);
				if (this.attachMode == ComplexThrowable.AttachMode.FixedJoint)
				{
					Object.Destroy(this.holdingHands[num].GetComponent<FixedJoint>());
				}
				Util.FastRemove<Hand>(this.holdingHands, num);
				Util.FastRemove<Rigidbody>(this.holdingBodies, num);
				Util.FastRemove<Vector3>(this.holdingPoints, num);
				return true;
			}
			return false;
		}

		private void FixedUpdate()
		{
			if (this.attachMode == ComplexThrowable.AttachMode.Force)
			{
				for (int i = 0; i < this.holdingHands.Count; i++)
				{
					Vector3 vector = this.holdingBodies[i].transform.TransformPoint(this.holdingPoints[i]);
					Vector3 a = this.holdingHands[i].transform.position - vector;
					this.holdingBodies[i].AddForceAtPosition(this.attachForce * a * this.holdingBodies[i].mass, vector, ForceMode.Force);
					this.holdingBodies[i].AddForceAtPosition(-this.attachForceDamper * this.holdingBodies[i].GetPointVelocity(vector) * this.holdingBodies[i].mass, vector, ForceMode.Force);
				}
			}
		}

		public float attachForce = 800f;

		public float attachForceDamper = 25f;

		public ComplexThrowable.AttachMode attachMode;

		[EnumFlags]
		public Hand.AttachmentFlags attachmentFlags;

		private List<Hand> holdingHands = new List<Hand>();

		private List<Rigidbody> holdingBodies = new List<Rigidbody>();

		private List<Vector3> holdingPoints = new List<Vector3>();

		private List<Rigidbody> rigidBodies = new List<Rigidbody>();

		public enum AttachMode
		{
			FixedJoint,
			Force
		}
	}
}
