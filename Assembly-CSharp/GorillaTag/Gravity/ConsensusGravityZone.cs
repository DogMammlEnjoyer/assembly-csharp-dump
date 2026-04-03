using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class ConsensusGravityZone : BasicGravityZone
	{
		protected override void Awake()
		{
			base.Awake();
			this.zoneCollider = base.GetComponent<Collider>();
		}

		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return base.transform.TransformVector(new Vector3(Mathf.Sin(this.currentRot * 0.017453292f), Mathf.Cos(this.currentRot * 0.017453292f), 0f));
		}

		private void FixedUpdate()
		{
			Vector3 a = Vector3.zero;
			int num = 0;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				Vector3 position = rigContainer.Rig.transform.position;
				if (this.zoneCollider.bounds.Contains(position))
				{
					a += position;
					num++;
				}
			}
			if (num > 0)
			{
				Vector3 position2 = a / (float)num;
				Vector3 vector = base.transform.InverseTransformPoint(position2);
				this.idealRot = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
			}
			float num2 = (this.idealRot - this.currentRot) * this.weightForce - this.currentRot * this.centeringForce;
			this.rotSpeed += num2 * Time.fixedDeltaTime;
			this.rotSpeed *= this.drag;
			this.currentRot += this.rotSpeed * Time.fixedDeltaTime;
			if (this.currentRot < this.rotMin)
			{
				this.rotSpeed = 0f;
				this.currentRot = this.rotMin;
				return;
			}
			if (this.currentRot > this.rotMax)
			{
				this.rotSpeed = 0f;
				this.currentRot = this.rotMax;
			}
		}

		protected override bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			return true;
		}

		private Collider zoneCollider;

		private float currentRot;

		private float idealRot;

		private float rotSpeed;

		[SerializeField]
		private float weightForce;

		[SerializeField]
		private float centeringForce;

		[SerializeField]
		private float drag;

		[SerializeField]
		private float rotMin = -45f;

		[SerializeField]
		private float rotMax = 45f;
	}
}
