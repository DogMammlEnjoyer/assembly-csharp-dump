using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class LockToPoint : MonoBehaviour
	{
		private void Start()
		{
			this.interactable = base.GetComponent<Interactable>();
			this.body = base.GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			bool flag = false;
			if (this.interactable != null)
			{
				flag = this.interactable.attachedToHand;
			}
			if (flag)
			{
				this.body.isKinematic = false;
				this.dropTimer = -1f;
				return;
			}
			this.dropTimer += Time.deltaTime / (this.snapTime / 2f);
			this.body.isKinematic = (this.dropTimer > 1f);
			if (this.dropTimer > 1f)
			{
				base.transform.position = this.snapTo.position;
				base.transform.rotation = this.snapTo.rotation;
				return;
			}
			float num = Mathf.Pow(35f, this.dropTimer);
			this.body.linearVelocity = Vector3.Lerp(this.body.linearVelocity, Vector3.zero, Time.fixedDeltaTime * 4f);
			if (this.body.useGravity)
			{
				this.body.AddForce(-Physics.gravity);
			}
			base.transform.position = Vector3.Lerp(base.transform.position, this.snapTo.position, Time.fixedDeltaTime * num * 3f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.snapTo.rotation, Time.fixedDeltaTime * num * 2f);
		}

		public Transform snapTo;

		private Rigidbody body;

		public float snapTime = 2f;

		private float dropTimer;

		private Interactable interactable;
	}
}
