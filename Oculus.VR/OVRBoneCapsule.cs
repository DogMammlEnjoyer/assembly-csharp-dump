using System;
using UnityEngine;

public class OVRBoneCapsule
{
	public short BoneIndex { get; set; }

	public Rigidbody CapsuleRigidbody { get; set; }

	public CapsuleCollider CapsuleCollider { get; set; }

	public OVRBoneCapsule()
	{
	}

	public OVRBoneCapsule(short boneIndex, Rigidbody capsuleRigidBody, CapsuleCollider capsuleCollider)
	{
		this.BoneIndex = boneIndex;
		this.CapsuleRigidbody = capsuleRigidBody;
		this.CapsuleCollider = capsuleCollider;
	}

	public void Cleanup()
	{
		if (this.CapsuleRigidbody != null)
		{
			Object.Destroy(this.CapsuleRigidbody.gameObject);
		}
		this.CapsuleRigidbody = null;
		this.CapsuleCollider = null;
	}
}
