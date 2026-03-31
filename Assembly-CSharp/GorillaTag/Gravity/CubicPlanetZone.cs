using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class CubicPlanetZone : PlanetZone
	{
		private void UpdateConstraint()
		{
			float num = Mathf.Abs(this.constraints.x * 0.5f);
			float num2 = Mathf.Abs(this.constraints.y * 0.5f);
			float num3 = Mathf.Abs(this.constraints.z * 0.5f);
			this.minConstraints.x = num * -1f;
			this.minConstraints.y = num2 * -1f;
			this.minConstraints.z = num3 * -1f;
			this.maxConstraints.x = num;
			this.maxConstraints.y = num2;
			this.maxConstraints.z = num3;
		}

		protected override void Awake()
		{
			base.Awake();
			this.inverseRotation = Quaternion.Inverse(base.transform.rotation);
			this.UpdateConstraint();
		}

		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return worldPosition - this.GetPointOnBounds(worldPosition);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetPointOnBounds(in Vector3 point)
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Vector3 vector = this.inverseRotation * (point - position);
			float x = vector.x;
			float y = vector.y;
			float z = vector.z;
			if (x <= this.maxConstraints.x && x >= this.minConstraints.x && y <= this.maxConstraints.y && y >= this.minConstraints.y && z <= this.maxConstraints.z && z >= this.minConstraints.z)
			{
				Vector3 vector2 = new Vector3((x > 0f) ? this.maxConstraints.x : this.minConstraints.x, (y > 0f) ? this.maxConstraints.y : this.minConstraints.y, (z > 0f) ? this.maxConstraints.z : this.minConstraints.z);
				float num = Mathf.Abs(vector2.x - x);
				float num2 = Mathf.Abs(vector2.y - y);
				float num3 = Mathf.Abs(vector2.z - z);
				Vector3 vector3 = new Vector3((num <= num2 && num <= num3) ? 1f : 0f, (num2 <= num && num2 <= num3) ? 1f : 0f, (num3 <= num && num3 <= num2) ? 1f : 0f);
				Vector3 vector4 = vector.MultiplyBy(vector3);
				Vector3 vector5 = vector2.MultiplyBy(vector3);
				float magnitude = vector4.magnitude;
				float magnitude2 = vector5.magnitude;
				float num4 = 1f / magnitude2;
				float num5 = magnitude * num4;
				num5 *= 0.99f;
				return position + transform.rotation * vector.Clamp(this.minConstraints * num5, this.maxConstraints * num5);
			}
			return position + transform.rotation * vector.Clamp(this.minConstraints, this.maxConstraints);
		}

		[Header("box constraint for where gravity center can be")]
		[SerializeField]
		protected Vector3 constraints;

		[SerializeField]
		protected Vector3 minConstraints;

		[SerializeField]
		protected Vector3 maxConstraints;

		protected Quaternion inverseRotation;
	}
}
