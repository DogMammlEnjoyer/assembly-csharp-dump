using System;

namespace UnityEngine.VFX.Utility
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	internal class VFXRigidBodyCollisionEventBinder : VFXEventBinderBase
	{
		protected override void SetEventAttribute(object[] parameters)
		{
			ContactPoint contactPoint = (ContactPoint)parameters[0];
			this.eventAttribute.SetVector3(this.positionParameter, contactPoint.point);
			this.eventAttribute.SetVector3(this.directionParameter, contactPoint.normal);
		}

		private void OnCollisionEnter(Collision collision)
		{
			foreach (ContactPoint contactPoint in collision.contacts)
			{
				base.SendEventToVisualEffect(new object[]
				{
					contactPoint
				});
			}
		}

		private ExposedProperty positionParameter = "position";

		private ExposedProperty directionParameter = "velocity";
	}
}
