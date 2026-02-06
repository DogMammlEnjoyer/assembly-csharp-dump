using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

public class StiltRBHandFollower : MonoBehaviour
{
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.maxAngularVelocity = this.angularSpeedLimit;
	}

	private void FixedUpdate()
	{
		Vector3 a = this.targetHand.TransformPoint(this.handOffset);
		float d;
		Vector3 a2;
		(this.targetHand.TransformRotation(this.handRotOffset) * Quaternion.Inverse(this.rb.transform.rotation)).ToAngleAxis(out d, out a2);
		this.rb.linearVelocity = (a - this.rb.transform.position) / Time.fixedDeltaTime;
		this.rb.angularVelocity = a2 * d * 0.017453292f / Time.fixedDeltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	private void OnCollisionStay(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	private void OnCollisionExit(Collision collision)
	{
		this.collisions.Remove(collision.collider);
	}

	private Rigidbody rb;

	[SerializeField]
	private Transform targetHand;

	[SerializeField]
	private Vector3 handOffset;

	[SerializeField]
	private Quaternion handRotOffset = Quaternion.identity;

	[SerializeField]
	private float angularSpeedLimit;

	private Dictionary<Collider, Vector3> collisions = new Dictionary<Collider, Vector3>();
}
