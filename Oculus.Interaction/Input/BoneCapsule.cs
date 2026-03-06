using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class BoneCapsule
	{
		public HandJointId StartJoint { get; private set; }

		public HandJointId EndJoint { get; private set; }

		public Rigidbody CapsuleRigidbody { get; private set; }

		public CapsuleCollider CapsuleCollider { get; private set; }

		public BoneCapsule(HandJointId fromJoint, HandJointId toJoint, Rigidbody body, CapsuleCollider collider)
		{
			this.StartJoint = fromJoint;
			this.EndJoint = toJoint;
			this.CapsuleRigidbody = body;
			this.CapsuleCollider = collider;
		}
	}
}
