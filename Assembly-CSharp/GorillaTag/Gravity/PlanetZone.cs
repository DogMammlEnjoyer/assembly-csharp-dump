using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class PlanetZone : BasicGravityZone
	{
		protected override void Awake()
		{
			base.Awake();
			this.sqrDistance = this.rotationDistance * this.rotationDistance;
		}

		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return worldPosition - base.transform.position;
		}

		protected override bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			if (this.alwaysRotate)
			{
				return true;
			}
			if (this.rotateTarget)
			{
				Vector3 vector = offsetFromGravity;
				return vector.sqrMagnitude < this.sqrDistance;
			}
			return false;
		}

		[Tooltip("how close to the center of the zone to enable rotating the player")]
		[SerializeField]
		protected float rotationDistance;

		[Tooltip("if enabled, always rotates the player")]
		[SerializeField]
		protected bool alwaysRotate = true;

		private float sqrDistance;
	}
}
